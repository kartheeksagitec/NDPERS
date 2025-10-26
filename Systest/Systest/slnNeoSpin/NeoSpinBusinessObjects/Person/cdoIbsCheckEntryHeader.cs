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
	/// Class NeoSpin.CustomDataObjects.cdoIbsCheckEntryHeader:
	/// Inherited from doIbsCheckEntryHeader, the class is used to customize the database object doIbsCheckEntryHeader.
	/// </summary>
    [Serializable]
	public class cdoIbsCheckEntryHeader : doIbsCheckEntryHeader
	{
		public cdoIbsCheckEntryHeader() : base()
		{
		}

        public decimal idecTotalAmountFromDetail { get; set; }
    } 
} 
