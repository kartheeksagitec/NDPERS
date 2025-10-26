using System;
using System.Collections.ObjectModel;
using System.Data;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using System.Reflection;
using Sagitec.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;
namespace NeoSpin.BusinessObjects
{
    public static class busPayeeAccountHelper
    {
        static int ZERO = 0;
        /// <summary>
        /// Returns maximum of Payment Date
        /// </summary>
        /// <returns>Last Benefit Payment Date</returns>
        public static DateTime GetLastBenefitPaymentDate()
        {
            busBase lobjBase = new busBase();
            return Convert.ToDateTime(DBFunction.DBExecuteScalar("cdoPaymentSchedule.GetRecentPaymentDate", new object[] { },
                                                lobjBase.iobjPassInfo.iconFramework, lobjBase.iobjPassInfo.itrnFramework));
        }

        public static int GetFedTaxVendorID()
        {
            busBase lobjBase = new busBase();
            DataTable ldtbFedTaxVendor = lobjBase.iobjPassInfo.isrvDBCache.GetCodeDescription(busConstant.SystemConstantCodeID, busConstant.SystemConstantFedTaxVendor);
            return busGlobalFunctions.GetOrgIdFromOrgCode(ldtbFedTaxVendor.Rows.Count > 0 ? ldtbFedTaxVendor.Rows[0]["data1"].ToString() : ZERO.ToString());
        }
        public static int GetStateTaxVendorID()
        {
            busBase lobjBase = new busBase();
            DataTable ldtbStateTaxVendor = lobjBase.iobjPassInfo.isrvDBCache.GetCodeDescription(busConstant.SystemConstantCodeID, busConstant.SystemConstantStateTaxVendor);
            return busGlobalFunctions.GetOrgIdFromOrgCode(ldtbStateTaxVendor.Rows.Count > 0 ? ldtbStateTaxVendor.Rows[0]["data1"].ToString() : ZERO.ToString());
        }

        //Backlog PIR 938
        public static busPaymentCheckBook GetPaymentCheckBookForGivenDate(DateTime adteGivenDate, int aintPlanID, string astrBenefitType)
        {
            busBase lobjBase = new busBase();
            DataTable ldtbResult = busBase.Select<cdoPaymentCheckBook>(null, null, null, null);
            Collection<busPaymentCheckBook> lclbPaymentCheckBook = lobjBase.GetCollection<busPaymentCheckBook>(ldtbResult, "icdoPaymentCheckBook");

            //Backlog PIR 938
            if (aintPlanID > 0)
            {
                busPlan lbusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusPlan.FindPlan(aintPlanID);
                astrBenefitType = lbusPlan.icdoPlan.benefit_type_value;
            }
            int lintCheckBookID = Convert.ToInt32(busGlobalFunctions.GetData3ByCodeValue(7005,
                    (astrBenefitType == busConstant.PlanBenefitTypeInsurance) ? busConstant.PlanBenefitTypeInsurance : busConstant.PlanBenefitTypeRetirement,
                    lobjBase.iobjPassInfo));
            if (lclbPaymentCheckBook.Count > 0)
            {
                var lclbCheckbook = from lobjCheckBook in lclbPaymentCheckBook
                                    where lobjCheckBook.icdoPaymentCheckBook.effective_date <= adteGivenDate
                                    && lobjCheckBook.icdoPaymentCheckBook.check_book_id == lintCheckBookID  //Backlog PIR 938
                                    select lobjCheckBook;
                if (lclbCheckbook.Count() > 0)
                {
                    busPaymentCheckBook lobjCheck = (from lobjPaymentCheckBook in lclbCheckbook
                                                     where lobjPaymentCheckBook.icdoPaymentCheckBook.effective_date ==
                                                     (from lobjCheckBook in lclbCheckbook
                                                      where lobjCheckBook.icdoPaymentCheckBook.effective_date <= adteGivenDate
                                                      select lobjCheckBook.icdoPaymentCheckBook.effective_date).Max()
                                                     select lobjPaymentCheckBook).FirstOrDefault();
                    return lobjCheck;
                }
            }
            return null;
        }

        //Load All the Payment Schedules
        public static Collection<busPaymentSchedule> LoadPaymentSchedules()
        {
            return LoadPaymentSchedules(DateTime.MinValue);
        }

