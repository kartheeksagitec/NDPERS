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
	/// Class NeoSpin.CustomDataObjects.cdoMasSelection:
	/// Inherited from doMasSelection, the class is used to customize the database object doMasSelection.
	/// </summary>
    [Serializable]
	public class cdoMasSelection : doMasSelection
	{
		public cdoMasSelection() : base()
		{
		}

        public int payee_account_id { get; set; }
    } 
} 
