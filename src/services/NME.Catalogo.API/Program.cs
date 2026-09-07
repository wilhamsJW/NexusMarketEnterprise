using NME.Catalogo.API.Configurations;

namespace NME.Catalogo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddApiConfiguration();
            builder.Services.AddDependencyInjectionConfiguration(builder.Configuration);
            builder.Services.AddJwtConfiguration(builder.Configuration, builder.Environment);
            builder.Services.AddSwaggerConfiguration();

            var app = builder.Build();

            app.UseSwaggerConfiguration();
            app.UseApiConfiguration(app.Environment);

            app.Run();
        }
    }
}
