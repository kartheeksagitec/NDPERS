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
using Sagitec.CustomDataObjects;
using System.Linq;
using System.Globalization;
using Sagitec.DataObjects;
using System.Text.RegularExpressions;
using NeoSpin.DataObjects;


#endregion

namespace NeoSpin.BusinessObjects
{
    /// <summary>
    /// Class NeoSpin.BusinessObjects.busWssPersonAccountEnrollmentRequest:
    /// Inherited from busWssPersonAccountEnrollmentRequestGen, the class is used to customize the business object busWssPersonAccountEnrollmentRequestGen.
    /// </summary>
    [Serializable]
    public class busWssPersonAccountEnrollmentRequest : busWssPersonAccountEnrollmentRequestGen
    {
        public string cobra_type_value { get; set; }
        public string istrAcknowledgementText { get; set; } //2368
        public string istrAcknowledgementTextForWaive { get; set; } 
        public string istrHDHPAcknowledgementText { get; set; }
        public string istrHSAAcknowledgementText { get; set; }
        public string istrAcknowledgementMemAuth { get; set; }
        public string istrWaiveReason { get; set; }
        public string istrWaiveReasonOTHRText { get; set; }
        public busPlan ibusPlan { get; set; }
        public bool iblnIsCountLifeEnrollmentRequest { get; set; }
        public busPersonEmploymentDetail ibusPersonEmploymentDetail { get; set; }
        public busPersonAccountEmploymentDetail ibusPersonAccountEmploymentDetail { get; set; }
        public ObjectState iobjObjectStateBeforePersist { get; set; }
        public busPersonAccount ibusPersonAccount { get; set; }
        public bool iblnIsDCEligible { get; set; }
        public Collection<busWssPersonAccountOtherCoverageDetail> iclbMSSOtherCoverageDetail { get; set; }
        public Collection<busWssPersonAccountWorkerCompensation> iclbMSSWorkerCompensation { get; set; }
        public Collection<busPersonAccountFlexCompHistory> iclbLatestFlexCompHistory { get; set; }
        public Collection<busPersonAccountGhdvHistory> iclbLatestGHDVHistory { get; set; }
        public Collection<busPersonAccountLifeHistory> iclbLatestLifeHistory { get; set; }
        public busPersonAccountRetirement ibusMSSPersonAccountRetirement { get; set; }
        public busWssPersonAccountLifeOption ibusMSSLifeOption { get; set; }
        public busPersonAccountLife ibusMSSPersonAccountLife { get; set; }
        public cdoWssPersonAccountFlexComp icdoMSSFlexComp { get; set; }
        public cdoWssPersonAccountFlexCompOption icdoMSSFlexCompOption { get; set; }
        public Collection<busWssPersonAccountFlexCompConversion> iclbMSSFlexCompConversion { get; set; }  //PIR-2373
        public busPersonAccountFlexComp ibusMSSPersonAccountFlexComp { get; set; }
        public decimal idecAmountToBeReplacedForACK91 { get; set; }
        public decimal idecAmountToBeReplacedForACK92 { get; set; }
        public decimal idecAmountToBeReplacedForAC10 { get; set; }
        public decimal idecAmountToBeReplacedForACK5 { get; set; }
        public bool iblnIsJobClassCareerAndtechEdCertifiedTeacher { get; set; }
        public bool iblnIsJobClassDeptOfPublicInstructionCertifiedTeacher { get; set; }
        public bool iblnIsJobClassRetirementPlan { get; set; }
        public string istrSFN53405PartBWaiverAuthorization { get; set; }
        public busPersonAccountGhdv ibusMSSPersonAccountGHDV { get; set; }
        public cdoWssPersonAccountGhdv icdoMSSGDHV { get; set; }
        public doPersonAccountGhdvHsa icdoPersonAccountGhdvHsa { get; set; }
        public bool iblnIsInsideMailVisible { get; set; }
        public bool iblnIsLifeAmountGreater { get; set; }
        public bool iblnIsDirectDepositVisible { get; set; }
        public bool iblnIsSFN58859Visible { get; set; }
        public busPersonDependent ibusPersonDependent { get; set; }
        public string istrCoverageCodeDescription { get; set; }
        public cdoWssPersonAccountFlexCompConversion icdoWSSPersonAccountFlexcompConversion { get; set; }
        public bool iblnFinishButtonClicked { get; set; }
        public decimal idecSupplementalAmount { get; set; }
        public decimal idecNewSupplementalAmount { get; set; }
        public string istrNewDependentSupplementalAmount { get; set; }
        public decimal idecDependentSupplementalAmount { get; set; }
        public decimal idecSpouseSupplementalAmount { get; set; }
        public string istrSupplementalWaiverFlag { get; set; }
        public string istrPreTaxFlag { get; set; }
        public bool iblnIsSupplementalWaiverFlagChanged { get; set; }
        public string istrSupplementalLifeInsurance { get; set; }
        public bool iblnIsSupplementalAmountChanged { get; set; }
        public bool iblnIsDepSupplementalWaiverFlagChanged { get; set; }
        public bool iblnIsSpouseSupplementalWaiverFlagChanged { get; set; }
        public bool iblnIsPreTaxFlagChanged { get; set; }
        public bool iblnIsSupplementalSelected { get; set; }
        public bool iblnIsDependentSelected { get; set; }
        public bool iblnIsSaveAndNext { get; set; }
        public string istrDependentWaiverFlag { get; set; }
        public string istrSpouseWaiverFlag { get; set; }
        public string istrSupplementalWaiverFlagFromLifeOption { get; set; }
        public string istrDependentWaiverFlagFromLifeOption { get; set; }
        public string istrSpouseWaiverFlagFromLifeOption { get; set; }
        public string istrWizardStepName { get; set; }
        public bool iblnIsFromPortal { get; set; }
        public bool iblnIsBenefitEnrollmentFlagNo { get; set; }
        public string istrPreTaxDeductionFlag { get; set; } //PIR 16533 & 17842 
        public bool iblnIsEOI { get; set; }
        public decimal idecNewSpouseSupplementalAmount { get; set; }
        public decimal idecNewDependentCoverageOptionValue { get; set; }
        public bool iblnIsNewHDHPEnrollment { get; set; }   //PIR 20902
        public DateTime idtRequestDateTime { get; set; }
		//PIR 25920 New Plan DC 2025
        public int iintAddlEEContributionPercent { get; set; }
        public string istrIsDCEligible { get; set; }
        public string istrScreenDifferentiator { get; set; }
		public bool iblnIsDC25PlanSelectionCarryForward { get; set; }
        public bool iblnMSSShowAddlEEContributionPercent { get; set; }
        public int iintOldAddlEEContributionPercent { get; set; }
        public string EnrollmentStep { get; set; }
        public bool iblnAutoPostflagCheck { get; set; }
        //PIR 18138
        public Collection<busWssPersonAccountFlexCompConversion> iclbMSSFlexCompConversionSelected { get; set; }

        #region Acknowledgements

        public utlCollection<cdoWssPersonAccountEeAcknowledgement> iclcEeAcknowledgement { get; set; }
        public Collection<busWssAcknowledgement> iclbGHDVAcknowledgement { get; set; }
        public Collection<busWssAcknowledgement> iclbLifeAcknowledgement { get; set; }
        public Collection<busWssAcknowledgement> iclbAcknowledgementCheck { get; set; }
        public Collection<busWssAcknowledgement> iclbAcknowledgementGen { get; set; }
        public Collection<busWssAcknowledgement> iclbAcknowledgementMemAuth { get; set; }
        public Collection<busWssAcknowledgement> iclbAcknowledgementWaiverAuth { get; set; }
        public Collection<busWssAcknowledgement> iclbAcknowledgementNotice { get; set; }
        public Collection<busWssAcknowledgement> iclbEEAcknowledgement { get; set; }
        public Collection<busWssAcknowledgement> iclbHDHPAcknowledgement { get; set; }
        public Collection<busWssAcknowledgement> iclbCollGen1 { get; set; }
        public Collection<busWssAcknowledgement> iclbCollGen2 { get; set; }
        public Collection<busWssAcknowledgement> iclbCollGen3 { get; set; }
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbDBO1 { get; set; }
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbDBO2 { get; set; }
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbDBO3 { get; set; }
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbDBO4 { get; set; }
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbDBO5 { get; set; }
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbDBO6 { get; set; }
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbDBO7 { get; set; }
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbDBO8 { get; set; }
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbDBO9 { get; set; }
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbDBO10 { get; set; }

        public decimal idecSupplementalLimit { get; set; }
        public decimal idecSpouseSupplementalLimit { get; set; }
        public decimal idecBasicCoverageAmount { get; set; }

        //PIR 25712 - Properties for display
        public decimal idecSuppGILimit { get; set; }
        public decimal idecSuppPreTaxLimit { get; set; }
        public decimal idecSuppIncrement { get; set; }
        public decimal idecSuppIncrementLimit { get; set; }
        public decimal idecSpouseSuppGILimit { get; set; }
        public decimal idecSpouseSuppIncrement { get; set; }
        public decimal idecSpouseSuppPercentage { get; set; }
         

        //PIR 18347
        public decimal idecMSRA { get; set; }
        public decimal idecDCRA { get; set; }
        public decimal idecDCRA1 { get; set; }        

        decimal adecMaxAmount = 0.00M; decimal adecMinAmount = 0.00M;
        //PIR 25920 New Plan DC 2025
        public bool iblnIsOnlyADECUpdate { get; set; }
        public string istrADECAcknowledgementAgreementFlag { get; set; }
        public string istrADECAckText { get; set; }
        public string istrADECInfoAckText { get; set; }
        public Collection<busWssAcknowledgement> iclbTempEEADECAcknowledgement { get; set; }
        public decimal idecTempDCTempContribution { get; set; }
        public bool iblnIsFromEmployment { get; set; }
        //PIR 10854
        public string istrDualMemberTFFR
        {
            get
            {
                if (icdoWssPersonAccountEnrollmentRequest.is_enrolled_in_tffr_flag == busConstant.Flag_Yes)
                    return "Yes";
                else
                    return "No";
            }
        }

        //PIR 10854
        public string istrDualMemberTIAACREF
        {
            get
            {
                if (icdoWssPersonAccountEnrollmentRequest.is_enrolled_in_tiaa_cref_flag == busConstant.Flag_Yes)
                    return "Yes";
                else
                    return "No";
            }
        }

        // PIR 9115 functionality enable/disable property
        public string istrIsPIR9115Enabled
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, "9115", iobjPassInfo);
            }
        }

        //pir 6878- modified by PIR 6961
        public string istrConfirmationText
        {
            get
            {
                string luserName = ibusPerson.icdoPerson.FullName;
                DateTime Now = DateTime.Now;
                DataTable ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='CONF'");
                busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
                string lstrConfimation = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, luserName, Now);
                return lstrConfimation;
            }
        }

        //PIR 11806
        public string istrFlexConfirmationText
        {
            get
            {
                string luserName = ibusPerson.icdoPerson.FullName;
                DataTable ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='CONF'");
                busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
                string lstrConfimation = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, luserName, icdoWssPersonAccountEnrollmentRequest.created_date);
                return lstrConfimation;
            }
        }

        public string istrHDHPConfirmationText
        {
            get
            {
                string luserName = string.Empty; if (ibusPerson.IsNotNull()) luserName = ibusPerson.icdoPerson.FullName;
                DataTable ldtbHDHPAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='HDAK'");
                busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
                if (ldtbHDHPAcknowledgement.Rows.Count > 0)
                    lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbHDHPAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
                return string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text, luserName, DateTime.Now);
            }
        }
        public string istrHDHPAchknowledgeText  //19997
        {
            get
            {
                string luserName = string.Empty; if (ibusPerson.IsNotNull()) luserName = ibusPerson.icdoPerson.FullName;
                DataTable ldtbHDHPAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='" + busConstant.ScreenStepValueHSA + "' AND EFFECTIVE_DATE <= '" + DateTime.Now + "'"); //PIR 20986
                var lvarRow = ldtbHDHPAcknowledgement.AsEnumerable().OrderByDescending(dr => dr.Field<DateTime>("EFFECTIVE_DATE")).ThenByDescending(dr => dr.Field<int>("DISPLAY_SEQUENCE")).FirstOrDefault();
                busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
                if (lvarRow.IsNotNull())
                    lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = Convert.ToString(lvarRow["acknowledgement_text"]);
                return lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
            }
        }
        public string istrIAgree
        {
            get
            {
                DataTable ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='GENL'");
                return ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
            }
        }

        public string istrWaive
        {
            get
            {
                DataTable ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='GWC'");
                return ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
            }
        }

        public void LoadHDHPAcknowledgement()
        {
            busBase lbus = new busBase();
            iclbHDHPAcknowledgement = new Collection<busWssAcknowledgement>();
            DataTable ldtbHDHPAck = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.AlternateStructureCodeHDHP });
            iclbHDHPAcknowledgement = lbus.GetCollection<busWssAcknowledgement>(ldtbHDHPAck);
        }

        public Collection<busWssAcknowledgement> LoadAckCheckDetails()
        {
            busBase lbus = new busBase();
            iclbAcknowledgementCheck = new Collection<busWssAcknowledgement>();
            string istrAcknowledgementCheck = null;
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
                istrAcknowledgementCheck = busConstant.LifeCheckBox;
            else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                            icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth ||
                            icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
                istrAcknowledgementCheck = busConstant.GHDVAgreementCheckBox;

            DataTable ldtbAckCheckDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, istrAcknowledgementCheck });
            iclbAcknowledgementCheck = lbus.GetCollection<busWssAcknowledgement>(ldtbAckCheckDetails);

            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
                iclbLifeAcknowledgement = lbus.GetCollection<busWssAcknowledgement>(ldtbAckCheckDetails);
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                            icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth ||
                            icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
                iclbGHDVAcknowledgement = lbus.GetCollection<busWssAcknowledgement>(ldtbAckCheckDetails);

            return iclbAcknowledgementCheck;
        }

        public Collection<busWssAcknowledgement> LoadAckMemAuthDetails()
        {
            busBase lbus = new busBase();
            if (String.IsNullOrEmpty(icdoWssPersonAccountEnrollmentRequest.enrollment_type_value))
                SetEnrollmentTypes();
            iclbAcknowledgementMemAuth = new Collection<busWssAcknowledgement>();
            string istrMemberAuthorization = null;
            if (ibusPlan.IsRetirementPlan())
            {
                if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBRetirement)
                    istrMemberAuthorization = busConstant.MainMemberAuthorization;
                if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBElectedOfficial)
                    istrMemberAuthorization = busConstant.DBRetirementEnrollmentMemberAuthorization;
                if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBOptional)
                    istrMemberAuthorization = busConstant.MainRetirementEnrollmentMemberAuthorization;
                if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCOptional)
                    istrMemberAuthorization = busConstant.DCRetirementEnrollmentMemberAuthorization;
                if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCRetirement)
                    istrMemberAuthorization = busConstant.DCRetirementEnrollmentMemberAuthorization;
            }
            else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex)
                istrMemberAuthorization = busConstant.FlexMemberAuthorization;
            else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
                istrMemberAuthorization = busConstant.GHDVAgreementMemberAuthorization;

            //PIR 16533 & 17842
            DataTable ldtbAckMemAuthDetails;

            if (istrMemberAuthorization == busConstant.GHDVAgreementMemberAuthorization)
                ldtbAckMemAuthDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { icdoWssPersonAccountEnrollmentRequest.date_of_change, istrMemberAuthorization });
            else
                ldtbAckMemAuthDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, istrMemberAuthorization });
            iclbAcknowledgementMemAuth = lbus.GetCollection<busWssAcknowledgement>(ldtbAckMemAuthDetails);

            StringBuilder lstrAcknowledement = new StringBuilder();
            if(iclbAcknowledgementMemAuth.IsNotNull())
            {
                foreach (busWssAcknowledgement AcknowledgementMemAuth in iclbAcknowledgementMemAuth)
                {                    
                    lstrAcknowledement.Append(AcknowledgementMemAuth.icdoWssAcknowledgement.acknowledgement_text);
                    lstrAcknowledement.Append("<br/>");
                }
                istrAcknowledgementMemAuth = lstrAcknowledement.ToString();              
            }

            return iclbAcknowledgementMemAuth;
        }

        public Collection<busWssAcknowledgement> LoadAckWaiverAuthDetails()
        {
            busBase lbus = new busBase();
            if (String.IsNullOrEmpty(icdoWssPersonAccountEnrollmentRequest.enrollment_type_value))
                SetEnrollmentTypes();
            iclbAcknowledgementWaiverAuth = new Collection<busWssAcknowledgement>();
            string istrWaiverAuthorization = null;
            if ((icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain2020)//PIR 20232 ?code
                && icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBElectedOfficial)
            {
                if (ibusPersonEmploymentDetail.IsNull())
                    LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassNonStateElectedOfficial ||
                    ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                {
                    if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing)
                        istrWaiverAuthorization = busConstant.DBElectedOfficialGeneral3;
                    else
                        istrWaiverAuthorization = busConstant.DBElectedOfficialGeneral2;
                }
            }

            DataTable ldtbLifeAckDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, istrWaiverAuthorization });
            iclbAcknowledgementWaiverAuth = lbus.GetCollection<busWssAcknowledgement>(ldtbLifeAckDetails);
            return iclbAcknowledgementWaiverAuth;
        }

        public Collection<busWssAcknowledgement> LoadAckNoticekDetails()
        {
            busBase lbus = new busBase();
            if (String.IsNullOrEmpty(icdoWssPersonAccountEnrollmentRequest.enrollment_type_value))
                SetEnrollmentTypes();
            iclbAcknowledgementNotice = new Collection<busWssAcknowledgement>();
            string istrImportantNotices = null;
            if (ibusPlan.IsRetirementPlan()) // PIR 9732
            {
                if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBRetirement)
                {
                    if (iblnIsDCEligible)
                        istrImportantNotices = busConstant.MainNotice1;
                    else if (iblnIsJobClassCareerAndtechEdCertifiedTeacher)
                        istrImportantNotices = busConstant.MainNotice2;
                    else if (iblnIsJobClassDeptOfPublicInstructionCertifiedTeacher)
                        istrImportantNotices = busConstant.MainNotice3;
                    else if (iblnIsJobClassRetirementPlan)
                        istrImportantNotices = "DBRE";
                }
                else if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBElectedOfficial)
                {
                    if (iblnIsDCEligible)
                        istrImportantNotices = busConstant.DBRetirementEnrollmentNotice1;
                    else if (iblnIsJobClassCareerAndtechEdCertifiedTeacher)
                        istrImportantNotices = busConstant.DBRetirementEnrollmentNotice2;
                    else if (iblnIsJobClassDeptOfPublicInstructionCertifiedTeacher)
                        istrImportantNotices = busConstant.DBRetirementEnrollmentNotice3;
                }
                else if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBOptional)
                {
                    if (iblnIsJobClassCareerAndtechEdCertifiedTeacher)
                        istrImportantNotices = busConstant.MainRetirementEnrollmentNotice1;
                    else if (iblnIsJobClassDeptOfPublicInstructionCertifiedTeacher)
                        istrImportantNotices = busConstant.MainRetirementEnrollmentNotice2;
                }
                else if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCOptional)
                {
                    if (iblnIsJobClassCareerAndtechEdCertifiedTeacher)
                        istrImportantNotices = busConstant.DCOptionalEnrollmentGeneral;
                    else if (iblnIsJobClassDeptOfPublicInstructionCertifiedTeacher)
                        istrImportantNotices = busConstant.DCOptionalEnrollmentGeneral1;
                }
                else if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCRetirement)
                {
                    istrImportantNotices = busConstant.DCRetirementEnrollmentNotice1;
                }
            }

            DataTable ldtbAckNoticeDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, istrImportantNotices });
            iclbAcknowledgementNotice = lbus.GetCollection<busWssAcknowledgement>(ldtbAckNoticeDetails);
            return iclbAcknowledgementNotice;
        }

        public Collection<busWssAcknowledgement> LoadAckGenDetails()
        {
            busBase lbus = new busBase();
            if (String.IsNullOrEmpty(icdoWssPersonAccountEnrollmentRequest.enrollment_type_value))
                SetEnrollmentTypes();
            iclbAcknowledgementGen = new Collection<busWssAcknowledgement>();

            string istrGeneralDetail = "";
            if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBOptional ||
                icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCOptional)
            {

                if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value.Equals(busConstant.EnrollmentTypeDBOptional))
                    istrGeneralDetail = busConstant.MainOptionalEnrollmentGeneral1;
                else if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value.Equals(busConstant.EnrollmentTypeDCOptional))
                    istrGeneralDetail = busConstant.DCOptionalEnrollmentGeneral1;
                if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value.Equals(busConstant.EnrollmentTypeDBOptional) && icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2025)
                    istrGeneralDetail = busConstant.TempAgreementToParticipate;

                iclbCollGen1 = new Collection<busWssAcknowledgement>();
                iclbCollGen2 = new Collection<busWssAcknowledgement>();
                iclbCollGen3 = new Collection<busWssAcknowledgement>();

                DataTable ldtbAckGenDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, istrGeneralDetail });
                iclbAcknowledgementGen = lbus.GetCollection<busWssAcknowledgement>(ldtbAckGenDetails);
                iclbCollGen1 = iclbAcknowledgementGen.Where(obj => obj.icdoWssAcknowledgement.display_sequence == 1).ToList().ToCollection<busWssAcknowledgement>();
                iclbCollGen2 = iclbAcknowledgementGen.Where(obj => obj.icdoWssAcknowledgement.display_sequence == 2).ToList().ToCollection<busWssAcknowledgement>();
                iclbCollGen3 = iclbAcknowledgementGen.Where(obj => obj.icdoWssAcknowledgement.display_sequence == 3).ToList().ToCollection<busWssAcknowledgement>();
            }
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
            {
                istrGeneralDetail = busConstant.GHDVWaiveGeneral;
                DataTable ldtbAckGenDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, istrGeneralDetail });
                iclbAcknowledgementGen = lbus.GetCollection<busWssAcknowledgement>(ldtbAckGenDetails);
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonEmplyDetailType && icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth
                    && iclbAcknowledgementGen.Count > 0)
                {
                    iclbAcknowledgementGen.RemoveAt(0);
                }
            }

            return iclbAcknowledgementGen;
        }
        //PIR 25920 New Plan DC 2025
        public void LoadADECAckGenDetails()
        {
            busBase lbus = new busBase();
            
            iclbTempEEADECAcknowledgement = new Collection<busWssAcknowledgement>();
            Collection<cdoCodeValue> lclbEligibleADECAmountValue = new Collection<cdoCodeValue>();
            lclbEligibleADECAmountValue = LoadADECAmountValues();
            if (ibusPerson.IsNull()) LoadPerson();
            string lstrPermOrTemp = "";
            if (IsTemporaryEmployee)
                lstrPermOrTemp = busConstant.TempAgreementToParticipate;
            else 
                lstrPermOrTemp = busConstant.PermAdditionalContributionElectionInfo;
            
            DataTable ldtbPermADECAck = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, lstrPermOrTemp });
            if (ldtbPermADECAck.IsNotNull() && ldtbPermADECAck.Rows.Count > 0)
                istrADECInfoAckText = ldtbPermADECAck.Rows[0]["acknowledgement_text"].ToString();
            istrADECInfoAckText = string.Format(istrADECInfoAckText, lclbEligibleADECAmountValue?.Count-1, lclbEligibleADECAmountValue?.Count-1);

            DataTable ldtbPermTempADECAck = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.PermAdditionalContributionElectionAck });
            if (ldtbPermTempADECAck.IsNotNull() && ldtbPermADECAck.Rows.Count > 0)
                istrADECAckText = ldtbPermTempADECAck.Rows[0]["acknowledgement_text"].ToString();
            istrADECAckText = string.Format(istrADECAckText, ibusPerson?.icdoPerson?.FullName);

            if (IsTemporaryEmployee)
            {
                DataTable ldtbTempEEADECAcknowledgement = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.TempAdditionalContributionElectionAck });
                iclbAcknowledgementGen = lbus.GetCollection<busWssAcknowledgement>(ldtbTempEEADECAcknowledgement);
                iclbTempEEADECAcknowledgement = iclbAcknowledgementGen.ToList().ToCollection<busWssAcknowledgement>();
                iclbTempEEADECAcknowledgement.Where(lobjPermTempADECAck => lobjPermTempADECAck.icdoWssAcknowledgement.display_sequence == 5).ForEach(lobjPermTempADECAck => lobjPermTempADECAck.icdoWssAcknowledgement.acknowledgement_text = string.Format(lobjPermTempADECAck.icdoWssAcknowledgement.acknowledgement_text, idecTempDCTempContribution));
            }
        }
        public bool IsAllAcknowledgementAreNotSelectedForADECTemp()
        {
            if (IsTemporaryEmployee)
            { 
                DataTable ldtbAckCheckDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.TempAdditionalContributionElectionAck });//PIR 6961
                var ldtbResult = ldtbAckCheckDetails.FilterTable(busConstant.DataType.String, "SHOW_CHECK_BOX_FLAG", busConstant.Flag_Yes);
                if (iclbTempEEADECAcknowledgement?.Count == ldtbResult.Count())
                {
                    if (iclbTempEEADECAcknowledgement.Where(o => o.icdoWssAcknowledgement.is_acknowledgement_selected != busConstant.Flag_Yes).Any())
                        return false;
                    return true;
                }
            }
            return true;
        }
        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbPersonAccountHDHPAcknowledgement { get; set; }
        public void LoadHDHPAcknowledgementView()
        {
            iclbPersonAccountHDHPAcknowledgement = new Collection<busWssPersonAccountEnrollmentRequestAck>();
            Collection<busWssPersonAccountEnrollmentRequestAck> lclbTempcoll = new Collection<busWssPersonAccountEnrollmentRequestAck>();
            DataTable ldtbHDHPAcknowledgement = Select("cdoWssPersonAccountEnrollmentRequestAck.SelectAckforView",
                new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id });
            busBase lbusBase = new busBase();
            StringBuilder lstrAcknowledgement_text = new StringBuilder(); //pir-2368
            lclbTempcoll = lbusBase.GetCollection<busWssPersonAccountEnrollmentRequestAck>(ldtbHDHPAcknowledgement, "icdoWssPersonAccountEnrollmentRequestAck");
            foreach (busWssPersonAccountEnrollmentRequestAck lobjReqAck in lclbTempcoll)
            {
                busWssAcknowledgement lobjAck = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
                lobjAck.FindWssAcknowledgement(lobjReqAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id);
                if (lobjAck.icdoWssAcknowledgement.screen_step_value == busConstant.AlternateStructureCodeHDHP)
                {
                    if (lobjAck.icdoWssAcknowledgement.acknowledgement_id == 149)  //19997
                    {
                        lobjReqAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = lobjReqAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text;
                        iclbPersonAccountHDHPAcknowledgement.Add(lobjReqAck);
                        lstrAcknowledgement_text.Append(lobjReqAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text);
                        lstrAcknowledgement_text.Append("<br/>");
                    }
                    else
                    {
                        lobjReqAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = lobjAck.icdoWssAcknowledgement.acknowledgement_text;
                        iclbPersonAccountHDHPAcknowledgement.Add(lobjReqAck);
                        lstrAcknowledgement_text.Append(lobjAck.icdoWssAcknowledgement.acknowledgement_text);
                        lstrAcknowledgement_text.Append("<br/>");
                    }
                }
                if (lobjAck.icdoWssAcknowledgement.screen_step_value == "HDAK")
                {
                    lobjReqAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = lobjReqAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text;
                    iclbPersonAccountHDHPAcknowledgement.Add(lobjReqAck);
                    lstrAcknowledgement_text.Append(lobjReqAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text);
                    lstrAcknowledgement_text.Append("<br/>");
                }
            }
            istrHDHPAcknowledgementText = lstrAcknowledgement_text.ToString();
            if(!string.IsNullOrEmpty(istrHDHPAchknowledgeText))
                istrHSAAcknowledgementText = istrHDHPAchknowledgeText.Replace("<br/><br/>", "<br/>");
        }

        public Collection<busWssPersonAccountEnrollmentRequestAck> iclbPersonAccountEnrollmentRequestAck { get; set; }
        public utlCollection<busWssPersonAccountEnrollmentRequestAck> iclbPersonAccountEnrollmentRequestAck2 { get; set; }
        public void LoadAckDetailsForView()
        {
            if (String.IsNullOrEmpty(icdoWssPersonAccountEnrollmentRequest.enrollment_type_value))
                SetEnrollmentTypes();
            iclbPersonAccountEnrollmentRequestAck = new utlCollection<busWssPersonAccountEnrollmentRequestAck>();
            DataTable ldtbViewAckDetails = Select("cdoWssPersonAccountEnrollmentRequestAck.SelectAckforView",
                new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id });
            if (ibusPerson.IsNull()) LoadPerson();
            StringBuilder lstrAcknowledgement_text = new StringBuilder(); //pir-2368
            if (!(icdoWssPersonAccountEnrollmentRequest.enrollment_type_value.Equals(busConstant.EnrollmentTypeDBElectedOfficial) &&
                icdoWssPersonAccountEnrollmentRequest.ee_acknowledgement_waiver_flag.Equals("Y")))
            {
                Collection<busWssPersonAccountEnrollmentRequestAck> lclbTempcoll = new utlCollection<busWssPersonAccountEnrollmentRequestAck>();
                busBase lbusBase = new busBase();
                lclbTempcoll = lbusBase.GetCollection<busWssPersonAccountEnrollmentRequestAck>(ldtbViewAckDetails, "icdoWssPersonAccountEnrollmentRequestAck");
                foreach (busWssPersonAccountEnrollmentRequestAck lobjPersonAccountEnrollmentRequestAck in lclbTempcoll)
                {
                    if (lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text != null)
                    {
                        busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
                        lobjWssAcknowledgement.FindWssAcknowledgement(lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id);
                        if (lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value != busConstant.AlternateStructureCodeHDHP)
                        {
                            iclbPersonAccountEnrollmentRequestAck.Add(lobjPersonAccountEnrollmentRequestAck);
                            lstrAcknowledgement_text.Append(lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text);
                            lstrAcknowledgement_text.Append("<br/>");
                            if (lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value == busConstant.ConfirmationString && !string.IsNullOrEmpty(istrHSAAcknowledgementText) && 
                                !istrHSAAcknowledgementText.Contains(lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text))
                                istrHSAAcknowledgementText = istrHSAAcknowledgementText + "<br/>" + lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text;
                        }
                    }
                    else if (lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text == null)
                    {
                        DataTable ldtbSpecificAckForView = Select("cdoWssAcknowledgement.SelectSpecificAckforView",
                            new object[1] { lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id });
                        busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                        lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                        lobjWssAcknowledgement.icdoWssAcknowledgement.LoadData(ldtbSpecificAckForView.Rows[0]);
                        lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        //PIR 25920 DC 2025 changes
                        if (lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value == busConstant.PermAdditionalContributionElectionAck)
                            lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = string.Format(lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text, ibusPerson.icdoPerson.FullName);
                        if (lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value != busConstant.AlternateStructureCodeHDHP && lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value != "HSA" &&
                            lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value != "HDAK")
                        {
                            iclbPersonAccountEnrollmentRequestAck.Add(lobjPersonAccountEnrollmentRequestAck);
                            lstrAcknowledgement_text.Append(lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text);
                            lstrAcknowledgement_text.Append("<br/>");
                        }
                    }
                }
            }
            if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBElectedOfficial &&
                icdoWssPersonAccountEnrollmentRequest.ee_acknowledgement_waiver_flag == "Y")
            {
                Collection<busWssPersonAccountEnrollmentRequestAck> lclbTempcoll = new utlCollection<busWssPersonAccountEnrollmentRequestAck>();
                busBase lbusBase = new busBase();
                lclbTempcoll = lbusBase.GetCollection<busWssPersonAccountEnrollmentRequestAck>(ldtbViewAckDetails, "icdoWssPersonAccountEnrollmentRequestAck");

               // lclbTempcoll = doBase.GetCollection<cdoWssPersonAccountEnrollmentRequestAck>(ldtbViewAckDetails);
                foreach (busWssPersonAccountEnrollmentRequestAck lobjPersonAccountEnrollmentRequestAck2 in lclbTempcoll)
                {
                    if (lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text != null)
                    {
                        busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement { icdoWssAcknowledgement = new cdoWssAcknowledgement() };
                        lobjWssAcknowledgement.FindWssAcknowledgement(lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id);
                        if (lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value != busConstant.AlternateStructureCodeHDHP)
                        {
                            iclbPersonAccountEnrollmentRequestAck.Add(lobjPersonAccountEnrollmentRequestAck2);
                            lstrAcknowledgement_text.Append(lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text);
                            lstrAcknowledgement_text.Append("<br/>");
                        }
                    }
                    else if (lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text == null)
                    {
                        DataTable ldtbSpecificAckForView = Select("cdoWssAcknowledgement.SelectSpecificAckforView",
                            new object[1] { lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id });
                        busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                        lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                        lobjWssAcknowledgement.icdoWssAcknowledgement.LoadData(ldtbSpecificAckForView.Rows[0]);
                        lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        //PIR 25920 DC 2025 changes
                        if (lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value == busConstant.PermAdditionalContributionElectionAck)
                            lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = string.Format(lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text, ibusPerson.icdoPerson.FullName);
                        if (lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value != busConstant.AlternateStructureCodeHDHP &&
                            lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value != "HDAK")
                        {
                            iclbPersonAccountEnrollmentRequestAck.Add(iclbPersonAccountEnrollmentRequestAck2);
                            lstrAcknowledgement_text.Append(lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text);
                            lstrAcknowledgement_text.Append("<br/>");
                        }
                    }
                }
            }
            istrAcknowledgementText = lstrAcknowledgement_text.ToString();
            istrAcknowledgementTextForWaive = lstrAcknowledgement_text.ToString();
        }

        public void LoadAckDetailsForDBODCOView()
        {
            if (String.IsNullOrEmpty(icdoWssPersonAccountEnrollmentRequest.enrollment_type_value))
                SetEnrollmentTypes();
            if (ibusPerson.IsNull()) LoadPerson();
            iclbPersonAccountEnrollmentRequestAck = new utlCollection<busWssPersonAccountEnrollmentRequestAck>();
            DataTable ldtbViewDetailsForDBODCO = Select("cdoWssPersonAccountEnrollmentRequestAck.SelectAckforView",
                new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id });
            string lstrGen = "";
            string lstrNotice = "";
            string lstrAuth = "";
            string lstrMain = "";
            if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value.Equals(busConstant.EnrollmentTypeDBOptional))
            {
                lstrGen = busConstant.MainOptionalEnrollmentGeneral;
                lstrNotice = busConstant.MainRetirementEnrollmentNotice;
                lstrAuth = busConstant.MainRetirementEnrollmentMemberAuthorization;
                lstrMain = busConstant.PlanRetirementTypeValueDB;
            }
            else if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value.Equals(busConstant.EnrollmentTypeDCOptional))
            {
                lstrGen = busConstant.DCOptionalEnrollmentGeneral;
                lstrNotice = busConstant.DCRetirementEnrollmentNotice;
                lstrAuth = busConstant.DCRetirementEnrollmentMemberAuthorization;
                lstrMain = busConstant.PlanRetirementTypeValueDC;
            }

            if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBOptional &&
                icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_c_flag == busConstant.Flag_Yes)
            {
                busBase lbus = new busBase();
                //PIR 25920 DC 2025 changes
                iclbPersonAccountEnrollmentRequestAck = lbus.GetCollection<busWssPersonAccountEnrollmentRequestAck>(ldtbViewDetailsForDBODCO);
                //iclbPersonAccountEnrollmentRequestAck = new utlCollection<busWssPersonAccountEnrollmentRequestAck>();
                //iclbPersonAccountEnrollmentRequestAck = doBase.GetCollection<busWssPersonAccountEnrollmentRequestAck>(ldtbViewDetailsForDBODCO);
                foreach (busWssPersonAccountEnrollmentRequestAck lobjPersonAccountEnrollmentRequestAck in iclbPersonAccountEnrollmentRequestAck)
                {
                    if (lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text != null)
                    {

                    }
                    else if (lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text == null)
                    {
                        DataTable ldtbSpecificAckForView = Select("cdoWssAcknowledgement.SelectSpecificAckforView",
                            new object[1] { lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id });
                        busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                        lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                        lobjWssAcknowledgement.icdoWssAcknowledgement.LoadData(ldtbSpecificAckForView.Rows[0]);
                        lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        //PIR 25920 DC 2025 changes
                        if (lobjWssAcknowledgement.icdoWssAcknowledgement.screen_step_value == busConstant.PermAdditionalContributionElectionAck)
                            lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = string.Format(lobjPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text, ibusPerson.icdoPerson.FullName);
                    }
                }
                int aintack_id = 0;
                DataTable ldtbListWSSAcknowledgement = new DataTable();
                ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='" + lstrGen + "1' and display_sequence=1");
                if(icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2025)
                ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='" + busConstant.TempAgreementToParticipate + "' and display_sequence=1");

                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    aintack_id = Convert.ToInt32(ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_id"]);
                iclbDBO1 = iclbPersonAccountEnrollmentRequestAck.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id).ToList().ToCollection<busWssPersonAccountEnrollmentRequestAck>();

                ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='" + lstrGen + "1' and display_sequence=2");
                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    aintack_id = Convert.ToInt32(ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_id"]);
                iclbDBO2 = iclbPersonAccountEnrollmentRequestAck.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id).ToList().ToCollection<busWssPersonAccountEnrollmentRequestAck>();

                ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='GENL'");
                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    aintack_id = Convert.ToInt32(ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_id"]);
                iclbDBO4 = iclbPersonAccountEnrollmentRequestAck.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id).ToList().ToCollection<busWssPersonAccountEnrollmentRequestAck>();

                ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value ='" + lstrNotice + "1'");
                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    aintack_id = Convert.ToInt32(ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_id"]);
                iclbDBO5 = iclbPersonAccountEnrollmentRequestAck.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id).ToList().ToCollection<busWssPersonAccountEnrollmentRequestAck>();

                ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value = '" + lstrNotice + "2'");
                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    aintack_id = Convert.ToInt32(ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_id"]);
                iclbDBO7 = iclbPersonAccountEnrollmentRequestAck.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id).ToList().ToCollection<busWssPersonAccountEnrollmentRequestAck>();

                ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value ='" + lstrAuth + "'");
                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    aintack_id = Convert.ToInt32(ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_id"]);
                iclbDBO8 = iclbPersonAccountEnrollmentRequestAck.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id).ToList().ToCollection<busWssPersonAccountEnrollmentRequestAck>();
                //PIR 25920 Additing DC25 related ack into list
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2025)
                {
                    ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value ='" + busConstant.PermAdditionalContributionElectionAck + "'");
                    if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                        aintack_id = Convert.ToInt32(ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_id"]);
                    Collection<busWssPersonAccountEnrollmentRequestAck> lstAckPACA = iclbPersonAccountEnrollmentRequestAck.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id).ToList().ToCollection<busWssPersonAccountEnrollmentRequestAck>();
                    if (lstAckPACA.IsNotNull() && iclbDBO8.IsNotNull())
                    {
                        var lstConcatList = iclbDBO8.Concat(lstAckPACA);
                        iclbDBO8 = lstConcatList.ToList().ToCollection();
                    }
                }

                ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value ='GNL2'");
                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    aintack_id = Convert.ToInt32(ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_id"]);
                iclbDBO9 = iclbPersonAccountEnrollmentRequestAck.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id).ToList().ToCollection<busWssPersonAccountEnrollmentRequestAck>();

                iclbDBO6 = new Collection<busWssPersonAccountEnrollmentRequestAck>();
                ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value ='" + lstrMain + "'");
                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    aintack_id = Convert.ToInt32(ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_id"]);
                for (int i = 0; i < ldtbListWSSAcknowledgement.Rows.Count; i++)
                {
                    busWssPersonAccountEnrollmentRequestAck lcdoAck = iclbPersonAccountEnrollmentRequestAck.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id + i).FirstOrDefault();
                    if (lcdoAck != null)
                        iclbDBO6.Add(lcdoAck);
                }

            }
            if ((icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBOptional) &&
                icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag == busConstant.Flag_Yes)
            {
                iclbPersonAccountEnrollmentRequestAck2 = doBase.GetCollection<busWssPersonAccountEnrollmentRequestAck>(ldtbViewDetailsForDBODCO);
                foreach (busWssPersonAccountEnrollmentRequestAck lobjPersonAccountEnrollmentRequestAck2 in iclbPersonAccountEnrollmentRequestAck2)
                {
                    if (lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text != null)
                    {

                    }
                    else if (lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text == null)
                    {
                        DataTable ldtbSpecificAckForView = Select("cdoWssAcknowledgement.SelectSpecificAckforView",
                            new object[1] { lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id });
                        busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                        lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                        lobjWssAcknowledgement.icdoWssAcknowledgement.LoadData(ldtbSpecificAckForView.Rows[0]);
                        lobjPersonAccountEnrollmentRequestAck2.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                    }
                }
                int aintack_id = 0;
                DataTable ldtbListWSSAck = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='" + lstrGen + "1' and display_sequence=3");
                if (ldtbListWSSAck.Rows.Count > 0)
                    aintack_id = Convert.ToInt32(ldtbListWSSAck.Rows[0]["acknowledgement_id"]);
                iclbDBO3 = iclbPersonAccountEnrollmentRequestAck2.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id).ToList().ToCollection<busWssPersonAccountEnrollmentRequestAck>();

                ldtbListWSSAck = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='CONF'");
                if (ldtbListWSSAck.Rows.Count > 0)
                    aintack_id = Convert.ToInt32(ldtbListWSSAck.Rows[0]["acknowledgement_id"]);
                iclbDBO4 = iclbPersonAccountEnrollmentRequestAck2.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id).ToList().ToCollection<busWssPersonAccountEnrollmentRequestAck>();

                iclbDBO6 = new Collection<busWssPersonAccountEnrollmentRequestAck>();
                ldtbListWSSAck = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value ='" + lstrMain + "'");
                if (ldtbListWSSAck.Rows.Count > 0)
                    aintack_id = Convert.ToInt32(ldtbListWSSAck.Rows[0]["acknowledgement_id"]);
                for (int i = 0; i < ldtbListWSSAck.Rows.Count; i++)
                {
                    busWssPersonAccountEnrollmentRequestAck lcdoAck = iclbPersonAccountEnrollmentRequestAck2.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == aintack_id + i).FirstOrDefault();
                    if (lcdoAck != null)
                        iclbDBO6.Add(lcdoAck);
                }
            }

            DataTable ldtbConfWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value ='CONF'");
            int lintack_id = 0;
            if (ldtbConfWSSAcknowledgement.Rows.Count > 0)
                lintack_id = Convert.ToInt32(ldtbConfWSSAcknowledgement.Rows[0]["acknowledgement_id"]);
            iclbDBO10 = iclbPersonAccountEnrollmentRequestAck.Where(obj => obj.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id == lintack_id).ToList().ToCollection<busWssPersonAccountEnrollmentRequestAck>();
        }

        //load ee acknowledgement
        public Collection<busWssAcknowledgement> LoadEEAcknowledgementCollection()
        {
            busBase lbus = new busBase();
            iclbEEAcknowledgement = new Collection<busWssAcknowledgement>();
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain2020)//PIR 20232 ?code
            {
                DataTable ldtbListWSSAckDB = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.PlanRetirementTypeValueDB });//PIR 6961
                //DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(1022, busConstant.AcknowledgementTypeDB, null, null);
                iclbEEAcknowledgement = lbus.GetCollection<busWssAcknowledgement>(ldtbListWSSAckDB);
            }
            else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2020) //PIR 20232
            {
                DataTable ldtbListWSSAckDC = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.PlanRetirementTypeValueDC });//PIR 6961
                //DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(1022, busConstant.AcknowledgementTypeDC, null, null);
                iclbEEAcknowledgement = lbus.GetCollection<busWssAcknowledgement>(ldtbListWSSAckDC);
            }
            else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2025) //PIR 25920 New Plan DC 2025
            {
                DataTable ldtbListWSSAckDC = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.TempAdditionalContributionElectionAck });//PIR 6961
                //DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(1022, busConstant.AcknowledgementTypeDC, null, null);
                iclbEEAcknowledgement = lbus.GetCollection<busWssAcknowledgement>(ldtbListWSSAckDC);
                //LoadADECAmountValues();
            }
            LoadAmountsToBeReplaced();
            foreach (busWssAcknowledgement lobjCodeValue in iclbEEAcknowledgement)
            {

                //if (lobjCodeValue.code_value == busConstant.EEAcknowledgementAC17)
                //{
                //    lobjCodeValue.description = lobjCodeValue.description.Replace("{0}", idecAmountToBeReplacedForACK91.ToString());
                //    lobjCodeValue.description = lobjCodeValue.description.Replace("{0}", idecAmountToBeReplacedForACK92.ToString());
                //}
                //PIR 6961
                if ((icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC ||
                     icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2020) && lobjCodeValue.icdoWssAcknowledgement.display_sequence == 7) //PIR 20232
                {
                    lobjCodeValue.icdoWssAcknowledgement.acknowledgement_text = string.Format(lobjCodeValue.icdoWssAcknowledgement.acknowledgement_text, idecAmountToBeReplacedForACK91.ToString());
                    lobjCodeValue.icdoWssAcknowledgement.acknowledgement_text = string.Format(lobjCodeValue.icdoWssAcknowledgement.acknowledgement_text, idecAmountToBeReplacedForACK91.ToString());

                }

                //else if (lobjCodeValue.code_value == busConstant.EEAcknowledgementAC10)
                //{
                //    lobjCodeValue.description = lobjCodeValue.description.Replace("{0}", idecAmountToBeReplacedForAC10.ToString());
                //}
                //PIR 6961
                else if ((icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain2020)//PIR 20232 ?code
                    && lobjCodeValue.icdoWssAcknowledgement.display_sequence == 7)
                {
                    lobjCodeValue.icdoWssAcknowledgement.acknowledgement_text = string.Format(lobjCodeValue.icdoWssAcknowledgement.acknowledgement_text, idecAmountToBeReplacedForAC10.ToString());
                }
                //else if ((icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2025)//PIR 25920 New Plan DC 2025
                //    && lobjCodeValue.icdoWssAcknowledgement.display_sequence == 5)
                //{
                //    lobjCodeValue.icdoWssAcknowledgement.acknowledgement_text = string.Format(lobjCodeValue.icdoWssAcknowledgement.acknowledgement_text, idecTempDCTempContribution.ToString());
                //}
                //else if (lobjCodeValue.code_value == busConstant.EEAcknowledgementAC16)
                //{
                //    lobjCodeValue.description = lobjCodeValue.description.Replace("{0}", idecAmountToBeReplacedForACK5.ToString());
                //}
                //PIR 6961
                else if ((icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2020) && lobjCodeValue.icdoWssAcknowledgement.display_sequence == 6) //PIR 20232
                {
                    lobjCodeValue.icdoWssAcknowledgement.acknowledgement_text = string.Format(lobjCodeValue.icdoWssAcknowledgement.acknowledgement_text, idecAmountToBeReplacedForACK5.ToString());
                }
            }
            return iclbEEAcknowledgement;
        }

        public void LoadEEAcknowledgement()
        {
            iclcEeAcknowledgement = new utlCollection<cdoWssPersonAccountEeAcknowledgement>();
            iclcEeAcknowledgement = GetCollection<cdoWssPersonAccountEeAcknowledgement>(
                new string[1] { "wss_person_account_enrollment_request_id" },
                new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
            //PIR-6195
            LoadAmountsToBeReplaced();
            foreach (cdoWssPersonAccountEeAcknowledgement lobjWssPersonAccountEeAcknowledgement in iclcEeAcknowledgement)
            {
                if (lobjWssPersonAccountEeAcknowledgement.ee_acknowledgement_value == busConstant.EEAcknowledgementAC17)
                {
                    lobjWssPersonAccountEeAcknowledgement.ee_acknowledgement_description = lobjWssPersonAccountEeAcknowledgement.ee_acknowledgement_description.Replace("{0}", idecAmountToBeReplacedForACK91.ToString());
                    lobjWssPersonAccountEeAcknowledgement.ee_acknowledgement_description = lobjWssPersonAccountEeAcknowledgement.ee_acknowledgement_description.Replace("{0}", idecAmountToBeReplacedForACK92.ToString());
                }
                else if (lobjWssPersonAccountEeAcknowledgement.ee_acknowledgement_value == busConstant.EEAcknowledgementAC10)
                {
                    lobjWssPersonAccountEeAcknowledgement.ee_acknowledgement_description = lobjWssPersonAccountEeAcknowledgement.ee_acknowledgement_description.Replace("{0}", idecAmountToBeReplacedForAC10.ToString());
                }
                else if (lobjWssPersonAccountEeAcknowledgement.ee_acknowledgement_value == busConstant.EEAcknowledgementAC16)
                {
                    lobjWssPersonAccountEeAcknowledgement.ee_acknowledgement_description = lobjWssPersonAccountEeAcknowledgement.ee_acknowledgement_description.Replace("{0}", idecAmountToBeReplacedForACK5.ToString());
                }
            }
        }

        //PIR 6961
        public void InsertCollection(Collection<busWssAcknowledgement> aclbAcknowledgement)
        {
            busWssPersonAccountEnrollmentRequestAck lobjWssPersonAccountEnrollmentRequestAck = new busWssPersonAccountEnrollmentRequestAck();
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck = new cdoWssPersonAccountEnrollmentRequestAck();

            foreach (busWssAcknowledgement lobjWssAcknowledgement in aclbAcknowledgement)
            {
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id = lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_id;
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.Insert();
            }

        }

        //PIR 6961
        public void InsertString(string astrQuersyString, string astrAcknowText = null)
        {
            busWssPersonAccountEnrollmentRequestAck lobjWssPersonAccountEnrollmentRequestAck = new busWssPersonAccountEnrollmentRequestAck();
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck = new cdoWssPersonAccountEnrollmentRequestAck();

            DataTable ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='" + astrQuersyString + "'");
            int iintack_id = 0;
            if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                iintack_id = Convert.ToInt32(ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_id"]);
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id = iintack_id;
            if (astrQuersyString == busConstant.ConfirmationString)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrConfirmationText;
            else if (astrQuersyString == busConstant.General || astrQuersyString == busConstant.General2)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrIAgree;
            else if (astrQuersyString == busConstant.DBElectedOfficialGeneral1)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrSFN53405PartBWaiverAuthorization;
            else if (astrQuersyString == busConstant.Misc)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = icdoWssPersonAccountEnrollmentRequest.comments;
            else if (astrQuersyString == busConstant.GHDVWaiveCheck)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrWaive;
            else if (astrQuersyString == busConstant.HighDeductibleAcknowledgement)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrHDHPConfirmationText;
            //PIR 25920 insert ack while selecting additional contribution only.
            //else if (astrQuersyString == busConstant.PermAdditionalContributionElectionAck)
            //    lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrADECAckText;
            else if (astrQuersyString == busConstant.PermAdditionalContributionElectionInfo)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrADECInfoAckText;

            else if (astrQuersyString == busConstant.AlternateStructureCodeHDHP)  //19997
            {
                int lintTrack_Id = 0;
                DataTable ldtbListWSSHDHPAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "ACKNOWLEDGEMENT_ID=" + 149 );
                if(ldtbListWSSHDHPAcknowledgement.Rows.Count>0)
                    lintTrack_Id = Convert.ToInt32(ldtbListWSSHDHPAcknowledgement.Rows[0]["acknowledgement_id"]);
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_id = lintTrack_Id;
                //lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = istrHDHPAchknowledgeText; Not needed to store text it has no parenthesis.
            }
            else if (astrQuersyString == busConstant.HealthPlanAcknowledgement)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = astrAcknowText;
            else if (astrQuersyString == busConstant.DentalPlanAcknowledgement)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = astrAcknowText;
            else if (astrQuersyString == busConstant.VisionPlanAcknowledgement)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = astrAcknowText;
            else if (astrQuersyString == busConstant.MedicarePlanAcknowledgement)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = astrAcknowText;
            else if (astrQuersyString == busConstant.LifePlanAcknowledgement)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = astrAcknowText;
            else if (astrQuersyString == busConstant.FlexPlanAcknowledgement)
                lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.acknowledgement_text = astrAcknowText;
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck.Insert();
        }

        #endregion

        // PIR 11840
        public Collection<busWssPersonDependent> iclbMSSPersonDependentTemp { get; set; }

        public Collection<busWssPersonDependent> iclbMSSPersonDependentPrevious { get; set; }


        public void LoadPersonEmploymentDetail()
        {
            if (ibusPersonEmploymentDetail.IsNull())
                ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
            ibusPersonEmploymentDetail.FindPersonEmploymentDetail(icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id);
            ibusPersonEmploymentDetail.LoadMemberType();
        }

        public void LoadPlan()
        {
            if (ibusPlan.IsNull())
                ibusPlan = new busPlan();

            ibusPlan.FindPlan(icdoWssPersonAccountEnrollmentRequest.plan_id);
        }

        //load ee acknowledgement
        public override void SetParentKey(Sagitec.DataObjects.doBase aobjBase)
        {
            if (aobjBase is cdoWssPersonAccountEeAcknowledgement)
            {
                cdoWssPersonAccountEeAcknowledgement lcdoEeAcknowledgement = (cdoWssPersonAccountEeAcknowledgement)aobjBase;
                lcdoEeAcknowledgement.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
            }
        }

        //get amounts that need to replaced in the description       
        public void LoadAmountsToBeReplaced()
        {
            DataTable ldtbList = Select<cdoPlanRetirementRate>(
                               new string[1] { "MEMBER_TYPE_VALUE" },
                               new object[1] { busConstant.MemberTypeMainTemp }, null, "EFFECTIVE_DATE desc");
            if (ldtbList.Rows.Count > 0)
            {
                busPlanRetirementRate lPlanRetirementRate = new busPlanRetirementRate { icdoPlanRetirementRate = new cdoPlanRetirementRate() };
                lPlanRetirementRate.icdoPlanRetirementRate.LoadData(ldtbList.Rows[0]);
                idecAmountToBeReplacedForAC10 = lPlanRetirementRate.icdoPlanRetirementRate.ee_pre_tax + lPlanRetirementRate.icdoPlanRetirementRate.ee_post_tax + lPlanRetirementRate.icdoPlanRetirementRate.ee_rhic + lPlanRetirementRate.icdoPlanRetirementRate.ee_emp_pickup;
            }
            DataTable ldtbListdtDTP = Select<cdoPlanRetirementRate>(
                              new string[1] { "MEMBER_TYPE_VALUE" },
                              new object[1] { busConstant.MemberTypeDCTemp }, null, "EFFECTIVE_DATE desc");
            if (ldtbListdtDTP.Rows.Count > 0)
            {
                busPlanRetirementRate lPlanRetirementRateDTP = new busPlanRetirementRate { icdoPlanRetirementRate = new cdoPlanRetirementRate() };
                lPlanRetirementRateDTP.icdoPlanRetirementRate.LoadData(ldtbListdtDTP.Rows[0]);
                idecAmountToBeReplacedForACK5 = lPlanRetirementRateDTP.icdoPlanRetirementRate.ee_pre_tax + lPlanRetirementRateDTP.icdoPlanRetirementRate.ee_post_tax + lPlanRetirementRateDTP.icdoPlanRetirementRate.ee_rhic + lPlanRetirementRateDTP.icdoPlanRetirementRate.ee_emp_pickup;
                idecAmountToBeReplacedForACK91 = lPlanRetirementRateDTP.icdoPlanRetirementRate.ee_emp_pickup + lPlanRetirementRateDTP.icdoPlanRetirementRate.ee_pre_tax + lPlanRetirementRateDTP.icdoPlanRetirementRate.ee_post_tax;
                idecAmountToBeReplacedForACK92 = lPlanRetirementRateDTP.icdoPlanRetirementRate.ee_rhic;
            }
            //if (ibusPersonAccount.IsNull())
            //    LoadPersonAccount();
            //if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
            //{
            //    if (ibusPersonAccount.iclbRetirementContributionAll.IsNull())
            //        ibusPersonAccount.LoadRetirementContributionAll();

            //    foreach (busPersonAccountRetirementContribution lobj in ibusPersonAccount.iclbRetirementContributionAll)
            //    {
            //        if (lobj.ibusPersonEmploymentDetail.IsNull())
            //            lobj.LoadPersonEmploymentDetail();
            //    }
            //    if (ibusPersonAccount.iclbRetirementContributionAll.Count > 0)
            //    {
            //        idecAmountToBeReplacedForACK91 = ibusPersonAccount.iclbRetirementContributionAll
            //            .Where(lobjRetCont => lobjRetCont.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value == busConstant.MemberTypeDCTemp)
            //            .Sum(lobjRetCon => lobjRetCon.icdoPersonAccountRetirementContribution.pre_tax_ee_amount
            //            + lobjRetCon.icdoPersonAccountRetirementContribution.post_tax_ee_amount + lobjRetCon.icdoPersonAccountRetirementContribution.ee_er_pickup_amount);

            //        idecAmountToBeReplacedForACK92 = ibusPersonAccount.iclbRetirementContributionAll
            //             .Where(lobjRetCont => lobjRetCont.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value == busConstant.MemberTypeDCTemp)
            //            .Sum(lobjRetCon => lobjRetCon.icdoPersonAccountRetirementContribution.ee_rhic_amount);

            //        idecAmountToBeReplacedForACK5 = ibusPersonAccount.iclbRetirementContributionAll
            //             .Where(lobjRetCont => lobjRetCont.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value == busConstant.MemberTypeMainTemp)
            //            .Sum(lobjRetCon => lobjRetCon.icdoPersonAccountRetirementContribution.pre_tax_ee_amount + lobjRetCon.icdoPersonAccountRetirementContribution.post_tax_ee_amount
            //                + lobjRetCon.icdoPersonAccountRetirementContribution.ee_er_pickup_amount + lobjRetCon.icdoPersonAccountRetirementContribution.ee_rhic_amount);

            //        idecAmountToBeReplacedForAC10 = ibusPersonAccount.iclbRetirementContributionAll.Sum(lobjRetCon => lobjRetCon.icdoPersonAccountRetirementContribution.pre_tax_ee_amount
            //                  + lobjRetCon.icdoPersonAccountRetirementContribution.post_tax_ee_amount + lobjRetCon.icdoPersonAccountRetirementContribution.ee_er_pickup_amount
            //                  + lobjRetCon.icdoPersonAccountRetirementContribution.ee_rhic_amount);
            //    }
            //}
        }

        public bool IsEffectiveDateFutureDate()
        {
            //PIR 17472 - Date of change cannot be future date for flex and 'Qualified change in status'
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex && icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueQualifiedChangeInStatus && icdoWssPersonAccountEnrollmentRequest.date_of_change > DateTime.Now)
            {
                return true;
            }
            return false;
        }

        public void CoverageAmountCheckforEOI()
        {
            iblnIsLifeAmountGreater = false;
            DataTable ldtbList = Select<cdoPersonAccountLifeOption>(
               new string[1] { "person_account_id" },
               new object[1] { ibusPersonAccount.icdoPersonAccount.person_account_id }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                Collection<busPersonAccountLifeOption> lclbPersonAccountLifeOption = GetCollection<busPersonAccountLifeOption>(ldtbList, "icdoPersonAccountLifeOption");
                foreach (busPersonAccountLifeOption lobjPersonAccountLifeOption in lclbPersonAccountLifeOption)
                {
                    if (lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == "SPML")
                    {
                        if ((lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.coverage_amount < ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount)
                            && (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount - lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.coverage_amount) > 5000)
                            iblnIsLifeAmountGreater = true;

                    }

                    if (lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == "SPSL")
                    {
                        if ((ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount > 500000) &&
                            (lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.coverage_amount < ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount)
                            && ((ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount - lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.coverage_amount) > 5000))
                        {
                            iblnIsLifeAmountGreater = true;
                        }
                    }
                }
            }
        }

        public override void BeforePersistChanges()
        {
            ArrayList larrList = new ArrayList();
            utlError lutlError = new utlError();
            if (iblnIsLifeAmountGreater)
            {                
                string lstrErrorMessage = "Please complete SFN-58859 to complete your enrollment";
                lutlError.istrErrorMessage = lstrErrorMessage;
                larrList.Add(lutlError);
            }
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth
                || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental
                || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
            {
                if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value != "WAIV")
                {
                    if (iclbGHDVAcknowledgement.Count > 0)
                    {
                        for (int i = iarrChangeLog.Count - 1; i >= 0; i--)//PIR 6961
                        {
                            if (iarrChangeLog[i] is cdoWssAcknowledgement)
                            {
                                cdoWssAcknowledgement lcdoWssAcknowledgement = (cdoWssAcknowledgement)iarrChangeLog[i];
                                iarrChangeLog.Remove(lcdoWssAcknowledgement);
                            }
                        }
                    }
                }
                if (iclbMSSDependents.IsNotNull() && iclbMSSDependents.Count > 0)
                {
                    for (int i = iarrChangeLog.Count - 1; i >= 0; i--)//PIR 6961
                    {
                        if (iarrChangeLog[i] is cdoWssPersonDependent)
                        {
                            cdoWssPersonDependent lcdoWssPersonDependent = (cdoWssPersonDependent)iarrChangeLog[i];
                            iarrChangeLog.Remove(lcdoWssPersonDependent);
                        }
                    }
                }
                //pir : 6341 start
                if (iclbMSSOtherCoverageDetail.Count() > 0)
                {
                    for (int i = iarrChangeLog.Count - 1; i >= 0; i--)//PIR 6961
                    {
                        if (iarrChangeLog[i] is cdoWssPersonAccountOtherCoverageDetail)
                        {
                            cdoWssPersonAccountOtherCoverageDetail lcdoWssOtherCoverageDetail = (cdoWssPersonAccountOtherCoverageDetail)iarrChangeLog[i];
                            iarrChangeLog.Remove(lcdoWssOtherCoverageDetail);
                        }
                    }
                    iclbMSSOtherCoverageDetail.ForEach(lobj => lobj.icdoWssPersonAccountOtherCoverageDetail.provider_org_id = LoadProviderOrgID(lobj.icdoWssPersonAccountOtherCoverageDetail.provider_org_name));
                }
                if (iclbMSSWorkerCompensation.Count() > 0)
                {
                    for (int i = iarrChangeLog.Count - 1; i >= 0; i--)//PIR 6961
                    {
                        if (iarrChangeLog[i] is cdoWssPersonAccountWorkerCompensation)
                        {
                            cdoWssPersonAccountWorkerCompensation lcdoWssWorkersComp = (cdoWssPersonAccountWorkerCompensation)iarrChangeLog[i];
                            iarrChangeLog.Remove(lcdoWssWorkersComp);
                        }
                    }
                    iclbMSSWorkerCompensation.ForEach(lobj => lobj.icdoWssPersonAccountWorkerCompensation.provider_org_id = LoadProviderOrgID(lobj.icdoWssPersonAccountWorkerCompensation.company_name));
                }
                //end
            }
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
            {
                if (iclbLifeAcknowledgement.Count > 0)
                {
                    for (int i = iarrChangeLog.Count - 1; i >= 0; i--)//PIR 6961
                    {
                        if (iarrChangeLog[i] is cdoWssAcknowledgement)
                        {
                            cdoWssAcknowledgement lcdoWssAcknowledgement = (cdoWssAcknowledgement)iarrChangeLog[i];
                            iarrChangeLog.Remove(lcdoWssAcknowledgement);
                        }
                    }
                }

                // Supplemental amount should be displayed with the addition of Basic coverage amount

                if (icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id == 0)
                {
                    if (istrSupplementalWaiverFlag == busConstant.Flag_No)
                    {
                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount -= ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                    }
                    if (idecSupplementalAmount > 0 && ibusMSSLifeOption.icdoWssPersonAccountLifeOption.wss_person_account_life_option_id == 0 && icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag == busConstant.Flag_No)
                    {
                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount -= ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                    }
                    //Update case - No is selected in first step of wizard, then supp amount is not changed.
                    if (idecSupplementalAmount > 0 && (istrSupplementalWaiverFlag == null || istrSupplementalWaiverFlag == busConstant.LifeInsuranceFlagValue))
                    {
                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount = idecSupplementalAmount - ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                    }
                }
                else if (icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
                {
                    if (istrSupplementalWaiverFlag == busConstant.Flag_No)
                    {
                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount -= ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                    }
                    //Update case - No is selected in first step of wizard, then supp amount is not changed.
                    if (idecSupplementalAmount > 0 && (istrSupplementalWaiverFlag == null || istrSupplementalWaiverFlag == busConstant.LifeInsuranceFlagValue))
                    {
                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount = idecSupplementalAmount - ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                    }
                }
                //PIR-6198
                CoverageAmountCheckforEOI();

            }
            if (String.IsNullOrEmpty(icdoWssPersonAccountEnrollmentRequest.enrollment_type_value))
                SetEnrollmentTypes();

            if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
            {
                LoadTotalSalaryRedirectionForPlanYearForDependent();
                LoadTotalSalaryRedirectionForPlanYearForMedical();
            }

            iobjObjectStateBeforePersist = icdoWssPersonAccountEnrollmentRequest.ienuObjectState;
            if (iblnFinishButtonClicked || (icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ChangeReasonAnnualEnrollment && icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ChangeReasonACAAnnualEnrollment))
                icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPendingRequest;
            else
                icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusFinishLater;

            // PIR 9966
            if ((icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment || icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonACAAnnualEnrollment) &&
                icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag == busConstant.Flag_No)
            {
                //PIR 25344 If no new history line is inserted, set enrollment request status as processed.
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth 
                    || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
                    icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.StatusProcessed;
                else
                    icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
            }
            if (icdoWssPersonAccountEnrollmentRequest.ienuObjectState == ObjectState.Select)
            {
                icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Update;
                iarrChangeLog.Add(icdoWssPersonAccountEnrollmentRequest);
            }
            if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
            {
                SetEnrollmentValues();
                //PIR 25920 DC 2025 changes
                if (iblnIsDC25PlanSelectionCarryForward)
                {
                    if(IsTemporaryEmployee)
                        icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent_temp = iintOldAddlEEContributionPercent;
                    else
                        icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent = iintOldAddlEEContributionPercent;
                }
                else
                {
                    if (IsTemporaryEmployee)
                        icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent_temp = iintAddlEEContributionPercent;
                    else
                        icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent = iintAddlEEContributionPercent;
                }

                if ((icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC ||
                    icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2020) && //PIR 20232
                    icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCOptional)
                {
                    foreach (cdoWssPersonAccountEeAcknowledgement lcdoAcknowledge in iclcEeAcknowledgement)
                    {
                        lcdoAcknowledge.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                    }
                    for (int i = iarrChangeLog.Count - 1; i >= 0; i--)
                    {
                        if (iarrChangeLog[i] is cdoWssPersonAccountEeAcknowledgement)
                        {
                            cdoWssPersonAccountEeAcknowledgement lcdoCodeValue = (cdoWssPersonAccountEeAcknowledgement)iarrChangeLog[i];
                            iarrChangeLog.Remove(lcdoCodeValue);
                        }
                    }
                }
                else if ((icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain2020 ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2025)//PIR 20232, 25920 ?code
                && icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBOptional)
                {
                    foreach (busWssAcknowledgement lcdoEEAcknowledgement in iclbEEAcknowledgement)
                    {
                        cdoWssPersonAccountEnrollmentRequestAck lobjEEAcknowledgement = new cdoWssPersonAccountEnrollmentRequestAck();
                        lobjEEAcknowledgement.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                        lobjEEAcknowledgement.acknowledgement_id = lcdoEEAcknowledgement.icdoWssAcknowledgement.acknowledgement_id;
                        lobjEEAcknowledgement.acknowledgement_text = lcdoEEAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
                        lobjEEAcknowledgement.Insert();
                    }
                }
            }
            else if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
            {
                if (iarrChangeLog.Count() > 0)
                {
                    foreach (object lobj in iarrChangeLog)
                    {
                        if (lobj is cdoWssPersonAccountEnrollmentRequest)
                        {
                            cdoWssPersonAccountEnrollmentRequest lobjRequest = (cdoWssPersonAccountEnrollmentRequest)lobj;
                            if (lobjRequest.ienuObjectState == Sagitec.Common.ObjectState.Insert)
                                lobjRequest.Insert();
                            else if (lobjRequest.ienuObjectState == Sagitec.Common.ObjectState.Update)
                                lobjRequest.Update();
                        }
                    }
                    foreach (object lobj in iarrChangeLog)
                    {
                        if (lobj is cdoWssPersonAccountGhdv)
                        {
                            cdoWssPersonAccountGhdv lobjGHDV = (cdoWssPersonAccountGhdv)lobj;
                            if (lobjGHDV.ienuObjectState == Sagitec.Common.ObjectState.Insert)
                            {
                                lobjGHDV.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                                lobjGHDV.Insert();
                            }
                            else if (lobjGHDV.ienuObjectState == Sagitec.Common.ObjectState.Update)
                                lobjGHDV.Update();
                        }
                    }
                    foreach (object lobj in iarrChangeLog)
                    {
                        if (lobj is cdoWssPersonAccountLifeOption)
                        {
                            cdoWssPersonAccountLifeOption lobjLife = (cdoWssPersonAccountLifeOption)lobj;
                            if (lobjLife.ienuObjectState == Sagitec.Common.ObjectState.Insert)
                            {
                                lobjLife.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                                lobjLife.Insert();
                            }
                            else if ((lobjLife.ienuObjectState == Sagitec.Common.ObjectState.Update) || (lobjLife.ienuObjectState == Sagitec.Common.ObjectState.Select))
                                lobjLife.Update();
                        }
                    }
                }
            }
            else if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
            {
                foreach (object lobj in iarrChangeLog)
                {
                    if (lobj is cdoWssPersonAccountEnrollmentRequest)
                    {
                        cdoWssPersonAccountEnrollmentRequest lobjRequest = (cdoWssPersonAccountEnrollmentRequest)lobj;
                        if (lobjRequest.ienuObjectState == Sagitec.Common.ObjectState.Insert)
                            lobjRequest.Insert();
                        else if (lobjRequest.ienuObjectState == Sagitec.Common.ObjectState.Update)
                            lobjRequest.Update();
                    }
                    if (lobj is cdoWssPersonAccountFlexCompOption)
                    {
                        cdoWssPersonAccountFlexCompOption lobjFlexCompOptions = (cdoWssPersonAccountFlexCompOption)lobj;
                        lobjFlexCompOptions.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                        if (lobjFlexCompOptions.ienuObjectState == Sagitec.Common.ObjectState.Insert)
                            lobjFlexCompOptions.Insert();
                        else if (lobjFlexCompOptions.ienuObjectState == Sagitec.Common.ObjectState.Update)
                            lobjFlexCompOptions.Update();
                    }
                    if (lobj is cdoWssPersonAccountFlexCompConversion)
                    {
                        cdoWssPersonAccountFlexCompConversion lobjFlexCompConversion = (cdoWssPersonAccountFlexCompConversion)lobj;
                        if (lobjFlexCompConversion.istrIsSelected == busConstant.Flag_Yes)
                        {
                            lobjFlexCompConversion.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                            if (lobjFlexCompConversion.ienuObjectState == Sagitec.Common.ObjectState.Insert)
                                lobjFlexCompConversion.Insert();
                            else if (lobjFlexCompConversion.ienuObjectState == Sagitec.Common.ObjectState.Update)
                                lobjFlexCompConversion.Update();
                        }
                        else// pir : 6290 start
                        {
                            if (lobjFlexCompConversion.ienuObjectState == Sagitec.Common.ObjectState.Update)
                            {
                                lobjFlexCompConversion.Delete();
                            }//end
                            else
                                lobjFlexCompConversion.ienuObjectState = Sagitec.Common.ObjectState.Select;
                        }
                    }
                }
            }
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex && !(icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment))
            {
                // PIR 17472 - If the change effective date falls after 20th, date of change should be two months post date.
                //This validation only applies when change effective date is not a future date.
                if (icdoWssPersonAccountEnrollmentRequest.date_of_change < DateTime.Now)
                {
                    if (DateTime.Now.Day <= 20)
                        icdoWssPersonAccountEnrollmentRequest.date_of_change = icdoWssPersonAccountEnrollmentRequest.created_date.GetFirstDayofNextMonth();
                    else
                        icdoWssPersonAccountEnrollmentRequest.date_of_change = icdoWssPersonAccountEnrollmentRequest.created_date.AddMonths(1).GetFirstDayofNextMonth();
                }
                icdoWssPersonAccountEnrollmentRequest.Update();
            }

            base.BeforePersistChanges();
        }
        public override int PersistChanges()
        {
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
            {
                //PIR 10695 "Save & Quit" functionality
                if (!iblnFinishButtonClicked && (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment || icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonACAAnnualEnrollment))
                {
                    iclbWSSDependent = new Collection<busWssPersonDependent>();
                    foreach (busWssPersonDependent lobjPersonDependent in iclbMSSPersonDependent)
                        iclbWSSDependent.Add(lobjPersonDependent);
                }
                //PIR 27247 
                if (IsCoverageSingle())
                {
                    iclbWSSDependent = new Collection<busWssPersonDependent>();
                    foreach (busWssPersonDependent lobjPersonDependent in iclbMSSPersonDependent)
                    {
                        if (lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value != busConstant.PlanOptionStatusValueWaived &&
                            lobjPersonDependent.icdoWssPersonDependent.wss_person_dependent_id > 0)
                        {
                            iclbWSSDependent.Add(lobjPersonDependent);
                        }
                    }
                }

                if (iclbWSSDependent.IsNotNull()) // PIR 11896 Null check handled
                {
                    foreach (busWssPersonDependent lobjWSSPersonDependent in iclbWSSDependent)
                    {
                        lobjWSSPersonDependent.icdoWssPersonDependent.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                        if (lobjWSSPersonDependent.icdoWssPersonDependent.wss_person_dependent_id == 0 || istrCancelEnrollmentFlag == busConstant.Flag_Yes) // PIR 10402
                        {
                            lobjWSSPersonDependent.icdoWssPersonDependent.Insert();
                        }
                        else
                            lobjWSSPersonDependent.icdoWssPersonDependent.Update();
                    }
                }

                foreach (busWssPersonAccountOtherCoverageDetail lobjOtherCoverages in iclbMSSOtherCoverageDetail)
                {
                    lobjOtherCoverages.icdoWssPersonAccountOtherCoverageDetail.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                    if (lobjOtherCoverages.icdoWssPersonAccountOtherCoverageDetail.wss_person_account_other_coverage_detail_id == 0)
                        lobjOtherCoverages.icdoWssPersonAccountOtherCoverageDetail.Insert();
                    else
                        lobjOtherCoverages.icdoWssPersonAccountOtherCoverageDetail.Update();
                }

                foreach (busWssPersonAccountWorkerCompensation lobjWorkersCompensation in iclbMSSWorkerCompensation)
                {
                    lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                    if (lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.wss_person_account_worker_compensation_id == 0)
                        lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.Insert();
                    else
                        lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.Update();
                }
            }
            return base.PersistChanges();
        }

        public bool iblnIsDependentAddedForGHDV { get; set; }
        public bool iblnIsDependentEndDatedForDV { get; set; }

        public override void InvokeEvaluateLoadRules(busBase aobjChildObject)
        {
            if (aobjChildObject is busWssPersonDependent)
            {
                if (aobjChildObject != null)
                {
                    busWssPersonAccountEnrollmentRequest lbusWssPersonAccountEnrollmentRequest = this;
                    busWssPersonDependent lbusWssPersonDependent = (busWssPersonDependent)aobjChildObject;
                    if (lbusWssPersonDependent.icdoWssPersonDependent.relationship_value.Equals(busConstant.DependentRelationshipGrandChild))
                        this.icdoWssPersonAccountEnrollmentRequest.iblnIsGrandChildAdded = true;
                    else
                        this.icdoWssPersonAccountEnrollmentRequest.iblnIsGrandChildAdded = false;
                    lbusWssPersonDependent.icdoWssPersonDependent.iblnResult = true;
                    lbusWssPersonDependent.icdoWssPersonDependent.iintPlanId = this.icdoWssPersonAccountEnrollmentRequest.plan_id;

                    if (lbusWssPersonDependent.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipGrandChild ||
                        lbusWssPersonDependent.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipAdoptiveChild ||
                        lbusWssPersonDependent.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipLegalGuardian)
                        iblnIsDependentAddedForGHDV = true;
                    else if(this.icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ChangeReasonAnnualEnrollment &&
                        (lbusWssPersonDependent.icdoWssPersonDependent.iintPlanId == busConstant.PlanIdDental 
                        || lbusWssPersonDependent.icdoWssPersonDependent.iintPlanId == busConstant.PlanIdVision) 
                        && lbusWssPersonDependent.icdoWssPersonDependent.effective_end_date == DateTime.MinValue
                        && lbusWssPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueWaived)
                        iblnIsDependentEndDatedForDV = true;
                }
            }
            base.InvokeEvaluateLoadRules(aobjChildObject);
        }

        //pir 6341
        public int LoadProviderOrgID(string astrProviderOrgName)
        {
            int lintProviderOrgID = 0;
            if (!astrProviderOrgName.IsNullOrEmpty())
            {
                DataTable ltdbProviders = SelectWithOperator<cdoOrganization>(new string[3] { "ORG_TYPE_VALUE", "STATUS_VALUE", "ORG_NAME".ToUpper() },
                                                            new string[3] { "=", "=", "=" },
                                                            new object[3] { busConstant.OrgTypeProvider, busConstant.StatusActive, astrProviderOrgName.ToUpper() },
                                                            null);
                busOrganization lbusProvider = GetCollection<busOrganization>(ltdbProviders, "icdoOrganization").FirstOrDefault();
                if (lbusProvider.IsNotNull())
                    lintProviderOrgID = lbusProvider.icdoOrganization.org_id;
            }
            return lintProviderOrgID;
        }

        //job class is ‘Career & Tech Ed Certified Teacher’        
        public void SetJobClassValue()
        {
            iblnIsJobClassCareerAndtechEdCertifiedTeacher = false;
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();

            if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassCareerAndTechEdCertifiedTeacher)
                iblnIsJobClassCareerAndtechEdCertifiedTeacher = true;

            if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassDeptofPublicInstructionCertifiedTeacher)
                iblnIsJobClassDeptOfPublicInstructionCertifiedTeacher = true;

            if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == "HPPN" || ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == "NSEM" ||
                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == "SUPR" || ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == "CORO" ||
                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == "PEAO")
                iblnIsJobClassRetirementPlan = true;
        }

        public bool IsJobClassStateorNonStateAndContributing()
        {
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();

            if ((ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial ||
                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassNonStateElectedOfficial) &&
                 ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing)
                return true;
            return false;
        }

        public bool IsJobClassStateorNonStateAndNonContributing()
        {
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();

            if ((ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial ||
                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassNonStateElectedOfficial) &&
                 ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusNonContributing)
                return true;
            return false;
        }

        public void LoadSFN53405PartBWaiverAuthorization()
        {
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

            DataTable ldtbWSSAcknowledgement = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.DBElectedOfficialGeneral1 });
            busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
            lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
            if (ldtbWSSAcknowledgement.Rows.Count > 0)
                lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbWSSAcknowledgement.Rows[0]["acknowledgement_text"].ToString();

            istrSFN53405PartBWaiverAuthorization = string.Format(lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text,
                ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_name,
                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.term_begin_date);//PIR 6961

            //istrSFN53405PartBWaiverAuthorization = "1.	I am an ELECTED official of " + ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_name +
            //  ", and my present term started " + ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.term_begin_date + ".  The title of the position I was elected to is  &nbsp;&nbsp;";
        }

        public override void AfterPersistChanges()
        {
            SetEnrollmentTypes();
            if (ibusPlan.IsNull()) LoadPlan();
            base.AfterPersistChanges();
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth && ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonEmplyDetailType)
            {
                //if (ibusPerson.FindPerson(icdoWssPersonAccountEnrollmentRequest.person_id))
                //{
                ibusPerson.LoadACAEligibilityCertification(ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id);
                //}
            }
            if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
            {
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassNonStateElectedOfficial
                    || ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value == busConstant.PersonJobClassStateElectedOfficial)
                {
                    DateTime ldtTobeCompared = DateTime.MinValue;

                    if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                    {
                        ldtTobeCompared = DateTime.Now;
                    }
                    else
                    {
                        ldtTobeCompared = ibusPersonAccount.icdoPersonAccount.start_date;
                    }
                    if (busGlobalFunctions.CheckDateOverlapping(ldtTobeCompared,
                                                             ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                                                                 ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddMonths(6)))
                    {
                        if (ibusPersonEmploymentDetail.IsNull())
                            LoadPersonEmploymentDetail();
                        if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                            ibusPersonEmploymentDetail.LoadPersonEmployment();

                        // PIR 9115
                        if (istrIsPIR9115Enabled == busConstant.Flag_No)
                        {
                            string lstrPrioityValue = string.Empty;
                            busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(2, iobjPassInfo, ref lstrPrioityValue), ibusPerson.icdoPerson.FullName, ibusPlan.icdoPlan.mss_plan_name),
                                lstrPrioityValue,
                                aintPlanID: icdoWssPersonAccountEnrollmentRequest.plan_id,
                                aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                        }
                        else
                        {
                            busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                                icdoWssPersonAccountEnrollmentRequest.plan_id, iobjPassInfo);
                        }

                    }
                }
            }

            busWssPersonAccountEnrollmentRequestAck lobjWssPersonAccountEnrollmentRequestAck = new busWssPersonAccountEnrollmentRequestAck();
            lobjWssPersonAccountEnrollmentRequestAck.icdoWssPersonAccountEnrollmentRequestAck = new cdoWssPersonAccountEnrollmentRequestAck();

            DataTable ldtbRequestAck = Select<cdoWssPersonAccountEnrollmentRequestAck>(new string[1] { "wss_person_account_enrollment_request_id" },
                new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
            if (ldtbRequestAck.Rows.Count > 0)
            {
                Collection<cdoWssPersonAccountEnrollmentRequestAck> iclbRequestAck = new Collection<cdoWssPersonAccountEnrollmentRequestAck>();
                iclbRequestAck = doBase.GetCollection<cdoWssPersonAccountEnrollmentRequestAck>(ldtbRequestAck);
                foreach (cdoWssPersonAccountEnrollmentRequestAck lobj in iclbRequestAck)
                {
                    lobj.Delete();
                }
            }

            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
            {
                SetVisibilityofLink();
                // Supplemental amount should be displayed with the addition of Basic coverage amount
                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount += ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;

                LoadAckCheckDetails();
                InsertCollection(iclbAcknowledgementCheck);
                InsertString(busConstant.ConfirmationString);

            }
            else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex)
            {

                if (icdoWssPersonAccountEnrollmentRequest.ee_acknowledgement_agreement_flag == busConstant.Flag_Yes)//only if i agree flag is selected
                    InsertCollection(iclbAcknowledgementMemAuth);
                else
                    InsertString(busConstant.ConfirmationString);
            }
            else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
            {
                // PIR 10402 incorrect acknowledgement issue
                bool lblnWaivedCancelledAck = false;
                if ((icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag == busConstant.Flag_Yes)
                    || (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueCancel
                        && icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment))
                    lblnWaivedCancelledAck = true;
                else if (icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag != busConstant.Flag_Yes)
                    lblnWaivedCancelledAck = false;

                if (!lblnWaivedCancelledAck)
                {
                    LoadAckCheckDetails();
                    InsertCollection(iclbAcknowledgementMemAuth);
                    InsertCollection(iclbGHDVAcknowledgement);
                }
                else if (lblnWaivedCancelledAck)
                {
                    InsertCollection(iclbAcknowledgementGen);
                    if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value != busConstant.PlanEnrollmentOptionValueCancel) // PIR 10402 incorrect acknowledgement issue
                        InsertString(busConstant.GHDVWaiveCheck);
                }
                if (icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP)
                {
                    InsertCollection(iclbHDHPAcknowledgement);
                    InsertString(busConstant.HighDeductibleAcknowledgement);
                    if (istrPreTaxHSA == "Y")
                         InsertString(busConstant.AlternateStructureCodeHDHP);  //19997
                }
                InsertString(busConstant.ConfirmationString);
            }
            else if (ibusPlan.IsRetirementPlan())
            {
                if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBRetirement)
                {
                    InsertCollection(iclbAcknowledgementMemAuth);

                    if (iblnIsDCEligible)
                        InsertString(busConstant.MainNotice1);
                    else if (iblnIsJobClassCareerAndtechEdCertifiedTeacher)
                        InsertString(busConstant.MainNotice2);
                    else if (iblnIsJobClassDeptOfPublicInstructionCertifiedTeacher)
                        InsertString(busConstant.MainNotice3);
                    else if (iblnIsJobClassRetirementPlan)
                        InsertString(busConstant.DBRetirement);
                    if (icdoWssPersonAccountEnrollmentRequest.ee_acknowledgement_agreement_flag == busConstant.Flag_Yes)//only if i agree flag is selected
                        InsertString(busConstant.General);
                    InsertString(busConstant.ConfirmationString);
                }
                else if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBElectedOfficial)
                {
                    if (icdoWssPersonAccountEnrollmentRequest.ee_acknowledgement_agreement_flag == busConstant.Flag_Yes)
                    {
                        InsertCollection(iclbAcknowledgementMemAuth);
                        if (iblnIsDCEligible)
                            InsertString(busConstant.DBRetirementEnrollmentNotice1);
                        else if (iblnIsJobClassCareerAndtechEdCertifiedTeacher)
                            InsertString(busConstant.DBRetirementEnrollmentNotice2);
                        else if (iblnIsJobClassDeptOfPublicInstructionCertifiedTeacher)
                            InsertString(busConstant.DBRetirementEnrollmentNotice3);
                        InsertString(busConstant.General);
                    }
                    if (icdoWssPersonAccountEnrollmentRequest.ee_acknowledgement_waiver_flag == busConstant.Flag_Yes)
                    {
                        InsertString(busConstant.DBElectedOfficialGeneral1);
                        InsertString(busConstant.Misc);
                        InsertCollection(iclbAcknowledgementWaiverAuth);
                        InsertString(busConstant.General);
                    }
                    InsertString(busConstant.ConfirmationString);
                }
                else if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBOptional)
                {
                    Collection<busWssAcknowledgement> lclbInsert = new Collection<busWssAcknowledgement>();
                    lclbInsert = LoadEEAcknowledgementCollection();
                    InsertCollection(lclbInsert);
                    if (icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_c_flag == busConstant.Flag_Yes)
                    {
                        InsertCollection(iclbCollGen1);
                        InsertString(busConstant.General);
                        InsertCollection(iclbCollGen2);
                        if (iblnIsJobClassCareerAndtechEdCertifiedTeacher)
                            InsertString(busConstant.MainRetirementEnrollmentNotice1);
                        else if (iblnIsJobClassDeptOfPublicInstructionCertifiedTeacher)
                            InsertString(busConstant.MainRetirementEnrollmentNotice2);
                        InsertCollection(iclbAcknowledgementMemAuth);
                        InsertString(busConstant.General2);
                    }
                    if (icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag == busConstant.Flag_Yes)
                    {
                        InsertCollection(iclbCollGen3);
                        InsertString(busConstant.General);
                    }
                    InsertString(busConstant.ConfirmationString);
                }
                else if (icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDCOptional)
                {
                    Collection<busWssAcknowledgement> lclbInsert = new Collection<busWssAcknowledgement>();
                    lclbInsert = LoadEEAcknowledgementCollection();
                    InsertCollection(lclbInsert);
                    if (icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_c_flag == busConstant.Flag_Yes)
                    {
                        InsertCollection(iclbCollGen1);
                        InsertString(busConstant.General);
                        InsertCollection(iclbCollGen2);
                        if (iblnIsJobClassCareerAndtechEdCertifiedTeacher)
                            InsertString(busConstant.DCRetirementEnrollmentNotice1);
                        else if (iblnIsJobClassDeptOfPublicInstructionCertifiedTeacher)
                            InsertString(busConstant.DCRetirementEnrollmentNotice2);
                        InsertCollection(iclbAcknowledgementMemAuth);
                        InsertString(busConstant.General2);
                    }
                    if (icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag == busConstant.Flag_Yes)
                    {
                        InsertCollection(iclbCollGen3);
                        InsertString(busConstant.General);
                    }
                    InsertString(busConstant.ConfirmationString);
                }
            }

            /// Auto Posting Logic
            //PIR 26799 auto_flag = 'N' when Plan Enrollment Option Value is Waived and date of change < DateTime.Today.
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
            {
                foreach (busWssPersonDependent lobjPersonDependent in iclbWSSDependent)
                {
                    if (lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueWaived &&
                        icdoWssPersonAccountEnrollmentRequest.date_of_change < DateTime.Today)
                    {
                        iblnAutoPostflagCheck = true;
                    }
                }
            }
            if (icdoAutoPostingCrossRef.IsNull()) FindAutoPostingCrossRef();
            if (icdoAutoPostingCrossRef.auto_post_flag == busConstant.Flag_Yes && ExceptionLOCForAutoPost && iblnAutoPostflagCheck == false)
            {
                iblnIsMemberAccountInReviewStatus = false;
                icdoWssPersonAccountEnrollmentRequest.change_effective_date = GetChangeEffectiveDate();
                if (IsghdvPlan)
                {
                    if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment || icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonACAAnnualEnrollment)
                    {
                        //PIR 15820
                        if (!iblnIsDependentAddedForGHDV)
                        {
                            if (iblnFinishButtonClicked)
                            {
                                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                                   icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
                                {
                                    if (icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag == busConstant.Flag_No &&
                                        icdoMSSGDHV.pre_tax_payroll_deduction_flag == busConstant.Flag_No && // PIR 10291 -- Prev Pretax 'N' and Current selection 'N', then only no history.
                                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.premium_conversion_indicator_flag == icdoMSSGDHV.pre_tax_payroll_deduction_flag)
                                    {
                                        // PIR 6699 Annual enrollment
                                        if (ibusPersonAccount.IsNull()) LoadPersonAccountForPosting();
                                        if (ibusMSSPersonAccountGHDV.IsNull()) LoadPersonAccountGHDV();
                                        ibusMSSPersonAccountGHDV.ibusHistory = ibusMSSPersonAccountGHDV.LoadHistoryByDate(AnnualEnrollmentEffectiveDate);

                                        ibusPersonAccount.icdoPersonAccount.history_change_date = AnnualEnrollmentEffectiveDate;
                                        ibusPersonAccount.icdoPersonAccount.Update();

                                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.premium_conversion_indicator_flag = icdoMSSGDHV.pre_tax_payroll_deduction_flag ?? busConstant.Flag_No;
                                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.reason_value = busConstant.AnnualEnrollment;
                                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.Update();

                                        if (ibusMSSPersonAccountGHDV.ibusHistory.IsNotNull() &&
                                            ibusMSSPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.start_date == AnnualEnrollmentEffectiveDate &&
                                            ibusMSSPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.premium_conversion_indicator_flag != icdoMSSGDHV.pre_tax_payroll_deduction_flag)
                                        {
                                            ibusMSSPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.premium_conversion_indicator_flag = icdoMSSGDHV.pre_tax_payroll_deduction_flag;
                                            ibusMSSPersonAccountGHDV.ibusHistory.icdoPersonAccountGhdvHistory.Update();
                                        }
                                    }
                                    else
                                    {
										//PIR 22950 1.	If requests cannot completely post that everything is reverted back
                                        if (iobjPassInfo.iblnInTransaction)
                                        {
                                            iobjPassInfo.Commit();
                                        }
                                        if (!iobjPassInfo.iblnInTransaction)
                                            iobjPassInfo.BeginTransaction();
                                        ArrayList larrPostGHDV = btnPostGHDV_Click();
                                        if (larrPostGHDV.IsNotNull() && larrPostGHDV.Count > 0 && larrPostGHDV[0] is utlError && iobjPassInfo.iblnInTransaction)
                                        {
                                            iobjPassInfo.Rollback();
                                        }
                                    }
                                }
                                else
                                {
                                    if (icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag == busConstant.Flag_Yes)
                                    {
                                        if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth && ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonEmplyDetailType)
                                        {
                                            if (ibusPerson.ibusWssEmploymentAcaCert.IsNotNull() && ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.IsNotNull() && ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.wss_employment_aca_cert_id > 0 &&
                                                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id == ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.per_emp_dtl_id)
                                            {
                                                //PIR 25392
                                                icdoWssPersonAccountEnrollmentRequest.change_effective_date = AnnualEnrollmentTempEffectiveDate;
                                                icdoWssPersonAccountEnrollmentRequest.Update();
                                            }
                                           
                                        }
                                        //PIR 22950 1.	If requests cannot completely post that everything is reverted back
                                        if (iobjPassInfo.iblnInTransaction)
                                        {
                                            iobjPassInfo.Commit();
                                        }
                                        if (!iobjPassInfo.iblnInTransaction)
                                            iobjPassInfo.BeginTransaction();
                                        ArrayList larrPostGHDV = btnPostGHDV_Click();
                                        if (larrPostGHDV.IsNotNull() && larrPostGHDV.Count > 0 && larrPostGHDV[0] is utlError && iobjPassInfo.iblnInTransaction)
                                        {
                                            iobjPassInfo.Rollback();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (!iblnIsDependentAddedForGHDV && !iblnIsDependentEndDatedForDV)
                    {
                        if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth && ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonEmplyDetailType)
                        {
                            if (ibusPerson.ibusWssEmploymentAcaCert.IsNotNull() && ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.IsNotNull() && ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id == ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.person_employment_id)
                            {
                                if (ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.method == busConstant.ACACertificationMethodLookBack && ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.lb_measure == busConstant.ACACertificationLookBackTypeAnnual)
                                {
                                    //DateTime ldtTempDate = ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.to_date;
                                    icdoWssPersonAccountEnrollmentRequest.change_effective_date = AnnualEnrollmentTempEffectiveDate;
                                    icdoWssPersonAccountEnrollmentRequest.Update();
                                }
                                else
                                {
                                    DateTime ldtTempDate = ibusPerson.ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.to_date;
                                    icdoWssPersonAccountEnrollmentRequest.change_effective_date = busGlobalFunctions.GetFirstDayofNextMonth(ldtTempDate);
                                    icdoWssPersonAccountEnrollmentRequest.Update();
                                }
                            }
                        }
                        //PIR 22950 1.	If requests cannot completely post that everything is reverted back
                        if (iobjPassInfo.iblnInTransaction)
                        {
                            iobjPassInfo.Commit();
                        }
                        if (!iobjPassInfo.iblnInTransaction)
                            iobjPassInfo.BeginTransaction();
                        ArrayList larrPostGHDV = btnPostGHDV_Click();
                        if (larrPostGHDV.IsNotNull() && larrPostGHDV.Count > 0 && larrPostGHDV[0] is utlError && iobjPassInfo.iblnInTransaction)
                        {
                            iobjPassInfo.Rollback();
                        }
                    }
                } //GHDV Ends here

                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
                {
                    iblnIsLifeAutoPosting = true;
                    if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment &&
                        icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag == busConstant.Flag_No)
                    {
                        // PIR 6699 Annual enrollment
                        if (ibusPersonAccount.IsNull())
                            LoadPersonAccountForPosting();

                        ibusPersonAccount.icdoPersonAccount.history_change_date = AnnualEnrollmentEffectiveDate;
                        ibusPersonAccount.icdoPersonAccount.Update();
                    }
                    else
                    {
						//PIR 22950 1.	If requests cannot completely post that everything is reverted back
                        if (iobjPassInfo.iblnInTransaction)
                        {
                            iobjPassInfo.Commit();
                        }
                        if (!iobjPassInfo.iblnInTransaction)
                            iobjPassInfo.BeginTransaction();
                        ArrayList larrPostLife = btnPostLife_Click();
                        if (larrPostLife.IsNotNull() && larrPostLife.Count > 0 && larrPostLife[0] is utlError && iobjPassInfo.iblnInTransaction)
                        {
                            iobjPassInfo.Rollback();
                        }
                    }
                    if (iblnIsEOIRequired && !IsWorkflowAlreadyExistForPerson(busConstant.MapProcessEvidenceofInsurability))
                    {
                        // Initiate workflow After rollback //PIR 22950 1.	If requests cannot completely post that everything is reverted back
                        if (!iobjPassInfo.iblnInTransaction)
                            iobjPassInfo.BeginTransaction();
                        //PIR 26406 When a Life enrollment requiring EOI is submitted from MSS, don't initiate Case ID 97.  Only the enrollment BPM should initiate.
                        //InitializeWorkflow(busConstant.MapProcessEvidenceofInsurability); 
                    }
                } //Life Plan Ends here

                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex)
                {
                    if (ibusPersonAccount.IsNull())
                        LoadPersonAccount();
                    if (ibusPersonAccount.ibusPersonAccountFlex.IsNull())
                        ibusPersonAccount.LoadPersonAccountFlex();

                    if ((icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.AnnualEnrollment || icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ChangeReasonNewHire)
                        || (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment && iblnFinishButtonClicked) 
                        || (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonNewHire && ibusPersonAccount.ibusPersonAccountFlex .IsPersonEmpDtlPrevLinkedEndDateNCurrentStartDateDiff31Days(icdoWssPersonAccountEnrollmentRequest.date_of_change)))
                    {
						//PIR 22950 1.	If requests cannot completely post that everything is reverted back
                        if (iobjPassInfo.iblnInTransaction)
                        {
                            iobjPassInfo.Commit();
                        }
                        if (!iobjPassInfo.iblnInTransaction)
                            iobjPassInfo.BeginTransaction();
                        ArrayList larrPostFlex = btnPostFlexComp_Click();
                        if (larrPostFlex.IsNotNull() && larrPostFlex.Count > 0 && larrPostFlex[0] is utlError && iobjPassInfo.iblnInTransaction)
                        {
                            iobjPassInfo.Rollback();
                        }
                    }
                }

                if (ibusPlan.IsRetirementPlan() || ibusPlan.IsDCRetirementPlan())
                {
                    if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
                    ibusPersonEmploymentDetail.SetMSSIsTempEmploymentDtlWithinFirstSixMonths(DateTime.Now);
                    if (ibusPersonEmploymentDetail.iblnIsMSSTempEmploymentDtlWithinFirstSixMonths)
                    {
						//PIR 22950 1.	If requests cannot completely post that everything is reverted back
                        if (iobjPassInfo.iblnInTransaction)
                        {
                            iobjPassInfo.Commit();
                        }
                        if (!iobjPassInfo.iblnInTransaction)
                            iobjPassInfo.BeginTransaction();
                        ArrayList larrPostRetirementOptional = btnRetirementOptionalPost_Click();
                        if (larrPostRetirementOptional.IsNotNull() && larrPostRetirementOptional.Count > 0 && larrPostRetirementOptional[0] is utlError && iobjPassInfo.iblnInTransaction)
                        {
                            iobjPassInfo.Rollback();
                        }
                    }
                    else
                    {
						//PIR 22950 1.	If requests cannot completely post that everything is reverted back
                        if (iobjPassInfo.iblnInTransaction)
                        {
                            iobjPassInfo.Commit();
                        }
                        if (!iobjPassInfo.iblnInTransaction)
                            iobjPassInfo.BeginTransaction();
                        ArrayList larrPostRetirement = btnRetirementPost_Click();
                        if (larrPostRetirement.IsNotNull() && larrPostRetirement.Count > 0 && larrPostRetirement[0] is utlError && iobjPassInfo.iblnInTransaction)
                        {
                            iobjPassInfo.Rollback();
                        }
                    }
                }
            }

            // Initiate workflow After rollback //PIR 22950 1.	If requests cannot completely post that everything is reverted back
            if (!iobjPassInfo.iblnInTransaction)
                iobjPassInfo.BeginTransaction();

            bool lblnWorkflowTriggered = false; // PIR 10952 Any time a WFL is already triggered because of some condition we donï¿½t need the generic WFL to trigger.
            if (iobjObjectStateBeforePersist == ObjectState.Insert &&
                icdoAutoPostingCrossRef.workflow_process_id > 0 &&
                ibusPlan.icdoPlan.plan_id != busConstant.PlanIdGroupLife &&
                !IsWorkflowAlreadyExistForPerson(icdoAutoPostingCrossRef.workflow_process_id)) // PIR 9702
            {
                lblnWorkflowTriggered = true;
                if (ibusPlan.IsRetirementPlan()) // PIR 9683
                {
                    if (icdoWssPersonAccountEnrollmentRequest.is_enrolled_in_tiaa_cref_flag == busConstant.Flag_Yes ||
                        icdoWssPersonAccountEnrollmentRequest.is_enrolled_in_tffr_flag == busConstant.Flag_Yes)
                        InitializeWorkflow(icdoAutoPostingCrossRef.workflow_process_id);
                }
                else
                    InitializeWorkflow(icdoAutoPostingCrossRef.workflow_process_id);
            }
            // PIR PIR 26168 new BPM for each request submitted, even if the same Case ID is already initiated for the same member.  
            if ((iblnIsMemberAccountInReviewStatus ||
                icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusPendingRequest ||
                icdoAutoPostingCrossRef.auto_post_flag == busConstant.Flag_No) && //!lblnWorkflowTriggered &&
                !(ibusPlan.icdoPlan.plan_id == busConstant.PlanIdFlex && icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive)) // PIR 10054
            {
                //PIR-10697 Start Change reason is Annual Enrollment then in that case We have trigger different Workflow
                if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment || icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonACAAnnualEnrollment)
                {
                    if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
                    {
                        if (iblnFinishButtonClicked && !(istrSupplementalWaiverFlag == busConstant.LifeInsuranceFlagValue && istrDependentWaiverFlag == busConstant.LifeInsuranceFlagValue
                            && istrSpouseWaiverFlag == busConstant.LifeInsuranceFlagValue) && icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag != busConstant.Flag_No) //No WFL should be created if there is no change in enrollment.
                        {
                            // PIR 26406 - When a Life enrollment requiring EOI is submitted from MSS, don't initiate Case ID 97.  Only the enrollment BPM should initiate.
                            //if (iblnIsEOIRequired)// && !IsWorkflowAlreadyExistForPerson(busConstant.MapProcessEvidenceofInsurability))
                            //    InitializeWorkflow(busConstant.MapProcessEvidenceofInsurability);
                            //if (!IsWorkflowAlreadyExistForPerson(busConstant.MSSLifeInsuranceAnnualEnrollment))
                            InitializeWorkflow(busConstant.MSSLifeInsuranceAnnualEnrollment);
                        }
                    }
                    else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental
                        || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
                    {
                        if (iblnFinishButtonClicked)  //!IsWorkflowAlreadyExistForPerson(busConstant.MapMSSHDVAnnualEnrollment) && 
                            InitializeWorkflow(busConstant.MapMSSHDVAnnualEnrollment);
                    }
                    else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex)
                    {
                        if (iblnFinishButtonClicked)  //!IsWorkflowAlreadyExistForPerson(busConstant.MSSFlexcompAnnualEnrollment) && 
                            InitializeWorkflow(busConstant.MSSFlexcompAnnualEnrollment);
                    }
                    else
                    {
                        //if (!IsWorkflowAlreadyExistForPerson(busConstant.MapWSSEnrollNewHireInPensionAndInsurancePlans))
                            InitializeWorkflow(busConstant.MapWSSEnrollNewHireInPensionAndInsurancePlans);
                    }
                }
                else
                {
                    // PIR 26406 - When a Life enrollment requiring EOI is submitted from MSS, don't initiate Case ID 97.  Only the enrollment BPM should initiate.
                    //if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
                    //{
                    //    //if (iblnIsEOIRequired) // && !IsWorkflowAlreadyExistForPerson(busConstant.MapProcessEvidenceofInsurability))
                    //    //    InitializeWorkflow(busConstant.MapProcessEvidenceofInsurability);
                    //    //if (!IsWorkflowAlreadyExistForPerson(busConstant.MapWSSEnrollNewHireInPensionAndInsurancePlans))
                    //        InitializeWorkflow(busConstant.MapWSSEnrollNewHireInPensionAndInsurancePlans);
                    //}

                    //if (!IsWorkflowAlreadyExistForPerson(busConstant.MapWSSEnrollNewHireInPensionAndInsurancePlans))
                    InitializeWorkflow(busConstant.MapWSSEnrollNewHireInPensionAndInsurancePlans);
                    
                }
            }
        }

        public DateTime GetChangeEffectiveDate()
        {
            if (icdoWssPersonAccountEnrollmentRequest.date_of_change.Day != 1)
            {
                if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueBirth &&
                    (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife ||
                    icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth ||
                    icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                    icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision))
                    // !IsPreviousHealthCoverageSingle(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code))  PIR 10152
                    return icdoWssPersonAccountEnrollmentRequest.date_of_change.GetFirstDayofCurrentMonth();
                else
                    return icdoWssPersonAccountEnrollmentRequest.date_of_change.GetFirstDayofNextMonth();
            }
            else
            {
                //if ((icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueBirth && ibusPlan.IsGHDVPlan() &&
                //    IsPreviousHealthCoverageSingle(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code)) ||  PIR 10152
                if ((icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife || //PIR 10058 
                    icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth || //PIR 13092 - Add GHDV condition
                    icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                    icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision) &&
                    icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueMarriage)
                    return icdoWssPersonAccountEnrollmentRequest.date_of_change.GetFirstDayofNextMonth();
                else
                    return icdoWssPersonAccountEnrollmentRequest.date_of_change;
            }
        }

        public bool iblnIsEOIRequired { get; set; }
        public bool iblnIsEOIflag { get; set; }
        public bool iblnIsMemberAccountInReviewStatus { get; set; }
        public bool iblnIsLifeAutoPosting { get; set; }

        //Check if any workflow already initiated for this person and workflow id
        public bool IsWorkflowAlreadyExistForPerson(int aintProcessId)
        {

            DataTable ldtbList = Select("entSolBpmActivityInstance.IsActivityInstanceAlreadyExists", new object[3] {
                                    icdoWssPersonAccountEnrollmentRequest.person_id, aintProcessId , icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id });

            if (ldtbList.Rows.Count > 0)
                return true;
            return false;
        }

        //load methods to set visibility
        private void SetEnrollmentTypes()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
            {
                if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)   //PIR 25920  New DC plan
                {
                    if (busWSSHelper.IsDBElectedRetirementEnrollment(icdoWssPersonAccountEnrollmentRequest.plan_id, ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                                     icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id))
                        icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.EnrollmentTypeDBElectedOfficial;

                    else if (busWSSHelper.IsDBRetirementOptional(icdoWssPersonAccountEnrollmentRequest.plan_id, icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id))
                        icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.EnrollmentTypeDBOptional;

                    else if (string.IsNullOrEmpty(icdoWssPersonAccountEnrollmentRequest.enrollment_type_value))
                        icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.EnrollmentTypeDBRetirement;
                }
                else if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDC)
                {
                    if (busWSSHelper.IsDCOptionalRetirementEnrollment(icdoWssPersonAccountEnrollmentRequest.plan_id,
                        ibusPersonAccount.icdoPersonAccount.person_account_id, icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id))
                        icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.EnrollmentTypeDCOptional;

                    else if (String.IsNullOrEmpty(icdoWssPersonAccountEnrollmentRequest.enrollment_type_value))
                        icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.EnrollmentTypeDCRetirement;
                }
            }
            else if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
            {
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth
                                     || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental
                                     || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMedicarePartD
                                     || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
                {
                    icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.EnrollmentTypeGHDV;
                }
                else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
                {
                    icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.EnrollmentTypeLife;
                }
                else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdEAP)
                {
                    icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.EnrollmentTypeEAP;
                }
                else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdLTC)
                {
                    icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.EnrollmentTypeLTC;
                }
                else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdHMO)
                {
                    icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.EnrollmentTypeGHDV; //need to be revisited later
                }
            }
            else if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
            {
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex)
                {
                    icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.EnrollmentTypeFlexComp;
                }
            }
        }

        //initialize workflow
        public void InitializeWorkflow(int aintProcessID)
        {
            // PIR 10820
            int reference_id = 0;
            if (aintProcessID != busConstant.Map_Process_Update_Flex_Comp_Plan)
                reference_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
            busWorkflowHelper.InitiateBpmRequest(aintProcessID, icdoWssPersonAccountEnrollmentRequest.person_id, 0, reference_id, iobjPassInfo);
        }

        public void LoadPersonAccountEmploymentDetail()
        {
            if (ibusPersonAccountEmploymentDetail.IsNull())
            {
                ibusPersonAccountEmploymentDetail = new busPersonAccountEmploymentDetail();
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail();
            }
            DataTable ldtbList = Select<cdoPersonAccountEmploymentDetail>(new string[2] { "plan_id", "PERSON_EMPLOYMENT_DTL_ID" },
                new object[2] { icdoWssPersonAccountEnrollmentRequest.plan_id, icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.LoadData(ldtbList.Rows[0]);
            }
        }

        public void LoadPersonAccount()
        {
            if (ibusPlan.IsNull())
                LoadPlan();

            if (ibusPersonAccount == null)
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.icolPersonAccount.IsNull())
                ibusPerson.LoadPersonAccount();
            //PIR 16647 - Only Consider Enrolled Or Suspended
            busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccount.Where(
                                                    i => i.icdoPersonAccount.plan_id == icdoWssPersonAccountEnrollmentRequest.plan_id &&
                                                        i.IsEnrolledOrSuspended()
                                                ).FirstOrDefault();
            if (lbusPersonAccount.IsNotNull())
            {
                ibusPersonAccount = lbusPersonAccount;
            }
        }
        public void LoadPersonAccountForPosting()
        {
            if (ibusPlan.IsNull())
                LoadPlan();

            if (ibusPersonAccount == null)
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.icolPersonAccount.IsNull())
                ibusPerson.LoadPersonAccount();
            //PIR 16647 - Only Consider Enrolled Or Suspended
            busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccount.Where(
                                                    i => i.icdoPersonAccount.plan_id == icdoWssPersonAccountEnrollmentRequest.plan_id &&
                                                        (i.IsEnrolledOrSuspended() || i.IsCanceled())
                                                ).FirstOrDefault();
            if (lbusPersonAccount.IsNotNull())
            {
                ibusPersonAccount = lbusPersonAccount;
            }
        }

        #region Retirement Plans

        //PIR 9769
        public bool IsDCPlanEligible()
        {
            if (!ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value.IsNullOrEmpty() && !ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value.IsNullOrEmpty())
            {
                DataTable ldtbDCPlanjobClass = new DataTable();
                ldtbDCPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[4] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE", "DC_TRANSFER_ELIGIBLE" },
                                             new object[4] {busConstant.PlanIdDC,ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value,
                                                          ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value, busConstant.Flag_Yes }, null, null);//PROD PIR 4631
                if (ldtbDCPlanjobClass != null && ldtbDCPlanjobClass.Rows.Count == 0)
                {
                    ldtbDCPlanjobClass = Select<cdoPlanJobClassCrossref>(new string[4] { "plan_id", "JOB_CLASS_VALUE", "JOB_TYPE_VALUE", "DC_TRANSFER_ELIGIBLE" },
                                             new object[4] {busConstant.PlanIdDC2020,ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.job_class_value, //PIR 20232
                                                          ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value, busConstant.Flag_Yes }, null, null);//PROD PIR 4631
                }
                if (ldtbDCPlanjobClass.Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }


        public ArrayList btnRetirementPost_Click()
        {
            ArrayList larrlist = new ArrayList();
            if (ibusPersonAccount == null)
                LoadPersonAccountForPosting();
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonAccountEmploymentDetail.IsNull())
                LoadPersonAccountEmploymentDetail();
            if (ibusPersonAccountEmploymentDetail.IsNull())
                LoadPersonAccountEmploymentDetail();
            bool iblnIsDCPlanEligible = IsDCPlanEligible();
            busPersonAccountRetirement lobjRetirement = new busPersonAccountRetirement();
            if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
            {
                lobjRetirement = CreatePersonAccount(ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date);
                if (lobjRetirement.iarrErrors.Count > 0)
                {
                    foreach (utlError lobjErr in lobjRetirement.iarrErrors)
                        larrlist.Add(lobjErr);
                    return larrlist;
                }
                else
                {
                    icdoWssPersonAccountEnrollmentRequest.target_person_account_id = lobjRetirement.icdoPersonAccount.person_account_id;
                    icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                    icdoWssPersonAccountEnrollmentRequest.Update();

                    ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = lobjRetirement.icdoPersonAccount.person_account_id;
                    ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                    ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();
                    larrlist.Add(this);

                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                        ibusPersonEmploymentDetail.LoadPersonEmployment();

                    if (lobjRetirement.icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && lobjRetirement.IsHistoryEntryRequired) 
                        lobjRetirement.InsertIntoEnrollmentData();//PIR 20017

                    busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                     busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);
                    //PIR 25920 DC 2025 changes 
                    if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2025 && iintAddlEEContributionPercent == 0)
                    {
                        busWSSHelper.PublishMSSMessage(0, 0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10519, iobjPassInfo), 30), 
                                                                           busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);
                    }

                }
                //PIR 25920 DC 2025 changes add extra line for adec selection in history
                if (iintAddlEEContributionPercent != 0)
                {
                    if (lobjRetirement.icdoPersonAccountRetirement.IsNull()) lobjRetirement.icdoPersonAccountRetirement = new cdoPersonAccountRetirement();
                    lobjRetirement.LoadPersonAccount();
                    //ibusPersonAccount.icdoPersonAccount = lobjRetirement.icdoPersonAccount;

                    if (lobjRetirement.icdoPersonAccount?.person_account_id == 0)
                        lobjRetirement.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
                    else
                    {
                        if (ibusMSSPersonAccountRetirement.IsNotNull() && ibusMSSPersonAccountRetirement.icdoPersonAccount.IsNotNull())
                            ibusPersonAccount.icdoPersonAccount = ibusMSSPersonAccountRetirement.icdoPersonAccount;
                        else
                            ibusPersonAccount.icdoPersonAccount = lobjRetirement.icdoPersonAccount;
                    }

                    //PIR 25920 New Plan DC 2025 set additional contribution for new or set old contribution if DC25 plan is reenroll after withdrawn or retireed.
                    //if (ibusPersonEmploymentDetail.iblnIsMSSTempEmploymentDtlWithinFirstSixMonths)
                    //iintAddlEEContributionPercent = iblnIsDC25PlanSelectionCarryForward ? iintOldAddlEEContributionPercent : iintAddlEEContributionPercent;

                    btnUpdatePersonAccountADECPercentage_Click();
                }
            }
            else
            {
                ibusMSSPersonAccountRetirement = new busPersonAccountRetirement
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountRetirement = new cdoPersonAccountRetirement()
                };

                string lstrPrevEnrollmentStatus = ibusPersonAccount.icdoPersonAccount.plan_participation_status_value;
                ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                ibusPersonAccount.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetirementEnrolled;
                ibusMSSPersonAccountRetirement.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
                ibusMSSPersonAccountRetirement.ibusPlan = this.ibusPlan; //PIR 11868 - Plan object is being checked in soft errors, assigned here
                if (ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail.IsNull())
                {
                    ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
                    ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
                }
                ibusMSSPersonAccountRetirement.LoadOrgPlan();

                //PIR 13590
                //If Person Account is already present in the table selecting the existing record
                if (!(ibusMSSPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id)))
                {
                    //when person account is not present assigning the person account ID as it is
                    ibusMSSPersonAccountRetirement.icdoPersonAccountRetirement.person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
                }
                ibusMSSPersonAccountRetirement.icdoPersonAccountRetirement.is_from_mss = true;
                if (ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail.LoadPersonEmployment();
                if (ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                    ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                if (lstrPrevEnrollmentStatus != busConstant.PlanParticipationStatusRetirementEnrolled) // PROD 9709
                    ibusMSSPersonAccountRetirement.icdoPersonAccount.history_change_date = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date;
                else
                    ibusMSSPersonAccountRetirement.icdoPersonAccount.history_change_date = icdoWssPersonAccountEnrollmentRequest.date_of_change;
                if (iblnIsDCPlanEligible)
                {
                    if (ibusMSSPersonAccountRetirement.IsDcEligibilityDateRequired() // PIR 11483
                        && ibusMSSPersonAccountRetirement.IsPopulateDCEligibilityDate(ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date)) // PIR 11891
                    {
                        ibusMSSPersonAccountRetirement.icdoPersonAccountRetirement.dc_eligibility_date = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(180);
                        if (ibusMSSPersonAccountRetirement.icdoPersonAccountRetirement.ienuObjectState == ObjectState.Update
                            || ibusMSSPersonAccountRetirement.icdoPersonAccountRetirement.ienuObjectState == ObjectState.Select)
                        {
                            ibusMSSPersonAccountRetirement.icdoPersonAccountRetirement.Update();
                        }
                    }
                }
                if (iblnIsDC25PlanSelectionCarryForward)
                {
                    ibusMSSPersonAccountRetirement.icdoPersonAccount.addl_ee_contribution_percent = iintOldAddlEEContributionPercent;
                    ibusMSSPersonAccountRetirement.iintAddlEEContributionPercent = iintOldAddlEEContributionPercent;
                    //ibusMSSPersonAccountRetirement.icdoPersonAccount.addl_ee_contribution_percent_end_date = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(30);
                }
                else
                {
                    ibusMSSPersonAccountRetirement.icdoPersonAccount.addl_ee_contribution_percent = iintAddlEEContributionPercent;
                    ibusMSSPersonAccountRetirement.iintAddlEEContributionPercent = iintAddlEEContributionPercent;
                }
                ibusMSSPersonAccountRetirement.BeforePersistChanges();
                ibusMSSPersonAccountRetirement.PersistChanges();
                ibusMSSPersonAccountRetirement.ValidateSoftErrors();
                ibusMSSPersonAccountRetirement.UpdateValidateStatus();
                ibusMSSPersonAccountRetirement.IsTransferEmployment();

                ibusMSSPersonAccountRetirement.iblnIsFromMSSForEnrollmentData = true;//PIR 20017
                ibusMSSPersonAccountRetirement.AfterPersistChanges();

                icdoWssPersonAccountEnrollmentRequest.target_person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
                icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                icdoWssPersonAccountEnrollmentRequest.Update();

                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = ibusMSSPersonAccountRetirement.icdoPersonAccount.person_account_id;
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();

                if (ibusMSSPersonAccountRetirement.icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && ibusMSSPersonAccountRetirement.IsHistoryEntryRequired)
                    ibusMSSPersonAccountRetirement.InsertIntoEnrollmentData();//PIR 20017

                ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
                ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(ibusPersonAccountEmploymentDetail);

                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                {
                    //for optional case  //regular case
                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id > 0)
                    {
                        if (ibusMSSPersonAccountRetirement.ibusHistory == null)
                            ibusMSSPersonAccountRetirement.LoadPreviousHistory();
                        if (!ibusMSSPersonAccountRetirement.IsTransferEmploymentFlag)
                        {
                            // PIR 9115
                            if (istrIsPIR9115Enabled == busConstant.Flag_No)
                            {
                                string lstrPrioityValue = string.Empty;
                                busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(30, iobjPassInfo, ref lstrPrioityValue),
                                    ibusPerson.icdoPerson.FullName, ibusPerson.icdoPerson.LastFourDigitsOfSSN, ibusPlan.icdoPlan.plan_name, ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.ToString("d", CultureInfo.CreateSpecificCulture("en-US"))), //prod pir 6294
                                    lstrPrioityValue, aintPlanID: ibusPersonAccount.icdoPersonAccount.plan_id, aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                            }
                        }
                    }
                }
                busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                 busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);
                larrlist.Add(this);
            }
            
            EvaluateInitialLoadRules();
            return larrlist;
        }
        /// <summary>
        /// Load ADEC Amount values 0-3 or 0-6
        /// </summary>
        /// <returns></returns>
		public Collection<cdoCodeValue> LoadADECAmountValues()
		{
            decimal ldecEEPreTaxAddlAmount = 0.00m;
            string lstrMemberType = string.Empty;
            int lintPlanId = 0;
            DataTable ldtbPlanRetirementRate = new DataTable();
            DataTable ldtbPersonAccountEmploymentDetail = new DataTable();
            Collection<cdoCodeValue> lclbEligibleADECAmountValue = new Collection<cdoCodeValue>();
            if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusPersonEmploymentDetail.LoadPersonEmployment();
            
            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                lintPlanId = ibusPersonAccount.icdoPersonAccount.plan_id;
            else
                lintPlanId = icdoWssPersonAccountEnrollmentRequest.plan_id;

            busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail = new busPersonAccountEmploymentDetail();
            ldtbPersonAccountEmploymentDetail = Select<cdoPersonAccountEmploymentDetail>(new string[2] { "PLAN_ID", "PERSON_EMPLOYMENT_DTL_ID" }, new object[2] { lintPlanId, ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id }, null, "person_employment_dtl_id DESC");
            if (ldtbPersonAccountEmploymentDetail.IsNotNull() && ldtbPersonAccountEmploymentDetail.Rows.Count > 0)
            {
                lbusPersonAccountEmploymentDetail = GetCollection<busPersonAccountEmploymentDetail>(ldtbPersonAccountEmploymentDetail, "icdoPersonAccountEmploymentDetail").First();
            }
                //if(GetCollection<busPersonAccountEmploymentDetail>(ldtbPersonAccountEmploymentDetail, "icdoPersonAccountEmploymentDetail").Any(i => i.icdoPersonAccountEmploymentDetail.person_account_id == ibusPersonAccount.icdoPersonAccount.person_account_id))
                //    lbusPersonAccountEmploymentDetail = GetCollection<busPersonAccountEmploymentDetail>(ldtbPersonAccountEmploymentDetail, "icdoPersonAccountEmploymentDetail").First(i => i.icdoPersonAccountEmploymentDetail.person_account_id == ibusPersonAccount.icdoPersonAccount.person_account_id);

            if (ibusPersonEmploymentDetail.IsNotNull() && lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.IsNotNull()
                && ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value.IsNull())
            {
                if (lbusPersonAccountEmploymentDetail.ibusPersonAccount.IsNull())
                {
                    if(lbusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.IsNotNull())
                        lbusPersonAccountEmploymentDetail.LoadPersonAccount();
                    if (lbusPersonAccountEmploymentDetail.ibusPersonAccount.IsNotNull() && lbusPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.IsNotNull() && 
                        lbusPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.plan_id == 0 )
                    {
                        lbusPersonAccountEmploymentDetail.ibusPersonAccount.icdoPersonAccount.plan_id = lintPlanId;
                    }
                }
                ibusPersonEmploymentDetail.LoadMemberType(DateTime.Now, lbusPersonAccountEmploymentDetail);
            }
            lstrMemberType = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value.IsNull() ? string.Empty : ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value;
            ldtbPlanRetirementRate = Select<cdoPlanRetirementRate>(new string[2] { "plan_id", "member_type_value" }, new object[2] { lintPlanId, lstrMemberType }, null, "addl_ee_pre_tax DESC");
            Collection<busPlanRetirementRate> lclbPlanRetirementRate = GetCollection<busPlanRetirementRate>(ldtbPlanRetirementRate, "icdoPlanRetirementRate");
            if (lclbPlanRetirementRate.IsNotNull() && lclbPlanRetirementRate.Count > 0)
            {
                busPlanRetirementRate lbusPlanRetirementRate = lclbPlanRetirementRate[0];
                ldecEEPreTaxAddlAmount = lbusPlanRetirementRate.icdoPlanRetirementRate.ee_pretax_addl + lbusPlanRetirementRate.icdoPlanRetirementRate.ee_post_tax_addl;
                idecTempDCTempContribution = lbusPlanRetirementRate.icdoPlanRetirementRate.ee_pre_tax + lbusPlanRetirementRate.icdoPlanRetirementRate.ee_post_tax + 
                                        lbusPlanRetirementRate.icdoPlanRetirementRate.ee_emp_pickup + lbusPlanRetirementRate.icdoPlanRetirementRate.ee_rhic + 
                                        lbusPlanRetirementRate.icdoPlanRetirementRate.er_post_tax + lbusPlanRetirementRate.icdoPlanRetirementRate.er_rhic;
            }

            for (int i = 0; i <= ldecEEPreTaxAddlAmount; i++)
            {
                if (ldecEEPreTaxAddlAmount != 0)
                {
                    if(i == 0)
                        lclbEligibleADECAmountValue.Add(new cdoCodeValue { code_value = Convert.ToString(i), code_id = -1 });
                    else
                        lclbEligibleADECAmountValue.Add(new cdoCodeValue { code_value = Convert.ToString(i), code_id = i });
                }
            }

            return lclbEligibleADECAmountValue;
        }
        /// <summary>
        /// validate the adec amount is blank or not  
        /// </summary>
        /// <returns></returns>
        public ArrayList btnValidateADECPercentageNotBlank()
        {
            ArrayList larrList = new ArrayList();
            if (iblnIsOnlyADECUpdate)
            {
                ValidateGroupRules("AddPercentageElection",utlPageMode.All);
                if (iarrErrors.Count > 0)
                {
                    foreach (utlError lobjErro in iarrErrors)
                    {
                        larrList.Add(lobjErro);
                    }
                    return larrList;
                }
            }
            return larrList;
        }
        /// <summary>
        /// validate the adec amount limit is 3 or 6 and update optional election if -1 then to 0 
        /// </summary>
        /// <returns></returns>
        /// 
        public ArrayList btnUpdatePersonAccountADECPercentage_Click()
        {
            ArrayList larrList = new ArrayList();
            if (iblnIsOnlyADECUpdate)
            {
                ValidateHardErrors(utlPageMode.All);
                if (iarrErrors.Count > 0)
                {
                    foreach (utlError lobjErro in iarrErrors)
                    {
                        larrList.Add(lobjErro);
                    }
                    return larrList;
                }
            }
            if (icdoWssPersonAccountEnrollmentRequest.IsNull()) icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();
            if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
            if (ibusPersonAccount.IsNull()) LoadPersonAccountForPosting();
            if (ibusPersonAccount.icdoPersonAccount.IsNull()) ibusPersonAccount.icdoPersonAccount = new cdoPersonAccount();
             
            icdoWssPersonAccountEnrollmentRequest.plan_id = ibusPersonAccount.icdoPersonAccount.plan_id > 0 ? ibusPersonAccount.icdoPersonAccount.plan_id : icdoWssPersonAccountEnrollmentRequest.plan_id;
            //if(iintAddlEEContributionPercent > 0)
            if(IsTemporaryEmployee)
                ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp = iintAddlEEContributionPercent;
            else
                ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent = iintAddlEEContributionPercent;
            ibusPersonAccount.icdoPersonAccount.modified_by = iobjPassInfo.istrUserID;
            ibusPersonAccount.icdoPersonAccount.modified_date = DateTime.Now;
            //insert enrollment requrest for choose additional contribution with acknoledgement text.
            if (iblnIsOnlyADECUpdate)
            {
                if (IsTemporaryEmployee)
                    icdoWssPersonAccountEnrollmentRequest.date_of_change = ibusPersonAccount.icdoPersonAccount.history_change_date;
                else
                    icdoWssPersonAccountEnrollmentRequest.date_of_change = busGlobalFunctions.GetFirstDayofNextMonth(ibusPersonAccount.icdoPersonAccount.history_change_date);
                icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                SetEnrollmentTypes();
                //icdoWssPersonAccountEnrollmentRequest.enrollment_type_value = "";
                icdoWssPersonAccountEnrollmentRequest.change_effective_date = icdoWssPersonAccountEnrollmentRequest.date_of_change;
                icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value = busConstant.PlanEnrollmentOptionValueEnroll;
                if (IsTemporaryEmployee)
                {
                    icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent_temp = iintAddlEEContributionPercent;
                    icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_c_date = icdoWssPersonAccountEnrollmentRequest.date_of_change;
                    icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_c_flag = busConstant.Flag_Yes;
                }
                else
                    icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent = iintAddlEEContributionPercent;
                icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.ReasonValueADEC;
                icdoWssPersonAccountEnrollmentRequest.ee_acknowledgement_agreement_flag = busConstant.Flag_Yes;
                //ibusPersonAccount.icdoPersonAccount.reason_value = busConstant.ReasonValueADEC;
                icdoWssPersonAccountEnrollmentRequest.Insert();
                string lstrPermOrTemp = "";
                if (IsTemporaryEmployee)
                {
                    lstrPermOrTemp = busConstant.TempAgreementToParticipate;
                    InsertString(busConstant.MainRetirementEnrollmentMemberAuthorization);
                }
                else
                    lstrPermOrTemp = busConstant.PermAdditionalContributionElectionInfo;
                InsertString(lstrPermOrTemp);
                InsertString(busConstant.PermAdditionalContributionElectionAck);
                InsertString(busConstant.ConfirmationString);
                if (IsTemporaryEmployee)
                    InsertCollection(iclbTempEEADECAcknowledgement);
                busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                 busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);
            }
            
            if(ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id == 0)
            {
                ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
            }
            if (ibusPersonAccount.ibusPersonAccountRetirement.IsNull())
                ibusPersonAccount.LoadPersonAccountRetirement();
            ibusPersonAccount.ibusPersonAccountRetirement.IsHistoryEntryRequired = true;
            if(IsTemporaryEmployee)
                ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.history_change_date = ibusPersonAccount.icdoPersonAccount.history_change_date;
            else
                ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.history_change_date = busGlobalFunctions.GetFirstDayofNextMonth(ibusPersonAccount.icdoPersonAccount.history_change_date);
            ibusPersonAccount.ibusPersonAccountRetirement.iintAddlEEContributionPercent = iintAddlEEContributionPercent;
            ibusPersonAccount.ibusPersonAccountRetirement?.ProcessHistory();
            ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.reason_value = busConstant.ReasonValueADEC;
            ibusPersonAccount.ibusPersonAccountRetirement.ibusHistory = null;
            //if (iintAddlEEContributionPercent != iintOldAddlEEContributionPercent)
            //    ibusPersonAccount.ibusPersonAccountRetirement.iblnIsEEPercentChanged = true;
            //ibusPersonAccount.ibusPersonAccountRetirement?.InsertIntoEnrollmentData();

            //PIR 25920 DC 2025 changes End date current Def Comp provider record and insert new record with new effective date 
            if (iintAddlEEContributionPercent > 0)
            {
                //optional %election is max then end current def comp provider and add new entry with same provider and apply matching is N 
                string lstrMemberType = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value.IsNull() ? string.Empty : ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value;
                decimal iintMaxAddlPercentageByMemberType = ibusPersonEmploymentDetail.GetMaxEEPreTaxAdditionalPercentage(lstrMemberType, ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.history_change_date, icdoWssPersonAccountEnrollmentRequest.plan_id);
                if (iintMaxAddlPercentageByMemberType == iintAddlEEContributionPercent)
                {
                    UpdateCurrentProviderAndInsertNew();
                }
            }
            //PIR 25920 DC 2025 changes update the person account and person account RETE history in case additional contribution is -1
            if (iintAddlEEContributionPercent == -1)
            {
                //int lintIsUpdateEnrollmentRequest = 0;
                //if(iobjPassInfo.istrFormName == "wfmMSSPensionPlanMainRetirementOptionalEnrollmentWizard" || iobjPassInfo.istrFormName == "wfmMSSPensionPlanRetirementEnrollmentMaintenance")
                //    lintIsUpdateEnrollmentRequest = 1;
                ibusPersonAccount.ibusPersonAccountRetirement.LoadPreviousHistory();
                DBFunction.DBNonQuery("cdoPersonAccountRetirementHistory.UpdateAdditionalContributionToZero", new object[2] { ibusPersonAccount.ibusPersonAccountRetirement.ibusHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id,
                        icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id},
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }

            if (!iblnIsFromEmployment)
            {
                if (iintAddlEEContributionPercent != iintOldAddlEEContributionPercent)
                    ibusPersonAccount.ibusPersonAccountRetirement.iblnIsEEPercentChanged = true;
                ibusPersonAccount.ibusPersonAccountRetirement?.InsertIntoEnrollmentData();
            }
        
            larrList.Add(this);
            return larrList;
        }
        /// <summary>
        /// optional %election is max then end current def comp provider and add new entry with same provider and apply matching is N 
        /// PIR 25920
        /// </summary>
        public void UpdateCurrentProviderAndInsertNew()
        {
            busPersonAccount lbusPersonAccountDefComp = new busPersonAccount();
            busPerson lbusPerson = new busPerson { icdoPerson = new cdoPerson() };
            if (ibusPersonAccount.ibusPerson.IsNull()) ibusPersonAccount.LoadPerson();
            lbusPerson.icdoPerson = ibusPersonAccount.ibusPerson.icdoPerson;
            lbusPerson.LoadPersonAccountByPlan(busConstant.PlanIdDeferredCompensation);
            //checking if the person account enrolled for def comp is exist  
            if (lbusPerson.icolPersonAccountByPlan.IsNotNull() && lbusPerson.icolPersonAccountByPlan.Count > 0)
            {
                if (lbusPerson.icolPersonAccountByPlan.Any(lobjPersonAccount => lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation
                 && lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled
                 && busGlobalFunctions.CheckDateOverlapping(ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.history_change_date,
                 lobjPersonAccount.icdoPersonAccount.start_date, lobjPersonAccount.icdoPersonAccount.end_date)))
                {
                    lbusPersonAccountDefComp = lbusPerson.icolPersonAccountByPlan.Where(lobjPersonAccount => lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation
                    && lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled
                    && busGlobalFunctions.CheckDateOverlapping(ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.history_change_date,
                    lobjPersonAccount.icdoPersonAccount.start_date, lobjPersonAccount.icdoPersonAccount.end_date)).First();
                }
            }
            if (lbusPersonAccountDefComp.IsNotNull())
            {
                if (lbusPersonAccountDefComp.icdoPersonAccount.IsNull()) lbusPersonAccountDefComp.icdoPersonAccount = new cdoPersonAccount();
                lbusPersonAccountDefComp.LoadActiveDefCompProviders(ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.history_change_date);
                if (lbusPersonAccountDefComp.iclcDefCompActiveProviders.IsNotNull() && lbusPersonAccountDefComp.iclcDefCompActiveProviders.Count > 0)
                {
                    if (lbusPersonAccountDefComp.iclcDefCompActiveProviders.Any(lobjDefCompActiveProvider => lobjDefCompActiveProvider.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution == busConstant.Flag_Yes))
                    {
                        busPersonAccountDeferredCompProvider lbusPersonAccountDeferredCompProviderOld = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
                        busPersonAccountDeferredCompProvider lbusPersonAccountDeferredCompProviderNew = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };

                        lbusPersonAccountDeferredCompProviderOld = lbusPersonAccountDefComp.iclcDefCompActiveProviders.Where(lobjDefCompActiveProvider => lobjDefCompActiveProvider.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution == busConstant.Flag_Yes).First();
                        if (lbusPersonAccountDeferredCompProviderOld.IsNotNull())
                        {
                            lbusPersonAccountDeferredCompProviderNew.istrUpdateExistingEmployerFlag = busConstant.Flag_Yes;
                            lbusPersonAccountDeferredCompProviderNew.istrUpdateExistingProviderFlag = busConstant.Flag_No;

                            lbusPersonAccountDeferredCompProviderNew.icdoPersonAccountDeferredCompProvider.person_account_id = lbusPersonAccountDeferredCompProviderOld.icdoPersonAccountDeferredCompProvider.person_account_id;
                            lbusPersonAccountDeferredCompProviderNew.icdoPersonAccountDeferredCompProvider.start_date = ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.history_change_date;
                            lbusPersonAccountDeferredCompProviderNew.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution = busConstant.Flag_Yes;
                            lbusPersonAccountDeferredCompProviderNew.UpdatePreviousDefCompProvider();

                            //lbusPersonAccountDeferredCompProviderOld.icdoPersonAccountDeferredCompProvider.end_date = ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccount.history_change_date;
                            //lbusPersonAccountDeferredCompProviderOld.icdoPersonAccountDeferredCompProvider.Update();
                            //if (lbusPersonAccountDeferredCompProviderOld.icdoPersonAccountDeferredCompProvider.is_apply_employer_matching_contribution == busConstant.Flag_Yes)
                            //    lbusPersonAccountDeferredCompProviderOld.InsertDefCompProviderWithEmployerMatchingn(lbusPersonAccountDeferredCompProviderOld);
                            //lbusPersonAccountDeferredCompProvider.iclbPersonAccountDeferredCompProviderUpdatedRecord.Add(lbusPersonAccountDeferredCompProvider);

                        }
                    }
                }
            }
        }
        public ArrayList btnDBElectedOfficialPost_Click()
        {
            ArrayList larrlist = new ArrayList();

            if (ibusPersonAccount == null)
                LoadPersonAccountForPosting();
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonAccountEmploymentDetail.IsNull())
                LoadPersonAccountEmploymentDetail();

            busPersonAccountRetirement lobjRetirement = new busPersonAccountRetirement();
            if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
            {
                lobjRetirement = CreatePersonAccount(ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date);
                if (lobjRetirement.iarrErrors.Count > 0)
                {
                    foreach (utlError lobjErr in lobjRetirement.iarrErrors)
                        larrlist.Add(lobjErr);
                    return larrlist;
                }
                else
                {
                    icdoWssPersonAccountEnrollmentRequest.target_person_account_id = lobjRetirement.icdoPersonAccount.person_account_id;
                    icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                    icdoWssPersonAccountEnrollmentRequest.Update();

                    ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = lobjRetirement.icdoPersonAccount.person_account_id;
                    ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                    larrlist.Add(this);

                    if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                        ibusPersonEmploymentDetail.LoadPersonEmployment();
                    busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                     busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);

                }
            }
            else
            {
                ibusMSSPersonAccountRetirement = new busPersonAccountRetirement
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountRetirement = new cdoPersonAccountRetirement()
                };

                ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                ibusPersonAccount.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetirementEnrolled;
                ibusMSSPersonAccountRetirement.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
                //this is needed for the validation
                if (ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail.IsNull())
                {
                    ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
                    ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
                }

                ibusMSSPersonAccountRetirement.LoadOrgPlan();
                ibusMSSPersonAccountRetirement.icdoPersonAccountRetirement.person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
                ibusMSSPersonAccountRetirement.BeforePersistChanges();
                ibusMSSPersonAccountRetirement.PersistChanges();
                ibusMSSPersonAccountRetirement.ValidateSoftErrors();
                ibusMSSPersonAccountRetirement.UpdateValidateStatus();
                ibusMSSPersonAccountRetirement.iblnIsFromMSSForEnrollmentData = true;
                ibusMSSPersonAccountRetirement.AfterPersistChanges();

                icdoWssPersonAccountEnrollmentRequest.target_person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
                icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                icdoWssPersonAccountEnrollmentRequest.Update();

                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = ibusMSSPersonAccountRetirement.icdoPersonAccount.person_account_id;
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;

                ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
                ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(ibusPersonAccountEmploymentDetail);
                busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                 busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);

            }
            if ((icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBElectedOfficial)
                && (icdoWssPersonAccountEnrollmentRequest.ee_acknowledgement_waiver_flag == busConstant.Flag_Yes))
            {
                //update the election value to waived if the part d flag is checked
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueWaived;

                // Post message to employer
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                // PIR 9115
                if (istrIsPIR9115Enabled == busConstant.Flag_No)
                {
                    string lstrPrioityValue = string.Empty;
                    busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(2, iobjPassInfo, ref lstrPrioityValue), ibusPerson.icdoPerson.FullName, ibusPlan.icdoPlan.mss_plan_name),
                        lstrPrioityValue, aintPlanID: icdoWssPersonAccountEnrollmentRequest.plan_id,
                        aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                            astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                }
            }
            ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();

            //PIR 17081
            if ((icdoWssPersonAccountEnrollmentRequest.enrollment_type_value == busConstant.EnrollmentTypeDBElectedOfficial)
                && (icdoWssPersonAccountEnrollmentRequest.ee_acknowledgement_waiver_flag == busConstant.Flag_Yes))
                InsertIntoEnrollmentData();
            else if (lobjRetirement.icdoPersonAccount.IsNotNull() && lobjRetirement.icdoPersonAccount.person_account_id > 0 && lobjRetirement.icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && lobjRetirement.IsHistoryEntryRequired)
                lobjRetirement.InsertIntoEnrollmentData();
            else if (ibusMSSPersonAccountRetirement != null && ibusMSSPersonAccountRetirement.icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && ibusMSSPersonAccountRetirement.IsHistoryEntryRequired)
                ibusMSSPersonAccountRetirement.InsertIntoEnrollmentData();

            EvaluateInitialLoadRules();
            return larrlist;
        }

        public ArrayList btnRetirementOptionalPost_Click()
        {
            ArrayList larrlist = new ArrayList();
            if (ibusPersonAccount == null)
                LoadPersonAccountForPosting();
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonAccountEmploymentDetail.IsNull())
                LoadPersonAccountEmploymentDetail();

            if (icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_c_flag == busConstant.Flag_Yes)
            {
                busPersonAccountRetirement lobjRetirement = new busPersonAccountRetirement();
                if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                {
                    lobjRetirement = CreatePersonAccount(icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_c_date);
                    if (lobjRetirement.iarrErrors.Count > 0)
                    {
                        //add the array list
                        foreach (utlError lobjErr in lobjRetirement.iarrErrors)
                            larrlist.Add(lobjErr);

                        return larrlist;
                    }
                    else
                    {
                        ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value = busConstant.EmploymentStatusContributing;
                        ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();

                        icdoWssPersonAccountEnrollmentRequest.target_person_account_id = lobjRetirement.icdoPersonAccount.person_account_id;
                        icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                        icdoWssPersonAccountEnrollmentRequest.Update();

                        ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = lobjRetirement.icdoPersonAccount.person_account_id;
                        ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                        ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();
                        if (lobjRetirement.icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && lobjRetirement.IsHistoryEntryRequired)
                            lobjRetirement.InsertIntoEnrollmentData();//PIR 20017
                        larrlist.Add(this);
                        //PIR 25920 DC 2025 changes 
                        if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2025 && iintAddlEEContributionPercent == 0)
                        {
                            busWSSHelper.PublishMSSMessage(0, 0, string.Format(busGlobalFunctions.GetMessageTextByMessageID(10519, iobjPassInfo), 180),
                                                                               busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);
                        }
                    }
                    //PIR 25920 DC 2025 changes add extra line for adec selection in history
                    //if (iintAddlEEContributionPercent != 0)
                    //{
                    //    if (lobjRetirement.icdoPersonAccount?.person_account_id == 0)
                    //        lobjRetirement.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
                    //    else
                    //    {
                    //        if (ibusMSSPersonAccountRetirement.IsNotNull() && ibusMSSPersonAccountRetirement.icdoPersonAccount.IsNotNull())
                    //            ibusPersonAccount.icdoPersonAccount = ibusMSSPersonAccountRetirement.icdoPersonAccount;
                    //        else
                    //            ibusPersonAccount.icdoPersonAccount = lobjRetirement.icdoPersonAccount;
                    //    }
                        //PIR 25920 New Plan DC 2025 set additional contribution for new or set old contribution if DC25 plan is reenroll after withdrawn or retireed.
                        //if (ibusPersonEmploymentDetail.iblnIsMSSTempEmploymentDtlWithinFirstSixMonths)
                        //    iintAddlEEContributionPercent = iblnIsDC25PlanSelectionCarryForward ? iintOldAddlEEContributionPercent : iintAddlEEContributionPercent;

                        //lobjRetirement.LoadPersonAccount();
                        //ibusPersonAccount.icdoPersonAccount = lobjRetirement.icdoPersonAccount;
                    //    btnUpdatePersonAccountADECPercentage_Click();
                    //}
                }
                else
                {
                    ibusMSSPersonAccountRetirement = new busPersonAccountRetirement
                    {
                        icdoPersonAccount = new cdoPersonAccount(),
                        icdoPersonAccountRetirement = new cdoPersonAccountRetirement()
                    };

                    ibusMSSPersonAccountRetirement.ibusPerson = ibusPerson;
                    ibusMSSPersonAccountRetirement.ibusPlan = ibusPlan;
                    ibusMSSPersonAccountRetirement.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
                    ibusMSSPersonAccountRetirement.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetirementEnrolled;
                    //this is needed for the validation
                    if (ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail.IsNull())
                    {
                        ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
                        ibusMSSPersonAccountRetirement.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
                    }

                    ibusMSSPersonAccountRetirement.icdoPersonAccountRetirement.person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
                    ibusMSSPersonAccountRetirement.icdoPersonAccount.history_change_date = icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_c_date;
                    ibusMSSPersonAccountRetirement.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                    ibusMSSPersonAccountRetirement.BeforeValidate(utlPageMode.New);
                    ibusMSSPersonAccountRetirement.ValidateHardErrors(utlPageMode.New);
                    if (ibusMSSPersonAccountRetirement.iarrErrors.Count > 0)
                    {
                        //add the array list
                        foreach (utlError lobjErr in ibusMSSPersonAccountRetirement.iarrErrors)
                            larrlist.Add(lobjErr);
                        return larrlist;
                    }
                    else
                    {
                        //this is added without null check bcoz in before validate the person employment detail objects resets and empl dtl is set to 0, which 
                        //will not the status change
                        LoadPersonEmploymentDetail();
                        ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value = busConstant.EmploymentStatusContributing;
                        ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();

                        ibusMSSPersonAccountRetirement.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                        ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = ibusMSSPersonAccountRetirement.icdoPersonAccount.person_account_id;
                        ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                        ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();

                        ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
                        ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(ibusPersonAccountEmploymentDetail);
                        ibusMSSPersonAccountRetirement.LoadOrgPlan();
                        if (iblnIsDC25PlanSelectionCarryForward)
                        {
                            ibusMSSPersonAccountRetirement.icdoPersonAccount.addl_ee_contribution_percent_temp = iintOldAddlEEContributionPercent;
                            ibusMSSPersonAccountRetirement.iintAddlEEContributionPercent = iintOldAddlEEContributionPercent;
                            //ibusMSSPersonAccountRetirement.icdoPersonAccount.addl_ee_contribution_percent_end_date = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(30);
                        }
                        else
                        {
                            ibusMSSPersonAccountRetirement.icdoPersonAccount.addl_ee_contribution_percent_temp = iintAddlEEContributionPercent;
                            ibusMSSPersonAccountRetirement.iintAddlEEContributionPercent = iintAddlEEContributionPercent;
                        }
                        ibusMSSPersonAccountRetirement.BeforePersistChanges();
                        ibusMSSPersonAccountRetirement.PersistChanges();
                        ibusMSSPersonAccountRetirement.ValidateSoftErrors();
                        ibusMSSPersonAccountRetirement.UpdateValidateStatus();
                        ibusMSSPersonAccountRetirement.iblnIsFromMSSForEnrollmentData = true;
                        ibusMSSPersonAccountRetirement.AfterPersistChanges();

                        icdoWssPersonAccountEnrollmentRequest.target_person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
                        icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                        icdoWssPersonAccountEnrollmentRequest.Update();

                        if (ibusMSSPersonAccountRetirement.icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && ibusMSSPersonAccountRetirement.IsHistoryEntryRequired)
                            ibusMSSPersonAccountRetirement.InsertIntoEnrollmentData();

                        if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                            ibusPersonEmploymentDetail.LoadPersonEmployment();
                        LoadPersonEmploymentDetail();
                        ibusPersonEmploymentDetail.LoadPersonEmployment();
                        ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                        larrlist.Add(this);
                    }
                }
                
                EvaluateInitialLoadRules();

                busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                 busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);
            }
            else if (icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag == busConstant.Flag_Yes)
            {
                //update the election value to waived if the part d flag is checked
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueWaived;
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();

                //PIR 17081
                InsertIntoEnrollmentData();

                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value = busConstant.EmploymentStatusNonContributing;
                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.Update();

                string lstrMessage = string.Empty;
                string lstrPrioityValue = string.Empty;
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain2020)//PIR 20232 ?code
                {
                    lstrMessage = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(32, iobjPassInfo, ref lstrPrioityValue), ibusPerson.icdoPerson.FullName, icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_date);
                }
                else
                {
                    lstrMessage = string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(33, iobjPassInfo, ref lstrPrioityValue), ibusPerson.icdoPerson.FullName, icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_date);
                }

                // PIR 9115
                if (istrIsPIR9115Enabled == busConstant.Flag_No)
                {
                    busWSSHelper.PublishESSMessage(0, 0, lstrMessage,
                                         lstrPrioityValue, aintPlanID: icdoWssPersonAccountEnrollmentRequest.plan_id, aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                        astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                }


                if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                    icdoWssPersonAccountEnrollmentRequest.target_person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
                icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                icdoWssPersonAccountEnrollmentRequest.Update();

                LoadPersonEmploymentDetail();
                ibusPersonEmploymentDetail.LoadPersonEmployment();
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                larrlist.Add(this);
                EvaluateInitialLoadRules();
                busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                 busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);
            }
            return larrlist;
        }

        #endregion

        public ArrayList btnReject_Click()
        {
            ArrayList larrlist = new ArrayList();
            if (icdoWssPersonAccountEnrollmentRequest.rejection_reason.IsNullOrEmpty())
            {
                utlError lerror = new utlError { istrErrorID = "10038", istrErrorMessage = "Enter Reason for Rejection." };
                larrlist.Add(lerror);
                return larrlist;
            }
            else
            {
                icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusRejected;
                icdoWssPersonAccountEnrollmentRequest.Update();
                larrlist.Add(this);
                EvaluateInitialLoadRules();
            }

            //PIR 23900 
            DataTable ldtbUserInfo = null;
            string lstrRejectedByUser = string.Empty;
            if (!string.IsNullOrEmpty(iobjPassInfo.istrUserID))
            {
                ldtbUserInfo = iobjPassInfo.isrvDBCache.GetUserInfo(iobjPassInfo.istrUserID);
                if (ldtbUserInfo?.Rows?.Count > 0)
                    lstrRejectedByUser = ldtbUserInfo.Rows[0]["FIRST_NAME"] + " " + ldtbUserInfo.Rows[0]["LAST_NAME"];
            }

            // PIR 13809 - Changed the message to be published on clicking reject button.
            //PROD PIR ID 6288
            if (ibusPlan.IsNull()) LoadPlan();
            busWSSHelper.PublishMSSMessage(0, 0,
                "The request to enroll/change your " + ibusPlan.icdoPlan.mss_plan_name + " plan has been rejected by " + lstrRejectedByUser + ". " + icdoWssPersonAccountEnrollmentRequest.rejection_reason,
                busConstant.WSSMessagePriorityHigh, icdoWssPersonAccountEnrollmentRequest.person_id, icdoWssPersonAccountEnrollmentRequest.plan_id);

            return larrlist;
        }

        // PIR 11102
        public ArrayList btnIgnore_Click()
        {
            ArrayList larrlist = new ArrayList();

            // PIR 13809 - Commented below code to remove message published on igmore Button 

            //if (icdoWssPersonAccountEnrollmentRequest.rejection_reason.IsNullOrEmpty())
            //{
            //    utlError lerror = new utlError { istrErrorID = "10038", istrErrorMessage = "Enter Reason for Ignore." };
            //    larrlist.Add(lerror);
            //    return larrlist;
            //}
            //else
            {
                icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusIgnored;
                icdoWssPersonAccountEnrollmentRequest.Update();
                larrlist.Add(this);
                EvaluateInitialLoadRules();
            }

            //if (ibusPlan.IsNull()) LoadPlan();
            //busWSSHelper.PublishMSSMessage(0, 0,
            //    "The request to enroll/change your " + ibusPlan.icdoPlan.mss_plan_name + " plan has been Ignored. Please review your plan details from the Benefit Plans tab",
            //    busConstant.WSSMessagePriorityHigh, icdoWssPersonAccountEnrollmentRequest.person_id, icdoWssPersonAccountEnrollmentRequest.plan_id);

            return larrlist;
        }

        private busPersonAccountRetirement CreatePersonAccount(DateTime adtStartDate)
        {
            busPersonAccountRetirement lobjPersonAccountReitirement = new busPersonAccountRetirement
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountRetirement = new cdoPersonAccountRetirement()
            };

            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

            lobjPersonAccountReitirement.icdoPersonAccount.person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
            lobjPersonAccountReitirement.icdoPersonAccount.plan_id = icdoWssPersonAccountEnrollmentRequest.plan_id;
            lobjPersonAccountReitirement.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;

            lobjPersonAccountReitirement.ibusPerson = ibusPerson;
            lobjPersonAccountReitirement.ibusPlan = ibusPlan;
            lobjPersonAccountReitirement.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
            lobjPersonAccountReitirement.ibusPersonEmploymentDetail.ibusPersonEmployment = ibusPersonEmploymentDetail.ibusPersonEmployment;
            lobjPersonAccountReitirement.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
            ////PIR 25920 New Plan DC 2025 set additional contribution for new or set old contribution if DC25 plan is reenroll after withdrawn or retireed.
            //if (ibusPersonEmploymentDetail.iblnIsMSSTempEmploymentDtlWithinFirstSixMonths)
            //    lobjPersonAccountReitirement.icdoPersonAccount.addl_ee_contribution_percent = iblnIsDC25PlanRefundWithdrawnRetired ?
            //        iintOldAddlEEContributionPercent : iintAddlEEContributionPercent;
            //PIR 25920 When enrolling as Temp, the enrollment and election have the same effective date
			if (icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent_temp != 0 && IsTemporaryEmployee)
            {
                lobjPersonAccountReitirement.icdoPersonAccount.addl_ee_contribution_percent_temp = icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent_temp;
                lobjPersonAccountReitirement.iintAddlEEContributionPercent = icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent_temp;
            }
            lobjPersonAccountReitirement.LoadOrgPlan();

            lobjPersonAccountReitirement.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusRetirementEnrolled;
            lobjPersonAccountReitirement.icdoPersonAccount.start_date = adtStartDate;
            lobjPersonAccountReitirement.icdoPersonAccount.ienuObjectState = ObjectState.Insert;
            lobjPersonAccountReitirement.icdoPersonAccount.status_value = busConstant.StatusValid;
            lobjPersonAccountReitirement.icdoPersonAccountRetirement.mutual_fund_window_flag = busConstant.Flag_No;
            lobjPersonAccountReitirement.icdoPersonAccountRetirement.ienuObjectState = ObjectState.Insert;

            lobjPersonAccountReitirement.BeforeValidate(utlPageMode.New);
            lobjPersonAccountReitirement.ValidateHardErrors(utlPageMode.New);

            if (lobjPersonAccountReitirement.iarrErrors.Count > 0)
            {
                return lobjPersonAccountReitirement;
            }
            else
            {
                // PIR 11483
                if (IsDCPlanEligible() && lobjPersonAccountReitirement.IsDcEligibilityDateRequired()
                    && (lobjPersonAccountReitirement.IsPopulateDCEligibilityDate(ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date))) // PIR 11891
                {
                    lobjPersonAccountReitirement.icdoPersonAccountRetirement.dc_eligibility_date =
                                                                ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddDays(180);
                }

                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                lobjPersonAccountReitirement.BeforePersistChanges();
                lobjPersonAccountReitirement.iarrChangeLog.Add(lobjPersonAccountReitirement.icdoPersonAccountRetirement);
                lobjPersonAccountReitirement.PersistChanges();
                lobjPersonAccountReitirement.ValidateSoftErrors();
                lobjPersonAccountReitirement.UpdateValidateStatus();

                lobjPersonAccountReitirement.iblnIsFromMSSForEnrollmentData = true;//PIR 20017
                lobjPersonAccountReitirement.AfterPersistChanges();

                //Set the Election Value as Enrolled in Person Employment Detail
                //ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = lobjPersonAccountReitirement.icdoPersonAccount.person_account_id;
                //ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();
                lobjPersonAccountReitirement.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
                lobjPersonAccountReitirement.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(ibusPersonAccountEmploymentDetail);
            }
            return lobjPersonAccountReitirement;
        }

        public void SetDCEligibleFlag()
        {
            iblnIsDCEligible = false;
            DataTable ldtbList = new DataTable();
            ldtbList = Select<cdoPersonAccountEmploymentDetail>(new string[2] { "plan_id", "PERSON_EMPLOYMENT_DTL_ID" },
               new object[2] { busConstant.PlanIdDC, icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id }, null, null);
            if (ldtbList != null && ldtbList.Rows.Count == 0)
            {
                ldtbList = Select<cdoPersonAccountEmploymentDetail>(new string[2] { "plan_id", "PERSON_EMPLOYMENT_DTL_ID" },
                new object[2] { busConstant.PlanIdDC2020, icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id }, null, null); //PIR 20232
            }
            if (ldtbList.Rows.Count > 0)
            {
                iblnIsDCEligible = true;
            }
        }

        # region Insurance

        public void LoadPersonAccountGHDV()
        {
            if (ibusMSSPersonAccountGHDV.IsNull())
                ibusMSSPersonAccountGHDV = new busPersonAccountGhdv();

            ibusMSSPersonAccountGHDV.FindGHDVByPersonAccountID(ibusPersonAccount.icdoPersonAccount.person_account_id);

        }

        //load change effective date
        public void LoadChangeEffectiveDate()
        {
            icdoWssPersonAccountEnrollmentRequest.date_of_change = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.GetFirstDayofNextMonth();  // PIR 11208         

            // PIR 11840 - 3rd issue
            if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                icdoWssPersonAccountEnrollmentRequest.date_of_change = AnnualEnrollmentEffectiveDate;
            else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth && icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonACAAnnualEnrollment)
            {
                icdoWssPersonAccountEnrollmentRequest.date_of_change = AnnualEnrollmentTempEffectiveDate;
            }
        }

        public bool IsValueEnteredInWizardStep1()
        {
            if ((!String.IsNullOrEmpty(icdoWssPersonAccountEnrollmentRequest.reason_value))
                || (!String.IsNullOrEmpty(icdoMSSGDHV.level_of_coverage_value))
                || (!String.IsNullOrEmpty(icdoMSSGDHV.coverage_code))
                || (icdoMSSGDHV.keeping_other_coverage_flag == busConstant.Flag_Yes))
                return true;
            if (IsDataEnteredInOthersCoverage() || IsDataEnteredInWorkersCompensation())
                return true;
            return false;
        }

        private bool IsDataEnteredInOthersCoverage()
        {
            foreach (busWssPersonAccountOtherCoverageDetail lobjOtherCoverage in iclbMSSOtherCoverageDetail)
            {
                if ((!String.IsNullOrEmpty(lobjOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.policy_holder))
                    || (!String.IsNullOrEmpty(lobjOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.policy_number))
                    || (!String.IsNullOrEmpty(lobjOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.covered_person))
                    || (!String.IsNullOrEmpty(lobjOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.other_coverage_number))
                    || (lobjOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.policy_end_date != DateTime.MinValue)
                   || (lobjOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.policy_start_date != DateTime.MinValue)
                    || (lobjOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.date_of_birth != DateTime.MinValue))
                    return true;
            }
            return false;
        }

        private bool IsDataEnteredInWorkersCompensation()
        {
            foreach (busWssPersonAccountWorkerCompensation lobjWorkersCompensation in iclbMSSWorkerCompensation)
            {
                if ((!String.IsNullOrEmpty(lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.person_name))
                    || (!String.IsNullOrEmpty(lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.company_name))
                    || (!String.IsNullOrEmpty(lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.type_of_injury))
                    || (lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.injury_date != DateTime.MinValue)
                   || (lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.phone_number.IsNotNullOrEmpty()))
                    return true;
            }
            return false;
        }

        //validation is date of injury not entered
        public bool IsDateOfInjuryNotEntered()
        {
            if (IsDataEnteredInWorkersCompensation())
            {
                if (iclbMSSWorkerCompensation.Where(lobjComp => lobjComp.icdoWssPersonAccountWorkerCompensation.injury_date == DateTime.MinValue).Count() > 0)
                    return true;
            }
            return false;
        }

        public void LoadMSSWorkersCompensation()
        {
            if (iclbMSSWorkerCompensation.IsNull())
                iclbMSSWorkerCompensation = new Collection<busWssPersonAccountWorkerCompensation>();

            DataTable ldtbList = Select<cdoWssPersonAccountWorkerCompensation>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
            iclbMSSWorkerCompensation = GetCollection<busWssPersonAccountWorkerCompensation>(ldtbList, "icdoWssPersonAccountWorkerCompensation");
        }

        public void LoadMSSOtherCoverageDetails()
        {
            if (iclbMSSOtherCoverageDetail.IsNull())
                iclbMSSOtherCoverageDetail = new Collection<busWssPersonAccountOtherCoverageDetail>();

            DataTable ldtbList = Select<cdoWssPersonAccountOtherCoverageDetail>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);

            iclbMSSOtherCoverageDetail = GetCollection<busWssPersonAccountOtherCoverageDetail>(ldtbList, "icdoWssPersonAccountOtherCoverageDetail");
        }

        public void LoadMSSGHDV()
        {
            if (icdoMSSGDHV == null)
                icdoMSSGDHV = new cdoWssPersonAccountGhdv();

            DataTable ldtbList = Select<cdoWssPersonAccountGhdv>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
            if (ldtbList.Rows.Count > 0)
                icdoMSSGDHV.LoadData(ldtbList.Rows[0]);
        }

        public bool IsPreviousHealthCoverageSingle(string astrCoverageCode)
        {
            if (astrCoverageCode.IsNotNullOrEmpty())
            {
                if (astrCoverageCode.Contains("0001") || astrCoverageCode.Contains("0004") || astrCoverageCode.Contains("0006") ||
                    astrCoverageCode.Contains("0021") || astrCoverageCode.Contains("0024") || astrCoverageCode.Contains("0044") ||
                    astrCoverageCode.Contains("0041") || astrCoverageCode.Contains("0043") || astrCoverageCode.Contains("0046") || astrCoverageCode.Contains("0048"))
                    return true;
            }
            return false;
        }

        public bool IsCoverageSingle()
        {
            bool lblnResult = false;
            // In GHDV wizard if there is any dependent active as of today, we need to show the tab, we cannot skip it.
            if (iclbMSSPersonDependent.IsNull())
                LoadDependents();
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth)
            {
                if (!string.IsNullOrEmpty(icdoMSSGDHV.coverage_code))
                {
                    if (icdoMSSGDHV.coverage_code.Contains("0001")
                        || icdoMSSGDHV.coverage_code.Contains("0004")
                        || icdoMSSGDHV.coverage_code.Contains("0006")
                        || icdoMSSGDHV.coverage_code.Contains("0021")
                        || icdoMSSGDHV.coverage_code.Contains("0024")
                        || icdoMSSGDHV.coverage_code.Contains("0044")
                        || icdoMSSGDHV.coverage_code.Contains("0041")
                        || icdoMSSGDHV.coverage_code.Contains("0043")
                        || icdoMSSGDHV.coverage_code.Contains("0046")
                        || icdoMSSGDHV.coverage_code.Contains("0048"))
                        lblnResult = true;
                }
            }
            else
            {
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
                {
                    if (!string.IsNullOrEmpty(icdoMSSGDHV.level_of_coverage_value))
                    {
                        if (icdoMSSGDHV.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividual)
                            lblnResult = true;
                    }
                }
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental)
                {
                    if (!string.IsNullOrEmpty(icdoMSSGDHV.level_of_coverage_value))
                    {
                        if (icdoMSSGDHV.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividual)
                            //|| (icdoMSSGDHV.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualChild) -------- PIR - 2514
                            //|| (icdoMSSGDHV.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualSpouse))
                            lblnResult = true;
                    }
                }
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdHMO)
                {
                    if (!string.IsNullOrEmpty(icdoMSSGDHV.level_of_coverage_value))
                    {
                        if (icdoMSSGDHV.level_of_coverage_value == busConstant.HMOLevelOfCoverageSingle)
                            lblnResult = true;
                    }
                }
            }
            return lblnResult;
        }

        #region Dependents

        public Collection<busWssPersonDependent> iclbMSSPersonDependent { get; set; }
        public void LoadDependents()
        {
            if (iclbMSSDependents.IsNull())
                LoadMSSDependents();

            if ((icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusFinishLater
                && iclbMSSPersonDependent.IsNotNull() && iclbMSSDependents.Count < iclbMSSPersonDependent.Count) ||
                (icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollmentStatusPending //PIR 13674
                && icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                && iclbMSSPersonDependent.IsNotNull()) //PIR 14895
            { }// do nothing
            else
                iclbMSSPersonDependent = iclbMSSDependents;

            if (ibusPerson.IsNull())
                LoadPerson();

            if (icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id != 0) //update mode
            {
                // PIR 10369
                DataTable ldtblist = Select<cdoWssPersonDependent>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                    new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
                Collection<busWssPersonDependent> iclbMSSDependent = GetCollection<busWssPersonDependent>(ldtblist, "icdoWssPersonDependent");

                foreach (busWssPersonDependent lobjPersonDependent in iclbMSSPersonDependent)
                {
                    if (iclbMSSDependent.Where(o => o.icdoWssPersonDependent.target_person_dependent_id == lobjPersonDependent.icdoWssPersonDependent.target_person_dependent_id).Any())
                    {
                        int lintDependentPersonId = 0; string lstrDependentName = string.Empty;
                        lintDependentPersonId = lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId;
                        lstrDependentName = lobjPersonDependent.icdoWssPersonDependent.istrDependentName;

                        lobjPersonDependent.icdoWssPersonDependent = iclbMSSDependent.Where(o => o.icdoWssPersonDependent.target_person_dependent_id ==
                            lobjPersonDependent.icdoWssPersonDependent.target_person_dependent_id
                            && (string.IsNullOrEmpty(o.icdoWssPersonDependent.first_name) == false ? o.icdoWssPersonDependent.first_name.Trim().ToLower() : "") == (string.IsNullOrEmpty(lobjPersonDependent.icdoWssPersonDependent.first_name) == false ? lobjPersonDependent.icdoWssPersonDependent.first_name.Trim().ToLower() : "")
                            && (string.IsNullOrEmpty(o.icdoWssPersonDependent.last_name) == false ? o.icdoWssPersonDependent.last_name.Trim().ToLower() : "") == (string.IsNullOrEmpty(lobjPersonDependent.icdoWssPersonDependent.last_name) == false ? lobjPersonDependent.icdoWssPersonDependent.last_name.Trim().ToLower() : "")
                            ).FirstOrDefault().icdoWssPersonDependent;

                        lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId = lintDependentPersonId;
                        lobjPersonDependent.icdoWssPersonDependent.istrDependentName = lstrDependentName;
                    }
                }
            }
            //else //new mode
            //{
            //    if (iclbMSSDependents.IsNull())
            //        LoadMSSDependents();
            //    iclbMSSPersonDependent = iclbMSSDependents;
            //}
            int i = 0;
            foreach (busWssPersonDependent lobjPersonDependent in iclbMSSPersonDependent)
            {
                //below code is required if we assaigning a collection to another one (for eg. sgt_person_dependent to sgt_wss_person_account_Dependent),
                //iobjMainCDO will be pointing to the original one and grid update will fail. Hence need to explicitly assaign the iobjMainCDO
                lobjPersonDependent.iobjMainCDO = lobjPersonDependent.icdoWssPersonDependent;
                lobjPersonDependent.icdoWssPersonDependent.istrDependentName = lobjPersonDependent.icdoWssPersonDependent.first_name + ", "
                    + lobjPersonDependent.icdoWssPersonDependent.middle_name + " ,"
                    + lobjPersonDependent.icdoWssPersonDependent.last_name;
                if (!string.IsNullOrEmpty(lobjPersonDependent.icdoWssPersonDependent.ssn))
                {
                    lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId = busGlobalFunctions.GetPersonIDBySSN(lobjPersonDependent.icdoWssPersonDependent.ssn);
                }
                lobjPersonDependent.icdoWssPersonDependent.iintDependentSeqCount = ++i;
                lobjPersonDependent.icdoWssPersonDependent.iintPersonID = this.ibusPerson.icdoPerson.person_id;
                lobjPersonDependent.ibusEnrollmentRequest = this;
            }
            //PIR - 16666 We need to change current_plan_enrollment_option_value to 'Waived' if  an end_date is future date
            if (iclbMSSPersonDependent.IsNotNull() && iclbMSSPersonDependent.Count > 0)
                iclbMSSPersonDependent.Where(dep => dep.icdoWssPersonDependent.effective_end_date != DateTime.MinValue
                                                    && dep.icdoWssPersonDependent.effective_end_date > DateTime.Now)
                                                    .ForEach(dep => { dep.icdoWssPersonDependent.current_plan_enrollment_option_value = busConstant.PlanOptionStatusValueWaived; dep.icdoWssPersonDependent.PopulateDescriptions(); });
        }

        public Collection<busWssPersonDependent> iclbWSSDependent { get; set; }
        public void LoadWSSPersonDependent()
        {
            DataTable ldtblist = Select<cdoWssPersonDependent>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
            iclbWSSDependent = GetCollection<busWssPersonDependent>(ldtblist, "icdoWssPersonDependent");
            foreach (busWssPersonDependent lobjWSSpersonDependent in iclbWSSDependent)
            {
                if (!string.IsNullOrEmpty(lobjWSSpersonDependent.icdoWssPersonDependent.ssn))
                {
                    lobjWSSpersonDependent.icdoWssPersonDependent.iintDependentPersonId = busGlobalFunctions.GetPersonIDBySSN(lobjWSSpersonDependent.icdoWssPersonDependent.ssn);
                }
            }
        }

        public Collection<busWssPersonDependent> iclbMSSDependents { get; set; }
        public void LoadMSSDependents()
        {
            iclbMSSDependents = new Collection<busWssPersonDependent>();
            if (ibusPerson.IsNull()) LoadPerson();
            Collection<busPersonDependent> lclbPersonDependent = new Collection<busPersonDependent>();
            DataTable ldtbPersonDependent = Select<cdoPersonDependent>(new string[1] { "person_id" }, new object[1] { icdoWssPersonAccountEnrollmentRequest.person_id }, null, null);
            lclbPersonDependent = GetCollection<busPersonDependent>(ldtbPersonDependent, "icdoPersonDependent");
            int lintPersonAccountID = busPersonAccountHelper.GetPersonAccountID(icdoWssPersonAccountEnrollmentRequest.plan_id, icdoWssPersonAccountEnrollmentRequest.person_id);

            foreach (busPersonDependent lobjPersonDependent in lclbPersonDependent)
            {
                if (busGlobalFunctions.GetData1ByCodeValue(321, lobjPersonDependent.icdoPersonDependent.relationship_value, iobjPassInfo) != busConstant.Flag_Yes)
                    lobjPersonDependent.icdoPersonDependent.relationship_value = string.Empty;
                if (busGlobalFunctions.GetDescriptionByCodeValue(6002, lobjPersonDependent.icdoPersonDependent.marital_status_value, iobjPassInfo) == string.Empty)
                    lobjPersonDependent.icdoPersonDependent.marital_status_value = string.Empty;
                if (lobjPersonDependent.iclbPersonAccountDependent.IsNull()) lobjPersonDependent.LoadPersonAccountDependent();
                lobjPersonDependent.LoadDependentInfo();
                var lvar = lobjPersonDependent.iclbPersonAccountDependent.Where(i => i.icdoPersonAccountDependent.person_account_id == lintPersonAccountID &&
                                        i.icdoPersonAccountDependent.end_date_no_null > DateTime.Now).FirstOrDefault();
                if (lvar.IsNotNull())
                    lobjPersonDependent.ibusPeronAccountDependent = lvar;
                else
                    lobjPersonDependent.ibusPeronAccountDependent = new busPersonAccountDependent { icdoPersonAccountDependent = new cdoPersonAccountDependent() };
                if (lobjPersonDependent.icdoPersonDependent.relationship_value != busConstant.DependentRelationshipExSpouse) // PIR 10302
                {
                    // PIR 10695 - Dependent grid after "Save & Quit" - start
                    if (((icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusPendingRequest) || (icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusFinishLater)) //PIR 16624 - Added a condition to load Dependents in update mode.
                        && (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment))
                    {
                        DataTable ldtbWssPersonDependent = Select<cdoWssPersonDependent>(new string[2] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID", "TARGET_PERSON_DEPENDENT_ID" },
                            new object[2] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id, lobjPersonDependent.icdoPersonDependent.person_dependent_id }, null, null);

                        if (ldtbWssPersonDependent.Rows.Count > 0)
                        {
                            busWssPersonDependent lobjWssPersonDependent = new busWssPersonDependent() { icdoWssPersonDependent = new cdoWssPersonDependent() };
                            lobjWssPersonDependent.icdoWssPersonDependent.LoadData(ldtbWssPersonDependent.Rows[0]);
                            lobjWssPersonDependent.icdoPersonDependent = lobjPersonDependent.icdoPersonDependent;
                            iclbMSSDependents.Add(lobjWssPersonDependent);
                        }
                        else
                            iclbMSSDependents.Add(LoadMSSDependents(lobjPersonDependent));
                    } // PIR 10695 - Dependent grid after "Save & Quit" - end
                    else
                        iclbMSSDependents.Add(LoadMSSDependents(lobjPersonDependent));
                }
            }
        }

        /// PIR 6335: To convert busPersonDependent to busWssPersonDependent
        private busWssPersonDependent LoadMSSDependents(busPersonDependent lobjPersonDependent)
        {
            busWssPersonDependent lobjWssPersonDependent = new busWssPersonDependent()
            {
                icdoWssPersonDependent = new cdoWssPersonDependent(),
                ibusPerson = new busPerson { icdoPerson = new cdoPerson() }
            };
            if (ibusPerson.IsNull()) LoadPerson();
            lobjWssPersonDependent.ibusPerson = ibusPerson;
            lobjWssPersonDependent.icdoWssPersonDependent.target_person_dependent_id = lobjPersonDependent.icdoPersonDependent.person_dependent_id;
            lobjWssPersonDependent.icdoWssPersonDependent.first_name = lobjPersonDependent.icdoPersonDependent.dependent_first_name;
            lobjWssPersonDependent.icdoWssPersonDependent.last_name = lobjPersonDependent.icdoPersonDependent.dependent_last_name;
            lobjWssPersonDependent.icdoWssPersonDependent.middle_name = lobjPersonDependent.icdoPersonDependent.dependent_middle_name;
            lobjWssPersonDependent.icdoWssPersonDependent.istrDependentName = lobjWssPersonDependent.icdoWssPersonDependent.first_name + ", "
                + lobjWssPersonDependent.icdoWssPersonDependent.middle_name + " ," + lobjWssPersonDependent.icdoWssPersonDependent.last_name;
            //pir 7790 start
            lobjWssPersonDependent.icdoWssPersonDependent.medicare_claim_no = lobjPersonDependent.icdoPersonDependent.medicare_claim_no;
            lobjWssPersonDependent.icdoWssPersonDependent.medicare_part_a_effective_date = lobjPersonDependent.icdoPersonDependent.medicare_part_a_effective_date;
            lobjWssPersonDependent.icdoWssPersonDependent.medicare_part_b_effective_date = lobjPersonDependent.icdoPersonDependent.medicare_part_b_effective_date;
            //end
            if (lobjPersonDependent.icdoPersonDependent.dependent_perslink_id <= 0)
            {
                lobjWssPersonDependent.icdoWssPersonDependent.gender_id = lobjPersonDependent.icdoPersonDependent.gender_id;
                lobjWssPersonDependent.icdoWssPersonDependent.gender_value = lobjPersonDependent.icdoPersonDependent.gender_value;
                lobjWssPersonDependent.icdoWssPersonDependent.gender_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(lobjWssPersonDependent.icdoWssPersonDependent.gender_id, lobjWssPersonDependent.icdoWssPersonDependent.gender_value);
                lobjWssPersonDependent.icdoWssPersonDependent.marital_status_id = lobjPersonDependent.icdoPersonDependent.marital_status_id;
                lobjWssPersonDependent.icdoWssPersonDependent.marital_status_value = lobjPersonDependent.icdoPersonDependent.marital_status_value;
                lobjWssPersonDependent.icdoWssPersonDependent.marital_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(lobjWssPersonDependent.icdoWssPersonDependent.marital_status_id, lobjWssPersonDependent.icdoWssPersonDependent.marital_status_value);
                lobjWssPersonDependent.icdoWssPersonDependent.date_of_birth = lobjPersonDependent.icdoPersonDependent.date_of_birth;
            }
            else
            {
                lobjPersonDependent.LoadDependentPerson();
                lobjWssPersonDependent.icdoWssPersonDependent.gender_id = lobjPersonDependent.ibusDependentPerson.icdoPerson.gender_id;
                lobjWssPersonDependent.icdoWssPersonDependent.gender_value = lobjPersonDependent.ibusDependentPerson.icdoPerson.gender_value;
                lobjWssPersonDependent.icdoWssPersonDependent.gender_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(lobjWssPersonDependent.icdoWssPersonDependent.gender_id, lobjWssPersonDependent.icdoWssPersonDependent.gender_value);
                lobjWssPersonDependent.icdoWssPersonDependent.marital_status_id = lobjPersonDependent.ibusDependentPerson.icdoPerson.marital_status_id;
                lobjWssPersonDependent.icdoWssPersonDependent.marital_status_value = lobjPersonDependent.ibusDependentPerson.icdoPerson.marital_status_value;
                lobjWssPersonDependent.icdoWssPersonDependent.marital_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(lobjWssPersonDependent.icdoWssPersonDependent.marital_status_id, lobjWssPersonDependent.icdoWssPersonDependent.marital_status_value);
                lobjWssPersonDependent.icdoWssPersonDependent.date_of_birth = lobjPersonDependent.ibusDependentPerson.icdoPerson.date_of_birth;
            }

            lobjWssPersonDependent.icdoWssPersonDependent.ssn = lobjPersonDependent.icdoPersonDependent.dependent_ssn;
            if (!string.IsNullOrEmpty(lobjWssPersonDependent.icdoWssPersonDependent.ssn))
                lobjWssPersonDependent.icdoWssPersonDependent.iintDependentPersonId = busGlobalFunctions.GetPersonIDBySSN(lobjWssPersonDependent.icdoWssPersonDependent.ssn);
            lobjWssPersonDependent.icdoWssPersonDependent.relationship_id = lobjPersonDependent.icdoPersonDependent.relationship_id;
            lobjWssPersonDependent.icdoWssPersonDependent.relationship_value = lobjPersonDependent.icdoPersonDependent.relationship_value;
            lobjWssPersonDependent.icdoWssPersonDependent.relationship_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(lobjWssPersonDependent.icdoWssPersonDependent.relationship_id, lobjWssPersonDependent.icdoWssPersonDependent.relationship_value);

            if (lobjPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.person_account_dependent_id > 0)
            {
                lobjWssPersonDependent.icdoWssPersonDependent.effective_start_date = lobjPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.start_date;
                lobjWssPersonDependent.icdoWssPersonDependent.effective_end_date = lobjPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date;
            }
            else
            {
                lobjWssPersonDependent.icdoWssPersonDependent.effective_start_date = GetChangeEffectiveDate();
                lobjWssPersonDependent.icdoWssPersonDependent.effective_end_date = DateTime.MinValue;
            }
            lobjWssPersonDependent.icdoWssPersonDependent.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
            lobjWssPersonDependent.icdoWssPersonDependent.full_time_student_flag = lobjPersonDependent.icdoPersonDependent.full_time_student_flag;
            if (lobjPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.person_account_dependent_id > 0)
            {
                iblnIsDependentsForCurrentPlanExists = true;
                lobjWssPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value = busConstant.PlanOptionStatusValueEnrolled;
            }
            else
                lobjWssPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value = busConstant.PlanOptionStatusValueWaived;
            lobjWssPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_description = busGlobalFunctions.GetDescriptionByCodeValue(340,
                                            lobjWssPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value, iobjPassInfo);
            lobjWssPersonDependent.icdoPersonDependent = lobjPersonDependent.icdoPersonDependent;
            return lobjWssPersonDependent;
        }

        // PIR 9881 --If Level of Coverage equals the following, the enrollment wizards need to skip the dependents step, even if dependents exist from another plan enrollment:  
        // Ie, If Health was family, dependents were added, Dental enrollment selection is “Individual Only” do not bring forward the dependents, skip the step
        public bool iblnIsDependentsForCurrentPlanExists { get; set; }

        /// <summary>
        ///  pir 6335: Visibility rule for dependent wizard step in ghdv
        /// </summary>
        /// <returns></returns>
        public bool VisibilityRuleForDependentStep()
        {
            bool lblnresult = true;
            if (IsCoverageSingle() || (IsMemberAccountExists && icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag == busConstant.Flag_No))
                lblnresult = false;
            return lblnresult;
        }

        public bool VisibilityRuleForDependentsReviewStep()
        {
            bool lblnresult = true;
            // PIR 10395 - comment code so that Dependents tab will be visible
            //if (IsMemberAccountExists && icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag == busConstant.Flag_No)
            //    lblnresult = false;
            if (IsCoverageSingle())
            {
                lblnresult = false;
                // Dependent tab should be visible in Review Step if Previous coverage is not single
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth && ibusMSSGHDVWeb.IsNotNull() &&
                    !ibusMSSGHDVWeb.IsCoverageCodeCodeSingle())
                    lblnresult = true;

                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental && ibusMSSGHDVWeb.IsNotNull() &&
                    ibusMSSGHDVWeb.icdoPersonAccountGhdv.level_of_coverage_value != busConstant.DentalLevelofCoverageIndividual)
                    lblnresult = true;

                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision && ibusMSSGHDVWeb.IsNotNull() &&
                    ibusMSSGHDVWeb.icdoPersonAccountGhdv.level_of_coverage_value != busConstant.VisionLevelofCoverageIndividual)
                    lblnresult = true;
            }
            return lblnresult;
        }

        /// <summary>
        ///  PIR 11840: Waived dependent should not be added if person account dependent does not exits
        /// </summary>
        /// <returns></returns>
        public bool IsDependentInsertRequired(busWssPersonDependent aobjPersonDependent)
        {
            bool lblnPersonAccountDependentExist = false;
            DataTable ldtbList = busNeoSpinBase.Select("cdoPersonAccountDependent.GetPersonAccountDependentId", new object[2] { ibusPersonAccount.icdoPersonAccount.person_account_id, aobjPersonDependent.icdoWssPersonDependent.target_person_dependent_id });
            if (ldtbList.Rows.Count > 0)
                lblnPersonAccountDependentExist = true;
            else
                lblnPersonAccountDependentExist = false;

            if ((aobjPersonDependent.icdoWssPersonDependent.effective_start_date == aobjPersonDependent.icdoWssPersonDependent.effective_end_date)
                    && (aobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueWaived)
                    && (!lblnPersonAccountDependentExist))
            {
                return false;
            }

            return true;
        }

        private ArrayList InsertDepedents(Collection<busWssPersonDependent> aclbPersonDependent)
        {
            if (ibusPerson.IsNull()) LoadPerson();
            ArrayList larrList = new ArrayList();
            if (iclbWSSDependent.IsNull()) LoadWSSPersonDependent();
            foreach (busWssPersonDependent lobjPersonDependent in iclbWSSDependent)
            {
                if (IsDependentInsertRequired(lobjPersonDependent)) // PIR 11840
                {
                    busPersonDependent lobjNewPersonDependent = new busPersonDependent
                    {
                        icdoPersonDependent = new cdoPersonDependent()
                    };
                    lobjNewPersonDependent.icdoPersonDependent.person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
                    lobjNewPersonDependent.LoadPerson();
                    if (lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId > 0)
                    {
                        lobjNewPersonDependent = GetPersonDependentRecordIfAlreadyExists(lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId);
                        if (lobjNewPersonDependent.icdoPersonDependent.person_dependent_id == 0 &&
                            lobjPersonDependent.icdoWssPersonDependent.target_person_dependent_id > 0)
                            lobjNewPersonDependent.FindPersonDependent(lobjPersonDependent.icdoWssPersonDependent.target_person_dependent_id);
                        lobjNewPersonDependent.icdoPersonDependent.person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
                        lobjNewPersonDependent.LoadPerson();
                        lobjNewPersonDependent.icdoPersonDependent.dependent_perslink_id = lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId;
                        lobjNewPersonDependent.ibusDependentPerson = new busPerson { icdoPerson = new cdoPerson() };
                        lobjNewPersonDependent.LoadDependentPerson();

                        if (lobjNewPersonDependent.ibusDependentPerson.icdoPerson.person_id > 0)
                        {
                            lobjNewPersonDependent.icdoPersonDependent.dependent_perslink_id = lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId;
                            SetPersonProperties(lobjNewPersonDependent.ibusDependentPerson, lobjPersonDependent);
                            lobjNewPersonDependent.ibusDependentPerson.icdoPerson.Update();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(lobjPersonDependent.icdoWssPersonDependent.ssn))
                            {
                                lobjNewPersonDependent.ibusDependentPerson = new busPerson { icdoPerson = new cdoPerson() };
                                SetPersonProperties(lobjNewPersonDependent.ibusDependentPerson, lobjPersonDependent);
                                lobjNewPersonDependent.ibusDependentPerson.icdoPerson.Insert();
                                lobjNewPersonDependent.icdoPersonDependent.dependent_perslink_id = lobjNewPersonDependent.ibusDependentPerson.icdoPerson.person_id;
                            }
                            else
                            {
                                SetPersonDependentProperties(lobjNewPersonDependent, lobjPersonDependent);
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(lobjPersonDependent.icdoWssPersonDependent.ssn))
                        {
                            lobjNewPersonDependent.ibusDependentPerson = new busPerson { icdoPerson = new cdoPerson() };
                            SetPersonProperties(lobjNewPersonDependent.ibusDependentPerson, lobjPersonDependent);
                            lobjNewPersonDependent.ibusDependentPerson.icdoPerson.Insert();
                            lobjNewPersonDependent.icdoPersonDependent.dependent_perslink_id = lobjNewPersonDependent.ibusDependentPerson.icdoPerson.person_id;
                        }
                        else
                        {
                            lobjNewPersonDependent = GetPersonDependentRecordIfAlreadyExistsByDependentName(lobjPersonDependent);
                            if (lobjNewPersonDependent.icdoPersonDependent.person_dependent_id == 0 &&
                                lobjPersonDependent.icdoWssPersonDependent.target_person_dependent_id > 0)
                                lobjNewPersonDependent.FindPersonDependent(lobjPersonDependent.icdoWssPersonDependent.target_person_dependent_id);
                            lobjNewPersonDependent.icdoPersonDependent.person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
                            lobjNewPersonDependent.LoadPerson();
                            SetPersonDependentProperties(lobjNewPersonDependent, lobjPersonDependent);
                        }
                    }

                    // PIR 10367 -- Request wont post if the relationship is empty.
                    if (lobjPersonDependent.icdoWssPersonDependent.relationship_value != string.Empty)
                        lobjNewPersonDependent.icdoPersonDependent.relationship_value = lobjPersonDependent.icdoWssPersonDependent.relationship_value;
                    //pir 7790
                    if (icdoMSSGDHV.is_dependent_medicare_eligible == "Y" || icdoMSSGDHV.is_dependent_medicare_esrd == "Y")
                    {
                        lobjNewPersonDependent.icdoPersonDependent.medicare_claim_no = lobjPersonDependent.icdoWssPersonDependent.medicare_claim_no;
                        lobjNewPersonDependent.icdoPersonDependent.medicare_part_a_effective_date = lobjPersonDependent.icdoWssPersonDependent.medicare_part_a_effective_date;
                        lobjNewPersonDependent.icdoPersonDependent.medicare_part_b_effective_date = lobjPersonDependent.icdoWssPersonDependent.medicare_part_b_effective_date;
                    }

                    //PIR 24470
                    lobjPersonDependent.icdoWssPersonDependent.target_person_dependent_id = lobjNewPersonDependent.icdoPersonDependent.person_dependent_id;

                    if (lobjPersonDependent.icdoWssPersonDependent.target_person_dependent_id > 0)
                    {
                        lobjNewPersonDependent.icdoPersonDependent.mss_person_dependent_id = lobjPersonDependent.icdoWssPersonDependent.target_person_dependent_id;
                        lobjNewPersonDependent.icdoPersonDependent.ienuObjectState = ObjectState.Update;
                        lobjNewPersonDependent.LoadPersonAccountDependent();
                        lobjNewPersonDependent.ibusPeronAccountDependent = lobjNewPersonDependent.iclbPersonAccountDependent
                                                                                .Where(o => o.icdoPersonAccountDependent.person_dependent_id == lobjNewPersonDependent.icdoPersonDependent.person_dependent_id &&
                                                                                    o.icdoPersonAccountDependent.person_account_id == ibusMSSPersonAccountGHDV.icdoPersonAccount.person_account_id).FirstOrDefault();
                    }
                    else
                    {
                        lobjNewPersonDependent.icdoPersonDependent.same_as_member_address = busConstant.Flag_Yes;
                        lobjNewPersonDependent.icdoPersonDependent.ienuObjectState = ObjectState.Insert;
                    }

                    if (lobjNewPersonDependent.ibusPeronAccountDependent == null)
                    {
                        lobjNewPersonDependent.ibusPeronAccountDependent = new busPersonAccountDependent { icdoPersonAccountDependent = new cdoPersonAccountDependent() };
                        lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.ienuObjectState = ObjectState.Insert;
                    }
                    else
                    {
                        lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.ienuObjectState = ObjectState.Update;
                    }

                    lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.person_account_id = ibusMSSPersonAccountGHDV.icdoPersonAccount.person_account_id;

                    //PIR 16666 - We need to change the logic so if an end date exists for dependents we don’t want to update with another end date
                    if (lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date == DateTime.MinValue
                        && lobjPersonDependent.icdoWssPersonDependent.effective_end_date != DateTime.MinValue
                        && lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.ienuObjectState == ObjectState.Update)
                    {
                        lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date = lobjPersonDependent.icdoWssPersonDependent.effective_end_date;
                    }
                    //PIR 25776 New hire MSS enrollments are posting with conflicting insurance and dependent effective dates
                    //lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.start_date = lobjPersonDependent.icdoWssPersonDependent.effective_start_date;
                    if (lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.start_date == DateTime.MinValue)
                        lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;


                    if (lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.start_date != DateTime.MinValue && lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date!= DateTime.MinValue
                        && lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueEnrolled)
                        lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;



                    lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.plan_id = icdoWssPersonAccountEnrollmentRequest.plan_id; //PIR 11841
                    if (lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.ienuObjectState == ObjectState.Update &&
                        lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueEnrolled &&
                        lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date != DateTime.MinValue)
                    {
                        lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date = DateTime.MinValue;
                    }
                    if (lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date == DateTime.MinValue)
                        lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag = null;
                    //Backlog PIR 8015 (PIR 16462, PIR 11040)
                    else if (lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.end_date != DateTime.MinValue &&
                             lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag != busConstant.Flag_Yes)
                        lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag = busConstant.Flag_No;
                    
                    lobjNewPersonDependent.iclbPersonAccountDependent = new Collection<busPersonAccountDependent>();
                    lobjNewPersonDependent.iclbPersonAccountDependent.Add(lobjNewPersonDependent.ibusPeronAccountDependent);
                    //pir 6270 : no need to validate address on posting of enrollment
                    lobjNewPersonDependent.iblnFromPortal = true;
                    lobjNewPersonDependent.BeforeValidate(utlPageMode.New);
                    lobjNewPersonDependent.ValidateHardErrors(utlPageMode.New);
                    if (lobjNewPersonDependent.iarrErrors.Count > 0)
                    {
                        foreach (utlError lobjError in lobjNewPersonDependent.iarrErrors)
                        {
                            if (lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueEnrolled) // PIR 10327
                                larrList.Add(lobjError);
                        }
                        break;
                    }
                    lobjNewPersonDependent.PersistChanges();
                    lobjNewPersonDependent.AfterPersistChanges();

                    lobjPersonDependent.icdoWssPersonDependent.target_person_dependent_id = lobjNewPersonDependent.icdoPersonDependent.person_dependent_id;

                    lobjPersonDependent.icdoWssPersonDependent.effective_start_date = lobjNewPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.start_date;


                    if (lobjPersonDependent.icdoWssPersonDependent.wss_person_dependent_id > 0)
                        lobjPersonDependent.icdoWssPersonDependent.Update();
                }
            }
            return larrList;
        }

        private busPersonDependent GetPersonDependentRecordIfAlreadyExists(int aintDependentPersonId)
        {
            busPersonDependent lobjPersonDependent = new busPersonDependent { icdoPersonDependent = new cdoPersonDependent() };

            if (ibusPerson.IsNull())
                LoadPerson();

            if (ibusPerson.iclbPersonDependent.IsNull())
                ibusPerson.LoadDependent();

            var lenumList = ibusPerson.iclbPersonDependent.Where(lobjPD => lobjPD.icdoPersonDependent.dependent_perslink_id == aintDependentPersonId);
            if (lenumList.Count() > 0)
                lobjPersonDependent = lenumList.FirstOrDefault();

            return lobjPersonDependent;
        }

        private busPersonDependent GetPersonDependentRecordIfAlreadyExistsByDependentName(busWssPersonDependent aobjWSSPersonDependent)
        {
            busPersonDependent lobjPersonDependent = new busPersonDependent { icdoPersonDependent = new cdoPersonDependent() };

            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbPersonDependent.IsNull())
                ibusPerson.LoadDependent();
            var lenumlist1 = ibusPerson.iclbPersonDependent.Where(lobjPD => lobjPD.icdoPersonDependent.first_name.IsNotNullOrEmpty() && lobjPD.icdoPersonDependent.last_name.IsNotNullOrEmpty());
            var lenumlist = lenumlist1.Where(lobjPD => lobjPD.icdoPersonDependent.first_name.ToUpper() == aobjWSSPersonDependent.icdoWssPersonDependent.first_name.ToUpper() &&
                                                lobjPD.icdoPersonDependent.last_name.ToUpper() == aobjWSSPersonDependent.icdoWssPersonDependent.last_name.ToUpper());
            if (lenumlist.Count() > 0)
            {
                lobjPersonDependent = lenumlist.FirstOrDefault();
            }
            return lobjPersonDependent;
        }

        #endregion

        public Collection<cdoCodeValue> LoadMSSLevelOfCoverageByPlan()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();

            return ibusPersonAccount.LoadLevelOfCoverageByPlan();
        }

        public Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> LoadMssCoverageCode()
        {
            if (ibusMSSPersonAccountGHDV.IsNull())
                LoadPersonAccountGHDV();

            ibusMSSPersonAccountGHDV.ibusPlan = new busPlan();
            ibusMSSPersonAccountGHDV.ibusPlan = ibusPlan;
            ibusPersonAccount.icdoPersonAccount.person_id = ibusPersonAccount.icdoPersonAccount.person_id == 0 ? ibusPerson.icdoPerson.person_id : ibusPersonAccount.icdoPersonAccount.person_id;
            ibusMSSPersonAccountGHDV.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;

            ibusMSSPersonAccountGHDV.icdoPersonAccount.plan_id = icdoWssPersonAccountEnrollmentRequest.plan_id;

            ibusMSSPersonAccountGHDV.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
            ibusMSSPersonAccountGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment = ibusPersonEmploymentDetail.ibusPersonEmployment;
            ibusMSSPersonAccountGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;

            if (ibusMSSPersonAccountGHDV.IsHealthOrMedicare)
            {
                if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment || icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonACAAnnualEnrollment)
                    ibusMSSPersonAccountGHDV.LoadOrgPlan(icdoWssPersonAccountEnrollmentRequest.date_of_change);
                else
                    ibusMSSPersonAccountGHDV.LoadOrgPlan(DateTime.Now.GetFirstDayofNextMonth());
                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.plan_option_value = ibusMSSPersonAccountGHDV.ibusOrgPlan.icdoOrgPlan.plan_option_value;
                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value = ibusMSSPersonAccountGHDV.ibusOrgPlan.HealthInsuranceType;
                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.employment_type_value = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;

                //ibusMSSPersonAccountGHDV.btnReloadCoverageCodeList_Click(); //PIR 23289
                ibusMSSPersonAccountGHDV.LoadCoverageCodeByFilter();
            }

            return ibusMSSPersonAccountGHDV.iclbCoverageRef;
        }

        public Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> LoadCoverageCodeForMSS()
        {
            Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> lclbCoverageRef = new Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>();
            Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> lclbFinalCoverageRef = new Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>();
            lclbCoverageRef = LoadMssCoverageCode();
            foreach (cdoOrgPlanGroupHealthMedicarePartDCoverageRef lcdoCoverageRef in lclbCoverageRef)
            {
                int lintIndex = lcdoCoverageRef.short_description.LastIndexOf('-');
                if (lintIndex > 0)
                    lcdoCoverageRef.short_description = lcdoCoverageRef.short_description.Substring(lintIndex + 2);
                if (lcdoCoverageRef.short_description != "Dual" && lcdoCoverageRef.short_description != "DUAL" && lcdoCoverageRef.short_description != string.Empty)
                {
                    // PIR 16314
                    if (!lclbFinalCoverageRef.Where(i => i.short_description.Equals(lcdoCoverageRef.short_description)).Any()) // PIR 9788
                        lclbFinalCoverageRef.Add(lcdoCoverageRef);
                }
            }
            return lclbFinalCoverageRef;
        }

        /// <summary>
        /// PIR 20902
        /// </summary>
        public void LoadPersonAccountGHDVHistory()
        {
            DataTable ldtbHistory = Select("cdoPersonAccountGhdv.LoadPersonAccountGHDVHistory", new object[1] { ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id });
            ibusMSSPersonAccountGHDVHsa.iclbPersonAccountGHDVHistory = new Collection<busPersonAccountGhdvHistory>();
            ibusMSSPersonAccountGHDVHsa.iclbPersonAccountGHDVHistory = GetCollection<busPersonAccountGhdvHistory>(ldtbHistory, "icdoPersonAccountGhdvHistory");
        }
        public ArrayList btnPostGHDV_Click()
        {
            bool lblnIsHistoryHDHPNotExist = false;
            ArrayList larrList = new ArrayList();
            bool lblnIsErrorFound = false;
            if (icdoWssPersonAccountEnrollmentRequest.wss_ben_app_id == 0) //active member's enrollment posting
            {
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccountForPosting();
                if (ibusMSSPersonAccountGHDV.IsNull())
                    LoadPersonAccountGHDV();
                if (ibusMSSPersonAccountGHDVHsa.IsNull())
                    LoadPersonAccountGHDVHsa();
                if (ibusPersonEmploymentDetail.IsNull())
                    LoadPersonEmploymentDetail();
                if (ibusPersonAccountEmploymentDetail.IsNull())
                    LoadPersonAccountEmploymentDetail();
                ibusMSSPersonAccountGHDV.iblnIsFromMSS = true; // PIR 10241
                //PIR 20902            
                LoadPersonAccountGHDVHistory();
                //PIR 26799 When completing MSS wizards for enrollment changes the system should not allow past dates to be entered when ending Dependents. Date should only allow future.
                if (iclbWSSDependent.IsNull()) LoadWSSPersonDependent();
                foreach (busWssPersonDependent lobjPersonDependent in iclbWSSDependent)
                {
                    if (icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ChangeReasonAnnualEnrollment && !iblnIsFromPortal)
                    {
                        if (lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueWaived &&
                            lobjPersonDependent.icdoWssPersonDependent.effective_start_date == icdoWssPersonAccountEnrollmentRequest.change_effective_date)
                        {
                            lobjPersonDependent.icdoWssPersonDependent.effective_end_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
                        }
                        if (lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueWaived &&
                            lobjPersonDependent.icdoWssPersonDependent.effective_start_date < icdoWssPersonAccountEnrollmentRequest.change_effective_date)
                        {
                            lobjPersonDependent.icdoWssPersonDependent.effective_end_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date.AddDays(-1);
                        }
                    }                   
                }
                if (ibusMSSPersonAccountGHDVHsa.iclbPersonAccountGHDVHistory.Where(m => m.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP).Count() == 0)
                    lblnIsHistoryHDHPNotExist = true;
                else
                    lblnIsHistoryHDHPNotExist = false;

                if (icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag != busConstant.Flag_Yes)
            {
                if (!IsAllAcknowledgementAreNotSelected())
                {
                    if (icdoWssPersonAccountEnrollmentRequest.change_effective_date == DateTime.MinValue)
                    {
                        utlError lerr = new utlError();
                        lerr.istrErrorID = "10014";
                        lerr.istrErrorMessage = "Change Effective Date is required.";
                        larrList.Add(lerr);
                        return larrList;
                    }

                    if (icdoWssPersonAccountEnrollmentRequest.reason_value == "ANNE" && icdoWssPersonAccountEnrollmentRequest.status_value == "PEND" && !iblnFinishButtonClicked &&
                        icdoWssPersonAccountEnrollmentRequest.change_effective_date < icdoWssPersonAccountEnrollmentRequest.date_of_change)
                    {
                        utlError lerr = new utlError();
                        lerr.istrErrorMessage = "Effective date is less than Current Annual Enrollment Plan Year";
                        larrList.Add(lerr);
                        return larrList;
                    }

                    else
                    {
                        if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                            ibusPersonEmploymentDetail.LoadPersonEmployment();
                        if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                            ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                        if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                        {
                                iblnIsNewHDHPEnrollment = true; //PIR20902
                                ibusMSSPersonAccountGHDV = CreateGHDVPersonAccount();
                            if (ibusMSSPersonAccountGHDV.iarrErrors.Count > 0)
                            {
                                foreach (utlError lobjError in ibusMSSPersonAccountGHDV.iarrErrors)
                                {
                                    lblnIsErrorFound = true;
                                    larrList.Add(lobjError);
                                }
                                return larrList;
                            }
                            //19997                            
                            if (icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP && icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_amount >= 0 
                                && icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date != DateTime.MinValue)
                            {
                                ibusMSSPersonAccountGHDVHsa = CreatePersonAccountGHDVHSA();
                                if (ibusMSSPersonAccountGHDVHsa.iarrErrors.Count > 0)
                                {
                                    lblnIsErrorFound = true;
                                    foreach (utlError lobjErr in ibusMSSPersonAccountGHDVHsa.iarrErrors)
                                    {
                                        lblnIsErrorFound = true;
                                        larrList.Add(lobjErr);
                                    }
                                    return larrList;
                                }
                            }
                        }
                        else
                        {
                                iblnIsNewHDHPEnrollment = false;  //PIR 20902 //PI
                                if (ibusMSSPersonAccountGHDV.IsNull())
                                LoadPersonAccountGHDV();
                            ibusMSSPersonAccountGHDV.ibusPerson = ibusPerson;
                            ibusMSSPersonAccountGHDV.ibusPlan = ibusPlan;
                            ibusMSSPersonAccountGHDV.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
                            if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueCancel)
                                ibusMSSPersonAccountGHDV.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceSuspended;
                            else
                            {
                                ibusMSSPersonAccountGHDV.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
                                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth)
                                    ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = icdoMSSGDHV.coverage_code;
                                else
                                    ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = icdoMSSGDHV.level_of_coverage_value;
                            }
                            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.reason_value = icdoWssPersonAccountEnrollmentRequest.reason_value;
                            ibusMSSPersonAccountGHDV.icdoPersonAccount.history_change_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
                            ibusMSSPersonAccountGHDV.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.premium_conversion_indicator_flag = icdoMSSGDHV.pre_tax_payroll_deduction_flag ?? busConstant.Flag_No;
                            ibusMSSPersonAccountGHDV.LoadPlanEffectiveDate();
                            ibusMSSPersonAccountGHDV.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
                            ibusMSSPersonAccountGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment = ibusPersonEmploymentDetail.ibusPersonEmployment;
                            ibusMSSPersonAccountGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
                            ibusMSSPersonAccountGHDV.LoadOrgPlan(ibusMSSPersonAccountGHDV.idtPlanEffectiveDate);
                            ibusMSSPersonAccountGHDV.ibusOrgPlan.ibusOrganization = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
                            ibusMSSPersonAccountGHDV.LoadProviderOrgPlan(ibusMSSPersonAccountGHDV.idtPlanEffectiveDate);
                            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.keeping_other_coverage_flag = icdoMSSGDHV.keeping_other_coverage_flag;
                            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_claim_no = icdoMSSGDHV.medicare_claim_no; // PIR 7790
                            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_a_effective_date = icdoMSSGDHV.medicare_part_a_effective_date;
                            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_b_effective_date = icdoMSSGDHV.medicare_part_b_effective_date;
                            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.employment_type_value = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
                            ibusMSSPersonAccountGHDV.icdoPersonAccount.is_from_mss = true;
                            ibusMSSPersonAccountGHDV.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                            if (icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP)
                            {
                                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.alternate_structure_code_value = busConstant.AlternateStructureCodeHDHP;
                                // PIR 10916 - As per Mail from Maik dated 03/08/2013.Update HSA_EFFECTIVE_DATE  only if it is NULL
                                if (ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date == DateTime.MinValue)
                                {
                                    ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
                                }
                            }
                            else
                            {
                                if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value != busConstant.PlanEnrollmentOptionValueCancel) //PIR 12721
                                {
                                    // PIR 10954 - Clear HDHP values if not entered.
                                    ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.alternate_structure_code_value = string.Empty;
                                    ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date = DateTime.MinValue;
                                }
                            }
                            //PIR - 12652 - Health rate structure code issue
                            if (
                                //icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth && //PIR 16276 - adjustments going to old org
                                icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll
                                && ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id == 0)
                            {
                                if (ibusMSSPersonAccountGHDV.iclbAccountEmploymentDetail.IsNull())
                                    ibusMSSPersonAccountGHDV.LoadPersonAccountEmploymentDetails();
                                ibusMSSPersonAccountGHDV.iclbAccountEmploymentDetail.Add(ibusPersonAccountEmploymentDetail);
                            }
                            ibusMSSPersonAccountGHDV.BeforeValidate(utlPageMode.All);
                            //this is temporary fix. need to change for COBRA
                            {
                                ibusMSSPersonAccountGHDV.iblnIsActiveMember = true;
                                ibusMSSPersonAccountGHDV.iblnIsCobraMember = false;
                            }
                            //PIR 11209
                            //ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value = ibusMSSPersonAccountGHDV.ibusOrgPlan.HealthInsuranceType;
                            //PIR 15544

                            if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll && icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id > 0)
                            {
                                ibusMSSPersonAccountGHDV.LoadPaymentElection();
                                if (ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
                                {
                                    ibusMSSPersonAccountGHDV.UpdateIBSFlagForActive();
                                    ibusMSSPersonAccountGHDV.iblnIsRetireeToActive = true;//PIR 26961 - Payment election not updated when enrolling as new hire from MSS 
                                }
                            }

                            //PIR 18155
                            if (icdoWssPersonAccountEnrollmentRequest.reason_value == "ANNE" && (ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree ||
                                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree ||
                                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree))
                            {
                                LoadGHDVInsuranceTypeValue(ibusMSSPersonAccountGHDV);
                                ibusMSSPersonAccountGHDV.iblnIsRetireeToActive = true;
                            }

                            ibusMSSPersonAccountGHDV.ValidateHardErrors(utlPageMode.All);
                            if (ibusMSSPersonAccountGHDV.iarrErrors.Count > 0)
                            {
                                lblnIsErrorFound = true;
                                foreach (utlError lobjErr in ibusMSSPersonAccountGHDV.iarrErrors)
                                {
                                    lblnIsErrorFound = true;
                                    larrList.Add(lobjErr);
                                }
                                return larrList;
                            }
                            else
                            {
                                ibusMSSPersonAccountGHDV.BeforePersistChanges();
                                ibusMSSPersonAccountGHDV.PersistChanges();
                                ibusMSSPersonAccountGHDV.ValidateSoftErrors();
                                ibusMSSPersonAccountGHDV.UpdateValidateStatus();
                                //this is temporary fix. need to change for COBRA
                                ibusMSSPersonAccountGHDV.iblnIsActiveMember = true;
                                ibusMSSPersonAccountGHDV.icdoPersonAccount.status_value = busConstant.StatusValid;

                                ibusMSSPersonAccountGHDV.iblnIsFromMSSForEnrollmentData = true;//PIR 20017
                                ibusMSSPersonAccountGHDV.AfterPersistChanges();
                            }
                            //19997 
                            if (icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP && icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_amount >= 0 && icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date != DateTime.MinValue)
                            {
                                ibusMSSPersonAccountGHDVHsa = CreatePersonAccountGHDVHSA();
                                if (ibusMSSPersonAccountGHDVHsa.iarrErrors.Count > 0)
                                {
                                    lblnIsErrorFound = true;
                                    foreach (utlError lobjErr in ibusMSSPersonAccountGHDVHsa.iarrErrors)
                                    {
                                        lblnIsErrorFound = true;
                                        larrList.Add(lobjErr);
                                    }
                                    return larrList;
                                }
                            }
                        }
                        if (!lblnIsErrorFound)
                        {
                            // Manage Dependents
                            ArrayList larrReturnErrorList = new ArrayList();
                            larrReturnErrorList = InsertDepedents(iclbMSSPersonDependent);
                            if (larrReturnErrorList.Count > 0)
                            {
                                foreach (utlError lobjError in larrReturnErrorList)
                                {
                                    lblnIsErrorFound = true;
                                    larrList.Add(lobjError);
                                }
                                return larrList;
                            }

                            // Workers compensation
                            larrReturnErrorList = InsertWorkersCompensation();
                            if (larrReturnErrorList.Count > 0)
                            {
                                foreach (utlError lobjError in larrReturnErrorList)
                                {
                                    lblnIsErrorFound = true;
                                    larrList.Add(lobjError);
                                }
                                return larrList;
                            }
                            // Other coverage details
                            larrReturnErrorList = InsertOtherCoverageDetails();
                            if (larrReturnErrorList.Count > 0)
                            {
                                iblnIsMemberAccountInReviewStatus = true;
                                foreach (utlError lobjError in larrReturnErrorList)
                                {
                                    lblnIsErrorFound = true;
                                    larrList.Add(lobjError);
                                }
                                return larrList;
                            }

                            if (larrReturnErrorList.Count == 0)
                            {
                                //PIR-10379 Start
                                if (ibusPerson == null)
                                    LoadPerson();
                                ibusPerson.LoadActivePersonEmployment();
                                Collection<busPersonAccountEmploymentDetail> lclbPersonAccountEmploymentDetail = new Collection<busPersonAccountEmploymentDetail>();
                                int lintcount = 0;
                                foreach (busPersonEmployment lobjemp in ibusPerson.iclbActivePersonEmployment)
                                {

                                    lobjemp.LoadLatestPersonEmploymentDetail();
                                    lobjemp.ibusLatestEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
                                    if (lintcount == 0)
                                        lclbPersonAccountEmploymentDetail = lobjemp.ibusLatestEmploymentDetail.iclbAllPersonAccountEmpDtl;
                                    else
                                        lclbPersonAccountEmploymentDetail = lclbPersonAccountEmploymentDetail.Union(lobjemp.ibusLatestEmploymentDetail.iclbAllPersonAccountEmpDtl).ToList().ToCollection();

                                    lintcount++;

                                }
                                lclbPersonAccountEmploymentDetail =
                                    lclbPersonAccountEmploymentDetail.Where(o => o.icdoPersonAccountEmploymentDetail.plan_id == icdoWssPersonAccountEnrollmentRequest.plan_id).ToList().ToCollection();

                                //Checks if there is Enrolled plan in for diffrent person employment
                                if (lclbPersonAccountEmploymentDetail.Where(o =>
                                                o.icdoPersonAccountEmploymentDetail.person_employment_dtl_id != icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id
                                                && o.icdoPersonAccountEmploymentDetail.person_account_id > 0).Count() > 0)
                                {
                                    //Filters Enrolled plan in for diffrent person employment
                                    busPersonAccountEmploymentDetail lobjPersAcctEmpdetail = lclbPersonAccountEmploymentDetail.Where(o => o.icdoPersonAccountEmploymentDetail.person_employment_dtl_id != icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id
                                        && o.icdoPersonAccountEmploymentDetail.person_account_id > 0).FirstOrDefault();

                                    lobjPersAcctEmpdetail.icdoPersonAccountEmploymentDetail.person_account_id = 0;
                                    lobjPersAcctEmpdetail.icdoPersonAccountEmploymentDetail.election_value = null;
                                    lobjPersAcctEmpdetail.icdoPersonAccountEmploymentDetail.Update();
                                    //Filters the Current Enrollment request EMployment and update the Person Account Employment Details 
                                    busPersonAccountEmploymentDetail lobjPersAcctEmpDetailEnroll =
                                    lclbPersonAccountEmploymentDetail.Where(o => o.icdoPersonAccountEmploymentDetail.person_employment_dtl_id == icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id).FirstOrDefault();
                                    if (lobjPersAcctEmpDetailEnroll.IsNotNull()) //PIR 13655
                                    {
                                        lobjPersAcctEmpDetailEnroll.icdoPersonAccountEmploymentDetail.person_account_id = ibusMSSPersonAccountGHDV.icdoPersonAccount.person_account_id;
                                        lobjPersAcctEmpDetailEnroll.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                                        lobjPersAcctEmpDetailEnroll.icdoPersonAccountEmploymentDetail.Update();
                                    }
                                    else
                                    {
                                        utlError lerr = new utlError();
                                        lerr.istrErrorID = "10223";
                                        lerr.istrErrorMessage = "Enrollment request is not linked to current Employment detail record.";
                                        larrList.Add(lerr);
                                        return larrList;
                                    }
                                }
                                else
                                {
                                    ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = ibusMSSPersonAccountGHDV.icdoPersonAccount.person_account_id;
                                    ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                                    ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();
                                }
                                //PIR-10379 End

                                if (ibusMSSPersonAccountGHDV.ibusPaymentElection.IsNull())
                                    ibusMSSPersonAccountGHDV.LoadPaymentElection();

                                //PIR 20017
                                if (ibusMSSPersonAccountGHDV.icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && ibusMSSPersonAccountGHDV.IsHistoryEntryRequired &&
                                    ((ibusMSSPersonAccountGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth && !(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.is_health_cobra || ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree))
                                    || (ibusMSSPersonAccountGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdDental && !(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA || ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree))
                                    || (ibusMSSPersonAccountGHDV.icdoPersonAccount.plan_id == busConstant.PlanIdVision && !(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA || ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree)))
                                    && ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag != busConstant.Flag_Yes)
                                    { 
                                        ibusMSSPersonAccountGHDV.InsertIntoEnrollmentData();


                                        //PIR 20902      - inserting default HSA entry in Enrollment data table  for HDHP                        
                                        if (icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP &&
                                            lblnIsHistoryHDHPNotExist && IsMemberHasHDHPPlan())
                                        {
                                            ibusMSSPersonAccountGHDV.InsertIntoEnrollmentDataDefaultEntryForHSA();
                                        }
                                        else  //PIR 21077 - 1. HDHP family to single or single to family, 2. Suspend HDHP, 3. HDHP to PPO
                                        {
                                            busPersonAccountGhdvHistory lbusPersonAccountGhdvHistory = new busPersonAccountGhdvHistory();
                                            if (ibusMSSPersonAccountGHDV.iclbPersonAccountGHDVHistory == null)
                                                ibusMSSPersonAccountGHDV.LoadPersonAccountGHDVHistory();
                                            if (ibusMSSPersonAccountGHDV.iclbPersonAccountGHDVHistory.Count >= 2)
                                                lbusPersonAccountGhdvHistory = ibusMSSPersonAccountGHDV.iclbPersonAccountGHDVHistory[1];
                                            else
                                                lbusPersonAccountGhdvHistory = ibusMSSPersonAccountGHDV.iclbPersonAccountGHDVHistory.FirstOrDefault();

                                            if ((icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP &&
                                                (icdoMSSGDHV.coverage_code != lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.coverage_code ||
                                                ibusMSSPersonAccountGHDV.icdoPersonAccount.plan_participation_status_value != lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value))
                                                || (icdoMSSGDHV.type_of_coverage_value != lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value
                                                && lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP))
                                            {
                                                busPersonAccountGhdv lbusPersonAccountGhdv = new busPersonAccountGhdv();
                                                lbusPersonAccountGhdv.FindGHDVByPersonAccountID(ibusPersonAccount.icdoPersonAccount.person_account_id);
                                                lbusPersonAccountGhdv.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
                                                if (lbusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                                                    lbusPersonAccountGhdv.InsertIntoEnrollmentDataDefaultEntryForHSA();
                                            }
                                        }
                                    }

                                    if (ibusMSSPersonAccountGHDVHsa.IsNotNull() && icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP && icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_amount >= 0 && icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date != DateTime.MinValue)
                                        ibusMSSPersonAccountGHDVHsa.InsertIntoEnrollmentDataForHSA();

                                    icdoMSSGDHV.target_person_account_ghdv_id = ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id;
                                if (icdoMSSGDHV.wss_person_account_ghdv_id == 0)
                                    icdoMSSGDHV.Insert();
                                else
                                    icdoMSSGDHV.Update();

                                LoadPersonEmploymentDetail(); // PIR 6367
                                ibusPersonEmploymentDetail.LoadPersonEmployment();
                                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                                // PIR 9115
                                if (istrIsPIR9115Enabled == busConstant.Flag_Yes)
                                {
                                    busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                                                    icdoWssPersonAccountEnrollmentRequest.plan_id, iobjPassInfo);
                                }
                                busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                                    busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);
                            }
                        }
                    }

                    if (ibusMSSPersonAccountGHDV.ibusSoftErrors.IsNotNull() && ibusMSSPersonAccountGHDV.ibusSoftErrors.iclbError.IsNotNull())
                    {
                        int lintSoftErrCount = ibusMSSPersonAccountGHDV.ibusSoftErrors.iclbError.Where(o => o.severity_value == busConstant.MessageSeverityInformation).Count();
                        if (ibusMSSPersonAccountGHDV.ibusSoftErrors.iclbError.Count > lintSoftErrCount)
                        {
                            iblnIsMemberAccountInReviewStatus = true;
                            lblnIsErrorFound = true;
                            foreach (busError lobjErr in ibusMSSPersonAccountGHDV.ibusSoftErrors.iclbError)
                            {
                                //PIR 17842 - Tester's finding - On post click internally, nothing happens, neither the request is posted nor errors are shown on screen.
                                utlError lerror = new utlError() { istrErrorID = Convert.ToString(lobjErr.message_id), istrErrorMessage = lobjErr.display_message };
                                larrList.Add(lerror);
                            }
                            return larrList;
                        }
                        else
                        {
                            icdoWssPersonAccountEnrollmentRequest.target_person_account_id = ibusMSSPersonAccountGHDV.icdoPersonAccount.person_account_id;
                            icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                            icdoWssPersonAccountEnrollmentRequest.Update();
                        }
                    }
                }
            }
            else
            {
                // Waive Plan
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueWaived;
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.is_waiver_report_generated = busConstant.Flag_No; // PIR 11684
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();

                //PIR 17081
                InsertIntoEnrollmentData();

                icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                icdoWssPersonAccountEnrollmentRequest.Update();

                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();

                // PIR 9115
                if (istrIsPIR9115Enabled == busConstant.Flag_No)
                    {
                        string lstrPrioityValue = string.Empty;
                        busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(2, iobjPassInfo, ref lstrPrioityValue), ibusPerson.icdoPerson.FullName, ibusPlan.icdoPlan.mss_plan_name),
                                lstrPrioityValue, aintPlanID: icdoWssPersonAccountEnrollmentRequest.plan_id,
                                aintOrgID: ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id, astrContactRoleValue: busConstant.OrgContactRoleAuthorizedAgent);
                    }
                    else
                {
                    busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id, icdoWssPersonAccountEnrollmentRequest.plan_id, iobjPassInfo);
                }
                busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                 busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);
            }
            LoadDependentsForViewRequest();
            larrList.Add(this);
            EvaluateInitialLoadRules();
                return larrList;
            }
            else // cobra/retiree enrollment posting
            {
                larrList = IsChangeEffectiveDateEntered(larrList);
                if (larrList.Count > 0) return larrList;
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMedicarePartD) // Retiree Medicare Part D posting - No COBRA possible with MEDICARE
                {
                    if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll)
                    {
                        busPersonAccountMedicarePartDHistory lobjPersonAccountMedicarePartDHistory = SetUpMedicareObject();
                        utlPageMode lutlPageMode = (lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.person_account_medicare_part_d_history_id == 0) ? utlPageMode.New : utlPageMode.Update;
                        lobjPersonAccountMedicarePartDHistory.BeforeValidate(lutlPageMode);
                        lobjPersonAccountMedicarePartDHistory.ValidateHardErrors(lutlPageMode);
                        if (lobjPersonAccountMedicarePartDHistory.iarrErrors.Count > 0)
                        {
                            foreach (utlError lobjErr in lobjPersonAccountMedicarePartDHistory.iarrErrors)
                            {
                                utlError lerror = new utlError
                                {
                                    istrErrorID = lobjErr.istrErrorID,
                                    istrErrorMessage = lobjErr.istrErrorMessage
                                };
                                larrList.Add(lerror);
                                return larrList;
                            }
                        }
                        else
                        {
                            lobjPersonAccountMedicarePartDHistory.BeforePersistChanges();
                            lobjPersonAccountMedicarePartDHistory.PersistChanges();
                            ArrayList larlstErrors = lobjPersonAccountMedicarePartDHistory.CreateOrUpdateAchDetail(icdoWssPersonAccountEnrollmentRequest.wss_ben_app_id);
                            if (larlstErrors.Count > 0)
                            {
                                foreach (utlError lobjErr in larlstErrors)
                                {
                                    while (lobjErr.istrErrorMessage.Contains(lobjErr.istrErrorID))
                                        lobjErr.istrErrorMessage = lobjErr.istrErrorMessage.Trim().Substring(lobjErr.istrErrorID.Length);
                                    larrList.Add(lobjErr);
                                    return larrList;
                                }
                            }
                            else
                            {
                                lobjPersonAccountMedicarePartDHistory.ValidateSoftErrors();
                                lobjPersonAccountMedicarePartDHistory.UpdateValidateStatus();
                                lobjPersonAccountMedicarePartDHistory.AfterPersistChanges();
                                UpdateRequestStatusToProcessed(lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.person_account_id);
                                larrList.Add(this);
                                EvaluateInitialLoadRules();
                                return larrList;
                            }
                        }
                    }
                }
                else if (((icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth) ||
                        (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental) ||
                        (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)))
                {
                    if (ibusMSSPersonAccountGHDV.IsNull()) LoadPersonAccountGHDV();
                    if (icdoWssPersonAccountEnrollmentRequest.mss_retiree_flag != busConstant.Flag_Yes &&
                        (ibusPersonAccount.icdoPersonAccount.person_account_id == 0 ||
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id == 0))
                    {
                        utlError lerr = AddError(0, "There Is No Plan Account For COBRA Enrollment.");
                        larrList.Add(lerr);
                        return larrList;
                    }
                    if (icdoWssPersonAccountEnrollmentRequest.mss_retiree_flag != busConstant.Flag_Yes &&
                        string.IsNullOrEmpty(cobra_type_value))
                    {
                        utlError lerr = AddError(6637, string.Empty);
                        larrList.Add(lerr);
                        return larrList;
                    }
                    if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll)
                    {
                        SetUpGHDVForCobraOrRetireeEnrlment();
                        utlPageMode lutlPageMode = ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id > 0 ? utlPageMode.Update : utlPageMode.New;
                        ibusMSSPersonAccountGHDV.BeforeValidate(lutlPageMode);
                        ibusMSSPersonAccountGHDV.iblnIsActiveMember = false;
                        ibusMSSPersonAccountGHDV.iblnIsCobraMember = (icdoWssPersonAccountEnrollmentRequest.mss_retiree_flag != busConstant.Flag_Yes) ? true : false;
                        ibusMSSPersonAccountGHDV.ValidateHardErrors(utlPageMode.New);
                        if (ibusMSSPersonAccountGHDV.iarrErrors.Count > 0)
                        {
                            foreach (utlError lobjErr in ibusMSSPersonAccountGHDV.iarrErrors)
                            {
                                utlError lerror = new utlError
                                {
                                    istrErrorID = lobjErr.istrErrorID,
                                    istrErrorMessage = lobjErr.istrErrorMessage
                                };
                                larrList.Add(lerror);
                                return larrList;
                            }
                        }
                        else
                        {
                            ArrayList larrReturnErrorList = new ArrayList();
                            ibusMSSPersonAccountGHDV.BeforePersistChanges();
                            ibusMSSPersonAccountGHDV.PersistChanges();
                            //PIR 18493 App Wizard - Moved CreateACHDetail Method before softerror so that it cannot be reviewed status. 
                            larrReturnErrorList = ibusMSSPersonAccountGHDV.CreateOrUpdateAchDetail(icdoWssPersonAccountEnrollmentRequest.wss_ben_app_id);
                            if (larrReturnErrorList.Count > 0)
                            {
                                foreach (utlError lobjError in larrReturnErrorList)
                                {
                                    lblnIsErrorFound = true;                                    
                                    while(lobjError.istrErrorMessage.Contains(lobjError.istrErrorID))
                                        lobjError.istrErrorMessage = lobjError.istrErrorMessage.Trim().Substring(lobjError.istrErrorID.Length);
                                    larrList.Add(lobjError);
                                }
                                return larrList;
                            }
                            ibusMSSPersonAccountGHDV.ValidateSoftErrors();
                            ibusMSSPersonAccountGHDV.UpdateValidateStatus();
                            ibusMSSPersonAccountGHDV.icdoPersonAccount.status_value = busConstant.StatusValid;
                            ibusMSSPersonAccountGHDV.AfterPersistChanges();
                            // Manage Dependents                            
                            larrReturnErrorList = InsertDepedents(iclbMSSPersonDependent);
                            if (larrReturnErrorList.Count > 0)
                            {
                                foreach (utlError lobjError in larrReturnErrorList)
                                {
                                    lblnIsErrorFound = true;
                                    larrList.Add(lobjError);
                                }
                                return larrList;
                            }                            
                            // Workers compensation
                            larrReturnErrorList = InsertWorkersCompensation();
                            if (larrReturnErrorList.Count > 0)
                            {
                                foreach (utlError lobjError in larrReturnErrorList)
                                {
                                    lblnIsErrorFound = true;
                                    larrList.Add(lobjError);
                                }
                                return larrList;
                            }
                            // Other coverage details
                            larrReturnErrorList = InsertOtherCoverageDetails();
                            if (larrReturnErrorList.Count > 0)
                            {
                                foreach (utlError lobjError in larrReturnErrorList)
                                {
                                    larrList.Add(lobjError);
                                }
                                return larrList;
                            }
                            UpdateRequestStatusToProcessed(ibusMSSPersonAccountGHDV.icdoPersonAccount.person_account_id);
                            larrList.Add(this);
                            EvaluateInitialLoadRules();
                            return larrList;
                        }
                    }
                }
            }
            return larrList;
        }

        private busPersonAccountGhdv SetUpGHDVForCobraOrRetireeEnrlment()
        {
            if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
            {
                ibusMSSPersonAccountGHDV.icdoPersonAccount.start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
                ibusMSSPersonAccountGHDV.icdoPersonAccount.ienuObjectState = ObjectState.Insert;
                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.ienuObjectState = ObjectState.Insert;
                ibusMSSPersonAccountGHDV.icdoPersonAccount.person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
                ibusMSSPersonAccountGHDV.icdoPersonAccount.plan_id = icdoWssPersonAccountEnrollmentRequest.plan_id;
            }
            else
            {
                ibusMSSPersonAccountGHDV.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.ienuObjectState = ObjectState.Update;
            }
            if (icdoWssPersonAccountEnrollmentRequest.mss_retiree_flag == busConstant.Flag_Yes)
            {
                switch (icdoWssPersonAccountEnrollmentRequest.plan_id)
                {
                    case busConstant.PlanIdGroupHealth:
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value = busConstant.HealthInsuranceTypeRetiree;
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.alternate_structure_code_value = string.Empty;
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date = DateTime.MinValue;
                        break;
                    case busConstant.PlanIdDental:
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeRetiree;
                        break;
                    case busConstant.PlanIdVision:
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeRetiree;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (icdoWssPersonAccountEnrollmentRequest.plan_id)
                {
                    case busConstant.PlanIdGroupHealth:
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.alternate_structure_code_value = string.Empty;
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date = DateTime.MinValue;
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = cobra_type_value;
                        break;
                    case busConstant.PlanIdDental:
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeCOBRA;
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = cobra_type_value;
                        break;
                    case busConstant.PlanIdVision:
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeCOBRA;
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = cobra_type_value;
                        break;
                    default:
                        break;
                }
            }
            ibusMSSPersonAccountGHDV.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
            ibusMSSPersonAccountGHDV.icdoPersonAccount.history_change_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            ibusMSSPersonAccountGHDV.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;

            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth)
                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = string.IsNullOrEmpty(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code) ? icdoMSSGDHV.coverage_code: ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code;
            else
                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = string.IsNullOrEmpty(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value) ? icdoMSSGDHV.level_of_coverage_value: ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value;

            ibusMSSPersonAccountGHDV.icdoPersonAccount.person_employment_dtl_id = 0;
            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.reason_value = icdoWssPersonAccountEnrollmentRequest.reason_value;
            ibusMSSPersonAccountGHDV.ibusPerson = ibusPerson;
            ibusMSSPersonAccountGHDV.ibusPerson.iintEnrollmentRequest = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;//PIR 10697
            ibusMSSPersonAccountGHDV.ibusPlan = ibusPlan;
            ibusMSSPersonAccountGHDV.LoadPaymentElection();
            ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag = busConstant.Flag_Yes;
            ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value = busConstant.IBSModeOfPaymentACH;
            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.keeping_other_coverage_flag = icdoMSSGDHV.keeping_other_coverage_flag;
            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_claim_no = icdoMSSGDHV.medicare_claim_no;
            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_a_effective_date = icdoMSSGDHV.medicare_part_a_effective_date;
            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_b_effective_date = icdoMSSGDHV.medicare_part_b_effective_date;
            ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.premium_conversion_indicator_flag = icdoMSSGDHV.pre_tax_payroll_deduction_flag ?? busConstant.Flag_No;
            ibusMSSPersonAccountGHDV.LoadActiveProviderOrgPlan(icdoWssPersonAccountEnrollmentRequest.change_effective_date);
            ibusMSSPersonAccountGHDV.iblnIsFromInternal = true; //this flag was already being used to insert payment election and history, so made true so that election and history are inserted/updated.
            return ibusMSSPersonAccountGHDV;
        }

        private busPersonAccountMedicarePartDHistory SetUpMedicareObject()
        {
            busPersonAccountMedicarePartDHistory lobjPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory();
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccount = new cdoPersonAccount();
            if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                if (lobjPersonAccountMedicarePartDHistory.FindMedicareByPersonAccountIDAndEffectiveDate(ibusPersonAccount.icdoPersonAccount.person_account_id, icdoWssPersonAccountEnrollmentRequest.change_effective_date))
                {
                    lobjPersonAccountMedicarePartDHistory.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
                    lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.ienuObjectState = ObjectState.Update;
                }
            }
            if (lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.person_account_medicare_part_d_history_id == 0)
            {
                lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.ienuObjectState = ObjectState.Insert;
                lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
                lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
                lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.plan_id = busConstant.PlanIdMedicarePartD;
                lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.initial_enroll_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            }
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.history_change_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.member_person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
            lobjPersonAccountMedicarePartDHistory.iintPersonMemberID = icdoWssPersonAccountEnrollmentRequest.person_id;
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.reason_value = icdoWssPersonAccountEnrollmentRequest.reason_value;
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.send_after = DateTime.Now.Date;
            if (icdoMSSGDHV.IsNotNull() && icdoMSSGDHV.wss_person_account_ghdv_id > 0)
            {
                lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.medicare_claim_no = icdoMSSGDHV.medicare_claim_no;
                lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.medicare_part_a_effective_date = icdoMSSGDHV.medicare_part_a_effective_date;
                lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.medicare_part_b_effective_date = icdoMSSGDHV.medicare_part_b_effective_date;
            }
            lobjPersonAccountMedicarePartDHistory.LoadPerson();
            lobjPersonAccountMedicarePartDHistory.LoadPlan();
            lobjPersonAccountMedicarePartDHistory.LoadMedicarePartDMembers();
            lobjPersonAccountMedicarePartDHistory.LoadHistory();
            lobjPersonAccountMedicarePartDHistory.LoadPaymentElection();
            lobjPersonAccountMedicarePartDHistory.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag = busConstant.Flag_Yes;
            lobjPersonAccountMedicarePartDHistory.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            lobjPersonAccountMedicarePartDHistory.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value = busConstant.IBSModeOfPaymentACH;
            lobjPersonAccountMedicarePartDHistory.LoadActiveProviderOrgPlan(icdoWssPersonAccountEnrollmentRequest.change_effective_date);
            return lobjPersonAccountMedicarePartDHistory;
        }

        private void UpdateRequestStatusToProcessed(int aintPersonAccountId)
        {
            icdoWssPersonAccountEnrollmentRequest.target_person_account_id = aintPersonAccountId;
            icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.StatusProcessed;
            icdoWssPersonAccountEnrollmentRequest.Update();
        }
        private ArrayList InsertWorkersCompensation()
        {
            ArrayList larrList = new ArrayList();
            LoadMSSWorkersCompensation();
            foreach (busWssPersonAccountWorkerCompensation lobjWorkersCompensation in iclbMSSWorkerCompensation)
            {
                busPersonAccountWorkerCompensation lobjNewWorkersCompensation = new busPersonAccountWorkerCompensation
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountWorkerCompensation = new cdoPersonAccountWorkerCompensation()
                };
                lobjNewWorkersCompensation.icdoPersonAccount = ibusMSSPersonAccountGHDV.icdoPersonAccount;
                lobjNewWorkersCompensation.icdoPersonAccountWorkerCompensation.person_account_id = ibusMSSPersonAccountGHDV.icdoPersonAccount.person_account_id;
                lobjNewWorkersCompensation.icdoPersonAccountWorkerCompensation.injury_date = lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.injury_date;
                lobjNewWorkersCompensation.icdoPersonAccountWorkerCompensation.person_name = lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.person_name;
                lobjNewWorkersCompensation.icdoPersonAccountWorkerCompensation.type_of_injury = lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.type_of_injury;
                lobjNewWorkersCompensation.icdoPersonAccountWorkerCompensation.provider_org_name = lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.company_name;
                lobjNewWorkersCompensation.icdoPersonAccountWorkerCompensation.provider_org_id = lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.provider_org_id;
                lobjNewWorkersCompensation.icdoPersonAccountWorkerCompensation.ienuObjectState = ObjectState.Insert;
                lobjNewWorkersCompensation.BeforeValidate(utlPageMode.New);
                lobjNewWorkersCompensation.ValidateHardErrors(utlPageMode.New);
                if (lobjNewWorkersCompensation.iarrErrors.Count > 0)
                {
                    foreach (utlError lobjError in lobjNewWorkersCompensation.iarrErrors)
                    {
                        larrList.Add(lobjError);
                    }
                    break;
                }
                lobjNewWorkersCompensation.PersistChanges();
                lobjNewWorkersCompensation.AfterPersistChanges();
                lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.target_account_worker_comp_id = lobjNewWorkersCompensation.icdoPersonAccountWorkerCompensation.account_worker_comp_id;
                if (lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.wss_person_account_worker_compensation_id > 0)
                    lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.Update();
                else
                {
                    lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                    lobjWorkersCompensation.icdoWssPersonAccountWorkerCompensation.Insert();
                }
            }
            return larrList;
        }

        private ArrayList InsertOtherCoverageDetails()
        {
            ArrayList larrList = new ArrayList();
            LoadMSSOtherCoverageDetails();
            foreach (busWssPersonAccountOtherCoverageDetail lobjPersonAccountOtherCoverage in iclbMSSOtherCoverageDetail)
            {
                busPersonAccountOtherCoverageDetail lobjOtherCoverage = new busPersonAccountOtherCoverageDetail
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountOtherCoverageDetail = new cdoPersonAccountOtherCoverageDetail()
                };
                lobjOtherCoverage.icdoPersonAccount = ibusMSSPersonAccountGHDV.icdoPersonAccount;
                lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.person_account_id = ibusMSSPersonAccountGHDV.icdoPersonAccount.person_account_id;
                lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.covered_person = lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.covered_person;
                lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.date_of_birth = lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.date_of_birth;
                lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.policy_end_date = lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.policy_end_date;
                lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.policy_holder = lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.policy_holder;
                lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.policy_number = lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.policy_number;
                lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.policy_start_date = lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.policy_start_date;
                //pir 6341 start
                lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.provider_org_name = lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.provider_org_name;
                lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.provider_org_id = lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.provider_org_id;
                //end
                lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.ienuObjectState = ObjectState.Insert;
                lobjOtherCoverage.BeforeValidate(utlPageMode.New);
                lobjOtherCoverage.ValidateHardErrors(utlPageMode.New);
                if (lobjOtherCoverage.iarrErrors.Count > 0)
                {
                    foreach (utlError lobjError in lobjOtherCoverage.iarrErrors)
                    {
                        larrList.Add(lobjError);
                    }
                    break;
                }
                lobjOtherCoverage.PersistChanges();
                lobjOtherCoverage.AfterPersistChanges();
                lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.target_account_other_coverage_detail_id = lobjOtherCoverage.icdoPersonAccountOtherCoverageDetail.account_other_coverage_detail_id;
                if (lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.wss_person_account_other_coverage_detail_id > 0)
                    lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.Update();
                else
                {
                    lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.wss_person_account_enrollment_request_id = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;
                    lobjPersonAccountOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.Insert();
                }
            }
            return larrList;
        }

        public void LoadCoverageCodeDescription()
        {
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth
                || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMedicarePartD)
            {
                if (!string.IsNullOrEmpty(icdoMSSGDHV.coverage_code))
                {
                    Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> lclbCoverageRef = LoadMssCoverageCode();

                    var lenumCoverageCodeList = lclbCoverageRef.Where(lobjCoverageRef => lobjCoverageRef.coverage_code
                        == icdoMSSGDHV.coverage_code);

                    string lstrCoverageCode = string.Empty;
                    if (lenumCoverageCodeList.Count() > 0)
                    {
                        ibusMSSPersonAccountGHDV.istrCoverageCode = lenumCoverageCodeList.FirstOrDefault().short_description;
                    }
                    else if(icdoWssPersonAccountEnrollmentRequest.wss_ben_app_id > 0)
                    {
                        string astrDesc = icdoMSSGDHV.coverage_code == busConstant.CoverageCodeSingleActive ? busConstant.CoverageSingle : busConstant.CoverageFamily;
                        ibusMSSPersonAccountGHDV.istrCoverageCode = lclbCoverageRef.Where(lobjCoverageRef => lobjCoverageRef.short_description.Length > 0 &&
                                                                    lobjCoverageRef.short_description.Contains(astrDesc))
                                                                    .Select(lobjCoverageRef => lobjCoverageRef.short_description).FirstOrDefault();
                    }
                }
            }
        }
        //19997
        private busPersonAccountGhdvHsa _ibusPersonAccountGhdvHsa;
        public busPersonAccountGhdvHsa ibusPersonAccountGhdvHsa
        {
            get { return _ibusPersonAccountGhdvHsa; }
            set { _ibusPersonAccountGhdvHsa = value; }
        }
        private busPersonAccountGhdv CreateGHDVPersonAccount()
        {
            busPersonAccountGhdv lobjPersonAccountGHDV = new busPersonAccountGhdv
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountGhdv = new cdoPersonAccountGhdv()
            };
            lobjPersonAccountGHDV.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
            lobjPersonAccountGHDV.icdoPersonAccount.start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            lobjPersonAccountGHDV.icdoPersonAccount.person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
            lobjPersonAccountGHDV.icdoPersonAccount.plan_id = icdoWssPersonAccountEnrollmentRequest.plan_id;
            lobjPersonAccountGHDV.icdoPersonAccountGhdv.reason_value = icdoWssPersonAccountEnrollmentRequest.reason_value;
            lobjPersonAccountGHDV.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
            lobjPersonAccountGHDV.ibusPerson = ibusPerson;
            lobjPersonAccountGHDV.ibusPerson.iintEnrollmentRequest = icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;//PIR 10697
            lobjPersonAccountGHDV.ibusPlan = ibusPlan;
            lobjPersonAccountGHDV.ibusPaymentElection = new busPersonAccountPaymentElection { icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection() };
            //pir 7790 start
            lobjPersonAccountGHDV.icdoPersonAccountGhdv.keeping_other_coverage_flag = icdoMSSGDHV.keeping_other_coverage_flag;
            lobjPersonAccountGHDV.icdoPersonAccountGhdv.medicare_claim_no = icdoMSSGDHV.medicare_claim_no;
            lobjPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_a_effective_date = icdoMSSGDHV.medicare_part_a_effective_date;
            lobjPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_b_effective_date = icdoMSSGDHV.medicare_part_b_effective_date;
            //end
            lobjPersonAccountGHDV.icdoPersonAccountGhdv.premium_conversion_indicator_flag = icdoMSSGDHV.pre_tax_payroll_deduction_flag ?? busConstant.Flag_No;
            lobjPersonAccountGHDV.icdoPersonAccountGhdv.employment_type_value = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
            lobjPersonAccountGHDV.icdoPersonAccount.ienuObjectState = ObjectState.Insert;
            lobjPersonAccountGHDV.icdoPersonAccountGhdv.ienuObjectState = ObjectState.Insert;
            lobjPersonAccountGHDV.icdoPersonAccount.status_value = busConstant.StatusValid;
            lobjPersonAccountGHDV.icdoPersonAccount.is_from_mss = true;
            if (icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP)
            {
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.alternate_structure_code_value = busConstant.AlternateStructureCodeHDHP;
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            }
            LoadGHDVInsuranceTypeValue(lobjPersonAccountGHDV);

            lobjPersonAccountGHDV.BeforeValidate(utlPageMode.New);
            //reload the person employment detail 
            //in b4 validate the system changes the employment detail id
            LoadGHDVInsuranceTypeValue(lobjPersonAccountGHDV);
            //this is temporary fix. need to change for COBRA
            lobjPersonAccountGHDV.iblnIsActiveMember = true;
            ibusMSSPersonAccountGHDV.iblnIsCobraMember = false;
            lobjPersonAccountGHDV.ValidateHardErrors(utlPageMode.New);
            if (lobjPersonAccountGHDV.iarrErrors.Count > 0)
                return lobjPersonAccountGHDV;
            else
            {
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                lobjPersonAccountGHDV.BeforePersistChanges();
                lobjPersonAccountGHDV.PersistChanges();
                lobjPersonAccountGHDV.ValidateSoftErrors();
                lobjPersonAccountGHDV.UpdateValidateStatus();
                //this is temporary fix. need to change for COBRA
                lobjPersonAccountGHDV.iblnIsActiveMember = true;
                //to insert history
                lobjPersonAccountGHDV.icdoPersonAccount.status_value = busConstant.StatusValid;

                lobjPersonAccountGHDV.iblnIsFromMSSForEnrollmentData = true;//PIR 20017
                lobjPersonAccountGHDV.AfterPersistChanges();

                lobjPersonAccountGHDV.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
                lobjPersonAccountGHDV.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(ibusPersonAccountEmploymentDetail);

            }
            return lobjPersonAccountGHDV;
        }

        private void LoadGHDVInsuranceTypeValue(busPersonAccountGhdv lobjPersonAccountGHDV)
        {
            LoadPersonEmploymentDetail();
            lobjPersonAccountGHDV.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
            lobjPersonAccountGHDV.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
            lobjPersonAccountGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment = ibusPersonEmploymentDetail.ibusPersonEmployment;
            lobjPersonAccountGHDV.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
            lobjPersonAccountGHDV.LoadOrgPlan(lobjPersonAccountGHDV.idtPlanEffectiveDate);
            lobjPersonAccountGHDV.ibusOrgPlan.ibusOrganization = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
            lobjPersonAccountGHDV.LoadProviderOrgPlan(lobjPersonAccountGHDV.idtPlanEffectiveDate);
            //org plan are reloaded in before validate.
            if (lobjPersonAccountGHDV.IsHealthOrMedicare)
            {
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.plan_option_value = lobjPersonAccountGHDV.ibusOrgPlan.icdoOrgPlan.plan_option_value;
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.health_insurance_type_value = lobjPersonAccountGHDV.ibusOrgPlan.HealthInsuranceType;
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code = icdoMSSGDHV.coverage_code;
            }
            else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental)
            {
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeActive;
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = icdoMSSGDHV.level_of_coverage_value;
            }
            else if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
            {
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeActive;
                lobjPersonAccountGHDV.icdoPersonAccountGhdv.level_of_coverage_value = icdoMSSGDHV.level_of_coverage_value;
            }
        }

        public void LoadMSSLifeOptions()
        {
            if (ibusMSSLifeOption.IsNull())
                ibusMSSLifeOption = new busWssPersonAccountLifeOption
                {
                    icdoWssPersonAccountLifeOption = new cdoWssPersonAccountLifeOption()
                };

            DataTable ldtbList = Select<cdoWssPersonAccountLifeOption>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);

            if (ldtbList.Rows.Count > 0)
                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.LoadData(ldtbList.Rows[0]);
        }
        //reload supplemental amount as in FindWizardCompletion the system updates the amount from database  
        public void LoadSupplementalAmount()
        {
            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount += ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
        }
        //visibility of link       
        public void SetVisibilityofLink()
        {
            iblnIsSFN58859Visible = false;

            if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                //load old coverage
                if (ibusMSSPersonAccountLife.IsNull())
                    LoadMSSPersonAccountLife();

                if (ibusMSSPersonAccountLife.iclbLifeOption.IsNull())
                    ibusMSSPersonAccountLife.LoadLifeOptionData();

                foreach (busPersonAccountLifeOption lobjOption in ibusMSSPersonAccountLife.iclbLifeOption)
                {
                    if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                    {
                        if (lobjOption.icdoPersonAccountLifeOption.coverage_amount != 0.00M)
                        {
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount > lobjOption.icdoPersonAccountLifeOption.coverage_amount)
                            {
                                iblnIsSFN58859Visible = true;
                                break;
                            }
                        }
                    }
                    else if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                    {
                        if (lobjOption.icdoPersonAccountLifeOption.coverage_amount != 0.00M)
                        {
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount > lobjOption.icdoPersonAccountLifeOption.coverage_amount)
                            {
                                iblnIsSFN58859Visible = true;
                                break;
                            }
                        }
                    }
                    else if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                    {
                        if (lobjOption.icdoPersonAccountLifeOption.coverage_amount != 0.00M)
                        {
                            if (!String.IsNullOrEmpty(ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value))
                            {
                                if ((Convert.ToDecimal(ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value))
                                    > lobjOption.icdoPersonAccountLifeOption.coverage_amount)
                                {
                                    iblnIsSFN58859Visible = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount > 50000M)
                    iblnIsSFN58859Visible = true;
                //foreach (busPersonAccountLifeOption lobjOption in ibusMSSPersonAccountLife.iclbLifeOption)
                //{
                //    if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                //    {
                //        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount > 50000M)
                //        {
                //            iblnIsSFN58859Visible = true;
                //            break;
                //        }
                //    }
                //}
            }
        }

        public void LoadMSSPersonAccountLife()
        {
            if (ibusMSSPersonAccountLife.IsNull())
            {
                ibusMSSPersonAccountLife = new busPersonAccountLife
                {
                    icdoPersonAccount = new cdoPersonAccount(),
                    icdoPersonAccountLife = new cdoPersonAccountLife()
                };
            }
            ibusMSSPersonAccountLife.FindPersonAccount(ibusPersonAccount.icdoPersonAccount.person_account_id);
            ibusMSSPersonAccountLife.FindPersonAccountLife(ibusPersonAccount.icdoPersonAccount.person_account_id);

        }

        //for Life Posting
        public ArrayList btnPostLife_Click()
        {
            iblnIsEOIflag = iblnIsEOIRequired;
            iblnIsEOIRequired = false;
            ArrayList larrlist = new ArrayList();

            //PIR 10237 --  User should not allow to modify their own record nor post MSS enrollment requests that is their own submission
            DataTable dtUserInfo = iobjPassInfo.isrvDBCache.GetUserInfo(iobjPassInfo.istrUserID);


            if (dtUserInfo?.Rows.Count > 0 && !String.IsNullOrEmpty(Convert.ToString(dtUserInfo.Rows[0]["Person_ID"])) && icdoWssPersonAccountEnrollmentRequest.person_id == Convert.ToInt32(dtUserInfo.Rows[0]["Person_ID"]))
            {
                utlError lerror = new utlError();
                lerror.istrErrorID = "10275";
                lerror.istrErrorMessage = "Request can not be posted by the same user who created the request.";
                larrlist.Add(lerror);
                return larrlist;

            }

            if (icdoWssPersonAccountEnrollmentRequest.change_effective_date == DateTime.MinValue)
            {
                utlError lerror = new utlError();
                lerror.istrErrorID = "10014";
                lerror.istrErrorMessage = "Change Effective Date is required.";
                larrlist.Add(lerror);
                return larrlist;
            }
            else
            {
                if (ibusPerson == null)
                    LoadPerson();
                if (ibusPlan == null)
                    LoadPlan();
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccountForPosting();
                if (ibusPersonEmploymentDetail.IsNull())
                    LoadPersonEmploymentDetail();
                if (ibusPersonAccountEmploymentDetail.IsNull())
                    LoadPersonAccountEmploymentDetail();

                if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                {
                    ibusMSSPersonAccountLife = CreateLifePersonAccount();
                    if (ibusMSSPersonAccountLife.iarrErrors.Count > 0)
                    {
                        foreach (utlError lobjError in ibusMSSPersonAccountLife.iarrErrors)
                        {
                            larrlist.Add(lobjError);
                            return larrlist;
                        }
                    }
                }
                else
                {
                    LoadMSSPersonAccountLife();
                    ibusMSSPersonAccountLife.ibusPerson = ibusPerson;
                    ibusMSSPersonAccountLife.ibusPlan = ibusPlan;
                    ibusMSSPersonAccountLife.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                    ibusMSSPersonAccountLife.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
                    ibusMSSPersonAccountLife.icdoPersonAccount.history_change_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
                    ibusMSSPersonAccountLife.icdoPersonAccountLife.reason_value = icdoWssPersonAccountEnrollmentRequest.reason_value; // PROD PIR ID 6368
                    ibusMSSPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value = busConstant.LifeInsuranceTypeActiveMember; // PIR 9702
                    ibusMSSPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction;
                    // PIR 10087
                    if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                        ibusMSSPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag = busConstant.Flag_No;
                    ibusMSSPersonAccountLife.LoadPlanEffectiveDate();
                    ibusMSSPersonAccountLife.LoadPaymentElection();
                    ibusMSSPersonAccountLife.LoadLifeOptionData();

                    LoadLifeOptionFromRequest(ibusMSSPersonAccountLife, false);
                    ibusMSSPersonAccountLife.LoadMemberAge(ibusMSSPersonAccountLife.idtPlanEffectiveDate);
                    ibusMSSPersonAccountLife.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                    ibusMSSPersonAccountLife.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
                    ibusMSSPersonAccountLife.ibusPersonEmploymentDetail.LoadPersonEmployment();
                    ibusMSSPersonAccountLife.LoadOrgPlan(ibusMSSPersonAccountLife.idtPlanEffectiveDate);
                    ibusMSSPersonAccountLife.LoadProviderOrgPlan(ibusMSSPersonAccountLife.idtPlanEffectiveDate);
                    ibusMSSPersonAccountLife.icdoPersonAccount.ienuObjectState = ObjectState.Update;
                    ibusMSSPersonAccountLife.icdoPersonAccount.is_from_mss = true;

                    // PIR 15544

                    if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll && icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id > 0)
                    {
                        if (ibusMSSPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
                        {
                            ibusMSSPersonAccountLife.UpdateIBSFlagForActive();
                        }
                    }
                    ibusMSSPersonAccountLife.BeforeValidate(utlPageMode.All);
                    ibusMSSPersonAccountLife.ValidateHardErrors(utlPageMode.All);
                    if (ibusMSSPersonAccountLife.iarrErrors.Count > 0)
                    {
                        foreach (utlError lobjErr in ibusMSSPersonAccountLife.iarrErrors)
                            larrlist.Add(lobjErr);
                    }
                    else
                    {
                        ibusMSSPersonAccountLife.BeforePersistChanges();
                        ibusMSSPersonAccountLife.PersistChanges();
                        ibusMSSPersonAccountLife.ValidateSoftErrors();
                        ibusMSSPersonAccountLife.UpdateValidateStatus();
                        ibusMSSPersonAccountLife.icdoPersonAccount.status_value = busConstant.StatusValid;

                        ibusMSSPersonAccountLife.iblnIsFromMSSForEnrollmentData = true;//PIR 20017
                        ibusMSSPersonAccountLife.AfterPersistChanges();
                    }
                }

                // No Hard errors
                if (ibusMSSPersonAccountLife.iarrErrors.Count == 0)
                {
                    ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = ibusMSSPersonAccountLife.icdoPersonAccount.person_account_id;
                    ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                    ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();

                    busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                     busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);

                    if (ibusMSSPersonAccountLife.ibusPaymentElection.IsNull())
                        ibusMSSPersonAccountLife.LoadPaymentElection();

                    //PIR 20017 - Calling InsertEnrollmentData explicity because Person Account ID should be updated into PA Employment Detail before record is inserted into Enrollment_Data. 
                    if (ibusMSSPersonAccountLife.iblnIsHistoryInserted && ibusMSSPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value != busConstant.LifeInsuranceTypeRetireeMember
                        && ibusMSSPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag != busConstant.Flag_Yes )
                        ibusMSSPersonAccountLife.InsertIntoEnrollmentData();
                }
                else
                {
                    iblnIsMemberAccountInReviewStatus = true;
                    return larrlist;
                }

                if (ibusMSSPersonAccountLife.ibusSoftErrors.IsNotNull() && ibusMSSPersonAccountLife.ibusSoftErrors.iclbError.IsNotNull())
                {
                    icdoWssPersonAccountEnrollmentRequest.target_person_account_id = ibusMSSPersonAccountLife.icdoPersonAccount.person_account_id;
                    int lintSoftErrCount = ibusMSSPersonAccountLife.ibusSoftErrors.iclbError.Where(o => o.severity_value == busConstant.MessageSeverityInformation).Count();
                    if (ibusMSSPersonAccountLife.ibusSoftErrors.iclbError.Count > lintSoftErrCount)
                        iblnIsMemberAccountInReviewStatus = true;
                    else
                        icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;// No Soft errors
                    icdoWssPersonAccountEnrollmentRequest.Update();
                }
                larrlist.Add(this);
                EvaluateInitialLoadRules();
            }
            return larrlist;
        }

        private busPersonAccountLife CreateLifePersonAccount()
        {
            busPersonAccountLife lobjPersonAccountLife = new busPersonAccountLife
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountLife = new cdoPersonAccountLife()
            };

            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

            lobjPersonAccountLife.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusInsuranceEnrolled;
            lobjPersonAccountLife.icdoPersonAccount.history_change_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            lobjPersonAccountLife.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
            lobjPersonAccountLife.icdoPersonAccount.person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
            lobjPersonAccountLife.icdoPersonAccount.plan_id = icdoWssPersonAccountEnrollmentRequest.plan_id;
            lobjPersonAccountLife.icdoPersonAccount.is_from_mss = true;
            lobjPersonAccountLife.icdoPersonAccountLife.reason_value = icdoWssPersonAccountEnrollmentRequest.reason_value;
            lobjPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value = busConstant.LifeInsuranceTypeActiveMember;
            lobjPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction;
            // PIR 10087
            if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                lobjPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag = busConstant.Flag_No;
            lobjPersonAccountLife.ibusPerson = ibusPerson;
            lobjPersonAccountLife.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
            lobjPersonAccountLife.ibusPersonEmploymentDetail.ibusPersonEmployment = ibusPersonEmploymentDetail.ibusPersonEmployment;
            lobjPersonAccountLife.ibusPlan = ibusPlan;
            lobjPersonAccountLife.LoadMemberAge(icdoWssPersonAccountEnrollmentRequest.change_effective_date);
            lobjPersonAccountLife.LoadOrgPlan();
            lobjPersonAccountLife.LoadProviderOrgPlan();

            lobjPersonAccountLife.icdoPersonAccountLife.ienuObjectState = ObjectState.Insert;
            lobjPersonAccountLife.icdoPersonAccount.ienuObjectState = ObjectState.Insert;
            lobjPersonAccountLife.icdoPersonAccount.status_value = busConstant.StatusValid;
            lobjPersonAccountLife.ibusPaymentElection = new busPersonAccountPaymentElection
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection()
            };

            LoadLifeOptionFromRequest(lobjPersonAccountLife, true);
            lobjPersonAccountLife.BeforeValidate(utlPageMode.New);
            lobjPersonAccountLife.ValidateHardErrors(utlPageMode.New);
            if (lobjPersonAccountLife.iarrErrors.Count > 0)
            {
                return lobjPersonAccountLife;
            }

            ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
            lobjPersonAccountLife.BeforePersistChanges();
            lobjPersonAccountLife.PersistChanges();
            lobjPersonAccountLife.ValidateSoftErrors();
            lobjPersonAccountLife.UpdateValidateStatus();
            //to insert history
            lobjPersonAccountLife.icdoPersonAccount.status_value = busConstant.StatusValid;

            lobjPersonAccountLife.iblnIsFromMSSForEnrollmentData = true;//PIR 20017
            lobjPersonAccountLife.AfterPersistChanges();

            lobjPersonAccountLife.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
            lobjPersonAccountLife.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(ibusPersonAccountEmploymentDetail);

            return lobjPersonAccountLife;
        }
        // PIR 17538
        private bool IsEvidenceofInsurabilityMandatory(busPersonAccountLife abusPersonAccountLife, bool ablnNewMode)
        {
            idecNewSupplementalAmount = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount;
            idecNewSpouseSupplementalAmount = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount;
            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value.IsNotNullOrEmpty())
                idecNewDependentCoverageOptionValue = Convert.ToDecimal(ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value);
            ////for supplemental
            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag == busConstant.Flag_No)
            {
                if (idecNewSupplementalAmount > 0)
                {
                    if (ablnNewMode)
                    {
                        if ((icdoAutoPostingCrossRef.change_reason_value == busConstant.AnnualEnrollment
                                || icdoAutoPostingCrossRef.change_reason_value == busConstant.ChangeReasonNewHire)
                            && idecNewSupplementalAmount <= idecSuppIncrementLimit)
                        {
                            iblnIsEOI = false;
                        }
                        else iblnIsEOI = true;

                        if (iblnIsEOI)
                        {
                            return iblnIsEOI;
                        }
                    }
                    else
                    {
                        busPersonAccountLifeOption lobjBasicLifeOption = new busPersonAccountLifeOption { icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption() };
                        lobjBasicLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_Basic;
                        busPersonAccountLifeOption lobjSupplementalLifeOption = new busPersonAccountLifeOption { icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption() };
                        lobjSupplementalLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_Supplemental;
                        if (abusPersonAccountLife.iclbPreviousHistory.IsNull()) abusPersonAccountLife.LoadPreviousHistory();
                        busPersonAccountLifeHistory lobjBasicHistory = abusPersonAccountLife.GetPreviousHistoryForOption(lobjBasicLifeOption);
                        busPersonAccountLifeHistory lobjSuppHistory = abusPersonAccountLife.GetPreviousHistoryForOption(lobjSupplementalLifeOption);
                        if ((icdoAutoPostingCrossRef.change_reason_value == busConstant.AnnualEnrollment &&
                        ((idecNewSupplementalAmount - (lobjSuppHistory.icdoPersonAccountLifeHistory.coverage_amount + lobjBasicHistory.icdoPersonAccountLifeHistory.coverage_amount)) <= idecSuppIncrementLimit
                        && idecNewSupplementalAmount <= idecSuppGILimit)))
                        {
                            iblnIsEOI = false;
                        }
                        else if ((icdoAutoPostingCrossRef.change_reason_value == busConstant.ChangeReasonNewHire)
                            && idecNewSupplementalAmount <= idecSuppGILimit) 
                        {
                            iblnIsEOI = false;
                        }
                        else iblnIsEOI = true;
                        if (iblnIsEOI)
                        {
                            if (lobjSuppHistory.icdoPersonAccountLifeHistory.coverage_amount > 0)
                            {
                                if (idecNewSupplementalAmount <=
                                   (lobjSuppHistory.icdoPersonAccountLifeHistory.coverage_amount + lobjBasicHistory.icdoPersonAccountLifeHistory.coverage_amount)) //PIR 19210 - As per Maik's call
                                {
                                    iblnIsEOI = false;
                                }
                            }
                        }
                        //PIR 23726 – EOI logic change in Life wizard 
                        if (icdoAutoPostingCrossRef?.change_reason_value == busConstant.AnnualEnrollment &&
                            lobjSuppHistory?.icdoPersonAccountLifeHistory?.coverage_amount == 0 &&
                            idecNewSupplementalAmount > 0)
                        {
                            iblnIsEOI = true;
                        }
						
                        if (iblnIsEOI)
                            return iblnIsEOI;
                    }
                }
            }
            ////for Dependent
            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.Flag_No)
            {
                if (idecNewDependentCoverageOptionValue > 0)
                {
                    //PIR 19210 - For PERSLink, including MSS, for the 2019 plan year, the requirement for EOI (validations and warnings) 
                    //should be removed for requests for dependent life coverage (both new coverage and increases in current level of coverage).  
                    //This should be updated for the annual enrollment in fall 2018 for the 2019 plan year.
                    iblnIsEOI = false;
                    //if (ablnNewMode)
                    //{
                    //    if (icdoAutoPostingCrossRef.change_reason_value == busConstant.ChangeReasonNewHire)
                    //    {
                    //        iblnIsEOI = false;
                    //    }
                    //    else
                    //        iblnIsEOI = true;
                    //    if (iblnIsEOI)
                    //        return iblnIsEOI;
                    //}
                    //else
                    //{
                    //    busPersonAccountLifeOption lobjDepedentLifeOption = new busPersonAccountLifeOption { icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption() };
                    //    lobjDepedentLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_DependentSupplemental;
                    //    if (abusPersonAccountLife.iclbPreviousHistory.IsNull()) abusPersonAccountLife.LoadPreviousHistory();
                    //    busPersonAccountLifeHistory lobjPrevDepSpouseHistory = abusPersonAccountLife.GetPreviousHistoryForOption(lobjDepedentLifeOption);
                    //    if (icdoAutoPostingCrossRef.change_reason_value == busConstant.ChangeReasonNewHire)
                    //    {
                    //        iblnIsEOI = false;
                    //    }
                    //    else if (lobjPrevDepSpouseHistory.icdoPersonAccountLifeHistory.coverage_amount > 0
                    //            && (idecNewDependentCoverageOptionValue <= lobjPrevDepSpouseHistory.icdoPersonAccountLifeHistory.coverage_amount))
                    //    {
                    //        iblnIsEOI = false;
                    //    }
                    //    else
                    //    {
                    //        iblnIsEOI = true;
                    //    }
                    //    if (iblnIsEOI)
                    //        return iblnIsEOI;
                    //}
                }
            }
            ////for Spouse Supplemental
            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_No)
            {
                if (idecNewSpouseSupplementalAmount > 0)
                {
                    if (ablnNewMode)
                    {
                        if ((icdoAutoPostingCrossRef.change_reason_value == busConstant.ReasonValueMarriage
                         || icdoAutoPostingCrossRef.change_reason_value == busConstant.ChangeReasonNewHire)
                         && idecNewSpouseSupplementalAmount <= idecSpouseSuppGILimit)
                        {
                            iblnIsEOI = false;
                        }
                        else iblnIsEOI = true;
                        if (iblnIsEOI)
                            return iblnIsEOI;
                    }
                    else
                    {
                        busPersonAccountLifeOption lobjSpouseLifeOption = new busPersonAccountLifeOption { icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption() };
                        lobjSpouseLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_SpouseSupplemental;
                        if (abusPersonAccountLife.iclbPreviousHistory.IsNull()) abusPersonAccountLife.LoadPreviousHistory();
                        busPersonAccountLifeHistory lobjPrevSpouseHistory = abusPersonAccountLife.GetPreviousHistoryForOption(lobjSpouseLifeOption);
                        if ((icdoAutoPostingCrossRef.change_reason_value == busConstant.ReasonValueMarriage
                             || icdoAutoPostingCrossRef.change_reason_value == busConstant.ChangeReasonNewHire)
                             && idecNewSpouseSupplementalAmount <= idecSpouseSuppGILimit) 
                        {
                            iblnIsEOI = false;
                        }
                        else iblnIsEOI = true;
                        if (iblnIsEOI)
                        {
                            if (lobjPrevSpouseHistory.icdoPersonAccountLifeHistory.coverage_amount > 0)
                            {
                                if (idecNewSpouseSupplementalAmount <= lobjPrevSpouseHistory.icdoPersonAccountLifeHistory.coverage_amount)
                                {
                                    iblnIsEOI = false;
                                }
                            }
                        }
                        if (iblnIsEOI)
                            return iblnIsEOI;
                    }
                }
            }
            return iblnIsEOI;
        }


        private void LoadLifeOptionFromRequest(busPersonAccountLife abusPersonAccountLife, bool ablnNewMode)
        {
            if (abusPersonAccountLife.iclbLifeOption.IsNull())
                abusPersonAccountLife.iclbLifeOption = new Collection<busPersonAccountLifeOption>();

            Collection<busPersonAccountLifeOption> lclbFinalLifeOption = new Collection<busPersonAccountLifeOption>();
            Collection<busPersonAccountLifeOption> lclbLifeOption = new Collection<busPersonAccountLifeOption>();
            lclbLifeOption = abusPersonAccountLife.iclbLifeOption;
            //load options
            if (ibusMSSLifeOption.IsNotNull())
            {
                //*********BASIC COVERAGE***********************
                busPersonAccountLifeOption lobjBasicLifeOption = new busPersonAccountLifeOption
                {
                    icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption()
                };

                //get life option by level of coverage
                var lenumLO = lclbLifeOption
                    .Where(lobjLO => lobjLO.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic);
                if (lenumLO.Count() > 0)
                {
                    lobjBasicLifeOption = lenumLO.FirstOrDefault();
                }
                lobjBasicLifeOption.icdoPersonAccountLifeOption.coverage_amount = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                lobjBasicLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_Basic;
                lobjBasicLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
                if (lobjBasicLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                    lobjBasicLifeOption.icdoPersonAccountLifeOption.effective_start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;

                lclbFinalLifeOption.Add(lobjBasicLifeOption);

                //PIR 25712
                GetCoverageAmountDetailsForDisplay();

                //*************SUPPLEMENTAL COVERAGE*******************
                decimal ldecSuppCoverageAmount = 0M;
                busPersonAccountLifeOption lobjSupplementalLifeOption = new busPersonAccountLifeOption
                {
                    icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption()
                };
                //get life option by level of coverage
                var lenumSLO = lclbLifeOption
                    .Where(lobjLO => lobjLO.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental);
                if (lenumSLO.Count() > 0)
                {
                    lobjSupplementalLifeOption = lenumSLO.FirstOrDefault();
                }
                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag != busConstant.Flag_Yes)
                {
                    if (iblnIsLifeAutoPosting)
                    {
                        ldecSuppCoverageAmount = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount;
                        lobjSupplementalLifeOption.icdoPersonAccountLifeOption.coverage_amount =
                                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount -
                                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                    }
                    else
                    {
                        // Posting from Request Maintenance
                        lobjSupplementalLifeOption.icdoPersonAccountLifeOption.coverage_amount = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount;
                        ldecSuppCoverageAmount = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount +
                                                 ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                    }
                    if (ablnNewMode)
                    {
                        if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueMarriage ||
                            icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                        {
                            if (ldecSuppCoverageAmount > idecSuppIncrementLimit && iblnIsEOIflag)//increment_limit
                            {
                                // 7805   No Supplemental coverage is enrolled.  Premium Conversion cannot be enrolled.
                                //abusPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag = busConstant.Flag_No;
                                //lobjSupplementalLifeOption.icdoPersonAccountLifeOption.coverage_amount = 0;
                                iblnIsEOIRequired = true;
                            }
                        }
                        if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonNewHire)// ||
                           // icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                        {
                            if (ldecSuppCoverageAmount > idecSuppGILimit) //PIR 25712 - Changed from 200k to 300k
                            {
                                // abusPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag = busConstant.Flag_No;
                                //lobjSupplementalLifeOption.icdoPersonAccountLifeOption.coverage_amount = 0;
                                iblnIsEOIRequired = true;
                            }
                        }
                    }
                    else
                    {
                        if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueMarriage ||
                            icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonNewHire ||
                            icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                        {
                            if (abusPersonAccountLife.iclbPreviousHistory.IsNull()) abusPersonAccountLife.LoadPreviousHistory();
                            busPersonAccountLifeHistory lobjSuppHistory = abusPersonAccountLife.GetPreviousHistoryForOption(lobjSupplementalLifeOption);
                            busPersonAccountLifeHistory lobjBasicHistory = abusPersonAccountLife.GetPreviousHistoryForOption(lobjBasicLifeOption);
                            if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueMarriage ||
                                icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                            {
                                if (lobjSuppHistory.icdoPersonAccountLifeHistory.coverage_amount > 0)
                                {
                                    if (ldecSuppCoverageAmount >
                                    (lobjSuppHistory.icdoPersonAccountLifeHistory.coverage_amount + lobjBasicHistory.icdoPersonAccountLifeHistory.coverage_amount + idecSuppIncrementLimit) && iblnIsEOIflag)//icrement_limit 25k
                                    {
                                        lobjSupplementalLifeOption.icdoPersonAccountLifeOption.coverage_amount = lobjSuppHistory.icdoPersonAccountLifeHistory.coverage_amount;
                                        iblnIsEOIRequired = true;
                                    }
                                }
                                else
                                {
                                    if (ldecSuppCoverageAmount > idecSuppIncrementLimit && iblnIsEOIflag)//icrement_limit 25000
                                    {
                                        // 7805   No Supplemental coverage is enrolled.  Premium Conversion cannot be enrolled.
                                        //abusPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag = busConstant.Flag_No;
                                        //lobjSupplementalLifeOption.icdoPersonAccountLifeOption.coverage_amount = 0;
                                        iblnIsEOIRequired = true;
                                    }
                                }
                            }
                            if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonNewHire ||
                                icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                            {
                                if (lobjSuppHistory.icdoPersonAccountLifeHistory.coverage_amount == 0 &&
                                    ldecSuppCoverageAmount > idecSuppGILimit) //PIR 25712 - Changed from 200k to 300k
                                {
                                    abusPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag = busConstant.Flag_No;
                                    lobjSupplementalLifeOption.icdoPersonAccountLifeOption.coverage_amount = lobjSuppHistory.icdoPersonAccountLifeHistory.coverage_amount;
                                }
                            }
                        }
                    }
                    lobjSupplementalLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_Supplemental;
                    lobjSupplementalLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
                    if (lobjSupplementalLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                        lobjSupplementalLifeOption.icdoPersonAccountLifeOption.effective_start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
                    lclbFinalLifeOption.Add(lobjSupplementalLifeOption);
                }
                else if (lobjSupplementalLifeOption.icdoPersonAccountLifeOption.person_account_id > 0 &&
                    lobjSupplementalLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                {
                    lobjSupplementalLifeOption.icdoPersonAccountLifeOption.coverage_amount = 0.00M;
                    lobjSupplementalLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_Supplemental;
                    lobjSupplementalLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;
                    lclbFinalLifeOption.Add(lobjSupplementalLifeOption);
                }


                //******************DEPENDENT SUPPLEMENTAL*********************
                busPersonAccountLifeOption lobjDepedentLifeOption = new busPersonAccountLifeOption
                {
                    icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption()
                };
                //get life option by level of coverage
                var lenumDLO = lclbLifeOption
                    .Where(lobjLO => lobjLO.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental);
                if (lenumDLO.Count() > 0)
                {
                    lobjDepedentLifeOption = lenumDLO.FirstOrDefault();
                }
                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.Flag_No)//ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag != busConstant.Flag_Yes
                {
                    lobjDepedentLifeOption.icdoPersonAccountLifeOption.coverage_amount = Convert.ToDecimal(ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value);
                    lobjDepedentLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_DependentSupplemental;
                    lobjDepedentLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
                    if (lobjDepedentLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                        lobjDepedentLifeOption.icdoPersonAccountLifeOption.effective_start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
                    ////PIR-10153 Start
                    if (abusPersonAccountLife.iclbPreviousHistory.IsNull()) abusPersonAccountLife.LoadPreviousHistory();
                    busPersonAccountLifeHistory lobjPrevDepSpouseHistory = abusPersonAccountLife.GetPreviousHistoryForOption(lobjDepedentLifeOption);
                    if ((lobjPrevDepSpouseHistory.icdoPersonAccountLifeHistory.coverage_amount == 0 ||
                        Convert.ToDecimal(ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value) > lobjPrevDepSpouseHistory.icdoPersonAccountLifeHistory.coverage_amount)
                        && this.icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ChangeReasonNewHire)
                    {
                        if (iblnIsEOIflag == false)
                            lobjDepedentLifeOption.icdoPersonAccountLifeOption.coverage_amount = Convert.ToDecimal(ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value);
                        else
                            lobjDepedentLifeOption.icdoPersonAccountLifeOption.coverage_amount = 0.00M;
                        //lobjDepedentLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = string.Empty;
                        if (iblnIsEOIflag)
                            iblnIsEOIRequired = true;
                    }
                    ////PIR-10153 End
                    lclbFinalLifeOption.Add(lobjDepedentLifeOption);
                }
                else if (lobjDepedentLifeOption.icdoPersonAccountLifeOption.person_account_id > 0 &&
                    lobjDepedentLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                {
                    lobjDepedentLifeOption.icdoPersonAccountLifeOption.coverage_amount = 0.00M;
                    lobjDepedentLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_DependentSupplemental;
                    lobjDepedentLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;
                    lclbFinalLifeOption.Add(lobjDepedentLifeOption);
                }


                //********************SPOUSE SUPPLEMENTAL**************************
                busPersonAccountLifeOption lobjSpouseLifeOption = new busPersonAccountLifeOption
                {
                    icdoPersonAccountLifeOption = new cdoPersonAccountLifeOption()
                };
                //get life option by level of coverage
                var lenumSSLO = lclbLifeOption
                    .Where(lobjLO => lobjLO.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental);
                if (lenumSSLO.Count() > 0)
                {
                    lobjSpouseLifeOption = lenumSSLO.FirstOrDefault();
                }
                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_No)
                {
                    // The Coverage amount logic is added as part of PIR 9702
                    lobjSpouseLifeOption.icdoPersonAccountLifeOption.coverage_amount = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount;
                    if (ablnNewMode)
                    {
                        if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueNewHire)
                        {
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount > idecSpouseSuppGILimit) //PIR 25712 - Changed from 50k to 100k
                            {
                                lobjSpouseLifeOption.icdoPersonAccountLifeOption.coverage_amount = idecSpouseSuppGILimit; //PIR 25712 - Changed from 50k to 100k
                                iblnIsEOIRequired = true;
                            }
                        }
                        else if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueMarriage)
                        {
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount > idecSpouseSuppGILimit) //PIR 25712 - Changed from 50k to 100k
                            {
                                //Backlog PIR 13587 - Commented below logic as per discussion with Maik

                                // The system should round down to the nearest $5000.00
                                //decimal ldecHalfSuppAmount = ldecSuppCoverageAmount > 0 ? ldecSuppCoverageAmount / 2 : 0;
                                //decimal ldecTempAmount = ldecHalfSuppAmount % 5000M;
                                //decimal ldecSuppAmount = ldecHalfSuppAmount - ldecTempAmount;
                                //if (ldecSuppAmount > 0)
                                //    lobjSpouseLifeOption.icdoPersonAccountLifeOption.coverage_amount = ldecSuppAmount;
                                //if(iblnIsEOIflag)
                                //iblnIsEOIRequired = true;
                            }
                        }
                        else if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                        {
                            // With Annual Enrollment don’t post Spouse Supp. Always need EOI.
                            if (iblnIsEOIflag)
                            {
                                lobjSpouseLifeOption.icdoPersonAccountLifeOption.coverage_amount = 0M;
                                iblnIsEOIRequired = true;
                            }
                        }
                    }
                    else
                    {
                        if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueMarriage ||
                            icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonNewHire || // PIR 10384
                            icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                        {
                            if (abusPersonAccountLife.iclbPreviousHistory.IsNull()) abusPersonAccountLife.LoadPreviousHistory();
                            busPersonAccountLifeHistory lobjPrevSpouseHistory = abusPersonAccountLife.GetPreviousHistoryForOption(lobjSpouseLifeOption);
                            if (lobjPrevSpouseHistory.icdoPersonAccountLifeHistory.coverage_amount == 0)
                            {
                                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount > lobjPrevSpouseHistory.icdoPersonAccountLifeHistory.coverage_amount)
                                {
                                    if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueNewHire)
                                    {
                                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount > idecSpouseSuppGILimit) //PIR 25712 - Changed from 50k to 100k
                                        {
                                            lobjSpouseLifeOption.icdoPersonAccountLifeOption.coverage_amount = idecSpouseSuppGILimit; //PIR 25712 - Changed from 50k to 100k
                                            iblnIsEOIRequired = true;
                                        }
                                    }
                                    else if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueMarriage)
                                    {
                                        //Backlog PIR 13587 - Commented below logic as per discussion with Maik

                                        //decimal ldecHalfSuppAmount = ldecSuppCoverageAmount > 0 ? ldecSuppCoverageAmount / 2 : 0;
                                        //decimal ldecTempAmount = ldecHalfSuppAmount % 5000M;
                                        //decimal ldecSuppAmount = ldecHalfSuppAmount - ldecTempAmount;
                                        //if (ldecSuppAmount > 0)
                                        //    lobjSpouseLifeOption.icdoPersonAccountLifeOption.coverage_amount = ldecSuppAmount;
                                        //if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount > 50000M && iblnIsEOIflag)
                                        //    iblnIsEOIRequired = true;
                                    }

                                    else if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                                    {
                                        // With Annual Enrollment don’t post Spouse Supp. Always need EOI.
                                        if (iblnIsEOIflag)
                                        {
                                            lobjSpouseLifeOption.icdoPersonAccountLifeOption.coverage_amount = 0M;
                                            iblnIsEOIRequired = true;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount > lobjPrevSpouseHistory.icdoPersonAccountLifeHistory.coverage_amount && iblnIsEOIflag)
                                {
                                    lobjSpouseLifeOption.icdoPersonAccountLifeOption.coverage_amount = 0M; // PIR 10384
                                    iblnIsEOIRequired = true;
                                }
                            }
                        }
                    }
                    lobjSpouseLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_SpouseSupplemental;
                    lobjSpouseLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueEnrolled;
                    if (lobjSpouseLifeOption.icdoPersonAccountLifeOption.effective_start_date == DateTime.MinValue)
                        lobjSpouseLifeOption.icdoPersonAccountLifeOption.effective_start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
                    lclbFinalLifeOption.Add(lobjSpouseLifeOption);
                }
                else if (lobjSpouseLifeOption.icdoPersonAccountLifeOption.person_account_id > 0 &&
                    lobjSpouseLifeOption.icdoPersonAccountLifeOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled)
                {
                    lobjSpouseLifeOption.icdoPersonAccountLifeOption.coverage_amount = 0.00M;
                    lobjSpouseLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value = busConstant.LevelofCoverage_SpouseSupplemental;
                    lobjSpouseLifeOption.icdoPersonAccountLifeOption.plan_option_status_value = busConstant.PlanOptionStatusValueWaived;
                    lclbFinalLifeOption.Add(lobjSpouseLifeOption);
                }
            }

            abusPersonAccountLife.iclbLifeOption = new Collection<busPersonAccountLifeOption>();
            abusPersonAccountLife.iclbLifeOption = lclbFinalLifeOption;
        }

        //validate that the life options are entered in sequence
        //1) basic
        //2) supple
        //3) dep supple
        //4) spouse supple
        public bool IsSupplementalNotEntered()
        {
            if ((ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount == 0.00M)
                && (!String.IsNullOrEmpty(ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value)))
                return true;
            return false;
        }

        public bool IsDepSupplementalNotEntered()
        {
            if ((String.IsNullOrEmpty(ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value))
                && ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount != 0.00M)
                return true;
            return false;
        }

        public bool IsSupplementalNotSelected()
        {
            if (iblnIsSupplementalSelected)
                return true;
            else
                return false;
        }

        public bool IsDependentNotSelected()
        {
            if (iblnIsDependentSelected)
                return true;
            else
                return false;
        }

        //Is spouse supple is greater than the half of member supplement amount
        public bool IsSpouseSupplementalAmountInValid()
        {
            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount != 0.00M
                && ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount != 0.00M)
                if ((ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount * 0.5M) < ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount)
                    return true;

            return false;
        }

        public bool IsSpouseSupplementalValid()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
            {
                if (ibusPerson.icolPersonContact.IsNull()) ibusPerson.LoadActiveContacts();
                if (ibusPerson.icolPersonContact.Count() == 0) return false;
                if (!ibusPerson.icolPersonContact.Any(i => i.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse &&
                                             i.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive))
                    return false;
            }
            return true;
        }



        //supplemental amount max limit - 200,000
        public bool IsInValidSupplementalAmountEntered()
        {
            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount > 200000M)
                return true;
            return false;
        }
        //spouse supplemental amount max limit - 100,000       
        public bool IsInValidSpouseSupplementalAmountEntered()
        {

            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount > idecSpouseSupplementalLimit)
                return true;
            return false;
        }

        public decimal GetCoverageAmountDetails(string astrLevelOfCoverage)
        {
            string lstrlifeInsurancetypeValue = null;
            DataTable ldtbEndDate = Select("cdoWssPersonAccountEnrollmentRequest.GetEndDateForEmploymentDetailId", new object[1] { icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id });
            if (ldtbEndDate.Rows.Count > 0)
            {
                LoadPersonAccount();

                ibusPersonAccount.LoadPersonAccountLife();
                if (ibusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value.IsNull() && (Convert.ToDateTime(ldtbEndDate.Rows[0]["END_DATE"]).Date == new DateTime(1753, 01, 01).Date))
                {
                    lstrlifeInsurancetypeValue = busConstant.LifeInsuranceTypeActiveMember;
                }
                else
                {
                    lstrlifeInsurancetypeValue = ibusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value;
                }

                if (icdoWssPersonAccountEnrollmentRequest.date_of_change != DateTime.MinValue)
                {
                    DataTable ldtbCovergeAmount = Select("cdoWssPersonAccountEnrollmentRequest.GetValidCoverageAmount", new object[3] { astrLevelOfCoverage, lstrlifeInsurancetypeValue, icdoWssPersonAccountEnrollmentRequest.date_of_change });
                    if (ldtbCovergeAmount.Rows.Count > 0)
                        adecMaxAmount = Convert.ToDecimal(ldtbCovergeAmount.Rows[0]["FULL_COVERAGE_AMT"]);
                }
            }
            return adecMaxAmount;
        }

        public void GetCoverageAmountDetailsSupplemental(ref decimal adecMinAmount, ref decimal adecMaxAmount)
        {

            string lstrlifeInsurancetypeValue = null;
            DataTable ldtbEndDate = Select("cdoWssPersonAccountEnrollmentRequest.GetEndDateForEmploymentDetailId", new object[1] { icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id });
            if (ldtbEndDate.Rows.Count > 0)
            {
                LoadPersonAccount();

                ibusPersonAccount.LoadPersonAccountLife();
                if (ibusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value.IsNull() && (Convert.ToDateTime(ldtbEndDate.Rows[0]["END_DATE"]).Date == new DateTime(1753, 01, 01).Date))
                {
                    lstrlifeInsurancetypeValue = busConstant.LifeInsuranceTypeActiveMember;
                }
                else
                {
                    lstrlifeInsurancetypeValue = ibusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value;
                }

                if (icdoWssPersonAccountEnrollmentRequest.date_of_change != DateTime.MinValue)
                {
                    DataTable ldtbCovergeAmount = Select("cdoWssPersonAccountEnrollmentRequest.GetValidCoverageAmount", new object[3] { busConstant.LevelofCoverage_Supplemental, lstrlifeInsurancetypeValue, icdoWssPersonAccountEnrollmentRequest.date_of_change });
                    if (ldtbCovergeAmount.Rows.Count > 0)
                    {
                        adecMaxAmount = Convert.ToDecimal(ldtbCovergeAmount.Rows[0]["FULL_COVERAGE_AMT"]);
                        ldtbCovergeAmount = ldtbCovergeAmount.AsEnumerable().OrderByDescending(row => row.Field<DateTime>("EFFECTIVE_DATE")).ThenBy(row => row.Field<Decimal>("FULL_COVERAGE_AMT")).AsDataTable();
                        adecMinAmount = Convert.ToDecimal(ldtbCovergeAmount.Rows[0]["FULL_COVERAGE_AMT"]);
                    }
                }
            }
        }
		
        public decimal GetCoverageAmountDetails(string astrLevelOfCoverage, string astrlifeInsurancetypeValue)
        {
            DataTable ldtbEndDate = Select("cdoWssPersonAccountEnrollmentRequest.GetEndDateForEmploymentDetailId", new object[1] { icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id });
            if (ldtbEndDate.Rows.Count > 0)
            {
                if (icdoWssPersonAccountEnrollmentRequest.date_of_change != DateTime.MinValue)
                {
                    DataTable ldtbCovergeAmount = Select("cdoWssPersonAccountEnrollmentRequest.GetValidCoverageAmount", new object[3] { astrLevelOfCoverage, astrlifeInsurancetypeValue, icdoWssPersonAccountEnrollmentRequest.date_of_change });
                    if (ldtbCovergeAmount.Rows.Count > 0 && ldtbCovergeAmount.Rows[0]["FULL_COVERAGE_AMT"]!= DBNull.Value)
                        adecMaxAmount = Convert.ToDecimal(ldtbCovergeAmount.Rows[0]["FULL_COVERAGE_AMT"]);
                }
            }
            return adecMaxAmount;
        }
		
        public void GetSpouseDetails()
        {
            ibusPerson.LoadSpouse();
            if (ibusPerson.ibusSpouse.icdoPerson.person_id != 0)
            {
                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_name = ibusPerson.ibusSpouse.icdoPerson.FullName;
                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_dob = ibusPerson.ibusSpouse.icdoPerson.date_of_birth;
            }
        }

        //if spouse exists then only allow user to enter the spouse supplemental
        public bool IsNotAllowedToEnterSpouseSupplemental()
        {
            if (ibusPerson.ibusSpouse.IsNull())
                ibusPerson.LoadSpouse();

            if ((ibusPerson.ibusSpouse.icdoPerson.person_id == 0)
                && ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount != 0.00M)
                return true;
            return false;
        }

        public bool IsSpousePresent()
        {
            if (ibusPerson.ibusSpouse.IsNull())
                ibusPerson.LoadSpouse();
            if (ibusPerson.ibusSpouse.icdoPerson.person_id == 0)
                return false;
            else
                return true;
        }

        public Collection<cdoCodeValue> LoadDependentSupplementalAmount()
        {
            Collection<cdoCodeValue> lclbResult = new Collection<cdoCodeValue>();
            DataTable ldtbList = Select("cdoWssPersonAccountEnrollmentRequest.LoadDependentSupplementalAmount", new object[1] { busConstant.CodeIdForDependentAmount });
            Collection<cdoCodeValue> lclbList = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            foreach (cdoCodeValue lcdoCodeValue in lclbList)
            {
                DateTime ldtStartDate = Convert.ToDateTime(lcdoCodeValue.data1);
                DateTime ldtEndDate = Convert.ToDateTime(lcdoCodeValue.data2);
                if (icdoWssPersonAccountEnrollmentRequest.date_of_change >= ldtStartDate &&
                    icdoWssPersonAccountEnrollmentRequest.date_of_change <= ldtEndDate)
                {
                    lclbResult.Add(lcdoCodeValue);
                }
            }
            return lclbResult;
        }


        //for Flex Comp Posting
        public ArrayList btnPostFlexComp_Click()
        {
            ArrayList larrlist = new ArrayList();
            if (icdoWssPersonAccountEnrollmentRequest.wss_ben_app_id == 0)
            {
                if (icdoWssPersonAccountEnrollmentRequest.change_effective_date == DateTime.MinValue)
            {
                utlError lerr = new utlError();
                lerr.istrErrorID = "10014";
                lerr.istrErrorMessage = "Change Effective Date is required.";
                larrlist.Add(lerr);
                return larrlist;
            }
            if (icdoWssPersonAccountEnrollmentRequest.reason_value == "ANNE" && icdoWssPersonAccountEnrollmentRequest.status_value == "PEND" && !iblnFinishButtonClicked &&
                icdoWssPersonAccountEnrollmentRequest.change_effective_date < icdoWssPersonAccountEnrollmentRequest.date_of_change)
            {
                utlError lerr = new utlError();
                lerr.istrErrorMessage = "Effective date is less than Current Annual Enrollment Plan Year";
                larrlist.Add(lerr);
                return larrlist;
            }
            else
            {
                if (ibusPersonAccount.IsNull())
                   LoadPersonAccountForPosting();
                if (ibusPersonEmploymentDetail.IsNull())
                    LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                if (ibusPersonAccountEmploymentDetail.IsNull())
                    LoadPersonAccountEmploymentDetail();
                if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value != busConstant.PlanEnrollmentOptionValueWaive)
                {
                    if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                    {
                        ibusMSSPersonAccountFlexComp = CreateFlexCompPersonAccount();
                        if (ibusMSSPersonAccountFlexComp.iarrErrors.Count > 0)
                        {
                            foreach (utlError lobjError in ibusMSSPersonAccountFlexComp.iarrErrors)
                            {
                                larrlist.Add(lobjError);
                                return larrlist;
                            }
                        }
                        else
                        {
                            //insert flex comp conversion
                            if (iclbMSSFlexCompConversion.Count > 0)
                            {
                                var lenumlist = iclbMSSFlexCompConversion.Where(lobjConversion => lobjConversion.icdoWssPersonAccountFlexCompConversion.istrIsSelected == busConstant.Flag_Yes
                                    && lobjConversion.icdoWssPersonAccountFlexCompConversion.effective_start_date  != DateTime.MinValue);
                                foreach (busWssPersonAccountFlexCompConversion lobjPAFlexCompConversion in lenumlist)
                                {
                                    busPersonAccountFlexcompConversion lobjNewConversion = new busPersonAccountFlexcompConversion
                                    {
                                        icdoPersonAccountFlexcompConversion = new cdoPersonAccountFlexcompConversion()
                                    };
                                    lobjNewConversion.icdoPersonAccountFlexcompConversion.effective_start_date = lobjPAFlexCompConversion.icdoWssPersonAccountFlexCompConversion.effective_start_date;
                                    lobjNewConversion.icdoPersonAccountFlexcompConversion.effective_end_date = lobjPAFlexCompConversion.icdoWssPersonAccountFlexCompConversion.effective_end_date;
                                    lobjNewConversion.icdoPersonAccountFlexcompConversion.org_id = lobjPAFlexCompConversion.icdoWssPersonAccountFlexCompConversion.org_id;
                                    lobjNewConversion.icdoPersonAccountFlexcompConversion.person_account_id = ibusMSSPersonAccountFlexComp.icdoPersonAccount.person_account_id;
                                    //PIR 17081
                                    //lobjNewConversion.icdoPersonAccountFlexcompConversion.is_enrollment_report_generated = busConstant.Flag_No; // PIR 13601
                                    lobjNewConversion.icdoPersonAccountFlexcompConversion.ienuObjectState = ObjectState.Insert;
                                    lobjNewConversion.BeforeValidate(utlPageMode.New);
                                    lobjNewConversion.ValidateHardErrors(utlPageMode.New);
                                    if (lobjNewConversion.iarrErrors.Count > 0)
                                    {
                                        foreach (utlError lobjError in lobjNewConversion.iarrErrors)
                                        {
                                            larrlist.Add(lobjError);
                                        }
                                        return larrlist;
                                    }
                                    else
                                    {
                                        lobjNewConversion.BeforePersistChanges();
                                        lobjNewConversion.PersistChanges();
                                        lobjNewConversion.ibusPersonAccount = ibusMSSPersonAccountFlexComp.ibusPersonAccount;
                                        lobjNewConversion.ibusPersonAccountFlexComp = ibusMSSPersonAccountFlexComp;
                                        lobjNewConversion.AfterPersistChanges();
                                        UpdateFlexCompConversion(lobjNewConversion);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        LoadMSSPersonAccountFlexComp();
                        ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
                        ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail.ibusPersonEmployment = ibusPersonEmploymentDetail.ibusPersonEmployment;
                        ibusMSSPersonAccountFlexComp.ibusPersonAccount = ibusPersonAccount;

                        if (ibusMSSPersonAccountFlexComp.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusFlexCompEnrolled)
                            ibusMSSPersonAccountFlexComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusFlexCompEnrolled;

                        ibusMSSPersonAccountFlexComp.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.reason_value = icdoWssPersonAccountEnrollmentRequest.reason_value;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.premium_conversion_waiver_flag = icdoMSSFlexComp.premium_conversion_waiver_flag;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.direct_deposit_flag = icdoMSSFlexComp.direct_deposit_flag;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.inside_mail_flag = icdoMSSFlexComp.inside_mail_flag;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.flex_comp_type_value = busConstant.FlexCompTypeValueActive;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccount.history_change_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccount.ienuObjectState = ObjectState.Update;

                        ibusMSSPersonAccountFlexComp.LoadPlanEffectiveDate();
                        ibusMSSPersonAccountFlexComp.LoadOrgPlan(ibusMSSPersonAccountFlexComp.idtPlanEffectiveDate);
                        ibusMSSPersonAccountFlexComp.LoadFlexCompOptionUpdate();
                        LoadMSSFlexCompOptions(ibusMSSPersonAccountFlexComp);

                        ibusMSSPersonAccountFlexComp.BeforeValidate(utlPageMode.All);
                        ibusMSSPersonAccountFlexComp.ValidateHardErrors(utlPageMode.All);
                        if (ibusMSSPersonAccountFlexComp.iarrErrors.Count > 0)
                        {
                            //add the array list
                            foreach (utlError lobjErr in ibusMSSPersonAccountFlexComp.iarrErrors)
                            {
                                larrlist.Add(lobjErr);
                                return larrlist;
                            }
                        }
                        else
                        {
                            //insert or update the conversion
                            ibusMSSPersonAccountFlexComp.LoadFlexCompConversion();
                            foreach (busWssPersonAccountFlexCompConversion lobjFlexCompConversion in iclbMSSFlexCompConversion)
                            {
                                var lenumConversion = ibusMSSPersonAccountFlexComp.iclbFlexcompConversion
                                            .Where(lobjConversion => lobjConversion.icdoPersonAccountFlexcompConversion.org_id == lobjFlexCompConversion.icdoWssPersonAccountFlexCompConversion.org_id);
                                if (lenumConversion.Count() > 0)
                                {
                                    busPersonAccountFlexcompConversion lobjNewFlexCompConversion = new busPersonAccountFlexcompConversion
                                    {
                                        icdoPersonAccountFlexcompConversion = new cdoPersonAccountFlexcompConversion()
                                    };
                                    lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion = lenumConversion.FirstOrDefault().icdoPersonAccountFlexcompConversion;
                                    if (lobjFlexCompConversion.icdoWssPersonAccountFlexCompConversion.effective_start_date == DateTime.MinValue)
                                    {
                                        // PIR 10251 -- Delete the existing provider if no values is entered in requests
                                        DataTable ldtbResults = Select<cdoWssPersonAccountFlexCompConversion>(new string[1] { "target_person_account_flex_comp_conversion_id" },
                                                                new object[1] { lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.person_account_flex_comp_conversion_id }, null, null);
                                        foreach (DataRow ldtr in ldtbResults.Rows)
                                        {
                                            busWssPersonAccountFlexCompConversion lobjMSSFlexCompConversion = new busWssPersonAccountFlexCompConversion();
                                            lobjMSSFlexCompConversion.icdoWssPersonAccountFlexCompConversion = new cdoWssPersonAccountFlexCompConversion();
                                            lobjMSSFlexCompConversion.icdoWssPersonAccountFlexCompConversion.LoadData(ldtr);
                                            lobjMSSFlexCompConversion.icdoWssPersonAccountFlexCompConversion.target_person_account_flex_comp_conversion_id = 0;
                                            lobjMSSFlexCompConversion.icdoWssPersonAccountFlexCompConversion.Update();
                                        }

                                        busPersonAccountFlexcompConversion lobjFlexCompProvider = new busPersonAccountFlexcompConversion();
                                        if (lobjFlexCompProvider.FindPersonAccountFlexcompConversion(lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.person_account_flex_comp_conversion_id))
                                            lobjFlexCompProvider.icdoPersonAccountFlexcompConversion.Delete();
                                    }
                                    else
                                    {
                                        lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.effective_start_date = lobjFlexCompConversion.icdoWssPersonAccountFlexCompConversion.effective_start_date;
                                        lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.effective_end_date = lobjFlexCompConversion.icdoWssPersonAccountFlexCompConversion.effective_end_date;
                                        // PIR 10270 -- End date is not removed
                                        if (lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.effective_start_date >
                                            lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.effective_end_date)
                                            lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.effective_end_date = DateTime.MinValue;
                                        //PIR 17081
                                        //lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.is_enrollment_report_generated = busConstant.Flag_No; // PIR 13601
                                        lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.ienuObjectState = ObjectState.Update;
                                        lobjNewFlexCompConversion.BeforeValidate(utlPageMode.All);
                                        lobjNewFlexCompConversion.ValidateHardErrors(utlPageMode.All);
                                        if (lobjNewFlexCompConversion.iarrErrors.Count > 0)
                                        {
                                            foreach (utlError lobjErrors in lobjNewFlexCompConversion.iarrErrors)
                                            {
                                                larrlist.Add(lobjErrors);
                                            }
                                            return larrlist;
                                        }
                                        else
                                        {
                                            lobjNewFlexCompConversion.BeforePersistChanges();
                                            lobjNewFlexCompConversion.PersistChanges();
                                            lobjNewFlexCompConversion.ibusPersonAccount = ibusMSSPersonAccountFlexComp.ibusPersonAccount;
                                                lobjNewFlexCompConversion.ibusPersonAccount.icdoPersonAccount = ibusMSSPersonAccountFlexComp.icdoPersonAccount;//PIR 20885 Flex comp Enl Data table can't insert the record if plan status change from suspend to enroll
                                                lobjNewFlexCompConversion.ibusPersonAccountFlexComp = ibusMSSPersonAccountFlexComp;
                                            lobjNewFlexCompConversion.AfterPersistChanges();
                                        }
                                    }
                                }
                                else if (lobjFlexCompConversion.icdoWssPersonAccountFlexCompConversion.istrIsSelected == busConstant.Flag_Yes && lobjFlexCompConversion.icdoWssPersonAccountFlexCompConversion.effective_start_date != DateTime.MinValue)
                                {
                                    busPersonAccountFlexcompConversion lobjNewFlexCompConversion = new busPersonAccountFlexcompConversion
                                    {
                                        icdoPersonAccountFlexcompConversion = new cdoPersonAccountFlexcompConversion()
                                    };
                                    lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.org_id = lobjFlexCompConversion.icdoWssPersonAccountFlexCompConversion.org_id;
                                    lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.effective_start_date = lobjFlexCompConversion.icdoWssPersonAccountFlexCompConversion.effective_start_date;
                                    lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.effective_end_date = lobjFlexCompConversion.icdoWssPersonAccountFlexCompConversion.effective_end_date;
                                    lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.person_account_id = ibusMSSPersonAccountFlexComp.icdoPersonAccount.person_account_id;//6407 and 6287
                                    //PIR 17081
                                    //lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.is_enrollment_report_generated = busConstant.Flag_No; // PIR 13601
                                    lobjNewFlexCompConversion.icdoPersonAccountFlexcompConversion.ienuObjectState = ObjectState.Insert;
                                    lobjNewFlexCompConversion.BeforeValidate(utlPageMode.New);
                                    lobjNewFlexCompConversion.ValidateHardErrors(utlPageMode.New);
                                    iblnIsBenefitEnrollmentFlagNo = true;
                                    if (lobjNewFlexCompConversion.iarrErrors.Count > 0)
                                    {
                                        foreach (utlError lobjErrors in lobjNewFlexCompConversion.iarrErrors)
                                        {
                                            larrlist.Add(lobjErrors);
                                        }
                                        return larrlist;
                                    }
                                    else
                                    {
                                        lobjNewFlexCompConversion.BeforePersistChanges();
                                        lobjNewFlexCompConversion.PersistChanges();
                                        lobjNewFlexCompConversion.ibusPersonAccount = ibusMSSPersonAccountFlexComp.ibusPersonAccount;
                                        lobjNewFlexCompConversion.ibusPersonAccount.icdoPersonAccount = ibusMSSPersonAccountFlexComp.icdoPersonAccount;//PIR 20885 Flex comp Enl Data table can't insert the record if plan status change from suspend to enroll
                                        lobjNewFlexCompConversion.ibusPersonAccountFlexComp = ibusMSSPersonAccountFlexComp;
                                        lobjNewFlexCompConversion.AfterPersistChanges();

                                        UpdateFlexCompConversion(lobjNewFlexCompConversion);
                                    }
                                }
                            }

                            if (larrlist.Count == 0)
                            {
                                ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
                                ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(ibusPersonAccountEmploymentDetail);

                                ibusMSSPersonAccountFlexComp.BeforePersistChanges();
                                ibusMSSPersonAccountFlexComp.PersistChanges();
                                ibusMSSPersonAccountFlexComp.ValidateSoftErrors();
                                ibusMSSPersonAccountFlexComp.UpdateValidateStatus();
                                //to insert history
                                ibusMSSPersonAccountFlexComp.icdoPersonAccount.status_value = busConstant.StatusValid;

                                ibusMSSPersonAccountFlexComp.iblnIsFromMSSForEnrollmentData = true;//PIR 20017
                                ibusMSSPersonAccountFlexComp.AfterPersistChanges();
                                if (iblnIsBenefitEnrollmentFlagNo)
                                    ibusMSSPersonAccountFlexComp.UpdateIsEnrollmentReportFlag();
                            }
                        }
                    }

                    if (ibusMSSPersonAccountFlexComp.iarrErrors.Count == 0)
                    {
                        ibusMSSPersonAccountFlexComp.LoadPremiumConversionForDentalVisionLife();
                        ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id = ibusMSSPersonAccountFlexComp.icdoPersonAccount.person_account_id;
                        if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive)
                            ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueWaived;
                        else
                            ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                        ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();

                        if (ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueWaived)
                            InsertIntoEnrollmentData();
                        else if (ibusMSSPersonAccountFlexComp.icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid && ibusMSSPersonAccountFlexComp.IsHistoryEntryRequired)
                            ibusMSSPersonAccountFlexComp.InsertIntoEnrollmentData();//PIR 20017


                        busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                            busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);
                    }
                    else
                    {
                        iblnIsMemberAccountInReviewStatus = true;
                        return larrlist;
                    }

                    if (ibusMSSPersonAccountFlexComp.ibusSoftErrors.IsNotNull() && ibusMSSPersonAccountFlexComp.ibusSoftErrors.iclbError.IsNotNull())
                    {
                        int lintSoftErrCount = ibusMSSPersonAccountFlexComp.ibusSoftErrors.iclbError.Where(o => o.severity_value == busConstant.MessageSeverityInformation).Count();
                        if (ibusMSSPersonAccountFlexComp.ibusSoftErrors.iclbError.Count > lintSoftErrCount)
                            iblnIsMemberAccountInReviewStatus = true;
                        else
                        {
                            icdoWssPersonAccountEnrollmentRequest.target_person_account_id = ibusMSSPersonAccountFlexComp.icdoPersonAccount.person_account_id;
                            icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                            icdoWssPersonAccountEnrollmentRequest.Update();
                        }
                    }
                }
                else
                {
                    // PIR 10264
                    if ((icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment) &&
                        (ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled)
                        && icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value != busConstant.PlanEnrollmentOptionValueWaive)
                    {
                        LoadMSSPersonAccountFlexComp();
                        ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
                        ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail.ibusPersonEmployment = ibusPersonEmploymentDetail.ibusPersonEmployment;
                        ibusMSSPersonAccountFlexComp.ibusPersonAccount = ibusPersonAccount;

                        if (ibusMSSPersonAccountFlexComp.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusFlexCompEnrolled)
                            ibusMSSPersonAccountFlexComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusFlexCompEnrolled;

                        ibusMSSPersonAccountFlexComp.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.reason_value = icdoWssPersonAccountEnrollmentRequest.reason_value;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.premium_conversion_waiver_flag = icdoMSSFlexComp.premium_conversion_waiver_flag;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.direct_deposit_flag = icdoMSSFlexComp.direct_deposit_flag;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.inside_mail_flag = icdoMSSFlexComp.inside_mail_flag;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.flex_comp_type_value = busConstant.FlexCompTypeValueActive;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccount.history_change_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
                        ibusMSSPersonAccountFlexComp.icdoPersonAccount.ienuObjectState = ObjectState.Update;

                        ibusMSSPersonAccountFlexComp.LoadPlanEffectiveDate();
                        ibusMSSPersonAccountFlexComp.LoadOrgPlan(ibusMSSPersonAccountFlexComp.idtPlanEffectiveDate);
                        ibusMSSPersonAccountFlexComp.LoadFlexCompOptionUpdate();
                        LoadMSSFlexCompOptions(ibusMSSPersonAccountFlexComp);

                        ibusMSSPersonAccountFlexComp.BeforeValidate(utlPageMode.All);
                        ibusMSSPersonAccountFlexComp.ValidateHardErrors(utlPageMode.All);
                        if (ibusMSSPersonAccountFlexComp.iarrErrors.Count > 0)
                        {
                            //add the array list
                            foreach (utlError lobjErr in ibusMSSPersonAccountFlexComp.iarrErrors)
                            {
                                larrlist.Add(lobjErr);
                                return larrlist;
                            }
                        }
                        else
                        {
                            if (larrlist.Count == 0)
                            {
                                ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
                                ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(ibusPersonAccountEmploymentDetail);

                                ibusMSSPersonAccountFlexComp.BeforePersistChanges();
                                ibusMSSPersonAccountFlexComp.PersistChanges();
                                ibusMSSPersonAccountFlexComp.ValidateSoftErrors();
                                ibusMSSPersonAccountFlexComp.UpdateValidateStatus();
                                //to insert history
                                ibusMSSPersonAccountFlexComp.icdoPersonAccount.status_value = busConstant.StatusValid;
                                ibusMSSPersonAccountFlexComp.IsHistoryEntryRequired = true;
                                ibusMSSPersonAccountFlexComp.AfterPersistChanges();
                            }
                        }
                    }
                    else
                    {
                        // PIR 13790 - added Condition to check if person account exists
                        // Waive Plan
                        //ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueWaived;
                        //// PIR 11684
                        //ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.is_waiver_report_generated = busConstant.Flag_No;
                        //busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                        //                                        icdoWssPersonAccountEnrollmentRequest.plan_id, iobjPassInfo);
                        //// PIR 11684 end
                        //ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();

                        if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                        {
                            LoadMSSPersonAccountFlexComp();
                            ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
                            ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail.ibusPersonEmployment = ibusPersonEmploymentDetail.ibusPersonEmployment;
                            ibusMSSPersonAccountFlexComp.ibusPersonAccount = ibusPersonAccount;

                            if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive)
                                ibusMSSPersonAccountFlexComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusFlexSuspended;

                            ibusMSSPersonAccountFlexComp.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                            ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.reason_value = icdoWssPersonAccountEnrollmentRequest.reason_value;
                            ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.premium_conversion_waiver_flag = icdoMSSFlexComp.premium_conversion_waiver_flag;
                            ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.direct_deposit_flag = icdoMSSFlexComp.direct_deposit_flag;
                            ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.inside_mail_flag = icdoMSSFlexComp.inside_mail_flag;
                            ibusMSSPersonAccountFlexComp.icdoPersonAccountFlexComp.flex_comp_type_value = busConstant.FlexCompTypeValueActive;
                            ibusMSSPersonAccountFlexComp.icdoPersonAccount.history_change_date = AnnualEnrollmentEffectiveDate;
                            ibusMSSPersonAccountFlexComp.icdoPersonAccount.ienuObjectState = ObjectState.Update;

                            ibusMSSPersonAccountFlexComp.LoadPlanEffectiveDate();
                            ibusMSSPersonAccountFlexComp.LoadOrgPlan(ibusMSSPersonAccountFlexComp.idtPlanEffectiveDate);
                            ibusMSSPersonAccountFlexComp.LoadFlexCompOptionUpdate();
                            LoadMSSFlexCompOptions(ibusMSSPersonAccountFlexComp);
                            if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive
                                && ibusMSSPersonAccountFlexComp.iclbFlexCompOption.Count > 0)
                                ibusMSSPersonAccountFlexComp.iclbFlexCompOption.ForEach(o => o.icdoPersonAccountFlexCompOption.annual_pledge_amount = 0); //PIR 17038
                            ibusMSSPersonAccountFlexComp.BeforeValidate(utlPageMode.All);
                            ibusMSSPersonAccountFlexComp.ValidateHardErrors(utlPageMode.All);
                            if (ibusMSSPersonAccountFlexComp.iarrErrors.Count > 0)
                            {
                                //add the array list
                                foreach (utlError lobjErr in ibusMSSPersonAccountFlexComp.iarrErrors)
                                {
                                    larrlist.Add(lobjErr);
                                    return larrlist;
                                }
                            }
                            else
                            {
                                if (larrlist.Count == 0)
                                {
                                    ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
                                    ibusMSSPersonAccountFlexComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(ibusPersonAccountEmploymentDetail);

                                    ibusMSSPersonAccountFlexComp.BeforePersistChanges();
                                    ibusMSSPersonAccountFlexComp.PersistChanges();
                                    ibusMSSPersonAccountFlexComp.ValidateSoftErrors();
                                    ibusMSSPersonAccountFlexComp.UpdateValidateStatus();
                                    //to insert history
                                    ibusMSSPersonAccountFlexComp.icdoPersonAccount.status_value = busConstant.StatusValid;
                                    ibusMSSPersonAccountFlexComp.icdoPersonAccount.suppress_warnings_flag = busConstant.Flag_No;
                                    ibusMSSPersonAccountFlexComp.IsHistoryEntryRequired = true;
                                    ibusMSSPersonAccountFlexComp.AfterPersistChanges();
                                }
                            }
                        }
                        //Waive Plan
                        ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueWaived;
                        // PIR 11684
                        ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.is_waiver_report_generated = busConstant.Flag_No;
                        busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                                                icdoWssPersonAccountEnrollmentRequest.plan_id, iobjPassInfo);
                        // PIR 11684 end
                        ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();
                        //PIR 17081
                        InsertIntoEnrollmentData();
                    }

                    icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPosted;
                    icdoWssPersonAccountEnrollmentRequest.Update();

                    busWSSHelper.PublishMSSMessage(0, 0, string.Format("Your enrollment/change for the {0} has been processed.", ibusPlan.icdoPlan.mss_plan_name),
                                                                        busConstant.WSS_MessageBoard_Priority_High, ibusPerson.icdoPerson.person_id);
                }
                larrlist.Add(this);
                EvaluateInitialLoadRules();
            }
            }
            else //Cobra posting
            {
                larrlist = IsChangeEffectiveDateEntered(larrlist);
                if (larrlist.Count > 0)
                {
                    return larrlist;
                }
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                busPersonAccountFlexComp lobjPersonAccountFlexComp = new busPersonAccountFlexComp();
                if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll &&
                    ibusPersonAccount.icdoPersonAccount.person_account_id > 0 &&
                    lobjPersonAccountFlexComp.FindPersonAccountFlexComp(ibusPersonAccount.icdoPersonAccount.person_account_id))
                {
                    lobjPersonAccountFlexComp = SetUpFlexCompCobraObject(lobjPersonAccountFlexComp);
                    lobjPersonAccountFlexComp.BeforeValidate(utlPageMode.Update);
                    lobjPersonAccountFlexComp.ValidateHardErrors(utlPageMode.Update);
                    if (lobjPersonAccountFlexComp.iarrErrors.Count > 0)
                    {
                        foreach (utlError lobjErr in lobjPersonAccountFlexComp.iarrErrors)
                        {
                            utlError lerror = new utlError
                            {
                                istrErrorID = lobjErr.istrErrorID,
                                istrErrorMessage = lobjErr.istrDisplayMessage
                            };
                            larrlist.Add(lerror);
                            return larrlist;
                        }
                    }
                    else
                    {
                        lobjPersonAccountFlexComp.BeforePersistChanges();
                        lobjPersonAccountFlexComp.PersistChanges();
                        lobjPersonAccountFlexComp.ValidateSoftErrors();
                        lobjPersonAccountFlexComp.UpdateValidateStatus();
                        if (!lobjPersonAccountFlexComp.IsHistoryEntryRequired) lobjPersonAccountFlexComp.IsHistoryEntryRequired = true;
                        if (lobjPersonAccountFlexComp.icdoPersonAccount.status_value != busConstant.PersonAccountStatusValid)
                            lobjPersonAccountFlexComp.icdoPersonAccount.status_value = busConstant.PersonAccountStatusValid;
                        lobjPersonAccountFlexComp.AfterPersistChanges();
                        UpdateRequestStatusToProcessed(ibusPersonAccount.icdoPersonAccount.person_account_id);
                        larrlist.Add(this);
                        EvaluateInitialLoadRules();
                        return larrlist;
                    }
                }
            }
            larrlist.Add(this);
            return larrlist;
        }

        private ArrayList IsChangeEffectiveDateEntered(ArrayList arrlist)
        {
            if (icdoWssPersonAccountEnrollmentRequest.change_effective_date == DateTime.MinValue)
            {
                utlError lerr = new utlError();
                lerr.istrErrorID = "10014";
                lerr.istrErrorMessage = "Change Effective Date is required.";
                arrlist.Add(lerr);
            }
            return arrlist;
        }

        private busPersonAccountFlexComp SetUpFlexCompCobraObject(busPersonAccountFlexComp abusPersonAccountFlexComp)
        {
            abusPersonAccountFlexComp.icdoPersonAccountFlexComp.flex_comp_type_value = busConstant.FlexCompTypeValueCOBRA;
            abusPersonAccountFlexComp.icdoPersonAccount.history_change_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            abusPersonAccountFlexComp.ibusPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            abusPersonAccountFlexComp.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
            abusPersonAccountFlexComp.ibusPersonAccount = ibusPersonAccount;
            abusPersonAccountFlexComp.icdoPersonAccount.person_employment_dtl_id = 0;
            abusPersonAccountFlexComp.icdoPersonAccountFlexComp.reason_value = icdoWssPersonAccountEnrollmentRequest.reason_value;
            abusPersonAccountFlexComp.icdoPersonAccount.ienuObjectState = ObjectState.Update;
            abusPersonAccountFlexComp.LoadOrgPlan(icdoWssPersonAccountEnrollmentRequest.change_effective_date);
            abusPersonAccountFlexComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusFlexCompEnrolled;
            abusPersonAccountFlexComp.LoadFlexCompOptionUpdate();
            abusPersonAccountFlexComp.iclbFlexCompOption.ForEach(option => { option.icdoPersonAccountFlexCompOption.effective_end_date = DateTime.MinValue; });
            return abusPersonAccountFlexComp;
        }
        private busPersonAccountFlexComp CreateFlexCompPersonAccount()
        {
            busPersonAccountFlexComp lobjPersonAccountFlexComp = new busPersonAccountFlexComp
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp()
            };

            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

            lobjPersonAccountFlexComp.icdoPersonAccount.person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
            lobjPersonAccountFlexComp.icdoPersonAccount.plan_id = icdoWssPersonAccountEnrollmentRequest.plan_id;
            lobjPersonAccountFlexComp.icdoPersonAccount.person_employment_dtl_id = icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
            lobjPersonAccountFlexComp.ibusPerson = ibusPerson;
            lobjPersonAccountFlexComp.ibusPlan = ibusPlan;
            lobjPersonAccountFlexComp.ibusPersonEmploymentDetail = ibusPersonEmploymentDetail;
            lobjPersonAccountFlexComp.ibusPersonEmploymentDetail.ibusPersonEmployment = ibusPersonEmploymentDetail.ibusPersonEmployment;
            lobjPersonAccountFlexComp.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;

            lobjPersonAccountFlexComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusFlexCompEnrolled;
            lobjPersonAccountFlexComp.icdoPersonAccount.start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            lobjPersonAccountFlexComp.icdoPersonAccount.history_change_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            lobjPersonAccountFlexComp.icdoPersonAccountFlexComp.reason_value = icdoWssPersonAccountEnrollmentRequest.reason_value;
            lobjPersonAccountFlexComp.icdoPersonAccountFlexComp.premium_conversion_waiver_flag = icdoMSSFlexComp.premium_conversion_waiver_flag;
            lobjPersonAccountFlexComp.icdoPersonAccountFlexComp.direct_deposit_flag = icdoMSSFlexComp.direct_deposit_flag;
            lobjPersonAccountFlexComp.icdoPersonAccountFlexComp.inside_mail_flag = icdoMSSFlexComp.inside_mail_flag;
            lobjPersonAccountFlexComp.icdoPersonAccountFlexComp.flex_comp_type_value = busConstant.FlexCompTypeValueActive;

            lobjPersonAccountFlexComp.icdoPersonAccountFlexComp.ienuObjectState = ObjectState.Insert;
            lobjPersonAccountFlexComp.icdoPersonAccount.ienuObjectState = ObjectState.Insert;
            lobjPersonAccountFlexComp.icdoPersonAccount.status_value = busConstant.StatusValid;
            lobjPersonAccountFlexComp.iclbFlexCompOption = new Collection<busPersonAccountFlexCompOption>();

            LoadMSSFlexCompOptions(lobjPersonAccountFlexComp);
            lobjPersonAccountFlexComp.LoadOrgPlan();
            lobjPersonAccountFlexComp.BeforeValidate(utlPageMode.New);
            lobjPersonAccountFlexComp.ValidateHardErrors(utlPageMode.New);
            if (lobjPersonAccountFlexComp.iarrErrors.Count > 0)
                return lobjPersonAccountFlexComp;
            lobjPersonAccountFlexComp.BeforePersistChanges();
            lobjPersonAccountFlexComp.PersistChanges();
            lobjPersonAccountFlexComp.ValidateSoftErrors();
            lobjPersonAccountFlexComp.UpdateValidateStatus();
            //to insert history
            lobjPersonAccountFlexComp.icdoPersonAccount.status_value = busConstant.StatusValid;

            lobjPersonAccountFlexComp.iblnIsFromMSSForEnrollmentData = true;//PIR 20017
            lobjPersonAccountFlexComp.AfterPersistChanges();
            lobjPersonAccountFlexComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
            lobjPersonAccountFlexComp.ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl.Add(ibusPersonAccountEmploymentDetail);
            return lobjPersonAccountFlexComp;
        }

        private void UpdateFlexCompConversion(busPersonAccountFlexcompConversion aobjFlexCompConversion)
        {
            var lenum = iclbMSSFlexCompConversion.Where(lobj => lobj.icdoWssPersonAccountFlexCompConversion.org_id == aobjFlexCompConversion.icdoPersonAccountFlexcompConversion.org_id);
            if (lenum.Count() > 0)
            {
                busWssPersonAccountFlexCompConversion lobjFlexCompConversion = lenum.FirstOrDefault();
                lobjFlexCompConversion.icdoWssPersonAccountFlexCompConversion.target_person_account_flex_comp_conversion_id = aobjFlexCompConversion.icdoPersonAccountFlexcompConversion.person_account_flex_comp_conversion_id;
                lobjFlexCompConversion.icdoWssPersonAccountFlexCompConversion.Update();
            }
        }

        private void LoadMSSFlexCompOptions(busPersonAccountFlexComp lobjPersonAccountFlexComp)
        {
            Collection<busPersonAccountFlexCompOption> lclbFlexOption = new Collection<busPersonAccountFlexCompOption>();
            Collection<busPersonAccountFlexCompOption> lclbFinalFlexOption = new Collection<busPersonAccountFlexCompOption>();
            lclbFlexOption = lobjPersonAccountFlexComp.iclbFlexCompOption;

            // PIR 10251 -- Even if no pledge amount is entered, that needs to be updated in the plan enrollment
            busPersonAccountFlexCompOption lobjFlexOption = new busPersonAccountFlexCompOption
            {
                icdoPersonAccountFlexCompOption = new cdoPersonAccountFlexCompOption()
            };
            //get option by level of coverage
            var lenumLO = lclbFlexOption
                .Where(lobjLO => lobjLO.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending);
            if (lenumLO.Count() > 0)
            {
                lobjFlexOption.icdoPersonAccountFlexCompOption = lenumLO.FirstOrDefault().icdoPersonAccountFlexCompOption;
            }
            lobjFlexOption.icdoPersonAccountFlexCompOption.annual_pledge_amount = icdoMSSFlexCompOption.medical_annual_pledge_amount;
            lobjFlexOption.icdoPersonAccountFlexCompOption.effective_start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            lobjFlexOption.icdoPersonAccountFlexCompOption.effective_end_date = DateTime.MinValue;
            lobjFlexOption.icdoPersonAccountFlexCompOption.level_of_coverage_value = busConstant.FlexLevelOfCoverageMedicareSpending;
            lclbFinalFlexOption.Add(lobjFlexOption);

            busPersonAccountFlexCompOption lobjFlexDCRAOption = new busPersonAccountFlexCompOption
            {
                icdoPersonAccountFlexCompOption = new cdoPersonAccountFlexCompOption()
            };
            //get option by level of coverage
            var lenumDCRALO = lclbFlexOption
                .Where(lobjLO => lobjLO.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending);
            if (lenumDCRALO.Count() > 0)
            {
                lobjFlexDCRAOption.icdoPersonAccountFlexCompOption = lenumDCRALO.FirstOrDefault().icdoPersonAccountFlexCompOption;
            }
            lobjFlexDCRAOption.icdoPersonAccountFlexCompOption.annual_pledge_amount = icdoMSSFlexCompOption.dependent_annual_pledge_amount;
            lobjFlexDCRAOption.icdoPersonAccountFlexCompOption.effective_start_date = icdoWssPersonAccountEnrollmentRequest.change_effective_date;
            lobjFlexDCRAOption.icdoPersonAccountFlexCompOption.effective_end_date = DateTime.MinValue;
            lobjFlexDCRAOption.icdoPersonAccountFlexCompOption.level_of_coverage_value = busConstant.FlexLevelOfCoverageDependentSpending;
            lclbFinalFlexOption.Add(lobjFlexDCRAOption);

            lobjPersonAccountFlexComp.iclbFlexCompOption = new Collection<busPersonAccountFlexCompOption>();
            lobjPersonAccountFlexComp.iclbFlexCompOption = lclbFinalFlexOption;
        }

        public void LoadMSSPersonAccountFlexComp()
        {
            if (ibusMSSPersonAccountFlexComp.IsNull())
                ibusMSSPersonAccountFlexComp = new busPersonAccountFlexComp();

            ibusMSSPersonAccountFlexComp.FindPersonAccountFlexComp(ibusPersonAccount.icdoPersonAccount.person_account_id);
        }

        public void LoadTotalSalaryRedirectionForPlanYearForMedical()
        {
            if (icdoMSSFlexCompOption.medical_salary_per_pay_period > 0.00M
                && icdoMSSFlexCompOption.medical_number_of_checks > 0)
                icdoMSSFlexCompOption.medical_annual_pledge_amount = ((decimal)icdoMSSFlexCompOption.medical_number_of_checks * icdoMSSFlexCompOption.medical_salary_per_pay_period);
        }

        public void LoadTotalSalaryRedirectionForPlanYearForDependent()
        {
            if (icdoMSSFlexCompOption.dependent_salary_per_pay_period > 0.00M
                && icdoMSSFlexCompOption.dependent_number_of_checks > 0)
                icdoMSSFlexCompOption.dependent_annual_pledge_amount = ((decimal)icdoMSSFlexCompOption.dependent_number_of_checks * icdoMSSFlexCompOption.dependent_salary_per_pay_period);
        }

        public decimal IsMedicareAmountExceedTheLimit()
        {
            Collection<cdoCodeValue> lclbCodeValue = GetCodeValue(416);
            foreach (cdoCodeValue lcdoCV in lclbCodeValue)
            {
                if (busGlobalFunctions.CheckDateOverlapping(icdoWssPersonAccountEnrollmentRequest.date_of_change,
                                Convert.ToDateTime(lcdoCV.data1), Convert.ToDateTime(lcdoCV.data2)))
                {
                    if (icdoMSSFlexCompOption.medical_annual_pledge_amount > Convert.ToDecimal(lcdoCV.data3))
                        return Convert.ToDecimal(lcdoCV.data3);
                }
            }
            return 0M;
        }

        public decimal IsDependentAmountExceedTheLimit()
        {
            Collection<cdoCodeValue> lclbCodeValue = GetCodeValue(417);
            foreach (cdoCodeValue lcdoCV in lclbCodeValue)
            {
                if (busGlobalFunctions.CheckDateOverlapping(icdoWssPersonAccountEnrollmentRequest.date_of_change,
                                Convert.ToDateTime(lcdoCV.data1), Convert.ToDateTime(lcdoCV.data2)))
                {
                    if (icdoMSSFlexCompOption.dependent_annual_pledge_amount > Convert.ToDecimal(lcdoCV.data3))
                        return Convert.ToDecimal(lcdoCV.data3);
                }
            }
            return 0M;
        }

        public void SetEffectiveStartDate()
        {
            if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusPersonEmploymentDetail.LoadPersonEmployment();

            if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
            {
                msra_effective_start_date = AnnualEnrollmentEffectiveDate;
                dcra_effective_start_date = AnnualEnrollmentEffectiveDate;
            }
            else
            {
                if (DateTime.Now.Day <= 15)
                {
                    msra_effective_start_date = DateTime.Now.GetFirstDayofNextMonth();
                    dcra_effective_start_date = DateTime.Now.GetFirstDayofNextMonth();
                }
                else
                {
                    msra_effective_start_date = DateTime.Now.AddMonths(1).GetFirstDayofNextMonth();
                    dcra_effective_start_date = DateTime.Now.AddMonths(1).GetFirstDayofNextMonth();
                }
            }
        }

        public DateTime msra_effective_start_date { get; set; }

        public DateTime dcra_effective_start_date { get; set; }

        public string msra_effective_start_long_date
        {
            get
            {
                if (msra_effective_start_date != DateTime.MinValue)
                    return msra_effective_start_date.ToString(busConstant.DateFormatLongDate);
                return string.Empty;
            }
        }

        public string msra_effective_start_year
        {
            get
            {
                if (msra_effective_start_date != DateTime.MinValue)
                    return msra_effective_start_date.ToString(busConstant.DateFormatYear);
                return string.Empty;
            }
        }

        public string dcra_effective_start_long_date
        {
            get
            {
                if (dcra_effective_start_date != DateTime.MinValue)
                    return dcra_effective_start_date.ToString(busConstant.DateFormatLongDate);
                return string.Empty;
            }
        }

        public string dcra_effective_start_year
        {
            get
            {
                if (dcra_effective_start_date != DateTime.MinValue)
                    return dcra_effective_start_date.ToString(busConstant.DateFormatYear);
                return string.Empty;
            }
        }

        // Inside Mail and Payment Options
        //  1. State and BND should see the selections
        //  2. Non PS Payroll, Higher Ed, and NULL should only see U.S. Mail and Check
        public string IsSelectionAllowed()
        {
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueState ||
                ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueBND)
                return busConstant.Flag_Yes;
            return busConstant.Flag_No;
        }

        // Inside Mail and Payment Options
        //  1. State and BND should see the selections
        //  2. Non PS Payroll, Higher Ed, and NULL should only see U.S. Mail and Check
        public string IsMailOptionSelectionAllowed()
        {
            if (IsSelectionAllowed() == busConstant.Flag_Yes ||
                ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.interoffice_mail_flag == busConstant.Flag_Yes)
                return busConstant.Flag_Yes;
            return busConstant.Flag_No;
        }

        //set visibility to direct deposit      
        public void SetVisbilityToDirectDeposit()
        {
            iblnIsDirectDepositVisible = false;
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value != null)
            {
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value.Equals(busConstant.PeopleSoftOrgGroupValueBND)
                    || ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value.Equals(busConstant.PeopleSoftOrgGroupValueState)
                    || ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.peoplesoft_org_group_value.Equals(busConstant.PeopleSoftOrgGroupValueHigherEd))
                    iblnIsDirectDepositVisible = true;
            }
        }

        //set visibility to direct deposit       
        public void SetVisbilityToInsideMail()
        {
            iblnIsInsideMailVisible = false;
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.interoffice_mail_flag == busConstant.Flag_Yes)
                iblnIsInsideMailVisible = true;
        }

        //load options
        public void LoadMssFlexCompOption()
        {
            if (icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
            {
                if (icdoMSSFlexCompOption.IsNull())
                    icdoMSSFlexCompOption = new cdoWssPersonAccountFlexCompOption();

                DataTable ldtbList = Select<cdoWssPersonAccountFlexCompOption>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                   new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    icdoMSSFlexCompOption.LoadData(ldtbList.Rows[0]);
                }
            }
        }

        //load conversion
        public void LoadMSSFlexCompConversion()
        {
            if (icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
            {
                if (iclbMSSFlexCompConversion.IsNull())
                    iclbMSSFlexCompConversion = new utlCollection<busWssPersonAccountFlexCompConversion>();
                busBase lbusBase = new busBase();
                DataTable ldtMSSFlexCompConv = Select("entWssPersonAccountFlexCompConversion.LoadPersonAccountFlexCompConversion", new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id });
                iclbMSSFlexCompConversion = lbusBase.GetCollection<busWssPersonAccountFlexCompConversion>(ldtMSSFlexCompConv, "icdoWssPersonAccountFlexCompConversion");
                //new string[1] { "wss_person_account_enrollment_request_id" },
                //new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);

                foreach (busWssPersonAccountFlexCompConversion lcdoConverison in iclbMSSFlexCompConversion)
                {
                    busWssPersonAccountFlexCompConversion lobjConversion = new busWssPersonAccountFlexCompConversion
                    {
                        icdoWssPersonAccountFlexCompConversion = new cdoWssPersonAccountFlexCompConversion()
                    };
                    lobjConversion.icdoWssPersonAccountFlexCompConversion = lcdoConverison.icdoWssPersonAccountFlexCompConversion;
                    lobjConversion.icdoWssPersonAccountFlexCompConversion.istrIsSelected = busConstant.Flag_Yes;
                    lobjConversion.LoadOrganization();
                    lobjConversion.icdoWssPersonAccountFlexCompConversion.istrOrgName = lobjConversion.ibusOrganization.icdoOrganization.org_name;
                }
            }
        }

        //load flex comp   
        public void LoadMSSFlexComp()
        {
            if (icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
            {
                if (icdoMSSFlexComp.IsNull())
                    icdoMSSFlexComp = new cdoWssPersonAccountFlexComp();

                DataTable ldtbList = Select<cdoWssPersonAccountFlexComp>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                      new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
                if (ldtbList.Rows.Count > 0)
                    icdoMSSFlexComp.LoadData(ldtbList.Rows[0]);
            }
        }

        public void LoadMSSActiveProviders()
        {
            if (ibusPerson.IsNull())
                LoadPersonAccount();

            if (iclbMSSFlexCompConversion == null)
                iclbMSSFlexCompConversion = new utlCollection<busWssPersonAccountFlexCompConversion>();

            if (ibusPerson.icolPersonEmployment.IsNull())
                ibusPerson.LoadPersonEmployment();

            var lenumEmploymentList = ibusPerson.icolPersonEmployment.Where(lobjPersonEmployment => lobjPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue);

            foreach (busPersonEmployment lobjPersonEmployment in lenumEmploymentList)
            {
                if (lobjPersonEmployment.ibusOrganization.IsNull())
                    lobjPersonEmployment.LoadOrganization();

                if (lobjPersonEmployment.ibusOrganization.icdoOrganization.status_value == busConstant.StatusActive)
                {
                    if (lobjPersonEmployment.ibusOrganization.iclbOrgPlan.IsNull())
                        lobjPersonEmployment.ibusOrganization.LoadOrgPlan();

                    var lenumFlexPlanList = lobjPersonEmployment.ibusOrganization.iclbOrgPlan.Where(lobjOP => lobjOP.icdoOrgPlan.plan_id == busConstant.PlanIdFlex
                        && busGlobalFunctions.CheckDateOverlapping(DateTime.Now, lobjOP.icdoOrgPlan.participation_start_date, lobjOP.icdoOrgPlan.participation_end_date));

                    foreach (busOrgPlan lobjOrgPlan in lenumFlexPlanList)
                    {
                        if (lobjOrgPlan.iclbOrgPlanProvider.IsNull())
                            lobjOrgPlan.LoadOrgPlanProviders();

                        var lenumProviderList = lobjOrgPlan.iclbOrgPlanProvider.Where(lobjProvider => lobjProvider.icdoOrgPlanProvider.status_value == busConstant.StatusActive);
                        foreach (busOrgPlanProvider lobjProvider in lenumProviderList)
                        {
                            if (lobjProvider.ibusProviderOrg.IsNull())
                                lobjProvider.LoadProviderOrg();

                            if (lobjProvider.ibusProviderOrg.icdoOrganization.status_value == busConstant.StatusActive)
                            {
                                if (lobjProvider.ibusProviderOrg.iclbOrgPlan.IsNull())
                                    lobjProvider.ibusProviderOrg.LoadOrgPlan();

                                var lenumOrgPlanList = lobjProvider.ibusProviderOrg.iclbOrgPlan.Where(lobjOP => lobjOP.icdoOrgPlan.plan_id == busConstant.PlanIdFlex
                                                       && busGlobalFunctions.CheckDateOverlapping(DateTime.Now, lobjOP.icdoOrgPlan.participation_start_date, lobjOP.icdoOrgPlan.participation_end_date));
                                if (lenumOrgPlanList.Count() > 0)
                                {
                                    var lenumProvdrList = iclbMSSFlexCompConversion.Where(lobjExistingProvider => lobjExistingProvider.icdoWssPersonAccountFlexCompConversion.org_id == lobjProvider.icdoOrgPlanProvider.provider_org_id);
                                    if (lenumProvdrList.Count() == 0)
                                    {
                                        lobjProvider.ibusWssPersonAccountFlexCompConversion = new busWssPersonAccountFlexCompConversion { icdoWssPersonAccountFlexCompConversion = new cdoWssPersonAccountFlexCompConversion() };
                                        lobjProvider.ibusWssPersonAccountFlexCompConversion.icdoWssPersonAccountFlexCompConversion.org_id = lobjProvider.ibusProviderOrg.icdoOrganization.org_id;
                                        lobjProvider.ibusWssPersonAccountFlexCompConversion.icdoWssPersonAccountFlexCompConversion.person_account_id = this.ibusPersonAccount.icdoPersonAccount.person_account_id;
                                        lobjProvider.ibusWssPersonAccountFlexCompConversion.LoadProvider();
                                        lobjProvider.ibusWssPersonAccountFlexCompConversion.LoadPersonAccount();

                                        if (lobjProvider.ibusWssPersonAccountFlexCompConversion.IsValidProvider())
                                        {
                                            if (!lobjProvider.ibusWssPersonAccountFlexCompConversion.iblnHaveVisionDentalOrLifePlan)
                                            {
                                                lobjProvider.ibusWssPersonAccountFlexCompConversion.icdoWssPersonAccountFlexCompConversion.istrOrgName = lobjProvider.ibusProviderOrg.icdoOrganization.org_name;
                                                iclbMSSFlexCompConversion.Add(lobjProvider.ibusWssPersonAccountFlexCompConversion);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool IsWaiverCheckedAndConversionEntered()
        {
            if (icdoMSSFlexComp.premium_conversion_waiver_flag == busConstant.Flag_Yes)
            {
                if (iclbMSSFlexCompConversion.Where(lobjConversion => lobjConversion.icdoWssPersonAccountFlexCompConversion.istrIsSelected == busConstant.Flag_Yes && lobjConversion.icdoWssPersonAccountFlexCompConversion.effective_start_date != DateTime.MinValue).Count() > 0)
                    return true;
            }
            return false;
        }

        public bool IsStartDateNotEnteredForDependent()
        {
            var lenumDepedent = iclbMSSPersonDependent.Where(lobjDep => lobjDep.icdoWssPersonDependent.date_of_birth != DateTime.MinValue
                || (!String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.first_name))
                || (!String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.last_name))
                 || (!String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.middle_name))
                 || (!String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.gender_value))
                 || (!String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.full_time_student_flag))
                 || (!String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.marital_status_value))
                 || (!String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.relationship_value))
                 || (!String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.marital_status_value)));
            if (lenumDepedent.Count() > 0)
            {
                foreach (busWssPersonDependent lobjPersonDep in lenumDepedent)
                {
                    if (lobjPersonDep.icdoWssPersonDependent.effective_start_date == DateTime.MinValue)
                        return true;
                }
            }
            return false;
        }

        public bool IsDataNotEnteredForDependent()
        {
            var lenumDepedent = iclbMSSPersonDependent.Where(lobjDep => lobjDep.icdoWssPersonDependent.date_of_birth == DateTime.MinValue
                && (String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.first_name))
                && (String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.last_name))
                 && (String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.middle_name))
                 && (String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.gender_value))
                 && (String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.full_time_student_flag))
                 && (String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.marital_status_value))
                 && (String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.relationship_value))
                 && (String.IsNullOrEmpty(lobjDep.icdoWssPersonDependent.marital_status_value)));

            if ((!IsCoverageSingle()) && (lenumDepedent.Count() > 0 || iclbMSSPersonDependent.Count == 0))
            {
                return false;
            }
            return true;
        }

        //for GHDV other coverage policy start date is mandatory
        public bool IsStartDateNotEnteredForOtherCoverage()
        {
            var lenumOtherCoverage = iclbMSSOtherCoverageDetail.Where(lobjOtherCov => lobjOtherCov.icdoWssPersonAccountOtherCoverageDetail.date_of_birth != DateTime.MinValue
                || (!String.IsNullOrEmpty(lobjOtherCov.icdoWssPersonAccountOtherCoverageDetail.covered_person))
                || (!String.IsNullOrEmpty(lobjOtherCov.icdoWssPersonAccountOtherCoverageDetail.other_coverage_number))
                 || (lobjOtherCov.icdoWssPersonAccountOtherCoverageDetail.policy_end_date != DateTime.MinValue)
                 || (!String.IsNullOrEmpty(lobjOtherCov.icdoWssPersonAccountOtherCoverageDetail.provider_org_name)));
            if (lenumOtherCoverage.Count() > 0)
            {
                foreach (busWssPersonAccountOtherCoverageDetail lobjOtherCoverage in lenumOtherCoverage)
                {
                    if (lobjOtherCoverage.icdoWssPersonAccountOtherCoverageDetail.policy_start_date == DateTime.MinValue)
                        return true;
                }
            }
            return false;
        }

        public string istrAcknowledgementFlag { get; set; }

        //check if all the acknowledgements are selected for GHDV
        //    public bool IsAllAcknowledgementAreNotSelected() => istrAcknowledgementFlag != busConstant.Flag_Yes;
        public bool IsAllAcknowledgementAreNotSelected()
        {
            if (istrAcknowledgementFlag == busConstant.Flag_No || string.IsNullOrEmpty(istrAcknowledgementFlag))
                return true;
            return false;
        }

        //check if all the acknowledgements are selected for Life
        public bool IsAllAcknowledgementAreNotSelectedForLife()
        {
            if (icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag == busConstant.Flag_No)
                return true;
            return false;
        }

        //check if all the acknowledgements are selected for Life
        public bool IsAllAcknowledgementAreNotSelectedForDB()
        {
            if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)   //PIR 25920  New DC plan
            {
                DataTable ldtbAckCheckDetails = new DataTable();
                if(ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB)
                    ldtbAckCheckDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.PlanRetirementTypeValueDB });//PIR 6961
                if(ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)
                    ldtbAckCheckDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.TempAdditionalContributionElectionAck });//PIR 26928 
                var ldtbResult = ldtbAckCheckDetails.FilterTable(busConstant.DataType.String, "SHOW_CHECK_BOX_FLAG", busConstant.Flag_Yes);
                if (iclbEEAcknowledgement.Count == ldtbResult.Count())
                {
                    if (iclbEEAcknowledgement.Where(o => o.icdoWssAcknowledgement.is_acknowledgement_selected != busConstant.Flag_Yes).Any())
                        return false;
                    return true;
                }
                //if (iclcEeAcknowledgement.Count == ldtbResult.Count())
                //{
                //    return !iclcEeAcknowledgement.Any(i => i.ienuObjectState != ObjectState.CheckListInsert);
                //}
            }
            else if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDC)
            {
                DataTable ldtbAckCheckDetails = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.PlanRetirementTypeValueDC });//PIR 6961
                var ldtbResult = ldtbAckCheckDetails.FilterTable(busConstant.DataType.String, "SHOW_CHECK_BOX_FLAG", busConstant.Flag_Yes);
                if (iclcEeAcknowledgement.Count == ldtbResult.Count())
                {
                    return !iclcEeAcknowledgement.Any(i => i.ienuObjectState != ObjectState.CheckListInsert);
                }
            }
            return false;
        }


        //load Acknowledgements in update mode in request page
        public void LoadGHDVAcknowledgements()
        {
            busBase lbus = new busBase();
            iclbGHDVAcknowledgement = new utlCollection<busWssAcknowledgement>();

            //DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(1022, "GHDV", null, null);
            DataTable ldtbList = Select("cdoWssAcknowledgement.SelectAck", new object[2] { DateTime.Now, busConstant.GHDVAgreementCheckBox });//PIR 6961
            iclbGHDVAcknowledgement = lbus.GetCollection<busWssAcknowledgement>(ldtbList);
        }

        #endregion

        public override void ValidateGroupRules(string astrGroupName, utlPageMode aenmPageMode)
        {
            if (astrGroupName == "GHDVEnrollment")
                LoadMSSLevelOfCoverageByPlan();
            if (astrGroupName == "FlexEnrollment")
            {
                if (icdoMSSFlexCompOption.medical_annual_pledge_amount > 0)
                {
                    ibusMSSPersonAccountFlexComp.ibusMSRACoverage = new busPersonAccountFlexCompOption { icdoPersonAccountFlexCompOption = new cdoPersonAccountFlexCompOption() };
                    ibusMSSPersonAccountFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.annual_pledge_amount = icdoMSSFlexCompOption.medical_annual_pledge_amount;
                }
            }
            base.ValidateGroupRules(astrGroupName, aenmPageMode);
            if (astrGroupName == "GHDVAcknowledgement" && ibusPersonEmploymentDetail.IsNotNull() && ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.IsNotNull() && ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonEmplyDetailType)
            {
                if (istrWaiveReason.IsNullOrEmpty() && icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive)
                {
                    utlError lerr = new utlError();
                    lerr.istrErrorMessage = "Please select at least one ‘Applicable coverage’ check box.";
                    iarrErrors.Add(lerr);
                }
            }
            if (astrGroupName == "FlexEnrollment" && icdoWssPersonAccountEnrollmentRequest.date_of_change != DateTime.MinValue)
            {
                decimal ldecMedAnnualAmount = IsMedicareAmountExceedTheLimit();
                if (ldecMedAnnualAmount > 0 && icdoMSSFlexCompOption.medical_annual_pledge_amount > ldecMedAnnualAmount)
                {
                    utlError lerr = new utlError();
                    lerr.istrErrorMessage = "Total Annual Medical Spending pledge amount cannot exceed $" + Convert.ToString(ldecMedAnnualAmount);
                    iarrErrors.Add(lerr);
                }
                decimal ldecDependentAnnualAmount = IsDependentAmountExceedTheLimit();
                if (ldecDependentAnnualAmount > 0 && icdoMSSFlexCompOption.dependent_annual_pledge_amount > ldecDependentAnnualAmount)
                {
                    utlError lerr = new utlError();
                    lerr.istrErrorMessage = "Total Annual Dependent care pledge amount cannot exceed $" + Convert.ToString(ldecDependentAnnualAmount);
                    iarrErrors.Add(lerr);
                }
            }
            if (astrGroupName == "LifeSupplemental")
            {
                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount > idecSupplementalLimit)
                {
                    utlError lerr = new utlError();
                    lerr.istrErrorMessage = "Supplement amount must not exceed " + idecSupplementalLimit;
                    iarrErrors.Add(lerr);
                }
                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount < idecMinSupplementalLimit)
                {
                    utlError lerr = new utlError();
                    lerr.istrErrorMessage = "Supplemental amount must be above " + idecMinSupplementalLimit;
                    iarrErrors.Add(lerr);
                }

            }
            foreach (utlError lobjError in iarrErrors)
                lobjError.istrErrorID = string.Empty;

            //Dependent grid was shrinking if error was thrown on the dependents step.
            if (iarrErrors.Count > 0 && astrGroupName == "GHDVDependent")
            {
                GetNextStepDependents();
            }
        }

        // PIR 12332 - Retroactive cancellations not allowed
        public bool IsDateOfChangePastDate()
        {
            DataTable ldtbEnrollmentRequest = busBase.Select("cdoWssPersonAccountEnrollmentRequest.GetEnrollmentRequestDateOfChange", new object[2] { icdoWssPersonAccountEnrollmentRequest.person_id, icdoWssPersonAccountEnrollmentRequest.plan_id });
            DateTime ldtDateofchange = new DateTime();
            if (ldtbEnrollmentRequest.Rows.Count > 0)
                ldtDateofchange = Convert.ToDateTime(ldtbEnrollmentRequest.Rows[0].Field<DateTime>("START_DATE"));
            else
                ldtDateofchange = DateTime.MinValue;
            if (icdoWssPersonAccountEnrollmentRequest.date_of_change < ldtDateofchange
                    || icdoWssPersonAccountEnrollmentRequest.date_of_change < DateTime.Today) // PIR 13493 - past date should not be allowed
                return true;
            else
                return false;
        }
        public bool IsAddlEEContributionPercentAvailable
        {
            get
            {
                if (ibusPerson.IsNull()) LoadPerson();
                if (ibusPersonAccount.IsNull()) LoadPersonAccount();
                return (IsTemporaryEmployee ? ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp : 
                    ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent) >= 1 ? true : false;
            }
        }

        public override void BeforeWizardStepValidate(utlPageMode aenmPageMode, string astrWizardName, string astrWizardStepName, utlWizardNavigationEventArgs we = null)
        {
            if (astrWizardStepName == "wzsMedDepCareReimbursement")
            {
                SetDateEventValidation();
            }
            //PIR 25920 DC 2025 changes
            if (astrWizardStepName == "wzsAddContryIntro")
            {
                if (ibusPersonAccount.IsNull()) LoadPersonAccount();
                int lintPercent = 0;
                if (IsTemporaryEmployee)
                    lintPercent = ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp;
                else
                    lintPercent = ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent;
                if (iintAddlEEContributionPercent != lintPercent)
                    iintAddlEEContributionPercent = Convert.ToInt32(lintPercent);
                EvaluateInitialLoadRules();
            }
            if (astrWizardStepName == "wzsAddContryStep2")
            {
                EvaluateInitialLoadRules();
            }
            if (astrWizardStepName == "wzsStepHDHP")
            {
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth &&
                    icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment &&
                    icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP &&
                    istrPreTaxHSA == busConstant.Flag_Yes)
                {
                    if (ibusPersonAccount.IsNull())
                        LoadPersonAccount();
                    if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0 && ibusMSSPersonAccountGHDV.IsNull())
                        LoadPersonAccountGHDV();

                    //PIR 25431 Member change to HDHP, then eff date should be 2/1 not 1/1
                    //Was working for the first time member enrolled in HDHP but 2nd time onwards setting 1/1 again
                    busPersonAccountGhdvHistory lobjPAGhdvHistoryToCheck = new busPersonAccountGhdvHistory { icdoPersonAccountGhdvHistory = new cdoPersonAccountGhdvHistory() };
                    if (ibusMSSPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNullOrEmpty())
                        ibusMSSPersonAccountGHDV.LoadPersonAccountGHDVHistory();

                    if(ibusMSSPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNotNull() && ibusMSSPersonAccountGHDV.iclbPersonAccountGHDVHistory.Count > 0)
                    {
                        lobjPAGhdvHistoryToCheck = ibusMSSPersonAccountGHDV.iclbPersonAccountGHDVHistory.Where(i => i.icdoPersonAccountGhdvHistory.start_date < icdoWssPersonAccountEnrollmentRequest.date_of_change).FirstOrDefault();
                    }
                    if (lobjPAGhdvHistoryToCheck.IsNotNull() && ibusPersonAccount.icdoPersonAccount.person_account_id > 0 &&
                        lobjPAGhdvHistoryToCheck.icdoPersonAccountGhdvHistory.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP)
                    {
                        icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date = AnnualEnrollmentEffectiveDate;
                    }
                    else
                        icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date = AnnualEnrollmentEffectiveDate.AddMonths(1);
                }
            }
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental)
            {
                icdoWssPersonAccountEnrollmentRequest.reason_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(332, icdoWssPersonAccountEnrollmentRequest.reason_value);
                icdoMSSGDHV.level_of_coverage_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, icdoMSSGDHV.level_of_coverage_value);

                if (astrWizardStepName == "wzsIntro")
                {
                    //pir 6979 - If Waive is selected , the flag must be automatically checked on load
                    if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == "WAIV")
                        icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag = busConstant.Flag_Yes;
                    else
                        icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag = busConstant.Flag_No;
                    if (icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id != 0)
                        icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Update;
                    else
                        icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Insert;

                    //PIR 10695 "Save & Quit" functionality
                    if ((icdoMSSGDHV.wss_person_account_ghdv_id != 0) && (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment))
                        LoadMSSGHDV();
                    if (icdoMSSGDHV.wss_person_account_ghdv_id != 0)
                        icdoMSSGDHV.ienuObjectState = ObjectState.Update;
                    else
                        icdoMSSGDHV.ienuObjectState = ObjectState.Insert;

                    iarrChangeLog.Add(icdoWssPersonAccountEnrollmentRequest);
                    iarrChangeLog.Add(icdoMSSGDHV);


                    if (istrCancelEnrollmentFlag == busConstant.Flag_Yes)
                    {
                        icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value = busConstant.PlanEnrollmentOptionValueCancel;
                        icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.AnnualEnrollment;
                        foreach (busWssPersonDependent lobjPersonDependent in iclbMSSPersonDependent)
                        {
                            DateTime ldteStartDate = icdoWssPersonAccountEnrollmentRequest.date_of_change;
                            if (icdoWssPersonAccountEnrollmentRequest.date_of_change.Day != 1)
                                ldteStartDate = icdoWssPersonAccountEnrollmentRequest.date_of_change.GetFirstDayofNextMonth();
                            lobjPersonDependent.icdoWssPersonDependent.effective_end_date = ldteStartDate.AddDays(-1);
                            //PIR 18017 - When member is enrolled in plan and cancels plan, the plan is suspending but the Dependents are not end dating
                            lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value = busConstant.PlanOptionStatusValueWaived;
                            lobjPersonDependent.icdoWssPersonDependent.PopulateDescriptions();
                            // PIR 10402
                            if ((lobjPersonDependent.icdoWssPersonDependent.effective_start_date == icdoWssPersonAccountEnrollmentRequest.date_of_change)
                                && (lobjPersonDependent.icdoWssPersonDependent.effective_end_date != DateTime.MinValue))
                            {
                                lobjPersonDependent.icdoWssPersonDependent.effective_end_date = lobjPersonDependent.icdoWssPersonDependent.effective_start_date;
                            }
                        }
                        // PIR 10402 - New request should be created
                        icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id = 0;
                        icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Insert;
                    }

                    //PIR 19647 and PIR 19698
                    if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth)
                    {
                        LoadPersonEmploymentDetail();
                        ibusPersonEmploymentDetail.LoadPersonEmployment();
                        ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                    }

                    SetEnrollmentValues();
                    iclbMSSPersonDependentPrevious = iclbMSSPersonDependent;
                }
                if (astrWizardStepName == "wzsStep2")
                {
                    LoadCoverageCodeDescription();
                    istrCoverageCodeDescription = ibusMSSPersonAccountGHDV.istrCoverageCode;
                    istrPreTaxHSADisplay = istrPreTaxHSA == busConstant.Flag_Yes ? busConstant.Flag_Yes_Value: busConstant.Flag_No_Value;
                    if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueCancel)
                    {
                        foreach (busWssPersonDependent lobjPersonDependent in iclbMSSPersonDependent)
                        {
                            DateTime ldteStartDate = icdoWssPersonAccountEnrollmentRequest.date_of_change;
                            if (icdoWssPersonAccountEnrollmentRequest.date_of_change.Day != 1)
                                ldteStartDate = icdoWssPersonAccountEnrollmentRequest.date_of_change.GetFirstDayofNextMonth();
                            if (icdoWssPersonAccountEnrollmentRequest.date_of_change != DateTime.MinValue)
                                lobjPersonDependent.icdoWssPersonDependent.effective_end_date = ldteStartDate.AddDays(-1);
                        }
                    }

                    // PIR 9923 -- End all dependents if the coverage is for Single.
                    DateTime ldteDateofChange = GetDateofChange();
                    foreach (busWssPersonDependent lobjPersonDependent in iclbMSSPersonDependent)
                    {
                        if (lobjPersonDependent.icdoPersonDependent.IsNull())
                            lobjPersonDependent.LoadPersonDependentProperties(); //F/W Upgrae PIR
                        bool lblnEndDependentFlag = false;
                        if (IsCoverageSingle())
                            lblnEndDependentFlag = true;
                        if (icdoMSSGDHV.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualChild ||
                            icdoMSSGDHV.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualChild)
                        {
                            if (icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusFinishLater)
                            {
                                if (!lobjPersonDependent.IsChildDependentWSS())
                                    lblnEndDependentFlag = true;
                            }
                            else
                            {
                                if (!lobjPersonDependent.IsChildDependent())
                                    lblnEndDependentFlag = true;
                            }
                        }
                        if (icdoMSSGDHV.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividualSpouse ||
                            icdoMSSGDHV.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividualSpouse)
                        {
                            if (icdoWssPersonAccountEnrollmentRequest.status_value == busConstant.EnrollRequestStatusFinishLater)
                            {
                                if (lobjPersonDependent.icdoWssPersonDependent.relationship_value != busConstant.DependentRelationshipSpouse)
                                    lblnEndDependentFlag = true;
                            }
                            else
                            {
                                if (lobjPersonDependent.icdoPersonDependent.relationship_value != busConstant.DependentRelationshipSpouse)
                                    lblnEndDependentFlag = true;
                            }
                        }

                        // PIR 10671
                        if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueCancel)
                            lblnEndDependentFlag = true;

                        if (lblnEndDependentFlag)
                        {
							//PIR 26370
                            if ((icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                               icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision) &&
                               icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.AnnualEnrollment &&
                               lobjPersonDependent.icdoWssPersonDependent.effective_end_date == DateTime.MinValue)
                                iblnIsDependentEndDatedForDV = true;
                            lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value = busConstant.PlanOptionStatusValueWaived;
                            lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_description =
                                busGlobalFunctions.GetDescriptionByCodeValue(340, busConstant.PlanOptionStatusValueWaived, iobjPassInfo);
                            if (lobjPersonDependent.icdoWssPersonDependent.effective_start_date != DateTime.MinValue &&
                                //lobjPersonDependent.icdoWssPersonDependent.effective_start_date <= icdoWssPersonAccountEnrollmentRequest.date_of_change && //PIR 21634 - New Hire, opted for Family, and then changed to single, dependents were not end dated.
                                ldteDateofChange != DateTime.MinValue) // PIR 10392
                                lobjPersonDependent.icdoWssPersonDependent.effective_end_date = ldteDateofChange.AddDays(-1);
                            
                            // PIR 10372 
                            if ((lobjPersonDependent.icdoWssPersonDependent.effective_start_date == icdoWssPersonAccountEnrollmentRequest.date_of_change) && 
                                (lobjPersonDependent.icdoWssPersonDependent.effective_end_date < lobjPersonDependent.icdoWssPersonDependent.effective_start_date)
                                && (lobjPersonDependent.icdoWssPersonDependent.effective_end_date != DateTime.MinValue))
                            {
                                lobjPersonDependent.icdoWssPersonDependent.effective_end_date = lobjPersonDependent.icdoWssPersonDependent.effective_start_date;
                            }
                            //PIR 21634 - (lobjPersonDependent.icdoWssPersonDependent.effective_start_date == icdoWssPersonAccountEnrollmentRequest.date_of_change) &&
                            if ((lobjPersonDependent.icdoWssPersonDependent.effective_end_date < lobjPersonDependent.icdoWssPersonDependent.effective_start_date)
                                    && (lobjPersonDependent.icdoWssPersonDependent.effective_end_date != DateTime.MinValue))
                            {
                                lobjPersonDependent.icdoWssPersonDependent.effective_end_date = lobjPersonDependent.icdoWssPersonDependent.effective_start_date;
                            }
                        }
                        else
                        {
                            // If the user reverses/modifies the coverage, the end date should be revereted.
                            if (ldteDateofChange != DateTime.MinValue && lobjPersonDependent.icdoWssPersonDependent.effective_end_date == ldteDateofChange.AddDays(-1)
                                && lobjPersonDependent.icdoWssPersonDependent.wss_person_dependent_id == 0)
                            {
                                lobjPersonDependent.icdoWssPersonDependent.effective_end_date = DateTime.MinValue;
                                lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value = busConstant.PlanOptionStatusValueEnrolled;
                                lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_description =
                                    busGlobalFunctions.GetDescriptionByCodeValue(340, busConstant.PlanOptionStatusValueEnrolled, iobjPassInfo);
                            }
                        }
                        lobjPersonDependent.icdoWssPersonDependent.level_of_coverage_value = icdoMSSGDHV.level_of_coverage_value;

                        // PIR 11920 - issue looged on 10/09/2013
                        if (!IsDependentInsertRequired(lobjPersonDependent))
                            lobjPersonDependent.icdoWssPersonDependent.effective_end_date = lobjPersonDependent.icdoWssPersonDependent.effective_start_date = DateTime.MinValue;
                    }
                    SetDateEventValidation(); // PIR 10309
                    iclbMSSPersonDependentPrevious = iclbMSSPersonDependent; //PIR 13542
                    LoadAckMemAuthDetails(); //PIR 16533 & 17842

                    if (!IsCurrentEmployerProvidesFlexDuringSameCalenderYear(icdoWssPersonAccountEnrollmentRequest.date_of_change))
                    {
                        icdoMSSGDHV.pre_tax_payroll_deduction_flag = busConstant.Flag_No;
                    }
                }
                if (astrWizardStepName == "wzsStep4")
                {
                    iclbMSSPersonDependentPrevious = iclbMSSPersonDependent;
                    if (!iblnIsSaveAndNext)
                    {
                        iclbMSSPersonDependentTemp = new Collection<busWssPersonDependent>();
                        DateTime ldteDateofChange = GetDateofChange();
                        if (ibusPersonAccountEmploymentDetail.IsNull()) LoadPersonAccountEmploymentDetail();
                        foreach (busWssPersonDependent lobjPersonDependent in iclbMSSPersonDependent)
                        {
                            if (lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueEnrolled)
                            {
                                lobjPersonDependent.icdoWssPersonDependent.effective_end_date = DateTime.MinValue;
                                if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                                {
                                    //PIR 13646 -- dependent enrollment startdate needs to be manipulated depending on the change reason
                                    if (lobjPersonDependent.icdoWssPersonDependent.effective_start_date == DateTime.MinValue || icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonNewHire
                                        || (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment
                                        && ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_id == 0))

                                        lobjPersonDependent.icdoWssPersonDependent.effective_start_date = ldteDateofChange;
                                }
                                else
                                {
                                    if ((lobjPersonDependent.icdoWssPersonDependent.effective_start_date == DateTime.MinValue)
                                        || ((icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ReasonValueNewHire)
                                        && (icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ChangeReasonAnnualEnrollment) // PIR 12089 - Overwrite effective start date for other than NEWH and ANNE
                                        && (lobjPersonDependent.icdoWssPersonDependent.effective_start_date == ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.GetFirstDayofNextMonth())))
                                        lobjPersonDependent.icdoWssPersonDependent.effective_start_date = ldteDateofChange;
                                }
                            }
                            else if (lobjPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueWaived &&
                                lobjPersonDependent.icdoWssPersonDependent.effective_start_date != DateTime.MinValue)
                            {
                                if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                                {
                                    if (lobjPersonDependent.icdoWssPersonDependent.effective_start_date < icdoWssPersonAccountEnrollmentRequest.date_of_change)
                                        lobjPersonDependent.icdoWssPersonDependent.effective_end_date = ldteDateofChange.AddDays(-1);
                                    else if (lobjPersonDependent.icdoWssPersonDependent.effective_start_date == icdoWssPersonAccountEnrollmentRequest.date_of_change)
                                        lobjPersonDependent.icdoWssPersonDependent.effective_end_date = ldteDateofChange;
                                }
                                else
                                {
                                    //PIR 26799 When completing MSS wizards for enrollment changes the system should not allow past dates to be entered when ending Dependents.
                                    if (lobjPersonDependent.icdoWssPersonDependent.effective_start_date < DateTime.Today)
                                    {
                                        DateTime ldteChangeDate = DateTime.Now.GetFirstDayofNextMonth();
                                        lobjPersonDependent.icdoWssPersonDependent.effective_end_date = ldteChangeDate.AddDays(-1);
                                    }
                                    if (lobjPersonDependent.icdoWssPersonDependent.effective_start_date >= DateTime.Today)
                                    {
                                        lobjPersonDependent.icdoWssPersonDependent.effective_end_date = lobjPersonDependent.icdoWssPersonDependent.effective_start_date;
                                    }
                                }
                                // PIR 12089 - Dont populate dates if new enrollment and enrollment option is kept Waived
                                if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                                {
                                    lobjPersonDependent.icdoWssPersonDependent.effective_start_date = DateTime.MinValue;
                                    lobjPersonDependent.icdoWssPersonDependent.effective_end_date = DateTime.MinValue;
                                }
                            }

                            if (!string.IsNullOrEmpty(lobjPersonDependent.icdoWssPersonDependent.ssn))
                            {
                                lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId = busGlobalFunctions.GetPersonIDBySSN(lobjPersonDependent.icdoWssPersonDependent.ssn);
                            }

                            // PIR 11840
                            if (IsDependentInsertRequired(lobjPersonDependent))
                                iclbMSSPersonDependentTemp.Add(lobjPersonDependent);
                            // PIR 11920 - issue looged on 10/09/2013
                            else
                                lobjPersonDependent.icdoWssPersonDependent.effective_end_date = lobjPersonDependent.icdoWssPersonDependent.effective_start_date = DateTime.MinValue;
                        }

                        // PIR 11840
                        if (iclbMSSPersonDependentTemp.Count > 0)
                            iclbMSSPersonDependent = iclbMSSPersonDependentTemp;
                    }
                }

                if (astrWizardStepName == "wzsStep6")
                {
                    if (ibusPersonEmploymentDetail.IsNotNull() && ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.IsNotNull() && ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonEmplyDetailType)
                    {
                        icdoMSSGDHV.waive_reason = istrWaiveReason;
                        if (istrWaiveReason == busConstant.WaiveReasonOther)
                        {
                            icdoMSSGDHV.waive_reason_text = istrWaiveReasonOTHRText;
                        }
                        iarrChangeLog.Add(icdoMSSGDHV);
                    }
                    LoadAckMemAuthDetails(); //PIR 16533 & 17842
                    iclbWSSDependent = new Collection<busWssPersonDependent>();
                    foreach (busWssPersonDependent lobjPersonDependent in iclbMSSPersonDependent)
                        iclbWSSDependent.Add(lobjPersonDependent);
                    if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth && icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonACAAnnualEnrollment)
                        iblnFinishButtonClicked = true;
                }

                //PIR 10695 "Save & Quit" functionality
                if (astrWizardStepName == "wzsStep7")
                {
                    iblnFinishButtonClicked = true;
                }

                if (astrWizardStepName == "wzsStepMedicare")
                {
                    ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_a_effective_date = icdoMSSGDHV.medicare_part_a_effective_date;
                    ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_b_effective_date = icdoMSSGDHV.medicare_part_b_effective_date;
                }

                if (iclbMSSPersonDependent.IsNotNull())
                {
                    foreach (busWssPersonDependent lobjPersonDepedent in iclbMSSPersonDependent)
                    {
                        if (!string.IsNullOrEmpty(lobjPersonDepedent.icdoWssPersonDependent.last_name))
                            lobjPersonDepedent.icdoWssPersonDependent.istrDependentName = lobjPersonDepedent.icdoWssPersonDependent.first_name;
                        if (!string.IsNullOrEmpty(lobjPersonDepedent.icdoWssPersonDependent.last_name))
                            lobjPersonDepedent.icdoWssPersonDependent.istrDependentName += " " + lobjPersonDepedent.icdoWssPersonDependent.middle_name;
                        if (!string.IsNullOrEmpty(lobjPersonDepedent.icdoWssPersonDependent.last_name))
                            lobjPersonDepedent.icdoWssPersonDependent.istrDependentName += " " + lobjPersonDepedent.icdoWssPersonDependent.last_name;
                    }
                }
            }

            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife ||
                icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex)
            {
                //PIR-20048
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex && 
                    (astrWizardStepName == "wzsMedDepCareReimbursement" || astrWizardStepName == "wzsMedDepCare") && //PIR 24638
                    !iarrChangeLog.Contains(icdoMSSFlexCompOption))
                {
                    icdoMSSFlexCompOption.ienuObjectState = (icdoMSSFlexCompOption.wss_person_account_flex_comp_option_id != 0) ? ObjectState.Update : ObjectState.Insert;
                    iarrChangeLog.Add(icdoMSSFlexCompOption);
                }
                icdoWssPersonAccountEnrollmentRequest.reason_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(332, icdoWssPersonAccountEnrollmentRequest.reason_value);
                if (astrWizardStepName == "wzsIntro")
                {
                    SetEnrollmentValues();
                    if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == "WAIV")
                        icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag = busConstant.Flag_Yes;
                    else
                        icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag = busConstant.Flag_No;

                    if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                        icdoWssPersonAccountEnrollmentRequest.date_of_change = AnnualEnrollmentEffectiveDate;

                    SetReimbursementAmounts();//PIR 18347
                    if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
                        GetCoverageAmountDetailsForDisplay();
                }
                if (astrWizardStepName == "wzsStep2")
                {
                    if (IsFlexCompPlanNotAvailable() || IsTemporaryEmployee)
                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction = busConstant.Flag_No;

                    if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife && iarrChangeLog.OfType<cdoWssPersonAccountLifeOption>().Count() == 0)
                    {
                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.ienuObjectState = (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.wss_person_account_life_option_id != 0) ? ObjectState.Update : ObjectState.Insert;
                        iarrChangeLog.Add(ibusMSSLifeOption.icdoWssPersonAccountLifeOption);
                    }
                }
                if (astrWizardStepName == "wzsNonNDPERSPreTax") //PIR 12532
                {
				    //PIR 18138
                    //if (iclbMSSFlexCompConversionSelected.IsNull())  //PIR-19610
                        iclbMSSFlexCompConversionSelected = new Collection<busWssPersonAccountFlexCompConversion>();

                    if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                    {
                        foreach (busWssPersonAccountFlexCompConversion lobjFlexCompConv in iclbMSSFlexCompConversion)
                        {
                            if (lobjFlexCompConv.icdoWssPersonAccountFlexCompConversion.istrIsSelected == busConstant.Flag_Yes)
                            {
                                lobjFlexCompConv.icdoWssPersonAccountFlexCompConversion.effective_start_date = AnnualEnrollmentEffectiveDate;
                                lobjFlexCompConv.icdoWssPersonAccountFlexCompConversion.effective_end_date = AnnualEnrollmentEffectiveEndDate;
								iclbMSSFlexCompConversionSelected.Add(lobjFlexCompConv);
                            }
                            else    
                            {
                                lobjFlexCompConv.icdoWssPersonAccountFlexCompConversion.effective_start_date = DateTime.MinValue;   //PIR-19610
                                lobjFlexCompConv.icdoWssPersonAccountFlexCompConversion.effective_end_date = DateTime.MinValue;                                
                            }
                        }
                    }
                    else
                    {
                        foreach (busWssPersonAccountFlexCompConversion lobjFlexCompConv in iclbMSSFlexCompConversion)
                        {
                            if (lobjFlexCompConv.icdoWssPersonAccountFlexCompConversion.istrIsSelected == busConstant.Flag_Yes && lobjFlexCompConv.icdoWssPersonAccountFlexCompConversion.effective_start_date != DateTime.MinValue)
                            {
                                lobjFlexCompConv.icdoWssPersonAccountFlexCompConversion.effective_end_date = busGlobalFunctions.GetLastDayOfEffectiveYear(lobjFlexCompConv.icdoWssPersonAccountFlexCompConversion.effective_start_date);
								iclbMSSFlexCompConversionSelected.Add(lobjFlexCompConv);
                            }
                        }
                    }
                }
                if (astrWizardStepName == "wzsNonNDPERSPreTax") //PIR 26356
                {
                    foreach (busWssPersonAccountFlexCompConversion lobjFlexCompConv in iclbMSSFlexCompConversion)
                    {
                        if ((icdoMSSFlexCompOption.IsNotNull() &&
                            (icdoMSSFlexCompOption.dependent_annual_pledge_amount == 0 && icdoMSSFlexCompOption.medical_annual_pledge_amount == 0)) &&
                            (lobjFlexCompConv.icdoWssPersonAccountFlexCompConversion.istrIsSelected == busConstant.Flag_Yes ||
                            lobjFlexCompConv.icdoWssPersonAccountFlexCompConversion.effective_start_date != DateTime.MinValue))
                        {
                            iblnIsMemberEligibleToSelectPreTaxProvider = true;
                            break;
                        }
                    }
                }

                if (astrWizardStepName == "wzsAck" || astrWizardStepName == "wzsStep")
                        iblnFinishButtonClicked = true;
            }

            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
            {
                if (astrWizardStepName == "wzsIntro")
                {
                    LoadPersonAccount();
                    idecBasicCoverageAmount = GetCoverageAmountDetails(busConstant.LevelofCoverage_Basic);

                    ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount = idecBasicCoverageAmount;
                    if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0 )
                    {
                        if (ibusMSSLifeWeb.iclbLifeOption.IsNull())   //PIR-20537
                        {
                            ibusMSSLifeWeb.iclbLifeOption = new Collection<busPersonAccountLifeOption>();
                            ibusMSSLifeWeb.LoadLifeOption();
                        }

                        if (ibusMSSLifeWeb.iclbLifeOption.IsNotNull())
                        {
                            foreach (busPersonAccountLifeOption lobjOption in ibusMSSLifeWeb.iclbLifeOption)
                            {
                                if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental)
                                {
                                    idecSupplementalAmount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                                    istrSupplementalWaiverFlagFromLifeOption = busConstant.Flag_No;
                                }
                                if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                                {
                                    idecDependentSupplementalAmount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                                    istrDependentWaiverFlagFromLifeOption = busConstant.Flag_No;
                                }
                                if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                                {
                                    idecSpouseSupplementalAmount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                                    istrSpouseWaiverFlagFromLifeOption = busConstant.Flag_No;
                                }
                            }
                        }
                    }

                    if (icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id == 0)
                    {
                        icdoWssPersonAccountEnrollmentRequest.ienuObjectState = ObjectState.Insert;
                        iarrChangeLog.Add(icdoWssPersonAccountEnrollmentRequest);
                    }



                    if ((ibusMSSLifeOption.icdoWssPersonAccountLifeOption.wss_person_account_life_option_id != 0) && (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment))
                    {
                        LoadMSSLifeOptions();
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount != 0.00M)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount += ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                    }
                }
                // PIR 10370 START
                if (astrWizardStepName == "wzsStep1")
                {
                    GetCoverageAmountDetailsSupplemental(ref adecMinAmount, ref adecMaxAmount);
                    idecSupplementalLimit = adecMaxAmount + idecBasicCoverageAmount;
                    idecMinSupplementalLimit = adecMinAmount + idecBasicCoverageAmount;
                    istrWizardStepName = astrWizardStepName;
                    istrSupplementalWaiverFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag;
                    istrDependentWaiverFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag;

                    if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag.IsNullOrEmpty() && ibusPerson?.icdoPerson?.marital_status_value != busConstant.PersonMaritalStatusMarried)
                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                    istrSpouseWaiverFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag;

                    if ((ibusMSSLifeOption.icdoWssPersonAccountLifeOption.wss_person_account_life_option_id != 0) && (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment))
                    {
                        LoadMSSLifeOptions();
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag != istrSpouseWaiverFlag && istrSpouseWaiverFlag.IsNotNull() && istrSpouseWaiverFlag != busConstant.LifeInsuranceFlagValue)
                            iblnIsSpouseSupplementalWaiverFlagChanged = true;
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag != istrSupplementalWaiverFlag && istrSupplementalWaiverFlag != busConstant.LifeInsuranceFlagValue)
                            iblnIsSupplementalWaiverFlagChanged = true;
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag != istrDependentWaiverFlag && istrDependentWaiverFlag.IsNotNull() && istrDependentWaiverFlag != busConstant.LifeInsuranceFlagValue)
                            iblnIsDepSupplementalWaiverFlagChanged = true;
                        //Supplemental amount should not be updated if amount is 0 or No is selected in update mode.
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount != 0.00M && istrSupplementalWaiverFlag == busConstant.Flag_No)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount += ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                        if (iblnIsSpouseSupplementalWaiverFlagChanged)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = istrSpouseWaiverFlag;
                        if (iblnIsSupplementalWaiverFlagChanged)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag = istrSupplementalWaiverFlag;
                        if (iblnIsDepSupplementalWaiverFlagChanged)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = istrDependentWaiverFlag;
                    }


                    if ((ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag == busConstant.Flag_No)
                        && (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                        && (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount == 0M))
                    {
                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount = idecSupplementalAmount;
                    }

                    if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                    {
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag == busConstant.Flag_Yes)
                        {
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount = 0M;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = busConstant.Flag_Yes;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value = null;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction = busConstant.Flag_No; // PIR 10978
                        }
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.Flag_Yes)
                        {
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value = null;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;
                        }

                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_Yes)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;

                        //In case of update (if enrollment is created for this year), if No is selected then don't change the value of the flag.
                        if (icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
                        {
                            if (istrSupplementalWaiverFlag == busConstant.LifeInsuranceFlagValue || istrDependentWaiverFlag == busConstant.LifeInsuranceFlagValue || istrSpouseWaiverFlag == busConstant.LifeInsuranceFlagValue)
                            {
                                LoadMSSLifeOptions();
                                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount != 0.00M)
                                    ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount += ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                            }
                            if (istrDependentWaiverFlag == busConstant.Flag_Yes)
                            {
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = busConstant.Flag_Yes;
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value = null;
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;
                            }

                            if (istrSpouseWaiverFlag == busConstant.Flag_Yes)
                            {
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;
                            }
                        }
                        //In case of new (no request is created earlier)
                        else if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0 && icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id == 0) // In case of New enrollment if No is selected then set the Waiver to Y. 
                        {
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag == busConstant.LifeInsuranceFlagValue)
                            {
                                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.Flag_Yes ||
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.Flag_No)
                                {
                                    iblnIsSupplementalSelected = true;
                                }
                                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                {
                                    if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_Yes ||
                                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_No)
                                    {
                                        iblnIsSupplementalSelected = true;
                                        iblnIsDependentSelected = true;
                                    }
                                    else
                                    {
                                        iblnIsSupplementalSelected = false;
                                        iblnIsDependentSelected = false;
                                    }
                                }
                            }
                            else
                            {
                                iblnIsSupplementalSelected = false;
                                iblnIsDependentSelected = false;
                            }

                            if (!iblnIsSupplementalSelected && !iblnIsDependentSelected)
                            {
                                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                    ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag = busConstant.Flag_Yes;
                                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                    ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = busConstant.Flag_Yes;
                                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                    ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                            }
                        }
                        //In case of members whose annual enrollment is not of current year and for members who are enrolled in Life but request was not created.
                        else
                        {
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag = istrSupplementalWaiverFlagFromLifeOption;
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = istrDependentWaiverFlagFromLifeOption;
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = istrSpouseWaiverFlagFromLifeOption;


                        }
                    }
                    else //Regular Enrollment
                    {
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag == busConstant.Flag_Yes)
                        {
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount = 0M;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = busConstant.Flag_Yes;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value = string.Empty;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction = busConstant.Flag_No; // PIR 10978
                        }
                    }

                    SetDateEventValidation(); // PIR 10309
                }
                if (astrWizardStepName == "wzsStep2")
                {
                    istrWizardStepName = astrWizardStepName;
                    istrDependentWaiverFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag;
                    istrSpouseWaiverFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag;

                    if ((ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.Flag_No)
                        && (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                        && (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value.IsNullOrEmpty()))
                    {
                        if (idecDependentSupplementalAmount == 0M)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value = null;
                        else
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value = idecDependentSupplementalAmount.ToString();
                    }
                    if ((ibusMSSLifeOption.icdoWssPersonAccountLifeOption.wss_person_account_life_option_id != 0) && (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment))
                    {
                        idecNewSupplementalAmount = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount;
                        //istrDepSupplementalWaiverFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag;
                        //istrSpouseSupplementalWaiverFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag;
                        istrPreTaxFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction;
                        LoadMSSLifeOptions();
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag != istrSpouseWaiverFlag && istrSpouseWaiverFlag.IsNotNull())
                            iblnIsSpouseSupplementalWaiverFlagChanged = true;
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction != istrPreTaxFlag)
                            iblnIsPreTaxFlagChanged = true;
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag != istrDependentWaiverFlag)
                            iblnIsDepSupplementalWaiverFlagChanged = true;
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount != idecNewSupplementalAmount)
                            iblnIsSupplementalAmountChanged = true;

                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount != 0.00M && istrSupplementalWaiverFlag == busConstant.Flag_No)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount += ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;

                        if (iblnIsSpouseSupplementalWaiverFlagChanged)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = istrSpouseWaiverFlag;
                        if (iblnIsPreTaxFlagChanged)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction = istrPreTaxFlag;
                        if (iblnIsSupplementalAmountChanged)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount = idecNewSupplementalAmount;
                        if (iblnIsDepSupplementalWaiverFlagChanged)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = istrDependentWaiverFlag;
                        if (iblnIsSupplementalWaiverFlagChanged)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag = istrSupplementalWaiverFlag;
                    }

                    if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                    {
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.Flag_Yes)
                        {
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value = null;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;
                        }
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_Yes)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;

                        if (icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
                        {
                            if (istrDependentWaiverFlag == busConstant.LifeInsuranceFlagValue || istrSpouseWaiverFlag == busConstant.LifeInsuranceFlagValue)
                            {
                                if (iblnIsSupplementalAmountChanged)
                                {
                                    LoadMSSLifeOptions();
                                    ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount = idecNewSupplementalAmount;
                                }
                                else
                                {
                                    LoadMSSLifeOptions();
                                    if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.wss_person_account_life_option_id > 0)
                                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount += ibusMSSLifeOption.icdoWssPersonAccountLifeOption.coverage_amount;
                                }
                            }
                            //If Dependent is not updated but Spouse is made Waive.
                            if (istrSpouseWaiverFlag == busConstant.Flag_Yes)
                            {
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;
                            }
                        }
                        else if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0 && icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id == 0)
                        {
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.LifeInsuranceFlagValue)
                            {
                                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_No ||
                                    ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_Yes)
                                {
                                    iblnIsDependentSelected = true;
                                }
                                else
                                    iblnIsDependentSelected = false;
                            }
                            else
                                iblnIsDependentSelected = false;

                            if (!iblnIsDependentSelected)
                            {
                                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                    ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = busConstant.Flag_Yes;
                                if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                    ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                            }
                        }
                        else
                        {
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag = istrSupplementalWaiverFlagFromLifeOption;
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = istrDependentWaiverFlagFromLifeOption;
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = istrSpouseWaiverFlagFromLifeOption;
                        }
                    }
                    else //Regular Enrollment
                    {
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.Flag_Yes)
                        {
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value = string.Empty;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;
                        }
                    }

                }
                if (astrWizardStepName == "wzsStep3")
                {

                    idecSpouseSupplementalLimit = GetCoverageAmountDetails(busConstant.LevelofCoverage_SpouseSupplemental);

                    if ((ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_No)
                        && (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                        && (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount == 0M))
                    {
                        ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = idecSpouseSupplementalAmount;
                    }


                    if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                    {
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_Yes)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;

                        istrSpouseWaiverFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag;

                        if (icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id > 0)
                        {
                            if (istrSpouseWaiverFlag == busConstant.LifeInsuranceFlagValue)
                            {
                                istrSupplementalWaiverFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag;
                                idecNewSupplementalAmount = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount;
                                istrPreTaxFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction;
                                istrDependentWaiverFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag;
                                istrNewDependentSupplementalAmount = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value;
                                LoadMSSLifeOptions();
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount = idecNewSupplementalAmount;
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value = istrNewDependentSupplementalAmount;
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = istrDependentWaiverFlag;
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction = istrPreTaxFlag;
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag = istrSupplementalWaiverFlag;
                            }
                        }
                        else if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0 && icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id == 0)
                        {
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_Yes;
                        }
                        else
                        {
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag = istrSupplementalWaiverFlagFromLifeOption;
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = istrDependentWaiverFlagFromLifeOption;
                            if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.LifeInsuranceFlagValue)
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = istrSpouseWaiverFlagFromLifeOption;
                        }
                    }
                    else //Regular Enrollment
                    {
                        if (ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag == busConstant.Flag_Yes)
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = 0M;

                        //PIR 17024
                        istrSpouseWaiverFlag = ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag;
                    }
                }
                // PIR 10370 END
                if (icdoWssPersonAccountEnrollmentRequest.is_changes_in_anne_flag == busConstant.Flag_No)
                    ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction = ibusMSSPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag;
                if (astrWizardStepName == "wzsStep2" || astrWizardStepName == "wzsStep3" || astrWizardStepName == "wzsStep4")
                {
                    if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                        iblnIsEOIRequired = IsEvidenceofInsurabilityMandatory(ibusMSSPersonAccountLife, false);
                    else
                        iblnIsEOIRequired = IsEvidenceofInsurabilityMandatory(ibusMSSPersonAccountLife, true);
                }

                //PIR 10695 "Save & Quit" functionality
                if ((iobjPassInfo.istrFormName == "wfmLifeEnrollmentWizard" && EnrollmentStep == "wzsStep5")
                || (iobjPassInfo.istrFormName == "wfmLifeAnnualEnrollmentWizard" && (EnrollmentStep == "wzsStep6")))
                {
                    iblnFinishButtonClicked = true;
                }

                EvaluateInitialLoadRules();
            }
            //PIR 10854
            if (astrWizardStepName == "wzsStep2" && (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdMain2020 || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDC2025))
            {
                EvaluateInitialLoadRules();
            }
            //PIR 25768  EnrollmentRequest for life  is more than two.
            if ((iobjPassInfo.istrFormName == "wfmLifeAnnualEnrollmentWizard") || (iobjPassInfo.istrFormName == "wfmLifeEnrollmentWizard"))
            {
                if (Convert.ToInt32(DBFunction.DBExecuteScalar("cdoWssPersonAccountEnrollmentRequest.CountLifeEnrollmentRequest", new object[1]
                       { icdoWssPersonAccountEnrollmentRequest.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) >= 2)
                {
                    iblnIsCountLifeEnrollmentRequest = true;
                }

            }

            base.BeforeWizardStepValidate(aenmPageMode, astrWizardName, astrWizardStepName);
        }

        public int iintDateEventValidation { get; set; }

        // PIR 10309 -- B.R. If today’s date is more then 31 days after the date of the “event” then throw an error.  
        // “You can only change your insurance coverage within 31 days of a qualifying event; you must wait until Annual Enrollment” 
        private void SetDateEventValidation()
        {
            iintDateEventValidation = 0;

            // PIR 12109 - For change reason NEWH employment start date should be considered
            DateTime ldtDateOfChange = new DateTime();
            if ((icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonNewHire ||
                        icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueEmploymentStatusChange) && icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id > 0)
                ldtDateOfChange = (DateTime)DBFunction.DBExecuteScalar("cdoPersonEmploymentDetail.GetStartDateByEmpDetailID", new object[1] { icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id },
                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework); //PIR 12385                
            else
                ldtDateOfChange = icdoWssPersonAccountEnrollmentRequest.date_of_change;

            if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll ||
                //icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueCancel ||
                icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueReturnFromLeave ||
                icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueAddRemoveDependent || (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueQualifiedChangeInStatus && icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex))
            {
                if (ldtDateOfChange != DateTime.MinValue &&
                    ldtDateOfChange.AddDays(31) < DateTime.Today)
                {
                    if ((icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueAddRemoveDependent &&
                        icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueRemoveDependent) ||
                        icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueCancel)
                        iintDateEventValidation = 1;
                    else
                        iintDateEventValidation = 2;
                }
            }
        }

        public void GetPreviousStepDependents()
        {
            if (!(icdoMSSGDHV.level_of_coverage_value == busConstant.DentalLevelofCoverageIndividual || icdoMSSGDHV.level_of_coverage_value == busConstant.VisionLevelofCoverageIndividual))
                iclbMSSPersonDependent = iclbMSSPersonDependentPrevious;
        }

        public void GetNextStepDependents()
        {
            iclbMSSPersonDependent = iclbMSSPersonDependentPrevious;
        }

        public DateTime GetDateofChange()
        {
            DateTime ldteDateofChange = new DateTime();
            if (icdoWssPersonAccountEnrollmentRequest.date_of_change.Day != 1)
            {
                if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueBirth && ibusPlan.IsGHDVPlan())
                    //!string.IsNullOrEmpty(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code) &&         
                    //!IsPreviousHealthCoverageSingle(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code)) // PIR 10152 remove conditions
                    ldteDateofChange = icdoWssPersonAccountEnrollmentRequest.date_of_change.GetFirstDayofCurrentMonth();
                else
                    ldteDateofChange = icdoWssPersonAccountEnrollmentRequest.date_of_change.GetFirstDayofNextMonth();
            }
            else
            {
                //if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueBirth && ibusPlan.IsGHDVPlan() &&
                //!string.IsNullOrEmpty(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code) &&
                //IsPreviousHealthCoverageSingle(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.coverage_code))
                if (ibusPlan.IsGHDVPlan() && icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueMarriage) //PIR 13092
                    ldteDateofChange = icdoWssPersonAccountEnrollmentRequest.date_of_change.GetFirstDayofNextMonth();
                else
                    ldteDateofChange = icdoWssPersonAccountEnrollmentRequest.date_of_change;
            }
            idtRequestDateTime = ldteDateofChange;
            return ldteDateofChange;
        }

        /// <summary>
        ///  pir: 6978 To check if duplicate ssn exists in dependents collection
        /// </summary>
        /// <returns></returns>
        public bool IsDuplicateSSNExists()
        {
            bool lblnResult = false;

            if (iclbMSSPersonDependent.IsNotNull())
            {
                foreach (busWssPersonDependent lobjPersonDepedent in iclbMSSPersonDependent)
                {
                    if (lobjPersonDepedent.icdoWssPersonDependent.ssn.IsNotNull())
                    {
                        var lenumlist = iclbMSSDependents.Where(lobjDep => lobjDep.icdoWssPersonDependent.ssn == lobjPersonDepedent.icdoWssPersonDependent.ssn &&
                                            lobjDep.icdoWssPersonDependent.wss_person_dependent_id != lobjDep.icdoWssPersonDependent.wss_person_dependent_id);
                        if (lenumlist.Count() > 1)
                        {
                            lblnResult = true;
                            break;
                        }
                    }
                }
            }
            return lblnResult;
        }

        /// <summary>
        /// method to set person properties for dependent
        /// </summary>
        /// <param name="abusPerson">new person obj</param>
        /// <param name="abusWSSPersonDep">wss person dependent obj</param>
        public void SetPersonProperties(busPerson abusPerson, busWssPersonDependent abusWSSPersonDep)
        {
            abusPerson.icdoPerson.first_name = abusWSSPersonDep.icdoWssPersonDependent.first_name;
            abusPerson.icdoPerson.middle_name = abusWSSPersonDep.icdoWssPersonDependent.middle_name;
            abusPerson.icdoPerson.last_name = abusWSSPersonDep.icdoWssPersonDependent.last_name;
            abusPerson.icdoPerson.ssn = abusWSSPersonDep.icdoWssPersonDependent.ssn;
            abusPerson.icdoPerson.date_of_birth = abusWSSPersonDep.icdoWssPersonDependent.date_of_birth;
            abusPerson.icdoPerson.gender_value = abusWSSPersonDep.icdoWssPersonDependent.gender_value;
            abusPerson.icdoPerson.marital_status_value = abusWSSPersonDep.icdoWssPersonDependent.marital_status_value;
        }

        /// <summary>
        /// method to person dependent properties
        /// </summary>
        /// <param name="abusNewPersonDependent">perslink person dependent obj</param>
        /// <param name="abusPersonDependent">mss person dependent obj</param>
        private void SetPersonDependentProperties(busPersonDependent abusNewPersonDependent, busWssPersonDependent abusPersonDependent)
        {
            abusNewPersonDependent.icdoPersonDependent.first_name = abusPersonDependent.icdoWssPersonDependent.first_name;
            abusNewPersonDependent.icdoPersonDependent.middle_name = abusPersonDependent.icdoWssPersonDependent.middle_name;
            abusNewPersonDependent.icdoPersonDependent.last_name = abusPersonDependent.icdoWssPersonDependent.last_name;
            abusNewPersonDependent.icdoPersonDependent.date_of_birth = abusPersonDependent.icdoWssPersonDependent.date_of_birth;
            abusNewPersonDependent.icdoPersonDependent.marital_status_value = abusPersonDependent.icdoWssPersonDependent.marital_status_value;
            abusNewPersonDependent.icdoPersonDependent.gender_value = abusPersonDependent.icdoWssPersonDependent.gender_value;
            abusNewPersonDependent.icdoPersonDependent.full_time_student_flag = abusPersonDependent.icdoWssPersonDependent.full_time_student_flag;
        }

        /// <summary>
        /// pir : 6335 To load dependents on view request screen
        /// </summary>
        public void LoadDependentsForViewRequest()
        {
            DataTable ldtblist = Select<cdoWssPersonDependent>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                    new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
            //DataTable ldtblistNew = ldtblist.Clone();

            //foreach (DataRow dr in ldtblist.Rows)
            //{
            //    if (!(dr["EFFECTIVE_START_DATE"].ToString().IsNullOrEmpty() && dr["EFFECTIVE_END_DATE"].ToString().IsNullOrEmpty()
            //        && dr["CURRENT_PLAN_ENROLLMENT_OPTION_VALUE"].ToString() == busConstant.PlanOptionStatusValueWaived))
            //        ldtblistNew.ImportRow(dr);
            //}

            //if (iblnIsFromPortal)
            iclbMSSPersonDependent = GetCollection<busWssPersonDependent>(ldtblist, "icdoWssPersonDependent");
            //else
            //    iclbMSSPersonDependent = GetCollection<busWssPersonDependent>(ldtblistNew, "icdoWssPersonDependent");

            foreach (busWssPersonDependent lobjPersonDependent in iclbMSSPersonDependent)
            {
                //below code is required if we assaigning a collection to another one (for eg. sgt_person_dependent to sgt_wss_person_account_Dependent),
                //iobjMainCDO will be pointing to the original one and grid update will fail. Hence need to explicitly assaign the iobjMainCDO
                lobjPersonDependent.iobjMainCDO = lobjPersonDependent.icdoWssPersonDependent;
                lobjPersonDependent.icdoWssPersonDependent.istrDependentName = lobjPersonDependent.icdoWssPersonDependent.first_name + ", "
                    + lobjPersonDependent.icdoWssPersonDependent.middle_name + " ,"
                    + lobjPersonDependent.icdoWssPersonDependent.last_name;

                if (!string.IsNullOrEmpty(lobjPersonDependent.icdoWssPersonDependent.ssn))
                {
                    lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId = busGlobalFunctions.GetPersonIDBySSN(lobjPersonDependent.icdoWssPersonDependent.ssn);
                }
                lobjPersonDependent.icdoWssPersonDependent.iintPersonID = this.ibusPerson.icdoPerson.person_id;
            }
        }

        //PIR 6406 Adding Hard Error
        public bool IsPlanStartDateLessThanFirstDayofFollowingMonthEmpDate()
        {
            LoadPersonEmploymentDetail();
            ibusPersonEmploymentDetail.LoadPersonEmployment();

            if ((ibusPersonEmploymentDetail != null) && (ibusPersonEmploymentDetail.ibusPersonEmployment != null))
            {
                if (icdoWssPersonAccountEnrollmentRequest.date_of_change != DateTime.MinValue)
                {
                    if ((ibusPlan.icdoPlan.plan_code == busConstant.PlanCode457) || (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeDental) ||
                       (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeEAP) || (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeGroupHealth) ||
                       (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeGroupLife) || (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeVision) ||
                       (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeFlex) || (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeOther457))
                    {
                        if (icdoWssPersonAccountEnrollmentRequest.date_of_change < ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        ///  pir 6287 and 6407 - Validations in View Request screen included in Wss
        /// </summary>
        /// <param name="aenmPageMode"></param>
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            base.ValidateHardErrors(aenmPageMode);
            if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex)
            {
                if (ibusPerson.IsNull()) LoadPerson();
                if (ibusPlan.IsNull()) LoadPlan();
                if (ibusPersonAccount.IsNull()) LoadPersonAccount();
                if (ibusPersonEmploymentDetail.IsNull())
                {
                    LoadPersonEmploymentDetail();
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                    ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                }
                if (ibusPersonAccountEmploymentDetail.IsNull()) LoadPersonAccountEmploymentDetail();
                //To load the annual pledge values
                LoadTotalSalaryRedirectionForPlanYearForDependent();
                LoadTotalSalaryRedirectionForPlanYearForMedical();

                //Validations for PersonAccountFlexComp
                busPersonAccountFlexComp lobjPersonAccountFlexComp = new busPersonAccountFlexComp { icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp() };
                lobjPersonAccountFlexComp.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
                lobjPersonAccountFlexComp.LoadPersonAccount();
                lobjPersonAccountFlexComp.LoadPerson();
                if (lobjPersonAccountFlexComp.icdoPersonAccount.plan_participation_status_value == null)
                    lobjPersonAccountFlexComp.icdoPersonAccount.plan_participation_status_value = busConstant.PlanParticipationStatusFlexCompEnrolled;
                lobjPersonAccountFlexComp.iclbFlexCompOption = new Collection<busPersonAccountFlexCompOption>();
                LoadMSSFlexCompOptions(lobjPersonAccountFlexComp);
                lobjPersonAccountFlexComp.iclbFlexCompOptionModified = new Collection<busPersonAccountFlexCompOption>();
                lobjPersonAccountFlexComp.icdoPersonAccountFlexComp.flex_comp_type_value = busConstant.FlexCompTypeValueActive;
                lobjPersonAccountFlexComp.iblnIsFromMSS = true;
                lobjPersonAccountFlexComp.BeforeValidate(utlPageMode.All);
                lobjPersonAccountFlexComp.ValidateHardErrors(utlPageMode.All);
                foreach (utlError aobjError in lobjPersonAccountFlexComp.iarrErrors)
                    iarrErrors.Add(aobjError);

                //Validations for PersonAccountFlexCompConversion
                if (iclbMSSFlexCompConversion.Count > 0)
                {
                    var lenumlist = iclbMSSFlexCompConversion.Where(lobjConversion => lobjConversion.icdoWssPersonAccountFlexCompConversion.istrIsSelected == busConstant.Flag_Yes);

                    foreach (busWssPersonAccountFlexCompConversion lobjPAFlexCompConversion in lenumlist)
                    {
                        busPersonAccountFlexcompConversion lobjPAFC = new busPersonAccountFlexcompConversion { icdoPersonAccountFlexcompConversion = new cdoPersonAccountFlexcompConversion() };
                        lobjPAFC.icdoPersonAccountFlexcompConversion.effective_start_date = lobjPAFlexCompConversion.icdoWssPersonAccountFlexCompConversion.effective_start_date;
                        lobjPAFC.icdoPersonAccountFlexcompConversion.org_id = lobjPAFlexCompConversion.icdoWssPersonAccountFlexCompConversion.org_id;
                        if (lobjPAFC.ibusPersonAccount.IsNull())
                            lobjPAFC.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                        lobjPAFC.ibusPersonAccount.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
                        lobjPAFC.icdoPersonAccountFlexcompConversion.person_account_id = ibusMSSPersonAccountFlexComp.icdoPersonAccount.person_account_id;
                        lobjPAFC.LoadPersonAccount();
                        lobjPAFC.ibusPersonAccount.LoadPerson();
                        lobjPAFC.BeforeValidate(utlPageMode.All);
                        lobjPAFC.ValidateHardErrors(utlPageMode.All);
                        foreach (utlError aobjError in lobjPAFC.iarrErrors)
                            if (!iarrErrors.Contains(aobjError))
                                iarrErrors.Add(aobjError);
                    }
                }
            }
            foreach (utlError lobjError in iarrErrors)
                lobjError.istrErrorID = string.Empty;
        }

        /// <summary>
        /// pir 7790 : Loads Medicare Details
        /// </summary>
        public void LoadMedicareDetails()
        {
            if (ibusMSSPersonAccountGHDV.IsNotNull())
            {
                icdoMSSGDHV.medicare_claim_no = ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_claim_no;
                icdoMSSGDHV.medicare_part_a_effective_date = ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_a_effective_date;
                icdoMSSGDHV.medicare_part_b_effective_date = ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_b_effective_date;
            }
        }

        /// <summary>
        /// pir 8004
        /// </summary>
        /// <returns>True: When Dependent Relationship is Spouse and 
        /// Marital status of Person  is not married
        /// </returns>
        public bool IsDependentSpouseAndMemberSingle()
        {
            bool lblnResult = false;
            if (iclbMSSPersonDependent.Count() > 0)
            {
                busWssPersonDependent lbusWssPersonDependent = iclbMSSPersonDependent.Where(i => i.icdoWssPersonDependent.relationship_value.IsNotNull()
                && i.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueEnrolled // PIR 11912 - trigger error 7800 only if Spouse is Enrolled
                && i.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse).FirstOrDefault();
                if (lbusWssPersonDependent.IsNotNull()
                    && !(ibusPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried))
                {
                    //if(!ibusPerson.icdoPerson.marital_status_value.Equals(busConstant.PersonMaritalStatusMarried))
                    //||lbusWssPersonDependent.icdoWssPersonDependent.marital_status_value.IsNull() 
                    // ||!lbusWssPersonDependent.icdoWssPersonDependent.marital_status_value.Equals(busConstant.PersonMaritalStatusMarried))
                    lblnResult = true;
                }
            }
            return lblnResult;
        }

        // PIR 9923
        public bool IsValidDependentExists()
        {
            if (!IsCoverageSingle()) //PIR 14894 - Dependent validation when coverage is not single
            {
                if (iclbMSSPersonDependent.IsNull() || iclbMSSPersonDependent.Count == 0 ||
                    (iclbMSSPersonDependent.Where(o => o.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PersonAccountElectionValueEnrolled).Count() == 0))
                    return false;
            }
            if ((!string.IsNullOrEmpty(icdoMSSGDHV.level_of_coverage_value) && icdoMSSGDHV.level_of_coverage_value == busConstant.VisionLevelofCoverageFamily ||
                icdoMSSGDHV.level_of_coverage_value == busConstant.DentalLevelofCoverageFamily))
            {
                if ((iclbMSSPersonDependent.IsNull() || iclbMSSPersonDependent.Count == 0 || iclbMSSPersonDependent.Count == 1 ||
                    (iclbMSSPersonDependent.Where(o => o.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PersonAccountElectionValueEnrolled).Count() == 0)
                    || (iclbMSSPersonDependent.Where(o => o.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PersonAccountElectionValueEnrolled).Count() == 1))
                    || !(iclbMSSPersonDependent.Where(o => o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipSpouse).Count() > 0
                    &&
                    iclbMSSPersonDependent.Where(o => o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipStepChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipAdoptiveChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipGrandChild ||
                        o.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipDisabledChild).Count() > 0))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// pir 8002
        /// </summary>
        /// <returns> True: When Effective start date of any dependent is less than Change Date
        /// </returns>
        public bool IsEffectiveStartDateLessThanPlanParticipationStartDate()
        {
            bool lblnResult = false;
            if (iclbMSSPersonDependent.Count() > 0)
            {
                if (iclbMSSPersonDependent.Where(i => i.icdoWssPersonDependent.effective_start_date.IsNotNull()
                    && i.icdoWssPersonDependent.effective_start_date < icdoWssPersonAccountEnrollmentRequest.date_of_change).Any())
                {
                    lblnResult = true;
                }
            }
            return lblnResult;
        }

        public string istrHDHPAcknowledgement { get; set; }

        #region Auto-Posting

        public DataTable idtbAutoPostingCrossRef { get; set; }
        public void LoadAutoPostingCrossRef()
        {
            idtbAutoPostingCrossRef = Select<cdoWssAutoPostingCrossRef>(new string[0] { }, new object[0] { }, null, null);
        }

        // PIR 9868 Underlying issue is that the plan is still Enrolled. Waive only removed the Employment Detail link.
        // Suggest not showing Waive as an option when a plan is already linked to an Employment without End Date.

        //PIR 16507 - Commented under 16507 as per discussion with Maik, uncommented under 16765 changes
        public bool IsWaiveOptionAvailable()
        {
            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            if (ibusPersonAccount.iclbEmploymentDetail.IsNull()) ibusPersonAccount.LoadAllPersonEmploymentDetails();
            if (ibusPersonAccount.iclbEmploymentDetail.Count > 0 &&
                ibusPersonAccount.iclbEmploymentDetail.Where(i => i.icdoPersonEmploymentDetail.end_date == DateTime.MinValue).Any())
                return false;
            return true;
        }

        public Collection<cdoCodeValue> LoadPlanEnrollmentOption()
        {
            Collection<cdoCodeValue> lclbResult = new Collection<cdoCodeValue>();
            Collection<cdoCodeValue> lclbPlanOption = GetCodeValue(6003);
            if (idtbAutoPostingCrossRef.IsNull()) LoadAutoPostingCrossRef();
            DataRow[] ldtrPlanOptionPerPlan = idtbAutoPostingCrossRef.FilterTable(busConstant.DataType.Numeric, "PLAN_ID", icdoWssPersonAccountEnrollmentRequest.plan_id);
            //bool lblnIsWaiveOptionAvailable = IsWaiveOptionAvailable();
            foreach (cdoCodeValue lcdoCV in lclbPlanOption)
            {
                //PIR 16507 - Cancel option - Should only show if current status is Enrolled & Waive Option - Should not be visible if current status is enrolled.
                if (IsMemberAccountStatusEnrolled == false && lcdoCV.code_value == busConstant.PlanEnrollmentOptionValueCancel)
                    continue;
                //PIR 16765 - commented
                //if (lcdoCV.code_value == busConstant.PlanEnrollmentOptionValueWaive && IsMemberAccountStatusEnrolled == true)
                //    continue;

                //PIR-20619 MSS Annual Enrollment - if plan status  is suspended for Vision & Dental Plans only show Enroll in DDL
                if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment &&
                    lcdoCV.code_value == busConstant.PlanEnrollmentOptionValueWaive &&
                    (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                    icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision) &&
                    ibusPersonAccount.IsNotNull()
                    && (ibusPersonAccount.icdoPersonAccount.plan_participation_status_value.IsNull() || ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended))
                {
                    continue;
                }

                //PIR 16765 - Waive option logic back to what it was as per Maik call, waive option logic changes done as part of PIR 16507 undone.
                if (lcdoCV.code_value == busConstant.PlanEnrollmentOptionValueWaive && !IsWaiveOptionAvailable())
                    continue;
                if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment && // PIR 9966
                    (lcdoCV.code_value == "ADRD" || lcdoCV.code_value == "RTFL"))
                    continue;
                if (lcdoCV.code_value == busConstant.PlanEnrollmentOptionValueCancel && IsghdvPlan)
                {
                    if (ibusPersonEmploymentDetail.IsNull())
                        LoadPersonEmploymentDetail();
                    if (ibusMSSPersonAccountGHDV.IsNotNull() &&
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id > 0 &&
                        (ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.premium_conversion_indicator_flag != busConstant.Flag_Yes
                        || busGlobalFunctions.DateDiffInDays(ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, DateTime.Today) < 31)
                        && icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth) //PIR 16533 & 17842 - Show 'Cancel' option only for Health.
                    {
                        if (ldtrPlanOptionPerPlan.Where(o => o.Field<string>("PLAN_ENROLLMENT_OPTION_VALUE") == lcdoCV.code_value).Any())
                            lclbResult.Add(lcdoCV);
                    }
                }
                //PIR 26356/26428
                //Discussion with Maik : Remove Enroll and Waive from dropdown and not allow user to enroll/waive as New Hires for this scenario. This applies to Flex, Dental, Vision
                else if (icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.AnnualEnrollment && 
                        ((lcdoCV.code_value == busConstant.PlanEnrollmentOptionValueEnroll || lcdoCV.code_value == busConstant.PlanEnrollmentOptionValueWaive)
                        && (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)))
                {
                    if (ibusPersonEmploymentDetail.IsNull())
                        LoadPersonEmploymentDetail();

                    if (ibusMSSPersonAccountGHDV.ibusPersonAccount.IsNull())
                        ibusMSSPersonAccountGHDV.LoadPersonAccount();

                    DataTable ldtEmpDetIdLinkedToPersonAccountId = busBase.Select("entPersonEmploymentDetail.GetEmpDetIdLinkedToPersonAccountId",
                                                        new object[2] { icdoWssPersonAccountEnrollmentRequest.person_id,
                                                                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.person_account_id });

                    if (ibusMSSPersonAccountGHDV.IsNotNull() && ibusMSSPersonAccountGHDV.ibusPersonAccount.IsNotNull()
                        && ibusMSSPersonAccountGHDV.ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                    {
                        lclbResult.Add(lcdoCV);
                    }

                    //PIR 26356 Existing Person Account and last enroll < 31 days, don’t allow enrollment, even when it is in the next calendar year.
                    else if (ibusMSSPersonAccountGHDV.IsNotNull() && ibusMSSPersonAccountGHDV.ibusPersonAccount.IsNotNull()
                        && ibusMSSPersonAccountGHDV.ibusPersonAccount.icdoPersonAccount.person_account_id > 0
                        //this checks if employment gap is > 31 days
                        && (ibusMSSPersonAccountGHDV.ibusPersonAccount.IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days() 
                            || ldtEmpDetIdLinkedToPersonAccountId.Rows.Count == 0))
                    {
                        lclbResult.Add(lcdoCV);
                    }
                } 
                else if (icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.AnnualEnrollment && 
                        ((lcdoCV.code_value == busConstant.PlanEnrollmentOptionValueEnroll || lcdoCV.code_value == busConstant.PlanEnrollmentOptionValueWaive || lcdoCV.code_value == busConstant.PlanEnrollmentOptionValueQualifiedChangeInStatus)
                        && (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex)))
                {
                    if (ibusMSSPersonAccountFlexComp.ibusPersonAccount.IsNull())
                        ibusMSSPersonAccountFlexComp.LoadPersonAccount();

                    DataTable ldtMemberWasEnrolledAsPreTax = busBase.Select("entPersonAccount.IsMemberWasEnrolledAsPreTax",
                                                new object[2] { busGlobalFunctions.GetSysManagementBatchDate().GetFirstDayofNextMonth().Year, ibusMSSPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_id });

                    if (ibusMSSPersonAccountFlexComp.IsNotNull() && ibusMSSPersonAccountFlexComp.ibusPersonAccount.IsNotNull()
                        && ibusMSSPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                    {
                        lclbResult.Add(lcdoCV);
                    }
                    //PIR 26356 Existing Person Account and last enroll < 31 days, don’t allow enrollment, even when it is in the next calendar year.
                    else if (ibusMSSPersonAccountFlexComp.IsNotNull() && ibusMSSPersonAccountFlexComp.ibusPersonAccount.IsNotNull()
                        //If member had Pre Tax Y for Dental/Vision or Life; hide New Hire from Flex if within the same calendar year 
                        && (ldtMemberWasEnrolledAsPreTax.IsNullOrEmpty() || ldtMemberWasEnrolledAsPreTax.Rows.Count == 0)
                        && ibusMSSPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_account_id > 0
                        //this checks if employment gap is > 31 days
                        && ibusMSSPersonAccountFlexComp.ibusPersonAccount.IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days()
                        //if employment gap > 31 days then look if member is enrolled in Flex for current year
                        && !(ibusMSSPersonAccountFlexComp.IsFlexCompEnrolledInCurrentYear(busGlobalFunctions.GetSysManagementBatchDate().GetFirstDayofNextMonth().Year)))
                    {
                        lclbResult.Add(lcdoCV);
                    }
                    else if (lcdoCV.code_value == busConstant.PlanEnrollmentOptionValueQualifiedChangeInStatus
                        && ibusMSSPersonAccountFlexComp.IsNotNull() && ibusMSSPersonAccountFlexComp.ibusPersonAccount.IsNotNull()
                        && ibusMSSPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_account_id > 0
                        //this checks if employment gap < 31 days
                        && (!ibusMSSPersonAccountFlexComp.ibusPersonAccount.IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days() ||
                            (
                                //If member had Pre Tax Y for Dental/Vision or Life; hide New Hire from Flex if within the same calendar year 
                                (ldtMemberWasEnrolledAsPreTax.IsNullOrEmpty() || ldtMemberWasEnrolledAsPreTax.Rows.Count == 0)
                                //this checks if employment gap is > 31 days
                                && ibusMSSPersonAccountFlexComp.ibusPersonAccount.IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days()
                                //if employment gap > 31 days then look if member is enrolled in Flex for current year
                                && !(ibusMSSPersonAccountFlexComp.IsFlexCompEnrolledInCurrentYear(busGlobalFunctions.GetSysManagementBatchDate().GetFirstDayofNextMonth().Year))
                            ))
                        )
                    {
                        lclbResult.Add(lcdoCV);
                    }
                }         
                else
                {
                    if (ldtrPlanOptionPerPlan.Where(o => o.Field<string>("PLAN_ENROLLMENT_OPTION_VALUE") == lcdoCV.code_value).Any())
                        lclbResult.Add(lcdoCV);
                }
            }
            if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth && iblnIsEnrollInHealthAsTemporary)
            {
                lclbResult = lclbResult
                            .Where(option => option.code_value == busConstant.PlanEnrollmentOptionValueEnroll || option.code_value == busConstant.PlanEnrollmentOptionValueWaive)
                            .ToList()
                            .ToCollection();
            }
            iblnIsChangeReasonDropDownVisible = true;
            if (lclbResult.Count == 0)
                iblnIsChangeReasonDropDownVisible = false;
            return lclbResult;
        }

        public bool IsghdvPlan
        {
            get
            {
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth ||
                    icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental ||
                    icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
                    return true;
                return false;
            }
        }

        // PIR 9393
        public bool IsTemporaryEmployee
        {
            get
            {
                if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                    return true;
                return false;
            }
        }

        // PIR 9873 Should only be available for: State, district health Unit
        public bool IsHDHPValid
        {
            get
            {
                if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusPersonEmploymentDetail.LoadPersonEmployment();
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull()) ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState ||
                    ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryDistrictHealthUnits)
                    return true;
                return false;
            }
        }

        //PIR-17683 Regular Health wizard landing page in MSS
        public bool IsHealthPlan
        {
            get
            {
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth)
                    return true;
                return false;
            }
        }

        //PIR-17683 Regular Health wizard landing page in MSS
        public string istrCancelMsgForNotInDHP
        {
            get
            {
                string lstrMessage = string.Empty;
                string lstrWhere = "message_id = 10393";
                DataTable ldtMessage = iobjPassInfo.isrvDBCache.GetCacheData("sgs_messages", lstrWhere);
                if(ldtMessage.Rows.Count > 0)
                {
                    lstrMessage = ldtMessage.Rows[0]["display_message"].ToString();
                }
                return lstrMessage;
            }
        }

        public Collection<cdoCodeValue> LoadTypeofCoverage()
        {
            Collection<cdoCodeValue> lclbResult = new Collection<cdoCodeValue>();
            Collection<cdoCodeValue> lclbTypeofCoverage = GetCodeValue(5015);
            if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
            foreach (cdoCodeValue lcdoCV in lclbTypeofCoverage)
            {
                if (lcdoCV.code_value == busConstant.AlternateStructureCodeHDHP && (IsTemporaryEmployee || !IsHDHPValid))
                    continue;
                lclbResult.Add(lcdoCV);
            }
            return lclbResult;
        }

        public Collection<cdoCodeValue> LoadChangeReason(string astrPlanEnrollmentOption)
        {
            Collection<cdoCodeValue> lclbResult = new Collection<cdoCodeValue>();
            Collection<cdoCodeValue> lclbPlanOption = GetCodeValue(332);
            if (idtbAutoPostingCrossRef.IsNull()) LoadAutoPostingCrossRef();
            DataRow[] ldtrPlanOptionPerPlan = idtbAutoPostingCrossRef.FilterTable(busConstant.DataType.Numeric, "PLAN_ID", icdoWssPersonAccountEnrollmentRequest.plan_id);
            foreach (cdoCodeValue lcdoCV in lclbPlanOption)
            {
                if (lcdoCV.code_value == busConstant.ReasonValueEmploymentStatusChange)
                {
                    // PIR 25345
                    if (IsMemberEmplChangeFromTempToPerm())
                    {
                        if (ldtrPlanOptionPerPlan.Where(o => o.Field<string>("PLAN_ENROLLMENT_OPTION_VALUE") == astrPlanEnrollmentOption &&
                                         o.Field<string>("CHANGE_REASON_VALUE") == lcdoCV.code_value).Any())
                            lclbResult.Add(lcdoCV);
                    }
                }
                    if (lcdoCV.code_value != busConstant.AnnualEnrollment && lcdoCV.code_value != busConstant.ChangeReasonACAAnnualEnrollment && lcdoCV.code_value != busConstant.ChangeReasonACAEligibleTemporary // PIR 9966
                        && lcdoCV.code_value != busConstant.ReasonValueEmploymentStatusChange) // PIR 25345
                {
                    if (ldtrPlanOptionPerPlan.Where(o => o.Field<string>("PLAN_ENROLLMENT_OPTION_VALUE") == astrPlanEnrollmentOption &&
                                                         o.Field<string>("CHANGE_REASON_VALUE") == lcdoCV.code_value).Any())
                        lclbResult.Add(lcdoCV);
                }
            }
            return lclbResult;
        }

        public Collection<cdoCodeValue> LoadChangeReasonWithoutPEOption()
        {
            return LoadChangeReason(icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value);
        }

        public cdoWssAutoPostingCrossRef icdoAutoPostingCrossRef { get; set; }
        public void FindAutoPostingCrossRef()
        {
            if (icdoAutoPostingCrossRef.IsNull()) icdoAutoPostingCrossRef = new cdoWssAutoPostingCrossRef();
            if (idtbAutoPostingCrossRef.IsNull()) LoadAutoPostingCrossRef();
            DataRow[] ldtrPlanOptionPerPlan = idtbAutoPostingCrossRef.FilterTable(busConstant.DataType.Numeric, "PLAN_ID", icdoWssPersonAccountEnrollmentRequest.plan_id);
            var lResult = ldtrPlanOptionPerPlan.Where(o => o.Field<string>("PLAN_ENROLLMENT_OPTION_VALUE") == icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value &&
                                                             o.Field<string>("CHANGE_REASON_VALUE") == icdoWssPersonAccountEnrollmentRequest.reason_value).FirstOrDefault();
            if (lResult.IsNotNull())
                icdoAutoPostingCrossRef.LoadData(lResult);
        }

        public void SetEnrollmentValues()
        {
            if (ibusPlan.IsRetirementPlan())
            {
                icdoWssPersonAccountEnrollmentRequest.date_of_change = DateTime.Today.GetFirstDayofNextMonth();
                if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
                icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value = busConstant.PlanEnrollmentOptionValueEnroll;
                icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.ReasonValueNotApplicable;
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date != DateTime.MinValue)
                    icdoWssPersonAccountEnrollmentRequest.date_of_change = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.GetFirstDayofNextMonth();
            }
            if (ibusPlan.IsInsurancePlan() || ibusPlan.IsDeferredCompPlan() ||
                ibusPlan.icdoPlan.plan_id == busConstant.PlanIdFlex) // PIR 9693
            {
                if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueWaive &&
                    icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.AnnualEnrollment && icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ChangeReasonACAAnnualEnrollment
                    && !(icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth && iblnIsEnrollInHealthAsTemporary))
                    icdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.ReasonValueWaivePlan; // PIR 10054
                if (icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueCancel &&
                    icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.AnnualEnrollment) // PIR 10249
                    icdoWssPersonAccountEnrollmentRequest.reason_value = "CNLD";

            }
            FindAutoPostingCrossRef(); // To Load everytime even if the reason is modified.
            if (icdoAutoPostingCrossRef.IsNotNull() &&
                (icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.AnnualEnrollment && icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.ChangeReasonACAAnnualEnrollment) &&
                istrCancelEnrollmentFlag != busConstant.Flag_Yes)
            {
                if (icdoAutoPostingCrossRef.change_effective_date_value == busConstant.ChangeEffectiveDateValueFirstofMonthFollowing)
                    icdoWssPersonAccountEnrollmentRequest.date_of_change = DateTime.Today.GetFirstDayofNextMonth();
                else
                {
                    if (!ibusPlan.IsRetirementPlan() && !(ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife))
                        icdoWssPersonAccountEnrollmentRequest.date_of_change = DateTime.MinValue;
                }

                if ((icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonNewHire || icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueEmploymentStatusChange )&&
                    ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date != DateTime.MinValue)
                {
                    if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdFlex)
                    {
                        if (!(icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment))
                        {
                            if (icdoWssPersonAccountEnrollmentRequest.date_of_change < DateTime.Now)
                            {
                                if (DateTime.Now.Day <= 20)
                                    icdoWssPersonAccountEnrollmentRequest.date_of_change = DateTime.Now.GetFirstDayofNextMonth();
                                else
                                    icdoWssPersonAccountEnrollmentRequest.date_of_change = DateTime.Now.AddMonths(1).GetFirstDayofNextMonth();
                            }
                        }
                        // PIR 10066
                        //if (DateTime.Now.Day <= 15)
                        //    icdoWssPersonAccountEnrollmentRequest.date_of_change = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.GetFirstDayofNextMonth();
                        //else
                        //    icdoWssPersonAccountEnrollmentRequest.date_of_change = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddMonths(1).GetFirstDayofNextMonth();
                    }
                    else
                        icdoWssPersonAccountEnrollmentRequest.date_of_change = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.GetFirstDayofNextMonth();
                }
            }
            if (ibusPlan.IsGHDVPlan() || ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife ||
                ibusPlan.icdoPlan.plan_id == busConstant.PlanIdFlex)
            {
                //PIR 23567 when choosing ‘Remove Dependent’ change reason set path of divorce if Is Applied for divorce value is "Yes"
                if (ibusPlan.IsGHDVPlan() && icdoWssPersonAccountEnrollmentRequest.is_applied_for_divorce == busConstant.Flag_Yes && icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueRemoveDependent)
                {
                    string lstrTempReasonValue = string.Empty;
                    lstrTempReasonValue = icdoWssPersonAccountEnrollmentRequest.reason_value;
                    icdoWssPersonAccountEnrollmentRequest.reason_value = "DIVR";
                    FindAutoPostingCrossRef();
                    icdoWssPersonAccountEnrollmentRequest.reason_value = lstrTempReasonValue;
                }
                else 
				{
                	if (icdoAutoPostingCrossRef.IsNull()) FindAutoPostingCrossRef();
				}
                istrDateofChangeText = busConstant.ChangeEffectiveDefaultText;
                if (icdoAutoPostingCrossRef.IsNotNull())
                {
                    if (!string.IsNullOrEmpty(icdoAutoPostingCrossRef.prompt_user_text))
                        istrDateofChangeText = icdoAutoPostingCrossRef.prompt_user_text + " ";
                }
            }
            if(ibusPlan.IsGHDVPlan() && icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonNewHire)
                icdoWssPersonAccountEnrollmentRequest.date_of_change = Convert.ToDateTime(ibusPersonEmploymentDetail?.ibusPersonEmployment?.icdoPersonEmployment?.start_date.GetFirstDayofNextMonth());  // PIR 25038 
        }

        public string istrDateofChangeText { get; set; }

        #endregion

        public ArrayList btnWaiveLTC_Click()
        {
            ArrayList larrlist = new ArrayList();
            if (ibusPersonEmploymentDetail.IsNotNull())
            {
                if (ibusPersonAccountEmploymentDetail.IsNull()) LoadPersonAccountEmploymentDetail();
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueWaived;
                // PIR 11684
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.is_waiver_report_generated = busConstant.Flag_No;
                ibusPersonEmploymentDetail.LoadPersonEmployment();
                busGlobalFunctions.PostESSMessage(ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id,
                                        icdoWssPersonAccountEnrollmentRequest.plan_id, iobjPassInfo);
                // PIR 11684 end
                ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.Update();
            }
            return larrlist;
        }

        public string istrMSSPaymentOptionValue
        {
            get
            {
                string lstrReturnValue = string.Empty;
                if (icdoMSSFlexComp.IsNotNull())
                {
                    if (icdoMSSFlexComp.direct_deposit_flag == busConstant.Flag_Yes)
                        lstrReturnValue = busConstant.FlexCompPaymentOptionDirectDeposit;
                    else
                        lstrReturnValue = busConstant.FlexCompPaymentOptionCheck;
                }
                return lstrReturnValue;
            }
        }

        public string istrMSSMailOptionValue
        {
            get
            {
                string lstrReturnValue = string.Empty;
                if (icdoMSSFlexComp.IsNotNull())
                {
                    if (icdoMSSFlexComp.inside_mail_flag == busConstant.Flag_Yes)
                        lstrReturnValue = busConstant.FlexCompMailOptionInsideMail;
                    else
                        lstrReturnValue = busConstant.FlexCompMailOptionUSPostalService;
                }
                return lstrReturnValue;
            }
        }

        // PIR 9796
        public bool IsSSNMissingForAnyDependents()
        {
            if (iclbMSSDependents.IsNull())
                LoadMSSDependents();
            if (iclbMSSDependents.IsNotNull() && iclbMSSDependents.Count > 0)
            {
                if (iclbMSSDependents.Where(o => o.icdoWssPersonDependent.ssn.IsNullOrEmpty()).Any())
                    return true;
            }
            return false;
        }

        public bool IsSupportingDocumentVisible()
        {
            if (icdoAutoPostingCrossRef.IsNotNull() && !string.IsNullOrEmpty(icdoAutoPostingCrossRef.document))
            {
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife)
                {
                    if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueNewHire ||
                        icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueMarriage ||
                        icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ReasonValueBirth ||
                        icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment)
                    {
                        if (iblnIsEOIRequired)
                            return true;
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool IsPremiumConversionStartDateValid()
        {
            if (iclbMSSFlexCompConversion.IsNotNull())
            {
                DateTime ldteStartDate = new DateTime();
                if (ibusPersonAccount.IsNotNull() && ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                    ldteStartDate = ibusPersonAccount.icdoPersonAccount.start_date; // Update mode
                else if (icdoWssPersonAccountEnrollmentRequest.date_of_change != DateTime.MinValue)
                    ldteStartDate = icdoWssPersonAccountEnrollmentRequest.date_of_change;
                if (iclbMSSFlexCompConversion.Where(i => i.icdoWssPersonAccountFlexCompConversion.effective_start_date != DateTime.MinValue && i.icdoWssPersonAccountFlexCompConversion.effective_start_date < ldteStartDate).Any())
                    return false;
            }
            return true;
        }

        public bool IsPremiumConversionNotSelected()
        {
            if (iclbMSSFlexCompConversion.IsNotNull())
            {
                foreach (busWssPersonAccountFlexCompConversion lcdoConversion in iclbMSSFlexCompConversion)
                {
                    if (lcdoConversion.icdoWssPersonAccountFlexCompConversion.effective_start_date != DateTime.MinValue &&
                        lcdoConversion.icdoWssPersonAccountFlexCompConversion.istrIsSelected != busConstant.Flag_Yes)
                        return true;
                }
            }
            return false;
        }

        public bool IsMedicareEffectiveDateNOTFirstDayofMonth()
        {
            if ((ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_a_effective_date != DateTime.MinValue &&
                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_a_effective_date.Day != 1) ||
                (ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_b_effective_date != DateTime.MinValue &&
                ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.medicare_part_b_effective_date.Day != 1))
                return true;
            return false;
        }

        #region Annual Enrollment

        public DateTime AnnualEnrollmentEffectiveDate
        {
            get
            {
                return Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo));
            }
        }

        public DateTime AnnualEnrollmentTempEffectiveDate
        {
            get
            {
                return Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.ACACertAnnualEnrollmentWindow, iobjPassInfo));
            }
        }

        //PIR 12532
        public DateTime AnnualEnrollmentEffectiveEndDate
        {
            get
            {
                return busGlobalFunctions.GetLastDayOfEffectiveYear(AnnualEnrollmentEffectiveDate);
            }
        }

        public bool FindWssPersonAccountEnrollmentRequest(int aintPlanId, int aintPersonId, int aintPersonEmploymentDetailId)
        {
            if (icdoWssPersonAccountEnrollmentRequest.IsNull())
                icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();
            DataTable ldtbResult = Select<cdoWssPersonAccountEnrollmentRequest>(new string[3] { "PERSON_ID", "PLAN_ID", "PERSON_EMPLOYMENT_DTL_ID" },
                                                                                new object[3] { aintPersonId, aintPlanId, aintPersonEmploymentDetailId }, null, null);
            var lvar = ldtbResult.AsEnumerable().Where(ldr => ldr.Field<string>("STATUS_VALUE") == busConstant.EnrollRequestStatusPendingRequest ||
                                                       ldr.Field<string>("STATUS_VALUE") == busConstant.EnrollRequestStatusRejected || ldr.Field<string>("STATUS_VALUE") == busConstant.EnrollRequestStatusFinishLater);
            if (lvar.AsDataTable().Rows.Count > 0)
            {
                icdoWssPersonAccountEnrollmentRequest.LoadData(lvar.AsDataTable().Rows[0]);
                return true;
            }
            return false;
        }

        public busMSSGHDVWeb ibusMSSGHDVWeb { get; set; }
        public busMSSLifeWeb ibusMSSLifeWeb { get; set; }

        public string AnnualEnrollmentEffectiveYear
        {
            get
            {
                return Convert.ToString(Convert.ToDateTime(busGlobalFunctions.GetData3ByCodeValue(52, busConstant.AnnualEnrollment, iobjPassInfo)).Year);
            }
        }

        public bool IsMemberAccountExists
        {
            get
            {
                if (ibusPersonAccount.IsNotNull())
                {
                    if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0 &&
                        ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        return true;
                }
                return false;
            }
        }

        public bool IsMemberAccountStatusEnrolled
        {
            get
            {
                if (ibusPersonAccount.IsNotNull())
                {
                    if (ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                        return true;
                }
                return false;
            }
        }

        public bool IsHealthAccountExistsDuringMSRAANNE()
        {
            bool lblnResult = false;
            if (ibusMSSPersonAccountFlexComp.ibusMSRACoverage.IsNull())
                ibusMSSPersonAccountFlexComp.LoadFlexCompIndividualOptions();
            if (ibusMSSPersonAccountFlexComp.ibusMSRACoverage.IsNotNull())
            {
                //person is enrolled in msra
                if (ibusMSSPersonAccountFlexComp.ibusMSRACoverage.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0)
                {
                    if (ibusPerson.IsNull())
                        LoadPerson();
                    if (ibusPerson.icolPersonAccount.IsNull())
                        ibusPerson.LoadPersonAccount();
                    if (ibusPerson.icolPersonAccount.Count > 0)
                    {
                        busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth).FirstOrDefault();
                        if (lbusPersonAccount.IsNotNull())
                        {
                            lbusPersonAccount.LoadPersonAccountGHDV();
                            if (lbusPersonAccount.ibusPersonAccountGHDV.IsNotNull())
                            {
                                if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.hsa_effective_date != DateTime.MinValue
                                    && lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP
                                    && lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) 
                                {
                                    lblnResult = true;
                                }
                            }
                        }
                    }
                    //PIR 26747 
                    if (Convert.ToInt32(DBFunction.DBExecuteScalar("entWssPersonAccountEnrollmentRequest.IsHeathRequestPendingDuringMSRAANNE", new object[1] { ibusPerson.icdoPerson.person_id },
                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) > 0)
                    {
                        lblnResult = true;
                    }
                }
            }
            return lblnResult;
        }

        public bool IsHealthAccountExistsDuringMSRA()
        {
            if (icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP)
            {
                if (ibusPerson.IsNull())
                    LoadPerson();
                if (ibusPerson.icolPersonAccount.IsNull())
                    ibusPerson.LoadPersonAccount();
                if (ibusPerson.icolPersonAccount.Count > 0)
                {
                    busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdFlex).FirstOrDefault();
                    if (lbusPersonAccount.IsNotNull()) // PIR 10392
                    {
                        lbusPersonAccount.LoadPersonAccountFlex();
                        lbusPersonAccount.ibusPersonAccountFlex.LoadFlexCompOptionUpdate();
                        lbusPersonAccount.ibusPersonAccountFlex.LoadFlexCompIndividualOptions();
                        if (lbusPersonAccount.ibusPersonAccountFlex.ibusMSRACoverage.icdoPersonAccountFlexCompOption.annual_pledge_amount > 0 &&
                            busGlobalFunctions.CheckDateOverlapping(icdoWssPersonAccountEnrollmentRequest.date_of_change,
                                        lbusPersonAccount.ibusPersonAccountFlex.ibusMSRACoverage.icdoPersonAccountFlexCompOption.effective_start_date,
                                        lbusPersonAccount.ibusPersonAccountFlex.ibusMSRACoverage.icdoPersonAccountFlexCompOption.effective_end_date)) // PIR 10264
                            return true;
                    }
                }
            }
            return false;
        }

        // PIR 11912
        public bool IsFlexAccountExistsDuringHealth()
        {
            if (icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP)
            {
                if (ibusPerson.IsNull())
                    LoadPerson();
                if (ibusPerson.icolPersonAccount.IsNull())
                    ibusPerson.LoadPersonAccount();
                if (ibusPerson.icolPersonAccount.Count > 0)
                {
                    busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdFlex).FirstOrDefault();
                    if (lbusPersonAccount.IsNotNull())
                    {
                        lbusPersonAccount.LoadPersonAccountFlex();
                        lbusPersonAccount.ibusPersonAccountFlex.LoadFlexCompHistory();
                        return lbusPersonAccount.ibusPersonAccountFlex
                            .iclbFlexCompHistory.Any(history => history.icdoPersonAccountFlexCompHistory.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending &&
                                                               history.icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled &&
                                                               history.icdoPersonAccountFlexCompHistory.annual_pledge_amount > 0 &&
                                                               history.icdoPersonAccountFlexCompHistory.effective_start_date.Year == icdoWssPersonAccountEnrollmentRequest.date_of_change.Year &&
                                                               history.icdoPersonAccountFlexCompHistory.effective_end_date == DateTime.MinValue); //18011
                    }
                }
            }
            return false;
        }

        public string istrCancelEnrollmentFlag { get; set; }
        //PIR--10124 Start 
        public bool IsFlexCompPlanNotAvailable()
        {
            bool lblnResult = false;
            if (this.ibusPersonEmploymentDetail == null)
                this.LoadPersonEmploymentDetail();
            if (this.ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                this.ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (this.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                this.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            if (this.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan == null || this.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan.Count == 0)
                this.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.LoadOrgPlan();

            busOrgPlan lbusOrgPlan = this.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan.Where(o => o.icdoOrgPlan.plan_id == busConstant.PlanIdFlex).FirstOrDefault();
            if (lbusOrgPlan == null)
                lblnResult = true;
            else if (lbusOrgPlan != null)
            {
                if (lbusOrgPlan.icdoOrgPlan.participation_end_date != DateTime.MinValue && lbusOrgPlan.icdoOrgPlan.participation_end_date < DateTime.Now)
                    lblnResult = true;
            }
            return lblnResult;
        }
        //PIR--10124 End
        #endregion

        public string istrLifePlanProviderName { get; set; }
        public string istrDentalPlanProviderName { get; set; }
        public string istrVisionPlanProviderName { get; set; }

        // PIR 10699
        public void LoadNDPERSPlanProviderInfo()
        {
            DateTime ldteAnnualEffectiveDate = AnnualEnrollmentEffectiveDate;
            busOrganization lobjLifePlanProvider = busGlobalFunctions.GetProviderOrgByPlan(busConstant.PlanIdGroupLife, ldteAnnualEffectiveDate);
            busOrganization lobjDentalPlanProvider = busGlobalFunctions.GetProviderOrgByPlan(busConstant.PlanIdDental, ldteAnnualEffectiveDate);
            busOrganization lobjVisionPlanProvider = busGlobalFunctions.GetProviderOrgByPlan(busConstant.PlanIdVision, ldteAnnualEffectiveDate);

            istrLifePlanProviderName = lobjLifePlanProvider.icdoOrganization.org_name;
            istrDentalPlanProviderName = lobjDentalPlanProvider.icdoOrganization.org_name;
            istrVisionPlanProviderName = lobjVisionPlanProvider.icdoOrganization.org_name;
        }
        //PIR-10856 Start 
        public void ClearCobraValues()
        {
            bool lblnIsClearCograValuesByCondition = false;
            
            if (ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                lblnIsClearCograValuesByCondition = true;
            }
            else
            {
                //PIR 25432 / 25669 If member is currently enrolled in COBRA and enrolls via the wizard in MSS make the COBRA Type Value NULL.
                if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusPersonEmploymentDetail.LoadPersonEmployment();
                if(ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled
                    && ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue)
                    lblnIsClearCograValuesByCondition = true;
            }

            if (lblnIsClearCograValuesByCondition)
            {
                // In Person Account GHDV clear COBRA Type, COBRA Expiration Date, change Insurance Type (Dental, Vision), remove User Structure Override (Health).
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth)
                {
                    if (ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = string.Empty;
                    if (ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code = string.Empty;
                    if (ibusMSSPersonAccountGHDV.icdoPersonAccount.cobra_expiration_date != DateTime.MinValue)
                        ibusMSSPersonAccountGHDV.icdoPersonAccount.cobra_expiration_date = DateTime.MinValue;
                }
                else
                {
                    if (ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value == busConstant.DentalInsuranceTypeCOBRA)
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.dental_insurance_type_value = busConstant.DentalInsuranceTypeActive;
                    if (ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value == busConstant.VisionInsuranceTypeCOBRA)
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.vision_insurance_type_value = busConstant.VisionInsuranceTypeActive;
                    if (ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value.IsNotNullOrEmpty())
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.cobra_type_value = string.Empty;
                    if (ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                        ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code = string.Empty;
                    if (ibusMSSPersonAccountGHDV.icdoPersonAccount.cobra_expiration_date != DateTime.MinValue)
                        ibusMSSPersonAccountGHDV.icdoPersonAccount.cobra_expiration_date = DateTime.MinValue;
                }
                if (ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
                {
                    // Reset IBS Flags
                    ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag = busConstant.Flag_No;
                    ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id = 0;
                    ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value = string.Empty;
                    ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date = DateTime.MinValue;
                    ibusMSSPersonAccountGHDV.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id = 0;
                }
            }
        }
        //PIR-10856 End

        public bool IsOptionalRetirementStartDateValid()
        {
            if ((ibusPlan.IsDBRetirementPlan() || ibusPlan.IsHBRetirementPlan()) && icdoWssPersonAccountEnrollmentRequest.date_of_change != DateTime.MinValue)
            {
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date.AddMonths(6) < icdoWssPersonAccountEnrollmentRequest.date_of_change)
                    return false;
            }
            return true;
        }

        //PIR 11795
        public bool IsValidMedicareClaimNo()
        {
            if (icdoMSSGDHV.medicare_claim_no != null)
            {
                Regex lobjexp = new Regex("^[a-zA-Z0-9 ]*$");
                if (!(lobjexp.IsMatch(icdoMSSGDHV.medicare_claim_no)))
                    return false;
            }
            return true;
        }
        /// <summary>
        /// PIR 14894
        /// This method throws hard error 10202 if the member is already enrolled in any of the GHDV plans
        /// as a dependent.
        /// </summary>
        /// <returns></returns>
        public bool IsMemberAlreadyEnrolledAsDependent()
        {
            DateTime ldtChangeDate;
            if (ibusPerson.IsNull())
                LoadPerson();
            if (ibusPerson.iclbPersonDependentByDependent.IsNull())
                ibusPerson.LoadPersonDependentByDependent();
            foreach (busPersonDependent lbusPersonDependent in ibusPerson.iclbPersonDependentByDependent)
            {
                if (lbusPersonDependent.iclbPersonAccountDependent.IsNull())
                    lbusPersonDependent.LoadPersonAccountDependent();
                foreach (busPersonAccountDependent lbusPersonAccountDependent in lbusPersonDependent.iclbPersonAccountDependent)
                {
                    if (lbusPersonAccountDependent.ibusPersonAccount.IsNull())
                        lbusPersonAccountDependent.LoadPersonAccount();
                    if (lbusPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == icdoWssPersonAccountEnrollmentRequest.plan_id)
                    {
                        ldtChangeDate = icdoWssPersonAccountEnrollmentRequest.date_of_change;
                        //PIR- 16397 Added 1 day in date of change if date of change and person Account Dependent date are same. 
                        if (icdoWssPersonAccountEnrollmentRequest.date_of_change == lbusPersonAccountDependent.icdoPersonAccountDependent.end_date &&
                            icdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value == busConstant.PlanEnrollmentOptionValueEnroll)
                        {
                            ldtChangeDate = icdoWssPersonAccountEnrollmentRequest.date_of_change.AddDays(1);
                        }
                        if (busGlobalFunctions.CheckDateOverlapping(ldtChangeDate, lbusPersonAccountDependent.icdoPersonAccountDependent.start_date, lbusPersonAccountDependent.icdoPersonAccountDependent.end_date))
                            return true;
                    }
                }
            }
            return false;
        }


        public void PostEnrollments()
        {
            int i = 0;
            DataTable ldtbEnrollments = Select("cdoWssPersonAccountEnrollmentRequest.PostEnrollments", new object[0] { });
            foreach (DataRow dr in ldtbEnrollments.Rows)
            {
                //UpdateProcessLog("Skipped due to inactive status ", "SUMM", iobjBatchSchedule);
                //istrProcessName = "Missing Mandatory Plan Enrollments Report";
                //idlgUpdateProcessLog("Creating Missing Mandatory Plan Enrollments Report", "INFO", istrProcessName);

                busWssPersonAccountEnrollmentRequest lbusEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
                //lobjWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();
                //lobjWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.LoadData(dr);
                int lintRequestId = Convert.ToInt32(dr["WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID"]);
                DBFunction.StoreProcessLog(100, "Posting enrollment for : " + i.ToString() + " , " + lintRequestId.ToString(), "INFO", "Enrollment posting", "Batch", iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                if (lbusEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(lintRequestId))
                {
                    lbusEnrollmentRequest.LoadPlan();
                    lbusEnrollmentRequest.LoadPerson();
                    lbusEnrollmentRequest.LoadPersonAccount();
                    lbusEnrollmentRequest.LoadMSSWorkersCompensation();
                    lbusEnrollmentRequest.LoadMSSOtherCoverageDetails();
                    lbusEnrollmentRequest.LoadDependentsForViewRequest();
                    lbusEnrollmentRequest.LoadMSSGHDV();
                    lbusEnrollmentRequest.LoadPersonAccountGHDV();
                    lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                    lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                    lbusEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                    lbusEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                    lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.icdoPersonAccount = lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount;
                    lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.icdoPersonAccount.person_employment_dtl_id = lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                    lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.LoadPaymentElection();
                    lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.LoadRateStructure();
                    lbusEnrollmentRequest.LoadCoverageCodeDescription();
                    lbusEnrollmentRequest.LoadPersonAccountEmploymentDetail();
                    lbusEnrollmentRequest.LoadPersonEmploymentDetail();
                    lbusEnrollmentRequest.ibusPersonEmploymentDetail.LoadPersonEmployment();
                    lbusEnrollmentRequest.ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
                    lbusEnrollmentRequest.ibusMSSPersonAccountGHDV.icdoPersonAccount.person_employment_dtl_id = lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                    if (lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.acknolwedgement_part_d_flag != busConstant.Flag_Yes)
                        lbusEnrollmentRequest.LoadGHDVAcknowledgements();
                    if (lbusEnrollmentRequest.icdoMSSGDHV.type_of_coverage_value == busConstant.AlternateStructureCodeHDHP)
                        lbusEnrollmentRequest.LoadHDHPAcknowledgementView();
                    lbusEnrollmentRequest.LoadAckDetailsForView();

                    if (!(lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)) //PIR 10695 "Save & Quit" functionality // PIR 14889
                    {
                        busMSSGHDVWeb lbusPersonAccountGHDV = new busMSSGHDVWeb();
                        if (lbusPersonAccountGHDV.FindPersonAccount(lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            if (lbusPersonAccountGHDV.FindGHDVByPersonAccountID(lbusEnrollmentRequest.ibusPersonAccount.icdoPersonAccount.person_account_id))
                            {
                                lbusPersonAccountGHDV.LoadPerson();
                                lbusPersonAccountGHDV.LoadPlan();
                                lbusPersonAccountGHDV.GetCoverageCodeBasedOnPlans();
                                lbusPersonAccountGHDV.LoadPaymentElection();
                                lbusPersonAccountGHDV.LoadMSSInsuranceDetails();
                                lbusPersonAccountGHDV.LoadMSSContributingEmployers();
                                lbusPersonAccountGHDV.LoadMSSDependent();
                                lbusPersonAccountGHDV.LoadMSSHistoryGhdv();
                                lbusPersonAccountGHDV.LoadPersonAccountAchDetail();
                                lbusPersonAccountGHDV.iintEnrollmentRequestId = lintRequestId;
                                lbusPersonAccountGHDV.icdoPersonAccount.person_employment_dtl_id = lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id;
                                lbusPersonAccountGHDV.LoadProvider();
                                lbusPersonAccountGHDV.LoadPretaxPayrollDeduction();
                            }
                        }
                        lbusEnrollmentRequest.ibusMSSGHDVWeb = lbusPersonAccountGHDV;
                        lbusEnrollmentRequest.icdoMSSGDHV.level_of_coverage_value = lbusEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.level_of_coverage_value;
                        lbusEnrollmentRequest.icdoMSSGDHV.coverage_code = lbusEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.coverage_code;
                        lbusEnrollmentRequest.icdoMSSGDHV.type_of_coverage_value = lbusEnrollmentRequest.ibusMSSGHDVWeb.icdoPersonAccountGhdv.alternate_structure_code_value;
                        lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(332, lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.reason_value);
                        lbusEnrollmentRequest.icdoMSSGDHV.level_of_coverage_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(408, lbusEnrollmentRequest.icdoMSSGDHV.level_of_coverage_value);
                    }

                    //lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest = lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest;
                    lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.change_effective_date = lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.date_of_change;
                    //lbusEnrollmentRequest.icdoMSSGDHV = lbusEnrollmentRequest.icdoMSSGDHV;
                    lbusEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.modified_by = "PIR15117";

                    lbusEnrollmentRequest.btnPostGHDV_Click();

                    i++;
                }
            }
        }

        public decimal idecMinSupplementalLimit { get; set; }

        //PIR-17035   
        //fetching medical_annual_pledge_amount & dependent_annual_pledge_amount from current enrollment.   
        public void LoadMSSAnnualPledgeAmount()
        {
            ibusMSSPersonAccountFlexComp.LoadFlexCompOptionUpdate();
            foreach (busPersonAccountFlexCompOption lobjPersonAccountFlexCompOpt in ibusMSSPersonAccountFlexComp.iclbFlexCompOption)
            {
                if (lobjPersonAccountFlexCompOpt.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending)
                    icdoMSSFlexCompOption.medical_annual_pledge_amount = lobjPersonAccountFlexCompOpt.icdoPersonAccountFlexCompOption.annual_pledge_amount;
                if (lobjPersonAccountFlexCompOpt.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageDependentSpending)
                    icdoMSSFlexCompOption.dependent_annual_pledge_amount = lobjPersonAccountFlexCompOpt.icdoPersonAccountFlexCompOption.annual_pledge_amount;
            }
        }

        //PIR 16533 & 17842 
        public void LoadPretaxDeductionFlag()
        {
            if (ibusPlan.icdoPlan.plan_id != busConstant.PlanIdGroupHealth)
            {
                if (ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.premium_conversion_indicator_flag == busConstant.Flag_Yes)
                    istrPreTaxDeductionFlag = busConstant.Flag_Yes_Value;
                else
                    istrPreTaxDeductionFlag = busConstant.Flag_No_Value;

                icdoMSSGDHV.pre_tax_payroll_deduction_flag = ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.premium_conversion_indicator_flag;

            }
        }
        //PIR 18043 – In ANNE wizard we display the existing amounts for Supp, Dep Supp, and Spouse Supp.  We need to do the same for the Regular wizard
        public void LoadExistingLifeInsuranceValues()
        {
            if (ibusMSSPersonAccountLife.IsNull())
                LoadMSSPersonAccountLife();

            if (ibusMSSPersonAccountLife.iclbLifeOption.IsNotNull() && ibusMSSPersonAccountLife.iclbLifeOption.Count > 0)
            {
                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount = 0;
                foreach (busPersonAccountLifeOption lobjOption in ibusMSSPersonAccountLife.iclbLifeOption)
                {
                    if (lobjOption.icdoPersonAccountLifeOption.coverage_amount > 0)
                    {
                        lobjOption.icdoPersonAccountLifeOption.effective_start_date = ibusMSSPersonAccountLife.icdoPersonAccount.history_change_date;
                        // Supplemental amount should be displayed with the addition of Basic coverage amount
                        if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental
                            || lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Basic)
                        {
                            //PIR 24313 
                            if (idecSupplementalAmount > 0)
                            {
                                ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_amount += lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                            }
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.supplemental_waiver_flag = busConstant.Flag_No;
                        }
                        if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental)
                        {
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_coverage_option_value =
                                Convert.ToString(Convert.ToInt32(lobjOption.icdoPersonAccountLifeOption.coverage_amount));
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.dependent_waiver_flag = busConstant.Flag_No;
                        }
                        if (lobjOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                        {
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_amount = lobjOption.icdoPersonAccountLifeOption.coverage_amount;
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.spouse_supplemental_waiver_flag = busConstant.Flag_No;
                        }
                        if ((icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.AnnualEnrollment) || (icdoWssPersonAccountEnrollmentRequest.reason_value != busConstant.AnnualEnrollment && !(ibusMSSPersonAccountLife.IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days()))) //PIR 25221
                            ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction = ibusMSSPersonAccountLife.icdoPersonAccountLife.premium_conversion_indicator_flag;
                    }
                }
            }

        }

        //PIR 18347
        public void SetReimbursementAmounts()
        {
            if (msra_effective_start_date != DateTime.MinValue)
            {
                DataTable ldtbMSRA = iobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", "code_id = 416");
                foreach (DataRow dr in ldtbMSRA.Rows)
                {
                    if (Convert.IsDBNull(dr["data2"]))
                        dr["data2"] = DateTime.MaxValue;
                    if (busGlobalFunctions.CheckDateOverlapping(msra_effective_start_date, Convert.ToDateTime(dr["data1"]), Convert.ToDateTime(dr["data2"])))
                    {
                        idecMSRA = Convert.ToDecimal(dr["data3"]);
                    }
                }
            }
            if (dcra_effective_start_date != DateTime.MinValue)
            {
                DataTable ldtbDCRA = iobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", "code_id = 417");
                foreach (DataRow dr in ldtbDCRA.Rows)
                {
                    if (Convert.IsDBNull(dr["data2"]))
                        dr["data2"] = DateTime.MaxValue;
                    if (busGlobalFunctions.CheckDateOverlapping(dcra_effective_start_date, Convert.ToDateTime(dr["data1"]), Convert.ToDateTime(dr["data2"])))
                    {
                        idecDCRA = Convert.ToDecimal(dr["data3"]);
                        idecDCRA1 = idecDCRA / 2;
                    }
                }
            }
        }
		
		public void InsertIntoEnrollmentData()
        {
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonAccountEmploymentDetail.IsNull())
                LoadPersonAccountEmploymentDetail();
            busEnrollmentData lobjEnrollmentData = new busEnrollmentData();
            lobjEnrollmentData.icdoEnrollmentData = new doEnrollmentData();

            lobjEnrollmentData.icdoEnrollmentData.source_id = ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.person_account_employment_dtl_id;
            lobjEnrollmentData.icdoEnrollmentData.plan_id = ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.plan_id;
            lobjEnrollmentData.icdoEnrollmentData.ssn = ibusPerson.icdoPerson.ssn;
            lobjEnrollmentData.icdoEnrollmentData.ndpers_member_id = ibusPerson.icdoPerson.person_id;
            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_id = ibusPerson.icdoPerson.peoplesoft_id;
            lobjEnrollmentData.icdoEnrollmentData.employer_org_id = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
            lobjEnrollmentData.icdoEnrollmentData.employment_type_value = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value;
            lobjEnrollmentData.icdoEnrollmentData.start_date = ibusPersonAccountEmploymentDetail.icdoPersonAccountEmploymentDetail.modified_date;
            lobjEnrollmentData.icdoEnrollmentData.monthly_premium = 0.0M;
            lobjEnrollmentData.icdoEnrollmentData.is_benefit_enrollment_report_generated = busConstant.Flag_No;
            lobjEnrollmentData.icdoEnrollmentData.peoplesoft_file_sent_flag = busConstant.Flag_Yes;
            lobjEnrollmentData.icdoEnrollmentData.Insert();
        }
        #region PIR-19997
        public string istrAcknowledgementTextForHDHP { get; set; }
        public string istrPreTaxHSA { get; set; }
        public string istrPreTaxHSADisplay { get; set; }

        public decimal idecMaxContibutionamount { get; set; }
        public string istrAcknowledgementTextForHDHP2 { get; set; }
        public busPersonAccountGhdvHsa ibusMSSPersonAccountGHDVHsa { get; set; }
        public string istrIsHDHPAcknowledged { get; set; }
        public Collection<busPersonAccountGhdvHsa> iclbPersonAccountGhdvHsaDetail { get; set; }
        busBase ibusbase = new busBase();
        public bool iblnIsEnrollInHealthAsTemporary { get; set; }
        public void LoadPersonAccountGHDVHsa()
        {
            if (ibusMSSPersonAccountGHDVHsa.IsNull())
                ibusMSSPersonAccountGHDVHsa = new busPersonAccountGhdvHsa { icdoPersonAccountGhdvHsa = new doPersonAccountGhdvHsa() };
            DataTable ldtResult = Select("cdoPersonAccountGhdvHsa.LoadPersonAccountGHDVHSA", new object[1] { ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id });
            iclbPersonAccountGhdvHsaDetail = ibusbase.GetCollection<busPersonAccountGhdvHsa>(ldtResult, "icdoPersonAccountGhdvHsa");
        }
        public bool IsMemberHasHDHPPlan()
        {
            if (ibusMSSPersonAccountGHDV.IsNull())
                LoadPersonAccountGHDV();
            if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                ibusMSSPersonAccountGHDV.LoadOrgPlan(icdoWssPersonAccountEnrollmentRequest.date_of_change);
            else
                ibusMSSPersonAccountGHDV.LoadOrgPlan(DateTime.Now.GetFirstDayofNextMonth());
            return (ibusMSSPersonAccountGHDV.ibusOrgPlan.icdoOrgPlan.hsa_pre_tax_agreement == "Y") ? true : false;
        }
        public bool IsHDHPAndHsaPanel()
        {
            bool lblnStepVisible = true;            
            if (ibusMSSPersonAccountGHDV.IsNull())
                LoadPersonAccountGHDV();
            if (icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                ibusMSSPersonAccountGHDV.LoadOrgPlan(icdoWssPersonAccountEnrollmentRequest.date_of_change);
            else
                ibusMSSPersonAccountGHDV.LoadOrgPlan(DateTime.Now.GetFirstDayofNextMonth());
            
            lblnStepVisible = istrPreTaxHSA == busConstant.Flag_Yes ? true : icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date != DateTime.MinValue ? true: false;
            return (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth && lblnStepVisible && ibusMSSPersonAccountGHDV.ibusOrgPlan.icdoOrgPlan.hsa_pre_tax_agreement == "Y") ? true : false;
        }

        //3.	‘Contribution Amount cannot exceed[Amount from Code ID 418, Code Value where Member Contribution Start Date between Data1 and Data2]’ 10370
        public bool IsContributionAmountExceeds()
        {           
            DataTable ldtResult = Select<cdoCodeValue>(new string[1] { "CODE_ID"},
                                                        new object[1] { 418}, null, null);

            if (ldtResult.Rows.Count > 0)
            {
                foreach (DataRow ldrCodeGroup in ldtResult.Rows)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date, 
                        Convert.ToDateTime(ldrCodeGroup["DATA1"]),
                        (Convert.IsDBNull(ldrCodeGroup["DATA2"]) ? DateTime.MaxValue : Convert.ToDateTime(ldrCodeGroup["DATA2"])))
                      && (icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_amount > Convert.ToDecimal(ldrCodeGroup["DATA3"])))
                    {
                        idecMaxContibutionamount = Convert.ToDecimal(ldrCodeGroup["DATA3"]);
                        return true;
                    }
                }
            }
            return false;
        }
        //5.	‘Contribution Start Date cannot be earlier then the first day of the month following your election’. 
        // I.e. Change Date is 4/18 then the Contribution Start Date cannot be earlier than 5/1.
        public bool ContributionStartDateCannotBeEarlier()
        {
            DateTime ldteDateofChange = GetDateofChange();
            return (icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date != DateTime.MinValue && icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date <= ldteDateofChange) ? true : false;
        }
       
        //7.	Date must be the first of the month(it always be 1st on month
        public bool IsContributionStartDateIsFirstDayOfMonth()
        {
            return (icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date != DateTime.MinValue && icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date != icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date.GetFirstDayofCurrentMonth()) ? true : false;
        }
   
        private busPersonAccountGhdvHsa CreatePersonAccountGHDVHSA()
        {
            busPersonAccountGhdvHsa lobjPersonAccountGHDVHsa = new busPersonAccountGhdvHsa
            {
                icdoPersonAccountGhdvHsa = new doPersonAccountGhdvHsa()
            };
            lobjPersonAccountGHDVHsa.icdoPersonAccountGhdvHsa.contribution_amount = icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_amount;
            lobjPersonAccountGHDVHsa.icdoPersonAccountGhdvHsa.contribution_start_date = icdoWssPersonAccountEnrollmentRequest.wss_hsa_contribution_start_date;
            if (lobjPersonAccountGHDVHsa.FindPersonAccount(ibusMSSPersonAccountGHDV.icdoPersonAccount.person_account_id))
            {
                if (lobjPersonAccountGHDVHsa.FindPersonAccountGHDV(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id))
                {
                    lobjPersonAccountGHDVHsa.iblnIsFromMSS = true;
                    lobjPersonAccountGHDVHsa.LoadPersonAccount();
                    lobjPersonAccountGHDVHsa.LoadPersonAccountGHDV();
                    lobjPersonAccountGHDVHsa.ValidateHardErrors(utlPageMode.All);
                    if (lobjPersonAccountGHDVHsa.iarrErrors.Count > 0)
                    {
                        return lobjPersonAccountGHDVHsa;
                    }
                    else
                    {
                        lobjPersonAccountGHDVHsa.PersistChanges();
                        lobjPersonAccountGHDVHsa.iblnIsFromMSSForEnrollmentData = true;
                        lobjPersonAccountGHDVHsa.AfterPersistChanges();
                    }
                }
            }
            return lobjPersonAccountGHDVHsa;
        }

        #endregion PIR-19997

        //wfmDefault.aspx file code conversion - btn_OpenPDF method 
        public string istrDownloadFileName { get; set; }
        public string lstrFilePath = string.Empty;
        public override void ProcessWizardData(utlWizardNavigationEventArgs we, string astrWizardName, string astrWizardStepName)
        {
            if (lstrFilePath == string.Empty)
            {
                DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'CorrPdf'");
                lstrFilePath = ldtbPathData.Rows[0]["path_value"].ToString();
            }
            if (iobjPassInfo.istrFormName == "wfmLifeEnrollmentWizard" && (we.istrNextStepID == "wzsStep2" || we.istrNextStepID == "wzsStep3" || we.istrNextStepID == "wzsStep4")
                || iobjPassInfo.istrFormName == "wfmLifeAnnualEnrollmentWizard" && (we.istrNextStepID == "wzsStep2" || we.istrNextStepID == "wzsStep3" || we.istrNextStepID == "wzsStep4"))
            {
                istrDownloadFileName = lstrFilePath + "ING_EOI.pdf";
            }
            else if ((iobjPassInfo.istrFormName == "wfmLifeEnrollmentWizard" && we.istrNextStepID == "wzsStep5")
                || (iobjPassInfo.istrFormName == "wfmLifeAnnualEnrollmentWizard" && (we.istrNextStepID == "wzsStep6" || we.istrNextStepID == "wzsIntro")))
            {
                istrDownloadFileName = lstrFilePath + "SFN-53855.pdf";
            }
            EnrollmentStep = we.istrNextStepID;
            base.ProcessWizardData(we, astrWizardName, astrWizardStepName);
        }


        /// <summary>
        /// PIR-18961 
        /// </summary>
        /// <returns></returns>
        public bool IsMemberEligibleToEnrollInDentalVisionPlan()
        {
            Boolean lblnIsMemberEligible = true;
            if (ibusPerson.icolPersonEmployment.IsNull())
                ibusPerson.LoadPersonEmployment();
            busPersonEmploymentDetail lbusCurrentPersonEmpDetail = new busPersonEmploymentDetail();
            lbusCurrentPersonEmpDetail.FindPersonEmploymentDetail(this.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id); // (ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id);
            lbusCurrentPersonEmpDetail?.LoadPersonEmployment();
            if (lbusCurrentPersonEmpDetail.ibusPersonEmployment.ibusOrganization.IsNull())
                lbusCurrentPersonEmpDetail.ibusPersonEmployment.LoadOrganization();
            if (lbusCurrentPersonEmpDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState &&
                (ibusPerson.icolPersonEmployment.Count > 1)) // need to check the below validation only if member has prior employments
            {
                busPersonEmployment lbusPersonEmployment = ibusPerson.icolPersonEmployment.FirstOrDefault(emp => emp.icdoPersonEmployment.end_date != DateTime.MinValue);
                if (lbusPersonEmployment.IsNotNull())
                {
                        if (lbusPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState && 
                           (Math.Abs(busGlobalFunctions.DateDiffInDays(lbusCurrentPersonEmpDetail.icdoPersonEmploymentDetail.start_date, lbusPersonEmployment.icdoPersonEmployment.end_date)) <= 30))  // PIR 23089
                    {
                        if (ibusPersonAccount?.icdoPersonAccount.person_account_id > 0)
                        {
                            DateTime ldtePrevEndDate = DateTime.MinValue;
                            if (lbusPersonEmployment.IsNotNull())
                            {
                                ldtePrevEndDate = lbusPersonEmployment.icdoPersonEmployment.end_date;
                            }
                            if (ldtePrevEndDate != DateTime.MinValue)
                            {
                                busPersonAccountGhdv lbusPersonAccountGhdv = new busPersonAccountGhdv() { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                                if (lbusPersonAccountGhdv.FindGHDVByPersonAccountID(ibusPersonAccount.icdoPersonAccount.person_account_id))
                                {
                                    busPersonAccountGhdvHistory lbusPersonAccountGhdvHistory = lbusPersonAccountGhdv.LoadHistoryByDate(ldtePrevEndDate);
                                    if (lbusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                                    {
                                        if (Math.Abs(busGlobalFunctions.DateDiffInDays(lbusCurrentPersonEmpDetail.ibusPersonEmployment.icdoPersonEmployment.start_date, DateTime.Now)) > 30)
                                        {
                                            lblnIsMemberEligible = false;
                                        }
                                    }
                                    else
                                        lblnIsMemberEligible = false;
                                }
                            }
                        }
                        else
                            lblnIsMemberEligible = false;
                    }
                }
            }
            return lblnIsMemberEligible;
        }

        public ArrayList btnReloadCoverageCodeList_Click()
        {
            if (icdoWssPersonAccountEnrollmentRequest.wss_ben_app_id > 0)
            {
                ibusMSSPersonAccountGHDV?.btnReloadCoverageCodeList_Click();
            }
            ArrayList larrlist = new ArrayList();
            larrlist.Add(this);
            return larrlist;
        }
        public Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> LoadMssCoverageCodeForAppWizard()
        {
            Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> lclbCoverageRef = new Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>();
            Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef> lclbFinalCoverageRef = new Collection<cdoOrgPlanGroupHealthMedicarePartDCoverageRef>();
            lclbCoverageRef = LoadMssCoverageCode();
            foreach (cdoOrgPlanGroupHealthMedicarePartDCoverageRef lcdoCoverageRef in lclbCoverageRef)
            {
                if (lcdoCoverageRef.coverage_code == icdoMSSGDHV.coverage_code || !string.IsNullOrEmpty(ibusMSSPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code))
                    lclbFinalCoverageRef.Add(lcdoCoverageRef);
            }
            return lclbFinalCoverageRef;
        }

        //PIR 24329
        public string istrDentalProviderName
        {
            get
            {
                string lstrDentalProviderName = Convert.ToString(DBFunction.DBExecuteScalar("entWssPersonAccountEnrollmentRequest.LoadDentalProviderName", new object[0]{ }, iobjPassInfo.iconFramework,
                                                iobjPassInfo.itrnFramework));

                return string.IsNullOrEmpty(lstrDentalProviderName) ? string.Empty : lstrDentalProviderName;
            }
        }

        public bool ExceptionLOCForAutoPost
        {
            get
            {
                if (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision)
                {
                    if (ibusPersonAccount.IsNull()) LoadPersonAccount();
                    if (ibusMSSPersonAccountGHDV.IsNull()) LoadPersonAccountGHDV();
                    busPersonAccountGhdvHistory lobjPersonAccountGhdvHistory = ibusMSSPersonAccountGHDV?.LoadHistoryByDate(icdoWssPersonAccountEnrollmentRequest.date_of_change);
                    
                    if (icdoAutoPostingCrossRef.IsNull()) FindAutoPostingCrossRef();
                    if (icdoAutoPostingCrossRef?.auto_post_flag == busConstant.Flag_Yes
                        && (icdoAutoPostingCrossRef.change_reason_value == busConstant.ReasonValueMarriage
                         || icdoAutoPostingCrossRef.change_reason_value == busConstant.ReasonValueRemoveDependent)
                        && icdoMSSGDHV?.level_of_coverage_value != lobjPersonAccountGhdvHistory?.icdoPersonAccountGhdvHistory?.level_of_coverage_value
                        )
                    {
                        return false;   //icdoAutoPostingCrossRef.auto_post_flag == busConstant.Flag_No;
                    }
                }

                if (IsghdvPlan && icdoWssPersonAccountEnrollmentRequest.reason_value == busConstant.ChangeReasonAnnualEnrollment)
                {
                    if (iclbMSSPersonDependent.IsNull())
                        LoadDependents();
                    if (iclbMSSPersonDependent.Any(objwssPersonDependent => (objwssPersonDependent.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipGrandChild
                        || objwssPersonDependent.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipLegalGuardian
                        || objwssPersonDependent.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipAdoptiveChild
                        || objwssPersonDependent.icdoWssPersonDependent.relationship_value == busConstant.DependentRelationshipDisabledChild)
                        && objwssPersonDependent.icdoWssPersonDependent.current_plan_enrollment_option_value == busConstant.PlanOptionStatusValueEnrolled
                        && objwssPersonDependent.icdoWssPersonDependent.target_person_dependent_id.IsNull()))
                    {
                        //icdoWssPersonAccountEnrollmentRequest.status_value = busConstant.EnrollRequestStatusPendingRequest;
                        return false;
                    }
                }
                return true;
            }
        }
        public bool IsHealthWithTemporaryEmployee
        {
            get
            {
                if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary && icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupHealth)
                    return true;
                return false;
            }
        }
 		public bool IsMemberEligibleToEnrollInCurrentYear()
        {
           if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            if (ibusPersonAccount.IsNotNull() && icdoWssPersonAccountEnrollmentRequest.person_id > 0)
            {
                ibusPersonAccount.icdoPersonAccount.person_id = icdoWssPersonAccountEnrollmentRequest.person_id;
                ibusPersonAccount.icdoPersonAccount.plan_id = icdoWssPersonAccountEnrollmentRequest.plan_id;
            }
           bool lblnCurrentPreTaxFlag = ((icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdDental || icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdVision) &&
                                          icdoMSSGDHV.IsNotNull() && icdoMSSGDHV.pre_tax_payroll_deduction_flag == busConstant.Flag_Yes) || (icdoWssPersonAccountEnrollmentRequest.plan_id == busConstant.PlanIdGroupLife && 
                                          ibusMSSLifeOption.IsNotNull() && ibusMSSLifeOption.icdoWssPersonAccountLifeOption.IsNotNull() && ibusMSSLifeOption.icdoWssPersonAccountLifeOption.pre_tax_payroll_deduction == busConstant.Flag_Yes) ? true : false;
           return ibusPersonAccount.IsNotNull() && ibusPersonAccount.IsMemberEligibleToEnrollInCurrentYear(icdoWssPersonAccountEnrollmentRequest.date_of_change, icdoWssPersonAccountEnrollmentRequest.reason_value, lblnCurrentPreTaxFlag, 
                                                                                                            icdoMSSFlexCompOption.IsNotNull() ? icdoMSSFlexCompOption.medical_annual_pledge_amount : 0, icdoMSSFlexCompOption.IsNotNull() ? icdoMSSFlexCompOption.dependent_annual_pledge_amount : 0);
        }

        // PIR 25345
        public bool IsMemberEmplChangeFromTempToPerm()
        {
            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();

            ibusPersonEmploymentDetail.ibusPersonEmployment.LoadPersonEmploymentDetail();            
            var lcolPersonEmploymentDetail = (from lobjPersonEmploymentDetail in ibusPersonEmploymentDetail.ibusPersonEmployment.icolPersonEmploymentDetail
                        where lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id == ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id
                            orderby lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date descending
                            select lobjPersonEmploymentDetail).Take(2).ToList();
            if (lcolPersonEmploymentDetail.Count > 1 &&
                lcolPersonEmploymentDetail[0].icdoPersonEmploymentDetail.end_date == DateTime.MinValue && lcolPersonEmploymentDetail[0].icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent &&
                lcolPersonEmploymentDetail[1].icdoPersonEmploymentDetail.end_date != DateTime.MinValue  && lcolPersonEmploymentDetail[1].icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary)
                return true;
            else
                return false;
        }
		
        public void GetCoverageAmountDetailsForDisplay()
        {
            string lstrlifeInsurancetypeValue = null;
            DataTable ldtbEndDate = Select("cdoWssPersonAccountEnrollmentRequest.GetEndDateForEmploymentDetailId", new object[1] { icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id });
            if (ldtbEndDate.Rows.Count > 0)
            {
                LoadPersonAccount();

                ibusPersonAccount.LoadPersonAccountLife();
                if (ibusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value.IsNull() && (Convert.ToDateTime(ldtbEndDate.Rows[0]["END_DATE"]).Date == new DateTime(1753, 01, 01).Date))
                    lstrlifeInsurancetypeValue = busConstant.LifeInsuranceTypeActiveMember;
                else
                    lstrlifeInsurancetypeValue = ibusPersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value;

                if (icdoWssPersonAccountEnrollmentRequest.date_of_change != DateTime.MinValue)
                {
                    DataTable ldtbCovergeAmountSupp = Select("cdoWssPersonAccountEnrollmentRequest.GetValidCoverageAmountForDisplay", new object[3] { busConstant.LevelofCoverage_Supplemental, lstrlifeInsurancetypeValue, icdoWssPersonAccountEnrollmentRequest.date_of_change });
                    if (ldtbCovergeAmountSupp.Rows.Count > 0)
                    {
                        idecSuppGILimit = Convert.ToDecimal(ldtbCovergeAmountSupp.Rows[0]["GI_LIMIT"]);
                        idecSuppPreTaxLimit = Convert.ToDecimal(ldtbCovergeAmountSupp.Rows[0]["PRE_TAX_LIMIT"]);
                        idecSuppIncrement = Convert.ToDecimal(ldtbCovergeAmountSupp.Rows[0]["INCREMENT"]);
                        idecSuppIncrementLimit = Convert.ToDecimal(ldtbCovergeAmountSupp.Rows[0]["INCREMENT_LIMIT"]);
                    }

                    DataTable ldtbCovergeAmountSuppSpouse = Select("cdoWssPersonAccountEnrollmentRequest.GetValidCoverageAmountForDisplay", new object[3] { busConstant.LevelofCoverage_SpouseSupplemental, lstrlifeInsurancetypeValue, icdoWssPersonAccountEnrollmentRequest.date_of_change });
                    if (ldtbCovergeAmountSuppSpouse.Rows.Count > 0)
                    {
                        idecSpouseSuppGILimit = Convert.ToDecimal(ldtbCovergeAmountSuppSpouse.Rows[0]["GI_LIMIT"]);
                        idecSpouseSuppIncrement = Convert.ToDecimal(ldtbCovergeAmountSuppSpouse.Rows[0]["INCREMENT"]);
                        idecSpouseSuppPercentage = Convert.ToDecimal(ldtbCovergeAmountSuppSpouse.Rows[0]["COVERAGE_LIMIT_PERCENTAGE"]);
                    }
                }
            }
        }
        /// <summary>
        /// return if DC25 plan is selection contribution to carry forward to new enrollment
        /// </summary>
        /// <returns></returns>
        public bool IsAdditionalSelectionMade()
        {
            int lintEmploymentDaysDiff;
            bool lblnReturnTrue = false;
            if (ibusPersonAccount == null) LoadPersonAccount();
            if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
            DateTime ldtAddlEEContributionPercentEndDate = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ?
                ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent_temp_end_date : ibusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent_end_date;
            if (ldtAddlEEContributionPercentEndDate == DateTime.MinValue)
            {
                lintEmploymentDaysDiff = busGlobalFunctions.DateDiffInDays(ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, busGlobalFunctions.GetSysManagementBatchDate());
                lblnReturnTrue = lintEmploymentDaysDiff < (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary ? 180 : 30);
            }
            else
            {
                lblnReturnTrue = busGlobalFunctions.GetSysManagementBatchDate() < ldtAddlEEContributionPercentEndDate;
            }
            if (lblnReturnTrue == false)
                return true;
            return iblnIsDC25PlanSelectionCarryForward;
        }
        public void LoadPreviousAdditionalContributionPercent()
        {
            iblnIsDC25PlanSelectionCarryForward = false;
            if (IsAdditionalSelectionValuesAvailable())
            {
                if (ibusPersonAccount == null) LoadPersonAccount();

                //if (IsMemberEmplChange())
                {
                    if (ibusPersonAccount.IsNotNull())
                    {
                        DataTable ldtPersonAccount = Select<cdoPersonAccount>(new string[1] { enmPersonAccount.person_account_id.ToString() },
                                       new object[1] { ibusPersonAccount.icdoPersonAccount.person_account_id }, null, null);
                        if (ldtPersonAccount.IsNotNull() && ldtPersonAccount.Rows.Count > 0)
                        {
                            if (IsTemporaryEmployee && ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()] != DBNull.Value)
                            {
                                iintOldAddlEEContributionPercent = Convert.ToInt32(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()].ToString());
                                iintOldAddlEEContributionPercent = iintOldAddlEEContributionPercent == 0 ? -1 : iintOldAddlEEContributionPercent;
                                //iintAddlEEContributionPercent = iintOldAddlEEContributionPercent;
                                iblnIsDC25PlanSelectionCarryForward = true;
                            }
                            else if (!IsTemporaryEmployee && ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()] != DBNull.Value)
                            {
                                iintOldAddlEEContributionPercent = Convert.ToInt32(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()].ToString());
                                iintOldAddlEEContributionPercent = iintOldAddlEEContributionPercent == 0 ? -1 : iintOldAddlEEContributionPercent;
                                //iintAddlEEContributionPercent = iintOldAddlEEContributionPercent;
                                iblnIsDC25PlanSelectionCarryForward = true;
                            }
                        }
                        else
                        {
                            busPersonAccount lbusPersonAccount = ibusPerson.icolPersonAccount.Where(
                                                                    i => i.icdoPersonAccount.plan_id == icdoWssPersonAccountEnrollmentRequest.plan_id &&
                                                                    i.IsRetiredOrWithdrwan()
                                                            ).FirstOrDefault();
                            if (lbusPersonAccount.IsNotNull())
                            {
                                ldtPersonAccount = Select<cdoPersonAccount>(new string[1] { enmPersonAccount.person_account_id.ToString() },
                                       new object[1] { lbusPersonAccount.icdoPersonAccount.person_account_id }, null, null);
                                if (ldtPersonAccount.IsNotNull() && ldtPersonAccount.Rows.Count > 0)
                                {
                                    if (IsTemporaryEmployee && ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()] != DBNull.Value)
                                    {
                                        iintOldAddlEEContributionPercent = Convert.ToInt32(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent_temp.ToString()].ToString());
                                        iintOldAddlEEContributionPercent = iintOldAddlEEContributionPercent == 0 ? -1 : iintOldAddlEEContributionPercent;
                                        //iintAddlEEContributionPercent = iintOldAddlEEContributionPercent;
                                        iblnIsDC25PlanSelectionCarryForward = true;
                                    }
                                    else if (!IsTemporaryEmployee && ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()] != DBNull.Value)
                                    {
                                        iintOldAddlEEContributionPercent = Convert.ToInt32(ldtPersonAccount.Rows[0][enmPersonAccount.addl_ee_contribution_percent.ToString()].ToString());
                                        iintOldAddlEEContributionPercent = iintOldAddlEEContributionPercent == 0 ? -1 : iintOldAddlEEContributionPercent;
                                        //iintAddlEEContributionPercent = iintOldAddlEEContributionPercent;
                                        iblnIsDC25PlanSelectionCarryForward = true;
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }
        public bool IsAdditionalSelectionValuesAvailable()
        {
            Collection<cdoCodeValue> lclbEligibleADECAmountValue = new Collection<cdoCodeValue>();
            lclbEligibleADECAmountValue = LoadADECAmountValues();
            if (lclbEligibleADECAmountValue.IsNotNull() && lclbEligibleADECAmountValue.Count > 0)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// return if DC25 plan is Employment Detail in election 
        /// </summary>
        /// <returns></returns>
        public bool IsEmploymentLinkedWithDC25Plan
        {
            get
            {
                //DataTable ldtbList = Select<cdoPersonAccountEmploymentDetail>(new string[2] { "plan_id", "PERSON_EMPLOYMENT_DTL_ID" },
                //    new object[2] { busConstant.PlanIdDC2025, icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id }, null, null);
                DataTable ldtbList = Select<cdoPersonAccount>(new string[3] { "plan_id", "PERSON_ID" ,"PLAN_PARTICIPATION_STATUS_VALUE"},
                    new object[3] { busConstant.PlanIdDC2025, icdoWssPersonAccountEnrollmentRequest.person_id, busConstant.PlanParticipationStatusRetirementEnrolled }, null, null);
                if (ldtbList.Rows.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }
        /// <summary>
        /// Loading Additional contribution amount 0, null and any number
        /// </summary>
        /// <returns></returns>
        public void LoadAdditionalEEContribution()
        {
            //if (!IsDC25PlanRefundWithdrawnRetired())
            {
                int lintAdditionalContribution = 0;
                string lstrFieldForAddlContribution= string.Empty;
                if (IsTemporaryEmployee)
                {
                    lintAdditionalContribution = icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent_temp;
                    lstrFieldForAddlContribution = enmWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent_temp.ToString();
                }
                else
                {
                    lintAdditionalContribution = icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent;
                    lstrFieldForAddlContribution = enmWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent.ToString();
                }
                if (lintAdditionalContribution > 0)
                    iintAddlEEContributionPercent = lintAdditionalContribution;
                else
                {
                    DataTable ldtLoadAdditionalContributionAmount = Select<cdoWssPersonAccountEnrollmentRequest>(new string[1] { enmWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id.ToString() },
                                           new object[1] { icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
                    if (ldtLoadAdditionalContributionAmount.IsNotNull() && ldtLoadAdditionalContributionAmount.Rows.Count > 0
                        && Convert.ToString(ldtLoadAdditionalContributionAmount.Rows[0][lstrFieldForAddlContribution]).IsNullOrEmpty())
                    {
                        iblnMSSShowAddlEEContributionPercent = false;
                    }
                    else
                    {
                        iblnMSSShowAddlEEContributionPercent = true;
                        iintAddlEEContributionPercent = -1;
                        if(icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent == -1)
                            icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent = 0;
                        if (icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent_temp == -1)
                            icdoWssPersonAccountEnrollmentRequest.addl_ee_contribution_percent_temp = 0;
                    }
                }
            }
            iblnMSSShowAddlEEContributionPercent = false;
        }

        public bool IsCurrentEmployerProvidesFlexDuringSameCalenderYear(DateTime adtmDateOfChange)
        {
            DataTable ldtIsCurrentEmployerProvidesFlex = busBase.Select("entPersonAccount.IsCurrentEmployerProvidesFlexInCurrentYear",
                                new object[2] { icdoWssPersonAccountEnrollmentRequest.person_id, adtmDateOfChange.Year });

            //If member switched to Org which doesnt provide Flex plan, only allowed to Post Tax.
            if (ldtIsCurrentEmployerProvidesFlex.IsNullOrEmpty() || ldtIsCurrentEmployerProvidesFlex.Rows.Count == 0)
                return false;
            return true;
        }
        public bool iblnIsMemberEligibleToSelectPreTaxProvider { get; set; }
        public string istrFlexIntroMessage
        {
            get
            {
                DateTime Now = DateTime.Now;
                DataTable ldtbListWSSAcknowledgement = iobjPassInfo.isrvDBCache.GetCacheData("sgt_wss_acknowledgement", "screen_step_value='FLIN'");
                busWssAcknowledgement lobjWssAcknowledgement = new busWssAcknowledgement();
                lobjWssAcknowledgement.icdoWssAcknowledgement = new cdoWssAcknowledgement();
                if (ldtbListWSSAcknowledgement.Rows.Count > 0)
                    lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text = ldtbListWSSAcknowledgement.Rows[0]["acknowledgement_text"].ToString();
                return lobjWssAcknowledgement.icdoWssAcknowledgement.acknowledgement_text;
            }
        }
        public bool iblnIsFlexIntroMessageVisible { get; set; }
        public void SetVisibilityToFlexIntroMessage()
        {
            LoadPlanEnrollmentOption();

            iblnIsFlexIntroMessageVisible = true;
            if (ibusMSSPersonAccountFlexComp.ibusPersonAccount.IsNull())
                ibusMSSPersonAccountFlexComp.LoadPersonAccount();

            DataTable ldtMemberWasEnrolledAsPreTax = busBase.Select("entPersonAccount.IsMemberWasEnrolledAsPreTax",
                                        new object[2] { busGlobalFunctions.GetSysManagementBatchDate().GetFirstDayofNextMonth().Year, ibusMSSPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_id });

            if (ibusMSSPersonAccountFlexComp.IsNotNull() && ibusMSSPersonAccountFlexComp.ibusPersonAccount.IsNotNull()
                && ibusMSSPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
            {
                iblnIsFlexIntroMessageVisible = false;
            }
            //PIR 26356 Existing Person Account and last enroll < 31 days, don’t allow enrollment, even when it is in the next calendar year.
            else if (ibusMSSPersonAccountFlexComp.IsNotNull() && ibusMSSPersonAccountFlexComp.ibusPersonAccount.IsNotNull()
                //If member had Pre Tax Y for Dental/Vision or Life; hide New Hire from Flex if within the same calendar year 
                && (ldtMemberWasEnrolledAsPreTax.IsNullOrEmpty() || ldtMemberWasEnrolledAsPreTax.Rows.Count == 0)
                && ibusMSSPersonAccountFlexComp.ibusPersonAccount.icdoPersonAccount.person_account_id > 0
                //this checks if employment gap is > 31 days
                && ibusMSSPersonAccountFlexComp.ibusPersonAccount.IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days()
                //if employment gap > 31 days then look if member is enrolled in Flex for current year
                && !(ibusMSSPersonAccountFlexComp.IsFlexCompEnrolledInCurrentYear(busGlobalFunctions.GetSysManagementBatchDate().GetFirstDayofNextMonth().Year)))
            {
                iblnIsFlexIntroMessageVisible = false;
            }
        }
        public bool iblnIsChangeReasonDropDownVisible { get; set; }
    }
}
