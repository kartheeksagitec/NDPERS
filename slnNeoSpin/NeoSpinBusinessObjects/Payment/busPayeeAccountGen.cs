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
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPayeeAccountGen : busExtendBase
    {
        public busPayeeAccountGen()
        {

        }
        public DataTable idtbPaymentItemType { get; set; }


        public string istrPayee { get; set; }

        public void LoadPayeeName()
        {
            if (icdoPayeeAccount.payee_perslink_id > 0)
            {
                if (ibusPayee == null)
                    LoadPayee();
                istrPayee = ibusPayee.icdoPerson.PayeeName;
            }
            else if (icdoPayeeAccount.payee_org_id > 0)
            {
                if (ibusRecipientOrganization == null)
                    LoadRecipientOrganization();
                istrPayee = ibusRecipientOrganization.icdoOrganization.RecipientOrg;
            }
        }
        public string lstrPayee
        {
            get
            {
                if (icdoPayeeAccount.payee_perslink_id > 0)
                {
                    if (ibusPayee == null)
                        LoadPayee();
                    return ibusPayee.icdoPerson.PayeeName;
                }
                else if (icdoPayeeAccount.payee_org_id > 0)
                {
                    if (ibusRecipientOrganization == null)
                        LoadRecipientOrganization();
                    return ibusRecipientOrganization.icdoOrganization.RecipientOrg;
                }
                return string.Empty;
            }
        }
        //Check whether the payee account is created for rhic_ee_amount alone with benefit type refund and benefit option is DB to TFFR or TIAA transfer options
        public bool IsitRefundRHICPayeeAccount()
        {
            if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                busBenefitRefundCalculation lobjBenefitRefundCalculation = new busBenefitRefundCalculation();
                lobjBenefitRefundCalculation.FindBenefitRefundCalculation(icdoPayeeAccount.calculation_id);
                if (lobjBenefitRefundCalculation.IsBenefitOptionTFFROrTIAA())
                {
                    if (icdoPayeeAccount.rhic_ee_amount_refund_flag == busConstant.Flag_Yes)
                        return true;
                }
            }
            return false;
        }
        private cdoPayeeAccount _icdoPayeeAccount;
        public cdoPayeeAccount icdoPayeeAccount
        {
            get
            {
                return _icdoPayeeAccount;
            }
            set
            {
                _icdoPayeeAccount = value;
            }
        }
        public decimal idecNontaxableAmount { get; set; }
        private decimal _idecExclusionAmount;

        public decimal idecExclusionAmount
        {
            get { return _idecExclusionAmount; }
            set { _idecExclusionAmount = value; }
        }


        private busBenefitCalculation _ibusBenefitCalculaton;
        public busBenefitCalculation ibusBenefitCalculaton
        {
            get { return _ibusBenefitCalculaton; }
            set { _ibusBenefitCalculaton = value; }
        }

        public void LoadBenefitCalculation()
        {
            if (ibusBenefitCalculaton == null)
                ibusBenefitCalculaton = new busBenefitCalculation();
            ibusBenefitCalculaton.FindBenefitCalculation(icdoPayeeAccount.calculation_id);
        }
        //PIR 25920 DC 2025 Changes 
        private busBenefitRefundCalculation _ibusBenefitRefundCalculaton;
        public busBenefitRefundCalculation ibusBenefitRefundCalculaton
        {
            get { return _ibusBenefitRefundCalculaton; }
            set { _ibusBenefitRefundCalculaton = value; }
        }
        public void LoadBenefitRefundCalculation()
        {
            if (ibusBenefitRefundCalculaton == null)
                ibusBenefitRefundCalculaton = new busBenefitRefundCalculation();
            ibusBenefitRefundCalculaton.FindBenefitRefundCalculation(icdoPayeeAccount.calculation_id);
        }
        public bool FindPayeeAccount(int Aintpayeeaccountid)
        {
            bool lblnResult = false;
            if (_icdoPayeeAccount == null)
            {
                _icdoPayeeAccount = new cdoPayeeAccount();
            }
            if (_icdoPayeeAccount.SelectRow(new object[1] { Aintpayeeaccountid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        //PIR 18503
        public bool FindPayeeAccountByPersonID(int AintPayeePerslinkID)
        {
            bool lblnResult = false;

            DataTable ldtbList = Select<cdoPayeeAccount>(
                new string[1] { "payee_perslink_id" },
                new object[1] { AintPayeePerslinkID }, null, null);
            if (icdoPayeeAccount == null)
                icdoPayeeAccount = new cdoPayeeAccount();
            if (ldtbList.Rows.Count > 0)
            {
                icdoPayeeAccount.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        //Fw upgrade issues - For workflow process
        public bool FindPayeeAccount(long Aintpayeeaccountid)
        {
            bool lblnResult = false;
            if (_icdoPayeeAccount == null)
            {
                _icdoPayeeAccount = new cdoPayeeAccount();
            }
            if (_icdoPayeeAccount.SelectRow(new object[1] { Aintpayeeaccountid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        #region valiadation prperties
        //Properties used for validating the payee account
        private bool _iblnAddionalContributionsIndicatorFlag;
        public bool iblnAddionalContributionsIndicatorFlag
        {
            get { return _iblnAddionalContributionsIndicatorFlag; }
            set { _iblnAddionalContributionsIndicatorFlag = value; }
        }
        public bool iblnAdditionalInterestIndicatorFlag { get; set; }
        public bool iblnApplicationStatusIndicator { get; set; }
        public bool iblnDeathNotificationIndicator { get; set; }
        public bool iblnACHInfoChangeIndicator { get; set; }
        public bool iblnNewEmploymentIndicator { get; set; }
        public bool iblnNullifyIndicator { get; set; }
        public bool iblnInvalidNetAmountIndicator { get; set; }
        public bool iblnPayeeAccountInfoChangeIndicator { get; set; }
        public bool iblnACHInfoDeleteIndicator { get; set; }
        public bool iblnTaxwithholdingInfoChangeIndicator { get; set; }
        public bool iblnTaxWithholdingInfoDeleteIndicator { get; set; }
        public bool iblnRolloverInfoChangeIndicator { get; set; }
        public bool iblnRolloverInfoDeleteIndicator { get; set; }
        public bool iblnNewUnderPaymentIndicator { get; set; }
        public bool iblnAdjustmentChangeIndicator { get; set; }
        public bool iblnAdjustmentInfoDeleteIndicator { get; set; }
        public bool iblnDedudtionInfoChangeIndicator { get; set; }
        public bool iblnNoTaxWithholdingRecordIndicator { get; set; }
        public bool iblnInvalidACHIndicator { get; set; }
        public bool iblnInvalidRolloverIndicator { get; set; }
        public bool iblnIsMembersDeathNotified { get; set; }
        public bool iblnIsPayeesDeathNotified { get; set; }
        public bool iblnNewPayeeAccountIndicator { get; set; }
        public bool iblnPayeeAccountChangeIndicator { get; set; }
        public bool iblnOverpaymentDetailsChangeIndicator { get; set; }
        public bool iblnRecoveryChangeIndicator { get; set; }
        public bool iblnPaymentCancelledIndicator { get; set; }
        public bool iblnPayeeRestrictedIndicator { get; set; }
        public bool iblnPayeeAddressNotExistsIndicator { get; set; }
        public bool iblnIsRHICGreaterThan3rdParty { get; set; }
        public bool iblnIsApprovedAutoRefundAndGrossAmountCheck { get; set; }


        #endregion
        private decimal _idecGrossAmount;
        public decimal idecGrossAmount
        {
            get { return _idecGrossAmount; }
            set { _idecGrossAmount = value; }
        }

        private DateTime _idtNextBenefitPaymentDate;
        public DateTime idtNextBenefitPaymentDate
        {
            get { return _idtNextBenefitPaymentDate; }
            set { _idtNextBenefitPaymentDate = value; }
        }

        private DateTime _idtLastBenefitPaymentDate;
        public DateTime idtLastBenefitPaymentDate
        {
            get { return _idtLastBenefitPaymentDate; }
            set { _idtLastBenefitPaymentDate = value; }
        }

        private busPayeeAccountAchDetail _ibusPrimaryAchDetail;
        public busPayeeAccountAchDetail ibusPrimaryAchDetail
        {
            get { return _ibusPrimaryAchDetail; }
            set { _ibusPrimaryAchDetail = value; }
        }

        private decimal _idecTotalAchAmount;
        public decimal idecTotalAchAmount
        {
            get { return _idecTotalAchAmount; }
            set { _idecTotalAchAmount = value; }
        }

        private decimal _idecBenefitAmount;
        public decimal idecBenefitAmount
        {
            get { return _idecBenefitAmount; }
            set { _idecBenefitAmount = value; }
        }
        public decimal idecTotalAccountBalance { get; set; }
        private decimal _idecTotalTaxableAmount;
        public decimal idecTotalTaxableAmount
        {
            get { return _idecTotalTaxableAmount; }
            set { _idecTotalTaxableAmount = value; }
        }
        private decimal _idecTotalTaxableAmountForFlatRates;
        public decimal idecTotalTaxableAmountForFlatRates
        {
            get { return _idecTotalTaxableAmountForFlatRates; }
            set { _idecTotalTaxableAmountForFlatRates = value; }
        }

        private decimal _idecTotalTaxableAmountForVariableTax;
        public decimal idecTotalTaxableAmountForVariableTax
        {
            get { return _idecTotalTaxableAmountForVariableTax; }
            set { _idecTotalTaxableAmountForVariableTax = value; }
        }

        private decimal _idecTotalAchPercentage;
        public decimal idecTotalAchPercentage
        {
            get { return _idecTotalAchPercentage; }
            set { _idecTotalAchPercentage = value; }
        }

        private decimal _idecTotalRolloverAmount;
        public decimal idecTotalRolloverAmount
        {
            get { return _idecTotalRolloverAmount; }
            set { _idecTotalRolloverAmount = value; }
        }

        private decimal _idecTotalRolloverPercentage;
        public decimal idecTotalRolloverPercentage
        {
            get { return _idecTotalRolloverPercentage; }
            set { _idecTotalRolloverPercentage = value; }
        }

        private decimal _idecTotalDeductionsAmount;
        public decimal idecTotalDeductionsAmount
        {
            get { return _idecTotalDeductionsAmount; }
            set { _idecTotalDeductionsAmount = value; }
        }

        private Collection<busPayeeAccountAchDetail> _iclbActiveACHDetails;
        public Collection<busPayeeAccountAchDetail> iclbActiveACHDetails
        {
            get { return _iclbActiveACHDetails; }
            set { _iclbActiveACHDetails = value; }
        }
        private Collection<busPayeeAccountAchDetail> _iclbFutureDatedACHDetails;
        public Collection<busPayeeAccountAchDetail> iclbFutureDatedACHDetails
        {
            get { return _iclbFutureDatedACHDetails; }
            set { _iclbFutureDatedACHDetails = value; }
        }

        //PIR 18503
        private Collection<busPayeeAccountAchDetail> _iclbACHDetailsWithEndDateNull;
        public Collection<busPayeeAccountAchDetail> iclbACHDetailsWithEndDateNull
        {
            get { return _iclbACHDetailsWithEndDateNull; }
            set { _iclbACHDetailsWithEndDateNull = value; }
        }

        private busBenefitApplication _ibusApplication;
        public busBenefitApplication ibusApplication
        {
            get { return _ibusApplication; }
            set { _ibusApplication = value; }
        }

        public busRetirementDisabilityApplication ibusRetirementDisabilityApplication { get; set; }
        public void LoadRetirementDisabilityApplication()
        {
            if (ibusRetirementDisabilityApplication == null)
                ibusRetirementDisabilityApplication = new busRetirementDisabilityApplication();
            ibusRetirementDisabilityApplication.FindBenefitApplication(icdoPayeeAccount.application_id);
        }
        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        private busPerson _ibusPayee;
        public busPerson ibusPayee
        {
            get { return _ibusPayee; }
            set { _ibusPayee = value; }
        }

        private busPerson _ibusMember;
        public busPerson ibusMember
        {
            get { return _ibusMember; }
            set { _ibusMember = value; }
        }

        private busPersonAccount _ibusMemberAccount;
        public busPersonAccount ibusMemberAccount
        {
            get { return _ibusMemberAccount; }
            set { _ibusMemberAccount = value; }
        }

        private busPayeeAccountStatus _ibusPayeeAccountActiveStatus;
        public busPayeeAccountStatus ibusPayeeAccountActiveStatus
        {
            get { return _ibusPayeeAccountActiveStatus; }
            set { _ibusPayeeAccountActiveStatus = value; }
        }

        private busOrganization _ibusRecipientOrganization;
        public busOrganization ibusRecipientOrganization
        {
            get { return _ibusRecipientOrganization; }
            set { _ibusRecipientOrganization = value; }
        }

        private busBenefitAccount _ibusBenefitAccount;
        public busBenefitAccount ibusBenefitAccount
        {
            get { return _ibusBenefitAccount; }
            set { _ibusBenefitAccount = value; }
        }

        private Collection<busPayeeAccountStatus> _iclbPayeeAccountStatus;
        public Collection<busPayeeAccountStatus> iclbPayeeAccountStatus
        {
            get { return _iclbPayeeAccountStatus; }
            set { _iclbPayeeAccountStatus = value; }
        }

        private Collection<busPayeeAccountRetroPayment> _iclbRetroPayment;
        public Collection<busPayeeAccountRetroPayment> iclbRetroPayment
        {
            get { return _iclbRetroPayment; }
            set { _iclbRetroPayment = value; }
        }

        private Collection<busPayeeAccountAchDetail> _iclbAchDetail;
        public Collection<busPayeeAccountAchDetail> iclbAchDetail
        {
            get { return _iclbAchDetail; }
            set { _iclbAchDetail = value; }
        }

        private Collection<busPayeeAccountRolloverDetail> _iclbRolloverDetail;
        public Collection<busPayeeAccountRolloverDetail> iclbRolloverDetail
        {
            get { return _iclbRolloverDetail; }
            set { _iclbRolloverDetail = value; }
        }

        private Collection<busPayeeAccountRolloverDetail> _iclbActiveRolloverDetails;
        public Collection<busPayeeAccountRolloverDetail> iclbActiveRolloverDetails
        {
            get { return _iclbActiveRolloverDetails; }
            set { _iclbActiveRolloverDetails = value; }
        }

        private Collection<busPaymentItemType> _iclbPaymentItemType;
        public Collection<busPaymentItemType> iclbPaymentItemType
        {
            get { return _iclbPaymentItemType; }
            set { _iclbPaymentItemType = value; }
        }

        private Collection<busPayeeAccountTaxWithholding> _iclbPayeeAccountFedTaxWithHolding;
        public Collection<busPayeeAccountTaxWithholding> iclbPayeeAccountFedTaxWithHolding
        {
            get { return _iclbPayeeAccountFedTaxWithHolding; }
            set { _iclbPayeeAccountFedTaxWithHolding = value; }
        }

        private Collection<busPayeeAccountTaxWithholding> _iclbPayeeAccountStateTaxWithHolding;
        public Collection<busPayeeAccountTaxWithholding> iclbPayeeAccountStateTaxWithHolding
        {
            get { return _iclbPayeeAccountStateTaxWithHolding; }
            set { _iclbPayeeAccountStateTaxWithHolding = value; }
        }


        private Collection<busPayeeAccountPaymentItemType> _iclbPaymentItemTypesToRollover;
        public Collection<busPayeeAccountPaymentItemType> iclbPaymentItemTypesToRollover
        {
            get { return _iclbPaymentItemTypesToRollover; }
            set { _iclbPaymentItemTypesToRollover = value; }
        }

        /// <summary>
        /// Loads the Payee Person or Organization
        /// </summary>
        public void LoadPayee()
        {
            if (_icdoPayeeAccount.payee_perslink_id != 0)
                LoadPayeePerson();
            if (_icdoPayeeAccount.payee_org_id != 0)
                LoadRecipientOrganization();
        }


        private busBenefitDroApplication _ibusDROApplication;

        public busBenefitDroApplication ibusDROApplication
        {
            get { return _ibusDROApplication; }
            set { _ibusDROApplication = value; }
        }
        private busBenefitDroCalculation _ibusDROCalculation;

        public busBenefitDroCalculation ibusDROCalculation
        {
            get { return _ibusDROCalculation; }
            set { _ibusDROCalculation = value; }
        }

        public void LoadDROApplication()
        {
            if (ibusDROCalculation == null)
                ibusDROCalculation = new busBenefitDroCalculation();
            if (ibusDROApplication == null)
                ibusDROApplication = new busBenefitDroApplication();
            ibusDROCalculation.FindBenefitDroCalculation(icdoPayeeAccount.dro_calculation_id);

            if (icdoPayeeAccount.dro_application_id > 0)
            {
                //changed for UCS 54
                ibusDROApplication.FindBenefitDroApplication(icdoPayeeAccount.dro_application_id);
            }
            else
            { ibusDROApplication.FindBenefitDroApplication(ibusDROCalculation.icdoBenefitDroCalculation.dro_application_id); }
        }
        public void LoadPayeePerson()
        {
            if (_ibusPayee == null)
            {
                _ibusPayee = new busPerson();
            }
            _ibusPayee.FindPerson(icdoPayeeAccount.payee_perslink_id);
        }

        public void LoadMember()
        {
            if (_ibusMember == null)
            {
                _ibusMember = new busPerson();
            }
            if ((icdoPayeeAccount.dro_calculation_id > 0) || (icdoPayeeAccount.dro_application_id > 0))
            {
                LoadDROApplication();
                _ibusMember.FindPerson(ibusDROApplication.icdoBenefitDroApplication.member_perslink_id);
            }
            else
            {
                _ibusMember.FindPerson(_ibusApplication.icdoBenefitApplication.member_person_id);
            }
        }
        public void LoadApplication()
        {
            if (_ibusApplication == null)
            {
                _ibusApplication = new busBenefitApplication();
            }
            _ibusApplication.FindBenefitApplication(icdoPayeeAccount.application_id);
        }

        public void LoadPlan()
        {
            if (ibusPlan == null)
            {
                ibusPlan = new busPlan();
            }
            if (icdoPayeeAccount.dro_calculation_id > 0)
            {
                if (ibusDROApplication == null)
                    LoadDROApplication();
                ibusPlan.FindPlan(ibusDROApplication.icdoBenefitDroApplication.plan_id);
            }
            else
            {
                if (ibusApplication == null)
                    LoadApplication();
                ibusPlan.FindPlan(ibusApplication.icdoBenefitApplication.plan_id);
            }
        }

        public void LoadRecipientOrganization()
        {
            if (_ibusRecipientOrganization == null)
            {
                _ibusRecipientOrganization = new busOrganization();
            }
            ibusRecipientOrganization.FindOrganization(icdoPayeeAccount.payee_org_id);
        }

        public void LoadBenfitAccount()
        {
            if (_ibusBenefitAccount == null)
            {
                _ibusBenefitAccount = new busBenefitAccount();
            }
            ibusBenefitAccount.FindBenefitAccount(icdoPayeeAccount.benefit_account_id);
        }

        //Load All Status changes of the Payee Account
        public void LoadPayeeAccountStatus()
        {
            DataTable ldtbList = Select<cdoPayeeAccountStatus>(
                  new string[1] { "payee_account_id" },
                  new object[1] { icdoPayeeAccount.payee_account_id }, null, "status_effective_date desc ,payee_account_status_id desc");
            _iclbPayeeAccountStatus = GetCollection<busPayeeAccountStatus>(ldtbList, "icdoPayeeAccountStatus");
        }

        //Load All Retro Payments for the Payee Account
        public void LoadRetroPayment()
        {
            DataTable ldtbList = Select<cdoPayeeAccountRetroPayment>(
                  new string[1] { "payee_account_id" },
                  new object[1] { icdoPayeeAccount.payee_account_id }, null, null);
            _iclbRetroPayment = GetCollection<busPayeeAccountRetroPayment>(ldtbList, "icdoPayeeAccountRetroPayment");
        }
        //Load All Retro Payments for the Payee Account
        public void LoadRetroTaxableAndNonTaxableAmount()
        {
            if (_iclbRetroPayment == null)
                LoadRetroPayment();
            foreach (busPayeeAccountRetroPayment lobjRetroPAyment in _iclbRetroPayment)
            {
                lobjRetroPAyment.LoadRetroTaxableAndNonTaxableAmount();
            }
        }
        //Load All Ach Details for the payee account - Sort ach start date descending order
        public void LoadACHDetail()
        {
            DataTable ldtbList = Select<cdoPayeeAccountAchDetail>(
                  new string[1] { "payee_account_id" },
                  new object[1] { icdoPayeeAccount.payee_account_id }, null, "ach_start_date desc");
            _iclbAchDetail = GetCollection<busPayeeAccountAchDetail>(ldtbList, "icdoPayeeAccountAchDetail");
            foreach (busPayeeAccountAchDetail lobjPayeeAccountAchDetail in _iclbAchDetail)
            {
                lobjPayeeAccountAchDetail.LoadBankOrgByOrgID();
            }
        }

        //Load All Rolover Details for the payee account- Sort by Created date descending order
        public void LoadRolloverDetail()
        {
            DataTable ldtbList = Select<cdoPayeeAccountRolloverDetail>(
                  new string[1] { "payee_account_id" },
                  new object[1] { icdoPayeeAccount.payee_account_id }, null, "created_date desc");
            _iclbRolloverDetail = GetCollection<busPayeeAccountRolloverDetail>(ldtbList, "icdoPayeeAccountRolloverDetail");
            foreach (busPayeeAccountRolloverDetail lobjPayeeAccountRolloverOrg in _iclbRolloverDetail)
            {
                lobjPayeeAccountRolloverOrg.LoadRolloverOrgByOrgID();
            }
        }

        //Load Payment Item Types
        public void LoadPaymentItemType()
        {
            if (idtbPaymentItemType == null)
            {
                idtbPaymentItemType = iobjPassInfo.isrvDBCache.GetCacheData("sgt_payment_item_type", null);
            }
            _iclbPaymentItemType = GetCollection<busPaymentItemType>(idtbPaymentItemType, "icdoPaymentItemType");
        }

        private Collection<busPayeeAccountPaymentItemType> _iclbPayeeAccountPaymentItemType;
        public Collection<busPayeeAccountPaymentItemType> iclbPayeeAccountPaymentItemType
        {
            get { return _iclbPayeeAccountPaymentItemType; }
            set { _iclbPayeeAccountPaymentItemType = value; }
        }

        // Instead of loading every Deduction's Child tables, Loading it from a single query
        public void LoadPayeeAccountPaymentItemType()
        {
            _iclbPayeeAccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();
            DataTable ldtbDeductions = Select("cdoPayeeAccountPaymentItemType.LoadPayeeAccountPaymentItemType", new object[1] { icdoPayeeAccount.payee_account_id });
            foreach (DataRow ldtrDeduction in ldtbDeductions.Rows)
            {
                busPayeeAccountPaymentItemType lobjDeduction = new busPayeeAccountPaymentItemType();
                lobjDeduction.ibusPaymentItemType = new busPaymentItemType();
                lobjDeduction.ibusVendor = new busOrganization();
                lobjDeduction.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
                lobjDeduction.ibusPaymentItemType.icdoPaymentItemType = new cdoPaymentItemType();
                lobjDeduction.ibusVendor.icdoOrganization = new cdoOrganization();

                lobjDeduction.icdoPayeeAccountPaymentItemType.LoadData(ldtrDeduction);
                lobjDeduction.icdoPayeeAccountPaymentItemType.update_seq = Convert.ToInt32(ldtrDeduction["papit_update_seq"]);
                lobjDeduction.ibusPaymentItemType.icdoPaymentItemType.LoadData(ldtrDeduction);
                lobjDeduction.ibusVendor.icdoOrganization.LoadData(ldtrDeduction);
                _iclbPayeeAccountPaymentItemType.Add(lobjDeduction);
            }
        }

        //PIR 18053
        public void LoadPayeeAccountPaymentItemTypeRTW()
        {
            _iclbPayeeAccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();
            DataTable ldtbDeductions = Select("cdoPayeeAccountPaymentItemType.LoadPAPITForRTWRecalculation", new object[1] { icdoPayeeAccount.payee_account_id });
            foreach (DataRow ldtrDeduction in ldtbDeductions.Rows)
            {
                busPayeeAccountPaymentItemType lobjDeduction = new busPayeeAccountPaymentItemType();
                lobjDeduction.ibusPaymentItemType = new busPaymentItemType();
                lobjDeduction.ibusVendor = new busOrganization();
                lobjDeduction.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
                lobjDeduction.ibusPaymentItemType.icdoPaymentItemType = new cdoPaymentItemType();
                lobjDeduction.ibusVendor.icdoOrganization = new cdoOrganization();

                lobjDeduction.icdoPayeeAccountPaymentItemType.LoadData(ldtrDeduction);
                lobjDeduction.icdoPayeeAccountPaymentItemType.update_seq = Convert.ToInt32(ldtrDeduction["papit_update_seq"]);
                lobjDeduction.ibusPaymentItemType.icdoPaymentItemType.LoadData(ldtrDeduction);
                lobjDeduction.ibusVendor.icdoOrganization.LoadData(ldtrDeduction);
                _iclbPayeeAccountPaymentItemType.Add(lobjDeduction);
            }
        }

        private Collection<busRetroItemType> _iclbRetroItemType;
        public Collection<busRetroItemType> iclbRetroItemType
        {
            get { return _iclbRetroItemType; }
            set { _iclbRetroItemType = value; }
        }

        public void LoadRetroItemType(string astrRetroPaymentItemType)
        {
            DataTable ldtbList = Select<cdoRetroItemType>(
                new string[1] { "retro_payment_type_value" },
                new object[1] { astrRetroPaymentItemType }, null, null);
            iclbRetroItemType = GetCollection<busRetroItemType>(ldtbList, "icdoRetroItemType");
        }

        // Deductions displaying in Payee Account Maintenance
        private Collection<busPayeeAccountPaymentItemType> _iclbDeductions;
        public Collection<busPayeeAccountPaymentItemType> iclbDeductions
        {
            get { return _iclbDeductions; }
            set { _iclbDeductions = value; }
        }

        public void LoadDeductions()
        {
            _iclbDeductions = new Collection<busPayeeAccountPaymentItemType>();
            if (_iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            foreach (busPayeeAccountPaymentItemType lobjDeduction in _iclbPayeeAccountPaymentItemType)
            {
                if (lobjDeduction.ibusPaymentItemType == null)
                    lobjDeduction.LoadPaymentItemType();
                //pir 2146 : as discussed with satya
                // PIR 8409 : only open item type deductions should be shown, so end_date check added
                if (lobjDeduction.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == -1 &&
                    lobjDeduction.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.AllowRolloverItemDONT &&
                    //PIR 26130 was not picking up future end dated PAPIT
                    (busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, lobjDeduction.icdoPayeeAccountPaymentItemType.start_date,
                    lobjDeduction.icdoPayeeAccountPaymentItemType.end_date)))
                    _iclbDeductions.Add(lobjDeduction);
            }
        }
        //For MSS Layout change
        public Collection<busPayeeAccountPaymentItemType> iclbActiveDeductions { get; set; }
        public void LoadActiveDeductions()
        {
            iclbActiveDeductions = new Collection<busPayeeAccountPaymentItemType>();
            if (_iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            foreach (busPayeeAccountPaymentItemType lobjDeduction in _iclbPayeeAccountPaymentItemType)
            {
                if (lobjDeduction.ibusPaymentItemType == null)
                    lobjDeduction.LoadPaymentItemType();
                //pir 2146 : as discussed with satya
                if (lobjDeduction.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == -1 &&
                    lobjDeduction.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.AllowRolloverItemDONT &&
                    (busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, lobjDeduction.icdoPayeeAccountPaymentItemType.start_date,
                    lobjDeduction.icdoPayeeAccountPaymentItemType.end_date)))
                {
                    lobjDeduction.idecItemAmount = lobjDeduction.idecamountmultipliedbyitemdirection;
                    iclbActiveDeductions.Add(lobjDeduction);

                }
            }
            busPayeeAccountPaymentItemType lobjPaymentItemType = new busPayeeAccountPaymentItemType() { ibusPaymentItemType = new busPaymentItemType()
            { icdoPaymentItemType = new cdoPaymentItemType()}};
            if (lobjPaymentItemType.icdoPayeeAccountPaymentItemType == null)
            {
                lobjPaymentItemType.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
            }
            lobjPaymentItemType.ibusVendor = new busOrganization(){icdoOrganization = new cdoOrganization()};
            
            lobjPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_description = "Total Deductions Amount";//PIR 8575
            lobjPaymentItemType.ibusVendor.icdoOrganization.org_name = String.Empty;
            lobjPaymentItemType.idecItemAmount = idecTotalDeductionsAmount;
            iclbActiveDeductions.Add(lobjPaymentItemType);


        }
        public busPayeeAccountTaxWithholding ibusPayeeAccountFedTaxWithholding { get; set; }//For MSS Layout change

        //Load the FedTaxWithHolding Information which are specific to the Payee Account      
        public void LoadFedTaxWithHoldingInfo()
        {
            DataTable ldtbList = Select<cdoPayeeAccountTaxWithholding>(
                   new string[1] { "payee_account_id" },
                   new object[1] { icdoPayeeAccount.payee_account_id }, null, "start_date desc");
            _iclbPayeeAccountFedTaxWithHolding = new Collection<busPayeeAccountTaxWithholding>();
            foreach (DataRow dr in ldtbList.Rows)
            {
                busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
                lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
                if (dr["tax_identifier_value"].ToString() == busConstant.PayeeAccountTaxIdentifierFedTax)
                {
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.LoadData(dr);
                    _iclbPayeeAccountFedTaxWithHolding.Add(lobjPayeeAccountTaxWithholding);
                }
            }

            //For MSS Layout change
            ibusPayeeAccountFedTaxWithholding = new busPayeeAccountTaxWithholding();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            ibusPayeeAccountFedTaxWithholding = iclbPayeeAccountFedTaxWithHolding.Where( o =>
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountTaxWithholding.start_date, o.icdoPayeeAccountTaxWithholding.end_date)).FirstOrDefault();
            
        }

        public busPayeeAccountTaxWithholding ibusPayeeAccountStateTaxWithholding { get; set; }//For MSS Layout change
        //Load the StateTaxWithHolding Information which are specific to the Payee Account       
        public void LoadStateTaxWithHoldingInfo()
        {
            DataTable ldtbList = Select<cdoPayeeAccountTaxWithholding>(
                   new string[1] { "payee_account_id" },
                   new object[1] { icdoPayeeAccount.payee_account_id }, null, "start_date desc");
            _iclbPayeeAccountStateTaxWithHolding = new Collection<busPayeeAccountTaxWithholding>();
            foreach (DataRow dr in ldtbList.Rows)
            {
                busPayeeAccountTaxWithholding lobjPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding();
                lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding();
                if (dr["tax_identifier_value"].ToString() == busConstant.PayeeAccountTaxIdentifierStateTax)
                {
                    lobjPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.LoadData(dr);
                    _iclbPayeeAccountStateTaxWithHolding.Add(lobjPayeeAccountTaxWithholding);
                }
            }

            //For MSS Layout change
            ibusPayeeAccountStateTaxWithholding = new busPayeeAccountTaxWithholding();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            ibusPayeeAccountStateTaxWithholding = iclbPayeeAccountStateTaxWithHolding.Where(o =>
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountTaxWithholding.start_date, o.icdoPayeeAccountTaxWithholding.end_date)).FirstOrDefault();
        }



        private Collection<busPayeeAccountPaymentItemType> _iclbMontlyBenefits;
        public Collection<busPayeeAccountPaymentItemType> iclbMonthlyBenefits
        {
            get { return _iclbMontlyBenefits; }
            set { _iclbMontlyBenefits = value; }
        }

        public Collection<busPayeeAccountPaymentItemType> iclbBenefitsOnly { get; set; }//For MSS Layout change
        // Loads the Monthly Benefit Collection
        public void LoadMontlyBenefits()
        {
            if (_iclbMontlyBenefits == null)
                _iclbMontlyBenefits = new Collection<busPayeeAccountPaymentItemType>();
            //For MSS Layout change
            if (iclbBenefitsOnly == null)
                iclbBenefitsOnly = new Collection<busPayeeAccountPaymentItemType>();
            if (_iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            foreach (busPayeeAccountPaymentItemType lobjPAPaymentItemType in _iclbPayeeAccountPaymentItemType)
            {
                if (lobjPAPaymentItemType.ibusPaymentItemType == null)
                    lobjPAPaymentItemType.LoadPaymentItemType();
                
                if ((lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.start_date <= idtNextBenefitPaymentDate) &&
                    (lobjPAPaymentItemType.icdoPayeeAccountPaymentItemType.end_date_no_null > idtNextBenefitPaymentDate))
                {
                    _iclbMontlyBenefits.Add(lobjPAPaymentItemType);
                    //For MSS Layout change
                    if (lobjPAPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1)
                    {
                        lobjPAPaymentItemType.idecItemAmount = lobjPAPaymentItemType.idecamountmultipliedbyitemdirection;
                        iclbBenefitsOnly.Add(lobjPAPaymentItemType);
                    }
                }
            }
            //For MSS Layout change- Start
            busPayeeAccount lobjPayeeAccount = (busPayeeAccount)this;
            busPayeeAccountPaymentItemType lobjPayeeAccPaymentItemType = new busPayeeAccountPaymentItemType() 
            {
                ibusPaymentItemType = new busPaymentItemType() { icdoPaymentItemType = new cdoPaymentItemType()} 
            };
            if (lobjPayeeAccPaymentItemType.icdoPayeeAccountPaymentItemType == null)
            {
                lobjPayeeAccPaymentItemType.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
            }            
            lobjPayeeAccPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_description = "Gross Benefit Amount";
            lobjPayeeAccPaymentItemType.idecItemAmount = lobjPayeeAccount.idecGrossBenfitAmount;
            iclbBenefitsOnly.Add(lobjPayeeAccPaymentItemType);
            lobjPayeeAccPaymentItemType = new busPayeeAccountPaymentItemType()
            {
                ibusPaymentItemType = new busPaymentItemType() { icdoPaymentItemType = new cdoPaymentItemType() }
            };
            if (lobjPayeeAccPaymentItemType.icdoPayeeAccountPaymentItemType == null)
            {
                lobjPayeeAccPaymentItemType.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
            }
            lobjPayeeAccPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_description = "Total Deductions";
            lobjPayeeAccPaymentItemType.idecItemAmount = idecTotalDeductionsAmount;
            iclbBenefitsOnly.Add(lobjPayeeAccPaymentItemType);
            lobjPayeeAccPaymentItemType = new busPayeeAccountPaymentItemType()
            {
                ibusPaymentItemType = new busPaymentItemType() { icdoPaymentItemType = new cdoPaymentItemType() }
            };
            if (lobjPayeeAccPaymentItemType.icdoPayeeAccountPaymentItemType == null)
            {
                lobjPayeeAccPaymentItemType.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
            }
            lobjPayeeAccPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_description = "Net Amount";
            lobjPayeeAccPaymentItemType.idecItemAmount = idecBenefitAmount;
            iclbBenefitsOnly.Add(lobjPayeeAccPaymentItemType);
            //For MSS Layout change End
        }

        private Collection<busPayeeAccountPaymentItemType> _iclbTaxabilityBenefits;
        public Collection<busPayeeAccountPaymentItemType> iclbTaxabilityBenefits
        {
            get { return _iclbTaxabilityBenefits; }
            set { _iclbTaxabilityBenefits = value; }
        }

        // Loads the Taxability Benefit Collection
        public void LoadTaxabilityBenefits()
        {
            if (_iclbTaxabilityBenefits == null)
                _iclbTaxabilityBenefits = new Collection<busPayeeAccountPaymentItemType>();
            if (_iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            foreach (busPayeeAccountPaymentItemType lobjPAPaymentItemType in _iclbPayeeAccountPaymentItemType)
            {
                if (lobjPAPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                    _iclbTaxabilityBenefits.Add(lobjPAPaymentItemType);
            }
        }

        // Used in Payee Attains 18 years Correspondence
        public string istrPayee18thBday
        {
            get
            {
                if (ibusPayee == null)
                    LoadPayee();
                return ibusPayee.icdoPerson.date_of_birth.AddYears(18).ToString(busConstant.DateFormatLongDate);
            }
        }

        public void LoadExclusionAmount()
        {
            idecExclusionAmount = 0M;
            if (iclbPayeeAccountPaymentItemType == null) LoadPayeeAccountPaymentItemType();
            if (idtNextBenefitPaymentDate == DateTime.MinValue) LoadNexBenefitPaymentDate();
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountPaymentItemType in iclbPayeeAccountPaymentItemType)
            {
                if (lobjPayeeAccountPaymentItemType.ibusPaymentItemType == null)
                    lobjPayeeAccountPaymentItemType.LoadPaymentItemType();
                if ((lobjPayeeAccountPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PaymentItemTypeNonTaxableAmount) ||
                    (lobjPayeeAccountPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.RefundPaymentItemPostTaxEEContributionAmount))
                {
                    if (busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.start_date,
                                    lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.end_date)) // PROD PIR 8673
                        idecExclusionAmount += lobjPayeeAccountPaymentItemType.icdoPayeeAccountPaymentItemType.amount;
                }
            }
        }
        private Collection<busPayeeAccountTaxWithholding> _iclbTaxWithholingHistory;

        public Collection<busPayeeAccountTaxWithholding> iclbTaxWithholingHistory
        {
            get { return _iclbTaxWithholingHistory; }
            set { _iclbTaxWithholingHistory = value; }
        }
        
        //pir 8600
        public Collection<busPayeeAccountTaxWithholding> iclbFederalTaxWithholding { get; set; }
        public Collection<busPayeeAccountTaxWithholding> iclbStateTaxWithholding { get; set; }

        public void LoadTaxWithHoldingHistory()
        {
            DataTable ldtbList = Select<cdoPayeeAccountTaxWithholding>(
                               new string[1] { "payee_account_id" },
                               new object[1] { icdoPayeeAccount.payee_account_id }, null, "start_date desc");
            _iclbTaxWithholingHistory = GetCollection<busPayeeAccountTaxWithholding>(ldtbList, "icdoPayeeAccountTaxWithholding");
            if (idtNextBenefitPaymentDate == DateTime.MinValue) LoadNexBenefitPaymentDate();
            foreach (busPayeeAccountTaxWithholding lobjTaxWithHolding in _iclbTaxWithholingHistory)
            {
                object lobjReturnValue = null;
                if (busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.start_date, lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.end_date))
                {
                    if (lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
                    {
                        lobjReturnValue = DBFunction.DBExecuteScalar("entPayeeAccountTaxWithholding.LoadTotalFederalTaxAmt",
                                    new object[3] { lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.payee_account_id, idtNextBenefitPaymentDate, lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    }
                    else if (lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
                    {
                        lobjReturnValue = Convert.ToDecimal(DBFunction.DBExecuteScalar("entPayeeAccountTaxWithholding.LoadTotalStateTaxAmt",
                                    new object[3] { lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.payee_account_id, idtNextBenefitPaymentDate, lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                    }
                }
                else if(lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.end_date != DateTime.MinValue && 
                    lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.start_date.Date != lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.end_date.Date)
                {
                    if (lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
                    {
                        lobjReturnValue = Convert.ToDecimal(DBFunction.DBExecuteScalar("entPayeeAccountTaxWithholding.LoadTotalFederalTaxAmt",
                                    new object[3] { lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.payee_account_id, lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.end_date.GetFirstDayofCurrentMonth(), lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                    }
                    else if (lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
                    {
                        lobjReturnValue = Convert.ToDecimal(DBFunction.DBExecuteScalar("entPayeeAccountTaxWithholding.LoadTotalStateTaxAmt",
                                    new object[3] { lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.payee_account_id, lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.end_date.GetFirstDayofCurrentMonth(), lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                    }
                }
                lobjTaxWithHolding.idecDeductionAmt = lobjReturnValue != DBNull.Value ? Convert.ToDecimal(lobjReturnValue) : 0.00M;
                lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.no_of_tax_allowance = lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.tax_allowance.ToString();
            }
            
            //pir 8600
            iclbFederalTaxWithholding = iclbTaxWithholingHistory.Where(o => o.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax).ToList().ToCollection();

            iclbStateTaxWithholding = iclbTaxWithholingHistory.Where(o => o.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax).ToList().ToCollection();
        }
        public void CalculateMinimumGuaranteeAmount()
        {
            decimal ldecMinimGuaranteeAmount = 0.0m;
            if (ibusBenefitCalculaton == null)
                LoadBenefitCalculation();
            if (ibusBenefitCalculaton.icdoBenefitCalculation.benefit_calculation_id > 0)
            {
                if (ibusBenefitCalculaton.ibusPersonAccount == null)
                    ibusBenefitCalculaton.LoadPersonAccount();
                decimal ldecMinimumGuarantee = 0M, ldecMemberAccountBalance = 0M;
                decimal ldecTaxableMinimumGuarantee = 0M, ldecNonTaxableMinimumGuarantee = 0M;
                decimal ldecTaxableAmount = 0M, ldecNonTaxableAmount = 0M;
                busPersonAccountRetirement lobjPersonAccountRetirement = new busPersonAccountRetirement();
                lobjPersonAccountRetirement.icdoPersonAccount = ibusBenefitCalculaton.ibusPersonAccount.icdoPersonAccount;
                lobjPersonAccountRetirement.FindPersonAccountRetirement(ibusBenefitCalculaton.ibusPersonAccount.icdoPersonAccount.person_account_id);
                lobjPersonAccountRetirement.LoadLTDSummary();
                ibusBenefitCalculaton.CalculateQDROAmount(true);

                ibusBenefitCalculaton.CalculateMinimumGuaranteedMemberAccount(ibusBenefitCalculaton.icdoBenefitCalculation.calculation_type_value,
                                                        ibusBenefitCalculaton.icdoBenefitCalculation.plan_id,
                                                        ibusBenefitCalculaton.icdoBenefitCalculation.benefit_option_value,
                                                        ibusBenefitCalculaton.icdoBenefitCalculation.person_id,
                                                        lobjPersonAccountRetirement.Member_Account_Balance_ltd,
                                                        DateTime.Today, ref ldecMinimumGuarantee,
                                                        ref ldecMemberAccountBalance, ibusBenefitCalculaton.icdoBenefitCalculation.final_monthly_benefit,
                                                        ref ldecTaxableMinimumGuarantee, ref ldecNonTaxableMinimumGuarantee,
                                                        ref ldecTaxableAmount, ref ldecNonTaxableAmount);
                ibusBenefitCalculaton.icdoBenefitCalculation.minimum_guarentee_amount = ldecMinimumGuarantee;

                ibusBenefitCalculaton.icdoBenefitCalculation.non_taxable_amount = lobjPersonAccountRetirement.Post_Tax_Total_Contribution_ltd;
            }
        }
        public decimal idecMinimumGuaranteeAmount { get; set; }
        public void LoadMinimumGuaranteeAmount()
        {
            if (icdoPayeeAccount.minimum_guarantee_amount > 0.0m)
            {
                if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                {
                    idecMinimumGuaranteeAmount = icdoPayeeAccount.minimum_guarantee_amount;
                }
                else
                {
                    decimal ldecAmountToSubractedFromMinimumGuarantee = Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoPayeeAccount.GetAmountToSubractedFromMinimumGuarantee",
                                new object[1] { icdoPayeeAccount.payee_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                    decimal ldecRecoveryAmount = Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoPayeeAccount.GetRecoveryAmount", // PROD PIR 8907
                                new object[1] { icdoPayeeAccount.payee_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                    idecMinimumGuaranteeAmount = icdoPayeeAccount.minimum_guarantee_amount + ldecRecoveryAmount - ldecAmountToSubractedFromMinimumGuarantee;
                    idecMinimumGuaranteeAmount = idecMinimumGuaranteeAmount > 0.0M ? idecMinimumGuaranteeAmount : 0.0M;
                }
            }
        }

        public decimal idecMedicarePremiumAmount { get; set; }
        public decimal idecOldMedicarePremiumAmount { get; set; }
        public decimal idecYTDMedicarePremiumAmount { get; set; }

        public decimal idecNontaxableBeginningBalnce { get; set; }
        public void LoadNontaxableBeginningBalnce()
        {
            if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                if (ibusBenefitCalculaton == null)
                    LoadBenefitCalculation();
                idecNontaxableBeginningBalnce = ibusBenefitCalculaton.icdoBenefitCalculation.non_taxable_amount;
            }
            else
            {
                idecNontaxableBeginningBalnce = icdoPayeeAccount.nontaxable_beginning_balance;
            }
        }
        
        public decimal idecStartingNontaxableAmount
        {
            get
            {
                //null check added
                if (icdoPayeeAccount == null)
                    return 0.00M;
                else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    return ibusBenefitCalculaton.icdoBenefitCalculation.non_taxable_amount;
                else
                    return ibusBenefitAccount.IsNotNull() && ibusBenefitAccount.icdoBenefitAccount.IsNotNull() ? ibusBenefitAccount.icdoBenefitAccount.starting_nontaxable_amount : 0.00M;
            }
        }
        // UCS-080 - Converting Disability to Normal Payee Account.
        public bool iblnNewCalculationNeeded { get; set; }

        public busBenefitCalculation ibusNormalBenefitCalculation { get; set; }
        public void LoadRetirementBenefitCalculation()
        {
            ibusNormalBenefitCalculation = new busBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            DataTable ldtbResult = SelectWithOperator<cdoBenefitCalculation>(
                                            new string[2] { "DISABILITY_PAYEE_ACCOUNT_ID", "ACTION_STATUS_VALUE" },
                                            new string[2] { "=", "<>" },
                                            new object[2] { icdoPayeeAccount.payee_account_id, busConstant.ApplicationActionStatusCancelled }, null);

            if (ldtbResult.Rows.Count > 0)
            {
                ibusNormalBenefitCalculation.icdoBenefitCalculation.LoadData(ldtbResult.Rows[0]);
                if (ibusNormalBenefitCalculation.icdoBenefitCalculation.is_rule_or_age_conversion == busConstant.ConvertedToNormalByRule)
                {
                    if (icdoPayeeAccount.workflow_age_conversion_flag == busConstant.Flag_Yes)
                    {
                        iblnNewCalculationNeeded = true;
                    }
                }
            }
            else
                iblnNewCalculationNeeded = true;
        }

        //Property to contain Payment History of a payee account
        public Collection<busPaymentHistoryHeader> iclbPaymentHistoryHeader { get; set; }

        /// <summary>
        /// Method to load Payment History collection for current payee account
        /// </summary>
        public void LoadPaymentHistoryHeader()
        {
            LoadPaymentHistoryHeader(icdoPayeeAccount.payee_account_id);
        }

        /// <summary>
        /// Method to load Payment History collection for a particular payee account
        /// </summary>
        public void LoadPaymentHistoryHeader(int aintPayeeAccountID)
        {
            if (iclbPaymentHistoryHeader == null)
                iclbPaymentHistoryHeader = new Collection<busPaymentHistoryHeader>();
            DataTable ldtPaymentHistory = Select<cdoPaymentHistoryHeader>(new string[1] { "payee_account_id" },
                                                                    new object[1] { aintPayeeAccountID },
                                                                    null, "PAYMENT_DATE desc");
            iclbPaymentHistoryHeader = GetCollection<busPaymentHistoryHeader>(ldtPaymentHistory, "icdoPaymentHistoryHeader");
        }

        public void LoadNexBenefitPaymentDate()
        {
            DateTime ldtPayeeNextPaymentDate = DateTime.MinValue;

            DateTime ldtLastBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate();
            idtNextBenefitPaymentDate = ldtLastBenefitPaymentDate.AddMonths(1);

            if (icdoPayeeAccount.benefit_begin_date > idtNextBenefitPaymentDate)
            {
                idtNextBenefitPaymentDate = icdoPayeeAccount.benefit_begin_date;

            }
        }
        public int GetAlreadyPaidNumberofPayments()
        {
            if (idtLastBenefitPaymentDate == DateTime.MinValue)
                LoadLastBenefitPaymentDate();
            return busGlobalFunctions.DateDiffByMonth(icdoPayeeAccount.benefit_begin_date, idtLastBenefitPaymentDate);
        }
        public void LoadLastBenefitPaymentDate()
        {
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            Collection<busPaymentHistoryHeader> lclbHistory = busGlobalFunctions.Sort<busPaymentHistoryHeader>("icdoPaymentHistoryHeader.payment_date desc", iclbPaymentHistoryHeader);
            if (lclbHistory.Count > 0)
            {
                //idtLastBenefitPaymentDate = lclbHistory[0].icdoPaymentHistoryHeader.payment_date;
                //prod pir 4433
                idtLastBenefitPaymentDate = lclbHistory.Where(o => o.icdoPaymentHistoryHeader.status_value != busConstant.HistoryHeaderStatusCancel)
                                                    .Select(o => o.icdoPaymentHistoryHeader.payment_date).FirstOrDefault();
            }
        }
        //Properties to display payment history tab

        public decimal idecpaidgrossamount { get; set; }
        public decimal idecpaidtaxableamount { get; set; }

        public decimal idecpaidnontaxableamount { get; set; }

        //prod pir 1454 : property to include rollover amounts
        public decimal idecPaidTaxableRolloverAmount { get; set; }
        public decimal idecPaidNonTaxableRolloverAmount { get; set; }

        //to load once to get collection of Case
        public Collection<busCase> iclbCase { get; set; }

        public void LoadCase()
        {
            if (iclbCase == null)
                iclbCase = new Collection<busCase>();
            DataTable ldtCase = Select<cdoCase>(new string[1] { "payee_account_id" },
                                                                    new object[1] { icdoPayeeAccount.payee_account_id },
                                                                    null, null);
            iclbCase = GetCollection<busCase>(ldtCase, "icdoCase");
        }
        //Get payment item type id by passing Item code
        public int GetPaymentItemTypeIDByItemCode(string astrItemTypeCode)
        {
            int lintPaymentItemTypeID = 0;
            if (iclbPaymentItemType == null)
                LoadPaymentItemType();
            lintPaymentItemTypeID = iclbPaymentItemType.Where(o => o.icdoPaymentItemType.item_type_code == astrItemTypeCode).Select(o => o.icdoPaymentItemType.payment_item_type_id).FirstOrDefault();
            return lintPaymentItemTypeID;
        }
        
        public Collection<busBenefitApplication> iclbBenefitApplicationByOriginatingPayeeAccount { get; set; }
        public void LoadBenefitApplicationByOriginatingPayeeAccount()
        {
            if (iclbBenefitApplicationByOriginatingPayeeAccount == null)
                iclbBenefitApplicationByOriginatingPayeeAccount = new Collection<busBenefitApplication>();

            DataTable ldtbList = Select<cdoBenefitApplication>(new string[1] { "ORIGINATING_PAYEE_ACCOUNT_ID" },
                                                                    new object[1] { icdoPayeeAccount.payee_account_id },
                                                                    null, null);
            iclbBenefitApplicationByOriginatingPayeeAccount = GetCollection<busBenefitApplication>(ldtbList, "icdoBenefitApplication");
        }

        //pir 8457
        public Collection<busRetroItemType> iclbRetroItemTypeInitialAndReac { get; set; }
        public void LoadRetroItemTypeInitialAndReactivation()
        {
            DataTable ldtbList = Select("cdoRetroItemType.LoadTypeInitialAndReactivation", new object[0] { });
            iclbRetroItemTypeInitialAndReac = GetCollection<busRetroItemType>(ldtbList, "icdoRetroItemType");
        }
    }
}
