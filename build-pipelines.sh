#!/bin/bash

ADO_PROJECT=$1
ADO_REPOSITORY=$2

echo "ADO Project = $ADO_PROJECT"
echo "ADO Repository = $ADO_REPOSITORY"

PIPELINE_NAME="$ADO_REPOSITORY Build-Dev-QA"

# Create the pipeline
az pipelines create --name "$PIPELINE_NAME" --branch main --project "$ADO_PROJECT" --yaml-path sampleapp/azure-pipeline.yml --repository "$ADO_REPOSITORY" --repository-type tfsgit --organization https://dev.azure.com/NetDocuments/ --skip-run

# Get the pipeline, to get it's id
BUILD_DEFINITION_ID=$(az pipelines show --name "$PIPELINE_NAME" --org https://dev.azure.com/NetDocuments/ --project "$ADO_PROJECT" --output json | jq -r .id)

ADO_REPOSITORY_ID=$(az repos show --organization https://dev.azure.com/NetDocuments/ --project "$ADO_PROJECT" --repository "$ADO_REPOSITORY" --output json | jq -r .id)

echo "Repository Id = $ADO_REPOSITORY_ID"
echo "Build Definition Id = $BUILD_DEFINITION_ID"

# Add a branch build policy
az repos policy build create --enabled true --blocking true --repository-id="$ADO_REPOSITORY_ID" --project "$ADO_PROJECT" --branch main --build-definition-id $BUILD_DEFINITION_ID --valid-duration 0.0 --queue-on-source-update-only false --manual-queue-only false --display-name "Main branch build policy" --organization https://dev.azure.com/NetDocuments/
