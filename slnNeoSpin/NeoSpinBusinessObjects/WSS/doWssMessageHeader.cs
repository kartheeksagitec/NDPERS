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
	/// Class NeoSpin.DataObjects.doWssMessageHeader:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssMessageHeader : doBase
    {
        
         public doWssMessageHeader() : base()
         {
         }
         public int wss_message_id { get; set; }
         public int message_id { get; set; }
         public string message_text { get; set; }
         public int priority_id { get; set; }
         public string priority_description { get; set; }
         public string priority_value { get; set; }
         public int audience_id { get; set; }
         public string audience_description { get; set; }
         public string audience_value { get; set; }
         public int person_id { get; set; }
         public int plan_id { get; set; }
         public int person_type_id { get; set; }
         public string person_type_description { get; set; }
         public string person_type_value { get; set; }
         public int org_id { get; set; }
         public int contact_role_id { get; set; }
         public string contact_role_description { get; set; }
         public int emp_category_id { get; set; }
         public string emp_category_description { get; set; }
         public string emp_category_value { get; set; }
         public string contact_role_value { get; set; }
         public int contact_id { get; set; }
         
         public string central_payroll_flag { get; set; }
         public int peoplesoft_org_group_id { get; set; }
         public string peoplesoft_org_group_description { get; set; }
         public string peoplesoft_org_group_value { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public int benefit_type_id { get; set; }
         public string benefit_type_description { get; set; }
         public string benefit_type_value { get; set; }
         public int member_type_id { get; set; }
         public string member_type_description { get; set; }
         public string member_type_value { get; set; }
         public int type_id { get; set; }
         public string type_description { get; set; }
         public string type_value { get; set; }
         public int job_class_id { get; set; }
         public string job_class_description { get; set; }
         public string job_class_value { get; set; }
         public DateTime benefit_begin_date_from { get; set; }
         public DateTime benefit_begin_date_to { get; set; }
         public string is_message_sent { get; set; }
		 public string wss_message_request_flag { get; set; }
    }
    [Serializable]
    public enum enmWssMessageHeader
    {
         wss_message_id ,
         message_id ,
         message_text ,
         priority_id ,
         priority_description ,
         priority_value ,
         audience_id ,
         audience_description ,
         audience_value ,
         person_id ,
         plan_id ,
         person_type_id ,
         person_type_description ,
         person_type_value ,
         org_id ,
         contact_role_id ,
         contact_role_description ,
         emp_category_id ,
         emp_category_description ,
         emp_category_value ,
         contact_role_value ,
         contact_id ,
         
         central_payroll_flag ,
         peoplesoft_org_group_id ,
         peoplesoft_org_group_description ,
         peoplesoft_org_group_value ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         benefit_type_id ,
         benefit_type_description ,
         benefit_type_value ,
         member_type_id ,
         member_type_description ,
         member_type_value ,
         type_id ,
         type_description ,
         type_value ,
         job_class_id ,
         job_class_description ,
         job_class_value ,
         benefit_begin_date_from ,
         benefit_begin_date_to ,
         is_message_sent ,
		 wss_message_request_flag ,
    }
}

