# 🚀 GUIA DE INSTALAÇÃO E EXECUÇÃO - BIO TRILHOS

## Configuração Rápida (5 minutos)

### 1️⃣ Pré-requisitos

Instale em seu computador:
- **PostgreSQL 13+**: https://www.postgresql.org/download/
- **.NET 8.0 SDK**: https://dotnet.microsoft.com/download

### 2️⃣ Criar Banco de Dados

Abra o terminal e execute:

```bash
# Conecte ao PostgreSQL
psql -U postgres

# Dentro do psql, crie o banco:
CREATE DATABASE biotrilhos 
    WITH ENCODING='UTF8' LC_COLLATE='C' LC_CTYPE='C';

\c biotrilhos

# Copie e execute todo o conteúdo do arquivo backend/biotrilhos.sql
```

**OU** use o arquivo SQL diretamente:

```bash
psql -U postgres -f backend/biotrilhos.sql
```

### 3️⃣ Configurar Conexão do PostgreSQL

**Arquivo**: `backend/appsettings.json`

Altere a string de conexão conforme suas credenciais:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=biotrilhos;Username=postgres;Password=SUA_SENHA;"
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
dotnet ef database update
```

### 6️⃣ Iniciar a API

```bash
cd backend
dotnet run
```

**Saída esperada:**
```
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
      Now listening on: https://localhost:5001
```

### 7️⃣ Acessar o Dashboard

**Opção A: Arquivo local**
```
Abra em seu navegador:
file:///C:/Users/seu_usuario/Downloads/Bio_Trilhos_Site/index.html
```

**Opção B: Servidor local (Python 3)**
```bash
cd Bio_Trilhos_Site
python -m http.server 8000
```
Abra: http://localhost:8000

**Opção C: Servidor local (Node.js)**
```bash
cd Bio_Trilhos_Site
npx http-server
```

### 8️⃣ Verificar Swagger (Documentação Interativa)

Acesse: http://localhost:5000/swagger

---

## 📌 Checklist de Sucesso

- [x] PostgreSQL instalado e rodando
- [x] Banco de dados `biotrilhos` criado
- [x] .NET 8.0 SDK instalado
- [x] `dotnet run` executa sem erros
- [x] Dashboard carrega em http://localhost:8000
- [x] Botão "Atualizar" funciona
- [x] Dados aparecem em tempo real

---

## 🔧 Troubleshooting

### ❌ Erro: "Connection refused" PostgreSQL

```bash
# Verificar se PostgreSQL está rodando
# Windows:
Get-Service postgresql*

# Linux:
systemctl status postgresql
```

**Solução**: Inicie o PostgreSQL

### ❌ Erro: ".NET SDK not found"

```bash
dotnet --version
```

**Solução**: Baixe e instale .NET 8.0 de: https://dotnet.microsoft.com/download

### ❌ Erro: "localhost:5000 connection refused"

A API não está rodando. Verifique:
```bash
cd backend
dotnet run
```

### ❌ Erro: "CORS error" no navegador

**Solução**: Certifique-se que a API está em http://localhost:5000

### ❌ Dados aparecem como null

**Solução**: Aguarde alguns segundos para a API carregar dados. Clique em "Atualizar".

---

## 🧪 Teste de Endpoints com cURL

```bash
# Listar estufas
curl http://localhost:5000/api/estufas

# Obter dashboard
curl http://localhost:5000/api/dashboard/1

# Obter sensores
curl http://localhost:5000/api/sensores?estufaId=1

# Registrar leitura de sensor
curl -X POST http://localhost:5000/api/leituras \
  -H "Content-Type: application/json" \
  -d '{
    "sensorId": 1,
    "valor": 25.5,
    "unidadeMedida": "°C",
    "status": "OK"
  }'

# Simular dados do Arduino
curl -X POST http://localhost:5000/api/iot/leituras \
  -H "Content-Type: application/json" \
  -d '{
    "dispositivoId": "ARDUINO_ESTUFA_01",
    "temperatura": 26.5,
    "umidadeAr": 72,
    "umidadeSolo": 48,
    "nivelAgua": 80,
    "dataHora": "2026-09-08T14:30:00"
  }'

# Iniciar irrigação manual
curl -X POST http://localhost:5000/api/irrigacao/iniciar \
  -H "Content-Type: application/json" \
  -d '{
    "estufaId": 1,
    "reservatorioId": 1,
    "motivo": "Teste manual",
    "duracaoSegundos": 60,
    "tipoAcionamento": 1
  }'

# Parar irrigação
curl -X POST http://localhost:5000/api/irrigacao/parar \
  -H "Content-Type: application/json" \
  -d '1'

# Obter dados meteorológicos
curl http://localhost:5000/api/meteorologia/atual/1
```

---

## 📚 Estrutura de Dados

### Tipos de Sensores (enum)
```
0 = Temperatura
1 = Umidade do Ar
2 = Umidade do Solo
3 = Luminosidade
4 = Sensor de Chuva
5 = Nível de Água
```

### Tipos de Acionamento
```
0 = Automático
1 = Manual
```

### Status de Irrigação
```
0 = Ativa
1 = Finalizada
2 = Cancelada
3 = Erro
```

---

## 🔌 Dados Iniciais (Já Inseridos)

**Estufa:**
- Nome: Bio Trilhos - Estufa Principal
- Localização: Cruzeiro - São Paulo - Brasil

**Sensores:**
- Sensor de Temperatura (TEMP_001)
- Sensor de Umidade do Ar (UMID_AR_001)
- Sensor de Umidade do Solo (UMID_SOLO_001)
- Sensor de Nível de Água (NIVEL_001)
- Sensor de Chuva (CHUVA_001)

**Reservatório:**
- Nome: Reservatório Principal
- Capacidade: 1.000 L
- Nível atual: 820 L (82%)

**Configurações Ambientais:**
- Umidade solo: 50% - 75%
- Temperatura: 20°C - 28°C
- Umidade ar: 60% - 80%
- Tempo máx. irrigação: 300 segundos
- Nível mín. reservatório: 20%

---

## 📊 Exemplo de Fluxo Completo

1. Arduino envia dados para `/api/iot/leituras`
2. API salva leituras no banco de dados
3. AutomationService verifica se deve irrigar
4. Frontend consulta `/api/dashboard/1`
5. Dashboard exibe dados em tempo real
6. Se solo < 50%, sistema ativa irrigação automaticamente
7. Histórico é registrado

---

## 🌐 API Meteorológica

**Open-Meteo** (Gratuita, sem chave)

Coordenadas de Cruzeiro, SP:
- Latitude: -22.6597
- Longitude: -44.9737

Dados atualizados automaticamente cada vez que você acessa `/api/meteorologia/atual/1`

---

## 📝 Próximas Ações

- [ ] Conectar Arduino ao sistema
- [ ] Configurar email para alertas
- [ ] Implementar autenticação
- [ ] Fazer backup automático
- [ ] Monitorar performance
- [ ] Documentar casos de uso

---

**Dúvidas?** Consulte o README.md completo na pasta backend/

