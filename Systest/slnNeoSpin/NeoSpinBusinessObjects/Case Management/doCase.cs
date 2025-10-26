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
	/// Class NeoSpin.DataObjects.doCase:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCase : doBase
    {
         
         public doCase() : base()
         {
         }
         public int case_id { get; set; }
         public int person_id { get; set; }
         public int case_type_id { get; set; }
         public string case_type_description { get; set; }
         public string case_type_value { get; set; }
         public int appeal_type_id { get; set; }
         public string appeal_type_description { get; set; }
         public string appeal_type_value { get; set; }
         public int case_status_id { get; set; }
         public string case_status_description { get; set; }
         public string case_status_value { get; set; }
         public int overall_time { get; set; }
         public string comments { get; set; }
         public DateTime filenet_date_from { get; set; }
         public DateTime filenet_date_to { get; set; }
         public int action_by { get; set; }
         public int payee_account_id { get; set; }
         public DateTime recertification_date { get; set; }
         public DateTime next_recertification_date { get; set; }
         public string first_notification_flag { get; set; }
         public string second_notification_flag { get; set; }
         public string third_notification_flag { get; set; }
         public decimal comparable_earnings_amount { get; set; }
         public DateTime income_verification_date { get; set; }
         public string income_verification_flag { get; set; }
         public string employer_name1 { get; set; }
         public string employer_name2 { get; set; }
         public string employer_name3 { get; set; }
         public decimal financial_hardship_requested_amount { get; set; }
         public string disability_recertification_not_necessary_flag { get; set; }
    }
    [Serializable]
    public enum enmCase
    {
         case_id ,
         person_id ,
         case_type_id ,
         case_type_description ,
         case_type_value ,
         appeal_type_id ,
         appeal_type_description ,
         appeal_type_value ,
         case_status_id ,
         case_status_description ,
         case_status_value ,
         overall_time ,
         comments ,
         filenet_date_from ,
         filenet_date_to ,
         action_by ,
         payee_account_id ,
         recertification_date ,
         next_recertification_date ,
         first_notification_flag ,
         second_notification_flag ,
         third_notification_flag ,
         comparable_earnings_amount ,
         income_verification_date ,
         income_verification_flag ,
         employer_name1 ,
         employer_name2 ,
         employer_name3 ,
         financial_hardship_requested_amount ,
         disability_recertification_not_necessary_flag ,
    }
}

