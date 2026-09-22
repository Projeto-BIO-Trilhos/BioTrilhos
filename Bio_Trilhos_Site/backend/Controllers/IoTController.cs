using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BioTrilhos.API.Data;
using BioTrilhos.API.DTOs;
using BioTrilhos.API.Models;

namespace BioTrilhos.API.Controllers;

/// <summary>
/// Controller para receber dados do Arduino/IoT.
/// Ponto de entrada para dispositivos conectados.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class IoTController : ControllerBase
{
    private readonly BioTrilhosDbContext _context;
    private readonly IAutomationService _automationService;
    private readonly ILogger<IoTController> _logger;

    public IoTController(BioTrilhosDbContext context, IAutomationService automationService, ILogger<IoTController> logger)
    {
        _context = context;
        _automationService = automationService;
        _logger = logger;
    }

    /// <summary>
    /// POST /api/iot/leituras
    /// Recebe leituras de sensores do Arduino/IoT.
    /// Exemplo de payload JSON:
    /// {
    ///   "dispositivoId": "ARDUINO_ESTUFA_01",
    ///   "temperatura": 26.5,
    ///   "umidadeAr": 72,
    ///   "umidadeSolo": 48,
    ///   "nivelAgua": 80,
    ///   "dataHora": "2026-09-08T14:00:00"
    /// }
    /// </summary>
    [HttpPost("leituras")]
    public async Task<ActionResult<IoTRespostaDto>> ReceberLeituras([FromBody] IoTLeituraDto dto)
    {
        var resposta = new IoTRespostaDto();

        try
        {
            if (string.IsNullOrWhiteSpace(dto.DispositivoId))
            {
                resposta.Erros.Add("DispositivoId é obrigatório");
                return BadRequest(resposta);
            }

            // Encontrar a estufa associada ao dispositivo
            var estufa = await _context.Estufas
                .FirstOrDefaultAsync(e => e.Ativa);

            if (estufa == null)
            {
                resposta.Erros.Add("Nenhuma estufa ativa encontrada");
                return BadRequest(resposta);
            }

            // Processar temperatura
            if (dto.Temperatura.HasValue)
            {
                await SalvarLeituraSensor(estufa.Id, TipoSensor.Temperatura, dto.Temperatura.Value, "°C");
            }

            // Processar umidade do ar
            if (dto.UmidadeAr.HasValue)
            {
                await SalvarLeituraSensor(estufa.Id, TipoSensor.UmidadeDoAr, dto.UmidadeAr.Value, "%");
            }

            // Processar umidade do solo
            if (dto.UmidadeSolo.HasValue)
            {
                await SalvarLeituraSensor(estufa.Id, TipoSensor.UmidadeDoSolo, dto.UmidadeSolo.Value, "%");
            }

            // Processar nível de água
            if (dto.NivelAgua.HasValue)
            {
                // Atualizar reservatório
                var reservatorio = await _context.Reservatorios
                    .FirstOrDefaultAsync(r => r.EstufaId == estufa.Id);

                if (reservatorio != null)
                {
                    reservatorio.PercentualAtual = dto.NivelAgua.Value;
                    reservatorio.NivelAtual = (dto.NivelAgua.Value / 100) * reservatorio.CapacidadeMaxima;
                    reservatorio.DataUltimaAtualizacao = DateTime.UtcNow;

                    // Atualizar status
                    var config = await _context.ConfiguracoesAmbientais
                        .FirstOrDefaultAsync(c => c.EstufaId == estufa.Id);

                    reservatorio.Status = reservatorio.PercentualAtual < config?.NivelMinimoReservatorio ? "Baixo" : "Normal";

                    _context.Reservatorios.Update(reservatorio);
                }
            }

            // Processar sensor de chuva
            if (dto.Chuva.HasValue)
            {
                var sensorChuva = await _context.Sensores
                    .FirstOrDefaultAsync(s => s.EstufaId == estufa.Id && s.TipoSensor == TipoSensor.Chuva);

                if (sensorChuva != null)
                {
                    var leitura = new LeituraSensor
                    {
                        SensorId = sensorChuva.Id,
                        Valor = dto.Chuva.Value ? 1 : 0,
                        DataHoraLeitura = dto.DataHora,
                        UnidadeMedida = "bool",
                        Status = "OK"
                    };

                    _context.LeiturasSensores.Add(leitura);
                }
            }

            await _context.SaveChangesAsync();

            // Verificar automação
            await _automationService.VerificarEAcionarIrrigacaoAsync(estufa.Id);
            await _automationService.VerificarAlertas(estufa.Id);

            resposta.Sucesso = true;
            resposta.Mensagem = "Leituras processadas com sucesso";

            _logger.LogInformation($"Leituras recebidas do Arduino {dto.DispositivoId}: Temp={dto.Temperatura}, Umidade Solo={dto.UmidadeSolo}%");

            return Ok(resposta);
        }
        catch (Exception ex)
        {
            resposta.Sucesso = false;
            resposta.Mensagem = "Erro ao processar leituras";
            resposta.Erros.Add(ex.Message);

            _logger.LogError($"Erro ao processar leituras IoT: {ex.Message}");
            return StatusCode(500, resposta);
        }
    }

    /// <summary>
    /// GET /api/iot/comandos/{dispositivoId}
    /// Retorna comandos pendentes para o Arduino executar.
    /// Possibilita controle remoto do dispositivo.
    /// </summary>
    [HttpGet("comandos/{dispositivoId}")]
    public async Task<ActionResult<object>> ObterComandos(string dispositivoId)
    {
        try
        {
            // Buscar estufa ativa
            var estufa = await _context.Estufas
                .FirstOrDefaultAsync(e => e.Ativa);

            if (estufa == null)
                return NotFound(new { mensagem = "Estufa não encontrada" });

            // Verificar se há irrigação ativa
            var irrigacaoAtiva = await _context.Irrigacoes
                .Where(i => i.EstufaId == estufa.Id && i.Status == StatusIrrigacao.Ativa)
                .FirstOrDefaultAsync();

            var comandos = new List<object>();

            if (irrigacaoAtiva != null)
            {
                comandos.Add(new
                {
                    tipo = "LIGAR_IRRIGACAO",
                    duracao = irrigacaoAtiva.DuracaoSegundos,
                    prioridade = "alta"
                });
            }
            else
            {
                comandos.Add(new
                {
                    tipo = "DESLIGAR_IRRIGACAO",
                    prioridade = "media"
                });
            }

            // Retornar configurações atuais
            var config = await _context.ConfiguracoesAmbientais
                .FirstOrDefaultAsync(c => c.EstufaId == estufa.Id);

            if (config != null)
            {
                comandos.Add(new
                {
                    tipo = "ATUALIZAR_CONFIGURACAO",
                    umidadeSoloMinima = config.UmidadeSoloMinima,
                    umidadeSoloMaxima = config.UmidadeSoloMaxima,
                    tempoMaximoIrrigacao = config.TempoMaximoIrrigacaoSegundos
                });
            }

            return Ok(new
            {
                dispositivoId = dispositivoId,
                tempoConsulta = DateTime.UtcNow,
                comandos = comandos
            });
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao obter comandos: {ex.Message}");
            return StatusCode(500, new { mensagem = "Erro ao obter comandos" });
        }
    }

    /// <summary>
    /// Salva uma leitura de sensor na base de dados.
    /// </summary>
    private async Task SalvarLeituraSensor(int estufaId, TipoSensor tipoSensor, decimal valor, string unidade)
    {
        var sensor = await _context.Sensores
            .FirstOrDefaultAsync(s => s.EstufaId == estufaId && s.TipoSensor == tipoSensor);

        if (sensor != null)
        {
            var leitura = new LeituraSensor
            {
                SensorId = sensor.Id,
                Valor = valor,
                DataHoraLeitura = DateTime.UtcNow,
                UnidadeMedida = unidade,
                Status = "OK"
            };

            _context.LeiturasSensores.Add(leitura);
        }
    }
}
