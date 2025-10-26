#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.CustomDataObjects;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using System.Linq;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busServicePurchaseDetail : busServicePurchaseDetailGen
    {
        private Collection<busServicePurchaseDetailConsolidated> _iclbServicePurchaseDetailConsolidated;
        public Collection<busServicePurchaseDetailConsolidated> iclbServicePurchaseDetailConsolidated
        {

            get
            {
                return _iclbServicePurchaseDetailConsolidated;
            }
            set
            {
                _iclbServicePurchaseDetailConsolidated = value;

            }
        }

        private Collection<busServicePurchaseDetailUserra> _iclbServicePurchaseDetailUserra;
        public Collection<busServicePurchaseDetailUserra> iclbServicePurchaseDetailUserra
        {
            get
            {
                return _iclbServicePurchaseDetailUserra;
            }
            set
            {
                _iclbServicePurchaseDetailUserra = value;
            }
        }

        private busServicePurchaseHeader _ibusServicePurchaseHeader;
        public busServicePurchaseHeader ibusServicePurchaseHeader
        {
            get
            {
                return _ibusServicePurchaseHeader;
            }
            set
            {
                _ibusServicePurchaseHeader = value;
            }
        }

        private cdoServiceCreditPlanFormulaRef iobjcdoServiceCreditPlanFormulaRef = null;

        public int iintTotalTimeToPurchaseInYears
        {
            get
            {
                return Convert.ToInt32(icdoServicePurchaseDetail.total_time_to_purchase / 12);
            }
        }

        //TODO: As of now, logic is same for all type of service purchases. this might change later.
        public decimal InitialPurchaseCost
        {
            get
            {
                return icdoServicePurchaseDetail.retirement_purchase_cost + icdoServicePurchaseDetail.rhic_purchase_cost;
            }
        }

        //This property is used for all the calculation (Benifit to be Purchased, ENRA etc..)
        //For Consolidated Purchase, we need to exclude Free Service before calculating the Amount. But, Free Service will go to Member PSC and VSC
        public decimal total_time_to_purchase_by_year
        {
            get
            {
                if ((ibusServicePurchaseHeader != null) && (ibusServicePurchaseHeader.icdoServicePurchaseHeader != null))
                {
                    if (ibusServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Consolidated_Purchase)
                    {
                        return icdoServicePurchaseDetail.total_time_to_purchase_exclude_free_service / 12;
                    }
                }
                return icdoServicePurchaseDetail.total_time_to_purchase / 12;
            }
        }

        //PIR 732
        //This property is used for all the calculation (ENRA etc..)
        //For Consolidated Purchase, we need to exclude Free Service before calculating the Amount. But, Free Service will go to Member PSC and VSC
        public decimal total_time_to_purchase_with_free_service_by_year
        {
            get
            {
                return icdoServicePurchaseDetail.total_time_to_purchase / 12;
            }
        }

        public decimal RoundedTotalTimeToPurchaseByYear
        {
            get
            {
                return busGlobalFunctions.RoundToPenny(total_time_to_purchase_by_year);
            }
        }

        private decimal _benefit_multiplier_current_and_purchased_service;
        public decimal benefit_multiplier_current_and_purchased_service
        {
            get
            {
                return _benefit_multiplier_current_and_purchased_service;
            }
            set
            {
                _benefit_multiplier_current_and_purchased_service = value;
            }
        }

        private decimal _benefit_multiplier_current_service;
        public decimal benefit_multiplier_current_service
        {
            get
            {
                return _benefit_multiplier_current_service;
            }
            set
            {
                _benefit_multiplier_current_service = value;
            }
        }

        public decimal increase_in_benifit_multiplier
        {
            get
            {
                return benefit_multiplier_current_and_purchased_service - benefit_multiplier_current_service;
            }
        }

        //This derived Property returns Total No of Years Service Credit Accumulated So far.
        public decimal current_years_of_service
        {
            get
            {
                if (ibusServicePurchaseHeader == null)
                    LoadServicePurchaseHeader();
                if (ibusServicePurchaseHeader.ibusPersonAccount == null)
                    ibusServicePurchaseHeader.LoadPersonAccount();
                return (icdoServicePurchaseDetail.vsc > 0) ? Convert.ToDecimal(icdoServicePurchaseDetail.vsc / 12) : 
                    Convert.ToDecimal(ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.Total_VSC / 12);
            }
        }
        public decimal current_years_of_service_with_service_rounded_to_nearst_month
        {
            get
            {//PIR20022:changes done for INPAYMENT logic
                decimal ldecMonthsOfService = (icdoServicePurchaseDetail.psc > 0) ?
                                              (icdoServicePurchaseDetail.psc) :
                                              (ibusServicePurchaseHeader.IsNotNull() && ibusServicePurchaseHeader.ibusPersonAccount.IsNotNull()) ?
                                              (ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.Total_PSC + ibusServicePurchaseHeader.idecAdditionalServicePSC) : 0;
                return ldecMonthsOfService / 12.0M;
            }
        }

        public decimal TotalYearsOfServicePlusPurchasedYearService
        {
            get
            {
                return current_years_of_service + total_time_to_purchase_by_year;
            }
        }

        private Collection<cdoBenefitProvisionMultiplier> _iclbBenefitMultiplierTierData;
        public Collection<cdoBenefitProvisionMultiplier> iclbBenefitMultiplierTierData
        {
            get
            {
                return _iclbBenefitMultiplierTierData;
            }
            set
            {
                _iclbBenefitMultiplierTierData = value;
            }

        }

        private decimal _tier1_percentage;
        public decimal tier1_percentage
        {
            get
            {
                return _tier1_percentage;
            }
            set
            {
                _tier1_percentage = value;
            }
        }

        private decimal _tier2_percentage;
        public decimal tier2_percentage
        {
            get
            {
                return _tier2_percentage;
            }
            set
            {
                _tier2_percentage = value;
            }
        }

        private decimal _tier3_percentage;
        public decimal tier3_percentage
        {
            get
            {
                return _tier3_percentage;
            }
            set
            {
                _tier3_percentage = value;
            }
        }

        private decimal _RHIC_Multiplier_Amount;
        public decimal RHIC_Multiplier_Amount
        {
            get
            {
                return _RHIC_Multiplier_Amount;
            }
            set
            {
                _RHIC_Multiplier_Amount = value;
            }
        }

        public decimal TotalYearsOfServicePlusFreePurchasedService
        {
            get
            {
                return current_years_of_service_with_service_rounded_to_nearst_month +
                    total_time_to_purchase_include_free_service_in_years -
                    total_time_to_purchase_exclude_free_service_in_years;
            }
        }

        public decimal tot_current_service_plus_free_purchase_in_months_psc
        {
            get
            {
                decimal lpsc = (icdoServicePurchaseDetail.psc > 0) ?
                                              Math.Ceiling(icdoServicePurchaseDetail.psc) : Math.Ceiling(ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.Total_PSC);
                return lpsc + TotalFreeServiceWithPurchase;
            }
        }
        public decimal total_time_to_purchase_include_free_service_in_years
        {
            get
            {
                return Convert.ToDecimal(icdoServicePurchaseDetail.total_time_to_purchase / 12.0M);
            }
        }

        public decimal total_time_to_purchase_exclude_free_service_in_years
        {
            get
            {
                return Convert.ToDecimal(icdoServicePurchaseDetail.total_time_to_purchase_exclude_free_service / 12.0M);
            }
        }


        public decimal TotalFreeServiceWithPurchase
        {
            get
            {
                return icdoServicePurchaseDetail.total_time_to_purchase - icdoServicePurchaseDetail.total_time_to_purchase_exclude_free_service;
            }
        }

        public decimal TotalYearsOfServicePlusAllPurchasedService
        {
            get
            {
                return current_years_of_service_with_service_rounded_to_nearst_month +
                    total_time_to_purchase_include_free_service_in_years;
            }
        }
        public decimal total_service_calculated_without_purchase
        {
            get
            {
                return ibusServicePurchaseHeader.IsNull() ? icdoServicePurchaseDetail.EarlierstNormalRetirementAgeWithoutServicePurchasedCustomRoundedToNearestYear +
                    TotalYearsOfServicePlusFreePurchasedService : icdoServicePurchaseDetail.EarlierstNormalRetirementAgeWithoutServicePurchasedCustomRoundedToNearestYear - ibusServicePurchaseHeader.icdoServicePurchaseHeader.current_age +
                    TotalYearsOfServicePlusFreePurchasedService;

            }
        }
        public decimal total_service_calculated_with_purchase
        {
            get
            {
                return ibusServicePurchaseHeader.IsNull() ? icdoServicePurchaseDetail.EarlierstNormalRetirementAgeCustomRoundedToNearestYear +
                    TotalYearsOfServicePlusAllPurchasedService : icdoServicePurchaseDetail.EarlierstNormalRetirementAgeCustomRoundedToNearestYear - ibusServicePurchaseHeader.icdoServicePurchaseHeader.current_age +
                    TotalYearsOfServicePlusAllPurchasedService;
            }
        }
        public decimal tiered_retirement_cost_without_purchase { get; set; }
        public decimal tiered_retirement_cost_with_purchase { get; set; }

        public void LoadBenefitMultiplierTierData()
        {
            if (_iclbBenefitMultiplierTierData == null)
                _iclbBenefitMultiplierTierData = new Collection<cdoBenefitProvisionMultiplier>();

            if (ibusServicePurchaseHeader == null)
                LoadServicePurchaseHeader();

            //Get the Tier Collection Bases on the Plan            

            if (ibusServicePurchaseHeader.ibusPersonAccount == null)
                ibusServicePurchaseHeader.LoadPersonAccount();

            if (ibusServicePurchaseHeader.ibusPersonAccount.ibusPlan == null)
                ibusServicePurchaseHeader.ibusPersonAccount.LoadPlan();

            int aintBenefitProvisionID = ibusServicePurchaseHeader.ibusPersonAccount.ibusPlan.icdoPlan.benefit_provision_id;
            string astrIsconversionRecord = "N";

            if (icdoServicePurchaseDetail.judges_conversion_flag == busConstant.Flag_Yes)
                astrIsconversionRecord = "Y";

            //PIR 24430 - Reverted changes made in PIR 23929 and added changes of PIR 23359.
            DateTime ldtEarlyStartDate = ibusServicePurchaseHeader.ibusPersonAccount.GetEarlyPlanParticiaptionStartDate();

            //PIR 25729
            if (ibusServicePurchaseHeader.IsNotNull() && ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase != DateTime.MinValue &&
                busGlobalFunctions.GetLatestBenefitProvisionTypeByEffectiveDate(ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase, 
				aintBenefitProvisionID) == busConstant.BenefitProvisionTypeRTDT)
            {
                ldtEarlyStartDate = ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase;
            }

            if (ldtEarlyStartDate == DateTime.MinValue)
                ldtEarlyStartDate = ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.start_date;

            _iclbBenefitMultiplierTierData = busPersonBase.LoadBenefitProvisionMultiplier(ldtEarlyStartDate,
                aintBenefitProvisionID, string.Empty, astrIsconversionRecord);
        }

        public void LoadTierPercentage()
        {
            if (_iclbBenefitMultiplierTierData == null)
                LoadBenefitMultiplierTierData();

            foreach (cdoBenefitProvisionMultiplier lobjBenefitProvisionMultiplier in _iclbBenefitMultiplierTierData)
            {
                if (lobjBenefitProvisionMultiplier.tier_number == 1)
                {
                    _tier1_percentage = Convert.ToDecimal(lobjBenefitProvisionMultiplier.multipier_percentage) / 100;
                }
                else if (lobjBenefitProvisionMultiplier.tier_number == 2)
                {
                    _tier2_percentage = Convert.ToDecimal(lobjBenefitProvisionMultiplier.multipier_percentage) / 100;
                }
                else if (lobjBenefitProvisionMultiplier.tier_number == 3)
                {
                    _tier3_percentage = Convert.ToDecimal(lobjBenefitProvisionMultiplier.multipier_percentage) / 100;
                }
            }
        }

        public void LoadRHICMultiplierData()
        {
            _RHIC_Multiplier_Amount = 0M;
            if (ibusServicePurchaseHeader.IsNull())
                LoadServicePurchaseHeader();
            DataTable ldtbResult = new DataTable();
            if ((ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id > 0) &&
                (ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase != DateTime.MinValue))
            {
				//PIR 14646 - Benefit Tier Changes
                if (ibusServicePurchaseHeader == null)
                    LoadServicePurchaseHeader();
                if (ibusServicePurchaseHeader.ibusPersonAccount == null)
                    ibusServicePurchaseHeader.LoadPersonAccount();
                string lstrBenefitTierValue = string.Empty;

                busPersonAccountRetirement lbusPersonAccountRetirement = new busPersonAccountRetirement();
                lbusPersonAccountRetirement.FindPersonAccountRetirement(ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.person_account_id);

                if (ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain)//PIR 20232
                {
                    lstrBenefitTierValue = string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) ? busConstant.MainBenefit1997Tier :
                        lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
                }
                //PIR 26282
                else if (ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf)
                {
                    lstrBenefitTierValue = string.IsNullOrEmpty(lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) ? busConstant.BCIBenefit2011Tier :
                        lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;
                }
                //PIR 26544
                else if (lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value.IsNotNullOrEmpty())
                    lstrBenefitTierValue = lbusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value;

                ldtbResult = busBase.Select("cdoBenefitProvisionBenefitType.GetBenefitProvisionByPlan", new object[4]{
                                                    ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id,
                                                    string.Empty,ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase, 
                                                        lstrBenefitTierValue});
            }
            if (ldtbResult.Rows.Count > 0)
            {
                _RHIC_Multiplier_Amount = Convert.ToDecimal(ldtbResult.Rows[0]["RHIC_SERVICE_FACTOR"]);
            }
        }

        private void CalculateBenefitMultiplierCurrentAndPurchasedService()
        {
            if (ibusServicePurchaseHeader == null)
                LoadServicePurchaseHeader();

            if (_iclbBenefitMultiplierTierData == null)
                LoadBenefitMultiplierTierData();

            if (_tier1_percentage == 0)
                LoadTierPercentage();

            benefit_multiplier_current_and_purchased_service = 0M;

            decimal ldecTier1Amount = 0M;
            decimal ldecTier2Amount = 0M;
            decimal ldecTier3Amount = 0M;

            if (icdoServicePurchaseDetail.judges_conversion_flag == busConstant.Flag_Yes)
            {
                if (TotalYearsOfServicePlusPurchasedYearService > 10)
                {
                    ldecTier1Amount = _tier1_percentage * 10;
                    ldecTier2Amount = _tier2_percentage * (TotalYearsOfServicePlusPurchasedYearService - 10);
                    benefit_multiplier_current_and_purchased_service = ldecTier1Amount + ldecTier2Amount;
                }
                else
                {
                    benefit_multiplier_current_and_purchased_service = _tier1_percentage * TotalYearsOfServicePlusPurchasedYearService;
                }
            }
            else
            {
                //Get the Tier Data based on the Plan
                switch (ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id.ToString())
                {
                    case busConstant.Plan_ID_Judges:
                        if (TotalYearsOfServicePlusPurchasedYearService > 20)
                        {
                            ldecTier1Amount = _tier1_percentage * 10;
                            ldecTier2Amount = _tier2_percentage * 10;
                            ldecTier3Amount = _tier3_percentage * (TotalYearsOfServicePlusPurchasedYearService - 20);
                            benefit_multiplier_current_and_purchased_service = ldecTier1Amount + ldecTier2Amount + ldecTier3Amount;
                        }
                        else if (TotalYearsOfServicePlusPurchasedYearService > 10)
                        {
                            ldecTier1Amount = _tier1_percentage * 10;
                            ldecTier2Amount = _tier2_percentage * (TotalYearsOfServicePlusPurchasedYearService - 10);
                            benefit_multiplier_current_and_purchased_service = ldecTier1Amount + ldecTier2Amount;
                        }
                        else
                        {
                            benefit_multiplier_current_and_purchased_service = _tier1_percentage * TotalYearsOfServicePlusPurchasedYearService;
                        }
                        break;
                    case busConstant.Plan_ID_Highway_Patrol:
                        if (TotalYearsOfServicePlusPurchasedYearService > 25)
                        {
                            ldecTier1Amount = _tier1_percentage * 25;
                            ldecTier2Amount = _tier2_percentage * (TotalYearsOfServicePlusPurchasedYearService - 25);
                            benefit_multiplier_current_and_purchased_service = ldecTier1Amount + ldecTier2Amount;
                        }
                        else
                        {
                            benefit_multiplier_current_and_purchased_service = _tier1_percentage * TotalYearsOfServicePlusPurchasedYearService;
                        }
                        break;
                    case busConstant.Plan_ID_Job_Service:
                        if (TotalYearsOfServicePlusPurchasedYearService > 10)
                        {
                            ldecTier1Amount = _tier1_percentage * 5;
                            ldecTier2Amount = _tier2_percentage * 5;
                            ldecTier3Amount = _tier3_percentage * (TotalYearsOfServicePlusPurchasedYearService - 10);
                            benefit_multiplier_current_and_purchased_service = ldecTier1Amount + ldecTier2Amount + ldecTier3Amount;
                        }
                        else if (TotalYearsOfServicePlusPurchasedYearService > 5)
                        {
                            ldecTier1Amount = _tier1_percentage * 5;
                            ldecTier2Amount = _tier2_percentage * (TotalYearsOfServicePlusPurchasedYearService - 5);
                            benefit_multiplier_current_and_purchased_service = ldecTier1Amount + ldecTier2Amount;
                        }
                        else
                        {
                            benefit_multiplier_current_and_purchased_service = _tier1_percentage * TotalYearsOfServicePlusPurchasedYearService;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void CalculateBenefitMultiplierCurrentService()
        {
            if (ibusServicePurchaseHeader == null)
                LoadServicePurchaseHeader();

            if (_iclbBenefitMultiplierTierData == null)
                LoadBenefitMultiplierTierData();

            if (_tier1_percentage == 0)
                LoadTierPercentage();

            benefit_multiplier_current_service = 0M;

            decimal ldecTier1Amount = 0M;
            decimal ldecTier2Amount = 0M;
            decimal ldecTier3Amount = 0M;

            if (icdoServicePurchaseDetail.judges_conversion_flag == busConstant.Flag_Yes)
            {
                if (current_years_of_service > 10)
                {
                    ldecTier1Amount = _tier1_percentage * 10;
                    ldecTier2Amount = _tier2_percentage * (current_years_of_service - 10);
                    benefit_multiplier_current_service = ldecTier1Amount + ldecTier2Amount;
                }
                else
                {
                    benefit_multiplier_current_service = _tier1_percentage * current_years_of_service;
                }
            }
            else
            {
                //Get the Tier Data based on the Plan
                switch (ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id.ToString())
                {
                    case busConstant.Plan_ID_Judges:
                        if (current_years_of_service > 20)
                        {
                            ldecTier1Amount = _tier1_percentage * 10;
                            ldecTier2Amount = _tier2_percentage * 10;
                            ldecTier3Amount = _tier3_percentage * (current_years_of_service - 20);
                            benefit_multiplier_current_service = ldecTier1Amount + ldecTier2Amount + ldecTier3Amount;
                        }
                        else if (current_years_of_service > 10)
                        {
                            ldecTier1Amount = _tier1_percentage * 10;
                            ldecTier2Amount = _tier2_percentage * (current_years_of_service - 10);
                            benefit_multiplier_current_service = ldecTier1Amount + ldecTier2Amount;
                        }
                        else
                        {
                            benefit_multiplier_current_service = _tier1_percentage * current_years_of_service;
                        }
                        break;
                    case busConstant.Plan_ID_Highway_Patrol:
                        if (current_years_of_service > 25)
                        {
                            ldecTier1Amount = _tier1_percentage * 25;

                            ldecTier2Amount = _tier2_percentage * (current_years_of_service - 25);
                            benefit_multiplier_current_service = ldecTier1Amount + ldecTier2Amount;
                        }
                        else
                        {
                            benefit_multiplier_current_service = _tier1_percentage * current_years_of_service;
                        }
                        break;
                    case busConstant.Plan_ID_Job_Service:
                        if (current_years_of_service > 10)
                        {
                            ldecTier1Amount = _tier1_percentage * 5;
                            ldecTier2Amount = _tier2_percentage * 5;
                            ldecTier3Amount = _tier3_percentage * (current_years_of_service - 10);
                            benefit_multiplier_current_service = ldecTier1Amount + ldecTier2Amount + ldecTier3Amount;
                        }
                        else if (current_years_of_service > 10)
                        {
                            ldecTier1Amount = _tier1_percentage * 5;
                            ldecTier2Amount = _tier2_percentage * (current_years_of_service - 5);
                            benefit_multiplier_current_service = ldecTier1Amount + ldecTier2Amount;
                        }
                        else
                        {
                            benefit_multiplier_current_service = _tier1_percentage * current_years_of_service;
                        }
                        break;
                    default:
                        break;

                }
            }
        }

        public bool iblnHeaderValidating = false;

        public DateTime idteNewCalcEffDate
        {
            get
            {
                return new DateTime(2018, 01, 01);
            }
        }

        public void RecomputeCalculatedFields()
        {
            // Do different calculations based on whether Consolidated/USERRA/Sick Leave has been selected
            if (ibusServicePurchaseHeader == null)
            {
                LoadServicePurchaseHeader();
            }

            if (ibusServicePurchaseHeader.ibusPersonAccount == null)
                ibusServicePurchaseHeader.LoadPersonAccount();
            switch (ibusServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value)
            {
                case busConstant.Service_Purchase_Type_Consolidated_Purchase:
                    CalculateTimetoPurchaseForConsolidated();
                    CalculateTotalRefundPlusInterest();
                    PopulateServiceCreditPlanFormula();
                    if (ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase.Date < idteNewCalcEffDate.Date || ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id == busConstant.PlanIdJobService)
                    {
                        CalculateBenefitToBePurchased();
                        CalculateEarliestNormalRetirementAge();
                        CalculateRetirementAndRHICActuarialFactor();
                    }
                    else
                    {
                        CalculateEarliestNormalRetirementAge(); //NRD with purchase
                        CalculateEarliestNormalRetirementAge(true); //NRD without purchase
                        LoadEEContributionRate();
                        CalculateRetirementAndRHICActuarialFactor();
                        CalculateRetirementAndRHICActuarialFactor(true);
                        LoadTieredRetirementCostWithAndWithoutPurchase();
                    }
                    CalculatePurchaseCost();
                    CalculateTotalPurchaseCost();
                    break;
                case busConstant.Service_Purchase_Type_Unused_Sick_Leave:
                    ibusServicePurchaseHeader.btnRecalculate_Click();
                    break;
                case busConstant.Service_Purchase_Type_USERRA_Military_Service:
                    LoadServicePurchaseDetailUSERRA();
                    RecomputeUSERRADetails();
                    CalculateTimetoPurchaseForUSERRA();
                    CalculatePurchaseCostForUSERRA();
                    CalculateTotalPurchaseCostForUSERRA();
                    break;
                default:
                    break;
            }
        }

        private void LoadTieredRetirementCostWithAndWithoutPurchase()
        {
            LoadTierPercentage();
            ibusServicePurchaseHeader.LoadPlan();
            switch (ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_code)
            {
                case busConstant.Plan_Code_Main:
                case busConstant.Plan_Code_National_Guard:
                case busConstant.Plan_Code_LE_With_Prior_Service:
                case busConstant.Plan_Code_LE_Without_Prior_Service:
                case busConstant.Plan_Code_Defined_Contribution:
                case busConstant.Plan_Code_BCI_Law_Enforcement:
                case busConstant.Plan_Code_Main_2020: //PIR 20232
                case busConstant.Plan_Code_Defined_Contribution_2020: //PIR 20232
                case busConstant.Plan_Code_State_Public_Safety: //PIR 25729
                    tiered_retirement_cost_with_purchase = _tier1_percentage * total_service_calculated_with_purchase;
                    tiered_retirement_cost_without_purchase = _tier1_percentage * total_service_calculated_without_purchase;
                    break;
                case busConstant.Plan_Code_Judges:
                    tiered_retirement_cost_with_purchase = (total_service_calculated_with_purchase > 20) ?
                        ((_tier1_percentage * 10) + (_tier2_percentage * 10) + (_tier3_percentage * (total_service_calculated_with_purchase - 20))) :
                        (total_service_calculated_with_purchase > 10) ? ((_tier1_percentage * 10) + (_tier2_percentage * (total_service_calculated_with_purchase - 10))) : (_tier1_percentage * total_service_calculated_with_purchase);
                    tiered_retirement_cost_without_purchase = (total_service_calculated_without_purchase > 20) ?
                        ((_tier1_percentage * 10) + (_tier2_percentage * 10) + (_tier3_percentage * (total_service_calculated_without_purchase - 20))) :
                        (total_service_calculated_without_purchase > 10) ? ((_tier1_percentage * 10) + (_tier2_percentage * (total_service_calculated_without_purchase - 10))) : (_tier1_percentage * total_service_calculated_without_purchase);
                    break;
                case busConstant.Plan_Code_Highway_Patrol:
                    tiered_retirement_cost_with_purchase = (total_service_calculated_with_purchase > 25) ?
                        ((_tier1_percentage * 25) + (_tier2_percentage * (total_service_calculated_with_purchase - 25))) : 
                        (_tier1_percentage * total_service_calculated_with_purchase);
                    tiered_retirement_cost_without_purchase = (total_service_calculated_without_purchase > 25) ?
                        ((_tier1_percentage * 25) + (_tier2_percentage * (total_service_calculated_without_purchase - 25))) :
                        (_tier1_percentage * total_service_calculated_without_purchase);
                    break;
                default:
                    tiered_retirement_cost_with_purchase = 0;
                    tiered_retirement_cost_without_purchase = 0;
                    break;
            }
        }

        private void LoadEEContributionRate()
        {
            if (!string.IsNullOrEmpty(ibusServicePurchaseHeader.icdoServicePurchaseHeader.member_type_value))
            {
                icdoServicePurchaseDetail.employee_contribution_rate = 0.0M;
                DataTable ldtbList = SelectWithOperator<cdoPlanRetirementRate>(
                                              new String[3] { enmPlanRetirementRate.member_type_value.ToString(), enmPlanRetirementRate.plan_id.ToString(), enmPlanRetirementRate.plan_id.ToString() },
                                              new String[3] { "=", "=", "<=" },
                                              new object[3] { ibusServicePurchaseHeader.icdoServicePurchaseHeader.member_type_value, ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id, ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase },
                                              "effective_date desc");
                if (ldtbList.Rows.Count > 0)
                {
                    cdoPlanRetirementRate lcdoPlanRetirementRate = new cdoPlanRetirementRate();
                    lcdoPlanRetirementRate.LoadData(ldtbList.Rows[0]);
                    icdoServicePurchaseDetail.employee_contribution_rate = lcdoPlanRetirementRate.ee_pre_tax + lcdoPlanRetirementRate.ee_post_tax +
                                                                           lcdoPlanRetirementRate.ee_emp_pickup + lcdoPlanRetirementRate.ee_rhic;
                    if (ibusServicePurchaseHeader.ibusPerson == null)
                        ibusServicePurchaseHeader.LoadPerson();
                    if (!string.IsNullOrEmpty(ibusServicePurchaseHeader.ibusPerson.icdoPerson.db_addl_contrib) &&
                        ibusServicePurchaseHeader.ibusPerson.icdoPerson.db_addl_contrib.ToUpper() == busConstant.Flag_Yes)
                    {
                        icdoServicePurchaseDetail.employee_contribution_rate += lcdoPlanRetirementRate.addl_ee_pre_tax + 
                                                                                lcdoPlanRetirementRate.addl_ee_post_tax +
                                                                                lcdoPlanRetirementRate.addl_ee_emp_pickup;
                    }
                    icdoServicePurchaseDetail.employee_contribution_rate = icdoServicePurchaseDetail.employee_contribution_rate / 100.0M;
                }
            }
        }

        public void CalculateTotalPurchaseCostForUSERRA()
        {
            icdoServicePurchaseDetail.total_purchase_cost = icdoServicePurchaseDetail.retirement_purchase_cost +
                                                            icdoServicePurchaseDetail.rhic_purchase_cost;

        }

        public void CalculateTotalPurchaseCostForUnUsedSickLeave()
        {
            icdoServicePurchaseDetail.total_purchase_cost = icdoServicePurchaseDetail.retirement_purchase_cost +
                                                            icdoServicePurchaseDetail.rhic_purchase_cost;

        }

        public void CalculateRetirementandRHICCostForUnUsedSickLeave()
        {

            if (ibusServicePurchaseHeader.ibusPlan == null)
            {
                ibusServicePurchaseHeader.LoadPlan();
            }

            DateTime ldteEffectiveDate = ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase;
            if (ibusServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_Unused_Sick_Leave &&
                icdoServicePurchaseDetail.termination_date != DateTime.MinValue) // PROD PIR ID 7764
                ldteEffectiveDate = icdoServicePurchaseDetail.termination_date;

            cdoPlanRetirementRate lobjcdoPlanRetirementRate = busGlobalFunctions.GetRetirementRateForPlanDateCombination(
                                                                    ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_id, ldteEffectiveDate, 
                                                                    ibusServicePurchaseHeader.icdoServicePurchaseHeader.member_type_value);

            //TODO check with Jeeva whether the value will be stored as /100 or multiplied by 100
            // TODO check with Jeeva whether the calculation should be done for only specific plans ?? as mentioned in the Use Case

            if (lobjcdoPlanRetirementRate != null && lobjcdoPlanRetirementRate.plan_rate_id > 0)
            {

                decimal ldecEmployeeRate = ((lobjcdoPlanRetirementRate.ee_pre_tax / 100) +
                                            (lobjcdoPlanRetirementRate.ee_post_tax / 100) +
                                            (lobjcdoPlanRetirementRate.ee_emp_pickup / 100));
                //PIR - 14656 start - Add condition to methods calculating Contributions based on Eligible Salary (Employer Report, ESS template, Service Purchase)
                if (ibusServicePurchaseHeader.ibusPerson.IsNull()) ibusServicePurchaseHeader.LoadPerson();
                if(!string.IsNullOrEmpty(ibusServicePurchaseHeader.ibusPerson.icdoPerson.db_addl_contrib) && 
                    ibusServicePurchaseHeader.ibusPerson.icdoPerson.db_addl_contrib.ToUpper() == busConstant.Flag_Yes)
                {
                    ldecEmployeeRate = (((lobjcdoPlanRetirementRate.ee_pre_tax + lobjcdoPlanRetirementRate.addl_ee_pre_tax) / 100) +
                                            ((lobjcdoPlanRetirementRate.ee_post_tax + lobjcdoPlanRetirementRate.addl_ee_post_tax) / 100) +
                                            ((lobjcdoPlanRetirementRate.ee_emp_pickup + lobjcdoPlanRetirementRate.addl_ee_emp_pickup) / 100));
                }
                //PIR - 14656 End - Add condition to methods calculating Contributions based on Eligible Salary (Employer Report, ESS template, Service Purchase)


                decimal ldecEmployerRate = lobjcdoPlanRetirementRate.er_post_tax / 100;

                decimal ldecCombinedEmployeeEmployerRate = ldecEmployeeRate + ldecEmployerRate;

                decimal ldecRHICRate = ((lobjcdoPlanRetirementRate.ee_rhic / 100) +

                                        (lobjcdoPlanRetirementRate.er_rhic / 100));
        
                icdoServicePurchaseDetail.retirement_purchase_cost =
                    Math.Round(ibusServicePurchaseHeader.icdoServicePurchaseHeader.idecUserFASSalary * icdoServicePurchaseDetail.RoundedTotalTimeOfPurchaseForSickLeave * ldecCombinedEmployeeEmployerRate, 2, MidpointRounding.AwayFromZero);

                icdoServicePurchaseDetail.retirement_cost_for_sick_leave_purchase = icdoServicePurchaseDetail.retirement_purchase_cost;

                icdoServicePurchaseDetail.rhic_purchase_cost =
                    Math.Round(ibusServicePurchaseHeader.icdoServicePurchaseHeader.idecUserFASSalary * icdoServicePurchaseDetail.RoundedTotalTimeOfPurchaseForSickLeave * ldecRHICRate, 2, MidpointRounding.AwayFromZero);

                icdoServicePurchaseDetail.rhic_cost_for_sick_leave_purchase = icdoServicePurchaseDetail.rhic_purchase_cost;
            }

        }

        public void CalculateTimeToPurchaseForUnUsedSickLeave()
        {
            if (icdoServicePurchaseDetail.unused_sick_leave_hours > 0)
            {
                icdoServicePurchaseDetail.total_time_to_purchase = Math.Ceiling(Convert.ToDecimal(icdoServicePurchaseDetail.unused_sick_leave_hours /
                                                                   GetSickLeavePurchaseDivisionValue()));
            }
        }

        private decimal GetSickLeavePurchaseDivisionValue()
        {
            decimal ldecSickLeaveTimeToPurchase = 0;
            string lstrData1Value = busGlobalFunctions.GetData1ByCodeValue(
                busConstant.SystemConstantsAndVariablesCodeID, busConstant.Service_Purchase_Sick_Leave_Time_To_Purchase, iobjPassInfo);

            if (!String.IsNullOrEmpty(lstrData1Value))
            {
                ldecSickLeaveTimeToPurchase = Convert.ToDecimal(lstrData1Value);
            }
            return ldecSickLeaveTimeToPurchase;
        }

        private void CalculatePurchaseCost()
        {
            if (iobjcdoServiceCreditPlanFormulaRef != null && iobjcdoServiceCreditPlanFormulaRef.service_credit_plan_formula_ref_id > 0)
            {
                switch (ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_code)
                {
                    case busConstant.Plan_Code_Main:
                    case busConstant.Plan_Code_Judges:
                    case busConstant.Plan_Code_Highway_Patrol:
                    case busConstant.Plan_Code_National_Guard:
                    case busConstant.Plan_Code_LE_With_Prior_Service:
                    case busConstant.Plan_Code_LE_Without_Prior_Service:
                    case busConstant.Plan_Code_Defined_Contribution:
                    case busConstant.Plan_Code_BCI_Law_Enforcement: //pir 7943
                    case busConstant.Plan_Code_Main_2020: //PIR 20232
                    case busConstant.Plan_Code_Defined_Contribution_2020: //PIR 20232
                    case busConstant.Plan_Code_State_Public_Safety: //PIR 25729
                        if (ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase.Date < idteNewCalcEffDate.Date)
                        {
                            icdoServicePurchaseDetail.retirement_purchase_cost = Math.Round(icdoServicePurchaseDetail.retirement_benefit_purchased *
                                                                                            icdoServicePurchaseDetail.retirement_actuarial_factor, 2, MidpointRounding.AwayFromZero);
                            icdoServicePurchaseDetail.rhic_purchase_cost = Math.Round(icdoServicePurchaseDetail.rhic_benefit_purchased * icdoServicePurchaseDetail.rhic_retirement_factor,
                                                                                        2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            LoadRetirementRhicCostWithAndWithoutPurchase();
                        }
                        break;
                    case busConstant.Plan_Code_Job_Service:
                        icdoServicePurchaseDetail.retirement_purchase_cost = Math.Round(icdoServicePurchaseDetail.retirement_benefit_purchased *
                                                                                        icdoServicePurchaseDetail.retirement_actuarial_factor, 2, MidpointRounding.AwayFromZero);
                        break;
                    default:
                        break;
                }
            }
        }
        private void LoadRetirementRhicCostWithAndWithoutPurchase()
        {
            // Calculation of Retirement Purchase Cost Starts Here
            /*Intermediate Retirement Cost 1 = FAS * 
                                            Accrual Rate * 
                                            (NRD excluding service to be purchased - Member Current Age + Member Current Service) * 
                                            Service Credit Actuarial Factor without considering service to be purchased */
            if (icdoServicePurchaseDetail.total_time_to_purchase_exclude_free_service > 0)
            {
                if (ibusServicePurchaseHeader.IsNull()) LoadServicePurchaseHeader();
                decimal ldecFAS = ibusServicePurchaseHeader.icdoServicePurchaseHeader.idecUserFASSalary;
                icdoServicePurchaseDetail.ret_cost_without_purchase = Math.Round((ldecFAS * tiered_retirement_cost_without_purchase *
                                                     icdoServicePurchaseDetail.retirement_actuarial_factor), 2, MidpointRounding.AwayFromZero);
                /*Intermediate Retirement Cost 2 = FAS * 
                                                Accrual Rate * 
                                                (NRD excluding service to be purchased - Member Current Age + Member Current Service + service to be purchased) * 
                                                Service Credit Actuarial Factor with considering service to be purchased */
                icdoServicePurchaseDetail.ret_cost_with_purchase = Math.Round((ldecFAS * tiered_retirement_cost_with_purchase  *
                                                     icdoServicePurchaseDetail.ret_act_factor_with_pur), 2, MidpointRounding.AwayFromZero);
                decimal ldecIntermediateRetCostDifference = icdoServicePurchaseDetail.ret_cost_with_purchase - icdoServicePurchaseDetail.ret_cost_without_purchase;
                /*Intermediate Future EE Contributions 1 = FAS * employee_contribution_rate * future_ee_factor without purchase
                 */
                icdoServicePurchaseDetail.fut_emp_cost_without_pur = Math.Round((ldecFAS * (icdoServicePurchaseDetail.employee_contribution_rate) * icdoServicePurchaseDetail.fut_ee_act_factor_without_pur), 2, MidpointRounding.AwayFromZero);
                /*Intermediate Future EE Contributions 2 = FAS * employee_contribution_rate * future_ee_factor with purchase
                 */
                icdoServicePurchaseDetail.fut_emp_cost_with_pur = Math.Round((ldecFAS * (icdoServicePurchaseDetail.employee_contribution_rate) * icdoServicePurchaseDetail.fut_ee_act_factor_with_pur), 2, MidpointRounding.AwayFromZero);
                decimal ldecFutureEEContributionDifference = Math.Abs(icdoServicePurchaseDetail.fut_emp_cost_with_pur - icdoServicePurchaseDetail.fut_emp_cost_without_pur);
                icdoServicePurchaseDetail.retirement_purchase_cost = ldecIntermediateRetCostDifference + ldecFutureEEContributionDifference;
                // Calculation of Retirement Purchase Cost Ends Here
                //Calculation of RHIC purchase cost starts here
                /* Intermediate RHIC Cost 1 = (NRD excluding service to be purchased - Member current Age + Member current Service (in years)) * RHIC multiplier * 
                 *                              Service Purchase RHIC Actuarial Factor without Purchase*/
                if (_RHIC_Multiplier_Amount == 0)
                    LoadRHICMultiplierData();
                icdoServicePurchaseDetail.rhic_cost_without_purchase = Math.Round((((icdoServicePurchaseDetail.EarlierstNormalRetirementAgeWithoutServicePurchasedCustomRoundedToNearestYear -
                                                    ibusServicePurchaseHeader.icdoServicePurchaseHeader.current_age + TotalYearsOfServicePlusFreePurchasedService)) *
                                                     _RHIC_Multiplier_Amount * icdoServicePurchaseDetail.rhic_retirement_factor), 2, MidpointRounding.AwayFromZero);
                /* Intermediate RHIC Cost 2 = (NRD excluding service to be purchased - Member current Age + Member current Service (in years) + Service to be purchased (in years)) * RHIC multiplier * 
                 *                              Service Purchase RHIC Actuarial Factor without Purchase*/
                icdoServicePurchaseDetail.rhic_cost_with_purchase = Math.Round(((/*busGlobalFunctions.Round*/(icdoServicePurchaseDetail.EarlierstNormalRetirementAgeCustomRoundedToNearestYear -
                                                    ibusServicePurchaseHeader.icdoServicePurchaseHeader.current_age + TotalYearsOfServicePlusAllPurchasedService)) *
                                                     _RHIC_Multiplier_Amount * icdoServicePurchaseDetail.rhic_ret_factor_with_pur), 2, MidpointRounding.AwayFromZero);
                icdoServicePurchaseDetail.rhic_purchase_cost = icdoServicePurchaseDetail.rhic_cost_with_purchase - icdoServicePurchaseDetail.rhic_cost_without_purchase;
            }
        }
        private void CalculateTotalPurchaseCost()
        {
            if (iobjcdoServiceCreditPlanFormulaRef != null && iobjcdoServiceCreditPlanFormulaRef.service_credit_plan_formula_ref_id > 0)
            {
                switch (ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_code)
                {
                    case busConstant.Plan_Code_Job_Service:
                    case busConstant.Plan_Code_LE_Without_Prior_Service:
                    case busConstant.Plan_Code_LE_With_Prior_Service:
                    case busConstant.Plan_Code_National_Guard:
                    case busConstant.Plan_Code_Highway_Patrol:
                    case busConstant.Plan_Code_Judges:
                    case busConstant.Plan_Code_Main:
                    case busConstant.Plan_Code_Defined_Contribution:
                    case busConstant.Plan_Code_BCI_Law_Enforcement: //pir 7943
                    case busConstant.Plan_Code_Defined_Contribution_2020: //PIR 20232
                    case busConstant.Plan_Code_Main_2020: //PIR 20232
                    case busConstant.Plan_Code_State_Public_Safety: //PIR 25729
                        icdoServicePurchaseDetail.total_purchase_cost = icdoServicePurchaseDetail.retirement_purchase_cost +
                                                                        icdoServicePurchaseDetail.rhic_purchase_cost;
                        break;
                    default:
                        break;
                }
            }
        }

        private void CalculatePurchaseCostForUSERRA()
        {
            icdoServicePurchaseDetail.retirement_purchase_cost = 0;
            icdoServicePurchaseDetail.rhic_purchase_cost = 0;
            if (iclbServicePurchaseDetailUserra != null)
            {
                foreach (busServicePurchaseDetailUserra lobjServicePurchaseDetailUserra in _iclbServicePurchaseDetailUserra)
                {
                    icdoServicePurchaseDetail.retirement_purchase_cost +=
                        lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.employee_contribution +
                        lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.employer_contribution +
                        lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.employee_pickup;        // Service Purchase Backlog PIR-10003 (PIR-11115)

                    icdoServicePurchaseDetail.rhic_purchase_cost +=
                        lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.rhic_contribution;
                }
            }
        }

        private void CalculateTimetoPurchaseForUSERRA()
        {
            int lintTotalTimeToPurchase = 0;
            int lintTotalContributionMonths = 0;

            if (ibusServicePurchaseHeader == null)
                LoadServicePurchaseHeader();

            //PIR 914 - If Grant Free Flag Checked, Time to purchase to should be calculated by Active Duty Start Date and End Date
            if (ibusServicePurchaseHeader.icdoServicePurchaseHeader.grant_free_flag == busConstant.Flag_Yes)
            {
                DateTime ldtDateFrom = DateTime.MinValue;
                DateTime ldtDateTo = DateTime.MinValue;

                if ((icdoServicePurchaseDetail.userra_active_duty_start_date > DateTime.MinValue) &&
                (icdoServicePurchaseDetail.userra_active_duty_end_date > DateTime.MinValue))
                {
                    int lintDiffInMonths;
                    int lintDiffInYears;

                    ldtDateFrom = new DateTime(icdoServicePurchaseDetail.userra_active_duty_start_date.Year,
                                         icdoServicePurchaseDetail.userra_active_duty_start_date.Month, 1);

                    ldtDateTo = new DateTime(icdoServicePurchaseDetail.userra_active_duty_end_date.Year,
                                     icdoServicePurchaseDetail.userra_active_duty_end_date.Month, 1);

                    lintTotalTimeToPurchase = HelperFunction.GetMonthSpan(ldtDateFrom, ldtDateTo, out lintDiffInYears, out lintDiffInMonths) + 1;

                    //If the Contribution already made for specific months, exclude it                    
                    while (ldtDateTo >= ldtDateFrom)
                    {
                        if (GetPSCForMonthYear(ldtDateTo.Month, ldtDateTo.Year) > 0)
                        {
                            lintTotalTimeToPurchase--;
                            lintTotalContributionMonths++;
                        }
                        ldtDateTo = ldtDateTo.AddMonths(-1);
                    }
                }
            }
            else
            {
                if (iclbServicePurchaseDetailUserra != null)
                {
                    lintTotalTimeToPurchase = iclbServicePurchaseDetailUserra.Count;
                }

                //Exclude the Overlapping Months of Service Credit - BR-33
                foreach (busServicePurchaseDetailUserra lobjServicePurchaseDetailUserra in iclbServicePurchaseDetailUserra)
                {
                    if (
                        GetPSCForMonthYear(
                            lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.missed_salary_month.Month,
                            lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.missed_salary_month.Year) > 0)
                    {
                        lintTotalTimeToPurchase--;
                        lintTotalContributionMonths++;
                    }
                }
            }
            icdoServicePurchaseDetail.total_time_to_purchase = lintTotalTimeToPurchase;
            icdoServicePurchaseDetail.total_time_to_purchase_contribution_months = lintTotalContributionMonths;
        }

        public decimal GetPSCForMonthYear(int aintMonth, int aintYear)
        {
            if (ibusServicePurchaseHeader == null)
                LoadServicePurchaseHeader();
            if (ibusServicePurchaseHeader.ibusPersonAccount == null)
                ibusServicePurchaseHeader.LoadPersonAccount();

            if (ibusServicePurchaseHeader.ibusPersonAccount.iclbRetirementContributionAll == null)
                ibusServicePurchaseHeader.ibusPersonAccount.LoadRetirementContributionAll();

            decimal ldecPSC = 0.00M;
            foreach (busPersonAccountRetirementContribution lobjRetContribution in ibusServicePurchaseHeader.ibusPersonAccount.iclbRetirementContributionAll)
            {
                if ((lobjRetContribution.icdoPersonAccountRetirementContribution.effective_date.Year == aintYear) &&
                    (lobjRetContribution.icdoPersonAccountRetirementContribution.effective_date.Month == aintMonth))
                {
                    ldecPSC += lobjRetContribution.icdoPersonAccountRetirementContribution.pension_service_credit;
                }
            }
            return ldecPSC;
        }

        //This method will update all the Contribution Data in Userra Detail. This will be called on Every Save Click.
        private void RecomputeUSERRADetails()
        {
            if (iclbServicePurchaseDetailUserra != null)
            {
                foreach (busServicePurchaseDetailUserra lobjServicePurchaseDetailUserra in iclbServicePurchaseDetailUserra)
                {
                    lobjServicePurchaseDetailUserra.CalculateContributionAmounts();
                    lobjServicePurchaseDetailUserra.icdoServicePurchaseDetailUserra.Update();
                }
            }
        }

        private void CalculateRetirementAndRHICActuarialFactor(bool ablnWithOrWithoutPurchase = false)
        {
            if (iobjcdoServiceCreditPlanFormulaRef != null && iobjcdoServiceCreditPlanFormulaRef.service_credit_plan_formula_ref_id > 0)
            {
                //Other than Job Service Plan, Retirment Age will be Earliest Normal Retirment Age
                //For Job Service Plan, System should calculate based on Projected Retirment Age

                int lintRetirmentAge = (ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase.Date < idteNewCalcEffDate.Date) ?
                                    icdoServicePurchaseDetail.EarlierstNormalRetirementAgeRoundedToNearestYear : (ablnWithOrWithoutPurchase) ?
                                    icdoServicePurchaseDetail.EarlierstNormalRetirementAgeCustomRoundedToNearestYear :
                                    icdoServicePurchaseDetail.EarlierstNormalRetirementAgeWithoutServicePurchasedCustomRoundedToNearestYear;
                int lintMemberAge = ibusServicePurchaseHeader.icdoServicePurchaseHeader.CurrentAgeRoundedToNearestYear;

                if (ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id == busConstant.PlanIdJobService)
                {
                    CalculateProjectedRetirmentAge();
                    lintRetirmentAge = icdoServicePurchaseDetail.ProjectedAgeRoundedToNearestYear;
                }
                //Systest PIR:2021 If the Member age is past the Retirement Age, use Member age as Retirement Age since we cannot allow Retro.

                if (lintMemberAge > lintRetirmentAge)
                {
                    lintMemberAge = lintRetirmentAge;
                }
                DataTable ldtbList = null;
                if (ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase.Date < idteNewCalcEffDate.Date || ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id == busConstant.PlanIdJobService)
                {
                    //PIR - 17357 Added new coloumn Effective_Date.
                    ldtbList = SelectWithOperator<cdoServiceCreditActuarialFactor>(
                                              new String[4] { "actuarial_table_reference_value", "member_age", "retirement_age", "effective_date" },
                                              new String[4] { "=", "=", "=", "<=" },
                                              new object[4] { iobjcdoServiceCreditPlanFormulaRef.actuarial_table_reference_value,lintMemberAge,
                                                          lintRetirmentAge,this._ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase},
                                              "effective_date desc");
                }
                else
                {
                    string lstrEmpType = string.Empty;
                    if (_ibusServicePurchaseHeader.ibusPersonAccount.IsNull()) _ibusServicePurchaseHeader.LoadPersonAccount();
                    if (_ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain ||
                        _ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020)//PIR 20232 ?code
                    {
                        _ibusServicePurchaseHeader.ibusPersonAccount.LoadAllPersonEmployments(_ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase);
                        if (_ibusServicePurchaseHeader.ibusPersonAccount.iclcPersonEmployments.Count > 0)
                        {
                            if (_ibusServicePurchaseHeader.ibusPersonAccount.iclcPersonEmployments.Count == 1)
                            {
                                lstrEmpType = (_ibusServicePurchaseHeader.ibusPersonAccount.iclcPersonEmployments[0]
                                                .ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState) ?
                                                busConstant.EmployerCategoryState : string.Empty;
                            }
                            else
                            {
                                if (_ibusServicePurchaseHeader.ibusPersonAccount.iclcPersonEmployments.Select(e => e.ibusOrganization.icdoOrganization.org_id).Distinct().Count() == 1)
                                {
                                    lstrEmpType = (_ibusServicePurchaseHeader.ibusPersonAccount.iclcPersonEmployments[0]
                                                .ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState) ?
                                                busConstant.EmployerCategoryState : string.Empty;
                                }
                                else
                                {
                                    busPersonEmployment lbusLongestPersonEmployment = _ibusServicePurchaseHeader
                                                            .ibusPersonAccount
                                                            .iclcPersonEmployments
                                                            .OrderByDescending(e => busGlobalFunctions.DateDiffInDays(e.icdoPersonEmployment.start_date, e.icdoPersonEmployment.end_date_no_null_today))
                                                            .FirstOrDefault();
                                    if (lbusLongestPersonEmployment.IsNotNull())
                                        lstrEmpType = (lbusLongestPersonEmployment.ibusOrganization.icdoOrganization.emp_category_value == busConstant.EmployerCategoryState) ?
                                                busConstant.EmployerCategoryState : string.Empty;
                                }
                            }
                        }
                    }
                    ldtbList = Select("cdoServiceCreditActuarialFactor.LoadServiceCreditActuarialFactors",
                                                        new object[7] { ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id,
                                                                        iobjcdoServiceCreditPlanFormulaRef.actuarial_table_reference_value,
                                                                        lintMemberAge,
                                                                        lintRetirmentAge,
                                                                        _ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase,
                                                                        busGlobalFunctions.Round(tot_current_service_plus_free_purchase_in_months_psc),
                                                                        lstrEmpType});
                }

                // Go and get the age value mentioned in the SGT_Service_Purchase_Formula_Ref table.
                if (ldtbList.Rows.Count > 0)
                {
                    cdoServiceCreditActuarialFactor lobjcdoServiceCreditActuarialFactor = new cdoServiceCreditActuarialFactor();
                    lobjcdoServiceCreditActuarialFactor.LoadData(ldtbList.Rows[0]);

                    switch (ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_code)
                    {
                        case busConstant.Plan_Code_Main:
                        case busConstant.Plan_Code_Judges:
                        case busConstant.Plan_Code_Highway_Patrol:
                        case busConstant.Plan_Code_National_Guard:
                        case busConstant.Plan_Code_LE_With_Prior_Service:
                        case busConstant.Plan_Code_LE_Without_Prior_Service:
                        case busConstant.Plan_Code_Defined_Contribution:
                        case busConstant.Plan_Code_BCI_Law_Enforcement: //pir 7943
                        case busConstant.Plan_Code_Defined_Contribution_2020: //PIR 20232
                        case busConstant.Plan_Code_Main_2020: //PIR 20232
                        case busConstant.Plan_Code_State_Public_Safety: //PIR 25729
                            if (!ablnWithOrWithoutPurchase)
                            {
                                icdoServicePurchaseDetail.retirement_actuarial_factor = lobjcdoServiceCreditActuarialFactor.retirement_actuarial_factor;
                                icdoServicePurchaseDetail.rhic_retirement_factor = lobjcdoServiceCreditActuarialFactor.rhic_actuarial_factor;
                                icdoServicePurchaseDetail.fut_ee_act_factor_without_pur = lobjcdoServiceCreditActuarialFactor.future_ee_actuarial_factor;
                            }
                            else
                            {
                                icdoServicePurchaseDetail.ret_act_factor_with_pur = lobjcdoServiceCreditActuarialFactor.retirement_actuarial_factor;
                                icdoServicePurchaseDetail.rhic_ret_factor_with_pur = lobjcdoServiceCreditActuarialFactor.rhic_actuarial_factor;
                                icdoServicePurchaseDetail.fut_ee_act_factor_with_pur = lobjcdoServiceCreditActuarialFactor.future_ee_actuarial_factor;
                            }
                            if (ibusServicePurchaseHeader.icdoServicePurchaseHeader.date_of_purchase.Date < idteNewCalcEffDate.Date)
                            {
                                icdoServicePurchaseDetail.fut_ee_act_factor_without_pur = 0.0M;
                            }
                            break;
                        case busConstant.Plan_Code_Job_Service:
                            icdoServicePurchaseDetail.retirement_actuarial_factor = lobjcdoServiceCreditActuarialFactor.retirement_actuarial_factor;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// This will be used for calculating Acturial Factor for Job Service Plan
        /// </summary>
        private void CalculateProjectedRetirmentAge()
        {
            if (_ibusServicePurchaseHeader == null)
                LoadServicePurchaseHeader();

            if (icdoServicePurchaseDetail.project_retirement_date != System.DateTime.MinValue)
            {
                if (_ibusServicePurchaseHeader.ibusPerson == null)
                {
                    _ibusServicePurchaseHeader.LoadPerson();
                }
                if (_ibusServicePurchaseHeader.ibusPerson.icdoPerson.date_of_birth != System.DateTime.MinValue)
                {
                    int lintYears;
                    int lintMonths;
                    DateTime ldtFrom = new DateTime(_ibusServicePurchaseHeader.ibusPerson.icdoPerson.date_of_birth.Year, _ibusServicePurchaseHeader.ibusPerson.icdoPerson.date_of_birth.Month, 1);
                    DateTime ldtTo = new DateTime(icdoServicePurchaseDetail.project_retirement_date.Year, icdoServicePurchaseDetail.project_retirement_date.Month, 1);
                    HelperFunction.GetMonthSpan(ldtFrom, ldtTo, out lintYears, out lintMonths);
                    icdoServicePurchaseDetail.projected_age_year_part = lintYears;
                    icdoServicePurchaseDetail.projected_age_month_part = lintMonths;
                }
            }
        }

        public void PopulateServiceCreditPlanFormula()
        {
            LoadServicePurchaseHeader();
            if (ibusServicePurchaseHeader.ibusPlan == null)
                ibusServicePurchaseHeader.LoadPlan();

            DataTable ldtbList = Select("cdoServiceCreditPlanFormulaRef.LoadServiceCreditFormulaRef",
                                        new object[3]
                                            {
                                                ibusServicePurchaseHeader.icdoServicePurchaseHeader.ServiceCreditTypeData1,
                                                ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id,
                                                ibusServicePurchaseHeader.icdoServicePurchaseHeader.payor_value
                                            });
            // Go and get the age value mentioned in the SGT_Service_Purchase_Formula_Ref table.
            if (ldtbList.Rows.Count > 0)
            {
                iobjcdoServiceCreditPlanFormulaRef = new cdoServiceCreditPlanFormulaRef();
                iobjcdoServiceCreditPlanFormulaRef.LoadData(ldtbList.Rows[0]);
            }
        }

        public void CalculateEarliestNormalRetirementAge(bool ablnExcludeServiceToBePurchased = false)
        {
            if (ibusServicePurchaseHeader == null)
                LoadServicePurchaseHeader();
            //PIR 26151
            // Check to see whether the object ServiceCreditPlanFormula has been populated.
            //if (iobjcdoServiceCreditPlanFormulaRef != null && iobjcdoServiceCreditPlanFormulaRef.service_credit_plan_formula_ref_id > 0)
            //{
                decimal lintEarliestNormalRetirementAge = 0;
                decimal lintTotalVestingServiceCredit = 0;
                //int lintAgeForENRACalculation = iobjcdoServiceCreditPlanFormulaRef.age_for_enra_calculation;
                //int lintAgeCeilingForENRA = iobjcdoServiceCreditPlanFormulaRef.age_ceiling_for_enra;
                switch (ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_code)
                {
                    case busConstant.Plan_Code_Main:
                    case busConstant.Plan_Code_LE_With_Prior_Service:
                    case busConstant.Plan_Code_LE_Without_Prior_Service:
                    case busConstant.Plan_Code_Judges:
                    case busConstant.Plan_Code_Highway_Patrol:
                    case busConstant.Plan_Code_Defined_Contribution:
                    case busConstant.Plan_Code_BCI_Law_Enforcement: //pir 7943
                    case busConstant.Plan_Code_Main_2020: //PIR 20232
                    case busConstant.Plan_Code_Defined_Contribution_2020: //PIR 20232
                    case busConstant.Plan_Code_State_Public_Safety: //PIR 25729
                    case busConstant.Plan_Code_National_Guard://PIR 25729
                        lintTotalVestingServiceCredit = (icdoServicePurchaseDetail.vsc > 0) ? icdoServicePurchaseDetail.vsc : (ibusServicePurchaseHeader.icdoServicePurchaseHeader.total_vsc);
                    //lintEarliestNormalRetirementAge = GetEarliestNormalRetirementAge(lintAgeForENRACalculation,
                    //                                                                 lintTotalVestingServiceCredit, ablnExcludeServiceToBePurchased);
                    //// We have to get the minimum value between Age ceiling and the calculated value.
                    //lintEarliestNormalRetirementAge = Math.Min(lintAgeCeilingForENRA,
                    //                                           lintEarliestNormalRetirementAge);
                    
                    if (!ablnExcludeServiceToBePurchased && ibusServicePurchaseHeader.idecAdditionalServiceVSC <= 0)
                    {
                        ibusServicePurchaseHeader.LoadInPaymnetServicePurchases();
                    }
                    
                    if (ibusServicePurchaseHeader.ibusPerson == null)
                        ibusServicePurchaseHeader.LoadPerson();
                    if (ibusServicePurchaseHeader.ibusPersonAccount.IsNull() || ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                        ibusServicePurchaseHeader.LoadPersonAccount();
                    DateTime ldtNormalRetirementDate = busPersonBase.GetNormalRetirementDateBasedOnNormalEligibility(ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_id,
                                                        ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_code, ibusServicePurchaseHeader.ibusPlan.icdoPlan.benefit_provision_id,
                                                        busConstant.ApplicationBenefitTypeRetirement, ibusServicePurchaseHeader.ibusPerson.icdoPerson.date_of_birth,
                                                        lintTotalVestingServiceCredit, 0, iobjPassInfo, DateTime.MinValue,
                                                        ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.person_account_id, true, ((ablnExcludeServiceToBePurchased) ? TotalFreeServiceWithPurchase : ibusServicePurchaseHeader.idecAdditionalServiceVSC + icdoServicePurchaseDetail.total_time_to_purchase));

                    int lintTotalMonths = 0;
                    int lintMemberAgeMonthPart = 0;
                    int lintMemberAgeYearPart = 0;
                    decimal adecMonthAndYear = 0;

                    busPersonBase.CalculateAge(ibusServicePurchaseHeader.ibusPerson.icdoPerson.date_of_birth.AddMonths(1), ldtNormalRetirementDate, ref lintTotalMonths, ref adecMonthAndYear,
                                               4, ref lintMemberAgeYearPart, ref lintMemberAgeMonthPart);
                    lintEarliestNormalRetirementAge = adecMonthAndYear;

                    break;
                    //PIR 25729 - Commented for NG
                    //case busConstant.Plan_Code_National_Guard:
                    //    // We have to get the maximum value between Age ceiling and the current age.
                    //    lintEarliestNormalRetirementAge = Math.Max(lintAgeCeilingForENRA,
                    //                                               ibusServicePurchaseHeader.icdoServicePurchaseHeader.
                    //                                                   current_age_year_part);
                    //    break;
                    default:
                        break;
                }

                //int lintMemberAge = ibusServicePurchaseHeader.icdoServicePurchaseHeader.CurrentAgeRoundedToNearestYear;
                ////PIR: 2021 When the person is past their NRD Date , then set NRD Age as the person age.
                //if (lintMemberAge > lintEarliestNormalRetirementAge)
                //    lintEarliestNormalRetirementAge = lintMemberAge;
                //PIR 18378 - For anyone in Benefit Tier 16MT the NRD cannot be below 60 - Start
                if (ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id == busConstant.PlanIdMain)//PIR 20232 ?code
                {
                    if (ibusServicePurchaseHeader.ibusPersonAccount.IsNull() || ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.person_account_id == 0)
                        ibusServicePurchaseHeader.LoadPersonAccount();
                    if (ibusServicePurchaseHeader.ibusPersonAccount.ibusPersonAccountRetirement.IsNull())
                        ibusServicePurchaseHeader.ibusPersonAccount.LoadPersonAccountRetirement();
                    if (!string.IsNullOrEmpty(ibusServicePurchaseHeader.ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value) &&
                        ibusServicePurchaseHeader.ibusPersonAccount.ibusPersonAccountRetirement.icdoPersonAccountRetirement.benefit_tier_value == busConstant.MainBenefit2016Tier &&
                        lintEarliestNormalRetirementAge < 60)
                    {
                        lintEarliestNormalRetirementAge = 60;
                    }
                }
                //PIR 18378 - For anyone in Benefit Tier 16MT the NRD cannot be below 60 - End
                // We need to update the value only when the earliest normal retirement age is not equal to the value already present
                // in the database.
                if (icdoServicePurchaseDetail.earliest_normal_retirement_age != lintEarliestNormalRetirementAge && !ablnExcludeServiceToBePurchased)
                {
                    icdoServicePurchaseDetail.earliest_normal_retirement_age =  lintEarliestNormalRetirementAge;
                }

            //PIR 26930
            decimal ldecMemberAge = Math.Round(ibusServicePurchaseHeader.icdoServicePurchaseHeader.current_age, 2, MidpointRounding.AwayFromZero);

            if (ablnExcludeServiceToBePurchased)
            {
                icdoServicePurchaseDetail.earliest_nor_ret_age_without_service_purchased = lintEarliestNormalRetirementAge;

                if (ldecMemberAge >= icdoServicePurchaseDetail.earliest_nor_ret_age_without_service_purchased)
                    icdoServicePurchaseDetail.earliest_nor_ret_age_without_service_purchased = ldecMemberAge;
            }
            //PIR 26930
            if (ldecMemberAge >= icdoServicePurchaseDetail.earliest_normal_retirement_age)
            {
                icdoServicePurchaseDetail.earliest_normal_retirement_age = ldecMemberAge;
            }
            //}
        }

        private decimal GetEarliestNormalRetirementAge(int lintAgeForENRACalculation, decimal lintTotalVestingServiceCredit, bool ablnExcludeServiceToPurchase = false)
        {
            decimal ldecEarliestNormalRetirementAge =
                ((
                    (lintAgeForENRACalculation * 12) -
                        (
                            ibusServicePurchaseHeader.icdoServicePurchaseHeader.current_age_in_months + lintTotalVestingServiceCredit
                            + ((ablnExcludeServiceToPurchase) ? TotalFreeServiceWithPurchase : icdoServicePurchaseDetail.total_time_to_purchase)
                            //+ ibusServicePurchaseHeader.icdoServicePurchaseHeader.free_or_dual_service
                        )
                 )
                    / 2
                +
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.current_age_in_months) / 12;

            return ldecEarliestNormalRetirementAge;
        }


        public void CalculateBenefitToBePurchased()
        {
            LoadServicePurchaseHeader();
            // We have to do the calculation only when the service purchase detail record is populated.
            if (ibusServicePurchaseHeader != null)
            {
                if (ibusServicePurchaseHeader.ibusPlan == null)
                    ibusServicePurchaseHeader.LoadPlan();

                if (_iclbBenefitMultiplierTierData == null)
                    LoadBenefitMultiplierTierData();

                if (_iclbBenefitMultiplierTierData.Count == 0) return;

                if (_tier1_percentage == 0)
                    LoadTierPercentage();

                if (_RHIC_Multiplier_Amount == 0)
                    LoadRHICMultiplierData();
					
                if (icdoServicePurchaseDetail.judges_conversion_flag == busConstant.Flag_Yes)
                {
                    // We need to calculate only the Retirement Benefit Purchased and not the RHIC purchased for
                    // Judges conversion.       
                    //UAT PIR 1060                    
                    CalculateBenefitMultiplierCurrentAndPurchasedService();
                    CalculateBenefitMultiplierCurrentService();
					
                    icdoServicePurchaseDetail.retirement_benefit_purchased =
                        busGlobalFunctions.RoundToPenny(ibusServicePurchaseHeader.icdoServicePurchaseHeader.idecUserFASSalary * increase_in_benifit_multiplier);
                    icdoServicePurchaseDetail.rhic_benefit_purchased = 0;
                }
                else
                {
                    switch (ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_code)
                    {
                        case busConstant.Plan_Code_Main:
                        case busConstant.Plan_Code_National_Guard:
                        case busConstant.Plan_Code_LE_With_Prior_Service:
                        case busConstant.Plan_Code_LE_Without_Prior_Service:
                        case busConstant.Plan_Code_Defined_Contribution:
                        case busConstant.Plan_Code_BCI_Law_Enforcement: //pir 7943
                        case busConstant.Plan_Code_Main_2020: //PIR 20232
                        case busConstant.Plan_Code_Defined_Contribution_2020: //PIR 20232
                        case busConstant.Plan_Code_State_Public_Safety: //PIR 25729
                            icdoServicePurchaseDetail.retirement_benefit_purchased =
                                busGlobalFunctions.RoundToPenny(ibusServicePurchaseHeader.icdoServicePurchaseHeader.idecUserFASSalary * _tier1_percentage * total_time_to_purchase_by_year);
                            icdoServicePurchaseDetail.rhic_benefit_purchased =
                                busGlobalFunctions.RoundToPenny(_RHIC_Multiplier_Amount * total_time_to_purchase_by_year);
                            break;
                        case busConstant.Plan_Code_Judges:
                        case busConstant.Plan_Code_Highway_Patrol:
                            CalculateBenefitMultiplierCurrentAndPurchasedService();
                            CalculateBenefitMultiplierCurrentService();
                            icdoServicePurchaseDetail.retirement_benefit_purchased =
                                busGlobalFunctions.RoundToPenny(ibusServicePurchaseHeader.icdoServicePurchaseHeader.idecUserFASSalary * increase_in_benifit_multiplier);
                            icdoServicePurchaseDetail.rhic_benefit_purchased =
                                busGlobalFunctions.RoundToPenny(_RHIC_Multiplier_Amount * total_time_to_purchase_by_year);
                            break;
                        case busConstant.Plan_Code_Job_Service:
                            CalculateBenefitMultiplierCurrentAndPurchasedService();
                            CalculateBenefitMultiplierCurrentService();
                            icdoServicePurchaseDetail.retirement_benefit_purchased =
                                busGlobalFunctions.RoundToPenny(ibusServicePurchaseHeader.icdoServicePurchaseHeader.idecUserFASSalary * increase_in_benifit_multiplier);
                            icdoServicePurchaseDetail.rhic_benefit_purchased = 0;
                            break;
                        default:
                            icdoServicePurchaseDetail.retirement_benefit_purchased = 0;
                            icdoServicePurchaseDetail.rhic_benefit_purchased = 0;
                            break;
                    }
                }
            }
        }

        public void LoadServicePurchaseDetailConsolidated()
        {
            if (iclbServicePurchaseDetailConsolidated == null)
                iclbServicePurchaseDetailConsolidated = new Collection<busServicePurchaseDetailConsolidated>();

            DataTable ldtbList = Select<cdoServicePurchaseDetailConsolidated>(
                new string[1] { "service_purchase_detail_id" },
                new object[1] { icdoServicePurchaseDetail.service_purchase_detail_id }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                iclbServicePurchaseDetailConsolidated = GetCollection<busServicePurchaseDetailConsolidated>(ldtbList, "icdoServicePurchaseDetailConsolidated");
            }
            foreach (busServicePurchaseDetailConsolidated lobjServicePurchaseDetailConsolidated in iclbServicePurchaseDetailConsolidated)
            {
                lobjServicePurchaseDetailConsolidated.LoadOrganization();
                lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated.istrOrgCodeId = lobjServicePurchaseDetailConsolidated.ibusOrganization.icdoOrganization.org_code;
            }
        }

        public void LoadServicePurchaseDetailUSERRA()
        {
            if (iclbServicePurchaseDetailUserra == null)
                iclbServicePurchaseDetailUserra = new Collection<busServicePurchaseDetailUserra>();
            DataTable ldtbList = Select<cdoServicePurchaseDetailUserra>(
             new string[1] { "service_purchase_detail_id" },
             new object[1] { icdoServicePurchaseDetail.service_purchase_detail_id }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                iclbServicePurchaseDetailUserra = GetCollection<busServicePurchaseDetailUserra>(ldtbList, "icdoServicePurchaseDetailUserra");
            }

            //Assign the current object into the Detail Object in Collection 
            //Will be used when we recompute the amount when the user presses the save button
            foreach (busServicePurchaseDetailUserra lobjServicePurchaseDetailUserra in _iclbServicePurchaseDetailUserra)
            {
                lobjServicePurchaseDetailUserra.ibusServicePurchaseDetail = this;
            }
        }

        public void CalculateTimetoPurchaseForConsolidated()
        {
            LoadServicePurchaseDetailConsolidated();

            if (iclbServicePurchaseDetailConsolidated != null && iclbServicePurchaseDetailConsolidated.Count > 0)
            {
                int lintTotalTimeToPurchase = 0;
                int lintTotalTimeToPurchaseExcludeFreeService = 0;
                foreach (busServicePurchaseDetailConsolidated lobjServicePurchaseDetailConsolidated in iclbServicePurchaseDetailConsolidated)
                {
                    lintTotalTimeToPurchase = lintTotalTimeToPurchase +
                                              lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated.calculated_time_to_purchase;

                    if (lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated.service_credit_type_value != busConstant.Service_Purchase_Type_Additional_Free_Service)
                    {
                        lintTotalTimeToPurchaseExcludeFreeService = lintTotalTimeToPurchaseExcludeFreeService +
                                              lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated.calculated_time_to_purchase;
                    }
                }
                icdoServicePurchaseDetail.total_time_to_purchase = lintTotalTimeToPurchase;
                icdoServicePurchaseDetail.total_time_to_purchase_exclude_free_service = lintTotalTimeToPurchaseExcludeFreeService;
            }
        }

        public void CalculateTotalRefundPlusInterest()
        {
            decimal ldecTotalRefundPlusInterest = 0;
            if (iclbServicePurchaseDetailConsolidated != null && iclbServicePurchaseDetailConsolidated.Count > 0)
            {
                foreach (
                    busServicePurchaseDetailConsolidated lobjServicePurchaseDetailConsolidated in
                        iclbServicePurchaseDetailConsolidated)
                {
                    if (lobjServicePurchaseDetailConsolidated.icdoServicePurchaseDetailConsolidated.service_credit_type_value == busConstant.Service_Purchase_Type_Previous_Pers_Employment)
                    {

                        ldecTotalRefundPlusInterest = ldecTotalRefundPlusInterest +
                                                      lobjServicePurchaseDetailConsolidated.
                                                          icdoServicePurchaseDetailConsolidated.refund_with_interest;
                    }
                }
            }
            icdoServicePurchaseDetail.total_refund_and_interest = ldecTotalRefundPlusInterest;
        }

        public void LoadServicePurchaseHeader()
        {
            if (ibusServicePurchaseHeader == null)
                ibusServicePurchaseHeader = new busServicePurchaseHeader { icdoServicePurchaseHeader = new cdoServicePurchaseHeader() };
            DataTable ldtbList = Select<cdoServicePurchaseHeader>(
                new string[1] { "service_purchase_header_id" },
                new object[1] { icdoServicePurchaseDetail.service_purchase_header_id }, null, null);

            if (ldtbList.Rows.Count > 0)
            {
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.LoadData(ldtbList.Rows[0]);
                ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail = this;
            }
        }



        public override bool ValidateSoftErrors()
        {
            if (iblnHeaderValidating)
            {
                if (ibusSoftErrors == null)
                {
                    LoadErrors();
                }

                LoadServicePurchaseDetailConsolidated();
                if (iclbServicePurchaseDetailConsolidated != null)
                {
                    foreach (busServicePurchaseDetailConsolidated lobjServicePurchaseDetailConsolidated in iclbServicePurchaseDetailConsolidated)
                    {
                        lobjServicePurchaseDetailConsolidated.iblnHeaderValidating = true;
                        lobjServicePurchaseDetailConsolidated.ibusServicePurchaseDetail = this;
                        lobjServicePurchaseDetailConsolidated.ValidateSoftErrors();
                    }
                }
                iblnClearSoftErrors = false;
                ibusSoftErrors.iblnClearError = false;
                return base.ValidateSoftErrors();
            }
            else
            {
                return ibusServicePurchaseHeader.ValidateSoftErrors();
            }
        }
        // get member's actual age
        public int iintMemberAge(DateTime adtStartDateOfService)
        {
            if (ibusServicePurchaseHeader == null)
                LoadServicePurchaseHeader();
            if (ibusServicePurchaseHeader.ibusPerson == null)
                ibusServicePurchaseHeader.LoadPerson();
            return busGlobalFunctions.CalulateAge(ibusServicePurchaseHeader.ibusPerson.icdoPerson.date_of_birth, adtStartDateOfService);
        }

        public decimal GetUSERRAERCostAmt()
        {
            decimal ldecReturn = 0;

            if (_iclbServicePurchaseDetailUserra == null)
                LoadServicePurchaseDetailUSERRA();
            foreach (busServicePurchaseDetailUserra lbusUSERRADtl in _iclbServicePurchaseDetailUserra)
            {
                ldecReturn += lbusUSERRADtl.icdoServicePurchaseDetailUserra.employer_contribution;
            }

            return ldecReturn;
        }
        public decimal GetUSERRAEECostAmt()
        {
            decimal ldecReturn = 0;

            if (_iclbServicePurchaseDetailUserra == null)
                LoadServicePurchaseDetailUSERRA();
            foreach (busServicePurchaseDetailUserra lbusUSERRADtl in _iclbServicePurchaseDetailUserra)
            {
                ldecReturn += lbusUSERRADtl.icdoServicePurchaseDetailUserra.employee_contribution;
            }

            return ldecReturn;
        }
        public decimal GetUSERRARHICCostAmt()
        {
            decimal ldecReturn = 0;

            if (_iclbServicePurchaseDetailUserra == null)
                LoadServicePurchaseDetailUSERRA();
            foreach (busServicePurchaseDetailUserra lbusUSERRADtl in _iclbServicePurchaseDetailUserra)
            {
                ldecReturn += lbusUSERRADtl.icdoServicePurchaseDetailUserra.rhic_contribution;
            }

            return ldecReturn;
        }
    }
}
