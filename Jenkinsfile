pipeline {
    agent any
    tools {
            msbuild 'MSBuild 2022'
        }
	environment {
		DOCKER_BUILDKIT=1
		ENVIRONMENT="${ENVIRONMENT}"
		APPNAME="${APPNAME}"
        PROJECT_PATH='TeamRadio/TeamRadio.csproj'
        SOLUTION_PATH='TeamRadio.sln'
    }

	stages {

	    stage("Build") {
            steps {
                    script {
//                         sh 'dotnet build /property:nowarn=* $PROJECT_PATH -p:PackageVersion=1.0.$BUILD_NUMBER --configuration Release'
                        sh 'msbuild $SOLUTION_PATH /p:Configuration=Release %MSBUILD_ARGS%'
                     }

            }
        }
        
		stage("Docker Build & Push") {
            steps {
                script {
                    def dockerFileName = "Dockerfile"
                    def dockerImage = docker.build("${APPNAME}:${ENVIRONMENT}-1.0.${env.BUILD_ID}", "-f ${dockerFileName} --no-cache --build-arg BUILD_ID=${env.BUILD_ID} --build-arg ENVIRONMENT=$ENVIRONMENT .")
                    try {
                        docker.withRegistry("https://index.docker.io/v1/", "docker-hub-credentials") {
                            dockerImage.push("${VERSION_PREFIX}${ENVIRONMENT}-1.0.${env.BUILD_ID}${VERSION_SUFFIX}")
                        }
                    } finally {
                        sh "docker login -u ${DOCKER_USERNAME} -p ${DOCKER_PASSWORD}"
                        sh "docker tag ${APPNAME}:${ENVIRONMENT}-1.0.${env.BUILD_ID} sadiksa/${APPNAME}:${ENVIRONMENT}-1.0.${env.BUILD_ID}"
                        sh "docker push sadiksa/${APPNAME}:${ENVIRONMENT}-1.0.${env.BUILD_ID}"
                        sh "docker images -q ${dockerImage.id} | xargs docker rmi --force"
                    }
                }
            }
        }

	}
}

