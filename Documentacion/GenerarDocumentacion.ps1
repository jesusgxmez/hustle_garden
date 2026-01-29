# Script para generar y servir la documentación de Hustle Garden
# Uso: .\GenerarDocumentacion.ps1 [-Servir]

param(
    [switch]$Servir
)

$ErrorActionPreference = "Stop"
$documentacionPath = Split-Path -Parent $MyInvocation.MyCommand.Path

Write-Host "=== Generador de Documentación Hustle Garden ===" -ForegroundColor Cyan
Write-Host ""

# Verificar que DocFX esté instalado
try {
    $docfxVersion = docfx --version 2>&1
    Write-Host "? DocFX encontrado: $docfxVersion" -ForegroundColor Green
} catch {
    Write-Host "? DocFX no está instalado" -ForegroundColor Red
    Write-Host "Instalando DocFX..." -ForegroundColor Yellow
    dotnet tool install -g docfx
    Write-Host "? DocFX instalado correctamente" -ForegroundColor Green
}

Write-Host ""
Write-Host "Generando documentación..." -ForegroundColor Yellow

# Navegar al directorio de documentación
Set-Location $documentacionPath

# Generar documentación
docfx docfx.json

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "? Documentación generada exitosamente" -ForegroundColor Green
    Write-Host "  Ubicación: $documentacionPath\_site" -ForegroundColor Gray
    
    if ($Servir) {
        Write-Host ""
        Write-Host "Iniciando servidor web en http://localhost:8080" -ForegroundColor Yellow
        Write-Host "Presiona Ctrl+C para detener el servidor" -ForegroundColor Gray
        Write-Host ""
        
        # Abrir navegador
        Start-Process "http://localhost:8080"
        
        # Servir la documentación
        docfx serve _site
    } else {
        Write-Host ""
        Write-Host "Para ver la documentación, ejecuta:" -ForegroundColor Cyan
        Write-Host "  docfx serve _site" -ForegroundColor White
        Write-Host "  o" -ForegroundColor Gray
        Write-Host "  .\GenerarDocumentacion.ps1 -Servir" -ForegroundColor White
    }
} else {
    Write-Host ""
    Write-Host "? Error al generar la documentación" -ForegroundColor Red
    exit 1
}
