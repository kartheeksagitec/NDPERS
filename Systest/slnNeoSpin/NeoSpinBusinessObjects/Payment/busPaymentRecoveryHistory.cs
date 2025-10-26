#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busPaymentRecoveryHistory:
	/// Inherited from busPaymentRecoveryHistoryGen, the class is used to customize the business object busPaymentRecoveryHistoryGen.
	/// </summary>
	[Serializable]
	public class busPaymentRecoveryHistory : busPaymentRecoveryHistoryGen
	{
        //Property to contain payment recovery
        public busPaymentRecovery ibusPaymentRecovery { get; set; }
        /// <summary>
        /// method to load Payment recovery
        /// </summary>
        /// <returns>boolean value</returns>
        public bool LoadPaymentRecovery()
        {
            if (ibusPaymentRecovery == null)
                ibusPaymentRecovery = new busPaymentRecovery();
            return ibusPaymentRecovery.FindPaymentRecovery(icdoPaymentRecoveryHistory.payment_recovery_id);
        }

        /// <summary>
        /// Method to reset the amount fields to zero
        /// </summary>
        public void ResetAmountFields()
        {
            if (icdoPaymentRecoveryHistory.allocated_amount != 0 || icdoPaymentRecoveryHistory.amortization_interest_paid != 0 ||
                icdoPaymentRecoveryHistory.principle_amount_paid != 0)
            {
                icdoPaymentRecoveryHistory.allocated_amount = icdoPaymentRecoveryHistory.amortization_interest_paid = icdoPaymentRecoveryHistory.principle_amount_paid = 0;
                icdoPaymentRecoveryHistory.Update();
            }
        }
	}
}
