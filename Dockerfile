# Stage 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and all project files first, then restore.
# This way Docker only re-runs restore when project files change,
# not on every source code change — significantly faster rebuilds.
COPY ["SnippetVault.slnx", "."]
COPY ["SnippetVault.API/SnippetVault.API.csproj", "SnippetVault.API/"]
COPY ["SnippetVault.Application/SnippetVault.Application.csproj", "SnippetVault.Application/"]
COPY ["SnippetVault.Domain/SnippetVault.Domain.csproj", "SnippetVault.Domain/"]
COPY ["SnippetVault.Infrastructure/SnippetVault.Infrastructure.csproj", "SnippetVault.Infrastructure/"]
COPY ["SnippetVault.Tests/SnippetVault.Tests.csproj", "SnippetVault.Tests/"]
RUN dotnet restore

# Copy all source and publish the API in Release mode
COPY . .
RUN dotnet publish "SnippetVault.API/SnippetVault.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Stage 2 — Runtime
# Only the compiled output is copied — no SDK, no source code, no build tools.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create Logs directory for Serilog's rolling file sink
RUN mkdir -p /app/Logs

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SnippetVault.API.dll"]
