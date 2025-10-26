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
	/// Class NeoSpin.DataObjects.doCorBeneDep:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCorBeneDep : doBase
    {
         public doCorBeneDep() : base()
         {
         }
         public int cor_bene_dep_id { get; set; }
         public int death_notification_id { get; set; }
         public int person_id { get; set; }
         public int org_id { get; set; }
         public int person_dependent_id { get; set; }
         public int person_account_id { get; set; }
         public string beneficiary_relationship { get; set; }
         public string dependent_relationship { get; set; }
         public string family_relationship { get; set; }
         public string account_relationship { get; set; }
         public int beneficiary_id { get; set; }
    }
    [Serializable]
    public enum enmCorBeneDep
    {
         cor_bene_dep_id ,
         death_notification_id ,
         person_id ,
         org_id ,
         person_dependent_id ,
         person_account_id ,
         beneficiary_relationship ,
         dependent_relationship ,
         family_relationship ,
         account_relationship ,
         beneficiary_id,
    }
}
