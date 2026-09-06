namespace NME.Catalogo.API.Configurations
{
    public static class ApiConfig
    {
        private const string CorsPolicyName = "Total";

        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Permissive CORS policy: any origin, method, and header
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicyName, policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            return services;
        }

        public static IApplicationBuilder UseApiConfiguration(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseRouting();
            app.UseCors(CorsPolicyName);
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());

            return app;
        }
    }
}