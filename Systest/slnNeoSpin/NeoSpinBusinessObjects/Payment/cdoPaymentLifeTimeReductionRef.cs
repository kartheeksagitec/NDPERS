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
	/// Class NeoSpin.CustomDataObjects.cdoPaymentLifeTimeReductionRef:
	/// Inherited from doPaymentLifeTimeReductionRef, the class is used to customize the database object doPaymentLifeTimeReductionRef.
	/// </summary>
    [Serializable]
	public class cdoPaymentLifeTimeReductionRef : doPaymentLifeTimeReductionRef
	{
		public cdoPaymentLifeTimeReductionRef() : base()
		{
		}
    } 
} 
