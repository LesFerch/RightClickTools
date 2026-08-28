# Build.ps1
# RightClickTools Automated Build Script (PowerShell Version)
#
# This script automates the build and signing process for both Debug and Release configurations
# 
# DEBUG MODE (Development):
#   - Uses self-signed certificate for fast iteration
#   - Publisher: CN=RightClickTools Development Certificate
#   - Run .\RightClickToolsHandler\Create-DevCert.ps1 first (one-time setup)
#   - Usage: .\Build.ps1 -Configuration Debug
#
# RELEASE MODE (Production):
#   - Uses USB key for signing
#   - Publisher: CN="Open Source Developer, Leslie Ferch"
#   - Usage: .\Build.ps1 -Configuration Release

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipSigning = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$Clean = $true
)

$ErrorActionPreference = "Stop"

# Configuration
$ProjectDir = $PSScriptRoot
$HandlerDir = Join-Path $PSScriptRoot "RightClickToolsHandler"
$Solution = Join-Path $ProjectDir "RightClickTools.sln"
$CsProj = Join-Path $ProjectDir "RightClickTools.csproj"
$OutputDir = Join-Path $ProjectDir "bin\$Configuration"
$ExeFile = Join-Path $OutputDir "RightClickTools.exe"
$MSIXOutputDir = Join-Path $ProjectDir "MSIX_Package"
$ManifestFile = Join-Path $HandlerDir "Package.appxmanifest"

# Certificate configuration
$DevCertConfig = Join-Path $HandlerDir "DevCert.config"
$DevCertPublisher = "CN=RightClickTools Development Certificate"
$ReleaseCertPublisher = 'CN="Open Source Developer, Leslie Ferch"'

Write-Host ""
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host "  RightClickTools Build Script" -ForegroundColor Cyan
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host ""

# Determine signing mode
if ($Configuration -eq "Debug") {
    $SignMode = "DEV"
    Write-Host "[MODE] DEVELOPMENT - Using self-signed certificate" -ForegroundColor Green
    Write-Host "       Configuration: Debug" -ForegroundColor Gray
} else {
    $SignMode = "RELEASE"
    Write-Host "[MODE] RELEASE - Using USB key certificate" -ForegroundColor Yellow
    Write-Host "       Configuration: Release" -ForegroundColor Gray
}
Write-Host ""

# =========================================================================
# Step 0: Patch Package.appxmanifest with correct Publisher
# =========================================================================
Write-Host "[STEP 0/6] Patching Package.appxmanifest..." -ForegroundColor Yellow

