using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Moq;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Controllers;
using Tekus.Suppliers.WebApi.Domain.Entities;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;
using Country = Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities.Country;

namespace Tekus.Suppliers.WebApi.Tests.ControllerTests
{
    public class CountryControllerTests
    {

        [Fact]
        public async Task GetCountries_ReturnsOk_WhenServiceReturnsSuccess()
        {
            var pagination = new PaginationDTO { Page = 1 };
            var expectedResponse = new ResponseCountryDto
            {
                IsSuccess = true,
                Message = "Success",
                Countries = new List<CountryDto> { new CountryDto { Name = new PropertyNameDto { Common = "Colombia", Official = "Republic of Colombia" } } }
            };

            var countryServiceMock = new Mock<ICountryService>();
            countryServiceMock.Setup(x => x.GetAllCountriesAsync(pagination)).ReturnsAsync(expectedResponse);

            var cacheMock = new Mock<IOutputCacheStore>();
            var localServiceMock = new Mock<ICountryLocalService>();

            var controller = new CountryController(countryServiceMock.Object, cacheMock.Object, localServiceMock.Object);

            var result = await controller.GetCountries(pagination);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedValue = Assert.IsType<ResponseCountryDto>(okResult.Value);
            Assert.True(returnedValue.IsSuccess);
            Assert.NotNull(returnedValue.Countries);
        }

        [Fact]
        public async Task GetCountries_ReturnsBadRequest_WhenServiceFails()
        {
            var pagination = new PaginationDTO { Page = 1 };
            var expectedResponse = new ResponseCountryDto
            {
                IsSuccess = false,
                Message = "Error retrieving countries"
            };

            var countryServiceMock = new Mock<ICountryService>();
            countryServiceMock.Setup(x => x.GetAllCountriesAsync(pagination)).ReturnsAsync(expectedResponse);

            var cacheMock = new Mock<IOutputCacheStore>();
            var localServiceMock = new Mock<ICountryLocalService>();

            var controller = new CountryController(countryServiceMock.Object, cacheMock.Object, localServiceMock.Object);


            var result = await controller.GetCountries(pagination);


            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var json = System.Text.Json.JsonSerializer.Serialize(badRequestResult.Value);
            var jsonDoc = System.Text.Json.JsonDocument.Parse(json);
            var message = jsonDoc.RootElement.GetProperty("message").GetString();

            Assert.Equal("Error retrieving countries", message);
        }

        [Fact]
        public async Task GetLocalCountries_ReturnsOk_WhenDataExists()
        {
            var countryList = new List<Country>
            {
                 new Country { CountryId = Guid.NewGuid(), CommonName = "Colombia", OfficialName = "Republic of Colombia" },
                 new Country { CountryId = Guid.NewGuid(), CommonName = "Argentina", OfficialName = "Argentine Republic" }
            };

            var expectedResponse = new ResponseDto
            {
                IsSuccess = true,
                Result = countryList
            };

            var countryServiceMock = new Mock<ICountryService>();
            var cacheMock = new Mock<IOutputCacheStore>();
            var countryLocalServiceMock = new Mock<ICountryLocalService>();

            countryLocalServiceMock
            .Setup(s => s.GetAllCountriesLocalAsync(It.IsAny<CountryFilterDto>()))
            .ReturnsAsync(expectedResponse);

            var controller = new CountryController(countryServiceMock.Object, cacheMock.Object, countryLocalServiceMock.Object);
            var filterDto = new CountryFilterDto();

            var result = await controller.GetLocalCountries(filterDto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var countries = Assert.IsAssignableFrom<List<CountryLocalDto>>(okResult.Value);
            Assert.Equal(2, countries.Count);
            Assert.Contains(countries, c => c.CommonName == "Colombia");
        }
        [Fact]
        public async Task GetLocalCountries_ReturnsBadRequest_WhenServiceFails()
        {
            var expectedResponse = new ResponseDto
            {
                IsSuccess = false,
                Message = "Error fetching countries",
                Result = null
            };

            var countryServiceMock = new Mock<ICountryService>();
            var cacheMock = new Mock<IOutputCacheStore>();
            var countryLocalServiceMock = new Mock<ICountryLocalService>();

            countryLocalServiceMock
            .Setup(s => s.GetAllCountriesLocalAsync(It.IsAny<CountryFilterDto>()))
            .ReturnsAsync(expectedResponse);

            var controller = new CountryController(countryServiceMock.Object, cacheMock.Object, countryLocalServiceMock.Object);
            var filterDto = new CountryFilterDto();

            var result = await controller.GetLocalCountries(filterDto);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResponseDto>(badRequest.Value);
            Assert.False(response.IsSuccess);
            Assert.Equal("Error fetching countries", response.Message);
        }
    }
}
