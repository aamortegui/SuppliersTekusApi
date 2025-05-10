using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities
{
    public class ServiceCountry
    {
        public Guid ServiceId { get; set; }
        public Service Service { get; set; }

        public Guid CountryId { get; set; }
        public Country Country { get; set; }
    }
}
