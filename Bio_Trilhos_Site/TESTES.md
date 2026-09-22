# 📝 EXEMPLOS DE TESTE - BIO TRILHOS API

Este arquivo contém exemplos práticos de como testar a API usando diferentes ferramentas.

---

## 🧪 Usando cURL (Windows PowerShell / Linux)

### 1. Testar Conectividade

```bash
curl http://localhost:5000/api/estufas
```

### 2. Listar Estufas

```bash
curl http://localhost:5000/api/estufas -v
```

### 3. Obter Dashboard

```bash
curl http://localhost:5000/api/dashboard/1
```

### 4. Registrar Leitura de Sensor

```bash
curl -X POST http://localhost:5000/api/leituras `
  -H "Content-Type: application/json" `
  -d '{
    "sensorId": 1,
    "valor": 25.5,
    "unidadeMedida": "°C",
    "status": "OK"
  }'
```

### 5. Simular Dados do Arduino

```bash
curl -X POST http://localhost:5000/api/iot/leituras `
  -H "Content-Type: application/json" `
  -d '{
    "dispositivoId": "ARDUINO_ESTUFA_01",
    "temperatura": 26.5,
    "umidadeAr": 72,
    "umidadeSolo": 48,
    "luminosidade": 650,
    "nivelAgua": 80,
    "chuva": false,
    "dataHora": "2026-09-08T14:30:00"
  }'
```

### 6. Iniciar Irrigação Manual

```bash
curl -X POST http://localhost:5000/api/irrigacao/iniciar `
  -H "Content-Type: application/json" `
  -d '{
    "estufaId": 1,
    "reservatorioId": 1,
    "motivo": "Teste manual do operador",
    "duracaoSegundos": 120,
    "tipoAcionamento": 1
  }'
```

### 7. Parar Irrigação

```bash
curl -X POST http://localhost:5000/api/irrigacao/parar `
  -H "Content-Type: application/json" `
  -d '1'
```

### 8. Obter Dados Meteorológicos Atuais

```bash
curl http://localhost:5000/api/meteorologia/atual/1
```

### 9. Obter Histórico Meteorológico

```bash
curl "http://localhost:5000/api/meteorologia/historico/1?dias=7"
```

### 10. Obter Configurações da Estufa

```bash
curl http://localhost:5000/api/configuracoes/1
```

### 11. Atualizar Configurações

```bash
curl -X PUT http://localhost:5000/api/configuracoes/1 `
  -H "Content-Type: application/json" `
  -d '{
    "umidadeSoloMinima": 45,
    "umidadeSoloMaxima": 80,
    "temperaturaMinnima": 18,
    "temperaturaMaxima": 30
  }'
```

### 12. Obter Comandos Pendentes (Arduino)

```bash
curl http://localhost:5000/api/iot/comandos/ARDUINO_ESTUFA_01
```

---

## 🔌 Usando Postman

### Importar Coleção

1. Abra o Postman
2. Click em "Import"
3. Selecione "Raw text"
4. Copie e cole o JSON abaixo:

```json
{
  "info": {
    "name": "Bio Trilhos API",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "Estufas",
      "item": [
        {
          "name": "Listar",
          "request": {
            "method": "GET",
            "url": "http://localhost:5000/api/estufas"
          }
        },
        {
          "name": "Obter uma",
          "request": {
            "method": "GET",
            "url": "http://localhost:5000/api/estufas/1"
          }
        }
      ]
    },
    {
      "name": "Dashboard",
      "item": [
        {
          "name": "Obter dados",
          "request": {
            "method": "GET",
            "url": "http://localhost:5000/api/dashboard/1"
          }
        }
      ]
    },
    {
      "name": "Sensores",
      "item": [
        {
          "name": "Listar",
          "request": {
            "method": "GET",
            "url": "http://localhost:5000/api/sensores?estufaId=1"
          }
        },
        {
          "name": "Últimas leituras",
          "request": {
            "method": "GET",
            "url": "http://localhost:5000/api/leituras/ultimas/1"
          }
        }
      ]
    },
    {
      "name": "IoT/Arduino",
      "item": [
        {
          "name": "Enviar leituras",
          "request": {
            "method": "POST",
            "header": [
              {
                "key": "Content-Type",
                "value": "application/json"
              }
            ],
            "url": "http://localhost:5000/api/iot/leituras",
            "body": {
              "mode": "raw",
              "raw": "{\n  \"dispositivoId\": \"ARDUINO_ESTUFA_01\",\n  \"temperatura\": 26.5,\n  \"umidadeAr\": 72,\n  \"umidadeSolo\": 48,\n  \"nivelAgua\": 80,\n  \"dataHora\": \"2026-09-08T14:30:00\"\n}"
            }
          }
        },
        {
          "name": "Obter comandos",
          "request": {
            "method": "GET",
            "url": "http://localhost:5000/api/iot/comandos/ARDUINO_ESTUFA_01"
          }
        }
      ]
    }
  ]
}
```

---

## 🐍 Usando Python

