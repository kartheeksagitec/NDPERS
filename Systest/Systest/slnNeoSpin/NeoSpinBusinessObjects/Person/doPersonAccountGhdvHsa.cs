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
	/// Class NeoSpin.DataObjects.doPersonAccountGhdvHsa:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountGhdvHsa : doBase
    {
         
         public doPersonAccountGhdvHsa() : base()
         {
         }
         public int person_account_ghdv_hsa_id { get; set; }
         public int person_account_ghdv_id { get; set; }
         public int person_account_id { get; set; }
         public DateTime contribution_start_date { get; set; }
         public DateTime contribution_end_date { get; set; }
         public decimal contribution_amount { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountGhdvHsa
    {
         person_account_ghdv_hsa_id ,
         person_account_ghdv_id ,
         person_account_id ,
         contribution_start_date ,
         contribution_end_date ,
         contribution_amount ,
    }
}
