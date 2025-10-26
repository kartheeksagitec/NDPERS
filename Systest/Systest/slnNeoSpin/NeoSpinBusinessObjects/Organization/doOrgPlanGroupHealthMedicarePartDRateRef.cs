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
	/// Class NeoSpin.DataObjects.doOrgPlanGroupHealthMedicarePartDRateRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doOrgPlanGroupHealthMedicarePartDRateRef : doBase
    {
         
         public doOrgPlanGroupHealthMedicarePartDRateRef() : base()
         {
         }
         public int org_plan_group_health_medicare_part_d_rate_ref_id { get; set; }
         public string rate_structure_code { get; set; }
         public int health_insurance_type_id { get; set; }
         public string health_insurance_type_description { get; set; }
         public string health_insurance_type_value { get; set; }
         public int rate_structure_id { get; set; }
         public string rate_structure_description { get; set; }
         public string rate_structure_value { get; set; }
         public int plan_option_id { get; set; }
         public string plan_option_description { get; set; }
         public string plan_option_value { get; set; }
         public string wellness { get; set; }
         public string low_income { get; set; }
         public int alternate_structure_code_id { get; set; }
         public string alternate_structure_code_description { get; set; }
         public string alternate_structure_code_value { get; set; }
    }
    [Serializable]
    public enum enmOrgPlanGroupHealthMedicarePartDRateRef
    {
         org_plan_group_health_medicare_part_d_rate_ref_id ,
         rate_structure_code ,
         health_insurance_type_id ,
         health_insurance_type_description ,
         health_insurance_type_value ,
         rate_structure_id ,
         rate_structure_description ,
         rate_structure_value ,
         plan_option_id ,
         plan_option_description ,
         plan_option_value ,
         wellness ,
         low_income ,
         alternate_structure_code_id ,
         alternate_structure_code_description ,
         alternate_structure_code_value ,
    }
}

