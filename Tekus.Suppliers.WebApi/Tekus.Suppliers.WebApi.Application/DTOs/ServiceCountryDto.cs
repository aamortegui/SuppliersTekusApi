using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Application.DTOs
{
    public class ServiceCountryDto
    {
        public Guid CountryId { get; set; }
        public string CommonName { get; set; }
        public string OfficialName { get; set; }
    }
}
