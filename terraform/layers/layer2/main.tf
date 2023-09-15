### Tasks to do
# stack needs to be pre approved so network can be created
#


# Steps to deploy
# 1. Ask CloudOps or Forerunners to create a stack (creates and creates a subnet for aks, and assigns an ip address for an app gateway)
# 2. Assign stack name to stack variable to
# 3. Terraform init
# 4. Tefraform plan
# 5. Terraform apply

locals {
  tags = {
    Application = "NETDOCUMENTS_STACK"
    Environment = var.environment
    Team        = "modernization"
    Train       = "pod2"
  }
}

module "application" {
  source = "git::ssh://NetDocuments@vs-ssh.visualstudio.com/v3/NetDocuments/DevOps/terraform-modules//modules/application_layers/layer2?ref=v0.9.0"
  # TODO: Add stackname
  stack  = "NETDOCUMENTS_STACK"

  environment  = var.environment
  azure_region = var.azure_region

  # TODO: Adjust namespaces
  aks_namespaces = ["sampleapp"]

  splunk_index_name = var.splunk_index_name

  extra_tags = local.tags
}

output "application" {
  value = module.application
}
