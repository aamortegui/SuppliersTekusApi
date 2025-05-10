using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Domain.Entities
{
    public class Country
    {        
        public PropertyName Name { get; set; }
    }

    public class PropertyName
    {
        
        public string Common { get; set; }
        
        public string Official { get; set; }
    }
}
