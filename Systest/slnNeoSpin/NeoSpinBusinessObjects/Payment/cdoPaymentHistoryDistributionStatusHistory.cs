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
	/// Class NeoSpin.CustomDataObjects.cdoPaymentHistoryDistributionStatusHistory:
	/// Inherited from doPaymentHistoryDistributionStatusHistory, the class is used to customize the database object doPaymentHistoryDistributionStatusHistory.
	/// </summary>
    [Serializable]
	public class cdoPaymentHistoryDistributionStatusHistory : doPaymentHistoryDistributionStatusHistory
	{
		public cdoPaymentHistoryDistributionStatusHistory() : base()
		{
		}
    } 
} 
