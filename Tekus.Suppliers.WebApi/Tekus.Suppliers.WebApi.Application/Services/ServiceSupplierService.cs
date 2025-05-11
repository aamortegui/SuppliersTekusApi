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
    public class ServiceSupplierService : IServiceSupplierService
    {
        private readonly IServiceRepository _serviceRepository;
        public ServiceSupplierService(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }
        public async Task<ResponseDto> GetAllServicesAsyc(ServiceFilterDto supplierServiceFilterDto)
        {
            var serviceFilter = new ServiceFilter()
            {
                Name = supplierServiceFilterDto.Name,
                Price = supplierServiceFilterDto.Price,
                OrderBy = supplierServiceFilterDto.OrderBy,
                IsDescending = supplierServiceFilterDto.IsDescending,
                ServiceId = supplierServiceFilterDto.ServiceId,
                Page = supplierServiceFilterDto.Page,
                RecordsPerPage = supplierServiceFilterDto.RecordsPerPage
            };

            var services = await _serviceRepository.GetAllServicesAsync(serviceFilter);

            return new ResponseDto()
            {
                IsSuccess = services.IsSuccess,
                Message = services.Message,
                Result = services.Result
            };
        }
        public async Task<ResponseDto> GetServiceByIdAsync(Guid id)
        {
            var service = await _serviceRepository.GetServiceByIdAsync(id);
            return new ResponseDto()
            {
                IsSuccess = service.IsSuccess,
                Message = service.Message,
                Result = service.Result
            };
        }
    }
}
