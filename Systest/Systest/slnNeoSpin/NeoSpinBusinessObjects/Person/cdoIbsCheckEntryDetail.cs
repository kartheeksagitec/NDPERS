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
	/// Class NeoSpin.CustomDataObjects.cdoIbsCheckEntryDetail:
	/// Inherited from doIbsCheckEntryDetail, the class is used to customize the database object doIbsCheckEntryDetail.
	/// </summary>
    [Serializable]
	public class cdoIbsCheckEntryDetail : doIbsCheckEntryDetail
	{
		public cdoIbsCheckEntryDetail() : base()
		{
		}
        public decimal due_amount { get; set; }
        public string name { get; set; }
        public string deposit_id_formatted
        {
            get
            {
                if (deposit_id > 0)
                    return deposit_id.ToString();
                else
                    return string.Empty;
            }
        }
    } 
} 
