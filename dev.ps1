$ErrorActionPreference = "Stop"

Write-Host "Garantindo que o MySQL esta no ar..." -ForegroundColor Cyan
docker compose up -d mysql

Write-Host "Aguardando healthcheck..." -ForegroundColor Yellow
do {
    Start-Sleep -Seconds 1
    $status = docker inspect -f '{{.State.Health.Status}}' minhaapi-mysql 2>$null
} while ($status -ne "healthy")

Write-Host "MySQL pronto." -ForegroundColor Green
Write-Host "Iniciando a API..." -ForegroundColor Cyan
dotnet run --project src/MinhaApi.Api
