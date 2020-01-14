using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Controllers
{
    public class KPIRequestData
    {
        //public KPIRequestData(string _workOrder,string _kpiType,string _part = "1"):WorkOrder(_workOrder),
        public string workOrder;
        public string kpiType;
        public string part ;
    }
}
