param()

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "RECONSTRUIR Y REINICIAR SERVICIOS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Write-Host "PASO 1: Deteniendo contenedores..." -ForegroundColor Yellow
docker-compose down
Write-Host "OK" -ForegroundColor Green
Write-Host ""

Write-Host "PASO 2: Recompilando imagenes..." -ForegroundColor Yellow
docker-compose build --no-cache
Write-Host "OK" -ForegroundColor Green
Write-Host ""

Write-Host "PASO 3: Iniciando servicios..." -ForegroundColor Yellow
docker-compose up -d
Write-Host "OK" -ForegroundColor Green
Write-Host ""

Write-Host "PASO 4: Esperando que los servicios esten listos..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "VERIFICANDO STATUS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

docker-compose ps

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "SERVICIOS DISPONIBLES EN:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "UserService:      http://localhost:5000" -ForegroundColor Cyan
Write-Host "AuthService:      http://localhost:5002" -ForegroundColor Cyan
Write-Host "AuditService:     http://localhost:5001" -ForegroundColor Cyan
Write-Host ""
