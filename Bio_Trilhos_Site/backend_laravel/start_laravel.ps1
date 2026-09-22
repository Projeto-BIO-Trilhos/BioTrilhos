# Start Laravel automation script (Windows PowerShell)
# Usage: Open PowerShell as admin and run ./start_laravel.ps1

param()

function Write-Err($m){ Write-Host "[ERROR] $m" -ForegroundColor Red }
function Write-Ok($m){ Write-Host "[OK] $m" -ForegroundColor Green }

# Config
$projectDir = Join-Path $PSScriptRoot 'backend_laravel_app'
$port = 5000

# Check prerequisites
if (-not (Get-Command php -ErrorAction SilentlyContinue)) { Write-Err "PHP não encontrado. Instale PHP 8.1+ e tente novamente."; exit 1 }
if (-not (Get-Command composer -ErrorAction SilentlyContinue)) { Write-Err "Composer não encontrado. Instale Composer e tente novamente."; exit 1 }
if (-not (Get-Command git -ErrorAction SilentlyContinue)) { Write-Host "Aviso: Git não encontrado. Continuando sem Git..." }

# Create Laravel project if not exists
if (-not (Test-Path $projectDir)) {
    Write-Host "Criando projeto Laravel em: $projectDir"
    composer create-project --prefer-dist laravel/laravel $projectDir
    if ($LASTEXITCODE -ne 0) { Write-Err "composer create-project falhou"; exit 1 }
    Write-Ok "Projeto Laravel criado"
} else {
    Write-Host "Projeto já existe em $projectDir"
}

# Copy stubs if available
$stubsDir = Join-Path $PSScriptRoot 'stubs'
if (Test-Path $stubsDir) {
    Write-Host "Copiando stubs para o projeto..."
    Copy-Item -Path (Join-Path $stubsDir '*') -Destination (Join-Path $projectDir '') -Recurse -Force
    Write-Ok "Stubs copiados"
} else {
    Write-Host "Nenhum stub local encontrado. Você pode adicionar controllers e rotas manualmente." -ForegroundColor Yellow
}

# Prompt for DB settings
$defaultHost = '127.0.0.1'
$dbHost = Read-Host "Host do PostgreSQL (padrão: $defaultHost)"; if ([string]::IsNullOrWhiteSpace($dbHost)) { $dbHost = $defaultHost }
$dbPort = Read-Host "Porta do PostgreSQL (padrão: 5432)"; if ([string]::IsNullOrWhiteSpace($dbPort)) { $dbPort = 5432 }
$dbName = Read-Host "Nome do banco (padrão: biotrilhos)"; if ([string]::IsNullOrWhiteSpace($dbName)) { $dbName = 'biotrilhos' }
$dbUser = Read-Host "Usuário do banco (padrão: postgres)"; if ([string]::IsNullOrWhiteSpace($dbUser)) { $dbUser = 'postgres' }
$dbPass = Read-Host "Senha do banco (deixe vazio para none)" -AsSecureString | ConvertFrom-SecureString

# Update .env
$envPath = Join-Path $projectDir '.env'
if (Test-Path $envPath) {
    (Get-Content $envPath) -replace 'DB_CONNECTION=.*', "DB_CONNECTION=pgsql" `
        -replace 'DB_HOST=.*', "DB_HOST=$dbHost" `
        -replace 'DB_PORT=.*', "DB_PORT=$dbPort" `
        -replace 'DB_DATABASE=.*', "DB_DATABASE=$dbName" `
        -replace 'DB_USERNAME=.*', "DB_USERNAME=$dbUser" `
        -replace 'DB_PASSWORD=.*', "DB_PASSWORD=$(ConvertTo-SecureString -String $dbPass -AsPlainText -Force)" | Set-Content $envPath
    Write-Ok "Arquivo .env atualizado (verifique manualmente se necessário)"
} else {
    Write-Err "Arquivo .env não encontrado em $projectDir. Edite manualmente.";
}

# Install dependencies (inside project)
Push-Location $projectDir
Write-Host "Instalando dependências..."
composer install
if ($LASTEXITCODE -ne 0) { Write-Err "composer install falhou"; Pop-Location; exit 1 }
Write-Ok "Dependências instaladas"

# Run migrations (user must ensure DB exists)
Write-Host "Executando migrations (php artisan migrate). Certifique-se que o banco '$dbName' existe." -ForegroundColor Yellow
php artisan migrate --force

# Serve the app
Write-Host "Iniciando servidor Laravel em http://localhost:$port"
Start-Process -NoNewWindow -FilePath php -ArgumentList "artisan", "serve", "--host=127.0.0.1", "--port=$port"
Write-Ok "Servidor iniciado (verifique a saída). Abra http://localhost:$port"
Pop-Location

Write-Host "Script concluído." -ForegroundColor Green