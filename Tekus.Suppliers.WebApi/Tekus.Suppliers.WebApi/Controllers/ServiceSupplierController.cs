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
    /// <summary>
    /// Controller for managing service suppliers.
    /// </summary>
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

        /// <summary>
        /// Retrieves a list of suppliers based on the provided filter.
        /// </summary>
        /// <param name="supplierFilterDto"></param>
        /// <returns>Retrieves a list of services with suppliers and countries</returns>
        /// <response code="200">Returns a list of suppliers</response>
        /// <response code="400">If the request is invalid</response>
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
        /// <summary>
        /// Retrieves a specific service by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retrieves a specific service</returns>
        /// <response code="200">Returns the service with the specified ID</response>
        /// <response code="400">If the service is not found</response>
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
        /// <summary>
        /// Creates a new service.
        /// </summary>
        /// <param name="serviceCreationDto"></param>
        /// <returns>New service created</returns>
        /// <response code="201">Returns the created service</response>
        /// <response code="400">If the service was not created</response>
        [HttpPost]
        public async Task<ActionResult<ResponseDto>> Post([FromBody] ServiceCreationDto serviceCreationDto)
        {
            if (serviceCreationDto == null)
            {
                return BadRequest("Service data is required.");
            }
            if (string.IsNullOrEmpty(serviceCreationDto.Name))
            {
                return BadRequest("Service name is required.");
            }
            if (serviceCreationDto.PriceHour <= 0)
            {
                return BadRequest("Service price must be greater than zero.");
            }
            
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
        /// <summary>
        /// Updates an existing service.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="serviceCreationDto"></param>
        /// <returns></returns>
        /// <response code="204">Returns no content if the update was successful</response>
        /// <response code="400">If the service ID is invalid or the update failed</response>
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
