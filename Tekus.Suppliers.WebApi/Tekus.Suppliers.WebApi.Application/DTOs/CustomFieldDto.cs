using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities;

namespace Tekus.Suppliers.WebApi.Application.DTOs
{
    public class CustomFieldDto
    {
        public string FieldName { get; set; } = string.Empty;
        public string FieldValue { get; set; } = string.Empty;
    }
}
