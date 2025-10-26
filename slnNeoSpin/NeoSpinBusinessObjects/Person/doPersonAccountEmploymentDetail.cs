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
	/// Class NeoSpin.DataObjects.doPersonAccountEmploymentDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountEmploymentDetail : doBase
    {
         
         public doPersonAccountEmploymentDetail() : base()
         {
         }
         public int person_account_employment_dtl_id { get; set; }
         public int person_employment_dtl_id { get; set; }
         public int plan_id { get; set; }
         public int person_account_id { get; set; }
         public string applicable_flag { get; set; }
         public int election_id { get; set; }
         public string election_description { get; set; }
         public string election_value { get; set; }
         public string db_batch_letter_sent_flag { get; set; }
         public string is_waiver_report_generated { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountEmploymentDetail
    {
         person_account_employment_dtl_id ,
         person_employment_dtl_id ,
         plan_id ,
         person_account_id ,
         applicable_flag ,
         election_id ,
         election_description ,
         election_value ,
         db_batch_letter_sent_flag ,
         is_waiver_report_generated ,
    }
}

