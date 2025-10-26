using System;
using System.Data;
using System.Collections.Generic;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Reflection;
using Sagitec.DataObjects;
using System.Text;
using System.Collections.Specialized;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public static class busConstant
    {
        #region Requirement

        public const string RequirementTypeValueChild = "CHLD";
        public const string RequirementTypeValueParent = "PAR";
        public const string ReportNameRequirementHistoryAsOfDate = "rptRequirementHistoryAsOfDate";

        #endregion

        #region Common

        public const string Flag_Yes = "Y";
        public const string Flag_No = "N";
        public const string Flag_System = "S";
        //uat pir 1254  
        public const string BenefitPaymentChangeGroupby = "G";

        public const string Flag_Yes_Value = "Yes";
        public const string Flag_No_Value = "No";
        public const string Flag_Yes_CAPS = "YES";
        public const string Flag_No_CAPS = "NO";
        public const string Flag_On = "1";
        public const string Flag_Off = "0";

        public const string StatusActive = "ACTV";
        public const string StatusInActive = "INAC";
        public const string StatusPending = "PEND";
        public const string StatusValid = "VALD";
        public const string StatusReview = "REVW";
        public const string StatusProcessed = "PROC";
        public const string StatusFailed = "FAIL";

        public const string DateTimeFormatYYMMDD = "yyMMdd_HHmmss";//PIR 14392
        public const string DateTimeFormatYYYYMMDD = "yyyyMMdd_HHmmss";//PIR 14915
        public const string DateFormat = "MMddyy_HHmmss";
        public const string DateFormatMMddyyyy = "MM/dd/yyyy";
        public const string DateFormatMMMyyyy = "MMM, yyyy";
        public const string DateFormatddMMMyyyy = "dd_MMM_yyyy";
        public const string DateFormatD8 = "yyyyMMdd";
        public const string DateFormatLongDate = "MMMM dd, yyyy"; // January 01, 0001
        public const string DateFormatMonthYear = "MMMM, yyyy"; // January, 0001
        public const string DateFormatMonth = "MMMM"; // January
        public const string DateFormatYear = "yyyy"; // 2012
        public const string DateTimeFormatMMMMddthyyyy = "MMMM dd\"th,\" yyyy";
        public const string DateFormatYearMonthDay = "yyyy-MM-dd";
        public const string DateFormatMMDDYYYY = "MM-dd-yyyy";
        public const string DateTimeFormatYYYY_MM_DD = "yyyy_MM_dd";

        public const string FileFormattxt = ".txt";
        public const string FileFormatcsv = ".csv";
        public const string FileFormatdat = ".dat";

        public const string ReportTableName = "ReportTable01";
        public const string ReportTableName02 = "ReportTable02";
        public const string ReportTableName03 = "ReportTable03";
        public const string ReportTableName04 = "ReportTable04";
        public const string ReportTableName05 = "ReportTable05";
        public const string ReportTableName06 = "ReportTable06";
        public const string ReportTableName07 = "ReportTable07";
        public const string ReportTableName08 = "ReportTable08";
        public const string ReportTableName09 = "ReportTable09";
        public const string ReportTableName10 = "ReportTable10";
        public const string ReportTableName11 = "ReportTable11";
        public const string ReportTableName12 = "ReportTable12";
        public const string ReportTableName13 = "ReportTable13";
        public const string ReportTableName14 = "ReportTable14";
        public const string ReportTableName15 = "ReportTable15";
        public const string ReportTableName16 = "ReportTable16";
        public const string ReportTableName17 = "ReportTable17";
        public const string ReportTableName18 = "ReportTable18";
        public const string ReportTableName19 = "ReportTable19";
        public const string ReportTableName20 = "ReportTable20";
        public const string ReportTableName21 = "ReportTable21";
        public const string ReportTableName22 = "ReportTable22";
        public const string ReportTableName23 = "ReportTable23";
        public const string ReportTableName24 = "ReportTable24";
        public const string ReportTableName25 = "ReportTable25";
        public const string ReportTableName26 = "ReportTable26";
        public const string ReportTableName27 = "ReportTable27";
        public const string ReportTableName28 = "ReportTable28";
        public const string ReportTableName29 = "ReportTable29";
        public const string ReportTableName30 = "ReportTable30";
        public const string ReportTableName31 = "ReportTable31";
        public const string ReportTableName32 = "ReportTable32"; //PIR 8837
        public const string ReportTableName33 = "ReportTable33"; //PIR 8837
        public const string ReportTableName34 = "ReportTable34"; //PIR 9790

        public const string NotApplicable = "N/A";
        public const int SystemConstantsAndVariablesCodeID = 52;
        public const string SystemConstantsLastEmployerPostingDate = "LEPD";
        public const string SystemConstantsLastIBSPostingDate = "LIPD";
        public const string SystemConstant_CentraPayrollOrg = "CEPA";
        public const string SystemConstant_GrandFatherDate = "GFDT";
        public const string ServicePurchaseCloseVariance = "SPCV"; //Service Purchase PIr-9224 (PIR-10439)

        public const string SubSystemValueEmployerReporting = "PAYR";
        public const string SubSystemValueConversion = "CONV";
        public const string SubSystemValueConversionOpeningBalance = "OPEN";
        public const string SubSystemInterestCredit = "INTR";
        public const string SubSystemValueAdjustment = "ADJS";
        public const string SubSystemValueServicePurchase = "PURC";
        public const string SubSystemValueTransfer = "ACTR";
        public const string SubSystemValueBenefitPayment = "PMNT";
        public const string SubSystemValuePensionRecv = "PRCB";
        public const string SubSystemPEP = "PEP";
        public const int SubSystemPEPId = 349;
        public const string PERSLinkBatchUser101 = "PERSLink Batch 101";
        public const string PERSLinkBatchUser = "PERSLink Batch";

        public const string PERSLinkServiceUser = "PERSLink Service";

        #endregion

        #region Pir
        public const string PirStatusReportBtnID = "btnSave";
        public const string PirStatusWorkInProgressBtnID = "btnWIP";
        public const string PirStatusDeployToSystemTestBtnID = "btnDeploy";
        public const string PirStatusRetestBtnID = "btnRetest";
        public const string PirStatusClosedBntID = "btnClose";
        //PIR 23372
        public const int USER_SERIAL_ID_AssignedToIT = 27;
        public const string PirStatusLogged = "RPRA";
        public const string PirStatusReported = "RPRR";
        public const string PirStatusWorkInProgress = "WIPR";
        public const string PirStatusDeployToSystemTest = "DEPL";
        public const string PirStatusRetest = "RTST";
        public const string PirStatusClosed = "CLOS";
        public const string PirPriorityMedium = "MEDM";
        public const string PirSeverityImportant = "IMPO";

        public const string PIRStatusReadyForSystest = "RSYS";
        public const string PIRStatusReadyForUAT = "RTST";
        public const string PIRStatusReadyForPROD = "CLOS";

        #endregion

        #region Person

        public const string BtnVerifyAndSaveClicked = "OnButtonVerifyAndSaveAddressClicked";
        public const string BtnRetrieveClicked = "OnButtonRetrieveAddressClicked";
        public const string BtnSaveClicked = "OnButtonSaveAddressClicked";
        public const string POA_RelationshipValue = "ATRN";
        public const string US_Code_ID = "0001";
        public const string Canada_Code_ID = "0036";
        public const string AddressTypePayment = "PAYM";
        public const string AddressTypePermanent = "PERM";
        public const string AddressTypeTemporary = "TEMP";
        public const string GenderTypeMale = "MALE";
        public const string GenderTypeFemale = "FEML";
        public const string NamePrefixMR = "MR";
        public const string NamePrefixMS = "MS";
        public const string NamePrefixMRS = "MRS";
        public const string NamePrefixMISS = "MISS";
        public const string PersonMaritalStatusMarried = "MRID";
        public const string PersonMaritalStatusSingle = "SNGL";
        public const string PersonMaritalStatusDivorced = "DVRC";
        public const string PersonMaritalStatusWidow = "WDOW";
        public const string PersonMaritalStatusMarriedDesc = "Married";
        public const string PersonMaritalStatusSingleDesc = "Single";
        public const string PersonMaritalStatusDivorcedDesc = "Divorced";
        public const string PersonMaritalStatusWidowDesc = "Widowed";
        public const string PersonContactTypePOA = "ATRN";
        public const string PersonContactTypeSpouse = "SPOU";
        public const string PersonContactTypeExSpouse = "EXPS";
        public const string PersonContactTypeChild = "CHLD";
        public const string PersonContactTypePrimaryContact = "PRCT";
        public const string PersonContactTypeGuardian = "GRDN";
        public const string PersonContactStatusActive = "ACT";
        public const string PersonContactStatusInActive = "INAC";
        public const string PersonJobTypePermanent = "PERM";
        public const string PersonJobTypeTemporary = "TEMP";
        public const string PersonJobClassClassifiedState = "CLAS"; // PIR 11891
        public const string PersonJobClassNonClassifiedState = "NCLS"; // PIR 11891
        public const string PersonJobClassStateElectedOfficial = "SEO";
        public const string PersonJobClassStateAppointedOfficial = "STAP"; // PIR 11891
        public const string PersonJobClassNonStateAppointedOfficial = "NSAP"; // Update Employement Wizard Visiblity Issues
        
        public const string PersonJobClassNonStateElectedOfficial = "NSEO";
        public const string PersonAccountEapInsuranceTypeRegular = "RE14";
        public const string EmploymentStatusContributing = "CONT";
        public const string EmploymentStatusNonContributing = "NCON";
        public const string EmploymentStatusTerminated = "TERM";
        public const string EmploymentStatusLOA = "LOA";
        public const string EmploymentStatusLOAM = "LOAM";
        public const string EmploymentStatusFMLA = "FMLA";//PIR 22835

        public const string PersonBeneficiaryRelationshipSpouse = "SPOU";
        public const string PersonBeneficiaryRelationshipChild = "CHLD";
        public const string PersonBeneficiaryRelationshipStepChild = "SCLD";
        public const string PersonBeneficiaryRelationshipAdoptedChild = "ADCH";
        public const string PersonBeneficiaryRelationshipDisabledChild = "DSCH";
        public const string PersonBeneficiaryRelationshipEstate = "ESTA";
        public const string PersonBeneficiaryRelationshipTrustee = "TRUS";
        public const string PersonBeneficiaryRelationshipExSpouse = "EXSP";

        // Person Account Adjustment
        public const int AdjustmentStatusCodeID = 352;
        public const string AdjustmentStatusValid = "VALD";
        public const string AdjustmentStatusPosted = "POST";

        // Person Account Transfer
        public const int TransferStatusCodeID = 353;
        public const string TransferStatusValid = "VALD";
        public const string TransferStatusPosted = "POST";

        // Person ACA Eligbility
        public const string PersonEmplyDetailType = "TEMP";
        public const string ACACertificationMethodNewHire = "NEWH";
        public const string ACACertificationMethodChangePosition = "CPHI";
        public const string ACACertificationMethodLookBack = "LBM";
        public const string ACACertificationLookBackTypeNew = "NEWE";
        public const string ACACertificationLookBackTypeAnnual = "ANNE";
        public const string WaiveReasonOther = "OTHR";
        public const string ACACertAnnualEnrollmentWindow = "ACAE";
        public const string ChangeReasonACAAnnualEnrollment = "ACAA";
        public const string ChangeReasonACAEligibleTemporary = "ACAT";
        // System Constants
        public const int SystemConstantCodeID = 52;
        public const int TaxOptionCodeID = 2218;
        public const string AddressStateTexas = "TX";
        public const string AddressStateUtah = "UT";

        //Parent Page identifier
        public const string PersonMaintenance = "PRSM";
        public const string DROApplicationMaintenance = "DROM";

        //for UCS-041
        public const string TransferTypeDBToDB = "DBDB";
        public const string TransferTypeMember = "MEMB";
        
        public const string CorTemplateNameSFN58871 = "SFN-58771";

        public const int RestrictionReasonValueCodeId = 1921; //PIR 20807

        public const string ChangeReasonValueINEM = "INEM"; //PIR 23734


        //new standard bookmark added for DC provider and phone number

        public const string CompanionAndDeferredCompData1 = "Empower";

        public const string CompanionAndDeferredCompData2 = "8668164400";

        public const string CompanionAndDeferredCompData3 = "https://www.empower.com/";

        public const string CPOR = "CPOR";

        public const string DCOR = "DCOR";



        #endregion

        #region Reports

        public const String ReportDateFormat = "MM/dd/yyyy";
        public const string ReportNameEmployerReceivableAgingReport = "rptEmployerReceivableAgingReport"; // PIR 8453
        public const string ReportNameMaritalStatusChangedRecords = "rptMaritalStatusChange";
        public const string ReportNameOverPaymentReport = "rptOverPaymentReport";
        public const string ReportNamePaymentListingReport = "rptPaymentListingReport"; //PIR 7190
        public const string ReportNamePensionPaymentHistory = "rptPensionPaymentHistory";
        public const string ReportNamePensionCheckPaymentReport = "rptPensionCheckPaymentReport";
        public const string ReportNameSummaryreportofalladhocpaymentsforaMonth = "rptSummaryreportofalladhocpaymentsforaMonth";
        public const string ReportNameListOfAppointments = "rptAppointments";
        public const string ReportNameDeferred3YrCatchUpEnding = "rptDeferred3YrCatchUpEnding";
        public const string ReportNameMedicareSplitError = "rptMedicareSplitErrorReport";
        public const string ReportMissingContributions = "rptMissingContributions";
        public const string ReportNameLeaveOfAbsence = "rptLeaveOfAbsence";
        public const string ReportNameMissingRetirementEnrollment = "rptMissingRetirementEnrollment";
        public const string ReportNamePopupPayeeAccount = "rptPopupPayeeAccount";
        public const string ReportNameHighRiskNewPayees = "rptHighRiskNewPayees";
        public const string ReportNameGLReportForAudit = "rptGLReportForAudit";
        public const string ReportNameContributionFileForAudit = "rptContributionFileForAudit";
        public const string ReportNameBenefitPaymentChangeDetails = "rptBenefitPaymentChangeDetailsReport";
        #endregion

        #region OrganizationStatus

        public const string OrganizationStatusActive = "ACTV";
        public const string OrganizationStatusDeclined = "DECL";
        public const string OrganizationStatusInactive = "IATV";
        public const string OrganizationStatusMerged = "MERG";
        public const string OrganizationStatusPending = "PEND";

        public const string OrganizationTypeEmployer = "EMPL";
        public const string OrganizationTypeProvider = "PRVD";
        public const string OrganizationTypeVendor = "VEND";
        public const string OrganizationTypeEstate = "EST";

        public const string OrganizationTypeBank = "BANK";//PIR 18503

        

        public const int MessageIdPrimaryAddress = 4045;

        #endregion

        #region Organization
        public const string OrgContactRolePrimaryAuthorizedAgent = "PAAG";
        public const string OrgContactRoleAgent = "AGNT";
        public const string OrgContactRoleOther = "OTHR";
        public const string OrgContactRoleAuthorizedAgent = "AUTH";
        public const string OrgContactRoleFinance = "FINA";
        public const string OrgContactRoleHumanResources = "HUMN";
        public const string OrgContactRolePayroll = "PAYR";
        public const string OrgContactRoleWellnessCoordinator = "WELC";
        public const string OrgTypeEmployer = "EMPL";
        public const string EmployerCategoryState = "STAT";
        public const string EmployerCategoryPoliticalSubDivisions = "PLSD";
        public const string EmployerCategoryDistrictHealthUnits = "DHSU";
        public const string EmployerCategorySchoolDistrict = "SCDS";
        public const string OrgTypeProvider = "PRVD";
        public const string PlanCodeEAP = "EAPP";
        public const string PlanCodeLTC = "LTCP";
        public const string PlanCodeGroupLife = "GRLF";
        public const string PlanCodeDental = "GRDT";
        public const string PlanCodeVision = "GRVS";
        public const string PlanCode457 = "DECM";
        public const string PlanCodeOther457 = "ODCM";
        public const string PlanCodeMedicarePartD = "MCPD";
        public const string PlanCodeGroupHealth = "GHLT";
        public const string PlanCodeHMO = "HMOP";
        public const string PlanCodeTIAA = "TIAA";
        public const string PlanCodeFlex = "FLEX";
        public const string PlanCodeTFFR = "TFFR";
        public const string PrimaryAddressYes = "Yes";
        public const string PrimaryAddressNo = "No";
        public const string DeffCompFrequencyMonthly = "MONT";
        public const string DeffCompFrequencySemiMonthly = "SEMI";
        public const string DeffCompFrequencyWeekly = "WKLY";
        public const string DeffCompFrequencyBiWeekly = "BWLK";
        public const string BankUsageDirectDeposit = "DEP";
        public const string BankUsageACHWithdrawals = "WID";
        public const int PlanIdMain = 1;
        public const int PlanIdLE = 2;
        public const int PlanIdNG = 3;
        public const int PlanIdHP = 4;
        public const int PlanIdJudges = 5;
        public const int PlanIdJobService = 6;
        public const int PlanIdDC = 7;
        public const int PlanIdOther457 = 8;
        public const int PlanIdMedicarePartD = 9;
        public const int PlanIdTIAA = 10;
        public const int PlanIdTFFR = 11;
        public const int PlanIdGroupHealth = 12;
        public const int PlanIdGroupLife = 13;
        public const int PlanIdEAP = 14;
        public const int PlanIdLTC = 15;
        public const int PlanIdDental = 16;
        public const int PlanIdVision = 17;
        public const int PlanIdFlex = 18;
        public const int PlanIdDeferredCompensation = 19;
        public const int PlanIdLEWithoutPS = 20;
        public const int PlanIdOasis = 21;
        public const int PlanIdHMO = 22;
        public const int PlanIdJobService3rdPartyPayor = 23;
        public const int PlanIdPriorJudges = 24;
        public const int PlanIdPriorService = 25;
        //PIR 25729
        public const int PlanIdStatePublicSafety = 29;
        //pir 7943
        public const int PlanIdBCILawEnf = 26;
        //pir 20232
        public const int PlanIdMain2020 = 27;
        public const int PlanIdDC2020 = 28;
        public const int PlanIdDC2025 = 30;				//PIR 25920 New Plan DC 2025
        public const string Plan_Code_DC_2025 = "DC25";
        public const int BenProvisionIdDC20 = 12;
        public const int BenProvisionIdMN20 = 11;
        public const int RefundFedPercent = 20;
        public const string WSSMessagePriorityUrgent = "URGE";
        public const string EmployerMergeStatusQueued = "QUEU";
        public const string EmployerMergeStatusCompleted = "CMPL";
        public const string PeopleSoftOrgGroupValueState = "STAT";
        public const string PeopleSoftOrgGroupValueBND = "BND";
        public const string PeopleSoftOrgGroupValueHigherEd = "HIED";
        public const string PeopleSoftOrgGroupValueNonPSParoll = "NPSP";
        public const string OrgTypeVendor = "VEND";
        public const string PeopleSoftCoverageBeginDate = "COVB";
        public const string PeopleSoftDeductionBeginDate = "DEDB";
        public const string PeopleSoftElectionDate = "ELED";

        public const string PeopleSoftDateBlank = "BLNK";
        public const string PeopleSoftDatePlanParticipationStartDate = "PPST";
        public const string PeopleSoftDateDateFileGenerated = "DFIG";
        public const string PeopleSoftDatePlanOptionStartDate = "POST";
        public const string PeopleSoftDateFirstDayPriorToPlanOptionStartDate = "FDPO";
        public const string PeopleSoftDateFirstDayPriorToPlanParticipationStartDate = "FDPP";
        public const string PeopleSoftDateDeductionBeginDate = "DSDP";
        public const string AttorneyGeneralOfficeOrgCode = "012500";
        public const string PrudentialLifeInsuranceOrgCode = "700020";
        public const string NDPERSCompanionPlanOrgCode = "700008";
        
        public const string DeductionEndDateDefCompProvider = "DEDP";//PIR 17081 - Deferred comp dates
        public const string DeductionStartDateMonthPriorDefCompProvider = "DSMP";
        public const string DeductionEndDateMonthpriorDefCompProvider = "DEMP";
	
		//prod pir 20481 - start
        public const string PeopleSoftDateFirstDayMonthPriorToHSAStartDate = "DBHS";
        public const string PeopleSoftDateHSARecordStartDate = "CBHS";
        public const string PeopleSoftDateFirstDayMonthPriorToHSAEndDate = "DEHS";
        public const string PeopleSoftDateHSARecordEndDate = "CEHS";

		public const string HSAStart = "HSAS";
        public const string HSAEnd = "HSAE";
		//prod pir 20481 - end
        //prod pir 5574
        public const string OrgContactStatusActive = "ACTV";
        public const string OrgContactStatusInActive = "INCV";

        //PIR-9649 MSS Access values
        public const string OrganizationLimitedAccess = "LMAC";
        public const string OrganizationFullAccess = "FLAC";

        #endregion

        #region ContactMgmt

        public const string ContactTicketStatusOpen = "OPEN";
        public const string ContactTicketStatusClosed = "CLOS";
        public const string ContactTicketStatusCancelled = "CANC";
        public const string ContactTicketStatusReassigned = "RASS";
        public const string ContactTicketTypeAppointment = "APP";
        public const string ContactTicketTypeRetBenefitEstimate = "RETB";
        public const string ContactTicketTypeDeath = "DETH";
        public const string ContactTicketTypeSeminarAndCounselingOutReach = "SEM";
        public const string ContactTicketTypeNewGroup = "NEWG";
        public const string ContactTicketTypeInsuranceRetiree = "INSR";
        public const string ContactTicketTypeRetPurchases = "RETC";
        public const string ContactTicketTypeWorkflow = "WKRF";
        public const string ContactTicketTypeRetAccount = "RETA";

        public const string ContactTicketTypeOtherProblem = "OTPR";
        public const string ContactTicketTypeInsuranceProblem = "INPR";
        public const string ContactTicketTypeDefCompProblem = "DFPR";
        public const string ContactTicketTypeRetProblem = "RTPR";
        //PIR-9849
        public const string ContactTicketTypePerslinkPortal = "PERS";

        public const string ResponseMethodCorrespondence = "CORR";
        public const string ResponseMethodWeb = "WEB";
        public const string ContactTicket_BenEstimate_RetirementTypeOther = "OTHR";

        public const string OnButtonValidateAddressClicked = "btnValidateAddress";

        public const string SeminarTypePayrollConference = "PAYC";
        public const string SeminarTypeWellnessForum = "WELL";
        public const string SeminarTypePrepEmployer = "PREE";
        public const string SeminarTypeOnSiteCounseling = "ONSC";
        public const string SeminarTypePrepNDPERS = "PREN";

        public const string PaymentMethodIDB = "IDBL";


        #endregion

        #region Employer Report

        public const string PayrollHeaderReportTypeRegular = "REG";
        public const string PayrollHeaderReportTypeAdjustment = "ADJS";
        public const string PayrollHeaderReportTypePenalty = "PEN";

        public const string PayrollDetailRecordTypeRegular = "REG";
        public const string PayrollDetailRecordTypePositiveAdjustment = "+ADJ";
        public const string PayrollDetailRecordTypeBonus = "BONS";
        public const string PayrollDetailRecordTypeNegativeBonus = "NBON"; // PIR 17777
        public const string PayrollDetailRecordTypeNegativeAdjustment = "-ADJ";
        public const string PayrollDetailRecordTypePurchase = "PURC";

        public const string PayrollHeaderBenefitTypeDefComp = "DEFF";
        public const string PayrollHeaderBenefitTypeRtmt = "RETR";
        public const string PayrollHeaderBenefitTypeInsr = "INSR";
        public const string PayrollHeaderBenefitTypePurchases = "PRCH";

        public const string PlanBenefitTypeRetirement = "RETR";
        public const string PlanBenefitTypeInsurance = "INSR";
        public const string PlanBenefitTypeDeferredComp = "DEFF";
        public const string PlanBenefitTypeFlex = "FLEX";

        public const string PayrollHeaderReportingSourcePaperRpt = "PR";
        public const string PayrollHeaderReportingSourceWebRpt = "WR";
        public const string PayrollHeaderReportingSourceInsuranceBatch = "INBH";

        public const string PayrollHeaderBalancingStatusBalanced = "BALN";
        public const string PayrollHeaderBalancingStatusNoRemittance = "NOR";
        public const string PayrollHeaderBalancingStatusUnbalanced = "UNBL";

        public const string PayrollHeaderStatusProcessedWithWarnings = "PRCW";
        public const string PayrollHeaderStatusReview = "REVW";
        public const string PayrollHeaderStatusValid = "VALD";
        public const string PayrollHeaderStatusIgnored = "IGNR";
        public const string PayrollHeaderStatusReadyToPost = "RPST";
        public const string PayrollHeaderStatusPosted = "PSTD";
        public const string PayrollHeaderStatusPending = "PEND";

        public const string PayrollDetailStatusReview = "REVW";
        public const string PayrollDetailStatusValid = "VALD";
        public const string PayrollDetailStatusIgnored = "IGNR";
        public const string PayrollDetailStatusPosted = "PSTD";
        public const string PayrollDetailStatusPending = "PEND";

        public const string PayrollDetailMemberStatusActive = "ACTV";
        public const string PayrollDetailMemberStatusDeath = "DETH";
        public const string PayrollDetailMemberStatusLOA = "LOA";
        public const string PayrollDetailMemberStatusLOAM = "LOAM";
        public const string PayrollDetailMemberStatusTerminated = "TERM";

        public const string PayrollDetailPaymentClassInstallmentPreTax = "INPR";
        public const string PayrollDetailPaymentClassInstallmentPostTax = "INPO";
        public const string PayrollDetailPaymentClassInstallmentDownPayment = "DNPM";

        public const string DepositTapeStatusReview = "REVW";
        public const string DepositTapeStatusValid = "VALD";
        public const string DepositTapeMethodCheck = "CHEK";
        public const string DepositTapeMethodWireTransfer = "WITR";
        public const string DepositTapeMethodACHPull = "ACHP";
        public const string DepositTapeAccountRetirement = "RETR";
        public const string DepositTapeAccountInsurance = "INSR";
        public const string EmployerPayrollDetailMemberStatusLOA = "LOA";
        public const string EmployerPayrollDetailMemberStatusLOAM = "LOAM";
        public const string EmployerPayrollDetailMemberStatusACTIVE = "ACTV";
        public const string EmployerPayrollDetailMemberStatusTerminated = "TERM";
        public const string EmployerPayrollDetailMemberStatusDeath = "DETH";

        public const string IBSHeaderStatusReview = "REVW";
        public const string IBSHeaderStatusPosted = "PSTD";
        public const string IBSHeaderStatusReadyPost = "RDPT";

        public const string DepositSourceNegativeAdjustment = "NA";
        public const string DepositSourceRegularDeposits = "RD";
        public const string DepositSourceRetireePaymentIBS = "RP";
        public const string DepositSourceIDBPayment = "IDBP";

        public const string IBSHeaderReportTypeRegular = "REG";
        public const string IBSHeaderReportTypeAdjustment = "ADJ";

        public const string EmployerReportPayrollPaidDate = "payroll_paid_date";
        public const string EmployerReportPayPeriod = "pay_period";
        public const string EmployerReportPayPeriodDate = "pay_period_date";
        public const string EmployerReportPayPeriodEndMonthForBonus = "pay_period_end_month_for_bonus";
        public const string EmployerReportMemberStatusEffectiveDate = "member_status_effective_date";
        public const string EmployerReportPayPeriodStartDate = "pay_period_start_date";
        public const string EmployerReportPayPeriodEndDate = "pay_period_end_date";
        public const string EmployerReportPayCheckDate = "pay_check_date";

        public const string RemittanceAllocationStatusAllocated = "ALOC";
        public const string RemittanceAllocationStatusPending = "PEND";
        public const string IBSRemittanceAllocationStatusPending = "PNDA";

        public const int DeferredCompOutboundFileID = 17;
        public const int RetirementOutboundFileID = 18;
        public const int InsuranceOutboundFileID = 19;
        public const int PurchaseOutboundFileID = 19;

        public const string ItemTypePurchase = "PURC";
        public const string ItemTypeEEContribution = "EMCO";
        public const string ItemTypeEEPreTax = "ECPT";
        public const string ItemTypeEEEmpPickup = "ECEP";
        public const string ItemTypeERContribution = "EMPC";
        public const string ItemTypeMemberInterest = "MEIN";
        public const string ItemTypeEmployerInterest = "EMIN";
        public const string ItemTypeRHICEmployerInterest = "ERRC";
        public const string ItemTypeRHICERContribution = "RERC";
        public const string ItemTypeRHICEEContribution = "REMC";
        public const string ItemTypeDefeCompAmount = "ECOA";
        public const string ItemTypeGroupHealthPremium = "GHPA";
        public const string ItemTypeHMOPremium = "HMOP";
        public const string ItemTypeGroupLifePremium = "GRLP";
        public const string ItemTypeEAPPremium = "EAPR";
        public const string ItemTypeLTCPremium = "LTCP";
        public const string ItemTypeGroupDentalPremium = "GRDP";
        public const string ItemTypeGroupVisionPremium = "GRVP";
        public const string ItemTypeMedicarePremium = "MEPA";
        public const string ItemTypeBenifitPayback = "BPBK";
        public const string ItemTypeContribution = "CNTR";
        public const string ItemTypePenalty = "PNTY"; //PIR 23999
        public const string ItemTypeRHICContribution = "RHCO";
        public const string ItemTypeIBSDeposit = "IBSD";
        public const string ItemTypeHealthAdminFee = "HLAF";
        public const string ItemTypeBuydownAmount = "P637"; // PIR 11239
        public const string ItemTypeMedicarePartDAmount = "P638"; // PIR 14529
        public const string ItemTypeMedicarePartDAmountIBS = "P608";//PIR 14848
        public const string ItemTypeJobSeriveHealthCredit = "JSRH";
        public const string ItemTypeDefeCompDeposit = "DCMD";
        public const string ItemTypeRHICAmount = "RHIC";
        public const string ItemTypeGroupHealthDeposit = "HLDP";
        public const string ItemTypeHMODeposit = "HMOD";
        public const string ItemTypeGroupLifeDeposit = "LIDP";
        public const string ItemTypeEAPDeposit = "EAPD";
        public const string ItemTypeLTCDeposit = "LTCD";
        public const string ItemTypeGroupDentalDeposit = "DNDP";
        public const string ItemTypeRHICNegative = "RHIN";
        public const string ItemTypeGroupVisionDeposit = "VISD";
        public const string ItemTypeMedicareDeposit = "MPDD";
        public const string ItemTypePurchaseRHIC = "PCEE"; //PIR 11171
        public const string ItemTypeADECContribution = "ADEC"; //PIR 25920
        public const string ItemTypeEEPostTaxAddlContribution = "AEPT"; //PIR 25920
        public const string ItemTypeEEPreTaxAddlCContribution = "AEPR"; //PIR 25920
        public const string ItemTypeERPreTaxMatchContribution = "EPTM"; //PIR 25920
        //PIR 26652
        public const string ItemTypeMemberExcessContributionTaxable = "P501";
        public const string ItemTypeMemberExcessContributionNonTaxable = "P502";
        public const string PaymentItemTypeCodeITEM501 = "ITEM501";
        public const string PaymentItemTypeCodeITEM503 = "ITEM503";
        public const string PaymentItemTypeCodeITEM504 = "ITEM504";
        public const string PaymentItemTypeCodeITEM505 = "ITEM505";
        public const int VendorOrgIDForState = 2985;
        public const int VendorOrgIDForFed = 2984;

        //pir 7705
        public const string ItemTypeAllocationGHHSA = "GSPA";
        public const string ItemTypeHSAVendorPayment = "P636";
        public const string ItemTypeHSAPremiumPayment = "P635";
        //end
        public const string FromItemTypeGroupHealthPremiumAmt = "GHPA";
        public const string FromItemTypeMedicarePremiumAmt = "MEPA";
        public const string FromItemTypeHMOPremiumAmt = "HMOP";
        public const string FromItemTypeGroupLifePremiumAmt = "GRLP";
        public const string FromItemTypeGroupVisionPremiumAmt = "GRVP";
        public const string FromItemTypeGroupDentalPremiumAmt = "GRDP";
        public const string FromItemTypeSeminar = "SMNR";
        public const string FromItemTypeSeminarSessions = "SESE";
        public const string ToItemTypeIBSDeposit = "IBSD";
        public const string ToItemTypeRHICNegative = "RHIN";
        public const string ItemTypePurchaseEE = "PURE";
        public const string ItemTypePurchaseER = "PUER";
        public const string ItemTypePurchaseRHICEE = "PREE";
        public const string ItemTypePurchaseRHICER = "PRER";
        public const string ItemTypeUSERRAEE = "USEE";
        public const string ItemTypeUSERRAER = "USER";
        public const string ItemTypeUSERRARHICEE = "UREE";
        public const string ItemTypeUSERRARHICER = "URER";
        public const string ItemTypeRHICEmpReport = "RHIC";
        public const string ItemTypeJSRHICEmpReport = "JSRH";
        public const string ItemTypeNegRHICEmpReport = "RHIN";

        public const string ToItemTypeJSRHICDeposit = "JSRD";

        public const string RemittanceTypeIBSDeposit = "IBSD";
        public const string RemittanceTypeSeminar = "SMNR";
        public const string RemittanceTypeJSRHICDeposit = "JSRD";
        public const string RemittanceTypePurchase = "PURC";
        public const string RemittanceTypeNegativeDeposit = "NEDE";

        public const string DepositRefundStatusPending = "PEND";
        public const string DepositRefundStatusApproved = "APRD";
        public const string DepositRefundStatusProcessed = "PRCD";
        public const int PostingInterestBatchStep = 20;

        #endregion

        #region GL

        public const string JournalHeaderStatusPosted = "POST";
        public const string JournalHeaderStatusEntered = "ENTR";

        public const string JournalDetailGLTypeDebit = "Debit";
        public const string JournalDetailGLTypeCredit = "Credit";

        public const string JournalEntryCutoffDay = "JECD";

        public const int GLPeopleSoftOutFileID = 21;
        public const int GLRIOOutFileID = 22;
        public const int GLReCreateFileID = 23;

        public const string GLFileOutDescription = "PERS";

        public const string GLFileOutFileName = "PERSLink_GL";

        public const string GLRIOFileName = "RIO_";
        public const string GLRIODateFormat = "MMyy";

        public const string TransactionTypeAllocation = "ALLO";
        public const string TransactionTypeItemLevel = "ITLV";
        public const string TransactionTypeStatusTransition = "STAT";
        public const string TransactionTypeTransfer = "TRAN";
        public const string TransactionTypeInterest = "INTE";
        public const string TransactionTypeEmployerInterest = "ERIN";
        public const string TransactionTypeVestedER = "VEER";
        public const string TransactionTypeCancelRetirement = "CNRT";
        public const string TransactionTypeCancelRefund = "CNRF";
        public const string TransactionTypeCancelPreRetirementDeath = "CNDT";
        //PIR 9546 - Capital gain to be shown on pension maintenance
        public const string TransactionTypeCapitalGain = "CPGN";

        public const string SourceTypeEmployerReporting = "EMPR";
        public const string SourceTypeIBS = "IBS";
        public const string SourceTypePayment = "PAYM";
        public const string SourceTypePurchase = "PURC";
        public const string SourceTypeRefunds = "REFN";
        public const string SourceTypeRemittance = "REMI";
        public const string SourceTypeSeminar = "SEMI";
        public const string SourceTypeJournalHeader = "JHDR";

        public const string StatusTransitionAppliedToInvalidated = "INVL";
        public const string StatusTransitionAppliedToNSF = "NSF";
        public const string StatusTransitionValidatedToApplied = "APPL";

        public const string FromItemTypePurchases = "PURC";

        public const string IsGLFlagTrue = "1";

        public const string DeptValue2001 = "2001";
        public const string FundValue470 = "470";

        #endregion

        #region DepositDetailStatus

        public const string DepositDetailStatusApplied = "APLD";
        public const string DepositDetailStatusCorrected = "CORD";
        public const string DepositDetailStatusInvalidated = "IVLD";
        public const string DepositDetailStatusNonSufficientFund = "NSF";
        public const string DepositDetailStatusReview = "REVW";
        public const string DepositDetailStatusValid = "VALD";

        public const int ACHPullFileID = 8;

        #endregion

        #region IBS Payment Entry Status

        public const string IBSCheckEntryHeaderStatusReview = "REVW";
        public const string IBSCheckEntryHeaderStatusProcessed = "PRCD";

        #endregion
        # region Workflow

        public const string ResumeActionAllDocuments = "ALLD";
        public const string ResumeActionAnyDocument = "ANYD";

        public const string DocumentTypeActionValueAlwaysInitiate = "ALIN";
        public const string DocumentTypeActionValueResumeOrInitiate = "REIN";
        public const string DocumentTypeActionValueResumeOrNeverInitiate = "RENE";

        public const string ActivityStatusInitiated = "UNPC";
        public const string ActivityStatusInProcess = "INPC";
        public const string ActivityStatusProcessed = "PROC";
        public const string ActivityStatusReleased = "RELE";
        public const string ActivityStatusSuspended = "SUSP";
        public const string ActivityStatusResumed = "RESU";
        public const string ActivityStatusCancelled = "CANC";
        public const string ActivityStatusReturned = "RETU";
        public const string ActivityStatusReturnedToAudit = "REAU";

        //Mail Distribution Workflow MAP Events
        public const string Event_Instance_Created = "InstanceCreated";
        public const string Event_Complete_Activity = "CompleteActivity";
        public const string Event_Cancel_Process = "CancelProcess";
        public const string Event_Return_Activity = "ReturnActivity";
        public const string Event_ReturnBack_Activity = "ReturnBackActivity";

        //List of Workflow Map
        public const int Map_Transfer_Call_And_Contact_Ticket = 211;
        public const int Map_Schedule_Appointment = 212;
        public const int Map_Schedule_Seminar = 213;
        public const int Map_Enroll_Membership = 214;
        public const int Map_Add_POA_Guardian = 215;
        public const int Map_Process_Service_Credit_Purchase_Election_And_Payment = 221;
        public const int Map_Enroll_New_Hire_in_Pension_and_Insurance_Plans = 229;
        public const int Map_Manage_Medicare_Age65_Letter = 230;
        public const int Map_Transition_Insurance_Coverages = 231;
        public const int Map_Maintain_Payee_Benefit_Plan = 232;
        public const int Map_Notice_of_Transfer_SFN_53506 = 233;
        public const int Map_Transfer_to_TFFR_or_TIAA_CREF = 234;
        public const int Map_Transfer_to_DC = 235;
        public const int Map_Process_DB_Retirement_Application = 236;
        public const int Map_Process_DB_To_DC_Transfer_Application = 259;
        public const int Map_Process_DB_To_TIAA_CREF_Transfer = 260; // Pir:27434
        public const int Map_Process_DB_To_TFFR_Transfer_Application = 261;
        public const int Map_Process_Job_Service_Application = 237;
        public const int Map_Process_DC_Retirement_Application = 238;
        public const int Map_Process_Disability_Application = 239;
        public const int Map_Process_Deferred_Retirement_Application = 240;
        public const int Map_Initialize_Retirement_Workflow = 241;
        public const int Map_Fulfill_RHIC_Estimate_Request = 246;
        public const int Map_Process_QDRO_Calculation = 265;
        public const int Map_Process_Death_Match = 266;
        public const int Map_Determine_Financial_Hardship = 269;
        public const int Map_Process_Benefit_Appeal = 270;
        public const int Map_Recertify_Disability = 271;
        public const int Map_Suspend_Disability_Benefits = 272;
        public const int Map_Verify_Disability_Income = 273;
        public const int Map_Recertify_Pre1991_Disability = 274;
        public const int Map_Process_RMD = 275;
        public const int Map_Remit_Tax_Withholding_Amounts = 276;
        public const int Map_Update_SSN_ToPerson_Record = 277;
        public const int Map_Split_Person_Record = 278;
        public const int Map_Merge_Person_Record = 279;
        public const int Map_Suspend_RTW_Payee_Account = 280;
        public const int Map_Reverse_RTW = 281;
        public const int Map_PSTD_Account_Owner_Application = 282;
        public const int Map_PSTD_Alternate_Payee_Application = 283;
        public const int Map_PSTD_First_Beneficiary_Application = 284;
        public const int Map_Process_Change_Payment_Distribution_Status = 285;
        public const int Map_Process_Remider_Refund = 257;
        public const int Map_Service_Purchase_Payment_Installments_Termination = 357; // PIR - 14214
        public const int Map_Service_Purchase_Payment_Installments_Employment_Change = 377; // PIR 25652
        public const int Map_Maintain_Rhic_Uncombined = 358; // PIR - 16731
        public const int Map_Nullify_QDRO = 264; // PIR - 22861

        //PIR 11946
        public const int Map_Process_Remainder_Transfer_Refund = 354;
        public const int Map_Process_Missing_RHIC_Record = 378; // PIR - 19158
        public const int Map_Update_Tax_Withholding = 252;
        //UCS - 079 - Workflowmap Start
        public const int Map_Recalculate_Pension_and_RHIC_Benefit = 292;
        public const int Map_Cancel_Payee_Account = 293;
        public const int Map_Popup_Payee_Account = 294;
        public const int Map_Reactivate_Payee_Account = 295;
        public const int Map_Update_Payment_Option = 296;
        public const int Map_Update_Dues_Rate_Table = 297;

        public const int Map_ACH_Pull_For_Insurance = 301;
        public const int Map_ACH_Pull_For_IBS_Insurance = 300;
        public const int Map_ACH_Pull_For_DeferredCompensation = 299;
        public const int Map_ACH_Pull_For_Retirement = 298;
        public const int Map_Resolve_Incoming_Mail = 337;
        public const int Map_Popup_RHIC_Amount = 352; //PIR 14346 - Initiating popup rhic amount workflow as per maik mail
        //UCS - 079 - Workflowmap End

        //PROD Pir - 4642
        public const int Map_Process_IBS_PaymentElection_Adjustment = 341;

        //UCS-055 Rhic Combining
        public const int Map_Maintain_Rhic = 247;

        // UCS 54 
        public const int Map_Creating_Receivable_Against_Payment_History = 286;

        //uat pir 1944
        public const int Map_Process_Notice_of_Employment_Change = 317;

        //uat pir 2118
        public const int Map_Process_Wellness_Benefit_Program = 327;
        public const int Map_Process_Verify_Pension_Income_and_Insurance_Premiums = 326;
        public const int Map_Process_Request_for_PHI = 325;
        public const int Map_Process_Dual_Membership = 324;
        public const int Map_Process_Correspondence = 323;
        public const int Map_Process_Internal_Auditor_Correspondence = 322;
        public const int Map_Process_Fulfill_Member_Request = 321;
        public const int Map_Process_Create_New_Organization = 320;
        public const int Map_Process_Counseling_Correspondence = 319;
        public const int Map_Process_Audit_Insurance_Claims = 318;
        public const int Map_Process_Update_Flex_Comp_Plan = 332;
        public const int Map_Process_Maintain_Person_Information = 331;
        public const int Map_Process_Maintain_Insurance_Plan = 330;
        public const int Map_Process_Maintain_Flex_Comp_Plan = 329;
        public const int Map_Process_Maintain_Deferred_Comp_Plan = 328;
        public const int Map_Process_Create_And_Maintain_Organization_Information = 291;//PIR 18503
        //pir 8313
        public const int MAP_Process_HDV_Annual_Enrollment = 339;
        public const int MAP_Process_DC_Enrollment = 383; //PIR 26939
        //Button Action        
        public const string WorkFlowButtonAction_Reassignment = "btnReassignment";
        public const string WorkFlowButtonAction_Suspend = "btnSuspend";

        //Workflow Instance Process Stats
        public const string WorkflowProcessStatus_UnProcessed = "PSNP";
        public const string WorkflowProcessStatus_Processed = "PSP";
        public const string WorkflowProcessStatus_Ignored = "IGNO";

        //Workflow Source
        public const string WorkflowProcessSource_Online = "ONLI";
        public const string WorkflowProcessSource_Batch = "BTCH";
        public const string WorkflowProcessSource_Indexing = "SCIN";
        public const string BPMProcessSource_Batch = "BTCH";

        //Activity Type
        public const string ActivityType_Verify = "VERF";

        public const string Return_From_Audit_Flag_Yes = "Y";
        public const string Return_From_Audit_Flag_No = "N";

        //Code IDs
        public const int ImageDoc_Category_Code_ID = 603;
        public const int FileNet_Document_Type_Code_ID = 604;

        public const string ProcessType_Person = "PERS";
        public const string ProcessType_Org = "ORGN";

        public const string MyBasketFilter_WorkPool = "WKPO";
        public const string MyBasketFilter_WorkAssigned = "ASWO";
        public const string MyBasketFilter_CompletedWork = "COWO";
        public const string MyBasketFilter_SuspendedWork = "SUWO";

        public const string ProcessInstanceStatusAborted = "ABRT";
        public const string ProcessInstanceStatusProcessed = "PROC";
        public const string ProcessInstanceStatusTerminated = "TERM";

        public const int MapWSSEnrollNewHireInPensionAndInsurancePlans = 306;
        //PIR-10697 Start 
        public const int MapMSSHDVAnnualEnrollment = 348;
        public const int MSSFlexcompAnnualEnrollment = 349;
        public const int MSSLifeInsuranceAnnualEnrollment = 350;
        //PIR-10697 End
        
        //PIR-10820 Start
        public const int WSSProcessUpdateFlexCompPlan = 351;
        //PIR-10820 End

        public const int MapESSDeferredCompProblem = 333;
        public const int MapESSInsuranceProblem = 334;
        public const int MapESSOtherProblem = 335;
        public const int MapESSRetirementProblem = 336;

        //PIR 9702 
        public const int MapProcessEvidenceofInsurability = 347;
        //PIR 9702 


        //PIR 14227
        public const String ApplyToHealthEnrollment = "HENL";

        //PIR 18974
        public const int Map_Process_MOU_Collection = 360;
        #endregion

        # region EmployerPayrollRemittanceAllocation

        public static string Allocated = "ALOC";
        public static string Pending_Allocation = "PEND";

        # endregion

        #region FileNet Constants
        public const string FileNetProperty_DocumentID = "Id";
        public const string FileNetProperty_VersionSeriesID = "VersionSeries";
        public const string FileNetProperty_DocumentTitle = "DocumentTitle";
        public const string FileNetProperty_InitiatedDate = "Date";
        public const string FileNetProperty_ShortName = "NDShortName";
        public const string FileNetProperty_SubjectTitle = "SubjectTitle";
        #endregion

        #region Correspondence

        public const string PersonTypeBeneficiary = "BENE";
        public const string PersonTypeMember = "MEMB";
        public const string PersonTypeRetiree = "RETR";
        public const string PersonTypeSpouse = "SPOU";
        public const string PersonTypePayee = "PAYE";
        public const string PersonTypeAlternatePayee = "ALTP";
        public const string PersonTypeDependent = "DPNT";

        public const string CorrespondenceStatus_Ready_For_Batch_Printing = "REBP";
        public const string CorrespondenceStatus_Ready_For_Imaging = "REIM";
        public const string CorrespondenceStatus_Imaged = "IMG";
        public const string CorrespondenceStatus_Printed = "PRNT";
        public const string CorrespondenceStatus_Generated = "GENR";
        public const string CorrespondenceStatus_Purged = "PURG";
        public const string CorrespondenceStatus_Archived = "ARCH";
        public const string CorrespondenceStatus_ImagedNOTPurged = "IMNP";

        public const string MailingLabelOutputFormatCSV = "FILE";
        public const string MailingLabelOutputFormatAvery = "AVRY";

        public const string MailingLabelCSVButton = "btnCSV";
        public const string MailingLabelAveryButton = "btnAvery";

        public const string PersonCommPrefMail = "EMIL";
        public const string PersonCommPrefRegMail = "MAIL";

        public const int MailingLabelOutFileIDPerson = 16;
        public const int MailingLabelOutFileIDOrg = 27;

        public const char ZipCodeSeparator = ',';

        public const string VSCThresholdHP = "HVSC";
        public const string VSCThresholdJudges = "JVSC";
        public const string VSCThresholdMLN = "MVSC";
        public const string VSCThresholdDC = "PSCM";

        #endregion

        #region Service Purchase

        public const string Plan_Code_Main = "MAIN";
        public const string Plan_Code_LE_With_Prior_Service = "LEOE";
        public const string Plan_Code_National_Guard = "NAGD";
        public const string Plan_Code_Highway_Patrol = "HWPL";
        public const string Plan_Code_Judges = "JDGS";
        public const string Plan_Code_Job_Service = "JBSR";
        public const string Plan_Code_Defined_Contribution = "DICM";
        public const string Plan_Code_LE_Without_Prior_Service = "LENE";
        // pir 7943
        public const string Plan_Code_BCI_Law_Enforcement = "BCLE";
        //PIR 20232
        public const string Plan_Code_Main_2020 = "MN20";

        //PIR 25729
        public const string Plan_Code_State_Public_Safety = "STLE";

        public const string Plan_Code_Defined_Contribution_2020 = "DC20";
        public const string Plan_changes_effective_date = "PCED";

        public const string Plan_ID_Main = "1";
        public const string Plan_ID_LE_With_Prior_Service = "2";
        public const string Plan_ID_National_Guard = "3";
        public const string Plan_ID_Highway_Patrol = "4";
        public const string Plan_ID_Judges = "5";
        public const string Plan_ID_Job_Service = "6";
        public const string Plan_ID_Judges_Conversion = "99";
        public const string Plan_ID_LE_Without_Prior_Service = "20";

        public const string Service_Purchase_Type_Consolidated_Purchase = "CONS";
        public const string Service_Purchase_Type_Unused_Sick_Leave = "UUSL";
        public const string Service_Purchase_Type_USERRA_Military_Service = "USRA";
        public const string Service_Purchase_Type_Previous_Pers_Employment = "PPER";
        public const string Service_Purchase_Type_Additional_Service_Credit = "ASC";
        public const string Service_Purchase_Type_Additional_Free_Service = "FS";
        public const string Service_Purchase_Type_Leave_Of_Absence = "LOA";

        public const string Service_Purchase_Type_Seasonal = "SEAS";
        public const string Service_Purchase_Type_Military_Service = "MS";
        public const string Service_Purchase_Type_Previous_Public_Employment = "PPE";

        public const string Service_Purchase_Action_Status_Pending = "PEND";
        public const string Service_Purchase_Action_Status_Approved = "APPR";
        public const string Service_Purchase_Action_Status_In_Payment = "INPY";
        public const string Service_Purchase_Action_Status_Void = "VOID";
        public const string Service_Purchase_Action_Status_Paid_In_Full = "PIF";
        public const string Service_Purchase_Action_Status_Closed = "CLOS";

        public const string Service_Purchase_Payor_Employer = "EMPR";
        public const string Service_Purchase_Payor_Employee = "EMPL";
        public const string Service_Purchase_Payor_Employer_And_Employee = "EREE";

        public const string Service_Purchase_Status_Review = "REVW";
        public const string Service_Purchase_Status_Valid = "VALD";
        public const string Service_Purchase_Status_Processed = "PROC";
        public const string Service_Purchase_Sick_Leave_Time_To_Purchase = "SLTP";
        public const string Service_Purchase_Contract_Interest = "SPCI";

        public const int Service_Purchase_Payment_Frequency_Code_Id = 330;

        public const string Service_Purchase_Class_Employer_LumpSum = "ERLS";
        public const string Service_Purchase_Class_Employer_USERRA_LumpSum = "ERUL";
        public const string Service_Purchase_Class_Employer_Rollover = "ROLL";
        public const string Service_Purchase_Class_Employer_Installment_PreTax = "INPR";
        public const string Service_Purchase_Class_Employer_Installment_PostTax = "INPO";
        public const string Service_Purchase_Class_Employer_Down_Payment = "DWPY";
        public const string Service_Purchase_Class_Employer_RHIC_Lumpsum = "RHLS";
        public const string Service_Purchase_Class_Employer_USERRA_Installment = "ERUI";
        public const string Service_Purchase_Class_Employer_USERRA_EE_Pickup = "EUEP";

        public const string Service_Purchase_Date_Of_Expiration = "SPDE";

        public const int Message_Id_Service_Purchase_Applied_Amount_Greater_Than_Remittance_Amount = 1125;
        public const int Message_Id_Service_Purchase_Applied_Amount_Entered_But_Payment_Class_Not_Selected = 1126;
        public const int Message_Id_Service_Purchase_Payment_Class_Employer_Lumpsum_But_Payor_Is_Not_Employer = 1127;
        public const int Message_Id_Service_Purchase_Payment_Class_Employer_But_Payment_Class_Not_Lumpsum = 1128;
        public const int Message_Id_Service_Purchase_Payment_Class_Not_Installment_And_PreTax_OR_PayrollDeduction_Checked = 1339;

        public const string ServicePurchasePaymentFrequencyValueMonthly = "MON";
        public const string ServicePurchasePaymentFrequencyValueAnnual = "ANN";
        public const string ServicePurchasePaymentFrequencyValueOneTimeLumpSumAmt = "OTP";
        public const string ServicePurchasePaymentFrequencyValueQuarterly = "QTR";
        public const string ServicePurchasePaymentFrequencyValueSemiAnnual = "SEMI";

        public const string ReferenceFormConstEmployerReporting = "EMPR";
        public const string ReferenceFormConstRemittance = "REMI";

        #endregion

        #region UCS-036 Disburse Funds to Provider or Vendors

        public const string Provider_Retirement = "RETIREMENT";
        public const string Provider_Vendor = "RETIREMENT";
        public const string Provider_DeffComp = "DEFCOMP";
        public const string Provider_Other457 = "O457";
        public const string Provider_Insurance = "INSURANCE";
        public const string Provider_Insurance_Medicare = "MEDICARE"; //PIR 14848

        public const string Provider_BankOfNorthDakota = "PD01";
        public const string Provider_HartFordLife = "PD02";
        public const string Provider_JacksonNationalLife = "PD03";
        public const string Provider_KEMPER = "PD04";
        public const string Provider_LincolnNational = "PD05";
        public const string Provider_NationWideLife = "PD06";
        public const string Provider_Fidelity = "PD07";
        public const string Provider_SYMETRA = "PD08";
        public const string Provider_AIGVALIC = "PD09";
        public const string Provider_WaddellAndReed = "PD10";
        public const string Provider_AXA = "PD13";
        public const string Provider_ING = "PD14";
        public const string Provider_AmericanTrustCenter = "PD15";
        public const string Provider_KANSAS = "PD36";
        public const string Provider_IRS = "PD68";

        public const string Vendor_Payment_Status_Processed = "PRSD";
        public const string Vendor_Payment_Status_NotProcessed = "NTPR";

        public const string PaymentOption_ACH = "A";
        public const string PaymentOption_WireTransfer = "C";
        public const string PaymentOption_Check = "W";

        public const string ServiceCode_CreditsOnly = "220";
        public const string ServiceCode_DebitsOnly = "225";
        public const string ServiceCode_CreditDebitMixed = "200";

        public const string RecordTypeNegativeAdjustment = "-ADJ";
        public const string RecordTypePositiveAdjustment = "+ADJ";

        public const int BNDDFINumber = 09130028;
        public const int WaddelDFINumber = 10100069;

        public const string IdentificationCodeNDPERS = "NDPERS";
        public const string IdentificationCodeDental = "239790001";

        public const string DELTAProviderCodeValue = "HIDL"; // PIR 10448
        public const string SanfordProviderCodeValue = "HISF";// PIR 14241
        public const string CIGNAProviderCodeValue = "HICG";
        public const string SuperiorVisionProviderCodeValue = "HISV";
        public const string BCBSProviderCodeValue = "HIBC"; //pir 8570
        #endregion

        #region UCS-022 Maintain Pension and Insurance Plan Account
		
		//PIR 11946
        public const string PlanParticipationStatusData2TransferredDC = "TDDC";
        public const string PlanParticipationStatusData2TransferredTIAACREF = "TTCF";
        public const string PlanParticipationStatusData2TransferredTFFR = "TFFR";
        public const string PlanParticipationStatusData2Enrolled = "ERLD";
        public const string PlanParticipationStatusData2Suspended = "SPND";
        public const string PlanParticipationStatusData2Cancelled = "CNLD";
        public const string PlanParticipationStatusData2WithDrawn = "WTDN";
        public const string PlanParticipationStatusTransferToTFFR = "TRTF";
        public const string PlanParticipationStatusRetirementTransferredTIAACREF = "TICR";
        //public const string LOCPlanOptionStatusWaived = "WAVD";
        public const string PlanParticipationStatusDefCompSuspended = "SUS1";
        public const string PlanParticipationStatusRetimentSuspended = "SUS2";
        public const string PlanParticipationStatusFlexSuspended = "SUS4";
        public const string PlanParticipationStatusFlexCancelled = "CAN4";
        public const string PlanParticipationStatusDefCompCancelled = "CAN3";
        //public const string PlanParticipationStatusRetirementWaived = "WAI1";
        public const string PersonAccountElectionValueWaived = "WAVD";
        public const string PlanParticipationStatusRetirementRetired = "RTRD";
        public const string PlanParticipationStatusTransferDC = "TRDC";
        public const string PlanParticipationStatusRetirementWithDrawn = "WTN1";
        public const string PlanParticipationStatusPostRetirementDeath = "DSD1";
        public const string PlanParticipationStatusPreRetirementDeath = "DSW1";
        public const string PlanParticipationStatusDeffCompWithDrawn = "WTN2";
        public const string PlanParticipationStatusRetirementEnrolled = "ENL1";
        public const string PlanParticipationStatusInsuranceEnrolled = "ENL2";
        public const string PlanParticipationStatusDefCompEnrolled = "ENL3";
        public const string PlanParticipationStatusFlexCompEnrolled = "ENL4";
        public const string PlanParticipationStatusRetirmentCancelled = "CAN1";
        public const string PersonAccountStatusValid = "VALD";
        public const string PersonAccount457LimitRegular = "REGU";
        public const string PersonAccount457Limit50 = "50+";
        public const string PersonAccount457LimitCatchUp = "CTCH";
        public const string FlexLevelOfCoverageMedicareSpending = "MSRA";
        public const string FlexLevelOfCoverageDependentSpending = "DCRA";
        public const string FlexLevelOfCoveragePremiumConversion = "PRCV";
        public const string FlexCompTypeValueCOBRA = "CBRA";
        public const string FlexCompTypeValueActive = "ACTV";
        public const string PersonAccountLtcRelationShipMember = "MEBR";
        public const string PersonAccountLtcRelationShipSpouse = "SPSE";
        public const string RequiredFlagOptionValueMandatory = "MAND";
        public const string EAPInsuranceTypeValueRegular = "RE14";
        public const int LevelofCoverage_CodeID = 408;

        public const string TransactionTypeRegularPayroll = "RGPR";
        public const string TransactionTypePayrollAdjustment = "PRAD";
        public const string TransactionTypeServicePurchase = "PURC";
        public const string TransactionTypePurchaseAdjustment = "PADJ";
        public const string TransactionTypeInternalAdjustment = "IADJ";
        public const string TransactionTypeAlternatePayeePayment = "APPY";

        public const string PersonAccountSubSystemTypePayrollReport = "PAYR";
        public const string LevelofCoverage_Basic = "BASC";
        public const string LevelofCoverage_Supplemental = "SPML";
        public const string LevelofCoverage_DependentSupplemental = "DSPL";
        public const string LevelofCoverage_SpouseSupplemental = "SPSL";
        public const string LifeInsuranceFlagValue = "O";

        public const string PlanRetirementTypeValueDB = "DB";
        public const string PlanRetirementTypeValueDC = "DC";
        public const string PlanRetirementTypeValueHB = "HB";
        public const string PlanOptionEPBA = "EPBA";
        public const string PlanOptionEPPB = "EPPB";
        public const string SubSystemValueIBSDeposit = "IBSD";
        public const string SubSystemValueIBSPayment = "IBSP";
        public const string TransactionTypeRegularIBS = "RIBS";
        public const string TransactionTypeIBSPayment = "IBSP";
        public const string TransactionTypeAdjustmentIBS = "AIBS";
        public const string PlanParticipationStatusInsuranceCancelled = "CAN2";
        public const string PlanParticipationStatusInsuranceSuspended = "SUS3";
        //public const string PlanParticipationStatusInsuranceWaived = "WAI2";

        public const string PlanOptionStatusValueEnrolled = "ENLD";
        public const string PlanOptionStatusValueWaived = "WAVD";
        public const int PlanOptionStatusID = 340;

        public const string BeneficiaryMemberTypePrimary = "PRIM";
        public const string BeneficiaryMemberTypeContingent = "CONT";

        public const string PersonTFFRTIAAServiceStatusApproved = "APRV";
        public const string PersonTFFRTIAAServiceStatusTentative = "TNTV";

        public const string DependentRelationshipChild = "CHLD";
        public const string DependentRelationshipStepChild = "SCLD";
        public const string DependentRelationshipAdoptiveChild = "ADCH";
        public const string DependentRelationshipGrandChild = "GRCH";
        public const string DependentRelationshipDisabledChild = "DSCH";
        public const string DependentRelationshipSpouse = "SPOU";
        public const string DependentRelationshipOthers = "OTHR";
        public const string DependentRelationshipExSpouse = "EXPS";
        public const string DependentRelationshipLegalGuardian = "LGLG"; //pir 7269

        public const decimal CoverageAmountBasic = 3500;

        public const string DentalLevelofCoverageFamily = "FM16";
        public const string DentalLevelofCoverageIndividual = "IN16";
        public const string DentalLevelofCoverageIndividualSpouse = "IS16";
        public const string DentalLevelofCoverageIndividualChild = "IC16";

        public const string DentalInsuranceTypeCOBRA = "CB16";
        public const string DentalInsuranceTypeActive = "AC16";
        public const string DentalInsuranceTypeRetiree = "RT16";

        public const string VisionInsuranceTypeCOBRA = "COBR";
        public const string VisionInsuranceTypeActive = "ACTV";
        public const string VisionInsuranceTypeRetiree = "RETR";

        public const string VisionLevelofCoverageFamily = "FMLY";
        public const string VisionLevelofCoverageIndividual = "INDV";
        public const string VisionLevelofCoverageIndividualSpouse = "INSP";
        public const string VisionLevelofCoverageIndividualChild = "INCH";

        public const string CoverageIndividual = "Individual Only";
        public const string CoverageIndividualSpouse = "Individual and Spouse";
        public const string CoverageIndividualChild = "Individual and Child(ren)";

        public const string COBRAType18Month = "18MC";
        public const string COBRAType36Month = "36MC";
        public const string COBRATypeDisability = "DISC";
        public const string COBRATypeRetiree18Month = "R18C";
        public const string COBRATypeRetiree36Month = "R36C";
        public const string COBRATypeRetireeDisability = "RDSC";

        public const string EnrollmentReasonEndofCOBRA = "ECOP";

        public const string HealthInsuranceTypeState = "ST12";
        public const string HealthInsuranceTypeNonState = "NS12";
        public const string HealthInsuranceTypeRetiree = "RT12";
        public const string MedicarePartDInsuranceTypeRetiree = "RT09";

        public const string LifeInsuranceTypeRetireeMember = "REME";
        public const string LifeInsuranceTypeActiveMember = "NREM";

        public const string TerminationLetterStatusNotRequired = "LNR";
        public const string TerminationLetterStatusNotSent = "LNS";
        public const string TerminationLetterStatusSent = "LSN";
        public const string PersonAccountElectionValueEnrolled = "ENLD";
        public const string LTCLevelOfCoverage3YRS = "3YRS";
        public const string LTCLevelOfCoverage5YRS = "5YRS";

        public const string COBRALetterStatusNotSent = "LNS";
        public const string COBRALetterStatusSent = "LSN";


        public const string DCEnrollmentOutBoundFileName = "Fidelity_DC_benefits";

        public const string SystemConstantISADentalCIGNA = "DCIG";
        public const string SystemConstantISADentalDelta = "DDEL"; // PIR 10025
        public const string SystemConstantISAHealthBCBS = "BCBS";
        public const string SystemConstantISAVisionAmeritas = "AMTS";
        public const string SystemConstantISASuperiorVision = "SUPU";
        public const string SuperiorVisionOrgCode = "700073";

        public const string AssetsWithProviderMember = "MEMB";
        public const string AssetsWithProviderBeneficiary = "BENE";

        //UCS-041 
        public const string DBDBTransferStatusPostedValue = "POST";

        public const string GroupHealthLevelofCoverageSingle = "Single";
        public const string GroupHealthLevelofCoverage1Medicare = "1 Medicare";

        public const string MemberTypeMainTemp = "MTP1";
        public const string MemberTypeDCTemp = "DTP1";

        public const string HMOLevelOfCoverageSingle = "SI22";

        #endregion

        #region IBS Constants

        public const string IBSModeOfPaymentACH = "ACH";
        public const string IBSModeOfPaymentPensionCheck = "PNCH";
        public const string IBSModeOfPaymentPersonalCheck = "PRCH";
        public const string IBSModeOfPaymentTFFRCheck = "TFPC";
        public const string IBSAllocationStatusAllocated = "ALTD";

        #endregion

        #region Benefit Application

        public const string ApplicationActionStatusPending = "PEND";
        public const string ApplicationActionStatusVerified = "VERF";
        public const string ApplicationActionStatusDeferred = "DEFE";
        public const string ApplicationActionStatusCancelled = "CANC";
        public const string ApplicationActionStatusDenied = "DENI";

        public const string ApplicationBenefitTypeRetirement = "RETR";
        public const string ApplicationBenefitTypeDisability = "DISA";
        public const string ApplicationBenefitTypePreRetirementDeath = "DETH";
        public const string ApplicationBenefitTypePostRetirementDeath = "PSTD";
        public const string ApplicationBenefitTypeRefund = "RFND";

        public const string ApplicationRHICReduced50 = "RD50";
        public const string ApplicationRHICReduced100 = "R100";
        public const string ApplicationStatusReview = "REVW";
        public const string ApplicationStatusValid = "VALD";
        public const string ApplicationStatusProcessed = "PROC";
        public const string CalculationStatusProcessed = "PROC";
        public const string CalculationStatusReview = "REVW";
        public const string ApplicationBenefitOptionValueSocialSecurityLevelIncome = "SSLI";

        //this const is used for checking the benefit option is Joint and Survivor
        public const string ApplicationBenefitOptionValueData3Value50JointSurvivor = "J50S";
        public const string ApplicationBenefitOptionValueData3Value55JointSurvivor = "J55S";
        public const string ApplicationBenefitOptionValueData3Value75JointSurvivor = "J75S";
        public const string ApplicationBenefitOptionValueData3Value100JointSurvivor = "J100";

        public const string ApplicationBenefitSubTypeNormal = "NRTR";
        public const string ApplicationBenefitSubTypeEarly = "ERTR";
        public const string ApplicationBenefitSubTypeDNRO = "DRTR";
        public const string ApplicationBenefitSubTypeDisability = "DISR";
        public const string ApplicationBenefitSubTypeDisabilitytoNormal = "DISN";
        public const int ApplicationLetterSent1 = 1;
        public const int ApplicationLetterSent2 = 2;
        public const string OtherDisBenWSI = "WSIB";
        public const string OtherDisBenSocialSecurityDisBen = "SSDB";
        public const decimal SSLIAgeForJobService = 62.0000M;
        public const decimal SSLIAgeLowerLimit = 62.0000M;
        public const int CodeValueForSSLILimit = 1912;
        public const int MinAgeForEarlyReductionSpecialCase = 55;
        public const string BenAppsValue = "SARC";
        public const string BenefitOptionRefund = "REFD";
        public const string BenefitOption50Percent = "L050";
        public const string BenefitOption100Percent = "J100";
        public const string BenefitOptionMonthlyLifeTimeBenefit = "MLBE";
        public const string BenefitOptionSpouseBenefit = "SPBE";
        public const string BenefitOptionChildTimeBenefit = "CDBE";
        public const string BenefitOptionBeneficiaryBenefit = "BEBE";
        public const string BenefitOptionPeriodicPayment = "PRPM";
        public const string BenefitOptionAutoRefundData3 = "AURD";
        public const string BenefitOptionSingleLife = "SNGL";
        public const string BenefitOptionStraightLife = "STLF";
        public const string BenefitOption55Percent = "J55S";
        public const string BenefitOption75Percent = "J75S";
        public const string BenefitOptionNormalRetBenefit = "NRTR";
        public const string BenefitOptionDisability = "DISA";
        public const string BenefitOptionDBToDCTransferSpecialElection = "SPEL";

        // Benefit Provision Eligibility 
        public const string BenefitProvisionEligibilityVested = "VEST";
        public const string BenefitProvisionEligibilityNormal = "NRML";
        public const string BenefitProvisionEligibilityEarly = "ERLY";

        public const string RelationshipTypeAlternatePayeeDescription = "Alternate Payee";

        public static string GraduatedBenefitOptionDateCodeValue = "GRAD";
        public static DateTime GraduatedBenefitOptionDate = new DateTime(2011, 03, 01);

        public static DateTime SSLIUniformIncomeEffectiveRetirementDate = new DateTime(2013, 07, 01); // PIR 11236

        #endregion

        #region UCS-071 Payee Account

        public const int PaymentBatchID = 50;
        public const int IBSBillingBatchID = 10;

        public const string PayeeAccountExclusionMethodSafeHarbor = "SHMT";
        public const string PayeeAccountExclusionMethodSimplified = "SIMT";

        public const string PayeeAccountRetroPaymentOptionRegular = "REGL";
        public const string PayeeAccountRetroPaymentOptionSpecial = "SPCK";

        public const string PayeeAccountDeductionPaymentOptionSpecial = "SPCL";

        public const string PayeeAccountStatusApproved = "APRD";
        public const string PayeeAccountStatusReceiving = "RECV";
        public const string PayeeAccountStatusDCReceiving = "DCRC";
        public const string PayeeAccountStatusReview = "REVW";
        public const string PayeeAccountStatusPreDeathReview = "RDRW";
        public const string PayeeAccountStatusPostDeathReview = "RPRW";
        public const string PayeeAccountStatusCancelled = "CNLD";
        public const string PayeeAccountStatusPaymentComplete = "TRMD";
        public const string PayeeAccountStatusTerminated = "TRMD";
        public const string PayeeAccountStatusSuspended = "SPND";
        public const string PayeeAccountStatusCancelPending = "CNLP";
        public const string PayeeAccountStatusJS3RDPartyReceiving = "3RDP"; //pir 8536
        public const string PayeeAccountStatusApprovedPSTD = "PAPD"; //PIR 13854
        public const string PayeeAccountStatusApprovedDETH = "DAPD"; //PIR 13854
        public const string PayeeAccountStatusSuspendedPSTD = "SPPS"; //PIR 19158
        public const string PayeeAccountStatusSuspendedPERDT = "SPDT"; //PIR 19158

        public const string PayeeAccountStatusRefundReview = "RRVW";
        public const string PayeeAccountStatusRetirementReview = "REVW";
        public const string PayeeAccountStatusDisabilityReview = "RVDS";
        public const string PayeeAccountStatusDisabilityApproved = "APDS";
        public const string PayeeAccountStatusRefundApproved = "RAPD";
        public const string PayeeAccountStatusRefundProcessed = "RPCD";
        public const string PayeeAccountStatusRetirmentApproved = "APRD";
        public const string PayeeAccountStatusRetirmentRecieving = "RECV";
        public const string PayeeAccountStatusRetirmentDCRecieving = "DCRC";
        public const string PayeeAccountStatusRetirmentCancelled = "CNLD";
        public const string PayeeAccountStatusRetirementPaymentCompleted = "TRMD";
        public const string PayeeAccountStatusDisabilityPaymentCompleted = "TRDS";
        public const string PayeeAccountStatusDisabilityCancelled = "CLDS";
        public const string PayeeAccountStatusDisabilityPendingCancelled = "CNDS";
        public const string PayeeAccountStatusDisabilitySuspended = "SPDS";
        public const string PayeeAccountStatusRefundCancelled = "RCLD";
        public const string PayeeAccountStatusPreRetirementDeathPaymentCompleted = "TRDT";
        public const string PayeeAccountStatusPostRetirementDeathPaymentCompleted = "TRPS";
        public const string PayeeAccountStatusPreRetirementDeathCancelled = "PCLD";
        public const string PayeeAccountStatusPostRetirementDeathCancelled = "PSCD";
        
        public const string PayeeAccountRolloverDetailStatusActive = "ACTV";
        public const string PayeeAccountRolloverDetailStatusProcessed = "PRCD";
        public const string PayeeAccountRolloverDetailStatusCancelled = "CANC";

        public const string PayeeAccountRolloverDetailStatusTerminated = "TRMD";
        public const string PayeeAccountRolloverDetailStatusSuspended = "SPND";
        public const string PayeeAccountAccountRelationshipOwner = "MEMB";
        public const string PayeeAccountAccountRelationshipAlternatePayee = "ALTP";
        public const string PayeeAccountFamilyRelationshipExSpouse = "EXPS";
        public const string PaymentItemTypeID3rdPartyRefund = "ITEM95";
        public const string PaymentItemTypeID3rdPartyHealthInsurance = "ITEM71";
        public const string PaymentItemTypeIDRHICBenefitReimbursement = "ITEM62";
        public const string PayeeAccountMaritalStatusMarried = "MRID";
        public const string FedTaxOptionFedTaxBasedOnIRS = "FTIR";
        public const string FedTaxOptionFederalTaxwithheld = "FTWH";
        public const string FedTaxOptionFedTaxBasedOnIRSAndAdditional = "FTIA";
        public const string StateTaxOptionFedTaxBasedOnIRS = "STST";

        public const string StateTaxOptionNoMonthlyNDTax = "NSTX";
        public const string StateTaxOptionNoOnetimeNDTax = "NSTW";

        public const string StateTaxOptionFedTaxBasedOnIRSAndAdditional = "RSTS";
        public const string PayeeAccountTaxIdentifierFedTax = "FDRL";
        public const string PayeeAccountTaxIdentifierStateTax = "STAT";
        public const string PayeeAccountTaxRefFed22Tax = "FD22";
        public const string PayeeAccountTaxRefState22Tax = "ST22";
        public const string RetroPaymentTypeInitial = "IRPM";
        public const string RetroPaymentTypeReactivation = "REAC";
        public const string RetroPaymentTypeBenefitUnderpayment = "RADJ";
        public const string RetroPaymentTypeRHICBenefitReimbursement = "RBRB";
        public const string RetroPaymentTypePopupBenefits = "POPB";
        public const string DeductionIndicatorHidden = "HDDN";
        public const string RolleverCodeValueCanBeRolled = "ROLL";
        public const string PayeeAccountRolloverOptionAllOfGross = "ALOG";
        public const string PayeeAccountRolloverOptionAllOfTaxable = "ALOT";
        public const string PayeeAccountRolloverOptionAmountOfTaxable = "DLOT";
        public const string PayeeAccountRolloverOptionPercentageOfTaxable = "PROT";
        public const string PaymentItemTypeCodeITEM14 = "ITEM14";

        public const string SpecialTaxIdendtifierFedTax = "FDTX";
        public const string SpecialTaxIdendtifierFlatTax = "FTTX";

        public const string BenefitDistributionMonthlyBenefit = "MNBF";
        public const string BenefitDistributionPSLO = "PSLO";
        public const string BenefitDistributionLumpSum = "LSDB";
        public const string BenefitDistributionRMD = "RMD";
        public const int WithholdingTaxAllowance = 3;

        public const string RetroPaymentInterestPercentage = "INTP";
        public const string PaymentItemTypeCodeITEM45 = "ITEM45";
        public const string PaymentItemTypeCodeITEM51 = "ITEM51";
        public const string PaymentItemTypeCodeITEM57 = "ITEM57";
        public const string PaymentItemTypeUsageMonthly = "MNTP";

        public const string ItemTypeFedTaxOnInterestForInitial = "ITEM46";
        public const string ItemTypeStateTaxOnInterestForInitial = "ITEM47";
        public const string ItemTypeFedTaxOnInterestForSusPension = "ITEM58";
        public const string ItemTypeStateTaxOnInterestForSuspension = "ITEM59";
        public const string ItemTypeFedTaxOnInterestForAdjustments = "ITEM52";
        public const string ItemTypeStateTaxOnInterestForAdjustments = "ITEM53";

        public const string PAPITPLSOTaxableRollover = "ITEM31";
        public const string PAPITPLSONonTaxableRollover = "ITEM32";
        public const string PAPITPLSOTaxableAmount = "ITEM33";
        public const string PAPITPLSONonTaxableAmount = "ITEM34";
        public const string PAPITPLSOFederalTaxAmount = "ITEM35";
        public const string PAPITPLSOStateTaxAmount = "ITEM36";
        public const string PAPITTaxableAmount = "ITEM37";
        public const string PAPITTaxalbeAmount = "ITEM38";
        public const string PAPITDues = "ITEM74";
        public const string PAPITFederalTaxAmount = "ITEM67";
        public const string PAPITFederalTaxRefund = "ITEM107";
        public const string PAPITNDStateTaxAmount = "ITEM68";
        public const string PAPITStateTaxRefund = "ITEM108";
        public const string PaymentDeductionOptionRegular = "RGLR";
        public const string PaymentDeductionOptionSpecial = "SPCL";
        public const string PAPITRefundFederalTaxAmount = "ITEM12";
        public const string PAPITRefundNDStateTaxAmount = "ITEM13";

        public const string PAPITRMDFederalTaxAmount = "ITEM81";
        public const string PAPITRMDStateTaxAmount = "ITEM82";


        public const string PAPITRolloverTaxableAmountForRothIRA = "ITEM217";
        public const string PAPITRothRolloverStateTaxAmount = "ITEM216";
        public const string PAPITRothRolloverFederalTaxAmount = "ITEM215";
        public const string PAPITPLSORothRolloverFederalTaxAmount = "ITEM218";
        public const string PAPITPLSORothRolloverStateTaxAmount = "ITEM219";
        public const string PAPITPLSORothRolloverTaxableAmount = "ITEM220";
        public const string PAPITRolloverNonTaxableAmountForRothIRA = "ITEM223";
        public const string PAPITPLSORothRolloverNonTaxableAmount = "ITEM225";


        public const string PAPITRecalcRetroStateTaxAmount = "ITEM117";
        public const string PAPITRecalcRetroFedTaxAmount = "ITEM116";

		//Backlog PIR 10966
        public const string PAPITRetroFedTaxAmount = "ITEM114";
        public const string PAPITRetroStateTaxAmount = "ITEM115";

        public const string PAPITRetroFedTaxAmountSpecial = "ITEM408";
        public const string PAPITRetroStateTaxAmountSpecial = "ITEM409";

        public const string PAPITNetAmountReissue = "ITEM448";
        public const string PAPITReissueRolloverTaxableAmount = "ITEM453";
        public const string PAPITNetAmountEscheat = "ITEM451";
        public const string PAPITHealthInsurance = "ITEM76";
        public const string PAPITLifeInsurance = "ITEM77";
        public const string PAPITDentalInsurance = "ITEM78";
        public const string PAPITVisionInsurance = "ITEM79";
        public const string PAPITLTCInsurance = "ITEM80";
        public const string PAPITMedicarePartD = "ITEM120";

        public const string PAPITRMDAmount = "ITEM449";

        public const string PAPITPSTDTaxableAmount = "ITEM430";
        public const string PAPITPSTDNonTaxableAmount = "ITEM431";

        public const string PAPITOverpaymentInterest = "ITEM413";

        public const string PAPITPensionReceivable = "ITEM65";
        public const string PAPITPensionReceivableRefund = "ITEM25";
        public const string PAPITRHICBenefitReimbursement = "ITEM62";
        public const string PAPITHSAAmount = "ITEM635";
        public const string PaymentItemTypeCheckRegular = "REGL";

        public const string CheckGroupCodeDeduction = "DEDT";

        public const string DeductionIndicatorReadOnly = "RONL";
        public const string DeductionIndicatorUpdatable = "UPDT";
        public const string TaxOptionFlatStateTaxOnly = "FSTO";
        public const string TaxOptionFlatFedTaxOnly = "FFTO";
        public const string TaxOptionStateTaxwithheld = "STWH";
        public const string TaxOptionFedTaxwithheld = "FTWH";
        public const string RollItemForCheck = "CRET";
        public const string RolloverItemReductionCheck = "RRED";
        public const string AllowRolloverItemDONT = "DONT";
        public const string StateNorthDakota = "ND";
        //UCS - 55 
        public const string NoFedTax = "NFTX";
        public const string NoStateTax = "NSTX";
        public const string NoFederalTaxWithheld = "NFTW";
        public const string NoStateTaxWithheld = "NSTW";

        public const string BenefitAccountSubTypePreRetirementDeathRefund = "LSDF";
        public const string BenefitAccountSubTypePreRetirementDeathBenefit = "DEBN";

        public const string MaritalStatusMarriedWithholdAtSingleRate = "MRDW";

        //RHIC Combine Detail
        public const string RHICCombineApplyToHealth = "NPHI";

        #endregion

        #region Benefit Calculation

        public const string CalculationTypeEstimate = "ESTM";
        public const string CalculationTypeFinal = "FNAL";
        //PIR 18053 - New Calculation type for RTW.
        public const string CalculationTypeSubsequent = "SBST";
        public const string CalculationTypeSubsequentAdjustment = "STAD";

        public static DateTime RTWEffectiveDate = new DateTime(2018, 07, 01);

        //PIR 19594 - New Calculation type for RTW estimates.
        public const string CalculationTypeEstimateSubsequent = "ESTS";

        public const string BenefitActionStatusPending = "PAPR";
        public const string BenefitActionStatusApproved = "APPR";
        public const string BenefitActionStatusCancelled = "CANC";

        public const string FASHighestAverage = "HIAV";
        public const string FASHighestConsecutive = "HICA";

        public const string ExclusionCalcPaymentTypeJointLife = "JOIL";
        public const string ExclusionCalcPaymentTypeSingleLife = "SINL";

        public const string RetirementFinal = "RTRF";
        public const string RetirementEstimate = "RTRE";
        public const string RefundFinal = "RFDF";
        public const string RefundAdjustment = "RFDA";
        public const string RetirementDisability = "RTDS";
        public const string RetirementDeath = "RTDT";
        public const string Refund = "RFND";
        public const string PreRetirementDeathEstimate = "DEES";
        public const string PreRetirementDeathFinal = "DEFN";
        public const string PostRetirementDeathFinal = "PSTD";

        public const string AccountRelationshipMember = "MEMB";
        public const string AccountRelationshipJointAnnuitant = "JANT";
        public const string AccountRelationshipSpouse = "SPJA";
        public const string AccountRelationshipChild = "SPOB";
        public const string AccountRelationshipBeneficiary = "BENE";
        public const string AccountRelationshipAlternatePayee = "ALTP";
        public const string FamilyRelationshipUnknown = "UNKN";

        public const string FamilyRelationshipSpouse = "SPOU";
        public const string FamilyRelationshipOthers = "OTHR";
        public const string FamilyRelationshipChild = "CHLD";
        public const string FamilyRelationshipMember = "MEMB";

        public const string RHICActionStatusPendingApproval = "PAPR";
        public const string RHICActionStatusApproved = "APPR";
        public const string RHICActionStatusDeny = "DEND";
        public const string RHICActionStatusCancelled = "CANC";
        public const string RHICActionStatusEnded = "ENDD";
        public const string RHICStatusValid = "VALD";
        public const string RHICStatusReview = "REVW";
        public const string BenefitHealthDeduction = "HLTH";
        public const string BenefitDentalDeduction = "DNTL";
        public const string BenefitVisionDeduction = "VISN";

        public const string RHICOptionStandard = "STRD";
        public const string RHICOptionReduced50 = "RD50";
        public const string RHICOptionReduced100 = "R100";

        public const string BenefitOption5YearTermLife = "5YTL";
        public const string BenefitOption10YearCertain = "T10C";
        public const string BenefitOption15YearCertain = "T15C";
        public const string BenefitOption20YearCertain = "T20C";
        public const string BenefitOption100PercentJS = "J100";
        public const string BenefitOption50PercentJS = "J50S";

        public const string BenefitFormulaValueOther = "OTHR";

        public const string FactorMethodValueOther = "OTHR";
        public const string FactorMethodValueMemberAndSurvivorAge = "MASA";
        public const string FactorMethodValueFactor = "FACT";

        public const decimal MaxRetirementAgeAtNormalRetDate = 73M; //PIR 24248

        public const string CalculationStatusPendingApproval = "PAPR";
        public const string CalculationStatusApproval = "APPR";
        public const string CalculationStatusCancel = "CANC";

        public const string PayeeIdentifierSpouse = "SPOU";
        public const string PayeeIdentifierChild = "CHLD";
        public const string PayeeIdentifierOther = "OTHR";

        public const int JobServiceBenefitProvisionID = 6;

        public const string PostRetirementAccountOwnerDeath = "ACOD";
        public const string PostRetirementAlternatePayeeDeath = "ALPD";
        public const string PostRetirementFirstBeneficiaryDeath = "FBED";

        public const string BenefitAccountSubTypePostRetDeath = "PSRD";

        public const string RuleIndicatorRuleof80 = "RL80";
        public const string RuleIndicatorRuleof85 = "RL85";
        public const string RuleIndicatorRuleof90 = "RL90"; //PIR 14646

        public const int AgePlusServiceRuleof80 = 80;
        public const int AgePlusServiceRuleof85 = 85;
	    public const int AgePlusServiceRuleof90 = 90; //PIR 14646

        //UAT PIR: 925 Graduated Benefit Option Code changes.
        public const string GraduatedBenefit1PerDecrease = "1PUP";
        public const string GraduatedBenefit2PerDecrease = "2PUP";


        #endregion

        # region Death Notification

        public const string DeathNotificationActionStatusCompleted = "COMP";
        public const string DeathNotificationActionStatusCancelled = "CANC";
        public const string DeathNotificationActionStatusNonResponsive = "NNRS";
        public const string DeathNotificationActionStatusInProgress = "INPR";
        public const string DeathNotificationActionStatusErroneous = "ERRN";
        public const string RelationshipWithMemberIsSelf = "Self";
        public const string RelationshipWithMemberIsDependent = "Dependent";
        public const string RelationshipWithMemberIsBeneficiary = "Beneficiary";
        public const string RelationshipWithMemberIsContact = "Contact";

        public const int Map_Initialize_PreRetirement_Death_Workflow = 249;

        public const int Map_Initialize_Process_Beneficiary_Auto_Refund_Workflow = 250;
        public const int Map_Initialize_Process_Auto_Refund_Workflow = 258;
        public const int Map_Initialize_Process_Death_Notification_Workflow = 248;
        public const int Map_Initialize_Process_Refund_Application_And_Calculation = 256;
        public const int Map_Initialize_Process_PA_Death_Notification_Workflow = 371;
        #endregion

        #region UCS -057 Refund Benefit Calculation

        public const string BenefitOptionAutoRefund = "AURD";
        public const string BenefitOptionRegularRefund = "RGRD";
        public const string BenefitOptionDBToTFFRTransferForDPICTE = "DBDC";
        public const string BenefitOptionDBToTFFRTransferForDualMembers = "DBTT";
        public const string BenefitOptionDBToDCTransfer = "DBCT";
        public const string BenefitOptionDBToTIAACREFTransfer = "DTCT";
        public const string PersonJobClassCareerAndTechEdCertifiedTeacher = "CTCT";
        public const string PersonJobClassDeptofPublicInstructionCertifiedTeacher = "DPIC";
        public const string CalculationTypeAdjustments = "ADJM";

        public const string RefundPaymentItemPreTaxEEContributionAmount = "ITEM2";
        public const string RefundPaymentItemPostTaxEEContributionAmount = "ITEM3";
        public const string RefundPaymentItemEEInterestAmount = "ITEM7";
        public const string RefundPaymentItemCapitalGain = "ITEM5";
        public const string RefundPaymentItemVestedERContributionAmount = "ITEM6";
        public const string RefundPaymentItemRHICEEAmount = "ITEM4";
        public const string RefundPaymentItemERPreTaxAmount = "ITEM19";
        public const string RefundPaymentItemERInterestAmount = "ITEM8";
        public const string RefundPaymentItemEEERPickupAmount = "ITEM452";
        public const string RefundPaymentItemAdditionalEEAmount = "ITEM20";
        public const string RefundPaymentItemAdditionalERAmount = "ITEM21";
        public const string RefundPaymentItemAdditionalEEInterestAmount = "ITEM18";

        #endregion

        #region Rate Constants
        public const string Per_Thousand = "PR1K";
        public const string Per_Coverage = "PRCV";
        #endregion

        #region UCS -058 DRO

        public const string DROApplicationStatusRecieved = "RCVD";
        public const string DROApplicationStatusApproved = "APRD";
        public const string DROApplicationStatusCancelled = "CNLD";
        public const string DROApplicationStatusDenied = "DNID";
        public const string DROApplicationStatusNullified = "NLFD";
        public const string DROApplicationStatusQualified = "QLFD";
        public const string DROApplicationStatusPendingNullified = "PDNF";

        public const string DROApplicationModelActiveDBFormerModel = "ADFM";
        public const string DROApplicationModelActiveDBModel = "ADBM";
        public const string DROApplicationModelActiveJobServiceModel = "AJSM";
        public const string DROApplicationModelDCModel = "DCMD";
        public const string DROApplicationModelRetiredJobServiceModel = "RJSM";
        public const string DROApplicationModelDeferredCompModel = "DFCM";
        public const string DROApplicationModelRetireeDBModel = "RDBM";

        public const string DROApplicationModelTimeOfBenfitUserEnteredDate = "UEDT";
        public const string DROApplicationModelTimeOfBenfitEarlyRetirementDate = "ERTA";
        public const string DROApplicationModelTimeOfBenfitNormalRetirementDate = "NTRA";
        public const string DROApplicationModelTimeOfBenfitRetirementDate = "RTRD";


        public const string DROApplicationPaymentStatusApproved = "APRD";
        public const string DROApplicationPaymentStatusPending = "PEND";
        public const string DROApplicationPaymentStatusProcessed = "PRCD";
        public const string DROApplicationPaymentStatusVerified = "VRFD";

        public const string DROApplicationPaymentTypeMonthlyBenefit = "MNBS";
        public const string DROApplicationPaymentTypeDeath = "DTSC";
        public const string DROApplicationPaymentTypeRefundSchedule = "RFSC";

        public const string PaymentItemTypeTaxableAmount = "ITEM37";
        public const string PaymentItemTypeNonTaxableAmount = "ITEM38";

        public const string PERSLinkGoLiveDate = "PGLD";

        public const int Map_Initialize__Enter_and_Qualify_DRO_Application = 263;

        //used for uCS -054
        public const string BenefitDurationValueLifeOfBenAccOwnerSingleLifeAnnuity = "LBSL";
        public const string BenefitDurationValueLifeOfBenAccOwnerStraightLife = "LOST";
        public const string BenefitDurationValueLifeOfBenAccOwner15YrTermCertainAndLifeOption = "LB15";
        public const string BenefitDurationValueLifeOfBenAccOwner5YrTermCertainAndLifeOption = "LB05";
        public const string BenefitDurationValueLifeOfBenAccOwner20YrTermCertainAndLifeOption = "LB20";
        public const string BenefitDurationValueLifeOfBenAccOwner10YrTermCertainAndLifeOption = "LB10";
        public const string BenefitDurationValueLifeOfAP5yrTermcertainAndLifeOption = "L05C";
        public const string BenefitDurationValueLifeofAP10yrTermcertainAndLifeOption = "L10C";
        public const string BenefitDurationValueLifeofAP20yrTermCertainAndLifeOption = "L20C";
        public const string BenefitDurationValueLifeofAP10yrperiod = "LA10";
        public const string BenefitDurationValueLifeofAP15yrPeriod = "LA15";
        public const string BenefitDurationValueLifeofAP20yrperiod = "LA20";

        //used for correspondence

        #endregion

        #region Deposit Tape

        public const string PullACHReadyStatus = "APRD";
        public const string PullACHCompleteStatus = "APCM";

        #endregion

        #region UCS - 081 Field Names
        public const string FieldNameDeathIndicator = "istrDeathIndicator";
        //For correspondence
        public const string BenefitOptionNotApplied = "Not Applied For";
        #endregion

        # region Case Management

        public const string CaseTypeBenefitAppeal = "BENA";
        public const string CaseTypeDisabilityRecertification = "DISR";
        public const string CaseTypeFinancialHardship = "FINH";
        public const string CaseTypePre1991DisabilityRecertification = "PRED";
        public const string BenefitAppealTypeRetirement = "RETR";
        public const string BenefitAppealTypeQDRO = "QDRO";
        public const string CaseStepStatusValuePending = "PEND";

        public const string CaseStatusValueApproved = "APPR";
        public const string CaseStatusValuePendingMember = "PENM";
        public const string CaseStatusValuePending3rdParty = "PEN3";
        public const string CaseStatusValuePendingNDPERS = "PENN";
        public const string CaseStatusValueCancelled = "CANC";

        public static DateTime Pre1991Disability = new DateTime(1991, 07, 01);

        public const int CodeIDCaseStep = 2258;

        # endregion

        #region Convert Disability to Normal

        public const int ConvertedToNormalByRule = 1;
        public const int ConvertedToNormalByAge = 2;

        public const string TerminationReasonEarlyToDisability = "ERDI";
        public const string TerminationReasonDisabilityToNormal = "DTNR";

        public const int Map_Process_Disability_to_Normal_Age_Conversion = 267;
        public const int Map_Process_Disability_to_Normal_Rule_Conversion = 268;

        #region Enums
        public enum DataType
        {
            String,
            DateTime,
            Numeric
        }
        #endregion

        #endregion

        #region UCS - 075 Field Names

        public const string PaymentScheduleStatusReview = "REVW";
        public const string PaymentScheduleStatusValid = "VALD";
        public const string PaymentScheduleStatusProcessed = "PRCD";

        public const string PaymentScheduleActionStatusPending = "PEND";
        public const string PaymentScheduleActionStatusFailed = "FILD";
        public const string PaymentScheduleActionStatusCancelled = "CNLD";
        public const string PaymentScheduleActionStatusProcessed = "PRCD";
        public const string PaymentScheduleActionStatusReadyforFinal = "RDFN";
        public const string PaymentScheduleActionStatusTrialExecuted = "TREX";
        public const string PaymentScheduleScheduleTypeMonthly = "MTLY";
        public const string PaymentScheduleScheduleTypeAdhoc = "ADHC";

        public const string CheckComponentGroupAdjustments = "ADJS";
        public const string CheckComponentGroupDeductions = "DEDT";
        public const string CheckComponentGroupGross = "GRSS";
        public const string CheckComponentGroupRollover = "RLOR";
        public const string CheckComponentDescriptionDues = "Dues";
        public const string BankAccountSavings = "SAVE";
        public const string BankAccountChecking = "CHKG";

        public const string PersonAccountBankAccountSavings = "SAV";
        public const string PersonAccountBankAccountChecking = "CHK";

        public const string PaymentScheduleStepStatusPending = "PEND";
        public const string PaymentScheduleStepStatusProcessed = "PRCD";
        public const string PaymentScheduleStepStatusFailed = "FALD";
        public const string CreditTransactionCodeNonPrenoteChecking = "22";
        public const string CreditTransactionCodePrenoteChecking = "23";
        public const string CreditTransactionCodeNonPrenoteSavings = "32";
        public const string CreditTransactionCodePrenoteSavings = "33";

        public const string DebitTransactionCodeNonPrenoteChecking = "27";
        public const string DebitTransactionCodePrenoteChecking = "28";
        public const string DebitTransactionCodeNonPrenoteSavings = "37";
        public const string DebitTransactionCodePrenoteSavings = "38";
        public const int MonthlyPaymentBatchScheduleID = 80;
        public const int AdhocPaymentBatchScheduleID = 86;
        public const int AdhocTrialBatchScheduleID = 88;

        public const string PaymentStatusCancelled = "CNLD";
        public const string PaymentDistributionStatusCleared = "CLRD";
        public const string PaymentDistributionStatusCancelled = "CNLD";
        public const string PaymentDistributionPaymentMethodACH = "ACH";
        public const string PaymentDistributionPaymentMethodCHK = "CHK";
        public const string PaymentDistributionPaymentMethodRACH = "RACH";
        public const string PaymentDistributionPaymentMethodRCHK = "RCHK";
        public const string PaymentDistributionPaymentMethodWIRE = "WIRE";
        public const string PaymentDistributionStatusCHKOutstanding = "COTS";
        public const string PaymentDistributionStatusACHOutstanding = "AOTS";
        public const string VendorPaymentItemDCContrib = "ITEM604";
        public const string VendorPaymentItemDefContribFidelity = "ITEM600";
        public const string VendorPaymentItemDefContrib = "ITEM602";
        public const string VendorPaymentItemLTC = "ITEM620";
        public const string VendorPaymentItemEAP = "ITEM618";
        public const string VendorPaymentItemHMO = "ITEM610";
        public const string VendorPaymentItemHealth = "ITEM606";
        public const string VendorPaymentItemBuydown = "ITEM637"; // PIR 11239
        public const string VendorPaymentItemMedicarePartDAmount = "ITEM638"; //PIR 14271
        public const string VendorPaymentItemMedicarePartD = "ITEM608";
        public const string VendorPaymentItemHealthFee = "ITEM607";
        public const string VendorPaymentItemMedicarePartDFee = "ITEM609";
        public const string VendorPaymentItemLife = "ITEM612";
        public const string VendorPaymentItemDental = "ITEM614";
        public const string VendorPaymentItemVision = "ITEM616";
        public const string PaymentItemCapitalGain = "ITEM5";
        public const string SystemConstantFedTaxVendor = "FTVN";
        public const string SystemConstantStateTaxVendor = "STCM";
        public const string VendorPaymentItemStateTax = "ITEM624";
        public const string ItemUsageMontlhyPayment = "MNTP";
        public const string ItemUsageOneTimePayment = "ONTP";
        public const string CheckTypeValue = "REGL";

        public const int CheckFileID = 69;
        public const int ACHFileID = 68;
        public const int DCFileID = 67;

        public const string IRSLimitBatchRunMonth = "IRSM";

        public const string InsurancePaymentItemHealth = "ITEM76";
        public const string InsurancePaymentItemLife = "ITEM77";
        public const string InsurancePaymentItemDental = "ITEM78";
        public const string InsurancePaymentItemVision = "ITEM79";
        public const string InsurancePaymentItemLTC = "ITEM80";
        public const string InsurancePaymentItemMedicare = "ITEM120"; // PIR 18243

        public const string ACHCheckGroupValueFedTax = "FDTX";
        public const string ACHCheckGroupValueStateTax = "STTX";

        public const string PaymentStatusOutstanding = "OUST";
        public const string PaymentStatusProcessed = "PRCD";
        public const string PaymentStatusCancelPending = "CPND";
        public const string OrgTypeRollover = "RLIT";

        public const string PayeeDetailFedTax = "FEDX";
        public const string PayeeDetailStateTax = "STTX";

        public const int Monthly_Start_Payment_Process = 100;
        public const int Monthly_Trial_Reports = 200;
        public const int Monthly_Total_Per_Items_Report = 210;
        public const int Monthly_New_Retiree_Detail_Report = 220;
        public const int Monthly_Reinstated_Retiree_Detail_Report = 230;
        public const int Monthly_Closed_Terminated_Suspended_Cancelled_Payees = 240;
        public const int Monthly_Retirement_Option_Summary_Report = 250;
        public const int Monthly_Benefit_Payment_Change_Report = 260;
        public const int Monthly_Monthly_Benefit_Payment_Summary_Report = 270;
        public const int Monthly_NonMonthly_Payment_Detail_Report = 280;
        public const int Monthly_Backup_Data_Prior_to_Batch = 300;
        public const int Monthly_Create_Payment_History_ = 400;
        public const int Monthly_Creating_Check_History_for_Payees = 410;
        public const int Monthly_Creating_ACH_History_for_Payees = 420;
        public const int Monthly_Creating_Check_History_Rollover = 430;
        public const int Monthly_Creating_Check_History_Payees_for_Payments_to_be_Reissued = 440;
        public const int Monthly_Creating_Check_History_Rollover_for_Payments_to_be_Reissued = 450;
        public const int Monthly_Creating_GL = 500;
        public const int Monthly_Create_Check_File = 600;
        public const int Monthly_Create_ACH_File = 700;
        public const int Monthly_Premium_Payments_for_IBS = 800;
        public const int Monthly_Updating_Person_account_Information = 900;
        public const int Monthly_Final_Total_Per_Items_Report = 1000;
        public const int Monthly_Create_ACH_Cover_Letter = 1100;
        public const int Monthly_Create_Payment_Register = 1200;
        public const int Monthly_Create_Vendor_Payment_Summary = 1300;
        public const int Monthly_Create_Dues_Withholding_Report = 1400;
        public const int Monthly_Create_Minimum_Guarantee_Change_Summary_Report = 245;
        public const int Monthly_Prepare_Payee_Account_for_Next_Pay_Period_ = 1600;
        public const int Monthly_Updating_Recovery_Payment_Corrections = 1610;
        public const int Monthly_Closing_Recovered_Corrections = 1620;
        public const int Monthly_Updating_Underpayment_Corrections = 1630;
        public const int Monthly_Updating_Item_History_for_Unrecovered_Corrections = 1640;
        public const int Monthly_Updating_NonTaxable_Amount_ = 1650;
        public const int Monthly_Updating_Application_Status_to_Processed_ = 1660;
        public const int Monthly_Updating_Benefit_Calculation_Status_to_Processed_ = 1670;
        public const int Monthly_Updating_Payee_Status_to_Receiving_Processed = 1680;
        public const int Monthly_Recalculating_Taxes = 1690;
        public const int Monthly_Generate_Correspondences = 1695;
        public const int Monthly_Close_Payment_Payroll_Process = 1700;
        public const int Adhoc_Start_Payment_Process = 1710;
        public const int Adhoc_Backup_data_Prior_to_Batch = 1720;
        public const int Adhoc_Update_Payment_Date_and_Interest_for_Transfer = 1740;
        public const int Adhoc_Trial_Adhoc_Reports = 1750;
        public const int Adhoc_Total_Per_Items_Report = 1760;
        public const int Adhoc_Payee_List_Report = 1790;
        public const int Adhoc_Create_Payment_History_ = 1800;
        public const int Adhoc_Creating_for_Payees = 1810;
        public const int Adhoc_Creating_for_Rollover = 1820;
        public const int Adhoc_Creating_for_Reissued_Distribution = 1830;
        public const int Adhoc_Creating_GL = 1840;
        public const int Adhoc_Create_Check_File = 1900;
        public const int Adhoc_Create_ACH_File = 1910;
        public const int Adhoc_Updating_Person_Account_Information = 1920;
        public const int Adhoc_Final_Total_Per_Items_Report = 1930;
        public const int Adhoc_Create_ACH_Cover_Letter = 1940;
        public const int Adhoc_Create_Payment_Register = 1950;
        public const int Adhoc_Create_DC_Transfer_File = 1960;
        public const int Adhoc_Create_Vendor_Payment_Summary = 1970;
        public const int Adhoc_Prepare_Payee_Account_for_the_Next_Benefit_Payment = 1980;
        public const int Adhoc_Generate_Correspondences = 1990;
        public const int Adhoc_Close_Payment_Payroll_Process = 2000;
        public const int Monthly_Vendor_Payment_Summary = 290;
        public const int Adhoc_Vendor_Payment_Summary = 1770;
        public const int Adhoc_Premium_Payments_for_IBS = 1915;
        public const string OrgBankStatusActive = "ACTV";
        public const string PayeeAccountStatusSuspensionReasonRTW = "RTNW";
        public const int NDSupremeCourtOrgId = 463;
        #endregion

        #region UCS-093 Required Minimum Distribution

        public const string LifeExpectancyTypeSingle = "SING";
        public const string LifeExpectancyTypeUniform = "UNIF";

        public const string DeferredPaymentStatusClosed = "CLOS";
        public const string DeferredPaymentStatusReceiving = "RECV";

        #endregion

        #region UCS - 092

        public const string AdhocActuaryFileStatusPending = "PEND";
        public const string AdhocActuaryFileStatusProcessed = "PRSD";

        public const string PensionFileTypeAdhoc = "ADHC";
        public const string PensionFileTypeAnnual = "ANNL";

        public const string AdhocActuaryFileTypeDecrmentFile = "ADTF";
        public const string AdhocActuaryFileTypeRHICFile = "ARHF";
        public const string AdhocActuaryFileTypePensionFile = "APPF";

        public const string DecrementReasonCancelled = "CNLD";
        public const string DecrementReasonDisabilityRetirement = "DISR";
        public const string DecrementReasonPriorToRetirement = "DPTR";
        public const string DecrementReasonDeathWhileActive = "DWAC";
        public const string DecrementReasonEndOfTermCertainPeriod = "ETCP";
        public const string DecrementReasonNonVestedTermNoWithdrawal = "NVTW";
        public const string DecrementReasonNonVestedWithdrawal = "NVWD";
        public const string DecrementReasonPaymentCompleted = "PMCP";
        public const string DecrementReasonPostRetirementDeath = "PSRD";
        public const string DecrementReasonServiceRetirement = "SRRT";
        public const string DecrementReasonSuspensionofPension = "SSOP";
        public const string DecrementReasonTransferredDC = "TRDC";
        public const string DecrementReasonTransferredTFFR = "TRTF";
        public const string DecrementReasonTransferredTIAACREF = "TRTI";
        public const string DecrementReasonVestedWithdrawal = "VSTW";
        public const string DecrementReasonVestedTermNoWithdrawal = "VTNW";

        public const string EmploymentEndDateAfterDeath = "EDT_AFT_DTH";
        public const string EmploymentEndDateBeforeDeath = "EDT_BEF_DTH";

        #endregion

        #region UCS-078

        public const string HistoryHeaderStatusReceivablePending = "RPND";
        public const string HistoryHeaderStatusReceivableCreated = "RCCD";
        public const string HistoryHeaderStatusCancelPending = "CPND";
        public const string HistoryHeaderStatusCancel = "CNLD";
        public const string HistoryHeaderStatusCancelPriorPayment = "CPYP";
        //PIR 16219
        public const string HistoryHeaderStatusEscheatToNDPERS = "ESND";
        public const string HistoryHeaderStatusEscheatToState = "ESST";


        public const string ACHReissueApproved = "RAP";
        public const string ACHCancelled = "ACLD";
        public const string ACHCleared = "ACRD";
        public const string ACHOutstanding = "AOTS";
        public const string ACHReceivablesCreated = "ARCD";
        public const string ACHReceivablePending = "ARP";
        public const string ACHReissued = "ARSD";
        public const string ACHVoidPending = "AVPD";

        public const string CHKEscheatReissueApproved = "CERA";
        public const string CHKEscheatedReissuePending = "CERP";
        public const string CHKEscheattoNDPERS = "CESN";
        public const string CHKEscheattoState = "CEST";
        public const string CHKCleared = "CLRD";
        public const string CHKCancelled = "CNLD";
        public const string CHKOutstanding = "COTS";
        public const string CHKReceivablesCreated = "CRCD";
        public const string CHKReceivablesPending = "CRCP";
        public const string CHKReissueApproved = "CRSA";
        public const string CHKReissued = "CVAC";
        public const string CHKStopPayPending = "CSPP";
        public const string CHKVoidPending = "CVPD";
        public const string CHKEscheatedReissued = "ERED";

        public const string RACHReceivablePending = "RARP";
        public const string RACHVoidPending = "RAVP";
        public const string RACHCancelled = "RCLD";
        public const string RACHCleared = "RCRD";
        public const string RACHReceivablesCreated = "RCTD";
        public const string RACHOutstanding = "ROTS";
        public const string RACHReissueApproved = "RRAP";
        public const string RACHReissued = "RRSD";

        public const string RCHKEscheatReissueApproved = "RERA";
        public const string RCHKEscheatedReissuePending = "RERP";
        public const string RCHKEscheattoNDPERS = "RSND";
        public const string RCHKEscheattoState = "RSST";
        public const string RCHKCleared = "RRCL";
        public const string RCHKCancelled = "RRCD";
        public const string RCHKOutstanding = "RROT";
        public const string RCHKReceivablesCreated = "RRBD";
        public const string RCHKReceivablesPending = "RCHP";
        public const string RCHKReissueApproved = "RRSA";
        public const string RCHKReissued = "RRRS";
        public const string RCHKStopPayPending = "RRSP";
        public const string RCHKVoidPending = "RVPD";
        public const string RCHKEscheatedReissue = "RRED";

        public const string DistributionStatusOutstanding = "OUTS";
        public const string DistributionStatusStopPayPending = "SPP";
        public const string DistributionStatusVoidPending = "VPND";
        public const string PaymentItemReissuetoPayeeAmount = "ITEM634";

        #endregion

        # region UCS 84
        public const string PostRetirementIncreaseTypeValueCOLA = "COLA";
        public const string PostRetirementIncreaseTypeValueAdHoc = "ADHO";
        public const string PostRetirementIncreaseTypeValueSupplemental = "SUPP";
        public const string PostRetirementIncreaseBatchTypeValueProcessed = "PROC";
        public const string PostRetirementIncreaseBatchTypeValueUnProcessed = "NPRO";
        public const string PAPITCOLABase = "ITEM39";
        public const int PAPITCOLABaseItemId = 34;
        public const string PAPITAdhoc = "ITEM40";
        public const string PAPITTaxableAmountOneTimePayment = "ITEM24";
        public const string PAPITFedTaxableOneTimePayment = "ITEM27";
        public const string PAPITNDStateTaxableAmountOneTimePayment = "ITEM28";
        public const int PAPITAdhocItemId = 35;


        //for correspondence
        public const string PAPITChildSupport = "ITEM72";
        public const string PAPITNDTaxLevy = "ITEM73";
        public const string PAPIT3rdPartyHealthInsurance = "ITEM71";
        public const string PAPITAFPEDues = "ITEM74";
        public const string PAPITNDPEADues = "ITEM74";
        public const string PAPITSupplementalItem = "ITEM24";

        public const string NDPEAValue = "NDEA";
        public const string AFPEValue = "AFPE";

        public const string MetroPolitanLifeInsuranceCompany = "MTRO";

        # endregion

        public enum PayeeStatusForRTW
        {
            SuspOrCompOrCancelled,
            SuspendedOnly,
            IgnoreStatus
        }

        #region UCS - 077
        public const int Map_Process_Cancel_Payment_History = 287;
        public const int Map_Process_Uncashed_Benefit_Checks = 290;
        #endregion

        #region UCS - 096
        public const string CAFRReportStatusPending = "PEND";
        public const string CAFRReportStatusProcessed = "PRSD";
        public const string CAFRReportStatusFailed = "FALD";

        public const string CAFRReportHealthInsurance = "HI";
        public const string CAFRReportLifeInsurance = "LI";
        public const string CAFRReportDentalInsurance = "D";
        public const string CAFRReportVisionInsurance = "V";
        public const string CAFRReportLTCEAPDCInsurance = "LED";

        public const string CAFRReportTitle = "CAFR Report Effective ";

        #endregion

        #region UCS - 079
        public const string BenRecalAdjustmentReasonRecalculation = "RCAL";
        public const string BenRecalAdjustmentReceivableCreated = "RCCP";

        public const string RecoveryStatusApproved = "APRD";
        public const string RecoveryStatusCancel = "CANL";
        public const string RecoveryStatusInProcess = "INPR";
        public const string RecoveryStatusPendingApproval = "PEAP";
        public const string RecoveryStatusSatisfied = "SATD";
        public const string RecoveryStatusWriteOff = "WROF";

        public const string RecoveryRePaymentTypeLifeTimeReduction = "LTRD";
        public const string RecoveryRePaymentTypeLifeTimeReductionForRTW = "LTRW";
        public const string RecoveryRePaymentTypeLumpSum = "LUSM";
        public const string RecoveryRePaymentTypeRepayOverTime = "RPOT";

        public const string RecoveryPaymentOptionNDPERSPensionChk = "NPCK";
        public const string RecoveryPaymentOptionPersonalChk = "PRCK";


        public const string AdjustmentReasonRecalculation = "RECL";
        public const string AdjustmentReasonPopUp = "POPP";
        public const string AdjustmentReasonFirstCheckRetro = "FCRT";
        public const int Correction1099rYrs = 5;

        public const string CancelPaymentWith1099r = "CWHR";
        public const string CancelPaymentWithout1099r = "CWTR";
        public const string CancelPaymentWithOrWithout1099r = "CNLD";

        public const string AdhocIncreasePaymentItem = "ADHC";
        public const string COLAPaymentItem = "COLA";
        public const string RegularPaymentItem = "REGL";
        #endregion

        # region UCS - 40
        //status for Rate change
        public const string LetterStatuValuePending = "PEND";
        public const string LetterStatuValueProcesssed = "PROC";
        public const string LetterStatuValueFailed = "FAIL";

        public const string LetterTypeValueRHIC = "RHIC";
        public const string LetterTypeValueLIFE = "Life";
        public const string LetterTypeValueDental = "DNTL";
        public const string LetterTypeValueVision = "VISN";
        public const string LetterTypeValueEap = "EAPP";
        public const string LetterTypeValueHealth = "HLIN";
        public const string LetterTypeValueLTC = "LTCP";
        public const string LetterTypeValueMedicare = "MDPD";

        //status for Dues change       
        //public const string StatusValid = "VALD";
        public const string StatusUnProcessed = "UNPC";
        public const string StatusApproved = "APPR";
        public const string RetirementAndInvestmentOrgCodeId = "019000";

        public const string lstrLifeAgeChangeLetterMonthData1 = "LACM";
        # endregion

        #region Member Annual Statement

        public const string BatchRequestTypeAnnual = "ANNL";
        public const string BatchRequestTypeIndividual = "INON";
        public const string BatchRequestTypeTargeted = "TGRP";

        public const string GroupTypeRetired = "RETR";
        public const string GroupTypeNonRetired = "NRTR";

        public const string BatchRequestActionStatusProcessed = "PROC";
        public const string BatchRequestActionStatusFailed = "FAIL";
        public const string BatchRequestActionStatusPending = "PEND";

        public const string SummaryStatementFile = "SUMM";
        public const string OnlineStatementFile = "ONLN";

        #endregion

        //PIR-8946 Start
        public const string Annual1099RBatch = "ANNB";
        public const string MonthlyCorrectionBatch = "CORR";
        public const string Monthly1099RReportsBatch = "MNTB";
        //PIR-8946 End

        #region Batch Schedule ID
        public const int BatchScheduleIDProcessCOLABatch = 96;
        public const int BatchScheduleIDProcessAdhocBatch = 97;
        public const int BatchScheduleIDProcessSupplemental = 98;

        public const int BatchScheduleIDUpdateDuesRate = 102;
        public const int BatchScheduleIDUpdateRHICRateChange = 113;

        #endregion

        #region UCS - 091

        public const string BatchRequest1099rStatusPending = "PEND";
        public const string BatchRequest1099rStatusApproved = "APRD";
        public const string BatchRequest1099rStatusCancelled = "CNLD";
        public const string BatchRequest1099rStatusProcessed = "PRSD";

        public const string BatchRequest1099rTypeAnnual = "ANNB";
        public const string BatchRequest1099rTypeMonthly = "CORR";
        public const string Monthly1009rRequestBatch = "MNTB"; //PIR-8946
        public const string Monthly1009rRequestBatchDescription = "Monthly 1099R Reports Batch"; //PIR-8946

        public const string Report1099rPath = "RptGN1099r";
        public const string ReportMASPath = "ReprtGNMAS";
        public const string ReportESSPath = "RptGNESS"; //PIR 10103
        public const string ReportPath = "BatchRptGN";
	    public const string PaymentReportPath = "RptPayment"; //PIR 10808

        #endregion

        #region GL

        public const string GLStatusTransitionRecovery = "REST";
        public const string GLStatusTransitionACH = "ACH";
        public const string GLStatusTransitionRACH = "RACH";
        public const string GLStatusTransitionCHK = "CHK";
        public const string GLStatusTransitionRCHK = "RCHK";
        public const string GLStatusTransitionPaymentHistory = "PHHS";
        public const string GLStatusTransitionApproved = "APVD";

        public const string GLTransactionTypeStatusTransition = "STAT";

        public const string GLSourceTypeValuePensionRecv = "PNRB";
        public const string GLSourceTypeValueChkMaintenance = "CHMN";
        public const string GLSourceTypeValueBenefitPayment = "BEPY";
        public const string GLSourceTypeValueInsuranceTransfer = "INTR";
        public const string GLSourceTypeValueVendorPayment = "VPMT";
        public const string GLSourceTypeValueRecovery = "RCVR";

        public const string PaymentItemCodeValueRecvInterest = "RCVI";
        public const string PaymentItemCodeValueAmortizedInterest = "AINT";
        public const string PaymentItemCodeValueNetAmountReissue = "P448";
		//PIR 16219
        public const string PaymentItemCodeValueNetAmountEscheatReissue = "P458";
        public const string PaymentItemTypeCodeNetAmountEscheatReissue = "ITEM458";
        public const string PaymentItemCodeValueFedTaxAmount = "P067";
        public const string PaymentItemCodeValueStateTaxAmount = "P068";
        //prod pir 5141 & 5142
        public const string PaymentItemCodeValueExcessIBSInsuranceRefund = "P454";

        //PIR 16219
        public const string PaymentItemCodeValueNetAmountEscheatToNDPERS = "P451";
        public const string PaymentItemCodeValueNetAmountEscheatToState = "P456";

        #endregion

        # region UCS 24
        public const string PlanParticipationStatusDescriptionEligible = "Eligible";
        public const string FlexCompPaymentOptionDirectDeposit = "Direct Deposit";
        public const string FlexCompPaymentOptionCheck = "Check";
        public const string FlexCompMailOptionInsideMail = "Inside Mail";
        public const string FlexCompMailOptionUSPostalService = "US Postal Service";
        public const string EEAcknowledgementAC16 = "AC16";
        public const string EEAcknowledgementAC10 = "AC10";
        public const string EEAcknowledgementAC17 = "AC17";
        public const string EnrollRequestStatusPendingRequest = "PEND";
        public const string EnrollRequestStatusPosted = "PSTD";
        public const string EnrollRequestStatusChildRqt = "CHID";
        public const string EnrollRequestStatusRejected = "RJTD";
        public const string EnrollRequestStatusIgnored = "IGNR";
        public const string EnrollRequestStatusFinishLater = "FLTR";

        public const string PlanParticipationStatusDescriptionWaived = "Waived";
        //************ for retirement
        public const string EnrollmentTypeDBRetirement = "DBRE";
        public const string EnrollmentTypeDBOptional = "DBOP";
        public const string EnrollmentTypeDBElectedOfficial = "DBEL";
        public const string EnrollmentTypeDCOptional = "DCOP";
        public const string EnrollmentTypeDCRetirement = "DCRE";


        //************ for insurance
        public const string EnrollmentTypeEAP = "EAPP";
        public const string EnrollmentTypeLTC = "LTCP";
        public const string EnrollmentTypeGHDV = "GHDV";
        public const string EnrollmentTypeFlexComp = "FLEX";
        public const string EnrollmentTypeLife = "LIFE";

        public const string ChangeReasonAnnualEnrollment = "ANNE";
        public const string ChangeReasonNewHire = "NEWH";

        public const string AcknowledgementTypeDB = "DB";
        public const string AcknowledgementTypeDC = "DC";
        public const string AcknowledgementTypeDF = "DF";
        public const string AcknowledgementTypeEDF = "EDF";

        public const string PeopleSoftURl = "www.connectnd.us/psp/ndrp/?cmd=login&languageCd=ENG&";

        public const string AnnualEnrollmentRequestStatus = "Action Required";
        public const string EnrollmentStatusEligible = "Eligible to Enroll";
        public const string EnrollmentStatusUnderReview = "Under NDPERS Review";
        public const string EnrollmentStatusRejected = "Rejected";
        public const string EnrollmentStatusIgnored = "Ignored";
        public const string EnrollmentStatusPending = "PEND"; //PIR-9702
        public const string EnrollmentStatusPosted = "Completed"; // PIR - 15126
        

        public const string PlanEnrollmentOptionValueCancel = "CANL";
        public const string PlanEnrollmentOptionValueEnroll = "ENRL";
        public const string PlanEnrollmentOptionValueWaive = "WAIV";
        public const string PlanEnrollmentOptionValueAddRemoveDependent = "ADRD";
        public const string PlanEnrollmentOptionValueReturnFromLeave = "RTFL";
        public const string PlanEnrollmentOptionValueQualifiedChangeInStatus = "QCIS";

        public const string ReasonValueNotApplicable = "NOAP";
        public const string ReasonValueMarriage = "MARR";
        public const string ReasonValueNewHire = "NEWH";
        public const string ReasonValueBirth = "BIRT";
        public const string ReasonValueRemoveDependent = "RMVD";
        public const string ReasonValueWaivePlan = "WAIV";
        public const string ReasonValueEmploymentStatusChange = "EMCH";
        public const string ReasonValueADEC = "ADEC"; //PIR 25920

        public const string ChangeEffectiveDateValueFirstofMonthFollowing = "FOMF";

        public const string ChangeEffectiveDefaultText = "When the change is effective? ";

        public const string PlanEnrollmentQuickEnrollment = "QUIK";
        public const string PlanEnrollmentRegularEnrollment = "REEN";

        public const string EstimatedUnusedServicePurchaseType = "UNUS";
        public const string EstimatedAdditionalServicePurchaseType = "ADDP";
        public const string EstimatedBothServicePurchaseType = "BOTH";

        # endregion

        #region UCS - 001

        public const string UserTypeInternal = "INTL";
        public const string UserTypeEmployer = "EMPL";
        public const string UserTypeMember = "MEMR";

        public const string ESSQuestionEmployerType = "EMTY";
        public const string ESSQuestionPrimaryZip = "ZIPC";
        public const string ESSQuestionContactRole = "CORO";

        #endregion

        #region Portal

        public const string DebitACHRequestWorkflowOrgCode = "019200";
        public const string Question_DateOfBirth = "DOBI";
        public const string Question_DateOfBirthForSpouse = "DBSP";
        public const string Question_MostRecentNetBenefitAmount = "MRNB";
        public const string Question_SpouseSSN = "SPSS";
        public const string Question_SSN = "SOSN";
        public const string Question_CurrentAddressZipCode = "ZICU";
        public const string WSS_MessageBoard_Priority_High = "HIGH";
        public const string WSS_MessageBoard_Priority_Low = "LOW";
        public const string WSS_MessageBoard_Audience_Member = "MEMB";
        public const string WSS_MessageBoard_Audience_Employer = "EMPL";

        public const string WSS_Debit_ACH_Request_Status_Pending = "PEND";

        public const int Map_Process_Online_Contact_Ticket = 302;
        public const int Map_Process_Online_Benefit_Estimate_Request = 303;
        public const int Map_Process_Online_Service_Purchase_Request = 304;
        public const int Map_Process_Seminar_IDB = 305;
        public const int Map_MSS_Schedule_Appointment = 310;
        public const int Map_ESS_Schedule_Appointment = 311;
        public const int Map_WSS_Death_Notification = 312;

        #endregion

        #region UCS - 032

        public const string WSSDebitBatchRequestProcessed = "PRSD";

        public const string WSSMessagePriorityHigh = "HIGH";
        public const string WSSMessagePriorityLow = "LOW";

        public const string WSSMessageAudienceMember = "MEMB";
        public const string WSSMessageAudienceEmployer = "EMPL";

        public const string WSSMessagePersonTypePayee = "PAYE";
        public const string WSSMessagePersonTypeNonPayee = "NPYE";

        public const string WSSMailFrom = "DONOTREPLY@NDPERS.GOV";
        public const string WSSMailSubject = "NDPERS Correspondence";
        public const string WSSMailBody = "Correspondence has been posted on the NDPERS Web Self Service. Please login to view the correspondence.";
        public const string WSSMessageForCorrs = "{0} correspondence is available for you to download."; //PIR 8889 - Change text

        public const string RemittanceReport = "rptRemittanceReport.rpt";

        public const string MessageSeverityError = "E";
        public const string MessageSeverityWarning = "W";
        public const string MessageSeverityInformation = "I";
        public const string MessageSeverityFatal = "F";

        public const string LTCRates = "LTCR";
        public const string LifeRates = "GPLF";
        public const string EmploymentChangeRequestStatusReview = "REVW";
        public const string EmploymentChangeRequestStatusProcessed = "PRCD";
        public const string EmploymentChangeRequestStatusRejected = "RJTD";
        public const string EmploymentChangeRequestStatusIgnored = "IGNR";
        public const string EmploymentChangeRequestStatusPendingAutoPosting = "PEND";

        public const string EmploymentChangeRequestChangeTypeClassification = "CLSC";
        public const string EmploymentChangeRequestChangeTypeEmployment = "ETCG";
        public const string EmploymentChangeRequestChangeTypeLOA = "LOAL";
        public const string EmploymentChangeRequestChangeTypeLOAM = "LOAM"; //pir 8127
        public const string EmploymentChangeRequestChangeTypeLOAR = "LOAR"; //PIR 8560
        public const string EmploymentChangeRequestChangeTypeTEEM = "TEEM";//PIR 14474
        public const string MemberRecordRequestWizard = "MRRW";
        public const int Map_Employment_Change_Request = 313;
        public const int Map_Member_Record_Request = 314;

        public const string MSSPortal = "MSS";
        public const string ESSPortal = "ESS";
        public const string BothPortal = "BOTH";

        #endregion

        #region ucs 55
        public enum donor_payee_account_type
        {
            member_db,
            member_dc,
            active_spouse,
            deceased_spouse,
            pre_retirement,
            post_retirement
        }

        public enum automatic_rhic_combine_trigger
        {
            initial_payment_approval,
            payment_completed_for_death_member,
            spouse_rhic_for_termination_of_death_member,
            payment_cancelled,
            payment_suspended_for_RTW,
            benefit_adjustment_approval,
            rhic_factor_change,
            enrollment_change,
            online_end_click,
            health_premium_change,
            payment_suspended_or_completed,
            benefit_calculation_approval,
            death_notification_save,
            death_notification_marked_completed
        }
        #endregion

        #region UCS - 038 Addendum

        public const string PersonAccountTransactionTypeIBSAdjPayment = "IBAP";

        public const string IBSAdjustmentRepaymentTypeLumpSum = "LPSM";
        public const string IBSAdjustmentRepaymentTypeInstallment = "INPY";
        public const string IBSAdjustmentRepaymentType3Installment = "3INS";

        public const string IBSAdjustmentStatusPending = "PEND";
        public const string IBSAdjustmentStatusApproved = "APRD";
        public const string IBSAdjustmentStatusCancelled = "CNLD";
        public const string IBSAdjustmentStatusCompleted = "COMT";
        public const string IBSAdjustmentStatusInPayment = "IPAY";

        public const int MapProcessIBSAdjustment = 315;
        public const int MapProcessIBSAdjustmentHeader = 316;

        #endregion

        public const string PriorJudgesOrgCode = "018100";

        #region UAT PIR 2373

        public const string EmploymentChangePermanentToTemporary = "EMPC";
        public const string LevelOfCoverageChange = "LOCC";
        public const string NewEnrollment = "NEEN";
        public const string PlanChangedToCancWithEmpHdrOpen = "PPSC";
        public const string EmploymentHdrEnddated = "TERM";
        public const string AnnualEnrollment = "ANNE";
        public const string AnnualEnrollmentWaived = "ANNT";
        public const string DeferredCompStartDateProvider = "DCSP";//PIR 17081
        public const string DeferredCompEndDateProvider = "DCEP";

        public const string PeopleSoftEmploymentStartDate = "ESDT";
        public const string PeopleSoftFirstDayoftheNextmonthofEmploymentHeaderEndDate = "FDED";
        public const string PeopleSoftFirstDayoftheMonthPriortoHistoryChangeDate = "FDHD";
        public const string PeopleSoftFirstDayofthemonthofEmploymentHeaderEndDate = "FDSM";
        public const string PeopleSoftFirstDayoftheMonthofPermanentEmploymentEndDate = "FDTE";
        public const string PeopleSoftFirstDayoftheMonthFollowingHistoryChangeDate = "FFHC";
        public const string PeopleSoftFirstDayoftheMonthFollowingTemproaryEmploymentStartDate = "FFTE";
        public const string PeopleSoftHistoryChangeDate = "HCDT";
        public const string PeopleSoftTemproaryEmploymentStartDate = "TEST";
        public const string PeopleSoftAnnualEnrollmentCoverageBeginDate = "AECB";
        public const string PeopleSoftAnnualEnrollmentDeductionBeginDate = "AEDB";

        #endregion


        public const string OrganizationPaymentOptionACH = "A";
        public const string OrganizationPaymentOptionCHK = "C";
        public const string OrganizationPaymentOptionWIRE = "W";

        public const string ACHFileNameRetirmentVendorPayment = "DF.BK870034";
        public const string ACHFileNameInsuranceVendorPayment = "DF.BK870066";
        public const string ACHFileNameRetirmentEmployerPayment = "DF.BK870065";
        public const string ACHFileNameInsuranceEmployerPayment = "DF.BK870070";
        public const string ACHFileNameDefCompEmployerPayment = "DF.BK870071";
        public const string ACHFileNameDefCompVendorPayment = "DF.BK870067";

        //PROD Pir 4555
        //Job class value
        public const string JobClassPeaceOfficer = "PEAO";
        public const string JobClassCorrectionalOfficer = "CORO";
        public const string JobClassJudge = "SUPR";
        public const string JobClassHighwayPatrolPerson = "HPPN";

        //PROD Pir 4079
        public const string GLItemTypeOverContribution = "OCRP";
        public const string GLItemTypeOverRHIC = "ORRP";
        public const string GLItemTypeUnderContribution = "UCRP";
        public const string GLItemTypeUnderRHIC = "URRP";
        public const string ConstantAllowableVairance = "ALVN";

        //PROD PIR ID 3253
        public const string SeminarTypeDefCompAgent = "DEFC";
        public const string SeminarTypeNewDefCompAgent = "NDFC";

        public const string NDPERSCodeValue = "NDRS";

        public const string IBSReportTypePositve = "POS";
        public const string IBSReportTypeNegative = "NEG";

        //prod pir 5141 : remittance Refund type
        public const string RemittanceRefundSameMember = "SMME";
        public const string RemittanceRefundSameOrganization = "SMOR";
        public const string RemittanceRefundEstateOfMember = "ESME";
        public const string RemittanceRefundNDPERS = "NDPS";
        public const string RemittanceRefundDifferentOrganization = "DFOR";
        public const string RemittanceRefundDifferentMember = "DFME"; //PIR 16208

        public const string FileHdrStatusUpload = "UPLD";
        public const string FileHdrStatusReview = "REVW";

        //prod pir 5142 : items to be included when receivable is created
        public const string ReceivableCreatedWithout1099r = "REWT";
        public const string ReceivableCreatedWithOrWithout1099r = "RECR";

        //PIR 11131
        public const string Corrected1099R = "Corrected";

        //PROD PIR ID 933
        public const string NonMedicareSplitCountOne = "ONEM";
        public const string NonMedicareSplitCountTwoorMore = "TWOM";
        public const string CoverageCodeSplitFlag = "CVST";

        //prod pir 6323
        public const string RemittanceLookUpAllocationStatusAllocated = "ALTD";
        public const string RemittanceLookUpAllocationStatusNonAllocated = "UALT";

        //prod pir 7029 : new grand father rate at organization level
        public const string NonGrandFatherRateStrucutre = "NGRF";
        //pir 7705
        public const string AlternateStructureCodeHDHP = "HDHP";
        public const string AlternateStructureCodeGFR1 = "GFR1"; // PIR 9115
        public const string HSAProvider = "HSAP";
        public const string BCBSOrgCode = "700040";
        //pir 7259 : life rate file name
        public const string LifeRateFileNameCodeValue = "LFRT";

        //PIR 6961
        public const string LifeCheckBox = "LC";
        public const string GHDVAgreementCheckBox = "GAC";
        public const string MainMemberAuthorization = "MA";
        public const string DBRetirementEnrollmentMemberAuthorization = "ERA";
        public const string MainRetirementEnrollmentMemberAuthorization = "MRA";
        public const string DCRetirementEnrollmentMemberAuthorization = "CRA";
        public const string FlexMemberAuthorization = "FA";
        public const string GHDVAgreementMemberAuthorization = "GAA";
        public const string DentalVisionAgreementMemberAuthorization = "DVAA";
        public const string DBElectedOfficialGeneral1 = "EEG1";
        public const string DBElectedOfficialGeneral2 = "EEG2";
        public const string DBElectedOfficialGeneral3 = "EEG3";
        public const string MainNotice1 = "MN1";
        public const string MainNotice2 = "MN2";
        public const string MainNotice3 = "MN3";
        public const string DBRetirement = "DBRE";
        public const string DBRetirementEnrollmentNotice1 = "ERN1";
        public const string DBRetirementEnrollmentNotice2 = "ERN2";
        public const string DBRetirementEnrollmentNotice3 = "ERN3";
        public const string MainRetirementEnrollmentNotice1 = "MRN1";
        public const string MainRetirementEnrollmentNotice2 = "MRN2";
        public const string DCRetirementEnrollmentNotice1 = "CRN1";
        public const string DCRetirementEnrollmentNotice2 = "CRN2";
        public const string MainOptionalEnrollmentGeneral1 = "MOG1";
        public const string DCOptionalEnrollmentGeneral1 = "COG1";
        public const string MainOptionalEnrollmentGeneral = "MOG";
        public const string MainRetirementEnrollmentNotice = "MRN";
        public const string DCOptionalEnrollmentGeneral = "COG";
        public const string DCRetirementEnrollmentNotice = "CRN";
        public const string GHDVWaiveGeneral = "GWG";
        public const string ConfirmationString = "CONF";
        public const string General = "GENL";
        public const string General2 = "GNL2";
        public const string GHDVWaiveCheck = "GWC";
        public const string Misc = "MISC";
        public const string DeferredCompensation = "DEFC";
        public const int MemberPortalEnrollmentRequestStatus = 1023;
        public const int MemberPortalEnrollmentType = 1026;
        public const string DeferredCompCheck = "DCC";
        public const string DeferredCompWaiveGeneral = "DWG";
        public const string DeferredCompWaiveCheck = "DWC";
        public const string DeferredCompExpediationCheck = "DEC";
        public const string HighDeductibleAcknowledgement = "HDAK";
        public const string PermAdditionalContributionElectionInfo = "ADCI";//PIR 25920
        public const string PermAdditionalContributionElectionAck = "PACA";//PIR 25920
        public const string TempAdditionalContributionElectionAck = "TACA";//PIR 25920
        public const string TempAgreementToParticipate = "TATP";//PIR 25920
        public const int    AnnualSalaryIncreaseMonth = 7;          //PIR 25920
        public const decimal AnnualSalaryIncreaseRate = 3.50m;       //PIR 25920
        public const decimal ContributionRetirementRate = 0.49m;          //PIR 25920
        public const decimal ContributionRateMain = 7.00m;          //PIR 25920
        public const decimal ContributionRateDC = 14.12m;           //PIR 25920
        public const decimal ContributionRateMain2020 = 7.00m;      //PIR 25920
        public const decimal ContributionRateDC2020 = 15.26m;       //PIR 25920

        public const string EEContributionRateDB = "7.00%";          //PIR 25920
        public const string EEContributionRateDC = "7.00%";           //PIR 25920
        public const string ERContributionRateDB = "8.12%";      //PIR 25920
        public const string ERContributionRateDC = "7.12%";       //PIR 25920
        public const string ERContributionRateDB2020 = "9.26%";      //PIR 25920
        public const string ERContributionRateDC2020 = "8.26%";       //PIR 25920

        public const decimal AnnualSalaryIncentiveAmount = 3333;       //PIR 25920
        //PIR 18503
        public const string ACHDetailsAuthorizationPart1 = "ACH1";
        public const string ACHDetailsAuthorizationPart2 = "ACH2";
        public const string ACHDetailsAcknowledgement = "ADAK";

        public const string PersonAccountACHDetailAuth = "PAAD";
        public const string PersonAccountACHDetailAckowlegment = "PAAA";
        public const string UpdatePaymentMethodWizardAcknowledgement = "UPMW";

        public const string PayeeAccountACHDetailOTPSource = "PYAD";
        public const string PersonAccountACHDetailOTPSource = "PRAD";
        public const string MSSMemberAuthenticationOTPSource = "MMAO";

        public const string AutomaticPremiumDeduction = "PRDE";
        public const string AutomaticWithholdingFromBenefitPayment = "BEPT";

        //Pir 7811
        public const string MutualFundWindowFlag = "MFWF";
        //prod pir 6944
        //--start--//
        //pir 8570 start
        public const string New5010HIPAA820LayoutForBCBS = "L820";
        public const string New5010HIPAA820LayoutForCigna = "L20C";
        public const string New5010HIPAA820LayoutForDelta = "L20D"; // PIR 10448
        public const string New5010HIPAA820LayoutForSuperVision = "L20S";
        public const string New5010HIPAALayoutForBCBS = "L834";
        public const string New5010HIPAALayoutForCigna = "LFCI";
        public const string New5010HIPAALayoutForDelta = "LFDE"; // PIR 10025
        public const string New5010HIPAALayoutForSuperVision = "LFSU";
        //pir 8570 end
        public const string Old820RepetitionSeparator = "U";
        public const string New820RepetitionSeparator = "^";
        public const string Old820InterchangeControlVersionNumber = "00401";
        public const string New820InterchangeControlVersionNumber = "00501";
        public const string Old820VersionReleaseIndustryIdentifierCode = "004010X061A1";
        public const string New820VersionReleaseIndustryIdentifierCode = "005010X218";
        public const string New820ImplementationConventionReference = "005010X218";
        public const string New820OriginatingCompanyIdentifier = "1450282090";
        public const string Old820ApplicationReceiversCode = "0001";
        public const string New820ApplicationReceiversCode = "0057";
        //--end--//

        //prod pir 7881
        public const string SeminarTypeCommonDays = "SCEN";
        public const string SeminarTypeWellnessForumDays = "SWEL";
        public const string SeminarTypePayrollConferenceDays = "SPAY";

        //PIR 7703
        public const string BenefitAccountOwner = "BAO";

        //prod pir 6696
        public const string InsurancePremiums = "Insurance Premiums (IBS)";
        public const string RetirementAndInsurancePrenote = "Retirement And Insurance Prenote";
        public const string PensionPayments = "Pension payments";
        public const string RetirementVendorPayment = "Retirement Vendor Payment";
        public const string InsuranceVendorPayment = "Insurance Vendor Payment";
        public const string DefferedcompVendorPayment = "Deffered comp Vendor Payment";
        public const string RetirementEmployerPayment = "Employer Retirement Payment";
        public const string InsuranceEmployerPayment = "Employer Insurance Payment";
        public const string DefferedcompEmployerPayment = "Employer Deffered comp Payment";

        //pir 7006
        public const string ReissuePendingApproval = "RIAP";
        public const string ReissueApproved = "RIA";
        public const string Reissued = "RI";

        //PIR 7987
        public const string PeopleSoftFileBenefitPlanDental = "DENTAL";
        public const string PeopleSoftFileBenefitPlanDenFlx = "DENFLX";
        public const string PeopleSoftFileBenefitPlanVision = "VISION";
        public const string PeopleSoftFileBenefitPlanVisFlx = "VISFLX";
        public const string PeopleSoftFileBenefitPlanLifeBasic = "BASIC";
        public const string PeopleSoftFileBenefitPlanFlxLif = "FLXLIF";
        public const string PeopleSoftFileBenefitPlanLifeSuppl = "SUPPLF";
        public const string PeopleSoftFileBenefitPlanLifeSpouSuppl = "SPLIFE";
        public const string PeopleSoftFileBenefitPlanFlexMSRA = "FSAMED";
        public const string PeopleSoftFileBenefitPlanFlexDCRA = "FSADEP";
        public const string PeopleSoftFileBenefitPlanHealthDAKHDH = "DAKHDH";
        public const string PeopleSoftFileBenefitPlanHealthHDHP = "HDHP";
        public const string PeopleSoftFileBenefitPlanHealthBND = "HSA2"; //PIR 16823 - Issue 1

        public const string PeopleSoftFileBenefitPlanHealthHSAWS = "HSAWS";
        public const string PeopleSoftFileBenefitPlanHealthHSANWS = "HSANWS";
        public const string PeopleSoftFileBenefitPlanHealthHSAWF = "HSAWF";
        public const string PeopleSoftFileBenefitPlanHealthHSANWF = "HSANWF";
        public const string CoverageSingle = "Single";
        public const string CoverageFamily = "Family";
        public const string CoverageCodeSingleActive = "0001";
        public const string CoverageCodeFamilyActive = "0002";
        //PIR 20178
        public const string PeopleSoftBenefitPlanLifeBasicTemp = "TEMP20";
        public const string PeopleSoftBenefitPlanLifeSuppTemp = "TEMP21";
        public const string PeopleSoftBenefitPlanLifeSpouseSuppTemp = "TEMP2A";

        public const string PeopleSoftURL = "PURL"; //PIR-11864
        public const string PeopleSoftHIEDURL = "HERL"; //PIR-24075

        //Start PIR 8518
        #region PIR 8518
        public const string Retiree_18_Month_COBRA = "R18C";
        public const string Retiree_36_Month_COBRA = "R36C";
        public const string Retiree_Disability_COBRA = "RDSC";
        #endregion
        //End PIR 8518

        #region MssVideosName
        public const string MSSLifePlanVideo = "PIGL"; //PIR 15061
        public const string MSSEAPVideo = "PEAP";
        public const string MSSDentalVideo = "PIDL";
        public const string MSSDefferedCompVideo = "PIDC";
        public const string MSSFlexCompVideo = "PIFC";
        public const string MSSLongTermCareVideo = "PLTC";
        public const string MSSVisionPlanVideo = "PIVP";
        public const string MSSHealthPlanVideo = "PIGH";
        public const string MSSDefinedBenefitVideo = "PIDB";
        public const string MSSDefinedContributionVideo = "PDCP";
        #endregion

        //PIR 11940
        public const string BCBSFileCodeDivorce = "DIVR";
        public const string BCBSFileCodeDeath = "DETH";
        public const string BCBSFileCodeEligibleRetirement = "ELRE";
        public const string BCBSFileCodeEndofCOBRAPeriod = "ECOP";
        public const string BCBSFileCodeMemberCancellation = "CNLD";
        public const string BCBSFileCodeRemoveDependent = "RMVD";
        public const string BCBSFileCodeLeaveofAbsence = "LOAE";
        public const string BCBSFileCodeUnpaidLeaveofAbsence = "UPLA";
        public const string BCBSFileCodeNonPayment = "NONP";
        public const string BCBSEmploymentTermination = "ETRM";
        public const int NDPERSProviderOrgPlanID = 2114;
        //ESS Def Comp Regular Header No Detail Text
        public const string EmployerOfferingOnly457text = "<b>By selecting ‘No’ the payroll report will not be created. A new payroll <br/>report will need to be created once employees have been updated.</b>";

        //PIR 13773 - Do not display ‘Create Payroll Report’ for employers with central_payroll_flag = ‘Y’
        public const string ESSCreatePayrollReport = "CRPR";
       
        //PIR 14264
        public const string RoutingNoAndAccNo = "RACC";

        //PIR 14646
        public const string MainBenefit1997Tier = "97MT"; 
        public const string MainBenefit2016Tier = "16MT";
		
        //PIR 26282 
        public const string BCIBenefit2011Tier = "11BT";
        public const string BCIBenefit2023Tier = "23BT";

		//PIR 17552
        public const string Main85 = "MA85";
        public const string Main202085 = "MN85";

        //PIR 14848
        public const string ChangeReasonMedicareEligibility = "MELG";
        public const string ChangeReasonNewRetiree = "NEWR";
        public const string ChangeReasonMemberCancellation = "CNLD";
        public const string ChangeReasonNonPayment = "NONP";
        public const string ChangeReasonDeath = "DETH";
        public const string ChangeReasonCMSLossOfEligibility = "LPDP";
        public const string ChangeReasonIBSPaymentMethod = "IBSM";
        public const string ChangeReasonLowIncomeSubsidy = "LISH";
        public const string ChangeReasonLateEnrollmentPenalty = "LENP";
        public const string ChangeReasonDataCorrection = "DATC";
        public const string ChangeReasonInitialEnrollDateChange = "IEDC";
        public const string ChangeReasonDateCorrection = "DTCR";
        public const string ChangeReasonStart_Of_COBRA_Period = "SCOP";

        public const string ChangeReasonDemographic = "DEMO";
        public const string ChangeReasonAddress = "ADDR";

        public const string LowIncomeCredit = "LIC";
		public const string RetirementPlanParticipationStatusTranToDb = "TRDB"; //PIR 14656

        public const string ESIPartDEnrollment = "ESI";// PIR 14915

        //PIR 11946
        public const string RetirementContributionTransferFlagC = "C";

        public const string PlanHealthName = "Health";
        public const string PlanMedicareName = "Medicare Part D";
        public const string PlanLifeName = "Life";
        public const string PlanDentalName = "Dental";
        public const string PlanVisionName = "Vision";
        public const string LifeCoverageBasic = "Basic";
        public const string LifeCoverageSupplemental = "Supplemental";
        public const string LifeCoverageDepSupplemental = "Dependent Supplemental";
        public const string LifeCoverageSpouseSupplemental = "Spouse Supplemental";

        public const int HealthInsuranceDeductionPaymentItemType = 70; //PIR 8912
		//PIR 17504 - GL Changes (938 residual changes logged under this PIR)
        public const string GLStatusTransitionValueWriteOff = "WRTF";
        public const string GLStatusTransitionValueCancelPendingToCancelPriorPayment = "PCPY";
        public const string GLMonthlyPayPenRecItemTypeCodeValue = "P065";
        public const string GLOneTimePayPenRecItemTypeCodeValue = "P025";
        public const string GLStatusTransitionValueCanceled = "CNCL";
        public const string GLStatusTransitionValueRecPendingApprToApproved = "PNAP";
        public const string GLDuesItemTypeCodeValue = "P074";
        public const string GLDuesReceivableItemTypeCodeValue = "P086";
        public const string GLStateTaxAmountItemTypeCodeValue = "P028";
        public const string GLPLSOFederalTaxAmountItemTypeCodeValue = "P035";
        public const string GLPLSOStateTaxAmountItemTypeCodeValue = "P036";
        public const string GLStateTaxOnInterestAmountItemTypeCodeValue = "P415";
        public const string GLFedTaxOnInterestAmountItemTypeCodeValue = "P424";
        public const string GLThirdPartyHealthInsuranceItemTypeCodeValue = "P071";
        public const string GLInsuPremHealthItemTypeCodeValue = "P076";
        public const string GLInsuPremLifeItemTypeCodeValue = "P077";
        public const string GLInsuPremDentalItemTypeCodeValue = "P078";
        public const string GLInsuPremVisItemTypeCodeValue = "P079";
        public const string GLInsuPremLTCItemTypeCodeValue = "P080";
        public const string GLThirdPartyHealthInsRecItemTypeCodeValue = "P083";
        public const string GLInsuPremHealthRecItemTypeCodeValue = "P088";
        public const string GLInsuPremLifeRecItemTypeCodeValue = "P089";
        public const string GLInsuPremDentRecItemTypeCodeValue = "P090";
        public const string GLInsuPremVisRecItemTypeCodeValue = "P091";
        public const string GLInsuPremLTCRecItemTypeCodeValue = "P092";
        public const string GLInsuPremMedPartDItemTypeCodeValue = "P120";
        public const string GLInsuPremMedPartDRecItemTypeCodeValue = "P121";
        public const int NDPERSOrgId = 44;
        public const int CanceledPremiumReceivableAccount = 244;
		public const int CodeIdForDependentAmount = 1027;
        public const string GLStatusTransitionValueApproveToCancel = "APCL"; // PIR 20688

        //PIR 938
        public const string GLRegOneTimeFedTaxAmtItemTypeCodeValue = "P012";
        public const string GLRegOneTimeStateTaxAmtItemTypeCodeValue = "P013";
        public const string GLSpecialOneTimeFedTaxAmtItemTypeCodeValue = "P027";
        public const string GLRegMonthlyFedTaxAmtItemTypeCodeValue = "P046";
        public const string GLRegMonthlyStateTaxAmtItemTypeCodeValue = "P047";
        public const string GLRegularMonthlyFederalTaxonInterestAmount = "P052";
        public const string GLRegularMonthlyStateTaxonInterestAmount = "P053";
        public const string GLRegularMonthlyFederalTaxonInterestAmount58 = "P058";
        public const string GLRegularMonthlyStateTaxonInterestAmount59 = "P059";
        public const string GLRegularMonthlyFederalTaxRefund = "P107";
        public const string GLRegularMonthlyStateTaxRefund = "P108";
        public const string GLRegularMonthlyFederalTaxAmount114 = "P114";
        public const string GLRegularMonthlyStateTaxAmount115 = "P115";
        public const string GLRegularMonthlyFederalTaxAmount116 = "P116";
        public const string GLRegularMonthlyStateTaxAmount117 = "P117";
        public const string GLRegularMonthlyFederalTaxAmount118 = "P118";
        public const string GLRegularMonthlyStateTaxAmount119 = "P119";
        public const string GLRegularOneTimeFederalTaxonInterestAmount = "P404";
        public const string GLRegularOneTimeStateTaxonInterestAmount = "P405";
        public const string GLRegularOneTimeFederalTaxAmount = "P408";
        public const string GLRegularOneTimeStateTaxAmount = "P409";
        public const string GLRegularOneTimeFederalTaxonInterestAmount414 = "P414";
        public const string GLRegularOneTimeFederalTaxAmount418 = "P418";
        public const string GLRegularOneTimeStateTaxAmount419 = "P419";
        public const string GLRegularOneTimeStateTaxonInterestAmount425 = "P425";
        public const string GLRegularOneTimeFederalTaxAmount428 = "P428";
        public const string GLRegularOneTimeStateTaxAmount429 = "P429";
        public const int TaxItemDebitAccountId = 11;
        public const int OrganizationTFFROrgId = 2428;
		public const string RolloverTypeValueForRothIRA = "RIRA";
        public const int ServicePurchaseContractInterestCodeId = 7007;  //PIR-17512 Interest Calculation

        //PIR 7202
        public const int RetirementInboundFileId = 10;
        public const int DefCompFileId = 11;
        public const int PurchaseFileId = 28;
		public const string CodeValueForRolloverCancelled = "CANC";
		
		//WSS message request PIR-17066
        public const string MemberTypeActive = "ACTV";
        public const string MemberTypeRetire = "RETR";
        public const string MemberTypeEmployer = "EMPL";

        //PIR 17081
        public const string LumpSumPayout = "LSPO";
        public const string ChangeReasonLifeAgeUpdate = "LAUP";
        public const string MainDPI = "DPI1";
        public const string MainCareerTech = "CAR1";

        //PIR-18492 , Email Notification Batch
        public const int EmailNotificationStepNumber = 1008;
        public const int LoginErrorMessage = 10344;
        public const int PopUpMessageForCertify=10345;
        public const int IsEmailWaiverFlagSelected = 10346;

        #region Resource
        public const int PersonBenefitApplicationRestrictAccountResource = 1901; //PIR 20807
        #endregion

        //18493
        public const string BenefitApplicationActionStatusSaveAndContinueLater = "SACL";
        public const string DefferalDateChoiceOptionOther = "OTHR";
        public const string RefundDistributionRefundToMeDirectly = "RTMD";
        public const string RefundDistributionRolloverPartOrAllOfMyRefund = "RPAR";
        public const string RefundDistributionRolloverPartOfMyRefund = "RPMR";
        public const string DepositAchType = "D";
        public const string WithdrawlAchType = "W";
        public const string InsuranceTypeOfCoveragePPOBasic = "PPOB";
        public const string BenefitApplicationActionStatusCompleted = "CMPT";
        public const string DefferalDateChoiceOptionNormalRetirementDate = "NRRD";
        public const string HealthPlanAcknowledgement = "HP";
        public const string DentalPlanAcknowledgement = "DP";
        public const string VisionPlanAcknowledgement = "VP";
        public const string MedicarePlanAcknowledgement = "MP";
        public const string LifePlanAcknowledgement = "LP";
        public const string FlexPlanAcknowledgement = "FP";
        public const string IsWSSAppWizardVisible = "APWI";
        public const string DateOfLastReportReceived = "__/__/____"; // PIR 26725
        //18493 - Workflow Processes
        public const int Map_MSS_Process_DB_Retirement_Application = 369;
        public const int Map_MSS_Process_Job_Service_Application = 362;
        public const int Map_MSS_Process_DC_Retirement_Application = 363;
        public const int Map_MSS_Process_Disability_Application = 364;
        public const int Map_MSS_Process_Deferred_Retirement_Application = 365;
        public const int Map_MSS_Initialize_Process_Refund_Application_And_Calculation = 366;
        public const int Map_Enroll_Retiree_Insurance_Plans = 367;
        public const int Map_Enroll_COBRA_Insurance_Plans = 368;
        public const int Map_MSS_Enroll_COBRA_Insurance_Plans = 382;
        public const string ImageDocCategoryMember = "Member";
        public const string ImageDocCategoryOrganization = "Organization";
        public const string FileNetDocumentTypeForm = "FORM";
        public const string FileNetDocumentTypeCorrespondence = "CORRESPONDENCE";
        //public const string InsEnrlTypeCobra = "C";
        //public const string InsEnrlTypeRetiree = "R";

        public const int DocumetCodeMemberBirthCertificate = 1032;
        public const int DocumetCodeBeneficiaryBirthCertificate = 1247;
        public const int DocumetCodeMarriageCertificate = 1043;
        public const int DocumetCodeMedicareIDCardCertificate = 1025;
        public const int DocumetCodeOccupationalDemandCertificate = 54398;
        public const int DocumetCodeStatementOfDisabilityCertificate = 54399;
        public const string DocumentIdMSSOtherDocuments = "682";

        //PIR 20229
        public const string FAS2010 = "1";
        public const string FAS2019 = "2";
        public const string FAS2020 = "3";

        //PIR 20986
        public const string ScreenStepValueHSA = "HSA";

        //PIR 21179 
        #region Workflow process - FirstActivityID
        public const int ACH_Pull_For_Retirement_FirstActivityID = 191; // Process_ID = 298
        public const int ACH_Pull_For_DeferredCompensation_FirstActivityID = 193; // Process_ID = 299
        public const int ACH_Pull_For_IBS_Insurance_FirstActivityID = 195; // Process_ID = 300
        public const int ACH_Pull_For_Insurance_FirstActivityID = 197; // Process_ID = 301
        #endregion
		//PIR 20868 
        public const string AddrChangeLetterNotSent = "1";
        public const string AddrChangeLetterSent = "2";

        public const string SystemRegionValueProd = "PROD";
        public const string UpdateAccountUrlCodeValue = "UUAU";
        public static DateTime RetirementDateForPreRTWPayeeAccountRHICReduction = new DateTime(2010, 10, 01);
        public static DateTime idteHPIndexEligibleDate = new DateTime(2020, 01, 01);
        public const string SupplementalCoverageAmountLimit = "SCAL";//PIR 23527
		
		//PIR 23167, 23340, 23408
        public const int OverlapResourceLife = 237;
        public const int OverlapResourceGHDV = 238;
        public const int OverlapResourceEAP = 239;
        public const int OverlapResourceFlex = 2056;

        public const int OverlapResourceEnhancedLife = 2052;
        public const int OverlapResourceEnhancedGHDV = 2053;
        public const int OverlapResourceEnhancedEAP = 2054;
        public const int OverlapResourceEnhancedFlex = 2055;

        // PIR 26102 added new resource
        public const int OverlapResourceEnhancedRetirement = 2089;
        public const int OverlapResourceEnhancedDepCompOtherPlan = 2090;


        public const string ContactMaintenanceScreenDifferentiator = "CONM";
        public const string OrgContactMaintenanceScreenDifferentiator = "OCOM";
        public const string EligibleWagesLimit = "EWLT"; // PIR 24243
        public const string BenefitEnrollmentReportTitle = "Benefit Enrollment/Termination Report"; // PIR 23729
        public const string AnnualEnrollmentSummaryReportTitle = "Annual Enrollment Summary Report"; // PIR 23729

        public const string BoardMemberStaticTextActive = "BMST";
        public const string BallotStaticTextActive = "BTST";
        public const string BoardMemberElectionTextActive = "BMET";
		
		//PIR 24035
        public const int MapMaintainLineOfDutySurvivor = 375;
        public const string HealthLifeUnderwritingGain = "HLUG";

        public const string BoardMemberStaticTextRetiree = "BMTR";
        public const string BallotStaticTextRetiree = "BTTR";
        public const string BoardMemberElectionTextRetiree = "BETR";

        //PIR New Hire Automation
        public const int RetirementCategory = 1;
        public const int EAPCategory = 2;
        public const int LifeCategory = 3;

        //ACH Pull Automation
        public const string DepositTapeBankAccountRetirement = "RETR";
        public const string DepositTapeBankAccountDeferredComp = "DCOM";
        public const string DepositTapeBankAccountInsurance = "INSR";
        public const string StringLetterS = "S";

        //PIR 25084 - W4P Tax Withholding
        public const int Twelve = 12;
        public const string FilingStatusSingleOrMrrdFlgSeparately = "SGMF";
        public const string FilingStatusMrdFilingJointlyOrQualWidower = "MFJQ";
        public const string FilingStatusHeadOfHousehold = "HOHH";
        public const decimal ZeroDecimal = 0.00M;
        public const int ZeroInt = 0;
        public const decimal HundredDecimal = 100.00M;
        public static string GetTaxRateBasedOnEffecDate = "entFedStateTaxRate.GetTaxRateForEffecDate";
        public const string TaxApplicable = "TAXA";

        //BPM Death Automation
        public const string NonEmployeeDeathLetterCodeValue = "NEDL";
        public const string EmployeeDeathLetterCodeValue = "EMDL";
        public const string PayeeDeathLetterCodeValue = "PADL";
        //PIR 25729
        public const string BenefitProvisionTypeRTDT = "RTDT";

        //PIR 26099
        public const string CalculateServicePurchaseCostEstimate = "ESTI";

        //PIR 25782
        public const int Map_Process_Marital_Status_Change = 376;

        //PIR 26260
        public const int Map_Process_Person_Contact_Expiring = 381;
		
		//PIR 26886
        public const int TemplateID_PER0055 = 537;

    }
}
