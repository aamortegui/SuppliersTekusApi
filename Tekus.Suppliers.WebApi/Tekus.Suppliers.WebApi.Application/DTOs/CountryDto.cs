using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Domain.Entities;

namespace Tekus.Suppliers.WebApi.Application.DTOs
{
    public class CountryDto
    {
        public PropertyNameDto Name { get; set; }
    }

    public class PropertyNameDto
    {

        public string Common { get; set; }

        public string Official { get; set; }
    }
}