        //Load Payment Schedules For the given date
        public static Collection<busPaymentSchedule> LoadPaymentSchedules(DateTime adteGivenDate)
        {
            busBase lobjBase = new busBase();
            DataTable ldtbResult = new DataTable();
            if (adteGivenDate != DateTime.MinValue)
            {
                ldtbResult = busBase.Select<cdoPaymentSchedule>(new string[1] { "payment_date" }, new object[1] { adteGivenDate }, null, null);
            }
            else
            {
                ldtbResult = busBase.Select<cdoPaymentSchedule>(null, null, null, null);
            }
            Collection<busPaymentSchedule> lclbPaymentSchedules = lobjBase.GetCollection<busPaymentSchedule>(ldtbResult, "icdoPaymentSchedule");
            return lclbPaymentSchedules;
        }
        //Check whether payment schedule exits for the given date and shedule type,if not exits create new one,else return the valid status record
        public static busPaymentSchedule GetPaymentSchedule(DateTime adteGivenDate, string astrScheduleType)
        {
            bool lblnScheduleExist = false;
            busPaymentSchedule lobjPaymentSchedule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };
            if (LoadPaymentSchedules(adteGivenDate).Count > 0)
            {
                lobjPaymentSchedule = LoadPaymentSchedules(adteGivenDate).Where(o => o.icdoPaymentSchedule.payment_date <= adteGivenDate &&
                    o.icdoPaymentSchedule.schedule_type_value == astrScheduleType && (o.icdoPaymentSchedule.status_value == busConstant.PaymentScheduleStatusValid)).FirstOrDefault();
                if (lobjPaymentSchedule != null)
                    lblnScheduleExist = true;
            }
            if (!lblnScheduleExist)
            {
                busPaymentSchedule lobjSchedule = new busPaymentSchedule { icdoPaymentSchedule = new cdoPaymentSchedule() };
                lobjSchedule.icdoPaymentSchedule.payment_date = adteGivenDate;
                lobjSchedule.icdoPaymentSchedule.effective_date = adteGivenDate;
                lobjSchedule.icdoPaymentSchedule.schedule_type_value = astrScheduleType;
                lobjSchedule.icdoPaymentSchedule.status_value = busConstant.PaymentScheduleStatusValid;
                lobjSchedule.icdoPaymentSchedule.action_status_value = busConstant.PaymentScheduleActionStatusReadyforFinal;
                lobjSchedule.icdoPaymentSchedule.Insert();
                lobjPaymentSchedule = lobjSchedule;
            }
            return lobjPaymentSchedule;
        }


        public static DateTime GetPERSLinkGoLiveDate()
        {
            busBase lobjBase = new busBase();
            DateTime ldtPersLinkGoLiveDate = DateTime.MinValue;
            DataTable ldtbCodeValue = lobjBase.iobjPassInfo.isrvDBCache.GetCodeDescription(52, busConstant.PERSLinkGoLiveDate);
            if (ldtbCodeValue.Rows.Count > 0)
            {
                cdoCodeValue lcdoCodeValue = new cdoCodeValue();
                lcdoCodeValue.LoadData(ldtbCodeValue.Rows[0]);
                ldtPersLinkGoLiveDate = Convert.ToDateTime(lcdoCodeValue.data1);
            }
            return ldtPersLinkGoLiveDate;
        }
        public static Collection<busFedStateTaxRate> iclbFedStatTax { get; set; }

