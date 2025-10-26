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
    public class busPayeeAccountTaxWithholdingGen : busExtendBase
    {
        public busPayeeAccountTaxWithholdingGen()
        {

        }
        public bool iblnIsUpdateMode = false;
        public bool IsUpdateModeEndDateNotNull()
        {
            if ((iblnIsUpdateMode) &&
                    (icdoPayeeAccountTaxWithholding.end_date != DateTime.MinValue))
            {
                return true;
            }
            return false;
        }
        private bool _iblnIsSaveButtonClicked;

        public bool iblnIsSaveButtonClicked
        {
            get { return _iblnIsSaveButtonClicked; }
            set { _iblnIsSaveButtonClicked = value; }
        }
	
        private cdoPayeeAccountTaxWithholding _icdoPayeeAccountTaxWithholding;
        public cdoPayeeAccountTaxWithholding icdoPayeeAccountTaxWithholding
        {
            get
            {
                return _icdoPayeeAccountTaxWithholding;
            }
            set
            {
                _icdoPayeeAccountTaxWithholding = value;
            }
        }

        private busPayeeAccount _ibusPayeeAccount;
        public busPayeeAccount ibusPayeeAccount
        {
            get
            {
                return _ibusPayeeAccount;
            }
            set
            {
                _ibusPayeeAccount = value;
            }
        }
        public bool FindPayeeAccountTaxWithholding(int Aintpayeeaccounttaxwithholdingid)
        {
            bool lblnResult = false;
            if (_icdoPayeeAccountTaxWithholding == null)
            {
                _icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
            }
            if (_icdoPayeeAccountTaxWithholding.SelectRow(new object[1] { Aintpayeeaccounttaxwithholdingid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public void LoadPayeeAccount()
        {
            if (_ibusPayeeAccount == null)
            {
                _ibusPayeeAccount = new busPayeeAccount();
            }
            _ibusPayeeAccount.FindPayeeAccount(_icdoPayeeAccountTaxWithholding.payee_account_id);
        }
        private Collection<busPayeeAccountTaxWithholdingItemDetail> _iclbTaxWithHoldingTaxItems;

        public Collection<busPayeeAccountTaxWithholdingItemDetail> iclbTaxWithHoldingTaxItems
        {
            get { return _iclbTaxWithHoldingTaxItems; }
            set { _iclbTaxWithHoldingTaxItems = value; }
        }

        private Collection<busPayeeAccountPaymentItemType> _iclbPayeeAccountTaxItems;

        public Collection<busPayeeAccountPaymentItemType> iclbPayeeAccountTaxItems
        {
            get { return _iclbPayeeAccountTaxItems; }
            set { _iclbPayeeAccountTaxItems = value; }
        }


        public void LoadTaxWithHoldingTaxItems()
        {
            _iclbTaxWithHoldingTaxItems = new Collection<busPayeeAccountTaxWithholdingItemDetail>();
            DataTable ldtbTax = Select("cdoPayeeAccountTaxWithholding.LoadTaxITems",
                new object[1] { icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id });
            foreach (DataRow dr in ldtbTax.Rows)
            {
                busPayeeAccountTaxWithholdingItemDetail lobjPayeeAccountTaxWithholdingItemDetail = new busPayeeAccountTaxWithholdingItemDetail();
                lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail = new cdoPayeeAccountTaxWithholdingItemDetail();
                lobjPayeeAccountTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.LoadData(dr);
                lobjPayeeAccountTaxWithholdingItemDetail.ibusPaymentItemType = new busPaymentItemType();
                lobjPayeeAccountTaxWithholdingItemDetail.ibusPaymentItemType.icdoPaymentItemType = new cdoPaymentItemType();
                lobjPayeeAccountTaxWithholdingItemDetail.ibusPaymentItemType.icdoPaymentItemType.LoadData(dr);
                lobjPayeeAccountTaxWithholdingItemDetail.LoadPayeeAccountPaymentItemType();
                _iclbTaxWithHoldingTaxItems.Add(lobjPayeeAccountTaxWithholdingItemDetail);
            }
        }
        public void LoadPayeeAccountTaxItems()
        {
            _iclbPayeeAccountTaxItems = new Collection<busPayeeAccountPaymentItemType>();
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            if (_iclbTaxWithHoldingTaxItems == null)
                LoadTaxWithHoldingTaxItems();
            foreach (busPayeeAccountPaymentItemType lobjTaxItems in ibusPayeeAccount.iclbPayeeAccountPaymentItemType)
            {
                foreach (busPayeeAccountTaxWithholdingItemDetail lobjTaxItemDetail in _iclbTaxWithHoldingTaxItems)
                {
                    if (lobjTaxItems.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id
                        == lobjTaxItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id)
                    {
                        _iclbPayeeAccountTaxItems.Add(lobjTaxItems);
                    }
                }
            }
        }
    }
}