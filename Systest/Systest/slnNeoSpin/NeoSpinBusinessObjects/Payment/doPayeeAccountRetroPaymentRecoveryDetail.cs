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
	/// Class NeoSpin.DataObjects.doPayeeAccountRetroPaymentRecoveryDetail:
	/// Inherited from doBase, the class is used to create a wrapper of database table object.
	/// Each property of an instance of this class represents a column of database table object.  
	/// </summary>
    [Serializable]
    public class doPayeeAccountRetroPaymentRecoveryDetail : doBase
    {
         
         public doPayeeAccountRetroPaymentRecoveryDetail() : base()
         {
         }
         public int retro_payment_recovery_detail_id { get; set; }
         public int payee_account_retro_payment_id { get; set; }
         public int payment_recovery_id { get; set; }
    }
    [Serializable]
    public enum enmPayeeAccountRetroPaymentRecoveryDetail
    {
         retro_payment_recovery_detail_id ,
         payee_account_retro_payment_id ,
         payment_recovery_id ,
    }
}

