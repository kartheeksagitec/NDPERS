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
	/// Class NeoSpin.DataObjects.doPaymentLifeTimeReductionRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentLifeTimeReductionRef : doBase
    {
         
         public doPaymentLifeTimeReductionRef() : base()
         {
         }
         public int life_time_reduction_ref_id { get; set; }
         public int benefit_option_id { get; set; }
         public string benefit_option_description { get; set; }
         public string benefit_option_value { get; set; }
         public DateTime effective_date { get; set; }
         public int member_gender_id { get; set; }
         public string member_gender_description { get; set; }
         public string member_gender_value { get; set; }
         public decimal member_age { get; set; }
         public decimal joint_and_survivor_age { get; set; }
         public int number_of_payments { get; set; }
    }
    [Serializable]
    public enum enmPaymentLifeTimeReductionRef
    {
         life_time_reduction_ref_id ,
         benefit_option_id ,
         benefit_option_description ,
         benefit_option_value ,
         effective_date ,
         member_gender_id ,
         member_gender_description ,
         member_gender_value ,
         member_age ,
         joint_and_survivor_age ,
         number_of_payments ,
    }
}

