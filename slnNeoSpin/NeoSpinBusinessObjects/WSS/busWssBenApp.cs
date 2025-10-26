#region Using directives

using System;
using System.Collections.ObjectModel;
using System.Data;
using Sagitec.Common;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Collections;
using NeoSpin.DataObjects;
using System.Collections.Generic;
using Sagitec.DataObjects;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using Sagitec.ExceptionPub;
using System.Collections.Specialized;
using System.Data.SqlClient;
using Sagitec.CustomDataObjects;


#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.busWssBenApp:
    /// Inherited from busWssBenAppGen, the class is used to customize the business object busWssBenAppGen.
    /// </summary>
    [Serializable]
    public class busWssBenApp : busWssBenAppGen
    {
        public bool iblnExternalLogin { get; set; } = false;
        public bool iblnDisplayUpdateContactInfoLink { get; set; }
        public string istrBenTypeValue { get; set; }
        public string istrDisplayErrorMsgText { get; set; }
        public string istrRoutingText { get; set; }

        public Collection<cdoPlan> iclcSupendedRetPlanAccounts { get; set; }
        public Collection<busBenefitApplication> iclbBenApps { get; set; }
        public Collection<busWssBenApp> iclbWSSBenApps { get; set; }
        public Collection<busWssBenApp> iclbWSSBenAppsSACL { get; set; }
        public int PersonAddressId { get; set; }
        #region Overridden Methods
        public override void BeforeWizardStepValidate(utlPageMode aenmPageMode, string astrWizardName, string astrWizardStepName, utlWizardNavigationEventArgs we = null)
        {
            switch (astrWizardStepName)
            {
                case "wzsSelectBenefitType":
                    SetUpWssBenApp();
                    validateOtherDisabilityBenefit();
                    LoadMSSContributionSummaryForRetirementPlans();
                    LoadAllAcknowledgement();
                    break;
                case "wzsDistribution":
                    SetUpPrimaryAchDetail();
                    SetUpSecondaryAchDetail();
                    PopulateDescriptions(icdoWssBenAppRolloverDetail);
                    LoadFedStateFlatTaxRate();
                    break;
                case "wzsTaxWithholding":
                    PopulateDescriptions(icdoWssBenAppTaxWithholdingFederal, icdoWssBenApp.ben_type_value != busConstant.ApplicationBenefitTypeRefund);
                    PopulateDescriptions(icdoWssBenAppTaxWithholdingState, icdoWssBenApp.ben_type_value != busConstant.ApplicationBenefitTypeRefund);                    
                    break;
                case "wzsHealth":
                    SetupInsEnrlRqst(ibusPAEnrollReqHealth);
                    SetDepCobraEnltValues();
                    ibusPAEnrollReqHealth.icdoMSSGDHV.level_of_coverage_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, ibusPAEnrollReqHealth.icdoMSSGDHV.level_of_coverage_value);
                    //Move information on Health enrollment we should display on Medicare Part D step
                    SetupDependentMedicareAndHealth(ibusPAEnrollReqHealth, ibusPAEnrollReqMedicare);
                    break;
                case "wzsDental":
                    SetupInsEnrlRqst(ibusPAEnrollReqDental);
                    SetDepCobraEnltValues();
                    ibusPAEnrollReqDental.icdoMSSGDHV.level_of_coverage_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, ibusPAEnrollReqDental.icdoMSSGDHV.level_of_coverage_value);
                    break;
                case "wzsVision":
                    SetupInsEnrlRqst(ibusPAEnrollReqVision);
                    ibusPAEnrollReqVision.icdoMSSGDHV.level_of_coverage_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, ibusPAEnrollReqVision.icdoMSSGDHV.level_of_coverage_value);
                    SetDepCobraEnltValues();
                    EvaluateInitialLoadRules();
                    break;
                case "wzsInsuranceTwo":
                    SetupInsEnrlRqst(ibusPAEnrollReqMedicare);
                    //Move enter Medicare information on Part D step, it should populate on the Health step
                    SetupDependentMedicareAndHealth(ibusPAEnrollReqMedicare, ibusPAEnrollReqHealth);
                    SetupFlexInsEnrlRqst();
                    break;
                case "wzsDependents":
                    ValidateDependents();
                    break;
                case "wzsStepLifeInsurance":
                    ValidateLifeInsuranceStep();
                    //LifePannel
                    SetupInsEnrlRqst(ibusPAEnrollReqLife);
                    SetUpWSSPersonAccountLifeOption(ibusPAEnrollReqLife);                    
                    break;
                case "wzsInsPayMethod":
                    AchDetailInSuranceSameAsDepositeInfo();
                    SetUpAchDetail(icdoWssBenAppAchDetailInSurance);
                    break;
                case "wzsSumE-Sig":
                    iblnIsSaveAndContinueClicked = (!string.IsNullOrEmpty(iobjPassInfo.istrPostBackControlID) &&
                                            iobjPassInfo.istrPostBackControlID.StartsWith("btnSaveAndNext")) ? true : false;
                    iblnFinishButtonClicked = true;

                    GenerateOTPForRefund();
                    break;
            }
            icdoWssBenApp.last_step_where_user_left = astrWizardStepName;
            istrBenTypeValue = icdoWssBenApp.ben_type_value;
            if (ibusMemberPerson.IsNull()) LoadMemberPerson();
            if (ibusPlan.IsNull()) LoadPlan();
            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            if (ibusPersonEmploymentDtl.IsNull()) LoadPersonEmploymentDetail();
            EvaluateInitialLoadRules();
        }

        public override void ProcessWizardData(utlWizardNavigationEventArgs we, string astrWizardName, string astrWizardStepName)
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund && astrWizardStepName == "wzsSumE-Sig" && iblnIsRefundFinishClicked && istrActivationCode.IsNullOrEmpty())
            {
                we.istrNextStepID = "wzsSumE-Sig";
            }
            if (!string.IsNullOrEmpty(istrLastStepToLoad))
            {
                we.iintCurrentStepIndex = GetNextStepIndex(istrLastStepToLoad);
                we.istrNextStepID = istrLastStepToLoad;
                istrLastStepToLoad = string.Empty;
            }
            base.ProcessWizardData(we, astrWizardName, astrWizardStepName);
        }
        
        public void LoadPreviousStepObjects(string astrCurrentStep)
        {
            //LoadHealthInsLevelOfCoverage();

            SetUpWssBenApp();
            LoadMSSContributionSummaryForRetirementPlans();
            LoadAllAcknowledgement();

            SetUpPrimaryAchDetail();
            SetUpSecondaryAchDetail();
            SetupInsEnrlRqst(ibusPAEnrollReqHealth);
            //Move information on Health enrollment we should display on Medicare Part D step
            //SetupDependentMedicareAndHealth(ibusPAEnrollReqHealth, ibusPAEnrollReqMedicare);
            SetupInsEnrlRqst(ibusPAEnrollReqDental);
            SetupInsEnrlRqst(ibusPAEnrollReqVision);
            SetDepCobraEnltValues();
            SetupInsEnrlRqst(ibusPAEnrollReqMedicare);

            //Move enter Medicare information on Part D step, it should populate on the Health step
            //SetupDependentMedicareAndHealth(ibusPAEnrollReqMedicare, ibusPAEnrollReqHealth);

            //SetupFlexInsEnrlRqst();
            //SetupInsEnrlRqst(ibusPAEnrollReqLife);
            //SetUpWSSPersonAccountLifeOption(ibusPAEnrollReqLife);
            //SetUpAchDetail(icdoWssBenAppAchDetailInSurance);
        }
        public int GetNextStepIndex(string astrCurrentStep)
        {
            int lntCurrentStepIndex = 0;
            //string lstrNextstep = string.Empty;
            switch (astrCurrentStep)
            {

                case "wzsSelectBenefitType":
                    lntCurrentStepIndex = 0;
                    break;
                case "wzsDistribution":
                    lntCurrentStepIndex = 1;
                    ////validateOtherDisabilityBenefit();
                    //LoadMSSContributionSummaryForRetirementPlans();
                    //LoadAllAcknowledgement();
                    break;
                case "wzsTaxWithholding":
                    lntCurrentStepIndex = 2;
                    //SetUpPrimaryAchDetail();
                    //SetUpSecondaryAchDetail();
                    ////PopulateDescriptions(icdoWssBenAppRolloverDetail);
                    break;
                case "wzsRHIC":
                    lntCurrentStepIndex = 3;
                    break;
                case "wzsSickLeaveConversion":
                    lntCurrentStepIndex = 4;
                    break;
                case "wzsHealth":
                    lntCurrentStepIndex = 5;
                    ////PopulateDescriptions(icdoWssBenAppTaxWithholdingFederal, icdoWssBenApp.ben_type_value != busConstant.ApplicationBenefitTypeRefund);
                    ////PopulateDescriptions(icdoWssBenAppTaxWithholdingState, icdoWssBenApp.ben_type_value != busConstant.ApplicationBenefitTypeRefund);
                    break;
                case "wzsDental":
                    lntCurrentStepIndex = 6;
                    //SetupInsEnrlRqst(ibusPAEnrollReqHealth);
                    ////Move information on Health enrollment we should display on Medicare Part D step
                    //SetupDependentMedicareAndHealth(ibusPAEnrollReqHealth, ibusPAEnrollReqMedicare);
                    break;
                case "wzsVision":
                    lntCurrentStepIndex = 7;
                    //SetupInsEnrlRqst(ibusPAEnrollReqDental);
                    break;
                
                case "wzsDependents":
                    lntCurrentStepIndex = 8;
                    //SetupInsEnrlRqst(ibusPAEnrollReqMedicare);
                    ////Move enter Medicare information on Part D step, it should populate on the Health step
                    //SetupDependentMedicareAndHealth(ibusPAEnrollReqMedicare, ibusPAEnrollReqHealth);
                    //SetupFlexInsEnrlRqst();
                    break;
                case "wzsStepLifeInsurance":
                    lntCurrentStepIndex = 9;
                    ////ValidateDependents();
                    break;
                case "wzsInsuranceTwo":
                    lntCurrentStepIndex = 10;
                    //SetupInsEnrlRqst(ibusPAEnrollReqVision);
                    //SetDepCobraEnltValues();
                    ////EvaluateInitialLoadRules();
                    break;
                case "wzsInsPayMethod":
                    lntCurrentStepIndex = 11;
                    ////ValidateLifeInsuranceStep();
                    ////LifePannel
                    //SetupInsEnrlRqst(ibusPAEnrollReqLife);
                    //SetUpWSSPersonAccountLifeOption(ibusPAEnrollReqLife);
                    break;
                case "wzsSumE-Sig":
                    lntCurrentStepIndex = 12;
                    ////AchDetailInSuranceSameAsDepositeInfo();
                    //SetUpAchDetail(icdoWssBenAppAchDetailInSurance);
                    break;
            }
            //if (lstrNextstep != string.Empty)
            //    LoadPreviousStepObjects(lstrNextstep);
            return lntCurrentStepIndex;
        }
        public bool iblnFaildToSendMail { get; set; }
        public bool iblnHealthCovFamilyWithNoDepEnrolled { get; set; }
        public bool iblnDentalCovNotIndividualWithNoDepEnrolled { get; set; }
        public bool iblnVisionCovNotIndividualWithNoDepEnrolled { get; set; }
        public bool iblnIsDepSpouseMemberSingle { get; set; }
        public bool iblnHealthCovFamilyWithNoFamilyDeps { get; set; }
        public bool iblnDentalCovFamilyWithNoFamilyDeps { get; set; }
        public bool iblnVisionCovFamilyWithNoFamilyDeps { get; set; }

        public bool iblnIsStartDateBlankOthrDisBenefit { get; set; }
        public bool iblnIsBenefitAmountBlankOthrDisBenefit { get; set; }
        public bool iblnIsBenefitAmountNegativeOthrDisBenefit { get; set; }
        public bool iblnIsStartDateGreaterThanEndDateOthrDisBenefit { get; set; }

        public bool iblnMemberBirthCertificateDocUploaded { get; set; }
        public bool iblnBeneficiaryBirthCertificateDocUploaded { get; set; }
        public bool iblnMerriageCertificateDocUploaded { get; set; }
        public bool iblnMedicareIDCertificateDocUploaded { get; set; }
        public bool iblnOccupationalDemandDocUploaded { get; set; }
        public bool iblnStatementOfDisabilityDocUploaded { get; set; }
        public Collection<busDocUpload> iclbDocUpload { get; set; }
        public string istrNewEmailAddress { get; set; }
        public Collection<busPersonAccount> iclbMSSRetirementPersonAccount { get; set; }
        public bool iblnIsRetirementAccountAvailable { get; set; }
        public void LoadDocuements()
        {
            if (icdoWssBenApp.person_id > 0)
            {

                iclbDocUpload = GetCollection<busDocUpload>(Select("cdoDocUpload.LoadDocuments", new object[0] { })).
                    Where<busDocUpload>(iclbFilterDocUpload => iclbFilterDocUpload.icdoDocUpload.person_id == icdoWssBenApp.person_id).ToList().ToCollection();                
            }
        }
        public void FindUploadedDocuements()
        {
            if(iclbDocUpload == null)
                LoadDocuements();
            if (iclbDocUpload != null)
            {
                foreach (busDocUpload lobjDocUpload in iclbDocUpload)
                {
                    switch (Convert.ToInt32(lobjDocUpload.icdoDocUpload.istrUploadedDocumentCode))
                    {
                        case busConstant.DocumetCodeMemberBirthCertificate:
                            iblnMemberBirthCertificateDocUploaded = true;
                            break;
                        case busConstant.DocumetCodeBeneficiaryBirthCertificate:
                            iblnBeneficiaryBirthCertificateDocUploaded = true;
                            break;
                        case busConstant.DocumetCodeMarriageCertificate:
                            iblnMerriageCertificateDocUploaded = true;
                            break;
                        case busConstant.DocumetCodeMedicareIDCardCertificate:
                            iblnMedicareIDCertificateDocUploaded = true;
                            break;
                        case busConstant.DocumetCodeOccupationalDemandCertificate:
                            iblnOccupationalDemandDocUploaded = true;
                            break;
                        case busConstant.DocumetCodeStatementOfDisabilityCertificate:
                            iblnStatementOfDisabilityDocUploaded = true;
                            break;
                    }
                }
            }
        }
        
        //Validate date field in grid.
        //1. Check start date can not be blank if the end date/ benefit amount is entered       
        //2. Check benefit amount can not be blank if the end date/ start date is entered
        //3. Check benefit amount can not be negative if benefit amount is entered
        //4. Check start date can not be greater than end date        
        public void validateOtherDisabilityBenefit()
        {
            iblnIsStartDateBlankOthrDisBenefit = false;
            iblnIsBenefitAmountBlankOthrDisBenefit = false;
            iblnIsBenefitAmountNegativeOthrDisBenefit = false;
            iblnIsStartDateGreaterThanEndDateOthrDisBenefit = false;

            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                if (iclcWssBenAppDisaOtherBenefits == null)
                    LoadWssOtherDisabilityBenefits();

                foreach (busWssBenAppDisaOtherBenefits lobjBenApplotherDisBenefits in iclcWssBenAppDisaOtherBenefits)
                {
                    //1.
                    if ((lobjBenApplotherDisBenefits.icdoWssBenAppDisaOtherBenefits.othr_disa_bene_begin_date == DateTime.MinValue)
                        && ((lobjBenApplotherDisBenefits.icdoWssBenAppDisaOtherBenefits.othr_disa_bene_end_date != DateTime.MinValue)
                        || (lobjBenApplotherDisBenefits.icdoWssBenAppDisaOtherBenefits.othr_disa_mon_benefit_amount != 0.00M)))
                    {
                        iblnIsStartDateBlankOthrDisBenefit = true;
                    }
                    //2.
                    if ((lobjBenApplotherDisBenefits.icdoWssBenAppDisaOtherBenefits.othr_disa_mon_benefit_amount == 0.00M)
                        && ((lobjBenApplotherDisBenefits.icdoWssBenAppDisaOtherBenefits.othr_disa_bene_end_date != DateTime.MinValue)
                        || (lobjBenApplotherDisBenefits.icdoWssBenAppDisaOtherBenefits.othr_disa_bene_begin_date != DateTime.MinValue)))
                    {
                        iblnIsBenefitAmountBlankOthrDisBenefit = true;
                    }
                    //3.
                    if (lobjBenApplotherDisBenefits.icdoWssBenAppDisaOtherBenefits.othr_disa_mon_benefit_amount < 0.00M)
                    {
                        iblnIsBenefitAmountNegativeOthrDisBenefit = true;
                    }
                    //4.
                    if ((lobjBenApplotherDisBenefits.icdoWssBenAppDisaOtherBenefits.othr_disa_bene_begin_date != DateTime.MinValue)
                        && (lobjBenApplotherDisBenefits.icdoWssBenAppDisaOtherBenefits.othr_disa_bene_end_date != DateTime.MinValue)
                        && (lobjBenApplotherDisBenefits.icdoWssBenAppDisaOtherBenefits.othr_disa_bene_begin_date > lobjBenApplotherDisBenefits.icdoWssBenAppDisaOtherBenefits.othr_disa_bene_end_date))
                    {
                        iblnIsStartDateGreaterThanEndDateOthrDisBenefit = true;
                    }
                }
            }
        }

        private void ValidateDependents()
        {
            iblnHealthCovFamilyWithNoDepEnrolled = IsInsCovNotSingleButNoDepEnrolled(iblnIsHealthStepVisible,
                                                        ibusPAEnrollReqHealth, busConstant.HMOLevelOfCoverageSingle,
                                                        dep => dep.icdoWssPersonDependent.is_health_enrolled == busConstant.Flag_No);
            iblnDentalCovNotIndividualWithNoDepEnrolled = IsInsCovNotSingleButNoDepEnrolled(iblnIsDentalStepVisible,
                                                        ibusPAEnrollReqDental, busConstant.DentalLevelofCoverageIndividual,
                                                        dep => dep.icdoWssPersonDependent.is_dental_enrolled == busConstant.Flag_No);
            iblnVisionCovNotIndividualWithNoDepEnrolled = IsInsCovNotSingleButNoDepEnrolled(iblnIsVisionStepVisible,
                                                        ibusPAEnrollReqVision, busConstant.VisionLevelofCoverageIndividual,
                                                        dep => dep.icdoWssPersonDependent.is_vision_enrolled == busConstant.Flag_No);
            iblnPlanOptionWaivedButDepsEnrolled = IsPlanOptionWaivedButDepsEnrolled();
            iblnIsCoverageIndividualButDepsEnrolled = IsCovrageOptionIndividualButDepsEnrolled();
            iblnCovNotIndividualWithInvalidDepsEnrolled = IsCoverageNotIndividualWithInvalidDepsEnrolled();

            IsDepSpouseMemberSingle();
            iblnHealthCovFamilyWithNoFamilyDeps = IsInsCovFamilyWithNoFamilyDeps(iblnIsHealthStepVisible, ibusPAEnrollReqHealth, busConstant.VisionLevelofCoverageFamily,
                dep => dep.icdoWssPersonDependent.is_health_enrolled == busConstant.Flag_Yes);
            iblnDentalCovFamilyWithNoFamilyDeps = IsInsCovFamilyWithNoFamilyDeps(iblnIsDentalStepVisible, ibusPAEnrollReqDental, busConstant.DentalLevelofCoverageFamily,
                dep => dep.icdoWssPersonDependent.is_dental_enrolled == busConstant.Flag_Yes);
            iblnVisionCovFamilyWithNoFamilyDeps = IsInsCovFamilyWithNoFamilyDeps(iblnIsVisionStepVisible, ibusPAEnrollReqVision, busConstant.VisionLevelofCoverageFamily,
                    dep => dep.icdoWssPersonDependent.is_vision_enrolled == busConstant.Flag_Yes);
        }

        private bool IsCoverageNotIndividualWithInvalidDepsEnrolled()
        {
            bool lblnResult = false;
            if(iblnIsDentalStepVisible && 
                ibusPAEnrollReqDental.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
                ibusPAEnrollReqDental.icdoMSSGDHV.level_of_coverage_value != busConstant.DentalLevelofCoverageIndividual && iclbRetireeDeps.Count > 0 && 
                iclbRetireeDeps.Any(dep=>dep.icdoWssPersonDependent.is_dental_enrolled == busConstant.Flag_Yes))
            {
                if(ibusPAEnrollReqDental.icdoMSSGDHV.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualSpouse)
                {
                    if (iclbRetireeDeps.Any(dep => dep.icdoWssPersonDependent.is_dental_enrolled == busConstant.Flag_Yes && 
                        dep.icdoWssPersonDependent.relationship_value != busConstant.DependentRelationshipSpouse))
                        lblnResult = true;
                }
                else if(ibusPAEnrollReqDental.icdoMSSGDHV.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualChild)
                {
                    if (iclbRetireeDeps.Any(dep => dep.icdoWssPersonDependent.is_dental_enrolled == busConstant.Flag_Yes && 
                            dep.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse))
                        lblnResult = true;
                }
            }
            if (iblnIsVisionStepVisible &&
                ibusPAEnrollReqVision.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
                ibusPAEnrollReqVision.icdoMSSGDHV.level_of_coverage_value != busConstant.VisionLevelofCoverageIndividual && iclbRetireeDeps.Count > 0 &&
                iclbRetireeDeps.Any(dep => dep.icdoWssPersonDependent.is_vision_enrolled == busConstant.Flag_Yes))
            {
                if (ibusPAEnrollReqVision.icdoMSSGDHV.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualSpouse)
                {
                    if (iclbRetireeDeps.Any(dep => dep.icdoWssPersonDependent.is_vision_enrolled == busConstant.Flag_Yes && 
                                dep.icdoWssPersonDependent.relationship_value != busConstant.DependentRelationshipSpouse))
                        lblnResult = true;
                }
                else if (ibusPAEnrollReqVision.icdoMSSGDHV.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualChild)
                {
                    if (iclbRetireeDeps.Any(dep => dep.icdoWssPersonDependent.is_vision_enrolled == busConstant.Flag_Yes && 
                            dep.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse))
                        lblnResult = true;
                }
            }
            return lblnResult;
        }

        private bool IsPlanOptionWaivedButDepsEnrolled() => ((ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive &&
    iclbRetireeDeps.Any(dep => dep.icdoWssPersonDependent.is_health_enrolled == busConstant.Flag_Yes)) ||
    (ibusPAEnrollReqDental.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive &&
    iclbRetireeDeps.Any(dep => dep.icdoWssPersonDependent.is_dental_enrolled == busConstant.Flag_Yes)) ||
    (ibusPAEnrollReqVision.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive &&
    iclbRetireeDeps.Any(dep => dep.icdoWssPersonDependent.is_vision_enrolled == busConstant.Flag_Yes)));

        private bool IsCovrageOptionIndividualButDepsEnrolled() => ((ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
    iclbRetireeDeps.Any(dep => dep.icdoWssPersonDependent.is_health_enrolled == busConstant.Flag_Yes) &&
ibusPAEnrollReqHealth.icdoMSSGDHV.level_of_coverage_value == busConstant.HMOLevelOfCoverageSingle) ||
    (ibusPAEnrollReqDental.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
    iclbRetireeDeps.Any(dep => dep.icdoWssPersonDependent.is_dental_enrolled == busConstant.Flag_Yes) &&
ibusPAEnrollReqDental.icdoMSSGDHV.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividual) ||
    (ibusPAEnrollReqVision.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
    iclbRetireeDeps.Any(dep => dep.icdoWssPersonDependent.is_vision_enrolled == busConstant.Flag_Yes) &&
ibusPAEnrollReqVision.icdoMSSGDHV.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividual));

        private bool IsInsCovFamilyWithNoFamilyDeps(bool ablnIsInsStepVisible, busWssPersonAccountEnrollmentRequest abusPAEnrollReq, string astrLevelofCoverage, Func<busWssPersonDependent, bool> aFuncDelegate)
        {
            bool lblnInsCovFamilyWithNoFamilyDeps = false;
            if (ablnIsInsStepVisible && abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
                abusPAEnrollReq.icdoMSSGDHV.level_of_coverage_value == astrLevelofCoverage)
            {
                //For health case Family coverage depedent either spouse or child, for others both needs to enroll.
                if (abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (
                        (iclbRetireeDeps.IsNull() || iclbRetireeDeps.Count < 1 ||
                        (iclbRetireeDeps.Where(aFuncDelegate).Count() < 1))
                    ||
                        !(iclbRetireeDeps.Where(o => o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse).Count() > 0
                            ||
                        iclbRetireeDeps.Where(o => o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipStepChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipAdoptiveChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipGrandChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipDisabledChild).Count() > 0)
                    )
                    {
                        lblnInsCovFamilyWithNoFamilyDeps = true;
                    }
                }
                else if ((iclbRetireeDeps.IsNull() || iclbRetireeDeps.Count <= 1 ||
                    (iclbRetireeDeps.Where(aFuncDelegate).Count() <= 1))
                    || !(iclbRetireeDeps.Where(o => o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse).Count() > 0
                    &&
                    iclbRetireeDeps.Where(o => o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipStepChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipAdoptiveChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipGrandChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipDisabledChild).Count() > 0))
                {
                    lblnInsCovFamilyWithNoFamilyDeps = true;
                }
            }
            return lblnInsCovFamilyWithNoFamilyDeps;
        }

        private void IsDepSpouseMemberSingle()
        {
            if (ibusMemberPerson.icdoPerson.marital_status_value != busConstant.PersonMaritalStatusMarried)
            {
                iblnIsDepSpouseMemberSingle = iclbRetireeDeps.IsNotNull() && iclbRetireeDeps.Count > 0 &&
                                                                        iclbRetireeDeps.Any(dep => (((iblnIsHealthStepVisible && dep.icdoWssPersonDependent.is_health_enrolled == busConstant.Flag_Yes) ||
                                                                        (iblnIsDentalStepVisible && dep.icdoWssPersonDependent.is_dental_enrolled == busConstant.Flag_Yes) ||
                                                                        (iblnIsVisionStepVisible && dep.icdoWssPersonDependent.is_vision_enrolled == busConstant.Flag_Yes)) &&
                                                                        dep.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse));
            }
        }

        private bool IsInsCovNotSingleButNoDepEnrolled(bool ablnInsVisibility, busWssPersonAccountEnrollmentRequest abusPAEnrollReq, string astrLevelOfCoverage, Func<busWssPersonDependent, bool> aFuncCondition)
        {
            bool lblnReturnValue = false;
            if (ablnInsVisibility &&
                    abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
                    !string.IsNullOrEmpty(abusPAEnrollReq.icdoMSSGDHV.level_of_coverage_value) &&
                    abusPAEnrollReq.icdoMSSGDHV.level_of_coverage_value != astrLevelOfCoverage &&
                    iclbRetireeDeps.IsNotNull())
            {
                if (iclbRetireeDeps.Count == 0 || iclbRetireeDeps.All(aFuncCondition))
                    lblnReturnValue = true;
            }
            return lblnReturnValue;
        }

        private bool IsInsLevelOfCoverage(bool ablnInsVisibility, busWssPersonAccountEnrollmentRequest abusPAEnrollReq, string astrLevelOfCoverage)
        {
            bool lblnReturnValue = false;
            if (ablnInsVisibility &&
                    abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
                    !string.IsNullOrEmpty(abusPAEnrollReq.icdoMSSGDHV.level_of_coverage_value) &&
                    abusPAEnrollReq.icdoMSSGDHV.level_of_coverage_value == astrLevelOfCoverage &&
                    iclbRetireeDeps.IsNotNull())
            {
                    lblnReturnValue = true;
            }
            return lblnReturnValue;
        }

        private void SetupFlexInsEnrlRqst()
        {
            ibusPAEnrollReqFlex.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value =
                                                            (iblnIsFlexPanelVisible && icdoWssBenApp.continue_flex_med_spending == busConstant.Flag_Yes) ?
                                                                        busConstant.PlanEnrollmentOptionValueEnroll : (iblnIsFlexPanelVisible &&
                                                                                                                    (String.IsNullOrEmpty(icdoWssBenApp.continue_flex_med_spending) || icdoWssBenApp.continue_flex_med_spending == busConstant.Flag_No))
                                                                                                                    ? busConstant.PlanEnrollmentOptionValueWaive : ibusPAEnrollReqFlex.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value;
            if (iblnIsFlexPanelVisible)
            {
                ibusPAEnrollReqFlex.icdoWssPersonAccountEnrollmentRequest.PopulateDescriptions();
                if (!iarrChangeLog.Any(dbsObj => dbsObj is cdoWssPersonAccountEnrollmentRequest && ((cdoWssPersonAccountEnrollmentRequest)dbsObj).plan_id == busConstant.PlanIdFlex))
                {
                    iarrChangeLog.Add(ibusPAEnrollReqFlex.icdoWssPersonAccountEnrollmentRequest);
                }
            }
        }

        private void SetupInsEnrlRqst(busWssPersonAccountEnrollmentRequest abusPAEnrollReq)
        {
            if (abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll && iclbInsPremACHDetailsAcknowledgement.IsNull())
                LoadInsPremACHDetailsAcknowledgement();
            abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.PopulateDescriptions();
            if (abusPAEnrollReq.icdoMSSGDHV.IsNotNull())
            {
                if (abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth)
                {
                    abusPAEnrollReq.icdoMSSGDHV.coverage_code = abusPAEnrollReq.icdoMSSGDHV.level_of_coverage_value == busConstant.HMOLevelOfCoverageSingle 
                        ? busConstant.CoverageCodeSingleActive : busConstant.CoverageCodeFamilyActive;
                }
                abusPAEnrollReq.icdoMSSGDHV.PopulateDescriptions();
            }
            //LifePannel
            if (abusPAEnrollReq.ibusMSSLifeOption.IsNotNull())
            {
                if(abusPAEnrollReq.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.IsNotNull())
                    abusPAEnrollReq.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.PopulateDescriptions();
            }
        }
        private void SetupDependentMedicareAndHealth(busWssPersonAccountEnrollmentRequest abusPAEnrollReq, busWssPersonAccountEnrollmentRequest abusPAEnrollReqToMove)
        {
            abusPAEnrollReqToMove.icdoMSSGDHV.medicare_claim_no = abusPAEnrollReq.icdoMSSGDHV.medicare_claim_no;
            abusPAEnrollReqToMove.icdoMSSGDHV.medicare_part_a_effective_date = abusPAEnrollReq.icdoMSSGDHV.medicare_part_a_effective_date;
            abusPAEnrollReqToMove.icdoMSSGDHV.medicare_part_b_effective_date = abusPAEnrollReq.icdoMSSGDHV.medicare_part_b_effective_date;
        }
        private void PopulateDescriptions(doBase adoBase, bool ablnPopulateAlways = true)
        {
            if (ablnPopulateAlways)
                adoBase.PopulateDescriptions();
        }

        private void GenerateOTPForRefund()
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund &&
                !string.IsNullOrEmpty(iobjPassInfo.istrPostBackControlID) && iobjPassInfo.istrPostBackControlID.StartsWith("btnNextOTPPanel"))
            {
                if (string.IsNullOrEmpty(istrActivationCode))
                    btnGenerateOTP_Click();
                else
                {
                    btnVertifyOTP_Click();
                }
            }
        }
        private void SetUpSecondaryAchDetail()
        {
            SetUpAchDetail(icdoWssBenAppAchDetailSecondary);
            ChangeSecondaryAchDetialOnCondition();
        }
        private void ChangeSecondaryAchDetialOnCondition()
        {
            if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRolloverPartOfMyRefund || icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRolloverPartOrAllOfMyRefund)
            {
                icdoWssBenAppAchDetailSecondary.routing_no = string.Empty;
                icdoWssBenAppAchDetailSecondary.account_number = string.Empty;
                icdoWssBenAppAchDetailSecondary.bank_account_type_value = string.Empty;
                icdoWssBenAppAchDetailSecondary.bank_account_type_description = string.Empty;
                icdoWssBenAppAchDetailSecondary.bank_name = string.Empty;
                icdoWssBenAppAchDetailSecondary.percentage_of_net_amount = 0.0M;
                icdoWssBenAppAchDetailSecondary.partial_amount = 0.0M;
                //Primary Ach details should be 100% while dist value is RPAR
                icdoWssBenAppAchDetailPrimary.percentage_of_net_amount = 100;
            }
            if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRolloverPartOfMyRefund)
            {
                icdoWssBenAppAchDetailPrimary.routing_no = string.Empty;
                icdoWssBenAppAchDetailPrimary.account_number = string.Empty;
                icdoWssBenAppAchDetailPrimary.bank_account_type_value = string.Empty;
                icdoWssBenAppAchDetailPrimary.bank_account_type_description = string.Empty;
                icdoWssBenAppAchDetailPrimary.bank_name = string.Empty;
                icdoWssBenAppAchDetailPrimary.percentage_of_net_amount = 0.0M;
                icdoWssBenAppAchDetailPrimary.partial_amount = 0.0M;
            }
        }
        private void SetUpAchDetail(cdoWssBenAppAchDetail acdoWssBenAppAchDetail)
        {
            if (!string.IsNullOrEmpty(acdoWssBenAppAchDetail.routing_no))
            {
                busOrganization lbusOrganization = DoesBankExistByRoutingNumber(acdoWssBenAppAchDetail.routing_no);
                //if (string.IsNullOrEmpty(acdoWssBenAppAchDetail.bank_name))       commented this due to bank name could not be updated while doing Next & previous
                //{
                    acdoWssBenAppAchDetail.bank_name = (lbusOrganization.icdoOrganization.org_id > 0) ? lbusOrganization.icdoOrganization.org_name :
                                              !string.IsNullOrEmpty(acdoWssBenAppAchDetail.istrBankName) ? acdoWssBenAppAchDetail.istrBankName : string.Empty;
                //}
                acdoWssBenAppAchDetail.PopulateDescriptions();
            }
            else
            {
                acdoWssBenAppAchDetail.account_number = string.Empty;
                acdoWssBenAppAchDetail.bank_account_type_value = string.Empty;
                acdoWssBenAppAchDetail.bank_account_type_description = string.Empty;
                acdoWssBenAppAchDetail.bank_name = string.Empty;
                acdoWssBenAppAchDetail.percentage_of_net_amount = 0.0M;
                acdoWssBenAppAchDetail.partial_amount = 0.0M;
                //if (string.IsNullOrEmpty(acdoWssBenAppAchDetail.routing_no) && acdoWssBenAppAchDetail.wss_ben_app_id > 0 && acdoWssBenAppAchDetail.ach_type == "D" && acdoWssBenAppAchDetail.primary_account_flag == "N" && string.IsNullOrEmpty(acdoWssBenAppAchDetail.account_number))
                //{
                //    acdoWssBenAppAchDetail.Delete();
                //}
            }
        }
        private void AchDetailInSuranceSameAsDepositeInfo()
        {
            if (chkSameAsDepositeInfo == "Y")
            {
                icdoWssBenAppAchDetailInSurance.routing_no = icdoWssBenAppAchDetailPrimary.routing_no;
                icdoWssBenAppAchDetailInSurance.bank_name = icdoWssBenAppAchDetailPrimary.bank_name;
                icdoWssBenAppAchDetailInSurance.account_number = icdoWssBenAppAchDetailPrimary.account_number;
                icdoWssBenAppAchDetailInSurance.bank_account_type_value = icdoWssBenAppAchDetailPrimary.bank_account_type_value;
                icdoWssBenAppAchDetailInSurance.istrBankName = icdoWssBenAppAchDetailPrimary.istrBankName;
                if (!iarrChangeLog.Contains(icdoWssBenAppAchDetailInSurance))
                {
                    iarrChangeLog.Add(icdoWssBenAppAchDetailInSurance);
                }
            }
        }
        private void SetUpPrimaryAchDetail()
        {
            SetUpAchDetail(icdoWssBenAppAchDetailPrimary);
        }
        public void LoadMSSContributionSummaryForRetirementPlans()
        {
            busMSSPersonAccountRetirementWeb lbusMSSPersonAccountRetirementWeb = new busMSSPersonAccountRetirementWeb();
            lbusMSSPersonAccountRetirementWeb.ibusMSSContributionSummaryForRetirementPlans = new busPersonAccountRetirement { icdoPersonAccountRetirement = new cdoPersonAccountRetirement() };
            lbusMSSPersonAccountRetirementWeb.ibusPerson = ibusMemberPerson;
            lbusMSSPersonAccountRetirementWeb.FindPersonAccount(ibusPersonAccount.icdoPersonAccount.person_account_id);
            lbusMSSPersonAccountRetirementWeb.LoadMSSContributionSummaryForRetirementPlans();
            TotalMemberAccountBalance = lbusMSSPersonAccountRetirementWeb.TotalMemberAccountBalance;
            TotalTaxableAmount = lbusMSSPersonAccountRetirementWeb.TotalTaxableAmount;
            TotalNonTaxableAmount = lbusMSSPersonAccountRetirementWeb.TotalNonTaxableAmount;
        }
        private void SetUpWssBenApp()
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                SetRefundDefaults();
                ResetDeferralValues();
            }
            else if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (icdoWssBenApp.plan_id == busConstant.PlanIdDC || icdoWssBenApp.plan_id == busConstant.PlanIdDC2020 || icdoWssBenApp.plan_id == busConstant.PlanIdDC2025) icdoWssBenApp.ben_opt_value = busConstant.BenefitOptionPeriodicPayment;// PIR 25920
                SetRetirementDate();
                if (icdoWssBenApp.is_deferred == busConstant.Flag_Yes)
                {
                    if (icdoWssBenApp.istrDeferralValue == busConstant.DefferalDateChoiceOptionOther)
                    {
                        DateTime ldteDefEffecDate = DateTime.MinValue;
                        string[] formats = { "MMyyyy", "MM/yyyy" };
                        if (!string.IsNullOrEmpty(icdoWssBenApp.istrDefferalDate) && DateTime.TryParseExact(icdoWssBenApp.istrDefferalDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,out ldteDefEffecDate))
                        {
                            icdoWssBenApp.deferred_date = ldteDefEffecDate;
                            icdoWssBenApp.retirement_date = icdoWssBenApp.deferred_date; // check again
                            icdoWssBenApp.istrRetirementDate = icdoWssBenApp.istrDefferalDate;
                        }
                    }
                    icdoWssBenApp.rhic_option_value = string.Empty;
                    icdoWssBenApp.sick_leave_purchase_indicated_flag = string.Empty;
                }
                else
                    ResetDeferralValues();
                ResetRefundValues();
                LoadRetAppForNRDAndSubTypeAndEligibility();
                if (icdoWssBenApp.is_deferred == busConstant.Flag_Yes && icdoWssBenApp.istrDeferralValue == busConstant.DefferalDateChoiceOptionNormalRetirementDate)
                {
                    icdoWssBenApp.deferred_date = icdoWssBenApp.normal_retr_date;
                    icdoWssBenApp.retirement_date = icdoWssBenApp.normal_retr_date;
                    icdoWssBenApp.istrRetirementDate = icdoWssBenApp.retirement_date.ToString("MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                }
            }
            else if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                if (icdoWssBenApp.plan_id == busConstant.PlanIdDC || icdoWssBenApp.plan_id == busConstant.PlanIdDC2020 || icdoWssBenApp.plan_id == busConstant.PlanIdDC2025) icdoWssBenApp.ben_opt_value = busConstant.BenefitOptionPeriodicPayment; //PIR 25920
                SetRetirementDate();
                ResetRefundValues();
                ResetDeferralValues();
                PopulateDescriptions(icdoWssBenAppDisaMilitaryService);
                LoadRetAppForNRDAndSubTypeAndEligibility();
                SetUpDisaOtherBenefits();
            }
            else
            {
                if (string.IsNullOrEmpty(icdoWssBenApp.ben_type_value) && !string.IsNullOrEmpty(icdoWssBenApp.ben_opt_value))
                {
                    icdoWssBenApp.ben_opt_value = string.Empty;
                }
            }
            SetInsStepsAndPanelsVisibilityAndLoadEnrollmentObjects(DateTime.Today);
            SetDepCobraEnltValues();            
            PopulateDescriptions(icdoWssBenApp);
        }

        private void SetRetirementDate()
        {
            DateTime ldteRetEffecDate = DateTime.MinValue;
            string[] formats = { "MMyyyy", "MM/yyyy" };
            if (!string.IsNullOrEmpty(icdoWssBenApp.istrRetirementDate) && DateTime.TryParseExact(icdoWssBenApp.istrRetirementDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,out ldteRetEffecDate))
            {
                icdoWssBenApp.retirement_date = ldteRetEffecDate;
            }
        }

        private void SetUpDisaOtherBenefits()
        {
            if (iclcWssBenAppDisaOtherBenefits.IsNotNull() && iclcWssBenAppDisaOtherBenefits.Count > 0)
            {
                if (icdoWssBenApp.is_social_security_applied == busConstant.Flag_Yes)
                {
                    busWssBenAppDisaOtherBenefits lbusWssBenAppDisaOtherBenefits = iclcWssBenAppDisaOtherBenefits.FirstOrDefault(Ob => Ob.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_value == busConstant.OtherDisBenSocialSecurityDisBen);
                    lbusWssBenAppDisaOtherBenefits.icdoWssBenAppDisaOtherBenefits.is_social_sec_or_workcomp_benefit_applied = busConstant.Flag_Yes;
                }
                if (icdoWssBenApp.is_worker_comp_benefit_applied == busConstant.Flag_Yes)
                {
                    busWssBenAppDisaOtherBenefits lbusWssBenAppDisaOtherBenefits = iclcWssBenAppDisaOtherBenefits.FirstOrDefault(Ob => Ob.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_value == busConstant.OtherDisBenWSI);
                    lbusWssBenAppDisaOtherBenefits.icdoWssBenAppDisaOtherBenefits.is_social_sec_or_workcomp_benefit_applied = busConstant.Flag_Yes;
                }
                iclcWssBenAppDisaOtherBenefits.ForEach(disa => disa.icdoWssBenAppDisaOtherBenefits.othr_disa_freq_description = busGlobalFunctions.GetDescriptionByCodeValue(disa.icdoWssBenAppDisaOtherBenefits.othr_disa_freq_id, disa.icdoWssBenAppDisaOtherBenefits.othr_disa_freq_value, iobjPassInfo));
                iclcWssBenAppDisaOtherBenefits.ForEach(disa => disa.PopulateDescriptions());
            }
        }

        private void ResetRefundValues()
        {
            icdoWssBenApp.payment_date = DateTime.MinValue;
            icdoWssBenApp.ref_dist_value = string.Empty;
            icdoWssBenApp.ref_state_tax_not_withhold = string.Empty;
        }

        private void ResetDeferralValues()
        {
            icdoWssBenApp.is_deferred = busConstant.Flag_No;
            icdoWssBenApp.istrDeferralValue = string.Empty;
            icdoWssBenApp.deferred_date = DateTime.MinValue;
        }

        private void SetRefundDefaults()
        {
            icdoWssBenApp.ben_opt_value = busConstant.BenefitOptionRegularRefund;
            icdoWssBenApp.retirement_date = DateTime.MinValue;
            icdoWssBenApp.payment_date = DateTime.Today.AddMonths(2).GetFirstDayofCurrentMonth();
            //icdoWssBenApp.member_age_year_part = 0;
            icdoWssBenApp.istrRetirementDate = string.Empty;
            icdoWssBenApp.rhic_option_value = string.Empty;
            icdoWssBenApp.sick_leave_purchase_indicated_flag = string.Empty;
            icdoWssBenApp.graduated_benefit_option_value = string.Empty;
            icdoWssBenApp.plso_requested_flag = busConstant.Flag_No;
            icdoWssBenApp.rollover_plso_flag = string.Empty;
        }

        public override void ValidateGroupRules(string astrGroupName, utlPageMode aenmPageMode)
        {
            iblnIsSaveAndContinueClicked = (!string.IsNullOrEmpty(iobjPassInfo.istrPostBackControlID) &&
                                            iobjPassInfo.istrPostBackControlID.StartsWith("btnSaveAndNext")) ? true : false;
            iblnIsRefundFinishClicked = (!string.IsNullOrEmpty(iobjPassInfo.istrPostBackControlID) && istrActivationCode.IsNullOrEmpty() &&
                                            iobjPassInfo.istrPostBackControlID.StartsWith("btnNextOTPPanel")) && icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund ? true : false;
            if (!iblnIsSaveAndContinueClicked)
            {
                base.ValidateGroupRules(astrGroupName, aenmPageMode);
                foreach (utlError item in iarrErrors) item.istrErrorID = string.Empty;
            }
            else
            {
                iblnFinishButtonClicked = false;
                if (iarrErrors.IsNull()) iarrErrors = new ArrayList();
            }
        }
        public override void BeforePersistChanges()
        {
            RemoveNonApplicableObjFromChangeLog();
            RemoveNonApplicableInsEnrlObjFromChangeLog();
            if (icdoWssBenApp.wss_ben_app_id == 0)
            {
                icdoWssBenApp.Insert();
            }
            else
            {
                icdoWssBenApp.Update();
            }
            SetUpInsEnrlObjects(ibusPAEnrollReqHealth, busConstant.PlanIdGroupHealth);
            SetUpInsEnrlObjects(ibusPAEnrollReqDental, busConstant.PlanIdDental);
            SetUpInsEnrlObjects(ibusPAEnrollReqVision, busConstant.PlanIdVision);
            //LifePannel
            SetUpInsEnrlObjects(ibusPAEnrollReqLife, busConstant.PlanIdGroupLife);

            SetUpInsEnrlObjects(ibusPAEnrollReqMedicare, busConstant.PlanIdMedicarePartD);
            UpdateEnrlReqStatusBeforeInsertingNew(ibusPAEnrollReqFlex, busConstant.PlanIdFlex);
            SetUpDepdentsByPlan();
            base.BeforePersistChanges();
        }

        private void SetUpDepdentsByPlan()
        {
            if (iarrChangeLog.Any(dobj => (dobj is cdoWssPersonDependent && ((cdoWssPersonDependent)dobj).enroll_req_plan_id == 0)))
            {
                List<doBase> llstDepsToRemove = iarrChangeLog.Where(dobj => (dobj is cdoWssPersonDependent && ((cdoWssPersonDependent)dobj).enroll_req_plan_id == 0)).ToList();
                if (llstDepsToRemove.Count > 0)
                    llstDepsToRemove.ForEach(dobj => iarrChangeLog.Remove(dobj));
            }
            if (iblnIsHealthStepVisible && ibusPAEnrollReqHealth.icdoMSSGDHV.level_of_coverage_value != busConstant.HMOLevelOfCoverageSingle)
                SetUpFinalDepsByPlanRetiree(ibusPAEnrollReqHealth, dep => dep.icdoWssPersonDependent.is_health_enrolled == busConstant.Flag_Yes);
            if (iblnIsDentalStepVisible && ibusPAEnrollReqDental.icdoMSSGDHV.level_of_coverage_value != busConstant.DentalLevelofCoverageIndividual)
                SetUpFinalDepsByPlanRetiree(ibusPAEnrollReqDental, dep => dep.icdoWssPersonDependent.is_dental_enrolled == busConstant.Flag_Yes);
            if (iblnIsVisionStepVisible && ibusPAEnrollReqVision.icdoMSSGDHV.level_of_coverage_value != busConstant.VisionLevelofCoverageIndividual)
                SetUpFinalDepsByPlanRetiree(ibusPAEnrollReqVision, dep => dep.icdoWssPersonDependent.is_vision_enrolled == busConstant.Flag_Yes);
        }
        public bool checkIsDependentHasMedicareInformation()
        {
            return ibusPAEnrollReqHealth.iclbMSSPersonDependent.IsNotNull() && 
                ibusPAEnrollReqHealth.iclbMSSPersonDependent.Any(lbusWssPersonDependent=> !string.IsNullOrEmpty(lbusWssPersonDependent.icdoWssPersonDependent.medicare_claim_no));
        }
        private void SetUpFinalDepsByPlanRetiree(busWssPersonAccountEnrollmentRequest abusPAEnrlRqst, Func<busWssPersonDependent, bool> aFncWssPersonDep)
        {
            if (abusPAEnrlRqst.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll)
            {
                Collection<busWssPersonDependent> lclcWssPersonDeps = iclbRetireeDeps.Where(aFncWssPersonDep).ToList().ToCollection();
                if (abusPAEnrlRqst.icdoWssPersonAccountEnrollmentRequest.plan_id != busConstant.PlanIdGroupHealth)
                {
                    if(abusPAEnrlRqst.iclbMSSPersonDependent.IsNull())
                    abusPAEnrlRqst.LoadDependents();
                }
                foreach (busWssPersonDependent lbusWssPersonDependent in abusPAEnrlRqst.iclbMSSPersonDependent)
                {
                    busWssPersonDependent lbusWssPersDep = lclcWssPersonDeps
                                                            .FirstOrDefault(wssdep => (wssdep.icdoWssPersonDependent.target_person_dependent_id > 0 &&
                                                            lbusWssPersonDependent.icdoWssPersonDependent.target_person_dependent_id > 0 &&
                                                            wssdep.icdoWssPersonDependent.target_person_dependent_id ==
                                                            lbusWssPersonDependent.icdoWssPersonDependent.target_person_dependent_id));
                    //assign changed values if any from new enrollment request
                    if (lbusWssPersDep.IsNotNull())
                    {
                        lbusWssPersonDependent.icdoWssPersonDependent.first_name = lbusWssPersDep.icdoWssPersonDependent.first_name;
                        lbusWssPersonDependent.icdoWssPersonDependent.middle_name = lbusWssPersDep.icdoWssPersonDependent.middle_name;
                        lbusWssPersonDependent.icdoWssPersonDependent.last_name = lbusWssPersDep.icdoWssPersonDependent.last_name;
                        lbusWssPersonDependent.icdoWssPersonDependent.ssn = lbusWssPersDep.icdoWssPersonDependent.ssn;
                        lbusWssPersonDependent.icdoWssPersonDependent.date_of_birth = lbusWssPersDep.icdoWssPersonDependent.date_of_birth;

                        lbusWssPersonDependent.icdoWssPersonDependent.marital_status_id = lbusWssPersDep.icdoWssPersonDependent.marital_status_id;
                        lbusWssPersonDependent.icdoWssPersonDependent.marital_status_value = lbusWssPersDep.icdoWssPersonDependent.marital_status_value;

                        lbusWssPersonDependent.icdoWssPersonDependent.relationship_id = lbusWssPersDep.icdoWssPersonDependent.relationship_id;
                        lbusWssPersonDependent.icdoWssPersonDependent.relationship_value = lbusWssPersDep.icdoWssPersonDependent.relationship_value;

                        lbusWssPersonDependent.icdoWssPersonDependent.gender_id = lbusWssPersDep.icdoWssPersonDependent.marital_status_id;
                        lbusWssPersonDependent.icdoWssPersonDependent.gender_value = lbusWssPersDep.icdoWssPersonDependent.gender_value;

                        lbusWssPersonDependent.icdoWssPersonDependent.medicare_claim_no = lbusWssPersDep.icdoWssPersonDependent.medicare_claim_no;
                        lbusWssPersonDependent.icdoWssPersonDependent.medicare_part_a_effective_date = lbusWssPersDep.icdoWssPersonDependent.medicare_part_a_effective_date;
                        lbusWssPersonDependent.icdoWssPersonDependent.medicare_part_b_effective_date = lbusWssPersDep.icdoWssPersonDependent.medicare_part_b_effective_date;
                    }
                    lbusWssPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value = (lbusWssPersDep.IsNotNull()) ?
                                                                                                            busConstant.PlanOptionStatusValueEnrolled :
                                                                                                            busConstant.PlanOptionStatusValueWaived;
                    lbusWssPersonDependent.icdoWssPersonDependent.effective_end_date = DateTime.MinValue;
                    lbusWssPersonDependent.icdoWssPersonDependent.wss_person_account_enrollment_request_id = abusPAEnrlRqst.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                    lbusWssPersonDependent.icdoWssPersonDependent.ienuObjectState = (lbusWssPersonDependent.icdoWssPersonDependent.wss_person_dependent_id > 0) ? ObjectState.Update :
                                                                                    ObjectState.Insert;
                    if (!iarrChangeLog.Contains(lbusWssPersonDependent.icdoWssPersonDependent))
                        iarrChangeLog.Add(lbusWssPersonDependent.icdoWssPersonDependent);
                }
                //Saving newly added dependents only on finish click
                if (iblnFinishButtonClicked && lclcWssPersonDeps.Any(dep => dep.icdoWssPersonDependent.target_person_dependent_id == 0))
                {
                    Collection<busWssPersonDependent> lclcNewlyAddedDeps =
                        lclcWssPersonDeps.Where(dep => dep.icdoWssPersonDependent.target_person_dependent_id == 0).ToList().ToCollection();
                    foreach (busWssPersonDependent lbusWssPersonDependent in lclcNewlyAddedDeps)
                    {
                        lbusWssPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value = busConstant.PlanOptionStatusValueEnrolled;
                        lbusWssPersonDependent.icdoWssPersonDependent.effective_start_date = (icdoWssBenApp.retirement_date != DateTime.MinValue) ?
                                                                                                icdoWssBenApp.retirement_date.AddMonths(1) :
                                                                                                DateTime.Today.GetFirstDayofCurrentMonth().AddMonths(2);
                        lbusWssPersonDependent.icdoWssPersonDependent.wss_person_account_enrollment_request_id = abusPAEnrlRqst.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                        lbusWssPersonDependent.icdoWssPersonDependent.ienuObjectState = ObjectState.Insert;
                        iarrChangeLog.Add(lbusWssPersonDependent.icdoWssPersonDependent);
                    }
                }
            }
        }
        /// <summary>
        /// These scenarios may be rare, but we do need to 
        /// handle these.
        /// </summary>
        private void RemoveNonApplicableInsEnrlObjFromChangeLog()
        {
            if (iarrChangeLog.Count > 0)
            {
                //Primary ACH detail is mandatory, so if primary ACH detail's percentage_of_net_amount is 100, no need to save secondary detail 
                //even if entered routing number
                if (!string.IsNullOrEmpty(icdoWssBenAppAchDetailPrimary.routing_no) && icdoWssBenAppAchDetailPrimary.percentage_of_net_amount == 100 &&
                    iarrChangeLog.Any(dataobject => (dataobject is cdoWssBenAppAchDetail
                                                    && ((cdoWssBenAppAchDetail)dataobject).primary_account_flag != busConstant.Flag_Yes
                                                    && ((cdoWssBenAppAchDetail)dataobject).ach_type == busConstant.DepositAchType)))
                {
                    Func<doBase, bool> lfuncRef = dataobject => (dataobject is cdoWssBenAppAchDetail &&
                                                                    ((cdoWssBenAppAchDetail)dataobject).primary_account_flag != busConstant.Flag_Yes &&
                                                                    ((cdoWssBenAppAchDetail)dataobject).ach_type == busConstant.DepositAchType);
                    if (iarrChangeLog.FirstOrDefault(lfuncRef).IsNotNull())
                        iarrChangeLog.Remove(iarrChangeLog.FirstOrDefault(lfuncRef));
                }
                //the other coverage details with policy start date as DateTime.MinValue are removed from changelog since they are nonnullable fields
                //keeping save and continue functionality in mind as discussed with Maik rather than putting validation on screen while entering.
                if (iarrChangeLog.Any(dobj => dobj is cdoWssPersonAccountOtherCoverageDetail))
                {
                    List<cdoWssPersonAccountOtherCoverageDetail> llstOtherDetails = iarrChangeLog.OfType<cdoWssPersonAccountOtherCoverageDetail>().ToList();
                    llstOtherDetails.ForEach(lcdoWssPAOthrCovDetail =>
                    {
                        if (lcdoWssPAOthrCovDetail.policy_start_date == DateTime.MinValue)
                            iarrChangeLog.Remove(lcdoWssPAOthrCovDetail);
                    });
                }
                //the Worker Compensation details with injury date as DateTime.MinValue are removed from changelog since they have nonnullable fields
                //keeping save and continue functionality in mind as discussed with Maik rather than putting validation on screen while entering.
                if (iarrChangeLog.Any(dobj => dobj is cdoWssPersonAccountWorkerCompensation))
                {
                    List<cdoWssPersonAccountWorkerCompensation> llstWorkComp = iarrChangeLog.OfType<cdoWssPersonAccountWorkerCompensation>().ToList();
                    llstWorkComp.ForEach(lcdoWssPAWorkComp =>
                    {
                        if (lcdoWssPAWorkComp.injury_date == DateTime.MinValue)
                            iarrChangeLog.Remove(lcdoWssPAWorkComp);
                    });
                }

                if (iarrChangeLog.Any(dobj => dobj is cdoPersonAccountLifeOption))
                {
                    List<cdoPersonAccountLifeOption> llstLifeOption = iarrChangeLog.OfType<cdoPersonAccountLifeOption>().ToList();
                    llstLifeOption.ForEach(lcdoPersonAccountLifeOption =>
                    {
                        iarrChangeLog.Remove(lcdoPersonAccountLifeOption);
                    });
                }
            }
        }

        /// <summary>
        /// //When member goes back and forth in the wizard, changing 
        /// the benefit types in which case some nonapplicable dataobjects might 
        /// //get added to iarrChangeLog, removing them here. 
        /// </summary>
        private void RemoveNonApplicableObjFromChangeLog()
        {
            if (icdoWssBenApp.ben_type_value != busConstant.ApplicationBenefitTypeDisability && iarrChangeLog.Count > 0)
            {
                //in new mode or update mode, though rare the user can change benefit types, so if
                //ben type is not disability, the disability info need to be removed from change log
                RemoveCollectionFromChangeLog(typeof(cdoWssBenAppDisaOtherBenefits));
                RemoveFromChangeLog(typeof(cdoWssBenAppDisaSicknessOrInjury));
                RemoveFromChangeLog(typeof(cdoWssBenAppDisaEducation));
                RemoveFromChangeLog(typeof(cdoWssBenAppDisaMilitaryService));
                RemoveCollectionFromChangeLog(typeof(cdoWssBenAppDisaWorkHistory));

                if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund)
                {
                    //in new mode or update mode, when ben type is refund, since for refund, the fed tax and state taxes
                    //are withheld by default, we do not store any tax withholding info for refund benefit type,
                    //if changelog contains withholding info for ben type refund, removing here.
                    //RemoveCollectionFromChangeLog(typeof(cdoWssBenAppTaxWithholding));
                    //when ben type is refund and the member opts to refund whole amount directly to him/her,
                    //we do not need rollover detail info, removing it if it is there in changelog, we need rollover detail,
                    //only when member opts to rollover part or all of the refund, i.e., when ref_dist_value = RefundDistributionRolloverPartOrAllOfMyRefund
                    if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRefundToMeDirectly)
                        RemoveFromChangeLog(typeof(cdoWssBenAppRolloverDetail));
                }
                else if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement)
                {
                    if (icdoWssBenApp.is_deferred == busConstant.Flag_Yes)
                    {
                        //in new mode or update mode, when the member chooses to defer his application, 
                        //then the business rule is that we do not capture primary or secondary deposit ach detail,
                        //tax withholding, or rollover detail, but we want withdrawl ACH detail for insurance premium if 
                        // at all member chooses to enroll in any insurance plan except flex.
                        RemoveCollectionFromChangeLog(typeof(cdoWssBenAppAchDetail));
                        RemoveCollectionFromChangeLog(typeof(cdoWssBenAppTaxWithholding));
                        RemoveFromChangeLog(typeof(cdoWssBenAppRolloverDetail));
                    }
                    if (icdoWssBenApp.sub_type_value != busConstant.ApplicationBenefitSubTypeNormal ||
                        icdoWssBenApp.plso_requested_flag != busConstant.Flag_Yes ||
                        icdoWssBenApp.rollover_plso_flag != busConstant.Flag_Yes)
                    {
                        //in new mode or update mode, since the member can play around with retirement dates on the first step, 
                        //though these cases are rare, the rollover detail might get added to changelog even if it is not applicable, 
                        //so removing here. 
                        RemoveFromChangeLog(typeof(cdoWssBenAppRolloverDetail));
                    }
                }
            }
        }
        public override int PersistChanges()
        {
            SetObjectState();
            return base.PersistChanges();
        }

        private void SetObjectState()
        {
            if (iarrChangeLog.Count > 0)
            {
                foreach (var item in this.iarrChangeLog)
                {
                    doBase ldoBase = (doBase)item;
                    if (ldoBase.iintPrimaryKey == 0 && ldoBase.ienuObjectState != ObjectState.Insert)
                        ldoBase.ienuObjectState = ObjectState.Insert;
                }
            }
        }
        public override void AfterPersistChanges()
        {
            if (iblnFinishButtonClicked)
            {
                if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund)
                {
                    busBenefitApplication lbjBenefitApplication = SetUpBenefitApplication();
                    if (lbjBenefitApplication is busBenefitRefundApplication)
                    {
                        busBenefitRefundApplication lobjBenefitApplication = (busBenefitRefundApplication)lbjBenefitApplication;
                        InitiateAppSaveProcess(lobjBenefitApplication);
                    }
                }
                else
                {
                    busBenefitApplication lBenefitApplication = SetUpBenefitApplication();
                    if (lBenefitApplication is busRetirementDisabilityApplication)
                    {
                        busRetirementDisabilityApplication lobjBenefitApplication = (busRetirementDisabilityApplication)lBenefitApplication;
                        lobjBenefitApplication.SetQDROFlag();
                        lobjBenefitApplication.LoadDeferralDate();
                        if (lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement && icdoWssBenApp.is_deferred == busConstant.Flag_Yes)
                        {
                            lobjBenefitApplication.btnDeferredClicked();
                            if (lobjBenefitApplication.iarrErrors.Count > 0)
                            {
                                string lstrMsg = "Your Application Cannot Be Submitted. Please Contact NDPERS. ";
                                foreach (utlError lutlErrorList in lobjBenefitApplication.iarrErrors)
                                {
                                    if (lutlErrorList.istrErrorMessage.IsNotNull())
                                        lstrMsg += lutlErrorList.istrErrorMessage;
                                }
                                lstrMsg = lstrMsg.Length > 2000 ? lstrMsg.Substring(0, 2000) : lstrMsg;

                                busWSSHelper.PublishMSSMessage(0, 0, string.Format(lstrMsg, ibusPlan.icdoPlan.mss_plan_name),
                                                                         busConstant.WSS_MessageBoard_Priority_High, ibusMemberPerson.icdoPerson.person_id);
                            }
                            else
                            {
                                InitiateWorkFlow(busConstant.Map_MSS_Process_Deferred_Retirement_Application, lobjBenefitApplication.icdoBenefitApplication.benefit_application_id);
                                UpdateWssBenAppStatusToComplete(lobjBenefitApplication.icdoBenefitApplication.benefit_application_id);
                            }
                        }
                        else
                        {
                            InitiateAppSaveProcess(lobjBenefitApplication);
                        }
                    }
                }

                //Insurance plan enrollment work flow initiation only when enrolled.
                string lstrHealthConfirmationText = string.Empty;
                if (iblnIsHealthEnrltAsCobra) lstrHealthConfirmationText += istrHPConfirmationText;
                if (ibusPAEnrollReqHealth.icdoMSSGDHV.is_dependent_medicare_eligible == busConstant.Flag_Yes) lstrHealthConfirmationText += "<br/>" + istrHPRTConfirmationText;

                InitiateInsEnrlMentWorkFlw(ibusPAEnrollReqHealth, iblnIsHealthStepVisible, busConstant.HealthPlanAcknowledgement, lstrHealthConfirmationText);

                InitiateInsEnrlMentWorkFlw(ibusPAEnrollReqDental, iblnIsDentalStepVisible, busConstant.DentalPlanAcknowledgement, istrDPConfirmationText);
                InitiateInsEnrlMentWorkFlw(ibusPAEnrollReqVision, iblnIsVisionStepVisible, busConstant.VisionPlanAcknowledgement, istrVPConfirmationText);
                //LifePannel
                InitiateInsEnrlMentWorkFlw(ibusPAEnrollReqLife, iblnIsLifePanelVisible, busConstant.LifePlanAcknowledgement, istrLPConfirmationText);

                bool lblnIsMedicareValid = iblnIsHealthEnrltAsRetiree && ibusPAEnrollReqHealth.IsNotNull() && ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0 &&
                    ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollmentStatusPending &&
                    ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll;

                InitiateInsEnrlMentWorkFlw(ibusPAEnrollReqMedicare, lblnIsMedicareValid, busConstant.MedicarePlanAcknowledgement, istrMPConfirmationText);
                InitiateInsEnrlMentWorkFlw(ibusPAEnrollReqFlex, iblnIsFlexPanelVisible, busConstant.FlexPlanAcknowledgement, istrFPConfirmationText);
            }
            /* There is only a 60-day window from the date of the employment termination within which 
             * the member has to enroll in cobra, so let us say, the user falls within that window when 
             * he/she first uses the wizard and let us say he/she enrolls in some plans and chooses to Save and Continue
             * the application, and let us say, by the time he/she landed up on the wizard second time, he/she became ineligible to enroll 
             * in insurance plans as cobra, so we need to delete already saved enrollment requests, that is what is being done here
             * * */
            try
            {
                if (iblnFinishButtonClicked)
                {
                    DeleteInsRequestIfNotEligibleToEnroll(ibusPAEnrollReqHealth, busConstant.PlanIdGroupHealth, iblnIsHealthStepVisible);
                    DeleteInsRequestIfNotEligibleToEnroll(ibusPAEnrollReqDental, busConstant.PlanIdDental, iblnIsDentalStepVisible);
                    DeleteInsRequestIfNotEligibleToEnroll(ibusPAEnrollReqVision, busConstant.PlanIdVision, iblnIsVisionStepVisible);
                    //LifePannel
                    DeleteInsRequestIfNotEligibleToEnroll(ibusPAEnrollReqLife, busConstant.PlanIdGroupLife, iblnIsLifePanelVisible);

                    DeleteInsRequestIfNotEligibleToEnroll(ibusPAEnrollReqFlex, busConstant.PlanIdFlex, iblnIsFlexPanelVisible);
                    DeleteDisaInfoIfBenTypeNotDisability();
                    DeleteTaxWithHoldingIfBenTypeRefund();
                    DeleteACHRollTaxWithDetailsForIfBenTypeRefund();
                    DeleteRolloverIfBenTypeNotNormalRtmt();
                    DeleteACHRollTaxWithDetailsForDefRtmt();
                }
            }
            catch (Exception ex)
            {
                NameValueCollection lnvCollection = new NameValueCollection();
                lnvCollection.Add("WssBenAppId", Convert.ToString(icdoWssBenApp.wss_ben_app_id));
                ExceptionManager.Publish(ex, lnvCollection);
            }
            if (iobjPassInfo.istrSenderID == "btnNextTaxWithholding" && icdoWssBenAppTaxWithholdingFederal.refund_fed_percent <= 20)
            {
                icdoWssBenAppTaxWithholdingFederal.refund_fed_percent = busConstant.RefundFedPercent;
            }
            base.AfterPersistChanges();
        }
        private void InitiateInsEnrlMentWorkFlw(busWssPersonAccountEnrollmentRequest abusPAEnrollReq, bool ablnIsInsStepVisible, string constantValue, string confirmationText)
        {
            if (ablnIsInsStepVisible && abusPAEnrollReq.IsNotNull() && abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0 &&
                    abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollmentStatusPending &&
                    abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
                    abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.wss_ben_app_id > 0)
            {
                /*  1.	Retirement & Disability – New WFL process – Enroll Retiree Insurance Plans – Includes all insurance plans entered when member 
                 *      completed Retirement or Disability app (even COBRA for retiree)
                    2.	Refund, Deferred – New WFL process – Enroll COBRA Insurance Plans – 
                        Includes all insurance plans entered when member selects benefit type other than Retirement or Disability.
                 */
                int lintProcessId = 0;
                switch (abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.plan_id)
                {
                    case busConstant.PlanIdGroupHealth:
                        lintProcessId = IsBenTypeRefundOrRetirementDeferred() ? busConstant.Map_MSS_Enroll_COBRA_Insurance_Plans : busConstant.Map_Enroll_Retiree_Insurance_Plans;
                        break;
                    case busConstant.PlanIdDental:
                        lintProcessId = IsBenTypeRefundOrRetirementDeferred() ? busConstant.Map_MSS_Enroll_COBRA_Insurance_Plans : busConstant.Map_Enroll_Retiree_Insurance_Plans;
                        break;
                    case busConstant.PlanIdVision:
                        lintProcessId = IsBenTypeRefundOrRetirementDeferred() ? busConstant.Map_MSS_Enroll_COBRA_Insurance_Plans : busConstant.Map_Enroll_Retiree_Insurance_Plans;
                        break;
                    //LifePannel
                    case busConstant.PlanIdGroupLife:
                        lintProcessId = busConstant.Map_Enroll_Retiree_Insurance_Plans;
                        ////Insurance plan enrollment ack; insert ack text collection only when enrolled in life.
                        //abusPAEnrollReq.InsertCollection(iclbLifeAcknowledgementText);
                        break;
                    case busConstant.PlanIdMedicarePartD:
                        lintProcessId = busConstant.Map_Enroll_Retiree_Insurance_Plans;
                        break;
                    case busConstant.PlanIdFlex:
                        lintProcessId = busConstant.Map_MSS_Enroll_COBRA_Insurance_Plans;
                        break;
                    default:
                        break;
                }
                ////Insurance plan enrollment ack; insert ack text only when enrolled.
                abusPAEnrollReq.InsertString(constantValue, confirmationText);

                if (!abusPAEnrollReq.IsWorkflowAlreadyExistForPerson(lintProcessId))
                {
                    abusPAEnrollReq.InitializeWorkflow(lintProcessId);
                }
            }
        }

        private void DeleteRolloverIfBenTypeNotNormalRtmt()
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeDisability && icdoWssBenApp.wss_ben_app_id > 0 && icdoWssBenAppRolloverDetail.wss_ben_app_rollover_detail_id > 0)
            {
                icdoWssBenAppRolloverDetail.Delete();
            }
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement && icdoWssBenApp.wss_ben_app_id > 0 && icdoWssBenAppRolloverDetail.wss_ben_app_rollover_detail_id > 0)
            {
                if (icdoWssBenApp.sub_type_value != busConstant.ApplicationBenefitSubTypeNormal || icdoWssBenApp.rollover_plso_flag != busConstant.Flag_Yes ||
                    icdoWssBenApp.plso_requested_flag != busConstant.Flag_Yes)
                {
                    icdoWssBenAppRolloverDetail.Delete();
                }
            }
        }

        private void DeleteTaxWithHoldingIfBenTypeRefund()
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund && icdoWssBenApp.wss_ben_app_id > 0)
            {
                if (icdoWssBenAppTaxWithholdingFederal.wss_ben_app_tax_withholding_id > 0)
                    icdoWssBenAppTaxWithholdingFederal.Delete();
                if (icdoWssBenAppTaxWithholdingState.wss_ben_app_tax_withholding_id > 0)
                    icdoWssBenAppTaxWithholdingState.Delete();
                if (    (icdoWssBenApp.ref_dist_value != busConstant.RefundDistributionRolloverPartOrAllOfMyRefund 
                        && icdoWssBenApp.ref_dist_value != busConstant.RefundDistributionRolloverPartOfMyRefund)
                    && icdoWssBenAppRolloverDetail.wss_ben_app_rollover_detail_id > 0)
                    icdoWssBenAppRolloverDetail.Delete();
            }
        }
        private void DeleteACHRollTaxWithDetailsForIfBenTypeRefund()
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund && icdoWssBenApp.wss_ben_app_id > 0)
            {
                if(icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRolloverPartOfMyRefund)
                { 
                    if (icdoWssBenAppAchDetailPrimary.wss_ben_app_ach_detail_id > 0)
                        icdoWssBenAppAchDetailPrimary.Delete();
                    if (icdoWssBenAppAchDetailSecondary.wss_ben_app_ach_detail_id > 0)
                        icdoWssBenAppAchDetailSecondary.Delete();
                    if (string.IsNullOrEmpty(icdoWssBenAppAchDetailSecondary.routing_no) && string.IsNullOrEmpty(icdoWssBenAppAchDetailSecondary.account_number)
                        && icdoWssBenAppAchDetailSecondary.ach_type == "D" && icdoWssBenAppAchDetailSecondary.primary_account_flag == "N"
                        && icdoWssBenAppAchDetailSecondary.wss_ben_app_id > 0)
                    {
                        icdoWssBenAppAchDetailSecondary.Delete();
                    }
                }
                //if ref dist value Rollover part or all of my refund no need to secondary ACH info and primary percentage should be 100
                if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRolloverPartOrAllOfMyRefund)
                {
                    if (icdoWssBenAppAchDetailSecondary.wss_ben_app_ach_detail_id > 0)
                        icdoWssBenAppAchDetailSecondary.Delete();
                    if (icdoWssBenAppAchDetailPrimary.wss_ben_app_ach_detail_id > 0 && icdoWssBenAppAchDetailPrimary.percentage_of_net_amount < 100)
                        icdoWssBenAppAchDetailPrimary.percentage_of_net_amount = 100;
                }
            }
        }
        private void DeleteDisaInfoIfBenTypeNotDisability()
        {
            if (icdoWssBenApp.ben_type_value != busConstant.ApplicationBenefitTypeDisability && icdoWssBenApp.wss_ben_app_id > 0)
            {
                if (icdoWssBenAppDisaEducation.disa_education_id > 0)
                    icdoWssBenAppDisaEducation.Delete();
                if (icdoWssBenAppDisaMilitaryService.disa_military_service_id > 0)
                    icdoWssBenAppDisaMilitaryService.Delete();
                if (icdoWssBenAppDisaSicknessOrInjury.disa_sickness_or_injury_id > 0)
                    icdoWssBenAppDisaSicknessOrInjury.Delete();
                List<busWssBenAppDisaOtherBenefits> llstWssBenAppOtherBens = iclcWssBenAppDisaOtherBenefits.Where(other => other.icdoWssBenAppDisaOtherBenefits.disa_othr_benefit_id > 0).ToList();
                if (llstWssBenAppOtherBens.Count > 0)
                    llstWssBenAppOtherBens.ForEach(other => other.icdoWssBenAppDisaOtherBenefits.Delete());
                List<busWssBenAppDisaWorkHistory> llstWssBenAppWorkHistory = iclcWssBenAppDisaWorkHistory.Where(workhis => workhis.icdoWssBenAppDisaWorkHistory.disa_work_history_id > 0).ToList();
                if (llstWssBenAppWorkHistory.Count > 0)
                    llstWssBenAppWorkHistory.ForEach(workhis => workhis.icdoWssBenAppDisaWorkHistory.Delete());
            }
        }

        private void DeleteACHRollTaxWithDetailsForDefRtmt()
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (icdoWssBenApp.is_deferred == busConstant.Flag_Yes && icdoWssBenApp.wss_ben_app_id > 0)
                {
                    if (icdoWssBenAppAchDetailPrimary.wss_ben_app_ach_detail_id > 0)
                        icdoWssBenAppAchDetailPrimary.Delete();
                    if (icdoWssBenAppAchDetailSecondary.wss_ben_app_ach_detail_id > 0)
                        icdoWssBenAppAchDetailSecondary.Delete();
                    if (icdoWssBenAppRolloverDetail.wss_ben_app_rollover_detail_id > 0)
                        icdoWssBenAppRolloverDetail.Delete();
                    if (icdoWssBenAppTaxWithholdingFederal.wss_ben_app_tax_withholding_id > 0)
                        icdoWssBenAppTaxWithholdingFederal.Delete();
                    if (icdoWssBenAppTaxWithholdingState.wss_ben_app_tax_withholding_id > 0)
                        icdoWssBenAppTaxWithholdingState.Delete();
                }
                if (string.IsNullOrEmpty(icdoWssBenAppAchDetailSecondary.routing_no) && string.IsNullOrEmpty(icdoWssBenAppAchDetailSecondary.account_number)
                    && icdoWssBenAppAchDetailSecondary.ach_type == "D" && icdoWssBenAppAchDetailSecondary.primary_account_flag == "N"
                    && icdoWssBenAppAchDetailSecondary.wss_ben_app_id > 0)
                {
                    icdoWssBenAppAchDetailSecondary.Delete();
                }
            }
        }
        #endregion

        #region Private Methods
        private void UpdateEnrlReqStatusBeforeInsertingNew(busWssPersonAccountEnrollmentRequest abusWssPersonAccountEnrollmentRequest, int aintPlanId)
        {
            if (iblnFinishButtonClicked)
            {
                //As long as user chooses to save and continue the WSS Ben App, the insurance enrollment requests will stay in finish later status,
                //but after finish button is clicked, the insurance enrollment requests' status need to be changed to PEND if plan enrollment option value 
                // is enroll or to posted if plan enrollment option value is waived.
                if (abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value
                    == busConstant.PlanEnrollmentOptionValueEnroll)
                {
                    abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.StatusPending;
                    abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag = busConstant.Flag_No;
                    if (iarrChangeLog.IsNotNull() && !iarrChangeLog.Any(dobj => dobj is cdoWssPersonAccountEnrollmentRequest && ((cdoWssPersonAccountEnrollmentRequest)dobj).plan_id == aintPlanId))
                    {
                        iarrChangeLog.Add(abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest);
                        if (abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
                            abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Update;
                        else
                            abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Insert;
                    }

                }
                else if (abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value
                    == busConstant.PlanEnrollmentOptionValueWaive)
                {
                    abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                    abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag = busConstant.Flag_Yes;
                    if (iarrChangeLog.IsNotNull() && !iarrChangeLog.Any(dobj => dobj is cdoWssPersonAccountEnrollmentRequest && ((cdoWssPersonAccountEnrollmentRequest)dobj).plan_id == aintPlanId))
                    {
                        iarrChangeLog.Add(abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest);
                        if (abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
                            abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Update;
                        else
                            abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Insert;
                    }
                }
            }
            if (iarrChangeLog.Count > 0 &&
                iarrChangeLog.Any(dobj => (dobj is cdoWssPersonAccountEnrollmentRequest && ((cdoWssPersonAccountEnrollmentRequest)dobj).plan_id == aintPlanId))
                && abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id == 0)
            {
                abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_ben_app_id = icdoWssBenApp.wss_ben_app_id;
                abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.Insert();
            }
        }
        private void SetUpInsEnrlObjects(busWssPersonAccountEnrollmentRequest abusWssPersonAccountEnrollmentRequest, int aintPlanId)
        {
            UpdateEnrlReqStatusBeforeInsertingNew(abusWssPersonAccountEnrollmentRequest, aintPlanId);
            //LifePannel
            if (abusWssPersonAccountEnrollmentRequest.icdoMSSGDHV.IsNotNull() && abusWssPersonAccountEnrollmentRequest.icdoMSSGDHV.wss_person_account_ghdv_id == 0)
            {
                abusWssPersonAccountEnrollmentRequest.icdoMSSGDHV.wss_person_account_enrollment_request_id = abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
            }
            if (abusWssPersonAccountEnrollmentRequest.iclbMSSOtherCoverageDetail.IsNotNull() && abusWssPersonAccountEnrollmentRequest.iclbMSSOtherCoverageDetail.Count > 0)
            {
                abusWssPersonAccountEnrollmentRequest.iclbMSSOtherCoverageDetail.ForEach(
                lbusWssPersonAccountOtherCoverageDetail =>
                {
                    if (lbusWssPersonAccountOtherCoverageDetail.icdoWssPersonAccountOtherCoverageDetail.wss_person_account_enrollment_request_id == 0)
                    {
                        lbusWssPersonAccountOtherCoverageDetail.icdoWssPersonAccountOtherCoverageDetail.wss_person_account_enrollment_request_id =
                            abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                    }
                    if (!string.IsNullOrEmpty(lbusWssPersonAccountOtherCoverageDetail.icdoWssPersonAccountOtherCoverageDetail.provider_org_name))
                    {
                        lbusWssPersonAccountOtherCoverageDetail.icdoWssPersonAccountOtherCoverageDetail.provider_org_id =
                            abusWssPersonAccountEnrollmentRequest
                            .LoadProviderOrgID(lbusWssPersonAccountOtherCoverageDetail.icdoWssPersonAccountOtherCoverageDetail.provider_org_name);
                    }
                    lbusWssPersonAccountOtherCoverageDetail.icdoWssPersonAccountOtherCoverageDetail.enroll_req_plan_id = aintPlanId;
                });
            }
            if (abusWssPersonAccountEnrollmentRequest.iclbMSSWorkerCompensation.IsNotNull() && abusWssPersonAccountEnrollmentRequest.iclbMSSWorkerCompensation.Count > 0)
            {
                abusWssPersonAccountEnrollmentRequest.iclbMSSWorkerCompensation.ForEach(lbusWssPersonAccountWorkerCompensation =>
                    {
                        if (lbusWssPersonAccountWorkerCompensation.icdoWssPersonAccountWorkerCompensation.wss_person_account_enrollment_request_id == 0)
                        {
                            lbusWssPersonAccountWorkerCompensation.icdoWssPersonAccountWorkerCompensation.wss_person_account_enrollment_request_id =
                                abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                        }
                        if (!string.IsNullOrEmpty(lbusWssPersonAccountWorkerCompensation.icdoWssPersonAccountWorkerCompensation.company_name))
                        {
                            lbusWssPersonAccountWorkerCompensation.icdoWssPersonAccountWorkerCompensation.provider_org_id =
                                abusWssPersonAccountEnrollmentRequest
                                .LoadProviderOrgID(lbusWssPersonAccountWorkerCompensation.icdoWssPersonAccountWorkerCompensation.company_name);
                        }
                        lbusWssPersonAccountWorkerCompensation.icdoWssPersonAccountWorkerCompensation.enroll_req_plan_id = aintPlanId;
                    });
            }
            if (aintPlanId == busConstant.PlanIdGroupHealth)
            {
                if (abusWssPersonAccountEnrollmentRequest.iclbMSSPersonDependent.IsNotNull() && abusWssPersonAccountEnrollmentRequest.iclbMSSPersonDependent.Count > 0)
                {
                    abusWssPersonAccountEnrollmentRequest.iclbMSSPersonDependent.ForEach(
                    lbusWssPersonDependent =>
                    {
                        if (lbusWssPersonDependent.icdoWssPersonDependent.wss_person_account_enrollment_request_id == 0)
                        {
                            lbusWssPersonDependent.icdoWssPersonDependent.wss_person_account_enrollment_request_id =
                                abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                        }
                        lbusWssPersonDependent.icdoWssPersonDependent.enroll_req_plan_id = aintPlanId;
                        lbusWssPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value = busConstant.PlanOptionStatusValueWaived;
                    });
                }
            }
            //LifePannel
            if (aintPlanId == busConstant.PlanIdGroupLife)
            {
                if (abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
                {
                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.wss_person_account_enrollment_request_id = abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                    if (iblnIsLifePanelVisible == true && abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll)
                    {
                        if (abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.wss_person_account_life_option_id == 0)
                            abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.Insert();
                        else
                            abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.Update();
                    }
                }
            }
            //First, the member opts to enroll and fills info and then opts to waive it though rare in which case few non-applicable objects gets added to change log,
            //removing here.
            if (abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive)
            {
                if (iarrChangeLog.Count > 0)
                {
                    if (iarrChangeLog.Any(dobj => (dobj is cdoWssPersonAccountGhdv && ((cdoWssPersonAccountGhdv)dobj).request_plan_id == aintPlanId)))
                    {
                        if (iarrChangeLog.FirstOrDefault(dobj => (dobj is cdoWssPersonAccountGhdv && ((cdoWssPersonAccountGhdv)dobj).request_plan_id == aintPlanId))
                            .IsNotNull())
                        {
                            iarrChangeLog.Remove(iarrChangeLog.FirstOrDefault(dobj => (dobj is cdoWssPersonAccountGhdv && ((cdoWssPersonAccountGhdv)dobj).request_plan_id == aintPlanId)));
                        }
                    }
                    if (iarrChangeLog.Any(dobj => (dobj is cdoWssPersonAccountOtherCoverageDetail && ((cdoWssPersonAccountOtherCoverageDetail)dobj).enroll_req_plan_id == aintPlanId)))
                    {
                        List<doBase> llstCovDetails = iarrChangeLog.Where(dobj => (dobj is cdoWssPersonAccountOtherCoverageDetail && ((cdoWssPersonAccountOtherCoverageDetail)dobj).enroll_req_plan_id == aintPlanId)).ToList();
                        if (llstCovDetails.Count > 0)
                        {
                            foreach (doBase item in llstCovDetails)
                            {
                                iarrChangeLog.Remove(item);
                            }
                        }
                    }
                    if (iarrChangeLog.Any(dobj => (dobj is cdoWssPersonAccountWorkerCompensation && ((cdoWssPersonAccountWorkerCompensation)dobj).enroll_req_plan_id == aintPlanId)))
                    {
                        List<doBase> llstWorkerComp = iarrChangeLog.Where(dobj => (dobj is cdoWssPersonAccountWorkerCompensation && ((cdoWssPersonAccountWorkerCompensation)dobj).enroll_req_plan_id == aintPlanId)).ToList();
                        if (llstWorkerComp.Count > 0)
                        {
                            foreach (doBase item in llstWorkerComp)
                            {
                                iarrChangeLog.Remove(item);
                            }
                        }
                    }
                    if (aintPlanId == busConstant.PlanIdGroupHealth)
                    {
                        if (iarrChangeLog.Any(dobj => (dobj is cdoWssPersonDependent && ((cdoWssPersonDependent)dobj).enroll_req_plan_id == busConstant.PlanIdGroupHealth)))
                        {
                            List<doBase> llstWssPersonDeps = iarrChangeLog.Where(dobj => (dobj is cdoWssPersonDependent && ((cdoWssPersonDependent)dobj).enroll_req_plan_id == busConstant.PlanIdGroupHealth)).ToList();
                            if (llstWssPersonDeps.IsNotNull() && llstWssPersonDeps.Count > 0)
                            {
                                llstWssPersonDeps.ForEach(dobj => iarrChangeLog.Remove(dobj));
                            }
                        }
                    }
                    if (aintPlanId == busConstant.PlanIdGroupLife)
                    {
                        if (iarrChangeLog.Any(dobj => (dobj is cdoPersonAccountLifeOption && ((cdoPersonAccountLifeOption)dobj).enroll_req_plan_id == busConstant.PlanIdGroupLife)))
                        {
                            List<doBase> llstWssPersonAccountLifeOption = iarrChangeLog.Where(dobj => (dobj is cdoPersonAccountLifeOption && ((cdoPersonAccountLifeOption)dobj).enroll_req_plan_id == busConstant.PlanIdGroupLife)).ToList();
                            if (llstWssPersonAccountLifeOption.IsNotNull() && llstWssPersonAccountLifeOption.Count > 0)
                            {
                                llstWssPersonAccountLifeOption.ForEach(dobj => iarrChangeLog.Remove(dobj));
                            }
                        }
                    }
                }
            }
        }
        private void RemoveFromChangeLog(Type type, int aintPlanId = 0, string astrACHType = null)
        {
            if (aintPlanId == 0)
            {
                if (string.IsNullOrEmpty(astrACHType))
                {
                    if (iarrChangeLog.Count > 0 && iarrChangeLog.Any(dataobject => dataobject.GetType() == type))
                    {
                        if (iarrChangeLog.FirstOrDefault(dataobject => dataobject.GetType() == type).IsNotNull())
                            iarrChangeLog.Remove(iarrChangeLog.FirstOrDefault(dataobject => dataobject.GetType() == type));
                    }
                }
                else
                {
                    if (iarrChangeLog.Count > 0 && iarrChangeLog.Any(dataobject =>
                                                                ((dataobject.GetType() == type) && (((cdoWssBenAppAchDetail)dataobject).ach_type == astrACHType))))
                    {
                        if (iarrChangeLog.FirstOrDefault(dataobject => dataobject.GetType() == type).IsNotNull())
                            iarrChangeLog.Remove(iarrChangeLog.FirstOrDefault(dataobject => dataobject.GetType() == type));
                    }
                }
            }
            else
            {
                if (iarrChangeLog.FirstOrDefault(dataobject => dataobject.GetType() == type && ((cdoWssPersonAccountEnrollmentRequest)dataobject).plan_id == aintPlanId).IsNotNull())
                    iarrChangeLog.Remove(iarrChangeLog.FirstOrDefault(dataobject => dataobject.GetType() == type && ((cdoWssPersonAccountEnrollmentRequest)dataobject).plan_id == aintPlanId));
            }
        }
        private void RemoveCollectionFromChangeLog(Type type)
        {
            if (iarrChangeLog.Count > 0 && iarrChangeLog.Any(dataobject => dataobject.GetType() == type))
            {
                List<doBase> llstToBeRemoved = iarrChangeLog
                                                    .Where(dataobject => ((dataobject.GetType() == type) &&
                                                                         (dataobject.GetType() == typeof(cdoWssBenAppAchDetail) &&
                                                                         (((cdoWssBenAppAchDetail)dataobject).ach_type == busConstant.WithdrawlAchType &&
                                                                         !IsWithdrawlACHDetailValid) ||
                                                                         !((dataobject is cdoWssBenAppAchDetail))) 
                                                                         )).ToList();
                if (llstToBeRemoved.Count() > 0)
                {
                    foreach (var item in llstToBeRemoved)
                        iarrChangeLog.Remove(item);
                }
            }
        }
        private void DeleteInsRequestIfNotEligibleToEnroll(busWssPersonAccountEnrollmentRequest abusWssPerAcntEnrlReq, int aintPlanId, bool ablnPlanVisibility)
        {
            if (icdoWssBenApp.wss_ben_app_id > 0 && !ablnPlanVisibility)
            {
                if (abusWssPerAcntEnrlReq.iclbMSSOtherCoverageDetail.IsNotNull() && abusWssPerAcntEnrlReq.iclbMSSOtherCoverageDetail.Count > 0)
                {
                    List<busWssPersonAccountOtherCoverageDetail> llstWssPAOtherCovDetails = abusWssPerAcntEnrlReq.iclbMSSOtherCoverageDetail.Where(other => other.icdoWssPersonAccountOtherCoverageDetail.wss_person_account_other_coverage_detail_id > 0).ToList();
                    if (llstWssPAOtherCovDetails.Count > 0)
                        llstWssPAOtherCovDetails.ForEach(other => other.icdoWssPersonAccountOtherCoverageDetail.Delete());
                }
                if (abusWssPerAcntEnrlReq.iclbMSSWorkerCompensation.IsNotNull() && abusWssPerAcntEnrlReq.iclbMSSWorkerCompensation.Count > 0)
                {
                    List<busWssPersonAccountWorkerCompensation> llstWssPAWorkComp = abusWssPerAcntEnrlReq.iclbMSSWorkerCompensation.Where(other => other.icdoWssPersonAccountWorkerCompensation.wss_person_account_worker_compensation_id > 0).ToList();
                    if (llstWssPAWorkComp.Count > 0)
                        llstWssPAWorkComp.ForEach(other => other.icdoWssPersonAccountWorkerCompensation.Delete());
                }
                if (abusWssPerAcntEnrlReq.iclbMSSPersonDependent.IsNotNull() && abusWssPerAcntEnrlReq.iclbMSSPersonDependent.Count > 0)
                {
                    List<busWssPersonDependent> llstWssPersonDeps = abusWssPerAcntEnrlReq.iclbMSSPersonDependent.Where(other => other.icdoWssPersonDependent.wss_person_dependent_id > 0).ToList();
                    if (llstWssPersonDeps.Count > 0)
                        llstWssPersonDeps.ForEach(other => other.icdoWssPersonDependent.Delete());
                }

                //LifePannel
                //delete Life step options values if panel is not visible
                if (abusWssPerAcntEnrlReq.ibusMSSLifeOption.IsNotNull())
                {
                    abusWssPerAcntEnrlReq.ibusMSSLifeOption.Delete();
                }

                if (abusWssPerAcntEnrlReq.icdoMSSGDHV.IsNotNull() && abusWssPerAcntEnrlReq.icdoMSSGDHV.wss_person_account_ghdv_id > 0)
                {
                    abusWssPerAcntEnrlReq.icdoMSSGDHV.Delete();
                }
                if (abusWssPerAcntEnrlReq.icdoWssPersonAccountEnrollmentRequest.IsNotNull() && abusWssPerAcntEnrlReq.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
                {
                    abusWssPerAcntEnrlReq.icdoWssPersonAccountEnrollmentRequest.Delete();
                }
            }
            //LifePannel
            if (aintPlanId == busConstant.PlanIdGroupLife && icdoWssBenApp.wss_ben_app_id > 0 && ablnPlanVisibility)
            {
                //delete Life step options values if waived
                if (abusWssPerAcntEnrlReq.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive)
                {
                    if(abusWssPerAcntEnrlReq.ibusMSSLifeOption.IsNotNull())
                        abusWssPerAcntEnrlReq.ibusMSSLifeOption.Delete();
                }
            }
        }

        private void InitiateAppSaveProcess(busBenefitApplication abusBenefitApplication)
        {
            abusBenefitApplication.BeforeValidate(utlPageMode.New);
            abusBenefitApplication.ValidateHardErrors(utlPageMode.New);
            if (abusBenefitApplication.iarrErrors.Count > 0)
            {
                string lstrMsg = "Your Application Cannot Be Submitted. Please Contact NDPERS. ";
                foreach (utlError lutlErrorList in abusBenefitApplication.iarrErrors)
                {
                    if (lutlErrorList.istrErrorMessage.IsNotNull())
                        lstrMsg += lutlErrorList.istrErrorMessage;
                }
                lstrMsg = lstrMsg.Length > 2000 ? lstrMsg.Substring(0, 2000) : lstrMsg;
                busWSSHelper.PublishMSSMessage(0, 0, string.Format(lstrMsg, ibusPlan.icdoPlan.mss_plan_name),
                                                             busConstant.WSS_MessageBoard_Priority_High, ibusMemberPerson.icdoPerson.person_id);
            }
            else
            {
                CheckAndUpdatePreviousDeffApptoCancel();
                abusBenefitApplication.BeforePersistChanges();
                if (abusBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                {
                    abusBenefitApplication.icdoBenefitApplication.termination_date = DateTime.MinValue;
                    abusBenefitApplication.icdoBenefitApplication.payment_date = DateTime.MinValue;
                }
                abusBenefitApplication.PersistChanges();
                abusBenefitApplication.ValidateSoftErrors();
                abusBenefitApplication.UpdateValidateStatus();
                abusBenefitApplication.AfterPersistChanges();
                int lintWorkFlowProcessId = (abusBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) ?
                                                busConstant.Map_MSS_Process_Disability_Application :
                                                (abusBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund) ?
                                                busConstant.Map_MSS_Initialize_Process_Refund_Application_And_Calculation :
                                                (abusBenefitApplication.icdoBenefitApplication.plan_id == busConstant.PlanIdDC) ?
                                                busConstant.Map_MSS_Process_DC_Retirement_Application : (abusBenefitApplication.icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
                                                ? busConstant.Map_MSS_Process_Job_Service_Application : busConstant.Map_MSS_Process_DB_Retirement_Application;

                InitiateWorkFlow(lintWorkFlowProcessId, abusBenefitApplication.icdoBenefitApplication.benefit_application_id);
                UpdateWssBenAppStatusToComplete(abusBenefitApplication.icdoBenefitApplication.benefit_application_id);
				InitiateBankOrgWorkFlow(icdoWssBenAppAchDetailPrimary);
				InitiateBankOrgWorkFlow(icdoWssBenAppAchDetailSecondary);
				InitiateBankOrgWorkFlow(icdoWssBenAppAchDetailInSurance);
            }
        }
        private void InitiateBankOrgWorkFlow(cdoWssBenAppAchDetail acdoWssBenAppAchDetail)
        {
            //checking object is filled with routing number and bank name
            if (acdoWssBenAppAchDetail.routing_no.IsNotNull())
            {
                //checking bank name already exists
                DoesBankExistByRoutingNumber(acdoWssBenAppAchDetail.routing_no);
                if (!iblnIsBankNameEdible)
                {
                    busPayeeAccountAchDetail lbusPayeeAccountAchDetail = new busPayeeAccountAchDetail();
                    //inserting bank name if not exists
                    lbusPayeeAccountAchDetail.InsertOrgBankForRoutingNumber(acdoWssBenAppAchDetail.bank_name, acdoWssBenAppAchDetail.routing_no);
                    lbusPayeeAccountAchDetail.LoadBankOrgByRoutingNumber(acdoWssBenAppAchDetail.routing_no);
                    if (lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_id > 0)
                    {
                        //creating workflow 
                        lbusPayeeAccountAchDetail.InitializeWorkflow(lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_id, busConstant.Map_Process_Create_And_Maintain_Organization_Information);
                    }
                }
            }
        }

        private void CheckAndUpdatePreviousDeffApptoCancel()
        {
            DataTable ldtbPreviousApplicationDeferred = Select("cdoWssPersonAccountEnrollmentRequest.LoadPreviousDeferredApplication", new object[2] { icdoWssBenApp.plan_id, icdoWssBenApp.person_id });
            Collection<busBenefitApplication> lclcPreviousApplicationExists = GetCollection<busBenefitApplication>(ldtbPreviousApplicationDeferred, "icdoBenefitApplication");
            
            if (lclcPreviousApplicationExists.IsNotNull() && lclcPreviousApplicationExists.Count > 0)
            {
                foreach (busBenefitApplication lbusDefferedBenefitApplication in lclcPreviousApplicationExists)
                {
                    if(lbusDefferedBenefitApplication.FindBenefitApplication(lbusDefferedBenefitApplication.icdoBenefitApplication.benefit_application_id))
                    {
                        lbusDefferedBenefitApplication.icdoBenefitApplication.ienuObjectState = ObjectState.Update;
                        lbusDefferedBenefitApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusCancelled;
                        lbusDefferedBenefitApplication.icdoBenefitApplication.Update();
                    }
                }                
            }
        }

        private busBenefitApplication SetUpBenefitApplication()
        {
            busBenefitApplication lbusBenefitApplication = null;
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund)
                lbusBenefitApplication = new busBenefitRefundApplication();
            else
                lbusBenefitApplication = new busRetirementDisabilityApplication();
            lbusBenefitApplication.icdoBenefitApplication = new cdoBenefitApplication
            {
                member_person_id = icdoWssBenApp.person_id,
                plan_id = icdoWssBenApp.plan_id,
                benefit_account_type_value = icdoWssBenApp.ben_type_value,
                action_status_value = busConstant.ApplicationActionStatusPending,
                received_date = DateTime.Today,
                //termination_date = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date,
                termination_date = icdoWssBenApp.termination_date,
                recipient_person_id = icdoWssBenApp.person_id,
                rhic_option_value = String.IsNullOrEmpty(icdoWssBenApp.rhic_option_value) ? busConstant.RHICOptionStandard : icdoWssBenApp.rhic_option_value

            };
            lbusBenefitApplication.idtTerminationDate = lbusBenefitApplication.icdoBenefitApplication.termination_date;            
            if (lbusBenefitApplication.icdoBenefitApplication.benefit_application_id > 0)
                lbusBenefitApplication.icdoBenefitApplication.ienuObjectState = ObjectState.Update;
            else
                lbusBenefitApplication.icdoBenefitApplication.ienuObjectState = ObjectState.Insert;
            lbusBenefitApplication.iarrChangeLog.Add(lbusBenefitApplication.icdoBenefitApplication);
            if (lbusBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                lbusBenefitApplication.icdoBenefitApplication.account_relationship_value = busConstant.AccountRelationshipMember;
                lbusBenefitApplication.icdoBenefitApplication.family_relationship_value = busConstant.AccountRelationshipMember;
                lbusBenefitApplication.icdoBenefitApplication.uniform_income_flag = busConstant.Flag_No;
                lbusBenefitApplication.icdoBenefitApplication.suppress_warnings_flag = busConstant.Flag_No;
                lbusBenefitApplication.icdoBenefitApplication.sick_leave_purchase_indicated_flag = busConstant.Flag_No;
                lbusBenefitApplication.icdoBenefitApplication.benefit_option_value = string.IsNullOrEmpty(icdoWssBenApp.ben_opt_value) ? busConstant.BenefitOptionRegularRefund : icdoWssBenApp.ben_opt_value;
                lbusBenefitApplication.icdoBenefitApplication.payment_date = lbusBenefitApplication.icdoBenefitApplication.received_date.AddMonths(2).GetFirstDayofCurrentMonth();
            }
            else
            {
                lbusBenefitApplication.icdoBenefitApplication.early_reduction_waived_flag = busConstant.Flag_No;
                lbusBenefitApplication.icdoBenefitApplication.benefit_option_value = string.IsNullOrEmpty(icdoWssBenApp.ben_opt_value) ? busConstant.BenefitOptionSingleLife : icdoWssBenApp.ben_opt_value;
                lbusBenefitApplication.icdoBenefitApplication.sick_leave_purchase_indicated_flag = DoesSickLeaveConversionApply() ? icdoWssBenApp.sick_leave_purchase_indicated_flag : busConstant.Flag_No;
                lbusBenefitApplication.iblnIsRTWMember = false;
                lbusBenefitApplication.icdoBenefitApplication.reduced_benefit_flag = busConstant.Flag_No;
                lbusBenefitApplication.icdoBenefitApplication.retirement_date = icdoWssBenApp.retirement_date;
                lbusBenefitApplication.ibusRecipient = ibusMemberPerson;
                lbusBenefitApplication.icdoBenefitApplication.idecTVSC = lbusBenefitApplication.GetRoundedTVSC();
                lbusBenefitApplication.ibusPersonEmploymentDtl = ibusPersonEmploymentDtl;
                lbusBenefitApplication.iclcBenAppOtherDisBenefit = new Collection<cdoBenAppOtherDisBenefit>();
                if (lbusBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                {
                    lbusBenefitApplication.icdoBenefitApplication.plso_requested_flag = icdoWssBenApp.plso_requested_flag;
                    lbusBenefitApplication.icdoBenefitApplication.graduated_benefit_option_value = icdoWssBenApp.graduated_benefit_option_value;
                    if (icdoWssBenApp.is_deferred == busConstant.Flag_Yes)
                    {
                        if (icdoWssBenApp.istrDeferralValue == busConstant.DefferalDateChoiceOptionOther)
                        {
                            lbusBenefitApplication.icdoBenefitApplication.deferral_date = icdoWssBenApp.deferred_date;
                            lbusBenefitApplication.icdoBenefitApplication.retirement_date = icdoWssBenApp.deferred_date;
                        }
                        else
                        {
                            lbusBenefitApplication.icdoBenefitApplication.deferral_date = icdoWssBenApp.normal_retr_date;
                            lbusBenefitApplication.icdoBenefitApplication.retirement_date = icdoWssBenApp.normal_retr_date;
                        }
                    }
                }
                lbusBenefitApplication.SetOrgIdAsLatestEmploymentOrgId();
            }
            lbusBenefitApplication.ibusPerson = ibusMemberPerson;
            lbusBenefitApplication.ibusPersonAccount = ibusPersonAccount;
            lbusBenefitApplication.ibusPlan = ibusPlan;
            lbusBenefitApplication.ibusPersonAccount.ibusPerson = ibusMemberPerson;
            lbusBenefitApplication.ibusPersonAccount.ibusPlan = ibusPlan;
            lbusBenefitApplication.ibusPersonEmploymentDtl = ibusPersonEmploymentDtl;
            return lbusBenefitApplication;
        }

        private void UpdateWssBenAppStatusToComplete(int aBenAppId)
        {
            icdoWssBenApp.bene_appl_id = aBenAppId;
            icdoWssBenApp.ben_action_status_value = busConstant.BenefitApplicationActionStatusCompleted;
            icdoWssBenApp.Update();
        }

        private void InitiateWorkFlow(int aintProcessId, int aBenAppId)
        {
            Dictionary<string, object> ldctParams = new Dictionary<string, object>();
            ldctParams["additional_parameter1"] = icdoWssBenApp.wss_ben_app_id;
            busWorkflowHelper.InitiateBpmRequest(aintProcessId, ibusMemberPerson.icdoPerson.person_id, 0, aBenAppId, iobjPassInfo, busConstant.WorkflowProcessSource_Online, ldctParams);

        }

        public void LoadMSSLifeOptionData(busWssPersonAccountEnrollmentRequest abusWssPersonAccountEnrollmentRequest)
        {
            if (abusWssPersonAccountEnrollmentRequest.IsNotNull())
            {
                if (abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.IsNull())
                    abusWssPersonAccountEnrollmentRequest.LoadMSSLifeOptions();
                if (abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.IsNotNull() && abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.wss_person_account_life_option_id > 0)
                {
                    if (iclbPersonAccountLifeOption.IsNotNull() && iclbPersonAccountLifeOption.Count > 0)
                    {
                        decimal idecBasicCoverageAmountRetiree = 0.0m;
                        foreach (busPersonAccountLifeOption lobjOption in iclbPersonAccountLifeOption)
                        {
                            if (lobjOption.icdoPersonAccountLifeOption.coverage_amount != 0.00M)
                            {
                                switch (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value)
                                {
                                    case busConstant.LevelofCoverage_Basic:
                                        lobjOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount = abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                                        idecBasicCoverageAmountRetiree = lobjOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount;
                                        break;
                                    case busConstant.LevelofCoverage_Supplemental:
                                        //while load supplemental amount it should be addition of basic.
                                        lobjOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount = abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount + idecBasicCoverageAmountRetiree;
                                        break;
                                    case busConstant.LevelofCoverage_DependentSupplemental:
                                        //we set drop down value to NewReducedCoverageAmount for display purpose in Print or maintenance(from LOB) screen.
                                        lobjOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount = Convert.ToDecimal(abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value);
                                        lobjOption.icdoPersonAccountLifeOption.dependent_coverage_option_value = abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value;
                                        break;
                                    case busConstant.LevelofCoverage_SpouseSupplemental:
                                        lobjOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount = abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }
        public void SetUpWSSPersonAccountLifeOption(busWssPersonAccountEnrollmentRequest abusWssPersonAccountEnrollmentRequest)
        {
            if (abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.IsNotNull() && abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll)
            {
                if (ibusMemberPerson.IsNull())
                    LoadMemberPerson();
                if (ibusMemberPerson.IsNotNull())
                    ibusMemberPerson.LoadSpouse();
                if (ibusMemberPerson.ibusSpouse.icdoPerson.person_id != 0)
                {
                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_name = ibusMemberPerson.ibusSpouse.icdoPerson.FullName;
                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_dob = ibusMemberPerson.ibusSpouse.icdoPerson.date_of_birth;
                }
                abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.plan_option_status_id = busConstant.PlanOptionStatusID;
                abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_id = busConstant.CodeIdForDependentAmount;
                //abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.plan_option_status_value = abusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll ? busConstant.PlanOptionStatusValueEnrolled : busConstant.PlanOptionStatusValueWaived;

                if (abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.wss_person_account_life_option_id == 0)
                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.ienuObjectState = ObjectState.Insert;
                else
                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.ienuObjectState = ObjectState.Update;

                //idecBasicCoverageAmount = abusWssPersonAccountEnrollmentRequest.GetCoverageAmountDetails(busConstant.LevelofCoverage_Basic);
                //abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount = idecBasicCoverageAmount;
                if (iclbPersonAccountLifeOption.IsNotNull() && iclbPersonAccountLifeOption.Count > 0)
                {
                    decimal idecNewBasicCoverageAmount = 0.0M;
                    foreach (busPersonAccountLifeOption lobjOption in iclbPersonAccountLifeOption)
                    {
                        if (lobjOption.icdoPersonAccountLifeOption.coverage_amount != 0.00M)
                        {
                            switch (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value)
                            {
                                case busConstant.LevelofCoverage_Basic:
                                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount = lobjOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount;
                                    idecNewBasicCoverageAmount = abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                                    break;
                                case busConstant.LevelofCoverage_Supplemental:
                                    //while storing in DB Supplemental amount should be decrease by basic amount
                                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount = lobjOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount - idecNewBasicCoverageAmount;
                                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag = busConstant.Flag_No;
                                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction = busConstant.Flag_No;
                                    break;
                                case busConstant.LevelofCoverage_DependentSupplemental:
                                    //we set drop down value to NewReducedCoverageAmount for display purpose in Summary and in Print screen.
                                    lobjOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount = Convert.ToDecimal(lobjOption.icdoPersonAccountLifeOption.dependent_coverage_option_value);
                                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value = Convert.ToString(lobjOption.icdoPersonAccountLifeOption.dependent_coverage_option_value);
                                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = busConstant.Flag_No;
                                    break;
                                case busConstant.LevelofCoverage_SpouseSupplemental:
                                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = lobjOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount;
                                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_No;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                }
                if (abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag.IsNullOrEmpty())
                {
                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag = busConstant.Flag_Yes;

                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = busConstant.Flag_Yes;
                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value = string.Empty;
                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;
                    abusWssPersonAccountEnrollmentRequest.ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction = busConstant.Flag_No;
                }
            }
        }
        #endregion

        #region Loading Methods
        public void InitializeDisabilityAndOtherObjects()
        {
            iclcWssBenAppDisaOtherBenefits = new Collection<busWssBenAppDisaOtherBenefits>();
            icdoWssBenAppDisaSicknessOrInjury = new cdoWssBenAppDisaSicknessOrInjury { spcfd_physcn_addr_country_value = busConstant.US_Code_ID };
            icdoWssBenAppDisaEducation = new cdoWssBenAppDisaEducation();
            icdoWssBenAppDisaMilitaryService = new cdoWssBenAppDisaMilitaryService();
            iclcWssBenAppDisaWorkHistory = new Collection<busWssBenAppDisaWorkHistory>();

            icdoWssBenAppRolloverDetail = new cdoWssBenAppRolloverDetail() { addr_country_value = busConstant.US_Code_ID };
            icdoWssBenAppAchDetailPrimary = new cdoWssBenAppAchDetail() { percentage_of_net_amount = 100, primary_account_flag = busConstant.Flag_Yes, ach_type = busConstant.DepositAchType };
            icdoWssBenAppAchDetailSecondary = new cdoWssBenAppAchDetail() { ach_type = busConstant.DepositAchType, primary_account_flag = busConstant.Flag_No };
            icdoWssBenAppAchDetailInSurance = new cdoWssBenAppAchDetail() { ach_type = busConstant.WithdrawlAchType };
            icdoWssBenAppTaxWithholdingFederal = new cdoWssBenAppTaxWithholding()
            {
                tax_identifier_value = busConstant.PayeeAccountTaxIdentifierFedTax,
                benefit_distribution_type_value = busConstant.BenefitDistributionMonthlyBenefit,
                tax_ref = busConstant.PayeeAccountTaxRefFed22Tax
            };
            icdoWssBenAppTaxWithholdingState = new cdoWssBenAppTaxWithholding()
            {
                tax_identifier_value = busConstant.PayeeAccountTaxIdentifierStateTax,
                benefit_distribution_type_value = busConstant.BenefitDistributionMonthlyBenefit,
                tax_ref = busConstant.PayeeAccountTaxRefState22Tax
            };
            InitializeInsEnrlRequests();
        }

        public void InitializeInsEnrlRequests()
        {
            ibusPAEnrollReqHealth = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest()
                {
                    person_id = icdoWssBenApp.person_id,
                    plan_id = busConstant.PlanIdGroupHealth,
                    person_employment_dtl_id = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.person_employment_dtl_id,
                    status_value = busConstant.EnrollRequestStatusFinishLater,
                    reason_value = busConstant.ChangeReasonNewRetiree,
                    enrollment_type_value = busConstant.EnrollmentTypeGHDV,
                },
                icdoMSSGDHV = new cdoWssPersonAccountGhdv
                {
                    type_of_coverage_value = busConstant.InsuranceTypeOfCoveragePPOBasic,
                    request_plan_id = busConstant.PlanIdGroupHealth
                },
                iclbMSSPersonDependent = new Collection<busWssPersonDependent>()
            };
            ibusPAEnrollReqHealth.LoadPersonAccount();
            ibusPAEnrollReqHealth.LoadPersonAccountGHDV();
            ibusPAEnrollReqHealth.LoadPlan();
            ibusPAEnrollReqHealth.icdoMSSGDHV.type_of_coverage_value = ibusPAEnrollReqHealth.ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.alternate_structure_code_value.IsNullOrEmpty() ?
                                                                        busConstant.InsuranceTypeOfCoveragePPOBasic : 
                                                                        ibusPAEnrollReqHealth.ibusMSSPersonAccountGHDV?.icdoPersonAccountGhdv?.alternate_structure_code_value;

            ibusPAEnrollReqDental = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest
                {
                    person_id = icdoWssBenApp.person_id,
                    plan_id = busConstant.PlanIdDental,
                    person_employment_dtl_id = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.person_employment_dtl_id,
                    status_value = busConstant.EnrollRequestStatusFinishLater,
                    reason_value = busConstant.ChangeReasonNewRetiree,
                    enrollment_type_value = busConstant.EnrollmentTypeGHDV,
                },
                icdoMSSGDHV = new cdoWssPersonAccountGhdv
                {
                    request_plan_id = busConstant.PlanIdDental
                }
            };
            ibusPAEnrollReqDental.LoadPlan();
            ibusPAEnrollReqVision = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest
                {
                    person_id = icdoWssBenApp.person_id,
                    plan_id = busConstant.PlanIdVision,
                    person_employment_dtl_id = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.person_employment_dtl_id,
                    status_value = busConstant.EnrollRequestStatusFinishLater,
                    reason_value = busConstant.ChangeReasonNewRetiree,
                    enrollment_type_value = busConstant.EnrollmentTypeGHDV,
                },
                icdoMSSGDHV = new cdoWssPersonAccountGhdv
                {
                    request_plan_id = busConstant.PlanIdVision
                },
            };
            ibusPAEnrollReqVision.LoadPlan();
            //LifePannel
            ibusPAEnrollReqLife = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest
                {
                    person_id = icdoWssBenApp.person_id,
                    plan_id = busConstant.PlanIdGroupLife,
                    person_employment_dtl_id = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.person_employment_dtl_id,
                    status_value = busConstant.EnrollRequestStatusFinishLater,
                    reason_value = busConstant.ChangeReasonNewRetiree,
                    enrollment_type_value = busConstant.EnrollmentTypeLife,                    
                },
                ibusMSSLifeOption = new busWssPersonAccountLifeOption
                {
                    icdoWssPersonAccountLifeOption = new cdoWssPersonAccountLifeOption
                    {
                        //wss_person_account_enrollment_request_id = ibusPAEnrollReqLife.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id,
                        level_of_coverage_id = busConstant.LevelofCoverage_CodeID,
                    }
                }
            };


            ibusPAEnrollReqMedicare = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest
                {
                    person_id = icdoWssBenApp.person_id,
                    plan_id = busConstant.PlanIdMedicarePartD,
                    person_employment_dtl_id = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.person_employment_dtl_id,
                    status_value = busConstant.EnrollRequestStatusFinishLater,
                    reason_value = busConstant.ChangeReasonNewRetiree,
                    enrollment_type_value = busConstant.EnrollmentTypeGHDV,
                },
                icdoMSSGDHV = new cdoWssPersonAccountGhdv
                {
                    request_plan_id = busConstant.PlanIdMedicarePartD
                },
            };
            ibusPAEnrollReqFlex = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest
                {
                    person_id = icdoWssBenApp.person_id,
                    plan_id = busConstant.PlanIdFlex,
                    person_employment_dtl_id = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.person_employment_dtl_id,
                    status_value = busConstant.EnrollRequestStatusFinishLater,
                    reason_value = busConstant.ChangeReasonNewRetiree,
                    enrollment_type_value = busConstant.EnrollmentTypeFlexComp,
                }
            };
            iclbWSSBenAppHealthMSSOtherCoverageDetail = new Collection<busWssPersonAccountOtherCoverageDetail>();
            iclbWSSBenAppDentalMSSOtherCoverageDetail = new Collection<busWssPersonAccountOtherCoverageDetail>();
            iclbWSSBenAppVisionMSSOtherCoverageDetail = new Collection<busWssPersonAccountOtherCoverageDetail>();

            iclbWSSBenAppHealthMSSWorkerCompensation = new Collection<busWssPersonAccountWorkerCompensation>();
            iclbWSSBenAppDentalMSSWorkerCompensation = new Collection<busWssPersonAccountWorkerCompensation>();
            iclbWSSBenAppVisionMSSWorkerCompensation = new Collection<busWssPersonAccountWorkerCompensation>();
        }

        public void LoadPlan()
        {
            ibusPlan = new busPlan();
            ibusPlan.FindPlan(icdoWssBenApp.plan_id);
        }
        public void LoadMemberPerson()
        {
            ibusMemberPerson = new busPerson();
            ibusMemberPerson.FindPerson(icdoWssBenApp.person_id);
            ibusMemberPerson.GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, DateTime.Now);
            PersonAddressId = ibusMemberPerson.ibusPersonCurrentAddress.icdoPersonAddress.person_address_id;
        }
        public void LoadPersonAccount()
        {
            ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            if (ibusMemberPerson.IsNull())
                LoadMemberPerson();
            if (ibusMemberPerson.iclbRetirementAccount.IsNull())
                ibusMemberPerson.LoadRetirementAccount();
            if (ibusMemberPerson.iclbRetirementAccount.FirstOrDefault(pa => pa.icdoPersonAccount.plan_id == icdoWssBenApp.plan_id && pa.IsEnrolledOrSuspended()).IsNotNull())
                ibusPersonAccount = ibusMemberPerson.iclbRetirementAccount.FirstOrDefault(pa => pa.icdoPersonAccount.plan_id == icdoWssBenApp.plan_id && pa.IsEnrolledOrSuspended());
        }
        public void LoadPersonEmploymentDetail()
        {
            if (ibusPersonEmploymentDtl.IsNull())
                ibusPersonEmploymentDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            ibusPersonEmploymentDtl = ibusPersonAccount.GetLatestEmploymentDetail();

            //ibusPersonEmploymentDtl = lbusPAEmpDetail.LoadPersonEmploymentDetail(idtPayPeriodDate, albnLoadMemberType);
        }
        /// <summary>
        /// Loading non-retiree dependents
        /// </summary>
        public void LoadNonRetireeDependents()
        {
            if (iclbNonRetireeDeps.IsNull())
                iclbNonRetireeDeps = new Collection<busWssPersonDependent>();
            if (ibusPersonEmploymentDtl.IsNull()) LoadPersonEmploymentDetail();
            if (icdoWssBenApp.termination_date != DateTime.MinValue)
            {
                DataTable ldtbNonRetDeps = Select("cdoWssBenApp.LoadNonRetireeDependents", new object[2] { icdoWssBenApp.person_id, icdoWssBenApp.termination_date });
                iclbNonRetireeDeps = GetCollection<busWssPersonDependent>(ldtbNonRetDeps, "icdoWssPersonDependent");
            }
        }

        private void LoadDepsUpdate(bool ablnIsRetiree)
        {
            if (icdoWssBenApp.wss_ben_app_id > 0)
            {
                DataTable ldtbDepsAllPlan = null;
                if (icdoWssBenApp.wss_ben_app_id > 0)
                    ldtbDepsAllPlan = Select("cdoWssBenApp.LoadDepsUpdate", new object[1] { icdoWssBenApp.wss_ben_app_id });
                //Update mode/save and continue case
                if (iclbRetireeDeps.Count > 0 && ldtbDepsAllPlan.IsNotNull() && ldtbDepsAllPlan.Rows.Count > 0)
                {
                    foreach (DataRow ldtrRow in ldtbDepsAllPlan.Rows)
                    {
                        int lintPlaId = (ldtrRow["PLAN_ID"] != DBNull.Value) ? Convert.ToInt32(ldtrRow["PLAN_ID"]) : 0;
                        int lintTargetDepId = (ldtrRow[enmWssPersonDependent.target_person_dependent_id.ToString()] != DBNull.Value) ?
                                                Convert.ToInt32(ldtrRow[enmWssPersonDependent.target_person_dependent_id.ToString()]) : 0;
                        string lstrCurPlanStatus = (ldtrRow[enmWssPersonDependent.current_plan_enrollment_option_value.ToString()] != DBNull.Value) ?
                                                                        Convert.ToString(ldtrRow[enmWssPersonDependent.current_plan_enrollment_option_value.ToString()]) : string.Empty;
                        if (lintPlaId > 0 && lintTargetDepId > 0 && !string.IsNullOrEmpty(lstrCurPlanStatus) &&
                            lstrCurPlanStatus == busConstant.PlanOptionStatusValueEnrolled)
                        {
                            busWssPersonDependent lbusWssPersonDependent = iclbRetireeDeps
                                                                            .FirstOrDefault(dep => dep.icdoWssPersonDependent.target_person_dependent_id == lintTargetDepId);
                            switch (lintPlaId)
                            {
                                case busConstant.PlanIdGroupHealth:
                                    if (lbusWssPersonDependent.IsNotNull())
                                    {
                                        lbusWssPersonDependent.icdoWssPersonDependent.is_health_enrolled = busConstant.Flag_Yes;
                                        lbusWssPersonDependent.icdoWssPersonDependent.medicare_claim_no = Convert.ToString(ldtrRow[enmWssPersonDependent.medicare_claim_no.ToString()]);
                                        lbusWssPersonDependent.icdoWssPersonDependent.medicare_part_a_effective_date = (ldtrRow[enmWssPersonDependent.medicare_part_a_effective_date.ToString()] != DBNull.Value) ?
                                            DateTime.Parse(Convert.ToString(ldtrRow[enmWssPersonDependent.medicare_part_a_effective_date.ToString()])) : DateTime.MinValue;
                                        lbusWssPersonDependent.icdoWssPersonDependent.medicare_part_b_effective_date = (ldtrRow[enmWssPersonDependent.medicare_part_a_effective_date.ToString()] != DBNull.Value) ? 
                                            DateTime.Parse(Convert.ToString(ldtrRow[enmWssPersonDependent.medicare_part_b_effective_date.ToString()])) : DateTime.MinValue;
                                    }   
                                    break;
                                case busConstant.PlanIdDental:
                                    if (lbusWssPersonDependent.IsNotNull())
                                        lbusWssPersonDependent.icdoWssPersonDependent.is_dental_enrolled = busConstant.Flag_Yes;
                                    break;
                                case busConstant.PlanIdVision:
                                    if (lbusWssPersonDependent.IsNotNull())
                                        lbusWssPersonDependent.icdoWssPersonDependent.is_vision_enrolled = busConstant.Flag_Yes;
                                    break;
                            }

                        }
                    }
                }
            }
        }

        public void LoadDependents()
        {
            if (ibusPersonEmploymentDtl.ibusPersonEmployment == null)
                ibusPersonEmploymentDtl.LoadPersonEmployment();
            if (iclbRetireeDeps.IsNull())
                iclbRetireeDeps = new Collection<busWssPersonDependent>();
            busWssPersonAccountEnrollmentRequest lbusWssPersonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest
            {
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest
                {
                    person_id = icdoWssBenApp.person_id
                }
            };
            lbusWssPersonAccountEnrollmentRequest.LoadDependents();
            //iclbRetireeDeps = lbusWssPersonAccountEnrollmentRequest.iclbMSSPersonDependent;

            foreach(busPersonDependent lbusPersonDependent in lbusWssPersonAccountEnrollmentRequest.iclbMSSPersonDependent)
            {
                if (lbusPersonDependent.icdoPersonDependent.dependent_date_of_death == DateTime.MinValue)
                {
                    lbusPersonDependent.LoadPersonAccountDependent();
                    if (lbusPersonDependent.iclbPersonAccountDependent.Any(i => i.icdoPersonAccountDependent.end_date == DateTime.MinValue) ||
                        (ibusPersonEmploymentDtl.ibusPersonEmployment.icdoPersonEmployment.end_date != DateTime.MinValue &&
                        lbusPersonDependent.iclbPersonAccountDependent.Any(i => i.icdoPersonAccountDependent.end_date >= DateTime.Now)))
                    {
                        iclbRetireeDeps.Add(lbusPersonDependent);
                    }
                }
            }
            LoadDepsUpdate(true);
        }
        public void LoadTaxRefConfig()
        {
            string lstrTaxRef = busConstant.PayeeAccountTaxRefFed22Tax; // icdoPayeeAccountTaxWithholding.tax_ref;
            string lstrTaxRefIdentifier = busConstant.PayeeAccountTaxIdentifierFedTax;
            //lstrTaxRefIdentifier = busConstant.PayeeAccountTaxIdentifierStateTax;
            lstrTaxRef = busConstant.PayeeAccountTaxRefFed22Tax;
            //lstrTaxRef = busConstant.PayeeAccountTaxRefState22Tax;
            ibusTaxRefConfig = new busTaxRefConfig();
            ibusTaxRefConfig.FindTaxRefConfig(lstrTaxRefIdentifier, lstrTaxRef, busGlobalFunctions.GetSysManagementBatchDate());
            LoadW4PUIElements();
        }
        public void LoadW4PUIElements()
        {
            if (ibusTaxRefConfig.IsNotNull() && ibusTaxRefConfig.icdoTaxRefConfig.IsNotNull())
            {
                icdoWssBenAppTaxWithholdingFederal.three_first_line = ibusTaxRefConfig.icdoTaxRefConfig.three_first_line.IsNull() ? string.Empty : string.Format(ibusTaxRefConfig.icdoTaxRefConfig.three_first_line, string.Format("{0:C}", ibusTaxRefConfig.icdoTaxRefConfig.total_amt_single), string.Format("{0:C}", ibusTaxRefConfig.icdoTaxRefConfig.total_amt_married));
                icdoWssBenAppTaxWithholdingFederal.three_second_line = ibusTaxRefConfig.icdoTaxRefConfig.three_second_line.IsNull() ? string.Empty : string.Format(ibusTaxRefConfig.icdoTaxRefConfig.three_second_line, ibusTaxRefConfig.icdoTaxRefConfig.child_age, string.Format("{0:C}", ibusTaxRefConfig.icdoTaxRefConfig.child_age_by_amt));
                icdoWssBenAppTaxWithholdingFederal.three_third_line = ibusTaxRefConfig.icdoTaxRefConfig.three_third_line.IsNull() ? string.Empty : string.Format(ibusTaxRefConfig.icdoTaxRefConfig.three_third_line, string.Format("{0:C}", ibusTaxRefConfig.icdoTaxRefConfig.other_depd_amt));
                icdoWssBenAppTaxWithholdingFederal.two_tip = ibusTaxRefConfig.icdoTaxRefConfig.two_tip;
            }
        }
        public void LoadInsPremACHDetailsAcknowledgement()
        {
            busWssPersonAccountACHDetail lbusWssPersonAccountACHDetail = new busWssPersonAccountACHDetail();
            lbusWssPersonAccountACHDetail.LoadACHDetailsAcknowledgement();
            if (iclbInsPremACHDetailsAcknowledgement.IsNull())
                iclbInsPremACHDetailsAcknowledgement = new Collection<busWssAcknowledgement>();
            iclbInsPremACHDetailsAcknowledgement = lbusWssPersonAccountACHDetail.iclbACHDetailsAcknowledgement;


            LoadTaxRefConfig();
        }

        public void LoadWssOtherDisabilityBenefits()
        {
            if (iclcWssBenAppDisaOtherBenefits.IsNull())
                iclcWssBenAppDisaOtherBenefits = new Collection<busWssBenAppDisaOtherBenefits>();
            List<utlCodeValue> llstGetOthrDisBen = iobjPassInfo.isrvDBCache.GetCodeValuesFromDict(1911);
            int lintIndex = 1;
            foreach (utlCodeValue lutlCodeValue in llstGetOthrDisBen)
            {
                busWssBenAppDisaOtherBenefits lobjOtherDisabilityBenefit = new busWssBenAppDisaOtherBenefits
                {
                    icdoWssBenAppDisaOtherBenefits =
                    new cdoWssBenAppDisaOtherBenefits()
                };
                lobjOtherDisabilityBenefit.icdoWssBenAppDisaOtherBenefits.disa_othr_benefit_id = lintIndex * -1;
                lobjOtherDisabilityBenefit.icdoWssBenAppDisaOtherBenefits.wss_ben_app_id = icdoWssBenApp.wss_ben_app_id;
                lobjOtherDisabilityBenefit.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_id = lutlCodeValue.code_id;
                lobjOtherDisabilityBenefit.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_value = lutlCodeValue.code_value;
                lobjOtherDisabilityBenefit.icdoWssBenAppDisaOtherBenefits.othr_disa_freq_id = 7017;
                lobjOtherDisabilityBenefit.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_description = busGlobalFunctions.GetData1ByCodeValue(1911,
                                                                                            lobjOtherDisabilityBenefit.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_value, iobjPassInfo);

                busWssBenAppDisaOtherBenefits lbusWssBenAppDisaOtherBenefits = iclcWssBenAppDisaOtherBenefits.FirstOrDefault(other => other.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_value == lobjOtherDisabilityBenefit.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_value);

                if (!iclcWssBenAppDisaOtherBenefits.Any(other => other.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_value == lobjOtherDisabilityBenefit.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_value))
                    iclcWssBenAppDisaOtherBenefits.Add(lobjOtherDisabilityBenefit);
                else
                {
                    lbusWssBenAppDisaOtherBenefits.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_description = lobjOtherDisabilityBenefit.icdoWssBenAppDisaOtherBenefits.othr_disability_benefit_description;
                }
                lintIndex++;
                lobjOtherDisabilityBenefit.PopulateDescriptions();
            }
        }
        public void LoadWssDisaWorkHistory()
        {
            if (iclcWssBenAppDisaWorkHistory.IsNull())
                iclcWssBenAppDisaWorkHistory = new Collection<busWssBenAppDisaWorkHistory>();
            DataTable ldtbdisaWorkHistory = null;
            if (icdoWssBenApp.wss_ben_app_id > 0)
            {
                ldtbdisaWorkHistory = Select<cdoWssBenAppDisaWorkHistory>(new string[1] { enmWssBenApp.wss_ben_app_id.ToString() },
                    new object[1] { icdoWssBenApp.wss_ben_app_id }, null, null);
            }
            if (icdoWssBenApp.wss_ben_app_id > 0 && ldtbdisaWorkHistory.IsNotNull() && ldtbdisaWorkHistory.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbdisaWorkHistory.Rows)
                {
                    busWssBenAppDisaWorkHistory lobjAppDisaWorkHistory = new busWssBenAppDisaWorkHistory { icdoWssBenAppDisaWorkHistory = new cdoWssBenAppDisaWorkHistory() };
                    lobjAppDisaWorkHistory.icdoWssBenAppDisaWorkHistory.LoadData(dr);
                    iclcWssBenAppDisaWorkHistory.Add(lobjAppDisaWorkHistory);
                }
            }
        }

        public void LoadInsEnrlRequests()
        {
            InitializeInsEnrlRequests();
            if (icdoWssBenApp.wss_ben_app_id > 0)
            {
                FindInsEnrlRqstByWssBenAppId(ibusPAEnrollReqHealth, busConstant.PlanIdGroupHealth);
                FindInsEnrlRqstByWssBenAppId(ibusPAEnrollReqDental, busConstant.PlanIdDental);
                FindInsEnrlRqstByWssBenAppId(ibusPAEnrollReqVision, busConstant.PlanIdVision);
                //LifePannel
                FindInsEnrlRqstByWssBenAppId(ibusPAEnrollReqLife, busConstant.PlanIdGroupLife);

                FindInsEnrlRqstByWssBenAppId(ibusPAEnrollReqMedicare, busConstant.PlanIdMedicarePartD);
                FindInsEnrlRqstByWssBenAppId(ibusPAEnrollReqFlex, busConstant.PlanIdFlex);
            }
        }

        private void FindInsEnrlRqstByWssBenAppId(busWssPersonAccountEnrollmentRequest abusPAEnrollReq, int aintPlanId)
        {
            DataTable ldtbInsRequest = Select<cdoWssPersonAccountEnrollmentRequest>(new string[2] { enmWssPersonAccountEnrollmentRequest.wss_ben_app_id.ToString(), enmWssPersonAccountEnrollmentRequest.plan_id.ToString() },
                        new object[2] { icdoWssBenApp.wss_ben_app_id, aintPlanId }, null, "wss_person_account_enrollment_request_id desc");
            if (ldtbInsRequest.Rows.Count > 0)
            {
                abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.LoadData(ldtbInsRequest.Rows[0]);
            }
            if (abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0 && aintPlanId != busConstant.PlanIdFlex && aintPlanId != busConstant.PlanIdGroupLife)
            {
                ldtbInsRequest = Select<cdoWssPersonAccountGhdv>(new string[1] { enmWssPersonAccountGhdv.wss_person_account_enrollment_request_id.ToString() },
                    new object[1] { abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, "wss_person_account_ghdv_id desc");
                if (ldtbInsRequest.Rows.Count > 0)
                    abusPAEnrollReq.icdoMSSGDHV.LoadData(ldtbInsRequest.Rows[0]);
            }
            //LifePannel
            if (abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0 && aintPlanId == busConstant.PlanIdGroupLife)
            {
                abusPAEnrollReq.LoadMSSLifeOptions();
                //LoadMSSLifeOptionData(abusPAEnrollReq);
            }
            //Due to save and continue functionality, the requests may or may not have these values populated always
            abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.person_id = icdoWssBenApp.person_id;
            abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.plan_id = aintPlanId;
            abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.person_employment_dtl_id;
            abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusFinishLater;
            abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.reason_value = IsBenTypeRefundOrRetirementDeferred() ? busConstant.ChangeReasonStart_Of_COBRA_Period : busConstant.ChangeReasonNewRetiree;
            //if (abusPAEnrollReq.icdoMSSGDHV.IsNotNull())
            //    abusPAEnrollReq.icdoMSSGDHV.type_of_coverage_value = (aintPlanId == busConstant.PlanIdGroupHealth) ? busConstant.InsuranceTypeOfCoveragePPOBasic : null;
            //LifePannel
            abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = (aintPlanId == busConstant.PlanIdFlex) ? busConstant.EnrollmentTypeFlexComp : (aintPlanId == busConstant.PlanIdGroupLife) ? busConstant.EnrollmentTypeLife : busConstant.EnrollmentTypeGHDV;
            if (abusPAEnrollReq.icdoMSSGDHV.IsNotNull())
                abusPAEnrollReq.icdoMSSGDHV.request_plan_id = aintPlanId;
        }
        public bool IsBenTypeRefundOrRetirementDeferred()
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund ||
                (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement && icdoWssBenApp.is_deferred == busConstant.Flag_Yes))
                return true;
            return false;            
        }
        public void SetReasonValueForReturnOrDeferred(busWssPersonAccountEnrollmentRequest abusPAEnrollReq)
        {
            abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.reason_value = IsBenTypeRefundOrRetirementDeferred() ? busConstant.ChangeReasonStart_Of_COBRA_Period : busConstant.ChangeReasonNewRetiree;
            abusPAEnrollReq.icdoWssPersonAccountEnrollmentRequest.date_of_change = IsBenTypeRefundOrRetirementDeferred() ? icdoWssBenApp.termination_date.AddMonths(1).GetFirstDayofCurrentMonth() : icdoWssBenApp.retirement_date.AddMonths(1).GetFirstDayofCurrentMonth();
        }
        public void LoadLifePersonAccount()
        {
            if (ibusMemberPerson.IsNull()) LoadMemberPerson();
            if (ibusMemberPerson.icolPersonAccount.IsNull()) ibusMemberPerson.LoadPersonAccount();
            if (iclbPersonAccountLifeOption == null) iclbPersonAccountLifeOption = new Collection<busPersonAccountLifeOption>();
            lbuslifePersonAccount = ibusMemberPerson.icolPersonAccount.FirstOrDefault(pa => pa.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife && !pa.IsCanceled());
            if (lbuslifePersonAccount.IsNotNull())
            {
                lbuslifePersonAccount.LoadPersonAccountLife();
                lbuslifePersonAccount.ibusPersonAccountLife.LoadPersonAccountLifeOptions();
            }
            //Load life acknoledment text
            //iclbLifeAcknowledgementText = new Collection<busWssAcknowledgement>();
            if (ibusPAEnrollReqLife.IsNotNull())
            {
                if (ibusPAEnrollReqLife.ibusPerson.IsNull())
                    ibusPAEnrollReqLife.ibusPerson = ibusMemberPerson;
                //istrConfConfirmationText = ibusPAEnrollReqLife.istrConfirmationText;
                //iclbLifeAcknowledgementText = ibusPAEnrollReqLife.LoadAckCheckDetails();
                //if(ibusPAEnrollReqLife.FindWssPersonAccountEnrollmentRequest(ibusPAEnrollReqLife.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id))                
                //{
                //    ibusPAEnrollReqLife.LoadAckDetailsForView();
                //}
            }
        }        
        #endregion

        #region RetrievalMethods
        public busOrganization DoesBankExistByRoutingNumber(string astrRoutingNumber)
        {
            busOrganization lbusOrganization = new busOrganization();
            lbusOrganization.FindOrganizationByRoutingNumber(astrRoutingNumber);
            iblnIsBankNameEdible = lbusOrganization.icdoOrganization.org_id > 0 ? true : false;
            if(iblnIsBankNameEdible == false)
            {
                lbusOrganization.icdoOrganization.istrRoutingText = "The routing number isn't in MSS. Please review it for accuracy—it may be valid but not recognized in our system.";
            }
            //icdoWssBenAppAchDetailSecondary.istrBankName = (lbusOrganization.icdoOrganization.org_id > 0) ? lbusOrganization.icdoOrganization.org_name : string.Empty;
            return lbusOrganization;
        }
        #endregion
        #region HardErrorMethods

        /// <summary>
        /// Used in Rule ID - GroupRuleDistributionPrimaryBankNameRequired
        /// </summary>
        /// <returns></returns>
        public bool IsBankNameRequired()
        {
            return ((!string.IsNullOrEmpty(icdoWssBenAppAchDetailPrimary.routing_no) &&
                !((new busOrganization()).FindOrganizationByRoutingNumber(icdoWssBenAppAchDetailPrimary.routing_no))) &&
                (string.IsNullOrEmpty(icdoWssBenAppAchDetailPrimary.bank_name)) ||
                (!string.IsNullOrEmpty(icdoWssBenAppAchDetailSecondary.routing_no) &&
                !((new busOrganization()).FindOrganizationByRoutingNumber(icdoWssBenAppAchDetailSecondary.routing_no)) &&
                string.IsNullOrEmpty(icdoWssBenAppAchDetailSecondary.bank_name)));
        }
        public bool IsRetEffecDateValid()
        {
            DateTime ldtResult = DateTime.MinValue;
            string[] formats = { "MMyyyy", "MM/yyyy" };
            if (string.IsNullOrEmpty(icdoWssBenApp.istrRetirementDate)) return true;
            return DateTime.TryParseExact(icdoWssBenApp.istrRetirementDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ldtResult);
        }
        public bool IsDefferalDateValid()
        {
            DateTime ldtResult = DateTime.MinValue;
            string[] formats = { "MMyyyy", "MM/yyyy" };
            return DateTime.TryParseExact(icdoWssBenApp.istrDefferalDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ldtResult);
        }
        public bool DoesBenefitDROApplicationExist()
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                busBenefitRefundApplication lbusBenefitRefundApplication = new busBenefitRefundApplication()
                {
                    icdoBenefitApplication = new cdoBenefitApplication()
                    {
                        member_person_id = icdoWssBenApp.person_id,
                        plan_id = icdoWssBenApp.plan_id
                    }
                };
                return (busConstant.DROApplicationStatusApproved == lbusBenefitRefundApplication.ValidateBenefitDROApplicationExist());
            }
            return false;
        }

        public bool CheckRetDateIsEarlierThanFirstOfFollowingEmploymentEndDate()
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeDisability || icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    if (ibusPersonEmploymentDtl.IsNull())
                        LoadPersonEmploymentDetail();
                    if (ibusPersonEmploymentDtl == null)
                        ibusPersonEmploymentDtl = ibusPersonAccount.GetLatestEmploymentDetail();
                    if (icdoWssBenApp.retirement_date != DateTime.MinValue && ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
                        //&& ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date != DateTime.MinValue)
                    {
                        DateTime ldtFirstOFFollowingEmploymentEndDate = new DateTime(icdoWssBenApp.termination_date.AddMonths(1).Year,
                                                                                    icdoWssBenApp.termination_date.AddMonths(1).Month, 1);
                        if ((icdoWssBenApp.retirement_date < ldtFirstOFFollowingEmploymentEndDate)
                            || icdoWssBenApp.retirement_date < icdoWssBenApp.termination_date)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public void LoadRetAppForNRDAndSubTypeAndEligibility()
        {
            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            if (ibusPersonEmploymentDtl.IsNull()) LoadPersonEmploymentDetail();
            busRetirementDisabilityApplication lbusRetDisApplication = new busRetirementDisabilityApplication
            {
                icdoBenefitApplication = new cdoBenefitApplication
                {
                    benefit_account_type_value = icdoWssBenApp.ben_type_value,
                    benefit_option_value = icdoWssBenApp.ben_opt_value,
                    member_person_id = icdoWssBenApp.person_id,
                    plan_id = icdoWssBenApp.plan_id,
                    retirement_date = icdoWssBenApp.retirement_date,
                    //termination_date = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date
                    termination_date = icdoWssBenApp.termination_date
                }
            };
            lbusRetDisApplication.ibusPersonAccount = ibusPersonAccount;
            lbusRetDisApplication.ibusPersonAccount.ibusPlan = ibusPlan;
            lbusRetDisApplication.ibusPersonAccount.ibusPerson = ibusMemberPerson;
            //if (lbusRetDisApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            icdoWssBenApp.normal_retr_date = lbusRetDisApplication.GetNormalRetirementDateBasedOnNormalEligibility(ibusPlan.icdoPlan.plan_code, true);
            icdoWssBenApp.iblnIsEligibleForRtmt = lbusRetDisApplication.CheckIsPersonEligible();
            icdoWssBenApp.sub_type_value = lbusRetDisApplication.icdoBenefitApplication.benefit_sub_type_value;
        }
        public bool IsDeferralDateLessThanNRD()
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement && icdoWssBenApp.is_deferred == busConstant.Flag_Yes &&
                icdoWssBenApp.istrDeferralValue == busConstant.DefferalDateChoiceOptionOther && icdoWssBenApp.deferred_date != DateTime.MinValue)
            {
                return icdoWssBenApp.normal_retr_date > DateTime.MinValue && icdoWssBenApp.deferred_date < icdoWssBenApp.normal_retr_date;
            }
            return false;
        }
        /// <summary>
        /// The retirement date you entered is more than 6 months in the future, please file your application within 6 months of retirement date or 
        /// defer your application.
        /// </summary>
        /// <returns></returns>
        public bool IsRetrDateSixMonthsInTheFuture()
        {
            DateTime ldteRetDate = DateTime.MinValue;
            string[] formats = { "MMyyyy", "MM/yyyy" };
            return ((icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement) &&
                icdoWssBenApp.is_deferred != busConstant.Flag_Yes &&
                !string.IsNullOrEmpty(icdoWssBenApp.istrRetirementDate) &&
                DateTime.TryParseExact(icdoWssBenApp.istrRetirementDate, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,out ldteRetDate) && ldteRetDate >= DateTime.Today.GetFirstDayofCurrentMonth().AddMonths(6));
        }
        public bool IsFederalPercentLessThanMinimum()
        {
            DateTime ldtPayeeNextPaymentDate = DateTime.MinValue;
            DateTime ldtLastBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate();
            busFedStateFlatTaxRate lbusFedStateFlatTaxRate = busPayeeAccountHelper.LoadFlatTaxRate(ldtLastBenefitPaymentDate.AddMonths(1), busConstant.PayeeAccountTaxIdentifierStateTax, busConstant.PayeeAccountTaxRefState22Tax, busConstant.Flag_No);
                return icdoWssBenAppTaxWithholdingFederal.refund_fed_percent < lbusFedStateFlatTaxRate?.icdoFedStateFlatTaxRate?.min_tax_percentage ? true : false;
        }
        public int ValidateLifeInsuranceStep()
        {
            decimal idecSpouse_Supplemental_Amount = 0.00M;
            decimal idecSupplemental_Amount = 0.00M;
            decimal adecMaxAmount = 0.00M; decimal adecMinAmount = 0.00M; decimal idecMinSupplementalLimit = 0.00M;  //decimal idecBasicCoverageAmount = 0.00M;

            if (ibusPAEnrollReqLife.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll)
            {
                idecSupplemental_Amount = Convert.ToDecimal(iclbPersonAccountLifeOption.Where(option => option.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                                                                            .Select(option => (decimal)option.icdoPersonAccountLifeOption.NewReducedCoverageAmount).SingleOrDefault());
                idecSpouse_Supplemental_Amount = Convert.ToDecimal(iclbPersonAccountLifeOption.Where(option => option.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                                                                            .Select(option => (decimal)option.icdoPersonAccountLifeOption.NewReducedCoverageAmount).SingleOrDefault());
                //idecBasicCoverageAmount = Convert.ToDecimal(iclbPersonAccountLifeOption.Where(option => option.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                //                                                            .Select(option => (decimal)option.icdoPersonAccountLifeOption.NewReducedCoverageAmount).SingleOrDefault());
                //Amount cannot be negative
                if (iclbPersonAccountLifeOption.Any(option => option.icdoPersonAccountLifeOption.NewReducedCoverageAmount < 0))
                    return 5583;
                //Supplemental coverage amount is mandatory.
                if (iclbPersonAccountLifeOption.Any(option => option.icdoPersonAccountLifeOption.NewReducedCoverageAmount <= 0))
                    return 10018;
                // New Reduced amount cannot be greater than Coverage Amount.
                if (iclbPersonAccountLifeOption.Any(option => option.icdoPersonAccountLifeOption.NewReducedCoverageAmount > option.icdoPersonAccountLifeOption.coverage_amount))
                    return 10384;
                // New Reduced amount cannot be greater than Coverage Amount.
                if (iclbPersonAccountLifeOption.Any(option => Convert.ToInt32(option.icdoPersonAccountLifeOption.dependent_coverage_option_value) > option.icdoPersonAccountLifeOption.coverage_amount))
                    return 10384;
                //Supplemental coverage amount should be multiple of $5000.00
                if (idecSupplemental_Amount != 0.00M && idecSupplemental_Amount % 5000M != 0)
                    return 10072;
                //Supplemental Spouse coverage amount should be multiple of $5000.00
                if (idecSpouse_Supplemental_Amount != 0.00M && idecSpouse_Supplemental_Amount % 5000M != 0)
                    return 10073;
                // Spouse Supplement should not exceed 50 % of ‘Member’ supplemental.
                if (idecSupplemental_Amount != 0.00M && idecSpouse_Supplemental_Amount != 0.00M && (idecSupplemental_Amount * 0.5M) < idecSpouse_Supplemental_Amount)
                    return 6578;
                // Supplemental amount must be above {0}.
                if (ibusPAEnrollReqLife.icdoWssPersonAccountEnrollmentRequest.date_of_change != DateTime.MinValue && idecSupplemental_Amount != 0.00M)
                {
                    ibusPAEnrollReqLife.GetCoverageAmountDetailsSupplemental(ref adecMinAmount, ref adecMaxAmount);
                    idecMinSupplementalLimit = adecMinAmount + idecBasicCoverageAmount;
                    if (idecSupplemental_Amount < idecMinSupplementalLimit)
                        return 10386;
                }
            }
            return 0;
        }
        #endregion
        #region VisibleRuleMethods
        /// <summary>
        /// 
        /// </summary>
        public void SetInsStepsAndPanelsVisibilityAndLoadEnrollmentObjects(DateTime TodayOrLastModifiedDate)
        {
            if (ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
            {
                //if in any case if ter date is minValue assign employment end Date 
                if (icdoWssBenApp.termination_date == DateTime.MinValue)
                {
                    icdoWssBenApp.termination_date = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date;
                }
                DateTime ldteEmpEndDate = icdoWssBenApp.termination_date;
                DateTime ldteEmpEndDatePlus60Days = ldteEmpEndDate.AddDays(60);
                DateTime ldteEmpEndDatePlus31Days = ldteEmpEndDate.AddDays(31);
                //Getting member's age at the time of termination.
                if (ibusMemberPerson.IsNull()) LoadMemberPerson();
                iintMemberAge = busGlobalFunctions.CalulateAge(ibusMemberPerson.icdoPerson.date_of_birth, icdoWssBenApp.retirement_date);                
                if (ldteEmpEndDatePlus60Days > TodayOrLastModifiedDate)
                {
                    if (ibusMemberPerson.icolPersonAccount.IsNull()) ibusMemberPerson.LoadPersonAccount();
                    SetCobraInsPlanEligibilityAndLoadEnrlObjects(busConstant.PlanIdGroupHealth);                    
                    SetCobraFlexPlanEligibilityAndLoadEnrlObjects();
                }
                if (ldteEmpEndDatePlus31Days > TodayOrLastModifiedDate)
                {
                    SetCobraInsPlanEligibilityAndLoadEnrlObjects(busConstant.PlanIdDental);
                    SetCobraInsPlanEligibilityAndLoadEnrlObjects(busConstant.PlanIdVision);
                }
                //set the icdoWssPersonAccountEnrollmentRequest.date_of_change value based upon benefit type before loadEnrObjects and life panel visibility
                SetReasonValueForReturnOrDeferred(ibusPAEnrollReqHealth);
                SetReasonValueForReturnOrDeferred(ibusPAEnrollReqDental);
                SetReasonValueForReturnOrDeferred(ibusPAEnrollReqVision);
                SetReasonValueForReturnOrDeferred(ibusPAEnrollReqFlex);
                SetReasonValueForReturnOrDeferred(ibusPAEnrollReqMedicare);
                SetReasonValueForReturnOrDeferred(ibusPAEnrollReqLife);

                if ((icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement && icdoWssBenApp.is_deferred != busConstant.Flag_Yes) ||
                         icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeDisability)
                {

                    iblnIsHealthEnrltAsRetiree = !iblnIsHealthEnrltAsCobra;
                    iblnIsDentalEnrltAsRetiree = !iblnIsDentalEnrltAsCobra;
                    iblnIsVisionEnrltAsRetiree = !iblnIsVisionEnrltAsCobra;
                    //LifePannel This is not at all required
                    //iblnIsLifeEnrltAsRetiree = !iblnIsLifeEnrltAsCobra;
                    DateTime ldteInsReqDateOfChange = icdoWssBenApp.retirement_date.AddMonths(1);
                    if (iblnIsHealthEnrltAsRetiree)
                        LoadInsEnrlObjects(busConstant.PlanIdGroupHealth, ldteInsReqDateOfChange);
                    if (iblnIsDentalEnrltAsRetiree)
                        LoadInsEnrlObjects(busConstant.PlanIdDental, ldteInsReqDateOfChange);
                    if (iblnIsVisionEnrltAsRetiree)
                        LoadInsEnrlObjects(busConstant.PlanIdVision, ldteInsReqDateOfChange);
                    //LifePannel
                    //if (iblnIsLifeEnrltAsRetiree)
                        LoadInsEnrlObjects(busConstant.PlanIdGroupLife, ldteInsReqDateOfChange);

                    //ibusPAEnrollReqMedicare.icdoWssPersonAccountEnrollmentRequest.date_of_change = ldteInsReqDateOfChange;
                }
                if (ldteEmpEndDatePlus31Days > TodayOrLastModifiedDate)
                {
                    SetLifePanelVisibility(ldteEmpEndDatePlus31Days,TodayOrLastModifiedDate);
                    if (iblnIsLifePanelVisible == true)
                    {
                        //LifePannel
                        //SetCobraInsPlanEligibilityAndLoadEnrlObjects(busConstant.PlanIdGroupLife);
                        LoadMSSLifeOptionData(ibusPAEnrollReqLife);
                    }
                }
                
            }
        }

        private void SetCobraFlexPlanEligibilityAndLoadEnrlObjects()
        {
            busPersonAccount lbusFlexPersonAccount = ibusMemberPerson.icolPersonAccount.FirstOrDefault(pa => pa.icdoPersonAccount.plan_id == busConstant.PlanIdFlex &&
                                                                                                    !pa.IsCanceled());
            if (lbusFlexPersonAccount.IsNotNull())
            {
                lbusFlexPersonAccount.LoadPersonAccountFlex();
                lbusFlexPersonAccount.ibusPersonAccountFlex.LoadFlexCompOptionUpdate();
                if (lbusFlexPersonAccount.ibusPersonAccountFlex.iclbFlexCompOption.Count > 0 &&
                    lbusFlexPersonAccount.ibusPersonAccountFlex.iclbFlexCompOption
                    .Any(option => option.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending &&
                                option.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0))
                {

                    busPersonAccountFlexCompOption lbusPersonAccountFlexCompOption = lbusFlexPersonAccount.ibusPersonAccountFlex.iclbFlexCompOption
                                                                                        .FirstOrDefault(option =>
                                                                                        option.icdoPersonAccountFlexCompOption.level_of_coverage_value ==
                                                                                        busConstant.FlexLevelOfCoverageMedicareSpending
                                                                                        && option.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0);
                    if (lbusPersonAccountFlexCompOption.IsNotNull())
                    {
                        busPersonAccountFlexCompHistory lbusPersonAccountFlexCompHistory = lbusFlexPersonAccount.ibusPersonAccountFlex
                                                            .LoadHistoryByDate(lbusPersonAccountFlexCompOption, icdoWssBenApp.termination_date);
                                                            //.LoadHistoryByDate(lbusPersonAccountFlexCompOption, ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date);
                        if (lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.plan_participation_status_value
                            == busConstant.PlanParticipationStatusFlexCompEnrolled &&
                            lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0 &&
                            lbusPersonAccountFlexCompHistory.icdoPersonAccountFlexCompHistory.flex_comp_type_value != busConstant.FlexCompTypeValueCOBRA)
                        {
                            iblnIsFlexPanelVisible = true;
                            //ibusPAEnrollReqFlex.icdoWssPersonAccountEnrollmentRequest.date_of_change = lbusFlexPersonAccount.icdoPersonAccount.history_change_date;
                        }
                    }
                }
            }
        }

        private void SetCobraInsPlanEligibilityAndLoadEnrlObjects(int aintplanId)
        {
            busPersonAccount lbusInsPersonAccount = ibusMemberPerson.icolPersonAccount.FirstOrDefault(pa => pa.icdoPersonAccount.plan_id == aintplanId && !pa.IsCanceled());
            if (lbusInsPersonAccount.IsNotNull())
            {
                lbusInsPersonAccount.LoadPersonAccountGHDV();
                busPersonAccountGhdvHistory lbusPersonAccountGhdvHistory = lbusInsPersonAccount
                                                                                .ibusPersonAccountGHDV
                                                                                .LoadHistoryByDate(icdoWssBenApp.termination_date);
                                                                                //.LoadHistoryByDate(ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date);
                                                                                
                if (lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id > 0 &&
                    ((lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) ||
                    (lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                    lbusInsPersonAccount.icdoPersonAccount.end_date.AddDays(60) >= DateTime.Today)))
                {
                    bool lblnCobraCondition = (aintplanId == busConstant.PlanIdGroupHealth) ?
                                                string.IsNullOrEmpty(lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.cobra_type_value) :
                                                (aintplanId == busConstant.PlanIdDental) ?
                                                (string.IsNullOrEmpty(lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.cobra_type_value) &&
                                                lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.dental_insurance_type_value
                                                != busConstant.DentalInsuranceTypeCOBRA) :
                                                (string.IsNullOrEmpty(lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.cobra_type_value) &&
                                                lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.vision_insurance_type_value
                                                != busConstant.VisionInsuranceTypeCOBRA);
                    if (lblnCobraCondition)
                    {
                        LoadInsEnrlObjects(aintplanId, lbusInsPersonAccount.icdoPersonAccount.history_change_date);
                        switch (aintplanId)
                        {
                            case busConstant.PlanIdGroupHealth:
                                iblnIsHealthEnrltAsCobra = true;
                                break;
                            case busConstant.PlanIdDental:
                                iblnIsDentalEnrltAsCobra = true;
                                break;
                            case busConstant.PlanIdVision:
                                iblnIsVisionEnrltAsCobra = true;
                                break;
                            default:
                                break;
                        }

                    }
                    switch (aintplanId)
                    {
                        case busConstant.PlanIdGroupHealth:
                            if (lbusInsPersonAccount.ibusPersonAccountGHDV.iclbCoverageRef.IsNull()) lbusInsPersonAccount.ibusPersonAccountGHDV.LoadCoverageCodeByFilter();
                            var lenumCoverageCodeList = lbusInsPersonAccount.ibusPersonAccountGHDV.iclbCoverageRef.Where(lobjCoverageRef => lobjCoverageRef.coverage_code == lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.coverage_code);
                            string lstrCoverageCode = string.Empty;
                            if (lenumCoverageCodeList.Count() > 0)
                            {
                                char[] lsplitter = { '-' };
                                //  lintIndex = lenumCoverageCodeList.FirstOrDefault().short_description.IndexOf("-");
                                lbusPersonAccountGhdvHistory.istrCoverageCode = lenumCoverageCodeList.FirstOrDefault().short_description.Split(lsplitter).Last().Trim();
                            }
                            istrCurrentHealthLevelOfCovrage = lbusPersonAccountGhdvHistory.istrCoverageCode;
                            break;
                        case busConstant.PlanIdDental:
                            istrCurrentDentalLevelOfCovrage = lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value;
                            break;
                        case busConstant.PlanIdVision:
                            istrCurrentVisionLevelOfCovrage = lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value;
                            break;
                        default:
                            break;
                    }
                }
            }

        }

        private void LoadInsEnrlObjects(int aintPlanId, DateTime aintDateOfChange)
        {
            switch (aintPlanId)
            {
                case busConstant.PlanIdGroupHealth:
                    iblnIsHealthStepVisible = true;
                    //ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.date_of_change = aintDateOfChange;
                    LoadInsReqOtherObjects(ibusPAEnrollReqHealth);
                    break;
                case busConstant.PlanIdDental:
                    iblnIsDentalStepVisible = true;
                    //ibusPAEnrollReqDental.icdoWssPersonAccountEnrollmentRequest.date_of_change = aintDateOfChange;
                    LoadInsReqOtherObjects(ibusPAEnrollReqDental);
                    break;
                case busConstant.PlanIdVision:
                    iblnIsVisionStepVisible = true;
                    //ibusPAEnrollReqVision.icdoWssPersonAccountEnrollmentRequest.date_of_change = aintDateOfChange;
                    LoadInsReqOtherObjects(ibusPAEnrollReqVision);
                    break;
                //LifePannel
                case busConstant.PlanIdGroupLife:
                    //iblnIsLifePanelVisible = true;
                    //ibusPAEnrollReqLife.icdoWssPersonAccountEnrollmentRequest.date_of_change = aintDateOfChange;
                    //ibusPAEnrollReqLife.icdoWssPersonAccountEnrollmentRequest.mss_retiree_flag = busConstant.Flag_Yes;
                    LoadInsReqOtherObjects(ibusPAEnrollReqLife);
                    break;
            }
        }

        private void LoadInsReqOtherObjects(busWssPersonAccountEnrollmentRequest abusWssPAEnrlRqst)
        {
            //LifePannel
            if (abusWssPAEnrlRqst.icdoWssPersonAccountEnrollmentRequest.plan_id != busConstant.PlanIdGroupLife)
            {
                abusWssPAEnrlRqst.LoadMSSWorkersCompensation();
                abusWssPAEnrlRqst.LoadMSSOtherCoverageDetails();                    
            }
            if (abusWssPAEnrlRqst.icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth)
                abusWssPAEnrlRqst.LoadDependents();
            switch (abusWssPAEnrlRqst.icdoWssPersonAccountEnrollmentRequest.plan_id)
            {
                case busConstant.PlanIdGroupHealth:
                    abusWssPAEnrlRqst.icdoWssPersonAccountEnrollmentRequest.mss_retiree_flag = iblnIsHealthEnrltAsRetiree ? busConstant.Flag_Yes : busConstant.Flag_No;
                    iclbWSSBenAppHealthMSSOtherCoverageDetail = abusWssPAEnrlRqst.iclbMSSOtherCoverageDetail;
                    iclbWSSBenAppHealthMSSWorkerCompensation = abusWssPAEnrlRqst.iclbMSSWorkerCompensation;
                    break;
                case busConstant.PlanIdDental:
                    abusWssPAEnrlRqst.icdoWssPersonAccountEnrollmentRequest.mss_retiree_flag = iblnIsDentalEnrltAsRetiree ? busConstant.Flag_Yes : busConstant.Flag_No;
                    iclbWSSBenAppDentalMSSOtherCoverageDetail = abusWssPAEnrlRqst.iclbMSSOtherCoverageDetail;
                    iclbWSSBenAppDentalMSSWorkerCompensation = abusWssPAEnrlRqst.iclbMSSWorkerCompensation;
                    break;
                case busConstant.PlanIdVision:
                    abusWssPAEnrlRqst.icdoWssPersonAccountEnrollmentRequest.mss_retiree_flag = iblnIsVisionEnrltAsRetiree ? busConstant.Flag_Yes : busConstant.Flag_No;
                    iclbWSSBenAppVisionMSSOtherCoverageDetail = abusWssPAEnrlRqst.iclbMSSOtherCoverageDetail;
                    iclbWSSBenAppVisionMSSWorkerCompensation = abusWssPAEnrlRqst.iclbMSSWorkerCompensation;
                    break;
                //LifePannel
                case busConstant.PlanIdGroupLife:
                    abusWssPAEnrlRqst.icdoWssPersonAccountEnrollmentRequest.mss_retiree_flag = busConstant.Flag_Yes;
                    break;
                default:
                    break;
            }
        }
        
        private void SetLifePanelVisibility(DateTime adteEffecDate, DateTime TodayOrLastModifiedDate)
        {
            //if (ibusMemberPerson.icolPersonAccount.IsNull()) ibusMemberPerson.LoadPersonAccount();
            //lbuslifePersonAccount = ibusMemberPerson.icolPersonAccount.FirstOrDefault(pa => pa.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife && !pa.IsCanceled());
            if (lbuslifePersonAccount.IsNull()) LoadLifePersonAccount();
            if (lbuslifePersonAccount.IsNotNull())
            {
                //lbuslifePersonAccount.LoadPersonAccountLife();                
                //lbuslifePersonAccount.ibusPersonAccountLife.LoadPersonAccountLifeOptions();
                if (lbuslifePersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeOption.Count > 0 &&
                    lbuslifePersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeOption
                    .Any(option => option.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic &&
                                option.icdoPersonAccountLifeOption.coverage_amount > 0))
                {
                    //LifePannel

                    //lbuslifePersonAccount.ibusPersonAccountLife.LoadMemberAge(icdoWssBenApp.retirement_date);
                    lbuslifePersonAccount.ibusPersonAccountLife.LoadPerson();
                    if(lbuslifePersonAccount.ibusPersonAccountLife.IsRetireeAttainedAge65(icdoWssBenApp.retirement_date) == 1) // if member age is greater than 65 show Basic supplement only
                        iclbPersonAccountLifeOption = lbuslifePersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeOption.Where(option=>option.icdoPersonAccountLifeOption.coverage_amount > 0 && option.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic).ToList().ToCollection();
                    else
                        iclbPersonAccountLifeOption = lbuslifePersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeOption.Where(option => option.icdoPersonAccountLifeOption.coverage_amount > 0).ToList().ToCollection();
                    
                    foreach (busPersonAccountLifeOption lbusPALifeOption in iclbPersonAccountLifeOption)
                    {
                        switch (lbusPALifeOption.icdoPersonAccountLifeOption.level_of_coverage_value)
                        {
                            case busConstant.LevelofCoverage_Basic:
                                //if (ibusPAEnrollReqLife.icdoWssPersonAccountEnrollmentRequest.date_of_change < DateTime.MinValue)
                                //    ibusPAEnrollReqLife.icdoWssPersonAccountEnrollmentRequest.date_of_change = ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.date_of_change;
                                idecBasicCoverageAmount = lbusPALifeOption.icdoPersonAccountLifeOption.coverage_amount;         //for validation of supplemental amount used in ValidateLifeInsuranceStep
                                lbusPALifeOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount = ibusPAEnrollReqLife.GetCoverageAmountDetails(lbusPALifeOption.icdoPersonAccountLifeOption.level_of_coverage_value, busConstant.LifeInsuranceTypeRetireeMember);
                                lbusPALifeOption.icdoPersonAccountLifeOption.coverage_amount = lbusPALifeOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount;
                                break;
                            case busConstant.LevelofCoverage_Supplemental:
                                //while load supplemental amount it should be addition of basic.
                                lbusPALifeOption.icdoPersonAccountLifeOption.coverage_amount = lbusPALifeOption.icdoPersonAccountLifeOption.coverage_amount + idecBasicCoverageAmount;
                                lbusPALifeOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount = lbusPALifeOption.icdoPersonAccountLifeOption.coverage_amount;
                                break;
                            case busConstant.LevelofCoverage_DependentSupplemental:
                                //we set drop down value to NewReducedCoverageAmount for display purpose in Print or maintenance(from LOB) screen.
                                lbusPALifeOption.icdoPersonAccountLifeOption.dependent_coverage_option_value = Convert.ToString(Convert.ToInt32(lbusPALifeOption.icdoPersonAccountLifeOption.coverage_amount));
                                break;
                            case busConstant.LevelofCoverage_SpouseSupplemental:
                                lbusPALifeOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount = lbusPALifeOption.icdoPersonAccountLifeOption.coverage_amount;
                                break;
                            default:
                                break;
                        }

                       lbusPALifeOption.icdoPersonAccountLifeOption.enroll_req_plan_id = busConstant.PlanIdGroupLife;
                       lbusPALifeOption.EvaluateInitialLoadRules();
                    }

                    busPersonAccountLifeOption lbusPersonAccountLifeOption = lbuslifePersonAccount.ibusPersonAccountLife.iclbPersonAccountLifeOption
                                                                                        .FirstOrDefault(option =>
                                                                                        option.icdoPersonAccountLifeOption.level_of_coverage_value ==
                                                                                        busConstant.LevelofCoverage_Basic
                                                                                        && option.icdoPersonAccountLifeOption.coverage_amount > 0);
                    if (lbusPersonAccountLifeOption.IsNotNull())
                    {
                        busPersonAccountLifeHistory lbusPersonAccountLifeHistory = lbuslifePersonAccount.ibusPersonAccountLife
                                                                        .LoadHistoryByDate(lbusPersonAccountLifeOption, icdoWssBenApp.termination_date);
                                                                    //.LoadHistoryByDate(lbusPersonAccountLifeOption, ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date);
                        if (lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.plan_participation_status_value
                            == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                            lbusPersonAccountLifeHistory.icdoPersonAccountLifeHistory.coverage_amount > 0 && adteEffecDate >= TodayOrLastModifiedDate)
                        {
                            iblnIsLifePanelVisible = true;
                            DataTable ldtbActiveProviders = Select("cdoIbsHeader.LoadAllActiveProviders", new object[1] { adteEffecDate });
                            if (ldtbActiveProviders.IsNotNull() && ldtbActiveProviders.Rows.Count > 0)
                            {
                                DataRow ldtrActiveProvider = ldtbActiveProviders.AsEnumerable().FirstOrDefault(dr => dr.Field<int>(enmOrgPlan.plan_id.ToString()) == busConstant.PlanIdGroupLife);
                                if (ldtrActiveProvider.IsNotNull() && ldtrActiveProvider[enmOrgPlan.org_id.ToString()] != DBNull.Value)
                                {
                                    int lintOrgId = Convert.ToInt32(ldtrActiveProvider[enmOrgPlan.org_id.ToString()]);
                                    busOrganization lbusOrganization = new busOrganization();
                                    if (lbusOrganization.FindOrganization(lintOrgId))
                                        istrLifeProvider = lbusOrganization.icdoOrganization.org_name + "&nbsp;";
                                }
                            }
                        }
                    }                    
                }
            }
        }

        public bool DoesSickLeaveConversionApply()
        {
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement || icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                //DateTime ldteFifteenthOfTheMonthFollowingEmpEndDate = ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.end_date.GetFirstDayofNextMonth().AddDays(14);
                DateTime ldteFifteenthOfTheMonthFollowingEmpEndDate = icdoWssBenApp.termination_date.GetFirstDayofNextMonth().AddDays(14);
                return ibusPersonEmploymentDtl.icdoPersonEmploymentDetail.person_employment_dtl_id > 0 &&
                        icdoWssBenApp.termination_date != DateTime.MinValue &&
                        (icdoWssBenApp.termination_date > DateTime.Today || DateTime.Today <= ldteFifteenthOfTheMonthFollowingEmpEndDate);
            }
            return false;
        }

        public bool VisibleRuleIsDepStepApplicable() => (((iblnIsHealthStepVisible && ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll
                                                         && ibusPAEnrollReqHealth.icdoMSSGDHV.level_of_coverage_value == busConstant.VisionLevelofCoverageFamily) || (iblnIsDentalStepVisible && ibusPAEnrollReqDental.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll
                                                        && ibusPAEnrollReqDental.icdoMSSGDHV.level_of_coverage_value != busConstant.DentalLevelofCoverageIndividual) || (iblnIsVisionStepVisible && ibusPAEnrollReqVision.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll
                                                        && ibusPAEnrollReqVision.icdoMSSGDHV.level_of_coverage_value != busConstant.VisionLevelofCoverageIndividual)));
        //LifePannel added one condition if life is enrolled
        public bool AnyInsPlanEnrldToPromptForInsPayElection() => ((ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll) ||
                                                                    (ibusPAEnrollReqDental.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll) ||
                                                                    (ibusPAEnrollReqVision.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll) ||
                                                                    (ibusPAEnrollReqLife.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll) ||
                                                                    (ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
                                                                    ibusPAEnrollReqMedicare.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll));
        #endregion
        public string istrOtherCoverageText => "Other Coverage Information";
        public string istrWorkerCompText => "Workers’ Compensation/No-Fault";
        public string istrMedicareText => "Medicare Coverage Information";
        
        public void PopulateObjectsWithDefaults()
        {
            if (string.IsNullOrEmpty(icdoWssBenAppDisaSicknessOrInjury.spcfd_physcn_addr_country_value))
                icdoWssBenAppDisaSicknessOrInjury.spcfd_physcn_addr_country_value = busConstant.US_Code_ID;
            if (string.IsNullOrEmpty(icdoWssBenAppRolloverDetail.addr_country_value))
                icdoWssBenAppRolloverDetail.addr_country_value = busConstant.US_Code_ID;
            icdoWssBenAppAchDetailPrimary.primary_account_flag = busConstant.Flag_Yes;
            icdoWssBenAppAchDetailPrimary.ach_type = busConstant.DepositAchType;
            if (icdoWssBenAppAchDetailPrimary.percentage_of_net_amount == 0.0M && icdoWssBenAppAchDetailPrimary.partial_amount == 0.0M)
                icdoWssBenAppAchDetailPrimary.percentage_of_net_amount = 100.0M;
            icdoWssBenAppAchDetailSecondary.primary_account_flag = busConstant.Flag_No;
            icdoWssBenAppAchDetailSecondary.ach_type = busConstant.DepositAchType;
            icdoWssBenAppAchDetailInSurance.ach_type = busConstant.WithdrawlAchType;
            icdoWssBenAppTaxWithholdingFederal.tax_identifier_value = busConstant.PayeeAccountTaxIdentifierFedTax;
            icdoWssBenAppTaxWithholdingFederal.benefit_distribution_type_value = busConstant.BenefitDistributionMonthlyBenefit;
            icdoWssBenAppTaxWithholdingFederal.tax_ref = busConstant.PayeeAccountTaxRefFed22Tax;
            icdoWssBenAppTaxWithholdingState.tax_identifier_value = busConstant.PayeeAccountTaxIdentifierStateTax;
            icdoWssBenAppTaxWithholdingState.benefit_distribution_type_value = busConstant.BenefitDistributionMonthlyBenefit;
            icdoWssBenAppTaxWithholdingState.tax_ref = busConstant.PayeeAccountTaxRefState22Tax;
        }

        public void btnVertifyOTP_Click()
        {
            string lstrCodeEntered = !string.IsNullOrEmpty(istrActivationCode) ? istrActivationCode.Trim() : istrActivationCode;
            if (!string.IsNullOrEmpty(icdoWssBenApp.otp) && !string.IsNullOrEmpty(lstrCodeEntered) && icdoWssBenApp.otp == lstrCodeEntered)
            {
                if (DateTime.Now <= icdoWssBenApp.otp_expiry_date)
                    icdoWssBenApp.is_otp_validated = busConstant.Flag_Yes;
                else
                    iblnOTPExpired = true;
            }
            else
            {
                icdoWssBenApp.is_otp_validated = busConstant.Flag_No;
            }
        }
        
        public ArrayList btnBenAppVerifyOTP_Click(string astrActivationCode)
        {
            ArrayList larrList = new ArrayList();            
            iblnFinishButtonClicked = true;            
            //if (IsActivationCodeEnteredWithin30Mintues())
            if (!(DateTime.Now <= icdoWssBenApp.otp_expiry_date))
            {
                iblnOTPExpired = true;
                utlError lobjError = new utlError();
                lobjError = AddError(10328, "");
                larrList.Add(lobjError);
                return larrList;
            }            
           if ( (string.IsNullOrEmpty(icdoWssBenApp.otp) || string.IsNullOrEmpty(istrActivationCode)) ||
                (!string.IsNullOrEmpty(icdoWssBenApp.otp) && !string.IsNullOrEmpty(istrActivationCode) && icdoWssBenApp.otp != istrActivationCode))
           {
                utlError lobjError = new utlError();
                lobjError = AddError(10318, "");
                larrList.Add(lobjError);
                return larrList;
            }

            if (iarrErrors.Count > 0)
            {
                return iarrErrors;
            }
            else
            {
                icdoWssBenApp.is_otp_validated = busConstant.Flag_Yes;                
                larrList.Add(this);
            }
            return larrList;
        }
        public void btnGenerateOTP_Click()
        {
            if (iblnFinishButtonClicked && icdoWssBenApp.is_otp_validated != busConstant.Flag_Yes && !iblnIsSaveAndContinueClicked)
            {
                GenerateOTPForWSSBenApp();
            }
        }

        private ArrayList GenerateOTPForWSSBenApp()
        {
            ArrayList larrList = new ArrayList();
            string lstrEmailFrom = Convert.ToString(SystemSettings.Instance["WSSRetrieveMailFrom"]);
            string lstrActivationCodeEmailSub = Convert.ToString(SystemSettings.Instance["WSSActivationCodeEmailSubject"]);
            string lstrActivationCodeEmailMsg = Convert.ToString(SystemSettings.Instance["WSSActivationCodeEmailMsg"]);
            string lstrEmailMessage = string.Empty;

            // string lstrEmailChangeEmailMsgSignature = Convert.ToString(HelperFunction.GetAppSettings("WSSMailBodySignature"));
            string lstrEmailChangeEmailMsgSignature = Convert.ToString(SystemSettings.Instance["WSSMailBodySignature"]);

            string lstrMemberName = ibusMemberPerson.icdoPerson.first_name + " " + ibusMemberPerson.icdoPerson.last_name;

            icdoWssBenApp.otp = busGlobalFunctions.GenerateAnOTP();
            icdoWssBenApp.otp_expiry_date = DateTime.Now.AddMinutes(30);
            icdoWssBenApp.is_otp_validated = busConstant.Flag_No;
            iblnOTPExpired = false;
            StoreObjectInDB(this);
            //lstrActivationCodeEmailMsg = lstrActivationCodeEmailMsg + " : " + icdoWssBenApp.otp;
            lstrEmailMessage = string.Format(lstrActivationCodeEmailMsg + lstrEmailChangeEmailMsgSignature, lstrMemberName, Convert.ToString(icdoWssBenApp.otp));
            try
            {
                busGlobalFunctions.SendMailRetryOnFail(lstrEmailFrom, ibusMemberPerson.icdoPerson.email_address, lstrActivationCodeEmailSub, lstrEmailMessage, true, true);
                larrList.Add(this);
            }
            catch (Exception ex)
            {
                utlError lobjError = new utlError();
                lobjError = AddError(0, "You Cannot Proceed Further With The Application. Please Contact NDPERS. - " + ex.Message.ToString());
                larrList.Add(lobjError);
                iblnFaildToSendMail = true;
                //return larrList;
            }
            return larrList;
        }

        public ArrayList btnResendOTP_Click()
        {
            ArrayList larrList = new ArrayList();
            if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund && icdoWssBenApp.is_otp_validated != busConstant.Flag_Yes)
            {
                larrList = GenerateOTPForWSSBenApp();
                //larrList.Add(this);
            }
            return larrList;
        }
        public ArrayList btnPreviousFromOTP_Click()
        {
            ArrayList larrList = new ArrayList();
            iblnFinishButtonClicked = false;
            istrActivationCode = null;
            return larrList;
        }
        public List<utlCodeValue> LoadLevelOfCoverageByPlan(int aintPlaind, Func<busWssPersonDependent, bool> aFuncDelegate,
            Func<busWssPersonDependent, bool> aFuncOtherDelegate, string astrLevelOfCoverage, bool ablnInsEnlTypeRetiree, string astrCurrentLevelOfCov)
        {
            List<utlCodeValue> llstLevelOfCoverageCodeValues = new List<utlCodeValue>();
            //getting sgs code value for Health Data2 and Dental/Vision from Data1 code id 408
            //List<utlCodeValue> llstCodeValues = (aintPlaind == busConstant.PlanIdGroupHealth) ?
            //                        iobjPassInfo.isrvDBCache.GetCodeValuesFromDict(408, null, Convert.ToString(aintPlaind), null) :
            //                        iobjPassInfo.isrvDBCache.GetCodeValuesFromDict(408, Convert.ToString(aintPlaind), null, null);

            //Filter that list with Dependent and retiree OR Cobra 
            //lCodeValues = (ablnInsEnlTypeCobraOrRetiree) ? llstCodeValues :
            //                        ((iclbRetireeDeps?.Count == 0) || (iclbRetireeDeps?.Count > 0 && iclbRetireeDeps.All(aFuncOtherDelegate)))
            //                ? llstCodeValues.Where(cov => cov.code_value == astrLevelOfCoverage).ToList() :
            //                    ((iclbRetireeDeps?.Count > 0 && iclbRetireeDeps.Any(aFuncDelegate)))
            //                ? llstCodeValues :
            //                    new List<utlCodeValue>();
            //foreach (busWssPersonDependent lbusDependentType in iclbRetireeDeps)
            {
                if (aintPlaind == busConstant.PlanIdGroupHealth)
                {
                    if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund)
                    {
                        if (astrCurrentLevelOfCov == busConstant.CoverageFamily)
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.VisionLevelofCoverageFamily, description = busConstant.CoverageFamily });
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.HMOLevelOfCoverageSingle, description = busConstant.CoverageSingle });
                        }
                        else 
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.HMOLevelOfCoverageSingle, description = busConstant.CoverageSingle });
                        }
                        
                    }
                    if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeDisability || icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement)
                    {
                        if (iintMemberAge > 65)
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.VisionLevelofCoverageFamily, description = busConstant.CoverageFamily });
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.HMOLevelOfCoverageSingle, description = busConstant.CoverageSingle });
                        }
                        else
                        {
                            if (astrCurrentLevelOfCov == busConstant.VisionLevelofCoverageFamily)
                            {
                                llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.VisionLevelofCoverageFamily, description = busConstant.CoverageFamily });
                                llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.HMOLevelOfCoverageSingle, description = busConstant.CoverageSingle });
                            }
                            else
                            {
                                llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.HMOLevelOfCoverageSingle, description = busConstant.CoverageSingle });
                            }
                        }
                    }
                }
                if (aintPlaind == busConstant.PlanIdDental)
                {
                    DataTable dtDentalRate = busGlobalFunctions.LoadDentalRateCacheData(iobjPassInfo);
                    Collection<busOrgPlanDentalRate> iclbOgPlanDentalRate = GetCollection<busOrgPlanDentalRate>(dtDentalRate, "icdoOrgPlanDentalRate");

                    iclbOgPlanDentalRate.OrderByDescending(objRate => objRate.icdoOrgPlanDentalRate.effective_date);

                    if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund ||
                        ((icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeDisability || icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement)
                            && !ablnInsEnlTypeRetiree)
                        )
                    {
                        if (astrCurrentLevelOfCov == busConstant.DentalLevelofCoverageFamily)
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.DentalLevelofCoverageFamily,description = string.Concat(busConstant.CoverageFamily, " - ", " $",
                            iclbOgPlanDentalRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive ||
                            objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanDentalRate.level_of_coverage_value == busConstant.DentalLevelofCoverageFamily).icdoOrgPlanDentalRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.DentalLevelofCoverageIndividualChild,description = string.Concat(busConstant.CoverageIndividualChild, " - ", " $",
                            iclbOgPlanDentalRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive ||
                            objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanDentalRate.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualChild).icdoOrgPlanDentalRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.DentalLevelofCoverageIndividualSpouse, description = string.Concat(busConstant.CoverageIndividualSpouse, " - ", " $",
                            iclbOgPlanDentalRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive ||
                            objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanDentalRate.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualSpouse).icdoOrgPlanDentalRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.DentalLevelofCoverageIndividual,description = string.Concat(busConstant.CoverageIndividual, " - ", " $",
                            iclbOgPlanDentalRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive ||
                            objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanDentalRate.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividual).icdoOrgPlanDentalRate.premium_amt.ToString(), "/month")});
                        }
                        else if (astrCurrentLevelOfCov == busConstant.DentalLevelofCoverageIndividualChild)
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.DentalLevelofCoverageIndividualChild,description = string.Concat(busConstant.CoverageIndividualChild, " - ", " $",
                            iclbOgPlanDentalRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive ||
                            objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanDentalRate.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualChild).icdoOrgPlanDentalRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.DentalLevelofCoverageIndividualSpouse, description = string.Concat(busConstant.CoverageIndividualSpouse, " - ", " $",
                            iclbOgPlanDentalRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive ||
                            objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanDentalRate.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualSpouse).icdoOrgPlanDentalRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.DentalLevelofCoverageIndividual, description = string.Concat(busConstant.CoverageIndividual, " - ", " $",
                            iclbOgPlanDentalRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive ||
                            objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanDentalRate.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividual).icdoOrgPlanDentalRate.premium_amt.ToString(), "/month")});
                        }
                        else if (astrCurrentLevelOfCov == busConstant.DentalLevelofCoverageIndividualSpouse)
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue
                            { code_value = busConstant.DentalLevelofCoverageIndividualSpouse,description = string.Concat(busConstant.CoverageIndividualSpouse, " - ", " $",
                            iclbOgPlanDentalRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive ||
                            objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanDentalRate.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualSpouse).icdoOrgPlanDentalRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.DentalLevelofCoverageIndividual,description = string.Concat(busConstant.CoverageIndividual, " - ", " $",
                            iclbOgPlanDentalRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive ||
                            objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanDentalRate.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividual).icdoOrgPlanDentalRate.premium_amt.ToString(), "/month")});
                        }
                        else
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.DentalLevelofCoverageIndividual, description = string.Concat(busConstant.CoverageIndividual, " - ", " $",
                            iclbOgPlanDentalRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeActive ||
                            objRate.icdoOrgPlanDentalRate.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanDentalRate.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividual).icdoOrgPlanDentalRate.premium_amt.ToString(), "/month")});
                        }
                    }
                    if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeDisability || icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement)
                    {
                        if (ablnInsEnlTypeRetiree)
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.DentalLevelofCoverageFamily, description = busConstant.CoverageFamily });
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.DentalLevelofCoverageIndividualChild, description = busConstant.CoverageIndividualChild });
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.DentalLevelofCoverageIndividualSpouse, description = busConstant.CoverageIndividualSpouse });
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.DentalLevelofCoverageIndividual, description = busConstant.CoverageIndividual });
                        }
                    }
                }
                if (aintPlaind == busConstant.PlanIdVision)
                {
                    DataTable dtVisionRate = busGlobalFunctions.LoadVisionRateCacheData(iobjPassInfo);
                    Collection<busOrgPlanVisionRate> iclbOgPlanVisionRate = GetCollection<busOrgPlanVisionRate>(dtVisionRate, "icdoOrgPlanVisionRate");

                    iclbOgPlanVisionRate.OrderByDescending(objRate => objRate.icdoOrgPlanVisionRate.effective_date);

                    if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund ||
                        ((icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeDisability || icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement)
                            && !ablnInsEnlTypeRetiree)
                        )
                    {
                        if (astrCurrentLevelOfCov == busConstant.VisionLevelofCoverageFamily)
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.VisionLevelofCoverageFamily,description = string.Concat(busConstant.CoverageFamily, " - ", " $",
                            iclbOgPlanVisionRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive ||
                            objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanVisionRate.level_of_coverage_value == busConstant.VisionLevelofCoverageFamily).icdoOrgPlanVisionRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.VisionLevelofCoverageIndividualChild, description = string.Concat(busConstant.CoverageIndividualChild, " - ", " $",
                            iclbOgPlanVisionRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive ||
                            objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanVisionRate.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualChild).icdoOrgPlanVisionRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.VisionLevelofCoverageIndividualSpouse, description = string.Concat(busConstant.CoverageIndividualSpouse, " - ", " $",
                            iclbOgPlanVisionRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive ||
                            objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanVisionRate.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualSpouse).icdoOrgPlanVisionRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.VisionLevelofCoverageIndividual, description = string.Concat(busConstant.CoverageIndividual, " - ", " $",
                            iclbOgPlanVisionRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive ||
                            objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanVisionRate.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividual).icdoOrgPlanVisionRate.premium_amt.ToString(), "/month")});
                        }
                        else if (astrCurrentLevelOfCov == busConstant.VisionLevelofCoverageIndividualChild)
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.VisionLevelofCoverageIndividualChild,description = string.Concat(busConstant.CoverageIndividualChild, " - ", " $",
                            iclbOgPlanVisionRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive ||
                            objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanVisionRate.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualChild).icdoOrgPlanVisionRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.VisionLevelofCoverageIndividualSpouse,description = string.Concat(busConstant.CoverageIndividualSpouse, " - ", " $",
                            iclbOgPlanVisionRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive ||
                            objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanVisionRate.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualSpouse).icdoOrgPlanVisionRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.VisionLevelofCoverageIndividual, description = string.Concat(busConstant.CoverageIndividual, " - ", " $",
                            iclbOgPlanVisionRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive ||
                            objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanVisionRate.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividual).icdoOrgPlanVisionRate.premium_amt.ToString(), "/month")});

                        }
                        else if (astrCurrentLevelOfCov == busConstant.VisionLevelofCoverageIndividualSpouse)
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.VisionLevelofCoverageIndividualSpouse, description = string.Concat(busConstant.CoverageIndividualSpouse, " - ", " $",
                            iclbOgPlanVisionRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive ||
                            objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanVisionRate.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualSpouse).icdoOrgPlanVisionRate.premium_amt.ToString(), "/month")});

                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.VisionLevelofCoverageIndividual, description = string.Concat(busConstant.CoverageIndividual, " - ", " $",
                            iclbOgPlanVisionRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive ||
                            objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanVisionRate.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividual).icdoOrgPlanVisionRate.premium_amt.ToString(), "/month")});
                        }
                        else
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue{ code_value = busConstant.VisionLevelofCoverageIndividual,description = string.Concat(busConstant.CoverageIndividual, " - ", " $",
                            iclbOgPlanVisionRate.FirstOrDefault(objRate => (objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeActive ||
                            objRate.icdoOrgPlanVisionRate.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA) &&
                            objRate.icdoOrgPlanVisionRate.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividual).icdoOrgPlanVisionRate.premium_amt.ToString(), "/month")});
                        }
                    }
                    if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeDisability || icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement)
                    {
                        if (ablnInsEnlTypeRetiree)
                        {
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.VisionLevelofCoverageFamily, description = busConstant.CoverageFamily });
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.VisionLevelofCoverageIndividualChild, description = busConstant.CoverageIndividualChild });
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.VisionLevelofCoverageIndividualSpouse, description = busConstant.CoverageIndividualSpouse });
                            llstLevelOfCoverageCodeValues.Add(new utlCodeValue { code_value = busConstant.VisionLevelofCoverageIndividual, description = busConstant.CoverageIndividual });
                        }
                    }
                }
            }

            return llstLevelOfCoverageCodeValues;
        }
        public List<utlCodeValue> LoadHealthInsLevelOfCoverage()
        {
            List<utlCodeValue> lclcvalues = LoadLevelOfCoverageByPlan(busConstant.PlanIdGroupHealth, dep => dep.icdoWssPersonDependent.is_health_enrolled_already > 0,
                dep => dep.icdoWssPersonDependent.is_health_enrolled_already == 0, busConstant.HMOLevelOfCoverageSingle, iblnIsHealthEnrltAsRetiree, istrCurrentHealthLevelOfCovrage);
            return lclcvalues;
        }
        public List<utlCodeValue> LoadDentInsLevelOfCoverage()
        {
            List<utlCodeValue> lclcvalues = LoadLevelOfCoverageByPlan(busConstant.PlanIdDental, dep => dep.icdoWssPersonDependent.is_dental_enrolled_already > 0,
                dep => dep.icdoWssPersonDependent.is_dental_enrolled_already == 0, busConstant.DentalLevelofCoverageIndividual, iblnIsDentalEnrltAsRetiree, istrCurrentDentalLevelOfCovrage);
            return lclcvalues;

        }
        public List<utlCodeValue> LoadVisInsLevelOfCoverage()
        {
            List<utlCodeValue> lclcvalues = LoadLevelOfCoverageByPlan(busConstant.PlanIdVision, dep => dep.icdoWssPersonDependent.is_vision_enrolled_already > 0,
                dep => dep.icdoWssPersonDependent.is_vision_enrolled_already == 0, busConstant.VisionLevelofCoverageIndividual, iblnIsVisionEnrltAsRetiree, istrCurrentVisionLevelOfCovrage);
            return lclcvalues;
        }

        public void InitializeDeps()
        {
            iclbRetireeDeps = new Collection<busWssPersonDependent>();
            //iclbNonRetireeDeps = new Collection<busWssPersonDependent>();
        }

        //public Collection<busWssAcknowledgement> LoadHPAcknowledgement(DateTime adtEffectiveDate, string astrStepName)
        //{
        //    if (iclbHDHPAcknowledgement == null)
        //    {
        //        busBase lbus = new busBase();
        //        iclbHDHPAcknowledgement = new Collection<busWssAcknowledgement>();
        //        DataTable ldtbHDHPAck = Select("cdoWssAcknowledgement.SelectAck", new object[2] { adtEffectiveDate, astrStepName });
        //        iclbHDHPAcknowledgement = lbus.GetCollection<busWssAcknowledgement>(ldtbHDHPAck);
        //    }
        //    return iclbHDHPAcknowledgement;
        //}

        public bool iblnIsBankNameEdible { get; set; }
        public string istrBenTypeText
        {
            get
            {
                return busGlobalFunctions.GetData2ByCodeValue(1904,icdoWssBenApp.ben_type_value, iobjPassInfo);
            }
        }
        public decimal idecFedStateFlatTaxRate { get; set; }
        public void LoadFedStateFlatTaxRate()
        {
            DataTable ldtbFedStateFlatTaxRate = iobjPassInfo.isrvDBCache.GetCacheData("sgt_fed_state_flat_tax_rate", null);
            Collection<busFedStateFlatTaxRate> iclbFedStateFlatTaxRate = GetCollection<busFedStateFlatTaxRate>(ldtbFedStateFlatTaxRate, "icdoFedStateFlatTaxRate");
            iclbFedStateFlatTaxRate.OrderByDescending(objFlatTaxRate => objFlatTaxRate.icdoFedStateFlatTaxRate.effective_date);

            foreach (busFedStateFlatTaxRate lobjFedStateFlatTaxRate in iclbFedStateFlatTaxRate)
            {
                if (lobjFedStateFlatTaxRate.icdoFedStateFlatTaxRate.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
                {
                    idecFedStateFlatTaxRate = lobjFedStateFlatTaxRate.icdoFedStateFlatTaxRate.flat_tax_percentage;
                }
            }
        }
        public void LoadAllAcknowledgement()
        {
            string lstrMemberName = ibusMemberPerson.icdoPerson.FullName;
            string lstrLifeProvider = string.IsNullOrEmpty(istrLifeProvider) ? string.Empty : istrLifeProvider.Trim();
            DataTable ldtbHDHPAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "1=1");
            Collection<busWssAcknowledgement> iclbWssAcknowledgement = GetCollection<busWssAcknowledgement>(ldtbHDHPAcknowledgement, "icdoWssAcknowledgement");
            foreach (busWssAcknowledgement lobjWssAcknowledgement in iclbWssAcknowledgement)
            {
                switch (lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value)
                {
                    case "HP":
                        istrHPConfirmationText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                    case "HPRT":
                        istrHPRTConfirmationText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                    case "HPCN":
                        istrHPCNConfirmationText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                    case "HPCR":
                        istrHPCRConfirmationText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                    case "DP":
                        istrDPConfirmationText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                    case "DPCN":
                        istrDPCNConfirmationText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                    case "DPCR":
                        istrDPCRConfirmationText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                    case "VP":
                        istrVPConfirmationText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                    case "RFAK":
                        istrRefundACHAckText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                    case "AHAK":
                        istrACHDirectDepositeAckText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                    case "MP":
                        istrMPConfirmationText = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, lstrMemberName, DateTime.Now);
                        break;
                    case "LI":
                        istrLIInformationText = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, lstrLifeProvider);
                        break;
                    case "RC":
                        istrRCRHICHelpText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                    case "LOA":
                        istrLoaAckText = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        break;
                }
            }
            //busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };

        }
        //public string istrHPConfirmationText
        //{
        //    get
        //    {
        //        string lstrMemberName = ibusMemberPerson.icdoPerson.FullName;
        //        DataTable ldtbHDHPAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='HP'");
        //        busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
        //        if (ldtbHDHPAcknowledgement.Rows.Count > 0)
        //            lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbHDHPAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
        //        return string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, lstrMemberName, DateTime.Now);

        //        //iclbHDHPAcknowledgement = LoadHPAcknowledgement(DateTime.Now, busConstant.HealthPlanAcknowledgement);
        //        //return string.Format(iclbHDHPAcknowledgement.FirstOrDefault().icdoWssAcknowledgement.acknowledgement_text, lstrMemberName);
        //    }
        //}
        //public string istrDPConfirmationText
        //{
        //    get
        //    {
        //        string lstrMemberName = ibusMemberPerson.icdoPerson.FullName;
        //        DataTable ldtbHDHPAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='DP'");
        //        busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
        //        if (ldtbHDHPAcknowledgement.Rows.Count > 0)
        //            lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbHDHPAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
        //        return string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, lstrMemberName, DateTime.Now);
        //        //iclbHDHPAcknowledgement = LoadHPAcknowledgement(DateTime.Now, busConstant.DentalPlanAcknowledgement);
        //        //return string.Format(iclbHDHPAcknowledgement.FirstOrDefault().icdoWssAcknowledgement.acknowledgement_text, lstrMemberName);
        //    }
        //}
        //public string istrVPConfirmationText
        //{
        //    get
        //    {
        //        string lstrMemberName = ibusMemberPerson.icdoPerson.FullName;
        //        DataTable ldtbHDHPAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='VP'");
        //        busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
        //        if (ldtbHDHPAcknowledgement.Rows.Count > 0)
        //            lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbHDHPAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
        //        return string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, lstrMemberName, DateTime.Now);
        //        //iclbHDHPAcknowledgement = LoadHPAcknowledgement(DateTime.Now, busConstant.VisionPlanAcknowledgement);
        //        //return string.Format(iclbHDHPAcknowledgement.FirstOrDefault().icdoWssAcknowledgement.acknowledgement_text, lstrMemberName);
        //    }
        //}

        //public string istrMPConfirmationText
        //{
        //    get
        //    {
        //        string lstrMemberName = ibusMemberPerson.icdoPerson.FullName;
        //        DataTable ldtbHDHPAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='MP'");
        //        busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
        //        if (ldtbHDHPAcknowledgement.Rows.Count > 0)
        //            lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbHDHPAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
        //        return string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, lstrMemberName, DateTime.Now);
        //        //iclbHDHPAcknowledgement = LoadHPAcknowledgement(DateTime.Now, busConstant.VisionPlanAcknowledgement);
        //        //return string.Format(iclbHDHPAcknowledgement.FirstOrDefault().icdoWssAcknowledgement.acknowledgement_text, lstrMemberName);
        //    }
        //}



  //      public string istrLIInformationText
  //      {
  //          get
  //          {
  //              string lstrMemberName = string.IsNullOrEmpty(istrLifeProvider) ? string.Empty : istrLifeProvider.Trim();
  //              DataTable ldtbHDHPAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='LI'");
  //              busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
  //              if (ldtbHDHPAcknowledgement.Rows.Count > 0)
  //                  lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbHDHPAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
  //              return string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, lstrMemberName);
  //          }
  //      }
		
		//public string istrRCRHICHelpText
  //      {
  //          get
  //          {
  //              DataTable ldtbHDHPAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='RC'");
  //              busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
  //              if (ldtbHDHPAcknowledgement.Rows.Count > 0)
  //                  lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbHDHPAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
  //              return string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text);
  //          }
  //      }
        //public string istrRefundACHAckText
        //{
        //    get
        //    {
        //        DataTable ldtbAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='RFAK'");
        //        if (ldtbAcknowledgement.Rows.Count > 0)
        //            return string.Format(ldtbAcknowledgement.Rows[0]["acknowledgement_text"].ToString());
        //        return string.Empty;
        //    }
        //}
        //public string istrACHDirectDepositeAckText
        //{
        //    get
        //    {
        //        DataTable ldtbAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='AHAK'");
        //        if (ldtbAcknowledgement.Rows.Count > 0)
        //            return string.Format(ldtbAcknowledgement.Rows[0]["acknowledgement_text"].ToString());
        //        else return string.Empty;
        //    }
        //}
        public string istrGraduatedBenefitOptionHelpText
        {
            get
            {
                return GetAcknowledgementByStepValue("GBO");
            }
        }
        public string istrPLSORequestedHelpText
        {
            get
            {
                return GetAcknowledgementByStepValue("PLO");
            }
        }
        public string istrLPConfirmationText
        {
            get
            {
                return GetAcknowledgementByStepValue("LP");
            }
        }
        public string istrPLSOConfirmationText
        {
            get
            {
                return GetAcknowledgementByStepValue("PLSO");
            }
        }
        public string istrGBConfirmationText
        {
            get
            {
                return GetAcknowledgementByStepValue("GB");
            }
        }
        public string istrSLCConfirmationText
        {
            get
            {
                return GetAcknowledgementByStepValue("SLC");
            }
        }
        public string istrFPConfirmationText
        {
            get
            {
                return GetAcknowledgementByStepValue("FP");
            }
        }
        public string istrAPDBDCConfirmationText
        {
            get
            {
                if (ibusPlan.IsNull()) LoadPlan();
                return icdoWssBenApp.is_deferred == busConstant.Flag_Yes ? GetAcknowledgementByStepValue("APRD"): 
                        (ibusPlan.IsDCRetirementPlan() || ibusPlan.IsHBRetirementPlan()) ? GetAcknowledgementByStepValue("APDC") : GetAcknowledgementByStepValue("APDB");
            }
        }
        public string istrApplicationSubmittedText => busGlobalFunctions.GetMessageTextByMessageID(10436, iobjPassInfo);
        public string istrMultiplanText => IsMemberHasMultiplePlans()? busGlobalFunctions.GetMessageTextByMessageID(10437, iobjPassInfo):string.Empty;
        public string istrRefundForOtherPlan => IsMemberHasMultiplePlans() ? busGlobalFunctions.GetMessageTextByMessageID(10432, iobjPassInfo) : string.Empty;

        public string GetAcknowledgementByStepValue(string screen_step_value)
        {
            DataTable ldtbHDHPAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='"+ screen_step_value +"'");
            if (ldtbHDHPAcknowledgement.Rows.Count > 0)
                return string.Format(Convert.ToString(ldtbHDHPAcknowledgement.Rows[0]["acknowledgement_text"]));
            return string.Format(string.Empty);
        }

        public bool iblnPlanOptionWaivedButDepsEnrolled { get; set; }
        public bool iblnIsCoverageIndividualButDepsEnrolled { get; set; }
        public bool iblnCovNotIndividualWithInvalidDepsEnrolled { get; set; }

        public void SetDepCobraEnltValues()
        {
            if (iclbNonRetireeDeps.IsNull())
                LoadNonRetireeDependents();
            foreach (busWssPersonDependent lbusWssPersonDependent in iclbRetireeDeps)
            {
                busWssPersonDependent lbusWssNonRetireePersonDependent = iclbNonRetireeDeps
                                                                    .FirstOrDefault(dep => dep.icdoWssPersonDependent.target_person_dependent_id ==
                                                                    lbusWssPersonDependent.icdoWssPersonDependent.target_person_dependent_id);
                if (lbusWssNonRetireePersonDependent.IsNotNull())
                {
                    lbusWssPersonDependent.icdoWssPersonDependent.is_health_enrolled_already = lbusWssNonRetireePersonDependent.icdoWssPersonDependent.is_health_enrolled_already;
                    lbusWssPersonDependent.icdoWssPersonDependent.is_dental_enrolled_already = lbusWssNonRetireePersonDependent.icdoWssPersonDependent.is_dental_enrolled_already;
                    lbusWssPersonDependent.icdoWssPersonDependent.is_vision_enrolled_already = lbusWssNonRetireePersonDependent.icdoWssPersonDependent.is_vision_enrolled_already;
                }
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsHealthEnrltAsCobra = iblnIsHealthEnrltAsCobra;
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsDentalEnrltAsCobra = iblnIsDentalEnrltAsCobra;
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsVisionEnrltAsCobra = iblnIsVisionEnrltAsCobra;
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsHealthEnrltAsRetiree = iblnIsHealthEnrltAsRetiree;
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsDentalEnrltAsRetiree = iblnIsDentalEnrltAsRetiree;
                lbusWssPersonDependent.icdoWssPersonDependent.iblnIsVisionEnrltAsRetiree = iblnIsVisionEnrltAsRetiree;

                //showDependantCheckBoxes
                //if (lbusWssPersonDependent.icdoWssPersonDependent.relationship_value == busConstant.FamilyRelationshipSpouse)
                {
                    lbusWssPersonDependent.icdoWssPersonDependent.iblnHealthCovEnrollFamily = IsInsLevelOfCoverage(iblnIsHealthStepVisible,
                                                        ibusPAEnrollReqHealth, busConstant.VisionLevelofCoverageFamily);
                    lbusWssPersonDependent.icdoWssPersonDependent.iblnDentalCovEnrollFamily = IsInsLevelOfCoverage(iblnIsDentalStepVisible,
                                                        ibusPAEnrollReqDental, busConstant.DentalLevelofCoverageFamily);
                    lbusWssPersonDependent.icdoWssPersonDependent.iblnVisionCovEnrollFamily = IsInsLevelOfCoverage(iblnIsVisionStepVisible,
                                                        ibusPAEnrollReqVision, busConstant.VisionLevelofCoverageFamily);

                    lbusWssPersonDependent.icdoWssPersonDependent.is_health_enrolled = lbusWssPersonDependent.icdoWssPersonDependent.iblnHealthCovEnrollFamily ? lbusWssPersonDependent.icdoWssPersonDependent.is_health_enrolled : busConstant.Flag_No;
                }
                if (lbusWssPersonDependent.icdoWssPersonDependent.relationship_value == busConstant.FamilyRelationshipSpouse)
                {
                    lbusWssPersonDependent.icdoWssPersonDependent.iblnDentalCovEnrollIndSpouse = IsInsLevelOfCoverage(iblnIsDentalStepVisible,
                                                        ibusPAEnrollReqDental, busConstant.DentalLevelofCoverageIndividualSpouse);
                    lbusWssPersonDependent.icdoWssPersonDependent.iblnVisionCovEnrollIndSpouse = IsInsLevelOfCoverage(iblnIsVisionStepVisible,
                                                        ibusPAEnrollReqVision, busConstant.VisionLevelofCoverageIndividualSpouse);

                    lbusWssPersonDependent.icdoWssPersonDependent.is_dental_enrolled = lbusWssPersonDependent.icdoWssPersonDependent.iblnDentalCovEnrollIndSpouse || lbusWssPersonDependent.icdoWssPersonDependent.iblnDentalCovEnrollFamily ? lbusWssPersonDependent.icdoWssPersonDependent.is_dental_enrolled : busConstant.Flag_No;
                    lbusWssPersonDependent.icdoWssPersonDependent.is_vision_enrolled = lbusWssPersonDependent.icdoWssPersonDependent.iblnVisionCovEnrollIndSpouse || lbusWssPersonDependent.icdoWssPersonDependent.iblnVisionCovEnrollFamily ? lbusWssPersonDependent.icdoWssPersonDependent.is_vision_enrolled : busConstant.Flag_No;
                }
                else
                {
                    lbusWssPersonDependent.icdoWssPersonDependent.iblnDentalCovEnrollIndSpouse = false;
                }
                if (lbusWssPersonDependent.icdoWssPersonDependent.relationship_value != busConstant.FamilyRelationshipSpouse)
                {
                    lbusWssPersonDependent.icdoWssPersonDependent.iblnDentalCovEnrollIndChild = IsInsLevelOfCoverage(iblnIsDentalStepVisible,
                                                        ibusPAEnrollReqDental, busConstant.DentalLevelofCoverageIndividualChild);
                    lbusWssPersonDependent.icdoWssPersonDependent.iblnVisionCovEnrollIndChild = IsInsLevelOfCoverage(iblnIsVisionStepVisible,
                                                        ibusPAEnrollReqVision, busConstant.VisionLevelofCoverageIndividualChild);

                    lbusWssPersonDependent.icdoWssPersonDependent.is_dental_enrolled = lbusWssPersonDependent.icdoWssPersonDependent.iblnDentalCovEnrollIndChild || lbusWssPersonDependent.icdoWssPersonDependent.iblnDentalCovEnrollFamily ? lbusWssPersonDependent.icdoWssPersonDependent.is_dental_enrolled : busConstant.Flag_No;
                    lbusWssPersonDependent.icdoWssPersonDependent.is_vision_enrolled = lbusWssPersonDependent.icdoWssPersonDependent.iblnVisionCovEnrollIndChild || lbusWssPersonDependent.icdoWssPersonDependent.iblnVisionCovEnrollFamily ? lbusWssPersonDependent.icdoWssPersonDependent.is_vision_enrolled : busConstant.Flag_No;
                }
                lbusWssPersonDependent.EvaluateInitialLoadRules();
            }
        }

        public override void AddToResponse(utlResponseData aobjResponseData)
        {
            aobjResponseData.OtherData["SelectedBenefitOptionValue"] = icdoWssBenApp.ben_opt_value;
            base.AddToResponse(aobjResponseData);
        }
        public Collection<utlCodeValue> LoadStateFromCountry(string ContryCodeValue)
        {
            List < utlCodeValue > llstGetStateList = iobjPassInfo.isrvDBCache.GetCodeValuesFromDict(150);
            Collection<utlCodeValue> lclbStateList = llstGetStateList.Where(filter => filter.data2 == ContryCodeValue).ToList().ToCollection();
            return lclbStateList;
        }
        public bool IsMemberHasMultiplePlans()
        {
            //bool lblnMemberHasMultiplePlans = false;
            Collection<cdoPlan> lclcSupendedRetPlanAccounts = doBase.GetCollection<cdoPlan>(Select("cdoWssPersonAccountEnrollmentRequest.LoadSuspendedRetPlanAccounts", new object[1] { icdoWssBenApp.person_id }));
            if (lclcSupendedRetPlanAccounts.IsNotNull() && lclcSupendedRetPlanAccounts.Count > 1)
            {
                if (icdoWssBenApp.plan_id != busConstant.PlanIdHP && lclcSupendedRetPlanAccounts.Count == 2 && lclcSupendedRetPlanAccounts.Any(x => x.plan_id == busConstant.PlanIdHP))
                    return false;
                return true;
            }
            return false;
        }
        public ArrayList SaveEmailAddress()
        {
            ArrayList larlstResult = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusMemberPerson.IsNull())
                LoadMemberPerson();


            if (ibusMemberPerson.icdoPerson.email_address.IsNullOrEmpty() && istrNewEmailAddress.IsNullOrEmpty())
            {
                lobjError = AddError(10343, busGlobalFunctions.GetMessageTextByMessageID(10343, iobjPassInfo));
                larlstResult.Add(lobjError);
                return larlstResult;
            }
            ibusMemberPerson.icdoPerson.email_address = istrNewEmailAddress;
            ibusMemberPerson.icdoPerson.modified_by = iobjPassInfo.istrUserID;
            ibusMemberPerson.icdoPerson.modified_date = DateTime.Now;
            ibusMemberPerson.icdoPerson.Update();
            return larlstResult;
        }
        public void IsRetireeAttainedAge65()
        {
            
            if (lbuslifePersonAccount.IsNull())
                LoadLifePersonAccount();
            if (iclbPersonAccountLifeOption.IsNotNull() && iclbPersonAccountLifeOption.Count > 0)
            {
                if (lbuslifePersonAccount.ibusPersonAccountLife.IsRetireeAttainedAge65(icdoWssBenApp.retirement_date) == 1)
                {
                    foreach (busPersonAccountLifeOption lobjOption in iclbPersonAccountLifeOption)
                    {
                        lobjOption.icdoPersonAccountLifeOption.NewReducedCoverageAmount = 0;
                    }
                }
            }
        }
        public void LoadActiveRetirementPlan()
        {
            DataTable ldtPersonEmploymentCount = DBFunction.DBSelect("cdoPersonEmployment.CountOfOpenEmployments",
                                                     new object[1] { this.ibusMemberPerson.icdoPerson.person_id }
                                                     , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            iclbMSSRetirementPersonAccount = new Collection<busPersonAccount>();
            ibusMemberPerson.LoadRetirementAccount();
            foreach (busPersonAccount lobjPersonAccount in ibusMemberPerson.iclbRetirementAccount)
            {
                if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled
                    || lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                {
                    lobjPersonAccount.LoadAllPersonEmploymentDetails();
                    if (lobjPersonAccount.iclbEmploymentDetail.Count > 0)
                    {
                        lobjPersonAccount.icdoPersonAccount.EmployerName =
                            lobjPersonAccount.iclbEmploymentDetail[0].ibusPersonEmployment.ibusOrganization.icdoOrganization.org_name;
                    }
                    iclbMSSRetirementPersonAccount.Add(lobjPersonAccount);

                }
            }
            if (iclbMSSRetirementPersonAccount.Count() > 1 && ldtPersonEmploymentCount.Rows.Count == 0)
                iblnIsRetirementAccountAvailable = true;
        }
        public bool ValidateDirectDepositInformation()
        {

            if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRolloverPartOrAllOfMyRefund)
            {
                if ((icdoWssBenAppRolloverDetail.IsNotNull() && icdoWssBenAppRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfTaxable
                      && TotalNonTaxableAmount > 0) || (icdoWssBenAppRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAmountOfTaxable
                      || icdoWssBenAppRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable))
                {
                    return true;
                }
            }
            return false;
        }
        public bool ValidateACHDirectDeposit()
        {
            if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRolloverPartOrAllOfMyRefund)
            {
                if (istrACHDirectDepositeknowledgement_checked != busConstant.Flag_Yes && icdoWssBenAppRolloverDetail?.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable
                    && icdoWssBenAppRolloverDetail?.percent_of_taxable == 0 || istrACHDirectDepositeknowledgement_checked != busConstant.Flag_Yes
                    && icdoWssBenAppRolloverDetail?.rollover_option_value == busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                {
                    return true;
                }
            }
            if(icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRefundToMeDirectly && istrACHDirectDepositeknowledgement_checked != busConstant.Flag_Yes || icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRefundToMeDirectly && istrACHDirectDepositeknowledgement_checked.IsNull())
            {
                return true;
            }
            return false;
        }
        public bool ValidateLetterofAcceptance()
        {
            if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRolloverPartOrAllOfMyRefund)
            {
                if (istrLetterofAcceptanceText_checked != busConstant.Flag_Yes && icdoWssBenAppRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfGross)
                {
                    return true;
                }
            }
            return false;
        }
        public bool ValidateRefundACK()
        {
            if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRolloverPartOrAllOfMyRefund)
            {
                if (istrRefundACHAckText_checked != busConstant.Flag_Yes && icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund && icdoWssBenAppRolloverDetail?.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable
                && icdoWssBenAppRolloverDetail?.percent_of_taxable == 0 || istrACHDirectDepositeknowledgement_checked != busConstant.Flag_Yes
                && icdoWssBenAppRolloverDetail?.rollover_option_value == busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                {
                    return true;
                }
            }
            if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRefundToMeDirectly && istrRefundACHAckText_checked != busConstant.Flag_Yes || icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRefundToMeDirectly && istrRefundACHAckText_checked.IsNull())
            {
                return true;
            }
            return false;
        }
        public string istrUpdateAddress { get; set; }
        public bool IsPeoplesoftEnabled()
        {
            string lstrResult = (string)DBFunction.DBExecuteScalar("cdoCodeValue.IsPeoplesoftEnabled", new object[] { },
                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (ibusMemberPerson.ibusCurrentEmployment == null)
                ibusMemberPerson.LoadCurrentEmployer();
            if (ibusMemberPerson.ibusCurrentEmployment.ibusOrganization == null)
                ibusMemberPerson.ibusCurrentEmployment.LoadOrganization();
            if (ibusMemberPerson.ibusCurrentEmployment.icolPersonEmploymentDetail == null)
                ibusMemberPerson.ibusCurrentEmployment.LoadPersonEmploymentDetail();
            if (lstrResult == null || lstrResult.ToUpper() == "N")
            {
                istrUpdateAddress = "true";
                return false;
            }
            else if (lstrResult.ToUpper() == "Y" && (ibusMemberPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueNonPSParoll
                //|| ibusPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueHigherEd    //PIR 24075
                || ibusMemberPerson.ibusCurrentEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == null))
            {
                istrUpdateAddress = "true";
                return false;
            }
            else if (lstrResult.ToUpper() == "Y" && (ibusMemberPerson.ibusCurrentEmployment.icolPersonEmploymentDetail != null && ibusMemberPerson.ibusCurrentEmployment.icolPersonEmploymentDetail.Count > 0 &&
                ibusMemberPerson.ibusCurrentEmployment.icolPersonEmploymentDetail[0].icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary))
            {
                istrUpdateAddress = "true";
                return false;
            }
            else
                return true;
        }
        //visibilty rule for certify button
        public bool IsPersonCertify()
        {
            bool lblnResult = false;
            if (iblnExternalLogin)
            {
                int lintMonths = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(7010, "MNTH", iobjPassInfo));

                if (ibusMemberPerson.icdoPerson.certify_date != DateTime.MinValue && ((ibusMemberPerson.icdoPerson.certify_date).AddMonths(lintMonths) >= DateTime.Today))
                    lblnResult = false;
                //Certify button visible for first time and after 9 months updating demographic info
                else if (ibusMemberPerson.icdoPerson.certify_date == DateTime.MinValue ||
                     (ibusMemberPerson.icdoPerson.certify_date != DateTime.MinValue && ((ibusMemberPerson.icdoPerson.certify_date).AddMonths(lintMonths) < DateTime.Today)))
                {
                    lblnResult = true;
                }
                else
                    lblnResult = true;
            }
            return lblnResult;
        }
        public bool IsExternalUser()
        {
            if (iblnExternalLogin)
            {
                return true;
            }
            return false;
        }
        public bool checkIfSpouseInformationExists()
        {
            if (ibusMemberPerson.ibusSpouse.IsNull()) ibusMemberPerson.LoadSpouse();
            busPersonContact lobjPersonContact = ibusMemberPerson.icolPersonContact.FirstOrDefault(o => o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
                                                                        && o.icdoPersonContact.status_value == busConstant.PersonContactStatusActive);
            if (lobjPersonContact.IsNull())
            {
                iblnDisplayUpdateContactInfoLink = true;
                return false;
            }
            else if (ibusMemberPerson.ibusSpouse.icdoPerson.date_of_death != DateTime.MinValue)
            {
                iblnDisplayUpdateContactInfoLink = true;
                return false;
            }
            else
                return true;
        }

        public bool IsHealthStepVisible()
        {
            if (ibusMemberPerson.IsNull())
                LoadMemberPerson();
            if (ibusMemberPerson.icolPersonAccount.IsNull())
                ibusMemberPerson.LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in ibusMemberPerson.icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPersonAccountGHDV.IsNull())
                    lbusPersonAccount.LoadPersonAccountGHDV();
                if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund && ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth &&
                    lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                    lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty()) ||
                    (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth &&
                    lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                    lbusPersonAccount.icdoPersonAccount.end_date.AddDays(60) >= DateTime.Today)))
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsDentalStepVisible()
        {
            if (ibusMemberPerson.IsNull())
                LoadMemberPerson();
            if (ibusMemberPerson.icolPersonAccount.IsNull())
                ibusMemberPerson.LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in ibusMemberPerson.icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPersonAccountGHDV.IsNull())
                    lbusPersonAccount.LoadPersonAccountGHDV();
                if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund && ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental &&
                    lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                    lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty()) ||
                    (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDental &&
                    lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                    lbusPersonAccount.icdoPersonAccount.end_date.AddDays(60) >= DateTime.Today)))
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsVisionStepVisible()
        {
            if (ibusMemberPerson.IsNull())
                LoadMemberPerson();
            if (ibusMemberPerson.icolPersonAccount.IsNull())
                ibusMemberPerson.LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in ibusMemberPerson.icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPersonAccountGHDV.IsNull())
                    lbusPersonAccount.LoadPersonAccountGHDV();
                if (icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRefund && ((lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision &&
                    lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                    lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNullOrEmpty()) ||
                    (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdVision &&
                    lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                    lbusPersonAccount.icdoPersonAccount.end_date.AddDays(60) >= DateTime.Today)))
                {
                    return true;
                }
            }
            return false;
        }
        public Collection<cdoCodeValue> LoadRolloverOptions()
        {
            Collection<cdoCodeValue> lclbPlanOption = GetCodeValue(2214);
            DataTable ldtRolloverOptions = Select("cdoWssBenApp.LoadRolloverOptions", new object[0] {  });
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtRolloverOptions);

            if (TotalNonTaxableAmount < 0)
                return lclbPlanOption;
            else
                return lclcCodeValue;
        }
        public bool VisibleRolloverText()
        {

            if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRolloverPartOrAllOfMyRefund)
            {
                if ((icdoWssBenAppRolloverDetail.IsNotNull() && icdoWssBenAppRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfTaxable 
                    && TotalNonTaxableAmount > 0))
                {
                    return true;
                }

            }
            return false;
        }
        public bool VisibleRuleForNoTRefund()
        {
            if(icdoWssBenApp.ben_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                return true;
            }
            return false;
        }
        public bool ValidateRoutingNumber()
        {
            if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRefundToMeDirectly &&
                icdoWssBenAppAchDetailPrimary.routing_no.IsNullOrEmpty())
            {
                return true;
            }
            if (icdoWssBenApp.ref_dist_value == busConstant.RefundDistributionRolloverPartOrAllOfMyRefund &&
                icdoWssBenAppAchDetailPrimary.routing_no.IsNullOrEmpty() && TotalNonTaxableAmount >0 && 
                icdoWssBenAppRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfTaxable)
            {
                return true;
            }
            if ((icdoWssBenAppRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAmountOfTaxable ||
                icdoWssBenAppRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionPercentageOfTaxable) &&
                icdoWssBenAppAchDetailPrimary.routing_no.IsNullOrEmpty())
            {
                return true;
            }
            return false;
        }

        public string istrPeopleSoftURL
        {
            get
            {
                
                if (ibusMemberPerson?.ibusCurrentEmployment == null)
                    ibusMemberPerson?.LoadCurrentEmployer();
                if (ibusMemberPerson?.ibusCurrentEmployment.ibusOrganization == null)
                    ibusMemberPerson?.ibusCurrentEmployment.LoadOrganization();
                if (ibusMemberPerson?.ibusCurrentEmployment?.ibusOrganization?.icdoOrganization?.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueHigherEd)
                {
                    return utlPassInfo.iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.SystemConstantCodeID, busConstant.PeopleSoftHIEDURL);
                }
                else
                {
                    return utlPassInfo.iobjPassInfo.isrvDBCache.GetCodeDescriptionString(busConstant.SystemConstantCodeID, busConstant.PeopleSoftURL);
                }
            }
        }
        public bool IsNDTaxWithHoldingVisible()
        {
            if (icdoWssBenAppTaxWithholdingState.tax_option_value == busConstant.TaxOptionStateTaxwithheld)
                return true;
            return false;
        }

    }

    [Serializable]
    public class busCustomCodeValue
    {
        public string code_value { get; set; }
        public string description { get; set; }
    }
}
