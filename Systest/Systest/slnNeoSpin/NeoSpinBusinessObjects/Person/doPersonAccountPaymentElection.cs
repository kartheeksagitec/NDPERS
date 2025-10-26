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
	/// Class NeoSpin.DataObjects.doPersonAccountPaymentElection:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPersonAccountPaymentElection : doBase
    {
         
         public doPersonAccountPaymentElection() : base()
         {
         }
         public int account_payment_election_id { get; set; }
         public int person_account_id { get; set; }
         public int payee_account_id { get; set; }
         public string ibs_flag { get; set; }
         public string ibs_supplemental_flag { get; set; }
         public int payment_method_id { get; set; }
         public string payment_method_description { get; set; }
         public string payment_method_value { get; set; }
         public DateTime ibs_effective_date { get; set; }
         public int ibs_org_id { get; set; }
         public int ibs_supplemental_org_id { get; set; }
         public decimal cobra_empr_share { get; set; }
    }
    [Serializable]
    public enum enmPersonAccountPaymentElection
    {
         account_payment_election_id ,
         person_account_id ,
         payee_account_id ,
         ibs_flag ,
         ibs_supplemental_flag ,
         payment_method_id ,
         payment_method_description ,
         payment_method_value ,
         ibs_effective_date ,
         ibs_org_id ,
         ibs_supplemental_org_id ,
         cobra_empr_share ,
    }
}

