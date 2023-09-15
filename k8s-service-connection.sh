#!/bin/bash

ADO_PROJECT=$1
echo "ADO Project = $ADO_PROJECT"

K8S_SERVER=$(kubectl config view --minify -o jsonpath={.clusters[0].cluster.server})
TOKEN=$(kubectl get secrets -n sampleapp $(kubectl get serviceAccounts sampleapp-deploy -n sampleapp -o=jsonpath={.secrets[*].name}) -o 'go-template={{index .data.token }}')
CRT=$(kubectl get secrets -n sampleapp $(kubectl get serviceAccounts sampleapp-deploy -n sampleapp -o=jsonpath={.secrets[*].name}) -o 'go-template={{index .data "ca.crt" }}')

cat << EOF > ./service_endpoint.json 
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
  "name": "sampleapp-dev-aks",
  "type": "kubernetes",
  "url": "$K8S_SERVER"
}
EOF

az devops service-endpoint create --service-endpoint-configuration ./service_endpoint.json --organization https://dev.azure.com/NetDocuments/ --project "$ADO_PROJECT"