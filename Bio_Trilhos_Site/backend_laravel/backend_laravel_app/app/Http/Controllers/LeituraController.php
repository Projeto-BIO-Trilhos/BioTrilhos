<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class LeituraController extends Controller
{
    public function store(Request $request)
    {
        $data=$request->validate(['sensorId'=>'required|integer','valor'=>'required|numeric','unidadeMedida'=>'nullable|string','status'=>'nullable|string']);
        $id=DB::table('leituras_sensores')->insertGetId(['sensor_id'=>$data['sensorId'],'valor'=>$data['valor'],'unidade_medida'=>$data['unidadeMedida']??null,'status'=>$data['status']??'OK','data_hora_leitura'=>now()]);
        return response()->json(['id'=>$id],201);
    }
}
