using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace NME.Identidade.API.Configuration
{
    public static class ConfigurationBuilderConfig
    {
        public static IConfigurationBuilder AddApiConfigurationBuilder(
            this IConfigurationBuilder builder,
            IWebHostEnvironment env)
        {
            builder
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Program>(optional: true);
            }

            return builder;
        }
    }
}