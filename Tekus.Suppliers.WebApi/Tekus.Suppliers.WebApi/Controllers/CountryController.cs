using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Newtonsoft.Json;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;

namespace Tekus.Suppliers.WebApi.Controllers
{
    [Route("api/country")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;
        private readonly IOutputCacheStore _outputCacheStore;
        private const string cacheTag = "countries";
        public CountryController(ICountryService countryService, IOutputCacheStore outputCacheStore)
        {
            _countryService = countryService;
            _outputCacheStore = outputCacheStore;
        }

        [HttpGet("all-countries")]
        [OutputCache(Tags = [cacheTag])]
        public async Task<IActionResult> GetCountries()
        {
            
            ResponseDto? response = await _countryService.GetAllCountriesAsync();

            if(response is not null && response.IsSuccess)
            {
                return Ok( response.Countries);               
            }
            else
            {
                return BadRequest(new { message = response?.Message });
            }            
        }
    }
}
