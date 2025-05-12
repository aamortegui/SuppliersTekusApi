using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Newtonsoft.Json;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Controllers
{
    /// <summary>
    /// Controller for managing suppliers.
    /// </summary>
    [Route("api/supplier")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "isadmin")]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "supplier";
        public SupplierController(ISupplierService supplierService, IOutputCacheStore outputCacheStore)
        {
            _supplierService = supplierService;
            _outputCacheStore = outputCacheStore;
        }
        /// <summary>
        /// Retrieves a list of suppliers based on the provided filter criteria.
        /// </summary>
        /// <param name="supplierFilterDto"></param>
        /// <returns>Retrieves a list of suppliers filtered</returns>
        /// <response code="200">Returns a list of suppliers</response>
        /// <response code="400">If the filter criteria is invalid</response>
        [HttpGet]
        [OutputCache(Tags = [cacheTag])]
        public async Task<IActionResult> GetSuppliers([FromQuery] SupplierFilterDto supplierFilterDto)
        {
            var response = await _supplierService.GetAllSuppliersAsyc(supplierFilterDto);

            if (!response.IsSuccess || response.Result == null)
            {
                return BadRequest(response);
            }

            var supplierEntities = response.Result as List<Supplier>;
                        
            var suppliers = supplierEntities?.Select(s => new SupplierResponseDto
            {
                Id = s.Id,
                Name = s.Name,
                NIT = s.NIT,
                CustomFields = s.CustomFields.Select(cf => new CustomFieldResponseDto
                {
                    Id = cf.Id,
                    FieldName = cf.FieldName,
                    FieldValue = cf.FieldValue
                }).ToList()
            }).ToList();

            return Ok(suppliers);
        }
        /// <summary>
        /// Retrieves a supplier by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Retrieve a supplier</returns>
        /// <response code="200">Returns the supplier</response>
        /// <response code="400">If the supplier ID is invalid</response>
        [HttpGet("{id}", Name = "GetSupplierById")]
        public async Task<IActionResult> GetSupplierById(Guid id)
        {
            var response = await _supplierService.GetSupplierByIdAsync(id);
            if (response is not null && !response.IsSuccess)
            {
                return BadRequest(new { message = response?.Message });
            }           

            return Ok(response);
        }
        /// <summary>
        /// Creates a new supplier.
        /// </summary>
        /// <param name="supplierCreationDto"></param>
        /// <returns>Retrieve the created supplier info</returns>
        /// <response code="201">Returns the created supplier</response>
        /// <response code="400">If the supplier data is invalid</response>
        [HttpPost]
        public async Task<ActionResult<ResponseDto>> Post([FromBody] SupplierCreationDto supplierCreationDto)
        {
            if (supplierCreationDto == null)
            {
                return BadRequest("Supplier data is required.");
            }
            if (string.IsNullOrEmpty(supplierCreationDto.Name))
            {
                return BadRequest("Supplier name is required.");
            }
            if (string.IsNullOrEmpty(supplierCreationDto.NIT))
            {
                return BadRequest("Supplier NIT is required.");
            }
            var supplierCreated = await _supplierService.CreateSupplier(supplierCreationDto);

            if (!supplierCreated.IsSuccess || supplierCreated.Result == null)
            {
                return BadRequest(supplierCreated);
            }

            var supplierEntity = (Supplier)supplierCreated.Result;

            if (supplierEntity == null)
            {
                return BadRequest("Supplier creation failed or returned an unexpected result.");
            }

            var supplier = new SupplierResponseDto
            {
                Id = supplierEntity.Id,
                Name = supplierEntity.Name,
                NIT = supplierEntity.NIT,
                CustomFields = supplierEntity.CustomFields.Select(cf => new CustomFieldResponseDto
                {
                    Id = cf.Id,
                    FieldName = cf.FieldName,
                    FieldValue = cf.FieldValue
                }).ToList()
            };
            await _outputCacheStore.EvictByTagAsync(cacheTag, default);
            return CreatedAtRoute("GetSupplierById", new { id = supplier.Id }, supplier);
        }
        /// <summary>
        /// Updates an existing supplier.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="supplierCreationDto"></param>
        /// <returns></returns>
        /// <response code="204">Returns no content</response>
        /// <response code="400">If the supplier ID is invalid</response>
        [HttpPut("{id}", Name ="Edit")]
        public async Task<ActionResult>Put(Guid id, [FromBody] SupplierCreationDto supplierCreationDto)
        {
            if(id == Guid.Empty)
            {
                return BadRequest("Invalid supplier ID.");
            }
            if (supplierCreationDto == null)
            {
                return BadRequest("Supplier data is required.");
            }
            await _supplierService.UpdateSupplier(id, supplierCreationDto);
            await _outputCacheStore.EvictByTagAsync(cacheTag, default);

            return NoContent();
        }
    }
}
