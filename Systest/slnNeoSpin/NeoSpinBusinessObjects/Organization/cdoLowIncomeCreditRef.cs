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
	/// Class NeoSpin.CustomDataObjects.cdoLowIncomeCreditRef:
	/// Inherited from doLowIncomeCreditRef, the class is used to customize the database object doLowIncomeCreditRef.
	/// </summary>
    [Serializable]
	public class cdoLowIncomeCreditRef : doLowIncomeCreditRef
	{
		public cdoLowIncomeCreditRef() : base()
		{
           
		}
        public string display_credit { get; set; }
    } 
} 
