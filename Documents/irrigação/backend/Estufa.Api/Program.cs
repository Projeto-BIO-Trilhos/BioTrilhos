using Estufa.Api.Data;
using Estufa.Api.Hubs;
using Estufa.Api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Configuration (set connection string in appsettings.json)
var defaultConn = builder.Configuration.GetConnectionString("DefaultConnection");
var useSqliteEnv = Environment.GetEnvironmentVariable("USE_SQLITE");
var useSqlite = string.Equals(useSqliteEnv, "true", StringComparison.OrdinalIgnoreCase) || (defaultConn != null && defaultConn.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase));
builder.Services.AddDbContext<EstufaDbContext>(options =>
{
    if (useSqlite)
    {
        var sqliteConn = defaultConn ?? "Data Source=estufa.db";
        options.UseSqlite(sqliteConn);
    }
    else
    {
        options.UseSqlServer(defaultConn);
    }
});

// Repository pattern
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
// Prefer environment variable Jwt__Key over appsettings for security
var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") ?? jwtSection.GetValue<string>("Key");
if (!string.IsNullOrWhiteSpace(jwtKey))
{
    var keyBytes = System.Text.Encoding.UTF8.GetBytes(jwtKey);
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(keyBytes),
                ValidateIssuer = true,
                ValidIssuer = jwtSection.GetValue<string>("Issuer"),
                ValidateAudience = true,
                ValidAudience = jwtSection.GetValue<string>("Audience"),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });
}

// SignalR
builder.Services.AddSignalR();

// CORS for frontend (development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
});

// Register repositories and services
builder.Services.AddScoped<ISensorRepository, SensorRepository>();
builder.Services.AddScoped<ILeituraRepository, LeituraRepository>();
builder.Services.AddScoped<IIrrigacaoRepository, IrrigacaoRepository>();
builder.Services.AddScoped<IReservatorioRepository, ReservatorioRepository>();
builder.Services.AddScoped<IMeteorologiaRepository, MeteorologiaRepository>();

builder.Services.AddScoped<ISensorService, SensorService>();
builder.Services.AddScoped<ILeituraService, LeituraService>();
builder.Services.AddScoped<IIrrigacaoService, IrrigacaoService>();
builder.Services.AddScoped<IReservatorioService, ReservatorioService>();
builder.Services.AddScoped<IMeteorologiaService, MeteorologiaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
// Configure OpenWeather HTTP client
builder.Services.AddHttpClient<IMeteorologyClient, OpenWeatherClient>(client =>
{
    // Base address for OpenWeather API
    client.BaseAddress = new Uri("https://api.openweathermap.org");
});

// Register weather polling background service
builder.Services.AddHostedService<Estufa.Api.Services.Weather.WeatherPollingService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Serve frontend static files from ../.. /frontend
var frontendPath = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "frontend");
if (Directory.Exists(frontendPath))
{
    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = new PhysicalFileProvider(frontendPath)
    });
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(frontendPath)
    });
}

app.MapControllers();

// Map SignalR hubs
app.MapHub<EstufaHub>("/hubs/estufa");

// Run DB seed
using (var scope = app.Services.CreateScope())
{
    var svc = scope.ServiceProvider;
    try
    {
        var ctx = svc.GetRequiredService<EstufaDbContext>();
        // Ensure database is created (SQLite fallback) and apply any pending migrations when available.
        try
        {
            // Prefer migrations when present, otherwise create schema for demo.
            var pending = ctx.Database.GetPendingMigrations();
            if (pending != null && pending.Any())
            {
                ctx.Database.Migrate();
            }
            else
            {
                ctx.Database.EnsureCreated();
            }
        }
        catch
        {
            // Fallback to EnsureCreated if migration APIs fail in restricted environments
            ctx.Database.EnsureCreated();
        }

        await Estufa.Api.Data.DbSeeder.SeedAsync(ctx);
    }
    catch (Exception ex)
    {
        // swallow - logging could be added
        Console.WriteLine("DB Seed failed: " + ex.Message);
    }
}

app.Run();
