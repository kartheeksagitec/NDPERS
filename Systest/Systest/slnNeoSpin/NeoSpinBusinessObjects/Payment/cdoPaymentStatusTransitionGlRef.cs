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
	/// Class NeoSpin.CustomDataObjects.cdoPaymentStatusTransitionGlRef:
	/// Inherited from doPaymentStatusTransitionGlRef, the class is used to customize the database object doPaymentStatusTransitionGlRef.
	/// </summary>
    [Serializable]
	public class cdoPaymentStatusTransitionGlRef : doPaymentStatusTransitionGlRef
	{
		public cdoPaymentStatusTransitionGlRef() : base()
		{
		}
    } 
} 
