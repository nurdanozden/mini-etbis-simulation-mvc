# Build asamasi
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Once sadece .csproj kopyala, restore et (layer cache icin)
COPY MiniETBIS/MiniETBIS.csproj MiniETBIS/
RUN dotnet restore MiniETBIS/MiniETBIS.csproj

# Sonra tum kaynak kodunu kopyala ve publish et
COPY MiniETBIS/ MiniETBIS/
RUN dotnet publish MiniETBIS/MiniETBIS.csproj -c Release -o /app/publish

# Calistirma asamasi
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render varsayilan olarak PORT=10000 atar
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "MiniETBIS.dll"]
