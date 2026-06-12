# AI Outfit Stylist

AI Outfit Stylist is a production-ready full-stack starter for an AI-powered styling platform. Users register, upload a photo, receive AI fashion analysis, generate three complete budget-aware outfits, and save or delete outfit history.

## Solution Folder Structure

```text
AIOutfitStylist.slnx
NuGet.Config
docs/
  database-design.md
  deployment.md
src/
  AIOutfitStylist.Api/
    Controllers/
    Middleware/
    Program.cs
    appsettings.json
  AIOutfitStylist.Application/
    Common/
    DTOs/
    Interfaces/
    Validation/
  AIOutfitStylist.Domain/
    Common/
    Entities/
    Enums/
  AIOutfitStylist.Infrastructure/
    Migrations/
    Options/
    Persistence/
    Services/
frontend/
  src/
    lib/
    pages/
    ui/
    main.tsx
    router.tsx
    styles.css
```

## Backend

- ASP.NET Core 9 Web API
- Clean Architecture layers
- Entity Framework Core with SQL Server
- Repository pattern and service layer
- JWT authentication
- Swagger/OpenAPI
- FluentValidation
- Azure Blob Storage abstraction
- Global exception handling
- OpenAI-ready AI stylist service

## Frontend

- React 19
- TypeScript
- Vite
- Tailwind CSS
- React Query
- React Hook Form
- Responsive SaaS UI
- Dark mode
- Loading and toast states

## Local Setup

1. Configure `src/AIOutfitStylist.Api/appsettings.json`.
2. Set `Jwt:SigningKey` to a strong secret.
3. Set `OpenAI:ApiKey` for live AI calls.
4. Set `AzureBlobStorage:ConnectionString` for Azure Blob uploads.
5. Run database migration:

```bash
dotnet ef database update --project src/AIOutfitStylist.Infrastructure --startup-project src/AIOutfitStylist.Api
```

6. Start the API:

```bash
dotnet run --project src/AIOutfitStylist.Api
```

7. Start the frontend:

```bash
cd frontend
npm install
npm run dev
```

## API Endpoints

- `POST /api/auth/register`
- `POST /api/auth/login`
- `GET /api/auth/profile`
- `PUT /api/auth/profile`
- `POST /api/photos/upload`
- `POST /api/analysis/analyze-photo`
- `POST /api/outfits/generate`
- `GET /api/outfits/history`
- `POST /api/outfits/save`
- `DELETE /api/outfits/{id}`

## Product Search

`ProductSearchService` returns normalized products from Amazon, Walmart, Target, H&M, and Old Navy. The included implementation is deterministic and affiliate-link ready; replace the search method internals with official retailer, affiliate, or commerce APIs for production.
