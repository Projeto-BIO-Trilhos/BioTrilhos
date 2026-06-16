using Estufa.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estufa.Api.Controllers
{
    [ApiController]
    [Route("api/meteorologia")]
    public class MeteorologiaController : ControllerBase
    {
        private readonly IMeteorologiaService _service;

        public MeteorologiaController(IMeteorologiaService service)
        {
            _service = service;
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> GetRecent([FromQuery] int days = 7)
        {
            var list = await _service.GetRecentosAsync(days);
            return Ok(list);
        }

        [HttpGet("atual")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> GetAtual()
        {
            // read configured OpenWeather coordinates
            try
            {
                var cfg = HttpContext.RequestServices.GetService(typeof(Microsoft.Extensions.Configuration.IConfiguration)) as Microsoft.Extensions.Configuration.IConfiguration;
                var section = cfg.GetSection("OpenWeather");
                var lat = section.GetValue<double>("Latitude");
                var lon = section.GetValue<double>("Longitude");
                var client = HttpContext.RequestServices.GetService(typeof(Estufa.Api.Services.Weather.IMeteorologyClient)) as Estufa.Api.Services.Weather.IMeteorologyClient;
                if (client == null) return StatusCode(500, "Meteorology client not available");
                var raw = await client.GetWeatherRawAsync(lat, lon);
                // attempt to parse and return minimal useful payload
                try
                {
                    using var doc = System.Text.Json.JsonDocument.Parse(raw);
                    var root = doc.RootElement;
                    var result = new System.Collections.Generic.Dictionary<string, object>();
                    if (root.TryGetProperty("current", out var current))
                    {
                        var cur = new System.Collections.Generic.Dictionary<string, object>();
                        if (current.TryGetProperty("temp", out var t)) cur["temp"] = t.GetDouble();
                        if (current.TryGetProperty("humidity", out var h)) cur["humidity"] = h.GetInt32();
                        if (current.TryGetProperty("weather", out var w) && w.GetArrayLength()>0)
                        {
                            var weather0 = w[0];
                            if (weather0.TryGetProperty("main", out var m)) cur["main"] = m.GetString();
                            if (weather0.TryGetProperty("description", out var d)) cur["description"] = d.GetString();
                            if (weather0.TryGetProperty("icon", out var ic)) cur["icon"] = ic.GetString();
                        }
                        result["current"] = cur;
                    }
                    if (root.TryGetProperty("daily", out var daily) && daily.GetArrayLength()>0)
                    {
                        var day0 = daily[0];
                        var d0 = new System.Collections.Generic.Dictionary<string, object>();
                        if (day0.TryGetProperty("pop", out var pop)) d0["pop"] = pop.GetDouble();
                        if (day0.TryGetProperty("temp", out var tempObj))
                        {
                            if (tempObj.TryGetProperty("min", out var min)) d0["min"] = min.GetDouble();
                            if (tempObj.TryGetProperty("max", out var max)) d0["max"] = max.GetDouble();
                        }
                        result["today"] = d0;
                    }
                    return Ok(result);
                }
                catch
                {
                    // return raw json if parse failed
                    return Content(raw, "application/json");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("registrar")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Registrar([FromQuery] string tipo, [FromQuery] string descricao, [FromBody] string dadosJson)
        {
            var ev = await _service.RegistrarEventoAsync(tipo, descricao, dadosJson);
            return Ok(ev);
        }

        // Test trigger (no auth) — creates a sample meteorology event and returns it.
        [HttpPost("trigger-test")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> TriggerTest()
        {
            var ev = await _service.RegistrarEventoAsync("Teste", "Evento de teste gerado via endpoint", "{}");
            return Ok(ev);
        }

        [HttpGet("{id}")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var ev = await _service.GetByIdAsync(id);
            if (ev == null) return NotFound();
            return Ok(ev);
        }
    }
}
