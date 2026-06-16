param(
    [string]$MigrationName = "InitialCreate",
    [string]$ConnectionString = $env:ConnectionStrings__DefaultConnection,
    [string]$JwtKey = $env:Jwt__Key
)

Set-Location -Path (Split-Path -Path $MyInvocation.MyCommand.Definition -Parent)
Set-Location ..

if ($ConnectionString) {
    Write-Host "Using provided connection string from arg or environment."
    $env:ConnectionStrings__DefaultConnection = $ConnectionString
}
if ($JwtKey) {
    Write-Host "Using provided Jwt__Key from arg or environment."
    $env:Jwt__Key = $JwtKey
}

if (-not (Test-Path .\Migrations)) {
    Write-Host "No Migrations folder found — creating initial migration: $MigrationName"
    dotnet ef migrations add $MigrationName
}

Write-Host "Applying migrations to database..."
dotnet ef database update

Write-Host "Starting application..."
dotnet run
