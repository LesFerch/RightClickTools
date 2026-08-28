# Create-DevCert.ps1
# Creates and installs a self-signed certificate for development code signing
# MUST be run as Administrator to install to LocalMachine stores (required for MSIX)

param(
    [string]$CertName = "RightClickTools-DevCert",
    [string]$Subject = "CN=RightClickTools Development Certificate",
    [int]$ValidYears = 5
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host "  RightClickTools Development Certificate Setup" -ForegroundColor Cyan
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host ""

# Check if running as administrator
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "[ERROR] This script MUST be run as Administrator!" -ForegroundColor Red
    Write-Host "        Right-click PowerShell and select 'Run as Administrator'" -ForegroundColor Yellow
    Write-Host ""
    exit 1
}

Write-Host "[OK] Running as Administrator" -ForegroundColor Green
Write-Host ""

# Configuration
$certPath = Join-Path $PSScriptRoot "DevCert.pfx"
$certPassword = "DevCert123_"
$securePassword = ConvertTo-SecureString -String $certPassword -Force -AsPlainText
$configPath = Join-Path $PSScriptRoot "DevCert.config"

# Step 1: Clean up ALL old certificates with this subject
Write-Host "[STEP 1/5] Cleaning up old certificates..." -ForegroundColor Yellow

$stores = @(
    "Cert:\CurrentUser\My",
    "Cert:\CurrentUser\Root",
    "Cert:\LocalMachine\My",
    "Cert:\LocalMachine\Root",
    "Cert:\LocalMachine\TrustedPeople"
)

$removedCount = 0
foreach ($store in $stores) {
    $oldCerts = Get-ChildItem -Path $store -ErrorAction SilentlyContinue | Where-Object { $_.Subject -eq $Subject }
    foreach ($cert in $oldCerts) {
        Write-Host "  Removing old cert from $store (Thumbprint: $($cert.Thumbprint))" -ForegroundColor Gray
        Remove-Item $cert.PSPath -Force -ErrorAction SilentlyContinue
        $removedCount++
    }
}

if ($removedCount -gt 0) {
    Write-Host "  [OK] Removed $removedCount old certificate(s)" -ForegroundColor Green
} else {
    Write-Host "  [OK] No old certificates found" -ForegroundColor Green
}

Write-Host ""

# Step 2: Create new certificate
Write-Host "[STEP 2/5] Creating new certificate..." -ForegroundColor Yellow

try {
    $cert = New-SelfSignedCertificate `
        -Type CodeSigningCert `
        -Subject $Subject `
        -FriendlyName $CertName `
        -KeyDescription "RightClickTools Development Code Signing" `
        -KeyFriendlyName $CertName `
        -CertStoreLocation Cert:\CurrentUser\My `
        -NotAfter (Get-Date).AddYears($ValidYears) `
        -KeyUsage DigitalSignature `
        -KeySpec Signature `
        -KeyLength 2048 `
        -HashAlgorithm SHA256 `
        -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3")

    Write-Host "  [OK] Certificate created" -ForegroundColor Green
    Write-Host "       Subject: $($cert.Subject)" -ForegroundColor Gray
    Write-Host "       Thumbprint: $($cert.Thumbprint)" -ForegroundColor Gray
    Write-Host "       Expires: $($cert.NotAfter.ToString('yyyy-MM-dd'))" -ForegroundColor Gray
}
catch {
    Write-Host "  [ERROR] Failed to create certificate: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Step 3: Export certificate
Write-Host "[STEP 3/5] Exporting certificate..." -ForegroundColor Yellow

try {
    # Export to PFX (for signing)
    Export-PfxCertificate -Cert $cert -FilePath $certPath -Password $securePassword -Force | Out-Null
    Write-Host "  [OK] Exported to PFX: $certPath" -ForegroundColor Green

    # Export to CER (for trust stores)
    $tempCer = Join-Path $env:TEMP "RightClickTools_DevCert.cer"
    Export-Certificate -Cert $cert -FilePath $tempCer -Force | Out-Null
    Write-Host "  [OK] Exported to CER (temp)" -ForegroundColor Green
}
catch {
    Write-Host "  [ERROR] Failed to export certificate: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Step 4: Install to all required stores
Write-Host "[STEP 4/5] Installing to certificate stores..." -ForegroundColor Yellow

try {
    # CurrentUser\Root
    Import-Certificate -FilePath $tempCer -CertStoreLocation Cert:\CurrentUser\Root | Out-Null
    Write-Host "  [OK] CurrentUser\Root" -ForegroundColor Green

    # LocalMachine\Root (CRITICAL FOR MSIX)
    Import-Certificate -FilePath $tempCer -CertStoreLocation Cert:\LocalMachine\Root | Out-Null
    Write-Host "  [OK] LocalMachine\Root (MSIX enabled!)" -ForegroundColor Green

    # LocalMachine\My
    Import-PfxCertificate -FilePath $certPath -CertStoreLocation Cert:\LocalMachine\My -Password $securePassword -Exportable | Out-Null
    Write-Host "  [OK] LocalMachine\My" -ForegroundColor Green

    # LocalMachine\TrustedPeople
    Import-Certificate -FilePath $tempCer -CertStoreLocation Cert:\LocalMachine\TrustedPeople | Out-Null
    Write-Host "  [OK] LocalMachine\TrustedPeople" -ForegroundColor Green

    # Clean up temp CER
    Remove-Item $tempCer -Force -ErrorAction SilentlyContinue
}
catch {
    Write-Host "  [ERROR] Failed to install to system stores: $_" -ForegroundColor Red
    Write-Host "          MSIX packages will NOT install!" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Step 5: Save configuration
Write-Host "[STEP 5/5] Saving configuration..." -ForegroundColor Yellow

@"
CERT_PATH=$certPath
CERT_PASSWORD=$certPassword
CERT_THUMBPRINT=$($cert.Thumbprint)
"@ | Out-File -FilePath $configPath -Encoding ASCII -Force

Write-Host "  [OK] Configuration saved to: $configPath" -ForegroundColor Green

Write-Host ""
Write-Host "========================================================================" -ForegroundColor Green
Write-Host "  SETUP COMPLETE" -ForegroundColor Green
Write-Host "========================================================================" -ForegroundColor Green
Write-Host ""
Write-Host "Certificate Details:" -ForegroundColor Cyan
Write-Host "  Subject:    $($cert.Subject)" -ForegroundColor Gray
Write-Host "  Thumbprint: $($cert.Thumbprint)" -ForegroundColor Gray
Write-Host "  Password:   $certPassword" -ForegroundColor Gray
Write-Host "  File:       $certPath" -ForegroundColor Gray
Write-Host "  Expires:    $($cert.NotAfter.ToString('yyyy-MM-dd'))" -ForegroundColor Gray
Write-Host ""
Write-Host "Certificate Stores:" -ForegroundColor Cyan
Write-Host "  CurrentUser\My              (for signing)" -ForegroundColor Gray
Write-Host "  CurrentUser\Root            (trusted)" -ForegroundColor Gray
Write-Host "  LocalMachine\My             (system signing)" -ForegroundColor Gray
Write-Host "  LocalMachine\Root           (MSIX requirement)" -ForegroundColor Gray
Write-Host "  LocalMachine\TrustedPeople  (additional trust)" -ForegroundColor Gray
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. Build project:  .\Build.ps1 -Configuration Debug" -ForegroundColor Yellow
Write-Host "  2. Install MSIX package and test" -ForegroundColor Yellow
Write-Host ""
Write-Host "========================================================================" -ForegroundColor Green
Write-Host ""

exit 0
