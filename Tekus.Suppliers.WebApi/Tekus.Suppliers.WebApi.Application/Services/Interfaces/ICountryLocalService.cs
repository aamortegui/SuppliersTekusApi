using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Application.DTOs;

namespace Tekus.Suppliers.WebApi.Application.Services.Interfaces
{
    public interface ICountryLocalService
    {
        Task<ResponseDto> GetAllCountriesLocalAsync(CountryFilterDto countryFilterDto);
    }
}
