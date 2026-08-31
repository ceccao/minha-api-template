$ErrorActionPreference = "Stop"

# MySQL roda nativo no Windows (Servico "MySQL80"), nao em Docker.
# O Windows ja garante que ele sobe sozinho no boot (Startup Type: Automatic).
Write-Host "Verificando o servico do MySQL..." -ForegroundColor Cyan
$mysqlService = Get-Service -Name "MySQL80" -ErrorAction SilentlyContinue
if ($null -eq $mysqlService) {
    Write-Host "Servico MySQL80 nao encontrado. Verifique o nome do servico com 'Get-Service *mysql*'." -ForegroundColor Red
    exit 1
}
if ($mysqlService.Status -ne "Running") {
    Write-Host "Iniciando o servico MySQL..." -ForegroundColor Yellow
    Start-Service -Name "MySQL80"
}

Write-Host "MySQL pronto." -ForegroundColor Green
Write-Host "Iniciando a API..." -ForegroundColor Cyan
dotnet run --project src/MinhaApi.Api --launch-profile https