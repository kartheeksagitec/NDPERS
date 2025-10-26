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
	/// Class NeoSpin.DataObjects.doCorInsurance:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCorInsurance : doBase
    {
         public doCorInsurance() : base()
         {
         }
         public int cor_insurance_id { get; set; }
         public int death_notification_id { get; set; }
         public int person_account_id { get; set; }
         public int person_dependent_id { get; set; }
    }
    [Serializable]
    public enum enmCorInsurance
    {
         cor_insurance_id ,
         death_notification_id ,
         person_account_id ,
         person_dependent_id ,
    }
}
