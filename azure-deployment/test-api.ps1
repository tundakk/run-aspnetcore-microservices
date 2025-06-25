# EmailIntelligence API Test Script
# This script validates the deployed EmailIntelligence API functionality

param(
    [Parameter(Mandatory=$true)]
    [string]$ApiBaseUrl,
    
    [Parameter()]
    [string]$TestUserId = "azure-test-user"
)

function Write-TestResult {
    param(
        [string]$TestName,
        [bool]$Success,
        [string]$Details = ""
    )
    
    $status = if ($Success) { "✅ PASS" } else { "❌ FAIL" }
    Write-Host "$status - $TestName" -ForegroundColor $(if ($Success) { "Green" } else { "Red" })
    if ($Details) {
        Write-Host "    $Details" -ForegroundColor Gray
    }
}

function Test-ApiEndpoint {
    param(
        [string]$Url,
        [string]$Method = "GET",
        [hashtable]$Headers = @{},
        [string]$Body = $null
    )
    
    try {
        $params = @{
            Uri = $Url
            Method = $Method
            Headers = $Headers
            TimeoutSec = 30
        }
        
        if ($Body) {
            $params.Body = $Body
            $params.ContentType = "application/json"
        }
        
        $response = Invoke-RestMethod @params
        return @{ Success = $true; Data = $response; StatusCode = 200 }
    }
    catch {
        return @{ Success = $false; Error = $_.Exception.Message; StatusCode = $_.Exception.Response.StatusCode.Value__ }
    }
}

Write-Host "🚀 Starting EmailIntelligence API Validation" -ForegroundColor Cyan
Write-Host "API Base URL: $ApiBaseUrl" -ForegroundColor Yellow
Write-Host "Test User ID: $TestUserId" -ForegroundColor Yellow
Write-Host ("-" * 50)

# Test 1: Health Check
Write-Host "🏥 Testing Health Endpoint..."
$healthResult = Test-ApiEndpoint -Url "$ApiBaseUrl/health"
Write-TestResult -TestName "Health Check" -Success $healthResult.Success -Details "Response: $($healthResult.Data.status)"

# Test 2: API Documentation (Swagger)
Write-Host "`n📚 Testing API Documentation..."
$swaggerResult = Test-ApiEndpoint -Url "$ApiBaseUrl/swagger/v1/swagger.json"
Write-TestResult -TestName "Swagger Documentation" -Success $swaggerResult.Success

# Test 3: Get User Tone Profiles
Write-Host "`n👤 Testing User Tone Profiles..."
$profilesResult = Test-ApiEndpoint -Url "$ApiBaseUrl/api/emailintelligence/users/$TestUserId/tone-profiles"
Write-TestResult -TestName "Get User Tone Profiles" -Success $profilesResult.Success -Details "Profiles found: $($profilesResult.Data.Count)"

# Test 4: Get Email Drafts
Write-Host "`n📧 Testing Email Drafts..."
$draftsResult = Test-ApiEndpoint -Url "$ApiBaseUrl/api/emailintelligence/users/$TestUserId/drafts"
Write-TestResult -TestName "Get Email Drafts" -Success $draftsResult.Success -Details "Drafts found: $($draftsResult.Data.Count)"

# Test 5: Get Processed Emails
Write-Host "`n📊 Testing Processed Emails..."
$processedResult = Test-ApiEndpoint -Url "$ApiBaseUrl/api/emailintelligence/users/$TestUserId/processed-emails"
Write-TestResult -TestName "Get Processed Emails" -Success $processedResult.Success -Details "Processed emails found: $($processedResult.Data.Count)"

# Test 6: Create Email Draft
Write-Host "`n✍️ Testing Email Draft Creation..."
$draftData = @{
    subject = "API Test Draft"
    content = "This is a test email draft created via API validation."
    toneStyle = "Professional"
} | ConvertTo-Json

$createDraftResult = Test-ApiEndpoint -Url "$ApiBaseUrl/api/emailintelligence/users/$TestUserId/drafts" -Method "POST" -Body $draftData
Write-TestResult -TestName "Create Email Draft" -Success $createDraftResult.Success -Details "Draft ID: $($createDraftResult.Data.id)"

# Test 7: Analyze Email (if endpoint exists)
Write-Host "`n🔍 Testing Email Analysis..."
$analysisData = @{
    content = "Hello, I hope this email finds you well. I wanted to follow up on our previous conversation."
    targetTone = "Professional"
} | ConvertTo-Json

$analysisResult = Test-ApiEndpoint -Url "$ApiBaseUrl/api/emailintelligence/analyze" -Method "POST" -Body $analysisData
Write-TestResult -TestName "Email Analysis" -Success $analysisResult.Success -Details "Analysis completed: $($analysisResult.Data.toneScore)"

# Test 8: Database Connectivity (indirect test)
Write-Host "`n💾 Testing Database Connectivity..."
$dbTestPassed = $profilesResult.Success -and $draftsResult.Success -and $processedResult.Success
Write-TestResult -TestName "Database Connectivity" -Success $dbTestPassed -Details "All database operations successful"

# Summary
Write-Host "`n" + ("=" * 50)
Write-Host "📋 Test Summary" -ForegroundColor Cyan

$tests = @(
    @{ Name = "Health Check"; Result = $healthResult.Success },
    @{ Name = "Swagger Documentation"; Result = $swaggerResult.Success },
    @{ Name = "User Tone Profiles"; Result = $profilesResult.Success },
    @{ Name = "Email Drafts"; Result = $draftsResult.Success },
    @{ Name = "Processed Emails"; Result = $processedResult.Success },
    @{ Name = "Create Email Draft"; Result = $createDraftResult.Success },
    @{ Name = "Email Analysis"; Result = $analysisResult.Success },
    @{ Name = "Database Connectivity"; Result = $dbTestPassed }
)

$passedTests = ($tests | Where-Object { $_.Result }).Count
$totalTests = $tests.Count

Write-Host "Passed: $passedTests/$totalTests tests" -ForegroundColor $(if ($passedTests -eq $totalTests) { "Green" } else { "Yellow" })

if ($passedTests -eq $totalTests) {
    Write-Host "🎉 All tests passed! The EmailIntelligence API is working correctly." -ForegroundColor Green
} elseif ($passedTests -gt $totalTests / 2) {
    Write-Host "⚠️ Most tests passed, but some issues were found. Check the details above." -ForegroundColor Yellow
} else {
    Write-Host "🚨 Multiple tests failed. Please check the API deployment and configuration." -ForegroundColor Red
}

Write-Host "`n🔗 Useful Links:"
Write-Host "• API Health: $ApiBaseUrl/health"
Write-Host "• Swagger UI: $ApiBaseUrl/swagger"
Write-Host "• API Documentation: $ApiBaseUrl/swagger/v1/swagger.json"

return $passedTests -eq $totalTests
