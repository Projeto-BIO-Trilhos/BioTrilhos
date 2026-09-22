# Bio Trilhos - Sistema de Monitoramento e Automação de Estufa Inteligente

## 📋 Visão Geral

**Bio Trilhos** é um sistema completo de monitoramento e automação para uma estufa tecnológica integrada à linha ferroviária de Cruzeiro - São Paulo. O projeto une tecnologia IoT, bancos de dados, API externa de meteorologia e automação inteligente para criar um ambiente sustentável de educação ambiental e turismo.

### Objetivos
- Monitorar em tempo real as condições ambientais da estufa
- Automatizar o sistema de irrigação baseado em sensores e meteorologia
- Registrar histórico completo de operações
- Integrar com Arduino e dispositivos IoT
- Fornecer dashboard intuitivo para visualização de dados

---

## 🏗️ Arquitetura do Projeto

### Estrutura de Pastas

```
Bio_Trilhos_Site/
├── frontend/                   # Interface Web (HTML/CSS/JS)
│   ├── index.html             # Dashboard principal
│   ├── script.js              # Lógica do front-end
│   └── style.css              # Estilos
│
└── backend/                   # API ASP.NET Core
    ├── Models/                # Modelos de domínio
    │   ├── Estufa.cs
    │   ├── Sensor.cs
    │   ├── LeituraSensor.cs
    │   ├── Reservatorio.cs
    │   ├── HistoricoReservatorio.cs
    │   ├── Irrigacao.cs
    │   ├── ConfiguracoesAmbientais.cs
    │   ├── DadosMeteorologicos.cs
    │   ├── Alerta.cs
    │   └── Usuario.cs
    │
    ├── DTOs/                  # Data Transfer Objects
    │   ├── EstufaDto.cs
    │   ├── SensorDto.cs
    │   ├── LeituraDto.cs
    │   ├── ReservatorioDto.cs
    │   ├── IrrigacaoDto.cs
    │   ├── MeteorologiaDto.cs
    │   ├── DashboardDto.cs
    │   ├── IoTDto.cs
    │   └── ConfiguracoesDto.cs
    │
    ├── Controllers/           # API REST Endpoints
    │   ├── EstufasController.cs
    │   ├── SensoresController.cs
    │   ├── LeiturasController.cs
    │   ├── IrrigacaoController.cs
    │   ├── ReservatoriosController.cs
    │   ├── MeteorologiaController.cs
    │   ├── ConfiguracoesController.cs
    │   ├── IoTController.cs
    │   └── DashboardController.cs
    │
    ├── Services/              # Lógica de Negócio
    │   ├── WeatherService.cs  # Integração com API meteorológica
    │   └── AutomationService.cs # Lógica de automação
    │
    ├── Data/                  # Acesso a Dados
    │   └── BioTrilhosDbContext.cs # Entity Framework Core DbContext
    │
    ├── Migrations/            # Entity Framework Migrations
    │
    ├── Program.cs             # Configuração da aplicação
    ├── appsettings.json       # Configurações
    ├── appsettings.Development.json
    ├── BioTrilhos.API.csproj  # Arquivo de projeto
    ├── biotrilhos.sql         # Script SQL completo
    └── README.md              # Este arquivo
```

---

## 🗄️ Banco de Dados

### Tecnologia
- **SGBD**: PostgreSQL
- **ORM**: Entity Framework Core 8.0
- **Driver**: Npgsql

### Entidades Principais

#### 1. **Estufas**
Armazena informações sobre cada estufa no sistema.
```sql
- Id (PK)
- Nome
- Localizacao
- Descricao
- Ativa
- DataCriacao
```

#### 2. **Sensores**
Registra os sensores instalados e conectados.
```sql
- Id (PK)
- EstufaId (FK)
- Nome
- TipoSensor (0=Temp, 1=UmidadeAr, 2=UmidadeSolo, 3=Lum, 4=Chuva, 5=NívelÁgua)
- Localizacao
- IdentificadorDispositivo (UNIQUE)
- UnidadeMedida
- Ativo
- DataInstalacao
```

