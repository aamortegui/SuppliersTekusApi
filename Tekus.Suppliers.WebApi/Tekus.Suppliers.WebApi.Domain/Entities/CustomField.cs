using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Domain.Entities
{
    public class CustomField
    {
        public Guid Id { get; set; }
        
        public Guid SupplierId { get; set; }
        

        public string FieldName { get; set; } = string.Empty;
        public string FieldValue { get; set; } = string.Empty;
    }
}
