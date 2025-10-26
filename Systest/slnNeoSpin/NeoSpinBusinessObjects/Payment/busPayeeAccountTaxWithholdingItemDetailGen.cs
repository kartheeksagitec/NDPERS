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
	public class busPayeeAccountTaxWithholdingItemDetailGen : busExtendBase
    {
		public busPayeeAccountTaxWithholdingItemDetailGen()
		{

		}

		private cdoPayeeAccountTaxWithholdingItemDetail _icdoPayeeAccountTaxWithholdingItemDetail;
		public cdoPayeeAccountTaxWithholdingItemDetail icdoPayeeAccountTaxWithholdingItemDetail
		{
			get
			{
				return _icdoPayeeAccountTaxWithholdingItemDetail;
			}
			set
			{
				_icdoPayeeAccountTaxWithholdingItemDetail = value;
			}
		}

		public bool FindPayeeAccountTaxWithholdingItemDetail(int Aintpayeetaxwithholdingitemdtlid)
		{
			bool lblnResult = false;
			if (_icdoPayeeAccountTaxWithholdingItemDetail == null)
			{
				_icdoPayeeAccountTaxWithholdingItemDetail = new cdoPayeeAccountTaxWithholdingItemDetail();
			}
			if (_icdoPayeeAccountTaxWithholdingItemDetail.SelectRow(new object[1] { Aintpayeetaxwithholdingitemdtlid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        private busPaymentItemType _ibusPaymentItemType;

        public busPaymentItemType ibusPaymentItemType
        {
            get { return _ibusPaymentItemType; }
            set { _ibusPaymentItemType = value; }
        }
        public void LoadPaymentItemType()
        {
            if (ibusPaymentItemType == null)
                ibusPaymentItemType = new busPaymentItemType();
            ibusPaymentItemType.FindPaymentItemType(icdoPayeeAccountTaxWithholdingItemDetail.payment_item_type_id);
        }

        public busPayeeAccountPaymentItemType ibusPayeeAccountPaymentItemType { get; set; }

        public void LoadPayeeAccountPaymentItemType()
        {
            if (ibusPayeeAccountPaymentItemType == null)
                ibusPayeeAccountPaymentItemType = new busPayeeAccountPaymentItemType();
            ibusPayeeAccountPaymentItemType.FindPayeeAccountPaymentItemType(icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id);
        }
	}
}
