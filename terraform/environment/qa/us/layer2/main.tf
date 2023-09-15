terraform {
  required_version = "~> 1.0"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=3.5.0"
    }
  }

  backend "azurerm" {
    resource_group_name  = "nd-uscq-state-rg"
    storage_account_name = "nduscqstate"
    container_name       = "nd-uscq-state-sc"
    key                  = "apps/NETDOCUMENTS_STACK/layer2"
  }
}

provider "azurerm" {
  features {
  }
}

module "application" {
  source       = "../../../../layers/layer2"
  environment  = "qa"
  azure_region = "us-central"

  splunk_index_name = "lab-k8-sampleapp"
}