using IdentityExpress.Manager.BusinessLogic.OpenIddict.Configuration;
using IdentityExpress.Manager.UI.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddAdminUI(options =>
{
    options.MigrationOptions = OpenIddictMigrationOptions.All;
});

var app = builder.Build();

app.UseAdminUI();

app.Run();