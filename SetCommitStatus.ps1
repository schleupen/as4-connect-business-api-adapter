param ($sha)
$user = $env:usr
$password = $env:pwd
$repo = "as4-connect-business-api-adapter"

[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $user, $password)))

$headers = @{Authorization = "Basic $base64AuthInfo" }

$url = "https://api.github.com/repos/schleupen/$($repo)/statuses/$($sha)"
Write-Host $url
curl.exe -XPOST -H "Authorization: token $password" $url -d "{
      \"state\": \"pending\",
      \"target_url\": \"https://jenkins",
      \"description\": \"The build is pending!\"
     '
    }"