        /// <summary>
        /// Method to Calculate State or federal tax for the Taxable Amount
        /// </summary>		
        /// <param name="adecTaxableAmount">decimal</param>
        /// <param name="aintNoOfAllowances">int</param>
        /// <param name="adtPaymentDate">DateTime</param>
        /// <param name="astrMaritalStatus">string</param>
        /// <param name="astrTaxIdentifier">string</param>
        /// <param name="adecAdditionalTax">decimal</param>
        /// <returns>decimal</returns>
        public static decimal CalculateFedOrStateTax(decimal adecTaxableAmount, int aintNoOfAllowances, DateTime adtPaymentDate, string astrMaritalStatus,
            string astrTaxIdentifier, decimal adecAdditionalTax)
        {
            decimal ldecTaxAmount = 0.00M, ldecResult4 = 0.0M;
            if ((astrMaritalStatus != null) && (astrTaxIdentifier != null))
            {
                if (iclbFedStatTax == null)
                    LoadFedStateTaxRates();
                /*Get the Latest Rates for the given effective date*/
                //uat pir 1375
                //if inner linq exp not returning any value, .Count will give exception, so added this check here
                if (iclbFedStatTax.Where(o => o.icdoFedStateTaxRate.marital_status_value == astrMaritalStatus
                                      && o.icdoFedStateTaxRate.tax_identifier_value == astrTaxIdentifier
                                      && o.icdoFedStateTaxRate.effective_date <= adtPaymentDate).Any())
                {
                    var lclbFedStatTax = from lbusFedStateTaxRate in iclbFedStatTax
                                         where lbusFedStateTaxRate.icdoFedStateTaxRate.marital_status_value == astrMaritalStatus
                                         && lbusFedStateTaxRate.icdoFedStateTaxRate.tax_identifier_value == astrTaxIdentifier
                                         && (lbusFedStateTaxRate.icdoFedStateTaxRate.effective_date ==
                                         (from lobjFedStateTaxRate in iclbFedStatTax
                                          where lobjFedStateTaxRate.icdoFedStateTaxRate.marital_status_value == astrMaritalStatus
                                          && lobjFedStateTaxRate.icdoFedStateTaxRate.tax_identifier_value == astrTaxIdentifier
                                          && lobjFedStateTaxRate.icdoFedStateTaxRate.effective_date <= adtPaymentDate
                                          select lobjFedStateTaxRate.icdoFedStateTaxRate.effective_date).Max())
                                         select lbusFedStateTaxRate;

                    if (lclbFedStatTax.Count() > 0)
                    {
                        decimal ldecResult1 = adecTaxableAmount - (aintNoOfAllowances * lclbFedStatTax.FirstOrDefault().icdoFedStateTaxRate.allowance_amount);
                        busFedStateTaxRate lobjFedStateTax = new busFedStateTaxRate();
                        /*Get exact rate for given taxable amount */
                        foreach (busFedStateTaxRate lobjFedStateTaxcol in lclbFedStatTax)
                        {
                            if ((ldecResult1 >= lobjFedStateTaxcol.icdoFedStateTaxRate.minimum_amount) &&
                                (ldecResult1 < lobjFedStateTaxcol.icdoFedStateTaxRate.maximum_amount))
                            {
                                lobjFedStateTax = lobjFedStateTaxcol;
                                break;
                            }
                        }
                        if (lobjFedStateTax.icdoFedStateTaxRate != null)
                        {
                            decimal ldecResult2 = ldecResult1 - lobjFedStateTax.icdoFedStateTaxRate.minimum_amount;
                            decimal ldecResult3 = ldecResult2 * lobjFedStateTax.icdoFedStateTaxRate.percentage / 100;
                            ldecResult4 = ldecResult3 + lobjFedStateTax.icdoFedStateTaxRate.tax_amount;

                        }
                    }
                }
                ldecTaxAmount = ldecResult4 + adecAdditionalTax;
            }
            return Math.Round(ldecTaxAmount, 2, MidpointRounding.AwayFromZero);
        }
        //Load Fed State Tax Rates From the Cache
        public static void LoadFedStateTaxRates()
        {
            busBase lobjBase = new busBase();
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;

            DataTable tax = lobjPassInfo.isrvDBCache.GetCacheData("sgt_fed_state_tax_rate", null);

            iclbFedStatTax = lobjBase.GetCollection<busFedStateTaxRate>(tax, "icdoFedStateTaxRate");
        }


