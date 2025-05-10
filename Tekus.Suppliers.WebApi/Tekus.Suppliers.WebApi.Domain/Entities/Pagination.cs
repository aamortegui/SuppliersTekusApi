using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tekus.Suppliers.WebApi.Domain.Entities
{
    public class Pagination
    {
        public int Page { get; set; } = 1;
        private int recordsPerPage = 10;
        private int maximumAmountOfRecordsPerPage = 50;

        public int RecordsPerPage
        {
            get { return recordsPerPage; }
            set
            {
                recordsPerPage = (value > maximumAmountOfRecordsPerPage) ? maximumAmountOfRecordsPerPage : value;
            }
        }
    }
}
