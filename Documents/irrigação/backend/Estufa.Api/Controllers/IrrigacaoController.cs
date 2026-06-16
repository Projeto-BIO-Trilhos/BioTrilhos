using Estufa.Api.Models;
using Estufa.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estufa.Api.Controllers
{
    [ApiController]
    [Route("api/irrigacao")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class IrrigacaoController : ControllerBase
    {
        private readonly IIrrigacaoService _service;

        public IrrigacaoController(IIrrigacaoService service)
        {
            _service = service;
        }

        [HttpPost("acionar")]
        public async Task<IActionResult> Acionar([FromBody] Irrigacao irrigacao)
        {
            var i = await _service.AcionarAsync(irrigacao);
            return Ok(i);
        }

        [HttpGet("historico")]
        public async Task<IActionResult> Historico([FromQuery] DateTime? from = null, [FromQuery] DateTime? to = null)
        {
            var list = await _service.HistoricoAsync(from, to);
            return Ok(list);
        }
    }
}
