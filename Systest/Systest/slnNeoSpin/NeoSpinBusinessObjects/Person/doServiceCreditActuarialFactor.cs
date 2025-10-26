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
	/// Class NeoSpin.DataObjects.doServiceCreditActuarialFactor:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doServiceCreditActuarialFactor : doBase
    {
         
         public doServiceCreditActuarialFactor() : base()
         {
         }
         public int service_credit_actuarial_factor_id { get; set; }
         public int actuarial_table_reference_id { get; set; }
         public string actuarial_table_reference_description { get; set; }
         public string actuarial_table_reference_value { get; set; }
         public int member_age { get; set; }
         public int retirement_age { get; set; }
         public decimal retirement_actuarial_factor { get; set; }
         public decimal rhic_actuarial_factor { get; set; }
         public DateTime effective_date { get; set; }
         public decimal future_ee_actuarial_factor { get; set; }
         public int months_of_service_from { get; set; }
         public int months_of_service_to { get; set; }
         public int plan_id { get; set; }
         public string employer_type { get; set; }
    }
    [Serializable]
    public enum enmServiceCreditActuarialFactor
    {
         service_credit_actuarial_factor_id ,
         actuarial_table_reference_id ,
         actuarial_table_reference_description ,
         actuarial_table_reference_value ,
         member_age ,
         retirement_age ,
         retirement_actuarial_factor ,
         rhic_actuarial_factor ,
         effective_date ,
         future_ee_actuarial_factor ,
         months_of_service_from ,
         months_of_service_to ,
         plan_id ,
         employer_type ,
    }
}

