using BioTrilhos.API.Data;
using BioTrilhos.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Adicionar DbContext com PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=biotrilhos;Username=postgres;Password=postgres";

builder.Services.AddDbContext<BioTrilhosDbContext>(options =>
    options.UseNpgsql(connectionString));

// Adicionar Services
builder.Services.AddScoped<IWeatherService, WeatherService>();
builder.Services.AddScoped<IAutomationService, AutomationService>();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();

// Adicionar Controllers
builder.Services.AddControllers();

// Adicionar CORS para permitir requisições do front-end
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Adicionar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Bio Trilhos API",
        Version = "v1",
        Description = "API para controle e monitoramento da estufa inteligente Bio Trilhos",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Bio Trilhos",
            Url = new Uri("https://biotrilhos.example.com")
        }
    });

    // Adicionar comentários XML
    var xmlFile = "BioTrilhos.API.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Bio Trilhos API v1");
        options.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// Middleware para logging
app.UseMiddleware<LoggingMiddleware>();

app.MapControllers();

// Inicializar banco de dados
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BioTrilhosDbContext>();
    
    // Aplicar migrations
    dbContext.Database.Migrate();
    
    // Semear dados iniciais
    await SeedData(dbContext);
}

await app.RunAsync();

/// <summary>
/// Semeia dados iniciais no banco de dados.
/// </summary>
async Task SeedData(BioTrilhosDbContext dbContext)
{
    try
    {
        // Verificar se já existem dados
        if (await dbContext.Estufas.AnyAsync())
            return;

        // Criar estufa padrão
        var estufa = new BioTrilhos.API.Models.Estufa
        {
            Nome = "Bio Trilhos - Estufa Principal",
            Localizacao = "Cruzeiro - São Paulo - Brasil",
            Descricao = "Estufa tecnológica integrada à linha ferroviária revitalizada para educação ambiental e turismo sustentável.",
            Ativa = true,
            DataCriacao = DateTime.UtcNow
        };

        dbContext.Estufas.Add(estufa);
        await dbContext.SaveChangesAsync();

        // Criar sensores padrão
        var sensores = new List<BioTrilhos.API.Models.Sensor>
        {
            new() { EstufaId = estufa.Id, Nome = "Sensor de Temperatura", TipoSensor = BioTrilhos.API.Models.TipoSensor.Temperatura, Localizacao = "Centro da estufa", IdentificadorDispositivo = "TEMP_001", UnidadeMedida = "°C", Ativo = true, DataInstalacao = DateTime.UtcNow },
            new() { EstufaId = estufa.Id, Nome = "Sensor de Umidade do Ar", TipoSensor = BioTrilhos.API.Models.TipoSensor.UmidadeDoAr, Localizacao = "Centro da estufa", IdentificadorDispositivo = "UMID_AR_001", UnidadeMedida = "%", Ativo = true, DataInstalacao = DateTime.UtcNow },
            new() { EstufaId = estufa.Id, Nome = "Sensor de Umidade do Solo", TipoSensor = BioTrilhos.API.Models.TipoSensor.UmidadeDoSolo, Localizacao = "Canteiro principal", IdentificadorDispositivo = "UMID_SOLO_001", UnidadeMedida = "%", Ativo = true, DataInstalacao = DateTime.UtcNow },
            new() { EstufaId = estufa.Id, Nome = "Sensor de Nível de Água", TipoSensor = BioTrilhos.API.Models.TipoSensor.NivelDeAgua, Localizacao = "Reservatório", IdentificadorDispositivo = "NIVEL_001", UnidadeMedida = "%", Ativo = true, DataInstalacao = DateTime.UtcNow },
            new() { EstufaId = estufa.Id, Nome = "Sensor de Chuva", TipoSensor = BioTrilhos.API.Models.TipoSensor.Chuva, Localizacao = "Topo da estufa", IdentificadorDispositivo = "CHUVA_001", UnidadeMedida = "bool", Ativo = true, DataInstalacao = DateTime.UtcNow }
        };

        dbContext.Sensores.AddRange(sensores);
        await dbContext.SaveChangesAsync();

        // Criar reservatório
        var reservatorio = new BioTrilhos.API.Models.Reservatorio
        {
            EstufaId = estufa.Id,
            Nome = "Reservatório Principal",
            CapacidadeMaxima = 1000,
            NivelAtual = 820,
            PercentualAtual = 82,
            Status = "Normal",
            DataUltimaAtualizacao = DateTime.UtcNow
        };

        dbContext.Reservatorios.Add(reservatorio);
        await dbContext.SaveChangesAsync();

        // Criar configurações ambientais
        var config = new BioTrilhos.API.Models.ConfiguracoesAmbientais
        {
            EstufaId = estufa.Id,
            UmidadeSoloMinima = 50,
            UmidadeSoloMaxima = 75,
            TemperaturaMinima = 20,
            TemperaturaMaxima = 28,
            UmidadeArMinima = 60,
            UmidadeArMaxima = 80,
            TempoMaximoIrrigacaoSegundos = 300,
            NivelMinimoReservatorio = 20,
            AtualizadoEm = DateTime.UtcNow
        };

        dbContext.ConfiguracoesAmbientais.Add(config);
        await dbContext.SaveChangesAsync();

        // Criar algumas leituras de exemplo
        var sensorTemp = sensores.First(s => s.TipoSensor == BioTrilhos.API.Models.TipoSensor.Temperatura);
        var sensorAr = sensores.First(s => s.TipoSensor == BioTrilhos.API.Models.TipoSensor.UmidadeDoAr);
        var sensorSolo = sensores.First(s => s.TipoSensor == BioTrilhos.API.Models.TipoSensor.UmidadeDoSolo);

        var leituras = new List<BioTrilhos.API.Models.LeituraSensor>
        {
            new() { SensorId = sensorTemp.Id, Valor = 24.8m, DataHoraLeitura = DateTime.UtcNow.AddMinutes(-30), UnidadeMedida = "°C", Status = "OK" },
            new() { SensorId = sensorTemp.Id, Valor = 25.2m, DataHoraLeitura = DateTime.UtcNow.AddMinutes(-60), UnidadeMedida = "°C", Status = "OK" },
            new() { SensorId = sensorAr.Id, Valor = 72m, DataHoraLeitura = DateTime.UtcNow.AddMinutes(-30), UnidadeMedida = "%", Status = "OK" },
            new() { SensorId = sensorAr.Id, Valor = 70m, DataHoraLeitura = DateTime.UtcNow.AddMinutes(-60), UnidadeMedida = "%", Status = "OK" },
            new() { SensorId = sensorSolo.Id, Valor = 64m, DataHoraLeitura = DateTime.UtcNow.AddMinutes(-30), UnidadeMedida = "%", Status = "OK" },
            new() { SensorId = sensorSolo.Id, Valor = 62m, DataHoraLeitura = DateTime.UtcNow.AddMinutes(-60), UnidadeMedida = "%", Status = "OK" }
        };

        dbContext.LeiturasSensores.AddRange(leituras);
        await dbContext.SaveChangesAsync();

        Console.WriteLine("✓ Dados iniciais inseridos com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Erro ao semear dados: {ex.Message}");
    }
}

/// <summary>
/// Middleware para logging de requisições.
/// </summary>
public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation($"Requisição: {context.Request.Method} {context.Request.Path}");
        await _next(context);
        _logger.LogInformation($"Resposta: {context.Response.StatusCode}");
    }
}
