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
	/// Class NeoSpin.DataObjects.doOrgPlanMemberType:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doOrgPlanMemberType : doBase
    {
         public doOrgPlanMemberType() : base()
         {
         }
         public int org_plan_member_type_id { get; set; }
         public int org_plan_id { get; set; }
         public int member_type_id { get; set; }
         public string member_type_description { get; set; }
         public string member_type_value { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public string benefit_plan { get; set; }
         public string rhic_benefit_plan { get; set; }
    }
    [Serializable]
    public enum enmOrgPlanMemberType
    {
         org_plan_member_type_id ,
         org_plan_id ,
         member_type_id ,
         member_type_description ,
         member_type_value ,
         start_date ,
         end_date ,
         benefit_plan ,
         rhic_benefit_plan ,
    }
}
