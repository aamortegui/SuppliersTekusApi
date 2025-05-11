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

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromDays(30);
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
StaticDetails.CountryAPIBase = builder.Configuration["ServiceUrls:CountryAPI"];

builder.Services.AddDbContext<ServiceSuppliersDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrderProductConnection")));

builder.Services.AddControllers();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseOutputCache();

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
