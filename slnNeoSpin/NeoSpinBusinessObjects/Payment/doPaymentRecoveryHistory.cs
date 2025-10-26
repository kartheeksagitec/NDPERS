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
	/// Class NeoSpin.DataObjects.doPaymentRecoveryHistory:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPaymentRecoveryHistory : doBase
    {
         
         public doPaymentRecoveryHistory() : base()
         {
         }
         public int recovery_history_id { get; set; }
         public int payment_recovery_id { get; set; }
         public DateTime posted_date { get; set; }
         public int remittance_id { get; set; }
         public int payment_history_header_id { get; set; }
         public decimal principle_amount_paid { get; set; }
         public decimal amortization_interest_paid { get; set; }
         public decimal allocated_amount { get; set; }
    }
    [Serializable]
    public enum enmPaymentRecoveryHistory
    {
         recovery_history_id ,
         payment_recovery_id ,
         posted_date ,
         remittance_id ,
         payment_history_header_id ,
         principle_amount_paid ,
         amortization_interest_paid ,
         allocated_amount ,
    }
}

