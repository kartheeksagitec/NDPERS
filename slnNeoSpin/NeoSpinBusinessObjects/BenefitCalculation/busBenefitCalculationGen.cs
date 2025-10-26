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

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitCalculationGen : busPersonBase
    {
        public busBenefitCalculationGen()
        {
        }

        private cdoBenefitCalculation _icdoBenefitCalculation;
        public cdoBenefitCalculation icdoBenefitCalculation
        {
            get
            {
                return _icdoBenefitCalculation;
            }
            set
            {
                _icdoBenefitCalculation = value;
            }
        }
        # region PropertiesForQuadroOnly
            public decimal idecquadro_ee_pre_tax_amount { get; set; }
            public decimal idecquadro_ee_post_tax_amount { get; set; }
            public decimal idecquadro_ee_er_pickup_amount { get; set; }
            public decimal idecquadro_capital_gain { get; set; }
            public decimal idecquadro_interest_amount { get; set; }
            public decimal idecquadro_er_vested_amount { get; set; }
            public decimal idecquadro_calculation_additional_interest_amount { get; set; }
        # endregion


        private Collection<busPayeeAccount> _iclbPayeeAccount;
        public Collection<busPayeeAccount> iclbPayeeAccount
        {
            get { return _iclbPayeeAccount; }
            set { _iclbPayeeAccount = value; }
        }
        public void LoadPayeeAccount()
        {
            DataTable ldtbResult = Select<cdoPayeeAccount>(
                                      new string[1] { "calculation_id" },
                                      new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            iclbPayeeAccount = GetCollection<busPayeeAccount>(ldtbResult, "icdoPayeeAccount");
        }
        public bool FindBenefitCalculation(int Aintbenefitcalculationid)
        {
            bool lblnResult = false;
            if (_icdoBenefitCalculation == null)
            {
                _icdoBenefitCalculation = new cdoBenefitCalculation();
            }
            if (_icdoBenefitCalculation.SelectRow(new object[1] { Aintbenefitcalculationid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        public bool FindBenefitCalculationByApplication(int Aintbenefitapplicationid)
        {
            bool lblnResult = false;
            if (_icdoBenefitCalculation == null)
            {
                _icdoBenefitCalculation = new cdoBenefitCalculation();
            }

            DataTable ldtbList = Select<cdoBenefitCalculation>(new string[1] { "BENEFIT_APPLICATION_ID" },
                  new object[1] { Aintbenefitapplicationid }, null, "benefit_calculation_id desc");
            if (ldtbList.Rows.Count > 0)
            {
                _icdoBenefitCalculation.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }

        //this method is written in order to get cal id for the given disability payee account id 
        //used in workflow. if this method got changed will reflect in workflow also.
        public bool FindBenefitCalculationByDisabilityPayeeAccountID(int Aintdisabilitypayeeaccountid)
        {
            bool lblnResult = false;
            if (_icdoBenefitCalculation == null)
            {
                _icdoBenefitCalculation = new cdoBenefitCalculation();
            }

            DataTable ldtbList = Select<cdoBenefitCalculation>(new string[1] { "DISABILITY_PAYEE_ACCOUNT_ID" },
                  new object[1] { Aintdisabilitypayeeaccountid }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                _icdoBenefitCalculation.LoadData(ldtbList.Rows[0]);
                lblnResult = true;
            }
            return lblnResult;
        }
        
        public bool IsJobService
        {
            get
            {
                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService)
                    return true;
                else
                    return false;
            }
        }

        private bool _iblnWaiveEarlyReduction;
        public bool iblnWaiveEarlyReduction
        {
            get { return _iblnWaiveEarlyReduction; }
            set { _iblnWaiveEarlyReduction = value; }
        }

        private bool _iblnIsBenefitPayeeTabForNewModeVisible;
        public bool iblnIsBenefitPayeeTabForNewModeVisible
        {
            get { return _iblnIsBenefitPayeeTabForNewModeVisible; }
            set { _iblnIsBenefitPayeeTabForNewModeVisible = value; }
        }

        public bool iblnIsDNROFactorNotExists { get; set; }

        public bool iblnIsEarlyFactorNotExists { get; set; }

        public bool iblnIsPLSOFactorNotExists { get; set; }

        private decimal _idecRemainingServiceCredit;
        public decimal idecRemainingServiceCredit
        {
            get { return _idecRemainingServiceCredit; }
            set { _idecRemainingServiceCredit = value; }
        }

        public decimal idecMemberAgeAsonDateofDeath { get; set; }

        private decimal _idecMemberAgeBasedOnRetirementDate;
        public decimal idecMemberAgeBasedOnRetirementDate
        {
            get { return _idecMemberAgeBasedOnRetirementDate; }
            set { _idecMemberAgeBasedOnRetirementDate = value; }
        }

        public string idecMemberAgeBasedOnRetirementDate_formatted
        {
            get
            {
                if (iintMembersAgeInMonthsAsOnRetirementDate < 0)
                    return String.Format("{0} Years and {1} Month(s)", Math.Ceiling(Convert.ToDouble(iintMembersAgeInMonthsAsOnRetirementDate / 12)).ToString(),
                                     Math.Round(Convert.ToDecimal(iintMembersAgeInMonthsAsOnRetirementDate % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years and {1} Month(s)", Math.Floor(Convert.ToDouble(iintMembersAgeInMonthsAsOnRetirementDate / 12)).ToString(),
                                     Math.Round(Convert.ToDecimal(iintMembersAgeInMonthsAsOnRetirementDate % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }

        private int _iintMembersAgeInMonthsAsOnRetirementDate;
        public int iintMembersAgeInMonthsAsOnRetirementDate
        {
            get { return _iintMembersAgeInMonthsAsOnRetirementDate; }
            set { _iintMembersAgeInMonthsAsOnRetirementDate = value; }
        }

        private int _iintNoofEligibleChild;
        public int iintNoofEligibleChild
        {
            get { return _iintNoofEligibleChild; }
            set { _iintNoofEligibleChild = value; }
        }

        private bool _iblnSpouseExists;
        public bool iblnSpouseExists
        {
            get { return _iblnSpouseExists; }
            set { _iblnSpouseExists = value; }
        }

        private bool _iblnChildExists;
        public bool iblnChildExists
        {
            get { return _iblnChildExists; }
            set { _iblnChildExists = value; }
        }
        private decimal _idecTotalSalary;
        public decimal idecTotalSalary
        {
            get { return _idecTotalSalary; }
            set { _idecTotalSalary = value; }
        }

        private decimal _idecTotalSalary2019;
        public decimal idecTotalSalary2019
        {
            get { return _idecTotalSalary2019; }
            set { _idecTotalSalary2019 = value; }
        }

        private decimal _idecTotalSalary2020;
        public decimal idecTotalSalary2020
        {
            get { return _idecTotalSalary2020; }
            set { _idecTotalSalary2020 = value; }
        }
        private decimal _idecSSliBenefitAmount;
        public decimal idecSSliBenefitAmount
        {
            get { return _idecSSliBenefitAmount; }
            set { _idecSSliBenefitAmount = value; }
        }

        private decimal _idecWSIBenefitAmount;
        public decimal idecWSIBenefitAmount
        {
            get { return _idecWSIBenefitAmount; }
            set { _idecWSIBenefitAmount = value; }
        }

        public decimal idecRHICEarlyRetirementReductionAmount
        {
            get
            {
                return Math.Round(icdoBenefitCalculation.unreduced_rhic_amount * icdoBenefitCalculation.rhic_early_reduction_factor, 2,MidpointRounding.AwayFromZero);
            }
        }

        public decimal idecReducedRHICAmount
        {
            get
            {
                return Math.Round(((1 - icdoBenefitCalculation.rhic_early_reduction_factor) * icdoBenefitCalculation.unreduced_rhic_amount), 2,MidpointRounding.AwayFromZero);
            }
        }

        public decimal idecReducedRHICFactorinPercentage
        {
            get
            {
                return (icdoBenefitCalculation.rhic_early_reduction_factor * 100); 
            }
        }

        //this variable sets the Member RHIC Amount which will be passed to Benefit Account for Final Calculation
        private decimal _idecMemberRHICAmount;
        public decimal idecMemberRHICAmount
        {
            get { return _idecMemberRHICAmount; }
            set { _idecMemberRHICAmount = value; }
        }

        private decimal _idecSpouseRHICAmount;
        public decimal idecSpouseRHICAmount
        {
            get { return _idecSpouseRHICAmount; }
            set { _idecSpouseRHICAmount = value; }
        }

        //this variable sets the Retirement Orgid From Benefit Application which will be passed to Benefit Account for Final Calculation
        private int _iintRetirementOrgId;
        public int iintRetirementOrgId
        {
            get { return _iintRetirementOrgId; }
            set { _iintRetirementOrgId = value; }
        }


        //this int variable is used to check if the retirement date is equal or not equal to normal or early eligible date.
        private int _iintIsRetirementDateNotEqualToNormalOREalryEligibleDate;
        public int iintIsRetirementDateNotEqualToNormalOREalryEligibleDate
        {
            get
            {
                return _iintIsRetirementDateNotEqualToNormalOREalryEligibleDate;
            }
            set
            {
                _iintIsRetirementDateNotEqualToNormalOREalryEligibleDate = value;
            }
        }

        public int iintFASMonths
        {
            get
            {
                if (iclbBenefitCalculationFASMonths != null)
                {
                    return iclbBenefitCalculationFASMonths.Count;
                }
                return 0;
            }
        }

        private DateTime _idteLastContributedDate;
        public DateTime idteLastContributedDate
        {
            get { return _idteLastContributedDate; }
            set { _idteLastContributedDate = value; }
        }

        public int iintLastSalaryPlanID { get; set; }

        private busPersonEmploymentDetail _ibusPersonEmploymentDtl;
        public busPersonEmploymentDetail ibusPersonEmploymentDtl
        {
            get { return _ibusPersonEmploymentDtl; }
            set { _ibusPersonEmploymentDtl = value; }
        }

        private busBenefitCalculationPersonAccount _ibusBenefitCalculationPersonAccount;
        public busBenefitCalculationPersonAccount ibusBenefitCalculationPersonAccount
        {
            get { return _ibusBenefitCalculationPersonAccount; }
            set { _ibusBenefitCalculationPersonAccount = value; }
        }

        private busPerson _ibusMember;
        public busPerson ibusMember
        {
            get { return _ibusMember; }
            set { _ibusMember = value; }
        }

        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        private busBenefitCalculationFasMonths _ibusFASMonths;
        public busBenefitCalculationFasMonths ibusFASMonths
        {
            get { return _ibusFASMonths; }
            set { _ibusFASMonths = value; }
        }
        private busBenefitApplication _ibusBenefitApplication;
        public busBenefitApplication ibusBenefitApplication
        {
            get { return _ibusBenefitApplication; }
            set { _ibusBenefitApplication = value; }
        }

        private busPersonAccount _ibusPersonAccount;
        public busPersonAccount ibusPersonAccount
        {
            get { return _ibusPersonAccount; }
            set { _ibusPersonAccount = value; }
        }

        private busBenefitProvisionBenefitType _ibusBenefitProvisionBenefitType;
        public busBenefitProvisionBenefitType ibusBenefitProvisionBenefitType
        {
            get { return _ibusBenefitProvisionBenefitType; }
            set { _ibusBenefitProvisionBenefitType = value; }
        }

        private busBenefitProvisionBenefitOption _ibusBenefitProvisionBenefitOption;
        public busBenefitProvisionBenefitOption ibusBenefitProvisionBenefitOption
        {
            get { return _ibusBenefitProvisionBenefitOption; }
            set { _ibusBenefitProvisionBenefitOption = value; }
        }

        private cdoCodeValue _icdoVestingEligibilityData;
        public cdoCodeValue icdoVestingEligibilityData
        {
            get { return _icdoVestingEligibilityData; }
            set { _icdoVestingEligibilityData = value; }
        }

        private busPerson _ibusJointAnnuitant;
        public busPerson ibusJointAnnuitant
        {
            get { return _ibusJointAnnuitant; }
            set { _ibusJointAnnuitant = value; }
        }

        private busBenefitDeductionSummary _ibusBenefitDeductionSummary;
        public busBenefitDeductionSummary ibusBenefitDeductionSummary
        {
            get { return _ibusBenefitDeductionSummary; }
            set { _ibusBenefitDeductionSummary = value; }
        }

        public busPayeeAccount ibusDisabilityPayeeAccount { get; set; }

        private Collection<busBenefitCalculationPersonAccount> _iclbBenefitCalculationPersonAccount;
        public Collection<busBenefitCalculationPersonAccount> iclbBenefitCalculationPersonAccount
        {
            get { return _iclbBenefitCalculationPersonAccount; }
            set { _iclbBenefitCalculationPersonAccount = value; }
        }

        private Collection<busBenefitApplication> _iclbBeneficiariesApplications;
        public Collection<busBenefitApplication> iclbBeneficiariesApplications
        {
            get { return _iclbBeneficiariesApplications; }
            set { _iclbBeneficiariesApplications = value; }
        }

        private Collection<busCodeValue> _iclbEarlyEligibilityData;
        public Collection<busCodeValue> iclbEarlyEligibilityData
        {
            get { return _iclbEarlyEligibilityData; }
            set { _iclbEarlyEligibilityData = value; }
        }

        private Collection<busCodeValue> _iclbNormalEligibilityData;
        public Collection<busCodeValue> iclbNormalEligibilityData
        {
            get { return _iclbNormalEligibilityData; }
            set { _iclbNormalEligibilityData = value; }
        }

        private Collection<busBenefitCalculationOtherDisBenefit> _iclcBenCalcOtherDisBenefit;
        public Collection<busBenefitCalculationOtherDisBenefit> iclcBenCalcOtherDisBenefit
        {
            get { return _iclcBenCalcOtherDisBenefit; }
            set { _iclcBenCalcOtherDisBenefit = value; }
        }

        private Collection<busBenefitCalculationOtherDisBenefit> _iclbBenefitCalculationOtherDisBenefit;
        public Collection<busBenefitCalculationOtherDisBenefit> iclbBenefitCalculationOtherDisBenefit
        {
            get { return _iclbBenefitCalculationOtherDisBenefit; }
            set { _iclbBenefitCalculationOtherDisBenefit = value; }
        }

        private Collection<busBenefitCalculationFasMonths> _iclbBenefitCalculationFASMonths;
        public Collection<busBenefitCalculationFasMonths> iclbBenefitCalculationFASMonths
        {
            get { return _iclbBenefitCalculationFASMonths; }
            set { _iclbBenefitCalculationFASMonths = value; }
        }

        //Loads FAS Months till termination date 12/31/2019
        private Collection<busBenefitCalculationFasMonths> _iclbBenefitCalculationFASMonths2019;
        public Collection<busBenefitCalculationFasMonths> iclbBenefitCalculationFASMonths2019
        {
            get { return _iclbBenefitCalculationFASMonths2019; }
            set { _iclbBenefitCalculationFASMonths2019 = value; }
        }

        //Loads FAS Months after termination date 12/31/2019
        private Collection<busBenefitCalculationFasMonths> _iclbBenefitCalculationFASMonths2020;
        public Collection<busBenefitCalculationFasMonths> iclbBenefitCalculationFASMonths2020
        {
            get { return _iclbBenefitCalculationFASMonths2020; }
            set { _iclbBenefitCalculationFASMonths2020 = value; }
        }

        private Collection<busPersonAccountRetirementContribution> _iclbFinalSalaryRecords;
        public Collection<busPersonAccountRetirementContribution> iclbFinalSalaryRecords
        {
            get { return _iclbFinalSalaryRecords; }
            set { _iclbFinalSalaryRecords = value; }
        }

        private Collection<busBenefitFasIndexing> _iclbBenefitFasIndexing;
        public Collection<busBenefitFasIndexing> iclbBenefitFasIndexing
        {
            get { return _iclbBenefitFasIndexing; }
            set { _iclbBenefitFasIndexing = value; }
        }

        private Collection<busBenefitMultiplier> _iclbBenefitMultiplier;
        public Collection<busBenefitMultiplier> iclbBenefitMultiplier
        {
            get { return _iclbBenefitMultiplier; }
            set { _iclbBenefitMultiplier = value; }
        }

        private Collection<busBenefitRHICOption> _iclbBenefitRHICOption;
        public Collection<busBenefitRHICOption> iclbBenefitRHICOption
        {
            get { return _iclbBenefitRHICOption; }
            set { _iclbBenefitRHICOption = value; }
        }

        private Collection<busBenefitCalculationOptions> _iclbBenefitCalculationOptions;
        public Collection<busBenefitCalculationOptions> iclbBenefitCalculationOptions
        {
            get { return _iclbBenefitCalculationOptions; }
            set { _iclbBenefitCalculationOptions = value; }
        }

        private Collection<busBenefitCalculationPayee> _iclbBenefitCalculationPayee;
        public Collection<busBenefitCalculationPayee> iclbBenefitCalculationPayee
        {
            get { return _iclbBenefitCalculationPayee; }
            set { _iclbBenefitCalculationPayee = value; }
        }

        private Collection<busBenefitServicePurchase> _iclbBenefitServicePurchase;
        public Collection<busBenefitServicePurchase> iclbBenefitServicePurchase
        {
            get { return _iclbBenefitServicePurchase; }
            set { _iclbBenefitServicePurchase = value; }
        }

        private Collection<busBenefitServicePurchase> _iclbBenefitServicePurchaseAll;
        public Collection<busBenefitServicePurchase> iclbBenefitServicePurchaseAll
        {
            get { return _iclbBenefitServicePurchaseAll; }
            set { _iclbBenefitServicePurchaseAll = value; }
        }

        private Collection<busServicePurchaseHeader> _iclbServicePurchaseHeader;
        public Collection<busServicePurchaseHeader> iclbServicePurchaseHeader
        {
            get { return _iclbServicePurchaseHeader; }
            set { _iclbServicePurchaseHeader = value; }
        }

        private Collection<busBenefitPayeeTaxWithholding> _iclbBenefitPayeeTaxWithholding;
        public Collection<busBenefitPayeeTaxWithholding> iclbBenefitPayeeTaxWithholding
        {
            get { return _iclbBenefitPayeeTaxWithholding; }
            set { _iclbBenefitPayeeTaxWithholding = value; }
        }

        private Collection<busBenefitGhdvDeduction> _iclbBenefitGHDVDeduction;
        public Collection<busBenefitGhdvDeduction> iclbBenefitGHDVDeduction
        {
            get { return _iclbBenefitGHDVDeduction; }
            set { _iclbBenefitGHDVDeduction = value; }
        }

        private Collection<busBenefitLifeDeduction> _iclbBenefitLifeDeduction;
        public Collection<busBenefitLifeDeduction> iclbBenefitLifeDeduction
        {
            get { return _iclbBenefitLifeDeduction; }
            set { _iclbBenefitLifeDeduction = value; }
        }

        private Collection<busBenefitLtcDeduction> _iclbBenefitLTCDeduction;
        public Collection<busBenefitLtcDeduction> iclbBenefitLTCDeduction
        {
            get { return _iclbBenefitLTCDeduction; }
            set { _iclbBenefitLTCDeduction = value; }
        }

        private Collection<busBenefitFasIndexing> _iclbBenefitFASIndexing;
        public Collection<busBenefitFasIndexing> iclbBenefitFASIndexing
        {
            get { return _iclbBenefitFASIndexing; }
            set { _iclbBenefitFASIndexing = value; }
        }

        private Collection<busBenefitDeductionSummary> _iclbBenefitDeductionSummary;
        public Collection<busBenefitDeductionSummary> iclbBenefitDeductionSummary
        {
            get { return _iclbBenefitDeductionSummary; }
            set { _iclbBenefitDeductionSummary = value; }
        }

        private Collection<busBenefitCalculationError> _iclbBenefitCalculationError;
        public Collection<busBenefitCalculationError> iclbBenefitCalculationError
        {
            get { return _iclbBenefitCalculationError; }
            set { _iclbBenefitCalculationError = value; }
        }

        # region cor UCS- 080
        //public decimal idecNormalEligibilityAge { get; set; }
        public DateTime idtFirstDayOfMonthNormalEligibilityDate { get; set; }
        # endregion

        public busPayeeAccount ibuspre_RTW_Payee_Account { get; set; }

        public void LoadPreRTWPayeeAccount()
        {
            if (ibuspre_RTW_Payee_Account == null)
                ibuspre_RTW_Payee_Account = new busPayeeAccount();
            ibuspre_RTW_Payee_Account.FindPayeeAccount(icdoBenefitCalculation.pre_rtw_payee_account_id);
        }

        //PIR 18053
        public busBenefitAccount ibuspre_RTW_Benefit_Account { get; set; }

        public void LoadBenefitAccountRTW(int aintBenefitAccountID)
        {
            if (ibuspre_RTW_Benefit_Account == null)
                ibuspre_RTW_Benefit_Account = new busBenefitAccount();
            ibuspre_RTW_Benefit_Account.FindBenefitAccount(aintBenefitAccountID);
        }

        public int iintTermCertainMonths { get; set; }

        public bool IsTermCertainBenefitOption()
        {
            if ((icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption10YearCertain) ||
               (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption15YearCertain) ||
               (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption20YearCertain) ||
               (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption5YearTermLife))
                return true;
            return false;
        }

        public busPayeeAccount ibusOriginatingPayeeAccount { get; set; }

        public void LoadOriginatingPayeeAccount()
        {
            if (ibusOriginatingPayeeAccount.IsNull())
                ibusOriginatingPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            ibusOriginatingPayeeAccount.FindPayeeAccount(icdoBenefitCalculation.originating_payee_account_id);
        }

        public busPerson ibusBeneficiaryPerson { get; set; }

        public void LoadBeneficiaryPerson()
        {
            if (ibusBeneficiaryPerson.IsNull())
                ibusBeneficiaryPerson = new busPerson { icdoPerson = new cdoPerson() };
            ibusBeneficiaryPerson.FindPerson(icdoBenefitCalculation.beneficiary_person_id);
        }

        public busRetirementBenefitCalculation ibusParentBenefitCalculation { get; set; }

        public void LoadParentBenefitCalculation()
        {
            if (ibusParentBenefitCalculation.IsNull())
                ibusParentBenefitCalculation = new busRetirementBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            ibusParentBenefitCalculation.FindBenefitCalculation(icdoBenefitCalculation.parent_benefit_calculation_id);
        }

        public string IsServicePurchaseExists
        {
            get
            {
                if (_iclbBenefitServicePurchaseAll.Count > 0)
                    return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }
        public bool iblnIsActualTerminationDate { get; set; }//PIR 23048
    }
}
