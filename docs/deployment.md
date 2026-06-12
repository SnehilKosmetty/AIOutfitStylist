# Deployment Instructions

## Azure Resources

Create these resources:

- Azure App Service for `AIOutfitStylist.Api`
- Azure Static Web Apps or Azure App Service for the React frontend
- Azure SQL Database
- Azure Blob Storage container named `user-photos`
- Application Insights
- Azure Key Vault for secrets

## API Configuration

Set these App Service settings:

```text
ConnectionStrings__DefaultConnection=<azure-sql-connection-string>
Jwt__Issuer=AIOutfitStylist
Jwt__Audience=AIOutfitStylist
Jwt__SigningKey=<strong-64-character-secret>
OpenAI__ApiKey=<openai-api-key>
OpenAI__VisionModel=gpt-4.1-mini
OpenAI__ChatModel=gpt-4.1-mini
AzureBlobStorage__ConnectionString=<storage-connection-string>
AzureBlobStorage__ContainerName=user-photos
Cors__AllowedOrigins__0=<frontend-origin>
```

## Database

Run migrations from CI/CD or a deployment workstation:

```bash
dotnet ef database update --project src/AIOutfitStylist.Infrastructure --startup-project src/AIOutfitStylist.Api
```

## Frontend Configuration

Set:

```text
VITE_API_BASE_URL=https://<api-app-name>.azurewebsites.net
```

Build and deploy:

```bash
cd frontend
npm ci
npm run build
```

Deploy `frontend/dist` to Azure Static Web Apps or a static-capable App Service.

## Production Hardening

- Move secrets to Key Vault.
- Enable managed identity for App Service.
- Restrict SQL firewall access.
- Enable Blob private containers and serve photos through signed URLs if privacy requirements demand it.
- Replace deterministic product search with official retailer or affiliate APIs.
- Replace fallback AI analysis/generation internals with OpenAI structured JSON calls.
- Enable Application Insights alerts for error rate, latency, and dependency failures.
