using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities
{
    public class CustomField
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public Guid SupplierId { get; set; }
        public Supplier Supplier { get; set; }

        public string FieldName { get; set; } = string.Empty;
        public string FieldValue { get; set; } = string.Empty;
    }
}
