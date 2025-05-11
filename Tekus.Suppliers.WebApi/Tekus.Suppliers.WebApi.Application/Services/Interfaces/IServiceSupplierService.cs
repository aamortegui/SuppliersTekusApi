using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Application.DTOs;

namespace Tekus.Suppliers.WebApi.Application.Services.Interfaces
{
    public interface IServiceSupplierService
    {
        Task<ResponseDto> GetAllServicesAsyc(ServiceFilterDto supplierServiceFilterDto);
        Task<ResponseDto> GetServiceByIdAsync(Guid id);
    }
}
