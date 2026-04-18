FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy and pack the shared project as a local NuGet package
COPY shared/microservices.shared/ shared/microservices.shared/
RUN dotnet pack shared/microservices.shared/microservices.shared.csproj -c Release -o /nuget-local

# Copy the sales project and restore with local NuGet source
COPY services/sales/src/sales/sales.csproj services/sales/src/sales/
RUN dotnet nuget add source /nuget-local --name local
RUN dotnet restore services/sales/src/sales/sales.csproj

COPY services/sales/src/sales/ services/sales/src/sales/
RUN dotnet publish services/sales/src/sales/sales.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "sales.dll"]