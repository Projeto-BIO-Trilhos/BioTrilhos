# Estufa.Api — Backend

**Visão rápida**: API ASP.NET Core para monitoramento da Estufa Tecnológica (sensores, leituras, irrigação, reservatórios, meteorologia). Inclui EF Core, JWT, SignalR e Swagger.

Arquivos relevantes: [backend/Estufa.Api/Program.cs](backend/Estufa.Api/Program.cs), [backend/Estufa.Api/appsettings.json](backend/Estufa.Api/appsettings.json), [backend/Estufa.Api/Data/DbSeeder.cs](backend/Estufa.Api/Data/DbSeeder.cs), [backend/Estufa.Api/Hubs/EstufaHub.cs](backend/Estufa.Api/Hubs/EstufaHub.cs).

**Pré-requisitos**
- .NET 7 SDK
- SQL Server (LocalDB é usado por padrão na `appsettings.json`)
- `dotnet-ef` (opcional, para migrações): `dotnet tool install --global dotnet-ef`

**Variáveis de ambiente recomendadas**
- `ConnectionStrings__DefaultConnection` — string de conexão para SQL Server (se não quer usar o `appsettings.json`).
- `Jwt__Key` — chave simétrica forte para gerar tokens JWT.

Defina a `Jwt__Key` em variáveis de ambiente (prioridade sobre `appsettings.json`). Exemplos:

PowerShell (Windows):
```powershell
$env:ConnectionStrings__DefaultConnection = 'Server=(localdb)\\mssqllocaldb;Database=EstufaDb;Trusted_Connection=True;'
$env:Jwt__Key = 'SUA_CHAVE_SUPER_SECRETA_AQUI'
```

Bash (Linux / macOS):
```bash
export ConnectionStrings__DefaultConnection='Server=(localdb)\\mssqllocaldb;Database=EstufaDb;Trusted_Connection=True;'
export Jwt__Key='SUA_CHAVE_SUPER_SECRETA_AQUI'
```

Nota: o código do `Program.cs` prioriza `Jwt__Key` do ambiente. Não deixe essa chave em repositórios públicos.

**Migrações / Banco de dados**
1. Navegue até a pasta do projeto: `backend/Estufa.Api`
2. Você pode usar os scripts fornecidos para criar migração inicial (se não existir), aplicar migrações e iniciar a API.

PowerShell (Windows):
```powershell
cd backend/Estufa.Api\scripts
./run_migrations_and_run.ps1 -MigrationName "InitialCreate"
```

Bash (Linux/macOS):
```bash
cd backend/Estufa.Api/scripts
./run_migrations_and_run.sh InitialCreate
```

Os scripts irão:
- Criar migração inicial se não houver pasta `Migrations`.
- Executar `dotnet ef database update`.
- Iniciar a aplicação com `dotnet run`.

Se preferir rodar manualmente:
```bash
dotnet ef migrations add Initial
dotnet ef database update
dotnet run
```

> Observação: o projeto já executa um seeder automático no startup que chama `DbSeeder.SeedAsync(...)`. Ele cria a base (EnsureCreated) e insere um usuário admin padrão se não houver usuários.

**Usuário seed (após startup)**
- Email: `admin@local`
- Senha: `Senha@123`

Altere a senha e a `Jwt__Key` antes de expor para produção.

**Executando a API (desenvolvimento)**
```bash
cd backend/Estufa.Api
dotnet run
```

**Executando testes unitários**
Os testes foram adicionados em `tests/Estufa.Tests` e usam EF Core InMemory para validar serviços principais.

```bash
cd tests/Estufa.Tests
dotnet test
```

A API será exposta (por padrão) em `https://localhost:5001` (dependendo das configurações). Swagger estará disponível em `/swagger` no ambiente de desenvolvimento.

**Endpoints úteis (resumo)**
- `POST /api/auth/register` — cadastrar usuário
- `POST /api/auth/login` — obter JWT
- `POST /api/sensores/leituras/enviar-dados` — enviar leituras (anônimo, usado pelo Arduino)
- `GET /api/sensores/leituras` — obter leituras recentes
- `POST /api/irrigacao/acionar` — acionar irrigação (protegido)
- `GET /api/reservatorios/principal` — obter nível do reservatório
- `POST /api/meteorologia/registrar` — registrar evento meteorológico (protegido)

**SignalR (Tempo real)**
- Hub: `/hubs/estufa`
- Evento broadcast: `NewLeitura` (payload: objeto `Leitura` salvo)

Exemplo curl para enviar leitura:
```bash
curl -k -X POST https://localhost:5001/api/sensores/leituras/enviar-dados \
  -H "Content-Type: application/json" \
  -d '{"sensorId":1,"temperatura":25.6,"umidadeAr":64,"umidadeSolo":42}'
```

**Notas de segurança & produção**
- Nunca guarde `Jwt:Key` em `appsettings.json` em produção; use variáveis de ambiente ou Azure Key Vault.
- Restrinja CORS em produção (no `Program.cs` atualmente há política `AllowAll` apenas para desenvolvimento).
- Configure HTTPS e certificados válidos.

---
Se precisar, posso gerar scripts de migração prontos, ajustar CORS para domínios específicos, ou preparar um arquivo `docker-compose` para orquestrar API + SQL Server + frontend.
