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
	/// Class NeoSpin.DataObjects.doOrgPlanGroupHealthMedicarePartDRateStructureRef:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doOrgPlanGroupHealthMedicarePartDRateStructureRef : doBase
    {
         
         public doOrgPlanGroupHealthMedicarePartDRateStructureRef() : base()
         {
         }
         public int org_plan_group_health_medicare_part_d_rate_structure_ref_id { get; set; }
         public DateTime effective_date { get; set; }
         public DateTime enrollment_date_from { get; set; }
         public DateTime enrollment_date_to { get; set; }
         public int health_insurance_type_id { get; set; }
         public string health_insurance_type_description { get; set; }
         public string health_insurance_type_value { get; set; }
         public int rate_structure_id { get; set; }
         public string rate_structure_description { get; set; }
         public string rate_structure_value { get; set; }
         public string low_income { get; set; }
         public int alternate_structure_code_id { get; set; }
         public string alternate_structure_code_description { get; set; }
         public string alternate_structure_code_value { get; set; }
    }
    [Serializable]
    public enum enmOrgPlanGroupHealthMedicarePartDRateStructureRef
    {
         org_plan_group_health_medicare_part_d_rate_structure_ref_id ,
         effective_date ,
         enrollment_date_from ,
         enrollment_date_to ,
         health_insurance_type_id ,
         health_insurance_type_description ,
         health_insurance_type_value ,
         rate_structure_id ,
         rate_structure_description ,
         rate_structure_value ,
         low_income ,
         alternate_structure_code_id ,
         alternate_structure_code_description ,
         alternate_structure_code_value ,
    }
}

