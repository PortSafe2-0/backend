using PortSafe.API.Data;
using Microsoft.EntityFrameworkCore;
using PortSafe.API.Interfaces;
using PortSafe.API.Repositories;
using PortSafe.API.Models;
using PortSafe.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models; // ✅ IMPORTANTE


var builder = WebApplication.CreateBuilder(args);
// ======================
// CORS
// ======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", 
        policy => policy.WithOrigins("http://localhost:3000", "http://localhost:8080", "http://localhost:8081")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});

// ======================
// Banco de dados
// ======================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ======================
// Injeção de dependência
// ======================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();


// Locker
builder.Services.AddScoped<ILockerRepository, LockerRepository>();
builder.Services.AddScoped<ILockerService, LockerService>();

// Delivery
builder.Services.AddScoped<IDeliveryRepository, DeliveryRepository>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();

// ======================
// JWT Authentication
// ======================
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// ======================
// Controllers
// ======================
builder.Services.AddControllers();

// ======================
// Swagger + JWT
// ======================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PortSafe API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Use: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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
                }
            },
            new string[] {}
        }
    });
});


var app = builder.Build();

// ======================
// Aplicar Migrations Automaticamente com Retry
// ======================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    int retries = 0;
    const int maxRetries = 10;
    const int delayMs = 3000;
    
    while (retries < maxRetries)
    {
        try
        {
            logger.LogInformation("Tentativa {Attempt} de {MaxRetries}: Aplicando migrations...", retries + 1, maxRetries);
            
            // Aguardar um pouco antes de tentar (se não for a primeira tentativa)
            if (retries > 0)
            {
                Thread.Sleep(delayMs);
            }
            
            dbContext.Database.Migrate();
            logger.LogInformation("✓ Migrations aplicadas com sucesso!");
            break;
        }
        catch (Exception ex)
        {
            retries++;
            logger.LogWarning(ex, "Erro ao conectar ao banco (tentativa {Attempt}): {Message}", retries, ex.Message);
            
            if (retries >= maxRetries)
            {
                logger.LogError("Falha ao aplicar migrations após {MaxRetries} tentativas", maxRetries);
                throw;
            }
        }
    }
}

// ======================
// Swagger
// ======================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ======================
// Middlewares
// ======================
app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

// ======================
// Rotas
// ======================
app.MapControllers();

app.Run();