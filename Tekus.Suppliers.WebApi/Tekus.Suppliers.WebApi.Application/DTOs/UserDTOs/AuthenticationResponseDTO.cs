using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Application.DTOs.UserDTOs
{
    public class AuthenticationResponseDTO
    {
        public required string Token { get; set; }
        public DateTime Expiration { get; set; }
    }
}
