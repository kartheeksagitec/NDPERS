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

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitCalculatorWeb : busExtendBase
    {
        public cdoWssBenefitCalculator icdoWssBenefitcalculator { get; set; }
        public string istrWizardStepName { get; set; }

        //public int iintPlanId { get; set; }
        public busPerson ibusMember { get; set; }
        public busPlan ibusPlan { get; set; }
        public busRetirementBenefitCalculation ibusBenefitCalculation { get; set; }
        public busPreRetirementDeathBenefitCalculation ibusPreRetirementDeathCalculation { get; set; }
        public busServicePurchaseHeader ibusSickLeavePurchase { get; set; }
        public busServicePurchaseHeader ibusConsolidatedPurchase { get; set; }
        public busPerson ibusPerson { get; set; }
        public Collection<busConsolidatedPurchaseWeb> iclbConsolidatedPurchase { get; set; }
        public Collection<busPersonTffrTiaaService> iclbTffrTiaaService { get; set; }
        public DateTime ldtSpouseDateOfBirth { get; set; }

        public int iintRetirementMonth { get; set; }
        public int istrRetirementYear { get; set; }

        public string istrBenefitAccountTypeValue { get; set; }

        //enhancement 6878
        public string istrConfirmationText
        {
            get
            {
                string luserName = ibusMember.icdoPerson.FullName;
                DateTime Now = DateTime.Now;
                string lstrConfimation = string.Format(busGlobalFunctions.GetMessageTextByMessageID(8566, iobjPassInfo), luserName, Now);
                return lstrConfimation;
            }

        }

        //calculation        
        public bool iblnIsPersonEligibleForEarly { get; set; }

        //to check if this is disability 
        //public string istrIsDisability { get; set; }

        // Service Credit Change
        //public string istrUnusedSickLeave { get; set; }
        //public string istrAdditionalServiceMonths { get; set; }
        //public string istrTFFRTIAAServiceCredit { get; set; }
        //public decimal idecTFFRTIAAServiceCredit { get; set; }

        //consolidate additional service credits
        //public int iintAdditionalServiceCredits { get; set; }
        //Service Purchase - Payment Election
        //public decimal idecDownPaymentAmount { get; set; }
        //public decimal idecPaymentAmount { get; set; }
        //public int iintNumberofPayments { get; set; }     

        //deduction
        public busDeductionCalculation ibusDeductionCalculationWeb { get; set; }

        public void LoadMember(int aintPersonID)
        {
            if (ibusMember.IsNull())
                ibusMember = new busPerson();
            ibusMember.FindPerson(aintPersonID);
        }
        public void LoadTffrTiaaService(int aintPersonID)
        {
            DataTable ldtbList = Select<cdoPersonTffrTiaaService>(
                new string[1] { "person_id" },
                new object[1] { aintPersonID }, null, null);
            iclbTffrTiaaService = GetCollection<busPersonTffrTiaaService>(ldtbList, "icdoPersonTffrTiaaService");
        }
        public void LoadPlan()
        {
            if (ibusPlan.IsNull())
                ibusPlan = new busPlan();
            ibusPlan.FindPlan(icdoWssBenefitcalculator.plan_id);
        }

        // load plans for which the member can estimate
        public Collection<cdoPlan> iclbEligiblePlan { get; set; }
        public Collection<cdoPlan> LoadPlansForBenefitCalculation()
        {
            LoadEligiblePlans();
            return iclbEligiblePlan;
        }

        public void LoadEligiblePlans()
        {
            iclbEligiblePlan = new Collection<cdoPlan>();

            if (ibusMember.icolPersonAccountByBenefitType.IsNull())
                ibusMember.LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeRetirement);

            var lenumPlanList = ibusMember.icolPersonAccountByBenefitType.Where(lobjPA => lobjPA.icdoPersonAccount.person_account_id > 0
                && (lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled
                 || lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended));

            foreach (busPersonAccount lobjPlan in lenumPlanList)
            {
                lobjPlan.LoadPlan();
                iclbEligiblePlan.Add(lobjPlan.ibusPlan.icdoPlan);
            }
        }

        public void LoadConsolidatePurchaseInNewMode()
        {
            if (iclbConsolidatedPurchase.IsNull())
                iclbConsolidatedPurchase = new Collection<busConsolidatedPurchaseWeb>();

            busConsolidatedPurchaseWeb lobjConsolidatePurchase1 = new busConsolidatedPurchaseWeb();
            lobjConsolidatePurchase1.istrConsolidatePurchaseTypeValue = busConstant.Service_Purchase_Type_Additional_Service_Credit;
            lobjConsolidatePurchase1.istrConsolidatePurchaseType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, lobjConsolidatePurchase1.istrConsolidatePurchaseTypeValue);
            iclbConsolidatedPurchase.Add(lobjConsolidatePurchase1);

            busConsolidatedPurchaseWeb lobjConsolidatePurchase2 = new busConsolidatedPurchaseWeb();
            lobjConsolidatePurchase2.istrConsolidatePurchaseTypeValue = busConstant.Service_Purchase_Type_Leave_Of_Absence;
            lobjConsolidatePurchase2.istrConsolidatePurchaseType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, lobjConsolidatePurchase2.istrConsolidatePurchaseTypeValue);
            iclbConsolidatedPurchase.Add(lobjConsolidatePurchase2);

            busConsolidatedPurchaseWeb lobjConsolidatePurchase3 = new busConsolidatedPurchaseWeb();
            lobjConsolidatePurchase3.istrConsolidatePurchaseTypeValue = busConstant.Service_Purchase_Type_Military_Service;
            lobjConsolidatePurchase3.istrConsolidatePurchaseType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, lobjConsolidatePurchase3.istrConsolidatePurchaseTypeValue);
            iclbConsolidatedPurchase.Add(lobjConsolidatePurchase3);

            busConsolidatedPurchaseWeb lobjConsolidatePurchase4 = new busConsolidatedPurchaseWeb();
            lobjConsolidatePurchase4.istrConsolidatePurchaseTypeValue = busConstant.Service_Purchase_Type_Previous_Public_Employment;
            lobjConsolidatePurchase4.istrConsolidatePurchaseType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, lobjConsolidatePurchase4.istrConsolidatePurchaseTypeValue);
            iclbConsolidatedPurchase.Add(lobjConsolidatePurchase4);

            busConsolidatedPurchaseWeb lobjConsolidatePurchase5 = new busConsolidatedPurchaseWeb();
            lobjConsolidatePurchase5.istrConsolidatePurchaseTypeValue = busConstant.Service_Purchase_Type_Previous_Pers_Employment;
            lobjConsolidatePurchase5.istrConsolidatePurchaseType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, lobjConsolidatePurchase5.istrConsolidatePurchaseTypeValue);
            iclbConsolidatedPurchase.Add(lobjConsolidatePurchase5);
        }

        public void SetITDeductionDescription()
        {
            if (ibusDeductionCalculationWeb.IsNotNull())
            {
                if (ibusDeductionCalculationWeb.ibusBenefitPayeeFedTaxWithholding.IsNotNull())
                {
                    if (ibusDeductionCalculationWeb.ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value != string.Empty)
                    {
                        ibusDeductionCalculationWeb.ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_description = busGlobalFunctions.GetDescriptionByCodeValue(
                            2218, ibusDeductionCalculationWeb.ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value, iobjPassInfo);
                    }
                    if (ibusDeductionCalculationWeb.ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_value != string.Empty)
                    {
                        ibusDeductionCalculationWeb.ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_description = busGlobalFunctions.GetDescriptionByCodeValue(
                            306, ibusDeductionCalculationWeb.ibusBenefitPayeeFedTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_value, iobjPassInfo);
                    }
                }
                if (ibusDeductionCalculationWeb.ibusBenefitPayeeStateTaxWithholding.IsNotNull())
                {
                    if (ibusDeductionCalculationWeb.ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value != string.Empty)
                    {
                        ibusDeductionCalculationWeb.ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_description = busGlobalFunctions.GetDescriptionByCodeValue(
                            2218, ibusDeductionCalculationWeb.ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.tax_option_value, iobjPassInfo);
                    }
                    if (ibusDeductionCalculationWeb.ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_value != string.Empty)
                    {
                        ibusDeductionCalculationWeb.ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_description = busGlobalFunctions.GetDescriptionByCodeValue(
                            306, ibusDeductionCalculationWeb.ibusBenefitPayeeStateTaxWithholding.icdoBenefitPayeeTaxWithholding.marital_status_value, iobjPassInfo);
                    }
                }
            }
        }

        #region Method to populate DDL
        public Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> LoadCoverageCodeByFilter()
        {
            bool lblnIsMemberEnrolledInHealth = ibusMember.IsMemberEnrolledInPlan(busConstant.PlanIdGroupHealth);
            Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> lclbResult = ibusDeductionCalculationWeb.LoadCoverageCodeByFilter();
            Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> lclbFinalCoverageRef = new Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>();
            foreach (cdoOrgPlanGroupHealthMedicarePartDCoverageRef lcdoResult in lclbResult)
            {
                cdoOrgPlanGroupHealthMedicarePartDCoverageRef lcdoTemp = new cdoOrgPlanGroupHealthMedicarePartDCoverageRef();
                int lintIndex = lcdoResult.short_description.LastIndexOf('-');
                if (lintIndex > 0)
                    lcdoResult.short_description = lcdoResult.short_description.Substring(lintIndex + 2);

                if (lcdoResult.short_description != "Dual" && lcdoResult.short_description != string.Empty && lcdoResult.show_online_flag != busConstant.Flag_No)
                {
                    if ((IsHealthCoverageCodeSingle(lcdoResult.coverage_code) || IsHealthCoverageCodeFamily(lcdoResult.coverage_code)) &&
                        (!lclbFinalCoverageRef.Where(o => o.coverage_code == lcdoResult.coverage_code).Any())) // PIR 10751
                        lclbFinalCoverageRef.Add(lcdoResult);
                    if (!lblnIsMemberEnrolledInHealth)
                    {
                        if ((IsHealthCoverageCodeFamily3(lcdoResult.coverage_code)) &&
                            (!lclbFinalCoverageRef.Where(o => o.coverage_code == lcdoResult.coverage_code).Any()))
                            lclbFinalCoverageRef.Add(lcdoResult);
                    }
                }
            }
            return lclbFinalCoverageRef;
        }

        public bool IsHealthCoverageCodeSingle(string astrCoverageCode)
        {
            if (!string.IsNullOrEmpty(astrCoverageCode))
            {
                if (astrCoverageCode.Contains("0001") || astrCoverageCode.Contains("0004") || astrCoverageCode.Contains("0006") ||
                    astrCoverageCode.Contains("0021") || astrCoverageCode.Contains("0024"))
                    return true;
            }
            return false;
        }

        public bool IsHealthCoverageCodeFamily(string astrCoverageCode)
        {
            if (!string.IsNullOrEmpty(astrCoverageCode))
            {
                if (astrCoverageCode.Contains("0002") || astrCoverageCode.Contains("0005") || astrCoverageCode.Contains("0007") ||
                    astrCoverageCode.Contains("0022") || astrCoverageCode.Contains("0025"))
                    return true;
            }
            return false;
        }

        public bool IsHealthCoverageCodeFamily3(string astrCoverageCode)
        {
            if (!string.IsNullOrEmpty(astrCoverageCode))
            {
                if (astrCoverageCode.Contains("0023") || astrCoverageCode.Contains("0026"))
                    return true;
            }
            return false;
        }

        public string istrHealthCoverage { get; set; }
        public void SetHDVCoverageCodeDescription()
        {
            // Health Coverage Description
            istrHealthCoverage = string.Empty;
            if (ibusDeductionCalculationWeb.IsNotNull() && ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.IsNotNull() &&
                ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.coverage_code != string.Empty)
            {
                Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> lclbResult = new Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>();
                lclbResult = LoadCoverageCodeByFilter();
                foreach (cdoOrgPlanGroupHealthMedicarePartDCoverageRef lcdoResult in lclbResult)
                {
                    if (lcdoResult.coverage_code == ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.coverage_code)
                    {
                        istrHealthCoverage = lcdoResult.short_description; break;
                    }
                }
            }

            // Dental Coverage Description
            if (ibusDeductionCalculationWeb.IsNotNull() && ibusDeductionCalculationWeb.ibusBenefitDentalDeduction.IsNotNull() &&
                ibusDeductionCalculationWeb.ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.level_of_coverage_value != string.Empty)
            {
                ibusDeductionCalculationWeb.ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.level_of_coverage_description = busGlobalFunctions.GetDescriptionByCodeValue(
                    408, ibusDeductionCalculationWeb.ibusBenefitDentalDeduction.icdoBenefitGhdvDeduction.level_of_coverage_value, iobjPassInfo);
            }

            // Vision Coverage Description
            if (ibusDeductionCalculationWeb.IsNotNull() && ibusDeductionCalculationWeb.ibusBenefitVisionDeduction.IsNotNull() &&
                ibusDeductionCalculationWeb.ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.level_of_coverage_value != string.Empty)
            {
                ibusDeductionCalculationWeb.ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.level_of_coverage_description = busGlobalFunctions.GetDescriptionByCodeValue(
                    408, ibusDeductionCalculationWeb.ibusBenefitVisionDeduction.icdoBenefitGhdvDeduction.level_of_coverage_value, iobjPassInfo);
            }
        }

        public Collection<cdoCodeValue> LoadLevelOfCoverageDental()
        {
            return ibusDeductionCalculationWeb.LoadLevelOfCoverageDental();
        }

        public Collection<cdoCodeValue> LoadLevelOfCoverageVision()
        {
            return ibusDeductionCalculationWeb.LoadLevelOfCoverageVision();
        }

        public Collection<cdoCodeValue> LoadHealthInsuranceTypeByPlan()
        {
            return ibusDeductionCalculationWeb.LoadHealthInsuranceTypeByPlan();
        }

        public Collection<cdoCodeValue> LoadTaxOptionForFedTax()
        {
            return ibusDeductionCalculationWeb.LoadTaxOptionForFedTax();
        }

        public Collection<cdoCodeValue> LoadTaxOptionForStateTax()
        {
            return ibusDeductionCalculationWeb.LoadTaxOptionForStateTax();
        }
        #endregion

        #region Validations
        public bool VisibleRuleForQDRO()
        {
            if (ibusMember.iclbDROApplication == null)
                ibusMember.LoadDROApplications();

            var query = ibusMember.iclbDROApplication.Where(lobjDRO => lobjDRO.icdoBenefitDroApplication.plan_id == icdoWssBenefitcalculator.plan_id
                                                                            && (lobjDRO.icdoBenefitDroApplication.status_value == busConstant.StatusValid)
                                                                            && (lobjDRO.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified));

            if (query.Count<busBenefitDroApplication>() > 0)
            {
                return true;
            }
            return false;
        }
        public bool IsTFFRServiceEmpty()
        {
            if (iclbTffrTiaaService.Count == 0)
                return false;
            return true;
        }

        public int iintAvailablePlanCount
        {
            get
            {
                return iclbEligiblePlan.Count;
            }
        }

        # endregion
        //this is only displayed when there are more plans available
        //PLSO
        //SSLI
        //QDRO
        public ArrayList btnRefreshPlanBasedRules()
        {
            ArrayList larrlist = new ArrayList();
            LoadPlan();
            EvaluateInitialLoadRules();
            larrlist.Add(this);
            return larrlist;
        }

        public bool iblnIsRetirementDateEntered { get; set; }

        public override void BeforeWizardStepValidate(Sagitec.Common.utlPageMode aenmPageMode, string astrWizardName, string astrWizardStepName, utlWizardNavigationEventArgs we = null)
        {
            istrWizardStepName = astrWizardStepName;
            if (istrWizardStepName == "wzsStep1")
            {
                // Assign Values
                //PIR 24972 hid the dropdown and set value to "No" by default
                icdoWssBenefitcalculator.is_estimate_added_flag = busConstant.Flag_No;

                if (iintRetirementMonth > 0 && Convert.ToInt32(istrRetirementYear) > 0)
                    ibusBenefitCalculation.icdoBenefitCalculation.retirement_date = new DateTime(Convert.ToInt32(istrRetirementYear), iintRetirementMonth, 01);
                else
                    ibusBenefitCalculation.icdoBenefitCalculation.retirement_date = DateTime.MinValue; //PIR 11765
                if (istrBenefitAccountTypeValue == busConstant.ApplicationBenefitTypePreRetirementDeath)
                    ibusBenefitCalculation.icdoBenefitCalculation.date_of_death = DateTime.Today;
                //PIR-20647 As LoadPersonPlanAccounts method was using icdoBenefitCalculation.plan_id in  LoadPersonAccount() to load Person account details
                //which was not getting set 
                ibusBenefitCalculation.icdoBenefitCalculation.plan_id = icdoWssBenefitcalculator.plan_id;
                //PIR 19594
                ibusBenefitCalculation.LoadPersonPlanAccounts();
                if (ibusBenefitCalculation.icdoBenefitCalculation.is_return_to_work_member)
                {
                    busBenefitCalculation lobjPrevBenefitCal = ibusBenefitCalculation.LoadPrevRTWBenefitCalculation();
                    if (istrBenefitAccountTypeValue == busConstant.ApplicationBenefitTypeRetirement &&
                        lobjPrevBenefitCal.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                    {
                        ibusBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimateSubsequent;
                        ibusBenefitCalculation.icdoBenefitCalculation.calculation_type_description =
                            iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeEstimateSubsequent);

                        ibusBenefitCalculation.icdoBenefitCalculation.benefit_option_value =
                                                                                lobjPrevBenefitCal.icdoBenefitCalculation.benefit_option_value;
                    }
                    else
                    {
                        ibusBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeEstimate;
                        ibusBenefitCalculation.icdoBenefitCalculation.benefit_option_value = string.Empty;
                    }
                }

                SetupStep1BenefitCalculationObject();
                SetupStep3DeductionObject();
                if (icdoWssBenefitcalculator.is_estimate_added_flag != busConstant.Flag_Yes)
                    ValidateAndPersistBenefitCalculationObject();
            }
            else if (istrWizardStepName == "wzsStep2")
            {
                SetupStep2ServicePurchaseObject();
                icdoWssBenefitcalculator.payment_frequency_description = busGlobalFunctions.GetDescriptionByCodeValue(330, icdoWssBenefitcalculator.payment_frequency_value, iobjPassInfo);
                if (icdoWssBenefitcalculator.is_disability != busConstant.Flag_Yes)
                {
                    ibusBenefitCalculation.idecRemainingServiceCredit = ibusSickLeavePurchase.TotalTimeToPurchase + ibusConsolidatedPurchase.TotalTimeToPurchase;
                    GetNormalRetirementEligibilityDate();
                    if (!iblnIsRetirementDateEntered &&
                        ibusBenefitCalculation.icdoBenefitCalculation.normal_retirement_date > DateTime.Now) // PIR 9976
                    {
                        // If the NRD is less than current date and no Retirement date entered, based on the Purchases the Retirement date need to be modified.
                        ibusBenefitCalculation.icdoBenefitCalculation.retirement_date = ibusBenefitCalculation.icdoBenefitCalculation.normal_retirement_date;
                        ibusBenefitCalculation.icdoBenefitCalculation.termination_date = ibusBenefitCalculation.icdoBenefitCalculation.retirement_date.AddMonths(-1);
                    }
                }
            }
            else if (istrWizardStepName == "wzsStep3")
            {
                ValidateAndPersistBenefitCalculationObject();
                SetStep3DeductionObject();
                ibusDeductionCalculationWeb.LoadLTCDeductions();
                SetHDVCoverageCodeDescription();
                SetITDeductionDescription();
            }
            base.BeforeWizardStepValidate(aenmPageMode, astrWizardName, astrWizardStepName);
        }

        private void SetupStep1BenefitCalculationObject()
        {
            if (icdoWssBenefitcalculator.plan_id != 0)
            {
                //Reloading the Plan Object
                LoadPlan();
                ibusBenefitCalculation.icdoBenefitCalculation.plan_id = icdoWssBenefitcalculator.plan_id;
                ibusBenefitCalculation.ibusPlan = ibusPlan;
                ibusBenefitCalculation.ibusMember = ibusMember;

                if (ibusBenefitCalculation.ibusPersonAccount.IsNull())
                    ibusBenefitCalculation.LoadPersonAccount();

                if (ibusBenefitCalculation.ibusPersonAccount.ibusPlan.IsNull())
                    ibusBenefitCalculation.ibusPersonAccount.ibusPlan = ibusPlan;

                ibusBenefitCalculation.ibusJointAnnuitant = ibusMember.ibusSpouse;
                icdoWssBenefitcalculator.spouse_date_of_birth = ibusMember.ibusSpouse.icdoPerson.date_of_birth;// PIR 14652

                // PIR 9711
                if (istrBenefitAccountTypeValue == busConstant.ApplicationBenefitTypeDisability)
                {
                    icdoWssBenefitcalculator.is_disability = busConstant.Flag_Yes;
                    ibusBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeDisability;
                }
                else if (istrBenefitAccountTypeValue == busConstant.ApplicationBenefitTypePreRetirementDeath)
                {
                    icdoWssBenefitcalculator.is_pre_retirement_death_flag = busConstant.Flag_Yes;
                    ibusBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypePreRetirementDeath;
                }
                else
                {
                    ibusBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                }
                ibusBenefitCalculation.icdoBenefitCalculation.benefit_account_type_description =
                    busGlobalFunctions.GetDescriptionByCodeValue(1904, ibusBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value, iobjPassInfo);

                //get normal retirement date (this needs purchase records too)
                ibusBenefitCalculation.icdoBenefitCalculation.normal_retirement_date = GetNormalRetirementEligibilityDate();

                if (ibusBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                {
                    //if retirement date is not entered then default to NRD
                    if (ibusBenefitCalculation.icdoBenefitCalculation.retirement_date == DateTime.MinValue)
                    {
                        iblnIsRetirementDateEntered = false;
                        if (ibusBenefitCalculation.icdoBenefitCalculation.normal_retirement_date < DateTime.Now) // PIR 9447
                            ibusBenefitCalculation.icdoBenefitCalculation.retirement_date = DateTime.Now.GetFirstDayofNextMonth();
                        else
                            ibusBenefitCalculation.icdoBenefitCalculation.retirement_date = ibusBenefitCalculation.icdoBenefitCalculation.normal_retirement_date;
                    }
                    else
                        iblnIsRetirementDateEntered = true;

                    //if termination date is null, then default to retirement date.
                    if (ibusBenefitCalculation.icdoBenefitCalculation.termination_date == DateTime.MinValue)
                        if (ibusBenefitCalculation.icdoBenefitCalculation.retirement_date != DateTime.MinValue)
                            ibusBenefitCalculation.icdoBenefitCalculation.termination_date = busGlobalFunctions.GetLastDayofMonth(ibusBenefitCalculation.icdoBenefitCalculation.retirement_date).AddMonths(-1);
                }
                else
                {
                    if (ibusBenefitCalculation.icdoBenefitCalculation.retirement_date == DateTime.MinValue)
                    {
                        iblnIsRetirementDateEntered = false;
                        if (ibusBenefitCalculation.icdoBenefitCalculation.termination_date == DateTime.MinValue) //PIR 23878
                        {
                            ibusBenefitCalculation.icdoBenefitCalculation.termination_date = DateTime.Now;
                        }
                        ibusBenefitCalculation.icdoBenefitCalculation.retirement_date = DateTime.Now.GetFirstDayofNextMonth();
                    }
                    else
                        iblnIsRetirementDateEntered = true;
                }

                if (istrBenefitAccountTypeValue == busConstant.ApplicationBenefitTypePreRetirementDeath)
                {
                    ibusPreRetirementDeathCalculation.icdoBenefitCalculation = ibusBenefitCalculation.icdoBenefitCalculation;
                    ibusPreRetirementDeathCalculation.icdoBenefitCalculation.status_value = busConstant.ApplicationStatusReview;
                    ibusPreRetirementDeathCalculation.icdoBenefitCalculation.status_description =
                                                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1902, busConstant.ApplicationStatusReview);
                    ibusPreRetirementDeathCalculation.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusPendingApproval;
                    ibusPreRetirementDeathCalculation.icdoBenefitCalculation.created_date = DateTime.Now;
                    ibusPreRetirementDeathCalculation.icdoBenefitCalculation.ienuObjectState = ObjectState.Insert;
                    ibusPreRetirementDeathCalculation.iblnIsNewMode = true;
                    ibusPreRetirementDeathCalculation.ibusMember = ibusBenefitCalculation.ibusMember;
                    ibusPreRetirementDeathCalculation.ibusPersonAccount = ibusBenefitCalculation.ibusPersonAccount;
                    ibusPreRetirementDeathCalculation.ibusPlan = ibusBenefitCalculation.ibusPlan;
                    ibusPreRetirementDeathCalculation.ibusJointAnnuitant = ibusMember.ibusSpouse;
                    ibusPreRetirementDeathCalculation.CalculateMemberAge();
                    ibusPreRetirementDeathCalculation.CalculateConsolidatedPSC();
                    ibusPreRetirementDeathCalculation.CalculateConsolidatedVSC();
                }
                else
                {
                    ibusBenefitCalculation.CalculateMemberAge();
                    if (!ibusBenefitCalculation.iblnConsoldatedVSCLoaded)
                        ibusBenefitCalculation.CalculateConsolidatedVSC();
                    if (!ibusBenefitCalculation.iblnConsolidatedPSCLoaded)
                        ibusBenefitCalculation.CalculateConsolidatedPSC();
                }
            }
        }

        private void SetupStep2ServicePurchaseObject()
        {
            if (icdoWssBenefitcalculator.unused_service_purchase_selected == busConstant.Flag_Yes)
            {
                ibusSickLeavePurchase.icdoServicePurchaseHeader.person_id = ibusMember.icdoPerson.person_id;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.plan_id = icdoWssBenefitcalculator.plan_id;
                ibusSickLeavePurchase.ibusPerson = ibusMember;
                ibusSickLeavePurchase.ibusPlan = ibusPlan;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.date_of_purchase = DateTime.Now;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_Pending;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.service_purchase_status_value = busConstant.Service_Purchase_Status_Review;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.service_purchase_type_value = busConstant.Service_Purchase_Type_Unused_Sick_Leave;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.payor_value = busConstant.Service_Purchase_Payor_Employee;
                //Setting this true for system to calculate FAS
                ibusSickLeavePurchase.iblnPageNewMode = true;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.down_payment = icdoWssBenefitcalculator.down_payments;
                ibusSickLeavePurchase.ibusPersonAccount = ibusBenefitCalculation.ibusPersonAccount;
                ibusSickLeavePurchase.iblnLoadMemberTypeByPlan = true;
                //TODO: needs to be revisted after member type PIR fixed
                cdoPersonEmploymentDetail lcdoPersonEmpDetail = ibusSickLeavePurchase.LoadMemberTypeByContributingStatus().FirstOrDefault();
                if (lcdoPersonEmpDetail != null)
                    ibusSickLeavePurchase.icdoServicePurchaseHeader.member_type_value = lcdoPersonEmpDetail.derived_member_type_value;
                ibusSickLeavePurchase.ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader = ibusSickLeavePurchase;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.expected_installment_amount = icdoWssBenefitcalculator.payment_amount;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.number_of_payments = icdoWssBenefitcalculator.number_of_payments;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.payment_frequency_value = icdoWssBenefitcalculator.payment_frequency_value;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.payroll_deduction = busConstant.Flag_No;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.pre_tax = busConstant.Flag_No;
                ibusSickLeavePurchase.icdoServicePurchaseHeader.ienuObjectState = ObjectState.Insert;
                ibusSickLeavePurchase.CalculateCurrentAge();
                ibusSickLeavePurchase.ibusPrimaryServicePurchaseDetail.CalculateTimeToPurchaseForUnUsedSickLeave();
            }

            if (icdoWssBenefitcalculator.additional_serivce_purchase_selected == busConstant.Flag_Yes)
            {
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.person_id = ibusMember.icdoPerson.person_id;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.plan_id = icdoWssBenefitcalculator.plan_id;
                ibusConsolidatedPurchase.ibusPerson = ibusMember;
                ibusConsolidatedPurchase.ibusPlan = ibusPlan;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.date_of_purchase = DateTime.Now;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.action_status_value = busConstant.Service_Purchase_Action_Status_Pending;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.service_purchase_status_value = busConstant.Service_Purchase_Status_Review;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.service_purchase_type_value = busConstant.Service_Purchase_Type_Consolidated_Purchase;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.payor_value = busConstant.Service_Purchase_Payor_Employee;
                //Setting this true for system to calculate FAS
                ibusConsolidatedPurchase.iblnPageNewMode = true;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.down_payment = icdoWssBenefitcalculator.down_payments;
                ibusConsolidatedPurchase.ibusPersonAccount = ibusBenefitCalculation.ibusPersonAccount;
                ibusConsolidatedPurchase.iblnLoadMemberTypeByPlan = true;
                //PIR 18462 - MSS benefit calculator Error
                cdoPersonEmploymentDetail lcdoPersonEmpDetail = ibusConsolidatedPurchase.LoadMemberTypeByContributingStatus().FirstOrDefault();
                if (lcdoPersonEmpDetail != null)
                    ibusConsolidatedPurchase.icdoServicePurchaseHeader.member_type_value = lcdoPersonEmpDetail.derived_member_type_value;
                ibusConsolidatedPurchase.ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader = ibusConsolidatedPurchase;

                //Setting up the Consolidated Entries
                ibusConsolidatedPurchase.ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated = new Collection<busServicePurchaseDetailConsolidated>();
                foreach (busConsolidatedPurchaseWeb lbusConsolidatedPurchaseWeb in iclbConsolidatedPurchase)
                {
                    if (lbusConsolidatedPurchaseWeb.IsValidEntry())
                    {
                        busServicePurchaseDetailConsolidated lbusSPConsolidated = new busServicePurchaseDetailConsolidated
                        {
                            icdoServicePurchaseDetailConsolidated = new cdoServicePurchaseDetailConsolidated()
                        };
                        lbusSPConsolidated.icdoServicePurchaseDetailConsolidated.service_credit_type_value = lbusConsolidatedPurchaseWeb.istrConsolidatePurchaseTypeValue;
                        lbusSPConsolidated.icdoServicePurchaseDetailConsolidated.time_to_purchase = lbusConsolidatedPurchaseWeb.iintAdditionalServiceCredits;
                        lbusSPConsolidated.icdoServicePurchaseDetailConsolidated.service_purchase_start_date = lbusConsolidatedPurchaseWeb.idtFromDate;
                        lbusSPConsolidated.icdoServicePurchaseDetailConsolidated.service_purchase_end_date = lbusConsolidatedPurchaseWeb.idtToDate;
                        lbusSPConsolidated.ibusServicePurchaseDetail = ibusConsolidatedPurchase.ibusPrimaryServicePurchaseDetail;
                        lbusSPConsolidated.ibusServicePurchaseDetail.ibusServicePurchaseHeader = ibusConsolidatedPurchase;
                        lbusSPConsolidated.BeforeValidate(utlPageMode.All);
                        lbusSPConsolidated.icdoServicePurchaseDetailConsolidated.ienuObjectState = ObjectState.Insert;
                        ibusConsolidatedPurchase.ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated.Add(lbusSPConsolidated);
                    }
                }

                ibusConsolidatedPurchase.icdoServicePurchaseHeader.expected_installment_amount = icdoWssBenefitcalculator.payment_amount;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.number_of_payments = icdoWssBenefitcalculator.number_of_payments;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.payment_frequency_id = 330;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.payment_frequency_value = icdoWssBenefitcalculator.payment_frequency_value;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.payroll_deduction = busConstant.Flag_No;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.pre_tax = busConstant.Flag_No;
                ibusConsolidatedPurchase.icdoServicePurchaseHeader.ienuObjectState = ObjectState.Insert;
                ibusConsolidatedPurchase.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.ienuObjectState = ObjectState.Insert;
                ibusConsolidatedPurchase.CalculateCurrentAge();
                ibusConsolidatedPurchase.ibusPrimaryServicePurchaseDetail.CalculateTimetoPurchaseForConsolidated();
            }
        }

        private void SetupStep3DeductionObject()
        {
            if (ibusMember.IsMemberEnrolledInPlan(busConstant.PlanIdGroupHealth))
            {
                //Is Medicare Attained                
                ibusMember.LoadPersonAccountByPlan(busConstant.PlanIdGroupHealth);
                busPersonAccount lbusHealthPersonAccount = ibusMember.icolPersonAccountByPlan.FirstOrDefault();
                if (lbusHealthPersonAccount != null)
                {
                    if (lbusHealthPersonAccount.ibusPersonAccountGHDV == null)
                        lbusHealthPersonAccount.LoadPersonAccountGHDV();
                    ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.ibusPersonAccountGHDV = lbusHealthPersonAccount.ibusPersonAccountGHDV;
                    if (busGlobalFunctions.CalulateAge(ibusMember.icdoPerson.date_of_birth, DateTime.Now) >= 65) // PIR 10751
                    {
                        ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.health_insurance_type_value = busConstant.HealthInsuranceTypeRetiree;
                    }
                    else
                    {
                        ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.health_insurance_type_value =
                        lbusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value;
                        if (lbusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value != busConstant.HealthInsuranceTypeRetiree)
                        {
                            ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.cobra_type_value = busConstant.COBRATypeRetiree18Month;
                            if (lbusHealthPersonAccount.ibusPersonAccountGHDV.iclbAccountEmploymentDetail == null)
                                lbusHealthPersonAccount.ibusPersonAccountGHDV.LoadPersonAccountEmploymentDetails();
                            busPersonAccountEmploymentDetail lbusLatestPAEmpDetail = lbusHealthPersonAccount.ibusPersonAccountGHDV.iclbAccountEmploymentDetail
                                                                                    .Where(i => i.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled)
                                                                                    .OrderByDescending(i => i.icdoPersonAccountEmploymentDetail.person_account_employment_dtl_id).FirstOrDefault();
                            if (lbusLatestPAEmpDetail != null)
                            {
                                if (lbusLatestPAEmpDetail.ibusEmploymentDetail == null)
                                    lbusLatestPAEmpDetail.LoadPersonEmploymentDetail();

                                lbusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.person_employment_dtl_id = lbusLatestPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                                lbusHealthPersonAccount.ibusPersonAccountGHDV.LoadOrgPlan(DateTime.Now);
                                ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.plan_option_value = lbusHealthPersonAccount.ibusPersonAccountGHDV.ibusOrgPlan.icdoOrgPlan.plan_option_value;
                            }

                        }
                    }
                }
                else
                {
                    //defaulting insurance type value
                    //rule - BR-24-56 - if member is not enrolled in the health plan then set to retiree    
                    ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.health_insurance_type_value = busConstant.HealthInsuranceTypeRetiree;
                    ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.icdoBenefitGhdvDeduction.plan_option_value = null;
                }
            }

            //Reload the Coverage Code based on the insurance type and cobra type selection                 
            ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.LoadGHDVObjectFromDeduction();
            ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.ibusPersonAccountGHDV.LoadRateStructure(DateTime.Now);
            ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.ibusPersonAccountGHDV.LoadCoverageRefID();
        }

        private void ValidateAndPersistBenefitCalculationObject()
        {
            if (icdoWssBenefitcalculator.is_estimate_added_flag != busConstant.Flag_Yes || !IsPersonEnrolledInLife())
                idecLifeBasicPremiumAmt = 0;
            if (istrBenefitAccountTypeValue != busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                iblnIsPersonEligibleForEarly = ibusBenefitCalculation.IsPersonEligibleForEarly();
                ibusBenefitCalculation.icdoBenefitCalculation.status_value = busConstant.ApplicationStatusReview;
                ibusBenefitCalculation.icdoBenefitCalculation.status_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1902, busConstant.ApplicationStatusReview);
                ibusBenefitCalculation.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusPendingApproval;
                ibusBenefitCalculation.icdoBenefitCalculation.created_date = DateTime.Now;
                ibusBenefitCalculation.icdoBenefitCalculation.ienuObjectState = ObjectState.Insert;
                ibusBenefitCalculation.iblnIsNewMode = true;

                if (!ibusBenefitCalculation.iblnConsoldatedVSCLoaded)
                    ibusBenefitCalculation.CalculateConsolidatedVSC();
                if (!ibusBenefitCalculation.iblnConsolidatedPSCLoaded)
                    ibusBenefitCalculation.CalculateConsolidatedPSC();

                ibusBenefitCalculation.SetBenefitSubType();
            }
        }

        private void SetStep3DeductionObject()
        {
            ibusDeductionCalculationWeb.ibusPerson = ibusMember;
            LoadBenefitLifeNew();
        }

        private void PersistsSickLeavePurchase()
        {
            ibusSickLeavePurchase.icdoServicePurchaseHeader.is_created_from_portal = busConstant.Flag_Yes;
            ibusSickLeavePurchase.iarrChangeLog.Add(ibusSickLeavePurchase.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail);
            ibusSickLeavePurchase.retirement_date_from_portal = ibusBenefitCalculation.icdoBenefitCalculation.retirement_date;
            ibusSickLeavePurchase.termination_date_from_portal = ibusBenefitCalculation.icdoBenefitCalculation.termination_date;
            ibusSickLeavePurchase.BeforePersistChanges();
            ibusSickLeavePurchase.PersistChanges();
            ibusSickLeavePurchase.ValidateSoftErrors();
            ibusSickLeavePurchase.UpdateValidateStatus();
            ibusSickLeavePurchase.AfterPersistChanges();
        }

        private void PersistsConsolidatedPurchase()
        {
            ibusConsolidatedPurchase.icdoServicePurchaseHeader.is_created_from_portal = busConstant.Flag_Yes;
            ibusConsolidatedPurchase.iarrChangeLog.Add(ibusConsolidatedPurchase.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail);
            ibusConsolidatedPurchase.BeforePersistChanges();
            ibusConsolidatedPurchase.PersistChanges();
            ibusConsolidatedPurchase.ValidateSoftErrors();
            ibusConsolidatedPurchase.UpdateValidateStatus();
            ibusConsolidatedPurchase.AfterPersistChanges();
            //insert consolidated enteries
            foreach (busServicePurchaseDetailConsolidated lobjSPConsolidated in ibusConsolidatedPurchase.ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated)
            {
                lobjSPConsolidated.ibusServicePurchaseDetail = ibusConsolidatedPurchase.ibusPrimaryServicePurchaseDetail;
                lobjSPConsolidated.ibusServicePurchaseDetail.ibusServicePurchaseHeader = ibusConsolidatedPurchase;
                lobjSPConsolidated.BeforePersistChanges();
                lobjSPConsolidated.PersistChanges();
                lobjSPConsolidated.ValidateSoftErrors();
                lobjSPConsolidated.UpdateValidateStatus();
                lobjSPConsolidated.AfterPersistChanges();
            }
        }

        private void PersistsBenefitCalculation()
        {
            if (icdoWssBenefitcalculator.is_pre_retirement_death_flag == busConstant.Flag_Yes)
            {
                // PIR 9711
                ibusPreRetirementDeathCalculation.icdoBenefitCalculation.is_created_from_portal = busConstant.Flag_Yes;
                ibusPreRetirementDeathCalculation.iblnConsoldatedVSCLoaded = false;
                ibusPreRetirementDeathCalculation.iblnConsolidatedPSCLoaded = false;
                ibusPreRetirementDeathCalculation.BeforePersistChanges();
                ibusPreRetirementDeathCalculation.PersistChanges();
                ibusPreRetirementDeathCalculation.ValidateSoftErrors();
                ibusPreRetirementDeathCalculation.UpdateValidateStatus();
                ibusPreRetirementDeathCalculation.AfterPersistChanges();
            }
            else
            {
                ibusBenefitCalculation.icdoBenefitCalculation.is_created_from_portal = busConstant.Flag_Yes;
                ibusBenefitCalculation.iblnConsoldatedVSCLoaded = false;
                ibusBenefitCalculation.iblnConsolidatedPSCLoaded = false;
                ibusBenefitCalculation.BeforePersistChanges();
                ibusBenefitCalculation.PersistChanges();
                ibusBenefitCalculation.ValidateSoftErrors();
                ibusBenefitCalculation.UpdateValidateStatus();
                ibusBenefitCalculation.AfterPersistChanges();
            }
        }

        private void PersistsDeductionObjects()
        {
            ibusDeductionCalculationWeb.icdoBenefitCalculation = ibusBenefitCalculation.icdoBenefitCalculation;
            ibusDeductionCalculationWeb.LoadBenefitDeductionSummary();
            iclbDeductionCalculationWeb = new Collection<busDeductionCalculation>();

            Collection<busBenefitCalculationOptions> lclbBenefitOptions = new Collection<busBenefitCalculationOptions>();
            if (icdoWssBenefitcalculator.is_pre_retirement_death_flag == busConstant.Flag_Yes)
                lclbBenefitOptions = ibusPreRetirementDeathCalculation.iclbBenefitCalculationOptions;
            else
                lclbBenefitOptions = ibusBenefitCalculation.iclbBenefitCalculationOptions;

            // Only one deduction record should be created ir-respective of the benefit options' count
            //foreach (busBenefitCalculationOptions lobjBenefitOptions in lclbBenefitOptions)
            //{
            busDeductionCalculation lobjNewDeductionCalculations = new busDeductionCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            lobjNewDeductionCalculations = ibusDeductionCalculationWeb;
            lobjNewDeductionCalculations.ibusBenefitCalculationOptions = new busBenefitCalculationOptions();
            if (lclbBenefitOptions.IsNotNull() && lclbBenefitOptions.Count > 0)
                lobjNewDeductionCalculations.ibusBenefitCalculationOptions.FindBenefitCalculationOptions(lclbBenefitOptions[0].icdoBenefitCalculationOptions.benefit_calculation_options_id);

            lobjNewDeductionCalculations.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.gross_monthly_benefit_amount =
               lobjNewDeductionCalculations.ibusBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_option_amount;
            iclbDeductionCalculationWeb.Add(lobjNewDeductionCalculations);

            if (ibusMember.ibusSpouse.IsNotNull())
                lobjNewDeductionCalculations.iintSpousePersonid = ibusMember.ibusSpouse.icdoPerson.person_id;
            lobjNewDeductionCalculations.idecTotalLtcPremium = 0M;
            lobjNewDeductionCalculations.idecTotalLifePremium = 0M;
            if (lobjNewDeductionCalculations.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.rhic_overridden_amount == 0M)
                lobjNewDeductionCalculations.ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.rhic_overridden_amount =
                     Math.Round(((1 - lobjNewDeductionCalculations.icdoBenefitCalculation.rhic_early_reduction_factor) *
                     lobjNewDeductionCalculations.icdoBenefitCalculation.unreduced_rhic_amount), 2, MidpointRounding.AwayFromZero);
            lobjNewDeductionCalculations.BeforePersistChanges();
            lobjNewDeductionCalculations.PersistChanges();
            lobjNewDeductionCalculations.ValidateSoftErrors();
            lobjNewDeductionCalculations.UpdateValidateStatus();
            lobjNewDeductionCalculations.AfterPersistChanges();

            //load the updated benefit by plan
            lobjNewDeductionCalculations.LoadBenefitDentalDeduction();
            lobjNewDeductionCalculations.LoadBenefitHealthDeduction();
            lobjNewDeductionCalculations.LoadBenefitVisionDeduction();
            lobjNewDeductionCalculations.LoadBenefitLifeDeductions();
            lobjNewDeductionCalculations.LoadBenefitLtcMemberDeductions();
            lobjNewDeductionCalculations.LoadBenefitLtcSpouseDeductions();
            lobjNewDeductionCalculations.LoadBenefitPayeeFedTaxWithholding();
            lobjNewDeductionCalculations.LoadBenefitPayeeStateTaxWithholding();
            //}
        }

        //public ArrayList btnRefreshCoverageCodeList_Click()
        //{
        //    ArrayList larrList = new ArrayList();
        //    ibusDeductionCalculationWeb.LoadHealthPlanOption();
        //    ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.LoadGHDVObjectFromDeduction();
        //    ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.ibusPersonAccountGHDV.LoadRateStructure(DateTime.Now);
        //    ibusDeductionCalculationWeb.ibusBenefitHealthDeduction.ibusPersonAccountGHDV.LoadCoverageRefID();
        //    larrList.Add(this);
        //    return larrList;
        //}

        public override void ValidateGroupRules(string astrGroupName, utlPageMode aenmPageMode)
        {
            //by default the system will set the mode to update
            //this object must be set to new mode 
            //always new record will be created
            aenmPageMode = utlPageMode.New;
            base.ValidateGroupRules(astrGroupName, aenmPageMode);
            switch (astrGroupName)
            {
                case "WizardStep1":
                    ValidateErrorsForBenefitCalculation(aenmPageMode);
                    break;
                case "WizardStep2":
                    ValidateErrorsForPurchase(aenmPageMode);
                    break;
                case "WizardStep3":
                    ValidateErrorsForDeductions(aenmPageMode);
                    break;
            }
        }

        private void ValidateErrorsForBenefitCalculation(utlPageMode aenmPageMode)
        {
            if (icdoWssBenefitcalculator.is_pre_retirement_death_flag == busConstant.Flag_Yes)
            {
                ibusPreRetirementDeathCalculation.iarrErrors = new ArrayList();
                ibusPreRetirementDeathCalculation.BeforeValidate(aenmPageMode);
                ibusPreRetirementDeathCalculation.ValidateHardErrors(aenmPageMode);
                if (ibusPreRetirementDeathCalculation.iarrErrors.Count > 0)
                {
                    foreach (utlError lerrors in ibusPreRetirementDeathCalculation.iarrErrors)
                    {
                        lerrors.istrErrorID = string.Empty;
                        iarrErrors.Add(lerrors);
                    }
                }
            }
            else
            {
                ibusBenefitCalculation.iarrErrors = new ArrayList();
                ibusBenefitCalculation.BeforeValidate(aenmPageMode);
                ibusBenefitCalculation.ValidateHardErrors(aenmPageMode);
                if (ibusBenefitCalculation.iarrErrors.Count > 0)
                {
                    foreach (utlError lerrors in ibusBenefitCalculation.iarrErrors)
                    {
                        if (!lerrors.istrErrorID.Equals("2055"))
                        {
                            lerrors.istrErrorID = string.Empty;
                            iarrErrors.Add(lerrors);
                        }
                    }
                }
            }
        }

        private void ValidateErrorsForDeductions(utlPageMode aenmPageMode)
        {
            ibusDeductionCalculationWeb.iarrErrors = new ArrayList();
            ibusDeductionCalculationWeb.BeforeValidate(aenmPageMode);
            ibusDeductionCalculationWeb.ValidateHardErrors(aenmPageMode);
            if (ibusDeductionCalculationWeb.iarrErrors.Count > 0)
            {
                foreach (utlError lerrors in ibusDeductionCalculationWeb.iarrErrors)
                {
                    lerrors.istrErrorID = string.Empty;
                    iarrErrors.Add(lerrors);
                }
                return;
            }

            busPersonAccountLife lobjPersonAccountLife = new busPersonAccountLife
            {
                icdoPersonAccountLife = new cdoPersonAccountLife(),
                icdoPersonAccount = new cdoPersonAccount()
            };
            lobjPersonAccountLife.icdoPersonAccount.plan_id = icdoWssBenefitcalculator.plan_id;
            lobjPersonAccountLife.icdoPersonAccount.person_id = ibusMember.icdoPerson.person_id;
            lobjPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value = ibusDeductionCalculationWeb.istrLifeInsuranceTypeValue;
            lobjPersonAccountLife.icdoPersonAccount.history_change_date = DateTime.Today;
            lobjPersonAccountLife.iclbLifeOption = new Collection<busPersonAccountLifeOption>();
            lobjPersonAccountLife.ibusPaymentElection = new busPersonAccountPaymentElection { icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection() };
            lobjPersonAccountLife.ibusPerson = ibusMember;

            foreach (busBenefitLifeDeduction lobjBenLife in ibusDeductionCalculationWeb.iclbBenefitLifeDeduction)
            {
                busPersonAccountLifeOption lobjPersonAccountLifeOption = new busPersonAccountLifeOption { icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption() };
                lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = lobjBenLife.icdoBenefitLifeDeduction.level_of_coverage_value;
                lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.coverage_amount = lobjBenLife.icdoBenefitLifeDeduction.coverage_amount;
                if (lobjBenLife.icdoBenefitLifeDeduction.coverage_amount != 0.00M)
                {
                    lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
                    lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.effective_start_date = DateTime.Now;
                    lobjPersonAccountLife.iclbLifeOption.Add(lobjPersonAccountLifeOption);
                }
            }

            lobjPersonAccountLife.BeforeValidate(utlPageMode.All);
            lobjPersonAccountLife.ValidateGroupRules("PortalWizardRules", utlPageMode.All);
            if (lobjPersonAccountLife.iarrErrors.Count > 0)
            {
                foreach (utlError lerrors in lobjPersonAccountLife.iarrErrors)
                {
                    lerrors.istrErrorID = string.Empty;
                    iarrErrors.Add(lerrors);
                }
                return;
            }
        }

        public busPersonAccountLife ibuPersonAccountLife { get; set; }
        private void ValidateErrorsForPurchase(utlPageMode aenmPageMode)
        {
            ibusConsolidatedPurchase.iarrErrors = new ArrayList(); //PIR 11765
            if (icdoWssBenefitcalculator.unused_service_purchase_selected == busConstant.Flag_Yes)
            {
                ibusSickLeavePurchase.BeforeValidate(aenmPageMode);
                ibusSickLeavePurchase.ValidateHardErrors(aenmPageMode);
                if (ibusSickLeavePurchase.iarrErrors.Count > 0)
                {
                    foreach (utlError lerrors in ibusSickLeavePurchase.iarrErrors)
                    {
                        if (lerrors.istrErrorID != "4731") // PIR 9964 Payment validation not valid for Sickleave Purchase
                        {
                            lerrors.istrErrorID = string.Empty;
                            iarrErrors.Add(lerrors);
                        }
                    }
                }
            }
            if (icdoWssBenefitcalculator.additional_serivce_purchase_selected == busConstant.Flag_Yes)
            {
                ibusConsolidatedPurchase.BeforeValidate(aenmPageMode);
                ibusConsolidatedPurchase.ValidateHardErrors(aenmPageMode);

                if (ibusConsolidatedPurchase.iarrErrors.Count > 0)
                {
                    foreach (utlError lerrors in ibusConsolidatedPurchase.iarrErrors)
                    {
                        lerrors.istrErrorID = string.Empty;
                        iarrErrors.Add(lerrors);
                    }
                }
                foreach (busServicePurchaseDetailConsolidated lobjSPConsolidated in ibusConsolidatedPurchase.ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated)
                {
                    lobjSPConsolidated.ValidateHardErrors(aenmPageMode);
                    if (lobjSPConsolidated.iarrErrors.Count > 0)
                    {
                        foreach (utlError lerrors in lobjSPConsolidated.iarrErrors)
                        {
                            lerrors.istrErrorID = string.Empty;
                            iarrErrors.Add(lerrors);
                        }
                    }
                }
            }
        }

        public override int PersistChanges()
        {
            if (icdoWssBenefitcalculator.unused_service_purchase_selected == busConstant.Flag_Yes &&
                ibusSickLeavePurchase.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.unused_sick_leave_hours > 0)
            {
                PersistsSickLeavePurchase();
                // add this object to benefit calculation service purchase
                AddToBenefitServicePurchase(ibusSickLeavePurchase);
            }
            if (icdoWssBenefitcalculator.additional_serivce_purchase_selected == busConstant.Flag_Yes)
            {
                PersistsConsolidatedPurchase();
                // add this object to benefit calculation service purchase
                AddToBenefitServicePurchase(ibusConsolidatedPurchase);
            }
            //benefit calculations
            PersistsBenefitCalculation();
            if (icdoWssBenefitcalculator.is_estimate_added_flag == busConstant.Flag_Yes)
                PersistsDeductionObjects();
            //if (icdoWssBenefitcalculator.is_pre_retirement_death_flag == busConstant.Flag_Yes)
            //    icdoWssBenefitcalculator.benefit_calculation_id = ibusPreRetirementDeathCalculation.icdoBenefitCalculation.benefit_calculation_id;
            //else
            icdoWssBenefitcalculator.benefit_calculation_id = ibusBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id;

            if (icdoWssBenefitcalculator.additional_serivce_purchase_selected == busConstant.Flag_Yes)
                icdoWssBenefitcalculator.consolidated_service_purchase_header_id = ibusConsolidatedPurchase.icdoServicePurchaseHeader.service_purchase_header_id;
            else // Added for MSS Sick Leave issue.
            {
                // Service purchase detail record was created without header ID causes incorrect calculation results
                foreach (object lobjtemp in iarrChangeLog)
                {
                    if (lobjtemp is cdoServicePurchaseDetail)
                    {
                        iarrChangeLog.Remove(lobjtemp);
                        break;
                    }
                }
            }
            if (icdoWssBenefitcalculator.unused_service_purchase_selected == busConstant.Flag_Yes)
                icdoWssBenefitcalculator.unused_sick_leave_service_purchase_header_id = ibusSickLeavePurchase.icdoServicePurchaseHeader.service_purchase_header_id;
            else
            {
                foreach (object lobjtemp in iarrChangeLog)
                {
                    if (lobjtemp is cdoServicePurchaseDetail)
                    {
                        iarrChangeLog.Remove(lobjtemp);
                        break;
                    }
                }
            }
            // PIR 14652 - Blank entry was inserted in person table if spouse's date of birth was entered, who was not on records.
            foreach (object lobj in iarrChangeLog)
            {
                if (lobj is cdoPerson)
                {
                    //if (ibusBenefitCalculation.ibusJointAnnuitant.icdoPerson.person_id == 0 && ibusBenefitCalculation.ibusJointAnnuitant.icdoPerson.last_name == null
                    //    && ibusBenefitCalculation.ibusJointAnnuitant.icdoPerson.first_name == null)
                    //{
                    iarrChangeLog.Remove(lobj);
                    break;
                    //}
                }
            }
			//F/W Upgrade PIR 11168 - The main cdo object not getting added to Changelog 
            if (iarrChangeLog.IsNotNull() && !iarrChangeLog.Any(ldoChangelog => ldoChangelog is cdoWssBenefitCalculator))
            {
                iarrChangeLog.Add(icdoWssBenefitcalculator);
            }
            if (icdoWssBenefitcalculator.wss_benefit_calculator_id == 0 & icdoWssBenefitcalculator.ienuObjectState != ObjectState.Insert)
                icdoWssBenefitcalculator.ienuObjectState = ObjectState.Insert;
            return base.PersistChanges();
        }

        private void AddToBenefitServicePurchase(busServicePurchaseHeader abusServicePurchase)
        {
            busBenefitServicePurchase lobjBenefitServicePurchase = new busBenefitServicePurchase { icdoBenefitServicePurchase = new cdoBenefitServicePurchase() };
            lobjBenefitServicePurchase.icdoBenefitServicePurchase.service_purchase_header_id = abusServicePurchase.icdoServicePurchaseHeader.service_purchase_header_id;
            lobjBenefitServicePurchase.icdoBenefitServicePurchase.remaining_psc = abusServicePurchase.TotalTimeToPurchase;
            lobjBenefitServicePurchase.istrIncludeRemainder = busConstant.Flag_Yes;
            lobjBenefitServicePurchase.icdoBenefitServicePurchase.ienuObjectState = ObjectState.Insert;
            ibusBenefitCalculation.iclbBenefitServicePurchaseAll.Add(lobjBenefitServicePurchase);
        }

        public bool iblnIsDataNotEnteredInAdditionalSC()
        {
            if (iclbConsolidatedPurchase.IsNotNull())
            {
                var lenumCount1 = iclbConsolidatedPurchase.Where(lobjSC => lobjSC.idtFromDate != DateTime.MinValue
                                                                    && lobjSC.idtToDate != DateTime.MinValue
                                                                    && lobjSC.istrConsolidatePurchaseTypeValue != busConstant.Service_Purchase_Type_Additional_Service_Credit).Count();
                var lenumCount2 = iclbConsolidatedPurchase.Where(lobjSC => lobjSC.iintAdditionalServiceCredits != 0
                                                                    && lobjSC.istrConsolidatePurchaseTypeValue == busConstant.Service_Purchase_Type_Additional_Service_Credit).Count();
                if (lenumCount1 == 0
                    && lenumCount2 == 0)
                    return true;
            }
            return false;
        }
        public bool IsMember401aLimitReached()
        {
            if(ibusMember.IsNotNull() && ibusMember?.icdoPerson?.person_id > 0 && ibusMember?.icdoPerson?.limit_401a == busConstant.Flag_Yes)
            {
                return true;
            }
            return false;
        }
        private DateTime GetNormalRetirementEligibilityDate()
        {
            decimal ldecConsolidatedServiceCredit = 0.0M;
            //if (ibusBenefitCalculation.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate ||
            //    ibusBenefitCalculation.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) //PIR 19594
            //{
            //    if (ibusBenefitCalculation.idecRemainingServiceCredit == 0.00M)
            //        ibusBenefitCalculation.LoadRemainingServicePurchaseCredit();
            //    ldecConsolidatedServiceCredit = ibusBenefitCalculation.idecRemainingServiceCredit + ibusBenefitCalculation.icdoBenefitCalculation.adjusted_tvsc;
            //}
            //PIR 23975 - TFFR TIAA Service credit is also added. 
            ldecConsolidatedServiceCredit = ibusBenefitCalculation.GetConsolidatedExtraServiceCredit();

            //if plan id is DC, then check if person is eligible for Main
            //then get normal retirement date based on main
            int lintPlanId = 0;
            string lstrPlanCode = string.Empty;
            int lintBenefitProvisionId = 0;
            lintPlanId = icdoWssBenefitcalculator.plan_id;
            lstrPlanCode = ibusPlan.icdoPlan.plan_code;
            lintBenefitProvisionId = ibusPlan.icdoPlan.benefit_provision_id;

            if (icdoWssBenefitcalculator.plan_id == busConstant.PlanIdDC || icdoWssBenefitcalculator.plan_id == busConstant.PlanIdDC2020 || icdoWssBenefitcalculator.plan_id == busConstant.PlanIdDC2025) //PIR 20232
                if (ibusMember.IsMemberEnrolledInPlan(busConstant.PlanIdMain) || ibusMember.IsMemberEnrolledInPlan(busConstant.PlanIdMain2020)) //PIR 20232
                {

                    busPlan lbusPlan = new busPlan();
                    if (ibusBenefitCalculation.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain)
                    {
                        lintPlanId = busConstant.PlanIdMain;
                        lbusPlan.FindPlan(busConstant.PlanIdMain);
                    }
                    else if (ibusBenefitCalculation.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020)
                    {
                        lintPlanId = busConstant.PlanIdMain2020;
                        lbusPlan.FindPlan(busConstant.PlanIdMain2020);
                    }
                    lstrPlanCode = lbusPlan.icdoPlan.plan_code;
                    lintBenefitProvisionId = lbusPlan.icdoPlan.benefit_provision_id;
                }

            ibusBenefitCalculation.icdoBenefitCalculation.normal_retirement_date =
                busPersonBase.GetNormalRetirementDateBasedOnNormalEligibility(lintPlanId, lstrPlanCode,
                                                            lintBenefitProvisionId, busConstant.ApplicationBenefitTypeRetirement,
                                                            ibusMember.icdoPerson.date_of_birth, ibusBenefitCalculation.icdoBenefitCalculation.credited_vsc,
                                                            ibusBenefitCalculation.icdoBenefitCalculation.is_rule_or_age_conversion, iobjPassInfo,
                                                            ibusBenefitCalculation.icdoBenefitCalculation.termination_date,
                                                            ibusBenefitCalculation.ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                            (ibusBenefitCalculation.icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimate)||
                                                            ibusBenefitCalculation.icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimateSubsequent)), //PIR 19594
                                                            ldecConsolidatedServiceCredit, ibusBenefitCalculation.icdoBenefitCalculation.retirement_date); // PIR 14646
            return ibusBenefitCalculation.icdoBenefitCalculation.normal_retirement_date;
        }

        public Collection<busDeductionCalculation> iclbDeductionCalculationWeb { get; set; }

        public void GetTentativeOrApprovedTFFRTIAAAmount()
        {
            if (ibusMember.iclbTffrTiaaService.IsNull())
                ibusMember.LoadTffrTiaaService();

            //PIR 25568
            var lTFFRTIAAServiceCredit = ibusMember.iclbTffrTiaaService.Where(lobjT => (lobjT.icdoPersonTffrTiaaService.tffr_service_status_value == busConstant.PersonTFFRTIAAServiceStatusApproved
                || lobjT.icdoPersonTffrTiaaService.tffr_service_status_value == busConstant.PersonTFFRTIAAServiceStatusTentative) ||
                (lobjT.icdoPersonTffrTiaaService.tiaa_service_status_value == busConstant.PersonTFFRTIAAServiceStatusApproved
                || lobjT.icdoPersonTffrTiaaService.tiaa_service_status_value == busConstant.PersonTFFRTIAAServiceStatusTentative)).Sum(lobjT => lobjT.icdoPersonTffrTiaaService.tiaa_service +
                    lobjT.icdoPersonTffrTiaaService.tffr_service);

            icdoWssBenefitcalculator.tffr_tiaa_service_credit = (decimal)lTFFRTIAAServiceCredit;
        }

        public decimal idecLifeBasicPremiumAmt { get; set; }
        public decimal idecLifeSupplementalPremiumAmount { get; set; }
        public string istrDepedendentCoverageOptionValue { get; set; }
        public decimal idecSpouseSupplementalPremiumAmt { get; set; }


        /// <summary>
        /// Loading Group Life Option Grid objects in new mode.
        /// </summary>
        private void LoadBenefitLifeNew()
        {
            ibusDeductionCalculationWeb.iclbBenefitLifeDeduction = new Collection<busBenefitLifeDeduction>();
            ibusDeductionCalculationWeb.istrLifeInsuranceTypeValue = busConstant.LifeInsuranceTypeRetireeMember;

            busBenefitLifeDeduction lobjBasicLifeOption = new busBenefitLifeDeduction();
            lobjBasicLifeOption.icdoBenefitLifeDeduction = new cdoBenefitLifeDeduction();
            lobjBasicLifeOption.icdoBenefitLifeDeduction.benefit_life_deduction_id = -1;
            lobjBasicLifeOption.icdoBenefitLifeDeduction.level_of_coverage_value = busConstant.LevelofCoverage_Basic;
            lobjBasicLifeOption.icdoBenefitLifeDeduction.level_of_coverage_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_Basic);
            lobjBasicLifeOption.icdoBenefitLifeDeduction.life_insurance_type_value = ibusDeductionCalculationWeb.istrLifeInsuranceTypeValue;
            lobjBasicLifeOption.icdoBenefitLifeDeduction.coverage_amount = idecLifeBasicPremiumAmt;
            lobjBasicLifeOption.icdoBenefitLifeDeduction.ienuObjectState = ObjectState.Insert;
            ibusDeductionCalculationWeb.iclbBenefitLifeDeduction.Add(lobjBasicLifeOption);

            busBenefitLifeDeduction lobjSupplementalLifeOption = new busBenefitLifeDeduction();
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction = new cdoBenefitLifeDeduction();
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction.benefit_life_deduction_id = -2;
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction.level_of_coverage_value = busConstant.LevelofCoverage_Supplemental;
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction.level_of_coverage_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_Supplemental);
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction.life_insurance_type_value = ibusDeductionCalculationWeb.istrLifeInsuranceTypeValue;
            if (idecLifeSupplementalPremiumAmount > 0.00M)
                lobjSupplementalLifeOption.icdoBenefitLifeDeduction.coverage_amount = idecLifeSupplementalPremiumAmount - idecLifeBasicPremiumAmt;
            lobjSupplementalLifeOption.icdoBenefitLifeDeduction.ienuObjectState = ObjectState.Insert;
            ibusDeductionCalculationWeb.iclbBenefitLifeDeduction.Add(lobjSupplementalLifeOption);

            busBenefitLifeDeduction lobjDependentLifeOption = new busBenefitLifeDeduction();
            lobjDependentLifeOption.icdoBenefitLifeDeduction = new cdoBenefitLifeDeduction();
            lobjDependentLifeOption.icdoBenefitLifeDeduction.benefit_life_deduction_id = -3;
            lobjDependentLifeOption.icdoBenefitLifeDeduction.level_of_coverage_value = busConstant.LevelofCoverage_DependentSupplemental;
            lobjDependentLifeOption.icdoBenefitLifeDeduction.level_of_coverage_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_DependentSupplemental);
            lobjDependentLifeOption.icdoBenefitLifeDeduction.life_insurance_type_value = ibusDeductionCalculationWeb.istrLifeInsuranceTypeValue;
            lobjDependentLifeOption.icdoBenefitLifeDeduction.coverage_amount = Convert.ToDecimal(istrDepedendentCoverageOptionValue);
            lobjDependentLifeOption.icdoBenefitLifeDeduction.ienuObjectState = ObjectState.Insert;
            ibusDeductionCalculationWeb.iclbBenefitLifeDeduction.Add(lobjDependentLifeOption);

            busBenefitLifeDeduction lobjSpouseLifeOption = new busBenefitLifeDeduction();
            lobjSpouseLifeOption.icdoBenefitLifeDeduction = new cdoBenefitLifeDeduction();
            lobjSpouseLifeOption.icdoBenefitLifeDeduction.benefit_life_deduction_id = -4;
            lobjSpouseLifeOption.icdoBenefitLifeDeduction.level_of_coverage_value = busConstant.LevelofCoverage_SpouseSupplemental;
            lobjSpouseLifeOption.icdoBenefitLifeDeduction.level_of_coverage_description =
                iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.LevelofCoverage_CodeID, busConstant.LevelofCoverage_SpouseSupplemental);
            lobjSpouseLifeOption.icdoBenefitLifeDeduction.life_insurance_type_value = ibusDeductionCalculationWeb.istrLifeInsuranceTypeValue;
            lobjSpouseLifeOption.icdoBenefitLifeDeduction.coverage_amount = idecSpouseSupplementalPremiumAmt;
            lobjSpouseLifeOption.icdoBenefitLifeDeduction.ienuObjectState = ObjectState.Insert;
            ibusDeductionCalculationWeb.iclbBenefitLifeDeduction.Add(lobjSpouseLifeOption);
        }

        // PROD PIR 6860
        public decimal GetBasicPremiumAmount()
        {
            decimal ldecCoverageAmount = 0M;
            DataTable ldtbValidCoverageAmounts = Select("cdoPersonAccountLife.CheckCoverageAmountExists",
                    new object[3] { busConstant.LevelofCoverage_Basic, busConstant.LifeInsuranceTypeRetireeMember, DateTime.Today });
            if (ldtbValidCoverageAmounts.Rows.Count > 0)
                ldecCoverageAmount = Convert.ToDecimal(ldtbValidCoverageAmounts.Rows[0]["FULL_COVERAGE_AMT"]);
            return ldecCoverageAmount;
        }
        //mss pir 7948
        public bool IsPersonEnrolledInLife()
        {
            if (ibusMember.ibusLifePersonAccount.IsNull())
                ibusMember.LoadLifePersonAccount();
            if (ibusMember.ibusLifePersonAccount.icdoPersonAccount.person_account_id > 0 && ibusMember.ibusLifePersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                return true;
            }
            return false;
        }

        public Collection<cdoCodeValue> LoadRetirementYear()
        {
            int lintMaxYear = 2100;
            Collection<cdoCodeValue> lclbRetirementYears = new Collection<cdoCodeValue>();
            for (int i = 2005; i < lintMaxYear; i++)
            {
                cdoCodeValue lcdoCodeValue = new cdoCodeValue();
                lcdoCodeValue.code_value = i.ToString();
                lcdoCodeValue.description = i.ToString();
                lclbRetirementYears.Add(lcdoCodeValue);
            }
            return lclbRetirementYears;
        }

        public Collection<cdoCodeValue> LoadBenefitAccountType()
        {
            Collection<cdoCodeValue> lclbBenefitAccountType = new Collection<cdoCodeValue>();
            Collection<cdoCodeValue> lclbFinalBenefitAccountType = new Collection<cdoCodeValue>();
            lclbBenefitAccountType = GetCodeValue(1904);
            if (ibusBenefitCalculation?.icdoBenefitCalculation?.plan_id > 0)
            {
                foreach (cdoCodeValue lcdoCV in lclbBenefitAccountType)
                {
                    if (lcdoCV.code_value == busConstant.ApplicationBenefitTypePostRetirementDeath || lcdoCV.code_value == busConstant.ApplicationBenefitTypeRefund)
                        continue;
                    if (ibusPlan.IsNull()) LoadPlan();
                    if (lcdoCV.code_value == busConstant.ApplicationBenefitTypeDisability && ((ibusPlan.IsDCRetirementPlan() || ibusPlan.IsHBRetirementPlan()) || ibusBenefitCalculation.IsEmpEndDate12MonthGreaterThanSystemMgntDate())) // PIR 23878
                        continue;
                    lclbFinalBenefitAccountType.Add(lcdoCV);
                }
            }
            return lclbFinalBenefitAccountType;
        }

        public string istrBenefitEstimateType { get; set; }

        //PIR 17073
        public DateTime LoadLastEndDateOfPersonEmployment(int aintPersonID)
        {
            DateTime ldtLastDateOfEmployment = DateTime.MinValue;
            DataTable ldtbResult = Select("cdoPersonEmployment.GetLatestEmpDetailEndDate", new object[1] { aintPersonID });
            if (ldtbResult.Rows.Count > 0)
                ldtLastDateOfEmployment = Convert.ToDateTime(ldtbResult.Rows[0]["end_date"]);

            return ldtLastDateOfEmployment;
        }

        public ArrayList btnGo_click()
        {
            ArrayList alReturn = new ArrayList();
            if (iarrErrors == null) iarrErrors = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPerson == null)
                LoadPerson();
            if (istrBenefitEstimateType.IsNotNullOrEmpty() && (istrBenefitEstimateType== "BENE" || istrBenefitEstimateType == "ESTM" || istrBenefitEstimateType == "BREQ"))
            {
                if (!IsMSSBenefitEstimateTypeValid(ibusPerson.icdoPerson.person_id))
                {
                    lobjError = AddError(0, "You are not enrolled in an NDPERS retirement plan.");
                    iarrErrors.Add(lobjError);
                    return iarrErrors;
                }             
            }
            alReturn.Add(this);
            return alReturn;
        }

        public bool IsMSSBenefitEstimateTypeValid(int aintPersonID)
        {
            busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson { person_id = aintPersonID } };
            if (lobjPerson.iclbRetirementAccount.IsNull()) lobjPerson.LoadRetirementAccount();
            foreach (busPersonAccount lobjPA in lobjPerson.iclbRetirementAccount)
            {
                if (lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                    lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                    return true;
            }
            return false;
        }
        public void LoadPerson()
        {
            if (ibusPerson == null)
            {
                ibusPerson = new busPerson();
            }
            ibusPerson.FindPerson((int)iobjPassInfo.idictParams["PersonID"]);
        }
    }
}
