#region Using directives

using System;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using Sagitec.CustomDataObjects;
using NeoSpin.CustomDataObjects;
using System.Collections.Generic;
using System.Reflection;
using NeoSpin.DataObjects;
using System.Text.RegularExpressions;
using System.Globalization;

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busEmployerPayrollDetail : busExtendBase
    {
        #region File Batch Related Properties
        // to be used n batch 
        //private static busSoftErrors ibusPayrollDtlSoftError;
        //private static utlRuleSection iutlPayrollDtlRuleSection;
        private string _istrOrgCodeId;
        public string istrOrgCodeId
        {
            get { return _istrOrgCodeId; }
            set { _istrOrgCodeId = value; }
        }
        private int _aintOrgId;
        public int aintOrgId
        {
            get { return _aintOrgId; }
            set { _aintOrgId = value; }
        }
        public void LoadOrgCodeID()
        {
            if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id != 0)
            {
                if (_ibusEmployerPayrollHeader.ibusOrganization == null)
                    _ibusEmployerPayrollHeader.LoadOrganization();

                _istrOrgCodeId = _ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_code;
            }

        }

        // to be used n batch 
        private string _istrPlanValue;
        public string istrPlanValue
        {
            get { return _istrPlanValue; }
            set { _istrPlanValue = value; }
        }

        // this property is used for batch
        private int _iintFileRecordType;
        public int iintFileRecordType
        {
            get { return _iintFileRecordType; }
            set { _iintFileRecordType = value; }
        }
		//PIR 17131
        public decimal idecContribMinusEnrollAmt
        { get
            {
                return icdoEmployerPayrollDetail.contribution_amount1 - icdoEmployerPayrollDetail.amount_from_enrollment1;
            }    
        }
        public decimal idecEnrollAmtMinusContrib
        {
            get
            {
                return  icdoEmployerPayrollDetail.amount_from_enrollment1 - icdoEmployerPayrollDetail.contribution_amount1;
            }
        }
        private decimal _idecOldSalaryValue;

        public decimal idecOldSalaryValue
        {
            get { return _idecOldSalaryValue; }
            set { _idecOldSalaryValue = value; }
        }


        //to get benefit type for header type
        public string istrBenefitTypeForHeaderType
        {
            get
            {
                string lstrBenefitType = string.Empty;
                lstrBenefitType = busEmployerReportHelper.GetBenefitTypeForEmployerHeaderType(_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value);
                return lstrBenefitType;
            }
        }

        private Collection<busEmployerPurchaseAllocation> _iclbEmployerPurchaseAllocation;
        public Collection<busEmployerPurchaseAllocation> iclbEmployerPurchaseAllocation
        {
            get
            {
                return _iclbEmployerPurchaseAllocation;
            }
            set
            {
                _iclbEmployerPurchaseAllocation = value;
            }
        }
        
        private Collection<busEmployerPayrollDetailError> _iclbEmployerPayrollDetailError;
        public Collection<busEmployerPayrollDetailError> iclbEmployerPayrollDetailError
        {
            get
            {
                return _iclbEmployerPayrollDetailError;
            }
            set
            {
                _iclbEmployerPayrollDetailError = value;
            }
        }
        private Collection<busEmployerPayrollDetail> _iclbEmployerPayrollDetail;
        public Collection<busEmployerPayrollDetail> iclbEmployerPayrollDetail
        {
            get
            {
                return _iclbEmployerPayrollDetail;
            }
            set
            {
                _iclbEmployerPayrollDetail = value;
            }
        }

        private Collection<cdoEmployerPayrollBonusDetail> _iclcEmployerPayrollBonusDetail;
        public Collection<cdoEmployerPayrollBonusDetail> iclcEmployerPayrollBonusDetail
        {
            get
            {
                return _iclcEmployerPayrollBonusDetail;
            }
            set
            {
                _iclcEmployerPayrollBonusDetail = value;
            }
        }

        private string _pay_period;
        public string pay_period
        {
            get
            {
                return _pay_period;
            }
            set
            {
                _pay_period = value;
            }
        }
        private string _pay_end_month;

        public string pay_end_month
        {
            get
            {
                return _pay_end_month;
            }
            set
            {
                _pay_end_month = value;
            }
        }

        //PIR - 154
        public string istrComments50Char
        {
            get
            {
                string lstrComments = String.Empty;
                if (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.comments))
                    if (_icdoEmployerPayrollDetail.comments.Length > 50)
                    {
                        lstrComments = _icdoEmployerPayrollDetail.comments.Substring(0, 50) + "...";
                    }
                    else
                    {
                        lstrComments = _icdoEmployerPayrollDetail.comments;
                    }
                return lstrComments;
            }

        }

        // to get tolerance limit from code value
        public decimal idecToleranceLimit
        {
            get
            {
                decimal ldecToleranceLimit = 0.00M;
                ldecToleranceLimit = Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(52, "TOLL", iobjPassInfo));
                return ldecToleranceLimit;
            }
        }

        // to get Seasonal and regular employee tolerance limit from code value
        public decimal idecSeasonalAndRegularEmployeeTolerance
        {
            get
            {
                decimal ldecSeasonalAndRegularEmployeeTolerance = 0.00M;
                ldecSeasonalAndRegularEmployeeTolerance = Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(52, "SRET", iobjPassInfo));
                return ldecSeasonalAndRegularEmployeeTolerance;
            }
        }

        // to get hourly tolerance limit from code value
        public decimal idecHoulryEmployeeTolerance
        {
            get
            {
                decimal ldecHoulryEmployeeTolerance = 0.00M;
                ldecHoulryEmployeeTolerance = Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(52, "HRET", iobjPassInfo));
                return ldecHoulryEmployeeTolerance;
            }
        }


        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get
            {
                return _ibusPlan;
            }

            set
            {
                _ibusPlan = value;
            }
        }
        public void LoadFileRecordType()
        {
            switch (_icdoEmployerPayrollDetail.record_type_value)
            {
                case busConstant.PayrollDetailRecordTypeRegular:
                    _iintFileRecordType = 1;
                    break;
                case busConstant.PayrollDetailRecordTypePositiveAdjustment:
                    _iintFileRecordType = 2;
                    break;
                case busConstant.PayrollDetailRecordTypeBonus:
                    _iintFileRecordType = 3;
                    break;
                case busConstant.PayrollDetailRecordTypeNegativeAdjustment:
                    _iintFileRecordType = 4;
                    break;
            }
        }

        private string _member_type;
        public string member_type
        {
            get
            {
                return _member_type;
            }
            set
            {
                _member_type = value;
            }
        }

        #endregion

        #region Properties
        //This field is used to Validate the Header if the Detail Record is Changed from the Screen
        public bool iblnValidateHeader = true;

        //this property is used in Employer Posting related cor for header type Def comp.
        private string _istrProviderName;
        public string istrProviderName
        {
            get { return _istrProviderName; }
            set { _istrProviderName = value; }
        }
        private decimal _idecMonthlyDeduction;
        public decimal idecMonthlyDeduction
        {
            get { return _idecMonthlyDeduction; }
            set { _idecMonthlyDeduction = value; }
        }

        //this property is used in Employer Posting related cor for header type INS comp.

        private decimal _idecInsAge;
        public decimal idecInsAge
        {
            get { return _idecInsAge; }
            set { _idecInsAge = value; }
        }
        private decimal _idecSpousePremium;
        public decimal idecSpousePremium
        {
            get { return _idecSpousePremium; }
            set { _idecSpousePremium = value; }
        }
        private decimal _idecSupplementPremium;
        public decimal idecSupplementPremium
        {
            get { return _idecSupplementPremium; }
            set { _idecSupplementPremium = value; }
        }
        private decimal _idecBasicPremium;
        public decimal idecBasicPremium
        {
            get { return _idecBasicPremium; }
            set { _idecBasicPremium = value; }
        }
        private decimal _idecDependentPremium;
        public decimal idecDependentPremium
        {
            get { return _idecDependentPremium; }
            set { _idecDependentPremium = value; }
        }
        private decimal _idecDentalInsPremium;
        public decimal idecDentalInsPremium
        {
            get { return _idecDentalInsPremium; }
            set { _idecDentalInsPremium = value; }
        }
        private decimal _idecLTCInsPremium;
        public decimal idecLTCInsPremium
        {
            get { return _idecLTCInsPremium; }
            set { _idecLTCInsPremium = value; }
        }
        private decimal _idecHealthInsPremium;
        public decimal idecHealthInsPremium
        {
            get { return _idecHealthInsPremium; }
            set { _idecHealthInsPremium = value; }
        }
        private decimal _idecVisionInsPremium;
        public decimal idecVisionInsPremium
        {
            get { return _idecVisionInsPremium; }
            set { _idecVisionInsPremium = value; }
        }

        private busOrganization _ibusProvider1;
        public busOrganization ibusProvider1
        {
            get { return _ibusProvider1; }
            set { _ibusProvider1 = value; }
        }
        private busOrganization _ibusProvider2;
        public busOrganization ibusProvider2
        {
            get { return _ibusProvider2; }
            set { _ibusProvider2 = value; }
        }
        private busOrganization _ibusProvider3;
        public busOrganization ibusProvider3
        {
            get { return _ibusProvider3; }
            set { _ibusProvider3 = value; }
        }
        private busOrganization _ibusProvider4;
        public busOrganization ibusProvider4
        {
            get { return _ibusProvider4; }
            set { _ibusProvider4 = value; }
        }
        private busOrganization _ibusProvider5;
        public busOrganization ibusProvider5
        {
            get { return _ibusProvider5; }
            set { _ibusProvider5 = value; }
        }
        private busOrganization _ibusProvider6;
        public busOrganization ibusProvider6
        {
            get { return _ibusProvider6; }
            set { _ibusProvider6 = value; }

        }
        private busOrganization _ibusProvider7;
        public busOrganization ibusProvider7
        {
            get { return _ibusProvider7; }
            set { _ibusProvider7 = value; }
        }

        private bool _isProvider1IsLinkedToOrg;
        public bool isProvider1IsLinkedToOrg
        {
            get { return _isProvider1IsLinkedToOrg; }
            set { _isProvider1IsLinkedToOrg = value; }
        }
        private bool _isProvider2IsLinkedToOrg;
        public bool isProvider2IsLinkedToOrg
        {
            get { return _isProvider2IsLinkedToOrg; }
            set { _isProvider2IsLinkedToOrg = value; }
        }
        private bool _isProvider3IsLinkedToOrg;
        public bool isProvider3IsLinkedToOrg
        {
            get { return _isProvider3IsLinkedToOrg; }
            set { _isProvider3IsLinkedToOrg = value; }
        }
        private bool _isProvider4IsLinkedToOrg;
        public bool isProvider4IsLinkedToOrg
        {
            get { return _isProvider4IsLinkedToOrg; }
            set { _isProvider4IsLinkedToOrg = value; }
        }
        private bool _isProvider5IsLinkedToOrg;
        public bool isProvider5IsLinkedToOrg
        {
            get { return _isProvider5IsLinkedToOrg; }
            set { _isProvider5IsLinkedToOrg = value; }
        }
        private bool _isProvider6IsLinkedToOrg;
        public bool isProvider6IsLinkedToOrg
        {
            get { return _isProvider6IsLinkedToOrg; }
            set { _isProvider6IsLinkedToOrg = value; }

        }
        private bool _isProvider7IsLinkedToOrg;
        public bool isProvider7IsLinkedToOrg
        {
            get { return _isProvider7IsLinkedToOrg; }
            set { _isProvider7IsLinkedToOrg = value; }
        }

        public DateTime idtPayPeriodDate
        {
            get
            {
                if (ibusEmployerPayrollHeader == null)
                    LoadPayrollHeader();
                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    return icdoEmployerPayrollDetail.pay_period_end_date;
                }
                else
                {
                    return icdoEmployerPayrollDetail.pay_period_date;
                }
            }
        }

        //PIR 8070
        public DateTime istrPayPeriodSortOrder
        {
            get
            {
                if (String.IsNullOrEmpty(icdoEmployerPayrollDetail.pay_period))
                    return DateTime.MinValue;
                else
                {
                    DateTime ldtPayPeriod = DateTime.Parse(icdoEmployerPayrollDetail.pay_period);
                    return ldtPayPeriod;
                }
            }
        }

        #endregion

        #region Bus Properties
        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }

        private busEmployerPayrollHeader _ibusEmployerPayrollHeader;
        public busEmployerPayrollHeader ibusEmployerPayrollHeader
        {
            get { return _ibusEmployerPayrollHeader; }
            set { _ibusEmployerPayrollHeader = value; }
        }

        private busEmployerPurchaseAllocation _ibusEmployerPurchaseAllocation;

        public busEmployerPurchaseAllocation ibusEmployerPurchaseAllocation
        {
            get { return _ibusEmployerPurchaseAllocation; }
            set { _ibusEmployerPurchaseAllocation = value; }
        }

        private busPersonAccount _ibusPersonAccount;
        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }

        public busPersonAccountRetirement ibusPersonAccountRetirement { get; set; }
        //prod pir 6649 - PIR 14291 - Commented - if there is no open employment detail, the detail should be in review - undone the 6649 logic.
        //public busPersonAccountRetirementContribution ibusRetirementContributionRegular { get; set; }
        public busPersonAccountMedicarePartDHistory ibusPersonAccountMedicare { get; set; }
        public busPersonAccountGhdv ibusPersonAccountGhdv { get; set; }
        public busPersonAccountLife ibusPersonAccountLife { get; set; }
        public busPersonAccountEAP ibusPersonAccountEAP { get; set; }
        public busPersonAccountLtc ibusPersonAccountLtc { get; set; }
        public busDBCacheData ibusDBCacheData { get; set; }

        #endregion
        #region Correspondence Properties
        public string istrMiscPayEmpty
        {
            get
            {
                return String.Empty;
            }
        }
        #endregion

        private bool iblnReloadDetailAfterSave = false;
        public DataTable idtbCachedLowIncomeCredit { get; set; } //Org to bill

        private Collection<busBenefitRefundApplication> _iclbBenefitRefundApplication;
        public Collection<busComments> iclbPayrollDetailCommentsHistory { get; set; } // ESS Backlog PIR - 13416

        public Collection<busBenefitRefundApplication> iclbBenefitRefundApplication
        {
            get { return _iclbBenefitRefundApplication; }
            set { _iclbBenefitRefundApplication = value; }
        }

        // PIR 11012
        public DateTime idtPayPeriod
        {
            get
            {
                if (String.IsNullOrEmpty(icdoEmployerPayrollDetail.pay_period))
                    return DateTime.MinValue;
                else
                {
                    DateTime ldtPayPeriod = DateTime.Parse(icdoEmployerPayrollDetail.pay_period);
                    return ldtPayPeriod;
                }
            }
        }
        //F/W upgrade PIR, dataformat of MM/yyyy not getting applied on string field
        private DateTime _idtPay_Period;

        public DateTime idtPay_Period
        {
            get
            {
                if (String.IsNullOrEmpty(icdoEmployerPayrollDetail.pay_period))
                    return DateTime.MinValue;
                else
                {
                    DateTime ldtePayPeriod = DateTime.MinValue;
                    if (DateTime.TryParse(icdoEmployerPayrollDetail.pay_period, out ldtePayPeriod))
                    {
                    }
                    return ldtePayPeriod;
                }
            }
            set
            {
                _idtPay_Period = value;
                icdoEmployerPayrollDetail.pay_period = _idtPay_Period.ToString("MM/yyyy");
            }
        }
        //PIR 21588 add new value PLAN_ID_ORIGINAL that should populate when reports are initially created 
        public string istrPlanNameOriginal
        {
            get
            {
                if (_icdoEmployerPayrollDetail.plan_id_original > 0)
                {
                    busPlan lbusPlan = new busPlan();
                    if (lbusPlan.FindPlan(_icdoEmployerPayrollDetail.plan_id_original))
                    {
                        return lbusPlan.icdoPlan.plan_name;
                    }
                }
                return string.Empty;
            }
        }
        //PIR 26369
        public decimal idecTotal_contribution_rates { get; set; }
        public decimal idecEe_pre_tax { get; set; }
        public decimal idecEe_post_tax { get; set; }
        public decimal idecEe_emp_pickup { get; set; }
        public decimal idecEe_rhic { get; set; }
        public decimal idecEr_post_tax { get; set; }
        public decimal idecEr_rhic { get; set; }
        //PIR 25920 New Plan DC 2025
        public decimal idectotal_EE_contribution_rates { get; set; }
        public decimal idectotal_ER_contribution_rates { get; set; }
        public decimal idecEE_pretax_addl { get; set; }
        public decimal idecEE_post_tax_addl { get; set; }
        public decimal idecER_pretax_match { get; set; }
        public decimal idecADEC { get; set; }
        public int iintPreviousPayrollProviderOrgID { get; set; }
        public int SrNo { get; set; }
        public void LoadBenefitRefundApplication()
        {
            iclbBenefitRefundApplication = new Collection<busBenefitRefundApplication>();
            Collection<busBenefitRefundApplication> lclbBenefitRefundApplication = new Collection<busBenefitRefundApplication>();
            DataTable ldtbResult = Select<cdoBenefitApplication>(new string[3] { "member_person_id", "plan_id", "benefit_account_type_value" },
                                        new object[3] { icdoEmployerPayrollDetail.person_id, icdoEmployerPayrollDetail.plan_id, busConstant.ApplicationBenefitTypeRefund },
                                        null, "benefit_application_id desc");
            iclbBenefitRefundApplication = GetCollection<busBenefitRefundApplication>(ldtbResult, "icdoBenefitApplication");
        }
        public Collection<cdoPlan> GetPlanByHeaderType()
        {
            DataTable ldtbPlan = new DataTable();
            String lstrBenefitTypeValue;
            if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value != null)
            {
                lstrBenefitTypeValue = busEmployerReportHelper.GetBenefitTypeForEmployerHeaderType(_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value);
                if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value != busConstant.PayrollHeaderBenefitTypePurchases)
                {
                    ldtbPlan = Select<cdoPlan>(
                        new string[1] { "benefit_type_value" }, new object[1] { lstrBenefitTypeValue }, null, null);
                }
                else
                {
                    ldtbPlan = Select<cdoPlan>(new string[0] { }, new object[0] { }, null, null);
                }
            }
            return Sagitec.DataObjects.doBase.LoadData<cdoPlan>(ldtbPlan);
        }

        //This method will be called in beforevalidate to load the Provider Objects (To Avoid queries in busXML)
        public void LoadOrgIdForProviders()
        {
            if (_icdoEmployerPayrollDetail.provider_org_code_id1 != null)
            {
                _ibusProvider1 = new busOrganization();
                _ibusProvider1.FindOrganizationByOrgCode(icdoEmployerPayrollDetail.provider_org_code_id1);
                icdoEmployerPayrollDetail.provider_id1 = busGlobalFunctions.GetOrgIdFromOrgCode(icdoEmployerPayrollDetail.provider_org_code_id1);
            }
            else
                icdoEmployerPayrollDetail.provider_id1 = 0;
            if (_icdoEmployerPayrollDetail.provider_org_code_id2 != null)
            {
                _ibusProvider2 = new busOrganization();
                _ibusProvider2.FindOrganizationByOrgCode(icdoEmployerPayrollDetail.provider_org_code_id2);
                icdoEmployerPayrollDetail.provider_id2 = busGlobalFunctions.GetOrgIdFromOrgCode(icdoEmployerPayrollDetail.provider_org_code_id2);
            }
            else
                icdoEmployerPayrollDetail.provider_id2 = 0;
            if (_icdoEmployerPayrollDetail.provider_org_code_id3 != null)
            {
                _ibusProvider3 = new busOrganization();
                _ibusProvider3.FindOrganizationByOrgCode(icdoEmployerPayrollDetail.provider_org_code_id3);
                icdoEmployerPayrollDetail.provider_id3 = busGlobalFunctions.GetOrgIdFromOrgCode(icdoEmployerPayrollDetail.provider_org_code_id3);
            }
            else
                icdoEmployerPayrollDetail.provider_id3 = 0;
            if (_icdoEmployerPayrollDetail.provider_org_code_id4 != null)
            {
                _ibusProvider4 = new busOrganization();
                _ibusProvider4.FindOrganizationByOrgCode(icdoEmployerPayrollDetail.provider_org_code_id4);
                icdoEmployerPayrollDetail.provider_id4 = busGlobalFunctions.GetOrgIdFromOrgCode(icdoEmployerPayrollDetail.provider_org_code_id4);
            }
            else
                icdoEmployerPayrollDetail.provider_id4 = 0;
            if (_icdoEmployerPayrollDetail.provider_org_code_id5 != null)
            {
                _ibusProvider5 = new busOrganization();
                _ibusProvider5.FindOrganizationByOrgCode(icdoEmployerPayrollDetail.provider_org_code_id5);
                icdoEmployerPayrollDetail.provider_id5 = busGlobalFunctions.GetOrgIdFromOrgCode(icdoEmployerPayrollDetail.provider_org_code_id5);
            }
            else
                icdoEmployerPayrollDetail.provider_id5 = 0;
            if (_icdoEmployerPayrollDetail.provider_org_code_id6 != null)
            {
                _ibusProvider6 = new busOrganization();
                _ibusProvider6.FindOrganizationByOrgCode(icdoEmployerPayrollDetail.provider_org_code_id6);
                icdoEmployerPayrollDetail.provider_id6 = busGlobalFunctions.GetOrgIdFromOrgCode(icdoEmployerPayrollDetail.provider_org_code_id6);
            }
            else
                icdoEmployerPayrollDetail.provider_id6 = 0;
            if (_icdoEmployerPayrollDetail.provider_org_code_id7 != null)
            {
                _ibusProvider7 = new busOrganization();
                _ibusProvider7.FindOrganizationByOrgCode(icdoEmployerPayrollDetail.provider_org_code_id7);
                icdoEmployerPayrollDetail.provider_id7 = busGlobalFunctions.GetOrgIdFromOrgCode(icdoEmployerPayrollDetail.provider_org_code_id7);
            }
            else
                icdoEmployerPayrollDetail.provider_id7 = 0;

            //prod pir 6077
            if (iblnOnlineCreation && !string.IsNullOrEmpty(icdoEmployerPayrollDetail.istrProviderOrgCode))
            {
                busOrganization lobjProvider = new busOrganization();
                lobjProvider.FindOrganizationByOrgCode(icdoEmployerPayrollDetail.istrProviderOrgCode);
                icdoEmployerPayrollDetail.provider_org_id = lobjProvider.icdoOrganization.org_id;
            }
        }
        public void LoadPayrollHeader()
        {
            if (_ibusEmployerPayrollHeader == null)
            {
                _ibusEmployerPayrollHeader = new busEmployerPayrollHeader();
            }
            _ibusEmployerPayrollHeader.FindEmployerPayrollHeader(_icdoEmployerPayrollDetail.employer_payroll_header_id);
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.ToString("MM/yyyy", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
            _ibusEmployerPayrollHeader.istrOrgCodeId = busGlobalFunctions.GetOrgCodeFromOrgId(_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id);            
        }

        public bool LoadPerson()
        {
            bool lblnPersonExists = false;
            //commented as when reloading person obj, all objects inside person also need be reloaded
            //if (_ibusPerson == null)
            //{
            _ibusPerson = new busPerson();
            //}
            if (icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdMedicarePartD)
            {
                if (_icdoEmployerPayrollDetail.iintPremiumForPersonId != 0)
                    lblnPersonExists = _ibusPerson.FindPerson(_icdoEmployerPayrollDetail.iintPremiumForPersonId);
                else
                {
                    busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                    lobjMedicare.FindMedicareByPersonAccountID(icdoEmployerPayrollDetail.person_account_id);
                    lobjMedicare.FindPersonAccount(icdoEmployerPayrollDetail.person_account_id);

                    lblnPersonExists = _ibusPerson.FindPerson(lobjMedicare.icdoPersonAccount.person_id);
                }
            }
            else
                lblnPersonExists = _ibusPerson.FindPerson(_icdoEmployerPayrollDetail.person_id);
            return lblnPersonExists;

        }
        public void LoadEmployerPurchaseAllocation()
        {
            DataTable ldtbList = Select<cdoEmployerPurchaseAllocation>(
                               new string[1] { "employer_payroll_detail_id" },
                               new object[1] { _icdoEmployerPayrollDetail.employer_payroll_detail_id }, null, null);
            _iclbEmployerPurchaseAllocation = GetCollection<busEmployerPurchaseAllocation>(ldtbList, "icdoEmployerPurchaseAllocation");
            foreach (busEmployerPurchaseAllocation lobjPurchaseAllocation in _iclbEmployerPurchaseAllocation)
            {
                lobjPurchaseAllocation.LoadPurchaseHeader();
            }
        }

        public void LoadEmployerPayrollDetailError()
        {
            DataTable ldtbList = Select<cdoEmployerPayrollDetailError>(
                            new string[1] { "employer_payroll_detail_id" },
                            new object[1] { _icdoEmployerPayrollDetail.employer_payroll_detail_id }, null, null);
            _iclbEmployerPayrollDetailError = GetCollection<busEmployerPayrollDetailError>(ldtbList, "icdoEmployerPayrollDetailError");
        }

        public void LoadEmployerPayrollDetailErrorLOB()
        {
            iclbEmployerPayrollDetailError = new Collection<busEmployerPayrollDetailError>();
            DataTable ldtbList = Select("entEmployerPayrollDetail.EmpPayrollDetErrorSummaryOnLOB",
                       new object[1] { _icdoEmployerPayrollDetail.employer_payroll_detail_id });
            iclbEmployerPayrollDetailError = GetCollection<busEmployerPayrollDetailError>(ldtbList, "icdoEmployerPayrollDetailError");
        }

        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            _ibusPlan.FindPlan(_icdoEmployerPayrollDetail.plan_id);
        }

        public void LoadPersonAccount()
        {
            if (ibusEmployerPayrollHeader == null)
                LoadPayrollHeader();
            if (ibusPerson == null)
                LoadPerson();

            //Employer Report Posting Batch Optimization
            if (ibusEmployerPayrollHeader.idtbAllPersonAccounts != null)
            {
                if (ibusPerson.icolPersonAccount == null)
                {
                    DataRow[] larrRow = ibusEmployerPayrollHeader.idtbAllPersonAccounts.FilterTable(busConstant.DataType.Numeric,
                                                                   "person_id",
                                                                   icdoEmployerPayrollDetail.person_id);
                    if (larrRow != null && larrRow.Length > 0)
                    {
                        ibusPerson.icolPersonAccount = GetCollection<busPersonAccount>(larrRow, "icdoPersonAccount");
                    }
                }
            }

            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount(false);

            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                _ibusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == icdoEmployerPayrollDetail.plan_id &&
                    busGlobalFunctions.CheckDateOverlapping(icdoEmployerPayrollDetail.pay_period_start_date, icdoEmployerPayrollDetail.pay_period_end_date,
                                                            i.icdoPersonAccount.start_date, i.icdoPersonAccount.end_date)).FirstOrDefault();
                if (_ibusPersonAccount == null)
                {
                    ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                }
            }
            else if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                if (ibusPerson.icolPersonAccountByPlan == null)
                    ibusPerson.LoadPersonAccountByPlan(icdoEmployerPayrollDetail.plan_id);

                if ((ibusPerson.icolPersonAccountByPlan != null) && (ibusPerson.icolPersonAccountByPlan.Count > 0))
                    ibusPersonAccount = ibusPerson.icolPersonAccountByPlan.First();
                else
                {
                    ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                }
            }
            else
            {
                _ibusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == icdoEmployerPayrollDetail.plan_id &&
                   busGlobalFunctions.CheckDateOverlapping(icdoEmployerPayrollDetail.pay_period_last_date, i.icdoPersonAccount.start_date, i.icdoPersonAccount.end_date)).FirstOrDefault();

                if (_ibusPersonAccount == null)
                {
                    ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                }
            }            
        }

        public void LoadPersonAccountRetirement()
        {
            if (_ibusEmployerPayrollHeader == null)
                LoadPayrollHeader();

            if (ibusPersonAccountRetirement == null)
                ibusPersonAccountRetirement = new busPersonAccountRetirement { icdoPersonAccountRetirement = new cdoPersonAccountRetirement() };
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();

                ibusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
            }
        }

        /// <summary>
        /// Load the Employment Detail for the Pay Period
        /// if multiple employment exists, check with the retirement rates and get the right employment
        /// for other types, just return the first one.
        /// </summary>
        /// 

        public void LoadPersonEmploymentDetail(bool albnLoadMemberType = true)
        {
            bool lblnMemberTypeLoaded = false;
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            //Employer Report Posting Batch Optimization
            if (ibusEmployerPayrollHeader.idtbAllPAEmpDetailWithChildData != null)
            {
                if (ibusPersonAccount.iclbAccountEmploymentDetail == null)
                {
                    DataRow[] larrRow = ibusEmployerPayrollHeader.idtbAllPAEmpDetailWithChildData.FilterTable(busConstant.DataType.Numeric,
                                                                   "person_account_id",
                                                                   ibusPersonAccount.icdoPersonAccount.person_account_id);
                    if (larrRow != null && larrRow.Length > 0)
                    {
                        ibusPersonAccount.iclbAccountEmploymentDetail = GetCollection<busPersonAccountEmploymentDetail>(larrRow, "icdoPersonAccountEmploymentDetail");
                    }
                }
            }

            if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                ibusPersonAccount.ibusPersonEmploymentDetail = new busPersonEmploymentDetail() { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };

            if (ibusPersonAccount.iclbAccountEmploymentDetail == null)
                ibusPersonAccount.LoadPersonAccountEmploymentDetails();
            foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in ibusPersonAccount.iclbAccountEmploymentDetail)
            {
                if (ibusEmployerPayrollHeader.idtbAllPAEmpDetailWithChildData != null)
                {
                    DataRow[] larrRow = ibusEmployerPayrollHeader.idtbAllPAEmpDetailWithChildData.FilterTable(busConstant.DataType.Numeric,
                                                                  "person_account_employment_dtl_id",
                                                                 lbusPAEmpDetail.icdoPersonAccountEmploymentDetail.person_account_employment_dtl_id);
                    if (larrRow != null && larrRow.Length > 0)
                    {
                        lbusPAEmpDetail.ibusEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                        lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.LoadData(larrRow[0]);

                        lbusPAEmpDetail.ibusEmploymentDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                        lbusPAEmpDetail.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.LoadData(larrRow[0]);
                    }
                    else
                    {
						//PIR 15741 - Object reference error
                        lbusPAEmpDetail.ibusEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                        lbusPAEmpDetail.ibusEmploymentDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                    }
                }
                else
                {
                    if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                        lbusPAEmpDetail.LoadPersonEmploymentDetail(idtPayPeriodDate, albnLoadMemberType);
                    if (lbusPAEmpDetail.ibusEmploymentDetail.ibusPersonEmployment == null)
                        lbusPAEmpDetail.ibusEmploymentDetail.LoadPersonEmployment();
                    lblnMemberTypeLoaded = true;
                }
                if (iblnIsFromValidDetail && lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id == 0)
                {
                    lbusPAEmpDetail.LoadPersonEmploymentDetail(idtPayPeriodDate, albnLoadMemberType);
                    lbusPAEmpDetail.ibusEmploymentDetail.LoadPersonEmployment();
                }
            }

            busPersonAccountEmploymentDetail lbusFinalPAEmpDetail = null;

            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                var lenuList = ibusPersonAccount.iclbAccountEmploymentDetail.Where(i => i.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id ==
                                                                             ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id &&
                                                                             busGlobalFunctions.CheckDateOverlapping(icdoEmployerPayrollDetail.pay_period_start_date, icdoEmployerPayrollDetail.pay_period_end_date,
                                                                             i.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date, i.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date));

                if (lenuList != null && lenuList.Count() > 0)
                {
                    lbusFinalPAEmpDetail = lenuList.First();
                    ibusPersonAccount.ibusPersonEmploymentDetail = lbusFinalPAEmpDetail.ibusEmploymentDetail;
                }

            }
            else
            {
                var lenuList = ibusPersonAccount.iclbAccountEmploymentDetail.Where(i => i.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id ==
                                                                             ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id &&
                                                                             (((icdoEmployerPayrollDetail.pay_period_date.Month == i.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date.Month &&
                                                                             icdoEmployerPayrollDetail.pay_period_date.Year == i.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date.Year) || //prod pir 6570
                                                                             (icdoEmployerPayrollDetail.pay_period_date.Month == i.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null.Month &&
                                                                             icdoEmployerPayrollDetail.pay_period_date.Year == i.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null.Year)) ||
                                                                             (busGlobalFunctions.CheckDateOverlapping(icdoEmployerPayrollDetail.pay_period_date, icdoEmployerPayrollDetail.pay_period_last_date, //prod pir 4055
                                                                             i.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date, i.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date))));
                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                {
                    ibusPersonAccount.iclbAccountEmploymentDetail.OrderByDescending(o => o.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null);
                    lenuList = ibusPersonAccount.iclbAccountEmploymentDetail.Where(i => i.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id ==
                                                                             ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id &&
                                                                             busGlobalFunctions.CheckDateOverlapping(icdoEmployerPayrollDetail.pay_period_date, icdoEmployerPayrollDetail.pay_period_last_date, //prod pir 4055
                                                                             i.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                                                             ((ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr &&
                                                                             i.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue) ?
                                                                             i.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date.AddMonths(1) :
                                                                             i.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date)));
                }
                if (lenuList != null && lenuList.Count() > 0)
                {
                    if (lenuList.Count() == 1)
                    {
                        lbusFinalPAEmpDetail = lenuList.First();
                        ibusPersonAccount.ibusPersonEmploymentDetail = lbusFinalPAEmpDetail.ibusEmploymentDetail;
                    }
                    else
                    {
                        //if there are more than one employment in the given period for the same org, find out the right employment by comparing the rates
                        //only for Retirement
                        if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                        {
                            foreach (busPersonAccountEmploymentDetail lbusTempPAEmpDetail in lenuList)
                            {
								//PIR 14164 and 14184							
                                lbusFinalPAEmpDetail = lbusTempPAEmpDetail;
                                ibusPersonAccount.ibusPersonEmploymentDetail = lbusFinalPAEmpDetail.ibusEmploymentDetail;
                                //prod pir 6443 : need to load member type if not already loaded
                                if (string.IsNullOrEmpty(lbusTempPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value))
                                    lbusTempPAEmpDetail.ibusEmploymentDetail.LoadMemberType(ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_last_date, lbusTempPAEmpDetail); //pir 8172 changed pay_period_end_date to payroll_paid_last_date

                                UpdateCalculatedAmountForRetirement(lbusTempPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value);
                                if (IsRetirementRateMatches())
                                {
                                    lbusFinalPAEmpDetail = lbusTempPAEmpDetail;
                                    ibusPersonAccount.ibusPersonEmploymentDetail = lbusFinalPAEmpDetail.ibusEmploymentDetail;
                                    break;
                                }
                                else //PIR 14164 and 14184
                                {
                                    lbusFinalPAEmpDetail = null;
                                    ibusPersonAccount.ibusPersonEmploymentDetail = new busPersonEmploymentDetail() { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() }; ;
                                }
                            }
                        }
                        else
                        {
                            lbusFinalPAEmpDetail = lenuList.First();
                            ibusPersonAccount.ibusPersonEmploymentDetail = lbusFinalPAEmpDetail.ibusEmploymentDetail;
                        }
                    }
                }
                //pir 6649 - PIR 14291 - undone the 6649 logic
                //else if (lenuList.Count() <= 0 && icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment &&
                //    ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                //{
                //    if (ibusPersonAccountRetirement == null)
                //        LoadPersonAccountRetirement();
                //    if (ibusPersonAccountRetirement.iclbRetirementContribution == null)
                //        ibusPersonAccountRetirement.LoadRetirementContribution();
                //    var lenumlist = ibusPersonAccountRetirement.iclbRetirementContribution.Where(i => i.icdoPersonAccountRetirementContribution.pay_period_month == icdoEmployerPayrollDetail.pay_period_date.Month
                //        && i.icdoPersonAccountRetirementContribution.pay_period_year == icdoEmployerPayrollDetail.pay_period_date.Year);
                //    foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lenumlist)
                //    {
                //        if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TransactionTypeRegularPayroll)
                //        {
                //            ibusRetirementContributionRegular = lobjRetirementContribution;
                //            break;
                //        }
                //    }
                //    if (ibusRetirementContributionRegular == null)
                //    {
                //        foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lenumlist)
                //        {
                //            if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TransactionTypePayrollAdjustment && lobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount > 0)
                //            {
                //                ibusRetirementContributionRegular = lobjRetirementContribution;
                //                break;
                //            }
                //        }
                //    }
                //    if (ibusRetirementContributionRegular == null)
                //    {
                //        foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lenumlist)
                //        {
                //            if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TransactionTypeInternalAdjustment)
                //            {
                //                ibusRetirementContributionRegular = lobjRetirementContribution;
                //                break;
                //            }
                //        }
                //    }
                //}
            }

            //Load Member Type (Posting Batch Optimization)
            if ((!lblnMemberTypeLoaded) && (lbusFinalPAEmpDetail != null))
            {
                lbusFinalPAEmpDetail.ibusPersonAccount = ibusPersonAccount;
                lbusFinalPAEmpDetail.ibusPersonAccount.ibusPersonEmploymentDetail = lbusFinalPAEmpDetail.ibusEmploymentDetail;
                lbusFinalPAEmpDetail.ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment = lbusFinalPAEmpDetail.ibusEmploymentDetail.ibusPersonEmployment;
                lbusFinalPAEmpDetail.ibusEmploymentDetail.iclbAllPlanMemberTypeCrossref = ibusEmployerPayrollHeader.iclbAllPlanMemberTypeCrossref;
                lbusFinalPAEmpDetail.ibusEmploymentDetail.iblnUseQueryToLoadData = false;
                if (ibusEmployerPayrollHeader.iclbAllOrgPlans != null)
                {
                    var lclbOrgPlanByDetail = ibusEmployerPayrollHeader.iclbAllOrgPlans.Where(i => i.icdoOrgPlan.plan_id == icdoEmployerPayrollDetail.plan_id).ToList().ToCollection();
                    lbusFinalPAEmpDetail.ibusPersonAccount.LoadOrgPlan(idtOrgPlanEffectiveDate, lclbOrgPlanByDetail);

                    if (ibusEmployerPayrollHeader.idtbAllOrgPlanMemberType != null)
                    {
                        if (lbusFinalPAEmpDetail.ibusPersonAccount.ibusOrgPlan.iclbOrgPlanMemberType == null)
                        {
                            DataRow[] larrRow = ibusEmployerPayrollHeader.idtbAllOrgPlanMemberType.FilterTable(busConstant.DataType.Numeric,
                                                                           "org_plan_id",
                                                                           lbusFinalPAEmpDetail.ibusPersonAccount.ibusOrgPlan.icdoOrgPlan.org_plan_id);
                            if (larrRow != null && larrRow.Length > 0)
                            {
                                lbusFinalPAEmpDetail.ibusPersonAccount.ibusOrgPlan.iclbOrgPlanMemberType = GetCollection<busOrgPlanMemberType>(larrRow, "icdoOrgPlanMemberType");
                            }
                        }
                    }
                }

                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    lbusFinalPAEmpDetail.ibusEmploymentDetail.LoadMemberType(ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_end_date, lbusFinalPAEmpDetail);
                }
                else
                {
                    lbusFinalPAEmpDetail.ibusEmploymentDetail.LoadMemberType(ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_last_date, lbusFinalPAEmpDetail);
                }
            }
        }

        public void LoadMemberType()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();

            if (ibusPersonAccount.ibusPersonEmploymentDetail != null)
            {
                if (ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
                {
                    //PIR-9784 --  For Record Type Bonus use pay_period_last_date_for_bonus to derive the Member Type
                    if (this.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus)
                    {
                        //PIR 15616 - Load Member Type Based on Header payroll paid date for bonus record
                        if (ibusEmployerPayrollHeader.IsNull()) LoadPayrollHeader();
                        ibusPersonAccount.ibusPersonEmploymentDetail.LoadMemberType(ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date);
                    }
                    else
                        ibusPersonAccount.ibusPersonEmploymentDetail.LoadMemberType(this.icdoEmployerPayrollDetail.pay_period_last_date); //Added for PIR 8928
                    member_type = ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value;
                }
            }
        }

        public void btnSuppress_Clicked()
        {
            _icdoEmployerPayrollDetail.suppress_warnings_flag = "Y";
            _icdoEmployerPayrollDetail.Update();

            //these methods are called in order to load objects that are needed for validation      
            SetPersonAccountIDForMedicare();//Org to bill
            SetPayPeriodDatePayPeriodEndMonthForBonus();
            LoadPersonBySSNAndLastName();
            LoadOrgIdForProviders();
            CheckProviderNotLinkedToEmployer();
            LoadObjectsForValidation(true);

            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                UpdateAmtFromEnrolmentForDefComp();
            }

            ValidateSoftErrors();
            UpdateValidateStatus();
        }
        //PIR 522,523 -UAT
        public ArrayList btnEmployerWarning_Clicked()
        {
            ArrayList larrresult = new ArrayList();
            _icdoEmployerPayrollDetail.allow_change_warnings = busConstant.Flag_Yes;
            _icdoEmployerPayrollDetail.Update();
            larrresult.Add(this);
            return larrresult;
        }
        public void btnIgnored_Clicked()
        {
            DeleteDetailErrors();
            _icdoEmployerPayrollDetail.status_value = busConstant.PayrollDetailStatusIgnored;
            _icdoEmployerPayrollDetail.Update();

            _icdoEmployerPayrollDetail.Select();
            _ibusEmployerPayrollHeader.LoadEmployerPayrollDetail();

            //PROD PIR 5036
            if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value != busConstant.PayrollHeaderBenefitTypeInsr ||
                (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr &&
                _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value != busConstant.PayrollHeaderReportTypeAdjustment))
            {
                //Validation only selected details for which we are ignoring 
                ValidateRelatedDetailRecords();
                //_ibusEmployerPayrollHeader.iblnValidateDetail = false;
            }
            _ibusEmployerPayrollHeader.ValidateSoftErrors();
            _ibusEmployerPayrollHeader.UpdateValidateStatus();
        }

        public void DeleteDetailErrors()
        {
            //DeleteErrorForDetail
            DBFunction.DBNonQuery("cdoEmployerPayrollDetail.DeleteErrorForDetail", new object[1] { _icdoEmployerPayrollDetail.employer_payroll_detail_id },
                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            /******************************************************************************
            * methods those are called here must be called in btnSuppress_Clicked method 
            * ****************************************************************************/
            SetPersonAccountIDForMedicare();//Org to bill
            SetPayPeriodDatePayPeriodEndMonthForBonus();
            LoadPersonBySSNAndLastName();
            LoadOrgIdForProviders();
            CheckProviderNotLinkedToEmployer();
            LoadObjectsForValidation(true);
            SetPurchasePayPeriodDate();
            iblnReloadDetailAfterSave = true;
            base.BeforeValidate(aenmPageMode);
        }

        /// <summary>
        /// method to set the pay period date for header type purchase
        /// </summary>
        private void SetPurchasePayPeriodDate()
        {
            if (ibusEmployerPayrollHeader == null)
                LoadPayrollHeader();
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                icdoEmployerPayrollDetail.purchase_pay_period_date = DateTime.Today;
        }

        //Org to bill
        private void SetPersonAccountIDForMedicare()
        {
            if (icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdMedicarePartD)
            {
                busPerson lobjPerson = new busPerson();
                lobjPerson.FindPerson(icdoEmployerPayrollDetail.iintPremiumForPersonId);

                lobjPerson.LoadPersonAccountByPlan(icdoEmployerPayrollDetail.plan_id);
                if (lobjPerson.icolPersonAccountByPlan.Count > 0)
                    icdoEmployerPayrollDetail.person_account_id = lobjPerson.icolPersonAccountByPlan[0].icdoPersonAccount.person_account_id;
            }
        }

        public void LoadPersonBySSNAndLastName()
        {
            if ((!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.ssn)) && (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.last_name)))
            {
                DataTable ldtbGetPerson = Select<cdoPerson>(
                                                new string[1] { "SSN" },
                                                new object[1] { icdoEmployerPayrollDetail.ssn }, null, null);
                if (ldtbGetPerson.Rows.Count > 0)
                {
                    //Org to bill
					if (icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory();
                        lobjMedicare.icdoPersonAccount = new cdoPersonAccount();
                        lobjMedicare.icdoPersonAccount.person_id = Convert.ToInt32(ldtbGetPerson.Rows[0]["person_id"]);
                        lobjMedicare.LoadPerson();
                        lobjMedicare.FindByMemberPersonID(Convert.ToInt32(ldtbGetPerson.Rows[0]["person_id"]));
                        lobjMedicare.LoadMedicarePartDMembers();

                        foreach (busPersonAccountMedicarePartDHistory lobjMedicareHistory in lobjMedicare.iclbPersonAccountMedicarePartDMembers)
                        {
                            busPerson lobjPerson = new busPerson();
                            lobjPerson.FindPerson(lobjMedicareHistory.icdoPersonAccountMedicarePartDHistory.person_id);

                            DataTable ldtbGetPersonMedicare = Select<cdoPerson>(
                                                                    new string[1] { "PERSON_ID" },
                                                                    new object[1] { lobjMedicareHistory.icdoPersonAccountMedicarePartDHistory.person_id }, null, null);

                            ldtbGetPersonMedicare = ldtbGetPersonMedicare.AsEnumerable()
                                .Where(o => (o.Field<string>("last_name").Trim().ToLower().ReplaceWith("[^a-zA-Z0-9]", "")) ==
                                (icdoEmployerPayrollDetail.last_name.Trim().ToLower().ReplaceWith("[^a-zA-Z0-9]", "")))
                                .AsDataTable();

                            if (!string.IsNullOrEmpty(_icdoEmployerPayrollDetail.first_name) &&
                                ldtbGetPersonMedicare.Rows.Count > 0 && ldtbGetPersonMedicare.Rows[0]["first_name"] != DBNull.Value)
                            {
                                ldtbGetPersonMedicare = ldtbGetPersonMedicare.AsEnumerable().Where(o => (o.Field<string>("first_name").ToLower()
                                        .ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 1)) ==
                                        (icdoEmployerPayrollDetail.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 1)))
                                        .AsDataTable();
                            }
                            if (ldtbGetPersonMedicare.Rows.Count > 0)
                            {
                                _icdoEmployerPayrollDetail.person_id = Convert.ToInt32(ldtbGetPerson.Rows[0]["person_id"]);
                                _icdoEmployerPayrollDetail.first_name = Convert.ToString(ldtbGetPersonMedicare.Rows[0]["first_name"]);
                                _icdoEmployerPayrollDetail.last_name = Convert.ToString(ldtbGetPersonMedicare.Rows[0]["last_name"]);
                                icdoEmployerPayrollDetail.istrPeopleSoftID = ldtbGetPersonMedicare.Rows[0]["peoplesoft_id"] == DBNull.Value ? string.Empty : Convert.ToString(ldtbGetPerson.Rows[0]["peoplesoft_id"]);
                                break;
                            }
                            else
                            {
                                _icdoEmployerPayrollDetail.person_id = 0;
                                icdoEmployerPayrollDetail.istrPeopleSoftID = string.Empty;
                            }

                        }
                    }
                    else
                    {
                        //uat pir 2488 : first five chars to be used for validation
                        ldtbGetPerson = ldtbGetPerson.AsEnumerable()
                            .Where(o => (o.Field<string>("last_name").Trim().ToLower().ReplaceWith("[^a-zA-Z0-9]", "")) ==
                                (icdoEmployerPayrollDetail.last_name.Trim().ToLower().ReplaceWith("[^a-zA-Z0-9]", "")))
                                .AsDataTable();
                        //PIR 14166 - Maik mail Let’s also add the 1st character of the First Name to the Save logic so it matches the validation from the other processes
                        if (!string.IsNullOrEmpty(_icdoEmployerPayrollDetail.first_name) &&
                            ldtbGetPerson.Rows.Count > 0 && ldtbGetPerson.Rows[0]["first_name"] != DBNull.Value)
                        {
                            ldtbGetPerson = ldtbGetPerson.AsEnumerable().Where(o => (o.Field<string>("first_name").ToLower()
                                    .ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 1)) ==
                                    (icdoEmployerPayrollDetail.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 1)))
                                    .AsDataTable();
                        }
                        if (ldtbGetPerson.Rows.Count > 0)
                        {
                            _icdoEmployerPayrollDetail.person_id = Convert.ToInt32(ldtbGetPerson.Rows[0]["person_id"]);
                            _icdoEmployerPayrollDetail.first_name = Convert.ToString(ldtbGetPerson.Rows[0]["first_name"]);
                            _icdoEmployerPayrollDetail.last_name = Convert.ToString(ldtbGetPerson.Rows[0]["last_name"]);
                            icdoEmployerPayrollDetail.istrPeopleSoftID = ldtbGetPerson.Rows[0]["peoplesoft_id"] == DBNull.Value ? string.Empty : Convert.ToString(ldtbGetPerson.Rows[0]["peoplesoft_id"]);
                        }
                        else
                        {
                            _icdoEmployerPayrollDetail.person_id = 0;
                            icdoEmployerPayrollDetail.istrPeopleSoftID = string.Empty;
                        }
                    }
                }
                else
                {
                    _icdoEmployerPayrollDetail.person_id = 0;
                    icdoEmployerPayrollDetail.istrPeopleSoftID = string.Empty;
                }
            }
        }

        public void LoadObjectsForValidation(bool ablnMustLoadObject = false)
        {
            //Reloading Plan Object for Validation
            if (ibusPlan == null || ablnMustLoadObject)
            {
                LoadPlan();
            }
            else if (ibusPlan.icdoPlan.plan_id != icdoEmployerPayrollDetail.plan_id)
            {
                LoadPlan();
            }

            //Reloading Header Org Object for Validation
            if (ibusEmployerPayrollHeader.ibusOrganization == null || ablnMustLoadObject)
            {
                ibusEmployerPayrollHeader.LoadOrganization();
            }

            //Reloading Person Object for Validation
            if (ibusPerson == null || ablnMustLoadObject)
            {
                LoadPerson();
            }
            else if (ibusPerson.icdoPerson.person_id != icdoEmployerPayrollDetail.person_id)
            {
                LoadPerson();
            }

            //Reloading Person Account for Validation
            if (ibusPersonAccount == null || ablnMustLoadObject)
            {
                LoadPersonAccount();
            }
            else if (ibusPersonAccount.icdoPersonAccount.plan_id != icdoEmployerPayrollDetail.plan_id)
            {
                LoadPersonAccount();
            }

            if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
            {
                LoadPersonEmploymentDetail();
            }

            if (ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment == null)
            {
                ibusPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();
            }

            if (ibusPersonAccount.ibusOrgPlan == null)
            {
                //systest 2381
                //ibusPersonAccount.LoadOrgPlan();
                LoadOrgPlan();
            }
        }

        public void LoadOrgPlan()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            ibusPersonAccount.LoadOrgPlan(idtOrgPlanEffectiveDate);
        }

        private DateTime idtOrgPlanEffectiveDate
        {
            get
            {
                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    return icdoEmployerPayrollDetail.pay_period_end_date;
                }
                else if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                {
                    return icdoEmployerPayrollDetail.purchase_pay_period_date;
                }
                else
                {
                    return icdoEmployerPayrollDetail.pay_period_last_date;
                }
            }
        }


        private void SetPayPeriodDatePayPeriodEndMonthForBonus()
        {
            if ((String.IsNullOrEmpty(_pay_period)))
            {
                _pay_period = String.Empty;
            }
            else
            {
                icdoEmployerPayrollDetail.pay_period_date = Convert.ToDateTime(_pay_period.ToString());

            }
            if ((String.IsNullOrEmpty(_pay_end_month)))
            {
                _pay_end_month = String.Empty;
            }
            else
            {
                icdoEmployerPayrollDetail.pay_period_end_month_for_bonus = Convert.ToDateTime(_pay_end_month.ToString());
            }
        }

        public override void BeforePersistChanges()
        {
            if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                if (_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment && _icdoEmployerPayrollDetail.eligible_wages < 0)
                {
                    _icdoEmployerPayrollDetail.eligible_wages = -1 * _icdoEmployerPayrollDetail.eligible_wages;
                }
                //Process Bonus Details
                //This also calculates the Member Interest and Employer Interest for each month bonus
                if (_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus || _icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)  //PIR-17777
                {
                    _iclcEmployerPayrollBonusDetail = busEmployerReportHelper.CalculateBonus(this);
                }
				//PIR 15616 - Bonus interest should be calculated same as normal adjustment
                //Process Interest Calculation for all the entries except Bonus Record Type
                //if (_icdoEmployerPayrollDetail.record_type_value != busConstant.PayrollDetailRecordTypeBonus)
                //{
                //PIR 13996-this method called to set reported amounts as these amounts used to calculate interest in ProcessInterestCalculation()
                UpdateCalculatedFields();
                ProcessInterestCalculation();
                //}
                //else
                //{
                //    _icdoEmployerPayrollDetail.member_interest_calculated = 0;
                //    _icdoEmployerPayrollDetail.employer_interest_calculated = 0;
                //    _icdoEmployerPayrollDetail.employer_rhic_interest_calculated = 0;
                //    foreach (cdoEmployerPayrollBonusDetail lcdoEmployerPayrollBonusDetail in _iclcEmployerPayrollBonusDetail)
                //    {
                //        _icdoEmployerPayrollDetail.member_interest_calculated += lcdoEmployerPayrollBonusDetail.member_interest;
                //        _icdoEmployerPayrollDetail.employer_interest_calculated += lcdoEmployerPayrollBonusDetail.employer_interest;
                //        _icdoEmployerPayrollDetail.employer_rhic_interest_calculated += lcdoEmployerPayrollBonusDetail.employer_rhic_interest;
                //    }
                //}

                //Get the Old Salary Only for Retirement
                LoadOldSalaryAmount();
            }

            //prod pir 6077
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr &&
                ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.report_type_value == busConstant.PayrollHeaderReportTypeAdjustment &&
                iblnOnlineCreation)
            {
                icdoEmployerPayrollDetail.status_value = busConstant.PayrollDetailStatusValid;
            }

            UpdateCalculatedFields();

            //Org to bill
            if (icdoEmployerPayrollDetail.plan_id ==  busConstant.PlanIdMedicarePartD)
            {
                busPerson lobjPerson = new busPerson();
                lobjPerson.FindPerson(icdoEmployerPayrollDetail.iintPremiumForPersonId);

                lobjPerson.LoadPersonAccountByPlan(icdoEmployerPayrollDetail.plan_id);
                if (lobjPerson.icolPersonAccountByPlan.Count > 0)
                    icdoEmployerPayrollDetail.person_account_id = lobjPerson.icolPersonAccountByPlan[0].icdoPersonAccount.person_account_id;

                //ldecLowIncomeCredit = icdoIbsDetail.lis_amount;
                busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                lobjMedicare.FindMedicareByPersonAccountID(icdoEmployerPayrollDetail.person_account_id);
                lobjMedicare.FindPersonAccount(icdoEmployerPayrollDetail.person_account_id);

                lobjMedicare.LoadPlanEffectiveDate();

                //Low Income Credit Amount should be populated from Ref table. 
                Decimal ldecLowIncomeCreditAmount = 0;
                DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == icdoEmployerPayrollDetail.idecLow_Income_Credit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                foreach (DataRow dr in lenumList)
                {
                    if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjMedicare.idtPlanEffectiveDate.Date)
                    {
                        ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                        break;
                    }
                }

                if (icdoEmployerPayrollDetail.premium_amount < 0 )
                    icdoEmployerPayrollDetail.lis_amount = Math.Abs(ldecLowIncomeCreditAmount) * -1;
                else
                    icdoEmployerPayrollDetail.lis_amount = ldecLowIncomeCreditAmount;
            }
            //PIR 26109 - Column with checkbox; read-only in LOB
            if (ibusEmployerPayrollHeader.IsNull())  LoadPayrollHeader();
            if ((iobjPassInfo.istrFormName == "wfmESSEmployerPayrollDetailMaintenance" || iobjPassInfo.istrFormName == "wfmEmployerPayrollDetailMaintenance") && icdoEmployerPayrollDetail.comments.IsNotNullOrEmpty())
            {
                ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.detail_comments = iobjPassInfo.istrFormName == "wfmESSEmployerPayrollDetailMaintenance" ? "Y" : "N";
                ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.Update();
            }
            

            base.BeforePersistChanges();
        }

        public override int PersistChanges()
        {
            if (iblnOnlineCreation)
            {
                if (!string.IsNullOrEmpty(icdoEmployerPayrollDetail.rate_structure_code))
                    icdoEmployerPayrollDetail.rate_structure_code = icdoEmployerPayrollDetail.rate_structure_code.PadLeft(4, '0');
                if (!string.IsNullOrEmpty(icdoEmployerPayrollDetail.coverage_code))
                    icdoEmployerPayrollDetail.coverage_code = icdoEmployerPayrollDetail.coverage_code.PadLeft(4, '0');
            }
            int lintResult = base.PersistChanges();

            //Insert Bonus Detail Records if the record type is bonus. Delete the Old Entries
            if ((_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus || _icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus) &&
                (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt))
            {
                PersistBonusDetails();
            }

            if ((ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                && (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value == busConstant.PayrollHeaderBalancingStatusNoRemittance))
            {
                AutomaticAllocation();
            }

            // PROD PIR ID 933
            if (icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupHealth &&
                icdoEmployerPayrollDetail.record_type_value == busConstant.RecordTypeNegativeAdjustment && iclbPADependent != null)
                InsertPADependentLink();
            // ESS Backlog PIR - 13416
            if (!String.IsNullOrEmpty(icdoEmployerPayrollDetail.comments))
            {
                busComments lbusComments = new busComments{icdoComments = new cdoComments()};
                lbusComments.icdoComments.comments = icdoEmployerPayrollDetail.comments;
                lbusComments.icdoComments.employer_payroll_header_id = icdoEmployerPayrollDetail.employer_payroll_header_id;
                lbusComments.icdoComments.employer_payroll_detail_id = icdoEmployerPayrollDetail.employer_payroll_detail_id;
                lbusComments.icdoComments.created_by = iobjPassInfo.istrUserID;
                lbusComments.icdoComments.modified_by = iobjPassInfo.istrUserID;
                lbusComments.icdoComments.created_date = DateTime.Now;
                lbusComments.icdoComments.modified_date = DateTime.Now;
                lbusComments.icdoComments.Insert();
                LoadEmployerPayrollDetailComments();
                icdoEmployerPayrollDetail.comments = String.Empty;
            }
            return lintResult;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            LoadPayrollHeader();
            LoadPerson();
            LoadPlan();
            _ibusEmployerPayrollHeader.LoadOrganization();
            LoadOrgCodeID();
            LoadEmployerPayrollDetailError();
            LoadEmployerPurchaseAllocation();

            // PROD PIR ID 933
            if (icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupHealth &&
                icdoEmployerPayrollDetail.record_type_value == busConstant.RecordTypeNegativeAdjustment)
            {
                LoadPersonAccountDependentBillingLink();
                LoadDependents();
            }
            if (iblnIsFromESS) LoadNegativeComponents();
            LoadEmployerPayrollDetailErrorLOB();
        }

        public override bool ValidateSoftErrors()
        {
            //Since we have all rules in soft error
            //if the user saves without entering anything field  we get system error because detail id is 0.

            if (_icdoEmployerPayrollDetail.employer_payroll_detail_id != 0 && !iblnOnlineCreation) //prod pir 6077
            {
                //
                if (iobjPassInfo == null)
                {
                    throw new Exception("Unable to execute busBase.ValidateSoftErrors, iobjPassInfo is set to null");
                }
                
                //if (ibusPayrollDtlSoftError == null)
                //{
                //    LoadErrors();
                //    ibusPayrollDtlSoftError = ibusSoftErrors;

                //    //FieldInfo lfld = typeof(busBase).GetField("_iutlSoftErrors", BindingFlags.NonPublic | BindingFlags.Instance);
                //    //iutlPayrollDtlRuleSection = (utlRuleSection)lfld.GetValue(this);
                //}
                //else
                //    ibusSoftErrors = ibusPayrollDtlSoftError;


                //ibusSoftErrors.istrParentKeyValue = icdoEmployerPayrollDetail.employer_payroll_detail_id.ToString();
                //ibusSoftErrors.iobjMainCDO = icdoEmployerPayrollDetail;
                //ibusSoftErrors.iclbError.Clear();
                //ibusSoftErrors.iclbEmployerError.Clear();
                //ibusSoftErrors.ibusMainObject = this;

                //FieldInfo lfld1 = typeof(busBase).GetField("_iutlSoftErrors", BindingFlags.NonPublic | BindingFlags.Instance);
                //lfld1.SetValue(this, iutlPayrollDtlRuleSection);

                base.ValidateSoftErrors();

                base.UpdateValidateStatus();
                //When the user makes changes in Detail Record on screen, we must reload the detail collection in header object for the header validation.
                if (iblnReloadDetailAfterSave)
                {
                    //to reload detail collection if changes made to detail records
                    _ibusEmployerPayrollHeader.LoadEmployerPayrollDetail();
                }
                if (iblnValidateHeader)
                {
                    _ibusEmployerPayrollHeader.ValidateSoftErrors();
                    _ibusEmployerPayrollHeader.UpdateValidateStatus();
                }
            }
            return true;
        }


        /// <summary>
        /// Deleting the Old Entries and Inserting New Calculated Bonus Details
        /// </summary>
        public void PersistBonusDetails()
        {
            if (_iclcEmployerPayrollBonusDetail == null)
                _iclcEmployerPayrollBonusDetail = new Collection<cdoEmployerPayrollBonusDetail>();
            //Delete the Old Entries from the Table
            DBFunction.DBNonQuery("cdoEmployerPayrollBonusDetail.DeleteBonusEntries",
                                  new object[1] { _icdoEmployerPayrollDetail.employer_payroll_detail_id },
                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            foreach (cdoEmployerPayrollBonusDetail lcdoEmployerPayrollBonusDetail in _iclcEmployerPayrollBonusDetail)
            {
                lcdoEmployerPayrollBonusDetail.employer_payroll_detail_id = _icdoEmployerPayrollDetail.employer_payroll_detail_id;
                lcdoEmployerPayrollBonusDetail.Insert();
            }
        }

        /// <summary>
        /// Calculate Interest only If interest waiver flag is not checked and the benifit type is retirement
        /// </summary>
        public void ProcessInterestCalculation()
        {
            _icdoEmployerPayrollDetail.recalculate_interest_flag = busConstant.Flag_No;
            //Loading the Payroll Header
            if (_ibusEmployerPayrollHeader == null)
                LoadPayrollHeader();

            //Resetting the Amount
            _icdoEmployerPayrollDetail.member_interest_calculated = 0;
            _icdoEmployerPayrollDetail.employer_interest_calculated = 0;
            _icdoEmployerPayrollDetail.employer_rhic_interest_calculated = 0;

            //PIR 26010 – If payroll reports have a Submitted Date after the 17th of the month, automatically calculate the interest
            //PIR 26457 - If Bonus/Retro need to look at pay_period_end_month_for_bonus date instead
            DateTime ldtmPayrollHeaderPayPeriodDate = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period.IsNullOrEmpty() ? _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date : Convert.ToDateTime(_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period);
            if (_ibusEmployerPayrollHeader.IsNotNull() &&
                (((_icdoEmployerPayrollDetail.record_type_value != busConstant.PayrollDetailRecordTypeNegativeAdjustment && _icdoEmployerPayrollDetail.record_type_value != busConstant.PayrollDetailRecordTypePositiveAdjustment) &&
                    (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date.Date > ldtmPayrollHeaderPayPeriodDate.AddMonths(1).AddDays(16).Date)) ||
                ((_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment || _icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypePositiveAdjustment) &&
                  _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date.Date > icdoEmployerPayrollDetail.pay_period_date.AddMonths(1).AddDays(16).Date)))
            {
                //PIR 400 - Dont Allow Interest Posting for DC Plans
                if (ibusPlan == null)
                    LoadPlan();
                //PIR 17380, though interest needs to be calculated for DC plan details, 
                //no interest component should be posted to contribution table, so instead of 
                //updating allow_interest_posting flag to 1 for DC plan, added this OR condition for DC
                if ((ibusPlan.icdoPlan.allow_interest_posting == 1) || (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC || ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                    ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC2025)) //PIR 25920
                {
                    if ((_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                       && (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.interest_waiver_flag != busConstant.Flag_Yes))
                    {
                        if (ibusPersonAccount == null) //PIR-17512 interest calculation
                            LoadPersonAccount();
                        if (ibusPersonAccount.icdoPersonAccount.person_account_id != 0)
                        {
                            //PIR 26457 uncommented after discussion with Maik

                            //Get the Last Interest Posted Interest Date
                            //PIR 25142   BPM - Interest Not calculating when clicking Save or Validate at header level --always calculate the posting date for each person
                            ibusEmployerPayrollHeader.idtLastInterestPostingDate = DateTime.MinValue;
                            //if (ibusEmployerPayrollHeader.idtLastInterestPostingDate == DateTime.MinValue)
                            {
                                //ibusEmployerPayrollHeader.LoadLastInterestBatchDate();
                                DataTable ldtbEffectiveAndTransDates = busNeoSpinBase.Select("cdoPersonAccountRetirementContribution.GetEffectiveAndTransactionDates",
                                              new object[1] { ibusPersonAccount.icdoPersonAccount.person_account_id });
                                if (ldtbEffectiveAndTransDates.Rows.Count > 0)
                                {
                                    ibusEmployerPayrollHeader.idtLastInterestPostingDate = Convert.ToDateTime(ldtbEffectiveAndTransDates.Rows[0]["effective_date"]);
                                }
                            }
                            DateTime ldtLastPostedInterestDate = ibusEmployerPayrollHeader.idtLastInterestPostingDate;
                            //There is a chance for Min Value when we run the system for the First Time.
                            //Such cases, get the next run date for Interest Posting batch Date and Substract two months
                            if (ldtLastPostedInterestDate == DateTime.MinValue)
                            {
                                busBatchSchedule lbusBatchSchedule = new busBatchSchedule();
                                lbusBatchSchedule.FindBatchSchedule(busConstant.PostingInterestBatchStep);
                                ldtLastPostedInterestDate = lbusBatchSchedule.icdoBatchSchedule.next_run_date;
                                if (ldtLastPostedInterestDate != DateTime.MinValue)
                                {
                                    ldtLastPostedInterestDate = ldtLastPostedInterestDate.AddMonths(-2);
                                    ldtLastPostedInterestDate = new DateTime(ldtLastPostedInterestDate.Year, ldtLastPostedInterestDate.Month, 1).AddMonths(1).AddDays(-1);
                                }
                            }
                            //PIR 26457 commented after discussion with Maik
                            ////PIR 26010 - Commented above ldtLastPostedInterestDate logic and passing submitted date for interest calculation
                            DateTime ldtSubmittedDate = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date;


                            if (ldtSubmittedDate != DateTime.MinValue)
                            {
                                //code commented as per PIR # :1952-may need to remove the comments when CCR comes in
                                ///PIR 26688 clarification from client
                                ///Member interest calculated on sum of each details EE Contribution Reported + EE Pre Tax Reported + EE ER Pickup Reported + EE Pre Tax Addl Reported + EE Post Tax Addl Reported
                                ///Employer interest calculated on sum of each details ER Contribution Reported + ER Pre Tax Match Reported +ADEC Reported
                                ///No change to RHIC

                                decimal ldecMemberContribution = _icdoEmployerPayrollDetail.ee_contribution_reported +
                                                                 _icdoEmployerPayrollDetail.ee_pre_tax_reported +
                                                                 _icdoEmployerPayrollDetail.ee_employer_pickup_reported +
                                                                 _icdoEmployerPayrollDetail.ee_pretax_addl_reported +
                                                                 _icdoEmployerPayrollDetail.ee_post_tax_addl_reported;

                                decimal ldecEmployerContribution = _icdoEmployerPayrollDetail.er_contribution_reported + _icdoEmployerPayrollDetail.er_pretax_match_reported + _icdoEmployerPayrollDetail.adec_reported;

                                decimal ldecEmployerRHICContribution = icdoEmployerPayrollDetail.rhic_ee_contribution_reported + icdoEmployerPayrollDetail.rhic_er_contribution_reported;

                                DateTime ldtBenefitBeginDate = GetBenefitBeginDate();

                                /*PIR - 15616 Interest should be calculated for bonus from header's payroll paid date to last interest posted date
                                 *for all other record types detail's pay period date to last interest posted date */
                                //PIR 26457 if bonus type, we need to use pay_period_end_month_for_bonus since we are now looking at this date to check whether interest needs to be calculated or not
                                //_icdoEmployerPayrollDetail.member_interest_calculated = busEmployerReportHelper.
                                //    CalculateMemberInterest(ldtLastPostedInterestDate, ((_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) || (_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)) ?  // PIR 17777
                                //    Convert.ToDateTime(_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period) : icdoEmployerPayrollDetail.pay_period_date,
                                //                            ldecMemberContribution, iobjPassInfo, ldtBenefitBeginDate, icdoEmployerPayrollDetail.plan_id);

                                //_icdoEmployerPayrollDetail.employer_interest_calculated = busEmployerReportHelper.
                                //    CalculateEmployerInterest(ldtLastPostedInterestDate,
                                //    ((_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) || (_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)) ? // PIR 17777
                                //    Convert.ToDateTime(_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period) :
                                //                              icdoEmployerPayrollDetail.pay_period_date, ldecMemberContribution,
                                //                              ldecEmployerContribution, iobjPassInfo, ldtBenefitBeginDate, icdoEmployerPayrollDetail.plan_id);

                                //_icdoEmployerPayrollDetail.employer_rhic_interest_calculated = busEmployerReportHelper.
                                //    CalculateEmployerRHICInterest(ldtLastPostedInterestDate, ((_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) || (_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)) ?   // PIR 17777
                                //    Convert.ToDateTime(_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period) : icdoEmployerPayrollDetail.pay_period_date,
                                //                            ldecEmployerRHICContribution, iobjPassInfo, ldtBenefitBeginDate, icdoEmployerPayrollDetail.plan_id);


                                //PIR 26688 New logic to calculate interest

                                //18th of month following Detail/Header Pay Period month 
                                DateTime ldtmPayPeriodDateForInterestCalculation = ((_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) || (_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)) ?
                                                                                     ldtmPayrollHeaderPayPeriodDate : icdoEmployerPayrollDetail.pay_period_date;
                                ldtmPayPeriodDateForInterestCalculation = ldtmPayPeriodDateForInterestCalculation.AddMonths(1).AddDays(16);

                                _icdoEmployerPayrollDetail.member_interest_calculated = Convert.ToDecimal(DBFunction.DBExecuteScalar("entEmployerPayrollDetail.CalculateInterestForDetail", new object[6] { icdoEmployerPayrollDetail.pay_period_date, ldtSubmittedDate, ldtmPayPeriodDateForInterestCalculation, ldecMemberContribution, icdoEmployerPayrollDetail.plan_id, "FALSE" }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                                _icdoEmployerPayrollDetail.employer_interest_calculated = Convert.ToDecimal(DBFunction.DBExecuteScalar("entEmployerPayrollDetail.CalculateInterestForDetail", new object[6] { icdoEmployerPayrollDetail.pay_period_date, ldtSubmittedDate, ldtmPayPeriodDateForInterestCalculation, ldecEmployerContribution, icdoEmployerPayrollDetail.plan_id, "FALSE" }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                                _icdoEmployerPayrollDetail.employer_rhic_interest_calculated = Convert.ToDecimal(DBFunction.DBExecuteScalar("entEmployerPayrollDetail.CalculateInterestForDetail", new object[6] { icdoEmployerPayrollDetail.pay_period_date, ldtSubmittedDate, ldtmPayPeriodDateForInterestCalculation, ldecEmployerRHICContribution, icdoEmployerPayrollDetail.plan_id, "TRUE" }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));

                            }
                        }
                    }
                }
            }
        }
        //If Active Payee account exists ,get the benefit begin date for the person who is receiving the contribution
		//PIR 9538
        public DateTime GetBenefitBeginDate()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.iclbPayeeAccount == null)
                ibusPerson.LoadPayeeAccount();
            foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbPayeeAccount)
            {
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsNull())
                    lobjPayeeAccount.LoadActivePayeeStatus();
                if ((!lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled()) // PIR 10128 -- Exclude Cancelled Payee Accounts
                    && (!lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusSuspended()))    // PIR 11384 -- Exclude Suspended Payee Accounts
                {
                    if (lobjPayeeAccount.ibusApplication == null)
                        lobjPayeeAccount.LoadApplication();
                    if (lobjPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts == null)
                        lobjPayeeAccount.ibusApplication.LoadBenefitApplicationPersonAccount();
                    foreach (busBenefitApplicationPersonAccount lobjAppPersonAccount in lobjPayeeAccount.ibusApplication.iclbBenefitApplicationPersonAccounts)
                    {
                        if (lobjAppPersonAccount.icdoBenefitApplicationPersonAccount.person_account_id == ibusPersonAccount.icdoPersonAccount.person_account_id &&
                            lobjAppPersonAccount.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes)
                        {
                            if (lobjPayeeAccount.iclbPaymentHistoryHeader == null)
                                lobjPayeeAccount.LoadPaymentHistoryHeader();
                            //uat pir 1384 : need to take payment history is ascending order
                            lobjPayeeAccount.iclbPaymentHistoryHeader
                                = busGlobalFunctions.Sort<busPaymentHistoryHeader>("icdoPaymentHistoryHeader.payment_date asc", lobjPayeeAccount.iclbPaymentHistoryHeader);

                            if (lobjPayeeAccount.iclbPaymentHistoryHeader.Count > 0)
                            {
                                if (lobjPayeeAccount.iclbPaymentHistoryHeader[0].ibusPaymentSchedule == null)
                                    lobjPayeeAccount.iclbPaymentHistoryHeader[0].LoadPaymentSchedule();
                                //uat pir 1384 : need to take payment history payment date if payment schedule is null
                                return lobjPayeeAccount.iclbPaymentHistoryHeader[0].ibusPaymentSchedule.icdoPaymentSchedule.payment_schedule_id > 0 ?
                                    lobjPayeeAccount.iclbPaymentHistoryHeader[0].ibusPaymentSchedule.icdoPaymentSchedule.payment_date :
                                    lobjPayeeAccount.iclbPaymentHistoryHeader[0].icdoPaymentHistoryHeader.payment_date;
                            }
                        }
                    }
                }
            }
            return DateTime.MinValue;
        }



        #region Method Validations
        public bool IsValidOrgPlanExists()
        {
            bool lblnResult = false;

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();

            if (ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                ibusPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();

            if (ibusPersonAccount.ibusOrgPlan == null)
            {
                //pir 2381
                //ibusPersonAccount.LoadOrgPlan();
                LoadOrgPlan();
            }

            if (ibusPersonAccount.ibusOrgPlan.icdoOrgPlan.org_plan_id > 0)
            {
                lblnResult = true;
            }

            return lblnResult;
        }

        public bool IsNameValid()
        {
            if (ibusPerson == null)
                LoadPerson();
            Boolean IsLastNameDiffer = false, IsFirstNameDiffer = false;

            //uat pir 2488 : first five chars to be used for validation
            if (!string.IsNullOrEmpty(icdoEmployerPayrollDetail.last_name) && !string.IsNullOrEmpty(ibusPerson.icdoPerson.last_name))
            {
                 
                     IsLastNameDiffer=
                    (_icdoEmployerPayrollDetail.last_name.Trim().ToLower().ReplaceWith("[^a-zA-Z0-9]", "")) ==
                    (_ibusPerson.icdoPerson.last_name.Trim().ToLower().ReplaceWith("[^a-zA-Z0-9]", ""));
            }
                if (!string.IsNullOrEmpty(icdoEmployerPayrollDetail.first_name) && !string.IsNullOrEmpty(ibusPerson.icdoPerson.first_name))
                {

                    //PIR 14166 - Deferred Comp First name validation
                    //All retirement, purchases, and deferred comp, validate the full SSN, the 1st 5 letters of the Last Name, and the 1st letter of the First Name.

                    //IsFirstNameDiffer = (_icdoEmployerPayrollDetail.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Length > 5 ? _icdoEmployerPayrollDetail.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 5) : _icdoEmployerPayrollDetail.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "")) ==
                    //    (_ibusPerson.icdoPerson.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Length > 5 ? _ibusPerson.icdoPerson.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 5) : _ibusPerson.icdoPerson.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", ""));


                   
                     IsFirstNameDiffer = (_icdoEmployerPayrollDetail.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 1)) ==
                                        (_ibusPerson.icdoPerson.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 1) );

                }
                if (IsLastNameDiffer && IsFirstNameDiffer)
                    return true;
                else
                    return false;
        }

        //Org to bill
        public bool IsNameValidMedicare()
        {
            if (ibusPerson == null)
                LoadPerson();
            Boolean IsLastNameDiffer = false, IsFirstNameDiffer = false;

            busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory();
            lobjMedicare.icdoPersonAccount = new cdoPersonAccount();
            lobjMedicare.icdoPersonAccount.person_id = icdoEmployerPayrollDetail.person_id;
            lobjMedicare.LoadPerson();
            lobjMedicare.FindByMemberPersonID(icdoEmployerPayrollDetail.person_id);
            lobjMedicare.LoadMedicarePartDMembers();

            foreach (busPersonAccountMedicarePartDHistory lobjPAMedicareHistory in lobjMedicare.iclbPersonAccountMedicarePartDMembers)
            {
                busPerson lobjPerson = new busPerson();
                lobjPerson.FindPerson(lobjPAMedicareHistory.icdoPersonAccountMedicarePartDHistory.person_id);

                if (!string.IsNullOrEmpty(icdoEmployerPayrollDetail.last_name) && !string.IsNullOrEmpty(lobjPerson.icdoPerson.last_name))
                {

                    IsLastNameDiffer =
                   (_icdoEmployerPayrollDetail.last_name.Trim().ToLower().ReplaceWith("[^a-zA-Z0-9]", "")) ==
                   (lobjPerson.icdoPerson.last_name.Trim().ToLower().ReplaceWith("[^a-zA-Z0-9]", ""));
                }
                if (!string.IsNullOrEmpty(icdoEmployerPayrollDetail.first_name) && !string.IsNullOrEmpty(lobjPerson.icdoPerson.first_name))
                {
                    IsFirstNameDiffer = (_icdoEmployerPayrollDetail.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 1)) ==
                                       (lobjPerson.icdoPerson.first_name.ToLower().ReplaceWith("[^a-zA-Z0-9]", "").Substring(0, 1));

                }
                if (IsLastNameDiffer && IsFirstNameDiffer)
                    break;
            }
            if (IsLastNameDiffer && IsFirstNameDiffer)
                return true;
            else
                return false;
        }

        //Check Pay Period / Pay month is same for header and detail
        //105
        public bool MatchPayPeriodForHeaderDetail()
        {
            if (_icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular)
            {
                if ((_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt) ||
                    (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr))
                {
                    if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date != icdoEmployerPayrollDetail.pay_period_date)
                    {
                        return true;
                    }
                }
                else if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_start_date != _icdoEmployerPayrollDetail.pay_period_start_date)
                    {
                        return true;
                    }
                    //PIR 14154 - IF Header and  Detail dates  are not same throw hard error 
                    if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period_end_date != _icdoEmployerPayrollDetail.pay_period_end_date)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //Check If Contribution/Premium is null 
        public bool CheckContributionOrPremiumNull()
        {
            decimal ldecSumOfContributions = 0.00M;
            if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                ldecSumOfContributions = _icdoEmployerPayrollDetail.ee_contribution_reported
                                            + _icdoEmployerPayrollDetail.er_contribution_reported
                                            + _icdoEmployerPayrollDetail.ee_pre_tax_reported
                                            + _icdoEmployerPayrollDetail.ee_employer_pickup_reported
                                            + _icdoEmployerPayrollDetail.rhic_ee_contribution_reported
                                            + _icdoEmployerPayrollDetail.ee_post_tax_addl_reported          //PIR 25920 DC 2025
                                            + _icdoEmployerPayrollDetail.ee_pretax_addl_reported
                                            + _icdoEmployerPayrollDetail.er_pretax_match_reported
                                            + _icdoEmployerPayrollDetail.adec_reported;
            }
            if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                ldecSumOfContributions = _icdoEmployerPayrollDetail.contribution_amount1 +
                                                _icdoEmployerPayrollDetail.contribution_amount2 +
                                                _icdoEmployerPayrollDetail.contribution_amount3 +
                                                _icdoEmployerPayrollDetail.contribution_amount4 +
                                                _icdoEmployerPayrollDetail.contribution_amount5 +
                                                _icdoEmployerPayrollDetail.contribution_amount6 +
                                                _icdoEmployerPayrollDetail.contribution_amount7;

            }
            if (_ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                ldecSumOfContributions = _icdoEmployerPayrollDetail.premium_amount;
            }
            if (ldecSumOfContributions == 0.00M)
            {
                return false;
            }
            return true;
        }

        //BR 130 Check if provider is linked to Employer and is active
        public void CheckProviderNotLinkedToEmployer()
        {
            //Commented As Per PIR 311
            //if (_icdoEmployerPayrollDetail.plan_id != busConstant.PlanIdOther457)
            //{
            DateTime ldtParticipationStartDate = DateTime.MinValue;
            DateTime ldtParticipationEndDate = DateTime.MinValue;
            DataTable ldtbOrgPlan = Select<cdoOrgPlan>(
                    new string[2] { "org_id", "plan_id" },
                    new object[2] { _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id, _icdoEmployerPayrollDetail.plan_id }, null, null);
            if ((_icdoEmployerPayrollDetail.pay_period_start_date != DateTime.MinValue) && (_icdoEmployerPayrollDetail.pay_period_end_date != DateTime.MinValue))
            {
                //PROD PIR 5497 : need to take effective org plan
                foreach (DataRow ldrOrgPlan in ldtbOrgPlan.Rows)
                {
                    ldtParticipationStartDate = DateTime.MinValue; ldtParticipationEndDate = DateTime.MinValue; //PROD PIR 7995 -- Refresh the values
                    if (!String.IsNullOrEmpty((ldrOrgPlan["PARTICIPATION_START_DATE"]).ToString()))
                    {
                        ldtParticipationStartDate = Convert.ToDateTime(ldrOrgPlan["PARTICIPATION_START_DATE"]);
                    }
                    if (!String.IsNullOrEmpty((ldrOrgPlan["PARTICIPATION_END_DATE"]).ToString()))
                    {
                        ldtParticipationEndDate = Convert.ToDateTime(ldrOrgPlan["PARTICIPATION_END_DATE"]);
                    }
                    //Check overlapping of plan particiaption dates
                    //PIR 7682 argument order changed
                    //if (busGlobalFunctions.CheckDateOverlapping(_icdoEmployerPayrollDetail.pay_period_start_date, ldtParticipationStartDate, ldtParticipationEndDate, _icdoEmployerPayrollDetail.pay_period_end_date))
                    if (busGlobalFunctions.CheckDateOverlapping(_icdoEmployerPayrollDetail.pay_period_start_date, _icdoEmployerPayrollDetail.pay_period_end_date, ldtParticipationStartDate, ldtParticipationEndDate))
                    {
                        {
                            if (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id1))
                            {
                                _isProvider1IsLinkedToOrg = CheckProviderExistsAndActive(Convert.ToInt32(ldrOrgPlan["ORG_PLAN_ID"]), _icdoEmployerPayrollDetail.provider_id1);
                            }
                            if (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id2))
                            {
                                _isProvider2IsLinkedToOrg = CheckProviderExistsAndActive(Convert.ToInt32(ldrOrgPlan["ORG_PLAN_ID"]), _icdoEmployerPayrollDetail.provider_id2);
                            }
                            if (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id3))
                            {
                                _isProvider3IsLinkedToOrg = CheckProviderExistsAndActive(Convert.ToInt32(ldrOrgPlan["ORG_PLAN_ID"]), _icdoEmployerPayrollDetail.provider_id3);
                            }
                            if (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id4))
                            {
                                _isProvider4IsLinkedToOrg = CheckProviderExistsAndActive(Convert.ToInt32(ldrOrgPlan["ORG_PLAN_ID"]), _icdoEmployerPayrollDetail.provider_id4);
                            }
                            if (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id5))
                            {
                                _isProvider5IsLinkedToOrg = CheckProviderExistsAndActive(Convert.ToInt32(ldrOrgPlan["ORG_PLAN_ID"]), _icdoEmployerPayrollDetail.provider_id5);
                            }
                            if (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id6))
                            {
                                _isProvider6IsLinkedToOrg = CheckProviderExistsAndActive(Convert.ToInt32(ldrOrgPlan["ORG_PLAN_ID"]), _icdoEmployerPayrollDetail.provider_id6);
                            }
                            if (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id7))
                            {
                                _isProvider7IsLinkedToOrg = CheckProviderExistsAndActive(Convert.ToInt32(ldrOrgPlan["ORG_PLAN_ID"]), _icdoEmployerPayrollDetail.provider_id7);
                            }
                        }
                    }
                }
            }
            //}
        }

        public bool CheckProviderExistsAndActive(int aintOrgPlanId, int aintProviderID)
        {
            DataTable ldtbProvider1 = Select<cdoOrgPlanProvider>(
                        new string[3] { "ORG_PLAN_ID", "PROVIDER_ORG_ID", "STATUS_VALUE" },
                        new object[3] { aintOrgPlanId, aintProviderID, "ACTV" }, null, null);
            if (ldtbProvider1.Rows.Count > 0)
                return true;
            return false;
        }

        // PIR 7585
        public bool IsValidProviderPlanCombination()
        {
            DataTable ldtbProviderOrg = Select("cdoOrganization.GetPlanOfProvider", new object[2] { icdoEmployerPayrollDetail.provider_org_id, icdoEmployerPayrollDetail.plan_id });
            if (ldtbProviderOrg.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }


        //PIR 13996

        public Boolean IsEligibleWagesValid()
        {
            if (_icdoEmployerPayrollDetail != null)
            {
                if (!string.IsNullOrEmpty(_icdoEmployerPayrollDetail.record_type_value) && _icdoEmployerPayrollDetail.record_type_value == "-ADJ")
                {

                    if (_icdoEmployerPayrollDetail.eligible_wages > 0)
                    {
                        return false;
                    }
                }
                if (!string.IsNullOrEmpty(_icdoEmployerPayrollDetail.record_type_value) && _icdoEmployerPayrollDetail.record_type_value == "REG")
                {

                    if (_icdoEmployerPayrollDetail.eligible_wages < 0)
                    {
                        return false;
                    }
                }

            }
            return true;
        }

        //PIR 14154- Validating Pay Period dates for Payroll Detail while uploading
        public Boolean IsPayPeriodStarDateValid()
        {           
             return ValidatePayPeriodDates(icdoEmployerPayrollDetail.pay_period_start_date.ToShortDateString().ToString()); 
        }

        //PIR 14154- Validating Pay Period dates for Payroll Detail while uploading
        public Boolean IsPayPeriodEndDateValid()
        { 
            return ValidatePayPeriodDates(icdoEmployerPayrollDetail.pay_period_end_date.ToShortDateString().ToString()) ;
        }


        //PIR 14154- Validating Pay Period End date with report frequency type
        public Boolean CheckEndDateByReportFrequency()
        {
            if (icdoEmployerPayrollDetail.pay_period_start_date != DateTime.MinValue && icdoEmployerPayrollDetail.pay_period_end_date != DateTime.MinValue)
            {
                string lstrFrequency = GetDeferredCompFrequencyPeriodByOrg();

                if (ibusEmployerPayrollHeader == null)
                    LoadPayrollHeader();
                //PIR 19701 Day_Of_Month from Org Plan Maintenance to be used for verify validation 4690           
                int lintDayofMonth = 1;
                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp && (lstrFrequency == busConstant.DeffCompFrequencyMonthly || lstrFrequency == busConstant.DeffCompFrequencySemiMonthly))
                    lintDayofMonth = ibusEmployerPayrollHeader.GetDayOfMonthForMonthlyFrequency(icdoEmployerPayrollDetail.pay_period_start_date, icdoEmployerPayrollDetail.pay_period_end_date);
                DateTime ldtEndDateLastPaid = busEmployerReportHelper.GetEndDateByReportFrequency(icdoEmployerPayrollDetail.pay_period_start_date, lstrFrequency, lintDayofMonth);

                if (ldtEndDateLastPaid != icdoEmployerPayrollDetail.pay_period_end_date)
                    return false;
            }
            return true;
        }

        public string GetDeferredCompFrequencyPeriodByOrg()
        {
            //Set Default
            string lstrResult = null;
            //PROD PIR 5497            
            if (icdoEmployerPayrollDetail.pay_period_start_date != DateTime.MinValue && icdoEmployerPayrollDetail.pay_period_end_date != DateTime.MinValue)
            {
                DataTable ldtbFrequency = Select("cdoEmployerPayrollHeader.GetDeferredCompFrequencyByOrg",
                                                 new object[3] { ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id, icdoEmployerPayrollDetail.pay_period_start_date, icdoEmployerPayrollDetail.pay_period_end_date });
                if (ldtbFrequency.Rows.Count > 0)
                {
                    lstrResult = ldtbFrequency.Rows[0]["REPORT_FREQUENCY_VALUE"].ToString();
                }
            }
           
            return lstrResult;
        }

        public Boolean ValidatePayPeriodDates(String strPayPeriodDate)
        {


            String[] strdates = strPayPeriodDate.Split("/");
            Int32 year = 0, month = 0, day = 0;

            Boolean isleap = false;

            // For some cases the Reporting month field is read with the remaining underscores ("_")
            //so addidng below check ex "01/111_"

            if (strdates[0].IsNotNullOrEmpty() && !strdates[0].Contains("_"))
            {
                month = Convert.ToInt32(strdates[0]);
            }

            if (strdates[1].IsNotNullOrEmpty() && !strdates[1].Contains("_"))
            {
                day = Convert.ToInt32(strdates[1]);

            }
            if (strdates[2].IsNotNullOrEmpty() && !strdates[2].Contains("_"))
            {
                year = Convert.ToInt32(strdates[2]);
            }

            isleap = DateTime.IsLeapYear(year);

            if ((month < 1 || month > 12) || (day < 1 || day > 31) || (year < 1901 || year > 2100))
            {
                return false;
            }

            if ((month == 4 || month == 6 || month == 9 || month == 11) && day == 31)
            {
                return false;
            }

            if (month == 2)
            {

                if (day > 29 || (day == 29 && !isleap))
                {
                    return false;
                }
            }



            return true; 
        }
        #endregion

        //Automatic Allocation IF Remittance Matches
        public void AutomaticAllocation()
        {
            if (icdoEmployerPayrollDetail.purchase_amount_reported > 0)
            {
                decimal ldecTotalPurchaseAmountAllocated = 0.00M;
                _ibusEmployerPurchaseAllocation = new busEmployerPurchaseAllocation();
                ibusEmployerPurchaseAllocation.LoadAllServicePurchaseHeaderForAllocation(this);

                DataTable ldtbDetailPurchase = Select<cdoEmployerPurchaseAllocation>(new string[1] { "employer_payroll_detail_id" }, new object[1] { _icdoEmployerPayrollDetail.employer_payroll_detail_id }, null, null);

                //get sum of purchase amount for this detail
                foreach (DataRow dr in ldtbDetailPurchase.Rows)
                {
                    if (!String.IsNullOrEmpty(dr["allocated_amount"].ToString()))
                    {
                        ldecTotalPurchaseAmountAllocated += Convert.ToDecimal(dr["allocated_amount"]);
                    }
                }

                if (ldecTotalPurchaseAmountAllocated < icdoEmployerPayrollDetail.purchase_amount_reported)
                {
                    if (ibusEmployerPurchaseAllocation.idecTotalExpectedPaymentAmount > 0)
                    {
                        if ((icdoEmployerPayrollDetail.purchase_amount_reported == ibusEmployerPurchaseAllocation.idecTotalExpectedPaymentAmount) || (ibusEmployerPurchaseAllocation.idecTotalExpectedPaymentAmount / 2 == icdoEmployerPayrollDetail.purchase_amount_reported && ibusEmployerPurchaseAllocation.iclbServicePurchase.Any(pur=>pur.icdoServicePurchaseHeader.payment_frequency_value == busConstant.ServicePurchasePaymentFrequencyValueMonthly && pur.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment)))
                        {
                            foreach (busServicePurchaseHeader lobjServicePurchaseHeader in ibusEmployerPurchaseAllocation.iclbServicePurchase)
                            {
                                decimal ldecAllocatdAmt = (icdoEmployerPayrollDetail.purchase_amount_reported == ibusEmployerPurchaseAllocation.idecTotalExpectedPaymentAmount) ? lobjServicePurchaseHeader.icdoServicePurchaseHeader.expected_installment_amount : icdoEmployerPayrollDetail.purchase_amount_reported;
                                //Create Purchase Allocation
                                CreatePurchaseAllocation(lobjServicePurchaseHeader, ldecAllocatdAmt);
                            }
                        }
                    }
                }
                if (!(ibusEmployerPurchaseAllocation.iclbServicePurchase.Any(pur => pur.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment && pur.icdoServicePurchaseHeader.payment_frequency_value == busConstant.ServicePurchasePaymentFrequencyValueMonthly)) &&
                    ldecTotalPurchaseAmountAllocated == icdoEmployerPayrollDetail.purchase_amount_reported && ldtbDetailPurchase?.Rows?.Count > 0)
                {
                    try
                    {
                        Collection<busEmployerPurchaseAllocation> lclbAllocation = GetCollection<busEmployerPurchaseAllocation>(ldtbDetailPurchase, "icdoEmployerPurchaseAllocation");
                        foreach (busEmployerPurchaseAllocation lbusEmployerPurchaseAllocation in lclbAllocation)
                        {
                            lbusEmployerPurchaseAllocation.icdoEmployerPurchaseAllocation.Delete();
                        }
                    }
                    catch (Exception ex)
                    {
                        Sagitec.ExceptionPub.ExceptionManager.Publish(ex);
                    }

                }
            }
        }

        private void CreatePurchaseAllocation(busServicePurchaseHeader lobjServicePurchaseHeader, decimal adecAllocatedAmt)
        {
            busEmployerPurchaseAllocation lobjPurchaseAllocation = new busEmployerPurchaseAllocation();
            lobjPurchaseAllocation.icdoEmployerPurchaseAllocation = new cdoEmployerPurchaseAllocation();
            lobjPurchaseAllocation.icdoEmployerPurchaseAllocation.allocated_amount = adecAllocatedAmt;
            lobjPurchaseAllocation.icdoEmployerPurchaseAllocation.allocated_date = DateTime.Now;
            lobjPurchaseAllocation.icdoEmployerPurchaseAllocation.employer_payroll_detail_id = icdoEmployerPayrollDetail.employer_payroll_detail_id;
            lobjPurchaseAllocation.icdoEmployerPurchaseAllocation.service_purchase_header_id = lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id;
            lobjPurchaseAllocation.icdoEmployerPurchaseAllocation.Insert();
        }

        public bool CheckContributionEnteredOrProviderNull()
        {
            if ((_icdoEmployerPayrollDetail.contribution_amount1 != 0.00M) && (String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id1)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount2 != 0.00M) && (String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id2)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount3 != 0.00M) && (String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id3)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount4 != 0.00M) && (String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id4)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount5 != 0.00M) && (String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id5)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount6 != 0.00M) && (String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id6)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount7 != 0.00M) && (String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id7)))
            {
                return false;
            }
            return true;
        }
        public bool CheckContributionNullOrProviderNotNull()
        {
            if ((_icdoEmployerPayrollDetail.contribution_amount1 == 0.00M) && (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id1)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount2 == 0.00M) && (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id2)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount3 == 0.00M) && (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id3)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount4 == 0.00M) && (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id4)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount5 == 0.00M) && (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id5)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount6 == 0.00M) && (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id6)))
            {
                return false;
            }
            if ((_icdoEmployerPayrollDetail.contribution_amount7 == 0.00M) && (!String.IsNullOrEmpty(_icdoEmployerPayrollDetail.provider_org_code_id7)))
            {
                return false;
            }
            return true;
        }

        //TO check if the report amount for each detail is same  as expected installment amount
        //else throw error
        public bool CheckExpectedInstallmentAmount()
        {
            busEmployerPurchaseAllocation lobjPurchaseAllocation = new busEmployerPurchaseAllocation();
            lobjPurchaseAllocation.LoadAllServicePurchaseHeaderForAllocation(this);
            if (lobjPurchaseAllocation.idecTotalExpectedPaymentAmount != 0.00M)
            {
                if (lobjPurchaseAllocation.idecTotalExpectedPaymentAmount == icdoEmployerPayrollDetail.purchase_amount_reported)
                {
                    return true;
                }
                else if(lobjPurchaseAllocation.iclbServicePurchase.Any(pur=>pur.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment && pur.icdoServicePurchaseHeader.payment_frequency_value == busConstant.ServicePurchasePaymentFrequencyValueMonthly) && lobjPurchaseAllocation.idecTotalExpectedPaymentAmount/2 == icdoEmployerPayrollDetail.purchase_amount_reported)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// BR - 87
        /// valiate contribution rates data for Retirement only        
        /// </summary>
        /// <returns></returns>         
        public bool CompareContributionRatesForRetirement()
        {
            bool lblnResult = true;

            if (String.IsNullOrEmpty(member_type))
                LoadMemberType();

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();
            //PIR 24585 Display only 4663 when contribution is null and 4707 when weges is null // PIR 25920 DC 2025 
            if ((ibusPersonAccount.ibusPersonEmploymentDetail != null) && ((icdoEmployerPayrollDetail.eligible_wages != 0) && ((icdoEmployerPayrollDetail.ee_contribution_reported !=0) ||
                (icdoEmployerPayrollDetail.ee_pre_tax_reported != 0) || (icdoEmployerPayrollDetail.ee_employer_pickup_reported != 0) || (icdoEmployerPayrollDetail.er_contribution_reported != 0)
                || (icdoEmployerPayrollDetail.er_contribution_reported != 0) || (icdoEmployerPayrollDetail.rhic_ee_contribution_reported != 0) || (icdoEmployerPayrollDetail.rhic_er_contribution_reported != 0)
                || (icdoEmployerPayrollDetail.ee_pretax_addl_reported != 0) || (icdoEmployerPayrollDetail.ee_post_tax_addl_reported != 0) || (icdoEmployerPayrollDetail.er_pretax_match_reported != 0) || (icdoEmployerPayrollDetail.adec_reported != 0))))
            {
                lblnResult = IsRetirementRateMatches();
            }
            return lblnResult;
        }

        public bool IsRetirementRateMatches()
        {
            //pir 6649 - undone 6649 logic for PIR 14291 fix
            //if (ibusRetirementContributionRegular != null && ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution != null &&
            //    ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.retirement_contribution_id > 0)
            //{
            //    if (Math.Round(ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.post_tax_ee_amount / ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.salary_amount, 2, MidpointRounding.AwayFromZero)
            //        == Math.Round(_icdoEmployerPayrollDetail.ee_contribution_reported / _icdoEmployerPayrollDetail.eligible_wages, 2, MidpointRounding.AwayFromZero)
            //        && Math.Round(ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.pre_tax_ee_amount / ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.salary_amount, 2, MidpointRounding.AwayFromZero)
            //        == Math.Round(_icdoEmployerPayrollDetail.ee_pre_tax_reported / _icdoEmployerPayrollDetail.eligible_wages, 2, MidpointRounding.AwayFromZero)
            //        && Math.Round(ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.pre_tax_er_amount / ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.salary_amount, 2, MidpointRounding.AwayFromZero)
            //        == Math.Round(_icdoEmployerPayrollDetail.er_contribution_reported / _icdoEmployerPayrollDetail.eligible_wages, 2, MidpointRounding.AwayFromZero)
            //        && Math.Round(ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.ee_er_pickup_amount / ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.salary_amount, 2, MidpointRounding.AwayFromZero)
            //        == Math.Round(_icdoEmployerPayrollDetail.ee_employer_pickup_reported / _icdoEmployerPayrollDetail.eligible_wages, 2, MidpointRounding.AwayFromZero)
            //        && Math.Round(ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.ee_rhic_amount / ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.salary_amount, 2, MidpointRounding.AwayFromZero)
            //        == Math.Round(_icdoEmployerPayrollDetail.rhic_ee_contribution_reported / _icdoEmployerPayrollDetail.eligible_wages, 2, MidpointRounding.AwayFromZero)
            //        && Math.Round(ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.er_rhic_amount / ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.salary_amount, 2, MidpointRounding.AwayFromZero)
            //        == Math.Round(_icdoEmployerPayrollDetail.rhic_er_contribution_reported / _icdoEmployerPayrollDetail.eligible_wages, 2, MidpointRounding.AwayFromZero))
            //        return true;
            //}
            // compare rates with detail  // PIR 14471 - Added Round function as calculated value comes with 4 decimal points
            if ((Math.Abs(Math.Round(_icdoEmployerPayrollDetail.ee_pre_tax_calculated, 2) - Math.Round(_icdoEmployerPayrollDetail.ee_pre_tax_reported, 2))) > idecToleranceLimit)
            {
                return false;
            }
            if ((Math.Abs(Math.Round(_icdoEmployerPayrollDetail.ee_employer_pickup_calculated, 2) - Math.Round(_icdoEmployerPayrollDetail.ee_employer_pickup_reported, 2))) > idecToleranceLimit)
            {
                return false;
            }
            if ((Math.Abs(Math.Round(_icdoEmployerPayrollDetail.ee_contribution_calculated, 2) - Math.Round(_icdoEmployerPayrollDetail.ee_contribution_reported, 2))) > idecToleranceLimit)
            {
                return false;
            }
            if ((Math.Abs(Math.Round(_icdoEmployerPayrollDetail.er_contribution_calculated, 2) - Math.Round(_icdoEmployerPayrollDetail.er_contribution_reported, 2))) > idecToleranceLimit)
            {
                return false;
            }
            if ((Math.Abs(Math.Round(_icdoEmployerPayrollDetail.rhic_ee_contribution_calculated, 2) - Math.Round(_icdoEmployerPayrollDetail.rhic_ee_contribution_reported, 2))) > idecToleranceLimit)
            {
                return false;
            }
            if ((Math.Abs(Math.Round(_icdoEmployerPayrollDetail.rhic_er_contribution_calculated, 2) - Math.Round(_icdoEmployerPayrollDetail.rhic_er_contribution_reported, 2))) > idecToleranceLimit)
            {
                return false;
            }
			//PIR 25920 New Plan DC 2025
            //new calculated fields should populate for all Ret plans if values are > 0 (Maik mail dated 4/23/24) 
                if ((Math.Abs(Math.Round(_icdoEmployerPayrollDetail.ee_pretax_addl_calculated, 2) - Math.Round(_icdoEmployerPayrollDetail.ee_pretax_addl_reported, 2))) > idecToleranceLimit)
                {
                    return false;
                }
                if ((Math.Abs(Math.Round(_icdoEmployerPayrollDetail.ee_post_tax_addl_calculated, 2) - Math.Round(_icdoEmployerPayrollDetail.ee_post_tax_addl_reported, 2))) > idecToleranceLimit)
                {
                    return false;
                }
                if ((Math.Abs(Math.Round(_icdoEmployerPayrollDetail.er_pretax_match_calculated, 2) - Math.Round(_icdoEmployerPayrollDetail.er_pretax_match_reported, 2))) > idecToleranceLimit)
                {
                    return false;
                }
                if ((Math.Abs(Math.Round(_icdoEmployerPayrollDetail.adec_calculated, 2) - Math.Round(_icdoEmployerPayrollDetail.adec_reported, 2))) > idecToleranceLimit)
                {
                    return false;
                }
            
            return true;
        }

        //This Amount we are getting it by linking the member with provider.SO, if the amount is zero, Member is not linked with provider
        public bool CheckMemberIsLinkedWithProvider()
        {
            if (icdoEmployerPayrollDetail.provider_id1 != 0)
            {
                if (!icdoEmployerPayrollDetail.is_provider1_linked_with_member)
                    return false;
            }
            if (icdoEmployerPayrollDetail.provider_id2 != 0)
            {
                if (!icdoEmployerPayrollDetail.is_provider2_linked_with_member)
                    return false;
            }
            if (icdoEmployerPayrollDetail.provider_id3 != 0)
            {
                if (!icdoEmployerPayrollDetail.is_provider3_linked_with_member)
                    return false;
            }
            if (icdoEmployerPayrollDetail.provider_id4 != 0)
            {
                if (!icdoEmployerPayrollDetail.is_provider4_linked_with_member)
                    return false;
            }
            if (icdoEmployerPayrollDetail.provider_id5 != 0)
            {
                if (!icdoEmployerPayrollDetail.is_provider5_linked_with_member)
                    return false;
            }
            if (icdoEmployerPayrollDetail.provider_id6 != 0)
            {
                if (!icdoEmployerPayrollDetail.is_provider6_linked_with_member)
                    return false;
            }
            if (icdoEmployerPayrollDetail.provider_id7 != 0)
            {
                if (!icdoEmployerPayrollDetail.is_provider7_linked_with_member)
                    return false;
            }
            return true;
        }


        private bool IsDeferredContributionReportedAmountMatched(ref bool ablnLessReportedAmountFound, ref bool ablnMoreReportedAmountFound)
        {
            //prod pir 4031 :  as discussed with satya, we need to validate for other 457 plan also
            //if (icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdOther457)
            //    return true;

            //get org plan id for providers 
            if (icdoEmployerPayrollDetail.provider_id1 != 0)
            {
                if (!IComparer.Equals(icdoEmployerPayrollDetail.amount_from_enrollment1, icdoEmployerPayrollDetail.contribution_amount1))
                {
                    if (icdoEmployerPayrollDetail.contribution_amount1 < icdoEmployerPayrollDetail.amount_from_enrollment1)
                        ablnLessReportedAmountFound = true;
                    else
                        ablnMoreReportedAmountFound = true;
                }
            }
            if (icdoEmployerPayrollDetail.provider_id2 != 0)
            {
                if (!IComparer.Equals(icdoEmployerPayrollDetail.amount_from_enrollment2, icdoEmployerPayrollDetail.contribution_amount2))
                {
                    if (icdoEmployerPayrollDetail.contribution_amount2 < icdoEmployerPayrollDetail.amount_from_enrollment2)
                        ablnLessReportedAmountFound = true;
                    else
                        ablnMoreReportedAmountFound = true;
                }
            }
            if (icdoEmployerPayrollDetail.provider_id3 != 0)
            {
                if (!IComparer.Equals(icdoEmployerPayrollDetail.amount_from_enrollment3, icdoEmployerPayrollDetail.contribution_amount3))
                {
                    if (icdoEmployerPayrollDetail.contribution_amount3 < icdoEmployerPayrollDetail.amount_from_enrollment3)
                        ablnLessReportedAmountFound = true;
                    else
                        ablnMoreReportedAmountFound = true;
                }
            }
            if (icdoEmployerPayrollDetail.provider_id4 != 0)
            {
                if (!IComparer.Equals(icdoEmployerPayrollDetail.amount_from_enrollment4, icdoEmployerPayrollDetail.contribution_amount4))
                {
                    if (icdoEmployerPayrollDetail.contribution_amount4 < icdoEmployerPayrollDetail.amount_from_enrollment4)
                        ablnLessReportedAmountFound = true;
                    else
                        ablnMoreReportedAmountFound = true;
                }
            }
            if (icdoEmployerPayrollDetail.provider_id5 != 0)
            {
                if (!IComparer.Equals(icdoEmployerPayrollDetail.amount_from_enrollment5, icdoEmployerPayrollDetail.contribution_amount5))
                {
                    if (icdoEmployerPayrollDetail.contribution_amount5 < icdoEmployerPayrollDetail.amount_from_enrollment5)
                        ablnLessReportedAmountFound = true;
                    else
                        ablnMoreReportedAmountFound = true;
                }
            }
            if (icdoEmployerPayrollDetail.provider_id6 != 0)
            {
                if (!IComparer.Equals(icdoEmployerPayrollDetail.amount_from_enrollment6, icdoEmployerPayrollDetail.contribution_amount6))
                {
                    if (icdoEmployerPayrollDetail.contribution_amount6 < icdoEmployerPayrollDetail.amount_from_enrollment6)
                        ablnLessReportedAmountFound = true;
                    else
                        ablnMoreReportedAmountFound = true;
                }
            }
            if (icdoEmployerPayrollDetail.provider_id7 != 0)
            {
                if (!IComparer.Equals(icdoEmployerPayrollDetail.amount_from_enrollment7, icdoEmployerPayrollDetail.contribution_amount7))
                {
                    if (icdoEmployerPayrollDetail.contribution_amount7 < icdoEmployerPayrollDetail.amount_from_enrollment7)
                        ablnLessReportedAmountFound = true;
                    else
                        ablnMoreReportedAmountFound = true;
                }
            }
            
            if (ablnLessReportedAmountFound || ablnMoreReportedAmountFound)
                return false;
            return true;
        }
        /// <summary>
        /// //PIR 25920 New Plan DC 2025
        /// Get sum of all provider contribution Amount for Deferred Comp
        /// </summary>
        /// <returns>decimal sum of amount</returns>
        public decimal GetSumOfAllDefCompProviderAmountFromEnrollment()
        {
            return icdoEmployerPayrollDetail.amount_from_enrollment1 + icdoEmployerPayrollDetail.amount_from_enrollment2 + icdoEmployerPayrollDetail.amount_from_enrollment3 +
                icdoEmployerPayrollDetail.amount_from_enrollment4 + icdoEmployerPayrollDetail.amount_from_enrollment5 + icdoEmployerPayrollDetail.amount_from_enrollment6 + icdoEmployerPayrollDetail.amount_from_enrollment7;
        }
        /// <summary>
        /// BR - 88
        /// Get Pledge Amount for Deferred Comp
        /// </summary>
        /// <returns></returns>
        public bool IsDeferredCompReportedAmountMoreThanEnrollment()
        {
            bool lblnIsReportedAmtLessThanEnrollment = false;
            bool lblnIsReportedAmtMoreThanEnrollment = false;
            bool lblnIsValid = IsDeferredContributionReportedAmountMatched(ref lblnIsReportedAmtLessThanEnrollment, ref lblnIsReportedAmtMoreThanEnrollment);
            if (!lblnIsValid)
            {
                if (lblnIsReportedAmtMoreThanEnrollment)
                    return true;
            }
            return false;
        }

        public bool IsDeferredCompReportedAmountLessthanEnrollment()
        {
            bool lblnIsReportedAmtLessThanEnrollment = false;
            bool lblnIsReportedAmtMoreThanEnrollment = false;
            bool lblnIsValid = IsDeferredContributionReportedAmountMatched(ref lblnIsReportedAmtLessThanEnrollment, ref lblnIsReportedAmtMoreThanEnrollment);
            if (!lblnIsValid)
            {
                if (lblnIsReportedAmtLessThanEnrollment)
                    return true;
            }
            return false;
        }
        public bool IsEmployerMatchAmountNotMatchedWithReportedAmount()
        {
            //PIR 25920 New Plan DC 2025  
            //decimal ldecPreTaxMatchCalculated = icdoEmployerPayrollDetail.er_pretax_match_calculated;
            //decimal ldecSumOfAmountFromEnrollment = GetSumOfAllDefCompProviderAmountFromEnrollment();
            //ldecPreTaxMatchCalculated = ldecPreTaxMatchCalculated > ldecSumOfAmountFromEnrollment ? ldecSumOfAmountFromEnrollment : ldecPreTaxMatchCalculated;
            if ((!IComparer.Equals(icdoEmployerPayrollDetail.er_pretax_match_reported, Math.Round(icdoEmployerPayrollDetail.er_pretax_match_calculated, 2, MidpointRounding.AwayFromZero))) //PIR 27135
                && IsPartialMonthLinkedToContributingEmploymentDetail()) //PIR 27238
            {
                return true;
            }
            return false;
        }
        //PIR 27238
        public bool IsPartialMonthLinkedToContributingEmploymentDetail()
        {
            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            DataTable ldtbSelectedPersonEmpDtl = Select("entPersonEmploymentDetail.IsPartialMonthLinkedToContributingEmploymentDetail", new object[3] { ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                                                                            icdoEmployerPayrollDetail.pay_period_start_date, icdoEmployerPayrollDetail.pay_period_end_date});

            if (ldtbSelectedPersonEmpDtl.Rows.Count > 0)
                    return false;

            return true;
        }
        public busPersonAccountDeferredCompProvider ibusPersonAccountDeferredCompProvider { get; set; }	//PIR 17131
        /// <summary>
        /// compare premium amount for insurance
        /// </summary>       
        /// <param name="aintProviderOrgId"></param>
        /// <param name="adtEffectiveDate"></param>
        /// <returns></returns>
        public decimal GetPledgeAmount(int aintProviderOrgId, DateTime adtEffectiveDate, ref bool ablnLinkedWithProvider, bool ablnLoadProviderObject = false)
        {
            decimal ldecPledgeAmount = 0.00M;

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.icdoPersonAccount.person_account_id != 0)
            {
                //Load the Provider Org ID
                ibusPersonAccount.LoadProviderOrgPlanByProviderOrgID(aintProviderOrgId, adtEffectiveDate);

                int lintPersonAccountId = ibusPersonAccount.icdoPersonAccount.person_account_id;

                //Person Employment ID added now in Deff Comp Provider Table. We must check the org code that should match provider person employment org code.
                DataTable ldtbList = Select<cdoPersonAccountDeferredCompProvider>(
                    new string[2] { "provider_org_plan_id", "person_account_id" },
                    new object[2] { ibusPersonAccount.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id, lintPersonAccountId },
                    null, "start_date desc");

                Collection<busPersonAccountDeferredCompProvider> lclbPADCProvider =
                    GetCollection<busPersonAccountDeferredCompProvider>(ldtbList, "icdoPersonAccountDeferredCompProvider");


                foreach (busPersonAccountDeferredCompProvider lbusPADCProvider in lclbPADCProvider)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                        lbusPADCProvider.icdoPersonAccountDeferredCompProvider.start_date,
                        lbusPADCProvider.icdoPersonAccountDeferredCompProvider.end_date) &&
                        lbusPADCProvider.icdoPersonAccountDeferredCompProvider.start_date != lbusPADCProvider.icdoPersonAccountDeferredCompProvider.end_date_no_null)
                    {
                        busPersonEmployment lbusPersonEmployment = new busPersonEmployment();
                        lbusPersonEmployment.FindPersonEmployment(
                            lbusPADCProvider.icdoPersonAccountDeferredCompProvider.person_employment_id);

                        if (lbusPersonEmployment.icdoPersonEmployment.org_id == ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id)
                        {
                            ablnLinkedWithProvider = true;
                            ldecPledgeAmount = lbusPADCProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                            if(ablnLoadProviderObject)	//PIR 17131
                                ibusPersonAccountDeferredCompProvider = lbusPADCProvider;
                            break;
                        }
                    }
                }
            }
            return ldecPledgeAmount;
        }
        //PIR 17131
        public override busBase GetCorOrganization()
        {

            if (_ibusEmployerPayrollHeader.ibusOrganization == null)            
                _ibusEmployerPayrollHeader.LoadOrganization();
                return _ibusEmployerPayrollHeader.ibusOrganization;
            
        }
        public override void LoadCorresProperties(string astrTemplateName)
        {
           
            if (astrTemplateName == "ENR-5406")
            {
                GetPledgeAmount(icdoEmployerPayrollDetail.provider_id1, icdoEmployerPayrollDetail.pay_period_start_date, ref icdoEmployerPayrollDetail.is_provider1_linked_with_member, true);
                if (ibusPersonAccountDeferredCompProvider.IsNull())
                    ibusPersonAccountDeferredCompProvider = new busPersonAccountDeferredCompProvider() { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
            }
        }
        
        //compare Premium Amount for Insurance
        public bool ComparePremiumAmount()
        {
            return
                IComparer.Equals(
                    Math.Round(icdoEmployerPayrollDetail.premium_amount_from_enrollment, 2,
                               MidpointRounding.AwayFromZero),
                    Math.Round(icdoEmployerPayrollDetail.premium_amount, 2, MidpointRounding.AwayFromZero));
        }

        /// <summary>
        /// BR - 95
        /// for retirement check for contribution date with date of termination
        /// </summary>
        /// <returns></returns>
        public int CheckTerminationDateAndContributionEffectiveDate()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPerson.icolPersonEmployment == null)
                ibusPerson.LoadPersonEmployment(false);

            busPersonEmployment lbusPersonEmployment = ibusPerson.icolPersonEmployment.Where(i => i.icdoPersonEmployment.org_id == ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id &&
                busGlobalFunctions.CheckDateOverlapping(icdoEmployerPayrollDetail.pay_period_date, icdoEmployerPayrollDetail.pay_period_last_date,  //prod pir 4055
                i.icdoPersonEmployment.start_date, i.icdoPersonEmployment.end_date)).FirstOrDefault();

            if (lbusPersonEmployment != null && lbusPersonEmployment.icdoPersonEmployment.person_employment_id > 0)
            {
                if (_ibusPersonAccount == null)
                    LoadPersonAccount();

                if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    /*//If the employer reports before to the termination date but getting posted after that date
                    if (ibusPersonAccount.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementEnrolled)
                    {
                        return false;
                    }*/
                    //PROD PIR - 4548
                    if (ibusPersonAccount.ibusPersonAccountRetirement == null)
                        ibusPersonAccount.LoadPersonAccountRetirement();
                    if (ibusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate == null)
                        ibusPersonAccount.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(icdoEmployerPayrollDetail.pay_period_date);
                    if (ibusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate.icdoPersonAccountRetirementHistory.plan_participation_status_value !=
                        busConstant.PlanParticipationStatusRetirementEnrolled)
                    {
                        return 1;
                    }
                }
            }
            else
            {
                busPersonEmployment lbusClosestEmploymentInEmpHeaderOrg = ibusPerson.icolPersonEmployment.FirstOrDefault(i =>
                                                                                                            i.icdoPersonEmployment.org_id == ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id &&
                                                                                                            i.icdoPersonEmployment.start_date.Date != i.icdoPersonEmployment.end_date.Date &&
                                                                                                            i.icdoPersonEmployment.start_date.Date <= icdoEmployerPayrollDetail.pay_period_date.Date);
                if (lbusClosestEmploymentInEmpHeaderOrg?.icdoPersonEmployment?.person_employment_id > 0 &&
                    lbusClosestEmploymentInEmpHeaderOrg.icdoPersonEmployment.end_date != DateTime.MinValue &&
                    icdoEmployerPayrollDetail.pay_period_date != DateTime.MinValue &&
                    icdoEmployerPayrollDetail.pay_period_date.Date > lbusClosestEmploymentInEmpHeaderOrg.icdoPersonEmployment.end_date.Date)
                {
                    double lintTotalDayDiff = (icdoEmployerPayrollDetail.pay_period_date.Date - lbusClosestEmploymentInEmpHeaderOrg.icdoPersonEmployment.end_date.Date).TotalDays;
                    if (lintTotalDayDiff >= 0 && lintTotalDayDiff <= 31)
                        return 1;
                    else if (lintTotalDayDiff > 31)
                        return 2;
                }
            }
            return 0;
        }

        /// <summary>
        /// BR - 98
        /// this method checks if the salary is changed
        /// and old value variance is within the acceptable limit
        /// for retirement only
        /// </summary>
        /// <returns></returns>
        public bool CheckSalaryAmtChangedWithInVariance()
        {
            //Exclude Bonus PIR ISSUES 13.doc, bonus check is done in xml - PIR 14008
            if (_idecOldSalaryValue != 0.00M)
            {
                if (icdoEmployerPayrollDetail.eligible_wages_rounded != _idecOldSalaryValue)
                {
                    if (!CheckIsHourlyEmployee())
                    {
                        if ((icdoEmployerPayrollDetail.eligible_wages_rounded - _idecOldSalaryValue) > (idecSeasonalAndRegularEmployeeTolerance * _idecOldSalaryValue))
                        {
                            return false;
                        }
                    }
                    //for hourly employee
                    //check if hourly value is selected
                    if (CheckIsHourlyEmployee())
                    {
                        if ((icdoEmployerPayrollDetail.eligible_wages_rounded - _idecOldSalaryValue) > (idecHoulryEmployeeTolerance * _idecOldSalaryValue))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        /// <summary>        
        /// for bonus/Retro pay check for Pay period end date with date of termination PIR 23480 
        /// </summary>
        /// <returns>return true if rule violation else false</returns>
        public bool CheckTerminationDateAndPayEndDateForBonus()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail(true);
            if(ibusEmployerPayrollHeader.IsNull())
                LoadPayrollHeader();

            if (ibusPersonAccount.IsNotNull() && ibusPersonAccount.ibusPersonEmploymentDetail.IsNotNull() &&
                ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                if (ibusPersonAccount.iclbAccountEmploymentDetail == null)
                    ibusPersonAccount.LoadPersonAccountEmploymentDetails();
                if (ibusPersonAccount.iclbAccountEmploymentDetail.IsNotNull())
                {
                    if (icdoEmployerPayrollDetail.pay_period_end_month_for_bonus.IsNull() || icdoEmployerPayrollDetail.pay_period_end_month_for_bonus == DateTime.MinValue)
                        SetPayPeriodDatePayPeriodEndMonthForBonus();
                    if (!ibusPersonAccount.iclbAccountEmploymentDetail.Any(i => i.ibusEmploymentDetail?.ibusPersonEmployment?.icdoPersonEmployment?.org_id ==
                                                                             ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id &&
                                                                             ((busGlobalFunctions.CheckDateOverlapping(Convert.ToDateTime(icdoEmployerPayrollDetail.pay_period_end_month_for_bonus),
                                                                             i.ibusEmploymentDetail?.icdoPersonEmploymentDetail?.start_date.GetFirstDayofCurrentMonth(), i.ibusEmploymentDetail?.icdoPersonEmploymentDetail?.end_date != DateTime.MinValue? i.ibusEmploymentDetail?.icdoPersonEmploymentDetail?.end_date.GetLastDayofMonth(): i.ibusEmploymentDetail?.icdoPersonEmploymentDetail?.end_date)))))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void LoadOldSalaryAmount()
        {
            decimal ldecSalaryOldAmount = 0.00M;
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail(true);
            //prod pir 4302
            busPersonEmployment lobjEmployment = new busPersonEmployment();
            if (ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
            {
                lobjEmployment.FindPersonEmployment(ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_id);
                //lobjEmployment.LoadPersonEmploymentDetail(false);
            }
            else
            {
                lobjEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                //lobjEmployment.icolPersonEmploymentDetail = new Collection<busPersonEmploymentDetail>();
            }
            if (icdoEmployerPayrollDetail.pay_period_last_date != DateTime.MinValue)
            {
                //PIR 14008 - commented - Salary variance logic change
                //DataTable ldtbGetPayrollDetail = busBase.Select<cdoPersonAccountRetirementContribution>(
                //    new string[3] { "person_account_ID", "pay_period_month", "pay_period_year" },
                //    new object[3]
                //    {
                //        ibusPersonAccount.icdoPersonAccount.person_account_id,
                //        icdoEmployerPayrollDetail.pay_period_last_date.AddMonths(-1).Month,
                //        icdoEmployerPayrollDetail.pay_period_last_date.AddMonths(-1).Year
                //    }, null, null);

                //foreach (DataRow ldrRow in ldtbGetPayrollDetail.Rows)
                //{
                //    //prod pir 4302
                //    int lintEmpDtlID = ldrRow["person_employment_dtl_id"] == DBNull.Value ? 0 : Convert.ToInt32(ldrRow["person_employment_dtl_id"]);
                //    if (!Convert.IsDBNull(ldrRow["SALARY_AMOUNT"]) &&
                //        lobjEmployment.icolPersonEmploymentDetail.Where(o => o.icdoPersonEmploymentDetail.person_employment_dtl_id == lintEmpDtlID).Any())
                //    {
                //        ldecSalaryOldAmount += (decimal)ldrRow["SALARY_AMOUNT"];
                //    }
                //}
                //PIR 14008 - commented - Salary variance logic change
                DataTable ldtbOldSalaryAmounts = busBase.Select("cdoEmployerPayrollDetail.LoadOldSalaryAmounts",
                                                  new object[3] 
                                                  {
                                                      ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                      lobjEmployment.icdoPersonEmployment.org_id,
                                                      icdoEmployerPayrollDetail.pay_period_last_date
                                                  }
                    );
                if (ldtbOldSalaryAmounts.IsNotNull() && ldtbOldSalaryAmounts.Rows.Count == 2)
                {
                    ldecSalaryOldAmount = Convert.ToDecimal(ldtbOldSalaryAmounts.Rows[0]["OLD_SALARY_AMOUNT"]);
                }
            }
            _idecOldSalaryValue = ldecSalaryOldAmount;
        }

        /// <summary>
        /// sub method to check is employee is seasonal or not
        /// </summary>
        /// <returns></returns>
        public bool CheckIsHourlyEmployee()
        {
            bool lblnResult = false;
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();

            if ((ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.hourly_value != null) &&
                (ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.hourly_value.ToLower() == busConstant.Flag_Yes_Value.ToLower()))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        /// <summary>
        /// BR - 103
        /// check if reporting date is after than (date of death plus one month)
        /// for retirement only
        /// </summary>
        /// <returns></returns>
        public bool CheckIfMemberDateOfDeath()
        {
            if (ibusPerson == null)
            {
                LoadPerson();
            }
            if (ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
            {
                if (icdoEmployerPayrollDetail.pay_period_date > ibusPerson.icdoPerson.date_of_death.AddMonths(1))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// BR - 106
        /// check if sum of all contribution is less than negative adjustment
        /// </summary>
        /// <returns></returns>
        public bool CheckIfNegativeAdjustmentGreaterThanTotalContrib()
        {
            if (icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();

                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                {
                    decimal ldecTotalSalary = 0.00M;
                    decimal ldecTotalEE = 0.00M;
                    decimal ldecTotalER = 0.00M;
                    decimal ldecTotalEEEmpPickup = 0.00M;
                    decimal ldecTotalEEPreTax = 0.00M;
                    decimal ldecTotalEERHIC = 0.00M;
                    decimal ldecTotalERRHIC = 0.00M;
                    //PIR 25920 DC 2025
                    decimal ldecTotalEEPreTaxAddl = 0.00M;
                    decimal ldecTotalEEPostTaxAddl = 0.00M;
                    decimal ldecTotalEEPreTaxMatch = 0.00M;
                    decimal ldecTotalADEC = 0.00M;

                    int lintMonth = icdoEmployerPayrollDetail.pay_period_date.Month;
                    int lintyear = icdoEmployerPayrollDetail.pay_period_date.Year;

                    DataTable ldtbRetirementContribution = Select<cdoPersonAccountRetirementContribution>(
                                     new string[3] { "person_account_id", "pay_period_month", "pay_period_year" },
                                     new object[3] { ibusPersonAccount.icdoPersonAccount.person_account_id, lintMonth, lintyear }, null, null);
                    if (ldtbRetirementContribution.Rows.Count > 0)
                    {
                        foreach (DataRow dr in ldtbRetirementContribution.Rows)
                        {
                            if (!String.IsNullOrEmpty(dr["salary_amount"].ToString()))
                            {
                                ldecTotalSalary += Convert.ToDecimal(dr["salary_amount"]);
                            }
                            if (!String.IsNullOrEmpty(dr["pre_tax_er_amount"].ToString()))
                            {
                                ldecTotalER += Convert.ToDecimal(dr["pre_tax_er_amount"]);
                            }
                            if (!String.IsNullOrEmpty(dr["ER_VESTED_AMOUNT"].ToString()))
                            {
                                ldecTotalER += Convert.ToDecimal(dr["ER_VESTED_AMOUNT"]);
                            }
                            if (!String.IsNullOrEmpty(dr["post_tax_ee_amount"].ToString()))
                            {
                                ldecTotalEE += Convert.ToDecimal(dr["post_tax_ee_amount"]);
                            }
                            if (!String.IsNullOrEmpty(dr["pre_tax_ee_amount"].ToString()))
                            {
                                ldecTotalEEPreTax += Convert.ToDecimal(dr["pre_tax_ee_amount"]);
                            }
                            if (!String.IsNullOrEmpty(dr["ee_rhic_amount"].ToString()))
                            {
                                ldecTotalEERHIC += Convert.ToDecimal(dr["ee_rhic_amount"]);
                            }
                            if (!String.IsNullOrEmpty(dr["er_rhic_amount"].ToString()))
                            {
                                ldecTotalERRHIC += Convert.ToDecimal(dr["er_rhic_amount"]);
                            }
                            if (!String.IsNullOrEmpty(dr["ee_er_pickup_amount"].ToString()))
                            {
                                ldecTotalEEEmpPickup += Convert.ToDecimal(dr["ee_er_pickup_amount"]);
                            }
                            //PIR 25920 DC 2025
                            if (!String.IsNullOrEmpty(dr["EE_PRETAX_ADDL_AMOUNT"].ToString()))
                            {
                                ldecTotalEEPreTaxAddl+= Convert.ToDecimal(dr["EE_PRETAX_ADDL_AMOUNT"]);
                            }
                            if (!String.IsNullOrEmpty(dr["EE_POST_TAX_ADDL_AMOUNT"].ToString()))
                            {
                                ldecTotalEEPostTaxAddl += Convert.ToDecimal(dr["EE_POST_TAX_ADDL_AMOUNT"]);
                            }
                            if (!String.IsNullOrEmpty(dr["ER_PRETAX_MATCH_AMOUNT"].ToString()))
                            {
                                ldecTotalEEPreTaxMatch += Convert.ToDecimal(dr["ER_PRETAX_MATCH_AMOUNT"]);
                            }
                            if (!String.IsNullOrEmpty(dr["ADEC_AMOUNT"].ToString()))
                            {
                                ldecTotalADEC += Convert.ToDecimal(dr["ADEC_AMOUNT"]);
                            }
                        }
                    }

                    if ((ldecTotalSalary < icdoEmployerPayrollDetail.eligible_wages)
                        || (ldecTotalERRHIC < icdoEmployerPayrollDetail.rhic_er_contribution_reported)
                        || (ldecTotalER < icdoEmployerPayrollDetail.er_contribution_reported)
                        || (ldecTotalEERHIC < icdoEmployerPayrollDetail.rhic_ee_contribution_reported)
                        || (ldecTotalEEPreTax < icdoEmployerPayrollDetail.ee_pre_tax_reported)
                        || (ldecTotalEEEmpPickup < icdoEmployerPayrollDetail.ee_employer_pickup_reported)
                        || (ldecTotalEE < icdoEmployerPayrollDetail.ee_contribution_reported)
                        || (ldecTotalEEPreTaxAddl < icdoEmployerPayrollDetail.ee_pretax_addl_reported)
                        || (ldecTotalEEPostTaxAddl < icdoEmployerPayrollDetail.ee_post_tax_addl_reported)
                        || (ldecTotalEEPreTaxMatch < icdoEmployerPayrollDetail.er_pretax_match_reported)
                        || (ldecTotalADEC < icdoEmployerPayrollDetail.adec_reported))
                    {
                        return false;
                    }
                }
                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    if ((icdoEmployerPayrollDetail.provider_id1 != 0) && (icdoEmployerPayrollDetail.contribution_amount1 != 0.00M))
                    {
                        if (icdoEmployerPayrollDetail.contribution_amount1 > GetpayPeriodContributionAmtForProvider(icdoEmployerPayrollDetail.provider_id1))
                            return false;
                    }
                    if ((icdoEmployerPayrollDetail.provider_id2 != 0) && (icdoEmployerPayrollDetail.contribution_amount2 != 0.00M))
                    {
                        if (icdoEmployerPayrollDetail.contribution_amount2 > GetpayPeriodContributionAmtForProvider(icdoEmployerPayrollDetail.provider_id2))
                            return false;
                    }
                    if ((icdoEmployerPayrollDetail.provider_id3 != 0) && (icdoEmployerPayrollDetail.contribution_amount3 != 0.00M))
                    {
                        if (icdoEmployerPayrollDetail.contribution_amount3 > GetpayPeriodContributionAmtForProvider(icdoEmployerPayrollDetail.provider_id3))
                            return false;
                    }
                    if ((icdoEmployerPayrollDetail.provider_id4 != 0) && (icdoEmployerPayrollDetail.contribution_amount4 != 0.00M))
                    {
                        if (icdoEmployerPayrollDetail.contribution_amount4 > GetpayPeriodContributionAmtForProvider(icdoEmployerPayrollDetail.provider_id4))
                            return false;
                    }
                    if ((icdoEmployerPayrollDetail.provider_id5 != 0) && (icdoEmployerPayrollDetail.contribution_amount5 != 0.00M))
                    {
                        if (icdoEmployerPayrollDetail.contribution_amount5 > GetpayPeriodContributionAmtForProvider(icdoEmployerPayrollDetail.provider_id5))
                            return false;
                    }
                    if ((icdoEmployerPayrollDetail.provider_id6 != 0) && (icdoEmployerPayrollDetail.contribution_amount6 != 0.00M))
                    {
                        if (icdoEmployerPayrollDetail.contribution_amount6 > GetpayPeriodContributionAmtForProvider(icdoEmployerPayrollDetail.provider_id6))
                            return false;
                    }
                    if ((icdoEmployerPayrollDetail.provider_id7 != 0) && (icdoEmployerPayrollDetail.contribution_amount7 != 0.00M))
                    {
                        if (icdoEmployerPayrollDetail.contribution_amount7 > GetpayPeriodContributionAmtForProvider(icdoEmployerPayrollDetail.provider_id7))
                            return false;
                    }
                }
            }
            return true;
        }

        private decimal GetpayPeriodContributionAmtForProvider(int aintProviderOrgId)
        {
            decimal ldecPayPeriodContributionAmt = 0.00M;
            DataTable ldtbDefCompContribution = Select<cdoPersonAccountDeferredCompContribution>(
                            new string[4] { "person_account_id", "pay_period_start_date", "pay_period_end_date", "provider_org_id" },
                            new object[4] { ibusPersonAccount.icdoPersonAccount.person_account_id, icdoEmployerPayrollDetail.pay_period_start_date, icdoEmployerPayrollDetail.pay_period_end_date, aintProviderOrgId }, null, null);
            if (ldtbDefCompContribution.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbDefCompContribution.Rows)
                {
                    if (!String.IsNullOrEmpty(dr["pay_period_contribution_amount"].ToString()))
                    {
                        ldecPayPeriodContributionAmt += Convert.ToDecimal(dr["pay_period_contribution_amount"]);
                    }
                }
            }
            return ldecPayPeriodContributionAmt;
        }

        /// <summary>
        /// BR - 107
        /// Check if contribution rates not exists for effective date
        /// </summary>
        /// <returns></returns>
        public bool CheckContributionRatesExists()
        {
            busPlanRetirementRate lobjPlanRetirement = new busPlanRetirementRate(); //PIR-9784 
            if (String.IsNullOrEmpty(member_type))
                LoadMemberType();
            if (icdoEmployerPayrollDetail.plan_id != 0)
            {
                // PIR 9784 - For Record Type Bonus use pay_period_last_date_for_bonus
                if (icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus)
                    lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(member_type, icdoEmployerPayrollDetail.pay_period_last_date_for_bonus, icdoEmployerPayrollDetail.plan_id);
                else
                    lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(member_type, icdoEmployerPayrollDetail.pay_period_last_date, icdoEmployerPayrollDetail.plan_id);
                if (lobjPlanRetirement.icdoPlanRetirementRate.plan_rate_id == 0)
                {
                    //pir 6649 - PIR 6649 changes commented as part of PIR 14291 fix - undone the PIR 6649 logic
                    //if (ibusRetirementContributionRegular != null && ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution != null &&
                    //    ibusRetirementContributionRegular.icdoPersonAccountRetirementContribution.retirement_contribution_id > 0)
                    //{
                    //    return true;
                    //}
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// BR - 034 - 110
        /// for retirement only
        /// to check if contribution date is before start date of member account 
        /// </summary>
        /// <returns></returns>
        public bool CheckContributionDateIsGreaterThanMemberEnrolment()
        {
            if ((icdoEmployerPayrollDetail.person_id != 0) && (icdoEmployerPayrollDetail.plan_id != 0))
            {
                busPersonAccount lbusPersonAccount = new busPersonAccount();
                if (ibusPerson.icolPersonAccount.IsNullOrEmpty())
                    ibusPerson.LoadPersonAccount();
                //if (ibusPersonAccount.IsNull())
                //    ibusPerson.LoadPersonAccount();
                lbusPersonAccount = ibusPerson.icolPersonAccount.Where(i => i.icdoPersonAccount.plan_id == icdoEmployerPayrollDetail.plan_id).FirstOrDefault();
                //LoadPersonAccount();

                if (lbusPersonAccount.IsNotNull() && lbusPersonAccount.icdoPersonAccount.person_account_id != 0)
                {
                    //prod pir 4096 : shoud allow to enter payroll header for the month of hire
                    if (lbusPersonAccount.icdoPersonAccount.start_date.GetFirstDayofCurrentMonth() > icdoEmployerPayrollDetail.pay_period_date.GetFirstDayofCurrentMonth())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// BR - 120
        /// for def Comp
        /// Check reported Amount is greater than Limit by IRS
        /// </summary>
        /// <returns></returns>
        public bool CheckReportedAmtGreaterThanlimitAmt()
        {
            //prod pir 4366 : need to do the below validation only for Def. comp plan 
            // get date span using Pay check date
            if (_icdoEmployerPayrollDetail.pay_check_date != DateTime.MinValue && icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdDeferredCompensation)
            {
                DateTime ldtStartDate = Convert.ToDateTime(("01/01/" + _icdoEmployerPayrollDetail.pay_check_date.Year));
                DateTime ldtEndDate = _icdoEmployerPayrollDetail.pay_check_date;

                //Get Contribution For this date span from deferred comp contribution table
                decimal ldecTotalContribution = 0.00M;

                if (ibusPersonAccount == null)
                    LoadPersonAccount();

                if (_ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    decimal ldecContributionAmt = Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoEmployerPayrollDetail.GetSumOfDefferedCompContribution", new object[3] { ldtStartDate, ldtEndDate, _ibusPersonAccount.icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));

                    //Get Contribution for this date span from payroll detail 
                    ldecTotalContribution = ldecContributionAmt + _icdoEmployerPayrollDetail.contribution_amount1 +
                                                                    _icdoEmployerPayrollDetail.contribution_amount2 +
                                                                    _icdoEmployerPayrollDetail.contribution_amount3 +
                                                                    _icdoEmployerPayrollDetail.contribution_amount4 +
                                                                    _icdoEmployerPayrollDetail.contribution_amount5 +
                                                                    _icdoEmployerPayrollDetail.contribution_amount6 +
                                                                    _icdoEmployerPayrollDetail.contribution_amount7;
                    #region Code Commented
                    /*
                    DataTable ldtbCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(64);
                    if (ldtbCodeValue.Rows.Count > 0)
                    {
                        foreach (DataRow dr in ldtbCodeValue.Rows)
                        {
                            string ldtUpperLimitDate = dr["data3"].ToString();
                            string ldtLowerLimitDate = dr["data2"].ToString();
                            decimal ReportedAmountLimit = Convert.ToDecimal(dr["data1"].ToString());

                            if ((!String.IsNullOrEmpty(ldtLowerLimitDate))
                                && (!String.IsNullOrEmpty(ldtUpperLimitDate)) &&
                                (icdoEmployerPayrollDetail.pay_check_date != DateTime.MinValue))
                            {
                                if (busGlobalFunctions.CheckDateOverlapping(icdoEmployerPayrollDetail.pay_check_date, Convert.ToDateTime(ldtLowerLimitDate), Convert.ToDateTime(ldtUpperLimitDate)))
                                {
                                    if (ReportedAmountLimit != 0.00M)
                                    {
                                        if (ReportedAmountLimit < ldecTotalContribution)
                                            return false;
                                    }
                                }
                            }
                        }
                    }*/
                    #endregion

                    if (Load457LimitAmount() < ldecTotalContribution)
                        return false;
                }
            }
            return true;
        }

        /// <summary>
        /// BR - 120 UAT PIR - 533
        /// for def Comp
        /// Check reported Amount is greater than Limit by IRS
        /// </summary>
        /// <returns></returns>
        public bool IsAmountWithinIRSLimit()
        {
            //If Pay Check date is  not set, this method should not throw an error.
            bool lblnResult = true;
            // get date span using Pay check date
            //prod pir 4366
            //need to check irs limit amount only for def. comp plan
            if (_icdoEmployerPayrollDetail.pay_check_date != DateTime.MinValue && icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdDeferredCompensation)
            {
                lblnResult = false;

                #region Commented Code
                /*
                DateTime ldtStartDate = Convert.ToDateTime(("01/01/" + _icdoEmployerPayrollDetail.pay_check_date.Year));
                DateTime ldtEndDate = _icdoEmployerPayrollDetail.pay_check_date;

                DataTable ldtbCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(64);
                if (ldtbCodeValue.Rows.Count > 0)
                {
                    foreach (DataRow dr in ldtbCodeValue.Rows)
                    {
                        string ldtUpperLimitDate = dr["data3"].ToString();
                        string ldtLowerLimitDate = dr["data2"].ToString();
                        decimal ReportedAmountLimit = Convert.ToDecimal(dr["data1"].ToString());

                        if ((!String.IsNullOrEmpty(ldtLowerLimitDate))
                            && (!String.IsNullOrEmpty(ldtUpperLimitDate)) &&
                            (icdoEmployerPayrollDetail.pay_check_date != DateTime.MinValue))
                        {
                            if (busGlobalFunctions.CheckDateOverlapping(icdoEmployerPayrollDetail.pay_check_date, Convert.ToDateTime(ldtLowerLimitDate), Convert.ToDateTime(ldtUpperLimitDate)))
                            {
                                lblnResult = true;
                                break;
                            }
                        }
                    }
                }*/
                #endregion

                if (Load457RefData().Rows.Count > 0)
                    lblnResult = true;
            }
            return lblnResult;
        }

        //Check Member Enrolment valid
        public bool CheckEnrollmentRequired()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();

            if (ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                ibusPersonAccount.ibusPersonEmploymentDetail.LoadPersonEmployment();

            if (ibusPersonAccount.ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.person_employment_id > 0)
            {
                return true;
            }
            return false;
        }

        //Check Enrollment Exists
        public bool CheckEnrolmentExists()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount(); 
            if (ibusPersonAccount.ibusPlan.IsNull())
                ibusPersonAccount.LoadPlan();
            //  // PIR 24585 - Error Message 4594 should when Member is not enrolled in any Retirement plan
            if (((ibusPersonAccount.icdoPersonAccount.person_account_id == 0) && (icdoEmployerPayrollDetail.ssn == ibusPerson.icdoPerson.ssn) && (CheckContributionDateIsGreaterThanMemberEnrolment() == true)) 
                && (ibusPersonAccount.ibusPlan.icdoPlan.benefit_type_value != busConstant.BenefitAppealTypeRetirement) && (CheckMemberIsNotEnrolledInRetirementPlan() == false)) 
            {
                return false;
            }
            return true;
        }

        //UAT PIR 1105 - Throw an Warning if the contribution reported for LOA Period
        public bool IsContributionPostedinLOAPeriod()
        {
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();

                if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                    LoadPersonEmploymentDetail();

                if (ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOA ||
                   ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusLOAM ||
                   ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusFMLA)
                {
                    return true;
                }
            }
            return false;
        }

        //UAT PIR 1105 - Throw an Warning if the contribution reported for LOA Period
        public bool IsContributionPostedinNonContributingPeriod()
        {
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();

                if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                    LoadPersonEmploymentDetail();

                if (ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.status_value == busConstant.EmploymentStatusNonContributing)
                {
                    return true;
                }
            }
            return false;
        }

        public void UpdateCalculatedFields()
        {
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                UpdateCalculatedAmountForRetirement(string.Empty);
            }
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                UpdateAmtFromEnrolmentForDefComp();
            }
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                if (icdoEmployerPayrollDetail.plan_id != 0)
                {
                    if (ibusEmployerPayrollHeader.ibusOrganization == null)
                        ibusEmployerPayrollHeader.LoadOrganization();

                    if (ibusPersonAccount == null)
                        LoadPersonAccount();

                    decimal ldecPremiumAmount = 0.00M;
                    decimal ldecFeeAmt = 0.00M;
                    decimal ldecBuydownAmt = 0.00M;
                    decimal ldecMedicarePartDAmt = 0.00M;
                    decimal ldecRHICAmt = 0.00M;
                    /* UAT PIR 476, Including other and JS RHIC Amount */
                    decimal ldecOthrRHICAmt = 0.00M;
                    decimal ldecJSRHICAmt = 0.00M;
                    //uat pir 1429  : post ghdv_history_id
                    int lintGHDVHistoryID = 0;
                    string lstrGroupNumber = string.Empty;
                    //prod pir 6076
                    string lstrCoverageCodeValue = string.Empty, lstrRateStructureCode = string.Empty;
                    //pir 7705
                    decimal ldecHSAAmt = 0.00M;
                    decimal ldecHSAVendorAmt = 0.0M;
                    //If it is Negative Adjustments, Bring it from Contribution (Sum of All for that Pay Period)
                    if (icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
                    {
                        if (ibusPersonAccount == null)
                            LoadPersonAccount();
                        if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                        {
                            if (ibusPersonAccount.iclbInsuranceContributionAll == null)
                                ibusPersonAccount.LoadInsuranceContributionAll();

                            var lclbFilterdContribution = ibusPersonAccount.iclbInsuranceContributionAll.Where(
                            i =>
                            i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting &&
                            i.icdoPersonAccountInsuranceContribution.effective_date == icdoEmployerPayrollDetail.pay_period_date);

                            if (lclbFilterdContribution != null)
                            {
                                ldecPremiumAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.due_premium_amount);
                                ldecFeeAmt = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.group_health_fee_amt);
                                ldecBuydownAmt = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.buydown_amount);
                                ldecMedicarePartDAmt = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.medicare_part_d_amt); //PIR 14271

                                //PROD PIR 5416, 5284 : Contribution premium amount already included the fee amount. so, we must exlcude the fee amount here since we are again adding
                                //the fee amount in end of this method
                                ldecPremiumAmount = ldecPremiumAmount - ldecFeeAmt;

                                //Contribution premium amount already excluded the buydown amount. so, we must exclude the buydown amount here since we are again subtracting
                                //the buydown amount in end of this method
                                ldecPremiumAmount = ldecPremiumAmount + ldecBuydownAmt - ldecMedicarePartDAmt; //PIR 14271

                                ldecRHICAmt = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.rhic_benefit_amount);
                                ldecOthrRHICAmt = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.othr_rhic_amount);
                                ldecJSRHICAmt = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.js_rhic_amount);
                            }
                        }
                    }
                    else
                    {
                        ldecPremiumAmount =
                       busRateHelper.GetInsurancePremiumAmount(ibusEmployerPayrollHeader.ibusOrganization,
                                                               icdoEmployerPayrollDetail.pay_period_last_date,
                                                               ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                               ibusPersonAccount.icdoPersonAccount.plan_id,
                                                               ref ldecFeeAmt, ref ldecBuydownAmt, ref ldecMedicarePartDAmt,ref ldecRHICAmt, ref ldecOthrRHICAmt, ref ldecJSRHICAmt, ref ldecHSAAmt, ref ldecHSAVendorAmt,
                                                               ibusPersonAccountLife, ibusPersonAccountGhdv,
                                                               ibusPersonAccountLtc, ibusPersonAccountEAP, ibusPersonAccountMedicare,   //PIR 15434
                                                               iobjPassInfo, ibusDBCacheData, ref lintGHDVHistoryID, ref lstrGroupNumber,
                                                               ref lstrCoverageCodeValue, ref lstrRateStructureCode);
                    }

                    //uat pir 1344
                    //--Start--//
                    decimal ldecEmprSharePremium = 0.00M, ldecEmprShareFee = 0.00M, ldecEmprShareRHICAmt = 0.00M, ldecEmprShareOtherRHICAmt = 0.00M, ldecEmprShareJSRHICAmt = 0.00M;
                    decimal ldecEmprShareBuydown = 0.00M, ldecEmprShareMedicarePartDAmt = 0.00M;
                    if (ibusPlan == null)
                        LoadPlan();
                    if (icdoEmployerPayrollDetail.rgroup_retiree_flag == busConstant.Flag_Yes && ibusPlan.IsGHDVPlan())
                    {
                        if (ibusPersonAccountGhdv != null)
                        {
                            if (ibusPersonAccountGhdv.ibusPaymentElection == null)
                                ibusPersonAccountGhdv.LoadPaymentElection();
                            if (!string.IsNullOrEmpty(ibusPersonAccountGhdv.icdoPersonAccountGhdv.cobra_type_value) &&
                                ibusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0 &&
                                ibusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share > 0 &&
                                ibusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share < 100)
                            {
                                ldecEmprSharePremium = Math.Round(ldecPremiumAmount *
                                                           ibusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                                ldecEmprShareFee = Math.Round(ldecFeeAmt *
                                    ibusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                                ldecEmprShareBuydown = Math.Round(ldecBuydownAmt *
                                    ibusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                                ldecEmprShareRHICAmt = Math.Round(ldecRHICAmt *
                                    ibusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                                ldecEmprShareOtherRHICAmt = Math.Round(ldecOthrRHICAmt *
                                    ibusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                                ldecEmprShareJSRHICAmt = Math.Round(ldecJSRHICAmt *
                                    ibusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);
                                ldecEmprShareMedicarePartDAmt = Math.Round(ldecMedicarePartDAmt *
                                    ibusPersonAccountGhdv.ibusPaymentElection.icdoPersonAccountPaymentElection.cobra_empr_share / 100, 2, MidpointRounding.AwayFromZero);//PIR 14271

                                ldecPremiumAmount = ldecEmprSharePremium;
                                ldecFeeAmt = ldecEmprShareFee;
                                ldecBuydownAmt = ldecEmprShareBuydown;
                                ldecRHICAmt = ldecEmprShareRHICAmt;
                                ldecOthrRHICAmt = ldecEmprShareOtherRHICAmt;
                                ldecJSRHICAmt = ldecEmprShareJSRHICAmt;
                                ldecMedicarePartDAmt = ldecEmprShareMedicarePartDAmt;//PIR 14271
                            }
                        }
                    }
                    //--End--//

                    if (icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupLife)
                    {
                        if (icdoEmployerPayrollDetail.rgroup_retiree_flag == busConstant.Flag_Yes)
                        {
                            //For R Group Life Plan, Include only Org To Bill Amount    
                            if (ibusPersonAccountLife != null)
                            {
                                if (ibusPersonAccountLife.ibusPaymentElection == null)
                                    ibusPersonAccountLife.LoadPaymentElection();
                                if (ibusPersonAccountLife.ibusPaymentElection != null)
                                {
                                    decimal ldecFinalPremiumAmount = 0.00M;
                                    if (ibusPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0)
                                    {
                                        ldecFinalPremiumAmount += ibusPersonAccountLife.idecLifeBasicPremiumAmt;
                                    }

                                    if (ibusPersonAccountLife.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id > 0)
                                    {
                                        ldecFinalPremiumAmount += ibusPersonAccountLife.idecLifeSupplementalPremiumAmount;
                                    }
                                    ldecPremiumAmount = ldecFinalPremiumAmount;
                                }
                            }
                        }
                    }

                    if (icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        //Low Income Credit Amount should be populated from Ref table. 
                        Decimal ldecLowIncomeCreditAmount = 0;
                        DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                        var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == icdoEmployerPayrollDetail.idecLow_Income_Credit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                        foreach (DataRow dr in lenumList)
                        {
                            if (Convert.ToDateTime(dr["effective_date"]).Date <= DateTime.Now)
                            {
                                ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                                break;
                            }
                        }

                        ldecPremiumAmount = ldecPremiumAmount - ldecLowIncomeCreditAmount;//PIR 18807
                    }

                    /* UAT PIR 476 ends here */
                    icdoEmployerPayrollDetail.premium_amount_from_enrollment = ldecPremiumAmount + ldecFeeAmt + ldecHSAAmt + ldecMedicarePartDAmt; // PROD PIR 7705 // PIR 14271
                    icdoEmployerPayrollDetail.premium_amount_from_enrollment -= ldecBuydownAmt;

                    //prod pir 6077
                    if (!iblnOnlineCreation)
                    {
                        //uat pir 1429 : post ghdv_history_id and group number
                        //prod pir 6076 & 6077 - Removal of person account ghdv history id
                        //icdoEmployerPayrollDetail.person_account_ghdv_history_id = lintGHDVHistoryID;
                        if (string.IsNullOrEmpty(lstrGroupNumber))
                        {
                            if (ibusEmployerPayrollHeader == null)
                                LoadPayrollHeader();
                            if (ibusEmployerPayrollHeader.ibusOrganization == null)
                                ibusEmployerPayrollHeader.LoadOrganization();
                            icdoEmployerPayrollDetail.group_number = ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_code;
                        }
                        else
                        {
                            icdoEmployerPayrollDetail.group_number = lstrGroupNumber;
                        }
                        //prod pir 6076
                        icdoEmployerPayrollDetail.coverage_code = lstrCoverageCodeValue;
                        icdoEmployerPayrollDetail.rate_structure_code = lstrRateStructureCode;
                    }
                }
            }
        }

        public void UpdateCalculatedAmountForRetirement(string astrMemberType)
        {
            if (String.IsNullOrEmpty(astrMemberType))
            {
                LoadMemberType();
            }
            else
            {
                member_type = astrMemberType;
            }

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();
            //PIR 15616 - Load Member Type Based on Header payroll paid date for bonus record
            if (ibusEmployerPayrollHeader.IsNull()) LoadPayrollHeader();

            if (ibusPersonAccount.ibusPersonEmploymentDetail != null)
            {
                busPlanRetirementRate lobjPlanRetirement = new busPlanRetirementRate();
                if ((icdoEmployerPayrollDetail.plan_id != 0) && (member_type != null))
                {
                    // PIR 9647 - For Record Type Bonus use pay_period_last_date_for_bonus
                    if (icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus || icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)
                    {                        
                        lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(member_type, ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date, icdoEmployerPayrollDetail.plan_id);
                    }
                    else
                        lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(member_type, icdoEmployerPayrollDetail.pay_period_last_date, icdoEmployerPayrollDetail.plan_id);
                    if (lobjPlanRetirement.icdoPlanRetirementRate.plan_rate_id != 0)
                    {
                        //get amount as per rates
                        decimal ldecSalary = _icdoEmployerPayrollDetail.eligible_wages;

                        _icdoEmployerPayrollDetail.ee_pre_tax_calculated = ldecSalary * lobjPlanRetirement.icdoPlanRetirementRate.ee_pre_tax / 100;
                        _icdoEmployerPayrollDetail.ee_contribution_calculated = ldecSalary * lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax / 100;
                        _icdoEmployerPayrollDetail.ee_employer_pickup_calculated = ldecSalary * lobjPlanRetirement.icdoPlanRetirementRate.ee_emp_pickup / 100;
                        _icdoEmployerPayrollDetail.rhic_ee_contribution_calculated = ldecSalary * lobjPlanRetirement.icdoPlanRetirementRate.ee_rhic / 100;
                        _icdoEmployerPayrollDetail.er_contribution_calculated = ldecSalary * lobjPlanRetirement.icdoPlanRetirementRate.er_post_tax / 100;
                        _icdoEmployerPayrollDetail.rhic_er_contribution_calculated = ldecSalary * lobjPlanRetirement.icdoPlanRetirementRate.er_rhic / 100;
                        //PIR 25920 New Plan DC 2025
                        //new calculated fields should populate for all Ret plans if values are > 0 (Maik mail dated 4/23/24) 
                        //{
                        //    _icdoEmployerPayrollDetail.ee_pretax_addl_calculated = ldecSalary * lobjPlanRetirement.icdoPlanRetirementRate.ee_pretax_addl / 100;
                        //    _icdoEmployerPayrollDetail.ee_post_tax_addl_calculated = ldecSalary * lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax_addl / 100;
                        //    _icdoEmployerPayrollDetail.er_pretax_match_calculated = ldecSalary * lobjPlanRetirement.icdoPlanRetirementRate.er_pretax_match / 100;
                        //    _icdoEmployerPayrollDetail.adec_calculated = ldecSalary * lobjPlanRetirement.icdoPlanRetirementRate.adec / 100;
                        //}
                        {
                            int? lintNullableaddl_ee_contribution_percent = 0;
                            int lintaddl_ee_contribution_percent = 0;
                            if (ibusPersonAccount.ibusPersonAccountRetirement == null)
                                ibusPersonAccount.LoadPersonAccountRetirement();
                            //if (ibusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate == null)
                            //    ibusPersonAccount.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date);
                            if (icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus || icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)
                            {
                                ibusPersonAccount.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date);
                            }
                            else
                                ibusPersonAccount.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(icdoEmployerPayrollDetail.pay_period_date);

                            lintNullableaddl_ee_contribution_percent = ibusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate.icdoPersonAccountRetirementHistory.addl_ee_contribution_percent;
                            lintaddl_ee_contribution_percent = Convert.ToInt32(lintNullableaddl_ee_contribution_percent);

                            _icdoEmployerPayrollDetail.ee_pretax_addl_calculated = ldecSalary * (lobjPlanRetirement.icdoPlanRetirementRate.ee_pretax_addl > 0 ? lintaddl_ee_contribution_percent : 0.00m) / 100;
                            _icdoEmployerPayrollDetail.ee_post_tax_addl_calculated = ldecSalary * (lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax_addl > 0 ? lintaddl_ee_contribution_percent  : 0.00m) / 100;
                            _icdoEmployerPayrollDetail.er_pretax_match_calculated = ldecSalary * (lobjPlanRetirement.icdoPlanRetirementRate.er_pretax_match > 0 ? lintaddl_ee_contribution_percent : 0.00m) / 100;
                            _icdoEmployerPayrollDetail.adec_calculated = lintaddl_ee_contribution_percent > 0 ? ldecSalary * lobjPlanRetirement.icdoPlanRetirementRate.adec / 100 : 0.00m;
                        }


                        //PIR 14656 - START - Add condition to methods calculating Contributions based on Eligible Salary (Employer Report, ESS template, Service Purchase)
                        if (ibusPerson.IsNull()) LoadPerson();
                        if (!string.IsNullOrEmpty(ibusPerson.icdoPerson.db_addl_contrib) 
                            && ibusPerson.icdoPerson.db_addl_contrib.ToUpper() == busConstant.Flag_Yes)
                        {
                            _icdoEmployerPayrollDetail.ee_pre_tax_calculated = (ldecSalary * (lobjPlanRetirement.icdoPlanRetirementRate.ee_pre_tax + lobjPlanRetirement.icdoPlanRetirementRate.addl_ee_pre_tax)) / 100;
                            _icdoEmployerPayrollDetail.ee_contribution_calculated = (ldecSalary * (lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax + lobjPlanRetirement.icdoPlanRetirementRate.addl_ee_post_tax)) / 100;
                            _icdoEmployerPayrollDetail.ee_employer_pickup_calculated = (ldecSalary * (lobjPlanRetirement.icdoPlanRetirementRate.ee_emp_pickup + lobjPlanRetirement.icdoPlanRetirementRate.addl_ee_emp_pickup)) / 100;
                        }
                        //PIR 14656 - END - Add condition to methods calculating Contributions based on Eligible Salary (Employer Report, ESS template, Service Purchase)

                        //PIR 2283 - as per discussion with satya,these reported amounts has to be posted with calculated amount;
                       
                       

                        if (iblnIsFromFile) //PIR 14519: Save issue
                        {
                            _icdoEmployerPayrollDetail.ee_pre_tax_reported = Math.Round(_icdoEmployerPayrollDetail.ee_pre_tax_calculated, 2, MidpointRounding.AwayFromZero);
                            _icdoEmployerPayrollDetail.ee_contribution_reported = Math.Round(_icdoEmployerPayrollDetail.ee_contribution_calculated, 2, MidpointRounding.AwayFromZero);
                            _icdoEmployerPayrollDetail.ee_employer_pickup_reported = Math.Round(_icdoEmployerPayrollDetail.ee_employer_pickup_calculated, 2, MidpointRounding.AwayFromZero);
                            _icdoEmployerPayrollDetail.rhic_ee_contribution_reported = Math.Round(_icdoEmployerPayrollDetail.rhic_ee_contribution_calculated, 2, MidpointRounding.AwayFromZero);
                            _icdoEmployerPayrollDetail.er_contribution_reported = Math.Round(_icdoEmployerPayrollDetail.er_contribution_calculated, 2, MidpointRounding.AwayFromZero);
                            _icdoEmployerPayrollDetail.rhic_er_contribution_reported = Math.Round(_icdoEmployerPayrollDetail.rhic_er_contribution_calculated, 2, MidpointRounding.AwayFromZero);
                            //PIR 25920 New Plan DC 2025
                            //new calculated fields should populate for all Ret plans if values are > 0 (Maik mail dated 4/23/24) 
                            _icdoEmployerPayrollDetail.ee_pretax_addl_reported = Math.Round(_icdoEmployerPayrollDetail.ee_pretax_addl_calculated, 2, MidpointRounding.AwayFromZero);
                            _icdoEmployerPayrollDetail.ee_post_tax_addl_reported = Math.Round(_icdoEmployerPayrollDetail.ee_post_tax_addl_calculated, 2, MidpointRounding.AwayFromZero);
                            _icdoEmployerPayrollDetail.er_pretax_match_reported = Math.Round(_icdoEmployerPayrollDetail.er_pretax_match_calculated, 2, MidpointRounding.AwayFromZero);
                            _icdoEmployerPayrollDetail.adec_reported = Math.Round(_icdoEmployerPayrollDetail.adec_calculated, 2, MidpointRounding.AwayFromZero);

                            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.total_adec_amount_reported += _icdoEmployerPayrollDetail.adec_reported;
                        }
                    }
                    else
                    {
                        //PIR 13884 - Calculated Fields Set To 0 if last name and SSN keyed in by employer not found in the system
                        UpdateCalculatedFieldsToZero();
                    }
                }
            }
            //PIR 13884 - Calculated Fields Set To 0 if last name and SSN keyed in by employer not found in the system
            if ((_icdoEmployerPayrollDetail.person_id == 0) || 
                (ibusPersonAccount.ibusPersonEmploymentDetail.IsNotNull() && ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id == 0))
            {
                UpdateCalculatedFieldsToZero();
            }
            if ((icdoEmployerPayrollDetail.plan_id != 0) && (member_type != null))
            {
                LoadRetiremntPlanRate();
            }
        }
        private void UpdateCalculatedFieldsToZero()
        {
            //if (iblnIsFromESS)
            //{
                _icdoEmployerPayrollDetail.ee_pre_tax_calculated = 0;
                _icdoEmployerPayrollDetail.ee_contribution_calculated = 0;
                _icdoEmployerPayrollDetail.ee_employer_pickup_calculated = 0;
                _icdoEmployerPayrollDetail.rhic_ee_contribution_calculated = 0;
                _icdoEmployerPayrollDetail.er_contribution_calculated = 0;
                _icdoEmployerPayrollDetail.rhic_er_contribution_calculated = 0;
                _icdoEmployerPayrollDetail.member_interest_calculated = 0;
                _icdoEmployerPayrollDetail.employer_interest_calculated = 0;
                _icdoEmployerPayrollDetail.employer_rhic_interest_calculated = 0;
            //PIR 25920 New Plan DC 2025
            //new calculated fields should populate for all Ret plans if values are > 0 (Maik mail dated 4/23/24) 
            
            {
                _icdoEmployerPayrollDetail.adec_calculated = 0;
                _icdoEmployerPayrollDetail.ee_pretax_addl_calculated = 0;
                _icdoEmployerPayrollDetail.ee_post_tax_addl_calculated = 0;
                _icdoEmployerPayrollDetail.er_pretax_match_calculated = 0;

                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                {
                    icdoEmployerPayrollDetail.er_pretax_match_calculated = CalculateEmployerMatchForDefComp();
                }
            }
            
            //}
        }
        public void UpdateAmtFromEnrolmentForDefComp()
        {
            //get pay period end date as effective date
            DateTime ldtEffectiveDate = icdoEmployerPayrollDetail.pay_period_start_date;
            //PROD PIR 4551 
            //atleast one date should be overlapping with provider start date end date
            if ((icdoEmployerPayrollDetail.provider_id1 != 0) && (icdoEmployerPayrollDetail.contribution_amount1 != 0.00M))
            {
                //Get Pledge amount for person account and Org plan Id and assign to  amount from enrolment              
                _icdoEmployerPayrollDetail.amount_from_enrollment1 =
                    GetPledgeAmount(icdoEmployerPayrollDetail.provider_id1, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider1_linked_with_member);
                if (!icdoEmployerPayrollDetail.is_provider1_linked_with_member)
                {
                    _icdoEmployerPayrollDetail.amount_from_enrollment1 =
                                       GetPledgeAmount(icdoEmployerPayrollDetail.provider_id1, icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider1_linked_with_member);
                }
            }
            if ((icdoEmployerPayrollDetail.provider_id2 != 0) && (icdoEmployerPayrollDetail.contribution_amount2 != 0.00M))
            {
                //Get Pledge amount for person account and Org plan Id and assign to  amount from enrolment              
                _icdoEmployerPayrollDetail.amount_from_enrollment2 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id2, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider2_linked_with_member);
                if (!icdoEmployerPayrollDetail.is_provider2_linked_with_member)
                {
                    _icdoEmployerPayrollDetail.amount_from_enrollment2 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id2,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider2_linked_with_member);
                }
            }
            if ((icdoEmployerPayrollDetail.provider_id3 != 0) && (icdoEmployerPayrollDetail.contribution_amount3 != 0.00M))
            {
                //Get Pledge amount for person account and Org plan Id and assign to  amount from enrolment              
                _icdoEmployerPayrollDetail.amount_from_enrollment3 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id3, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider3_linked_with_member);
                if (!icdoEmployerPayrollDetail.is_provider3_linked_with_member)
                {
                    _icdoEmployerPayrollDetail.amount_from_enrollment3 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id3,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider3_linked_with_member);
                }
            }
            if ((icdoEmployerPayrollDetail.provider_id4 != 0) && (icdoEmployerPayrollDetail.contribution_amount4 != 0.00M))
            {
                //Get Pledge amount for person account and Org plan Id and assign to  amount from enrolment              
                _icdoEmployerPayrollDetail.amount_from_enrollment4 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id4, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider4_linked_with_member);
                if (!icdoEmployerPayrollDetail.is_provider4_linked_with_member)
                {
                    _icdoEmployerPayrollDetail.amount_from_enrollment4 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id4,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider4_linked_with_member);
                }
            }
            if ((icdoEmployerPayrollDetail.provider_id5 != 0) && (icdoEmployerPayrollDetail.contribution_amount5 != 0.00M))
            {
                //Get Pledge amount for person account and Org plan Id and assign to amount from enrolment              
                _icdoEmployerPayrollDetail.amount_from_enrollment5 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id5, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider5_linked_with_member);
                if (!icdoEmployerPayrollDetail.is_provider5_linked_with_member)
                {
                    _icdoEmployerPayrollDetail.amount_from_enrollment5 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id5,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider5_linked_with_member);
                }
            }
            if ((icdoEmployerPayrollDetail.provider_id6 != 0) && (icdoEmployerPayrollDetail.contribution_amount6 != 0.00M))
            {
                //Get Pledge amount for person account and Org plan Id and assign to amount from enrolment               
                _icdoEmployerPayrollDetail.amount_from_enrollment6 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id6, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider6_linked_with_member);
                if (!icdoEmployerPayrollDetail.is_provider6_linked_with_member)
                {
                    _icdoEmployerPayrollDetail.amount_from_enrollment6 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id6,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider6_linked_with_member);
                }
            }
            if ((icdoEmployerPayrollDetail.provider_id7 != 0) && (icdoEmployerPayrollDetail.contribution_amount7 != 0.00M))
            {
                //Get Pledge amount for person account and Org plan Id and assign to amount from enrolment             
                _icdoEmployerPayrollDetail.amount_from_enrollment7 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id7, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider7_linked_with_member);
                if (!icdoEmployerPayrollDetail.is_provider7_linked_with_member)
                {
                    _icdoEmployerPayrollDetail.amount_from_enrollment7 = GetPledgeAmount(icdoEmployerPayrollDetail.provider_id7,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider7_linked_with_member);
                }
            }
			//PIR 25920 New Plan DC 2025
            if (String.IsNullOrEmpty(member_type)) LoadMemberType();           
            if(member_type.IsNotNullOrEmpty())
                icdoEmployerPayrollDetail.er_pretax_match_calculated = CalculateEmployerMatchForDefComp();
        }
        
        /// <summary>
        /// Calculate the employer match amount for def comp 
        /// return the decimal amount $ value.
        /// </summary>
        /// PIR 25920 New Plan DC 2025
        public decimal CalculateEmployerMatchForDefComp()
        {
            int? lintNullableaddl_ee_contribution_percent = 0;
            int lintaddl_ee_contribution_percent= 0;
            decimal ldecEmployerMatchCalculated = 0.00m;
            decimal ldecLeastERPreTaxMatch = 0.00m;
            DataTable ldtbPlanRetirementRate = new DataTable();
            
            
            //if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            busPersonAccount lbusPersonAccount = new busPersonAccount();
            lbusPersonAccount = LoadPersonAccountDBPlan();
            if (icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdDeferredCompensation)
            {
                if (lbusPersonAccount.IsNotNull() && lbusPersonAccount.icdoPersonAccount.IsNotNull())
                {
                    //lintaddl_ee_contribution_percent = lbusPersonAccount.icdoPersonAccount.addl_ee_contribution_percent;
                    if (lbusPersonAccount.ibusPersonAccountRetirement == null)
                        lbusPersonAccount.LoadPersonAccountRetirement();
                    if (lbusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate == null)
                        lbusPersonAccount.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(icdoEmployerPayrollDetail.pay_period_start_date);
                    lintNullableaddl_ee_contribution_percent = lbusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate.icdoPersonAccountRetirementHistory.addl_ee_contribution_percent;
                    lintaddl_ee_contribution_percent = Convert.ToInt32(lintNullableaddl_ee_contribution_percent);
                    busPlanRetirementRate lobjPlanRetirement = new busPlanRetirementRate();
                    lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(member_type, icdoEmployerPayrollDetail.pay_period_start_date, lbusPersonAccount.icdoPersonAccount.plan_id);

                    if (lobjPlanRetirement.IsNotNull())
                    {
                        ldecLeastERPreTaxMatch = lobjPlanRetirement.icdoPlanRetirementRate.er_pretax_match <
                            (lobjPlanRetirement.icdoPlanRetirementRate.ee_pretax_addl + lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax_addl) ?
                            lobjPlanRetirement.icdoPlanRetirementRate.er_pretax_match :
                            (lobjPlanRetirement.icdoPlanRetirementRate.ee_pretax_addl + lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax_addl);
                    }
                }

                ldecEmployerMatchCalculated = ldecLeastERPreTaxMatch > 0 ? _icdoEmployerPayrollDetail.eligible_wages_defcomp * (ldecLeastERPreTaxMatch - lintaddl_ee_contribution_percent) / 100 : 0.00m;

                //decEmployerMatchReported = ldecERPreTaxAmount > 0 ? _icdoEmployerPayrollDetail.eligible_wages_defcomp * (ldecEmployerMatchCalculated - lintaddl_ee_contribution_percent) / 100 : 0.00m;
                decimal ldecSumOfAmountFromEnrollment = GetSumOfAllDefCompProviderAmountFromEnrollment();
                if (ldecSumOfAmountFromEnrollment != 0)
                    ldecEmployerMatchCalculated = ldecEmployerMatchCalculated > ldecSumOfAmountFromEnrollment ? ldecSumOfAmountFromEnrollment : ldecEmployerMatchCalculated;
            }
            return ldecEmployerMatchCalculated;
        }
        /// <summary>
        /// Load the Person account for retirement plan that member has enrolled 
        /// if multiple account exists, take the first account
        /// return the retirment person account.
        /// </summary>
        /// PIR 25920 New Plan DC 2025
        public busPersonAccount LoadPersonAccountDBPlan()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            if (ibusPerson.icolPersonAccount == null)
                ibusPerson.LoadPersonAccount(false);
            
            busPersonAccount lbusPersonAccount = new busPersonAccount();
            if (ibusPerson.icolPersonAccount.IsNotNull())
            {
                ibusPerson.icolPersonAccount.ForEach(objPersonAccount => objPersonAccount.LoadPlan());
                if(ibusPerson.icolPersonAccount.Any(objPersonAccount => objPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement
                        && (objPersonAccount.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB
                            || objPersonAccount.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)       //PIR 25920  New DC plan
                        && objPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled
                    ))
                lbusPersonAccount = ibusPerson.icolPersonAccount.Where(objPersonAccount => objPersonAccount.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement
                        && (objPersonAccount.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB
                            || objPersonAccount.ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)       //PIR 25920  New DC plan
                        && objPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled
                    ).FirstOrDefault();
            }
            return lbusPersonAccount;

        }
        
        //(Header Type Retirement or Def Comp) and
        //((Retirement Plan Status Withdrawn) or
        //(Processed or Payment Complete Pre - Retirement Death) or
        //(Processed or Payment Complete Post Retirement Death))
        //PIR 26959 Validation on Payroll Reports for Deferred Comp and Retirement
        public bool LoadPersonAccountWithDrawnRetirementPlan()
        {
            if (ibusEmployerPayrollHeader.IsNull())
                LoadPayrollHeader();
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt || ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PlanBenefitTypeDeferredComp)
            {
                if (ibusPerson.IsNull())
                    LoadPerson();
                if (ibusPerson.iclbPayeeAccountsByMemberID == null)
                    ibusPerson.LoadPayeeAccountsByMemberID();
                if (ibusPlan.IsNull()) LoadPlan();
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                if (ibusPerson.iclbPayeeAccountsByMemberID.IsNotNull())
                {
                    if ((ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementWithDrawn)||
                        (ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDeffCompWithDrawn) ||
                       (ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusPreRetirementDeath ||
                        ibusPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired && 
                        ibusPerson.iclbPayeeAccountsByMemberID.Any(objPayeeAccount => (objPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath ||
                                                                                       objPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath) &&
                                                                                       (objPayeeAccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusRefundProcessed ||
                                                                                        objPayeeAccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusPaymentComplete))))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    
    
        /// <summary>
        /// Check the Person account exist for retirement plan DC 25 for that member has enrolled 
        /// if multiple account exists, take the first account
        /// return true if person account exist for Plan DC 25 on payroll period.
        /// </summary>
        /// PIR 25920 New Plan DC 2025
        public bool CheckMemberIsEnrolledInDC25()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            
            ibusPerson.LoadDC2025PersonAccount();
            if (ibusPerson.ibusDC2025PersonAccount.IsNotNull())
            {
                if (ibusPerson.ibusDC2025PersonAccount.ibusPersonAccountRetirement == null)
                    ibusPerson.ibusDC2025PersonAccount.LoadPersonAccountRetirement();
                //if (ibusPerson.ibusDC2025PersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate == null)
                ibusPerson.ibusDC2025PersonAccount.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(icdoEmployerPayrollDetail.pay_period_start_date);
                if (ibusPerson.ibusDC2025PersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC2025 &&
                    ibusPerson.ibusDC2025PersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate.icdoPersonAccountRetirementHistory.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// Check the Person account exist for retirement plan for that member has enrolled 
        /// if multiple account exists, take the first account
        /// get the optional election value, get the member type and max pretax match value
        /// return true if election value is less than pretax max 
        /// </summary>
        /// PIR 25920 New Plan DC 2025
        public bool IsEmployerMatchPlanRateAvailable
        {
            get
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();

                if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                    LoadPersonEmploymentDetail();

                return ibusPersonAccount.IsEmployerMatchAvailableWithElection(icdoEmployerPayrollDetail.pay_period_start_date);
                //decimal ldecLeastERPreTaxMatch = 0.00m;
                //string lstrMember_Type = string.Empty;
                //if (ibusPersonAccount.IsNull()) LoadPersonAccount();
                //if (ibusPersonAccount.icdoPersonAccount.person_account_id == 0) LoadPersonAccount();
                //if (ibusPersonAccount.ibusPersonEmploymentDetail.IsNull())
                //    LoadPersonEmploymentDetail(true);
                //if (ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id == 0)
                //    LoadPersonEmploymentDetail(true);
                ////if (String.IsNullOrEmpty(member_type)) LoadMemberType();
                //busPersonAccount lbusPersonAccountRetirementForMatch = new busPersonAccount();
                //lbusPersonAccountRetirementForMatch = LoadPersonAccountDBPlan();
                //if (lbusPersonAccountRetirementForMatch.IsNotNull() && lbusPersonAccountRetirementForMatch.icdoPersonAccount.IsNotNull())
                //{
                    
                //    lbusPersonAccountRetirementForMatch.icdoPersonAccount.person_employment_dtl_id =
                //        ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                //    lbusPersonAccountRetirementForMatch.LoadPersonEmploymentDetail();


                //    if (lbusPersonAccountRetirementForMatch.ibusPersonEmploymentDetail != null)
                //    {
                //        if (lbusPersonAccountRetirementForMatch.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
                //        {
                //            //PIR-9784 --  For Record Type Bonus use pay_period_last_date_for_bonus to derive the Member Type
                //            if (this.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus)
                //            {
                //                //PIR 15616 - Load Member Type Based on Header payroll paid date for bonus record
                //                if (ibusEmployerPayrollHeader.IsNull()) LoadPayrollHeader();
                //                lbusPersonAccountRetirementForMatch.ibusPersonEmploymentDetail.LoadMemberType(ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date);
                //            }
                //            else
                //                lbusPersonAccountRetirementForMatch.ibusPersonEmploymentDetail.LoadMemberType(this.icdoEmployerPayrollDetail.pay_period_start_date); //Added for PIR 8928
                //            lstrMember_Type = lbusPersonAccountRetirementForMatch.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value;
                //        }
                //    }


                //    busPlanRetirementRate lobjPlanRetirement = new busPlanRetirementRate();
                //    lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(lstrMember_Type, icdoEmployerPayrollDetail.pay_period_start_date, lbusPersonAccountRetirementForMatch.icdoPersonAccount.plan_id);

                //    if (lobjPlanRetirement.IsNotNull())
                //    {
                //        ldecLeastERPreTaxMatch = lobjPlanRetirement.icdoPlanRetirementRate.er_pretax_match <
                //            (lobjPlanRetirement.icdoPlanRetirementRate.ee_pretax_addl + lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax_addl) ?
                //            lobjPlanRetirement.icdoPlanRetirementRate.er_pretax_match :
                //            (lobjPlanRetirement.icdoPlanRetirementRate.ee_pretax_addl + lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax_addl);
                //    }
                //}
                //return ldecLeastERPreTaxMatch > 0;
            }
        }

        
        /// <summary>
        /// Check the Person Payroll already exists and wages the Employer Match  
        /// File has a wages and employer match 
        /// return true if person payroll exist for Plan DeffComp on payroll same period.
        /// </summary>
        /// PIR 25920 New Plan DC 2025
        public bool CheckMemberPayrollIsPostedInDefComp(int aintOrgID = 0)
        {
            if (ibusPerson.IsNull()) LoadPerson();

            DataTable ldtbPostedPayrollDetail = busNeoSpinBase.Select("cdoEmployerPayrollDetail.CheckCheckMemberPayrollIsPostedSamePeriod",
                                                new object[3]
                                              {
                                                  _icdoEmployerPayrollDetail.person_id,
                                                  _icdoEmployerPayrollDetail.employer_payroll_detail_id,
                                                  aintOrgID
                                              });
            if (ldtbPostedPayrollDetail.IsNotNull() && ldtbPostedPayrollDetail.Rows.Count > 0 )
            {
                if (_icdoEmployerPayrollDetail.er_pretax_match_reported != 0m && _icdoEmployerPayrollDetail.eligible_wages_defcomp != 0m && 
                    _icdoEmployerPayrollDetail.pay_period_start_date == Convert.ToDateTime(ldtbPostedPayrollDetail.Rows[0][enmEmployerPayrollDetail.pay_period_start_date.ToString()])
                    && _icdoEmployerPayrollDetail.pay_period_end_date == Convert.ToDateTime(ldtbPostedPayrollDetail.Rows[0][enmEmployerPayrollDetail.pay_period_end_date.ToString()]))
                {
                    iintPreviousPayrollProviderOrgID = Convert.ToInt32(Convert.ToString(ldtbPostedPayrollDetail.Rows[0][enmEmployerPayrollDetail.provider_id1.ToString()]));
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Method to get the RetirementContributionConfirmation report
        /// </summary>
        /// <param name="aintOrgID">Organization ID</param>
        /// <param name="adtStartDate">Start Date</param>
        /// <param name="adtEndDate">End Date</param>
        /// <returns>Report result in Dataset</returns>
        public DataSet rptRetirementContributionConfirmation(string astrOrgCodeID, DateTime adtStartDate, DateTime adtEndDate)
        {
            DataTable ldtReportResult = busBase.Select("cdoEmployerPayrollDetail.rptRetirementContributionConfirmation",
                new object[3] { adtStartDate, adtEndDate, astrOrgCodeID });
            ldtReportResult.TableName = busConstant.ReportTableName;
            DataSet ldsReportResult = new DataSet();
            ldsReportResult.Tables.Add(ldtReportResult.Copy());
            return ldsReportResult;
        }

        //BR - 057-39,40,41,45,46,36
        public void CreateAdjustmentRefund(decimal adecInterestAmount = 0.00M)
        {
            if (iclbBenefitRefundApplication == null)
                LoadBenefitRefundApplication();
            busBenefitRefundApplication lbusLatestRefundApplication = iclbBenefitRefundApplication.FirstOrDefault();
            if (lbusLatestRefundApplication.IsNotNull())
            {
                if (!lbusLatestRefundApplication.IsApplicationCancelledOrDenied())
                {
                    if (lbusLatestRefundApplication.iclbBenefitApplicationPersonAccounts == null)
                        lbusLatestRefundApplication.LoadBenefitApplicationPersonAccount();
                    if (_ibusPersonAccount == null)
                        LoadPersonAccount();
                    bool lblnContuinue = false;
                    foreach (busBenefitApplicationPersonAccount lbusBenefitApplicationPersonAccount in lbusLatestRefundApplication.iclbBenefitApplicationPersonAccounts)
                    {
                        if (lbusBenefitApplicationPersonAccount.icdoBenefitApplicationPersonAccount.person_account_id == _ibusPersonAccount.icdoPersonAccount.person_account_id)
                            lblnContuinue = true;
                    }
                    if (lblnContuinue)
                    {
                        //Setting the Addition Cont. Indicator and Validating Payee Account so that it puts the warning in validation tab.
                        if (lbusLatestRefundApplication.iclbPayeeAccount == null)
                            lbusLatestRefundApplication.LoadPayeeAccount();
                        foreach (busPayeeAccount lobjPayeeAccount in lbusLatestRefundApplication.iclbPayeeAccount)
                        {
                            if (lobjPayeeAccount.ibusPayeeAccountActiveStatus == null)
                                lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                            if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value != busConstant.PayeeAccountStatusRefundCancelled)
                            {
                                //ibusEmployerPayrollHeader.InitiateWorkflow(icdoEmployerPayrollDetail.person_id, lobjPayeeAccount.icdoPayeeAccount.payee_account_id,
                                //    busConstant.Map_Process_Remider_Refund);
                                if (lobjPayeeAccount.ibusSoftErrors == null)
                                    lobjPayeeAccount.LoadErrors();
                                lobjPayeeAccount.iblnClearSoftErrors = false;
                                lobjPayeeAccount.ibusSoftErrors.iblnClearError = false;
                                lobjPayeeAccount.iblnAddionalContributionsIndicatorFlag = true;
                                lobjPayeeAccount.ValidateSoftErrors();
                                lobjPayeeAccount.UpdateValidateStatus();
                                lobjPayeeAccount.CreateReviewStatus();
                                lobjPayeeAccount.LoadRolloverDetail();
                                foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in lobjPayeeAccount.iclbRolloverDetail)
                                {
                                    if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusProcessed &&
                                        lobjRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_option_value != busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                                    {
                                        lobjRolloverDetail.icdoPayeeAccountRolloverDetail.status_value = busConstant.PayeeAccountRolloverDetailStatusActive;
                                        lobjRolloverDetail.icdoPayeeAccountRolloverDetail.Update();
                                    }
                                }
                            }
                        }
                        lbusLatestRefundApplication.CreateAdjustmentRefund(this, icdoEmployerPayrollDetail.pay_period_date, true, adecAdditionalInterestAmount: adecInterestAmount);
                    }
                }
            }
        }
        /// <summary>
        /// PIR 25920 DC 2025 changes if there is no Provider or Amt entered then Wages and Match are not required.
        /// </summary>
        /// <param name="aobjPayrollDetail"></param>
        /// <returns></returns>
        public bool IsAnyOneContributionAvailableDefComp()
        {
            return !(icdoEmployerPayrollDetail.contribution_amount1 == 0M && icdoEmployerPayrollDetail.contribution_amount2 == 0M
                            && icdoEmployerPayrollDetail.contribution_amount3 == 0M && icdoEmployerPayrollDetail.contribution_amount4 == 0M
                            && icdoEmployerPayrollDetail.contribution_amount5 == 0M && icdoEmployerPayrollDetail.contribution_amount6 == 0M
                            && icdoEmployerPayrollDetail.contribution_amount7 == 0M);
        }
        /// <summary>
        /// PIR 25920 DC 2025 changes if there is no Provider or Amt entered then Wages and Match are not required.
        /// </summary>
        /// <param name="aobjPayrollDetail"></param>
        /// <returns></returns>
        public bool IsAnyOneProviderAvailableDefComp()
        {
            return !(string.IsNullOrEmpty(icdoEmployerPayrollDetail.provider_org_code_id1)
                                && string.IsNullOrEmpty(icdoEmployerPayrollDetail.provider_org_code_id2)
                                && string.IsNullOrEmpty(icdoEmployerPayrollDetail.provider_org_code_id3)
                                && string.IsNullOrEmpty(icdoEmployerPayrollDetail.provider_org_code_id4)
                                && string.IsNullOrEmpty(icdoEmployerPayrollDetail.provider_org_code_id5)
                                && string.IsNullOrEmpty(icdoEmployerPayrollDetail.provider_org_code_id6)
                                && string.IsNullOrEmpty(icdoEmployerPayrollDetail.provider_org_code_id7));
        }
        public bool IsAmountLessThanZero()
        {
            bool lblnResult = false;
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                if (!string.IsNullOrEmpty(_icdoEmployerPayrollDetail.record_type_value) && !(_icdoEmployerPayrollDetail.record_type_value.Equals(busConstant.PayrollDetailRecordTypeNegativeAdjustment)))
                {
                    //PIR 25920 DC 2025 
                  if (icdoEmployerPayrollDetail.eligible_wages < 0 || icdoEmployerPayrollDetail.ee_contribution_reported < 0 ||
                        icdoEmployerPayrollDetail.ee_pre_tax_reported < 0 || icdoEmployerPayrollDetail.ee_employer_pickup_reported < 0 ||
                        icdoEmployerPayrollDetail.er_contribution_reported < 0 || icdoEmployerPayrollDetail.rhic_er_contribution_reported < 0 ||
                        icdoEmployerPayrollDetail.rhic_ee_contribution_reported < 0 || icdoEmployerPayrollDetail.ee_pretax_addl_reported < 0 ||
                        icdoEmployerPayrollDetail.ee_post_tax_addl_reported < 0 || icdoEmployerPayrollDetail.er_pretax_match_reported < 0 ||
                        icdoEmployerPayrollDetail.adec_reported < 0 )
                    {
                        lblnResult = true;
                    }
                }

            }
            else if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
            {
                if (icdoEmployerPayrollDetail.contribution_amount1 < 0 || icdoEmployerPayrollDetail.contribution_amount2 < 0 ||
                    icdoEmployerPayrollDetail.contribution_amount3 < 0 || icdoEmployerPayrollDetail.contribution_amount4 < 0 ||
                    icdoEmployerPayrollDetail.contribution_amount5 < 0 || icdoEmployerPayrollDetail.contribution_amount6 < 0 ||
                    icdoEmployerPayrollDetail.contribution_amount7 < 0)
                {
                    lblnResult = true;
                }
            }
            else if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                if (icdoEmployerPayrollDetail.premium_amount < 0)
                    lblnResult = true;
            }
            else if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                if (icdoEmployerPayrollDetail.purchase_amount_reported < 0)
                    lblnResult = true;
            }
            return lblnResult;
        }

        /// <summary>
        /// Method to check whether payment class matches from employer payroll detail to service purchase header
        /// </summary>
        /// <returns></returns>
        public bool IsPaymentClassValid()
        {
            bool lblnResult = false;

            if (ibusEmployerPayrollHeader == null)
                LoadPayrollHeader();
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                if (iclbEmployerPurchaseAllocation == null)
                    LoadEmployerPurchaseAllocation();
                if (iclbEmployerPurchaseAllocation.Count > 0)
                {
                    string lstrPreTax = string.Empty;
                    if (icdoEmployerPayrollDetail.payment_class_value == busConstant.PayrollDetailPaymentClassInstallmentPreTax)
                        lstrPreTax = busConstant.Flag_Yes;
                    else if (icdoEmployerPayrollDetail.payment_class_value == busConstant.PayrollDetailPaymentClassInstallmentPostTax)
                        lstrPreTax = busConstant.Flag_No;
                    if (iclbEmployerPurchaseAllocation.Where(o => o.ibusServicePurchaseHeader.icdoServicePurchaseHeader.payroll_deduction == busConstant.Flag_Yes &&
                                                                o.ibusServicePurchaseHeader.icdoServicePurchaseHeader.pre_tax != lstrPreTax).Any())
                    {
                        lblnResult = true;
                    }
                }
            }

            return lblnResult;
        }

        public string CheckPurchaseAmountValid()
        {
            string lstrResult = string.Empty;

            if (ibusEmployerPayrollHeader == null)
                LoadPayrollHeader();
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                //if (iclbEmployerPurchaseAllocation == null)
                    LoadEmployerPurchaseAllocation();
                if (iclbEmployerPurchaseAllocation.Count > 0)
                {
                    string lstrPreTax = string.Empty;
                    if (icdoEmployerPayrollDetail.payment_class_value == busConstant.PayrollDetailPaymentClassInstallmentPreTax)
                        lstrPreTax = busConstant.Flag_Yes;
                    else if (icdoEmployerPayrollDetail.payment_class_value == busConstant.PayrollDetailPaymentClassInstallmentPostTax)
                        lstrPreTax = busConstant.Flag_No;
                    decimal ldecTotalPurchaseAmount = iclbEmployerPurchaseAllocation.Where(o => o.ibusServicePurchaseHeader.icdoServicePurchaseHeader.pre_tax == lstrPreTax &&
                                                                                            o.ibusServicePurchaseHeader.icdoServicePurchaseHeader.payroll_deduction == busConstant.Flag_Yes)
                                                                                    .Sum(o => o.ibusServicePurchaseHeader.icdoServicePurchaseHeader.expected_installment_amount);
                    if (lstrPreTax == busConstant.Flag_Yes && ldecTotalPurchaseAmount != icdoEmployerPayrollDetail.purchase_amount_reported)
                        lstrResult = "ERR";
                    else if (lstrPreTax == busConstant.Flag_No && ldecTotalPurchaseAmount != icdoEmployerPayrollDetail.purchase_amount_reported)
                        lstrResult = "WARN";
                    if(ldecTotalPurchaseAmount/2 == icdoEmployerPayrollDetail.purchase_amount_reported &&
                        iclbEmployerPurchaseAllocation.Any(pur=>pur.ibusServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment && pur.ibusServicePurchaseHeader.icdoServicePurchaseHeader.payment_frequency_value == busConstant.ServicePurchasePaymentFrequencyValueMonthly ))
                        lstrResult = string.Empty;
                }
            }

            return lstrResult;
        }

        //PIR 522 523 UAT
        public Collection<busError> iclbESSEmployerErrors { get; set; }
        public void LoadEmployerErrorsForESS()
        {
            iclbESSEmployerErrors = new Collection<busError>();
            if (ibusSoftErrors != null && ibusSoftErrors.iclbError != null)
            {
                foreach (busError lbusError in ibusSoftErrors.iclbError)
                {
                    if (lbusError.severity_value == busConstant.MessageSeverityError)
                        iclbESSEmployerErrors.Add(lbusError);
                    else if (lbusError.severity_value == busConstant.MessageSeverityWarning)
                        if (icdoEmployerPayrollDetail.allow_change_warnings == busConstant.Flag_Yes)
                            iclbESSEmployerErrors.Add(lbusError);
                }
            }
        }

        public decimal idecTotalInterest
        {
            get
            {
                return icdoEmployerPayrollDetail.member_interest_calculated +
                        icdoEmployerPayrollDetail.employer_interest_calculated +
                        icdoEmployerPayrollDetail.employer_rhic_interest_calculated;
            }
        }

        public void LoadPeoplesoftID()
        {
            if (ibusPerson == null)
                LoadPerson();
            icdoEmployerPayrollDetail.istrPeopleSoftID = ibusPerson.icdoPerson.peoplesoft_id;
        }

        /// <summary>
        /// method to load the 457 IRS limit data from ref table
        /// </summary>
        /// <returns>datatable containing irs limit</returns>
        public DataTable Load457RefData()
        {
            busPersonAccountDeferredComp lobjDefComp = new busPersonAccountDeferredComp();
            DataTable ldt457Limit = new DataTable();

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            lobjDefComp.FindPersonAccountDeferredComp(ibusPersonAccount.icdoPersonAccount.person_account_id);
            lobjDefComp.ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            lobjDefComp.ibusPersonAccount = ibusPersonAccount;
            lobjDefComp.LoadPADeffCompHistory();

            busPersonAccountDeferredCompHistory lobjDefCompHistory = lobjDefComp.icolPADeferredCompHistory
                                                                        .Where(o => busGlobalFunctions.CheckDateOverlapping(icdoEmployerPayrollDetail.pay_check_date,
                                                                            o.icdoPersonAccountDeferredCompHistory.start_date,
                                                                            o.icdoPersonAccountDeferredCompHistory.end_date))
                                                                        .FirstOrDefault();
            if (lobjDefCompHistory != null)
            {
                lobjDefCompHistory.Set457Limit(icdoEmployerPayrollDetail.pay_check_date);
                ldt457Limit = Select("cdoPersonAccount457LimitRef.GetIRSLimit",
                    new object[2] { icdoEmployerPayrollDetail.pay_check_date, lobjDefCompHistory.icdoPersonAccountDeferredCompHistory.limit_457_value });
            }

            return ldt457Limit;
        }

        /// <summary>
        /// method to get 457 limit amount
        /// </summary>
        /// <returns>irs limit</returns>
        public decimal Load457LimitAmount()
        {
            decimal ldecIRSLimit = 0.00M;

            DataTable ldt457Limit = Load457RefData();
            if (ldt457Limit.Rows.Count > 0)
                ldecIRSLimit = ldt457Limit.Rows[0]["amount"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldt457Limit.Rows[0]["amount"]);

            return ldecIRSLimit;
        }

        public bool CheckEmployerPurchaseAllocationValid()
        {
            bool lblnResult = false;
            //if (iclbEmployerPurchaseAllocation == null)
                LoadEmployerPurchaseAllocation();

            if (iclbEmployerPurchaseAllocation.Count() > 0 &&
                iclbEmployerPurchaseAllocation.Sum(o => o.icdoEmployerPurchaseAllocation.allocated_amount) == icdoEmployerPayrollDetail.purchase_amount_reported)
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool IsNegativeAdjustmentPostedForExActiveEmployee()
        {
            if (icdoEmployerPayrollDetail.record_type_value == busConstant.RecordTypeNegativeAdjustment)
            {
                if (ibusPersonAccount.IsNotNull())
                {
                    if (ibusPersonAccount.ibusPersonEmploymentDetail.IsNull())
                        return true;
                    else if (ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id == 0)
                        return true;
                }
            }
            return false;
        }

        //prod pir 6077
        public bool iblnOnlineCreation { get; set; }
        /// <summary>
        /// prod pir 6077 : life premium validation
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsLifePremiumValid()
        {
            bool lblnResult = true;

            if (icdoEmployerPayrollDetail.premium_amount != (icdoEmployerPayrollDetail.basic_premium + icdoEmployerPayrollDetail.dependent_premium +
                                                            icdoEmployerPayrollDetail.supplemental_premium + icdoEmployerPayrollDetail.spouse_premium))
            {
                lblnResult = false;
            }
            return lblnResult;
        }

        public bool IsADAndDRateEntered()
        {
            bool lblnResult = true;
            if ((icdoEmployerPayrollDetail.basic_premium > 0 && icdoEmployerPayrollDetail.ad_and_d_basic_premium_rate == 0) ||
                (icdoEmployerPayrollDetail.supplemental_premium > 0 && icdoEmployerPayrollDetail.ad_and_d_supplemental_premium_rate == 0))
            {
                lblnResult = false;
            }
            return lblnResult;
        }

        public bool IsBasicCoverageAmountValid()
        {
            bool lblnResult = true;
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonAccountLife == null)
                ibusPersonAccount.LoadPersonAccountLife();

            if (icdoEmployerPayrollDetail.life_basic_coverage_amount > 0)
            {
                if (!ibusPersonAccount.ibusPersonAccountLife.IsValidCoverageAmount(busConstant.LevelofCoverage_Basic, busConstant.LifeInsuranceTypeActiveMember,
                    icdoEmployerPayrollDetail.life_basic_coverage_amount, icdoEmployerPayrollDetail.pay_period_date))
                    lblnResult = false;
            }

            return lblnResult;
        }

        public bool IsSuppCoverageAmountValid()
        {
            bool lblnResult = true;
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonAccountLife == null)
                ibusPersonAccount.LoadPersonAccountLife();

            if (icdoEmployerPayrollDetail.life_supp_coverage_amount > 0)
            {
                if (!ibusPersonAccount.ibusPersonAccountLife.IsValidCoverageAmount(busConstant.LevelofCoverage_Supplemental, busConstant.LifeInsuranceTypeActiveMember,
                    icdoEmployerPayrollDetail.life_supp_coverage_amount, icdoEmployerPayrollDetail.pay_period_date))
                    lblnResult = false;
            }

            return lblnResult;
        }

        public bool IsSpouseSuppCoverageAmountValid()
        {
            bool lblnResult = true;
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonAccountLife == null)
                ibusPersonAccount.LoadPersonAccountLife();

            if (icdoEmployerPayrollDetail.life_spouse_supp_coverage_amount > 0)
            {
                if (!ibusPersonAccount.ibusPersonAccountLife.IsValidCoverageAmount(busConstant.LevelofCoverage_SpouseSupplemental, busConstant.LifeInsuranceTypeActiveMember,
                    icdoEmployerPayrollDetail.life_spouse_supp_coverage_amount, icdoEmployerPayrollDetail.pay_period_date))
                    lblnResult = false;
            }

            return lblnResult;
        }

        public bool IsDepSuppCoverageAmountValid()
        {
            bool lblnResult = true;
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonAccountLife == null)
                ibusPersonAccount.LoadPersonAccountLife();

            if (icdoEmployerPayrollDetail.life_dep_supp_coverage_amount > 0)
            {
                if (!ibusPersonAccount.ibusPersonAccountLife.IsValidCoverageAmount(busConstant.LevelofCoverage_DependentSupplemental, busConstant.LifeInsuranceTypeActiveMember,
                    icdoEmployerPayrollDetail.life_dep_supp_coverage_amount, icdoEmployerPayrollDetail.pay_period_date))
                    lblnResult = false;
            }

            return lblnResult;
        }

        /// <summary>
        /// method to insert person account dependent billing link
        /// </summary>
        /// <param name="adarrPADep">array of person account dependent table</param>
        public void InsertPersonAccountDependentBillingLink(IEnumerable<busPersonAccountDependent> adarrPADep, int aintPersonAccountID = 0)
        {
            //need to do only for health plan
            if (icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupHealth)
            {
                if (aintPersonAccountID > 0)
                {
                    DataTable ldtPADep = Select<cdoPersonAccountDependent>(new string[1] { "person_account_id" },
                                                                        new object[1] { aintPersonAccountID }, null, null);
                    foreach (DataRow ldrPADep in ldtPADep.Rows)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(icdoEmployerPayrollDetail.pay_period_date, Convert.ToDateTime(ldrPADep["start_date"]),
                            Convert.ToDateTime(ldrPADep["end_date"] == DBNull.Value ? "9999/12/31" : ldrPADep["end_date"]).GetLastDayofMonth()))
                        {
                            cdoPersonAccountDependentBillingLink lcdoPADepBillingLink = new cdoPersonAccountDependentBillingLink();
                            lcdoPADepBillingLink.employer_payroll_detail_id = icdoEmployerPayrollDetail.employer_payroll_detail_id;
                            lcdoPADepBillingLink.person_account_dependent_id = Convert.ToInt32(ldrPADep["person_account_dependent_id"]);
                            lcdoPADepBillingLink.Insert();
                        }
                    }
                }
                else if (adarrPADep != null)
                {
                    //inserting into link table
                    foreach (busPersonAccountDependent ldrPADep in adarrPADep)
                    {
                        cdoPersonAccountDependentBillingLink lcdoPADepBillingLink = new cdoPersonAccountDependentBillingLink();
                        lcdoPADepBillingLink.employer_payroll_detail_id = icdoEmployerPayrollDetail.employer_payroll_detail_id;
                        lcdoPADepBillingLink.person_account_dependent_id = ldrPADep.icdoPersonAccountDependent.person_account_dependent_id;
                        lcdoPADepBillingLink.Insert();
                    }
                }
            }
        }

        #region PROD PIR 933

        public Collection<busPersonAccountDependent> iclbPADependent { get; set; }

        public void LoadDependents()
        {
            iclbPADependent = new Collection<busPersonAccountDependent>();
            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            if (iclbPADependentBillingLink.IsNull()) LoadPersonAccountDependentBillingLink();
            DataTable ldtbResults = Select<cdoPersonAccountDependent>(new string[1] { "PERSON_ACCOUNT_ID" },
                                            new object[1] { ibusPersonAccount.icdoPersonAccount.person_account_id }, null, null);
            foreach (DataRow ldtrRow in ldtbResults.Rows)
            {
                busPersonAccountDependent lobjPADependent = new busPersonAccountDependent
                {
                    icdoPersonAccountDependent = new cdoPersonAccountDependent(),
                    icdoPersonDependent = new cdoPersonDependent()
                };
                lobjPADependent.icdoPersonAccountDependent.LoadData(ldtrRow);
                lobjPADependent.FindPersonDependent(lobjPADependent.icdoPersonAccountDependent.person_dependent_id);
                lobjPADependent.LoadDependentInfo();
                if (!iclbPADependentBillingLink.Where(lobj => lobj.icdoPersonAccountDependentBillingLink.person_account_dependent_id ==
                    lobjPADependent.icdoPersonAccountDependent.person_account_dependent_id).Any())
                    iclbPADependent.Add(lobjPADependent);
            }
        }

        private void InsertPADependentLink()
        {
            foreach (busPersonAccountDependent lobjPADependent in iclbPADependent)
            {
                if (lobjPADependent.icdoPersonDependent.is_selected_flag == busConstant.Flag_Yes)
                {
                    cdoPersonAccountDependentBillingLink lcdoPADependentLink = new cdoPersonAccountDependentBillingLink
                    {
                        person_account_dependent_id = lobjPADependent.icdoPersonAccountDependent.person_account_dependent_id,
                        employer_payroll_detail_id = icdoEmployerPayrollDetail.employer_payroll_detail_id
                    };
                    lcdoPADependentLink.Insert();
                }
            }
        }

        public Collection<busPersonAccountDependentBillingLink> iclbPADependentBillingLink { get; set; }

        public void LoadPersonAccountDependentBillingLink()
        {
            if (iclbPADependentBillingLink.IsNull()) iclbPADependentBillingLink = new Collection<busPersonAccountDependentBillingLink>();
            DataTable ldtbResult = Select<cdoPersonAccountDependentBillingLink>(new string[1] { "EMPLOYER_PAYROLL_DETAIL_ID" },
                                            new object[1] { icdoEmployerPayrollDetail.employer_payroll_detail_id }, null, null);
            iclbPADependentBillingLink = GetCollection<busPersonAccountDependentBillingLink>(ldtbResult, "icdoPersonAccountDependentBillingLink");
            foreach (busPersonAccountDependentBillingLink lobjPADependentBillLink in iclbPADependentBillingLink)
                lobjPADependentBillLink.LoadPADependent();
        }

        public bool iblnIsCoverageNeedsToSplit
        {
            get
            {
                if (Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.CoverageCodeSplitFlag, iobjPassInfo)) == busConstant.Flag_Yes)
                    return true;
                return false;
            }
        }

        #endregion

        public bool IsFutureDateValidationRequired()
        {
            if (busGlobalFunctions.GetData1ByCodeValue(52, "EMRE", iobjPassInfo) == busConstant.Flag_No)
                return false;
            return true;
        }

        /// <summary>
        /// PIR 8898 : Checks whether deferred comp provider org codes are repeated.
        /// </summary>
        /// <returns>
        /// True: if duplicate Org IDs exists
        /// False: otherwise
        /// </returns>
        public bool IsDeferredCompProviderOrgIDRepeated()
        {
            bool lblnResult = false;
            // Get all entered provider orgs in a collection
            Collection<busOrganization> lclbProviderOrgs = new Collection<busOrganization>();
            if (ibusProvider1.IsNotNull()) lclbProviderOrgs.Add(ibusProvider1);
            if (ibusProvider2.IsNotNull()) lclbProviderOrgs.Add(ibusProvider2);
            if (ibusProvider3.IsNotNull()) lclbProviderOrgs.Add(ibusProvider3);
            if (ibusProvider4.IsNotNull()) lclbProviderOrgs.Add(ibusProvider4);
            if (ibusProvider5.IsNotNull()) lclbProviderOrgs.Add(ibusProvider5);
            if (ibusProvider6.IsNotNull()) lclbProviderOrgs.Add(ibusProvider6);
            if (ibusProvider7.IsNotNull()) lclbProviderOrgs.Add(ibusProvider7);
            //Check for duplicates
            if (lclbProviderOrgs.Count > 0)
            {
                var lenumProviderOrgs = lclbProviderOrgs.Select(i => i.icdoOrganization.org_code).Distinct();
                if (lenumProviderOrgs.Count() < lclbProviderOrgs.Count)
                    lblnResult = true;
            }
            return lblnResult;
        }

        public void ClearObjects()
        {
            ihstRulesResult.Clear();
            ihstRulesResult = null;
            ibusSoftErrors.iclbError.Clear();
            ibusSoftErrors.iclbEmployerError.Clear();
            ibusSoftErrors = null;
        }

        /// <summary>
        /// Optimization for Validation only selected details for which we are ignoring the 
        /// </summary>
        /// <param name="aclbEmployerPayrollDetail"></param>
        public void ValidateRelatedDetailRecords()
        {
            Collection<busEmployerPayrollDetail> lclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
            var lresultEmployerPayrollDetail = _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Where(o => o.icdoEmployerPayrollDetail.ssn == icdoEmployerPayrollDetail.ssn);
            lclbEmployerPayrollDetail = lresultEmployerPayrollDetail.ToList<busEmployerPayrollDetail>().ToCollection();

            if (lclbEmployerPayrollDetail == null || lclbEmployerPayrollDetail.Count == 0 || icdoEmployerPayrollDetail.ssn.IsNullOrEmpty())
                return;

            foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in lclbEmployerPayrollDetail)
            {
                if ((lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusIgnored)
                    && (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusPosted))
                {
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = ibusEmployerPayrollHeader;

                    lobjEmployerPayrollDetail.LoadObjectsForValidation();

                    if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                    {
                        lobjEmployerPayrollDetail.LoadOrgIdForProviders();
                        lobjEmployerPayrollDetail.CheckProviderNotLinkedToEmployer();
                    }
                    lobjEmployerPayrollDetail.iblnValidateHeader = false;
                    if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                    {
                        lobjEmployerPayrollDetail.LoadOldSalaryAmount();
                    }

                    //Automatic Purchase Allocation for each detail
                    if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                    {
                        ibusEmployerPayrollHeader.AllocateAutomaticForPurchase();
                    }
                    lobjEmployerPayrollDetail.UpdateCalculatedFields();

                    lobjEmployerPayrollDetail.ValidateSoftErrors();
                    lobjEmployerPayrollDetail.ClearObjects();
                }
            }
        }


        public void UpdateAmtAndProviderFromFile()
        {
            DateTime ldtEffectiveDate = icdoEmployerPayrollDetail.pay_period_start_date;
            if ((icdoEmployerPayrollDetail.provider_id1 != 0) && (icdoEmployerPayrollDetail.contribution_amount1 != 0.00M)
                && (icdoEmployerPayrollDetail.contribution_amount1 != icdoEmployerPayrollDetail.amount_from_enrollment1))
            {
                UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id1, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider1_linked_with_member, icdoEmployerPayrollDetail.contribution_amount1);
                if (!icdoEmployerPayrollDetail.is_provider1_linked_with_member)
                {
                    UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id1,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider1_linked_with_member, icdoEmployerPayrollDetail.contribution_amount1);
                }
                icdoEmployerPayrollDetail.amount_from_enrollment1 = icdoEmployerPayrollDetail.contribution_amount1;
            }
            if ((icdoEmployerPayrollDetail.provider_id2 != 0) && (icdoEmployerPayrollDetail.contribution_amount2 != 0.00M)
                && (icdoEmployerPayrollDetail.contribution_amount2 != icdoEmployerPayrollDetail.amount_from_enrollment2))
            {
                UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id2, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider2_linked_with_member, icdoEmployerPayrollDetail.contribution_amount2);
                if (!icdoEmployerPayrollDetail.is_provider2_linked_with_member)
                {
                    UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id2,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider2_linked_with_member, icdoEmployerPayrollDetail.contribution_amount2);
                }
                icdoEmployerPayrollDetail.amount_from_enrollment2 = icdoEmployerPayrollDetail.contribution_amount2;
            }
            if ((icdoEmployerPayrollDetail.provider_id3 != 0) && (icdoEmployerPayrollDetail.contribution_amount3 != 0.00M)
                && (icdoEmployerPayrollDetail.contribution_amount3 != icdoEmployerPayrollDetail.amount_from_enrollment3))
            {
                UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id3, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider3_linked_with_member, icdoEmployerPayrollDetail.contribution_amount3);
                if (!icdoEmployerPayrollDetail.is_provider3_linked_with_member)
                {
                    UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id3,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider3_linked_with_member, icdoEmployerPayrollDetail.contribution_amount3);
                }
                icdoEmployerPayrollDetail.amount_from_enrollment3 = icdoEmployerPayrollDetail.contribution_amount3;
            }
            if ((icdoEmployerPayrollDetail.provider_id4 != 0) && (icdoEmployerPayrollDetail.contribution_amount4 != 0.00M)
                && (icdoEmployerPayrollDetail.contribution_amount4 != icdoEmployerPayrollDetail.amount_from_enrollment4))
            {
                UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id4, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider4_linked_with_member, icdoEmployerPayrollDetail.contribution_amount4);
                if (!icdoEmployerPayrollDetail.is_provider4_linked_with_member)
                {
                    UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id4,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider4_linked_with_member, icdoEmployerPayrollDetail.contribution_amount4);
                }
                icdoEmployerPayrollDetail.amount_from_enrollment4 = icdoEmployerPayrollDetail.contribution_amount4;
            }
            if ((icdoEmployerPayrollDetail.provider_id5 != 0) && (icdoEmployerPayrollDetail.contribution_amount5 != 0.00M)
                && (icdoEmployerPayrollDetail.contribution_amount5 != icdoEmployerPayrollDetail.amount_from_enrollment5))
            {
                UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id5, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider5_linked_with_member, icdoEmployerPayrollDetail.contribution_amount5);
                if (!icdoEmployerPayrollDetail.is_provider5_linked_with_member)
                {
                    UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id5,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider5_linked_with_member, icdoEmployerPayrollDetail.contribution_amount5);
                }
                icdoEmployerPayrollDetail.amount_from_enrollment5 = icdoEmployerPayrollDetail.contribution_amount5;
            }
            if ((icdoEmployerPayrollDetail.provider_id6 != 0) && (icdoEmployerPayrollDetail.contribution_amount6 != 0.00M)
                && (icdoEmployerPayrollDetail.contribution_amount6 != icdoEmployerPayrollDetail.amount_from_enrollment6))
            {
                UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id6, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider6_linked_with_member, icdoEmployerPayrollDetail.contribution_amount6);
                if (!icdoEmployerPayrollDetail.is_provider6_linked_with_member)
                {
                    UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id6,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider6_linked_with_member, icdoEmployerPayrollDetail.contribution_amount6);
                }
                icdoEmployerPayrollDetail.amount_from_enrollment6 = icdoEmployerPayrollDetail.contribution_amount6;
            }
            if ((icdoEmployerPayrollDetail.provider_id7 != 0) && (icdoEmployerPayrollDetail.contribution_amount7 != 0.00M)
                && (icdoEmployerPayrollDetail.contribution_amount7 != icdoEmployerPayrollDetail.amount_from_enrollment7))
            {
                UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id7, ldtEffectiveDate, ref icdoEmployerPayrollDetail.is_provider7_linked_with_member, icdoEmployerPayrollDetail.contribution_amount7);
                if (!icdoEmployerPayrollDetail.is_provider7_linked_with_member)
                {
                    UpdatePledgeAmountForFile(icdoEmployerPayrollDetail.provider_id7,
                        icdoEmployerPayrollDetail.pay_period_end_date, ref icdoEmployerPayrollDetail.is_provider7_linked_with_member, icdoEmployerPayrollDetail.contribution_amount7);
                }
                icdoEmployerPayrollDetail.amount_from_enrollment7 = icdoEmployerPayrollDetail.contribution_amount7;
            }

        }

        private void UpdatePledgeAmountForFile(int aintProviderOrgId, DateTime adtEffectiveDate, ref bool ablnLinkedWithProvider, decimal adecContributionAmount)
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.icdoPersonAccount.person_account_id != 0)
            {
                //Load the Provider Org ID
                ibusPersonAccount.LoadProviderOrgPlanByProviderOrgID(aintProviderOrgId, adtEffectiveDate);

                int lintPersonAccountId = ibusPersonAccount.icdoPersonAccount.person_account_id;

                //Person Employment ID added now in Deff Comp Provider Table. We must check the org code that should match provider person employment org code.
                DataTable ldtbList = Select<cdoPersonAccountDeferredCompProvider>(
                    new string[2] { "provider_org_plan_id", "person_account_id" },
                    new object[2] { ibusPersonAccount.ibusProviderOrgPlan.icdoOrgPlan.org_plan_id, lintPersonAccountId },
                    null, "start_date desc");

                Collection<busPersonAccountDeferredCompProvider> lclbPADCProvider =
                    GetCollection<busPersonAccountDeferredCompProvider>(ldtbList, "icdoPersonAccountDeferredCompProvider");


                foreach (busPersonAccountDeferredCompProvider lbusPADCProvider in lclbPADCProvider)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                        lbusPADCProvider.icdoPersonAccountDeferredCompProvider.start_date,
                        lbusPADCProvider.icdoPersonAccountDeferredCompProvider.end_date) &&
                        lbusPADCProvider.icdoPersonAccountDeferredCompProvider.start_date != lbusPADCProvider.icdoPersonAccountDeferredCompProvider.end_date_no_null)
                    {
                        busPersonEmployment lbusPersonEmployment = new busPersonEmployment();
                        lbusPersonEmployment.FindPersonEmployment(
                            lbusPADCProvider.icdoPersonAccountDeferredCompProvider.person_employment_id);

                        if (lbusPersonEmployment.icdoPersonEmployment.org_id == ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id)
                        {
                            ablnLinkedWithProvider = true;
                            lbusPADCProvider.icdoPersonAccountDeferredCompProvider.end_date = adtEffectiveDate.AddDays(-1);
                            lbusPADCProvider.icdoPersonAccountDeferredCompProvider.Update();
                            lbusPADCProvider.InsertIntoEnrollmentData(false, busConstant.Flag_No); //PIR 20169
                            busPersonAccountDeferredCompProvider lbusPersonAccountDeferredCompProvider = new busPersonAccountDeferredCompProvider { icdoPersonAccountDeferredCompProvider = new cdoPersonAccountDeferredCompProvider() };
                            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider = lbusPADCProvider.icdoPersonAccountDeferredCompProvider;
                            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.start_date = adtEffectiveDate;
                            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.end_date = DateTime.MinValue;
                            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt = adecContributionAmount;
                            //lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.is_enrollment_report_generated = busConstant.Flag_No;//PIR 17081
                            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.created_by = "PERSLinkBatch";
                            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.created_date = DateTime.Now;
                            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.update_seq = 0;
                            lbusPersonAccountDeferredCompProvider.icdoPersonAccountDeferredCompProvider.Insert();
                            lbusPersonAccountDeferredCompProvider.InsertIntoEnrollmentData(false, busConstant.Flag_No); //PIR 20169
                            break;
                        }
                    }
                }
            }
        }

        // Code Added for performance optimization
        public Collection<cdoPlan> ldtbPlan { get; set; }
        public Collection<cdoPlan> GetPlanByHeaderTypeRetirement()
        {
            if (ldtbPlan != null)
            {
                return ldtbPlan;
            }
            else
            {
                return ibusEmployerPayrollHeader.iclbPlan;
            }
        }

        //public Collection<cdoPlan> GetPlanByHeaderTypeRetirement()
        //{
        //    DataTable ldtbPlan = null;
        //    Collection<cdoPlan> lcdoPlan;
        //    if (ibusEmployerPayrollHeader.iclbPlan == null)
        //    {
        //        lcdoPlan = new Collection<cdoPlan>();
        //        string lstrBenefitTypeValue = busEmployerReportHelper.GetBenefitTypeForEmployerHeaderType(ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value);
        //        if (lstrBenefitTypeValue == busConstant.PayrollHeaderBenefitTypeDefComp)
        //        {
        //            ldtbPlan = busNeoSpinBase.Select("cdoPlan.GetOrgSpecificPlanWithdate",
        //                                                         new object[3] { aintOrgId, ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_check_date, lstrBenefitTypeValue });
        //        }
        //        else
        //        {
        //            ldtbPlan = busNeoSpinBase.Select("cdoPlan.RetirementPlans",
        //                                                          new object[2] { aintOrgId, lstrBenefitTypeValue });
        //        }
        //        foreach (cdoPlan tcdoPlan in Sagitec.DataObjects.doBase.LoadData<cdoPlan>(ldtbPlan))
        //        {
        //            lcdoPlan.Add(tcdoPlan);
        //        }
        //        ibusEmployerPayrollHeader.iclbPlan = lcdoPlan;
        //    }
        //    else
        //    {
        //        lcdoPlan = ibusEmployerPayrollHeader.iclbPlan;
        //    }
        //    return lcdoPlan;
        //}

        public void LoadOriginalWages(string astrMemberType)
        {
            if (String.IsNullOrEmpty(astrMemberType))
            {
                LoadMemberType();
            }
            else
            {
                member_type = astrMemberType;
            }

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();

            if (ibusPersonAccount.ibusPersonEmploymentDetail != null)
            {
                busPlanRetirementRate lobjPlanRetirement = new busPlanRetirementRate();
                if ((icdoEmployerPayrollDetail.plan_id != 0) && (member_type != null))
                {
                    // PIR 9647 - For Record Type Bonus use pay_period_last_date_for_bonus
                    if (icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus)
                        lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(member_type, icdoEmployerPayrollDetail.pay_period_last_date_for_bonus, icdoEmployerPayrollDetail.plan_id);
                    else
                        lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(member_type, icdoEmployerPayrollDetail.pay_period_last_date, icdoEmployerPayrollDetail.plan_id);
                    if (lobjPlanRetirement.icdoPlanRetirementRate.plan_rate_id != 0)
                    {
                        //get amount as per rates
                        decimal ldecSalary = 0.00M;
                        if (lobjPlanRetirement.icdoPlanRetirementRate.ee_emp_pickup != 0)
                        {
                            ldecSalary = _icdoEmployerPayrollDetail.ee_employer_pickup_original * 100 / lobjPlanRetirement.icdoPlanRetirementRate.ee_emp_pickup;
                        }
                        else if (lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax != 0)
                        {
                            ldecSalary = _icdoEmployerPayrollDetail.ee_contribution_original * 100 / lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax;
                        }
                        else if (lobjPlanRetirement.icdoPlanRetirementRate.ee_pre_tax != 0)
                        {
                            ldecSalary = _icdoEmployerPayrollDetail.ee_pre_tax_original * 100 / lobjPlanRetirement.icdoPlanRetirementRate.ee_pre_tax;
                        }
                        else if (lobjPlanRetirement.icdoPlanRetirementRate.er_post_tax != 0)
                        {
                            ldecSalary = _icdoEmployerPayrollDetail.er_contribution_original * 100 / lobjPlanRetirement.icdoPlanRetirementRate.er_post_tax;
                        }
                        else if (lobjPlanRetirement.icdoPlanRetirementRate.ee_rhic != 0)
                        {
                            ldecSalary = _icdoEmployerPayrollDetail.rhic_ee_contribution_original * 100 / lobjPlanRetirement.icdoPlanRetirementRate.ee_rhic;
                        }
                        else if (lobjPlanRetirement.icdoPlanRetirementRate.er_rhic != 0)
                        {
                            ldecSalary = _icdoEmployerPayrollDetail.rhic_er_contribution_original * 100 / lobjPlanRetirement.icdoPlanRetirementRate.er_rhic;
                        }
                        _icdoEmployerPayrollDetail.eligible_wages_original = ldecSalary;
                    }
                }
            }

        }



        public void LoadNegativeComponents()
        {
            if (icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment &&
                    ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                _icdoEmployerPayrollDetail.eligible_wages =
                    _icdoEmployerPayrollDetail.eligible_wages < 0 ?
                    _icdoEmployerPayrollDetail.eligible_wages : -1 * _icdoEmployerPayrollDetail.eligible_wages;
                _icdoEmployerPayrollDetail.ee_contribution_calculated =
                   _icdoEmployerPayrollDetail.ee_contribution_calculated < 0 ?
                    _icdoEmployerPayrollDetail.ee_contribution_calculated : -1 * _icdoEmployerPayrollDetail.ee_contribution_calculated;
                _icdoEmployerPayrollDetail.ee_pre_tax_calculated =
                    _icdoEmployerPayrollDetail.ee_pre_tax_calculated < 0 ?
                    _icdoEmployerPayrollDetail.ee_pre_tax_calculated : -1 * _icdoEmployerPayrollDetail.ee_pre_tax_calculated;
                _icdoEmployerPayrollDetail.ee_employer_pickup_calculated =
                    _icdoEmployerPayrollDetail.ee_employer_pickup_calculated < 0 ?
                    _icdoEmployerPayrollDetail.ee_employer_pickup_calculated : -1 * _icdoEmployerPayrollDetail.ee_employer_pickup_calculated;
                _icdoEmployerPayrollDetail.er_contribution_calculated =
                    _icdoEmployerPayrollDetail.er_contribution_calculated < 0 ?
                    _icdoEmployerPayrollDetail.er_contribution_calculated : -1 * _icdoEmployerPayrollDetail.er_contribution_calculated;
                _icdoEmployerPayrollDetail.rhic_er_contribution_calculated =
                    _icdoEmployerPayrollDetail.rhic_er_contribution_calculated < 0 ?
                    _icdoEmployerPayrollDetail.rhic_er_contribution_calculated : -1 * _icdoEmployerPayrollDetail.rhic_er_contribution_calculated;
                _icdoEmployerPayrollDetail.rhic_ee_contribution_calculated =
                    _icdoEmployerPayrollDetail.rhic_ee_contribution_calculated < 0 ?
                    _icdoEmployerPayrollDetail.rhic_ee_contribution_calculated : -1 * _icdoEmployerPayrollDetail.rhic_ee_contribution_calculated;

                //PIR 13996 -  displaying -ve interest values in round braces

                  _icdoEmployerPayrollDetail.member_interest_calculated =
                  _icdoEmployerPayrollDetail.member_interest_calculated < 0 ?
                  _icdoEmployerPayrollDetail.member_interest_calculated : -1 * _icdoEmployerPayrollDetail.member_interest_calculated;

                  _icdoEmployerPayrollDetail.employer_interest_calculated =
                  _icdoEmployerPayrollDetail.employer_interest_calculated < 0 ?
                  _icdoEmployerPayrollDetail.employer_interest_calculated : -1 * _icdoEmployerPayrollDetail.employer_interest_calculated;
    
                  _icdoEmployerPayrollDetail.employer_rhic_interest_calculated =
                  _icdoEmployerPayrollDetail.employer_rhic_interest_calculated < 0 ?
                  _icdoEmployerPayrollDetail.employer_rhic_interest_calculated : -1 * _icdoEmployerPayrollDetail.employer_rhic_interest_calculated;

                //PIR 25920 DC 2025 changes -  displaying -ve interest values in round braces

                _icdoEmployerPayrollDetail.ee_pretax_addl_calculated =
                _icdoEmployerPayrollDetail.ee_pretax_addl_calculated < 0 ?
                _icdoEmployerPayrollDetail.ee_pretax_addl_calculated : -1 * _icdoEmployerPayrollDetail.ee_pretax_addl_calculated;

                _icdoEmployerPayrollDetail.ee_post_tax_addl_calculated =
                _icdoEmployerPayrollDetail.ee_post_tax_addl_calculated < 0 ?
                _icdoEmployerPayrollDetail.ee_post_tax_addl_calculated : -1 * _icdoEmployerPayrollDetail.ee_post_tax_addl_calculated;

                _icdoEmployerPayrollDetail.er_pretax_match_calculated =
                _icdoEmployerPayrollDetail.er_pretax_match_calculated < 0 ?
                _icdoEmployerPayrollDetail.er_pretax_match_calculated : -1 * _icdoEmployerPayrollDetail.er_pretax_match_calculated;

                _icdoEmployerPayrollDetail.adec_calculated =
                _icdoEmployerPayrollDetail.adec_calculated < 0 ?
                _icdoEmployerPayrollDetail.adec_calculated : -1 * _icdoEmployerPayrollDetail.adec_calculated;

            }
        }

        public bool iblnIsFromFile { get; set; } //PIR 14519 : Save issue

        //PIR 14542 - If a payroll deduction reported through a payroll header is for more than the payoff of the purchase, the record should go into review
        public bool IsPurchaseAmountReportedGreaterThanPayOffAmount()
        {
            bool lblnResult = false;
            if (ibusEmployerPayrollHeader == null)
                LoadPayrollHeader();
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
            {
                if (iclbEmployerPurchaseAllocation == null)
                    LoadEmployerPurchaseAllocation();
                if (iclbEmployerPurchaseAllocation.Count > 0)
                {
                    busServicePurchaseHeader lobjServicePurchaseHeader = new busServicePurchaseHeader();
                    if (lobjServicePurchaseHeader.FindServicePurchaseHeader(iclbEmployerPurchaseAllocation[0].icdoEmployerPurchaseAllocation.service_purchase_header_id))
                    {
                        lobjServicePurchaseHeader.LoadPlan();
                        lobjServicePurchaseHeader.LoadServicePurchaseDetail();
                        lobjServicePurchaseHeader.LoadAmortizationSchedule();
                        if (lobjServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule.Count > 0)
                        {
                            busServicePurchaseAmortizationSchedule lbusServicePurchaseAmortizationSchedule = lobjServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule
                                 .Where(i => i.icdoServicePurchaseAmortizationSchedule.idecPayOffAmount > 0.00M).FirstOrDefault();
                            if (lbusServicePurchaseAmortizationSchedule.IsNotNull())
                            {
                                if (lbusServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.idecPayOffAmount < icdoEmployerPayrollDetail.purchase_amount_reported)
                                {
                                    lblnResult = true;
                                }
                            }

                        }
                    }
                }
            }
            return lblnResult;
        }

        //PIR 15741 - Detail Should not be valid if person account employment detail election status is not enrolled
        public bool IsValidPersonAccountElectionStatus()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if(ibusPersonAccount.icdoPersonAccount.person_account_id > 0 && ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
            {
                //validating details from employer report posting batch 
                if(ibusEmployerPayrollHeader.idtbAllPAEmpDetailWithChildData != null)
                {
                               DataRow ldrResult = ibusEmployerPayrollHeader
                                        .idtbAllPAEmpDetailWithChildData
                                        .AsEnumerable()
                                        .Where(i => (i.Field<int?>("person_account_id").IsNotNull()  && i.Field<int>("person_account_id") == ibusPersonAccount.icdoPersonAccount.person_account_id)
                                        && i.Field<int>("PERSON_EMPLOYMENT_DTL_ID") == ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id)
                                        .FirstOrDefault();
                               if (ldrResult.IsNotNull())
                                   if (Convert.ToString(ldrResult["ELECTION_VALUE"]) != busConstant.PersonAccountElectionValueEnrolled)
                                       return false;
                                        
                }
                else //validating the detail from screen
                {
                    DataTable ldtbPerAccEmpDetailElection = busBase.Select<cdoPersonAccountEmploymentDetail>
                        (new string[2] { enmPersonAccountEmploymentDetail.person_account_id.ToString(), enmPersonAccountEmploymentDetail.person_employment_dtl_id.ToString() }, 
                        new object[2] { ibusPersonAccount.icdoPersonAccount.person_account_id, ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id }, 
                        null, null);
                    if(ldtbPerAccEmpDetailElection.IsNotNull() && ldtbPerAccEmpDetailElection.Rows.Count > 0)
                    {
                        if (Convert.ToString(ldtbPerAccEmpDetailElection.Rows[0]["ELECTION_VALUE"]) != busConstant.PersonAccountElectionValueEnrolled)
                            return false;
                    }
                                                                                                                        
                }
            }
            return true;
        }

        //Org to bill
        public Collection<cdoLowIncomeCreditRef> iclbLowIncomeCreditRef { get; set; }
        public Collection<cdoLowIncomeCreditRef> LoadLowIncomeCreditRef()
        {
            busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };

            lobjMedicare.FindMedicareByPersonAccountID(icdoEmployerPayrollDetail.person_account_id);
            lobjMedicare.FindPersonAccount(icdoEmployerPayrollDetail.person_account_id);


            if (idtbCachedLowIncomeCredit == null)
                idtbCachedLowIncomeCredit = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);

            if (lobjMedicare.IsNotNull())
            {
                if (lobjMedicare.idtPlanEffectiveDate == DateTime.MinValue)
                    lobjMedicare.LoadPlanEffectiveDate();
            }
            else
                lobjMedicare.idtPlanEffectiveDate = DateTime.Now;

            DateTime ldtEffectiveDate = new DateTime();
            var lenumList = idtbCachedLowIncomeCredit.AsEnumerable().OrderByDescending(i => i.Field<DateTime>("effective_date"));
            foreach (DataRow dr in lenumList)
            {
                if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjMedicare.idtPlanEffectiveDate.Date)
                {
                    ldtEffectiveDate = Convert.ToDateTime(dr["effective_date"]).Date;
                    break;
                }
            }
            DataTable ldtFilteredLowIncomeCredit = idtbCachedLowIncomeCredit.AsEnumerable().Where(i => i.Field<DateTime>("effective_date") == ldtEffectiveDate.Date).AsDataTable();

            iclbLowIncomeCreditRef = Sagitec.DataObjects.doBase.GetCollection<cdoLowIncomeCreditRef>(ldtFilteredLowIncomeCredit);
            iclbLowIncomeCreditRef.ForEach(i => i.display_credit = i.low_income_credit.ToString());
            ////Adding Empty Item Here since Framework has bug if you select the Last Item. Temporary Workaround
            var lcdoTempRef = new cdoLowIncomeCreditRef();
            lcdoTempRef.amount = 0;
            lcdoTempRef.low_income_credit = 0;
            lcdoTempRef.display_credit = string.Empty;
            iclbLowIncomeCreditRef.Add(lcdoTempRef);
            iclbLowIncomeCreditRef.OrderBy(i => i.low_income_credit);
            return iclbLowIncomeCreditRef;
        }

        public bool CheckIfPayPeriodStartDateIsValid()
        {

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonDeferredComp == null)
                ibusPersonAccount.ibusPersonDeferredComp = new busPersonAccountDeferredComp { icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp() };
            ibusPersonAccount.ibusPersonDeferredComp.icdoPersonAccountDeferredComp.person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;

            if (ibusPersonAccount.ibusPersonDeferredComp.iclbActiveProvidersByPayPeriodStartDate == null)
                ibusPersonAccount.ibusPersonDeferredComp.LoadActivePersonAccountProvidersByPayPeriodStartDate(icdoEmployerPayrollDetail.pay_period_start_date);
            return ((ibusPersonAccount.ibusPersonDeferredComp.iclbActiveProvidersByPayPeriodStartDate.Count() > 0) || (ibusPersonAccount.icdoPersonAccount.person_account_id == 0)) ? true : false;
        }

		//PIR 7941 - We want to avoid having to go into each detail individually to ignore them or to add comments when there are many records to ignore.
        public List<int> IgnoreSelectedPayrollDetail(List<int> aEmpPayDetailIds, string astrComment,bool ablnIsSetIgnorStatus)
        {
            List<int> lintEmployerPayrollDetailIds = new List<int>();
            foreach (int lintEmployerPayrollDetailId in aEmpPayDetailIds)
            {
                busEmployerPayrollDetail lbusEmployerPayrollDetail = new busEmployerPayrollDetail();
                if(lbusEmployerPayrollDetail.FindEmployerPayrollDetail(lintEmployerPayrollDetailId))
                {
                    if (ablnIsSetIgnorStatus)
                    {
                        if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusPosted)
                        {
                            lbusEmployerPayrollDetail.DeleteDetailErrors();
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value = busConstant.PayrollDetailStatusIgnored;
                            //
                            SaveComments(lbusEmployerPayrollDetail, astrComment);
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.Update();
                        }
                        else
                        {
                            lintEmployerPayrollDetailIds.Add(lintEmployerPayrollDetailId);
                        }
                    }
                    else
                    {
                        SaveComments(lbusEmployerPayrollDetail, astrComment);
                    }
                }
            }
            return lintEmployerPayrollDetailIds;
        }
        public bool iblnIsFromValidDetail { get; set; }
		// ESS Backlog PIR - 13416
        public void LoadEmployerPayrollDetailComments()
        {
            DataTable ldtbList = Select<cdoComments>(
                               new string[1] { "employer_payroll_detail_id" },
                               new object[1] { _icdoEmployerPayrollDetail.employer_payroll_detail_id }, null, null);
            iclbPayrollDetailCommentsHistory = GetCollection<busComments>(ldtbList, "icdoComments");
            iclbPayrollDetailCommentsHistory = iclbPayrollDetailCommentsHistory.OrderBy(o => o.icdoComments.created_date).ToList<busComments>().ToCollection<busComments>();
            if ((this.iobjPassInfo.istrFormName == "wfmEmployerPayrollDetailLookup" || this.iobjPassInfo.istrFormName == "wfmEmployerPayrollDetailMaintenance") && iobjPassInfo.idictParams.ContainsKey("UserSettingForRenderNeoGrid") && Convert.ToBoolean(iobjPassInfo.idictParams["UserSettingForRenderNeoGrid"]) == false)
            {
                foreach (busComments item in iclbPayrollDetailCommentsHistory)
                {
                    Regex regex = new Regex(@"\n");
                    string temp = item.icdoComments.comments;
                    item.icdoComments.comments = regex.Replace(temp, "</br>");
                }
            }
        }

        //ESS Backlog PIR - 13416
		public String LoadCommentsOfPayrollDetailForExportToText()
        {
            const string lstrSpaceSeperator = " ";
            String Comment = String.Empty;
            StringBuilder sb = new StringBuilder();
            if (iclbPayrollDetailCommentsHistory == null)
                LoadEmployerPayrollDetailComments();
            foreach (busComments iobjbusComments in iclbPayrollDetailCommentsHistory)
            {
                sb.Append(iobjbusComments.icdoComments.comments.ToString());
                sb.Append(lstrSpaceSeperator);
                sb.Append(iobjbusComments.icdoComments.created_by.ToString());
                sb.Append(lstrSpaceSeperator);
                sb.Append(iobjbusComments.icdoComments.created_date.ToString());
                sb.Append(";");
                Comment += sb.ToString();
                sb.Clear();
            }
            return Comment;
        } 
