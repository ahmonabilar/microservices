using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace microservices.identity.Extensions;

public static class ConfigurationExtension
{
    extension(IConfiguration configuration)
    {
        public void GetDbConnection(DbContextOptionsBuilder optBuilder, string settingName = "OpenIddictConnectionString")
        {
            var openIddictConnectionString = configuration.GetValue<string>(settingName);
            var dbProvider = configuration.GetValue<string>("DbProvider");
            var migrationAssembly = typeof(WebApplicationBuilderExtensions).GetTypeInfo().Assembly.GetName().Name;
            
            switch (dbProvider)
            {
                case "SqlServer":
                    optBuilder.UseSqlServer(openIddictConnectionString, options => options.MigrationsAssembly(migrationAssembly));
                    break;
                case "MySql":
                    throw new NotSupportedException("Temporarily out of support in AdminUI until Pomelo EF provider is updated to support .NET 10.");
                // optBuilder.UseMySQL(openIddictConnectionString, options => options.MigrationsAssembly(migrationAssembly));
                // break;
                case "PostgreSql":
                    optBuilder.UseNpgsql(openIddictConnectionString, options => options.MigrationsAssembly(migrationAssembly));
                    break;
                default:
                    throw new NotSupportedException($"{dbProvider} is not a supported database provider.");
            }
        }
    }
}