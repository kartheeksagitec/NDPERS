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
	/// Class NeoSpin.DataObjects.doUserChangeHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doUserChangeHistory : doBase
    {
         
         public doUserChangeHistory() : base()
         {
         }
         public int user_change_id { get; set; }
         public int user_serial_id { get; set; }
         public string previous_user_id { get; set; }
         public string previous_first_name { get; set; }
         public string previous_middle_initial { get; set; }
         public string previous_last_name { get; set; }
    }
    [Serializable]
    public enum enmUserChangeHistory
    {
         user_change_id ,
         user_serial_id ,
         previous_user_id ,
         previous_first_name ,
         previous_middle_initial ,
         previous_last_name ,
    }
}

