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
	/// Class NeoSpin.DataObjects.doMasPersonPlanBeneficiaryDependent:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doMasPersonPlanBeneficiaryDependent : doBase
    {
         
         public doMasPersonPlanBeneficiaryDependent() : base()
         {
         }
         public int mas_person_plan_beneficiary_id { get; set; }
         public int mas_person_id { get; set; }
         public int plan_id { get; set; }
         public string plan_name { get; set; }
         public string beneficiary_type_description { get; set; }
         public string beneficiary_full_name { get; set; }
         public string relationship_description { get; set; }
         public DateTime date_of_birth { get; set; }
         public decimal percentage { get; set; }
         public string is_beneficiary_flag { get; set; }
    }
    [Serializable]
    public enum enmMasPersonPlanBeneficiaryDependent
    {
         mas_person_plan_beneficiary_id ,
         mas_person_id ,
         plan_id ,
         plan_name ,
         beneficiary_type_description ,
         beneficiary_full_name ,
         relationship_description ,
         date_of_birth ,
         percentage ,
         is_beneficiary_flag ,
    }
}

