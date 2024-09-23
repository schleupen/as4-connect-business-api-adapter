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
        label 'built-in'
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
                    currentBuild.displayName = "${Version}"
                    
                    withCredentials([usernamePassword(credentialsId: 'Schleupen-Jenkins-AS4-GitHub', passwordVariable: 'pwd', usernameVariable: 'usr')]) {
                        powershellFile(filename: ".\\GithubSetCommitStatus.ps1", argumentList: "-sha ${SHA} -status pending")            
                    }
                    
                  }
            }
        }

        stage('IntegrativeTests') 
        {
            agent {
                label 'hv24-entflochten-windows-sqlserver || hv24-integriert-windows-sqlserver || fv24-entflochten-windows-sqlserver || sv24-integriert-windows-sqlserver'
            }
            stages 
            {
                stage('copy build artifacts')
                {
                    steps
                    {
                        script
                        {
                            bat "xcopy /y /f /S /v ${BuildPipelineLocation}\\Schleupen.AS4.BusinessAdapter\\${Version}\\ .\\build\\"
                        }
                    }
                }
                stage('start FakeServer') {
                    steps 
                    {
                        timeout(time: 3, unit: 'HOURS') {
                            withCredentials([usernamePassword(credentialsId: 'Schleupen-Jenkins-AS4-GitHub', passwordVariable: 'pwd', usernameVariable: 'usr')]) {
                                powershellFile(filename: ".\\BusinessAdapter.FP.IntegrativeTests\\Start-As4ConnectFakeServer.ps1")  
                            }                                 
                        }
                    }
                }
                stage('Tests') {
                    steps 
                    {
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
                stage('push packages') 
                {
                    steps 
                    {
                        withCredentials([string(credentialsId: '697d0028-bb04-467b-bb3f-83699e6f49c3', variable: 'NEXUS_TOKEN')]) 
                        {
                            bat "dotnet nuget push ./build/BusinessAdapter/bin/Release/Schleupen.AS4.BusinessAdapter.${Version}.nupkg -s ${SchleupenNugetRepository}/Schleupen.CS.Nuget/index.json -k ${NEXUS_TOKEN}"
                            bat "dotnet nuget push ./build/BusinessAdapter.FP/bin/Release/Schleupen.AS4.BusinessAdapter.FP.${Version}.nupkg -s ${SchleupenNugetRepository}/Schleupen.CS.Nuget/index.json -k ${NEXUS_TOKEN}"
                        }          
                    }
                }
            }
            post
            {
                success 
                {
                    processBuildSucceeded()
                }
                unstable 
                {
                    processBuildUnstable()
                }
                failure 
                {
                    processBuildUnstable()
                }
                aborted 
                {
                    processBuildUnstable()
                }
            }
        }
    }

    post 
    {
           success 
           {
               withCredentials([usernamePassword(credentialsId: 'Schleupen-Jenkins-AS4-GitHub', passwordVariable: 'pwd', usernameVariable: 'usr')]) 
               {
                   powershellFile(filename: ".\\GithubSetCommitStatus.ps1", argumentList: "-sha ${SHA} -status success")
               
                   script
                   {
                       if (env.BRANCH_NAME == 'main') {
                            bat("git tag -a $Version ${SHA} -m ${Version}")
                            bat("git push https://${usr}:${pwd}@github.com/schleupen/as4-connect-business-api-adapter $Version")
                       }
                   }
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