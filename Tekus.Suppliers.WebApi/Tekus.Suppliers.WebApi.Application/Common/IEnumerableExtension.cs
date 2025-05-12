using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Application.DTOs;

namespace Tekus.Suppliers.WebApi.Domain.Common
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<T>Paginate<T>(this IEnumerable<T>collectionable, 
            PaginationDTO pagination)
        {
            if (collectionable == null) return Enumerable.Empty<T>();

            return collectionable
                .Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                .Take(pagination.RecordsPerPage);
        }
    }
}
