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
	/// Class NeoSpin.CustomDataObjects.cdoRateChangeLetterDetail:
	/// Inherited from doRateChangeLetterDetail, the class is used to customize the database object doRateChangeLetterDetail.
	/// </summary>
    [Serializable]
	public class cdoRateChangeLetterDetail : doRateChangeLetterDetail
	{
		public cdoRateChangeLetterDetail() : base()
		{
		}
    } 
} 
