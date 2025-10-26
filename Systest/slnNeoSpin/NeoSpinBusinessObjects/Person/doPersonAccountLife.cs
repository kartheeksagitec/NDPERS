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
	/// Class NeoSpin.DataObjects.doPersonAccountLife:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountLife : doBase
    {
         
         public doPersonAccountLife() : base()
         {
         }
         public int person_account_id { get; set; }
         public int life_insurance_type_id { get; set; }
         public string life_insurance_type_description { get; set; }
         public string life_insurance_type_value { get; set; }
         public string premium_waiver_flag { get; set; }
         public DateTime projected_premium_waiver_date { get; set; }
         public DateTime actual_premium_waiver_date { get; set; }
         public decimal waived_amount { get; set; }
         public string disability_letter_sent_flag { get; set; }
         public int premium_waiver_provider_org_id { get; set; }
         public string modified_after_tffr_file_sent_flag { get; set; }
         public int reason_id { get; set; }
         public string reason_description { get; set; }
         public string reason_value { get; set; }
         public string accelerated_life_benefit_paid_flag { get; set; }
         public DateTime accelerated_life_benefit_effective_date { get; set; }
         public decimal accelerated_life_benefit_amount { get; set; }
         public string premium_conversion_indicator_flag { get; set; }
         public DateTime premium_conversion_effective_date { get; set; }
         public int ps_file_change_event_id { get; set; }
         public string ps_file_change_event_description { get; set; }
         public string ps_file_change_event_value { get; set; }
         public decimal spouse_waived_amount { get; set; }
         public decimal dependent_waived_amount { get; set; }
         public string ret_voya_sent { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountLife
    {
         person_account_id ,
         life_insurance_type_id ,
         life_insurance_type_description ,
         life_insurance_type_value ,
         premium_waiver_flag ,
         projected_premium_waiver_date ,
         actual_premium_waiver_date ,
         waived_amount ,
         disability_letter_sent_flag ,
         premium_waiver_provider_org_id ,
         modified_after_tffr_file_sent_flag ,
         reason_id ,
         reason_description ,
         reason_value ,
         accelerated_life_benefit_paid_flag ,
         accelerated_life_benefit_effective_date ,
         accelerated_life_benefit_amount ,
         premium_conversion_indicator_flag ,
         premium_conversion_effective_date ,
         ps_file_change_event_id ,
         ps_file_change_event_description ,
         ps_file_change_event_value ,
         spouse_waived_amount ,
         dependent_waived_amount ,
         ret_voya_sent ,
    }
}

