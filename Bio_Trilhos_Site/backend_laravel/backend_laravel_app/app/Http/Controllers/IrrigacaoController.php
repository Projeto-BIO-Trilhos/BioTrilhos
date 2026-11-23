<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Support\Facades\DB;

class IrrigacaoController extends Controller
{
    public function start(Request $request)
    {
        $data=$request->validate(['estufaId'=>'required|integer','reservatorioId'=>'nullable|integer','motivo'=>'nullable|string','duracaoSegundos'=>'required|integer','tipoAcionamento'=>'required|integer']);
        $id=DB::table('irrigacoes')->insertGetId(['estufa_id'=>$data['estufaId'],'reservatorio_id'=>$data['reservatorioId']??null,'data_hora_inicio'=>now(),'duracao_segundos'=>$data['duracaoSegundos'],'motivo'=>$data['motivo']??'Manual','tipo_acionamento'=>$data['tipoAcionamento'],'status'=>0]);
        return response()->json(['id'=>$id],201);
    }

    public function stop(Request $request)
    {
        $id=(int)(json_decode($request->getContent(),true) ?? $request->input('id') ?? 0);
        $query=DB::table('irrigacoes')->where('status',0);
        if($id>0) $query->where('id',$id);
        $query->update(['data_hora_fim'=>now(),'status'=>1]);
        return response()->json(['mensagem'=>'Irrigação parada']);
    }
}
