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
	/// Class NeoSpin.DataObjects.doWssPersonAccountWorkerCompensation:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAccountWorkerCompensation : doBase
    {
         
         public doWssPersonAccountWorkerCompensation() : base()
         {
         }
         public int wss_person_account_worker_compensation_id { get; set; }
         public int wss_person_account_enrollment_request_id { get; set; }
         public int target_account_worker_comp_id { get; set; }
         public string person_name { get; set; }
         public DateTime injury_date { get; set; }
         public string type_of_injury { get; set; }
         public int provider_org_id { get; set; }
         public string company_name { get; set; }
         public string phone_number { get; set; }
    }
    [Serializable]
    public enum enmWssPersonAccountWorkerCompensation
    {
         wss_person_account_worker_compensation_id ,
         wss_person_account_enrollment_request_id ,
         target_account_worker_comp_id ,
         person_name ,
         injury_date ,
         type_of_injury ,
         provider_org_id ,
         company_name ,
         phone_number ,
    }
}

