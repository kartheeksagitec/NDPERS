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
	/// Class NeoSpin.CustomDataObjects.cdoPayeeAccountMinimumGuaranteeHistory:
	/// Inherited from doPayeeAccountMinimumGuaranteeHistory, the class is used to customize the database object doPayeeAccountMinimumGuaranteeHistory.
	/// </summary>
    [Serializable]
	public class cdoPayeeAccountMinimumGuaranteeHistory : doPayeeAccountMinimumGuaranteeHistory
	{
		public cdoPayeeAccountMinimumGuaranteeHistory() : base()
		{
		}
    } 
} 
