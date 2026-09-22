<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Http;

class MeteorologiaController extends Controller
{
    public function current($estufaId)
    {
        // Exemplo: consulta API Open-Meteo e retorna resultado simplificado
        $lat = '-22.6597';
        $lon = '-44.9737';
        $url = "https://api.open-meteo.com/v1/forecast?latitude={$lat}&longitude={$lon}&current_weather=true&timezone=America/Sao_Paulo";

        try {
            $resp = Http::get($url);
            if (!$resp->ok()) return response()->json(['erro' => 'Falha na API meteorológica'], 500);

            $json = $resp->json();
            $current = $json['current_weather'] ?? null;

            return response()->json([
                'temperaturaExterna' => $current['temperature'] ?? null,
                'velocidadeVento' => $current['windspeed'] ?? null,
                'condicao' => $current['weathercode'] ?? null,
                'dataHora' => now()
            ]);
        } catch (\Exception $e) {
            return response()->json(['erro' => $e->getMessage()], 500);
        }
    }
}
