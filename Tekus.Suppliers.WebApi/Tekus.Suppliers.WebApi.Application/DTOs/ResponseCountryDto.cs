using Tekus.Suppliers.WebApi.Domain.Entities;

namespace Tekus.Suppliers.WebApi.Application.DTOs
{
    public class ResponseCountryDto
    {
        public List<CountryDto>? Countries { get; set; }
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; } = "";
    }
}
