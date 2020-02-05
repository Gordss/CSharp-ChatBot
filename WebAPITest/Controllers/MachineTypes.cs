using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Controllers
{
    public static class MachineTypes
    {
        #region DataMembers

        public static List<string> types = new List<string>
        {
            "AlarmCount",
            "Downtime",
            "DowntimeSummary",
            "StatusSummary",
            "oee",
            "oee.availability",
            "oee.performance",
            "oee.quality"
        };

        #endregion
    }
}
