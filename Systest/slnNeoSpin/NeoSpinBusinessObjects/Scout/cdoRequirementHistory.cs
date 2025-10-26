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
	public class cdoRequirementHistory : doRequirementHistory
	{
		public cdoRequirementHistory() : base()
		{
		}
        private String _Parent_Requirement_Key;
        public String Parent_Requirement_Key
        {
            get { return _Parent_Requirement_Key; }
            set { _Parent_Requirement_Key = value; }
        }
        
    } 
} 
