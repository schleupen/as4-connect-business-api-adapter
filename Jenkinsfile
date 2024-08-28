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
        label 'hv24-entflochten-windows-sqlserver'
    }

    environment
    {        
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
                    if (env.BRANCH_NAME == 'main') {
                        VERSION_NUMBER = "1.0.${BUILD_NUMBER}"
                    } else {
                        VERSION_NUMBER = "0.0.${BUILD_NUMBER}-${GIT_BRANCH.split("/")[1]}"
                    }                    
                    currentBuild.displayName = "${VERSION_NUMBER}"
                    
                    bat  "dotnet build -c Release ./BusinessAdapter.sln -p:Version=${VERSION_NUMBER}"
                }
            }
        }

        stage('IntegrativeTests') {
            parallel {
                stage('Installation') {
                    stages {
                        stage('FakeServer') {
                            steps {
                                timeout(time: 3, unit: 'HOURS') {
                                    withCredentials([usernamePassword(credentialsId: 'Schleupen-Jenkins-AS4-GitHub', passwordVariable: 'pwd', usernameVariable: 'usr')]) {
                                        powershellFile(filename: ".\\BusinessAdapter.FP.IntegrativeTests\\Start-As4ConnectFakeServer.ps1")  
                                    }                                 
                                }
                            }
                        }
                        stage('Tests') {
                            steps {
                                timeout(time: 3, unit: 'HOURS') {
                                   script {                                                                          
                                    bat  'dotnet restore ./BusinessAdapter.sln'
                                    bat  'dotnet build -c Release ./BusinessAdapter.sln'

                                    bat "dotnet test -c Release BusinessAdapter.FP.IntegrativeTests/BusinessAdapter.FP.IntegrativeTests.csproj --logger:\"junit;LogFilePath=BusinessAdapter.FP.IntegrativeTests.junit.xml\" --no-build"
                                  }
                                }                          
                            }
                        }
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
                        bat  'dotnet test -c Release BusinessAdapter.UnitTests/BusinessAdapter.UnitTests.csproj --logger:\"junit;LogFilePath=BusinessAdapter.UnitTests.junit.xml\" -e HOME=/tmp'
                                                
                        bat  'dotnet test -c Release BusinessAdapter.FP.UnitTests/BusinessAdapter.FP.UnitTests.csproj --logger:\"junit;LogFilePath=BusinessAdapter.FP.UnitTests.junit.xml\" -e HOME=/tmp'
                        bat  'dotnet test -c Release BusinessAdapter.FP.Console.UnitTests/BusinessAdapter.FP.Console.UnitTests.csproj --logger:\"junit;LogFilePath=BusinessAdapter.FP.Console.UnitTests.junit.xml\" -e HOME=/tmp'
                        
                        bat  'dotnet test -c Release BusinessAdapter.MP.UnitTests/BusinessAdapter.MP.UnitTests.csproj --logger:\"junit;LogFilePath=BusinessAdapter.MP.UnitTests.junit.xml\" -e HOME=/tmp'
                        bat  'dotnet test -c Release BusinessAdapter.MP.Console.UnitTests/BusinessAdapter.MP.Console.UnitTests.csproj --logger:\"junit;LogFilePath=BusinessAdapter.MP.Console.UnitTests.junit.xml\" -e HOME=/tmp'
                }
            }
            post
            {
                always
                {
                    archiveArtifacts allowEmptyArchive: false, artifacts: '*/*.junit.xml'
                    junit skipPublishingChecks: true, testResults: '*/*.junit.xml'
                }
            }
        }
   
    }

    post {
           success {
               setBuildStatus("Build succeeded", "SUCCESS")      
               withCredentials([string(credentialsId: '697d0028-bb04-467b-bb3f-83699e6f49c3', variable: 'NEXUS_TOKEN')]) {
                    bat "dotnet nuget push .\BusinessAdapter.FP\bin\Release\Schleupen.AS4.BusinessAdapter.FP.${VERSION_NUMBER}.nupkg -s https://nexus.schleupen-ag.de/repository/nuget-v3/"
               }         
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
