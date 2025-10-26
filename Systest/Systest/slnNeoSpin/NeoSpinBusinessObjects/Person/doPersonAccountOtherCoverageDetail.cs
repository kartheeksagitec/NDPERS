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
	/// Class NeoSpin.DataObjects.doPersonAccountOtherCoverageDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountOtherCoverageDetail : doBase
    {
         
         public doPersonAccountOtherCoverageDetail() : base()
         {
         }
         public int account_other_coverage_detail_id { get; set; }
         public int person_account_id { get; set; }
         public int provider_org_id { get; set; }
         public string policy_number { get; set; }
         public string policy_holder { get; set; }
         public DateTime date_of_birth { get; set; }
         public DateTime policy_start_date { get; set; }
         public DateTime policy_end_date { get; set; }
         public string covered_person { get; set; }
         public string provider_org_name { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountOtherCoverageDetail
    {
         account_other_coverage_detail_id ,
         person_account_id ,
         provider_org_id ,
         policy_number ,
         policy_holder ,
         date_of_birth ,
         policy_start_date ,
         policy_end_date ,
         covered_person ,
         provider_org_name ,
    }
}

