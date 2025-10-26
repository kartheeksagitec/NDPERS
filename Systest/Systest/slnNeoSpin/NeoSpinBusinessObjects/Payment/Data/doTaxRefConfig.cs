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
	/// Class NeoSpin.DataObjects.doTaxRefConfig:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doTaxRefConfig : doBase
    {
         public doTaxRefConfig() : base()
         {
         }
         public int tax_ref_id { get; set; }
         public int tax_identifier_id { get; set; }
         public string tax_identifier_description { get; set; }
         public string tax_identifier_value { get; set; }
         public string tax_ref { get; set; }
         public DateTime effective_date { get; set; }
         public int no_of_periods { get; set; }
         public decimal oneg_amt_single { get; set; }
         public decimal oneg_amt_married { get; set; }
         public decimal oneg_amt_hohh { get; set; }
         public decimal default_allowance_amt { get; set; }
        public decimal total_amt_single { get; set; }
        public decimal total_amt_married { get; set; }
        public int child_age { get; set; }
        public decimal child_age_by_amt { get; set; }
        public decimal other_depd_amt { get; set; }
        public string three_first_line { get; set; }
        public string three_second_line { get; set; }
        public string three_third_line { get; set; }
        public string two_tip { get; set; }
    }
    [Serializable]
    public enum enmTaxRefConfig
    {
         tax_ref_id ,
         tax_identifier_id ,
         tax_identifier_description ,
         tax_identifier_value ,
         tax_ref ,
         effective_date ,
         no_of_periods ,
         oneg_amt_single ,
         oneg_amt_married ,
         oneg_amt_hohh ,
         default_allowance_amt ,
         total_amt_single,
         total_amt_married,
         child_age,
         child_age_by_amt,
         other_depd_amt,
         three_first_line,
         three_second_line,
         three_third_line,
         two_tip,
}
}
