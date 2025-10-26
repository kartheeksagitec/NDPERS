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
	/// Class NeoSpin.DataObjects.doWssMemberRecordRequest:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssMemberRecordRequest : doBase
    {
         public doWssMemberRecordRequest() : base()
         {
         }
         public int member_record_request_id { get; set; }
         public int person_id { get; set; }
         public string first_name { get; set; }
         public string middle_name { get; set; }
         public string last_name { get; set; }
         public int name_prefix_id { get; set; }
         public string name_prefix_description { get; set; }
         public string name_prefix_value { get; set; }
         public int name_suffix_id { get; set; }
         public string name_suffix_description { get; set; }
         public string name_suffix_value { get; set; }
         public DateTime date_of_birth { get; set; }
         public string ssn { get; set; }
         public int gender_id { get; set; }
         public string gender_description { get; set; }
         public string gender_value { get; set; }
         public int marital_status_id { get; set; }
         public string marital_status_description { get; set; }
         public string marital_status_value { get; set; }
         public string home_phone_no { get; set; }
         public string work_phone_no { get; set; }
         public string cell_phone_no { get; set; }
         public string posted_in_perslink_by { get; set; }
         public DateTime posted_date { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public DateTime date_of_last_regular_pay_check { get; set; }
         public DateTime first_month_for_retirement_contribution { get; set; }
         public string rejection_reason { get; set; }
         public int contact_id { get; set; }
         public int peoplesoft_id { get; set; }
         public DateTime marital_status_date { get; set; }
         public string work_phone_ext { get; set; }
         public string ps_initiated_flag { get; set; }
         public string bpm_initiated { get; set; }
    }
    [Serializable]
    public enum enmWssMemberRecordRequest
    {
         member_record_request_id ,
         person_id ,
         first_name ,
         middle_name ,
         last_name ,
         name_prefix_id ,
         name_prefix_description ,
         name_prefix_value ,
         name_suffix_id ,
         name_suffix_description ,
         name_suffix_value ,
         date_of_birth ,
         ssn ,
         gender_id ,
         gender_description ,
         gender_value ,
         marital_status_id ,
         marital_status_description ,
         marital_status_value ,
         home_phone_no ,
         work_phone_no ,
         cell_phone_no ,
         posted_in_perslink_by ,
         posted_date ,
         status_id ,
         status_description ,
         status_value ,
         date_of_last_regular_pay_check ,
         first_month_for_retirement_contribution,
         rejection_reason ,
         contact_id ,
         peoplesoft_id ,
         marital_status_date ,
         work_phone_ext ,
         ps_initiated_flag ,
         bpm_initiated ,
    }
}

