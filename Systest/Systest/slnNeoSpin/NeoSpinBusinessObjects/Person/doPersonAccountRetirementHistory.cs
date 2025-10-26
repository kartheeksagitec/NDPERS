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
	/// Class NeoSpin.DataObjects.doPersonAccountRetirementHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountRetirementHistory : doBase
    {
         
         public doPersonAccountRetirementHistory() : base()
         {
         }
         public int person_account_retirement_history_id { get; set; }
         public int person_account_id { get; set; }
         public DateTime dc_eligibility_date { get; set; }
         public string mutual_fund_window_flag { get; set; }
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
         public decimal capital_gain { get; set; }
         public decimal rhic_benfit_amount { get; set; }
         public string vesting_letter_sent_flag { get; set; }
         public int provider_org_id { get; set; }
         public string is_enrollment_report_generated { get; set; }
        //PIR 25920 New Plan DC 2025
         public int addl_ee_contribution_percent { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountRetirementHistory
    {
         person_account_retirement_history_id ,
         person_account_id ,
         dc_eligibility_date ,
         mutual_fund_window_flag ,
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
         capital_gain ,
         rhic_benfit_amount ,
         vesting_letter_sent_flag ,
         provider_org_id ,
         is_enrollment_report_generated,
         addl_ee_contribution_percent,
    }
}

