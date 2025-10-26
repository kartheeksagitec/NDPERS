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
	public class busPayeeAccountPaymentItemTypeGen : busExtendBase
	{
		public busPayeeAccountPaymentItemTypeGen()
		{

		}
        //Property to inlude deduction refund in the screen
        public busPayeeAccountDeductionRefund ibusPayeeAccountDeductionRefund { get; set; }
        public decimal idecamountmultipliedbyitemdirection
        {
            get
            {
                if (ibusPaymentItemType == null)
                    LoadPaymentItemType();
                return icdoPayeeAccountPaymentItemType.amount * ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
            }
        }
		private cdoPayeeAccountPaymentItemType _icdoPayeeAccountPaymentItemType;
		public cdoPayeeAccountPaymentItemType icdoPayeeAccountPaymentItemType
		{
			get
			{
				return _icdoPayeeAccountPaymentItemType;
			}
			set
			{
				_icdoPayeeAccountPaymentItemType = value;
			}
		}
        public int iintOriginalPaymentItemTypeID { get; set; }
		public bool FindPayeeAccountPaymentItemType(int Aintpayeeaccountpaymentitemtypeid)
		{
			bool lblnResult = false;
			if (_icdoPayeeAccountPaymentItemType == null)
			{
				_icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
			}
			if (_icdoPayeeAccountPaymentItemType.SelectRow(new object[1] { Aintpayeeaccountpaymentitemtypeid }))
			{
				lblnResult = true;
			}
			return lblnResult;
		}
        private Collection<busPayeeAccountPaymentItemType> _iclbDeductionHistory;
        public Collection<busPayeeAccountPaymentItemType> iclbDeductionHistory
        {
            get { return _iclbDeductionHistory; }
            set { _iclbDeductionHistory = value; }
        }
        // Load the Payment Item Type(Deduction) object.
        public void LoadPaymentItemType()
        {
            if (ibusPaymentItemType == null)
                ibusPaymentItemType = new busPaymentItemType();
            ibusPaymentItemType.FindPaymentItemType(icdoPayeeAccountPaymentItemType.payment_item_type_id);
        }
        private busPayeeAccount _ibusPayeeAccount;
        public busPayeeAccount ibusPayeeAccount
        {
            get { return _ibusPayeeAccount; }
            set { _ibusPayeeAccount = value; }
        }

        private busPaymentItemType _ibusPaymentItemType;
        public busPaymentItemType ibusPaymentItemType
        {
            get { return _ibusPaymentItemType; }
            set { _ibusPaymentItemType = value; }
        }

        private busOrganization _ibusVendor;
        public busOrganization ibusVendor
        {
            get { return _ibusVendor; }
            set { _ibusVendor = value; }
        }

        private decimal _idecExclusionAmount;
        public decimal idecExclusionAmount
        {
            get { return _idecExclusionAmount; }
            set { _idecExclusionAmount = value; }
        }

        private string _istrExclusionMethod;
        public string istrExclusionMethod
        {
            get { return _istrExclusionMethod; }
            set { _istrExclusionMethod = value; }
        }

        private decimal _idecExclusionRatio;
        public decimal idecExclusionRatio
        {
            get { return _idecExclusionRatio; }
            set { _idecExclusionRatio = value; }
        }

        private decimal _idecNonTaxableAmount;
        public decimal idecNonTaxableAmount
        {
            get { return _idecNonTaxableAmount; }
            set { _idecNonTaxableAmount = value; }
        }

        private decimal _idecBalanceNonTaxableAmount;
        public decimal idecBalanceNonTaxableAmount
        {
            get { return _idecBalanceNonTaxableAmount; }
            set { _idecBalanceNonTaxableAmount = value; }
        }
	}
}
