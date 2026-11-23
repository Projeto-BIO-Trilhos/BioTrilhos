<?php

namespace App\Http\Controllers;

use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Http;

class MeteorologiaController extends Controller
{
    public function current($estufaId)
    {
        try {
            $response = Http::timeout(10)->acceptJson()->get('https://api.open-meteo.com/v1/forecast', [
                'latitude' => '-22.6597',
                'longitude' => '-44.9737',
                'current' => 'temperature_2m,relative_humidity_2m,precipitation,wind_speed_10m',
                'timezone' => 'America/Sao_Paulo',
            ]);

            if (!$response->ok()) {
                throw new \RuntimeException('Open-Meteo respondeu com status '.$response->status());
            }

            $current = $response->json('current');
            if (!is_array($current)) {
                throw new \RuntimeException('Resposta Open-Meteo sem dados atuais');
            }

            $temperatura = (float) ($current['temperature_2m'] ?? 25.5);
            $umidade = (float) ($current['relative_humidity_2m'] ?? 68);
            $vento = (float) ($current['wind_speed_10m'] ?? 8);
            $chovendo = (float) ($current['precipitation'] ?? 0) > 0;
            $probabilidade = $chovendo ? 35 : 25;
            $fonte = 'Open-Meteo';
        } catch (\Throwable $e) {
            $temperatura = (float) (DB::table('dados_meteorologicos')->where('estufa_id', $estufaId)->orderByDesc('data_hora_consulta')->value('temperatura_externa') ?? 25.5);
            $umidade = 68;
            $vento = 8;
            $chovendo = false;
            $probabilidade = (float) (DB::table('dados_meteorologicos')->where('estufa_id', $estufaId)->orderByDesc('data_hora_consulta')->value('probabilidade_chuva') ?? 25);
            $fonte = 'Fallback local';
        }

        return response()->json([
            'temperaturaExterna' => $temperatura,
            'umidadeExterna' => $umidade,
            'velocidadeVento' => $vento,
            'estaChovendo' => $chovendo,
            'condicaoClimatica' => $chovendo ? 'Chuvoso' : 'Parcialmente limpo',
            'probabilidadeChuva' => $probabilidade,
            'fonte' => $fonte,
        ]);
    }
}
