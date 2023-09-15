param(
    [Parameter(Mandatory=$true)][string]$ADO_PROJECT
)

Write-Host "ADO Project = $ADO_PROJECT"

$K8S_SERVER=(kubectl config view --minify -o jsonpath={.clusters[0].cluster.server})
$TOKEN=(kubectl get secrets -n demo $(kubectl get serviceAccounts demo-deploy -n demo -o=jsonpath={.secrets[*].name}) -o 'go-template={{index .data.token }}')
$CRT=(kubectl get secrets -n demo $(kubectl get serviceAccounts demo-deploy -n demo -o=jsonpath={.secrets[*].name}) -o 'go-template={{index .data "ca.crt" }}')

@"
{
  "authorization": {
    "parameters": {
      "apitoken": "$TOKEN",
      "isCreatedFromSecretYaml": "true",
      "serviceAccountCertificate": "$CRT"
    },
    "scheme": "Token"
  },
  "data": {
    "authorizationType": "ServiceAccount"
  },
  "name": "demo-dev-aks",
  "type": "kubernetes",
  "url": "$K8S_SERVER"
}
"@ | Set-Content -Path ./service_endpoint.json

az devops service-endpoint create --service-endpoint-configuration ./service_endpoint.json --organization https://dev.azure.com/NetDocuments/ --project "$ADO_PROJECT"