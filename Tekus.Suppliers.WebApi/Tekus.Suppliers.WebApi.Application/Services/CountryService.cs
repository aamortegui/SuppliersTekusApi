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
        public async Task<ResponseDto?> GetAllCountriesAsync()
        {
            var response = await _baseService.GetAllCountriesAsync(new Request()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CountryAPIBase + "/all",
            });

            return new ResponseDto()
            {
                IsSuccess = response.IsSuccess,                
                Countries = response?.Countries?.Select(dto => new CountryDto 
                {
                    Name = new PropertyNameDto 
                    { 
                        Common = dto.Name.Common, 
                        Official= dto.Name.Official
                    },
                }).OrderBy(x => x.Name.Common).ToList(),
                Message = response.Message
            };
        }
    }
}
