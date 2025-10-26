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
	/// Class NeoSpin.DataObjects.doWssAutoPostingCrossRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssAutoPostingCrossRef : doBase
    {
         
         public doWssAutoPostingCrossRef() : base()
         {
         }
         public int auto_posting_cross_ref_id { get; set; }
         public int plan_id { get; set; }
         public int plan_enrollment_option_id { get; set; }
         public string plan_enrollment_option_description { get; set; }
         public string plan_enrollment_option_value { get; set; }
         public int change_reason_id { get; set; }
         public string change_reason_description { get; set; }
         public string change_reason_value { get; set; }
         public int change_effective_date_id { get; set; }
         public string change_effective_date_description { get; set; }
         public string change_effective_date_value { get; set; }
         public string auto_post_flag { get; set; }
         public int workflow_process_id { get; set; }
         public string prompt_user_text { get; set; }
         public string document { get; set; }
    }
    [Serializable]
    public enum enmWssAutoPostingCrossRef
    {
         auto_posting_cross_ref_id ,
         plan_id ,
         plan_enrollment_option_id ,
         plan_enrollment_option_description ,
         plan_enrollment_option_value ,
         change_reason_id ,
         change_reason_description ,
         change_reason_value ,
         change_effective_date_id ,
         change_effective_date_description ,
         change_effective_date_value ,
         auto_post_flag ,
         workflow_process_id ,
         prompt_user_text ,
         document ,
    }
}

