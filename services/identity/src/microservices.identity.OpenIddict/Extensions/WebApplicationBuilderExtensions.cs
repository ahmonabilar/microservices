using IdentityExpress.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using microservices.identity.Data;
using Rsk.Saml.Configuration;
using Rsk.Saml.OpenIddict.AspNetCore.Identity.Configuration.DependencyInjection;
using Rsk.Saml.OpenIddict.Configuration.DependencyInjection;
using Rsk.Saml.OpenIddict.EntityFrameworkCore.Configuration.DependencyInjection;
using Rsk.Saml.OpenIddict.Quartz.Configuration.DependencyInjection;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace microservices.identity.Extensions;

public static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder webAppBuilder)
    {
        public void ConfigureServices()
        {
            webAppBuilder.Services.AddControllersWithViews();
            webAppBuilder.Services.AddRazorPages();

            webAppBuilder.Services.AddDbContext<IdentityDbContext>(opt =>
                webAppBuilder.Configuration.GetDbConnection(opt));
            webAppBuilder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                webAppBuilder.Configuration.GetDbConnection(options);

                // Register the entity sets needed by OpenIddict.
                // Note: Use the generic overload if you need
                // to replace the default OpenIddict entities.
                options.UseOpenIddict();
            });

            webAppBuilder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Register the Identity webAppBuilder.Services.
            webAppBuilder.Services.AddIdentity<ApplicationUser, IdentityExpressRole>()
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
            // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
            webAppBuilder.Services.AddQuartz(options =>
            {
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
            webAppBuilder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            webAppBuilder.Services.AddOpenIddict()

                // Register the OpenIddict core components.
                .AddCore(options =>
                {
                    // Configure OpenIddict to use the Entity Framework Core stores and models.
                    // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                    options.UseEntityFrameworkCore()
                        .UseDbContext<ApplicationDbContext>();

                    // Enable Quartz.NET integration.
                    options.UseQuartz();
                })

                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                    options.DisableAccessTokenEncryption();

                    // Enable the authorization, logout, token and userinfo endpoints.
                    options
                        //Authorisation Endpoints
                        .SetAuthorizationEndpointUris("connect/authorize")
                        .SetEndSessionEndpointUris("connect/logout")
                        //Device Endpoints
                        .SetDeviceAuthorizationEndpointUris("connect/device")
                        .SetEndUserVerificationEndpointUris("connect/verify")
                        //Shared Endpoints
                        .SetTokenEndpointUris("connect/token")
                        .SetUserInfoEndpointUris("connect/userinfo");

                    // Mark the "email", "profile" and "roles" scopes as supported scopes.
                    options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles);

                    options.AllowAuthorizationCodeFlow()
                        .AllowRefreshTokenFlow()
                        .AllowClientCredentialsFlow()
                        .AllowDeviceAuthorizationFlow();

                    // Register the signing and encryption credentials.
                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    options.UseAspNetCore()
                        .DisableTransportSecurityRequirement()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableEndSessionEndpointPassthrough()
                        .EnableTokenEndpointPassthrough()
                        .EnableUserInfoEndpointPassthrough()
                        .EnableStatusCodePagesIntegration();


                    options.AddSamlPlugin(builder =>
                    {
                        builder.UseSamlEntityFrameworkCore()
                            .AddSamlArtifactDbContext(opt => webAppBuilder.Configuration.GetDbConnection(opt))
                            .AddSamlConfigurationDbContext(opt => webAppBuilder.Configuration.GetDbConnection(opt))
                            .AddSamlMessageDbContext(opt => webAppBuilder.Configuration.GetDbConnection(opt));

                        builder.ConfigureSamlOpenIddictServerOptions(serverOptions =>
                        {
                            serverOptions.HostOptions = new SamlHostUserInteractionOptions()
                            {
                                LoginUrl = "/Identity/Account/Login",
                                LogoutUrl = "/connect/logout",
                            };

                            var licensee = webAppBuilder.Configuration.GetValue<string>("SAML2PLicensee");
                            var license = webAppBuilder.Configuration.GetValue<string>("SAML2PLicense");
                            serverOptions.IdpOptions = new SamlIdpOptions()
                            {
                                Licensee = licensee,
                                LicenseKey = license,
                            };
                        });

                        builder.PruneSamlMessages();

                        builder.AddSamlAspIdentity<ApplicationUser>();
                    });
                })

                // Register the OpenIddict validation components.
                .AddValidation(options =>
                {
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });
        }

    }
}