        public static Collection<busFedStateFlatTaxRate> iclbFedStatFlatTax { get; set; }
        /// <summary>
        /// Method to Calculate Flat Tax for the Taxable Amount
        /// </summary>		
        /// <param name="adecTaxableAmount">decimal</param>
        /// <param name="astrBenefitType">string</param>
        /// <param name="astrBenefitSubType">string</param>
        /// <param name="astrAccountRelationship">string</param>
        /// <param name="adtPaymentDate">string</param>
        /// <param name="astrTaxIdentifier">string</param>
        /// <returns>decimal</returns>
        public static decimal CalculateFlatTax(decimal adecTaxableAmount, DateTime adtPaymentDate, string astrTaxIdentifier, string astrTaxRef, string astrIsRMD)
        {
            decimal ldecTaxAmount = 0.00M;
            if (iclbFedStatFlatTax == null)
                LoadFedStateFlatTaxRates();
            busFedStateFlatTaxRate lbusFedStateFlatTaxRate = null;
            if (astrTaxIdentifier == busConstant.PayeeAccountTaxIdentifierFedTax)
            {
                lbusFedStateFlatTaxRate = iclbFedStatFlatTax
                                            .OrderByDescending(rate => rate.icdoFedStateFlatTaxRate.effective_date)
                                            .FirstOrDefault(rate => rate.icdoFedStateFlatTaxRate.effective_date <= adtPaymentDate &&
                                            rate.icdoFedStateFlatTaxRate.tax_identifier_value == astrTaxIdentifier &&
                                            rate.icdoFedStateFlatTaxRate.tax_ref == astrTaxRef &&
                                            rate.icdoFedStateFlatTaxRate.is_rmd == astrIsRMD);
            }
            else if (astrTaxIdentifier == busConstant.PayeeAccountTaxIdentifierStateTax)
            {
                lbusFedStateFlatTaxRate = iclbFedStatFlatTax
                                          .OrderByDescending(rate => rate.icdoFedStateFlatTaxRate.effective_date)
                                          .FirstOrDefault(rate => rate.icdoFedStateFlatTaxRate.effective_date <= adtPaymentDate &&
                                            rate.icdoFedStateFlatTaxRate.tax_identifier_value == astrTaxIdentifier &&
                                            rate.icdoFedStateFlatTaxRate.tax_ref == astrTaxRef);
            }
            if (lbusFedStateFlatTaxRate?.icdoFedStateFlatTaxRate.fed_state_flat_tax_id > 0)
            {
                ldecTaxAmount = adecTaxableAmount * lbusFedStateFlatTaxRate.icdoFedStateFlatTaxRate.flat_tax_percentage / 100;
            }
            return ldecTaxAmount;
        }

        public static decimal CalculateFlatTaxValues(DateTime adtPaymentDate, string astrTaxIdentifier, string astrTaxRef, string astrIsRMD)
        {
            decimal ldecFlatTaxPercentage = 0.00M;
            if (iclbFedStatFlatTax == null)
                LoadFedStateFlatTaxRates();
            busFedStateFlatTaxRate lbusFedStateFlatTaxRate = null;
            if (astrTaxIdentifier == busConstant.PayeeAccountTaxIdentifierFedTax)
            {
                lbusFedStateFlatTaxRate = iclbFedStatFlatTax
                                            .OrderByDescending(rate => rate.icdoFedStateFlatTaxRate.effective_date)
                                            .FirstOrDefault(rate => rate.icdoFedStateFlatTaxRate.effective_date <= adtPaymentDate &&
                                            rate.icdoFedStateFlatTaxRate.tax_identifier_value == astrTaxIdentifier &&
                                            rate.icdoFedStateFlatTaxRate.tax_ref == astrTaxRef &&
                                            rate.icdoFedStateFlatTaxRate.is_rmd == astrIsRMD);
            }
            else if (astrTaxIdentifier == busConstant.PayeeAccountTaxIdentifierStateTax)
            {
                lbusFedStateFlatTaxRate = iclbFedStatFlatTax
                                          .OrderByDescending(rate => rate.icdoFedStateFlatTaxRate.effective_date)
                                          .FirstOrDefault(rate => rate.icdoFedStateFlatTaxRate.effective_date <= adtPaymentDate &&
                                            rate.icdoFedStateFlatTaxRate.tax_identifier_value == astrTaxIdentifier &&
                                            rate.icdoFedStateFlatTaxRate.tax_ref == astrTaxRef);
            }
            if (lbusFedStateFlatTaxRate?.icdoFedStateFlatTaxRate.fed_state_flat_tax_id > 0)
            {
                ldecFlatTaxPercentage = lbusFedStateFlatTaxRate.icdoFedStateFlatTaxRate.flat_tax_percentage;
            }
            return ldecFlatTaxPercentage;
        }
        public static busFedStateFlatTaxRate LoadFlatTaxRate(DateTime adtPaymentDate, string astrTaxIdentifier, string astrTaxRef, string astrIsRMD)
        {
            if (iclbFedStatFlatTax == null)
                LoadFedStateFlatTaxRates();
            busFedStateFlatTaxRate lbusFedStateFlatTaxRate = null;
            if (astrTaxIdentifier == busConstant.PayeeAccountTaxIdentifierFedTax)
            {
                lbusFedStateFlatTaxRate = iclbFedStatFlatTax
                                            .OrderByDescending(rate => rate.icdoFedStateFlatTaxRate.effective_date)
                                            .FirstOrDefault(rate => rate.icdoFedStateFlatTaxRate.effective_date <= adtPaymentDate &&
                                            rate.icdoFedStateFlatTaxRate.tax_identifier_value == astrTaxIdentifier &&
                                            rate.icdoFedStateFlatTaxRate.tax_ref == astrTaxRef &&
                                            rate.icdoFedStateFlatTaxRate.is_rmd == astrIsRMD);
            }
            else if (astrTaxIdentifier == busConstant.PayeeAccountTaxIdentifierStateTax)
            {
                lbusFedStateFlatTaxRate = iclbFedStatFlatTax
                                          .OrderByDescending(rate => rate.icdoFedStateFlatTaxRate.effective_date)
                                          .FirstOrDefault(rate => rate.icdoFedStateFlatTaxRate.effective_date <= adtPaymentDate &&
                                            rate.icdoFedStateFlatTaxRate.tax_identifier_value == astrTaxIdentifier &&
                                            rate.icdoFedStateFlatTaxRate.tax_ref == astrTaxRef);
            }
            return lbusFedStateFlatTaxRate;
        }


