using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc"; // triggers redirect to OpenIddict
    })
    .AddCookie("cookie")
    .AddOpenIdConnect(
        "oidc",
        options =>
        {
            options.Authority = "https://localhost:5003"; // your OpenIddict server
            options.ClientId = "web_client";

            options.ResponseType = "code";

            options.UsePkce = true;
            options.SaveTokens = true;

            options.Scope.Add("api");

            options.RequireHttpsMetadata = false; // dev only

            options.CallbackPath = "/signin-oidc";
        }
    );

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.MapGet(
    "/logout",
    async (HttpContext context) =>
    {
        await context.SignOutAsync("cookie");

        return Results.Redirect("/");
    }
);

app.Run();
