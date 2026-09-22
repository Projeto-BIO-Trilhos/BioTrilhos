<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class IrrigacaoController extends Controller
{
    public function start(Request $request)
    {
        $data = $request->validate([
            'estufaId' => 'required|integer',
            'reservatorioId' => 'required|integer',
            'motivo' => 'nullable|string',
            'duracaoSegundos' => 'required|integer',
            'tipoAcionamento' => 'required|integer'
        ]);

        // Inserir registro simples na tabela irrigacoes
        $id = DB::table('irrigacoes')->insertGetId([
            'estufa_id' => $data['estufaId'],
            'reservatorio_id' => $data['reservatorioId'],
            'motivo' => $data['motivo'] ?? 'Manual',
            'duracao_segundos' => $data['duracaoSegundos'],
            'tipo_acionamento' => $data['tipoAcionamento'],
            'status' => 0, // Ativa
            'data_hora_inicio' => now()
        ]);

        return response()->json(['id' => $id], 201);
    }

    public function stop(Request $request)
    {
        $id = $request->getContent();
        // Lógica de parada deve atualizar registro: data_hora_fim, status
        DB::table('irrigacoes')->where('id', (int)$id)->update([
            'data_hora_fim' => now(),
            'status' => 1 // Finalizada
        ]);

        return response()->json(['mensagem' => 'Irrigação parada']);
    }
}
