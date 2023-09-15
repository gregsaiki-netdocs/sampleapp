variable "environment" {
  description = "Environment, dev, qa, stage, production"
  type        = string
}

variable "azure_region" {
  description = "Azure region, ex: us-west-3"
  type        = string
}

variable "splunk_index_name" {
  type = string
  default = "need_a_splunk_index"
}