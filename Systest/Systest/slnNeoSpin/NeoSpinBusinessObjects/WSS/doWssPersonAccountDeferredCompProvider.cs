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
	/// Class NeoSpin.DataObjects.doWssPersonAccountDeferredCompProvider:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAccountDeferredCompProvider : doBase
    {
         
         public doWssPersonAccountDeferredCompProvider() : base()
         {
         }
         public int wss_person_account_deferred_comp_provider_id { get; set; }
         public int wss_person_account_deferred_comp_id { get; set; }
         public int provider_org_plan_id { get; set; }
         public string company_name { get; set; }
         public int provider_agent_contact_id { get; set; }
         public decimal per_pay_period_contribution_amt { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public string mutual_fund_window_flag { get; set; }
         public int person_employment_id { get; set; }
         public int assets_with_provider_id { get; set; }
         public string assets_with_provider_description { get; set; }
         public string assets_with_provider_value { get; set; }
         public string comments { get; set; }
         public int payment_status_id { get; set; }
         public string payment_status_description { get; set; }
         public string payment_status_value { get; set; }
         public string update_existing_provider { get; set; }
    }
    [Serializable]
    public enum enmWssPersonAccountDeferredCompProvider
    {
         wss_person_account_deferred_comp_provider_id ,
         wss_person_account_deferred_comp_id ,
         provider_org_plan_id ,
         company_name ,
         provider_agent_contact_id ,
         per_pay_period_contribution_amt ,
         start_date ,
         end_date ,
         mutual_fund_window_flag ,
         person_employment_id ,
         assets_with_provider_id ,
         assets_with_provider_description ,
         assets_with_provider_value ,
         comments ,
         payment_status_id ,
         payment_status_description ,
         payment_status_value ,
         update_existing_provider ,
    }
}

