using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Controllers;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Tests.ControllerTests
{
    public class SupplierControllerTests
    {
        private readonly Mock<ISupplierService> _mockSupplierService;
        private readonly Mock<IOutputCacheStore> _mockCacheStore;
        private readonly SupplierController _controller;

        public SupplierControllerTests()
        {
            _mockSupplierService = new Mock<ISupplierService>();
            _mockCacheStore = new Mock<IOutputCacheStore>();
            _controller = new SupplierController(_mockSupplierService.Object, _mockCacheStore.Object);
        }

        [Fact]
        public async Task GetSuppliers_ReturnsOkResult_WhenSuccessful()
        {
            var suppliers = new List<Supplier> {
            new Supplier {
                Id = Guid.NewGuid(), Name = "Proveedor", NIT = "123", CustomFields = new List<CustomField>()
            }
        };
            var response = new ResponseDto
            {
                IsSuccess = true,
                Result = suppliers
            };

            _mockSupplierService.Setup(s => s.GetAllSuppliersAsyc(It.IsAny<SupplierFilterDto>()))
            .ReturnsAsync(response);

            var result = await _controller.GetSuppliers(new SupplierFilterDto());

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSuppliers = Assert.IsType<List<SupplierResponseDto>>(okResult.Value);
            Assert.Single(returnedSuppliers);
        }

        [Fact]
        public async Task GetSupplierById_ReturnsBadRequest_WhenNotSuccess()
        {
            var response = new ResponseDto { IsSuccess = false, Message = "Not found" };
            _mockSupplierService.Setup(s => s.GetSupplierByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(response);

            var result = await _controller.GetSupplierById(Guid.NewGuid());

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("message", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Post_ReturnsCreatedAtRoute_WhenSuccessful()
        {
            var supplierDto = new SupplierCreationDto 
            { 
                Name = "Proveedor", 
                NIT = "123", 
                CustomFields = new List<CustomFieldDto>() 
            };

            var createdSupplier = new Supplier
            {
                Id = Guid.NewGuid(),
                Name = supplierDto.Name,
                NIT = supplierDto.NIT,
                CustomFields = new List<CustomField>()
            };

            var response = new ResponseDto
            {
                IsSuccess = true,
                Result = createdSupplier
            };

            _mockSupplierService.Setup(s => s.CreateSupplier(It.IsAny<SupplierCreationDto>()))
            .ReturnsAsync(response);

            _mockCacheStore.Setup(c => c.EvictByTagAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask());

            var result = await _controller.Post(supplierDto);

            var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
            var supplierResponse = Assert.IsType<SupplierResponseDto>(createdResult.Value);
            Assert.Equal("Proveedor", supplierResponse.Name);
        }

        [Fact]
        public async Task Put_ReturnsNoContent_WhenValid()
        {
            var id = Guid.NewGuid();
            var dto = new SupplierCreationDto 
            { 
                Name = "Coordinadora", 
                NIT = "890904713-2", 
                CustomFields = new List<CustomFieldDto>() 
            };

            _mockSupplierService.Setup(s => s.UpdateSupplier(id, dto)).Returns(Task.CompletedTask);
            _mockCacheStore.Setup(c => c.EvictByTagAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask());

            var result = await _controller.Put(id, dto);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenDtoIsNull()
        {            
            var result = await _controller.Post(null);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Supplier data is required.", badRequest.Value);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenNameIsEmpty()
        {
            
            var dto = new SupplierCreationDto { Name = "", NIT = "123" };

            
            var result = await _controller.Post(dto);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Supplier name is required.", badRequest.Value);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenNITIsEmpty()
        {
            
            var dto = new SupplierCreationDto { Name = "Proveedor", NIT = "" };

            
            var result = await _controller.Post(dto);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Supplier NIT is required.", badRequest.Value);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenCreationFails()
        {
            
            var dto = new SupplierCreationDto { Name = "Proveedor", NIT = "123" };

            _mockSupplierService.Setup(s => s.CreateSupplier(dto)).ReturnsAsync(new ResponseDto
            {
                IsSuccess = false,
                Message = "Error al crear"
            });

            
            var result = await _controller.Post(dto);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<ResponseDto>(badRequest.Value);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task Put_ReturnsBadRequest_WhenIdIsEmpty()
        {
            
            var dto = new SupplierCreationDto { Name = "Proveedor", NIT = "123" };

            
            var result = await _controller.Put(Guid.Empty, dto);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid supplier ID.", badRequest.Value);
        }
        [Fact]
        public async Task Put_ReturnsBadRequest_WhenDtoIsNull()
        {
            
            var result = await _controller.Put(Guid.NewGuid(), null);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Supplier data is required.", badRequest.Value);
        }
    }
}
