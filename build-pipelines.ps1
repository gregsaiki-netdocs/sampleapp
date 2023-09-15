param(
    [Parameter(Mandatory=$true)][string]$ADO_PROJECT,
    [Parameter(Mandatory=$true)][string]$ADO_REPOSITORY
)

Write-Host "ADO Project = $ADO_PROJECT"
Write-Host "ADO Repository = $ADO_REPOSITORY"

$PIPELINE_NAME="$ADO_REPOSITORY Build-Dev-QA"

# Create the pipeline
az pipelines create --name "$PIPELINE_NAME" --branch main --project "$ADO_PROJECT" --yaml-path sampleapp/azure-pipeline.yml --repository "$ADO_REPOSITORY" --repository-type tfsgit --organization https://dev.azure.com/NetDocuments/ --skip-run

# Get the pipeline, to get its id
$BUILD_DEFINITION_ID=(az pipelines show --name "$PIPELINE_NAME" --org https://dev.azure.com/NetDocuments/ --project "$ADO_PROJECT" --output json | ConvertFrom-Json).id

$ADO_REPOSITORY_ID=(az repos show --organization https://dev.azure.com/NetDocuments/ --project "$ADO_PROJECT" --repository "$ADO_REPOSITORY" --output json | ConvertFrom-Json).id

Write-Host "Repository Id = $ADO_REPOSITORY_ID"
Write-Host "Build Definition Id = $BUILD_DEFINITION_ID"

# Add a branch build policy
az repos policy build create --enabled true --blocking true --repository-id="$ADO_REPOSITORY_ID" --project "$ADO_PROJECT" --branch main --build-definition-id $BUILD_DEFINITION_ID --valid-duration 0.0 --queue-on-source-update-only false --manual-queue-only false --display-name "Main branch build policy" --organization https://dev.azure.com/NetDocuments/