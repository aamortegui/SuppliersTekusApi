using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Application.DTOs
{
    public class CountryFilterDto
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        internal PaginationDTO PaginationDTO
        {
            get
            {
                return new PaginationDTO() { Page = Page, RecordsPerPage = RecordsPerPage };
            }
        }        
        public string? CommonName { get; set; }
        public string? OrderBy { get; set; }
        public bool IsDescending { get; set; }
        public Guid CountryId { get; set; }
        public string? OfficialName { get; set; }
    }
}
