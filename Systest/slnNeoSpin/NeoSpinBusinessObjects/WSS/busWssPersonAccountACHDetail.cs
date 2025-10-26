using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;
using System.Collections;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.DataObjects;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busWssPersonAccountACHDetail : busPersonAccountAchDetail
    {
        public busPerson ibusPerson { get; set; }
        public busPersonAccountAchDetail ibusPersonAccountACHDetail { get; set; }
        public busPersonAccountPaymentElection ibusPersonAccountPaymentElection { get; set; }
        public busPayeeAccount ibusPayeeAccount { get; set; }
        public cdoPayeeAccount icdoPayeeAccount { get; set; }
        public busOrganization ibusOrganization { get; set; }

        public Collection<busPersonAccount> iclbEnrolledPlans { get; set; }
        public Collection<busPersonAccountAchDetail> iclbACHDetailsWithEndDateNull { get; set; }
        public Collection<busPayeeAccountPaymentItemType> iclbPayeeAccountPaymentItemType { get; set; }
        public Collection<busWssAcknowledgement> iclbACHDetailsAcknowledgement { get; set; }
        public Collection<busPersonAccount> iclbPlansWithholdingFromBenefitPayment { get; set; }
        public Collection<busPersonAccount> iclbPlansWithPremiumDeduction { get; set; }
        public Collection<cdoPayeeAccount> iclbRetirementPayeeAccount { get; set; }

        public DataTable idtbPaymentItemType { get; set; }

        public int iintPayeeAccountACHID { get; set; }
        public string istrRoutingNumber { get; set; }
        public int iintPersonID { get; set; }
        public string istrAcknowledgementFlag { get; set; }
        public DateTime idtNextBenefitPaymentDate { get; set; }
        public string istrPaymentElectionHealthMedicare { get; set; }
        public string istrPaymentElectionDental { get; set; }
        public string istrPaymentElectionVision { get; set; }
        public string istrPaymentElectionLife { get; set; }

        public DateTime idtEffectiveDateHealthMedicare { get; set; }
        public DateTime idtEffectiveDateDental { get; set; }
        public DateTime idtEffectiveDateVision { get; set; }
        public DateTime idtEffectiveDateLife { get; set; }
        public DateTime idtACHEffectiveDate { get; set; }
        //Properties to display on Authorization and Print steps
        public string istrPaymentElectionHealthMedicareDisplay { get; set; }
        public string istrPaymentElectionDentalDisplay { get; set; }
        public string istrPaymentElectionVisionDisplay { get; set; }
        public string istrPaymentElectionLifeDisplay { get; set; }
        public string istrBenefitAccountType { get; set; }
        public string istrPlanName { get; set; }
        public string istrBenefitOption { get; set; }
        public string istrBenefitAmount { get; set; }

        public bool iblnIsRoutingNumberExists { get; set; }
        public bool iblnOrgIDDoesNotExist { get; set; }
        public bool iblnRNExists { get; set; }
        //PIR 24989 
        public string istrHealthandMedicarePersonAccountIds { get; set; }
        public string istrDentalPersonAccountId { get; set; }
        public string istrVisionPersonAccountId { get; set; }
        public string istrLifePersonAccountId { get; set; }
        public bool isACHDetailsInsert { get; set; }
        public string istrConfirmationText
        {
            get
            {
                if (ibusPerson == null)
                    LoadPerson(iintPersonID);

                string luserName = ibusPerson.icdoPerson.FullName;
                DateTime Now = DateTime.Now;
                DataTable ldtbListWSSAck = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='" + busConstant.PersonAccountACHDetailAckowlegment + "'");
                busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                if (ldtbListWSSAck.Rows.Count > 0)
                    lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbListWSSAck.Rows[0]["acknowledgement_text"].ToString();
                string lstrConfimation = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, luserName, Now);
                return lstrConfimation;
            }
        }
        //PIR 23837	Add message to MSS when changing bank accounts for Insurance Premium Deductions after pre-note
        public string istrUPMWAcknowledgementText
        {
            get
            {
                string lstrConfimation = string.Empty;
                DateTime ldtmSysBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
                idtACHEffectiveDate = ldtmSysBatchDate.Day < 15 ? busGlobalFunctions.GetFirstDayofNextMonth(ldtmSysBatchDate) : busGlobalFunctions.GetFirstDayofNextMonth(ldtmSysBatchDate.AddMonths(1));
                DateTime ldtmLatestAccountHistoryDate = GetLatestAccountHistoryDate();
                idtACHEffectiveDate = ldtmLatestAccountHistoryDate <= idtACHEffectiveDate ? idtACHEffectiveDate : ldtmLatestAccountHistoryDate;
                if (ldtmSysBatchDate.Day >= 15)
                {
                    DataTable ldtbListWSSAck = Select("cdoWssAcknowledgement.SelectACHAck", new object[3] { busConstant.UpdatePaymentMethodWizardAcknowledgement, DateTime.Now, 1 });
                    busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                    lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                    if (ldtbListWSSAck?.Rows.Count > 0)
                    {
                        lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbListWSSAck.Rows[0]["acknowledgement_text"].ToString();
                        lstrConfimation = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, idtACHEffectiveDate.ToString(busConstant.DateFormatMMddyyyy));
                    }
                }
                return lstrConfimation;
            }
        }
        /// <summary>
        /// return the latest history change date //PIR 23837
        /// </summary>
        /// <returns></returns>
        private DateTime GetLatestAccountHistoryDate()
        {
            if (iclbPlansWithPremiumDeduction == null)
                LoadPlansWithPremiumDeduction();
            return iclbPlansWithPremiumDeduction.Count > 0 ? iclbPlansWithPremiumDeduction.Max(i => i.icdoPersonAccount.history_change_date) : DateTime.MinValue;
        }
        public bool LoadPerson(int aintPersonID)
        {
            if (ibusPerson == null)
                ibusPerson = new busPerson();

            return ibusPerson.FindPerson(aintPersonID);
        }

        public void LoadCurrentlyEnrolledPlans()
        {
            iclbEnrolledPlans = new Collection<busPersonAccount>();
            istrHealthandMedicarePersonAccountIds = string.Empty;
            istrDentalPersonAccountId = string.Empty;
            istrVisionPersonAccountId = string.Empty;
            istrLifePersonAccountId = string.Empty;
            ibusPerson.LoadInsuranceAccounts();

            foreach (busPersonAccount lbusPersonAccount in ibusPerson.iclbInsuranceAccounts)
            {
                if (((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth ||
                    lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD) ||
                    lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental ||
                    lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision))
                {
                    lbusPersonAccount.ibusPersonAccountGHDV = new busPersonAccountGhdv() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                    lbusPersonAccount.ibusPersonAccountGHDV.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id);

                    if ((lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree ||
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree ||
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree) ||
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())
                    {
                        busPersonAccountGhdvHistory lobjGHDVHistory = lbusPersonAccount.ibusPersonAccountGHDV.LoadHistoryByDate(DateTime.Now);
                        if (lobjGHDVHistory.IsNotNull() && lobjGHDVHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        {
                            iclbEnrolledPlans.Add(lbusPersonAccount);

                            if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                                istrHealthandMedicarePersonAccountIds += istrHealthandMedicarePersonAccountIds.IsNotNull() ? lbusPersonAccount.icdoPersonAccount.person_account_id.ToString() + "," : lbusPersonAccount.icdoPersonAccount.person_account_id.ToString();
                            else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                                istrDentalPersonAccountId = lbusPersonAccount.icdoPersonAccount.person_account_id.ToString();
                            else if(lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                                istrVisionPersonAccountId = lbusPersonAccount.icdoPersonAccount.person_account_id.ToString();
                        }
                    }
                    if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        iclbEnrolledPlans.Add(lbusPersonAccount);
                        istrHealthandMedicarePersonAccountIds += istrHealthandMedicarePersonAccountIds.IsNotNull() ? lbusPersonAccount.icdoPersonAccount.person_account_id.ToString() + "," : lbusPersonAccount.icdoPersonAccount.person_account_id.ToString();
                    }
                }
                if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                {
                    lbusPersonAccount.ibusPersonAccountLife = new busPersonAccountLife() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountLife = new cdoPersonAccountLife() };
                    lbusPersonAccount.ibusPersonAccountLife.FindPersonAccountLife(lbusPersonAccount.icdoPersonAccount.person_account_id);
                    lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;

                    if (lbusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeRetireeMember)
                    {
                        lbusPersonAccount.ibusPersonAccountLife.LoadHistory();
                        if (lbusPersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeHistory.Where(o => busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                                            o.icdoPersonAccountLifeHistory.effective_start_date, o.icdoPersonAccountLifeHistory.effective_end_date) &&
                                            o.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Any())
                        {
                            iclbEnrolledPlans.Add(lbusPersonAccount);
                            istrLifePersonAccountId = lbusPersonAccount.icdoPersonAccount.person_account_id.ToString();
                        }
                    }
                }
            }
            istrHealthandMedicarePersonAccountIds = istrHealthandMedicarePersonAccountIds.TrimEnd(',');
        }

        //Visibility rule on Step 1 for Health and Medicare
        public bool IsHealthMedicareEnrolled()
        {
            if (ibusPerson.iclbInsuranceAccounts == null)
                ibusPerson.LoadInsuranceAccounts();
            if (iclbEnrolledPlans == null)
                LoadCurrentlyEnrolledPlans();

            var lclbHealthMedicareEnrolled = iclbEnrolledPlans.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth || o.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD);
            if (lclbHealthMedicareEnrolled.Count() > 0)
                return true;

            return false;
        }

        //Visibility rule on Step 1 for Dental
        public bool IsDentalEnrolled()
        {
            if (ibusPerson.iclbInsuranceAccounts == null)
                ibusPerson.LoadInsuranceAccounts();
            if (iclbEnrolledPlans == null)
                LoadCurrentlyEnrolledPlans();

            var lclbHealthMedicareEnrolled = iclbEnrolledPlans.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdDental);
            if (lclbHealthMedicareEnrolled.Count() > 0)
                return true;

            return false;
        }

        //Visibility rule on Step 1 for Vision
        public bool IsVisionEnrolled()
        {
            if (ibusPerson.iclbInsuranceAccounts == null)
                ibusPerson.LoadInsuranceAccounts();
            if (iclbEnrolledPlans == null)
                LoadCurrentlyEnrolledPlans();

            var lclbHealthMedicareEnrolled = iclbEnrolledPlans.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdVision);
            if (lclbHealthMedicareEnrolled.Count() > 0)
                return true;

            return false;
        }

        //Visibility rule on Step 1 for Life
        public bool IsLifeEnrolled()
        {
            if (ibusPerson.iclbInsuranceAccounts == null)
                ibusPerson.LoadInsuranceAccounts();
            if (iclbEnrolledPlans == null)
                LoadCurrentlyEnrolledPlans();

            var lclbHealthMedicareEnrolled = iclbEnrolledPlans.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife);
            if (lclbHealthMedicareEnrolled.Count() > 0)
                return true;

            return false;
        }
        public override void BeforeWizardStepValidate(utlPageMode aenmPageMode, string astrWizardName, string astrWizardStepName, utlWizardNavigationEventArgs we = null)
        {
            //Step 2
            if (astrWizardStepName == "wzsBankAccountInfo")
            {
                iblnOrgIDDoesNotExist = false;
                //To display Payment Election Method on Acknowledgment and Print step
                DisplayPaymentElectionMethod();
                LoadACHDetailsAcknowledgement();

                if (ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.istrRoutingNumber.IsNotNullOrEmpty())
                {
                    LoadBankOrgByRoutingNumber(ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.istrRoutingNumber);
                    if (ibusBankOrg.icdoOrganization.org_id == 0)
                        iblnOrgIDDoesNotExist = true; //To check if routing number does not exists then bank name is mandatory.

                    if (!iblnOrgIDDoesNotExist)
                    {
                        ibusOrganization = new busOrganization();
                        ibusOrganization = GetBankNameForWithdrawal(ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.istrRoutingNumber);
                        ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.istrBankName = ibusOrganization.icdoOrganization.org_name;
                        iblnRNExists = ibusOrganization.icdoOrganization.iblnIsRoutingNumberExists;
                    }
                }

                ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.bank_account_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1706, ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.bank_account_type_value);

            }
            //Step 3
            if (astrWizardStepName == "wzsRetirementPayment")
            {
                LoadPremiumDeductionAsPerPlan();

                LoadACHDetailsAcknowledgement();

                //To display Payment Election Method on Acknowledgment and Print step
                DisplayPaymentElectionMethod();

                //To display selected Retirement Account 
                DisplaySelectedRetirementPaymentAccount();
            }
            //Step 4
            if (astrWizardStepName == "wzsReviewAndAuthorize")
            {
                isACHDetailsInsert = false;
                if (!IsAcknowledgementNotSelected() && !IsACHDetailRecordExistsWithSameStartDate())
                {
                    //Create new ACH Detail
                    CreatePersonAccountACHDetailForEnrolledPlans();
                    isACHDetailsInsert = true;
                    //Create PAPIT and Person Account Payment Election entries.
                    CreatePaymentElectionAndPAPIT();

                    DisplayEffectiveDateOnPrintScreen();

                    busWSSHelper.PublishMSSMessage(0, 0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10319, iobjPassInfo), "Payment Method for Premium Deduction"), busConstant.WSS_MessageBoard_Priority_High,
                    ibusPerson.icdoPerson.person_id);

                    if (this.iarrChangeLog.Count() > 0)
                    {
                        foreach (object item in iarrChangeLog.ToList())
                        {
                            iarrChangeLog.Remove(item);
                        }
                    }
                }

            }
            base.BeforeWizardStepValidate(aenmPageMode, astrWizardName, astrWizardStepName);
        }

        //Method to load Premium amounts of plans
        public void LoadPremiumDeductionAsPerPlan()
        {
            if (ibusPerson.iclbInsuranceAccounts == null)
                ibusPerson.LoadInsuranceAccounts();
            if (iclbEnrolledPlans == null)
                LoadCurrentlyEnrolledPlans();

            iclbPlansWithholdingFromBenefitPayment = new Collection<busPersonAccount>();

            foreach (busPersonAccount lbusPersonAccount in iclbEnrolledPlans)
            {
                //Dental
                if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                {
                    if (istrPaymentElectionDental == busConstant.AutomaticWithholdingFromBenefitPayment)
                    {
                        lbusPersonAccount.ibusPersonAccountGHDV = new busPersonAccountGhdv() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                        lbusPersonAccount.ibusPersonAccountGHDV.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountGHDV.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountGHDV.LoadPlan();
                        lbusPersonAccount.ibusPersonAccountGHDV.ibusProviderOrgPlan = new busOrgPlan() { icdoOrgPlan = new cdoOrgPlan() };
                        lbusPersonAccount.ibusPersonAccountGHDV.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id = ibusPerson.LoadDefaultOrgPlanIdByPlanId(lbusPersonAccount.icdoPersonAccount.plan_id);
                        if (lbusPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate == DateTime.MinValue)
                            lbusPersonAccount.ibusPersonAccountGHDV.LoadPlanEffectiveDate();
                        lbusPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmount(lbusPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate);
                        lbusPersonAccount.idecMSSMonthlyPremiumAmount = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
                        iclbPlansWithholdingFromBenefitPayment.Add(lbusPersonAccount);
                    }
                }

                //Vision
                else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                {
                    if (istrPaymentElectionVision == busConstant.AutomaticWithholdingFromBenefitPayment)
                    {
                        lbusPersonAccount.ibusPersonAccountGHDV = new busPersonAccountGhdv() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                        lbusPersonAccount.ibusPersonAccountGHDV.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountGHDV.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountGHDV.LoadPlan();
                        lbusPersonAccount.ibusPersonAccountGHDV.ibusProviderOrgPlan = new busOrgPlan() { icdoOrgPlan = new cdoOrgPlan() };
                        lbusPersonAccount.ibusPersonAccountGHDV.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id = ibusPerson.LoadDefaultOrgPlanIdByPlanId(lbusPersonAccount.icdoPersonAccount.plan_id);
                        if (lbusPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate == DateTime.MinValue)
                            lbusPersonAccount.ibusPersonAccountGHDV.LoadPlanEffectiveDate();
                        lbusPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmount(lbusPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate);
                        lbusPersonAccount.idecMSSMonthlyPremiumAmount = lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
                        iclbPlansWithholdingFromBenefitPayment.Add(lbusPersonAccount);
                    }
                }

                //Health
                else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (istrPaymentElectionHealthMedicare == busConstant.AutomaticWithholdingFromBenefitPayment)
                    {
                        lbusPersonAccount.LoadPremiumMonthlyAmount();
                        lbusPersonAccount.idecMSSMonthlyPremiumAmount = lbusPersonAccount.idecMonthlyPremiumAmountByPlan;
                        iclbPlansWithholdingFromBenefitPayment.Add(lbusPersonAccount);
                    }
                }
                //Medicare
                else if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    if (istrPaymentElectionHealthMedicare == busConstant.AutomaticWithholdingFromBenefitPayment)
                    {
                        lbusPersonAccount.ibusPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                        lbusPersonAccount.ibusPersonAccountMedicarePartDHistory.FindMedicareByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountMedicarePartDHistory.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountMedicarePartDHistory.LoadPlan();
                        lbusPersonAccount.ibusPersonAccountMedicarePartDHistory.GetMonthlyPremiumAmountForMedicarePartD();
                        lbusPersonAccount.idecMSSMonthlyPremiumAmount = lbusPersonAccount.ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount;
                        iclbPlansWithholdingFromBenefitPayment.Add(lbusPersonAccount);
                    }
                }
                //Life
                else
                {
                    if (istrPaymentElectionLife == busConstant.AutomaticWithholdingFromBenefitPayment)
                    {
                        lbusPersonAccount.ibusPersonAccountLife = new busPersonAccountLife() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountLife = new cdoPersonAccountLife() };
                        lbusPersonAccount.ibusPersonAccountLife.FindPersonAccountLife(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountLife.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        lbusPersonAccount.ibusPersonAccountLife.LoadPlan();
                        lbusPersonAccount.ibusPersonAccountLife.LoadLifeOption();
                        if (lbusPersonAccount.ibusPersonAccountLife.idtPlanEffectiveDate == DateTime.MinValue)
                            lbusPersonAccount.ibusPersonAccountLife.LoadPlanEffectiveDate();
                        lbusPersonAccount.ibusPersonAccountLife.GetMonthlyPremiumAmount(lbusPersonAccount.ibusPersonAccountLife.idtPlanEffectiveDate, ibusPerson.LoadDefaultOrgPlanIdByPlanId(busConstant.PlanIdGroupLife));
                        lbusPersonAccount.idecMSSMonthlyPremiumAmount = lbusPersonAccount.ibusPersonAccountLife.idecTotalMonthlyPremium;
                        iclbPlansWithholdingFromBenefitPayment.Add(lbusPersonAccount);
                    }
                }
            }
        }

        public void LoadPlansWithPremiumDeduction()
        {
            if (ibusPerson.iclbInsuranceAccounts == null)
                ibusPerson.LoadInsuranceAccounts();
            if (iclbEnrolledPlans == null)
                LoadCurrentlyEnrolledPlans();

            iclbPlansWithPremiumDeduction = new Collection<busPersonAccount>();

            foreach (busPersonAccount lobjPersonAccount in iclbEnrolledPlans)
            {
                if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                {
                    if (istrPaymentElectionDental == busConstant.AutomaticPremiumDeduction)
                        iclbPlansWithPremiumDeduction.Add(lobjPersonAccount);
                }

                else if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                {
                    if (istrPaymentElectionVision == busConstant.AutomaticPremiumDeduction)
                        iclbPlansWithPremiumDeduction.Add(lobjPersonAccount);
                }

                else if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (istrPaymentElectionHealthMedicare == busConstant.AutomaticPremiumDeduction)
                        iclbPlansWithPremiumDeduction.Add(lobjPersonAccount);
                }

                else if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    if (istrPaymentElectionHealthMedicare == busConstant.AutomaticPremiumDeduction)
                        iclbPlansWithPremiumDeduction.Add(lobjPersonAccount);
                }

                else if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                {
                    if (istrPaymentElectionLife == busConstant.AutomaticPremiumDeduction)
                        iclbPlansWithPremiumDeduction.Add(lobjPersonAccount);
                }
            }
        }

        //Method to end the previous ACH Details
        public void EndExistingACHDetailsPremiumDeduction()
        {
            if (iclbPlansWithPremiumDeduction == null)
                LoadPlansWithPremiumDeduction();

            LoadAllACHDetailsWithEndDateNullMSS();

            if (iclbACHDetailsWithEndDateNull != null && iclbACHDetailsWithEndDateNull.Count > 0)
            {
                foreach (busPersonAccountAchDetail lobjACHDetail in iclbACHDetailsWithEndDateNull)
                {
                    if (iclbPlansWithPremiumDeduction.Where(o => o.icdoPersonAccount.plan_id == lobjACHDetail.icdoPersonAccountAchDetail.plan_id_ach).Any())
                    {
                        lobjACHDetail.icdoPersonAccountAchDetail.ach_end_date = lobjACHDetail.icdoPersonAccountAchDetail.ach_start_date == idtACHEffectiveDate ? idtACHEffectiveDate : idtACHEffectiveDate.AddDays(-1);//PIR 23837
                        lobjACHDetail.icdoPersonAccountAchDetail.Update();
                    }
                }
            }
        }

        //End ACH if Prev ACH -> Payee Account.
        public void EndExistingACHDetailsBenefitPayment()
        {
            if (iclbPlansWithholdingFromBenefitPayment == null)
                LoadPremiumDeductionAsPerPlan();

            LoadAllACHDetailsWithEndDateNullMSS();

            if (iclbACHDetailsWithEndDateNull != null)
            {
                foreach (busPersonAccountAchDetail lobjACHDetail in iclbACHDetailsWithEndDateNull)
                {
                    if (iclbPlansWithholdingFromBenefitPayment.Where(o => o.icdoPersonAccount.plan_id == lobjACHDetail.icdoPersonAccountAchDetail.plan_id_ach).Any())
                    {
                        lobjACHDetail.icdoPersonAccountAchDetail.ach_end_date = lobjACHDetail.icdoPersonAccountAchDetail.ach_start_date == idtACHEffectiveDate ? idtACHEffectiveDate : idtACHEffectiveDate.AddDays(-1);//PIR 23837
                        lobjACHDetail.icdoPersonAccountAchDetail.Update();
                    }
                }
            }
        }

        //Method to create ACH Details
        //Prev ACH to ACH - Create new ACH and end previous one.
        //Prev Payee Account to ACH - Create new ACH, end PAPIT with 1 day prior to ACH start date and remove payee account id from Payment Election with payment method as 'ACH'
        public void CreatePersonAccountACHDetailForEnrolledPlans()
        {
            if (iclbEnrolledPlans == null)
                LoadCurrentlyEnrolledPlans();
            //iclbPlansWithPremiumDeduction - Plans which selected 'Automatic Premium Deduction'
            if (iclbPlansWithPremiumDeduction == null)
                LoadPlansWithPremiumDeduction();

            // Will enter only if 'Automatic Premium Deduction' is selected
            if (iclbPlansWithPremiumDeduction.Count > 0)
            {
                int iintBankOrgID = 0;
                LoadBankOrgByRoutingNumber(ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.istrRoutingNumber);
                if (ibusBankOrg.icdoOrganization.org_id > 0)
                    iintBankOrgID = ibusBankOrg.icdoOrganization.org_id;
                else
                {
                    //Create workflow 
                    InsertOrgBankForRoutingNumber(ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.istrBankName, Convert.ToInt32(ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.istrRoutingNumber));

                    LoadBankOrgByRoutingNumber(ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.istrRoutingNumber);
                    InitializeWorkflow(ibusBankOrg.icdoOrganization.org_id, busConstant.Map_Process_Create_And_Maintain_Organization_Information);

                    iintBankOrgID = ibusBankOrg.icdoOrganization.org_id;
                }

                //Load ACH for Medicare before end dating the existing ACH to get the ACH of member and spouse
                DataTable ldtbList = Select("cdoPersonAccountAchDetail.LoadACHForMedicare", new object[1] { iintPersonID });
                var lclbACHMedicare = GetCollection<busPersonAccountAchDetail>(ldtbList, "icdoPersonAccountAchDetail");

                //End Existing ACH Details.
                EndExistingACHDetailsPremiumDeduction();

                //Insert ACH details for plans with Premium Deduction.
                foreach (busPersonAccount lobjPersonAccount in iclbPlansWithPremiumDeduction)
                {
                    if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        //Create ACH detail for Member and Spouse.
                        foreach (busPersonAccountAchDetail lobjACHMedicare in lclbACHMedicare)
                        {
                            ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.person_account_id = lobjACHMedicare.icdoPersonAccountAchDetail.person_account_id;
                            ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.pre_note_flag = busConstant.Flag_Yes;
                            ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.bank_org_id = iintBankOrgID;
                            ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.aba_number = Convert.ToInt32(ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.istrRoutingNumber);
                            //PIR 23837	Add message to MSS when changing bank accounts for Insurance Premium Deductions after pre-note
                            ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.ach_start_date = idtACHEffectiveDate;
                            ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.Insert();

                            //Update Person Account history change date
                            lobjPersonAccount.icdoPersonAccount.history_change_date = idtACHEffectiveDate; //PIR 23837
                            lobjPersonAccount.icdoPersonAccount.Update();

                            //Insert History record
                            InsertHistoryForMedicare(lobjPersonAccount);
                        }

                    }
                    else
                    {
                        ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.person_account_id = lobjPersonAccount.icdoPersonAccount.person_account_id;
                        ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.pre_note_flag = busConstant.Flag_Yes;
                        ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.bank_org_id = iintBankOrgID;
                        ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.aba_number = Convert.ToInt32(ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.istrRoutingNumber);
                        //PIR 23837	Add message to MSS when changing bank accounts for Insurance Premium Deductions after pre-note
                        ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.ach_start_date = idtACHEffectiveDate;
                        ibusPersonAccountACHDetail.icdoPersonAccountAchDetail.Insert();

                        //Update Person Account history change date
                        lobjPersonAccount.icdoPersonAccount.history_change_date = idtACHEffectiveDate;//PIR 23837
                        lobjPersonAccount.icdoPersonAccount.Update();

                        //Insert History record
                        if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth ||
                            lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental ||
                            lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                        {
                            busPersonAccountGhdv lobjPersonAccountGHDV = new busPersonAccountGhdv() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                            lobjPersonAccountGHDV.FindGHDVByPersonAccountID(lobjPersonAccount.icdoPersonAccount.person_account_id);
                            lobjPersonAccountGHDV.FindPersonAccount(lobjPersonAccount.icdoPersonAccount.person_account_id);
                            lobjPersonAccountGHDV.LoadPreviousHistory();

                            //End previous history record
                            if (lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id > 0)
                            {
                                if (lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.start_date == lobjPersonAccount.icdoPersonAccount.history_change_date)
                                {
                                    lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.end_date = lobjPersonAccount.icdoPersonAccount.history_change_date;
                                }
                                else
                                {
                                    lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.end_date = lobjPersonAccount.icdoPersonAccount.history_change_date.AddDays(-1);
                                }
                                lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.Update();
                            }
                            //PIR 24236
                            if (lobjPersonAccountGHDV.IsHealthOrMedicare)
                            {
                                if (lobjPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                                {
                                    lobjPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                                }
                                else
                                {
                                    lobjPersonAccountGHDV.LoadHealthParticipationDate();
                                    lobjPersonAccountGHDV.LoadHealthPlanOption();
                                    lobjPersonAccountGHDV.LoadRateStructure();
                                }
                            }
                            lobjPersonAccountGHDV.icdoPersonAccountGhdv.reason_value = busConstant.ChangeReasonIBSPaymentMethod;
                            lobjPersonAccountGHDV.icdoPersonAccountGhdv.Update();

                            lobjPersonAccountGHDV.InsertHistory();
                        }

                        if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                        {
                            InsertHistoryForLife(lobjPersonAccount);
                        }
                    }
                }

                //Check if the plan which selected Premium Deduction was previously deducted through Benefit Payment.
                //If 'Yes' then 1. End PAPIT with 1 day prior to ACH Start date 
                //2. Remove payee account id from Payment election and Change Payment method to ACH

                UpdatePAPITAndPaymentElection();

            }

        }

        //Update PAPIT and Payment Election if previously the plan was deducted through Benefit Payment. (Prev Payee Account --> ACH)
        public void UpdatePAPITAndPaymentElection()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();

            //Load Payee Account which is selected
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            //if (idtNextBenefitPaymentDate == DateTime.MinValue)
            //    LoadNexBenefitPaymentDate();

            Collection<cdoPayeeAccount> iclbPayeeAccount = new Collection<cdoPayeeAccount>();

            if (iintPayeeAccountACHID > 0)
                lobjPayeeAccount.FindPayeeAccount(iintPayeeAccountACHID);
            else
            {
                DataTable ldtbList = Select("cdoPayeeAccount.LoadRetirementPayeeAccount", new object[2] { ibusPerson.icdoPerson.person_id, ibusPayeeAccount.idtNextBenefitPaymentDate });
                iclbPayeeAccount = Sagitec.DataObjects.doBase.GetCollection<cdoPayeeAccount>(ldtbList);
            }

            DataTable ldtbListMedicare = Select("cdoPersonAccountAchDetail.LoadACHForMedicare", new object[1] { iintPersonID });
            var lclbMedicare = GetCollection<busPersonAccount>(ldtbListMedicare, "icdoPersonAccount");

            if (iclbPlansWithPremiumDeduction != null && iclbPlansWithPremiumDeduction.Count > 0)
            {
                foreach (busPersonAccount lobjPersonAccount in iclbPlansWithPremiumDeduction)
                {
                    LoadPaymentElectionPerPlan(lobjPersonAccount.icdoPersonAccount.person_account_id);
                    //Update Payment Election
                    if (ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0)
                    {
                        if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                        {
                            if (lclbMedicare.Count > 0)
                            {
                                foreach (busPersonAccount lobjPA in lclbMedicare)
                                {
                                    //Load Payment election for Medicare
                                    LoadPaymentElectionPerPlan(lobjPA.icdoPersonAccount.person_account_id);

                                    ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payee_account_id = 0;
                                    ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payment_method_value = busConstant.IBSModeOfPaymentACH;
                                    ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.Update();

                                    //Insert Payment Election history
                                    busPersonAccountPaymentElectionHistory lbusPersonAccountPaymentElectionHistory = new busPersonAccountPaymentElectionHistory { icdoPersonAccountPaymentElectionHistory = new cdoPersonAccountPaymentElectionHistory() };
                                    lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection = ibusPersonAccountPaymentElection;
                                    //Load Person Account details of spouse as lobjPersonAccount will have member's details
                                    if (lobjPA.icdoPersonAccount.person_account_id != lobjPersonAccount.icdoPersonAccount.person_account_id)
                                    {
                                        lobjPA.LoadPersonAccountMedicare(lobjPA.icdoPersonAccount.person_account_id);
                                        lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPA.ibusPersonAccount.icdoPersonAccount;
                                    }
                                    else
                                        lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                                    lbusPersonAccountPaymentElectionHistory.InsertPaymentElectionHistory();

                                }
                            }
                        }
                        else
                        {
                            ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payee_account_id = 0;
                            ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payment_method_value = busConstant.IBSModeOfPaymentACH;
                            ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.Update();

                            //Insert Payment Election history
                            busPersonAccountPaymentElectionHistory lbusPersonAccountPaymentElectionHistory = new busPersonAccountPaymentElectionHistory { icdoPersonAccountPaymentElectionHistory = new cdoPersonAccountPaymentElectionHistory() };
                            lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection = ibusPersonAccountPaymentElection;
                            lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                            lbusPersonAccountPaymentElectionHistory.InsertPaymentElectionHistory();
                        }
                    }

                    //Update PAPIT
                    busPayeeAccountPaymentItemType lobjPAPIT;
                    DataTable ldtIBSInsuranceItems;
                    string lstrWhere = string.Empty;

                    lstrWhere = " code_id = 1708 and data1 = '" + lobjPersonAccount.icdoPersonAccount.plan_id.ToString() + "'";
                    ldtIBSInsuranceItems = iobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", lstrWhere);

                    //In case of Payee to ACH if there are 2 payee accounts, then to end PAPIT of related payee account
                    if (iclbPayeeAccount != null && iclbPayeeAccount.Count > 0)
                    {
                        foreach (cdoPayeeAccount lobjPA in iclbPayeeAccount)
                        {
                            busPayeeAccount lobjPayeeAcct = new busPayeeAccount();
                            lobjPayeeAcct.FindPayeeAccount(lobjPA.payee_account_id);

                            lobjPayeeAcct.LoadNexBenefitPaymentDate();

                            if ((ldtIBSInsuranceItems != null) && (ldtIBSInsuranceItems.Rows.Count > 0))
                            {
                                if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                                {
                                    if (lclbMedicare.Count > 0)
                                    {
                                        foreach (busPersonAccount lobjSpouse in lclbMedicare)
                                        {
                                            //Member
                                            if (lobjPersonAccount.icdoPersonAccount.person_account_id == lobjSpouse.icdoPersonAccount.person_account_id)
                                            {
                                                lobjPAPIT = lobjPayeeAcct.GetLatestPayeeAccountPaymentItemTypeMedicare(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(), lobjPayeeAcct.idtNextBenefitPaymentDate, lobjPersonAccount.icdoPersonAccount.person_account_id);
                                                if (lobjPAPIT != null)
                                                {
                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = busGlobalFunctions.GetFirstDayofNextMonth(DateTime.Now).AddDays(-1);
                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                                                }
                                            }
                                            else//Spouse
                                            {
                                                lobjPAPIT = lobjPayeeAcct.GetLatestPayeeAccountPaymentItemTypeMedicare(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(), lobjPayeeAcct.idtNextBenefitPaymentDate, lobjSpouse.icdoPersonAccount.person_account_id);
                                                if (lobjPAPIT != null)
                                                {
                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = busGlobalFunctions.GetFirstDayofNextMonth(DateTime.Now).AddDays(-1);
                                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                                                }
                                            }
                                        }

                                    }
                                }
                                else
                                {
                                    lobjPAPIT = lobjPayeeAcct.GetLatestPayeeAccountPaymentItemType(ldtIBSInsuranceItems.Rows[0]["data2"].ToString());
                                    if (lobjPAPIT != null)
                                    {
                                        lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = busGlobalFunctions.GetFirstDayofNextMonth(DateTime.Now).AddDays(-1);
                                        lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (lobjPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                            lobjPayeeAccount.LoadNexBenefitPaymentDate();
                        if ((ldtIBSInsuranceItems != null) && (ldtIBSInsuranceItems.Rows.Count > 0))
                        {
                            if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                            {
                                if (lclbMedicare.Count > 0)
                                {
                                    foreach (busPersonAccount lobjSpouse in lclbMedicare)
                                    {
                                        //Member
                                        if (lobjPersonAccount.icdoPersonAccount.person_account_id == lobjSpouse.icdoPersonAccount.person_account_id)
                                        {
                                            lobjPAPIT = lobjPayeeAccount.GetLatestPayeeAccountPaymentItemTypeMedicare(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(), lobjPayeeAccount.idtNextBenefitPaymentDate, lobjPersonAccount.icdoPersonAccount.person_account_id);
                                            if (lobjPAPIT != null)
                                            {
                                                lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = busGlobalFunctions.GetFirstDayofNextMonth(DateTime.Now).AddDays(-1);
                                                lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                                            }
                                        }
                                        else //Spouse
                                        {
                                            lobjPAPIT = lobjPayeeAccount.GetLatestPayeeAccountPaymentItemTypeMedicare(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(), lobjPayeeAccount.idtNextBenefitPaymentDate, lobjSpouse.icdoPersonAccount.person_account_id);
                                            if (lobjPAPIT != null)
                                            {
                                                lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = busGlobalFunctions.GetFirstDayofNextMonth(DateTime.Now).AddDays(-1);
                                                lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                lobjPAPIT = lobjPayeeAccount.GetLatestPayeeAccountPaymentItemType(ldtIBSInsuranceItems.Rows[0]["data2"].ToString());
                                if (lobjPAPIT != null)
                                {
                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = busGlobalFunctions.GetFirstDayofNextMonth(DateTime.Now).AddDays(-1);
                                    lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                                }
                            }
                        }
                    }
                }

            }

        }

        //Method to create Payment Election and PAPIT
        //Prev ACH to Payee Account - End ACH, Create Payment Election and PAPIT
        //Prev Payee Account to Payee Account - Do nothing.
        public void CreatePaymentElectionAndPAPIT()
        {
            if (iclbEnrolledPlans == null)
                LoadCurrentlyEnrolledPlans();

            //iclbBenefitPlanAccounts - Plans which selected 'Automatic withholding from Benefit Payment'.
            if (iclbPlansWithholdingFromBenefitPayment == null)
                LoadPremiumDeductionAsPerPlan();

            //Load Payee Account which is selected
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            lobjPayeeAccount.FindPayeeAccount(iintPayeeAccountACHID);

            Collection<cdoPayeeAccount> iclbPayeeAccount = new Collection<cdoPayeeAccount>();
            DataTable ldtbList = Select("cdoPayeeAccount.LoadRetirementPayeeAccount", new object[2] { ibusPerson.icdoPerson.person_id, ibusPayeeAccount.idtNextBenefitPaymentDate });
            iclbPayeeAccount = Sagitec.DataObjects.doBase.GetCollection<cdoPayeeAccount>(ldtbList);

            var iclbPA = iclbPayeeAccount.Where(o => o.payee_account_id != iintPayeeAccountACHID);

            if (lobjPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                lobjPayeeAccount.LoadNexBenefitPaymentDate();

            DataTable ldtbListMedicare = Select("cdoPersonAccountAchDetail.LoadACHForMedicare", new object[1] { iintPersonID });
            var lclbMedicare = GetCollection<busPersonAccount>(ldtbListMedicare, "icdoPersonAccount");

            foreach (busPersonAccount lobjPersonAccount in iclbPlansWithholdingFromBenefitPayment)
            {
                //If Payment Election already exists and member selects 'Automatic withholding from Benefit Payment', then update payee_account_id 
                //if different payee account is selected in Payment Election
                //Else create a new Payment Election.
                LoadPaymentElectionPerPlan(lobjPersonAccount.icdoPersonAccount.person_account_id);

                //Case ACH to Payee Account - if Payment Election exists with Payment Method ACH, then update IBS Flag, Payee Account id.
                if (ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0 &&
                    (ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payment_method_value != busConstant.IBSModeOfPaymentPensionCheck))
                {
                    if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        if (lclbMedicare.Count > 0)
                        {
                            foreach (busPersonAccount lobjPA in lclbMedicare)
                            {
                                //Load Payment election for Medicare
                                LoadPaymentElectionPerPlan(lobjPA.icdoPersonAccount.person_account_id);
                                if (ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.ibs_flag != busConstant.Flag_Yes)
                                    ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.ibs_flag = busConstant.Flag_No;
                                ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payee_account_id = iintPayeeAccountACHID;
                                ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payment_method_value = busConstant.IBSModeOfPaymentPensionCheck;
                                ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.Update();

                                //Insert Payment Election history
                                busPersonAccountPaymentElectionHistory lbusPAPaymentElectionHistory = new busPersonAccountPaymentElectionHistory { icdoPersonAccountPaymentElectionHistory = new cdoPersonAccountPaymentElectionHistory() };
                                lbusPAPaymentElectionHistory.ibusPersonAccountPaymentElection = ibusPersonAccountPaymentElection;

                                //Load Person Account details of spouse as lobjPersonAccount will have member's details
                                if (lobjPA.icdoPersonAccount.person_account_id != lobjPersonAccount.icdoPersonAccount.person_account_id)
                                {
                                    lobjPA.LoadPersonAccountMedicare(lobjPA.icdoPersonAccount.person_account_id);
                                    lbusPAPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPA.ibusPersonAccount.icdoPersonAccount;
                                }
                                else
                                    lbusPAPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                                lbusPAPaymentElectionHistory.InsertPaymentElectionHistory();

                                //Update Person Account history change date
                                lobjPersonAccount.icdoPersonAccount.history_change_date = busGlobalFunctions.GetFirstDayofNextMonth(DateTime.Now);
                                lobjPersonAccount.icdoPersonAccount.Update();

                                //Insert History record
                                InsertHistoryForMedicare(lobjPersonAccount);
                            }
                        }
                    }
                    else
                    {
                        if (ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.ibs_flag != busConstant.Flag_Yes)
                            ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.ibs_flag = busConstant.Flag_No;
                        ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payee_account_id = iintPayeeAccountACHID;
                        ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payment_method_value = busConstant.IBSModeOfPaymentPensionCheck;
                        ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.Update();

                        //Insert Payment Election history
                        busPersonAccountPaymentElectionHistory lbusPersonAccountPaymentElectionHistory = new busPersonAccountPaymentElectionHistory { icdoPersonAccountPaymentElectionHistory = new cdoPersonAccountPaymentElectionHistory() };
                        lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection = ibusPersonAccountPaymentElection;
                        lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                        lbusPersonAccountPaymentElectionHistory.InsertPaymentElectionHistory();

                        //Update Person Account history change date
                        lobjPersonAccount.icdoPersonAccount.history_change_date = idtACHEffectiveDate;//PIR 23837
                        lobjPersonAccount.icdoPersonAccount.Update();

                        //Insert History record
                        if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth ||
                            lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental ||
                            lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                        {
                            busPersonAccountGhdv lobjPersonAccountGHDV = new busPersonAccountGhdv() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                            lobjPersonAccountGHDV.FindGHDVByPersonAccountID(lobjPersonAccount.icdoPersonAccount.person_account_id);
                            lobjPersonAccountGHDV.FindPersonAccount(lobjPersonAccount.icdoPersonAccount.person_account_id);
                            lobjPersonAccountGHDV.LoadPreviousHistory();

                            //End previous history record
                            if (lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id > 0)
                            {
                                if (lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.start_date == lobjPersonAccount.icdoPersonAccount.history_change_date)
                                {
                                    lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.end_date = lobjPersonAccount.icdoPersonAccount.history_change_date;
                                }
                                else
                                {
                                    lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.end_date = lobjPersonAccount.icdoPersonAccount.history_change_date.AddDays(-1);
                                }
                                lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.Update();
                            }
                            //PIR 24236
                            if (lobjPersonAccountGHDV.IsHealthOrMedicare)
                            {
                                if (lobjPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                                {
                                    lobjPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                                }
                                else
                                {
                                    lobjPersonAccountGHDV.LoadHealthParticipationDate();
                                    lobjPersonAccountGHDV.LoadHealthPlanOption();
                                    lobjPersonAccountGHDV.LoadRateStructure();
                                }
                            }
                            lobjPersonAccountGHDV.icdoPersonAccountGhdv.reason_value = busConstant.ChangeReasonIBSPaymentMethod;
                            lobjPersonAccountGHDV.icdoPersonAccountGhdv.Update();

                            lobjPersonAccountGHDV.InsertHistory();
                        }

                        if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                        {
                            InsertHistoryForLife(lobjPersonAccount);
                        }
                    }

                }
                //Case Payee Account to Payee Account - if different Payee Account is selection for deduction then update the payee account id.
                else if (ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0 &&
                    ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck)
                {
                    if (ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payee_account_id != iintPayeeAccountACHID)
                    {
                        if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                        {
                            if (lclbMedicare.Count > 0)
                            {
                                foreach (busPersonAccount lobjPA in lclbMedicare)
                                {
                                    //Load Payment election for Medicare
                                    LoadPaymentElectionPerPlan(lobjPA.icdoPersonAccount.person_account_id);

                                    ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payee_account_id = iintPayeeAccountACHID;
                                    ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.Update();

                                    //Insert Payment Election history
                                    busPersonAccountPaymentElectionHistory lbusPAPaymentElectionHistory = new busPersonAccountPaymentElectionHistory { icdoPersonAccountPaymentElectionHistory = new cdoPersonAccountPaymentElectionHistory() };
                                    lbusPAPaymentElectionHistory.ibusPersonAccountPaymentElection = ibusPersonAccountPaymentElection;
                                    //Load Person Account details of spouse as lobjPersonAccount will have member's details
                                    if (lobjPA.icdoPersonAccount.person_account_id != lobjPersonAccount.icdoPersonAccount.person_account_id)
                                    {
                                        lobjPA.LoadPersonAccountMedicare(lobjPA.icdoPersonAccount.person_account_id);
                                        lbusPAPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPA.ibusPersonAccount.icdoPersonAccount;
                                    }
                                    else
                                        lbusPAPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                                    lbusPAPaymentElectionHistory.InsertPaymentElectionHistory();

                                    //Update Person Account history change date
                                    lobjPersonAccount.icdoPersonAccount.history_change_date = idtACHEffectiveDate; //PIR 23837
                                    lobjPersonAccount.icdoPersonAccount.Update();

                                    //Insert History record
                                    InsertHistoryForMedicare(lobjPersonAccount);
                                }
                            }
                        }
                        else
                        {
                            ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payee_account_id = iintPayeeAccountACHID;
                            ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.Update();

                            //Insert Payment Election history
                            busPersonAccountPaymentElectionHistory lbusPersonAccountPaymentElectionHistory = new busPersonAccountPaymentElectionHistory { icdoPersonAccountPaymentElectionHistory = new cdoPersonAccountPaymentElectionHistory() };
                            lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection = ibusPersonAccountPaymentElection;
                            lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                            lbusPersonAccountPaymentElectionHistory.InsertPaymentElectionHistory();

                            //Update Person Account history change date
                            lobjPersonAccount.icdoPersonAccount.history_change_date = idtACHEffectiveDate;//PIR 23837
                            lobjPersonAccount.icdoPersonAccount.Update();

                            //Insert History record
                            if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth ||
                                lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental ||
                                lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                            {
                                busPersonAccountGhdv lobjPersonAccountGHDV = new busPersonAccountGhdv() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                                lobjPersonAccountGHDV.FindGHDVByPersonAccountID(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                lobjPersonAccountGHDV.FindPersonAccount(lobjPersonAccount.icdoPersonAccount.person_account_id);
                                lobjPersonAccountGHDV.LoadPreviousHistory();

                                //End previous history record
                                if (lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id > 0)
                                {
                                    if (lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.start_date == lobjPersonAccount.icdoPersonAccount.history_change_date)
                                    {
                                        lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.end_date = lobjPersonAccount.icdoPersonAccount.history_change_date;
                                    }
                                    else
                                    {
                                        lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.end_date = lobjPersonAccount.icdoPersonAccount.history_change_date.AddDays(-1);
                                    }
                                    lobjPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.Update();
                                }
                                //PIR 24236
                                if (lobjPersonAccountGHDV.IsHealthOrMedicare)
                                {
                                    if (lobjPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                                    {
                                        lobjPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                                    }
                                    else
                                    {
                                        lobjPersonAccountGHDV.LoadHealthParticipationDate();
                                        lobjPersonAccountGHDV.LoadHealthPlanOption();
                                        lobjPersonAccountGHDV.LoadRateStructure();
                                    }
                                }
                                lobjPersonAccountGHDV.icdoPersonAccountGhdv.reason_value = busConstant.ChangeReasonIBSPaymentMethod;
                                lobjPersonAccountGHDV.icdoPersonAccountGhdv.Update();

                                lobjPersonAccountGHDV.InsertHistory();
                            }

                            if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                            {
                                InsertHistoryForLife(lobjPersonAccount);
                            }
                        }
                    }

                }
                ////Case ACH to Payee Account - if Payment Election does not exists, create one.
                else
                {
                    if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        if (lclbMedicare.Count > 0)
                        {
                            foreach (busPersonAccount lobjPA in lclbMedicare)
                            {
                                //Load Payment election for Medicare
                                LoadPaymentElectionPerPlan(lobjPA.icdoPersonAccount.person_account_id);

                                ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.ibs_flag = busConstant.Flag_No;
                                ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payee_account_id = iintPayeeAccountACHID;
                                ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payment_method_value = busConstant.IBSModeOfPaymentPensionCheck;
                                ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date = lobjPersonAccount.icdoPersonAccount.start_date;
                                ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.Update();

                                //Insert Payment Election history
                                busPersonAccountPaymentElectionHistory lbusPAPaymentElectionHistory = new busPersonAccountPaymentElectionHistory { icdoPersonAccountPaymentElectionHistory = new cdoPersonAccountPaymentElectionHistory() };
                                lbusPAPaymentElectionHistory.ibusPersonAccountPaymentElection = ibusPersonAccountPaymentElection;
                                //Load Person Account details of spouse as lobjPersonAccount will have member's details
                                if (lobjPA.icdoPersonAccount.person_account_id != lobjPersonAccount.icdoPersonAccount.person_account_id)
                                {
                                    lobjPA.LoadPersonAccountMedicare(lobjPA.icdoPersonAccount.person_account_id);
                                    lbusPAPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPA.ibusPersonAccount.icdoPersonAccount;
                                }
                                else
                                    lbusPAPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                                lbusPAPaymentElectionHistory.InsertPaymentElectionHistory();
                            }
                        }
                    }
                    else
                    {
                        ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.ibs_flag = busConstant.Flag_No;
                        ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payment_method_value = busConstant.IBSModeOfPaymentPensionCheck;
                        ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date = lobjPersonAccount.icdoPersonAccount.start_date;
                        ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.payee_account_id = iintPayeeAccountACHID;
                        ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.Insert();

                        //Insert Payment Election history
                        busPersonAccountPaymentElectionHistory lbusPersonAccountPaymentElectionHistory = new busPersonAccountPaymentElectionHistory { icdoPersonAccountPaymentElectionHistory = new cdoPersonAccountPaymentElectionHistory() };
                        lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection = ibusPersonAccountPaymentElection;
                        lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                        lbusPersonAccountPaymentElectionHistory.InsertPaymentElectionHistory();
                    }
                }

                busPayeeAccountPaymentItemType lobjPAPIT;
                decimal ldecAmount = 0.0M;
                DataTable ldtIBSInsuranceItems;
                string lstrWhere = string.Empty;

                lstrWhere = " code_id = 1708 and data1 = '" + lobjPersonAccount.icdoPersonAccount.plan_id.ToString() + "'";
                ldtIBSInsuranceItems = iobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", lstrWhere);
                if ((ldtIBSInsuranceItems != null) && (ldtIBSInsuranceItems.Rows.Count > 0))
                {
                    if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                        lobjPAPIT = lobjPayeeAccount.GetLatestPayeeAccountPaymentItemTypeMedicare(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(), lobjPayeeAccount.idtNextBenefitPaymentDate, lobjPersonAccount.icdoPersonAccount.person_account_id);
                    else
                        lobjPAPIT = lobjPayeeAccount.GetLatestPayeeAccountPaymentItemType(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(), lobjPayeeAccount.idtNextBenefitPaymentDate);
                    if (lobjPAPIT != null)
                    {
                        if (lobjPAPIT.icdoPayeeAccountPaymentItemType.start_date > lobjPayeeAccount.idtNextBenefitPaymentDate)
                        {
                            lobjPAPIT.icdoPayeeAccountPaymentItemType.Delete();
                            lobjPayeeAccount.LoadPayeeAccountPaymentItemType();
                        }
                        else
                            ldecAmount = lobjPAPIT.icdoPayeeAccountPaymentItemType.amount;
                    }

                    //For Medicare Part D total premium amount (member amount + spouse amount) should be inserted.
                    if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        if (lclbMedicare.Count > 0)
                        {
                            foreach (busPersonAccount lobjPA in lclbMedicare)
                            {
                                //Member
                                if (lobjPA.icdoPersonAccount.person_account_id == lobjPersonAccount.icdoPersonAccount.person_account_id)
                                {
                                    if (Math.Round(ldecAmount, 2, MidpointRounding.AwayFromZero) != Math.Round(lobjPersonAccount.idecMSSMonthlyPremiumAmount, 2, MidpointRounding.AwayFromZero))
                                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(),
                                                                                            lobjPersonAccount.idecMSSMonthlyPremiumAmount,
                                                                                            string.Empty,
                                                                                            lobjPersonAccount.icdoPersonAccount.provider_org_id,
                                                                                            lobjPayeeAccount.idtNextBenefitPaymentDate,
                                                                                            DateTime.MinValue,
                                                                                            false, false, lobjPersonAccount.icdoPersonAccount.person_account_id, true);
                                }
                                else //Spouse
                                {
                                    //Load premium amount for Spouse.
                                    busPersonAccountMedicarePartDHistory lobjSpouseMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                                    lobjSpouseMedicare.FindMedicareByPersonAccountID(lobjPA.icdoPersonAccount.person_account_id);
                                    lobjSpouseMedicare.FindPersonAccount(lobjPA.icdoPersonAccount.person_account_id);
                                    lobjSpouseMedicare.LoadPlan();
                                    lobjSpouseMedicare.GetMonthlyPremiumAmountForMedicarePartD();

                                    if (Math.Round(ldecAmount, 2, MidpointRounding.AwayFromZero) != Math.Round(lobjSpouseMedicare.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount, 2, MidpointRounding.AwayFromZero))
                                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(),
                                                                                            lobjSpouseMedicare.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount,
                                                                                            string.Empty,
                                                                                            lobjSpouseMedicare.icdoPersonAccount.provider_org_id,
                                                                                            lobjPayeeAccount.idtNextBenefitPaymentDate,
                                                                                            DateTime.MinValue,
                                                                                            false, false, lobjSpouseMedicare.icdoPersonAccount.person_account_id, true);
                                }
                            }
                        }
                    }
                    else
                    {
                        //PIR 26292	MSS Payment Method Update Wizard -- The wizard should not update PAPIT that already exist and should insert with the correct premium amounts where they don’t exist. 
                        if (lobjPAPIT.IsNull())
                            if (Math.Round(ldecAmount, 2, MidpointRounding.AwayFromZero) != Math.Round(lobjPersonAccount.idecMSSMonthlyPremiumAmount, 2, MidpointRounding.AwayFromZero))
                                lobjPayeeAccount.CreatePayeeAccountPaymentItemType(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(),
                                                                                lobjPersonAccount.idecMSSMonthlyPremiumAmount,
                                                                                string.Empty,
                                                                                lobjPersonAccount.icdoPersonAccount.provider_org_id,
                                                                                lobjPayeeAccount.idtNextBenefitPaymentDate,
                                                                                DateTime.MinValue,
                                                                                false, false, 0, ablnIsFromIBS: true);
                    }
                }

                //Case Payee Account to Payee Account - if different payee account selected.
                //End PAPIT if different payee account selected.

                if (iclbPA != null && iclbPA.Count() > 0)
                {
                    foreach (cdoPayeeAccount lobjPA in iclbPA)
                    {
                        busPayeeAccount lobjPayeeAcct = new busPayeeAccount();
                        lobjPayeeAcct.FindPayeeAccount(lobjPA.payee_account_id);

                        lobjPayeeAcct.LoadNexBenefitPaymentDate();

                        if ((ldtIBSInsuranceItems != null) && (ldtIBSInsuranceItems.Rows.Count > 0))
                        {
                            if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                                lobjPAPIT = lobjPayeeAcct.GetLatestPayeeAccountPaymentItemTypeMedicare(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(), lobjPayeeAcct.idtNextBenefitPaymentDate, lobjPersonAccount.icdoPersonAccount.person_account_id);
                            else
                                lobjPAPIT = lobjPayeeAcct.GetLatestPayeeAccountPaymentItemType(ldtIBSInsuranceItems.Rows[0]["data2"].ToString());
                            if (lobjPAPIT != null)
                            {
                                lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date = busGlobalFunctions.GetFirstDayofNextMonth(DateTime.Now).AddDays(-1);
                                lobjPAPIT.icdoPayeeAccountPaymentItemType.Update();
                            }
                        }
                    }
                }

            }
            //End ACH if Any
            EndExistingACHDetailsBenefitPayment();
        }

        public void DisplayPaymentElectionMethod()
        {
            if (istrPaymentElectionHealthMedicare == busConstant.AutomaticPremiumDeduction)
                istrPaymentElectionHealthMedicareDisplay = busGlobalFunctions.GetDescriptionByCodeValue(7012, busConstant.AutomaticPremiumDeduction, iobjPassInfo);
            else
                istrPaymentElectionHealthMedicareDisplay = busGlobalFunctions.GetDescriptionByCodeValue(7012, busConstant.AutomaticWithholdingFromBenefitPayment, iobjPassInfo);

            if (istrPaymentElectionDental == busConstant.AutomaticPremiumDeduction)
                istrPaymentElectionDentalDisplay = busGlobalFunctions.GetDescriptionByCodeValue(7012, busConstant.AutomaticPremiumDeduction, iobjPassInfo);
            else
                istrPaymentElectionDentalDisplay = busGlobalFunctions.GetDescriptionByCodeValue(7012, busConstant.AutomaticWithholdingFromBenefitPayment, iobjPassInfo);

            if (istrPaymentElectionVision == busConstant.AutomaticPremiumDeduction)
                istrPaymentElectionVisionDisplay = busGlobalFunctions.GetDescriptionByCodeValue(7012, busConstant.AutomaticPremiumDeduction, iobjPassInfo);
            else
                istrPaymentElectionVisionDisplay = busGlobalFunctions.GetDescriptionByCodeValue(7012, busConstant.AutomaticWithholdingFromBenefitPayment, iobjPassInfo);

            if (istrPaymentElectionLife == busConstant.AutomaticPremiumDeduction)
                istrPaymentElectionLifeDisplay = busGlobalFunctions.GetDescriptionByCodeValue(7012, busConstant.AutomaticPremiumDeduction, iobjPassInfo);
            else
                istrPaymentElectionLifeDisplay = busGlobalFunctions.GetDescriptionByCodeValue(7012, busConstant.AutomaticWithholdingFromBenefitPayment, iobjPassInfo);
        }

        public void DisplayEffectiveDateOnPrintScreen()
        {
            if (istrPaymentElectionHealthMedicare == busConstant.AutomaticPremiumDeduction)
                idtEffectiveDateHealthMedicare = idtACHEffectiveDate;
            else
                idtEffectiveDateHealthMedicare = ibusPayeeAccount.idtNextBenefitPaymentDate;

            if (istrPaymentElectionDental == busConstant.AutomaticPremiumDeduction)
                idtEffectiveDateDental = idtACHEffectiveDate;
            else
                idtEffectiveDateDental = ibusPayeeAccount.idtNextBenefitPaymentDate;

            if (istrPaymentElectionVision == busConstant.AutomaticPremiumDeduction)
                idtEffectiveDateVision = idtACHEffectiveDate;
            else
                idtEffectiveDateVision = ibusPayeeAccount.idtNextBenefitPaymentDate;

            if (istrPaymentElectionLife == busConstant.AutomaticPremiumDeduction)
                idtEffectiveDateLife = idtACHEffectiveDate;
            else
                idtEffectiveDateLife = ibusPayeeAccount.idtNextBenefitPaymentDate;
        }

        public void DisplaySelectedRetirementPaymentAccount()
        {
            if (iclbRetirementPayeeAccount == null)
                LoadRetirementPayeeAccount();

            var lclbPayeeAccount = iclbRetirementPayeeAccount.Where(o => o.payee_account_id == iintPayeeAccountACHID);

            foreach (cdoPayeeAccount lobjPA in lclbPayeeAccount)
            {
                istrBenefitAccountType = lobjPA.benefit_account_type_description;
                istrPlanName = lobjPA.istrPlanName;
                istrBenefitOption = lobjPA.benefit_option_description;
                istrBenefitAmount = "$" + lobjPA.istrBenefitAmount;
            }
        }

        public busOrganization GetBankNameForWithdrawal(string AintRoutingNumber)
        {
            busOrganization lobjOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
            if (AintRoutingNumber != null)
            {
                lobjOrg.FindOrganizationByRoutingNumber(AintRoutingNumber);
                if (lobjOrg.icdoOrganization.org_id > 0)
                {
                    lobjOrg.icdoOrganization.iblnIsRoutingNumberExists = true;
                }
            }
            return lobjOrg;
        }

        public void LoadACHDetailsAcknowledgement()
        {
            busBase lbusbase = new busBase();
            iclbACHDetailsAcknowledgement = new Collection<busWssAcknowledgement>();
            DataTable ldtbList = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.PersonAccountACHDetailAuth });
            iclbACHDetailsAcknowledgement = lbusbase.GetCollection<busWssAcknowledgement>(ldtbList);
        }

        public bool IsAcknowledgementNotSelected() => istrAcknowledgementFlag != busConstant.Flag_Yes;

        //public bool IsAcknowledgementNotSelected()
        //{
        //    if (istrAcknowledgementFlag == busConstant.Flag_No)
        //        return true;
        //    return false;
        //}

        public bool IsPaymentOptionSelectedBEPT()
        {
            if (istrPaymentElectionHealthMedicare == busConstant.AutomaticWithholdingFromBenefitPayment ||
                istrPaymentElectionDental == busConstant.AutomaticWithholdingFromBenefitPayment ||
                istrPaymentElectionVision == busConstant.AutomaticWithholdingFromBenefitPayment ||
                istrPaymentElectionLife == busConstant.AutomaticWithholdingFromBenefitPayment)
                return true;
            return false;
        }

        //Rule to check if Payee Account status is in Approved or Receiving status.(Step 1)
        public bool IsPayeeAccountApprovedOrReceiving()
        {
            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();

            DataTable ldtbList = Select("cdoPayeeAccount.LoadRetirementPayeeAccount", new object[2] { ibusPerson.icdoPerson.person_id, ibusPayeeAccount.idtNextBenefitPaymentDate });

            if (ldtbList.Rows.Count > 0)
                return true;

            return false;
        }

        public void LoadPayeeAccount()
        {
            if (ibusPayeeAccount == null)
            {
                ibusPayeeAccount = new busPayeeAccount();
            }
            ibusPayeeAccount.FindPayeeAccountByPersonID(ibusPerson.icdoPerson.person_id);
        }

        public void InitializeObjects()
        {
            ibusPersonAccountACHDetail = new busPersonAccountAchDetail();
            ibusPersonAccountACHDetail.icdoPersonAccountAchDetail = new cdoPersonAccountAchDetail();

            ibusPersonAccountPaymentElection = new busPersonAccountPaymentElection();
            ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection();
        }

        public void LoadBankOrgByRoutingNumber(string istrRoutingNo)
        {
            string istrRoutingNumber;
            if (ibusBankOrg == null)
            {
                ibusBankOrg = new busOrganization();
            }

            //Left padding '0' to the routing number as aba_number in SGT_PERSON_ACCOUNTACH_DETAIL is int and routing number in SGT_ORGANIZATION is a string.
            if (istrRoutingNo.ToString().Length != 9)
                istrRoutingNumber = istrRoutingNo.ToString().PadLeft(9, '0');
            else
                istrRoutingNumber = istrRoutingNo.ToString();

            ibusBankOrg.FindOrganizationByRoutingNumber(istrRoutingNumber);
        }

        //Method to insert new bank details if routing number does not match.
        public void InsertOrgBankForRoutingNumber(string astrBankName, int astrRoutingNumber)
        {
            busOrganization lobjOrganization = new busOrganization();
            lobjOrganization.icdoOrganization = new cdoOrganization();

            lobjOrganization.icdoOrganization.org_name = astrBankName;
            lobjOrganization.icdoOrganization.routing_no = astrRoutingNumber.ToString();
            lobjOrganization.icdoOrganization.org_type_value = busConstant.OrganizationTypeBank;
            lobjOrganization.icdoOrganization.status_value = busConstant.OrganizationStatusPending;

            lobjOrganization.icdoOrganization.org_code = lobjOrganization.GetNewOrgCodeRangeID();
            DBFunction.DBNonQuery("cdoOrgCodeByType.UpdateLastEnteredOrgCodeID", new object[2] {
                                            (Convert.ToInt32(lobjOrganization.icdoOrganization.org_code)- 1), Convert.ToInt32(lobjOrganization.icdoOrganization.org_code) },
                                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (lobjOrganization.icdoOrganization.org_code.Length < 6)
                lobjOrganization.icdoOrganization.org_code = lobjOrganization.icdoOrganization.org_code.PadLeft(6, '0');

            lobjOrganization.icdoOrganization.Insert();
        }

        //Initialize WFl for new bank.
        public void InitializeWorkflow(int aintOrgID, int aintProcessID)
        {
            if (!busWorkflowHelper.IsActiveInstanceAvailableForOrg(busConstant.Map_Process_Create_And_Maintain_Organization_Information, aintOrgID))
            {
                busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Process_Create_And_Maintain_Organization_Information, 0, aintOrgID, 0, iobjPassInfo);
            }
        }

        //Method to load previous ACH details.
        public void LoadAllACHDetailsWithEndDateNullMSS()
        {
            if (iclbACHDetailsWithEndDateNull == null)
                iclbACHDetailsWithEndDateNull = new Collection<busPersonAccountAchDetail>();

            DataTable ldtbList = Select("cdoPersonAccountAchDetail.LoadExistingACHDetails", new object[1] { iintPersonID });
            iclbACHDetailsWithEndDateNull = GetCollection<busPersonAccountAchDetail>(ldtbList, "icdoPersonAccountAchDetail");

        }

        public Collection<cdoPayeeAccount> LoadRetirementPayeeAccount()
        {
            if (iclbRetirementPayeeAccount == null)
                iclbRetirementPayeeAccount = new Collection<cdoPayeeAccount>();

            if (ibusPayeeAccount == null)
                LoadPayeeAccount();
            if (ibusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                ibusPayeeAccount.LoadNexBenefitPaymentDate();

            Collection<cdoPayeeAccount> iclbPayeeAccount = new Collection<cdoPayeeAccount>();
            DataTable ldtbList = Select("cdoPayeeAccount.LoadRetirementPayeeAccount", new object[2] { ibusPerson.icdoPerson.person_id, ibusPayeeAccount.idtNextBenefitPaymentDate });
            iclbPayeeAccount = Sagitec.DataObjects.doBase.GetCollection<cdoPayeeAccount>(ldtbList);

            //iclbRetirementPayeeAccount count == 0 checked because when previous is clicked, again payee accounts are added in iclbRetirementPayeeAccount which is already filled.
            if (iclbRetirementPayeeAccount.Count == 0)
            {
                foreach (cdoPayeeAccount lobjPA in iclbPayeeAccount)
                {
                    busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                    lobjPayeeAccount.FindPayeeAccount(lobjPA.payee_account_id);

                    if (lobjPayeeAccount.idecBenefitAmount == 0.00M)
                        lobjPayeeAccount.LoadBenefitAmount();

                    lobjPA.istrDisplayPayeeAccounts = lobjPA.benefit_account_type_description + ", " + lobjPA.istrPlanName + ", " + lobjPA.benefit_option_description + ",  $" + lobjPayeeAccount.idecBenefitAmount;
                    lobjPA.istrBenefitAmount = lobjPayeeAccount.idecBenefitAmount.ToString();
                    iclbRetirementPayeeAccount.Add(lobjPA);
                }
            }

            return iclbRetirementPayeeAccount;
        }

        //Rule to check if Premium deduction results into Negative amount.
        public bool IsPremiumDeductionNegative()
        {
            if (iclbPlansWithholdingFromBenefitPayment.IsNull())
                LoadPremiumDeductionAsPerPlan();

            decimal idecTotalPremiumAmountsOfPlans = 0.00M;

            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            lobjPayeeAccount.FindPayeeAccount(iintPayeeAccountACHID);

            if (lobjPayeeAccount.idecBenefitAmount == 0.00M)
                lobjPayeeAccount.LoadBenefitAmount();

            foreach (busPersonAccount lobjPersonAccount in iclbPlansWithholdingFromBenefitPayment)
            {
                if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                    idecTotalPremiumAmountsOfPlans += lobjPersonAccount.idecMSSMonthlyPremiumAmount;
                if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                    idecTotalPremiumAmountsOfPlans += lobjPersonAccount.idecMSSMonthlyPremiumAmount;
                if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental)
                    idecTotalPremiumAmountsOfPlans += lobjPersonAccount.idecMSSMonthlyPremiumAmount;
                if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                    idecTotalPremiumAmountsOfPlans += lobjPersonAccount.idecMSSMonthlyPremiumAmount;
                if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                    idecTotalPremiumAmountsOfPlans += lobjPersonAccount.idecMSSMonthlyPremiumAmount;
            }

            if (lobjPayeeAccount.idecBenefitAmount - idecTotalPremiumAmountsOfPlans < 0)
                return true;

            return false;
        }

        public void LoadPaymentElectionPerPlan(int AintPersonAccountID)
        {
            ibusPersonAccountPaymentElection = new busPersonAccountPaymentElection();
            ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection();

            DataTable ldtbPaymentElection = Select<cdoPersonAccountPaymentElection>(new string[1] { "person_account_id" },
                                            new object[1] { AintPersonAccountID }, null, null);
            if (ldtbPaymentElection.Rows.Count > 0)
                ibusPersonAccountPaymentElection.icdoPersonAccountPaymentElection.LoadData(ldtbPaymentElection.Rows[0]);
        }

        public void InsertHistoryForMedicare(busPersonAccount lobjPersonAccount)
        {
            busPersonAccountMedicarePartDHistory ibusPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory() { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
            ibusPersonAccountMedicarePartDHistory.FindMedicareByPersonAccountID(lobjPersonAccount.icdoPersonAccount.person_account_id);
            ibusPersonAccountMedicarePartDHistory.LoadPreviousHistory();

            //End previous history record
            if (ibusPersonAccountMedicarePartDHistory.ibusHistory.icdoPersonAccountMedicarePartDHistory.person_account_medicare_part_d_history_id > 0)
            {
                if (ibusPersonAccountMedicarePartDHistory.ibusHistory.icdoPersonAccountMedicarePartDHistory.start_date == lobjPersonAccount.icdoPersonAccount.history_change_date)
                    ibusPersonAccountMedicarePartDHistory.ibusHistory.icdoPersonAccountMedicarePartDHistory.end_date = lobjPersonAccount.icdoPersonAccount.history_change_date;

                else
                    ibusPersonAccountMedicarePartDHistory.ibusHistory.icdoPersonAccountMedicarePartDHistory.end_date = lobjPersonAccount.icdoPersonAccount.history_change_date.AddDays(-1);

                ibusPersonAccountMedicarePartDHistory.ibusHistory.icdoPersonAccountMedicarePartDHistory.Update();
            }

            cdoPersonAccountMedicarePartDHistory lobjCdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
            lobjCdoPersonAccountMedicarePartDHistory.person_account_id = lobjPersonAccount.icdoPersonAccount.person_account_id;
            lobjCdoPersonAccountMedicarePartDHistory.person_id = lobjPersonAccount.icdoPersonAccount.person_id;
            lobjCdoPersonAccountMedicarePartDHistory.start_date = lobjPersonAccount.icdoPersonAccount.history_change_date;
            lobjCdoPersonAccountMedicarePartDHistory.plan_participation_status_value = lobjPersonAccount.icdoPersonAccount.plan_participation_status_value;
            lobjCdoPersonAccountMedicarePartDHistory.status_value = lobjPersonAccount.icdoPersonAccount.status_value;

            lobjCdoPersonAccountMedicarePartDHistory.reason_value = busConstant.ChangeReasonIBSPaymentMethod;
            lobjCdoPersonAccountMedicarePartDHistory.suppress_warnings_flag = ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.suppress_warnings_flag;
            lobjCdoPersonAccountMedicarePartDHistory.medicare_claim_no = ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.medicare_claim_no;
            lobjCdoPersonAccountMedicarePartDHistory.medicare_part_a_effective_date = ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.medicare_part_a_effective_date;
            lobjCdoPersonAccountMedicarePartDHistory.medicare_part_b_effective_date = ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.medicare_part_b_effective_date;
            lobjCdoPersonAccountMedicarePartDHistory.low_income_credit = ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.low_income_credit;
            lobjCdoPersonAccountMedicarePartDHistory.late_enrollment_penalty = ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty;
            lobjCdoPersonAccountMedicarePartDHistory.member_person_id = ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.member_person_id;
            ibusPersonAccountMedicarePartDHistory.UpdateEnrollmentAndRecordTypeFlags();
            lobjCdoPersonAccountMedicarePartDHistory.record_type_flag = ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.record_type_flag;
            lobjCdoPersonAccountMedicarePartDHistory.enrollment_file_sent_flag = ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.enrollment_file_sent_flag;
            lobjCdoPersonAccountMedicarePartDHistory.send_after = DateTime.Now.Date;

            if (ibusPersonAccountMedicarePartDHistory.iclbPersonAccountMedicarePartDHistory == null)
                ibusPersonAccountMedicarePartDHistory.LoadPersonAccountMedicarePartDHistory(lobjPersonAccount.icdoPersonAccount.person_id);

            if (ibusPersonAccountMedicarePartDHistory.iclbPersonAccountMedicarePartDHistory.Where(i => i.icdoPersonAccountMedicarePartDHistory.start_date != i.icdoPersonAccountMedicarePartDHistory.end_date
                && i.icdoPersonAccountMedicarePartDHistory.start_date > i.icdoPersonAccountMedicarePartDHistory.end_date &&
                i.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Count() == 0)
                lobjCdoPersonAccountMedicarePartDHistory.initial_enroll_date = ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.start_date;
            else
                lobjCdoPersonAccountMedicarePartDHistory.initial_enroll_date = ibusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.initial_enroll_date;

            lobjCdoPersonAccountMedicarePartDHistory.provider_org_id = lobjPersonAccount.icdoPersonAccount.provider_org_id;

            lobjCdoPersonAccountMedicarePartDHistory.Insert();
        }

        public void InsertHistoryForLife(busPersonAccount lobjPersonAccount)
        {
            busPersonAccountLife ibusPersonAccountLife = new busPersonAccountLife() { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountLife = new cdoPersonAccountLife() };
            ibusPersonAccountLife.FindPersonAccountLife(lobjPersonAccount.icdoPersonAccount.person_account_id);
            ibusPersonAccountLife.FindPersonAccount(lobjPersonAccount.icdoPersonAccount.person_account_id);
            ibusPersonAccountLife.LoadPersonAccountLifeOptions();
            ibusPersonAccountLife.LoadPreviousHistory();

            ibusPersonAccountLife.icdoPersonAccountLife.reason_value = busConstant.ChangeReasonIBSPaymentMethod;
            ibusPersonAccountLife.icdoPersonAccountLife.Update();

            if (ibusPersonAccountLife.iclbPersonAccountLifeOption.IsNotNull())
            {
                foreach (busPersonAccountLifeOption lobjLifeOption in ibusPersonAccountLife.iclbPersonAccountLifeOption)
                {
                    if (lobjLifeOption.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue)
                    {
                        //End previous history record
                        busPersonAccountLifeHistory lobjHistory = ibusPersonAccountLife.GetPreviousHistoryForOption(lobjLifeOption);
                        if (lobjHistory.icdoPersonAccountLifeHistory.person_account_life_history_id > 0)
                        {
                            if (lobjHistory.icdoPersonAccountLifeHistory.effective_start_date == lobjPersonAccount.icdoPersonAccount.history_change_date)
                            {
                                lobjHistory.icdoPersonAccountLifeHistory.effective_end_date = lobjPersonAccount.icdoPersonAccount.history_change_date;
                                lobjHistory.icdoPersonAccountLifeHistory.Update();
                            }
                            else if (lobjHistory.icdoPersonAccountLifeHistory.effective_end_date == DateTime.MinValue
                                || (lobjHistory.icdoPersonAccountLifeHistory.effective_end_date > lobjPersonAccount.icdoPersonAccount.history_change_date.AddDays(-1)
                                && lobjHistory.icdoPersonAccountLifeHistory.effective_start_date <= lobjPersonAccount.icdoPersonAccount.history_change_date.AddDays(-1)))
                            {
                                lobjHistory.icdoPersonAccountLifeHistory.effective_end_date = lobjPersonAccount.icdoPersonAccount.history_change_date.AddDays(-1);
                                lobjHistory.icdoPersonAccountLifeHistory.Update();
                            }
                        }


                        cdoPersonAccountLifeHistory lobjLifeHistory = new cdoPersonAccountLifeHistory();
                        lobjLifeHistory.person_account_id = lobjPersonAccount.icdoPersonAccount.person_account_id;
                        lobjLifeHistory.start_date = lobjPersonAccount.icdoPersonAccount.history_change_date;

                        lobjLifeHistory.plan_participation_status_value = lobjPersonAccount.icdoPersonAccount.plan_participation_status_value;
                        lobjLifeHistory.status_id = lobjPersonAccount.icdoPersonAccount.status_id;
                        lobjLifeHistory.status_value = lobjPersonAccount.icdoPersonAccount.status_value;
                        lobjLifeHistory.from_person_account_id = lobjPersonAccount.icdoPersonAccount.from_person_account_id;
                        lobjLifeHistory.to_person_account_id = lobjPersonAccount.icdoPersonAccount.to_person_account_id;
                        lobjLifeHistory.suppress_warnings_by = lobjPersonAccount.icdoPersonAccount.suppress_warnings_by;
                        lobjLifeHistory.suppress_warnings_date = lobjPersonAccount.icdoPersonAccount.suppress_warnings_date;
                        lobjLifeHistory.suppress_warnings_flag = lobjPersonAccount.icdoPersonAccount.suppress_warnings_flag;
                        lobjLifeHistory.life_insurance_type_id = ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_id;
                        lobjLifeHistory.life_insurance_type_value = ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value;
                        lobjLifeHistory.premium_waiver_flag = ibusPersonAccountLife.icdoPersonAccountLife.premium_waiver_flag;
                        lobjLifeHistory.projected_premium_waiver_date = ibusPersonAccountLife.icdoPersonAccountLife.projected_premium_waiver_date;
                        lobjLifeHistory.actual_premium_waiver_date = ibusPersonAccountLife.icdoPersonAccountLife.actual_premium_waiver_date;
                        lobjLifeHistory.premium_waiver_provider_org_id = ibusPersonAccountLife.icdoPersonAccountLife.premium_waiver_provider_org_id;
                        lobjLifeHistory.waived_amount = ibusPersonAccountLife.icdoPersonAccountLife.waived_amount;
                        lobjLifeHistory.spouse_waived_amount = ibusPersonAccountLife.icdoPersonAccountLife.spouse_waived_amount;
                        lobjLifeHistory.dependent_waived_amount = ibusPersonAccountLife.icdoPersonAccountLife.dependent_waived_amount;
                        lobjLifeHistory.level_of_coverage_id = lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_id;
                        lobjLifeHistory.level_of_coverage_value = lobjLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value;

                        if (lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount == 0.0M ||
                            lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueWaived)
                            lobjLifeHistory.ps_file_change_event_value = busConstant.AnnualEnrollmentWaived;
                        else
                            lobjLifeHistory.ps_file_change_event_value = busConstant.AnnualEnrollment;

                        //lobjLifeHistory.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081

                        lobjLifeHistory.effective_start_date = lobjPersonAccount.icdoPersonAccount.history_change_date;


                        lobjLifeHistory.plan_option_status_value = lobjLifeOption.icdoPersonAccountLifeOption.plan_option_status_value;
                        lobjLifeHistory.coverage_amount = lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                        lobjLifeHistory.provider_name = ibusPersonAccountLife.icdoPersonAccountLife.Provider_Name;
                        lobjLifeHistory.disability_letter_sent_flag = ibusPersonAccountLife.icdoPersonAccountLife.disability_letter_sent_flag;
                        lobjLifeHistory.provider_org_id = lobjPersonAccount.icdoPersonAccount.provider_org_id;
                        lobjLifeHistory.reason_id = ibusPersonAccountLife.icdoPersonAccountLife.reason_id;
                        lobjLifeHistory.reason_value = ibusPersonAccountLife.icdoPersonAccountLife.reason_value;
                        lobjLifeHistory.premium_conversion_indicator_flag = ibusPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag;
                        lobjLifeHistory.Insert();

                    }
                }
            }
        }

        //PIR 24923- is record exists with same start date
        public bool IsACHDetailRecordExistsWithSameStartDate()
        {
            bool lblnIsExists = false;
            if (!isACHDetailsInsert)
            {
                string listrAllPlansPersonAccountId = string.Empty;                
                if (istrPaymentElectionHealthMedicare == busConstant.AutomaticPremiumDeduction)
                    listrAllPlansPersonAccountId = istrHealthandMedicarePersonAccountIds + ",";
                if (istrPaymentElectionDental == busConstant.AutomaticPremiumDeduction)
                    listrAllPlansPersonAccountId += istrDentalPersonAccountId + ",";
                if (istrPaymentElectionVision == busConstant.AutomaticPremiumDeduction)
                    listrAllPlansPersonAccountId += istrVisionPersonAccountId + ",";
                if (istrPaymentElectionLife == busConstant.AutomaticPremiumDeduction)
                    listrAllPlansPersonAccountId += istrLifePersonAccountId + ",";
                string[] larPersonAccountIds = listrAllPlansPersonAccountId.TrimEnd(',').Split(',');
                foreach (string lstrPersonAccountId in larPersonAccountIds)
                {
                    if (lstrPersonAccountId.IsNotNullOrEmpty())
                    {
                        DataTable ldtbList = Select<cdoPersonAccountAchDetail>(
                                                  new string[1] { "person_account_id" },
                                                  new object[1] { lstrPersonAccountId }, null, null);
                        Collection<busPersonAccountAchDetail> lclbPersonAccountAchDetail = GetCollection<busPersonAccountAchDetail>(ldtbList, "icdoPersonAccountAchDetail");

                        if (lclbPersonAccountAchDetail.IsNotNull() && lclbPersonAccountAchDetail.Count > 0 &&
                           lclbPersonAccountAchDetail.Where(i => i.icdoPersonAccountAchDetail.ach_start_date.Date == idtACHEffectiveDate.Date).Count() > 0)
                        {
                            lblnIsExists = true;
                            break;
                        }
                    }
                }
            }
            return lblnIsExists;

        }
    }
}
