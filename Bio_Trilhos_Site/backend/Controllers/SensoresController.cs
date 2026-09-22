using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioTrilhos.API.Data;
using BioTrilhos.API.DTOs;
using BioTrilhos.API.Models;

namespace BioTrilhos.API.Controllers;

/// <summary>
/// Controller para gerenciar sensores.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SensoresController : ControllerBase
{
    private readonly BioTrilhosDbContext _context;
    private readonly ILogger<SensoresController> _logger;

    public SensoresController(BioTrilhosDbContext context, ILogger<SensoresController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/sensores
    /// Lista todos os sensores de uma estufa.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<SensorDto>>> GetSensores([FromQuery] int estufaId)
    {
        try
        {
            var sensores = await _context.Sensores
                .Where(s => s.EstufaId == estufaId)
                .Select(s => new SensorDto
                {
                    Id = s.Id,
                    EstufaId = s.EstufaId,
                    Nome = s.Nome,
                    TipoSensor = (int)s.TipoSensor,
                    Localizacao = s.Localizacao,
                    IdentificadorDispositivo = s.IdentificadorDispositivo,
                    UnidadeMedida = s.UnidadeMedida,
                    Ativo = s.Ativo,
                    DataInstalacao = s.DataInstalacao
                })
                .ToListAsync();

            return Ok(sensores);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao listar sensores: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao listar sensores" });
        }
    }

    /// <summary>
    /// GET /api/sensores/{id}
    /// Obtém um sensor específico.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<SensorDto>> GetSensor(int id)
    {
        try
        {
            var sensor = await _context.Sensores.FindAsync(id);
            if (sensor == null)
                return NotFound(new { mensagem = "Sensor não encontrado" });

            return Ok(new SensorDto
            {
                Id = sensor.Id,
                EstufaId = sensor.EstufaId,
                Nome = sensor.Nome,
                TipoSensor = (int)sensor.TipoSensor,
                Localizacao = sensor.Localizacao,
                IdentificadorDispositivo = sensor.IdentificadorDispositivo,
                UnidadeMedida = sensor.UnidadeMedida,
                Ativo = sensor.Ativo,
                DataInstalacao = sensor.DataInstalacao
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter sensor: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter sensor" });
        }
    }

    /// <summary>
    /// POST /api/sensores
    /// Cria um novo sensor.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<SensorDto>> CreateSensor([FromBody] CreateSensorDto dto)
    {
        try
        {
            // Verificar se estufa existe
            var estufa = await _context.Estufas.FindAsync(dto.EstufaId);
            if (estufa == null)
                return BadRequest(new { mensagem = "Estufa não encontrada" });

            var sensor = new Sensor
            {
                EstufaId = dto.EstufaId,
                Nome = dto.Nome,
                TipoSensor = (TipoSensor)dto.TipoSensor,
                Localizacao = dto.Localizacao,
                IdentificadorDispositivo = dto.IdentificadorDispositivo,
                UnidadeMedida = dto.UnidadeMedida,
                Ativo = true,
                DataInstalacao = DateTime.UtcNow
            };

            _context.Sensores.Add(sensor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSensor), new { id = sensor.Id }, new SensorDto
            {
                Id = sensor.Id,
                EstufaId = sensor.EstufaId,
                Nome = sensor.Nome,
                TipoSensor = (int)sensor.TipoSensor,
                Localizacao = sensor.Localizacao,
                IdentificadorDispositivo = sensor.IdentificadorDispositivo,
                UnidadeMedida = sensor.UnidadeMedida,
                Ativo = sensor.Ativo,
                DataInstalacao = sensor.DataInstalacao
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao criar sensor: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao criar sensor" });
        }
    }

    /// <summary>
    /// GET /api/sensores/{id}/leituras
    /// Obtém leituras de um sensor.
    /// </summary>
    [HttpGet("{id}/leituras")]
    public async Task<ActionResult<List<LeituraDto>>> GetLeiturasSensor(int id, [FromQuery] int dias = 7)
    {
        try
        {
            var sensor = await _context.Sensores.FindAsync(id);
            if (sensor == null)
                return NotFound(new { mensagem = "Sensor não encontrado" });

            var dataInicio = DateTime.UtcNow.AddDays(-dias);

            var leituras = await _context.LeiturasSensores
                .Where(l => l.SensorId == id && l.DataHoraLeitura >= dataInicio)
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
            _logger.LogError($"Erro ao obter leituras do sensor: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter leituras do sensor" });
        }
    }
}
