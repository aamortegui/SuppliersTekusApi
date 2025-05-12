using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Controllers
{
    [Route("api/service-supplier")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isadmin")]
    public class ServiceSupplierController : ControllerBase
    {
        private readonly IServiceSupplierService _serviceSupplier;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "service";

        public ServiceSupplierController(IServiceSupplierService serviceSupplierService, IOutputCacheStore outputCacheStore)
        {
            _serviceSupplier = serviceSupplierService;
            _outputCacheStore = outputCacheStore;
        }

        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<IActionResult> GetSuppliers([FromQuery] ServiceFilterDto supplierFilterDto)
        {
            var response = await _serviceSupplier.GetAllServicesAsyc(supplierFilterDto);

            if (!response.IsSuccess || response.Result == null)
            {
                return BadRequest(response);
            }

            var supplierEntities = response.Result as List<Service>;

            var suppliers = supplierEntities?.Select(s => new ServiceResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                PriceHour = s.PriceHour,
                ServiceCountries = s.ServiceCountries.Select(cf => new ServiceCountryDto
                {
                    CountryId = cf.CountryId,
                    CommonName = cf.Country.CommonName,
                    OfficialName = cf.Country.OfficialName,
                }).ToList(),
                SupplierServices = s.SupplierServices.Select(ss => new SupplierServiceDto
                {
                    SupplierId = ss.SupplierId,
                    SupplierName = ss.Supplier.Name,
                }).ToList()
            }).ToList();

            return Ok(suppliers);
        }

        [HttpGet("{id}", Name = "GetServiceById")]
        public async Task<IActionResult> GetServiceById(Guid id)
        {
            var response = await _serviceSupplier.GetServiceByIdAsync(id);
            if (response is not null && !response.IsSuccess)
            {
                return BadRequest(new { message = response?.Message });
            }

            return Ok(response);
        }

        //TODO: Implement the POST and PUT methods
    }
}
