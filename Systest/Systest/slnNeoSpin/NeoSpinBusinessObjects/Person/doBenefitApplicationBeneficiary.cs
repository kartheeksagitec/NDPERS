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
	/// Class NeoSpin.DataObjects.doBenefitApplicationBeneficiary:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBenefitApplicationBeneficiary : doBase
    {
         
         public doBenefitApplicationBeneficiary() : base()
         {
         }
         public int application_beneficiary_id { get; set; }
         public int beneficiary_id { get; set; }
         public int dro_application_id { get; set; }
         public int benefit_application_id { get; set; }
         public DateTime start_date { get; set; }
         public DateTime end_date { get; set; }
         public decimal dist_percent { get; set; }
         public int beneficiary_type_id { get; set; }
         public string beneficiary_type_description { get; set; }
         public string beneficiary_type_value { get; set; }
    }
    [Serializable]
    public enum enmBenefitApplicationBeneficiary
    {
         application_beneficiary_id ,
         beneficiary_id ,
         dro_application_id ,
         benefit_application_id ,
         start_date ,
         end_date ,
         dist_percent ,
         beneficiary_type_id ,
         beneficiary_type_description ,
         beneficiary_type_value ,
    }
}

