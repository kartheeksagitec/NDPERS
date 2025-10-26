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
	/// Class NeoSpin.DataObjects.doWssPersonAccountDeferredComp:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doWssPersonAccountDeferredComp : doBase
    {
         
         public doWssPersonAccountDeferredComp() : base()
         {
         }
         public int wss_person_account_deferred_comp_id { get; set; }
         public int wss_person_account_enrollment_request_id { get; set; }
         public int target_person_account_id { get; set; }
         public DateTime catch_up_start_date { get; set; }
         public DateTime catch_up_end_date { get; set; }
         public int limit_457_id { get; set; }
         public string limit_457_description { get; set; }
         public string limit_457_value { get; set; }
         public string hardship_withdrawal_flag { get; set; }
         public DateTime hardship_withdrawal_effective_date { get; set; }
         public string de_minimus_distribution_flag { get; set; }
         public string file_457_sent_flag { get; set; }
    }
    [Serializable]
    public enum enmWssPersonAccountDeferredComp
    {
         wss_person_account_deferred_comp_id ,
         wss_person_account_enrollment_request_id ,
         target_person_account_id ,
         catch_up_start_date ,
         catch_up_end_date ,
         limit_457_id ,
         limit_457_description ,
         limit_457_value ,
         hardship_withdrawal_flag ,
         hardship_withdrawal_effective_date ,
         de_minimus_distribution_flag ,
         file_457_sent_flag ,
    }
}

