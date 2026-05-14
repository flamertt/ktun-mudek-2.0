# ── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS builder

WORKDIR /src

# Solution + proje dosyalarını önce kopyala (restore cache katmanı)
COPY BitirmeApi/BitirmeApi.sln ./
COPY BitirmeApi/BitirmeApi.Entity/BitirmeApi.Entity.csproj            BitirmeApi.Entity/
COPY BitirmeApi/BitirmeApi.Core/BitirmeApi.Core.csproj                BitirmeApi.Core/
COPY BitirmeApi/BitirmeApi.DataAccess/BitirmeApi.DataAccess.csproj    BitirmeApi.DataAccess/
COPY BitirmeApi/BitirmeApi.Business/BitirmeApi.Business.csproj        BitirmeApi.Business/
COPY BitirmeApi/BitirmeApi.Presentation/BitirmeApi.Presentation.csproj BitirmeApi.Presentation/

RUN dotnet restore BitirmeApi.Presentation/BitirmeApi.Presentation.csproj

# Geri kalan kaynak kodunu kopyala
COPY BitirmeApi/BitirmeApi.Entity/            BitirmeApi.Entity/
COPY BitirmeApi/BitirmeApi.Core/              BitirmeApi.Core/
COPY BitirmeApi/BitirmeApi.DataAccess/        BitirmeApi.DataAccess/
COPY BitirmeApi/BitirmeApi.Business/          BitirmeApi.Business/
COPY BitirmeApi/BitirmeApi.Presentation/      BitirmeApi.Presentation/

RUN dotnet publish BitirmeApi.Presentation/BitirmeApi.Presentation.csproj \
    -c Release -o /app/publish --no-restore

# ── Stage 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app
COPY --from=builder /app/publish .

# .NET 8 konteynerde varsayılan port 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "BitirmeApi.Presentation.dll"]
