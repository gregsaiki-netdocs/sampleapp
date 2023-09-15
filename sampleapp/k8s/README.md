# Run this first
## Create "sampleapp" namespace
'''
kubectl create namespace sampleapp
'''

# Generate a real manifest
'''
kubectl kustomize k8s/deploy/env/local > out.yaml
'''

# Apply the manifest to the "sampleapp" namespace
'''
kubectl apply -f out.yaml -n sampleapp
'''

# List the pods in the "sampleapp" namespace
'''
kubectl get pods -n sampleapp
'''

# Show more info about a particular pod
'''
kubectl describe pods -n sampleapp {pod name}
'''

# Get onto the pod
'''
kubetl exec -it -n sampleapp {pod name} -- sh
'''

# Delete the deployment
'''
kubectl delete -f out.yaml -n sampleapp
'''

# Port forward to 8080 to port 80 of ingress
'''
kubectl port-forward -n ingress-nginx service/ingress-nginx-controller 8080:80
'''