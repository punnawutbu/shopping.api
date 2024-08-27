def folderName = 'shopping.api'
def gitUrl = 'https://github.com/punnawutbu/shopping.api.git'
def gitBranch = 'refs/heads/main'
 
folder(folderName)
job("$folderName/Seed") {
  logRotator(-1, 10)
  wrappers {
    timestamps()
  }
 
  scm {
    git {
      branch(gitBranch)
      remote {
        url(gitUrl)
        credentials('gitlab')
      }
    }
  }
 
  steps {
    triggers {
      gitlab {
        triggerOnPush(true)
        triggerOnMergeRequest(false)
        triggerOnAcceptedMergeRequest(false)
        triggerOnClosedMergeRequest(false)
        triggerOpenMergeRequestOnPush('never')
        triggerOnNoteRequest(false)
        noteRegex('')
        skipWorkInProgressMergeRequest(false)
        ciSkip(false)
        setBuildDescription(false)
        addNoteOnMergeRequest(false)
        addCiMessage(false)
        addVoteOnMergeRequest(false)
        acceptMergeRequestOnSuccess(false)
        branchFilterType('RegexBasedFilter')
        includeBranchesSpec('')
        excludeBranchesSpec('')
        targetBranchRegex('master')
        mergeRequestLabelFilterConfig {
          include('')
          exclude('')
        }
        secretToken('d9604e4ba8c8bd3140ab99e909aa807d')
        triggerOnPipelineEvent(false)
      }
    }
 
    dsl {
      external "**/*.groovy"
    }
 
    publishers {
        wsCleanup()
    }
  }
}