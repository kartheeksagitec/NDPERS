#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using System.Linq;
using System.Linq.Expressions;
using Sagitec.Bpm;
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPaymentHistoryDistribution : busPaymentHistoryDistributionGen
    {
        //property to get the net amount in words
        public string istrNetAmountinWords
        {
            get
            {
                if (icdoPaymentHistoryDistribution.net_amount > 0.0m)
                    return busGlobalFunctions.AmountToWords(icdoPaymentHistoryDistribution.net_amount.ToString());
                return string.Empty;
            }
        }
        //prop to Load remittance when when check created for premium refund
        public busRemittance ibusRemittance { get; set; }
        //Set visiblity to Escheat to NDPERS and Escheat to  State Button visible
        public bool IsEscheattoNDPERSAndStateBtnVisible()
        {
            if (ibusDistributionHistory == null)
                LoadLatestDistibutionStatusHistory();
            if (!IsPaymentMethodACHOrRACH() &&
                ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.status_changed_by != iobjPassInfo.istrUserID &&
                (icdoPaymentHistoryDistribution.status_value == busConstant.DistributionStatusStopPayPending ||
                icdoPaymentHistoryDistribution.status_value == busConstant.DistributionStatusVoidPending))
            {
                return true;
            }
            return false;
        }
        // //Set visiblity to Reissue Approved Button visible
        public bool IsReissueApprovedBtnVisible()
        {
            bool lblnIsReissueApprovedBtnVisible = false;
            if (ibusDistributionHistory == null)
                LoadLatestDistibutionStatusHistory();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.ibusPayeeAccount == null)
                ibusPaymentHistoryHeader.LoadPayeeAccount();

            if (!IsPaymentMethodACHOrRACH() &&
                 ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.status_changed_by != iobjPassInfo.istrUserID &&
                (icdoPaymentHistoryDistribution.status_value == busConstant.DistributionStatusStopPayPending ||
                icdoPaymentHistoryDistribution.status_value == busConstant.DistributionStatusVoidPending))
            {
                lblnIsReissueApprovedBtnVisible = IsReissueApproveValid();
            }
            else if (IsPaymentMethodACHOrRACH() &&
                 ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.status_changed_by != iobjPassInfo.istrUserID &&
                icdoPaymentHistoryDistribution.status_value == busConstant.DistributionStatusVoidPending)
            {
                lblnIsReissueApprovedBtnVisible = IsReissueApproveValid();
            }
            return lblnIsReissueApprovedBtnVisible;
        }

        private bool IsReissueApproveValid()
        {
            bool lblnIsReissueApprovedBtnVisible = false;
            if (ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund &&
                icdoPaymentHistoryDistribution.reissue_to_rollover_org_flag == busConstant.Flag_Yes)
            {
                if (icdoPaymentHistoryDistribution.reissue_to_rollover_org_by != iobjPassInfo.istrUserID)
                {
                    lblnIsReissueApprovedBtnVisible = true;
                }
            }
            else
            {
                lblnIsReissueApprovedBtnVisible = true;
            }
            return lblnIsReissueApprovedBtnVisible;
        }

        //check the payment method is ACH or check
        public bool IsPaymentMethodACHOrRACH()
        {
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH
                || icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRACH)
            {
                return true;
            }
            return false;
        }
        //check the payment method is ACH or check
        public bool IsPaymentMethodCheckOrACH()
        {
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH
                || icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                return true;
            }
            return false;
        }
        //This method will called when btnStopPayPending_Click clicked on the screen
        public ArrayList btnStopPayPending_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            if (istrPaymentMethodRollover == busConstant.Flag_Yes &&
               ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption5YearTermLife)
            {
                if (ibusPaymentHistoryHeader.ibusPayeeAccount == null)
                    ibusPaymentHistoryHeader.LoadPayeeAccount();
                if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbRolloverDetail == null)
                    ibusPaymentHistoryHeader.ibusPayeeAccount.LoadRolloverDetail();
                foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in ibusPaymentHistoryHeader.ibusPayeeAccount.iclbRolloverDetail)
                {
                    if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_org_id == icdoPaymentHistoryDistribution.org_id &&
                        lobjRolloverDetail.icdoPayeeAccountRolloverDetail.status_value != busConstant.PayeeAccountRolloverDetailStatusProcessed)
                    {
                        lobjError = AddError(6464, "");
                        alReturn.Add(lobjError);
                        return alReturn;
                    }
                }
            }
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKStopPayPending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKStopPayPending;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKStopPayPending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKStopPayPending;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
                2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            CreateHistory(DateTime.Now);
            LoadStatus();
            LoadDistributionHistory();
            LoadLatestDistibutionStatusHistory();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                // ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            alReturn.Add(this);
            return alReturn;
        }
        //This method will called when btnVoidPending_Click clicked on the screen
        public ArrayList btnVoidPending_Click()
        {
            ArrayList larrErros = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.ibusPayeeAccount == null)
                ibusPaymentHistoryHeader.LoadPayeeAccount();
            if (istrPaymentMethodRollover == busConstant.Flag_Yes &&
                ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption5YearTermLife)
            {
                foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in ibusPaymentHistoryHeader.ibusPayeeAccount.iclbRolloverDetail)
                {
                    if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_org_id == icdoPaymentHistoryDistribution.org_id &&
                        lobjRolloverDetail.icdoPayeeAccountRolloverDetail.status_value != busConstant.PayeeAccountRolloverDetailStatusProcessed)
                    {
                        lobjError = AddError(6464, "");
                        larrErros.Add(lobjError);
                        return larrErros;
                    }
                }
            }
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKVoidPending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKVoidPending;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.ACHVoidPending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.ACHVoidPending;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKVoidPending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKVoidPending;
            }
            else
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RACHVoidPending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RACHVoidPending;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
              2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            CreateHistory(DateTime.Now);
            LoadDistributionHistory();
            LoadLatestDistibutionStatusHistory();
            LoadStatus();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                //  SetProcessInstanceParameters();
                 SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                // ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
                busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;
                if (lbusActivityInstance.ibusBpmProcessInstance == null)
                {
                    lbusActivityInstance.LoadBpmProcessInstance();
                }
                if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Process_Uncashed_Benefit_Checks)
                {
                    lbusActivityInstance.UpdateParameter("PaymentHistoryDistributionId", icdoPaymentHistoryDistribution.payment_history_distribution_id.ToString());
                }
            }
            this.EvaluateInitialLoadRules();
            larrErros.Add(this);
            return larrErros;
        }
        //This method will be called when btnOutstanding_Click clicked on the screen
        public ArrayList btnOutstanding_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKOutstanding, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKOutstanding;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.ACHOutstanding, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.ACHOutstanding;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKOutstanding, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKOutstanding;
            }
            else
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RACHOutstanding, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RACHOutstanding;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
              2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            CreateHistory(DateTime.Now);
            LoadDistributionHistory();
            LoadLatestDistibutionStatusHistory();

            //PIR 26202 start
            //Update Distribution history to Outstanding
            if(ibusDistributionHistory.IsNotNull())
            {
                if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
                {
                    ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_value = busConstant.CHKOutstanding;
                }
                else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
                {
                    ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_value = busConstant.ACHOutstanding;
                }
                else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
                {
                    ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_value = busConstant.RCHKOutstanding;
                }
                else
                {
                    ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_value = busConstant.RACHOutstanding;
                }
                ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
                  2505, icdoPaymentHistoryDistribution.distribution_status_value);
                ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.Update();
            }
            //Update Payment history header status to Outstanding
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.IsNotNull() && ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id > 0)
            {
                ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value = busConstant.PaymentStatusOutstanding;
                ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
                                                                                        2507, ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value);
                ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.Update();
            }
            //PIR 26202 end

            LoadStatus();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                //SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                // ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            alReturn.Add(this);
            return alReturn;
        }
        //This method will be called when btnOutstanding_Click clicked on the screen
        public ArrayList btnCleared_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKCleared, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKCleared;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.ACHCleared, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.ACHCleared;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKCleared, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKCleared;
            }
            else
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RACHCleared, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RACHCleared;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
              2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            CreateHistory(DateTime.Now);
            LoadDistributionHistory();
            LoadLatestDistibutionStatusHistory();
            LoadStatus();

            //PIR 26202 start
            //Update Distribution history to Outstanding
            if (ibusDistributionHistory.IsNotNull())
            {
                if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
                {
                    ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_value = busConstant.CHKCleared;
                }
                else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
                {
                    ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_value = busConstant.ACHCleared;
                }
                else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
                {
                    ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_value = busConstant.RCHKCleared;
                }
                else
                {
                    ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_value = busConstant.RACHCleared;
                }
                ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
                  2505, icdoPaymentHistoryDistribution.distribution_status_value);
                ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.Update();
            }
            //PIR 26202 end

            //Load all disribution records
            ibusPaymentHistoryHeader.LoadPaymentHistoryDistribution();
            //Change payment history header status to to cancelled whenn all distribution recrds changed to cancelled -BR-075-98
            //PIR 
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDistribution.Count()
                == ibusPaymentHistoryHeader.iclbPaymentHistoryDistribution.Where(o =>
                    o.icdoPaymentHistoryDistribution.status_value == busConstant.PaymentDistributionStatusCleared).Count())
            {
                ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value = busConstant.PaymentStatusProcessed;
                ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.Update();
            }

            if (ibusBaseActivityInstance.IsNotNull())
            {
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                //  ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            alReturn.Add(this);
            return alReturn;
        }
        ////PIR 12930 && PIR 14270 - the flag used to skip validation in btnReissueApproved_Click() method
        public string istrReissueSameRolloverFlag { get; set; }

   
        
        /// <summary>
        /// if payment method is ACH or Check and reissue to rollover org flag is checked, check whether rollover org is exist
        /// if allow to change the status.elso do not
        /// </summary>
        /// <returns>ArrayList</returns>
        public ArrayList btnReissueApproved_Click()
        {
            ArrayList larrErros = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            ibusPaymentHistoryHeader.CalculateAmounts();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDistribution == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDistribution();
            if (ibusPaymentHistoryHeader.ibusPayeeAccount == null)
                ibusPaymentHistoryHeader.LoadPayeeAccount();
            if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails == null)
                ibusPaymentHistoryHeader.ibusPayeeAccount.LoadActiveRolloverDetail();
            if (ibusPaymentHistoryHeader.ibusPaymentSchedule == null)
                ibusPaymentHistoryHeader.LoadPaymentSchedule();
            //pir 7006
            if (!(ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id == 0 && (ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id != 0 ||
                ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id != 0) &&
                ibusPaymentHistoryHeader.ibusPaymentSchedule.icdoPaymentSchedule.schedule_type_value == busConstant.PaymentScheduleScheduleTypeAdhoc))
            {


                if (IsPaymentMethodCheckOrACH())
                {
                    if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails.Count == 0 &&
                        icdoPaymentHistoryDistribution.reissue_to_rollover_org_flag == busConstant.Flag_Yes)
                    {
                        lobjError = AddError(6451, "");
                        larrErros.Add(lobjError);
                        return larrErros;
                    }
                    else if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails.Count > 0 &&
                        (icdoPaymentHistoryDistribution.reissue_to_rollover_org_flag == busConstant.Flag_No || icdoPaymentHistoryDistribution.reissue_to_rollover_org_flag.IsNullOrEmpty()))
                    {
                        if (!ibusPaymentHistoryHeader.ibusPayeeAccount.IsRolloverCheckReissueApproved())
                        {
                            lobjError = AddError(6452, "");
                            larrErros.Add(lobjError);
                            return larrErros;
                        }
                    }
                    else if (ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund &&
                        (icdoPaymentHistoryDistribution.reissue_to_rollover_org_flag == busConstant.Flag_No || icdoPaymentHistoryDistribution.reissue_to_rollover_org_flag.IsNullOrEmpty()))
                    {
                        if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails.Count > 0)
                        {
                            lobjError = AddError(6455, "");
                            larrErros.Add(lobjError);
                            return larrErros;
                        }
                    }
                    else if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails.Count > 0 &&
                       icdoPaymentHistoryDistribution.reissue_to_rollover_org_flag == busConstant.Flag_Yes)
                    {
                        if (ibusPaymentHistoryHeader.iclbPaymentHistoryDistribution.Count > 1)
                        {
                            lobjError = AddError(6466, "");
                            larrErros.Add(lobjError);
                            return larrErros;
                        }
                    }
                }
                /*PIR 14270 - when ‘Reissue To Same Rollover Org’ is checked, if no active Rollover detail exists 
                throw a hard error 'Please add new Rollover Information with an Active status before approving reissue.'*/
                if (istrPaymentMethodRollover == busConstant.Flag_Yes && istrReissueSameRolloverFlag == busConstant.Flag_Yes)
                {
                    if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails.Count() == 0)
                    {
                        lobjError = AddError(10263, "");
                        larrErros.Add(lobjError);
                        return larrErros;
                    }
                }
                if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbRolloverDetail == null)
                    ibusPaymentHistoryHeader.ibusPayeeAccount.LoadRolloverDetail();
                if (istrPaymentMethodRollover == busConstant.Flag_Yes && (istrReissueSameRolloverFlag == busConstant.Flag_No || istrReissueSameRolloverFlag.IsNullOrEmpty()))
                {
                    foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in ibusPaymentHistoryHeader.ibusPayeeAccount.iclbRolloverDetail)
                    {
                        if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_org_id == icdoPaymentHistoryDistribution.org_id &&
                            lobjRolloverDetail.icdoPayeeAccountRolloverDetail.status_value != busConstant.PayeeAccountRolloverDetailStatusCancelled)
                        {
                            lobjError = AddError(6460, "");
                            larrErros.Add(lobjError);
                            return larrErros;
                        }
                    }
                }
                //load active rolllver dertails
                if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbCancelledRolloverDetail == null)
                    ibusPaymentHistoryHeader.ibusPayeeAccount.LoadCancelledRolloverDetails();
                //Check for rollover to member
                if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbCancelledRolloverDetail.Where(o =>
                    o.icdoPayeeAccountRolloverDetail.rollover_org_id == icdoPaymentHistoryDistribution.org_id).Count() > 0 &&
                    ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails.Count == 0)
                {
                    ibusPaymentHistoryHeader.ibusPayeeAccount.iblnIsRolloverAmountReissuedToMember = true;
                    CreateReissuePapitItems(busConstant.PAPITReissueRolloverTaxableAmount);
                    ibusPaymentHistoryHeader.ibusPayeeAccount.CalculateAdjustmentTax(false);
                }//Check for rollover to rollover
                else if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbCancelledRolloverDetail.Count() > 0 &&
                    ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails.Count() > 0)
                {
                    foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails)
                    {
                        lobjRolloverDetail.iblnIsReissueApproveClick = true;
                        lobjRolloverDetail.idtReissuedHistoryPaymentDate = ibusPaymentHistoryHeader.ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                        lobjRolloverDetail.btnRollover_Click();
                    }
                    //since reissue to rollover flag check box is not available, creating net amount reissue explicitly
					//PIR 938 - Commented so as not to create payment history header for member when rollover payment is reissued
                    //if (icdoPaymentHistoryDistribution.net_amount > 0)
                    //    CreateReissuePapitItems(busConstant.PAPITNetAmountReissue, icdoPaymentHistoryDistribution.net_amount);
                }//Check for Member to Rollover
                else if (icdoPaymentHistoryDistribution.reissue_to_rollover_org_flag == busConstant.Flag_Yes &&
                    ibusPaymentHistoryHeader.ibusPayeeAccount.iclbCancelledRolloverDetail.Count == 0 &&
                    ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails.Count() > 0)
                {
                    foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails)
                    {
                        lobjRolloverDetail.iblnIsReissueApproveClick = true;
                        lobjRolloverDetail.idtReissuedHistoryPaymentDate = ibusPaymentHistoryHeader.ibusPaymentSchedule.icdoPaymentSchedule.payment_date;
                        lobjRolloverDetail.btnRollover_Click();
                    }

                    if (ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.gross_amount > 0)
                        CreateReissuePapitItems(busConstant.PAPITNetAmountReissue, ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.gross_amount);
                    //Generate gl for fed and state tax if any
                    GenerateGLForFedAndStateTax();
                }
                //member to member
                else if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails.Count() == 0)
                {
                    CreateReissuePapitItems(busConstant.PAPITNetAmountReissue);
                }
                /*
                if (icdoPaymentHistoryDistribution.reissue_to_rollover_org_flag == busConstant.Flag_Yes &&
                    ibusPaymentHistoryHeader.ibusPayeeAccount.iclbActiveRolloverDetails.Count() > 0)
                {
                    if (ibusPaymentHistoryHeader.ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        ibusPaymentHistoryHeader.ibusPayeeAccount.LoadNexBenefitPaymentDate();
                    if (ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPayeeAccountPaymentItemType == null)
                        ibusPaymentHistoryHeader.ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                    decimal ldecRolloverAmount= ibusPaymentHistoryHeader.ibusPayeeAccount.iclbPayeeAccountPaymentItemType.Where(o=>
                       busGlobalFunctions.CheckDateOverlapping(ibusPaymentHistoryHeader.ibusPayeeAccount.idtNextBenefitPaymentDate,
                       o.icdoPayeeAccountPaymentItemType.start_date,o.icdoPayeeAccountPaymentItemType.end_date) &&
                       o.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value==busConstant.RolloverItemReductionCheck).Sum(o=>o.icdoPayeeAccountPaymentItemType.amount);
                    CreateReissuePapitItems(busConstant.PAPITNetAmountReissue, ldecRolloverAmount);               
                }*/
            }
            else
            {
                //PIR-10786 Start
                if (ibusPaymentHistoryHeader.iclbRemittances == null || ibusPaymentHistoryHeader.iclbRemittances.Count() == 0)
                    ibusPaymentHistoryHeader.LoadRemittances();
                foreach (busRemittance lbusRemittance in ibusPaymentHistoryHeader.iclbRemittances)
                {
                    lbusRemittance.icdoRemittance.refund_status_value = busConstant.ReissuePendingApproval;
                    lbusRemittance.icdoRemittance.refund_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2510, lbusRemittance.icdoRemittance.refund_status_value);
                    lbusRemittance.icdoRemittance.Update();
                }
                //PIR-10786 End
            }
            //Set the staus
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKReissueApproved, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKReissueApproved;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.ACHReissueApproved, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.ACHReissueApproved;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKReissueApproved, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKReissueApproved;
            }
            else
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RACHReissueApproved, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RACHReissueApproved;
            }
            icdoPaymentHistoryDistribution.distribution_status_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            CreateHistory(DateTime.Now);
            LoadStatus();
            LoadDistributionHistory();
            larrErros.Add(this);
            if (ibusBaseActivityInstance.IsNotNull())
            {
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                //  ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            return larrErros;
        }

        /// <summary>
        /// Method to generate GL for fed and state tax
        /// </summary>
        private void GenerateGLForFedAndStateTax()
        {
            decimal ldecFedTax = Math.Abs(ibusPaymentHistoryHeader.iclbPaymentHistoryDetail
                                                            .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value == busConstant.PayeeDetailFedTax)
                                                            .Sum(o => o.icdoPaymentHistoryDetail.amount));
            decimal ldecStateTax = Math.Abs(ibusPaymentHistoryHeader.iclbPaymentHistoryDetail
                                                            .Where(o => o.ibusPaymentItemType.icdoPaymentItemType.payee_detail_group_value == busConstant.PayeeDetailStateTax)
                                                            .Sum(o => o.icdoPaymentHistoryDetail.amount));
            if (ldecFedTax > 0)
            {
                busGLHelper.GenerateGL(ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id,
                                            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id,
                                            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.plan_id,
                                            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id,
                                            ldecFedTax,
                                            busConstant.GLSourceTypeValueChkMaintenance,
                                            busConstant.GLTransactionTypeStatusTransition,
                                            busConstant.GLStatusTransitionApproved,
                                            busConstant.PaymentItemCodeValueFedTaxAmount,
                                            DateTime.Now,
                                            DateTime.Now,
                                            iobjPassInfo);
            }
            if (ldecStateTax > 0)
            {
                busGLHelper.GenerateGL(ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.person_id,
                                            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.org_id,
                                            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.plan_id,
                                            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id,
                                            ldecStateTax,
                                            busConstant.GLSourceTypeValueChkMaintenance,
                                            busConstant.GLTransactionTypeStatusTransition,
                                            busConstant.GLStatusTransitionApproved,
                                            busConstant.PaymentItemCodeValueStateTaxAmount,
                                            DateTime.Now,
                                            DateTime.Now,
                                            iobjPassInfo);
            }
        }
        //Create reissue items in papit when reissue approving the check or ach
        private void CreateReissuePapitItems(string astrItemCode)
        {
            //PIR 10036
            if (ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id > 0)
            {
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                if (ibusPaymentHistoryHeader.ibusPayeeAccount == null)
                    ibusPaymentHistoryHeader.LoadPayeeAccount();
                busPayeeAccountPaymentItemType lobjPapitItem = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                lobjPapitItem.icdoPayeeAccountPaymentItemType.payment_item_type_id =
                                ibusPaymentHistoryHeader.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(astrItemCode);
                lobjPapitItem.icdoPayeeAccountPaymentItemType.payee_account_id = ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
                lobjPapitItem.icdoPayeeAccountPaymentItemType.start_date = DateTime.Today;
                lobjPapitItem.icdoPayeeAccountPaymentItemType.amount = ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.net_amount;
                lobjPapitItem.icdoPayeeAccountPaymentItemType.reissue_item_flag = busConstant.Flag_Yes;
                lobjPapitItem.icdoPayeeAccountPaymentItemType.Insert();
            }
        }
        //Create reissue items in papit when reissue approving the check or ach
        private void CreateReissuePapitItems(string astrItemCode, decimal adecAmount)
        {
            //PIR 10036
            if (ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id > 0)
            {
                busPayeeAccountPaymentItemType lobjPapitItem = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                lobjPapitItem.icdoPayeeAccountPaymentItemType.payment_item_type_id =
                ibusPaymentHistoryHeader.ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(astrItemCode);
                lobjPapitItem.icdoPayeeAccountPaymentItemType.payee_account_id =
                             ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.payee_account_id;
                lobjPapitItem.icdoPayeeAccountPaymentItemType.start_date = DateTime.Today;
                lobjPapitItem.icdoPayeeAccountPaymentItemType.amount = adecAmount;
                lobjPapitItem.icdoPayeeAccountPaymentItemType.reissue_item_flag = busConstant.Flag_Yes;
                lobjPapitItem.icdoPayeeAccountPaymentItemType.Insert();
            }
        }
        //This method will called when btnEscheattoNDPERS_Click clicked on the screen
        public ArrayList btnEscheattoNDPERS_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKEscheattoNDPERS, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo, busConstant.PaymentItemCodeValueNetAmountEscheatToNDPERS);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKEscheattoNDPERS;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKEscheattoNDPERS, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo, busConstant.PaymentItemCodeValueNetAmountEscheatToNDPERS);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKEscheattoNDPERS;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
             2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            //PIR 16219 - START - Update Payment History Header status with distribution status - For Escheat to NDPERS as well as Escheat to State								
            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value = busConstant.HistoryHeaderStatusEscheatToNDPERS;
            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.Update();
            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.Select();
            //PIR 16219 - END - Update Payment History Header status with distribution status - For Escheat to NDPERS as well as Escheat to State
            CreateHistory(DateTime.Now);
            LoadStatus();
            LoadDistributionHistory();
            LoadLatestDistibutionStatusHistory();
            alReturn.Add(this);
            if (ibusBaseActivityInstance.IsNotNull())
            {
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                // ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            return alReturn;
        }
        //This method will called when btnEscheattoState_Click clicked on the screen
        public ArrayList btnEscheattoState_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKEscheattoState, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo, busConstant.PaymentItemCodeValueNetAmountEscheatToState);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKEscheattoState;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKEscheattoState, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo, busConstant.PaymentItemCodeValueNetAmountEscheatToState);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKEscheattoState;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
              2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            //PIR 16219 - START - Update Payment History Header status with distribution status - For Escheat to NDPERS as well as Escheat to State								
            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value = busConstant.HistoryHeaderStatusEscheatToState;
            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.Update();
            ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.Select();
            //PIR 16219 - END - Update Payment History Header status with distribution status - For Escheat to NDPERS as well as Escheat to State
            CreateHistory(DateTime.Now);
            LoadStatus();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                //  ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            LoadDistributionHistory();
            alReturn.Add(this);
            return alReturn;
        }
        //This method will called when btnEscheatReissued_Click clicked on the screen
        public ArrayList btnEscheatReissued_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKEscheatedReissued, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKEscheatedReissued;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKEscheatedReissue, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKEscheatedReissue;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
              2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            CreateHistory(DateTime.Now);
            LoadStatus();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                // ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            LoadDistributionHistory();
            alReturn.Add(this);
            return alReturn;
        }
        //This method will called when btnReceivablePending_Click clicked on the screen
        public ArrayList btnReceivablePending_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKReceivablesPending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKReceivablesPending;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.ACHReceivablePending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.ACHReceivablePending;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKReceivablesPending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKReceivablesPending;
            }
            else
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RACHReceivablePending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RACHReceivablePending;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
              2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            CreateHistory(DateTime.Now);
            LoadStatus();
            LoadDistributionHistory();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                // ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            alReturn.Add(this);
            return alReturn;
        }
        //This method will called when btnCancel_Click clicked on the screen
        public ArrayList btnCancel_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKCancelled, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKCancelled;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.ACHCancelled, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.ACHCancelled;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKCancelled, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKCancelled;
            }
            else
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RACHCancelled, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RACHCancelled;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
              2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            CreateHistory(DateTime.Now);
            LoadStatus();
            LoadDistributionHistory();

            //UCS - 079 Start  -- Changing Recovery history amounts to zero          
            UpdateRecoveryHistory();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                //  ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            //Load all disribution records
            ibusPaymentHistoryHeader.LoadPaymentHistoryDistribution();
            //Change payment history header status to to cancelled whenn all distribution recrds changed to cancelled -BR-075-98

            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDistribution.Count()
                == ibusPaymentHistoryHeader.iclbPaymentHistoryDistribution.Where(o =>
                    o.icdoPaymentHistoryDistribution.status_value == busConstant.PaymentDistributionStatusCancelled).Count())
            {
                ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.status_value = busConstant.PaymentStatusCancelled;
                ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.Update();
            }

            this.EvaluateInitialLoadRules();
            alReturn.Add(this);
            return alReturn;
        }

        //This method will called when btnReceivableCreated_Click clicked on the screen
        public ArrayList btnReceivableCreated_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKReceivablesCreated, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKReceivablesCreated;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.ACHReceivablesCreated, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.ACHReceivablesCreated;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKReceivablesCreated, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKReceivablesCreated;
            }
            else
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRACH, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RACHReceivablesCreated, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RACHReceivablesCreated;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
               2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            CreateHistory(DateTime.Now);
            LoadStatus();
            LoadDistributionHistory();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                // ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            alReturn.Add(this);
            return alReturn;
        }
        //This method will called when btnEscheatReissuePending_Click clicked on the screen
        public ArrayList btnEscheatReissuePending_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKEscheatedReissuePending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKEscheatedReissuePending;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKEscheatedReissuePending, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKEscheatedReissuePending;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
             2505, icdoPaymentHistoryDistribution.distribution_status_value);
            icdoPaymentHistoryDistribution.Update();
            CreateHistory(DateTime.Now);
            LoadStatus();
            LoadDistributionHistory();
            LoadLatestDistibutionStatusHistory();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                //SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                //  ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            alReturn.Add(this);
            return alReturn;
        }
        //This method will called when btnEscheatReissueApproved_Click clicked on the screen
        public ArrayList btnEscheatReissueApproved_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                ibusPaymentHistoryHeader.LoadPaymentHistoryDetails();
            if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.CHKEscheatReissueApproved, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.CHKEscheatReissueApproved;
            }
            else if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
            {
                //Creating GL
                busGLHelper.GenerateGL(busConstant.GLStatusTransitionRCHK, icdoPaymentHistoryDistribution.distribution_status_value,
                                        busConstant.RCHKEscheatReissueApproved, busConstant.GLTransactionTypeStatusTransition,
                                        busConstant.GLSourceTypeValueChkMaintenance, ibusPaymentHistoryHeader, null, iobjPassInfo);
                icdoPaymentHistoryDistribution.distribution_status_value = busConstant.RCHKEscheatReissueApproved;
            }
            icdoPaymentHistoryDistribution.distribution_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(
             2505, icdoPaymentHistoryDistribution.distribution_status_value);
            CreateReissuePapitItems(busConstant.PaymentItemTypeCodeNetAmountEscheatReissue);
            icdoPaymentHistoryDistribution.Update();
            CreateHistory(DateTime.Now);
            LoadStatus();
            LoadDistributionHistory();
            if (ibusBaseActivityInstance.IsNotNull())
            {
                //SetProcessInstanceParameters();
                SetCaseInstanceParameters();
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                ibusPaymentHistoryHeader.ibusBaseActivityInstance = ibusBaseActivityInstance;
                //   ibusPaymentHistoryHeader.SetProcessInstanceParameters();
                ibusPaymentHistoryHeader.SetCaseInstanceParameters();
            }
            this.EvaluateInitialLoadRules();
            alReturn.Add(this);
            return alReturn;
        }
        public override void BeforePersistChanges()
        {
            //assign current login user id to reissue_to_rollover_org_by if reissue_to_rollover_org_flag is set to "YES"
            icdoPaymentHistoryDistribution.reissue_to_rollover_org_by = icdoPaymentHistoryDistribution.reissue_to_rollover_org_flag == busConstant.Flag_Yes ?
                iobjPassInfo.istrUserID : null;
            base.BeforePersistChanges();
        }
        //Whenever the distribution status is changed ,there should be a record inserted into distribution history tables
        public override void AfterPersistChanges()
        {
            if (icdoPaymentHistoryDistribution.distribution_status_value
                != ibusDistributionHistory.icdoPaymentHistoryDistributionStatusHistory.distribution_status_value)
            {
                CreateHistory(DateTime.Now);
            }
            LoadDistributionHistory();
            LoadLatestDistibutionStatusHistory();
        }
        //Create a record in distribution history tables
        public void CreateHistory(DateTime adtEffetiveDate)
        {
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.ibusPaymentSchedule == null)
                ibusPaymentHistoryHeader.LoadPaymentSchedule();
            cdoPaymentHistoryDistributionStatusHistory lobjPaymentHistoryDistributionStatusHistory = new cdoPaymentHistoryDistributionStatusHistory();
            lobjPaymentHistoryDistributionStatusHistory.payment_history_distribution_id = icdoPaymentHistoryDistribution.payment_history_distribution_id;
            lobjPaymentHistoryDistributionStatusHistory.payment_history_header_id = icdoPaymentHistoryDistribution.payment_history_header_id;
            lobjPaymentHistoryDistributionStatusHistory.transaction_date = adtEffetiveDate;
            lobjPaymentHistoryDistributionStatusHistory.distribution_status_value = icdoPaymentHistoryDistribution.distribution_status_value;
            lobjPaymentHistoryDistributionStatusHistory.status_change_reason_value = icdoPaymentHistoryDistribution.status_change_reason_value;
            lobjPaymentHistoryDistributionStatusHistory.status_changed_by = iobjPassInfo.istrUserID;
            lobjPaymentHistoryDistributionStatusHistory.Insert();
        }
        //Set visibility Reissue to rollover org flag if benefit type is refund and payment method is 'ACH' or 'Check
        public bool IsReissueToRolloverOrgAllowed()
        {
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.ibusPayeeAccount == null)
                ibusPaymentHistoryHeader.LoadPayeeAccount();
            if ((icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH ||
                icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodCHK)
                && (ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund ||
                ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRegularRefund))
            {
                return true;
            }
            return false;
        }
        //PIR 12930 && PIR 14270 - Setting visibility for reissue to rollover org
        public bool IsReissueToSameRolloverOrgAllowed()
        {
            if (ibusPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (ibusPaymentHistoryHeader.ibusPayeeAccount == null)
                ibusPaymentHistoryHeader.LoadPayeeAccount();
            if ((icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
                && (ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund ||
                ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRegularRefund))
            {
                return true;
            }
            return false;
        }
        //The system must throw error ‘Cannot Reissue. 1099r is already issued for the Payment.’ 
        //when user checks ‘Reissue to Rollover Organization’ and there exists 1099r for the ‘Year’ of the ‘Payment Date’ and saves the record,
        public bool Is1099rExistForRefund()
        {
            if (icdoPaymentHistoryDistribution.reissue_to_rollover_org_flag == busConstant.Flag_Yes)
            {
                if (ibusPaymentHistoryHeader.Is1099rExistsForPaymentYear())
                {
                    return true;
                }
            }
            return false;
        }
        //Correspondence Properties
        public string istrPaymentMonth
        {
            get
            {
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                DateTime ldtPayPeriod = new DateTime(ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date.Year,
                   ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date.Month, ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date.Day);
                return ldtPayPeriod.ToString("MMMM");
            }
        }
        //PAY-4200 properties
        public string istrPaymentMethodRollover
        {
            get
            {
                if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRACH ||
                    icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRCHK)
                {
                    return busConstant.Flag_Yes;
                }
                return busConstant.Flag_No;
            }
        }
        public string istrPaymentMethodDirectDeposit
        {
            get
            {
                if (icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodACH ||
                    icdoPaymentHistoryDistribution.payment_method_value == busConstant.PaymentDistributionPaymentMethodRACH)
                {
                    return busConstant.Flag_Yes;
                }
                return busConstant.Flag_No;
            }
        }
        //PIR 16219 - Correspondance properties - PAY-4025
        public string istrBenefitTypeOfDistribution
        {
            get
            {
                if (ibusPaymentHistoryHeader.IsNull())
                    LoadPaymentHistoryHeader();
                if(ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id > 0)
                {
                    if(ibusPaymentHistoryHeader.ibusPayeeAccount.IsNull()) ibusPaymentHistoryHeader.LoadPayeeAccount();
                    if (ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.Refund)
                        return ibusPaymentHistoryHeader.ibusPayeeAccount.icdoPayeeAccount.benefit_account_type_value;
                    else
                        return busConstant.PlanBenefitTypeRetirement; 
                }
                return string.Empty;
            }
        }
        public string istrBenefitTypePremiumRefund
        {
            get
            {
                if (ibusPaymentHistoryHeader.IsNull())
                    LoadPaymentHistoryHeader();
                if (ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payee_account_id == 0)
                {
                    DataTable ldtbRemittance = busBase.Select<cdoRemittance>(new string[1] 
                    { enmRemittance.payment_history_header_id.ToString() }, new object[1] { ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_history_header_id }, null, null);
                    if(ldtbRemittance.Rows.Count > 0)
                    {
                        return busConstant.Flag_Yes;
                    }
                }
                return busConstant.Flag_No;
            }
        }


        //Load Prior Rollover Organization
        public string istrPriorRolloverOrg { get; set; }

        #region Methods for Correpondence
        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                LoadPerson();
            return ibusPerson;
        }
        public override busBase GetCorOrganization()
        {
            if (ibusOrganization == null)
                LoadOrganization();
            return ibusOrganization;
        }

        public string istrPaymentDateFormatted
        {
            get
            {
                if (ibusPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
                if (ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date == DateTime.MinValue)
                    return string.Empty;
                else
                    return ibusPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date.ToString(busConstant.DateFormatLongDate);
            }
        }
        #endregion
        //prop to indicate 2nd notice for outstanding checks
        public string istr2ndCashOutstandingNotice { get; set; }
        public DateTime AsofDate { get { return DateTime.Today; } }
        public string istrAsofDate { get { return DateTime.Today.ToString(busConstant.DateFormatLongDate); } }
        //prop to indicate check created for Premium Refund
        //prop to indicate 2nd notice for outstanding checks
        public string istrPremiumRefundCheck { get; set; }
        public string istrIsInsurancePlan
        {
            get
            {
                if (ibusPaymentHistoryHeader.IsNull())
                    LoadPaymentHistoryHeader();
                if (ibusPaymentHistoryHeader.ibusPlan.IsNull())
                    ibusPaymentHistoryHeader.LoadPlan();
                if (ibusPaymentHistoryHeader.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
                {
                    return busConstant.Flag_Yes;
                }
                return busConstant.Flag_No;
            }
        }
        #region UCS - 079
        //Property to contain Payment Recovery History
        public Collection<busPaymentRecoveryHistory> iclbRecoveryHistory { get; set; }
        /// <summary>
        /// Method to load Payment recovery history
        /// </summary>
        public void LoadPaymentRecoveryHistoryByPaymentHistoryId()
        {
            DataTable ldtRecoveryHistory = Select<cdoPaymentRecoveryHistory>
                (new string[1] { enmPaymentRecoveryHistory.payment_history_header_id.ToString() },
                new object[1] { icdoPaymentHistoryDistribution.payment_history_header_id }, null, null);
            iclbRecoveryHistory = new Collection<busPaymentRecoveryHistory>();
            iclbRecoveryHistory = GetCollection<busPaymentRecoveryHistory>(ldtRecoveryHistory, "icdoPaymentRecoveryHistory");
            foreach (busPaymentRecoveryHistory lobjRecoveryHistory in iclbRecoveryHistory)
                lobjRecoveryHistory.LoadPaymentRecovery();
        }

        /// <summary>
        /// Method to update Recovery history amounts
        /// </summary>
        public void UpdateRecoveryHistory()
        {
            if (iclbRecoveryHistory == null)
                LoadPaymentRecoveryHistoryByPaymentHistoryId();
            foreach (busPaymentRecoveryHistory lobjRecoveryHistory in iclbRecoveryHistory)
            {
                //Updating recovery history amounts to zero
                lobjRecoveryHistory.ResetAmountFields();
                //updating recovery status to InProgress if the current status is Satisfied
                if (lobjRecoveryHistory.ibusPaymentRecovery.icdoPaymentRecovery.status_value == busConstant.RecoveryStatusSatisfied)
                {
                    lobjRecoveryHistory.ibusPaymentRecovery.icdoPaymentRecovery.status_value = busConstant.RecoveryStatusInProcess;
                    lobjRecoveryHistory.ibusPaymentRecovery.icdoPaymentRecovery.Update();
                }
            }
        }

        #endregion


        # region UCS - 54
        public bool isOutStandingStatus()
        {
            return busGlobalFunctions.GetData2ByCodeValue(2505, icdoPaymentHistoryDistribution.distribution_status_value, iobjPassInfo).Equals(busConstant.DistributionStatusOutstanding);
        }
        public bool isClearedStatus()
        {
            return busGlobalFunctions.GetData2ByCodeValue(2505, icdoPaymentHistoryDistribution.distribution_status_value, iobjPassInfo).Equals(busConstant.CHKCleared);
        }
        public bool isCancelledStatus()
        {
            return busGlobalFunctions.GetData2ByCodeValue(2505, icdoPaymentHistoryDistribution.distribution_status_value, iobjPassInfo).Equals(busConstant.CHKCancelled);
        }
        public bool isReceivableCreatedStatus()
        {
            return busGlobalFunctions.GetData2ByCodeValue(2505, icdoPaymentHistoryDistribution.distribution_status_value, iobjPassInfo).Equals(busConstant.CHKReceivablesCreated);
        }
        public bool IsNotCancelledStatus()
        {
            return !busGlobalFunctions.GetData2ByCodeValue(2505, icdoPaymentHistoryDistribution.distribution_status_value, iobjPassInfo).Equals(busConstant.CHKCancelled);
        }
        # endregion
    }
}