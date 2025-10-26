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
	[Serializable]
	public class busPayeeAccountDeductionRefundItemGen : busExtendBase
    {
		public busPayeeAccountDeductionRefundItemGen()
		{

		}

		public cdoPayeeAccountDeductionRefundItem icdoPayeeAccountDeductionRefundItem { get; set; }

		public busPayeeAccountDeductionRefund ibusPayeeAccountDeductionRefund { get; set; }

		public busPaymentItemType ibusPaymentItemType { get; set; }




		public virtual bool FindPayeeAccountDeductionRefundItem(int Aintpayeeaccountdeductionrefunditemid)
		{
			bool lblnResult = false;
			if (icdoPayeeAccountDeductionRefundItem == null)
			{
				icdoPayeeAccountDeductionRefundItem = new cdoPayeeAccountDeductionRefundItem();
			}
			if (icdoPayeeAccountDeductionRefundItem.SelectRow(new object[1] { Aintpayeeaccountdeductionrefunditemid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

		public virtual void LoadibusPayeeAccountDeductionRefund()
		{
			if (ibusPayeeAccountDeductionRefund == null)
			{
				ibusPayeeAccountDeductionRefund = new busPayeeAccountDeductionRefund();
			}
			ibusPayeeAccountDeductionRefund.FindPayeeAccountDeductionRefund(icdoPayeeAccountDeductionRefundItem.payee_account_deduction_refund_id);
		}

		public virtual void LoadibusPaymentItemType()
		{
			if (ibusPaymentItemType == null)
			{
				ibusPaymentItemType = new busPaymentItemType();
			}
			ibusPaymentItemType.FindPaymentItemType(icdoPayeeAccountDeductionRefundItem.payment_item_type_id);
		}

	}
}
