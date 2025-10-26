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
	/// Class NeoSpin.DataObjects.doBenefitGraduatedBenefitOptionFactor:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitGraduatedBenefitOptionFactor : doBase
    {
         public doBenefitGraduatedBenefitOptionFactor() : base()
         {
         }
         public int benefit_graduated_benefit_option_factor_id { get; set; }
         public decimal member_age { get; set; }
         public decimal ben_age { get; set; }
         public string benefit_type { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public int graduated_benefit_option_id { get; set; }
         public string graduated_benefit_option_description { get; set; }
         public string graduated_benefit_option_value { get; set; }
         public int plan_id { get; set; }
         public decimal factor { get; set; }
         public DateTime effective_date { get; set; }
    }
    [Serializable]
    public enum enmBenefitGraduatedBenefitOptionFactor
    {
         benefit_graduated_benefit_option_factor_id ,
         member_age ,
         ben_age ,
         benefit_type ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         graduated_benefit_option_id ,
         graduated_benefit_option_description ,
         graduated_benefit_option_value ,
         plan_id ,
         factor ,
         effective_date ,
    }
}

