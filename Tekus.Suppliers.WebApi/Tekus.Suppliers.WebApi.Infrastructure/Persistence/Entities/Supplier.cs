using System.ComponentModel.DataAnnotations;

namespace Tekus.Suppliers.WebApi.Infrastructure.Persistence.Entities
{
    public class Supplier
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string NIT { get; set; } = string.Empty;
        [Required]
        public string Name { get; set; } = string.Empty;
        public string CustomField { get; set; } = string.Empty;
    }
}
