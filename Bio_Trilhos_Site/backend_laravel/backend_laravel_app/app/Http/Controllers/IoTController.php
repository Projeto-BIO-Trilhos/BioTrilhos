<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class IoTController extends Controller
{
    public function store(Request $request)
    {
        $data=$request->validate(['dispositivoId'=>'required|string','temperatura'=>'nullable|numeric','umidadeAr'=>'nullable|numeric','umidadeSolo'=>'nullable|numeric','nivelAgua'=>'nullable|numeric','dataHora'=>'nullable|date']);
        $hora=$data['dataHora']??now();
        $map=[1=>$data['temperatura']??null,2=>$data['umidadeAr']??null,3=>$data['umidadeSolo']??null];
        foreach($map as $sensorId=>$valor){ if($valor!==null) DB::table('leituras_sensores')->insert(['sensor_id'=>$sensorId,'valor'=>$valor,'data_hora_leitura'=>$hora,'unidade_medida'=>$sensorId===1?'°C':'%','status'=>'OK']); }
        if(isset($data['nivelAgua'])) DB::table('reservatorios')->where('estufa_id',1)->update(['nivel_atual'=>$data['nivelAgua']*10,'percentual_atual'=>$data['nivelAgua'],'data_ultima_atualizacao'=>now()]);
        return response()->json(['mensagem'=>'Leitura registrada'],201);
    }
}
