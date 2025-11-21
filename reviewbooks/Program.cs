using ReviewBooks.Users.Repository;
using ReviewBooks.Users.Services;
using ReviewBooks.Data;
using Microsoft.EntityFrameworkCore;
using ReviewBooks.Books.Services;
using ReviewBooks.Books.Repository;
using ReviewBooks.Reviews.Services;
using ReviewBooks.Reviews.Repository;
using ReviewBooks.Auth.Services;
using ReviewBooks.Auth.Repository;
using ReviewBooks.Forum.Service;
using ReviewBooks.Forum.Repository;
using ReviewBooks.Favorite.Services;
using ReviewBooks.Favorite.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Npgsql;
using DotNetEnv;
var builder = WebApplication.CreateBuilder(args);
Env.Load();
var bookApiKey = Environment.GetEnvironmentVariable("BOOK_API_KEY") ?? "";
var rawConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
var connectionString = rawConnection
    .Replace("${DB_HOST}", Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost")
    .Replace("${DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME") ?? "reviewbooks")
    .Replace("${DB_USER}", Environment.GetEnvironmentVariable("DB_USER") ?? "postgres")
    .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "");

var builderDb = new NpgsqlConnectionStringBuilder(connectionString);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

// Read JWT Key from environment variable or appsettings.json
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")?.Split('\n', '\r')[0].Trim()
    ?? builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT Key is not configured. Set JWT_KEY environment variable or Jwt:Key in appsettings.json");
}

// Read JWT Issuer and Audience from environment or appsettings
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Jwt:Issuer"];
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(jwtKey);

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCORS",
        policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
    );
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Enter your token in the text input below."
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IForumService, ForumService>();
builder.Services.AddScoped<IForumRepository, ForumRepository>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();
builder.Services.AddHttpClient();

var app = builder.Build();

var apiUrl = Environment.GetEnvironmentVariable("BackendUrl") ?? "http://localhost:8080/swagger/index.html";
Console.WriteLine($"ReviewBooks API listening on: {apiUrl}");

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Production"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowCORS");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
