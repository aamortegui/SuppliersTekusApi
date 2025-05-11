using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Domain.Entities;

namespace Tekus.Suppliers.WebApi.Domain.Interfaces
{
    public interface IServiceRepository
    {
        Task<Response> GetAllServicesAsync(ServiceFilter serviceFilter);
        Task<Response> GetServiceByIdAsync(Guid id);
    }
}
