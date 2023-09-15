terraform {
  required_version = "~> 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=3.5.0"
    }
  }

  backend "azurerm" {
    resource_group_name  = "nd-usps-state-rg"
    storage_account_name = "nduspsstate"
    container_name       = "nd-usps-state-sc"
    key                  = "apps/NETDOCUMENTS_STACK/layer1"
  }
}

provider "azurerm" {
  features {
  }
}

module "application_layer1" {
  source       = "../../../../layers/layer1"
  environment  = "prod"
  azure_region = "us-central"

  # TODO: Adjust as needed. Shouldn't need to change node counts in dev
  # but may need to in qa.
  aks_vm_size    = "Standard_D2s_v5"
  aks_node_count = 2
  aks_max_node_count = 2

  # TODO: Adjust as needed. 
  kubernetes_version = "1.24.9"

  // Tenant Id for "Netdocuments-US-PROD" subscription
  tenant_id = "8d8705d8-e89c-47ef-a298-69dcd364b2d0"
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