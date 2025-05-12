using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Domain.Interfaces;
using Tekus.Suppliers.WebApi.Infrastructure.Services;
using Tekus.Suppliers.WebApi.Application.Services;
using Tekus.Suppliers.WebApi.Domain.Common;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Tekus.Suppliers.WebApi.Infrastructure;
using System;
using Tekus.Suppliers.WebApi.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Reflection;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = " Tekus Technical Test", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token in the following format: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}
        }
    });
});

builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromDays(1);
});

// Add services to the container.
builder.Services.AddHttpContextAccessor();
//builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ICountryService, CountryService>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICountryService, CountryService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IServiceSupplierService, ServiceSupplierService>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IIndicatorService, IndicatorService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ICountryLocalService, CountryLocalService>();
builder.Services.AddScoped<ICountryLocalRepository, CountryLocalRepository>();

StaticDetails.CountryAPIBase = builder.Configuration["ServiceUrls:CountryAPI"];

builder.Services.AddDbContext<ServiceSuppliersDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderProductConnection")));

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ServiceSuppliersDBContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.MapInboundClaims = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtkey"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("isadmin", policy => policy.RequireClaim("isadmin"));
});

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseOutputCache();

app.UseAuthorization();

app.MapControllers();

ApplyMigration();

await SynchronizateCountries();

app.Run();


void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<ServiceSuppliersDBContext>();

        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}

async Task SynchronizateCountries()
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var syncService = services.GetRequiredService<IBaseService>();
        await syncService.SyncCountriesToDatabaseAsync();
    }
}
