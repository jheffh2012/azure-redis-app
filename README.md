# .NET 8 API - Deploy Automático com GitHub Actions e AKS

Este repositório contém uma aplicação ASP.NET Core 8 (API) com pipeline CI/CD usando GitHub Actions para:

- Build da imagem Docker
- Push para o GitHub Container Registry (GHCR)
- Deploy automático em um cluster **AKS (Azure Kubernetes Service)** usando **Helm**

---

## 🚀 Tecnologias

- [.NET 8](https://learn.microsoft.com/en-us/dotnet/core/whats-new/dotnet-8)
- [Docker](https://www.docker.com/)
- [GitHub Actions](https://docs.github.com/en/actions)
- [Azure Kubernetes Service (AKS)](https://learn.microsoft.com/en-us/azure/aks/)
- [Helm](https://helm.sh/)

---

## 🛠️ Estrutura do Projeto

```
.
├── .github/
│   └── workflows/
│       └── deploy.yml         # Pipeline GitHub Actions
├── helm/
│   └── chart/                 # Helm chart para deploy no AKS
│       ├── Chart.yaml
│       ├── values.yaml
│       └── templates/
├── src/
│   └── MinhaApi/              # Código-fonte .NET 8
├── Dockerfile
└── README.md
```

---

## 🔐 Secrets Necessários

No GitHub (Settings → Secrets → Actions), adicione:

| Nome                       | Descrição |
|---------------------------|-----------|
| `REGISTRY_USERNAME`       | Usuário do GitHub (ex: `jheffh2012`) |
| `REGISTRY_TOKEN`          | Token de acesso com permissão `write:packages` |
| `AZURE_CREDENTIALS`       | JSON de credenciais do Azure (`az ad sp create-for-rbac --sdk-auth`) |
| `HELM_VALUES_SECRET_SECRET_KEY` | Exemplo de valor secreto injetado no Helm |
| `HELM_VALUES_SECRET_MOUNTPATH`| Outro valor secreto |
| `HELM_VALUES_SERVICE_TYPE`| Outro valor secreto |
| `HELM_VALUES_SERVICE_PORT`| Outro valor secreto |
| `HELM_VALUES_USER_ASSIGNED_IDENTITY_ID`| Outro valor secreto |
| `HELM_VALUES_KEY_VAULT_NAME`| Outro valor secreto |
| `HELM_VALUES_TENANT_ID`| Outro valor secreto |

---

## ⚙️ Como Funciona o Deploy

1. O pipeline é disparado **quando uma tag semântica é criada** (ex: `v1.0.0`)
2. A imagem é **buildada e publicada no GHCR**
3. O Helm substitui os valores e aplica o **deploy no AKS**

---

## 🏁 Publicando uma nova versão

Para publicar uma nova versão e iniciar o deploy:

```bash
git tag v1.0.0
git push origin v1.0.0
```