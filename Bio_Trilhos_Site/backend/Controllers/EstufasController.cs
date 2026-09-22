using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioTrilhos.API.Data;
using BioTrilhos.API.DTOs;
using BioTrilhos.API.Models;

namespace BioTrilhos.API.Controllers;

/// <summary>
/// Controller para gerenciar estufas.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EstufasController : ControllerBase
{
    private readonly BioTrilhosDbContext _context;
    private readonly ILogger<EstufasController> _logger;

    public EstufasController(BioTrilhosDbContext context, ILogger<EstufasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/estufas
    /// Lista todas as estufas.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<EstufaDto>>> GetEstufas()
    {
        try
        {
            var estufas = await _context.Estufas
                .Select(e => new EstufaDto
                {
                    Id = e.Id,
                    Nome = e.Nome,
                    Localizacao = e.Localizacao,
                    Descricao = e.Descricao,
                    Ativa = e.Ativa,
                    DataCriacao = e.DataCriacao
                })
                .ToListAsync();

            return Ok(estufas);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao listar estufas: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao listar estufas" });
        }
    }

    /// <summary>
    /// GET /api/estufas/{id}
    /// Obtém uma estufa específica.
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<EstufaDto>> GetEstufa(int id)
    {
        try
        {
            var estufa = await _context.Estufas.FindAsync(id);
            if (estufa == null)
                return NotFound(new { mensagem = "Estufa não encontrada" });

            return Ok(new EstufaDto
            {
                Id = estufa.Id,
                Nome = estufa.Nome,
                Localizacao = estufa.Localizacao,
                Descricao = estufa.Descricao,
                Ativa = estufa.Ativa,
                DataCriacao = estufa.DataCriacao
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter estufa: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter estufa" });
        }
    }

    /// <summary>
    /// POST /api/estufas
    /// Cria uma nova estufa.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<EstufaDto>> CreateEstufa([FromBody] CreateEstufaDto dto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dto.Nome))
                return BadRequest(new { mensagem = "Nome da estufa é obrigatório" });

            var estufa = new Estufa
            {
                Nome = dto.Nome,
                Localizacao = dto.Localizacao,
                Descricao = dto.Descricao,
                Ativa = dto.Ativa,
                DataCriacao = DateTime.UtcNow
            };

            _context.Estufas.Add(estufa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEstufa), new { id = estufa.Id }, new EstufaDto
            {
                Id = estufa.Id,
                Nome = estufa.Nome,
                Localizacao = estufa.Localizacao,
                Descricao = estufa.Descricao,
                Ativa = estufa.Ativa,
                DataCriacao = estufa.DataCriacao
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao criar estufa: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao criar estufa" });
        }
    }

    /// <summary>
    /// PUT /api/estufas/{id}
    /// Atualiza uma estufa.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<EstufaDto>> UpdateEstufa(int id, [FromBody] CreateEstufaDto dto)
    {
        try
        {
            var estufa = await _context.Estufas.FindAsync(id);
            if (estufa == null)
                return NotFound(new { mensagem = "Estufa não encontrada" });

            estufa.Nome = dto.Nome;
            estufa.Localizacao = dto.Localizacao;
            estufa.Descricao = dto.Descricao;
            estufa.Ativa = dto.Ativa;

            _context.Estufas.Update(estufa);
            await _context.SaveChangesAsync();

            return Ok(new EstufaDto
            {
                Id = estufa.Id,
                Nome = estufa.Nome,
                Localizacao = estufa.Localizacao,
                Descricao = estufa.Descricao,
                Ativa = estufa.Ativa,
                DataCriacao = estufa.DataCriacao
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao atualizar estufa: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao atualizar estufa" });
        }
    }

    /// <summary>
    /// DELETE /api/estufas/{id}
    /// Deleta uma estufa.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteEstufa(int id)
    {
        try
        {
            var estufa = await _context.Estufas.FindAsync(id);
            if (estufa == null)
                return NotFound(new { mensagem = "Estufa não encontrada" });

            _context.Estufas.Remove(estufa);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao deletar estufa: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao deletar estufa" });
        }
    }
}
