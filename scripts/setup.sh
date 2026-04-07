#!/usr/bin/env bash
# setup.sh — Project setup script for azure-demos-evaluation-options
#
# This script verifies prerequisites and sets up the local environment
# for running the AI evaluation demos.

set -euo pipefail

# ─── Colors ────────────────────────────────────────────────────────────────────
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

info()    { echo -e "${GREEN}[INFO]${NC} $*"; }
warn()    { echo -e "${YELLOW}[WARN]${NC} $*"; }
error()   { echo -e "${RED}[ERROR]${NC} $*"; exit 1; }

# ─── Check Prerequisites ────────────────────────────────────────────────────────
info "Checking prerequisites..."

# Check .NET SDK
if ! command -v dotnet &>/dev/null; then
    error ".NET SDK not found. Install from https://dotnet.microsoft.com/download/dotnet/9.0"
fi

DOTNET_VERSION=$(dotnet --version)
info ".NET SDK found: $DOTNET_VERSION"

# Check Azure CLI
if ! command -v az &>/dev/null; then
    warn "Azure CLI not found. Install from https://learn.microsoft.com/en-us/cli/azure/install-azure-cli"
else
    AZ_VERSION=$(az version --query '"azure-cli"' -o tsv 2>/dev/null || echo "unknown")
    info "Azure CLI found: $AZ_VERSION"
fi

# ─── Load Environment Variables ────────────────────────────────────────────────
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ROOT_DIR="$(dirname "$SCRIPT_DIR")"
ENV_FILE="$ROOT_DIR/.env"

if [[ -f "$ENV_FILE" ]]; then
    info "Loading environment variables from .env..."
    # shellcheck disable=SC1090
    set -a && source "$ENV_FILE" && set +a
else
    warn ".env file not found at $ENV_FILE"
    warn "Copy $SCRIPT_DIR/.env.template to $ROOT_DIR/.env and fill in your values."
fi

# ─── Restore NuGet Packages ─────────────────────────────────────────────────────
if find "$ROOT_DIR/src" -name "*.csproj" -print -quit | grep -q .; then
    info "Restoring NuGet packages..."
    dotnet restore "$ROOT_DIR/src"
    info "Restore complete."
else
    warn "No .csproj files found in src/. Skipping restore."
fi

info "Setup complete! See docs/overview.md to get started."
