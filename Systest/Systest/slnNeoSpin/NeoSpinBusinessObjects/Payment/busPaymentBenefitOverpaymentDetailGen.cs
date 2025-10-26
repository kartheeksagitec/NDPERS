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
    /// Class NeoSpin.BusinessObjects.busPaymentBenefitOverpaymentDetailGen:
    /// Inherited from busBase, used to create new business object for main table cdoPaymentBenefitOverpaymentDetail and its children table. 
    /// </summary>
	[Serializable]
	public class busPaymentBenefitOverpaymentDetailGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.BusinessObjects.busPaymentBenefitOverpaymentDetailGen
        /// </summary>
		public busPaymentBenefitOverpaymentDetailGen()
		{

		}

        /// <summary>
        /// Gets or sets the main-table object contained in busPaymentBenefitOverpaymentDetailGen.
        /// </summary>
		public cdoPaymentBenefitOverpaymentDetail icdoPaymentBenefitOverpaymentDetail { get; set; }




        /// <summary>
        /// NeoSpin.busPaymentBenefitOverpaymentDetailGen.FindPaymentBenefitOverpaymentDetail():
        /// Finds a particular record from cdoPaymentBenefitOverpaymentDetail with its primary key. 
        /// </summary>
        /// <param name="aintbenefitoverpaymentdetailid">A primary key value of type int of cdoPaymentBenefitOverpaymentDetail on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
		public virtual bool FindPaymentBenefitOverpaymentDetail(int aintbenefitoverpaymentdetailid)
		{
			bool lblnResult = false;
			if (icdoPaymentBenefitOverpaymentDetail == null)
			{
				icdoPaymentBenefitOverpaymentDetail = new cdoPaymentBenefitOverpaymentDetail();
			}
			if (icdoPaymentBenefitOverpaymentDetail.SelectRow(new object[1] { aintbenefitoverpaymentdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
