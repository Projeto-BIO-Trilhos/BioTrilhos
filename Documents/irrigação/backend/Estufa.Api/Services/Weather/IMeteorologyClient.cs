using System.Threading.Tasks;

namespace Estufa.Api.Services.Weather
{
    public interface IMeteorologyClient
    {
        /// <summary>
        /// Consulta a API meteorológica e retorna se há previsão de chuva nas próximas 24h e o payload bruto.
        /// </summary>
        Task<(bool rainExpected, string rawJson)> GetRainForecastNext24hAsync(double lat, double lon);

        /// <summary>
        /// Retorna o payload bruto da API meteorológica (onecall) incluindo dados atuais.
        /// </summary>
        Task<string> GetWeatherRawAsync(double lat, double lon);
    }
}
