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
	public class cdoDeathNotification : doDeathNotification
	{
		public cdoDeathNotification() : base()
		{
		}

        public string death_type_value { get; set; } //Payee Death letter - ACOD/ALPD/FBED
    } 
} 
