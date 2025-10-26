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
	/// Class NeoSpin.CustomDataObjects.cdoWssDebitAchRequestDetail:
	/// Inherited from doWssDebitAchRequestDetail, the class is used to customize the database object doWssDebitAchRequestDetail.
	/// </summary>
    [Serializable]
	public class cdoWssDebitAchRequestDetail : doWssDebitAchRequestDetail
	{
		public cdoWssDebitAchRequestDetail() : base()
		{
		}
    } 
} 
