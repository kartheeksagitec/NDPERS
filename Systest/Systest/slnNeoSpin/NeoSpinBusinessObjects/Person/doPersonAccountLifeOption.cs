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
	/// Class NeoSpin.DataObjects.doPersonAccountLifeOption:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountLifeOption : doBase
    {
         
         public doPersonAccountLifeOption() : base()
         {
         }
         public int account_life_option_id { get; set; }
         public int person_account_id { get; set; }
         public int level_of_coverage_id { get; set; }
         public string level_of_coverage_description { get; set; }
         public string level_of_coverage_value { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public int plan_option_status_id { get; set; }
         public string plan_option_status_description { get; set; }
         public string plan_option_status_value { get; set; }
         public decimal coverage_amount { get; set; }
         public string people_soft_file_sent_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountLifeOption
    {
         account_life_option_id ,
         person_account_id ,
         level_of_coverage_id ,
         level_of_coverage_description ,
         level_of_coverage_value ,
         effective_start_date ,
         effective_end_date ,
         plan_option_status_id ,
         plan_option_status_description ,
         plan_option_status_value ,
         coverage_amount ,
         people_soft_file_sent_flag ,
    }
}

