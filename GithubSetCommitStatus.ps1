param ($sha, $status, $description)

$user = $env:usr
$password = $env:pwd
$repo = $env:GIT_URL.split("/")[-1]

$url = "https://api.github.com/repos/schleupen/$($repo)/statuses/$($sha)"

$ErrorActionPreference = "Stop"
curl.exe -L -X POST -f -H "Authorization: token $password" -H "Accept: application/vnd.github+json" -H "X-GitHub-Api-Version: 2022-11-28" $url -d "{""""state"""":""""$status"""",""""target_url"""":""""$env:BUILD_URL"""",""""description"""":""""$description"""",""""context"""":""""$env:JOB_BASE_NAME """"}"
