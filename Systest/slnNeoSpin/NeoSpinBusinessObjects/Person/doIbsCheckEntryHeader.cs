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
	/// Class NeoSpin.DataObjects.doIbsCheckEntryHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doIbsCheckEntryHeader : doBase
    {
         
         public doIbsCheckEntryHeader() : base()
         {
         }
         public int ibs_check_entry_header_id { get; set; }
         public DateTime deposit_date { get; set; }
         public int deposit_method_id { get; set; }
         public string deposit_method_description { get; set; }
         public string deposit_method_value { get; set; }
         public int deposit_tape_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public decimal deposit_total { get; set; }
    }
    [Serializable]
    public enum enmIbsCheckEntryHeader
    {
         ibs_check_entry_header_id ,
         deposit_date ,
         deposit_method_id ,
         deposit_method_description ,
         deposit_method_value ,
         deposit_tape_id ,
         status_id ,
         status_description ,
         status_value ,
         deposit_total ,
    }
}

