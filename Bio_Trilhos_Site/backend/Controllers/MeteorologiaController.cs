using Microsoft.AspNetCore.Mvc;
using BioTrilhos.API.Data;
using BioTrilhos.API.DTOs;
using BioTrilhos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BioTrilhos.API.Controllers;

/// <summary>
/// Controller para gerenciar dados meteorológicos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MeteorologiaController : ControllerBase
{
    private readonly BioTrilhosDbContext _context;
    private readonly IWeatherService _weatherService;
    private readonly ILogger<MeteorologiaController> _logger;

    public MeteorologiaController(BioTrilhosDbContext context, IWeatherService weatherService, ILogger<MeteorologiaController> logger)
    {
        _context = context;
        _weatherService = weatherService;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/meteorologia/atual/{estufaId}
    /// Obtém os dados meteorológicos atuais (consulta API externa).
    /// </summary>
    [HttpGet("atual/{estufaId}")]
    public async Task<ActionResult<DadosMeteorologicosDto>> GetWeatherAtual(int estufaId)
    {
        try
        {
            var estufa = await _context.Estufas.FindAsync(estufaId);
            if (estufa == null)
                return NotFound(new { mensagem = "Estufa não encontrada" });

            var dados = await _weatherService.GetWeatherAtualAsync(estufaId);
            if (dados == null)
                return StatusCode(503, new { mensagem = "API meteorológica indisponível" });

            return Ok(dados);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter dados meteorológicos atuais: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter dados meteorológicos" });
        }
    }

    /// <summary>
    /// GET /api/meteorologia/previsao/{estufaId}
    /// Obtém o histórico de dados meteorológicos.
    /// </summary>
    [HttpGet("previsao/{estufaId}")]
    public async Task<ActionResult<List<DadosMeteorologicosDto>>> GetWeatherPrevisao(int estufaId, [FromQuery] int dias = 7)
    {
        try
        {
            var estufa = await _context.Estufas.FindAsync(estufaId);
            if (estufa == null)
                return NotFound(new { mensagem = "Estufa não encontrada" });

            var historico = await _weatherService.GetWeatherHistoricoAsync(estufaId, dias);
            return Ok(historico);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter previsão: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter previsão" });
        }
    }

    /// <summary>
    /// GET /api/meteorologia/historico/{estufaId}
    /// Obtém o histórico de dados meteorológicos com filtro.
    /// </summary>
    [HttpGet("historico/{estufaId}")]
    public async Task<ActionResult<List<DadosMeteorologicosDto>>> GetWeatherHistorico(
        int estufaId,
        [FromQuery] DateTime? dataInicio = null,
        [FromQuery] DateTime? dataFim = null)
    {
        try
        {
            var estufa = await _context.Estufas.FindAsync(estufaId);
            if (estufa == null)
                return NotFound(new { mensagem = "Estufa não encontrada" });

            var query = _context.DadosMeteorologicos.Where(d => d.EstufaId == estufaId);

            if (dataInicio.HasValue)
                query = query.Where(d => d.DataHoraConsulta >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(d => d.DataHoraConsulta <= dataFim.Value);

            var dados = await query
                .OrderByDescending(d => d.DataHoraConsulta)
                .Select(d => new DadosMeteorologicosDto
                {
                    Id = d.Id,
                    EstufaId = d.EstufaId,
                    TemperaturaExterna = d.TemperaturaExterna,
                    UmidadeExterna = d.UmidadeExterna,
                    CondicaoClimatica = d.CondicaoClimatica,
                    EstaChovendo = d.EstaChovendo,
                    ProbabilidadeChuva = d.ProbabilidadeChuva,
                    PrevisaoChuva = d.PrevisaoChuva,
                    VelocidadeVento = d.VelocidadeVento,
                    DataHoraConsulta = d.DataHoraConsulta,
                    FonteAPI = d.FonteAPI
                })
                .ToListAsync();

            return Ok(dados);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter histórico meteorológico: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter histórico" });
        }
    }
}
