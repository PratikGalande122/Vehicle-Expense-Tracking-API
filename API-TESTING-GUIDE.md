# ?? API Testing Guide

## ? CORS Configuration Applied

Your .NET API now accepts requests from:
- `http://localhost:4200` (Angular default)
- `https://localhost:4200`
- `http://localhost:5173` (Vite default)
- `https://localhost:5173`

---

## ?? Quick Start - Test All Endpoints

### Option 1: PowerShell Script (Recommended)

**Stop and restart your API first**, then run:

```powershell
# Navigate to your project directory
cd D:\PersonalProject\VehicleExpenseTrackingAPI

# Run the comprehensive test
.\test-all-endpoints.ps1
```

This will test all 14 endpoints and give you a fresh JWT token!

---

### Option 2: Browser CORS Test

1. **Serve the HTML file on port 4200:**

```bash
# Using npx (if you have Node.js)
npx http-server -p 4200

# OR using Python
python -m http.server 4200
```

2. **Open in browser:**
```
http://localhost:4200/cors-test.html
```

3. **Click the buttons in order:**
   - 1?? Send OTP
   - 2?? Verify OTP
   - 3?? Get Profile
   - 4?? Get Dashboard
   - 5?? Get Vehicles

If all buttons work = ? CORS is working perfectly!

---

## ?? Manual Testing in Swagger

1. **Stop the debugger and restart your API** (CORS changes need restart)

2. **Navigate to Swagger:**
```
https://localhost:7257/swagger
```

3. **Get a fresh token:**

**Step 1:** POST `/api/auth/send-otp`
```json
{
  "email": "demo@example.com",
  "fullName": "Demo User"
}
```
Copy the `otp` from response.

**Step 2:** POST `/api/auth/verify-otp`
```json
{
  "email": "demo@example.com",
  "otp": "123456"
}
```
Copy the `accessToken` from response.

**Step 3:** Click ?? **Authorize** button in Swagger
Paste: `Bearer eyJhbGci...`

**Step 4:** Test any protected endpoint! ??

---

## ?? Testing Individual Endpoints

### Public Endpoints (No Auth)

```bash
# Health Check
curl -k https://localhost:7257/health

# Send OTP
curl -k -X POST https://localhost:7257/api/auth/send-otp \
  -H "Content-Type: application/json" \
  -d '{"email":"demo@example.com","fullName":"Demo User"}'

# Verify OTP
curl -k -X POST https://localhost:7257/api/auth/verify-otp \
  -H "Content-Type: application/json" \
  -d '{"email":"demo@example.com","otp":"123456"}'
```

### Protected Endpoints (Requires JWT)

```bash
# Set your token
$token = "your-jwt-token-here"

# Get Profile
curl -k https://localhost:7257/api/user/profile \
  -H "Authorization: Bearer $token"

# Get Dashboard
curl -k https://localhost:7257/api/dashboard \
  -H "Authorization: Bearer $token"

# Get Vehicles
curl -k https://localhost:7257/api/vehicles \
  -H "Authorization: Bearer $token"

# Get Fuel Logs
curl -k https://localhost:7257/api/fuel \
  -H "Authorization: Bearer $token"

# Get Expenses
curl -k https://localhost:7257/api/expenses \
  -H "Authorization: Bearer $token"
```

---

## ?? Troubleshooting

### "Token expired" Error
Your token expires after 60 minutes. Get a new one:
1. POST `/api/auth/send-otp`
2. POST `/api/auth/verify-otp`
3. Use the new `accessToken`

### CORS Errors in Browser
1. Make sure API is running on `https://localhost:7257`
2. Restart the API after CORS changes
3. Serve your frontend from `http://localhost:4200`
4. Check browser console for specific CORS errors

### 401 Unauthorized
1. Make sure you're using a valid, non-expired token
2. Check that the `Authorization` header format is: `Bearer <token>`
3. Ensure you clicked "Authorize" in Swagger

---

## ?? Expected Test Results

When you run `test-all-endpoints.ps1`, you should see:

```
1. ? OTP: 123456
2. ? UserId: 1
   ? Name: Demo User
   ? Token: eyJhbGci...
3. ? Profile loaded: Demo User
4. ? Profile updated: Demo User Updated
5. ? Total Vehicles: 5
   ? Total Fuel Cost: ?50000
6. ? Vehicles count: 5
7. ? Vehicle added: Test Vehicle
8. ? Vehicle updated: Test Vehicle Updated
9. ? Fuel logs count: 22
10. ? Fuel log added: ?5005.0
11. ? Expenses count: 31
12. ? Expense added: ?5500.0
13. ? Vehicle deleted successfully
14. ? Health status: Healthy

? All API Endpoints Tested Successfully!
```

---

## ?? Next Steps

1. **Stop the debugger**
2. **Restart the API** (so CORS changes take effect)
3. **Run the test script:** `.\test-all-endpoints.ps1`
4. **Use the generated token** in your Angular app

Your API is now ready for frontend integration! ??
