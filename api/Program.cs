using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CORS", policy =>
    {
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowAnyOrigin();
    });
});

var authority = builder.Configuration["Keycloak:Authority"];
var clientId = builder.Configuration["Keycloak:ClientId"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authority;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new()
        {
            ValidateAudience = false,
            ValidIssuers = [authority, "http://localhost:8080/realms/master"],
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                // Map Keycloak resource_access.<clientId>.roles to ClaimTypes.Role
                var identity = context.Principal?.Identity as ClaimsIdentity;
                var resourceAccess = context.Principal?.FindFirst("resource_access");
                if (resourceAccess != null)
                {
                    using var doc = JsonDocument.Parse(resourceAccess.Value);
                    if (doc.RootElement.TryGetProperty(clientId!, out var clientAccess) &&
                        clientAccess.TryGetProperty("roles", out var roles))
                        foreach (var role in roles.EnumerateArray())
                        {
                            var roleValue = role.GetString();
                            if (roleValue != null)
                                identity?.AddClaim(new Claim(ClaimTypes.Role, roleValue));
                        }
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("users:read", policy => policy.RequireRole("users:read"));
    options.AddPolicy("users:write", policy => policy.RequireRole("users:write"));
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("CORS");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();