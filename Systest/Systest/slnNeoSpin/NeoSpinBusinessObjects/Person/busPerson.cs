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
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using NeoSpin.DataObjects;
using Sagitec.ExceptionPub;
using System.Net.Mail;
using Sagitec.Bpm;
#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busPerson : busPersonBase
    {
        //PIR 10697
        public busWssPersonAccountEnrollmentRequest ibusLatestPesonAccountEnrollmentRequest { get; set; }
        public busWssEmploymentAcaCert ibusWssEmploymentAcaCert { get; set; }

        public Collection<busWssEmploymentAcaCert> iclbWssEmploymentAcaCert { get; set; }

        public int iintEnrollmentRequest { get; set; }

        public bool iblnIsFromPs { get; set; } //PIR-11030 15 September

        public bool iblnIsFromMSS { get; set; }//PIR 25093

        public string istrEffectiveDate { get; set; } //PIR 18503

        // This property is used to Validate New in Employement History Tab
        private string _iblnIsValidateNewTrue;
        public string iblnIsValidateNewTrue
        {
            get { return _iblnIsValidateNewTrue; }
            set { _iblnIsValidateNewTrue = value; }
        }

        private Collection<busPerson> _icolPerson;
        public Collection<busPerson> icolPerson
        {
            get
            {
                return _icolPerson;
            }

            set
            {
                _icolPerson = value;
            }
        }

        private DateTime _idtCoverageBeginDate;
        public DateTime idtCoverageBeginDate
        {
            get { return _idtCoverageBeginDate; }
            set { _idtCoverageBeginDate = value; }
        }

        private DateTime _idtCoverageEndDate;
        public DateTime idtCoverageEndDate
        {
            get { return _idtCoverageEndDate; }
            set { _idtCoverageEndDate = value; }
        }

        private Collection<busPersonAccountEmploymentDetail> _icolEnrolledPersonAccountEmploymentDetail;
        public Collection<busPersonAccountEmploymentDetail> icolEnrolledPersonAccountEmploymentDetail
        {
            get { return _icolEnrolledPersonAccountEmploymentDetail; }
            set { _icolEnrolledPersonAccountEmploymentDetail = value; }
        }

        private Collection<busPersonAccount> _icolPersonAccount;
        public Collection<busPersonAccount> icolPersonAccount
        {
            get { return _icolPersonAccount; }
            set { _icolPersonAccount = value; }
        }

        public Collection<busPersonAccount> icolPersonAccountByPlan { get; set; }

        private Collection<busPayeeAccount> _iclbPayeeAccount;

        public Collection<busPayeeAccount> iclbPayeeAccount
        {
            get { return _iclbPayeeAccount; }
            set { _iclbPayeeAccount = value; }
        }

        public busPaymentElectionAdjustment ibusPaymentElectionAdjustment { get; set; }
        private Collection<busPersonAccount> _icolPersonAccountByBenefitType;
        public Collection<busPersonAccount> icolPersonAccountByBenefitType
        {
            get { return _icolPersonAccountByBenefitType; }
            set { _icolPersonAccountByBenefitType = value; }
        }
        #region DC/457 Enrollment
        public string istrMaritalStatusForFile { get; set; }
        public string istrGenderForFile { get; set; }
        public string istrFullNameForFile
        {
            get
            {
                if ((icdoPerson.last_name + "," + icdoPerson.first_name + " " + icdoPerson.middle_name).Length > 30)
                {
                    return (icdoPerson.last_name + "," + icdoPerson.first_name + " " + icdoPerson.middle_name).ToUpper().Substring(0, 30);
                }
                else
                {
                    return (icdoPerson.last_name + "," + icdoPerson.first_name + " " + icdoPerson.middle_name).ToUpper();
                }
            }
        }

        public String istrFullName
        {
            get
            {
                StringBuilder lstrbFullName = new StringBuilder();
                if (!String.IsNullOrEmpty(icdoPerson.first_name))
                {
                    lstrbFullName.Append(icdoPerson.first_name);
                }
                if (!String.IsNullOrEmpty(icdoPerson.middle_name))
                {
                    lstrbFullName.Append(" ");
                    lstrbFullName.Append(icdoPerson.middle_name);
                }
                if (!String.IsNullOrEmpty(icdoPerson.last_name))
                {
                    lstrbFullName.Append(" ");
                    lstrbFullName.Append(icdoPerson.last_name);
                }
                return lstrbFullName.ToString();
            }
            set { }
        }

        public string istrPersonName
        {
            get
            {
                return (icdoPerson.first_name != null ? icdoPerson.first_name.Trim() + ", " : "") + ((icdoPerson.last_name != null && icdoPerson.last_name != string.Empty) ? icdoPerson.last_name.Trim() : "");
            }
            set { }
        }
        public string istrEmployeeID
        {
            get
            {
                return icdoPerson.person_id.ToString();
            }
        }
        #endregion

        #region HSA Enrollment
        public string istrCountryCode { 
            get 
            {
                if (ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_value.IsNotNullOrEmpty() && ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_value != "0001")
                {
                    return busGlobalFunctions.GetData1ByCodeValue(ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_id, ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_value, iobjPassInfo);
                }
                else
                    return string.Empty;
            } 
        }

        public string istrLastName
        {
          get
          {
              return icdoPerson.last_name.IsNotNullOrEmpty() ? icdoPerson.last_name.ToUpper().ReplaceWith("[^a-zA-Z0-9]", "") : string.Empty;
          }
        }

        public string istrFirstName
        {
            get
            {
                return icdoPerson.first_name.IsNotNullOrEmpty() ? icdoPerson.first_name.ToUpper().ReplaceWith("[^a-zA-Z0-9]", "") : string.Empty;
            }

        }
        public string istrMiddleInitial {
            get
            {
                if (icdoPerson.middle_name.IsNotNullOrEmpty())
                    return icdoPerson.middle_name.ElementAt(0).ToString().ToUpper().ReplaceWith("[^a-zA-Z]", "");
                else
                    return string.Empty;
            }
        }
      
        public string istrAddressLine1 {
            get
            {
                return ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_1.IsNotNullOrEmpty() ? ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_1.ToUpper().ReplaceWith("[^ a-zA-Z0-9]", "") : string.Empty;
            }
        }
        public string istrAddressLine2
        {
            get
            {
                return ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_2.IsNotNullOrEmpty() ? ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_line_2.ToUpper().ReplaceWith("[^ a-zA-Z0-9]", "") : string.Empty;
            }
        }
		
        public string istrPlanName { get; set; }
        public string istrYouMustEnrollOrWaive { get; set; }

        public string istrIfYouDoNoAction { get; set; }

        public Collection<busPerson> iclbWelComeBenefitPlans { get; set; }

        public void LoadEmployerWelcomeBenefitPlans(int aIntPersonEmploymentDtlId)
        {

            DataTable ldtEmployerWelcomeBenefitPlan = Select("entPerson.LoadPlanDetailsForWelcomeLetter", new object[1] { aIntPersonEmploymentDtlId });
            iclbWelComeBenefitPlans = new Collection<busPerson>();
            foreach (DataRow aBenefitPlanRow in ldtEmployerWelcomeBenefitPlan.Rows)
            {
                busPerson lobjPerson = new busPerson();
                sqlFunction.LoadQueryResult(lobjPerson, aBenefitPlanRow);
                iclbWelComeBenefitPlans.Add(lobjPerson);
            }
        }

        public string istrZipCode
        {
            get
            {
                if (ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_code.IsNotNullOrEmpty())
                {
                    if (ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_4_code.IsNotNullOrEmpty())
                        return ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_code.ReplaceWith("[^0-9]", "") + "-" + ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_4_code.ReplaceWith("[^0-9]", "");
                    else
                        return ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_code.ReplaceWith("[^0-9]", "");
                }
                else
                    return string.Empty;
            }
        }


        #endregion

      
        #region Member Summary Properties
        private decimal _idecMemberSummaryCurrentPremiumAmt;

        public decimal idecMemberSummaryCurrentPremiumAmt
        {
            get { return _idecMemberSummaryCurrentPremiumAmt; }
            set { _idecMemberSummaryCurrentPremiumAmt = value; }
        }

        private decimal _idecMemberSummaryNetPremiumAmt;

        public decimal idecMemberSummaryNetPremiumAmt
        {
            get { return _idecMemberSummaryNetPremiumAmt; }
            set { _idecMemberSummaryNetPremiumAmt = value; }
        }
        private decimal _idecMemberSummaryRhicAmt;

        public decimal idecMemberSummaryRhicAmt
        {
            get { return _idecMemberSummaryRhicAmt; }
            set { _idecMemberSummaryRhicAmt = value; }
        }
        private decimal _idecMemberSummaryJSRhicAmount;

        public decimal idecMemberSummaryJSRhicAmount
        {
            get { return _idecMemberSummaryJSRhicAmount; }
            set { _idecMemberSummaryJSRhicAmount = value; }
        }
        private decimal _idecMemberSummaryBalanceForward;

        public decimal idecMemberSummaryBalanceForward
        {
            get { return _idecMemberSummaryBalanceForward; }
            set { _idecMemberSummaryBalanceForward = value; }
        }
        private decimal _idecMemberSummaryTotalDueAmt;

        public decimal idecMemberSummaryTotalDueAmt
        {
            get { return _idecMemberSummaryTotalDueAmt; }
            set { _idecMemberSummaryTotalDueAmt = value; }
        }

        public decimal idecMemberPaymentsDone { get; set; }

        public decimal idecAdjustmentsPaid { get; set; }

        public decimal idecPendingAdjustments { get; set; }

        #endregion

        #region SFN16789 IBS Properties
        private string _DeductionMethodIdentifier;
        public string DeductionMethodIdentifier
        {
            get { return _DeductionMethodIdentifier; }
            set { _DeductionMethodIdentifier = value; }
        }
        private string _PastDueIdentifier;
        public string PastDueIdentifier
        {
            get { return _PastDueIdentifier; }
            set { _PastDueIdentifier = value; }
        }
        private string _HealthCreditIdentifier;
        public string HealthCreditIdentifier
        {
            get { return _HealthCreditIdentifier; }
            set { _HealthCreditIdentifier = value; }
        }

        private string _IBSReportType;
        public string IBSReportType
        {
            get { return _IBSReportType; }
            set { _IBSReportType = value; }
        }

        private string _BillDate;

        public string BillDate
        {
            get { return _BillDate; }
            set { _BillDate = value; }
        }

        private int _OrgID;

        public int OrgID
        {
            get { return _OrgID; }
            set { _OrgID = value; }
        }
        private string _DueDate;

        public string DueDate
        {
            get { return _DueDate; }
            set { _DueDate = value; }
        }

        private decimal _idecTotalDue;

        public decimal idecTotalDue
        {
            get { return _idecTotalDue; }
            set { _idecTotalDue = value; }
        }
        private decimal _idecHealthCredit;

        public decimal idecHealthCredit
        {
            get { return _idecHealthCredit; }
            set { _idecHealthCredit = value; }
        }
        private decimal _idecTotalHealthPremiumAmt;

        public decimal idecTotalMedicarePremium { get; set; }

        public decimal idecTotalHealthPremiumAmt
        {
            get { return _idecTotalHealthPremiumAmt; }
            set { _idecTotalHealthPremiumAmt = value; }
        }

        private decimal _idecGropHealthPremiumAmt;

        public decimal idecGropHealthPremiumAmt
        {
            get { return _idecGropHealthPremiumAmt; }
            set { _idecGropHealthPremiumAmt = value; }
        }
        private decimal _idecGroupDentalPremiumAmt;

        public decimal idecGroupDentalPremiumAmt
        {
            get { return _idecGroupDentalPremiumAmt; }
            set { _idecGroupDentalPremiumAmt = value; }
        }
        private decimal _idecGroupVisionPremiumAmt;

        public decimal idecGroupVisionPremiumAmt
        {
            get { return _idecGroupVisionPremiumAmt; }
            set { _idecGroupVisionPremiumAmt = value; }
        }
        private decimal _idecLifeBasicPremiumAmt;

        public decimal idecLifeBasicPremiumAmt
        {
            get { return _idecLifeBasicPremiumAmt; }
            set { _idecLifeBasicPremiumAmt = value; }
        }
        private decimal _idecLifeSupplementalPremiumAmount;

        public decimal idecLifeSupplementalPremiumAmount
        {
            get { return _idecLifeSupplementalPremiumAmount; }
            set { _idecLifeSupplementalPremiumAmount = value; }
        }
        private decimal _idecDependentSupplementalPremiumAmt;

        public decimal idecDependentSupplementalPremiumAmt
        {
            get { return _idecDependentSupplementalPremiumAmt; }
            set { _idecDependentSupplementalPremiumAmt = value; }
        }
        private decimal _idecSpouseSupplementalPremiumAmt;

        public decimal idecSpouseSupplementalPremiumAmt
        {
            get { return _idecSpouseSupplementalPremiumAmt; }
            set { _idecSpouseSupplementalPremiumAmt = value; }
        }
        private decimal _idecTotalLifePremiumAmount;

        public decimal idecTotalLifePremiumAmount
        {
            get { return _idecTotalLifePremiumAmount; }
            set { _idecTotalLifePremiumAmount = value; }
        }

        private string _istrGroupHealthCoverageDescription;

        public string istrGroupHealthCoverageDescription
        {
            get { return _istrGroupHealthCoverageDescription; }
            set { _istrGroupHealthCoverageDescription = value; }
        }
        private string _istrGroupDentalDescription;

        public string istrGroupDentalDescription
        {
            get { return _istrGroupDentalDescription; }
            set { _istrGroupDentalDescription = value; }
        }
        private string _istrGroupVisionDescription;

        public string istrGroupVisionDescription
        {
            get { return _istrGroupVisionDescription; }
            set { _istrGroupVisionDescription = value; }
        }
        private decimal _idecBalanceForward;

        public decimal idecBalanceForward
        {
            get { return _idecBalanceForward; }
            set { _idecBalanceForward = value; }
        }

        public string istrAdjustmentIdentifier { get; set; }

        public decimal idecAdjustmentAmount { get; set; }

        public decimal idecLTCPremium { get; set; }

        #endregion

        #region PensionAccountSummary

        private DateTime _idtMemberSummaryEffectiveDate;

        public DateTime idtMemberSummaryEffectiveDate
        {
            get { return _idtMemberSummaryEffectiveDate; }
            set { _idtMemberSummaryEffectiveDate = value; }
        }

        private Collection<busPersonAccount> _iclbActivePensionAccounts;
        public Collection<busPersonAccount> iclbActivePensionAccounts
        {
            get { return _iclbActivePensionAccounts; }
            set { _iclbActivePensionAccounts = value; }
        }

        private Collection<busPersonAccountRetirement> _iclbPensionAccounts;
        public Collection<busPersonAccountRetirement> iclbPensionAccounts
        {
            get { return _iclbPensionAccounts; }
            set { _iclbPensionAccounts = value; }
        }

        private Collection<busPersonAccountRetirement> _iclbClosedPensionAccounts;
        public Collection<busPersonAccountRetirement> iclbClosedPensionAccounts
        {
            get { return _iclbClosedPensionAccounts; }
            set { _iclbClosedPensionAccounts = value; }
        }

        /// This Collection is used for Person Overview Maintenance.
        private Collection<busPersonAccountDeferredComp> _iclbDeferredCompAccounts;
        public Collection<busPersonAccountDeferredComp> iclbDeferredCompAccounts
        {
            get { return _iclbDeferredCompAccounts; }
            set { _iclbDeferredCompAccounts = value; }
        }

        /// This Collection is used for Person Overview Maintenance.
        private Collection<busPersonAccount> _iclbClosedInsuranceAccounts;
        public Collection<busPersonAccount> iclbClosedInsuranceAccounts
        {
            get { return _iclbClosedInsuranceAccounts; }
            set { _iclbClosedInsuranceAccounts = value; }
        }

        /// This Collection is used for Person Overview Maintenance.
        private Collection<busPersonAccount> _iclbActiveInsuranceAccounts;
        public Collection<busPersonAccount> iclbActiveInsuranceAccounts
        {
            get { return _iclbActiveInsuranceAccounts; }
            set { _iclbActiveInsuranceAccounts = value; }
        }

        #endregion

        private busPersonAddress _ibusPersonCurrentAddress;
        public busPersonAddress ibusPersonCurrentAddress
        {
            get
            {
                return _ibusPersonCurrentAddress;
            }

            set
            {
                _ibusPersonCurrentAddress = value;
            }
        }

        //PIR 14848
        private busPersonAccountMedicarePartDHistory _ibusPersonAccountMedicare;
        public busPersonAccountMedicarePartDHistory ibusPersonAccountMedicare
        {
            get
            {
                return _ibusPersonAccountMedicare;
            }

            set
            {
                _ibusPersonAccountMedicare = value;
            }
        }

        //PIR - 346
        private Collection<busNotes> _iclbNotes;
        public Collection<busNotes> iclbNotes
        {
            get
            {
                return _iclbNotes;
            }

            set
            {
                _iclbNotes = value;
            }
        }

        private Collection<busPerson> _iclbPaymentRecieved;
        public Collection<busPerson> iclbPaymentRecieved
        {
            get { return _iclbPaymentRecieved; }
            set { _iclbPaymentRecieved = value; }
        }

        private Collection<busSeminarAttendeeDetail> _icolSeminarAttendeeDetail;
        public Collection<busSeminarAttendeeDetail> icolSeminarAttendeeDetail
        {
            get { return _icolSeminarAttendeeDetail; }
            set { _icolSeminarAttendeeDetail = value; }
        }

        private Collection<busContactTicket> _icolPersonContactTicket;
        public Collection<busContactTicket> icolPersonContactTicket
        {
            get { return _icolPersonContactTicket; }
            set { _icolPersonContactTicket = value; }
        }

        private Collection<busPersonAddress> _icolPersonAddress;
        public Collection<busPersonAddress> icolPersonAddress
        {
            get { return _icolPersonAddress; }
            set { _icolPersonAddress = value; }
        }

        private Collection<busPersonTffrTiaaService> _iclbTffrTiaaService;
        public Collection<busPersonTffrTiaaService> iclbTffrTiaaService
        {
            get { return _iclbTffrTiaaService; }
            set { _iclbTffrTiaaService = value; }
        }

        private Collection<busBenefitApplication> _iclbBenefitApplication;
        public Collection<busBenefitApplication> iclbBenefitApplication
        {
            get { return _iclbBenefitApplication; }
            set { _iclbBenefitApplication = value; }
        }

        //this collection is used to load death notification of beneficiary dependent contacts if ny
        private Collection<busDeathNotification> _iclbDeathNotificationForBeneficiaries;
        public Collection<busDeathNotification> iclbDeathNotificationForBeneficiaries
        {
            get { return _iclbDeathNotificationForBeneficiaries; }
            set { _iclbDeathNotificationForBeneficiaries = value; }
        }

        private Collection<busDeathNotification> _iclbDeathNotificationForDependents;
        public Collection<busDeathNotification> iclbDeathNotificationForDependents
        {
            get { return _iclbDeathNotificationForDependents; }
            set { _iclbDeathNotificationForDependents = value; }
        }

        private Collection<busDeathNotification> _iclbDeathNotificationForContacts;
        public Collection<busDeathNotification> iclbDeathNotificationForContacts
        {
            get { return _iclbDeathNotificationForContacts; }
            set { _iclbDeathNotificationForContacts = value; }
        }
        //this cdoPersonAddress will be used for PersonInboundfile only 
        //and should not be used for any other purpose. 
        private cdoPersonAddress _icdoFilePersonAddress;
        public cdoPersonAddress icdoFilePersonAddress
        {
            get
            {
                return _icdoFilePersonAddress;
            }

            set
            {
                _icdoFilePersonAddress = value;
            }
        }

        private Collection<busPersonBeneficiary> _iclbPersonBeneficiary;
        public Collection<busPersonBeneficiary> iclbPersonBeneficiary
        {
            get
            {
                return _iclbPersonBeneficiary;
            }

            set
            {
                _iclbPersonBeneficiary = value;
            }
        }
        public Collection<busPersonBeneficiary> iclbPersonBeneficiaryCor { get; set; } //PIR 21162
        private Collection<busPersonBeneficiary> _iclbPersonBeneficiaryForDeceased;
        public Collection<busPersonBeneficiary> iclbPersonBeneficiaryForDeceased
        {
            get
            {
                return _iclbPersonBeneficiaryForDeceased;
            }

            set
            {
                _iclbPersonBeneficiaryForDeceased = value;
            }
        }

        private Collection<busPersonContact> _icolPersonContact;
        public Collection<busPersonContact> icolPersonContact
        {
            get
            {
                return _icolPersonContact;
            }

            set
            {
                _icolPersonContact = value;
            }
        }
        private Collection<busPersonDependent> _iclbPersonDependent;
        public Collection<busPersonDependent> iclbPersonDependent
        {
            get
            {
                return _iclbPersonDependent;
            }

            set
            {
                _iclbPersonDependent = value;
            }
        }
        private Collection<busPersonEmployment> _icolPersonEmployment;
        public Collection<busPersonEmployment> icolPersonEmployment
        {
            get
            {
                return _icolPersonEmployment;
            }

            set
            {
                _icolPersonEmployment = value;
            }
        }
        //PIR - 329
        private busUser _ibusUser;
        public busUser ibusUser
        {
            get
            {
                return _ibusUser;
            }

            set
            {
                _ibusUser = value;
            }
        }

        private Collection<busRemittance> _iclbAvailableRemittance;
        public Collection<busRemittance> iclbAvailableRemittance
        {
            get { return _iclbAvailableRemittance; }
            set { _iclbAvailableRemittance = value; }
        }

        //modify entity - venkat - no reference found in entity.
        private Collection<busBpmActivityInstanceHistory> _iclbWorkflowProcessHistory;
        public Collection<busBpmActivityInstanceHistory> iclbWorkflowProcessHistory
        {
            get { return _iclbWorkflowProcessHistory; }
            set { _iclbWorkflowProcessHistory = value; }
        }

        private Collection<busDeathNotification> _iclbDeathNotification;
        public Collection<busDeathNotification> iclbDeathNotification
        {
            get { return _iclbDeathNotification; }
            set { _iclbDeathNotification = value; }
        }


        // this collection is used to Load all th RHIC donor collections
        private Collection<busBenefitCalculationPayee> _iclbBenefitCalculationPayee;
        public Collection<busBenefitCalculationPayee> iclbBenefitCalculationPayee
        {
            get { return _iclbBenefitCalculationPayee; }
            set { _iclbBenefitCalculationPayee = value; }
        }
        //property used 75 correspondence
        public busBenefitAccount ibusBenefitAccount { get; set; }

        //this property is used as plan in death notification
        private string _istrPlan;
        public string istrPlan
        {
            get { return _istrPlan; }
            set { _istrPlan = value; }
        }

        //PIR - 1053
        //rounded tpsc to be used in correspondence PER-0201
        public decimal idecRoundedTPSC
        {
            get
            {
                decimal ldecRoundedTPSC = GetTotalPSC(DateTime.MinValue);
                if (ldecRoundedTPSC != 0.00M)
                    ldecRoundedTPSC = Math.Round(ldecRoundedTPSC);
                return ldecRoundedTPSC;
            }
        }

        //PER-0204        
        public decimal idecRoundedTVSC
        {
            get
            {
                decimal ldecRoundedTvSC = GetTotalVSCForPerson(false, DateTime.Now);
                if (ldecRoundedTvSC != 0.00M)
                    ldecRoundedTvSC = Math.Round(ldecRoundedTvSC);
                return ldecRoundedTvSC;
            }
        }

        private busDeathNotification _ibusDeathNotification;
        public busDeathNotification ibusDeathNotification
        {
            get { return _ibusDeathNotification; }
            set { _ibusDeathNotification = value; }
        }

        private Collection<busPersonAccountBeneficiary> _iclbPersonAccountBeneficiary;
        public Collection<busPersonAccountBeneficiary> iclbPersonAccountBeneficiary
        {
            get { return _iclbPersonAccountBeneficiary; }
            set { _iclbPersonAccountBeneficiary = value; }
        }

        private Collection<busPersonAccountDependent> _iclbPersonAccountDependent;
        public Collection<busPersonAccountDependent> iclbPersonAccountDependent
        {
            get { return _iclbPersonAccountDependent; }
            set { _iclbPersonAccountDependent = value; }
        }

        private Collection<busPersonContact> _iclbPersonContactTo;
        public Collection<busPersonContact> iclbPersonContactTo
        {
            get { return _iclbPersonContactTo; }
            set { _iclbPersonContactTo = value; }
        }

        public Collection<busPersonDependent> iclbPersonDependentByDependent { get; set; }

        public Collection<busPersonAddress> iclbPersonAddress { get; set; }
        private bool _iblnIsUpdateAddressClicked;
        public bool iblnIsUpdateAddressClicked
        {
            get
            {
                return _iblnIsUpdateAddressClicked;
            }
            set
            {
                _iblnIsUpdateAddressClicked = value;
            }
        }
        private string _istrSuppressWarning;

        public string istrSuppressWarning
        {
            get { return _istrSuppressWarning; }
            set { _istrSuppressWarning = value; }
        }
        //PIR 25920 DC 2025 changes
        public int iintESSAddlEEContributionPercent { get; set; }
        public bool iblnESSShowAddlEEContributionPercent { get; set; }
        public void LoadPersonDependentByDependent()
        {
            DataTable ldtbPersonDependent = Select<cdoPersonDependent>(new string[1] { "dependent_perslink_id" },
                                                                       new object[1] { _icdoPerson.person_id }, null, null);
            iclbPersonDependentByDependent = GetCollection<busPersonDependent>(ldtbPersonDependent, "icdoPersonDependent");

        }

        //PIR 10697
        public void LoadPersonDependentByDependentForMSS(int aintDependentID)
        {
            DataTable ldtbPersonDependent = Select<cdoPersonDependent>(new string[1] { "dependent_perslink_id" },
                                                                       new object[1] { aintDependentID }, null, null);
            iclbPersonDependentByDependent = GetCollection<busPersonDependent>(ldtbPersonDependent, "icdoPersonDependent");

        }
        public void LoadDeathNotification()
        {
            if (_ibusDeathNotification == null)
                _ibusDeathNotification = new busDeathNotification();
            _ibusDeathNotification.icdoDeathNotification = new cdoDeathNotification();
            _ibusDeathNotification.FindDeathNotificationByPersonId(icdoPerson.person_id);
        }

        public void LoadIBSMemberSummary(bool ablnLoadRemittance = true)
        {
            decimal ldecTotalPaymentRecieved = 0.00M;
            DateTime ldtBillMonth = Convert.ToDateTime(
                                          (DateTime.Today.AddMonths(1).ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"))));
            if (iclbPaymentRecieved == null)
            {
                iclbPaymentRecieved = new Collection<busPerson>();
            }
            DataTable ldtbList = Select("cdoPerson.MemberSummary",
                        new object[1] { icdoPerson.person_id });
            if (ldtbList.Rows.Count > 0)
            {
                sqlFunction.LoadQueryResult(this, ldtbList.Rows[0]);
            }
            if (ablnLoadRemittance)
            {
                DataTable ldtbAppliedRemittance = Select("cdoPerson.MemberSummaryPaymentRecieved",
                                                                new object[1] { icdoPerson.person_id });
                Collection<busRemittance> icolAppliedRemittance = new Collection<busRemittance>();
                icolAppliedRemittance = GetCollection<busRemittance>(ldtbAppliedRemittance, "icdoRemittance");
                _iclbAvailableRemittance = new Collection<busRemittance>();
                foreach (busRemittance lobjremittance in icolAppliedRemittance)
                {
                    if (busEmployerReportHelper.GetRemittanceAvailableAmount(lobjremittance.icdoRemittance.remittance_id) > 0)
                    {
                        lobjremittance.ldclBalanceAmount =
                            busEmployerReportHelper.GetRemittanceAvailableAmount(lobjremittance.icdoRemittance.remittance_id);
                        //Load the Plan Name
                        lobjremittance.LoadPlanName();
                        _iclbAvailableRemittance.Add(lobjremittance);
                    }
                    ldecTotalPaymentRecieved += lobjremittance.ldclBalanceAmount;
                }
            }
            DataTable ldtbIBSBalanceForward = Select("cdoPerson.MemberSummaryBalanceForward",
                                                         new object[1] { icdoPerson.person_id });
            if ((ldtbIBSBalanceForward.Rows.Count > 0) && (ldtbIBSBalanceForward.Rows[0]["idecMemberSummaryBalanceForward"] != DBNull.Value))
            {
                idecMemberSummaryBalanceForward = Convert.ToDecimal(ldtbIBSBalanceForward.Rows[0]["idecMemberSummaryBalanceForward"]);
            }
            DataTable ldtbPaymentsDone = Select("cdoRemittance.GetPayments",
                                                         new object[1] { icdoPerson.person_id });
            if ((ldtbPaymentsDone.Rows.Count > 0) && (ldtbPaymentsDone.Rows[0]["payments_made"] != DBNull.Value))
                idecMemberPaymentsDone = Convert.ToDecimal(ldtbPaymentsDone.Rows[0]["payments_made"]);

            DataTable ldtAdjustments = Select("cdoIbsDetail.GetAdjustmentPayments",
                                                        new object[1] { icdoPerson.person_id });
            if ((ldtAdjustments.Rows.Count > 0) && (ldtAdjustments.Rows[0]["idecTotalAdjustments"] != DBNull.Value))
                idecAdjustmentsPaid = Convert.ToDecimal(ldtAdjustments.Rows[0]["idecTotalAdjustments"]);
            //prod pir 5884
            DataTable ldtPaymentElectionAdjustments = Select("cdoPaymentElectionAdjustment.GetAdjustmentPendingForPerson",
                                                        new object[1] { icdoPerson.person_id });
            if ((ldtPaymentElectionAdjustments.Rows.Count > 0) && (ldtPaymentElectionAdjustments.Rows[0]["monthly_amount"] != DBNull.Value))
                idecPendingAdjustments = Convert.ToDecimal(ldtPaymentElectionAdjustments.Rows[0]["monthly_amount"]);

            idecMemberSummaryTotalDueAmt = idecMemberSummaryNetPremiumAmt + idecMemberSummaryBalanceForward + idecAdjustmentsPaid - idecMemberPaymentsDone;
        }

        public void LoadPersonAccountEmploymentDetailEnrolled()
        {
            DataTable ldtPersonAccountEmploymentDetail = Select("cdoPerson.LoadEnrolledEmploymentDetails", new object[1] { icdoPerson.person_id });
            _icolEnrolledPersonAccountEmploymentDetail =
                GetCollection<busPersonAccountEmploymentDetail>(ldtPersonAccountEmploymentDetail, "icdoPersonAccountEmploymentDetail");
        }

        public void LoadPensionSummary()
        {
            if (icolPersonAccount == null)
                LoadPersonAccount();
            foreach (busPersonAccount lobjPersonAccount in icolPersonAccount)
            {
                if (lobjPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                {
                    if (lobjPersonAccount.ibusPlan == null)
                        lobjPersonAccount.LoadPlan();
                    if (lobjPersonAccount.ibusPerson == null)
                        lobjPersonAccount.ibusPerson = this;

                    if (lobjPersonAccount.ibusPersonAccountRetirement == null)
                        lobjPersonAccount.LoadPersonAccountRetirement();

                    if (lobjPersonAccount.ibusPersonAccountRetirement.ibusPlan == null)
                        lobjPersonAccount.ibusPersonAccountRetirement.ibusPlan = lobjPersonAccount.ibusPlan;

                    if (lobjPersonAccount.ibusPersonAccountRetirement.ibusPerson == null)
                        lobjPersonAccount.ibusPersonAccountRetirement.ibusPerson = this;

                    if (lobjPersonAccount.ibusPersonAccountRetirement.ibusPersonAccount == null)
                        lobjPersonAccount.ibusPersonAccountRetirement.ibusPersonAccount = lobjPersonAccount;

                    if (!lobjPersonAccount.ibusPersonAccountRetirement.iblnLTDSummaryLoaded)
                        lobjPersonAccount.ibusPersonAccountRetirement.LoadLTDSummary();

                    lobjPersonAccount.ibusPersonAccountRetirement.LoadTotalVSC();
                    lobjPersonAccount.ibusPersonAccountRetirement.LoadTotalPSC(); // PIR ID 2187 - Earlier, in Calculation method loads the PSC as of Retirement date

                    //PIR - 991
                    //only plans with status as withdrwan, retired, cancelled will be displayed in the closed pension summary tab,
                    //rest will be displayed in the active pension summary tab (including the records with history change date as future date 
                    //and status as  withdrwan, retired, cancelled )
                    //if (lobjPersonAccount.icdoPersonAccount.end_date_no_null > DateTime.Now)
                    // UAT PIR ID 1534 - PlanParticipationStatus Retirement TransferredTIAACREF should be display in closed Accounts.
                    if (lobjPersonAccount.IsClosedAccountStatus()
                        && lobjPersonAccount.icdoPersonAccount.history_change_date_no_null <= DateTime.Now)
                    {
                        if (iclbClosedPensionAccounts == null)
                            iclbClosedPensionAccounts = new Collection<busPersonAccountRetirement>();
                        iclbClosedPensionAccounts.Add(lobjPersonAccount.ibusPersonAccountRetirement);
                    }
                    else
                    {
                        if (iclbPensionAccounts == null)
                            iclbPensionAccounts = new Collection<busPersonAccountRetirement>();
                        iclbPensionAccounts.Add(lobjPersonAccount.ibusPersonAccountRetirement);
                    }
                }
                if (lobjPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeDeferredComp)
                {
                    if (iclbDeferredCompAccounts == null)
                    {
                        iclbDeferredCompAccounts = new Collection<busPersonAccountDeferredComp>();
                    }
                    lobjPersonAccount.ibusPersonDeferredComp = new busPersonAccountDeferredComp();
                    lobjPersonAccount.ibusPersonDeferredComp.icdoPersonAccount = new cdoPersonAccount();
                    lobjPersonAccount.ibusPersonDeferredComp.icdoPersonAccount = lobjPersonAccount.icdoPersonAccount;
                    lobjPersonAccount.ibusPersonDeferredComp.FindPersonAccountDeferredComp(lobjPersonAccount.icdoPersonAccount.person_account_id);
                    lobjPersonAccount.ibusPersonDeferredComp.LoadContributionSummary();
                    lobjPersonAccount.ibusPersonDeferredComp.LoadPlan();
                    lobjPersonAccount.ibusPersonDeferredComp.LoadPersonAccountProviderInfo();
                    lobjPersonAccount.ibusPersonDeferredComp.LoadAllPersonEmploymentDetails();
                    if (lobjPersonAccount.ibusPersonDeferredComp.iclbEmploymentDetail.Count > 0)
                    {
                        lobjPersonAccount.icdoPersonAccount.EmployerName =
                            lobjPersonAccount.ibusPersonDeferredComp.iclbEmploymentDetail[0].ibusPersonEmployment.ibusOrganization.icdoOrganization.org_name;
                    }
                    foreach (busPersonAccountDeferredCompProvider lobjDefProvider in lobjPersonAccount.ibusPersonDeferredComp.icolPersonAccountDeferredCompProvider)
                    {
                        if (lobjDefProvider.icdoPersonAccountDeferredCompProvider.end_date_no_null > DateTime.Today)
                        {
                            lobjPersonAccount.ibusPersonDeferredComp.idecCurrentPayPeriodAmount += lobjDefProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                        }
                    }
                    iclbDeferredCompAccounts.Add(lobjPersonAccount.ibusPersonDeferredComp);
                }
            }
        }

        public bool IsExcessContribitionPaid()
        {
            if (iclbPaymentDetails == null)
                LoadPaymentDetails();
            if (iclbPaymentDetails.Count > 0)
                return true;
            return false;
        }

        public void LoadInsuranceSummary()
        {
            if (icolPersonAccount == null)
                LoadPersonAccount();
            foreach (busPersonAccount lobjPersonAccount in icolPersonAccount)
            {
                if ((lobjPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance) ||
                    (lobjPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex))
                {
                    lobjPersonAccount.ibusPerson = this;
                    if (lobjPersonAccount.ibusPlan == null)
                        lobjPersonAccount.LoadPlan();
                    lobjPersonAccount.LoadProvider();
                    if (lobjPersonAccount.IsGhdvPlan)
                    {
                        if (lobjPersonAccount.ibusPersonAccountGHDV == null)
                            lobjPersonAccount.LoadPersonAccountGHDV();
                        lobjPersonAccount.ibusPersonAccountGHDV.ibusPerson = this;
                        lobjPersonAccount.ibusPersonAccountGHDV.ibusPlan = lobjPersonAccount.ibusPlan;
                        if (lobjPersonAccount.ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory == null)
                            lobjPersonAccount.ibusPersonAccountGHDV.LoadPersonAccountGHDVHistory();

                        //Initialize the Org Object to Avoid the NULL error
                        lobjPersonAccount.ibusPersonAccountGHDV.InitializeObjects();
                        lobjPersonAccount.ibusPersonAccountGHDV.LoadPlanEffectiveDate();
                        lobjPersonAccount.ibusPersonAccountGHDV.DetermineEnrollmentAndLoadObjects(lobjPersonAccount.ibusPersonAccountGHDV.idtPlanEffectiveDate, false);
                        if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                            lobjPersonAccount.icdoPersonAccount.end_date_no_null >= DateTime.Now)
                        {
                            if (lobjPersonAccount.ibusPersonAccountGHDV.IsHealthOrMedicare)
                            {
                                if (lobjPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                                {
                                    lobjPersonAccount.ibusPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                                }
                                else
                                {
                                    //Load the Health Plan Participation Date (based on effective Date)
                                    lobjPersonAccount.ibusPersonAccountGHDV.LoadHealthParticipationDate();
                                    //To Get the Rate Structure Code (Derived Field)
                                    lobjPersonAccount.ibusPersonAccountGHDV.LoadRateStructure();
                                }

                                //Get the Coverage Ref ID
                                lobjPersonAccount.ibusPersonAccountGHDV.LoadCoverageRefID();

                                lobjPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmountByRefID();
                            }
                            else
                            {
                                lobjPersonAccount.ibusPersonAccountGHDV.GetMonthlyPremiumAmount();
                            }

                            //UAT PIR: 1014
                            lobjPersonAccount.idecMemberPlanSummaryCurrentPremiumAmt = lobjPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.MonthlyPremiumAmount;
                            lobjPersonAccount.idecMemberPlanSummaryRhicAmt = lobjPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.total_rhic_amount;
                            lobjPersonAccount.idecMemberPlanSummaryNetPremiumAmt = lobjPersonAccount.idecMemberPlanSummaryCurrentPremiumAmt - lobjPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.total_rhic_amount;
                        }

                    }
                    if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                    {
                        if (lobjPersonAccount.ibusPersonAccountLife == null)
                            lobjPersonAccount.LoadPersonAccountLife();
                        lobjPersonAccount.ibusPersonAccountLife.ibusPerson = this;
                        lobjPersonAccount.ibusPersonAccountLife.ibusPlan = lobjPersonAccount.ibusPlan;
                        lobjPersonAccount.ibusPersonAccountLife.LoadPlanEffectiveDate();
                        lobjPersonAccount.ibusPersonAccountLife.icdoPersonAccount.person_employment_dtl_id = lobjPersonAccount.ibusPersonAccountLife.GetEmploymentDetailID();
                        if (lobjPersonAccount.ibusPersonAccountLife.icdoPersonAccount.person_employment_dtl_id != 0)
                        {
                            lobjPersonAccount.ibusPersonAccountLife.LoadPersonEmploymentDetail();
                            lobjPersonAccount.ibusPersonAccountLife.ibusPersonEmploymentDetail.LoadPersonEmployment();
                            lobjPersonAccount.ibusPersonAccountLife.LoadOrgPlan(lobjPersonAccount.ibusPersonAccountLife.idtPlanEffectiveDate);
                            lobjPersonAccount.ibusPersonAccountLife.LoadProviderOrgPlan(lobjPersonAccount.ibusPersonAccountLife.idtPlanEffectiveDate);
                        }
                        else
                        {
                            lobjPersonAccount.ibusPersonAccountLife.LoadActiveProviderOrgPlan(lobjPersonAccount.ibusPersonAccountLife.idtPlanEffectiveDate);
                        }
                        lobjPersonAccount.ibusPersonAccountLife.LoadLifeOptionData();
                        lobjPersonAccount.ibusPersonAccountLife.LoadMemberAge();
                        if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                            lobjPersonAccount.icdoPersonAccount.end_date_no_null >= DateTime.Now)
                        {
                            lobjPersonAccount.ibusPersonAccountLife.GetMonthlyPremiumAmount();
                            //UAT PIR: 1014
                            lobjPersonAccount.idecMemberPlanSummaryCurrentPremiumAmt = lobjPersonAccount.ibusPersonAccountLife.idecTotalMonthlyPremium;
                            lobjPersonAccount.idecMemberPlanSummaryNetPremiumAmt = lobjPersonAccount.ibusPersonAccountLife.idecTotalMonthlyPremium;
                        }
                    }
                    if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdLTC)
                    {
                        if (lobjPersonAccount.ibusPersonAccountLtc == null)
                            lobjPersonAccount.LoadPersonAccountLtc();
                        lobjPersonAccount.ibusPersonAccountLtc.ibusPerson = this;
                        lobjPersonAccount.ibusPersonAccountLtc.ibusPlan = lobjPersonAccount.ibusPlan;
                        lobjPersonAccount.ibusPersonAccountLtc.LoadPlanEffectiveDate();
                        lobjPersonAccount.ibusPersonAccountLtc.icdoPersonAccount.person_employment_dtl_id = lobjPersonAccount.ibusPersonAccountLtc.GetEmploymentDetailID();
                        if (lobjPersonAccount.ibusPersonAccountLtc.icdoPersonAccount.person_employment_dtl_id != 0)
                        {
                            lobjPersonAccount.ibusPersonAccountLtc.LoadPersonEmploymentDetail();
                            lobjPersonAccount.ibusPersonAccountLtc.ibusPersonEmploymentDetail.LoadPersonEmployment();
                            lobjPersonAccount.ibusPersonAccountLtc.LoadOrgPlan(lobjPersonAccount.ibusPersonAccountLtc.idtPlanEffectiveDate);
                            lobjPersonAccount.ibusPersonAccountLtc.LoadProviderOrgPlan(lobjPersonAccount.ibusPersonAccountLtc.idtPlanEffectiveDate);
                        }
                        else
                        {
                            lobjPersonAccount.ibusPersonAccountLtc.LoadActiveProviderOrgPlan(lobjPersonAccount.ibusPersonAccountLtc.idtPlanEffectiveDate);
                        }
                        lobjPersonAccount.ibusPersonAccountLtc.LoadLtcOptionUpdateMember();
                        lobjPersonAccount.ibusPersonAccountLtc.LoadLtcOptionUpdateSpouse();
                        if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                            lobjPersonAccount.icdoPersonAccount.end_date_no_null >= DateTime.Now)
                        {
                            lobjPersonAccount.ibusPersonAccountLtc.GetMonthlyPremiumAmount();
                            //UAT PIR: 1014
                            lobjPersonAccount.idecMemberPlanSummaryCurrentPremiumAmt = lobjPersonAccount.ibusPersonAccountLtc.idecTotalMonthlyPremium;
                            lobjPersonAccount.idecMemberPlanSummaryNetPremiumAmt = lobjPersonAccount.ibusPersonAccountLtc.idecTotalMonthlyPremium;
                        }
                    }
                    if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdEAP)
                    {
                        if (lobjPersonAccount.ibusPersonAccountEAP == null)
                            lobjPersonAccount.LoadPersonAccountEAP();
                        lobjPersonAccount.ibusPersonAccountEAP.ibusPerson = this;
                        lobjPersonAccount.ibusPersonAccountEAP.ibusPlan = lobjPersonAccount.ibusPlan;
                        if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                            lobjPersonAccount.icdoPersonAccount.end_date_no_null >= DateTime.Now)
                        {
                            lobjPersonAccount.ibusPersonAccountEAP.GetMonthlyPremium();
                            //UAT PIR: 1014
                            lobjPersonAccount.idecMemberPlanSummaryCurrentPremiumAmt = lobjPersonAccount.ibusPersonAccountEAP.idecMonthlyPremium;
                            lobjPersonAccount.idecMemberPlanSummaryNetPremiumAmt = lobjPersonAccount.ibusPersonAccountEAP.idecMonthlyPremium;
                        }
                    }

                    if (((lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) ||
                        (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled)) &&
                        (lobjPersonAccount.icdoPersonAccount.end_date_no_null >= DateTime.Now))
                    {
                        if (iclbActiveInsuranceAccounts == null)
                        {
                            iclbActiveInsuranceAccounts = new Collection<busPersonAccount>();
                        }
                        iclbActiveInsuranceAccounts.Add(lobjPersonAccount);
                    }
                    else
                    {
                        if (iclbClosedInsuranceAccounts == null)
                        {
                            iclbClosedInsuranceAccounts = new Collection<busPersonAccount>();
                        }
                        iclbClosedInsuranceAccounts.Add(lobjPersonAccount);
                    }
                }
            }
        }

        //modify query - Venkat
        public void LoadWorkflowProcessHistory()
        {
            DataTable ldtbAIH = busNeoSpinBase.Select("entSolBpmActivityInstance.LoadProcessInstanceHistoryByPerson", new object[1] { _icdoPerson.person_id });
            _iclbWorkflowProcessHistory = GetCollection<busBpmActivityInstanceHistory>(ldtbAIH, "icdoBpmActivityInstanceHistory");
        }

        protected override void LoadOtherObjects(DataRow adtrRow, busBase aobjBus)
        {
            if (aobjBus is busBpmActivityInstanceHistory)
            {
                busBpmActivityInstanceHistory lbusActivityInstanceHistory = (busBpmActivityInstanceHistory)aobjBus;
                if (!Convert.IsDBNull(adtrRow["STATUS_DESCRIPTION"]))
                {
                    lbusActivityInstanceHistory.icdoBpmActivityInstanceHistory.status_description = adtrRow["STATUS_DESCRIPTION"].ToString();
                }
                lbusActivityInstanceHistory.ibusBpmActivityInstance = new busSolBpmActivityInstance();
                 // lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmActivity = busBpmActivity.GetBpmActivityByActivityType("") ;
                lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmActivity = new busSolBpmActivity();

                if (!Convert.IsDBNull(adtrRow["ACTIVITY_NAME"]))
                {
                    lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmActivity.icdoBpmActivity.name = adtrRow["ACTIVITY_NAME"].ToString();
                }

                lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance = new busSolBpmProcessInstance();
                lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance = new busSolBpmCaseInstance();
                lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest = new busSolBpmRequest();
                if (!Convert.IsDBNull(adtrRow["SOURCE"]))
                {
                    lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance.ibusBpmRequest.icdoBpmRequest.source_description = adtrRow["SOURCE"].ToString();
                }

                if (!Convert.IsDBNull(adtrRow["contact_ticket_id"]))
                {
                    ((busSolBpmCaseInstance)lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmCaseInstance).contact_ticket_id = Convert.ToInt32(adtrRow["contact_ticket_id"]);
                }

                lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess = ClassMapper.GetObject<busBpmProcess>();
                if (!Convert.IsDBNull(adtrRow["Process_Description"]))
                {
                    lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.description = adtrRow["Process_Description"].ToString();
                }
                if (!Convert.IsDBNull(adtrRow["PROCESS_NAME"]))
                {
                    lbusActivityInstanceHistory.ibusBpmActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name = adtrRow["PROCESS_NAME"].ToString();
                }
            }
        }

        //Load Workflow Image Data
        public Collection<busSolBpmProcessInstanceAttachments> iclbProcessInstanceImageData { get; set; }

        public void LoadWorkflowImageData()
        {
            iclbProcessInstanceImageData = busFileNetHelper.LoadFileNetImagesByPerson(_icdoPerson.person_id);
        }

        private string _istrMember;
        public string istrMember
        {
            get
            {
                return _istrMember;
            }
            set
            {
                _istrMember = value;
            }

        }

        private string _istrSpouse;
        public string istrSpouse
        {
            get
            {
                return _istrSpouse;
            }
            set
            {
                _istrSpouse = value;
            }

        }

        private string _istrPayee;
        public string istrPayee
        {
            get
            {
                return _istrPayee;
            }
            set
            {
                _istrPayee = value;
            }

        }
        private string _istrRetiree;
        public string istrRetiree
        {
            get
            {
                return _istrRetiree;
            }
            set
            {
                _istrRetiree = value;
            }
        }
        private string _istrAlternatePayee;
        public string istrAlternatePayee
        {
            get
            {
                return _istrAlternatePayee;
            }
            set
            {
                _istrAlternatePayee = value;
            }
        }

        //used in correspondence
        public string iblnIsPOAExits
        {
            get
            {
                int lintGetCountOfPOA = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonContact.Check_If_POA_Exists", new object[1] { _icdoPerson.person_id },
                            iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                if (lintGetCountOfPOA > 0)
                {
                    return busConstant.Flag_Yes;
                }
                return busConstant.Flag_No;
            }
        }

        private string _istrPOA;
        public string istrPOA
        {
            get
            {
                return _istrPOA;
            }
            set
            {
                _istrPOA = value;
            }
        }
        private string _istrContactPerson;
        public string istrContactPerson
        {
            get
            {
                return _istrContactPerson;
            }
            set
            {
                _istrContactPerson = value;
            }
        }

        private string _istrBeneficiary;
        public string istrBeneficiary
        {
            get { return _istrBeneficiary; }
            set { _istrBeneficiary = value; }
        }

        private string _istrDependent;
        public string istrDependent
        {
            get { return _istrDependent; }
            set { _istrDependent = value; }
        }

        private Collection<busBenefitDroApplication> _iclbDROApplication;
        public Collection<busBenefitDroApplication> iclbDROApplication
        {
            get { return _iclbDROApplication; }
            set { _iclbDROApplication = value; }
        }

        public void LoadDROApplications()
        {
            if (iclbMemberDROApplication.IsNull())
                LoadMemberDROApplications(); ;
            if (iclbAlternatePayeeDROApplication.IsNull())
                LoadAlternatePayeeDROApplicaitons();

            if (iclbMemberDROApplication.Count > 0)
                _iclbDROApplication = iclbMemberDROApplication;
            else
                _iclbDROApplication = iclbAlternatePayeeDROApplication;

            foreach (busBenefitDroApplication lobjBenefitDroApplication in _iclbDROApplication)
            {
                lobjBenefitDroApplication.LoadAlternatePayee();
                lobjBenefitDroApplication.LoadMember();
                lobjBenefitDroApplication.LoadPlan();
            }
        }

        /// <summary>
        /// Load the Person Type flags to display in the Person maintenance screen.
        /// </summary>
        public void LoadPersonTypes()
        {
            // Check for Beneficiary
            if (_iclbBeneficiaryTo == null)
                LoadBeneficiaryTo();
            if (_iclbBeneficiaryTo.Count > 0)
                _istrBeneficiary = busConstant.Flag_Yes;
            else
                _istrBeneficiary = busConstant.Flag_No;

            // Check for Dependent
            if (_iclbDependentTo == null)
                LoadDependentTo();
            if (_iclbDependentTo.Count > 0)
                _istrDependent = busConstant.Flag_Yes;
            else
                _istrDependent = busConstant.Flag_No;

            // Check for Active Member
            if (IsMember())
                _istrMember = busConstant.Flag_Yes;
            else
                _istrMember = busConstant.Flag_No;

            // Check for Retiree
            if (IsRetiree())
                _istrRetiree = busConstant.Flag_Yes;
            else
                _istrRetiree = busConstant.Flag_No;

            // Check for Spouse
            if (IsActiveSpouseToContact())
                _istrSpouse = busConstant.Flag_Yes;
            else
                _istrSpouse = busConstant.Flag_No;

            //POA
            if (IsActivePOA())
                _istrPOA = busConstant.Flag_Yes;
            else
                _istrPOA = busConstant.Flag_No;

            if (IsAlternatePayee())
                _istrAlternatePayee = busConstant.Flag_Yes;
            else
                _istrAlternatePayee = busConstant.Flag_No;

            if (IsDisabilityPrePostRetirementPayee())
                _istrPayee = busConstant.Flag_Yes;
            else
                _istrPayee = busConstant.Flag_No;

            //UAT PIR 1506 - If alternate payee has valid payee account, then check the payee flag also
            if (_istrPayee == busConstant.Flag_No)
            {
                if (iclbAlternatePayeeDROApplication == null)
                    LoadAlternatePayeeDROApplicaitons();
                var lenuList = iclbAlternatePayeeDROApplication.Where(i => i.icdoBenefitDroApplication.dro_status_value != busConstant.DROApplicationStatusCancelled);
                foreach (busBenefitDroApplication lbusBenefitDroApplication in lenuList)
                {
                    if (lbusBenefitDroApplication.iclbPayeeAccount == null)
                        lbusBenefitDroApplication.LoadPayeeAccount();
                    foreach (busPayeeAccount lbusPayeeAccount in lbusBenefitDroApplication.iclbPayeeAccount)
                    {
                        //Commented since LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                        //if (lbusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                        lbusPayeeAccount.LoadActivePayeeStatus();
                    }
                    if (lbusBenefitDroApplication.iclbPayeeAccount.Any(i => (!i.ibusPayeeAccountActiveStatus.IsStatusCompleted()) && (!i.ibusPayeeAccountActiveStatus.IsStatusCancelled())))
                    {
                        _istrPayee = busConstant.Flag_Yes;
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// Returns true if the Person is Spouse to some Contact.
        /// </summary>
        /// <returns></returns>
        public bool IsActiveSpouseToContact()
        {
            if (iclbPersonContactTo == null)
                LoadPersonContactTo();

            if (iclbPersonContactTo.Any(i => i.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse &&
                                             i.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the Person is POA to some contact
        /// </summary>
        /// <returns></returns>
        public bool IsActivePOA()
        {
            if (iclbPersonContactTo == null)
                LoadPersonContactTo();

            if (iclbPersonContactTo.Any(i => i.icdoPersonContact.relationship_value == busConstant.PersonContactTypePOA &&
                                             i.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// The ‘Member’ checkbox should be checked when the member has a person account record for any plan.  
        ///Exceptions: All Plan Participation Statuses are ‘Cancelled’
        /// </summary>
        /// <returns> True</returns>
        public bool IsMember()
        {
            if (icolPersonAccount == null)
                LoadPersonAccount(false);

            int lintCancelledPlanCount = 0;
            foreach (busPersonAccount lbusPersonAccount in icolPersonAccount)
            {
                if (lbusPersonAccount.icdoPersonAccount.is_plan_cancelled)
                {
                    lintCancelledPlanCount++;
                }
            }
            //If member has some person account and not all the Plans are cancelled
            if ((icolPersonAccount.Count > 0) && (icolPersonAccount.Count != lintCancelledPlanCount))
            {
                return true;
            }
            return false;
        }

        public bool IsAlternatePayee()
        {
            if (iclbAlternatePayeeDROApplication == null)
                LoadAlternatePayeeDROApplicaitons();

            //If the DRO Apps other than cancelled exists
            if (iclbAlternatePayeeDROApplication.Any(i => i.icdoBenefitDroApplication.dro_status_value != busConstant.DROApplicationStatusCancelled))
                return true;
            return false;
        }

        public bool IsDisabilityPrePostRetirementPayee()
        {
            if (iclbPayeeAccount == null)
                LoadPayeeAccount();

            //Load the Status
            foreach (busPayeeAccount lbusPayeeAccount in iclbPayeeAccount)
            {
                //Commented since LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                //if (lbusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                lbusPayeeAccount.LoadActivePayeeStatus();
            }

            if (iclbPayeeAccount.Any(i => (i.IsBenefitAccountTypeIsDisability() || i.IsBenefitAccountTypePreRetirementDeath() || i.IsBenefitAccountTypePostRetirementDeath()) &&
                                        (!i.ibusPayeeAccountActiveStatus.IsStatusCompleted()) && (!i.ibusPayeeAccountActiveStatus.IsStatusCancelled())))
                return true;

            return false;
        }


        /// <summary>
        /// Loads the Current Active Person Address
        /// </summary>		        
        public void LoadPersonCurrentAddress()
        {
            LoadPersonCurrentAddress(true, DateTime.Today);
        }
        /// <summary>
        /// Loads the Current Active Person Address for the date given
        /// </summary>		        
        public void LoadPersonCurrentAddress(DateTime adteGivenDate)
        {
            LoadPersonCurrentAddress(true, adteGivenDate);
        }
        /// <summary>
        /// Loads the Person Object by the Given Person Id
        /// </summary>		
        /// <param name="ablnIgnorePayment">Flag to Ignore Payment Address Type</param> 
        public void LoadPersonCurrentAddress(bool ablnIgnorePayment, DateTime adteGivenDate)
        {
            //Check the Payment Address
            if (!ablnIgnorePayment)
            {
                if (GetPersonCurrentAddressByType(busConstant.AddressTypePayment, adteGivenDate))
                {
                    //If any records found, return;
                    return;
                }
            }
            //Check the Temporary Address
            if (GetPersonCurrentAddressByType(busConstant.AddressTypeTemporary, adteGivenDate))
            {
                //If any records found, return;
                return;
            }
            //Check the Permanent Address
            if (GetPersonCurrentAddressByType(busConstant.AddressTypePermanent, adteGivenDate))
            {
                //If any records found, return;
                return;
            }
        }

        public bool GetPersonCurrentAddressByType(string astrAddressType, DateTime adteGivenDate)
        {
            _ibusPersonCurrentAddress = new busPersonAddress();
            _ibusPersonCurrentAddress.icdoPersonAddress = new cdoPersonAddress();

            if (iclbPersonAddress == null)
                LoadPersonAddress();

            IEnumerable<busPersonAddress> ienuPersonAddressByType =
                iclbPersonAddress.Where(i => i.icdoPersonAddress.address_type_value == astrAddressType);
            if (ienuPersonAddressByType != null)
            {
                foreach (busPersonAddress lbusPersonAddress in ienuPersonAddressByType)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(adteGivenDate,
                                                                lbusPersonAddress.icdoPersonAddress.start_date,
                                                                lbusPersonAddress.icdoPersonAddress.end_date))
                    {
                        _ibusPersonCurrentAddress = lbusPersonAddress;
                        _ibusPersonCurrentAddress.icdoPersonCurrentAddress = lbusPersonAddress.icdoPersonAddress;
                        return true;
                    }
                }
            }
            return false;
        }

        public void LoadAddresses()
        {
            DataTable ldtbList = Select<cdoPersonAddress>(
                new string[1] { "person_id" },
                new object[1] { icdoPerson.person_id }, null, "start_date");
            _icolPersonAddress = GetCollection<busPersonAddress>(ldtbList, "icdoPersonAddress"); 
        }

        //pir 8664
        public Collection<busPersonAddress> iclbPersonFutureAddress { get; set; }
        public bool iblnIsFutureAddressAvailable { get; set; }
        public void LoadFutureAddresses()
        {
            DataTable ldtbList = SelectWithOperator<cdoPersonAddress>(
                new string[2] { "person_id","start_date" },
                new string[2] { "=" , ">" },
                new object[2] { icdoPerson.person_id, DateTime.Now }, "start_date");
            iclbPersonFutureAddress = GetCollection<busPersonAddress>(ldtbList, "icdoPersonAddress");
            if (iclbPersonFutureAddress.Count() > 0)
                iblnIsFutureAddressAvailable = true;
        }

        public void LoadPersonEmployment(bool ablnLoadOtherObjects)
        {
            DataTable ldtbList = Select<cdoPersonEmployment>(
                new string[1] { "person_id" },
                new object[1] { icdoPerson.person_id }, null, "start_date DESC");
            _icolPersonEmployment = GetCollection<busPersonEmployment>(ldtbList, "icdoPersonEmployment");
            if (ablnLoadOtherObjects)
            {
                foreach (busPersonEmployment lobjPersonEmployment in _icolPersonEmployment)
                {
                    lobjPersonEmployment.LoadOrganization();
                    lobjPersonEmployment.icdoPersonEmployment.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjPersonEmployment.icdoPersonEmployment.org_id);
                    //BPM Death Automation
                    lobjPersonEmployment.icdoPersonEmployment.iblnEmploymentEndDateFromDeath = lobjPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue ? true : false;
                }
            }
        }
        public string istrCurrentEmployerName { get; set; }
        public string istrCurrentEmployerOrgCode { get; set; }

        public string istrMultiplePlanName { get; set; }

        public string istrDollar1 { get; set; }
        public string istrDollar2 { get; set; }
        public string istrDollar3 { get; set; }
        public string istrNumber1 { get; set; }
        public string istrNumber2 { get; set; }
        public string istrNumber3 { get; set; }
        public string istrCheckNumberDollar { get; set; }


        public void LoadCurrentEmployerDetails()
        {
            if (iclbActivePersonEmployment == null)
                LoadActivePersonEmployment();
            if (iclbActivePersonEmployment.Count > 0)
            {
                if (iclbActivePersonEmployment[0].ibusOrganization == null)
                    iclbActivePersonEmployment[0].LoadOrganization();
                istrCurrentEmployerName = iclbActivePersonEmployment[0].ibusOrganization.icdoOrganization.org_name;
                istrCurrentEmployerOrgCode = iclbActivePersonEmployment[0].ibusOrganization.icdoOrganization.org_code;
            }
        }
        public void LoadPersonEmployment()
        {
            LoadPersonEmployment(true);
        }

        public Collection<busPersonEmployment> iclbActivePersonEmployment { get; set; }
        public void LoadActivePersonEmployment()
        {
            if (iclbActivePersonEmployment == null)
                iclbActivePersonEmployment = new Collection<busPersonEmployment>();

            if (icolPersonEmployment == null)
                LoadPersonEmployment(false);

            foreach (busPersonEmployment lbusPersonEmployment in icolPersonEmployment)
            {
                if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                                                            lbusPersonEmployment.icdoPersonEmployment.start_date,
                                                            lbusPersonEmployment.icdoPersonEmployment.end_date_no_null))
                {
                    iclbActivePersonEmployment.Add(lbusPersonEmployment);
                }
            }
        }
        public Collection<busPayeeAccount> iclbPayeeAccountsByMemberID { get; set; }
        public void LoadPayeeAccountsByMemberID()
        {
            iclbPayeeAccountsByMemberID = new Collection<busPayeeAccount>();
            DataTable ldtbList = busBase.Select("entPayeeAccount.LoadPayeeAccountByMemberPerslinkId",
                                                            new object[1] { icdoPerson.person_id });

            iclbPayeeAccountsByMemberID = GetCollection<busPayeeAccount>(ldtbList, "icdoPayeeAccount");
            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccountsByMemberID)
            {
                //Dont check Null condition as LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                lobjPayeeAccount.LoadActivePayeeStatus();
            }
        }
        
        public void LoadPersonAccount()
        {
            LoadPersonAccount(true);
        }

        public void LoadPersonAccount(bool ablnLoadOtherObjects)
        {
            DataTable ldtbList = Select<cdoPersonAccount>(
                new string[1] { "person_id" },
                new object[1] { icdoPerson.person_id }, null, null);
            _icolPersonAccount = GetCollection<busPersonAccount>(ldtbList, "icdoPersonAccount");

            if (ablnLoadOtherObjects)
            {
                foreach (busPersonAccount lobjPersonAcc in _icolPersonAccount)
                {
                    if (idtbPlanCacheData != null)
                        lobjPersonAcc.idtbPlanCacheData = idtbPlanCacheData;
                    lobjPersonAcc.LoadPlan();
                }
                //prod pir 5554
                if ((_icolPersonAccount != null) && (_icolPersonAccount.Count > 0))
                    busGlobalFunctions.Sort<busPersonAccount>("ibusPlan.icdoPlan.sort_order", _icolPersonAccount);
            }
        }

        public void LoadPersonAccountExcludeWithdrawn()
        {
            _icolPersonAccount = new Collection<busPersonAccount>();
            DataTable ldtbList = Select<cdoPersonAccount>(
                new string[1] { "person_id" },
                new object[1] { icdoPerson.person_id }, null, null);
            Collection<busPersonAccount> lcolPersonAccount = GetCollection<busPersonAccount>(ldtbList, "icdoPersonAccount");

            foreach (busPersonAccount lbusPersonAccount in lcolPersonAccount)
            {
                if (!lbusPersonAccount.IsWithDrawn())
                    icolPersonAccount.Add(lbusPersonAccount);
            }
        }
        public void LoadPersonAccountByBenefitType(string astrBenefitType)
        {
            LoadPersonAccountByBenefitType(astrBenefitType, false);
        }
        public void LoadPersonAccountByBenefitType(string astrBenefitType, bool ablnExcludeTransferToDC, bool ablnIsFromPensionFile = false)
        {
            _icolPersonAccountByBenefitType = new Collection<busPersonAccount>();
            if (_icolPersonAccount == null)
                LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in _icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();

                if (lbusPersonAccount.ibusPlan.icdoPlan.benefit_type_value == astrBenefitType)
                {
                    //PIR: 2034: Do not Include the Withdrawn Person Account When Loading the Person Account in case of 
                    //Retirement Plans. 
                    if (astrBenefitType == busConstant.PlanBenefitTypeRetirement)
                    {
                        if (!lbusPersonAccount.IsWithDrawn() || ablnIsFromPensionFile)//PIR 17486 - Issue 1
                        {
                            if (ablnExcludeTransferToDC)
                            {
                                if (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusTransferDC)
                                {
                                    _icolPersonAccountByBenefitType.Add(lbusPersonAccount);
                                }
                            }
                            else
                            {
                                _icolPersonAccountByBenefitType.Add(lbusPersonAccount);
                            }
                        }
                    }
                    else
                    {
                        _icolPersonAccountByBenefitType.Add(lbusPersonAccount);
                    }
                }
            }
        }

        public void LoadPersonAccountByPlan(int aintPlanID)
        {
            icolPersonAccountByPlan = new Collection<busPersonAccount>();
            if (_icolPersonAccount == null)
                LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in _icolPersonAccount)
            {
                if (lbusPersonAccount.icdoPersonAccount.plan_id == aintPlanID)
                    icolPersonAccountByPlan.Add(lbusPersonAccount);
            }
        }

        public busPersonAccount ibusHealthPersonAccount { get; set; }
        public void LoadHealthPersonAccount()
        {
            if (ibusHealthPersonAccount == null)
                ibusHealthPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            LoadPersonAccountByPlan(busConstant.PlanIdGroupHealth);
            if (icolPersonAccountByPlan.Count > 0)
                ibusHealthPersonAccount = icolPersonAccountByPlan.First();
        }

        public busPersonAccount ibusLifePersonAccount { get; set; }
        public void LoadLifePersonAccount()
        {
            if (ibusLifePersonAccount == null)
                ibusLifePersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            LoadPersonAccountByPlan(busConstant.PlanIdGroupLife);
            if (icolPersonAccountByPlan.Count > 0)
                ibusLifePersonAccount = icolPersonAccountByPlan.First();
        }

        public void LoadPayeeAccount()
        {
            LoadPayeeAccount(false);
        }

        //Load payee account status 
        public void LoadPayeeAccount(bool ablnLoadStatus)
        {
            DataTable ldtbList = Select<cdoPayeeAccount>(
                          new string[1] { "PAYEE_PERSLINK_ID" },
                          new object[1] { icdoPerson.person_id }, null, null);
            _iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtbList, "icdoPayeeAccount");
            if (ablnLoadStatus)
            {
                foreach (busPayeeAccount lobjPayeeAccount in _iclbPayeeAccount)
                {
                    //Dont check Null condition as LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                    lobjPayeeAccount.LoadActivePayeeStatus();
                    lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
                                busGlobalFunctions.GetData2ByCodeValue(2203, lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                }
            }
        }

        public void LoadContacts()
        {
            DataTable ldtbList = Select<cdoPersonContact>(
                new string[1] { "person_id" },
                new object[1] { icdoPerson.person_id }, null, null);
            _icolPersonContact = GetCollection<busPersonContact>(ldtbList, "icdoPersonContact");
            foreach (busPersonContact lobjTemp in _icolPersonContact)
            {
                lobjTemp.icdoPersonContact.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjTemp.icdoPersonContact.contact_org_id);
                lobjTemp.LoadContactName();
            }
        }
        public void LoadSpouseContact()
        {
            if (ibusDeathNotification.IsNull())
                LoadDeathNotification();
            DataTable ldtbCertifiedDeathNotification = busBase.Select("entPersonContact.LoadMembersSpouseContact", new object[1] { ibusDeathNotification.icdoDeathNotification.person_id });
            _icolPersonContact = GetCollection<busPersonContact>(ldtbCertifiedDeathNotification, "icdoPersonContact");
            foreach (busPersonContact lobjTemp in _icolPersonContact)
            {
                lobjTemp.icdoPersonContact.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjTemp.icdoPersonContact.contact_org_id);
                lobjTemp.LoadContactName();
            }
        }

        public void LoadActiveContacts()
        {
            DataTable ldtbList = Select<cdoPersonContact>(
                new string[2] { "person_id", "status_value" },
                new object[2] { icdoPerson.person_id, busConstant.PersonContactStatusActive }, null, null);
            _icolPersonContact = GetCollection<busPersonContact>(ldtbList, "icdoPersonContact");
            foreach (busPersonContact lobjTemp in _icolPersonContact)
            {
                lobjTemp.icdoPersonContact.istrOrgCodeID = busGlobalFunctions.GetOrgCodeFromOrgId(lobjTemp.icdoPersonContact.contact_org_id);
                lobjTemp.LoadContactName();
            }
        }

        public void LoadPersonAddress()
        {
            DataTable ldtbList = Select<cdoPersonAddress>(
                new string[1] { "person_id" },
                new object[1] { icdoPerson.person_id }, null, null);
            iclbPersonAddress = GetCollection<busPersonAddress>(ldtbList, "icdoPersonAddress");
        }

        public void LoadTffrTiaaService()
        {
            DataTable ldtbList = Select<cdoPersonTffrTiaaService>(
                new string[1] { "person_id" },
                new object[1] { icdoPerson.person_id }, null, null);
            _iclbTffrTiaaService = GetCollection<busPersonTffrTiaaService>(ldtbList, "icdoPersonTffrTiaaService");
        }
        //PIR - 2065 2064 1863
        //only if the person having beneficiary then only add to this collection
        public Collection<busPersonBeneficiary> iclbPersonBeneficiaryWithoutPlanAccounts { get; set; }
        public void LoadBeneficiary()
        {
            DataTable ldtbBeneficiaries = Select("cdoPerson.LoadBeneficiaries", new object[1] { _icdoPerson.person_id });
            _iclbPersonBeneficiary = new Collection<busPersonBeneficiary>();
            iclbPersonBeneficiaryCor = new Collection<busPersonBeneficiary>();
            iclbPersonBeneficiaryWithoutPlanAccounts = new Collection<busPersonBeneficiary>();

            foreach (DataRow drBeneficiary in ldtbBeneficiaries.Rows)
            {
                busPersonBeneficiary lobjPersonBeneficiary = new busPersonBeneficiary();
                lobjPersonBeneficiary.icdoPersonBeneficiary = new cdoPersonBeneficiary();
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary = new busPersonAccountBeneficiary();
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary();
                lobjPersonBeneficiary.icdoPersonBeneficiary.LoadData(drBeneficiary);
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.LoadData(drBeneficiary);
                busPerson lbusBenPerson = null;
                busOrganization lbusBenOrg = null;
                //PIR - 2568 Person object is not loading in the query thats why lbusBenPerson is set to null
                //beneficiary person is loaded confilcting with person id, name fields                
                if (lobjPersonBeneficiary.icdoPersonBeneficiary.benificiary_org_id > 0)
                {
                    lbusBenOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
                    lbusBenOrg.icdoOrganization.LoadData(drBeneficiary);
                }
                lobjPersonBeneficiary.LoadBeneficiaryInfo(lbusBenPerson, lbusBenOrg);

                // PIR - 1863 // PIR 5248
                _iclbPersonBeneficiary.Add(lobjPersonBeneficiary);
                iclbPersonBeneficiaryWithoutPlanAccounts.Add(lobjPersonBeneficiary);
                // PIR 21162 - Only select the Active beneficiaries For Correspondence
                DateTime ldteSystBatchDate = busGlobalFunctions.GetSysManagementBatchDate(); 
                    if (busGlobalFunctions.CheckDateOverlapping(ldteSystBatchDate,
                            lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                            lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date))
                    iclbPersonBeneficiaryCor.Add(lobjPersonBeneficiary);
            }
            _iclbPersonBeneficiary = busGlobalFunctions.Sort<busPersonBeneficiary>(
                                "ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.sort_order",
                                _iclbPersonBeneficiary);
            iclbPersonBeneficiaryWithoutPlanAccounts = busGlobalFunctions.Sort<busPersonBeneficiary>(
                             "ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.sort_order",
                             iclbPersonBeneficiaryWithoutPlanAccounts);
            iclbPersonBeneficiaryCor = busGlobalFunctions.Sort<busPersonBeneficiary>(
                                "ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.sort_order",
                                iclbPersonBeneficiaryCor);
        }


        public void LoadApplicationBeneficiary(bool ablnFromDRO = false)
        {
            DataTable ldtbBeneficiaries = Select("cdoPerson.LoadApplicationBeneficiary", new object[1] { _icdoPerson.person_id });
            _iclbPersonBeneficiary = new Collection<busPersonBeneficiary>();
            foreach (DataRow drBeneficiary in ldtbBeneficiaries.Rows)
            {
                if ((ablnFromDRO && drBeneficiary["dro_application_id"] != DBNull.Value && Convert.ToInt32(drBeneficiary["dro_application_id"]) > 0) ||
                    !ablnFromDRO)
                {
                    busPersonBeneficiary lobjPersonBeneficiary = new busPersonBeneficiary();
                    lobjPersonBeneficiary.icdoPersonBeneficiary = new cdoPersonBeneficiary();
                    lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary = new busBenefitApplicationBeneficiary();
                    lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary = new cdoBenefitApplicationBeneficiary();

                    lobjPersonBeneficiary.icdoPersonBeneficiary.LoadData(drBeneficiary);
                    lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.LoadData(drBeneficiary);
                    lobjPersonBeneficiary.LoadBeneficiaryInfo();
                    _iclbPersonBeneficiary.Add(lobjPersonBeneficiary);
                }
            }
            _iclbPersonBeneficiary = busGlobalFunctions.Sort<busPersonBeneficiary>(
                                "ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.sort_order",
                                _iclbPersonBeneficiary);
        }


        public void LoadPersonOverviewBeneficiary(bool ablnIsCorrespondence = false)
        {
            DataTable ldtbBeneficiaries = Select("cdoPerson.LoadOverviewBeneficiaries", new object[1] { _icdoPerson.person_id });
            _iclbPersonBeneficiary = new Collection<busPersonBeneficiary>();
            iclbPersonBeneficiaryWithoutPlanAccounts = new Collection<busPersonBeneficiary>();
            foreach (DataRow drBeneficiary in ldtbBeneficiaries.Rows)
            {
                busPersonBeneficiary lobjPersonBeneficiary = new busPersonBeneficiary();
                lobjPersonBeneficiary.icdoPersonBeneficiary = new cdoPersonBeneficiary();
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary = new busPersonAccountBeneficiary();
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary();
                lobjPersonBeneficiary.icdoPersonBeneficiary.LoadData(drBeneficiary);
                lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.LoadData(drBeneficiary);
                busPerson lbusBenPerson = null;
                busOrganization lbusBenOrg = null;
                //PIR - 2606 Person object is not loading in the query thats why lbusBenPerson is set to null
                //beneficiary person is loaded conflicting with person id, name fields      
                if (lobjPersonBeneficiary.icdoPersonBeneficiary.benificiary_org_id > 0)
                {
                    lbusBenOrg = new busOrganization { icdoOrganization = new cdoOrganization() };
                    lbusBenOrg.icdoOrganization.LoadData(drBeneficiary);
                }
                lobjPersonBeneficiary.LoadBeneficiaryInfo(lbusBenPerson, lbusBenOrg);

                if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id > 0)
                {
                    if(ablnIsCorrespondence)
                    {
                        // PROD PIR ID 5259 - Only select the Active beneficiaries to the Letter
                        if(busGlobalFunctions.CheckDateOverlapping(DateTime.Today,
                            lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                            lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date))
                            _iclbPersonBeneficiary.Add(lobjPersonBeneficiary);
                    }
                    else
                        _iclbPersonBeneficiary.Add(lobjPersonBeneficiary);
                }
                iclbPersonBeneficiaryWithoutPlanAccounts.Add(lobjPersonBeneficiary);
            }
            _iclbPersonBeneficiary = busGlobalFunctions.Sort<busPersonBeneficiary>(
                                "ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.sort_order",
                                _iclbPersonBeneficiary);
            iclbPersonBeneficiaryWithoutPlanAccounts = busGlobalFunctions.Sort<busPersonBeneficiary>(
                             "ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.sort_order",
                             iclbPersonBeneficiaryWithoutPlanAccounts);
        }

        public void LoadDependent()
        {
            DataTable ldtbDependents = Select("cdoPerson.LoadDependents", new object[1] { _icdoPerson.person_id });
            _iclbPersonDependent = new Collection<busPersonDependent>();
            foreach (DataRow drDependent in ldtbDependents.Rows)
            {
                busPersonDependent lobjPersonDependent = new busPersonDependent();
                lobjPersonDependent.ibusPeronAccountDependent = new busPersonAccountDependent();
                lobjPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent = new cdoPersonAccountDependent();
                lobjPersonDependent.icdoPersonDependent = new cdoPersonDependent();
                lobjPersonDependent.icdoPersonDependent.LoadData(drDependent);
                lobjPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.LoadData(drDependent);
                //PIR - 2568 Person object is not loading in the query thats why lbusBenPerson is set to null
                //beneficiary person is loaded confilcting with person id, name fields      
                lobjPersonDependent.LoadDependentInfo();
                _iclbPersonDependent.Add(lobjPersonDependent);
            }
        }

        public void LoadPersonOverviewDependent()
        {
            DataTable ldtbDependents = Select("cdoPerson.LoadOverviewDependents", new object[1] { _icdoPerson.person_id });
            _iclbPersonDependent = new Collection<busPersonDependent>();
            foreach (DataRow drDependent in ldtbDependents.Rows)
            {
                busPersonDependent lobjPersonDependent = new busPersonDependent();
                lobjPersonDependent.ibusPeronAccountDependent = new busPersonAccountDependent();
                lobjPersonDependent.icdoPersonDependent = new cdoPersonDependent();
                lobjPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent = new cdoPersonAccountDependent();
                lobjPersonDependent.icdoPersonDependent.LoadData(drDependent);
                lobjPersonDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.LoadData(drDependent);
                //PIR - 2606 Person object is not loading in the query thats why lbusBenPerson is set to null
                //beneficiary person is loaded confilcting with person id, name fields      
                lobjPersonDependent.LoadDependentInfo();
                _iclbPersonDependent.Add(lobjPersonDependent);
            }
        }

        public void LoadContactsAppointments()
        {
            _icolPersonContactTicket = new Collection<busContactTicket>();
            DataTable ldtbList = busNeoSpinBase.Select("cdoPerson.LOAD_CONTACT_TICKET", new object[1] { _icdoPerson.person_id });
            foreach (DataRow adrRow in ldtbList.Rows)
            {
                busContactTicket lbusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
                lbusContactTicket.icdoContactTicket.LoadData(adrRow);

                if (lbusContactTicket.icdoContactTicket.contact_type_value == busConstant.ContactTicketTypeAppointment)
                {
                    lbusContactTicket.ibusAppointmentSchedule = new busAppointmentSchedule { icdoAppointmentSchedule = new cdoAppointmentSchedule() };
                    lbusContactTicket.ibusAppointmentSchedule.icdoAppointmentSchedule.LoadData(adrRow);
                    lbusContactTicket.ibusCounselor = new busUser { icdoUser = new cdoUser() };
                    if (!Convert.IsDBNull(adrRow["counselor_first_name"]))
                        lbusContactTicket.ibusCounselor.icdoUser.first_name = adrRow["counselor_first_name"].ToString();
                    if (!Convert.IsDBNull(adrRow["counselor_last_name"]))
                        lbusContactTicket.ibusCounselor.icdoUser.last_name = adrRow["counselor_last_name"].ToString();
                }

                lbusContactTicket.ibusUser = new busUser { icdoUser = new cdoUser() };
                lbusContactTicket.ibusUser.icdoUser.LoadData(adrRow);
                _icolPersonContactTicket.Add(lbusContactTicket);
            }
        }

        public void LoadSeminarAttendance()
        {
            DataTable ldtbList = busNeoSpinBase.Select("cdoPerson.LOAD_SEMINAR_ATTENDANCE", new object[1] { _icdoPerson.person_id });
            _icolSeminarAttendeeDetail = new Collection<busSeminarAttendeeDetail>();
            foreach (DataRow ldrRow in ldtbList.Rows)
            {
                busSeminarAttendeeDetail lbusSemAttDetail = new busSeminarAttendeeDetail { icdoSeminarAttendeeDetail = new cdoSeminarAttendeeDetail() };
                lbusSemAttDetail.icdoSeminarAttendeeDetail.LoadData(ldrRow);

                lbusSemAttDetail.ibusSeminarSchedule = new busSeminarSchedule { icdoSeminarSchedule = new cdoSeminarSchedule() };
                lbusSemAttDetail.ibusSeminarSchedule.icdoSeminarSchedule.LoadData(ldrRow);

                lbusSemAttDetail.ibusSeminarSchedule.ibusContactTicket = new busContactTicket { icdoContactTicket = new cdoContactTicket() };
                lbusSemAttDetail.ibusSeminarSchedule.ibusContactTicket.icdoContactTicket.LoadData(ldrRow);

                lbusSemAttDetail.ibusFacilitator = new busUser { icdoUser = new cdoUser() };
                lbusSemAttDetail.ibusFacilitator.icdoUser.LoadData(ldrRow);
                _icolSeminarAttendeeDetail.Add(lbusSemAttDetail);
            }
        }

        /// <summary>
        /// Validate the Duplicate SSN in PERSON Table 
        /// </summary>		
        /// <returns>bool</returns>
        public bool CheckSSNExists()
        {
            bool lblnResult = false;
            if (icdoPerson.ssn != null)
            {
                //first if check for person indexing maintenance
                if (_icdoPerson.ihstOldValues.Count > 0)
                {
                    if ((string)_icdoPerson.ihstOldValues["ssn"] != icdoPerson.ssn.ToString())
                    {
                        int Count = (int)DBFunction.DBExecuteScalar("cdoPerson.VALIDATE_DUPLICATE_SSN_UPDATE",
                            new object[2] { _icdoPerson.person_id, _icdoPerson.ssn }
                        , iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                        if (Count > 0)
                        {
                            lblnResult = true;
                        }
                    }
                }
            }
            return lblnResult;
        }
        /// <summary>
        /// Gender and Prefix Verification
        /// </summary>		
        /// <returns>bool</returns>        
        public bool CheckGenderNamePrefix()
        {
            if (((icdoPerson.gender_value == busConstant.GenderTypeFemale) && (icdoPerson.name_prefix_value == busConstant.NamePrefixMR))
            || ((icdoPerson.gender_value == busConstant.GenderTypeMale) && ((icdoPerson.name_prefix_value == busConstant.NamePrefixMISS)
            || (icdoPerson.name_prefix_value == busConstant.NamePrefixMRS)
            || (icdoPerson.name_prefix_value == busConstant.NamePrefixMS))))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Valaidate Email
        /// </summary>		
        /// <returns>bool</returns>   
        public bool ValidateEmail()
        {
            if (_icdoPerson.email_address != null)
            {
                return busGlobalFunctions.IsEmailValid(_icdoPerson.email_address);  
            }
            return true;
        }

		
		
        public override busBase GetCorPerson()
        {
            return this;
        }

        public override busBase GetCorOrganization()
        {
            if (ibusCurrentEmployment == null)
            {
                LoadCurrentEmployer();
            }
            if (ibusCurrentEmployment != null)
            {                
                if (ibusCurrentEmployment.ibusOrganization == null)
                {
                    // If no active employment exists, load the last employment.
                    if (icolPersonEmployment == null)
                        LoadPersonEmployment();
                    if (icolPersonEmployment.Count > 0)
                        ibusCurrentEmployment = icolPersonEmployment[0];
                    ibusCurrentEmployment.LoadOrganization();
                }
            }
            return ibusCurrentEmployment.ibusOrganization;
        }

        /// <summary>
        /// this method is used to check the validation on
        /// clicking new button in the Employment history tab
        /// </summary>
        /// <param name="ahstParam"></param>
        /// <returns></returns>
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;

            if (((string)ahstParam["iblnIsValidateNewTrue"]) == "true")
            {
                DataTable ldtbAddressList = Select<cdoPersonAddress>(
                    new string[1] { "person_id" },
                    new object[1] { ahstParam["person_id"] }, null, null);

                busPerson lobjPerson = new busPerson();
                lobjPerson.FindPerson(Convert.ToInt32(ahstParam["person_id"]));

                if ((lobjPerson.icdoPerson.ssn == null)
                            || (lobjPerson.icdoPerson.date_of_birth == null)
                            || (lobjPerson.icdoPerson.gender_value == null)
                            || (lobjPerson.icdoPerson.marital_status_value == null)
                            || (ldtbAddressList.Rows.Count == 0))
                {
                    lobjError = AddError(1092, "");
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }
            }

            //PIR-15272
            if (((string)ahstParam["iblnIsValidAddress"]) == "true")
            {
                DataTable ldtbAddressList = Select<cdoPersonAddress>(
                   new string[1] { "person_id" },
                   new object[1] { ahstParam["aint_person_id"] }, null, null);
                if (ldtbAddressList.Rows.Count == 0)
                {
                    lobjError = AddError(10277, "");
                    larrErrors.Add(lobjError);
                    return larrErrors;
                }
            }
            return larrErrors;
        }


        //PIR - 329
        // To get the PERSLink ID of the current logged in user.       

        public void LoadUser()
        {
            if (_ibusUser == null)
            {
                _ibusUser = new busUser();
            }
            _ibusUser.FindUser(iobjPassInfo.iintUserSerialID);
        }

        //PIR - 346
        public void LoadNotes()
        {
            DataTable ldtbNotesPerson = busNeoSpinBase.Select("cdoNotes.PersonLookup",
                           new object[3] { "GENR", 0, _icdoPerson.person_id });
            _iclbNotes = GetCollection<busNotes>(ldtbNotesPerson, "icdoNotes");
        }

        public override void BeforePersistChanges()
        {
            // temp_ssn is used since the ssn value is not updating while saving.
            // comparing the old values is checked since, the null value is saving for ssn when non accessible users modify person record
            if (icdoPerson.ienuObjectState != ObjectState.Insert)
            {
                if (icdoPerson.ihstOldValues.Count > 0)
                {
                    if (Convert.ToString(icdoPerson.ihstOldValues["ssn"]) != icdoPerson.temp_ssn)
                        icdoPerson.ssn = icdoPerson.temp_ssn;
                    //UCS - 079 : checking conditions and intiating Pop up BenefitWorkflow
                    CheckConditionsForWorkflow();
                    //UCS - 079 : checking whether date of birth is changed and initiate Recalculate Pension workflow
                    CheckDateOfBirthChange();

                    istrOldMarriedStatus = Convert.ToString(icdoPerson.ihstOldValues["marital_status_value"]);
                }
            }
            //PIR 18492-When from screen account locked is unchecked also reset failed count to zero
            if (icdoPerson.is_user_locked =="N" && icdoPerson.failed_login_attempt_count >= 6)
            {
                icdoPerson.failed_login_attempt_count = 0;
            }
            //PIR 347
            if (!(String.IsNullOrEmpty(_icdoPerson.peoplesoft_id)))
            {
                _icdoPerson.peoplesoft_id = _icdoPerson.peoplesoft_id.PadLeft(7, '0');
            }
            //UCS - 057 - BR - 25,26
            if (icdoPerson.restriction_flag == busConstant.Flag_Yes)
            {
                icdoPerson.restricted_by = iobjPassInfo.istrUserID;
                icdoPerson.restricted_date = DateTime.Today;
            }
            else
            {
                if ((icdoPerson.ihstOldValues.Count > 0) &&
                    (icdoPerson.ihstOldValues["restriction_flag"] != null && icdoPerson.ihstOldValues["restriction_flag"].ToString() == busConstant.Flag_Yes))
                {
                    if (icdoPerson.restriction_flag == busConstant.Flag_No)
                    {
                        icdoPerson.unrestricted_by = iobjPassInfo.istrUserID;
                        icdoPerson.restricted_by = string.Empty;
                        icdoPerson.restricted_date = DateTime.MinValue;
                    }
                }
            }

            //UCS 55 Rhic Combining BR-055-149
            if ((icdoPerson.ihstOldValues.Count > 0) &&
                (icdoPerson.ihstOldValues["marital_status_value"] != null && icdoPerson.ihstOldValues["marital_status_value"].ToString() == busConstant.PersonMaritalStatusMarried
                && (icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle || icdoPerson.marital_status_value == busConstant.PersonMaritalStatusWidow
                || icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced)))
            {

                if (ibusLatestBenefitRhicCombine == null)
                    LoadLatestBenefitRhicCombine();

                if (ibusLatestBenefitRhicCombine != null)
                {
                    //PROD PIR : 4047 Ref Raj Mail dated on 12/29/2010
                    //Initiate Worflow only if spouse is donating RHIC
                    if (ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail == null)
                        ibusLatestBenefitRhicCombine.LoadBenefitRhicCombineDetails();

                    bool lblnSpouseDonorFound = false;
                    foreach (var lbusBenRhicCombineDetail in ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail)
                    {
                        if (lbusBenRhicCombineDetail.ibusPayeeAccount == null)
                            lbusBenRhicCombineDetail.LoadPayeeAccount();
                        if (lbusBenRhicCombineDetail.ibusPayeeAccount.icdoPayeeAccount.payee_perslink_id != icdoPerson.person_id)
                        {
                            lblnSpouseDonorFound = true;
                            break;
                        }
                    }

                    if (lblnSpouseDonorFound)
                        ibusLatestBenefitRhicCombine.InitiateRHICCombineWorkflow();
                }
            }
            //pir 7105
            // Needs to set flag when a PERSON, whose PERSON Type is "Member" or "Retiree", or "Payee" when the Marital Status value is changed to "Divorced"
            // Only need to generate letter when the person has Retirement Plan (NOT Withdrawn or Cancelled) or a Life plan (Enrolled only).
            if (IsMember() || IsRetiree() || IsPayee())
            {
                //PIR 24927 - Update ms_change_batch_flag to Y if there is change in martital status. Skip members with DOD.
                if ((icdoPerson.ihstOldValues.Count > 0) &&
                    (icdoPerson.ihstOldValues["marital_status_value"] != null && icdoPerson.ihstOldValues["marital_status_value"].ToString() != icdoPerson.marital_status_value) &&
                    icdoPerson.date_of_death == DateTime.MinValue && icdoPerson.marital_status_value != busConstant.PersonMaritalStatusWidow)
                {
                    //PIR 21454 system-generated letter for marital status changes doesn't generate in all instances //ms_change_batch_flag should update from MSS as well
                    UpdateMSChangeBatchFlag();
                }
            }
            // pir 7232 Trim the leading and trailing spaces before saving
            //PIR 11071
            if (!icdoPerson.first_name.IsNullOrEmpty())
                icdoPerson.first_name = icdoPerson.first_name.Trim();
            if (!icdoPerson.last_name.IsNullOrEmpty())
                 icdoPerson.last_name = icdoPerson.last_name.Trim();
            if (!icdoPerson.middle_name.IsNullOrEmpty())
                icdoPerson.middle_name = icdoPerson.middle_name.Trim();

            //PIR-15430-always inserts new address and ends the previous address.
            if (iblnIsUpdateAddressClicked)
            {
                ibusPersonCurrentAddress.UpdatePreviousAddressOfCurrentTypeForESS();
                ibusPersonCurrentAddress.icdoPersonAddress.addr_country_value = "0001"; //PIR- 16310
                ibusPersonCurrentAddress.icdoPersonAddress.created_by = iobjPassInfo.istrUserID;
                ibusPersonCurrentAddress.icdoPersonAddress.modified_by = iobjPassInfo.istrUserID;
                ibusPersonCurrentAddress.icdoPersonAddress.created_date = DateTime.Now;
                ibusPersonCurrentAddress.icdoPersonAddress.modified_date = DateTime.Now;               
                ibusPersonCurrentAddress.icdoPersonAddress.Insert();                
            }
            //ESS Update Address Enable
            iblnIsUpdateAddressClicked = false;
            base.BeforePersistChanges();
        }

        public override int PersistChanges()
        {
            UpdateMedicarePartDFlags();
            return base.PersistChanges();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            icdoPerson.temp_ssn = icdoPerson.ssn;
            if (icdoPerson.restriction_flag == busConstant.Flag_Yes)
            {
                if (iclbPayeeAccount == null)
                    LoadPayeeAccount();
                foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
                {
                    if (lobjPayeeAccount.ibusSoftErrors == null)
                        lobjPayeeAccount.LoadErrors();
                    lobjPayeeAccount.iblnClearSoftErrors = false;
                    lobjPayeeAccount.ibusSoftErrors.iblnClearError = false;
                    lobjPayeeAccount.iblnPayeeRestrictedIndicator = true;
                    lobjPayeeAccount.CreateReviewPayeeAccountStatus();
                    lobjPayeeAccount.ValidateSoftErrors();
                    lobjPayeeAccount.UpdateValidateStatus();
                }
            }
        }

        // Person Extract from Legacy - Inbound File
        private string _istrPrevSSN;
        public string istrPrevSSN
        {
            get { return _istrPrevSSN; }
            set { _istrPrevSSN = value; }
        }

        // Person Extract from Legacy - Inbound File
        private string _istrFirstandMiddleName;
        public string istrFirstandMiddleName
        {
            get { return _istrFirstandMiddleName; }
            set { _istrFirstandMiddleName = value; }
        }

        #region STC-021 PIR 70

        private Collection<busPersonBeneficiary> _iclbBeneficiaryTo;
        public Collection<busPersonBeneficiary> iclbBeneficiaryTo
        {
            get { return _iclbBeneficiaryTo; }
            set { _iclbBeneficiaryTo = value; }
        }

        private Collection<busPerson> _iclbDependentTo;
        public Collection<busPerson> iclbDependentTo
        {
            get { return _iclbDependentTo; }
            set { _iclbDependentTo = value; }
        }

        private Collection<busPerson> _iclbContactTo;
        public Collection<busPerson> iclbContactTo
        {
            get { return _iclbContactTo; }
            set { _iclbContactTo = value; }
        }

        //concept for loading beneficary is changed as per PIR = 1863
        //ref - New method with same name is written
        //public void LoadBeneficiaryTo()
        //{
        //    DataTable ldtbBeneficiaryTo = Select("cdoPerson.LoadPersonAsBeneficiary", new object[1] { _icdoPerson.person_id });
        //    _iclbBeneficiaryTo = GetCollection<busPerson>(ldtbBeneficiaryTo, "icdoPerson");
        //}

        public void LoadDependentTo()
        {
            DataTable ldtbDependentTo = Select("cdoPerson.LoadPersonAsDependent", new object[1] { _icdoPerson.person_id });
            _iclbDependentTo = GetCollection<busPerson>(ldtbDependentTo, "icdoPerson");
        }

        public void LoadContactTo()
        {
            DataTable ldtbContactTo = Select("cdoPerson.LoadPersonAsContact", new object[1] { _icdoPerson.person_id });
            _iclbContactTo = GetCollection<busPerson>(ldtbContactTo, "icdoPerson");
        }

        #endregion

        // Correspondence PER-0051
        public void UpdateMSChangeBatchFlag()
        {            
            if (iclbActivePensionAccounts == null)
                LoadActivePlanDetails();
            if (iclbActiveInsuranceAccounts == null)
                LoadInsuranceSummary();
            if ((iclbActivePensionAccounts.IsNotNull()
                && iclbActivePensionAccounts.Any(i => i.ibusPlan.IsRetirementPlan() && i.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn))
                || (iclbActiveInsuranceAccounts.IsNotNull()
                && iclbActiveInsuranceAccounts.Any(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife && i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)))
            {
                busCorTracking lobjCorTracking = new busCorTracking { icdoCorTracking = new cdoCorTracking() };
                DataTable ldtbCorTracking = Select("entCorTracking.GetCorrTrackPersonTemplate", new object[2] { icdoPerson.person_id, busConstant.TemplateID_PER0055 });

                if (ldtbCorTracking.Rows.Count > 0)
                {
                    lobjCorTracking.icdoCorTracking.LoadData(ldtbCorTracking.Rows[0]);
                    if (lobjCorTracking.icdoCorTracking.IsNotNull() && 
                        busGlobalFunctions.DateDiffInDays(lobjCorTracking.icdoCorTracking.generated_date , busGlobalFunctions.GetSysManagementBatchDate()) >= 6)
                    {
                        icdoPerson.ms_change_batch_flag = busConstant.Flag_Yes;
                    }
                }//New member - No corr generated ever for the member, set the flag to Y. 
                else
                    icdoPerson.ms_change_batch_flag = busConstant.Flag_Yes;
            }
        }
        public bool IsPOAExists
        {
            get
            {
                int lintCount = 0;
                lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonContact.Check_If_POA_Exists", new object[1] { _icdoPerson.person_id },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                if (lintCount > 0)
                    return true;
                else
                    return false;
            }
        }

        private Collection<busServicePurchaseHeader> _iclbServicePurchaseHeader;
        public Collection<busServicePurchaseHeader> iclbServicePurchaseHeader
        {
            get { return _iclbServicePurchaseHeader; }
            set { _iclbServicePurchaseHeader = value; }
        }

        public void LoadServicePurchase()
        {
            LoadServicePurchase(true);
        }

        public void LoadServicePurchase(bool ablnLoadOtherObjects)
        {
            DataTable ldtbServicePurchaseHeader = Select<cdoServicePurchaseHeader>(
                       new string[1] { "person_id" },
                       new object[1] { _icdoPerson.person_id }, null, null);
            _iclbServicePurchaseHeader = GetCollection<busServicePurchaseHeader>(ldtbServicePurchaseHeader, "icdoServicePurchaseHeader");
            if (ablnLoadOtherObjects)
            {
                foreach (busServicePurchaseHeader lobjServicePurchase in _iclbServicePurchaseHeader)
                {
                    lobjServicePurchase.LoadPlan();
                    lobjServicePurchase.LoadServicePurchaseDetail();
                    //To Load the Payoff Amount
                    lobjServicePurchase.LoadAmortizationSchedule();
                }
            }
        }

        #region Insurance Plan Enrollment - Button Visibility

        private bool _iblnHealth;
        public bool iblnHealth
        {
            get { return _iblnHealth; }
            set { _iblnHealth = value; }
        }

        private bool _iblnMedicarePartD;
        public bool iblnMedicarePartD
        {
            get { return _iblnMedicarePartD; }
            set { _iblnMedicarePartD = value; }
        }

        private bool _iblnDental;
        public bool iblnDental
        {
            get { return _iblnDental; }
            set { _iblnDental = value; }
        }

        private bool _iblnVision;
        public bool iblnVision
        {
            get { return _iblnVision; }
            set { _iblnVision = value; }
        }

        private bool _iblnLife;
        public bool iblnLife
        {
            get { return _iblnLife; }
            set { _iblnLife = value; }
        }

        private bool _iblnFlex;
        public bool iblnFlex
        {
            get { return _iblnFlex; }
            set { _iblnFlex = value; }
        }

        private bool _iblnLTC;
        public bool iblnLTC
        {
            get { return _iblnLTC; }
            set { _iblnLTC = value; }
        }

        public bool iblnGHDV { get; set; }

        public bool iblnEAP { get; set; }

        public bool iblnDefComp { get; set; }

        public bool iblnOth457Plan { get; set; }

        public bool iblnRetirement { get; set; }

        //Revisited GHDV Doc : Open the Enrollment to Everyone as long as there is enrollment record in plan
        public void LoadPlanEnrollmentVisibility()
        {
            _iblnHealth = true;
            _iblnMedicarePartD = true;
            _iblnDental = true;
            _iblnVision = true;
            _iblnLife = true;
            //UAT PIR: 1013 For Flex it should not be visible for retirees.
            _iblnFlex = false;
            _iblnLTC = true;

            if (IsMemberEnrolledInPlan(busConstant.PlanIdGroupHealth))
                _iblnHealth = false;
            if (IsMemberEnrolledInPlan(busConstant.PlanIdMedicarePartD))
                _iblnMedicarePartD = false;
            if (IsMemberEnrolledInPlan(busConstant.PlanIdDental))
                _iblnDental = false;
            if (IsMemberEnrolledInPlan(busConstant.PlanIdVision))
                _iblnVision = false;
            if (IsMemberEnrolledInPlan(busConstant.PlanIdGroupLife))
                _iblnLife = false;
            //UAT PIR: 1013 For Flex it should not be visible for retirees.
            /*if (IsMemberEnrolledInPlan(busConstant.PlanIdFlex))
                _iblnFlex = false;*/
            if (IsMemberEnrolledInPlan(busConstant.PlanIdLTC))
                _iblnLTC = false;
        }

        #endregion

        public int iintPlanIDDental
        {
            get { return busConstant.PlanIdDental; }
        }

        public int iintPlanIdVision
        {
            get { return busConstant.PlanIdVision; }
        }

        public int iintPlanIdGroupHealth
        {
            get { return busConstant.PlanIdGroupHealth; }
        }

        public int iintPlanIdHMO
        {
            get { return busConstant.PlanIdHMO; }
        }

        public int iintPlanidMedicare
        {
            get { return busConstant.PlanIdMedicarePartD; }
        }

        public bool IsPersonInHMO()
        {
            return IsMemberEnrolledInPlan(busConstant.PlanIdHMO);
        }

        public bool IsPersonInGroupHealth()
        {
            return IsMemberEnrolledInPlan(busConstant.PlanIdGroupHealth);
        }
        public bool IsPersonInMedicarePartd()
        {
            return IsMemberEnrolledInPlan(busConstant.PlanIdMedicarePartD);
        }
        public bool IsPersonInDental()
        {
            return IsMemberEnrolledInPlan(busConstant.PlanIdDental);
        }
        public bool IsPersonInVision()
        {
            return IsMemberEnrolledInPlan(busConstant.PlanIdVision);
        }
        public bool IsPersonInDeferredComp()
        {
            return IsMemberEnrolledInPlan(busConstant.PlanIdDeferredCompensation);
        }
        public bool IsPersonInFlex()
        {
            return IsMemberEnrolledInPlan(busConstant.PlanIdFlex);
        }


        protected Collection<busPersonAccount> _iclbRetirementAccount;
        public Collection<busPersonAccount> iclbRetirementAccount
        {
            get
            {
                return _iclbRetirementAccount;
            }
        }

        public void LoadRetirementAccount()
        {
            if (_iclbRetirementAccount == null)
            {
                _iclbRetirementAccount = new Collection<busPersonAccount>();
                if (_icolPersonAccount == null)
                    LoadPersonAccount();
                foreach (busPersonAccount lbusPersonAccount in _icolPersonAccount)
                {
                    if (lbusPersonAccount.ibusPlan == null)
                        lbusPersonAccount.LoadPlan();

                    if (lbusPersonAccount.ibusPlan.IsRetirementPlan())
                        _iclbRetirementAccount.Add(lbusPersonAccount);
                }
            }
        }

        public Collection<busPersonAccount> iclbInsuranceAccounts { get; set; }

        public void LoadInsuranceAccounts()
        {
            if (iclbInsuranceAccounts.IsNull())
                iclbInsuranceAccounts = new Collection<busPersonAccount>();
            if (_icolPersonAccount == null)
                LoadPersonAccount();
            foreach (busPersonAccount lbusPersonAccount in _icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();

                if (lbusPersonAccount.ibusPlan.IsInsurancePlan())
                    iclbInsuranceAccounts.Add(lbusPersonAccount);
            }
        }

        /// This Collection has the list of all Def Comp Accounts the Member has irrespective of the Plan Participation Status.
        public Collection<busPersonAccount> iclbDefCompAccounts { get; set; }

        public void LoadDefCompAccounts()
        {
            if (iclbDefCompAccounts.IsNull())
                iclbDefCompAccounts = new Collection<busPersonAccount>();
            if (_icolPersonAccount == null)
                LoadPersonAccount();
            foreach (busPersonAccount lbusPersonAccount in _icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();

                if (lbusPersonAccount.ibusPlan.IsDeferredCompPlan())
                    iclbDefCompAccounts.Add(lbusPersonAccount);
            }
        }

        public bool IsRetiree()
        {
            bool lblnResult = false;

            if (_iclbRetirementAccount == null)
                LoadRetirementAccount();

            foreach (busPersonAccount lobjPersonAccount in _iclbRetirementAccount)
            {
                if ((DateTime.Now >= lobjPersonAccount.icdoPersonAccount.history_change_date_no_null) &&
                    (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired))
                {
                    lblnResult = true;
                    break;
                }
            }

            return lblnResult;
        }

        /// <summary>
        /// This Method will check whether this person is eligible for Depedent Cobra for Given Plan and Effective Date
        /// </summary>
        /// <param name="aintPlanID"></param>
        /// <param name="adtEffectiveDate"></param>
        /// <returns></returns>
        public bool IsDependentCobra(int aintPlanID, DateTime adtEffectiveDate)
        {
            var lcdoPersonAccount = new cdoPersonAccount();
            return IsDependentCobra(aintPlanID, adtEffectiveDate, ref lcdoPersonAccount);
        }

        public bool IsDependentCobra(int aintPlanID, DateTime adtEffectiveDate, ref cdoPersonAccount acdoMemberPersonAccount, int aintMemberPersonAccountID = 0)
        {
            if (iclbPersonDependentByDependent == null)
                LoadPersonDependentByDependent();

            int lintCounter = 1;
            foreach (busPersonDependent lbusPersonDependent in iclbPersonDependentByDependent)
            {
                if (lbusPersonDependent.iclbPersonAccountDependent == null)
                    lbusPersonDependent.LoadPersonAccountDependent();

                foreach (var lbusPersonAccountDependent in lbusPersonDependent.iclbPersonAccountDependent)
                {
                    if (lbusPersonAccountDependent.ibusPersonAccount == null)
                        lbusPersonAccountDependent.LoadPersonAccount();

                    if ((lbusPersonAccountDependent.icdoPersonAccountDependent.end_date_no_null < adtEffectiveDate) &&
                       (lbusPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == aintPlanID))
                    {
                        //PIR 22945 - If 'Dependent of' is selected then load that members person account.
                        if (lbusPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.person_account_id == aintMemberPersonAccountID)
                        {
                            acdoMemberPersonAccount = lbusPersonAccountDependent.ibusPersonAccount.icdoPersonAccount;
                            return true;
                        }
                        else if (iclbPersonDependentByDependent.Count() == lintCounter)
                        {
                            acdoMemberPersonAccount = lbusPersonAccountDependent.ibusPersonAccount.icdoPersonAccount;
                            return true;
                        }
                    }
                }
                lintCounter++;
            }
            return false;
        }

        public bool IsPayee()
        {
            bool lblnResult = false;
            if (iclbPayeeAccount == null)
                LoadPayeeAccount(true);

            var lintCount = from PA in iclbPayeeAccount select PA.ibusPayeeAccountActiveStatus.IsStatusNotProcessed();

            if (lintCount.Count() > 0)
                lblnResult = true;
            return lblnResult;
        }

        public busPersonEmployment ibusCurrentEmployment { get; set; }

        /// <summary>
        /// Loads the Current Employer as on Today's Date.
        /// </summary>
        public void LoadCurrentEmployer()
        {
            LoadCurrentEmployer(DateTime.Now);
        }

        /// <summary>
        /// Loads the Current Employer as on Given Date.
        /// </summary>
        /// <param name="adteGivenDate"></param>
        public void LoadCurrentEmployer(DateTime adteGivenDate)
        {
            if (ibusCurrentEmployment == null)
            {
                ibusCurrentEmployment = new busPersonEmployment();
                ibusCurrentEmployment.icdoPersonEmployment = new cdoPersonEmployment();
            }

            if (icolPersonEmployment == null)
                LoadPersonEmployment();

            var lvarEmp = from org in icolPersonEmployment.AsEnumerable()
                          where busGlobalFunctions.CheckDateOverlapping(adteGivenDate,
                          org.icdoPersonEmployment.start_date, org.icdoPersonEmployment.end_date)
                          select org;

            if (lvarEmp.Count() > 0)
            {
                ibusCurrentEmployment = (busPersonEmployment)lvarEmp.First();
                ibusCurrentEmployment.LoadOrganization();
            }
        }

        /// <summary>
        /// Load the Current Employer by given Plan.
        /// </summary>
        /// <param name="aintPlanID"></param>
        public void LoadCurrentEmployer(int aintPlanID)
        {
            if (_icolEnrolledPersonAccountEmploymentDetail == null)
                LoadPersonAccountEmploymentDetailEnrolled();

            Collection<busPersonAccountEmploymentDetail> lclbPAEmpDtl = new Collection<busPersonAccountEmploymentDetail>();
            foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in _icolEnrolledPersonAccountEmploymentDetail)
            {
                if (lobjPAEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == aintPlanID)
                {
                    lobjPAEmpDtl.LoadPersonEmploymentDetail();
                    lclbPAEmpDtl.Add(lobjPAEmpDtl);
                }
            }

            var lvar = from EmpDtl in lclbPAEmpDtl.AsEnumerable()
                       orderby EmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date
                       select EmpDtl;

            int lintPersonEmploymentID = 0;
            if (lvar.Count() > 0)
                lintPersonEmploymentID = ((busPersonAccountEmploymentDetail)lvar.First()).ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id;

            if (ibusCurrentEmployment == null)
                ibusCurrentEmployment = new busPersonEmployment();
            ibusCurrentEmployment.FindPersonEmployment(lintPersonEmploymentID);
            ibusCurrentEmployment.LoadOrganization();
        }

        // this property is used to get total PSC and used in correpondence
        //changing this property will reflect the value in correspondence       
        public decimal GetTotalPSC(DateTime adtDateToCompare)
        {
            decimal ldecTotalPSC = 0.00M;
            if (_icolPersonAccountByBenefitType == null)
                LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeRetirement);
            foreach (busPersonAccount lobjPersonAccount in _icolPersonAccountByBenefitType)
            {
                if (adtDateToCompare == DateTime.MinValue)
                {
                    if (lobjPersonAccount.iclbRetirementContributionAll == null)
                        lobjPersonAccount.LoadRetirementContributionAll();
                    foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lobjPersonAccount.iclbRetirementContributionAll)
                    {
                        ldecTotalPSC += lobjRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit;
                    }
                }
                else
                {
                    if (lobjPersonAccount.iclbRetirementContributionAllAsOfDate == null)
                        lobjPersonAccount.LoadRetirementContributionByDate(null, adtDateToCompare);
                    foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lobjPersonAccount.iclbRetirementContributionAllAsOfDate)
                    {
                        ldecTotalPSC += lobjRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit;
                    }
                }
            }
            return ldecTotalPSC;
        }
        //UAT PIR:950. Do not Include the TFFR Service for PSC calculation.Include Tentative TFFR and TIAA only when it is Estimate for VSC calculation.
        public decimal GetTotalVSCForPerson(bool iblnIsPlanJobService, DateTime adtToCompare, bool alblnNullCheckContribution = true)
        {
            return GetTotalVSCForPerson(iblnIsPlanJobService, adtToCompare, false, alblnNullCheckContribution);
        }

        public decimal idecVSCForSelectedPlan { get; set; }
        public decimal idecVSCForOtherPlan { get; set; }

        //Get Total VSC for All Person Account for the Person with benefit Type Retirement
        //first load all the Person Account for the person id
        //loop thru the collection 
        //inside collection load Retirement Contribution for the Person account adn get sum of VSC 
        //plus add the TFFR and TIAA for plan Other than Job Service.
        //For  job service sum only vsc for that plan only
        //date parameter is added to this method in order to compare and get only those records 
        //that are below the adtToCompare :: Deepa
        //UAT PIR:950. Do not Include the TFFR Service for PSC calculation.Include Tentative TFFR and TIAA only when it is Estimate for VSC calculation.
        public decimal GetTotalVSCForPerson(bool iblnIsPlanJobService, DateTime adtToCompare, bool ablnIncludeTentativeTFFRService,
                                                                    bool alblnNullCheckContribution, bool ablnIsDROEstimate = false, bool ablnIsFromPensionFile = false, int iintBenefitPlanId = 0)
        {
            decimal ldecTotalVSC = 0.00M;
            //Withdrawn accounts excluded here
            if (_icolPersonAccountByBenefitType == null)
                LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeRetirement, true, ablnIsFromPensionFile);//PIR 17486 - Issue 1
            //PIR 18592 - Withdrawn and transferred to DC have already been excluded in the above method, 
            //we need to also exclude TICR, TRDB, TRTF accounts 
            Collection<busPersonAccount> lcolPersonAccountByBenefitTypeExcludingTransferredAccounts =
                _icolPersonAccountByBenefitType
                .Where(pa => pa.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementTransferredTIAACREF &&
                            pa.icdoPersonAccount.plan_participation_status_value != busConstant.RetirementPlanParticipationStatusTranToDb && 
                            pa.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusTransferToTFFR)
                .ToList()
                .ToCollection();
            foreach (busPersonAccount lobjPersonAccount in lcolPersonAccountByBenefitTypeExcludingTransferredAccounts)
            {
                if (!iblnIsPlanJobService)
                {
                    if (lobjPersonAccount.icdoPersonAccount.plan_id != busConstant.PlanIdJobService)
                    {
                        if (adtToCompare == DateTime.MinValue)
                        {
                            if (alblnNullCheckContribution)
                            {
                                if ((lobjPersonAccount.iclbRetirementContributionAll == null)
                                || (lobjPersonAccount.iclbRetirementContributionAll.Count == 0))
                                    lobjPersonAccount.LoadRetirementContributionAll();
                            }
                            else
                            {
                                //for refreshing the datatable if the contribution needs to be reloaded, datatable is assigned as null
                                lobjPersonAccount.idtRetirementContributionAll = null;
                                lobjPersonAccount.LoadRetirementContributionAll();
                            }

                            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lobjPersonAccount.iclbRetirementContributionAll)
                            {
                                //this filteraion is done in order to avoid negative value for subsystem type benefit Payment
                                if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value != busConstant.SubSystemValueBenefitPayment)
                                {
                                    if (lobjPersonAccount.icdoPersonAccount.plan_id > 0 && lobjPersonAccount.icdoPersonAccount.plan_id == iintBenefitPlanId)
                                    {
                                        idecVSCForSelectedPlan += lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit;
                                    }
                                    else
                                    {
                                        idecVSCForOtherPlan += lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit;
                                    }
                                    ldecTotalVSC += lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit;
                                }
                            }
                        }
                        else
                        {
                            //if ((lobjPersonAccount.iclbRetirementContributionAllAsOfDate == null)
                            //    || (lobjPersonAccount.iclbRetirementContributionAllAsOfDate.Count == 0))
                            //no need to check null as it has to be realoaded every time when the date to compare is changed
                            lobjPersonAccount.LoadRetirementContributionByDate(null, adtToCompare, ablnIsDROEstimate: ablnIsDROEstimate);
                            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lobjPersonAccount.iclbRetirementContributionAllAsOfDate)
                            {
                                if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value != busConstant.SubSystemValueBenefitPayment)
                                {
                                    if (lobjPersonAccount.icdoPersonAccount.plan_id > 0 && lobjPersonAccount.icdoPersonAccount.plan_id == iintBenefitPlanId)
                                    {
                                        idecVSCForSelectedPlan += lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit;
                                    }
                                    else
                                    {
                                        idecVSCForOtherPlan += lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit;
                                    }
                                    ldecTotalVSC += lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdJobService)
                    {
                        if (adtToCompare == DateTime.MinValue)
                        {
                            if ((lobjPersonAccount.iclbRetirementContributionAll == null)
                                || (lobjPersonAccount.iclbRetirementContributionAll.Count == 0))
                                lobjPersonAccount.LoadRetirementContributionAll();

                            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lobjPersonAccount.iclbRetirementContributionAll)
                            {
                                ldecTotalVSC += lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit;
                            }
                        }
                        else
                        {
                            if ((lobjPersonAccount.iclbRetirementContributionAllAsOfDate == null)
                               || (lobjPersonAccount.iclbRetirementContributionAllAsOfDate.Count == 0))
                                //no need to check null as it has to be realoaded every time when the date to compare is changed
                                lobjPersonAccount.LoadRetirementContributionByDate(null, adtToCompare, ablnIsDROEstimate: ablnIsDROEstimate);
                            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lobjPersonAccount.iclbRetirementContributionAllAsOfDate)
                            {
                                ldecTotalVSC += lobjRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit;
                            }
                        }
                    }
                }
            }
            //PIR 18592 - TFFR/TIAA service should only be considered once; as this below block was in foreach loop before, TFFR/TIAA 
            //serivce was getting added as many times as the retirement plan accounts for the person
            if (!iblnIsPlanJobService)
            {
                decimal ldecTFFRService = 0.00M;
                decimal ldecTIAAService = 0.00M;
                decimal ldecTentativeTFFRService = 0.00M;
                decimal ldecTentativeTIAAService = 0.00M;
                busPersonAccount lobjPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount() { person_id = icdoPerson.person_id } };
                lobjPersonAccount.LoadTFFRTIAAService(ref ldecTFFRService, ref ldecTIAAService, ref ldecTentativeTFFRService, ref ldecTentativeTIAAService);
                if (ablnIncludeTentativeTFFRService)
                {
                    ldecTotalVSC = ldecTotalVSC + ldecTFFRService + ldecTIAAService + ldecTentativeTFFRService + ldecTentativeTIAAService;
                }
                else
                {
                    ldecTotalVSC = ldecTotalVSC + ldecTFFRService + ldecTIAAService;
                }
            }
            return ldecTotalVSC;
        }

        //Load All Benefit Applications for this Person

        public void LoadBenefitApplication()
        {
            if (_iclbBenefitApplication == null)
                _iclbBenefitApplication = new Collection<busBenefitApplication>();
            DataTable ldtbBenefitApplication = Select<cdoBenefitApplication>(new string[1] { "member_person_id" }, new object[1] { icdoPerson.person_id }, null, null);
            _iclbBenefitApplication = GetCollection<busBenefitApplication>(ldtbBenefitApplication, "icdoBenefitApplication");
            foreach (busBenefitApplication lobjBenefitApplication in iclbBenefitApplication)
            {
                lobjBenefitApplication.LoadRecipient();
                if (lobjBenefitApplication.ibusPlan == null)
                    lobjBenefitApplication.LoadPlan();
            }
        }

        // pir - 1284 Load application for recipient person id also 
        public Collection<busBenefitApplication> iclbApplicantsBenefitApplications { get; set; }
        public void LoadApplicantsbenefitApplication()
        {
            if (iclbBenefitApplication == null)
                LoadBenefitApplication();

            Collection<busBenefitApplication> lclbBenefitApplication = new Collection<busBenefitApplication>();
            iclbApplicantsBenefitApplications = new Collection<busBenefitApplication>();
            DataTable ldtbBenefitApplication = Select<cdoBenefitApplication>(new string[1] { "recipient_person_id" }, new object[1] { icdoPerson.person_id }, null, null);
            lclbBenefitApplication = GetCollection<busBenefitApplication>(ldtbBenefitApplication, "icdoBenefitApplication");

            foreach (busBenefitApplication lobjBenefitApplication in lclbBenefitApplication)
            {
                if ((lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                    || (lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath))
                {
                    if (lobjBenefitApplication.icdoBenefitApplication.recipient_person_id == icdoPerson.person_id)
                    {
                        lobjBenefitApplication.ibusRecipient = this;
                        if (lobjBenefitApplication.ibusPlan == null)
                            lobjBenefitApplication.LoadPlan();
                        iclbBenefitApplication.Add(lobjBenefitApplication);

                        iclbApplicantsBenefitApplications.Add(lobjBenefitApplication);
                    }
                }
            }
        }

        //Return Org Plan id for a PlanId
        public int LoadDefaultOrgPlanIdByPlanId(int aintPlanid)
        {
            return LoadDefaultOrgPlanIdByPlanId(aintPlanid, DateTime.Now);
        }

        // PROD PIR ID 7066
        public int LoadDefaultOrgPlanIdByPlanId(int aintPlanid, DateTime? adteGivenDate)
        {
            int lintOrgPlanID = 0;
            Collection<busOrgPlan> lclbProviderOrgPlanForCobaRetiree = new Collection<busOrgPlan>();
            DataTable ldtbProviderOrgPlan = Select("cdoPersonAccount.LoadProviderOrgPlanIfEmploymentNotExists", new object[1] { aintPlanid });
            lclbProviderOrgPlanForCobaRetiree = GetCollection<busOrgPlan>(ldtbProviderOrgPlan, "icdoOrgPlan");
            foreach (busOrgPlan lobjOrgPlan in lclbProviderOrgPlanForCobaRetiree)
            {
                if (busGlobalFunctions.CheckDateOverlapping(adteGivenDate ?? DateTime.Now, lobjOrgPlan.icdoOrgPlan.participation_start_date,
                    lobjOrgPlan.icdoOrgPlan.participation_end_date))
                {
                    lintOrgPlanID = lobjOrgPlan.icdoOrgPlan.org_plan_id;
                    break;
                }
            }
            return lintOrgPlanID;
        }

        //Load Plan tab in death notification
        //Load Person Account and loop thru
        //if plan is retirement or Deferred compensation  then check plan is not end dated.
        //for insurance check if the plan is not cancelled.
        public void LoadActivePlanDetails()
        {
            _iclbActivePensionAccounts = new Collection<busPersonAccount>();

            if (icolPersonAccount == null)
                LoadPersonAccount();

            foreach (busPersonAccount lobjPersonAccount in _icolPersonAccount)
            {
                lobjPersonAccount.LoadPlan();
                if ((lobjPersonAccount.ibusPlan.IsRetirementPlan())
                    || (lobjPersonAccount.ibusPlan.icdoPlan.benefit_type_value == "DEFF"))
                {
                    if (lobjPersonAccount.icdoPersonAccount.end_date == DateTime.MinValue)
                    {
                        _iclbActivePensionAccounts.Add(lobjPersonAccount);
                    }
                }
                if (lobjPersonAccount.ibusPlan.IsInsurancePlan()
                    || lobjPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeFlex) // UAT PIR 2206 display flex comp plan also
                {
                    if (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceCancelled)
                    {
                        _iclbActivePensionAccounts.Add(lobjPersonAccount);
                    }
                }
            }
        }

        //BR-053-04
        //Load beneficiary for Deceased
        //loop thru and Load all person Account data for the beneficiary.
        //Load Person Account in order to load Plan.
        //Check if Plan is retirement or Life plan. 
        //if yes then add to the collection that will displayed in the death notification screen.
        public Collection<busPersonBeneficiary> iclbPersonPlanBeneficiaryForDeceased { get; set; }

        public void LoadBeneficiaryForDeceased()
        {
            if (iclbPersonBeneficiary == null)
                LoadBeneficiary();

            _iclbPersonBeneficiaryForDeceased = new Collection<busPersonBeneficiary>();
            iclbPersonPlanBeneficiaryForDeceased = new Collection<busPersonBeneficiary>();

            foreach (busPersonBeneficiary lobjPersonBeneficiary in iclbPersonBeneficiary)
            {
                bool lblnRecordFound = false;
                //if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id > 0)
                //{
                if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount == null)
                    lobjPersonBeneficiary.ibusPersonAccountBeneficiary.LoadPersonAccount();
                if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan == null)
                    lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.LoadPlan();
                if ((lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.IsRetirementPlan())
               || (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.ibusPersonAccount.ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeGroupLife))
                    lblnRecordFound = true;
                //}

                //if (lobjPersonBeneficiary.iclbBenefitApplicationBeneficiary.IsNull())
                //    lobjPersonBeneficiary.LoadBenefitApplicationsBeneficiary();
                //if (lobjPersonBeneficiary.iclbBenefitApplicationBeneficiary.Count > 0)
                //{
                //    lblnRecordFound = true;
                //}
                if (lblnRecordFound)
                {
                    if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id != 0)
                        lobjPersonBeneficiary.SetAddressStatusFlag();
                    else if (lobjPersonBeneficiary.icdoPersonBeneficiary.benificiary_org_id != 0)
                    {
                        if (lobjPersonBeneficiary.ibusBeneficiaryOrganization.icdoOrganization.primary_address_id != 0)
                            lobjPersonBeneficiary.icdoPersonBeneficiary.istrIsAddressActive = busConstant.Flag_Yes;
                    }

                    if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_beneficiary_id > 0)
                        iclbPersonPlanBeneficiaryForDeceased.Add(lobjPersonBeneficiary);
                    _iclbPersonBeneficiaryForDeceased.Add(lobjPersonBeneficiary);
                }
            }
            iclbPersonPlanBeneficiaryForDeceased = iclbPersonPlanBeneficiaryForDeceased.OrderByDescending(i => i.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date == DateTime.MinValue).
                                                   ThenByDescending(obj => obj.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date).ToList().ToCollection();
        }

        private Collection<busPersonBeneficiary> _iclbBeneficiariesByPersonAccount;
        public Collection<busPersonBeneficiary> iclbBeneficiariesByPersonAccount
        {
            get { return _iclbBeneficiariesByPersonAccount; }
            set { _iclbBeneficiariesByPersonAccount = value; }
        }

        public void LoadBeneficiaryForMemberByPersonAccount(int aintPersonAccountID)
        {
            if (iclbPersonBeneficiary == null)
                LoadBeneficiary();

            _iclbBeneficiariesByPersonAccount = new Collection<busPersonBeneficiary>();
            foreach (busPersonBeneficiary lobjPersonBeneficiary in _iclbPersonBeneficiary)
            {
                if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.IsNotNull())
                {
                    if (lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_id == aintPersonAccountID)
                    {
                        lobjPersonBeneficiary.istrMSSPlanName = lobjPersonBeneficiary.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.istrPlanName + " Retirement"; //pir 8506
                        _iclbBeneficiariesByPersonAccount.Add(lobjPersonBeneficiary);
                    }
                }
            }
        }

        //load deceased Person details related to this Member
        //this will be used in person overview maintenance
        public void LoadAllDeathNotificationForPerson()
        {
            if (_iclbDeathNotification == null)
                _iclbDeathNotification = new Collection<busDeathNotification>();

            //merge all collection into one.
            MergeAllDeathnotificationCollections();

            //check if the person himself is dead.
            // if so then add himself into collection with relationship as Self.
            busDeathNotification lobjDeathNotification = new busDeathNotification();
            if (lobjDeathNotification.FindDeathNotificationByPersonId(icdoPerson.person_id))
            {
                lobjDeathNotification.LoadPerson();
                lobjDeathNotification.istrRealtionship = busConstant.RelationshipWithMemberIsSelf;
                iclbDeathNotification.Add(lobjDeathNotification);
            }
        }

        //Load all beneficiaries, contacts, dependents collection.
        //and merge it in one collections
        private void MergeAllDeathnotificationCollections()
        {
            if (iclbDeathNotificationForBeneficiaries == null)
                LoadDeathNotificationsForBenficiaries();
            foreach (busDeathNotification lobjDeath in iclbDeathNotificationForBeneficiaries)
            {
                lobjDeath.LoadPerson();
                _iclbDeathNotification.Add(lobjDeath);
            }

            if (iclbDeathNotificationForContacts == null)
                LoaddeathNotificationForContact();
            foreach (busDeathNotification lobjDeath in iclbDeathNotificationForContacts)
            {
                lobjDeath.LoadPerson();
                _iclbDeathNotification.Add(lobjDeath);
            }

            if (iclbDeathNotificationForDependents == null)
                LoadDeathNotificationsForDependent();
            foreach (busDeathNotification lobjDeath in iclbDeathNotificationForDependents)
            {
                lobjDeath.LoadPerson();
                iclbDeathNotification.Add(lobjDeath);
            }
        }
        //Load death notification of the contacts for this person id.
        private void LoaddeathNotificationForContact()
        {
            iclbDeathNotificationForContacts = new Collection<busDeathNotification>();
            if (icolPersonContact == null)
                LoadContacts();
            foreach (busPersonContact lobjPersonContact in icolPersonContact)
            {
                busDeathNotification lobjdeathNotification = new busDeathNotification();
                if (lobjdeathNotification.FindDeathNotificationByPersonId(lobjPersonContact.icdoPersonContact.contact_person_id))
                {
                    lobjdeathNotification.istrRealtionship = busConstant.RelationshipWithMemberIsContact;
                    _iclbDeathNotificationForContacts.Add(lobjdeathNotification);
                }
            }
        }

        //Load death notification of the dependents for this person id.
        private void LoadDeathNotificationsForDependent()
        {
            iclbDeathNotificationForDependents = new Collection<busDeathNotification>();
            if (iclbPersonDependent == null)
                LoadDependent();
            foreach (busPersonDependent lobjPersonDependent in iclbPersonDependent)
            {
                busDeathNotification lobjdeathNotification = new busDeathNotification();
                if (lobjdeathNotification.FindDeathNotificationByPersonId(lobjPersonDependent.icdoPersonDependent.dependent_perslink_id))
                {
                    lobjdeathNotification.istrRealtionship = busConstant.RelationshipWithMemberIsDependent;
                    _iclbDeathNotificationForDependents.Add(lobjdeathNotification);
                }
            }
        }

        //Load death notification of the beneficiaries for this person id.
        private void LoadDeathNotificationsForBenficiaries()
        {
            iclbDeathNotificationForBeneficiaries = new Collection<busDeathNotification>();
            if (iclbPersonBeneficiary == null)
                LoadBeneficiary();
            foreach (busPersonBeneficiary lobjPersonBeneficiary in iclbPersonBeneficiary)
            {
                busDeathNotification lobjdeathNotification = new busDeathNotification();
                if (lobjdeathNotification.FindDeathNotificationByPersonId(lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id))
                {
                    lobjdeathNotification.istrRealtionship = busConstant.RelationshipWithMemberIsBeneficiary;
                    _iclbDeathNotificationForBeneficiaries.Add(lobjdeathNotification);
                }
            }
        }

        //Load all the benefit calculation Payee
        public void LoadBenefitCalculationPayee()
        {
            DataTable ldtbBenefitCalculationPayeelist = Select<cdoBenefitCalculationPayee>(new string[1] { "payee_person_id" }, new object[1] { icdoPerson.person_id }, null, null);
            _iclbBenefitCalculationPayee = GetCollection<busBenefitCalculationPayee>(ldtbBenefitCalculationPayeelist, "icdoBenefitCalculationPayee");
        }

        public void LoadPersonAccountBeneficiary()
        {
            DataTable ldtbPersonAccountBeneficiary = Select("cdoPerson.LoadPersonAccountBeneficiary", new object[1] { icdoPerson.person_id });
            _iclbPersonAccountBeneficiary = new Collection<busPersonAccountBeneficiary>();
            foreach (DataRow dr in ldtbPersonAccountBeneficiary.Rows)
            {
                busPersonAccountBeneficiary lobjPersonAccountBeneficiary = new busPersonAccountBeneficiary();
                lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary = new cdoPersonAccountBeneficiary();
                lobjPersonAccountBeneficiary.icdoPersonAccountBeneficiary.LoadData(dr);
                _iclbPersonAccountBeneficiary.Add(lobjPersonAccountBeneficiary);
            }
        }

        public void LoadPersonAccountDependent()
        {
            DataTable ldtbPersonAccountDependent = Select("cdoPerson.LoadPersonAccountDependent", new object[1] { icdoPerson.person_id });
            _iclbPersonAccountDependent = new Collection<busPersonAccountDependent>();
            foreach (DataRow dr in ldtbPersonAccountDependent.Rows)
            {
                busPersonAccountDependent lobjPersonAccountDepenedent = new busPersonAccountDependent();
                lobjPersonAccountDepenedent.icdoPersonAccountDependent = new cdoPersonAccountDependent();
                lobjPersonAccountDepenedent.icdoPersonAccountDependent.LoadData(dr);
                _iclbPersonAccountDependent.Add(lobjPersonAccountDepenedent);
            }
        }

        public void LoadPersonContactTo()
        {
            DataTable ldtPersonContactTo = Select<cdoPersonContact>(new string[1] { "contact_person_id" }, new object[1] { icdoPerson.person_id }, null, null);
            _iclbPersonContactTo = GetCollection<busPersonContact>(ldtPersonContactTo, "icdoPersonContact");
            foreach (busPersonContact lobjPersonContact in _iclbPersonContactTo)
            {
                lobjPersonContact.LoadContactName();
                lobjPersonContact.LoadPerson(); // UAT PIR ID 1208
            }
        }
        # region Cor 53 related properties and methods
        //*******************CORRESPONDENCE PROPERTIES USED FOR UCS-053

        private string _isDeceasedSpouseInLife;
        public string isDeceasedSpouseInLife
        {
            get
            {
                return _isDeceasedSpouseInLife;
            }
            set
            {
                _isDeceasedSpouseInLife = value;
            }
        }

        private string _isDeceasedEnrolledInHealth;
        public string isDeceasedEnrolledInHealth
        {
            get
            {
                IsPersonDependentForInsurancePlan();
                return _isDeceasedEnrolledInHealth;
            }

        }

        private string _isDeceasedEnrolledInLife;
        public string isDeceasedEnrolledInLife
        {
            get
            {
                IsPersonDependentForInsurancePlan();
                return _isDeceasedEnrolledInLife;
            }
        }

        private string _isDeceasedEnrolledInVision;
        public string isDeceasedEnrolledInVision
        {
            get
            {
                IsPersonDependentForInsurancePlan();
                return _isDeceasedEnrolledInVision;
            }

        }

        private string _isDeceasedEnrolledInDental;
        public string isDeceasedEnrolledInDental
        {
            get
            {
                IsPersonDependentForInsurancePlan();
                return _isDeceasedEnrolledInDental;
            }
        }

        private decimal _idecDependentSupplementalAmount;
        public decimal idecDependentSupplementalAmount
        {
            get { return _idecDependentSupplementalAmount; }
            set { _idecDependentSupplementalAmount = value; }
        }

        private bool _isMemberEnrolledInLifeSpouseSupplemental;
        public bool isMemberEnrolledInLifeSpouseSupplemental
        {
            get { return _isMemberEnrolledInLifeSpouseSupplemental; }
            set { _isMemberEnrolledInLifeSpouseSupplemental = value; }
        }

        public bool _IsDeceasedEnrolledInLTC;
        public bool IsDeceasedEnrolledInLTC
        {
            get
            {
                return _IsDeceasedEnrolledInLTC;
            }
        }

        private Collection<busPersonAccountLifeOption> _iclbPersonAccountLifeOption;

        public Collection<busPersonAccountLifeOption> iclbPersonAccountLifeOption
        {
            get { return _iclbPersonAccountLifeOption; }
            set { _iclbPersonAccountLifeOption = value; }
        }

        //Cor APP-7500
        //DeceasedIsBeneficiaryForDBOrDCPlan
        //check deceased member is enrolled in DB or DC plan for beneficiary
        //with doubt do we have to check if the person account beneficiary must be end dated????????????????????????????????
        private string _IsPersonBeneficiaryForDBOrDCPlan;
        public string IsPersonBeneficiaryForDBOrDCPlan
        {
            get
            {
                return _IsPersonBeneficiaryForDBOrDCPlan;
            }
            set { _IsPersonBeneficiaryForDBOrDCPlan = value; }
        }

        public string istrPersonInLTC
        {
            get
            {
                int lintPersonAccountID = LoadActivePersonAccountByPlan(busConstant.PlanIdLTC).icdoPersonAccount.person_account_id;
                if (lintPersonAccountID != 0)
                    return busConstant.Flag_Yes;
                else
                    return busConstant.Flag_No;
            }
        }

        public void IsPersonDependentForInsurancePlan()
        {
            if (iclbPersonAccountDependent == null)
                LoadPersonAccountDependent();
            foreach (busPersonAccountDependent lobjPersonAccountDependent in iclbPersonAccountDependent)
            {
                if (lobjPersonAccountDependent.ibusPersonAccount == null)
                    lobjPersonAccountDependent.LoadPersonAccount();
                if (lobjPersonAccountDependent.ibusPersonAccount.ibusPlan == null)
                    lobjPersonAccountDependent.ibusPersonAccount.LoadPlan();

                if (lobjPersonAccountDependent.ibusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupHealth)
                {
                    _isDeceasedEnrolledInHealth = busConstant.Flag_Yes;
                }
                if (lobjPersonAccountDependent.ibusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDental)
                {
                    _isDeceasedEnrolledInDental = busConstant.Flag_Yes;
                }
                if (lobjPersonAccountDependent.ibusPersonAccount.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdVision)
                {
                    _isDeceasedEnrolledInVision = busConstant.Flag_Yes;
                }
            }
        }


        //Check member is enrolled in Def comp
        public string istrMemberEnrolledInDefComp
        {
            get
            {
                if (icolPersonAccount == null)
                    LoadPersonAccount();
                foreach (busPersonAccount lobjPersonAccount in icolPersonAccount)
                {
                    if ((lobjPersonAccount.icdoPersonAccount.end_date == DateTime.MinValue)
                        && ((lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation)
                        || (lobjPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdOther457))
                        && (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled))
                        return busConstant.Flag_Yes;
                }
                return busConstant.Flag_No;
            }
        }

        //Load Person Account Life option
        public Collection<busPersonAccountLifeOption> LoadPersonAccountLifeOption()
        {
            _iclbPersonAccountLifeOption = new Collection<busPersonAccountLifeOption>();

            if (ibusLifePersonAccount == null)
                LoadLifePersonAccount();
            if (ibusLifePersonAccount.ibusPersonAccountLife == null)
                ibusLifePersonAccount.LoadPersonAccountLife();

            if (ibusLifePersonAccount.ibusPersonAccountLife.icdoPersonAccountLife.person_account_id > 0)
            {
                if (ibusLifePersonAccount.ibusPersonAccountLife.iclbLifeOption == null)
                {
                    ibusLifePersonAccount.ibusPersonAccountLife.LoadLifeOptionData();
                    _iclbPersonAccountLifeOption = ibusLifePersonAccount.ibusPersonAccountLife.iclbLifeOption;
                }
            }
            return _iclbPersonAccountLifeOption;
        }

        //check if member is enrolled in Dependent Supplemental Life Insurance
        public string istrMemberIsDependentSupplementalLifeInsurance
        {
            get
            {
                if (ibusLifePersonAccount.IsNull()) LoadLifePersonAccount();
                if (_iclbPersonAccountLifeOption == null) LoadPersonAccountLifeOption();

                if (ibusLifePersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) // PROD PIR 7068
                {
                    foreach (busPersonAccountLifeOption lobjPersonAccountLifeOption in _iclbPersonAccountLifeOption)
                    {
                        if (lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental &&
                            lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.coverage_amount != 0.00M)
                        {
                            _idecDependentSupplementalAmount = lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                            return busConstant.Flag_Yes;
                        }
                    }
                }
                return busConstant.Flag_No;
            }
        }

        //this property is used in correspondence in ucs-53
        private decimal _idecSpouseSupplementalAmount;
        public decimal idecSpouseSupplementalAmount
        {
            get { return _idecSpouseSupplementalAmount; }
            set { _idecSpouseSupplementalAmount = value; }
        }

        public string IsDeceasedIsSpouseToPerson { get; set; }

        public string istrMemberIsSpouseSupplementalLifeInsurance
        {
            get
            {
                if (iclbPersonAccountLifeOption == null)
                    LoadPersonAccountLifeOption();

                foreach (busPersonAccountLifeOption lobjPersonAccountLifeOption in iclbPersonAccountLifeOption)
                {
                    if ((lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental)
                        && (IsDeceasedIsSpouseToPerson == busConstant.Flag_Yes))
                    {
                        _idecSpouseSupplementalAmount = lobjPersonAccountLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                        return busConstant.Flag_Yes;
                    }
                }
                return busConstant.Flag_No;
            }
        }

        # endregion

        # region Employee death batch letter
        public string istrRelationshipToBeneficiary { get; set; }
        public string istrIsMemberNotBeneficiaryInDBDCPlan { get; set; }
        public string istrIsEstateInDBDCPlan { get; set; }
        public string istrIsRelationshipTrustee { get; set; }
        public string istrIsDeceasedEmployeeVested { get; set; }
        public string istrIsDeceasedEmployeeNonVested { get; set; }
        public string istrIsVestedIfMultipleBenesOrNonVestedIfPersonIsBene { get; set; }
        public string istrIsRelationshipSpouse { get; set; }
        public string istrIsVestedRelationshipSpouse { get; set; }
        public string istrIsBeneficiaryAgeLessThan18 { get; set; }
        public string istrIsDCBeneficiarySpouse { get; set; }
        public string istrIsMemberHadDCPlan { get; set; }
        //health
        public string istrIsBeneficiaryDependentInHealth { get; set; }
        public string istrIsDeceasedNotEnrolledNotVestedInHealth { get; set; }
        public string istrIsDeceasedEnrolledInHealthNoDependents { get; set; }
        public string istrIsVestedMarriedBeneDependentInHealth { get; set; }
        public string istrIsVestedNotInHealthRelationshipMarried { get; set; }
        public string istrIsSpouseOrDependentInHealth { get; set; }
        //dental
        public string istrIsBeneficiaryDependentInDental { get; set; }
        public string istrIsDeceasedNotEnrolledNotVestedInDental { get; set; }
        public string istrIsDeceasedEnrolledInDentalNoDependents { get; set; }
        public string istrIsVestedMarriedBeneDependentInDental { get; set; }
        public string istrIsVestedNotInDentalRelationshipMarried { get; set; }
        public string istrIsSpouseOrDependentInDental { get; set; }
        //Vision
        public string istrIsBeneficiaryDependentInVision { get; set; }
        public string istrIsDeceasedNotEnrolledNotVestedInVision { get; set; }
        public string istrIsDeceasedEnrolledInVisionNoDependents { get; set; }
        public string istrIsVestedMarriedBeneDependentInVision { get; set; }
        public string istrIsVestedNotInVisionRelationshipMarried { get; set; }
        public string istrIsSpouseOrDependentInVision { get; set; }
        //Life
        public string istrIsBeneficiaryInLife { get; set; }
        public string istrIsBeneficiaryNotInLife { get; set; }
        public string istrIsLifeBeneAgeUnder18 { get; set; }
        public string istrIsLifeBeneTrusteeOrEstate { get; set; }
        public string istrIsLifeBeneTrustee { get; set; }
        public string istrIsLifeBeneEstate { get; set; }

        //LTC
        public string istrIsLTCExtistsForBoth { get; set; }
        public string istrIsLTCExtistsForNone { get; set; }
        public string istrIsLTCExtistsForDeceased { get; set; }
        public string istrIsLTCExtistsForSpouse { get; set; }

        //Def comp  
        public string istrIsdefCompPlanExits { get; set; }
        public string istrIsdefCompPlanNotExits { get; set; }

        //Other 457 
        public string istrIsOther457PlanExits { get; set; }
        public string istrIsOther457PlanNotExits { get; set; }

        //DC
        public string istrIsDCPlanExits { get; set; }

        public string istrIsMemberInHealthDependentsEligible { get; set; }
        public string istrIsMemberInDentalDependentsEligible { get; set; }
        public string istrIsMemberInVisionDependentsEligible { get; set; }
        public string istrIsDCNonVestedOrSingleDeathOrDCVestedAndMarriedDeath { get; set; }

        //Amounts
        public decimal idecDeceasedMemberBalanceAmount { get; set; }
        public decimal idecTaxableMemberBalanceAmount { get; set; }
        public decimal idecNonTaxableMemberBalanceAmount { get; set; }
        public decimal idec20PercentOfTaxableAmount { get; set; }
        public decimal idecHigherValueOf50PercentOr100PercentJSAmt { get; set; }

        public decimal idecHealthSinglePolicyCOBRAPremium { get; set; }
        public decimal idecHealthFamilyPolicyCOBRAPremium { get; set; }
        public decimal idecHealthFamilyOf3PolicyCOBRAPremium { get; set; }

        public decimal idecHealthSinglePolicyCOBRANetAmt { get; set; }

        public decimal idecHealthFamilyPolicyCOBRANetAmt { get; set; }
        public decimal idecHealthFamilyOf3PolicyCOBRANetAmt { get; set; }

        public decimal idecHealthRHIC { get; set; }
        public decimal idecHealthSinglePolicyNetAmount { get; set; }
        public decimal idecHealthFamilyPolicyNetAmount { get; set; }
        public decimal idecHealthFamilyOf3PolicyNetAmount { get; set; }

        public decimal idecHealthSinglePolicyPremium { get; set; }
        public decimal idecHealthFamilyPolicyPremium { get; set; }
        public decimal idecHealthFamily3PolicyPremium { get; set; }

        public decimal idecDentalSinglePolicyCOBRAPremium { get; set; }
        public decimal idecDentalFamilyPolicyCOBRAPremium { get; set; }

        public decimal idecDentalSinglePolicyPremium { get; set; }
        public decimal idecDentalFamilyPolicyPremium { get; set; }

        public decimal idecVisionSinglePolicyCOBRAPremium { get; set; }
        public decimal idecVisionFamilyPolicyCOBRAPremium { get; set; }

        public decimal idecVisionSinglePolicyPremium { get; set; }
        public decimal idecVisionFamilyPolicyPremium { get; set; }

        //public decimal idecSinglePolicyPremium { get; set; }
        //public decimal idecFamilyPolicyPremium { get; set; }
        public decimal idecLifeInsurancePolicyValue { get; set; }
        public decimal idecBeneLifePercentage { get; set; }
        public decimal idecFamilyOfThreePolicyPremium { get; set; }
        public decimal idecBeneRetPercentage { get; set; } //PROD PIR 7067

        public string istrisHealthVisible { get; set; }
        public string istrisDentalVisible { get; set; }
        public string istrisVisionVisible { get; set; }

        #endregion

        #region Welcome Batch Properties
        private bool _is_permanent_has_past_refund;
        public bool is_permanent_has_past_refund
        {
            get { return _is_permanent_has_past_refund; }
            set { _is_permanent_has_past_refund = value; }
        }

        private bool _is_permanent_and_not_in_flex_plan;
        public bool is_permanent_and_not_in_flex_plan
        {
            get { return _is_permanent_and_not_in_flex_plan; }
            set { _is_permanent_and_not_in_flex_plan = value; }
        }

        private bool _is_permanent_and_not_in_deff_comp_plan;
        public bool is_permanent_and_not_in_deff_comp_plan
        {
            get { return _is_permanent_and_not_in_deff_comp_plan; }
            set { _is_permanent_and_not_in_deff_comp_plan = value; }
        }

        private DateTime _previous_employment_start_date;
        public DateTime previous_employment_start_date
        {
            get { return _previous_employment_start_date; }
            set { _previous_employment_start_date = value; }
        }

        private DateTime _previous_employment_end_date;
        public DateTime previous_employment_end_date
        {
            get { return _previous_employment_end_date; }
            set { _previous_employment_end_date = value; }
        }

        private Collection<busPersonEmployment> _iclbRefundIssuedEmployment;
        public Collection<busPersonEmployment> iclbRefundIssuedEmployment
        {
            get { return _iclbRefundIssuedEmployment; }
            set { _iclbRefundIssuedEmployment = value; }
        }
        #endregion

        /// <summary>
        /// Method to Determine the Member is already enrolled in given Plan
        /// </summary>
        /// <param name="aintPlanId"></param>
        /// <returns></returns>
        public bool IsMemberEnrolledInPlan(int aintPlanId)
        { 
            bool lblnResult = false;
            if (icolPersonAccount == null)
                LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in icolPersonAccount)
            {
                if (lbusPersonAccount.icdoPersonAccount.plan_id == aintPlanId && lbusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirmentCancelled 
                    && lbusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.RetirementPlanParticipationStatusTranToDb) //PIR 15851 - As per call with maik
                {
                    lblnResult = true;
                    break;
                }
            }
            return lblnResult;
        }

        /// <summary>
        /// Method to Check this member is permanent in any employment detail (Multiple Employment Scenario)
        /// </summary>
        /// <returns></returns>
        public bool IsPermanentMember()
        {
            if (icolPersonEmployment == null)
                LoadPersonEmployment();

            foreach (busPersonEmployment lbusPersonEmployment in icolPersonEmployment)
            {
                if (lbusPersonEmployment.icolPersonEmploymentDetail == null)
                    lbusPersonEmployment.LoadPersonEmploymentDetail();

                foreach (busPersonEmploymentDetail lbusPersonEmploymentDetail in lbusPersonEmployment.icolPersonEmploymentDetail)
                {
                    if (lbusPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #region service purchase PIR-15531 and PIR-9403
        public bool IsPermanentMemberService()
        {
            DataTable ldtPersonEmployementDetail = busBase.Select("cdoPersonEmploymentDetail.LoadLatestPersonEmploymentDetail", new object[1] {icdoPerson.person_id});
            return (ldtPersonEmployementDetail.Rows.Count > 0) && (ldtPersonEmployementDetail.Rows[0]["TYPE_VALUE"] != DBNull.Value) && 
                (ldtPersonEmployementDetail.Rows[0]["TYPE_VALUE"].ToString() == busConstant.PersonJobTypePermanent) ? true : false;
        }
        #endregion

        public bool IsMemberHasRefundApplication()
        {
            bool lblnResult = false;
            if (_iclbBenefitApplication == null)
                LoadBenefitApplication();

            foreach (busBenefitApplication lbusBenefitApplication in _iclbBenefitApplication)
            {
                if (lbusBenefitApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                {
                    lblnResult = true;
                    break;
                }
            }

            return lblnResult;
        }

        //property to payment history details for refund amount
        public Collection<busPaymentHistoryHeader> iclbPaymentDetails { get; set; }
        //property to store total gross amount paid
        public decimal idecGrossAmountPaid { get; set; }
        //property to store total non taxable amount paid
        public decimal idecNonTaxableAmountPaid { get; set; }
        //property to store total taxable amount paid
        public decimal idecTaxableAmountPaid { get; set; }

        // 75 Correspondence
        public decimal idecCheckAmount { get; set; }
        public string istrChecknumber { get; set; }
        public string istrPaymentMonth { get; set; }
        public string istrIsInsurancePlan { get; set; }
        public void Load75CorrespondenceProperties()
        {
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                if (iclbPaymentHistoryHeader[0].iclbPaymentHistoryDistribution == null)
                    iclbPaymentHistoryHeader[0].LoadPaymentHistoryDistribution();
                if (iclbPaymentHistoryHeader[0].iclbPaymentHistoryDistribution.Count > 0)
                {
                    idecCheckAmount = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDistribution.FirstOrDefault().icdoPaymentHistoryDistribution.net_amount;
                    istrChecknumber = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDistribution.FirstOrDefault().icdoPaymentHistoryDistribution.check_number;
                    istrPaymentMonth = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDistribution.FirstOrDefault().istrPaymentMonth;
                    istrIsInsurancePlan = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDistribution.FirstOrDefault().istrIsInsurancePlan;
                    if (iclbPaymentHistoryHeader[0].ibusPlan == null)
                        iclbPaymentHistoryHeader[0].LoadPlan();
                    istrPlan = iclbPaymentHistoryHeader[0].ibusPlan.icdoPlan.plan_name;
                }
            }
        }
        //Property to contain Payment History of a payee account
        public Collection<busPaymentHistoryHeader> iclbPaymentHistoryHeader { get; set; }

        /// <summary>
        /// Method to load Payment History collection for current payee account
        /// </summary>
        public void LoadPaymentHistoryHeader()
        {
            if (iclbPaymentHistoryHeader == null)
                iclbPaymentHistoryHeader = new Collection<busPaymentHistoryHeader>();
            DataTable ldtPaymentHistory = Select<cdoPaymentHistoryHeader>(new string[1] { "person_id" },
                                                                    new object[1] { icdoPerson.person_id },
                                                                    null, "PAYMENT_DATE desc");
            iclbPaymentHistoryHeader = GetCollection<busPaymentHistoryHeader>(ldtPaymentHistory, "icdoPaymentHistoryHeader");
        }
        /// <summary>
        /// Method to load Payment details 
        /// </summary>
        /// 

        public void LoadPaymentDetails()
        {
            iclbPaymentDetails = new Collection<busPaymentHistoryHeader>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                DateTime ldtLastPaymentDate = new DateTime(iclbPaymentHistoryHeader[0].icdoPaymentHistoryHeader.payment_date.Year,
                    iclbPaymentHistoryHeader[0].icdoPaymentHistoryHeader.payment_date.Month, 1);
                ldtLastPaymentDate = ldtLastPaymentDate.AddMonths(1).AddDays(-1);

                DataTable ldtPaymentHistory = Select("cdoPaymentHistoryHeader.LoadRefundAmtForPerson",
                    new object[3] { icdoPerson.person_id, ldtLastPaymentDate, busConstant.Flag_Yes });
                iclbPaymentDetails = GetCollection<busPaymentHistoryHeader>(ldtPaymentHistory, "icdoPaymentHistoryHeader");
                if (iclbPaymentDetails.Count > 0)
                {
                    idecGrossAmountPaid = iclbPaymentDetails.Select(o => o.icdoPaymentHistoryHeader.gross_amount).Sum();
                    idecNonTaxableAmountPaid = iclbPaymentDetails.Select(o => o.icdoPaymentHistoryHeader.NonTaxable_Amount).Sum();
                    idecTaxableAmountPaid = iclbPaymentDetails.Select(o => o.icdoPaymentHistoryHeader.taxable_amount).Sum();
                }
            }
        }
        #region UCS - 081 Correspondence

        public busBenefitApplication ibusBenefitApplication { get; set; }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            LoadRetirementBenefitApplication();
            Load75CorrespondenceProperties();
            if (icolPersonEmployment.IsNull())
                LoadPersonEmployment(true);
            LoadCurrentEmployerDetails();
            SetIsMemberEnrolledInDBOrDC();
            if(astrTemplateName=="SFN-58868")
            {
                this.ibusCurrentEmployment = iclbActivePersonEmployment.Count > 0 ? iclbActivePersonEmployment.FirstOrDefault() : icolPersonEmployment.Count > 0 ? icolPersonEmployment.FirstOrDefault() : new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                this.ibusCurrentEmployment.LoadOrganization();
                this.iintPlanID = busConstant.PlanIdMain;
            }
            
        }

        public Collection<busPerson> iclbPremiumAmountsForCorr { get; set; }
        public string istrPlanNameForCorr { get; set; }
        public string istrOldRatesForCorr { get; set; }
        public string istrNewRatesForCorr { get; set; }
        public string istrRateDifferenceForCorr { get; set; }

        public override void LoadBookmarkValues(utlCorresPondenceInfo aobjCorrespondenceInfo, Hashtable ahstQueryBookmarks)
        {
            if (aobjCorrespondenceInfo.istrTemplateName == "PAY-4309")
            {
                List<string> lstPlanName = new List<string>();
                List<string> lstPlanNameForDisplay = new List<string>();
                string lstrCombinedPlanName = string.Empty;
                List<string> lstAmount = new List<string>();

                if (ahstQueryBookmarks.IsNotNull() && ahstQueryBookmarks.Keys.Count > 0)
                {
                    foreach (DictionaryEntry ldicentry in ahstQueryBookmarks)
                    {
                        if (ldicentry.Key.ToString() == "PLAN")
                        {
                            lstrCombinedPlanName = GetValueOfHashtable(Convert.ToString(ldicentry.Value));
                            lstPlanNameForDisplay.Add(Convert.ToString(ldicentry.Value));
                        }
                    }

                    lstPlanName = lstrCombinedPlanName.Split(';').ToList();

                    iclbPremiumAmountsForCorr = new Collection<busPerson>();
                    lstPlanName.Where(lstr=>lstr=="Health" || lstr == "Dental" || lstr == "Vision").ForEach(a =>
                        {
                            string ldecOldRatesForCorr = a == "Health" ? "OldRate1" : a == "Dental" ? "OldRate2" : a== "Vision" ? "OldRate3" : string.Empty;
                            string ldecNewRatesForCorr = a == "Health" ? "NewRate1" : a == "Dental" ? "NewRate2" : a == "Vision" ? "NewRate3" : string.Empty;
                            string ldecRateDifferenceForCorr = a == "Health" ? "DifferenceInRates1" : a == "Dental" ? "DifferenceInRates2" : a == "Vision" ? "DifferenceInRates3" : string.Empty;

                            string ldecTempOldRatesForCorr = GetValueOfHashtable(Convert.ToString(ahstQueryBookmarks[ldecOldRatesForCorr]));
                            string ldecTempNewRatesForCorr = GetValueOfHashtable(Convert.ToString(ahstQueryBookmarks[ldecNewRatesForCorr]));
                            string ldecTempRateDifferenceForCorr = GetValueOfHashtable(Convert.ToString(ahstQueryBookmarks[ldecRateDifferenceForCorr]));
                            iclbPremiumAmountsForCorr.Add(new busPerson()
                            {
                                istrPlanNameForCorr = a.ToString(),
                                istrOldRatesForCorr = ldecTempOldRatesForCorr.Substring(ldecTempOldRatesForCorr.IndexOf('$') + 1, ldecTempOldRatesForCorr.Length - (ldecTempOldRatesForCorr.IndexOf('$') + 1)),
                                istrNewRatesForCorr = ldecTempNewRatesForCorr.Substring(ldecTempNewRatesForCorr.IndexOf('$') + 1, ldecTempNewRatesForCorr.Length - (ldecTempNewRatesForCorr.IndexOf('$') + 1)),
                                istrRateDifferenceForCorr = ldecTempRateDifferenceForCorr.Substring(ldecTempRateDifferenceForCorr.IndexOf('$') + 1, ldecTempRateDifferenceForCorr.Length - (ldecTempRateDifferenceForCorr.IndexOf('$') + 1)),
                            });
                        });
                        istrDollar1 = GetValueOfHashtable(Convert.ToString(ahstQueryBookmarks["DOLLAR1"]));
                        istrDollar2 = GetValueOfHashtable(Convert.ToString(ahstQueryBookmarks["DOLLAR2"]));
                        istrDollar3 = GetValueOfHashtable(Convert.ToString(ahstQueryBookmarks["DOLLAR3"]));
                        istrNumber1 = GetValueOfHashtable(Convert.ToString(ahstQueryBookmarks["NUMBER1"]));
                        istrNumber2 = GetValueOfHashtable(Convert.ToString(ahstQueryBookmarks["NUMBER2"]));
                        istrNumber3 = GetValueOfHashtable(Convert.ToString(ahstQueryBookmarks["NUMBER3"]));
                }

                StringBuilder lsbNumberDollar = new StringBuilder();
                if (istrNumber1.IsNotNullOrEmpty() && istrDollar1.IsNotNullOrEmpty())
                {
                    lsbNumberDollar.Append(istrNumber1 + " for " + istrDollar1);
                }
                if (istrNumber2.IsNotNullOrEmpty() && istrDollar2.IsNotNullOrEmpty())
                {
                    lsbNumberDollar.Append(istrNumber3.IsNotNullOrEmpty() && istrDollar3.IsNotNullOrEmpty() ? ", " + istrNumber2 + " for " + istrDollar2 : " and " + istrNumber2 + " for " + istrDollar2);
                }
                if (istrNumber3.IsNotNullOrEmpty() && istrDollar3.IsNotNullOrEmpty())
                {
                    lsbNumberDollar.Append(" and " + istrNumber3 + " for " + istrDollar3);
                }
                istrCheckNumberDollar = lsbNumberDollar.ToString();

                string lstrTempstring = lstPlanNameForDisplay[0].ToString().Replace(";", ",");
                int lintStartIndex = lstrTempstring.IndexOf(':') + 1;
                int lintEndIndex = lstrTempstring.Length - lintStartIndex;
                lstrTempstring = lstrTempstring.Substring(lintStartIndex, lintEndIndex);
                List<string> lstrTemp = new List<string>(lstrTempstring.Split(','));

                if (lstrTemp.Count > 1)
                    istrMultiplePlanName = String.Join(", ", lstrTemp.ToArray(), 0, lstrTemp.Count - 1) + " and " + lstrTemp.LastOrDefault();
                else
                    istrMultiplePlanName = lstrTemp.LastOrDefault();
            }
            else if(aobjCorrespondenceInfo.istrTemplateName== "PER-0250")
            {
                if (aobjCorrespondenceInfo.icolBookmarkChldTemplateInfo.Count>0)
                {
                    foreach (utlCorresPondenceInfo ldicEntry in aobjCorrespondenceInfo.icolBookmarkChldTemplateInfo)
                    {
                        if (ldicEntry.istrTemplateName == "SFN-51702" && ldicEntry.icolBookmarkFieldInfo.Count > 0 || ldicEntry.istrTemplateName == "SFN-52254" && ldicEntry.icolBookmarkFieldInfo.Count > 0)
                        {
                            foreach (utlBookmarkFieldInfo lultFieldinfo in ldicEntry.icolBookmarkFieldInfo)
                            {

                                if (lultFieldinfo.istrName.ToString() == "DeceasedDateOfBirth")
                                {
                                    lultFieldinfo.istrValue = icdoPerson.date_of_birth.ToString("MM/dd/yyyy");
                                }
                                else if (lultFieldinfo.istrName.ToString() == "DeceasedPerslinkId" || lultFieldinfo.istrName.ToString() == "DeceasedMemberID")
                                {
                                    lultFieldinfo.istrValue = icdoPerson.person_id.ToString();

                                }
                                else if (lultFieldinfo.istrName.ToString() == "DeceasedName")
                                {
                                    lultFieldinfo.istrValue = icdoPerson.FullName;
                                }
                                else if (lultFieldinfo.istrName.ToString() == "DeceasedSSN")
                                {
                                    lultFieldinfo.istrValue = icdoPerson.LastFourDigitsOfSSN;
                                }
                                
                            }
                        }
                        
                    }
                    
                }
            }
            base.LoadBookmarkValues(aobjCorrespondenceInfo, ahstQueryBookmarks);
        }

        public static string GetValueOfHashtable(string astrTempString)
        {
            return astrTempString.Substring(astrTempString.IndexOf(':') + 1, astrTempString.Length - (astrTempString.IndexOf(':') + 1));
        }

        public void LoadRetirementBenefitApplication()
        {
            if (ibusBenefitApplication == null)
                ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            DataTable ldtRetirmentBenefitApplication = Select<cdoBenefitApplication>(
                                                        new string[2] { "member_person_id", "benefit_account_type_value" },
                                                        new object[2] { icdoPerson.person_id, busConstant.ApplicationBenefitTypeRetirement },
                                                        null, null);
            if (ldtRetirmentBenefitApplication.Rows.Count > 0)
                ibusBenefitApplication.icdoBenefitApplication.LoadData(ldtRetirmentBenefitApplication.Rows[0]);
        }

        #endregion

        // Load all the Retirement Contribution for the person for all Plans
        public Collection<busPersonAccountRetirementContribution> iclbAllRetirementContributions { get; set; }

        public void LoadAllRetirementContribution()
        {
            // Includes Job Service Plan
            DataTable ldtbResult = Select("cdoPerson.GetAllMemberContributions", new object[1] { icdoPerson.person_id });
            iclbAllRetirementContributions = GetCollection<busPersonAccountRetirementContribution>(ldtbResult, "icdoPersonAccountRetirementContribution");
        }

        # region UCS 60


        public bool IsRTWMember()
        {
            if (iclbRetirementAccount.IsNull())
                LoadRetirementAccount();
            var lclbDistincePersonAccount = iclbRetirementAccount
                .Select(o => o.icdoPersonAccount.plan_id).Distinct();

            foreach (int lintPlanId in lclbDistincePersonAccount)
            {
                if (IsRTWMember(lintPlanId, busConstant.PayeeStatusForRTW.IgnoreStatus))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// used to check the overlapping of plan participation for plan id passed
        /// if the member is RTW in the plan id passed then the system must not check overlapping
        /// </summary>
        /// <param name="aintPlanID"></param>
        /// <returns></returns>
        public bool IsRTWMember(int aintPlanID)
        {
            return IsRTWMember(aintPlanID, busConstant.PayeeStatusForRTW.SuspOrCompOrCancelled);
        }

        /// <summary>
        /// this method is used to check if the member is having payee account
        /// if the payee account is returned then the DB button will be visibile
        /// </summary>
        /// <param name="aenuStatusCheck"></param>
        /// <param name="aintPreRTWPayeeAccountID"></param>
        /// <returns></returns>
        public bool IsRTWMember(int aintPlanid, busConstant.PayeeStatusForRTW aenuStatusCheck)
        {
            int lintPayeeAccountID = 0;
            return IsRTWMember(aintPlanid, aenuStatusCheck, ref lintPayeeAccountID);
        }

        /// <summary>
        /// this method is used only in processing RTW member application and Calculation
        /// UCS-060 - By passing the Member and Plan ID , this method will refer to the corresponding Payee Account, Person Account
        /// </summary>
        /// <param name="aintPlanID">Plan ID</param>
        /// <param name="ablnIncludeEnrolledStatus">for application and calculation only, enrolled status will be checked</param>
        /// <param name="aenuStatusCheck"></param>
        /// <param name="aintPreRTWPayeeAccountID">Return to work Member's previous Payee Account</param>
        /// <returns></returns>
        public bool IsRTWMember(int aintPlanID, busConstant.PayeeStatusForRTW aenuStatusCheck, ref int aintPreRTWPayeeAccountID)
        {
            bool lblnIsRTWMember = false;

            if (iclbRetirementAccount.IsNull())
                LoadRetirementAccount();

            IEnumerable<busPersonAccount> lclbFilterdPersonAccounts =
                iclbRetirementAccount.Where(o => o.icdoPersonAccount.plan_id == aintPlanID &&
                                            o.IsPlanParticipationStatusRetiredOrSuspendedOrEnrolled())
                                            .OrderByDescending(o => o.icdoPersonAccount.start_date);

            if (lclbFilterdPersonAccounts != null)
            {
                foreach (busPersonAccount lbusPersonAccount in lclbFilterdPersonAccounts)
                {
                    if (lbusPersonAccount.IsNotNull())
                    {
                        var lbusPayeeAccount = lbusPersonAccount.LoadRetirementDisablityPayeeAccount();
                        if (lbusPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                        {
                            aintPreRTWPayeeAccountID = lbusPayeeAccount.icdoPayeeAccount.payee_account_id;
                            //Commented since LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                            //if (lbusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                            lbusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();

                            switch (aenuStatusCheck)
                            {
                                case busConstant.PayeeStatusForRTW.SuspOrCompOrCancelled:
                                    if (lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsSuspendedOrCancelledOrCompletedForRetirement() ||
                                        lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsSuspendedOrCancelledOrCompletedForDisability())
                                    {
                                        lblnIsRTWMember = true;
                                    }
                                    break;
                                case busConstant.PayeeStatusForRTW.SuspendedOnly:
                                    lblnIsRTWMember = lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsSuspendedForRetirementOrDisability();
                                    break;
                                case busConstant.PayeeStatusForRTW.IgnoreStatus:
                                    lblnIsRTWMember = true;
                                    break;
                            }
                        }

                        if (lblnIsRTWMember) break;
                    }
                }
            }
            return lblnIsRTWMember;
        }

        /// <summary>
        /// get latest person account id for the passed plan
        /// ignore withdrawn status person account
        /// PIR 16439 - Modified to ignore canceled and all transferred person accounts
        /// </summary>
        /// <param name="aintPlanID"></param>
        /// <returns></returns>
        public busPersonAccount LoadActivePersonAccountByPlan(int aintPlanID)
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            if (icolPersonAccount.IsNull())
                LoadPersonAccount();
            var lenumPersonAccount = icolPersonAccount.Where(
                                                     lobjPA => lobjPA.icdoPersonAccount.plan_id == aintPlanID 
                                                     && (!lobjPA.IsWithDrawn())
                                                     && (!lobjPA.IsCanceled())
                                                     && (!lobjPA.IsTransferredDC())
                                                     && (!lobjPA.IsTransferredDB())
                                                     && (!lobjPA.IsTransferredTIAACREF())
                                                     && (!lobjPA.IsTransferredToTFFR()))
                                                     .OrderByDescending(lobjPA => lobjPA.icdoPersonAccount.start_date);
            if (lenumPersonAccount.Count() > 0)
                lobjPersonAccount = lenumPersonAccount.First();
            return lobjPersonAccount;
        }
        public busPersonAccount LoadActivePersonAccountByPlanForNewHirePostingBatch(int aintPlanID)
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            if (icolPersonAccount.IsNull())
                LoadPersonAccount();
            var lenumPersonAccount = icolPersonAccount.Where(
                                                     lobjPA => lobjPA.icdoPersonAccount.plan_id == aintPlanID
                                                     && (!lobjPA.IsWithDrawn())
                                                     && (!lobjPA.IsTransferredDC())
                                                     && (!lobjPA.IsTransferredDB())
                                                     && (!lobjPA.IsTransferredTIAACREF())
                                                     && (!lobjPA.IsTransferredToTFFR()))
                                                     .OrderByDescending(lobjPA => lobjPA.icdoPersonAccount.start_date);
            if (lenumPersonAccount.Count() > 0)
                lobjPersonAccount = lenumPersonAccount.First();
            return lobjPersonAccount;
        }
        public busPersonAccount LoadEnrOrSuspOrCancAccountByPlan(int aintPlanID)
        {
            busPersonAccount lobjPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            if (icolPersonAccount.IsNull())
                LoadPersonAccount();
            var lenumPersonAccount = icolPersonAccount.Where(
                                                     lobjPA => (lobjPA.icdoPersonAccount.plan_id == aintPlanID)
                                                     && (lobjPA.IsEnrolledOrSuspended() || lobjPA.IsCanceled()))
                                                     .OrderByDescending(lobjPA => lobjPA.icdoPersonAccount.start_date);
            if (lenumPersonAccount.Count() > 0)
                lobjPersonAccount = lenumPersonAccount.First();
            return lobjPersonAccount;
        }
        # endregion

        # region UCS -054

        //BR - 054 01
        public Collection<busPayeeAccount> iclbAlternatePayeeAccounts { get; set; }
        public Collection<busPayeeAccount> iclbMemberPlusAlternatePayeeAccounts { get; set; }

        //load Payee  account associated with the deceased
        //null check for status not done as system must reload whenever there is change in status
        //THIS METHOD IS USED IN THE WEB PORTAL ALSO. B4 CHANGING THIS METHOD LOOK AT THE IMPACT ON PORTAL ALSO
        public void LoadMemberPlusAlternatePayeeAccounts(bool ablndontLoadDROPayeeAccounts = false)
        {
            iclbMemberPlusAlternatePayeeAccounts = new Collection<busPayeeAccount>();
            if (iclbPayeeAccount.IsNull())
                LoadPayeeAccount();
            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
            {
                if (lobjPayeeAccount.ibusPayee.IsNull())
                    lobjPayeeAccount.LoadPayee();
                //Dont check Null condition as LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                lobjPayeeAccount.LoadActivePayeeStatus();
                iclbMemberPlusAlternatePayeeAccounts.Add(lobjPayeeAccount);
            }
            if (!ablndontLoadDROPayeeAccounts) //PIR 17878 - Should not show the alternate payee account
            {
                ////load DRO payee accounts also           
                if (iclbAlternatePayeeAccounts.IsNull())
                    LoadAlternatePayeePayeeAccounts();
                foreach (busPayeeAccount lobjPayeeAccount in iclbAlternatePayeeAccounts)
                {
                    if (lobjPayeeAccount.icdoPayeeAccount.dro_application_id > 0)
                    {
                        if (lobjPayeeAccount.ibusMember.IsNull())
                            lobjPayeeAccount.LoadMember();
                    }
                    if (lobjPayeeAccount.ibusPayee.IsNull()) lobjPayeeAccount.LoadPayee();
                    if (lobjPayeeAccount.ibusDROApplication.IsNull()) lobjPayeeAccount.LoadDROApplication();
                    if (lobjPayeeAccount.ibusDROApplication.istrLifeOfAP != "1") // PROD PIR ID 7963
                    {
                        //Dont check Null condition as LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                        lobjPayeeAccount.LoadActivePayeeStatus();
                        iclbMemberPlusAlternatePayeeAccounts.Add(lobjPayeeAccount);
                    }
                }
            }
        }

        public void LoadAlternatePayeePayeeAccounts()
        {
            iclbAlternatePayeeAccounts = new Collection<busPayeeAccount>();
            if (iclbMemberDROApplication.IsNull())
                LoadMemberDROApplications();
            foreach (busBenefitDroApplication lobjDROApplication in iclbMemberDROApplication)
            {
                if (lobjDROApplication.ibusMember.IsNull())
                    lobjDROApplication.LoadMember();
                lobjDROApplication.LoadPayeeAccountByDROApplicationID();
                foreach (busPayeeAccount lobjPayeeAccount in lobjDROApplication.iclbPayeeAccountByDROAppId)
                {
                    if (lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id != icdoPerson.person_id)
                    {
                        lobjPayeeAccount.ibusMember = new busPerson();
                        lobjPayeeAccount.ibusMember = lobjDROApplication.ibusMember;
                        iclbAlternatePayeeAccounts.Add(lobjPayeeAccount);
                    }
                }
            }
        }

        public Collection<busBenefitDroApplication> iclbMemberDROApplication { get; set; }
        public void LoadMemberDROApplications()
        {
            if (iclbMemberDROApplication.IsNull()) iclbMemberDROApplication = new Collection<busBenefitDroApplication>();
            DataTable ldtbAlternatePayee = Select<cdoBenefitDroApplication>(
              new string[1] { "member_perslink_id" },
              new object[1] { icdoPerson.person_id }, null, null);
            iclbMemberDROApplication = GetCollection<busBenefitDroApplication>(ldtbAlternatePayee, "icdoBenefitDroApplication");
        }

        public Collection<busBenefitDroApplication> iclbAlternatePayeeDROApplication { get; set; }
        public void LoadAlternatePayeeDROApplicaitons()
        {
            if (iclbAlternatePayeeDROApplication.IsNull()) iclbAlternatePayeeDROApplication = new Collection<busBenefitDroApplication>();
            DataTable ldtbAlternatePayee = Select<cdoBenefitDroApplication>(
              new string[1] { "alternate_payee_perslink_id" },
              new object[1] { icdoPerson.person_id }, null, null);
            iclbAlternatePayeeDROApplication = GetCollection<busBenefitDroApplication>(ldtbAlternatePayee, "icdoBenefitDroApplication");
        }

        # endregion

        public bool IsMarried
        {
            get
            {
                if (icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                    return true;
                return false;
            }
        }

        // Returns true if Death Notified for this Person.
        public bool IsDeathNotified()
        {
            DataTable ldtbResult = Select<cdoDeathNotification>(new string[1] { "PERSON_ID" }, new object[1] { icdoPerson.person_id }, null, null);
            if (ldtbResult.Rows.Count > 0)
            {
                busDeathNotification lobjDeathNotification = new busDeathNotification { icdoDeathNotification = new cdoDeathNotification() };
                lobjDeathNotification.icdoDeathNotification.LoadData(ldtbResult.Rows[0]);
                if (!(lobjDeathNotification.icdoDeathNotification.action_status_value == busConstant.DeathNotificationActionStatusCancelled ||
                    lobjDeathNotification.icdoDeathNotification.action_status_value == busConstant.DeathNotificationActionStatusErroneous))
                {
                    return true;
                }
            }
            return false;
        }

        public Collection<busPersonBeneficiary> iclbActiveBeneForGivenPlan { get; set; }

        /// Load all the Beneficiary for the Person
        /// Get all the Beneficiary for the Given Plan
        /// Add all the active Beneficiary on the Given Date to the iclbActiveBeneForGivenPlan Collection
        public void LoadActiveBeneForGivenPlan(int aintPlanID, DateTime adteGivenDate, string astrExcludePlanCheck = null)
        {
            if (iclbActiveBeneForGivenPlan.IsNull())
                iclbActiveBeneForGivenPlan = new Collection<busPersonBeneficiary>();
            //UAT PIR:1244
            busPersonAccount lobjPersonAccount = LoadActivePersonAccountByPlan(aintPlanID);

            // LoadBeneficiary method will Load the Collection group by Plan, Since loaded the DataTable
            DataTable ldtbResult = Select<cdoPersonBeneficiary>(new string[1] { "PERSON_ID" }, new object[1] { icdoPerson.person_id }, null, null);
            foreach (DataRow ldtrRow in ldtbResult.Rows)
            {
                busPersonBeneficiary lobjBeneficiary = new busPersonBeneficiary { icdoPersonBeneficiary = new cdoPersonBeneficiary() };
                lobjBeneficiary.icdoPersonBeneficiary.LoadData(ldtrRow);
                lobjBeneficiary.LoadPersonAccountBeneficiaryData();
                foreach (busPersonAccountBeneficiary lobjPABeneficiary in lobjBeneficiary.iclbPersonAccountBeneficiary)
                {
                    // PIR 8810 - if condition added to exclude Contingent beneficiaries for benefit calculation
                    if (lobjPABeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_value != busConstant.BeneficiaryMemberTypeContingent)
                    {
                        if (lobjPABeneficiary.ibusPersonAccount.IsNull())
                            lobjPABeneficiary.LoadPersonAccount();

                        if ((((lobjPABeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id == aintPlanID)
                            && (lobjPABeneficiary.ibusPersonAccount.icdoPersonAccount.person_account_id == lobjPersonAccount.icdoPersonAccount.person_account_id))
                            || (astrExcludePlanCheck == busConstant.Flag_Yes)) &&
                            (lobjPABeneficiary.icdoPersonAccountBeneficiary.start_date != DateTime.MinValue))
                        {
                            if ((busGlobalFunctions.CheckDateOverlapping(adteGivenDate,
                                lobjPABeneficiary.icdoPersonAccountBeneficiary.start_date,
                                lobjPABeneficiary.icdoPersonAccountBeneficiary.end_date)))
                            {
                                lobjBeneficiary.icdoPersonBeneficiary.beneficiary_percentage = lobjPABeneficiary.icdoPersonAccountBeneficiary.dist_percent;
                                lobjBeneficiary.LoadBeneficiaryAddress();
                                iclbActiveBeneForGivenPlan.Add(lobjBeneficiary);
                            }
                        }
                    }
                }
            }
        }

        // UAT - PIR ID - 937
        // To Determine the screen while checking for the possible duplicate person records
        // 1 - indicates the Person Maintenance
        // 2 - indicates the Person Employment Maintenance
        public int iintScreenIdentifier
        {
            get
            {
                return 1;
            }
        }

        // UAT - PIR ID - 937
        // To Display the Employer name in the Possible Duplicate Person screen.
        public string istrPossibleDuplicateEmployerName { get; set; }

        // UCS-094 - Person Overview Maintenance
        public Collection<busMASStatementFile> iclbMASStatementFile { get; set; }

        public void LoadMASStatementFile()
        {
            if (iclbMASStatementFile.IsNull()) iclbMASStatementFile = new Collection<busMASStatementFile>();
            DataTable ldtbResult = Select("entMASBatchRequest.LoadMASStatementFile", new object[1] { icdoPerson.person_id });
            if (ldtbResult.Rows.Count > 0)
            {
                foreach (DataRow ldtr in ldtbResult.Rows)
                {
                    busMASStatementFile lobjStatementFile = new busMASStatementFile
                    {
                        icdoMasStatementFile = new cdoMasStatementFile(),
                        ibusMASSelection = new busMASSelection
                        {
                            icdoMasSelection = new cdoMasSelection(),
                            ibusBatchRequest = new busMASBatchRequest { icdoMasBatchRequest = new cdoMasBatchRequest() }
                        }
                    };
                    lobjStatementFile.icdoMasStatementFile.LoadData(ldtr);
                    lobjStatementFile.ibusMASSelection.icdoMasSelection.LoadData(ldtr);
                    lobjStatementFile.ibusMASSelection.ibusBatchRequest.icdoMasBatchRequest.LoadData(ldtr);
                    iclbMASStatementFile.Add(lobjStatementFile);
             }
            }
        }

        #region UCS - 079

        //Property to contain payee account with acct. relationship Member
        public busPayeeAccount ibusMemberPayeeAccount { get; set; }
        /// <summary>
        /// Method to load Payee account with acct. relationship value member
        /// </summary>
        public void LoadMemberPayeeAccount()
        {
            if (ibusMemberPayeeAccount == null)
                ibusMemberPayeeAccount = new busPayeeAccount();
            if (iclbPayeeAccount == null)
                LoadPayeeAccount();
            iclbPayeeAccount = busGlobalFunctions.Sort<busPayeeAccount>("icdoPayeeAccount.payee_account_id desc", iclbPayeeAccount);
            ibusMemberPayeeAccount = iclbPayeeAccount.Where(o => o.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember &&
                                                                (o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement ||
                                                                o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))//pir 4015 : as per satya, need to pick up only retr or disa benefit account type
                                                                .FirstOrDefault();
        }

        //PIR 11946
        public void LoadMemberPayeeAccountTransferFromDB(busPersonAccount abusPersonAccount)
        {
            if (ibusMemberPayeeAccount == null)
                ibusMemberPayeeAccount = new busPayeeAccount();
            if (iclbPayeeAccount == null)
                LoadPayeeAccount();

            foreach (busPayeeAccount lbusPayeeAccount in iclbPayeeAccount)
            {
                lbusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            }

            iclbPayeeAccount = busGlobalFunctions.Sort<busPayeeAccount>("icdoPayeeAccount.payee_account_id desc", iclbPayeeAccount);

            if (abusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn)
            {
                iclbPayeeAccount = iclbPayeeAccount.Where(o => (o.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusApproved
                                                                    || o.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusRefundProcessed
                                                                    || o.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusReview) &&
                                                                    o.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember &&
                                                                    o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund &&
                                                                    (o.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToDCTransfer
                                                                        || o.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToDCTransferSpecialElection
                                                                        || o.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToTIAACREFTransfer
                                                                        || o.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDPICTE
                                                                        || o.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionDBToTFFRTransferForDualMembers)
                                                                    ).ToList().ToCollection();
            }
            else
            {
                iclbPayeeAccount = iclbPayeeAccount.Where(o => (o.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusApproved
                                                                    || o.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusRefundProcessed
                                                                    || o.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusReview) &&
                                                                    o.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember &&
                                                                    o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund &&
                                                                    (o.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionAutoRefund
                                                                        || o.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRegularRefund)
                                                                    ).ToList().ToCollection();
            }

            foreach (busPayeeAccount lbusPayeeAccount in iclbPayeeAccount)
            {
                if (lbusPayeeAccount.ibusApplication == null)
                    lbusPayeeAccount.LoadApplication();
                if (lbusPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts == null)
                    lbusPayeeAccount.ibusApplication.LoadBenefitApplicationPersonAccount();
                busBenefitApplicationPersonAccount lobjBAPA = lbusPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts
                    .Where(o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes).FirstOrDefault();

                if (lobjBAPA != null && lobjBAPA.icdoBenefitApplicationPersonAccount.person_account_id == abusPersonAccount.icdoPersonAccount.person_account_id)
                {
                    ibusMemberPayeeAccount = lbusPayeeAccount;
                    break;
                }
            }
        }

        /// <summary>
        /// method to check condtions before initiating Pop up benefit workflow
        /// </summary>
        private void CheckConditionsForWorkflow()
        {
            //checking whether Person marital status is changed from Married to Single or Divorced
            if (Convert.ToString(icdoPerson.ihstOldValues["marital_status_value"]) != icdoPerson.marital_status_value &&
                Convert.ToString(icdoPerson.ihstOldValues["marital_status_value"]) == busConstant.PersonMaritalStatusMarried &&
                (icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced ||
                icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle))
            {
                CheckPayeeAccount();
            }
        }

        /// <summary>
        /// method to check payee account before initiating the Pop up benefit workflow
        /// </summary>
        public void CheckPayeeAccount()
        {
            if (ibusMemberPayeeAccount == null)
                LoadMemberPayeeAccount();
            if (ibusMemberPayeeAccount != null && ibusMemberPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
            {
                ibusMemberPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 =
                    busGlobalFunctions.GetData2ByCodeValue(2203, ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                //prod. pir 4003 : initiate pop up benefits only if payee account status is not in payments complete or canceled status
                if (!ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted() &&
                    !ibusMemberPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                {
                    ibusMemberPayeeAccount.LoadApplication();
                    ibusMemberPayeeAccount.LoadBenfitAccount();
                    //checking whether plan id is not Job Service and Benefit option is 100% JS or 50% JS or RHIC benefit option value is Reduced 100% or 50%
                    if (ibusMemberPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id != Convert.ToInt32(busConstant.Plan_ID_Job_Service) &&
                        (ibusMemberPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption100PercentJS ||
                        ibusMemberPayeeAccount.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption50PercentJS))
                    {
                        InitiateWorkflow();
                    }
                    //Initiating Popup RHIC Amount workflow as per Maik's mail dated July 8, 2015
                    if (ibusMemberPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id != Convert.ToInt32(busConstant.Plan_ID_Job_Service) &&
                        (ibusMemberPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value == busConstant.ApplicationRHICReduced100 ||
                        ibusMemberPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value == busConstant.ApplicationRHICReduced50))
                    {
                        InitiateWorkflow(true);
                    }
                }
            }
        }

        /// <summary>
        /// method to initiate Pop up Benefit Workflow
        /// </summary>
        private void InitiateWorkflow(bool ablnIsPopUpRhic = false)
        {
            int process_id = 0;
            if (!ablnIsPopUpRhic)
                process_id = busConstant.Map_Popup_Payee_Account;
            else
                process_id = busConstant.Map_Popup_RHIC_Amount;
            busWorkflowHelper.InitiateBpmRequest(process_id, icdoPerson.person_id, 0, 0, iobjPassInfo);
        }
        //property to contain collection of benefit applications for joint annuitant
        public Collection<busBenefitApplication> iclbJointAnnuitantApplications { get; set; }
        /// <summary>
        /// Method to load benefit application based on joint annuitant id
        /// </summary>
        internal void LoadBenefitApplicationForJointAnnuitant()
        {
            DataTable ldtBenefitApplication = Select<cdoBenefitApplication>
                (new string[1] { enmBenefitApplication.joint_annuitant_perslink_id.ToString() },
                new object[1] { icdoPerson.person_id }, null, null);
            iclbJointAnnuitantApplications = new Collection<busBenefitApplication>();
            iclbJointAnnuitantApplications = GetCollection<busBenefitApplication>(ldtBenefitApplication, "icdoBenefitApplication");
        }

        /// <summary>
        /// Method to check whether date of birth is changed
        /// </summary>
        private void CheckDateOfBirthChange()
        {
            if (Convert.ToDateTime(icdoPerson.ihstOldValues["date_of_birth"]) != icdoPerson.date_of_birth)
            {
                if (iclbBenefitApplication == null)
                    LoadBenefitApplication();
                if (iclbJointAnnuitantApplications == null)
                    LoadBenefitApplicationForJointAnnuitant();

                busBenefitApplication lobjJointAnnuitantApplication = iclbJointAnnuitantApplications
                                                                        .Where(o => o.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusVerified)
                                                                        .FirstOrDefault();
                busBenefitApplication lobjMemberApplication = iclbBenefitApplication
                                                                        .Where(o => o.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusVerified)
                                                                        .FirstOrDefault();
                if (lobjMemberApplication != null)
                {
                    CheckPayeeAccountAndPaymentDetails(lobjMemberApplication.icdoBenefitApplication.benefit_application_id);
                }
                if (lobjJointAnnuitantApplication != null)
                {
                    CheckPayeeAccountAndPaymentDetails(lobjJointAnnuitantApplication.icdoBenefitApplication.benefit_application_id);
                }
            }
        }

        /// <summary>
        /// Method to check Payee account status and Payment details and initiate the workflow
        /// </summary>
        /// <param name="aintApplicationID">Benefit Application ID</param>
        private void CheckPayeeAccountAndPaymentDetails(int aintApplicationID)
        {
            busBenefitApplication lobjApplication = new busBenefitApplication();
            lobjApplication.FindBenefitApplication(aintApplicationID);
            lobjApplication.LoadPayeeAccount();

            foreach (busPayeeAccount lobjPayeeAccount in lobjApplication.iclbPayeeAccount)
            {
                //Dont check Null condition as LoadActivePayeeAccountStatus and LoadActivePayeeStatusAsofNextBenefitPaymentDate use same bus. object and need to be loaded always
                lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                //Loading the data2 column from DBCache
                DataTable ldtbStatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2203, lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value);
                lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 = ldtbStatus.Rows.Count > 0 ? ldtbStatus.Rows[0]["data2"].ToString() : string.Empty;
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusApproved ||
                    lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReceiving ||
                    lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReview)
                {
                    lobjPayeeAccount.LoadPaymentHistoryHeader();
                    if (lobjPayeeAccount.iclbPaymentHistoryHeader.Where(o => o.icdoPaymentHistoryHeader.status_value == busConstant.PaymentStatusOutstanding
                                                || o.icdoPaymentHistoryHeader.status_value == busConstant.PaymentStatusProcessed).Count() > 0)
                    {
                        InitiateWorkflow(lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id, lobjPayeeAccount.icdoPayeeAccount.payee_account_id);
                    }
                }
            }
        }

        /// <summary>
        /// method to initiate Recalculate pension and RHIC benefit
        /// </summary>
        /// <param name="aintPersonID">Person ID</param>
        /// <param name="aintPayeeAccountID">Payee Account ID</param>
        private void InitiateWorkflow(int aintPersonID, int aintPayeeAccountID)
        {
            if (!busWorkflowHelper.IsActiveInstanceAvailable(aintPersonID, busConstant.Map_Recalculate_Pension_and_RHIC_Benefit))
            {
                busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Recalculate_Pension_and_RHIC_Benefit, aintPersonID, 0, aintPayeeAccountID, iobjPassInfo);
            }
        }

        #endregion


        # region PIR - 1863
        //Load Applications
        //PIR - 2065 2064 1863
        public Collection<busBenefitApplication> iclbBeneficiaryApplication { get; set; }
        public Collection<busBenefitApplication> iclbBeneficiaryApplicationForDisplay { get; set; }
        public void LoadBeneficiaryApplication(bool ablnIsFromDeath)
        {
            Collection<busPersonBeneficiary> lclbPersonBeneficiary = new Collection<busPersonBeneficiary>();

            iclbBeneficiaryApplication = new Collection<busBenefitApplication>();

            if (iclbBenefitApplication.IsNull())
                LoadBenefitApplication();

            var lenumApplication = iclbBenefitApplication.Where(lobjBA => lobjBA.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath
                || lobjBA.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath);

            if (iclbPersonBeneficiaryWithoutPlanAccounts.IsNull())
                LoadBeneficiary();
            foreach (busPersonBeneficiary lobjPerBen in iclbPersonBeneficiaryWithoutPlanAccounts)
            {
                lobjPerBen.ibusPersonAccountBeneficiary.LoadPersonAccount();
            }
            lclbPersonBeneficiary = iclbPersonBeneficiaryWithoutPlanAccounts;

            LoadBeneficiaryApplication(lclbPersonBeneficiary, lenumApplication);
        }

        private void LoadBeneficiaryApplication(Collection<busPersonBeneficiary> lclbPersonBeneficiary, IEnumerable<busBenefitApplication> lenumApplication)
        {
            iclbBeneficiaryApplicationForDisplay = new Collection<busBenefitApplication>();

            foreach (busPersonBeneficiary lobjPersonBene in lclbPersonBeneficiary)
            {
                if (lobjPersonBene.iclbBenefitApplicationBeneficiary.IsNull())
                    lobjPersonBene.LoadBenefitApplicationsBeneficiary();

                foreach (busBenefitApplicationBeneficiary lobjbeneAppl in lobjPersonBene.iclbBenefitApplicationBeneficiary)
                {
                    cdoBenefitApplicationBeneficiary llcdoBeneApplication = lobjbeneAppl.icdoBenefitApplicationBeneficiary;
                    busBenefitApplication lobjBenApp = AssignFieldsAndAddToCollection(lobjPersonBene, llcdoBeneApplication);
                    lobjBenApp.istrBeneficiaryName = lobjPersonBene.ibusBeneficiaryPerson.icdoPerson.FullName;
                    iclbBeneficiaryApplication.Add(lobjBenApp);
                }
            }
            iclbBeneficiaryApplicationForDisplay = iclbBeneficiaryApplication.GroupBy(i => i.iintBeneficiaryid).Select(o => o.FirstOrDefault()).OrderByDescending
                                                    (i => i.idtEndDate == DateTime.MinValue).ThenByDescending(obj => obj.idtEndDate).ToList().ToCollection();
        }

        private busBenefitApplication AssignFieldsAndAddToCollection(busPersonBeneficiary lobjPersonBene, cdoBenefitApplicationBeneficiary lobjbeneAppl)
        {
            bool lblnAddToCollection = false;
            busBenefitApplicationBeneficiary lobjBeneApplication = new busBenefitApplicationBeneficiary { icdoBenefitApplicationBeneficiary = new cdoBenefitApplicationBeneficiary() };
            lobjBeneApplication.icdoBenefitApplicationBeneficiary = lobjbeneAppl;
            busBenefitApplication lobjBenApp = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            if (lobjBeneApplication.icdoBenefitApplicationBeneficiary.benefit_application_id > 0)
            {
                if (lobjBeneApplication.ibusBenefitApplication.IsNull())
                    lobjBeneApplication.LoadBenefitApplication();
                if (lobjBeneApplication.ibusBenefitApplication.ibusRecipient.IsNull())
                    lobjBeneApplication.ibusBenefitApplication.LoadRecipient();
                if (lobjBeneApplication.ibusBenefitApplication.ibusPlan.IsNull())
                    lobjBeneApplication.ibusBenefitApplication.LoadPlan();
            }
            else
            {
                if (lobjBeneApplication.ibusBenefitDROApplication.IsNull())
                    lobjBeneApplication.LoadBenefitDROApplication();
                if (lobjBeneApplication.ibusBenefitDROApplication.ibusPlan.IsNull())
                    lobjBeneApplication.ibusBenefitDROApplication.LoadPlan();
                if (lobjBeneApplication.ibusBenefitDROApplication.ibusBenefitDroCalculation.IsNull())
                    lobjBeneApplication.ibusBenefitDROApplication.LoadDROCalculation();
            }
            if (lobjBeneApplication.icdoBenefitApplicationBeneficiary.benefit_application_id > 0)
            {
                //check if the record is already combined
                var lenum = iclbBeneficiaryApplication.Where(lobjBA => lobjBA.icdoBenefitApplication.benefit_application_id == lobjBeneApplication.icdoBenefitApplicationBeneficiary.benefit_application_id
                                                    && lobjBeneApplication.ibusBenefitApplication.iintBeneficiaryid == lobjbeneAppl.beneficiary_id).Count();
                if (lenum == 0)
                {
                    lblnAddToCollection = true;
                }
            }
            else
            {
                var lenum = iclbBeneficiaryApplication.Where(lobjBA => lobjBA.icdoBenefitApplication.iintDROApplicationId == lobjBeneApplication.icdoBenefitApplicationBeneficiary.dro_application_id
                                               && lobjBeneApplication.ibusBenefitApplication.iintBeneficiaryid == lobjbeneAppl.beneficiary_id).Count();
                if (lenum == 0)
                { lblnAddToCollection = true; }
            }
            if (lblnAddToCollection)
            {
                if (lobjPersonBene.ibusBeneficiaryPerson.IsNull())
                    lobjPersonBene.LoadBeneficiaryPerson();

                if (lobjBeneApplication.icdoBenefitApplicationBeneficiary.benefit_application_id > 0)
                {
                    lobjBenApp.icdoBenefitApplication = lobjBeneApplication.ibusBenefitApplication.icdoBenefitApplication;
                    lobjBenApp.icdoBenefitApplication.iintApplicationId = lobjBeneApplication.icdoBenefitApplicationBeneficiary.benefit_application_id;
                    lobjBenApp.istrPlanName = lobjBeneApplication.ibusBenefitApplication.ibusPlan.icdoPlan.plan_name;
                    lobjBenApp.istrBenefitOption = lobjBeneApplication.ibusBenefitApplication.icdoBenefitApplication.benefit_option_description;
                    if (lobjBeneApplication.ibusBenefitApplication.icdoBenefitApplication.retirement_date == DateTime.MinValue)
                    {
                        if (lobjBeneApplication.ibusBenefitApplication.ibusPerson.IsNull())
                            lobjBeneApplication.ibusBenefitApplication.LoadPerson();
                        lobjBenApp.icdoBenefitApplication.retirement_date = lobjBeneApplication.ibusBenefitApplication.ibusPerson.icdoPerson.date_of_death.AddMonths(1);
                    }
                    else
                        lobjBenApp.icdoBenefitApplication.retirement_date = lobjBeneApplication.ibusBenefitApplication.icdoBenefitApplication.retirement_date;
                }
                else
                {
                    lobjBenApp.icdoBenefitApplication.iintApplicationId = lobjBeneApplication.icdoBenefitApplicationBeneficiary.dro_application_id;
                    lobjBenApp.istrPlanName = lobjBeneApplication.ibusBenefitDROApplication.ibusPlan.icdoPlan.plan_name;
                    lobjBenApp.istrBenefitOption = lobjBeneApplication.ibusBenefitDROApplication.icdoBenefitDroApplication.benefit_duration_option_description;
                    if (!String.IsNullOrEmpty(lobjBeneApplication.ibusBenefitDROApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_type_value))
                        lobjBenApp.icdoBenefitApplication.benefit_account_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, lobjBeneApplication.ibusBenefitDROApplication.GetBenefitType());
                    else
                        lobjBenApp.icdoBenefitApplication.benefit_account_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, busConstant.ApplicationBenefitTypeRetirement);

                    lobjBenApp.icdoBenefitApplication.retirement_date = lobjBeneApplication.ibusBenefitDROApplication.icdoBenefitDroApplication.received_date;
                }

                if (lobjPersonBene.icdoPersonBeneficiary.benificiary_org_id > 0)
                {
                    lobjPersonBene.LoadBeneficiaryInfo();
                    lobjBenApp.istrBenefitciaryOrgCode = lobjPersonBene.ibusBeneficiaryOrganization.icdoOrganization.org_code;
                    lobjBenApp.istrBenefitciaryOrgName = lobjPersonBene.ibusBeneficiaryOrganization.icdoOrganization.org_name;
                }

                lobjBenApp.iintBeneficiaryid = lobjbeneAppl.beneficiary_id;
                lobjBenApp.istrBenefitciaryRelationshipType = lobjPersonBene.icdoPersonBeneficiary.relationship_description;
                lobjBenApp.istrBenefitciaryType = lobjbeneAppl.beneficiary_type_description;
                lobjBenApp.idecBenefitciaryPercentage = lobjbeneAppl.dist_percent;
                lobjBenApp.idtStartDate = lobjbeneAppl.start_date;
                lobjBenApp.idtEndDate = lobjbeneAppl.end_date;
            }
            return lobjBenApp;
        }

        //load beneficiary To tab details
        public Collection<busBenefitApplication> iclbBeneficiaryApplicationForBeneficiaryTo { get; set; }
        public void LoadApplicationsForBeneficiaryTo()
        {
            Collection<busPersonBeneficiary> lclbPersonbeneTo = new Collection<busPersonBeneficiary>();
            iclbBeneficiaryApplicationForBeneficiaryTo = new Collection<busBenefitApplication>();

            if (iclbBeneficiaryTo.IsNull())
                LoadBeneficiaryTo();

            //need to load as it will not be called else where in person maintenance
            LoadApplicantsbenefitApplication();

            LoadBeneficiaryApplicationForBeneficiaryTo();
        }

        private void LoadBeneficiaryApplicationForBeneficiaryTo()
        {
            DataTable ldtbBeneficiaryTo = Select("cdoPerson.LoadPersonAsBeneficiary", new object[1] { _icdoPerson.person_id });

            foreach (DataRow dr in ldtbBeneficiaryTo.Rows)
            {
                busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjPerson.icdoPerson.LoadData(dr);

                busPersonBeneficiary lobjPersonBeneficiary = new busPersonBeneficiary { icdoPersonBeneficiary = new cdoPersonBeneficiary() };
                lobjPersonBeneficiary.icdoPersonBeneficiary.LoadData(dr);

                if (lobjPersonBeneficiary.iclbBenefitApplicationBeneficiary.IsNull())
                    lobjPersonBeneficiary.LoadBenefitApplicationsBeneficiary();

                foreach (busBenefitApplicationBeneficiary lcdoBeneApplication in lobjPersonBeneficiary.iclbBenefitApplicationBeneficiary)
                {
                    cdoBenefitApplicationBeneficiary llcdoBeneApplication = lcdoBeneApplication.icdoBenefitApplicationBeneficiary;
                    busBenefitApplication lobjBenApp = AssignFieldsAndAddToCollection(lobjPersonBeneficiary, llcdoBeneApplication);
                    lobjBenApp.istrBeneficiaryToName = lobjPerson.icdoPerson.FullName;
                    lobjBenApp.iintMemberPerslinkId = lobjPerson.icdoPerson.person_id;
                    iclbBeneficiaryApplicationForBeneficiaryTo.Add(lobjBenApp);
                }
            }
            iclbBeneficiaryApplicationForBeneficiaryTo = iclbBeneficiaryApplicationForBeneficiaryTo.OrderByDescending(i => i.idtEndDate == DateTime.MinValue).ThenByDescending(obj => obj.idtEndDate).ToList().ToCollection();
        }

        public void LoadApplicantsBenefitApplicationWithDRO()
        {
            if (iclbDROApplication.IsNull())
                LoadDROApplications();

            //either the person can be alternate payee or member  for DRo Application
            if ((iclbDROApplication.Count > 0)
                && (iclbMemberDROApplication.Count == 0))
            {
                foreach (busBenefitDroApplication lobjDROApplication in iclbDROApplication)
                {
                    if (lobjDROApplication.ibusBenefitDroCalculation.IsNull())
                        lobjDROApplication.LoadDROCalculation();

                    busBenefitApplication lobjBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                    lobjBenefitApplication.istrBeneficiaryToName = lobjDROApplication.ibusMember.icdoPerson.FullName;
                    lobjBenefitApplication.icdoBenefitApplication.recipient_person_id = icdoPerson.person_id;
                    lobjBenefitApplication.icdoBenefitApplication.plan_id = lobjDROApplication.icdoBenefitDroApplication.plan_id;
                    lobjBenefitApplication.icdoBenefitApplication.benefit_option_description = lobjDROApplication.icdoBenefitDroApplication.benefit_duration_option_description;
                    lobjBenefitApplication.icdoBenefitApplication.iintDROApplicationId = lobjDROApplication.icdoBenefitDroApplication.dro_application_id;
                    // lobjBenefitApplication.icdoBenefitApplication.benefit_application_id = lobjDROApplication.icdoBenefitDroApplication.dro_application_id;
                    lobjBenefitApplication.icdoBenefitApplication.benefit_account_type_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, lobjDROApplication.GetBenefitType());
                    // lobjBenefitApplication.istrPlanName = lobjDROApplication.ibusPlan.icdoPlan.plan_name;
                    lobjBenefitApplication.icdoBenefitApplication.account_relationship_description = busConstant.RelationshipTypeAlternatePayeeDescription;
                    //lobjBenefitApplication.icdoBenefitApplication.retirement_date = lobjDROApplication.icdoBenefitDroApplication.received_date;
                    //lobjBenefitApplication.istrBenefitciaryType = lobjPersonBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.beneficiary_type_description;
                    //lobjBenefitApplication.idecBenefitciaryPercentage = lobjPersonBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.dist_percent;
                    //lobjBenefitApplication.idtStartDate = lobjPersonBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date;
                    //lobjBenefitApplication.idtEndDate = lobjPersonBen.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date;
                    //lobjBenefitApplication.iintBeneficiaryid = lobjPersonBen.icdoPersonBeneficiary.beneficiary_id;
                    iclbApplicantsBenefitApplications.Add(lobjBenefitApplication);
                }
            }
        }

        //load beneficiary To  has been changed as per PIR 1863
        public void LoadBeneficiaryTo()
        {
            iclbBeneficiaryTo = new Collection<busPersonBeneficiary>();

            DataTable ldtbBeneficiaryTo = Select("cdoPerson.LoadPersonAsBeneficiary", new object[1] { _icdoPerson.person_id });

            foreach (DataRow dr in ldtbBeneficiaryTo.Rows)
            {
                busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjPerson.icdoPerson.LoadData(dr);

                lobjPerson.LoadBeneficiary();
                lobjPerson.LoadPersonAccount();

                var lenumBeneTo = lobjPerson.iclbPersonBeneficiary.Where(lobjPersonBen => lobjPersonBen.icdoPersonBeneficiary.beneficiary_person_id == icdoPerson.person_id);

                foreach (busPersonBeneficiary lobjPersonbene in lenumBeneTo)
                {
                    if (lobjPersonbene.ibusPersonAccountBeneficiary.ibusPersonAccount.IsNull())
                        lobjPersonbene.ibusPersonAccountBeneficiary.LoadPersonAccount();

                    var lPersonAccount = lobjPerson.icolPersonAccount.Where(lobjPA => lobjPA.icdoPersonAccount.plan_id
                                                                                == lobjPersonbene.ibusPersonAccountBeneficiary.ibusPersonAccount.icdoPersonAccount.plan_id
                                                                                && busGlobalFunctions.CheckDateOverlapping(lobjPersonbene.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.start_date,
                                                                                lobjPersonbene.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.end_date,
                                                                                lobjPA.icdoPersonAccount.start_date, lobjPA.icdoPersonAccount.end_date));
                    if (lPersonAccount.Count() > 0)
                    {
                        lobjPersonbene.iintBeneficiaryToPersonId = lobjPerson.icdoPerson.person_id;
                        lobjPersonbene.istrBeneficiaryToName = lobjPerson.icdoPerson.FullName;
                        lobjPersonbene.istrPlanName = lPersonAccount.First().ibusPlan.icdoPlan.plan_name;
                        lobjPersonbene.idtInitialPlanStartDate = lPersonAccount.First().icdoPersonAccount.start_date;
                        lobjPersonbene.istrPlanParticipationStatus = lPersonAccount.First().icdoPersonAccount.plan_participation_status_description;
                        iclbBeneficiaryTo.Add(lobjPersonbene);
                    }
                }
            }
        }

        # endregion


        // UAT PIR ID 472
        public bool IsActiveFlexPremiumConversionExists(int aintPlanID)
        {
            if (IsMemberEnrolledInPlan(busConstant.PlanIdFlex))
            {
                busPersonAccountFlexComp lobjFlexAccount = new busPersonAccountFlexComp { icdoPersonAccount = new cdoPersonAccount() };
                lobjFlexAccount.icdoPersonAccount = LoadActivePersonAccountByPlan(busConstant.PlanIdFlex).icdoPersonAccount;
                lobjFlexAccount.LoadFlexCompConversion();
                foreach (busPersonAccountFlexcompConversion lobjPremiumConversion in lobjFlexAccount.iclbFlexcompConversion)
                {
                    if (lobjPremiumConversion.icdoPersonAccountFlexcompConversion.effective_end_date == DateTime.MinValue)
                    {
                        if (lobjPremiumConversion.ibusProvider.IsNull())
                            lobjPremiumConversion.LoadProvider();
                        if (lobjPremiumConversion.ibusProvider.iclbOrgPlan.IsNull())
                            lobjPremiumConversion.ibusProvider.LoadOrgPlan();
                        foreach (busOrgPlan lobjOrgPlan in lobjPremiumConversion.ibusProvider.iclbOrgPlan)
                        {
                            if ((lobjOrgPlan.icdoOrgPlan.plan_id == aintPlanID) &&
                                (busGlobalFunctions.CheckDateOverlapping(
                                        lobjPremiumConversion.icdoPersonAccountFlexcompConversion.effective_start_date,
                                        lobjPremiumConversion.icdoPersonAccountFlexcompConversion.effective_end_date,
                                        lobjOrgPlan.icdoOrgPlan.participation_start_date,
                                        lobjOrgPlan.icdoOrgPlan.participation_end_date)))
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        # region UCS 22 correspondence

        //get LTC Provider name
        //effective at the time  of letter generation
        public string istrLTCCarrierName { get; set; }
        public void LoadLTCProviderName()
        {
            Collection<busOrganization> lclbProviderList = new Collection<busOrganization>();
            DataTable ldtbProviderList = Select<cdoOrganization>(new string[1] { "org_type_value" }, new object[1] { busConstant.OrgTypeProvider }, null, null);
            lclbProviderList = GetCollection<busOrganization>(ldtbProviderList, "icdoOrganization");

            foreach (busOrganization lobjProvider in lclbProviderList)
            {
                lobjProvider.LoadOrganizationOfferedPlans();

                var lenumProviderlist = lobjProvider.iclbOrganizationOfferedPlans.Where(lobjOP => lobjOP.icdoOrgPlan.plan_id == busConstant.PlanIdLTC);

                if (lenumProviderlist.Count() > 0)
                    istrLTCCarrierName = lobjProvider.icdoOrganization.org_name;
            }
        }

        //check if the member is enrolled in DB or DC
        public string istrIsMemberEnrolledInDB { get; set; }
        public string istrIsMemberEnrolledInDC { get; set; }

        public void SetIsMemberEnrolledInDBOrDC()
        {
            istrIsMemberEnrolledInDB = busConstant.Flag_No;
            istrIsMemberEnrolledInDC = busConstant.Flag_No;
            busPersonAccount lobjDCPersonAccount = new busPersonAccount();
            lobjDCPersonAccount = LoadActivePersonAccountByPlan(busConstant.PlanIdDC);
            if (lobjDCPersonAccount.icdoPersonAccount.person_account_id == 0)
            {
                lobjDCPersonAccount = LoadActivePersonAccountByPlan(busConstant.PlanIdDC2020); //PIR 20232
            }
            if (lobjDCPersonAccount.icdoPersonAccount.person_account_id == 0)
            {
                lobjDCPersonAccount = LoadActivePersonAccountByPlan(busConstant.PlanIdDC2025); //PIR 25920
            }
            if (lobjDCPersonAccount.icdoPersonAccount.person_account_id > 0)
                istrIsMemberEnrolledInDC = busConstant.Flag_Yes;
            else
            {
                //check if member is enrolled in the DB
                if (icolPersonAccountByBenefitType.IsNull())
                    LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeRetirement);

                if (icolPersonAccountByBenefitType.Count > 0)
                {
                    var lintCountOfDBPlans = icolPersonAccountByBenefitType.Where(lobjPerAcc => lobjPerAcc.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB ||
                    lobjPerAcc.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB).Count();       //PIR 25920  New DC plan
                    if (lintCountOfDBPlans > 0)
                        istrIsMemberEnrolledInDB = busConstant.Flag_Yes;
                }
            }
        }

        //FOR PER-0951
        public DateTime idtSFN58771ReceivedDate { get; set; }
        public void GetFileNetDateForDoc()
        {
            if (iclbProcessInstance.IsNull())
                LoadProcessInstance();

            if (iclbProcessInstanceImageData.IsNull())
                LoadWorkflowImageData();

            if (iclbProcessInstance.Count > 0 && iclbProcessInstanceImageData.Count > 0)
            {
                var lenumImageForSFN58771 = iclbProcessInstanceImageData.Where(lobj => lobj.icdoBpmProcessInstanceAttachments.doc_type == busConstant.CorTemplateNameSFN58871);

                var ldtInitiatedDate = iclbProcessInstance.Join(lenumImageForSFN58771,
                    ProIns => ProIns.iclbBpmProcessInstance[0].icdoBpmProcessInstance.process_instance_id,
                    Image => Image.icdoBpmProcessInstanceAttachments.bpm_process_instance_id,
                    (ProIns, Image) => new { Image.icdoBpmProcessInstanceAttachments.created_date });
                if (ldtInitiatedDate.IsNotNull())
                    idtSFN58771ReceivedDate = Convert.ToDateTime(ldtInitiatedDate);
            }
        }

        //public Collection<busProcessInstance> iclbProcessInstance { get; set; }
        //public void LoadProcessInstance()
        //{
        //    DataTable ldtbProcessIntance = Select<cdoProcessInstance>(new string[1] { "person_id" }, new object[1] { icdoPerson.person_id }, null, null);
        //    iclbProcessInstance = GetCollection<busProcessInstance>(ldtbProcessIntance, "icdoProcessIntance");
        //}

        //modify entity mapping - venkat
        public Collection<busSolBpmCaseInstance> iclbProcessInstance { get; set; }
        public void LoadProcessInstance()
        {
            DataTable ldtbProcessIntance = Select<doBpmCaseInstance>(new string[1] { "person_id" }, new object[1] { icdoPerson.person_id }, null, null);
            iclbProcessInstance = GetCollection<busSolBpmCaseInstance>(ldtbProcessIntance, "icdoBpmCaseIntance");
        }
        # endregion

        # region RHIC Combine

        //BR-055-149
        //initialize workflow when there is change in the person married status value
        //and person is having the RHIC associated 
        public string istrOldMarriedStatus { get; set; }
        public Collection<busBenefitRhicCombine> iclbBenefitRhicCombine { get; set; }

        public void LoadBenefitRhicCombine(bool ablnLoadOtherObjects = false)
        {
            if (iclbBenefitRhicCombine == null)
                iclbBenefitRhicCombine = new Collection<busBenefitRhicCombine>();
            DataTable ldtbList = Select<cdoBenefitRhicCombine>(
                  new string[1] { "person_id" },
                  new object[1] { icdoPerson.person_id }, null, null);
            iclbBenefitRhicCombine = GetCollection<busBenefitRhicCombine>(ldtbList, "icdoBenefitRhicCombine");

            //UAT PIR 2102
            if (ablnLoadOtherObjects)
            {
                foreach (busBenefitRhicCombine lobjRHICCombine in iclbBenefitRhicCombine)
                {
                    lobjRHICCombine.ibusPerson = this;
                }
            }
        }

        # endregion

        #region UCS - 001

        public busPerson ibusSpouse { get; set; }
        public void LoadSpouse()
        {
            ibusSpouse = new busPerson { icdoPerson = new cdoPerson() };

            if (icolPersonContact == null)
                LoadContacts();
            busPersonContact lobjPersonContact = icolPersonContact.Where(o => o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
                                                                            && o.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive)
                                                                    .FirstOrDefault();

            if (lobjPersonContact.IsNotNull())
            {
                ibusSpouse.FindPerson(lobjPersonContact.icdoPersonContact.contact_person_id);
            }
        }

        public Collection<busPerson> iclbAllSpouse { get; set; }
        public void LoadAllSpouse()
        {
            iclbAllSpouse = new Collection<busPerson>();

            if (icolPersonContact == null)
                LoadContacts();
            var lenuList = icolPersonContact.Where(o => o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse);

            foreach (busPersonContact lbusPersonContact in lenuList)
            {
                if (lbusPersonContact.ibusContactPerson == null)
                    lbusPersonContact.LoadContactPerson();
                iclbAllSpouse.Add(lbusPersonContact.ibusContactPerson);
            }
        }

        #endregion

        # region UCS 24
		//PIR 25990 Possible Duplicate Person screen changes
        public ArrayList btnCallDuplicatePersonScreen_Click()
        {
            ArrayList larrList = new ArrayList();
            //ValidateHardErrors(utlPageMode.All);
            larrList.Add(this);
            return larrList;
        }
        public ArrayList btnRemoveWSSAccess_Click()
        {
            ArrayList larrList = new ArrayList();
            icdoPerson.previous_ndpers_login_id = icdoPerson.ndpers_login_id;
            icdoPerson.ndpers_login_id = null;
            icdoPerson.Update();

            //TODO: Remove the user from PERSLink Group (IBM Secureway)
            EvaluateInitialLoadRules(utlPageMode.All);
            larrList.Add(this);
            return larrList;
        }

        //PIR ID - 20400 - Member Account Lock
        public string istrIsUserLocked
        {
            get {
                return (icdoPerson.is_user_locked == "N" || icdoPerson.is_user_locked.IsNullOrEmpty()) ? busConstant.Flag_No_Value : busConstant.Flag_Yes_Value;
            }
        }
        //public ArrayList btnUnlockWSSAccount_Click()
        //{         
        //    ArrayList larrList = new ArrayList();
        //    this.icdoPerson.is_user_locked = "N";
        //    this.icdoPerson.failed_login_attempt_count = 0;
        //    icdoPerson.Update();
        //    EvaluateInitialLoadRules(utlPageMode.All);
        //    larrList.Add(this);
        //    return larrList;
        //}

   

        public Collection<busWssPersonAccountEnrollmentRequest> iclbWSSEnrollmentRequest { get; set; }
        public void LoadWSSEnrollmentRequest()
        {
            if (iclbWSSEnrollmentRequest.IsNull())
                iclbWSSEnrollmentRequest = new Collection<busWssPersonAccountEnrollmentRequest>();

            DataTable ldtbList = Select<cdoWssPersonAccountEnrollmentRequest>(new string[1] { "person_id" }, new object[1] { icdoPerson.person_id }, null, null);
            iclbWSSEnrollmentRequest = GetCollection<busWssPersonAccountEnrollmentRequest>(ldtbList, "icdoWssPersonAccountEnrollmentRequest");
        }

        public void LoadMSSActiveCOntacts()
        {
            if (_icolPersonContact.IsNull())
                LoadActiveContacts();
            foreach (busPersonContact lobjPersonContact in icolPersonContact)
            {
                lobjPersonContact.LoadMSSContactName();
            }
        }

        public void LoadMSSAddress()
        {
            if (icolPersonAddress.IsNull())
                LoadAddresses();
            foreach (busPersonAddress lobjPersonAddress in icolPersonAddress)
            {
                if (String.IsNullOrEmpty(icdoPerson.peoplesoft_id))
                    lobjPersonAddress.istrAddressWithoutPeopleSoftID = lobjPersonAddress.addr_description;
                else
                    lobjPersonAddress.istrAddressWithPeopleSoftID = lobjPersonAddress.addr_description;
            }
        }
        # endregion

        # region UCS 41 1099 R
        public Collection<busPayment1099r> iclbPayment1099R { get; set; }

        public void LoadPayment1099R()
        {
            iclbPayment1099R = new Collection<busPayment1099r>();
            DataTable ldtb1099RList = Select<cdoPayment1099r>(new string[1] { "person_id" }, new object[1] { icdoPerson.person_id }, null, null);
            iclbPayment1099R = GetCollection<busPayment1099r>(ldtb1099RList, "icdoPayment1099r");
        }

        /// <summary>
        ///   //check if any 1099R exists for this person        
        ///   if so then bypass the second activity in the
        ///   1)Update SSN on person record
        ///   2)Split Person record
        ///   3)Merge Personn record
        /// </summary>
        /// <returns></returns>
        public void SetFlagIs1099RExits()
        {
            icdoPerson.iblnIs1099RExists = false;
            if (iclbPayment1099R.IsNull())
                LoadPayment1099R();
            if (iclbPayment1099R.Count > 0)
                icdoPerson.iblnIs1099RExists = true;
        }

        # endregion

        #region Portal
        public int GrantOnlineAccessForMember(string astrQuestion1Code, string astrAnswer1, string astrQuestion2Code, string astrAnswer2,
                                               string astrNDPERSLoginID, string astrFirstName, string astrLastName)
        {
            if (icdoPerson.date_of_death == DateTime.MinValue)
            {
                if (ibusSpouse == null)
                    LoadSpouse();

                if (ibusPersonCurrentAddress == null)
                    LoadPersonCurrentAddress();

                if (ValidatePortalMemberQuestion(astrQuestion1Code, astrAnswer1))
                {
                    if (ValidatePortalMemberQuestion(astrQuestion2Code, astrAnswer2))
                    {
                        //BR -32 First Name and Last Name also should match
                        //pir 7114 : as per request from Client, need to check only Last Name
                        //if (icdoPerson.first_name.ToLower() == astrFirstName.ToLower() && icdoPerson.last_name.ToLower() == astrLastName.ToLower())
                        //astrLastName.ReplaceWith("[^a-zA-Z0-9]","");
                        if (icdoPerson.last_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "") == astrLastName.ToLower().ReplaceWith("[^a-zA-Z0-9]", ""))
                        {
                            icdoPerson.previous_ndpers_login_id = icdoPerson.ndpers_login_id;
                            icdoPerson.ndpers_login_id = astrNDPERSLoginID;
                            icdoPerson.Update();
                            GenerateCorrespondenceForSuccessfulRegistration();
                            return 0;
                        }
                        else
                            return 1;
                    }
                    else
                        return 2;
                }
                else
                    return 3;
            }
            else
                return 4;
        }

        private bool ValidatePortalMemberQuestion(string astrQuestionCode, string astrAnswer)
        {
            bool lblnResult = false;
            DateTime ldtTempDate;
            bool lblnDateConversionSucceed = DateTime.TryParse(astrAnswer, out ldtTempDate);
            decimal ldecNetBenefitAnswerAmount;
            bool lblnNetBenefitAmtConversionSucceed = Decimal.TryParse(astrAnswer, out ldecNetBenefitAnswerAmount);
            switch (astrQuestionCode)
            {
                case busConstant.Question_DateOfBirth:
                    if (lblnDateConversionSucceed)
                    {
                        if (icdoPerson.date_of_birth == ldtTempDate)
                            lblnResult = true;
                    }
                    break;
                case busConstant.Question_DateOfBirthForSpouse:
                    if (lblnDateConversionSucceed)
                    {
                        if (ibusSpouse.icdoPerson.date_of_birth == ldtTempDate)
                            lblnResult = true;
                    }
                    break;
                case busConstant.Question_MostRecentNetBenefitAmount:
                    if (lblnNetBenefitAmtConversionSucceed)
                    {
                        decimal ldecRecentNetBenefitAmount = LoadRecentNetBenefitAmount();
                        if ((ldecRecentNetBenefitAmount > 0) && (ldecRecentNetBenefitAmount == ldecNetBenefitAnswerAmount))
                        {
                            lblnResult = true;
                        }
                    }
                    break;
                case busConstant.Question_SpouseSSN:
                    if (ibusSpouse.icdoPerson.ssn == astrAnswer.Replace("-", ""))
                        lblnResult = true;
                    break;
                //MSS Pir 7114
                case busConstant.Question_SSN:
                    if (!string.IsNullOrEmpty(icdoPerson.ssn) && icdoPerson.ssn.Length > 4 && icdoPerson.ssn.Right(4) == astrAnswer)
                        lblnResult = true;
                    break;
                case busConstant.Question_CurrentAddressZipCode:
                    if (ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_zip_code == astrAnswer)
                        lblnResult = true;
                    break;
            }
            return lblnResult;
        }
        //pir 7708
        public void GenerateCorrespondenceForSuccessfulRegistration()
        {
            ArrayList larrlist = new ArrayList();
            larrlist.Add(this);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence("PER-0957", this, lhstDummyTable);
            if (lobjCorresPondenceInfo.istrGeneratedFileName.IsNotNull()) // PIR 10400
            {
                Sagitec.CorBuilder.CorBuilderXML lobjCorBuilder = new Sagitec.CorBuilder.CorBuilderXML(); //PIR 16947 - Change to CorBuilderXML class
                lobjCorBuilder.InstantiateWord();
                lobjCorBuilder.CreateCorrespondenceFromTemplate("PER-0957", lobjCorresPondenceInfo, iobjPassInfo.istrUserID);
                lobjCorBuilder.CloseWord();
            }
        }
        private decimal LoadRecentNetBenefitAmount()
        {
            decimal ldecAmount = 0.00M;
            DataTable ldtbList = Select("cdoPerson.LoadRecentNetBenefitAmount",
                       new object[1] { icdoPerson.person_id });
            if (ldtbList.Rows.Count > 0)
            {
                ldecAmount = Convert.ToDecimal(ldtbList.Rows[0]["NETPAYMENT"]);
            }
            return ldecAmount;
        }
        #endregion

        //LoadContributionForPerson      
        public Collection<busPersonAccountRetirementContribution> iclbJobServiceRetirementContribution { get; set; }
        public Collection<busPersonAccountRetirementContribution> iclbRetirementContributionExcludeJobService { get; set; }
        //LoadRetContributionForJobService
        public void LoadJobServiceRetContritbution()
        {
            iclbJobServiceRetirementContribution = new Collection<busPersonAccountRetirementContribution>();
            DataTable ldtbList = Select("cdoPerson.LoadRetContributionForJobService", new object[1] { icdoPerson.person_id });
            iclbJobServiceRetirementContribution = GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");
        }
        public void LoadRetContritbutionExcludeJobService()
        {
            iclbRetirementContributionExcludeJobService = new Collection<busPersonAccountRetirementContribution>();
            DataTable ldtbList = Select("cdoPerson.LoadRetContributionExcludeJobService", new object[1] { icdoPerson.person_id });
            iclbRetirementContributionExcludeJobService = GetCollection<busPersonAccountRetirementContribution>(ldtbList, "icdoPersonAccountRetirementContribution");
        }

        #region UCS - 032

        public busPersonEmployment ibusESSPersonEmployment { get; set; }

        public busPersonAccount ibusESSPersonAccount { get; set; }

        public busPersonAccountDeferredComp ibusESSPersonAccountDeffComp { get; set; }
        public busPersonAccountDeferredComp ibusESSPersonAccountDeffCompOther457 { get; set; }

        public busPersonAccountGhdv ibusESSPersonAccountGhdv { get; set; }

        public busPersonAccountGhdv ibusESSPersonAccountHealthGhdv { get; set; }
		
        public busPersonAccountGhdv ibusESSPersonAccountDentalGhdv { get; set; }
		
        public busPersonAccountGhdv ibusESSPersonAccountVisionGhdv { get; set; }

        public busPersonAccountLife ibusESSPersonAccountGroupLife { get; set; }

        public busPersonAccountEAP ibusESSPersonAccountEAP { get; set; }

        public busPersonAccountLtc ibusESSPersonAccountLtc { get; set; }

        public busPersonAccountFlexComp ibusESSPersonAccountFlexComp { get; set; }

        public busPersonAccount ibusESSPersonAccountRetirement { get; set; }


        public int iintOrgID { get; set; }

        public int iintPlanID { get; set; }

        public int iintContactID { get; set; }

        public void ESSLoadPersonEmployment()
        {
            DataTable ldtEmployment = Select<cdoPersonEmployment>(new string[2] { "person_id", "org_id" },
                                                                new object[2] { icdoPerson.person_id, iintOrgID }, null, "start_date DESC");
            ibusESSPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
            ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            if (ldtEmployment.Rows.Count > 0)
            {
                ibusESSPersonEmployment.icdoPersonEmployment.LoadData(ldtEmployment.Rows[0]);
                ibusESSPersonEmployment.ESSLoadPersonEmploymentDetail();
                if (ibusESSPersonEmployment.icolPersonEmploymentDetail.Count > 0)
                    ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail = ibusESSPersonEmployment.icolPersonEmploymentDetail[0];
            }
        }
        public Collection<busPersonAccount> icolPersonEnrolledPlans { get; set; }

        public void ESSLoadPersonAccountForEnrolledPlans()
        {
            DataTable ldtbEnrolledPlans = Select("cdoPersonAccount.GetEnrolledPlansForPersonID", new object[2] { icdoPerson.person_id, iintOrgID }); //PIR-18010
            icolPersonEnrolledPlans = GetCollection<busPersonAccount>(ldtbEnrolledPlans, "icdoPersonAccount");
            if (icolPersonEnrolledPlans.IsNull())
                icolPersonEnrolledPlans = new Collection<busPersonAccount>();
            foreach (busPersonAccount lbusPersonAccount in icolPersonEnrolledPlans)
            {
                switch (lbusPersonAccount.icdoPersonAccount.plan_id)
                {
                    case busConstant.PlanIdGroupHealth:
                        ibusESSPersonAccountHealthGhdv = new busPersonAccountGhdv { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                        if (ibusESSPersonAccountHealthGhdv.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            ibusESSPersonAccountHealthGhdv.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                            ibusESSPersonAccountHealthGhdv.LoadPlan();
                            ibusESSPersonAccountHealthGhdv.ESSLoadProviderOrgName(iintOrgID);
                            ibusESSPersonAccountHealthGhdv.LoadPlanEffectiveDate();
                            ibusESSPersonAccountHealthGhdv.DetermineEnrollmentAndLoadObjects(ibusESSPersonAccountHealthGhdv.idtPlanEffectiveDate, false);
                            ibusESSPersonAccountHealthGhdv.LoadPersonAccountGHDVHsaFutureDated();
                            if (ibusESSPersonAccountHealthGhdv.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                            {
                                ibusESSPersonAccountHealthGhdv.LoadRateStructureForUserStructureCode();
                            }
                            else
                            {
                                //Load the Health Plan Participation Date (based on effective Date)
                                ibusESSPersonAccountHealthGhdv.LoadHealthParticipationDate();
                                //To Get the Rate Structure Code (Derived Field)
                                ibusESSPersonAccountHealthGhdv.LoadRateStructure();
                            }
                            //Get the Coverage Ref ID
                            ibusESSPersonAccountHealthGhdv.LoadCoverageRefID();
                            ibusESSPersonAccountHealthGhdv.GetMonthlyPremiumAmountByRefID();
                            iblnHealth = true;
                            ibusESSPersonAccountHealthGhdv.LoadCoverageCodeByFilter();
                            ibusESSPersonAccountHealthGhdv.ESSLoadCoverageCodeDescription();
                        }
                        break;
                    case busConstant.PlanIdDental:
                        ibusESSPersonAccountDentalGhdv = new busPersonAccountGhdv { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                        if (ibusESSPersonAccountDentalGhdv.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            ibusESSPersonAccountDentalGhdv.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                            ibusESSPersonAccountDentalGhdv.LoadPlan();
                            ibusESSPersonAccountDentalGhdv.LoadPlanEffectiveDate();
                            ibusESSPersonAccountDentalGhdv.DetermineEnrollmentAndLoadObjects(ibusESSPersonAccountDentalGhdv.idtPlanEffectiveDate, false);
                            ibusESSPersonAccountDentalGhdv.GetMonthlyPremiumAmount();
                            ibusESSPersonAccountDentalGhdv.ESSLoadCoverageCodeDescription();
                            ibusESSPersonAccountDentalGhdv.ESSLoadProviderOrgName(iintOrgID);
                            iblnDental = true;
                            ibusESSPersonAccountDentalGhdv.icdoPersonAccountGhdv.premium_conversion_indicator_flag =
                                ibusESSPersonAccountDentalGhdv.icdoPersonAccountGhdv.premium_conversion_indicator_flag == busConstant.Flag_Yes ? busConstant.Flag_Yes_Value : busConstant.Flag_No_Value;
                        }
                        break;
                    case busConstant.PlanIdVision:
                        ibusESSPersonAccountVisionGhdv = new busPersonAccountGhdv { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                        if (ibusESSPersonAccountVisionGhdv.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            ibusESSPersonAccountVisionGhdv.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                            ibusESSPersonAccountVisionGhdv.LoadPlan();
                            ibusESSPersonAccountVisionGhdv.LoadPlanEffectiveDate();
                            ibusESSPersonAccountVisionGhdv.DetermineEnrollmentAndLoadObjects(ibusESSPersonAccountVisionGhdv.idtPlanEffectiveDate, false);
                            ibusESSPersonAccountVisionGhdv.GetMonthlyPremiumAmount();
                            ibusESSPersonAccountVisionGhdv.ESSLoadCoverageCodeDescription();
                            ibusESSPersonAccountVisionGhdv.ESSLoadProviderOrgName(iintOrgID);
                            iblnVision = true;
                            ibusESSPersonAccountVisionGhdv.icdoPersonAccountGhdv.premium_conversion_indicator_flag =
                                ibusESSPersonAccountVisionGhdv.icdoPersonAccountGhdv.premium_conversion_indicator_flag == busConstant.Flag_Yes ? busConstant.Flag_Yes_Value : busConstant.Flag_No_Value;

                        }
                        break;
                    case busConstant.PlanIdGroupLife:
                        ibusESSPersonAccountGroupLife = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife(), icdoPersonAccount = new cdoPersonAccount() };
                        if (ibusESSPersonAccountGroupLife.FindPersonAccountLife(lbusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            ibusESSPersonAccountGroupLife.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                            ibusESSPersonAccountGroupLife.LoadLifeOption();
                            ibusESSPersonAccountGroupLife.LoadLifeOptionData();
                            //PIR 13881                           
                            ibusESSPersonAccountGroupLife.LoadLifeHistoryData(iintOrgID);                           
                            ibusESSPersonAccountGroupLife.ESSLoadProviderOrgName(iintOrgID);
                            ibusESSPersonAccountGroupLife.LoadPlanEffectiveDate();
                            ibusESSPersonAccountGroupLife.LoadMemberAge(ibusESSPersonAccountGroupLife.idtPlanEffectiveDate);
                            ibusESSPersonAccountGroupLife.LoadPlan();
                            ibusESSPersonAccountGroupLife.icdoPersonAccount.person_employment_dtl_id = ibusESSPersonAccountGroupLife.GetEmploymentDetailID();
                            if (ibusESSPersonAccountGroupLife.icdoPersonAccount.person_employment_dtl_id != 0)
                            {
                                ibusESSPersonAccountGroupLife.LoadPersonEmploymentDetail();
                                ibusESSPersonAccountGroupLife.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                //PIR 2052 : Load the Org Plan by Plan Effective Date (Transfer Employment Scenario)
                                ibusESSPersonAccountGroupLife.LoadOrgPlan(ibusESSPersonAccountGroupLife.idtPlanEffectiveDate);
                                ibusESSPersonAccountGroupLife.LoadProviderOrgPlan(ibusESSPersonAccountGroupLife.idtPlanEffectiveDate);
                            }
                            else
                            {
                                ibusESSPersonAccountGroupLife.LoadActiveProviderOrgPlan(ibusESSPersonAccountGroupLife.idtPlanEffectiveDate);
                            }
                            if (ibusESSPersonAccountGroupLife.icdoPersonAccountLife.premium_waiver_flag != busConstant.Flag_Yes)
                            {
                                ibusESSPersonAccountGroupLife.GetMonthlyPremiumAmount();                                
                                //PIR 13881
                                ibusESSPersonAccountGroupLife.GetMonthlyPremiumAmount_ForLifeHistory();
                            }
                            ibusESSPersonAccountGroupLife.icdoPersonAccountLife.premium_conversion_indicator_flag =
                                ibusESSPersonAccountGroupLife.icdoPersonAccountLife.premium_conversion_indicator_flag == busConstant.Flag_Yes ? busConstant.Flag_Yes_Value : busConstant.Flag_No_Value;
                            iblnLife = true;
                        }
                        break;
                    case busConstant.PlanIdEAP:
                        ibusESSPersonAccountEAP = new busPersonAccountEAP { icdoPersonAccount = new cdoPersonAccount() };
                        if (ibusESSPersonAccountEAP.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            ibusESSPersonAccountEAP.LoadPlanEffectiveDate();
                            ibusESSPersonAccountEAP.LoadOrgPlan(ibusESSPersonAccountEAP.idtPlanEffectiveDate);
                            ibusESSPersonAccountEAP.LoadProviderOrgPlanByProviderOrgID(ibusESSPersonAccountEAP.icdoPersonAccount.provider_org_id, ibusESSPersonAccountEAP.idtPlanEffectiveDate);
                            ibusESSPersonAccountEAP.GetMonthlyPremium();
                            ibusESSPersonAccountEAP.LoadPlan();
                            ibusESSPersonAccountEAP.LoadProvider();
                            iblnEAP = true;
                        }
                        break;
                    case busConstant.PlanIdFlex:
                        ibusESSPersonAccountFlexComp = new busPersonAccountFlexComp { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp() };
                        if (ibusESSPersonAccountFlexComp.FindPersonAccountFlexComp(lbusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            ibusESSPersonAccountFlexComp.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                            ibusESSPersonAccountFlexComp.LoadPlan();
                            ibusESSPersonAccountFlexComp.LoadFlexCompOptionUpdate();
                            ibusESSPersonAccountFlexComp.LoadFlexCompConversion();
                            ibusESSPersonAccountFlexComp.LoadFlexCompIndividualOptions();
                            ibusESSPersonAccountFlexComp.icdoPersonAccountFlexComp.premium_conversion_waiver_flag =
                                ibusESSPersonAccountFlexComp.icdoPersonAccountFlexComp.premium_conversion_waiver_flag == busConstant.Flag_Yes ? busConstant.Flag_Yes_Value : busConstant.Flag_No_Value;
                            iblnFlex = true;
                            ibusESSPersonAccountFlexComp.iclbFlexcompConversion =
                                ibusESSPersonAccountFlexComp.iclbFlexcompConversion.Where(o => (o.icdoPersonAccountFlexcompConversion.effective_start_date >= DateTime.Today.Date) || ((o.icdoPersonAccountFlexcompConversion.effective_start_date != DateTime.MinValue) && 
                                         (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, o.icdoPersonAccountFlexcompConversion.effective_start_date, o.icdoPersonAccountFlexcompConversion.effective_end_date)))).ToList().ToCollection();
                        }
                        break;
                    case busConstant.PlanIdLTC:
                        ibusESSPersonAccountLtc = new busPersonAccountLtc { icdoPersonAccount = new cdoPersonAccount() };
                        if (ibusESSPersonAccountLtc.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            iblnLTC = true;
                            ibusESSPersonAccountLtc.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                            ibusESSPersonAccountLtc.LoadPlan();
                            ibusESSPersonAccountLtc.LoadPlanEffectiveDate();
                            ibusESSPersonAccountLtc.icdoPersonAccount.person_employment_dtl_id = ibusESSPersonAccountLtc.GetEmploymentDetailID();
                            if (ibusESSPersonAccountLtc.icdoPersonAccount.person_employment_dtl_id != 0)
                            {
                                ibusESSPersonAccountLtc.LoadPersonEmploymentDetail();
                                ibusESSPersonAccountLtc.ibusPersonEmploymentDetail.LoadPersonEmployment();
                                ibusESSPersonAccountLtc.LoadOrgPlan(ibusESSPersonAccountLtc.idtPlanEffectiveDate);
                                ibusESSPersonAccountLtc.LoadProviderOrgPlan(ibusESSPersonAccountLtc.idtPlanEffectiveDate);
                            }
                            else
                            {
                                ibusESSPersonAccountLtc.LoadActiveProviderOrgPlan(ibusESSPersonAccountLtc.idtPlanEffectiveDate);
                            }
                            ibusESSPersonAccountLtc.LoadLtcOptionUpdateMember();
                            ibusESSPersonAccountLtc.LoadLtcOptionUpdateSpouse();
                            ibusESSPersonAccountLtc.iclbLtcOptionMember =
                            ibusESSPersonAccountLtc.iclbLtcOptionMember.Concat(ibusESSPersonAccountLtc.iclbLtcOptionSpouse)
                            .Where(o => o.icdoPersonAccountLtcOption.plan_option_status_value == busConstant.PlanOptionStatusValueEnrolled).ToList().ToCollection();
                            ibusESSPersonAccountLtc.GetMonthlyPremiumAmount();
                        }
                        break;
                    case busConstant.PlanIdDeferredCompensation:
                        //case busConstant.PlanIdOther457:
                        ibusESSPersonAccountDeffComp = new busPersonAccountDeferredComp { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp() };
                        if (ibusESSPersonAccountDeffComp.FindPersonAccountDeferredComp(lbusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            iblnDefComp = true;
                            ibusESSPersonAccountDeffComp.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                            ibusESSPersonAccountDeffComp.LoadPlan();
                            ibusESSPersonAccountDeffComp.LoadActivePersonAccountProviders();
                            //ibusESSPersonAccountDeffComp.icolPersonAccountDeferredCompProvider =
                            //    ibusESSPersonAccountDeffComp.icolPersonAccountDeferredCompProvider
                            //    .Where(o => o.icdoPersonAccountDeferredCompProvider.provider_org_plan_id != busConstant.NDPERSProviderOrgPlanID).ToList().ToCollection();
                        }
                        break;                    
                    case busConstant.PlanIdOther457:
                        ibusESSPersonAccountDeffCompOther457 = new busPersonAccountDeferredComp { icdoPersonAccount = new cdoPersonAccount(), icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp() };
                        if (ibusESSPersonAccountDeffCompOther457.FindPersonAccountDeferredComp(lbusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            iblnOth457Plan = true;
                            ibusESSPersonAccountDeffCompOther457.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                            ibusESSPersonAccountDeffCompOther457.LoadPlan();
                            ibusESSPersonAccountDeffCompOther457.LoadActivePersonAccountProviders();
                            foreach (busPersonAccountDeferredCompProvider lobjDefProvider in ibusESSPersonAccountDeffCompOther457.icolPersonAccountDeferredCompProvider)
                            {
                                if (lobjDefProvider.icdoPersonAccountDeferredCompProvider.end_date_no_null > DateTime.Today)
                                {
                                    ibusESSPersonAccountDeffCompOther457.idecCurrentPayPeriodAmount += lobjDefProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                                }
                            }

                        }
                        break;
                    case busConstant.PlanIdMain:
                    case busConstant.PlanIdLE:
                    case busConstant.PlanIdNG:
                    case busConstant.PlanIdHP:
                    case busConstant.PlanIdJudges:
                    case busConstant.PlanIdJobService:
                    case busConstant.PlanIdDC:
                    case busConstant.PlanIdLEWithoutPS:
                    case busConstant.PlanIdOasis:
                    case busConstant.PlanIdJobService3rdPartyPayor:
                    case busConstant.PlanIdPriorJudges:
                    case busConstant.PlanIdPriorService:
                    case busConstant.PlanIdBCILawEnf:
                    case busConstant.PlanIdMain2020:
                    case busConstant.PlanIdDC2020: //PIR 20232
                    case busConstant.PlanIdStatePublicSafety: //PIR 25729
                    case busConstant.PlanIdDC2025:		//PIR 25920 New DC Plan
                        ibusESSPersonAccountRetirement = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                        if (ibusESSPersonAccountRetirement.FindPersonAccount(lbusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            iblnRetirement = true;
                            ibusESSPersonAccountRetirement.LoadPlan();
                            //PIR 25920 DC 2025 changes
                            int lintperson_account_retirement_history_id = 0;
                            ibusESSPersonAccountRetirement.LoadPersonAccountRetirement();
                            ibusESSPersonAccountRetirement.ibusPersonAccountRetirement.LoadPreviousHistory();
                            lintperson_account_retirement_history_id = ibusESSPersonAccountRetirement.ibusPersonAccountRetirement.ibusHistory.icdoPersonAccountRetirementHistory.person_account_retirement_history_id;
                            DataTable ldtLatestHistory = Select<cdoPersonAccountRetirementHistory>(new string[1] { enmPersonAccountRetirementHistory.person_account_retirement_history_id.ToString() },
                                       new object[1] { lintperson_account_retirement_history_id }, null, null);
                            if (ldtLatestHistory.IsNotNull() && ldtLatestHistory.Rows.Count > 0 && Convert.ToString(ldtLatestHistory.Rows[0][enmPersonAccountRetirementHistory.addl_ee_contribution_percent.ToString()]).IsNullOrEmpty())
                            {
                                iblnESSShowAddlEEContributionPercent = false;
                            }
                            else
                            {
                                iblnESSShowAddlEEContributionPercent = true;
                                iintESSAddlEEContributionPercent = ibusESSPersonAccountRetirement.ibusPersonAccountRetirement.ibusHistory.icdoPersonAccountRetirementHistory.addl_ee_contribution_percent;
                            }
                        }
                        break;
                }
            }

        }
        public void ESSLoadPersonAccount()
        {
            if (icolPersonAccount == null)
                LoadPersonAccount();
            ibusESSPersonAccount = icolPersonAccount.Where(o => o.icdoPersonAccount.plan_id == iintPlanID && (!o.IsWithDrawn())).FirstOrDefault();
            if (ibusESSPersonAccount.IsNull())
                ibusESSPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
        }

        public void ESSLoadPersonAccountDeffComp()
        {
            if (ibusESSPersonAccount == null)
                ESSLoadPersonAccount();
            ibusESSPersonAccountDeffComp = new busPersonAccountDeferredComp { icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp() };
            ibusESSPersonAccountDeffComp.FindPersonAccountDeferredComp(ibusESSPersonAccount.icdoPersonAccount.person_account_id);
        }

        #endregion

        // BR-041-03
        public bool IsPersonDuplicated()
        {
            if (!iblnIsFromPs && !iblnIsFromESS)
            {
                if ((icdoPerson.last_name.IsNotNullOrEmpty()) &&
                    (icdoPerson.date_of_birth != DateTime.MinValue) &&
                    (icdoPerson.gender_value.IsNotNullOrEmpty()) &&
                    (icdoPerson.ssn.IsNotNullOrEmpty()))
                {
                    busDuplicatePersonScreen lobjDuplicatePerson = new busDuplicatePersonScreen();
                    lobjDuplicatePerson.LoadDuplicatePersons(icdoPerson.person_id, icdoPerson.last_name, icdoPerson.date_of_birth, icdoPerson.gender_value, 1, 0, icdoPerson.ssn);
                    if (lobjDuplicatePerson.iclbDuplicatePersons.Count > 0)
                        return true;
                }
            }
            return false;
        }

        public Collection<busPerson> iclbSpouseUnionSpouseTo { get; set; }
        public void LoadSpouseUnionSpouseTo()
        {
            iclbSpouseUnionSpouseTo = new Collection<busPerson>();
            //Load Spouse that is added in contact of current person
            if (iclbAllSpouse == null)
                LoadAllSpouse();

            if (iclbPersonContactTo == null)
                LoadPersonContactTo();

            var lclbSpouseContactTo = iclbPersonContactTo.Where(i => i.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse);

            foreach (busPerson lbusSpouse in iclbAllSpouse)
                iclbSpouseUnionSpouseTo.Add(lbusSpouse);

            if (lclbSpouseContactTo != null)
            {
                foreach (busPersonContact lbusPersonContact in lclbSpouseContactTo)
                {
                    if (lbusPersonContact.ibusPerson == null)
                        lbusPersonContact.LoadPerson();

                    if (!iclbSpouseUnionSpouseTo.Any(i => i.icdoPerson.person_id == lbusPersonContact.ibusPerson.icdoPerson.person_id))
                        iclbSpouseUnionSpouseTo.Add(lbusPersonContact.ibusPerson);
                }
            }
        }

        //Load Donors (Different Combinations) 
        //BR -055 132 Point No 1
        public Collection<busPayeeAccount> LoadActiveDBRetirementDisablityPayeeAccount()
        {
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();

            if (iclbPayeeAccount == null)
                LoadPayeeAccount();

            foreach (busPayeeAccount lbusPayeeAccount in iclbPayeeAccount)
            {
                if (lbusPayeeAccount.ibusPlan == null)
                    lbusPayeeAccount.LoadPlan();

                if (lbusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                    lbusPayeeAccount.LoadActivePayeeStatus();

                if ((lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement ||
                   lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
                   (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted()) &&
                   (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled()) && 
                   (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusSuspended()) && 
                   lbusPayeeAccount.ibusPlan.IsDBRetirementPlan() && lbusPayeeAccount.icdoPayeeAccount.rhic_amount > 0)
                {
                    lclbPayeeAccount.Add(lbusPayeeAccount);
                }
            }
            return lclbPayeeAccount;
        }

        //BR -055 132 Point No 2
        public Collection<busPayeeAccount> LoadActiveDCRetirementDisablityPayeeAccount()
        {
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();

            if (iclbPayeeAccount == null)
                LoadPayeeAccount();

            foreach (busPayeeAccount lbusPayeeAccount in iclbPayeeAccount)
            {
                if (lbusPayeeAccount.ibusPlan == null)
                    lbusPayeeAccount.LoadPlan();

                if (lbusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                    lbusPayeeAccount.LoadActivePayeeStatus();

                if ((lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) &&
                   (!(lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted() && icdoPerson.date_of_death != DateTime.MinValue)) &&
                   (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled()) && 
                   (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusSuspended()) &&
                   (lbusPayeeAccount.ibusPlan.IsDCRetirementPlan() || lbusPayeeAccount.ibusPlan.IsHBRetirementPlan()) &&
                   lbusPayeeAccount.icdoPayeeAccount.rhic_amount > 0)
                {
                    lclbPayeeAccount.Add(lbusPayeeAccount);
                }
            }
            return lclbPayeeAccount;
        }

        //In the case automatic combine, we may not consider whether rhic is being apply to health or not.. but, when the user creates the record, we need to consider that too.
        //Basically an active spouse scenario, when the person health record is suspended, and approved rhic record is exists, system should initiate workflow when the spouse as recceiver.
        //so that, user will end that record before creating the rhic record for the spouse receiver.
        public Collection<busPayeeAccount> LoadActiveSpousePayeeAccount(busPerson abusSpouse, DateTime adtEffectiveDate, bool ablnIsEstimate = false, bool ablnIsAutomaticCombine = false)
        {
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
            if (abusSpouse.icdoPerson.person_id > 0)
            {
                if (abusSpouse.icdoPerson.date_of_death == DateTime.MinValue)
                {
                    if (abusSpouse.iclbPayeeAccount == null)
                        abusSpouse.LoadPayeeAccount();

                    Collection<busPayeeAccount> lclbSpouseActiveDBPayeeAccount = abusSpouse.LoadActiveDBRetirementDisablityPayeeAccount();
                    Collection<busPayeeAccount> lclbSpouseActiveDCPayeeAccount = abusSpouse.LoadActiveDCRetirementDisablityPayeeAccount();

                    var lclbSpouseActiveRetrDisaPayeeAccount = lclbSpouseActiveDBPayeeAccount.Union<busPayeeAccount>(lclbSpouseActiveDCPayeeAccount);

                    foreach (busPayeeAccount lbusPayeeAccount in lclbSpouseActiveRetrDisaPayeeAccount)
                    {
                        if (lbusPayeeAccount.ibusPlan == null)
                            lbusPayeeAccount.LoadPlan();

                        lbusPayeeAccount.ibusPayee = abusSpouse;

                        if (lbusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                            lbusPayeeAccount.LoadActivePayeeStatus();

                        if ((lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement ||
                            lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
                            ((!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted()) || (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled()) ||
                            (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusSuspended())) &&
                            lbusPayeeAccount.icdoPayeeAccount.rhic_amount > 0)
                        {
                            //Systest PIR : 2582 , For Estimate, We dont need to do Health Check
                            //if (ablnIsEstimate)
                            //{
                                lclbPayeeAccount.Add(lbusPayeeAccount);
                            //}
                            //else PIR 14346 change, we do not need the health check anymore
                            //{
                            //    if (ibusHealthPersonAccount == null)
                            //        LoadHealthPersonAccount();
                            //    if (ibusHealthPersonAccount.ibusPersonAccountGHDV == null)
                            //        ibusHealthPersonAccount.LoadPersonAccountGHDV();
                            //    if (ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                            //    {
                            //        if (!ibusHealthPersonAccount.ibusPersonAccountGHDV.IsCoverageCodeCodeSingle())
                            //        {
                            //            if (iclbPersonDependent == null)
                            //                LoadDependent();

                            //            busPersonDependent lbusSpouseDependent = iclbPersonDependent
                            //                                                    .FirstOrDefault(i => i.icdoPersonDependent.dependent_perslink_id == abusSpouse.icdoPerson.person_id);
                            //            if (lbusSpouseDependent != null)
                            //            {
                            //                if (lbusSpouseDependent.iclbPersonAccountDependent == null)
                            //                    lbusSpouseDependent.LoadPersonAccountDependent();

                            //                foreach (busPersonAccountDependent lbusPADependent in lbusSpouseDependent.iclbPersonAccountDependent)
                            //                {
                            //                    if (lbusPADependent.ibusPersonAccount == null)
                            //                        lbusPADependent.LoadPersonAccount();
                            //                }

                            //                busPersonAccountDependent lbusSpousePADependent = lbusSpouseDependent.iclbPersonAccountDependent
                            //                    .FirstOrDefault(i => i.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth);

                            //                if (lbusSpousePADependent != null)
                            //                {
                            //                    if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                            //                        lbusSpousePADependent.icdoPersonAccountDependent.start_date, lbusSpousePADependent.icdoPersonAccountDependent.end_date))
                            //                    {
                            //                        lclbPayeeAccount.Add(lbusPayeeAccount);
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                        }
                    }
                }
            }
            return lclbPayeeAccount;
        }

        public Collection<busPayeeAccount> LoadDeceasedSpousePayeeAccount(busPerson abusSpouse)
        {
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
            if (abusSpouse.icdoPerson.person_id > 0)
            {
                if (abusSpouse.icdoPerson.date_of_death != DateTime.MinValue)
                {
                    if (abusSpouse.iclbPayeeAccount == null)
                        abusSpouse.LoadPayeeAccount();

                    foreach (busPayeeAccount lbusPayeeAccount in abusSpouse.iclbPayeeAccount)
                    {
                        if (lbusPayeeAccount.ibusPlan == null)
                            lbusPayeeAccount.LoadPlan();
                        if (lbusPayeeAccount.ibusBenefitAccount == null)
                            lbusPayeeAccount.LoadBenfitAccount();

                        lbusPayeeAccount.ibusPayee = abusSpouse;

                        if (lbusPayeeAccount.ibusPayeeAccountActiveStatus == null)
                            lbusPayeeAccount.LoadActivePayeeStatus();

                        if ((lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement ||
                            lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) &&
                            (
                            //Systest PIR 2584
                                (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted()) ||
                                (lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted()
                                && HasSpouseAndBeingJoinAnnuitant(abusSpouse, lbusPayeeAccount)
                                && (!HasPostRetirementDeathApplication(lbusPayeeAccount)))
                            )
                            &&
                            lbusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0)
                        {
                            lclbPayeeAccount.Add(lbusPayeeAccount);
                        }
                    }
                }
            }
            return lclbPayeeAccount;
        }

        private bool HasPostRetirementDeathApplication(busPayeeAccount abusPayeeAccount)
        {
            if (abusPayeeAccount.iclbBenefitApplicationByOriginatingPayeeAccount == null)
                abusPayeeAccount.LoadBenefitApplicationByOriginatingPayeeAccount();

            if (abusPayeeAccount.iclbBenefitApplicationByOriginatingPayeeAccount
                .Any(i => i.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath))
                return true;
            return false;
        }

        private bool HasSpouseAndBeingJoinAnnuitant(busPerson abusSpouse, busPayeeAccount abusPayeeAccount)
        {
            if (icolPersonContact == null)
                LoadContacts();
            var lenuList = icolPersonContact.Where(o => o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse);
            if (lenuList != null)
            {
                foreach (busPersonContact lbusPersonContact in lenuList)
                {
                    if (lbusPersonContact.icdoPersonContact.contact_person_id == abusSpouse.icdoPerson.person_id)
                    {
                        if (abusPayeeAccount.ibusApplication == null)
                            abusPayeeAccount.LoadApplication();

                        if (abusPayeeAccount.ibusApplication.ibusJointAnniutantPerson == null)
                            abusPayeeAccount.ibusApplication.LoadJointAnniutantPerson();

                        if (abusPayeeAccount.ibusApplication.ibusJointAnniutantPerson.icdoPerson.person_id == icdoPerson.person_id)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public Collection<busPayeeAccount> LoadPostRetirementPayeeAccount()
        {
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
            if (iclbPayeeAccount == null)
                LoadPayeeAccount(true);

            var lenuPostRetDeathPayeeAccounts = iclbPayeeAccount.Where(i => i.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath);

            foreach (busPayeeAccount lbusPayeeAccount in lenuPostRetDeathPayeeAccounts)
            {
                if (lbusPayeeAccount.ibusBenefitAccount == null)
                    lbusPayeeAccount.LoadBenfitAccount();
                //Commenting this Code for now since conversion is not updating originating payee account id
                //if (lbusPayeeAccount.ibusApplication == null)
                //    lbusPayeeAccount.LoadApplication();
                //if (lbusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount == null)
                //    lbusPayeeAccount.ibusApplication.LoadOriginatingPayeeAccount();
                //if (lbusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.ibusApplication == null)
                //    lbusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.LoadApplication();
                //if (lbusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.ibusApplication.ibusJointAnniutantPerson == null)
                //    lbusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.ibusApplication.LoadJointAnniutantPerson();

                if (((!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted()) &&
                    (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled()) && !lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusSuspended()) &&
                    lbusPayeeAccount.icdoPayeeAccount.rhic_amount > 0
                    //&&
                    //(!lbusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.ibusApplication.IsSingleLifeStandardRhicOption()) &&
                    //lbusPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.ibusApplication.ibusJointAnniutantPerson.icdoPerson.person_id > 0
                    )
                {
                    lclbPayeeAccount.Add(lbusPayeeAccount);
                }
            }
            return lclbPayeeAccount;
        }

        public Collection<busPayeeAccount> LoadPreRetirementPayeeAccount()
        {
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
            if (iclbPayeeAccount == null)
                LoadPayeeAccount(true);

            foreach (busPayeeAccount lbusPayeeAccount in iclbPayeeAccount)
            {
                if (lbusPayeeAccount.ibusBenefitAccount == null)
                    lbusPayeeAccount.LoadBenfitAccount();
                if ((lbusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath) &&
                    (lbusPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOptionRefund &&
                    lbusPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOptionAutoRefund) &&
                    ((!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted()) &&
                    (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled()) && (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusSuspended())) && 

                    lbusPayeeAccount.icdoPayeeAccount.rhic_amount > 0)
                {
                    lclbPayeeAccount.Add(lbusPayeeAccount);
                }
            }
            return lclbPayeeAccount;
        }

        public busPersonAccountGhdv ibusReceiverHealthPersonAccount { get; set; }
        public void LoadReceiverHealthPersonAccount(DateTime adtEffectiveDate)
        {
            ibusReceiverHealthPersonAccount = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };

            if (icolPersonAccount == null)
                LoadPersonAccount();

            busPersonAccount lbusPersonAccount = icolPersonAccount.FirstOrDefault(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth);
            if (lbusPersonAccount != null)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();
                //BR-055-133
                if (lbusPersonAccount.ibusPlan.icdoPlan.apply_rhic_flag == busConstant.Flag_Yes)
                {
                    //RHIC can not be applied to health plan provided by employer
                    if (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        if (lbusPersonAccount.ibusPaymentElection == null)
                            lbusPersonAccount.LoadPaymentElection();

                        if (lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
                        {
                            ibusReceiverHealthPersonAccount.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                            ibusReceiverHealthPersonAccount.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        }
                    }
                }
            }
        }

        public busPersonAccountGhdv ibusReceiverMedicarePersonAccount { get; set; }
        public void LoadReceiverMedicarePersonAccount(DateTime adtEffectiveDate)
        {
            ibusReceiverMedicarePersonAccount = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };

            if (icolPersonAccount == null)
                LoadPersonAccount();

            busPersonAccount lbusPersonAccount = icolPersonAccount.FirstOrDefault(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD);
            if (lbusPersonAccount != null)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();
                //BR-055-133
                if (lbusPersonAccount.ibusPlan.icdoPlan.apply_rhic_flag == busConstant.Flag_Yes)
                {
                    //RHIC can not be applied to health plan provided by employer
                    if (lbusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        if (lbusPersonAccount.ibusPaymentElection == null)
                            lbusPersonAccount.LoadPaymentElection();

                        if (lbusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
                        {
                            ibusReceiverMedicarePersonAccount.icdoPersonAccount = lbusPersonAccount.icdoPersonAccount;
                            ibusReceiverMedicarePersonAccount.FindGHDVByPersonAccountID(lbusPersonAccount.icdoPersonAccount.person_account_id);
                        }
                    }
                }
            }
        }

        //Latest Valid Approved Combine Record for this Donor
        public busBenefitRhicCombine ibusLatestBenefitRhicCombine { get; set; }

        public void LoadLatestBenefitRhicCombine()
        {
            //Load the Old Rhic Combine Record for this Donor Payee Account
            if (iclbBenefitRhicCombine == null)
                LoadBenefitRhicCombine();

            //Load the Valid Approved Record for Donor Payee Account
            ibusLatestBenefitRhicCombine = iclbBenefitRhicCombine.Where(i => i.icdoBenefitRhicCombine.status_value == busConstant.RHICStatusValid &&
                                                                    i.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusApproved)
                                                                    .OrderByDescending(i => i.icdoBenefitRhicCombine.end_date_no_null)
                                                                    .ThenByDescending(i=>i.icdoBenefitRhicCombine.start_date) // PROD PIR ID 5867
                                                                    .ThenByDescending(i => i.icdoBenefitRhicCombine.benefit_rhic_combine_id)
                                                                    .FirstOrDefault();
        }

        // UAT PIR ID 1427
        public Collection<busCase> iclbCase { get; set; }

        public void LoadCase()
        {
            if (iclbCase.IsNull()) iclbCase = new Collection<busCase>();
            DataTable ldtbResults = Select("cdoPerson.LoadCases", new object[1] { icdoPerson.person_id });
            iclbCase = GetCollection<busCase>(ldtbResults, "icdoCase");
        }

        public busPersonEmployment ibusPreviousEmployment { get; set; }
        public busPersonEmployment ibusLastEmployment { get; set; }		//PIR-10361

        public void LoadPreviousEmployment()
        {
            if (icolPersonEmployment == null)
                LoadPersonEmployment();

            if (icolPersonEmployment.Count > 1)
            {
                ibusPreviousEmployment = icolPersonEmployment.OrderByDescending(i => i.icdoPersonEmployment.end_date_no_null).Skip(1).FirstOrDefault();
            }
            if (ibusPreviousEmployment == null)
                ibusPreviousEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
        }
	  //PIR-10361
      public void LoadLastEmployment()
        {
            if (icolPersonEmployment == null)
                LoadPersonEmployment();

            if (icolPersonEmployment.Count > 0)
            {
                ibusLastEmployment = icolPersonEmployment.Where(o=>o.icdoPersonEmployment.end_date !=DateTime.MinValue)
                    .OrderByDescending(i => i.icdoPersonEmployment.end_date_no_null).FirstOrDefault();
            }
            if (ibusLastEmployment == null)
                ibusLastEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
        }
        //UAT PIR 1573
        public bool IsAnyBeneficiaryDeathInProgressorNonResponsive()
        {
            if (iclbPersonBeneficiary == null) LoadBeneficiary();
            foreach (busPersonBeneficiary lobjPersonBeneficiary in iclbPersonBeneficiary)
            {
                busDeathNotification lobjDeathNotification = new busDeathNotification();
                if (lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id != 0)
                {
                    if (lobjDeathNotification.FindDeathNotificationByPersonId(lobjPersonBeneficiary.icdoPersonBeneficiary.beneficiary_person_id))
                    {
                        if ((lobjDeathNotification.icdoDeathNotification.action_status_value == busConstant.DeathNotificationActionStatusInProgress) ||
                            (lobjDeathNotification.icdoDeathNotification.action_status_value == busConstant.DeathNotificationActionStatusNonResponsive))
                            return true;
                    }
                }
            }
            return false;
        }

        //UAT PIR 1573
        public bool IsAnyDependentDeathInProgressorNonResponsive()
        {
            if (iclbPersonDependent == null) LoadDependent();
            foreach (busPersonDependent lobjPersonDependent in iclbPersonDependent)
            {
                busDeathNotification lobjDeathNotification = new busDeathNotification();
                if (lobjDeathNotification.FindDeathNotificationByPersonId(lobjPersonDependent.icdoPersonDependent.dependent_perslink_id))
                {
                    if ((lobjDeathNotification.icdoDeathNotification.action_status_value == busConstant.DeathNotificationActionStatusInProgress) ||
                        (lobjDeathNotification.icdoDeathNotification.action_status_value == busConstant.DeathNotificationActionStatusNonResponsive))
                        return true;
                }
            }
            return false;
        }
        //Exlcude Withdrawn
        public bool IsDBPersonAccountExists()
        {
            if (icolPersonAccount == null)
                LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();
            }

            if (icolPersonAccount.Any(i => (i.ibusPlan.IsDBRetirementPlan()|| i.ibusPlan.IsHBRetirementPlan()) && i.IsPlanParticipationStatusRetiredOrSuspendedOrEnrolled()))
                return true;
            return false;
        }

        public bool IsDCPersonAccountExists(bool ablnTransferDBCheck = false)
        {
            if (icolPersonAccount == null)
                LoadPersonAccount();

            foreach (busPersonAccount lbusPersonAccount in icolPersonAccount)
            {
                if (lbusPersonAccount.ibusPlan == null)
                    lbusPersonAccount.LoadPlan();
            }

            if (icolPersonAccount.Any(i => i.ibusPlan.IsDCRetirementPlan() && (!i.IsWithDrawn())))
            {
                //PIR 16439
                if (!ablnTransferDBCheck)
                    return true;
                else
                {
                    if (icolPersonAccount.Any(i => i.ibusPlan.IsDCRetirementPlan() && (!i.IsTransferredDB())))
                        return true;
                }
            }
            return false;
        }

        public bool iblnIsEmploymentSeasonalLoaded = false;
        public int iintSeasonalMonths { get; set; }
        public bool iblnIsEmploymentSeasonal = false;
        public bool iblnIsFromESS { get; set; }
        public void LoadEmploymentSeasonal()
        {
            iblnIsEmploymentSeasonal = false;
            iintSeasonalMonths = 0;

            if (icolPersonEmployment == null)
                LoadPersonEmployment();

            busPersonEmployment lobjPersonEmployment = icolPersonEmployment.OrderByDescending(lobjPE => lobjPE.icdoPersonEmployment.start_date).FirstOrDefault();

            if (lobjPersonEmployment != null)
            {
                if (lobjPersonEmployment.icolPersonEmploymentDetail == null)
                    lobjPersonEmployment.LoadPersonEmploymentDetail(false);
                //UAT: PIR : 935
                /*?	Employment record is not end dated, and	Employment status is ?Contributing,? and ?Seasonal? data field entry is a value other than ?Blank.?.  
                 * It should still go as per seasonal data. Like if one is 6 month and another is 10 month then total should be 10 month for FY
                 */
                busPersonEmploymentDetail lobjPersonEmploymentdtl = lobjPersonEmployment.icolPersonEmploymentDetail
                                                                       .OrderByDescending(lobjDet => lobjDet.icdoPersonEmploymentDetail.start_date)
                                                                       .FirstOrDefault();

                if (lobjPersonEmploymentdtl != null)
                {
                    if ((!string.IsNullOrEmpty(lobjPersonEmploymentdtl.icdoPersonEmploymentDetail.seasonal_value))
                        && (lobjPersonEmploymentdtl.icdoPersonEmploymentDetail.end_date == DateTime.MinValue)
                        && (lobjPersonEmploymentdtl.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusContributing))
                    {
                        iintSeasonalMonths = Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(312, lobjPersonEmploymentdtl.icdoPersonEmploymentDetail.seasonal_value, iobjPassInfo));
                        iblnIsEmploymentSeasonal = true;
                    }
                }
            }
            iblnIsEmploymentSeasonalLoaded = true;
        }

        #region 32
        public bool IsESSPremiumConversionTextVisible()
        {
            bool lblnResult = false;
            int lintProviderOrgID = 0;
            if ((ibusESSPersonAccountGhdv != null && ibusESSPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id > 0) ||
                (ibusESSPersonAccountGroupLife != null && ibusESSPersonAccountGroupLife.icdoPersonAccountLife.person_account_id > 0))
            {
                if (icolPersonAccount == null)
                    LoadPersonAccount();
                if (icolPersonAccount.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdFlex).Count() > 0)
                {
                    ibusESSPersonAccountFlexComp = new busPersonAccountFlexComp
                    {
                        icdoPersonAccount = new cdoPersonAccount(),
                        icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp()
                    };
                    ibusESSPersonAccountFlexComp.FindPersonAccountFlexComp(icolPersonAccount.Where(o =>
                        o.icdoPersonAccount.plan_id == busConstant.PlanIdFlex).Select(o => o.icdoPersonAccount.person_account_id).FirstOrDefault());
                    ibusESSPersonAccountFlexComp.LoadFlexCompConversion();
                    if (ibusESSPersonAccountGhdv != null && ibusESSPersonAccountGhdv.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                    {
                        if (ibusESSPersonAccountGhdv.iintESSProviderOrgID <= 0)
                            ibusESSPersonAccountGhdv.ESSLoadProviderOrgName(iintOrgID);
                        lintProviderOrgID = ibusESSPersonAccountGhdv.iintESSProviderOrgID;
                    }
                    else if (ibusESSPersonAccountGroupLife != null && ibusESSPersonAccountGroupLife.icdoPersonAccountLife.person_account_id > 0)
                    {
                        if (ibusESSPersonAccountGroupLife.iintESSProviderOrgID <= 0)
                            ibusESSPersonAccountGroupLife.ESSLoadProviderOrgName(iintOrgID);
                        lintProviderOrgID = ibusESSPersonAccountGroupLife.iintESSProviderOrgID;
                    }

                    if (ibusESSPersonAccountFlexComp.iclbFlexcompConversion != null &&
                        ibusESSPersonAccountFlexComp.iclbFlexcompConversion.Count > 0 &&
                        ibusESSPersonAccountFlexComp.iclbFlexcompConversion.Where(o => busGlobalFunctions.CheckDateOverlapping(DateTime.Today, o.icdoPersonAccountFlexcompConversion.effective_start_date,
                               o.icdoPersonAccountFlexcompConversion.effective_end_date) && o.icdoPersonAccountFlexcompConversion.org_id == lintProviderOrgID).Any())
                    {
                        lblnResult = true;
                    }
                }
            }
            return lblnResult;
        }

        public bool IsESSEmploymentPanelVisible()
        {
            bool lblnResult = false;
            bool lblnRetExists = false;
            if (ibusESSPersonAccount != null)
            {
                if (ibusESSPersonAccount.ibusPlan == null)
                    ibusESSPersonAccount.LoadPlan();
                if (ibusESSPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                    lblnResult = true;
                else
                {
                    if (icolPersonEmployment == null)
                        LoadPersonEmployment(false);
                    IEnumerable<busPersonEmployment> lenmPersonEmployment = icolPersonEmployment.Where(o => o.icdoPersonEmployment.org_id == iintOrgID);
                    foreach (busPersonEmployment lobjPE in lenmPersonEmployment)
                    {
                        if (lobjPE.icolPersonEmploymentDetail == null)
                            lobjPE.LoadPersonEmploymentDetail(false);
                        foreach (busPersonEmploymentDetail lobjPED in lobjPE.icolPersonEmploymentDetail)
                        {
                            if (lobjPED.iclbAllPersonAccountEmpDtl == null)
                                lobjPED.LoadAllPersonAccountEmploymentDetails(true);
                            foreach (busPersonAccountEmploymentDetail lobjPAED in lobjPED.iclbAllPersonAccountEmpDtl)
                            {
                                lobjPAED.ibusPersonAccount.LoadPlan();
                            }
                            if (lobjPED.iclbAllPersonAccountEmpDtl.Where(o => o.ibusPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement).Any())
                                lblnRetExists = true;
                        }
                    }
                    if (!lblnRetExists)
                        lblnResult = true;
                }
            }
            return lblnResult;
        }
        #endregion
        #region 32 addendum
        public bool IsEnrollin457ApplicableESS()
        {
            if (ibusESSPersonEmployment == null)
                ESSLoadPersonEmployment();
            if (ibusESSPersonEmployment.ibusOrganization == null)
                ibusESSPersonEmployment.LoadOrganization();
            if (ibusESSPersonEmployment.ibusOrganization.iclbOrgPlan == null)
                ibusESSPersonEmployment.ibusOrganization.LoadOrgPlan();
            if (ibusESSPersonEmployment.ibusOrganization.iclbOrgPlan.Where(o =>
                o.icdoOrgPlan.plan_id == busConstant.PlanIdOther457 &&
                busGlobalFunctions.CheckDateOverlapping(DateTime.Today, o.icdoOrgPlan.participation_start_date, o.icdoOrgPlan.participation_end_date)).Count() > 0)
            {
                return true;
            }
            return false;
        }
        public int iintEmploymentChangeREquestId { get; set; }
        public bool iblnTerminateEmploymentRequestExists { get; set; }
        public bool iblnEmploymwntChangeRequestExists { get; set; }
        public int iintTermEmploymentChangeRequestID { get; set; }
        public int iintUpdateEmploymentChangeRequestID { get; set; }
        public int iintLOALEmploymentChangeRequestID { get; set; }
        public int iintLOAMEmploymentChangeRequestID { get; set; }
        public int iintLOAREmploymentChangeRequestID { get; set; }

        public void LoadEmploymentChangeRequestDetails()
        {
            DataTable ldtbRequest = Select<cdoWssEmploymentChangeRequest>(new string[2] { "person_id", "org_id" },
                new object[2] { icdoPerson.person_id, iintOrgID }, null, null);

            DataTable ldtbPendingRequests = ldtbRequest.AsEnumerable().Where(ldr => ldr.Field<string>("status_value") != null
                 && (ldr.Field<string>("status_value") == busConstant.EmploymentChangeRequestStatusRejected ||
                 ldr.Field<string>("status_value") == busConstant.EmploymentChangeRequestStatusReview)).AsDataTable();
            Collection<busWssEmploymentChangeRequest> lclbRequest = GetCollection<busWssEmploymentChangeRequest>(ldtbPendingRequests, "icdoWssEmploymentChangeRequest");
            foreach (busWssEmploymentChangeRequest lbusWssEmploymentChangeRequest in lclbRequest)
            {
                if (!string.IsNullOrEmpty(lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value))
                    iblnEmploymwntChangeRequestExists = true;
                if (lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.last_date_of_service != DateTime.MinValue)
                    iblnTerminateEmploymentRequestExists = true;
                iintEmploymentChangeREquestId = lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_change_request_id;
            }
        }
        public void LoadESSEmploymentChangeRequestDetails()
        {
            DataTable ldtbRequest = Select<cdoWssEmploymentChangeRequest>(new string[2] { "person_id", "org_id" },
                new object[2] { icdoPerson.person_id, iintOrgID }, null, null);

            DataTable ldtbPendingRequests = ldtbRequest.AsEnumerable().Where(ldr => ldr.Field<string>("status_value") != null
                 && (ldr.Field<string>("status_value") == busConstant.EmploymentChangeRequestStatusRejected ||
                 ldr.Field<string>("status_value") == busConstant.EmploymentChangeRequestStatusReview)).AsDataTable();
            string lstrChangetype = string.Empty;
            Collection<busWssEmploymentChangeRequest> lclbRequest = GetCollection<busWssEmploymentChangeRequest>(ldtbPendingRequests, "icdoWssEmploymentChangeRequest");
            foreach (busWssEmploymentChangeRequest lbusWssEmploymentChangeRequest in lclbRequest)
            {
                lstrChangetype = lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.change_type_value;
                if (!string.IsNullOrEmpty(lstrChangetype))
                {
                    switch (lstrChangetype)
                    {
                        case "TEEM":
                            iintTermEmploymentChangeRequestID = lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_change_request_id;
                            break;
                        case "CLSC":
                            iintUpdateEmploymentChangeRequestID = lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_change_request_id;
                            break;
                        case "LOAL":
                        case "LOAM":
                        case "LOAR":
                            iintLOALEmploymentChangeRequestID = lbusWssEmploymentChangeRequest.icdoWssEmploymentChangeRequest.employment_change_request_id;
                            break;
                    }
                }
            }
        }

        #endregion

        //load rhic in which person is donor 
        public Collection<busBenefitRhicCombine> iclbBenefitRHICCombineAsDonor { get; set; }
        public void LoadRhicCombineForPersonAsDonor()
        {
            if (iclbBenefitRHICCombineAsDonor.IsNull())
                iclbBenefitRHICCombineAsDonor = new Collection<busBenefitRhicCombine>();

            if (iclbPayeeAccount.IsNull())
                LoadPayeeAccount();

            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
            {
                if (lobjPayeeAccount.iclbBenefitRHICCombineDetail.IsNull())
                    lobjPayeeAccount.LoadBenefitRhicCombineDetail();

                foreach (busBenefitRhicCombineDetail lobjCombineDetail in lobjPayeeAccount.iclbBenefitRHICCombineDetail)
                {
                    lobjCombineDetail.LoadRHICCombine();
                    iclbBenefitRHICCombineAsDonor.Add(lobjCombineDetail.ibusBenefitRHICCombine);
                }
            }
        }

        /// UAT PIR ID 2161 - MS Change date is required if Marital Status date is modified
        public bool IsMSChangeDateNeeded()
        {
            if (Convert.ToString(icdoPerson.ihstOldValues["marital_status_value"]) != icdoPerson.marital_status_value &&
                icdoPerson.ms_change_date == DateTime.MinValue)
                return true;
            return false;
        }

        #region Benefit Calculation

        public bool IsFormerDBPlanTransfertoDC(int aintFormerPlanID)
        {
            bool bIsFormerPlan = false;
            if (iclbRetirementAccount == null)
                LoadRetirementAccount();
            foreach (busPersonAccount lobjPersonAccount in iclbRetirementAccount)
            {
                //UAT PIR:2077 For DC plan. NO need to check the date.
                if ((lobjPersonAccount.icdoPersonAccount.plan_id == aintFormerPlanID)
                    && (lobjPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferDC))
                {
                    bIsFormerPlan = true;
                    break;
                }
            }
            return bIsFormerPlan;
        }

        #endregion

        public DataTable idtbPlanCacheData { get; set; }


        public void GenerateCorrespondenceForSendingPersLinkID()
        {
            ArrayList larrlist = new ArrayList();
            larrlist.Add(this);
            Hashtable lhstDummyTable = new Hashtable();
            lhstDummyTable.Add("sfwCallingForm", "Batch");
            utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence("PER-0975", this, lhstDummyTable);
            if (lobjCorresPondenceInfo.istrGeneratedFileName.IsNotNull()) // PIR 10400
            {
                Sagitec.CorBuilder.CorBuilderXML lobjCorBuilder = new Sagitec.CorBuilder.CorBuilderXML(); //PIR 16947 - Change to CorBuilderXML class
                lobjCorBuilder.InstantiateWord();
                lobjCorBuilder.CreateCorrespondenceFromTemplate("PER-0975", lobjCorresPondenceInfo, iobjPassInfo.istrUserID);
                lobjCorBuilder.CloseWord();
            }
        }


        // PAY-4302
        public string istrFirstofCurrentMonthFormatted
        {
            get
            {
                return DateTime.Now.GetLastDayofMonth().ToString(busConstant.DateFormatLongDate);
            }
        }

        public string istrFirstofNextMonthFormatted
        {
            get
            {
                return DateTime.Now.AddMonths(1).GetLastDayofMonth().ToString(busConstant.DateFormatLongDate);
            }
        }
        //pir 6566
        public string beneficiary_required_flag_for_display { get; set; }
        public void SetBeneficiaryRequiredFlag()
        {
            if (iclbActivePensionAccounts == null)
                LoadActivePlanDetails();
            if (iclbPayeeAccount == null)
                LoadPayeeAccount();
            var lenumPersonAcc = iclbActivePensionAccounts.Where(i => i.ibusPlan.IsRetirementPlan() || i.icdoPersonAccount.plan_id == 13);//pir 6566
            if (lenumPersonAcc.Count() <= 0)
                beneficiary_required_flag_for_display = busConstant.Flag_No;
            else // pir 6566
            if (lenumPersonAcc.Where(i => i.icdoPersonAccount.plan_id == 13 && i.icdoPersonAccount.plan_participation_status_value 
                == busConstant.PlanParticipationStatusInsuranceEnrolled).Count() <= 0 && iclbPayeeAccount.Count > 0)
            {
                beneficiary_required_flag_for_display = null;
                foreach(busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
                {
                    lobjPayeeAccount.LoadMinimumGuaranteeAmount();
                    if (lobjPayeeAccount.idecMinimumGuaranteeAmount == 0)
                    {
                        if (lobjPayeeAccount.IsBenefitOptionSingleLife)
                        {
                            beneficiary_required_flag_for_display = busConstant.Flag_No;
                            break;
                        }
                        else if (lobjPayeeAccount.IsTermCertainOption && lobjPayeeAccount.IsTermCertainEndDatePastDate)
                        //else if (lenumPayeeAcc.Where(i => (i.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption10YearCertain && i.icdoPayeeAccount.benefit_begin_date.AddYears(Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, "busConstant.BenefitOption10YearCertain", utlPassInfo.iobjPassInfo))) < DateTime.Now)
                        //    || (i.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption20YearCertain && i.icdoPayeeAccount.benefit_begin_date.AddYears(Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, "busConstant.BenefitOption20YearCertain", utlPassInfo.iobjPassInfo))) < DateTime.Now)
                        //    || (i.icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption15YearCertain && i.icdoPayeeAccount.benefit_begin_date.AddYears(Convert.ToInt32(busGlobalFunctions.GetData1ByCodeValue(52, "busConstant.BenefitOption15YearCertain", utlPassInfo.iobjPassInfo))) < DateTime.Now)).Count() > 0)
                        {
                            beneficiary_required_flag_for_display = busConstant.Flag_No;
                            break;
                        }
                        else if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.BenefitAccountSubTypePreRetirementDeathBenefit
                            || lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.BenefitAccountSubTypePostRetDeath)
                        {
                            beneficiary_required_flag_for_display = busConstant.Flag_No;
                            break;
                        }
                        
                    }
                }
            }
            if (beneficiary_required_flag_for_display == null)
                beneficiary_required_flag_for_display = busConstant.Flag_Yes;
                
        }
        //pir 8504
        public void LoadBeneficiaryForRetirementPlan()
        {
            if (iclbRetirementAccount.IsNull())
                LoadRetirementAccount();
            foreach (busPersonAccount lobjRetirementAccount in iclbRetirementAccount)
            {
                LoadBeneficiaryForMemberByPersonAccount(lobjRetirementAccount.icdoPersonAccount.person_account_id);
            }
        }

        //PIR-10687 Start 
        /// <summary>
        /// Here we are loading the Payee Account of the spouse who is reciving from the her x spouse
        /// </summary>
        /// <param name="abusSpouse"></param>
        /// <param name="adtEffectiveDate"></param>
        /// <param name="ablnIsEstimate"></param>
        /// <param name="ablnIsAutomaticCombine"></param>
        /// <returns></returns>

        public Collection<busPayeeAccount> LoadReceivingSpousePayeeAccount(busPerson abusSpouse, DateTime adtEffectiveDate, bool ablnIsEstimate = false, bool ablnIsAutomaticCombine = false)
        {
            Collection<busPayeeAccount> lclbPayeeAccount = new Collection<busPayeeAccount>();
            if (abusSpouse.icdoPerson.person_id > 0)
            {
                if (abusSpouse.icdoPerson.date_of_death == DateTime.MinValue)
                {
                    if (iclbPayeeAccount == null)
                        LoadPayeeAccount(true);

                    //var lenuPreRetDeathRecievingPayeeAccounts = iclbPayeeAccount.Where(i => i.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath);

                    foreach (busPayeeAccount lbusPayeeAccount in iclbPayeeAccount)
                    {
                        if (lbusPayeeAccount.ibusBenefitAccount == null)
                            lbusPayeeAccount.LoadBenfitAccount();

                        if (((!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted()) &&
                            (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled()) && (!lbusPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusSuspended())) &&
                            lbusPayeeAccount.icdoPayeeAccount.rhic_amount > 0)
                        {
                            lbusPayeeAccount.LoadApplication();
                            if (lbusPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id != abusSpouse.icdoPerson.person_id)
                            {
                                lclbPayeeAccount.Add(lbusPayeeAccount);                                                            
                            }

                            //PIR 14346 - No need to check health enrollment for adding donor detail
                            //Need to confirm on the Estimate and in what case we need to add the Donor in RHICDonor detail grid && also in Estimate RHIC Donor grid
                            //if (ablnIsEstimate)
                            //{
                                //lclbPayeeAccount.Add(lbusPayeeAccount);
                            //}
                            // PIR 10687 - if spouse is enrolled in health plan, it should not show as eligible to combine
                            //else
                            //{
                            //    LoadHealthPersonAccount();
                            //    lbusPayeeAccount.LoadApplication();
                            //    if (ibusHealthPersonAccount.icdoPersonAccount.person_account_id > 0)
                            //    {
                            //        if (ibusHealthPersonAccount.ibusPersonAccountGHDV.IsNull()) ibusHealthPersonAccount.LoadPersonAccountGHDV();
                            //        busPersonAccountGhdvHistory lobjPAGhdvHistory = ibusHealthPersonAccount.ibusPersonAccountGHDV.LoadHistoryByDate(adtEffectiveDate);
                            //        if (lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.person_account_ghdv_history_id > 0)
                            //        {
                            //            if (lobjPAGhdvHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled
                            //                && lbusPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id != abusSpouse.icdoPerson.person_id)
                            //            {
                            //                lclbPayeeAccount.Add(lbusPayeeAccount);

                            //            }
                            //        }
                            //    }
                            //    //PIR 14150 - iii.	If there is no Health Plan, it should show as eligible to combine - maik mail dated February 24, 2015
                            //    else
                            //    {
                            //        if (lbusPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id != abusSpouse.icdoPerson.person_id)
                            //        {
                            //            lclbPayeeAccount.Add(lbusPayeeAccount);
                            //        }
                            //    }
                                //commented as part of PIR 14150 fix - PIR 11020 also tested with this fix.
                                //if (ibusHealthPersonAccount.ibusPersonAccountGHDV == null) //PIR-11020 
                                //    ibusHealthPersonAccount.LoadPersonAccountGHDV();
                                //if (ibusHealthPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                                //{
                                //    lbusPayeeAccount.LoadApplication();
                                //    if (lbusPayeeAccount.ibusApplication.icdoBenefitApplication.member_person_id != abusSpouse.icdoPerson.person_id)
                                //    {
                                //        lclbPayeeAccount.Add(lbusPayeeAccount);
                                //    }
                                //}
                            //}
                        }
                    }
                }
            }
            return lclbPayeeAccount;
        }


        //PIR-11030 Start
        public busPerson LoadPersonBySsn(string lstrSSN)
        {
            //busPerson lobjPerson= new busPerson ();
            Collection<busPerson> lclbPerson = new Collection<busPerson>();
            DataTable ldtbPerson = DBFunction.DBSelect("cdoPerson.GetPersonBySSN", new string[1] { lstrSSN },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (ldtbPerson.Rows.Count > 0)
            {
                lclbPerson = GetCollection<busPerson>(ldtbPerson, "icdoPerson");
                return lclbPerson[0];
            }
            return null;
        }
        //PIR-11030 End


        //PIR-10687 End 

        //PIR 11867 - Load address irrespective of end date and undeliverable flag
        public void GetPersonLatestAddress()
        {
            _ibusPersonCurrentAddress = new busPersonAddress();
            _ibusPersonCurrentAddress.icdoPersonAddress = new cdoPersonAddress();

            if (iclbPersonAddress == null)
                LoadLatestPersonAddress();

            if (iclbPersonAddress.Count() > 0)
            {
                _ibusPersonCurrentAddress = iclbPersonAddress.First();
                _ibusPersonCurrentAddress.icdoPersonAddress = iclbPersonAddress.First().icdoPersonAddress;
                _ibusPersonCurrentAddress.icdoPersonCurrentAddress = iclbPersonAddress.First().icdoPersonAddress;
            }
        }

        public void LoadLatestPersonAddress()
        {
            DataTable ldtbList = Select<cdoPersonAddress>(
                new string[1] { "person_id" },
                new object[1] { icdoPerson.person_id }, null, "case when end_date is null then 0 else 1 end, start_date desc, end_date desc");
            iclbPersonAddress = GetCollection<busPersonAddress>(ldtbList, "icdoPersonAddress");
        }
        public bool IsSupplementalLifeOptionAmountGreaterThanZero()
        {
            bool lblnResult = false;
            if (ibusESSPersonAccountGroupLife.IsNotNull() && ibusESSPersonAccountGroupLife.iclbLifeOption.IsNotNull())
            {
                lblnResult = ibusESSPersonAccountGroupLife.iclbLifeOption.Any(i => i.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_Supplemental && i.icdoPersonAccountLifeOption.coverage_amount != 0.00M);
            }
            
            return lblnResult;

        }

        //PIR-11060 Start
        // Not required for now that. Will delete this once it is finalized
        //public void LoadEmploymentWithRespecToHistory(busPersonAccountPeopleSoftFile aobjPeopleSoftFile)
        //{
        //    LoadPersonEmployment();
        //    busPersonEmployment lobjPersonEmployment = new busPersonEmployment();
        //    switch (aobjPeopleSoftFile.ibusPersonAccount.ibusPlan.icdoPlan.plan_id)
        //    {
        //        case busConstant.PlanIdGroupHealth:
        //        case busConstant.PlanIdDental:
        //        case busConstant.PlanIdVision:
        //        lobjPersonEmployment = icolPersonEmployment.Where(o => busGlobalFunctions.CheckDateOverlapping(aobjPeopleSoftFile.ibusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.start_date,
        //        o.icdoPersonEmployment.start_date,o.icdoPersonEmployment.end_date)).FirstOrDefault();            
        //        lobjPersonEmployment.LoadPersonEmploymentDetail();
        //        aobjPeopleSoftFile.ibusPersonAccount.ibusPersonEmploymentDetail = lobjPersonEmployment.icolPersonEmploymentDetail.Where(o => 
        //            busGlobalFunctions.CheckDateOverlapping(aobjPeopleSoftFile.ibusPersonAccountGhdvHistory.icdoPersonAccountGhdvHistory.start_date,
        //        o.icdoPersonEmploymentDetail.start_date,o.icdoPersonEmploymentDetail.end_date)).FirstOrDefault();
        //        break; 
        //    }       
            
        //   //lobjPersonEmployment.LoadPersonEmploymentDetail();
        //}
        //PIR-11060 End
        public ArrayList UpdateAddressClick()
        {
            ArrayList larrList = new ArrayList();
            iblnIsUpdateAddressClicked = true;
			//PIR Change 13157
            istrSuppressWarning = "N";
            
            //PIR-15430
            ibusPersonCurrentAddress.icdoPersonAddress.start_date = DateTime.Now;
            ibusPersonCurrentAddress.icdoPersonAddress.end_date = DateTime.MinValue;
            ibusPersonCurrentAddress.icdoPersonAddress.person_id = icdoPerson.person_id;
            ibusPersonCurrentAddress.icdoPersonAddress.person_address_id = 0;
            ibusPersonCurrentAddress.icdoPersonAddress.address_type_value=busConstant.AddressTypePermanent; 
            this.EvaluateInitialLoadRules();
            larrList.Add(this);
            return larrList;
        }

     
        public void VerifyAddressUsingUSPS()
        {
            ArrayList larrErrors = new ArrayList();

            //If Suppress Warning Flag is Checked, we can skip the Web Service Validation
            //istrSuppressWarning = "N";
            if (istrSuppressWarning == busConstant.Flag_Yes)
            {
                //address_validate_flag = busConstant.Flag_Yes;

                ibusPersonCurrentAddress.icdoPersonAddress.address_validate_flag = "Y";
                return;
            }
            cdoWebServiceAddress _lobjcdoWebServiceAddress = new cdoWebServiceAddress();
            _lobjcdoWebServiceAddress.addr_line_1 = ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1;
            _lobjcdoWebServiceAddress.addr_line_2 = ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2;
            _lobjcdoWebServiceAddress.addr_city = ibusPersonCurrentAddress.icdoPersonAddress.addr_city;
            _lobjcdoWebServiceAddress.addr_state_value = ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value;
            _lobjcdoWebServiceAddress.addr_zip_code = ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code;
            _lobjcdoWebServiceAddress.addr_zip_4_code = ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code;

            cdoWebServiceAddress _lobjcdoWebServiceAddressResult = busGlobalFunctions.ValidateWebServiceAddress(_lobjcdoWebServiceAddress);
            if (_lobjcdoWebServiceAddressResult.address_validate_flag != busConstant.Flag_No)
            {
                //ASSSIGN 
                ibusPersonCurrentAddress.icdoPersonAddress.addr_line_1 = _lobjcdoWebServiceAddressResult.addr_line_1;
                ibusPersonCurrentAddress.icdoPersonAddress.addr_line_2 = _lobjcdoWebServiceAddressResult.addr_line_2;
                ibusPersonCurrentAddress.icdoPersonAddress.addr_city = _lobjcdoWebServiceAddressResult.addr_city;
                ibusPersonCurrentAddress.icdoPersonAddress.addr_state_value = _lobjcdoWebServiceAddressResult.addr_state_value;
                ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_code = _lobjcdoWebServiceAddressResult.addr_zip_code;
                ibusPersonCurrentAddress.icdoPersonAddress.addr_zip_4_code = _lobjcdoWebServiceAddressResult.addr_zip_4_code;
            }
            //address_validate_flag = _lobjcdoWebServiceAddressResult.address_validate_flag;
            ibusPersonCurrentAddress.icdoPersonAddress.address_validate_flag = _lobjcdoWebServiceAddressResult.address_validate_flag;
            ibusPersonCurrentAddress.icdoPersonAddress.address_validate_error = _lobjcdoWebServiceAddressResult.address_validate_error;

            //return larrErrors.Add(this);
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {

            if (this.iblnIsFromESS)
            {
                VerifyAddressUsingUSPS();
            }
            base.BeforeValidate(aenmPageMode);
        }
        //Check if the contact is active PAAG or AUTH
        public bool IsContactPrimaryAuthOrAuthAgent()
        {
            busOrganization lobjbusOrganization = new busOrganization();
            lobjbusOrganization.FindOrganization(iintOrgID);
            lobjbusOrganization.LoadOrgContact();
            if (lobjbusOrganization.iclbOrgContact.Where(i => i.icdoOrgContact.contact_id == iintContactID && (i.icdoContactRole.contact_role_value == busConstant.OrgContactRolePrimaryAuthorizedAgent || i.icdoContactRole.contact_role_value == busConstant.OrgContactRoleAuthorizedAgent) && i.icdoOrgContact.status_value==busConstant.StatusActive).Count() > 0)
            {
                return true;
            }
            return false;
        }
        //FOR PIR 13157 Modified the Visibility for update address button if Contact is PAAG or AUTH         
        public bool IsUpdateAddressVisible()
        {
            busOrganization lobjbusOrganization = new busOrganization();
            lobjbusOrganization.FindOrganization(iintOrgID);
            if(ibusESSPersonEmployment.IsNull()) ESSLoadPersonEmployment(); //Org Name on ESS Employee maintenance not showing up if loaded without null check
            if (IsContactPrimaryAuthOrAuthAgent())
            {
                if ((lobjbusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueState
                || lobjbusOrganization.icdoOrganization.peoplesoft_org_group_value == busConstant.PeopleSoftOrgGroupValueBND)
                && ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypePermanent)
                    return false;
                else
                    return true;
            }            
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aenmPageMode"></param>
		//PIR Change 13157	  
        public override void ValidateHardErrors(utlPageMode aenmPageMode)
        {
            iarrErrors = new ArrayList();
            base.ValidateHardErrors(aenmPageMode);
            if (iblnIsFromESS)
            {
                foreach (utlError lobjErr in iarrErrors)
                {
                    lobjErr.istrErrorID = string.Empty;
                }
            }
        }

        //PIR 14848 - Medicare Part D changes
        public void UpdateMedicarePartDFlags()
        {
            if (icdoPerson.ihstOldValues.Count > 0)
            {
                if ((!iblnIsFromMSS && (Convert.ToString(icdoPerson.ihstOldValues["first_name"]) != icdoPerson.first_name ||
                    (Convert.ToString(icdoPerson.ihstOldValues["last_name"]) != icdoPerson.last_name ||
                    Convert.ToString(icdoPerson.ihstOldValues["middle_name"]) != icdoPerson.middle_name ||
                    Convert.ToString(icdoPerson.ihstOldValues["ssn"]) != icdoPerson.ssn) ||
                    (Convert.ToDateTime(icdoPerson.ihstOldValues["date_of_birth"]) != icdoPerson.date_of_birth) ||
                    Convert.ToString(icdoPerson.ihstOldValues["home_phone_no"]) != icdoPerson.home_phone_no ||
                    Convert.ToString(icdoPerson.ihstOldValues["gender_value"]) != icdoPerson.gender_value))  || 
                    (iblnIsFromMSS && (Convert.ToString(icdoPerson.ihstOldValues["home_phone_no"]) != icdoPerson.home_phone_no ||
                    Convert.ToString(icdoPerson.ihstOldValues["work_phone_no"]) != icdoPerson.work_phone_no ||
                    Convert.ToString(icdoPerson.ihstOldValues["cell_phone_no"]) != icdoPerson.cell_phone_no)))
                {
                    ibusPersonAccountMedicare = new busPersonAccountMedicarePartDHistory();
                    ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();

                    if (ibusPersonAccountMedicare.FindMedicareByPersonID(icdoPerson.person_id))
                    {
                        //Insert a new history record if any change in person maintenance is made.
                        cdoPersonAccountMedicarePartDHistory lobjHistory = new cdoPersonAccountMedicarePartDHistory();
                        ibusPersonAccountMedicare.FindPersonAccount(ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.person_account_id);    
                        lobjHistory.person_account_id = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.person_account_id;
                        lobjHistory.person_id = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.person_id;
                        lobjHistory.start_date = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.start_date;
                        lobjHistory.plan_participation_status_value = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value;
                        lobjHistory.status_value = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.status_value;

                        lobjHistory.reason_value = busConstant.ChangeReasonDemographic;
                        lobjHistory.suppress_warnings_flag = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.suppress_warnings_flag;
                        lobjHistory.medicare_claim_no = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.medicare_claim_no;
                        lobjHistory.medicare_part_a_effective_date = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.medicare_part_a_effective_date;
                        lobjHistory.medicare_part_b_effective_date = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.medicare_part_b_effective_date;
                        lobjHistory.low_income_credit = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.low_income_credit;
                        lobjHistory.late_enrollment_penalty = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty;
                        lobjHistory.member_person_id = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.member_person_id;

                        if (ibusPersonAccountMedicare.iclbPersonAccountMedicarePartDHistory == null)
                            ibusPersonAccountMedicare.LoadPersonAccountMedicarePartDHistory(ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.person_id);

                        lobjHistory.record_type_flag = "C";
                        lobjHistory.enrollment_file_sent_flag = busConstant.Flag_No;
                        lobjHistory.send_after = DateTime.Now.Date;

                        if (ibusPersonAccountMedicare.iclbPersonAccountMedicarePartDHistory.Where(i => i.icdoPersonAccountMedicarePartDHistory.start_date != i.icdoPersonAccountMedicarePartDHistory.end_date
                            && i.icdoPersonAccountMedicarePartDHistory.start_date > i.icdoPersonAccountMedicarePartDHistory.end_date &&  
                            i.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Count() == 0)
                            lobjHistory.initial_enroll_date = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.start_date;
                        else
                            lobjHistory.initial_enroll_date = ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.initial_enroll_date;

                        //PIR 24309
                        ibusPersonAccountMedicare.LoadActiveProviderOrgPlan(lobjHistory.start_date);
                        lobjHistory.provider_org_id = ibusPersonAccountMedicare.ibusProviderOrgPlan.icdoOrgPlan.org_id;

                        lobjHistory.Insert();

                        if (ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.start_date == ibusPersonAccountMedicare.icdoPersonAccount.history_change_date)
                            ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.end_date = ibusPersonAccountMedicare.icdoPersonAccount.history_change_date;
                        else
                            ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.end_date = ibusPersonAccountMedicare.icdoPersonAccount.history_change_date.AddDays(-1);

                        ibusPersonAccountMedicare.icdoPersonAccountMedicarePartDHistory.Update();
                    }

                }

            }
        }

         //PIR-15430
        //This Method checks whether the start date i.e. effective date of the current address is past date or not
        public bool IsValidStartDateESS()
        {
            bool lblnValidDate = true;
            if (_ibusPersonCurrentAddress.icdoPersonAddress.start_date.Date < DateTime.Now.Date)
             {
                 lblnValidDate = false;
             }
            return lblnValidDate;
        }

        private Collection<busPersonAddress> _iclbOtherAddresses;
        public Collection<busPersonAddress> iclbOtherAddresses
        {
            get { return _iclbOtherAddresses; }
            set { _iclbOtherAddresses = value; }
        }

        //PIR-15430
        //This Method checks whether the start date i.e. effective date of the current address is less than other future dated records start date or not
        public bool IsStartDateInValidForAddressType()
        {
            if (ibusPersonCurrentAddress != null)
            {
                ibusPersonCurrentAddress.iclbOtherAddresses = null;
                ibusPersonCurrentAddress.LoadPreviousAddressOfCurrentType();
                if ((ibusPersonCurrentAddress.ibusPreviousAddressOfCurrentType.icdoPersonAddress.start_date != DateTime.MinValue)
                && (ibusPersonCurrentAddress.icdoPersonAddress.start_date != DateTime.MinValue))
                {
                    if (ibusPersonCurrentAddress.ibusPreviousAddressOfCurrentType.icdoPersonAddress.start_date >= ibusPersonCurrentAddress.icdoPersonAddress.start_date)
                        return true;
                }
            }
            return false;
        }

        //PIR-15430
        public bool IsDatesOverlapping()
        {
            bool lblnRecordMatch = false;
            if (ibusPersonCurrentAddress != null)
            {
                var lGetAddressListOfCurrentType = ibusPersonCurrentAddress.iclbOtherAddresses.Where(lobjAdd => lobjAdd.icdoPersonAddress.address_type_value == ibusPersonCurrentAddress.icdoPersonAddress.address_type_value);
                foreach (busPersonAddress lobjPersonAddress in lGetAddressListOfCurrentType)
                {
                    if ((ibusPersonCurrentAddress.icdoPersonAddress.address_type_value == lobjPersonAddress.icdoPersonAddress.address_type_value) &&
                        (ibusPersonCurrentAddress.icdoPersonAddress.start_date < lobjPersonAddress.icdoPersonAddress.end_date))
                    {
                        lblnRecordMatch = true;
                        break;
                    }
                }
              return lblnRecordMatch;
            }
            return lblnRecordMatch;
        }
        public busPayeeAccount ibusMemberRefundPayeeAccount { get; set; }
        public void LoadMemberRefundPayeeAccount()
        {
            if (ibusMemberRefundPayeeAccount == null)
                ibusMemberRefundPayeeAccount = new busPayeeAccount();
            if (iclbPayeeAccount == null)
                LoadPayeeAccount();
            iclbPayeeAccount = busGlobalFunctions.Sort<busPayeeAccount>("icdoPayeeAccount.payee_account_id desc", iclbPayeeAccount);
            ibusMemberRefundPayeeAccount = iclbPayeeAccount.Where(o => o.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember &&
                                                                o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                                                                .FirstOrDefault();
        }
        //Welcome Letter Generation on button click instead of Popup
		
        public string WelcomeLetterCorrespondenceGeneration()
        {
            try
            {
                string lstrFileName = string.Empty;
                //ArrayList larrlist = new ArrayList();
                //larrlist.Add(this);
                Hashtable lhstdummytable = new Hashtable();
                lhstdummytable.Add("sfwcallingform", "batch");
                utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence("ENR-5953", this, lhstdummytable);
                if (lobjCorresPondenceInfo.istrGeneratedFileName.IsNotNull())
                {
					//CorBuilder's CreateCorrespondenceFromTemplate Method was trying to print the welcome letter, 
                    Sagitec.CorBuilder.CorBuilderXML lobjCorBuilder = new Sagitec.CorBuilder.CorBuilderXML();
                    lstrFileName = lobjCorBuilder.CreateCorrespondenceFromTemplate("ENR-5953", lobjCorresPondenceInfo, iobjPassInfo.istrUserID);
                }
                return System.IO.Path.Combine(lobjCorresPondenceInfo.istrGeneratePath, lstrFileName);
            }
            catch(Exception ex)
            {
                Sagitec.ExceptionPub.ExceptionManager.Publish(ex);
                return "Exception - " + ex.Message;
            }
        }
        
		//PIR - 5882
        public DataSet LoadPaymentMethodDiscrepancies()
        {
            DataSet ldsPaymentMethodDiscrepancies = new DataSet();
            DataTable ldtReportTabl01 = new DataTable();
            DataTable ldtReportTabl02 = new DataTable();
            DataTable ldtReportTabl03 = new DataTable();
            ldtReportTabl01 = Select("cdoPerson.PaymentMethodDiscrepanciesForHealth&Medicare", new object[0] { });
            ldtReportTabl02 = Select("cdoPerson.PaymentMethodDiscrepanciesForAllACH", new object[0] { });
            ldtReportTabl03 = Select("cdoPerson.UpdatePayeeAccountsInPaymentElection", new object[0] { });
            ldtReportTabl01.TableName = busConstant.ReportTableName;
            ldtReportTabl02.TableName = busConstant.ReportTableName02;
            ldtReportTabl03.TableName = busConstant.ReportTableName03;
            ldsPaymentMethodDiscrepancies.Tables.Add(ldtReportTabl01.Copy());
            ldsPaymentMethodDiscrepancies.Tables.Add(ldtReportTabl02.Copy());
            ldsPaymentMethodDiscrepancies.Tables.Add(ldtReportTabl03.Copy());
            return ldsPaymentMethodDiscrepancies;
        }

        // PIR - 17242 Displaying First and last comment with respective detail in BIS Error Report
        public DataSet LoadPayrollDetailsInReviewForBISErrors()
        {
            DataSet ldsPayrollDetailsInReviewForBISErrors = new DataSet();
            DataTable ldtReportTabl01 = new DataTable();
            const string lstrSpaceSeperator = " ";
            StringBuilder sb = new StringBuilder();
            ldtReportTabl01 = Select("cdoEmployerPayrollDetail.rptPayrollDetailsInReviewForBISErrors", new object[0] { });

            foreach (DataRow lDataRow in ldtReportTabl01.Rows)
            {
              busEmployerPayrollDetail iobjbusEmployerPayrollDetail = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
              iobjbusEmployerPayrollDetail.icdoEmployerPayrollDetail.LoadData(lDataRow);
              iobjbusEmployerPayrollDetail.LoadEmployerPayrollDetailComments();

              if (iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.Count > 0)
              {
                  sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.FirstOrDefault().icdoComments.comments.ToString());
                  sb.Append(lstrSpaceSeperator);
                  sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.FirstOrDefault().icdoComments.created_by.ToString());
                  sb.Append(lstrSpaceSeperator);
                  sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.FirstOrDefault().icdoComments.created_date.ToString());
                  if (iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.Count > 1)
                  {
                    sb.Append(";");
                    sb.Append(lstrSpaceSeperator);
                    sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.Last().icdoComments.comments.ToString());
                    sb.Append(lstrSpaceSeperator);
                    sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.Last().icdoComments.created_by.ToString());
                    sb.Append(lstrSpaceSeperator);
                    sb.Append(iobjbusEmployerPayrollDetail.iclbPayrollDetailCommentsHistory.Last().icdoComments.created_date.ToString());
                  }
                  lDataRow["Comments"]= sb.ToString();
                  sb.Clear();
              }
            }
        
            ldtReportTabl01.TableName = busConstant.ReportTableName;
            ldsPayrollDetailsInReviewForBISErrors.Tables.Add(ldtReportTabl01.Copy());
            return ldsPayrollDetailsInReviewForBISErrors;
        }
		
        // PIR - 17572
        public bool LoadInvalidSSN(string astrSSN)
        {
            return DBFunction.DBSelect("cdoPersonInvalidSsnRef.InvalidSSN", new object[1] { astrSSN },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework).Rows.Count > 0;
        }
		
        public bool iblnLoadOnlyRequiredFieldsForVSCOrPSC { get; set; } //PIR 18567

        /// <summary>
        /// PIR 18492-This function is used to lock the user after 6 consecutive unsuccessful login. After every successful login failed_login_attempt_count set to zero and is_user_locked flag to N.
        /// </summary>
        /// <param name="lboolIsLoginSuccess"></param>
        /// <returns></returns>
        //public bool SetUserLock(bool lboolIsLoginSuccess, string lstrProfileUserId)
        //{
        //   bool lboolIsUserLocked = false;
        //   if (lboolIsLoginSuccess)
        //   {
        //        this.icdoPerson.failed_login_attempt_count = 0;
        //        this.icdoPerson.is_user_locked = "N";
        //    }
        //    else
        //    {
        //        this.icdoPerson.failed_login_attempt_count = this.icdoPerson.failed_login_attempt_count + 1;
        //        if (this.icdoPerson.failed_login_attempt_count >= 6)
        //        {
        //            this.icdoPerson.is_user_locked = "Y";
        //            lboolIsUserLocked = true;
        //            if (!String.IsNullOrEmpty(icdoPerson.email_address) && icdoPerson.email_waiver_flag != "Y")
        //            {
        //                HelperFunction.SendMail(NeoSpin.Common.ApplicationSettings.Instance.WSSMailFrom.ToString(),
        //                                                            this.icdoPerson.email_address,
        //                                                            "MSS Login",
        //                                                            "Your NDPERS account has been locked. To unlock your account please contact NDPERS for assistance by calling (701) 328-3900 if you have not done so already.");
        //            }
        //        }
        //      }
        //    this.icdoPerson.iobjPassInfo.istrUserID = lstrProfileUserId;
        //    this.icdoPerson.Update();
        //    return lboolIsUserLocked;
        //}
        public bool IsHealthAltStructureCodeHDHP()
        {
            return  ibusESSPersonAccountHealthGhdv.IsNotNull()
                    && ibusESSPersonAccountHealthGhdv.icdoPersonAccountGhdv?.alternate_structure_code_value == busConstant.AlternateStructureCodeHDHP
                    && ibusESSPersonAccountHealthGhdv.iclbPersonAccountGhdvHsa.IsNotNull();
                    //&& ibusESSPersonAccountHealthGhdv.iclbPersonAccountGhdvHsa.Count > 0;
        }

        //PIR 20807
        public bool Is1901ResourceForUser()
        {
            bool lblnHas1901Resource = false;
            // check if user has 1901 Resource

            DataTable ldtbCount = busNeoSpinBase.Select("cdoUser.GetResourceByUser", new object[2] { busConstant.PersonBenefitApplicationRestrictAccountResource, iobjPassInfo.iintUserSerialID });
            if (ldtbCount.Rows.Count > 0)
            {
                lblnHas1901Resource = true;
            }
            return lblnHas1901Resource;
        }
        //PIR 20807
        public string istrIsRestrictedFlag
        {
            get
            {
                return (icdoPerson.restriction_flag == "N" || icdoPerson.restriction_flag.IsNullOrEmpty()) ? busConstant.Flag_No_Value : busConstant.Flag_Yes_Value;
            }
        }
         //PIR 20868
		public int iintPersonAddressID { get; set; }
        public void LoadPersonAddressFromAddressId()
        {
            _ibusPersonCurrentAddress = new busPersonAddress();
            _ibusPersonCurrentAddress.icdoPersonAddress = new cdoPersonAddress();
            if (iclbPersonAddress == null)
                LoadPersonAddress();
            _ibusPersonCurrentAddress = iclbPersonAddress.FirstOrDefault(i => i.icdoPersonAddress.person_address_id == this.iintPersonAddressID);
        }

        //PIR 22732
        public bool iblnHealthEnrolled { get; set; } = false;
        public bool iblnDentalEnrolled { get; set; } = false;
        public bool iblnVisionEnrolled { get; set; } = false;
        public bool iblnEAPEnrolled { get; set; } = false;
        public bool iblnLifeEnrolled { get; set; } = false;
        public bool iblnLTCEnrolled { get; set; } = false;
        public bool iblnFlexEnrolled { get; set; } = false;

        public void IsPlanEnrolled()
        {
            if (icolPersonAccount == null)
                LoadPersonAccount(false);
            if (icolPersonAccount.IsNotNull() && icolPersonAccount.Count > 0)
            {
                if (icolPersonAccount.Any(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth && i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
                        iblnHealthEnrolled = true;
                if (icolPersonAccount.Any(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdDental && i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
                        iblnDentalEnrolled = true;
                if (icolPersonAccount.Any(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdVision && i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
                        iblnVisionEnrolled = true;
                if (icolPersonAccount.Any(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdEAP && i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
                        iblnEAPEnrolled = true;
                if (icolPersonAccount.Any(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife && i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
                        iblnLifeEnrolled = true;
                if (icolPersonAccount.Any(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdLTC && i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled))
                        iblnLTCEnrolled = true;
                if (icolPersonAccount.Any(i => i.icdoPersonAccount.plan_id == busConstant.PlanIdFlex && i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusFlexCompEnrolled))
                        iblnFlexEnrolled = true;
            }
        }
        public bool IsACAEligibilityCertificationButtonVisible()
        {
            DataTable ldtEmployeeWithActiveHealthPlan = new DataTable();
            if (ibusESSPersonEmployment?.ibusOrganization?.icdoOrganization != null)
            {
                ldtEmployeeWithActiveHealthPlan = Select("cdoPerson.OrgWithActivehealthPlan", new object[3] { this.ibusESSPersonEmployment.icdoPersonEmployment.person_employment_id, this.ibusESSPersonEmployment.ibusOrganization.icdoOrganization.org_id, icdoPerson.person_id });
            }
            if (ldtEmployeeWithActiveHealthPlan.Rows.Count > 0)
                return true;
            return false;
        }

        public void LoadACAEligibilityCertification(int aintPersonEmpDetId)
        {
            ibusWssEmploymentAcaCert = new busWssEmploymentAcaCert { icdoWssEmploymentAcaCert = new cdoWssEmploymentAcaCert() };
            if (aintPersonEmpDetId > 0)
            {
                DataTable ldtACAEligibilityCertification = Select<cdoWssEmploymentAcaCert>(new string[2] { enmWssEmploymentAcaCert.person_id.ToString(), enmWssEmploymentAcaCert.per_emp_dtl_id.ToString() },
                                                                   new object[2] { icdoPerson.person_id, aintPersonEmpDetId }, null, "wss_employment_aca_cert_id desc");
                if (ldtACAEligibilityCertification.Rows.Count > 0)
                {
                    ibusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.LoadData(ldtACAEligibilityCertification.Rows[0]);
                }
            }
        }
        public void LoadACAEligibilityCertifications(int aintPersonEmpDetId)
        {
            iclbWssEmploymentAcaCert = new Collection<busWssEmploymentAcaCert>();
            if (aintPersonEmpDetId > 0)
            {
                DataTable ldtACAEligibilityCertification = Select<cdoWssEmploymentAcaCert>(new string[2] { enmWssEmploymentAcaCert.person_id.ToString(), enmWssEmploymentAcaCert.per_emp_dtl_id.ToString() },
                                                                   new object[2] { icdoPerson.person_id, aintPersonEmpDetId }, null, "wss_employment_aca_cert_id desc");
                if (ldtACAEligibilityCertification.Rows.Count > 0)
                {
                    iclbWssEmploymentAcaCert = GetCollection<busWssEmploymentAcaCert>(ldtACAEligibilityCertification, "icdoWssEmploymentAcaCert");
                    foreach (busWssEmploymentAcaCert lbusresult in iclbWssEmploymentAcaCert)
                    {
                        lbusresult.istrMethodDescription = busGlobalFunctions.GetDescriptionByCodeValue(7031, lbusresult.icdoWssEmploymentAcaCert.method, iobjPassInfo);
                        if (lbusresult.icdoWssEmploymentAcaCert.method == busConstant.ACACertificationMethodLookBack && lbusresult.icdoWssEmploymentAcaCert.lb_measure.IsNotNullOrEmpty())
                            lbusresult.istrLBMethodType = busGlobalFunctions.GetDescriptionByCodeValue(7032, lbusresult.icdoWssEmploymentAcaCert.lb_measure, iobjPassInfo);
                    }
                }
            }
        }
        public bool IsLookBackMeasureIsNull()
        {
            bool lblnResult = false;
            LoadACAEligibilityCertifications(ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id);
            if (iclbWssEmploymentAcaCert.Count > 0)
            {
                foreach (busWssEmploymentAcaCert lbusWssEmploymentAcaCert in iclbWssEmploymentAcaCert)
                {
                    if (lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.lb_measure == null || lbusWssEmploymentAcaCert.icdoWssEmploymentAcaCert.method == busConstant.ACACertificationMethodNewHire)
                        lblnResult = true;
                }
            }
            return lblnResult;
        }
        //PIR 24755 load method for buspaymentelectionadjusment
        public void LoadPaymentElectionAdjustment()
        {
            if (ibusPaymentElectionAdjustment.IsNull())
                ibusPaymentElectionAdjustment = new busPaymentElectionAdjustment()
                {
                    icdoPaymentElectionAdjustment = new cdoPaymentElectionAdjustment(),
                    ibusPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount(), ibusPlan = new busPlan() { icdoPlan = new cdoPlan() }, ibusPerson = new busPerson() },
                };
            ibusPaymentElectionAdjustment.ibusPersonAccount.ibusPerson.icdoPerson = this.icdoPerson;
            
        }

        //PIR 23482
        public ArrayList btnCertifyPerson_Click()
        {
            ArrayList larrList = new ArrayList();
            if (icdoPerson.IsNotNull())
            {
                icdoPerson.certify_date = DateTime.Now;
                icdoPerson.activation_code_date = DateTime.Now;
                icdoPerson.activation_code_flag = busConstant.Flag_Yes;
                if (icdoPerson.email_address.IsNullOrEmpty())
                {
                    icdoPerson.email_waiver_date = DateTime.Now;
                    icdoPerson.email_waiver_flag = busConstant.Flag_Yes;
                }
                icdoPerson.Update();

                larrList.Add(this);
                return larrList;
            }
            return larrList;
        }
        public Collection<busDeathNotice> iclbDeathNoticeForGrid { get; set; }
        public void LoadDeathNoticeForGrid()
        {
            DataTable ldtbList = Select("entPerson.LoadPersonDeathNotices", new object[1] { icdoPerson.person_id });
            iclbDeathNoticeForGrid = GetCollection<busDeathNotice>(ldtbList, "icdoDeathNotice");
        }
        // PIR 24927
        public bool iblnIsRetirementPlanEnrlOrRetrOrSus
        {
            get
            {
                bool lblnIsRetirementPlanEnrlOrRetrOrSus = false;

                if (this.icolPersonAccount.IsNull())
                    LoadPersonAccount();
                foreach (busPersonAccount lobjPersonAccount in icolPersonAccount)
                {
                    lobjPersonAccount.LoadPlan();
                    if (lobjPersonAccount.ibusPlan.IsRetirementPlan() || lobjPersonAccount.ibusPlan.IsDCRetirementPlan())
                    {
                        if (lobjPersonAccount.IsPlanParticipationStatusRetiredOrSuspendedOrEnrolled())
                            lblnIsRetirementPlanEnrlOrRetrOrSus = true;
                    }
                }
                return lblnIsRetirementPlanEnrlOrRetrOrSus;
            }
        }

        // PIR 24927
        public bool iblnIsLifeInsurancePlanEnrolled
        {
            get
            {
                if (this.icolPersonAccount.IsNull())
                    LoadPersonAccount();
                return (icolPersonAccount.Where(i => i.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife &&
                                                i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Any());

            }
        }

        public busPersonAccount ibusDC2025PersonAccount { get; set; }
        public busPersonAccount ibusPersonAccountForOptionalElection { get; set; }
        public void LoadDC2025PersonAccount()
        {
            if (ibusDC2025PersonAccount == null)
                ibusDC2025PersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            LoadPersonAccountByPlan(busConstant.PlanIdDC2025);
            if (icolPersonAccountByPlan.Count > 0)
                ibusDC2025PersonAccount = icolPersonAccountByPlan.First();
        }

        //PIR 26615 TFFR Header grid on Person Maintenance
        public Collection<busPersonTffrHeader> iclbPersonTffrHeader { get; set; }
        public void LoadPersonTffrHeader()
        {
            if(icdoPerson.person_id > 0)
            {
                DataTable ldtbPersonTffrHeader = Select<doPersonTffrHeader>(new string[1] { "person_id" },
                                new object[1] { icdoPerson.person_id }, null, "upload_date desc");

                if (iclbPersonTffrHeader.IsNullOrEmpty())
                    iclbPersonTffrHeader = new Collection<busPersonTffrHeader>();

                foreach (DataRow ldr in ldtbPersonTffrHeader.Rows)
                {
                    busPersonTffrHeader lobjPersonTffrHeader = new busPersonTffrHeader { icdoPersonTffrHeader = new doPersonTffrHeader() };
                    lobjPersonTffrHeader.icdoPersonTffrHeader.LoadData(ldr);
                    iclbPersonTffrHeader.Add(lobjPersonTffrHeader);
                }
            }
        }



        public void UpdateTFFRRequest()
        {
            if (icdoPerson.person_id > 0)
            {
                FindPerson(icdoPerson.person_id);
                icdoPerson.tffr_request = busConstant.Flag_Yes;
                icdoPerson.Update();

            }
        }
        public void EndOfLifeDocumentsReceived()
        {
            if (icdoPerson.person_id > 0)
            {
                FindPerson(icdoPerson.person_id);
                icdoPerson.end_of_life_docs = busConstant.Flag_Yes;
                icdoPerson.Update();
            }
        }
        public void LoadPersonForBPM(int aintPersonID)
        {
            if(aintPersonID > 0)
                FindPerson(aintPersonID);
        }

    }
}
        
