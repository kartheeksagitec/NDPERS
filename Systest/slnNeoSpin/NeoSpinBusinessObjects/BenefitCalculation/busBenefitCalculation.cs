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
using NeoSpin.DataObjects;
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitCalculation : busBenefitCalculationGen
    {
        public bool IsMemberMarried()
        {
            if (ibusMember == null)
                LoadMember();
            if (ibusMember.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusMarried)
                return true;
            return false;
        }

        public bool IsMemberVested()
        {
            bool lblnIsVested = false;
            if (ibusPlan == null)
                LoadPlan();
            if (!iblnConsoldatedVSCLoaded)
                CalculateConsolidatedVSC();
            if (idecMemberAgeBasedOnRetirementDate == 0M)
                CalculateMemberAge();
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            lblnIsVested = CheckIsPersonVested(icdoBenefitCalculation.plan_id, ibusPlan.icdoPlan.plan_code, ibusPlan.icdoPlan.benefit_provision_id,
                                    icdoBenefitCalculation.benefit_account_type_value, icdoBenefitCalculation.credited_vsc,
                                    idecMemberAgeBasedOnRetirementDate, icdoBenefitCalculation.date_of_death,
                                    (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate) ? true : false,
                                    icdoBenefitCalculation.termination_date,
                                    ibusPersonAccount, iobjPassInfo);
            return lblnIsVested;
        }

        public decimal idecDNROPercentageIncrease
        {
            get
            {
                //PIR: 2038. In case of RTW DNRo Increase is equal to DNRo Monthly Increase / Actuarially Adjusted Monthly Single Life benefit
                //The Value of actuarially_adjusted_monthly_single_life_benefit will be populated only in case of RTW.
                if (icdoBenefitCalculation.actuarially_adjusted_monthly_single_life_benefit != 0M)
                {
                    return Math.Round(icdoBenefitCalculation.dnro_monthly_increase / icdoBenefitCalculation.actuarially_adjusted_monthly_single_life_benefit, 4);
                }
                else if (icdoBenefitCalculation.unreduced_benefit_amount != 0M)
                {
                    return Math.Round(icdoBenefitCalculation.dnro_monthly_increase / icdoBenefitCalculation.unreduced_benefit_amount, 4);
                }
                return 0M;
            }
        }

        #region Properties for Correspondence

        public int Judges
        {
            get
            {
                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdJudges)
                    return 1;
                else
                    return 0;
            }
        }

        public int HP
        {
            get
            {
                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdHP)
                    return 1;
                else
                    return 0;
            }
        }

        public int NG
        {
            get
            {
                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdNG)
                    return 1;
                else
                    return 0;
            }
        }

        public int Main
        {
            get
            {
                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdMain)
                    return 1;
                else
                    return 0;
            }
        }

        public int LE
        {
            get
            {
                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdLE)
                    return 1;
                else
                    return 0;
            }
        }
        //pir 7943 ?
        public int BCILE
        {
            get
            {
                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdBCILawEnf)
                    return 1;
                else
                    return 0;
            }
        }
        //PIR 25729
        public int StateLE
        {
            get
            {
                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdStatePublicSafety)
                    return 1;
                else
                    return 0;
            }
        }

        public int Main2020 //PIR 20232
        {
            get
            {
                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdMain2020)
                    return 1;
                else
                    return 0;
            }
        }
        public int DC2020 //PIR 20232
        {
            get
            {
                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020)
                    return 1;
                else
                    return 0;
            }
        }
        public int MainorLEorNG
        {
            get
            {
                return (Main == 1 || Main2020 == 1 || LE == 1 || NG == 1 || BCILE == 1 || StateLE == 1 ? 1 : 0); //PIR 20232,25729
            }
        }

        public int JudgesorHP
        {
            get
            {
                return (Judges == 1 || HP == 1 ? 1 : 0);
            }
        }

        //PIR 14111 : Added property to check Married.
        public int IsMarried  
        {
            get
            {
                if (ibusMember.icdoPerson.marital_status_value.ToUpper() == "MRID")
                    return 1;
                else
                    return 0;
            }
        }

        public int SSLI
        {
            get
            {
                if (icdoBenefitCalculation.uniform_income_or_ssli_flag.ToUpper() == "Y")
                    return 1;
                else
                    return 0;
            }
        }

        public int PLSO
        {
            get
            {
                if (icdoBenefitCalculation.plso_requested_flag.ToUpper() == "Y")
                    return 1;
                else
                    return 0;
            }
        }

        public int QDRO
        {
            get
            {
                return (icdoBenefitCalculation.qdro_amount == 0M ? 0 : 1);
            }
        }

        public int SSLIAndPLSO
        {
            get
            {
                return (SSLI == 1 && PLSO == 1 ? 1 : 0);
            }
        }

        public int NotSSLIAndPLSO
        {
            get
            {
                return (SSLI == 0 && PLSO == 1 ? 1 : 0);
            }
        }

        public decimal idecRoundedYrsOfService
        {
            get
            {
                if (icdoBenefitCalculation.consolidated_psc_in_years > 0.00M)
                {
                    return Math.Round(icdoBenefitCalculation.consolidated_psc_in_years, 4, MidpointRounding.AwayFromZero);
                }
                return 0.00M;
            }
        }
        private cdoBenefitProvision _icdoBenefitProvision;
        public cdoBenefitProvision icdoBenefitProvision
        {
            get { return _icdoBenefitProvision; }
            set { _icdoBenefitProvision = value; }
        }

        private Collection<cdoBenefitProvisionMultiplier> _iclbBenefitProvisionMultiplier;
        public Collection<cdoBenefitProvisionMultiplier> iclbBenefitProvisionMultiplier
        {
            get { return _iclbBenefitProvisionMultiplier; }
            set { _iclbBenefitProvisionMultiplier = value; }
        }

        public decimal idecFirstYrs
        {
            get
            {
                if (iclbBenefitProvisionMultiplier == null)
                    LoadBenefitProvisionMultiplier();
                if(icdoBenefitCalculation.plan_id== busConstant.PlanIdBCILawEnf)
                {
                    iclbBenefitProvisionMultiplier = iclbBenefitProvisionMultiplier.OrderByDescending(i => i.effective_date).Where(i => i.effective_date <= icdoBenefitCalculation.retirement_date && i.benefit_account_type_value==busConstant.PlanBenefitTypeRetirement).ToList().ToCollection();
                }
                if (iclbBenefitMultiplier.IsNull())
                    LoadBenefitMultiplier();
                foreach (cdoBenefitProvisionMultiplier lcdoProvision in iclbBenefitProvisionMultiplier)
                {
                    if (lcdoProvision.tier_number == 1 && idecDisabilityBenefitPercentage == lcdoProvision.multipier_percentage)
                        return Math.Round(lcdoProvision.multipier_percentage, 2, MidpointRounding.AwayFromZero);
                }
                return 0.0M;
            }
        }

        public decimal idecNextYrs
        {
            get
            {
                if (iclbBenefitProvisionMultiplier == null)
                    LoadBenefitProvisionMultiplier();
                foreach (cdoBenefitProvisionMultiplier lcdoProvision in iclbBenefitProvisionMultiplier)
                {
                    if (lcdoProvision.tier_number == 2)
                        return lcdoProvision.multipier_percentage_formatted;
                }
                return 0.0M;
            }
        }

        public decimal idecLastYrs
        {
            get
            {
                if (iclbBenefitProvisionMultiplier == null)
                    LoadBenefitProvisionMultiplier();
                foreach (cdoBenefitProvisionMultiplier lcdoProvision in iclbBenefitProvisionMultiplier)
                {
                    if (lcdoProvision.tier_number == 3)
                        return Math.Round(lcdoProvision.multipier_percentage, 4, MidpointRounding.AwayFromZero);
                }
                return 0.0M;
            }
        }

        public string istrRetirementSubType
        {
            get
            {
                if (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal)
                    return icdoBenefitCalculation.benefit_account_sub_type_description + " (Unreduced)";
                else if (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
                    return icdoBenefitCalculation.benefit_account_sub_type_description + " (Reduced)";
                else if (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO)
                    return icdoBenefitCalculation.benefit_account_sub_type_description + " (Increased)";
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Function to load Deduction Summary business object with values
        /// </summary>
        public busBenefitDeductionSummary AssignDefaultDeductionSummaryValues()
        {
            busBenefitDeductionSummary lobjDedSummary = new busBenefitDeductionSummary();
            lobjDedSummary.icdoBenefitDeductionSummary = new cdoBenefitDeductionSummary();
            lobjDedSummary.icdoBenefitDeductionSummary.benefit_deduction_summary_id = -1;
            lobjDedSummary.icdoBenefitDeductionSummary.gross_monthly_benefit_amount = 0.0M; //need to calculate as said in UID,Pg 18
            lobjDedSummary.icdoBenefitDeductionSummary.rhic_overridden_amount = 0.0M;//need to bring in the rhic amt           
            return lobjDedSummary;
        }

        public decimal idecNetPLSO
        {
            get
            {
                if (ibusBenefitDeductionSummary == null)
                    LoadBenefitDeductionSummary();
                return (icdoBenefitCalculation.plso_lumpsum_amount -
                        (//icdoBenefitCalculation.taxable_amount + icdoBenefitCalculation.non_taxable_amount +
                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.fit_overridden_amount +
                        ibusBenefitDeductionSummary.icdoBenefitDeductionSummary.ndit_overridden_amount));
            }
        }

        public decimal idecNetTaxablePLSO
        {
            get
            {
                return (icdoBenefitCalculation.plso_lumpsum_amount - icdoBenefitCalculation.non_taxable_plso);                        
            }
        }

        #region UCS -056 Correspondence Properties

        //Property to get the person account id for life plan
        private busPersonAccount _ibusPersonAccountForLife;
        public busPersonAccount ibusPersonAccountForLife
        {
            get { return _ibusPersonAccountForLife; }
            set { _ibusPersonAccountForLife = value; }
        }

        //Property to contain all life options for the member
        private Collection<busPersonAccountLifeOption> _iclbPersonAccountLifeOption;
        public Collection<busPersonAccountLifeOption> iclbPersonAccountLifeOption
        {
            get { return _iclbPersonAccountLifeOption; }
            set { _iclbPersonAccountLifeOption = value; }
        }

        //Property to store total Life insurance policy value
        public decimal idecLifeInsurancePolicyValue
        {
            get
            {
                decimal ldecLifePolicyValue = 0.0M;
                foreach (busPersonAccountLifeOption lobjLifeOption in iclbPersonAccountLifeOption)
                {
                    ldecLifePolicyValue += lobjLifeOption.icdoPersonAccountLifeOption.coverage_amount;
                }
                return Math.Round(ldecLifePolicyValue, MidpointRounding.AwayFromZero);
            }
        }

        #endregion

        #endregion

        public void LoadMember()
        {
            if (ibusMember == null)
            {
                ibusMember = new busPerson();
            }
            ibusMember.FindPerson(icdoBenefitCalculation.person_id);
        }

        public void LoadPlan()
        {
            if (ibusPlan == null)
            {
                ibusPlan = new busPlan();
            }
            ibusPlan.FindPlan(icdoBenefitCalculation.plan_id);
        }

        public void LoadPersonAccount()
        {
            if (ibusPersonAccount == null)
                ibusPersonAccount = new busPersonAccount();
            if ((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath) &&
                (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAlternatePayeeDeath))
            {
                if (ibusOriginatingPayeeAccount.IsNull())
                    LoadOriginatingPayeeAccount();
                if (ibusOriginatingPayeeAccount.ibusDROApplication.IsNull())
                    ibusOriginatingPayeeAccount.LoadDROApplication();
                // For DRO Calculations, the Person Account wont be Active. Hence the Person Account in DRO applications has to be loaded.
                ibusPersonAccount.FindPersonAccount(ibusOriginatingPayeeAccount.ibusDROApplication.icdoBenefitDroApplication.person_account_id);
            }
            else
            {
                if (ibusMember == null)
                    LoadMember();
                ibusPersonAccount = ibusMember.LoadActivePersonAccountByPlan(icdoBenefitCalculation.plan_id);
            }
        }

        public void LoadBenefitApplication()
        {
            if (ibusBenefitApplication == null)
                ibusBenefitApplication = new busBenefitApplication();
            ibusBenefitApplication.FindBenefitApplication(icdoBenefitCalculation.benefit_application_id);
        }

        public void LoadBenefitCalculationOtherDisBenefit()
        {
            if (iclbBenefitCalculationOtherDisBenefit == null)
                iclbBenefitCalculationOtherDisBenefit = new Collection<busBenefitCalculationOtherDisBenefit>();
            DataTable ldtbResult = Select<cdoBenefitCalculationOtherDisBenefit>(new string[1] { "benefit_calculation_id" },
                                                                         new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                         null, null);
            iclbBenefitCalculationOtherDisBenefit = GetCollection<busBenefitCalculationOtherDisBenefit>(ldtbResult, "icdoBenefitCalculationOtherDisBenefit");
        }

        public void LoadBenefitPayeeTaxWithholding()
        {
            if (iclbBenefitPayeeTaxWithholding == null)
                iclbBenefitPayeeTaxWithholding = new Collection<busBenefitPayeeTaxWithholding>();
            DataTable ldtbResult = Select<cdoBenefitPayeeTaxWithholding>(new string[1] { "benefit_calculation_id" },
                                                                         new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                         null, null);
            iclbBenefitPayeeTaxWithholding = GetCollection<busBenefitPayeeTaxWithholding>(ldtbResult, "icdoBenefitPayeeTaxWithholding");
        }

        public void LoadBenefitGhdvDeduction()
        {
            if (iclbBenefitGHDVDeduction == null)
                iclbBenefitGHDVDeduction = new Collection<busBenefitGhdvDeduction>();
            DataTable ldtbResult = Select<cdoBenefitGhdvDeduction>(new string[1] { "benefit_calculation_id" },
                                                                         new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                         null, null);
            iclbBenefitGHDVDeduction = GetCollection<busBenefitGhdvDeduction>(ldtbResult, "icdoBenefitGhdvDeduction");
        }

        public void LoadBenefitLifeDeduction()
        {
            if (iclbBenefitLifeDeduction == null)
                iclbBenefitLifeDeduction = new Collection<busBenefitLifeDeduction>();
            DataTable ldtbResult = Select<cdoBenefitLifeDeduction>(new string[1] { "benefit_calculation_id" },
                                                                         new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                         null, null);
            iclbBenefitLifeDeduction = GetCollection<busBenefitLifeDeduction>(ldtbResult, "icdoBenefitLifeDeduction");
        }

        public void LoadBenefitLTCDeduction()
        {
            if (iclbBenefitLTCDeduction == null)
                iclbBenefitLTCDeduction = new Collection<busBenefitLtcDeduction>();
            DataTable ldtbResult = Select<cdoBenefitLtcDeduction>(new string[1] { "benefit_calculation_id" },
                                                                         new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                         null, null);
            iclbBenefitLTCDeduction = GetCollection<busBenefitLtcDeduction>(ldtbResult, "icdoBenefitLtcDeduction");
        }

        public void LoadBenefitFasIndexing()
        {
            if (iclbBenefitFASIndexing == null)
                iclbBenefitFASIndexing = new Collection<busBenefitFasIndexing>();
            DataTable ldtbResult = Select<cdoBenefitFasIndexing>(new string[1] { "benefit_calculation_id" },
                                                                         new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                         null, null);
            iclbBenefitFASIndexing = GetCollection<busBenefitFasIndexing>(ldtbResult, "icdoBenefitFasIndexing");
        }

        public void LoadBenefitDeductionSummaryToDelete()
        {
            if (iclbBenefitDeductionSummary == null)
                iclbBenefitDeductionSummary = new Collection<busBenefitDeductionSummary>();
            DataTable ldtbResult = Select<cdoBenefitDeductionSummary>(new string[1] { "benefit_calculation_id" },
                                                                         new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                         null, null);
            iclbBenefitDeductionSummary = GetCollection<busBenefitDeductionSummary>(ldtbResult, "icdoBenefitDeductionSummary");
        }

        public void LoadBenefitDeductionSummary()
        {
            if (ibusBenefitDeductionSummary == null)
                ibusBenefitDeductionSummary = new busBenefitDeductionSummary();
            if (!ibusBenefitDeductionSummary.FindBenefitDeductionSummaryByCalId(icdoBenefitCalculation.benefit_calculation_id))
                ibusBenefitDeductionSummary = AssignDefaultDeductionSummaryValues();
        }

        public void LoadDeduction()
        {
            if (ibusBenefitDeductionSummary == null)
                ibusBenefitDeductionSummary = new busBenefitDeductionSummary();
            ibusBenefitDeductionSummary.FindBenefitDeductionSummaryByCalId(icdoBenefitCalculation.benefit_calculation_id);
        }

        public void LoadDisabilityPayeeAccount()
        {
            if (ibusDisabilityPayeeAccount == null)
                ibusDisabilityPayeeAccount = new busPayeeAccount();
            ibusDisabilityPayeeAccount.FindPayeeAccount(icdoBenefitCalculation.disability_payee_account_id);
        }

        public void LoadBenefitCalculationError()
        {
            if (iclbBenefitCalculationError == null)
                iclbBenefitCalculationError = new Collection<busBenefitCalculationError>();
            DataTable ldtbResult = Select<cdoBenefitCalculationError>(new string[1] { "benefit_calculation_id" },
                                                                         new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                         null, null);
            iclbBenefitCalculationError = GetCollection<busBenefitCalculationError>(ldtbResult, "icdoBenefitCalculationError");
        }

        public void LoadBenefitRHICOptionFromDB()
        {
            if (iclbBenefitRHICOption == null)
                iclbBenefitRHICOption = new Collection<busBenefitRHICOption>();
            DataTable ldtbResult = Select<cdoBenefitRhicOption>(new string[1] { "benefit_calculation_id" },
                                                                         new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                         null, null);
            iclbBenefitRHICOption = GetCollection<busBenefitRHICOption>(ldtbResult, "icdoBenefitRhicOption");
        }

        public void LoadBenefitProvisionBenefitType()
        {
            LoadBenefitProvisionBenefitType(icdoBenefitCalculation.retirement_date);
        }

        public void LoadBenefitProvisionBenefitType(DateTime adteGivenDate)
        {
            if (ibusBenefitProvisionBenefitType == null)
            {
                ibusBenefitProvisionBenefitType = new busBenefitProvisionBenefitType();
                ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType = new cdoBenefitProvisionBenefitType();
            }
            if (adteGivenDate != DateTime.MinValue)
            {
				//PIR 14646 - Benefit Tier Changes
                string lstrBenefitTierValue = GetBenefitTierValue();
                DataTable ldtbResult = busBase.Select("cdoBenefitProvisionBenefitType.GetBenefitProvisionByPlan", new object[4]{
                                                    icdoBenefitCalculation.plan_id,
                                                    icdoBenefitCalculation.benefit_account_type_value,
                                                    adteGivenDate, lstrBenefitTierValue});
                if (ldtbResult.Rows.Count > 0)
                {
                    ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.LoadData(ldtbResult.Rows[0]);
                }
            }
        }
        public void LoadBenefitProvisionBenefitType(DataTable adtBenProvisionBenType)
        {
            if (ibusBenefitProvisionBenefitType == null)
            {
                ibusBenefitProvisionBenefitType = new busBenefitProvisionBenefitType();
                ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType = new cdoBenefitProvisionBenefitType();
            }
            if (icdoBenefitCalculation.retirement_date != DateTime.MinValue)
            {
				//PIR 14646 - Benefit Tier Changes
                string lstrBenefitTierValue = GetBenefitTierValue();
                DataTable ldtbResult = adtBenProvisionBenType.AsEnumerable().Where(i => i.Field<int>("plan_id") == icdoBenefitCalculation.plan_id
                                                     && i.Field<string>("benefit_account_type_value") == icdoBenefitCalculation.benefit_account_type_value
                                                     && i.Field<DateTime>("EFFECTIVE_DATE") <= icdoBenefitCalculation.retirement_date
                                                     && i.Field<string>("BENEFIT_TIER_VALUE") == (string.IsNullOrEmpty(lstrBenefitTierValue) ? null : lstrBenefitTierValue)
                                                     ).OrderByDescending(o => o.Field<DateTime>("EFFECTIVE_DATE")).AsDataTable();

                if (ldtbResult.Rows.Count > 0)
                {

                    ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.LoadData(ldtbResult.Rows[0]);
                }
            }
        }
		//PIR 14646 - Benefit Tier Changes
        private string GetBenefitTierValue()
        {
            string lstrBenefitTierValue = string.Empty;

            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement();
            lbusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);

            if (icdoBenefitCalculation.plan_id == busConstant.PlanIdMain)
            {
                lstrBenefitTierValue = string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) ? busConstant.MainBenefit1997Tier :
                    lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
            }
            //PIR 26282
            else if (icdoBenefitCalculation.plan_id == busConstant.PlanIdBCILawEnf)
            {
                lstrBenefitTierValue = string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) ? busConstant.BCIBenefit2011Tier :
                    lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
            }
            //PIR 26544
            else if (lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value.IsNotNullOrEmpty())
                lstrBenefitTierValue = lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;

            return lstrBenefitTierValue;
        }

        public void LoadBenefitProvisionBenefitOption(string astrBenefitTypeValue, string astrBenefitOptionValue,
                                                                            DateTime adteRetirementDate, int aintBenefitProvisionID)
        {
            if (ibusBenefitProvisionBenefitOption == null)
            {
                ibusBenefitProvisionBenefitOption = new busBenefitProvisionBenefitOption();
                ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption = new cdoBenefitProvisionBenefitOption();
            }
            if (icdoBenefitCalculation.retirement_date != DateTime.MinValue)
            {
                DataTable ldbtResult = SelectWithOperator<cdoBenefitProvisionBenefitOption>(
                                new string[4] { "BENEFIT_ACCOUNT_TYPE_VALUE", "BENEFIT_OPTION_VALUE", "EFFECTIVE_DATE", "BENEFIT_PROVISION_ID" },
                                new string[4] { "=", "=", "<=", "=" },
                                new object[4] { astrBenefitTypeValue, astrBenefitOptionValue, adteRetirementDate, aintBenefitProvisionID }, "EFFECTIVE_DATE DESC");
                if (ldbtResult.Rows.Count > 0)
                    ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.LoadData(ldbtResult.Rows[0]);
            }
        }

        public void LoadBenefitProvisionBenefitOption()
        {
            if (ibusBenefitProvisionBenefitType == null)
                LoadBenefitProvisionBenefitType();
            LoadBenefitProvisionBenefitOption(icdoBenefitCalculation.benefit_account_type_value, icdoBenefitCalculation.benefit_option_value,
                                                icdoBenefitCalculation.retirement_date, ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_provision_id);
        }
        public void LoadBenefitCalculationPersonAccount()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();

            if (iclbBenefitCalculationPersonAccount == null)
                iclbBenefitCalculationPersonAccount = new Collection<busBenefitCalculationPersonAccount>();
            DataTable ldtbResult = Select<cdoBenefitCalculationPersonAccount>(new string[1] { "benefit_calculation_id" },
                                                                                new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                                null, "benefit_calculation_person_account_id desc");
            foreach (DataRow ldtr in ldtbResult.Rows)
            {
                busBenefitCalculationPersonAccount lobjBenCalcPersonAccount = new busBenefitCalculationPersonAccount
                {
                    icdoBenefitCalculationPersonAccount = new cdoBenefitCalculationPersonAccount()
                };
                lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.LoadData(ldtr);

                if (ibusPersonAccount.icdoPersonAccount.person_account_id != lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.person_account_id)
                {
                    if (lobjBenCalcPersonAccount.ibusPersonAccount.IsNull())
                        lobjBenCalcPersonAccount.LoadPersonAccount();
                    if (lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.payee_account_id > 0)
                    {
                        lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.istrUse = busConstant.Flag_Yes_Value;
                        //Since User cannot select the Person account as it is obsolete. It is set as "Y" Always.

                        lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.is_person_account_selected_flag = busConstant.Flag_Yes;
                    }
                    iclbBenefitCalculationPersonAccount.Add(lobjBenCalcPersonAccount);
                }
            }
        }
        // This Method is Loaded in order to Delete the Benefit Calculation 
        public void LoadBenefitCalculationPersonAccountFromDB()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();

            if (iclbBenefitCalculationPersonAccount == null)
                iclbBenefitCalculationPersonAccount = new Collection<busBenefitCalculationPersonAccount>();
            DataTable ldtbResult = Select<cdoBenefitCalculationPersonAccount>(new string[1] { "benefit_calculation_id" },
                                                                                new object[1] { icdoBenefitCalculation.benefit_calculation_id },
                                                                                null, "benefit_calculation_person_account_id desc");
            foreach (DataRow ldtr in ldtbResult.Rows)
            {
                busBenefitCalculationPersonAccount lobjBenCalcPersonAccount = new busBenefitCalculationPersonAccount
                {
                    icdoBenefitCalculationPersonAccount = new cdoBenefitCalculationPersonAccount()
                };
                lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.LoadData(ldtr);
                iclbBenefitCalculationPersonAccount.Add(lobjBenCalcPersonAccount);
            }
        }

        // Load the Joint Annuitant in case of Estimate Calculation
        public void LoadJointAnnuitant()
        {
            if (ibusJointAnnuitant == null)
                ibusJointAnnuitant = new busPerson { icdoPerson = new cdoPerson() };
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments ||//to handle when RecalculateBenefit is clicked from PA
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequent ||
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)//PIR 18053
            {
                if (icdoBenefitCalculation.annuitant_id != 0)
                    ibusJointAnnuitant.FindPerson(icdoBenefitCalculation.annuitant_id);
            }
            else
            {
                if (ibusMember == null)
                    LoadMember();
                if (ibusMember.icolPersonContact == null)
                    ibusMember.LoadContacts();
                foreach (busPersonContact lobjPersonContact in ibusMember.icolPersonContact)
                {
                    if ((lobjPersonContact.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse)
                            && (lobjPersonContact.icdoPersonContact.status_value.Trim() == busConstant.PersonContactStatusActive.Trim()))
                    {
                        if (lobjPersonContact.icdoPersonContact.contact_person_id != 0)
                        {
                            ibusJointAnnuitant.FindPerson(lobjPersonContact.icdoPersonContact.contact_person_id);
                            break;
                        }
                        else
                        {
                            ibusJointAnnuitant.icdoPerson.first_name = lobjPersonContact.icdoPersonContact.contact_name;
                            break;
                        }
                    }
                }
            }
        }

        //PIRs 15600
        public void LoadRHICEffectiveDate()
        {
            DateTime ldtNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
            if (icdoBenefitCalculation.retirement_date != DateTime.MinValue)
            {
                if (icdoBenefitCalculation.retirement_date.AddMonths(1).Date == ldtNextBenefitPaymentDate.Date ||
                    icdoBenefitCalculation.retirement_date.Date == ldtNextBenefitPaymentDate.Date)
                    icdoBenefitCalculation.rhic_effective_date = icdoBenefitCalculation.retirement_date;
            }
        }

        public void LoadBenefitRHICOption()
        {
            if (iclbBenefitRHICOption == null)
                iclbBenefitRHICOption = new Collection<busBenefitRHICOption>();
            DataTable ldtbResult = Select<cdoBenefitRhicOption>(new string[1] { "benefit_calculation_id" },
                                        new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            if (icdoBenefitCalculation.is_created_from_portal == busConstant.Flag_Yes)
            {
                // PROD PIR ID 7009
                iclbBenefitRHICOption = new Collection<busBenefitRHICOption>();
                Collection<busBenefitRHICOption> tempClc = new Collection<busBenefitRHICOption>();
                tempClc = GetCollection<busBenefitRHICOption>(ldtbResult, "icdoBenefitRhicOption");
                var lenumString = tempClc.Select(i => i.icdoBenefitRhicOption.rhic_option_value).Distinct();
                foreach (string lstr in lenumString)
                {
                    iclbBenefitRHICOption.Add(tempClc.Where(i => i.icdoBenefitRhicOption.rhic_option_value == lstr).FirstOrDefault());
                }
                iclbBenefitRHICOption.ForEach(i => i.icdoBenefitRhicOption.istrBenefitOptionData2 = busGlobalFunctions.GetData2ByCodeValue(1905, i.icdoBenefitRhicOption.rhic_option_value, iobjPassInfo));
            }
            else
            {
                iclbBenefitRHICOption = GetCollection<busBenefitRHICOption>(ldtbResult, "icdoBenefitRhicOption");
            }

            foreach (busBenefitRHICOption lobjBenefitRHICOption in iclbBenefitRHICOption)
            {
                lobjBenefitRHICOption.LoadBenefitProvisionOption();
                //Prod PIR:4789 RHIC property populating while Approving the calculation
                if(icdoBenefitCalculation.calculation_type_value != busConstant.CalculationTypeEstimate ||
                    icdoBenefitCalculation.calculation_type_value != busConstant.CalculationTypeEstimateSubsequent) //PIR 19594
                {
                    idecMemberRHICAmount=lobjBenefitRHICOption.icdoBenefitRhicOption.member_rhic_amount;
                    idecSpouseRHICAmount=lobjBenefitRHICOption.icdoBenefitRhicOption.spouse_rhic_amount;
                }

            }
        }
       
        public void LoadBenefitMultiplier()
        {
            if (iclbBenefitMultiplier == null)
                iclbBenefitMultiplier = new Collection<busBenefitMultiplier>();
            DataTable ldtbResult = Select<cdoBenefitMultiplier>(new string[1] { "benefit_calculation_id" },
                                        new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            iclbBenefitMultiplier = GetCollection<busBenefitMultiplier>(ldtbResult, "icdoBenefitMultiplier");
        }

        public void LoadBenefitCalculationOptions()
        {
            if (iclbBenefitCalculationOptions == null)
                iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
            DataTable ldtbResult = Select<cdoBenefitCalculationOptions>(new string[1] { "benefit_calculation_id" },
                                        new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, "benefit_calculation_options_id"); // PROD PIR ID 7069
            iclbBenefitCalculationOptions = GetCollection<busBenefitCalculationOptions>(ldtbResult, "icdoBenefitCalculationOptions");
            foreach (busBenefitCalculationOptions lobjBenefitoption in iclbBenefitCalculationOptions)
            {
                lobjBenefitoption.LoadBenefitProvisionOption();
                lobjBenefitoption.LoadBenefitCalculationPayee();
                if (lobjBenefitoption.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.payee_person_id != 0)
                {
                    if (lobjBenefitoption.ibusBenefitCalculationPayee.ibusPayee.IsNull())
                        lobjBenefitoption.ibusBenefitCalculationPayee.LoadPayee();
                    lobjBenefitoption.icdoBenefitCalculationOptions.payee_name = lobjBenefitoption.ibusBenefitCalculationPayee.ibusPayee.icdoPerson.PersonIdWithName;
                }
                else
                {
                    if (lobjBenefitoption.ibusBenefitCalculationPayee.ibusPayeeOrg.IsNull())
                        lobjBenefitoption.ibusBenefitCalculationPayee.LoadPayeeOrg();
                    lobjBenefitoption.icdoBenefitCalculationOptions.payee_name = lobjBenefitoption.ibusBenefitCalculationPayee.ibusPayeeOrg.icdoOrganization.org_name;
                }
                lobjBenefitoption.ibusBenefitCalculation = this;
            }
        }

        public DataTable idtbLastSalaryWithPersonAccount { get; set; }
        public void LoadLastSalaryWithPersonAccount()
        {
            idtbLastSalaryWithPersonAccount = busBase.Select("cdoBenefitCalculationFasMonths.LoadLastSalaryRecord",
                                                               new object[3] { ibusPersonAccount.icdoPersonAccount.person_id,
                                                            ibusPersonAccount.icdoPersonAccount.plan_id,
                                                            ibusPersonAccount.icdoPersonAccount.person_account_id });
        }

        public DataTable idtbLastSalaryWithoutPersonAccount { get; set; }
        public void LoadLastSalaryWitouthPersonAccount()
        {
            idtbLastSalaryWithoutPersonAccount = busBase.Select("cdoBenefitCalculationFasMonths.LoadLastSalaryRecord",
                                                new object[3] { ibusPersonAccount.icdoPersonAccount.person_id,
                                                            ibusPersonAccount.icdoPersonAccount.plan_id,0 });
        }

        public void LoadLastContributedDate()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();

            // PIR ID 1920 For RTW refund case, the FAS should not consider the refunded contributions
            DataTable ldtbResult = new DataTable();
            if ((icdoBenefitCalculation.is_rtw_less_than_2years_flag == busConstant.Flag_Yes) &&
                (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper()))
            {
                if (idtbLastSalaryWithPersonAccount == null)
                {
                    LoadLastSalaryWithPersonAccount();
                }
                ldtbResult = idtbLastSalaryWithPersonAccount;
            }
            else
            {
                if (idtbLastSalaryWithoutPersonAccount == null)
                    LoadLastSalaryWitouthPersonAccount();
                ldtbResult = idtbLastSalaryWithoutPersonAccount;
            }
            if (ldtbResult.Rows.Count > 0)
            {
                busPersonAccountRetirementContribution lobjRetirementContribution = new busPersonAccountRetirementContribution();
                lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.LoadData(ldtbResult.Rows[0]);
                //UAT PIR: 1131 Setting the Last Contribution Record Plan ID.
                lobjRetirementContribution.ibusPARetirement = new busPersonAccountRetirement();
                lobjRetirementContribution.ibusPARetirement.icdoPersonAccount = new cdoPersonAccount();
                lobjRetirementContribution.ibusPARetirement.ibusPlan = new busPlan();
                lobjRetirementContribution.ibusPARetirement.ibusPlan.icdoPlan = new cdoPlan();
                lobjRetirementContribution.ibusPARetirement.ibusPlan.icdoPlan.LoadData(ldtbResult.Rows[0]);

                DateTime ldteLastContributionDate = new DateTime();
                ldteLastContributionDate = new DateTime(lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                                                        lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month, 01);
                ldteLastContributionDate = ldteLastContributionDate.AddMonths(1).AddDays(-1);
                iintLastSalaryPlanID = lobjRetirementContribution.ibusPARetirement.ibusPlan.icdoPlan.plan_id;
                SetDualFASTerminationDate();
                if (icdoBenefitCalculation.fas_termination_date != DateTime.MinValue)
                {
					//prod pir:4761: termination date set as last day of the Month to accommodate that month salary too
                    idteLastContributedDate = busGlobalFunctions.GetMin(ldteLastContributionDate, icdoBenefitCalculation.fas_termination_date.GetLastDayofMonth());
                }
            }
        }

        public void CalculateAnnuitantAge()
        {
            decimal ldecAnnuitantAge = 0M;
            if (iclbBenefitCalculationPayee != null)
            {
                if ((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) ||
                    (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement))
                {
                    foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                    {
                        if ((lobjPayee.icdoBenefitCalculationPayee.family_relationship_value == busConstant.FamilyRelationshipSpouse) &&
                            (lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth != DateTime.MinValue))
                            CalculatePersonAge(lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth,
                                icdoBenefitCalculation.retirement_date, ref ldecAnnuitantAge, 4);
                    }
                    if (ldecAnnuitantAge == 0M)
                    {
                        if (ibusJointAnnuitant == null)
                            LoadJointAnnuitant();
                        CalculatePersonAge(ibusJointAnnuitant.icdoPerson.date_of_birth,
                            icdoBenefitCalculation.retirement_date, ref ldecAnnuitantAge, 4);
                    }
                }
                if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                {
                    foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                    {
                        if ((lobjPayee.icdoBenefitCalculationPayee.family_relationship_value == busConstant.FamilyRelationshipSpouse) &&
                            (lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth != DateTime.MinValue))
                            CalculatePersonAge(lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth,
                                icdoBenefitCalculation.date_of_death, ref ldecAnnuitantAge, 4);
                    }
                }
            }
            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
            {
                if (ibusJointAnnuitant.IsNull())
                    LoadJointAnnuitant();
                CalculatePersonAge(ibusJointAnnuitant.icdoPerson.date_of_birth,
                        icdoBenefitCalculation.date_of_death, ref ldecAnnuitantAge, 4);
            }
            icdoBenefitCalculation.annuitant_age = ldecAnnuitantAge;
        }

        public void CalculateMemberAge()
        {
            decimal ldecMemberAge = 0M; decimal ldecMemberAgeAsofDateofDeath = 0M;
            if (ibusMember == null)
                LoadMember();
            iintMembersAgeInMonthsAsOnRetirementDate = CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.retirement_date, ref ldecMemberAge, 4);
            idecMemberAgeBasedOnRetirementDate = ldecMemberAge;

            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                iintMembersAgeInMonthsAsOnRetirementDate = CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.date_of_death, ref ldecMemberAgeAsofDateofDeath, 4);
                idecMemberAgeAsonDateofDeath = ldecMemberAgeAsofDateofDeath;
            }
        }

        //PIR 11734 - Calculate member's age according to Termination date. If no Termination date exists and the member is still employed, then calculated age
        // by Calculation date ie. Statement Effective Date.
        public void CalculateAgeOfMember()
        {
            decimal ldecMemberAge = 0M; decimal ldecMemberAgeAsofDateofDeath = 0M;
            if (ibusMember == null)
                LoadMember();
            if (icdoBenefitCalculation.termination_date != DateTime.MinValue)
            {
                if (icdoBenefitCalculation.termination_date >= ibusPersonAccount.idtMASStatementEffectiveDate)
                {
                    iintMembersAgeInMonthsAsOnRetirementDate = CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, ibusPersonAccount.idtMASStatementEffectiveDate, ref ldecMemberAge, 4);
                    idecMemberAgeBasedOnRetirementDate = ldecMemberAge;
                }
                else if (icdoBenefitCalculation.termination_date < ibusPersonAccount.idtMASStatementEffectiveDate)
                {
                    iintMembersAgeInMonthsAsOnRetirementDate = CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.termination_date, ref ldecMemberAge, 4);
                    idecMemberAgeBasedOnRetirementDate = ldecMemberAge;
                }
            }
            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                iintMembersAgeInMonthsAsOnRetirementDate = CalculatePersonAge(ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.date_of_death, ref ldecMemberAgeAsofDateofDeath, 4);
                idecMemberAgeAsonDateofDeath = ldecMemberAgeAsofDateofDeath;
            }
        }


        /// *** BR-055-42-44 *** Calculate Person Age based on Calculation Type with Retirement Date
        /// Returns the Total number of Months
        /// Referred to the decimal value of Total Year and Months
        public int CalculatePersonAge(DateTime adtePersonDOB, DateTime adteDateToCompare, ref decimal adecMonthAndYear, int aintDecimallength)
        {
            int lintTotalMonths = 0;
            if ((adtePersonDOB != DateTime.MinValue) && (adteDateToCompare != DateTime.MinValue))
            {
                DateTime ldteFromDate = adtePersonDOB.AddMonths(1);
                int lintMemberAgeMonthPart = 0;
                int lintMemberAgeYearPart = 0;
                CalculateAge(ldteFromDate, adteDateToCompare, ref lintTotalMonths, ref adecMonthAndYear,
                    aintDecimallength, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);
            }
            return lintTotalMonths;
        }

        // Approve Button Visibility
        public bool IsCreatedMemberLoggedIn()
        {
            if (icdoBenefitCalculation.created_by.Trim().ToLower() == iobjPassInfo.istrUserID.Trim().ToLower())
                return true;
            return false;
        }

        protected void ResetCalculationObject()
        {
            /* Reset all the EARLY,PLSO and DNRO Related Fields before calculation  */

            icdoBenefitCalculation.dnro_factor = 0;
            icdoBenefitCalculation.dnro_missed_months = 0;
            icdoBenefitCalculation.dnro_monthly_increase = 0;
            icdoBenefitCalculation.dnro_total_missed_amount = 0;
            icdoBenefitCalculation.adhoc_or_cola_amount = 0;
            icdoBenefitCalculation.dnro_percentage_increase = 0;

            icdoBenefitCalculation.early_reduction_factor = 0;
            icdoBenefitCalculation.early_reduced_months = 0;
            icdoBenefitCalculation.early_retirement_percentage_decrease = 0;
            icdoBenefitCalculation.early_monthly_decrease = 0;

            icdoBenefitCalculation.reduced_monthly_after_plso_deduction = 0;
            icdoBenefitCalculation.plso_factor = 0;
            icdoBenefitCalculation.plso_lumpsum_amount = 0;
            icdoBenefitCalculation.plso_reduction_amount = 0;

            /* Reset all the EARLY,PLSO and DNRO Related Fields before calculation  */
        }

        #region Service Purchase

        public void LoadPaidServicePurchase()
        {
            if (ibusMember == null)
                LoadMember();
            if (ibusMember.iclbServicePurchaseHeader == null)
                ibusMember.LoadServicePurchase(false);
            if (iclbServicePurchaseHeader == null)
                iclbServicePurchaseHeader = new Collection<busServicePurchaseHeader>();

            foreach (busServicePurchaseHeader lobjServicePurchaseHeader in ibusMember.iclbServicePurchaseHeader)
            {
                if (lobjServicePurchaseHeader.icdoServicePurchaseHeader.plan_id == icdoBenefitCalculation.plan_id)
                {
                    if ((lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Paid_In_Full) ||
                        (lobjServicePurchaseHeader.icdoServicePurchaseHeader.grant_free_flag == busConstant.Flag_Yes) ||
                        (lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment) ||
                        (lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Pending) 
                        || (lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Approved)  //PIR 10715
                        )
                    {
                        if (lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail == null)
                            lobjServicePurchaseHeader.LoadServicePurchaseDetail();

                        lobjServicePurchaseHeader.idecTotalAllocatedPsc = lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase
                            - lobjServicePurchaseHeader.icdoServicePurchaseHeader.prorated_psc;

                        iclbServicePurchaseHeader.Add(lobjServicePurchaseHeader);
                    }
                }
            }
        }

        public void LoadPaidServicePurchaseForMSS()
        {
            if (ibusMember == null)
                LoadMember();
            if (ibusMember.iclbServicePurchaseHeader == null)
                ibusMember.LoadServicePurchase(false);
            if (iclbServicePurchaseHeader == null)
                iclbServicePurchaseHeader = new Collection<busServicePurchaseHeader>();

            foreach (busServicePurchaseHeader lobjServicePurchaseHeader in ibusMember.iclbServicePurchaseHeader)
            {
                if (lobjServicePurchaseHeader.icdoServicePurchaseHeader.plan_id == icdoBenefitCalculation.plan_id)
                {
                    if ((lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_Paid_In_Full) ||
                        (lobjServicePurchaseHeader.icdoServicePurchaseHeader.grant_free_flag == busConstant.Flag_Yes) ||
                        (lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value == busConstant.Service_Purchase_Action_Status_In_Payment))
                    {
                        if (lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail == null)
                            lobjServicePurchaseHeader.LoadServicePurchaseDetail();

                        lobjServicePurchaseHeader.idecTotalAllocatedPsc = lobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase
                            - lobjServicePurchaseHeader.icdoServicePurchaseHeader.prorated_psc;

                        iclbServicePurchaseHeader.Add(lobjServicePurchaseHeader);
                    }
                }
            }
        }

        public void LoadBenefitServicePurchaseForMSS()
        {
            if (iclbServicePurchaseHeader == null)
                LoadPaidServicePurchaseForMSS();

            if (iclbBenefitServicePurchase == null)
                LoadBenefitServicePurchasebyCalculationID();

            if (iclbBenefitServicePurchaseAll == null)
            {
                iclbBenefitServicePurchaseAll = new Collection<busBenefitServicePurchase>();

                foreach (busServicePurchaseHeader lobjServicePurchaseHeader in iclbServicePurchaseHeader)
                {
                    busBenefitServicePurchase lobjBenefitServicePurchase =
                        LoadBenefitServicePurchaseByPurchaseID(lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id);
                    lobjBenefitServicePurchase.icdoBenefitServicePurchase.remaining_psc = lobjServicePurchaseHeader.idecTotalAllocatedPsc;
                    lobjBenefitServicePurchase.istrActionStatus = lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_description;
                    lobjBenefitServicePurchase.istrServicePurchaseType = lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_description;
                    // UAT PIR ID 1068
                    bool lblnRemainderIncluded = iclbBenefitServicePurchase.Where(o => o.icdoBenefitServicePurchase.service_purchase_header_id ==
                                                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id).Any();
                    if (lblnRemainderIncluded)
                        lobjBenefitServicePurchase.istrIncludeRemainder = busConstant.Flag_Yes;
                    iclbBenefitServicePurchaseAll.Add(lobjBenefitServicePurchase);
                }
            }
        }

        public void LoadBenefitServicePurchase()
        {
            if (iclbServicePurchaseHeader == null)
                LoadPaidServicePurchase();

            //Backlog PIR 10051 - Added count condition
            if (iclbBenefitServicePurchase == null || iclbBenefitServicePurchase.Count == 0)
                LoadBenefitServicePurchasebyCalculationID();

            if (iclbBenefitServicePurchaseAll == null)
            {
                iclbBenefitServicePurchaseAll = new Collection<busBenefitServicePurchase>();

                foreach (busServicePurchaseHeader lobjServicePurchaseHeader in iclbServicePurchaseHeader)
                {
                    busBenefitServicePurchase lobjBenefitServicePurchase =
                        LoadBenefitServicePurchaseByPurchaseID(lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id);                    
                    lobjBenefitServicePurchase.icdoBenefitServicePurchase.remaining_psc = lobjServicePurchaseHeader.idecTotalAllocatedPsc;
                    lobjBenefitServicePurchase.istrActionStatus = lobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_description;
                    lobjBenefitServicePurchase.istrServicePurchaseType = lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_description;
                    // UAT PIR ID 1068
                    bool lblnRemainderIncluded = iclbBenefitServicePurchase.Where(o => o.icdoBenefitServicePurchase.service_purchase_header_id ==
                                                    lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id).Any();
                    if (lblnRemainderIncluded)
                        lobjBenefitServicePurchase.istrIncludeRemainder = busConstant.Flag_Yes;

                    // PIR 9447
                    if (icdoBenefitCalculation.is_created_from_portal == busConstant.Flag_Yes)
                    {
                        if (ibusBenefitCalculatorWeb.IsNull()) LoadBenefitcalculatorWeb();
                        if ((lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave &&
                            ibusBenefitCalculatorWeb.icdoWssBenefitcalculator.unused_service_purchase_selected == busConstant.Flag_Yes) ||
                            (lobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase &&
                            ibusBenefitCalculatorWeb.icdoWssBenefitcalculator.additional_serivce_purchase_selected == busConstant.Flag_Yes))
                            lobjBenefitServicePurchase.istrIncludeRemainder = busConstant.Flag_Yes;
                    }

                    //PIR 10715  -
                    // In Payment should always show. Approved or Pending should only show until the Expiration Date.

                    if (lobjServicePurchaseHeader.icdoServicePurchaseHeader.is_created_from_portal != busConstant.Flag_Yes && 
                        ((lobjBenefitServicePurchase.istrActionStatus.Equals("Approved") || lobjBenefitServicePurchase.istrActionStatus.Equals("Pending")) &&
                        lobjServicePurchaseHeader.icdoServicePurchaseHeader.expiration_date >= DateTime.Now.Date) ||
                        lobjBenefitServicePurchase.istrActionStatus.Equals("In Payment") 
                        ) // PIR 9974
                        iclbBenefitServicePurchaseAll.Add(lobjBenefitServicePurchase);
                }
            }
        }

        private busBenefitServicePurchase LoadBenefitServicePurchaseByPurchaseID(int aintPurchaseID)
        {
            foreach (busBenefitServicePurchase lobjBenefitServicePurchase in iclbBenefitServicePurchase)
            {
                if (lobjBenefitServicePurchase.icdoBenefitServicePurchase.service_purchase_header_id == aintPurchaseID)
                {
                    return lobjBenefitServicePurchase;
                }
            }
            busBenefitServicePurchase lobjBenefitServicePurchase1 = new busBenefitServicePurchase();
            lobjBenefitServicePurchase1.icdoBenefitServicePurchase = new cdoBenefitServicePurchase();
            lobjBenefitServicePurchase1.icdoBenefitServicePurchase.service_purchase_header_id = aintPurchaseID;
            return lobjBenefitServicePurchase1;
        }

        public void LoadBenefitServicePurchasebyCalculationID()
        {
            DataTable ldtbList = Select<cdoBenefitServicePurchase>(
                new string[1] { "benefit_calculation_id" },
                new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            iclbBenefitServicePurchase = GetCollection<busBenefitServicePurchase>(ldtbList, "icdoBenefitServicePurchase");
        }

        public void LoadRemainingServicePurchaseCredit()
        {
            if (iclbBenefitServicePurchaseAll == null)
            {
                LoadBenefitServicePurchase();
            }
            foreach (busBenefitServicePurchase lobjBenefitServicePurchase in iclbBenefitServicePurchaseAll)
            {
                if (lobjBenefitServicePurchase.istrIncludeRemainder == busConstant.Flag_Yes)
                {
                    idecRemainingServiceCredit += lobjBenefitServicePurchase.icdoBenefitServicePurchase.remaining_psc;
                }
            }
        }

        #endregion

        #region Benefit Calculation Payee

        public bool IsPayeeRecordsExist()
        {
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();
            if (iclbBenefitCalculationPayee.Count < 2)
                return true;
            return false;
        }

        public Collection<busBenefitCalculationPayee> iclbBenefitCalculationPayeeFromDB { get; set; }

        public Collection<busBenefitCalculationPayee> LoadBenefitCalculationPayeeFromDB()
        {
            Collection<busBenefitCalculationPayee> lclbBenefitCalculationPayee = new Collection<busBenefitCalculationPayee>();
            DataTable ldtbList = Select<cdoBenefitCalculationPayee>(
                    new string[1] { "benefit_calculation_id" },
                    new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            lclbBenefitCalculationPayee = GetCollection<busBenefitCalculationPayee>(ldtbList, "icdoBenefitCalculationPayee");
            return lclbBenefitCalculationPayee;
        }

        public void LoadBenefitCalculationPayee()
        {
            if (iclbBenefitCalculationPayee == null)
                iclbBenefitCalculationPayee = new Collection<busBenefitCalculationPayee>();
            iclbBenefitCalculationPayee = LoadBenefitCalculationPayeeFromDB();
            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                if (lobjPayee.ibusBenefitApplication.IsNull())
                    lobjPayee.LoadBenefitApplication();
                if (lobjPayee.icdoBenefitCalculationPayee.payee_person_id != 0)
                {
                    if (lobjPayee.ibusPayee.IsNull())
                        lobjPayee.LoadPayee();
                    lobjPayee.icdoBenefitCalculationPayee.payee_name = lobjPayee.ibusPayee.icdoPerson.PersonIdWithName;
                }
                else if (lobjPayee.icdoBenefitCalculationPayee.payee_org_id != 0)
                {
                    if (lobjPayee.ibusPayeeOrg.IsNull())
                        lobjPayee.LoadPayeeOrg();
                    lobjPayee.icdoBenefitCalculationPayee.payee_name = lobjPayee.ibusPayeeOrg.icdoOrganization.org_name;
                }
            }
        }

        #endregion

        #region Other Disability Benefits

        public void LoadSsliAndWsiAmount()
        {
            if (iclcBenCalcOtherDisBenefit == null)
                LoadOtherDisabilityBenefits();
            foreach (busBenefitCalculationOtherDisBenefit lobjOtherDisabilityBenefit in iclcBenCalcOtherDisBenefit)
            {
                if (lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.other_disability_benefit_value == busConstant.OtherDisBenSocialSecurityDisBen)
                {
                    idecSSliBenefitAmount = lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.monthly_benefit_amount;
                }
                else
                {
                    idecWSIBenefitAmount = lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.monthly_benefit_amount;
                }
            }
        }

        //BR-051-67
        //Load Other Disability Benefits 
        //load datatable from code values for the code value Other disability benefits
        //loop through the datatable and create cdo object for Other disability Benefits
        //map the fields to cdoBenAppOthersDisBenefits
        //and add the newly created instance of cdo object to collection

        public void LoadOtherDisabilityBenefitsFromApplication(int aintBenefitApplicationID)
        {
            //this check is done in order to know that user never entered any other disability benefit option            
            iclcBenCalcOtherDisBenefit = new Collection<busBenefitCalculationOtherDisBenefit>();
            DataTable ldtbOtherDisBen = Select<cdoBenAppOtherDisBenefit>(new string[1] { "benefit_application_id" },
                                                    new object[1] { aintBenefitApplicationID }, null, null);
            if (ldtbOtherDisBen.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbOtherDisBen.Rows)
                {
                    string lstrCodeValue = Convert.ToString(dr["other_disability_benefit_value"]);
                    if (((icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Judges) &&
                    (lstrCodeValue == "SSDB")) ||
                    ((icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Highway_Patrol) &&
                    (lstrCodeValue == "WSIB")) ||
                    ((icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Judges) &&
                    (lstrCodeValue == "WSIB")))
                    {

                        busBenefitCalculationOtherDisBenefit lobjOtherDisabilityBenefit = new busBenefitCalculationOtherDisBenefit() { icdoBenefitCalculationOtherDisBenefit = new cdoBenefitCalculationOtherDisBenefit() };
                        lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.LoadData(dr);
                        lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.other_disability_benefit_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1911,
                                                                                                   lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.other_disability_benefit_value);
                        iclcBenCalcOtherDisBenefit.Add(lobjOtherDisabilityBenefit);
                    }
                }
            }
        }

        public void LoadOtherDisabilityBenefits()
        {
            //this check is done in order to know that user never entered any other disability benefit option            
            iclcBenCalcOtherDisBenefit = new Collection<busBenefitCalculationOtherDisBenefit>();
            DataTable ldtbOtherDisBen = Select<cdoBenefitCalculationOtherDisBenefit>(new string[1] { "benefit_calculation_id" },
                                                    new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            if (ldtbOtherDisBen.Rows.Count > 0)
            {
                foreach (DataRow dr in ldtbOtherDisBen.Rows)
                {
                    busBenefitCalculationOtherDisBenefit lobjOtherDisabilityBenefit = new busBenefitCalculationOtherDisBenefit() { icdoBenefitCalculationOtherDisBenefit = new cdoBenefitCalculationOtherDisBenefit() };
                    lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.LoadData(dr);
                    lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.other_disability_benefit_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1911,
                                                                                               lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.other_disability_benefit_value);
                    iclcBenCalcOtherDisBenefit.Add(lobjOtherDisabilityBenefit);
                }
            }
            //check if already options are entered for the application
            else
            {
                DataTable ldtbGetOthrDisBen = iobjPassInfo.isrvDBCache.GetCodeValues(1911);
                int lintIndex = 1;
                foreach (DataRow dr in ldtbGetOthrDisBen.Rows)
                {
                    if (dr["code_value"] != DBNull.Value)
                    {
                        string lstrCodeValue = Convert.ToString(dr["code_value"]);
                        if (((icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Judges) &&
                            (lstrCodeValue == "SSDB")) ||
                            ((icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Highway_Patrol) &&
                            (lstrCodeValue == "WSIB")) ||
                            ((icdoBenefitCalculation.plan_id.ToString() == busConstant.Plan_ID_Judges) &&
                            (lstrCodeValue == "WSIB"))
                            )
                        {
                            busBenefitCalculationOtherDisBenefit lobjOtherDisabilityBenefit = new busBenefitCalculationOtherDisBenefit() { icdoBenefitCalculationOtherDisBenefit = new cdoBenefitCalculationOtherDisBenefit() };
                            lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.benefit_estimate_other_dis_benefit_id = lintIndex * -1;
                            lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.other_disability_benefit_value = lstrCodeValue;
                            lobjOtherDisabilityBenefit.icdoBenefitCalculationOtherDisBenefit.other_disability_benefit_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1911, lstrCodeValue);
                            iclcBenCalcOtherDisBenefit.Add(lobjOtherDisabilityBenefit);
                            lintIndex++;
                        }
                    }
                }
            }
        }

        #endregion

        #region Final Average Salary

        /// <summary>
        /// Loads FAS Months after termination date 12/31/2019.Pass 3 for this FAS logic.
        /// </summary>
        public void LoadFASMonths2020()
        {
            idecTotalSalary2020 = 0M;
            iclbBenefitCalculationFASMonths2020 = new Collection<busBenefitCalculationFasMonths>();
            // Used query inorder to get Plan Name
            DataTable ldtbResult = Select("cdoBenefitCalculation.LoadFASMonths", new object[2] { icdoBenefitCalculation.benefit_calculation_id, busConstant.FAS2020 });
            foreach (DataRow ldrFASMonth in ldtbResult.Rows)
            {
                busBenefitCalculationFasMonths lobjFASMonth = new busBenefitCalculationFasMonths();
                lobjFASMonth.icdoBenefitCalculationFasMonths = new cdoBenefitCalculationFasMonths();
                lobjFASMonth.ibusPersonAccount = new busPersonAccount();
                lobjFASMonth.ibusPersonAccount.icdoPersonAccount = new cdoPersonAccount();
                lobjFASMonth.ibusPersonAccount.ibusPlan = new busPlan();
                lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan = new cdoPlan();
                lobjFASMonth.icdoBenefitCalculationFasMonths.LoadData(ldrFASMonth);
                lobjFASMonth.ibusPersonAccount.icdoPersonAccount.LoadData(ldrFASMonth);
                lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan.LoadData(ldrFASMonth);
                // Add to Collection
                if (lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan.plan_id == 0)
                    lobjFASMonth.istrPlanName = "Combined";
                else
                {
                    lobjFASMonth.istrPlanName = lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan.plan_name;
                }
                iclbBenefitCalculationFASMonths2020.Add(lobjFASMonth);
                // Calculate Total Salary
                idecTotalSalary2020 += lobjFASMonth.icdoBenefitCalculationFasMonths.salary_amount;
            }
        }

        /// <summary>
        /// Loads FAS Months before termination date 12/31/2019.Pass 2 for this FAS logic.
        /// </summary>
        public void LoadFASMonths2019()
        {
            idecTotalSalary2019 = 0M;
            iclbBenefitCalculationFASMonths2019 = new Collection<busBenefitCalculationFasMonths>();
            // Used query inorder to get Plan Name
            DataTable ldtbResult = Select("cdoBenefitCalculation.LoadFASMonths", new object[2] { icdoBenefitCalculation.benefit_calculation_id, busConstant.FAS2019 });
            foreach (DataRow ldrFASMonth in ldtbResult.Rows)
            {
                busBenefitCalculationFasMonths lobjFASMonth = new busBenefitCalculationFasMonths();
                lobjFASMonth.icdoBenefitCalculationFasMonths = new cdoBenefitCalculationFasMonths();
                lobjFASMonth.ibusPersonAccount = new busPersonAccount();
                lobjFASMonth.ibusPersonAccount.icdoPersonAccount = new cdoPersonAccount();
                lobjFASMonth.ibusPersonAccount.ibusPlan = new busPlan();
                lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan = new cdoPlan();
                lobjFASMonth.icdoBenefitCalculationFasMonths.LoadData(ldrFASMonth);
                lobjFASMonth.ibusPersonAccount.icdoPersonAccount.LoadData(ldrFASMonth);
                lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan.LoadData(ldrFASMonth);
                // Add to Collection
                if (lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan.plan_id == 0)
                    lobjFASMonth.istrPlanName = "Combined";
                else
                {
                    lobjFASMonth.istrPlanName = lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan.plan_name;
                }
                iclbBenefitCalculationFASMonths2019.Add(lobjFASMonth);
                // Calculate Total Salary
                idecTotalSalary2019 += lobjFASMonth.icdoBenefitCalculationFasMonths.salary_amount;
            }
        }
        // Loads the Final Average Salary Months from DB
        public void LoadFASMonths()
        {
            idecTotalSalary = 0M;
            iclbBenefitCalculationFASMonths = new Collection<busBenefitCalculationFasMonths>();
            // Used query inorder to get Plan Name
            DataTable ldtbResult = Select("cdoBenefitCalculation.LoadFASMonths", new object[2] { icdoBenefitCalculation.benefit_calculation_id, busConstant.FAS2010 });

            foreach (DataRow ldrFASMonth in ldtbResult.Rows)
            {
                busBenefitCalculationFasMonths lobjFASMonth = new busBenefitCalculationFasMonths();
                lobjFASMonth.icdoBenefitCalculationFasMonths = new cdoBenefitCalculationFasMonths();
                lobjFASMonth.ibusPersonAccount = new busPersonAccount();
                lobjFASMonth.ibusPersonAccount.icdoPersonAccount = new cdoPersonAccount();
                lobjFASMonth.ibusPersonAccount.ibusPlan = new busPlan();
                lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan = new cdoPlan();
                lobjFASMonth.icdoBenefitCalculationFasMonths.LoadData(ldrFASMonth);
                lobjFASMonth.ibusPersonAccount.icdoPersonAccount.LoadData(ldrFASMonth);
                lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan.LoadData(ldrFASMonth);
                // Add to Collection
                if (lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan.plan_id == 0)
                    lobjFASMonth.istrPlanName = "Combined";
                else
                {
                    lobjFASMonth.istrPlanName = lobjFASMonth.ibusPersonAccount.ibusPlan.icdoPlan.plan_name;
                }
                iclbBenefitCalculationFASMonths.Add(lobjFASMonth);
                // Calculate Total Salary
                idecTotalSalary += lobjFASMonth.icdoBenefitCalculationFasMonths.salary_amount;
                //int month = lobjFASMonth.icdoBenefitCalculationFasMonths.month;
                //int year = lobjFASMonth.icdoBenefitCalculationFasMonths.year;
                //DateTime a = icdoBenefitCalculation.termination_date;
                //int year1 = a.Year;
                //int month1 = a.Month;
                //if (year > year1)
                //{
                //    lobjFASMonth.icdoBenefitCalculationFasMonths.salary_amount = 0M;
                //}
                //else if (year1 == year)
                //{
                //    if (month > month1)
                //    {
                //        lobjFASMonth.icdoBenefitCalculationFasMonths.salary_amount = 0M;
                //    }
                //}

                //idecTotalSalary += lobjFASMonth.icdoBenefitCalculationFasMonths.salary_amount;
            }
        }

        //PIR 22785
        public int iintSalaryCount { get; set; }

        // Calculates the Final Average Salary Months
        public void CalculateFAS()
        {
            decimal ldecComputedFAS = 0M, ldecCalculationFAS = 0M, ldecCalculationFAS2019 = 0M, ldecComputedFAS2019 = 0M, ldecCalculationFAS2020 = 0M, ldecComputedFAS2020 = 0M;
            CalculateFAS(ref ldecComputedFAS, ref ldecCalculationFAS);
            icdoBenefitCalculation.computed_final_average_salary = ldecComputedFAS;
            //icdoBenefitCalculation.indexed_final_average_salary = ldecIndexedFAS;
            icdoBenefitCalculation.calculation_final_average_salary = ldecCalculationFAS;
            icdoBenefitCalculation.fas_2010 = ldecComputedFAS;

            // Call new FAS logic for the termination date greater than following date & Plan should not be job service
            DateTime ldteCheckTerminationDate = new DateTime(2019, 12, 31);
            if ((icdoBenefitCalculation.termination_date > ldteCheckTerminationDate) &&
                  (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService) &&
                  (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService3rdPartyPayor))
            {
                //PIR 22785
                if (iintSalaryCount > 36)
                {
                    CalculateFAS2019(ref ldecComputedFAS2019, ref ldecCalculationFAS2019);
                    icdoBenefitCalculation.fas_2019 = ldecComputedFAS2019;
                }
                CalculateFAS2020(ref ldecComputedFAS2020, ref ldecCalculationFAS2020);
                icdoBenefitCalculation.fas_2020 = ldecComputedFAS2020;

                //set calculation_final_average_salary amount from 2019 & 2020 amount whichever is higher.
                icdoBenefitCalculation.computed_final_average_salary = ldecComputedFAS2019 > ldecComputedFAS2020 ? ldecComputedFAS2019 : ldecComputedFAS2020;
                icdoBenefitCalculation.calculation_final_average_salary = ldecCalculationFAS2019 > ldecCalculationFAS2020 ? ldecCalculationFAS2019 : ldecCalculationFAS2020;
            }
        }

        /// <summary>
        /// Determines the Final Average Salary.
        /// </summary>
        /// <param name="aobjBenefitCalculation">Benefit Calculation</param>
        /// <param name="adecComputedFAS">Computed FAS</param>
        /// <param name="adecIndexedFAS">Indexed FAS</param>
        /// <param name="adecCalculationFAS">Calculation FAS</param>
        public void CalculateFAS(ref decimal adecComputedFAS, ref decimal adecCalculationFAS)
        {
            int lintFASDivider = 0;
            // Load Benefit Provision Benefit Type
            if (ibusBenefitProvisionBenefitType == null)
                LoadBenefitProvisionBenefitType();
            if (icdoBenefitCalculation.iblnUseMainPlanFasFactors)   // Service Purchase PIR-1054 FAS 
                LoadBenefitProvisionBenefitTypeForDCUsingMainFasFactors();

            //UAT PIR: 1131 Dual FAS Date.
            bool iblnIsProjectionApplicable = true;
            iblnIsProjectionApplicable = SetDualFASTerminationDate();

            if ((ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value == busConstant.FASHighestConsecutive) ||
                (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value == busConstant.FASHighestAverage))
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                Collection<busPersonAccountRetirementContribution> lclbFinalSalaryRecords = new Collection<busPersonAccountRetirementContribution>();
                if (idteLastContributedDate == DateTime.MinValue)
                    LoadLastContributedDate();

                // SYSTEST - PIR - 1477 - For Pre-Retirement death if no termination date exists, use date of death for FAS Calculation.
                DateTime ldteTerminationDate = new DateTime();
                DateTime ldteTerminationDateUpdated = new DateTime();
                DataTable idtAllContEmpdetails = LoadAllContributingEmploymentDetails();
				//PIR 17924
                if (idtAllContEmpdetails.IsNotNull() && idtAllContEmpdetails.Rows.Count > 0)
                {
                    ldteTerminationDateUpdated = (idtAllContEmpdetails.Rows[0]["END_DATE"] != DBNull.Value) ? Convert.ToDateTime(idtAllContEmpdetails.Rows[0]["END_DATE"]) : DateTime.MinValue;
                }
                if ((icdoBenefitCalculation.termination_date == DateTime.MinValue) &&
                    (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
                    ldteTerminationDate = icdoBenefitCalculation.date_of_death;
                else
                    ldteTerminationDate = icdoBenefitCalculation.fas_termination_date; 

				//prod pir:4761: termination date set as last day of the Month to accommodate that month salary too
                ldteTerminationDate = ldteTerminationDate.GetLastDayofMonth();

                DateTime ldteStartDate = (idteLastContributedDate == DateTime.MinValue) ? ldteTerminationDate : idteLastContributedDate;
                if (!iblnConsolidatedPSCLoaded)
                    CalculateConsolidatedPSC();
                if (!iblnConsoldatedVSCLoaded)
                    CalculateConsolidatedVSC();
                int lintRTWPersonAccountID = 0;
                // PIR ID 1920  For RTW refund case, the FAS should not consider the refunded contributions
                if ((icdoBenefitCalculation.is_rtw_less_than_2years_flag == busConstant.Flag_Yes) &&
                    (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper()))
                {
                    lintRTWPersonAccountID = ibusPersonAccount.icdoPersonAccount.person_account_id;
                }
                if ((IsJobService) &&
                    (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value == busConstant.FASHighestConsecutive))
                {
                    if (icdoBenefitCalculation.is_rtw_member_subsequent_retirement || 
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                    {
                        lclbFinalSalaryRecords = GetJobServiceSalaryRecords(ldteStartDate,
                                                                        ibusPersonAccount.icdoPersonAccount.person_id,
                                                                        ibusPersonAccount.icdoPersonAccount.plan_id,
                                                                        icdoBenefitCalculation.calculation_type_value,
                                                                        icdoBenefitCalculation.salary_month_increase,
                                                                        icdoBenefitCalculation.percentage_salary_increase,
                                                                        ldteTerminationDate,
                                                                        ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods,
                                                                        lintRTWPersonAccountID, iobjPassInfo, true, ibusPersonAccount.icdoPersonAccount.person_account_id);
                    }
                    else
                    {
                        lclbFinalSalaryRecords = GetJobServiceSalaryRecords(ldteStartDate,
                                                                            ibusPersonAccount.icdoPersonAccount.person_id,
                                                                            ibusPersonAccount.icdoPersonAccount.plan_id,
                                                                            icdoBenefitCalculation.calculation_type_value,
                                                                            icdoBenefitCalculation.salary_month_increase,
                                                                            icdoBenefitCalculation.percentage_salary_increase,
                                                                            ldteTerminationDate,
                                                                            ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods,
                                                                            lintRTWPersonAccountID, iobjPassInfo);
                    }
                }
                else if (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value == busConstant.FASHighestAverage)
                {
                    //UAT PIR: 1577
                    //Fetch Last 180 Months including the Projected Salary and then find the Top 36.
                    int lintFASnumberofPeriods = ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods;
                    if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                    {
                        lintFASnumberofPeriods = ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range;
                    }
                    //End Of UAT PIR:1577

                    if (icdoBenefitCalculation.is_rtw_member_subsequent_retirement ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                    {
                        lclbFinalSalaryRecords = GetSalaryRecords(ldteStartDate,
                                                      ibusPersonAccount.icdoPersonAccount.person_id,
                                                      ibusPersonAccount.icdoPersonAccount.plan_id,
                                                      lintFASnumberofPeriods,
                                                      ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range,
                                                      lintRTWPersonAccountID, true, ibusPersonAccount.icdoPersonAccount.person_account_id);
                    }
                    else
                    {
                        lclbFinalSalaryRecords = GetSalaryRecords(ldteStartDate,
                                                      ibusPersonAccount.icdoPersonAccount.person_id,
                                                      ibusPersonAccount.icdoPersonAccount.plan_id,
                                                      lintFASnumberofPeriods,
                                                      ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range,
                                                      lintRTWPersonAccountID);
                    }

                }

                //PIR 22785
                object lobjFASCount = DBFunction.DBExecuteScalar("cdoBenefitCalculationFasMonths.GetCountOfSalaryRecords", new object[3]{
                                        ibusPersonAccount.icdoPersonAccount.person_id,
                                        ibusPersonAccount.icdoPersonAccount.plan_id,
                                        ldteStartDate}, iobjPassInfo.iconFramework,
                                        iobjPassInfo.itrnFramework);
                if (lobjFASCount.IsNotNull())
                    iintSalaryCount = Convert.ToInt32(lobjFASCount);

                //Commented as per David mail on Dual FAS dtd 02 Jun 2010
                //DateTime ldteActualEmployeeTerminationDate = new DateTime();
                //GetOrgIdAsLatestEmploymentOrgId(
                //                    ibusPersonAccount,
                //                    busConstant.ApplicationBenefitTypeRetirement,
                //                    ref ldteActualEmployeeTerminationDate);
                //if (ldteActualEmployeeTerminationDate == DateTime.MinValue)
                //{
                if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                    && (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value != busConstant.FASHighestConsecutive))
                {
                    // PIR ID 1920 For an RTW estimate, project salary only if the Refund selection is No.
                    if (!((icdoBenefitCalculation.is_return_to_work_member) &&
                         (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper())))
                    {
                        if (iblnIsProjectionApplicable)
                        {
                            Collection<busPersonAccountRetirementContribution> lclbProjectedSalaryRecords = new Collection<busPersonAccountRetirementContribution>();
                            lclbProjectedSalaryRecords = GetProjectedSalaryRecords(ibusPersonAccount.icdoPersonAccount.person_id,
                                                                                       ibusPersonAccount.icdoPersonAccount.plan_id,
                                                                                       (ldteTerminationDateUpdated == DateTime.MinValue) ? ldteTerminationDate : ldteTerminationDateUpdated,
                                                                                       icdoBenefitCalculation.salary_month_increase,
                                                                                       icdoBenefitCalculation.percentage_salary_increase, iobjPassInfo,
                                                                                       ibusPersonAccount.ibusPerson, idtbLastSalaryWithoutPersonAccount);
                            foreach (busPersonAccountRetirementContribution lobjProjSalaryRecord in lclbProjectedSalaryRecords)
                            {
                                lclbFinalSalaryRecords.Add(lobjProjSalaryRecord);
                            }
                        }
                    }
                }
                //}

                //UAT PIR: 1577
                //Fetch Last 180 Months including the Projected Salary and then find the Top 36.
                if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                    && (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value == busConstant.FASHighestAverage)
                    && (!(IsJobService)))
                {
                    if (lclbFinalSalaryRecords.Count > ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range)
                    {
                        lclbFinalSalaryRecords = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>
                        ("icdoPersonAccountRetirementContribution.effective_date desc", lclbFinalSalaryRecords);

                        Collection<busPersonAccountRetirementContribution> lclbTempFinalSalaryRecords = new Collection<busPersonAccountRetirementContribution>();

                        foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lclbFinalSalaryRecords)
                        {
                            if (lclbTempFinalSalaryRecords.Count < ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range)
                            {
                                lclbTempFinalSalaryRecords.Add(lobjRetirementContribution);
                            }
                            else
                            {
                                break;
                            }
                        }

                        lclbFinalSalaryRecords = lclbTempFinalSalaryRecords;
                    }
                }
                //End Of UAT PIR:1577
                
                /********* PROD PIR 2004 has modified rules for HP Indexing and when to apply ~ START*******/
                /*HP salary is only indexed when:
                    1.	Member has TVSC = 120
                    2.	Retirement Date must be at least 1 month after the Termination Date for indexing to be applied.
                    3.	Not RTW
                    4.	If Member has enrolled/suspended HP account. */

                if (iblnIsActualTerminationDate)//PIR 23048
                {
                    if (IsMemberEligibleForHPIndexing(ldteTerminationDate) &&
                        lclbFinalSalaryRecords.Where(i => i.ibusPARetirement.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdHP).Any())
                    {
                        foreach (busPersonAccountRetirementContribution lobjRetrContribution in lclbFinalSalaryRecords)
                        {
                            if (lobjRetrContribution.ibusPARetirement.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdHP)
                            {
                                lobjRetrContribution.icdoPersonAccountRetirementContribution.salary_amount =
                                    CalculateIndexedFAS(ldteTerminationDate, icdoBenefitCalculation.retirement_date,
                                                        lobjRetrContribution.icdoPersonAccountRetirementContribution.salary_amount,
                                                        lobjRetrContribution.ibusPARetirement.ibusPlan.icdoPlan.plan_id, DateTime.Today);
                            }
                        }
                    }
                }
                /********* PROD PIR 2004 has modified rules for HP Indexing and when to apply ~ END *******/

                // Sort the FAS Months Collections // PIR ID 1125
                // Projected FAS months was not upto the Termination Date // PIR ID 1494
                iclbBenefitCalculationFASMonths = new Collection<busBenefitCalculationFasMonths>();
                lclbFinalSalaryRecords = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>
                                        ("icdoPersonAccountRetirementContribution.salary_amount desc," +
                                         "icdoPersonAccountRetirementContribution.effective_date desc",
                                        lclbFinalSalaryRecords);

                iclbFinalSalaryRecords = lclbFinalSalaryRecords;
                foreach (busPersonAccountRetirementContribution lobjRetirementContribution in iclbFinalSalaryRecords)
                {
                    if (iclbBenefitCalculationFASMonths.Count < ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods)
                    {
                        busBenefitCalculationFasMonths lobjFASMonth = new busBenefitCalculationFasMonths
                        {
                            icdoBenefitCalculationFasMonths = new cdoBenefitCalculationFasMonths(),
                            ibusPersonAccount = new busPersonAccount
                            {
                                icdoPersonAccount = new cdoPersonAccount(),
                                ibusPlan = new busPlan { icdoPlan = new cdoPlan() }
                            }
                        };
                        lobjFASMonth.icdoBenefitCalculationFasMonths.fas_logic_id = 7018;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.fas_logic_value = busConstant.FAS2010;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.person_account_id =
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.month =
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.year =
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.effective_date = lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.salary_amount =
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.projected_flag =
                            lobjRetirementContribution.ibnlIsProjectedSalaryRecord == true ? busConstant.Flag_Yes : busConstant.Flag_No;
                        lobjFASMonth.ibusPersonAccount.ibusPlan = lobjRetirementContribution.ibusPARetirement.ibusPlan;
                        iclbBenefitCalculationFASMonths.Add(lobjFASMonth);
                    }
                }
                lintFASDivider = iclbBenefitCalculationFASMonths.Count;
                idecTotalSalary = 0.00M;
                foreach (busBenefitCalculationFasMonths lobjFASMonths in iclbBenefitCalculationFASMonths)
                {
                    idecTotalSalary += lobjFASMonths.icdoBenefitCalculationFasMonths.salary_amount;
                }
                adecComputedFAS = idecTotalSalary;
                adecComputedFAS = FormatFAS(idecTotalSalary, lintFASDivider, icdoBenefitCalculation.plan_id);
                adecCalculationFAS = adecComputedFAS;

                /********* PROD PIR 2004 has modified rules for HP Indexing and when to apply *******/
                //if (IsMemberEligibleForHPIndexing(ldteTerminationDate))
                //{
                //    adecIndexedFAS = CalculateIndexedFAS(ldteTerminationDate,
                //                            icdoBenefitCalculation.retirement_date, adecComputedFAS,
                //                            icdoBenefitCalculation.plan_id, DateTime.Today);
                //    adecCalculationFAS = adecIndexedFAS;
                //}
                if (icdoBenefitCalculation.overridden_final_average_salary > 0.00M)
                {
                    adecCalculationFAS = icdoBenefitCalculation.overridden_final_average_salary;
                }
            }
        }
        /// <summary>
        /// Calculate FAS before Termination Date - 12/31/2019. 
        /// </summary>
        /// <param name="adecCalculationFAS2019"></param>
        public void CalculateFAS2019(ref decimal adecComputedFAS2019, ref decimal adecCalculationFAS2019)
        {
            int lintFASDivider = 0;
            // Load Benefit Provision Benefit Type
            if (ibusBenefitProvisionBenefitType == null)
                LoadBenefitProvisionBenefitType();
            if (icdoBenefitCalculation.iblnUseMainPlanFasFactors)   // Service Purchase PIR-1054 FAS 
                LoadBenefitProvisionBenefitTypeForDCUsingMainFasFactors();

            //UAT PIR: 1131 Dual FAS Date.
            /* bool iblnIsProjectionApplicable = true;
             iblnIsProjectionApplicable = SetDualFASTerminationDate();*/
            //icdoBenefitCalculation.fas_termination_date = DateTime;

            if ((ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value == busConstant.FASHighestConsecutive) ||
                (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value == busConstant.FASHighestAverage))
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                Collection<busPersonAccountRetirementContribution> lclbFinalSalaryRecords = new Collection<busPersonAccountRetirementContribution>();
                if (idteLastContributedDate == DateTime.MinValue)
                    LoadLastContributedDate();


                DateTime ldteTerminationDate = new DateTime();
                /*
                // SYSTEST - PIR - 1477 - For Pre-Retirement death if no termination date exists, use date of death for FAS Calculation.
                DateTime ldteTerminationDateUpdated = new DateTime();
                DataTable idtAllContEmpdetails = LoadAllContributingEmploymentDetails();
                //PIR 17924
                if (idtAllContEmpdetails.IsNotNull() && idtAllContEmpdetails.Rows.Count > 0)
                {
                    ldteTerminationDateUpdated = (idtAllContEmpdetails.Rows[0]["END_DATE"] != DBNull.Value) ? Convert.ToDateTime(idtAllContEmpdetails.Rows[0]["END_DATE"]) : DateTime.MinValue;
                }
               
                if ((icdoBenefitCalculation.termination_date == DateTime.MinValue) &&
                    (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
                    ldteTerminationDate = icdoBenefitCalculation.date_of_death;
                else
                    ldteTerminationDate = icdoBenefitCalculation.fas_termination_date;
               */
                //prod pir:4761: termination date set as last day of the Month to accommodate that month salary too
                ldteTerminationDate = new DateTime(2019, 12, 31);//Need to set fix Termination date
                //icdoBenefitCalculation.fas_termination_date=ldteTerminationDate;
                ldteTerminationDate = ldteTerminationDate.GetLastDayofMonth();

                //DateTime ldteStartDate = (idteLastContributedDate == DateTime.MinValue) ? ldteTerminationDate : idteLastContributedDate;
                DateTime ldteStartDate = ldteTerminationDate;
                //if (!iblnConsolidatedPSCLoaded)
                //    CalculateConsolidatedPSC();

                int lintRTWPersonAccountID = 0;
                // PIR ID 1920  For RTW refund case, the FAS should not consider the refunded contributions
                if ((icdoBenefitCalculation.is_rtw_less_than_2years_flag == busConstant.Flag_Yes) &&
                    (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper()))
                {
                    lintRTWPersonAccountID = ibusPersonAccount.icdoPersonAccount.person_account_id;
                }
                if ((IsJobService) &&
                    (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value == busConstant.FASHighestConsecutive))
                {
                    if (icdoBenefitCalculation.is_rtw_member_subsequent_retirement ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                    {
                        lclbFinalSalaryRecords = GetJobServiceSalaryRecords(ldteStartDate,
                                                                        ibusPersonAccount.icdoPersonAccount.person_id,
                                                                        ibusPersonAccount.icdoPersonAccount.plan_id,
                                                                        icdoBenefitCalculation.calculation_type_value,
                                                                        icdoBenefitCalculation.salary_month_increase,
                                                                        icdoBenefitCalculation.percentage_salary_increase,
                                                                        ldteTerminationDate,
                                                                        ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods,
                                                                        lintRTWPersonAccountID, iobjPassInfo, true, ibusPersonAccount.icdoPersonAccount.person_account_id);
                    }
                    else
                    {
                        lclbFinalSalaryRecords = GetJobServiceSalaryRecords(ldteStartDate,
                                                                            ibusPersonAccount.icdoPersonAccount.person_id,
                                                                            ibusPersonAccount.icdoPersonAccount.plan_id,
                                                                            icdoBenefitCalculation.calculation_type_value,
                                                                            icdoBenefitCalculation.salary_month_increase,
                                                                            icdoBenefitCalculation.percentage_salary_increase,
                                                                            ldteTerminationDate,
                                                                            ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods,
                                                                            lintRTWPersonAccountID, iobjPassInfo);
                    }
                }
                else if (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value == busConstant.FASHighestAverage)
                {
                    //UAT PIR: 1577
                    //Fetch Last 180 Months including the Projected Salary and then find the Top 36.
                    int lintFASnumberofPeriods = ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods;
                    if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                    {
                        lintFASnumberofPeriods = ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range;
                    }
                    //End Of UAT PIR:1577

                    if (icdoBenefitCalculation.is_rtw_member_subsequent_retirement ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                    {
                        lclbFinalSalaryRecords = GetSalaryRecords(ldteStartDate,
                                                      ibusPersonAccount.icdoPersonAccount.person_id,
                                                      ibusPersonAccount.icdoPersonAccount.plan_id,
                                                      lintFASnumberofPeriods,
                                                      ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range,
                                                      lintRTWPersonAccountID, true, ibusPersonAccount.icdoPersonAccount.person_account_id);
                    }
                    else
                    {
                        lclbFinalSalaryRecords = GetSalaryRecords(ldteStartDate,
                                                      ibusPersonAccount.icdoPersonAccount.person_id,
                                                      ibusPersonAccount.icdoPersonAccount.plan_id,
                                                      lintFASnumberofPeriods,
                                                      ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range,
                                                      lintRTWPersonAccountID);
                    }

                }

                //Commented as per David mail on Dual FAS dtd 02 Jun 2010
                //DateTime ldteActualEmployeeTerminationDate = new DateTime();
                //GetOrgIdAsLatestEmploymentOrgId(
                //                    ibusPersonAccount,
                //                    busConstant.ApplicationBenefitTypeRetirement,
                //                    ref ldteActualEmployeeTerminationDate);
                //if (ldteActualEmployeeTerminationDate == DateTime.MinValue)
                //{

                //Dont consider Projection for FAS 2019-fix termination date 12/31/2019  
                /*if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                    && (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value != busConstant.FASHighestConsecutive))
                {
                    // PIR ID 1920 For an RTW estimate, project salary only if the Refund selection is No.
                    if (!((icdoBenefitCalculation.is_return_to_work_member) &&
                         (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper())))
                    {
                        if (iblnIsProjectionApplicable)
                        {
                            Collection<busPersonAccountRetirementContribution> lclbProjectedSalaryRecords = new Collection<busPersonAccountRetirementContribution>();
                            lclbProjectedSalaryRecords = GetProjectedSalaryRecords(ibusPersonAccount.icdoPersonAccount.person_id,
                                                                                       ibusPersonAccount.icdoPersonAccount.plan_id,
                                                                                       (ldteTerminationDateUpdated == DateTime.MinValue) ? ldteTerminationDate : ldteTerminationDateUpdated,
                                                                                       icdoBenefitCalculation.salary_month_increase,
                                                                                       icdoBenefitCalculation.percentage_salary_increase, iobjPassInfo,
                                                                                       ibusPersonAccount.ibusPerson, idtbLastSalaryWithoutPersonAccount);
                            foreach (busPersonAccountRetirementContribution lobjProjSalaryRecord in lclbProjectedSalaryRecords)
                            {
                                lclbFinalSalaryRecords.Add(lobjProjSalaryRecord);
                            }
                        }
                    }
                }*/

                //}

                //UAT PIR: 1577
                //Fetch Last 180 Months including the Projected Salary and then find the Top 36.
                if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                    && (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_formula_value == busConstant.FASHighestAverage)
                    && (!(IsJobService)))
                {
                    if (lclbFinalSalaryRecords.Count > ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range)
                    {
                        lclbFinalSalaryRecords = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>
                        ("icdoPersonAccountRetirementContribution.effective_date desc", lclbFinalSalaryRecords);

                        Collection<busPersonAccountRetirementContribution> lclbTempFinalSalaryRecords = new Collection<busPersonAccountRetirementContribution>();

                        foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lclbFinalSalaryRecords)
                        {
                            if (lclbTempFinalSalaryRecords.Count < ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range)
                            {
                                lclbTempFinalSalaryRecords.Add(lobjRetirementContribution);
                            }
                            else
                            {
                                break;
                            }
                        }

                        lclbFinalSalaryRecords = lclbTempFinalSalaryRecords;
                    }
                }
                //End Of UAT PIR:1577

                /********* PROD PIR 2004 has modified rules for HP Indexing and when to apply ~ START*******/
                /*HP salary is only indexed when:
                    1.	Member has TVSC = 120
                    2.	Retirement Date must be at least 1 month after the Termination Date for indexing to be applied.
                    3.	Not RTW
                    4.	If Member has enrolled/suspended HP account. */

                if (iblnIsActualTerminationDate)//PIR 23048
                {
                    if (IsMemberEligibleForHPIndexing(ldteTerminationDate) &&
                        lclbFinalSalaryRecords.Where(i => i.ibusPARetirement.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdHP).Any())
                    {
                        foreach (busPersonAccountRetirementContribution lobjRetrContribution in lclbFinalSalaryRecords)
                        {
                            if (lobjRetrContribution.ibusPARetirement.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdHP)
                            {
                                lobjRetrContribution.icdoPersonAccountRetirementContribution.salary_amount =
                                    CalculateIndexedFAS(ldteTerminationDate, icdoBenefitCalculation.retirement_date,
                                                        lobjRetrContribution.icdoPersonAccountRetirementContribution.salary_amount,
                                                        lobjRetrContribution.ibusPARetirement.ibusPlan.icdoPlan.plan_id, DateTime.Today);
                            }
                        }
                    }
                }

                /********* PROD PIR 2004 has modified rules for HP Indexing and when to apply ~ END *******/

                // Sort the FAS Months Collections // PIR ID 1125
                // Projected FAS months was not upto the Termination Date // PIR ID 1494
                iclbBenefitCalculationFASMonths2019 = new Collection<busBenefitCalculationFasMonths>();
                lclbFinalSalaryRecords = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>
                                        ("icdoPersonAccountRetirementContribution.salary_amount desc," +
                                         "icdoPersonAccountRetirementContribution.effective_date desc",
                                        lclbFinalSalaryRecords);

                //iclbFinalSalaryRecords = lclbFinalSalaryRecords;
                foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lclbFinalSalaryRecords)
                {
                    if (iclbBenefitCalculationFASMonths2019.Count < ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods)
                    {
                        busBenefitCalculationFasMonths lobjFASMonth = new busBenefitCalculationFasMonths
                        {
                            icdoBenefitCalculationFasMonths = new cdoBenefitCalculationFasMonths(),
                            ibusPersonAccount = new busPersonAccount
                            {
                                icdoPersonAccount = new cdoPersonAccount(),
                                ibusPlan = new busPlan { icdoPlan = new cdoPlan() }
                            }
                        };
                        lobjFASMonth.icdoBenefitCalculationFasMonths.fas_logic_id = 7018;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.fas_logic_value = busConstant.FAS2019;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.person_account_id =
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.month =
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.year =
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.effective_date = lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.salary_amount =
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount;
                        lobjFASMonth.icdoBenefitCalculationFasMonths.projected_flag =
                            lobjRetirementContribution.ibnlIsProjectedSalaryRecord == true ? busConstant.Flag_Yes : busConstant.Flag_No;
                        lobjFASMonth.ibusPersonAccount.ibusPlan = lobjRetirementContribution.ibusPARetirement.ibusPlan;
                        iclbBenefitCalculationFASMonths2019.Add(lobjFASMonth);
                    }
                }
                lintFASDivider = iclbBenefitCalculationFASMonths2019.Count;
                idecTotalSalary2019 = 0.00M;
                foreach (busBenefitCalculationFasMonths lobjFASMonths in iclbBenefitCalculationFASMonths2019)
                {
                    idecTotalSalary2019 += lobjFASMonths.icdoBenefitCalculationFasMonths.salary_amount;
                }
                adecComputedFAS2019 = idecTotalSalary2019;
                adecComputedFAS2019 = FormatFAS(idecTotalSalary2019, lintFASDivider, icdoBenefitCalculation.plan_id);
                adecCalculationFAS2019 = adecComputedFAS2019;

                if (icdoBenefitCalculation.overridden_final_average_salary > 0.00M)
                {
                    adecCalculationFAS2019 = icdoBenefitCalculation.overridden_final_average_salary;
                }
            }
        }

        /// <summary>
        /// Calculate FAS After Termination Date - 12/31/2019. 
        /// </summary>
        /// <param name="adecCalculationFAS2020"></param>
        public void CalculateFAS2020(ref decimal adecComputedFAS2020, ref decimal adecCalculationFAS2020)
        {
            int lintFASDivider = 0;
            DateTime ldteTerminationDateUpdated = new DateTime();
            DateTime ldteTerminationDate = new DateTime();
            Collection<busPersonAccountRetirementContribution> lclbFinalSalaryRecords = new Collection<busPersonAccountRetirementContribution>();

            //if (ibusBenefitProvisionBenefitType == null)
            //    LoadBenefitProvisionBenefitType();

            //if (icdoBenefitCalculation.iblnUseMainPlanFasFactors)   // Service Purchase PIR-1054 FAS 
            //    LoadBenefitProvisionBenefitTypeForDCUsingMainFasFactors();

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (idteLastContributedDate == DateTime.MinValue)
                LoadLastContributedDate();

            // SYSTEST - PIR - 1477 - For Pre-Retirement death if no termination date exists, use date of death for FAS Calculation.
            DataTable idtAllContEmpdetails = LoadAllContributingEmploymentDetails();

            //PIR 17924
            if (idtAllContEmpdetails.IsNotNull() && idtAllContEmpdetails.Rows.Count > 0)
            {
                ldteTerminationDateUpdated = (idtAllContEmpdetails.Rows[0]["END_DATE"] != DBNull.Value) ? Convert.ToDateTime(idtAllContEmpdetails.Rows[0]["END_DATE"]) : DateTime.MinValue;
            }

            if ((icdoBenefitCalculation.termination_date == DateTime.MinValue) &&
                (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
                ldteTerminationDate = icdoBenefitCalculation.date_of_death;
            else
                ldteTerminationDate = icdoBenefitCalculation.fas_termination_date;

            //prod pir:4761: termination date set as last day of the Month to accommodate that month salary too
            ldteTerminationDate = ldteTerminationDate.GetLastDayofMonth();

            DateTime ldteStartDate = (idteLastContributedDate == DateTime.MinValue) ? ldteTerminationDate : idteLastContributedDate;

            bool iblnIsProjectionApplicable = true;
            iblnIsProjectionApplicable = SetDualFASTerminationDate();

            //if (!iblnConsolidatedPSCLoaded)
            //    CalculateConsolidatedPSC();

            int lintRTWPersonAccountID = 0;
            // PIR ID 1920  For RTW refund case, the FAS should not consider the refunded contributions
            if ((icdoBenefitCalculation.is_rtw_less_than_2years_flag == busConstant.Flag_Yes) &&
                (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper()))
            {
                lintRTWPersonAccountID = ibusPersonAccount.icdoPersonAccount.person_account_id;
            }
            // Load all the Service Contribution  
            if (icdoBenefitCalculation.is_rtw_member_subsequent_retirement ||
                        icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
            {
                lclbFinalSalaryRecords = GetSalaryRecordsFAS2020(ldteStartDate,
                                                ibusPersonAccount.icdoPersonAccount.person_id,
                                                ibusPersonAccount.icdoPersonAccount.plan_id,
                                                lintRTWPersonAccountID, ibusPersonAccount.icdoPersonAccount.person_account_id, true);
            }
            else
            {
                lclbFinalSalaryRecords = GetSalaryRecordsFAS2020(ldteStartDate,
                                              ibusPersonAccount.icdoPersonAccount.person_id,
                                              ibusPersonAccount.icdoPersonAccount.plan_id,
                                              lintRTWPersonAccountID, ibusPersonAccount.icdoPersonAccount.person_account_id);
            }

            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate ||
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
            {
                // PIR ID 1920 For an RTW estimate, project salary only if the Refund selection is No.
                if (!((icdoBenefitCalculation.is_return_to_work_member) &&
                     (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper())))
                {
                    if (iblnIsProjectionApplicable)
                    {
                        Collection<busPersonAccountRetirementContribution> lclbProjectedSalaryRecords = new Collection<busPersonAccountRetirementContribution>();
                        lclbProjectedSalaryRecords = GetProjectedSalaryRecords(ibusPersonAccount.icdoPersonAccount.person_id,
                                                                                   ibusPersonAccount.icdoPersonAccount.plan_id,
                                                                                   (ldteTerminationDateUpdated == DateTime.MinValue) ? ldteTerminationDate : ldteTerminationDateUpdated,
                                                                                   icdoBenefitCalculation.salary_month_increase,
                                                                                   icdoBenefitCalculation.percentage_salary_increase, iobjPassInfo,
                                                                                   ibusPersonAccount.ibusPerson, idtbLastSalaryWithoutPersonAccount, true);
                        foreach (busPersonAccountRetirementContribution lobjProjSalaryRecord in lclbProjectedSalaryRecords)
                        {
                            lclbFinalSalaryRecords.Add(lobjProjSalaryRecord);
                        }

                        //PIR 21429 & 23059 - Changes done for 21429 reverted and new logic implemented for 23059.
                        //If final salary records has duplicate salary records, the one with 0 salary are excluded.
                        foreach (busPersonAccountRetirementContribution lobjProjectedSalary in lclbProjectedSalaryRecords)
                        {
                            busPersonAccountRetirementContribution lobjTempFinalSalary = new busPersonAccountRetirementContribution ();
                            
                            if (lclbFinalSalaryRecords.Any(i => i.icdoPersonAccountRetirementContribution.pay_period_year ==
                            lobjProjectedSalary.icdoPersonAccountRetirementContribution.pay_period_year &&
                            i.icdoPersonAccountRetirementContribution.pay_period_month ==
                            lobjProjectedSalary.icdoPersonAccountRetirementContribution.pay_period_month &&
                            i.icdoPersonAccountRetirementContribution.salary_amount == 0.0M))
                            {
                                lobjTempFinalSalary = lclbFinalSalaryRecords.FirstOrDefault(i => i.icdoPersonAccountRetirementContribution.pay_period_year ==
                                 lobjProjectedSalary.icdoPersonAccountRetirementContribution.pay_period_year &&
                                 i.icdoPersonAccountRetirementContribution.pay_period_month ==
                                 lobjProjectedSalary.icdoPersonAccountRetirementContribution.pay_period_month &&
                                 i.icdoPersonAccountRetirementContribution.salary_amount == 0.0M);

                                if (lobjTempFinalSalary.IsNotNull())
                                    lclbFinalSalaryRecords.Remove(lobjTempFinalSalary);
                            }
                        }
                    }
                }
            }

            lclbFinalSalaryRecords = GetSalaryRecordBlocksFAS2020(lclbFinalSalaryRecords);

            // Sort the FAS Months Collections // PIR ID 1125
            // Projected FAS months was not upto the Termination Date // PIR ID 1494
            iclbBenefitCalculationFASMonths2020 = new Collection<busBenefitCalculationFasMonths>();
            lclbFinalSalaryRecords = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>
                                    ("icdoPersonAccountRetirementContribution.salary_amount desc," +
                                     "icdoPersonAccountRetirementContribution.effective_date desc",
                                    lclbFinalSalaryRecords);

            //iclbFinalSalaryRecords = lclbFinalSalaryRecords;
            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lclbFinalSalaryRecords)
            {
                if (iclbBenefitCalculationFASMonths2020.Count <= 36) //ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods)
                {
                    busBenefitCalculationFasMonths lobjFASMonth = new busBenefitCalculationFasMonths
                    {
                        icdoBenefitCalculationFasMonths = new cdoBenefitCalculationFasMonths(),
                        ibusPersonAccount = new busPersonAccount
                        {
                            icdoPersonAccount = new cdoPersonAccount(),
                            ibusPlan = new busPlan { icdoPlan = new cdoPlan() }
                        }
                    };
                    lobjFASMonth.icdoBenefitCalculationFasMonths.fas_logic_id = 7018;
                    lobjFASMonth.icdoBenefitCalculationFasMonths.fas_logic_value = busConstant.FAS2020;
                    lobjFASMonth.icdoBenefitCalculationFasMonths.person_account_id =
                        lobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id;
                    lobjFASMonth.icdoBenefitCalculationFasMonths.month =
                        lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month;
                    lobjFASMonth.icdoBenefitCalculationFasMonths.year =
                        lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year;
                    lobjFASMonth.icdoBenefitCalculationFasMonths.effective_date = lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date;
                    lobjFASMonth.icdoBenefitCalculationFasMonths.salary_amount =
                        lobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount;
                    lobjFASMonth.icdoBenefitCalculationFasMonths.projected_flag =
                        lobjRetirementContribution.ibnlIsProjectedSalaryRecord == true ? busConstant.Flag_Yes : busConstant.Flag_No;
                    lobjFASMonth.ibusPersonAccount.ibusPlan = lobjRetirementContribution.ibusPARetirement.ibusPlan;
                    iclbBenefitCalculationFASMonths2020.Add(lobjFASMonth);
                }
            }
            lintFASDivider = iclbBenefitCalculationFASMonths2020.Where(lobjFAS => lobjFAS.icdoBenefitCalculationFasMonths.salary_amount > 0.0M).Count();
            idecTotalSalary2020 = 0.00M;
            foreach (busBenefitCalculationFasMonths lobjFASMonths in iclbBenefitCalculationFASMonths2020)
            {
                idecTotalSalary2020 += lobjFASMonths.icdoBenefitCalculationFasMonths.salary_amount;
            }
            adecComputedFAS2020 = idecTotalSalary2020;
            adecComputedFAS2020 = FormatFAS(idecTotalSalary2020, lintFASDivider, icdoBenefitCalculation.plan_id);
            adecCalculationFAS2020 = adecComputedFAS2020;

            if (icdoBenefitCalculation.overridden_final_average_salary > 0.00M)
            {
                adecCalculationFAS2020 = icdoBenefitCalculation.overridden_final_average_salary;
            }
        }
        // PIR 17924 - Load latest employment detail with contributing status.
        public DataTable LoadAllContributingEmploymentDetails()
        {
            return Select("cdoPersonEmployment.GetLatestContEmpDetail", new object[1] { icdoBenefitCalculation.person_id });         
        }

   //SP PIR-1054 FAS
        private void LoadBenefitProvisionBenefitTypeForDCUsingMainFasFactors()
        {
            if (ibusBenefitProvisionBenefitType == null)
            {
                ibusBenefitProvisionBenefitType = new busBenefitProvisionBenefitType();
                ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType = new cdoBenefitProvisionBenefitType();
            }
            if (icdoBenefitCalculation.retirement_date != DateTime.MinValue)
            {
                DataTable ldtbResult = new DataTable();
                //PIR 26544
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement();
                    lbusPersonAccountRetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);

                    ldtbResult = busBase.Select("cdoBenefitProvisionBenefitType.GetBenefitProvisionByPlan", new object[4]{
                                                    busConstant.PlanIdMain,
                                                    icdoBenefitCalculation.benefit_account_type_value,
                                                    icdoBenefitCalculation.retirement_date, lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value.IsNull() ? busConstant.MainBenefit1997Tier : lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value});
                    if (ldtbResult != null && ldtbResult.Rows.Count == 0)
                    {
                        ldtbResult = busBase.Select("cdoBenefitProvisionBenefitType.GetBenefitProvisionByPlan", new object[4]{
                                                    busConstant.PlanIdMain2020, //PIR 20232
                                                    icdoBenefitCalculation.benefit_account_type_value,
                                                    icdoBenefitCalculation.retirement_date, lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value});
                    }
                    if (ldtbResult.Rows.Count > 0)
                    {
                        ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.LoadData(ldtbResult.Rows[0]);
                    }
                }
            }
        }

        private bool IsMemberEligibleForHPIndexing(DateTime adtTerminationDate)
        {
            /********* UAT PIR: 1952. Rules regarding HP Indexing and When to Apply *************/
            /********* PROD PIR 2004 has modified rules for HP Indexing and when to apply *******/
            bool iblnIsMemberEligibleForHPindexing = false;
            decimal ldecTFFRService = 0.00M;
            decimal ldecTIAAService = 0.00M;
            decimal ldecTentativeTFFRService = 0.00M;
            decimal ldecTentativeTIAAService = 0.00M;
            decimal ldecTotalTIAAService = 0.0M;

            if (!iblnConsoldatedVSCLoaded)
                CalculateConsolidatedVSC();
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();

            ibusPersonAccount.LoadTFFRTIAAService(ref ldecTFFRService, ref ldecTIAAService, ref ldecTentativeTFFRService, ref ldecTentativeTIAAService);
            ldecTotalTIAAService = ldecTIAAService;
            if (icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimate) ||
                icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
            {
                ldecTotalTIAAService += ldecTentativeTIAAService;
            }

            if (!icdoBenefitCalculation.is_return_to_work_member) // BR-060-30 - The FAS should not be indexed for an HP Plan in case of RTW Member.
            {
                if ((icdoBenefitCalculation.credited_vsc - ldecTotalTIAAService) >= 120)
                {
                    int lintNumberofMonths = busGlobalFunctions.DateDiffByMonth(adtTerminationDate, icdoBenefitCalculation.retirement_date);

                    if ((lintNumberofMonths - 2) > 0)
                        iblnIsMemberEligibleForHPindexing = true;
                }
            }
            return iblnIsMemberEligibleForHPindexing;
        }

        /// <summary>
        /// Determines the Indexed FAS.
        /// </summary>
        /// <param name="adteTerminationDate">Termination Date</param>
        /// <param name="adteBenefitCommencementDate">Benefit Commencement Date</param>
        /// <param name="adecComputedFAS">Computed FAS</param>
        /// <param name="aintPlanID">Plan ID</param>
        /// <param name="adteCalculationDate">Calculation Date</param>
        /// <returns></returns>
        public decimal CalculateIndexedFAS(DateTime adteTerminationDate, DateTime adteBenefitCommencementDate, decimal adecComputedFAS,
                                                    int aintPlanID, DateTime adteCalculationDate)
        {
            decimal ldecIndexedFASAmount = 0.0M;
            Collection<busBenefitFasIndexFactor> lclbFASIndexFactor = new Collection<busBenefitFasIndexFactor>();
            iclbBenefitFASIndexing = new Collection<busBenefitFasIndexing>();

            DataTable ldtbResult = busBase.Select("cdoBenefitFasIndexFactor.LoadFASIndexingFactor", new object[4]{
                                                adteTerminationDate,adteBenefitCommencementDate,aintPlanID,adteCalculationDate});
            foreach (DataRow dr in ldtbResult.Rows)
            {
                busBenefitFasIndexFactor lobjFASIndexFactor = new busBenefitFasIndexFactor();
                lobjFASIndexFactor.icdoBenefitFasIndexFactor = new cdoBenefitFasIndexFactor();
                lobjFASIndexFactor.icdoBenefitFasIndexFactor.LoadData(dr);
                lclbFASIndexFactor.Add(lobjFASIndexFactor);
            }
            int lintCounter = 1;
            decimal ldecSalaryFactor = 1;
            foreach (busBenefitFasIndexFactor lobjFASFactor in lclbFASIndexFactor)
            {
                int lintDivisor = 0;
                int lintDiffInMonths = 0;
                decimal ldecAverageIncreaseFactorYr1 = 0.0M;
                decimal ldecAverageIncreaseFactorYr2 = 0.0M;
                DateTime ldtTempStartdate = lobjFASFactor.icdoBenefitFasIndexFactor.fas_start_date;
                DateTime ldtTempEndDate = lobjFASFactor.icdoBenefitFasIndexFactor.fas_end_date;
                decimal ldecAverageIncreaseFactorYr3 = lobjFASFactor.icdoBenefitFasIndexFactor.average_increase_factor;

                if (lintCounter == 1)
                {
                    lintDivisor = 1;
                }
                else if (lintCounter == 2)
                {
                    lintDivisor = 2;
                    ldecAverageIncreaseFactorYr2 = lclbFASIndexFactor[lintCounter - 2].icdoBenefitFasIndexFactor.average_increase_factor;
                }
                else
                {
                    lintDivisor = 3;
                    ldecAverageIncreaseFactorYr1 = lclbFASIndexFactor[lintCounter - 3].icdoBenefitFasIndexFactor.average_increase_factor;
                    ldecAverageIncreaseFactorYr2 = lclbFASIndexFactor[lintCounter - 2].icdoBenefitFasIndexFactor.average_increase_factor;
                }
                if ((lintCounter == 1) || (lintCounter == lclbFASIndexFactor.Count))
                {
                    ldtTempStartdate = lclbFASIndexFactor[lintCounter - 1].icdoBenefitFasIndexFactor.fas_start_date;
                    ldtTempStartdate = new DateTime(ldtTempStartdate.Year,
                                                                    ldtTempStartdate.Month, 01);
                    ldtTempEndDate = lclbFASIndexFactor[lintCounter - 1].icdoBenefitFasIndexFactor.fas_end_date;
                    ldtTempEndDate = new DateTime(ldtTempEndDate.Year,
                                                                    ldtTempEndDate.Month, 01);
                    ldtTempEndDate = ldtTempEndDate.AddMonths(1);


                    lintDiffInMonths = busGlobalFunctions.DateDiffByMonth(ldtTempStartdate,
                         ldtTempEndDate) - 1;
                    if (lintDiffInMonths != 0)
                    {
                        lintDiffInMonths = lintDiffInMonths - 1;
                    }
                }
                else
                {
                    lintDiffInMonths = 12;
                }

                busBenefitFasIndexing lobjFASIndexing = new busBenefitFasIndexing();
                lobjFASIndexing.icdoBenefitFasIndexing = new cdoBenefitFasIndexing();
                lobjFASIndexing.icdoBenefitFasIndexing.benefit_begin_date = lobjFASFactor.icdoBenefitFasIndexFactor.fas_start_date;
                lobjFASIndexing.icdoBenefitFasIndexing.benefit_end_date = lobjFASFactor.icdoBenefitFasIndexFactor.fas_end_date;
                lobjFASIndexing.icdoBenefitFasIndexing.year_average_increase_factor = Convert.ToDecimal(lintDiffInMonths) / 12;
                lobjFASIndexing.icdoBenefitFasIndexing.average_increase_percentage = ((ldecAverageIncreaseFactorYr1 + ldecAverageIncreaseFactorYr2 +
                                                                                     ldecAverageIncreaseFactorYr3) / lintDivisor) + 1;
                decimal ldecPropotionateRate = Math.Round(((lobjFASIndexing.icdoBenefitFasIndexing.average_increase_percentage - 1) *
                                                lobjFASIndexing.icdoBenefitFasIndexing.year_average_increase_factor) + 1,6,MidpointRounding.AwayFromZero); 
                ldecSalaryFactor = ldecSalaryFactor * ldecPropotionateRate;
                lobjFASIndexing.icdoBenefitFasIndexing.salary_factor = ldecPropotionateRate;

                // PROD PIR ID 2004 -- Indexed HP salary amount will be persisted in benefit_calculation_fas_months table.
                // benefit_fas_indexing table is no more in use. No data will be persisted to this table.
                //iclbBenefitFASIndexing.Add(lobjFASIndexing);
                lintCounter++;
            }            
            ldecIndexedFASAmount = Math.Round((ldecSalaryFactor * adecComputedFAS), 2);
            return ldecIndexedFASAmount;
        }

        public void LoadBenefitFASIndexingMonths()
        {
            if (iclbBenefitFASIndexing == null)
                iclbBenefitFASIndexing = new Collection<busBenefitFasIndexing>();

            DataTable ldtbResult = Select<cdoBenefitFasIndexing>(new string[1] { "benefit_calculation_id" }, new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            iclbBenefitFASIndexing = GetCollection<busBenefitFasIndexing>(ldtbResult, "icdoBenefitFasIndexing");
        }

        #endregion

        #region Benefit Multiplier

        public decimal idecFASBenefitMultiplier { get; set; }

        public bool iblnBenefitMultiplierLoaded = false;
        public void CalculateBenefitMultiplier()
        {
            decimal adecFASMultiplier = 0.0M;
            if (!iblnConsolidatedPSCLoaded)
                CalculateConsolidatedPSC();
            if (!iblnConsoldatedVSCLoaded)
                CalculateConsolidatedVSC();
            if (iclbBenefitMultiplier.IsNull())
                iclbBenefitMultiplier = new Collection<busBenefitMultiplier>();

            //PIR 23359
            DateTime ldtEarlyStartDate = ibusPersonAccount.GetEarlyPlanParticiaptionStartDate();
            //PIR 25729
            if (icdoBenefitCalculation.retirement_date != DateTime.MinValue && 
                busGlobalFunctions.GetLatestBenefitProvisionTypeByEffectiveDate(icdoBenefitCalculation.retirement_date, 
				ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_provision_id) == busConstant.BenefitProvisionTypeRTDT)
            {
                ldtEarlyStartDate = icdoBenefitCalculation.retirement_date;
            }

            if (ldtEarlyStartDate == DateTime.MinValue)
                ldtEarlyStartDate = ibusPersonAccount.icdoPersonAccount.start_date;

            iclbBenefitMultiplier = CalculateBenefitProvisionMultiplier(
                                                ldtEarlyStartDate,
                                                ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_provision_id,
                                                icdoBenefitCalculation.benefit_account_type_value,
                                                (icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService) ? Math.Round(icdoBenefitCalculation.consolidated_psc_in_years, 5, MidpointRounding.AwayFromZero) :
                                                Math.Round(icdoBenefitCalculation.consolidated_psc_in_years, 4, MidpointRounding.AwayFromZero),
                                                icdoBenefitCalculation.calculation_final_average_salary,
                                                ref adecFASMultiplier);
            idecFASBenefitMultiplier = adecFASMultiplier;
            iblnBenefitMultiplierLoaded = true;
        }

        public Collection<busBenefitMultiplier> CalculateBenefitProvisionMultiplier(DateTime adteCalculationDate, int aintBenefitProvisionID,
                                                               string astrBenefitAccountType, decimal adecYearsOfService, decimal adecOptFinalAverageSalary,
                                                               ref decimal adecTotalbenefit_Multiplier_current_service)
        {
            adecTotalbenefit_Multiplier_current_service = 0.0M;
            Collection<cdoBenefitProvisionMultiplier> lclbBenefitProvisionMultiplier = new Collection<cdoBenefitProvisionMultiplier>();
            lclbBenefitProvisionMultiplier = LoadBenefitProvisionMultiplier(adteCalculationDate, aintBenefitProvisionID, astrBenefitAccountType);
            Collection<busBenefitMultiplier> lclbBenefitMultiplier = new Collection<busBenefitMultiplier>();
            foreach (cdoBenefitProvisionMultiplier lcdoBenefitProvisionMultiplier in lclbBenefitProvisionMultiplier)
            {
                busBenefitMultiplier lobjBenefitMultiplier = new busBenefitMultiplier();
                lobjBenefitMultiplier.icdoBenefitMultiplier = new cdoBenefitMultiplier();
                lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_rate = lcdoBenefitProvisionMultiplier.multipier_percentage / 100;
                if (lcdoBenefitProvisionMultiplier.is_flat_percentage_flag == busConstant.Flag_Yes)
                {
                    adecTotalbenefit_Multiplier_current_service = Math.Round((adecOptFinalAverageSalary * (lcdoBenefitProvisionMultiplier.multipier_percentage / 100)), 2, MidpointRounding.AwayFromZero);
                    lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_amount = adecTotalbenefit_Multiplier_current_service;
                    lobjBenefitMultiplier.icdoBenefitMultiplier.pension_service_credit = Math.Round(adecYearsOfService, 4, MidpointRounding.AwayFromZero);
                    lclbBenefitMultiplier.Add(lobjBenefitMultiplier);
                    adecYearsOfService = 0;
                }
                else
                {
                    if (adecYearsOfService < lcdoBenefitProvisionMultiplier.service_period)
                    {
                        if ((aintBenefitProvisionID == busConstant.JobServiceBenefitProvisionID) && !string.IsNullOrEmpty(astrBenefitAccountType))
                        {
                            /* Code Block Commented as per latest master Bosch Example for JS UAT PIR:1641
                            ////UAT PIR: 1557. matched with SusanErbele.xls, Slice needs to be introduced for the job Service Sick leave conversion.
                            //if (ibusMember.IsNull())
                            //    LoadMember();
                            //if (ibusMember.icdoPerson.job_service_sick_leave > 0)
                            //{
                            //adecYearsOfService = Slice(adecYearsOfService, 3);
                            //}                          
                             * *  Code Block Commented as per latest master Bosch Example for JS UAT PIR:1641 */
                            adecTotalbenefit_Multiplier_current_service =
                                adecTotalbenefit_Multiplier_current_service + Slice((adecOptFinalAverageSalary * adecYearsOfService * (lcdoBenefitProvisionMultiplier.multipier_percentage / 100)), 2);
                            lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_amount = Slice((adecOptFinalAverageSalary * adecYearsOfService * (lcdoBenefitProvisionMultiplier.multipier_percentage / 100)), 2);
                        }
                        else
                        {
                            adecTotalbenefit_Multiplier_current_service =
                                adecTotalbenefit_Multiplier_current_service + Math.Round((adecOptFinalAverageSalary * adecYearsOfService * (lcdoBenefitProvisionMultiplier.multipier_percentage / 100)), 2, MidpointRounding.AwayFromZero);
                            lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_amount = adecOptFinalAverageSalary * adecYearsOfService * (lcdoBenefitProvisionMultiplier.multipier_percentage / 100);
                        }
                        lobjBenefitMultiplier.icdoBenefitMultiplier.pension_service_credit = Math.Round(adecYearsOfService, 4, MidpointRounding.AwayFromZero);
                        lclbBenefitMultiplier.Add(lobjBenefitMultiplier);
                        adecYearsOfService = 0;
                    }
                    else
                    {
                        if ((aintBenefitProvisionID == busConstant.JobServiceBenefitProvisionID) && !string.IsNullOrEmpty(astrBenefitAccountType))
                        {
                            adecTotalbenefit_Multiplier_current_service = adecTotalbenefit_Multiplier_current_service +
                                Slice((adecOptFinalAverageSalary * lcdoBenefitProvisionMultiplier.service_period * (lcdoBenefitProvisionMultiplier.multipier_percentage / 100)), 2);

                            lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_amount = Slice((adecOptFinalAverageSalary * lcdoBenefitProvisionMultiplier.service_period * (lcdoBenefitProvisionMultiplier.multipier_percentage / 100)), 2);
                        }
                        else
                        {
                            adecTotalbenefit_Multiplier_current_service = adecTotalbenefit_Multiplier_current_service +
                                Math.Round((adecOptFinalAverageSalary * lcdoBenefitProvisionMultiplier.service_period * (lcdoBenefitProvisionMultiplier.multipier_percentage / 100)), 2, MidpointRounding.AwayFromZero);
                            lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_amount = Math.Round((adecOptFinalAverageSalary * lcdoBenefitProvisionMultiplier.service_period * (lcdoBenefitProvisionMultiplier.multipier_percentage / 100)), 2, MidpointRounding.AwayFromZero);
                        }
                        lobjBenefitMultiplier.icdoBenefitMultiplier.pension_service_credit =
                            Math.Round(Convert.ToDecimal(lcdoBenefitProvisionMultiplier.service_period), 4, MidpointRounding.AwayFromZero);
                        lclbBenefitMultiplier.Add(lobjBenefitMultiplier);
                        adecYearsOfService = adecYearsOfService - lcdoBenefitProvisionMultiplier.service_period;
                    }
                }
                if (adecYearsOfService == 0)
                    break;
            }
            return lclbBenefitMultiplier;
        }

        #endregion

        #region DNRO

        public void CalculateDNROBenefitAmount(decimal adecMemberAge, decimal adecBenAge, decimal adecUnreducedAfterActuarialIncrease,
            decimal adecAdhocAmount, ref decimal adecDNROMissedPaymentAmount, ref decimal adecMonthlyActuarialIncrease, ref decimal adecDNROBenefitAmount)
        {
            DateTime ldtRetirementDate = icdoBenefitCalculation.retirement_date;
            DateTime ldtDNRORetirementDate = icdoBenefitCalculation.normal_retirement_date;
            DateTime ldtChangedEmpEnddate = icdoBenefitCalculation.termination_date.AddMonths(1);
            DateTime ldtDNROComapredate = DateTime.MinValue;
            // Maximum date of dtNormalRetirementDate and dtChangedEmpEnddate
            //UAT PIR: 1617
            //Unlike DNRO For other DB Plans , JobService requires no Gap between Termination date and Retirement Date.
            if (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService)
            {
                ldtDNROComapredate = DateTime.Compare(ldtDNRORetirementDate, ldtChangedEmpEnddate) > 0 ? ldtDNRORetirementDate : ldtChangedEmpEnddate;
            }
            else
            {
                ldtDNROComapredate = ldtDNRORetirementDate;
            }

            int lintDNROMonthsToIncrease = busGlobalFunctions.DateDiffByMonth(ldtDNROComapredate, ldtRetirementDate);
            if (lintDNROMonthsToIncrease != 0)
            {
                lintDNROMonthsToIncrease = lintDNROMonthsToIncrease - 1;
            }
            decimal ldecDNROFactor = 0.0M;
            if (lintDNROMonthsToIncrease != 0)
            {
                //TODO -DnRO Factor method
                ldecDNROFactor = GetDNROEarlyPLSOFactor(icdoBenefitCalculation.plan_id, adecMemberAge, adecBenAge,
                                                                busConstant.ApplicationBenefitSubTypeDNRO, string.Empty, DateTime.Today);
            }
            // PIR: UAT: 1133. When the number of months is 0. DNRO Factor not available warning message should not be raised.
            //else
            //{
            //    // When the Number of Months is 0 then no DNRO Factor is found. So in that case too the warning has to be raised.
            //    iblnIsDNROFactorNotExists = true;
            //}
            DateTime ldtPersLinkGoLiveDate = busPayeeAccountHelper.GetPERSLinkGoLiveDate();
            if (icdoBenefitCalculation.plan_id == busConstant.PlanIdJobService)
            {
                if ((adecMemberAge < 65) && (lintDNROMonthsToIncrease > 0))
                {
                    ldecDNROFactor = 1.0M;
                }
                /*Jobservice depends on the ldecDNROFactor since it is provided in direct percentages.so when ldecDNRO factor is 0, then the DNRO
                Benefit Amount would be 0 which should not happen.(ie Amount after DNRO Increase will be equal to 0-Unreduced Monthly Amount when 
                No DNRO factor is found.So set it to Unreduced Monthly Amount */

                if (ldecDNROFactor == 0.0M)
                {
                    adecDNROBenefitAmount = Math.Round((adecUnreducedAfterActuarialIncrease + adecAdhocAmount), 2);
                }
                else
                {
                    adecDNROBenefitAmount = Math.Round((adecUnreducedAfterActuarialIncrease + adecAdhocAmount) * ldecDNROFactor, 2);
                }

                adecMonthlyActuarialIncrease = adecDNROBenefitAmount - Math.Round(adecUnreducedAfterActuarialIncrease, 2);

                adecDNROMissedPaymentAmount = (lintDNROMonthsToIncrease * adecUnreducedAfterActuarialIncrease) + adecAdhocAmount;

                if ((icdoBenefitCalculation.overridden_dnro_missed_payment_amount > 0) && (ldtDNROComapredate < ldtPersLinkGoLiveDate))
                {
                    adecDNROMissedPaymentAmount = icdoBenefitCalculation.overridden_dnro_missed_payment_amount;
                }
            }
            else
            {
                adecDNROMissedPaymentAmount = (lintDNROMonthsToIncrease * adecUnreducedAfterActuarialIncrease) + adecAdhocAmount;
                if ((icdoBenefitCalculation.overridden_dnro_missed_payment_amount > 0) && (ldtDNROComapredate < ldtPersLinkGoLiveDate))
                {
                    adecDNROMissedPaymentAmount = icdoBenefitCalculation.overridden_dnro_missed_payment_amount;
                }
                adecMonthlyActuarialIncrease = Math.Round(((adecDNROMissedPaymentAmount * ldecDNROFactor) / 12), 2);
                adecDNROBenefitAmount = adecUnreducedAfterActuarialIncrease + adecMonthlyActuarialIncrease;
            }
            icdoBenefitCalculation.dnro_factor = ldecDNROFactor;
            icdoBenefitCalculation.dnro_missed_months = lintDNROMonthsToIncrease;
            icdoBenefitCalculation.dnro_monthly_increase = adecMonthlyActuarialIncrease;
            icdoBenefitCalculation.dnro_total_missed_amount = adecDNROMissedPaymentAmount;
            icdoBenefitCalculation.adhoc_or_cola_amount = adecAdhocAmount;
            if (adecUnreducedAfterActuarialIncrease != 0)
            {
                icdoBenefitCalculation.dnro_percentage_increase =
                    100 - ((adecDNROBenefitAmount * 100) / adecUnreducedAfterActuarialIncrease);
            }
        }

        public decimal GetDNROEarlyPLSOFactor(int aintPlanID, decimal adecMemberAge, decimal adecBenAge, string astrTrantype,
           string astSubtype, DateTime adtCalculationDate)
        {
            decimal ldecFactor = 0M;
            int lintMemberage = 0, lintBenAge = 0;
            if (adecMemberAge > 0)
            {
                lintMemberage = (int)adecMemberAge;
            }

            if (adecBenAge > 0)
            {
                lintBenAge = (int)adecBenAge;
            }

            if ((astrTrantype == busConstant.ApplicationBenefitSubTypeDNRO) || (astrTrantype == "PLSO"))
            {
                //since for DNRO and PLSO and for plans main, LE and NG Beneficiary Age does not exists. pass Ben age as 0.
                if ((aintPlanID == busConstant.PlanIdMain)
                || (aintPlanID == busConstant.PlanIdMain2020) //PIR 20232
                    || (aintPlanID == busConstant.PlanIdLE) || (aintPlanID == busConstant.PlanIdLEWithoutPS)
                || (aintPlanID == busConstant.PlanIdNG) || (aintPlanID == busConstant.PlanIdBCILawEnf) // pir 7943
                || (aintPlanID == busConstant.PlanIdStatePublicSafety)) //PIR 25729
                {
                    lintBenAge = 0;
                }
            }
            //UAT PIR: 2096 & 2095. For JobService Early or DNRO ,Ben Age is always Zero.
            if (((astrTrantype == busConstant.ApplicationBenefitSubTypeEarly) || (astrTrantype == busConstant.ApplicationBenefitSubTypeDNRO)) && (aintPlanID == busConstant.PlanIdJobService))
            {
                //For fetching Early factors of Jobservice we do not require the Beneficiary Age.
                lintBenAge = 0;
            }

            DateTime ldteRetirementDate = (icdoBenefitCalculation.retirement_date != DateTime.MinValue) ? icdoBenefitCalculation.retirement_date : DateTime.Today;
            DataTable ldtbResult = busBase.Select("cdoBenefitDnroPlsoEarlyFactor.LoadBenefitDnroPlsoEarlyFactor",
                                    new object[6] { astrTrantype, astSubtype, aintPlanID, ldteRetirementDate, lintMemberage, lintBenAge });
            foreach (DataRow dr in ldtbResult.Rows)
            {
                decimal ldecMemberage = 0.00M;
                decimal ldecBenAge = 0.00M;
                if (dr["BEN_AGE"] != DBNull.Value)
                {
                    ldecBenAge = Convert.ToDecimal(dr["BEN_AGE"]);
                }
                if (dr["MEMBER_AGE"] != DBNull.Value)
                {
                    ldecMemberage = Convert.ToDecimal(dr["MEMBER_AGE"]);
                }
                //UAT PIR: 2096 & 2095. For JobService Early or DNRO the age has to be compared with the  Decimal Age.
                if (((astrTrantype == busConstant.ApplicationBenefitSubTypeEarly) || (astrTrantype == busConstant.ApplicationBenefitSubTypeDNRO)) && (aintPlanID == busConstant.PlanIdJobService))
                {
                    if ((ldecBenAge == lintBenAge) && (ldecMemberage == adecMemberAge))
                    {
                        if (dr["FACTOR"] != DBNull.Value)
                            ldecFactor = Convert.ToDecimal(dr["FACTOR"]);
                    }
                }
                else
                {
                    if ((ldecBenAge == lintBenAge) && (ldecMemberage == lintMemberage))
                    {
                        if (dr["FACTOR"] != DBNull.Value)
                            ldecFactor = Convert.ToDecimal(dr["FACTOR"]);
                    }
                }
            }

            // PIR ID 1111 - Show Soft error if no Factor exists.
            if ((astrTrantype == busConstant.ApplicationBenefitSubTypeEarly) && (ldecFactor == 0M))
                iblnIsEarlyFactorNotExists = true;
            if ((astrTrantype == busConstant.ApplicationBenefitSubTypeDNRO) && (ldecFactor == 0M))
                iblnIsDNROFactorNotExists = true;
            if ((astrTrantype == "PLSO") && (ldecFactor == 0M))
                iblnIsPLSOFactorNotExists = true;

            return ldecFactor;
        }

        #endregion

        public void CalculateEARLYBenefitAmount(ref decimal adecEarlyReductionPercentage, ref decimal adecEarlyReductionAmount, decimal adecUnreducedAfterActuarialIncrease,
                        ref decimal adecEarlyReducedMonthlyBenefitAmount, decimal adecMemberAgeMontAndYear, decimal adecBenAgeMontAndYear)
        {
            string lstrSubType = string.Empty;
            decimal ldecEarlyFactor = 0.0M;

            if ((ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.early_reduction_method_value != null)
                && (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.early_reduction_method_value != "OTHR"))
            {
                DateTime ldtNormalCalcDate = icdoBenefitCalculation.normal_retirement_date;

                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdHP)
                {
                    if (ibusMember == null)
                        LoadMember();
                    ldtNormalCalcDate = ibusMember.icdoPerson.date_of_birth.AddYears(55).AddMonths(1);
                }
                int lintMonthsToReduce = busGlobalFunctions.DateDiffByMonth(icdoBenefitCalculation.retirement_date, ldtNormalCalcDate);
                if (lintMonthsToReduce != 0)
                {
                    lintMonthsToReduce = lintMonthsToReduce - 1;
                }
                decimal ldecEarlyFactorAge = 0.0M; decimal ldecMonthPart = 0.0M;
                if ((icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService) &&
                    (ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.early_reduction_method_value == "FAPM"))
                {
                    if (lintMonthsToReduce != 0)
                    {
                        ldecEarlyFactor = ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.early_reduction_factor;
                        adecEarlyReductionPercentage = lintMonthsToReduce * ldecEarlyFactor;
                        adecEarlyReductionPercentage = Math.Round(adecEarlyReductionPercentage, 4, MidpointRounding.AwayFromZero);
                        adecEarlyReductionAmount = (adecUnreducedAfterActuarialIncrease * adecEarlyReductionPercentage);
                        adecEarlyReductionAmount = Math.Round(adecEarlyReductionAmount, 2, MidpointRounding.AwayFromZero);
                    }
                    adecEarlyReducedMonthlyBenefitAmount = adecUnreducedAfterActuarialIncrease - adecEarlyReductionAmount;
                }
                else
                {
                    if (icdoBenefitCalculation.credited_psc >= 60)
                    {
                        int lintEarlyAge = 0;
                        if (icdoBenefitCalculation.credited_psc >= 240)
                        {
                            lintEarlyAge = 60;
                            lstrSubType = "60/20";
                        }
                        else
                        {
                            lintEarlyAge = 62;
                            lstrSubType = "62/05";
                        }
                        //UAT PIR: 2096 & 2095. For JobService DNRO it is the attained age.
                        DateTime ldtCalcDate = ibusMember.icdoPerson.date_of_birth.AddYears(lintEarlyAge).AddMonths(1);
                        lintMonthsToReduce = busGlobalFunctions.DateDiffByMonth(icdoBenefitCalculation.retirement_date, ldtCalcDate);
                        if (lintMonthsToReduce != 0)
                        {
                            lintMonthsToReduce = lintMonthsToReduce - 1;
                        }
                        int lintYearPart = lintMonthsToReduce / 12;
                        int lintMonthPart = lintMonthsToReduce - (lintYearPart * 12);
                        ldecMonthPart = Convert.ToDecimal(lintMonthPart) / 12;
                        ldecMonthPart = Math.Round(ldecMonthPart, 4, MidpointRounding.AwayFromZero);
                        ldecEarlyFactorAge = lintYearPart + ldecMonthPart;
                    }
                    ldecEarlyFactor = GetDNROEarlyPLSOFactor(icdoBenefitCalculation.plan_id, ldecEarlyFactorAge, adecBenAgeMontAndYear,
                                              busConstant.ApplicationBenefitSubTypeEarly, lstrSubType, DateTime.Today);
                    //ldecEarlyFactor = Slice(ldecEarlyFactor, 2);
                    adecEarlyReductionPercentage = (1 - ldecEarlyFactor);
                    adecEarlyReducedMonthlyBenefitAmount = Math.Round(adecUnreducedAfterActuarialIncrease * ldecEarlyFactor, 2, MidpointRounding.AwayFromZero);
                    adecEarlyReductionAmount = Slice(adecUnreducedAfterActuarialIncrease - adecEarlyReducedMonthlyBenefitAmount, 2);
                }
                icdoBenefitCalculation.early_reduction_factor = ldecEarlyFactor;
                icdoBenefitCalculation.early_reduced_months = lintMonthsToReduce;
                icdoBenefitCalculation.early_retirement_percentage_decrease = adecEarlyReductionPercentage;
                icdoBenefitCalculation.early_monthly_decrease = adecEarlyReductionAmount;
            }
        }

        public void CalculatePLSOBenefitAmount(decimal adecMemberAge, decimal adecBenAge,
                                decimal adecReducedMonthlyBenefitAmountAfterDNRO, ref decimal adecPLSOReductionAmt,
                                ref decimal adecPLSOReducedBenefitAmt, ref decimal adecPLSOLumpSumAmt, ref decimal adecPLSOFactor)
        {
            if (ibusBenefitProvisionBenefitType == null)
                LoadBenefitProvisionBenefitType();
            if ((ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.plso_factor_method_value != busConstant.FactorMethodValueOther) &&
                (!string.IsNullOrEmpty(ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.plso_factor_method_value)))
            {
                // TODO:
                if (((icdoBenefitCalculation.plan_id == busConstant.PlanIdHP) ||
                    (icdoBenefitCalculation.plan_id == busConstant.PlanIdJudges)) &&
                    (icdoBenefitCalculation.annuitant_age == 0M))
                {
                    CalculateAnnuitantAge();
                }
                adecPLSOFactor = GetDNROEarlyPLSOFactor(icdoBenefitCalculation.plan_id, adecMemberAge, adecBenAge, "PLSO", string.Empty, DateTime.Today);
                adecPLSOLumpSumAmt = adecReducedMonthlyBenefitAmountAfterDNRO * 12;
                adecPLSOReductionAmt = Math.Round((adecReducedMonthlyBenefitAmountAfterDNRO * adecPLSOFactor), 2);
                adecPLSOReducedBenefitAmt = adecReducedMonthlyBenefitAmountAfterDNRO - adecPLSOReductionAmt;
            }
        }

        public decimal CalculateUnReducedMonthlyBenefitAmount(decimal adecFinalFAS, decimal adecFASMultiplier, int aintPlanID,
                                                string astrBenefitType, decimal adecReducedAmount1, decimal adecReducedAmount2, string astrBenefitFormulaValue)
        {
            decimal adecUnReducedBenefitAmount = 0.0M;
            if (astrBenefitFormulaValue != busConstant.BenefitFormulaValueOther)
            {
                if (aintPlanID == busConstant.PlanIdJobService)
                {
                    adecUnReducedBenefitAmount = Slice(adecFASMultiplier, 2);
                }
                else
                {
                    adecUnReducedBenefitAmount =
                        Math.Round(((adecFASMultiplier) - (adecReducedAmount1 + adecReducedAmount2)), 2, MidpointRounding.AwayFromZero);
                }
            }
            return adecUnReducedBenefitAmount;
        }

        /// <summary>
        /// Determines the SSLI Factor
        /// </summary>
        /// <param name="aintPlanID">Plan ID</param>
        /// <param name="adteCalculationDate">Calculation Date</param>
        /// <param name="adecMemberAge">Member Age</param>
        /// <param name="adecSSLIAge">SSLI Age</param>
        /// <returns>SSLI Factor</returns>
        public decimal GetSSLIFactor(int aintPlanID, DateTime adteCalculationDate, decimal adecMemberAge, decimal adecSSLIAge)
        {
            decimal ldecSSLIFactor = 0.0M;
            int lintMemberAge = Convert.ToInt32((Slice(adecMemberAge, 0)));
            int lintSSLIAge = Convert.ToInt32((Slice(adecSSLIAge, 0)));

            // In order to optimize the result data, Member and SSLI Age is compared in query.
            DataTable ldtbResult =
                busBase.Select("cdoBenefitSsliFactor.GetSSLIFactor", new object[4] { aintPlanID, adteCalculationDate, lintMemberAge, lintSSLIAge });
            foreach (DataRow dr in ldtbResult.Rows)
            {
                busBenefitSSLIFactor lobjSSLIFactor = new busBenefitSSLIFactor();
                lobjSSLIFactor.icdoBenefitSsliFactor = new cdoBenefitSsliFactor();
                lobjSSLIFactor.icdoBenefitSsliFactor.LoadData(dr);
                if ((lobjSSLIFactor.icdoBenefitSsliFactor.member_age == adecMemberAge) &&
                   (lobjSSLIFactor.icdoBenefitSsliFactor.ssli_age == adecSSLIAge))
                {
                    ldecSSLIFactor = lobjSSLIFactor.icdoBenefitSsliFactor.ssli_factor;
                    break;
                }
            }
            return ldecSSLIFactor;
        }

        /// <summary>
        /// Determines the Benefit Option Factor.
        /// </summary>
        /// <param name="adecMemberAge">Member Age</param>
        /// <param name="adecBeneficiaryAge">Beneficiary Age</param>
        /// <param name="astrBenefitType">Benefit Option Type</param>
        /// <param name="aintPlanID">Plan ID</param>
        /// <param name="astrOptionType">Benefit Option Type</param>
        /// <returns>Benefit Option Factor</returns>

        public DataTable idtbBenOptionFactor { get; set; }
        public bool iblnUseDataTableForBenOptionFactor = false;
        public decimal GetOptionFactorsForPlan(decimal adecMemberAge, decimal adecBeneficiaryAge,
                                                                    string astrBenefitType, int aintPlanID, string astrOptionType)
        {
            decimal ldecFactor = 0M;
            decimal ldecMemberAge = Slice(adecMemberAge, 0);
            decimal ldecBeneficiaryAge = Slice(adecBeneficiaryAge, 0);
            DataTable ldtbResult = null;
            DateTime ldteRetirementDate = (icdoBenefitCalculation.retirement_date != DateTime.MinValue) ? icdoBenefitCalculation.retirement_date : DateTime.Today;
            if (iblnUseDataTableForBenOptionFactor)
            {
                if (idtbBenOptionFactor == null)
                {
                    idtbBenOptionFactor = Select<cdoBenefitOptionFactor>(
                               new string[0] { },
                               new object[0] { }, null, null);
                }

                ldtbResult = idtbBenOptionFactor.AsEnumerable().Where(i => i.Field<int>("PLAN_ID") == aintPlanID &&
                                                                                   i.Field<string>("BENEFIT_OPTION_VALUE") == astrOptionType &&
                                                                                   i.Field<string>("BENEFIT_TYPE") == astrBenefitType &&
                                                                                   i.Field<decimal>("MEMBER_AGE") == ldecMemberAge &&
                                                                                   ((i.IsNull("BEN_AGE") && 0 == ldecBeneficiaryAge) ||
                                                                                   ((!(i.IsNull("BEN_AGE"))) && i.Field<decimal>("BEN_AGE") == ldecBeneficiaryAge))
                                                                                   && i.Field<DateTime>("EFFECTIVE_DATE") <= ldteRetirementDate).OrderByDescending(i => i.Field<DateTime>("EFFECTIVE_DATE")).AsDataTable();
            }
            else
            {
                ldtbResult = busBase.Select("cdoBenefitOptionFactor.GetOptionsFactorByPlan", new object[6]{
                                                aintPlanID, astrOptionType, astrBenefitType, ldecMemberAge, ldecBeneficiaryAge, ldteRetirementDate});
            }
            if(ldtbResult.Rows.Count > 0)
            {
                ldecFactor = Convert.ToDecimal(ldtbResult.Rows[0]["FACTOR"]);
            }
            return ldecFactor;
        }

        public bool iblnConsolidatedPSCLoaded = false;
        public void CalculateConsolidatedPSC()
        {
            decimal ldecConsolidatedPSC = 0M;
            int lintEstimatesContributions = 0;
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            DateTime ldteEffectiveDate = (icdoBenefitCalculation.is_dro_estimate == busConstant.Flag_Yes) ?
                                            icdoBenefitCalculation.termination_date.GetLastDayofMonth() : icdoBenefitCalculation.retirement_date; // PROD PIR ID 1414

            ibusPersonAccount.LoadTotalPSC(ldteEffectiveDate, alblnIsDROEstimate: (icdoBenefitCalculation.is_dro_estimate == busConstant.Flag_Yes) ? true : false);
            /* UAT PIR : 935 FAS projection for seasonal data */
            if (ibusMember == null)
                LoadMember();
            if (!ibusMember.iblnIsEmploymentSeasonalLoaded)
                ibusMember.LoadEmploymentSeasonal();

            // UCS-060 Return To Work
            if ((!icdoBenefitCalculation.is_return_to_work_member) ||
                ((icdoBenefitCalculation.is_return_to_work_member) && (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_No_Value.ToUpper())) ||
                 (icdoBenefitCalculation.is_rtw_member_subsequent_retirement || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
            {
                ldecConsolidatedPSC = ibusPersonAccount.icdoPersonAccount.Total_PSC;
            }
            if (icdoBenefitCalculation.is_return_to_work_member && (!icdoBenefitCalculation.is_rtw_member_subsequent_retirement
                && icdoBenefitCalculation.calculation_type_value != busConstant.CalculationTypeEstimateSubsequent))
            {
                if (iclbBenefitCalculationPersonAccount == null)
                    LoadBenefitCalculationPersonAccount();
                foreach (busBenefitCalculationPersonAccount lobjBenCalcPersonAccount in iclbBenefitCalculationPersonAccount)
                {
                    if (lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.is_person_account_selected_flag == busConstant.Flag_Yes)
                    {
                        if (lobjBenCalcPersonAccount.ibusPersonAccount == null)
                            lobjBenCalcPersonAccount.LoadPersonAccount();
                        lobjBenCalcPersonAccount.ibusPersonAccount.LoadTotalPSC(ldteEffectiveDate, 
                                    alblnIsDROEstimate: (icdoBenefitCalculation.is_dro_estimate == busConstant.Flag_Yes) ? true : false);
                        ldecConsolidatedPSC += lobjBenCalcPersonAccount.ibusPersonAccount.icdoPersonAccount.Total_PSC;
                    }
                }
            }
            /* UAT PIR:950. Do not Include the TFFR Service for PSC calculation.             
             */
            //if ((!IsJobService) &&
            //    (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
            //    icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments))//to handle when RecalculateBenefit is clicked from PA
            //{
            //    decimal ldecTFFRService = 0.00M;
            //    decimal ldecTIAAService = 0.00M;
            //    if (ibusBenefitApplication == null)
            //        LoadBenefitApplication();
            //    if (ibusBenefitApplication.icdoBenefitApplication.tffr_calculation_method_value == "MET2")
            //    {
            //        ibusPersonAccount.LoadTFFRTIAAService(ref ldecTFFRService, ref ldecTIAAService);
            //        ldecConsolidatedPSC += ldecTFFRService;
            //    }
            //}
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) //PIR 19594
            {   

                /// //PIR : 14177 - Check if at least one ACTIVE Employment Detail with CONTRIBUTING STATUS exists.  
                //If there is only 1 Non Contributing, LOA, or LOAM we do not have to project. 
                //If there are two employment records; one Contributing and one Non Contributing we still need to project
               
                if (busPersonBase.CheckIfEmploymentISContributingStatus(ibusPersonAccount))
                { 
                    //PIR:1481 Created common function to be invoked
                    lintEstimatesContributions = GetProjectedEstimateServiceCredit(ibusMember.iblnIsEmploymentSeasonal, ibusMember.iintSeasonalMonths, true);
                    icdoBenefitCalculation.projected_psc = lintEstimatesContributions;
                }
                icdoBenefitCalculation.credited_psc_from_file = ldecConsolidatedPSC; // PIR 9447
                if (idecRemainingServiceCredit == 0.00M)
                    LoadRemainingServicePurchaseCredit();
                if (icdoBenefitCalculation.is_created_from_portal == busConstant.Flag_Yes)
                {
                    // PIR 11386
                    if (ibusBenefitCalculatorWeb.IsNull()) LoadBenefitcalculatorWeb();
                    busWssBenefitCalculator lbusBenefitCalculatorSummary = new busWssBenefitCalculator();
                    lbusBenefitCalculatorSummary.FindWssBenefitCalculator(ibusBenefitCalculatorWeb.icdoWssBenefitcalculator.wss_benefit_calculator_id);
                    lbusBenefitCalculatorSummary.LoadbusUnusedServicePurchaseHeader();
                    lbusBenefitCalculatorSummary.ibusUnusedServicePurchaseHeader.LoadServicePurchaseDetail();
                    lbusBenefitCalculatorSummary.LoadbusConsolidatedServicePurchaseHeader();
                    lbusBenefitCalculatorSummary.ibusConsolidatedServicePurchaseHeader.LoadServicePurchaseDetail();
                    idecRemainingServiceCredit +=
                    lbusBenefitCalculatorSummary.ibusConsolidatedServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.RoundedTotalTimeOfPurchaseExcludeFreeService
                    + lbusBenefitCalculatorSummary.ibusUnusedServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase;
                }
                ldecConsolidatedPSC = ldecConsolidatedPSC + idecRemainingServiceCredit
                                        + icdoBenefitCalculation.adjusted_psc + lintEstimatesContributions;
                icdoBenefitCalculation.estimated_credited_psc = ldecConsolidatedPSC; // PIR 9447
            }
            //Prod PIR: 4154
            //The Service Credit That has been posted after the Retirement date also needs to be Considered.
            if (ibusPersonAccount.IsNotNull())
            {
                ldecConsolidatedPSC = ldecConsolidatedPSC + ibusPersonAccount.GetTotalServicePurchaseCreditPostedAfterRetirementDate(true, ldteEffectiveDate);
            }
            
            if (IsJobService)
            {
                icdoBenefitCalculation.credited_psc = Slice(ldecConsolidatedPSC + ibusMember.icdoPerson.job_service_sick_leave, 4); // PROD PIR ID 7625
            }
            else
            {
                icdoBenefitCalculation.credited_psc = Math.Round(ldecConsolidatedPSC, 4, MidpointRounding.AwayFromZero);
            }
            iblnConsolidatedPSCLoaded = true;
        }

        /* UAT PIR : 935 FAS projection for seasonal data */
        public int CalculateProjectedSeasonalCredits(DateTime adtStartDate, DateTime adtEnddate, int aintSeasonalLimit, DateTime adtRetirementDate)
        {
            int lintEstimatesContributions = 0;
            DateTime idteProjectedDateStart = adtStartDate.AddMonths(1);

            bool iblnIsProrationRequired = false;
			//PIR 27199
            if (busGlobalFunctions.DateDiffByMonth(adtStartDate, adtEnddate) - 1 < aintSeasonalLimit)
            {
                iblnIsProrationRequired = true;
            }

            if (iblnIsProrationRequired)
            {
                decimal ldecTempProratedPSC = 0.0M;
                ldecTempProratedPSC = Math.Round((Convert.ToDecimal(aintSeasonalLimit) / 12.0M) * (Convert.ToDecimal(busGlobalFunctions.DateDiffByMonth(adtStartDate, adtEnddate) - 1)), 0, MidpointRounding.AwayFromZero);
                lintEstimatesContributions = Convert.ToInt32(ldecTempProratedPSC);
            }
            else
            {

                while (idteProjectedDateStart <= adtEnddate)
                {
                    if (IsFiscalMonthForSeasonalEligible(idteProjectedDateStart.Month, aintSeasonalLimit))
                    {
                        lintEstimatesContributions = lintEstimatesContributions + 1;
                    }
                    idteProjectedDateStart = idteProjectedDateStart.AddMonths(1);
                }
            }
            return lintEstimatesContributions;
        }
        public bool iblnConsoldatedVSCLoaded = false;
        public void CalculateConsolidatedVSC(bool ablnIsFromPensionFile = false)
        {
            int lintEstimatesContributions = 0;
            decimal ldecConsolidatedVSC = 0.00M;
            DateTime ldteEffectiveDate = (icdoBenefitCalculation.is_dro_estimate == busConstant.Flag_Yes) ? 
                                        icdoBenefitCalculation.termination_date.GetLastDayofMonth() : icdoBenefitCalculation.retirement_date; // PROD PIR ID 1414
            //PROD PIR 7330 : From MAS we need to calculate VSC as of statement effective date 
            if (ibusPersonAccount != null && ibusPersonAccount.idtMASStatementEffectiveDate != DateTime.MinValue)
                ldteEffectiveDate = ibusPersonAccount.idtMASStatementEffectiveDate.AddDays(1);
            if (ibusMember == null)
                LoadMember();
            if (!ibusMember.iblnIsEmploymentSeasonalLoaded)
                ibusMember.LoadEmploymentSeasonal();

            //UAT PIR: 950. Include Tentative TFFR and TIAA only when it is Estimate.
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate 
                || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)   //PIR 19594
            {

                /// //PIR : 14177 - Check if at least one ACTIVE Employment Detail with CONTRIBUTING STATUS exists.  
                //If there is only 1 Non Contributing, LOA, or LOAM we do not have to project. 
                //If there are two employment records; one Contributing and one Non Contributing we still need to project

                if (ibusPersonAccount == null)
                    LoadPersonAccount();

                if (busPersonBase.CheckIfEmploymentISContributingStatus(ibusPersonAccount))
                {
                    //PIR:1481 Created common function to be invoked
                    lintEstimatesContributions = GetProjectedEstimateServiceCredit(ibusMember.iblnIsEmploymentSeasonal, ibusMember.iintSeasonalMonths, false);
                    icdoBenefitCalculation.projected_vsc = lintEstimatesContributions;
                }                
                if (idecRemainingServiceCredit == 0.00M)
                    LoadRemainingServicePurchaseCredit();
                ldecConsolidatedVSC = ibusMember.GetTotalVSCForPerson(IsJobService, ldteEffectiveDate,
                    (icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimate) ||
                     icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimateSubsequent)), false, //PIR 19594
                    ablnIsDROEstimate: (icdoBenefitCalculation.is_dro_estimate == busConstant.Flag_Yes) ? true : false, ablnIsFromPensionFile: ablnIsFromPensionFile, iintBenefitPlanId: icdoBenefitCalculation.plan_id) +
                    idecRemainingServiceCredit + lintEstimatesContributions + icdoBenefitCalculation.adjusted_tvsc;

                icdoBenefitCalculation.idecCredited_Vsc_From_File = ibusMember.idecVSCForSelectedPlan + idecRemainingServiceCredit + icdoBenefitCalculation.adjusted_tvsc;
                icdoBenefitCalculation.idecCredited_Vsc_From_File_For_OtherPlans = ibusMember.idecVSCForOtherPlan;
            }
            else
            {
                ldecConsolidatedVSC = ibusMember.GetTotalVSCForPerson(IsJobService, ldteEffectiveDate,
                    icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimate) ||
                    (icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimateSubsequent)), false, //PIR 19594 
                    ablnIsDROEstimate: (icdoBenefitCalculation.is_dro_estimate == busConstant.Flag_Yes) ? true : false, ablnIsFromPensionFile : ablnIsFromPensionFile); // PROD PIR ID 1414
            }

            // UCS-060 Return to Work Member
            if ((icdoBenefitCalculation.is_return_to_work_member) &&
                (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper()))
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                ibusPersonAccount.LoadTotalVSC();
                ldecConsolidatedVSC -= ibusPersonAccount.icdoPersonAccount.Total_VSC;
            }

            //Prod PIR: 4154
            //The Service Credit That has been posted after the Retirement date also needs to be Considered.
            if (ibusPersonAccount.IsNotNull())
            {
                ldecConsolidatedVSC = ldecConsolidatedVSC + ibusPersonAccount.GetTotalServicePurchaseCreditPostedAfterRetirementDate(false, ldteEffectiveDate);
            }

            if (IsJobService)
            {
                icdoBenefitCalculation.credited_vsc = Slice(ldecConsolidatedVSC + ibusMember.icdoPerson.job_service_sick_leave, 4); // PROD PIR ID 7625
            }
            else
            {
                icdoBenefitCalculation.credited_vsc = Math.Round(ldecConsolidatedVSC, 4, MidpointRounding.AwayFromZero);
            }
            iblnConsoldatedVSCLoaded = true;
        }

        #region Retiree Health Insurance Credit

        public Collection<busCodeValue> GetAvailableRHICOptionDetails(string astrBenefitOption, string astrRHICSelected)
        {
            Collection<busCodeValue> lclbCodeValue = new Collection<busCodeValue>();
            Collection<busCodeValue> lclbFinalCodeValue = new Collection<busCodeValue>();
            lclbCodeValue = busGlobalFunctions.LoadCodeValueByData1(2317, astrBenefitOption, "Code_value_order Asc");
            if (astrRHICSelected == null)
                astrRHICSelected = string.Empty;

            foreach (busCodeValue lobjbusCodeValue in lclbCodeValue)
            {
                if (astrRHICSelected != string.Empty)
                {
                    if (lobjbusCodeValue.icdoCodeValue.data2 == astrRHICSelected)
                    {
                        lclbFinalCodeValue.Add(lobjbusCodeValue);
                    }
                }
                else
                {
                    lclbFinalCodeValue.Add(lobjbusCodeValue);
                }
            }
            return lclbFinalCodeValue;
        }

        public decimal GetRHICEARLYReductionFactor(int aintPlanID, decimal adecMemberAge, DateTime adteCalculationDate)
        {
            decimal adecReductionFactor = 0.0M;
            DataTable ldtbResult = busBase.SelectWithOperator<cdoBenefitRhicReductionFactor>(new string[2] { "plan_id", "effective_date" },
                                            new string[2] { "=", "<=" },
                                            new object[2] { aintPlanID, adteCalculationDate }, "effective_date desc");
            foreach (DataRow dr in ldtbResult.Rows)
            {
                busBenefitRHICReductionFactor lobjReductionFactor = new busBenefitRHICReductionFactor();
                lobjReductionFactor.icdoBenefitRhicReductionFactor = new cdoBenefitRhicReductionFactor();
                lobjReductionFactor.icdoBenefitRhicReductionFactor.LoadData(dr);
                if ((adecMemberAge >= lobjReductionFactor.icdoBenefitRhicReductionFactor.min_age) &&
                    (adecMemberAge < lobjReductionFactor.icdoBenefitRhicReductionFactor.max_age))
                {
                    adecReductionFactor = lobjReductionFactor.icdoBenefitRhicReductionFactor.factor;
                    break;
                }
            }
            return adecReductionFactor;
        }

        public Collection<busBenefitProvisionBenefitOption> GetAvailableOptionsDetails(int aintBenefitProvisionID, DateTime adteCalculationDate,
                                                    string astrBenefitAccountType, string astrBenefitOptionValue, string astrSSLIFlag)
        {
            if (astrBenefitOptionValue == null)
                astrBenefitOptionValue = string.Empty;
            Collection<busBenefitProvisionBenefitOption> lclbBenefitProvisionOption = new Collection<busBenefitProvisionBenefitOption>();
            if (!string.IsNullOrEmpty(astrBenefitAccountType))
            {
                // astrSSLIFlag Null is handled in query
                DataTable ldtbResult = busBase.Select("cdoBenefitProvisionBenefitOption.GetBenefitProvisionOptionDetails", new object[5]{
                                            adteCalculationDate,astrBenefitAccountType,astrBenefitOptionValue,aintBenefitProvisionID,astrSSLIFlag ?? string.Empty});
                foreach (DataRow dr in ldtbResult.Rows)
                {
                    busBenefitProvisionBenefitOption lobjBenefitProvisionOption = new busBenefitProvisionBenefitOption();
                    lobjBenefitProvisionOption.icdoBenefitProvisionBenefitOption = new cdoBenefitProvisionBenefitOption();
                    lobjBenefitProvisionOption.icdoBenefitProvisionBenefitOption.LoadData(dr);
                    lclbBenefitProvisionOption.Add(lobjBenefitProvisionOption);
                }
            }
            return lclbBenefitProvisionOption;
        }

        //todo option value pass
        public Collection<busBenefitRHICOption> CalculateBenefitAmountForRHICOptionFactor(string astrCalculationType, string astrBenefitAccountType,
                                decimal adecStdRHICAmount, DateTime adteMemberDOB, DateTime adteBeneficiaryDOB,
                                string astrBenefitOptionValue,
                                bool ablnIsWaiveEarlyReduction, bool ablnIsJointAnnuitantExists, string astrBenefitSubType, DateTime adteRetirementDate,
                                DateTime adteCalculationDate, string astrRHICOption, ref decimal adecRHICEARLYReductionAmount,
                                ref decimal adecRHICAfterEARLYReduction, ref decimal adecReductionFactor, bool bIsCalledFromMASBatch)
        {
            busBase lobjBase = new busBase();
            Collection<busBenefitRHICOption> lclbBenefitRHICOption = new Collection<busBenefitRHICOption>();

            //UAT PIR: 1170
            //The Subtype will be set for DC only when they are eligible for Normal or Early for a Main calculation.So when it is not set then no need to calculate Early or RHIC options.
            if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdDC ||
                icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020 || icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025) && (astrBenefitSubType.IsNullOrEmpty())) //PIR 20232 //PIR 25920
            {
                return lclbBenefitRHICOption;
            }
            decimal adecMemberAge = 0M;
            decimal adecBeneficiaryAge = 0M;
            int lintTempPlanID = icdoBenefitCalculation.plan_id;
            int lintEarlyPlanID = 0;
            int lintEarlyMemberAge = 0;
            CalculatePersonAge(adteMemberDOB, icdoBenefitCalculation.retirement_date, ref adecMemberAge, 4);
            if (ablnIsJointAnnuitantExists)
                CalculatePersonAge(adteBeneficiaryDOB, icdoBenefitCalculation.retirement_date, ref adecBeneficiaryAge, 4);

            adecRHICAfterEARLYReduction = adecStdRHICAmount;
            bool lblnEarlyFetch = false;
            //UAT PIR: 1316. If the Retirement Date is equal to the NRD then no Reduction
            //Should be Applied.
            if (!(ablnIsWaiveEarlyReduction))
            {
                DateTime ldtNormalCalcDate = icdoBenefitCalculation.normal_retirement_date;
                if (ldtNormalCalcDate != DateTime.MinValue)
                {
                    int lintMonthsToReduce = busGlobalFunctions.DateDiffByMonth(icdoBenefitCalculation.retirement_date, ldtNormalCalcDate);
                    if (lintMonthsToReduce != 0)
                    {
                        lintMonthsToReduce = lintMonthsToReduce - 1;
                    }
                    if (lintMonthsToReduce == 0)
                    {
                        ablnIsWaiveEarlyReduction = true;
                    }
                }
            }
            if ((!(ablnIsWaiveEarlyReduction)) && (astrBenefitSubType == busConstant.ApplicationBenefitSubTypeEarly) &&
                                (astrBenefitAccountType == busConstant.ApplicationBenefitTypeRetirement))
            {
                lblnEarlyFetch = true;
                if (icdoBenefitCalculation.plan_id == busConstant.PlanIdDC ||
                    icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025 || //PIR 25920
                   icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020) //PIR 20232
                {
                    lblnEarlyFetch = false;
                    if (ibusMember == null)
                        LoadMember();
                    //UAT PIR:2077 Changes.            
                    if (ibusMember.IsFormerDBPlanTransfertoDC(busConstant.PlanIdNG))
                    {
                        lintTempPlanID = busConstant.PlanIdNG;
                        lintEarlyPlanID = lintTempPlanID;
                        lblnEarlyFetch = true;
                    }
                    else if (ibusMember.IsFormerDBPlanTransfertoDC(busConstant.PlanIdMain))
                    {
                        lblnEarlyFetch = true;
                        lintEarlyPlanID = busConstant.PlanIdMain;
                    }
                    else if (ibusMember.IsFormerDBPlanTransfertoDC(busConstant.PlanIdMain2020))//PIR 20232 ?code
                    {
                        lblnEarlyFetch = true;
                        lintEarlyPlanID = busConstant.PlanIdMain2020;
                    }

                    //UAT PIR:2077 If For DC plan the person is less than early Retirement age of MAin or NG, then set his age as Early age (For Calculation Purpose only)
                    if (lintEarlyPlanID != 0)
                    {
                        busPlan ibusTempPlan = new busPlan { icdoPlan = new cdoPlan() };
                        ibusTempPlan.FindPlan(lintEarlyPlanID);
                        if (ibusPersonAccount.IsNull())
                            LoadPersonAccount();

                        Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionEarlyEligibility = new Collection<cdoBenefitProvisionEligibility>();
                        lclbBenefitProvisionEarlyEligibility = LoadEligibilityForPlan(lintEarlyPlanID, ibusTempPlan.icdoPlan.benefit_provision_id, astrBenefitAccountType, busConstant.BenefitProvisionEligibilityEarly, iobjPassInfo,
                                                                ibusPersonAccount?.icdoPersonAccount?.start_date);

                        if (lclbBenefitProvisionEarlyEligibility.Count() > 0)
                        {
                            lintEarlyMemberAge = Convert.ToInt32(lclbBenefitProvisionEarlyEligibility[0].age);
                            if (adecMemberAge < lintEarlyMemberAge)
                            {
                                adecMemberAge = lintEarlyMemberAge;
                            }
                        }
                    }
                }
                if (lblnEarlyFetch)
                {
                    adecReductionFactor = GetRHICEARLYReductionFactor(lintTempPlanID, adecMemberAge, adteCalculationDate);
                    // PIR ID 1237
                    //if (aintPlanID == busConstant.PlanIdJobService)
                    //{
                    //    adecRHICEARLYReductionAmount = Slice((adecStdRHICAmount * adecReductionFactor), 2);
                    //}
                    //else
                    //{
                    adecRHICEARLYReductionAmount = Math.Round(adecStdRHICAmount * adecReductionFactor, 2, MidpointRounding.AwayFromZero);
                    //}
                    //adecRHICAfterEARLYReduction = adecRHICAfterEARLYReduction - adecRHICEARLYReductionAmount;
                    adecRHICAfterEARLYReduction = Math.Round(((1 - adecReductionFactor) * adecStdRHICAmount), 2, MidpointRounding.AwayFromZero);
                }
            }
            if (!bIsCalledFromMASBatch)
            {
                if (astrBenefitOptionValue == null)
                    astrBenefitOptionValue = string.Empty;

                LoadBenefitProvisionBenefitType(adteCalculationDate); // PROD PIR ID 5867 - Calculate based on Calculation date.
                // Load All Available RHIC Benefit Options
                Collection<busBenefitProvisionBenefitOption> lclbProvisionBenefitOption =
                    GetAvailableOptionsDetails(ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.benefit_provision_id, adteCalculationDate,
                                                astrBenefitAccountType, astrBenefitOptionValue, icdoBenefitCalculation.uniform_income_or_ssli_flag);

                // PIR ID 1319
                // Filter the available RHIC Benefit Options
                Collection<busBenefitProvisionBenefitOption> lclbFinalProvisionBenefitOption = new Collection<busBenefitProvisionBenefitOption>();
                if (icdoBenefitCalculation.plan_id != busConstant.PlanIdDC &&
                    icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2020 && //PIR 20232
                    icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2025) //PIR 25920
                {
                    if (iclbBenefitCalculationOptions != null)
                    {
                        foreach (busBenefitProvisionBenefitOption lobjProvisionBenefitOption in lclbProvisionBenefitOption)
                        {
                            foreach (busBenefitCalculationOptions lobjBenefitOptions in iclbBenefitCalculationOptions)
                            {
                                if (lobjBenefitOptions.ibusBenefitProvisionBenefitOption == null)
                                    lobjBenefitOptions.LoadBenefitProvisionOption();
                                if (lobjBenefitOptions.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value ==
                                    lobjProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value)
                                {
                                    lclbFinalProvisionBenefitOption.Add(lobjProvisionBenefitOption);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (busBenefitProvisionBenefitOption lobjProvisionBenefitOption in lclbProvisionBenefitOption)
                    {
                        lclbFinalProvisionBenefitOption.Add(lobjProvisionBenefitOption);
                    }
                }


                decimal ldecRHICOptionFactor = 0M;
                foreach (busBenefitProvisionBenefitOption lobjProvisionBenefitOption in lclbFinalProvisionBenefitOption)
                {
                    if (icdoBenefitCalculation.plan_id != busConstant.PlanIdDC &&
                        icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2020 &&//PIR 20232
                        icdoBenefitCalculation.plan_id != busConstant.PlanIdDC2025) //PIR 25920
                    {
                        if ((string.IsNullOrEmpty(lobjProvisionBenefitOption.icdoBenefitProvisionBenefitOption.factor_method_value) ||
                           lobjProvisionBenefitOption.icdoBenefitProvisionBenefitOption.factor_method_value == busConstant.FactorMethodValueOther))
                            continue;
                    }
                    string lstrBenefitOption = lobjProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value;
                    //string lstrBenefitOption = 
                    //    busGlobalFunctions.GetData3ByCodeValue(1903, lobjProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value);
                    if ((astrCalculationType == busConstant.CalculationTypeEstimate || astrCalculationType == busConstant.CalculationTypeEstimateSubsequent) && //PIR 19594
                        (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService) &&
                        (!ablnIsJointAnnuitantExists) &&
                        ((lstrBenefitOption == busConstant.BenefitOption100PercentJS) ||
                         (lstrBenefitOption == busConstant.BenefitOption50PercentJS)))
                        continue;

                    // The Five year Term certain is used only for Conversion purpose only. So dont populate it in the Grid. (UAT PIR: 1093).
                    if (lstrBenefitOption == busConstant.BenefitOption5YearTermLife)
                    {
                        continue;
                    }

                    //This string fetches the Unique value for a Plan,BenefitOption and Benefit Type
                    string lstrTmpUniqueBenefitOption = string.Empty;
                    DataTable ldtbResult = Select<cdoCodeValue>(new string[4] { "code_id", "data1", "data2", "data3" },
                                                    new object[4]{ 1903, icdoBenefitCalculation.plan_id,icdoBenefitCalculation.benefit_account_type_value,
                                                lstrBenefitOption}, null, null);
                    if (ldtbResult.Rows.Count > 0)
                        lstrTmpUniqueBenefitOption = Convert.ToString(ldtbResult.Rows[0]["code_value"]);

                    Collection<busCodeValue> lclbRHICOptionDetails = new Collection<busCodeValue>();
                    lclbRHICOptionDetails = GetAvailableRHICOptionDetails(lstrTmpUniqueBenefitOption, astrRHICOption);
                    foreach (busCodeValue lobjRHICOption in lclbRHICOptionDetails)
                    {
                        if (lobjRHICOption.icdoCodeValue.data2 == busConstant.RHICOptionStandard)
                        { 
                            busBenefitRHICOption lobjBenefitStandardRHICOption = new busBenefitRHICOption();
                            lobjBenefitStandardRHICOption.icdoBenefitRhicOption = new cdoBenefitRhicOption();
                            lobjBenefitStandardRHICOption.icdoBenefitRhicOption.benefit_provision_benefit_option_id =
                                lobjProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_provision_benefit_option_id;
                            lobjBenefitStandardRHICOption.icdoBenefitRhicOption.option_factor = 1;
                            lobjBenefitStandardRHICOption.icdoBenefitRhicOption.rhic_option_value = lobjRHICOption.icdoCodeValue.data2.ToString();
                            lobjBenefitStandardRHICOption.icdoBenefitRhicOption.rhic_option_description =
                                lobjBase.iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1905, lobjBenefitStandardRHICOption.icdoBenefitRhicOption.rhic_option_value);
                            lobjBenefitStandardRHICOption.icdoBenefitRhicOption.member_rhic_amount =
                                                                Math.Round(adecRHICAfterEARLYReduction, 2, MidpointRounding.AwayFromZero);

                            // UAT PIR : 921 -- If the RHIC option is Standard & Spouse exists then the Member RHIC amount is the Spouse RHIC amount
                            // If 'Benefit Option' = Single Life, and 'RHIC Option' = Standard RHIC, then do not display any Spouse RHIC amount.
                            if (ablnIsJointAnnuitantExists)
                            {
                                if (lobjBenefitStandardRHICOption.ibusBenefitProvisionOption.IsNull())
                                    lobjBenefitStandardRHICOption.LoadBenefitProvisionOption();
                                if (lobjBenefitStandardRHICOption.ibusBenefitProvisionOption.icdoBenefitProvisionBenefitOption.benefit_option_value != busConstant.BenefitOptionSingleLife)
                                {
                                    lobjBenefitStandardRHICOption.icdoBenefitRhicOption.spouse_rhic_amount =
                                        Math.Round(lobjBenefitStandardRHICOption.icdoBenefitRhicOption.member_rhic_amount * 1, 2, MidpointRounding.AwayFromZero);
                                    lobjBenefitStandardRHICOption.icdoBenefitRhicOption.spouse_rhic_percentage = 1;
                                }
                                else
                                    lobjBenefitStandardRHICOption.icdoBenefitRhicOption.spouse_rhic_amount = 0M;
                            }

                            lclbBenefitRHICOption.Add(lobjBenefitStandardRHICOption);
                            // To handle when RecalculateBenefit is clicked from PA
                            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments ||
                                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequent ||   //PIR 18053
                                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)
                            {
                                idecMemberRHICAmount = lobjBenefitStandardRHICOption.icdoBenefitRhicOption.member_rhic_amount;
                            }
                            idecSpouseRHICAmount = lobjBenefitStandardRHICOption.icdoBenefitRhicOption.spouse_rhic_amount;
                        }
                        else
                        {
                            if (ablnIsJointAnnuitantExists)
                            {
                                busBenefitRHICOption lobjBenefitJointRHICOption = new busBenefitRHICOption();
                                lobjBenefitJointRHICOption.icdoBenefitRhicOption = new cdoBenefitRhicOption();
                                lobjBenefitJointRHICOption.icdoBenefitRhicOption.benefit_provision_benefit_option_id =
                                    lobjProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_provision_benefit_option_id;
                                decimal ldecSpouseOptionFactor = 1M;
                                string lstrTempOptionFactor = busGlobalFunctions.GetData3ByCodeValue(1903, lobjRHICOption.icdoCodeValue.data3, iobjPassInfo);
                                if (lobjRHICOption.icdoCodeValue.data2 == busConstant.RHICOptionReduced50)
                                {
                                    ldecSpouseOptionFactor = 0.5M;
                                }
                                if ((icdoBenefitCalculation.plan_id == busConstant.PlanIdJudges) ||
                                    (icdoBenefitCalculation.plan_id == busConstant.PlanIdHP) ||
                                    (icdoBenefitCalculation.plan_id == busConstant.PlanIdDC) ||
                                    (icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2020) || //PIR 20232
                                    (icdoBenefitCalculation.plan_id == busConstant.PlanIdDC2025)) //PIR 25920
                                {
                                    lintTempPlanID = busConstant.PlanIdMain;//PIR 20232 ?code
                                }
                                else
                                {
                                    lintTempPlanID = icdoBenefitCalculation.plan_id;
                                }
                                ldecRHICOptionFactor =
                                    GetOptionFactorsForPlan(adecMemberAge, adecBeneficiaryAge, astrBenefitAccountType, lintTempPlanID, lstrTempOptionFactor);
                                lobjBenefitJointRHICOption.icdoBenefitRhicOption.option_factor = ldecRHICOptionFactor;
                                lobjBenefitJointRHICOption.icdoBenefitRhicOption.rhic_option_value = lobjRHICOption.icdoCodeValue.data2.ToString();
                                lobjBenefitJointRHICOption.icdoBenefitRhicOption.rhic_option_description =
                                lobjBase.iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1905, lobjBenefitJointRHICOption.icdoBenefitRhicOption.rhic_option_value);
                                lobjBenefitJointRHICOption.icdoBenefitRhicOption.member_rhic_amount =
                                                Math.Round(adecRHICAfterEARLYReduction * ldecRHICOptionFactor, 2, MidpointRounding.AwayFromZero);
                                lobjBenefitJointRHICOption.icdoBenefitRhicOption.spouse_rhic_amount =
                                                Math.Round(ldecSpouseOptionFactor * lobjBenefitJointRHICOption.icdoBenefitRhicOption.member_rhic_amount, 2, MidpointRounding.AwayFromZero);
                                lobjBenefitJointRHICOption.icdoBenefitRhicOption.spouse_rhic_percentage = ldecSpouseOptionFactor;
                                lclbBenefitRHICOption.Add(lobjBenefitJointRHICOption);

                                // To handle when RecalculateBenefit is clicked from PA
                                if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                                    icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments ||
                                    icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequent ||   //PIR 18053
                                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)
                                {
                                    idecMemberRHICAmount = lobjBenefitJointRHICOption.icdoBenefitRhicOption.member_rhic_amount;
                                }
                                idecSpouseRHICAmount = lobjBenefitJointRHICOption.icdoBenefitRhicOption.spouse_rhic_amount;
                            }
                        }
                    }
                }
            }
            return lclbBenefitRHICOption;
        }

        #endregion

        public void CalculateMinimumGuaranteedMemberAccount()
        {
            decimal ldecMinimumGuarantee = 0M, ldecMemberAccountBalance = 0M;
            decimal ldecTaxableMinimumGuarantee = 0M, ldecNonTaxableMinimumGuarantee = 0M;
            decimal ldecTaxableAmount = 0M, ldecNonTaxableAmount = 0M;
            decimal ldecMemberAccountBalanceLTD = 0M;
            decimal ldecTaxableMemberAccountBalance = 0M, ldecNonTaxableMemberAccountBalance = 0M;
            DateTime ldteTerminationDate = new DateTime();

            if (ibusPersonAccount == null)
                LoadPersonAccount();

            if (ibusPersonAccount.ibusPersonAccountRetirement.IsNull())
                ibusPersonAccount.LoadPersonAccountRetirement();

            // SYSTEST - PIR - 1477 - For Pre-Retirement death if no termination date exists, use date of death for FAS Calculation.
            if ((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                //Commented as per PIR from david on UCS94 Systest PIR 2336
                //&& (icdoBenefitCalculation.termination_date == DateTime.MinValue)
                )
                ldteTerminationDate = icdoBenefitCalculation.date_of_death.AddMonths(1).GetLastDayofMonth();
            else
                ldteTerminationDate = icdoBenefitCalculation.termination_date.GetLastDayofMonth();// PROD PIR ID 4851: Purchase posted on the same month also needs to be included.


            //UAT PIR: 1131 For Estimates the MAB should be calculated as on Retirement date instead of Termination date.
            if (((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                || (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                && (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
            {
                ldteTerminationDate = icdoBenefitCalculation.retirement_date;
                if (icdoBenefitCalculation.is_dro_estimate == busConstant.Flag_Yes) // PROD PIR ID 1414
                    ldteTerminationDate = icdoBenefitCalculation.termination_date.GetLastDayofMonth(); 
            }
            //UAT PIR:2258. To Load the Capital Gain to avoid difference in Min Guarantee 
            ibusPersonAccount.ibusPersonAccountRetirement.LoadLTDSummaryForCalculation(ldteTerminationDate,
                            icdoBenefitCalculation.benefit_account_type_value, icdoBenefitCalculation.is_dro_estimate == busConstant.Flag_Yes,
                            icdoBenefitCalculation.benefit_option_value== busConstant.BenefitOptionRefund);

            if (icdoBenefitCalculation.is_return_to_work_member && (!icdoBenefitCalculation.is_rtw_member_subsequent_retirement &&
                icdoBenefitCalculation.calculation_type_value != busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
            {
                if (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_No_Value.ToUpper())
                {
                    ldecMemberAccountBalanceLTD += ibusPersonAccount.ibusPersonAccountRetirement.Member_Account_Balance_ltd;
                    ldecTaxableMemberAccountBalance = ibusPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd;
                    ldecNonTaxableMemberAccountBalance = ibusPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd;
                }
                if (iclbBenefitCalculationPersonAccount.IsNotNull())
                {
                    foreach (busBenefitCalculationPersonAccount lobjBenCalcPersonAccount in iclbBenefitCalculationPersonAccount)
                    {
                        if ((lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.is_person_account_selected_flag == busConstant.Flag_Yes) &&
                            (lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.person_account_id != ibusPersonAccount.icdoPersonAccount.person_account_id))
                        {
                            lobjBenCalcPersonAccount.LoadPayeeAccount();
                            if (lobjBenCalcPersonAccount.ibusPersonAccountRetirement.IsNull())
                                lobjBenCalcPersonAccount.ibusPersonAccountRetirement = new busPersonAccountRetirement();
                            lobjBenCalcPersonAccount.ibusPersonAccountRetirement.FindPersonAccountRetirement(
                                lobjBenCalcPersonAccount.icdoBenefitCalculationPersonAccount.person_account_id);
                            if (lobjBenCalcPersonAccount.ibusBenefitCalculation.IsNull())
                                lobjBenCalcPersonAccount.LoadBenefitCalculation();
                            lobjBenCalcPersonAccount.ibusPersonAccountRetirement.LoadLTDSummaryForCalculation(ldteTerminationDate,
                                                            lobjBenCalcPersonAccount.ibusBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value);
                            lobjBenCalcPersonAccount.ibusPayeeAccount.LoadPaymentDetails();
                            //UAT PIR: 1226,1575. If the old balance is negative, set it as 0.
                            ldecMemberAccountBalanceLTD += (lobjBenCalcPersonAccount.ibusPersonAccountRetirement.Member_Account_Balance_ltd -
                                    lobjBenCalcPersonAccount.ibusPayeeAccount.idecpaidgrossamount) > 0 ? (lobjBenCalcPersonAccount.ibusPersonAccountRetirement.Member_Account_Balance_ltd -
                                    lobjBenCalcPersonAccount.ibusPayeeAccount.idecpaidgrossamount) : 0;

                            //UAT PIR: 1226. Reduce the Taxable and Non Taxable only when the Member Account Balance LTD - Already Paid is greater than 0.
                            if (lobjBenCalcPersonAccount.ibusPersonAccountRetirement.Member_Account_Balance_ltd -
                                lobjBenCalcPersonAccount.ibusPayeeAccount.idecpaidgrossamount > 0)
                            {
                                ldecTaxableMemberAccountBalance += (lobjBenCalcPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd -
                                        lobjBenCalcPersonAccount.ibusPayeeAccount.idecpaidtaxableamount) > 0 ? (lobjBenCalcPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd -
                                        lobjBenCalcPersonAccount.ibusPayeeAccount.idecpaidtaxableamount) : 0;
                                ldecNonTaxableMemberAccountBalance += (lobjBenCalcPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd -
                                        lobjBenCalcPersonAccount.ibusPayeeAccount.idecpaidnontaxableamount) > 0 ? (lobjBenCalcPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd -
                                        lobjBenCalcPersonAccount.ibusPayeeAccount.idecpaidnontaxableamount) : 0;
                            }
                        }
                    }
                }
            }
            else
            {
                ldecMemberAccountBalanceLTD = ibusPersonAccount.ibusPersonAccountRetirement.Member_Account_Balance_ltd;
                ldecTaxableMemberAccountBalance = ibusPersonAccount.ibusPersonAccountRetirement.Pre_Tax_Employee_Contribution_ltd;
                ldecNonTaxableMemberAccountBalance = ibusPersonAccount.ibusPersonAccountRetirement.Post_Tax_Total_Contribution_ltd;
            }
            //PIR 9945 - QDRO Amount should be calculated for both 'Estimates' and 'Final' calculation type value

            //if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate)
            //{
            //    CalculateQDROAmount(false);
            //}
            //else
            //{
            //    CalculateQDROAmount(true);
            //}
            
            CalculateQDROAmount(true);
            decimal adecTotalPaidTaxableAmount = 0.0m;
            decimal adecTotalPaidNonTaxableAmount = 0.0m;

            if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionBeneficiaryBenefit)
                {
                    FetchAlreadyPaidAmountForSpouseandChild(ref adecTotalPaidTaxableAmount, ref adecTotalPaidNonTaxableAmount);

                    ldecMemberAccountBalanceLTD = ((ldecMemberAccountBalanceLTD - (adecTotalPaidTaxableAmount + adecTotalPaidNonTaxableAmount)) > 0) ? ldecMemberAccountBalanceLTD - (adecTotalPaidTaxableAmount + adecTotalPaidNonTaxableAmount) : 0;
                    ldecTaxableMemberAccountBalance = (ldecTaxableMemberAccountBalance - adecTotalPaidTaxableAmount > 0) ? ldecTaxableMemberAccountBalance - adecTotalPaidTaxableAmount : 0;
                    ldecNonTaxableMemberAccountBalance = (ldecNonTaxableMemberAccountBalance - adecTotalPaidNonTaxableAmount > 0) ? ldecNonTaxableMemberAccountBalance - adecTotalPaidNonTaxableAmount : 0;
                }
            }

            CalculateMinimumGuaranteedMemberAccount(icdoBenefitCalculation.calculation_type_value,
                                                    icdoBenefitCalculation.plan_id,
                                                    icdoBenefitCalculation.benefit_option_value,
                                                    icdoBenefitCalculation.person_id,
                                                    ldecMemberAccountBalanceLTD,
                                                    DateTime.Today, ref ldecMinimumGuarantee,
                                                    ref ldecMemberAccountBalance, icdoBenefitCalculation.final_monthly_benefit,
                                                    ref ldecTaxableMinimumGuarantee, ref ldecNonTaxableMinimumGuarantee,
                                                    ref ldecTaxableAmount, ref ldecNonTaxableAmount);
            // UAT PIR ID 1226 - Values cannot be Negative.
            icdoBenefitCalculation.minimum_guarentee_amount = (ldecMinimumGuarantee < 0M) ? 0M : ldecMinimumGuarantee;
            icdoBenefitCalculation.member_account_balance = (ldecMemberAccountBalance < 0M) ? 0M : ldecMemberAccountBalance;
            icdoBenefitCalculation.minimum_guarentee_amount_taxable_amount = (ldecTaxableMinimumGuarantee < 0M) ? 0M : ldecTaxableMinimumGuarantee;
            icdoBenefitCalculation.minimum_guarentee_amount_non_taxable_amount = (ldecNonTaxableMinimumGuarantee < 0M) ? 0M : ldecNonTaxableMinimumGuarantee;
            icdoBenefitCalculation.non_taxable_amount = (ldecNonTaxableMemberAccountBalance < 0M) ? 0M : ldecNonTaxableMemberAccountBalance;
            icdoBenefitCalculation.taxable_amount = (ldecTaxableMemberAccountBalance < 0M) ? 0M : ldecTaxableMemberAccountBalance;
        }


        public void FetchAlreadyPaidAmountForSpouseandChild(ref decimal adecTotalPaidTaxableAmount, ref decimal adecTotalPaidNonTaxableAmount)
        {
            if (ibusMember == null)
                LoadMember();

            if (ibusMember.icolPersonContact == null)
                ibusMember.LoadContacts();

            var icolTempPersonContact = ibusMember.icolPersonContact.Where(o => (o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeSpouse
                            || o.icdoPersonContact.relationship_value == busConstant.PersonContactTypeChild)
                            && (o.icdoPersonContact.contact_person_id > 0));

            adecTotalPaidTaxableAmount = 150;
            adecTotalPaidNonTaxableAmount = 100;
            foreach (busPersonContact lobjPersonContact in icolTempPersonContact)
            {

                lobjPersonContact.LoadContactPerson();

                lobjPersonContact.ibusContactPerson.LoadBenefitApplication();

                if (lobjPersonContact.ibusContactPerson.iclbBeneficiaryApplication.IsNotNull())
                {
                    var lobjTempApplication = lobjPersonContact.ibusContactPerson.iclbBeneficiaryApplication.Where(o => (o.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                        && (o.icdoBenefitApplication.member_person_id == icdoBenefitCalculation.person_id)
                        && (o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusCancelled)
                        && (o.icdoBenefitApplication.action_status_value != busConstant.ApplicationActionStatusDenied)).FirstOrDefault();


                    if (lobjTempApplication.IsNotNull())
                    {
                        lobjTempApplication.LoadPayeeAccount();

                        if (lobjTempApplication.iclbPayeeAccount.Count != 0)
                        {
                            foreach (busPayeeAccount lobjTemppayeeAccount in lobjTempApplication.iclbPayeeAccount)
                            {
                                lobjTemppayeeAccount.LoadActivePayeeStatus();
                                if ((lobjTemppayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted()))
                                {
                                    lobjTemppayeeAccount.LoadPaymentDetails();
                                    adecTotalPaidTaxableAmount += lobjTemppayeeAccount.idecpaidtaxableamount;
                                    adecTotalPaidNonTaxableAmount += lobjTemppayeeAccount.idecpaidnontaxableamount;
                                }
                            }
                        }
                    }
                }
            }
        }


        public void CalculateMinimumGuaranteedMemberAccount(string astrCalculationType, int aintPlanID, string astrOptionName, int aintPersonID,
                                        decimal adecMemberAccountBalanceLTD, DateTime adtCalculationDate, ref decimal adecMinimumGuarantee,
                                        ref decimal adecMemberAccountBalance, decimal adecMonthlyBenefitAmount, ref decimal adecTaxableMinimumGuarantee,
                                        ref decimal adecNonTaxableMinimumGuarantee, ref decimal adecTaxableAmount, ref decimal adecNonTaxableAmount)
        {
            int lintNumberOfMonths = 1;

            adecMinimumGuarantee = adecMemberAccountBalanceLTD;
            adecMemberAccountBalance = adecMinimumGuarantee;

            /*********************************/
            //Todo: Logic for Finding the NonTaxable part of Member Account to be revisited after Exclusion Ratio rules.

            adecNonTaxableMinimumGuarantee = 0;
            adecNonTaxableAmount = 0;
            adecTaxableAmount = 0;
            /********************************/
            adecTaxableMinimumGuarantee = adecMemberAccountBalance - adecNonTaxableMinimumGuarantee;

            if (aintPlanID != busConstant.PlanIdJobService)
            {
                switch (astrOptionName)
                {
                    case busConstant.BenefitOption5YearTermLife:
                        lintNumberOfMonths = 5 * 12;
                        adecMinimumGuarantee = adecMonthlyBenefitAmount;
                        break;
                    case busConstant.BenefitOption10YearCertain:
                        lintNumberOfMonths = 10 * 12;
                        adecMinimumGuarantee = adecMonthlyBenefitAmount;
                        break;
                    case busConstant.BenefitOption15YearCertain:
                        lintNumberOfMonths = 15 * 12;
                        adecMinimumGuarantee = adecMonthlyBenefitAmount;
                        break;
                    case busConstant.BenefitOption20YearCertain:
                        lintNumberOfMonths = 20 * 12;
                        adecMinimumGuarantee = adecMonthlyBenefitAmount;
                        break;
                }
            }
            else
            {
                // PROD PIR ID 6481
                switch (astrOptionName)
                {
                    case busConstant.BenefitOption5YearTermLife:
                        lintNumberOfMonths = 5 * 12;
                        break;
                    case busConstant.BenefitOption10YearCertain:
                        lintNumberOfMonths = 10 * 12;
                        break;
                    case busConstant.BenefitOption15YearCertain:
                        lintNumberOfMonths = 15 * 12;
                        break;
                    case busConstant.BenefitOption20YearCertain:
                        lintNumberOfMonths = 20 * 12;
                        break;
                }
            }

            // UCS-060 Return to Work
            decimal ldecRTWAdjustmentAmount = 0.0M;
            if (icdoBenefitCalculation.is_return_to_work_member)
            {
                //There should a value in PreRTWPayeeAccount
                if (ibuspre_RTW_Payee_Account == null)
                    LoadPreRTWPayeeAccount();

                ibuspre_RTW_Payee_Account.LoadPaymentDetails();
                ldecRTWAdjustmentAmount = ibuspre_RTW_Payee_Account.idecpaidgrossamount;

                if (!IsJobService)
                {
                    if ((icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption5YearTermLife) ||
                        (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption10YearCertain) ||
                        (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption15YearCertain) ||
                        (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption20YearCertain))
                    {
                        lintNumberOfMonths -= ibuspre_RTW_Payee_Account.GetAlreadyPaidNumberofPayments();
                    }
                }
                //adecMinimumGuarantee = adecMinimumGuarantee * iintTermCertainMonths;               
            }
            iintTermCertainMonths = lintNumberOfMonths;
            // PIR 9166 - Minimum guarantee should be same as Member Account Balance for Job Service plan.
            if (aintPlanID != busConstant.PlanIdJobService)
                adecMinimumGuarantee = adecMinimumGuarantee * lintNumberOfMonths;
            if (lintNumberOfMonths == 1)
                adecMinimumGuarantee = adecMinimumGuarantee - (icdoBenefitCalculation.non_taxable_qdro_amount + icdoBenefitCalculation.taxable_qdro_amount);

            //UAT PIR:1179
            //If for term certain options, the minimum Guarantee Should be the higher of the Calculated Minimum Guarantee and Member Account Balance
            if (aintPlanID != busConstant.PlanIdJobService)
            {
                if ((astrOptionName == busConstant.BenefitOption5YearTermLife) ||
                    (astrOptionName == busConstant.BenefitOption10YearCertain) ||
                    (astrOptionName == busConstant.BenefitOption15YearCertain) ||
                    (astrOptionName == busConstant.BenefitOption20YearCertain))
                {
                    if (adecMinimumGuarantee < adecMemberAccountBalanceLTD)
                    {
                        adecMinimumGuarantee = adecMemberAccountBalanceLTD;
                    }
                }
            }
        }

        public void CreateBenefitCalculationPayeeDetails()
        {
            foreach (busBenefitCalculationPayee lobjBenefitCalculationPayee in iclbBenefitCalculationPayee)
            {
                lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                // PROD PIR 8042
                if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                {
                    lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.member_account_negated_flag = busConstant.Flag_No;
                    if (ibusOriginatingPayeeAccount.IsNull()) LoadOriginatingPayeeAccount();
                    if (ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeDisability)
                        lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.member_account_negated_flag = busConstant.Flag_Yes;
                }
                lobjBenefitCalculationPayee.icdoBenefitCalculationPayee.Insert();
            }
        }

        public void DeleteBenefitCalculationPayeeDetails()
        {
            Collection<busBenefitCalculationPayee> lclbBenefitCalculationPayee = new Collection<busBenefitCalculationPayee>();
            lclbBenefitCalculationPayee = LoadBenefitCalculationPayeeFromDB();
            foreach (busBenefitCalculationPayee lobjPayee in lclbBenefitCalculationPayee)
            {
                lobjPayee.icdoBenefitCalculationPayee.Delete();
            }
        }

        public void DeleteBenefitCalculationDetails()
        {
            LoadFASMonths();
            LoadFASMonths2019();
            LoadFASMonths2020();
            LoadBenefitMultiplier();
            LoadBenefitCalculationOptions();
            LoadBenefitRHICOptionFromDB();
            LoadBenefitFASIndexingMonths();

            if ((iclbBenefitCalculationFASMonths != null) &&
                (iclbBenefitCalculationFASMonths.Count > 0))
            {
                foreach (busBenefitCalculationFasMonths lobjFASMonths in iclbBenefitCalculationFASMonths)
                {
                    lobjFASMonths.icdoBenefitCalculationFasMonths.Delete();
                }
                iclbBenefitCalculationFASMonths = new Collection<busBenefitCalculationFasMonths>();
            }
            if ((iclbBenefitCalculationFASMonths2019 != null) &&
               (iclbBenefitCalculationFASMonths2019.Count > 0))
            {
                foreach (busBenefitCalculationFasMonths lobjFASMonths in iclbBenefitCalculationFASMonths2019)
                {
                    lobjFASMonths.icdoBenefitCalculationFasMonths.Delete();
                }
                iclbBenefitCalculationFASMonths2019 = new Collection<busBenefitCalculationFasMonths>();
            }
            if ((iclbBenefitCalculationFASMonths2020 != null) &&
                (iclbBenefitCalculationFASMonths2020.Count > 0))
            {
                foreach (busBenefitCalculationFasMonths lobjFASMonths in iclbBenefitCalculationFASMonths2020)
                {
                    lobjFASMonths.icdoBenefitCalculationFasMonths.Delete();
                }
                iclbBenefitCalculationFASMonths2020 = new Collection<busBenefitCalculationFasMonths>();
            }

            if ((iclbBenefitMultiplier != null) &&
                (iclbBenefitMultiplier.Count > 0))
            {
                foreach (busBenefitMultiplier lobjBenefitMultiplier in iclbBenefitMultiplier)
                {
                    lobjBenefitMultiplier.icdoBenefitMultiplier.Delete();
                }
                iclbBenefitMultiplier = new Collection<busBenefitMultiplier>();
            }

            if ((iclbBenefitCalculationOptions != null) &&
                (iclbBenefitCalculationOptions.Count > 0))
            {
                foreach (busBenefitCalculationOptions lobjBenefitCalculationOptions in iclbBenefitCalculationOptions)
                {
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.Delete();
                }
                iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
            }

            if ((iclbBenefitRHICOption != null) &&
                (iclbBenefitRHICOption.Count > 0))
            {
                foreach (busBenefitRHICOption lobjBenefitRHICOption in iclbBenefitRHICOption)
                {
                    lobjBenefitRHICOption.icdoBenefitRhicOption.Delete();
                }
                iclbBenefitRHICOption = new Collection<busBenefitRHICOption>();
            }


            if ((icdoBenefitCalculation.benefit_account_type_value != busConstant.ApplicationBenefitTypePostRetirementDeath) &&
                (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
            {
                LoadBenefitServicePurchase();
                if ((iclbBenefitServicePurchase != null) &&
                    (iclbBenefitServicePurchase.Count > 0))
                {
                    foreach (busBenefitServicePurchase lobjBenefitServicePurchase in iclbBenefitServicePurchase)
                    {
                        lobjBenefitServicePurchase.icdoBenefitServicePurchase.Delete();
                    }
                    iclbBenefitServicePurchase = new Collection<busBenefitServicePurchase>();
                }
            }

            if ((iclbBenefitFASIndexing != null) &&
           (iclbBenefitFASIndexing.Count > 0))
            {
                foreach (busBenefitFasIndexing lobjBenefitFasIndexing in iclbBenefitFASIndexing)
                {
                    lobjBenefitFasIndexing.icdoBenefitFasIndexing.Delete();
                }
                iclbBenefitFASIndexing = new Collection<busBenefitFasIndexing>();
            }

        }


        public void CreateBenefitCalculationDetails()
        {
            // Insert FAS Records
            if (iclbBenefitCalculationFASMonths != null)
            {
                foreach (busBenefitCalculationFasMonths lobjFASMonth in iclbBenefitCalculationFASMonths)
                {
                    lobjFASMonth.icdoBenefitCalculationFasMonths.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjFASMonth.icdoBenefitCalculationFasMonths.Insert();
                }
            }

            // Insert FAS Records Till Termination Date 12/31/2019
            if (iclbBenefitCalculationFASMonths2019 != null)
            {
                foreach (busBenefitCalculationFasMonths lobjFASMonthTermDate in iclbBenefitCalculationFASMonths2019)
                {
                    lobjFASMonthTermDate.icdoBenefitCalculationFasMonths.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjFASMonthTermDate.icdoBenefitCalculationFasMonths.Insert();
                }
            }

            // Insert FAS Records after Termination Date 12/31/2019
            if (iclbBenefitCalculationFASMonths2020 != null)
            {
                foreach (busBenefitCalculationFasMonths lobjFASMonthTermDate in iclbBenefitCalculationFASMonths2020)
                {
                    lobjFASMonthTermDate.icdoBenefitCalculationFasMonths.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjFASMonthTermDate.icdoBenefitCalculationFasMonths.Insert();
                }
            }

            // Insert FAS Indexing Records
            if (iclbBenefitFASIndexing != null)
            {
                foreach (busBenefitFasIndexing lobjenefitFasIndexing in iclbBenefitFASIndexing)
                {
                    lobjenefitFasIndexing.icdoBenefitFasIndexing.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjenefitFasIndexing.icdoBenefitFasIndexing.Insert();
                }
            }

            // Insert Benefit Mulitplier
            if (iclbBenefitMultiplier != null)
            {
                foreach (busBenefitMultiplier lobjBenefitMultiplier in iclbBenefitMultiplier)
                {
                    lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjBenefitMultiplier.icdoBenefitMultiplier.Insert();
                }
            }
            // Insert Benefit Options
            if (iclbBenefitCalculationOptions != null)
            {
                foreach (busBenefitCalculationOptions lobjBenefitCalculationOptions in iclbBenefitCalculationOptions)
                {
                    //PIR: 2118. Benefit Payee is not deleted for retirement or disability
                    if ((icdoBenefitCalculation.benefit_account_type_value != busConstant.ApplicationBenefitTypeRetirement) &&
                        (icdoBenefitCalculation.benefit_account_type_value != busConstant.ApplicationBenefitTypeDisability))
                    {
                        lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_payee_id = 0;
                    }
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjBenefitCalculationOptions.icdoBenefitCalculationOptions.Insert();
                }
            }
            // Insert RHIC Options
            if (iclbBenefitRHICOption != null)
            {
                foreach (busBenefitRHICOption lobjBenefitRHICOption in iclbBenefitRHICOption)
                {
                    lobjBenefitRHICOption.icdoBenefitRhicOption.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                    lobjBenefitRHICOption.icdoBenefitRhicOption.Insert();
                }
            }
            // Insert Benefit Service Purchase
            if ((icdoBenefitCalculation.benefit_account_type_value != busConstant.ApplicationBenefitTypePostRetirementDeath) &&
                (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent))  //PIR 19594
            {
                if (iclbBenefitServicePurchaseAll != null)
                {
                    foreach (busBenefitServicePurchase lobjServicePurchase in iclbBenefitServicePurchaseAll)
                    {
                        if (lobjServicePurchase.istrIncludeRemainder == busConstant.Flag_Yes)
                        {
                            lobjServicePurchase.icdoBenefitServicePurchase.benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id;
                            lobjServicePurchase.icdoBenefitServicePurchase.Insert();
                        }
                    }
                }
                LoadBenefitServicePurchase();
            }
            icdoBenefitCalculation.Update();

            LoadFASMonths();
            LoadFASMonths2019();
            LoadFASMonths2020();
            LoadBenefitMultiplier();
            LoadBenefitRHICOption();
            LoadBenefitCalculationOptions();
            LoadBenefitFASIndexingMonths();
        }

        #region Tax Components Calculation

        /* This Method Gets parameters required for Finding PLSO Taxable And Non Taxable Components.         
         */
        public void CalculatePLSOTaxComponents(decimal adecPLSOLumpSumAmount, decimal adecEmpPostTaxContributionAmount, decimal adecMonthlyAmountAfterDeduction,
                                        decimal adecPLSOFactor, decimal adecQDROAmount, ref decimal ldecNonTaxablePLSOAmount, ref decimal ldecTaxablePLSOAmount,
                                        ref decimal adecPLSOExclusionRatio)
        {
            decimal ldecActuarialValue = 0.0M;

            //Initializing the Reference Parameters
            ldecNonTaxablePLSOAmount = 0.0M;
            ldecTaxablePLSOAmount = 0.0M;

            if (adecPLSOFactor != 0)
            {
                ldecActuarialValue = (adecMonthlyAmountAfterDeduction * 12) / adecPLSOFactor;
            }

            if (ldecActuarialValue != 0)
            {
                adecPLSOExclusionRatio = (adecEmpPostTaxContributionAmount - adecQDROAmount) / ldecActuarialValue;
            }

            //Taxable and Non Taxable PLSO Calculation
            //Non Taxable PLSO = PLSO Lump Sum Amount * PLSO Exclusion Ratio
            //Taxable PLSO = PLSO Lump Sum Amount - Non Taxable PLSO 
            ldecNonTaxablePLSOAmount = adecPLSOLumpSumAmount * adecPLSOExclusionRatio;
            ldecTaxablePLSOAmount = adecPLSOLumpSumAmount - ldecNonTaxablePLSOAmount;

        }

        public void CalculateMonthlyTaxComponents(DateTime adtCalculationDate, decimal adecMemberAge, decimal adecBeneficiaryAge,
            string astrExclusionCalculationPaymentValue, decimal adecEmpPostTaxContributionAmount, decimal adecMonthlyAmountAfterDeduction,
            decimal ldecNonTaxablePLSOAmount, decimal adecQDROAmount, ref decimal ldecNonTaxableAmount, ref decimal ldecTaxableAmount,
            ref decimal adecExclusionRatioAmount, decimal adecBenefitPercentage)
        {

            //Initializing the Reference Parameters
            ldecNonTaxableAmount = 0.0M;
            ldecTaxableAmount = 0.0M;

            int lintSumofAge = 0;
            decimal ldecNoOfPayments = 0.0M;

            //1. Find Sum of Member Age and Beneficiary Age to find the number of Payments from Life Annuity Table           
            if (astrExclusionCalculationPaymentValue == busConstant.ExclusionCalcPaymentTypeJointLife)
            {
                lintSumofAge = Convert.ToInt32((Math.Round(adecMemberAge + adecBeneficiaryAge, 0, MidpointRounding.AwayFromZero)));
            }
            else
            {
                lintSumofAge = Convert.ToInt32((Math.Round(adecMemberAge, 0, MidpointRounding.AwayFromZero)));
            }
            ldecNoOfPayments = GetNumberofPayments(astrExclusionCalculationPaymentValue, lintSumofAge, adtCalculationDate);

            //2. Find the Exclusion Ratio Amount or the Non Taxable Monthly Amount
            // ExclusionAmount =( Employee Post Tax Contribution - PLSO NonTaxable Amount - QDRO Amount )/ Number of Payments

            if (ldecNoOfPayments != 0)
            {
                adecExclusionRatioAmount = (adecEmpPostTaxContributionAmount - ldecNonTaxablePLSOAmount - adecQDROAmount) / ldecNoOfPayments;
                if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                    adecExclusionRatioAmount = Math.Round((adecExclusionRatioAmount * adecBenefitPercentage) / 100M, 2, MidpointRounding.AwayFromZero);
            }

            ldecNonTaxableAmount = Math.Round(adecExclusionRatioAmount, 2, MidpointRounding.AwayFromZero);
            //PIR: 1888 Do not reduce the Nont Taxable Amount if it is not greater than 0.
            if (ldecNonTaxableAmount > 0)
            {
                ldecTaxableAmount = Math.Round(adecMonthlyAmountAfterDeduction - ldecNonTaxableAmount, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                ldecTaxableAmount = Math.Round(adecMonthlyAmountAfterDeduction, 2, MidpointRounding.AwayFromZero);
            }
        }

        public DataTable idtbBenefitProvisionExclusion { get; set; }
        public int GetNumberofPayments(string astrExclusionCalculationPaymentValue, int aintSumofAge, DateTime adecEffectivedate)
        {
            if (idtbBenefitProvisionExclusion == null)
            {
                idtbBenefitProvisionExclusion = Select<cdoBenefitProvisionExclusion>(
                               new string[0] { },
                               new object[0] { }, null, null);
            }
            DataTable ldtbList = idtbBenefitProvisionExclusion.AsEnumerable().Where(i => aintSumofAge >= i.Field<int>("MINIMUM_AGE") &&
                                                                  aintSumofAge <= i.Field<int>("MAXIMUM_AGE") &&
                                                                  i.Field<string>("EXCLUSION_CALC_PAYMENT_TYPE_VALUE") == astrExclusionCalculationPaymentValue &&
                                                                  i.Field<DateTime>("EFFECTIVE_DATE") <= adecEffectivedate)
                                                       .OrderByDescending(i => i.Field<DateTime>("EFFECTIVE_DATE")).AsDataTable();
            int lintNoOfPayments = 0;
            if (ldtbList.Rows.Count > 0)
            {
                lintNoOfPayments = Convert.ToInt32(ldtbList.Rows[0]["NUMBER_OF_PAYMENTS"].ToString());
            }
            return lintNoOfPayments;
        }

        #endregion

        #region Methods for Correspondence

        public override busBase GetCorPerson()
        {
            if (ibusMember == null)
                LoadMember();
            return ibusMember;
        }

        public void LoadBenefitProvision()
        {
            if (_icdoBenefitProvision == null)
                _icdoBenefitProvision = new cdoBenefitProvision();
            DataTable ldtbList = Select<cdoBenefitProvision>(
                           new string[1] { "benefit_provision_id" },
                           new object[1] { ibusPlan.icdoPlan.benefit_provision_id }, null, null);
            if (ldtbList.Rows.Count > 0)
                _icdoBenefitProvision.LoadData(ldtbList.Rows[0]);
        }

        public void LoadBenefitProvisionMultiplier()
        {
            if (_iclbBenefitProvisionMultiplier == null)
                _iclbBenefitProvisionMultiplier = new Collection<cdoBenefitProvisionMultiplier>();
            DataTable ldtbList = Select<cdoBenefitProvisionMultiplier>(
                           new string[1] { "benefit_provision_id" },
                           new object[1] { ibusPlan.icdoPlan.benefit_provision_id }, null, null);
            _iclbBenefitProvisionMultiplier = cdoBenefitProvisionMultiplier.GetCollection<cdoBenefitProvisionMultiplier>(ldtbList);
        }

        //loop thru this collection and assign value
        //loop thru this collection and assign value
        public void LoadBenefitAmount()
        {
            if (iclbBenefitCalculationOptions == null)
                LoadBenefitCalculationOptions();

            foreach (busBenefitCalculationOptions lobjBenefitMultiplier in iclbBenefitCalculationOptions)
            {
                if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption == null)
                    lobjBenefitMultiplier.LoadBenefitProvisionOption();
                if (lobjBenefitMultiplier.ibusBenefitCalculationPayee == null)
                    lobjBenefitMultiplier.LoadBenefitCalculationPayee();

                if (lobjBenefitMultiplier.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value == busConstant.AccountRelationshipMember)
                {
                    if ((lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOptionSingleLife)
                        || (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOptionStraightLife))
                    {  //*********************** this is before SSLI
                        icdoBenefitCalculation.idecSingleLifeAmount = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                        icdoBenefitCalculation.idecSingleLifeAmountBeforeSSLI = lobjBenefitMultiplier.icdoBenefitCalculationOptions.before_ssli_amount;
                        //*********************This is after SSLI
                        icdoBenefitCalculation.idecSingleLifeAmountAfterSSLI = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idecStraightLifeBenefitOptionAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                        icdoBenefitCalculation.idecSingleLifeWithPLSOAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_with_plso;
                    }
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption5YearTermLife)
                    {
                        icdoBenefitCalculation.idec5YrCertainAmount = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                        icdoBenefitCalculation.idec5YrCertainAmountAfterSSLI = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                    }
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption50PercentJS)
                    {
                        icdoBenefitCalculation.idec50PercentJSAmount = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                        icdoBenefitCalculation.idec50PercentJSAmountAfterSSLI = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idec50PercentJSWithPLSOAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_with_plso;
                    }
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption20YearCertain)
                    {
                        icdoBenefitCalculation.idec20YrCertainAmount = lobjBenefitMultiplier.icdoBenefitCalculationOptions.before_ssli_amount;
                        icdoBenefitCalculation.idec20YrCertainAmountAfterSSLI = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idec20YrCertainBenefitOptionAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                        icdoBenefitCalculation.idec20YrCertainWithPLSOAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_with_plso;
                    }
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption15YearCertain)
                    {
                        icdoBenefitCalculation.idec15YrCertainAmount = lobjBenefitMultiplier.icdoBenefitCalculationOptions.before_ssli_amount;
                        icdoBenefitCalculation.idec15YrCertainAmountAfterSSLI = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idec15YrCertainBenefitOptionAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                    }
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption10YearCertain)
                    {
                        icdoBenefitCalculation.idec10YrCertainAmount = lobjBenefitMultiplier.icdoBenefitCalculationOptions.before_ssli_amount;
                        icdoBenefitCalculation.idec10YrCertainAmountAfterSSLI = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idec10YrCertainBenefitOptionAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                        icdoBenefitCalculation.idec10YrCertainWithPLSOAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_with_plso;
                    }
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption100PercentJS)
                    {
                        icdoBenefitCalculation.idec100PercentJSAmount = lobjBenefitMultiplier.icdoBenefitCalculationOptions.before_ssli_amount;
                        icdoBenefitCalculation.idec100PercentJSAmountAfterSSLI = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idec100PercentJSBenefitOptionAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                        icdoBenefitCalculation.idec100PercentJSWithPLSOAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_with_plso;
                    }
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption55Percent)
                    {
                        icdoBenefitCalculation.idec55PercentJSAmount = lobjBenefitMultiplier.icdoBenefitCalculationOptions.before_ssli_amount;
                        icdoBenefitCalculation.idec55PercentJSAmountAfterSSLI = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idec55PercentJSBenefitOptionAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                    }
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption75Percent)
                    {
                        icdoBenefitCalculation.idec75PercentJSAmount = lobjBenefitMultiplier.icdoBenefitCalculationOptions.before_ssli_amount;
                        icdoBenefitCalculation.idec75PercentJSAmountAfterSSLI = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idec75PercentJSBenefitOptionAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                    }
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit)
                    {
                        icdoBenefitCalculation.idecNormalRetBenefitAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                        icdoBenefitCalculation.idecNormalRetBenefitAmtAfterSSLI = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idecNormalBenefitWithPLSOAmt = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_with_plso;
                    }
                }
                  
                else if (lobjBenefitMultiplier.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.family_relationship_value == busConstant.FamilyRelationshipSpouse)
                {
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption55Percent)
                    {
                        icdoBenefitCalculation.idec55PercentJSAmountForSpouse = lobjBenefitMultiplier.icdoBenefitCalculationOptions.before_ssli_amount;
                        icdoBenefitCalculation.idec55PercentJSAmountAfterSSLIForSpouse = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idec55PercentJSBenefitOptionAmtForSpouse = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                    }
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption75Percent)
                    {
                        icdoBenefitCalculation.idec75PercentJSAmountForSpouse = lobjBenefitMultiplier.icdoBenefitCalculationOptions.before_ssli_amount;
                        icdoBenefitCalculation.idec75PercentJSAmountAfterSSLIForSpouse = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idec75PercentJSBenefitOptionAmtForSpouse = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                    }
                    if (lobjBenefitMultiplier.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption100PercentJS)
                    {
                        icdoBenefitCalculation.idec100PercentJSAmountForSpouse = lobjBenefitMultiplier.icdoBenefitCalculationOptions.before_ssli_amount;
                        icdoBenefitCalculation.idec100PercentJSAmountAfterSSLIForSpouse = lobjBenefitMultiplier.icdoBenefitCalculationOptions.after_ssli_amount;
                        icdoBenefitCalculation.idec100PercentJSBenefitOptionAmtForSpouse = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_option_amount;
                       // icdoBenefitCalculation.idec100PercentJSWithPLSOAmtForSpouse = lobjBenefitMultiplier.icdoBenefitCalculationOptions.benefit_with_plso;
                    }
                }
            }
        }

        //load RHIC amount 
        public void LoadRHICAmountForMember()
        {
            if (iclbBenefitRHICOption == null)
                LoadBenefitRHICOption();
            foreach (busBenefitRHICOption lobjRHICOption in iclbBenefitRHICOption)
            {
                if (lobjRHICOption.icdoBenefitRhicOption.rhic_option_value == busConstant.RHICOptionStandard)
                    icdoBenefitCalculation.idecRHICOptionStandardAmt = lobjRHICOption.icdoBenefitRhicOption.member_rhic_amount;
                if (lobjRHICOption.icdoBenefitRhicOption.rhic_option_value == busConstant.RHICOptionReduced50)
                    icdoBenefitCalculation.idecRHICOption50PercentAmt = lobjRHICOption.icdoBenefitRhicOption.member_rhic_amount;
                if (lobjRHICOption.icdoBenefitRhicOption.rhic_option_value == busConstant.RHICOptionReduced100)
                    icdoBenefitCalculation.idecRHICOption100PercentAmt = lobjRHICOption.icdoBenefitRhicOption.member_rhic_amount;
            }
        }

        #region UCS -056 Correspondence Functions

        public override void LoadCorresProperties(string astrTemplateName)
        {
            if (ibusPlan.IsNull())
                LoadPlan();
            if (ibusMember.IsNull())
                LoadMember();
            //ucs-055 cor related methods
            LoadBenefitProvision();
            LoadRHICAmountForMember();
            LoadBenefitProvisionMultiplier();
            LoadBenefitAmount();

            //ucs-056 cor related methods
            LoadPersonAccountForLife();
            LoadPersonAccountLifeOptions();

            //UCS- 080 related methods
            //***** cor - ucs 80
            if (ibusBenefitApplication == null)
            {
                ibusBenefitApplication = new busRetirementDisabilityApplication();
                ibusBenefitApplication.FindBenefitApplication(icdoBenefitCalculation.benefit_application_id);
            }
            GetNormalEligibilityDetails();
            //**************
        }

        public void LoadPersonAccountForLife()
        {
            if (ibusPersonAccountForLife == null)
                ibusPersonAccountForLife = new busPersonAccount();
            ibusPersonAccountForLife = ibusMember.LoadActivePersonAccountByPlan(busConstant.PlanIdGroupLife);
        }

        public void LoadPersonAccountLifeOptions()
        {
            if (iclbPersonAccountLifeOption == null)
                iclbPersonAccountLifeOption = new Collection<busPersonAccountLifeOption>();
            if (ibusPersonAccountForLife.icdoPersonAccount.person_account_id != 0)
            {
                DataTable ldtbList = Select<cdoPersonAccountLifeOption>(
                    new string[1] { "person_account_id" },
                    new object[1] { ibusPersonAccountForLife.icdoPersonAccount.person_account_id }, null, null);
                iclbPersonAccountLifeOption = GetCollection<busPersonAccountLifeOption>(ldtbList, "icdoPersonAccountLifeOption");
            }
        }

        #endregion

        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            if (ahstParam.Count == 2)
            {
                int lintBenefitOptionID = 0;
                if (ahstParam["aintBenefitOptionID"].ToString() != "")
                {
                    lintBenefitOptionID = Convert.ToInt32(ahstParam["aintBenefitOptionID"]);
                }
                busBenefitCalculationOptions lobjBenefitOptions = new busBenefitCalculationOptions();
                lobjBenefitOptions.FindBenefitCalculationOptions(lintBenefitOptionID);
                lobjBenefitOptions.LoadBenefitCalculationPayee();
                if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                {
                    if (lobjBenefitOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value !=
                        busConstant.AccountRelationshipMember)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5106, "");
                        larrErrors.Add(lobjError);
                    }
                }
                else if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                {
                    if (lobjBenefitOptions.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value !=
                        busConstant.AccountRelationshipSpouse)
                    {
                        utlError lobjError = null;
                        lobjError = AddError(5107, "");
                        larrErrors.Add(lobjError);
                    }
                }
            }
            return larrErrors;
        }

        #endregion

        public Collection<busBenefitDroApplication> iclbBenefitDroApplicationByPersonPlan { get; set; }
        public void LoadBenefitDroApplicationByPersonPlan()
        {
            DataTable ldtbList = Select<cdoBenefitDroApplication>(
                                    new string[2] { "member_perslink_id", "plan_id" },
                                    new object[2] { icdoBenefitCalculation.person_id, icdoBenefitCalculation.plan_id }, null, null);
            iclbBenefitDroApplicationByPersonPlan = GetCollection<busBenefitDroApplication>(ldtbList, "icdoBenefitDroApplication");
        }
        public DateTime idtLastIntertestPostDate { get; set; }
        public void CalculateQDROAmount(bool bsetQDROAmount)
        {
            decimal ldecTotalQDROAmount = 0.0M;
            decimal ldecTotalNonTaxableQDROAmount = 0.0M;
            decimal ldecTotalTaxableQDROAmount = 0.0M;

            bool bAllowQDROAmount = false;

            if (iclbBenefitDroApplicationByPersonPlan == null)
            {
                LoadBenefitDroApplicationByPersonPlan();
            }

            foreach (busBenefitDroApplication lobjbenefitdroApplication in iclbBenefitDroApplicationByPersonPlan)
            {
                if (lobjbenefitdroApplication.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified || lobjbenefitdroApplication.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusApproved)
                {
                    ldecTotalQDROAmount = ldecTotalQDROAmount + lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_amount;
                    if (lobjbenefitdroApplication.ibusBenefitDroCalculation == null)
                        lobjbenefitdroApplication.LoadDROCalculation();

                    bAllowQDROAmount = true;
                    if (lobjbenefitdroApplication.ibusBenefitDroCalculation != null)
                    {
                        if (lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.payment_status_value == busConstant.DROApplicationPaymentStatusProcessed)
                        {
                            bAllowQDROAmount = false;
                        }
                        else
                        {
                            //Calculate Additional Interest Amount 
                            if (idtLastIntertestPostDate == DateTime.MinValue)
                                idtLastIntertestPostDate = busInterestCalculationHelper.GetInterestBatchLastRunDate();
                            lobjbenefitdroApplication.CalculateAdditionalInterest(idtLastIntertestPostDate);
                            idecquadro_calculation_additional_interest_amount = lobjbenefitdroApplication.ibusBenefitDroCalculation.icdoBenefitDroCalculation.additional_interest;
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecquadro_calculation_additional_interest_amount;
                        }
                    }

                    if (bAllowQDROAmount)
                    {
                        //Calculating Non Taxable Components of QDRO
                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_post_tax_amount > 0.0M)
                        {
                            idecquadro_ee_post_tax_amount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_post_tax_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalNonTaxableQDROAmount = ldecTotalNonTaxableQDROAmount + idecquadro_ee_post_tax_amount;

                        }
                        else
                        {
                            idecquadro_ee_post_tax_amount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_ee_post_tax_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalNonTaxableQDROAmount = ldecTotalNonTaxableQDROAmount + idecquadro_ee_post_tax_amount;
                        }

                        //Calculating Taxable Components of QDRO

                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_pre_tax_amount > 0.0M)
                        {
                            idecquadro_ee_pre_tax_amount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_pre_tax_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecquadro_ee_pre_tax_amount;

                        }
                        else
                        {
                            idecquadro_ee_pre_tax_amount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_ee_pre_tax_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecquadro_ee_pre_tax_amount;

                        }
                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_er_pickup_amount > 0.0M)
                        {
                            idecquadro_ee_er_pickup_amount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_ee_er_pickup_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecquadro_ee_er_pickup_amount;

                        }
                        else
                        {
                            idecquadro_ee_er_pickup_amount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_ee_er_pickup_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecquadro_ee_er_pickup_amount;

                        }
                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_er_vested_amount > 0.0M)
                        {
                            idecquadro_er_vested_amount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_er_vested_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecquadro_er_vested_amount;

                        }
                        else
                        {
                            idecquadro_er_vested_amount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_er_vested_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecquadro_er_vested_amount;

                        }
                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_interest_amount > 0.0M)
                        {
                            idecquadro_interest_amount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_interest_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecquadro_interest_amount;

                        }
                        else
                        {
                            idecquadro_interest_amount = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_interest_amount * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecquadro_interest_amount;

                        }

                        if (lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_capital_gain > 0.0M)
                        {
                            idecquadro_capital_gain = ((lobjbenefitdroApplication.icdoBenefitDroApplication.overridden_capital_gain * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecquadro_capital_gain;

                        }
                        else
                        {
                            idecquadro_capital_gain = ((lobjbenefitdroApplication.icdoBenefitDroApplication.computed_capital_gain * lobjbenefitdroApplication.icdoBenefitDroApplication.monthly_benefit_percentage) / 100.0M);
                            ldecTotalTaxableQDROAmount = ldecTotalTaxableQDROAmount + idecquadro_capital_gain;

                        }
                    }
                }
            }
            if (bsetQDROAmount && (iclbBenefitDroApplicationByPersonPlan.Count() > 0 && 
                (iclbBenefitDroApplicationByPersonPlan.Any(i => i.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusQualified 
                || i.icdoBenefitDroApplication.dro_status_value == busConstant.DROApplicationStatusApproved)))) //PIR 18808 - Update the QDRO amount only if there is DRO Application, else keep it as is.
            {
                icdoBenefitCalculation.qdro_amount = ldecTotalQDROAmount;
            }
            icdoBenefitCalculation.taxable_qdro_amount = ldecTotalTaxableQDROAmount;
            icdoBenefitCalculation.non_taxable_qdro_amount = ldecTotalNonTaxableQDROAmount;
        }

        public bool IsMemberContributed()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.iclbRetirementContributionAll == null)
                ibusPersonAccount.LoadRetirementContributionAll();
            if (ibusPersonAccount.iclbRetirementContributionAll.Count > 0 || 
                (icdoBenefitCalculation.calculation_type_value != busConstant.CalculationTypeEstimate ||
                icdoBenefitCalculation.calculation_type_value != busConstant.CalculationTypeEstimateSubsequent)) //PIR 19594
                return true;
            return false;
        }

        //this method checks whether the decimal part entered 
        //in the SSLI age if with in the range of (1/12 to 11/12)
        public bool IsSSLIDecimalPartWithinRange()
        {
            if (icdoBenefitCalculation.ssli_or_uniform_income_commencement_age > 0.00M)
            {
                decimal ldecSSLITruncated = decimal.Truncate(icdoBenefitCalculation.ssli_or_uniform_income_commencement_age);

                decimal ldecDecimalPart = icdoBenefitCalculation.ssli_or_uniform_income_commencement_age - ldecSSLITruncated;
                if (ldecDecimalPart > 0.00M)
                {
                    if (!((ldecDecimalPart >= 0.0833M)
                        && (ldecDecimalPart <= 0.9167M)))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        //UCS-084 29
        //check if the retirement date and termination date are not prior to Go live Date
        public bool IsRetirementAndTerminationDateAfterGoLiveDate()
        {
            if (icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO)
            {
                DateTime ldtGoLiveDate = busPayeeAccountHelper.GetPERSLinkGoLiveDate();
                if ((icdoBenefitCalculation.termination_date != DateTime.MinValue)
                    && (icdoBenefitCalculation.normal_retirement_date != DateTime.MinValue))
                {
                    if ((ldtGoLiveDate > icdoBenefitCalculation.termination_date)
                        && (ldtGoLiveDate > icdoBenefitCalculation.normal_retirement_date))
                        return false;
                }
            }
            return true;
        }

        # region Correspondence 80

        public string istrNormalEligibilityRule { get; set; }
        public int iintNormalEligibilityAge { get; set; }
        public decimal idecNormalEligibilityAttainedAge { get; set; }
        public DateTime idtEligibleDateForRuleConversionMinus2Months
        {
            get
            {
                DateTime ldtReturnDate = DateTime.MinValue;
                if (idtNormalEligibilityDateByRule != DateTime.MinValue)
                    ldtReturnDate = idtNormalEligibilityDateByRule.AddMonths(-2);
                return ldtReturnDate;
            }
        }

        public DateTime idtNormalEligibilityDateByAge { get; set; }
        public DateTime idtNormalEligibilityDateByRule { get; set; }

        //This is used for 80 correspondence
        //getting all normal eligibility details used in cor
        //1. get rule or age based on plan
        //2. get normal retirement date based on Age and Rule
        //3. Get first day of month of normal eligibility
        //4. get attained age of the member as on normal retirement date
        public void GetNormalEligibilityDetails(busDBCacheData abusDBCacheData = null)
        {
            if (ibusMember == null)
                LoadMember();
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
				//PIR 14646 - Main Benefit Tier Changes
            if (ibusPersonAccount.ibusPersonAccountRetirement.IsNull())
                ibusPersonAccount.LoadPersonAccountRetirement();
            string lstrBenefitTierValue = string.Empty;
            if (ibusPlan.IsNotNull() && ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMain)
            {
                if (ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.MainBenefit2016Tier)
                    lstrBenefitTierValue = ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
                else
                    lstrBenefitTierValue = busConstant.MainBenefit1997Tier;
            }
            else if(ibusPlan.IsNotNull() && ibusPlan.icdoPlan.plan_id == busConstant.PlanIdBCILawEnf)//PIR 26282
            {
                if (ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.BCIBenefit2023Tier)
                    lstrBenefitTierValue = ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
                else
                    lstrBenefitTierValue = busConstant.BCIBenefit2011Tier;
            }
            //PIR 26544
            else if (ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value.IsNotNullOrEmpty())
                lstrBenefitTierValue = ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;

            //load benefit Provision Eligibility details from DB cache
            if (abusDBCacheData == null)
            {
                abusDBCacheData = new busDBCacheData();
                abusDBCacheData.idtbCachedBenefitProvisionEligibility = busGlobalFunctions.LoadBenefitProvisionEligibilityCacheData(iobjPassInfo);
            }
            Collection<cdoBenefitProvisionEligibility> lobjBenefitProvisionEligibility = new Collection<cdoBenefitProvisionEligibility>();
            var lobjResult = from a in abusDBCacheData.idtbCachedBenefitProvisionEligibility.AsEnumerable()
                             where a.Field<int>("benefit_provision_id") == ibusPlan.icdoPlan.benefit_provision_id
                              && a.Field<string>("benefit_account_type_value") == busConstant.ApplicationBenefitTypeRetirement
                              && a.Field<string>("ELIGIBILITY_TYPE_VALUE") == busConstant.BenefitProvisionEligibilityNormal
                              && a.Field<string>("BENEFIT_TIER_VALUE") == (string.IsNullOrEmpty(lstrBenefitTierValue) ? null : lstrBenefitTierValue)
                             select a;

            lobjBenefitProvisionEligibility = cdoBenefitProvisionEligibility.GetCollection<cdoBenefitProvisionEligibility>(lobjResult.AsDataTable());


            //get rule no based on plan
            if (lobjBenefitProvisionEligibility.Count > 0)
            {
                if ((icdoBenefitCalculation.plan_id != busConstant.PlanIdNG)
                    && (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService))
                {
                    istrNormalEligibilityRule = lobjBenefitProvisionEligibility[0].age_plus_service.ToString();
                }
                iintNormalEligibilityAge = (int)lobjBenefitProvisionEligibility[0].age;
            }
            //get current normal retirement date
            DateTime ldtNormalEligibilityDate = icdoBenefitCalculation.normal_retirement_date;

            //Systest PIR: 1227. The Service Credit sent as the last parameter is the selected service purchase in the 
            //Estimate screen. For all others it should be zero.
            //Dont Modify the Code.
            decimal ldecConsolidatedServiceCredit = 0.0M;
            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate ||
                icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) //PIR 19594
            {
                if (idecRemainingServiceCredit == 0.00M)
                    LoadRemainingServicePurchaseCredit();
                ldecConsolidatedServiceCredit = idecRemainingServiceCredit + icdoBenefitCalculation.adjusted_tvsc;
            }


            // get normal retirement date based on Age and Rule
            if (icdoBenefitCalculation.benefit_account_type_value != null)
            {
                idtNormalEligibilityDateByAge = GetNormalRetirementDateBasedOnNormalEligibility(icdoBenefitCalculation.plan_id, ibusPlan.icdoPlan.plan_code,
                                                                ibusPlan.icdoPlan.benefit_provision_id, icdoBenefitCalculation.benefit_account_type_value,
                                                                ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.credited_vsc,
                                                                2, iobjPassInfo, icdoBenefitCalculation.termination_date,
                                                            ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                            (icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimate) ||
                                                            icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimateSubsequent)), //PIR 19594
                                                            ldecConsolidatedServiceCredit, icdoBenefitCalculation.retirement_date, abusDBCacheData, ibusPersonAccount); //PIR 14646

                idtNormalEligibilityDateByRule = GetNormalRetirementDateBasedOnNormalEligibility(icdoBenefitCalculation.plan_id, ibusPlan.icdoPlan.plan_code,
                                                                ibusPlan.icdoPlan.benefit_provision_id, icdoBenefitCalculation.benefit_account_type_value,
                                                                ibusMember.icdoPerson.date_of_birth, icdoBenefitCalculation.credited_vsc,
                                                                1, iobjPassInfo, icdoBenefitCalculation.termination_date,
                                                            ibusPersonAccount.icdoPersonAccount.person_account_id,
                                                            (icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimate) ||
                                                            icdoBenefitCalculation.calculation_type_value.Equals(busConstant.CalculationTypeEstimateSubsequent)), //PIR 19594
                                                            ldecConsolidatedServiceCredit, icdoBenefitCalculation.retirement_date, abusDBCacheData, ibusPersonAccount); //PIR 14646
            }
            //get first day on month of normal eligibility date
            idtFirstDayOfMonthNormalEligibilityDate = new DateTime(idtNormalEligibilityDateByAge.Year, idtNormalEligibilityDateByAge.Month, 1);

            //get attained age as per normal retirement date
            idecNormalEligibilityAttainedAge = Convert.ToDecimal(busGlobalFunctions.CalulateAge(ibusMember.icdoPerson.date_of_birth, ldtNormalEligibilityDate));
        }

        # endregion

        public void btnPendingApprovalClick()
        {
            icdoBenefitCalculation.action_status_value = busConstant.BenefitActionStatusPending;
            icdoBenefitCalculation.Update();
        }

       
        public virtual ArrayList btnCancelClick()
        {
		   // PIR 17082
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();

            if (iclbPayeeAccount == null)
                LoadPayeeAccount();
            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccount)
            {
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsNull())
                    lobjPayeeAccount.LoadActivePayeeStatus();
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 != busConstant.PayeeAccountStatusCancelled)
                {
                    lobjError = AddError(10302, "");
                    alReturn.Add(lobjError);
                    return alReturn;
                }
            }
            if (alReturn.Count == 0)
            {
                icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusCancel;
                icdoBenefitCalculation.Update();
            }
            return alReturn;
        }

        // This property is used only in case RTW. Stores the first/initial/Retired Person Account
        public busPersonAccount ibusRetiredPersonAccount { get; set; }

        public void LoadRetiredPersonAccount()
        {
            if (ibusRetiredPersonAccount.IsNull())
                ibusRetiredPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

            if (ibusMember == null)
                LoadMember();

            if (ibusMember.icolPersonAccountByPlan == null)
                ibusMember.LoadPersonAccountByPlan(icdoBenefitCalculation.plan_id);

            ibusRetiredPersonAccount = ibusMember.icolPersonAccountByPlan.Where(o => o.icdoPersonAccount.end_date != DateTime.MinValue &&
                                        o.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired).FirstOrDefault();
        }

        // PIR ID 2074
        public decimal GetMemberOptionFactor()
        {
            decimal ldecOptionFactor = 0M;
            if ((iclbBenefitCalculationOptions.IsNotNull()) &&
                (iclbBenefitCalculationPayee.IsNotNull()))
            {
                var lbusMember = iclbBenefitCalculationPayee.Where(obj => obj.icdoBenefitCalculationPayee.account_relationship_value == busConstant.AccountRelationshipMember);
                if (lbusMember.Count() > 0)
                {
                    var lbusOption = iclbBenefitCalculationOptions.Where(obj =>
                                        (obj.icdoBenefitCalculationOptions.benefit_calculation_payee_id == lbusMember.First().icdoBenefitCalculationPayee.benefit_calculation_payee_id) &&
                                        ((obj.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption100PercentJS) ||
                                        (obj.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption50PercentJS) ||
                                        (obj.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value == busConstant.BenefitOption75Percent))
                                        );
                    if (lbusOption.Count() > 0)
                        ldecOptionFactor = lbusOption.First().icdoBenefitCalculationOptions.option_factor;
                }
            }
            return ldecOptionFactor;
        }

        //PIR:1481 Created common function to be invoked
        public int GetProjectedEstimateServiceCredit(bool bIsEmployeeSeasonal, int lintSeasonalLimit, bool IsProjectionForPSC)
        {
            int lintEstimatesContributions = 0;
            //UAT PIR: 1131 Dual FAS Date.
            SetDualFASTerminationDate();
            DateTime ldteActualEmployeeTerminationDate = new DateTime();
            GetOrgIdAsLatestEmploymentOrgId(
                                ibusPersonAccount,
                                busConstant.ApplicationBenefitTypeRetirement,
                                ref ldteActualEmployeeTerminationDate);
            //The Termination Date check commmented as per confirmation Mail from David regarding Dual Plans.Dtd:June 2 2010
            //Also the Job Service check removed as per UAT PIR: 1624
            //if (//(ldteActualEmployeeTerminationDate == DateTime.MinValue) && 
            //    (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService))
            //{
            if (idteLastContributedDate == DateTime.MinValue)
                LoadLastContributedDate();
            //PIR 24228 - PSC/VSC Is Being Wrongly Projected When Member Contributes With New Employment After A Break In 
            //Employment
            if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
            {
                if (ibusPersonAccount.iclbAccountEmploymentDetail.IsNull()) ibusPersonAccount.LoadPersonAccountEmploymentDetails();
                foreach (busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail in ibusPersonAccount.iclbAccountEmploymentDetail)
                {
                    lbusPersonAccountEmploymentDetail.LoadPersonEmploymentDetail();
                }
                if (ibusPersonAccount.iclbAccountEmploymentDetail.Count > 0)
                {
                    busPersonAccountEmploymentDetail lbusPersonAccountEmploymentDetail = ibusPersonAccount.iclbAccountEmploymentDetail
                       .Where(i => i.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueEnrolled)?
                       .OrderByDescending(i => i.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date)?
                       .FirstOrDefault();
                    DateTime ldteLatestEmpDtlStartDate = DateTime.MinValue;
                    if (lbusPersonAccountEmploymentDetail.IsNotNull())
                        ldteLatestEmpDtlStartDate = lbusPersonAccountEmploymentDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date;
                    if(ldteLatestEmpDtlStartDate != DateTime.MinValue 
                        && idteLastContributedDate != DateTime.MinValue 
                        && ldteLatestEmpDtlStartDate > idteLastContributedDate)
                    {
                        idteLastContributedDate = ldteLatestEmpDtlStartDate.AddMonths(-1).GetLastDayofMonth();
                    }
                }
            }
            if (idteLastContributedDate != DateTime.MinValue)
            {
                // SYSTEST - PIR - 1477 - For Pre-Retirement death if no termination date exists, use date of death for FAS Calculation.
                DateTime ldteTerminationDate = new DateTime();
                if ((icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath) &&
                    (icdoBenefitCalculation.termination_date == DateTime.MinValue))
                    ldteTerminationDate = icdoBenefitCalculation.date_of_death;
                else
                    ldteTerminationDate = icdoBenefitCalculation.fas_termination_date;

                // PIR ID 1920 For an RTW estimate, project salary only if the Refund selection is No.
                // Supporting PIR: 2109
                if (!((icdoBenefitCalculation.is_return_to_work_member) &&
                     (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper())))
                {
                    if (!(bIsEmployeeSeasonal))
                    {
                        lintEstimatesContributions = busGlobalFunctions.DateDiffByMonth
                            (idteLastContributedDate, icdoBenefitCalculation.fas_termination_date); //PIR 9895
                        if (lintEstimatesContributions > 0)
                        {
                            lintEstimatesContributions = lintEstimatesContributions - 1;
                        }
                    }
                    else
                    {
                        lintEstimatesContributions = CalculateProjectedSeasonalCredits(idteLastContributedDate, ldteTerminationDate, lintSeasonalLimit, icdoBenefitCalculation.retirement_date);
                    }
                }
                //UAT PIR: 1131
                //PSC Will not be accounted when projection is made for the other plan although VSC will be included.
                if (IsProjectionForPSC)
                {
                    if (iintLastSalaryPlanID > 0)
                    {
                        if (iintLastSalaryPlanID != icdoBenefitCalculation.plan_id)
                        {
                            lintEstimatesContributions = 0;
                        }
                    }
                }
            }
            //}

            return lintEstimatesContributions;
        }

        public bool IsCalcTypeFinalApproved()
        {
            if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal) &&
                (icdoBenefitCalculation.action_status_value == busConstant.CalculationStatusApproval))
                return true;
            return false;
        }

        public bool IsCalcTypeAdjustmentApproved()
        {
            if ((icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments) &&
                (icdoBenefitCalculation.action_status_value == busConstant.CalculationStatusApproval))
                return true;
            return false;
        }

        // UCS-094 Disability Benefit Percentage
        public decimal idecDisabilityBenefitPercentage
        {
            get
            {
                if (iclbBenefitMultiplier.IsNotNull() && iclbBenefitMultiplier.Count > 0)
                    return iclbBenefitMultiplier.FirstOrDefault().icdoBenefitMultiplier.benefit_multiplier_rate_percentage;
                return 0M;
            }
        }

        //UAT PIR: 1131,1159 For Dual Person
        //UAT PIR: 1164. Transfer to DC plan status excluded in checking for DUAL.
        public bool IsMemberDual()
        {
            //Get all the person accounts for the person. If the person has an person account other than the current plan for which calcualtion is done, he is dual.
            //Withdrawn,Cancelled plans not to be used.

            if (ibusMember == null)
                LoadMember();

            if (ibusMember.icolPersonAccount.IsNull())
                ibusMember.LoadPersonAccount();

            var lintPACount = ibusMember.icolPersonAccount.Where(obj =>
                                        (obj.icdoPersonAccount.plan_id != icdoBenefitCalculation.plan_id) &&
                                        (obj.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn) &&
                                        (obj.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirmentCancelled) &&
                                        (obj.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusTransferDC) &&
                                        (obj.icdoPersonAccount.plan_participation_status_value != busConstant.RetirementPlanParticipationStatusTranToDb) &&      //PIR 23991                                  
                                        (obj.icdoPersonAccount.plan_id != busConstant.PlanIdJobService) &&
                                        (obj.ibusPlan.IsRetirementPlan())).Count();

            if ((lintPACount > 0) && (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService))
            {
                return true;
            }
            return false;
        }

        //UAT PIR: 1131 Dual FAS Date.
        //UAT PIR: 1131 FAS termination date fixes.
        public bool SetDualFASTerminationDate()
        {
            bool iblnIsProjectionApplicable = true;
            icdoBenefitCalculation.fas_termination_date = icdoBenefitCalculation.termination_date;
            if ((icdoBenefitCalculation.termination_date == DateTime.MinValue) &&
                (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
            {
                icdoBenefitCalculation.fas_termination_date = icdoBenefitCalculation.date_of_death;
            }
            
            if (IsMemberDual())
            {
                DateTime ldtEmploymentEnddate = DateTime.MinValue;
                ldtEmploymentEnddate = GetLastPersonEmploymentEndDate();
                if (ldtEmploymentEnddate != DateTime.MinValue)     //means it is an Closed Employment                    
                {
                    if ((ldtEmploymentEnddate > icdoBenefitCalculation.retirement_date) && (icdoBenefitCalculation.retirement_date != DateTime.MinValue))
                    {
                        icdoBenefitCalculation.fas_termination_date = icdoBenefitCalculation.retirement_date.AddDays(-1);
                    }
                    else
                    {
                        icdoBenefitCalculation.fas_termination_date = ldtEmploymentEnddate.GetLastDayofMonth();
                    }
                    if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate || icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent) //PIR 19594
                    {
                        iblnIsProjectionApplicable = false;
                    }
                }
                //PIR 15630
                //else //Means it is a Open Employment
                //{
                //    if (icdoBenefitCalculation.retirement_date != DateTime.MinValue)
                //    {
                //        icdoBenefitCalculation.fas_termination_date = icdoBenefitCalculation.retirement_date.AddDays(-1);
                //    }
                //}
            }

            return iblnIsProjectionApplicable;
        }

        private DateTime GetLastPersonEmploymentEndDate()
        {
            // PROD PIR 8333 - Employment End date should be loaded based on Plan
            DateTime ldtEmploymentEnddate = DateTime.MinValue;
            if (ibusPersonAccount.IsNull()) LoadPersonAccount();
            LoadAllPersonEmploymentDetails(false);
            if (ibusPersonAccount.iclbEmploymentDetail.Count > 0)
                ldtEmploymentEnddate = ibusPersonAccount.iclbEmploymentDetail[0].icdoPersonEmploymentDetail.end_date;
            return ldtEmploymentEnddate;
        }

        //PIR 11414 - Added new function for getting all employment date and calculate max end date
        private void LoadAllPersonEmploymentDetails(bool ablnLoadOtherObjects)
        {
            ibusPersonAccount.iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>();
            DataTable ldtbEmpDetail = Select("cdoPersonAccountRetirement.LoadAllEmploymentDetail",
                                                          new object[1] { ibusPersonAccount.icdoPersonAccount.person_id });
            ibusPersonAccount.iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>();
            foreach (DataRow ldtrempdetails in ldtbEmpDetail.Rows)
            {
                busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtrempdetails);
                if (ablnLoadOtherObjects)
                {
                    lobjPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                    lobjPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.LoadData(ldtrempdetails);

                    lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                    lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldtrempdetails);
                    if (!Convert.IsDBNull(ldtrempdetails["ORG_STATUS_VALUE"]))
                    {
                        lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.status_value = ldtrempdetails["ORG_STATUS_VALUE"].ToString();
                    }
                }
                ibusPersonAccount.iclbEmploymentDetail.Add(lobjPersonEmploymentDetail);
            }
        }

        //UAT PIR - 925
        public DateTime idtGraduatedBenefitOptionDate
        {
            get
            { return Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.GraduatedBenefitOptionDateCodeValue, iobjPassInfo)); }
        }
        //PIR 1520
        public int iintSSLIUniformAge
        {
            get { return icdoBenefitCalculation.ssli_or_uniform_income_commencement_age == 0.00M ? 0 : Convert.ToInt32(icdoBenefitCalculation.ssli_or_uniform_income_commencement_age); }
        }
        
        public bool iblnIsApplicationModified { get; set; }

        //method to create new pre retirement death calculation from interest postin batch
        internal void CreateNewDeathCalcFromInterestPostingBatch(decimal adecInterestAmount)
        {
            cdoBenefitCalculation lcdoCalc = new cdoBenefitCalculation();
            lcdoCalc.benefit_application_id = icdoBenefitCalculation.benefit_application_id;
            lcdoCalc.person_id = icdoBenefitCalculation.person_id;
            lcdoCalc.plan_id = icdoBenefitCalculation.plan_id;
            lcdoCalc.calculation_type_id = icdoBenefitCalculation.calculation_type_id;
            lcdoCalc.calculation_type_value = busConstant.CalculationTypeAdjustments;
            lcdoCalc.benefit_account_type_id = icdoBenefitCalculation.benefit_account_type_id;
            lcdoCalc.benefit_account_type_value = icdoBenefitCalculation.benefit_account_type_value;
            lcdoCalc.benefit_account_sub_type_id = icdoBenefitCalculation.benefit_account_sub_type_id;
            lcdoCalc.benefit_account_sub_type_value = icdoBenefitCalculation.benefit_account_sub_type_value;
            lcdoCalc.benefit_option_id = icdoBenefitCalculation.benefit_option_id;
            lcdoCalc.benefit_option_value = icdoBenefitCalculation.benefit_option_value;
            lcdoCalc.termination_date = icdoBenefitCalculation.termination_date;
            lcdoCalc.normal_retirement_date = icdoBenefitCalculation.normal_retirement_date;
            lcdoCalc.retirement_date = icdoBenefitCalculation.retirement_date;
            lcdoCalc.date_of_death = icdoBenefitCalculation.date_of_death;
            lcdoCalc.rhic_option_id = icdoBenefitCalculation.rhic_option_id;
            lcdoCalc.status_id = icdoBenefitCalculation.status_id;
            lcdoCalc.status_value = busConstant.StatusValid; //PROD PIR 8301
            lcdoCalc.action_status_id = icdoBenefitCalculation.action_status_id;
            lcdoCalc.action_status_value = busConstant.CalculationStatusPendingApproval;
            lcdoCalc.rtw_refund_election_id = icdoBenefitCalculation.rtw_refund_election_id;
            lcdoCalc.post_retirement_death_reason_type_id = icdoBenefitCalculation.post_retirement_death_reason_type_id;
            lcdoCalc.rule_indicator_id = icdoBenefitCalculation.rule_indicator_id;
            lcdoCalc.graduated_benefit_option_id = icdoBenefitCalculation.graduated_benefit_option_id;
            lcdoCalc.tffr_calculation_method_id = icdoBenefitCalculation.tffr_calculation_method_id;
            lcdoCalc.Insert();

            CreateDeathCalculationPayee(lcdoCalc.benefit_calculation_id, adecInterestAmount);            
        }

        private void CreateDeathCalculationPayee(int aintBenefitCalculationID, decimal adecInterestAmount)
        {
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();
            if (iclbBenefitCalculationOptions == null)
                LoadBenefitCalculationOptions();

            cdoBenefitCalculationPayee lcdoPayee = new cdoBenefitCalculationPayee();
            cdoBenefitCalculationOptions lcdoOption = new cdoBenefitCalculationOptions();
            int i = 1;
            decimal ldecPercentageAmount = 0.000000M, ldecRemainingAmount = 0.000000M;
            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                ldecPercentageAmount = ldecRemainingAmount = 0.000000M;
                lcdoPayee = new cdoBenefitCalculationPayee();
                lcdoPayee = lobjPayee.icdoBenefitCalculationPayee;
                lcdoPayee.benefit_calculation_id = aintBenefitCalculationID;
                lcdoPayee.benefit_calculation_payee_id = 0;
                lcdoPayee.payee_account_id = 0;
                lcdoPayee.created_by = null;
                lcdoPayee.created_date = DateTime.MinValue;
                lcdoPayee.modified_by = null;
                lcdoPayee.modified_date = DateTime.MinValue;
                lcdoPayee.update_seq = 0;
                lcdoPayee.Insert();

                busBenefitCalculationOptions lbusOption = iclbBenefitCalculationOptions
                    .Where(o => o.icdoBenefitCalculationOptions.benefit_calculation_payee_id == lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id)
                    .FirstOrDefault();
                if (lbusOption != null)
                {
                    lcdoOption = new cdoBenefitCalculationOptions();
                    lcdoOption.benefit_calculation_id = aintBenefitCalculationID;
                    lcdoOption.benefit_calculation_payee_id = lcdoPayee.benefit_calculation_payee_id;
                    lcdoOption.benefit_provision_benefit_option_id = lbusOption.icdoBenefitCalculationOptions.benefit_provision_benefit_option_id;
                    lcdoOption.graduated_benefit_option_id = lbusOption.icdoBenefitCalculationOptions.graduated_benefit_option_id;
                    lcdoOption.option_factor = lbusOption.icdoBenefitCalculationOptions.option_factor;
                    if (iclbBenefitCalculationPayee.Count == 1)
                    {
                        lcdoOption.interest_amount = adecInterestAmount;
                        lcdoOption.taxable_amount = adecInterestAmount;
                        lcdoOption.benefit_option_amount = adecInterestAmount;
                    }
                    else
                    {
                        ldecPercentageAmount = Math.Round(adecInterestAmount * lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100, 6, MidpointRounding.AwayFromZero);
                        if (i == iclbBenefitCalculationPayee.Count)
                        {
                            ldecRemainingAmount = adecInterestAmount - (ldecPercentageAmount * (iclbBenefitCalculationPayee.Count - 1));
                            lcdoOption.interest_amount = ldecRemainingAmount;
                            lcdoOption.taxable_amount = ldecRemainingAmount;
                            lcdoOption.benefit_option_amount = Math.Round(ldecRemainingAmount, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            lcdoOption.interest_amount = ldecPercentageAmount;
                            lcdoOption.taxable_amount = ldecPercentageAmount;
                            lcdoOption.benefit_option_amount = Math.Round(ldecPercentageAmount, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    lcdoOption.Insert();
                }
                i++;
            }
        }

        //method to update pre retirement death calculation from interest postin batch
        internal void UpdateDeathCalcFromInterestPostingBatch(decimal adecInterestAmount)
        {
            if (iclbPayeeAccount == null)
                LoadPayeeAccount();
            if (iclbBenefitCalculationPayee == null)
                LoadBenefitCalculationPayee();
            if (iclbBenefitCalculationOptions == null)
                LoadBenefitCalculationOptions();

            int i = 1;
            decimal ldecPercentageAmount = 0.000000M, ldecRemainingAmount = 0.000000M, ldecAdditionalInterestPAPIT = 0.00M, ldecInterestAmountAllocated = 0.000000M;
            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                lobjPayeeAccount = new busPayeeAccount();
                lobjPayeeAccount.FindPayeeAccount(lobjPayee.icdoBenefitCalculationPayee.payee_account_id);
                ldecPercentageAmount = ldecRemainingAmount = 0.000000M;
                ldecAdditionalInterestPAPIT = 0.00M;

                busBenefitCalculationOptions lbusOption = iclbBenefitCalculationOptions
                   .Where(o => o.icdoBenefitCalculationOptions.benefit_calculation_payee_id == lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id)
                   .FirstOrDefault();
                if (lbusOption != null)
                {
                    if (iclbBenefitCalculationPayee.Count == 1)
                    {
                        lbusOption.icdoBenefitCalculationOptions.interest_amount += adecInterestAmount;
                        lbusOption.icdoBenefitCalculationOptions.taxable_amount += adecInterestAmount;
                        lbusOption.icdoBenefitCalculationOptions.benefit_option_amount += adecInterestAmount;
                        ldecAdditionalInterestPAPIT = adecInterestAmount;
                    }
                    else
                    {
                        ldecPercentageAmount = Math.Round(adecInterestAmount * lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100, 6, MidpointRounding.AwayFromZero);
                        if (i == iclbBenefitCalculationPayee.Count)
                        {
                            ldecRemainingAmount = adecInterestAmount - ldecInterestAmountAllocated;
                            lbusOption.icdoBenefitCalculationOptions.interest_amount += ldecRemainingAmount;
                            lbusOption.icdoBenefitCalculationOptions.taxable_amount += ldecRemainingAmount;
                            lbusOption.icdoBenefitCalculationOptions.benefit_option_amount += Math.Round(ldecRemainingAmount, 2, MidpointRounding.AwayFromZero);
                            ldecAdditionalInterestPAPIT = Math.Round(ldecRemainingAmount, 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            lbusOption.icdoBenefitCalculationOptions.interest_amount += ldecPercentageAmount;
                            lbusOption.icdoBenefitCalculationOptions.taxable_amount += ldecPercentageAmount;
                            lbusOption.icdoBenefitCalculationOptions.benefit_option_amount += Math.Round(ldecPercentageAmount, 2, MidpointRounding.AwayFromZero);
                            ldecAdditionalInterestPAPIT = Math.Round(ldecPercentageAmount, 2, MidpointRounding.AwayFromZero);
                        }
                        ldecInterestAmountAllocated += ldecPercentageAmount;
                    }
                    lbusOption.icdoBenefitCalculationOptions.Update();
                    if (ldecAdditionalInterestPAPIT > 0.00M && lobjPayeeAccount.icdoPayeeAccount.payee_account_id > 0)
                    {
                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.RefundPaymentItemAdditionalEEInterestAmount,
                                                                   ldecAdditionalInterestPAPIT, string.Empty, 0,
                                                                   busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1), DateTime.MinValue);
                    }
                    if (lobjPayeeAccount.iclbActiveRolloverDetails == null)
                        lobjPayeeAccount.LoadActiveRolloverDetail();

                    if (lobjPayeeAccount.iclbActiveRolloverDetails.Count > 0)
                    {
                        lobjPayeeAccount.CreateRolloverAdjustment();
                    }
                    else
                    {
                        lobjPayeeAccount.CalculateAdjustmentTax(false);
                    }
                }
                i++;
            }
        }

        public busBenefitCalculatorWeb ibusBenefitCalculatorWeb { get; set; }
        public void LoadBenefitcalculatorWeb()
        {
            if (ibusBenefitCalculatorWeb.IsNull())
                ibusBenefitCalculatorWeb = new busBenefitCalculatorWeb { icdoWssBenefitcalculator = new cdoWssBenefitCalculator() };
            DataTable ldtbResult = Select<cdoWssBenefitCalculator>(new string[1] { "benefit_calculation_id" }, new object[1] { icdoBenefitCalculation.benefit_calculation_id }, null, null);
            if (ldtbResult.Rows.Count > 0)
                ibusBenefitCalculatorWeb.icdoWssBenefitcalculator.LoadData(ldtbResult.Rows[0]);
        }
        //PIR 14346 Changes
        public void CreateRHICCombineOnCalculationApproval(busPayeeAccount abusPayeeAccount, string astrCalculationType)
        {
            if (abusPayeeAccount.ibusBenefitAccount == null)
                abusPayeeAccount.LoadBenfitAccount();
            if ((abusPayeeAccount.icdoPayeeAccount.payee_perslink_id > 0) && ((abusPayeeAccount.icdoPayeeAccount.rhic_amount > 0) ||
                (abusPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0)))
            {
                busBenefitRhicCombine lbusBenefitRhicCombine = new busBenefitRhicCombine { icdoBenefitRhicCombine = new cdoBenefitRhicCombine() };
                lbusBenefitRhicCombine.icdoBenefitRhicCombine.person_id = abusPayeeAccount.icdoPayeeAccount.payee_perslink_id;
                lbusBenefitRhicCombine.ibusPerson = abusPayeeAccount.ibusPayee;
                //bool lblnInitiateWorkflow = false;
                //PIR 15600
                DateTime ldtEffectiveStartDate;
                if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                {
                    ldtEffectiveStartDate = CalculateRHICEffectiveStartDateForCalculationApproval(abusPayeeAccount /*, ref lblnInitiateWorkflow*/);
                }
                else
                {
                    ldtEffectiveStartDate = icdoBenefitCalculation.rhic_effective_date;
                }
                
                busBenefitRhicCombineDetail lbusBenefitRhicCombineDetail = null;
                //to check whether RHIC amount changed for calculation type of adjustments 
                if (astrCalculationType == busConstant.CalculationTypeAdjustments || astrCalculationType == busConstant.CalculationTypeSubsequentAdjustment)
                {
                    if (abusPayeeAccount.ibusLatestBenefitRhicCombine.IsNull()) abusPayeeAccount.LoadLatestBenefitRhicCombine(true);
                    {
                        if (abusPayeeAccount.ibusLatestBenefitRhicCombine.IsNotNull())
                        {
                            abusPayeeAccount.ibusLatestBenefitRhicCombine.LoadBenefitRhicCombineDetails();
                            if (abusPayeeAccount.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.person_id == abusPayeeAccount.icdoPayeeAccount.payee_perslink_id)
                            {
                                lbusBenefitRhicCombineDetail = abusPayeeAccount.ibusLatestBenefitRhicCombine.iclbBenefitRhicCombineDetail.Where(i => i.icdoBenefitRhicCombineDetail.donar_payee_account_id
                                     == abusPayeeAccount.icdoPayeeAccount.payee_account_id).FirstOrDefault();
                            }
                        }
                    }
                }
                //if (lblnInitiateWorkflow)
                //{
                //    lbusBenefitRhicCombine.InitiateRHICCombineWorkflow();
                //    return;
                //}

                //if (ldtEffectiveStartDate != DateTime.MinValue)
                //{
                bool lblnAutoRhicEstablished = false;
                if (astrCalculationType == busConstant.CalculationTypeFinal || astrCalculationType == busConstant.CalculationTypeSubsequent)
                {
                    lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = ldtEffectiveStartDate;
                    lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.benefit_calculation_approval;
                    lblnAutoRhicEstablished = lbusBenefitRhicCombine.CreateAutomatedRHICCombine();
                }
                else if ((astrCalculationType == busConstant.CalculationTypeAdjustments || astrCalculationType == busConstant.CalculationTypeSubsequentAdjustment ) && lbusBenefitRhicCombineDetail.IsNotNull() &&
                    abusPayeeAccount.icdoPayeeAccount.rhic_amount != lbusBenefitRhicCombineDetail.icdoBenefitRhicCombineDetail.rhic_amount)
                {
                    lbusBenefitRhicCombine.icdoBenefitRhicCombine.start_date = ldtEffectiveStartDate;
                    lbusBenefitRhicCombine.ienmAutomaticRhicCombineTrigger = busConstant.automatic_rhic_combine_trigger.benefit_adjustment_approval;
                    lblnAutoRhicEstablished = lbusBenefitRhicCombine.CreateAutomatedRHICCombine();
                }
                if (lblnAutoRhicEstablished)
                {
                    lbusBenefitRhicCombine.CreatePayrollAdjustment();
                    lbusBenefitRhicCombine.CreatePAPITAdjustment();
                }

                //}
            }
        }

        public busDBCacheData ibusDBCacheData { get; set; } //Backlog PIR - 1869
        private DateTime CalculateRHICEffectiveStartDateForCalculationApproval(busPayeeAccount abusPayeeAccount
            //,ref bool ablnOutInitiateWorkflow
                                                                                )
        {
            DateTime ldtEffectiveStartDate = DateTime.MinValue;
            ldtEffectiveStartDate = abusPayeeAccount.icdoPayeeAccount.benefit_begin_date;

            //Existing logic for on payment initial approval logic
            //if ((abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) ||
            //   (abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
            //{
            //    //NDPERS defined first payment date
            //    if (abusPayeeAccount.ibusApplication == null)
            //        abusPayeeAccount.LoadApplication();

            //    if (abusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value != busConstant.ApplicationBenefitSubTypeDNRO
            //        && abusPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value != busConstant.ApplicationBenefitSubTypeEarly) //PIR 13085
            //    {
            //        ldtEffectiveStartDate = abusPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_date.GetFirstDayofNextMonth();
            //    }
            //    else //for DNRO retirement payee account
            //    {
            //        ldtEffectiveStartDate = abusPayeeAccount.ibusApplication.icdoBenefitApplication.retirement_date;
            //    }

            //    if (abusPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
            //        abusPayeeAccount.LoadNexBenefitPaymentDate();

            //    //If the first payment is being disbursed later than defined benefit payment cycle, initiate workflow
            //    if (ldtEffectiveStartDate < abusPayeeAccount.idtNextBenefitPaymentDate)
            //    {
            //        ablnOutInitiateWorkflow = true;
            //        return DateTime.MinValue;
            //    }
            //}
            //else if ((abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) ||
            //         (abusPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath))
            //{
            //    ldtEffectiveStartDate = abusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
            //}

            //DC Plan
            if (abusPayeeAccount.ibusPlan == null)
                abusPayeeAccount.LoadPlan();

            if (abusPayeeAccount.ibusPayee == null)
                abusPayeeAccount.LoadPayee();
            if (abusPayeeAccount.ibusPlan.IsDCRetirementPlan() || abusPayeeAccount.ibusPlan.IsHBRetirementPlan())
            {
                //Sujatha Sent Mail Maik dated on 9/3/2010
                //If prior NG, logic needs to modify
                //bool lblnPriorNG = false;
                ////UAT PIR:2077 Changes.
                //if (abusPayeeAccount.ibusPayee.IsFormerDBPlanTransfertoDC(busConstant.PlanIdNG))
                //    lblnPriorNG = true;

                //if (lblnPriorNG)
                //{
                //    DateTime ldtRetirementDate = abusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                //    DateTime ldtPayee50YearDate = abusPayeeAccount.ibusPayee.icdoPerson.date_of_birth.AddYears(50).GetFirstDayofNextMonth();
                //    ldtEffectiveStartDate = ldtRetirementDate > ldtPayee50YearDate ? ldtRetirementDate : ldtPayee50YearDate;
                //}
                //else
                //{
                //DateTime ldtRetirementDate = abusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                DateTime ldtPayee55YearDate = abusPayeeAccount.ibusPayee.icdoPerson.date_of_birth.AddYears(55).GetFirstDayofNextMonth();
                DateTime ldtRuleOf85Date = abusPayeeAccount.GetRuleOf85Date().GetFirstDayofNextMonth();

                //Logic Change UAT PIR : 2052 Ref: David Mail dated on 7/28/2010
                DateTime ldtEarlierOfAge55VsRule85 = ldtPayee55YearDate < ldtRuleOf85Date ? ldtPayee55YearDate : ldtRuleOf85Date; //Earlier

                ldtEffectiveStartDate = ldtEarlierOfAge55VsRule85 < DateTime.Today ? ldtEffectiveStartDate : ldtEarlierOfAge55VsRule85; //later
                //}
            }
            return ldtEffectiveStartDate;
        }

        // PIR 15927
        private Collection<busPersonAccountRetirementContribution> _iclcSalaryVarianceRecords;
        public Collection<busPersonAccountRetirementContribution> iclcSalaryVarianceRecords
        {
            get { return _iclcSalaryVarianceRecords; }
            set { _iclcSalaryVarianceRecords = value; }
        }

        private Collection<busEmployerPayrollDetail> _iclbEmployerPayrollDetail;
        public Collection<busEmployerPayrollDetail> iclbEmployerPayrollDetail
        {
            get { return _iclbEmployerPayrollDetail; }
            set { _iclbEmployerPayrollDetail = value; }
        }

        public void LoadBenefitCalculationPCD()
        {
            decimal ldecPreviousMonthSalary = 0;
            string lstrPreviousOrgCode = string.Empty;

            DataTable ldtbContributionDetail = Select("entPersonAccountRetirementContribution.LoadPCD",
                                                          new object[2] { ibusPersonAccount.icdoPersonAccount.person_account_id, ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.fas_no_periods_range });

            iclcSalaryVarianceRecords = GetCollection<busPersonAccountRetirementContribution>(ldtbContributionDetail, "icdoPersonAccountRetirementContribution"); 
           
            foreach (busPersonAccountRetirementContribution lobjPersonAccountRetirementContribution in iclcSalaryVarianceRecords.Reverse())
            {
                lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.BenCalc_Plan_Name = ibusPlan.icdoPlan.plan_name;
                if (lstrPreviousOrgCode != lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.org_code)
                    ldecPreviousMonthSalary = 0;
                if (ldecPreviousMonthSalary != lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount && ldecPreviousMonthSalary > 0)
                    lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.MonthlySalaryChangePerc = Math.Round(((lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount - ldecPreviousMonthSalary) / ldecPreviousMonthSalary), 4) * 100;
                ldecPreviousMonthSalary = lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount;
                lstrPreviousOrgCode = lobjPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.org_code;
            }
        }

        public void LoadEmployerPayrollDetailForFAS()
        {
            _iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
            DataTable ldtbList = Select<cdoEmployerPayrollDetail>(new string[2] { enmEmployerPayrollDetail.person_id.ToString(), enmEmployerPayrollDetail.plan_id.ToString() }, new object[2] { ibusPersonAccount.icdoPersonAccount.person_id, icdoBenefitCalculation.plan_id }, null, null);
            foreach (DataRow ldtrow in ldtbList.Rows)
            {
                busEmployerPayrollDetail lobjEmployerPayrollDetail = new busEmployerPayrollDetail { icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail() };
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.LoadData(ldtrow);
                lobjEmployerPayrollDetail.LoadPayrollHeader();
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.istrlookupComment = lobjEmployerPayrollDetail.LoadCommentsOfPayrollDetailForExportToText();
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_name = ibusPlan.icdoPlan.plan_name;
                _iclbEmployerPayrollDetail.Add(lobjEmployerPayrollDetail);
            }
        }
        public bool iblnAdditionalContributionsReported { get; set; } // PIR 17140

        //PIR 17140 - Put calculation in review when additional contributions come in via person account adjustment, employer report posting, interest posting
        public bool PutCalcInReview()
        {
            try
            {
                if (ibusSoftErrors == null) LoadErrors();
                iblnClearSoftErrors = false;
                ibusSoftErrors.iblnClearError = false;
                iblnAdditionalContributionsReported = true;
                ValidateSoftErrors();
                UpdateValidateStatus();
                return true;
            }
            catch(Exception ex)
            {
                Sagitec.ExceptionPub.ExceptionManager.Publish(ex);
                return false;
            }
        }

        //PIR 16989 - Validation 1916 should be triggered for Final calculation (all benefit types)
        public bool CheckIFTentativeTFFROrTIAAExistsForPerson()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson == null)
                ibusPersonAccount.LoadPerson();

            if (ibusPersonAccount.ibusPerson.iclbTffrTiaaService == null)
                ibusPersonAccount.ibusPerson.LoadTffrTiaaService();
            foreach (busPersonTffrTiaaService lobjPersonTffrTiaaService in ibusPersonAccount.ibusPerson.iclbTffrTiaaService)
            {
                if (lobjPersonTffrTiaaService.icdoPersonTffrTiaaService.tffr_service_status_value == busConstant.PersonTFFRTIAAServiceStatusTentative || lobjPersonTffrTiaaService.icdoPersonTffrTiaaService.tiaa_service_status_value == busConstant.PersonTFFRTIAAServiceStatusTentative)
                {
                    return true;
                }
            }
            return false;
        }

        public int iintBenAppID { get; set; }

        public void InitiatePostCreatePayeeAccountProcess(int aintPayeeAccountId)
        {
            DataTable ldtbWssBenApps = Select<cdoWssBenApp>(new string[2] { enmWssBenApp.bene_appl_id.ToString(), enmWssBenApp.ben_action_status_value.ToString() },
                                                    new object[2] { ibusBenefitApplication.icdoBenefitApplication.benefit_application_id, busConstant.BenefitApplicationActionStatusCompleted },
                                                        null, "WSS_BEN_APP_ID DESC");
            if (ldtbWssBenApps.Rows.Count > 0 && (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal))
            {
                int lintWssBenAppId = (ldtbWssBenApps.Rows[0]["WSS_BEN_APP_ID"] != DBNull.Value) ? Convert.ToInt32(ldtbWssBenApps.Rows[0]["WSS_BEN_APP_ID"]) : 0;
                busPayeeAccount lbusPayeeAccount = new busPayeeAccount();
                if (lintWssBenAppId > 0 && lbusPayeeAccount.FindPayeeAccount(aintPayeeAccountId))
                {
                    lbusPayeeAccount.iintWssBenAppId = lintWssBenAppId;
                    iintBenAppID = lintWssBenAppId;
                    lbusPayeeAccount.CreateACHAndTaxesFromWssBenAppId();
                }
            }
        }
        public bool IsPersonReached401aFlag()
        {
            if (ibusMember.IsNull())
                LoadMember();
            return (ibusMember.icdoPerson.limit_401a == busConstant.Flag_Yes);
        }
        public void CreateDefaultWithHolding(int aintPayeeAccountID)
        {
            busPayeeAccount lbusPayeeAccount = new busPayeeAccount() { icdoPayeeAccount = new cdoPayeeAccount() { payee_account_id = aintPayeeAccountID } };
            lbusPayeeAccount.LoadTaxWithHoldingHistory();
            if (lbusPayeeAccount.FindPayeeAccount(aintPayeeAccountID)
                && lbusPayeeAccount.DoesTaxWithholdingExistForFedTaxRefund())
            {
                CreateTaxWithHolding(lbusPayeeAccount, busConstant.PayeeAccountTaxIdentifierFedTax, busConstant.BenefitDistributionLumpSum, busConstant.PayeeAccountTaxRefFed22Tax);
            }
            if (lbusPayeeAccount.FindPayeeAccount(aintPayeeAccountID) 
                && lbusPayeeAccount.DoesTaxWithholdingExistForStateTaxRefund())
            {
                CreateTaxWithHolding(lbusPayeeAccount, busConstant.PayeeAccountTaxIdentifierStateTax, busConstant.BenefitDistributionLumpSum, busConstant.PayeeAccountTaxRefState22Tax);
            }
            lbusPayeeAccount.LoadPayeeAccountPaymentItemType();
            if (lbusPayeeAccount.iclbPayeeAccountPaymentItemType.Any(papit => papit.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRMDAmount))
            {
                if (!lbusPayeeAccount.iclbTaxWithholingHistory.Any(withholding => withholding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax &&
                                                                   withholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD))
                {
                    CreateTaxWithHolding(lbusPayeeAccount, busConstant.PayeeAccountTaxIdentifierFedTax, busConstant.BenefitDistributionRMD, busConstant.PayeeAccountTaxRefFed22Tax);
                }
                if (!lbusPayeeAccount.iclbTaxWithholingHistory.Any(withholding => withholding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax &&
                                                                    withholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD))
                {
                    CreateTaxWithHolding(lbusPayeeAccount, busConstant.PayeeAccountTaxIdentifierStateTax, busConstant.BenefitDistributionRMD, busConstant.PayeeAccountTaxRefState22Tax);
                }
            }
        }
        public void CreateTaxWithHolding(busPayeeAccount abusPayeeAccount, string astrTaxIdentifier, string astrBenefitDistribution, string astrTaxRef)
        {
            try
            {
                busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding() { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
                lbusPayeeAccountTaxWithholding.ibusPayeeAccount = abusPayeeAccount;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = astrTaxIdentifier;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value = astrBenefitDistribution;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_ref = astrTaxRef;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_id = abusPayeeAccount.icdoPayeeAccount.payee_account_id;
                //PIR 25668 for auto refund only if payee address is non ND then state tax with holding not eligible hence update with No ND state tax withholding option and amount 0
				if (astrTaxIdentifier == busConstant.PayeeAccountTaxIdentifierStateTax 
                    && icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund
                    && icdoBenefitCalculation.benefit_account_sub_type_value == busConstant.BenefitOptionAutoRefund
                    && icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionAutoRefund && IsAddressNotInNDState())
                {
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value = busConstant.NoStateTaxWithheld;
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_tax_option = busConstant.NoStateTaxWithheld;
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_state_amt = 0.00M;
                }
                if (astrTaxIdentifier == busConstant.PayeeAccountTaxIdentifierStateTax && string.IsNullOrEmpty(lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value))
                {
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_tax_option = busConstant.TaxOptionStateTaxwithheld;
                }
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.ienuObjectState = ObjectState.Insert;
                lbusPayeeAccountTaxWithholding.LoadDefaultTaxValues();
                lbusPayeeAccountTaxWithholding.BeforeValidate(utlPageMode.New);
                lbusPayeeAccountTaxWithholding.ValidateHardErrors(utlPageMode.New);
                if (lbusPayeeAccountTaxWithholding.iarrErrors.Count == 0)
                {
                    lbusPayeeAccountTaxWithholding.BeforePersistChanges();
                    lbusPayeeAccountTaxWithholding.PersistChanges();
                    lbusPayeeAccountTaxWithholding.AfterPersistChanges();
                }
            }
            catch (Exception ex)
            {
                Sagitec.ExceptionPub.ExceptionManager.Publish(ex);
            }
        }
		//PIR 25668 check payee address and is from state ND or not -- non ND return true
        public bool IsAddressNotInNDState()
        {
            if (ibusBenefitApplication.IsNull())
                LoadBenefitApplication();
            if (ibusBenefitApplication.ibusPerson.IsNull())
                ibusBenefitApplication.LoadPerson();
            if (ibusBenefitApplication.ibusPerson.ibusPersonCurrentAddress.IsNull())
                ibusBenefitApplication.ibusPerson.GetPersonLatestAddress();
            if (ibusBenefitApplication?.ibusPerson?.ibusPersonCurrentAddress?.icdoPersonAddress?.addr_state_value != busConstant.StateNorthDakota)
                return true;
            return false;
        }

        //PIR 23183
        public bool IsFASCalculatedBy3Consecutive12MonthsPeriod()
        {
            DateTime ldteCheckTerminationDate = new DateTime(2019, 12, 31);
            if ((icdoBenefitCalculation.termination_date > ldteCheckTerminationDate) &&
                  (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService) &&
                  (icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService3rdPartyPayor))
            {
                DateTime ldteTerminationDateUpdated = new DateTime();
                DateTime ldteTerminationDate = new DateTime();
                Collection<busPersonAccountRetirementContribution> lclbFinalSalaryRecords = new Collection<busPersonAccountRetirementContribution>();

                if (ibusPersonAccount == null)
                    LoadPersonAccount();

                if (idteLastContributedDate == DateTime.MinValue)
                    LoadLastContributedDate();

                DataTable idtAllContEmpdetails = LoadAllContributingEmploymentDetails();

                if (idtAllContEmpdetails.IsNotNull() && idtAllContEmpdetails.Rows.Count > 0)
                {
                    ldteTerminationDateUpdated = (idtAllContEmpdetails.Rows[0]["END_DATE"] != DBNull.Value) ? Convert.ToDateTime(idtAllContEmpdetails.Rows[0]["END_DATE"]) : DateTime.MinValue;
                }

                if ((icdoBenefitCalculation.termination_date == DateTime.MinValue) &&
                    (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
                    ldteTerminationDate = icdoBenefitCalculation.date_of_death;
                else
                    ldteTerminationDate = icdoBenefitCalculation.fas_termination_date;


                ldteTerminationDate = ldteTerminationDate.GetLastDayofMonth();

                DateTime ldteStartDate = (idteLastContributedDate == DateTime.MinValue) ? ldteTerminationDate : idteLastContributedDate;

                bool iblnIsProjectionApplicable = true;
                iblnIsProjectionApplicable = SetDualFASTerminationDate();

                int lintRTWPersonAccountID = 0;

                if ((icdoBenefitCalculation.is_rtw_less_than_2years_flag == busConstant.Flag_Yes) &&
                    (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper()))
                {
                    lintRTWPersonAccountID = ibusPersonAccount.icdoPersonAccount.person_account_id;
                }

                if (icdoBenefitCalculation.is_rtw_member_subsequent_retirement ||
                            icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                {
                    lclbFinalSalaryRecords = GetSalaryRecordsFAS2020(ldteStartDate,
                                                    ibusPersonAccount.icdoPersonAccount.person_id,
                                                    ibusPersonAccount.icdoPersonAccount.plan_id,
                                                    lintRTWPersonAccountID, ibusPersonAccount.icdoPersonAccount.person_account_id, true);
                }
                else
                {
                    lclbFinalSalaryRecords = GetSalaryRecordsFAS2020(ldteStartDate,
                                                  ibusPersonAccount.icdoPersonAccount.person_id,
                                                  ibusPersonAccount.icdoPersonAccount.plan_id,
                                                  lintRTWPersonAccountID, ibusPersonAccount.icdoPersonAccount.person_account_id);
                }

                if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimate ||
                    icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeEstimateSubsequent)
                {

                    if (!((icdoBenefitCalculation.is_return_to_work_member) &&
                         (icdoBenefitCalculation.rtw_refund_election_value == busConstant.Flag_Yes_Value.ToUpper())))
                    {
                        if (iblnIsProjectionApplicable)
                        {
                            Collection<busPersonAccountRetirementContribution> lclbProjectedSalaryRecords = new Collection<busPersonAccountRetirementContribution>();
                            lclbProjectedSalaryRecords = GetProjectedSalaryRecords(ibusPersonAccount.icdoPersonAccount.person_id,
                                                                                       ibusPersonAccount.icdoPersonAccount.plan_id,
                                                                                       (ldteTerminationDateUpdated == DateTime.MinValue) ? ldteTerminationDate : ldteTerminationDateUpdated,
                                                                                       icdoBenefitCalculation.salary_month_increase,
                                                                                       icdoBenefitCalculation.percentage_salary_increase, iobjPassInfo,
                                                                                       ibusPersonAccount.ibusPerson, idtbLastSalaryWithoutPersonAccount, true);
                            foreach (busPersonAccountRetirementContribution lobjProjSalaryRecord in lclbProjectedSalaryRecords)
                            {
                                lclbFinalSalaryRecords.Add(lobjProjSalaryRecord);
                            }

                            foreach (busPersonAccountRetirementContribution lobjProjectedSalary in lclbProjectedSalaryRecords)
                            {
                                busPersonAccountRetirementContribution lobjTempFinalSalary = new busPersonAccountRetirementContribution();

                                if (lclbFinalSalaryRecords.Any(i => i.icdoPersonAccountRetirementContribution.pay_period_year ==
                                lobjProjectedSalary.icdoPersonAccountRetirementContribution.pay_period_year &&
                                i.icdoPersonAccountRetirementContribution.pay_period_month ==
                                lobjProjectedSalary.icdoPersonAccountRetirementContribution.pay_period_month &&
                                i.icdoPersonAccountRetirementContribution.salary_amount == 0.0M))
                                {
                                    lobjTempFinalSalary = lclbFinalSalaryRecords.FirstOrDefault(i => i.icdoPersonAccountRetirementContribution.pay_period_year ==
                                     lobjProjectedSalary.icdoPersonAccountRetirementContribution.pay_period_year &&
                                     i.icdoPersonAccountRetirementContribution.pay_period_month ==
                                     lobjProjectedSalary.icdoPersonAccountRetirementContribution.pay_period_month &&
                                     i.icdoPersonAccountRetirementContribution.salary_amount == 0.0M);

                                    if (lobjTempFinalSalary.IsNotNull())
                                        lclbFinalSalaryRecords.Remove(lobjTempFinalSalary);
                                }
                            }
                        }
                    }
                }

                int iintCount = lclbFinalSalaryRecords.Count();

                lclbFinalSalaryRecords = GetSalaryRecordBlocksFAS2020(lclbFinalSalaryRecords);

                return iintCount > 36 && lclbFinalSalaryRecords.Count() < 36 ? true : false;
            }
            return false;
        }
        public bool IsMemberIsTFFRdual() => busNeoSpinBase.Select("cdoPersonTffrTiaaService.GetMemberTFFRdual", new object[1] { icdoBenefitCalculation.person_id }).Rows.Count > 0;

        //PIR 14288 - Added Validation in case Benefit Overpayment exists for RTW employee and there is no recovery associated with it. 
        public bool IsBenefitOverpayment()
        {
            bool iblnIsBenefitOverPayment = false;
            DataTable idtAllPayeeAccount = Select<cdoPayeeAccount>(new string[1] { "payee_perslink_id" }, new object[1] { icdoBenefitCalculation.person_id }, null, null);
            foreach (DataRow dr in idtAllPayeeAccount.Rows)
            {
                DataTable idtOverPaymentHdr = Select<cdoPaymentBenefitOverpaymentHeader>(new string[1] { "payee_account_id" }, new object[1] { dr["PAYEE_ACCOUNT_ID"] }, null, null);
                if (idtOverPaymentHdr.Rows.Count > 0)
                {
                    foreach (DataRow ldr in idtOverPaymentHdr.Rows)
                    {
                        DataTable ldtRecovery = Select<cdoPaymentRecovery>(new string[1] { "BENEFIT_OVERPAYMENT_ID" },
                                                                   new object[1] { ldr["BENEFIT_OVERPAYMENT_ID"] },
                                                                   null, null);
                        if (ldtRecovery.Rows.Count == 0)
                        {
                            iblnIsBenefitOverPayment = true;
                        }
                        else
                        {
                            foreach (DataRow ldrRecovery in ldtRecovery.Rows)
                            {
                                if (Convert.ToString(ldrRecovery["STATUS_VALUE"]) == busConstant.RecoveryStatusPendingApproval)
                                {
                                    iblnIsBenefitOverPayment = true;
                                }
                            }
                        }
                    }
                }
            }
            return iblnIsBenefitOverPayment;
        }
        /// <summary>
        /// PIR 18974 - Initiate BPM - If Benefit Overpayment exists for RTW employee and there is no recovery associated with it. 
        /// </summary>
        public void IsBenefitOverpaymentBPMInitiate()
        {
            if (IsBenefitOverpayment())
            {
                if (icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath ||
                    icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                {
                    busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Initialize_Process_PA_Death_Notification_Workflow, icdoBenefitCalculation.person_id, 0, 0, iobjPassInfo);
                }
                else
                {
                    busWorkflowHelper.InitiateBpmRequest(busConstant.Map_Process_MOU_Collection, icdoBenefitCalculation.person_id, 0, 0, iobjPassInfo);
                }
            }
        }

        //PIR 26088 Refund Application and calculation should have a red suppressable error warning for deal member
        public bool DualMemberExistSuppressibleWarning()
        {
            if (icdoBenefitCalculation.suppress_warnings_flag.IsNullOrEmpty() || icdoBenefitCalculation.suppress_warnings_flag == busConstant.Flag_No)
            {
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                if (ibusPersonAccount.ibusPerson.IsNull())
                    ibusPersonAccount.LoadPerson();

                ibusPersonAccount.ibusPerson.LoadRetirementAccount();
                if ((ibusPersonAccount.ibusPerson.iclbRetirementAccount.IsNotNull() && ibusPersonAccount.ibusPerson.iclbRetirementAccount.Count > 0) &&
                    ibusPersonAccount.ibusPerson.iclbRetirementAccount.Where(i => i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                                                            i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended).Count() > 1)
                {
                    return true;
                }              
                DataTable ldtResult = Select("cdoBenefitRefundApplication.LoadDualMemberSuppressWarning", new object[1] { icdoBenefitCalculation.person_id });
                if (ldtResult.Rows.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
