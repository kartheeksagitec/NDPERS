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
	/// Class NeoSpin.DataObjects.doLowIncomeCreditRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doLowIncomeCreditRef : doBase
    {
         
         public doLowIncomeCreditRef() : base()
         {
         }
         public int low_income_credit_ref_id { get; set; }
         public DateTime effective_date { get; set; }
         public decimal low_income_credit { get; set; }
         public decimal amount { get; set; }
    }
    [Serializable]
    public enum enmLowIncomeCreditRef
    {
         low_income_credit_ref_id ,
         effective_date ,
         low_income_credit ,
         amount ,
    }
}

