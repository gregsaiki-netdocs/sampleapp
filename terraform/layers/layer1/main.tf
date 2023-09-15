### Tasks to do
# stack needs to be pre approved so network can be created
#


# Steps to deploy
# 1. Ask CloudOps or Forerunners to create a stack (creates and creates a subnet for aks, and assigns an ip address for an app gateway)
#  1. subnet ip block
#  2. reserve private app gateway ip address
#  3. generate public ip on firewall with dnat rule
#  4. dns rule to the public ip from 3      setbuilder-qa.netdocuments.com -> public ip -> private app gateway ip -> aks load balancer
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
  source = "git::ssh://NetDocuments@vs-ssh.visualstudio.com/v3/NetDocuments/DevOps/terraform-modules//modules/application_layers/layer1?ref=v0.9.0"
  # TODO: Add stackname
  stack  = "NETDOCUMENTS_STACK"

  environment  = var.environment
  azure_region = var.azure_region

  aks_vm_size        = var.aks_vm_size
  aks_node_count     = var.aks_node_count
  aks_min_node_count = var.aks_min_node_count
  aks_max_node_count = var.aks_max_node_count
  kubernetes_version = var.kubernetes_version

  tenant_id = var.tenant_id
  
  # TODO: Uncomment if deploying the topics service. Modify topics
  # and subscriptions
  # topics = {
  #   topic_subscriptions = [{
  #     topic         = "generate_binder"
  #     subscriptions = ["binder_generator"]
  #     }, {
  #     topic         = "test_topic"
  #     subscriptions = ["test_subscription"]
  #   }]
  # }

  # TODO: Uncomment if deploying the the collections service
  # collections = {}

  extra_tags = local.tags
}

output "application" {
  value = module.application
}