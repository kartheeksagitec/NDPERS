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
	/// Class NeoSpin.CustomDataObjects.cdoPaymentStepRef:
	/// Inherited from doPaymentStepRef, the class is used to customize the database object doPaymentStepRef.
	/// </summary>
    [Serializable]
	public class cdoPaymentStepRef : doPaymentStepRef
	{
		public cdoPaymentStepRef() : base()
		{
		}
    } 
} 
