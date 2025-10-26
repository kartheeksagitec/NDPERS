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
	/// Class NeoSpin.DataObjects.doPersonAccountRetirementContribution:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountRetirementContribution : doBase
    {
         
         public doPersonAccountRetirementContribution() : base()
         {
         }
         public int retirement_contribution_id { get; set; }
         public int person_account_id { get; set; }
         public int subsystem_id { get; set; }
         public string subsystem_description { get; set; }
         public string subsystem_value { get; set; }
         public int subsystem_ref_id { get; set; }
         public DateTime transaction_date { get; set; }
         public DateTime effective_date { get; set; }
         public int pay_period_month { get; set; }
         public int pay_period_year { get; set; }
         public int person_employment_dtl_id { get; set; }
         public int transaction_type_id { get; set; }
         public string transaction_type_description { get; set; }
         public string transaction_type_value { get; set; }
         public decimal salary_amount { get; set; }
         public decimal post_tax_er_amount { get; set; }
         public decimal post_tax_ee_amount { get; set; }
         public decimal pre_tax_er_amount { get; set; }
         public decimal pre_tax_ee_amount { get; set; }
         public decimal ee_rhic_amount { get; set; }
         public decimal er_rhic_amount { get; set; }
         public decimal ee_er_pickup_amount { get; set; }
         public decimal er_vested_amount { get; set; }
         public decimal interest_amount { get; set; }
         public decimal vested_service_credit { get; set; }
         public decimal pension_service_credit { get; set; }
         public decimal employer_interest { get; set; }
         public int vested_er_percentage_ref_id { get; set; }
         public decimal employer_rhic_interest { get; set; }
         public int transfer_flag_id { get; set; }
         public string transfer_flag_description { get; set; }
         public string transfer_flag_value { get; set; }
		 //PIR 25920 New Plan DC 2025
        public decimal ee_pretax_addl_amount { get; set; }
        public decimal ee_post_tax_addl_amount { get; set; }
        public decimal er_pretax_match_amount { get; set; }
        public decimal adec_amount { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountRetirementContribution
    {
         retirement_contribution_id ,
         person_account_id ,
         subsystem_id ,
         subsystem_description ,
         subsystem_value ,
         subsystem_ref_id ,
         transaction_date ,
         effective_date ,
         pay_period_month ,
         pay_period_year ,
         person_employment_dtl_id ,
         transaction_type_id ,
         transaction_type_description ,
         transaction_type_value ,
         salary_amount ,
         post_tax_er_amount ,
         post_tax_ee_amount ,
         pre_tax_er_amount ,
         pre_tax_ee_amount ,
         ee_rhic_amount ,
         er_rhic_amount ,
         ee_er_pickup_amount ,
         er_vested_amount ,
         interest_amount ,
         vested_service_credit ,
         pension_service_credit ,
         employer_interest ,
         vested_er_percentage_ref_id ,
         employer_rhic_interest ,
         transfer_flag_id ,
         transfer_flag_description ,
         transfer_flag_value ,
         ee_pretax_addl_amount,
         ee_post_tax_addl_amount,
         er_pretax_match_amount,
         adec_amount,
    }
}

