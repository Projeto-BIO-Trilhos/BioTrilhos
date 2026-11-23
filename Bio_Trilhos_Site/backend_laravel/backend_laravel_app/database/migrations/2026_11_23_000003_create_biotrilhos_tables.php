<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\DB;
use Illuminate\Support\Facades\Schema;

return new class extends Migration
{
    public function up(): void
    {
        if (!Schema::hasTable('estufas')) {
            Schema::create('estufas', function (Blueprint $table) {
                $table->id();
                $table->string('nome');
                $table->string('localizacao')->nullable();
                $table->text('descricao')->nullable();
                $table->boolean('ativa')->default(true);
                $table->timestamp('data_criacao')->useCurrent();
            });
        }

        if (!Schema::hasTable('sensores')) {
            Schema::create('sensores', function (Blueprint $table) {
                $table->id();
                $table->foreignId('estufa_id')->constrained('estufas')->cascadeOnDelete();
                $table->string('nome');
                $table->integer('tipo_sensor');
                $table->string('localizacao')->nullable();
                $table->string('identificador_dispositivo')->nullable()->unique();
                $table->string('unidade_medida')->nullable();
                $table->boolean('ativo')->default(true);
                $table->timestamp('data_instalacao')->useCurrent();
            });
        }

        if (!Schema::hasTable('leituras_sensores')) {
            Schema::create('leituras_sensores', function (Blueprint $table) {
                $table->id();
                $table->foreignId('sensor_id')->constrained('sensores')->cascadeOnDelete();
                $table->decimal('valor', 10, 2);
                $table->timestamp('data_hora_leitura')->useCurrent();
                $table->string('unidade_medida')->nullable();
                $table->string('status')->default('OK');
            });
        }

        if (!Schema::hasTable('reservatorios')) {
            Schema::create('reservatorios', function (Blueprint $table) {
                $table->id();
                $table->foreignId('estufa_id')->constrained('estufas')->cascadeOnDelete();
                $table->string('nome');
                $table->decimal('capacidade_maxima', 12, 2);
                $table->decimal('nivel_atual', 12, 2);
                $table->decimal('percentual_atual', 5, 2);
                $table->string('status')->default('Normal');
                $table->timestamp('data_ultima_atualizacao')->useCurrent();
            });
        }

        if (!Schema::hasTable('irrigacoes')) {
            Schema::create('irrigacoes', function (Blueprint $table) {
                $table->id();
                $table->foreignId('estufa_id')->constrained('estufas')->cascadeOnDelete();
                $table->foreignId('reservatorio_id')->nullable()->constrained('reservatorios')->nullOnDelete();
                $table->timestamp('data_hora_inicio')->useCurrent();
                $table->timestamp('data_hora_fim')->nullable();
                $table->integer('duracao_segundos')->nullable();
                $table->string('motivo')->nullable();
                $table->integer('tipo_acionamento');
                $table->integer('status')->default(0);
            });
        }

        if (!Schema::hasTable('dados_meteorologicos')) {
            Schema::create('dados_meteorologicos', function (Blueprint $table) {
                $table->id();
                $table->foreignId('estufa_id')->constrained('estufas')->cascadeOnDelete();
                $table->decimal('temperatura_externa', 5, 2)->nullable();
                $table->decimal('umidade_externa', 5, 2)->nullable();
                $table->string('condicao_climatica')->nullable();
                $table->boolean('esta_chovendo')->default(false);
                $table->decimal('probabilidade_chuva', 5, 2)->nullable();
                $table->string('previsao_chuva')->nullable();
                $table->decimal('velocidade_vento', 5, 2)->nullable();
                $table->timestamp('data_hora_consulta')->useCurrent();
                $table->string('fonte_api')->default('Open-Meteo');
            });
        }

        if (!DB::table('estufas')->where('id', 1)->exists()) {
            DB::table('estufas')->insert(['id' => 1, 'nome' => 'Bio Trilhos - Estufa Principal', 'localizacao' => 'Cruzeiro - São Paulo - Brasil', 'descricao' => 'Estufa tecnológica integrada à linha ferroviária revitalizada.', 'ativa' => true]);
        }

        if (!DB::table('sensores')->where('estufa_id', 1)->exists()) {
            DB::table('sensores')->insert([
                ['estufa_id'=>1,'nome'=>'Sensor de Temperatura','tipo_sensor'=>0,'localizacao'=>'Centro da estufa','identificador_dispositivo'=>'TEMP_001','unidade_medida'=>'°C','ativo'=>true],
                ['estufa_id'=>1,'nome'=>'Sensor de Umidade do Ar','tipo_sensor'=>1,'localizacao'=>'Centro da estufa','identificador_dispositivo'=>'UMID_AR_001','unidade_medida'=>'%','ativo'=>true],
                ['estufa_id'=>1,'nome'=>'Sensor de Umidade do Solo','tipo_sensor'=>2,'localizacao'=>'Canteiro principal','identificador_dispositivo'=>'UMID_SOLO_001','unidade_medida'=>'%','ativo'=>true],
                ['estufa_id'=>1,'nome'=>'Sensor de Nível de Água','tipo_sensor'=>5,'localizacao'=>'Reservatório','identificador_dispositivo'=>'NIVEL_001','unidade_medida'=>'%','ativo'=>true],
                ['estufa_id'=>1,'nome'=>'Sensor de Chuva','tipo_sensor'=>4,'localizacao'=>'Topo da estufa','identificador_dispositivo'=>'CHUVA_001','unidade_medida'=>'bool','ativo'=>true],
            ]);
        }

        if (!DB::table('reservatorios')->where('estufa_id', 1)->exists()) {
            DB::table('reservatorios')->insert(['estufa_id'=>1,'nome'=>'Reservatório Principal','capacidade_maxima'=>1000,'nivel_atual'=>820,'percentual_atual'=>82,'status'=>'Normal']);
        }

        if (!DB::table('leituras_sensores')->exists()) {
            DB::table('leituras_sensores')->insert([
                ['sensor_id'=>1,'valor'=>24.8,'unidade_medida'=>'°C','status'=>'OK'],
                ['sensor_id'=>2,'valor'=>72,'unidade_medida'=>'%','status'=>'OK'],
                ['sensor_id'=>3,'valor'=>64,'unidade_medida'=>'%','status'=>'OK'],
            ]);
        }

        if (!DB::table('dados_meteorologicos')->where('estufa_id', 1)->exists()) {
            DB::table('dados_meteorologicos')->insert(['estufa_id'=>1,'temperatura_externa'=>25.5,'umidade_externa'=>68,'condicao_climatica'=>'Parcialmente nublado','esta_chovendo'=>false,'probabilidade_chuva'=>25,'velocidade_vento'=>8,'fonte_api'=>'Open-Meteo']);
        }
    }

    public function down(): void
    {
        Schema::dropIfExists('dados_meteorologicos');
        Schema::dropIfExists('irrigacoes');
        Schema::dropIfExists('reservatorios');
        Schema::dropIfExists('leituras_sensores');
        Schema::dropIfExists('sensores');
        Schema::dropIfExists('estufas');
    }
};