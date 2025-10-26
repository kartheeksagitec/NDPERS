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
	/// Class NeoSpin.DataObjects.doPostRetirementIncreaseBatchAccountRelationship:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPostRetirementIncreaseBatchAccountRelationship : doBase
    {
         
         public doPostRetirementIncreaseBatchAccountRelationship() : base()
         {
         }
         public int post_retirement_increase_batch_account_relationship_id { get; set; }
         public int post_retirement_increase_batch_request_id { get; set; }
         public int account_relationship_id { get; set; }
         public string account_relationship_description { get; set; }
         public string account_relationship_value { get; set; }
    }
    [Serializable]
    public enum enmPostRetirementIncreaseBatchAccountRelationship
    {
         post_retirement_increase_batch_account_relationship_id ,
         post_retirement_increase_batch_request_id ,
         account_relationship_id ,
         account_relationship_description ,
         account_relationship_value ,
    }
}

