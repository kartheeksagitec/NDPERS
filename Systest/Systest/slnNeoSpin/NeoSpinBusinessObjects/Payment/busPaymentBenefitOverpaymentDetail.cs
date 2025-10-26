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
	/// Class NeoSpin.BusinessObjects.busPaymentBenefitOverpaymentDetail:
	/// Inherited from busPaymentBenefitOverpaymentDetailGen, the class is used to customize the business object busPaymentBenefitOverpaymentDetailGen.
	/// </summary>
	[Serializable]
	public class busPaymentBenefitOverpaymentDetail : busPaymentBenefitOverpaymentDetailGen
	{
        //property to contain Payment Item Type
        public busPaymentItemType ibusPaymentItemType { get; set; }
        /// <summary>
        /// Method to load Payment Item Type
        /// </summary>
        public void LoadPaymentItemType()
        {
            if (ibusPaymentItemType == null)
                ibusPaymentItemType = new busPaymentItemType();
            ibusPaymentItemType.FindPaymentItemType(icdoPaymentBenefitOverpaymentDetail.payment_item_type_id);
        }
	}
}
