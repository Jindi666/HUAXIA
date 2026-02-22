@Library('laiye') _

pipeline {
    environment {
        PROJECT = 'laiye-customer-webapi'
    }
    agent any
    options {
        buildDiscarder logRotator(artifactDaysToKeepStr: '', artifactNumToKeepStr: '', daysToKeepStr: '30', numToKeepStr: '3')
    }
    stages {

        stage('Build Docker Images') {
            failFast true
            parallel {

                stage('Build laiye-customer-webapi image with tag') {
                    when {
                        anyOf {
                            tag "v*"
                        }
                    }

                    steps {
                        echo "laiye-customer-webapi"
                        script {
                            commander.build_image("${env.PROJECT}", "laiye-customer-webapi","${env.BRANCH_NAME}", "./docker/Dockerfile")
                        }
                    }
                }

                stage('Build laiye-customer-webapi image with branch') {
                    when {
                        anyOf {
                            branch "dev*"
                        }
                    }

                    steps {
                        echo "laiye-customer-webapi"
                        script {
                            commander.test_build("${env.PROJECT}", "laiye-customer-webapi", "./docker/Dockerfile")
                        }
                    }
                }

            }
        }
    }
}