#### 3. **LeiturasSensores**
Histórico de leituras de todos os sensores.
```sql
- Id (PK)
- SensorId (FK) [Índice]
- Valor
- DataHoraLeitura [Índice]
- UnidadeMedida
- Status
```
**Índices**: sensor_id, data_hora_leitura, (sensor_id, data_hora_leitura)

#### 4. **Reservatorios**
Registra informações dos reservatórios de água.
```sql
- Id (PK)
- EstufaId (FK)
- Nome
- CapacidadeMaxima
- NivelAtual
- PercentualAtual
- Status
- DataUltimaAtualizacao
```

#### 5. **HistoricosReservatorios**
Rastreia alterações de nível dos reservatórios.
```sql
- Id (PK)
- ReservatorioId (FK)
- NivelAnterior
- NivelAtual
- TipoMovimentacao (0=Chuva, 1=Consumo, 2=Manual, 3=Outro)
- Descricao
- DataHora
```

#### 6. **Irrigacoes**
Registra cada acionamento do sistema de irrigação.
```sql
- Id (PK)
- EstufaId (FK)
- ReservatorioId (FK)
- DataHoraInicio
- DataHoraFim
- DuracaoSegundos
- Motivo
- TipoAcionamento (0=Automático, 1=Manual)
- Status (0=Ativa, 1=Finalizada, 2=Cancelada, 3=Erro)
```

#### 7. **ConfiguracoesAmbientais**
Define os limites ideais para automação.
```sql
- Id (PK)
- EstufaId (FK) [UNIQUE]
- UmidadeSoloMinima (default: 50%)
- UmidadeSoloMaxima (default: 75%)
- TemperaturaMinima (default: 20°C)
- TemperaturaMaxima (default: 28°C)
- UmidadeArMinima (default: 60%)
- UmidadeArMaxima (default: 80%)
- TempoMaximoIrrigacaoSegundos (default: 300)
- NivelMinimoReservatorio (default: 20%)
- AtualizadoEm
```

#### 8. **DadosMeteorologicos**
Armazena dados da API externa.
```sql
- Id (PK)
- EstufaId (FK)
- TemperaturaExterna
- UmidadeExterna
- CondicaoClimatica
- EstaChovendo
- ProbabilidadeChuva
- PrevisaoChuva
- VelocidadeVento
- DataHoraConsulta
- FonteAPI (default: "Open-Meteo")
```

#### 9. **Alertas**
Sistema de notificações de eventos importantes.
```sql
- Id (PK)
- EstufaId (FK)
- Tipo
- Mensagem
- Nivel (0=Info, 1=Aviso, 2=Crítico)
- Resolvido
- DataCriacao
- DataResolucao
```

#### 10. **Usuarios**
Preparação para futura autenticação.
```sql
- Id (PK)
- Nome
- Email [UNIQUE]
- SenhaHash (nunca armazene em texto puro)
- TipoUsuario (0=Admin, 1=Operador, 2=Visualização)
- Ativo
- DataCriacao
```

---

## 🌐 API REST - Endpoints

### Base URL
```
http://localhost:5000/api
```

### Autenticação
Atualmente a API é aberta. Em produção, implementar JWT ou OAuth2.

### Documentação Interativa
Acesse Swagger em: `http://localhost:5000`

---

### 1️⃣ **Estufas**

#### GET /estufas
Lista todas as estufas.
```json
Response: [
  {
    "id": 1,
    "nome": "Bio Trilhos - Estufa Principal",
    "localizacao": "Cruzeiro - São Paulo - Brasil",
    "descricao": "...",
    "ativa": true,
    "dataCriacao": "2026-09-08T14:00:00Z"
  }
]
```

#### GET /estufas/{id}
Obtém uma estufa específica.

#### POST /estufas
Cria uma nova estufa.
```json
Body: {
  "nome": "Nova Estufa",
  "localizacao": "Cruzeiro, SP",
  "descricao": "Descrição",
  "ativa": true
}
```

