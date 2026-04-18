using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Rsk.Saml.OpenIddict.Configuration.DependencyInjection;

namespace microservices.identity.Extensions;

public static class WebApplicationExtensions
{
    extension(WebApplication webApp)
    {
        public void Configure()
        {
            if (webApp.Environment.IsDevelopment())
            {
                webApp.UseDeveloperExceptionPage();
                webApp.UseMigrationsEndPoint();
            }
            else
            {
                webApp.UseStatusCodePagesWithReExecute("~/error");
                //webApp.UseExceptionHandler("~/error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //webApp.UseHsts();
            }

            webApp.UseHttpsRedirection();
            webApp.UseStaticFiles();

            webApp.UseRouting();

            webApp.UseAuthentication();
            webApp.UseAuthorization();

            webApp.UseOpenIddictSamlPlugin();

            webApp.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}