using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Domain.Entities;

namespace Tekus.Suppliers.WebApi.Domain.Interfaces
{
    public interface IBaseService
    {
        Task<ResponseCountry?> GetCountriesAsync(Request requestApi);
        Task<bool> SyncCountriesToDatabaseAsync();
    }
}
