using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Models
{
    public class KPIRequestDataWithoutPart
    {

        #region DataMembers

        private string workOrder;
        private string kpiType;

        #endregion

        #region Constructors

        KPIRequestDataWithoutPart(string _workOrder, string _kpiType)
        {
            WorkOrder = _workOrder;
            KpiType = _kpiType;
        }

        KPIRequestDataWithoutPart() : this("", "") { }

        KPIRequestDataWithoutPart(KPIRequestDataWithoutPart data) : this(data.workOrder, data.KpiType) { } 

        #endregion

        #region Properties

        public string WorkOrder
        {
            get => workOrder;
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Couldn't initialize workOrderID!");
                else
                {
                    foreach (char symbol in value)
                    {
                        if(symbol < '0' || symbol > '9')
                        {
                            throw new ArgumentException("WorkOrderID isn't a number!");
                        }
                    }

                    workOrder = value;
                }
            }
        }

        public string KpiType
        {
            get
            {
                return kpiType;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Couldn't initialize kpiType!");
                }
                else
                {
                    bool found = false;

                    foreach (string type in KpiTypes.types)
                    {
                        if (type == value)
                        {
                            found = true;
                        }
                    }

                    if (found == false)
                    {
                        throw new ArgumentException("Couldn't initialize kpiType(type is not existing)");
                    }

                    kpiType = value;
                }
            }
        }

        #endregion
    }
}
