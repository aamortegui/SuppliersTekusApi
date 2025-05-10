using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Domain.Entities
{
    public class ResponseCountry
    {        
        public List<Country>? Countries { get; set; }
        
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
    }
}
