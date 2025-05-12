using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Domain.Entities;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Controllers
{
    [Route("api/service-supplier")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isadmin")]
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
                    CountryId = cf.CountryId
                    
                }).ToList(),
                SupplierServices = s.SupplierServices.Select(ss => new SupplierServiceDto
                {
                    SupplierId = ss.SupplierId
                    
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
        
        [HttpPost]
        public async Task<ActionResult<ResponseDto>> Post([FromBody] ServiceCreationDto serviceCreationDto)
        {
            var serviceCreated = await _serviceSupplier.CreateServiceAsync(serviceCreationDto);
            if (!serviceCreated.IsSuccess || serviceCreated.Result == null)
            {
                return BadRequest(serviceCreated);
            }
            var serviceEntity = serviceCreated.Result as Service;

            if (serviceEntity == null)
            {
                return BadRequest("Service creation failed or returned an unexpected result.");
            }

            var serviceResponse = new ServiceResponseDto
            {
                Id = serviceEntity.Id,
                Name = serviceEntity.Name,
                PriceHour = serviceEntity.PriceHour,
                ServiceCountries = serviceEntity.ServiceCountries.Select(sc => new ServiceCountryDto
                {
                    CountryId = sc.CountryId                    
                }).ToList(),
                SupplierServices = serviceEntity.SupplierServices.Select(ss => new SupplierServiceDto
                {
                    SupplierId = ss.SupplierId                    
                }).ToList()
            };
            await _outputCacheStore.EvictByTagAsync(cacheTag, default);
            return CreatedAtRoute("GetServiceById", new { id = serviceResponse.Id }, serviceResponse);
        }
        
        [HttpPut("{id}", Name = "Edit-Service")]
        public async Task<ActionResult> Put(Guid id, [FromBody] ServiceCreationDto serviceCreationDto)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid service ID.");
            }
            if (serviceCreationDto == null)
            {
                return BadRequest("Service data is required.");
            }

            await _serviceSupplier.UpdateServiceAsync(id, serviceCreationDto);

            await _outputCacheStore.EvictByTagAsync(cacheTag, default);

            return NoContent();
        }
    }
}
