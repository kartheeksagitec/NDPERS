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
	/// Class NeoSpin.CustomDataObjects.cdoPayeeAccountRetroPaymentRecoveryDetail:
	/// Inherited from doPayeeAccountRetroPaymentRecoveryDetail, the class is used to customize the database object doPayeeAccountRetroPaymentRecoveryDetail.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountRetroPaymentRecoveryDetail : doPayeeAccountRetroPaymentRecoveryDetail
	{
		public cdoPayeeAccountRetroPaymentRecoveryDetail() : base()
		{
		}
    } 
} 
