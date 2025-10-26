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
	/// Class NeoSpin.DataObjects.doPersonAccountPaymentElectionHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountPaymentElectionHistory : doBase
    {
         
         public doPersonAccountPaymentElectionHistory() : base()
         {
         }
         public int account_payment_election_history_id { get; set; }
         public int account_payment_election_id { get; set; }
         public int person_account_id { get; set; }
         public string ibs_flag { get; set; }
         public int payment_method_id { get; set; }
         public string payment_method_description { get; set; }
         public string payment_method_value { get; set; }
         public DateTime history_change_date { get; set; }
         public DateTime ibs_effective_date { get; set; }
         public int ibs_org_id { get; set; }
         public string ibs_supplemental_flag { get; set; }
         public int ibs_supplemental_org_id { get; set; }
         public int payee_account_id { get; set; }
         public decimal cobra_empr_share { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountPaymentElectionHistory
    {
         account_payment_election_history_id ,
         account_payment_election_id ,
         person_account_id ,
         ibs_flag ,
         payment_method_id ,
         payment_method_description ,
         payment_method_value ,
         history_change_date ,
         ibs_effective_date ,
         ibs_org_id ,
         ibs_supplemental_flag ,
         ibs_supplemental_org_id ,
         payee_account_id ,
         cobra_empr_share ,
    }
}

