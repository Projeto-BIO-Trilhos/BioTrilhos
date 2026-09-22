<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class IoTController extends Controller
{
    public function store(Request $request)
    {
        $data = $request->validate([
            'dispositivoId' => 'required|string',
            'temperatura' => 'nullable|numeric',
            'umidadeAr' => 'nullable|numeric',
            'umidadeSolo' => 'nullable|numeric',
            'nivelAgua' => 'nullable|numeric',
            'dataHora' => 'required|date'
        ]);

        // Mapear e inserir nas tabelas existentes (ajuste conforme seu schema)
        try {
            // Exemplo: salvar na tabela leituras_sensores (assumindo sensores já mapeados)
            DB::table('leituras_sensores')->insert([
                'sensor_id' => 1, // ajuste necessário na migração real
                'valor' => $data['temperatura'] ?? null,
                'data_hora_leitura' => $data['dataHora'],
                'unidade_medida' => '°C',
                'status' => 'OK'
            ]);

            return response()->json(['mensagem' => 'Leitura registrada'], 201);
        } catch (\Exception $e) {
            return response()->json(['erro' => $e->getMessage()], 500);
        }
    }
}
