def projectName = 'shopping.api'
def gitUri = 'https://github.com/punnawutbu/shopping.api.git'
def dockerTag = 'Production:5000/shopping.api'
def dockerComposeFile = 'Production/docker-compose.yml'

folder(projectName)
folder("$projectName/Production") {
    description 'The production environment is the environment in which the application runs when it is live and being used by end users.'
}

pipelineJob("$projectName/Production/Deploy_pmcs01n1") {
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

                netProduction {
                    remoteHost = 'pmcs01n1.devshift.local'
                    gitUri = '$gitUri'
                    gitBranch = _gitBranch
                    projectName = '$projectName'
                    dockerTag = _dockerTag
                    dockerComposeFile = '$dockerComposeFile'
                }
            """)
        }
    }
}

pipelineJob("$projectName/Production/Deploy_pmcs02n2") {
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

                netProduction {
                    remoteHost = 'pmcs02n2.devshift.local'
                    gitUri = '$gitUri'
                    gitBranch = _gitBranch
                    projectName = '$projectName'
                    dockerTag = _dockerTag
                    dockerComposeFile = '$dockerComposeFile'
                }
            """)
        }
    }
}

pipelineJob("$projectName/Production/Deploy_aws") {
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

                netProductionAws {
                    remoteHost = 'mcsn1.nggaws-local'
                    gitUri = '$gitUri'
                    gitBranch = _gitBranch
                    projectName = '$projectName'
                    dockerTag = _dockerTag
                    dockerComposeFile = '$dockerComposeFile'
                }
            """)
        }
    }
}

pipelineJob("$projectName/Production/Newman") {
    logRotator(-1, 10)
    definition {
        parameters {
            stringParam('Tag', 'latest', 'Git tag for build Docker image and push to Docker Registry.')
        }
        cps {
            sandbox()
            script("""
                @Library('jenkins-shared-libraries')_

                def _gitBranch = "refs/tags/\$Tag"

                newman {
                    gitUri = '$gitUri'
                    gitBranch = _gitBranch
                    environment = 'Production'
                }
            """)
        }
    }
}