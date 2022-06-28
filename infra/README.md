#Deploying to Kubernetes

## Set up a Linux VM
Make sure you have root (or sudo) and ssh access.
Hetzner is a good option for renting your VM

## Install Kubernetes using k3sup

```bash
k3sup install \
    --ip your-ip-here \
    --user root \
    --context k8s-aci
 ```

The Kubeconfig file (which is needed to perform kubectl commands) corresponding to this node will automatically be saved to the current directory.
In order to point to this config when you're working in another directory, use the following command.

```bash
export kubeconfig="/path/to/your/kubeconfig"
```

The command above is not persistent and will be reset after closing the terminal.
To make it persistent, put the command in your `bashrc` or `.zshrc` file

## Install cert-manager 
You'll need this for HTTPS on Ingress
```bash
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.8.0/cert-manager.yaml
```

## Apply the yaml files
You can find these in the manifest folder, or generate them yourself using the Helm chart in infra/aci-rental-app
When using the Helm chart, you can edit the variables if you want to use a different domain name for example
```bash
cd infra/manifest
kubectl apply -f .
```