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
using DotNetEnv;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);
Env.Load();

var rawConnection = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
var connectionString = rawConnection
    .Replace("${DB_HOST}", Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost")
    .Replace("${DB_PORT}", Environment.GetEnvironmentVariable("DB_PORT") ?? "5432")
    .Replace("${DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME") ?? "reviewbooks")
    .Replace("${DB_USER}", Environment.GetEnvironmentVariable("DB_USER") ?? "postgres")
    .Replace("${DB_PASSWORD}", Environment.GetEnvironmentVariable("DB_PASSWORD") ?? "");

var builderDb = new NpgsqlConnectionStringBuilder(connectionString);

Console.WriteLine(
    @$"[ReviewBooksService] DB: Host={builderDb.Host};Port={builderDb.Port};
    Database={builderDb.Database};Username={builderDb.Username};
    Password={(string.IsNullOrEmpty(builderDb.Password) ? "(empty)" : "******")};
    SSL Mode={builderDb.SslMode}"
);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var keyValue = jwtSettings["Key"];
    if (string.IsNullOrEmpty(keyValue))
        throw new InvalidOperationException("JWT Key missing in configuration");

    var key = Encoding.UTF8.GetBytes(keyValue);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<EmailService>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCORS",
        policy => policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:5500")
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
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Scheme = "Bearer",
        BearerFormat = "JWT",
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
builder.Services.AddHttpClient();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IForumService, ForumService>();
builder.Services.AddScoped<IForumRepository, ForumRepository>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();
builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();

var app = builder.Build();

var apiUrl = Environment.GetEnvironmentVariable("REVIEWBOOKS_API_URL") ?? "http://localhost:5072";
Console.WriteLine($"ReviewBooks API listening on: {apiUrl}");

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Production"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowCORS");

app.Run();