//SP PIR-12686
        public bool IsServicePurchaseInPayment()
        {
            DataTable  ldtbServicePurchaseInpayment = busBase.Select("cdoEmployerPayrollDetail.EmployerDetailsSPinPayment", new object[1] {icdoEmployerPayrollDetail.employer_payroll_detail_id} );
            return (ldtbServicePurchaseInpayment.Rows.Count > 0) ? true : false;
        }

        //Correspondance backlog PIR - 17184
        public override busBase GetCorPerson()
        {
            if (_ibusPerson == null)
                LoadPerson();
            return _ibusPerson;
        }
		// PIR-17777
        public bool IsPositiveBonusAvailable()
        {            
                DataTable ldtPositiveBonus = busBase.Select("cdoEmployerPayrollDetail.AllowNegativeBonus", new object[6]{ icdoEmployerPayrollDetail.plan_id, ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id,
            icdoEmployerPayrollDetail.pay_period_date, icdoEmployerPayrollDetail.pay_period_end_month_for_bonus, icdoEmployerPayrollDetail.person_id, icdoEmployerPayrollDetail.eligible_wages});
                if (ldtPositiveBonus.Rows.Count > 0)
                {
                    return true;
                }
                return false;                      
        }
        //PIR - 8444
        public void btnSaveComments_click(string astrComment)
        {
            if (!String.IsNullOrWhiteSpace(astrComment))
            {
                busComments lbusComments = new busComments { icdoComments = new cdoComments() };
                lbusComments.icdoComments.comments = astrComment;
                lbusComments.icdoComments.employer_payroll_header_id = icdoEmployerPayrollDetail.employer_payroll_header_id;
                lbusComments.icdoComments.employer_payroll_detail_id = icdoEmployerPayrollDetail.employer_payroll_detail_id;                
                lbusComments.icdoComments.Insert();
            }
            //PIR 26109 - Column with checkbox; read-only in LOB
            if (ibusEmployerPayrollHeader.IsNull())  LoadPayrollHeader();
            if ((iobjPassInfo.istrFormName == "wfmESSEmployerPayrollDetailMaintenance" || iobjPassInfo.istrFormName == "wfmEmployerPayrollDetailMaintenance") && astrComment.IsNotNullOrEmpty())
            {
                ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.detail_comments = iobjPassInfo.istrFormName== "wfmESSEmployerPayrollDetailMaintenance"? "Y":"N";
                ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.Update();
            }
            
        }
        /// <summary>
        /// PIR 21202 - Amounts cannot be less than $25/month 
        /// (or $12.50 for Bi-weekly, semimonthly).
        /// </summary>
        /// <returns></returns>
        public bool IsPledgeAmtNotAsPerFrequency()
        {
            if (icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdOther457 && ibusPersonAccount?.icdoPersonAccount?.person_account_id > 0 &&
                icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular)
            {
                busPersonAccountDeferredComp lbusPersonAccountDeferredComp = new busPersonAccountDeferredComp() { icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp() };

                if (ibusPersonAccount.ibusPersonEmploymentDetail == null)
                    LoadPersonEmploymentDetail();
                lbusPersonAccountDeferredComp.FindPersonAccountDeferredComp(ibusPersonAccount.icdoPersonAccount.person_account_id);
                if (ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
                {
                    lbusPersonAccountDeferredComp.icdoPersonAccount.person_employment_dtl_id = ibusPersonAccount.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;

                    lbusPersonAccountDeferredComp.LoadOrgPlan(idtOrgPlanEffectiveDate);
                    decimal ldecComparedAmt = 0.0M;
                    if ((lbusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyBiWeekly) ||
                              (lbusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencySemiMonthly))
                    {
                        ldecComparedAmt = 12.50M;
                    }
                    else if ((lbusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value == busConstant.DeffCompFrequencyMonthly))
                    {
                        ldecComparedAmt = 25.00M;
                    }
                    if (ldecComparedAmt > 0.0M)
                    {
                        return ((icdoEmployerPayrollDetail.contribution_amount1 > 0 && icdoEmployerPayrollDetail.contribution_amount1 < ldecComparedAmt) ||
                            (icdoEmployerPayrollDetail.contribution_amount2 > 0 && icdoEmployerPayrollDetail.contribution_amount2 < ldecComparedAmt) ||
                            (icdoEmployerPayrollDetail.contribution_amount3 > 0 && icdoEmployerPayrollDetail.contribution_amount3 < ldecComparedAmt) ||
                            (icdoEmployerPayrollDetail.contribution_amount4 > 0 && icdoEmployerPayrollDetail.contribution_amount4 < ldecComparedAmt) ||
                            (icdoEmployerPayrollDetail.contribution_amount5 > 0 && icdoEmployerPayrollDetail.contribution_amount5 < ldecComparedAmt) ||
                            (icdoEmployerPayrollDetail.contribution_amount6 > 0 && icdoEmployerPayrollDetail.contribution_amount6 < ldecComparedAmt) ||
                            (icdoEmployerPayrollDetail.contribution_amount7 > 0 && icdoEmployerPayrollDetail.contribution_amount7 < ldecComparedAmt));
                    }
                }
            }
            return false;
        }

        private void SaveComments(busEmployerPayrollDetail lbusEmployerPayrollDetail, string astrComment)
        {
            //ESS Backlog PIR - 13416 
            if (!String.IsNullOrEmpty(astrComment))
            {
                busComments lbusComments = new busComments { icdoComments = new cdoComments() };
                lbusComments.icdoComments.comments = astrComment;
                lbusComments.icdoComments.employer_payroll_header_id = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id;
                lbusComments.icdoComments.employer_payroll_detail_id = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id;
                lbusComments.icdoComments.created_by = iobjPassInfo.istrUserID;
                lbusComments.icdoComments.modified_by = iobjPassInfo.istrUserID;
                lbusComments.icdoComments.created_date = DateTime.Now;
                lbusComments.icdoComments.modified_date = DateTime.Now;
                lbusComments.icdoComments.Insert();
                //PIR 26109 - Column with checkbox; read-only in LOB
                if (_icdoEmployerPayrollDetail.IsNull()) _icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail();
                _icdoEmployerPayrollDetail.employer_payroll_header_id = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id;
                LoadPayrollHeader();
                ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.detail_comments = "N";
                ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.Update();

            }
        }

        //PIR 26199 Add Replicate Button on Payroll Details regardless of status
        public busEmployerPayrollDetail btnReplicate_Clicked(int Aintemployerpayrolldetailid)
        {
            busEmployerPayrollDetail lobjEmployerPayrollDetail = new busEmployerPayrollDetail();
            //To Avoid Null Issues in Smart Navigation
            lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = new busEmployerPayrollHeader();
            lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            if (lobjEmployerPayrollDetail.FindEmployerPayrollDetail(Aintemployerpayrolldetailid))
            {
                lobjEmployerPayrollDetail.LoadPayrollHeader();
                lobjEmployerPayrollDetail.iblnOnlineCreation = true;
                lobjEmployerPayrollDetail.LoadOrgCodeID();

                //Org to bill
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    //lobjIbsDetail.icdoIbsDetail.ldecLowIncomeCredit = lobjIbsDetail.icdoIbsDetail.lis_amount;
                    busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                    lobjMedicare.FindMedicareByPersonAccountID(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_account_id);
                    lobjMedicare.FindPersonAccount(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_account_id);

                    lobjMedicare.LoadPlanEffectiveDate();

                    //Low Income Credit Amount should be populated from Ref table. 
                    DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                    DateTime ldtEffectiveDate = new DateTime();
                    var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().OrderByDescending(i => i.Field<DateTime>("effective_date"));
                    foreach (DataRow dr in lenumList)
                    {
                        if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjMedicare.idtPlanEffectiveDate.Date)
                        {
                            ldtEffectiveDate = Convert.ToDateTime(dr["effective_date"]).Date;
                            break;
                        }
                    }
                    DataTable ldtFilteredLowIncomeCredit = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<DateTime>("effective_date") == ldtEffectiveDate.Date).AsDataTable();

                    var lenumListFiltered = ldtFilteredLowIncomeCredit.AsEnumerable().Where(i => i.Field<Decimal>("amount") == Math.Abs(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.lis_amount)).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                    foreach (DataRow dr in lenumListFiltered)
                    {
                        if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjMedicare.idtPlanEffectiveDate.Date)
                        {
                            lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.idecLow_Income_Credit = Convert.ToDecimal(dr["low_income_credit"]);
                            break;
                        }
                    }

                    lobjEmployerPayrollDetail.LoadLowIncomeCreditRef();

                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.iintPremiumForPersonId = lobjMedicare.icdoPersonAccount.person_id;//Display Premium for on screen
                }

                lobjEmployerPayrollDetail.LoadObjectsForValidation();
                lobjEmployerPayrollDetail.LoadErrors();
                lobjEmployerPayrollDetail.LoadEmployerPurchaseAllocation();
                lobjEmployerPayrollDetail.LoadEmployerPayrollDetailError();
                lobjEmployerPayrollDetail.LoadEmployerPayrollDetailErrorLOB();

                if (lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date != DateTime.MinValue)
                {
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.pay_period = String.Empty;
                }
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date != DateTime.MinValue)
                {
                    lobjEmployerPayrollDetail.pay_period = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lobjEmployerPayrollDetail.pay_period = String.Empty;
                }
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus != DateTime.MinValue)
                {
                    lobjEmployerPayrollDetail.pay_end_month = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_end_month_for_bonus.ToString("MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));
                }
                else
                {
                    lobjEmployerPayrollDetail.pay_end_month = String.Empty;
                }
                lobjEmployerPayrollDetail.LoadPeoplesoftID();

                // PROD PIR 933
                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupHealth &&
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.RecordTypeNegativeAdjustment)
                {
                    lobjEmployerPayrollDetail.LoadPersonAccountDependentBillingLink();
                    lobjEmployerPayrollDetail.LoadDependents();
                }

                //ESS Backlog PIR- 12843 Added Provider org code for insurance type records
                if (lobjEmployerPayrollDetail.ibusPlan.IsInsurancePlan())
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code = busGlobalFunctions.GetOrgCodeFromOrgId(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_id);
                // ESS Backlog PIR - 13416
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.comments = String.Empty;
                lobjEmployerPayrollDetail.LoadEmployerPayrollDetailComments();

            }

            if (lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
            {
                lobjEmployerPayrollDetail.ienmPageMode = Sagitec.Common.utlPageMode.New;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ienuObjectState = Sagitec.Common.ObjectState.Insert;
                lobjEmployerPayrollDetail.ibusSoftErrors = null;
                lobjEmployerPayrollDetail.iclbEmployerPayrollDetailError = new Collection<busEmployerPayrollDetailError>();
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id = 0;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.comments = string.Empty;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.created_date = DateTime.MinValue;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.created_by = string.Empty;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.modified_by = string.Empty;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.modified_date = DateTime.MinValue;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.status_value = busConstant.StatusReview;
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1210, busConstant.StatusReview);
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.istrProviderOrgCode = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code;
            }

            else
            {
                lobjEmployerPayrollDetail.ienmPageMode = Sagitec.Common.utlPageMode.New;
                this.icdoEmployerPayrollDetail.ienuObjectState = Sagitec.Common.ObjectState.Insert;
                this.ibusSoftErrors = null;
                this.iclbEmployerPayrollDetailError = new Collection<busEmployerPayrollDetailError>();
                this.icdoEmployerPayrollDetail.employer_payroll_detail_id = 0;
                this.icdoEmployerPayrollDetail.comments = string.Empty;
                this.icdoEmployerPayrollDetail.created_date = DateTime.MinValue;
                this.icdoEmployerPayrollDetail.created_by = string.Empty;
                this.icdoEmployerPayrollDetail.modified_by = string.Empty;
                this.icdoEmployerPayrollDetail.modified_date = DateTime.MinValue;
                this.icdoEmployerPayrollDetail.status_value = busConstant.StatusReview;
                this.icdoEmployerPayrollDetail.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1210, busConstant.StatusReview);
                this.icdoEmployerPayrollDetail.istrProviderOrgCode = lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.provider_org_code;
            }

            return lobjEmployerPayrollDetail;
        }
        //PIR 26369
        public void LoadRetiremntPlanRate()
        {
            if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
            {
                int? lintNullableaddl_ee_contribution_percent = 0;
                int lintaddl_ee_contribution_percent = 0;
                busPlanRetirementRate lobjPlanRetirement = new busPlanRetirementRate();
                if (member_type == null)
                    LoadMemberType();
                if ((icdoEmployerPayrollDetail.plan_id != 0) && (member_type != null))
                {
                    if (icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus || icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeBonus)
                    {
                        if (ibusEmployerPayrollHeader.IsNull()) LoadPayrollHeader();
                        lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(member_type, ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date, icdoEmployerPayrollDetail.plan_id);
                    }
                    else
                        lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(member_type, icdoEmployerPayrollDetail.pay_period_last_date, icdoEmployerPayrollDetail.plan_id);

                    idecEe_pre_tax = lobjPlanRetirement.icdoPlanRetirementRate.ee_pre_tax;
                    idecEr_post_tax = lobjPlanRetirement.icdoPlanRetirementRate.er_post_tax;
                    idecEe_post_tax = lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax;
                    idecEe_emp_pickup = lobjPlanRetirement.icdoPlanRetirementRate.ee_emp_pickup;
                    idecEe_rhic = lobjPlanRetirement.icdoPlanRetirementRate.ee_rhic;
                    idecEr_rhic = lobjPlanRetirement.icdoPlanRetirementRate.er_rhic;
                    //PIR 25920 New Plan DC 2025
                    if (ibusPersonAccount.IsNull()) LoadPersonAccount();
                    if (ibusPersonAccount.icdoPersonAccount.IsNull()) LoadPersonAccount();
                    if (ibusPersonAccount.ibusPersonAccountRetirement == null)
                        ibusPersonAccount.LoadPersonAccountRetirement();
                    if (ibusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate == null)
                    {
                        if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                            ibusPersonAccount.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(icdoEmployerPayrollDetail.pay_period_date);
                        if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                            ibusPersonAccount.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(icdoEmployerPayrollDetail.pay_period_start_date);
                    }
                    lintNullableaddl_ee_contribution_percent = ibusPersonAccount.ibusPersonAccountRetirement.ibusHistoryAsofDate?.icdoPersonAccountRetirementHistory?.addl_ee_contribution_percent;
                    lintaddl_ee_contribution_percent = Convert.ToInt32(lintNullableaddl_ee_contribution_percent);

                    idecEE_pretax_addl = lobjPlanRetirement.icdoPlanRetirementRate.ee_pretax_addl > 0 ? lintaddl_ee_contribution_percent : 0.00m;
                    idecEE_post_tax_addl = lobjPlanRetirement.icdoPlanRetirementRate.ee_post_tax_addl > 0 ? lintaddl_ee_contribution_percent : 0.00m;
                    idecER_pretax_match = lobjPlanRetirement.icdoPlanRetirementRate.er_pretax_match > 0 ? lintaddl_ee_contribution_percent : 0.00m;
                    idecADEC = lintaddl_ee_contribution_percent > 0 ? lobjPlanRetirement.icdoPlanRetirementRate.adec : 0.00m;

                    idectotal_EE_contribution_rates = (idecEe_post_tax + idecEe_pre_tax + idecEe_rhic + idecEE_pretax_addl + idecEE_post_tax_addl);
                    idectotal_ER_contribution_rates = (idecEe_emp_pickup + idecEr_post_tax + idecEr_rhic + idecER_pretax_match + idecADEC);
                    idecTotal_contribution_rates = (idectotal_ER_contribution_rates + idectotal_EE_contribution_rates);
                    //idecTotal_contribution_rates = idecEe_pre_tax + idecEr_post_tax + idecEe_post_tax + idecEe_emp_pickup + idecEe_rhic + idecEr_rhic
                    //    + idecEE_pretax_addl+ idecEE_post_tax_addl+ idecER_pretax_match + idecADEC;  //PIR 25920 New Plan DC 2025
                }
            }
        }
        //PIR 24585 - Error Messagr 4554 should trigger when member is enrolled in a Retirement plan but the payroll detail plan is not the same
        public bool CheckMemberIsNotEnrolledInRetirementPlan()
        {
            if(this.icdoEmployerPayrollDetail.plan_id > 0 && this.icdoEmployerPayrollDetail.person_id > 0)
            {
                DataTable ldtbEnrolledPlans = Select("entEmployerPayrollDetail.DoesMemberHaveEnrolledHistoryPerDetail", 
                                            new object[2] { this.icdoEmployerPayrollDetail.person_id, this.icdoEmployerPayrollDetail.plan_id });
                return (ldtbEnrolledPlans?.Rows?.Count > 0) ? false : true;
            }
            return false;
        }
    }
}
