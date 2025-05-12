using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Application.DTOs
{
    public class CountrySummaryDto
    {
        public string CountryName { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalServices { get; set; }
    }
}
