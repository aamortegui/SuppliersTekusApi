using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities
{
    public class ServiceSupplier
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public string Name { get; set; }
        public decimal PriceHour { get; set; }
        public string Country { get; set; }

        public Supplier Supplier { get; set; } = null;
    }
}
