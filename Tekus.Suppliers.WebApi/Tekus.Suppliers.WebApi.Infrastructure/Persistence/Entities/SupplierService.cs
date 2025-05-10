using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities
{
    public class SupplierService
    {
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public Guid ServiceId { get; set; }
        public Service Service { get; set; }
    }
}
