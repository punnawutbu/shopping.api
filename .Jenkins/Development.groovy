def projectName = 'shopping.api'
def gitUri = 'https://github.com/punnawutbu/shopping.api.git'
def gitBranch = 'refs/heads/main'
def publishProject = 'shopping.api/shopping.api.csproj'
def dockerTag = 'workstation01.devshift.local:5000/shopping.api:latest'
def dockerComposeFile = 'Development/docker-compose.yml'

folder(projectName)
folder("$projectName/Development") {
  description 'The development is a environment used for developing an application.'
}

pipelineJob("$projectName/Development/Build") {
  logRotator(-1, 10)
  properties {
    disableConcurrentBuilds()
    pipelineTriggers {
      triggers {
        upstream {
          upstreamProjects "../Seed"
          threshold 'SUCCESS'
        }
      }
    }
  }
  definition {
    cps {
      sandbox()
      script("""
        @Library('jenkins-shared-libraries')_

        netDevelopmentN2V2 {
          gitUri = '$gitUri'
          gitBranch = '$gitBranch'
          publishProject = '$publishProject'
          dockerTag = '$dockerTag'
          projectName = '$projectName'
          dockerComposeFile = '$dockerComposeFile'
        }
      """)
    }
  }
}

pipelineJob("$projectName/Development/Build_Image") {
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
                def _dockerTag = '$dockerTag'.replaceAll(/:\\w+\$/, '') + ":\$Tag"

                netImageV2 {
                    gitUri = '$gitUri'
                    gitBranch = _gitBranch
                    projectName = '$projectName'
                    publishProject = '$publishProject'
                    dockerTag = _dockerTag
                }
            """)
        }
    }
}

pipelineJob("$projectName/Development/Newman") {
    logRotator(-1, 10)
    definition {
    cps {
      sandbox()
      script("""
        @Library('jenkins-shared-libraries')_

        newman {
            gitUri = '$gitUri'
            gitBranch = '$gitBranch'
            environment = 'Development'
        }
      """)
    }
  }
}