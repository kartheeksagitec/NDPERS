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
	/// Class NeoSpin.DataObjects.doPeoplesoftPlanCrossRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPeoplesoftPlanCrossRef : doBase
    {
         
         public doPeoplesoftPlanCrossRef() : base()
         {
         }
         public int sgt_peoplesoft_plan_cross_ref_id { get; set; }
         public int plan_id { get; set; }
         public string description { get; set; }
         public string level_of_coverage_value { get; set; }
         public string insurance_type_value { get; set; }
         public string emp_type_value { get; set; }
         public string plan_option_value { get; set; }
         public string wellness_flag { get; set; }
         public string ltc_relationship_value { get; set; }
         public string member_type_value { get; set; }
         public string check_flex_comp_plan { get; set; }
         public string provider_org_code { get; set; }
         public string plan_dtl { get; set; }
         public string full_description { get; set; }
         public string plan_type { get; set; }
         public string f13 { get; set; }
         public string benefit_plan { get; set; }
         public string coverage_begin_date_required { get; set; }
         public string deduction_begin_date_required { get; set; }
         public string coverage_election_required { get; set; }
         public string election_date_required { get; set; }
         public string benefit_plan_required { get; set; }
         public string covergae_code_required { get; set; }
         public string flat_amount_required { get; set; }
         public string print_coverage_amount_in_flat_amount { get; set; }
         public string calculation_routine_required { get; set; }
         public decimal coverage_amount { get; set; }
         public string rhic_plan_type { get; set; }
         public string rhic_benefit_plan { get; set; }
         public int addl_ee_contribution_percent { get; set; }
    }
    [Serializable]
    public enum enmPeoplesoftPlanCrossRef
    {
         sgt_peoplesoft_plan_cross_ref_id ,
         plan_id ,
         description ,
         level_of_coverage_value ,
         insurance_type_value ,
         emp_type_value ,
         plan_option_value ,
         wellness_flag ,
         ltc_relationship_value ,
         member_type_value ,
         check_flex_comp_plan ,
         provider_org_code ,
         plan_dtl ,
         full_description ,
         plan_type ,
         f13 ,
         benefit_plan ,
         coverage_begin_date_required ,
         deduction_begin_date_required ,
         coverage_election_required ,
         election_date_required ,
         benefit_plan_required ,
         covergae_code_required ,
         flat_amount_required ,
         print_coverage_amount_in_flat_amount ,
         calculation_routine_required ,
         coverage_amount ,
         rhic_plan_type ,
         rhic_benefit_plan ,
         addl_ee_contribution_percent,
    }
}

