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
	/// Class NeoSpin.CustomDataObjects.cdoPaymentRecoveryHistory:
	/// Inherited from doPaymentRecoveryHistory, the class is used to customize the database object doPaymentRecoveryHistory.
	/// </summary>
    [Serializable]
	public class cdoPaymentRecoveryHistory : doPaymentRecoveryHistory
	{
		public cdoPaymentRecoveryHistory() : base()
		{
		}

        public int iintControlNumber { get; set; }

        public int iintPaymentScheduleID { get; set; }
    } 
} 
