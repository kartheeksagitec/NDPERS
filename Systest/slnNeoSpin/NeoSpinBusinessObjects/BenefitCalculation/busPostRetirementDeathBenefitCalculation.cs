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
using Sagitec.Bpm;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPostRetirementDeathBenefitCalculation : busBenefitCalculation
    {
        public bool iblnIsNewMode = false;

        public cdoPostRetirementDeathBenefitOptionRef icdoPSTDBenefitOptionRef { get; set; }

        public void LoadPSTDBenefitOptionRef()
        {
            if (icdoPSTDBenefitOptionRef.IsNull())
                icdoPSTDBenefitOptionRef = new cdoPostRetirementDeathBenefitOptionRef();
            if (ibusOriginatingPayeeAccount.IsNull())
                LoadOriginatingPayeeAccount();

            // Account Type should be null in case of Alternate Payee Death in order to fetch results.
            string lstrBenefitAccountType = string.Empty;
            if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                lstrBenefitAccountType = null;
            else
                lstrBenefitAccountType = ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value;


            bool iblnIsTermDatePastDateofDeath = false;

            if ((ibusOriginatingPayeeAccount.icdoPayeeAccount.IsTermCertainBenefitOption())
                && (icdoBenefitCalculation.post_retirement_death_reason_type_value !=busConstant.PostRetirementFirstBeneficiaryDeath))
            {
                if (ibusOriginatingPayeeAccount.icdoPayeeAccount.term_certain_end_date != DateTime.MinValue)
                {
                    if (ibusOriginatingPayeeAccount.icdoPayeeAccount.term_certain_end_date < icdoBenefitCalculation.date_of_death)
                    {
                        iblnIsTermDatePastDateofDeath = true;
                    }
                }
            }

            icdoPSTDBenefitOptionRef = FetchDeathBenefitOption(
                                            ibusPlan.icdoPlan.benefit_provision_id, lstrBenefitAccountType,
                                            ibusMember.IsMarried, icdoBenefitCalculation.benefit_option_value,
                                            icdoBenefitCalculation.post_retirement_death_reason_type_value, true,iblnIsTermDatePastDateofDeath, iobjPassInfo);
        }

        /// CALCULATES THE POST-RETIREMENT DEATH BENEFIT AMOUNT
        public void CalculatePostRetirementDeathBenefit()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            if (icdoPSTDBenefitOptionRef.IsNull())
                LoadPSTDBenefitOptionRef();
            if (ibusBenefitProvisionBenefitType.IsNull())
                LoadBenefitProvisionBenefitType();
            if (ibusBenefitProvisionBenefitOption.IsNull())
                LoadBenefitProvisionBenefitOption();
            if (ibusOriginatingPayeeAccount.IsNull())
                LoadOriginatingPayeeAccount();

            // 1. Get Finaly Monthly Benfit Amount
			//Calculate the Monthly Benefit Amount as on the Date of Death of the person, other wise when the death notification is completed and an end date is put, the next benefit begin date gross amount will yield zero only.
            //UAT PIR: 2132.
            //While fetching the Monthly Benefit Amount, Fetch the amount as of 1st of the month following date of death, here retiremetn date or Go Live date which ever is later.
            DateTime ldtMonthlyAmountFetchDate = icdoBenefitCalculation.retirement_date;
            DateTime ldtPersLinkGoLiveDate = busPayeeAccountHelper.GetPERSLinkGoLiveDate();
            if (ldtMonthlyAmountFetchDate < ldtPersLinkGoLiveDate)
            {
                ldtMonthlyAmountFetchDate = ldtPersLinkGoLiveDate;
            }
            ibusOriginatingPayeeAccount.LoadGrossAmount(ldtMonthlyAmountFetchDate);  // Calculate Gross Amount everytime

            icdoBenefitCalculation.final_monthly_benefit = ibusOriginatingPayeeAccount.idecGrossAmount;
            if ((icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath) &&
                (icdoPSTDBenefitOptionRef.is_monthly_benefit_flag == busConstant.Flag_Yes) &&
                (icdoPSTDBenefitOptionRef.account_relation_value == busConstant.AccountRelationshipJointAnnuitant) &&
                (IsJobService || icdoBenefitCalculation.plan_id == busConstant.PlanIdJudges))
            {
                if (ibusOriginatingPayeeAccount.ibusBenefitAccount.IsNull())
                    ibusOriginatingPayeeAccount.LoadBenfitAccount();
                if (ibusMember.icdoPerson.date_of_death >= ibusOriginatingPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.ssli_change_date)
                {
                    icdoBenefitCalculation.final_monthly_benefit = ibusOriginatingPayeeAccount.idecGrossAmount - icdoBenefitCalculation.estimated_ssli_benefit_amount;
                }
            }
			//UAT PIR: 925 Graduated Benefit Option Code changes.
            if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath)
            {
                icdoBenefitCalculation.graduated_benefit_option_value = ibusOriginatingPayeeAccount.icdoPayeeAccount.graduated_benefit_option_value;
            }

            // 2. Get Remaining Minimum Guarantee amount to be split up for Refund.
            decimal ldecTaxableMininumGuaranteeAmount = 0M;
            decimal ldecNonTaxableMinimumGuranteeAmount = 0M;
            decimal ldecMemberAccountBalance = 0M;
            decimal ldecRemainingMemberAccountBalance = 0M;

            ibusOriginatingPayeeAccount.LoadPaymentDetails();
            busPersonAccountRetirement lobjPARetirement = new busPersonAccountRetirement { icdoPersonAccountRetirement = new cdoPersonAccountRetirement() };
            lobjPARetirement.FindPersonAccountRetirement(ibusPersonAccount.icdoPersonAccount.person_account_id);
            lobjPARetirement.LoadLTDSummaryForCalculation(icdoBenefitCalculation.date_of_death, icdoBenefitCalculation.benefit_account_type_value);

            ldecMemberAccountBalance = lobjPARetirement.Member_Account_Balance_ltd;
			//For Post Retirement Death Account Owner Death, the originating Payee Account Benefit Type being Disability and the Option available for the beneficiary being Refund, the Amount being splitted into beneficiaries is stored in Contribution Table Factors like ER_VESTED Amount etc...
            decimal ldecpre_tax_ee_amount_ltd = lobjPARetirement.pre_tax_ee_amount_ltd;
            decimal ldecpost_tax_ee_amount_ltd = lobjPARetirement.post_tax_ee_amount_ltd;
            decimal ldecee_er_pickup_amount_ltd = lobjPARetirement.ee_er_pickup_amount_ltd;
            decimal ldecinterest_amount_ltd = lobjPARetirement.interest_amount_ltd;
            decimal ldecer_vested_amount_ltd = lobjPARetirement.er_vested_amount_ltd;
            decimal ldecee_rhic_amount_ltd = lobjPARetirement.ee_rhic_amount_ltd;
            decimal ldecCapital_gain = lobjPARetirement.icdoPersonAccountRetirement.capital_gain;
            decimal ldecpost_tax_ee_ser_pur_cont_ltd = lobjPARetirement.post_tax_ee_ser_pur_cont_ltd;
            decimal ldecee_rhic_ser_pur_cont_ltd = lobjPARetirement.ee_rhic_ser_pur_cont_ltd;
            decimal ldecpre_tax_ee_ser_pur_cont_ltd = lobjPARetirement.pre_tax_ee_ser_pur_cont_ltd;


            ldecRemainingMemberAccountBalance = ldecMemberAccountBalance - ibusOriginatingPayeeAccount.idecpaidgrossamount;
            icdoBenefitCalculation.taxable_amount = lobjPARetirement.Pre_Tax_Employee_Contribution_ltd - ibusOriginatingPayeeAccount.idecpaidtaxableamount;
            icdoBenefitCalculation.non_taxable_amount = lobjPARetirement.Post_Tax_Total_Contribution_ltd - ibusOriginatingPayeeAccount.idecpaidnontaxableamount;

            if (ldecRemainingMemberAccountBalance < 0) ldecRemainingMemberAccountBalance = 0M;
            if (icdoBenefitCalculation.taxable_amount < 0) icdoBenefitCalculation.taxable_amount = 0M;
            if (icdoBenefitCalculation.non_taxable_amount < 0) icdoBenefitCalculation.non_taxable_amount = 0M;

            //ldecTaxableMininumGuaranteeAmount = icdoBenefitCalculation.taxable_amount;
            //ldecNonTaxableMinimumGuranteeAmount = icdoBenefitCalculation.non_taxable_amount;
            //As per Systest PIR: 1999, MG is always MG in Originating payee account - amount already paid.
            //Logic for MG taxable and Non Taxable as suggested by satya.
            ibusOriginatingPayeeAccount.LoadBalanceNontaxableAmount();
            //PIR:1224
            //For Disability since the Minimum Guarantee is not stored, it is calculated as on date of death so that the interest amount is added.
            // Do not reduce the already paid amount
            if ((icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath)
                && (ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value==busConstant.ApplicationBenefitTypeDisability))
            {
                icdoBenefitCalculation.minimum_guarentee_amount = ldecMemberAccountBalance;
            }
            else
            {
                icdoBenefitCalculation.minimum_guarentee_amount = ibusOriginatingPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount - ibusOriginatingPayeeAccount.idecpaidgrossamount;
            }
            
            // 3. RTW (Optional Case)
            decimal ldecRTWRHICAmount = 0.0M;            
            if (IsRTW())
            {
                ReCalculateRTWRetirementBenefit();
                icdoBenefitCalculation.final_monthly_benefit = ibusParentBenefitCalculation.icdoBenefitCalculation.final_monthly_benefit;
                icdoBenefitCalculation.minimum_guarentee_amount = ibusParentBenefitCalculation.icdoBenefitCalculation.minimum_guarentee_amount;
                ldecRTWRHICAmount = ibusParentBenefitCalculation.idecMemberRHICAmount;

                // This code remains commented till confirm with Satya.
                //if (ibusParentBenefitCalculation.icdoBenefitCalculation.uniform_income_or_ssli_flag == busConstant.Flag_Yes)
                //{
                //    ibusParentBenefitCalculation.CalculateMemberAge();
                //    if (ibusParentBenefitCalculation.idecMemberAgeBasedOnRetirementDate >=
                //        ibusParentBenefitCalculation.icdoBenefitCalculation.ssli_or_uniform_income_commencement_age)
                //    {
                //        icdoBenefitCalculation.final_monthly_benefit -= ibusParentBenefitCalculation.icdoBenefitCalculation.estimated_ssli_benefit_amount;
                //    }
                //}
            }

            if (icdoBenefitCalculation.minimum_guarentee_amount < ibusOriginatingPayeeAccount.idecBalanceNontaxableAmount)
            {
                if(icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionRefund) //PIR 15440
                    icdoBenefitCalculation.minimum_guarentee_amount_non_taxable_amount = icdoBenefitCalculation.minimum_guarentee_amount;
                else
                    icdoBenefitCalculation.minimum_guarentee_amount_non_taxable_amount = ibusOriginatingPayeeAccount.idecBalanceNontaxableAmount;
                icdoBenefitCalculation.minimum_guarentee_amount_taxable_amount = 0;
            }
            else
            {
                icdoBenefitCalculation.minimum_guarentee_amount_taxable_amount = icdoBenefitCalculation.minimum_guarentee_amount - ibusOriginatingPayeeAccount.idecBalanceNontaxableAmount;
                icdoBenefitCalculation.minimum_guarentee_amount_non_taxable_amount = ibusOriginatingPayeeAccount.idecBalanceNontaxableAmount;
            }

            // 4. Get the Beneficiary Percentage
            decimal ldecBeneficiaryPercentage = 100M;
            //if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementFirstBeneficiaryDeath)
            //{
            //    ldecBeneficiaryPercentage = GetFirstBeneficiaryPercentage();
            //}

            // 5. Calculate Amount for Beneficiary
            if (icdoPSTDBenefitOptionRef.is_monthly_benefit_flag == busConstant.Flag_No)
            {
                icdoBenefitCalculation.minimum_guarentee_amount = (icdoBenefitCalculation.minimum_guarentee_amount * ldecBeneficiaryPercentage) / 100;
                icdoBenefitCalculation.minimum_guarentee_amount_non_taxable_amount = (icdoBenefitCalculation.minimum_guarentee_amount_non_taxable_amount * ldecBeneficiaryPercentage) / 100;
                icdoBenefitCalculation.minimum_guarentee_amount_taxable_amount = (icdoBenefitCalculation.minimum_guarentee_amount_taxable_amount * ldecBeneficiaryPercentage) / 100;
            }

            // 6. Load Benefit Calculation Payee for Grid
            LoadBenefitCalculationPayeeForNewMode();

            // 7. Calculate Benefit Option Amount for Payee
            iclbBenefitCalculationOptions = new Collection<busBenefitCalculationOptions>();
            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                decimal ldecTaxableAmount = icdoBenefitCalculation.minimum_guarentee_amount_taxable_amount;
                decimal ldecNonTaxableAmount = icdoBenefitCalculation.minimum_guarentee_amount_non_taxable_amount;

                if (lobjPayee.ibusPayee.IsNull())
                    lobjPayee.LoadPayee();
                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_amount = GetBenefitOptionAmountForPayee(
                                                icdoBenefitCalculation.benefit_option_value,
                                                lobjPayee.icdoBenefitCalculationPayee.benefit_percentage, icdoBenefitCalculation.minimum_guarentee_amount,
                                                icdoBenefitCalculation.final_monthly_benefit, ref ldecTaxableAmount, ref ldecNonTaxableAmount);

                busBenefitCalculationOptions lobjOptions = new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                lobjOptions.icdoBenefitCalculationOptions.benefit_option_amount = lobjPayee.icdoBenefitCalculationPayee.payee_benefit_amount;
                lobjOptions.icdoBenefitCalculationOptions.benefit_provision_benefit_option_id =
                                                ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_provision_benefit_option_id;
                //UAT PIR:2080 Fix
				if (lobjPayee.icdoBenefitCalculationPayee.payee_org_id > 0)
                {
                    lobjOptions.icdoBenefitCalculationOptions.payee_name = busGlobalFunctions.GetOrgNameByOrgID(
                                                            lobjPayee.icdoBenefitCalculationPayee.payee_org_id);
                }
                else
                {
                    lobjOptions.icdoBenefitCalculationOptions.payee_name = lobjPayee.ibusPayee.icdoPerson.PersonIdWithName;
                }
                if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionRefund)
                {
                    lobjOptions.icdoBenefitCalculationOptions.taxable_amount = ldecTaxableAmount;
                    lobjOptions.icdoBenefitCalculationOptions.non_taxable_amount = ldecNonTaxableAmount;

                    if ((icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath)
                    && (ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability))
                    {
                        decimal ldecBenpre_tax_ee_amount_ltd = ldecpre_tax_ee_amount_ltd * (lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100);
                        decimal ldecBenpost_tax_ee_amount_ltd = ldecpost_tax_ee_amount_ltd * (lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100);
                        decimal ldecBenee_er_pickup_amount_ltd = ldecee_er_pickup_amount_ltd * (lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100);
                        decimal ldecBeninterest_amount_ltd = ldecinterest_amount_ltd * (lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100);
                        decimal ldecBener_vested_amount_ltd = ldecer_vested_amount_ltd * (lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100);
                        decimal ldecBenee_rhic_amount_ltd = ldecee_rhic_amount_ltd * (lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100);
                        decimal ldecBenCapital_gain = ldecCapital_gain * (lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100);
                        decimal ldecBenpost_tax_ee_ser_pur_cont_ltd = ldecpost_tax_ee_ser_pur_cont_ltd * (lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100);
                        decimal ldecBenee_rhic_ser_pur_cont_ltd = ldecee_rhic_ser_pur_cont_ltd * (lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100);
                        decimal ldecBenpre_tax_ee_ser_pur_cont_ltd = ldecpre_tax_ee_ser_pur_cont_ltd * (lobjPayee.icdoBenefitCalculationPayee.benefit_percentage / 100);

                        lobjOptions.icdoBenefitCalculationOptions.pre_tax_ee_amount = ldecBenpre_tax_ee_amount_ltd;
                        lobjOptions.icdoBenefitCalculationOptions.post_tax_ee_amount = ldecBenpost_tax_ee_amount_ltd;
                        lobjOptions.icdoBenefitCalculationOptions.ee_er_pickup_amount = ldecBenee_er_pickup_amount_ltd;
                        lobjOptions.icdoBenefitCalculationOptions.interest_amount = ldecBeninterest_amount_ltd;
                        lobjOptions.icdoBenefitCalculationOptions.er_vested_amount = ldecBener_vested_amount_ltd;
                        lobjOptions.icdoBenefitCalculationOptions.ee_rhic_amount = ldecBenee_rhic_amount_ltd;
                        lobjOptions.icdoBenefitCalculationOptions.capital_gain = ldecBenCapital_gain;
                        lobjOptions.icdoBenefitCalculationOptions.post_tax_ee_ser_pur_cont = ldecBenpost_tax_ee_ser_pur_cont_ltd;
                        lobjOptions.icdoBenefitCalculationOptions.ee_rhic_ser_pur_cont = ldecBenee_rhic_ser_pur_cont_ltd;
                        lobjOptions.icdoBenefitCalculationOptions.pre_tax_ee_ser_pur_cont = ldecBenpre_tax_ee_ser_pur_cont_ltd;
                    }
                }
                iclbBenefitCalculationOptions.Add(lobjOptions);
            }
            // 8. Calculate RHIC Benefit Option
            iclbBenefitRHICOption = new Collection<busBenefitRHICOption>();

            //UAT PIR: 1222
            //For some options even though the Account Relation Ship value is not Joint Annuitant , but the application relation ship value is Spouse
            //and there exists only one Beneficiary then the Spouse is eligible to get the RHIC Amount in case of Account OWner death
            bool bIsMemberEligibleForRHIC = false;
            bool bFetchSpouseRHIC = false;
            if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath)
            {
                if (icdoPSTDBenefitOptionRef.is_monthly_benefit_flag == busConstant.Flag_Yes)
                {
                    if (icdoPSTDBenefitOptionRef.account_relation_value == busConstant.AccountRelationshipJointAnnuitant)
                    {
                        bIsMemberEligibleForRHIC = true;
                    }
                    else
                    {
                        if (ibusBenefitApplication == null)
                            LoadBenefitApplication();
                        if ((ibusBenefitApplication.icdoBenefitApplication.family_relationship_value == busConstant.FamilyRelationshipSpouse)
                            && (iclbBenefitCalculationPayee.Count() == 1))
                        {
                            bIsMemberEligibleForRHIC = true;
                        }
                    }
                }
                else
                {
                    //UAT PIR: 1245
                    //IF the person has opted for Reduced RHIC or Any other RHIC Option which has Spouse RHIC Amount in it.
                    //Then it should be provided to the Joint Annuitant as RHIC.Eventhough it has Refund Option.
                    if(ibusOriginatingPayeeAccount.ibusBenefitAccount==null)
                        ibusOriginatingPayeeAccount.LoadBenfitAccount();
                    if (ibusOriginatingPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount > 0)                        
                    {
                        if ((ibusBenefitApplication.icdoBenefitApplication.family_relationship_value == busConstant.FamilyRelationshipSpouse)
                            && (iclbBenefitCalculationPayee.Count() == 1))
                        {
                            bIsMemberEligibleForRHIC = true;
                            bFetchSpouseRHIC = true;
                        }
                    }
                }
            }            

            if(bIsMemberEligibleForRHIC)
            {
                if (ibusOriginatingPayeeAccount.ibusApplication.IsNull())
                    ibusOriginatingPayeeAccount.LoadApplication();
                string lstrRHICOptionValue = ibusOriginatingPayeeAccount.ibusApplication.icdoBenefitApplication.rhic_option_value ?? string.Empty;

                busBenefitRHICOption lobjRHICOption = new busBenefitRHICOption { icdoBenefitRhicOption = new cdoBenefitRhicOption() };
                lobjRHICOption.ibusBenefitProvisionOption = new busBenefitProvisionBenefitOption
                {
                    icdoBenefitProvisionBenefitOption = new cdoBenefitProvisionBenefitOption()
                };
                lobjRHICOption.icdoBenefitRhicOption.benefit_provision_benefit_option_id =
                                                ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_provision_benefit_option_id;
                lobjRHICOption.ibusBenefitProvisionOption.icdoBenefitProvisionBenefitOption.benefit_option_description =
                                                ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_description;
                //if it is an RTW Member then the Latest RHIC Amount Takes the precedence.
                if (ldecRTWRHICAmount > 0.0M)
                {
                    lobjRHICOption.icdoBenefitRhicOption.member_rhic_amount = ldecRTWRHICAmount;
                }
                else
                {
                    lobjRHICOption.icdoBenefitRhicOption.member_rhic_amount = ibusOriginatingPayeeAccount.icdoPayeeAccount.rhic_amount;
                }
                if (lstrRHICOptionValue == busConstant.RHICOptionReduced50)
                    lobjRHICOption.icdoBenefitRhicOption.option_factor = 0.5M;
                else
                    lobjRHICOption.icdoBenefitRhicOption.option_factor = 1;
                lobjRHICOption.icdoBenefitRhicOption.spouse_rhic_percentage = 0;
                lobjRHICOption.icdoBenefitRhicOption.rhic_option_value = lstrRHICOptionValue;
                lobjRHICOption.icdoBenefitRhicOption.rhic_option_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1905, lstrRHICOptionValue);
                if (bFetchSpouseRHIC)
                {
                    lobjRHICOption.icdoBenefitRhicOption.spouse_rhic_amount = ibusOriginatingPayeeAccount.ibusBenefitAccount.icdoBenefitAccount.spouse_rhic_amount;
                }
                else
                {
                    lobjRHICOption.icdoBenefitRhicOption.spouse_rhic_amount = (lobjRHICOption.icdoBenefitRhicOption.member_rhic_amount *
                                                    lobjRHICOption.icdoBenefitRhicOption.option_factor);
                }
                if (lobjRHICOption.icdoBenefitRhicOption.spouse_rhic_amount > 0)
                {
                    iclbBenefitRHICOption.Add(lobjRHICOption);
                }
            }
        }

        public decimal GetBenefitOptionAmountForPayee(string astrBenefitOptionValue, decimal adecBenefitPercentage, decimal adecMemberAccountBalance,
                                                decimal adecMonthlyAmount, ref decimal ldecTaxableAmount, ref decimal ldecNonTaxableAmount)
        {
            if (ibusBenefitProvisionBenefitOption.IsNull())
                LoadBenefitProvisionBenefitOption();

            if (icdoPSTDBenefitOptionRef.is_monthly_benefit_flag == busConstant.Flag_Yes)
            {
                if (ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.factor_method_value == busConstant.FactorMethodValueFactor)
                {
                    return Math.Round((adecMonthlyAmount * ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.spouse_factor), 2);
                }
                else if (ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.factor_method_value == busConstant.FactorMethodValueOther)
                {
                    return Math.Round(((adecMonthlyAmount * adecBenefitPercentage) / 100M), 2);
                }
            }
            else
            {
                if (ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.factor_method_value == busConstant.FactorMethodValueOther)
                {
                    ldecTaxableAmount = Math.Round(((icdoBenefitCalculation.minimum_guarentee_amount_taxable_amount * adecBenefitPercentage) / 100M), 2);
                    ldecNonTaxableAmount = Math.Round(((icdoBenefitCalculation.minimum_guarentee_amount_non_taxable_amount * adecBenefitPercentage) / 100M), 2);
                    return Math.Round(((adecMemberAccountBalance * adecBenefitPercentage) / 100M), 2);
                }
            }
            return 0M;
        }

        /// Saves the Parent Benefit Calculation as from screen.
        public void ReCalculateRTWRetirementBenefit()
        {
            ibusParentBenefitCalculation = new busRetirementBenefitCalculation { icdoBenefitCalculation = new cdoBenefitCalculation() };
            if (icdoBenefitCalculation.parent_benefit_calculation_id == 0)
            {
                if (ibusBenefitApplication.IsNull())
                    LoadBenefitApplication();
                busRetirementDisabilityApplication lobjParentBenefitApplication = new busRetirementDisabilityApplication
                {
                    icdoBenefitApplication = new cdoBenefitApplication()
                };
                lobjParentBenefitApplication.icdoBenefitApplication = ibusBenefitApplication.icdoBenefitApplication;
                ibusParentBenefitCalculation.GetCalculationByApplication(lobjParentBenefitApplication);
                ibusParentBenefitCalculation.icdoBenefitCalculation.ienuObjectState = ObjectState.Insert;
                ibusParentBenefitCalculation.icdoBenefitCalculation.annuitant_id = icdoBenefitCalculation.beneficiary_person_id;
                ibusParentBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                ibusParentBenefitCalculation.icdoBenefitCalculation.created_date = icdoBenefitCalculation.created_date;
                ibusParentBenefitCalculation.icdoBenefitCalculation.benefit_account_type_description =
                    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1904, busConstant.ApplicationBenefitTypeRetirement);
                ibusParentBenefitCalculation.icdoBenefitCalculation.pre_rtw_payee_account_id = iintRTWPayeeAccountID;
                ibusParentBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value = busConstant.ApplicationBenefitTypeRetirement;
                ibusParentBenefitCalculation.icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
                ibusParentBenefitCalculation.icdoBenefitCalculation.status_value = busConstant.StatusReview;
                ibusParentBenefitCalculation.icdoBenefitCalculation.action_status_value = busConstant.CalculationStatusPendingApproval;
                ibusParentBenefitCalculation.LoadJointAnnuitant();
                ibusParentBenefitCalculation.LoadPersonAccount();
                ibusParentBenefitCalculation.CalculateMemberAge();
                ibusParentBenefitCalculation.CalculateAnnuitantAge();                
                ibusParentBenefitCalculation.LoadBenefitCalculationPayeeForNewMode();
                ibusParentBenefitCalculation.iblnIsBenefitPayeeTabForNewModeVisible = true;
                ibusParentBenefitCalculation.CalculateRetirementBenefit();
                ibusParentBenefitCalculation.EvaluateInitialLoadRules();
                ibusParentBenefitCalculation.icdoBenefitCalculation.rtw_refund_election_value = busConstant.Flag_No_Value.ToUpper();
                ibusParentBenefitCalculation.CalculateRetirementBenefit();
            }
            else
            {
                ibusParentBenefitCalculation.FindBenefitCalculation(icdoBenefitCalculation.parent_benefit_calculation_id);
                ibusParentBenefitCalculation.LoadMember();
                ibusParentBenefitCalculation.LoadJointAnnuitant();
                ibusParentBenefitCalculation.LoadPlan();
                ibusParentBenefitCalculation.LoadPersonAccount();
                ibusParentBenefitCalculation.CalculateMemberAge();
                ibusParentBenefitCalculation.CalculateAnnuitantAge();
                ibusParentBenefitCalculation.LoadBenefitCalculationOptions();
                ibusParentBenefitCalculation.LoadBenefitRHICOption();
                ibusParentBenefitCalculation.LoadBenefitCalculationPersonAccount();
                // Do not Calculate Retirement Benefit if the Action Status is Approved or Cancelled.
                if ((ibusParentBenefitCalculation.icdoBenefitCalculation.action_status_value != busConstant.BenefitActionStatusApproved) ||
                    (ibusParentBenefitCalculation.icdoBenefitCalculation.action_status_value != busConstant.BenefitActionStatusCancelled))
                {
                    ibusParentBenefitCalculation.SetBenefitSubType();
                    ibusParentBenefitCalculation.CalculateRetirementBenefit();
                }
                if (ibusParentBenefitCalculation.icdoBenefitCalculation.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                    ibusParentBenefitCalculation.LoadOtherDisabilityBenefits();
                ibusParentBenefitCalculation.LoadErrors();
                ibusParentBenefitCalculation.CalculateMinimumGuaranteedMemberAccount();
                //Load the required benefit options, Benefit Payee, FAS , RHIC and person account details.
            }
        }
        public void LoadBenefitCalculationPayeeApplicationInfo()
        {
            //This method updates the application info for a benefit payee for a calculation 

            if (iclbBenefitCalculationPayee.IsNull())
                LoadBenefitCalculationPayee();

            int lintCurrentApplicationID = 0;
            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                lintCurrentApplicationID = 0;
                lintCurrentApplicationID = lobjPayee.icdoBenefitCalculationPayee.benefit_application_id;
                busBenefitApplication lobjCurrentBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };

                if ((lobjPayee.icdoBenefitCalculationPayee.payee_person_id > 0))
                {
                    lobjCurrentBenefitApplication = GetPSTDApplication(lobjPayee.icdoBenefitCalculationPayee.payee_person_id, 0);
                    if (lintCurrentApplicationID != lobjCurrentBenefitApplication.icdoBenefitApplication.benefit_application_id)
                    {
                        lobjPayee.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lobjPayee.ibusBenefitApplication = lobjCurrentBenefitApplication;
                        lobjPayee.icdoBenefitCalculationPayee.benefit_application_id =
                                        lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id;                      
                        if (lobjPayee.icdoBenefitCalculationPayee.benefit_application_id > 0)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value =
                                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_value;
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_description =
                                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_description;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_value =
                                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_value;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_description =
                                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_description;

                        }
                    }
                }
                else if ((lobjPayee.icdoBenefitCalculationPayee.payee_org_id > 0))
                {
                    lobjCurrentBenefitApplication = GetPSTDApplication(0, lobjPayee.icdoBenefitCalculationPayee.payee_org_id);

                    if (lintCurrentApplicationID != lobjCurrentBenefitApplication.icdoBenefitApplication.benefit_application_id)
                    {
                        lobjPayee.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lobjPayee.ibusBenefitApplication = lobjCurrentBenefitApplication;
                        lobjPayee.icdoBenefitCalculationPayee.benefit_application_id =
                                        lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id;                        
                        if (lobjPayee.icdoBenefitCalculationPayee.benefit_application_id > 0)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value =
                                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_value;
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_description =
                                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_description;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_value =
                                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_value;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_description =
                                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_description;

                        }
                    }
                }
            }
        }

        public void LoadBenefitCalculationPayeeForNewMode()
        {
            // If (Reason_Type_Value = 'Account Owner Death')
            //      If it is Monthly then either spouse can get it or the amount is split up into beneficiaries.
            //      If it is Refund then we have to load all his beneficiaries and their application information
            // If(Reason_Type_Value ='First beneficiary Death')
            //      Only Option possible is Refund. we have to load all his beneficiaries and their application information
            // If(Reason_Type_Value = 'Alternate Payee Death')
            //      Only Option possible is Monthly. we have to load all his beneficiaries and their application information and create payee accounts monthly.

            iclbBenefitCalculationPayee = new Collection<busBenefitCalculationPayee>();
            if (icdoPSTDBenefitOptionRef.IsNull())
                LoadPSTDBenefitOptionRef();

            if (icdoPSTDBenefitOptionRef.is_monthly_benefit_flag == busConstant.Flag_No)
            {
                if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath)
                {
                    if (ibusMember.IsNull())
                        LoadMember();
                    iclbBenefitCalculationPayee = GetBenefitCalculationPayee(ibusMember);
                }
	            //UAT PIR:2080 Fix
                else if ((icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementFirstBeneficiaryDeath)
                    ||(icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAlternatePayeeDeath))
                {
                    if (ibusBeneficiaryPerson.IsNull())
                        LoadBeneficiaryPerson();
                    iclbBenefitCalculationPayee = GetBenefitCalculationPayee(ibusBeneficiaryPerson);
                }
            }
            else
            {
                if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath)
                {
                    if (icdoPSTDBenefitOptionRef.account_relation_value == busConstant.AccountRelationshipBeneficiary)
                    {
                        if (ibusMember.IsNull())
                            LoadMember();
                        iclbBenefitCalculationPayee = GetBenefitCalculationPayee(ibusMember);
                    }
                    else
                    {
                        if (ibusBenefitApplication.IsNull())
                            LoadBenefitApplication();
                        if (ibusBenefitApplication.ibusRecipient.IsNull())
                            ibusBenefitApplication.LoadRecipient();
                        if (ibusBenefitApplication.ibusPerson.IsNull())
                            ibusBenefitApplication.LoadPerson();
                        if (ibusBenefitApplication.ibusPerson.iclbActiveBeneForGivenPlan.IsNull())
                            ibusBenefitApplication.ibusPerson.LoadActiveBeneForGivenPlan(
                                ibusPlan.icdoPlan.plan_id,
                                icdoBenefitCalculation.date_of_death);

                        // Loads the busBenefitCalculationPayee only for the current Recipient.
                        // Hence the common method cannot be used.
                        foreach (busPersonBeneficiary lobjBeneficiary in ibusBenefitApplication.ibusPerson.iclbActiveBeneForGivenPlan)
                        {
                            if ((lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id == ibusBenefitApplication.icdoBenefitApplication.recipient_person_id) &&
                                (lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id != 0))
                            {
                                busBenefitCalculationPayee lobjPayee = new busBenefitCalculationPayee { icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee() };
                                lobjPayee.icdoBenefitCalculationPayee.payee_person_id = ibusBenefitApplication.ibusRecipient.icdoPerson.person_id;
                                lobjPayee.icdoBenefitCalculationPayee.payee_name = ibusBenefitApplication.ibusRecipient.icdoPerson.PersonIdWithName;
                                lobjPayee.icdoBenefitCalculationPayee.benefit_percentage = lobjBeneficiary.icdoPersonBeneficiary.beneficiary_percentage;
                                lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth =
                                                ibusBenefitApplication.ibusRecipient.icdoPerson.date_of_birth;
                                lobjPayee.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                                lobjPayee.ibusBenefitApplication = GetPSTDApplication(ibusBenefitApplication.ibusRecipient.icdoPerson.person_id, 0);
                                lobjPayee.icdoBenefitCalculationPayee.benefit_application_id =
                                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id;
                                if (lobjPayee.icdoBenefitCalculationPayee.benefit_application_id == 0)
                                {
                                    lobjPayee.icdoBenefitCalculationPayee.account_relationship_description = ibusBenefitApplication.icdoBenefitApplication.account_relationship_description;
                                    lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = ibusBenefitApplication.icdoBenefitApplication.account_relationship_value;
                                    lobjPayee.icdoBenefitCalculationPayee.family_relationship_description = lobjBeneficiary.icdoPersonBeneficiary.relationship_description;
                                    lobjPayee.icdoBenefitCalculationPayee.family_relationship_value = lobjBeneficiary.icdoPersonBeneficiary.relationship_value;
                                }
                                else
                                {
                                    lobjPayee.icdoBenefitCalculationPayee.account_relationship_value =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_value;
                                    lobjPayee.icdoBenefitCalculationPayee.account_relationship_description =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_description;
                                    lobjPayee.icdoBenefitCalculationPayee.family_relationship_value =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_value;
                                    lobjPayee.icdoBenefitCalculationPayee.family_relationship_description =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_description;

									
                                    lobjPayee.ibusBenefitApplication.LoadPayeeAccount();
                                    if (lobjPayee.ibusBenefitApplication.iclbPayeeAccount.IsNotNull())
                                    {
                                        var lobjvarPayee = lobjPayee.ibusBenefitApplication.iclbPayeeAccount.
                                        Where(o => o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath).FirstOrDefault();

                                        if (lobjvarPayee.IsNotNull())
                                        {
                                            lobjPayee.icdoBenefitCalculationPayee.payee_account_id = lobjvarPayee.icdoPayeeAccount.payee_account_id;
                                        }
                                    }
                                }

                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option =
                                                ibusBenefitApplication.icdoBenefitApplication.benefit_option_value;
                                iclbBenefitCalculationPayee.Add(lobjPayee);
                            }
                            else if ((lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id == ibusBenefitApplication.icdoBenefitApplication.payee_org_id)
                                && (lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id != 0))
                            {
                                busBenefitCalculationPayee lobjPayee = new busBenefitCalculationPayee { icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee() };
                                lobjPayee.icdoBenefitCalculationPayee.payee_org_id = ibusBenefitApplication.icdoBenefitApplication.payee_org_id;
                                lobjPayee.icdoBenefitCalculationPayee.payee_name = busGlobalFunctions.GetOrgNameByOrgID(
                                                                        ibusBenefitApplication.icdoBenefitApplication.payee_org_id);
                                lobjPayee.icdoBenefitCalculationPayee.benefit_percentage = lobjBeneficiary.icdoPersonBeneficiary.beneficiary_percentage;
                                lobjPayee.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                                lobjPayee.ibusBenefitApplication = GetPSTDApplication(0, ibusBenefitApplication.icdoBenefitApplication.payee_org_id);
                                lobjPayee.icdoBenefitCalculationPayee.benefit_application_id =
                                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id;
                                if (lobjPayee.icdoBenefitCalculationPayee.benefit_application_id == 0)
                                {
                                    lobjPayee.icdoBenefitCalculationPayee.account_relationship_description = ibusBenefitApplication.icdoBenefitApplication.account_relationship_description;
                                    lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = ibusBenefitApplication.icdoBenefitApplication.account_relationship_value;
                                    lobjPayee.icdoBenefitCalculationPayee.family_relationship_description = lobjBeneficiary.icdoPersonBeneficiary.relationship_description;
                                    lobjPayee.icdoBenefitCalculationPayee.family_relationship_value = lobjBeneficiary.icdoPersonBeneficiary.relationship_value;
                                }
                                else
                                {
                                    lobjPayee.icdoBenefitCalculationPayee.account_relationship_value =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_value;
                                    lobjPayee.icdoBenefitCalculationPayee.account_relationship_description =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_description;
                                    lobjPayee.icdoBenefitCalculationPayee.family_relationship_value =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_value;
                                    lobjPayee.icdoBenefitCalculationPayee.family_relationship_description =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_description;

                                    lobjPayee.ibusBenefitApplication.LoadPayeeAccount();

                                    if (lobjPayee.ibusBenefitApplication.iclbPayeeAccount.IsNotNull())
                                    {
                                        var lobjvarPayee = lobjPayee.ibusBenefitApplication.iclbPayeeAccount.
                                        Where(o => o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath).FirstOrDefault();

                                        if (lobjvarPayee.IsNotNull())
                                        {
                                            lobjPayee.icdoBenefitCalculationPayee.payee_account_id = lobjvarPayee.icdoPayeeAccount.payee_account_id;
                                        }
                                    }
                                }                               

                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option =
                                                lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option;
                                iclbBenefitCalculationPayee.Add(lobjPayee);
                            }
                        }
                    }
                }
                else if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                {
                    if (ibusOriginatingPayeeAccount.ibusPayee.IsNull())
                        ibusOriginatingPayeeAccount.LoadPayee();
                    iclbBenefitCalculationPayee = GetBenefitCalculationPayee(ibusOriginatingPayeeAccount.ibusPayee);
                }
            }
        }

        /// Loads the Active Beneficiary of the Given Person.
        /// Returns the busBenefitCalculationPayee Collection for the Active Beneficiaries.
        private Collection<busBenefitCalculationPayee> GetBenefitCalculationPayee(busPerson aobjPerson)
        {
            Collection<busBenefitCalculationPayee> lclbBenefitCalculationPayee = new Collection<busBenefitCalculationPayee>();
            if (ibusOriginatingPayeeAccount == null)
                LoadOriginatingPayeeAccount();

            if (aobjPerson.IsNotNull())
            {
                if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath)
                {
                    if (aobjPerson.iclbActiveBeneForGivenPlan.IsNull())
                        aobjPerson.LoadActiveBeneForGivenPlan(ibusPlan.icdoPlan.plan_id, icdoBenefitCalculation.date_of_death);
                }
                else if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementFirstBeneficiaryDeath)
                {
                    if (aobjPerson.iclbPersonBeneficiary == null)
                        aobjPerson.LoadApplicationBeneficiary();

                    Collection<busPersonBeneficiary> lclbApplicationBeneficiary = new Collection<busPersonBeneficiary>();
                    foreach (busPersonBeneficiary lobjPersonBeneficiary in aobjPerson.iclbPersonBeneficiary)
                    {
                        if ((lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.benefit_application_id ==
                            ibusOriginatingPayeeAccount.icdoPayeeAccount.application_id) && 
                            (lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id == 0)
                            && (lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value==busConstant.BeneficiaryMemberTypePrimary))
                        {
                            lclbApplicationBeneficiary.Add(lobjPersonBeneficiary);
                        }
                    }
                    aobjPerson.iclbActiveBeneForGivenPlan = lclbApplicationBeneficiary;
                }
                else if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                {
                    if (aobjPerson.iclbPersonBeneficiary == null)
                        aobjPerson.LoadApplicationBeneficiary();

                    Collection<busPersonBeneficiary> lclbDROBeneficiary = new Collection<busPersonBeneficiary>();
                    foreach (busPersonBeneficiary lobjPersonBeneficiary in aobjPerson.iclbPersonBeneficiary)
                    {
                        if ((lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id ==
                            ibusOriginatingPayeeAccount.icdoPayeeAccount.dro_application_id)
                            && (lobjPersonBeneficiary.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_type_value==busConstant.BeneficiaryMemberTypePrimary))
                        {
                            lclbDROBeneficiary.Add(lobjPersonBeneficiary);
                        }
                    }
                    aobjPerson.iclbActiveBeneForGivenPlan = lclbDROBeneficiary;
                }

                foreach (busPersonBeneficiary lobjBeneficiary in aobjPerson.iclbActiveBeneForGivenPlan)
                {
                    if (lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id != 0)
                    {
                        if (lobjBeneficiary.ibusBeneficiaryPerson.IsNull())
                            lobjBeneficiary.LoadBeneficiaryPerson();
                        busBenefitCalculationPayee lobjPayee = new busBenefitCalculationPayee { icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee() };
                        lobjPayee.icdoBenefitCalculationPayee.payee_person_id = lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id;
                        lobjPayee.icdoBenefitCalculationPayee.payee_name = lobjBeneficiary.ibusBeneficiaryPerson.icdoPerson.PersonIdWithName;
                        if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementFirstBeneficiaryDeath)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.benefit_percentage = aobjPerson.iclbPersonBeneficiary.Where(lobjPA => lobjPA.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.benefit_application_id == ibusOriginatingPayeeAccount.icdoPayeeAccount.application_id &&
                                lobjPA.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_id == lobjBeneficiary.icdoPersonBeneficiary.beneficiary_id).FirstOrDefault().
                                ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dist_percent;
                        }
                        else if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.benefit_percentage = aobjPerson.iclbPersonBeneficiary.Where(lobjPA => lobjPA.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id == ibusOriginatingPayeeAccount.icdoPayeeAccount.dro_application_id &&
                                lobjPA.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_id == lobjBeneficiary.icdoPersonBeneficiary.beneficiary_id).FirstOrDefault().ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dist_percent;
                        }
                        else
                        {
                            lobjPayee.icdoBenefitCalculationPayee.benefit_percentage = lobjBeneficiary.icdoPersonBeneficiary.beneficiary_percentage;
                        }
                        lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth = lobjBeneficiary.ibusBeneficiaryPerson.icdoPerson.date_of_birth;
                        lobjPayee.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lobjPayee.ibusBenefitApplication = GetPSTDApplication(lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id, 0);
                        lobjPayee.icdoBenefitCalculationPayee.benefit_application_id =
                                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id;
                        if (lobjPayee.icdoBenefitCalculationPayee.benefit_application_id == 0)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_description = ibusBenefitApplication.icdoBenefitApplication.account_relationship_description;
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = ibusBenefitApplication.icdoBenefitApplication.account_relationship_value;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_description = lobjBeneficiary.icdoPersonBeneficiary.relationship_description;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_value = lobjBeneficiary.icdoPersonBeneficiary.relationship_value;
                        }
                        else
                        {
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_value;
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_description =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_description;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_value =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_value;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_description =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_description;


                            lobjPayee.ibusBenefitApplication.LoadPayeeAccount();
                            if (lobjPayee.ibusBenefitApplication.iclbPayeeAccount.IsNotNull())
                            {
                                var lobjvarPayee = lobjPayee.ibusBenefitApplication.iclbPayeeAccount.
                                Where(o => o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath).FirstOrDefault();

                                if (lobjvarPayee.IsNotNull())
                                {
                                    lobjPayee.icdoBenefitCalculationPayee.payee_account_id = lobjvarPayee.icdoPayeeAccount.payee_account_id;
                                }
                            }

                        }
                        lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option =
                                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_option_value;
                        lclbBenefitCalculationPayee.Add(lobjPayee);
                    }
                    else if (lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id != 0)
                    {
                        busBenefitCalculationPayee lobjPayee = new busBenefitCalculationPayee { icdoBenefitCalculationPayee = new cdoBenefitCalculationPayee() };
                        lobjPayee.icdoBenefitCalculationPayee.payee_org_id = lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id;
                        lobjPayee.icdoBenefitCalculationPayee.payee_name = busGlobalFunctions.GetOrgNameByOrgID(
                                                lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id);
                        if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementFirstBeneficiaryDeath)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.benefit_percentage = aobjPerson.iclbPersonBeneficiary.Where(lobjPA => lobjPA.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.benefit_application_id == ibusOriginatingPayeeAccount.icdoPayeeAccount.application_id &&
                                lobjPA.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_id == lobjBeneficiary.icdoPersonBeneficiary.beneficiary_id).FirstOrDefault().
                                ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dist_percent; 
                        }
                        else if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.benefit_percentage = aobjPerson.iclbPersonBeneficiary.Where(lobjPA => lobjPA.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dro_application_id== ibusOriginatingPayeeAccount.icdoPayeeAccount.dro_application_id &&
                                lobjPA.ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.beneficiary_id == lobjBeneficiary.icdoPersonBeneficiary.beneficiary_id).FirstOrDefault().
                                ibusBenefitApplicationBeneficiary.icdoBenefitApplicationBeneficiary.dist_percent; 
                        }
                        else
                        {
                            lobjPayee.icdoBenefitCalculationPayee.benefit_percentage = lobjBeneficiary.icdoPersonBeneficiary.beneficiary_percentage;
                        }
                        lobjPayee.ibusBenefitApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                        lobjPayee.ibusBenefitApplication = GetPSTDApplication(0, lobjBeneficiary.icdoPersonBeneficiary.benificiary_org_id);
                        lobjPayee.icdoBenefitCalculationPayee.benefit_application_id =
                                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id;
                        if (lobjPayee.icdoBenefitCalculationPayee.benefit_application_id == 0)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_description = ibusBenefitApplication.icdoBenefitApplication.account_relationship_description;
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value = ibusBenefitApplication.icdoBenefitApplication.account_relationship_value;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_description = lobjBeneficiary.icdoPersonBeneficiary.relationship_description;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_value = lobjBeneficiary.icdoPersonBeneficiary.relationship_value;
                        }
                        else
                        {
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_value =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_value;
                            lobjPayee.icdoBenefitCalculationPayee.account_relationship_description =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.account_relationship_description;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_value =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_value;
                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_description =
                                                    lobjPayee.ibusBenefitApplication.icdoBenefitApplication.family_relationship_description;


                            lobjPayee.ibusBenefitApplication.LoadPayeeAccount();
                            if (lobjPayee.ibusBenefitApplication.iclbPayeeAccount.IsNotNull())
                            {
                                var lobjvarPayee = lobjPayee.ibusBenefitApplication.iclbPayeeAccount.
                                Where(o => o.icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath).FirstOrDefault();

                                if (lobjvarPayee.IsNotNull())
                                {
                                    lobjPayee.icdoBenefitCalculationPayee.payee_account_id = lobjvarPayee.icdoPayeeAccount.payee_account_id;
                                }
                            }

                        }
                        lobjPayee.icdoBenefitCalculationPayee.payee_benefit_option =
                                                lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_option_value;
                        lclbBenefitCalculationPayee.Add(lobjPayee);
                    }
                }
            }
            return lclbBenefitCalculationPayee;
        }

        // Is Benefit Application Alreadty exists for the Person
        private busBenefitApplication GetPSTDApplication(int aintPersonID, int aintOrgID)
        {
            busBenefitApplication lobjPSTDApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
            DataTable ldtbApplication = new DataTable();
            if (aintPersonID != 0)
            {
                ldtbApplication = busBase.SelectWithOperator<cdoBenefitApplication>(
                                                new string[7] { "MEMBER_PERSON_ID", "RECIPIENT_PERSON_ID", "PLAN_ID", 
                                                                "ORIGINATING_PAYEE_ACCOUNT_ID","POST_RETIREMENT_DEATH_REASON_TYPE_VALUE","ACTION_STATUS_VALUE","ACTION_STATUS_VALUE" },
                                                new string[7] { "=", "=", "=", "=", "=", "<>", "<>" },
                                                new object[7] { icdoBenefitCalculation.person_id, aintPersonID,ibusPlan.icdoPlan.plan_id,
                                                                ibusOriginatingPayeeAccount.icdoPayeeAccount.payee_account_id,
                                                                icdoBenefitCalculation.post_retirement_death_reason_type_value,
                                                                busConstant.ApplicationActionStatusCancelled,
                                                                busConstant.ApplicationActionStatusDenied}, null);
            }
            else
            {
                ldtbApplication = busBase.SelectWithOperator<cdoBenefitApplication>(
                                                new string[7] { "MEMBER_PERSON_ID", "PAYEE_ORG_ID", "PLAN_ID", "ORIGINATING_PAYEE_ACCOUNT_ID", 
                                                                "POST_RETIREMENT_DEATH_REASON_TYPE_VALUE","ACTION_STATUS_VALUE","ACTION_STATUS_VALUE" },
                                                new string[7] { "=", "=", "=", "=", "=", "<>", "<>" },
                                                new object[7] { icdoBenefitCalculation.person_id, aintOrgID,ibusPlan.icdoPlan.plan_id,
                                                                ibusOriginatingPayeeAccount.icdoPayeeAccount.payee_account_id , 
                                                                icdoBenefitCalculation.post_retirement_death_reason_type_value,
                                                                busConstant.ApplicationActionStatusCancelled,
                                                                busConstant.ApplicationActionStatusDenied}, null);
            }
            foreach (DataRow ldtr in ldtbApplication.Rows)
            {
                lobjPSTDApplication = new busBenefitApplication { icdoBenefitApplication = new cdoBenefitApplication() };
                lobjPSTDApplication.icdoBenefitApplication.LoadData(ldtr);
                return lobjPSTDApplication;
            }
            return lobjPSTDApplication;
        }

        public decimal GetFirstBeneficiaryPercentage()
        {
            decimal ldecBeneficiaryPercentage = 0M;
            if (ibusPlan.IsNull())
                LoadPlan();
            if (ibusOriginatingPayeeAccount.ibusMember.IsNull())
                ibusOriginatingPayeeAccount.LoadMember();
            if (ibusOriginatingPayeeAccount.ibusPayee.IsNull())
                ibusOriginatingPayeeAccount.LoadPayee();
            if (ibusOriginatingPayeeAccount.ibusMember.iclbActiveBeneForGivenPlan.IsNull())
                ibusOriginatingPayeeAccount.ibusMember.LoadActiveBeneForGivenPlan(
                    ibusPlan.icdoPlan.plan_id,
                    icdoBenefitCalculation.date_of_death);

            foreach (busPersonBeneficiary lobjBeneficiary in ibusOriginatingPayeeAccount.ibusMember.iclbActiveBeneForGivenPlan)
            {
                if (lobjBeneficiary.icdoPersonBeneficiary.beneficiary_person_id == icdoBenefitCalculation.beneficiary_person_id)
                {
                    ldecBeneficiaryPercentage = lobjBeneficiary.icdoPersonBeneficiary.beneficiary_percentage;
                }
            }
            return ldecBeneficiaryPercentage;
        }

        public ArrayList btnApprove_Clicked()
        {
            ArrayList larrErrors = new ArrayList();
            utlError lutlError = new utlError();
            // UCS - 54 - BR - 31
            // Commented on 07-30-2010 // UAT PIR ID 1477 // Handled in UCS-079
            //if (IsInValidPaymentHistoryDistributionRecordExists())
            //{
            //    lutlError = AddError(7730, "");
            //    larrErrors.Add(lutlError);
            //    return larrErrors;
            //}
            this.ValidateHardErrors(utlPageMode.All);
            if (larrErrors.Count == 0)
            {
                LoadBenefitCalculationPayee();
                foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                {
                    if (lobjPayee.ibusBenefitApplication.IsNull())
                        lobjPayee.LoadBenefitApplication();

                    // Create New Payee Account iff the Application status Valid/Verified
                    if ((lobjPayee.ibusBenefitApplication.icdoBenefitApplication.action_status_value == busConstant.ApplicationActionStatusVerified) &&
                        (lobjPayee.ibusBenefitApplication.icdoBenefitApplication.status_value == busConstant.ApplicationStatusValid))
                    {
                        // 1.End the Originating Payee Account
                        if (ibusOriginatingPayeeAccount.IsNull())
                            LoadOriginatingPayeeAccount();
                        if (ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_end_date == DateTime.MinValue)
                        {
                            ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_end_date = icdoBenefitCalculation.date_of_death;
                            ibusOriginatingPayeeAccount.icdoPayeeAccount.Update();
                        }

                        // 2.Load Benefit Option Corresponding to this Payee.
                        busBenefitCalculationOptions lobjPayeeBenefitOption =
                                                new busBenefitCalculationOptions { icdoBenefitCalculationOptions = new cdoBenefitCalculationOptions() };
                        LoadBenefitCalculationOptions();
                        if (iclbBenefitCalculationOptions.IsNotNull())
                        {
                            lobjPayeeBenefitOption = iclbBenefitCalculationOptions.Where(o => o.icdoBenefitCalculationOptions.benefit_calculation_payee_id ==
                                    lobjPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id).FirstOrDefault();
                        }

                        int lintPayeeAccountID = 0;
                        decimal ldecTaxableAmount = 0M;
                        decimal ldecNonTaxableAmount = 0M;
                        decimal ldecExclusionRatioAmount = 0M;
                        decimal ldecPayeeMinimumGuaranteeAmount = 0.0M;
                        // 3.Load the values
                        if (lobjPayeeBenefitOption != null)
                        {
                            ldecPayeeMinimumGuaranteeAmount = icdoBenefitCalculation.minimum_guarentee_amount *
                                                ((lobjPayee.icdoBenefitCalculationPayee.benefit_percentage) / 100M);

                            //if Payee Minimum guarantee Amount is less than 0 then set it 0. As per discussion with Satya.
                            if (ldecPayeeMinimumGuaranteeAmount < 0)
                            {
                                ldecPayeeMinimumGuaranteeAmount = 0;
                            }

                            // For Alternate payee Death Alone , the M.G will be 0.As per discussion with Satya.
                            if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAlternatePayeeDeath)
                            {
                                ldecPayeeMinimumGuaranteeAmount = 0;
                            }

                            if (icdoBenefitCalculation.benefit_option_value != busConstant.BenefitOptionRefund)
                            {
                                ldecTaxableAmount = icdoBenefitCalculation.minimum_guarentee_amount_taxable_amount *
                                                ((lobjPayee.icdoBenefitCalculationPayee.benefit_percentage) / 100M); ;
                                ldecNonTaxableAmount = icdoBenefitCalculation.minimum_guarentee_amount_non_taxable_amount *
                                                ((lobjPayee.icdoBenefitCalculationPayee.benefit_percentage) / 100M);
                            }
                            else
                            {
                                ldecTaxableAmount = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.taxable_amount;
                                ldecNonTaxableAmount = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.non_taxable_amount;
                            }
                            if (ldecNonTaxableAmount < 0M)
                                ldecNonTaxableAmount = 0M;
                        }

                        DateTime ldteNextBenefitPaymentDate = busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
						//UAT PIR: 2076
                        DateTime ldteBenefitBeginDate = icdoBenefitCalculation.retirement_date;

                        string lstrBenefitAccountSubType = busConstant.BenefitAccountSubTypePostRetDeath;
                        decimal ldecSpouseRHICAmount = 0M;
                        if (iclbBenefitRHICOption.Count > 0)
                            ldecSpouseRHICAmount = iclbBenefitRHICOption[0].icdoBenefitRhicOption.spouse_rhic_amount;

                        // 4. Creates or Updates the Payee Account
                        if (lobjPayee.icdoBenefitCalculationPayee.payee_person_id == 0)
                        {
                            lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(
                                                        lobjPayee.icdoBenefitCalculationPayee.payee_org_id,
                                                        ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_id,
                                                        lobjPayee.icdoBenefitCalculationPayee.account_relationship_value,
                                                        icdoBenefitCalculation.benefit_account_type_value, true);
                        }
                        else
                        {
                            lintPayeeAccountID = busPayeeAccountHelper.IsPayeeAccountExists(
                                                        lobjPayee.icdoBenefitCalculationPayee.payee_person_id,
                                                        ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_id,
                                                        lobjPayee.icdoBenefitCalculationPayee.account_relationship_value,
                                                        icdoBenefitCalculation.benefit_account_type_value, false);
                        }
                        //The End date should not be equal to the End date of the originating payee accountid, since in this case it will be equal to the date of death, but the benefit begin date would be a future date. So it should be equal to the term certain end date if exists.
                        if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments)
                        {
                            lintPayeeAccountID = busPayeeAccountHelper.ManagePayeeAccount(
                                                            lobjPayee.icdoBenefitCalculationPayee.payee_person_id,
                                                            lobjPayee.icdoBenefitCalculationPayee.payee_org_id,
                                                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id,
                                                            icdoBenefitCalculation.benefit_calculation_id,
                                                            ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_id, 0,
                                                            busConstant.StatusValid, icdoBenefitCalculation.benefit_account_type_value,
                                                            lstrBenefitAccountSubType, busConstant.Flag_No, DateTime.MinValue,
                                                            DateTime.MinValue, lobjPayee.icdoBenefitCalculationPayee.account_relationship_value,
                                                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_value,
                                                            ldecPayeeMinimumGuaranteeAmount, ldecNonTaxableAmount,
                                                            icdoBenefitCalculation.benefit_option_value, ldecSpouseRHICAmount, 
                                                            lintPayeeAccountID, busConstant.PayeeAccountExclusionMethodSimplified, 0.0M,
                                                            ibusOriginatingPayeeAccount.icdoPayeeAccount.term_certain_end_date,
                                                            string.Empty, 0,icdoBenefitCalculation.graduated_benefit_option_value);
                        }
                        else
                        {
                            lintPayeeAccountID = busPayeeAccountHelper.ManagePayeeAccount(
                                                            lobjPayee.icdoBenefitCalculationPayee.payee_person_id,
                                                            lobjPayee.icdoBenefitCalculationPayee.payee_org_id,
                                                            lobjPayee.ibusBenefitApplication.icdoBenefitApplication.benefit_application_id,
                                                            icdoBenefitCalculation.benefit_calculation_id,
                                                            ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_id, 0,
                                                            busConstant.StatusValid, icdoBenefitCalculation.benefit_account_type_value,
                                                            lstrBenefitAccountSubType, busConstant.Flag_No, ldteBenefitBeginDate,
                                                            DateTime.MinValue, lobjPayee.icdoBenefitCalculationPayee.account_relationship_value,
                                                            lobjPayee.icdoBenefitCalculationPayee.family_relationship_value,
                                                            ldecPayeeMinimumGuaranteeAmount, ldecNonTaxableAmount,
                                                            icdoBenefitCalculation.benefit_option_value, ldecSpouseRHICAmount, 
                                                            lintPayeeAccountID, busConstant.PayeeAccountExclusionMethodSimplified, 0.0M,
                                                            ibusOriginatingPayeeAccount.icdoPayeeAccount.term_certain_end_date,
                                                            string.Empty, 0,icdoBenefitCalculation.graduated_benefit_option_value);
                        }

                        if (lintPayeeAccountID != 0)
                        {
                            lobjPayee.icdoBenefitCalculationPayee.payee_account_id = lintPayeeAccountID;

                            busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                            lobjPayeeAccount.FindPayeeAccount(lintPayeeAccountID);
                            if (ibusBaseActivityInstance.IsNotNull())
                            {
                                if (lobjPayeeAccount.ibusBaseActivityInstance.IsNull())
                                    lobjPayeeAccount.ibusBaseActivityInstance = new busSolBpmActivityInstance();
                                lobjPayeeAccount.ibusBaseActivityInstance = ibusBaseActivityInstance;
                                //lobjPayeeAccount.SetProcessInstanceParameters();
                                lobjPayeeAccount.SetCaseInstanceParameters();
                            }
                            if (lobjPayee.ibusBenefitProvisionBenefitOption == null)
                                lobjPayee.LoadBenefitProvisionBenefitOption();

                            // 5. Creates Payee Account Payment Item Type(PAPIT)
                            if (icdoBenefitCalculation.benefit_option_value != busConstant.BenefitOptionRefund)
                            {
                                //decimal ldecSpouseAge = 0M;
                                decimal ldecMonthlyTaxableAmount = 0M;
                                decimal ldecMonthlyNonTaxableAmount = 0M;
                                //CalculatePersonAge(lobjPayee.icdoBenefitCalculationPayee.payee_date_of_birth,
                                //                                icdoBenefitCalculation.date_of_death, ref ldecSpouseAge, 4);
                                //if (lobjPayeeBenefitOption.ibusBenefitProvisionBenefitOption.IsNull())
                                //    lobjPayeeBenefitOption.LoadBenefitProvisionOption();
                                //CalculateMonthlyTaxComponents(icdoBenefitCalculation.created_date, ldecSpouseAge, 0,
                                //        lobjPayeeBenefitOption.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.exclusive_calc_payment_type_value,
                                //        ldecNonTaxableAmount,
                                //        lobjPayeeBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount, 0,
                                //        0, ref ldecMonthlyNonTaxableAmount, ref ldecMonthlyTaxableAmount, ref ldecExclusionRatioAmount,
                                //        lobjPayee.icdoBenefitCalculationPayee.benefit_percentage);

                                //Need not calculate the Exclusion Amount again. get it from the Originating Payee AccountId.As per discussion with Satya.
                                
                                ibusOriginatingPayeeAccount.LoadExclusionAmount();

                                ldecMonthlyNonTaxableAmount = ibusOriginatingPayeeAccount.idecExclusionAmount;

                                if ((ldecNonTaxableAmount < ldecMonthlyNonTaxableAmount) && (ldecNonTaxableAmount >0))
                                {
                                    ldecMonthlyNonTaxableAmount = ldecNonTaxableAmount;
                                }

                                ldecMonthlyTaxableAmount = lobjPayeeBenefitOption.icdoBenefitCalculationOptions.benefit_option_amount - ldecMonthlyNonTaxableAmount;

                                if (ldecMonthlyTaxableAmount > 0M)
                                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxableAmount, ldecMonthlyTaxableAmount, string.Empty, 0,
                                                        ldteNextBenefitPaymentDate, DateTime.MinValue, false);
                                if (ldecMonthlyNonTaxableAmount > 0M)
                                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITTaxalbeAmount, ldecMonthlyNonTaxableAmount, string.Empty, 0,
                                                                            ldteNextBenefitPaymentDate, DateTime.MinValue, false);
                            }
                            else
                            {
                                ldecTaxableAmount = Math.Round(ldecTaxableAmount, 2, MidpointRounding.AwayFromZero);
                                ldecNonTaxableAmount = Math.Round(ldecNonTaxableAmount, 2, MidpointRounding.AwayFromZero);

                                if (ldecTaxableAmount > 0.0M)
                                {
                                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPSTDTaxableAmount,
                                        ldecTaxableAmount, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                }
                                if (ldecNonTaxableAmount > 0.0M)
                                {
                                    lobjPayeeAccount.CreatePayeeAccountPaymentItemType(busConstant.PAPITPSTDNonTaxableAmount,
                                        ldecNonTaxableAmount, "", 0, ldteBenefitBeginDate, DateTime.MinValue, false);
                                }
                            }
                            // 6.Make the Payee Account Status to Review
                            lobjPayeeAccount.CreateReviewPayeeAccountStatus();
                            lobjPayee.icdoBenefitCalculationPayee.Update();

                            //UCS - 079 : If calculation type is Adjustments, then need to create Underpayment
                            //--Start of code --//
                            if (icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal)
                            {
                                //if at all we are passing recal adjust reason as recalculation, only initial retro will be created
                                lobjPayeeAccount.CreateBenefitOverPaymentorUnderPayment(busConstant.BenRecalAdjustmentReasonRecalculation);
                            }
                            //--End of code --//
                            if (icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption50PercentJS
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption100PercentJS
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption75Percent
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption55Percent
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOption50Percent
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionSpouseBenefit
                                || icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionMonthlyLifeTimeBenefit)
                                CreateRHICCombineOnCalculationApproval(lobjPayeeAccount, icdoBenefitCalculation.calculation_type_value);
                        }
                    }
                }

                // 7.Change Calculation Action Status to Approved.
                bool lblnPayeeAccountCreatedForAllPayee = true;
                foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
                {
                    if (lobjPayee.icdoBenefitCalculationPayee.payee_account_id == 0)
                    {
                        lblnPayeeAccountCreatedForAllPayee = false;
                        break;
                    }
                }
                if (lblnPayeeAccountCreatedForAllPayee)
                {
                    icdoBenefitCalculation.approved_by = iobjPassInfo.istrUserID;
                    icdoBenefitCalculation.action_status_value = busConstant.BenefitActionStatusApproved;
                    icdoBenefitCalculation.Update();
                }

                //PIR 18974
                IsBenefitOverpaymentBPMInitiate();
            }
            else
            {
                foreach (utlError lobjError in iarrErrors)
                {
                    larrErrors.Add(lobjError);
                }
            }
            return larrErrors;
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            if (aenmPageMode == utlPageMode.New)
            {
                iblnIsNewMode = true;
            }
            else
            {
                iblnIsNewMode = false;
            }
        }

        public override void BeforePersistChanges()
        {
            if (icdoBenefitCalculation.suppress_warnings_flag == busConstant.Flag_Yes)
                icdoBenefitCalculation.suppress_warnings_by = iobjPassInfo.istrUserID;

            DeleteBenefitCalculationDetails();
            DeleteBenefitCalculationPayeeDetails();

            // Calculate Benefit amount
            CalculatePostRetirementDeathBenefit();

            base.BeforePersistChanges();
        }

        public override int PersistChanges()
        {
            base.PersistChanges();
            if (iblnIsNewMode)
            {
                if (ibusPersonAccount == null)
                    LoadPersonAccount();
                if (ibusPersonAccount.icdoPersonAccount.person_account_id > 0)
                {
                    // Insert the Benefit Calculation Person Account only once in case of Insert.
                    cdoBenefitCalculationPersonAccount lcdoBenCalcPersonAccount = new cdoBenefitCalculationPersonAccount
                    {
                        benefit_calculation_id = icdoBenefitCalculation.benefit_calculation_id,
                        person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id
                    };
                    lcdoBenCalcPersonAccount.Insert();
                }
            }
            CreateBenefitCalculationDetails();
            CreateBenefitCalculationPayeeDetails();
            return 1;
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();

            // Insert the Parent Benefit Calculation for the specific case RTW
            if (IsRTW() && iblnIsNewMode)
            {
                ReCalculateRTWRetirementBenefit();
                ibusParentBenefitCalculation.icdoBenefitCalculation.is_calculation_visible_flag = busConstant.Flag_No;                
                ibusParentBenefitCalculation.BeforeValidate(utlPageMode.New);
                ibusParentBenefitCalculation.BeforePersistChanges();
                ibusParentBenefitCalculation.PersistChanges();
                ibusParentBenefitCalculation.AfterPersistChanges();
                icdoBenefitCalculation.minimum_guarentee_amount = ibusParentBenefitCalculation.icdoBenefitCalculation.minimum_guarentee_amount; // PIR ID 2179
                icdoBenefitCalculation.final_monthly_benefit = ibusParentBenefitCalculation.icdoBenefitCalculation.final_monthly_benefit;
                icdoBenefitCalculation.parent_benefit_calculation_id = ibusParentBenefitCalculation.icdoBenefitCalculation.benefit_calculation_id;
                icdoBenefitCalculation.Update();
            }

            EvaluateInitialLoadRules();

            // Refresh Payee Grid From DB
            LoadBenefitCalculationPayee();
            foreach (busBenefitCalculationPayee lobjPayee in iclbBenefitCalculationPayee)
            {
                if (lobjPayee.ibusPayee.IsNull())
                    lobjPayee.LoadPayee();

                if (lobjPayee.ibusBenefitApplication.IsNull())
                    lobjPayee.LoadBenefitApplication();
            }

            // Refresh Benefit Options Grid From DB
            int lintCounter = 0;
            foreach (busBenefitCalculationOptions lobjBenOptions in iclbBenefitCalculationOptions)
            {
                if (iclbBenefitCalculationPayee.Count >= lintCounter)
                {
                    lobjBenOptions.icdoBenefitCalculationOptions.benefit_calculation_payee_id =
                        iclbBenefitCalculationPayee[lintCounter].icdoBenefitCalculationPayee.benefit_calculation_payee_id;
                    lobjBenOptions.icdoBenefitCalculationOptions.Update();
                }
                lobjBenOptions.LoadBenefitCalculationPayee();
                if (lobjBenOptions.ibusBenefitCalculationPayee.ibusPayee.IsNull())
                    lobjBenOptions.ibusBenefitCalculationPayee.LoadPayee();
                lobjBenOptions.icdoBenefitCalculationOptions.payee_name = lobjBenOptions.ibusBenefitCalculationPayee.ibusPayee.icdoPerson.PersonIdWithName;
                lintCounter++;
            }
            LoadBenefitCalculationOptions();
            LoadBenefitRHICOption();
            LoadMemberTerminationDate();

            if (ibusBaseActivityInstance != null)
            {
                busBpmActivityInstance lbusActivityInstance = (busBpmActivityInstance)ibusBaseActivityInstance;
                if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_Recalculate_Pension_and_RHIC_Benefit)
                {
                    lbusActivityInstance.UpdateParameter("calculation_reference_id", icdoBenefitCalculation.benefit_calculation_id.ToString());
                }
                if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_PSTD_Account_Owner_Application)
                {
                    lbusActivityInstance.UpdateParameter("calculation_reference_id", icdoBenefitCalculation.benefit_calculation_id.ToString());
                }
                //PIR 26112 - As discussed with Maik, BPM was opening wrong calculation ID. 
                if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_PSTD_Alternate_Payee_Application)
                {
                    lbusActivityInstance.UpdateParameter("calculation_reference_id", icdoBenefitCalculation.benefit_calculation_id.ToString());
                }
                if (busWorkflowHelper.GetWorkflowProcessIdByBpmProcessName(lbusActivityInstance.ibusBpmProcessInstance.ibusBpmProcess.icdoBpmProcess.name) == busConstant.Map_PSTD_First_Beneficiary_Application)
                {
                    lbusActivityInstance.UpdateParameter("calculation_reference_id", icdoBenefitCalculation.benefit_calculation_id.ToString());
                }
            }
        }

        // Loads the Calculation object from the Given Post-Retirement Death Application.
        public void GetCalculationByApplication(busPostRetirementDeathApplication aobjPostRetirementDeathApplication)
        {
            icdoBenefitCalculation.originating_payee_account_id = aobjPostRetirementDeathApplication.icdoBenefitApplication.originating_payee_account_id;
            icdoBenefitCalculation.post_retirement_death_reason_type_value =
                aobjPostRetirementDeathApplication.icdoBenefitApplication.post_retirement_death_reason_type_value;
            icdoBenefitCalculation.person_id = aobjPostRetirementDeathApplication.icdoBenefitApplication.member_person_id;
            if (ibusMember == null)
                LoadMember();
            if (ibusMember.icolPersonEmployment.IsNull())
                ibusMember.LoadPersonEmployment();
            icdoBenefitCalculation.termination_date = aobjPostRetirementDeathApplication.icdoBenefitApplication.termination_date;
            icdoBenefitCalculation.beneficiary_person_id = aobjPostRetirementDeathApplication.icdoBenefitApplication.beneficiary_person_id;
            LoadBeneficiaryPerson();
            if (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath)
            {
                icdoBenefitCalculation.date_of_death = ibusMember.icdoPerson.date_of_death;
            }
            else
            {
                icdoBenefitCalculation.date_of_death = ibusBeneficiaryPerson.icdoPerson.date_of_death;
            }                           
            
            icdoBenefitCalculation.retirement_date = aobjPostRetirementDeathApplication.icdoBenefitApplication.retirement_date;
            icdoBenefitCalculation.plan_id = aobjPostRetirementDeathApplication.icdoBenefitApplication.plan_id;
            if (ibusPlan == null)
                LoadPlan();
            icdoBenefitCalculation.benefit_account_type_value = aobjPostRetirementDeathApplication.icdoBenefitApplication.benefit_account_type_value;
            icdoBenefitCalculation.benefit_account_type_description = aobjPostRetirementDeathApplication.icdoBenefitApplication.benefit_account_type_description;
            icdoBenefitCalculation.benefit_option_value = aobjPostRetirementDeathApplication.icdoBenefitApplication.benefit_option_value;
            icdoBenefitCalculation.benefit_option_description = aobjPostRetirementDeathApplication.icdoBenefitApplication.benefit_option_description;
            //icdoBenefitCalculation.calculation_type_value = busConstant.CalculationTypeFinal;
            //icdoBenefitCalculation.calculation_type_description =
            //    iobjPassInfo.isrvDBCache.GetCodeDescriptionString(2303, busConstant.CalculationTypeFinal);
            icdoBenefitCalculation.benefit_application_id = aobjPostRetirementDeathApplication.icdoBenefitApplication.benefit_application_id;
            icdoBenefitCalculation.plso_requested_flag = aobjPostRetirementDeathApplication.icdoBenefitApplication.plso_requested_flag;
            icdoBenefitCalculation.ssli_or_uniform_income_commencement_age = aobjPostRetirementDeathApplication.icdoBenefitApplication.ssli_age;
            icdoBenefitCalculation.uniform_income_or_ssli_flag = aobjPostRetirementDeathApplication.icdoBenefitApplication.uniform_income_flag;
            icdoBenefitCalculation.estimated_ssli_benefit_amount = aobjPostRetirementDeathApplication.icdoBenefitApplication.estimated_ssli_benefit_amount;
            icdoBenefitCalculation.reduced_benefit_flag = aobjPostRetirementDeathApplication.icdoBenefitApplication.reduced_benefit_flag;
            icdoBenefitCalculation.rhic_option_value = aobjPostRetirementDeathApplication.icdoBenefitApplication.rhic_option_value;
            icdoBenefitCalculation.rhic_option_description = aobjPostRetirementDeathApplication.icdoBenefitApplication.rhic_option_description;
            icdoBenefitCalculation.annuitant_id = aobjPostRetirementDeathApplication.icdoBenefitApplication.joint_annuitant_perslink_id;

            iintRetirementOrgId = aobjPostRetirementDeathApplication.icdoBenefitApplication.retirement_org_id;

            //PROD PIR ID 7101
            icdoBenefitCalculation.benefit_account_sub_type_value = aobjPostRetirementDeathApplication.icdoBenefitApplication.benefit_sub_type_value;
            icdoBenefitCalculation.benefit_account_sub_type_description = aobjPostRetirementDeathApplication.icdoBenefitApplication.benefit_sub_type_description;
        }

        public void CalculateSpouseAge()
        {
            if (ibusBenefitApplication.IsNull())
                LoadBenefitApplication();
            if ((ibusBenefitApplication.icdoBenefitApplication.account_relationship_value == busConstant.AccountRelationshipJointAnnuitant) &&
                (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath))
            {
                if (ibusBenefitApplication.IsNull())
                    LoadBenefitApplication();
                icdoBenefitCalculation.annuitant_id = ibusBenefitApplication.icdoBenefitApplication.recipient_person_id;
                CalculateAnnuitantAge();
            }
        }

        /// Method no longer in use. Validation handled in UCS-079
        public bool IsInValidPaymentHistoryDistributionRecordExists()
        {
            if (ibusOriginatingPayeeAccount.IsNull())
                LoadOriginatingPayeeAccount();

            if (ibusOriginatingPayeeAccount.ibusPayee.IsNull())
                ibusOriginatingPayeeAccount.LoadPayee();

            if (ibusOriginatingPayeeAccount.iclbPaymentHistoryHeader.IsNull())
                ibusOriginatingPayeeAccount.LoadPaymentHistoryHeader();

            if (ibusOriginatingPayeeAccount.iclbPaymentRecovery.IsNull())
                ibusOriginatingPayeeAccount.LoadPaymentRecovery();

            var ldecNonSatisfiedRecoveryAmount = ibusOriginatingPayeeAccount.iclbPaymentRecovery
                                                .Where(lobjReco => (lobjReco.icdoPaymentRecovery.status_value != busConstant.RecoveryStatusSatisfied) ||
                                                                    (lobjReco.icdoPaymentRecovery.status_value != busConstant.RecoveryStatusWriteOff)) // PIR ID 2204
                                                .Sum(lobjRec => lobjRec.icdoPaymentRecovery.recovery_amount);

            foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in ibusOriginatingPayeeAccount.iclbPaymentHistoryHeader)
            {
                if (lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_date > ibusOriginatingPayeeAccount.ibusPayee.icdoPerson.date_of_death)
                {
                    if (lobjPaymentHistoryHeader.iclbPaymentHistoryDistribution.IsNull())
                        lobjPaymentHistoryHeader.LoadPaymentHistoryDistribution();
                    
                    int lintNonCancelledCount = lobjPaymentHistoryHeader.iclbPaymentHistoryDistribution.Where(lobj => lobj.IsNotCancelledStatus()).Count(); // SYS PIR ID 2204
                    int lintReceivableCount = lobjPaymentHistoryHeader.iclbPaymentHistoryDistribution.Where(lobj => lobj.isReceivableCreatedStatus()).Count();
                    if ((lintNonCancelledCount > 0) || ((ldecNonSatisfiedRecoveryAmount > 0.00M) && (lintReceivableCount > 0)))
                        return true;
                }
            }
            return false;
        }

        public int iintRTWPayeeAccountID { get; set; }

        public bool IsRTW()
        {
            int lintPreRTWPayeeAccountID = 0;
            if (ibusMember.IsNull())
                LoadMember();
            if (ibusMember.iclbRetirementAccount.IsNull())
                ibusMember.LoadRetirementAccount();
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            //Since we are allowing any status for an RTW Member we are checking whether the person account other than withdrawn is > 1.
            var iclbfilteredPlan = ibusMember.iclbRetirementAccount.Where(
                                    o => o.icdoPersonAccount.plan_id == ibusPersonAccount.icdoPersonAccount.plan_id &&
                                    o.icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementWithDrawn);
            ibusMember.IsRTWMember(icdoBenefitCalculation.plan_id, busConstant.PayeeStatusForRTW.IgnoreStatus, ref lintPreRTWPayeeAccountID);
            iintRTWPayeeAccountID = lintPreRTWPayeeAccountID;
            if ((lintPreRTWPayeeAccountID > 0) &&
                (icdoBenefitCalculation.post_retirement_death_reason_type_value == busConstant.PostRetirementAccountOwnerDeath) &&
                (iclbfilteredPlan.Count() > 1))
            {
                return true;
            }
            return false;
        }

        /// UAT PIR ID 1221
        public DateTime idteMemberTerminationDate { get; set; }

        public void LoadMemberTerminationDate()
        {
            if (icdoBenefitCalculation.post_retirement_death_reason_type_value.Equals(busConstant.PostRetirementFirstBeneficiaryDeath))
            {
                if (ibusOriginatingPayeeAccount.IsNull()) LoadOriginatingPayeeAccount();
                if (ibusOriginatingPayeeAccount.ibusApplication.IsNull()) ibusOriginatingPayeeAccount.LoadApplication();
                if (ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value.Equals(busConstant.ApplicationBenefitTypePostRetirementDeath))
                {
                    if (ibusOriginatingPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.IsNull()) ibusOriginatingPayeeAccount.ibusApplication.LoadOriginatingPayeeAccount();
                    if (ibusOriginatingPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.ibusApplication.IsNull()) ibusOriginatingPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.LoadApplication();
                    idteMemberTerminationDate = ibusOriginatingPayeeAccount.ibusApplication.ibusOriginatingPayeeAccount.ibusApplication.icdoBenefitApplication.termination_date;
                }
                else if (ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_account_type_value.Equals(busConstant.ApplicationBenefitTypePreRetirementDeath))
                {
                    DateTime ldteTempDate = new DateTime();
                    if (ibusOriginatingPayeeAccount.ibusApplication.ibusPersonAccount.IsNull()) ibusOriginatingPayeeAccount.ibusApplication.LoadPersonAccount();
                    GetOrgIdAsLatestEmploymentOrgId(ibusOriginatingPayeeAccount.ibusApplication.ibusPersonAccount, string.Empty, ref ldteTempDate);
                    idteMemberTerminationDate = ldteTempDate;
                }
            }
            else if (icdoBenefitCalculation.post_retirement_death_reason_type_value.Equals(busConstant.PostRetirementAlternatePayeeDeath))
                idteMemberTerminationDate = icdoBenefitCalculation.termination_date;
            else if (icdoBenefitCalculation.post_retirement_death_reason_type_value.Equals(busConstant.PostRetirementAccountOwnerDeath))
                idteMemberTerminationDate = ibusOriginatingPayeeAccount.ibusApplication.icdoBenefitApplication.termination_date;
        }
    }
}
