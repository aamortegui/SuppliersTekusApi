using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Domain.Entities;
using Tekus.Suppliers.WebApi.Domain.Interfaces;

namespace Tekus.Suppliers.WebApi.Application.Services
{
    public class CountryLocalService : ICountryLocalService
    {
        private readonly ICountryLocalRepository _countryLocalRepository;
        public CountryLocalService(ICountryLocalRepository countryLocalRepository)
        {
            _countryLocalRepository = countryLocalRepository;
        }
        public async Task<ResponseDto> GetAllCountriesLocalAsync(CountryFilterDto countryFilterDto)
        {
            var countryFilter = new CountryFilter()
            {
                CommonName = countryFilterDto.CommonName,
                OfficialName = countryFilterDto.OfficialName,
                CountryId = countryFilterDto.CountryId,
                OrderBy = countryFilterDto.OrderBy,
                IsDescending = countryFilterDto.IsDescending,
                Page = countryFilterDto.Page,
                RecordsPerPage = countryFilterDto.RecordsPerPage
            };

            var services = await _countryLocalRepository.GetAllCountiesLocalAsync(countryFilter);

            return new ResponseDto()
            {
                IsSuccess = services.IsSuccess,
                Message = services.Message,
                Result = services.Result
            };
        }
    }
}
