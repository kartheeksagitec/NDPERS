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
	/// Class NeoSpin.DataObjects.doBenefitEstimateRetirementType:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitEstimateRetirementType : doBase
    {
         
         public doBenefitEstimateRetirementType() : base()
         {
         }
         public int contact_ticket_retirement_type_id { get; set; }
         public int benefit_estimate_id { get; set; }
         public int retirement_type_id { get; set; }
         public string retirement_type_description { get; set; }
         public string retirement_type_value { get; set; }
    }
    [Serializable]
    public enum enmBenefitEstimateRetirementType
    {
         contact_ticket_retirement_type_id ,
         benefit_estimate_id ,
         retirement_type_id ,
         retirement_type_description ,
         retirement_type_value ,
    }
}

