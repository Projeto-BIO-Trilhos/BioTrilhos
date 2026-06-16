using Estufa.Api.Services.Weather;
using Estufa.Api.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Estufa.Api.Services.Weather
{
    using Microsoft.AspNetCore.SignalR;
    using Estufa.Api.Hubs;

    public class WeatherPollingService : BackgroundService
    {
        private readonly IMeteorologyClient _client;
        private readonly IMeteorologiaService _service;
        private readonly ILogger<WeatherPollingService> _logger;
        private readonly IConfiguration _config;
        private readonly IHubContext<EstufaHub> _hubContext;

        public WeatherPollingService(IMeteorologyClient client, IMeteorologiaService service, ILogger<WeatherPollingService> logger, IConfiguration config, IHubContext<EstufaHub> hubContext)
        {
            _client = client;
            _service = service;
            _logger = logger;
            _config = config;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("WeatherPollingService started");

            var section = _config.GetSection("OpenWeather");
            var lat = section.GetValue<double>("Latitude");
            var lon = section.GetValue<double>("Longitude");
            var minutes = section.GetValue<int?>("PollMinutes") ?? 60;

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var (rain, raw) = await _client.GetRainForecastNext24hAsync(lat, lon);
                    if (rain)
                    {
                        _logger.LogInformation("Rain expected — registering meteorological event");
                        var ev = await _service.RegistrarEventoAsync("PrevisaoChuva", "Previsão de chuva nas próximas 24h", raw);
                        // Broadcast new meteorological event to connected clients
                        try
                        {
                            await _hubContext.Clients.All.SendAsync("NewMeteorologyEvent", ev);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to broadcast meteorology event");
                        }
                    }
                    else
                    {
                        _logger.LogInformation("No rain expected");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while polling weather");
                }

                await Task.Delay(TimeSpan.FromMinutes(minutes), stoppingToken);
            }
        }
    }
}
