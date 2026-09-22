using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioTrilhos.API.Data;
using BioTrilhos.API.DTOs;
using BioTrilhos.API.Models;

namespace BioTrilhos.API.Controllers;

/// <summary>
/// Controller para o endpoint do Dashboard.
/// Retorna as principais informações para a interface do usuário.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly BioTrilhosDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(BioTrilhosDbContext context, ILogger<DashboardController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// GET /api/dashboard/{estufaId}
    /// Retorna os dados principais do dashboard para uma estufa.
    /// </summary>
    [HttpGet("{estufaId}")]
    public async Task<ActionResult<DashboardDto>> GetDashboard(int estufaId)
    {
        try
        {
            // Verificar se estufa existe
            var estufa = await _context.Estufas.FindAsync(estufaId);
            if (estufa == null)
                return NotFound(new { mensagem = "Estufa não encontrada" });

            var dashboard = new DashboardDto
            {
                EstufaId = estufaId,
                EstufaNome = estufa.Nome,
                UltimaAtualizacao = DateTime.UtcNow,
                SistemaOperacional = true
            };

            // Obter últimas leituras de sensores
            await CarregarDadosSensores(estufaId, dashboard);

            // Obter dados do reservatório
            await CarregarDadosReservatorio(estufaId, dashboard);

            // Obter dados de irrigação
            await CarregarDadosIrrigacao(estufaId, dashboard);

            // Obter dados meteorológicos
            await CarregarDadosMetereologicos(estufaId, dashboard);

            // Obter alertas ativos
            await CarregarAlertas(estufaId, dashboard);

            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter dashboard: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter dados do dashboard" });
        }
    }

    private async Task CarregarDadosSensores(int estufaId, DashboardDto dashboard)
    {
        try
        {
            // Temperatura
            var sensorTemp = await _context.Sensores
                .FirstOrDefaultAsync(s => s.EstufaId == estufaId && s.TipoSensor == TipoSensor.Temperatura);
            
            if (sensorTemp != null)
            {
                var leituraTemp = await _context.LeiturasSensores
                    .Where(l => l.SensorId == sensorTemp.Id)
                    .OrderByDescending(l => l.DataHoraLeitura)
                    .FirstOrDefaultAsync();

                if (leituraTemp != null)
                {
                    dashboard.TemperaturaAtual = leituraTemp.Valor;
                }
            }

            // Umidade do ar
            var sensorAr = await _context.Sensores
                .FirstOrDefaultAsync(s => s.EstufaId == estufaId && s.TipoSensor == TipoSensor.UmidadeDoAr);
            
            if (sensorAr != null)
            {
                var leituraAr = await _context.LeiturasSensores
                    .Where(l => l.SensorId == sensorAr.Id)
                    .OrderByDescending(l => l.DataHoraLeitura)
                    .FirstOrDefaultAsync();

                if (leituraAr != null)
                {
                    dashboard.UmidadeArAtual = leituraAr.Valor;
                }
            }

            // Umidade do solo
            var sensorSolo = await _context.Sensores
                .FirstOrDefaultAsync(s => s.EstufaId == estufaId && s.TipoSensor == TipoSensor.UmidadeDoSolo);
            
            if (sensorSolo != null)
            {
                var leituraSolo = await _context.LeiturasSensores
                    .Where(l => l.SensorId == sensorSolo.Id)
                    .OrderByDescending(l => l.DataHoraLeitura)
                    .FirstOrDefaultAsync();

                if (leituraSolo != null)
                {
                    dashboard.UmidadeSoloAtual = leituraSolo.Valor;
                }
            }

            dashboard.UltimaAtualizacaoSensores = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar dados de sensores: {ex.Message}");
        }
    }

    private async Task CarregarDadosReservatorio(int estufaId, DashboardDto dashboard)
    {
        try
        {
            var reservatorio = await _context.Reservatorios
                .FirstOrDefaultAsync(r => r.EstufaId == estufaId);

            if (reservatorio != null)
            {
                dashboard.ReservatorioNivel = reservatorio.NivelAtual;
                dashboard.ReservatorioCapacidade = reservatorio.CapacidadeMaxima;
                dashboard.ReservatorioPercentual = reservatorio.PercentualAtual;
                dashboard.ReservatorioStatus = reservatorio.Status;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar dados do reservatório: {ex.Message}");
        }
    }

    private async Task CarregarDadosIrrigacao(int estufaId, DashboardDto dashboard)
    {
        try
        {
            // Verificar irrigação ativa
            var irrigacaoAtiva = await _context.Irrigacoes
                .Where(i => i.EstufaId == estufaId && i.Status == StatusIrrigacao.Ativa)
                .FirstOrDefaultAsync();

            dashboard.IrrigacaoAtiva = irrigacaoAtiva != null;

            // Última irrigação
            var ultimaIrrigacao = await _context.Irrigacoes
                .Where(i => i.EstufaId == estufaId && i.Status == StatusIrrigacao.Finalizada)
                .OrderByDescending(i => i.DataHoraFim)
                .FirstOrDefaultAsync();

            if (ultimaIrrigacao != null)
            {
                dashboard.UltimaIrrigacao = ultimaIrrigacao.DataHoraFim;
                dashboard.DuracaoUltimaIrrigacao = ultimaIrrigacao.DuracaoSegundos;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar dados de irrigação: {ex.Message}");
        }
    }

    private async Task CarregarDadosMetereologicos(int estufaId, DashboardDto dashboard)
    {
        try
        {
            var dadosMeteo = await _context.DadosMeteorologicos
                .Where(d => d.EstufaId == estufaId)
                .OrderByDescending(d => d.DataHoraConsulta)
                .FirstOrDefaultAsync();

            if (dadosMeteo != null)
            {
                dashboard.TemperaturaExterna = dadosMeteo.TemperaturaExterna ?? 0;
                dashboard.UmidadeExterna = dadosMeteo.UmidadeExterna ?? 0;
                dashboard.CondicaoClimatica = dadosMeteo.CondicaoClimatica;
                dashboard.EstaChovendo = dadosMeteo.EstaChovendo;
                dashboard.ProbabilidadeChuva = dadosMeteo.ProbabilidadeChuva;
                dashboard.UltimaAtualizacaoClima = dadosMeteo.DataHoraConsulta;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar dados meteorológicos: {ex.Message}");
        }
    }

    private async Task CarregarAlertas(int estufaId, DashboardDto dashboard)
    {
        try
        {
            var alertas = await _context.Alertas
                .Where(a => a.EstufaId == estufaId && !a.Resolvido)
                .OrderByDescending(a => a.DataCriacao)
                .Take(10)
                .ToListAsync();

            dashboard.AlertasAtivos = alertas.Count;
            dashboard.AlertasRecentes = alertas.Select(a => new AlertaDto
            {
                Id = a.Id,
                Tipo = a.Tipo,
                Mensagem = a.Mensagem,
                Nivel = (int)a.Nivel,
                Resolvido = a.Resolvido,
                DataCriacao = a.DataCriacao
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar alertas: {ex.Message}");
        }
    }
}
