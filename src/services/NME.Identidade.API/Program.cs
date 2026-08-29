using NME.Identidade.API.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Environment.EnvironmentName = "Staging";
builder.Configuration.AddApiConfigurationBuilder(builder.Environment);
builder.Services.AddApiConfiguration(builder.Configuration);
builder.Services.AddIdentityConfiguration(builder.Configuration);
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

app.UseApiConfiguration(app.Environment);
app.UseSwaggerConfiguration(app.Environment);

app.Run();