        //Load Fed State Tax Rate from cache
        public static void LoadFedStateFlatTaxRates()
        {

            busBase lobjbase = new busBase();
            utlPassInfo lobjPassInfo = utlPassInfo.iobjPassInfo;

            DataTable tax = lobjPassInfo.isrvDBCache.GetCacheData("sgt_fed_state_flat_tax_rate", null);

            iclbFedStatFlatTax = lobjbase.GetCollection<busFedStateFlatTaxRate>(tax, "icdoFedStateFlatTaxRate");
        }

        public static int ManagePayeeAccount(int aintPayeePERSLinkID, int aintPayeeOrgID, int aintApplicationID, int aintCalculationID, int aintBenefitAccountID, int aintDROCalculationID,
                                              string astrStatusValue, string astrBenefitAccountTypeValue, string astrBenefitAccountSubTypeValue, string astrPullCheckFlag,
                                              DateTime adteBenefitBeginDate, DateTime adteBenefitEndDate, string astrAccountRelationValue, string astrFamilyRelationValue,
                                              decimal adecMinimumGuaranteeAmount, decimal adecNonTaxableBeginningBalance, string astrBenefitOptionValue,
                                              decimal adecMemberRHICAmount, int aintPayeeAccountID, string astrExclusionMethod,
                                              decimal adecExclusionRatio, DateTime adteTermCertainEndDate, string astrRHICRefundFlag, int aintDROApplicationID, string astrGraduatedBenefitOptionValue)
        {
            if ((aintBenefitAccountID != 0) &&
               (!string.IsNullOrEmpty(astrBenefitAccountTypeValue)) &&
               (!string.IsNullOrEmpty(astrBenefitAccountSubTypeValue)) &&
               (!string.IsNullOrEmpty(astrPullCheckFlag)) &&
               (adteBenefitBeginDate != DateTime.MinValue || aintPayeeAccountID > 0) &&
               (!string.IsNullOrEmpty(astrAccountRelationValue)) &&
               (!string.IsNullOrEmpty(astrFamilyRelationValue)))
            {
                busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                if (aintPayeeAccountID == 0)
                    lobjPayeeAccount.icdoPayeeAccount = new cdoPayeeAccount();
                else
                    lobjPayeeAccount.FindPayeeAccount(aintPayeeAccountID);
                lobjPayeeAccount.icdoPayeeAccount.payee_perslink_id = aintPayeePERSLinkID;
                lobjPayeeAccount.icdoPayeeAccount.payee_org_id = aintPayeeOrgID;
                lobjPayeeAccount.icdoPayeeAccount.application_id = aintApplicationID;
                lobjPayeeAccount.icdoPayeeAccount.calculation_id = aintCalculationID;
                lobjPayeeAccount.icdoPayeeAccount.dro_calculation_id = aintDROCalculationID;
                lobjPayeeAccount.icdoPayeeAccount.benefit_account_id = aintBenefitAccountID;
                lobjPayeeAccount.icdoPayeeAccount.status_value = astrStatusValue;
                lobjPayeeAccount.icdoPayeeAccount.benefit_account_type_value = astrBenefitAccountTypeValue;
                lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value = astrBenefitAccountSubTypeValue;
                lobjPayeeAccount.icdoPayeeAccount.pull_check_flag = astrPullCheckFlag;
                //ucs - 079 : for adj. cal benefit begin date should not be updated
                lobjPayeeAccount.icdoPayeeAccount.benefit_begin_date = adteBenefitBeginDate == DateTime.MinValue ?
                    lobjPayeeAccount.icdoPayeeAccount.benefit_begin_date : adteBenefitBeginDate;
                lobjPayeeAccount.icdoPayeeAccount.benefit_end_date = adteBenefitEndDate;
                lobjPayeeAccount.icdoPayeeAccount.account_relation_value = astrAccountRelationValue;
                lobjPayeeAccount.icdoPayeeAccount.family_relation_value = astrFamilyRelationValue;
                lobjPayeeAccount.icdoPayeeAccount.minimum_guarantee_amount = adecMinimumGuaranteeAmount;
                lobjPayeeAccount.icdoPayeeAccount.nontaxable_beginning_balance = adecNonTaxableBeginningBalance;
                lobjPayeeAccount.icdoPayeeAccount.benefit_option_value = astrBenefitOptionValue;
                lobjPayeeAccount.icdoPayeeAccount.rhic_amount = adecMemberRHICAmount;
                lobjPayeeAccount.icdoPayeeAccount.exclusion_method_value = astrExclusionMethod;
                lobjPayeeAccount.icdoPayeeAccount.term_certain_end_date = adteTermCertainEndDate;
                lobjPayeeAccount.icdoPayeeAccount.rhic_ee_amount_refund_flag = astrRHICRefundFlag;
                lobjPayeeAccount.icdoPayeeAccount.dro_application_id = aintDROApplicationID;
                //UAT PIR:925 Adding Graduated Benefit Option Value into Payee Account.
                lobjPayeeAccount.icdoPayeeAccount.graduated_benefit_option_value = astrGraduatedBenefitOptionValue;
                //PIR:1713 Payee Account Disability Recertification date should be set when a payee account is set.
                if (astrBenefitAccountTypeValue == busConstant.ApplicationBenefitTypeDisability && adteBenefitBeginDate != DateTime.MinValue) // SYS PIR ID 2279
                {
                    // UAT PIR ID 1362
                    if (lobjPayeeAccount.idtNextBenefitPaymentDate == DateTime.MinValue)
                        lobjPayeeAccount.LoadNexBenefitPaymentDate();
                    DateTime ldteRecertificationDate = lobjPayeeAccount.idtNextBenefitPaymentDate.AddMonths(18);
                    lobjPayeeAccount.icdoPayeeAccount.recertification_date = new DateTime(ldteRecertificationDate.Year, ldteRecertificationDate.Month, 01);
                    lobjPayeeAccount.icdoPayeeAccount.case_recertification_date = lobjPayeeAccount.icdoPayeeAccount.recertification_date;
                    lobjPayeeAccount.icdoPayeeAccount.is_recertification_date_set_flag = busConstant.Flag_Yes;
                    if (lobjPayeeAccount.icdoPayeeAccount.benefit_begin_date < busConstant.Pre1991Disability)
                        lobjPayeeAccount.icdoPayeeAccount.is_pre_1991_disability_flag = busConstant.Flag_Yes;
                    else
                        lobjPayeeAccount.icdoPayeeAccount.is_pre_1991_disability_flag = busConstant.Flag_No;
                }
                //---PIR 1713 Ends

                if (aintPayeeAccountID == 0)
                {
                    if (lobjPayeeAccount.icdoPayeeAccount.Insert() == 1)
                    {
                        lobjPayeeAccount.iblnNewPayeeAccountIndicator = true;
                        lobjPayeeAccount.ValidateSoftErrors();
                        lobjPayeeAccount.UpdateValidateStatus();
                        return lobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                    }
                }
                else
                {
                    lobjPayeeAccount.icdoPayeeAccount.Update();
                    if (lobjPayeeAccount.ibusSoftErrors == null)
                        lobjPayeeAccount.LoadErrors();
                    lobjPayeeAccount.iblnClearSoftErrors = false;
                    lobjPayeeAccount.ibusSoftErrors.iblnClearError = false;
                    lobjPayeeAccount.iblnPayeeAccountInfoChangeIndicator = true;
                    lobjPayeeAccount.ValidateSoftErrors();
                    lobjPayeeAccount.UpdateValidateStatus();
                    return aintPayeeAccountID;
                }
            }
            return 0;
        }

