using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Models
{
    public class KpiTypes
    {
        #region DataMembers

        public static readonly string[] types = {
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
