# CSIR Recruitment Portal - Auto Configuration Script
# Run as Administrator

$SiteName = "CSIRRecruitmentPortal"
$AppPoolName = "CSIRRecruitmentPool"
$Port = 443
$PhysicalPath = "C:\inetpub\wwwroot\csir-recruitment"
$UploadsPath = "$PhysicalPath\wwwroot\uploads"

Write-Host "Starting Configuration for $SiteName..." -ForegroundColor Cyan

# 1. Check for IIS
if (!(Get-WindowsFeature Web-Server)) {
    Write-Host "IIS is not installed. Please install IIS first." -ForegroundColor Red
    Exit
}

# 2. Create Application Pool
if (!(Test-Path "IIS:\AppPools\$AppPoolName")) {
    Write-Host "Creating Application Pool: $AppPoolName"
    New-WebAppPool -Name $AppPoolName
    Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name "managedRuntimeVersion" -Value "" # No Managed Code
    Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name "processModel.identityType" -Value "ApplicationPoolIdentity"
} else {
    Write-Host "Application Pool $AppPoolName already exists." -ForegroundColor Yellow
}

# 3. Create Website
if (!(Test-Path "IIS:\Sites\$SiteName")) {
    Write-Host "Creating Website: $SiteName"
    
    if (!(Test-Path $PhysicalPath)) {
        New-Item -ItemType Directory -Force -Path $PhysicalPath
    }
    
    New-Website -Name $SiteName -Port $Port -PhysicalPath $PhysicalPath -ApplicationPool $AppPoolName -Ssl
    Write-Host "Please configure SSL Binding manually or use 'netsh http add sslcert' if needed." -ForegroundColor Yellow
} else {
    Write-Host "Website $SiteName already exists." -ForegroundColor Yellow
}

# 4. Create Uploads Directory and Set Permissions
if (!(Test-Path $UploadsPath)) {
    New-Item -ItemType Directory -Force -Path $UploadsPath
    Write-Host "Created uploads directory at $UploadsPath"
}

# Grant Permissions to AppPool Identity
$Acl = Get-Acl $UploadsPath
$Ar = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS AppPool\$AppPoolName", "Modify", "ContainerInherit,ObjectInherit", "None", "Allow")
$Acl.SetAccessRule($Ar)
Set-Acl $UploadsPath $Acl
Write-Host "Granted Write permissions to IIS AppPool\$AppPoolName on uploads folder." -ForegroundColor Green

Write-Host "Configuration Complete! Deploy published files to $PhysicalPath" -ForegroundColor Green
