using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Domain.Entities;

namespace Tekus.Suppliers.WebApi.Domain.Common
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable,
            Pagination pagination)
        {
            var page = pagination.Page < 1 ? 1 : pagination.Page;
            var recordsPerPage = pagination.RecordsPerPage < 1 ? 10 : pagination.RecordsPerPage;

            return queryable
                .Skip((page - 1) * recordsPerPage)
                .Take(recordsPerPage);
        }
    }
}
