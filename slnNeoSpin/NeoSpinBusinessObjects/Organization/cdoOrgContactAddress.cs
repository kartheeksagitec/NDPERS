#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.CustomDataObjects
{
    [Serializable]
	public class cdoOrgContactAddress : doOrgContactAddress
	{
		public cdoOrgContactAddress() : base()
		{
		}
       
        public string lstrAddr_line_1
        {
            get
            {
                return !String.IsNullOrEmpty(addr_line_1) ? addr_line_1.ToUpper() : addr_line_1;
            }
        }
        public string lstraddr_line_2
        {
            get
            {
                return !String.IsNullOrEmpty(addr_line_2) ? addr_line_2.ToUpper() : addr_line_2;
            }
        }
        public string lstrcity
        {
            get
            {
                return !String.IsNullOrEmpty(city) ? city.ToUpper() : city;
            }
        }
        public string lstrstate_value
        {
            get
            {
                return !String.IsNullOrEmpty(state_value) ? state_value.ToUpper() : status_value;
            }
        }

        // PROD PIR ID 6785
        public string zip_code_value
        {
            get
            {
                string lstrValue = string.Empty;
                if (!string.IsNullOrEmpty(zip_code))
                    lstrValue = zip_code;
                if (!string.IsNullOrEmpty(zip_4_code))
                    lstrValue += "-" + zip_4_code;
                return lstrValue;
            }
        }
    } 
} 
