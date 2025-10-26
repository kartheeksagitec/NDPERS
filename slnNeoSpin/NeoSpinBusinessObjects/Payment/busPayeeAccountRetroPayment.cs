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
using System.Collections.Generic;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPayeeAccountRetroPayment : busPayeeAccountRetroPaymentGen
    {
        public int iintRetroPaymentMonths
        {
            get
            {
                return busGlobalFunctions.DateDiffByMonth(icdoPayeeAccountRetroPayment.effective_start_date, icdoPayeeAccountRetroPayment.effective_end_date);
            }
        }
        public string istrNoStateTax { get; set; }
        public bool IsNewMode = false;
		//Backlog PIR 10966
        public bool iblnIsInitialRetroCreation { get; set; }
        private decimal InitialGrossAmount = 0.00M;
        public bool iblnIsReacivation { get; set; }
        public bool iblnSpecialCheckIncludeInAdhocFlag { get; set; }
        public string istrSupressWarningsFlag { get; set; }
        //If Start Date is less than  next benefit payment date or not first of month,then throw an error
        public bool IsStartdateNotValid()
        {
            if (icdoPayeeAccountRetroPayment.start_date != DateTime.MinValue)
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                DateTime ldtStartDate = new DateTime(ibusPayeeAccount.idtNextBenefitPaymentDate.Year, ibusPayeeAccount.idtNextBenefitPaymentDate.Month, 1);
                if (icdoPayeeAccountRetroPayment.start_date < ldtStartDate)
                {
                    return true;
                }
                else if (icdoPayeeAccountRetroPayment.start_date >= ldtStartDate)
                {
                    if (icdoPayeeAccountRetroPayment.start_date.Day != 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //Effective Start and End date is required for Retro payment type Pop up Benefits-BR-79-94
        public string IsEffectiveStartDateAndEndDateRequired()
        {
            string lstrReqInd = string.Empty;
            if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypePopupBenefits)
            {
                if (icdoPayeeAccountRetroPayment.effective_start_date == DateTime.MinValue || icdoPayeeAccountRetroPayment.effective_end_date == DateTime.MinValue)
                {
                    return "REQ";
                }
            }
            return lstrReqInd;
        }
        //If payee account relation is not memeber or joint annuitant person not deseased,do not allow for pop up benefits
        public bool IsPopupBenefitsNotAllowed()
        {
            if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypePopupBenefits)
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.ibusApplication == null)
                    ibusPayeeAccount.LoadApplication();
                if (ibusPayeeAccount.ibusApplication.ibusJointAnniutantPerson == null)
                    ibusPayeeAccount.ibusApplication.LoadJointAnniutantPerson();
                if (ibusPayeeAccount.icdoPayeeAccount.account_relation_value != busConstant.AccountRelationshipMember)
                {
                    return true;
                }
                else if (ibusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember &&
                    ibusPayeeAccount.ibusApplication.ibusJointAnniutantPerson.icdoPerson.person_id > 0 &&
                    !(ibusPayeeAccount.ibusApplication.ibusJointAnniutantPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced ||
                        ibusPayeeAccount.ibusApplication.ibusJointAnniutantPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle ||
                        ibusPayeeAccount.ibusPayee.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced ||
                        ibusPayeeAccount.ibusPayee.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle
                    ) &&
                    ibusPayeeAccount.ibusApplication.ibusJointAnniutantPerson.icdoPerson.date_of_death == DateTime.MinValue)
                {
                    return true;
                }
            }
            return false;
        }
        //If  End Date is less than or equal to next benefit payment date or one day before next benefit payment date ,then throw an error 
        public bool IsEnddateNotValid()
        {
            if ((icdoPayeeAccountRetroPayment.start_date != DateTime.MinValue) && (icdoPayeeAccountRetroPayment.end_date != DateTime.MinValue))
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                DateTime ldtBatchDate = ibusPayeeAccount.idtNextBenefitPaymentDate;
                if (icdoPayeeAccountRetroPayment.end_date < ldtBatchDate.AddDays(-1))
                {
                    return true;
                }
                else if (icdoPayeeAccountRetroPayment.end_date >= ldtBatchDate.AddDays(-1))
                {
                    DateTime ldtLastDateOfMonth = busGlobalFunctions.GetLastDayOfMonth(icdoPayeeAccountRetroPayment.end_date);
                    if (ldtLastDateOfMonth.Day != icdoPayeeAccountRetroPayment.end_date.Day)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public override int Delete()
        {
            //On deletion of  Details change the payee account status to review
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            ibusPayeeAccount.CreateReviewPayeeAccountStatus();
            if (ibusPayeeAccount.ibusSoftErrors == null)
                ibusPayeeAccount.LoadErrors();
            ibusPayeeAccount.iblnClearSoftErrors = false;
            ibusPayeeAccount.ibusSoftErrors.iblnClearError = false;
            ibusPayeeAccount.iblnAdjustmentInfoDeleteIndicator = true;
            ibusPayeeAccount.ValidateSoftErrors();
            ibusPayeeAccount.UpdateValidateStatus();
            DeleteRetroPaymentDetails();
            DeleteMonthWiseAdjustmentDetails();
            return base.Delete();
        }
        public bool IsRecordExistForSamePeriod()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbRetroPayment == null)
                ibusPayeeAccount.LoadRetroPayment();
            foreach (busPayeeAccountRetroPayment lobjRetroPayment in ibusPayeeAccount.iclbRetroPayment)
            {
                if (lobjRetroPayment.icdoPayeeAccountRetroPayment.payee_account_retro_payment_id != icdoPayeeAccountRetroPayment.payee_account_retro_payment_id)
                {
                    if ((icdoPayeeAccountRetroPayment.retro_payment_type_value == lobjRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_value) &&
                        (busGlobalFunctions.CheckDateOverlapping(icdoPayeeAccountRetroPayment.start_date,
                        lobjRetroPayment.icdoPayeeAccountRetroPayment.start_date, lobjRetroPayment.icdoPayeeAccountRetroPayment.end_date)))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /* This method fills the Retro payment details collection in the new mode.
          1.System calculates start date and end date for retro payment calculation
          2.System Calls the  Method in busPayeeAccountCalculateRetroPaymentItems(startdate,enddate,retropaymenttype).
          3.Method returns Retro payment items Collection with the retro amount for the payee account
          4.Create the Retro payment details from the above returned collection*/
        public void CreateRetroPaymentDetails()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbRetroItemType == null)
                ibusPayeeAccount.LoadRetroItemType(icdoPayeeAccountRetroPayment.retro_payment_type_value);
            if (ibusPayeeAccount.iclbPaymentItemType == null)
                ibusPayeeAccount.LoadPaymentItemType();
            DeleteRetroPaymentDetails();
            DeleteMonthWiseAdjustmentDetails();
            if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            int lintNoOfPayments = busGlobalFunctions.DateDiffByMonth(icdoPayeeAccountRetroPayment.start_date, icdoPayeeAccountRetroPayment.end_date);
            if (lintNoOfPayments > 0)
            {
                Collection<busPayeeAccountPaymentItemType> lclbPayeeAcccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();
                if (icdoPayeeAccountRetroPayment.retro_payment_type_value != busConstant.RetroPaymentTypeInitial)
                {
                    lclbPayeeAcccountPaymentItemType = ibusPayeeAccount.CalculateRetroPaymentItemsForUnderpayment(icdoPayeeAccountRetroPayment.effective_start_date,
                                                       icdoPayeeAccountRetroPayment.effective_end_date, icdoPayeeAccountRetroPayment.retro_payment_type_value);
                }
                else
                {
                    lclbPayeeAcccountPaymentItemType = ibusPayeeAccount.CalculateRetroPaymentItemsForInitial(icdoPayeeAccountRetroPayment.effective_start_date,
                                                     icdoPayeeAccountRetroPayment.effective_end_date, icdoPayeeAccountRetroPayment.retro_payment_type_value);
                }

                var lenmItemWiseHistoryDetail = from lobjPAPIT in lclbPayeeAcccountPaymentItemType
                                                group lobjPAPIT by new { lobjPAPIT.icdoPayeeAccountPaymentItemType.payment_item_type_id }
                                                    into ItemWiseHistoryDetail
                                                    select new
                                                    {
                                                        lintPaymentItemTypeID = ItemWiseHistoryDetail.Key.payment_item_type_id,
                                                        ldecAmount = ItemWiseHistoryDetail.Sum(o =>
                                                            o.icdoPayeeAccountPaymentItemType.amount)
                                                    };
                foreach (var lobjPayeeAccountPaymentItemType in lenmItemWiseHistoryDetail)
                {
                    if (lobjPayeeAccountPaymentItemType.ldecAmount > 0.0m)
                    {
                        busPaymentItemType lobjRetroItem = ibusPayeeAccount.iclbPaymentItemType.Where(o =>
                            o.icdoPaymentItemType.payment_item_type_id == lobjPayeeAccountPaymentItemType.lintPaymentItemTypeID).FirstOrDefault();

                        busRetroItemType lobjRetroRef = ibusPayeeAccount.iclbRetroItemType.Where(o =>
                            o.icdoRetroItemType.to_item_type == lobjRetroItem.icdoPaymentItemType.item_type_code).FirstOrDefault();

                        busPaymentItemType lobjOriginalItemType = ibusPayeeAccount.iclbPaymentItemType.Where(o =>
                            o.icdoPaymentItemType.item_type_code == lobjRetroRef.icdoRetroItemType.from_item_type).FirstOrDefault();

                        int lintVendorID = ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(o =>
                            o.icdoPayeeAccountPaymentItemType.payment_item_type_id == lobjOriginalItemType.icdoPaymentItemType.payment_item_type_id).Select(o =>
                                o.icdoPayeeAccountPaymentItemType.vendor_org_id).FirstOrDefault();

                        InsertIntoRetroPaymentDetail(lintNoOfPayments, lobjPayeeAccountPaymentItemType.ldecAmount,
                            lobjPayeeAccountPaymentItemType.lintPaymentItemTypeID, lobjOriginalItemType.icdoPaymentItemType.payment_item_type_id, lintVendorID);
                    }
                }
                if (ibusPayeeAccount.iclbMonthwiseAdjustmentDetail != null && ibusPayeeAccount.iclbMonthwiseAdjustmentDetail.Count > 0)
                {
                    decimal ldecInterstPercentage = Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(52,
                        busConstant.RetroPaymentInterestPercentage, iobjPassInfo));
                    decimal ldecIntrRate = ((ldecInterstPercentage / 100) / 12M);
                    foreach (busPayeeAccountMonthwiseAdjustmentDetail lobjAdjustmentDetail in ibusPayeeAccount.iclbMonthwiseAdjustmentDetail)
                    {

                        lobjAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.payee_account_retro_payment_id
                            = icdoPayeeAccountRetroPayment.payee_account_retro_payment_id;
                        if (icdoPayeeAccountRetroPayment.calculate_interest_flag == busConstant.Flag_Yes)
                        {
                            int lintTotalDueMonths = busEmployerReportHelper.GetTotalDueMonths(ibusPayeeAccount.idtNextBenefitPaymentDate.AddDays(-1),
                                lobjAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date);

                            lobjAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount
                                = Convert.ToDecimal((Convert.ToDouble(lobjAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.amount) *
                               Math.Pow(1 + Convert.ToDouble(ldecIntrRate), Convert.ToDouble(lintTotalDueMonths))) -
                              Convert.ToDouble(lobjAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.amount));
                        }
                        lobjAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.payee_account_id = icdoPayeeAccountRetroPayment.payee_account_id;
                        lobjAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.Insert();
                    }
                }
            }
        }
        //insert records  into retor payment detail table
        public void InsertIntoRetroPaymentDetail(int aintNoOfPayments, decimal adecAmount, int aintPaymentItemTypeID, int aintOriginalPaymentItemTypeID, int aintVendorOrgID)
        {
            busPayeeAccountRetroPaymentDetail lobjRetroPaymentDetail = new busPayeeAccountRetroPaymentDetail
            {
                icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail()
            };
            lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = aintPaymentItemTypeID;
            lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = aintOriginalPaymentItemTypeID;
            lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_id = icdoPayeeAccountRetroPayment.payee_account_retro_payment_id;
            lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.vendor_org_id = aintVendorOrgID;
            lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount = adecAmount / aintNoOfPayments;
            lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Insert();
        }
        public void DeleteRetroPaymentDetails()
        {
            if (iclbPayeeAccountRetroPaymentDetail != null)
            {
                foreach (busPayeeAccountRetroPaymentDetail lobjRetroPAymentDetail in iclbPayeeAccountRetroPaymentDetail)
                {
                    lobjRetroPAymentDetail.icdoPayeeAccountRetroPaymentDetail.Delete();
                }
            }
        }
		
		//Backlog PIR 10966
        public void DeleteRetroPaymentDetailsFederalAndStateTax()
        {
            if (iclbPayeeAccountRetroPaymentDetail != null)
            {
                foreach (busPayeeAccountRetroPaymentDetail lobjRetroPAymentDetail in iclbPayeeAccountRetroPaymentDetail)
                {
                    if (lobjRetroPAymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRetroFedTaxAmount
                        || lobjRetroPAymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRetroStateTaxAmount
                        || lobjRetroPAymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRetroFedTaxAmountSpecial
                        || lobjRetroPAymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRetroStateTaxAmountSpecial)
                        lobjRetroPAymentDetail.icdoPayeeAccountRetroPaymentDetail.Delete();
                }
            }
        }
        //Delete Monthwise details when recalculating
        public void DeleteMonthWiseAdjustmentDetails()
        {

            LoadRetrMonthWiseAdjustMentDetail();
            if (iclbRetrMonthWiseAdjustMentDetail != null)
            {
                foreach (busPayeeAccountMonthwiseAdjustmentDetail lbusMonthwiseAdjustmentDetail in iclbRetrMonthWiseAdjustMentDetail)
                {
                    lbusMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.Delete();
                }
            }
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (aenmPageMode == utlPageMode.New)
            {
                IsNewMode = true;
            }
            base.BeforeValidate(aenmPageMode);
        }
        //Retro Payment Type Inital,Reactivation and Adjustments,details records cannot be entered.It should be calculated
        public bool IsModifyingDetailRecordInValid()
        {
            if (icdoPayeeAccountRetroPayment.retro_payment_type_value != busConstant.RetroPaymentTypeRHICBenefitReimbursement)
            {
                foreach (object lobjtemp in iarrChangeLog)
                {
                    if (lobjtemp is cdoPayeeAccountRetroPaymentDetail)
                    {
                        if ((((cdoPayeeAccountRetroPaymentDetail)lobjtemp).ienuObjectState) == ObjectState.Update)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        //if RHIC Reimbursement Amount is Zero ,do not allow to approve
        public bool IsRHICReimbursementAmountZero()
        {
            if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeRHICBenefitReimbursement)
            {
                if (iclbPayeeAccountRetroPaymentDetail == null)
                    LoadPayeeAccountRetroPaymentDetail();
                foreach (busPayeeAccountRetroPaymentDetail lobjRetro in iclbPayeeAccountRetroPaymentDetail)
                {
                    if (lobjRetro.icdoPayeeAccountRetroPaymentDetail.amount <= 0.0m)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Post All the Retro payment details records into payeeaccount payment item type table
        // so that retro payments will be included in the next payment 

        public ArrayList btnApprove_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = null;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (iclbPayeeAccountRetroPaymentDetail == null)
                LoadPayeeAccountRetroPaymentDetail();
            foreach (object lobjtemp in iarrChangeLog)
            {
                if (lobjtemp is cdoPayeeAccountRetroPaymentDetail)
                {
                    if ((((cdoPayeeAccountRetroPaymentDetail)lobjtemp).ienuObjectState) == ObjectState.Update)
                    {
                        lobjError = AddError(5667, "");
                        larrList.Add(lobjError);
                        return larrList;
                    }
                }
            }
            //uat pir - 1369
            if (string.IsNullOrEmpty(icdoPayeeAccountRetroPayment.payment_option_value) || icdoPayeeAccountRetroPayment.payment_option_value == " ")
            {
                lobjError = AddError(5651, "");
                larrList.Add(lobjError);
                return larrList;
            }
            if (IsRHICReimbursementAmountZero())
            {
                lobjError = AddError(7565, "");
                larrList.Add(lobjError);
                return larrList;
            }
            if (DoesPayeeAccountHasPriorPopup())
            {
                lobjError = AddError(10407, "");
                larrList.Add(lobjError);
                return larrList;
            }
            if (ibusPayeeAccount.iclbRetroItemType == null)
                ibusPayeeAccount.LoadRetroItemType(icdoPayeeAccountRetroPayment.retro_payment_type_value);
            if (ibusPayeeAccount.iclbPaymentItemType == null)
                ibusPayeeAccount.LoadPaymentItemType();
            //Pop up benefits
            if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypePopupBenefits)
            {
                CalculatePAPITAmounts();
                UpdateBenefitOpitonAndRHICBenefitOption();
            }
            else
            {
                if (icdoPayeeAccountRetroPayment.payment_option_value == busConstant.PayeeAccountRetroPaymentOptionSpecial)
                {
                    foreach (busPayeeAccountRetroPaymentDetail lobjPayeeAccountRetroPaymentDetail in iclbPayeeAccountRetroPaymentDetail)
                    {
                        if (lobjPayeeAccountRetroPaymentDetail.ibusOriginalPaymentItemType == null)
                            lobjPayeeAccountRetroPaymentDetail.LoadOriginalPaymentItemType();

                        busRetroItemType lobjRetroItemType = ibusPayeeAccount.iclbRetroItemType.Where(o =>
                            o.icdoRetroItemType.from_item_type == lobjPayeeAccountRetroPaymentDetail.ibusOriginalPaymentItemType.icdoPaymentItemType.item_type_code &&
                            o.icdoRetroItemType.payment_option_value == icdoPayeeAccountRetroPayment.payment_option_value).FirstOrDefault();
                        if (lobjRetroItemType != null)
                        {

                            busPaymentItemType lobjPaymentItemType = ibusPayeeAccount.iclbPaymentItemType.Where(o =>
                                o.icdoPaymentItemType.item_type_code == lobjRetroItemType.icdoRetroItemType.to_item_type).FirstOrDefault();
                            if (lobjPaymentItemType != null)
                            {
                                ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id,
                                    lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount, string.Empty, lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.vendor_org_id,
                                    icdoPayeeAccountRetroPayment.start_date, icdoPayeeAccountRetroPayment.end_date, false);
                                lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id =
                            lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id;
                                lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Update();
                            }
                        }
                    }
                }
                else
                {
                    foreach (busPayeeAccountRetroPaymentDetail lobjPayeeAccountRetroPaymentDetail in iclbPayeeAccountRetroPaymentDetail)
                    {
                        ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id,
                            lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount, string.Empty, lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.vendor_org_id,
                            icdoPayeeAccountRetroPayment.start_date, icdoPayeeAccountRetroPayment.end_date, false);
                    }
                }
                if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeRHICBenefitReimbursement)
                {
                    CreateRHICHealthSplit();
                }
                UpdateInterestFlagForUnderpayment();
            }
            icdoPayeeAccountRetroPayment.approved_flag = busConstant.Flag_Yes;
            //dont remove this update from here, as value set by above function UpdateInterestFlagForUnderpayment() is updated by below command
            icdoPayeeAccountRetroPayment.Update();
            if (icdoPayeeAccountRetroPayment.retro_payment_type_value != busConstant.RetroPaymentTypeInitial)
            {
                ibusPayeeAccount.CalculateAdjustmentTax(false);
            }
            this.EvaluateInitialLoadRules();
            larrList.Add(this);
            return larrList;
        }

        private void UpdateInterestFlagForUnderpayment()
        {
            if (icdoPayeeAccountRetroPayment.is_online_flag == busConstant.Flag_Yes && icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeBenefitUnderpayment)
            {
                foreach (busPayeeAccountRetroPaymentDetail lobjDetail in iclbPayeeAccountRetroPaymentDetail)
                {
                    lobjDetail.LoadOriginalPaymentItemType();
                    if (lobjDetail.ibusOriginalPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PaymentItemTypeCodeITEM51 ||
                        lobjDetail.ibusOriginalPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ItemTypeFedTaxOnInterestForAdjustments ||
                        lobjDetail.ibusOriginalPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.ItemTypeStateTaxOnInterestForAdjustments)
                    {
                        icdoPayeeAccountRetroPayment.calculate_interest_flag = busConstant.Flag_Yes;
                        break;
                    }
                }
            }

        }

        public override void BeforePersistChanges()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            //If the records in the RetroPayment detail collection are changed , then update
            foreach (object lobjtemp in iarrChangeLog)
            {
                if (lobjtemp is cdoPayeeAccountRetroPaymentDetail)
                {
                    if ((((cdoPayeeAccountRetroPaymentDetail)lobjtemp).ienuObjectState) == ObjectState.Update)
                    {
                        busPayeeAccountRetroPaymentDetail lobjPayeeAccountRetroPaymentDetail = new busPayeeAccountRetroPaymentDetail();
                        lobjPayeeAccountRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail = (cdoPayeeAccountRetroPaymentDetail)lobjtemp;
                    }
                }
            }

            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();
            //if retro payment type is reactivation,Start date is ‘effective date when Payee Status was changed to SUSPENDED’ 
            //and Effective End Date is next ‘Benefit Payment Date’
            //UAT PIR 890 - Retro payment should include Next payment month for calculation
            DateTime ldtefirstDateOfNextPaymentDate = new DateTime(ibusPayeeAccount.idtNextBenefitPaymentDate.Year, ibusPayeeAccount.idtNextBenefitPaymentDate.Month, 1);
            DateTime ldteLastDateMonthBeforeNextPaymentDate = ldtefirstDateOfNextPaymentDate.AddDays(-1);

            if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeReactivation &&
                icdoPayeeAccountRetroPayment.is_online_flag != busConstant.Flag_Yes)
            {
                if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                    ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                if (ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountRolloverDetailStatusSuspended)
                    icdoPayeeAccountRetroPayment.effective_start_date = ibusPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_effective_date;
                else if (iblnIsReacivation)
                {
                    icdoPayeeAccountRetroPayment.effective_start_date = icdoPayeeAccountRetroPayment.reactivation_date;
                }
                else
                {
                    if (ibusPayeeAccount == null)
                        LoadPayeeAccount();
                    if (ibusPayeeAccount.iclbPayeeAccountStatus == null)
                        ibusPayeeAccount.LoadPayeeAccountStatus();
                    if (ibusPayeeAccount.iclbPayeeAccountStatus.Count > 0)
                    {
                        icdoPayeeAccountRetroPayment.effective_start_date = ibusPayeeAccount.iclbPayeeAccountStatus.Where(o =>
                            o.IsStatusSuspended()).Max(o => o.icdoPayeeAccountStatus.status_effective_date);
                    }
                }
                icdoPayeeAccountRetroPayment.effective_end_date = ldteLastDateMonthBeforeNextPaymentDate;
            }
            else if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeRHICBenefitReimbursement)
            {
                icdoPayeeAccountRetroPayment.effective_start_date = icdoPayeeAccountRetroPayment.start_date;
                icdoPayeeAccountRetroPayment.effective_end_date = icdoPayeeAccountRetroPayment.end_date;
            }
            else if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeInitial)
            {
                icdoPayeeAccountRetroPayment.effective_start_date = ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                icdoPayeeAccountRetroPayment.effective_end_date = ldteLastDateMonthBeforeNextPaymentDate;
            }
            //if retro payment type is initial ,adjustments , Start date is Retirement Date and Effective End Date is next ‘Benefit Payment Date’           
            else if (icdoPayeeAccountRetroPayment.retro_payment_type_value != busConstant.RetroPaymentTypePopupBenefits &&
                icdoPayeeAccountRetroPayment.is_online_flag != busConstant.Flag_Yes)
            {
                if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_application_id == 0)
                {
                    if (ibusPayeeAccount.ibusDROApplication == null)
                        ibusPayeeAccount.LoadDROApplication();
                    icdoPayeeAccountRetroPayment.effective_start_date = ibusPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.benefit_receipt_date;
                }
                else if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                {
                    icdoPayeeAccountRetroPayment.effective_start_date = ibusPayeeAccount.ibusApplication.icdoBenefitApplication.termination_date.AddMonths(1);
                }
                //UAT PIR:2076. effective date set as last of the month following the date of death As set in Pre Retirement Death.
                else if (ibusPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                {
                    if (ibusPayeeAccount.ibusApplication.ibusPerson.IsNull())
                        ibusPayeeAccount.ibusApplication.LoadPerson();
                    if (ibusPayeeAccount.ibusApplication.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                    {
                        DateTime ldteTempDate = ibusPayeeAccount.ibusApplication.ibusPerson.icdoPerson.date_of_death.AddMonths(1);
                        DateTime ldtRetirementDate = new DateTime(ldteTempDate.Year, ldteTempDate.Month, 01);
                        icdoPayeeAccountRetroPayment.effective_start_date = ldtRetirementDate;
                    }
                }
                else
                {
                    icdoPayeeAccountRetroPayment.effective_start_date = ibusPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_date;
                }
                icdoPayeeAccountRetroPayment.effective_end_date = ldteLastDateMonthBeforeNextPaymentDate;
            }

            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();

            if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeRHICBenefitReimbursement)
            {
                if (IsNewMode && icdoPayeeAccountRetroPayment.is_online_flag == busConstant.Flag_Yes)
                {
                    CreateRetroPaymentDetailsForRHICBenefitReimbursement();
                }
            }
            //If the Calculate interest flag is checked,Find Interest for the Gross Amount wit 6% interest rate, Create a Payment Item for this interest amount
            if ((icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeBenefitUnderpayment ||
                icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeInitial ||
                icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeReactivation) &&
                icdoPayeeAccountRetroPayment.is_online_flag != busConstant.Flag_Yes)
            {
                //Create retro payment details for intial,benefit underpayment types
				//Backlog PIR 10966
                if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeInitial)
                {
                    if (iblnIsInitialRetroCreation)
                        CreateRetroPaymentDetails();
                    else
                    {
                        CalculateFedAndStateTaxRetro();
                    }
                }
                else
                {
                    CreateRetroPaymentDetails();
                }
                LoadGrossAndNetRetroPayments();
                if (icdoPayeeAccountRetroPayment.calculate_interest_flag == busConstant.Flag_Yes)
                {
                    CreatePaymentItemForInterestandTax();
                }
            }
            //Reload always
            LoadPayeeAccountRetroPaymentDetail();
            //UCS  -079 : Additional rules as per satya
            //if created online,should not calculate tax as user will be entering it
            if (icdoPayeeAccountRetroPayment.is_online_flag != busConstant.Flag_Yes)
                CalculateFlatTaxForRecalculationRetroAmount();


            LoadGrossAndNetRetroPayments();
            //on addition or modification of Ach Details change the payee account status to review

            if (IsNewMode)
            {
                ibusPayeeAccount.iblnNewUnderPaymentIndicator = true;
                if (!iblnIsReacivation)
                {
                    ibusPayeeAccount.CreateReviewPayeeAccountStatus();
                }
            }
            else
            {
                ibusPayeeAccount.iblnAdjustmentChangeIndicator = true;
                if (!iblnIsReacivation)
                {
                    ibusPayeeAccount.CreateReviewPayeeAccountStatus();
                }
            }
            if (ibusPayeeAccount.ibusSoftErrors == null)
                ibusPayeeAccount.LoadErrors();
            ibusPayeeAccount.iblnClearSoftErrors = false;
            ibusPayeeAccount.ibusSoftErrors.iblnClearError = false;
            ibusPayeeAccount.ValidateSoftErrors();
            ibusPayeeAccount.UpdateValidateStatus();
            IsNewMode = false;
        }

        public override bool SeBpmActivityInstanceReferenceID()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                ibusPayeeAccount.ibusBaseActivityInstance = ibusBaseActivityInstance;
                ibusPayeeAccount.SeBpmActivityInstanceReferenceID();
                //   ibusPayeeAccount.SetProcessInstanceParameters();
                ibusPayeeAccount.SetCaseInstanceParameters();
            }
            return true;
        }

        //Create Retro Payment Details For RHIC BenefitReimbursement
        private void CreateRetroPaymentDetailsForRHICBenefitReimbursement()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();

            if (ibusPayeeAccount.iclbPaymentItemType == null)
                ibusPayeeAccount.LoadPaymentItemType();


            busPaymentItemType lobjPayeeAccountItemType = ibusPayeeAccount.iclbPaymentItemType.Where(o => o.icdoPaymentItemType.item_type_code == "ITEM62").
                                            FirstOrDefault();

            InsertIntoRetroPaymentDetail(1, 0.0m, Convert.ToInt32(lobjPayeeAccountItemType.icdoPaymentItemType.payment_item_type_id),
                lobjPayeeAccountItemType.icdoPaymentItemType.payment_item_type_id, 0);
        }


        //Load Gross and net amounts and update it
        public void LoadGrossAndNetRetroPayments()
        {
            decimal ldecPositiveAmount = 0.00M;
            decimal ldecNegativeAmount = 0.00M;
            //Reload always
            LoadPayeeAccountRetroPaymentDetail();
            foreach (busPayeeAccountRetroPaymentDetail lobjRetroPaymentDetail in iclbPayeeAccountRetroPaymentDetail)
            {
                if (lobjRetroPaymentDetail.ibusPaymentItemType == null)
                    lobjRetroPaymentDetail.LoadPaymentItemType();
                if (lobjRetroPaymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1)
                {
                    ldecPositiveAmount += lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount *
                        lobjRetroPaymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                }
                else
                {
                    ldecNegativeAmount += lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount *
                        lobjRetroPaymentDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                }
            }
            icdoPayeeAccountRetroPayment.Select();
            icdoPayeeAccountRetroPayment.gross_payment_amount = ldecPositiveAmount;
            icdoPayeeAccountRetroPayment.net_payment_amount = ldecPositiveAmount + ldecNegativeAmount;
            icdoPayeeAccountRetroPayment.Update();
        }
        //Calculate interest for the under payment and calculate tax for the interestSS
        private void CreatePaymentItemForInterestandTax()
        {
            decimal ldecInterest = 0.00M;
            bool lblnSpecialFlatTaxIndicator = false;

            LoadRetrMonthWiseAdjustMentDetail();
            iclbRetrMonthWiseAdjustMentDetail = busGlobalFunctions.Sort<busPayeeAccountMonthwiseAdjustmentDetail>
                ("icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date", iclbRetrMonthWiseAdjustMentDetail);
            ldecInterest = iclbRetrMonthWiseAdjustMentDetail.Sum(o => o.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount);

            decimal ldecStateTaxOnInterst = 0.0M, ldecFedTaxOnInterest = 0.0M;
            CheckTaxCalcutationRequired();
            if (ldecInterest > 0.0m)
            {
                if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeInitial)
                {
                    //Create  Payee Account Payment Item for Interest with ITEM Code 45
                    int lintInitialInterestItemTypeID = GetPaymentItemTypeID(busConstant.PaymentItemTypeCodeITEM45, ref lblnSpecialFlatTaxIndicator);
                    CreateRetroPaymentDetailForInterestAndTaxOnInterest(lintInitialInterestItemTypeID, ldecInterest, 0);

                    CalculateFlatTax(lblnSpecialFlatTaxIndicator, ldecInterest, ref ldecStateTaxOnInterst, ref ldecFedTaxOnInterest);

                    if ((ldecFedTaxOnInterest > 0.00M) && (!fed_tax_Identifier_no_tax))
                    {
                        int lintInitialInterestFedTaxItemTypeID = GetPaymentItemTypeID(busConstant.ItemTypeFedTaxOnInterestForInitial, ref lblnSpecialFlatTaxIndicator);
                        CreateRetroPaymentDetailForInterestAndTaxOnInterest(lintInitialInterestFedTaxItemTypeID, ldecFedTaxOnInterest, busPayeeAccountHelper.GetFedTaxVendorID());
                    }
                    if ((ldecStateTaxOnInterst > 0.00M) && (!state_tax_identifier_no_tax))
                    {
                        int lintInitialInterestStateTaxItemTypeID = GetPaymentItemTypeID(busConstant.ItemTypeStateTaxOnInterestForInitial, ref lblnSpecialFlatTaxIndicator);
                        CreateRetroPaymentDetailForInterestAndTaxOnInterest(lintInitialInterestStateTaxItemTypeID, ldecStateTaxOnInterst, busPayeeAccountHelper.GetStateTaxVendorID());
                    }
                }
                else if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeBenefitUnderpayment)
                {
                    //Create  Payee Account Payment Item for Interest with ITEM Code 51  
                    int lintPaymentItemTypeID = GetPaymentItemTypeID(busConstant.PaymentItemTypeCodeITEM51, ref lblnSpecialFlatTaxIndicator);
                    CreateRetroPaymentDetailForInterestAndTaxOnInterest(lintPaymentItemTypeID, ldecInterest, 0);

                    CalculateFlatTax(lblnSpecialFlatTaxIndicator, ldecInterest, ref ldecStateTaxOnInterst, ref ldecFedTaxOnInterest);

                    if ((ldecFedTaxOnInterest > 0.00M) && (!fed_tax_Identifier_no_tax))
                    {
                        int lintAdjustmentInterestFedTaxItemTypeID = GetPaymentItemTypeID(busConstant.ItemTypeFedTaxOnInterestForAdjustments, ref lblnSpecialFlatTaxIndicator);
                        CreateRetroPaymentDetailForInterestAndTaxOnInterest(lintAdjustmentInterestFedTaxItemTypeID, ldecFedTaxOnInterest, busPayeeAccountHelper.GetFedTaxVendorID());
                    }
                    if ((ldecStateTaxOnInterst > 0.00M) && (!state_tax_identifier_no_tax))
                    {
                        int lintAdjustmentInterestStateTaxItemTypeID = GetPaymentItemTypeID(busConstant.ItemTypeStateTaxOnInterestForAdjustments, ref lblnSpecialFlatTaxIndicator);
                        CreateRetroPaymentDetailForInterestAndTaxOnInterest(lintAdjustmentInterestStateTaxItemTypeID, ldecStateTaxOnInterst, busPayeeAccountHelper.GetStateTaxVendorID());
                    }
                }

                else if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeReactivation)
                {
                    //Create Payee Account Payment Item for Interest with ITEM Code 57               
                    int lintPaymentItemTypeID = GetPaymentItemTypeID(busConstant.PaymentItemTypeCodeITEM57, ref lblnSpecialFlatTaxIndicator);

                    CreateRetroPaymentDetailForInterestAndTaxOnInterest(lintPaymentItemTypeID, ldecInterest, 0);

                    CalculateFlatTax(lblnSpecialFlatTaxIndicator, ldecInterest, ref ldecStateTaxOnInterst, ref ldecFedTaxOnInterest);

                    if ((ldecFedTaxOnInterest > 0.00M) && (!fed_tax_Identifier_no_tax))
                    {
                        int lintSuspensionInterestFedTaxItemTypeID = GetPaymentItemTypeID(busConstant.ItemTypeFedTaxOnInterestForSusPension, ref lblnSpecialFlatTaxIndicator);
                        CreateRetroPaymentDetailForInterestAndTaxOnInterest(lintSuspensionInterestFedTaxItemTypeID, ldecFedTaxOnInterest, busPayeeAccountHelper.GetFedTaxVendorID());
                    }
                    if ((ldecStateTaxOnInterst > 0.00M) && (!state_tax_identifier_no_tax))
                    {
                        int lintSuspensionInterestStateTaxItemTypeID = GetPaymentItemTypeID(busConstant.ItemTypeStateTaxOnInterestForSuspension, ref lblnSpecialFlatTaxIndicator);
                        CreateRetroPaymentDetailForInterestAndTaxOnInterest(lintSuspensionInterestStateTaxItemTypeID, ldecStateTaxOnInterst, busPayeeAccountHelper.GetStateTaxVendorID());
                    }
                }
            }
        }
        //Create retro payment details for the retro interest and tax amounts for the interest
        private void CreateRetroPaymentDetailForInterestAndTaxOnInterest(int aintItemID, decimal adecAmount, int aintVendorID)
        {
            int lintNoOfPaymentsToPay = busGlobalFunctions.DateDiffByMonth(icdoPayeeAccountRetroPayment.start_date, icdoPayeeAccountRetroPayment.end_date);
            if (lintNoOfPaymentsToPay > 0)
            {
                busPayeeAccountRetroPaymentDetail lobjRetroPaymentDetail = new busPayeeAccountRetroPaymentDetail();
                lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail();
                lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = aintItemID;
                lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = aintItemID;
                lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.vendor_org_id = aintVendorID;
                lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.payee_account_retro_payment_id
                                                = icdoPayeeAccountRetroPayment.payee_account_retro_payment_id;
                lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.amount = adecAmount / lintNoOfPaymentsToPay;
                lobjRetroPaymentDetail.icdoPayeeAccountRetroPaymentDetail.Insert();
            }
        }
        //Get the payment item type id and check flat tax indicator 
        private int GetPaymentItemTypeID(string astrItemCode, ref bool iblnSpecialFlatTaxIndicator)
        {
            if (ibusPayeeAccount.iclbPaymentItemType == null)
                ibusPayeeAccount.LoadPaymentItemType();
            busPaymentItemType lobjPaymentItemType
                = ibusPayeeAccount.iclbPaymentItemType.Where(o => o.icdoPaymentItemType.item_type_code == astrItemCode).FirstOrDefault();
            if (lobjPaymentItemType.icdoPaymentItemType.special_tax_treatment_code_value == busConstant.SpecialTaxIdendtifierFlatTax)
            {
                iblnSpecialFlatTaxIndicator = true;
            }
            return lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id;
        }
        //Calculate tax on interest amount
        private void CalculateFlatTax(bool ablnSpecialFlatTaxIndicator, decimal adecAmount, ref decimal adecStateTax, ref decimal adecFedTax)
        {
            if (ablnSpecialFlatTaxIndicator)
            {
                adecFedTax = busPayeeAccountHelper.CalculateFlatTax(adecAmount, 
                    ibusPayeeAccount.idtNextBenefitPaymentDate, busConstant.PayeeAccountTaxIdentifierFedTax, busConstant.PayeeAccountTaxRefFed22Tax, busConstant.Flag_No);
                adecStateTax = busPayeeAccountHelper.CalculateFlatTax(adecAmount, 
                   ibusPayeeAccount.idtNextBenefitPaymentDate, busConstant.PayeeAccountTaxIdentifierStateTax,
                   busConstant.PayeeAccountTaxRefState22Tax, busConstant.Flag_No);
            }
        }
        //79 
        /// <summary>
        /// Method to create Monthwise Adjustment Details
        /// </summary>
        /// <param name="adtEffectiveDate">Effective Date</param>
        /// <param name="adecAmount">Amount</param>
        /// <param name="adecInterest">Interest Amount</param>
        public void CreateMonthwiseAdjustmentDetail(DateTime adtEffectiveDate, decimal adecAmount, decimal adecInterest)
        {
            busPayeeAccountMonthwiseAdjustmentDetail lobjMonthwiseAdjustmentDetail = new busPayeeAccountMonthwiseAdjustmentDetail { icdoPayeeAccountMonthwiseAdjustmentDetail = new cdoPayeeAccountMonthwiseAdjustmentDetail() };

            lobjMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.payee_account_retro_payment_id = icdoPayeeAccountRetroPayment.payee_account_retro_payment_id;
            lobjMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date = adtEffectiveDate;
            lobjMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.amount = adecAmount;
            lobjMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount = adecInterest;
            lobjMonthwiseAdjustmentDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.Insert();
        }

        //property to contain all recovery for the payee account in retro payment
        public busPaymentRecovery ibusPaymentRecovery { get; set; }
        //property to contain gross reduction amount from recovery
        public decimal idecGrossReductionAmount { get; set; }
        //property to contain cola amount
        public decimal idecCOLAAmount { get; set; }
        /// <summary>
        /// method to load payment recovery of type life time reduction or life time reduction RTW
        /// recovery should be approved or in progress
        /// </summary>
        public void LoadPaymentRecovery()
        {
            ibusPaymentRecovery = new busPaymentRecovery { icdoPaymentRecovery = new cdoPaymentRecovery() };
            DataTable ldtRecovery = Select<cdoPaymentRecovery>(new string[1] { enmPaymentRecovery.payee_account_id.ToString() },
                                                                new object[1] { icdoPayeeAccountRetroPayment.payee_account_id }, null, null);
            DataTable ldtFilteredRecovery = ldtRecovery.AsEnumerable()
                                                        .Where(o => o.Field<string>("status_value") == busConstant.RecoveryStatusSatisfied &&
                                                                    (o.Field<string>("repayment_type_value") == busConstant.RecoveryRePaymentTypeLifeTimeReduction ||
                                                                    o.Field<string>("repayment_type_value") == busConstant.RecoveryRePaymentTypeLifeTimeReductionForRTW))
                                                        .AsDataTable();
            if (ldtFilteredRecovery.Rows.Count > 0)
            {
                ibusPaymentRecovery.icdoPaymentRecovery.LoadData(ldtFilteredRecovery.Rows[0]);
                idecGrossReductionAmount = ibusPaymentRecovery.icdoPaymentRecovery.gross_reduction_amount;
            }
        }
        /// <summary>
        /// Method to load gross reduction amount from recovery
        /// </summary>
        public void LoadGrossReductionAmount()
        {
            if (ibusPaymentRecovery == null)
                LoadPaymentRecovery();
            idecGrossReductionAmount = ibusPaymentRecovery.icdoPaymentRecovery.gross_reduction_amount;
        }
        /// <summary>
        /// Method to calculate and insert new PAPIT amounts for Pop up benefits
        /// </summary>
        public void CalculatePAPITAmounts()
        {
            decimal ldecAdjustedBenefitAmount = 0.0M, ldecAdjustedSingleLifeBenefitAmount = 0.0M, ldecNonTaxableAmount = 0.0M, ldecNewTaxableAmount = 0.0M;

            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();

            //Method to calculate adjusted benefit amount
            ldecAdjustedBenefitAmount = CalculateAdjustedBenefitAmount();
            //method to get the final adjusted single life benefit
            ldecAdjustedSingleLifeBenefitAmount = CalculateAdjustedSingleLifeBenefitAmount(ldecAdjustedBenefitAmount);
            //method to get the latest non taxable amount from PAPIT
            ldecNonTaxableAmount = ibusPayeeAccount.LoadLatestPAPITAmount(busConstant.PAPITTaxalbeAmount);
            //calculating new taxable amount and inserting into PAPIT
            ldecNewTaxableAmount = ldecAdjustedSingleLifeBenefitAmount > ldecNonTaxableAmount ?
                ldecAdjustedSingleLifeBenefitAmount - ldecNonTaxableAmount : 0;
            ibusPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxableAmount,
                                                                ldecNewTaxableAmount,
                                                                string.Empty,
                                                                0,
                                                                ibusPayeeAccount.idtNextBenefitPaymentDate,
                                                                DateTime.MinValue,
                                                                false);
            //Method to calculate new Adhoc increase / COLA items
            CalculateAndUpdateAdhocAndCOLAItems();
        }

        /// <summary>
        /// Method to calculate adjusted single life benefit amount
        /// </summary>
        /// <param name="adecAdjustedBenefitAmount">Calculated Adjusted benefit amount</param>
        /// <returns>Calculated adjusted single life benefit amount</returns>
        public decimal CalculateAdjustedSingleLifeBenefitAmount(decimal adecAdjustedBenefitAmount)
        {
            decimal ldecAdjustedSingleLifeBenefitAmount = 0.0M;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusBenefitAccount == null)
                ibusPayeeAccount.LoadBenfitAccount();
            LoadGrossReductionAmount();

            ldecAdjustedSingleLifeBenefitAmount = adecAdjustedBenefitAmount /
                (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.option_factor > 0 ? ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.option_factor : 1);
            return ldecAdjustedSingleLifeBenefitAmount - idecGrossReductionAmount;
        }
        /// <summary>
        /// Method to calculate adjusted benefit amount
        /// </summary>
        /// <returns>Calculated adjusted benefit amount</returns>
        public decimal CalculateAdjustedBenefitAmount()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
            if (ibusPayeeAccount.idecGrossBenefitAmountExcludingCOLAAndAhocIncrease == 0)
                ibusPayeeAccount.LoadGrossBenefitAmountExcludingCOLAAndAhocIncrease(ibusPayeeAccount.idtNextBenefitPaymentDate);
            LoadGrossReductionAmount();

            return ibusPayeeAccount.idecGrossBenefitAmountExcludingCOLAAndAhocIncrease + idecGrossReductionAmount;
        }

        /// <summary>
        /// Method to update Adhoc increase and COLA items
        /// </summary>
        public void CalculateAndUpdateAdhocAndCOLAItems()
        {
            if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            //getting all active AdhocIncrease items
            IEnumerable<busPayeeAccountPaymentItemType> lenmAdhocIncreaseItems = ibusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                                .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.adhoc_cola_group_value == busConstant.AdhocIncreasePaymentItem);
            //getting all active COLA items
            IEnumerable<busPayeeAccountPaymentItemType> lenmCOLAItems = ibusPayeeAccount.iclbPayeeAccountPaymentItemType
                                                                                .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.adhoc_cola_group_value == busConstant.COLAPaymentItem);
            //inserting new Adhoc increase items into PAPIT
            CalculateAndUpdatePAPITItems(lenmAdhocIncreaseItems);
            //inserting new COLA items into PAPIT
            CalculateAndUpdatePAPITItems(lenmCOLAItems);
        }

        /// <summary>
        /// Method to insert increased COLA/Adhoc items items into PAPIT
        /// </summary>
        /// <param name="aenmCOLAItems">PAPIT items</param>
        public void CalculateAndUpdatePAPITItems(IEnumerable<busPayeeAccountPaymentItemType> aenmPAPITItems)
        {
            decimal ldecAmount = 0.0M, ldecFlatIncreaseAmount = 0.0M;
            if (ibusPayeeAccount.ibusBenefitAccount == null)
                ibusPayeeAccount.LoadBenfitAccount();

            foreach (busPayeeAccountPaymentItemType lobjPAPIT in aenmPAPITItems)
            {
                //calculating the new amounts based on option factor
                ldecFlatIncreaseAmount = GetFlatDollarAmountIncrease(lobjPAPIT.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id);
                ldecAmount = (lobjPAPIT.icdoPayeeAccountPaymentItemType.amount - ldecFlatIncreaseAmount) /
                (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.option_factor > 0 ? ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.option_factor : 1);
                ldecAmount += ldecFlatIncreaseAmount;
                //creating papit entries
                ibusPayeeAccount.CreatePayeeAccountPaymentItemType(lobjPAPIT.icdoPayeeAccountPaymentItemType.payment_item_type_id,
                                                                    ldecAmount,
                                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.account_number,
                                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.vendor_org_id,
                                                                    ibusPayeeAccount.idtNextBenefitPaymentDate,
                                                                    DateTime.MinValue,
                                                                    false);
            }
        }

        /// <summary>
        /// method to get flat dollar increase
        /// </summary>
        /// <param name="aintPAPITId">PAPIT id</param>
        /// <returns>Flat dollar increase</returns>
        public decimal GetFlatDollarAmountIncrease(int aintPAPITId)
        {
            decimal ldecFlatIncrease = 0.0M;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();

            DataTable ldtDetail = Select<cdoPostRetirementIncreaseBatchRequestDetail>(
                new string[2] { enmPostRetirementIncreaseBatchRequestDetail.payee_account_id.ToString(), enmPostRetirementIncreaseBatchRequestDetail.payee_account_payment_item_type_id.ToString() },
                new object[2] { ibusPayeeAccount.icdoPayeeAccount.payee_account_id, aintPAPITId }, null, null);

            if (ldtDetail.Rows.Count > 0)
            {
                DataTable ldtBatchRequest = Select<cdoPostRetirementIncreaseBatchRequest>(new string[1] { enmPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id.ToString() },
                    new string[1] { ldtDetail.Rows[0]["post_retirement_increase_batch_request_id"].ToString() }, null, null);

                if (ldtBatchRequest.Rows.Count > 0 && ldtBatchRequest.Rows[0]["increase_flat_amount"] != DBNull.Value)
                    ldecFlatIncrease = Convert.ToDecimal(ldtBatchRequest.Rows[0]["increase_flat_amount"]);
            }
            return ldecFlatIncrease;
        }

		//Backlog PIR 10966
        public void CalculateFedAndStateTaxRetro()
        {
            decimal ldecFedTaxAmt = 0, ldecStateTaxAmt = 0;
            int lintNoOfPayments = busGlobalFunctions.DateDiffByMonth(icdoPayeeAccountRetroPayment.start_date, icdoPayeeAccountRetroPayment.end_date);
            if (ibusPayeeAccount.IsNull())
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbMonthlyBenefits.IsNull())
                ibusPayeeAccount.LoadMontlyBenefits();
            busPayeeAccountPaymentItemType lbusPAFedPaymentItemType = ibusPayeeAccount.iclbMonthlyBenefits.FirstOrDefault(papit=>papit.ibusPaymentItemType.icdoPaymentItemType.payment_item_type_id == 62);
            busPayeeAccountPaymentItemType lbusPAStatePaymentItemType = ibusPayeeAccount.iclbMonthlyBenefits.FirstOrDefault(papit => papit.ibusPaymentItemType.icdoPaymentItemType.payment_item_type_id == 63);
            decimal ldecPerMonthFedTax = 0.0M, ldecPerMonthStateTax = 0.0M;
            if (lbusPAFedPaymentItemType.IsNotNull())
                ldecPerMonthFedTax = lbusPAFedPaymentItemType.icdoPayeeAccountPaymentItemType.amount;
            if (lbusPAStatePaymentItemType.IsNotNull())
                ldecPerMonthStateTax = lbusPAStatePaymentItemType.icdoPayeeAccountPaymentItemType.amount;
            if (ibusPayeeAccount.iclbPaymentItemType == null)
                ibusPayeeAccount.LoadPaymentItemType();
            if (ibusPayeeAccount.iclbRetroItemType == null)
                ibusPayeeAccount.LoadRetroItemType(icdoPayeeAccountRetroPayment.retro_payment_type_value);
             CalculateStateAndFedTaxAmtBasedOnPaymentMonths( ldecPerMonthStateTax, ldecPerMonthFedTax, ref ldecStateTaxAmt, ref ldecFedTaxAmt);
          
            DeleteRetroPaymentDetailsFederalAndStateTax();
            if (ldecFedTaxAmt > 0)
            {
                bool lblnSpecialFlatTaxIndicator = false;
                string lstrItemCode = string.Empty;
                if (icdoPayeeAccountRetroPayment.payment_option_value == busConstant.PayeeAccountRetroPaymentOptionRegular)
                    lstrItemCode = busConstant.PAPITRetroFedTaxAmount;
                else if (icdoPayeeAccountRetroPayment.payment_option_value == busConstant.PayeeAccountRetroPaymentOptionSpecial)
                    lstrItemCode = busConstant.PAPITRetroFedTaxAmountSpecial;


                int lintPaymentItemTypeID = GetPaymentItemTypeID(lstrItemCode, ref lblnSpecialFlatTaxIndicator);

                busRetroItemType lobjRetroRef = ibusPayeeAccount.iclbRetroItemType.Where(o =>
                    o.icdoRetroItemType.to_item_type == lstrItemCode).FirstOrDefault();

                busPaymentItemType lobjOriginalItemType = ibusPayeeAccount.iclbPaymentItemType.Where(o =>
                    o.icdoPaymentItemType.item_type_code == lobjRetroRef.icdoRetroItemType.from_item_type).FirstOrDefault();

                InsertIntoRetroPaymentDetail(lintNoOfPayments, ldecFedTaxAmt,
                    lintPaymentItemTypeID, lobjOriginalItemType.icdoPaymentItemType.payment_item_type_id, busPayeeAccountHelper.GetFedTaxVendorID());
            }

            if (ldecStateTaxAmt > 0)
            {
                bool lblnSpecialFlatTaxIndicator = false;
                string lstrItemCode = string.Empty;
                if (icdoPayeeAccountRetroPayment.payment_option_value == busConstant.PayeeAccountRetroPaymentOptionRegular)
                    lstrItemCode = busConstant.PAPITRetroStateTaxAmount;
                else if (icdoPayeeAccountRetroPayment.payment_option_value == busConstant.PayeeAccountRetroPaymentOptionSpecial)
                    lstrItemCode = busConstant.PAPITRetroStateTaxAmountSpecial;

                int lintPaymentItemTypeID = GetPaymentItemTypeID(lstrItemCode, ref lblnSpecialFlatTaxIndicator);

                busRetroItemType lobjRetroRef = ibusPayeeAccount.iclbRetroItemType.Where(o =>
                    o.icdoRetroItemType.to_item_type == lstrItemCode).FirstOrDefault();

                busPaymentItemType lobjOriginalItemType = ibusPayeeAccount.iclbPaymentItemType.Where(o =>
                    o.icdoPaymentItemType.item_type_code == lobjRetroRef.icdoRetroItemType.from_item_type).FirstOrDefault();

                InsertIntoRetroPaymentDetail(lintNoOfPayments, ldecStateTaxAmt,
                    lintPaymentItemTypeID, lobjOriginalItemType.icdoPaymentItemType.payment_item_type_id, busPayeeAccountHelper.GetStateTaxVendorID());
            }
        }
        private void CalculateStateAndFedTaxAmtBasedOnPaymentMonths(decimal adecPerMonthStateTax, decimal adecPerMonthFedTax, ref decimal adecStateTaxAmt, ref decimal adecFedTaxAmt)
        {
            decimal ldecTotalTaxableAmt = (decimal)DBFunction.DBExecuteScalar("entPayeeAccountRetroPayment.LoadRetroPaymentStateAndFedTaxAmount", new object[1] { icdoPayeeAccountRetroPayment.payee_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework); //calculate total taxable amt from retro payment
            if (iintRetroPaymentMonths > 7)
            {
                adecFedTaxAmt = (icdoPayeeAccountRetroPayment.fed_additional_tax_amount > 0) ?
                    icdoPayeeAccountRetroPayment.fed_additional_tax_amount :
                    (ldecTotalTaxableAmt * 0.2M);

                decimal ldecStateTaxBasedOnPercentage = (ldecTotalTaxableAmt * 0.0392M);
                adecStateTaxAmt = (icdoPayeeAccountRetroPayment.stat_additional_tax_amount > 0) ?
                    icdoPayeeAccountRetroPayment.stat_additional_tax_amount :
                    Math.Max((adecPerMonthStateTax * iintRetroPaymentMonths), ldecStateTaxBasedOnPercentage);
            }
            else
            {
                adecFedTaxAmt = (icdoPayeeAccountRetroPayment.fed_additional_tax_amount > 0) ?
                    icdoPayeeAccountRetroPayment.fed_additional_tax_amount :
                    (adecPerMonthFedTax * iintRetroPaymentMonths);
                adecStateTaxAmt = (icdoPayeeAccountRetroPayment.stat_additional_tax_amount > 0) ?
                    icdoPayeeAccountRetroPayment.stat_additional_tax_amount :
                    (adecPerMonthStateTax * iintRetroPaymentMonths);
            }
            adecStateTaxAmt = (istrNoStateTax == busConstant.Flag_Yes) ? 0.0M : adecStateTaxAmt;
        }

		//Backlog PIR 10966
        public Collection<cdoCodeValue> LoadTaxOptionForFedTax()
        {
            Collection<cdoCodeValue> lclbTaxOption = new Collection<cdoCodeValue>();
            DataTable ldtbTaxOptions = iobjPassInfo.isrvDBCache.GetCodeValues(2218);
            foreach (DataRow dr in ldtbTaxOptions.Rows)
            {
                cdoCodeValue lobjCodeValue = new cdoCodeValue();
                if (dr["data2"].ToString() == busConstant.PayeeAccountTaxIdentifierFedTax)
                {
                    if (dr["data1"].ToString() == busConstant.Flag_No)
                    {
                        lobjCodeValue.LoadData(dr);
                        lclbTaxOption.Add(lobjCodeValue);
                    }
                }
            }
            return lclbTaxOption;
        }

		//Backlog PIR 10966
        public Collection<cdoCodeValue> LoadTaxOptionForStateTax()
        {
            Collection<cdoCodeValue> lclbTaxOption = new Collection<cdoCodeValue>();
            DataTable ldtbTaxOptions = iobjPassInfo.isrvDBCache.GetCodeValues(2218);
            foreach (DataRow dr in ldtbTaxOptions.Rows)
            {
                cdoCodeValue lobjCodeValue = new cdoCodeValue();
                if (dr["data2"].ToString() == busConstant.PayeeAccountTaxIdentifierStateTax)
                {
                    if (dr["data1"].ToString() == busConstant.Flag_No)
                    {
                        lobjCodeValue.LoadData(dr);
                        lclbTaxOption.Add(lobjCodeValue);
                    }
                }
            }
            return lclbTaxOption;
        }

        public void CalculateFlatTaxForRecalculationRetroAmount()
        {
            //Calculate flat tax for recalulation retro amount and post
            decimal ldecRecalcRetroFedTax = 0.0m;
            decimal ldecRecalcRetroStateTax = 0.0m;
            //Relaod payee account payment item type
            ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            decimal ldecRetroAmount = iclbPayeeAccountRetroPaymentDetail.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.retro_payment_type_value == busConstant.RetroPaymentTypeBenefitUnderpayment &&
                !string.IsNullOrEmpty(o.ibusPaymentItemType.icdoPaymentItemType.retro_item_code_link) &&
                o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.special_tax_treatment_code_value == busConstant.SpecialTaxIdendtifierFlatTax).
                    Sum(o => o.icdoPayeeAccountRetroPaymentDetail.amount);
            if (ldecRetroAmount > 0.0m)
            {
                CalculateFlatTax(true, ldecRetroAmount, ref ldecRecalcRetroStateTax, ref ldecRecalcRetroFedTax);
                //Post Tax amounts

                if (ibusPayeeAccount.iclbPaymentItemType == null)
                    ibusPayeeAccount.LoadPaymentItemType();

                busPaymentItemType lobjStateTaxRetroItem = ibusPayeeAccount.iclbPaymentItemType.Where(o =>
                                           o.icdoPaymentItemType.item_type_code == busConstant.PAPITRecalcRetroStateTaxAmount).FirstOrDefault();

                busRetroItemType lobjStateTaxRetroRef = ibusPayeeAccount.iclbRetroItemType.Where(o =>
                    o.icdoRetroItemType.to_item_type == lobjStateTaxRetroItem.icdoPaymentItemType.item_type_code).FirstOrDefault();

                busPaymentItemType lobjOriginalStateTaxItemType = ibusPayeeAccount.iclbPaymentItemType.Where(o =>
                    o.icdoPaymentItemType.item_type_code == lobjStateTaxRetroRef.icdoRetroItemType.from_item_type).FirstOrDefault();

                busPaymentItemType lobjFedTaxRetroItem = ibusPayeeAccount.iclbPaymentItemType.Where(o =>
                                         o.icdoPaymentItemType.item_type_code == busConstant.PAPITRecalcRetroFedTaxAmount).FirstOrDefault();

                busRetroItemType lobjFedTaxRetroRef = ibusPayeeAccount.iclbRetroItemType.Where(o =>
                    o.icdoRetroItemType.to_item_type == lobjFedTaxRetroItem.icdoPaymentItemType.item_type_code).FirstOrDefault();//issue identified by sathya while 1099R reconciliation

                busPaymentItemType lobjOriginalFedTaxItemType = ibusPayeeAccount.iclbPaymentItemType.Where(o =>
                    o.icdoPaymentItemType.item_type_code == lobjFedTaxRetroRef.icdoRetroItemType.from_item_type).FirstOrDefault();//issue identified by sathya while 1099R reconciliation
                int lintNoOfPayments = busGlobalFunctions.DateDiffByMonth(icdoPayeeAccountRetroPayment.start_date, icdoPayeeAccountRetroPayment.end_date);
                CheckTaxCalcutationRequired();
                if (lintNoOfPayments > 0)
                {
                    if (ldecRecalcRetroFedTax > 0.0m && (!fed_tax_Identifier_no_tax))
                    {
                        InsertIntoRetroPaymentDetail(lintNoOfPayments, ldecRecalcRetroFedTax, lobjFedTaxRetroItem.icdoPaymentItemType.payment_item_type_id,
                            lobjOriginalFedTaxItemType.icdoPaymentItemType.payment_item_type_id, busPayeeAccountHelper.GetFedTaxVendorID());
                    }
                    if (ldecRecalcRetroStateTax > 0.0m && (!state_tax_identifier_no_tax))
                    {
                        InsertIntoRetroPaymentDetail(lintNoOfPayments, ldecRecalcRetroStateTax, lobjStateTaxRetroItem.icdoPaymentItemType.payment_item_type_id,
                            lobjOriginalStateTaxItemType.icdoPaymentItemType.payment_item_type_id, busPayeeAccountHelper.GetStateTaxVendorID());
                    }
                }
            }
        }

        // When status changed to approved,check whether adjustments with type "popup benefit" is approved then 
        //Check the benfit option is other sigle life ,then change it to single life or
        //Check the benfit option is other standard rhic option ,then change it to standard rhic option    
        public void UpdateBenefitOpitonAndRHICBenefitOption()
        {
            if (IsPopupBenefitApprovedAndNotSingleLifeOption())
            {
                ibusPayeeAccount.icdoPayeeAccount.benefit_option_value = busConstant.BenefitOptionSingleLife;
                ibusPayeeAccount.icdoPayeeAccount.Update();
                if (ibusPayeeAccount.ibusApplication == null)
                    ibusPayeeAccount.LoadApplication();
                ibusPayeeAccount.ibusApplication.icdoBenefitApplication.benefit_option_value = busConstant.BenefitOptionSingleLife;
                ibusPayeeAccount.ibusApplication.icdoBenefitApplication.Update();
            }
            if (IsPopupBenefitApprovedAndNotRHICOptionStandardOrOptionFactorNotNull())
            {
                if (ibusPayeeAccount.ibusBenefitAccount == null)
                    ibusPayeeAccount.LoadBenfitAccount();
                //if rhic benefit option is other than standard, need to update rhic amount
                if (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value != busConstant.RHICOptionStandard &&
                    ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_option_factor > 0)
                {
                    ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_amount =
                        ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_amount /
                        ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_option_factor;
                    //updating member rhic amount and spouse rhic amount in payee account
                    ibusPayeeAccount.icdoPayeeAccount.rhic_amount = ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_amount;
                    ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount = 0;
                    ibusPayeeAccount.icdoPayeeAccount.Update();

                    //UCS 55 - Initialize RHIC Combining Record
                    //CreateRHICCombineOnPopupBenefit(); //Commented as per call with Maik
                }
                ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value = busConstant.RHICOptionStandard;
                //updating option factor to null
                ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.option_factor = 0;
                ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount = 0; //Backlog PIR 8844 changes.
                ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.Update();
                if (ibusPayeeAccount.ibusApplication == null)
                    ibusPayeeAccount.LoadApplication();
                ibusPayeeAccount.ibusApplication.icdoBenefitApplication.rhic_option_value = busConstant.RHICOptionStandard;
                ibusPayeeAccount.ibusApplication.icdoBenefitApplication.Update();
            }
        }

        //check whether Popup Benefit Approved and Benefit option is other than Single life
        public bool IsPopupBenefitApprovedAndNotSingleLifeOption()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();

            //PIR 16510 - For RHIC Popup 'J100' & 'J50S' should be converted to 'Single Life' Benefit Option.

            if (ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption100Percent||ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption50PercentJS )
            {
                return true;
            }

            return false;
        }
        //check whether Popup Benefit Approved and rhic option is not standard rhic
        public bool IsPopupBenefitApprovedAndNotRHICOptionStandardOrOptionFactorNotNull()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusBenefitAccount == null)
                ibusPayeeAccount.LoadBenfitAccount();

            if (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value != busConstant.RHICOptionStandard ||
                ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.option_factor > 0)
            {
                return true;
            }

            return false;
        }

        public void InitializePayeeAccountRetroPaymentDetail()
        {
            iclbPayeeAccountRetroPaymentDetail = new Collection<busPayeeAccountRetroPaymentDetail>();

            if (ibusPayeeAccount.iclbPaymentItemType == null)
                ibusPayeeAccount.LoadPaymentItemType();
            if (ibusPayeeAccount.iclbRetroItemType == null)
                ibusPayeeAccount.LoadRetroItemType(icdoPayeeAccountRetroPayment.retro_payment_type_value);

            IEnumerable<busRetroItemType> lenmRetroItem = ibusPayeeAccount.iclbRetroItemType
                                                                            .Where(o => o.icdoRetroItemType.payment_option_value == busConstant.PayeeAccountRetroPaymentOptionRegular);

            busPaymentItemType lobjTaxablePIT = ibusPayeeAccount.iclbPaymentItemType.Where(o => o.icdoPaymentItemType.item_type_code == busConstant.PAPITTaxableAmount).FirstOrDefault();
            busPaymentItemType lobjNonTaxablePIT = ibusPayeeAccount.iclbPaymentItemType.Where(o => o.icdoPaymentItemType.item_type_code == busConstant.PAPITTaxalbeAmount).FirstOrDefault();
            busPaymentItemType lobjFedTaxPIT = ibusPayeeAccount.iclbPaymentItemType.Where(o => o.icdoPaymentItemType.item_type_code == busConstant.PAPITFederalTaxAmount).FirstOrDefault();
            busPaymentItemType lobjStateTaxPIT = ibusPayeeAccount.iclbPaymentItemType.Where(o => o.icdoPaymentItemType.item_type_code == busConstant.PAPITNDStateTaxAmount).FirstOrDefault();
            busPaymentItemType lobjInterestPIT = ibusPayeeAccount.iclbPaymentItemType.Where(o => o.icdoPaymentItemType.item_type_code == busConstant.PaymentItemTypeCodeITEM51).FirstOrDefault();
            busPaymentItemType lobjFedTaxInterestPIT = ibusPayeeAccount.iclbPaymentItemType.Where(o => o.icdoPaymentItemType.item_type_code == busConstant.ItemTypeFedTaxOnInterestForAdjustments).FirstOrDefault();
            busPaymentItemType lobjStateTaxInterestPIT = ibusPayeeAccount.iclbPaymentItemType.Where(o => o.icdoPaymentItemType.item_type_code == busConstant.ItemTypeStateTaxOnInterestForAdjustments).FirstOrDefault();

            if (lobjTaxablePIT != null)
            {
                busPayeeAccountRetroPaymentDetail lobjDetail = new busPayeeAccountRetroPaymentDetail { icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail() };
                busRetroItemType lobjRetroItem = lenmRetroItem.Where(o => o.icdoRetroItemType.from_item_type == lobjTaxablePIT.icdoPaymentItemType.item_type_code)
                                                                                    .FirstOrDefault();
                if (lobjRetroItem != null)
                {
                    lobjDetail.ibusPaymentItemType = lobjTaxablePIT;
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(lobjRetroItem.icdoRetroItemType.to_item_type);
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = lobjTaxablePIT.icdoPaymentItemType.payment_item_type_id;
                    iclbPayeeAccountRetroPaymentDetail.Add(lobjDetail);
                }
            }
            if (lobjNonTaxablePIT != null)
            {
                busPayeeAccountRetroPaymentDetail lobjDetail = new busPayeeAccountRetroPaymentDetail { icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail() };
                busRetroItemType lobjRetroItem = ibusPayeeAccount.iclbRetroItemType.Where(o => o.icdoRetroItemType.from_item_type == lobjNonTaxablePIT.icdoPaymentItemType.item_type_code)
                                                                                    .FirstOrDefault();
                if (lobjRetroItem != null)
                {
                    lobjDetail.ibusPaymentItemType = lobjNonTaxablePIT;
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(lobjRetroItem.icdoRetroItemType.to_item_type);
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = lobjNonTaxablePIT.icdoPaymentItemType.payment_item_type_id;
                    iclbPayeeAccountRetroPaymentDetail.Add(lobjDetail);
                }
            }
            if (lobjFedTaxPIT != null)
            {
                busPayeeAccountRetroPaymentDetail lobjDetail = new busPayeeAccountRetroPaymentDetail { icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail() };
                busRetroItemType lobjRetroItem = lenmRetroItem.Where(o => o.icdoRetroItemType.from_item_type == lobjFedTaxPIT.icdoPaymentItemType.item_type_code)
                                                                                    .FirstOrDefault();
                if (lobjRetroItem != null)
                {
                    lobjDetail.ibusPaymentItemType = lobjFedTaxPIT;
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(lobjRetroItem.icdoRetroItemType.to_item_type);
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = lobjFedTaxPIT.icdoPaymentItemType.payment_item_type_id;
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.vendor_org_id = busPayeeAccountHelper.GetFedTaxVendorID();
                    iclbPayeeAccountRetroPaymentDetail.Add(lobjDetail);
                }
            }
            if (lobjStateTaxPIT != null)
            {
                busPayeeAccountRetroPaymentDetail lobjDetail = new busPayeeAccountRetroPaymentDetail { icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail() };
                busRetroItemType lobjRetroItem = lenmRetroItem.Where(o => o.icdoRetroItemType.from_item_type == lobjStateTaxPIT.icdoPaymentItemType.item_type_code)
                                                                                    .FirstOrDefault();
                if (lobjRetroItem != null)
                {
                    lobjDetail.ibusPaymentItemType = lobjStateTaxPIT;
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(lobjRetroItem.icdoRetroItemType.to_item_type);
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = lobjStateTaxPIT.icdoPaymentItemType.payment_item_type_id;
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.vendor_org_id = busPayeeAccountHelper.GetStateTaxVendorID();
                    iclbPayeeAccountRetroPaymentDetail.Add(lobjDetail);
                }
            }
            if (lobjInterestPIT != null)
            {
                busPayeeAccountRetroPaymentDetail lobjDetail = new busPayeeAccountRetroPaymentDetail { icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail() };
                busRetroItemType lobjRetroItem = lenmRetroItem.Where(o => o.icdoRetroItemType.from_item_type == lobjInterestPIT.icdoPaymentItemType.item_type_code)
                                                                                    .FirstOrDefault();
                if (lobjRetroItem != null)
                {
                    lobjDetail.ibusPaymentItemType = lobjInterestPIT;
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(lobjRetroItem.icdoRetroItemType.to_item_type);
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = lobjInterestPIT.icdoPaymentItemType.payment_item_type_id;
                    iclbPayeeAccountRetroPaymentDetail.Add(lobjDetail);
                }
            }
            if (lobjFedTaxInterestPIT != null)
            {
                busPayeeAccountRetroPaymentDetail lobjDetail = new busPayeeAccountRetroPaymentDetail { icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail() };
                busRetroItemType lobjRetroItem = lenmRetroItem.Where(o => o.icdoRetroItemType.from_item_type == lobjFedTaxInterestPIT.icdoPaymentItemType.item_type_code)
                                                                                    .FirstOrDefault();
                if (lobjRetroItem != null)
                {
                    lobjDetail.ibusPaymentItemType = lobjFedTaxInterestPIT;
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(lobjRetroItem.icdoRetroItemType.to_item_type);
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = lobjFedTaxInterestPIT.icdoPaymentItemType.payment_item_type_id;
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.vendor_org_id = busPayeeAccountHelper.GetFedTaxVendorID();
                    iclbPayeeAccountRetroPaymentDetail.Add(lobjDetail);
                }
            }
            if (lobjStateTaxInterestPIT != null)
            {
                busPayeeAccountRetroPaymentDetail lobjDetail = new busPayeeAccountRetroPaymentDetail { icdoPayeeAccountRetroPaymentDetail = new cdoPayeeAccountRetroPaymentDetail() };
                busRetroItemType lobjRetroItem = lenmRetroItem.Where(o => o.icdoRetroItemType.from_item_type == lobjStateTaxInterestPIT.icdoPaymentItemType.item_type_code)
                                                                                    .FirstOrDefault();
                if (lobjRetroItem != null)
                {
                    lobjDetail.ibusPaymentItemType = lobjStateTaxInterestPIT;
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.payment_item_type_id = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(lobjRetroItem.icdoRetroItemType.to_item_type);
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.original_payment_item_type_id = lobjStateTaxInterestPIT.icdoPaymentItemType.payment_item_type_id;
                    lobjDetail.icdoPayeeAccountRetroPaymentDetail.vendor_org_id = busPayeeAccountHelper.GetStateTaxVendorID();
                    iclbPayeeAccountRetroPaymentDetail.Add(lobjDetail);
                }
            }
        }

        //uat pir 1420
        public bool IsPayeeAccountStatusCancelPendingOrCancelled()
        {
            bool lblnResult = false;
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                ibusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();

            if (ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelPending() || ibusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                lblnResult = true;

            return lblnResult;
        }

        //ucs - 55 : dependent rules
        public bool IsRHICReimbursementAmountInValid()
        {
            bool lblnResult = false;
            decimal ldecRHICEligibleReimbursement = 0.0M;
            if (icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeRHICBenefitReimbursement)
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusPayeeAccount.ibusPayee == null)
                    ibusPayeeAccount.LoadPayeePerson();
                if (ibusPayeeAccount.ibusPayee.ibusLatestBenefitRhicCombine == null)
                    ibusPayeeAccount.ibusPayee.LoadLatestBenefitRhicCombine();

                if (ibusPayeeAccount.ibusPayee.ibusLatestBenefitRhicCombine != null)
                {
                    ldecRHICEligibleReimbursement = ibusPayeeAccount.ibusPayee.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.total_js_rhic_amount +
                                                    ibusPayeeAccount.ibusPayee.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.total_other_rhic_amount;
                }
                if (iclbPayeeAccountRetroPaymentDetail != null)
                {
                    busPayeeAccountRetroPaymentDetail lobjDetail = iclbPayeeAccountRetroPaymentDetail
                                .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRHICBenefitReimbursement)
                                .FirstOrDefault();
                    if (lobjDetail != null)
                    {
                        if (lobjDetail.icdoPayeeAccountRetroPaymentDetail.amount > ldecRHICEligibleReimbursement)
                            lblnResult = true;
                    }
                }
            }
            return lblnResult;
        }

        /// <summary>
        /// method to create rhic health split
        /// </summary>
        private void CreateRHICHealthSplit()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.ibusPayee == null)
                ibusPayeeAccount.LoadPayeePerson();
            if (ibusPayeeAccount.ibusPayee.ibusLatestBenefitRhicCombine == null)
                ibusPayeeAccount.ibusPayee.LoadLatestBenefitRhicCombine();
            if (ibusPayeeAccount.ibusApplication == null)
                ibusPayeeAccount.LoadApplication();
            if (ibusPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts == null)
                ibusPayeeAccount.ibusApplication.LoadBenefitApplicationPersonAccount();

            busBenefitApplicationPersonAccount lobjBAPA = ibusPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts
                                                            .Where(o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes)
                                                            .FirstOrDefault();
            busPayeeAccountRetroPaymentDetail lobjDetail = iclbPayeeAccountRetroPaymentDetail
                        .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRHICBenefitReimbursement)
                        .FirstOrDefault();
            if (lobjBAPA != null && lobjDetail != null)
            {
                ibusPayeeAccount.ibusPayee.ibusLatestBenefitRhicCombine.CreateRHICCombineHealthSplit(lobjBAPA.icdoBenefitApplicationPersonAccount.person_account_id,
                                                                                                        0.0M, 0.0M, lobjDetail.icdoPayeeAccountRetroPaymentDetail.amount,
                                                                                                        icdoPayeeAccountRetroPayment.payee_account_retro_payment_id);
                ibusPayeeAccount.ibusPayee.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.total_reimbursement_amount += lobjDetail.icdoPayeeAccountRetroPaymentDetail.amount;
                ibusPayeeAccount.ibusPayee.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.Update();
            }
        }

        private void CreateRHICCombineOnPopupBenefit()
        {
            if (ibusPayeeAccount.ibusBenefitAccount == null)
                ibusPayeeAccount.LoadBenfitAccount();
            if ((ibusPayeeAccount.icdoPayeeAccount.rhic_amount > 0) ||
                (ibusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0))
            {
                //Load the Old Rhic Combine Record for this Donor Payee Account
                if (ibusPayeeAccount.ibusLatestBenefitRhicCombine == null)
                    ibusPayeeAccount.LoadLatestBenefitRhicCombine(true);

                //If there is no Previous Record, we are not going to establish Rhic Combine Record. 
                //TODO: Check with Raj, is it okay to initiate workflow here
                if (ibusPayeeAccount.ibusLatestBenefitRhicCombine != null)
                {
                    busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
                    lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = ibusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id;
                    lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = ibusPayeeAccount.GetRHICCombineStartDate(); //PROD PIR ID 5867
                    lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.benefit_adjustment_approval;
                    bool lblnAutoRhicEstablished = lbusBenefitRhicCombine.CreateAutomatedRHICCombine();
                    if (lblnAutoRhicEstablished)
                    {
                        lbusBenefitRhicCombine.CreatePayrollAdjustment();
                        lbusBenefitRhicCombine.CreatePAPITAdjustment();
                    }
                }
            }
        }
        /// <summary>
        /// If retro payment option is special check and include_in_adhoc_flag on the 
        /// payee account is YES, then throw error.
        /// </summary>
        /// <returns></returns>
        public bool IsRetroPaymentWithSpecialCheckSetUpAndIncludeInAdhocChecked()
        {
            if (ibusPayeeAccount.IsNull()) LoadPayeeAccount();
            return (iblnSpecialCheckIncludeInAdhocFlag && icdoPayeeAccountRetroPayment.payment_option_value == busConstant.PayeeAccountRetroPaymentOptionSpecial &&
                icdoPayeeAccountRetroPayment.payment_history_header_id == 0 &&
                ibusPayeeAccount.icdoPayeeAccount.include_in_adhoc_flag == busConstant.Flag_Yes);
        }
        public bool DoesPayeeAccountHasPriorPopup()
        {
            if ((this.istrSupressWarningsFlag != busConstant.Flag_Yes) && ((this.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeBenefitUnderpayment) || 
                (this.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypeRHICBenefitReimbursement)))
            {
                if (ibusPayeeAccount.IsNull()) LoadPayeeAccount();
                if (ibusPayeeAccount.iclbRetroPayment.IsNull()) ibusPayeeAccount.LoadRetroPayment();
                return ibusPayeeAccount
                    .iclbRetroPayment
                    .Any(retropayment => retropayment.icdoPayeeAccountRetroPayment.retro_payment_type_value == 
                    busConstant.RetroPaymentTypePopupBenefits && retropayment.icdoPayeeAccountRetroPayment.approved_flag == busConstant.Flag_Yes);
            }
            return false;
        }
        public bool DoesWithholdingRecordExistForNextBenefitPaymentDate()
        {
            if (string.IsNullOrEmpty(istrSupressWarningsFlag) || istrSupressWarningsFlag != busConstant.Flag_Yes)
            {
                if (ibusPayeeAccount.IsNull()) LoadPayeeAccount();
                if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                    ibusPayeeAccount.LoadNexBenefitPaymentDate();
                if (ibusPayeeAccount.iclbTaxWithholingHistory.IsNull()) ibusPayeeAccount.LoadTaxWithHoldingHistory();
                if(!(ibusPayeeAccount.iclbTaxWithholingHistory.Any(wh=>wh.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax && 
                busGlobalFunctions.CheckDateOverlapping(ibusPayeeAccount.idtNextBenefitPaymentDate, wh.icdoPayeeAccountTaxWithholding.start_date, wh.icdoPayeeAccountTaxWithholding.end_date))) ||
                    !(ibusPayeeAccount.iclbTaxWithholingHistory.Any(wh => wh.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax &&
                    busGlobalFunctions.CheckDateOverlapping(ibusPayeeAccount.idtNextBenefitPaymentDate, wh.icdoPayeeAccountTaxWithholding.start_date, wh.icdoPayeeAccountTaxWithholding.end_date))))
                {
                    return false;
                }
            }
            return true;
        }
    }
}