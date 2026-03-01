using ApiGateway.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

/*	Descrição
 *	1 YARP Reverse Proxy = Roteia automaticamente as requisições para os microserviços usando o appsettings.json
 *	2 Autenticação JWT   = Valida tokens emitidos pelo AuthService usando a mesma chave simétrica, issuer e audience
 *	3 Controllers	     = Suporte para controllers locais do gateway (pasta Controllers/ já existente)
 *	4 OpenAPI + Bearer   = Documentação OpenAPI com security scheme Bearer configurado em todas as operações
 *	5 CORS               = Política permissiva para desenvolvimento (AllowAnyOrigin/Method/Header)
 *	6 Health Checks      = Endpoint/health para monitoramento 
*/

#region Builder configuration
//proxy
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

//jwt
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

builder.Services.AddAuthorizationBuilder().
    AddPolicy("AuthenticatedUser", policy =>
    {
        policy.RequireAuthenticatedUser();
    });

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("AuthenticatedPolicy", httpContext =>
    {
        var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
          ?? httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userId ?? httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            });
    });

    options.AddPolicy("AuthEndpointsPolicy", httpContext =>
    {
        var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: ip,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1)
            });
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType = "application/json";

        var logger = context.HttpContext.RequestServices
        .GetRequiredService<ILogger<Program>>();

        logger.LogWarning("Rate limit exceeded for {IP}",
            context.HttpContext.Connection.RemoteIpAddress);

        await context.HttpContext.Response.WriteAsync(
            JsonSerializer.Serialize(new
            {
                message = "Rate limit exceeded. Try again later."
            }),
            cancellationToken: token);
    };
});

//controllers
builder.Services.AddControllers();

//openapi
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

//cors
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

//health checks
builder.Services.AddHealthChecks();
var app = builder.Build();

#endregion 

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("DefaultCors");
app.Use(async (context, next) =>
{
    var logger = context.RequestServices
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("GatewayLogger");

    if (logger.IsEnabled(LogLevel.Information))
    {
        logger.LogInformation(
            "Incoming {Method} {Path} from {IP}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress
        );
    }

    await next();
});
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
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

        if (path.StartsWithSegments("/auth"))
        {
            await next();
            return;
        }

        // Extrai claims
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
             ?? context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var email = context.User.FindFirst("email")?.Value;

        if (!string.IsNullOrEmpty(userId))
            context.Request.Headers["X-User-Id"] = userId;

        if (!string.IsNullOrEmpty(email))
            context.Request.Headers["X-User-Email"] = email;

        await next();
    });
});


app.Run();
