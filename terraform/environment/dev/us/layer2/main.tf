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
    key                  = "apps/NETDOCUMENTS_STACK/layer2"
  }
}

provider "azurerm" {
  features {
  }
}

module "application" {
  source       = "../../../../layers/layer2"
  environment  = "dev"
  azure_region = "us-west-3"

  splunk_index_name = "lab-k8-sampleapp"
}