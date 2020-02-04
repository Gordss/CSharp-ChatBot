using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Controllers
{
    public static class KpiTypes
    {
        #region DataMembers
        public static List<string> types = new List<string>
        {
            "OrderActualRemainigWork",
            "OrderEstimatedRemainigWork",
            "OrderRemainingWorkDeviation",
            "OrderPartTimePerItem",
            "PartActualRemainigWork",
            "PartCostDeviation",
            "PartEstimatedRemainigWork",
            "PercentScrapPartsPerOrder",
            "OTD"
        };
        #endregion
    }
}
