using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Controllers
{
    public class KPIRequestDataInRange
    {
        public string workOrder;
        public string kpiType;
        public string part = "1";
        public DateTime before;
        public DateTime after;
        public int count;
    }
}
