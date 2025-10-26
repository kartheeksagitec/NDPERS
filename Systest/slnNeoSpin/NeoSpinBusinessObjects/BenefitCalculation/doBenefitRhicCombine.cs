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
	/// Class NeoSpin.DataObjects.doBenefitRhicCombine:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitRhicCombine : doBase
    {
        
         public doBenefitRhicCombine() : base()
         {
         }
         public int benefit_rhic_combine_id { get; set; }
         public int person_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public decimal initial_rhic_amount { get; set; }
         public decimal combined_rhic_amount { get; set; }
         public decimal estimated_combined_rhic_amount { get; set; }
         public string apply_to_value { get; set; }
         public int action_status_id { get; set; }
         public string action_status_description { get; set; }
         public string action_status_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public string approved_by { get; set; }
         public DateTime request_date { get; set; }
         public decimal total_js_rhic_amount { get; set; }
         public decimal total_other_rhic_amount { get; set; }
         public decimal total_reimbursement_amount { get; set; }
         public string suppress_warnings_flag { get; set; }
         public string comments { get; set; }
    }
    [Serializable]
    public enum enmBenefitRhicCombine
    {
         benefit_rhic_combine_id ,
         person_id ,
         start_date ,
         end_date ,
         initial_rhic_amount ,
         combined_rhic_amount ,
         estimated_combined_rhic_amount ,
         apply_to_value ,
         action_status_id ,
         action_status_description ,
         action_status_value ,
         status_id ,
         status_description ,
         status_value ,
         approved_by ,
         request_date ,
         total_js_rhic_amount ,
         total_other_rhic_amount ,
         total_reimbursement_amount ,
         suppress_warnings_flag ,
         comments ,
    }
}

