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
	public class cdoPersonAccountAdjustment : doPersonAccountAdjustment
	{
		public cdoPersonAccountAdjustment() : base()
		{
		}

        public string istrSuppressWarning { get; set; }
    } 
} 
