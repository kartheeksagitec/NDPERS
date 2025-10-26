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
	/// Class NeoSpin.DataObjects.doBenefitCalculationFasMonths:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitCalculationFasMonths : doBase
    {
         public doBenefitCalculationFasMonths() : base()
         {
         }
         public int benefit_calc_months_id { get; set; }
         public int benefit_calculation_id { get; set; }
         public int person_account_id { get; set; }
         public int year { get; set; }
         public int month { get; set; }
         public decimal salary_amount { get; set; }
         public string projected_flag { get; set; }
         public DateTime effective_date { get; set; }

        public int fas_logic_id { get; set; }
        public string fas_logic_description { get; set; }
        public string fas_logic_value { get; set; }
    }
    [Serializable]
    public enum enmBenefitCalculationFasMonths
    {
         benefit_calc_months_id ,
         benefit_calculation_id ,
         person_account_id ,
         year ,
         month ,
         salary_amount ,
         projected_flag ,
         effective_date ,
        fas_logic_id,
        fas_logic_description,
        fas_logic_value,
    }
}

