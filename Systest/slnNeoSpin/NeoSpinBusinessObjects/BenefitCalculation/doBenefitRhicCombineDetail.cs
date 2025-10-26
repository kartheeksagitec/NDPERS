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
	/// Class NeoSpin.DataObjects.doBenefitRhicCombineDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitRhicCombineDetail : doBase
    {
         
         public doBenefitRhicCombineDetail() : base()
         {
         }
         public int benefit_rhic_combine_detail_id { get; set; }
         public int benefit_rhic_combine_id { get; set; }
         public int donar_payee_account_id { get; set; }
         public decimal rhic_amount { get; set; }
         public string combine_flag { get; set; }
    }
    [Serializable]
    public enum enmBenefitRhicCombineDetail
    {
         benefit_rhic_combine_detail_id ,
         benefit_rhic_combine_id ,
         donar_payee_account_id ,
         rhic_amount ,
         combine_flag ,
    }
}

