FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY services/identity/src/microservices.identity.OpenIddict/microservices.identity.OpenIddict.csproj services/identity/src/microservices.identity.OpenIddict/
RUN dotnet restore services/identity/src/microservices.identity.OpenIddict/microservices.identity.OpenIddict.csproj

COPY services/identity/src/microservices.identity.OpenIddict/ services/identity/src/microservices.identity.OpenIddict/
RUN dotnet publish services/identity/src/microservices.identity.OpenIddict/microservices.identity.OpenIddict.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "microservices.identity.OpenIddict.dll"]
