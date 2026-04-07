# Scripts

This folder contains utility scripts for setting up and managing the project.

| Script | Description |
|---|---|
| [`setup.sh`](setup.sh) | Verifies prerequisites and sets up the local development environment |
| [`.env.template`](.env.template) | Template for environment variables — copy to `.env` in the repo root |

## Usage

```bash
# Copy environment template and fill in your Azure resource values
cp scripts/.env.template .env

# Run the setup script
bash scripts/setup.sh
```

## Environment Variables

See [`.env.template`](.env.template) for the full list of required and optional environment variables.

Key variables:

| Variable | Required | Description |
|---|---|---|
| `AZURE_OPENAI_ENDPOINT` | ✅ | Your Azure OpenAI resource endpoint |
| `AZURE_OPENAI_API_KEY` | ✅ | Your Azure OpenAI API key |
| `AZURE_OPENAI_DEPLOYMENT_NAME` | ✅ | Deployment name (e.g., `gpt-4o`) |
| `AZURE_AI_PROJECT_CONNECTION_STRING` | For Foundry demos | Connection string for your Azure AI Foundry project |
