using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Estufa.Api.Services.Weather
{
    public class OpenWeatherClient : IMeteorologyClient
    {
        private readonly HttpClient _http;

        public OpenWeatherClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<(bool rainExpected, string rawJson)> GetRainForecastNext24hAsync(double lat, double lon)
        {
            // Use One Call API (exclude unnecessary parts)
            var url = $"/data/2.5/onecall?lat={lat}&lon={lon}&exclude=minutely,current,alerts&units=metric";
            var res = await _http.GetAsync(url);
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(json);
                // Check 'daily' array first day (next 24h)
                if (doc.RootElement.TryGetProperty("daily", out var daily) && daily.GetArrayLength() > 0)
                {
                    var today = daily[0];
                    // check rain probability
                    if (today.TryGetProperty("pop", out var pop))
                    {
                        var prob = pop.GetDouble();
                        if (prob >= 0.3) return (true, json);
                    }
                    // also check weather descriptions
                    if (today.TryGetProperty("weather", out var weatherArr) && weatherArr.GetArrayLength()>0)
                    {
                        var w = weatherArr[0];
                        if (w.TryGetProperty("main", out var main))
                        {
                            var m = main.GetString()?.ToLowerInvariant() ?? string.Empty;
                            if (m.Contains("rain") || m.Contains("storm")) return (true, json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignore parse errors, return false with raw
            }

            return (false, json);
        }

        public async Task<string> GetWeatherRawAsync(double lat, double lon)
        {
            // include 'current' and 'hourly' so frontend can display current conditions
            var url = $"/data/2.5/onecall?lat={lat}&lon={lon}&exclude=minutely,alerts&units=metric";
            var res = await _http.GetAsync(url);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync();
        }
    }
}
