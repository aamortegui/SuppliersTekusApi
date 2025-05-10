using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Domain.Interfaces;
using Tekus.Suppliers.WebApi.Infrastructure.Services;
using Tekus.Suppliers.WebApi.Application.Services;
using Tekus.Suppliers.WebApi.Domain.Common;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen();

builder.Services.AddOutputCache(options =>
{
    options.DefaultExpirationTimeSpan = TimeSpan.FromDays(30);
});

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<ICountryService, CountryService>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ICountryService, CountryService>();
StaticDetails.CountryAPIBase = builder.Configuration["ServiceUrls:CountryAPI"];

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

app.Run();
