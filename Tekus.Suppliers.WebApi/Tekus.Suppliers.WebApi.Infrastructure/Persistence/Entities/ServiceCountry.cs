using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities
{
    public class ServiceCountry
    {
        public Guid ServiceId { get; set; }
        [JsonIgnore]
        public Service Service { get; set; }

        public Guid CountryId { get; set; }
        [JsonIgnore]
        public Country Country { get; set; }
    }
}
