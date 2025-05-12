using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Domain.Entities;
using Tekus.Suppliers.WebApi.Domain.Interfaces;

namespace Tekus.Suppliers.WebApi.Infrastructure.Services
{
    public class ReportService : IReportService
    {
        private readonly ServiceSuppliersDBContext _context;
        public ReportService(ServiceSuppliersDBContext context)
        {
            _context = context;
        }
        public async Task<List<CountrySummary>> GetCountrySummaryAsync()
        {
            var summaryCountry =  await (from c in _context.Countries
                                         select new CountrySummary
                                         {
                                             CountryName = c.CommonName,
                                             TotalSuppliers = c.ServiceCountries
                                                .SelectMany(sc => sc.Service.SupplierServices)
                                                .Select(ss => ss.SupplierId)
                                                .Distinct()
                                                .Count(),
                                             TotalServices = c.ServiceCountries
                                                .Select(sc => sc.ServiceId)
                                                .Distinct()
                                                .Count()
                                         }).OrderByDescending(x => x.TotalServices).ToListAsync();

            return summaryCountry;
        }
    }
}
