using Estufa.Api.Models;
using Estufa.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estufa.Api.Controllers
{
    [ApiController]
    [Route("api/reservatorios")]
    public class ReservatoriosController : ControllerBase
    {
        private readonly IReservatorioService _service;

        public ReservatoriosController(IReservatorioService service)
        {
            _service = service;
        }

        [HttpGet("principal")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> GetPrincipal()
        {
            var res = await _service.GetPrincipalAsync();
            if (res == null) return NotFound();
            return Ok(res);
        }

        [HttpPut("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Reservatorio res)
        {
            if (id != res.Id) return BadRequest();
            var updated = await _service.UpdateAsync(res);
            return Ok(updated);
        }
    }
}
