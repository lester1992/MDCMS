using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Components.WebAssembly.Server; // ? Added this namespace for UseBlazorFrameworkFiles
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MDCMS.Server.Data;
using MDCMS.Server.Models;
using MDCMS.Server.Services;

var builder = WebApplication.CreateBuilder(args);

// Config
builder.Services.Configure<CosmosMongoOptions>(builder.Configuration.GetSection("CosmosMongo"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));

// Mongo client
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<CosmosMongoOptions>>().Value;
    return new MongoClient(cfg.ConnectionString);
});
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IKioskRepository, KioskRepository>();
builder.Services.AddScoped<IImageAdsRepository, ImageAdsRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();

// JWT service
builder.Services.AddScoped<IJwtService, JwtService>();
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
var keyBytes = Encoding.UTF8.GetBytes(jwt.Key);

// Authentication
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
        ValidIssuer = jwt.Issuer,
        ValidateAudience = true,
        ValidAudience = jwt.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(30)
    };
});

// Add services
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddOpenApi();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseBlazorFrameworkFiles();  // ? now works

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
// Fallback for client-side Blazor routes
app.MapFallbackToFile("index.html");

app.Run();
