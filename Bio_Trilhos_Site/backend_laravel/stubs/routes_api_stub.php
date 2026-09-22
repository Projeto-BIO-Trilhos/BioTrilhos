<?php

use Illuminate\Http\Request;
use Illuminate\Support\Facades\Route;

Route::middleware('api')->group(function () {
    Route::get('/dashboard/{estufaId}', [App\Http\Controllers\DashboardController::class, 'show']);
    Route::get('/estufas', [App\Http\Controllers\EstufaController::class, 'index']);
    Route::get('/sensores', [App\Http\Controllers\SensorController::class, 'index']);
    Route::post('/leituras', [App\Http\Controllers\LeituraController::class, 'store']);
    Route::post('/iot/leituras', [App\Http\Controllers\IoTController::class, 'store']);
    Route::post('/irrigacao/iniciar', [App\Http\Controllers\IrrigacaoController::class, 'start']);
    Route::post('/irrigacao/parar', [App\Http\Controllers\IrrigacaoController::class, 'stop']);
    Route::get('/meteorologia/atual/{estufaId}', [App\Http\Controllers\MeteorologiaController::class, 'current']);
});
