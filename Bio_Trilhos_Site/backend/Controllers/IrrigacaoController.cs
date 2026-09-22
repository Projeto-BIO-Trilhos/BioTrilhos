using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioTrilhos.API.Data;
using BioTrilhos.API.DTOs;
using BioTrilhos.API.Models;

namespace BioTrilhos.API.Controllers;

/// <summary>
/// Controller para gerenciar a irrigação.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class IrrigacaoController : ControllerBase
{
    private readonly BioTrilhosDbContext _context;
    private readonly IAutomationService _automationService;
    private readonly ILogger<IrrigacaoController> _logger;

    public IrrigacaoController(BioTrilhosDbContext context, IAutomationService automationService, ILogger<IrrigacaoController> logger)
    {
        _context = context;
        _automationService = automationService;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/irrigacao
    /// Lista todas as irrigações de uma estufa.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<IrrigacaoDto>>> GetIrrigacoes([FromQuery] int estufaId)
    {
        try
        {
            var irrigacoes = await _context.Irrigacoes
                .Where(i => i.EstufaId == estufaId)
                .OrderByDescending(i => i.DataHoraInicio)
                .Select(i => new IrrigacaoDto
                {
                    Id = i.Id,
                    EstufaId = i.EstufaId,
                    ReservatorioId = i.ReservatorioId,
                    DataHoraInicio = i.DataHoraInicio,
                    DataHoraFim = i.DataHoraFim,
                    DuracaoSegundos = i.DuracaoSegundos,
                    Motivo = i.Motivo,
                    TipoAcionamento = (int)i.TipoAcionamento,
                    Status = (int)i.Status
                })
                .ToListAsync();

            return Ok(irrigacoes);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao listar irrigações: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao listar irrigações" });
        }
    }

    /// <summary>
    /// GET /api/irrigacao/{id}
    /// Obtém uma irrigação específica.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<IrrigacaoDto>> GetIrrigacao(int id)
    {
        try
        {
            var irrigacao = await _context.Irrigacoes.FindAsync(id);
            if (irrigacao == null)
                return NotFound(new { mensagem = "Irrigação não encontrada" });

            return Ok(new IrrigacaoDto
            {
                Id = irrigacao.Id,
                EstufaId = irrigacao.EstufaId,
                ReservatorioId = irrigacao.ReservatorioId,
                DataHoraInicio = irrigacao.DataHoraInicio,
                DataHoraFim = irrigacao.DataHoraFim,
                DuracaoSegundos = irrigacao.DuracaoSegundos,
                Motivo = irrigacao.Motivo,
                TipoAcionamento = (int)irrigacao.TipoAcionamento,
                Status = (int)irrigacao.Status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter irrigação: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter irrigação" });
        }
    }

    /// <summary>
    /// POST /api/irrigacao/iniciar
    /// Inicia uma irrigação manual.
    /// </summary>
    [HttpPost("iniciar")]
    public async Task<ActionResult<IrrigacaoDto>> IniciarIrrigacao([FromBody] AcionarIrrigacaoDto dto)
    {
        try
        {
            var estufa = await _context.Estufas.FindAsync(dto.EstufaId);
            if (estufa == null)
                return BadRequest(new { mensagem = "Estufa não encontrada" });

            // Verificar se já existe irrigação ativa
            var ativa = await _context.Irrigacoes
                .Where(i => i.EstufaId == dto.EstufaId && i.Status == StatusIrrigacao.Ativa)
                .FirstOrDefaultAsync();

            if (ativa != null)
                return BadRequest(new { mensagem = "Já existe uma irrigação ativa para esta estufa" });

            var config = await _context.ConfiguracoesAmbientais
                .FirstOrDefaultAsync(c => c.EstufaId == dto.EstufaId);

            var duracao = dto.DuracaoSegundos ?? (config?.TempoMaximoIrrigacaoSegundos ?? 300);

            var irrigacao = new Irrigacao
            {
                EstufaId = dto.EstufaId,
                ReservatorioId = dto.ReservatorioId,
                DataHoraInicio = DateTime.UtcNow,
                DuracaoSegundos = duracao,
                Motivo = dto.Motivo,
                TipoAcionamento = (TipoAcionamento)dto.TipoAcionamento,
                Status = StatusIrrigacao.Ativa
            };

            _context.Irrigacoes.Add(irrigacao);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIrrigacao), new { id = irrigacao.Id }, new IrrigacaoDto
            {
                Id = irrigacao.Id,
                EstufaId = irrigacao.EstufaId,
                ReservatorioId = irrigacao.ReservatorioId,
                DataHoraInicio = irrigacao.DataHoraInicio,
                DataHoraFim = irrigacao.DataHoraFim,
                DuracaoSegundos = irrigacao.DuracaoSegundos,
                Motivo = irrigacao.Motivo,
                TipoAcionamento = (int)irrigacao.TipoAcionamento,
                Status = (int)irrigacao.Status
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao iniciar irrigação: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao iniciar irrigação" });
        }
    }

    /// <summary>
    /// POST /api/irrigacao/parar
    /// Para a irrigação ativa.
    /// </summary>
    [HttpPost("parar")]
    public async Task<ActionResult> PararIrrigacao([FromBody] int estufaId)
    {
        try
        {
            await _automationService.PararIrrigacaoAsync(estufaId);
            return Ok(new { mensagem = "Irrigação parada com sucesso" });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao parar irrigação: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao parar irrigação" });
        }
    }

    /// <summary>
    /// GET /api/irrigacao/ativa/{estufaId}
    /// Verifica se há irrigação ativa.
    /// </summary>
    [HttpGet("ativa/{estufaId}")]
    public async Task<ActionResult<object>> GetIrrigacaoAtiva(int estufaId)
    {
        try
        {
            var ativa = await _context.Irrigacoes
                .Where(i => i.EstufaId == estufaId && i.Status == StatusIrrigacao.Ativa)
                .FirstOrDefaultAsync();

            if (ativa == null)
                return Ok(new { ativa = false });

            return Ok(new
            {
                ativa = true,
                id = ativa.Id,
                dataHoraInicio = ativa.DataHoraInicio,
                duracao = ativa.DuracaoSegundos,
                motivo = ativa.Motivo
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao verificar irrigação ativa: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao verificar irrigação ativa" });
        }
    }
}
