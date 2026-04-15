using System.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sales.Configuration;

namespace Sales;

/// <summary>
/// Startup configuration for the Sales project.
/// This is an alternative to using Program.cs with extension methods.
/// Use this if you prefer the traditional Startup.cs pattern.
/// </summary>
public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Configure database connection
        services.AddScoped<IDbConnection>(sp =>
            new SqlConnection(Configuration.GetConnectionString("SalesDb")
                ?? throw new InvalidOperationException("Connection string 'SalesDb' not found.")));

        // Add Sales module (services, repositories, and MassTransit consumers)
        services.AddSalesModule();

        // Configure logging
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
