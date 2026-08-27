using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NME.Identidade.API.Data;
using NME.Identidade.API.Extensions;
using NME.Identidade.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Busca o bloco "AppSettings" dentro do arquivo appsettings.json
var appSettingsSection = builder.Configuration.GetSection("AppSettings");

// 2. Registra no container do .NET para injetar (IOptions<AppSettings>) em Controllers e Serviços
builder.Services.Configure<AppSettings>(appSettingsSection);

// 3. Converte o JSON em um objeto C# vivo para usar as propriedades aqui mesmo no Program.cs
var appSettings = appSettingsSection.Get<AppSettings>()
    ?? throw new InvalidOperationException("Seção AppSettings não configurada no appsettings.json");

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure JWT Authentication
var key = Encoding.UTF8.GetBytes(appSettings.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(bearerOptions =>
{
    // Exige HTTPS para trafegar o token com segurança em produção
    bearerOptions.RequireHttpsMetadata = true;

// Guarda o token no HttpContext após validar (permite ler o token na controller se precisar)
bearerOptions.SaveToken = true;

    // Conjunto de regras que o .NET vai usar para validar cada requisição
    bearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        // Força o .NET a validar se a assinatura do token bate com a nossa chave secreta
        ValidateIssuerSigningKey = true,

        // Converte a Secret (string) em uma chave simétrica de criptografia para decodificar o token
        IssuerSigningKey = new SymmetricSecurityKey(key),

        // Obrega a verificação de quem emitia (gerou) o token
        ValidateIssuer = true,

        // Obriga a verificação de para quem o token foi emitido (destinatário)
        ValidateAudience = true,

        // O valor exato que o token PRECISA ter no campo Audience para ser aceito (veio do appsettings)
        ValidAudience = appSettings.ValidoEm,

        // O valor exato que o token PRECISA ter no campo Issuer para ser aceito (veio do appsettings)
        ValidIssuer = appSettings.Emissor
    };
});

// Register JWT Service
builder.Services.AddScoped<JwtService>();

// Add Controllers
builder.Services.AddControllers();

// Add API Explorer
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI
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

    // Configuração para autenticação JWT no Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Insira o token JWT desta maneira: Bearer {seu token}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NME Identidade API v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();