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
	/// Class NeoSpin.DataObjects.doPaymentElectionAdjustment:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentElectionAdjustment : doBase
    {
         
         public doPaymentElectionAdjustment() : base()
         {
         }
         public int payment_election_adjustment_id { get; set; }
         public int ibs_header_id { get; set; }
         public int person_account_id { get; set; }
         public int payee_account_id { get; set; }
         public int payment_option_id { get; set; }
         public string payment_option_description { get; set; }
         public string payment_option_value { get; set; }
         public int repayment_type_id { get; set; }
         public string repayment_type_description { get; set; }
         public string repayment_type_value { get; set; }
         public int status_id { get; set; }
         public string status_description { get; set; }
         public string status_value { get; set; }
         public decimal total_adjustment_amount { get; set; }
         public decimal monthly_amount { get; set; }
         public DateTime approved_date { get; set; }
         public int provider_org_id { get; set; }
    }
    [Serializable]
    public enum enmPaymentElectionAdjustment
    {
         payment_election_adjustment_id ,
         ibs_header_id ,
         person_account_id ,
         payee_account_id ,
         payment_option_id ,
         payment_option_description ,
         payment_option_value ,
         repayment_type_id ,
         repayment_type_description ,
         repayment_type_value ,
         status_id ,
         status_description ,
         status_value ,
         total_adjustment_amount ,
         monthly_amount ,
         approved_date ,
         provider_org_id ,
    }
}

