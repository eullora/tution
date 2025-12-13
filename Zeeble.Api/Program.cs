using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Zeeble.Api.Config;
using Zeeble.Api.Middleware;
using Zeeble.Api.Services;
using Zeeble.Shared.Helpers;

var builder = WebApplication.CreateBuilder(args);

var issuer = builder.Configuration["JwtConfig:Issuer"];
var jwtSecret = builder.Configuration["JwtConfig:Secret"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
                    };
                });


builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<WebApiDBContext>(x => x.UseSqlite(connectionString));

builder.Services.AddScoped<ITokenConfig, TokenConfig>(tp => new TokenConfig(symmetricKey: jwtSecret, issuer: issuer));
builder.Services.AddScoped<ITokenService, TokenService>();

//builder.Services.AddHostedService<ResultBackGroundService>();

var app = builder.Build();

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "firebase-admin.json")),
});

#if DEBUG
app.UseHttpsRedirection();
#endif


app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthorization();
app.MapControllers();
app.Run();
