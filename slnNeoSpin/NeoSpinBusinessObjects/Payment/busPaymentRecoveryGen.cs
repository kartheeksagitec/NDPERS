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
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPaymentRecoveryGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentRecovery and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentRecoveryGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPaymentRecoveryGen
        /// </summary>
		public busPaymentRecoveryGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentRecoveryGen.
        /// </summary>
		public cdoPaymentRecovery icdoPaymentRecovery { get; set; }


        /// <summary>
        /// Gets or sets the collection object of type busPayeeAccountRetroPaymentRecoveryDetail. 
        /// </summary>
		public Collection<busPayeeAccountRetroPaymentRecoveryDetail> iclbPayeeAccountRetroPaymentRecoveryDetail { get; set; }

        /// <summary>
        /// Gets or sets the collection object of type busPaymentRecoveryHistory. 
        /// </summary>
		public Collection<busPaymentRecoveryHistory> iclbPaymentRecoveryHistory { get; set; }



        /// <summary>
        /// NeoSpin.busPaymentRecoveryGen.FindPaymentRecovery():
        /// Finds a particular record from cdoPaymentRecovery with its primary key. 
        /// </summary>
        /// <param name="aintpaymentrecoveryid">A primary key value of type int of cdoPaymentRecovery on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentRecovery(int aintpaymentrecoveryid)
		{
			bool lblnResult = false;
			if (icdoPaymentRecovery == null)
			{
				icdoPaymentRecovery = new cdoPaymentRecovery();
			}
			if (icdoPaymentRecovery.SelectRow(new object[1] { aintpaymentrecoveryid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

        /// <summary>
        /// NeoSpin.busPaymentRecoveryGen.LoadPayeeAccountRetroPaymentRecoveryDetails():
        /// Loads Collection object iclbPayeeAccountRetroPaymentRecoveryDetail of type busPayeeAccountRetroPaymentRecoveryDetail.
        /// </summary>
		public virtual void LoadPayeeAccountRetroPaymentRecoveryDetails()
		{
			DataTable ldtbList = Select<cdoPayeeAccountRetroPaymentRecoveryDetail>(
				new string[1] { enmPayeeAccountRetroPaymentRecoveryDetail.payment_recovery_id.ToString() },
				new object[1] { icdoPaymentRecovery.payment_recovery_id }, null, null);
			iclbPayeeAccountRetroPaymentRecoveryDetail = GetCollection<busPayeeAccountRetroPaymentRecoveryDetail>(ldtbList, "icdoPayeeAccountRetroPaymentRecoveryDetail");
		}        

	}
}
