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
	/// Class NeoSpin.DataObjects.doPersonAccountFlexComp:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountFlexComp : doBase
    {
         
         public doPersonAccountFlexComp() : base()
         {
         }
         public int person_account_id { get; set; }
         public string premium_conversion_waiver_flag { get; set; }
         public string direct_deposit_flag { get; set; }
         public string inside_mail_flag { get; set; }
         public int flex_comp_type_id { get; set; }
         public string flex_comp_type_description { get; set; }
         public string flex_comp_type_value { get; set; }
         public int reason_id { get; set; }
         public string reason_description { get; set; }
         public string reason_value { get; set; }
         public int ps_file_change_event_id { get; set; }
         public string ps_file_change_event_description { get; set; }
         public string ps_file_change_event_value { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountFlexComp
    {
         person_account_id ,
         premium_conversion_waiver_flag ,
         direct_deposit_flag ,
         inside_mail_flag ,
         flex_comp_type_id ,
         flex_comp_type_description ,
         flex_comp_type_value ,
         reason_id ,
         reason_description ,
         reason_value ,
         ps_file_change_event_id ,
         ps_file_change_event_description ,
         ps_file_change_event_value ,
    }
}

