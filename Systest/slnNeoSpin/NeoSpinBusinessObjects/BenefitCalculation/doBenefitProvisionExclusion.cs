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
	/// Class NeoSpin.DataObjects.doBenefitProvisionExclusion:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitProvisionExclusion : doBase
    {
         
         public doBenefitProvisionExclusion() : base()
         {
         }
         public int benefit_provision_exclusion_id { get; set; }
         public int exclusion_calc_payment_type_id { get; set; }
         public string exclusion_calc_payment_type_description { get; set; }
         public string exclusion_calc_payment_type_value { get; set; }
         public DateTime effective_date { get; set; }
         public int minimum_age { get; set; }
         public int maximum_age { get; set; }
         public int number_of_payments { get; set; }
    }
    [Serializable]
    public enum enmBenefitProvisionExclusion
    {
         benefit_provision_exclusion_id ,
         exclusion_calc_payment_type_id ,
         exclusion_calc_payment_type_description ,
         exclusion_calc_payment_type_value ,
         effective_date ,
         minimum_age ,
         maximum_age ,
         number_of_payments ,
    }
}

