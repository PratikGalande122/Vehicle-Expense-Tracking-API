# Vehicle Expense API - Complete Test Script
# Run this in PowerShell after starting the API

$baseUrl = "https://localhost:7257/api"
$email = "demo@example.com"
$fullName = "Demo User"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Vehicle Expense API - Endpoint Tests" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# ============================================================================
# 1. AUTHENTICATION - Send OTP
# ============================================================================
Write-Host "1. Testing: POST /api/auth/send-otp" -ForegroundColor Yellow
$sendOtpResponse = Invoke-RestMethod -Uri "$baseUrl/auth/send-otp" `
    -Method POST `
    -ContentType "application/json" `
    -Body (@{
        email = $email
        fullName = $fullName
    } | ConvertTo-Json) `
    -SkipCertificateCheck

Write-Host "   ? OTP: $($sendOtpResponse.otp)" -ForegroundColor Green
$otp = $sendOtpResponse.otp

# ============================================================================
# 2. AUTHENTICATION - Verify OTP
# ============================================================================
Write-Host "`n2. Testing: POST /api/auth/verify-otp" -ForegroundColor Yellow
$verifyResponse = Invoke-RestMethod -Uri "$baseUrl/auth/verify-otp" `
    -Method POST `
    -ContentType "application/json" `
    -Body (@{
        email = $email
        otp = $otp
    } | ConvertTo-Json) `
    -SkipCertificateCheck

$token = $verifyResponse.accessToken
Write-Host "   ? UserId: $($verifyResponse.userId)" -ForegroundColor Green
Write-Host "   ? Name: $($verifyResponse.name)" -ForegroundColor Green
Write-Host "   ? Token: $($token.Substring(0, 50))..." -ForegroundColor Green

$headers = @{
    "Authorization" = "Bearer $token"
    "Content-Type" = "application/json"
}

# ============================================================================
# 3. USER PROFILE - Get Profile
# ============================================================================
Write-Host "`n3. Testing: GET /api/user/profile" -ForegroundColor Yellow
$profile = Invoke-RestMethod -Uri "$baseUrl/user/profile" `
    -Method GET `
    -Headers $headers `
    -SkipCertificateCheck

Write-Host "   ? Profile loaded: $($profile.fullName)" -ForegroundColor Green
Write-Host "   ? Email: $($profile.email)" -ForegroundColor Green

# ============================================================================
# 4. USER PROFILE - Update Profile
# ============================================================================
Write-Host "`n4. Testing: PUT /api/user/profile" -ForegroundColor Yellow
$updateProfileBody = @{
    mobileNumber = "9876543210"
    dateOfBirth = "1990-05-15"
    bloodGroup = 6  # OPositive
    emergencyContactName = "John Doe"
    emergencyContactNumber = "9876543211"
    emergencyContactRelation = "Father"
    fullName = "Demo User Updated"
    gender = 0
    address = "123 Test Street"
    city = "Mumbai"
    state = "Maharashtra"
    country = "India"
} | ConvertTo-Json

$updatedProfile = Invoke-RestMethod -Uri "$baseUrl/user/profile" `
    -Method PUT `
    -Headers $headers `
    -Body $updateProfileBody `
    -SkipCertificateCheck

Write-Host "   ? Profile updated: $($updatedProfile.fullName)" -ForegroundColor Green

# ============================================================================
# 5. DASHBOARD - Get Dashboard
# ============================================================================
Write-Host "`n5. Testing: GET /api/dashboard" -ForegroundColor Yellow
$dashboard = Invoke-RestMethod -Uri "$baseUrl/dashboard" `
    -Method GET `
    -Headers $headers `
    -SkipCertificateCheck

Write-Host "   ? Total Vehicles: $($dashboard.totalVehicles)" -ForegroundColor Green
Write-Host "   ? Total Fuel Cost: ?$($dashboard.totalFuelCost)" -ForegroundColor Green
Write-Host "   ? Total Expense Cost: ?$($dashboard.totalExpenseCost)" -ForegroundColor Green
Write-Host "   ? Total Cost: ?$($dashboard.totalCost)" -ForegroundColor Green

# ============================================================================
# 6. VEHICLES - Get All Vehicles
# ============================================================================
Write-Host "`n6. Testing: GET /api/vehicles" -ForegroundColor Yellow
$vehicles = Invoke-RestMethod -Uri "$baseUrl/vehicles" `
    -Method GET `
    -Headers $headers `
    -SkipCertificateCheck

Write-Host "   ? Vehicles count: $($vehicles.Count)" -ForegroundColor Green
if ($vehicles.Count -gt 0) {
    Write-Host "   ? First vehicle: $($vehicles[0].name) ($($vehicles[0].registrationNumber))" -ForegroundColor Green
    $vehicleId = $vehicles[0].id
}

# ============================================================================
# 7. VEHICLES - Add Vehicle
# ============================================================================
Write-Host "`n7. Testing: POST /api/vehicles" -ForegroundColor Yellow
$addVehicleBody = @{
    name = "Test Vehicle"
    registrationNumber = "TEST-9999"
    vehicleType = 2  # Car
    fuelType = 1     # Petrol
    year = 2024
    brand = "Honda"
    model = "City"
} | ConvertTo-Json

