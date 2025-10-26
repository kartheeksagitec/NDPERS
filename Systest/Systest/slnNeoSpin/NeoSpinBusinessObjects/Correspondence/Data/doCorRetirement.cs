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
	/// Class NeoSpin.DataObjects.doCorRetirement:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doCorRetirement : doBase
    {
         public doCorRetirement() : base()
         {
         }
         public int cor_retirement_id { get; set; }
         public int death_notification_id { get; set; }
         public int person_account_id { get; set; }
         public string is_vested { get; set; }
         public string is_db_retirement { get; set; }
         public string is_dc_retirement { get; set; }
    }
    [Serializable]
    public enum enmCorRetirement
    {
         cor_retirement_id ,
         death_notification_id ,
         person_account_id ,
         is_vested ,
         is_db_retirement ,
         is_dc_retirement ,
    }
}
