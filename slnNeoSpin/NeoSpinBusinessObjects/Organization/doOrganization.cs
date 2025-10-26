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
	/// Class NeoSpin.DataObjects.doOrganization:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doOrganization : doBase
    {
         
         public doOrganization() : base()
         {
         }
         public int org_id { get; set; }
         public string org_name { get; set; }
         public int org_type_id { get; set; }
         public string org_type_description { get; set; }
         public string org_type_value { get; set; }
         public int reporting_method_id { get; set; }
         public string reporting_method_description { get; set; }
         public string reporting_method_value { get; set; }
         public int remittance_method_id { get; set; }
         public string remittance_method_description { get; set; }
         public string remittance_method_value { get; set; }
         public string federal_id { get; set; }
         public string org_code { get; set; }
         public string legacy_dept_no { get; set; }
         public string telephone { get; set; }
         public string email { get; set; }
         public string fax { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public int primary_address_id { get; set; }
         public int primary_contact_id { get; set; }
         public int emp_category_id { get; set; }
         public string emp_category_description { get; set; }
         public string emp_category_value { get; set; }
         public string routing_no { get; set; }
         public string central_payroll_flag { get; set; }
         public string interoffice_mail_flag { get; set; }
         public string emp_trans_report_flag { get; set; }
         public int provider_type_id { get; set; }
         public string provider_type_description { get; set; }
         public string provider_type_value { get; set; }
         public string early_retirement_incentive_purchase_agreement { get; set; }
         public int peoplesoft_org_group_id { get; set; }
         public string peoplesoft_org_group_description { get; set; }
         public string peoplesoft_org_group_value { get; set; }
         public string hipaa_reference_id { get; set; }
         public string hipaa_branch_id { get; set; }
         public string wire_transfer_repeat_code { get; set; }
         public int payment_option_id { get; set; }
         public string payment_option_description { get; set; }
         public string payment_option_value { get; set; }
         public string company_for_people_soft_file { get; set; }
         public string business_unit { get; set; }
         public string ach_account_number { get; set; }
         public string pre_tax_purchase { get; set; }
         public int mss_access_id { get; set; }
         public string mss_access_description { get; set; }
         public string mss_access_value { get; set; }
         public string telephone_no_extn { get; set; }
    }
    [Serializable]
    public enum enmOrganization
    {
         org_id ,
         org_name ,
         org_type_id ,
         org_type_description ,
         org_type_value ,
         reporting_method_id ,
         reporting_method_description ,
         reporting_method_value ,
         remittance_method_id ,
         remittance_method_description ,
         remittance_method_value ,
         federal_id ,
         org_code ,
         legacy_dept_no ,
         telephone ,
         email ,
         fax ,
         status_id ,
         status_description ,
         status_value ,
         primary_address_id ,
         primary_contact_id ,
         emp_category_id ,
         emp_category_description ,
         emp_category_value ,
         routing_no ,
         central_payroll_flag ,
         interoffice_mail_flag ,
         emp_trans_report_flag ,
         provider_type_id ,
         provider_type_description ,
         provider_type_value ,
         early_retirement_incentive_purchase_agreement ,
         peoplesoft_org_group_id ,
         peoplesoft_org_group_description ,
         peoplesoft_org_group_value ,
         hipaa_reference_id ,
         hipaa_branch_id ,
         wire_transfer_repeat_code ,
         payment_option_id ,
         payment_option_description ,
         payment_option_value ,
         company_for_people_soft_file ,
         business_unit ,
         ach_account_number ,
         pre_tax_purchase ,
         mss_access_id ,
         mss_access_description ,
         mss_access_value ,
         telephone_no_extn ,
    }
}

