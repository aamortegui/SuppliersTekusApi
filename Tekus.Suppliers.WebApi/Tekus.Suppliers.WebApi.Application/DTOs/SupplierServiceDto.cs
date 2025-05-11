using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Application.DTOs
{
    public class SupplierServiceDto
    {
        public Guid SupplierId { get; set; }        

        public string SupplierName { get; set; }
        
    }
}
