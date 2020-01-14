using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Controllers
{
    public class MachineRequestDataInRange
    {
        public string sensorId;
        public string type;
        public DateTime before;
        public DateTime after;
        public int count;
    }
}