        // Insert values into the Table PayeeAccountStatus
        // Returns true if successfully inserts the value
        public static bool CreatePayeeAccountStatus(int aintPayeeAccountID, string astrStatusValue, DateTime adteStatusEffectiveDate,
                                                                    string astrSuspensionReasonValue, string astrTerminationReasonValue)
        {
            if ((aintPayeeAccountID != 0) &&
               (!(string.IsNullOrEmpty(astrStatusValue))) &&
               (adteStatusEffectiveDate != DateTime.MinValue))
            {
                busPayeeAccountStatus lobjPayeeAccountStatus = new busPayeeAccountStatus();
                lobjPayeeAccountStatus.icdoPayeeAccountStatus = new cdoPayeeAccountStatus();
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.payee_account_id = aintPayeeAccountID;
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = astrStatusValue;
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = adteStatusEffectiveDate;
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.suspension_status_reason_value = astrSuspensionReasonValue;
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.terminated_status_reason_value = astrTerminationReasonValue;
                if (lobjPayeeAccountStatus.icdoPayeeAccountStatus.Insert() == 1)
                    return true;
            }
            return false;
        }

        // Returns true if Payee Account already exists for the given Person ID 
        public static int IsPayeeAccountExists(int aintPayeeID, int aintBenefitAccountID, string astrBenefitAccountRelationvalue,
                                                string astrBenefitAccountTypeValue, bool ablnIsPayeeOrg)
        {
            /// PIR ID 1778 - For Convert to Normal case, the Payee account has to be created under same Benefit account.
            /// Hence the Benefit account type value constraint added
            DataTable ldtbResult = new DataTable();
            int lintPayeeAccountid = 0;
            if (!ablnIsPayeeOrg)
            {
                ldtbResult = busBase.Select<cdoPayeeAccount>(
                            new string[4] { "PAYEE_PERSLINK_ID", "ACCOUNT_RELATION_VALUE", "BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE" },
                            new object[4] { aintPayeeID, astrBenefitAccountRelationvalue, aintBenefitAccountID, astrBenefitAccountTypeValue }, null, null);
            }
            else
            {
                ldtbResult = busBase.Select<cdoPayeeAccount>(
                            new string[4] { "PAYEE_ORG_ID", "ACCOUNT_RELATION_VALUE", "BENEFIT_ACCOUNT_ID", "BENEFIT_ACCOUNT_TYPE_VALUE" },
                            new object[4] { aintPayeeID, astrBenefitAccountRelationvalue, aintBenefitAccountID, astrBenefitAccountTypeValue }, null, null);
            }
            //Parallel PIR: 2189 cancelled and Payment complete  Payee account should not be considered

            foreach (DataRow dr in ldtbResult.Rows)
            {
                busPayeeAccount lobjTempPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
                lobjTempPayeeAccount.icdoPayeeAccount.LoadData(dr);
                lobjTempPayeeAccount.LoadActivePayeeStatus();

                if (!((lobjTempPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled()) || (lobjTempPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted())))
                {
                    lintPayeeAccountid = lobjTempPayeeAccount.icdoPayeeAccount.payee_account_id;
                    break;
                }
            }

            return lintPayeeAccountid;
        }

        /// <summary>
        /// Returns maximum of check ef Date
        /// </summary>
        /// <returns>Last Benefit Payment Date</returns>
        public static DateTime GetLastCheckEffectiveDate()
        {
            busBase lobjBase = new busBase();
            return Convert.ToDateTime(DBFunction.DBExecuteScalar("cdoPaymentSchedule.GetRecentCheckEffectiveDate", new object[] { },
                                                lobjBase.iobjPassInfo.iconFramework, lobjBase.iobjPassInfo.itrnFramework));
        }

    }
}
