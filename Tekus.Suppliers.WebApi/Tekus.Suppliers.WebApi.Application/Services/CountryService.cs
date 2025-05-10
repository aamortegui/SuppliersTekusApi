using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Domain.Common;
using Tekus.Suppliers.WebApi.Domain.Entities;
using Tekus.Suppliers.WebApi.Domain.Interfaces;

namespace Tekus.Suppliers.WebApi.Application.Services
{
    public class CountryService : ICountryService
    {
        private readonly IBaseService _baseService;
        public CountryService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseCountryDto?> GetAllCountriesAsync(PaginationDTO pagination)
        {
            var response = await _baseService.GetCountriesAsync(new Request()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CountryAPIBase + "/all",
            });

            var mappedCountries = response?.Countries?
                .Select(dto => new CountryDto
                {
                    Name = new PropertyNameDto
                    {
                        Common = dto.Name.Common,
                        Official = dto.Name.Official
                    }
                });
           
            var orderedCountries = mappedCountries?.OrderBy(x => x.Name.Common);
            var pagedCountries = orderedCountries?.Paginate(pagination);

            return new ResponseCountryDto()
            {
                IsSuccess = response.IsSuccess,
                Countries = pagedCountries?.ToList(),
                Message = response.Message
            };
        }
        public async Task<ResponseCountryDto?> GetCountryAsync(string name)
        {
            var response = await _baseService.GetCountriesAsync(new Request()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CountryAPIBase + "/name/"+ name
            });
            var mappedCountries = response?.Countries?
                .Select(dto => new CountryDto
                {
                    Name = new PropertyNameDto
                    {
                        Common = dto.Name.Common,
                        Official = dto.Name.Official
                    }
                });

            return new ResponseCountryDto()
            {
                IsSuccess = response.IsSuccess,
                Countries = mappedCountries?.ToList(),
                Message = response.Message
            };
        }
    }
}
