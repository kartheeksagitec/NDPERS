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
	/// Class NeoSpin.DataObjects.doPersonAccountLtcOptionHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountLtcOptionHistory : doBase
    {
         
         public doPersonAccountLtcOptionHistory() : base()
         {
         }
         public int person_account_ltc_option_history_id { get; set; }
         public int person_account_ltc_option_id { get; set; }
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
         public int plan_option_status_id { get; set; }
         public string plan_option_status_description { get; set; }
         public string plan_option_status_value { get; set; }
         public int ltc_relationship_id { get; set; }
         public string ltc_relationship_description { get; set; }
         public string ltc_relationship_value { get; set; }
         public int ltc_insurance_type_id { get; set; }
         public string ltc_insurance_type_description { get; set; }
         public string ltc_insurance_type_value { get; set; }
         public int level_of_coverage_id { get; set; }
         public string level_of_coverage_description { get; set; }
         public string level_of_coverage_value { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public int person_id { get; set; }
         public int provider_org_id { get; set; }
         public int reason_id { get; set; }
         public string reason_description { get; set; }
         public string reason_value { get; set; }
         public string is_enrollment_report_generated { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountLtcOptionHistory
    {
         person_account_ltc_option_history_id ,
         person_account_ltc_option_id ,
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
         plan_option_status_id ,
         plan_option_status_description ,
         plan_option_status_value ,
         ltc_relationship_id ,
         ltc_relationship_description ,
         ltc_relationship_value ,
         ltc_insurance_type_id ,
         ltc_insurance_type_description ,
         ltc_insurance_type_value ,
         level_of_coverage_id ,
         level_of_coverage_description ,
         level_of_coverage_value ,
         effective_start_date ,
         effective_end_date ,
         person_id ,
         provider_org_id ,
         reason_id ,
         reason_description ,
         reason_value ,
         is_enrollment_report_generated ,
    }
}

