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
	/// Class NeoSpin.DataObjects.doOrgPlanGroupHealthMedicarePartDCoverageRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doOrgPlanGroupHealthMedicarePartDCoverageRef : doBase
    {
         
         public doOrgPlanGroupHealthMedicarePartDCoverageRef() : base()
         {
         }
         public int org_plan_group_health_medicare_part_d_coverage_ref_id { get; set; }
         public string coverage_code { get; set; }
         public int org_plan_group_health_medicare_part_d_rate_ref_id { get; set; }
         public int employment_type_id { get; set; }
         public string employment_type_description { get; set; }
         public string employment_type_value { get; set; }
         public string medicare_in { get; set; }
         public string cobra_in { get; set; }
         public int group_coverage_id { get; set; }
         public string group_coverage_description { get; set; }
         public string group_coverage_value { get; set; }
         public string short_description { get; set; }
         public string description { get; set; }
         public string client_description { get; set; }
         public string show_online_flag { get; set; }
    }
    [Serializable]
    public enum enmOrgPlanGroupHealthMedicarePartDCoverageRef
    {
         org_plan_group_health_medicare_part_d_coverage_ref_id ,
         coverage_code ,
         org_plan_group_health_medicare_part_d_rate_ref_id ,
         employment_type_id ,
         employment_type_description ,
         employment_type_value ,
         medicare_in ,
         cobra_in ,
         group_coverage_id ,
         group_coverage_description ,
         group_coverage_value ,
         short_description ,
         description ,
         client_description ,
         show_online_flag ,
    }
}

