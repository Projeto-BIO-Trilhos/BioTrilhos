-- ============================================================================
-- BIO TRILHOS - Sistema de Monitoramento e Automação de Estufa Inteligente
-- ============================================================================
-- Database Script para PostgreSQL
-- Última atualização: 2026-09-08
-- ============================================================================

-- Criar banco de dados
CREATE DATABASE biotrilhos
    WITH
    ENCODING = 'UTF8'
    LC_COLLATE = 'pt_BR.UTF-8'
    LC_CTYPE = 'pt_BR.UTF-8'
    TEMPLATE = template0;

-- Conectar ao banco
\c biotrilhos;

-- ============================================================================
-- TABELAS
-- ============================================================================

-- Tabela de Estufas
CREATE TABLE estufas (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    localizacao VARCHAR(255) NOT NULL,
    descricao VARCHAR(1000),
    ativa BOOLEAN NOT NULL DEFAULT true,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_estufas_ativa ON estufas(ativa);

-- Tabela de Sensores
CREATE TABLE sensores (
    id SERIAL PRIMARY KEY,
    estufa_id INTEGER NOT NULL,
    nome VARCHAR(255) NOT NULL,
    tipo_sensor INTEGER NOT NULL,
    localizacao VARCHAR(255),
    identificador_dispositivo VARCHAR(100) NOT NULL UNIQUE,
    unidade_medida VARCHAR(50),
    ativo BOOLEAN NOT NULL DEFAULT true,
    data_instalacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (estufa_id) REFERENCES estufas(id) ON DELETE CASCADE
);

CREATE INDEX idx_sensores_estufa ON sensores(estufa_id);
CREATE INDEX idx_sensores_dispositivo ON sensores(identificador_dispositivo);

-- Tabela de Leituras de Sensores
CREATE TABLE leituras_sensores (
    id SERIAL PRIMARY KEY,
    sensor_id INTEGER NOT NULL,
    valor NUMERIC(10, 2) NOT NULL,
    data_hora_leitura TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    unidade_medida VARCHAR(50),
    status VARCHAR(50) DEFAULT 'OK',
    FOREIGN KEY (sensor_id) REFERENCES sensores(id) ON DELETE CASCADE
);

-- Índices críticos para performance de consultas
CREATE INDEX idx_leituras_sensor ON leituras_sensores(sensor_id);
CREATE INDEX idx_leituras_data ON leituras_sensores(data_hora_leitura);
CREATE INDEX idx_leituras_sensor_data ON leituras_sensores(sensor_id, data_hora_leitura);

-- Tabela de Reservatórios
CREATE TABLE reservatorios (
    id SERIAL PRIMARY KEY,
    estufa_id INTEGER NOT NULL,
    nome VARCHAR(255) NOT NULL,
    capacidade_maxima NUMERIC(12, 2) NOT NULL,
    nivel_atual NUMERIC(12, 2) NOT NULL,
    percentual_atual NUMERIC(5, 2) NOT NULL,
    status VARCHAR(50) DEFAULT 'Normal',
    data_ultima_atualizacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (estufa_id) REFERENCES estufas(id) ON DELETE CASCADE
);

CREATE INDEX idx_reservatorios_estufa ON reservatorios(estufa_id);

-- Tabela de Histórico de Reservatórios
CREATE TABLE historicos_reservatorios (
    id SERIAL PRIMARY KEY,
    reservatorio_id INTEGER NOT NULL,
    nivel_anterior NUMERIC(12, 2) NOT NULL,
    nivel_atual NUMERIC(12, 2) NOT NULL,
    tipo_movimentacao INTEGER NOT NULL,
    descricao VARCHAR(1000),
    data_hora TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (reservatorio_id) REFERENCES reservatorios(id) ON DELETE CASCADE
);

CREATE INDEX idx_historicos_reservatorio ON historicos_reservatorios(reservatorio_id);
CREATE INDEX idx_historicos_data ON historicos_reservatorios(data_hora);

-- Tabela de Irrigação
CREATE TABLE irrigacoes (
    id SERIAL PRIMARY KEY,
    estufa_id INTEGER NOT NULL,
    reservatorio_id INTEGER,
    data_hora_inicio TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_hora_fim TIMESTAMP,
    duracao_segundos INTEGER,
    motivo VARCHAR(500),
    tipo_acionamento INTEGER NOT NULL,
    status INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (estufa_id) REFERENCES estufas(id) ON DELETE CASCADE,
    FOREIGN KEY (reservatorio_id) REFERENCES reservatorios(id) ON DELETE SET NULL
);

CREATE INDEX idx_irrigacoes_estufa ON irrigacoes(estufa_id);
CREATE INDEX idx_irrigacoes_data ON irrigacoes(data_hora_inicio);

-- Tabela de Configurações Ambientais
CREATE TABLE configuracoes_ambientais (
    id SERIAL PRIMARY KEY,
    estufa_id INTEGER NOT NULL UNIQUE,
    umidade_solo_minima NUMERIC(5, 2) NOT NULL DEFAULT 50.00,
    umidade_solo_maxima NUMERIC(5, 2) NOT NULL DEFAULT 75.00,
    temperatura_minima NUMERIC(5, 2) NOT NULL DEFAULT 20.00,
    temperatura_maxima NUMERIC(5, 2) NOT NULL DEFAULT 28.00,
    umidade_ar_minima NUMERIC(5, 2) NOT NULL DEFAULT 60.00,
    umidade_ar_maxima NUMERIC(5, 2) NOT NULL DEFAULT 80.00,
    tempo_maximo_irrigacao_segundos INTEGER NOT NULL DEFAULT 300,
    nivel_minimo_reservatorio NUMERIC(5, 2) NOT NULL DEFAULT 20.00,
    atualizado_em TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (estufa_id) REFERENCES estufas(id) ON DELETE CASCADE
);

CREATE INDEX idx_configs_estufa ON configuracoes_ambientais(estufa_id);

-- Tabela de Dados Meteorológicos
CREATE TABLE dados_meteorologicos (
    id SERIAL PRIMARY KEY,
    estufa_id INTEGER NOT NULL,
    temperatura_externa NUMERIC(5, 2),
    umidade_externa NUMERIC(5, 2),
    condicao_climatica VARCHAR(255),
    esta_chovendo BOOLEAN NOT NULL DEFAULT false,
    probabilidade_chuva NUMERIC(5, 2),
    previsao_chuva VARCHAR(500),
    velocidade_vento NUMERIC(5, 2),
    data_hora_consulta TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fonte_api VARCHAR(100) NOT NULL DEFAULT 'Open-Meteo',
    FOREIGN KEY (estufa_id) REFERENCES estufas(id) ON DELETE CASCADE
);

CREATE INDEX idx_meteorologia_estufa ON dados_meteorologicos(estufa_id);
CREATE INDEX idx_meteorologia_data ON dados_meteorologicos(data_hora_consulta);

-- Tabela de Alertas
CREATE TABLE alertas (
    id SERIAL PRIMARY KEY,
    estufa_id INTEGER NOT NULL,
    tipo VARCHAR(100) NOT NULL,
    mensagem VARCHAR(1000) NOT NULL,
    nivel INTEGER NOT NULL DEFAULT 1,
    resolvido BOOLEAN NOT NULL DEFAULT false,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    data_resolucao TIMESTAMP,
    FOREIGN KEY (estufa_id) REFERENCES estufas(id) ON DELETE CASCADE
);

CREATE INDEX idx_alertas_estufa ON alertas(estufa_id);
CREATE INDEX idx_alertas_data ON alertas(data_criacao);
CREATE INDEX idx_alertas_resolvido ON alertas(resolvido);

-- Tabela de Usuários
CREATE TABLE usuarios (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    senha_hash VARCHAR(255) NOT NULL,
    tipo_usuario INTEGER NOT NULL DEFAULT 1,
    ativo BOOLEAN NOT NULL DEFAULT true,
    data_criacao TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_usuarios_email ON usuarios(email);

-- ============================================================================
-- DADOS INICIAIS
-- ============================================================================

-- Inserir estufa padrão
INSERT INTO estufas (nome, localizacao, descricao, ativa)
VALUES (
    'Bio Trilhos - Estufa Principal',
    'Cruzeiro - São Paulo - Brasil',
    'Estufa tecnológica integrada à linha ferroviária revitalizada para educação ambiental e turismo sustentável.',
    true
);

-- Inserir sensores padrão
INSERT INTO sensores (estufa_id, nome, tipo_sensor, localizacao, identificador_dispositivo, unidade_medida)
VALUES
    (1, 'Sensor de Temperatura', 0, 'Centro da estufa', 'TEMP_001', '°C'),
    (1, 'Sensor de Umidade do Ar', 1, 'Centro da estufa', 'UMID_AR_001', '%'),
    (1, 'Sensor de Umidade do Solo', 2, 'Canteiro principal', 'UMID_SOLO_001', '%'),
    (1, 'Sensor de Nível de Água', 5, 'Reservatório', 'NIVEL_001', '%'),
    (1, 'Sensor de Chuva', 4, 'Topo da estufa', 'CHUVA_001', 'bool');

-- Inserir reservatório
INSERT INTO reservatorios (estufa_id, nome, capacidade_maxima, nivel_atual, percentual_atual, status)
VALUES (1, 'Reservatório Principal', 1000.00, 820.00, 82.00, 'Normal');

-- Inserir configurações ambientais
INSERT INTO configuracoes_ambientais (
    estufa_id,
    umidade_solo_minima,
    umidade_solo_maxima,
    temperatura_minima,
    temperatura_maxima,
    umidade_ar_minima,
    umidade_ar_maxima,
    tempo_maximo_irrigacao_segundos,
    nivel_minimo_reservatorio
) VALUES (1, 50.00, 75.00, 20.00, 28.00, 60.00, 80.00, 300, 20.00);

-- Inserir algumas leituras de exemplo
INSERT INTO leituras_sensores (sensor_id, valor, unidade_medida, status)
VALUES
    (1, 24.8, '°C', 'OK'),
    (1, 25.2, '°C', 'OK'),
    (2, 72.0, '%', 'OK'),
    (2, 70.0, '%', 'OK'),
    (3, 64.0, '%', 'OK'),
    (3, 62.0, '%', 'OK');

-- ============================================================================
-- VIEWS (Opcional - para facilitar consultas)
-- ============================================================================

-- View: Última leitura de cada sensor
CREATE OR REPLACE VIEW vw_ultimas_leituras AS
SELECT
    s.id,
    s.estufa_id,
    s.nome AS sensor_nome,
    s.tipo_sensor,
    l.valor,
    l.unidade_medida,
    l.data_hora_leitura,
    l.status
FROM sensores s
LEFT JOIN LATERAL (
    SELECT *
    FROM leituras_sensores
    WHERE sensor_id = s.id
    ORDER BY data_hora_leitura DESC
    LIMIT 1
) l ON true
WHERE s.ativo = true;

-- View: Status de irrigação
CREATE OR REPLACE VIEW vw_status_irrigacao AS
SELECT
    e.id,
    e.nome,
    COUNT(CASE WHEN i.status = 0 THEN 1 END) AS irrigacoes_ativas,
    COUNT(CASE WHEN i.status = 1 THEN 1 END) AS irrigacoes_finalizadas,
    MAX(i.data_hora_fim) AS ultima_irrigacao
FROM estufas e
LEFT JOIN irrigacoes i ON e.id = i.estufa_id
GROUP BY e.id, e.nome;

-- ============================================================================
-- FIM DO SCRIPT
-- ============================================================================
