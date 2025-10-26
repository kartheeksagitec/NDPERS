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
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPaymentHistoryHeader : busPaymentHistoryHeaderGen
    {

        //Property to display fed tax amount in the screen
        public decimal idecFedTaxAmount { get; set; }
        //Property to display state tax amount in the screen
        public decimal idecStateTaxAmount { get; set; }

        //Load the tax amounts based on the payment date
        public void LoadTaxAmount()
        {
            if (icdoPaymentHistoryHeader.payee_account_id > 0)
                LoadPayeeAccount();
            //Load all the fedtax taxwithholding  details
            if (icdoPaymentHistoryHeader.payee_account_id > 0)
            {
                if (ibusPayeeAccount.iclbPayeeAccountFedTaxWithHolding == null)
                    ibusPayeeAccount.LoadFedTaxWithHoldingInfo();
                //get the Fed taxwithholding details for the payment
                busPayeeAccountTaxWithholding lobjFedTaxWithHoldingInfo
                    = ibusPayeeAccount.iclbPayeeAccountFedTaxWithHolding.Where(o => busGlobalFunctions.CheckDateOverlapping(icdoPaymentHistoryHeader.payment_date,
                        o.icdoPayeeAccountTaxWithholding.start_date, o.icdoPayeeAccountTaxWithholding.end_date)).FirstOrDefault();
                if (lobjFedTaxWithHoldingInfo != null)
                {
                    //get the fed tax amount from the tax with holding item detail
                    if (lobjFedTaxWithHoldingInfo.iclbPayeeAccountTaxItems == null)
                        lobjFedTaxWithHoldingInfo.LoadPayeeAccountTaxItems();
                    idecFedTaxAmount = lobjFedTaxWithHoldingInfo.iclbPayeeAccountTaxItems.Where(o => busGlobalFunctions.CheckDateOverlapping(
                        icdoPaymentHistoryHeader.payment_date, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)).Select(p => p.icdoPayeeAccountPaymentItemType.amount).FirstOrDefault();
                }
                //Load all the State tax taxwithholding  details
                if (ibusPayeeAccount.iclbPayeeAccountStateTaxWithHolding == null)
                    ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
                //get the state tax taxwithholding details for the payment
                busPayeeAccountTaxWithholding lobjStateTaxWithHoldingInfo
                  = ibusPayeeAccount.iclbPayeeAccountStateTaxWithHolding.Where(o => busGlobalFunctions.CheckDateOverlapping(icdoPaymentHistoryHeader.payment_date,
                      o.icdoPayeeAccountTaxWithholding.start_date, o.icdoPayeeAccountTaxWithholding.end_date)).FirstOrDefault();
                if (lobjStateTaxWithHoldingInfo != null)
                {
                    //get the state tax amount from the tax with holding item detail
                    if (lobjStateTaxWithHoldingInfo.iclbPayeeAccountTaxItems == null)
                        lobjStateTaxWithHoldingInfo.LoadPayeeAccountTaxItems();
                    idecStateTaxAmount = lobjStateTaxWithHoldingInfo.iclbPayeeAccountTaxItems.Where(o => busGlobalFunctions.CheckDateOverlapping(
                        icdoPaymentHistoryHeader.payment_date, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)).Select(p => p.icdoPayeeAccountPaymentItemType.amount).FirstOrDefault();
                }
            }
        }
        //Create payement history header for each provider from ucs-36 Disburse Funds Batch 
        public void CreateVendorPaymentHistoryHeader(int aintOrgId, int aintPlanId, DateTime adtPaymentDate)
        {
            busPaymentSchedule lobjPaymentSchedule = busPayeeAccountHelper.GetPaymentSchedule(adtPaymentDate, "VNPM");
            if (lobjPaymentSchedule != null)
            {
                this.icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader();
                this.icdoPaymentHistoryHeader.plan_id = aintPlanId;
                this.icdoPaymentHistoryHeader.org_id = aintOrgId;
                this.icdoPaymentHistoryHeader.payment_schedule_id = busPayeeAccountHelper.GetPaymentSchedule(adtPaymentDate, "VNPM").icdoPaymentSchedule.payment_schedule_id;
                this.icdoPaymentHistoryHeader.payment_date = adtPaymentDate;
                this.icdoPaymentHistoryHeader.status_value = busConstant.PaymentStatusOutstanding;
                this.icdoPaymentHistoryHeader.Insert();
            }
        }
        //Create payement history details for each provider from ucs-36 Disburse Funds Batch 
        public void CreateVendorPaymentHistoryDetails(decimal adecAmount, string astrPaymentItemCode,int aintPaymentDItemTypeID)
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            int lintPaymentItemTypeID=0;
            if(aintPaymentDItemTypeID>0)
                lintPaymentItemTypeID = aintPaymentDItemTypeID;
            else
                lintPaymentItemTypeID = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(astrPaymentItemCode);
            busPaymentHistoryDetail lobjPaymentHistoryDetail = new busPaymentHistoryDetail();
            lobjPaymentHistoryDetail.icdoPaymentHistoryDetail = new cdoPaymentHistoryDetail();
            lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.payment_history_header_id = this.icdoPaymentHistoryHeader.payment_history_header_id;
            lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.payment_item_type_id = lintPaymentItemTypeID;
            lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.amount = adecAmount;
            lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.Insert();
        }
        //Create payment Check history details for each provider from ucs-36 Disburse Funds Batch 
        public int CreateVendorPaymentDistributionDetails(decimal adecAmount, int aintLastCheckNumber)
        {
            if (ibusOrganization == null)
                LoadOrganization();
            if (ibusOrganization.ibusOrgPrimaryAddress == null)
                ibusOrganization.LoadOrgPrimaryAddress();
            if (ibusOrganization.iclbOrgBank == null)
                ibusOrganization.LoadOrgBank();
            busPaymentHistoryDistribution lobjPaymentDistributionDetails = new busPaymentHistoryDistribution { icdoPaymentHistoryDistribution = new cdoPaymentHistoryDistribution() };
            lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.payment_history_header_id = this.icdoPaymentHistoryHeader.payment_history_header_id;
            string lstrPaymentMethod = string.Empty;
            //if(bank org exist for the Vendor Org,then the payment method id ACH else Check
            //lstrPaymentMethod = ibusOrganization.iclbOrgBank.Where(o => o.icdoOrgBank.usage_value == busConstant.BankUsageDirectDeposit
            //    && o.icdoOrgBank.status_value == busConstant.StatusActive).Count() > 0 ?
            //        busConstant.PaymentDistributionPaymentMethodACH : busConstant.PaymentDistributionPaymentMethodCHK;
            if (string.IsNullOrEmpty(ibusOrganization.icdoOrganization.payment_option_value) || 
                ibusOrganization.icdoOrganization.payment_option_value == busConstant.OrganizationPaymentOptionCHK)
            {
                lstrPaymentMethod = busConstant.PaymentDistributionPaymentMethodCHK;
            }
            else if (ibusOrganization.icdoOrganization.payment_option_value == busConstant.OrganizationPaymentOptionWIRE)
            {
                lstrPaymentMethod = busConstant.PaymentDistributionPaymentMethodWIRE;
            }
            else if (ibusOrganization.icdoOrganization.payment_option_value == busConstant.OrganizationPaymentOptionACH &&
                ibusOrganization.iclbOrgBank.Where(o => o.icdoOrgBank.usage_value == busConstant.BankUsageDirectDeposit
                && o.icdoOrgBank.status_value == busConstant.StatusActive).Any())
            {
                lstrPaymentMethod = busConstant.PaymentDistributionPaymentMethodACH;
            }
            else
            {
                lstrPaymentMethod = busConstant.PaymentDistributionPaymentMethodCHK;
            }
            lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.payment_method_value = lstrPaymentMethod;
            //if payment method is ACH ,Get the Bank details of the Vendor Org
            if (lstrPaymentMethod == busConstant.PaymentDistributionPaymentMethodACH)
            {
                busOrgBank lobjOrgBank = ibusOrganization.iclbOrgBank.Where(o => o.icdoOrgBank.usage_value == busConstant.BankUsageDirectDeposit
                 && o.icdoOrgBank.status_value == busConstant.StatusActive).FirstOrDefault();
                if (lobjOrgBank.ibusBankOrg == null)
                    lobjOrgBank.LoadBankOrg();
                if (lobjOrgBank.ibusBankOrg.icdoOrganization.org_id > 0)
                {
                    lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.routing_number = lobjOrgBank.ibusBankOrg.icdoOrganization.routing_no;
                    lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.account_number = lobjOrgBank.icdoOrgBank.account_no;
                    lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.account_type_value = lobjOrgBank.icdoOrgBank.account_type_value;
                    lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PaymentDistributionStatusACHOutstanding;
                }
            }
            else if (lstrPaymentMethod == busConstant.PaymentDistributionPaymentMethodWIRE)
            {
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PaymentDistributionStatusCleared;
                icdoPaymentHistoryHeader.status_value = busConstant.PaymentStatusProcessed;
                icdoPaymentHistoryHeader.Update();
            }
            else
            {
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.PaymentDistributionStatusCHKOutstanding;
                aintLastCheckNumber = aintLastCheckNumber + 1;
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.check_number = aintLastCheckNumber.ToString();
            }
            if (ibusOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.contact_org_address_id > 0)
            {
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.addr_line_1 = ibusOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_1;
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.addr_line_2 = ibusOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.addr_line_2;
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.addr_city = ibusOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.city;
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.addr_state_value = ibusOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.state_value;
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.addr_country_value = "0001";
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.addr_zip_code = ibusOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_code;
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.addr_zip_4_code = ibusOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.zip_4_code;
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.foreign_postal_code = ibusOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.foreign_postal_code;
                lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.foreign_province = ibusOrganization.ibusOrgPrimaryAddress.icdoOrgContactAddress.foreign_province;
            }
            lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.net_amount = adecAmount;
            lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.org_id = this.icdoPaymentHistoryHeader.org_id;           
            lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.recipient_name = ibusOrganization.icdoOrganization.org_name;        
            lobjPaymentDistributionDetails.icdoPaymentHistoryDistribution.Insert();
            lobjPaymentDistributionDetails.CreateHistory(this.icdoPaymentHistoryHeader.payment_date);
            return aintLastCheckNumber;
        }
        
        public ArrayList btnReceivablesPending_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            //Creating GL
            busGLHelper.GenerateGL(busConstant.GLStatusTransitionPaymentHistory, icdoPaymentHistoryHeader.status_value,
                                    busConstant.HistoryHeaderStatusReceivablePending, busConstant.GLTransactionTypeStatusTransition,
                                    busConstant.GLSourceTypeValueChkMaintenance, this, null, iobjPassInfo);
            icdoPaymentHistoryHeader.status_value = busConstant.HistoryHeaderStatusReceivablePending;
            icdoPaymentHistoryHeader.Update();
            if (iclbPaymentHistoryDistribution == null)
                LoadPaymentHistoryDistribution();
            foreach (busPaymentHistoryDistribution lobjPaymentHistoryDistribution in iclbPaymentHistoryDistribution)
            {
                if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKReceivablesPending;
                }
                else if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.ACHReceivablePending;
                }
                else if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKReceivablesPending;
                }
                else
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RACHReceivablePending;
                }
                lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();
                lobjPaymentHistoryDistribution.CreateHistory(DateTime.Now);
                lobjPaymentHistoryDistribution.LoadLatestDistibutionStatusHistory();
            }
            if (!iblnIsPayeeAccountCancelled)
            {
                if (ibusBaseActivityInstance.IsNotNull())
                    //SetProcessInstanceParameters();
                    SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            alReturn.Add(this);
            return alReturn;
        }
        //Check whether any distribution record exist in outstanding status
        public bool IsOutstandingDistributionRecordExist()
        {
            foreach (busPaymentHistoryDistribution lobjPaymentHistoryDistribution in iclbPaymentHistoryDistribution)
            {
                lobjPaymentHistoryDistribution.LoadStatus();
                if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.status_value == busConstant.DistributionStatusOutstanding)
                {
                    return true;
                }
            }
            return false;
        }
        //Change the status to Cancel Pending when btnCancelPending_Click is clicked
        public ArrayList btnCancelPending_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            //Creating GL
            busGLHelper.GenerateGL(busConstant.GLStatusTransitionPaymentHistory, icdoPaymentHistoryHeader.status_value,
                                    busConstant.HistoryHeaderStatusCancelPending, busConstant.GLTransactionTypeStatusTransition,
                                    busConstant.GLSourceTypeValueChkMaintenance, this, null, iobjPassInfo);
            icdoPaymentHistoryHeader.status_value = busConstant.HistoryHeaderStatusCancelPending;
            icdoPaymentHistoryHeader.Update();
            if (!iblnIsPayeeAccountCancelled)
            {
                if (ibusBaseActivityInstance.IsNotNull())
                    // SetProcessInstanceParameters();
                    SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            alReturn.Add(this);
            return alReturn;
        }
        //Change the status to Cancel Prior Payment when btnCancelPriorPayment_Click is clicked
        public ArrayList btnCancelPriorPayment_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (!iblnIsPayeeAccountCancelled && IsOutstandingDistributionRecordExist())
            {
                lobjError = AddError(6454, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            if (iclbPaymentHistoryDistribution == null)
                LoadPaymentHistoryDistribution();
            foreach (busPaymentHistoryDistribution lobjPaymentHistoryDistribution in iclbPaymentHistoryDistribution)
            {
                if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKCancelled;
                }
                else if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.ACHCancelled;
                }
                else if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKCancelled;
                }
                else
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RACHCancelled;
                }
                lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();
                lobjPaymentHistoryDistribution.CreateHistory(DateTime.Now);
                lobjPaymentHistoryDistribution.LoadLatestDistibutionStatusHistory();
            }
            //Creating GL
            if ((icdoPaymentHistoryHeader.old_payment_history_header_id > 0) && (icdoPaymentHistoryHeader.payee_account_id > 0) && (icdoPaymentHistoryHeader.person_id > 0))
            {
                busPaymentHistoryHeader lbusPaymentHistoryHeader = GetOriginalPayment(icdoPaymentHistoryHeader.old_payment_history_header_id);
                if (lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id > 0)
                    busGLHelper.GenerateGL(busConstant.GLStatusTransitionPaymentHistory, icdoPaymentHistoryHeader.status_value,
                                            busConstant.HistoryHeaderStatusCancelPriorPayment, busConstant.GLTransactionTypeStatusTransition,
                                            busConstant.GLSourceTypeValueChkMaintenance, lbusPaymentHistoryHeader, null, iobjPassInfo);
            }
            else
            {
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionPaymentHistory, icdoPaymentHistoryHeader.status_value,
                                        busConstant.HistoryHeaderStatusCancelPriorPayment, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, this, null, iobjPassInfo);

            }
            icdoPaymentHistoryHeader.status_value = busConstant.HistoryHeaderStatusCancelPriorPayment;
            icdoPaymentHistoryHeader.Update();
            //PIR - 1994 (benefit overpayment creation on Cancel Prior Payment)
            //UCS - 079 -- Start - Cancelling Payment history header
            if (!iblnIsPayeeAccountCancelled)
            {
                if (iclbPaymentHistoryDetail == null)
                    LoadPaymentHistoryDetails();
                if (iclbPaymentHistoryDetail.Sum(o => o.icdoPaymentHistoryDetail.amount) != 0)
                    CreateBenefitOverPayment(busConstant.HistoryHeaderStatusCancel);              
            }
            this.EvaluateInitialLoadRules();
            alReturn.Add(this);
            return alReturn;
        }
        //cancel or cancel prior payment button visiblity- if 1099r exist for the payment date year ,show prior payment button else show cancel button
        public string IsbtnCancelPriorPaymentVisible()
        {
            string lstrCancel = string.Empty;
            if (icdoPaymentHistoryHeader.status_value == busConstant.HistoryHeaderStatusCancelPending && icdoPaymentHistoryHeader.modified_by != iobjPassInfo.istrUserID)
            {
                lstrCancel = "CNLD";
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (Is1099rExistsForPaymentYear())
                {
                    lstrCancel = "CPYP";
                }
            }
            return lstrCancel;
        }
        public bool Is1099rExistsForPaymentYear()
        {
            if (ibusPayeeAccount.iclbPayment1099r == null)
                ibusPayeeAccount.LoadPayment1099r();
            if (ibusPayeeAccount.iclbPayment1099r.Where(o =>
                o.icdoPayment1099r.tax_year == icdoPaymentHistoryHeader.payment_date.Year).Count() > 0)
            {
                return true;
            }
            return false;
        }
        //Change the status to Cancel  when btnCancel_Click is clicked
        public ArrayList btnCancel_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (!iblnIsPayeeAccountCancelled&& IsOutstandingDistributionRecordExist())
            {
                lobjError = AddError(6454, "");
                alReturn.Add(lobjError);
                return alReturn;
            }
            if (iclbPaymentHistoryDistribution == null)
                LoadPaymentHistoryDistribution();
            foreach (busPaymentHistoryDistribution lobjPaymentHistoryDistribution in iclbPaymentHistoryDistribution)
            {
                if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKCancelled;
                }
                else if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.ACHCancelled;
                }
                else if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKCancelled;
                }
                else
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RACHCancelled;
                }
                lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();
                lobjPaymentHistoryDistribution.CreateHistory(DateTime.Now);
                lobjPaymentHistoryDistribution.LoadLatestDistibutionStatusHistory();
            }
            if ((icdoPaymentHistoryHeader.old_payment_history_header_id > 0) && (icdoPaymentHistoryHeader.payee_account_id > 0) && (icdoPaymentHistoryHeader.person_id > 0))
            {
                ////938 GL Changes
                busPaymentHistoryHeader lbusPaymentHistoryHeader = GetOriginalPayment(icdoPaymentHistoryHeader.old_payment_history_header_id);
                if (lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id > 0)
                    busGLHelper.GenerateGL(busConstant.GLStatusTransitionPaymentHistory, icdoPaymentHistoryHeader.status_value,
                                            busConstant.HistoryHeaderStatusCancel, busConstant.GLTransactionTypeStatusTransition,
                                            busConstant.GLSourceTypeValueChkMaintenance, lbusPaymentHistoryHeader, null, iobjPassInfo);
            }
            else //Creating GL
            {
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionPaymentHistory, icdoPaymentHistoryHeader.status_value,
                                        busConstant.HistoryHeaderStatusCancel, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, this, null, iobjPassInfo);
            }
            icdoPaymentHistoryHeader.status_value = busConstant.HistoryHeaderStatusCancel;
            icdoPaymentHistoryHeader.Update();

            //pir 8530
            if (icdoPaymentHistoryHeader.payee_account_id == 0 && (icdoPaymentHistoryHeader.org_id != 0 ||
                icdoPaymentHistoryHeader.person_id != 0) &&
                ibusPaymentSchedule.icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeAdhoc)
            {
                LoadRemittance();
                ibusRemittance.LoadDeposit();
                ibusRemittance.ibusDeposit.LoadDepositTape();
                CreateRemittance(CreateDeposit(CreateDepositTape()));

            }

            //UCS - 079 -- Start - Cancelling Payment history header
            if (!iblnIsPayeeAccountCancelled)
            {
                if (iclbPaymentHistoryDetail == null)
                    LoadPaymentHistoryDetails();

                CreateBenefitOverPayment(busConstant.HistoryHeaderStatusCancel);

                if (ibusBaseActivityInstance.IsNotNull())
                    // SetProcessInstanceParameters();
                    SetCaseInstanceParameters();
            }

            //PIR 26043 -- Delete latest recovery history
            DeleteRecoveryHistory();

            alReturn.Add(this);
            return alReturn;
        }
        /// <summary>
        /// PIR 938 GL changes
		/// To get the original payment of reissued payments
        /// </summary>
        /// <param name="aintOldPaymentHistoryHeaderId"></param>
        /// <returns></returns>
        private busPaymentHistoryHeader GetOriginalPayment(int aintOldPaymentHistoryHeaderId)
        {
            busPaymentHistoryHeader lbusPaymentHistoryHeader = new busPaymentHistoryHeader();
            if(lbusPaymentHistoryHeader.FindPaymentHistoryHeader(aintOldPaymentHistoryHeaderId))
            {
                if(lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.old_payment_history_header_id > 0)
                {
                    lbusPaymentHistoryHeader = GetOriginalPayment(lbusPaymentHistoryHeader.icdoPaymentHistoryHeader.old_payment_history_header_id);
                }
                else
                {
                    return lbusPaymentHistoryHeader;
                }
            }
            return lbusPaymentHistoryHeader;
        }

        public bool iblnProviderReportPosting { get; set; }

        /// <summary>
        /// Method to check whether benefit overpayment need to be created or not
        /// </summary>
        /// <returns></returns>
        public bool IsPaymentDetailExistsForBenefitoverpaymentHeader()
        {
            //NOTE : Should not load payment history details in this method, any method using this should populate history detail
            //since its used for payee account cancel functionality too and we aggreate all payment history deatils for the payee 
            //account for that
            bool lblnResut = false;
            IEnumerable<busPaymentHistoryDetail> lenmPaymentHistoryDetail;
            if (ibusPayeeAccount.ibusPayment1099r == null)
                LoadPayment1099r(icdoPaymentHistoryHeader.payment_date.Year);
            if (ibusPayeeAccount.ibusPayment1099r.icdoPayment1099r.payment_1099r_id > 0)
            {
                lenmPaymentHistoryDetail = iclbPaymentHistoryDetail
                                             .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWith1099r ||
                                             o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWithOrWithout1099r);               
            }
            else
            {
                lenmPaymentHistoryDetail = iclbPaymentHistoryDetail
                                            .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWithout1099r ||
                                            o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWithOrWithout1099r);

                if (iclbPaymentHistoryDetail.Where(o => o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWith1099r).Any())
                    iblnProviderReportPosting = true;
            }
            if (lenmPaymentHistoryDetail.Any())
                lblnResut = true;
            return lblnResut;
        }

        //Change the status to Receivables Created  when btnReceivablesCreated_Click is clicked
        public ArrayList btnReceivablesCreated_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
          
            if (iclbPaymentHistoryDistribution == null)
                LoadPaymentHistoryDistribution();
            foreach (busPaymentHistoryDistribution lobjPaymentHistoryDistribution in iclbPaymentHistoryDistribution)
            {
                if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKReceivablesCreated;
                }
                else if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.ACHReceivablesCreated;
                }
                else if (lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKReceivablesCreated;
                }
                else
                {
                    lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RACHReceivablesCreated;
                }
                lobjPaymentHistoryDistribution.icdoPaymentHistoryDistribution.Update();
                lobjPaymentHistoryDistribution.CreateHistory(DateTime.Now);
                lobjPaymentHistoryDistribution.LoadLatestDistibutionStatusHistory();
            }

            //UCS - 079 -- Start - Receivable created for Payment history header
            if (!iblnIsPayeeAccountCancelled)
            {
                if (iclbPaymentHistoryDetail == null)
                    LoadPaymentHistoryDetails();
                if (iclbPaymentHistoryDetail.Sum(o => o.icdoPaymentHistoryDetail.amount) != 0)
                    CreateBenefitOverPayment(busConstant.HistoryHeaderStatusReceivableCreated);
            }
            //Creating GL
            busGLHelper.GenerateGL(busConstant.GLStatusTransitionPaymentHistory, icdoPaymentHistoryHeader.status_value,
                                    busConstant.HistoryHeaderStatusReceivableCreated, busConstant.GLTransactionTypeStatusTransition,
                                    busConstant.GLSourceTypeValueChkMaintenance, this, null, iobjPassInfo);
            icdoPaymentHistoryHeader.status_value = busConstant.HistoryHeaderStatusReceivableCreated;
            icdoPaymentHistoryHeader.Update();
            //UCS - 079 -- End
            alReturn.Add(this);
            return alReturn;
        }
        //Calculate tax for the check which is reissued to member
        //public void CalculateTaxForRolloverAmountToPayee()
        //{
        //    if (ibusPayeeAccount == null)
        //        LoadPayeeAccount();
        //    if (ibusPayeeAccount.ibusApplication == null)
        //        ibusPayeeAccount.LoadApplication();
        //    if (iclbPaymentHistoryDetail == null)
        //        LoadPaymentHistoryDetails();
        //    decimal ldecTaxableAmount = iclbPaymentHistoryDetail.Where(o =>
        //        o.icdoPaymentHistoryDetail.payment_item_type_id == ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PaymentItemReissuetoPayeeAmount) &&
        //        o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes &&
        //        o.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != busConstant.RollItemForCheck).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
        //    if (icdoPaymentHistoryHeader.payee_account_id > 0 && ldecTaxableAmount > 0.00m)
        //    {
        //        if (ibusPayeeAccount.iclbPayeeAccountFedTaxWithHolding == null)
        //            ibusPayeeAccount.LoadFedTaxWithHoldingInfo();
        //        //get the Fed taxwithholding details for the payment
        //        busPayeeAccountTaxWithholding lobjFedTaxWithHoldingInfo
        //            = ibusPayeeAccount.iclbPayeeAccountFedTaxWithHolding.Where(o => busGlobalFunctions.CheckDateOverlapping(icdoPaymentHistoryHeader.payment_date,
        //                o.icdoPayeeAccountTaxWithholding.start_date, o.icdoPayeeAccountTaxWithholding.end_date)).FirstOrDefault();
        //        if (lobjFedTaxWithHoldingInfo != null)
        //        {
        //            string lstrPlsoFlag=string.Empty;
        //            if(ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plso_requested_flag==busConstant.Flag_Yes)
        //            {
        //                lstrPlsoFlag = busConstant.Flag_Yes;
                        
        //            }
        //            else
        //            {
        //                lstrPlsoFlag = busConstant.Flag_No;
        //            }
        //            decimal ldecFedTax =0.0m;
        //            if (ibusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipBeneficiary &&
        //          ibusPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.FamilyRelationshipSpouse)
        //            {
        //                //ldecFedTax = busPayeeAccountHelper.CalculateFlatTax(ldecTaxableAmount, ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
        //                //    ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value, ibusPayeeAccount.icdoPayeeAccount.account_relation_value,
        //                //    icdoPaymentHistoryHeader.payment_date, lobjFedTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_identifier_value,
        //                //    lstrPlsoFlag, ibusPayeeAccount.icdoPayeeAccount.family_relation_value, busConstant.Flag_No);
        //            }
        //            else
        //            {
        //                //ldecFedTax = busPayeeAccountHelper.CalculateFlatTax(ldecTaxableAmount, ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
        //                //   ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value, ibusPayeeAccount.icdoPayeeAccount.account_relation_value,
        //                //   icdoPaymentHistoryHeader.payment_date, lobjFedTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_identifier_value,
        //                //   lstrPlsoFlag, null, busConstant.Flag_No);
        //            }
        //            int lintFedTaxPaymentItemTypeID = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITRefundFederalTaxAmount);
        //            ldecFedTax = Math.Round(ldecFedTax, 2, MidpointRounding.AwayFromZero);
        //            CreatePaymentHistoryDetail(lintFedTaxPaymentItemTypeID, ldecFedTax, busPayeeAccountHelper.GetFedTaxVendorID());
        //        }
        //        if (ibusPayeeAccount.iclbPayeeAccountStateTaxWithHolding == null)
        //            ibusPayeeAccount.LoadStateTaxWithHoldingInfo();
        //        //get the state tax taxwithholding details for the payment
        //        busPayeeAccountTaxWithholding lobjStateTaxWithHoldingInfo
        //          = ibusPayeeAccount.iclbPayeeAccountStateTaxWithHolding.Where(o => busGlobalFunctions.CheckDateOverlapping(icdoPaymentHistoryHeader.payment_date,
        //              o.icdoPayeeAccountTaxWithholding.start_date, o.icdoPayeeAccountTaxWithholding.end_date)).FirstOrDefault();

        //        if (lobjStateTaxWithHoldingInfo != null)
        //        {
        //            decimal ldecStateTax=0.0m;
        //            if (ibusPayeeAccount.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipBeneficiary &&
        //        ibusPayeeAccount.icdoPayeeAccount.family_relation_value == busConstant.FamilyRelationshipSpouse)
        //            {
        //                //ldecStateTax = busPayeeAccountHelper.CalculateFlatTax(ldecTaxableAmount, ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
        //                //    ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value, ibusPayeeAccount.icdoPayeeAccount.account_relation_value,
        //                //    icdoPaymentHistoryHeader.payment_date, lobjStateTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_identifier_value,
        //                //    ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plso_requested_flag, ibusPayeeAccount.icdoPayeeAccount.family_relation_value, busConstant.Flag_No);
        //            }
        //            else
        //            {
        //                //ldecStateTax = busPayeeAccountHelper.CalculateFlatTax(ldecTaxableAmount, ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value,
        //                //    ibusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value, ibusPayeeAccount.icdoPayeeAccount.account_relation_value,
        //                //    icdoPaymentHistoryHeader.payment_date, lobjStateTaxWithHoldingInfo.icdoPayeeAccountTaxWithholding.tax_identifier_value,
        //                //    ibusPayeeAccount.ibusApplication.icdoBenefitApplication.plso_requested_flag, null, busConstant.Flag_No);
        //            }
        //            int lintStateTaxPaymentItemTypeID = ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITRefundNDStateTaxAmount);
        //            ldecStateTax = Math.Round(ldecStateTax, 2, MidpointRounding.AwayFromZero);
        //            CreatePaymentHistoryDetail(lintStateTaxPaymentItemTypeID, ldecStateTax, busPayeeAccountHelper.GetStateTaxVendorID());
        //        }
        //    }
        //}

        //Calculate tax for the check which is reissued to member
        public void CalculateTaxForExcessTaxableContrbutionAmountToMember(decimal adecTaxableAmt, int aintBatchScheduleID)
        {
            if (adecTaxableAmt > 0)
            {
                decimal ldecStateTax = 0;
                decimal ldecFedTax = 0;

                ldecStateTax = busPayeeAccountHelper.CalculateFlatTax(adecTaxableAmt, icdoPaymentHistoryHeader.payment_date, busConstant.PayeeAccountTaxIdentifierStateTax, busConstant.PayeeAccountTaxRefState22Tax, busConstant.Flag_No);
                int lintStateTaxPaymentItemTypeID = GetPaymentItemTypeIDByItemCode(busConstant.PaymentItemTypeCodeITEM504);
                CreatePaymentHistoryDetail(lintStateTaxPaymentItemTypeID, Math.Round(ldecStateTax, 2, MidpointRounding.AwayFromZero), aintBatchScheduleID, busConstant.VendorOrgIDForState);

                ldecFedTax = busPayeeAccountHelper.CalculateFlatTax(adecTaxableAmt, icdoPaymentHistoryHeader.payment_date, busConstant.PayeeAccountTaxIdentifierFedTax, busConstant.PayeeAccountTaxRefFed22Tax, busConstant.Flag_Yes);
                int lintFedTaxPaymentItemTypeID = GetPaymentItemTypeIDByItemCode(busConstant.PaymentItemTypeCodeITEM503);
                CreatePaymentHistoryDetail(lintFedTaxPaymentItemTypeID, Math.Round(ldecFedTax, 2, MidpointRounding.AwayFromZero), aintBatchScheduleID, busConstant.VendorOrgIDForFed);

            }
        }
        //Get payment item type id by passing Item code
        private Collection<busPaymentItemType> _iclbPaymentItemType;
        public Collection<busPaymentItemType> iclbPaymentItemType
        {
            get { return _iclbPaymentItemType; }
            set { _iclbPaymentItemType = value; }
        }
        public DataTable idtbPaymentItemType { get; set; }

        public int GetPaymentItemTypeIDByItemCode(string astrItemTypeCode)
        {
            int lintPaymentItemTypeID = 0;
            if (idtbPaymentItemType == null)
            {
                idtbPaymentItemType = iobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_item_type", null);
            }
            _iclbPaymentItemType = GetCollection<busPaymentItemType>(idtbPaymentItemType, "icdoPaymentItemType");
            lintPaymentItemTypeID = _iclbPaymentItemType.Where(o => o.icdoPaymentItemType.item_type_code == astrItemTypeCode).Select(o => o.icdoPaymentItemType.payment_item_type_id).FirstOrDefault();
            return lintPaymentItemTypeID;
        }
        public void CreatePaymentHistoryDetail(int aintPaymentItemTypeId, decimal ldecAmount, int aintBatchScheduleID, int aintVendorOrgID = 0)
        {
            cdoPaymentHistoryDetail lobjPaymentHistoryDetail = new cdoPaymentHistoryDetail();
            lobjPaymentHistoryDetail.payment_history_header_id = icdoPaymentHistoryHeader.payment_history_header_id;
            lobjPaymentHistoryDetail.payment_item_type_id = aintPaymentItemTypeId;
            lobjPaymentHistoryDetail.amount = ldecAmount;
            lobjPaymentHistoryDetail.created_by = busConstant.PERSLinkBatchUser + ' ' + aintBatchScheduleID;
            lobjPaymentHistoryDetail.modified_by = busConstant.PERSLinkBatchUser + ' ' + aintBatchScheduleID;
            lobjPaymentHistoryDetail.vendor_org_id = aintVendorOrgID;
            lobjPaymentHistoryDetail.Insert();
        }

        public bool IsPaymentTypeVendor()
        {
            if (ibusPaymentSchedule == null)
                LoadPaymentSchedule();
            if (ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id > 0 &&
                !(ibusPaymentSchedule.icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeAdhoc
                || ibusPaymentSchedule.icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeMonthly)
                && icdoPaymentHistoryHeader.org_id > 0)
            {
                return true;
            }
            return false;
        }
        public busRemittance ibusRemittance { get; set; }
        public void LoadRemittance()
        {
            if (ibusRemittance == null)
            {
                ibusRemittance = new busRemittance();
                ibusRemittance.icdoRemittance = new cdoRemittance();
            }
            DataTable ldtbRemittance = Select<cdoRemittance>(new string[1] { "payment_history_header_id" },
                new object[1] { icdoPaymentHistoryHeader.payment_history_header_id }, null, null);
            if (ldtbRemittance.Rows.Count > 0)
            {
                ibusRemittance.icdoRemittance.LoadData(ldtbRemittance.Rows[0]);
            }
        }
        //PIR-10786 Start
        public Collection<busRemittance> iclbRemittances { get; set; }
        public void LoadRemittances()
        {
            DataTable ldtbRemittance = Select<cdoRemittance>(
                new string[1] { "payment_history_header_id" },
                new object[1] { icdoPaymentHistoryHeader.payment_history_header_id }, null, null);
            iclbRemittances = GetCollection<busRemittance>(ldtbRemittance, "icdoRemittance");   
        }
        //PIR-10786 End

        //pir 8530
        public int CreateDepositTape()
        {
            busDepositTape lobjDepositTape = new busDepositTape() { icdoDepositTape = new cdoDepositTape()};
            lobjDepositTape.icdoDepositTape.status_value = busConstant.DepositTapeStatusValid;
            lobjDepositTape.icdoDepositTape.deposit_method_value = ibusRemittance.ibusDeposit.ibusDepositTape.icdoDepositTape.deposit_method_value;
            lobjDepositTape.icdoDepositTape.deposit_date = DateTime.Today;
            lobjDepositTape.icdoDepositTape.bank_account_value = ibusRemittance.ibusDeposit.ibusDepositTape.icdoDepositTape.bank_account_value;
            lobjDepositTape.icdoDepositTape.total_amount = ibusRemittance.icdoRemittance.overridden_refund_amount > 0 ? 
                                                           ibusRemittance.icdoRemittance.overridden_refund_amount : ibusRemittance.icdoRemittance.computed_refund_amount;
            lobjDepositTape.icdoDepositTape.Insert();
            return lobjDepositTape.icdoDepositTape.deposit_tape_id;
        }

        public int CreateDeposit(int aintDepositTapeID)
        {
            busDeposit lobjDeposit = new busDeposit() { icdoDeposit = new cdoDeposit()};
            busDepositTape lobjDepositTape = new busDepositTape() { icdoDepositTape = new cdoDepositTape() };
            lobjDepositTape.FindDepositTape(aintDepositTapeID);
            lobjDeposit.icdoDeposit.org_id = ibusRemittance.ibusDeposit.icdoDeposit.org_id;
            lobjDeposit.icdoDeposit.person_id = ibusRemittance.ibusDeposit.icdoDeposit.person_id;
            lobjDeposit.icdoDeposit.reference_no = ibusRemittance.ibusDeposit.icdoDeposit.reference_no;
            lobjDeposit.icdoDeposit.deposit_amount = ibusRemittance.icdoRemittance.overridden_refund_amount > 0 ? 
                                                           ibusRemittance.icdoRemittance.overridden_refund_amount : ibusRemittance.icdoRemittance.computed_refund_amount;
            lobjDeposit.icdoDeposit.status_value = busConstant.DepositTapeStatusValid;
            lobjDeposit.icdoDeposit.deposit_source_value = ibusRemittance.ibusDeposit.icdoDeposit.deposit_source_value;
            lobjDeposit.icdoDeposit.deposit_tape_id = aintDepositTapeID;
            lobjDeposit.icdoDeposit.payment_date = DateTime.Today;
            lobjDeposit.icdoDeposit.deposit_date = aintDepositTapeID != 0 ? lobjDepositTape.icdoDepositTape.deposit_date : DateTime.Today;
            lobjDeposit.icdoDeposit.Insert();
            return lobjDeposit.icdoDeposit.deposit_id;
        }

        public void CreateRemittance(int aintDepositID)
        {
            busRemittance lobjRemittance = new busRemittance() { icdoRemittance = new cdoRemittance() };
            lobjRemittance.icdoRemittance.deposit_id = aintDepositID;
            lobjRemittance.icdoRemittance.person_id = ibusRemittance.icdoRemittance.person_id;
            lobjRemittance.icdoRemittance.org_id = ibusRemittance.icdoRemittance.org_id;
            lobjRemittance.icdoRemittance.remittance_type_value = ibusRemittance.icdoRemittance.remittance_type_value;
            lobjRemittance.icdoRemittance.remittance_amount = ibusRemittance.icdoRemittance.overridden_refund_amount > 0 ? 
                                                           ibusRemittance.icdoRemittance.overridden_refund_amount : ibusRemittance.icdoRemittance.computed_refund_amount;
            lobjRemittance.icdoRemittance.plan_id = ibusRemittance.icdoRemittance.plan_id;
            lobjRemittance.icdoRemittance.Insert();
        }
        //Used for UCS-78 correspondence
        public decimal idecRolledoverTaxableAmount { get; set; }
        public decimal idecRolledOverNonTaxableAmount { get; set; }

        #region Methods for Correpondence
        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                LoadPerson();
            //in case of rollover
            if (icdoPaymentHistoryHeader.org_id > 0)
            {
                if (ibusOrganization == null)
                    LoadOrganization();
                if (ibusOrganization.icdoOrganization.org_type_value == busConstant.OrgTypeRollover)
                {
                    if (ibusPayeeAccount == null)
                        LoadPayeeAccount();
                    if (ibusPayeeAccount.ibusPayee == null)
                        ibusPayeeAccount.LoadPayee();
                    ibusPerson = ibusPayeeAccount.ibusPayee;
                }
            }
            return ibusPerson;
        }
        public override busBase GetCorOrganization()
        {
            if (ibusOrganization == null)
                LoadOrganization();
            return ibusOrganization;
        }
        public void LoadRolloverAmounts()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.iclbPaymentItemType == null)
                ibusPayeeAccount.LoadPaymentItemType();
            if (ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            if (iclbPaymentHistoryDetail == null)
                LoadPaymentHistoryDetails();


            idecRolledoverTaxableAmount = (from lobjHistory in iclbPaymentHistoryDetail
                                           join lobjPIT in ibusPayeeAccount.iclbPaymentItemType
                                             on lobjHistory.icdoPaymentHistoryDetail.payment_item_type_id
                                                 equals lobjPIT.icdoPaymentItemType.payment_item_type_id
                                           join lobjPIT1 in ibusPayeeAccount.iclbPaymentItemType
                                            on lobjPIT.icdoPaymentItemType.item_type_code
                                                 equals lobjPIT1.icdoPaymentItemType.rollover_item_code
                                           where lobjPIT1.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes
                                           select lobjHistory.icdoPaymentHistoryDetail.amount).Sum();


            idecRolledOverNonTaxableAmount = (from lobjHistory in iclbPaymentHistoryDetail
                                              join lobjPIT in ibusPayeeAccount.iclbPaymentItemType
                                                on lobjHistory.icdoPaymentHistoryDetail.payment_item_type_id
                                                    equals lobjPIT.icdoPaymentItemType.payment_item_type_id
                                              join lobjPIT1 in ibusPayeeAccount.iclbPaymentItemType
                                               on lobjPIT.icdoPaymentItemType.item_type_code
                                                    equals lobjPIT1.icdoPaymentItemType.rollover_item_code
                                              where (lobjPIT1.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_No && 
                                                        (
                                                            //PIR 25283 - Roth Rollovers Which Will have Taxes is being included in nontaxable amount
                                                            lobjPIT1.icdoPaymentItemType.item_type_code != busConstant.PAPITPLSORothRolloverStateTaxAmount &&
                                                            lobjPIT1.icdoPaymentItemType.item_type_code != busConstant.PAPITRothRolloverStateTaxAmount &&
                                                            lobjPIT1.icdoPaymentItemType.item_type_code != busConstant.PAPITPLSORothRolloverFederalTaxAmount &&
                                                            lobjPIT1.icdoPaymentItemType.item_type_code != busConstant.PAPITRothRolloverFederalTaxAmount
                                                        )
                                                    )
                                              select lobjHistory.icdoPaymentHistoryDetail.amount).Sum();

            IEnumerable<busPaymentHistoryDetail> lclbRothStateTaxPaymentHistoryDetails = iclbPaymentHistoryDetail.Where(detail => (detail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSORothRolloverStateTaxAmount ||
                                                                                        detail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRothRolloverStateTaxAmount));
            if (lclbRothStateTaxPaymentHistoryDetails.IsNotNull())
                idecRolledOverStateTaxAmount = lclbRothStateTaxPaymentHistoryDetails.Sum(detail => detail.icdoPaymentHistoryDetail.amount);

            IEnumerable<busPaymentHistoryDetail> lclbRothFedTaxPaymentHistoryDetails = iclbPaymentHistoryDetail.Where(detail => (detail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSORothRolloverFederalTaxAmount ||
                                                                                    detail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRothRolloverFederalTaxAmount));
            if (lclbRothFedTaxPaymentHistoryDetails.IsNotNull())
                idecRolledOverFederalTaxAmount = lclbRothFedTaxPaymentHistoryDetails.Sum(detail => detail.icdoPaymentHistoryDetail.amount);



        }
        public bool IsRothRollverPayment
        {
            get
            {
                if (iclbPaymentHistoryDetail.IsNull()) LoadPaymentHistoryDetails();
                return iclbPaymentHistoryDetail.Any(detail => (detail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSORothRolloverStateTaxAmount ||
                                                                detail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRothRolloverStateTaxAmount ||
                                                                detail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSORothRolloverFederalTaxAmount ||
                                                                detail.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRothRolloverFederalTaxAmount));
            }
        } 
        //Property used in batch correspondence
        public string istrIsRolloverincludeNontaxableAmount
        {
            get
            {
                LoadRolloverAmounts();
                if (idecRolledOverNonTaxableAmount > 0.00m)
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }
        #endregion

        #region UCS - 079

        /// <summary>
        /// Method to load 1099r bus object
        /// </summary>
        public void LoadPayment1099r(int aintYear)
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            ibusPayeeAccount.LoadLatest1099rByPayeeAccount(aintYear);            
        }

        /// <summary>
        /// Method to get the amount paid for one header id
        /// </summary>
        /// <returns>Amount Paid</returns>
        public decimal GetAmountPaid()
        {
            decimal ldecAmountPaid = 0.0M;
            if (iclbPaymentHistoryDetail == null)
                LoadPaymentHistoryDetails();
            ldecAmountPaid = iclbPaymentHistoryDetail.Where(o => o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                (o.ibusPaymentItemType.icdoPaymentItemType.base_amount_flag == busConstant.Flag_Yes ||
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITSupplementalItem) && //uat pir 1375
                string.IsNullOrEmpty(o.ibusPaymentItemType.icdoPaymentItemType.retro_payment_type_value))
                .Sum(o => o.icdoPaymentHistoryDetail.amount);
            return ldecAmountPaid;
        }

        //Property to contain current BenefitOverpayment without any recovery
        public busPaymentBenefitOverpaymentHeader ibusBenefitOverpaymentHeader { get; set; }
        /// <summary>
        /// Method to load Benefit overpayment header
        /// </summary>
        public void LoadBenfitOverpaymentHeader()
        {
            if (ibusBenefitOverpaymentHeader == null)
                ibusBenefitOverpaymentHeader = new busPaymentBenefitOverpaymentHeader();
            DataTable ldtBenefitOverpayment = Select<cdoPaymentBenefitOverpaymentHeader>
                (new string[1] { enmPaymentBenefitOverpaymentHeader.payee_account_id.ToString() },
                new object[1] { icdoPaymentHistoryHeader.payee_account_id }, null, null);
            Collection<busPaymentBenefitOverpaymentHeader> lclbBenefitOverpayment =
                GetCollection<busPaymentBenefitOverpaymentHeader>(ldtBenefitOverpayment, "icdoPaymentBenefitOverpaymentHeader");
            foreach (busPaymentBenefitOverpaymentHeader lobjHeader in lclbBenefitOverpayment)
            {
                if (lobjHeader.IsRecoveryNotAvailble())
                    ibusBenefitOverpaymentHeader = lobjHeader;
            }
        }
        //Prop to indicate payee account cancelled
        public bool iblnIsPayeeAccountCancelled { get; set; }

        public Collection<busPayeeAccountMonthwiseAdjustmentDetail> iclbMonthwiseAdjustmentDetails { get; set; }

        /// <summary>
        /// Method to check and create any overpayment
        /// </summary>
        public void CreateBenefitOverPayment(string astrStatus)
        {
            if (astrStatus == busConstant.HistoryHeaderStatusReceivableCreated || IsPaymentDetailExistsForBenefitoverpaymentHeader())
            {
                if (ibusPayeeAccount == null)
                    LoadPayeeAccount();
                if (ibusBenefitOverpaymentHeader == null)
                    LoadBenfitOverpaymentHeader();
                if (ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader == null)
                {
                    ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader = new cdoPaymentBenefitOverpaymentHeader();
                    ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.payee_account_id = icdoPaymentHistoryHeader.payee_account_id;
                    ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.adjustment_reason_value = busConstant.BenRecalAdjustmentReceivableCreated;
                    ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.Insert();
                }
                if (!iblnIsPayeeAccountCancelled)
                    CreateBenefitOverpaymentDetails(astrStatus);
                else
                    CreateBenefitOverpaymentDetailsWhenPayeeAccountCancelled(astrStatus);
            }
            else if (astrStatus == busConstant.HistoryHeaderStatusCancel && iblnProviderReportPosting)
            {
                PostIntoProviderReportPayment();
            }
        }

        /// <summary>
        /// Method to create Monthly adjustment details
        /// </summary>
        public void CreateMonthlyAdjustmentDetails()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            decimal ldecAmountPaidFTM = ibusPayeeAccount.GetTotalAmountPaidForTheMonth(icdoPaymentHistoryHeader.payment_date,
                icdoPaymentHistoryHeader.payment_date,
                icdoPaymentHistoryHeader.payment_history_header_id,0);
            ibusBenefitOverpaymentHeader.CreateMonthwiseAdjustmentDetail(icdoPaymentHistoryHeader.payment_date,
                ldecAmountPaidFTM, 0, ibusPayeeAccount.icdoPayeeAccount.payee_account_id);
        }

        /// <summary>
        /// Method to create 1099r items
        /// </summary>
        /// <param name="lintOverpaymentID"></param>
        public void CreateBenefitOverpaymentDetails(string astrStatus)
        {
            //Loading payment history details for the header
            if (iclbPaymentHistoryDetail == null)
                LoadPaymentHistoryDetails();
            if (ibusPayeeAccount.ibusPayment1099r == null)
                LoadPayment1099r(icdoPaymentHistoryHeader.payment_date.Year);
            //if 1099r doesn't exists for the payee for the year in which payment is done
            IEnumerable<busPaymentHistoryDetail> lenmPaymentHistoryDetail;
            //If history status is changed to Receivables Created
            if (astrStatus == busConstant.HistoryHeaderStatusReceivableCreated)
            {
                if (ibusPayeeAccount.ibusPayment1099r.icdoPayment1099r.payment_1099r_id <= 0)
                {
                    //prod pir 5142
                    lenmPaymentHistoryDetail = iclbPaymentHistoryDetail
                                                 .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.receivable_creation_1099r_value == busConstant.ReceivableCreatedWithOrWithout1099r ||
                                                                o.ibusPaymentItemType.icdoPaymentItemType.receivable_creation_1099r_value == busConstant.ReceivableCreatedWithout1099r);
                }
                //if 1099r exists for the payee for the year in which payment is done
                else
                {
                    //prod pir 5142
                    lenmPaymentHistoryDetail = iclbPaymentHistoryDetail
                                            .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.receivable_creation_1099r_value == busConstant.ReceivableCreatedWithOrWithout1099r);
                }
                //Creating Monthly Adjustment Detail
                CreateMonthlyAdjustmentDetails();
                //Creating overpayment items for 1099r
                CreateBenefitOverpaymentDetail(lenmPaymentHistoryDetail,astrStatus,icdoPaymentHistoryHeader.payment_date);
            }
            //If history status is changed to Cancelled
            else if (astrStatus == busConstant.HistoryHeaderStatusCancel)
            {
                if (ibusPayeeAccount.ibusPayment1099r.icdoPayment1099r.payment_1099r_id > 0)
                {
                    lenmPaymentHistoryDetail = iclbPaymentHistoryDetail
                                                 .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWith1099r ||
                                                 o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWithOrWithout1099r);
                    //Creating overpayment items for 1099r
                    CreateBenefitOverpaymentDetail(lenmPaymentHistoryDetail, astrStatus, icdoPaymentHistoryHeader.payment_date);
                }
                else
                {
                    lenmPaymentHistoryDetail = iclbPaymentHistoryDetail
                                                .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWithout1099r ||
                                                o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWithOrWithout1099r);
                    //Creating overpayment items for 1099r
                    CreateBenefitOverpaymentDetail(lenmPaymentHistoryDetail, astrStatus, icdoPaymentHistoryHeader.payment_date);

                    IEnumerable<busPaymentHistoryDetail> lenmDetailTobePostedInProviderReport = iclbPaymentHistoryDetail
                                                 .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWith1099r);
                    //Posting into provider report payment
                    PostIntoProviderReportPayment(lenmDetailTobePostedInProviderReport);
                }
            }
        }

        public void PostIntoProviderReportPayment()
        {
            if (iblnIsPayeeAccountCancelled)
            {
                IEnumerable<int> lenm1099rYears = (from lobjPaymentHistoryDetail in iclbPaymentHistoryDetail
                                               orderby lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.payment_date.Year descending
                                               select lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.payment_date.Year).Distinct();

                //Loading payment history details for the header            
                foreach (int lintYear in lenm1099rYears)
                {
                    DateTime ldtPaymentDate = DateTime.MinValue;
                    if (lintYear == DateTime.Today.Year)
                        ldtPaymentDate = DateTime.Today;
                    else
                        ldtPaymentDate = new DateTime(lintYear, 12, 31);
                    LoadPayment1099r(lintYear);
                    if (ibusPayeeAccount.ibusPayment1099r.icdoPayment1099r.payment_1099r_id == 0)
                    {
                        IEnumerable<busPaymentHistoryDetail> lenmDetailToBePostedIntoProviderReport = iclbPaymentHistoryDetail
                                                         .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWith1099r &&
                                                             o.icdoPaymentHistoryDetail.payment_date.Year == lintYear);
                        IEnumerable<IGrouping<int, busPaymentHistoryDetail>> lgrpDetailGrouped = lenmDetailToBePostedIntoProviderReport.AsEnumerable()
                                                                .GroupBy(o => o.icdoPaymentHistoryDetail.payment_item_type_id);
                        PostIntoProviderReportPayment(lgrpDetailGrouped,ldtPaymentDate);
                    }
                }
            }
            else
            {
                IEnumerable<busPaymentHistoryDetail> lenmDetailTobePostedInProviderReport = iclbPaymentHistoryDetail
                                                               .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWith1099r);
                //Posting into provider report payment
                PostIntoProviderReportPayment(lenmDetailTobePostedInProviderReport);
            }
        }

        /// <summary>
        /// method to post into provider report payment
        /// </summary>
        /// <param name="aenmDetailTobePostedInProviderReport">payment history detail items to be  posted</param>
        public void PostIntoProviderReportPayment(IEnumerable<busPaymentHistoryDetail> aenmDetailTobePostedInProviderReport)
        {
            busProviderReportPayment lobjProviderReport = new busProviderReportPayment { icdoProviderReportPayment = new cdoProviderReportPayment() };
            foreach (busPaymentHistoryDetail lobjDetail in aenmDetailTobePostedInProviderReport)
            {
                if (lobjDetail.ibusPaymentItemType == null)
                    lobjDetail.LoadPaymentItemType();
                DataTable ldtCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(2518);
                DataTable ldtFiltered = ldtCodeValue.AsEnumerable()
                                                    .Where(o => o.Field<string>("data1") == lobjDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code)
                                                    .AsDataTable();
                string lstrItemCode = ldtFiltered.Rows.Count > 0 ? ldtFiltered.Rows[0]["data2"].ToString() : string.Empty;
                DataTable ldtPaymentItemType = iobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_item_type", "item_type_code = '" + lstrItemCode + "'");

                // PIR 18547 - added check so Vendor Org id passed as zero is greater thean zero for org headers for roth Rollover tax items.
                if (ldtPaymentItemType.Rows.Count > 0 && lobjDetail.icdoPaymentHistoryDetail.vendor_org_id > 0)
                {
                    lobjProviderReport.CreateProviderReportPayment(busConstant.SubSystemValuePensionRecv,//todo check with ragavan whether subsystem value is correct
                                                                    lobjDetail.icdoPaymentHistoryDetail.payment_history_detail_id,
                                                                    ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                                                    lobjDetail.icdoPaymentHistoryDetail.vendor_org_id,
                                                                    ibusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                                    icdoPaymentHistoryHeader.payment_date,
                                                                    lobjDetail.icdoPaymentHistoryDetail.amount,
                                                                    0, Convert.ToInt32(ldtPaymentItemType.Rows[0]["payment_item_type_id"]));
                }
            }
        }

        /// <summary>
        /// Method to create Benefit Overpayment details when payee account is cancelled
        /// </summary>
        /// <param name="astrStatus"></param>
        public void CreateBenefitOverpaymentDetailsWhenPayeeAccountCancelled(string astrStatus)
        {
            IEnumerable<int> lenm1099rYears = (from lobjPaymentHistoryDetail in iclbPaymentHistoryDetail
                                               orderby lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.payment_date.Year descending
                                               select lobjPaymentHistoryDetail.icdoPaymentHistoryDetail.payment_date.Year).Distinct();

            //Loading payment history details for the header            
            foreach (int lintYear in lenm1099rYears)
            {
                LoadPayment1099r(lintYear);
                //if 1099r doesn't exists for the payee for the year in which payment is done
                IEnumerable<busPaymentHistoryDetail> lenmPaymentHistoryDetail;
                IEnumerable<IGrouping<int, busPaymentHistoryDetail>> lgrpHistoryDetail;
                //If history status is changed to Receivables Created
                DateTime ldtPaymentDate = DateTime.MinValue;
                if (lintYear == DateTime.Today.Year)
                    ldtPaymentDate = DateTime.Today;
                else
                    ldtPaymentDate = new DateTime(lintYear, 12, 31);
                if (astrStatus == busConstant.HistoryHeaderStatusReceivableCreated)
                {
                    if (ibusPayeeAccount.ibusPayment1099r.icdoPayment1099r.payment_1099r_id <= 0)
                    {
                        //prod pir 5142
                        lenmPaymentHistoryDetail = iclbPaymentHistoryDetail.Where(o =>
                                                         (o.ibusPaymentItemType.icdoPaymentItemType.receivable_creation_1099r_value == busConstant.ReceivableCreatedWithOrWithout1099r ||
                                                         o.ibusPaymentItemType.icdoPaymentItemType.receivable_creation_1099r_value == busConstant.ReceivableCreatedWithout1099r) &&
                                                         o.icdoPaymentHistoryDetail.payment_date.Year == lintYear);
                    }
                    //if 1099r exists for the payee for the year in which payment is done
                    else
                    {
                        //prod pir 5142
                        lenmPaymentHistoryDetail = iclbPaymentHistoryDetail
                                .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.receivable_creation_1099r_value == busConstant.ReceivableCreatedWithOrWithout1099r &&
                                o.icdoPaymentHistoryDetail.payment_date.Year == lintYear);
                    }
                    //Grouping the items based on payment item type id
                    lgrpHistoryDetail = lenmPaymentHistoryDetail.AsEnumerable()
                                                                .GroupBy(o => o.icdoPaymentHistoryDetail.payment_item_type_id);     
                    //Creating monthwise adjustment details
                    CreateMonthwiseAdjustmentWhenPayeeAccountCancelled();
                    //Creating overpayment items for 1099r
                    CreateBenefitOverpaymentDetail(lgrpHistoryDetail, astrStatus, ldtPaymentDate);
                }
                //If history status is changed to Cancelled
                else if (astrStatus == busConstant.HistoryHeaderStatusCancel)
                {
                    if (ibusPayeeAccount.ibusPayment1099r.icdoPayment1099r.payment_1099r_id > 0)
                    {
                        lenmPaymentHistoryDetail = iclbPaymentHistoryDetail
                                                     .Where(o => (o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWith1099r ||
                                                         o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWithOrWithout1099r) &&
                                                         o.icdoPaymentHistoryDetail.payment_date.Year == lintYear);
                        //Grouping the items based on payment item type id
                        lgrpHistoryDetail = lenmPaymentHistoryDetail.AsEnumerable()
                                                                .GroupBy(o => o.icdoPaymentHistoryDetail.payment_item_type_id);
                        //Creating overpayment items for 1099r
                        CreateBenefitOverpaymentDetail(lgrpHistoryDetail, astrStatus, ldtPaymentDate);
                    }
                    else
                    {
                        lenmPaymentHistoryDetail = iclbPaymentHistoryDetail
                                                    .Where(o => (o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWithout1099r ||
                                                        o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWithOrWithout1099r) &&
                                                        o.icdoPaymentHistoryDetail.payment_date.Year == lintYear);
                        //Grouping the items based on payment item type id
                        lgrpHistoryDetail = lenmPaymentHistoryDetail.AsEnumerable()
                                                                .GroupBy(o => o.icdoPaymentHistoryDetail.payment_item_type_id);
                        //Creating overpayment items for 1099r
                        CreateBenefitOverpaymentDetail(lgrpHistoryDetail, astrStatus, ldtPaymentDate);

                        IEnumerable<busPaymentHistoryDetail> lenmDetailToBePostedIntoProviderReport = iclbPaymentHistoryDetail
                                                     .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.cancel_payment_1099r_value == busConstant.CancelPaymentWith1099r &&
                                                         o.icdoPaymentHistoryDetail.payment_date.Year == lintYear);
                        IEnumerable<IGrouping<int, busPaymentHistoryDetail>> lgrpDetailGrouped = lenmDetailToBePostedIntoProviderReport.AsEnumerable()
                                                                .GroupBy(o => o.icdoPaymentHistoryDetail.payment_item_type_id);
                        PostIntoProviderReportPayment(lgrpDetailGrouped,ldtPaymentDate);
                    }
                }
            }
        }

        /// <summary>
        /// method to post into provider report payment
        /// </summary>
        /// <param name="lgrpDetailGrouped">grouped payment history detail items to be posted</param>
        private void PostIntoProviderReportPayment(IEnumerable<IGrouping<int, busPaymentHistoryDetail>> lgrpDetailGrouped, DateTime adtPaymentDate)
        {
            busProviderReportPayment lobjProviderReport = new busProviderReportPayment { icdoProviderReportPayment = new cdoProviderReportPayment() };
            foreach (IGrouping<int, busPaymentHistoryDetail> lobjGroupDetail in lgrpDetailGrouped)
            {
                decimal ldecAmount = lobjGroupDetail.Sum(o => o.icdoPaymentHistoryDetail.amount);
                busPaymentHistoryDetail lobjDetail = lobjGroupDetail.FirstOrDefault();

                if (lobjDetail.ibusPaymentItemType == null)
                    lobjDetail.LoadPaymentItemType();
                DataTable ldtCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(2518);
                DataTable ldtFiltered = ldtCodeValue.AsEnumerable()
                                                    .Where(o => o.Field<string>("data1") == lobjDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code)
                                                    .AsDataTable();
                string lstrItemCode = ldtFiltered.Rows.Count > 0 ? ldtFiltered.Rows[0]["data2"].ToString() : string.Empty;
                DataTable ldtPaymentItemType = iobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_item_type", "item_type_code = '" + lstrItemCode + "'");
                if (ldtPaymentItemType.Rows.Count > 0)
                {
                    lobjProviderReport.CreateProviderReportPayment(busConstant.SubSystemValuePensionRecv,//todo check with ragavan whether subsystem value is correct
                                                                    lobjDetail.icdoPaymentHistoryDetail.payment_history_detail_id,
                                                                    ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id,
                                                                    lobjDetail.icdoPaymentHistoryDetail.vendor_org_id,
                                                                    ibusPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                                    adtPaymentDate,
                                                                    ldecAmount,
                                                                    0, Convert.ToInt32(ldtPaymentItemType.Rows[0]["payment_item_type_id"]));
                }
            }
        }

        /// <summary>
        /// Method to create Monthly adjustment details
        /// </summary>
        private void CreateMonthwiseAdjustmentWhenPayeeAccountCancelled()
        {
            foreach (busPayeeAccountMonthwiseAdjustmentDetail lobjAdjustment in iclbMonthwiseAdjustmentDetails)
            {
                lobjAdjustment.icdoPayeeAccountMonthwiseAdjustmentDetail.benefit_overpayment_id =
                    ibusBenefitOverpaymentHeader.icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id;
                lobjAdjustment.icdoPayeeAccountMonthwiseAdjustmentDetail.Insert();
            }
        }

        /// <summary>
        /// Method to create Benefit overpayment detail
        /// </summary>
        /// <param name="lgrpHistoryDetail">filtered and grouped collection of payment history detail</param>
        /// <param name="astrStatus">payment history status</param>
        /// <param name="ldtPaymentDate">payment date</param>
        private void CreateBenefitOverpaymentDetail(IEnumerable<IGrouping<int, busPaymentHistoryDetail>> agrpHistoryDetail, string astrStatus, DateTime adtPaymentDate)
        {
            decimal ldecAmount = 0.0M;
            //iterating through the groups
            foreach (IGrouping<int, busPaymentHistoryDetail> lobjGroup in agrpHistoryDetail)
            {
                ldecAmount = 0.0M;
                //taking sum of amount from first gorup and inserting into Benefit overpayment details
                ldecAmount = lobjGroup.Sum(o => o.icdoPaymentHistoryDetail.amount);
                if (lobjGroup.FirstOrDefault().ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_Yes &&
                    astrStatus == busConstant.HistoryHeaderStatusCancel)
                {
                    ibusBenefitOverpaymentHeader.CreateBenefitOverpaymentDetail(ldecAmount * -1,
                                                                        lobjGroup.FirstOrDefault().icdoPaymentHistoryDetail.payment_item_type_id,
                                                                        adtPaymentDate,
                                                                        lobjGroup.FirstOrDefault().icdoPaymentHistoryDetail.vendor_org_id);
                }
                else
                {
                    if (lobjGroup.FirstOrDefault().ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_Yes)
                        ibusBenefitOverpaymentHeader.CreateBenefitOverpaymentDetail(ldecAmount,
                                                                           lobjGroup.FirstOrDefault().icdoPaymentHistoryDetail.payment_item_type_id,
                                                                           adtPaymentDate,
                                                                           lobjGroup.FirstOrDefault().icdoPaymentHistoryDetail.vendor_org_id);
                    else
                        ibusBenefitOverpaymentHeader.CreateBenefitOverpaymentDetail(ldecAmount,
                                                                       lobjGroup.FirstOrDefault().icdoPaymentHistoryDetail.payment_item_type_id,
                                                                       adtPaymentDate,0);
                }
            }
        }

        /// <summary>
        /// Method to create Benefit overpayment detail
        /// </summary>
        /// <param name="aenmPaymentHistoryDetail">Filtered collection of payment history detail</param>
        /// <param name="astrStatus">Payment history status</param>
        /// <param name="adtPaymentDate">Payment date</param>
        private void CreateBenefitOverpaymentDetail(IEnumerable<busPaymentHistoryDetail> aenmPaymentHistoryDetail, string astrStatus,DateTime adtPaymentDate)
        {
            foreach (busPaymentHistoryDetail lobjDetail in aenmPaymentHistoryDetail)
            {
                if (lobjDetail.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_Yes &&
                    astrStatus == busConstant.HistoryHeaderStatusCancel)
                {   //if vendor flag yes, then need to post vendor org id
                    ibusBenefitOverpaymentHeader.CreateBenefitOverpaymentDetail(lobjDetail.icdoPaymentHistoryDetail.amount * -1,
                                                                        lobjDetail.icdoPaymentHistoryDetail.payment_item_type_id,
                                                                        adtPaymentDate,
                                                                        lobjDetail.icdoPaymentHistoryDetail.vendor_org_id);
                }
                else
                {   //if vendor flag yes, then need to post vendor org id
                    if (lobjDetail.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_Yes)
                        ibusBenefitOverpaymentHeader.CreateBenefitOverpaymentDetail(lobjDetail.icdoPaymentHistoryDetail.amount,
                                                                            lobjDetail.icdoPaymentHistoryDetail.payment_item_type_id,
                                                                            adtPaymentDate,
                                                                            lobjDetail.icdoPaymentHistoryDetail.vendor_org_id);
                    else
                        ibusBenefitOverpaymentHeader.CreateBenefitOverpaymentDetail(lobjDetail.icdoPaymentHistoryDetail.amount,
                                                                        lobjDetail.icdoPaymentHistoryDetail.payment_item_type_id,
                                                                        adtPaymentDate,0);
                }
            }
        }        
        #endregion

        public decimal idecRolledOverStateTaxAmount { get; set; }

        public decimal idecRolledOverFederalTaxAmount { get; set; }

        // PIR 26043 remove the recovery history if the payment is canceled
        public Collection<busPaymentRecoveryHistory> iclbPaymentRecoveryHistory { get; set; }
        public void LoadPaymentRecoveryHistoryByPaymentHistoryHeaderId()
        {
            DataTable ldtRecoveryHistory = Select<cdoPaymentRecoveryHistory>
                                            (new string[1] { enmPaymentRecoveryHistory.payment_history_header_id.ToString() },
                                                new object[1] { icdoPaymentHistoryHeader.payment_history_header_id }, null, null);
            iclbPaymentRecoveryHistory = new Collection<busPaymentRecoveryHistory>();
            iclbPaymentRecoveryHistory = GetCollection<busPaymentRecoveryHistory>(ldtRecoveryHistory, "icdoPaymentRecoveryHistory");
            foreach (busPaymentRecoveryHistory lobjRecoveryHistory in iclbPaymentRecoveryHistory)
                lobjRecoveryHistory.LoadPaymentRecovery();
        }
        public void DeleteRecoveryHistory()
        {
            //need to reload everytime this function is called
            LoadPaymentRecoveryHistoryByPaymentHistoryHeaderId();

            //Select the latest recovery history
            busPaymentRecoveryHistory lobjLatestRecoveryHistory = new busPaymentRecoveryHistory { icdoPaymentRecoveryHistory = new cdoPaymentRecoveryHistory() };
            lobjLatestRecoveryHistory = iclbPaymentRecoveryHistory.OrderByDescending(i => i.icdoPaymentRecoveryHistory.recovery_history_id).FirstOrDefault();

            if (lobjLatestRecoveryHistory.IsNotNull())
            {
                if (lobjLatestRecoveryHistory.ibusPaymentRecovery.IsNotNull() &&
                    lobjLatestRecoveryHistory.icdoPaymentRecoveryHistory.payment_history_header_id > 0 &&
                    lobjLatestRecoveryHistory?.ibusPaymentRecovery?.icdoPaymentRecovery?.status_value == busConstant.RecoveryStatusSatisfied)
                {
                    //If the cancelled payment is also the last payment that satisfied the recovery, set it back to In Process.
                    lobjLatestRecoveryHistory.ibusPaymentRecovery.icdoPaymentRecovery.status_value = busConstant.RecoveryStatusInProcess;
                    lobjLatestRecoveryHistory.ibusPaymentRecovery.icdoPaymentRecovery.Update();
                }
                else
                {
                    //Delete the history line
                    if (lobjLatestRecoveryHistory?.icdoPaymentRecoveryHistory?.recovery_history_id > 0)
                        lobjLatestRecoveryHistory.icdoPaymentRecoveryHistory.Delete();
                }
            }
        }
    }
}