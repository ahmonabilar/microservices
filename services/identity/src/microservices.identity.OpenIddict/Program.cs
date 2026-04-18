using System;
using microservices.identity.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

app.Configure();

using (var scope = app.Services.CreateScope())
{
    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    // Register identity_client (microservice.identity.client)
    var existingIdentityClient = await manager.FindByClientIdAsync("identity_client");
    if (existingIdentityClient != null)
    {
        await manager.DeleteAsync(existingIdentityClient);
    }

    await manager.CreateAsync(
        new OpenIddictApplicationDescriptor
        {
            ClientId = "identity_client",

            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,

            DisplayName = "Identity Client App",

            RedirectUris =
            {
                new Uri("https://localhost:5002/signin-oidc"),
            },

            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:5002/signout-callback-oidc"),
            },

            Permissions =
            {
                // Endpoints
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                // Grant types
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                // Response type
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                // Scopes
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Prefixes.Scope + "api",
            },

            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange,
            },
        }
    );

    // Register blazorapp_client (blazorapp)
    var existingBlazorClient = await manager.FindByClientIdAsync("blazorapp_client");
    if (existingBlazorClient != null)
    {
        await manager.DeleteAsync(existingBlazorClient);
    }

    await manager.CreateAsync(
        new OpenIddictApplicationDescriptor
        {
            ClientId = "blazorapp_client",

            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,

            DisplayName = "Blazor App",

            RedirectUris =
            {
                new Uri("https://localhost:7219/signin-oidc"),
            },

            PostLogoutRedirectUris =
            {
                new Uri("https://localhost:7219/signout-callback-oidc"),
            },

            Permissions =
            {
                // Endpoints
                OpenIddictConstants.Permissions.Endpoints.Authorization,
                OpenIddictConstants.Permissions.Endpoints.EndSession,
                OpenIddictConstants.Permissions.Endpoints.Token,
                // Grant types
                OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                // Response type
                OpenIddictConstants.Permissions.ResponseTypes.Code,
                // Scopes
                OpenIddictConstants.Permissions.Scopes.Profile,
                OpenIddictConstants.Permissions.Scopes.Email,
                OpenIddictConstants.Permissions.Scopes.Roles,
                OpenIddictConstants.Permissions.Prefixes.Scope + "api",
            },

            Requirements =
            {
                OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange,
            },
        }
    );

    // Register the "api" scope so OpenIddict recognizes it
    var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

    if (await scopeManager.FindByNameAsync("api") == null)
    {
        await scopeManager.CreateAsync(
            new OpenIddictScopeDescriptor { Name = "api", DisplayName = "API access" }
        );
    }
}

app.Run();
