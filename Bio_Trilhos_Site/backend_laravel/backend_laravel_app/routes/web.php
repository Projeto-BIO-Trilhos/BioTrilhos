<?php

use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Route;

Route::get('/', function () {
    try {
        DB::connection()->getPdo();
        $database = 'Conectado';
    } catch (Throwable $e) {
        $database = 'Desconectado';
    }

    return response()->json([
        'nome' => 'Bio Trilhos API',
        'status' => 'online',
        'versao' => '1.0',
        'banco' => $database,
        'frontend' => 'http://127.0.0.1:8000/index.html',
        'endpoints' => [
            'dashboard' => '/api/dashboard/{estufaId}',
            'estufas' => '/api/estufas',
            'sensores' => '/api/sensores?estufaId=1',
            'clima' => '/api/meteorologia/atual/{estufaId}',
            'irrigacao' => '/api/irrigacao/iniciar',
        ],
    ]);
});
