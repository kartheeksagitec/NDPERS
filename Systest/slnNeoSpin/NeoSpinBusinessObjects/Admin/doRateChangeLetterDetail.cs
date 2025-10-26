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
	/// Class NeoSpin.DataObjects.doRateChangeLetterDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doRateChangeLetterDetail : doBase
    {
         public doRateChangeLetterDetail() : base()
         {
         }
         public int rate_change_letter_detail_id { get; set; }
         public int batch_request_id { get; set; }
         public int person_id { get; set; }
         public decimal rhic_amount { get; set; }
         public decimal curr_health_prem { get; set; }
         public decimal new_health_prem { get; set; }
         public decimal curr_medd_prem { get; set; }
         public decimal new_medd_prem { get; set; }
         public decimal curr_den_prem { get; set; }
         public decimal new_den_prem { get; set; }
         public decimal curr_vis_prem { get; set; }
         public decimal new_vis_prem { get; set; }
         public decimal life_basic_amt { get; set; }
         public decimal curr_basic_prem { get; set; }
         public decimal new_basic_prem { get; set; }
         public decimal life_supp_amt { get; set; }
         public decimal curr_supp_prem { get; set; }
         public decimal new_supp_prem { get; set; }
         public decimal life_dep_supp_amt { get; set; }
         public decimal curr_dep_supp_prem { get; set; }
         public decimal new_dep_supp_prem { get; set; }
         public decimal life_sp_supp_amt { get; set; }
         public decimal curr_sp_supp_prem { get; set; }
         public decimal new_sp_supp_prem { get; set; }
         public decimal curr_rhic_mult { get; set; }
         public decimal new_rhic_mult { get; set; }
         public string letter_generated { get; set; }
    }
    [Serializable]
    public enum enmRateChangeLetterDetail
    {
         rate_change_letter_detail_id ,
         batch_request_id ,
         person_id ,
         rhic_amount ,
         curr_health_prem ,
         new_health_prem ,
         curr_medd_prem ,
         new_medd_prem ,
         curr_den_prem ,
         new_den_prem ,
         curr_vis_prem ,
         new_vis_prem ,
         life_basic_amt ,
         curr_basic_prem ,
         new_basic_prem ,
         life_supp_amt ,
         curr_supp_prem ,
         new_supp_prem ,
         life_dep_supp_amt ,
         curr_dep_supp_prem ,
         new_dep_supp_prem ,
         life_sp_supp_amt ,
         curr_sp_supp_prem ,
         new_sp_supp_prem ,
         curr_rhic_mult ,
         new_rhic_mult ,
         letter_generated ,
    }
}

