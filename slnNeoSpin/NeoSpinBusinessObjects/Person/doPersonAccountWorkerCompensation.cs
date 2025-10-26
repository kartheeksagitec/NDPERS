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
	/// Class NeoSpin.DataObjects.doPersonAccountWorkerCompensation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountWorkerCompensation : doBase
    {
         
         public doPersonAccountWorkerCompensation() : base()
         {
         }
         public int account_worker_comp_id { get; set; }
         public int person_account_id { get; set; }
         public string person_name { get; set; }
         public DateTime injury_date { get; set; }
         public string type_of_injury { get; set; }
         public int provider_org_id { get; set; }
         public string provider_org_name { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountWorkerCompensation
    {
         account_worker_comp_id ,
         person_account_id ,
         person_name ,
         injury_date ,
         type_of_injury ,
         provider_org_id ,
         provider_org_name ,
    }
}

