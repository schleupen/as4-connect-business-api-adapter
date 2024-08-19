@Library('schleupen@master') _

void setBuildStatus(String message, String state) {
  step([
      $class: "GitHubCommitStatusSetter",
      reposSource: [$class: "ManuallyEnteredRepositorySource", url: "https://github.com/schleupen/as4-connect-business-api-adapter"],
      contextSource: [$class: "ManuallyEnteredCommitContextSource", context: "ci/jenkins/build-status"],
      errorHandlers: [[$class: "ChangingBuildStatusErrorHandler", result: "UNSTABLE"]],
      statusResultSource: [ $class: "ConditionalStatusResultSource", results: [[$class: "AnyBuildResult", message: message, state: state]] ]
  ]);
}

pipeline
{
    options 
    {
        buildDiscarder(logRotator(numToKeepStr: "8"))
        disableConcurrentBuilds()
        skipStagesAfterUnstable()
    }

    agent
    {
        label 'linux-docker'
    }

    environment
    {
        DOCKER_BUILDKIT = 1
        
        HTTPS_PROXY = "${SchleupenInternetProxyUrl}"
        HTTP_PROXY = "${SchleupenInternetProxyUrl}"
        NO_PROXY = "127.0.0.0/8,10.0.0.0/8,localhost,.schleupen-ag.de"
    }

    stages
    {
        stage('build')
        {
            steps
            {
                script
                {             
                    docker.image("mcr.microsoft.com/dotnet/sdk:8.0").inside("-u 0:0")
                    {
                        sh 'dotnet restore ./BusinessAdapter.sln'
                        sh 'dotnet build -c Release --no-restore ./BusinessAdapter.sln'
                    }
                }
            }
        }

        stage('unittests')
        {
            steps
            {
                script
                {
                    sh 'mkdir -p ./Tests/unit/results'

                    docker.image("mcr.microsoft.com/dotnet/sdk:8.0").inside("-u 0:0")
                    {
                        sh 'dotnet test ./BusinessAdapter.UnitTests/bin/Release/net8.0/Schleupen.AS4.BusinessAdapter.UnitTests.dll --results-directory ./Tests/unit/results --logger \'junit;LogFileName=BusinessAdapter.UnitTests.junit.xml\' -e HOME=/tmp'
                                                
                        sh 'dotnet test ./BusinessAdapter.FP.UnitTests/bin/Release/net8.0/Schleupen.AS4.BusinessAdapter.FP.UnitTests.dll --results-directory ./Tests/unit/results --logger \'junit;LogFileName=BusinessAdapter.FP.UnitTests.junit.xml\' -e HOME=/tmp'
                        sh 'dotnet test ./BusinessAdapter.FP.Console.UnitTests/bin/Release/net8.0/Schleupen.AS4.BusinessAdapter.FP.Console.UnitTests.dll --results-directory ./Tests/unit/results --logger \'junit;LogFileName=BusinessAdapter.FP.Console.UnitTests.junit.xml\' -e HOME=/tmp'
                        
                        sh 'dotnet test ./BusinessAdapter.MP.UnitTests/bin/Release/net8.0/Schleupen.AS4.BusinessAdapter.MP.UnitTests.dll --results-directory ./Tests/unit/results --logger \'junit;LogFileName=BusinessAdapter.MP.UnitTests.junit.xml\' -e HOME=/tmp'
                        sh 'dotnet test ./BusinessAdapter.MP.Console.UnitTests/bin/Release/net8.0/Schleupen.AS4.BusinessAdapter.MP.Console.UnitTests.dll --results-directory ./Tests/unit/results --logger \'junit;LogFileName=BusinessAdapter.MP.Console.UnitTests.junit.xml\' -e HOME=/tmp'
                    }
                    
                }
            }
            post
            {
                always
                {
                    archiveArtifacts allowEmptyArchive: false, artifacts: '*/unit/results/*.junit.xml'
                    junit skipPublishingChecks: true, testResults: '*/unit/results/*.junit.xml'
                }
            }
        }
        
        stage('integrative tests')
        {
            steps 
            {
                withCredentials([usernamePassword(credentialsId: 'Schleupen-Jenkins-AS4-GitHub', passwordVariable: 'pwd', usernameVariable: 'usr')]) {
                    powershellFile(filename: ".\\BusinessAdapter.FP.IntegrativeTests\\Start-As4ConnectFakeServer.ps1")  
               
                }
                script 
                {
                    sh 'mkdir -p ./Tests/unit/results'
                
                    docker.image("mcr.microsoft.com/dotnet/sdk:8.0").inside("-u 0:0")
                    {
                        sh 'dotnet test ./BusinessAdapter.FP.IntegrativeTests/bin/Release/net8.0/Schleupen.AS4.BusinessAdapter.FP.IntegrativeTests.dll --results-directory ./Tests/unit/results --logger \'junit;LogFileName=BusinessAdapter.FP.IntegrativeTests.junit.xml\' -e HOME=/tmp'
                    }
                }
            }
        }
    }

    post {
        success {
            setBuildStatus("Build succeeded", "SUCCESS")
            notifyBuildSuccessful()
        }
        unstable {
            notifyBuildUnstable()
        }
        failure {
            notifyBuildFailed()
        }
    }
}
