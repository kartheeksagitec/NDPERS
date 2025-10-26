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
    public class busPayeeAccountRetroPaymentGen : busExtendBase
    {
        public busPayeeAccountRetroPaymentGen()
        {

        }

        private cdoPayeeAccountRetroPayment _icdoPayeeAccountRetroPayment;
        public cdoPayeeAccountRetroPayment icdoPayeeAccountRetroPayment
        {
            get
            {
                return _icdoPayeeAccountRetroPayment;
            }
            set
            {
                _icdoPayeeAccountRetroPayment = value;
            }
        }
        public decimal idecRetroTotalAmount { get; set; }
        public decimal idecRetroTaxableAmount { get; set; }
        public decimal idecRetroNonTaxableAmount { get; set; }
        public void LoadRetroTaxableAndNonTaxableAmount()
        {
            if (iclbPayeeAccountRetroPaymentDetail == null)
                LoadPayeeAccountRetroPaymentDetail();
            foreach (busPayeeAccountRetroPaymentDetail lobjRetroPaymentdetail in iclbPayeeAccountRetroPaymentDetail)
            {
                if (lobjRetroPaymentdetail.ibusPaymentItemType == null)
                    lobjRetroPaymentdetail.LoadPaymentItemType();
                //UAT PIR 1231 : code added so as to take only items which have item direction 1 to calculate taxable and non taxable amount
                if (lobjRetroPaymentdetail.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1)
                {
                    if (lobjRetroPaymentdetail.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                        idecRetroTaxableAmount += lobjRetroPaymentdetail.icdoPayeeAccountRetroPaymentDetail.amount;
                    else
                        idecRetroNonTaxableAmount += lobjRetroPaymentdetail.icdoPayeeAccountRetroPaymentDetail.amount;
                }
            }
            idecRetroTotalAmount = idecRetroTaxableAmount + idecRetroNonTaxableAmount;
        }
        public bool FindPayeeAccountRetroPayment(int Aintpayeeaccountretropaymentid)
        {
            bool lblnResult = false;
            if (_icdoPayeeAccountRetroPayment == null)
            {
                _icdoPayeeAccountRetroPayment = new cdoPayeeAccountRetroPayment();
            }
            if (_icdoPayeeAccountRetroPayment.SelectRow(new object[1] { Aintpayeeaccountretropaymentid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        private busPayeeAccount _ibusPayeeAccount;

        public busPayeeAccount ibusPayeeAccount
        {
            get { return _ibusPayeeAccount; }
            set { _ibusPayeeAccount = value; }
        }
        public void LoadPayeeAccount()
        {
            if (_ibusPayeeAccount == null)
            {
                _ibusPayeeAccount = new busPayeeAccount();
            }
            _ibusPayeeAccount.FindPayeeAccount(icdoPayeeAccountRetroPayment.payee_account_id);
        }
        private Collection<busPayeeAccountRetroPaymentDetail> _iclbPayeeAccountRetroPaymentDetail;

        public Collection<busPayeeAccountRetroPaymentDetail> iclbPayeeAccountRetroPaymentDetail
        {
            get { return _iclbPayeeAccountRetroPaymentDetail; }
            set { _iclbPayeeAccountRetroPaymentDetail = value; }
        }
        private Collection<busPaymentItemType> _iclbTaxItems;

        public Collection<busPaymentItemType> iclbTaxItems
        {
            get { return _iclbTaxItems; }
            set { _iclbTaxItems = value; }
        }
        public Collection<busPayeeAccountMonthwiseAdjustmentDetail> iclbRetrMonthWiseAdjustMentDetail { get; set; }

        public void LoadRetrMonthWiseAdjustMentDetail()
        {
            DataTable ldtbList = Select<cdoPayeeAccountMonthwiseAdjustmentDetail>(
                     new string[1] { "payee_account_retro_payment_id" },
                     new object[1] { icdoPayeeAccountRetroPayment.payee_account_retro_payment_id }, null, null);
            iclbRetrMonthWiseAdjustMentDetail = GetCollection<busPayeeAccountMonthwiseAdjustmentDetail>(ldtbList, "icdoPayeeAccountMonthwiseAdjustmentDetail");
        }
        public void LoadPayeeAccountRetroPaymentDetail()
        {
            DataTable ldtbList = Select<cdoPayeeAccountRetroPaymentDetail>(
                     new string[1] { "payee_account_retro_payment_id" },
                     new object[1] { icdoPayeeAccountRetroPayment.payee_account_retro_payment_id }, null, null);
            _iclbPayeeAccountRetroPaymentDetail = GetCollection<busPayeeAccountRetroPaymentDetail>(ldtbList, "icdoPayeeAccountRetroPaymentDetail");
            foreach (busPayeeAccountRetroPaymentDetail lobjPayeeAccountRetroPaymentCollection in _iclbPayeeAccountRetroPaymentDetail)
            {
                lobjPayeeAccountRetroPaymentCollection.LoadPaymentItemType();
            }
        }
        public void LoadTaxItemsForInterest()
        {
            if (iclbTaxItems == null)
                iclbTaxItems = new Collection<busPaymentItemType>();
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbPaymentItemType == null)
                ibusPayeeAccount.LoadPaymentItemType();
            foreach (busPaymentItemType lobjPaymentItemType in ibusPayeeAccount.iclbPaymentItemType)
            {
                if ((lobjPaymentItemType.icdoPaymentItemType.retro_payment_type_value != null) &&
                    (lobjPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_No))
                {
                    iclbTaxItems.Add(lobjPaymentItemType);
                }
            }
        }
        private bool _fed_tax_Identifier_no_tax;

        public bool fed_tax_Identifier_no_tax
        {
            get { return _fed_tax_Identifier_no_tax; }
            set { _fed_tax_Identifier_no_tax = value; }
        }
        private bool _state_tax_identifier_no_tax;

        public bool state_tax_identifier_no_tax
        {
            get { return _state_tax_identifier_no_tax; }
            set { _state_tax_identifier_no_tax = value; }
        }
        public void CheckTaxCalcutationRequired()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == null)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (ibusPayeeAccount.iclbPayeeAccountFedTaxWithHolding == null)
                ibusPayeeAccount.LoadFedTaxWithHoldingInfo();
            if (ibusPayeeAccount.iclbPayeeAccountStateTaxWithHolding == null)
                ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
            state_tax_identifier_no_tax = false;
            fed_tax_Identifier_no_tax = false;
            if (ibusPayeeAccount.iclbPayeeAccountStateTaxWithHolding.Count > 0)
            {
                foreach (busPayeeAccountTaxWithholding lobjTaxWithHoldingInfo in ibusPayeeAccount.iclbPayeeAccountStateTaxWithHolding)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(ibusPayeeAccount.idtNextBenefitPaymentDate, lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.start_date,
                        lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.end_date))
                    {
                        if ((lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoFederalTaxWithheld) ||
                            (lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoFedTax) ||
                            (lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoStateTax) ||
                            (lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoStateTaxWithheld))
                        {
                            state_tax_identifier_no_tax = true;
                        }
                    }
                }
            }
            else
            {
                state_tax_identifier_no_tax = true;
            }
          
            if (ibusPayeeAccount.iclbPayeeAccountFedTaxWithHolding.Count > 0)
            {
                foreach (busPayeeAccountTaxWithholding lobjTaxWithHoldingInfo in ibusPayeeAccount.iclbPayeeAccountFedTaxWithHolding)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(ibusPayeeAccount.idtNextBenefitPaymentDate,
                        lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.start_date,
                        lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.end_date))
                    {
                        if ((lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoFederalTaxWithheld) ||
                            (lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoFedTax) ||
                            (lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoStateTax) ||
                            (lobjTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoStateTaxWithheld))
                        {
                            fed_tax_Identifier_no_tax = true;
                        }
                    }
                }
            }
            else
            {
                fed_tax_Identifier_no_tax = true;
            }
        }
    }
}