#### PUT /estufas/{id}
Atualiza uma estufa.

#### DELETE /estufas/{id}
Deleta uma estufa (cascata: sensores, leituras, etc.).

---

### 2️⃣ **Sensores**

#### GET /sensores?estufaId=1
Lista sensores de uma estufa.

#### GET /sensores/{id}
Obtém um sensor específico.

#### POST /sensores
Cria um novo sensor.
```json
Body: {
  "estufaId": 1,
  "nome": "Novo Sensor",
  "tipoSensor": 0,  // 0=Temp, 1=UmidadeAr, 2=UmidadeSolo, etc.
  "localizacao": "Centro",
  "identificadorDispositivo": "SENSOR_001",
  "unidadeMedida": "°C"
}
```

#### GET /sensores/{id}/leituras?dias=7
Obtém leituras do sensor dos últimos 7 dias.

---

### 3️⃣ **Leituras**

#### POST /leituras
Registra uma nova leitura de sensor.
```json
Body: {
  "sensorId": 1,
  "valor": 24.8,
  "unidadeMedida": "°C",
  "status": "OK"
}
```

#### GET /leituras/historico/{sensorId}?dataInicio=&dataFim=
Obtém histórico filtrado de leituras.

#### GET /leituras/ultimas/{estufaId}
Obtém as últimas leituras de todos os sensores da estufa.

---

### 4️⃣ **Dashboard**

#### GET /dashboard/{estufaId}
Retorna dados consolidados do dashboard.
```json
Response: {
  "estufaId": 1,
  "estufaNome": "Bio Trilhos - Estufa Principal",
  "temperaturaAtual": 24.8,
  "umidadeArAtual": 72,
  "umidadeSoloAtual": 64,
  "ultimaAtualizacaoSensores": "2026-09-08T14:30:00Z",
  "reservatorioNivel": 820,
  "reservatorioCapacidade": 1000,
  "reservatorioPercentual": 82,
  "reservatorioStatus": "Normal",
  "irrigacaoAtiva": false,
  "ultimaIrrigacao": "2026-09-08T09:42:00Z",
  "duracacaoUltimaIrrigacao": 240,
  "temperaturaExterna": 23,
  "umidadeExterna": 72,
  "condicaoClimatica": "Parcialmente nublado",
  "estaChovendo": false,
  "probabilidadeChuva": 35,
  "ultimaAtualizacaoClima": "2026-09-08T14:00:00Z",
  "alertasAtivos": 0,
  "alertasRecentes": [],
  "sistemaOperacional": true,
  "ultimaAtualizacao": "2026-09-08T14:30:00Z"
}
```

---

### 5️⃣ **Irrigação**

#### GET /irrigacao?estufaId=1
Lista todas as irrigações de uma estufa.

#### POST /irrigacao/iniciar
Inicia uma irrigação manual.
```json
Body: {
  "estufaId": 1,
  "reservatorioId": 1,
  "motivo": "Acionamento manual",
  "duracaoSegundos": 300,
  "tipoAcionamento": 1  // 0=Auto, 1=Manual
}
```

#### POST /irrigacao/parar
Para a irrigação ativa.
```json
Body: 1  // estufaId
```

#### GET /irrigacao/ativa/{estufaId}
Verifica se há irrigação ativa.

---

### 6️⃣ **Reservatórios**

#### GET /reservatorios?estufaId=1
Lista reservatórios de uma estufa.

#### GET /reservatorios/{id}
Obtém informações de um reservatório.

#### POST /reservatorios
Cria um novo reservatório.
```json
Body: {
  "estufaId": 1,
  "nome": "Reservatório Principal",
  "capacidadeMaxima": 1000,
  "nivelAtual": 820
}
```

#### PUT /reservatorios/{id}
Atualiza o nível (atualização manual).
```json
Body: {
  "estufaId": 1,
  "nome": "Reservatório Principal",
  "capacidadeMaxima": 1000,
  "nivelAtual": 750  // Novo nível
}
```

