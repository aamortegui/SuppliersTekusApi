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

        public ICollection<SupplierService> SupplierServices { get; set; }
        public ICollection<CustomField> CustomFields { get; set; } = new List<CustomField>();
    }
}
