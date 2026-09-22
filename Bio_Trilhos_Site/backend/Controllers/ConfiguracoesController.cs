using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioTrilhos.API.Data;
using BioTrilhos.API.DTOs;
using BioTrilhos.API.Models;

namespace BioTrilhos.API.Controllers;

/// <summary>
/// Controller para gerenciar configurações ambientais das estufas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConfiguracoesController : ControllerBase
{
    private readonly BioTrilhosDbContext _context;
    private readonly ILogger<ConfiguracoesController> _logger;

    public ConfiguracoesController(BioTrilhosDbContext context, ILogger<ConfiguracoesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/configuracoes/{estufaId}
    /// Obtém as configurações ambientais de uma estufa.
    /// </summary>
    [HttpGet("{estufaId}")]
    public async Task<ActionResult<ConfiguracoesAmbientaisDto>> GetConfiguracoes(int estufaId)
    {
        try
        {
            var config = await _context.ConfiguracoesAmbientais
                .FirstOrDefaultAsync(c => c.EstufaId == estufaId);

            if (config == null)
                return NotFound(new { mensagem = "Configurações não encontradas" });

            return Ok(new ConfiguracoesAmbientaisDto
            {
                Id = config.Id,
                EstufaId = config.EstufaId,
                UmidadeSoloMinima = config.UmidadeSoloMinima,
                UmidadeSoloMaxima = config.UmidadeSoloMaxima,
                TemperaturaMinima = config.TemperaturaMinima,
                TemperaturaMaxima = config.TemperaturaMaxima,
                UmidadeArMinima = config.UmidadeArMinima,
                UmidadeArMaxima = config.UmidadeArMaxima,
                TempoMaximoIrrigacaoSegundos = config.TempoMaximoIrrigacaoSegundos,
                NivelMinimoReservatorio = config.NivelMinimoReservatorio,
                AtualizadoEm = config.AtualizadoEm
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter configurações: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter configurações" });
        }
    }

    /// <summary>
    /// PUT /api/configuracoes/{estufaId}
    /// Atualiza as configurações ambientais de uma estufa.
    /// </summary>
    [HttpPut("{estufaId}")]
    public async Task<ActionResult<ConfiguracoesAmbientaisDto>> UpdateConfiguracoes(
        int estufaId,
        [FromBody] UpdateConfiguracoesAmbientaisDto dto)
    {
        try
        {
            var config = await _context.ConfiguracoesAmbientais
                .FirstOrDefaultAsync(c => c.EstufaId == estufaId);

            if (config == null)
                return NotFound(new { mensagem = "Configurações não encontradas" });

            // Atualizar apenas os campos fornecidos
            if (dto.UmidadeSoloMinima.HasValue)
                config.UmidadeSoloMinima = dto.UmidadeSoloMinima.Value;

            if (dto.UmidadeSoloMaxima.HasValue)
                config.UmidadeSoloMaxima = dto.UmidadeSoloMaxima.Value;

            if (dto.TemperaturaMinima.HasValue)
                config.TemperaturaMinima = dto.TemperaturaMinima.Value;

            if (dto.TemperaturaMaxima.HasValue)
                config.TemperaturaMaxima = dto.TemperaturaMaxima.Value;

            if (dto.UmidadeArMinima.HasValue)
                config.UmidadeArMinima = dto.UmidadeArMinima.Value;

            if (dto.UmidadeArMaxima.HasValue)
                config.UmidadeArMaxima = dto.UmidadeArMaxima.Value;

            if (dto.TempoMaximoIrrigacaoSegundos.HasValue)
                config.TempoMaximoIrrigacaoSegundos = dto.TempoMaximoIrrigacaoSegundos.Value;

            if (dto.NivelMinimoReservatorio.HasValue)
                config.NivelMinimoReservatorio = dto.NivelMinimoReservatorio.Value;

            config.AtualizadoEm = DateTime.UtcNow;

            _context.ConfiguracoesAmbientais.Update(config);
            await _context.SaveChangesAsync();

            return Ok(new ConfiguracoesAmbientaisDto
            {
                Id = config.Id,
                EstufaId = config.EstufaId,
                UmidadeSoloMinima = config.UmidadeSoloMinima,
                UmidadeSoloMaxima = config.UmidadeSoloMaxima,
                TemperaturaMinima = config.TemperaturaMinima,
                TemperaturaMaxima = config.TemperaturaMaxima,
                UmidadeArMinima = config.UmidadeArMinima,
                UmidadeArMaxima = config.UmidadeArMaxima,
                TempoMaximoIrrigacaoSegundos = config.TempoMaximoIrrigacaoSegundos,
                NivelMinimoReservatorio = config.NivelMinimoReservatorio,
                AtualizadoEm = config.AtualizadoEm
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao atualizar configurações: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao atualizar configurações" });
        }
    }
}