$newVehicle = Invoke-RestMethod -Uri "$baseUrl/vehicles" `
    -Method POST `
    -Headers $headers `
    -Body $addVehicleBody `
    -SkipCertificateCheck

Write-Host "   ? Vehicle added: $($newVehicle.name) (ID: $($newVehicle.id))" -ForegroundColor Green
$newVehicleId = $newVehicle.id

# ============================================================================
# 8. VEHICLES - Update Vehicle
# ============================================================================
Write-Host "`n8. Testing: PUT /api/vehicles/$newVehicleId" -ForegroundColor Yellow
$updateVehicleBody = @{
    name = "Test Vehicle Updated"
    registrationNumber = "TEST-9999"
    vehicleType = 2
    fuelType = 1
    year = 2024
    brand = "Honda"
    model = "City ZX"
} | ConvertTo-Json

$updatedVehicle = Invoke-RestMethod -Uri "$baseUrl/vehicles/$newVehicleId" `
    -Method PUT `
    -Headers $headers `
    -Body $updateVehicleBody `
    -SkipCertificateCheck

Write-Host "   ? Vehicle updated: $($updatedVehicle.name)" -ForegroundColor Green

# ============================================================================
# 9. FUEL LOGS - Get All Fuel Logs
# ============================================================================
Write-Host "`n9. Testing: GET /api/fuel" -ForegroundColor Yellow
$fuelLogs = Invoke-RestMethod -Uri "$baseUrl/fuel" `
    -Method GET `
    -Headers $headers `
    -SkipCertificateCheck

Write-Host "   ? Fuel logs count: $($fuelLogs.Count)" -ForegroundColor Green
if ($fuelLogs.Count -gt 0) {
    Write-Host "   ? First log: ?$($fuelLogs[0].totalCost) on $(Get-Date $fuelLogs[0].filledAt -Format 'yyyy-MM-dd')" -ForegroundColor Green
}

# ============================================================================
# 10. FUEL LOGS - Add Fuel Log
# ============================================================================
Write-Host "`n10. Testing: POST /api/fuel" -ForegroundColor Yellow
$addFuelBody = @{
    vehicleId = $vehicleId
    litresFilled = 45.5
    pricePerLitre = 110.0
    totalCost = 5005.0
    odometerReading = 25000.0
    filledAt = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
    notes = "Test fuel entry"
} | ConvertTo-Json

$newFuelLog = Invoke-RestMethod -Uri "$baseUrl/fuel" `
    -Method POST `
    -Headers $headers `
    -Body $addFuelBody `
    -SkipCertificateCheck

Write-Host "   ? Fuel log added: ?$($newFuelLog.totalCost) (ID: $($newFuelLog.id))" -ForegroundColor Green

# ============================================================================
# 11. EXPENSES - Get All Expenses
# ============================================================================
Write-Host "`n11. Testing: GET /api/expenses" -ForegroundColor Yellow
$expenses = Invoke-RestMethod -Uri "$baseUrl/expenses" `
    -Method GET `
    -Headers $headers `
    -SkipCertificateCheck

Write-Host "   ? Expenses count: $($expenses.Count)" -ForegroundColor Green
if ($expenses.Count -gt 0) {
    Write-Host "   ? First expense: ?$($expenses[0].amount) - $($expenses[0].description)" -ForegroundColor Green
}

# ============================================================================
# 12. EXPENSES - Add Expense
# ============================================================================
Write-Host "`n12. Testing: POST /api/expenses" -ForegroundColor Yellow
$addExpenseBody = @{
    vehicleId = $vehicleId
    expenseType = 2  # Service
    amount = 5500.0
    description = "Test service"
    expenseDate = (Get-Date).ToString("yyyy-MM-ddTHH:mm:ss")
} | ConvertTo-Json

$newExpense = Invoke-RestMethod -Uri "$baseUrl/expenses" `
    -Method POST `
    -Headers $headers `
    -Body $addExpenseBody `
    -SkipCertificateCheck

Write-Host "   ? Expense added: ?$($newExpense.amount) (ID: $($newExpense.id))" -ForegroundColor Green

# ============================================================================
# 13. VEHICLES - Delete Test Vehicle
# ============================================================================
Write-Host "`n13. Testing: DELETE /api/vehicles/$newVehicleId" -ForegroundColor Yellow
Invoke-RestMethod -Uri "$baseUrl/vehicles/$newVehicleId" `
    -Method DELETE `
    -Headers $headers `
    -SkipCertificateCheck | Out-Null

Write-Host "   ? Vehicle deleted successfully" -ForegroundColor Green

# ============================================================================
# 14. HEALTH CHECK
# ============================================================================
Write-Host "`n14. Testing: GET /health" -ForegroundColor Yellow
$health = Invoke-RestMethod -Uri "https://localhost:7257/health" `
    -Method GET `
    -SkipCertificateCheck

Write-Host "   ? Health status: $($health.status)" -ForegroundColor Green

# ============================================================================
# SUMMARY
# ============================================================================
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "? All API Endpoints Tested Successfully!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "`nToken for manual testing:" -ForegroundColor Yellow
Write-Host $token -ForegroundColor White
Write-Host "`nUse this token in Swagger or your Angular app!" -ForegroundColor Yellow
