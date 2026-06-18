using AutoMapper;
using GeekShopping.ProductAPI.Config;
using GeekShopping.ProductAPI.Models.Context;
using GeekShopping.ProductAPI.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. ADICIONAR SERVIÇOS AO CONTAINER
// ============================================

// Obter connection string
var connection = builder.Configuration.GetConnectionString("MySqlConnection");

// Adicionar DbContext do MySQL
builder.Services.AddDbContext<MySQLContext>(options =>
    options.UseMySql(
        connection,
        new MySqlServerVersion(new Version(8, 4, 7))
    )
);

// Adicionar Controllers
builder.Services.AddControllers();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", jwtOptions =>
    {
        jwtOptions.Authority = "https://localhost:4435/"; // URL do IdentityServer
        jwtOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false // Desabilitar validação de audiência, pois o IdentityServer pode emitir tokens para múltiplos recursos
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "geek_shopping");
    });

// Adicionar Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GeekShopping.ProductAPI",
        Version = "v1"
    });
    c.EnableAnnotations();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});
    

//AutoMapper para mapear os objetos de domínio para os objetos de valor (VO) e vice-versa
//Encapsulamento de entidades de domínio, evitando que sejam expostas diretamente nas APIs

IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Adicionar repositório de produtos
builder.Services.AddScoped<IProductRepository, ProductRepository>();
// ============================================
// 2. CONSTRUIR A APLICAÇÃO
// ============================================

var app = builder.Build();

// ============================================
// 3. CONFIGURAR O PIPELINE DE REQUISIÇÕES
// ============================================

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger/index.html");
    return Task.CompletedTask;
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();