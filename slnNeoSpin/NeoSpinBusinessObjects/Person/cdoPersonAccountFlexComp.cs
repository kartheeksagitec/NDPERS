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
	public class cdoPersonAccountFlexComp : doPersonAccountFlexComp
	{
		public cdoPersonAccountFlexComp() : base()
		{
		}

        public decimal msra_pledge_amount { get; set; }

        public decimal dcra_pledge_amount { get; set; }

        public int start_year { get; set; }
    } 
} 
