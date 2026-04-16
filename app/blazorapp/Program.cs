using blazorapp.Components;
using blazorapp.Services;
using MassTransit;
using microservices.shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure HttpClient for the sales service
builder.Services.AddHttpClient();

// Register customer client service
builder.Services.AddScoped<ICustomerClient, CustomerClient>();
builder.Services.RegisterShared();


builder.Services.AddMassTransit(conf =>
{
    conf.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("localhost", "/", h => {
            h.Username("user");
            h.Password("password");
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
