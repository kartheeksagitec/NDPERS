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

#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busPostRetirementIncreaseBatchRequest:
    /// Inherited from busPostRetirementIncreaseBatchRequestGen, the class is used to customize the business object busPostRetirementIncreaseBatchRequestGen.
    /// </summary>
    [Serializable]
    public class busPostRetirementIncreaseBatchRequest : busPostRetirementIncreaseBatchRequestGen
    {
        public bool iblnIsCancelledButtonClicked { get; set; }
        # region Button Logic

        public ArrayList btnCancelClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lerror = new utlError();
            iblnIsCancelledButtonClicked = true;
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                icdoPostRetirementIncreaseBatchRequest.action_status_value = busConstant.BenefitActionStatusCancelled;
                icdoPostRetirementIncreaseBatchRequest.action_status_description =
                                                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2302, icdoPostRetirementIncreaseBatchRequest.action_status_value);
                icdoPostRetirementIncreaseBatchRequest.Update();
                alReturn.Add(this);
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }
        public ArrayList btnApproveClicked()
        {
            ArrayList alReturn = new ArrayList();
            utlError lerror = new utlError();
            this.ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count == 0)
            {
                icdoPostRetirementIncreaseBatchRequest.action_status_value = busConstant.BenefitActionStatusApproved;
                icdoPostRetirementIncreaseBatchRequest.action_status_description =
                                                        iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2302, icdoPostRetirementIncreaseBatchRequest.action_status_value);
                icdoPostRetirementIncreaseBatchRequest.approved_by = iobjPassInfo.istrUserID;
                icdoPostRetirementIncreaseBatchRequest.Update();
                alReturn.Add(this);
                this.EvaluateInitialLoadRules();
            }
            else
            {
                foreach (utlError larr in iarrErrors)
                {
                    alReturn.Add(larr);
                }
            }
            return alReturn;
        }

        # endregion

        # region Rules
        //visible rules
        public bool IsLoginUserSameAsCreatedBy()
        {
            return (icdoPostRetirementIncreaseBatchRequest.created_by == null ? true :
                                    icdoPostRetirementIncreaseBatchRequest.created_by.Equals(iobjPassInfo.istrUserID));
        }

        public int IsEffectiveDateValid()
        {
            //PIR - 1570
            //BR- 06
            //only for COLA and ADhoc request type.
            if (icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value == busConstant.PostRetirementIncreaseTypeValueCOLA
                || icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value == busConstant.PostRetirementIncreaseTypeValueAdHoc)
            {

                DateTime ldtTempdate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
                ldtTempdate = new DateTime(ldtTempdate.Year, ldtTempdate.Month, 1);
                return icdoPostRetirementIncreaseBatchRequest.effective_date == DateTime.MinValue ? 1 :
                           icdoPostRetirementIncreaseBatchRequest.effective_date
                           .CompareTo(busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1));
            }
            return 1;
        }

        //check if the user checked valid plan based on the Post retirement increase type
        public bool IsValidPlanForPostRetirmentIncreaseType()
        {
            foreach (cdoPostRetirementIncreaseBatchPlan lobjPlan in iclcPlan)
            {
                if (icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value == busConstant.PostRetirementIncreaseTypeValueCOLA)
                {
                    if ((lobjPlan.plan_id != busConstant.PlanIdJobService)
                        && (lobjPlan.plan_id != busConstant.PlanIdJobService3rdPartyPayor))
                        return false;
                }
                else
                {
                    if ((lobjPlan.plan_id == busConstant.PlanIdJobService)
                                           || (lobjPlan.plan_id == busConstant.PlanIdJobService3rdPartyPayor))
                        return false;
                }
            }
            return true;
        }

        //check if plan is selected
        public int IsPlanSelected()
        {
            return iclcPlan.Where(lobjPlan => lobjPlan.ienuObjectState != ObjectState.CheckListDelete).Count();
        }

        public int IsBenefitAccountSelected()
        {
            return iclcBenefitAccount.Where(lobjBenefitAccount => lobjBenefitAccount.ienuObjectState != ObjectState.CheckListDelete).Count();
        }

        public int IsAccountRelationshipSelected()
        {
            return iclcAccountRelationship.Where(lobjAccountRelationship => lobjAccountRelationship.ienuObjectState != ObjectState.CheckListDelete).Count();
        }
        # endregion

        #region Set, Load Methods

        public utlCollection<cdoPostRetirementIncreaseBatchPlan> iclcPlan { get; set; }
        public utlCollection<cdoPostRetirementIncreaseBatchBenefitAccountType> iclcBenefitAccount { get; set; }
        public utlCollection<cdoPostRetirementIncreaseBatchAccountRelationship> iclcAccountRelationship { get; set; }

        public void LoadPlanList()
        {
            iclcPlan = GetCollection<cdoPostRetirementIncreaseBatchPlan>(new string[1] { "post_retirement_increase_batch_request_id" },
                                                                         new object[1] { icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id }, null, null);
        }

        public void LoadAccountRelationshipList()
        {
            iclcAccountRelationship = GetCollection<cdoPostRetirementIncreaseBatchAccountRelationship>(new string[1] { "post_retirement_increase_batch_request_id" },
                                                                                                       new object[1] { icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id }, null, null);
        }

        public void LoadBenefitAccoutTypeList()
        {
            iclcBenefitAccount = GetCollection<cdoPostRetirementIncreaseBatchBenefitAccountType>(new string[1] { "post_retirement_increase_batch_request_id" },
                                                                                                 new object[1] { icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id }, null, null);
        }

        public override void SetParentKey(Sagitec.DataObjects.doBase aobjBase)
        {
            if (aobjBase is cdoPostRetirementIncreaseBatchPlan)
            {
                var lcdoBatchPlan = (cdoPostRetirementIncreaseBatchPlan)aobjBase;
                lcdoBatchPlan.post_retirement_increase_batch_request_id = icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id;
            }

            if (aobjBase is cdoPostRetirementIncreaseBatchAccountRelationship)
            {
                var lcdoRelationship = (cdoPostRetirementIncreaseBatchAccountRelationship)aobjBase;
                lcdoRelationship.post_retirement_increase_batch_request_id = icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id;
            }

            if (aobjBase is cdoPostRetirementIncreaseBatchBenefitAccountType)
            {
                var lcdoType = (cdoPostRetirementIncreaseBatchBenefitAccountType)aobjBase;
                lcdoType.post_retirement_increase_batch_request_id = icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id;
            }
        }

        # endregion


        # region overriden methods

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            //Remove all the CheckListDelete Status Items on New Mode, Otherwise we will get exception while saving on new mode
            //Add other Items into iarrChangeLog also set the object State as CheckListInsert
            if (icdoPostRetirementIncreaseBatchRequest.ienuObjectState == ObjectState.Insert)
            {
                SetObjectStateForPlanList();
                SetObjectStateForBenefitAccountList();
                SetObjectStateForAccountRelationshipList();
            }
            if (icdoPostRetirementIncreaseBatchRequest.suppress_warnings_flag == busConstant.Flag_Yes)
            {
                icdoPostRetirementIncreaseBatchRequest.suppress_warnings_date = DateTime.Now;
                icdoPostRetirementIncreaseBatchRequest.suppress_warnings_by = iobjPassInfo.istrUserID;
            }
            base.BeforeValidate(aenmPageMode);
        }

        public override void BeforePersistChanges()
        {
            //this is done only for Copy Button
            //as Framework will not be able to add the new object to the iarrChangelog
            if (iarrChangeLog.Where(lobj => (Sagitec.DataObjects.doBase)lobj == this.icdoPostRetirementIncreaseBatchRequest).Count() == 0)
                iarrChangeLog.Add(this.icdoPostRetirementIncreaseBatchRequest);
            base.BeforePersistChanges();
        }

        private void SetObjectStateForAccountRelationshipList()
        {
            for (int i = iclcAccountRelationship.Count - 1; i >= 0; i--)
            {
                cdoPostRetirementIncreaseBatchAccountRelationship lcdoAccountrelationship = iclcAccountRelationship[i];
                if (lcdoAccountrelationship.ienuObjectState == ObjectState.CheckListDelete)
                {
                    iclcAccountRelationship.Remove(lcdoAccountrelationship);
                    iarrChangeLog.Remove(lcdoAccountrelationship);
                }
                else
                {
                    lcdoAccountrelationship.ienuObjectState = ObjectState.CheckListInsert;
                    iarrChangeLog.Add(lcdoAccountrelationship);
                }
            }
        }

        private void SetObjectStateForBenefitAccountList()
        {
            for (int i = iclcBenefitAccount.Count - 1; i >= 0; i--)
            {
                cdoPostRetirementIncreaseBatchBenefitAccountType lcdoBenefitAccount = iclcBenefitAccount[i];
                if (lcdoBenefitAccount.ienuObjectState == ObjectState.CheckListDelete)
                {
                    iclcBenefitAccount.Remove(lcdoBenefitAccount);
                    iarrChangeLog.Remove(lcdoBenefitAccount);
                }
                else
                {
                    lcdoBenefitAccount.ienuObjectState = ObjectState.CheckListInsert;
                    iarrChangeLog.Add(lcdoBenefitAccount);
                }
            }
        }

        private void SetObjectStateForPlanList()
        {
            for (int i = iclcPlan.Count - 1; i >= 0; i--)
            {
                cdoPostRetirementIncreaseBatchPlan lcdoPlan = iclcPlan[i];
                if (lcdoPlan.ienuObjectState == ObjectState.CheckListDelete)
                {
                    iclcPlan.Remove(lcdoPlan);
                    iarrChangeLog.Remove(lcdoPlan);
                }
                else
                {
                    lcdoPlan.ienuObjectState = ObjectState.CheckListInsert;
                    iarrChangeLog.Add(lcdoPlan);
                }
            }
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadAccountRelationshipList();
            LoadPlanList();
            LoadBenefitAccoutTypeList();
        }
        # endregion

        # region Correspondence
        public override busBase GetCorPerson()
        {
            //ibuspayee can not be loaded
            //ibusPayee account will be loaded in the batch itself
            //if at all payee account is null return null
            if (ibusPayeeAccount.IsNotNull())
            {
                if (ibusPayeeAccount.ibusPayee.IsNull())
                    ibusPayeeAccount.LoadPayee();
                return ibusPayeeAccount.ibusPayee;
            } return null;
        }
        public string istrOrgCodeId
        {
            get
            {
                string lstrOrgCodeId = busGlobalFunctions.GetData1ByCodeValue(52,
                    busConstant.MetroPolitanLifeInsuranceCompany, iobjPassInfo).ToString();
                return lstrOrgCodeId;
            }
        }
        public string istrPriorPlanYear
        {
            get
            {
                return icdoPostRetirementIncreaseBatchRequest.effective_date.Year.ToString(); // UAT PIR ID 1499
            }
        }

        public string istrCOLAIncrease
        {
            get
            {
                if (icdoPostRetirementIncreaseBatchRequest.increase_percentage > 0.0M)
                {
                    return Convert.ToString(icdoPostRetirementIncreaseBatchRequest.increase_percentage) + " Percent";
                }
                else
                {
                    return "$" + Convert.ToString(icdoPostRetirementIncreaseBatchRequest.increase_flat_amount);
                }
            }
        }

        public string IsPercentageIncreaseEntered
        {
            get
            {
                if (icdoPostRetirementIncreaseBatchRequest.increase_percentage > 0.00M)
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }

        public string IsFlatIncreaseEntered
        {
            get
            {
                if (icdoPostRetirementIncreaseBatchRequest.increase_flat_amount > 0.00M)
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }

        public decimal idecBenefitAmount { get; set; }
        public decimal idecNewBenefitAmount { get; set; }
        public decimal idecNetAmount { get; set; }
        public decimal idecNetNewBenefitAmount { get; set; }
        public decimal idecFedTaxAmount { get; set; }
        public decimal idecStateTaxAmount { get; set; }
        public decimal idecNewFedTaxAmount { get; set; }
        public decimal idecNewStateTaxAmount { get; set; }

        public bool IsHealthPremiumNull // PIR 8409
        {
            get
            {
                if (ibusPayeeAccount.idecHealthInsurancePremiumAmount > 0.00M)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsMedicarePremiumNull => (ibusPayeeAccount?.idecMedicareInsPremiumAmount > 0.00M) ? false : true; // PIR 18243

        public bool IsDentalPremiumNull // PIR 8409
        {
            get
            {
                if (ibusPayeeAccount.idecDentalInsurancePremiumAmount > 0.00M)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsVisionPremiumNull // PIR 8409
        {
            get
            {
                if (ibusPayeeAccount.idecVisionInsurancePremiumAmount > 0.00M)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsLifePremiumNull // PIR 8409
        {
            get
            {
                if (ibusPayeeAccount.idecLifePremiumAmount > 0.00M)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsNDPADeductionNull // PIR 8409
        {
            get
            {
                if (ibusPayeeAccount.idecNDPEADuesAmount > 0.00M)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsAFPEDeductionNull // PIR 8409
        {
            get
            {
                if (ibusPayeeAccount.idecAFPEDuesAmount > 0.00M)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsChildSupportDeductionNull // PIR 8409
        {
            get
            {
                if (ibusPayeeAccount.idecChildSupportAmount > 0.00M)
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsFederalTaxNull // PIR 8409
        {
            get
            {
                if ((idecFedTaxAmount > 0.00M) || (idecNewFedTaxAmount > 0.00M))
                {
                    return false;
                }
                return true;
            }
        }

        public bool IsStateTaxNull // PIR 8409
        {
            get
            {
                if ((idecStateTaxAmount > 0.00M) || (idecNewStateTaxAmount > 0.00M))
                {
                    return false;
                }
                return true;
            }
        }
        
        public void LoadBenefitAmount()
        {
            string istrPaymentItemCode = string.Empty;
            if (icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value == busConstant.PostRetirementIncreaseTypeValueCOLA)
                istrPaymentItemCode = busConstant.PAPITCOLABase;
            else if (icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value == busConstant.PostRetirementIncreaseTypeValueAdHoc)
                istrPaymentItemCode = busConstant.PAPITAdhoc;

            //idecBenefitAmount = iclbPorRetirementIncreaseBatchRequestDetail
            //                            .Where(lobj => lobj.payment_item_type_id == ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(istrPaymentItemCode) &&
            //                                lobj.payee_account_id==ibusPayeeAccount.icdoPayeeAccount.payee_account_id)
            //                            .Sum(lobjDet => lobjDet.original_amount);
            /*idecNewBenefitAmount = iclbPorRetirementIncreaseBatchRequestDetail
                                        .Where(lobj => lobj.payment_item_type_id == ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(istrPaymentItemCode) &&
                                            lobj.payee_account_id == ibusPayeeAccount.icdoPayeeAccount.payee_account_id)
                                        .Sum(lobjDet => lobjDet.increase_amount); */

            //UAT PIR 1942 - Load the Old Benefit Amount based on effective date -1 month. Discussed with David
            ibusPayeeAccount.LoadGrossAmount(icdoPostRetirementIncreaseBatchRequest.effective_date.AddMonths(-1));
            idecBenefitAmount = ibusPayeeAccount.idecGrossAmount;

            ibusPayeeAccount.LoadGrossAmount(icdoPostRetirementIncreaseBatchRequest.effective_date);
            idecNewBenefitAmount = ibusPayeeAccount.idecGrossAmount;

            if (icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value == busConstant.PostRetirementIncreaseTypeValueAdHoc)
            {
                idecFedTaxAmount = iclbPorRetirementIncreaseBatchRequestDetail
                                            .Where(lobj => lobj.payment_item_type_id == ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITFederalTaxAmount) &&
                                            lobj.payee_account_id == ibusPayeeAccount.icdoPayeeAccount.payee_account_id)
                                            .Sum(lobj => lobj.original_amount);
                idecStateTaxAmount = iclbPorRetirementIncreaseBatchRequestDetail
                                            .Where(lobj => lobj.payment_item_type_id == ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDStateTaxAmount)
                                            && lobj.payee_account_id == ibusPayeeAccount.icdoPayeeAccount.payee_account_id)
                                            .Sum(lobj => lobj.original_amount);

                idecNewFedTaxAmount = iclbPorRetirementIncreaseBatchRequestDetail
                                            .Where(lobj => lobj.payment_item_type_id == ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITFederalTaxAmount)
                                            && lobj.payee_account_id == ibusPayeeAccount.icdoPayeeAccount.payee_account_id)
                                            .Sum(lobj => lobj.increase_amount);
                idecNewStateTaxAmount = iclbPorRetirementIncreaseBatchRequestDetail
                                            .Where(lobj => lobj.payment_item_type_id == ibusPayeeAccount.GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDStateTaxAmount)
                                            && lobj.payee_account_id == ibusPayeeAccount.icdoPayeeAccount.payee_account_id)
                                            .Sum(lobj => lobj.increase_amount);
            }
            else if (icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value == busConstant.PostRetirementIncreaseTypeValueCOLA)
            {
                //The old State tax is calculated before recalculating the tax in the cola batch.               
                ibusPayeeAccount.LoadTaxAmounts(icdoPostRetirementIncreaseBatchRequest.effective_date);
                idecNewFedTaxAmount = ibusPayeeAccount.idecFedTaxAmount;
                idecNewStateTaxAmount = ibusPayeeAccount.idecStateTaxAmount;
            }
        }

        public void LoadNewBenefitAmount()
        {
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();

            ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
            ibusPayeeAccount.LoadInsurancePremiumAmount();
            ibusPayeeAccount.LoadTotalDeductionAmount();
            //ibusPayeeAccount.LoadTaxAmounts();
            ibusPayeeAccount.LoadOtherDeductionAmount();

            decimal ldecAmountTobeDeducted = ibusPayeeAccount.idecDentalInsurancePremiumAmount + ibusPayeeAccount.idecHealthInsurancePremiumAmount
                                                + ibusPayeeAccount.idecLifePremiumAmount + ibusPayeeAccount.idecChildSupportAmount
                                                + ibusPayeeAccount.idecVisionInsurancePremiumAmount + ibusPayeeAccount.idecMedicareInsPremiumAmount
                                                //+ ibusPayeeAccount.idecTotalDeductionsAmount
                                                + ibusPayeeAccount.idecNDPEADuesAmount + ibusPayeeAccount.idecAFPEDuesAmount
                                                + idecFedTaxAmount + idecStateTaxAmount;
            idecNetAmount = idecBenefitAmount - ldecAmountTobeDeducted;

            decimal ldecNewAmountTobeDeducted = ibusPayeeAccount.idecDentalInsurancePremiumAmount + ibusPayeeAccount.idecHealthInsurancePremiumAmount
                                                        + ibusPayeeAccount.idecLifePremiumAmount + ibusPayeeAccount.idecChildSupportAmount
                                                        + ibusPayeeAccount.idecVisionInsurancePremiumAmount + ibusPayeeAccount.idecMedicareInsPremiumAmount
                //+ ibusPayeeAccount.idecTotalDeductionsAmount
                                                        + ibusPayeeAccount.idecNDPEADuesAmount + ibusPayeeAccount.idecAFPEDuesAmount
                                                        + idecNewFedTaxAmount + idecNewStateTaxAmount;
            idecNetNewBenefitAmount = idecNewBenefitAmount - ldecNewAmountTobeDeducted;
        }

        #endregion

        public string istrCOLAEffectiveDateLongDate
        {
            get
            {
                return icdoPostRetirementIncreaseBatchRequest.effective_date.ToString(busConstant.DateFormatLongDate);
            }
        }

        public bool iblnIsEffectiveDateOverlappingForCOLA()
        {
            if (!iblnIsCancelledButtonClicked)
            {
                DataTable ldtbList = Select<cdoPostRetirementIncreaseBatchRequest>(new string[2] { "post_retirement_increase_type_value", "effective_date" },
                    new object[2] { busConstant.PostRetirementIncreaseTypeValueCOLA, icdoPostRetirementIncreaseBatchRequest.effective_date }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    if (ldtbList.AsEnumerable().Where(lrow => ((lrow.Field<string>("batch_request_status_value") == busConstant.PostRetirementIncreaseBatchTypeValueUnProcessed
                        && lrow.Field<string>("action_status_value") != busConstant.BenefitActionStatusCancelled)
                        //||
                        //(lrow.Field<string>("batch_request_status_value") == busConstant.PostRetirementIncreaseBatchTypeValueUnProcessed
                        //&& lrow.Field<string>("status_value") == busConstant.ApplicationStatusValid)
                        )
                        && (lrow.Field<int>("post_retirement_increase_batch_request_id")
                        != icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id)).Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
