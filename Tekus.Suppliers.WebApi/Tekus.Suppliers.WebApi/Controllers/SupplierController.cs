using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Controllers
{
    [Route("api/supplier")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly ISupplierService _supplierService;
        public SupplierController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSuppliers([FromQuery] SupplierFilterDto supplierFilterDto)
        {
            var response = await _supplierService.GetAllSuppliersAsyc(supplierFilterDto);

            if (!response.IsSuccess || response.Result == null)
            {
                return BadRequest(response);
            }

            var supplierEntities = response.Result as List<Supplier>;
                        
            var suppliers = supplierEntities.Select(s => new SupplierResponseDto
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

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> Post([FromBody] SupplierCreationDto supplierCreationDto)
        {
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

            return CreatedAtRoute("GetSupplierById", new { id = supplier.Id }, supplier);
        }
    }
}
