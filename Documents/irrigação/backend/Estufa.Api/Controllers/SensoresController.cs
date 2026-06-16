using Estufa.Api.Models;
using Estufa.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Estufa.Api.Controllers
{
    [ApiController]
    [Route("api/sensores")]
    public class SensoresController : ControllerBase
    {
        private readonly ISensorService _service;

        public SensoresController(ISensorService service)
        {
            _service = service;
        }

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        [HttpPost]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Create([FromBody] Sensor sensor)
        {
            var s = await _service.CreateAsync(sensor);
            return CreatedAtAction(nameof(GetById), new { id = s.Id }, s);
        }

        [HttpPut("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] Sensor sensor)
        {
            if (id != sensor.Id) return BadRequest();
            await _service.UpdateAsync(sensor);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
