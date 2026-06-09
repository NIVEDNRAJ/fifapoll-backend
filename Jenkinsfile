pipeline {
    agent any

    environment {
        CONTAINER_NAME = 'fifa-poll-backend'
        IMAGE_NAME = 'fifa-poll-backend'
        NETWORK_NAME = 'fifa-poll-net'
        PORT_MAPPING = '5000:8080'
        DB_CONNECTION = "Server=fifa-poll-db;Database=fifapoll;User=root;Password=RootPassword123!;"
        JWT_KEY = 'super_secret_fifa_world_cup_polling_secret_key_2026'
        JWT_ISSUER = 'FifaPollApi'
        JWT_AUDIENCE = 'FifaPollUi'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build Docker Image') {
            steps {
                bat "docker build --no-cache -t ${IMAGE_NAME}:latest -t ${IMAGE_NAME}:${BUILD_NUMBER} ."
            }
        }

        stage('Deploy Container') {
            steps {
                script {
                    // Ensure Docker network exists
                    bat "docker network create ${NETWORK_NAME} 2>nul || ver >nul"
                    
                    // Stop and remove existing container if running
                    bat "docker stop ${CONTAINER_NAME} 2>nul || ver >nul"
                    bat "docker rm ${CONTAINER_NAME} 2>nul || ver >nul"
                    
                    // Launch new container using Windows Batch line continuation
                    bat """
                        docker run -d ^
                            --name ${CONTAINER_NAME} ^
                            --network ${NETWORK_NAME} ^
                            -p ${PORT_MAPPING} ^
                            -e ConnectionStrings__DefaultConnection="${DB_CONNECTION}" ^
                            -e JwtSettings__Key="${JWT_KEY}" ^
                            -e JwtSettings__Issuer="${JWT_ISSUER}" ^
                            -e JwtSettings__Audience="${JWT_AUDIENCE}" ^
                            ${IMAGE_NAME}:latest
                    """
                }
            }
        }
    }

    post {
        success {
            echo "Backend pipeline completed successfully!"
        }
        failure {
            echo "Backend pipeline failed. Please check the logs."
        }
    }
}