if (Test-Path $ManifestFile) {
    try {
        # Read manifest as XML
        [xml]$manifest = Get-Content $ManifestFile
        
        # Get current publisher
        $currentPublisher = $manifest.Package.Identity.Publisher
        
        # Determine target publisher based on configuration
        if ($Configuration -eq "Debug") {
            $targetPublisher = $DevCertPublisher
        } else {
            $targetPublisher = $ReleaseCertPublisher
        }
        
        # Update if different
        if ($currentPublisher -ne $targetPublisher) {
            Write-Host "  Updating Publisher:" -ForegroundColor Gray
            Write-Host "    From: $currentPublisher" -ForegroundColor Gray
            Write-Host "    To:   $targetPublisher" -ForegroundColor Gray
            
            $manifest.Package.Identity.Publisher = $targetPublisher
            $manifest.Save($ManifestFile)
            
            Write-Host "  [OK] Manifest updated" -ForegroundColor Green
        } else {
            Write-Host "  [OK] Manifest already correct" -ForegroundColor Green
        }
    }
    catch {
        Write-Host "  [ERROR] Failed to patch manifest: $_" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "  [WARN] Package.appxmanifest not found: $ManifestFile" -ForegroundColor Yellow
    Write-Host "         Continuing without manifest update" -ForegroundColor Gray
}

Write-Host ""

# =========================================================================
# Step 1: Check Prerequisites
# =========================================================================
Write-Host "[STEP 1/6] Checking prerequisites..." -ForegroundColor Yellow

# Check for MSBuild
$MSBuild = $null
$versionFolders = @("18", "2026", "2022", "2019", "2017", "17", "16", "15")
$editions = @("Community", "Professional", "Enterprise", "Preview")

foreach ($version in $versionFolders) {
    foreach ($edition in $editions) {
        $testPath = "${env:ProgramFiles}\Microsoft Visual Studio\$version\$edition\MSBuild\Current\Bin\MSBuild.exe"
        if (Test-Path $testPath) {
            $MSBuild = $testPath
            break
        }
    }
    if ($MSBuild) { break }
}

if (-not $MSBuild) {
    Write-Host "  [ERROR] MSBuild not found" -ForegroundColor Red
    Write-Host "          Please install Visual Studio 2017 or later" -ForegroundColor Yellow
    exit 1
}

Write-Host "  [OK] MSBuild found: $MSBuild" -ForegroundColor Green

# Check for signtool
$SignTool = $null

# Try to find x64 version first
Get-ChildItem "${env:ProgramFiles(x86)}\Windows Kits\10\bin" -Recurse -Filter "signtool.exe" -ErrorAction SilentlyContinue | ForEach-Object {
    if ($_.FullName -match "\\x64\\") {
        $SignTool = $_.FullName
    }
}

# Fall back to any non-ARM version
if (-not $SignTool) {
    Get-ChildItem "${env:ProgramFiles(x86)}\Windows Kits\10\bin" -Recurse -Filter "signtool.exe" -ErrorAction SilentlyContinue | ForEach-Object {
        if ($_.FullName -notmatch "\\arm\\") {
            $SignTool = $_.FullName
        }
    }
}

if (-not $SignTool) {
    Write-Host "  [ERROR] signtool.exe not found" -ForegroundColor Red
    Write-Host "          Please install Windows SDK" -ForegroundColor Yellow
    exit 1
}

Write-Host "  [OK] signtool found: $SignTool" -ForegroundColor Green

# Check certificate configuration for DEV mode
if ($SignMode -eq "DEV" -and -not $SkipSigning) {
    if (-not (Test-Path $DevCertConfig)) {
        Write-Host "  [ERROR] DevCert.config not found!" -ForegroundColor Red
        Write-Host "          Please run Create-DevCert.ps1 first to set up development certificate" -ForegroundColor Yellow
        Write-Host "" -ForegroundColor Yellow
        Write-Host "          Run this command in PowerShell as Administrator:" -ForegroundColor Yellow
        Write-Host "            .\RightClickToolsHandler\Create-DevCert.ps1" -ForegroundColor Gray
        exit 1
    }
    
    # Load certificate configuration
    $certConfig = @{}
    Get-Content $DevCertConfig | ForEach-Object {
        if ($_ -match "^([^=]+)=(.+)$") {
            $certConfig[$matches[1]] = $matches[2]
        }
    }
    
    $CertPath = $certConfig["CERT_PATH"]
    $CertPassword = $certConfig["CERT_PASSWORD"]
    $CertThumbprint = $certConfig["CERT_THUMBPRINT"]
    
    if (-not (Test-Path $CertPath)) {
        Write-Host "  [ERROR] Certificate file not found: $CertPath" -ForegroundColor Red
        Write-Host "          Please run Create-DevCert.ps1 again" -ForegroundColor Yellow
        exit 1
    }
    
    Write-Host "  [OK] Development certificate configured" -ForegroundColor Green
}

Write-Host ""

# =========================================================================
# Step 2: Clean Previous Build
# =========================================================================
if ($Clean) {
    Write-Host "[STEP 2/6] Cleaning previous build..." -ForegroundColor Yellow

    if (Test-Path $OutputDir) {
        # Delete specific files and folders but preserve AppParts
        Get-ChildItem $OutputDir -File | Remove-Item -Force -ErrorAction SilentlyContinue
        Get-ChildItem $OutputDir -Directory | Where-Object { $_.Name -ne "AppParts" } | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    }

    if (Test-Path $MSIXOutputDir) {
        # Empty the folder contents instead of deleting the folder itself
        Get-ChildItem $MSIXOutputDir -Recurse | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    } else {
        # Create the folder if it doesn't exist
        New-Item -ItemType Directory -Path $MSIXOutputDir | Out-Null
    }

    Write-Host "  [OK] Clean complete (AppParts preserved)" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "[STEP 2/6] Skipping clean (incremental build)" -ForegroundColor Yellow
    Write-Host ""
}

# =========================================================================
# Step 3: Build Solution
# =========================================================================
Write-Host "[STEP 3/6] Building solution..." -ForegroundColor Yellow
Write-Host ""

# Try different platform configurations
$buildArgs = @(
    $Solution,
    "/t:Rebuild",
    "/p:Configuration=$Configuration",
    "/v:minimal",
    "/nologo"
)

& $MSBuild $buildArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "  [ERROR] Build failed!" -ForegroundColor Red
    Write-Host "  Try building from Visual Studio to diagnose the issue" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "  [OK] Build successful" -ForegroundColor Green
Write-Host ""

# Build Setup.exe
Write-Host "  Building Setup.exe..." -ForegroundColor Cyan

$SetupProj = Join-Path $ProjectDir "Setup.csproj"
$SetupBuildArgs = @(
    $SetupProj,
    "/t:Rebuild",
    "/p:Configuration=$Configuration",
    "/v:minimal",
    "/nologo"
)

& $MSBuild $SetupBuildArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "  [ERROR] Setup.exe build failed!" -ForegroundColor Red
    exit 1
}

