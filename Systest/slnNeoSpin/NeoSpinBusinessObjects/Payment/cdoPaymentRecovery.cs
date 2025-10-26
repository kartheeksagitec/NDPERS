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
	/// <summary>
	/// Class NeoSpin.CustomDataObjects.cdoPaymentRecovery:
	/// Inherited from doPaymentRecovery, the class is used to customize the database object doPaymentRecovery.
	/// </summary>
    [Serializable]
	public class cdoPaymentRecovery : doPaymentRecovery
	{
		public cdoPaymentRecovery() : base()
		{
		}

        public int person_id { get; set; }

        public int org_id { get; set; }

        public string istrSuppressWarning { get; set; }
    } 
} 
