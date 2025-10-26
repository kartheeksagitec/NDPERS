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
	/// Class NeoSpin.DataObjects.doCorDeathNotification:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCorDeathNotification : doBase
    {
         public doCorDeathNotification() : base()
         {
         }
         public int cor_death_notification_id { get; set; }
         public int death_notification_id { get; set; }
         public int person_id { get; set; }
         public string first_name { get; set; }
         public string middle_name { get; set; }
         public string last_name { get; set; }
         public string is_active { get; set; }
         public string is_retiree { get; set; }
         public string is_retirement_plan { get; set; }
         public string is_insurance_plan { get; set; }
    }
    [Serializable]
    public enum enmCorDeathNotification
    {
         cor_death_notification_id ,
         death_notification_id ,
         person_id ,
         first_name ,
         middle_name ,
         last_name ,
         is_active ,
         is_retiree ,
         is_retirement_plan ,
         is_insurance_plan ,
    }
}
