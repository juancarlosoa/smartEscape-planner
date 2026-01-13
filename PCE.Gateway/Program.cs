using Yarp.ReverseProxy.Transforms;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

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
        options.Authority = "https://accounts.google.com";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "https://accounts.google.com",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? builder.Configuration["Google:ClientId"],
            ValidateLifetime = true,
        };
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

            // Extract user info from Google JWT claims and add to header
            var user = transformContext.HttpContext.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                // Google ID Token "sub" is the unique user ID
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value; 
                // Or use email if preferred for slug
                var email = user.FindFirst(ClaimTypes.Email)?.Value;

                // Using email as slug for readability as per previous logic, but sanitized
                var userSlug = email?.Replace("@", "-").Replace(".", "-");
                
                if (!string.IsNullOrEmpty(userSlug))
                {
                    transformContext.ProxyRequest.Headers.Add("X-User-Slug", userSlug);
                }
            }
            else
            {
                // Optional: Block unauthenticated requests here or let downstream handle 401?
                // For now, if not authenticated, we don't add the header, downstream will throw 401 via Extensions
            }

            return ValueTask.CompletedTask;
        });
    });

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapReverseProxy();

app.Run();
