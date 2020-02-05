using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Controllers
{
    public class MachineRequestData
    {
        #region DataMembers
        private string sensorID;
        
        private string type;
        #endregion


        #region Properties

        public string SensorID
        {
            get
            {
                return sensorID;
            }
            set
            {
                if(value == null)
                {
                    throw new ArgumentNullException("SensorID is null!");
                }
                else
                {
                    if (value[0] != 'M' && value[0] != 'm')
                    {
                        throw new ArgumentException("SensorID should begin with 'm'!");
                    }
                    else
                    {
                        for (var i = 1; i < value.Length; i++)
                        {
                            if (value[i] < '0' || value[i] > '9')
                            {
                                throw new ArgumentException("SensorID is not a number!");
                            }
                        }
                        sensorID = 'M' + value.Substring(1);
                    }
                }
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Machine type is null!");
                }
                else
                {
                    bool found = false;

                    foreach (string type in MachineTypes.types)
                    {
                        if(value == type)
                        {
                            found = true;
                        }
                    }
                    if(found == false)
                    {
                        throw new ArgumentException("Machine type isn't existing!");
                    }

                    type = value; 
                }
            }
        }
        #endregion
    }
}