#### GET /reservatorios/{id}/historico?dias=30
Obtém histórico de alterações.

---

### 7️⃣ **Meteorologia**

#### GET /meteorologia/atual/{estufaId}
Obtém dados meteorológicos atuais (consulta API Open-Meteo).
```json
Response: {
  "id": 1,
  "estufaId": 1,
  "temperaturaExterna": 23,
  "umidadeExterna": 72,
  "condicaoClimatica": "Parcialmente nublado",
  "estaChovendo": false,
  "probabilidadeChuva": 35,
  "velocidadeVento": 12.5,
  "dataHoraConsulta": "2026-09-08T14:00:00Z",
  "fonteAPI": "Open-Meteo"
}
```

#### GET /meteorologia/previsao/{estufaId}?dias=7
Obtém histórico de dados meteorológicos.

#### GET /meteorologia/historico/{estufaId}?dataInicio=&dataFim=
Obtém histórico filtrado.

---

### 8️⃣ **IoT/Arduino**

#### POST /iot/leituras
Recebe leituras do Arduino.
```json
Body: {
  "dispositivoId": "ARDUINO_ESTUFA_01",
  "temperatura": 26.5,
  "umidadeAr": 72,
  "umidadeSolo": 48,
  "luminosidade": 650,
  "nivelAgua": 80,
  "chuva": false,
  "dataHora": "2026-09-08T14:00:00"
}
```

#### GET /iot/comandos/{dispositivoId}
Retorna comandos pendentes para o Arduino executar.
```json
Response: {
  "dispositivoId": "ARDUINO_ESTUFA_01",
  "tempoConsulta": "2026-09-08T14:30:00Z",
  "comandos": [
    {
      "tipo": "LIGAR_IRRIGACAO",
      "duracao": 300,
      "prioridade": "alta"
    }
  ]
}
```

---

### 9️⃣ **Configurações**

#### GET /configuracoes/{estufaId}
Obtém as configurações ambientais.

#### PUT /configuracoes/{estufaId}
Atualiza as configurações.
```json
Body: {
  "umidadeSoloMinima": 50,
  "umidadeSoloMaxima": 75,
  "temperaturaMinnima": 20,
  "temperaturaMaxima": 28,
  "umidadeArMinima": 60,
  "umidadeArMaxima": 80,
  "tempoMaximoIrrigacaoSegundos": 300,
  "nivelMinimoReservatorio": 20
}
```

---

## 🤖 Lógica de Automação

### AutomationService

O serviço de automação verifica continuamente as condições e toma decisões automáticas.

#### Lógica de Irrigação

```
SE:
  ✓ Umidade do solo < UmidadeSoloMinima
  ✓ Nível do reservatório > NivelMinimoReservatorio
  ✓ NÃO está chovendo
  ✓ NÃO há irrigação ativa

ENTÃO:
  → Iniciar irrigação automática
  → Registrar motivo: "Automático - Umidade do solo em X%"
  → Duração: TempoMaximoIrrigacaoSegundos
```

#### Lógica de Parada

Quando duracao é atingida:
1. Marcar irrigação como finalizada
2. Calcular consumo estimado (5L por minuto)
3. Atualizar nível do reservatório
4. Registrar consumo no histórico

#### Geração de Alertas

```
- Temperatura muito baixa / alta
- Umidade do solo baixa
- Nível do reservatório baixo
- Falha de sensor (status ≠ OK)
- Falha na API meteorológica
```

Alertas não duplicados: um alerta não resolvido do mesmo tipo não gera novo alerta.

---

## 🌤️ Integração Meteorológica

### API: Open-Meteo

**Vantagens:**
- ✅ Sem necessidade de chave de autenticação
- ✅ Completamente gratuita
- ✅ Sem limite de requisições
- ✅ Dados em tempo real e previsões
- ✅ Cobertura global

### Implementação

**Serviço**: `WeatherService.cs`

**Localização**: Cruzeiro - São Paulo - Brasil
- Latitude: -22.6597
- Longitude: -44.9737

