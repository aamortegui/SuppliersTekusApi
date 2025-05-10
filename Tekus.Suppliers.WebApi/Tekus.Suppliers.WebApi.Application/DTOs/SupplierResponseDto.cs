using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Application.DTOs
{
    public class SupplierResponseDto
    {
        public Guid Id { get; set; }

        public string NIT { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public ICollection<CustomFieldResponseDto> CustomFields { get; set; } = new List<CustomFieldResponseDto>();
    }
}
