# Gera um dump de todo o código-fonte (src + tests), excluindo bin/obj,
# pra permitir uma revisão completa da estrutura de uma vez só.

$raiz = Get-Location
$saida = "dump-codigo.txt"

if (Test-Path $saida) { Remove-Item $saida }

Get-ChildItem -Path src, tests -Recurse -Filter "*.cs" -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' } |
    Sort-Object FullName |
    ForEach-Object {
        $caminhoRelativo = $_.FullName.Replace("$raiz\", "")
        Add-Content -Path $saida -Value "===== $caminhoRelativo ====="
        Get-Content $_.FullName -Raw | Add-Content -Path $saida
        Add-Content -Path $saida -Value ""
    }

Write-Host "Dump gerado em: $saida"
Write-Host "Tamanho: $((Get-Item $saida).Length / 1KB) KB"