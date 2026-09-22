# Inicia a API .NET existente em backend/
# Uso: Execute este script no PowerShell (não como administrador necessariamente)

$backendPath = Join-Path $PSScriptRoot 'backend'

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    Write-Host '[ERROR] dotnet não encontrado. Instale .NET 8 SDK: https://dotnet.microsoft.com/download' -ForegroundColor Red
    exit 1
}

Write-Host "Entrando em: $backendPath"
Push-Location $backendPath

Write-Host 'Restaurando dependências...'
dotnet restore
if ($LASTEXITCODE -ne 0) { Write-Host '[ERROR] dotnet restore falhou' -ForegroundColor Red; Pop-Location; exit 1 }

# Aplicar migrations se existir dotnet-ef
Write-Host 'Aplicando migrations (se aplicável)...'
try {
    dotnet ef database update -v
} catch {
    Write-Host 'Aviso: dotnet-ef pode não estar disponível. Selecione manualmente rodar migrations.' -ForegroundColor Yellow
}

Write-Host 'Iniciando aplicação (.NET)...'
# Inicia o dotnet run na mesma janela (para ver logs)
dotnet run

Pop-Location
