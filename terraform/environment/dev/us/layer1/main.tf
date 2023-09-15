terraform {
  required_version = "~> 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=3.5.0"
    }
  }

  backend "azurerm" {
    resource_group_name  = "nd-usw3d-state-rg"
    storage_account_name = "ndusw3dstate"
    container_name       = "nd-usw3d-state-sc"
    key                  = "apps/NETDOCUMENTS_STACK/layer1"
  }
}

provider "azurerm" {
  features {
  }
}

module "application_layer1" {
  source       = "../../../../layers/layer1"
  environment  = "dev"
  azure_region = "us-west-3"

  # TODO: Adjust as needed. Shouldn't need to change node counts in dev
  # but may need to in qa.
  aks_vm_size    = "Standard_D2s_v5"
  aks_node_count = 2
  aks_max_node_count = 2
  aks_min_node_count = 2

  # TODO: Adjust as needed. 
  kubernetes_version = "1.26.3"

  // Tenant Id for "eng-shared-002" subscription
  tenant_id = "dcf9bb95-76f7-4474-893f-4935531d0946"
}

output "aad_managed_identity" {
  value = module.application_layer1.application.aad_managed_identity
}

output "aks" {
  value = module.application_layer1.application.aks
}

output "rg" {
  value = module.application_layer1.application.rg
}
