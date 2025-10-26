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
	/// Class NeoSpin.DataObjects.doActuaryFileRhicDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doActuaryFileRhicDetail : doBase
    {
         
         public doActuaryFileRhicDetail() : base()
         {
         }
         public int actuary_file_rhic_detail_id { get; set; }
         public int actuary_file_header_id { get; set; }
         public int person_id { get; set; }
         public int payee_account_id { get; set; }
         public int person_account_id { get; set; }
         public int account_relation_id { get; set; }
         public string account_relation_description { get; set; }
         public string account_relation_value { get; set; }
         public int payee_status_id { get; set; }
         public string payee_status_description { get; set; }
         public string payee_status_value { get; set; }
         public int marital_status_id { get; set; }
         public string marital_status_description { get; set; }
         public string marital_status_value { get; set; }
         public DateTime rhic_benefit_begin_date { get; set; }
         public DateTime plan_participation_start_date { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public decimal pension_service_credit { get; set; }
         public decimal total_vested_service_credit { get; set; }
         public decimal monthly_rhic_amount { get; set; }
         public decimal applied_ghdv_rhic_amount { get; set; }
         public string coverage_code { get; set; }
         public DateTime joint_annuitant_dob { get; set; }
         public int joint_annuitant_gender_id { get; set; }
         public string joint_annuitant_gender_description { get; set; }
         public string joint_annuitant_gender_value { get; set; }
         public int rhic_option_id { get; set; }
         public string rhic_option_description { get; set; }
         public string rhic_option_value { get; set; }
    }
    [Serializable]
    public enum enmActuaryFileRhicDetail
    {
         actuary_file_rhic_detail_id ,
         actuary_file_header_id ,
         person_id ,
         payee_account_id ,
         person_account_id ,
         account_relation_id ,
         account_relation_description ,
         account_relation_value ,
         payee_status_id ,
         payee_status_description ,
         payee_status_value ,
         marital_status_id ,
         marital_status_description ,
         marital_status_value ,
         rhic_benefit_begin_date ,
         plan_participation_start_date ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         pension_service_credit ,
         total_vested_service_credit ,
         monthly_rhic_amount ,
         applied_ghdv_rhic_amount ,
         coverage_code ,
         joint_annuitant_dob ,
         joint_annuitant_gender_id ,
         joint_annuitant_gender_description ,
         joint_annuitant_gender_value ,
         rhic_option_id ,
         rhic_option_description ,
         rhic_option_value ,
    }
}

