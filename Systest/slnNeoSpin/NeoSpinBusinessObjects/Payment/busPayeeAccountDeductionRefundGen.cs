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
using System.Linq;
using System.Linq.Expressions;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPayeeAccountDeductionRefundGen : busExtendBase
    {
        public busPayeeAccountDeductionRefundGen()
        {

        }

        public cdoPayeeAccountDeductionRefund icdoPayeeAccountDeductionRefund { get; set; }
        //Property to load the payee account
        public busPayeeAccount ibusPayeeAccount { get; set; }
        //Property to load Deduction Refund Items 
        public Collection<busPayeeAccountDeductionRefundItem> iclbPayeeAccountDeductionRefundItem { get; set; }

        //Property to load Payee Account Payment Item - Deduction Refund Items 
        public Collection<busPayeeAccountPaymentItemType> iclbPayeeAccountPaymentDeductionRefundItems { get; set; }
        //Load the Payee account payment iten which are related to the deduction refund items
        public void LoadPayeeAccountPaymentDeductionRefundItems()
        {
            iclbPayeeAccountPaymentDeductionRefundItems = new Collection<busPayeeAccountPaymentItemType>();
            if (ibusPayeeAccount == null)
                LoadibusPayeeAccount();
            if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            if (iclbPayeeAccountDeductionRefundItem == null)
                LoadPayeeAccountDeductionRefundItems();
            foreach (busPayeeAccountPaymentItemType lobjpapit in ibusPayeeAccount.iclbPayeeAccountPaymentItemType)
            {
                foreach (busPayeeAccountDeductionRefundItem lobjdeductionitem in iclbPayeeAccountDeductionRefundItem)
                {
                    if (lobjdeductionitem.icdoPayeeAccountDeductionRefundItem.payment_item_type_id == lobjpapit.icdoPayeeAccountPaymentItemType.payment_item_type_id)
                    {
                        iclbPayeeAccountPaymentDeductionRefundItems.Add(lobjpapit);
                    }
                }
            }
        }
        //Find the  Payee Account Deduction Refund
        public virtual bool FindPayeeAccountDeductionRefund(int Aintpayeeaccountdeductionrefundid)
        {
            bool lblnResult = false;
            if (icdoPayeeAccountDeductionRefund == null)
            {
                icdoPayeeAccountDeductionRefund = new cdoPayeeAccountDeductionRefund();
            }
            if (icdoPayeeAccountDeductionRefund.SelectRow(new object[1] { Aintpayeeaccountdeductionrefundid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        //Find the  Payee Account Deduction Refund
        public virtual bool FindPayeeAccountDeductionRefundByPayeeAccountPaymentItemID(int AintPayeeAccountPaymentItemID)
        {
            bool lblnResult = false;
            if (icdoPayeeAccountDeductionRefund == null)
            {
                icdoPayeeAccountDeductionRefund = new cdoPayeeAccountDeductionRefund();
            }
            DataTable ldtbList = Select<cdoPayeeAccountDeductionRefund>(
                                    new string[1] { "payee_account_payment_item_type_id" },
                                    new object[1] { AintPayeeAccountPaymentItemID }, null, null);
            if (ldtbList.Rows.Count>0)
            {
                icdoPayeeAccountDeductionRefund.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
        //Load  Payee Account
        public virtual void LoadibusPayeeAccount()
        {
            if (ibusPayeeAccount == null)
            {
                ibusPayeeAccount = new busPayeeAccount();
            }
            ibusPayeeAccount.FindPayeeAccount(icdoPayeeAccountDeductionRefund.payee_account_id);
        }
        //Load Deduction Refund Items 
        public virtual void LoadPayeeAccountDeductionRefundItems()
        {
            iclbPayeeAccountDeductionRefundItem = new Collection<busPayeeAccountDeductionRefundItem>();
            DataTable ldtbDeductionRefundItemDetail = Select("cdoPayeeAccountDeductionRefund.LoadDeductionRefundItems", new object[0] { });

            foreach (DataRow drStep in ldtbDeductionRefundItemDetail.Rows)
            {
                busPayeeAccountDeductionRefundItem lobjDeductionRefundItem = new busPayeeAccountDeductionRefundItem { icdoPayeeAccountDeductionRefundItem = new cdoPayeeAccountDeductionRefundItem() };
                lobjDeductionRefundItem.ibusPaymentItemType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
                lobjDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.LoadData(drStep);
                lobjDeductionRefundItem.ibusPaymentItemType.icdoPaymentItemType.LoadData(drStep);
                iclbPayeeAccountDeductionRefundItem.Add(lobjDeductionRefundItem);
            }
        }

        //Property to Get the Taxable and non taxable deduction refund amount
        public decimal idecDeductionTotalAmount { get; set; }
        public decimal idecDeductionTaxableAmount { get; set; }
        public decimal idecDeductionNonTaxableAmount { get; set; }

        //Get the Taxable and non taxable deduction refund amount
        public void LoadDeductionTaxableAndNonTaxableAmount()
        {
            if (iclbPayeeAccountDeductionRefundItem == null)
                LoadPayeeAccountDeductionRefundItems();

            idecDeductionTaxableAmount = iclbPayeeAccountDeductionRefundItem.Where(o => o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes).Select(o => o.icdoPayeeAccountDeductionRefundItem.amount).Sum();
            idecDeductionNonTaxableAmount = iclbPayeeAccountDeductionRefundItem.Where(o => o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_No).Select(o => o.icdoPayeeAccountDeductionRefundItem.amount).Sum();
            idecDeductionTotalAmount = idecDeductionTaxableAmount + idecDeductionNonTaxableAmount;
        }
    }
}