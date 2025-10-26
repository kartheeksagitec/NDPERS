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
	/// Class NeoSpin.DataObjects.doPostRetirementIncreaseBatchBenefitAccountType:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPostRetirementIncreaseBatchBenefitAccountType : doBase
    {
         
         public doPostRetirementIncreaseBatchBenefitAccountType() : base()
         {
         }
         public int post_retirement_increase_batch_benefit_account_type_id { get; set; }
         public int post_retirement_increase_batch_request_id { get; set; }
         public int benefit_account_type_id { get; set; }
         public string benefit_account_type_description { get; set; }
         public string benefit_account_type_value { get; set; }
    }
    [Serializable]
    public enum enmPostRetirementIncreaseBatchBenefitAccountType
    {
         post_retirement_increase_batch_benefit_account_type_id ,
         post_retirement_increase_batch_request_id ,
         benefit_account_type_id ,
         benefit_account_type_description ,
         benefit_account_type_value ,
    }
}

