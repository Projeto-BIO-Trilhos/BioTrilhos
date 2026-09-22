using BioTrilhos.API.Data;
using BioTrilhos.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BioTrilhos.API.Services;

/// <summary>
/// Interface para o serviço de automação de irrigação.
/// </summary>
public interface IAutomationService
{
    Task<bool> VerificarEAcionarIrrigacaoAsync(int estufaId);
    Task PararIrrigacaoAsync(int estufaId);
    Task VerificarAlertas(int estufaId);
}

/// <summary>
/// Serviço de lógica de automação para irrigação e controle da estufa.
/// Implementa as regras de negócio para tomar decisões automáticas.
/// </summary>
public class AutomationService : IAutomationService
{
    private readonly BioTrilhosDbContext _context;
    private readonly ILogger<AutomationService> _logger;

    public AutomationService(BioTrilhosDbContext context, ILogger<AutomationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Verifica as condições e decide se deve acionar a irrigação automaticamente.
    /// Lógica de negócio baseada em sensor de umidade do solo, disponibilidade de água, etc.
    /// </summary>
    public async Task<bool> VerificarEAcionarIrrigacaoAsync(int estufaId)
    {
        try
        {
            // 1. Obter configurações ambientais da estufa
            var config = await _context.ConfiguracoesAmbientais
                .FirstOrDefaultAsync(c => c.EstufaId == estufaId);

            if (config == null)
            {
                _logger.LogWarning($"Configurações ambientais não encontradas para estufa {estufaId}");
                return false;
            }

            // 2. Obter última leitura de umidade do solo
            var sensorUmidadeSolo = await _context.Sensores
                .FirstOrDefaultAsync(s => s.EstufaId == estufaId && s.TipoSensor == TipoSensor.UmidadeDoSolo);

            if (sensorUmidadeSolo == null)
            {
                _logger.LogWarning($"Sensor de umidade do solo não encontrado para estufa {estufaId}");
                return false;
            }

            var ultimaLeitura = await _context.LeiturasSensores
                .Where(l => l.SensorId == sensorUmidadeSolo.Id)
                .OrderByDescending(l => l.DataHoraLeitura)
                .FirstOrDefaultAsync();

            if (ultimaLeitura == null)
            {
                _logger.LogWarning($"Nenhuma leitura de umidade do solo para estufa {estufaId}");
                return false;
            }

            // 3. Verificar se umidade está abaixo do mínimo
            if (ultimaLeitura.Valor >= config.UmidadeSoloMinima)
            {
                _logger.LogInformation($"Umidade do solo ({ultimaLeitura.Valor}%) está dentro da faixa. Irrigação não necessária.");
                return false;
            }

            // 4. Verificar disponibilidade de água no reservatório
            var reservatorio = await _context.Reservatorios
                .FirstOrDefaultAsync(r => r.EstufaId == estufaId);

            if (reservatorio == null || reservatorio.PercentualAtual < config.NivelMinimoReservatorio)
            {
                _logger.LogWarning($"Nível de água insuficiente para iniciar irrigação. Nível: {reservatorio?.PercentualAtual}%");
                
                // Criar alerta de nível baixo
                await CriarAlertaAsync(estufaId, "NivelBaixoReservatorio",
                    $"Nível de água no reservatório abaixo do mínimo ({reservatorio?.PercentualAtual}%)",
                    NivelAlerta.Critico);

                return false;
            }

            // 5. Verificar condições meteorológicas - não irrigar se estiver chovendo
            var dadosMeteo = await _context.DadosMeteorologicos
                .Where(d => d.EstufaId == estufaId)
                .OrderByDescending(d => d.DataHoraConsulta)
                .FirstOrDefaultAsync();

            if (dadosMeteo != null && dadosMeteo.EstaChovendo)
            {
                _logger.LogInformation("Está chovendo. Irrigação será suspensa.");
                return false;
            }

            // 6. Verificar se já existe irrigação ativa
            var irrigacaoAtiva = await _context.Irrigacoes
                .Where(i => i.EstufaId == estufaId && i.Status == StatusIrrigacao.Ativa)
                .FirstOrDefaultAsync();

            if (irrigacaoAtiva != null)
            {
                _logger.LogInformation("Já existe uma irrigação ativa para esta estufa.");
                return false;
            }

            // 7. Iniciar irrigação automática
            var irrigacao = new Irrigacao
            {
                EstufaId = estufaId,
                ReservatorioId = reservatorio.Id,
                DataHoraInicio = DateTime.UtcNow,
                DuracaoSegundos = config.TempoMaximoIrrigacaoSegundos,
                Motivo = $"Automático - Umidade do solo em {ultimaLeitura.Valor}%",
                TipoAcionamento = TipoAcionamento.Automatico,
                Status = StatusIrrigacao.Ativa
            };

            _context.Irrigacoes.Add(irrigacao);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Irrigação automática iniciada para estufa {estufaId}");

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao verificar automação: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Para a irrigação ativa de uma estufa.
    /// </summary>
    public async Task PararIrrigacaoAsync(int estufaId)
    {
        try
        {
            var irrigacao = await _context.Irrigacoes
                .Where(i => i.EstufaId == estufaId && i.Status == StatusIrrigacao.Ativa)
                .FirstOrDefaultAsync();

            if (irrigacao != null)
            {
                irrigacao.Status = StatusIrrigacao.Finalizada;
                irrigacao.DataHoraFim = DateTime.UtcNow;

                // Atualizar duração real
                if (irrigacao.DataHoraFim.HasValue)
                {
                    irrigacao.DuracaoSegundos = (int)(irrigacao.DataHoraFim.Value - irrigacao.DataHoraInicio).TotalSeconds;
                }

                // Registrar saída de água no reservatório
                if (irrigacao.ReservatorioId.HasValue)
                {
                    var reservatorio = await _context.Reservatorios.FindAsync(irrigacao.ReservatorioId);
                    if (reservatorio != null)
                    {
                        var consumoEstimado = (irrigacao.DuracaoSegundos / 60.0m) * 5; // Estimativa: 5L por minuto
                        var nivelAnterior = reservatorio.NivelAtual;
                        
                        reservatorio.NivelAtual = Math.Max(0, reservatorio.NivelAtual - consumoEstimado);
                        reservatorio.PercentualAtual = (reservatorio.NivelAtual / reservatorio.CapacidadeMaxima) * 100;
                        reservatorio.DataUltimaAtualizacao = DateTime.UtcNow;

                        // Registrar no histórico
                        var historico = new HistoricoReservatorio
                        {
                            ReservatorioId = reservatorio.Id,
                            NivelAnterior = nivelAnterior,
                            NivelAtual = reservatorio.NivelAtual,
                            TipoMovimentacao = TipoMovimentacao.ConsumoIrrigacao,
                            Descricao = $"Consumo de irrigação: {consumoEstimado}L",
                            DataHora = DateTime.UtcNow
                        };

                        _context.HistoricosReservatorios.Add(historico);
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Irrigação parada para estufa {estufaId}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao parar irrigação: {ex.Message}");
        }
    }

    /// <summary>
    /// Verifica as condições e gera alertas quando necessário.
    /// </summary>
    public async Task VerificarAlertas(int estufaId)
    {
        try
        {
            var config = await _context.ConfiguracoesAmbientais
                .FirstOrDefaultAsync(c => c.EstufaId == estufaId);

            if (config == null) return;

            // Obter últimas leituras
            var sensorTemp = await _context.Sensores
                .FirstOrDefaultAsync(s => s.EstufaId == estufaId && s.TipoSensor == TipoSensor.Temperatura);
            
            var sensorUmidadeAr = await _context.Sensores
                .FirstOrDefaultAsync(s => s.EstufaId == estufaId && s.TipoSensor == TipoSensor.UmidadeDoAr);
            
            var sensorUmidadeSolo = await _context.Sensores
                .FirstOrDefaultAsync(s => s.EstufaId == estufaId && s.TipoSensor == TipoSensor.UmidadeDoSolo);

            // Verificar temperatura
            if (sensorTemp != null)
            {
                var ultimaTemp = await _context.LeiturasSensores
                    .Where(l => l.SensorId == sensorTemp.Id)
                    .OrderByDescending(l => l.DataHoraLeitura)
                    .FirstOrDefaultAsync();

                if (ultimaTemp != null)
                {
                    if (ultimaTemp.Valor < config.TemperaturaMinima)
                    {
                        await CriarAlertaAsync(estufaId, "TemperaturaBaixa",
                            $"Temperatura muito baixa: {ultimaTemp.Valor}°C (mínimo: {config.TemperaturaMinima}°C)",
                            NivelAlerta.Aviso);
                    }
                    else if (ultimaTemp.Valor > config.TemperaturaMaxima)
                    {
                        await CriarAlertaAsync(estufaId, "TemperaturaAlta",
                            $"Temperatura muito alta: {ultimaTemp.Valor}°C (máximo: {config.TemperaturaMaxima}°C)",
                            NivelAlerta.Aviso);
                    }
                }
            }

            // Verificar umidade do solo
            if (sensorUmidadeSolo != null)
            {
                var ultimaUmidade = await _context.LeiturasSensores
                    .Where(l => l.SensorId == sensorUmidadeSolo.Id)
                    .OrderByDescending(l => l.DataHoraLeitura)
                    .FirstOrDefaultAsync();

                if (ultimaUmidade != null && ultimaUmidade.Valor < config.UmidadeSoloMinima)
                {
                    await CriarAlertaAsync(estufaId, "UmidadeSoloBaixa",
                        $"Umidade do solo baixa: {ultimaUmidade.Valor}% (mínimo: {config.UmidadeSoloMinima}%)",
                        NivelAlerta.Critico);
                }
            }

            // Verificar reservatório
            var reservatorio = await _context.Reservatorios
                .FirstOrDefaultAsync(r => r.EstufaId == estufaId);

            if (reservatorio != null && reservatorio.PercentualAtual < config.NivelMinimoReservatorio)
            {
                await CriarAlertaAsync(estufaId, "NivelBaixoReservatorio",
                    $"Nível de água baixo: {reservatorio.PercentualAtual}% (mínimo: {config.NivelMinimoReservatorio}%)",
                    NivelAlerta.Critico);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao verificar alertas: {ex.Message}");
        }
    }

    /// <summary>
    /// Cria um alerta se não existir um não resolvido do mesmo tipo.
    /// </summary>
    private async Task CriarAlertaAsync(int estufaId, string tipo, string mensagem, NivelAlerta nivel)
    {
        try
        {
            var alertaExistente = await _context.Alertas
                .Where(a => a.EstufaId == estufaId && a.Tipo == tipo && !a.Resolvido)
                .FirstOrDefaultAsync();

            if (alertaExistente != null)
                return; // Já existe alerta não resolvido deste tipo

            var alerta = new Alerta
            {
                EstufaId = estufaId,
                Tipo = tipo,
                Mensagem = mensagem,
                Nivel = nivel,
                Resolvido = false,
                DataCriacao = DateTime.UtcNow
            };

            _context.Alertas.Add(alerta);
            await _context.SaveChangesAsync();

            _logger.LogWarning($"Alerta criado: {tipo} - {mensagem}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao criar alerta: {ex.Message}");
        }
    }
}