```python
import requests
import json
from datetime import datetime

BASE_URL = "http://localhost:5000/api"
ESTUFA_ID = 1

def test_api():
    # 1. Listar estufas
    response = requests.get(f"{BASE_URL}/estufas")
    print("Estufas:", response.json())
    
    # 2. Obter dashboard
    response = requests.get(f"{BASE_URL}/dashboard/{ESTUFA_ID}")
    dashboard = response.json()
    print(f"\n📊 Dashboard:")
    print(f"  Temperatura: {dashboard['temperaturaAtual']:.1f}°C")
    print(f"  Umidade Solo: {dashboard['umidadeSoloAtual']:.0f}%")
    print(f"  Água: {dashboard['reservatorioPercentual']:.0f}%")
    
    # 3. Enviar leitura de sensor
    leitura = {
        "sensorId": 1,
        "valor": 25.5,
        "unidadeMedida": "°C",
        "status": "OK"
    }
    response = requests.post(f"{BASE_URL}/leituras", json=leitura)
    print(f"\n✓ Leitura registrada: {response.status_code}")
    
    # 4. Simular dados do Arduino
    dados_arduino = {
        "dispositivoId": "ARDUINO_ESTUFA_01",
        "temperatura": 26.5,
        "umidadeAr": 72,
        "umidadeSolo": 48,
        "nivelAgua": 80,
        "dataHora": datetime.now().isoformat()
    }
    response = requests.post(f"{BASE_URL}/iot/leituras", json=dados_arduino)
    print(f"\n✓ Dados Arduino enviados: {response.status_code}")
    
    # 5. Iniciar irrigação
    irrigacao = {
        "estufaId": ESTUFA_ID,
        "reservatorioId": 1,
        "motivo": "Teste automático",
        "duracaoSegundos": 60,
        "tipoAcionamento": 1
    }
    response = requests.post(f"{BASE_URL}/irrigacao/iniciar", json=irrigacao)
    print(f"\n✓ Irrigação iniciada: {response.status_code}")
    
    # 6. Obter dados meteorológicos
    response = requests.get(f"{BASE_URL}/meteorologia/atual/{ESTUFA_ID}")
    clima = response.json()
    print(f"\n🌤️  Clima:")
    print(f"  Temperatura externa: {clima['temperaturaExterna']:.1f}°C")
    print(f"  Chovendo: {'Sim' if clima['estaChovendo'] else 'Não'}")
    print(f"  Chance de chuva: {clima['probabilidadeChuva']:.0f}%")

if __name__ == "__main__":
    test_api()
```

**Executar:**
```bash
pip install requests
python test_api.py
```

---

## 🧬 Usando JavaScript/Node.js

```javascript
const axios = require('axios');

const BASE_URL = 'http://localhost:5000/api';
const ESTUFA_ID = 1;

async function testAPI() {
  try {
    // 1. Listar estufas
    const estufas = await axios.get(`${BASE_URL}/estufas`);
    console.log('Estufas:', estufas.data);
    
    // 2. Obter dashboard
    const dashboard = await axios.get(`${BASE_URL}/dashboard/${ESTUFA_ID}`);
    console.log('\n📊 Dashboard:');
    console.log(`  Temperatura: ${dashboard.data.temperaturaAtual.toFixed(1)}°C`);
    console.log(`  Umidade Solo: ${dashboard.data.umidadeSoloAtual.toFixed(0)}%`);
    console.log(`  Água: ${dashboard.data.reservatorioPercentual.toFixed(0)}%`);
    
    // 3. Enviar leitura
    const leitura = await axios.post(`${BASE_URL}/leituras`, {
      sensorId: 1,
      valor: 25.5,
      unidadeMedida: '°C',
      status: 'OK'
    });
    console.log('\n✓ Leitura registrada:', leitura.status);
    
    // 4. Dados do Arduino
    const arduino = await axios.post(`${BASE_URL}/iot/leituras`, {
      dispositivoId: 'ARDUINO_ESTUFA_01',
      temperatura: 26.5,
      umidadeAr: 72,
      umidadeSolo: 48,
      nivelAgua: 80
    });
    console.log('\n✓ Arduino:', arduino.status);
    
    // 5. Meteorologia
    const clima = await axios.get(`${BASE_URL}/meteorologia/atual/${ESTUFA_ID}`);
    console.log('\n🌤️  Clima:');
    console.log(`  Temp: ${clima.data.temperaturaExterna.toFixed(1)}°C`);
    console.log(`  Chovendo: ${clima.data.estaChovendo ? 'Sim' : 'Não'}`);
    
  } catch (error) {
    console.error('Erro:', error.message);
  }
}

testAPI();
```

**Executar:**
```bash
npm install axios
node test_api.js
```

---

## 🧪 Teste de Automação

### Cenário: Irrigação Automática

1. Registre uma leitura baixa de umidade do solo (< 50%)
```bash
curl -X POST http://localhost:5000/api/leituras `
  -H "Content-Type: application/json" `
  -d '{
    "sensorId": 3,
    "valor": 40,
    "unidadeMedida": "%",
    "status": "OK"
  }'
```

2. Verifique se irrigação foi acionada automaticamente
```bash
curl http://localhost:5000/api/irrigacao/ativa/1
```

3. Envie dados do Arduino (simular melhoria)
```bash
curl -X POST http://localhost:5000/api/iot/leituras `
  -H "Content-Type: application/json" `
  -d '{
    "dispositivoId": "ARDUINO_ESTUFA_01",
    "umidadeSolo": 65
  }'
```

4. Verifique alertas
```bash
curl http://localhost:5000/api/dashboard/1
```

---

## 📈 Teste de Performance

```bash
# Teste de carga: 100 requisições sequenciais
for i in {1..100}; do
  curl http://localhost:5000/api/dashboard/1
done
```

---

## ✅ Checklist de Testes

- [x] API conecta ao banco de dados
- [x] GET /api/estufas retorna dados
- [x] GET /api/dashboard/1 retorna dados completos
- [x] POST /api/leituras salva no banco
- [x] POST /api/iot/leituras processa Arduino
- [x] Irrigação automática funciona
- [x] API meteorológica está funcionando
- [x] Frontend carrega dados reais
- [x] Dashboard atualiza a cada 30s
- [x] Swagger está acessível

---

**Dúvidas ou erros?** Consulte o README.md ou SETUP.md

