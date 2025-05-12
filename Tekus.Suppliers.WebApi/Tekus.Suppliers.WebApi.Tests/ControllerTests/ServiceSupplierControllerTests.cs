using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Moq;
using Newtonsoft.Json;
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
    public class ServiceSupplierControllerTests
    {
        private readonly Mock<IServiceSupplierService> _mockServiceSupplier;
        private readonly Mock<IOutputCacheStore> _mockCacheStore;
        private readonly ServiceSupplierController _controller;

        public ServiceSupplierControllerTests()
        {
            _mockServiceSupplier = new Mock<IServiceSupplierService>();
            _mockCacheStore = new Mock<IOutputCacheStore>();
            _controller = new ServiceSupplierController(_mockServiceSupplier.Object, _mockCacheStore.Object);
        }

        [Fact]
        public async Task GetSuppliers_ReturnsOkResult_WithListOfSuppliers()
        {
            var filter = new ServiceFilterDto(); // usar datos si es necesario
            var services = new List<Service>
            {
                new Service
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Service",
                    PriceHour = 100,
                    ServiceCountries = new List<ServiceCountry> { new ServiceCountry { CountryId = Guid.NewGuid() } },
                    SupplierServices = new List<SupplierService> { new SupplierService { SupplierId = Guid.NewGuid() } }
                }
            };
            var response = new ResponseDto
            {
                IsSuccess = true,
                Result = services
            };

            _mockServiceSupplier
            .Setup(s => s.GetAllServicesAsyc(filter))
            .ReturnsAsync(response);

            var result = await _controller.GetSuppliers(filter);

           
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedSuppliers = Assert.IsType<List<ServiceResponseDto>>(okResult.Value);
            Assert.Single(returnedSuppliers);
            Assert.Equal("Test Service", returnedSuppliers[0].Name);
        }

        [Fact]
        public async Task GetSuppliers_ReturnsBadRequest_WhenServiceFails()
        {
            var filter = new ServiceFilterDto();
            var response = new ResponseDto
            {
                IsSuccess = false,
                Result = null,
                Message = "Error"
            };

            _mockServiceSupplier
            .Setup(s => s.GetAllServicesAsyc(filter))
            .ReturnsAsync(response);

            var result = await _controller.GetSuppliers(filter);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(response, badRequest.Value);
        }

        [Fact]
        public async Task GetServiceById_ReturnsOk_WhenServiceExists()
        {
            var serviceId = Guid.NewGuid();
            var service = new Service
            {
                Id = serviceId,
                Name = "Internet",
                PriceHour = 150,
                ServiceCountries = new List<ServiceCountry>(),
                SupplierServices = new List<SupplierService>()
            };

            var response = new ResponseDto
            {
                IsSuccess = true,
                Result = service
            };

            _mockServiceSupplier
            .Setup(s => s.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(response);

            var result = await _controller.GetServiceById(serviceId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(response, okResult.Value);
        }

        [Fact]
        public async Task GetServiceById_ReturnsBadRequest_WhenServiceFails()
        {
            var serviceId = Guid.NewGuid();
            var response = new ResponseDto
            {
                IsSuccess = false,
                Message = "Service not found"
            };

            _mockServiceSupplier
            .Setup(s => s.GetServiceByIdAsync(serviceId))
            .ReturnsAsync(response);

            var result = await _controller.GetServiceById(serviceId);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var expectedJson = JsonConvert.SerializeObject(new { message = "Service not found" });
            var actualJson = JsonConvert.SerializeObject(badRequestResult.Value);
            Assert.Equal(expectedJson, actualJson);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenDtoIsNull()
        {
            
            var result = await _controller.Post(null);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Service data is required.", badRequest.Value);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenNameIsMissing()
        {
            
            var dto = new ServiceCreationDto
            {
                Name = null,
                PriceHour = 100
            };

            
            var result = await _controller.Post(dto);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Service name is required.", badRequest.Value);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenPriceIsInvalid()
        {
            
            var dto = new ServiceCreationDto
            {
                Name = "Test Service",
                PriceHour = 0 
            };

            
            var result = await _controller.Post(dto);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Service price must be greater than zero.", badRequest.Value);
        }

        [Fact]
        public async Task Post_ReturnsBadRequest_WhenServiceCreationFails()
        {
            
            var dto = new ServiceCreationDto
            {
                Name = "Test Service",
                PriceHour = 100
            };

            var failedResponse = new ResponseDto
            {
                IsSuccess = false,
                Message = "Creation failed",
                Result = null
            };

            _mockServiceSupplier
                .Setup(s => s.CreateServiceAsync(dto))
                .ReturnsAsync(failedResponse);

            
            var result = await _controller.Post(dto);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(failedResponse, badRequest.Value);
        }

        [Fact]
        public async Task Post_ReturnsCreatedAtRoute_WhenSuccess()
        {
            // Arrange
            var dto = new ServiceCreationDto
            {
                Name = "Test Service",
                PriceHour = 100,
                ServiceCountries = new List<ServiceCreationCountryDto>
                    {
                        new() { CountryId = Guid.NewGuid() }
                    },
                            SupplierServices = new List<AssociateSupplierServiceDto>
                    {
                        new() { SupplierId = Guid.NewGuid() }
                    }
            };

            var service = new Service
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                PriceHour = dto.PriceHour,
                ServiceCountries = dto.ServiceCountries
                    .Select(c => new ServiceCountry { CountryId = c.CountryId })
                    .ToList(),
                SupplierServices = dto.SupplierServices
                    .Select(s => new SupplierService { SupplierId = s.SupplierId })
                    .ToList()
            };

            var successResponse = new ResponseDto
            {
                IsSuccess = true,
                Result = service
            };

            _mockServiceSupplier
                .Setup(s => s.CreateServiceAsync(dto))
                .ReturnsAsync(successResponse);

            
            var result = await _controller.Post(dto);

            
            var createdResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.Equal("GetServiceById", createdResult.RouteName);
            var returnedDto = Assert.IsType<ServiceResponseDto>(createdResult.Value);
            Assert.Equal(dto.Name, returnedDto.Name);
            Assert.Equal(dto.PriceHour, returnedDto.PriceHour);
        }

        [Fact]
        public async Task Put_ReturnsBadRequest_WhenIdIsEmpty()
        {
            
            var id = Guid.Empty;
            var serviceDto = new ServiceCreationDto();

            
            var result = await _controller.Put(id, serviceDto);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid service ID.", badRequest.Value);
        }

        [Fact]
        public async Task Put_ReturnsBadRequest_WhenDtoIsNull()
        {
            
            var id = Guid.NewGuid();

            
            var result = await _controller.Put(id, null);

            
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Service data is required.", badRequest.Value);
        }

        [Fact]
        public async Task Put_ReturnsNoContent_WhenUpdateSucceeds()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new ServiceCreationDto
            {
                Name = "Test Service",
                PriceHour = 100
            };

            _mockServiceSupplier.Setup(s => s.UpdateServiceAsync(id, dto)).Returns(Task.CompletedTask);
            _mockCacheStore.Setup(c => c.EvictByTagAsync(It.IsAny<string>(), default)).Returns(ValueTask.CompletedTask);

            // Act
            var result = await _controller.Put(id, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockServiceSupplier.Verify(s => s.UpdateServiceAsync(id, dto), Times.Once);
            _mockCacheStore.Verify(c => c.EvictByTagAsync("service", default), Times.Once);
        }
    }
}
