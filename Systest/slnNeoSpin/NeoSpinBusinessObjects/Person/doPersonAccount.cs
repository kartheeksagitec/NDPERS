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
	/// Class NeoSpin.DataObjects.doPersonAccount:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccount : doBase
    {
         
         public doPersonAccount() : base()
         {
         }
         public int person_account_id { get; set; }
         public int person_id { get; set; }
         public int plan_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public int provider_org_id { get; set; }
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
         public DateTime history_change_date { get; set; }
         public DateTime cobra_expiration_date { get; set; }
         public string people_soft_file_sent_flag { get; set; }
         public string rmd_batch_initiated_flag { get; set; }
         public string def_comp_month_letter_flag { get; set; }
         public string def_comp_yearly_letter_flag { get; set; }
         public int reason_id { get; set; }
         public string reason_description { get; set; }
         public string reason_value { get; set; }
         public string npsp_flexcomp_flag { get; set; }
         public DateTime npsp_flexcomp_change_date { get; set; }
         public int ps_file_change_event_id { get; set; }
         public string ps_file_change_event_description { get; set; }
         public string ps_file_change_event_value { get; set; }
         public string no_bene_sent { get; set; }
		 //PIR 25920 New Plan DC 2025
         public int addl_ee_contribution_percent { get; set; }
        public int addl_ee_contribution_percent_temp { get; set; }
        public DateTime addl_ee_contribution_percent_end_date { get; set; }
        public DateTime addl_ee_contribution_percent_temp_end_date { get; set; }
		public string dc25_reminder_letter_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonAccount
    {
         person_account_id ,
         person_id ,
         plan_id ,
         start_date ,
         end_date ,
         provider_org_id ,
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
         history_change_date ,
         cobra_expiration_date ,
         people_soft_file_sent_flag ,
         rmd_batch_initiated_flag ,
         def_comp_month_letter_flag ,
         def_comp_yearly_letter_flag ,
         reason_id ,
         reason_description ,
         reason_value ,
         npsp_flexcomp_flag ,
         npsp_flexcomp_change_date ,
         ps_file_change_event_id ,
         ps_file_change_event_description ,
         ps_file_change_event_value ,
         no_bene_sent ,
         addl_ee_contribution_percent,
         addl_ee_contribution_percent_temp,
         addl_ee_contribution_percent_end_date,
        addl_ee_contribution_percent_temp_end_date,
		dc25_reminder_letter_flag ,
    }
}

