using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities
{
    public class Service
    {
        [Key]
        public Guid Id { get; set; }        
        [Required]
        public string Name { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PriceHour { get; set; }        

        public ICollection<SupplierService> SupplierServices { get; set; } = new List<SupplierService>();
        public ICollection<ServiceCountry> ServiceCountries { get; set; } = new List<ServiceCountry>();
    }
}
