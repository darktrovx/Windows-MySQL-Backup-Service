# Request Administrator Privileges
Function Request-Admin {
    If (-Not ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")) {
        Write-Host "Script needs to be run as Administrator. Restarting with elevated privileges..."
        Start-Process powershell.exe "-File $PSCommandPath" -Verb RunAs
        Exit
    }
}

Request-Admin

# Get the Directory of the Current Script
$ScriptDirectory = $PSScriptRoot

# Check if the Service Executable Exists in the Same Folder
$ServiceExe = Join-Path $ScriptDirectory "Setup.exe"  # Replace with your .exe name
if (-Not (Test-Path $ServiceExe)) {
    Write-Host "The service executable 'Setup.exe' was not found in the same directory as the script." -ForegroundColor Red
    Exit 1
}

# Define the Path to InstallUtil
$InstallUtilPath = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe"  # Update based on your system

# Check if InstallUtil Exists
if (-Not (Test-Path $InstallUtilPath)) {
    Write-Host "InstallUtil.exe not found at $InstallUtilPath. Ensure .NET Framework is installed." -ForegroundColor Red
    Exit 1
}

# Temporarily Add InstallUtil to PATH
$env:PATH += ";C:\Windows\Microsoft.NET\Framework64\v4.0.30319"

# Install the Service
Write-Host "Installing the Windows Service..."
try {
    & $InstallUtilPath $ServiceExe
    if ($LASTEXITCODE -eq 0) {
        Write-Host "Service installed successfully!" -ForegroundColor Green
    } else {
        Write-Host "Service installation failed with exit code $LASTEXITCODE." -ForegroundColor Red
    }
} catch {
    Write-Host "Error during installation: $_" -ForegroundColor Red
}

# Verify the Service Installation
$ServiceName = "MySQL Backup"  # Replace with the actual service name
if (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue) {
    Write-Host "Service '$ServiceName' is installed and available in the Services Manager (services.msc)." -ForegroundColor Green
} else {
    Write-Host "Service '$ServiceName' not found. Installation may have failed." -ForegroundColor Red
}
