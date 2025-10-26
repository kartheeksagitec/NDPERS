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
	/// Class NeoSpin.DataObjects.doDeathNotification:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doDeathNotification : doBase
    {
         public doDeathNotification() : base()
         {
         }
         public int death_notification_id { get; set; }
         public int person_id { get; set; }
         public string last_name { get; set; }
         public string first_name { get; set; }
         public DateTime date_of_death { get; set; }
         public string death_certified_flag { get; set; }
         public DateTime date_of_death_certified_on { get; set; }
         public DateTime life_claim_payment_date { get; set; }
         public decimal life_claim_payment_amount { get; set; }
         public int action_status_id { get; set; }
         public string action_status_description { get; set; }
         public string action_status_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string suppress_warnings_flag { get; set; }
         public string suppress_warnings_by { get; set; }
         public DateTime suppress_warnings_date { get; set; }
         public string workflow_initiated_flag { get; set; }
         public string non_employee_death_batch_flag { get; set; }
         public string is_payee_death_letter_sent_flag { get; set; }
         public string employee_death_batch_letter_sent { get; set; }
    }
    [Serializable]
    public enum enmDeathNotification
    {
         death_notification_id ,
         person_id ,
         last_name ,
         first_name ,
         date_of_death ,
         death_certified_flag ,
         date_of_death_certified_on ,
         life_claim_payment_date ,
         life_claim_payment_amount ,
         action_status_id ,
         action_status_description ,
         action_status_value ,
         status_id ,
         status_description ,
         status_value ,
         suppress_warnings_flag ,
         suppress_warnings_by ,
         suppress_warnings_date ,
         workflow_initiated_flag ,
         non_employee_death_batch_flag ,
         is_payee_death_letter_sent_flag ,
         employee_death_batch_letter_sent ,
    }
}

