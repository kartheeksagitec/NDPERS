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
	/// Class NeoSpin.DataObjects.doMasBatchRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasBatchRequest : doBase
    {
         
         public doMasBatchRequest() : base()
         {
         }
         public int mas_batch_request_id { get; set; }
         public int group_type_id { get; set; }
         public string group_type_description { get; set; }
         public string group_type_value { get; set; }
         public int batch_request_type_id { get; set; }
         public string batch_request_type_description { get; set; }
         public string batch_request_type_value { get; set; }
         public DateTime statement_effective_date { get; set; }
         public int person_id { get; set; }
         public int plan_participation_status_id { get; set; }
         public string plan_participation_status_description { get; set; }
         public string plan_participation_status_value { get; set; }
         public int job_class_id { get; set; }
         public string job_class_description { get; set; }
         public string job_class_value { get; set; }
         public int employment_type_id { get; set; }
         public string employment_type_description { get; set; }
         public string employment_type_value { get; set; }
         public int org_id { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int batch_current_status_id { get; set; }
         public string batch_current_status_description { get; set; }
         public string batch_current_status_value { get; set; }
         public int action_status_id { get; set; }
         public string action_status_description { get; set; }
         public string action_status_value { get; set; }
         public int employment_status_id { get; set; }
         public string employment_status_description { get; set; }
         public string employment_status_value { get; set; }
         public int payee_org_id { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
         public int benefit_account_sub_type_id { get; set; }
         public string benefit_account_sub_type_description { get; set; }
         public string benefit_account_sub_type_value { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public string suppress_warnings_flag { get; set; }
         public string suppress_warnings_by { get; set; }
         public DateTime suppress_warnings_date { get; set; }
         public string bulk_insert_mas_data_flag { get; set; }
         public string mailing_generate_flag { get; set; }   // Annual Statements - PIR 17506
    }
    [Serializable]
    public enum enmMasBatchRequest
    {
         mas_batch_request_id ,
         group_type_id ,
         group_type_description ,
         group_type_value ,
         batch_request_type_id ,
         batch_request_type_description ,
         batch_request_type_value ,
         statement_effective_date ,
         person_id ,
         plan_participation_status_id ,
         plan_participation_status_description ,
         plan_participation_status_value ,
         job_class_id ,
         job_class_description ,
         job_class_value ,
         employment_type_id ,
         employment_type_description ,
         employment_type_value ,
         org_id ,
         status_id ,
         status_description ,
         status_value ,
         batch_current_status_id ,
         batch_current_status_description ,
         batch_current_status_value ,
         action_status_id ,
         action_status_description ,
         action_status_value ,
         employment_status_id ,
         employment_status_description ,
         employment_status_value ,
         payee_org_id ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
         benefit_account_sub_type_id ,
         benefit_account_sub_type_description ,
         benefit_account_sub_type_value ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         suppress_warnings_flag ,
         suppress_warnings_by ,
         suppress_warnings_date ,
         bulk_insert_mas_data_flag ,
         mailing_generate_flag ,
    }
}

