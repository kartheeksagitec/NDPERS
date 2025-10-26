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
	/// Class NeoSpin.DataObjects.doBenefitRefundCalculation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitRefundCalculation : doBase
    {
         
         public doBenefitRefundCalculation() : base()
         {
         }
         public int benefit_calculation_id { get; set; }
         public DateTime calculation_date { get; set; }
         public decimal pre_tax_ee_contribution_amount { get; set; }
         public decimal post_tax_ee_contribution_amount { get; set; }
         public decimal ee_er_pickup_amount { get; set; }
         public decimal ee_interest_amount { get; set; }
         public decimal vested_er_contribution_amount { get; set; }
         public decimal rhic_ee_amount { get; set; }
         public decimal er_pre_tax_amount { get; set; }
         public decimal er_interest_amount { get; set; }
         public decimal additional_ee_amount { get; set; }
         public decimal additional_er_amount_interest { get; set; }
         public decimal additional_er_amount { get; set; }
         public decimal overridden_er_pre_tax_amount { get; set; }
         public decimal overridden_er_interest_amount { get; set; }
         public decimal capital_gain { get; set; }
        public decimal calculated_actuarial_value { get; set; }
        public decimal db_account_balance { get; set; }
        public decimal accrued_benefit_amount { get; set; }
    }
    [Serializable]
    public enum enmBenefitRefundCalculation
    {
         benefit_calculation_id ,
         calculation_date ,
         pre_tax_ee_contribution_amount ,
         post_tax_ee_contribution_amount ,
         ee_er_pickup_amount ,
         ee_interest_amount ,
         vested_er_contribution_amount ,
         rhic_ee_amount ,
         er_pre_tax_amount ,
         er_interest_amount ,
         additional_ee_amount ,
         additional_er_amount_interest ,
         additional_er_amount ,
         overridden_er_pre_tax_amount ,
         overridden_er_interest_amount ,
         capital_gain ,
        calculated_actuarial_value,
        db_account_balance,
        accrued_benefit_amount,
    }
}

