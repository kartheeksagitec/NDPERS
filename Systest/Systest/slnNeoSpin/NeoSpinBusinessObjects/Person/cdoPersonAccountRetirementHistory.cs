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
	public class cdoPersonAccountRetirementHistory : doPersonAccountRetirementHistory
	{
		public cdoPersonAccountRetirementHistory() : base()
		{
            
		}
        public string strAddl_ee_contribution_percent
        {
            get
            { 
                return Convert.ToString(addl_ee_contribution_percent);
            }
        }
    } 
} 
