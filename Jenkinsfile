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
                    bat  'dotnet restore ./BusinessAdapter.sln'
                    bat  'dotnet build -c Release ./BusinessAdapter.sln'
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
                                    dir('update') {
                                          withCredentials([usernamePassword(credentialsId: 'Schleupen-Jenkins-AS4-GitHub', passwordVariable: 'pwd', usernameVariable: 'usr')]) {
                                              powershellFile(filename: "..\\BusinessAdapter.FP.IntegrativeTests\\Start-As4ConnectFakeServer.ps1")  
                                          }
                                    }
                                }
                            }
                        }
                        stage('Tests') {
                            steps {
                                timeout(time: 3, unit: 'HOURS') {
                                   script {  
                                    sh 'mkdir -p ./Tests/unit/results'
                                                                        
                                    bat  'dotnet restore ./BusinessAdapter.sln'
                                    bat  'dotnet build -c Release ./BusinessAdapter.sln'

                                    bat "dotnet test -c Release BusinessAdapter.FP.IntegrativeTests/BusinessAdapter.FP.IntegrativeTests.csproj --results-directory ./Tests/unit/results --logger \"trx;LogFileName=IntegrativeTests.xml\" --no-build"
                                  }
                                }                          
                            }
                        }
                    }                    
                    post {
                        success {
                            processBuildSucceeded()
                        }
                        unstable {
                            processBuildUnstable()
                        }
                        failure {
                            processBuildUnstable()
                        }
                        aborted {
                            processBuildUnstable()
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
                        sh 'mkdir -p ./Tests/unit/results'

                        bat  'dotnet test -c Release BusinessAdapter.UnitTests/BusinessAdapter.UnitTests.csproj --results-directory ./Tests/unit/results --logger \'trx;LogFileName=BusinessAdapter.UnitTests.junit.xml\' -e HOME=/tmp'
                                                
                        bat  'dotnet test -c Release BusinessAdapter.FP.UnitTests/BusinessAdapter.FP.UnitTests.csproj --results-directory ./Tests/unit/results --logger \'trx;LogFileName=BusinessAdapter.FP.UnitTests.junit.xml\' -e HOME=/tmp'
                        bat  'dotnet test -c Release BusinessAdapter.FP.Console.UnitTests/BusinessAdapter.FP.Console.UnitTests.csproj --results-directory ./Tests/unit/results --logger \'trx;LogFileName=BusinessAdapter.FP.Console.UnitTests.junit.xml\' -e HOME=/tmp'
                        
                        bat  'dotnet test -c Release BusinessAdapter.MP.UnitTests/BusinessAdapter.MP.UnitTests.csproj --results-directory ./Tests/unit/results --logger \'trx;LogFileName=BusinessAdapter.MP.UnitTests.junit.xml\' -e HOME=/tmp'
                        bat  'dotnet test -c Release BusinessAdapter.MP.Console.UnitTests/BusinessAdapter.MP.Console.UnitTests.csproj --results-directory ./Tests/unit/results --logger \'trx;LogFileName=BusinessAdapter.MP.Console.UnitTests.junit.xml\' -e HOME=/tmp'
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