$serviceName = "RFSerialBridge"
$installDir = "$Env:ProgramFiles\RFSerialBridge"
$exeName = "RFSerialBridge.exe"
$downloadUrl = "https://github.com/kumas-dev/rf-serial-bridge/releases/download/v1.0.0/RFSerialBridge.zip"
$zipPath = "$installDir\app.zip"

Write-Host "Install directory: $installDir"

# Check and remove existing service
$existingService = Get-Service -Name $serviceName -ErrorAction SilentlyContinue
if ($existingService) {
    Write-Host "Stopping and removing existing service..."

    if ($existingService.Status -eq 'Running') {
        Stop-Service -Name $serviceName -Force
    }

    sc.exe delete $serviceName | Out-Null

    # 🔁 Wait until the service is fully deleted
    Write-Host "Waiting for service to be fully removed..."
    do {
        Start-Sleep -Seconds 1
        $check = Get-Service -Name $serviceName -ErrorAction SilentlyContinue
    } while ($check)
}

# Remove previous installation if it exists
if (Test-Path $installDir) {
    Write-Host "Removing previous installation..."
    Remove-Item $installDir -Recurse -Force
}

# Create directory
New-Item -ItemType Directory -Path $installDir | Out-Null

# Download package
Write-Host "Downloading application package..."
Invoke-WebRequest -Uri $downloadUrl -OutFile $zipPath

# Extract package
Write-Host "Extracting package..."
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory($zipPath, $installDir)

# Clean up zip
Remove-Item $zipPath

$exePath = "$installDir\$exeName"

# Register and start service
Write-Host "Registering Windows Service..."
sc.exe create $serviceName binPath= "`"$exePath`"" start= auto

# Set restart policy for service failures
Write-Host "Setting restart policy..."
sc.exe failure $serviceName reset= 0 actions= restart/10000/restart/10000/restart/10000

sc.exe start $serviceName

Write-Host "Installation completed successfully."