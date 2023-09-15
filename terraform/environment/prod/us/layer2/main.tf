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
    key                  = "apps/NETDOCUMENTS_STACK/layer2"
  }
}

provider "azurerm" {
  features {
  }
}

module "application" {
  source       = "../../../../layers/layer2"
  environment  = "prod"
  azure_region = "us-central"

  splunk_index_name = "k8-sampleapp"
}