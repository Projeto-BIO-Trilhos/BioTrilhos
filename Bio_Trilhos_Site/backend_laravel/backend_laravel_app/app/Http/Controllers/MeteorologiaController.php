<?php

namespace App\Http\Controllers;

use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Http;

class MeteorologiaController extends Controller
{
    public function current($estufaId)
    {
        try {
            $response=Http::timeout(5)->get('https://api.open-meteo.com/v1/forecast',['latitude'=>'-22.6597','longitude'=>'-44.9737','current'=>'temperature_2m,relative_humidity_2m,precipitation,wind_speed_10m','timezone'=>'America/Sao_Paulo']);
            $current=$response->ok() ? ($response->json('current') ?? []) : [];
            $temperatura=(float)($current['temperature_2m'] ?? 25.5);
            $umidade=(float)($current['relative_humidity_2m'] ?? 68);
            $vento=(float)($current['wind_speed_10m'] ?? 8);
            $chovendo=(float)($current['precipitation'] ?? 0)>0;
        } catch (\Throwable $e) {
            $temperatura=25.5;$umidade=68;$vento=8;$chovendo=false;
        }
        $probabilidade=(float)DB::table('dados_meteorologicos')->where('estufa_id',$estufaId)->orderByDesc('data_hora_consulta')->value('probabilidade_chuva') ?? 25;
        return response()->json(['temperaturaExterna'=>$temperatura,'umidadeExterna'=>$umidade,'velocidadeVento'=>$vento,'estaChovendo'=>$chovendo,'condicaoClimatica'=>$chovendo?'Chuvoso':'Parcialmente limpo','probabilidadeChuva'=>$probabilidade]);
    }
}
