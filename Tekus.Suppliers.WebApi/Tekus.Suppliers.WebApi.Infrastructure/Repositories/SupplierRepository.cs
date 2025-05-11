using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Domain.Common;
using Tekus.Suppliers.WebApi.Domain.Entities;
using Tekus.Suppliers.WebApi.Domain.Interfaces;
using System.Linq.Dynamic.Core;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;
using Supplier = Tekus.Suppliers.WebApi.Domain.Entities.Supplier;
using Microsoft.Extensions.Configuration;

namespace Tekus.Suppliers.WebApi.Infrastructure.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly ServiceSuppliersDBContext _context;
        private readonly string[] _allowedOrderFields;

        public SupplierRepository(ServiceSuppliersDBContext context, IConfiguration config)
        {
            _context = context;
            _allowedOrderFields = config
            .GetSection("OrderSettings:AllowedOrderFields")
            .Get<string[]>()!;
        }

        public async Task<Response> GetAllSuppliersAsync(SupplierFilter supplierFilter)
        {
            Response response = new Response();
            try
            {
                var suppliersQueryabale = _context.Suppliers
                                            .Include(x => x.CustomFields)
                                            .AsQueryable();
                if (!string.IsNullOrEmpty(supplierFilter.Name))
                {
                    suppliersQueryabale = suppliersQueryabale.Where(x => x.Name.Contains(supplierFilter.Name));
                }
                if (supplierFilter.SupplierId != Guid.Empty)
                {
                    suppliersQueryabale = suppliersQueryabale.Where(x => x.Id == supplierFilter.SupplierId);
                }
                if (!string.IsNullOrEmpty(supplierFilter.NIT))
                {
                    suppliersQueryabale = suppliersQueryabale.Where(x => x.NIT.Contains(supplierFilter.NIT));                }

                
                string? upperOrderBy = supplierFilter.OrderBy?.ToUpper();
                if (!string.IsNullOrEmpty(supplierFilter.OrderBy) && _allowedOrderFields.Contains(upperOrderBy))
                {
                    var direction = supplierFilter.IsDescending ? "descending" : "ascending";
                    suppliersQueryabale = suppliersQueryabale.OrderBy($"{supplierFilter.OrderBy} {direction}");
                }

                var suppliers = await suppliersQueryabale
                    .Paginate(new Pagination { Page = supplierFilter.Page, RecordsPerPage = supplierFilter.RecordsPerPage })
                    .ToListAsync();

                response.Result = suppliers;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> GetSupplierByIdAsync(Guid supplierId)
        {
            Response response = new Response();
            try
            {
                var supplier = await _context.Suppliers
                    .Include(x => x.CustomFields)
                    .FirstOrDefaultAsync(x => x.Id == supplierId);
                if (supplier is null)
                {
                    response.IsSuccess = false;
                    response.Message = "Supplier not found";
                    return response;
                }
                response.Result = supplier;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }

        public async Task<Response> CreateSupplierAsync(Supplier supplier)
        {
            Response response = new Response();
            var supplierId = Guid.NewGuid();

            var existingSupplier = await _context.Suppliers
                .FirstOrDefaultAsync(x => x.Name == supplier.Name && x.NIT == supplier.NIT);

            if (existingSupplier is not null)
            {
                response.IsSuccess = false;
                response.Message = "Supplier already exists";
                return response;
            }

                using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var supplierEntity = new Persistence.Entities.Supplier()
                {
                    Id = supplierId,
                    Name = supplier.Name,
                    NIT = supplier.NIT,
                    CustomFields = supplier.CustomFields.Select(cf => new Persistence.Entities.CustomField
                    {
                        Id = Guid.NewGuid(),
                        SupplierId = supplierId,
                        FieldName = cf.FieldName,
                        FieldValue = cf.FieldValue
                    }).ToList()
                };

                await _context.Suppliers.AddAsync(supplierEntity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                response.IsSuccess = true;
                response.Result = supplierEntity;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
