<#
    Author: Merl Creps
    Date: 2022-08-10
    Description: Installtion script for Toledo Fascia Daily Scans & LoadInfo
    HowTo: run install.ps1 as Local Admin
 #>
#>
$creds = Get-Credential

$serviceName = "ToledoFasicaDailyScans"

if ((Get-Service | Where-Object {$_.Name -eq $serviceName}).length -eq 1) {
    Write-Host "Stopping Service"
    Stop-Service -Name $serviceName

    Write-Host "Removing Service"
    sc.exe delete $serviceName
}

Write-Host "Building Service"
$Cred = New-Object System.Management.Automation.PSCredential ($creds.Username, $creds.Password)
$params = @{
  Name = $serviceName
  BinaryPathName = "E:\Applications\ToledoFascia\ToledoFasciaScans.exe"
  DependsOn = "NetLogon"
  DisplayName = "Toledo Fascia Daily Scans"
  StartupType = "Automatic"
  Description = "Toledo Fascia Daily Scans"
  Credential = $Cred
}
New-Service @params

Write-Host "Starting Service"
Start-Service -Name $serviceName