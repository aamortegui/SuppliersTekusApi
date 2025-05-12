using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Newtonsoft.Json;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Controllers
{
    [Route("api/country")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly ICountryLocalService _countryLocalService;
        private const string cacheTag = "countries";
        public CountryController(ICountryService countryService, IOutputCacheStore outputCacheStore, 
            ICountryLocalService countryLocalService)
        {
            _countryService = countryService;
            _outputCacheStore = outputCacheStore;
            _countryLocalService = countryLocalService;
        }

        [HttpGet("all-countries")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<IActionResult> GetCountries([FromQuery] PaginationDTO pagination)
        {
            
            ResponseCountryDto? response = await _countryService.GetAllCountriesAsync(pagination);

            if(response is not null && response.IsSuccess)
            {
                return Ok( response);               
            }
            else
            {
                return BadRequest(new { message = response?.Message });
            }            
        }
        [HttpGet("get-country/{name}")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<IActionResult> GetCountry(string name)
        {
            ResponseCountryDto? response = await _countryService.GetCountryAsync(name);

            if (response is not null && response.IsSuccess)
            {
                return Ok(response);
            }
            else
            {
                return BadRequest(new { message = response?.Message });
            }
        }

        [HttpGet("get-local-country")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<IActionResult> GetLocalCountries([FromQuery] CountryFilterDto countryFilterDto)
        {
            var response = await _countryLocalService.GetAllCountriesLocalAsync(countryFilterDto);

            if (!response.IsSuccess || response.Result == null)
            {
                return BadRequest(response);
            }

            var countryEntities = response.Result as List<Country>;

            var countries = countryEntities?.Select(c => new CountryLocalDto
            {
                CountryId = c.CountryId,
                CommonName = c.CommonName,
                OfficialName = c.OfficialName
            }).ToList();

            return Ok(countries);
        }
    }
}
