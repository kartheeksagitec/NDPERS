#region Using directives

using System;
using System.Data;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using System.Collections.ObjectModel;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Linq;
#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.busWssBenAppGen:
    /// Inherited from busBase, used to create new business object for main table cdoWssBenApp and its children table. 
    /// </summary>
	[Serializable]
    public class busWssBenAppGen : busExtendBase
    {
        /// <summary>
        /// Constructor for NeoSpin.busWssBenAppGen
        /// </summary>
		public busWssBenAppGen()
        {

        }

        /// <summary>
        /// Gets or sets the main-table object contained in busWssBenAppGen.
        /// </summary>
		public cdoWssBenApp icdoWssBenApp { get; set; }

        #region Properties
        public busBenefitRefundApplication ibusMSSBenefitRefundApplication { get; set; }
        public busRetirementDisabilityApplication ibusMSSRetirementDisabilityApplication { get; set; }
        public string istrActivationCode { get; set; }
        public string chkSameAsDepositeInfo { get; set; }
        public busPerson ibusMemberPerson { get; set; }

        public busPlan ibusPlan { get; set; }
        public busPersonAccount ibusPersonAccount { get; set; }

        public busPersonEmploymentDetail ibusPersonEmploymentDtl { get; set; }
        public busTaxRefConfig ibusTaxRefConfig { get; set; }
        #region DisabilityInfo
        public Collection<busWssBenAppDisaOtherBenefits> iclcWssBenAppDisaOtherBenefits { get; set; }
        public busWssBenAppDisaOtherBenefits ibusWssBenAppDisaOtherBenefits { get; set; }

        public cdoWssBenAppDisaSicknessOrInjury icdoWssBenAppDisaSicknessOrInjury { get; set; }
        public cdoWssBenAppDisaEducation icdoWssBenAppDisaEducation { get; set; }
        public cdoWssBenAppDisaMilitaryService icdoWssBenAppDisaMilitaryService { get; set; }

        //public busWssBenAppDisaWorkHistory ibusWssBenAppDisaWorkHistory { get; set; }
        public Collection<busWssBenAppDisaWorkHistory> iclcWssBenAppDisaWorkHistory { get; set; }
        #endregion
        public cdoWssBenAppAchDetail icdoWssBenAppAchDetailPrimary { get; set; }
        public cdoWssBenAppAchDetail icdoWssBenAppAchDetailSecondary { get; set; }

        public cdoWssBenAppAchDetail icdoWssBenAppAchDetailInSurance { get; set; }

        public cdoWssBenAppRolloverDetail icdoWssBenAppRolloverDetail { get; set; }

        public cdoWssBenAppTaxWithholding icdoWssBenAppTaxWithholdingFederal { get; set; }
        public cdoWssBenAppTaxWithholding icdoWssBenAppTaxWithholdingState { get; set; }

        public busWssPersonAccountEnrollmentRequest ibusPAEnrollReqHealth { get; set; }
        public busWssPersonAccountEnrollmentRequest ibusPAEnrollReqDental { get; set; }
        public busWssPersonAccountEnrollmentRequest ibusPAEnrollReqVision { get; set; }
        //LifePannel
        public busWssPersonAccountEnrollmentRequest ibusPAEnrollReqLife { get; set; }
        public busWssPersonAccountEnrollmentRequest ibusPAEnrollReqMedicare { get; set; }
        public busWssPersonAccountEnrollmentRequest ibusPAEnrollReqFlex { get; set; }

        public Collection<busWssPersonDependent> iclbNonRetireeDeps { get; set; }
        public Collection<busWssPersonDependent> iclbRetireeDeps { get; set; }

        public Collection<busWssAcknowledgement> iclbInsPremACHDetailsAcknowledgement { get; set; }
        public Collection<busWssAcknowledgement> iclbHDHPAcknowledgement { get; set; }

        public bool iblnIsSaveAndContinueClicked { get; set; }
        public bool iblnIsRefundFinishClicked { get; set; }

        public bool iblnFinishButtonClicked = false;
        public bool iblnIsHealthStepVisible { get; set; }
        public bool iblnIsDentalStepVisible { get; set; }
        public bool iblnIsVisionStepVisible { get; set; }

        public bool iblnOTPExpired { get; set; }
        public int iintMemberAge { get; set; }
        public bool iblnIsMedicarePanelVisible { get; set; }
        public bool iblnIsFlexPanelVisible { get; set; }
        public bool iblnIsLifePanelVisible { get; set; }
        public bool iblnRefundSupressStateTaxWithholdingWarning { get; set; }
        public string istrLifeProvider { get; set; }
        //public string istrInsPayACHAcknowledgementFlag { get; set; }
        public bool iblnIsHealthEnrltAsCobra { get; set; }
        public bool iblnIsDentalEnrltAsCobra { get; set; }
        public bool iblnIsVisionEnrltAsCobra { get; set; }
        public string istrCurrentHealthLevelOfCovrage { get; set; }
        public string istrCurrentDentalLevelOfCovrage { get; set; }
        public string istrCurrentVisionLevelOfCovrage { get; set; }
        //public bool iblnIsLifeEnrltAsCobra { get; set; }

        public bool iblnIsHealthEnrltAsRetiree { get; set; }
        public bool iblnIsDentalEnrltAsRetiree { get; set; }
        public bool iblnIsVisionEnrltAsRetiree { get; set; }
        //public bool iblnIsLifeEnrltAsRetiree { get; set; }
        public decimal TotalMemberAccountBalance { get; set; }
        public decimal TotalTaxableAmount { get; set; }
        public decimal TotalNonTaxableAmount { get; set; }
        public Collection<busPersonAccountLifeOption> iclbPersonAccountLifeOption { get; set; }
        public busPersonAccount lbuslifePersonAccount { get; set; }
        public decimal idecBasicCoverageAmount { get; set; }
        public Collection<busWssAcknowledgement> iclbLifeAcknowledgementText { get; set; }
        public string istrConfConfirmationText = string.Empty;
        public Collection<busWssPersonAccountOtherCoverageDetail> iclbWSSBenAppHealthMSSOtherCoverageDetail { get; set; }
        public Collection<busWssPersonAccountOtherCoverageDetail> iclbWSSBenAppDentalMSSOtherCoverageDetail { get; set; }
        public Collection<busWssPersonAccountOtherCoverageDetail> iclbWSSBenAppVisionMSSOtherCoverageDetail { get; set; }

        public Collection<busWssPersonAccountWorkerCompensation> iclbWSSBenAppHealthMSSWorkerCompensation { get; set; }
        public Collection<busWssPersonAccountWorkerCompensation> iclbWSSBenAppDentalMSSWorkerCompensation { get; set; }
        public Collection<busWssPersonAccountWorkerCompensation> iclbWSSBenAppVisionMSSWorkerCompensation { get; set; }
        public string istrIsTaxWithHoldingAcknowledged { get; set; }
        public string istrConfirmationText
        {
            get
            {
                busWssPersonAccountACHDetail lbusWssPersonAccountACHDetail = new busWssPersonAccountACHDetail { ibusPerson = ibusMemberPerson };
                return lbusWssPersonAccountACHDetail.istrConfirmationText;
            }
        }
        public bool IsWithdrawlACHDetailValid
        {
            get
            {
                //LifePannel added one codition if life step is enrolled.
                return ((iblnIsHealthStepVisible &&
                       ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll) ||
                       (iblnIsDentalStepVisible &&
                       ibusPAEnrollReqDental.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll) ||
                       (iblnIsVisionStepVisible &&
                       ibusPAEnrollReqVision.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll) ||
                       (iblnIsLifePanelVisible &&
                       ibusPAEnrollReqLife.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll) ||
                       (iblnIsHealthEnrltAsRetiree &&
                       ibusPAEnrollReqHealth.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
                       ibusPAEnrollReqMedicare.icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll));
            }
        }

        #endregion

        /// <summary>
        /// NeoSpin.busWssBenAppGen.FindWssBenApp():
        /// Finds a particular record from cdoWssBenApp with its primary key. 
        /// </summary>
        /// <param name="aintWssBenAppId">A primary key value of type int of cdoWssBenApp on which search is performed.</param>
        /// <returns>true if found otherwise false</returns>
        public virtual bool FindWssBenApp(int aintWssBenAppId, bool ablnLoadDisaObjects = false, bool ablnLoadOtherObjects = false)
        {
            bool lblnResult = false;
            if (icdoWssBenApp == null)
            {
                icdoWssBenApp = new cdoWssBenApp();
            }
            if (icdoWssBenApp.SelectRow(new object[1] { aintWssBenAppId }))
            {
                if (ablnLoadDisaObjects)
                {
                    if (icdoWssBenAppDisaEducation.IsNull())
                        icdoWssBenAppDisaEducation = new cdoWssBenAppDisaEducation();
                    if (icdoWssBenAppDisaMilitaryService.IsNull())
                        icdoWssBenAppDisaMilitaryService = new cdoWssBenAppDisaMilitaryService();
                    if (icdoWssBenAppDisaSicknessOrInjury.IsNull())
                        icdoWssBenAppDisaSicknessOrInjury = new cdoWssBenAppDisaSicknessOrInjury();
                    if (iclcWssBenAppDisaOtherBenefits.IsNull())
                        iclcWssBenAppDisaOtherBenefits = new Collection<busWssBenAppDisaOtherBenefits>();
                    if (iclcWssBenAppDisaWorkHistory.IsNull())
                        iclcWssBenAppDisaWorkHistory = new Collection<busWssBenAppDisaWorkHistory>();
                    DataTable ldtbDisaObjects = Select<cdoWssBenAppDisaEducation>(new string[1] { enmWssBenAppDisaEducation.wss_ben_app_id.ToString() },
                    new object[1] { icdoWssBenApp.wss_ben_app_id }, null, "disa_education_id desc");
                    if (ldtbDisaObjects.Rows.Count > 0)
                    {
                        icdoWssBenAppDisaEducation.LoadData(ldtbDisaObjects.Rows[0]);
                    }
                    ldtbDisaObjects = Select<cdoWssBenAppDisaMilitaryService>(new string[1] { enmWssBenAppDisaMilitaryService.wss_ben_app_id.ToString() },
                   new object[1] { icdoWssBenApp.wss_ben_app_id }, null, "disa_military_service_id desc");

                    if (ldtbDisaObjects.Rows.Count > 0)
                    {
                        icdoWssBenAppDisaMilitaryService.LoadData(ldtbDisaObjects.Rows[0]);
                    }
                    ldtbDisaObjects = Select<cdoWssBenAppDisaSicknessOrInjury>(new string[1] { enmWssBenAppDisaSicknessOrInjury.wss_ben_app_id.ToString() },
                   new object[1] { icdoWssBenApp.wss_ben_app_id }, null, "disa_sickness_or_injury_id desc");

                    if (ldtbDisaObjects.Rows.Count > 0)
                    {
                        icdoWssBenAppDisaSicknessOrInjury.LoadData(ldtbDisaObjects.Rows[0]);
                    }
                    ldtbDisaObjects = Select<cdoWssBenAppDisaOtherBenefits>(new string[1] { enmWssBenAppDisaOtherBenefits.wss_ben_app_id.ToString() },
                    new object[1] { icdoWssBenApp.wss_ben_app_id }, null, null);
                    if (ldtbDisaObjects.Rows.Count > 0)
                        iclcWssBenAppDisaOtherBenefits = GetCollection<busWssBenAppDisaOtherBenefits>(ldtbDisaObjects, "icdoWssBenAppDisaOtherBenefits");
                    ldtbDisaObjects = Select<cdoWssBenAppDisaWorkHistory>(new string[1] { enmWssBenAppDisaWorkHistory.wss_ben_app_id.ToString() },
                    new object[1] { icdoWssBenApp.wss_ben_app_id }, null, null);
                    if (ldtbDisaObjects.Rows.Count > 0)
                        iclcWssBenAppDisaWorkHistory = GetCollection<busWssBenAppDisaWorkHistory>(ldtbDisaObjects, "icdoWssBenAppDisaWorkHistory");                    
                }
                if (ablnLoadOtherObjects)
                {
                    if (icdoWssBenAppAchDetailPrimary.IsNull())
                        icdoWssBenAppAchDetailPrimary = new cdoWssBenAppAchDetail();
                    if (icdoWssBenAppAchDetailSecondary.IsNull())
                        icdoWssBenAppAchDetailSecondary = new cdoWssBenAppAchDetail();
                    if (icdoWssBenAppAchDetailInSurance.IsNull())
                        icdoWssBenAppAchDetailInSurance = new cdoWssBenAppAchDetail();
                    if (icdoWssBenAppRolloverDetail.IsNull())
                        icdoWssBenAppRolloverDetail = new cdoWssBenAppRolloverDetail();
                    if (icdoWssBenAppTaxWithholdingFederal.IsNull())
                        icdoWssBenAppTaxWithholdingFederal = new cdoWssBenAppTaxWithholding();
                    if (icdoWssBenAppTaxWithholdingState.IsNull())
                        icdoWssBenAppTaxWithholdingState = new cdoWssBenAppTaxWithholding();

                    DataTable ldtbOtherObjects = Select<cdoWssBenAppAchDetail>(new string[3] { enmWssBenAppAchDetail.wss_ben_app_id.ToString(), enmWssBenAppAchDetail.primary_account_flag.ToString(), enmWssBenAppAchDetail.ach_type.ToString() },
                    new object[3] { icdoWssBenApp.wss_ben_app_id, busConstant.Flag_Yes, busConstant.DepositAchType }, null, "wss_ben_app_ach_detail_id desc");
                    if (ldtbOtherObjects.Rows.Count > 0)
                    {
                        icdoWssBenAppAchDetailPrimary.LoadData(ldtbOtherObjects.Rows[0]);
                    }
                    ldtbOtherObjects = Select<cdoWssBenAppAchDetail>(new string[3] { enmWssBenAppAchDetail.wss_ben_app_id.ToString(), enmWssBenAppAchDetail.primary_account_flag.ToString(), enmWssBenAppAchDetail.ach_type.ToString() },
                    new object[3] { icdoWssBenApp.wss_ben_app_id, busConstant.Flag_No, busConstant.DepositAchType }, null, "wss_ben_app_ach_detail_id desc");
                    if (ldtbOtherObjects.Rows.Count > 0)
                    {
                        icdoWssBenAppAchDetailSecondary.LoadData(ldtbOtherObjects.Rows[0]);
                    }
                    ldtbOtherObjects = Select<cdoWssBenAppAchDetail>(new string[2] { enmWssBenAppAchDetail.wss_ben_app_id.ToString(), enmWssBenAppAchDetail.ach_type.ToString() },
                    new object[2] { icdoWssBenApp.wss_ben_app_id, busConstant.WithdrawlAchType }, null, "wss_ben_app_ach_detail_id desc");
                    if (ldtbOtherObjects.Rows.Count > 0)
                    {
                        icdoWssBenAppAchDetailInSurance.LoadData(ldtbOtherObjects.Rows[0]);
                    }
                    ldtbOtherObjects = Select<cdoWssBenAppRolloverDetail>(new string[1] { enmWssBenAppRolloverDetail.wss_ben_app_id.ToString() },
                    new object[1] { icdoWssBenApp.wss_ben_app_id }, null, "wss_ben_app_rollover_detail_id desc");
                    if (ldtbOtherObjects.Rows.Count > 0)
                    {
                        icdoWssBenAppRolloverDetail.LoadData(ldtbOtherObjects.Rows[0]);
                    }
                    ldtbOtherObjects = Select<cdoWssBenAppTaxWithholding>(new string[2] { enmWssBenAppTaxWithholding.wss_ben_app_id.ToString(), enmWssBenAppTaxWithholding.tax_identifier_value.ToString() },
                    new object[2] { icdoWssBenApp.wss_ben_app_id, busConstant.PayeeAccountTaxIdentifierFedTax }, null, "wss_ben_app_tax_withholding_id desc");
                    if (ldtbOtherObjects.Rows.Count > 0)
                    {
                        icdoWssBenAppTaxWithholdingFederal.LoadData(ldtbOtherObjects.Rows[0]);
                    }
                    ldtbOtherObjects = Select<cdoWssBenAppTaxWithholding>(new string[2] { enmWssBenAppTaxWithholding.wss_ben_app_id.ToString(), enmWssBenAppTaxWithholding.tax_identifier_value.ToString() },
                    new object[2] { icdoWssBenApp.wss_ben_app_id, busConstant.PayeeAccountTaxIdentifierStateTax }, null, "wss_ben_app_tax_withholding_id desc");
                    if (ldtbOtherObjects.Rows.Count > 0)
                    {
                        icdoWssBenAppTaxWithholdingState.LoadData(ldtbOtherObjects.Rows[0]);
                    }
                }
                lblnResult = true;
            }
            return lblnResult;
        }

        public virtual bool FindWssBenAppByBenAppId(int aintBenAppId)
        {
            bool lblnResult = false;
            if (icdoWssBenApp == null)
            {
                icdoWssBenApp = new cdoWssBenApp();
            }
            DataTable ldtbBenApp = Select<cdoWssBenApp>(new string[2] { enmWssBenApp.bene_appl_id.ToString(), enmWssBenApp.ben_action_status_value.ToString() },
                                                        new object[2]
                                            { aintBenAppId, busConstant.BenefitApplicationActionStatusCompleted }, null, "WSS_BEN_APP_ID DESC");
            if (ldtbBenApp.Rows.Count > 0)
            {
                icdoWssBenApp.LoadData(ldtbBenApp.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
        public virtual bool FindWssBenAppByPersonId(int aintPersonId, int aintPlanId, string astrBenActionStatusValue)
        {
            bool lblnResult = false;
            if (icdoWssBenApp == null)
            {
                icdoWssBenApp = new cdoWssBenApp();
            }
            DataTable ldtbBenApp = Select<cdoWssBenApp>(new string[3] { enmWssBenApp.person_id.ToString(), enmWssBenApp.plan_id.ToString(), enmWssBenApp.ben_action_status_value.ToString() },
                                                        new object[3] { aintPersonId, aintPlanId, astrBenActionStatusValue }, null, "WSS_BEN_APP_ID DESC");
            if (ldtbBenApp.Rows.Count > 0)
            {
                icdoWssBenApp.LoadData(ldtbBenApp.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        //LifePannel
        public string istrLifeConfirmationTextForView
        {
            get
            {
                return ibusPAEnrollReqLife.istrAcknowledgementText;
            }
        }
        public string istrGetConfirmationText
        {
            get
            {
                return istrConfConfirmationText;
            }
        }
        public string istrHPConfirmationText { get; set; }
        public string istrHPRTConfirmationText { get; set; }
        public string istrHPCNConfirmationText { get; set; }
        public string istrHPCRConfirmationText { get; set; }

        public string istrDPConfirmationText { get; set; }
        public string istrDPCNConfirmationText { get; set; }
        public string istrDPCRConfirmationText { get; set; }
        public string istrVPConfirmationText { get; set; }
        public string istrRefundACHAckText { get; set; }
        public string istrACHDirectDepositeAckText { get; set; }
        public string istrMPConfirmationText { get; set; }
        public string istrLIInformationText { get; set; }
        public string istrRCRHICHelpText { get; set; }

        public string istrLoaAckText { get; set; }
        public string istrHPRTConfirmationText_checked { get; set; }
        public string istrHPCNConfirmationText_checked { get; set; }
        public string istrHPCRConfirmationText_checked { get; set; }
        public string istrDPCNConfirmationText_checked { get; set; }
        public string istrDPCRConfirmationText_checked { get; set; }
        public string istrDPCNVisionConfirmationText_checked { get; set; }
        public string istrDPCRVisionConfirmationText_checked { get; set; }
        public string istrRefundACHAckText_checked { get; set; }
        public string istrACHDirectDepositeknowledgement_checked { get; set; }
        public string istrLastStepToLoad { get; set; }

        public string istrLetterofAcceptanceText_checked { get; set; }
    }
}
