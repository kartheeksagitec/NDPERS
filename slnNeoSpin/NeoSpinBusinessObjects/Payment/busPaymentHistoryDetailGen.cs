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
	public class busPaymentHistoryDetailGen : busExtendBase
    {
		public busPaymentHistoryDetailGen()
		{

		}

		public cdoPaymentHistoryDetail icdoPaymentHistoryDetail { get; set; }


        public busPaymentItemType ibusPaymentItemType { get; set; }
        public void LoadPaymentItemType()
        {
            if (ibusPaymentItemType == null)
                ibusPaymentItemType = new busPaymentItemType();
            ibusPaymentItemType.FindPaymentItemType(icdoPaymentHistoryDetail.payment_item_type_id);
        }
		public virtual bool FindPaymentHistoryDetail(int Aintpaymenthistoryitemdetailid)
		{
			bool lblnResult = false;
			if (icdoPaymentHistoryDetail == null)
			{
				icdoPaymentHistoryDetail = new cdoPaymentHistoryDetail();
			}
			if (icdoPaymentHistoryDetail.SelectRow(new object[1] { Aintpaymenthistoryitemdetailid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}

	}
}
