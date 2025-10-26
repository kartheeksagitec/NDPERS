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
	/// Class NeoSpin.DataObjects.doWssPersonAccountOtherCoverageDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAccountOtherCoverageDetail : doBase
    {
         
         public doWssPersonAccountOtherCoverageDetail() : base()
         {
         }
         public int wss_person_account_other_coverage_detail_id { get; set; }
         public int wss_person_account_enrollment_request_id { get; set; }
         public int target_account_other_coverage_detail_id { get; set; }
         public int provider_org_id { get; set; }
         public string policy_number { get; set; }
         public string policy_holder { get; set; }
         public DateTime date_of_birth { get; set; }
         public DateTime policy_start_date { get; set; }
         public DateTime policy_end_date { get; set; }
         public string covered_person { get; set; }
         public string other_coverage_number { get; set; }
         public string provider_org_name { get; set; }
    }
    [Serializable]
    public enum enmWssPersonAccountOtherCoverageDetail
    {
         wss_person_account_other_coverage_detail_id ,
         wss_person_account_enrollment_request_id ,
         target_account_other_coverage_detail_id ,
         provider_org_id ,
         policy_number ,
         policy_holder ,
         date_of_birth ,
         policy_start_date ,
         policy_end_date ,
         covered_person ,
         other_coverage_number ,
         provider_org_name ,
    }
}

