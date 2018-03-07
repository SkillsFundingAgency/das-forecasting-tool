<#

.SYNOPSIS
Takes a pfx path and password and converts it to PKCS12 format and outputs a Base64 secure string of the PKCS12

.DESCRIPTION
Takes a pfx path and password and converts it to PKCS12 format and outputs a Base64 secure string of the PKCS12

.PARAMETER PfxFilePath
Filepath to pfx file - can be used alongside download secure file

.PARAMETER PfxPassword
pfx password in secure string format

.PARAMETER VstsVariableToSet
VSTS variable that should be set containing the Base64 secure string

#>
Param(
    [Parameter(Mandatory = $true)]
    [String[]]$PfxFilePath,
    [Parameter(Mandatory = $true)]
    [SecureString]$PfxPassword,
    [Parameter(Mandatory = $true)]
    [String]$VstsVariableToSet
)

$Flag = [System.Security.Cryptography.X509Certificates.X509KeyStorageFlags]::Exportable 
$Collection = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$Pkcs12ContentType = [System.Security.Cryptography.X509Certificates.X509ContentType]::Pkcs12 

try{
    $Collection.Import($PfxFilePath, $PfxPassword, $Flag) 
    $ClearBytes = $Collection.Export($Pkcs12ContentType) 
    $FileContentEncoded = [System.Convert]::ToBase64String($ClearBytes) 
    #$PfxBase64 = ConvertTo-SecureString -String $fileContentEncoded -AsPlainText –Force
    
    if($VstsVariableToSet){
	   Write-Host ("##vso[task.setvariable variable=$VstsVariableToSet;issecret=true;]$FileContentEncoded")
    }
}
catch{
    throw $_
}

return $PfxBase64