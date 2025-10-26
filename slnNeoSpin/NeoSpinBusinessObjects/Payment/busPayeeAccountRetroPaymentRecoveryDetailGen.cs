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
    /// Class NeoSpin.BusinessObjects.busPayeeAccountRetroPaymentRecoveryDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPayeeAccountRetroPaymentRecoveryDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPayeeAccountRetroPaymentRecoveryDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPayeeAccountRetroPaymentRecoveryDetailGen
        /// </summary>
		public busPayeeAccountRetroPaymentRecoveryDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPayeeAccountRetroPaymentRecoveryDetailGen.
        /// </summary>
		public cdoPayeeAccountRetroPaymentRecoveryDetail icdoPayeeAccountRetroPaymentRecoveryDetail { get; set; }




        /// <summary>
        /// NeoSpin.busPayeeAccountRetroPaymentRecoveryDetailGen.FindPayeeAccountRetroPaymentRecoveryDetail():
        /// Finds a particular record from cdoPayeeAccountRetroPaymentRecoveryDetail with its primary key. 
        /// </summary>
        /// <param name="aintretropaymentrecoverydetailid">A primary key value of type int of cdoPayeeAccountRetroPaymentRecoveryDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPayeeAccountRetroPaymentRecoveryDetail(int aintretropaymentrecoverydetailid)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountRetroPaymentRecoveryDetail == null)
			{
				icdoPayeeAccountRetroPaymentRecoveryDetail = new cdoPayeeAccountRetroPaymentRecoveryDetail();
			}
			if (icdoPayeeAccountRetroPaymentRecoveryDetail.SelectRow(new object[1] { aintretropaymentrecoverydetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
