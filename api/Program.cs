using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using WebAPIApplication;

var builder = WebApplication.CreateBuilder(args);

// CORS: Entweder AllowAnyOrigin ODER AllowCredentials
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

var domain = $"https://{builder.Configuration["Keycloak:Domain"]}/";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = domain;
        options.Audience = builder.Configuration["Keycloak:Audience"];
    });

builder.Services.AddAuthorization(options =>
    options.AddPolicy("users:read", policy =>
        policy.Requirements.Add(new HasScopeRequirement("users:read", domain))));
builder.Services.AddAuthorization(options =>
    options.AddPolicy("users:write", policy =>
        policy.Requirements.Add(new HasScopeRequirement("users:write", domain))));

builder.Services.AddControllers();
builder.Services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();

var app = builder.Build();

// Environment-Check korrigiert
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
app.MapControllers(); // Moderner Ansatz statt UseRouting/UseEndpoints

app.Run();