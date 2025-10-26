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
	/// Class NeoSpin.DataObjects.doBenefitDroCalculation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitDroCalculation : doBase
    {
         public doBenefitDroCalculation() : base()
         {
         }
         public int dro_calculation_id { get; set; }
         public int dro_application_id { get; set; }
         public int payment_type_id { get; set; }
         public string payment_type_description { get; set; }
         public string payment_type_value { get; set; }
         public int payment_status_id { get; set; }
         public string payment_status_description { get; set; }
         public string payment_status_value { get; set; }
         public DateTime benefit_begin_date { get; set; }
         public decimal monthly_benefit_amount { get; set; }
         public decimal ee_pre_tax_amount { get; set; }
         public decimal ee_post_tax_amount { get; set; }
         public decimal ee_er_pickup_amount { get; set; }
         public decimal er_vested_amount { get; set; }
         public decimal interest_amount { get; set; }
         public decimal capital_gain { get; set; }
         public decimal additional_interest_amount { get; set; }
         public string verified_by_user { get; set; }
         public decimal rmd_amount { get; set; }
         public decimal minimum_guarantee { get; set; }
         public decimal starting_non_taxable { get; set; }
    }
    [Serializable]
    public enum enmBenefitDroCalculation
    {
         dro_calculation_id ,
         dro_application_id ,
         payment_type_id ,
         payment_type_description ,
         payment_type_value ,
         payment_status_id ,
         payment_status_description ,
         payment_status_value ,
         benefit_begin_date ,
         monthly_benefit_amount ,
         ee_pre_tax_amount ,
         ee_post_tax_amount ,
         ee_er_pickup_amount ,
         er_vested_amount ,
         interest_amount ,
         capital_gain ,
         additional_interest_amount ,
         verified_by_user ,
         rmd_amount ,
         minimum_guarantee ,
         starting_non_taxable ,
    }
}

