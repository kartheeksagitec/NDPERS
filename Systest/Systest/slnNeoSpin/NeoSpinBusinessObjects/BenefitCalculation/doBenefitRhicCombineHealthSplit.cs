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
	/// Class NeoSpin.DataObjects.doBenefitRhicCombineHealthSplit:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitRhicCombineHealthSplit : doBase
    {
         
         public doBenefitRhicCombineHealthSplit() : base()
         {
         }
         public int benefit_rhic_combine_health_split_id { get; set; }
         public int person_account_id { get; set; }
         public int benefit_rhic_combine_id { get; set; }
         public int payee_account_retro_payment_id { get; set; }
         public decimal js_rhic_amount { get; set; }
         public decimal other_rhic_amount { get; set; }
         public decimal reimbursement_amount { get; set; }
    }
    [Serializable]
    public enum enmBenefitRhicCombineHealthSplit
    {
         benefit_rhic_combine_health_split_id ,
         person_account_id ,
         benefit_rhic_combine_id ,
         payee_account_retro_payment_id ,
         js_rhic_amount ,
         other_rhic_amount ,
         reimbursement_amount ,
    }
}

