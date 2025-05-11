using Microsoft.EntityFrameworkCore;
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
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Infrastructure.Repositories
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ServiceSuppliersDBContext _context;
        private readonly string[] _allowedOrderServiceFields;


        public ServiceRepository(ServiceSuppliersDBContext context, IConfiguration config)
        {
            _context = context;
            _allowedOrderServiceFields = config
            .GetSection("OrderSettings:AllowedOrderServiceFields")
            .Get<string[]>()!;
        }

        public async Task<Response> GetAllServicesAsync(ServiceFilter serviceFilter)
        {
            Response response = new Response();

            try
            {
                var servicesQuery = _context.Services
                    .Include(x => x.ServiceCountries)
                    .Include(x => x.SupplierServices)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(serviceFilter.Name))
                {
                    servicesQuery = servicesQuery.Where(x => x.Name.Contains(serviceFilter.Name));
                }
                if (serviceFilter.ServiceId != Guid.Empty)
                {
                    servicesQuery = servicesQuery.Where(x => x.Id == serviceFilter.ServiceId);
                }
                if (serviceFilter.Price != null && serviceFilter.Price > 0)
                {
                    servicesQuery = servicesQuery.Where(x => x.PriceHour == serviceFilter.Price);
                }

                string? upperOrderBy = serviceFilter.OrderBy?.ToUpper();
                if (!string.IsNullOrEmpty(serviceFilter.OrderBy) && _allowedOrderServiceFields.Contains(upperOrderBy))
                {
                    var direction = serviceFilter.IsDescending ? "descending" : "ascending";
                    servicesQuery = servicesQuery.OrderBy($"{serviceFilter.OrderBy} {direction}");
                }
                var services = await servicesQuery
                        .Paginate(new Pagination { Page = serviceFilter.Page, RecordsPerPage = serviceFilter.RecordsPerPage })
                        .ToListAsync();

                response.Result = services;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> GetServiceByIdAsync(Guid id)
        {
            Response response = new Response();
            try
            {
                var service = await _context.Services
                    .Include(x => x.ServiceCountries)
                    .Include(x => x.SupplierServices)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (service == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Service not found";
                }
                response.Result = service;
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
