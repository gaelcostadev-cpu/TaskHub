using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ApiGateway.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

/*	Descrição
 *	1 YARP Reverse Proxy = Roteia automaticamente as requisições para os microserviços usando o appsettings.json
 *	2 Autenticação JWT   = Valida tokens emitidos pelo AuthService usando a mesma chave simétrica, issuer e audience
 *	3 Controllers	     = Suporte para controllers locais do gateway (pasta Controllers/ já existente)
 *	4 OpenAPI + Bearer   = Documentação OpenAPI com security scheme Bearer configurado em todas as operações
 *	5 CORS               = Política permissiva para desenvolvimento (AllowAnyOrigin/Method/Header)
 *	6 Health Checks      = Endpoint/health para monitoramento 
*/


builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("Jwt settings are not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
    AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer      = jwtOptions.Issuer,
            ValidAudience    = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                                   Encoding.UTF8.GetBytes(jwtOptions.Key)),

            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AuthenticatedUser", policy =>
    {
        policy.RequireAuthenticatedUser();
    });
});



builder.Services.AddControllers();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new()
        {
            Title       = "TaskHub — API Gateway",
            Version     = "v1",
            Description = "Gateway centralizado do TaskHub. " +
                          "Roteia requisições para AuthService, TasksService e NotificationsService."
        };

        // Registra o security scheme Bearer no documento
        var securitySchemes = new Dictionary<string, IOpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type         = SecuritySchemeType.Http,
                Scheme       = "bearer",
                In           = ParameterLocation.Header,
                BearerFormat = "JWT",
                Description  = "Insira o token JWT no campo abaixo (sem o prefixo 'Bearer')"
            }
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = securitySchemes;

        // Aplica o requisito de segurança em todas as operações
        if (document.Paths is null) return Task.CompletedTask;
        foreach (var operation in document.Paths.Values
            .Where(path => path.Operations is not null)
            .SelectMany(path => path.Operations!))
        {
            operation.Value.Security ??= [];
            operation.Value.Security.Add(new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        }

        return Task.CompletedTask;
    });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


builder.Services.AddHealthChecks();
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("DefaultCors");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapGet("/", () => Results.Ok(new
{
    Service     = "TaskHub API Gateway",
    Status      = "Running",
    Timestamp   = DateTime.UtcNow
}));
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        var path = context.Request.Path;

        // Permite auth sem token
        if (path.StartsWithSegments("/auth"))
        {
            await next();
            return;
        }

        // Exige autenticação para qualquer outra rota
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next();
    });
});


app.Run();
