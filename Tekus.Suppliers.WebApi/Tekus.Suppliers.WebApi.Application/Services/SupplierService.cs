using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Application.DTOs;
using Tekus.Suppliers.WebApi.Application.Services.Interfaces;
using Tekus.Suppliers.WebApi.Domain.Entities;
using Tekus.Suppliers.WebApi.Domain.Interfaces;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;
using CustomField = Tekus.Suppliers.WebApi.Domain.Entities.CustomField;
using Supplier = Tekus.Suppliers.WebApi.Domain.Entities.Supplier;

namespace Tekus.Suppliers.WebApi.Application.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;

        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<ResponseDto> GetAllSuppliersAsyc(SupplierFilterDto supplierFilterDto)
        {
            var supplierFilter = new SupplierFilter()
            {
                Name = supplierFilterDto.Name,
                NIT = supplierFilterDto.NIT,
                OrderBy = supplierFilterDto.OrderBy,
                IsDescending = supplierFilterDto.IsDescending,
                SupplierId = supplierFilterDto.SupplierId,
                Page = supplierFilterDto.Page,
                RecordsPerPage = supplierFilterDto.RecordsPerPage
            };
            var suppliers = await _supplierRepository.GetAllSuppliersAsync(supplierFilter);

            return new ResponseDto()
            {
                IsSuccess = suppliers.IsSuccess,
                Message = suppliers.Message,
                Result = suppliers.Result
            };
        }

        public async Task<ResponseDto> GetSupplierByIdAsync(Guid supplierId)
        {
            var supplier = await _supplierRepository.GetSupplierByIdAsync(supplierId);
            return new ResponseDto()
            {
                IsSuccess = supplier.IsSuccess,
                Message = supplier.Message,
                Result = supplier.Result
            };
        }

        public async Task<ResponseDto> CreateSupplier(SupplierCreationDto supplierCreationDto)
        {
            var supplier = new Supplier()
            {
                Name = supplierCreationDto.Name,
                NIT = supplierCreationDto.NIT,
                CustomFields = supplierCreationDto.CustomFields.Select(x => new CustomField()
                {
                    FieldName = x.FieldName,
                    FieldValue = x.FieldValue
                }).ToList()
            };
            var response = await _supplierRepository.CreateSupplierAsync(supplier);
            return new ResponseDto()
            {
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Result = response.Result
            };
        }
    }
}
