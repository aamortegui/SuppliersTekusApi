using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Application.DTOs
{
    public class SupplierCreationDto
    {   
        public string NIT { get; set; } = string.Empty;
        
        public string Name { get; set; } = string.Empty;

        public ICollection<CustomFieldDto> CustomFields { get; set; } = new List<CustomFieldDto>();
    }
}
