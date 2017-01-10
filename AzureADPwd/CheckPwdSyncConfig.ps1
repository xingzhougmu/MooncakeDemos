# Check if AAD Sync Powershell is avaiable
if ((Get-Module -ListAvailable adsync) -eq $null)
{
    throw "AAD Sync Powershell Module cannot be found!"
}

Import-Module adsync

$adConnector = Get-ADSyncConnector | where {$_.ConnectorTypeName -eq "AD"}
$pwdSyncConfig = Get-ADSyncAADPasswordSyncConfiguration -SourceConnector $adConnector.Name

Write-Output("********************")
Write-Output ("Your Password Sync Configuration is.........")
Write-Output("SourceConnector: {0}" -f $pwdSyncConfig.SourceConnector )
Write-Output("TargetConnector: {0}" -f $pwdSyncConfig.TargetConnector )
Write-Output("Enabled: {0}" -f $pwdSyncConfig.Enabled )