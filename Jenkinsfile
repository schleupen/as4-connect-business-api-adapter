@Library('schleupen@master') _

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
        label 'hv24-entflochten-windows-sqlserver || hv24-integriert-windows-sqlserver || fv24-entflochten-windows-sqlserver || sv24-integriert-windows-sqlserver'
    }

    environment
    {        
        HTTPS_PROXY = "${SchleupenInternetProxyUrl}"
        HTTP_PROXY = "${SchleupenInternetProxyUrl}"
        NO_PROXY = "127.0.0.0/8,10.0.0.0/8,localhost,.schleupen-ag.de"
        GITHUB_TOKEN = credentials('Schleupen-Jenkins-AS4-GitHub')
    }

    stages
    {
        stage('preparation')
        {
            steps
            {
                script
                {
                    VERSION_NUMBER = "${Version}" // from jenkins build parameter
                    currentBuild.displayName = "${Version}"
                    
                    withCredentials([usernamePassword(credentialsId: 'Schleupen-Jenkins-AS4-GitHub', passwordVariable: 'pwd', usernameVariable: 'usr')]) {
                        powershellFile(filename: ".\\GithubSetCommitStatus.ps1", argumentList: "-sha ${SHA} -status pending")            
                    }
                    
                  }
            }
        }
        stage('copy build artifacts')
        {
            steps
            {
                script
                {
                    bat "xcopy /y /f /S /v \\\\schleupen-ag.de\\eww\\Build\\Pipeline\\Schleupen.AS4.BusinessAdapter\\${VERSION_NUMBER}\\ .\\build\\"
                }
            }
        }

        stage('IntegrativeTests') {
            stages {
                stage('start FakeServer') {
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
                            bat "dotnet test -c Release build/BusinessAdapter.FP.IntegrativeTests/bin/Release/net8.0/Schleupen.AS4.BusinessAdapter.FP.IntegrativeTests.dll --logger:\"junit;LogFilePath=Schleupen.AS4.BusinessAdapter.FP.IntegrativeTests.junit.xml\" --no-build"
                          }
                        }                          
                    }
                    post
                        {
                            always
                            {
                                archiveArtifacts allowEmptyArchive: false, artifacts: '*.junit.xml'
                                junit skipPublishingChecks: true, testResults: '*.junit.xml'
                            }
                        }
                }
            }                        
        }    
    }

    post {
           success {
               withCredentials([string(credentialsId: '697d0028-bb04-467b-bb3f-83699e6f49c3', variable: 'NEXUS_TOKEN')]) {
                    bat "dotnet nuget push ./build/BusinessAdapter/bin/Release/Schleupen.AS4.BusinessAdapter.${VERSION_NUMBER}.nupkg -s ${SchleupenNugetRepository}/Schleupen.CS.Nuget/index.json -k ${NEXUS_TOKEN}"
                    bat "dotnet nuget push ./build/BusinessAdapter.FP/bin/Release/Schleupen.AS4.BusinessAdapter.FP.${VERSION_NUMBER}.nupkg -s ${SchleupenNugetRepository}/Schleupen.CS.Nuget/index.json -k ${NEXUS_TOKEN}"
               }                          

               withCredentials([usernamePassword(credentialsId: 'Schleupen-Jenkins-AS4-GitHub', passwordVariable: 'pwd', usernameVariable: 'usr')]) {
                   powershellFile(filename: ".\\GithubSetCommitStatus.ps1", argumentList: "-sha ${SHA} -status success")
               
                   bat("git tag -a $Version ${SHA} -m ${Version}")
                   bat("git push https://${usr}:${pwd}@github.com/schleupen/as4-connect-business-api-adapter $Version")
               }
               
               
               notifyBuildSuccessful()
           }
           unstable {
               withCredentials([usernamePassword(credentialsId: 'Schleupen-Jenkins-AS4-GitHub', passwordVariable: 'pwd', usernameVariable: 'usr')]) {
                   powershellFile(filename: ".\\GithubSetCommitStatus.ps1", argumentList: "-sha ${SHA} -status error -description unstable")
               }
               notifyBuildUnstable()
           }
           failure {
               withCredentials([usernamePassword(credentialsId: 'Schleupen-Jenkins-AS4-GitHub', passwordVariable: 'pwd', usernameVariable: 'usr')]) {
                   powershellFile(filename: ".\\GithubSetCommitStatus.ps1", argumentList: "-sha ${SHA} -status failure -description failure") 
               }
               notifyBuildFailed()
           }
    }
}
