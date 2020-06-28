
Param(
    [Parameter(Mandatory=$true)]
    [string] $HostPoolName,

    [Parameter(Mandatory=$true)]
    [string] $UserName
)

# $HostPoolName=$args[0]
# $UserName=$args[1]

$TenantName = "FWS Tenant"

$creds = New-Object System.Management.Automation.PSCredential("7df5e7b4-cace-4177-818b-86b361a425b5", (ConvertTo-SecureString "0WyLPFlMpPvCpdDxhvVfgUBD8QrE4aqqARNhLOnV8e4=" -AsPlainText -Force))

Add-RdsAccount -DeploymentUrl "https://rdbroker.wvd.microsoft.com" -Credential $creds -ServicePrincipal -AadTenantId "96496f55-c758-4b60-b41f-eb2ec1cca295"

Add-RdsAppGroupUser -TenantName $TenantName -HostPoolName $HostPoolName -AppGroupName "Desktop Application Group" -UserPrincipalName $UserName