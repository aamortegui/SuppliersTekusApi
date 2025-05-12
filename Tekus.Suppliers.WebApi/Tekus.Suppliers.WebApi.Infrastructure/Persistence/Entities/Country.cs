using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities
{
    public class Country
    {
        [Key]
        public Guid CountryId { get; set; }
        [Required]
        public string CommonName { get; set; }
        [Required]
        public string OfficialName { get; set; }

        public ICollection<ServiceCountry> ServiceCountries { get; set; } = new List<ServiceCountry>();

    }
}