$SetupExeFile = Join-Path $OutputDir "Setup.exe"
if (Test-Path $SetupExeFile) {
    Write-Host "  [OK] Setup.exe built successfully" -ForegroundColor Green
} else {
    Write-Host "  [ERROR] Setup.exe not found at: $SetupExeFile" -ForegroundColor Red
    exit 1
}

Write-Host ""

# =========================================================================
# Step 4: Sign Executable
# =========================================================================
Write-Host "[STEP 4/6] Signing executable..." -ForegroundColor Yellow
Write-Host ""

if (-not (Test-Path $ExeFile)) {
    Write-Host "  [ERROR] Output file not found: $ExeFile" -ForegroundColor Red
    exit 1
}

$SetupExeFile = Join-Path $OutputDir "Setup.exe"
if (-not (Test-Path $SetupExeFile)) {
    Write-Host "  [ERROR] Setup.exe not found: $SetupExeFile" -ForegroundColor Red
    exit 1
}

if ($SkipSigning) {
    Write-Host "  [SKIP] Signing skipped (SkipSigning flag set)" -ForegroundColor Yellow
} elseif ($SignMode -eq "DEV") {
    # Sign with development certificate
    Write-Host "  Using development certificate..." -ForegroundColor Cyan
    Write-Host ""

    # Sign RightClickTools.exe
    Write-Host "  Signing RightClickTools.exe..." -ForegroundColor Gray
    $signArgs = @(
        "sign",
        "/f", $CertPath,
        "/p", $CertPassword,
        "/fd", "SHA256",
        "/tr", "http://timestamp.digicert.com",
        "/td", "SHA256",
        "/v",
        $ExeFile
    )

    & $SignTool $signArgs

    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "  [ERROR] Signing RightClickTools.exe failed" -ForegroundColor Red
        Write-Host "" -ForegroundColor Red
        Write-Host "  Troubleshooting:" -ForegroundColor Yellow
        Write-Host "    1. Verify certificate exists: $CertPath" -ForegroundColor Gray
        Write-Host "    2. Try running Create-DevCert.ps1 again" -ForegroundColor Gray
        Write-Host "    3. Check that certificate is valid and not expired" -ForegroundColor Gray
        exit 1
    }

    Write-Host "  [OK] RightClickTools.exe signed" -ForegroundColor Green
    Write-Host ""

    # Sign Setup.exe
    Write-Host "  Signing Setup.exe..." -ForegroundColor Gray
    $signSetupArgs = @(
        "sign",
        "/f", $CertPath,
        "/p", $CertPassword,
        "/fd", "SHA256",
        "/tr", "http://timestamp.digicert.com",
        "/td", "SHA256",
        "/v",
        $SetupExeFile
    )

    & $SignTool $signSetupArgs

    if ($LASTEXITCODE -ne 0) {
        Write-Host ""
        Write-Host "  [ERROR] Signing Setup.exe failed" -ForegroundColor Red
        exit 1
    }

    Write-Host "  [OK] Setup.exe signed" -ForegroundColor Green
    Write-Host ""
    Write-Host "  [OK] Both executables signed with development certificate" -ForegroundColor Green
} else {
    # Sign with USB key (manual process)
    Write-Host "  MANUAL SIGNING REQUIRED" -ForegroundColor Yellow
    Write-Host "  =====================================================" -ForegroundColor Yellow
    Write-Host "  Please sign the following files with your USB key:" -ForegroundColor Yellow
    Write-Host "  1. $ExeFile" -ForegroundColor Cyan
    Write-Host "  2. $SetupExeFile" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "  Recommended signtool command:" -ForegroundColor Gray
    Write-Host "  signtool sign /sha1 YOUR_CERT_THUMBPRINT /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 `"$ExeFile`" `"$SetupExeFile`"" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  =====================================================" -ForegroundColor Yellow
    Write-Host ""
    Read-Host "  Press Enter after signing is complete"

    # Verify signature exists on both files
    $verifyArgs = @("verify", "/pa", "/v", $ExeFile)
    & $SignTool $verifyArgs > $null 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Host "  [WARN] RightClickTools.exe does not appear to be signed" -ForegroundColor Yellow
        $continue = Read-Host "  Continue anyway? (y/n)"
        if ($continue -ne "y") {
            exit 1
        }
    } else {
        Write-Host "  [OK] RightClickTools.exe signature verified" -ForegroundColor Green
    }

    $verifySetupArgs = @("verify", "/pa", "/v", $SetupExeFile)
    & $SignTool $verifySetupArgs > $null 2>&1

    if ($LASTEXITCODE -ne 0) {
        Write-Host "  [WARN] Setup.exe does not appear to be signed" -ForegroundColor Yellow
        $continue = Read-Host "  Continue anyway? (y/n)"
        if ($continue -ne "y") {
            exit 1
        }
    } else {
        Write-Host "  [OK] Setup.exe signature verified" -ForegroundColor Green
    }
}

