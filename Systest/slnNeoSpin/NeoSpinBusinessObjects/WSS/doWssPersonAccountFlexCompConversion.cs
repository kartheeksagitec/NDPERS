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
	/// Class NeoSpin.DataObjects.doWssPersonAccountFlexCompConversion:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAccountFlexCompConversion : doBase
    {
         
         public doWssPersonAccountFlexCompConversion() : base()
         {
         }
         public int wss_person_flex_comp_conversion_id { get; set; }
         public int wss_person_account_enrollment_request_id { get; set; }
         public int target_person_account_flex_comp_conversion_id { get; set; }
         public int org_id { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
    }
    [Serializable]
    public enum enmWssPersonAccountFlexCompConversion
    {
         wss_person_flex_comp_conversion_id ,
         wss_person_account_enrollment_request_id ,
         target_person_account_flex_comp_conversion_id ,
         org_id ,
         effective_start_date ,
         effective_end_date ,
    }
}

