using Estufa.Api.Models;
using Estufa.Api.Services;
using Estufa.Api.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Estufa.Api.Controllers
{
    [ApiController]
    [Route("api/sensores/leituras")]
    public class LeiturasController : ControllerBase
    {
        private readonly ILeituraService _service;
        private readonly IHubContext<EstufaHub> _hubContext;

        public LeiturasController(ILeituraService service, IHubContext<EstufaHub> hubContext)
        {
            _service = service;
            _hubContext = hubContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetRecent([FromQuery] int minutes = 60)
        {
            var itens = await _service.GetRecentAsync(minutes);
            return Ok(itens);
        }

        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        [HttpPost("enviar-dados")]
        public async Task<IActionResult> EnviarDados([FromBody] Leitura leitura)
        {
            var l = await _service.AddAsync(leitura);
            // Broadcast via SignalR to connected clients
            try
            {
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
                await _hubContext.Clients.All.SendAsync("NewLeitura", l);
            }
            catch
            {
                // ignore SignalR errors in controller flow
            }
            return CreatedAtAction(nameof(GetRecent), new { minutes = 60 }, l);
        }
    }
}
