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
	/// Class NeoSpin.DataObjects.doPersonAccountLtcOption:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountLtcOption : doBase
    {
         
         public doPersonAccountLtcOption() : base()
         {
         }
         public int person_account_ltc_option_id { get; set; }
         public int person_account_id { get; set; }
         public int plan_option_status_id { get; set; }
         public string plan_option_status_description { get; set; }
         public string plan_option_status_value { get; set; }
         public int ltc_relationship_id { get; set; }
         public string ltc_relationship_description { get; set; }
         public string ltc_relationship_value { get; set; }
         public int ltc_insurance_type_id { get; set; }
         public string ltc_insurance_type_description { get; set; }
         public string ltc_insurance_type_value { get; set; }
         public int level_of_coverage_id { get; set; }
         public string level_of_coverage_description { get; set; }
         public string level_of_coverage_value { get; set; }
         public DateTime effective_start_date { get; set; }
         public DateTime effective_end_date { get; set; }
         public int person_id { get; set; }
         public string people_soft_file_sent_flag { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountLtcOption
    {
         person_account_ltc_option_id ,
         person_account_id ,
         plan_option_status_id ,
         plan_option_status_description ,
         plan_option_status_value ,
         ltc_relationship_id ,
         ltc_relationship_description ,
         ltc_relationship_value ,
         ltc_insurance_type_id ,
         ltc_insurance_type_description ,
         ltc_insurance_type_value ,
         level_of_coverage_id ,
         level_of_coverage_description ,
         level_of_coverage_value ,
         effective_start_date ,
         effective_end_date ,
         person_id ,
         people_soft_file_sent_flag ,
    }
}

