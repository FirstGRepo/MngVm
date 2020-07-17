
Import-Module "C:\Program Files\WindowsPowerShell\Modules\microsoft.rdinfra.rdpowershell"

$TenantName = "FWS Tenant"

$creds = New-Object System.Management.Automation.PSCredential("7df5e7b4-cace-4177-818b-86b361a425b5", (ConvertTo-SecureString "0WyLPFlMpPvCpdDxhvVfgUBD8QrE4aqqARNhLOnV8e4=" -AsPlainText -Force))

$connection = Add-RdsAccount -DeploymentUrl "https://rdbroker.wvd.microsoft.com" -Credential $creds -ServicePrincipal -AadTenantId "96496f55-c758-4b60-b41f-eb2ec1cca295"

#Get-RdsHostPool -TenantName $TenantName

$HostPoolName = @('WVDHP01','HP01','OG-Asia-HP03','OGUSWHP04','WVDHP06','PHP03')


$userSessionArray=New-Object System.Collections.ArrayList;


Foreach($hostpool in $HostPoolName)
{

$sysInfoList =  Get-RdsSessionHost -TenantName $TenantName  -HostPoolName $hostpool

    if($sysInfoList)
    {

        Foreach($sysInfo in $sysInfoList)
        {

        $runCommandObj = New-Object -TypeName PSObject
		
		Add-Member -InputObject $runCommandObj -MemberType NoteProperty -Name HostPoolName -Value $sysInfo.HostPoolName
        Add-Member -InputObject $runCommandObj -MemberType NoteProperty -Name VmName -Value ($sysInfo.SessionHostName -replace ".fws.local$", "")
        

        $userSessionArray.Add($runCommandObj) | out-null
        
        }
    }

}

#$userSessionArray

$postObj = (ConvertTo-Json $userSessionArray)

Write-Output $postObj