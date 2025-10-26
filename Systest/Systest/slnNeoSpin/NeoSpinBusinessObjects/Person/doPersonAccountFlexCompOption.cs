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
	/// Class NeoSpin.DataObjects.doPersonAccountFlexCompOption:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountFlexCompOption : doBase
    {
         
         public doPersonAccountFlexCompOption() : base()
         {
         }
         public int account_flex_comp_option_id { get; set; }
         public int person_account_id { get; set; }
         public int level_of_coverage_id { get; set; }
         public string level_of_coverage_description { get; set; }
         public string level_of_coverage_value { get; set; }
         public decimal annual_pledge_amount { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public string people_soft_file_sent_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountFlexCompOption
    {
         account_flex_comp_option_id ,
         person_account_id ,
         level_of_coverage_id ,
         level_of_coverage_description ,
         level_of_coverage_value ,
         annual_pledge_amount ,
         effective_start_date ,
         effective_end_date ,
         people_soft_file_sent_flag ,
    }
}

