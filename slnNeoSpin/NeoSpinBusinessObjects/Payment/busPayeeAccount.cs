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
#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPayeeAccount : busPayeeAccountGen
    {
        //pir 8618
        public bool iblnIsFromMSS { get; set; }
        public bool iblnIsEmailAddressWaived { get; set; }

        public bool iblnIsMSSTaxWithholding { get; set; } //PIR 20387
        //PIR 
        public string istrNextBenefitPaymentDate
        {
            get
            {
                if (ibusPayeeAccountActiveStatus == null)
                    LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                if (idtNextBenefitPaymentDate == DateTime.MinValue)
                    LoadNexBenefitPaymentDate();
                if (ibusPayeeAccountActiveStatus.IsStatusCompletedOrProcessed())
                {
                    return string.Empty;
                }
                return idtNextBenefitPaymentDate.ToString("d");
            }
        }
        //PIR 8912
        public String istrDBPlan
        {
            get
            {
                if (ibusPlan.IsNull()) LoadPlan();
                if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDB || ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueHB)      
                    return "Benefit";
                if (ibusPlan.icdoPlan.retirement_type_value == busConstant.PlanRetirementTypeValueDC)
                    return "Contribution";
                return string.Empty;
            }
        }
        public int iintBenefitType
        {
            get
            {
                if (icdoPayeeAccount.benefit_account_type_value == "DETH"
                    || icdoPayeeAccount.benefit_account_type_value == "PSTD")
                    return 1;
                if (icdoPayeeAccount.benefit_account_type_value == "RFND")
                    return 2;
                if (icdoPayeeAccount.benefit_account_type_value == "DISA"
                    || icdoPayeeAccount.benefit_account_type_value == "RETR")
                    return 3;
                else
                    return 0;
            }
        }
        public decimal idecHealthPremiumDeduction
        {
            get
            {
                LoadTotalInsurancePremiumDeductionAmountForPlan(busConstant.HealthInsuranceDeductionPaymentItemType);
                return idecTotalDeductionsAmount;
            }
        }
        public void LoadTotalInsurancePremiumDeductionAmountForPlan(int aintPaymentItemTypeId)
        {
            LoadNexBenefitPaymentDate();
            idecTotalDeductionsAmount = 0.00m;
            if (iclbDeductions == null)
                LoadDeductions();
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountItemType in iclbDeductions)
            {
                if (busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date,
                    lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date)
                    && lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id == aintPaymentItemTypeId)
                {
                    idecTotalDeductionsAmount += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount;
                }
            }
        }
        public string istrSpouseName
        {
            get
            {
                if (ibusPayee.ibusSpouse.IsNull())
                    ibusPayee.LoadSpouse();
                return ibusPayee.ibusSpouse.icdoPerson.FullName;
            }
        }
        public string istrSpouseSSN
        {
            get
            {
                if (ibusPayee.ibusSpouse.IsNull())
                    ibusPayee.LoadSpouse();
                return ibusPayee.ibusSpouse.icdoPerson.ssn;
            }
        }
        public DateTime idtSpouseDOB
        {
            get
            {
                if (ibusPayee.ibusSpouse.IsNull())
                    ibusPayee.LoadSpouse();
                return ibusPayee.ibusSpouse.icdoPerson.date_of_birth;
            }
        }

        //PIR 15750 - added a property for Rhic Amount

        public decimal idecRhicAmount
        {
            get
            {
                if (ibusPayee.ibusLatestBenefitRhicCombine == null)
                    ibusPayee.LoadLatestBenefitRhicCombine();

                return (ibusPayee.ibusLatestBenefitRhicCombine.IsNotNull()) ?
                    ibusPayee.ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.combined_rhic_amount : 0.0M; //PIR 16202
            }
        }

        //PIR 20233 - added a property For check plan is main2020 or dc2020
        public int iintIsMain2020orDC2020
        {
            get
            {
                if (ibusPlan == null)
                    LoadPlan();
                return (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMain2020 || ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC2020) ? 1 : 0;
            }
        }
        public string istrNextBenefitPaymentDateFormatted
        {
            get
            {
                if (idtNextBenefitPaymentDate == DateTime.MinValue)
                    LoadNexBenefitPaymentDate();
                return idtNextBenefitPaymentDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        public string istrNextBenefitPaymentDateFormatMonthYear
        {
            get
            {
                if (idtNextBenefitPaymentDate == DateTime.MinValue)
                    LoadNexBenefitPaymentDate();
                return idtNextBenefitPaymentDate.ToString(busConstant.DateFormatMonthYear);
            }
        }
        public bool iblnSpecialCheckIncludeInAdhocFlag { get; set; }
        public string istrPopUpMessageForCertify => busGlobalFunctions.GetMessageTextByMessageID(busConstant.PopUpMessageForCertify, iobjPassInfo);

        public Collection<cdoCodeValue> LoadPayeeAccountStatusByBenefitAccountType()
        {
            Collection<cdoCodeValue> lclcDropDownCodeValue = new Collection<cdoCodeValue>();
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(2203, icdoPayeeAccount.benefit_account_type_value, null, null);
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);

            // PIR 9157 
            foreach (cdoCodeValue lobjCodeValue in lclcCodeValue.Where(o => o.data3 == busConstant.Flag_Yes || o.data2 == busConstant.PayeeAccountStatusJS3RDPartyReceiving))
            {
                if (ibusApplication.icdoBenefitApplication.plan_id == busConstant.PlanIdJobService3rdPartyPayor)
                {
                    if (lobjCodeValue.data2 != busConstant.PayeeAccountStatusApproved)
                    {
                        lclcDropDownCodeValue.Add(lobjCodeValue);
                    }
                }
                else
                {
                    if (lobjCodeValue.data2 != busConstant.PayeeAccountStatusJS3RDPartyReceiving)
                    {
                        lclcDropDownCodeValue.Add(lobjCodeValue);
                    }
                }
            }
            return lclcDropDownCodeValue;
        }
        public bool IsPayeeStatusInReview()
        {
            if (ibusPayeeAccountActiveStatus == null)
                LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            if (ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRefundReview)
                return true;
            else if (ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusRetirementReview)
                return true;
            return false;
        }

        public bool iblnIsRollOverFromScreen { get; set; }
        public bool IsAdditionalContributionsReportedForRefund()
        {
            if (ibusBenefitCalculaton == null)
                LoadBenefitCalculation();
            if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                && (ibusBenefitCalculaton.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments) && IsPayeeStatusInReview())
            {
                return true;
            }
            return false;
        }
        //This method is used for setting up the visibility of Rollover tab
        public bool IsRolloverEligible()
        {
            if (ibusApplication == null)
            {
                LoadApplication();
            }
            if ((ibusApplication.icdoBenefitApplication.plso_requested_flag == busConstant.Flag_Yes) ||
                (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund &&
                    (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRegularRefund ||
                     icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionAutoRefund)) ||
                (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption5YearTermLife) ||
                (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund &&
                    (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath ||
                    icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)))
            {
                return true;
            }
            return false;
        }
        //If payee account relation is not memeber or joint annuitant person not deseased,do not allow for pop up benefits
        public bool IsPopupBenefitsNotAllowed()
        {
            if (ibusApplication == null)
                LoadApplication();
            if (ibusApplication.ibusJointAnniutantPerson == null)
                ibusApplication.LoadJointAnniutantPerson();
            if (icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember &&
                ibusApplication.ibusJointAnniutantPerson.icdoPerson.person_id > 0 &&
                ibusApplication.ibusJointAnniutantPerson.icdoPerson.date_of_death != DateTime.MinValue)
            {
                return true;
            }
            else if (icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipMember &&
                ibusApplication.ibusJointAnniutantPerson.icdoPerson.person_id > 0 && (
                ibusApplication.ibusJointAnniutantPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced ||
                ibusApplication.ibusJointAnniutantPerson.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle ||
                ibusPayee.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusDivorced ||
                ibusPayee.icdoPerson.marital_status_value == busConstant.PersonMaritalStatusSingle))
            {
                return true;
            }
            return false;
        }
        //This method is used for setting up the visibility of New Button in the ACH tab
        public bool IsAchApplicable()
        {
            if (iclbActiveACHDetails == null)
            {
                LoadActiveACHDetail();
            }
            if (iclbActiveACHDetails.Count < 2)
            {
                return true;
            }
            return false;
        }
        //This methdod is used for setting up the visibility of New Button in the Rollover tab
        public bool IsRolloverApplicable()
        {
            if (iclbActiveRolloverDetails == null)
                LoadActiveRolloverDetail();
            //foreach (busPayeeAccountRolloverDetail lobjPayeeAccountRolloverDetail in iclbActiveRolloverDetails)
            //{
            //    if ((lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfTaxable
            //        || lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_option_value == busConstant.PayeeAccountRolloverOptionAllOfGross)
            //        && lobjPayeeAccountRolloverDetail.icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusActive)
            //    {
            //        return false;
            //    }
            //}

            if (iclbActiveRolloverDetails.Count >= 1) //PIR 20189 - Only one rollover detail per payee account
            {
                return false;
            }
            return true;
        }
        public void LoadNontaxableAmount()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            LoadNontaxableAmount(idtNextBenefitPaymentDate);
        }
        public void LoadNontaxableAmount(DateTime adtEffectiveDate)
        {
            idecNontaxableAmount = 0.00m;
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            idecNontaxableAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_No &&
                busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)).
                Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();
        }
        /// <summary>
        /// PIR 17140, 16606, 16896
        /// Adjusts Rollover and Taxes.
        /// </summary>
        public void AdjustRollOverAndTax()
        {
            if (iclbActiveRolloverDetails == null)
                LoadActiveRolloverDetail();

            if (iclbActiveRolloverDetails.Count > 0)
            {
                CreateRolloverAdjustment();
            }
            else
            {
                CalculateAdjustmentTax(false);
            }
        }

        public void LoadTaxableAmount()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            LoadTaxableAmount(idtNextBenefitPaymentDate);
        }
        public void LoadTaxableAmount(DateTime adtEffectiveDate)
        {
            idecTotalTaxableAmount = 0.00m;
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            idecTotalTaxableAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_No &&
                  o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes &&
                busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)).
                Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();
        }
        /* This method is to Get the Total Gross Amount from Payment Item Type
       1.Load all Item Types for the Payee Account       
       2.Sum all the Items whisch are Item Type Direction as '1'. */
        public void LoadGrossAmount()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            LoadGrossAmount(idtNextBenefitPaymentDate);
        }

        public void LoadGrossAmount(DateTime adtEffectiveDate)
        {
            idecGrossAmount = 0.00m;
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountItemType in iclbPayeeAccountPaymentItemType)
            {
                //Take only the items which are effective for the next benfit payment date
                if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date,
                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date))
                {
                    if (lobjPayeeAccountItemType.ibusPaymentItemType == null)
                    {
                        lobjPayeeAccountItemType.LoadPaymentItemType();
                    }
                    //Take only the Items which are have item type direction "1" 
                    if (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1)
                    {
                        idecGrossAmount += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount;
                    }
                }
            }
        }
        /* This method is to Get the Total Gross Amount from Payment Item Type
     1.Load all Item Types for the Payee Account       
     2.sum of item which has Retro_payment_Type is BLANK or 
      NULL and Retro_special_Payment_IND = 0 or NULL and PLSO IND = 0 and Vendor Flag = 0 
      and 1099r_Flag = 1 and  Item_Usage = ‘Monthly’ or 'Onetime Payment' Check_Type = ‘Regular’     */
        public void LoadGrossBenefitAmount()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            LoadGrossBenefitAmount(idtNextBenefitPaymentDate);
        }
        public decimal idecGrossBenfitAmount { get; set; }
        public void LoadGrossBenefitAmount(DateTime adtEffectiveDate)
        {
            idecGrossBenfitAmount = 0.00m;
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            idecGrossBenfitAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.base_amount_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)).
                Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();
        }

        public decimal idecGrossBenefitAmountExcludingCOLAAndAhocIncrease { get; set; }
        public void LoadGrossBenefitAmountExcludingCOLAAndAhocIncrease(DateTime adtEffectiveDate)
        {
            idecGrossBenefitAmountExcludingCOLAAndAhocIncrease = 0.00m;
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            idecGrossBenefitAmountExcludingCOLAAndAhocIncrease = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.base_amount_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date) &&
                o.ibusPaymentItemType.icdoPaymentItemType.adhoc_cola_group_value != busConstant.AdhocIncreasePaymentItem &&
                o.ibusPaymentItemType.icdoPaymentItemType.adhoc_cola_group_value != busConstant.COLAPaymentItem)
                .Select(o => o.icdoPayeeAccountPaymentItemType.amount)
                .Sum();
        }

        /* This method is to Get the Total Deduction Amount from Payment Item Type
        1.Load all Item Types for the Payee Account       
        2.Sum all the Deductions by Date. */
        public void LoadTotalDeductionAmount()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            LoadTotalDeductionAmount(idtNextBenefitPaymentDate);
        }

        public void LoadTotalDeductionAmount(DateTime adtEffectiveDate)
        {
            idecTotalDeductionsAmount = 0.00m;
            if (iclbDeductions == null)
                LoadDeductions();
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountItemType in iclbDeductions)
            {
                //Take only the items which are effective for the next benfit payment date
                if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date,
                    lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date))
                {
                    idecTotalDeductionsAmount += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount;
                }
            }
        }

        /* This method is to Get the Total Benefit Amount from Payment Item Type
         1.Load all Item Types for the Payee Account       
         2.Sum all the Items after multiplying Amount by Item Type Direction. */
        public void LoadBenefitAmount()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            LoadBenefitAmount(idtNextBenefitPaymentDate);
        }

        public void LoadBenefitAmount(DateTime adtEffectiveDate)
        {
            idecBenefitAmount = 0.00m;
            decimal ldecPositiveItemAmount = 0.00M;
            decimal ldecNegativeItemAmount = 0.00M;
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountItemType in iclbPayeeAccountPaymentItemType)
            {
                if (lobjPayeeAccountItemType.ibusPaymentItemType == null)
                {
                    lobjPayeeAccountItemType.LoadPaymentItemType();
                }
                if ((lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != busConstant.RollItemForCheck) &&
                    (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != busConstant.RolloverItemReductionCheck))
                {
                    //Take only the items which are effective for the next benfit payment date
                    if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date,
                    lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date))
                    {
                        //Take only the Items which are have item type direction "1" and "-1" 
                        if (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1)
                        {
                            ldecPositiveItemAmount += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount
                                * lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                        }
                        else
                        {
                            ldecNegativeItemAmount += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount
                                * lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                        }
                    }
                }
            }
            idecBenefitAmount = ldecPositiveItemAmount + ldecNegativeItemAmount;
        }
        public void LoadBenefitAmountForPayee()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            idecBenefitAmount = 0.00m;
            decimal ldecPositiveItemAmount = 0.00M;
            decimal ldecNegativeItemAmount = 0.00M;
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountItemType in iclbPayeeAccountPaymentItemType)
            {
                if (lobjPayeeAccountItemType.ibusPaymentItemType == null)
                {
                    lobjPayeeAccountItemType.LoadPaymentItemType();
                }
                //if ((lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != busConstant.RollItemForCheck) &&
                //    (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != busConstant.RolloverItemReductionCheck))
                //{
                    //Take only the items which are effective for the next benfit payment date
                    if (busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date,
                    lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date))
                    {
                        //Take only the Items which are have item type direction "1" and "-1" 
                        if (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1)
                        {
                            ldecPositiveItemAmount += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount
                                * lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                        }
                        else
                        {
                            ldecNegativeItemAmount += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount
                                * lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                        }
                    }
                //}
            }
            idecBenefitAmount = ldecPositiveItemAmount + ldecNegativeItemAmount;
        }
        /* This method is to Get the Total Taxable Amount from Payment Item Type
           1.Load all Item Types for the Payee Account          
           3.Sum all the Items which are have item type direction "1" and taxable indicator "Y" */
        public void LoadTotalTaxableAmount()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            LoadTotalTaxableAmount(idtNextBenefitPaymentDate);
        }

        public void LoadTotalTaxableAmount(DateTime adtEffectiveDate)
        {
            decimal ldecPositiveItemAmount = 0.00M;
            decimal ldecNegativeItemAmount = 0.00M;
            idecTotalTaxableAmount = 0.00M;
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountItemType in iclbPayeeAccountPaymentItemType)
            {
                //Take only the items which are effective for the next benfit payment date
                if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date,
                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date))
                {
                    if (lobjPayeeAccountItemType.ibusPaymentItemType == null)
                    {
                        lobjPayeeAccountItemType.LoadPaymentItemType();
                    }
                    //Take only the Items which are have item type direction "1" and "-1" and taxable indicator "Y" 
                    if (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                    {
                        if (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1)
                        {
                            ldecPositiveItemAmount += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount
                                * lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                        }
                        else
                        {
                            ldecNegativeItemAmount += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount
                                * lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                        }
                    }
                }
            }
            idecTotalTaxableAmount = ldecPositiveItemAmount + ldecNegativeItemAmount;
        }

        public void LoadTaxableAmountForVariableTax()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            LoadTaxableAmountForVariableTax(idtNextBenefitPaymentDate);
        }

        /* This method is to get the Total Taxable Amount for the special tax treatment from Payment Item Type
          1.Load all Item Types for the Payee Account         
          3.Sum all the Items which are having taxable indicator "Y" and special tax treatment indicator as Taxes should be calculated using Fed tax table . .*/
        public void LoadTaxableAmountForVariableTax(DateTime adtEffectiveDate)
        {
            idecTotalTaxableAmountForVariableTax = 0.00M;
            //reload always for the recalculations
            LoadPayeeAccountPaymentItemType();

            //pir 8457
            //if (iclbRetroItemType == null)
            //  LoadRetroItemType(busConstant.RetroPaymentTypeInitial);
            LoadRetroItemTypeInitialAndReactivation();

            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountItemType in iclbPayeeAccountPaymentItemType)
            {
                //Take only the items which are effective for the next benfit payment date
                if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date,
                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date))
                {
                    if (lobjPayeeAccountItemType.ibusPaymentItemType == null)
                    {
                        lobjPayeeAccountItemType.LoadPaymentItemType();
                    }
                    bool lblnIsInitialRetropaymentItem = false;
                    if (iclbRetroItemTypeInitialAndReac.Where(o =>
                         o.icdoRetroItemType.to_item_type == lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code &&
                         (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.retro_payment_type_value == busConstant.RetroPaymentTypeInitial ||
                         lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.retro_payment_type_value == busConstant.RetroPaymentTypeReactivation)).Count() > 0) //pir 8457
                    {
                        lblnIsInitialRetropaymentItem = true;
                    }

                    if (!lblnIsInitialRetropaymentItem)
                    {

                        //Take only Items which are having taxable indicator "Y" and special tax treatment indicator as Taxes should be calculated using Fed tax table .*/
                        if ((lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                            && (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.special_tax_treatment_code_value == busConstant.SpecialTaxIdendtifierFedTax
                            || lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolloverItemReductionCheck))
                        {
                            idecTotalTaxableAmountForVariableTax += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount *
                                lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                        }
                    }
                }
            }
        }
        public bool iblnIsColaOrAdhocIncreaseTaxCalculation { get; set; }
        public decimal idecTotalTaxableAmountForVariableTaxExcludingRetro { get; set; }

        // Exclude the Payment Item type
        public void LoadTaxableAmountForVariableTaxExcludeRetroItem(DateTime adtEffectiveDate)
        {
            idecTotalTaxableAmountForVariableTaxExcludingRetro = 0.00M;
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountItemType in iclbPayeeAccountPaymentItemType)
            {
                //Take only the items which are effective for the next benfit payment date
                if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date,
                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date))
                {
                    if (lobjPayeeAccountItemType.ibusPaymentItemType == null)
                    {
                        lobjPayeeAccountItemType.LoadPaymentItemType();
                    }
                    //Take only Items which are having taxable indicator "Y" and special tax treatment indicator as Taxes should be calculated using Fed tax table .*/
                    if ((lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                        && (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.special_tax_treatment_code_value ==
                                busConstant.SpecialTaxIdendtifierFedTax
                        || lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolloverItemReductionCheck) &&
                        (string.IsNullOrEmpty(lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.retro_payment_type_value)))
                    {
                        idecTotalTaxableAmountForVariableTaxExcludingRetro += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount *
                            lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                    }
                }
            }
        }
        //property to identify rollover amount reissued to member

        public bool iblnIsRolloverAmountReissuedToMember { get; set; }

        /* This method is to Get the Total Taxable Amount for flat rate caluculation from Payment Item Type
          1.Load all Item Types for the Payee Account       
          3.Sum all the Items which are having taxable indicator "Y" and special tax treatment indicator as Taxes should be calculated as a flat % .*/
        public void LoadTaxableAmountForFlatRates(DateTime adtEffectiveDate, bool ablnIsPlso, bool ablnIsRMD = false)
        {
            idecTotalTaxableAmountForFlatRates = 0.00M;
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountItemType in iclbPayeeAccountPaymentItemType)
            {
                if (lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue)
                {
                    if (lobjPayeeAccountItemType.ibusPaymentItemType == null)
                    {
                        lobjPayeeAccountItemType.LoadPaymentItemType();
                    }
                    if (ablnIsPlso)
                    {
                        if (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.plso_flag == busConstant.Flag_Yes)
                        {
                            idecTotalTaxableAmountForFlatRates += GetTotalFlatTax(adtEffectiveDate, lobjPayeeAccountItemType);
                        }
                    }
                    else if (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.plso_flag == busConstant.Flag_No)
                    {
                        if (ablnIsRMD && lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRMDAmount)
                            idecTotalTaxableAmountForFlatRates += GetTotalFlatTax(adtEffectiveDate, lobjPayeeAccountItemType);
                        else if (!ablnIsRMD && lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code != busConstant.PAPITRMDAmount)
                            idecTotalTaxableAmountForFlatRates += GetTotalFlatTax(adtEffectiveDate, lobjPayeeAccountItemType);
                    }
                }
            }
        }
        private decimal GetTotalFlatTax(DateTime adtEffectiveDate, busPayeeAccountPaymentItemType lobjPayeeAccountItemType)
        {
            decimal ldecTotalTaxableAmountForFlatRates = 0.00M;
            //Take only the details which are effective for the next benfit payment date
            if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date,
            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date))
            {
                //Take only Items which are having taxable indicator "Y" and special tax treatment indicator as Taxes should be calculated as a flat % .*/
                if ((lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes)
                  && (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.special_tax_treatment_code_value == busConstant.SpecialTaxIdendtifierFlatTax
                    || lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolloverItemReductionCheck
                    || (
                       icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption5YearTermLife &&
                       lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolleverCodeValueCanBeRolled)))
                {
                    ldecTotalTaxableAmountForFlatRates += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount * lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                }
                else if (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSORothRolloverFederalTaxAmount ||
                    lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSORothRolloverStateTaxAmount ||
                    lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRothRolloverFederalTaxAmount ||
                    lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRothRolloverStateTaxAmount)
                {
                    ldecTotalTaxableAmountForFlatRates += lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount * lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_direction;
                }
            }
            return ldecTotalTaxableAmountForFlatRates;
        }
        /* 1.Load all the Status Details of Payee descending order by PayeeAcountStatusID
           2.Get The Latest Payee Status */
        public void LoadActivePayeeStatus()
        {
            //Commented since payee account status need to be loaded
            //if (iclbPayeeAccountStatus == null)
            LoadPayeeAccountStatus();
            if (ibusPayeeAccountActiveStatus.IsNull())
                ibusPayeeAccountActiveStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
            if (iclbPayeeAccountStatus.Count > 0)
            {
                ibusPayeeAccountActiveStatus = iclbPayeeAccountStatus[0];
                ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo); //pir 8536
            }
        }

        /// <summary>
        /// method to load payee account status as of next benefit payment date
        /// </summary>
        public void LoadActivePayeeStatusAsofNextBenefitPaymentDate()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            //Commented since payee account status need to be loaded
            //if (iclbPayeeAccountStatus == null)
            LoadPayeeAccountStatus();

            if (iclbPayeeAccountStatus.Count > 0)
            {
                ibusPayeeAccountActiveStatus = iclbPayeeAccountStatus.Where(o =>
                    o.icdoPayeeAccountStatus.status_effective_date <= idtNextBenefitPaymentDate).FirstOrDefault();
                if (ibusPayeeAccountActiveStatus.IsNull())
                    ibusPayeeAccountActiveStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
                else
                    ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo); //pir 8536
            }

            //If there is no record as of next benefit payment date, assign the default object
            if (ibusPayeeAccountActiveStatus.IsNull())
                ibusPayeeAccountActiveStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
        }


        /* 1.Load All the ACH Details
           2.Get the Active ACH Details based on the Next Benefit Payment Date. */
        public void LoadActiveACHDetail()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();

            if (iclbAchDetail == null)
            {
                LoadACHDetail();
            }
            if (iclbActiveACHDetails == null)
            {
                iclbActiveACHDetails = new Collection<busPayeeAccountAchDetail>();
            }
            if (iclbFutureDatedACHDetails == null)
            {
                iclbFutureDatedACHDetails = new Collection<busPayeeAccountAchDetail>();
            }
            foreach (busPayeeAccountAchDetail lobjPayeeAccountAchDetail in iclbAchDetail)
            {
                //Take only the details which are effective for the next benfit payment date
                if (busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date,
                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date))
                {
                    iclbActiveACHDetails.Add(lobjPayeeAccountAchDetail);
                    iclbFutureDatedACHDetails.Add(lobjPayeeAccountAchDetail);
                }
                else if (idtNextBenefitPaymentDate < lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date
                    && lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date == DateTime.MinValue)
                {
                    if (!iclbFutureDatedACHDetails.Contains(lobjPayeeAccountAchDetail))
                        iclbFutureDatedACHDetails.Add(lobjPayeeAccountAchDetail);
                }
            }
        }

        /* 1.Load All the Primary ACH Details
            2.Get the Primary ACH Detail based on primary ach flag. */
        public void LoadPrimaryACHDetail(DateTime adtEffectiveDate)
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            if (iclbAchDetail == null)
                LoadACHDetail();
            if (ibusPrimaryAchDetail == null)
                ibusPrimaryAchDetail = new busPayeeAccountAchDetail();
            if (ibusPrimaryAchDetail.icdoPayeeAccountAchDetail == null)
                ibusPrimaryAchDetail.icdoPayeeAccountAchDetail = new cdoPayeeAccountAchDetail();
            foreach (busPayeeAccountAchDetail lobjPayeeAccountAchDetail in iclbAchDetail)
            {
                if ((busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date,
                    lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_end_date))
                    && (lobjPayeeAccountAchDetail.IsPrimaryACHSelected))
                {
                    ibusPrimaryAchDetail = lobjPayeeAccountAchDetail;
                    break;
                }
            }
        }

        /* 1.Get the Active ACH Details based ach start date and ach end date
           2.Get Total Ach amount and Total Ach Percentage. */
        public void GetTotalACHAmountAndPercentage()
        {
            idecTotalAchPercentage = 0.00M;
            idecTotalAchAmount = 0.00M;
            if (iclbActiveACHDetails == null)
                LoadActiveACHDetail();
            if (idecBenefitAmount == 0.00M)
            {
                LoadBenefitAmount();
            }
            foreach (busPayeeAccountAchDetail lobjPayeeAccountAchDetail in iclbActiveACHDetails)
            {
                if (lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.partial_amount != 0.00M)
                {
                    idecTotalAchPercentage += lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.partial_amount * 100 / idecBenefitAmount;
                    idecTotalAchAmount += lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.partial_amount;
                }
                else
                {
                    idecTotalAchPercentage += lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.percentage_of_net_amount;
                    idecTotalAchAmount += idecBenefitAmount * lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.percentage_of_net_amount / 100;
                }
            }
        }

        //Check Taxwithholding exist for PLSO,if not, Set the Visiblity for NewStateTaxPLSO button TaxWithHolding Tab in PayeeAccountMaintenace screen
        public bool IsTaxWithholdingExistForStateTaxPLSO()
        {
            if (iclbTaxWithholingHistory == null)
            {
                LoadTaxWithHoldingHistory();
            }
            foreach (busPayeeAccountTaxWithholding lobjTaxwithHoling in iclbTaxWithholingHistory)
            {
                if ((lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO) &&
                    (lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax))
                {
                    return true;
                }
            }
            return false;
        }
        //Check Taxwithholding exist for PLSO,if not, Set the Visiblity for NewFedTaxPLSO button TaxWithHolding Tab in PayeeAccountMaintenace screen
        public bool IsTaxWithholdingExistForFedTaxPLSO()
        {
            if (iclbTaxWithholingHistory == null)
            {
                LoadTaxWithHoldingHistory();
            }
            foreach (busPayeeAccountTaxWithholding lobjTaxwithHoling in iclbTaxWithholingHistory)
            {
                if ((lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionPSLO) &&
                    (lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax))
                {
                    return true;
                }
            }
            return false;
        }
        public bool IsMonthTaxButtonVisible()
        {
            if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && !IsBenefitoptionDeathRefund()) ||
                (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && !IsBenefitoptionDeathRefund()) ||
                icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement ||
                icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                return true;
            }
            return false;
        }
        private bool IsBenefitoptionDeathRefund()
        {
            if (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund
                || icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionAutoRefund)
            {
                return true;
            }
            return false;
        }
        //Check Taxwithholding exist for Refund,if not, Set the Visiblity for NewFedTaxRefund button TaxWithHolding Tab in PayeeAccountMaintenace screen
        public bool IsTaxWithholdingExistForFedTaxRefund()
        {
            bool lblRecordNotExist = true;
            if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && IsBenefitoptionDeathRefund()) ||
                (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && IsBenefitoptionDeathRefund()) ||
                icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                if (iclbTaxWithholingHistory == null)
                {
                    LoadTaxWithHoldingHistory();
                }
                foreach (busPayeeAccountTaxWithholding lobjTaxwithHoling in iclbTaxWithholingHistory)
                {
                    if ((lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum) &&
                        (lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax))
                    {
                        lblRecordNotExist = false;
                    }
                }
            }
            else if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && !IsBenefitoptionDeathRefund()) ||
             (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && !IsBenefitoptionDeathRefund()))
            {
                lblRecordNotExist = false;
            }
            else
            {
                lblRecordNotExist = false;
            }
            return lblRecordNotExist;
        }
        //Check Taxwithholding exist for Refund,if not, Set the Visiblity for NewStateTaxRefund button TaxWithHolding Tab in PayeeAccountMaintenace screen
        public bool IsTaxWithholdingExistForStateTaxRefund()
        {
            bool lblRecordNotExist = true;
            if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && IsBenefitoptionDeathRefund()) ||
               (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && IsBenefitoptionDeathRefund()) ||
               icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                if (iclbTaxWithholingHistory == null)
                {
                    LoadTaxWithHoldingHistory();
                }

                foreach (busPayeeAccountTaxWithholding lobjTaxwithHoling in iclbTaxWithholingHistory)
                {
                    if ((lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum) &&
                        (lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax))
                    {
                        lblRecordNotExist = false;
                    }
                }
            }
            else if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && !IsBenefitoptionDeathRefund()) ||
               (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && !IsBenefitoptionDeathRefund()))
            {
                lblRecordNotExist = false;
            }
            else
            {
                lblRecordNotExist = false;
            }
            return lblRecordNotExist;
        }


        public bool DoesTaxWithholdingExistForFedTaxRefund()
        {
            bool lblRecordNotExist = true;
            if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && IsBenefitoptionDeathRefund()) ||
                (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && IsBenefitoptionDeathRefund()) ||
                icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                if (iclbTaxWithholingHistory == null)
                {
                    LoadTaxWithHoldingHistory();
                }
                //foreach (busPayeeAccountTaxWithholding lobjTaxwithHoling in iclbTaxWithholingHistory)
                //{
                //    if ((lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum) &&
                //        (lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax))
                //    {
                //        lblRecordNotExist = false;
                //    }
                //}
            }
            else if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && !IsBenefitoptionDeathRefund()) ||
             (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && !IsBenefitoptionDeathRefund()))
            {
                lblRecordNotExist = false;
            }
            else
            {
                lblRecordNotExist = false;
            }
            return lblRecordNotExist;
        }
        //Check Taxwithholding exist for Refund,if not, Set the Visiblity for NewStateTaxRefund button TaxWithHolding Tab in PayeeAccountMaintenace screen
        public bool DoesTaxWithholdingExistForStateTaxRefund()
        {
            bool lblRecordNotExist = true;
            if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && IsBenefitoptionDeathRefund()) ||
               (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && IsBenefitoptionDeathRefund()) ||
               icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                if (iclbTaxWithholingHistory == null)
                {
                    LoadTaxWithHoldingHistory();
                }

                //foreach (busPayeeAccountTaxWithholding lobjTaxwithHoling in iclbTaxWithholingHistory)
                //{
                //    if ((lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionLumpSum) &&
                //        (lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax))
                //    {
                //        lblRecordNotExist = false;
                //    }
                //}
            }
            else if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && !IsBenefitoptionDeathRefund()) ||
               (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && !IsBenefitoptionDeathRefund()))
            {
                lblRecordNotExist = false;
            }
            else
            {
                lblRecordNotExist = false;
            }
            return lblRecordNotExist;
        }




        public bool IsTaxWithholdingExistForFedTaxRMD()
        {
            bool lblRecordNotExist = true;
            if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && IsBenefitoptionDeathRefund()) ||
                icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                if (iclbTaxWithholingHistory == null)
                {
                    LoadTaxWithHoldingHistory();
                }
                foreach (busPayeeAccountTaxWithholding lobjTaxwithHoling in iclbTaxWithholingHistory)
                {
                    if ((lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD) &&
                        (lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax))
                    {
                        lblRecordNotExist = false;
                    }
                }
            }
            else if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && !IsBenefitoptionDeathRefund()))
            {
                lblRecordNotExist = false;
            }
            else
            {
                lblRecordNotExist = false;
            }
            if (lblRecordNotExist)
            {
                lblRecordNotExist = IsRMDPAPITExisting();
            }
            return lblRecordNotExist;
        }
        //Check Taxwithholding exist for Refund,if not, Set the Visiblity for NewStateTaxRefund button TaxWithHolding Tab in PayeeAccountMaintenace screen
        public bool IsTaxWithholdingExistForStateTaxRMD()
        {
            bool lblRecordNotExist = true;
            if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && IsBenefitoptionDeathRefund()) ||
               icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                if (iclbTaxWithholingHistory == null)
                {
                    LoadTaxWithHoldingHistory();
                }

                foreach (busPayeeAccountTaxWithholding lobjTaxwithHoling in iclbTaxWithholingHistory)
                {
                    if ((lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionRMD) &&
                        (lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax ||
                        lobjTaxwithHoling.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxRefState22Tax))
                    {
                        lblRecordNotExist = false;
                    }
                }
            }
            else if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && !IsBenefitoptionDeathRefund()) ||
               (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && !IsBenefitoptionDeathRefund()))
            {
                lblRecordNotExist = false;
            }
            else
            {
                lblRecordNotExist = false;
            }
            if (lblRecordNotExist)
            {
                lblRecordNotExist = IsRMDPAPITExisting();
            }
            return lblRecordNotExist;
        }

        private bool IsRMDPAPITExisting()
        {
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            if (iclbPayeeAccountPaymentItemType.Count > 0)
            {
                if (idtNextBenefitPaymentDate == DateTime.MinValue)
                    LoadNexBenefitPaymentDate();
                if (idtNextBenefitPaymentDate > DateTime.MinValue)
                {
                    if ((iclbPayeeAccountPaymentItemType
                        .Any(papit => papit.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRMDAmount &&
                        busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, papit.icdoPayeeAccountPaymentItemType.start_date,
                        papit.icdoPayeeAccountPaymentItemType.end_date))))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /* 1.Load All the Rollover Details
           2.Get the Active Rollover Details based Rollover instruction status.*/
        public void LoadActiveRolloverDetail()
        {
            if (iclbRolloverDetail == null)
            {
                LoadRolloverDetail();
            }
            if (iclbActiveRolloverDetails == null)
            {
                iclbActiveRolloverDetails = new Collection<busPayeeAccountRolloverDetail>();
            }
            foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in iclbRolloverDetail)
            {
                //Take only the dettils which are active
                if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.status_value
                            == busConstant.PayeeAccountRolloverDetailStatusActive)
                {
                    iclbActiveRolloverDetails.Add(lobjRolloverDetail);
                }
            }
        }

        /* 1.Load All the Rollover Details
           2.Get the Active Rollover Details based Rollover instruction status.
           3.Get Total Rollover Amount Or Percentage*/
        public void LoadTotalRolloverAmountOrPercentage()
        {
            if (iclbActiveRolloverDetails == null)
            {
                LoadActiveRolloverDetail();
            }
            foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in iclbActiveRolloverDetails)
            {
                if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.amount_of_taxable > 0.00M)
                {
                    idecTotalRolloverAmount += lobjRolloverDetail.icdoPayeeAccountRolloverDetail.amount_of_taxable;
                }
                else if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.percent_of_taxable > 0.00M)
                {
                    idecTotalRolloverPercentage = lobjRolloverDetail.icdoPayeeAccountRolloverDetail.percent_of_taxable;
                }
            }
        }
        /* 1.Load All the Rollover Details
           2.Get the Active Rollover Details based Rollover instruction status.
           3.Return true if percentage entered*/
        public bool IsPercetageEnteredForRollover()
        {
            if (iclbActiveRolloverDetails == null)
            {
                LoadActiveRolloverDetail();
            }
            foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in iclbActiveRolloverDetails)
            {
                if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.percent_of_taxable > 0.00M)
                {
                    return true;
                }
            }
            return false;
        }
        /* 1.Load All the Rollover Details
           2.Get the Active Rollover Details based Rollover instruction status.
           3.Return true if amount entered*/
        public bool IsAmountEnteredForRollover()
        {
            if (iclbActiveRolloverDetails == null)
            {
                LoadActiveRolloverDetail();
            }
            foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in iclbActiveRolloverDetails)
            {
                if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.amount_of_taxable > 0.00M)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// This Method calculates retro payments for the payee acccount and returns collection of retro payment items
        /// 1.Load all the Payment item types for payee account and check for vaild item based on Next Benefit payment date
        /// 2.Find the retro payment items by calling cdoPayeeAccountRetroPayment.GetPaymentItemTypeForRetroItem query.
        /// 3 And Multiply the Retro amount by the number of months between Effective Start Date and Effective End Date 
        /// </summary>
        /// <param name="adtStartDate">DateTime</param>
        /// <param name="adtEndDate"><DateTime/param>
        /// <param name="astrRetroPaymentType">string</param>
        /// <returns>Collection<busPayeeAccountPaymentItemType> </returns>
        public Collection<busPayeeAccountPaymentItemType> CalculateRetroPaymentItemsForInitial(DateTime adtStartDate, DateTime adtEndDate, string astrRetroPaymentType)
        {
            //Load all the Payment item types for payee account 
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            iclbMonthwiseAdjustmentDetail = new Collection<busPayeeAccountMonthwiseAdjustmentDetail>();
            Collection<busPayeeAccountPaymentItemType> lclbRetroPaymentItems = new Collection<busPayeeAccountPaymentItemType>();
            int lintMonthDifferenceBetweenDates = busGlobalFunctions.DateDiffByMonth(adtStartDate, adtEndDate);
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            foreach (busPayeeAccountPaymentItemType lobjPayeeAccountItemType in iclbPayeeAccountPaymentItemType)
            {
                if (lobjPayeeAccountItemType.ibusPaymentItemType == null)
                    lobjPayeeAccountItemType.LoadPaymentItemType();
                //Select the payment items which are having retro payment item type value null
                if (lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.retro_payment_type_value == null)
                {
                    //Check whether the payments items are valid for the next benefit paymetn date
                    if (busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date,
                        lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date))
                    {
                        //find retro payment item by passing the actual payment item and retro payment type
                        if (iclbRetroItemType == null)
                            LoadRetroItemType(astrRetroPaymentType);
                        busRetroItemType lobjRetroItemType = iclbRetroItemType.Where(o =>
                            o.icdoRetroItemType.from_item_type == lobjPayeeAccountItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code &&
                            o.icdoRetroItemType.payment_option_value == busConstant.PayeeAccountRetroPaymentOptionRegular).FirstOrDefault();

                        if (lobjRetroItemType != null)
                        {
                            if (iclbPaymentItemType == null)
                                LoadPaymentItemType();
                            busPaymentItemType lobjPaymentItemType = iclbPaymentItemType.Where(o =>
                                o.icdoPaymentItemType.item_type_code == lobjRetroItemType.icdoRetroItemType.to_item_type).FirstOrDefault();
                            if (lobjPaymentItemType != null && lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id > 0)
                            {
                                busPayeeAccountPaymentItemType lobjPaymentPayeeAccount = new busPayeeAccountPaymentItemType();
                                lobjPaymentPayeeAccount.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
                                lobjPaymentPayeeAccount.icdoPayeeAccountPaymentItemType.payee_account_id = icdoPayeeAccount.payee_account_id;
                                lobjPaymentPayeeAccount.icdoPayeeAccountPaymentItemType.payment_item_type_id
                                    = lobjPaymentItemType.icdoPaymentItemType.payment_item_type_id;
                                lobjPaymentPayeeAccount.icdoPayeeAccountPaymentItemType.amount
                                    = (lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount * lintMonthDifferenceBetweenDates);
                                lobjPaymentPayeeAccount.icdoPayeeAccountPaymentItemType.vendor_org_id
                                  = lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.vendor_org_id;
                                lobjPaymentPayeeAccount.iintOriginalPaymentItemTypeID = lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id;
                                lclbRetroPaymentItems.Add(lobjPaymentPayeeAccount);
                            }
                        }
                    }
                }
            }
            while (adtStartDate < idtNextBenefitPaymentDate)
            {
                LoadGrossAmount();
                LoadMonthwiseAdjustmentDetail(adtStartDate, idecGrossAmount, 0.0m, 0);
                adtStartDate = adtStartDate.AddMonths(1);
            }
            return lclbRetroPaymentItems;
        }
        public Collection<busPayeeAccountPaymentItemType> CalculateRetroPaymentItemsForUnderpayment(DateTime adtStartDate,
            DateTime adtEndDate, string astrRetroPaymentType)
        {
            DateTime ldtStartDate, ldtEndDate;
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            ldtStartDate = adtStartDate;
            if (iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            iclbMonthwiseAdjustmentDetail = new Collection<busPayeeAccountMonthwiseAdjustmentDetail>();
            //Loading gross benefit amt for a date
            Collection<busPayeeAccountPaymentItemType> lclbRetroPaymentItems = new Collection<busPayeeAccountPaymentItemType>();
            if (iclbRetroItemType == null)
                LoadRetroItemType(astrRetroPaymentType);

            int lintERPayeeAccountID = 0;
            DateTime ldtDisabilityPaymentDate = new DateTime();

            GetEarlyRetirementPayeeAccountId(ref lintERPayeeAccountID, ref ldtDisabilityPaymentDate);
            if (lintERPayeeAccountID > 0)
            {
                ldtStartDate = ldtDisabilityPaymentDate;
            }
            decimal ldecCOLAmount = 0.0M;
            while (ldtStartDate < idtNextBenefitPaymentDate)
            {
                busPayeeAccountPaymentItemType lobjCOLA = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                busPayeeAccountPaymentItemType lobjSuppCheck = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                ldtEndDate = ldtStartDate.AddMonths(1).AddDays(-1);
                Collection<busPaymentHistoryDetail> lclbPaidHistoryForTheMonth = GetTotalPaidHistoryForTheMonth(ldtStartDate, ldtEndDate, 0, lintERPayeeAccountID);
                IEnumerable<busPayeeAccountPaymentItemType> lclbCurrenPaymentItems = new Collection<busPayeeAccountPaymentItemType>();
                decimal ldecMonthlyDue = 0.0m;
                if (astrRetroPaymentType == busConstant.RetroPaymentTypeReactivation)
                {
                    lclbCurrenPaymentItems = iclbPayeeAccountPaymentItemType.Where(o =>
                    busGlobalFunctions.CheckDateOverlapping(ldtStartDate, o.icdoPayeeAccountPaymentItemType.start_date,
                       o.icdoPayeeAccountPaymentItemType.end_date));
                }
                else
                {
                    lclbCurrenPaymentItems = iclbPayeeAccountPaymentItemType.Where(o =>
                  busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date,
                     o.icdoPayeeAccountPaymentItemType.end_date));
                    //to get cola/adhoc amounts
                    GetCOLAOrAdhocAmount(ldtStartDate, ldtEndDate, lobjCOLA);
                    if (lobjCOLA.icdoPayeeAccountPaymentItemType.end_date != DateTime.MinValue && lobjCOLA.icdoPayeeAccountPaymentItemType.end_date < idtNextBenefitPaymentDate)
                        ldecCOLAmount += lobjCOLA.icdoPayeeAccountPaymentItemType.amount;
                    //to get supplemental check amounts
                    GetSupplementalCheckAmount(ldtStartDate, ldtEndDate, lobjSuppCheck);
                }
                foreach (busPayeeAccountPaymentItemType lobjPapit in lclbCurrenPaymentItems)
                {
                    if (lobjPapit.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1)
                    {
                        busPaymentHistoryDetail lobjHistoryItem = lclbPaidHistoryForTheMonth.Where(o =>
                            o.icdoPaymentHistoryDetail.payment_item_type_id == lobjPapit.icdoPayeeAccountPaymentItemType.payment_item_type_id).FirstOrDefault();

                        if (lobjHistoryItem != null)
                        {
                            busRetroItemType lobjRetroRef = iclbRetroItemType.Where(o =>
                                o.icdoRetroItemType.from_item_type == lobjHistoryItem.ibusPaymentItemType.icdoPaymentItemType.item_type_code &&
                                o.icdoRetroItemType.payment_option_value == busConstant.PayeeAccountRetroPaymentOptionRegular).FirstOrDefault();
                            if (lobjRetroRef != null)
                            {
                                if (lobjHistoryItem.ibusPaymentItemType == null)
                                    lobjHistoryItem.LoadPaymentItemType();
                                busPayeeAccountPaymentItemType lobjRetroPaymentItem =
                                CreatePayeeAccountPaymentItemType(GetPaymentItemTypeIDByItemCode(lobjRetroRef.icdoRetroItemType.to_item_type),
                                    lobjPapit.icdoPayeeAccountPaymentItemType.amount - lobjHistoryItem.icdoPaymentHistoryDetail.amount
                                    , string.Empty, lobjHistoryItem.icdoPaymentHistoryDetail.vendor_org_id,
                                    idtNextBenefitPaymentDate, DateTime.MinValue);
                                lobjRetroPaymentItem.iintOriginalPaymentItemTypeID = lobjPapit.icdoPayeeAccountPaymentItemType.payment_item_type_id;
                                lclbRetroPaymentItems.Add(lobjRetroPaymentItem);
                                ldecMonthlyDue +=
                                     lobjPapit.icdoPayeeAccountPaymentItemType.amount - lobjHistoryItem.icdoPaymentHistoryDetail.amount;
                            }
                        }
                        else
                        {
                            busRetroItemType lobjRetroRef = iclbRetroItemType.Where(o =>
                               o.icdoRetroItemType.from_item_type == lobjPapit.ibusPaymentItemType.icdoPaymentItemType.item_type_code).FirstOrDefault();
                            if (lobjRetroRef != null)
                            {
                                busPayeeAccountPaymentItemType lobjRetroPaymentItem =
                                CreatePayeeAccountPaymentItemType(GetPaymentItemTypeIDByItemCode(lobjRetroRef.icdoRetroItemType.to_item_type),
                                    lobjPapit.icdoPayeeAccountPaymentItemType.amount, string.Empty, lobjPapit.icdoPayeeAccountPaymentItemType.vendor_org_id,
                                    idtNextBenefitPaymentDate, DateTime.MinValue);

                                ldecMonthlyDue += lobjPapit.icdoPayeeAccountPaymentItemType.amount;

                                lclbRetroPaymentItems.Add(lobjRetroPaymentItem);
                            }
                        }
                    }
                }
                if (astrRetroPaymentType != busConstant.RetroPaymentTypeReactivation)
                {
                    ldecMonthlyDue += ldecCOLAmount;
                    ldecMonthlyDue += lobjSuppCheck.icdoPayeeAccountPaymentItemType.amount;
                    //uat pir 1375
                    //adding cola item to retro item detail
                    if (lobjCOLA.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id > 0 &&
                        lobjCOLA.icdoPayeeAccountPaymentItemType.end_date != DateTime.MinValue &&
                        lobjCOLA.icdoPayeeAccountPaymentItemType.end_date < idtNextBenefitPaymentDate)
                    {
                        lobjCOLA.LoadPaymentItemType();
                        busRetroItemType lobjRetroRef = iclbRetroItemType.Where(o =>
                                  o.icdoRetroItemType.from_item_type == lobjCOLA.ibusPaymentItemType.icdoPaymentItemType.item_type_code &&
                                    o.icdoRetroItemType.payment_option_value == busConstant.PayeeAccountRetroPaymentOptionRegular).FirstOrDefault();
                        if (lobjRetroRef != null)
                        {
                            busPayeeAccountPaymentItemType lobjRetroPaymentItem =
                            CreatePayeeAccountPaymentItemType(GetPaymentItemTypeIDByItemCode(lobjRetroRef.icdoRetroItemType.to_item_type),
                                lobjCOLA.icdoPayeeAccountPaymentItemType.amount, string.Empty, lobjCOLA.icdoPayeeAccountPaymentItemType.vendor_org_id,
                                idtNextBenefitPaymentDate, DateTime.MinValue);

                            lclbRetroPaymentItems.Add(lobjRetroPaymentItem);
                        }
                    }
                    //uat pir 1375
                    //adding supplemental item to retro item detail
                    if (lobjSuppCheck.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id > 0)
                    {
                        lobjSuppCheck.LoadPaymentItemType();
                        busRetroItemType lobjRetroRef = iclbRetroItemType.Where(o =>
                                  o.icdoRetroItemType.from_item_type == lobjSuppCheck.ibusPaymentItemType.icdoPaymentItemType.item_type_code &&
                                    o.icdoRetroItemType.payment_option_value == busConstant.PayeeAccountRetroPaymentOptionRegular).FirstOrDefault();
                        if (lobjRetroRef != null)
                        {
                            busPayeeAccountPaymentItemType lobjRetroPaymentItem =
                            CreatePayeeAccountPaymentItemType(GetPaymentItemTypeIDByItemCode(lobjRetroRef.icdoRetroItemType.to_item_type),
                                lobjSuppCheck.icdoPayeeAccountPaymentItemType.amount, string.Empty, lobjSuppCheck.icdoPayeeAccountPaymentItemType.vendor_org_id,
                                idtNextBenefitPaymentDate, DateTime.MinValue);

                            lclbRetroPaymentItems.Add(lobjRetroPaymentItem);
                        }
                    }
                }
                foreach (busPaymentHistoryDetail lobjHistoryDetail in lclbPaidHistoryForTheMonth)
                {
                    if (lclbCurrenPaymentItems.Where(o =>
                        o.icdoPayeeAccountPaymentItemType.payment_item_type_id == lobjHistoryDetail.icdoPaymentHistoryDetail.payment_item_type_id).Count() == 0)
                    {
                        busRetroItemType lobjRetroRef = iclbRetroItemType.Where(o =>
                     o.icdoRetroItemType.from_item_type == lobjHistoryDetail.ibusPaymentItemType.icdoPaymentItemType.item_type_code).FirstOrDefault();
                        if (lobjRetroRef != null)
                        {
                            busPayeeAccountPaymentItemType lobjRetroPaymentItem =
                           CreatePayeeAccountPaymentItemType(GetPaymentItemTypeIDByItemCode(lobjRetroRef.icdoRetroItemType.to_item_type),
                               0.0m - lobjHistoryDetail.icdoPaymentHistoryDetail.amount, string.Empty, lobjHistoryDetail.icdoPaymentHistoryDetail.vendor_org_id,
                               idtNextBenefitPaymentDate, DateTime.MinValue);

                            ldecMonthlyDue -= lobjHistoryDetail.icdoPaymentHistoryDetail.amount;

                            lclbRetroPaymentItems.Add(lobjRetroPaymentItem);
                        }
                    }
                }

                LoadMonthwiseAdjustmentDetail(ldtStartDate, ldecMonthlyDue, 0.0m, 0);
                ldtStartDate = ldtStartDate.AddMonths(1);
            }
            return lclbRetroPaymentItems;
        }
        //Load Monthwise details for overpayment or underpayment
        public void LoadMonthwiseAdjustmentDetail(DateTime adtEffectiveDate, decimal adecAmount, decimal adecInterest, int aintRefID)
        {
            if (iclbMonthwiseAdjustmentDetail == null)
                iclbMonthwiseAdjustmentDetail = new Collection<busPayeeAccountMonthwiseAdjustmentDetail>();
            busPayeeAccountMonthwiseAdjustmentDetail lobjMonthwiseDetail = new busPayeeAccountMonthwiseAdjustmentDetail
            {
                icdoPayeeAccountMonthwiseAdjustmentDetail = new cdoPayeeAccountMonthwiseAdjustmentDetail()
            };
            lobjMonthwiseDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date = adtEffectiveDate;
            lobjMonthwiseDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.amount = adecAmount;
            lobjMonthwiseDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount = adecInterest;
            iclbMonthwiseAdjustmentDetail.Add(lobjMonthwiseDetail);
        }
        /// <summary>
        /// Load Payment Item Types which can be rolled over for the specific payee account
        /// </summary>
        public void LoadPaymentItemTypesToRollover()
        {
            if (iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            if (iclbPaymentItemTypesToRollover == null)
                iclbPaymentItemTypesToRollover = new Collection<busPayeeAccountPaymentItemType>();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            if (ibusApplication == null)
                LoadApplication();
            foreach (busPayeeAccountPaymentItemType lobjPaymentItemType in iclbPayeeAccountPaymentItemType)
            { //Take only the items which can be rolled over
                if (lobjPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolleverCodeValueCanBeRolled)
                {
                    //Take only the items which are effective for the next benfit payment date
                    if ((lobjPaymentItemType.icdoPayeeAccountPaymentItemType.start_date <= idtNextBenefitPaymentDate) &&
                        (lobjPaymentItemType.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue))
                    {
                        if (lobjPaymentItemType.ibusPaymentItemType == null)
                            lobjPaymentItemType.LoadPaymentItemType();
                        if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                        {
                            if ((ibusApplication.icdoBenefitApplication.plso_requested_flag == busConstant.Flag_Yes)
                                && (lobjPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.plso_flag == busConstant.Flag_Yes))
                            {
                                iclbPaymentItemTypesToRollover.Add(lobjPaymentItemType);
                            }
                            else if (ibusApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOption5YearTermLife)
                            {
                                iclbPaymentItemTypesToRollover.Add(lobjPaymentItemType);
                            }
                        }
                        else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                        {
                            iclbPaymentItemTypesToRollover.Add(lobjPaymentItemType);
                        }
                        else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath ||
                            icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                        {
                            if (ibusApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionAutoRefund ||
                                ibusApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionRefund ||
                                ibusApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOption5YearTermLife)
                            {
                                iclbPaymentItemTypesToRollover.Add(lobjPaymentItemType);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load Payment Item Types which can be rolled over for the specific payee account
        /// </summary>
        public void LoadPaymentItemTypesToRolloverForReissue(DateTime adtEffectivedate)
        {
            if (iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            if (iclbPaymentItemTypesToRollover == null)
                iclbPaymentItemTypesToRollover = new Collection<busPayeeAccountPaymentItemType>();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            if (ibusApplication == null)
                LoadApplication();
            foreach (busPayeeAccountPaymentItemType lobjPaymentItemType in iclbPayeeAccountPaymentItemType)
            { //Take only the items which can be rolled over
                if (lobjPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value == busConstant.RolleverCodeValueCanBeRolled)
                {
                    //Take only the items which are effective for adtEffective date(payment date for payment history header which is reissued)
                    if ((lobjPaymentItemType.icdoPayeeAccountPaymentItemType.start_date <= adtEffectivedate) &&
                        (lobjPaymentItemType.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue ||
                        lobjPaymentItemType.icdoPayeeAccountPaymentItemType.end_date >= adtEffectivedate))
                    {
                        if (lobjPaymentItemType.ibusPaymentItemType == null)
                            lobjPaymentItemType.LoadPaymentItemType();
                        if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                        {
                            if ((ibusApplication.icdoBenefitApplication.plso_requested_flag == busConstant.Flag_Yes)
                                && (lobjPaymentItemType.ibusPaymentItemType.icdoPaymentItemType.plso_flag == busConstant.Flag_Yes))
                            {
                                iclbPaymentItemTypesToRollover.Add(lobjPaymentItemType);
                            }
                            else if (ibusApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOption5YearTermLife)
                            {
                                iclbPaymentItemTypesToRollover.Add(lobjPaymentItemType);
                            }
                        }
                        else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                        {
                            iclbPaymentItemTypesToRollover.Add(lobjPaymentItemType);
                        }
                        //both pre and post conditions are same
                        else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath ||
                            icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                        {
                            if (ibusApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionAutoRefund ||
                                ibusApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOptionRefund ||
                                ibusApplication.icdoBenefitApplication.benefit_option_value == busConstant.BenefitOption5YearTermLife)
                            {
                                iclbPaymentItemTypesToRollover.Add(lobjPaymentItemType);
                            }
                        }
                    }
                }
            }
        }
        // Since the Active Status of a payee is obtained by getting the latest record status, 
        // Inserting a Review record changes the Payee Account Status to Review        
        public void CreateReviewPayeeAccountStatus()
        {
            string lstrStatus = string.Empty;
            //commented since we need to reload payee account status every time
            //if (ibusPayeeAccountActiveStatus == null)
            LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            if (ibusPayeeAccountActiveStatus == null)
            {
                CreateReviewStatus();
            }
            else
            {
                lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                if (IsReviewStatusAllowed())
                {
                    CreateReviewStatus();
                }//PIR 1496                
                else if (lstrStatus == busConstant.PayeeAccountStatusRetirementReview)
                {
                    ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.modified_by = iobjPassInfo.istrUserID;
                    ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.Update();
                }
            }
        }
        public DateTime idtStatusEffectiveDate { get; set; }

        public void CreateReviewStatus()
        {
            busPayeeAccountStatus lobjPayeeAccountStatus = new busPayeeAccountStatus();
            lobjPayeeAccountStatus.icdoPayeeAccountStatus = new cdoPayeeAccountStatus();
            lobjPayeeAccountStatus.icdoPayeeAccountStatus.payee_account_id = icdoPayeeAccount.payee_account_id;
            if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PayeeAccountStatusRefundReview;
            }
            else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
            {
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PayeeAccountStatusRetirementReview;
            }
            else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PayeeAccountStatusDisabilityReview;
            }
            else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
            {
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PayeeAccountStatusPreDeathReview;
            }
            else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
            {
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PayeeAccountStatusPostDeathReview;
            }
            lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = idtStatusEffectiveDate != DateTime.MinValue ? idtStatusEffectiveDate : DateTime.Today;
            lobjPayeeAccountStatus.icdoPayeeAccountStatus.Insert();
        }
        //Check before changing the status to review and 
        public bool IsReviewStatusAllowed()
        {
            string lstrStatus = string.Empty;
            bool lblnIsReviewStatusAllowed = true;
            if (ibusPayeeAccountActiveStatus == null)
                LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            if (ibusPayeeAccountActiveStatus != null)
            {
                //Check for the Active status,if it is not review and then allow to change the status to review
                lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                if (lstrStatus == busConstant.PayeeAccountStatusRetirementReview ||  //PIR 11946
                    lstrStatus == busConstant.PayeeAccountStatusTerminated || lstrStatus == busConstant.PayeeAccountStatusCancelled)
                {
                    lblnIsReviewStatusAllowed = false;
                }
            }
            return lblnIsReviewStatusAllowed;
        }
        //Visible Rule for changing the payee status
        public bool IsStatusUpdateAllowed()
        {
            string lstrStatus = string.Empty;
            bool lblnIsReviewStatusAllowed = true;
            if (ibusPayeeAccountActiveStatus == null)
                LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            if (ibusPayeeAccountActiveStatus != null)
            {
                lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                if (//lstrStatus == busConstant.PayeeAccountStatusRefundProcessed || //uat pir 1420
                    lstrStatus == busConstant.PayeeAccountStatusTerminated || lstrStatus == busConstant.PayeeAccountStatusCancelled)
                {
                    lblnIsReviewStatusAllowed = false;
                }
            }
            return lblnIsReviewStatusAllowed;
        }
        /// <summary>
        /// Method to insert Payment complete status into Payee Accout status table
        /// </summary>
        public void CreatePaymentCompletePayeeAccountStatus()
        {
            string lstrStatus = string.Empty;
            if (ibusPayeeAccountActiveStatus == null)
                LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            if (ibusPayeeAccountActiveStatus != null)
            {
                //Check for the Active status,if it is not Paymentscompleted and then allow to change the status
                lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
            }
            if (lstrStatus != busConstant.PayeeAccountStatusRetirementPaymentCompleted)
            {
                busPayeeAccountStatus lobjPayeeAccountStatus = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.payee_account_id = icdoPayeeAccount.payee_account_id;
                lobjPayeeAccountStatus.ibusPayeeAccount = this;
                if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
                {
                    lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PayeeAccountStatusDisabilityPaymentCompleted;
                }
                else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                {
                    lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PayeeAccountStatusPreRetirementDeathPaymentCompleted;
                }
                else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath)
                {
                    lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PayeeAccountStatusPostRetirementDeathPaymentCompleted;
                }
                else if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement)
                {
                    lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_value = busConstant.PayeeAccountStatusRetirementPaymentCompleted;
                }
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.status_effective_date = DateTime.Today;
                lobjPayeeAccountStatus.icdoPayeeAccountStatus.Insert();

                //UCS-055 BR 149: End the Rhic Record When Payment Completed. //PIR 14346 - Commented
                //lobjPayeeAccountStatus.CreateRHICCombineOnPaymentCompleted();
            }
        }

        public busPayeeAccountPaymentItemType CreatePayeeAccountPaymentItemType(int aintPaymentItemTypeID, decimal adecAmount, string astrAccountNo, int aintVendorOrgID,
                                                                                       DateTime adteStartDate, DateTime adtEndDate, int aintPersonAccountID = 0)
        {
            busPayeeAccountPaymentItemType lobjPayeeAccountItemType = new busPayeeAccountPaymentItemType();
            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType();
            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payee_account_id = icdoPayeeAccount.payee_account_id;
            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id = aintPaymentItemTypeID;
            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount = adecAmount;
            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.account_number = astrAccountNo;
            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.vendor_org_id = aintVendorOrgID;
            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date = adteStartDate;
            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date = adtEndDate;
            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.person_account_id = aintPersonAccountID;
            if (iintBatchScheudleID > 0)
                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = iintBatchScheudleID;
            lobjPayeeAccountItemType.iobjPassInfo.iintPersonID = icdoPayeeAccount.payee_perslink_id;
            return lobjPayeeAccountItemType;
        }

        public int CreatePayeeAccountPaymentItemType(int aintPaymentItemTypeID, decimal adecAmount, string astrAccountNo, int aintVendorOrgID,
                                                                                DateTime adteStartDate, DateTime adtEndDate, bool ablnForceInsert, bool ablnCheckVendorOrg = false, int aintPersonAccountID = 0, bool ablnUpdateFlag = false, bool ablnIsFromIBsBilling = false,
                                                                                bool ablnIsRTWRecalculate = false)
        {
            DateTime ldtPreviousEndDate = DateTime.MinValue;
            if ((aintPaymentItemTypeID != 0) &&
                (adteStartDate != DateTime.MinValue) &&
                (icdoPayeeAccount.payee_account_id != 0))
            {
                bool lblnCreate = false;
                if (ablnForceInsert && !iblnIsFromMSS) //pir 8618
                {
                    lblnCreate = true;
                }
                //Below block is for handling PAPIT creation of same item for multiple vendor org id
                else if (ablnCheckVendorOrg)
                {
                    if (idtNextBenefitPaymentDate == DateTime.MinValue)
                        LoadNexBenefitPaymentDate();
                    if (iclbPayeeAccountPaymentItemType == null)
                        LoadPayeeAccountPaymentItemType();
                    busPayeeAccountPaymentItemType lobjPayeeAccountItemType
                        = iclbPayeeAccountPaymentItemType.Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id == aintPaymentItemTypeID &&
                                                                     o.icdoPayeeAccountPaymentItemType.payee_account_id == icdoPayeeAccount.payee_account_id &&
                                                                     o.icdoPayeeAccountPaymentItemType.end_date_no_null >= idtNextBenefitPaymentDate &&
                                                                     o.icdoPayeeAccountPaymentItemType.vendor_org_id == aintVendorOrgID).FirstOrDefault();

                    if (lobjPayeeAccountItemType != null)
                    {
                        //as per meeting with satya on Aug 13,2010 : need to set to null, if modified from screen
                        lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = iintBatchScheudleID;

                        //added for UCS - 079, after recalculation is done, if an item have amount as zero, we need to enddate the existing one
                        if (adecAmount == 0.0M)
                        {
                            busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
                            lobjPaymentItemType.FindPaymentItemType(lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id);
                            if (lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date < idtNextBenefitPaymentDate)
                            {
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date = idtNextBenefitPaymentDate.AddDays(-1);
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Update();
                            }
                            else
                            {
                                if (iclbTaxWithholingHistory == null)
                                    LoadTaxWithHoldingHistory();
                                foreach (busPayeeAccountTaxWithholding lobjtaxwithholding in iclbTaxWithholingHistory)
                                {
                                    lobjtaxwithholding.LoadTaxWithHoldingTaxItems();
                                    foreach (busPayeeAccountTaxWithholdingItemDetail lobjTaxWithholdingItemDetail in lobjtaxwithholding.iclbTaxWithHoldingTaxItems)
                                        if (lobjTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id ==
                                            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id)
                                            lobjTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.Delete();
                                }
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Delete();
                            }
                        }
                        else
                            if (lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date < idtNextBenefitPaymentDate)
                        {
                            if (ibusPayeeAccountActiveStatus == null)
                                LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                            if (lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date == adteStartDate)
                            {
                                if (IsPayeeAccountStatusProcessedORRecieving())
                                {
                                    return 0;
                                }
                                else
                                {
                                    lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount = adecAmount;
                                    lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Update();
                                    lblnCreate = false;
                                    return lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
                                }
                            }
                            else
                            {
                                if (lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date != DateTime.MinValue)
                                    ldtPreviousEndDate = lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date;

                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date = adteStartDate.AddDays(-1);
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Update();
                                lblnCreate = true;
                            }
                        }
                        else
                        {
                            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount = adecAmount;
                            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Update();
                            lblnCreate = false;
                            return lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
                        }
                    }
                    else
                    {
                        lblnCreate = true;
                    }
                }
                else
                {
                    if (idtNextBenefitPaymentDate == DateTime.MinValue)
                        LoadNexBenefitPaymentDate();
                    if (iclbPayeeAccountPaymentItemType == null)
                    {
                        //PIR 18053
                        if (ablnIsRTWRecalculate)
                            LoadPayeeAccountPaymentItemTypeRTW();
                        else
                            LoadPayeeAccountPaymentItemType();
                    }
                    busPayeeAccountPaymentItemType lobjPayeeAccountItemType = new busPayeeAccountPaymentItemType();
                    if (iblnIsMSSTaxWithholding && iclbPayeeAccountPaymentItemType?.Count > 0) //PIR 20387
                    {
                        iclbPayeeAccountPaymentItemType = iclbPayeeAccountPaymentItemType
                                                            .Where(taxwithholding =>
                                                            taxwithholding.icdoPayeeAccountPaymentItemType.start_date.Date !=
                                                            taxwithholding.icdoPayeeAccountPaymentItemType.end_date.Date)
                                                            .OrderByDescending(taxwithholding => taxwithholding.icdoPayeeAccountPaymentItemType.start_date)
                                                            .ToList()
                                                            .ToCollection();
                    }
                    if (aintPersonAccountID == 0)
                    {
                        lobjPayeeAccountItemType
                            = iclbPayeeAccountPaymentItemType.Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id == aintPaymentItemTypeID &&
                                                                         o.icdoPayeeAccountPaymentItemType.payee_account_id == icdoPayeeAccount.payee_account_id &&
                                                                         o.icdoPayeeAccountPaymentItemType.end_date_no_null >= idtNextBenefitPaymentDate).FirstOrDefault();
                    }
                    else if (aintPersonAccountID > 0)
                    {
                        lobjPayeeAccountItemType
                            = iclbPayeeAccountPaymentItemType.Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id == aintPaymentItemTypeID &&
                                                                         o.icdoPayeeAccountPaymentItemType.payee_account_id == icdoPayeeAccount.payee_account_id &&
                                                                         o.icdoPayeeAccountPaymentItemType.person_account_id == aintPersonAccountID &&
                                                                         o.icdoPayeeAccountPaymentItemType.end_date_no_null >= idtNextBenefitPaymentDate).FirstOrDefault();
                    }

                    if (lobjPayeeAccountItemType != null)
                    {
                        //as per meeting with satya on Aug 13,2010 : need to set to null, if modified from screen
                        lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.batch_schedule_id = iintBatchScheudleID;

                        //added for UCS - 079, after recalculation is done, if an item have amount as zero, we need to enddate the existing one
                        if (adecAmount == 0.0M)
                        {
                            busPaymentItemType lobjPaymentItemType = new busPaymentItemType();
                            lobjPaymentItemType.FindPaymentItemType(lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payment_item_type_id);
                            if (lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date < idtNextBenefitPaymentDate)
                            {
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date = idtNextBenefitPaymentDate.AddDays(-1);
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Update();
                            }
                            else
                            {
                                if (iclbTaxWithholingHistory == null)
                                    LoadTaxWithHoldingHistory();
                                foreach (busPayeeAccountTaxWithholding lobjtaxwithholding in iclbTaxWithholingHistory)
                                {
                                    lobjtaxwithholding.LoadTaxWithHoldingTaxItems();
                                    foreach (busPayeeAccountTaxWithholdingItemDetail lobjTaxWithholdingItemDetail in lobjtaxwithholding.iclbTaxWithHoldingTaxItems)
                                        if (lobjTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.payee_account_payment_item_type_id ==
                                            lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id)
                                            lobjTaxWithholdingItemDetail.icdoPayeeAccountTaxWithholdingItemDetail.Delete();
                                }
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Delete();
                            }
                        }
                        else
                            if (lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date < idtNextBenefitPaymentDate)
                        {
                            if (ibusPayeeAccountActiveStatus == null)
                                LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                            if (lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date == adteStartDate)
                            {
                                if (IsPayeeAccountStatusProcessedORRecieving())
                                {
                                    return 0;
                                }
                                else
                                {
                                    lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount = adecAmount;
                                    lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Update();
                                    lblnCreate = false;
                                    return lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
                                }
                            }
                            else
                            {
                                if (lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date != DateTime.MinValue)
                                    ldtPreviousEndDate = lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date;

                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date = adteStartDate.AddDays(-1);
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Update();
                                lblnCreate = true;
                            }
                        }
                        else
                        {
                            busPaymentItemType lobjPaymentItemType = new busPaymentItemType() { icdoPaymentItemType = new cdoPaymentItemType() };
                            lobjPaymentItemType.FindPaymentItemType(aintPaymentItemTypeID);
                            if ((lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRothRolloverFederalTaxAmount || lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRothRolloverStateTaxAmount || lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSORothRolloverFederalTaxAmount || lobjPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSORothRolloverStateTaxAmount) && iblnIsRollOverFromScreen == true)
                            {
                                lblnCreate = true;
                            }
                            else
                            {
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount = adecAmount;
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Update();
                                lblnCreate = false;
                                return lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
                            }

                            /*if (lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.start_date <= adteStartDate)
                            {
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.amount = adecAmount;
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Update();
                                lblnCreate = false;
                                return lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
                            }
                            else
                            {
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.end_date = adteStartDate.AddDays(-1);
                                lobjPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Update();
                                lblnCreate = true;
                            }*/
                        }
                    }
                    else
                    {
                        lblnCreate = true;
                    }
                }
                iblnCreateFlag = false;
                if ((lblnCreate) && (adecAmount > 0))
                {
                    if (ablnIsFromIBsBilling && ldtPreviousEndDate > DateTime.MinValue)
                        adtEndDate = ldtPreviousEndDate;

                    busPayeeAccountPaymentItemType lbusPayeeAccountItemType = CreatePayeeAccountPaymentItemType(aintPaymentItemTypeID, adecAmount, astrAccountNo, aintVendorOrgID, adteStartDate, adtEndDate, aintPersonAccountID);
                    lbusPayeeAccountItemType.icdoPayeeAccountPaymentItemType.Insert();
                    iblnCreateFlag = true;
                    return lbusPayeeAccountItemType.icdoPayeeAccountPaymentItemType.payee_account_payment_item_type_id;
                }
            }
            return 0;
        }
        public bool IsPayeeAccountStatusProcessedORRecieving()
        {
            DataTable ldtb = this.iobjPassInfo.isrvDBCache.GetCodeDescription(2203, ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value);
            if (ldtb.Rows.Count > 0)
            {
                if ((ldtb.Rows[0]["data2"].ToString() == "RECV") || (ldtb.Rows[0]["data2"].ToString() == "TRMD") || (ldtb.Rows[0]["data2"].ToString() == "DCRC")
                    || (ldtb.Rows[0]["data2"].ToString() == "RPCD") || (ldtb.Rows[0]["data2"].ToString() == "CNLD"))
                {
                    return true;
                }
            }
            return false;
        }
        public int CreatePayeeAccountPaymentItemType(string astrItemCode, decimal adecAmount, string astrAccountNo, int aintVendorOrgID,
                                                                                DateTime adteStartDate, DateTime adteEndDate, int aintPersonAccountID = 0)
        {
            int lintPayeeAccountPIT = 0;
            int lintPaymentItemTypeID = GetPaymentItemTypeIDByItemCode(astrItemCode);
            lintPayeeAccountPIT = CreatePayeeAccountPaymentItemType(lintPaymentItemTypeID, adecAmount, astrAccountNo, aintVendorOrgID, adteStartDate, adteEndDate, true, false, aintPersonAccountID);
            return lintPayeeAccountPIT;
        }
        public int CreatePayeeAccountPaymentItemType(string astrItemCode, decimal adecAmount, string astrAccountNo, int aintVendorOrgID,
                                                                               DateTime adteStartDate, DateTime adteEndDate, bool ablnForceInsert, bool ablnCheckVendorOrg = false, int aintPersonAccountID = 0, bool ablnIsFromIBS = false,
                                                                               bool ablnIsRTWRecalculate = false)
        {
            int lintPayeeAccountPIT = 0;
            int lintPaymentItemTypeID = GetPaymentItemTypeIDByItemCode(astrItemCode);
            lintPayeeAccountPIT = CreatePayeeAccountPaymentItemType(lintPaymentItemTypeID, adecAmount, astrAccountNo, aintVendorOrgID, adteStartDate, adteEndDate, ablnForceInsert, ablnCheckVendorOrg, aintPersonAccountID, ablnIsFromIBsBilling: ablnIsFromIBS, ablnIsRTWRecalculate: ablnIsRTWRecalculate);
            return lintPayeeAccountPIT;
        }

        //PIR 18053 - Insert prev accounts PAPIT.
        public void CreatePayeeAccountPaymentItemTypeRTW(int aintPreRTWPayeeAccountID, DateTime adtePaymentDate)
        {
            DataTable ldtbPreRTWPAPIT = Select("cdoPayeeAccountPaymentItemType.LoadPayeeAccountPaymentItemTypeRTW", new object[2] { aintPreRTWPayeeAccountID, adtePaymentDate });

            if (ldtbPreRTWPAPIT.Rows.Count > 0)
            {
                Collection<busPayeeAccountPaymentItemType> lclbPayeeAccountPaymentItemType = new Collection<busPayeeAccountPaymentItemType>();
                lclbPayeeAccountPaymentItemType = GetCollection<busPayeeAccountPaymentItemType>(ldtbPreRTWPAPIT, "icdoPayeeAccountPaymentItemType");

                foreach (busPayeeAccountPaymentItemType lobjPAPIT in lclbPayeeAccountPaymentItemType)
                {
                    busPayeeAccountPaymentItemType lobjPreRTWPAPIT = new busPayeeAccountPaymentItemType() { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.payee_account_id = icdoPayeeAccount.payee_account_id;
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.payment_item_type_id = lobjPAPIT.icdoPayeeAccountPaymentItemType.payment_item_type_id;
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.amount = lobjPAPIT.icdoPayeeAccountPaymentItemType.amount;
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.start_date = lobjPAPIT.icdoPayeeAccountPaymentItemType.start_date;
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.end_date = lobjPAPIT.icdoPayeeAccountPaymentItemType.end_date;
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.vendor_org_id = lobjPAPIT.icdoPayeeAccountPaymentItemType.vendor_org_id;
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.account_number = lobjPAPIT.icdoPayeeAccountPaymentItemType.account_number;
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.batch_schedule_id = lobjPAPIT.icdoPayeeAccountPaymentItemType.batch_schedule_id;
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.reissue_item_flag = lobjPAPIT.icdoPayeeAccountPaymentItemType.reissue_item_flag;
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.miscellaneous_correction_flag = lobjPAPIT.icdoPayeeAccountPaymentItemType.miscellaneous_correction_flag;
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.pre_rtw_payee_account_id = aintPreRTWPayeeAccountID;
                    lobjPreRTWPAPIT.icdoPayeeAccountPaymentItemType.Insert();
                }
            }
        }



        public void CreateRolloverAdjustment()
        {
            if (iclbActiveRolloverDetails == null)
                LoadActiveRolloverDetail();
            foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in iclbActiveRolloverDetails)
            {
                if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.rollover_option_value != busConstant.PayeeAccountRolloverOptionAmountOfTaxable)
                {
                    if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusProcessed)
                    {
                        lobjRolloverDetail.icdoPayeeAccountRolloverDetail.status_value = busConstant.PayeeAccountRolloverDetailStatusActive;
                        lobjRolloverDetail.icdoPayeeAccountRolloverDetail.Update();
                    }
                    //lobjRolloverDetail.DeleteRolledverItems();
                    lobjRolloverDetail.iblnIsRolloverToBeAdjusted = true;
                    lobjRolloverDetail.CreatRolloverAdjustment();
                }
                else
                {
                    // PIR 21180
                    CalculateAdjustmentTax(false);
                }
            }
        }

        //UAT PIR 1173 if RHIC amount is greater than 3rd party amount ,throw an error
        public bool IsRHICGreaterThan3rdPary()
        {
            //Reload papit items
            LoadPayeeAccountPaymentItemType();
            decimal ldecRHICAMount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PaymentItemTypeIDRHICBenefitReimbursement &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)).Sum(o => o.icdoPayeeAccountPaymentItemType.amount);
            decimal ldec3rdPartyAmount = iclbPayeeAccountPaymentItemType.Where(o =>
               (o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PaymentItemTypeID3rdPartyRefund ||
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PaymentItemTypeID3rdPartyHealthInsurance) &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)).Sum(o => o.icdoPayeeAccountPaymentItemType.amount);
            if (ldecRHICAMount > ldec3rdPartyAmount)
            {
                return true;
            }
            return false;
        }
        //Batch Schedule ID Used for posting into papit
        public int iintBatchScheudleID { get; set; }

        //Recalculate tax for the payee account
        public void CalculateAdjustmentTax(bool ablnApproveClick)
        {
            bool lblnCheckUpdate = true;
            if (ablnApproveClick)
            {
                lblnCheckUpdate = false;
            }

            //Re Load always - to refresh the collection when multiple time called for same payee account
            LoadTaxWithHoldingHistory();

            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            foreach (busPayeeAccountTaxWithholding lobjTaxWithHolding in iclbTaxWithholingHistory)
            {
                lobjTaxWithHolding.ibusPayeeAccount = this;
                //PIR 25946 - the system should not have recalculated RMD taxes 
                if ((lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value != busConstant.BenefitDistributionRMD)
                    && busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate,
                    lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.start_date, lobjTaxWithHolding.icdoPayeeAccountTaxWithholding.end_date))
                {
                    lobjTaxWithHolding.CalculateTax(false);
                    if (!iblnIsRolloverAmountReissuedToMember)
                    {
                        lobjTaxWithHolding.PostTaxAmount(lblnCheckUpdate);
                    }
                    else
                    {
                        lobjTaxWithHolding.PostTaxAmountForReissuedAmount();
                    }
                }
                if (!iblnIsRolloverAmountReissuedToMember)
                {
                    lobjTaxWithHolding.DeleteOldItems();
                }
            }
        }
        public void CreateACHAdjustment()
        {
            if (iclbAchDetail == null)
                LoadACHDetail();
            iclbAchDetail = busGlobalFunctions.Sort<busPayeeAccountAchDetail>("icdoPayeeAccountAchDetail.ach_start_date desc", iclbAchDetail);
            foreach (busPayeeAccountAchDetail lobjACHDetail in iclbAchDetail)
            {
                if (lobjACHDetail.icdoPayeeAccountAchDetail.primary_account_flag == busConstant.Flag_Yes)
                {
                    lobjACHDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.MinValue;
                    lobjACHDetail.icdoPayeeAccountAchDetail.Update();
                    break;
                }
            }
            foreach (busPayeeAccountAchDetail lobjACHDetail in iclbAchDetail)
            {
                if (lobjACHDetail.icdoPayeeAccountAchDetail.primary_account_flag == busConstant.Flag_No)
                {
                    lobjACHDetail.icdoPayeeAccountAchDetail.ach_end_date = DateTime.MinValue;
                    lobjACHDetail.icdoPayeeAccountAchDetail.Update();
                    break;
                }
            }
        }
        public override int PersistChanges()
        {
            iblnPayeeAccountInfoChangeIndicator = true;
            int lintrtn = base.PersistChanges();
            if (iclbTaxWithholingHistory == null)
                LoadTaxWithHoldingHistory();
            if (iclbTaxWithholingHistory.Count == 0)
            {
                //PIR 8858 -- transfers should not have any requirements to withhold taxes. 
                if (ibusApplication == null)
                    LoadApplication();
                if (!ibusApplication.IsBenefitOptionTransfers())
                    iblnNoTaxWithholdingRecordIndicator = true;
            }
            return lintrtn;
        }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            base.BeforeValidate(aenmPageMode);
            LoadRetirementBenefitCalculation();
        }

        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            //Systest PIR - 2260
            //On payee account screen only editable fields are Only 'Pull Check' and 'Disability Recertification Date' and these doesnt require payee account status to review
            //CreateReviewPayeeAccountStatus();
            //LoadActivePayeeStatus();
        }

        public override busBase GetCorPerson()
        {
            if (ibusPayee == null)
                LoadPayee();
            return ibusPayee;
        }

        public override void LoadCorresProperties(string astrTemplateName)
        {
            if (ibusApplication == null)
                LoadApplication();
            if (ibusApplication.ibusPersonAccount == null)
                ibusApplication.LoadPersonAccount();
            if (ibusPlan == null)
                LoadPlan();
            if (icdoPayeeAccount.application_id > 0)
            {
                if (ibusRetirementDisabilityApplication == null)
                    LoadRetirementDisabilityApplication();

                ibusRetirementDisabilityApplication.LoadNormalEligibilityDetails();

                LoadNontaxableAmount(idtNextBenefitPaymentDate);
            }
            //to check whther payee account created from benefit calculation
            //from UCS - 075 , dro payee account can also come, which will break " ibusBenefitCalculaton.GetNormalEligibilityDetails();" function
            if (icdoPayeeAccount.calculation_id > 0)
            {
                //******* UCS-080 correspondence
                if (ibusBenefitCalculaton == null)
                    LoadBenefitCalculation();
                ibusBenefitCalculaton.GetNormalEligibilityDetails();
                //load benefit amounts for cor - PAY - 4260
                busRetirementBenefitCalculation lobjRetirementCalculation = new busRetirementBenefitCalculation();
                lobjRetirementCalculation.icdoBenefitCalculation = ibusBenefitCalculaton.icdoBenefitCalculation;
                lobjRetirementCalculation.GetRetirementCalculationByDisabilityPayeeAccount(ibusApplication, icdoPayeeAccount.payee_account_id, ibusBenefitCalculaton.icdoBenefitCalculation.plan_id);
                ibusBenefitCalculaton.icdoBenefitCalculation.disability_gross_benefit_amount = lobjRetirementCalculation.icdoBenefitCalculation.disability_gross_benefit_amount;
                //********** 

                if (astrTemplateName == "PAY-4212" ||
                    astrTemplateName == "PAY-4211" ||
                    astrTemplateName == "PAY-4210" ||
                    astrTemplateName == "PAY-4209" ||
                    astrTemplateName == "PAY-4208" ||
                    astrTemplateName == "PAY-4207" ||
                    astrTemplateName == "PAY-4206")
                {
                    LoadMonthlyBenefitAmount(idtNextBenefitPaymentDate);
                    NumberofMonthsFirstPaymentRepresents(idtNextBenefitPaymentDate);
                    FormatPaymentDate(idtNextBenefitPaymentDate);
                    LoadPlanCode();
                    LoadJSPercent();
                    LoadJointAnnuitant();
                    LoadBenefits();
                    SetHPJudgesNBCondition();
                    SetBenefitAcctSubType();
                    SetJobServiceCondition();
                    SetJSRHICCondition();
                    LoadSurvivingSpouseCondition();
                    SetQDROApplies();
                    SetRetirementCondtion();
                    SetReducedBenefit();
                    LoadRTWOrReducedBeneftiOption();
                }
            }
            LoadUnderPaymentCorrespondenceProperties();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            if (iclbMonthlyBenefits == null)
                LoadMontlyBenefits();
            if (iclbPAPITForCorrs == null)
                LoadPAPITForCorrs();
            if (iclbRetroPayment == null)
                LoadRetroPayment();
            if (iclbRetroPayment.Where(o => o.icdoPayeeAccountRetroPayment.retro_payment_type_value == busConstant.RetroPaymentTypePopupBenefits).Count() > 0)
                LoadJointSurvivorCorProperties();
            //prod pir 5571 : need to create a recovery object and calculate amounts
            //--start--//
            if (astrTemplateName == "PAY-4207")
            {
                if (iclbPaymentBenefitOverpaymentHeader == null)
                    LoadBenefitOverPaymentHeader();
                busPaymentBenefitOverpaymentHeader lobjBOH = iclbPaymentBenefitOverpaymentHeader.OrderByDescending(o => o.icdoPaymentBenefitOverpaymentHeader.created_date).FirstOrDefault();
                if (lobjBOH != null)
                {
                    busPaymentRecovery lobjRecovery = new busPaymentRecovery { icdoPaymentRecovery = new cdoPaymentRecovery() };
                    lobjRecovery.icdoPaymentRecovery.benefit_overpayment_id = lobjBOH.icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id;
                    lobjRecovery.icdoPaymentRecovery.payee_account_id = icdoPayeeAccount.payee_account_id;
                    lobjRecovery.LoadTotalRecoveryAmount();
                    lobjRecovery.icdoPaymentRecovery.repayment_type_value = busConstant.RecoveryRePaymentTypeLifeTimeReduction;
                    lobjRecovery.icdoPaymentRecovery.payment_option_value = busConstant.RecoveryPaymentOptionNDPERSPensionChk;
                    lobjRecovery.icdoPaymentRecovery.effective_date = DateTime.Today;
                    lobjRecovery.iclbPayeeAccountRetroPaymentRecoveryDetail = new Collection<busPayeeAccountRetroPaymentRecoveryDetail>();
                    lobjRecovery.iclbPaymentRecoveryHistory = new Collection<busPaymentRecoveryHistory>();
                    if (lobjRecovery.CalculateGrossReductionAmount())
                    {
                        idecGrossReductionAmount = lobjRecovery.icdoPaymentRecovery.gross_reduction_amount;
                        idecRecoveryAmount = lobjRecovery.icdoPaymentRecovery.recovery_amount;
                        idtRecoveryEffectiveDate = lobjRecovery.icdoPaymentRecovery.effective_date;
                        iintNumberOfMonthsToRecover = Convert.ToInt32(idecRecoveryAmount / idecGrossReductionAmount);
                    }
                }
            }
            //--end--//
        }

        //Property  to contain Payment History Item Details for a payee account
        public Collection<busPaymentHistoryDetail> iclbPaymentHistoryDetail { get; set; }

        /// <summary>
        /// Method to load Payment history item details
        /// </summary>
        public void LoadPaymentHistoryDetail()
        {
            if (iclbPaymentHistoryDetail == null)
                iclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            foreach (busPaymentHistoryHeader lobjPaymentHistory in iclbPaymentHistoryHeader)
            {
                busPaymentHistoryDetail lobjPaymentHistoryDetail = new busPaymentHistoryDetail { icdoPaymentHistoryDetail = new cdoPaymentHistoryDetail() };
                if (lobjPaymentHistoryDetail.LoadPaymentHistoryDetailByHistoryID(lobjPaymentHistory.icdoPaymentHistoryHeader.payment_history_header_id))
                {
                    foreach (busPaymentHistoryDetail lobjPHDetail in lobjPaymentHistoryDetail.iclbPaymentHistoryDetail)
                    {
                        lobjPHDetail.LoadPaymentItemType();
                        lobjPHDetail.icdoPaymentHistoryDetail.payment_date = lobjPaymentHistory.icdoPaymentHistoryHeader.payment_date;
                        iclbPaymentHistoryDetail.Add(lobjPHDetail);
                    }
                }
            }
        }
        /// <summary>
        /// Method to load Payment history item details
        /// </summary>
        public void LoadPaymentHistoryDetail(int aintPayeeAccountID)
        {
            if (iclbPaymentHistoryDetail == null)
                iclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader(aintPayeeAccountID);
            foreach (busPaymentHistoryHeader lobjPaymentHistory in iclbPaymentHistoryHeader)
            {
                busPaymentHistoryDetail lobjPaymentHistoryDetail = new busPaymentHistoryDetail { icdoPaymentHistoryDetail = new cdoPaymentHistoryDetail() };
                if (lobjPaymentHistoryDetail.LoadPaymentHistoryDetailByHistoryID(lobjPaymentHistory.icdoPaymentHistoryHeader.payment_history_header_id))
                {
                    foreach (busPaymentHistoryDetail lobjPHDetail in lobjPaymentHistoryDetail.iclbPaymentHistoryDetail)
                    {
                        lobjPHDetail.LoadPaymentItemType();
                        lobjPHDetail.icdoPaymentHistoryDetail.payment_date = lobjPaymentHistory.icdoPaymentHistoryHeader.payment_date;
                        iclbPaymentHistoryDetail.Add(lobjPHDetail);
                    }
                }
            }
        }

        /// <summary>
        /// Method to return latest PAPIT with end date as null
        /// </summary>
        /// <param name="aobjPayeeAccount">payee account</param>
        /// <param name="astrItemCode">item code</param>
        /// <returns></returns>
        public busPayeeAccountPaymentItemType GetLatestPayeeAccountPaymentItemType(string astrItemCode, DateTime? adtEffectiveDate = null)
        {
            if (iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            if (iclbPaymentItemType == null)
                LoadPaymentItemType();
            //Loading Payment Item type bus obj for taxable amount item
            busPaymentItemType lobjPIT = iclbPaymentItemType
                                            .Where(o => o.icdoPaymentItemType.item_type_code == astrItemCode).FirstOrDefault();
            busPayeeAccountPaymentItemType lobjPAPIT = null;
            if (lobjPIT != null)
            {
                if (adtEffectiveDate == null)
                {
                    if (idtNextBenefitPaymentDate == DateTime.MinValue)
                        LoadNexBenefitPaymentDate();
                    //Loading Payee Account Payment with latest Taxable amount for member
                    lobjPAPIT = iclbPayeeAccountPaymentItemType
                                .Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                    lobjPIT.icdoPaymentItemType.payment_item_type_id
                                    && o.icdoPayeeAccountPaymentItemType.end_date_no_null >= idtNextBenefitPaymentDate).FirstOrDefault();
                }
                else
                {
                    //PROD ISSUE: when the enrollment suspended with future date, payment items gets ended with the future date. but, when the next IBS Billing batch run,
                    //it just check the open item only (end date = null) and creates the new item based on that.
                    //Now, we have added the logic to handle the future suspended items too.

                    //Loading Payee Account Payment with latest Taxable amount for member
                    lobjPAPIT = iclbPayeeAccountPaymentItemType
                                .Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                    lobjPIT.icdoPaymentItemType.payment_item_type_id
                                    && o.icdoPayeeAccountPaymentItemType.end_date_no_null >= adtEffectiveDate).FirstOrDefault();
                }
            }
            return lobjPAPIT;
        }

        public busPayeeAccountPaymentItemType GetLatestPayeeAccountPaymentItemTypeMedicare(string astrItemCode, DateTime? adtEffectiveDate = null, int aintPersonAccountID = 0)
        {
            if (iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            if (iclbPaymentItemType == null)
                LoadPaymentItemType();
            //Loading Payment Item type bus obj for taxable amount item
            busPaymentItemType lobjPIT = iclbPaymentItemType
                                            .Where(o => o.icdoPaymentItemType.item_type_code == astrItemCode).FirstOrDefault();
            busPayeeAccountPaymentItemType lobjPAPIT = null;
            if (lobjPIT != null)
            {
                if (adtEffectiveDate == null)
                {
                    if (idtNextBenefitPaymentDate == DateTime.MinValue)
                        LoadNexBenefitPaymentDate();
                    //Loading Payee Account Payment with latest Taxable amount for member
                    lobjPAPIT = iclbPayeeAccountPaymentItemType
                                .Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                    lobjPIT.icdoPaymentItemType.payment_item_type_id
                                    && o.icdoPayeeAccountPaymentItemType.end_date_no_null >= idtNextBenefitPaymentDate
                                    && o.icdoPayeeAccountPaymentItemType.person_account_id == aintPersonAccountID).FirstOrDefault();
                }
                else
                {
                    //PROD ISSUE: when the enrollment suspended with future date, payment items gets ended with the future date. but, when the next IBS Billing batch run,
                    //it just check the open item only (end date = null) and creates the new item based on that.
                    //Now, we have added the logic to handle the future suspended items too.

                    //Loading Payee Account Payment with latest Taxable amount for member
                    lobjPAPIT = iclbPayeeAccountPaymentItemType
                                .Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                    lobjPIT.icdoPaymentItemType.payment_item_type_id
                                    && o.icdoPayeeAccountPaymentItemType.end_date_no_null >= adtEffectiveDate
                                    && o.icdoPayeeAccountPaymentItemType.person_account_id == aintPersonAccountID).FirstOrDefault();
                }
            }
            return lobjPAPIT;
        }

        /// <summary>
        /// Method to return latest PAPIT with end date as null
        /// </summary>
        /// <param name="aobjPayeeAccount">payee account</param>
        /// <param name="astrItemCode">item code</param>
        /// <returns></returns>
        public busPayeeAccountPaymentItemType GetLatestPayeeAccountPaymentItemTypeByVendorOrgID(string astrItemCode, int aintVendorOrgId)
        {
            if (iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            if (iclbPaymentItemType == null)
                LoadPaymentItemType();
            //Loading Payment Item type bus obj for taxable amount item
            busPaymentItemType lobjPIT = iclbPaymentItemType
                                            .Where(o => o.icdoPaymentItemType.item_type_code == astrItemCode).FirstOrDefault();
            busPayeeAccountPaymentItemType lobjPAPIT = null;
            if (lobjPIT != null)
            {
                //Loading Payee Account Payment with latest Taxable amount for member
                lobjPAPIT = iclbPayeeAccountPaymentItemType
                            .Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id ==
                                lobjPIT.icdoPaymentItemType.payment_item_type_id
                                && o.icdoPayeeAccountPaymentItemType.end_date == DateTime.MinValue
                                && o.icdoPayeeAccountPaymentItemType.vendor_org_id == aintVendorOrgId).FirstOrDefault();
            }
            return lobjPAPIT;
        }

        /// <summary>
        /// Function to get the latest non ended Amout in Payee account payment item
        /// </summary>
        /// <param name="aobjPayeeAccount">Payee Account</param>
        /// <param name="astrItemCode">Item Code</param>
        /// <returns>Amount</returns>
        public decimal LoadLatestPAPITAmount(string astrItemCode)
        {
            decimal ldecAmount = 0.0M;
            busPayeeAccountPaymentItemType lobjPAPIT = GetLatestPayeeAccountPaymentItemType(astrItemCode);
            if (lobjPAPIT != null)
                ldecAmount = lobjPAPIT.icdoPayeeAccountPaymentItemType.amount;
            return ldecAmount;
        }

        public void LoadRecertificationDate()
        {
            if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                if (ibusPayeeAccountActiveStatus == null)
                    LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                if ((ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value != busConstant.PayeeAccountStatusDisabilityCancelled) &&
                    (ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value != busConstant.PayeeAccountStatusDisabilityPaymentCompleted))
                {
                    if ((icdoPayeeAccount.recertification_date == DateTime.MinValue) &&
                        (string.IsNullOrEmpty(icdoPayeeAccount.is_recertification_date_set_flag)))
                    {
                        // UAT PIR ID 1362
                        if (idtNextBenefitPaymentDate == DateTime.MinValue)
                            LoadNexBenefitPaymentDate();
                        DateTime ldteRecertificationDate = idtNextBenefitPaymentDate.AddMonths(18);
                        icdoPayeeAccount.recertification_date = new DateTime(ldteRecertificationDate.Year, ldteRecertificationDate.Month, 01);
                        icdoPayeeAccount.case_recertification_date = icdoPayeeAccount.recertification_date;
                        icdoPayeeAccount.is_recertification_date_set_flag = busConstant.Flag_Yes;
                        if (icdoPayeeAccount.benefit_begin_date < busConstant.Pre1991Disability)
                            icdoPayeeAccount.is_pre_1991_disability_flag = busConstant.Flag_Yes;
                        else
                            icdoPayeeAccount.is_pre_1991_disability_flag = busConstant.Flag_No;
                    }
                }
            }
        }

        // ** BR-082-24 ** Recertification Date should be first of the month.
        public bool IsRecertificationDayNotFirstofMonth()
        {
            if (icdoPayeeAccount.recertification_date != DateTime.MinValue)
                if (icdoPayeeAccount.recertification_date.Day != 1)
                    return true;
            return false;
        }

        public bool IsValidRecertificationDate()
        {
            if (icdoPayeeAccount.recertification_date != DateTime.MinValue)
            {
                if (iclbCase.IsNull())
                    LoadCase();
                foreach (busCase lobjCase in iclbCase)
                {
                    if ((lobjCase.icdoCase.case_status_value != busConstant.CaseStatusValueApproved) &&
                        (lobjCase.icdoCase.case_status_value != busConstant.CaseStatusValueCancelled))
                    {
                        if (icdoPayeeAccount.recertification_date != icdoPayeeAccount.case_recertification_date)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// this method is used to check the validation on
        /// clicking new button in the rollover tab
        /// </summary>
        /// <param name="ahstParam"></param>
        /// <returns></returns>
        public override ArrayList ValidateNew(Hashtable ahstParam)
        {
            ArrayList larrErrors = new ArrayList();
            utlError lobjError = null;
            if (((string)ahstParam["ablnisvalidatenewtrue"]) == "true")
            {
                busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                lobjPayeeAccount.FindPayeeAccount(Convert.ToInt32(ahstParam["aintpayeeaccountid"]));
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus == null)
                    lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                if (lobjPayeeAccount.iclbRolloverDetail == null)
                    lobjPayeeAccount.LoadRolloverDetail();
                string lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                if (lstrStatus == busConstant.PayeeAccountStatusReceiving ||
                       lstrStatus == busConstant.PayeeAccountStatusPaymentComplete ||
                       lstrStatus == busConstant.PayeeAccountStatusRefundProcessed)
                {
                    if (lobjPayeeAccount.iclbRolloverDetail.Count > 0)
                    {
                        // if (lobjPayeeAccount.iclbRolloverDetail.Count < 2 &&
                        //lobjPayeeAccount.IsReIssueToRolloverAllowedForMember())
                        //      {
                        //}
                        //       else if (lobjPayeeAccount.iclbRolloverDetail.Where(o => o.icdoPayeeAccountRolloverDetail.status_value
                        //          == busConstant.PayeeAccountRolloverDetailStatusCancelled).Count() == 0)
                        //{
                        //lobjError = AddError(6453, "");
                        //           larrErrors.Add(lobjError);
                        //           return larrErrors;
                        //       }
                        if (lobjPayeeAccount.iclbRolloverDetail.Where(o => o.icdoPayeeAccountRolloverDetail.status_value
                            == busConstant.PayeeAccountRolloverDetailStatusCancelled).Count() == 0)
                        {
                            lobjError = AddError(6453, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                        else if (!lobjPayeeAccount.IsReIssueToRolloverAllowedForOrg())
                        {
                            lobjError = AddError(6458, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                        else if (lobjPayeeAccount.IsReIssueToRolloverAllowedForOrg() &&
                            lobjPayeeAccount.iclbRolloverDetail.Where(o => o.icdoPayeeAccountRolloverDetail.status_value
                            == busConstant.PayeeAccountRolloverDetailStatusCancelled).Count() == 0)
                        {
                            lobjError = AddError(6453, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                    }
                    else if (lobjPayeeAccount.icdoPayeeAccount.benefit_option_value != busConstant.BenefitOption5YearTermLife)
                    {
                        if (!lobjPayeeAccount.IsReIssueToRolloverAllowedForMember())
                        {
                            lobjError = AddError(6458, "");
                            larrErrors.Add(lobjError);
                            return larrErrors;
                        }
                    }
                }
            }
            return larrErrors;
        }
        //Check Whether payee account is processed
        public bool IsPayeeAcountProcessed()
        {
            if (ibusPayeeAccountActiveStatus == null)
                LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            string lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
            if (lstrStatus == busConstant.PayeeAccountStatusPaymentComplete || lstrStatus == busConstant.PayeeAccountStatusRefundProcessed)
            {
                return true;
            }
            return false;
        }
        //Property to load cancelled rollover details
        public Collection<busPayeeAccountRolloverDetail> iclbCancelledRolloverDetail { get; set; }

        //Load Cancelled Rollover Details
        public void LoadCancelledRolloverDetails()
        {
            if (iclbRolloverDetail == null)
                LoadRolloverDetail();
            iclbCancelledRolloverDetail = new Collection<busPayeeAccountRolloverDetail>();
            foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in iclbRolloverDetail.Where(o =>
                o.icdoPayeeAccountRolloverDetail.status_value == busConstant.PayeeAccountRolloverDetailStatusCancelled))
                iclbCancelledRolloverDetail.Add(lobjRolloverDetail);
        }
        //BR-78-12a The system must allow user to add a ‘Rollover Instruction’ only if there exist a ‘Payment History Distribution’ in ‘Void Pending’ 
        //or ‘Stop Pay Pending’ status.
        public bool IsReIssueToRolloverAllowedForMember()
        {
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            foreach (busPaymentHistoryHeader lobjPaymentHeader in iclbPaymentHistoryHeader)
            {
                if (lobjPaymentHeader.iclbPaymentHistoryDistribution == null)
                    lobjPaymentHeader.LoadPaymentHistoryDistribution();
                if (lobjPaymentHeader.iclbPaymentHistoryDistribution.Count > 0)
                {
                    if (lobjPaymentHeader.iclbPaymentHistoryDistribution.Where(o =>
                        o.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.ACHVoidPending ||
                         o.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.CHKVoidPending ||
                         o.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.CHKStopPayPending).Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //The Rollover check should be in Void Pending or Stop Pay Pending Status for creating new Rollover
        public bool IsReIssueToRolloverAllowedForOrg()
        {
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            foreach (busPaymentHistoryHeader lobjPaymentHeader in iclbPaymentHistoryHeader)
            {
                if (lobjPaymentHeader.iclbPaymentHistoryDistribution == null)
                    lobjPaymentHeader.LoadPaymentHistoryDistribution();
                if (lobjPaymentHeader.iclbPaymentHistoryDistribution.Count > 0)
                {
                    if (lobjPaymentHeader.iclbPaymentHistoryDistribution.Where(o =>
                          o.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.RCHKVoidPending ||
                          o.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.RACHVoidPending ||
                          o.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.RCHKStopPayPending).Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //The Rollover check should be in Void Pending or Stop Pay Pending Status for creating new Rollover
        public bool IsRolloverCheckReissueApproved()
        {
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            foreach (busPaymentHistoryHeader lobjPaymentHeader in iclbPaymentHistoryHeader)
            {
                if (lobjPaymentHeader.iclbPaymentHistoryDistribution == null)
                    lobjPaymentHeader.LoadPaymentHistoryDistribution();
                if (lobjPaymentHeader.iclbPaymentHistoryDistribution.Count > 0)
                {
                    if (lobjPaymentHeader.iclbPaymentHistoryDistribution.Where(o =>
                          o.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.RCHKReissueApproved ||
                          o.icdoPaymentHistoryDistribution.distribution_status_value == busConstant.RACHReissueApproved).Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool IsBenefitAccountTypeIsRetirement()
        {
            return icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement;
        }

        public bool IsBenefitAccountTypeIsDisability()
        {
            return icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability;
        }

        public bool IsBenefitAccountTypePreRetirementDeath()
        {
            return icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath;
        }

        public bool IsBenefitAccountTypePostRetirementDeath()
        {
            return icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath;
        }

        public bool IsBenefitAccountTypeRefund()
        {
            return icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund;
        }

        public bool IsBenefitAccountTypeIsRetirmentOrDisability()
        {
            if (IsBenefitAccountTypeIsRetirement() || IsBenefitAccountTypeIsDisability())
                return true;
            return false;
        }
        public bool IsBenefitAccountTypeIsRetirmentOrDisabilityOrRefund()
        {
            if (IsBenefitAccountTypeIsRetirement() || IsBenefitAccountTypeIsDisability() || IsBenefitAccountTypeRefund())
                return true;
            return false;
        }


        #region UCS - 079
        //UCS - 079 : Property to contain BenefitOverpaymentheaders
        public Collection<busPaymentBenefitOverpaymentHeader> iclbPaymentBenefitOverpaymentHeader { get; set; }

        /// <summary>
        /// method to load Benefit Over Payment Header
        /// </summary>
        public void LoadBenefitOverPaymentHeader()
        {
            DataTable ldtBenefitOverPaymentHeader = Select<cdoPaymentBenefitOverpaymentHeader>
                (new string[1] { enmPaymentBenefitOverpaymentHeader.payee_account_id.ToString() },
                new object[1] { icdoPayeeAccount.payee_account_id }, null, null);
            iclbPaymentBenefitOverpaymentHeader = GetCollection<busPaymentBenefitOverpaymentHeader>
                (ldtBenefitOverPaymentHeader, "icdoPaymentBenefitOverpaymentHeader");
            foreach (busPaymentBenefitOverpaymentHeader lobjHeader in iclbPaymentBenefitOverpaymentHeader)
                lobjHeader.LoadTotalAmountDue();
        }

        /// <summary>
        /// Method to get paid history headers for a payee account
        /// </summary>
        /// <param name="adtStartDate">Start Date</param>
        /// <param name="adtEndDate">End Date</param>
        /// <param name="aintPaymentHistoryHeaderID">Payment history header id</param>
        /// <param name="aintPayeeAccountID">Payee account id</param>
        /// <returns>Paid history headers</returns>
        public IEnumerable<busPaymentHistoryHeader> GetPaidHistoryHeaders(DateTime adtStartDate, DateTime adtEndDate, int aintPaymentHistoryHeaderID, int aintPayeeAccountID)
        {
            if (iblnIsERToDisabiliyConverion)
            {
                if (iclbPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader(aintPayeeAccountID);
            }
            else
            {
                if (iclbPaymentHistoryHeader == null)
                    LoadPaymentHistoryHeader();
            }

            IEnumerable<busPaymentHistoryHeader> lenmPaymentHistory;
            if (aintPaymentHistoryHeaderID != 0)
            {
                lenmPaymentHistory = iclbPaymentHistoryHeader
                .Where(o => o.icdoPaymentHistoryHeader.payment_history_header_id == aintPaymentHistoryHeaderID);
            }
            else
            {
                lenmPaymentHistory = iclbPaymentHistoryHeader
                .Where(o => (o.icdoPaymentHistoryHeader.status_value == busConstant.PaymentStatusOutstanding ||
                            o.icdoPaymentHistoryHeader.status_value == busConstant.PaymentStatusProcessed) &&
                            o.icdoPaymentHistoryHeader.payment_date >= adtStartDate &&
                            o.icdoPaymentHistoryHeader.payment_date <= adtEndDate);
            }
            return lenmPaymentHistory;
        }

        /// <summary>
        /// Method to get total amount paid for a month from Payment history detail table
        /// </summary>
        /// <param name="adtStartDate">Start Date</param>
        /// <param name="adtEndDate">End Date</param>
        /// <returns>Amount Paid</returns>
        public decimal GetTotalAmountPaidForTheMonth(DateTime adtStartDate, DateTime adtEndDate, int aintPaymentHistoryHeaderID, int aintPayeeAccountID)
        {
            decimal ldecAmountPaid = 0.0M;
            //getting paid history headers
            IEnumerable<busPaymentHistoryHeader> lenmPaymentHistory = GetPaidHistoryHeaders(adtStartDate, adtEndDate, aintPaymentHistoryHeaderID, aintPayeeAccountID);
            //Amount from Payment History Detail
            foreach (busPaymentHistoryHeader lobjPaymentHeader in lenmPaymentHistory)
                ldecAmountPaid += lobjPaymentHeader.GetAmountPaid();
            //Amount from Payee account Retro item monthwise
            if (iclbPaymentMonthwiseAdjustmentDetail == null)
                LoadRetroPaymentMonthwiseDetail();
            decimal ldecRetroAmount = iclbPaymentMonthwiseAdjustmentDetail
                .Where(o => o.icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date >= adtStartDate &&
                            o.icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date <= adtEndDate)
                .Sum(o => o.icdoPayeeAccountMonthwiseAdjustmentDetail.amount + o.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount);
            return ldecAmountPaid + ldecRetroAmount;
        }
        /// <summary>
        /// Method to get  paid history for a month from Payment history detail table
        /// </summary>
        /// <param name="adtStartDate">Start Date</param>
        /// <param name="adtEndDate">End Date</param>
        /// <returns>Amount Paid</returns>
        public Collection<busPaymentHistoryDetail> GetTotalPaidHistoryForTheMonth(DateTime adtStartDate, DateTime adtEndDate,
            int aintPaymentHistoryHeaderID, int aintPayeeAccountID)
        {
            Collection<busPaymentHistoryDetail> lclbPaymentHistoryDetail = new Collection<busPaymentHistoryDetail>();
            //getting paid history headers
            IEnumerable<busPaymentHistoryHeader> lenmPaymentHistory = GetPaidHistoryHeaders(adtStartDate, adtEndDate, aintPaymentHistoryHeaderID, aintPayeeAccountID);
            foreach (busPaymentHistoryHeader lobjPaymentHeader in lenmPaymentHistory)
            {
                if (lobjPaymentHeader.iclbPaymentHistoryDetail == null)
                    lobjPaymentHeader.LoadPaymentHistoryDetailsUnsigned();
                foreach (busPaymentHistoryDetail lobjDetail in lobjPaymentHeader.iclbPaymentHistoryDetail)
                    lclbPaymentHistoryDetail.Add(lobjDetail);
            }

            return lclbPaymentHistoryDetail;
        }

        //Property to contain all retro items for one payment history header
        public Collection<busPayeeAccountRetroPayment> iclbPaidRetroPayments { get; set; }
        /// <summary>
        /// Method to load Retropayment items
        /// </summary>
        public void LoadPaidRetroPayments()
        {
            iclbPaidRetroPayments = new Collection<busPayeeAccountRetroPayment>();
            DataTable ldtRetroItems = Select("cdoPayeeAccountRetroPayment.GetRetroPaymentsForPayeeAccount",
                new object[1] { icdoPayeeAccount.payee_account_id });

            iclbPaidRetroPayments = GetCollection<busPayeeAccountRetroPayment>(ldtRetroItems, "icdoPayeeAccountRetroPayment");
            foreach (busPayeeAccountRetroPayment lobjRetroPayment in iclbPaidRetroPayments)
                lobjRetroPayment.LoadPayeeAccountRetroPaymentDetail();
        }
        //Property to contain Retro Payment Monthwise details
        public Collection<busPayeeAccountMonthwiseAdjustmentDetail> iclbPaymentMonthwiseAdjustmentDetail { get; set; }
        /// <summary>
        /// Method to load Retro Payment Monthwise details
        /// </summary>
        public void LoadRetroPaymentMonthwiseDetail()
        {
            DataTable ldtRetroMonthwise = new DataTable();
            if (iclbPaidRetroPayments == null)
                LoadPaidRetroPayments();
            //loop thru each retro payment header
            foreach (busPayeeAccountRetroPayment lobjRetroPayment in iclbPaidRetroPayments)
            {
                //monthwise details for each retro payment detail is added to one single table
                if (ldtRetroMonthwise.Rows.Count <= 0)
                {
                    ldtRetroMonthwise = Select<cdoPayeeAccountMonthwiseAdjustmentDetail>
                        (new string[1] { enmPayeeAccountMonthwiseAdjustmentDetail.payee_account_retro_payment_id.ToString() },
                        new object[1] { lobjRetroPayment.icdoPayeeAccountRetroPayment.payee_account_retro_payment_id },
                        null, null);
                }
                else
                {
                    DataTable ldtMonthwise = Select<cdoPayeeAccountMonthwiseAdjustmentDetail>
                        (new string[1] { enmPayeeAccountMonthwiseAdjustmentDetail.payee_account_retro_payment_id.ToString() },
                        new object[1] { lobjRetroPayment.icdoPayeeAccountRetroPayment.payee_account_retro_payment_id },
                        null, null);
                    ldtRetroMonthwise.Merge(ldtMonthwise);
                }
            }
            //loding the collection with list of all monthwise details for a payee account
            iclbPaymentMonthwiseAdjustmentDetail = GetCollection<busPayeeAccountMonthwiseAdjustmentDetail>
                (ldtRetroMonthwise, "icdoPayeeAccountMonthwiseAdjustmentDetail");
        }

        //Property to contain all benefit applications for a person
        public Collection<busBenefitApplication> iclbBenefitApplication { get; set; }
        /// <summary>
        /// Method to load all benefit applications for a person which are not in cancelled or deferred status
        /// </summary>
        /// <param name="aintPersonID">Person ID</param>
        public void LoadDisabilityBenefitApplicationsByPersonID(int aintPersonID)
        {
            DataTable ldtBenefitApplications = SelectWithOperator<cdoBenefitApplication>
                (new string[4] { enmBenefitApplication.member_person_id.ToString(), enmBenefitApplication.action_status_value.ToString(),
                                enmBenefitApplication.action_status_value.ToString(), enmBenefitApplication.benefit_account_type_value.ToString() },
                new string[4] { "=", "<>", "<>", "=" },
                new object[4] { aintPersonID,busConstant.ApplicationActionStatusCancelled,busConstant.ApplicationActionStatusDeferred,
                                busConstant.ApplicationBenefitTypeDisability}, null);
            iclbBenefitApplication = new Collection<busBenefitApplication>();
            iclbBenefitApplication = GetCollection<busBenefitApplication>(ldtBenefitApplications, "icdoBenefitApplication");
        }
        //prop to identifiy whether this payee account is converted from Early retirement to disability
        public bool iblnIsERToDisabiliyConverion { get; set; }

        public Collection<busPayeeAccountMonthwiseAdjustmentDetail> iclbMonthwiseAdjustmentDetail { get; set; }
        /// <summary>
        ///  Method to check and create any overpayment or underpayment based on Total Amount Due
        /// </summary>
        /// <param name="astrAdjustmentReason">Adjustment reason for benefit overpayment</param>
        /// <returns>int value</returns>
        public int CreateBenefitOverPaymentorUnderPayment(string astrAdjustmentReason, int aintBenefitDROCalculationID = 0)
        {
            int lintERPayeeAccountID = 0;
            decimal ldecAmountDue = 0.0M, ldecCOLAandAdhoc, ldecBenefitAmountFTM = 0.0M, ldecAmountPaidFTM, ldecTotalAmountDue = 0.0M, ldecSupplementalCheckAmount = 0.0M;
            DateTime ldtStartDate = DateTime.MinValue, ldtEndDate = DateTime.MinValue, ldtPayeeAccountBenefitBeginDate = DateTime.MinValue;
            int lintOverpaymentorUnderpaymentID = 0;
            Collection<busPayeeAccountMonthwiseAdjustmentDetail> lclbMonthwiseAdjustmentDetail = new Collection<busPayeeAccountMonthwiseAdjustmentDetail>();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            if (ibusBenefitCalculaton == null)
                LoadBenefitCalculation();

            if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability)
            {
                if (ibusPayee.IsNull()) LoadPayee();
                GetEarlyRetirementPayeeAccountId(ref lintERPayeeAccountID, ref ldtStartDate);
                if (ldtStartDate != DateTime.MinValue)
                {
                    ldtPayeeAccountBenefitBeginDate = ldtStartDate;
                }
                else
                {
                    ldtStartDate = icdoPayeeAccount.benefit_begin_date;
                    ldtPayeeAccountBenefitBeginDate = ldtStartDate;
                }
                ldtEndDate = ldtStartDate.GetFirstDayofNextMonth().AddDays(-1);
            }
            else
            {
                ldtStartDate = icdoPayeeAccount.benefit_begin_date;
                ldtPayeeAccountBenefitBeginDate = ldtStartDate;
                ldtEndDate = icdoPayeeAccount.benefit_begin_date.GetFirstDayofNextMonth().AddDays(-1);
            }
            //Loading gross benefit amt for a date
            iclbPayeeAccountPaymentItemType = null;
            LoadGrossBenefitAmount();
            ldecBenefitAmountFTM = idecGrossBenfitAmount;
            busPayeeAccountPaymentItemType lobjPAPIT = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
            if ((ldtStartDate != DateTime.MinValue) && (ldtEndDate != DateTime.MinValue))
            {
                while (ldtStartDate < idtNextBenefitPaymentDate)
                {
                    ldecBenefitAmountFTM -= ldecSupplementalCheckAmount;
                    busPayeeAccountMonthwiseAdjustmentDetail lobjMonthwiseAdjustment = new busPayeeAccountMonthwiseAdjustmentDetail { icdoPayeeAccountMonthwiseAdjustmentDetail = new cdoPayeeAccountMonthwiseAdjustmentDetail() };
                    ldecAmountPaidFTM = ldecCOLAandAdhoc = ldecAmountDue = ldecSupplementalCheckAmount = 0.0M;

                    //TODO need to get COLA / Adhoc amounts
                    ldecCOLAandAdhoc = GetCOLAOrAdhocAmount(ldtStartDate, ldtEndDate, lobjPAPIT);
                    if (iblnIsERToDisabiliyConverion)
                    {
                        //loading the amont paid in that month
                        ldecAmountPaidFTM = GetTotalAmountPaidForTheMonth(ldtStartDate, ldtEndDate, 0, lintERPayeeAccountID);
                    }
                    else
                    {
                        //loading the amont paid in that month
                        ldecAmountPaidFTM = GetTotalAmountPaidForTheMonth(ldtStartDate, ldtEndDate, 0, 0);
                    }
                    //total amount due for the month
                    ldecBenefitAmountFTM += ldecCOLAandAdhoc;
                    //method to get supplemental check amount and add to benefit amount for the month
                    ldecSupplementalCheckAmount = GetSupplementalCheckAmount(ldtStartDate, ldtEndDate, lobjPAPIT);
                    ldecBenefitAmountFTM += ldecSupplementalCheckAmount;
                    //finding any difference in amount paid                
                    ldecAmountDue = ldecBenefitAmountFTM - ldecAmountPaidFTM;
                    //Adding to monthwise adjustment collection
                    lobjMonthwiseAdjustment.icdoPayeeAccountMonthwiseAdjustmentDetail.amount = ldecAmountDue;
                    lobjMonthwiseAdjustment.icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date = ldtStartDate;
                    lclbMonthwiseAdjustmentDetail.Add(lobjMonthwiseAdjustment);
                    //adding to total net amount
                    ldecTotalAmountDue += ldecAmountDue;
                    //moving to next month start date / end date
                    ldtStartDate = ldtEndDate.AddDays(1);
                    ldtEndDate = ldtStartDate.GetFirstDayofNextMonth().AddDays(-1);
                }
                //check to create overpayment
                if (ldecTotalAmountDue < 0 && ((ibusBenefitCalculaton.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments || ibusBenefitCalculaton.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment
                    ) || iblnIsERToDisabiliyConverion))
                {
                    lintOverpaymentorUnderpaymentID = CreateBenefitOverpayment(lintERPayeeAccountID, astrAdjustmentReason, lclbMonthwiseAdjustmentDetail);
                }
                else if (ldecTotalAmountDue > 0)
                {
                    //PIR 13577
                    CreateBenefitUnderPayment(ldtPayeeAccountBenefitBeginDate, ldtEndDate, aintBenefitDROCalculationID);
                }
            }
            return lintOverpaymentorUnderpaymentID;
        }

        private decimal GetCOLAOrAdhocAmount(DateTime adtStartDate, DateTime adtEndDate, busPayeeAccountPaymentItemType abusPAPIT)
        {
            DataTable ldtbRequests = Select("cdoPayeeAccount.LoadPostRetirmentIncreaseRequests", new object[2] { adtStartDate, adtEndDate });
            decimal ldecRefAmount = 0.0m;
            if (ldtbRequests.Rows.Count > 0)
            {
                busPostRetirementIncreaseBatchRequest lobjPostRetirementIncreaseBatchRequest = new busPostRetirementIncreaseBatchRequest
                {
                    icdoPostRetirementIncreaseBatchRequest = new cdoPostRetirementIncreaseBatchRequest()
                };
                lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.LoadData(ldtbRequests.Rows[0]);
                GetCOLAOrAdhocAmount(lobjPostRetirementIncreaseBatchRequest.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id, ref ldecRefAmount, 0, abusPAPIT);
            }
            return ldecRefAmount;
        }

        public decimal GetSupplementalCheckAmount(DateTime adtStartDate, DateTime adtEndDate, busPayeeAccountPaymentItemType abusPAPIT)
        {
            decimal ldecSupplementalAmount = 0.0M;

            DataTable ldtPAPIT = Select("cdoPostRetirementIncreaseBatchRequest.GetSupplementalCheckAmount",
                                        new object[3] { adtStartDate, adtEndDate, icdoPayeeAccount.payee_account_id });
            if (ldtPAPIT.Rows.Count > 0)
            {
                abusPAPIT.icdoPayeeAccountPaymentItemType.LoadData(ldtPAPIT.Rows[0]);
                ldecSupplementalAmount = abusPAPIT.icdoPayeeAccountPaymentItemType.amount;
            }
            return ldecSupplementalAmount;
        }

        /// <summary>
        /// Method to create benfit overpayment
        /// </summary>
        /// <param name="aintERPayeeAccountID">Early retr. payee account</param>
        /// <param name="astrAdjustmentReason">adjustment reason</param>
        /// <param name="aclbMonthwiseAdjustmentDetail">monthwise adjustment details</param>
        /// <returns>created benefit overpayment id</returns>
        public int CreateBenefitOverpayment(int aintERPayeeAccountID, string astrAdjustmentReason, Collection<busPayeeAccountMonthwiseAdjustmentDetail> aclbMonthwiseAdjustmentDetail)
        {
            busPaymentBenefitOverpaymentHeader lobjOverPaymentHeader = new busPaymentBenefitOverpaymentHeader { icdoPaymentBenefitOverpaymentHeader = new cdoPaymentBenefitOverpaymentHeader() };

            //PIR - 1958 : need to create benefit overpayment on ER payee account if ER to Disability conversion
            if (!iblnIsERToDisabiliyConverion)
                aintERPayeeAccountID = icdoPayeeAccount.payee_account_id;
            //Recalcualte taxes
            CalculateAdjustmentTax(false);
            //adding overpayment header record
            lobjOverPaymentHeader.icdoPaymentBenefitOverpaymentHeader.payee_account_id = aintERPayeeAccountID;
            lobjOverPaymentHeader.icdoPaymentBenefitOverpaymentHeader.adjustment_reason_value = astrAdjustmentReason;
            lobjOverPaymentHeader.icdoPaymentBenefitOverpaymentHeader.Insert();
            lobjOverPaymentHeader.icdoPaymentBenefitOverpaymentHeader.Select();

            //adding monthwise adjustment details
            foreach (busPayeeAccountMonthwiseAdjustmentDetail lobjDetail in aclbMonthwiseAdjustmentDetail)
            {
                lobjOverPaymentHeader.CreateMonthwiseAdjustmentDetail(lobjDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.effective_date,
                    lobjDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.amount * -1,
                    lobjDetail.icdoPayeeAccountMonthwiseAdjustmentDetail.interest_amount, aintERPayeeAccountID);
            }
            return lobjOverPaymentHeader.icdoPaymentBenefitOverpaymentHeader.benefit_overpayment_id;
        }

        /// <summary>
        /// Method to create Benefit under payment
        /// </summary>
        /// <param name="adtStartDate">Retro payment effective start date</param>
        /// <param name="adtEndDate">Retro payment effective end date</param>
        public void CreateBenefitUnderPayment(DateTime adtStartDate, DateTime adtEndDate, int aintBenefitDROCalculationID = 0)
        {
            busPayeeAccountRetroPayment lobjUnderPaymentHeader = new busPayeeAccountRetroPayment { icdoPayeeAccountRetroPayment = new cdoPayeeAccountRetroPayment() };
            lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.payee_account_id = icdoPayeeAccount.payee_account_id;

            //PIR 13577
            if (ibusBenefitCalculaton == null)
            {
                ibusBenefitCalculaton = new busBenefitCalculation();
                ibusBenefitCalculaton.icdoBenefitCalculation = new cdoBenefitCalculation();
            }

            if ((ibusBenefitCalculaton.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                ibusBenefitCalculaton.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequent)
                && iblnIsERToDisabiliyConverion)
            {
                lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.retro_payment_type_value = busConstant.RetroPaymentTypeBenefitUnderpayment;
                lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.adjustment_reason_value = busConstant.AdjustmentReasonRecalculation;
            }
            else if ((ibusBenefitCalculaton.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeFinal ||
                ibusBenefitCalculaton.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequent) || aintBenefitDROCalculationID > 0)  //PIR 13577
            {
                lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.retro_payment_type_value = busConstant.RetroPaymentTypeInitial;
                lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.adjustment_reason_value = busConstant.AdjustmentReasonFirstCheckRetro;
                //Backlog PIR 10966
                lobjUnderPaymentHeader.iblnIsInitialRetroCreation = true;
            }
            else if (ibusBenefitCalculaton.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeAdjustments ||
                     ibusBenefitCalculaton.icdoBenefitCalculation.calculation_type_value == busConstant.CalculationTypeSubsequentAdjustment)
            {
                lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.retro_payment_type_value = busConstant.RetroPaymentTypeBenefitUnderpayment;
                lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.adjustment_reason_value = busConstant.AdjustmentReasonRecalculation;
            }
            lobjUnderPaymentHeader.IsNewMode = true;
            lobjUnderPaymentHeader.iarrChangeLog.Add(lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment);
            lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.ienuObjectState = ObjectState.Insert;
            CreateBenefitUnderpaymentAdjustmentsHeader(lobjUnderPaymentHeader.icdoPayeeAccountRetroPayment.retro_payment_type_value,
                lobjUnderPaymentHeader, adtStartDate, adtEndDate);
            lobjUnderPaymentHeader.BeforePersistChanges();
            lobjUnderPaymentHeader.PersistChanges();
            lobjUnderPaymentHeader.AfterPersistChanges();
        }

        public void GetEarlyRetirementPayeeAccountId(ref int lintERPayeeAccountID, ref DateTime ldtStartDate)
        {
            if (ibusPayee == null)
                LoadPayee();
            if (ibusPayee.iclbPayeeAccount == null)
                ibusPayee.LoadPayeeAccount();
            foreach (busPayeeAccount lobjPayeeAccount in ibusPayee.iclbPayeeAccount)
            {
                if (lobjPayeeAccount.ibusPayeeAccountActiveStatus == null)
                    lobjPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                if (lobjPayeeAccount.icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly &&
                    lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                {
                    ldtStartDate = lobjPayeeAccount.icdoPayeeAccount.benefit_begin_date;
                    lintERPayeeAccountID = lobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                    iblnIsERToDisabiliyConverion = true;
                }
            }
        }

        ////Create Benefit Underpayment Adjustments Header 
        public void CreateBenefitUnderpaymentAdjustmentsHeader(string astrRetroPaymentType,
            busPayeeAccountRetroPayment abusPayeeAccountRetroPayment, DateTime adtStartDate, DateTime adtEndDate)
        {
            abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.payee_account_id = icdoPayeeAccount.payee_account_id;
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            //For initial and benefit under payment adjustments ,we set the start date as retirement date or withdrwan date
            //and end date as last payment date
            if (iblnIsERToDisabiliyConverion)
            {
                abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_start_date = adtStartDate;
                abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_end_date = adtEndDate;
            }
            else if (astrRetroPaymentType == busConstant.RetroPaymentTypeBenefitUnderpayment)
            {
                if (ibusApplication == null)
                    LoadApplication();
                if (ibusApplication.icdoBenefitApplication.benefit_application_id == 0)
                {
                    if (ibusDROApplication == null)
                        LoadDROApplication();
                    abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_start_date = ibusDROApplication.icdoBenefitDroApplication.benefit_receipt_date;
                }
                else if (ibusApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
                {
                    abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_start_date = ibusApplication.icdoBenefitApplication.termination_date.AddMonths(1);
                }
                //UAT PIR:2076. effective date set as last of the month following the date of death
                //As set in Pre Retirement Death.
                else if (ibusApplication.icdoBenefitApplication.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath)
                {
                    if (ibusApplication.ibusPerson.IsNull())
                        ibusApplication.LoadPerson();
                    if (ibusApplication.ibusPerson.icdoPerson.date_of_death != DateTime.MinValue)
                    {
                        DateTime ldteTempDate = ibusApplication.ibusPerson.icdoPerson.date_of_death.AddMonths(1);
                        DateTime ldtRetirementDate = new DateTime(ldteTempDate.Year, ldteTempDate.Month, 01);
                        abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_start_date = ldtRetirementDate;
                    }
                }
                else
                {
                    abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_start_date = ibusApplication.icdoBenefitApplication.retirement_date;
                }
                abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_end_date = idtNextBenefitPaymentDate.AddDays(-1);
            }
            //for initial retro item, effective start date should be benefit begin date
            else if (astrRetroPaymentType == busConstant.RetroPaymentTypeInitial)
            {
                abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_start_date = icdoPayeeAccount.benefit_begin_date;
                abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_end_date = idtNextBenefitPaymentDate.AddDays(-1);
            }
            //For reacitivation of payee account ,we set the start date as retirement date or withdrwan date
            //and end date as last payment date
            else if (astrRetroPaymentType == busConstant.RetroPaymentTypeReactivation)
            {
                if (ibusPayeeAccountActiveStatus == null)
                    LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_start_date = ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_effective_date;
                abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.effective_end_date = idtNextBenefitPaymentDate.AddDays(-1);
            }//retro payments should be made as one payment
            abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.start_date = idtNextBenefitPaymentDate;
            abusPayeeAccountRetroPayment.icdoPayeeAccountRetroPayment.end_date = idtNextBenefitPaymentDate.AddMonths(1).AddDays(-1); ;
            //to do - Update gross amount,net amount
        }
        /// <summary>
        /// Method to set the visibility for Recalculate Benefit button
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsRecalculateButtonVisible()
        {
            bool lblnResult = false;
            if (ibusPayeeAccountActiveStatus == null)
                LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            //if (iclbPaymentHistoryHeader == null)
            //    LoadPaymentHistoryHeader();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            //Loading the data2 column from DBCache
            DataTable ldtbStatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2203, ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value);
            ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 = ldtbStatus.Rows.Count > 0 ? ldtbStatus.Rows[0]["data2"].ToString() : string.Empty;
            //commented as per call with Maik, any payments made check is removed as we also removed this check to initiate recalculate pension and rhic WFL
            //checking for any valid Payment Header records
            //IEnumerable<busPaymentHistoryHeader> lenmPaymentHistory = iclbPaymentHistoryHeader
            //                                                            .Where(o => o.icdoPaymentHistoryHeader.payment_date <= idtNextBenefitPaymentDate
            //                                                                && (o.icdoPaymentHistoryHeader.status_value == busConstant.PaymentStatusOutstanding
            //                                                                || o.icdoPaymentHistoryHeader.status_value == busConstant.PaymentStatusProcessed));
            if (ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReceiving ||
                ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusApproved ||
                ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReview)
                lblnResult = true;

            return lblnResult;
        }

        //PIR 11946
        public bool IsRecalculateButtonVisibleForRefund()
        {
            bool lblnResult = false;
            if (ibusPayeeAccountActiveStatus == null)
                LoadActivePayeeStatusAsofNextBenefitPaymentDate();

            //Loading the data2 column from DBCache
            DataTable ldtbStatus = iobjPassInfo.isrvDBCache.GetCodeDescription(2203, ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value);
            ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 = ldtbStatus.Rows.Count > 0 ? ldtbStatus.Rows[0]["data2"].ToString() : string.Empty;

            if (ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusApproved ||
                ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusReview ||
                ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value_data1 == busConstant.PayeeAccountStatusRefundProcessed)
                lblnResult = true;

            return lblnResult;
        }

        public busPayment1099r ibusPayment1099r { get; set; }
        /// <summary>
        /// Method to load latest payment1099r based on Payee account id and Year
        /// </summary>
        /// <param name="aintPayeeAccountID">Payee account id</param>
        /// <param name="aintYear">year</param>
        /// <returns>boolean value</returns>
        public bool LoadLatest1099rByPayeeAccount(int aintYear)
        {
            ibusPayment1099r = new busPayment1099r { icdoPayment1099r = new cdoPayment1099r() };
            bool lblnResult = false;
            DataTable ldt1099r = Select<cdoPayment1099r>
                (new string[2] { enmPayment1099r.payee_account_id.ToString(), enmPayment1099r.tax_year.ToString() },
                new object[2] { icdoPayeeAccount.payee_account_id, aintYear }, null, "payment_1099r_id desc");
            if (ldt1099r.Rows.Count > 0)
            {
                lblnResult = true;
                ibusPayment1099r.icdoPayment1099r.LoadData(ldt1099r.Rows[0]);
            }
            return lblnResult;
        }

        public Collection<busPaymentRecovery> iclbPaymentRecoveryWithoutBenefitOverpaymentHeader { get; set; }
        public void LoadPaymentRecoveryWithoutBenefitOverpaymentHeader()
        {
            if (iclbPaymentRecoveryWithoutBenefitOverpaymentHeader.IsNull())
                iclbPaymentRecoveryWithoutBenefitOverpaymentHeader = new Collection<busPaymentRecovery>();

            DataTable ldtbRecoveryList = Select("cdoPaymentRecovery.LoadRecovery", new object[1] { icdoPayeeAccount.payee_account_id });
            iclbPaymentRecoveryWithoutBenefitOverpaymentHeader = GetCollection<busPaymentRecovery>(ldtbRecoveryList, "icdoPaymentRecovery");
            foreach (busPaymentRecovery lobjRecovery in iclbPaymentRecoveryWithoutBenefitOverpaymentHeader)
            {
                lobjRecovery.LoadLTDAmountPaid();
                lobjRecovery.CalculateEstimatedEndDate();
            }
        }

        #endregion

        public busPayeeAccountStatus ibusPASOnGivenDate { get; set; }
        public void LoadPayeeAccountStatus(DateTime adtStatusEffectiveDate)
        {
            DataTable ldtbList = Select<cdoPayeeAccountStatus>(
                  new string[1] { "payee_account_id" },
                  new object[1] { icdoPayeeAccount.payee_account_id }, null, "status_effective_date desc,payee_account_status_id desc");
            Collection<busPayeeAccountStatus> lclbPAS = GetCollection<busPayeeAccountStatus>(ldtbList, "icdoPayeeAccountStatus");
            if (lclbPAS.Count > 0)
                ibusPASOnGivenDate = lclbPAS.Where(o => o.icdoPayeeAccountStatus.status_effective_date <= adtStatusEffectiveDate).FirstOrDefault();
            if (ibusPASOnGivenDate.IsNull())
                ibusPASOnGivenDate = new busPayeeAccountStatus { icdoPayeeAccountStatus = new cdoPayeeAccountStatus() };
        }

        //Property used in batch correspondence
        public DateTime idtFinalPaymentDate
        {
            get
            {
                if (ibusPayeeAccountActiveStatus == null)
                    LoadActivePayeeStatusAsofNextBenefitPaymentDate();
                if (ibusPayeeAccountActiveStatus != null)
                {
                    string lstrStatus = busGlobalFunctions.GetData2ByCodeValue(2203, ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value, iobjPassInfo);
                    if (lstrStatus == busConstant.PayeeAccountStatusPaymentComplete || lstrStatus == busConstant.PayeeAccountStatusRefundProcessed)
                    {
                        return ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_effective_date;
                    }
                }
                return DateTime.MinValue;
            }
        }

        public string istrFinalPaymentDate
        {
            get
            {
                return idtFinalPaymentDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        //uat pir 1385
        public decimal idecDisabilityTotalNonTaxableAmount { get; set; }
        public decimal idecDisabilityTotalTaxableAmount { get; set; }
        public decimal idecDisabilityTotalAccountAmount { get; set; }
        //uat pir 1385
        /// <summary>
        /// method to get the account balance for disability payee accounts
        /// </summary>
        public void GetAccountBalancesForDisability()
        {
            if (ibusApplication == null)
                LoadApplication();
            if (ibusApplication.iclbBenefitApplicationPersonAccounts == null)
                ibusApplication.LoadBenefitApplicationPersonAccount();
            busBenefitApplicationPersonAccount lobjBAPA = ibusApplication.iclbBenefitApplicationPersonAccounts
                                                                        .Where(o => o.icdoBenefitApplicationPersonAccount.is_application_person_account_flag == busConstant.Flag_Yes)
                                                                        .FirstOrDefault();
            if (lobjBAPA != null)
            {
                DataTable ldtPersonAccountRetrContr = Select("cdoPersonAccountRetirementContribution.LoadLTDDetail",
                    new object[1] { lobjBAPA.icdoBenefitApplicationPersonAccount.person_account_id });

                lobjBAPA.LoadPersonAccountRetirement();

                idecDisabilityTotalNonTaxableAmount = ldtPersonAccountRetrContr.AsEnumerable()
                                                            .Sum(o => o.Field<decimal>("post_tax_ee_amount"));
                idecDisabilityTotalTaxableAmount = ldtPersonAccountRetrContr.AsEnumerable()
                                                            .Sum(o => o.Field<decimal>("ee_er_pickup_amount") + o.Field<decimal>("pre_tax_ee_amount") +
                                                                o.Field<decimal>("interest_amount") + o.Field<decimal>("er_vested_amount"))
                                                                + lobjBAPA.ibusPersonAccountRetirement.icdoPersonAccountRetirement.capital_gain;
                idecDisabilityTotalAccountAmount = idecDisabilityTotalNonTaxableAmount + idecDisabilityTotalTaxableAmount;
            }
        }

        #region UCS - 075 Correspondence
        //property to load monthly benefit amount
        public busBenefitPostRetirementDROCalculation ibusBenefitPostRetirementDROCalculation { get; set; }
        //Method to load monthly benefit amount
        public void LoadMonthlyBenefitAmount(DateTime adtPaymentDate)
        {
            if (ibusBenefitPostRetirementDROCalculation == null)
                ibusBenefitPostRetirementDROCalculation = new busBenefitPostRetirementDROCalculation();
            ibusBenefitPostRetirementDROCalculation.CalculateGrossMonthlyAmount(adtPaymentDate, this);
        }
        //property to contain Benefit multiplier
        public int iintBenefitMultiplier
        {
            get
            {
                if (ibusBenefitCalculaton == null)
                    LoadBenefitCalculation();
                if (ibusBenefitCalculaton.iclbBenefitMultiplier == null)
                    ibusBenefitCalculaton.LoadBenefitMultiplier();
                if (ibusBenefitCalculaton.iclbBenefitMultiplier.Count > 0)
                    return Convert.ToInt32(ibusBenefitCalculaton.iclbBenefitMultiplier[0].icdoBenefitMultiplier.benefit_multiplier_rate * 100);
                else
                    return 0;
            }
        }
        //Property to contain Number of months the first payment represents
        public int iintNoofMonthsFirstPaymentRepresents { get; set; }
        //method to load Number of months the first payment represents
        public void NumberofMonthsFirstPaymentRepresents(DateTime adtPaymentDate)
        {
            iintNoofMonthsFirstPaymentRepresents = busGlobalFunctions.DateDiffByMonth(icdoPayeeAccount.benefit_begin_date, adtPaymentDate);
        }
        public string istrNoofMonthsFirstPaymentRepresents
        {
            get { return iintNoofMonthsFirstPaymentRepresents.ToString(); }
        }

        public string istrMonthYearPaymentDate { get; set; }
        public void FormatPaymentDate(DateTime adtPaymentDate)
        {
            istrMonthYearPaymentDate = adtPaymentDate.ToString("MMMM") + " " + adtPaymentDate.Year.ToString();
        }

        public string istrMonthYearTCEndDate
        {
            get
            {
                if (icdoPayeeAccount.term_certain_end_date != DateTime.MinValue)
                    return icdoPayeeAccount.term_certain_end_date.ToString("MMMM") + " " + icdoPayeeAccount.term_certain_end_date.Year.ToString();
                else
                    return string.Empty;
            }
        }

        //Property to display in retiree change notice
        public string istrPrimaryBeneficiaryName { get; set; }
        public string istrContingentBeneficiaryName { get; set; }
        public Collection<busPersonAccountBeneficiary> iclbPersonAccountBene { get; set; }
        public string istrIsBeneficiaryBlockVisible { get; set; }
        //Property to contain plan code
        public string istrPlanCode { get; set; }
        //Method to load the plan code
        public void LoadPlanCode()
        {
            if (ibusPlan == null)
                LoadPlan();
            if (IsPlanHP() || IsPlanJudges())
                istrPlanCode = "H";
            else if (IsPlanMain() || IsPlanLE() || IsPlanNG() || IsPlanBCILE() || IsPlanMain2020() || IsPlanStateLE()) //pir 7943,20232,25729
                istrPlanCode = "M";
        }
        //Propety to contain Joint Survior Payable factor
        public decimal idecJSPayableFactor
        {
            get
            {
                if (ibusBenefitCalculaton == null)
                    LoadBenefitCalculation();
                if (ibusBenefitCalculaton.iclbBenefitRHICOption == null)
                    ibusBenefitCalculaton.LoadBenefitRHICOption();
                if (ibusBenefitCalculaton.iclbBenefitRHICOption.Count > 0)
                    return Convert.ToDecimal(ibusBenefitCalculaton.iclbBenefitRHICOption[0].icdoBenefitRhicOption.option_factor * 100);
                else
                    return 0.00M;
            }
        }
        //Properties to conatin Benefit calculation items
        public decimal idecMonthlyNormalBenefit { get; set; }
        public decimal idecMonthlySingleBenefit { get; set; }
        public decimal idecFinalAverageSalary { get; set; }
        public decimal idecWSIorSSAReduction { get; set; }
        public decimal idecTermCertainFactor { get; set; }
        public decimal idecSpouseBenefit { get; set; }
        public string istrJSPercent { get; set; }
        public decimal idecMultiplier1 { get; set; }
        public decimal idecMultiplier2 { get; set; }
        public decimal idecMultiplier3 { get; set; }
        public decimal idecYrsofService1 { get; set; }
        public decimal idecYrsofService2 { get; set; }
        public decimal idecYrsofService3 { get; set; }
        public decimal idecMonthlyBenefit1 { get; set; }
        public decimal idecMonthlyBenefit2 { get; set; }
        public decimal idecMonthlyBenefit3 { get; set; }
        public decimal idecReducedPLSOBenefit { get; set; }
        public decimal idecLevelOptionSupplement { get; set; }
        public decimal idecLIBenefittoAgePrior { get; set; }
        public decimal idecLIBenefittoAgeBeg { get; set; }
        public decimal idecJSRHICOptionFactor { get; set; }
        public decimal idecMemberRHICAmount { get; set; }
        public decimal idecPLSOMonthlyBenefit { get; set; }
        public decimal idecHalfMonthlyBenefit { get; set; }
        public int iintJobServiceCondition { get; set; }
        public decimal idecOptionMonthlyBenefit { get; set; }
        public decimal idecReducedBenefit { get; set; }
        public string istrPaymentMethod { get; set; }
        public string istrPaymentMethodDesc { get; set; }
        public decimal idecEarlySingleLifeBenefit { get; set; }
        public decimal idecEarlyRetirementPayableFactor { get; set; }
        public decimal idecUIFactor { get; set; }
        public decimal idecReducedQDROBenefit { get; set; }
        public decimal idecReducedQDROBenefitAfterDNRO { get; set; }
        public decimal idecReducedBenefitDecrease { get; set; }
        public decimal idecGraduatedBenefitFactor { get; set; }
        public decimal idecGraduatedBenefit { get; set; }
        public decimal idecEarlySingleLifeBenefitAfterDecrease { get; set; }
        public int iintNumberOfPayments { get; set; }
        public decimal idecActurallyAdjustedBenefit { get; set; }
        public decimal idecPLSOPayableFactor { get; set; }
        public decimal idecEarlyMonthlyPayableFactor { get; set; }
        public decimal idecSpouseRHICAmount { get; set; }
        public string istrChangeDate { get; set; }
        public decimal idecDNROIncrease { get; set; }
        public string istrNumberOfPaymentsMade { get; set; }
        public string istrFirstPaymentDate { get; set; }
        //PIR 7891
        public void SetNumberOfPayments()
        {
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            istrNumberOfPaymentsMade = iclbPaymentHistoryHeader.Where(i => i.icdoPaymentHistoryHeader.status_value == "OUST" || i.icdoPaymentHistoryHeader.status_value == "PRCD").Count().ToString();
        }
        //PIR 7891
        public void SetFirstPaymentDate()
        {
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            iclbPaymentHistoryHeader.OrderBy(i => i.icdoPaymentHistoryHeader.payment_date);
            busPaymentHistoryHeader lobjPaymentHistory = iclbPaymentHistoryHeader.FirstOrDefault();
            if (lobjPaymentHistory.icdoPaymentHistoryHeader.payment_history_header_id > 0)
            {
                istrFirstPaymentDate = lobjPaymentHistory.icdoPaymentHistoryHeader.payment_date.ToString(busConstant.DateFormatMonthYear);
            }
        }
        // property used in 75- correspondence
        public string istrIsDROTermCertain
        {
            get
            {
                if (icdoPayeeAccount.dro_calculation_id > 0)
                {
                    DataTable ldtdBenefitOption = iobjPassInfo.isrvDBCache.GetCodeDescription(icdoPayeeAccount.benefit_option_id, icdoPayeeAccount.benefit_option_value);
                    if (ldtdBenefitOption.Rows.Count > 0)
                    {
                        return ldtdBenefitOption.Rows[0]["data1"] != DBNull.Value ? busConstant.Flag_Yes : busConstant.Flag_No;
                    }
                }
                return busConstant.Flag_No; ;
            }
        }

        public string istrBenefitBeginDateMonthAndYear
        {
            get
            {
                return icdoPayeeAccount.benefit_begin_date.ToString(busConstant.DateFormatMonthYear);
            }
        }

        //Property to contain Member acct balance        
        public decimal idecMemberTaxableAmount { get; set; }
        public decimal idecMemberNonTaxableAmount { get; set; }
        public decimal idecMemberAccountBalance { get; set; }

        //method to load JS percentage(50% or 100%)
        public void LoadJSPercent()
        {
            if (ibusApplication == null)
                LoadApplication();
            istrJSPercent = busGlobalFunctions.GetData1ByCodeValue(1905, ibusApplication.icdoBenefitApplication.rhic_option_value, iobjPassInfo);
        }
        //Common method to load all Calculation related properties
        public void LoadBenefits()
        {
            if (ibusBenefitCalculaton == null)
                LoadBenefitCalculation();
            if (ibusBenefitCalculaton.iclbBenefitCalculationOtherDisBenefit == null)
                ibusBenefitCalculaton.LoadBenefitCalculationOtherDisBenefit();
            if (ibusBenefitCalculaton.iclbBenefitCalculationPayee == null)
                ibusBenefitCalculaton.LoadBenefitCalculationPayee();
            if (ibusBenefitCalculaton.iclbBenefitCalculationOptions == null)
                ibusBenefitCalculaton.LoadBenefitCalculationOptions();
            if (ibusBenefitCalculaton.iclbBenefitMultiplier == null)
                ibusBenefitCalculaton.LoadBenefitMultiplier();
            if (ibusBenefitCalculaton.ibusPlan == null)
                ibusBenefitCalculaton.LoadPlan();
            if (ibusBenefitCalculaton.iclbBenefitProvisionMultiplier == null)
            {
				//PIR 24881 Graduated Benefit option
                if (ibusApplication.IsNull()) LoadApplication();
                if(ibusApplication.ibusPersonAccount.IsNull()) ibusApplication.LoadPersonAccount();
                DateTime ldtEarlyStartDate = ibusApplication.ibusPersonAccount.GetEarlyPlanParticiaptionStartDate();
                if (ldtEarlyStartDate == DateTime.MinValue)
                    ldtEarlyStartDate = ibusApplication.ibusPersonAccount.icdoPersonAccount.start_date;
                
                //ibusBenefitCalculaton.LoadBenefitProvisionMultiplier();
                ibusBenefitCalculaton.iclbBenefitProvisionMultiplier = busPersonBase.LoadBenefitProvisionMultiplier(
                    ldtEarlyStartDate, ibusBenefitCalculaton.ibusPlan.icdoPlan.benefit_provision_id,
                    ibusBenefitCalculaton.icdoBenefitCalculation.benefit_account_type_value);
            }
            if (ibusBenefitCalculaton.iclbBenefitRHICOption == null)
                ibusBenefitCalculaton.LoadBenefitRHICOption();

            if (icdoPayeeAccount.benefit_account_type_value != busConstant.ApplicationBenefitTypeDisability)
            {
                if (ibusBenefitAccount == null)
                    LoadBenfitAccount();
                idecMemberTaxableAmount = ibusBenefitAccount.icdoBenefitAccount.starting_taxable_amount;
                idecMemberNonTaxableAmount = ibusBenefitAccount.icdoBenefitAccount.starting_nontaxable_amount;
            }
            else
            {
                GetAccountBalancesForDisability();
                idecMemberTaxableAmount = idecDisabilityTotalTaxableAmount;
                idecMemberNonTaxableAmount = idecDisabilityTotalNonTaxableAmount;
            }
            idecMemberAccountBalance = idecMemberNonTaxableAmount + idecMemberTaxableAmount;

            idecFinalAverageSalary = ibusBenefitCalculaton.icdoBenefitCalculation.overridden_final_average_salary > 0 ?
                ibusBenefitCalculaton.icdoBenefitCalculation.overridden_final_average_salary :
                ibusBenefitCalculaton.icdoBenefitCalculation.computed_final_average_salary;
            idecMonthlyNormalBenefit = Math.Round(idecFinalAverageSalary * Convert.ToDecimal(0.7), 2);
            idecMonthlySingleBenefit = Math.Round(idecFinalAverageSalary * Convert.ToDecimal(0.25), 2);
            idecWSIorSSAReduction = ibusBenefitCalculaton.iclbBenefitCalculationOtherDisBenefit
                                    .Sum(o => o.icdoBenefitCalculationOtherDisBenefit.monthly_benefit_amount);
            int lintBenefitCalcPayeeId = (from lobjBenefitCalcPayee in ibusBenefitCalculaton.iclbBenefitCalculationPayee
                                          where lobjBenefitCalcPayee.icdoBenefitCalculationPayee.family_relationship_value == busConstant.FamilyRelationshipMember
                                          select lobjBenefitCalcPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id).FirstOrDefault();
            int lintBenefitCalcSpousePayeeId = (from lobjBenefitCalcPayee in ibusBenefitCalculaton.iclbBenefitCalculationPayee
                                                where lobjBenefitCalcPayee.icdoBenefitCalculationPayee.family_relationship_value == busConstant.FamilyRelationshipSpouse
                                                select lobjBenefitCalcPayee.icdoBenefitCalculationPayee.benefit_calculation_payee_id).FirstOrDefault();
            decimal ldecOptionFactor = (from lobjBenefitCalOption in ibusBenefitCalculaton.iclbBenefitCalculationOptions
                                        where lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_calculation_payee_id == lintBenefitCalcPayeeId &&
                                     lobjBenefitCalOption.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value != busConstant.BenefitOptionRefund
                                        select lobjBenefitCalOption.icdoBenefitCalculationOptions.option_factor).FirstOrDefault();
            idecTermCertainFactor = ldecOptionFactor * 100;
            if (ibusBenefitCalculaton.icdoBenefitCalculation.plso_requested_flag == busConstant.Flag_Yes)
            {
                idecSpouseBenefit = (from lobjBenefitCalOption in ibusBenefitCalculaton.iclbBenefitCalculationOptions
                                     where lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_calculation_payee_id == lintBenefitCalcSpousePayeeId &&
                                     lobjBenefitCalOption.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value != busConstant.BenefitOptionRefund
                                     select lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_with_plso).FirstOrDefault();
                idecOptionMonthlyBenefit = (from lobjBenefitCalOption in ibusBenefitCalculaton.iclbBenefitCalculationOptions
                                            where lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_calculation_payee_id == lintBenefitCalcPayeeId &&
                                     lobjBenefitCalOption.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value != busConstant.BenefitOptionRefund
                                            select lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_with_plso).FirstOrDefault();
            }
            else
            {
                idecSpouseBenefit = (from lobjBenefitCalOption in ibusBenefitCalculaton.iclbBenefitCalculationOptions
                                     where lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_calculation_payee_id == lintBenefitCalcSpousePayeeId &&
                                     lobjBenefitCalOption.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value != busConstant.BenefitOptionRefund
                                     select lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_option_amount).FirstOrDefault();
                idecOptionMonthlyBenefit = (from lobjBenefitCalOption in ibusBenefitCalculaton.iclbBenefitCalculationOptions
                                            where lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_calculation_payee_id == lintBenefitCalcPayeeId &&
                                     lobjBenefitCalOption.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value != busConstant.BenefitOptionRefund
                                            select lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_option_amount).FirstOrDefault();
            }
            //PIR 24881 Graduated Benefit option need to be considered in the templates "PAY-4018" & "PAY-4014" 
            //if (ibusBenefitCalculaton.icdoBenefitCalculation.benefit_option_value == busConstant.BenefitOptionSingleLife)
            //{
                idecGraduatedBenefitFactor = (from lobjBenefitCalOption in ibusBenefitCalculaton.iclbBenefitCalculationOptions
                                              where
                                         lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_calculation_payee_id == lintBenefitCalcPayeeId &&
                                         lobjBenefitCalOption.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value != busConstant.BenefitOptionRefund
                                              select lobjBenefitCalOption.icdoBenefitCalculationOptions.graduated_benefit_factor).FirstOrDefault();
            //}
            //else
            //{
            //    idecGraduatedBenefitFactor = (from lobjBenefitCalOption in ibusBenefitCalculaton.iclbBenefitCalculationOptions
            //                                  where lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_calculation_payee_id != lintBenefitCalcPayeeId &&
            //                             lobjBenefitCalOption.ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.benefit_option_value != busConstant.BenefitOptionRefund
            //                                  select lobjBenefitCalOption.icdoBenefitCalculationOptions.graduated_benefit_factor).FirstOrDefault();
            //}
            // PIR-11233 repurposed under PIR-5883
            if (ibusBenefitCalculaton.icdoBenefitCalculation.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value50JointSurvivor ||
                ibusBenefitCalculaton.icdoBenefitCalculation.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value55JointSurvivor ||
                ibusBenefitCalculaton.icdoBenefitCalculation.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value75JointSurvivor ||
                ibusBenefitCalculaton.icdoBenefitCalculation.benefit_option_value == busConstant.ApplicationBenefitOptionValueData3Value100JointSurvivor)
            {
                idecJSMonthlyBenefit = (from lobjBenefitCalOption in ibusBenefitCalculaton.iclbBenefitCalculationOptions
                                        where lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_calculation_payee_id != lintBenefitCalcPayeeId &&
                                   lobjBenefitCalOption.ibusBenefitCalculationPayee.icdoBenefitCalculationPayee.account_relationship_value == busConstant.AccountRelationshipJointAnnuitant
                                        select lobjBenefitCalOption.icdoBenefitCalculationOptions.benefit_option_amount).FirstOrDefault();
            }
            idecUIFactor = Math.Round(ibusBenefitCalculaton.icdoBenefitCalculation.ssli_uniform_income_factor * 100, 2);
            int lintCount = 1;
            foreach (cdoBenefitProvisionMultiplier lcdoPM in ibusBenefitCalculaton.iclbBenefitProvisionMultiplier)
            {
                if (lintCount == 1)
                    idecMultiplier1 = lcdoPM.multipier_percentage_formatted;
                else if (lintCount == 2)
                    idecMultiplier2 = lcdoPM.multipier_percentage_formatted;
                else if (lintCount == 3)
                    idecMultiplier3 = lcdoPM.multipier_percentage_formatted;
                lintCount++;
            } // override above values with actual stored values which was calculated at the time of calculation
            lintCount = 1;
            foreach (busBenefitMultiplier lobjBenefitMultiplier in ibusBenefitCalculaton.iclbBenefitMultiplier)
            {
                if (lintCount == 1)
                {
                    idecMultiplier1 = Math.Round(lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_rate * 100, 2, MidpointRounding.AwayFromZero);
                    idecYrsofService1 = lobjBenefitMultiplier.icdoBenefitMultiplier.pension_service_credit;
                    idecMonthlyBenefit1 = Math.Round(lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_amount, 2, MidpointRounding.AwayFromZero);
                }
                else if (lintCount == 2)
                {
                    idecMultiplier2 = Math.Round(lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_rate * 100, 2, MidpointRounding.AwayFromZero);
                    idecYrsofService2 = lobjBenefitMultiplier.icdoBenefitMultiplier.pension_service_credit;
                    idecMonthlyBenefit2 = Math.Round(lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_amount, 2, MidpointRounding.AwayFromZero);
                }
                else if (lintCount == 3)
                {
                    idecMultiplier3 = Math.Round(lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_rate * 100, 2, MidpointRounding.AwayFromZero);
                    idecYrsofService3 = lobjBenefitMultiplier.icdoBenefitMultiplier.pension_service_credit;
                    idecMonthlyBenefit3 = Math.Round(lobjBenefitMultiplier.icdoBenefitMultiplier.benefit_multiplier_amount, 2, MidpointRounding.AwayFromZero);
                }
                lintCount++;
            }
            idecReducedPLSOBenefit = idecMonthlyBenefit1 - ibusBenefitCalculaton.icdoBenefitCalculation.plso_reduction_amount;
            //Set Values for Calculation properties
            if (ibusBenefitCalculaton.icdoBenefitCalculation.plan_id != busConstant.PlanIdJobService)
            {
                ibusBenefitCalculaton.icdoBenefitCalculation.early_retirement_percentage_decrease =
                    (ibusBenefitCalculaton.icdoBenefitCalculation.early_reduced_months *
                    ibusBenefitCalculaton.icdoBenefitCalculation.early_reduction_factor);
            }
            else
            {
                if (ibusBenefitCalculaton.icdoBenefitCalculation.early_reduction_factor != 0M)
                    ibusBenefitCalculaton.icdoBenefitCalculation.early_retirement_percentage_decrease =
                        1 - ibusBenefitCalculaton.icdoBenefitCalculation.early_reduction_factor;
            }
            idecEarlyRetirementPayableFactor = 100.00M - Math.Round(ibusBenefitCalculaton.icdoBenefitCalculation.early_retirement_percentage_decrease * 100,
                2, MidpointRounding.AwayFromZero);
            idecEarlySingleLifeBenefit = idecMonthlyBenefit1 - ibusBenefitCalculaton.icdoBenefitCalculation.early_monthly_decrease;
            idecLevelOptionSupplement = Math.Round(ibusBenefitCalculaton.icdoBenefitCalculation.estimated_ssli_benefit_amount *
                                        ibusBenefitCalculaton.icdoBenefitCalculation.ssli_uniform_income_factor, 2, MidpointRounding.AwayFromZero);
            idecLIBenefittoAgePrior = idecOptionMonthlyBenefit + idecLevelOptionSupplement;
            idecLIBenefittoAgeBeg = idecLIBenefittoAgePrior - ibusBenefitCalculaton.icdoBenefitCalculation.estimated_ssli_benefit_amount;
            if (ibusBenefitCalculaton.iclbBenefitRHICOption.Count > 0)
            {
                idecJSRHICOptionFactor = Math.Round(ibusBenefitCalculaton.iclbBenefitRHICOption.FirstOrDefault().icdoBenefitRhicOption.option_factor, 2, MidpointRounding.AwayFromZero);
                idecMemberRHICAmount = ibusBenefitCalculaton.iclbBenefitRHICOption.FirstOrDefault().icdoBenefitRhicOption.member_rhic_amount;
                idecSpouseRHICAmount = ibusBenefitCalculaton.iclbBenefitRHICOption.FirstOrDefault().icdoBenefitRhicOption.spouse_rhic_amount;
            }

            idecHalfMonthlyBenefit = ibusBenefitCalculaton.icdoBenefitCalculation.final_monthly_benefit / 2;

            idecReducedQDROBenefitAfterDNRO = ibusBenefitCalculaton.icdoBenefitCalculation.dnro_monthly_increase - ibusBenefitCalculaton.icdoBenefitCalculation.qdro_amount;

            if (ibusBenefitCalculaton.idecDNROPercentageIncrease > 0)
                idecDNROIncrease = 1 + ibusBenefitCalculaton.idecDNROPercentageIncrease; //will be printing in percentage format in corrs. template, so need to add with 1(100/100)

            if (ibusBenefitCalculaton.icdoBenefitCalculation.actuarially_adjusted_monthly_single_life_benefit != 0)
            {
                idecActurallyAdjustedBenefit = ibusBenefitCalculaton.icdoBenefitCalculation.actuarially_adjusted_monthly_single_life_benefit;
                idecReducedBenefitDecrease = ibusBenefitCalculaton.icdoBenefitCalculation.unreduced_benefit_amount -
                    ibusBenefitCalculaton.icdoBenefitCalculation.actuarially_adjusted_monthly_single_life_benefit;
            }
            else
            {
                idecReducedBenefitDecrease = 0.0M;
                idecActurallyAdjustedBenefit = ibusBenefitCalculaton.icdoBenefitCalculation.unreduced_benefit_amount;
            }
            if (icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
                idecReducedBenefit = idecActurallyAdjustedBenefit - ibusBenefitCalculaton.icdoBenefitCalculation.early_monthly_decrease;
            else
                idecReducedBenefit = idecActurallyAdjustedBenefit + ibusBenefitCalculaton.icdoBenefitCalculation.dnro_monthly_increase;
            if (idecActurallyAdjustedBenefit > 0.0m)
            {
                idecEarlyMonthlyPayableFactor = Math.Round((idecReducedBenefit / idecActurallyAdjustedBenefit) * 100, 2, MidpointRounding.AwayFromZero);
            }
            idecReducedQDROBenefit = (idecReducedBenefit > 0 ? idecReducedBenefit : ibusBenefitCalculaton.icdoBenefitCalculation.unreduced_benefit_amount) -
                ibusBenefitCalculaton.icdoBenefitCalculation.qdro_amount;
            idecPLSOPayableFactor = 100 - (ibusBenefitCalculaton.icdoBenefitCalculation.plso_factor * 100);
            idecPLSOMonthlyBenefit = idecReducedQDROBenefit - ibusBenefitCalculaton.icdoBenefitCalculation.plso_reduction_amount;
            idecGraduatedBenefitFactor = idecGraduatedBenefitFactor * 100;
            idecGraduatedBenefit = idecOptionMonthlyBenefit; //graduated benefit amount is stored in benefit option amount itself

            if (string.IsNullOrEmpty(ibusJointAnnuitant.icdoPerson.FullName))
                ibusJointAnnuitant.icdoPerson.first_name = "Spouse";
        }

        public decimal idecReducedMonthlyHealthCredit
        {
            get
            {
                if (ibusBenefitCalculaton == null)
                    LoadBenefitCalculation();
                if (ibusBenefitCalculaton.iclbBenefitRHICOption == null)
                    ibusBenefitCalculaton.LoadBenefitRHICOption();
                if (ibusBenefitCalculaton.iclbBenefitRHICOption.Count > 0)
                    return ibusBenefitCalculaton.iclbBenefitRHICOption[0].icdoBenefitRhicOption.member_rhic_amount;
                return 0M;
            }
        }

        public int iintTotalBenefitMonths
        {
            get
            {
                if (icdoPayeeAccount.benefit_end_date == DateTime.MinValue)
                    return 0;
                else
                    return busGlobalFunctions.DateDiffByMonth(icdoPayeeAccount.benefit_begin_date, icdoPayeeAccount.benefit_end_date);
            }
        }

        public busPerson ibusJointAnnuitant { get; set; }
        public void LoadJointAnnuitant()
        {
            if (ibusApplication == null)
                LoadApplication();
            ibusJointAnnuitant = new busPerson { icdoPerson = new cdoPerson() };
            ibusJointAnnuitant.FindPerson(ibusApplication.icdoBenefitApplication.joint_annuitant_perslink_id);
        }

        public bool IsPlanHP()
        {
            bool lblnResult = false;
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdHP)
                lblnResult = true;
            return lblnResult;
        }

        public bool IsPlanJudges()
        {
            bool lblnResult = false;
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJudges)
                lblnResult = true;
            return lblnResult;
        }

        public bool IsPlanMain()
        {
            bool lblnResult = false;
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMain)
                lblnResult = true;
            return lblnResult;
        }

        public bool IsPlanLE()
        {
            bool lblnResult = false;
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdLE)
                lblnResult = true;
            return lblnResult;
        }

        //PIR 17270 - Added LE without Prior Service Plan to the Surviving Spouse Condition 
        public bool IsPlanLEWithoutPriorService()
        {
            bool lblnResult = false;
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdLEWithoutPS)
                lblnResult = true;
            return lblnResult;
        }
        //pir 7943
        public bool IsPlanBCILE()
        {
            bool lblnResult = false;
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdBCILawEnf)
                lblnResult = true;
            return lblnResult;
        }
        //PIR 25729
        public bool IsPlanStateLE()
        {
            bool lblnResult = false;
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdStatePublicSafety)
                lblnResult = true;
            return lblnResult;
        }

        //pir 20232
        public bool IsPlanMain2020()
        {
            bool lblnResult = false;
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdMain2020)
                lblnResult = true;
            return lblnResult;
        }

        //pir 20232
        public bool IsPlanDc2020()
        {
            bool lblnResult = false;
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdDC2020)
                lblnResult = true;
            return lblnResult;
        }
        public bool IsPlanNG()
        {
            bool lblnResult = false;
            if (ibusPlan == null)
                LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdNG)
                lblnResult = true;
            return lblnResult;
        }

        public bool IsPLSO()
        {
            bool lblnResult = false;
            if (ibusBenefitCalculaton == null)
                LoadBenefitCalculation();
            if (ibusBenefitCalculaton.icdoBenefitCalculation.plso_requested_flag == busConstant.Flag_Yes)
                lblnResult = true;
            return lblnResult;
        }

        public bool IsGraduatedBenefit()
        {
            bool lblnResult = false;
            if (idecGraduatedBenefitFactor > 0)
                lblnResult = true;
            return lblnResult;
        }

        public bool IsReducedBenefit()
        {
            bool lblnResult = false;
            if (ibusBenefitCalculaton == null)
                LoadBenefitCalculation();
            if (ibusBenefitCalculaton.icdoBenefitCalculation.actuarial_benefit_reduction != 0)
                lblnResult = true;
            return lblnResult;
        }

        public bool IsQDROApplies()
        {
            bool lblnResult = false;
            if (ibusBenefitCalculaton == null)
                LoadBenefitCalculation();
            if (ibusBenefitCalculaton.icdoBenefitCalculation.qdro_amount != 0)
                lblnResult = true;
            return lblnResult;
        }

        public bool IsNormalRetirement()
        {
            bool lblnResult = false;
            if (icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal)
                lblnResult = true;
            return lblnResult;
        }

        public bool IsEarlyRetirement()
        {
            bool lblnResult = false;
            if (icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
                lblnResult = true;
            return lblnResult;
        }

        public bool IsDNRO()
        {
            bool lblnResult = false;
            if (icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO)
                lblnResult = true;
            return lblnResult;
        }

        public int iintHPJudgesNBCondition { get; set; }
        public void SetHPJudgesNBCondition()
        {
            if (IsPlanHP() && IsEarlyRetirement())
                iintHPJudgesNBCondition = 1;
            else if (IsPlanHP() && IsDNRO() && !IsPLSO())
                iintHPJudgesNBCondition = 3;
            else if (IsPlanHP() && IsNormalRetirement() && IsGraduatedBenefit())
                iintHPJudgesNBCondition = 4;
            else if (IsPlanHP() && IsNormalRetirement() && IsPLSO())
                iintHPJudgesNBCondition = 5;
            else if (IsPlanHP() && IsDNRO() && IsPLSO())
                iintHPJudgesNBCondition = 11;
            else if (IsPlanHP() && IsNormalRetirement() && !IsPLSO() && !IsGraduatedBenefit())
                iintHPJudgesNBCondition = 2;
            else if (IsPlanJudges() && IsEarlyRetirement())
                iintHPJudgesNBCondition = 6;
            else if (IsPlanJudges() && IsDNRO() && !IsPLSO())
                iintHPJudgesNBCondition = 8;
            else if (IsPlanJudges() && IsNormalRetirement() && IsGraduatedBenefit())
                iintHPJudgesNBCondition = 9;
            else if (IsPlanJudges() && IsNormalRetirement() && IsPLSO())
                iintHPJudgesNBCondition = 10;
            else if (IsPlanJudges() && IsDNRO() && IsPLSO())
                iintHPJudgesNBCondition = 12;
            else if (IsPlanJudges() && IsNormalRetirement() && !IsPLSO() && !IsGraduatedBenefit())
                iintHPJudgesNBCondition = 7;
        }

        public int iintBenefitAcctSubType { get; set; }
        public void SetBenefitAcctSubType()
        {
            if (IsDNRO())
                iintBenefitAcctSubType = 1;
            else if (IsNormalRetirement() && IsGraduatedBenefit())
                iintBenefitAcctSubType = 2;
            else if (IsNormalRetirement() && IsPLSO())
                iintBenefitAcctSubType = 3;
        }

        public int iintQDROApplies { get; set; }
        public void SetQDROApplies()
        {
            if (IsQDROApplies())
                iintQDROApplies = 1;
        }

        public void SetJobServiceCondition()
        {
            if (ibusPlan == null)
                LoadPlan();
            if (ibusApplication == null)
                LoadApplication();
            if (ibusPlan.icdoPlan.plan_id == 6)
            {
                if (icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
                {
                    if (ibusApplication.icdoBenefitApplication.estimated_ssli_benefit_amount > 0)
                        iintJobServiceCondition = 2;
                    else
                        iintJobServiceCondition = 1;
                }
                else if (icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal)
                {
                    if (ibusApplication.icdoBenefitApplication.estimated_ssli_benefit_amount > 0)
                        iintJobServiceCondition = 4;
                    else
                        iintJobServiceCondition = 3;
                }
                else if (icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO)
                {
                    if (ibusApplication.icdoBenefitApplication.estimated_ssli_benefit_amount > 0)
                        iintJobServiceCondition = 6;
                    else
                        iintJobServiceCondition = 5;
                }
            }
        }

        public int iintJSRHICCondition { get; set; }
        public void SetJSRHICCondition()
        {
            if (ibusApplication == null)
                LoadApplication();
            if (ibusApplication.icdoBenefitApplication.rhic_option_value != busConstant.RHICOptionStandard)
            {
                if (icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeNormal ||
                    icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeDNRO)
                    iintJSRHICCondition = 1;
                else if (icdoPayeeAccount.benefit_account_sub_type_value == busConstant.ApplicationBenefitSubTypeEarly)
                    iintJSRHICCondition = 2;
            }
        }

        public int iintNoofMonths
        {
            get
            {
                if (icdoPayeeAccount.term_certain_end_date == DateTime.MinValue)
                    return 0;
                else
                    return busGlobalFunctions.DateDiffByMonth(icdoPayeeAccount.benefit_begin_date, icdoPayeeAccount.term_certain_end_date);
            }
        }

        public int iintSurvivingSpouseCondition { get; set; }
        public void LoadSurvivingSpouseCondition()
        {
            if (((IsPlanMain() || IsPlanLE() || IsPlanNG() || IsPlanBCILE() || IsPlanLEWithoutPriorService() || IsPlanStateLE() || IsPlanMain2020()) && //PIR 25729
                (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption50PercentJS || icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption100PercentJS))
                || ((IsPlanHP() || IsPlanJudges()) && icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption100PercentJS))
                iintSurvivingSpouseCondition = 1;
            else if ((IsPlanHP() || IsPlanJudges()) && icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionNormalRetBenefit)
                iintSurvivingSpouseCondition = 2;
        }

        public int iintRetirementCondition { get; set; }
        public void SetRetirementCondtion()
        {
            if (IsEarlyRetirement())
                iintRetirementCondition = 1;
            else if (IsNormalRetirement() && !IsPLSO() && !IsGraduatedBenefit())
                iintRetirementCondition = 2;
            else if (IsDNRO() && !IsPLSO())
                iintRetirementCondition = 3;
            else if (IsNormalRetirement() && IsGraduatedBenefit())
                iintRetirementCondition = 4;
            else if (IsNormalRetirement() && IsPLSO())
                iintRetirementCondition = 5;
            else if (IsDNRO() && IsPLSO())
                iintRetirementCondition = 6;
        }

        public int iintReducedBenefit { get; set; }
        public void SetReducedBenefit()
        {
            if (IsReducedBenefit())
                iintReducedBenefit = 1;
        }

        public string istrRTWOrReducedBenefitOption { get; set; }
        public void LoadRTWOrReducedBeneftiOption()
        {
            if (ibusBenefitCalculaton == null)
                LoadBenefitCalculation();
            if ((ibusBenefitCalculaton.icdoBenefitCalculation.reduced_benefit_flag == busConstant.Flag_Yes ||
                ibusBenefitCalculaton.icdoBenefitCalculation.pre_rtw_payee_account_id > 0) && idecReducedBenefitDecrease > 0)
            {
                istrRTWOrReducedBenefitOption = busConstant.Flag_Yes;
            }
            else
            {
                istrRTWOrReducedBenefitOption = busConstant.Flag_No;
            }
        }

        #endregion

        # region UCS -084 Correspondence
        //used for 75 correspondence also
        public decimal idecChildSupportAmount { get; set; }
        public decimal idecTaxLevyAmount { get; set; }
        public decimal idec3rdPartyHealthInsuranceAmount { get; set; }
        public decimal idecAFPEDuesAmount { get; set; }
        public decimal idecNDPEADuesAmount { get; set; }
        public decimal idecMiscellaneousDueAmount { get; set; }
        public void LoadOtherDeductionAmount()
        {
            if (iclbDeductions.IsNull())
                LoadDeductions();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            idecChildSupportAmount = iclbDeductions.Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPITChildSupport) &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Sum(o => o.icdoPayeeAccountPaymentItemType.amount);
            idecTaxLevyAmount = iclbDeductions.Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDTaxLevy) &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Sum(o => o.icdoPayeeAccountPaymentItemType.amount);
            idec3rdPartyHealthInsuranceAmount = iclbDeductions.Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPIT3rdPartyHealthInsurance) &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Sum(o => o.icdoPayeeAccountPaymentItemType.amount);

            idecMiscellaneousDueAmount = idecChildSupportAmount + idecTaxLevyAmount + idec3rdPartyHealthInsuranceAmount;

            var lenumDueDeductions = iclbDeductions.Where(o => o.icdoPayeeAccountPaymentItemType.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPITAFPEDues) &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date));
            foreach (busPayeeAccountPaymentItemType lobjPAPIT in lenumDueDeductions)
            {
                if (lobjPAPIT.ibusVendor.IsNull())
                    lobjPAPIT.LoadVendor();
                if (lobjPAPIT.ibusVendor.icdoOrganization.org_code ==
                                                                 (busGlobalFunctions.GetData1ByCodeValue(52, busConstant.NDPEAValue, iobjPassInfo).ToString()))
                {
                    idecNDPEADuesAmount = idecNDPEADuesAmount + lobjPAPIT.icdoPayeeAccountPaymentItemType.amount;
                }
                else if (lobjPAPIT.ibusVendor.icdoOrganization.org_code ==
                                                                (busGlobalFunctions.GetData1ByCodeValue(52, busConstant.AFPEValue, iobjPassInfo).ToString()))
                {
                    idecAFPEDuesAmount = idecAFPEDuesAmount + lobjPAPIT.icdoPayeeAccountPaymentItemType.amount;
                }
            }
        }
        //Porperty to Load Insurance Premium Amounts
        public decimal idecHealthInsurancePremiumAmount { get; set; }
        public decimal idecLTCInsurancePremiumAmount { get; set; }
        public decimal idecDentalInsurancePremiumAmount { get; set; }
        public decimal idecVisionInsurancePremiumAmount { get; set; }
        public decimal idecLifePremiumAmount { get; set; }
        //Method to Correspondence Properties
        public void LoadInsurancePremiumAmount()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            if (iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            idecHealthInsurancePremiumAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemHealth &&
                 busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                 ).Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();
            idecLTCInsurancePremiumAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemLTC &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();
            idecDentalInsurancePremiumAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemDental &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();
            idecLifePremiumAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemLife &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();
            idecVisionInsurancePremiumAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemVision &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();
            idecMedicareInsPremiumAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemMedicare &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum(); //PIR 18243
        }
        public decimal idecMedicareInsPremiumAmount { get; set; }
        //Property to load Fed Tax
        public decimal idecFedTaxAmount { get; set; }
        //Property to load State Tax
        public decimal idecStateTaxAmount { get; set; }
        //Load Tax amounts
        //Property to indicate tax changes
        public string istrTaxChanges { get; set; }
        public void LoadTaxAmounts()
        {
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
            {
                LoadNexBenefitPaymentDate();
            }
            LoadTaxAmounts(idtNextBenefitPaymentDate);
        }

        public void LoadTaxAmounts(DateTime adtEffectivedate)
        {
            if (iclbPayeeAccountPaymentItemType == null)
                LoadPayeeAccountPaymentItemType();
            idecFedTaxAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.ach_check_group_value == busConstant.ACHCheckGroupValueFedTax &&
                 o.ibusPaymentItemType.icdoPaymentItemType.item_usage_value == busConstant.ItemUsageMontlhyPayment &&
                 o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                  o.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_Yes &&
                busGlobalFunctions.CheckDateOverlapping(adtEffectivedate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();
            idecStateTaxAmount = iclbPayeeAccountPaymentItemType.Where(o =>
               o.ibusPaymentItemType.icdoPaymentItemType.ach_check_group_value == busConstant.ACHCheckGroupValueStateTax &&
               o.ibusPaymentItemType.icdoPaymentItemType.item_usage_value == busConstant.ItemUsageMontlhyPayment &&
               o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
               o.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_Yes &&
               busGlobalFunctions.CheckDateOverlapping(adtEffectivedate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
               ).Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();
        }


        //Load BenefitPayment Change correspondece data

        public decimal idecOldChildSupportAmount { get; set; }
        public decimal idecOldTaxLevyAmount { get; set; }
        public decimal idecOld3rdPartyHealthInsuranceAmount { get; set; }
        public decimal idecOldAFPEDuesAmount { get; set; }
        public decimal idecOldNDPEADuesAmount { get; set; }
        public decimal idecOldMiscellaneousDueAmount { get; set; }
        public void LoadOldOtherDeductionAmount()
        {
            Collection<busPaymentHistoryDetail> lclbPaymentHistoryDetails = new Collection<busPaymentHistoryDetail>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                if (iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail == null)
                    iclbPaymentHistoryHeader[0].LoadPaymentHistoryDetails();
                lclbPaymentHistoryDetails = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail;

                if (iclbDeductions.IsNull())
                    LoadDeductions();
                idecOldChildSupportAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.icdoPaymentHistoryDetail.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPITChildSupport))
                    .Sum(o => o.icdoPaymentHistoryDetail.amount);
                idecOldTaxLevyAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.icdoPaymentHistoryDetail.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDTaxLevy))
                    .Sum(o => o.icdoPaymentHistoryDetail.amount);
                idecOld3rdPartyHealthInsuranceAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.icdoPaymentHistoryDetail.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPIT3rdPartyHealthInsurance)
                    ).Sum(o => o.icdoPaymentHistoryDetail.amount);

                idecOldMiscellaneousDueAmount = idecChildSupportAmount + idecTaxLevyAmount + idec3rdPartyHealthInsuranceAmount;

                var lenumDueDeductions = lclbPaymentHistoryDetails.Where(o =>
                    o.icdoPaymentHistoryDetail.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPITAFPEDues));
                foreach (busPaymentHistoryDetail lbusPaymentHistoryDetail in lenumDueDeductions)
                {
                    if (lbusPaymentHistoryDetail.ibusVendor.IsNull())
                        lbusPaymentHistoryDetail.LoadVendor();
                    if (lbusPaymentHistoryDetail.ibusVendor.icdoOrganization.org_code ==
                                                                     (busGlobalFunctions.GetData1ByCodeValue(52, busConstant.NDPEAValue, iobjPassInfo).ToString()))
                    {
                        idecOldNDPEADuesAmount = idecNDPEADuesAmount + lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.amount;
                    }
                    else if (lbusPaymentHistoryDetail.ibusVendor.icdoOrganization.org_code ==
                                                                    (busGlobalFunctions.GetData1ByCodeValue(52, busConstant.AFPEValue, iobjPassInfo).ToString()))
                    {
                        idecOldAFPEDuesAmount = idecAFPEDuesAmount + lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.amount;
                    }
                }
            }
        }
        //Porperty to Load Insurance Premium Amounts
        public decimal idecOldHealthInsurancePremiumAmount { get; set; }
        public decimal idecOldLTCInsurancePremiumAmount { get; set; }
        public decimal idecOldDentalInsurancePremiumAmount { get; set; }
        public decimal idecOldVisionInsurancePremiumAmount { get; set; }
        public decimal idecOldLifePremiumAmount { get; set; }
        //Method to Correspondence Properties
        public void LoadOldInsurancePremiumAmount()
        {
            Collection<busPaymentHistoryDetail> lclbPaymentHistoryDetails = new Collection<busPaymentHistoryDetail>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                if (iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail == null)
                    iclbPaymentHistoryHeader[0].LoadPaymentHistoryDetails();
                lclbPaymentHistoryDetails = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail;

                idecOldHealthInsurancePremiumAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemHealth).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
                idecOldLTCInsurancePremiumAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemLTC)
                    .Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
                idecOldDentalInsurancePremiumAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemDental
                    ).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
                idecOldLifePremiumAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemLife
                    ).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
                idecOldVisionInsurancePremiumAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemVision
                    ).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
            }
        }
        //Property to load Fed Tax
        public decimal idecOldFedTaxAmount { get; set; }
        //Property to load State Tax
        public decimal idecOldStateTaxAmount { get; set; }
        //Load Tax amounts
        //Property to indicate tax changes

        public void LoadOldTaxAmounts()
        {
            Collection<busPaymentHistoryDetail> lclbPaymentHistoryDetails = new Collection<busPaymentHistoryDetail>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                if (iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail == null)
                    iclbPaymentHistoryHeader[0].LoadPaymentHistoryDetails();
                lclbPaymentHistoryDetails = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail;

                idecOldFedTaxAmount = lclbPaymentHistoryDetails.Where(o =>
                        o.ibusPaymentItemType.icdoPaymentItemType.ach_check_group_value == busConstant.ACHCheckGroupValueFedTax &&
                 o.ibusPaymentItemType.icdoPaymentItemType.item_usage_value == busConstant.ItemUsageMontlhyPayment &&
                 o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                  o.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_Yes).
                    Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
                idecOldStateTaxAmount = lclbPaymentHistoryDetails.Where(o =>
                      o.ibusPaymentItemType.icdoPaymentItemType.ach_check_group_value == busConstant.ACHCheckGroupValueFedTax &&
                 o.ibusPaymentItemType.icdoPaymentItemType.item_usage_value == busConstant.ItemUsageMontlhyPayment &&
                 o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                  o.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_Yes
                   ).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
            }
        }


        //Load BenefitPayment Change correspondece data

        public decimal idecYTDChildSupportAmount { get; set; }
        public decimal idecYTDTaxLevyAmount { get; set; }
        public decimal idecYTD3rdPartyHealthInsuranceAmount { get; set; }
        public decimal idecYTDAFPEDuesAmount { get; set; }
        public decimal idecYTDNDPEADuesAmount { get; set; }
        public decimal idecYTDMiscellaneousDueAmount { get; set; }
        public void LoadYTDOtherDeductionAmount()
        {
            Collection<busPaymentHistoryDetail> lclbPaymentHistoryDetails = new Collection<busPaymentHistoryDetail>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in iclbPaymentHistoryHeader.Where(o =>
                   o.icdoPaymentHistoryHeader.payment_date.Year == DateTime.Today.Year))
                {
                    if (lobjPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                        lobjPaymentHistoryHeader.LoadPaymentHistoryDetails();
                    foreach (busPaymentHistoryDetail lobjPaymentHistoryDetail in lobjPaymentHistoryHeader.iclbPaymentHistoryDetail)
                    {
                        lclbPaymentHistoryDetails.Add(lobjPaymentHistoryDetail);
                    }
                }


                if (iclbDeductions.IsNull())
                    LoadDeductions();
                idecYTDChildSupportAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.icdoPaymentHistoryDetail.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPITChildSupport))
                    .Sum(o => o.icdoPaymentHistoryDetail.amount);
                idecYTDTaxLevyAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.icdoPaymentHistoryDetail.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPITNDTaxLevy))
                    .Sum(o => o.icdoPaymentHistoryDetail.amount);
                idecYTD3rdPartyHealthInsuranceAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.icdoPaymentHistoryDetail.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPIT3rdPartyHealthInsurance)
                    ).Sum(o => o.icdoPaymentHistoryDetail.amount);

                idecYTDMiscellaneousDueAmount = idecChildSupportAmount + idecTaxLevyAmount + idec3rdPartyHealthInsuranceAmount;

                var lenumDueDeductions = lclbPaymentHistoryDetails.Where(o =>
                    o.icdoPaymentHistoryDetail.payment_item_type_id == GetPaymentItemTypeIDByItemCode(busConstant.PAPITAFPEDues));
                foreach (busPaymentHistoryDetail lbusPaymentHistoryDetail in lenumDueDeductions)
                {
                    if (lbusPaymentHistoryDetail.ibusVendor.IsNull())
                        lbusPaymentHistoryDetail.LoadVendor();
                    if (lbusPaymentHistoryDetail.ibusVendor.icdoOrganization.org_code ==
                                                                     (busGlobalFunctions.GetData1ByCodeValue(52, busConstant.NDPEAValue, iobjPassInfo).ToString()))
                    {
                        idecYTDNDPEADuesAmount = idecNDPEADuesAmount + lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.amount;
                    }
                    else if (lbusPaymentHistoryDetail.ibusVendor.icdoOrganization.org_code ==
                                                                    (busGlobalFunctions.GetData1ByCodeValue(52, busConstant.AFPEValue, iobjPassInfo).ToString()))
                    {
                        idecYTDAFPEDuesAmount = idecAFPEDuesAmount + lbusPaymentHistoryDetail.icdoPaymentHistoryDetail.amount;
                    }
                }
            }
        }
        //Porperty to Load Insurance Premium Amounts
        public decimal idecYTDHealthInsurancePremiumAmount { get; set; }
        public decimal idecYTDLTCInsurancePremiumAmount { get; set; }
        public decimal idecYTDDentalInsurancePremiumAmount { get; set; }
        public decimal idecYTDVisionInsurancePremiumAmount { get; set; }
        public decimal idecYTDLifePremiumAmount { get; set; }
        public decimal idecTotalLTDReceived { get; set; }
        //Method to Correspondence Properties
        public void LoadYTDInsurancePremiumAmount()
        {
            Collection<busPaymentHistoryDetail> lclbPaymentHistoryDetails = new Collection<busPaymentHistoryDetail>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in iclbPaymentHistoryHeader.Where(o =>
                   o.icdoPaymentHistoryHeader.payment_date.Year == DateTime.Today.Year))
                {
                    if (lobjPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                        lobjPaymentHistoryHeader.LoadPaymentHistoryDetails();
                    foreach (busPaymentHistoryDetail lobjPaymentHistoryDetail in lobjPaymentHistoryHeader.iclbPaymentHistoryDetail)
                    {
                        lclbPaymentHistoryDetails.Add(lobjPaymentHistoryDetail);
                    }
                }
                idecYTDHealthInsurancePremiumAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemHealth).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
                idecYTDLTCInsurancePremiumAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemLTC)
                    .Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
                idecYTDDentalInsurancePremiumAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemDental
                    ).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
                idecYTDLifePremiumAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemLife
                    ).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
                idecYTDVisionInsurancePremiumAmount = lclbPaymentHistoryDetails.Where(o =>
                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.InsurancePaymentItemVision
                    ).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
            }
        }
        //Property to load Fed Tax
        public decimal idecYTDFedTaxAmount { get; set; }
        //Property to load State Tax
        public decimal idecYTDStateTaxAmount { get; set; }
        //Load Tax amounts
        //Property to indicate tax changes

        public void LoadYTDTaxAmounts()
        {
            Collection<busPaymentHistoryDetail> lclbPaymentHistoryDetails = new Collection<busPaymentHistoryDetail>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in iclbPaymentHistoryHeader.Where(o =>
                    o.icdoPaymentHistoryHeader.payment_date.Year == DateTime.Today.Year))
                {
                    if (lobjPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                        lobjPaymentHistoryHeader.LoadPaymentHistoryDetails();
                    foreach (busPaymentHistoryDetail lobjPaymentHistoryDetail in lobjPaymentHistoryHeader.iclbPaymentHistoryDetail)
                    {
                        lclbPaymentHistoryDetails.Add(lobjPaymentHistoryDetail);
                    }
                }

                idecYTDFedTaxAmount = lclbPaymentHistoryDetails.Where(o =>
                        o.ibusPaymentItemType.icdoPaymentItemType.ach_check_group_value == busConstant.ACHCheckGroupValueFedTax &&
                 o.ibusPaymentItemType.icdoPaymentItemType.item_usage_value == busConstant.ItemUsageMontlhyPayment &&
                 o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                  o.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_Yes).
                    Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
                idecYTDStateTaxAmount = lclbPaymentHistoryDetails.Where(o =>
                       o.ibusPaymentItemType.icdoPaymentItemType.ach_check_group_value == busConstant.ACHCheckGroupValueFedTax &&
                 o.ibusPaymentItemType.icdoPaymentItemType.item_usage_value == busConstant.ItemUsageMontlhyPayment &&
                 o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                  o.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_Yes
                   ).Select(o => o.icdoPaymentHistoryDetail.amount).Sum();
            }
        }
        public decimal idecNontaxableYTD { get; set; }
        public decimal idecOldNonTaxable { get; set; }

        public void LoadNontaxableAmountYTD()
        {
            idecNontaxableAmount = 0.00m;
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            idecNontaxableAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_No &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)).
                Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();

            Collection<busPaymentHistoryDetail> lclbPaymentHistoryDetails = new Collection<busPaymentHistoryDetail>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in iclbPaymentHistoryHeader.Where(o =>
                    o.icdoPaymentHistoryHeader.payment_date.Year == DateTime.Today.Year))
                {
                    if (lobjPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                        lobjPaymentHistoryHeader.LoadPaymentHistoryDetails();
                    foreach (busPaymentHistoryDetail lobjPaymentHistoryDetail in lobjPaymentHistoryHeader.iclbPaymentHistoryDetail)
                    {
                        lclbPaymentHistoryDetails.Add(lobjPaymentHistoryDetail);
                    }
                }

                idecNontaxableYTD = lclbPaymentHistoryDetails.Where(o =>
                         o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_No).
                    Select(o => o.icdoPaymentHistoryDetail.amount).Sum();

                if (iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail == null)
                    iclbPaymentHistoryHeader[0].LoadPaymentHistoryDetails();

                idecOldNonTaxable = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail.Where(o =>
                    o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                    o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                    o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_No)
                    .Sum(o => o.icdoPaymentHistoryDetail.amount);
            }
        }
        public decimal idecTaxableYTD { get; set; }
        public decimal idecOldTaxable { get; set; }

        public void LoadTaxableAmountYTD()
        {
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            idecTotalTaxableAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != busConstant.RollItemForCheck &&
                 o.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_No &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)).
                Select(o => o.icdoPayeeAccountPaymentItemType.amount).Sum();

            Collection<busPaymentHistoryDetail> lclbPaymentHistoryDetails = new Collection<busPaymentHistoryDetail>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in iclbPaymentHistoryHeader.Where(o =>
                    o.icdoPaymentHistoryHeader.payment_date.Year == DateTime.Today.Year))
                {
                    if (lobjPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                        lobjPaymentHistoryHeader.LoadPaymentHistoryDetails();
                    foreach (busPaymentHistoryDetail lobjPaymentHistoryDetail in lobjPaymentHistoryHeader.iclbPaymentHistoryDetail)
                    {
                        lclbPaymentHistoryDetails.Add(lobjPaymentHistoryDetail);
                    }
                }

                idecTaxableYTD = lclbPaymentHistoryDetails.Where(o =>
                        o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.allow_rollover_code_value != busConstant.RollItemForCheck &&
                 o.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_No).
                    Select(o => o.icdoPaymentHistoryDetail.amount).Sum();

                if (iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail == null)
                    iclbPaymentHistoryHeader[0].LoadPaymentHistoryDetails();

                idecOldTaxable = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail.Where(o =>
                       o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                o.ibusPaymentItemType.icdoPaymentItemType.payment_1099r_flag == busConstant.Flag_Yes &&
                o.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes &&
                 o.ibusPaymentItemType.icdoPaymentItemType.vendor_flag == busConstant.Flag_No)
                    .Sum(o => o.icdoPaymentHistoryDetail.amount);
            }
        }

        public decimal idecNetAmountYTD { get; set; }
        public decimal idecOldNetAmount { get; set; }

        public void LoadNetAmountYTD()
        {
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            idecBenefitAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)).
                Select(o => o.icdoPayeeAccountPaymentItemType.amount * o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();

            Collection<busPaymentHistoryDetail> lclbPaymentHistoryDetails = new Collection<busPaymentHistoryDetail>();
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in iclbPaymentHistoryHeader.Where(o =>
                    o.icdoPaymentHistoryHeader.payment_date.Year == DateTime.Today.Year))
                {
                    if (lobjPaymentHistoryHeader.iclbPaymentHistoryDetail == null)
                        lobjPaymentHistoryHeader.LoadPaymentHistoryDetails();
                    foreach (busPaymentHistoryDetail lobjPaymentHistoryDetail in lobjPaymentHistoryHeader.iclbPaymentHistoryDetail)
                    {
                        lclbPaymentHistoryDetails.Add(lobjPaymentHistoryDetail);
                    }
                }

                idecNetAmountYTD = lclbPaymentHistoryDetails.
                    Select(o => o.icdoPaymentHistoryDetail.amount * o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();

                if (iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail == null)
                    iclbPaymentHistoryHeader[0].LoadPaymentHistoryDetails();

                idecOldNetAmount = iclbPaymentHistoryHeader[0].iclbPaymentHistoryDetail.
                    Sum(o => o.icdoPaymentHistoryDetail.amount * o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction);
            }
        }
        # endregion

        #region UCS - 091
        //Propety to contain all 1099r records for a payee account
        public Collection<busPayment1099r> iclbPayment1099r { get; set; }
        /// <summary>
        /// Method to load Payment 1099r records for payee account
        /// </summary>
        public void LoadPayment1099r()
        {
            iclbPayment1099r = new Collection<busPayment1099r>();
            DataTable ldtPayment1099r = Select<cdoPayment1099r>(new string[1] { enmPayment1099r.payee_account_id.ToString() },
                                                                new object[1] { icdoPayeeAccount.payee_account_id },
                                                                null, "tax_year desc");
            if (ldtPayment1099r.Rows.Count > 0)
                iclbPayment1099r = GetCollection<busPayment1099r>(ldtPayment1099r, "icdoPayment1099r");
        }
        #endregion

        # region UCS - 54
        public Collection<busPaymentRecovery> iclbPaymentRecovery { get; set; }
        //load all the recovery for this payee account id
        public void LoadPaymentRecovery()
        {
            if (iclbPaymentRecovery.IsNull())
                iclbPaymentRecovery = new Collection<busPaymentRecovery>();

            DataTable ldtbRecoveryList = Select<cdoPaymentRecovery>(new string[1] { "payee_account_id" }, new object[1] { icdoPayeeAccount.payee_account_id }, null, null);
            iclbPaymentRecovery = GetCollection<busPaymentRecovery>(ldtbRecoveryList, "icdoPaymentRecovery");
        }
        # endregion

        #region UCS - 079 corresponce

        public DateTime idtDateafter60daysfromToday
        {
            get
            {
                return DateTime.Today.AddDays(60);
            }
        }
        public DateTime idtNextPaymentDateafter60daysfromToday
        {
            get
            {
                return new DateTime(idtDateafter60daysfromToday.AddMonths(1).Year, idtDateafter60daysfromToday.AddMonths(1).Month, 1);
            }
        }
        public decimal idecRecoveryAmount { get; set; }

        public decimal idecGrossReductionAmount { get; set; }

        public DateTime idtRecoveryEffectiveDate { get; set; }

        public int iintCurrenYear
        {
            get
            {
                return DateTime.Today.Year;
            }
        }
        public int iintNumberOfMonthsToRecover { get; set; }

        public decimal idecTotalMonthlyUnderPaymentAmount { get; set; }
        public decimal iintNoOfUnderPaymentMonths { get; set; }
        public decimal idecMonthlyUnderPaymentAMount { get; set; }
        public string istrInterestAppliedForUnderpayment { get; set; }
        public void LoadUnderPaymentCorrespondenceProperties()
        {
            if (iclbRetroPayment == null)
                LoadRetroPayment();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();

            busPayeeAccountRetroPayment lobjRetroPayment = iclbRetroPayment.Where(o =>
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountRetroPayment.start_date, o.icdoPayeeAccountRetroPayment.end_date)).FirstOrDefault();
            if (lobjRetroPayment != null)
            {
                iintNoOfUnderPaymentMonths =
                 busGlobalFunctions.DateDiffByMonth(lobjRetroPayment.icdoPayeeAccountRetroPayment.effective_start_date, lobjRetroPayment.icdoPayeeAccountRetroPayment.effective_end_date);
                idecTotalMonthlyUnderPaymentAmount = lobjRetroPayment.icdoPayeeAccountRetroPayment.net_payment_amount;
                if (iclbRetroItemType == null)
                    LoadRetroItemType(lobjRetroPayment.icdoPayeeAccountRetroPayment.retro_payment_type_value);
                if (iclbPayeeAccountPaymentItemType == null)
                    LoadPayeeAccountPaymentItemType();

                idecMonthlyUnderPaymentAMount = idecGrossBenfitAmount;

                //+= iclbPayeeAccountPaymentItemType.Where(o =>
                //busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date) &&
                //   iclbRetroItemType.Where(m => m.icdoRetroItemType.to_item_type == o.ibusPaymentItemType.icdoPaymentItemType.item_type_code).Count() > 0).
                //   Sum(o => o.icdoPayeeAccountPaymentItemType.amount);
            }
        }

        #region ucs-079 - Joint surviver pop up correspondence properties

        public DateTime idt10DaysPriorNextBenefitPaymentDate
        {
            get
            {
                if (idtNextBenefitPaymentDate == DateTime.MinValue)
                    LoadNexBenefitPaymentDate();
                return idtNextBenefitPaymentDate.AddDays(-10);
            }
        }
        public string istrIsNoTaxOptionSelected { get; set; }
        public string istrIsSpouseBeneficiary { get; set; }
        public string istrIsSpouseDepedentOnHealthPlan { get; set; }
        public string istrIsSpouseDepedentOnDentalPlan { get; set; }
        public string istrIsSpouseDepedentOnVisionPlan { get; set; }
        public string istrIsSpouseBeneficiaryOnLifePlan { get; set; }
        public string istrIsSpouseDeathAndMemberDependentAndSupplemental { get; set; }
        public decimal idecSpouseSupplementalCoverageAmount { get; set; }
        public decimal idecDependentSupplementalCoverageAmount { get; set; }
        public string istrAssetsWithProvider { get; set; }
        public string istrSpouseEnrolledinLTC { get; set; }
        public string isrtProviderOrgName { get; set; }
        public string istrIsRHICoptionSelected { get; set; }
        private void LoadJointSurvivorCorProperties()
        {
            if (ibusBenefitAccount == null)
                LoadBenfitAccount();
            if (ibusBenefitAccount.icdoBenefitAccount.rhic_benefit_option_value != null)
                istrIsRHICoptionSelected = busConstant.Flag_Yes;
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            if (iclbPayeeAccountFedTaxWithHolding == null)
                LoadFedTaxWithHoldingInfo();
            if (iclbPayeeAccountStateTaxWithHolding == null)
                LoadStateTaxWithHoldingInfo();
            if (iclbPayeeAccountStateTaxWithHolding.Where(o =>
                (o.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoStateTax || o.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoStateTaxWithheld) &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountTaxWithholding.start_date, o.icdoPayeeAccountTaxWithholding.end_date)).Count() > 0 &&
                iclbPayeeAccountFedTaxWithHolding.Where(o =>
                (o.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoFedTax || o.icdoPayeeAccountTaxWithholding.tax_option_value == busConstant.NoFederalTaxWithheld) &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountTaxWithholding.start_date, o.icdoPayeeAccountTaxWithholding.end_date)).Count() > 0)
            {
                istrIsNoTaxOptionSelected = busConstant.Flag_Yes;
            }

            if (ibusJointAnnuitant == null)
                LoadJointAnnuitant();
            if (ibusJointAnnuitant.iclbPayeeAccount == null)
                ibusJointAnnuitant.LoadPayeeAccount();
            if (ibusJointAnnuitant.iclbPayeeAccount.Where(o => o.icdoPayeeAccount.account_relation_value == busConstant.AccountRelationshipBeneficiary).Count() > 0)
                istrIsSpouseBeneficiary = busConstant.Flag_Yes;

            if (ibusPayee == null)
                LoadPayee();
            int lintPayeeHealthAccountID = 0;
            if (ibusPayee != null)
                lintPayeeHealthAccountID = ibusPayee.LoadActivePersonAccountByPlan(busConstant.PlanIdGroupHealth).icdoPersonAccount.person_account_id;
            if (ibusPayee.iclbPersonDependent == null)
                ibusPayee.LoadDependent();
            if (ibusPayee.iclbPersonDependent.Where(o => o.icdoPersonDependent.dependent_perslink_id == ibusJointAnnuitant.icdoPerson.person_id &&
                o.ibusPeronAccountDependent.icdoPersonAccountDependent.person_account_id == lintPayeeHealthAccountID).Count() > 0)
            {
                istrIsSpouseDepedentOnHealthPlan = busConstant.Flag_Yes;
            }

            if (ibusPayee == null)
                LoadPayee();
            int lintPayeeLifeAccountID = 0;
            if (ibusPayee != null)
                lintPayeeLifeAccountID = ibusPayee.LoadActivePersonAccountByPlan(busConstant.PlanIdGroupLife).icdoPersonAccount.person_account_id;
            if (ibusPayee.iclbPersonBeneficiary == null)
                ibusPayee.LoadBeneficiary();
            if (ibusPayee.iclbPersonBeneficiary.Where(o => o.icdoPersonBeneficiary.beneficiary_person_id == ibusJointAnnuitant.icdoPerson.person_id &&
                o.ibusPersonAccountBeneficiary.icdoPersonAccountBeneficiary.person_account_id == lintPayeeLifeAccountID).Count() > 0)
            {
                istrIsSpouseBeneficiaryOnLifePlan = busConstant.Flag_Yes;
            }

            busPersonAccountLife lobjPersonAccountLife = new busPersonAccountLife
            {
                icdoPersonAccount = new cdoPersonAccount(),
                icdoPersonAccountLife = new cdoPersonAccountLife()
            };
            lobjPersonAccountLife.FindPersonAccountLife(lintPayeeLifeAccountID);
            lobjPersonAccountLife.FindPersonAccount(lintPayeeLifeAccountID);
            lobjPersonAccountLife.LoadLifeOptionData();

            if (lobjPersonAccountLife.iclbLifeOption.Where(o =>
                o.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental &&
                o.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue).Count() > 0)
            {
                idecSpouseSupplementalCoverageAmount = lobjPersonAccountLife.iclbLifeOption.Where(o =>
                o.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_SpouseSupplemental &&
                o.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue).Select(o => o.icdoPersonAccountLifeOption.coverage_amount).Sum();

                idecDependentSupplementalCoverageAmount = lobjPersonAccountLife.iclbLifeOption.Where(o =>
                o.icdoPersonAccountLifeOption.level_of_coverage_value == busConstant.LevelofCoverage_DependentSupplemental &&
                o.icdoPersonAccountLifeOption.effective_start_date != DateTime.MinValue).Select(o => o.icdoPersonAccountLifeOption.coverage_amount).Sum();
            }

            if (ibusPayee == null)
                LoadPayee();
            int lintPayeeDentalAccountID = 0;
            if (ibusPayee != null)
                lintPayeeDentalAccountID = ibusPayee.LoadActivePersonAccountByPlan(busConstant.PlanIdDental).icdoPersonAccount.person_account_id;

            if (ibusPayee.iclbPersonDependent == null)
                ibusPayee.LoadDependent();
            if (ibusPayee.iclbPersonDependent.Where(o => o.icdoPersonDependent.dependent_perslink_id == ibusJointAnnuitant.icdoPerson.person_id &&
                o.ibusPeronAccountDependent.icdoPersonAccountDependent.person_account_id == lintPayeeDentalAccountID).Count() > 0)
            {
                istrIsSpouseDepedentOnDentalPlan = busConstant.Flag_Yes;
            }

            if (ibusPayee == null)
                LoadPayee();
            int lintPayeVisionAccountID = 0;
            if (ibusPayee != null)
                lintPayeVisionAccountID = ibusPayee.LoadActivePersonAccountByPlan(busConstant.PlanIdVision).icdoPersonAccount.person_account_id;
            if (ibusPayee.iclbPersonDependent == null)
                ibusPayee.LoadDependent();
            if (ibusPayee.iclbPersonDependent.Where(o => o.icdoPersonDependent.dependent_perslink_id == ibusJointAnnuitant.icdoPerson.person_id &&
                o.ibusPeronAccountDependent.icdoPersonAccountDependent.person_account_id == lintPayeVisionAccountID).Count() > 0)
            {
                istrIsSpouseDepedentOnVisionPlan = busConstant.Flag_Yes;
            }
            if (ibusJointAnnuitant.icolPersonAccount == null)
                ibusJointAnnuitant.LoadPersonAccount();
            if (ibusJointAnnuitant.icolPersonAccount.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdLTC).Count() > 0)
                istrSpouseEnrolledinLTC = busConstant.Flag_Yes;
            if (ibusPayee.icolPersonAccount == null)
                ibusPayee.LoadPersonAccount();
            if (ibusPayee.icolPersonAccount.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation).Count() > 0)
            {
                busPersonAccountDeferredComp lobjPersonAccountDeferredComp = new busPersonAccountDeferredComp();

                lobjPersonAccountDeferredComp.FindPersonAccountDeferredComp(ibusPayee.icolPersonAccount.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation).
                    Select(o => o.icdoPersonAccount.person_account_id).FirstOrDefault());
                lobjPersonAccountDeferredComp.LoadActivePersonAccountProviders();

                busPersonAccountDeferredCompProvider lobjDCompPro = lobjPersonAccountDeferredComp.icolPersonAccountDeferredCompProvider.Where(o =>
                    o.icdoPersonAccountDeferredCompProvider.assets_with_provider_value == busConstant.AssetsWithProviderBeneficiary).FirstOrDefault();
                if (lobjDCompPro != null)
                {
                    istrAssetsWithProvider = busConstant.Flag_Yes;
                    lobjDCompPro.LoadProviderOrgPlan();
                    lobjDCompPro.ibusProviderOrgPlan.LoadOrganization();
                    isrtProviderOrgName = lobjDCompPro.ibusProviderOrgPlan.ibusOrganization.icdoOrganization.org_name;
                }
            }
            LoadInsurancePremiumAmount();
        }

        #endregion

        # endregion

        #region APP-7150

        public override busBase GetCorOrganization()
        {
            if (ibusBenefitAccount.IsNull())
                LoadBenfitAccount();
            busOrganization lobjOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
            lobjOrganization.FindOrganization(ibusBenefitAccount.icdoBenefitAccount.retirement_org_id);
            return lobjOrganization;
        }

        public decimal idecPLSOTaxable { get; set; }
        public void LoadPLSOTaxable()
        {
            idecPLSOTaxable = 0M;
            if (iclbPayeeAccountPaymentItemType.IsNull())
                LoadPayeeAccountPaymentItemType();
            idecPLSOTaxable = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSOTaxableAmount &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Select(o => o.icdoPayeeAccountPaymentItemType.amount * o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();
        }

        public decimal idecPLSONonTaxable { get; set; }
        public void LoadPLSONonTaxable()
        {
            idecPLSONonTaxable = 0M;
            if (iclbPayeeAccountPaymentItemType.IsNull())
                LoadPayeeAccountPaymentItemType();
            idecPLSONonTaxable = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSONonTaxableAmount &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Select(o => o.icdoPayeeAccountPaymentItemType.amount * o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();
        }

        public decimal idecPLSOFederalTaxAmount { get; set; }
        public void LoadPLSOFederalTaxAmount()
        {
            idecPLSOFederalTaxAmount = 0M;
            if (iclbPayeeAccountPaymentItemType.IsNull())
                LoadPayeeAccountPaymentItemType();
            idecPLSOFederalTaxAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSOFederalTaxAmount &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Select(o => o.icdoPayeeAccountPaymentItemType.amount * o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();
        }

        public decimal idecPLSOStateTaxAmount { get; set; }
        public void LoadPLSOStateTaxAmount()
        {
            idecPLSOStateTaxAmount = 0M;
            if (iclbPayeeAccountPaymentItemType.IsNull())
                LoadPayeeAccountPaymentItemType();
            idecPLSOStateTaxAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSOStateTaxAmount &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Select(o => o.icdoPayeeAccountPaymentItemType.amount * o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();
        }

        public decimal idecPLSORolloverAmount { get; set; }
        public void LoadPLSORolloverAmount()
        {
            idecPLSORolloverAmount = 0M;
            if (iclbPayeeAccountPaymentItemType.IsNull())
                LoadPayeeAccountPaymentItemType();

            idecPLSORolloverAmount = iclbPayeeAccountPaymentItemType.Where(o =>
                (o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSOTaxableRollover ||
                o.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITPLSONonTaxableRollover) &&
                busGlobalFunctions.CheckDateOverlapping(idtNextBenefitPaymentDate, o.icdoPayeeAccountPaymentItemType.start_date, o.icdoPayeeAccountPaymentItemType.end_date)
                ).Select(o => o.icdoPayeeAccountPaymentItemType.amount * o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction).Sum();
        }

        public decimal idecNetPLSO { get; set; }
        public void LoadNetPLSO()
        {
            idecNetPLSO = 0M;
            LoadPLSOTaxable();
            LoadPLSONonTaxable();
            LoadPLSOFederalTaxAmount();
            LoadPLSOStateTaxAmount();
            LoadPLSORolloverAmount();

            idecNetPLSO = idecPLSOTaxable + idecPLSONonTaxable + idecPLSOFederalTaxAmount + idecPLSOStateTaxAmount;
            if (idecNetPLSO < 0M) idecNetPLSO = 0M;
        }


        public bool IsPLSORollovered
        {
            get
            {
                LoadPLSORolloverAmount();
                if (idecPLSORolloverAmount > 0)
                    return true;
                return false;
            }
        }
        public int iintWssBenAppId { get; set; } //PIR 18493
        #endregion

        # region UCS 84
        public void GetCOLAOrAdhocAmount(int aintRequestId, ref decimal adecReturnAmount, int aintPaymentItemTypeID, busPayeeAccountPaymentItemType aobjPAPIT)
        {
            busPostRetirementIncreaseBatchRequest lobjRequest = new busPostRetirementIncreaseBatchRequest();
            if (lobjRequest.FindPostRetirementIncreaseBatchRequest(aintRequestId))
            {
                //this is to check the eligibility whether the payee account is allowed to take COLA amount
                DataTable ldtbPayeeListTobeProcessed = busBase.Select("cdoPostRetirementIncreaseBatchRequest.LoadCOLAAdhocRequestForPayeeAccount",
                                               new object[3] {lobjRequest.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                        lobjRequest.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_batch_request_id,
                                                        icdoPayeeAccount.payee_account_id});
                if (ldtbPayeeListTobeProcessed.Rows.Count > 0)
                {
                    //For COLA
                    if (lobjRequest.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value.Equals(busConstant.PostRetirementIncreaseTypeValueCOLA))
                    {
                        adecReturnAmount = GetAmountForCOLAOrAdhoc(aintPaymentItemTypeID,
                                                                lobjRequest.icdoPostRetirementIncreaseBatchRequest.base_date,
                                                                lobjRequest.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount,
                                                                lobjRequest.icdoPostRetirementIncreaseBatchRequest.increase_percentage,
                                                                lobjRequest.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                                true, aobjPAPIT);
                    }
                    //For Ad hoc
                    else if (lobjRequest.icdoPostRetirementIncreaseBatchRequest.post_retirement_increase_type_value.Equals(busConstant.PostRetirementIncreaseTypeValueAdHoc))
                    {
                        adecReturnAmount = GetAmountForCOLAOrAdhoc(aintPaymentItemTypeID,
                                                                   lobjRequest.icdoPostRetirementIncreaseBatchRequest.base_date,
                                                                   lobjRequest.icdoPostRetirementIncreaseBatchRequest.increase_flat_amount,
                                                                   lobjRequest.icdoPostRetirementIncreaseBatchRequest.increase_percentage,
                                                                   lobjRequest.icdoPostRetirementIncreaseBatchRequest.effective_date,
                                                                   false, aobjPAPIT);
                    }
                }
            }
        }

        //NOTE : Moved updation and insertion of PAPIT into payee account status screen since this function need to be reused
        private decimal GetAmountForCOLAOrAdhoc(int aintPaymentItemTypeID, DateTime adtBaseDate, decimal adecIncreaseFlatAmount,
                                                                                    decimal adecIncreasePercentage, DateTime adtEffectiveDate, bool ablnIsForCOLA,
                                                                                    busPayeeAccountPaymentItemType aobjPAPIT)
        {
            decimal ldecbaseAmount = 0.00M;
            decimal ldecMonthlyIncrease = 0.00M;
            decimal ldecOldAmount = 0.00M;
            decimal ldecAdjustedAmount = 0.00M;
            int lintNumberOfMonths = 0;
            int lintMonthsDiff = 0;
            string lsrtItemType = string.Empty;
            int lintBatchScheduleId = 0;
            if (ablnIsForCOLA)
            {
                lsrtItemType = busConstant.PAPITCOLABase;
                lintBatchScheduleId = busConstant.BatchScheduleIDProcessCOLABatch;
            }
            else
            {
                lsrtItemType = busConstant.PAPITAdhoc;
                lintBatchScheduleId = busConstant.BatchScheduleIDProcessAdhocBatch;
            }

            if (ibusPlan.IsNull())
                LoadPlan();
            if (ibusApplication.IsNull())
                LoadApplication();

            //load all Payment item types     
            if (iclbPayeeAccountPaymentItemType.IsNull())
                LoadPayeeAccountPaymentItemType();

            //load Base amount
            LoadGrossAmount(adtBaseDate);
            ldecbaseAmount = idecGrossAmount;
            //UAT PIR: 2416. Checked with COLA batch and the Paid up annuity is added in teh base amount and not directly in PAPIT
            if (ibusApplication.icdoBenefitApplication.plan_id == busConstant.PlanIdJobService)
            {
                ldecbaseAmount += ibusApplication.icdoBenefitApplication.paid_up_annuity_amount; // UAT PIR ID 1341;
            }

            //load COLA/Adhoc increase
            if (adecIncreaseFlatAmount > 0)
            {
                ldecMonthlyIncrease = adecIncreaseFlatAmount;
            }
            else
            {
                ldecMonthlyIncrease = (ldecbaseAmount * adecIncreasePercentage) / 100;
                ldecMonthlyIncrease = busGlobalFunctions.RoundToPenny(ldecMonthlyIncrease);
            }

            //only for COLA
            if (ablnIsForCOLA)
            {
                //now get adjusted COLA                   
                lintNumberOfMonths = busGlobalFunctions.DateDiffByMonth(icdoPayeeAccount.benefit_begin_date, adtEffectiveDate);

                if (lintNumberOfMonths > 1)
                    lintNumberOfMonths = lintNumberOfMonths - 1;

                lintMonthsDiff = lintNumberOfMonths;

                /* UAT PIR: 2416 COLA Proration logic provided by Leon
                * 1)Benefit Type is Retirement or Disability:     if the numbers of Months between Payee Account Benefit begin date and COLA Effective date is less than 12.
                  2)Benefit Type: Pre Retirement :  if the number of months between Payee Account Benefit Begin date and COLA Effective date is less than 12. 
                  3)Benefit Type: Post Retirement:  :   if the numbers of Member payments plus joint annuitant payments and the COLA Effective date are less than 12.             
                */
                //check if the member dies within one year of retirement if so then joint annuitant pension benefits will be calculated
                //else member payee account will continue.
                bool blnAllowProration = false;
                if (icdoPayeeAccount.benefit_account_type_value == busConstant.BenefitAccountSubTypePostRetDeath)
                {
                    int lintProrationMonths = 0;
                    if (ibusApplication.ibusOriginatingPayeeAccount.IsNull())
                        ibusApplication.LoadOriginatingPayeeAccount();

                    lintProrationMonths = ibusApplication.ibusOriginatingPayeeAccount.GetAlreadyPaidNumberofPayments();

                    //lintProrationMonths = busGlobalFunctions.DateDiffByMonth(ibusApplication.ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_begin_date,
                    //ibusApplication.ibusOriginatingPayeeAccount.icdoPayeeAccount.benefit_end_date);
                    if ((lintProrationMonths + lintNumberOfMonths) / 12 < 1)
                    {
                        blnAllowProration = true;
                        lintMonthsDiff = lintProrationMonths + lintNumberOfMonths;
                    }
                }
                else if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeDisability) ||
                        (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRetirement) ||
                        (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath))
                {
                    if (lintNumberOfMonths / 12 < 1)
                    {
                        blnAllowProration = true;
                    }
                }


                if (blnAllowProration)
                {
                    ldecAdjustedAmount = ((ldecMonthlyIncrease * lintMonthsDiff) / 12);
                }
                else
                {
                    ldecAdjustedAmount = ldecMonthlyIncrease;
                }
            }
            else
                ldecAdjustedAmount = ldecMonthlyIncrease;

            bool lblnContinue = false;

            if (((ldecAdjustedAmount > 0.00M)
                && (ablnIsForCOLA))
                || (!ablnIsForCOLA))
            {
                lblnContinue = true;
            }
            if (lblnContinue)
            {
                var lPaymentItemTypeList = iclbPayeeAccountPaymentItemType.Where(lobjPayItemType => lobjPayItemType.ibusPaymentItemType.icdoPaymentItemType.item_type_code == lsrtItemType
                     && busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                                                                lobjPayItemType.icdoPayeeAccountPaymentItemType.start_date, lobjPayItemType.icdoPayeeAccountPaymentItemType.end_date));

                foreach (var lobjPaymentItemType in lPaymentItemTypeList)
                {
                    //updation of papit moved to payee account status
                    ldecOldAmount += lobjPaymentItemType.icdoPayeeAccountPaymentItemType.amount;
                }

                aintPaymentItemTypeID = GetPaymentItemTypeIDByItemCode(lsrtItemType);

                //when amount is there then only insert in PAPIT
                if (ldecAdjustedAmount + ldecOldAmount > 0)
                {
                    //insert the new cola amount/Adhoc as new payment type record for this payee account
                    if (aobjPAPIT == null)
                        aobjPAPIT = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
                    aobjPAPIT.icdoPayeeAccountPaymentItemType.payee_account_id = icdoPayeeAccount.payee_account_id;
                    aobjPAPIT.icdoPayeeAccountPaymentItemType.payment_item_type_id = aintPaymentItemTypeID;
                    aobjPAPIT.icdoPayeeAccountPaymentItemType.start_date = adtEffectiveDate;
                    aobjPAPIT.icdoPayeeAccountPaymentItemType.amount = ldecAdjustedAmount + ldecOldAmount;
                    aobjPAPIT.icdoPayeeAccountPaymentItemType.batch_schedule_id = lintBatchScheduleId;
                    //moved the PAPIT insertion part into payee account status object, since for creation of overpayment or underpayment need to reuse this method
                    iintBatchScheudleID = lintBatchScheduleId;
                    idtStatusEffectiveDate = adtEffectiveDate;
                    iblnIsColaOrAdhocIncreaseTaxCalculation = true;

                }
            }
            return ldecAdjustedAmount + ldecOldAmount + ibusApplication.icdoBenefitApplication.paid_up_annuity_amount; // UAT PIR ID 1341
        }

        # endregion

        #region UCS 55 RHIC Addendum
        public Collection<busBenefitRhicCombineDetail> iclbBenefitRHICCombineDetail { get; set; }
        public void LoadBenefitRhicCombineDetail()
        {
            if (iclbBenefitRHICCombineDetail == null)
                iclbBenefitRHICCombineDetail = new Collection<busBenefitRhicCombineDetail>();
            DataTable ldtbList = Select<cdoBenefitRhicCombineDetail>(
                  new string[1] { "donar_payee_account_id" },
                  new object[1] { icdoPayeeAccount.payee_account_id }, null, null);
            iclbBenefitRHICCombineDetail = GetCollection<busBenefitRhicCombineDetail>(ldtbList, "icdoBenefitRhicCombineDetail");
        }

        public bool IsPaymentHistoryRecordFound()
        {
            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();

            if (iclbPaymentHistoryHeader.Count > 0)
                return true;

            return false;
        }

        public DateTime GetRuleOf85Date()
        {
            DateTime ldtMemberAttainedRuleOf80Or85 = DateTime.MinValue;

            if (ibusPayee == null)
                LoadPayee();

            if (ibusPlan == null)
                LoadPlan();

            if (ibusApplication == null)
                LoadApplication();

            if (ibusApplication.ibusPersonAccount == null)
                ibusApplication.LoadPersonAccount();

            DataTable ldtDBCacheBenefitProvision = iobjPassInfo.isrvDBCache.GetCacheData("sgt_benefit_provision_eligibility", null);

            if (ldtDBCacheBenefitProvision.Rows.Count > 0)
            {
                var ldrRow = ldtDBCacheBenefitProvision.AsEnumerable().Where(dr => dr.Field<int>("benefit_provision_id") == ibusPlan.icdoPlan.benefit_provision_id
                    && dr.Field<string>("benefit_account_type_value") == icdoPayeeAccount.benefit_account_type_value
                    && dr.Field<string>("ELIGIBILITY_TYPE_VALUE") == busConstant.BenefitProvisionEligibilityNormal
                    && dr.Field<DateTime>("Effective_date") <= ibusApplication.icdoBenefitApplication.termination_date)
                    .OrderByDescending(i => i.Field<DateTime>("Effective_date")).FirstOrDefault();

                if (ldrRow != null)
                {
                    if (!Convert.IsDBNull(ldrRow["age_plus_service"]))
                    {
                        decimal ldecTargetTVCS = Convert.ToDecimal(ldrRow["age_plus_service"]) * 12;

                        ldtMemberAttainedRuleOf80Or85 = busPersonBase.GetNormalRetirementDateByRuleOf80Or85(
                            ibusPayee.icdoPerson.date_of_birth, ldecTargetTVCS,
                            ibusApplication.icdoBenefitApplication.termination_date,
                            ibusApplication.ibusPersonAccount.icdoPersonAccount.person_account_id, false, 0.00M, iobjPassInfo);
                    }
                }
            }
            return ldtMemberAttainedRuleOf80Or85;
        }

        //Latest Valid Approved Combine Record for this Donor
        public busBenefitRhicCombine ibusLatestBenefitRhicCombine { get; set; }

        public void LoadLatestBenefitRhicCombine(bool ablnCombineFlag = false)
        {
            //Load the Old Rhic Combine Record for this Donor Payee Account
            if (iclbBenefitRHICCombineDetail == null)
                LoadBenefitRhicCombineDetail();

            if (iclbBenefitRHICCombineDetail.Count == 0) //Conversion Record, So take it from Payee Perslink ID
            {
                if (ibusPayee == null)
                    LoadPayee();

                ibusPayee.LoadLatestBenefitRhicCombine();
                ibusLatestBenefitRhicCombine = ibusPayee.ibusLatestBenefitRhicCombine;
            }
            else
            {
                foreach (busBenefitRhicCombineDetail lbusBenRhicComDetail in iclbBenefitRHICCombineDetail)
                {
                    if (lbusBenRhicComDetail.ibusBenefitRHICCombine == null)
                        lbusBenRhicComDetail.LoadRHICCombine();
                }
                busBenefitRhicCombineDetail lbusResultBenRhicComDetail = null;
                if (!ablnCombineFlag)
                {

                    //Load the Valid Approved Record for Donor Payee Account
                    lbusResultBenRhicComDetail = iclbBenefitRHICCombineDetail.Where(i => i.ibusBenefitRHICCombine.icdoBenefitRhicCombine.status_value == busConstant.RHICStatusValid &&
                                                                            i.ibusBenefitRHICCombine.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusApproved)
                                                                            .OrderByDescending(i => i.ibusBenefitRHICCombine.icdoBenefitRhicCombine.end_date_no_null)
                                                                            .FirstOrDefault();
                }
                else
                {
                    //PIR 14346 - Loading the latest combined record with this donor detail combine flag as YES
                    lbusResultBenRhicComDetail = iclbBenefitRHICCombineDetail.Where(i => i.ibusBenefitRHICCombine.icdoBenefitRhicCombine.status_value == busConstant.RHICStatusValid &&
                                                                            i.ibusBenefitRHICCombine.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusApproved &&
                                                                            i.icdoBenefitRhicCombineDetail.combine_flag == busConstant.Flag_Yes)
                                                                            .OrderByDescending(i => i.ibusBenefitRHICCombine.icdoBenefitRhicCombine.end_date_no_null)
                                                                            .FirstOrDefault();
                }
                if (lbusResultBenRhicComDetail != null)
                    ibusLatestBenefitRhicCombine = lbusResultBenRhicComDetail.ibusBenefitRHICCombine;
            }

        }
        #endregion

        #region UCS-054 Payee Death Letter

        public bool IsBenefitOptionSingleLife
        {
            get
            {
                if (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionSingleLife)
                    return true;
                return false;
            }
        }

        public bool IsBenefitOptionStraightLife
        {
            get
            {
                if (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionStraightLife)
                    return true;
                return false;
            }
        }

        public bool IsBenefitOptionJandS
        {
            get
            {
                if ((icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption100PercentJS) ||
                    (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption50PercentJS) ||
                    (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption55Percent) ||
                    (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption75Percent))
                    return true;
                return false;
            }
        }

        public bool IsTermCertainOption
        {
            get
            {
                if ((icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption5YearTermLife) ||
                    (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption10YearCertain) ||
                    (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption15YearCertain) ||
                    (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOption20YearCertain))
                    return true;
                return false;
            }
        }

        public bool IsTermCertainEndDatePastDate
        {
            get
            {
                if (icdoPayeeAccount.term_certain_end_date < DateTime.Now)
                    return true;
                return false;
            }
        }

        public string IsBenefitPayable
        {
            get
            {
                if (idecMinimumGuaranteeAmount == 0M) LoadMinimumGuaranteeAmount();
                if (idecMinimumGuaranteeAmount > 0M) return busConstant.Flag_Yes;
                return busConstant.Flag_No;
            }
        }

        public busPaymentHistoryHeader ibusLatestPaymentHistory { get; set; }

        public void LoadLatestPaymentHistory()
        {
            if (ibusLatestPaymentHistory.IsNull()) ibusLatestPaymentHistory = new busPaymentHistoryHeader { icdoPaymentHistoryHeader = new cdoPaymentHistoryHeader() };
            if (iclbPaymentHistoryHeader.IsNull()) LoadPaymentHistoryHeader();

            if (iclbPaymentHistoryHeader.Count > 0)
                ibusLatestPaymentHistory = iclbPaymentHistoryHeader[0];

            ibusLatestPaymentHistory.CalculateAmounts();
        }

        public busBenefitProvisionBenefitOption ibusBenefitProvisionBenefitOption { get; set; }

        public void LoadBenefitProvisionBenefitOption(string astrBenefitTypeValue, string astrBenefitOptionValue,
                                                                    DateTime adteRetirementDate, int aintBenefitProvisionID)
        {
            if (ibusBenefitProvisionBenefitOption == null)
                ibusBenefitProvisionBenefitOption = new busBenefitProvisionBenefitOption { icdoBenefitProvisionBenefitOption = new cdoBenefitProvisionBenefitOption() };
            if (adteRetirementDate != DateTime.MinValue)
            {
                DataTable ldbtResult = SelectWithOperator<cdoBenefitProvisionBenefitOption>(
                                new string[4] { "BENEFIT_ACCOUNT_TYPE_VALUE", "BENEFIT_OPTION_VALUE", "EFFECTIVE_DATE", "BENEFIT_PROVISION_ID" },
                                new string[4] { "=", "=", "<=", "=" },
                                new object[4] { astrBenefitTypeValue, astrBenefitOptionValue, adteRetirementDate, aintBenefitProvisionID }, "EFFECTIVE_DATE DESC");
                if (ldbtResult.Rows.Count > 0)
                    ibusBenefitProvisionBenefitOption.icdoBenefitProvisionBenefitOption.LoadData(ldbtResult.Rows[0]);
            }
        }

        #endregion

        /// <summary>
        /// Method to get total amount in papit of insurance recv. items
        /// </summary>
        /// <returns></returns>
        internal decimal LoadTotalPAPITAmountForInsuranceRecv()
        {
            decimal ldecAmount = 0.0M;
            DataTable ldtCodeValue = iobjPassInfo.isrvDBCache.GetCodeValues(1711);
            foreach (DataRow dr in ldtCodeValue.Rows)
            {
                if (dr["data1"] != DBNull.Value)
                {
                    busPayeeAccountPaymentItemType lobjPAPIT = GetLatestPayeeAccountPaymentItemType(dr["data1"].ToString());
                    if (lobjPAPIT != null)
                        ldecAmount += lobjPAPIT.icdoPayeeAccountPaymentItemType.amount;
                }
            }
            return ldecAmount;
        }

        //this is used in the correspondence PAU - 4260
        public string IsNonTaxableAmountExists
        {
            get
            {
                return idecNontaxableAmount > 0M ? busConstant.Flag_Yes : busConstant.Flag_No;
            }
        }

        // PROD PIR ID 4660
        public bool IsDisabilityNormalEffectiveDateValid()
        {
            if (ibusPlan.IsNull()) LoadPlan();
            if (ibusPayee.IsNull()) LoadPayee();
            //PIR 26282
            int lintPersonAccountID = busPersonAccountHelper.GetPersonAccountID(ibusPlan.icdoPlan.plan_id, ibusPayee.icdoPerson.person_id);
            busPersonAccount lbusPersonAccount = new busPersonAccount();
            lbusPersonAccount.FindPersonAccount(lintPersonAccountID);
            Collection<cdoBenefitProvisionEligibility> lclbBenefitProvisionNormalEligibility = new Collection<cdoBenefitProvisionEligibility>();
            lclbBenefitProvisionNormalEligibility = busPersonBase.LoadEligibilityForPlan(ibusPlan.icdoPlan.plan_id,
                    ibusPlan.icdoPlan.benefit_provision_id, busConstant.ApplicationBenefitTypeRetirement, busConstant.BenefitProvisionEligibilityNormal, iobjPassInfo, lbusPersonAccount?.icdoPersonAccount?.start_date);
            if (lclbBenefitProvisionNormalEligibility.Count > 0 && ibusPayee.icdoPerson.person_id != 0 && icdoPayeeAccount.disa_normal_effective_date != DateTime.MinValue)
            {
                // PROD PIR ID 6293 - For eligibility consider the next month with the normal effective date
                DateTime ldteEligibilityDate = ibusPayee.icdoPerson.date_of_birth.AddYears(Convert.ToInt32(lclbBenefitProvisionNormalEligibility[0].age)).GetFirstDayofNextMonth();
                if (icdoPayeeAccount.disa_normal_effective_date > ldteEligibilityDate)
                    return false;
            }
            return true;
        }

        public bool IsDisabilityNormalEffectiveDateFirstofMonth()
        {
            if (icdoPayeeAccount.disa_normal_effective_date.Day != 1)
                return false;
            return true;
        }


        /// <summary>
        /// method to load collection for corr app-7012
        /// </summary>
        public Collection<busPayeeAccountPaymentItemType> iclbPAPITForCorrs { get; set; }
        public void LoadPAPITForCorrs()
        {
            if (iclbMonthlyBenefits == null)
                LoadMontlyBenefits();
            iclbPAPITForCorrs = new Collection<busPayeeAccountPaymentItemType>();
            decimal ldecGrossAmount = 0.00M, ldecDeductions = 0.00M;

            ldecGrossAmount = iclbMonthlyBenefits.Where(o => o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1)
                                                    .Sum(o => o.icdoPayeeAccountPaymentItemType.amount);
            ldecDeductions = iclbMonthlyBenefits.Where(o => o.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == -1)
                                                    .Sum(o => o.icdoPayeeAccountPaymentItemType.amount);

            busPayeeAccountPaymentItemType lobjGross = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
            lobjGross.ibusPaymentItemType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
            lobjGross.ibusPaymentItemType.icdoPaymentItemType.item_type_description = "Gross Amount*";
            lobjGross.icdoPayeeAccountPaymentItemType.amount = ldecGrossAmount;
            iclbPAPITForCorrs.Add(lobjGross);
            foreach (busPayeeAccountPaymentItemType lobjPAPIT in iclbMonthlyBenefits)
            {
                if (lobjPAPIT.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == -1)
                    iclbPAPITForCorrs.Add(lobjPAPIT);
            }
            busPayeeAccountPaymentItemType lobjNet = new busPayeeAccountPaymentItemType { icdoPayeeAccountPaymentItemType = new cdoPayeeAccountPaymentItemType() };
            lobjNet.ibusPaymentItemType = new busPaymentItemType { icdoPaymentItemType = new cdoPaymentItemType() };
            lobjNet.ibusPaymentItemType.icdoPaymentItemType.item_type_description = "Net Amount";
            lobjNet.icdoPayeeAccountPaymentItemType.amount = ldecGrossAmount - ldecDeductions;
            iclbPAPITForCorrs.Add(lobjNet);
        }

        //PIR 22733
        public DateTime idt30DaysfromDateofLetter
        {
            get
            {
                return DateTime.Now.AddDays(30);
            }
        }
        public decimal idecTotalAmountDuefromPayee
        {
            get
            {
                if (iclbPaymentRecoveryWithoutBenefitOverpaymentHeader == null)
                    LoadPaymentRecoveryWithoutBenefitOverpaymentHeader();

                return iclbPaymentRecoveryWithoutBenefitOverpaymentHeader
                                     .Where(o => (o.icdoPaymentRecovery.status_value == busConstant.RecoveryStatusInProcess) ||
                                     (o.icdoPaymentRecovery.status_value == busConstant.RecoveryStatusApproved))
                                     .Sum(o => o.icdoPaymentRecovery.recovery_amount);
            }
        }
        //prod pir 7613
        public DateTime idtDependentChangeDate { get; set; }

        // PROD PIR ID 5867
        public DateTime GetRHICCombineStartDate()
        {
            // Email from Maik - Wed 2/16/2011
            // Instead of using the Retirement Date as the RHIC Effective Start Date, use the most recent date of the following: 
            // Retirement Date, latest Health enrollment, current RHIC rate effective date, or RHIC combining (combined with Spouse) – modification to point 3.
            if (ibusBenefitCalculaton.IsNull()) LoadBenefitCalculation();
            if (ibusApplication == null) LoadApplication();
            if (ibusBenefitCalculaton.ibusBenefitProvisionBenefitType.IsNull()) ibusBenefitCalculaton.LoadBenefitProvisionBenefitType(DateTime.Now);
            if (ibusLatestBenefitRhicCombine == null) LoadLatestBenefitRhicCombine();
            if (ibusPayee.IsNull()) LoadPayee();
            if (ibusPayee.icolPersonAccount == null)
                ibusPayee.LoadPersonAccount(false);
            DateTime ldteLatestHealthEnrollment = new DateTime();
            if (ibusPayee.IsMemberEnrolledInPlan(busConstant.PlanIdGroupHealth))
            {
                ldteLatestHealthEnrollment = ibusPayee.icolPersonAccount.Where(o => o.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                                                .FirstOrDefault().icdoPersonAccount.history_change_date;
            }

            DateTime ldteStartDate = ibusApplication.icdoBenefitApplication.retirement_date;
            if (ibusLatestBenefitRhicCombine != null && ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine != null)
            {
                ldteStartDate = busGlobalFunctions.GetMax(ibusLatestBenefitRhicCombine.icdoBenefitRhicCombine.start_date, ibusApplication.icdoBenefitApplication.retirement_date);
            }
            ldteStartDate = busGlobalFunctions.GetMax(ldteStartDate, ibusBenefitCalculaton.ibusBenefitProvisionBenefitType.icdoBenefitProvisionBenefitType.effective_date);
            ldteStartDate = busGlobalFunctions.GetMax(ldteStartDate, ldteLatestHealthEnrollment);
            ldteStartDate = busGlobalFunctions.GetMax(ldteStartDate, idtDependentChangeDate);
            return ldteStartDate;
        }

        //property to payment history details
        public Collection<busPaymentHistoryHeader> iclbPaymentDetails { get; set; }
        /// <summary>
        /// Method to load Payment details - 
        /// </summary>
        public void LoadPaymentDetails()
        {
            //null exception handle
            iclbPaymentDetails = new Collection<busPaymentHistoryHeader>();

            if (iclbPaymentHistoryHeader == null)
                LoadPaymentHistoryHeader();
            if (iclbPaymentHistoryHeader.Count > 0)
            {
                DateTime ldtLastPaymentDate = new DateTime(iclbPaymentHistoryHeader[0].icdoPaymentHistoryHeader.payment_date.Year,
                    iclbPaymentHistoryHeader[0].icdoPaymentHistoryHeader.payment_date.Month, 1);
                ldtLastPaymentDate = ldtLastPaymentDate.AddMonths(1).AddDays(-1);
                DataTable ldtPaymentHistory = Select("cdoPayeeAccount.LoadPaymentDetails",
                                new object[2] { icdoPayeeAccount.payee_account_id, ldtLastPaymentDate });
                iclbPaymentDetails = GetCollection<busPaymentHistoryHeader>(ldtPaymentHistory, "icdoPaymentHistoryHeader");
                if (iclbPaymentDetails.Count > 0)
                {
                    //prod pir 5651
                    //need to reduce the benefit overpayments
                    //--start--//
                    if (iclbBenefitOverpaymentDetailsToBeReduced == null)
                        LoadBenefitOverPaymentHeaderToBeReduced();
                    foreach (busPaymentHistoryHeader lobjHeader in iclbPaymentDetails)
                    {
                        busPaymentBenefitOverpaymentHeader lobjBenefitOverpayment = iclbBenefitOverpaymentDetailsToBeReduced
                            .Where(o => o.icdoPaymentBenefitOverpaymentHeader.year_1099r == lobjHeader.icdoPaymentHistoryHeader.payment_year).FirstOrDefault();
                        if (lobjBenefitOverpayment != null)
                        {
                            lobjHeader.icdoPaymentHistoryHeader.gross_amount -= lobjBenefitOverpayment.icdoPaymentBenefitOverpaymentHeader.gross_amount;
                            lobjHeader.icdoPaymentHistoryHeader.taxable_amount -= lobjBenefitOverpayment.icdoPaymentBenefitOverpaymentHeader.taxable_amount;
                            lobjHeader.icdoPaymentHistoryHeader.NonTaxable_Amount -= lobjBenefitOverpayment.icdoPaymentBenefitOverpaymentHeader.nontaxable_amount;
                            lobjHeader.icdoPaymentHistoryHeader.deduction_amount -= lobjBenefitOverpayment.icdoPaymentBenefitOverpaymentHeader.deduction_amount;
                            lobjHeader.icdoPaymentHistoryHeader.net_amount -= lobjBenefitOverpayment.icdoPaymentBenefitOverpaymentHeader.net_amount;
                        }
                    }
                    //--end--//
                    idecpaidgrossamount = iclbPaymentDetails.Select(o => o.icdoPaymentHistoryHeader.gross_amount).Sum();
                    idecpaidnontaxableamount = iclbPaymentDetails.Select(o => o.icdoPaymentHistoryHeader.NonTaxable_Amount).Sum();
                    idecpaidtaxableamount = iclbPaymentDetails.Select(o => o.icdoPaymentHistoryHeader.taxable_amount).Sum();
                    //prod pir 1454 : to load rollover amount details
                    idecPaidTaxableRolloverAmount = iclbPaymentDetails.Sum(o => o.icdoPaymentHistoryHeader.taxable_rollover_amount);
                    idecPaidNonTaxableRolloverAmount = iclbPaymentDetails.Sum(o => o.icdoPaymentHistoryHeader.nontaxable_rollover_amount);
                }
            }
        }

        //prod pir 5651
        //need to reduce the benefit overpayments
        //--start--//
        public Collection<busPaymentBenefitOverpaymentHeader> iclbBenefitOverpaymentDetailsToBeReduced { get; set; }
        private void LoadBenefitOverPaymentHeaderToBeReduced()
        {
            iclbBenefitOverpaymentDetailsToBeReduced = new Collection<busPaymentBenefitOverpaymentHeader>();
            DataTable ldtBenefitOverpayments = Select("cdoPaymentBenefitOverpaymentHeader.LoadBenefitOverpaymentDetails",
                                new object[1] { icdoPayeeAccount.payee_account_id });
            iclbBenefitOverpaymentDetailsToBeReduced = GetCollection<busPaymentBenefitOverpaymentHeader>(ldtBenefitOverpayments, "icdoPaymentBenefitOverpaymentHeader");
        }
        //--end--//

        #region UCS 24 Member portal
        public decimal idecMSSYTDpaidgrossamount { get; set; }
        public decimal idecMSSYTDpaidtaxableamount { get; set; }
        public decimal idecMSSYTDpaidnontaxableamount { get; set; }
        public Collection<busPaymentHistoryHeader> iclbMSSPaymentHistoryHeader { get; set; }
        public decimal idecMSSYTDTotalFederalTaxAmount { get; set; } //PIR 25699
        public decimal idecMSSYTDTotalNDStateTaxAmount { get; set; } //PIR 25699
        public void LoadMSSYTDPaymentDetails()
        {
            if (iclbPaymentDetails.IsNull())
                LoadPaymentDetails();
            if (iclbPaymentDetails.Count > 0)
            {
                idecMSSYTDpaidgrossamount = iclbPaymentDetails.Where(lobjPaymentHistory => lobjPaymentHistory.icdoPaymentHistoryHeader.payment_year == DateTime.Now.Year)
                    .Select(o => o.icdoPaymentHistoryHeader.gross_amount).Sum();
                idecMSSYTDpaidnontaxableamount = iclbPaymentDetails.Where(lobjPaymentHistory => lobjPaymentHistory.icdoPaymentHistoryHeader.payment_year == DateTime.Now.Year)
                    .Select(o => o.icdoPaymentHistoryHeader.NonTaxable_Amount).Sum();
                idecMSSYTDpaidtaxableamount = iclbPaymentDetails.Where(lobjPaymentHistory => lobjPaymentHistory.icdoPaymentHistoryHeader.payment_year == DateTime.Now.Year)
                    .Select(o => o.icdoPaymentHistoryHeader.taxable_amount).Sum();
                //PIR 25699
                idecMSSYTDTotalFederalTaxAmount = iclbPaymentDetails.Where(lobjPaymentHistory => lobjPaymentHistory.icdoPaymentHistoryHeader.payment_year == DateTime.Now.Year)
                    .Select(o => o.icdoPaymentHistoryHeader.idecFedTaxAmount).Sum();
                idecMSSYTDTotalNDStateTaxAmount = iclbPaymentDetails.Where(lobjPaymentHistory => lobjPaymentHistory.icdoPaymentHistoryHeader.payment_year == DateTime.Now.Year)
                    .Select(o => o.icdoPaymentHistoryHeader.idecNDStateTaxAmount).Sum();
            }
        }

        public void LoadMSSPaymentDetailsFromGoLiveDate()
        {
            if (iclbMSSPaymentHistoryHeader.IsNull())
                iclbMSSPaymentHistoryHeader = new Collection<busPaymentHistoryHeader>();

            DateTime ldteGoLiveDate = Convert.ToDateTime(busGlobalFunctions.GetData1ByCodeValue(52, "PGLD", iobjPassInfo));

            LoadPaymentDetailsByGoLiveDate(ldteGoLiveDate);

            foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in iclbPaymentDetails)
            {
                lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_year = lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.PaymentYearStartDate.Year;
                // PIR 8888 - exclude Go live date's year
                if (lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.payment_year > ldteGoLiveDate.Year)
                    iclbMSSPaymentHistoryHeader.Add(lobjPaymentHistoryHeader);
            }
        }

        private void LoadPaymentDetailsByGoLiveDate(DateTime ldteGoLiveDate)
        {
            DataTable ldtPaymentHistory = Select("cdoWssPersonAccountEnrollmentRequest.LoadPaymentDetails",
                              new object[2] { icdoPayeeAccount.payee_account_id, ldteGoLiveDate });
            iclbPaymentDetails = GetCollection<busPaymentHistoryHeader>(ldtPaymentHistory, "icdoPaymentHistoryHeader");
        }
        #endregion

        //Balance Nontaxable amount will get reduced after ever month payment
        public decimal idecBalanceNontaxableAmount { get; set; }
        public void LoadBalanceNontaxableAmount()
        {
            if (idecpaidnontaxableamount == 0.00m)
                LoadPaymentDetails();
            idecBalanceNontaxableAmount = icdoPayeeAccount.nontaxable_beginning_balance - idecpaidnontaxableamount;
        }

        //prod pir 1453 : property to contain SSLI After Change and method to load the same
        //--start--//
        public decimal idecSSLIAfterChange { get; set; }
        public void LoadSSLIAfterChange()
        {
            if (ibusBenefitAccount == null)
                LoadBenfitAccount();
            if (idecGrossBenfitAmount == 0.00M)
                LoadGrossBenefitAmount();
            if (idtNextBenefitPaymentDate == DateTime.MinValue)
                LoadNexBenefitPaymentDate();
            if (ibusBenefitAccount.icdoBenefitAccount.ssli_change_date > idtNextBenefitPaymentDate)
                idecSSLIAfterChange = idecGrossBenfitAmount - ibusBenefitAccount.icdoBenefitAccount.estimated_ss_benefit_amount;
        }
        //--end--//
        public Collection<busPayeeAccount> iclbPayeeAccountByPersonID { get; set; }
        public bool IsRetiree(int aintPersonID, bool ablnFromscreen = false)
        {
            if (icdoPayeeAccount == null)
                icdoPayeeAccount = new cdoPayeeAccount();
            DataTable adtPayeeAccount = Select<cdoPayeeAccount>(new string[1] { "payee_perslink_id" }, new object[1] { aintPersonID }, null, null);
            iclbPayeeAccountByPersonID = GetCollection<busPayeeAccount>(adtPayeeAccount, "icdoPayeeAccount");
            foreach (busPayeeAccount ibusPayeeAccountPerson in iclbPayeeAccountByPersonID)
            {
                // PIR 10973 let’s include anyone who has a Payee Account, regardless of option.  This will give them an opportunity to reprint their 1099-R.  
                // Since only refunds are not showing right now I guess we could just add that one condition.
                if (ablnFromscreen)
                {
                    if (ibusPayeeAccountPerson.icdoPayeeAccount.benefit_account_type_value != busConstant.Refund)
                    {
                        ibusPayeeAccountPerson.LoadActivePayeeStatus();
                        if (//!ibusPayeeAccountPerson.ibusPayeeAccountActiveStatus.IsStatusCompleted() && 
                            !ibusPayeeAccountPerson.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                            return true;
                        return false;
                    }
                }
                else
                {
                    ibusPayeeAccountPerson.LoadActivePayeeStatus();
                    if (!ibusPayeeAccountPerson.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                        return true;
                }
            }
            return false;
        }

        //PIR 12577 - Added below function for Name/Marital Status Change Link Display
        public bool IsRetireeLinkUpdate(int aintPersonID, bool ablnFromscreen = false)
        {
            if (icdoPayeeAccount == null)
                icdoPayeeAccount = new cdoPayeeAccount();
            DataTable adtPayeeAccount = Select<cdoPayeeAccount>(new string[1] { "payee_perslink_id" }, new object[1] { aintPersonID }, null, null);
            iclbPayeeAccountByPersonID = GetCollection<busPayeeAccount>(adtPayeeAccount, "icdoPayeeAccount");
            foreach (busPayeeAccount ibusPayeeAccountPerson in iclbPayeeAccountByPersonID)
            {

                if (ablnFromscreen)
                {
                    if (ibusPayeeAccountPerson.icdoPayeeAccount.benefit_account_type_value != busConstant.Refund)
                    {
                        ibusPayeeAccountPerson.LoadActivePayeeStatus();
                        if (//!ibusPayeeAccountPerson.ibusPayeeAccountActiveStatus.IsStatusCompleted() && 
                            ibusPayeeAccountPerson.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                            return true;
                        if (ibusPayeeAccountPerson.icdoPayeeAccount.benefit_account_type_value == busConstant.PostRetirementDeathFinal && ibusPayeeAccountPerson.ibusPayeeAccountActiveStatus.IsStatusReceiving())
                            return false;
                        return false;
                    }
                }
                else
                {
                    ibusPayeeAccountPerson.LoadActivePayeeStatus();
                    if (!ibusPayeeAccountPerson.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                        return true;
                }
            }
            return false;
        }

        //For MSS Layout change
        public Collection<busPaymentHistoryHeader> iclbPaymentDetailsThatYear { get; set; }
        public void LoadPaymentDetailsThatYear(DateTime ldatePaymentDateFrom, DateTime ldatePaymentDateTo)
        {
            // PIR 8888
            DataTable ldtb = Select("cdoPaymentHistoryHeader.LoadPaymentDetailsThatYear",
                new object[2] { ldatePaymentDateFrom.Year, icdoPayeeAccount.payee_account_id });

            iclbPaymentDetailsThatYear = GetCollection<busPaymentHistoryHeader>(ldtb, "icdoPaymentHistoryHeader");
        }

        // This method is used for setting up the visiblity of ACH Details Tab - PIR 9157
        public bool IsJobServiceThirdParty()
        {
            if (ibusPlan.IsNull()) LoadPlan();
            if (ibusPlan.icdoPlan.plan_id == busConstant.PlanIdJobService3rdPartyPayor)
            {
                return true;
            }
            return false;
        }
        // PIR-11233 repurposed under PIR-5883
        public decimal idecJSMonthlyBenefit { get; set; }

        public bool iblnCreateFlag { get; set; }
        /// <summary>
        /// If retro payment option is special check and include_in_adhoc_flag on the 
        /// payee account is YES, then throw error.
        /// </summary>
        /// <returns></returns>
        public bool IsRetroPaymentWithSpecialCheckSetUpAndIncludeInAdhocChecked()
        {
            DataTable ldtbPayeeAccountsRetroPayments = Select<cdoPayeeAccountRetroPayment>(new string[1] { enmPayeeAccountRetroPayment.payee_account_id.ToString() },
                                                                                            new object[1] { icdoPayeeAccount.payee_account_id }, null, null);
            return (iblnSpecialCheckIncludeInAdhocFlag && (ldtbPayeeAccountsRetroPayments.Rows.Count > 0) &&
                    (icdoPayeeAccount.include_in_adhoc_flag == busConstant.Flag_Yes) &&
                    ldtbPayeeAccountsRetroPayments
                    .AsEnumerable()
                    .Any(datarow => (datarow.Field<int?>(enmPayeeAccountRetroPayment.payment_history_header_id.ToString()) == null ||
                                    datarow.Field<int?>(enmPayeeAccountRetroPayment.payment_history_header_id.ToString()) == 0) &&
                         datarow.Field<string>(enmPayeeAccountRetroPayment.payment_option_value.ToString()) == busConstant.PayeeAccountRetroPaymentOptionSpecial));
        }

        public void CreateACHAndTaxesFromWssBenAppId()
        {
            busWssBenApp lbusWssBenApp = new busWssBenApp();
            if (lbusWssBenApp.FindWssBenApp(iintWssBenAppId, false, true))
            {
                if (lbusWssBenApp.icdoWssBenAppAchDetailPrimary.wss_ben_app_ach_detail_id > 0)
                {
                    CreateAchDetail(lbusWssBenApp.icdoWssBenAppAchDetailPrimary, true);
                }
                if (lbusWssBenApp.icdoWssBenAppAchDetailSecondary.wss_ben_app_ach_detail_id > 0 && !string.IsNullOrEmpty(lbusWssBenApp.icdoWssBenAppAchDetailSecondary.routing_no))
                {
                    CreateAchDetail(lbusWssBenApp.icdoWssBenAppAchDetailSecondary, false, lbusWssBenApp.icdoWssBenAppAchDetailPrimary);
                }
                CreateTax(lbusWssBenApp.icdoWssBenAppTaxWithholdingFederal, true);
                CreateTax(lbusWssBenApp.icdoWssBenAppTaxWithholdingState, false, lbusWssBenApp.icdoWssBenApp.ref_state_tax_not_withhold);
            }
        }
        private void CreateTax(cdoWssBenAppTaxWithholding acdoWssBenAppTaxWithholding, bool ablnIsFederal, string astrLumpsumStateWithheldFlag = null)
        {
            busPayeeAccountTaxWithholding lbusPayeeAccountTaxWithholding = new busPayeeAccountTaxWithholding { icdoPayeeAccountTaxWithholding = new cdoPayeeAccountTaxWithholding() };
            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.payee_account_id = icdoPayeeAccount.payee_account_id;
            lbusPayeeAccountTaxWithholding.ibusPayeeAccount = this;
            lbusPayeeAccountTaxWithholding.ibusPayeeAccount.LoadApplication();
            lbusPayeeAccountTaxWithholding.ibusPayeeAccount.LoadPayee();
            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value =
                (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund) ? busConstant.BenefitDistributionLumpSum :
                busConstant.BenefitDistributionMonthlyBenefit;
            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.start_date = lbusPayeeAccountTaxWithholding.ibusPayeeAccount.icdoPayeeAccount.benefit_begin_date;
            if (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund)
            {
                if (ablnIsFederal)
                {
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value = busConstant.TaxOptionFedTaxwithheld;
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = busConstant.PayeeAccountTaxIdentifierFedTax;
                }
                else
                {
                    if (!string.IsNullOrEmpty(astrLumpsumStateWithheldFlag) && astrLumpsumStateWithheldFlag == busConstant.Flag_Yes)
                    {
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value = busConstant.StateTaxOptionNoOnetimeNDTax;
                    }
                    else
                    {
                        lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value = busConstant.TaxOptionStateTaxwithheld;
                    }
                    lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = busConstant.PayeeAccountTaxIdentifierStateTax;
                }
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_tax_option = lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_fed_percent = acdoWssBenAppTaxWithholding.refund_fed_percent;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.refund_state_amt = acdoWssBenAppTaxWithholding.refund_state_amt;
            }
            else
            {
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value = acdoWssBenAppTaxWithholding.tax_option_value;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option = lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_option_value;

                if (!ablnIsFederal) lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.state_flat_amount = acdoWssBenAppTaxWithholding.state_flat_amount;
            }
            if (lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.benefit_distribution_type_value == busConstant.BenefitDistributionMonthlyBenefit)
            {
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_identifier_value = acdoWssBenAppTaxWithholding.tax_identifier_value;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.monthly_benefit_tax_option = acdoWssBenAppTaxWithholding.tax_option_value;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.marital_status_value = acdoWssBenAppTaxWithholding.marital_status_value;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_allowance = acdoWssBenAppTaxWithholding.tax_allowance;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.additional_tax_amount = acdoWssBenAppTaxWithholding.additional_tax_amount;

                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.filing_status_value = acdoWssBenAppTaxWithholding.filing_status_value;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.filing_status_id = 7033;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.tax_ref = acdoWssBenAppTaxWithholding.tax_ref;

                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.two_b_i = acdoWssBenAppTaxWithholding.two_b_i;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.two_b_ii = acdoWssBenAppTaxWithholding.two_b_ii;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.two_b_iii = acdoWssBenAppTaxWithholding.two_b_iii;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.three_1 = acdoWssBenAppTaxWithholding.three_1;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.three_2 = acdoWssBenAppTaxWithholding.three_2;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.three_3 = acdoWssBenAppTaxWithholding.three_3;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.three_total = acdoWssBenAppTaxWithholding.three_total;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.four_a = acdoWssBenAppTaxWithholding.four_a;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.four_b = acdoWssBenAppTaxWithholding.four_b;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.four_c = acdoWssBenAppTaxWithholding.four_c;
                lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.no_fed_withholding = acdoWssBenAppTaxWithholding.no_fed_withholding;
            }
            lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding.ienuObjectState = ObjectState.Insert;
            lbusPayeeAccountTaxWithholding.iarrChangeLog.Add(lbusPayeeAccountTaxWithholding.icdoPayeeAccountTaxWithholding);
            lbusPayeeAccountTaxWithholding.BeforeValidate(utlPageMode.New);
            lbusPayeeAccountTaxWithholding.ValidateHardErrors(utlPageMode.New);
            if (lbusPayeeAccountTaxWithholding.iarrErrors.Count > 0)
            {
            }
            else
            {
                lbusPayeeAccountTaxWithholding.BeforePersistChanges();
                lbusPayeeAccountTaxWithholding.PersistChanges();
                lbusPayeeAccountTaxWithholding.ValidateSoftErrors();
                lbusPayeeAccountTaxWithholding.UpdateValidateStatus();
                lbusPayeeAccountTaxWithholding.AfterPersistChanges();
            }
        }

        private void CreateAchDetail(cdoWssBenAppAchDetail acdoWssBenAppAchDetail, bool ablnIsPrimaryAct, cdoWssBenAppAchDetail acdoWssBenAppAchDetailPrimary = null)
        {
            busPayeeAccountAchDetail lbusPayeeAccountAchDetail = PopulateAchDetailObject();
            lbusPayeeAccountAchDetail.LoadBankOrgByRoutingNumber(acdoWssBenAppAchDetail.routing_no);
            if (lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_id > 0)
            {
                lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_org_id = lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_id;
                lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.org_code = lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_code;
            }
            else
            {
                lbusPayeeAccountAchDetail.InsertOrgBankForRoutingNumber(acdoWssBenAppAchDetail.bank_name, acdoWssBenAppAchDetail.routing_no);
                lbusPayeeAccountAchDetail.LoadBankOrgByRoutingNumber(acdoWssBenAppAchDetail.routing_no);
                if (lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_id > 0)
                {
                    lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_org_id = lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_id;
                    lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.org_code = lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_code;
                    lbusPayeeAccountAchDetail.InitializeWorkflow(lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_id, busConstant.Map_Process_Create_And_Maintain_Organization_Information);
                }
            }
            if (lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_org_id > 0)
            {
                lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.bank_account_type_value = acdoWssBenAppAchDetail.bank_account_type_value;
                lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.account_number = acdoWssBenAppAchDetail.account_number;
                lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.primary_account_flag = ablnIsPrimaryAct ? busConstant.Flag_Yes : busConstant.Flag_No;
                lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ach_start_date = lbusPayeeAccountAchDetail.ibusPayeeAccount.idtNextBenefitPaymentDate;
                lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.percentage_of_net_amount = acdoWssBenAppAchDetail.percentage_of_net_amount;
                if (lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.percentage_of_net_amount == 0.0M && acdoWssBenAppAchDetail.partial_amount > 0 && ablnIsPrimaryAct)
                {
                    lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.partial_amount =
                        (lbusPayeeAccountAchDetail.ibusPayeeAccount.idecBenefitAmount >= acdoWssBenAppAchDetail.partial_amount) ? acdoWssBenAppAchDetail.partial_amount :
                                                                                                    lbusPayeeAccountAchDetail.ibusPayeeAccount.idecBenefitAmount;
                }
                if (!ablnIsPrimaryAct && acdoWssBenAppAchDetailPrimary.IsNotNull() && acdoWssBenAppAchDetailPrimary.wss_ben_app_ach_detail_id > 0)
                {
                    if (acdoWssBenAppAchDetailPrimary.percentage_of_net_amount > 0 && acdoWssBenAppAchDetailPrimary.percentage_of_net_amount < 100)
                    {
                        lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.percentage_of_net_amount = (100 - acdoWssBenAppAchDetail.percentage_of_net_amount);
                        lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.partial_amount = 0.0M;
                    }
                    else if (acdoWssBenAppAchDetailPrimary.partial_amount > 0)
                    {
                        lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.percentage_of_net_amount = 0.0M;
                        lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.partial_amount = 0.0M;
                    }
                }
                lbusPayeeAccountAchDetail.iblnIsFromMSS = false;
                lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail.ienuObjectState = ObjectState.Insert;
                lbusPayeeAccountAchDetail.iarrChangeLog.Add(lbusPayeeAccountAchDetail.icdoPayeeAccountAchDetail);
                lbusPayeeAccountAchDetail.BeforeValidate(utlPageMode.New);
                lbusPayeeAccountAchDetail.ValidateHardErrors(utlPageMode.New);
                if (lbusPayeeAccountAchDetail.iarrErrors.Count > 0)
                {
                }
                else
                {
                    lbusPayeeAccountAchDetail.BeforePersistChanges();
                    lbusPayeeAccountAchDetail.PersistChanges();
                    lbusPayeeAccountAchDetail.ValidateSoftErrors();
                    lbusPayeeAccountAchDetail.UpdateValidateStatus();
                    lbusPayeeAccountAchDetail.AfterPersistChanges();
                }
            }
        }

        private busPayeeAccountAchDetail PopulateAchDetailObject()
        {
            busPayeeAccountAchDetail lobjPayeeAccountAchDetail = new busPayeeAccountAchDetail { icdoPayeeAccountAchDetail = new cdoPayeeAccountAchDetail() };
            lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.payee_account_id = icdoPayeeAccount.payee_account_id;
            lobjPayeeAccountAchDetail.icdoPayeeAccountAchDetail.pre_note_flag = busConstant.Flag_Yes;
            lobjPayeeAccountAchDetail.ibusPayeeAccount = this;
            lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadNexBenefitPaymentDate();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.iclbActiveACHDetails = new Collection<busPayeeAccountAchDetail>();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.ibusPrimaryAchDetail = new busPayeeAccountAchDetail { icdoPayeeAccountAchDetail = new cdoPayeeAccountAchDetail() };
            lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadBenefitAmount();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadApplication();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.ibusApplication.LoadPersonAccount();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadMember();
            lobjPayeeAccountAchDetail.ibusPayeeAccount.LoadPayee();
            return lobjPayeeAccountAchDetail;
        }
        public bool DoesMemberEnteredRolloverDetailExist()
        {
            if (ibusApplication.IsNull()) LoadApplication();
            if (ibusApplication.icdoBenefitApplication.benefit_application_id > 0)
            {
                busWssBenApp lbusWssBenApp = new busWssBenApp();
                if (lbusWssBenApp.FindWssBenAppByBenAppId(ibusApplication.icdoBenefitApplication.benefit_application_id))
                {
                    return Select<cdoWssBenAppRolloverDetail>(new string[1] { enmWssBenAppRolloverDetail.wss_ben_app_id.ToString() },
                                                     new object[1] { lbusWssBenApp.icdoWssBenApp.wss_ben_app_id }, null, "wss_ben_app_id desc").Rows.Count > 0;
                }
            }
            return false;
        }
        //PIR 18503
        //Function to fetch All ACH Details with null end date.
        public void LoadAllACHDetailsWithEndDateNullMSS()
        {
            if (iclbAchDetail == null)
                LoadACHDetail();

            if (iclbACHDetailsWithEndDateNull == null)
                iclbACHDetailsWithEndDateNull = new Collection<busPayeeAccountAchDetail>();

            foreach (busPayeeAccountAchDetail lobjACHDetail in iclbAchDetail)
            {
                if (lobjACHDetail.icdoPayeeAccountAchDetail.ach_end_date == DateTime.MinValue)
                    iclbACHDetailsWithEndDateNull.Add(lobjACHDetail);
            }
        }
        public bool IsTaxWithholdingRefundVisible()
        {
            if (ibusPayeeAccountActiveStatus == null)
                LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            return (((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePreRetirementDeath && IsBenefitoptionDeathRefund()) ||
                (icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypePostRetirementDeath && IsBenefitoptionDeathRefund()) ||
                icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund) && (ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusReview ||
                ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusApproved));
        }
        public bool IsTaxWithholdingRMDVisible()
        {
            if (iclbPayeeAccountPaymentItemType == null)
            {
                LoadPayeeAccountPaymentItemType();
            }
            if (ibusPayeeAccountActiveStatus == null)
                LoadActivePayeeStatusAsofNextBenefitPaymentDate();
            return iclbPayeeAccountPaymentItemType.Any(papit => papit.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PAPITRMDAmount) && (ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusReview ||
                ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 == busConstant.PayeeAccountStatusApproved);
        }
        public bool IsDateLessThanFebOneTwentyTwo()
        {
            return busGlobalFunctions.GetSysManagementBatchDate().Date < new DateTime(2023, 02, 01).Date;
        }
        public bool IsRefundPayeeAccount()
        {
            if ((icdoPayeeAccount.benefit_account_type_value == busConstant.ApplicationBenefitTypeRefund) || 
                (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRefund) ||
                (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionAutoRefund) ||
                (icdoPayeeAccount.benefit_option_value == busConstant.BenefitOptionRegularRefund))
                return true;
            return false;

        }
        public bool IsPayeeAccountStatusCancelOrSuspendOrCompleteOrProcessed()
        {
            LoadActivePayeeStatus();
            if (ibusPayeeAccountActiveStatus.IsStatusCancelled() || ibusPayeeAccountActiveStatus.IsStatusSuspended() ||
                ibusPayeeAccountActiveStatus.IsStatusPaymentCompleteOrProcessed())
                return true;
            return false;

        }
        //PIR 25699
        //start
        public DateTime idtmFedTaxEffDate { get; set; }
        public DateTime idtmStateTaxEffDate { get; set; }
        public int iintFedTaxWithholdingId { get; set; }
        public int iintStateTaxWithholdingId { get; set; }
        public void GetTaxWitholdingEffectiveDates()
        {
            if (iclbTaxWithholingHistory.IsNullOrEmpty())
            {
                DataTable ldtbList = Select<cdoPayeeAccountTaxWithholding>(
                                     new string[1] { "payee_account_id" },
                                     new object[1] { icdoPayeeAccount.payee_account_id }, null, "start_date desc");
                iclbTaxWithholingHistory = GetCollection<busPayeeAccountTaxWithholding>(ldtbList, "icdoPayeeAccountTaxWithholding");
            }
            if (iclbTaxWithholingHistory.IsNotNull() && iclbTaxWithholingHistory.Count() > 0)
            {
                iintFedTaxWithholdingId = iclbTaxWithholingHistory.FirstOrDefault(i => i.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax).icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id;
                iintStateTaxWithholdingId = iclbTaxWithholingHistory.FirstOrDefault(i => i.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax).icdoPayeeAccountTaxWithholding.payee_account_tax_withholding_id;

                foreach (busPayeeAccountTaxWithholding lobjPATaxWitholding in iclbTaxWithholingHistory)
                {
                    if (idtmFedTaxEffDate == DateTime.MinValue && lobjPATaxWitholding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierFedTax)
                    {
                        idtmFedTaxEffDate = lobjPATaxWitholding.icdoPayeeAccountTaxWithholding.start_date;
                    }
                    else if (idtmStateTaxEffDate == DateTime.MinValue && lobjPATaxWitholding.icdoPayeeAccountTaxWithholding.tax_identifier_value == busConstant.PayeeAccountTaxIdentifierStateTax)
                    {
                        idtmStateTaxEffDate = lobjPATaxWitholding.icdoPayeeAccountTaxWithholding.start_date;
                    }
                }
            }
        }
        //end
        //PIR 26488 Properties to load in Cor Template PAY-4008
        public string istrFBOName { get; set; }
        public string istrRolloverInstituteName { get; set; }
        public string istrRolloverAdd1 { get; set; }
        public string istrRolloverAdd2 { get; set; }
        public string istrRolloverCity { get; set; }
        public string istrRolloverState { get; set; }
        public string istrRolloverZipCode { get; set; }
        public string istrRolloverAccountNumber { get; set; }

        public void RolloverInformation()
        {
            if (iclbRolloverDetail.IsNull())
                LoadRolloverDetail();

            foreach (busPayeeAccountRolloverDetail lobjRolloverDetail in iclbRolloverDetail)
            {
                if (lobjRolloverDetail.ibusRolloverOrg.IsNull())
                    lobjRolloverDetail.LoadRolloverOrgByOrgID();
                istrFBOName = (!string.IsNullOrEmpty(lobjRolloverDetail.icdoPayeeAccountRolloverDetail.fbo) ? lobjRolloverDetail.icdoPayeeAccountRolloverDetail.fbo.ToUpper() : string.Empty);
                istrRolloverAdd1 = (!string.IsNullOrEmpty(lobjRolloverDetail.icdoPayeeAccountRolloverDetail.addr_line_1) ? lobjRolloverDetail.icdoPayeeAccountRolloverDetail.addr_line_1.ToUpper() : string.Empty);
                istrRolloverAdd2 = (!string.IsNullOrEmpty(lobjRolloverDetail.icdoPayeeAccountRolloverDetail.addr_line_2) ? lobjRolloverDetail.icdoPayeeAccountRolloverDetail.addr_line_2.ToUpper() : string.Empty);
                istrRolloverCity = (!string.IsNullOrEmpty(lobjRolloverDetail.icdoPayeeAccountRolloverDetail.city) ? lobjRolloverDetail.icdoPayeeAccountRolloverDetail.city.ToUpper() : string.Empty);
                istrRolloverState = (!string.IsNullOrEmpty(lobjRolloverDetail.icdoPayeeAccountRolloverDetail.state_value) ? lobjRolloverDetail.icdoPayeeAccountRolloverDetail.state_value.ToUpper() : string.Empty);
                istrRolloverInstituteName = (!string.IsNullOrEmpty(lobjRolloverDetail.ibusRolloverOrg.icdoOrganization.org_name) ? lobjRolloverDetail.ibusRolloverOrg.icdoOrganization.org_name.ToUpper() : string.Empty);
                istrRolloverAccountNumber = lobjRolloverDetail.icdoPayeeAccountRolloverDetail.account_number;
                if (lobjRolloverDetail.icdoPayeeAccountRolloverDetail.zip_4_code.IsNullOrEmpty())
                    istrRolloverZipCode = lobjRolloverDetail.icdoPayeeAccountRolloverDetail.zip_code;
                else
                    istrRolloverZipCode = lobjRolloverDetail.icdoPayeeAccountRolloverDetail.zip_code + "-" + lobjRolloverDetail.icdoPayeeAccountRolloverDetail.zip_4_code;

            }

        }
        // PIR 26296 Add language "Retiree QDRO Reduction Exists” on the Benefit Account Owner’s payee account
        public bool CheckDroApplicationStatus()
        {
            if (ibusBenefitCalculaton.IsNull())
                LoadBenefitCalculation();
            DataTable ldtbDroDeatils = Select("cdoPayeeAccount.LoadDroDetails",
                                        new object[1] { ibusBenefitCalculaton.icdoBenefitCalculation.benefit_calculation_id});
            if (ldtbDroDeatils.Rows.Count > 0)
                return true;
            else
                return false;
        }
		//PIR 26896
        public string istrPullCheckFlag { get; set; }

    }
}
