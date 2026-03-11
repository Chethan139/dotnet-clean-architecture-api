# ============================================================
# Stage 1: Build
# ============================================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy solution and project files first to leverage layer caching
COPY dotnet-clean-architecture-api.sln .
COPY src/Domain/Domain.csproj src/Domain/
COPY src/Application/Application.csproj src/Application/
COPY src/Infrastructure/Infrastructure.csproj src/Infrastructure/
COPY src/API/API.csproj src/API/
COPY tests/Application.Tests/Application.Tests.csproj tests/Application.Tests/

# Restore all dependencies
RUN dotnet restore dotnet-clean-architecture-api.sln

# Copy remaining source files
COPY src/ src/
COPY tests/ tests/

# Build and test
RUN dotnet build dotnet-clean-architecture-api.sln --configuration Release --no-restore
RUN dotnet test tests/Application.Tests/Application.Tests.csproj \
    --configuration Release \
    --no-build \
    --logger "trx;LogFileName=test-results.trx" \
    --collect:"XPlat Code Coverage"

# Publish the API project
RUN dotnet publish src/API/API.csproj \
    --configuration Release \
    --no-build \
    --output /app/publish

# ============================================================
# Stage 2: Runtime
# ============================================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Create non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

COPY --from=build /app/publish .

# Set file ownership
RUN chown -R appuser:appgroup /app

USER appuser

EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_URLS="http://+:8080"
ENV ASPNETCORE_ENVIRONMENT="Production"

HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "API.dll"]