Write-Host ""

# =========================================================================
# Step 5: Create MSIX Package
# =========================================================================
Write-Host "[STEP 5/6] Creating MSIX package..." -ForegroundColor Yellow

# Create MSIX output directory
if (-not (Test-Path $MSIXOutputDir)) {
    New-Item -ItemType Directory -Path $MSIXOutputDir | Out-Null
}

# Look for makeappx.exe
$MakeAppx = $null
Get-ChildItem "${env:ProgramFiles(x86)}\Windows Kits\10\bin" -Recurse -Filter "makeappx.exe" -ErrorAction SilentlyContinue | ForEach-Object {
    if ($_.FullName -match "\\x64\\") {
        $MakeAppx = $_.FullName
    }
}

if (-not $MakeAppx) {
    Write-Host "  [WARN] makeappx.exe not found, skipping MSIX package creation" -ForegroundColor Yellow
    Write-Host "         Install Windows SDK to enable MSIX packaging" -ForegroundColor Gray
} else {
    Write-Host "  [OK] makeappx found: $MakeAppx" -ForegroundColor Green

    # Create temporary MSIX staging directory
    $msixStagingDir = Join-Path $ProjectDir "MSIX_Staging_Temp"
    if (Test-Path $msixStagingDir) {
        Remove-Item $msixStagingDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $msixStagingDir | Out-Null

    # Copy manifest with the required name (AppxManifest.xml)
    Copy-Item $ManifestFile (Join-Path $msixStagingDir "AppxManifest.xml")

    # Copy other required files from HandlerDir
    $excludePatterns = @("*.ps1", "*.bat", "*.config", "*.pfx", "*.md", "*.cpp", "*.h", "*.vcxproj*", "*.def", "*.obj", "*.pdb", "*.lib", "*.exp", "*.iobj", "*.ipdb")
    Get-ChildItem $HandlerDir -File | Where-Object {
        $file = $_
        $shouldExclude = $false
        foreach ($pattern in $excludePatterns) {
            if ($file.Name -like $pattern) {
                $shouldExclude = $true
                break
            }
        }
        -not $shouldExclude
    } | ForEach-Object {
        Copy-Item $_.FullName $msixStagingDir
    }

    # Copy Images folder if it exists
    $imagesSource = Join-Path $ProjectDir "Images"
    if (Test-Path $imagesSource) {
        $imagesDest = Join-Path $msixStagingDir "Images"
        Copy-Item $imagesSource $imagesDest -Recurse
        Write-Host "  [OK] Images folder copied to MSIX package" -ForegroundColor Green
    } else {
        Write-Host "  [WARNING] Images folder not found at: $imagesSource" -ForegroundColor Yellow
    }

    # Copy the built executable and DLLs from output directory
    if (Test-Path $ExeFile) {
        Copy-Item $ExeFile $msixStagingDir
    }

    # Copy the DLL
    $dllFile = Join-Path $ProjectDir "x64\$Configuration\RightClickToolsHandler.dll"
    if (Test-Path $dllFile) {
        Copy-Item $dllFile $msixStagingDir
    }

    # Copy minimal AppParts files for MSIX (hybrid installation model)
    # Full AppParts will be installed to C:\Program Files by InnoSetup
    # MSIX only needs Language.ini and Icons folder for COM handler
    $appPartsSource = Join-Path $OutputDir "AppParts"
    if (Test-Path $appPartsSource) {
        $appPartsDest = Join-Path $msixStagingDir "AppParts"
        New-Item -ItemType Directory -Path $appPartsDest -Force | Out-Null

        # Copy Language.ini
        $languageIni = Join-Path $appPartsSource "Language.ini"
        if (Test-Path $languageIni) {
            Copy-Item $languageIni $appPartsDest
            Write-Host "  [OK] Language.ini copied to MSIX package" -ForegroundColor Green
        } else {
            Write-Host "  [WARNING] Language.ini not found at: $languageIni" -ForegroundColor Yellow
        }

        # Copy Icons folder (excluding subfolders)
        $iconsSource = Join-Path $appPartsSource "Icons"
        if (Test-Path $iconsSource) {
            $iconsDest = Join-Path $appPartsDest "Icons"
            New-Item -ItemType Directory -Path $iconsDest -Force | Out-Null

            # Copy only files directly in Icons folder, not subfolders
            Get-ChildItem $iconsSource -File | ForEach-Object {
                Copy-Item $_.FullName $iconsDest
            }

            $iconCount = (Get-ChildItem $iconsDest -File).Count
            Write-Host "  [OK] Icons folder copied to MSIX package ($iconCount files, no subfolders)" -ForegroundColor Green
        } else {
            Write-Host "  [WARNING] Icons folder not found at: $iconsSource" -ForegroundColor Yellow
        }

        Write-Host "  [INFO] MSIX package uses minimal AppParts (hybrid installation mode)" -ForegroundColor Cyan
        Write-Host "         Full AppParts installation will be handled by InnoSetup" -ForegroundColor Gray
    } else {
        Write-Host "  [WARNING] AppParts folder not found at: $appPartsSource" -ForegroundColor Yellow
    }

    # Build MSIX package
    # $msixFile = Join-Path $MSIXOutputDir "RightClickTools_$Configuration.msix"
    $msixFile = Join-Path $MSIXOutputDir "RightClickTools.msix"

    $makeappxArgs = @(
        "pack",
        "/d", $msixStagingDir,
        "/p", $msixFile,
        "/nv"
    )

    Write-Host "  Creating MSIX package: $msixFile" -ForegroundColor Cyan
    $makeappxOutput = & $MakeAppx $makeappxArgs 2>&1

    # Clean up staging directory
    Remove-Item $msixStagingDir -Recurse -Force -ErrorAction SilentlyContinue

    if ($LASTEXITCODE -ne 0) {
        Write-Host "  [ERROR] MSIX package creation failed" -ForegroundColor Red
        Write-Host $makeappxOutput -ForegroundColor Red
    } else {
        Write-Host "  [OK] MSIX package created" -ForegroundColor Green
        
        # Sign MSIX package
        if (-not $SkipSigning) {
            Write-Host "  Signing MSIX package..." -ForegroundColor Cyan
            
            if ($SignMode -eq "DEV") {
                $signMsixArgs = @(
                    "sign",
                    "/f", $CertPath,
                    "/p", $CertPassword,
                    "/fd", "SHA256",
                    "/tr", "http://timestamp.digicert.com",
                    "/td", "SHA256",
                    $msixFile
                )
                
                & $SignTool $signMsixArgs
                
                if ($LASTEXITCODE -ne 0) {
                    Write-Host "  [ERROR] MSIX signing failed" -ForegroundColor Red
                } else {
                    Write-Host "  [OK] MSIX package signed" -ForegroundColor Green
                }
            } else {
                Write-Host "  [INFO] Please sign the MSIX package manually:" -ForegroundColor Yellow
                Write-Host "         $msixFile" -ForegroundColor Cyan
            }
        }
    }
}

Write-Host ""

# =========================================================================
# Step 5.5: Uninstall Previous MSIX Package (Debug mode only)
# =========================================================================
if ($SignMode -eq "DEV" -and $MakeAppx) {
    Write-Host "[STEP 5.5/6] Uninstalling previous MSIX package..." -ForegroundColor Yellow

    $existingPackage = Get-AppxPackage | Where-Object { $_.Name -like "*RightClickTools*" }

    if ($existingPackage) {
        Write-Host "  Found existing package: $($existingPackage.Name) v$($existingPackage.Version)" -ForegroundColor Gray
        try {
            Get-AppxPackage *RightClickTools* | Remove-AppxPackage
            Write-Host "  [OK] Previous package uninstalled" -ForegroundColor Green
            Write-Host "       System ready for fresh installation" -ForegroundColor Gray
        }
        catch {
            Write-Host "  [WARN] Could not uninstall previous package: $_" -ForegroundColor Yellow
            Write-Host "         You may need to uninstall manually" -ForegroundColor Gray
        }
    } else {
        Write-Host "  [OK] No previous package found" -ForegroundColor Green
    }

    Write-Host ""
}

# =========================================================================
# Step 6: Verify Build Outputs
# =========================================================================
Write-Host "[STEP 6/6] Verifying build outputs..." -ForegroundColor Yellow

$allGood = $true

# Check RightClickTools.exe
if (Test-Path $ExeFile) {
    $exeSize = (Get-Item $ExeFile).Length
    Write-Host "  [OK] RightClickTools.exe" -ForegroundColor Green
    Write-Host "       Size: $exeSize bytes" -ForegroundColor Gray

    # Verify signature
    if (-not $SkipSigning) {
        try {
            $sig = Get-AuthenticodeSignature $ExeFile
            if ($sig.Status -eq "Valid") {
                Write-Host "       Signature: Valid" -ForegroundColor Gray
            } else {
                Write-Host "       Signature: $($sig.Status)" -ForegroundColor Yellow
            }
        }
        catch {
            Write-Host "       Signature: Unknown" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "  [FAIL] RightClickTools.exe not found" -ForegroundColor Red
    $allGood = $false
}

Write-Host ""

# Check Setup.exe
$SetupExeFile = Join-Path $OutputDir "Setup.exe"
if (Test-Path $SetupExeFile) {
    $setupExeSize = (Get-Item $SetupExeFile).Length
    Write-Host "  [OK] Setup.exe" -ForegroundColor Green
    Write-Host "       Size: $setupExeSize bytes" -ForegroundColor Gray

    # Verify signature
    if (-not $SkipSigning) {
        try {
            $setupSig = Get-AuthenticodeSignature $SetupExeFile
            if ($setupSig.Status -eq "Valid") {
                Write-Host "       Signature: Valid" -ForegroundColor Gray
            } else {
                Write-Host "       Signature: $($setupSig.Status)" -ForegroundColor Yellow
            }
        }
        catch {
            Write-Host "       Signature: Unknown" -ForegroundColor Yellow
        }
    }
} else {
    Write-Host "  [FAIL] Setup.exe not found" -ForegroundColor Red
    $allGood = $false
}

Write-Host ""

# Check MSIX
if ($MakeAppx) {
    $msixFile = Join-Path $MSIXOutputDir "RightClickTools_$Configuration.msix"
    if (Test-Path $msixFile) {
        $msixSize = (Get-Item $msixFile).Length
        Write-Host "  [OK] MSIX Package" -ForegroundColor Green
        Write-Host "       Location: $msixFile" -ForegroundColor Gray
        Write-Host "       Size: $msixSize bytes" -ForegroundColor Gray
        
        # Verify MSIX signature
        if (-not $SkipSigning -and $SignMode -eq "DEV") {
            try {
                $msixSig = Get-AuthenticodeSignature $msixFile
                if ($msixSig.Status -eq "Valid") {
                    Write-Host "       Signature: Valid" -ForegroundColor Gray
                } else {
                    Write-Host "       Signature: $($msixSig.Status)" -ForegroundColor Yellow
                }
            }
            catch {
                Write-Host "       Signature: Unknown" -ForegroundColor Yellow
            }
        }
    }
}

Write-Host ""

# =========================================================================
# Summary
# =========================================================================
if ($allGood) {
    Write-Host "========================================================================" -ForegroundColor Green
    Write-Host "  BUILD SUCCESSFUL" -ForegroundColor Green
    Write-Host "========================================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "  Output: $ExeFile" -ForegroundColor Cyan
    Write-Host "  Mode:   $SignMode ($Configuration)" -ForegroundColor Cyan
    
    if ($MakeAppx -and (Test-Path (Join-Path $MSIXOutputDir "RightClickTools_$Configuration.msix"))) {
        Write-Host "  MSIX:   $(Join-Path $MSIXOutputDir "RightClickTools_$Configuration.msix")" -ForegroundColor Cyan
    }
    
    Write-Host ""
    
    if ($SignMode -eq "DEV") {
        Write-Host "  NOTE: This build is signed with a development certificate" -ForegroundColor Yellow
        Write-Host "        For production release, use: .\Build.ps1 -Configuration Release" -ForegroundColor Yellow
    } else {
        Write-Host "  NOTE: This is a production release build" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "  To test: $ExeFile" -ForegroundColor Gray
    Write-Host "========================================================================" -ForegroundColor Green
    Write-Host ""
    
    exit 0
} else {
    Write-Host "========================================================================" -ForegroundColor Red
    Write-Host "  BUILD FAILED" -ForegroundColor Red
    Write-Host "========================================================================" -ForegroundColor Red
    Write-Host ""
    exit 1
}
