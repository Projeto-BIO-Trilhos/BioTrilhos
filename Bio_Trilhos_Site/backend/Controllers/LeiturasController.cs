using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioTrilhos.API.Data;
using BioTrilhos.API.DTOs;
using BioTrilhos.API.Models;

namespace BioTrilhos.API.Controllers;

/// <summary>
/// Controller para gerenciar leituras de sensores.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LeiturasController : ControllerBase
{
    private readonly BioTrilhosDbContext _context;
    private readonly IAutomationService _automationService;
    private readonly ILogger<LeiturasController> _logger;

    public LeiturasController(BioTrilhosDbContext context, IAutomationService automationService, ILogger<LeiturasController> logger)
    {
        _context = context;
        _automationService = automationService;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/leituras
    /// Cria uma nova leitura de sensor.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LeituraDto>> CreateLeitura([FromBody] CreateLeituraDto dto)
    {
        try
        {
            // Verificar se sensor existe
            var sensor = await _context.Sensores.FindAsync(dto.SensorId);
            if (sensor == null)
                return BadRequest(new { mensagem = "Sensor não encontrado" });

            var leitura = new LeituraSensor
            {
                SensorId = dto.SensorId,
                Valor = dto.Valor,
                DataHoraLeitura = DateTime.UtcNow,
                UnidadeMedida = dto.UnidadeMedida,
                Status = dto.Status
            };

            _context.LeiturasSensores.Add(leitura);
            await _context.SaveChangesAsync();

            // Verificar se deve acionar automação
            await _automationService.VerificarEAcionarIrrigacaoAsync(sensor.EstufaId);
            await _automationService.VerificarAlertas(sensor.EstufaId);

            return CreatedAtAction(nameof(CreateLeitura), new LeituraDto
            {
                Id = leitura.Id,
                SensorId = leitura.SensorId,
                Valor = leitura.Valor,
                DataHoraLeitura = leitura.DataHoraLeitura,
                UnidadeMedida = leitura.UnidadeMedida,
                Status = leitura.Status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao criar leitura: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao criar leitura" });
        }
    }

    /// <summary>
    /// GET /api/leituras/historico/{sensorId}
    /// Obtém histórico de leituras com filtro opcional por período.
    /// </summary>
    [HttpGet("historico/{sensorId}")]
    public async Task<ActionResult<List<LeituraDto>>> GetHistoricoLeituras(
        int sensorId,
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null)
    {
        try
        {
            var sensor = await _context.Sensores.FindAsync(sensorId);
            if (sensor == null)
                return NotFound(new { mensagem = "Sensor não encontrado" });

            var query = _context.LeiturasSensores.Where(l => l.SensorId == sensorId);

            if (dataInicio.HasValue)
                query = query.Where(l => l.DataHoraLeitura >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(l => l.DataHoraLeitura <= dataFim.Value);

            var leituras = await query
                .OrderByDescending(l => l.DataHoraLeitura)
                .Select(l => new LeituraDto
                {
                    Id = l.Id,
                    SensorId = l.SensorId,
                    Valor = l.Valor,
                    DataHoraLeitura = l.DataHoraLeitura,
                    UnidadeMedida = l.UnidadeMedida,
                    Status = l.Status
                })
                .ToListAsync();

            return Ok(leituras);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter histórico de leituras: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter histórico" });
        }
    }

    /// <summary>
    /// GET /api/leituras/ultimasestuf{estufaId}
    /// Obtém as últimas leituras de todos os sensores de uma estufa.
    /// </summary>
    [HttpGet("ultimas/{estufaId}")]
    public async Task<ActionResult<List<object>>> GetUltimasLeituras(int estufaId)
    {
        try
        {
            var sensores = await _context.Sensores
                .Where(s => s.EstufaId == estufaId && s.Ativo)
                .ToListAsync();

            var resultado = new List<object>();

            foreach (var sensor in sensores)
            {
                var ultimaLeitura = await _context.LeiturasSensores
                    .Where(l => l.SensorId == sensor.Id)
                    .OrderByDescending(l => l.DataHoraLeitura)
                    .FirstOrDefaultAsync();

                if (ultimaLeitura != null)
                {
                    resultado.Add(new
                    {
                        sensorId = sensor.Id,
                        sensorNome = sensor.Nome,
                        tipoSensor = (int)sensor.TipoSensor,
                        valor = ultimaLeitura.Valor,
                        unidade = ultimaLeitura.UnidadeMedida,
                        dataHora = ultimaLeitura.DataHoraLeitura,
                        status = ultimaLeitura.Status
                    });
                }
            }

            return Ok(resultado);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter últimas leituras: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter últimas leituras" });
        }
    }
}
