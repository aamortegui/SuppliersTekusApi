using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Domain.Interfaces;

namespace Tekus.Suppliers.WebApi.Application.Services
{
    public class IndicatorService: IIndicatorService
    {
        private readonly IReportService _indicatorRepository;

        public IndicatorService(IReportService indicatorRepository)
        {
            _indicatorRepository = indicatorRepository;
        }

        public async Task<List<CountrySummaryDto>> GetCountrySummaryAsync()
        {
            List<CountrySummaryDto> summaryCountryDto = new List<CountrySummaryDto>();

            var summaryCountry = await _indicatorRepository.GetCountrySummaryAsync();

            summaryCountryDto = summaryCountry.Select(s => new CountrySummaryDto
            {
                CountryName = s.CountryName,
                TotalServices = s.TotalServices,
                TotalSuppliers = s.TotalSuppliers
            }).ToList();

            return summaryCountryDto;
        }
    }
}
