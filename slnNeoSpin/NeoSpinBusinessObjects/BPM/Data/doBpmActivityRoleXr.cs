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
	/// Class NeoSpin.DataObjects.doBpmActivityRoleXr:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doBpmActivityRoleXr : doBase
    {
         public doBpmActivityRoleXr() : base()
         {
         }
         public int activity_role_xr_id { get; set; }
         public int activity_id { get; set; }
         public int role_id { get; set; }
    }
    [Serializable]
    public enum enmBpmActivityRoleXr
    {
         activity_role_xr_id ,
         activity_id ,
         role_id ,
    }
}
