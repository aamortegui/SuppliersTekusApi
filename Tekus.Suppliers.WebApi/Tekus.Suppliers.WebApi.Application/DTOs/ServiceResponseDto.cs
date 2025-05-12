using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Application.DTOs
{
    public class ServiceResponseDto
    {        
        public Guid Id { get; set; }
       
        public string Name { get; set; }
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PriceHour { get; set; }

        public ICollection<SupplierServiceDto> SupplierServices { get; set; } = new List<SupplierServiceDto>();
        public ICollection<ServiceCountryDto> ServiceCountries { get; set; } = new List<ServiceCountryDto>();
    }
}
