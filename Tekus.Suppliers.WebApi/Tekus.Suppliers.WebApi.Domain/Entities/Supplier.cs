using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Domain.Entities
{
    public class Supplier
    {
        public Guid Id { get; set; }
        public string NIT { get; set; } = string.Empty;
       
        public string Name { get; set; } = string.Empty;
        public ICollection<CustomField> CustomFields { get; set; } = new List<CustomField>();
    }
}
