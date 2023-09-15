variable "environment" {
  description = "Environment, dev, qa, stage, production"
  type        = string
}

variable "azure_region" {
  description = "Azure region, ex: us-west-3"
  type        = string
}

variable "tenant_id" {
  description = "Tenant Id"
  type = string
}

variable "aks_vm_size" {
  description = "The VM size to use for the AKS default node pool"
  type        = string
}

variable "aks_node_count" {
  description = "Minimun number of nodes in AKS default node pool"
  type        = string
  default     = "1"
}

variable "aks_max_node_count" {
  description = "Maximum number of nodes in AKS default node pool"
  type        = string
  default     = "1"
}

variable "aks_min_node_count" {
  description = "Maximum number of nodes in AKS default node pool"
  type        = string
  default     = "1"
}

variable "kubernetes_version" {
  type = string
}
