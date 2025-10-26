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
	/// Class NeoSpin.DataObjects.doMasPersonCalculation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasPersonCalculation : doBase
    {
         
         public doMasPersonCalculation() : base()
         {
         }
         public int mas_person_calculation_id { get; set; }
         public int mas_person_id { get; set; }
         public int plan_id { get; set; }
         public string plan_name { get; set; }
         public string benefit_account_type_value { get; set; }
         public string age_description { get; set; }
         public decimal monthly_benefit { get; set; }
         public decimal rhic_benefit { get; set; }
         public string is_nrd_date { get; set; }
         public decimal member_account_balance { get; set; }
         public decimal qdro_deductions { get; set; }
         public DateTime normal_retirement_date { get; set; }
         public decimal final_average_salary { get; set; }
         public decimal pension_service_credit { get; set; }
         public decimal vested_service_credit { get; set; }
         public decimal disability_benefit_percentage { get; set; }
         public string is_default_retirement { get; set; }
         public DateTime termination_date { get; set; }
         public DateTime retirement_date { get; set; }
         public string calculation_type { get; set; }
         public string is_rule_or_age_indicator { get; set; }
         public decimal mab_as_on_effective_date { get; set; }
    }
    [Serializable]
    public enum enmMasPersonCalculation
    {
         mas_person_calculation_id ,
         mas_person_id ,
         plan_id ,
         plan_name ,
         benefit_account_type_value ,
         age_description ,
         monthly_benefit ,
         rhic_benefit ,
         is_nrd_date ,
         member_account_balance ,
         qdro_deductions ,
         normal_retirement_date ,
         final_average_salary ,
         pension_service_credit ,
         vested_service_credit ,
         disability_benefit_percentage ,
         is_default_retirement ,
         termination_date ,
         retirement_date ,
         calculation_type ,
         is_rule_or_age_indicator ,
         mab_as_on_effective_date ,
    }
}

