def projectName = 'shopping.api'
def gitUri = 'http://Staging/dotnet/shopping.api.git'
def dockerTag = 'Staging:5000/shopping.api'
def dockerComposeFile1 = 'Staging/docker-compose.yml'

folder(projectName)
folder("$projectName/Staging") {
    description 'The staging environment is the environment in which the application runs when it is live and being used by end users.'
}

pipelineJob("$projectName/Staging/Deploy_umcs01n1") {
    logRotator(-1, 10)
    definition {
        parameters {
            stringParam('Tag', 'latest', 'Docker image tag for deploy.')
        }
        cps {
            sandbox()
            script("""
                @Library('jenkins-shared-libraries')_

                def _gitBranch = "refs/tags/\$Tag"
                def _dockerTag = '$dockerTag'.replaceAll(/:\\w+\$/, '') + ":\$Tag"

                netStaging {
                    remoteHost = 'umcs01n1.devshift.local'
                    gitUri = '$gitUri'
                    gitBranch = _gitBranch
                    projectName = '$projectName'
                    dockerTag = _dockerTag
                    dockerComposeFile = '$dockerComposeFile1'
                }
            """)
        }
    }
}


