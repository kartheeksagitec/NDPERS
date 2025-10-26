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
	/// Class NeoSpin.DataObjects.doPersonAccountDeferredCompHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountDeferredCompHistory : doBase
    {
         
         public doPersonAccountDeferredCompHistory() : base()
         {
         }
         public int person_account_deferred_comp_history_id { get; set; }
         public int person_account_id { get; set; }
         public DateTime catch_up_start_date { get; set; }
         public DateTime catch_up_end_date { get; set; }
         public int limit_457_id { get; set; }
         public string limit_457_description { get; set; }
         public string limit_457_value { get; set; }
         public string hardship_withdrawal_flag { get; set; }
         public DateTime hardship_withdrawal_effective_date { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int plan_participation_status_id { get; set; }
         public string plan_participation_status_description { get; set; }
         public string plan_participation_status_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int from_person_account_id { get; set; }
         public int to_person_account_id { get; set; }
         public string suppress_warnings_flag { get; set; }
         public string suppress_warnings_by { get; set; }
         public DateTime suppress_warnings_date { get; set; }
         public string de_minimus_distribution_flag { get; set; }
         public string is_enrollment_report_generated { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountDeferredCompHistory
    {
         person_account_deferred_comp_history_id ,
         person_account_id ,
         catch_up_start_date ,
         catch_up_end_date ,
         limit_457_id ,
         limit_457_description ,
         limit_457_value ,
         hardship_withdrawal_flag ,
         hardship_withdrawal_effective_date ,
         start_date ,
         end_date ,
         plan_participation_status_id ,
         plan_participation_status_description ,
         plan_participation_status_value ,
         status_id ,
         status_description ,
         status_value ,
         from_person_account_id ,
         to_person_account_id ,
         suppress_warnings_flag ,
         suppress_warnings_by ,
         suppress_warnings_date ,
         de_minimus_distribution_flag ,
         is_enrollment_report_generated ,
    }
}

