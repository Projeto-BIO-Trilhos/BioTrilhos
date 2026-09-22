# Inicia ambos os backends (Laravel + .NET) e o frontend (se possível)
# Uso: Execute no PowerShell. Este script abre novas janelas do PowerShell para cada serviço.

$root = $PSScriptRoot
$laravelScript = Join-Path $root 'backend_laravel\start_laravel.ps1'
$dotnetScript = Join-Path $root 'start_dotnet.ps1'
$frontendPath = $root

function Start-ServiceWindow($scriptPath, $title) {
    if (-not (Test-Path $scriptPath)) { Write-Host "[WARN] Script não encontrado: $scriptPath" -ForegroundColor Yellow; return }
    $psArgs = "-NoExit -ExecutionPolicy Bypass -File '$scriptPath'"
    Start-Process -FilePath powershell -ArgumentList $psArgs -WindowStyle Normal
    Write-Host "Janela iniciada para: $title"
}

# Iniciar Laravel (cria projeto se necessário)
Start-ServiceWindow $laravelScript 'Laravel'

# Iniciar .NET
Start-ServiceWindow $dotnetScript '.NET API'

# Iniciar frontend com Python se disponível
if (Get-Command python -ErrorAction SilentlyContinue) {
    Write-Host 'Iniciando servidor HTTP simples (python -m http.server 8000) na pasta do frontend...'
    Start-Process -FilePath python -ArgumentList '-m','http.server','8000' -WorkingDirectory $frontendPath
    Write-Host 'Frontend disponível em http://localhost:8000'
} elseif (Get-Command npx -ErrorAction SilentlyContinue) {
    Write-Host 'Iniciando http-server via npx...'
    Start-Process -FilePath npx -ArgumentList 'http-server' -WorkingDirectory $frontendPath
    Write-Host 'Frontend iniciado via npx http-server'
} else {
    Write-Host 'Nenhum servidor HTTP encontrado (python/npx). Start manual do frontend necessário.' -ForegroundColor Yellow
}

Write-Host 'Comandos iniciados. Verifique as janelas do PowerShell abertas para logs.' -ForegroundColor Green