**Endpoints:**
- Tempo atual: `/v1/forecast`
- Parâmetros: `temperature_2m`, `relative_humidity_2m`, `weather_code`, `rain`, `wind_speed_10m`

### Configuração

Adicione a URL e coordenadas em `appsettings.json`:

```json
"WeatherApi": {
  "BaseUrl": "https://api.open-meteo.com/v1",
  "Latitude": "-22.6597",
  "Longitude": "-44.9737",
  "TimeoutSeconds": 30
}
```

### Tratamento de Erros

Se a API cair:
- ✓ Retorna dados em cache da última consulta
- ✓ Gera alerta "Falha na API meteorológica"
- ✓ Continua automação usando dados anteriores

---

## 🚀 Como Executar o Projeto

### Pré-requisitos

- .NET 8.0 SDK
- PostgreSQL 13+
- Node.js (opcional, para ferramentas frontend)

### 1️⃣ Instalar PostgreSQL

**Windows:**
```bash
# Baixar PostgreSQL de: https://www.postgresql.org/download/windows/
# Executar instalador
# Criar usuário: postgres / senha: postgres (ou alterada)
```

**Linux:**
```bash
sudo apt-get install postgresql postgresql-contrib
sudo systemctl start postgresql
```

### 2️⃣ Criar Banco de Dados

```bash
# Conexão ao PostgreSQL
psql -U postgres

# Ou usar o script SQL fornecido
psql -U postgres -f backend/biotrilhos.sql
```

Alternativamente, as migrations criarão automaticamente:

```bash
cd backend
dotnet ef database update
```

### 3️⃣ Configurar Variáveis de Ambiente

**Arquivo: backend/appsettings.Development.json**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=biotrilhos;Username=postgres;Password=sua_senha;"
  }
}
```

### 4️⃣ Restaurar Dependências

```bash
cd backend
dotnet restore
```

### 5️⃣ Executar Migrations (se necessário)

```bash
cd backend
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 6️⃣ Executar a API

```bash
cd backend
dotnet run
```

A API estará disponível em:
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger

### 7️⃣ Executar o Frontend

```bash
# Abrir um navegador e acessar o arquivo:
# file:///caminho/para/Bio_Trilhos_Site/index.html
```

Ou usar um servidor local:

```bash
# Com Python 3:
python -m http.server 8000

# Com Node.js (http-server):
npx http-server
```

---

## 🔌 Integração com Arduino

### Exemplo de Código Arduino

```cpp
#include <WiFi.h>
#include <HTTPClient.h>
#include <ArduinoJson.h>

const char* ssid = "SEU_WIFI";
const char* password = "SENHA_WIFI";
const char* apiUrl = "http://SEU_IP:5000/api/iot/leituras";
const char* dispositivoId = "ARDUINO_ESTUFA_01";

void setup() {
  Serial.begin(115200);
  conectarWiFi();
}

void loop() {
  // Ler sensores
  float temperatura = lerTemperatura();
  float umidadeAr = lerUmidadeAr();
  float umidadeSolo = lerUmidadeSolo();
  int nivelAgua = lerNivelAgua();

  // Enviar para API
  enviarDados(temperatura, umidadeAr, umidadeSolo, nivelAgua);

  // Verificar comandos
  verificarComandos();

  delay(60000);  // A cada 1 minuto
}

void enviarDados(float temp, float ar, float solo, int agua) {
  if (WiFi.status() == WL_CONNECTED) {
    HTTPClient http;
    http.begin(apiUrl);
    http.addHeader("Content-Type", "application/json");

    StaticJsonDocument<200> doc;
    doc["dispositivoId"] = dispositivoId;
    doc["temperatura"] = temp;
    doc["umidadeAr"] = ar;
    doc["umidadeSolo"] = solo;
    doc["nivelAgua"] = agua;
    doc["dataHora"] = "2026-09-08T14:00:00";

    String payload;
    serializeJson(doc, payload);

    int httpResponseCode = http.POST(payload);
    Serial.println("Status: " + String(httpResponseCode));

    http.end();
  }
}

void verificarComandos() {
  if (WiFi.status() == WL_CONNECTED) {
    String url = String(apiUrl) + "/../comandos/" + dispositivoId;
    HTTPClient http;
    http.begin(url);

    int httpResponseCode = http.GET();
    if (httpResponseCode == 200) {
      String response = http.getString();
      Serial.println(response);
      // Parsear JSON e executar comandos
    }
    http.end();
  }
}
```

