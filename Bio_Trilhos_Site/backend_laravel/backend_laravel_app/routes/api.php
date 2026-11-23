<?php

use Illuminate\Support\Facades\Route;
use App\Http\Controllers\DashboardController;
use App\Http\Controllers\EstufaController;
use App\Http\Controllers\SensorController;
use App\Http\Controllers\LeituraController;
use App\Http\Controllers\IoTController;
use App\Http\Controllers\IrrigacaoController;
use App\Http\Controllers\MeteorologiaController;

Route::get('/dashboard/{estufaId}', [DashboardController::class, 'show']);
Route::get('/estufas', [EstufaController::class, 'index']);
Route::get('/sensores', [SensorController::class, 'index']);
Route::post('/leituras', [LeituraController::class, 'store']);
Route::post('/iot/leituras', [IoTController::class, 'store']);
Route::post('/irrigacao/iniciar', [IrrigacaoController::class, 'start']);
Route::post('/irrigacao/parar', [IrrigacaoController::class, 'stop']);
Route::get('/meteorologia/atual/{estufaId}', [MeteorologiaController::class, 'current']);
