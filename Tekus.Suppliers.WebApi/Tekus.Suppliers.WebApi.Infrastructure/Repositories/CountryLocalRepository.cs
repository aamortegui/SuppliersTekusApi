using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Domain.Entities;
using Tekus.Suppliers.WebApi.Domain.Interfaces;
using System.Linq.Dynamic.Core;
using Tekus.Suppliers.WebApi.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Infrastructure.Repositories
{
    public class CountryLocalRepository : ICountryLocalRepository
    {
        private readonly ServiceSuppliersDBContext _context;
        private readonly string[] _allowedOrderServiceFields;
        public CountryLocalRepository(ServiceSuppliersDBContext context, IConfiguration config)
        {
            _context = context;
            _allowedOrderServiceFields = config
            .GetSection("OrderSettings:AllowedCountryFields")
            .Get<string[]>()!;
        }
        public async Task<Response> GetAllCountiesLocalAsync(CountryFilter countryFilter)
        {
            Response response = new Response();

            try
            {
                var countryQuery = _context.Countries.AsQueryable();

                if (!string.IsNullOrEmpty(countryFilter.CommonName))
                {
                    countryQuery = countryQuery.Where(c => c.CommonName.Contains(countryFilter.CommonName));
                }
                if (!string.IsNullOrEmpty(countryFilter.OfficialName))
                {
                    countryQuery = countryQuery.Where(c => c.OfficialName.Contains(countryFilter.OfficialName));
                }
                if (countryFilter.CountryId != Guid.Empty)
                {
                    countryQuery = countryQuery.Where(c => c.CountryId == countryFilter.CountryId);
                }

                string? upperOrderBy = countryFilter.OrderBy?.ToUpper();
                if (!string.IsNullOrEmpty(countryFilter.OrderBy) && _allowedOrderServiceFields.Contains(upperOrderBy))
                {
                    var direction = countryFilter.IsDescending ? "descending" : "ascending";
                    countryQuery = countryQuery.OrderBy($"{countryFilter.OrderBy} {direction}");
                }
                var countries = await countryQuery
                        .Paginate(new Pagination { Page = countryFilter.Page, RecordsPerPage = countryFilter.RecordsPerPage })
                        .ToListAsync();

                response.Result = countries;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
