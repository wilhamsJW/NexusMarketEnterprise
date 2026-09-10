using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Dá acesso aos padrões e opções do esquema JwtBearer
using Microsoft.AspNetCore.Builder; // Necessário para estender o IApplicationBuilder (Middlewares)
using Microsoft.Extensions.Configuration; // Permite ler o arquivo appsettings.json via IConfiguration
using Microsoft.Extensions.DependencyInjection; // Necessário para estender o IServiceCollection (DI Container)
using Microsoft.IdentityModel.Tokens; // Fornece as classes de chave de criptografia e parâmetros de validação de Token

namespace WebApi.Core.Identidade
{
    // A classe é 'static' para permitir a criação de Métodos de Extensão (Extension Methods) no C#
    public static class JwtConfig
    {
        // =========================================================================
        // PASSO 1: REGISTRO DOS SERVIÇOS (Invocado no Program.cs ANTES do builder.Build())
        // =========================================================================
        // O termo 'this IServiceCollection services' transforma esta função em um Método de Extensão.
        // Isso permite chamá-la no Program.cs com a sintaxe limpa: 'builder.Services.AddJwtConfiguration(configuration)'
        public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Localiza a seção chamada "AppSettings" dentro do arquivo 'appsettings.json'
            var appSettingsSection = configuration.GetSection("AppSettings");

            // 2. Mapeia a seção do arquivo para a classe POCO 'AppSettings' usando o padrão IOptions
            // Permite injetar 'IOptions<AppSettings>' em qualquer Controller/Serviço caso precise no futuro
            services.Configure<AppSettings>(appSettingsSection);

            // 3. Extrai as propriedades da seção e popula um objeto da classe AppSettings diretamente em memória.
            // Se a seção não existir no appsettings.json, interrompe a subida da API lançando uma exceção explícita
            var appSettings = appSettingsSection.Get<AppSettings>() ??
                throw new InvalidOperationException("The 'AppSettings' section has not been configured or is null in appsettings.json.");

            // 4. Converte a chave secreta (string definida no appsettings) para um array de bytes (byte[]),
            // que é o formato exigido pelos algoritmos de criptografia da biblioteca de Segurança do .NET
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            // 5. Configura o esquema de Autenticação Padrão do ASP.NET Core
            services.AddAuthentication(options =>
            {
                // Define que o mecanismo principal para saber 'Quem é o usuário' será o esquema 'Bearer' (Token JWT)
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                // Define que, quando a API pedir login/desafio de segurança, ela também exigirá o esquema 'Bearer'
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            // 6. Adiciona e configura as regras de validação do Token JWT
            }).AddJwtBearer(bearerOptions =>
            {
                // Exige que a requisição venha via protocolo seguro (HTTPS)
                bearerOptions.RequireHttpsMetadata = true;

                // Salva o Token JWT no 'HttpContext', permitindo recuperá-lo se necessário
                bearerOptions.SaveToken = true;

                // Define os critérios rígidos de segurança que OBRIGATORIAMENTE o Token enviado deve cumprir
                bearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    // Exige que a assinatura do token seja validada usando a chave simétrica definida
                    ValidateIssuerSigningKey = true,

                    // Define a chave secreta usada para verificar se o token é autêntico e não foi forjado
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    // Exige que o emissor do token seja validado (quem gerou o token)
                    ValidateIssuer = true,

                    // Exige que a audiência seja validada (para onde o token foi gerado)
                    ValidateAudience = true,

                    // Qual o valor esperado da Audiência (ex: "http://localhost", "https://meusaas.com")
                    ValidAudience = appSettings.ValidoEm,

                    // Qual o valor esperado do Emissor (ex: "https://identidade.meusaas.com")
                    ValidIssuer = appSettings.Emissor
                };
            });
        }

        // =========================================================================
        // PASSO 2: CONFIGURAÇÃO DO PIPELINE (Invocado no Program.cs DEPOIS do builder.Build())
        // =========================================================================
        // O termo 'this IApplicationBuilder app' estende a instância da aplicação pronta.
        // Permite chamar no Program.cs com: 'app.UseJwtConfiguration()'
        public static void UseJwtConfiguration(this IApplicationBuilder app)
        {
            // Ativa o Middleware de Autenticação na esteira HTTP:
            // Ele lê o cabeçalho 'Authorization: Bearer <token>', valida a assinatura com a chave configurada 
            // e popula o 'HttpContext.User' com os dados/claims que estão dentro do Token
            app.UseAuthentication();

            // Ativa o Middleware de Autorização na esteira HTTP:
            // Ele verifica se o usuário autenticado no 'UseAuthentication()' tem permissão (claims/roles)
            // para acessar o endpoint que está tentando chamar (bloqueia ou libera o acesso)
            app.UseAuthorization();
        }
    }
}