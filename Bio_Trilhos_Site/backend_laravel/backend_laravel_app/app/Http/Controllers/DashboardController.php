<?php

namespace App\Http\Controllers;

use Illuminate\Support\Facades\DB;

class DashboardController extends Controller
{
    public function show($estufaId)
    {
        $temperatura = DB::table('leituras_sensores')->join('sensores','leituras_sensores.sensor_id','=','sensores.id')->where('sensores.estufa_id',$estufaId)->where('sensores.tipo_sensor',0)->orderByDesc('leituras_sensores.data_hora_leitura')->value('leituras_sensores.valor') ?? 24.8;
        $umidadeAr = DB::table('leituras_sensores')->join('sensores','leituras_sensores.sensor_id','=','sensores.id')->where('sensores.estufa_id',$estufaId)->where('sensores.tipo_sensor',1)->orderByDesc('leituras_sensores.data_hora_leitura')->value('leituras_sensores.valor') ?? 72;
        $umidadeSolo = DB::table('leituras_sensores')->join('sensores','leituras_sensores.sensor_id','=','sensores.id')->where('sensores.estufa_id',$estufaId)->where('sensores.tipo_sensor',2)->orderByDesc('leituras_sensores.data_hora_leitura')->value('leituras_sensores.valor') ?? 64;
        $reservatorio = DB::table('reservatorios')->where('estufa_id',$estufaId)->first();
        $irrigacao = DB::table('irrigacoes')->where('estufa_id',$estufaId)->orderByDesc('data_hora_inicio')->first();
        $meteo = DB::table('dados_meteorologicos')->where('estufa_id',$estufaId)->orderByDesc('data_hora_consulta')->first();

        return response()->json([
            'temperaturaAtual' => (float)$temperatura,
            'umidadeArAtual' => (float)$umidadeAr,
            'umidadeSoloAtual' => (float)$umidadeSolo,
            'reservatorioNivel' => (float)($reservatorio->nivel_atual ?? 820),
            'reservatorioPercentual' => (float)($reservatorio->percentual_atual ?? 82),
            'irrigacaoAtiva' => (bool)$irrigacao && $irrigacao->status === 0,
            'ultimaIrrigacao' => $irrigacao ? $irrigacao->data_hora_inicio : null,
            'temperaturaExterna' => (float)($meteo->temperatura_externa ?? 25.5),
            'probabilidadeChuva' => (float)($meteo->probabilidade_chuva ?? 25),
            'ultimaAtualizacao' => now()->toIso8601String(),
        ]);
    }
}
