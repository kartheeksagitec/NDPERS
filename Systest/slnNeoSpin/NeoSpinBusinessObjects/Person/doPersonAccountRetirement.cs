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
	/// Class NeoSpin.DataObjects.doPersonAccountRetirement:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountRetirement : doBase
    {
        
         public doPersonAccountRetirement() : base()
         {
         }
         public int person_account_id { get; set; }
         public DateTime dc_eligibility_date { get; set; }
         public string mutual_fund_window_flag { get; set; }
         public decimal capital_gain { get; set; }
         public decimal rhic_benfit_amount { get; set; }
         public string vesting_letter_sent_flag { get; set; }
         public string dc_file_sent_flag { get; set; }
         public string dc_trasnfer_reminder_letter1_flag { get; set; }
         public string dc_trasnfer_reminder_letter2_flag { get; set; }
         public string dc_trasnfer_reminder_letter3_flag { get; set; }
         public DateTime dc_trasnfer_reminder_letter1_date { get; set; }
         public int benefit_tier_id { get; set; }
         public string benefit_tier_description { get; set; }
         public string benefit_tier_value { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountRetirement
    {
         person_account_id ,
         dc_eligibility_date ,
         mutual_fund_window_flag ,
         capital_gain ,
         rhic_benfit_amount ,
         vesting_letter_sent_flag ,
         dc_file_sent_flag ,
         dc_trasnfer_reminder_letter1_flag ,
         dc_trasnfer_reminder_letter2_flag ,
         dc_trasnfer_reminder_letter3_flag ,
         dc_trasnfer_reminder_letter1_date ,
         benefit_tier_id ,
         benefit_tier_description ,
         benefit_tier_value ,
    }
}

