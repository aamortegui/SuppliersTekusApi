using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Application.DTOs.UserDTOs
{
    public class EditClaimDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
    }
}
