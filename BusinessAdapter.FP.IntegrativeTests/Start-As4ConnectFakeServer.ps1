[CmdletBinding()]
Param(
)
$user = $env:usr
$password = $env:pwd
$repo = "as4-connect-business-api-fakeserver-releases"

function Set-CertificatePrivateKeyPermission {
    param (
        [Parameter(Mandatory = $true)][string]$CertStorePath
    ) 

    Import-Module WebAdministration
    $certificate = Get-ChildItem $CertStorePath | Where-Object Subject -Like "*dnQualifier=AS4*"

    if ($null -eq $certificate) {
        Write-Error "Kein Zertifikat mit 'dnQualifier=AS4' gefunden in $($CertStorePath)."
    }
    else {
        $privateKey = [System.Security.Cryptography.X509Certificates.ECDsaCertificateExtensions]::GetECDsaPrivateKey($certificate)
        if (-not $privateKey) {
            $privateKey = [System.Security.Cryptography.X509Certificates.RSACertificateExtensions]::GetRSAPrivateKey($certificate)
        }
       
        $path = "$env:ALLUSERSPROFILE\Microsoft\Crypto\Keys\$($privateKey.key.UniqueName)"
        $permissions = Get-Acl -Path $path
        $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("SCHLEUPEN-AG\CS_Applikation", 'Read', 'None', 'None', 'Allow')
        $permissions.AddAccessRule($accessRule)
        Set-Acl -Path $path -AclObject $permissions
    }
}

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $user, $password)))

$headers = @{Authorization = "Basic $base64AuthInfo" }

$url = "https://api.github.com/repos/schleupen/$($repo)/releases/latest"
Write-Output "Suche nach aktuellster Version ($url)..."
$json = Invoke-RestMethod -Method Get -Uri $url -Headers $headers
$assetId = $json.assets[0].id

$assetHeaders = @{Authorization = "Basic $base64AuthInfo"; Accept = "application/octet-stream" }

$downloadUrl = "https://api.github.com/repos/Schleupen/${repo}/releases/assets/${assetId}"
Write-Output "Lade Assets herunter ($downloadUrl)..."
New-Item "C:\GitHubDownload" -ItemType Directory -Force | Out-Null
New-Item "C:\GitHubDownload\FakeServer" -ItemType Directory -Force | Out-Null
Invoke-WebRequest -Method Get -Uri $downloadUrl -Headers $assetHeaders -OutFile "C:\GitHubDownload\FakeServer.zip"

Write-Output "Entpacke ZIP..."
Expand-Archive -Path "C:\GitHubDownload\FakeServer.zip" -DestinationPath "C:\GitHubDownload\FakeServer" -Force

Write-Output "Installiere Zertifikate..."
Import-PfxCertificate -FilePath "C:\GitHubDownload\FakeServer\Certificates\client.pfx" -CertStoreLocation "Cert:\LocalMachine\My" -Password (ConvertTo-SecureString -String test -AsPlainText -Force) | Out-Null
Import-Certificate -FilePath "C:\GitHubDownload\FakeServer\Certificates\fakeserver_ca.crt" -CertStoreLocation "Cert:\LocalMachine\Root" | Out-Null
Write-Output "Installiere Entwicklungszertifikate..."
dotnet dev-certs https
Get-ChildItem -Path Cert:\CurrentUser\My | Where-Object -Property Subject -eq "CN=localhost" | Export-PfxCertificate -FilePath C:\dotnetdevcert.pfx -Password (ConvertTo-SecureString -AsPlainText -Force "test")
Import-PfxCertificate -FilePath "C:\dotnetdevcert.pfx" -CertStoreLocation "Cert:\LocalMachine\Root" -Password (ConvertTo-SecureString -String test -AsPlainText -Force) | Out-Null

Write-Output "Berechtigung für CS_Applikation setzen an Private Key..."
Set-CertificatePrivateKeyPermission -CertStorePath "Cert:\LocalMachine\My"

Write-Output "Starte Fakeserver..."
Start-Process .\Schleupen.AS4.MSH.BusinessAPI.FakeServer.exe -WorkingDirectory "C:\GitHubDownload\FakeServer"

Write-Output "Fertig."