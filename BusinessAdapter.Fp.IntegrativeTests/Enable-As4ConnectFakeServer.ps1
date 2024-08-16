Write-Output "Entferne Endpunkt 'Schleupen.CS.edi.as4.BusinessAPI_3.0'..."
Remove-Endpoint -AddressTemplate "https://erp.as4.schleupen.cloud:443" -ArtifactIdentifier "Schleupen.CS.edi.as4.BusinessAPI_3.0"

Write-Output "Lege Endpunkt 'Schleupen.CS.edi.as4.BusinessAPI_3.0' für Fakeserver an..."
Add-Endpoint -AddressTemplate "https://localhost:8043" -ArtifactIdentifier "Schleupen.CS.edi.as4.BusinessAPI_3.0"