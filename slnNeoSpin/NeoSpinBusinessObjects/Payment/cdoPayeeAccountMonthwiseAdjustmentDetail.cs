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
	/// Class NeoSpin.CustomDataObjects.cdoPayeeAccountMonthwiseAdjustmentDetail:
	/// Inherited from doPayeeAccountMonthwiseAdjustmentDetail, the class is used to customize the database object doPayeeAccountMonthwiseAdjustmentDetail.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountMonthwiseAdjustmentDetail : doPayeeAccountMonthwiseAdjustmentDetail
	{
		public cdoPayeeAccountMonthwiseAdjustmentDetail() : base()
		{
		}
    } 
} 
