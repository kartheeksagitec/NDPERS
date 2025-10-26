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
    public class busPayeeAccountDeductionRefund : busPayeeAccountDeductionRefundGen
    {
        //Start date should be equal to or less than next benefit payment date and it should be first of month
        //else throw an error
        public bool IsStartdateNotValid()
        {
            if ((icdoPayeeAccountDeductionRefund.start_date != DateTime.MinValue) && (!AreFieldsReadOnly()))
            {
                if (ibusPayeeAccount == null)
                    LoadibusPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                DateTime ldtStartDate = new DateTime(ibusPayeeAccount.idtNextBenefitPaymentDate.Year, ibusPayeeAccount.idtNextBenefitPaymentDate.Month, 1);
                if (icdoPayeeAccountDeductionRefund.start_date < ldtStartDate)
                {
                    return true;
                }
                else if (icdoPayeeAccountDeductionRefund.start_date >= ldtStartDate)
                {
                    if (icdoPayeeAccountDeductionRefund.start_date.Day != 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //If  End Date is less than or equal to next benefit payment date or one day before next benefit payment date ,then throw an error 
        public bool IsEnddateNotValid()
        {
            if ((icdoPayeeAccountDeductionRefund.start_date != DateTime.MinValue) && (icdoPayeeAccountDeductionRefund.end_date != DateTime.MinValue))
            {
                if (ibusPayeeAccount == null)
                    LoadibusPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                DateTime ldtBatchDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
                if (icdoPayeeAccountDeductionRefund.end_date < ldtBatchDate.AddDays(-1))
                {
                    return true;
                }
                else if (icdoPayeeAccountDeductionRefund.end_date >= ldtBatchDate.AddDays(-1))
                {
                    DateTime ldtLastDateOfMonth = busGlobalFunctions.GetLastDayOfMonth(icdoPayeeAccountDeductionRefund.end_date);
                    if (ldtLastDateOfMonth.Day != icdoPayeeAccountDeductionRefund.end_date.Day)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //If Start Date is less than  next benefit payment date or not first of month,then throw an error
        public bool AreFieldsReadOnly()
        {
            if (ibusPayeeAccount == null)
                LoadibusPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if ((icdoPayeeAccountDeductionRefund.ienuObjectState != ObjectState.Insert) && (icdoPayeeAccountDeductionRefund.start_date != DateTime.MinValue)
                && (icdoPayeeAccountDeductionRefund.start_date < ibusPayeeAccount.idtNextBenefitPaymentDate))
            {
                return true;
            }
            return false;
        }
        //if the deduction refund detail start date is less than or equal to next benefit payment date,then dont allow to delete.else delete all
        public override int Delete()
        {
            if (!AreFieldsReadOnly())
            {
                if (iclbPayeeAccountDeductionRefundItem == null)
                    LoadPayeeAccountDeductionRefundItems();
                if (iclbPayeeAccountPaymentDeductionRefundItems == null)
                    LoadPayeeAccountPaymentDeductionRefundItems();
                //iclbPayeeAccountDeductionRefundItem.Select(o => o.icdoPayeeAccountDeductionRefundItem.Delete());
                //iclbPayeeAccountPaymentDeductionRefundItems.Select(o => o.icdoPayeeAccountPaymentItemType.Delete());
                return base.Delete();
            }
            return 0;
        }
        public override void BeforePersistChanges()
        {
            if (icdoPayeeAccountDeductionRefund.ienuObjectState == ObjectState.Insert)
            {
                icdoPayeeAccountDeductionRefund.Insert();
            }
            //Set the Object State for the Collection based on the Mode
            foreach (busPayeeAccountDeductionRefundItem lobjDeductionRefundItem in iclbPayeeAccountDeductionRefundItem)
            {
                lobjDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.payee_account_deduction_refund_id = icdoPayeeAccountDeductionRefund.payee_account_deduction_refund_id;
                if (lobjDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.payee_account_deduction_refund_item_id == 0)
                {
                    lobjDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.payee_account_deduction_refund_item_id = 1;
                    lobjDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.Insert();
                }
                else if (lobjDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.payee_account_deduction_refund_id > 0 && lobjDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.payee_account_deduction_refund_item_id > 0)
                    lobjDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.Update();
            }
            base.BeforePersistChanges();
        }
        // Post All the deduction details records into payeeaccount payment item type table
        // so that deduction will be included in the next payment 

        public ArrayList btnApprove_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = null;
            if (ibusPayeeAccount == null)
                LoadibusPayeeAccount();
            if (iclbPayeeAccountDeductionRefundItem == null)
                LoadPayeeAccountDeductionRefundItems();
            //If the Deduction are approved dont allow them to modify
            if (!AreFieldsReadOnly())
            {
                foreach (object lobjtemp in iarrChangeLog)
                {
                    if (lobjtemp is cdoPayeeAccountDeductionRefundItem)
                    {
                        lobjError = AddError(6410, "");
                        larrList.Add(lobjError);
                        return larrList;
                    }
                }
            }
            //On approval,post deduction details into payee account payment item type table.            
            foreach (busPayeeAccountDeductionRefundItem lobjPayeeAccountDeductionRefundItem in iclbPayeeAccountDeductionRefundItem)
            {
                int lintPayeeAccountPaymentItemID = 0;
                //Post the deduction items ,then update Deduction Refund Item table with payee account payment item type id   
                if (lobjPayeeAccountDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.payee_account_payment_item_type_id == 0)
                {
                    lintPayeeAccountPaymentItemID = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lobjPayeeAccountDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.payment_item_type_id,
                         lobjPayeeAccountDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.amount, string.Empty, 0,
                         icdoPayeeAccountDeductionRefund.start_date, icdoPayeeAccountDeductionRefund.end_date, true);
                    if (lintPayeeAccountPaymentItemID > 0)
                    {
                         
                        lobjPayeeAccountDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.payee_account_payment_item_type_id = lintPayeeAccountPaymentItemID;
                        lobjPayeeAccountDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.Update();
                    }
                }
                else//if the items are already posted and modified ,then update the payee account payment item type table.       
                {
                    lintPayeeAccountPaymentItemID = ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lobjPayeeAccountDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.payment_item_type_id,
                        lobjPayeeAccountDeductionRefundItem.icdoPayeeAccountDeductionRefundItem.amount, string.Empty, 0,
                        icdoPayeeAccountDeductionRefund.start_date, icdoPayeeAccountDeductionRefund.end_date, false);
                }
              
            }           
            icdoPayeeAccountDeductionRefund.Update();
            this.EvaluateInitialLoadRules();
            larrList.Add(this);
            return larrList;
        }
    }
}