---

## 📊 Exemplo de Teste com cURL

```bash
# Listar estufas
curl http://localhost:5000/api/estufas

# Obter dashboard
curl http://localhost:5000/api/dashboard/1

# Registrar leitura
curl -X POST http://localhost:5000/api/leituras \
  -H "Content-Type: application/json" \
  -d '{
    "sensorId": 1,
    "valor": 25.5,
    "unidadeMedida": "°C",
    "status": "OK"
  }'

# Obter dados meteorológicos
curl http://localhost:5000/api/meteorologia/atual/1

# Enviar dados do Arduino
curl -X POST http://localhost:5000/api/iot/leituras \
  -H "Content-Type: application/json" \
  -d '{
    "dispositivoId": "ARDUINO_ESTUFA_01",
    "temperatura": 26.5,
    "umidadeAr": 72,
    "umidadeSolo": 48,
    "nivelAgua": 80,
    "dataHora": "2026-09-08T14:00:00"
  }'
```

---

## 🔒 Segurança

### Implementado

- ✅ Não armazena senhas em texto puro (preparado para Hash)
- ✅ Chaves de API em configurações (não no código)
- ✅ CORS configurável
- ✅ Validação de entrada em DTOs
- ✅ Logging de operações

### Recomendações para Produção

1. **Autenticação e Autorização**
   - Implementar JWT ou OAuth2
   - Roles: Admin, Operador, Visualização

2. **HTTPS**
   - Usar certificados SSL/TLS
   - Redirecionar HTTP para HTTPS

3. **Rate Limiting**
   - Limitar requisições por IP/usuário
   - Proteger contra DoS

4. **Validação Adicional**
   - Validar ALL inputs do Arduino
   - Sanitizar dados de terceiros

5. **Backup e Disaster Recovery**
   - Backup automático do PostgreSQL
   - Plano de recuperação

6. **Monitoramento**
   - Application Insights ou similar
   - Alertas em produção

---

## 📝 Estrutura de Dados - Enums

```csharp
// TipoSensor
0 = Temperatura
1 = UmidadeDoAr
2 = UmidadeDoSolo
3 = Luminosidade
4 = Chuva
5 = NivelDeAgua

// TipoAcionamento
0 = Automatico
1 = Manual

// StatusIrrigacao
0 = Ativa
1 = Finalizada
2 = Cancelada
3 = Erro

// TipoMovimentacao
0 = EntradaDeChuva
1 = ConsumoIrrigacao
2 = AtualizacaoManual
3 = Outro

// NivelAlerta
0 = Info
1 = Aviso
2 = Critico

// TipoUsuario
0 = Admin
1 = Operador
2 = Visualizacao
```

---

## 🎯 Próximas Melhorias

- [ ] Autenticação JWT
- [ ] Notificações por email/SMS
- [ ] Gráficos mais avançados
- [ ] App mobile (Flutter/React Native)
- [ ] Integração com sistemas de gestão agrícola
- [ ] IA para predição de irrigação
- [ ] Sistema de backup automático
- [ ] Multi-idioma
- [ ] Relatórios em PDF
- [ ] Integração com sensores adicionais (pH, condutividade, etc.)

---

## 📞 Suporte e Contribuições

Para reportar bugs ou sugerir melhorias, abra uma issue no repositório.

---

## 📄 Licença

Projeto desenvolvido para fins educacionais e de pesquisa.

---

**Última atualização**: 08 de Setembro de 2026
**Versão**: 1.0.0
