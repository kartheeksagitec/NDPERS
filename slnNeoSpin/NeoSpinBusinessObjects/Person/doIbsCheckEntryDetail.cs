#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sagitec.Common;
using Sagitec.DataObjects;

#endregion

namespace NeoSpin.DataObjects
{
	/// <summary>
	/// Class NeoSpin.DataObjects.doIbsCheckEntryDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIbsCheckEntryDetail : doBase
    {
         
         public doIbsCheckEntryDetail() : base()
         {
         }
         public int ibs_check_entry_detail_id { get; set; }
         public int ibs_check_entry_header_id { get; set; }
         public int person_id { get; set; }
         public DateTime payment_date { get; set; }
         public string reference_no { get; set; }
         public decimal amount_paid { get; set; }
         public int deposit_id { get; set; }
    }
    [Serializable]
    public enum enmIbsCheckEntryDetail
    {
         ibs_check_entry_detail_id ,
         ibs_check_entry_header_id ,
         person_id ,
         payment_date ,
         reference_no ,
         amount_paid ,
         deposit_id ,
    }
}

