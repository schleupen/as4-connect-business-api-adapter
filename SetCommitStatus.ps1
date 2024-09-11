param ($sha)
$user = $env:usr
$password = $env:pwd
$repo = "as4-connect-business-api-adapter"

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $user, $password)))

$url = "https://api.github.com/repos/schleupen/$($repo)/statuses/$($sha)"
$json = '{ "state": "pending", "target_url": "https://jenkins", "description": "The build is pending!" }'

Write-Host $url
Write-Host $json

ls env:

curl.exe -L -X POST -H "Authorization: token $password" -H "Accept: application/vnd.github+json" -H "X-GitHub-Api-Version: 2022-11-28" $url -d "{""""state"""":""""success"""",""""target_url"""":""""$env:BUILD_URL"""",""""description"""":""""The build succeeded!"""",""""context"""":""""continuous-integration/jenkins""""}"
