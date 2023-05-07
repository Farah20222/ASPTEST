using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using WebApplication100.Models;
using WebApplication100.Repository.Implementation;
using WebApplication100.Repository.Interface;
using WebApplication100.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter a valid JWT bearer token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {securityScheme, new string[] {} }
    });
});

builder.Services.AddDbContext<AssignmentDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Assignment"));
});
builder.Services.AddScoped<ITimeZoneService, TimeZoneService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<ITokenHandler, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

/// adds jwt authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        // event to check token expiry
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context => {
                if (context.ErrorDescription != null)
                {
                    if (context.ErrorDescription.Contains("expired"))
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "text/plain";

                        await context.Response.WriteAsync("Token is expired");
                        await context.Response.Body.FlushAsync();
                        return;
                    }
                }
                await Task.CompletedTask;
            }
        };
    }
);


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdmin", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "admin");
    });



    options.AddPolicy("IsCustomer", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "customer");
    });

    options.AddPolicy("IsVendor", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "vendor");
    });

    options.AddPolicy("IsAdminOrVendor", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, new string[] { "admin", "vendor" });
    });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
