using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Domain.Entities
{
    public class ServiceCreation
    {
        public string Name { get; set; }
        public decimal PriceHour { get; set; }
        public List<ServiceCreationCountry> ServiceCountries { get; set; }
        public List<AssociateSupplierService> SupplierServices { get; set; }
    }
}
