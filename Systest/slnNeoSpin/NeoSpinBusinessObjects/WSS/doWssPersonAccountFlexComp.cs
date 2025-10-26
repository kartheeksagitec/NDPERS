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
	/// Class NeoSpin.DataObjects.doWssPersonAccountFlexComp:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAccountFlexComp : doBase
    {
         
         public doWssPersonAccountFlexComp() : base()
         {
         }
         public int wss_person_account_flex_comp_id { get; set; }
         public int wss_person_account_enrollment_request_id { get; set; }
         public string premium_conversion_waiver_flag { get; set; }
         public string direct_deposit_flag { get; set; }
         public string inside_mail_flag { get; set; }
         public int flex_comp_type_id { get; set; }
         public string flex_comp_type_description { get; set; }
         public string flex_comp_type_value { get; set; }
    }
    [Serializable]
    public enum enmWssPersonAccountFlexComp
    {
         wss_person_account_flex_comp_id ,
         wss_person_account_enrollment_request_id ,
         premium_conversion_waiver_flag ,
         direct_deposit_flag ,
         inside_mail_flag ,
         flex_comp_type_id ,
         flex_comp_type_description ,
         flex_comp_type_value ,
    }
}

