using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioTrilhos.API.Data;
using BioTrilhos.API.DTOs;
using BioTrilhos.API.Models;

namespace BioTrilhos.API.Controllers;

/// <summary>
/// Controller para gerenciar reservatórios de água.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReservatoriosController : ControllerBase
{
    private readonly BioTrilhosDbContext _context;
    private readonly ILogger<ReservatoriosController> _logger;

    public ReservatoriosController(BioTrilhosDbContext context, ILogger<ReservatoriosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/reservatorios
    /// Lista todos os reservatórios de uma estufa.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<ReservatorioDto>>> GetReservatorios([FromQuery] int estufaId)
    {
        try
        {
            var reservatorios = await _context.Reservatorios
                .Where(r => r.EstufaId == estufaId)
                .Select(r => new ReservatorioDto
                {
                    Id = r.Id,
                    EstufaId = r.EstufaId,
                    Nome = r.Nome,
                    CapacidadeMaxima = r.CapacidadeMaxima,
                    NivelAtual = r.NivelAtual,
                    PercentualAtual = r.PercentualAtual,
                    Status = r.Status,
                    DataUltimaAtualizacao = r.DataUltimaAtualizacao
                })
                .ToListAsync();

            return Ok(reservatorios);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao listar reservatórios: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao listar reservatórios" });
        }
    }

    /// <summary>
    /// GET /api/reservatorios/{id}
    /// Obtém um reservatório específico.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ReservatorioDto>> GetReservatorio(int id)
    {
        try
        {
            var reservatorio = await _context.Reservatorios.FindAsync(id);
            if (reservatorio == null)
                return NotFound(new { mensagem = "Reservatório não encontrado" });

            return Ok(new ReservatorioDto
            {
                Id = reservatorio.Id,
                EstufaId = reservatorio.EstufaId,
                Nome = reservatorio.Nome,
                CapacidadeMaxima = reservatorio.CapacidadeMaxima,
                NivelAtual = reservatorio.NivelAtual,
                PercentualAtual = reservatorio.PercentualAtual,
                Status = reservatorio.Status,
                DataUltimaAtualizacao = reservatorio.DataUltimaAtualizacao
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter reservatório: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter reservatório" });
        }
    }

    /// <summary>
    /// POST /api/reservatorios
    /// Cria um novo reservatório.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ReservatorioDto>> CreateReservatorio([FromBody] CreateReservatorioDto dto)
    {
        try
        {
            var estufa = await _context.Estufas.FindAsync(dto.EstufaId);
            if (estufa == null)
                return BadRequest(new { mensagem = "Estufa não encontrada" });

            var reservatorio = new Reservatorio
            {
                EstufaId = dto.EstufaId,
                Nome = dto.Nome,
                CapacidadeMaxima = dto.CapacidadeMaxima,
                NivelAtual = dto.NivelAtual,
                PercentualAtual = (dto.NivelAtual / dto.CapacidadeMaxima) * 100,
                Status = "Normal",
                DataUltimaAtualizacao = DateTime.UtcNow
            };

            _context.Reservatorios.Add(reservatorio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservatorio), new { id = reservatorio.Id }, new ReservatorioDto
            {
                Id = reservatorio.Id,
                EstufaId = reservatorio.EstufaId,
                Nome = reservatorio.Nome,
                CapacidadeMaxima = reservatorio.CapacidadeMaxima,
                NivelAtual = reservatorio.NivelAtual,
                PercentualAtual = reservatorio.PercentualAtual,
                Status = reservatorio.Status,
                DataUltimaAtualizacao = reservatorio.DataUltimaAtualizacao
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao criar reservatório: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao criar reservatório" });
        }
    }

    /// <summary>
    /// PUT /api/reservatorios/{id}
    /// Atualiza o nível de um reservatório (atualização manual).
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ReservatorioDto>> UpdateReservatorio(int id, [FromBody] CreateReservatorioDto dto)
    {
        try
        {
            var reservatorio = await _context.Reservatorios.FindAsync(id);
            if (reservatorio == null)
                return NotFound(new { mensagem = "Reservatório não encontrado" });

            var nivelAnterior = reservatorio.NivelAtual;

            reservatorio.NivelAtual = dto.NivelAtual;
            reservatorio.PercentualAtual = (dto.NivelAtual / reservatorio.CapacidadeMaxima) * 100;
            reservatorio.DataUltimaAtualizacao = DateTime.UtcNow;
            reservatorio.Status = reservatorio.PercentualAtual < 20 ? "Baixo" : "Normal";

            // Registrar no histórico
            var historico = new HistoricoReservatorio
            {
                ReservatorioId = id,
                NivelAnterior = nivelAnterior,
                NivelAtual = dto.NivelAtual,
                TipoMovimentacao = TipoMovimentacao.AtualizacaoManual,
                Descricao = $"Atualização manual: {nivelAnterior}L → {dto.NivelAtual}L",
                DataHora = DateTime.UtcNow
            };

            _context.Reservatorios.Update(reservatorio);
            _context.HistoricosReservatorios.Add(historico);
            await _context.SaveChangesAsync();

            return Ok(new ReservatorioDto
            {
                Id = reservatorio.Id,
                EstufaId = reservatorio.EstufaId,
                Nome = reservatorio.Nome,
                CapacidadeMaxima = reservatorio.CapacidadeMaxima,
                NivelAtual = reservatorio.NivelAtual,
                PercentualAtual = reservatorio.PercentualAtual,
                Status = reservatorio.Status,
                DataUltimaAtualizacao = reservatorio.DataUltimaAtualizacao
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao atualizar reservatório: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao atualizar reservatório" });
        }
    }

    /// <summary>
    /// GET /api/reservatorios/{id}/historico
    /// Obtém o histórico de alterações do nível.
    /// </summary>
    [HttpGet("{id}/historico")]
    public async Task<ActionResult<List<object>>> GetHistoricoReservatorio(int id, [FromQuery] int dias = 30)
    {
        try
        {
            var reservatorio = await _context.Reservatorios.FindAsync(id);
            if (reservatorio == null)
                return NotFound(new { mensagem = "Reservatório não encontrado" });

            var dataInicio = DateTime.UtcNow.AddDays(-dias);

            var historico = await _context.HistoricosReservatorios
                .Where(h => h.ReservatorioId == id && h.DataHora >= dataInicio)
                .OrderByDescending(h => h.DataHora)
                .Select(h => new
                {
                    h.Id,
                    h.NivelAnterior,
                    h.NivelAtual,
                    tipoMovimentacao = h.TipoMovimentacao.ToString(),
                    h.Descricao,
                    h.DataHora
                })
                .ToListAsync();

            return Ok(historico);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter histórico do reservatório: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter histórico" });
        }
    }
}
