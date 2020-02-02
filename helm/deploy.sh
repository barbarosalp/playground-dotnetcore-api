#!/bin/bash

set -e

if [ -h "${BASH_SOURCE[0]}" ]; then
    WORK_DIR=$( cd "$( dirname "`readlink -f "${BASH_SOURCE[0]}"`" )" && pwd )
else
    WORK_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )
fi

# Variables
HELM_LOCAL_REPO="${HOME}"/.helm/localrepo

# Docker build
eval $(minikube docker-env)
docker build -t barb/dotnetcore-api:1.0-SNAPSHOT .

# Application Chart
helm package --dependency-update "${WORK_DIR}"/dotnetcore-api --destination "${HELM_LOCAL_REPO}"

# Add application chart to local helm repo
helm repo index "${HELM_LOCAL_REPO}" --url http://127.0.0.1:8879/charts
helm repo update

# Umbrella Chart
helm package --dependency-update "${WORK_DIR}"/component --destination "${HELM_LOCAL_REPO}"

# Install
helm upgrade --install component -f "${WORK_DIR}"/component-values.yaml "${HELM_LOCAL_REPO}"/component-1.0.0.tgz

# Wait for healthcheck
#kubectl rollout status --watch=true deployment component-dotnetcore-api --timeout=60s
