using Yarp.ReverseProxy.Transforms;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var allowedOrigins = builder.Configuration["ALLOWED_ORIGINS"]?.Split(',') ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId not configured");
        options.ClientSecret = builder.Configuration["Google:ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret not configured");
        options.CallbackPath = "/auth/callback/google";
    });

builder.Services.AddAuthorization();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        builderContext.AddRequestTransform(transformContext =>
        {
            var secret = builder.Configuration["INTERNAL_SECRET"];
            if (!string.IsNullOrEmpty(secret))
            {
                transformContext.ProxyRequest.Headers.Add("X-Internal-Secret", secret);
            }

            // Extract userSlug from JWT claims and add to header
            var user = transformContext.HttpContext.User;
            var userSlug = user.FindFirst("userSlug")?.Value; // Assuming claim name is "userSlug"
            if (!string.IsNullOrEmpty(userSlug))
            {
                transformContext.ProxyRequest.Headers.Add("X-User-Slug", userSlug);
            }

            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.MapGet("/auth/login/google", () => Results.Challenge(new AuthenticationProperties { RedirectUri = "/auth/callback/google" }, [GoogleDefaults.AuthenticationScheme]));

app.MapGet("/auth/callback/google", async (HttpContext ctx) =>
{
    var result = await ctx.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
    if (!result.Succeeded)
    {
        return Results.BadRequest(new { error = "Authentication failed" });
    }

    var claims = result.Principal!.Claims;
    var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
    var googleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(name))
    {
        return Results.BadRequest(new { error = "Required user information not provided" });
    }

    // Generate userSlug from email (simplified)
    var userSlug = email.Replace("@", "-").Replace(".", "-");

    // Generate JWT
    var tokenHandler = new JwtSecurityTokenHandler();
    var config = ctx.RequestServices.GetRequiredService<IConfiguration>();
    var key = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? "YourSuperSecretKeyHere12345678901234567890");

    var jwtClaims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, userSlug),
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim("userSlug", userSlug),
        new Claim("name", name),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(jwtClaims),
        Expires = DateTime.UtcNow.AddHours(1),
        Issuer = config["Jwt:Issuer"] ?? "SmartEscape-Planner",
        Audience = config["Jwt:Audience"] ?? "SmartEscape-Users",
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    var accessToken = tokenHandler.WriteToken(token);

    // Clean up authentication
    await ctx.SignOutAsync(GoogleDefaults.AuthenticationScheme);

    return Results.Json(new
    {
        access_token = accessToken,
        token_type = "Bearer",
        expires_in = 3600,
        user_slug = userSlug,
        user_info = new { email, name }
    });
});
