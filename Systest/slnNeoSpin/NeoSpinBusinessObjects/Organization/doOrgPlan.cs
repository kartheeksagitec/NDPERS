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
	/// Class NeoSpin.DataObjects.doOrgPlan:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doOrgPlan : doBase
    {
         
         public doOrgPlan() : base()
         {
         }
         public int org_plan_id { get; set; }
         public int org_id { get; set; }
         public int plan_id { get; set; }
         public DateTime participation_start_date { get; set; }
         public DateTime participation_end_date { get; set; }
         public int report_frequency_id { get; set; }
         public string report_frequency_description { get; set; }
         public string report_frequency_value { get; set; }
         public string comments { get; set; }
         public int plan_option_id { get; set; }
         public string plan_option_description { get; set; }
         public string plan_option_value { get; set; }
         public string wellness_flag { get; set; }
         public string pre_tax_purchase { get; set; }
         public string restriction { get; set; }
         public DateTime health_participation_start_date { get; set; }
         public int alternate_structure_code_id { get; set; }
         public string alternate_structure_code_description { get; set; }
         public string alternate_structure_code_value { get; set; }
         public string hsa_pre_tax_agreement { get; set; }
        public int day_of_month { get; set; }
    }
    [Serializable]
    public enum enmOrgPlan
    {
         org_plan_id ,
         org_id ,
         plan_id ,
         participation_start_date ,
         participation_end_date ,
         report_frequency_id ,
         report_frequency_description ,
         report_frequency_value ,
         comments ,
         plan_option_id ,
         plan_option_description ,
         plan_option_value ,
         wellness_flag ,
         pre_tax_purchase ,
         restriction ,
         health_participation_start_date ,
         alternate_structure_code_id ,
         alternate_structure_code_description ,
         alternate_structure_code_value ,
         hsa_pre_tax_agreement ,
        day_of_month,
    }
}

