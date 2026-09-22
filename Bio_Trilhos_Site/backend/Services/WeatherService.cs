using BioTrilhos.API.Data;
using BioTrilhos.API.DTOs;
using BioTrilhos.API.Models;

namespace BioTrilhos.API.Services;

/// <summary>
/// Interface para o serviço de meteorologia.
/// </summary>
public interface IWeatherService
{
    Task<DadosMeteorologicosDto?> GetWeatherAtualAsync(int estufaId);
    Task<List<DadosMeteorologicosDto>> GetWeatherHistoricoAsync(int estufaId, int dias = 7);
    Task SalvarDadosMeteorologicosAsync(int estufaId, DadosMeteorologicos dados);
}

/// <summary>
/// Serviço de integração com API de meteorologia (Open-Meteo).
/// Open-Meteo é uma API gratuita sem necessidade de chave de autenticação.
/// </summary>
public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly BioTrilhosDbContext _context;
    private readonly ILogger<WeatherService> _logger;
    private const string BaseUrlOpenMeteo = "https://api.open-meteo.com/v1";

    public WeatherService(HttpClient httpClient, BioTrilhosDbContext context, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Obtém dados meteorológicos atuais para Cruzeiro - São Paulo - Brasil.
    /// Latitude: -22.6597, Longitude: -44.9737
    /// </summary>
    public async Task<DadosMeteorologicosDto?> GetWeatherAtualAsync(int estufaId)
    {
        try
        {
            // Coordenadas de Cruzeiro - São Paulo - Brasil
            const string latitude = "-22.6597";
            const string longitude = "-44.9737";
            
            var url = $"{BaseUrlOpenMeteo}/forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m,relative_humidity_2m,weather_code,rain,wind_speed_10m&timezone=America/Sao_Paulo";
            
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            
            var json = await response.Content.ReadAsStringAsync();
            var weatherData = ParseOpenMeteoResponse(json);
            
            if (weatherData == null)
            {
                _logger.LogWarning($"Falha ao processar dados meteorológicos para estufa {estufaId}");
                return null;
            }

            // Salvar dados no banco
            var dados = new DadosMeteorologicos
            {
                EstufaId = estufaId,
                TemperaturaExterna = weatherData.Temperatura,
                UmidadeExterna = weatherData.Umidade,
                CondicaoClimatica = weatherData.CondicaoClimatica,
                EstaChovendo = weatherData.ChuveSim,
                ProbabilidadeChuva = weatherData.ProbabilidadeChuva,
                VelocidadeVento = weatherData.VelocidadeVento,
                DataHoraConsulta = DateTime.UtcNow,
                FonteAPI = "Open-Meteo"
            };

            await SalvarDadosMeteorologicosAsync(estufaId, dados);

            return new DadosMeteorologicosDto
            {
                Id = dados.Id,
                EstufaId = estufaId,
                TemperaturaExterna = weatherData.Temperatura,
                UmidadeExterna = weatherData.Umidade,
                CondicaoClimatica = weatherData.CondicaoClimatica,
                EstaChovendo = weatherData.ChuveSim,
                ProbabilidadeChuva = weatherData.ProbabilidadeChuva,
                VelocidadeVento = weatherData.VelocidadeVento,
                DataHoraConsulta = DateTime.UtcNow,
                FonteAPI = "Open-Meteo"
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError($"Erro ao conectar à API meteorológica: {ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter dados meteorológicos: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Obtém histórico de dados meteorológicos.
    /// </summary>
    public async Task<List<DadosMeteorologicosDto>> GetWeatherHistoricoAsync(int estufaId, int dias = 7)
    {
        var dataInicio = DateTime.UtcNow.AddDays(-dias);
        
        var dados = _context.DadosMeteorologicos
            .Where(d => d.EstufaId == estufaId && d.DataHoraConsulta >= dataInicio)
            .OrderByDescending(d => d.DataHoraConsulta)
            .Select(d => new DadosMeteorologicosDto
            {
                Id = d.Id,
                EstufaId = d.EstufaId,
                TemperaturaExterna = d.TemperaturaExterna,
                UmidadeExterna = d.UmidadeExterna,
                CondicaoClimatica = d.CondicaoClimatica,
                EstaChovendo = d.EstaChovendo,
                ProbabilidadeChuva = d.ProbabilidadeChuva,
                PrevisaoChuva = d.PrevisaoChuva,
                VelocidadeVento = d.VelocidadeVento,
                DataHoraConsulta = d.DataHoraConsulta,
                FonteAPI = d.FonteAPI
            })
            .ToList();

        return dados;
    }

    /// <summary>
    /// Salva dados meteorológicos no banco.
    /// </summary>
    public async Task SalvarDadosMeteorologicosAsync(int estufaId, DadosMeteorologicos dados)
    {
        try
        {
            _context.DadosMeteorologicos.Add(dados);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao salvar dados meteorológicos: {ex.Message}");
        }
    }

    /// <summary>
    /// Faz parsing da resposta JSON da API Open-Meteo.
    /// </summary>
    private WeatherData? ParseOpenMeteoResponse(string json)
    {
        try
        {
            // Parse simplificado - em produção usar System.Text.Json ou Newtonsoft.Json
            var weatherData = new WeatherData();
            
            // Exemplo de parsing (seria melhor usar JSON parser)
            if (json.Contains("temperature_2m"))
            {
                var tempStart = json.IndexOf("\"temperature_2m\":", StringComparison.OrdinalIgnoreCase) + 17;
                var tempEnd = json.IndexOf(",", tempStart);
                if (decimal.TryParse(json.Substring(tempStart, tempEnd - tempStart).Trim(), out var temp))
                {
                    weatherData.Temperatura = temp;
                }
            }

            if (json.Contains("relative_humidity_2m"))
            {
                var humStart = json.IndexOf("\"relative_humidity_2m\":", StringComparison.OrdinalIgnoreCase) + 23;
                var humEnd = json.IndexOf(",", humStart);
                if (int.TryParse(json.Substring(humStart, humEnd - humStart).Trim(), out var hum))
                {
                    weatherData.Umidade = hum;
                }
            }

            if (json.Contains("rain"))
            {
                weatherData.ChuveSim = json.Contains("\"rain\":", StringComparison.OrdinalIgnoreCase) && 
                                       !json.Substring(json.IndexOf("\"rain\":", StringComparison.OrdinalIgnoreCase), 20).Contains("0");
            }

            weatherData.CondicaoClimatica = DeterminarCondicaoClimatica(json);
            return weatherData;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Erro ao fazer parsing: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Determina a condição climática baseado no código de clima do Open-Meteo.
    /// </summary>
    private string DeterminarCondicaoClimatica(string json)
    {
        if (json.Contains("\"weather_code\":0")) return "Céu limpo";
        if (json.Contains("\"weather_code\":1") || json.Contains("\"weather_code\":2")) return "Parcialmente nublado";
        if (json.Contains("\"weather_code\":3")) return "Nublado";
        if (json.Contains("\"weather_code\":45") || json.Contains("\"weather_code\":48")) return "Nevoeiro";
        if (json.Contains("\"weather_code\":51") || json.Contains("\"weather_code\":53") || json.Contains("\"weather_code\":55")) return "Chuvisco";
        if (json.Contains("\"weather_code\":61") || json.Contains("\"weather_code\":63") || json.Contains("\"weather_code\":65")) return "Chuva";
        if (json.Contains("\"weather_code\":80") || json.Contains("\"weather_code\":81") || json.Contains("\"weather_code\":82")) return "Chuva forte";
        if (json.Contains("\"weather_code\":85") || json.Contains("\"weather_code\":86")) return "Chuva com neve";
        if (json.Contains("\"weather_code\":95") || json.Contains("\"weather_code\":96") || json.Contains("\"weather_code\":99")) return "Tempestade";
        
        return "Desconhecido";
    }

    private class WeatherData
    {
        public decimal Temperatura { get; set; }
        public decimal Umidade { get; set; }
        public bool ChuveSim { get; set; }
        public decimal ProbabilidadeChuva { get; set; }
        public decimal VelocidadeVento { get; set; }
        public string CondicaoClimatica { get; set; } = string.Empty;
    }
}
