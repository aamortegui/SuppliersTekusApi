using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Application.DTOs
{
    public class ServiceCreationDto
    {
        public string Name { get; set; }
        public decimal PriceHour { get; set; }
        public List<ServiceCreationCountryDto> ServiceCountries { get; set; }
        public List<AssociateSupplierServiceDto> SupplierServices { get; set; }
    }
}
