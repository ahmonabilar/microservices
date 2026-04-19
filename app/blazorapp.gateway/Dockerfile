FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the shared project
COPY shared/microservices.shared/ shared/microservices.shared/

# Copy the gateway project and restore
COPY app/blazorapp.gateway/blazorapp.gateway.csproj app/blazorapp.gateway/
RUN dotnet restore app/blazorapp.gateway/blazorapp.gateway.csproj

COPY app/blazorapp.gateway/ app/blazorapp.gateway/
RUN dotnet publish app/blazorapp.gateway/blazorapp.gateway.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "blazorapp.gateway.dll"]
