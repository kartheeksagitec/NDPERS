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
	/// Class NeoSpin.DataObjects.doPlanMemberTypeCrossref:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPlanMemberTypeCrossref : doBase
    {
         public doPlanMemberTypeCrossref() : base()
         {
         }
         public int plan_member_type_crossref_id { get; set; }
         public int plan_id { get; set; }
         public int job_class_id { get; set; }
         public string job_class_description { get; set; }
         public string job_class_value { get; set; }
         public int employment_type_id { get; set; }
         public string employment_type_description { get; set; }
         public string employment_type_value { get; set; }
         public int member_type_id { get; set; }
         public string member_type_description { get; set; }
         public string member_type_value { get; set; }
    }
    [Serializable]
    public enum enmPlanMemberTypeCrossref
    {
         plan_member_type_crossref_id ,
         plan_id ,
         job_class_id ,
         job_class_description ,
         job_class_value ,
         employment_type_id ,
         employment_type_description ,
         employment_type_value ,
         member_type_id ,
         member_type_description ,
         member_type_value ,
    }
}

