<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class DashboardController extends Controller
{
    public function show($estufaId)
    {
        // Exemplo simples que consulta o banco diretamente usando query builder.
        // Em um projeto Laravel real, crie Models/Eloquent e use relacionamentos.

        $sensores = DB::table('sensores')->where('estufa_id', $estufaId)->get();
        $reservatorio = DB::table('reservatorios')->where('estufa_id', $estufaId)->first();
        $leituras = DB::table('leituras_sensores')
            ->join('sensores', 'leituras_sensores.sensor_id', '=', 'sensores.id')
            ->where('sensores.estufa_id', $estufaId)
            ->orderBy('data_hora_leitura', 'desc')
            ->limit(20)
            ->get();

        return response()->json([
            'sensores' => $sensores,
            'reservatorio' => $reservatorio,
            'leituras' => $leituras,
            'ultimaAtualizacao' => now()->toDateTimeString()
        ]);
    }
}
