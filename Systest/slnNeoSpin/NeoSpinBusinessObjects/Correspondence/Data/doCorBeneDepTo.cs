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
	/// Class NeoSpin.DataObjects.doCorBeneDepTo:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCorBeneDepTo : doBase
    {
         public doCorBeneDepTo() : base()
         {
         }
         public int cor_bene_dep_to_id { get; set; }
         public int death_notification_id { get; set; }
         public int person_account_id { get; set; }
         public string dependent_to { get; set; }
         public string beneficiary_to { get; set; }
         public int person_account_beneficiary_id { get; set; }
         public int person_account_dependent_id { get; set; }
         public string beneficiary_relationship { get; set; }
         public string dependent_relationship { get; set; }
    }
    [Serializable]
    public enum enmCorBeneDepTo
    {
         cor_bene_dep_to_id ,
         death_notification_id ,
         person_account_id ,
         dependent_to ,
         beneficiary_to ,
         person_account_beneficiary_id ,
         person_account_dependent_id ,
         beneficiary_relationship ,
         dependent_relationship ,
    }
}
