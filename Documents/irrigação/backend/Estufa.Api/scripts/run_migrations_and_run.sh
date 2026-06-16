#!/usr/bin/env bash
set -e
MIGRATION_NAME=${1:-InitialCreate}

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR/.."

if [ -n "$2" ]; then
  export ConnectionStrings__DefaultConnection="$2"
  echo "Using provided connection string from arg."
fi
if [ -n "$3" ]; then
  export Jwt__Key="$3"
  echo "Using provided Jwt__Key from arg."
fi

if [ ! -d "Migrations" ]; then
  echo "No Migrations folder found — creating initial migration: $MIGRATION_NAME"
  dotnet ef migrations add "$MIGRATION_NAME"
fi

echo "Applying migrations to database..."
dotnet ef database update

echo "Starting application..."
dotnet run
