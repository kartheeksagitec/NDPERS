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
	/// Class NeoSpin.DataObjects.doPostRetirementIncreaseBatchRequestDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPostRetirementIncreaseBatchRequestDetail : doBase
    {
         
         public doPostRetirementIncreaseBatchRequestDetail() : base()
         {
         }
         public int post_retirement_increase_batch_request_detail_id { get; set; }
         public int post_retirement_increase_batch_request_id { get; set; }
         public int payee_account_id { get; set; }
         public decimal original_amount { get; set; }
         public decimal increase_amount { get; set; }
         public int payee_account_payment_item_type_id { get; set; }
         public string is_processed_flag { get; set; }
         public decimal minimum_gaurantee_amount { get; set; }
         public int payment_item_type_id { get; set; }
    }
    [Serializable]
    public enum enmPostRetirementIncreaseBatchRequestDetail
    {
         post_retirement_increase_batch_request_detail_id ,
         post_retirement_increase_batch_request_id ,
         payee_account_id ,
         original_amount ,
         increase_amount ,
         payee_account_payment_item_type_id ,
         is_processed_flag ,
         minimum_gaurantee_amount ,
         payment_item_type_id ,
    }
}

