using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Controllers
{
    public class KPIRequestDataWithPart
    {
        #region DataMembers
        private string workOrder;
        private string kpiType;
        private string part;
        #endregion

        #region Properties
        public string WorkOrder
        {
            get => workOrder;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                else
                {
                    foreach (char symbol in value)
                    {
                        if (symbol < '0' || symbol > '9')
                        {
                            throw new ArgumentException();
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
                    throw new ArgumentNullException();
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
                        throw new ArgumentException();
                    }

                    kpiType = value;
                }
            }
        }

        public string Part
        {
            get
            {
                return part;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }
                else
                {
                    foreach (char symbol in value)
                    {
                        if (symbol < '0' || symbol > '9')
                        {
                            throw new ArgumentException();
                        }
                    }
                    part = value;
                }
            }
        }

        #endregion
    }
}
