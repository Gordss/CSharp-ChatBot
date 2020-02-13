using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Models
{
    public class KPIApiResponse
    {
        public string Value { get; set; }
        public string Details { get; set; }
        public string UtcTimestamp { get; set; }
    }
}
