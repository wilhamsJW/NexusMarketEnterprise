using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NME.Identidade.API.Data;

namespace NME.Identidade.API
{
    // Classe principal do projeto onde o processo da aplicação é iniciado
    public class Program
    {
        // Método 'Main': É o ponto de entrada (entry point) que o C# executa ao ligar a API
        public static void Main(string[] args)
        {
            // Cria o 'builder', que é o construtor da aplicação web. 
            // Ele carrega as configurações iniciais do appsettings.json, variáveis de ambiente, etc.
            var builder = WebApplication.CreateBuilder(args);

            // =========================================================================
            // SEÇÃO 1: REGISTRO DE SERVIÇOS (INJEÇÃO DE DEPENDÊNCIA)
            // Tudo o que fica aqui é adicionado no contêiner de serviços antes do Build.
            // =========================================================================

            // Configuração do DbContext com SQL Server apontando para a ConnectionString "DefaultConnection"
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Configuração do ASP.NET Core Identity com Roles, EF Stores e Token Providers
            builder.Services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Adiciona o suporte para Controllers (para que a API reconheça suas rotas em [ApiController])
            builder.Services.AddControllers();

            // Configura o explorador de endpoints necessário para mapear as rotas no Swagger
            builder.Services.AddEndpointsApiExplorer();

            // Adiciona e gera a documentação do Swagger para você testar a API no navegador
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "NME Identidade API",
                    Version = "v1",
                    Description = "API de Identidade do Nexus Market Enterprise",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact
                    {
                        Name = "Nexus Market Enterprise",
                        Email = "contato@nexusmarket.com"
                    }
                });
            });

            // =========================================================================
            // SEÇÃO 2: CONSTRUÇÃO DA APLICAÇÃO (A LINHA DIVISÓRIA)
            // Aqui o .NET "fecha a caixa" de serviços e constrói a aplicação executável (app)
            // =========================================================================
            var app = builder.Build();

            // =========================================================================
            // SEÇÃO 3: PIPELINE HTTP (MIDDLEWARES)
            // Define o caminho que cada requisição HTTP fará quando chegar na API
            // =========================================================================

            // Verifica se a aplicação está rodando em ambiente de Desenvolvimento
            if (app.Environment.IsDevelopment())
            {
                // Habilita a geração dos dados em formato JSON do Swagger
                app.UseSwagger();

                // Habilita a interface visual gráfica do Swagger no navegador para fazer testes
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NME Identidade API v1");
                    c.RoutePrefix = "swagger"; // Swagger UI accessible at /swagger
                });
            }

            // Força o redirecionamento automático de chamadas HTTP não seguras para HTTPS (criptografado)
            app.UseHttpsRedirection();

            // Identifica quem é o usuário (Autenticação vem SEMPRE antes de Autorização)
            app.UseAuthentication();

            // Aplica as regras de autorização nas rotas (só deixa passar quem tem permissão, se aplicável)
            app.UseAuthorization();

            // Mapeia os métodos das Controllers para responderem nas URLs/rotas correspondentes
            app.MapControllers();

            // Inicia o servidor web e coloca a API para rodar escutando requisições na rede
            app.Run();
        }
    }
}