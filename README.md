# .NET 8 API - Automatic Deploy with GitHub Actions and AKS

This repository contains an ASP.NET Core 8 (API) application with a CI/CD pipeline using GitHub Actions for:

- Building the Docker image
- Pushing to GitHub Container Registry (GHCR)
- Automatic deployment to an **AKS (Azure Kubernetes Service)** cluster using **Helm**

For the pipeline to work:

- The required permissions must be applied to the used ClientId;
- Add a secret in the keyvault with the Redis connection string;
- Grant permission in the keyvault for the Kubernetes addon with CSI;

```
az aks show --resource-group resource-group-name --name resource-name --query addonProfiles.azureKeyvaultSecretsProvider.identity.objectId -o tsv
to add in keyvault permission
az aks show --resource-group resource-group-name --name resource-name --query addonProfiles.azureKeyvaultSecretsProvider.identity.clientId -o tsv
to add in helm
```

---

## 🚀 Technologies

- [.NET 8](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Docker](https://www.docker.com/)
- [GitHub Actions](https://docs.github.com/en/actions)
- [Azure Kubernetes Service (AKS)](https://learn.microsoft.com/en-us/azure/aks/)
- [Helm](https://helm.sh/)

---

## 🛠️ Project Structure

```
.
├── .github/
│   └── workflows/
│       └── deploy.yml         # GitHub Actions pipeline
├── chart/                     # Helm chart for AKS deployment
│   ├── Chart.yaml
│   ├── values.yaml
│   └── templates/
├── Redis.API/                 # .NET 8 source code
├── Dockerfile
└── README.md
```

---

## 🔐 Required Secrets

In GitHub (Settings → Secrets → Actions), add:

| Name                              | Description |
|------------------------------------|-------------|
| `REGISTRY_USERNAME`                | GitHub username (e.g., `jheffh2012`) |
| `REGISTRY_TOKEN`                   | Access token with `write:packages` permission |
| `AZURE_CREDENTIALS`                | Azure credentials JSON (`az ad sp create-for-rbac --sdk-auth`) |
| `HELM_VALUES_SECRET_SECRET_KEY`    | Example of a secret value injected into Helm |
| `HELM_VALUES_SECRET_MOUNTPATH`     | Another secret value |
| `HELM_VALUES_SERVICE_TYPE`         | Another secret value |
| `HELM_VALUES_SERVICE_PORT`         | Another secret value |
| `HELM_VALUES_USER_ASSIGNED_IDENTITY_ID` | Another secret value |
| `HELM_VALUES_KEY_VAULT_NAME`       | Another secret value |
| `HELM_VALUES_TENANT_ID`            | Another secret value |

---

## ⚙️ How the Deploy Works

1. The pipeline is triggered **when a semantic tag is created** (e.g., `v1.0.0`)
2. The image is **built and published to Docker Hub**
3. Helm replaces the values and applies the **deploy to AKS**

---

## 🏁 Publishing a New Version

To publish a new version and start the deploy:

```bash
git tag v1.0.0
git push origin v1.0.0
```

## 🏁 Next Steps

1. Add tests to the application and pipeline;
2. Remove Helm from the repository and use GitOps