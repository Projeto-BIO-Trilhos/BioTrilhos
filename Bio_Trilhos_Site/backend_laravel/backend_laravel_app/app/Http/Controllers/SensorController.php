<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class SensorController extends Controller
{
    public function index(Request $request)
    {
        $query = DB::table('sensores');
        if ($request->filled('estufaId')) $query->where('estufa_id',$request->integer('estufaId'));
        return response()->json($query->get()->map(fn ($sensor) => [
            'id' => $sensor->id,
            'nome' => $sensor->nome,
            'tipoSensor' => (int)$sensor->tipo_sensor,
            'unidadeMedida' => $sensor->unidade_medida,
            'ativo' => (bool)$sensor->ativo,
        ]));
    }
}
