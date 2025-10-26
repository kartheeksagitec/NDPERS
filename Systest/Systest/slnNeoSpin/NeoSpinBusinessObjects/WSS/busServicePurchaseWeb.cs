using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;
using System.Collections;
using Sagitec.Common;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busServicePurchaseWeb : busExtendBase
    {
        public cdoServicePurchaseHeader icdoServicePurchaseWeb { get; set; }

        public busServicePurchaseHeader ibusServicePurchaseHeader { get; set; }

        public busRetirementBenefitCalculation ibusRetirementBenefitCalculation { get; set; }

        public busPerson ibusPerson { get; set; }
        public Collection<busServicePurchaseAmortizationSchedule> iclbUnusedSickLeavePurchaseSchedule { get; set; }

        public Collection<busServicePurchaseAmortizationSchedule> iclbConsolidatedPurchaseSchedule { get; set; }
        
        public Collection<busConsolidatedPurchaseWeb> iclbConsolidatedPurchase { get; set; }

        public decimal idecUnusedTotalTimeToPurchase { get; set; }

        public void LoadConsolidatePurchaseInNewMode()
        {
            if (iclbConsolidatedPurchase.IsNull())
                iclbConsolidatedPurchase = new Collection<busConsolidatedPurchaseWeb>();
            if (ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_id == 0 ||
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id != ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_id)
                ibusServicePurchaseHeader.LoadPlan();
            if (ibusServicePurchaseHeader.ibusPersonAccount.IsNull() ||
                ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.plan_id != ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id)
                ibusServicePurchaseHeader.LoadPersonAccount();
            if (ibusServicePurchaseHeader.ibusPersonAccount.ibusPersonAccountRetirement.IsNull())
                ibusServicePurchaseHeader.ibusPersonAccount.LoadPersonAccountRetirement();
            if (!busPersonBase.CheckIsPersonVestedForEstimateServicePurchase(ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_id, ibusServicePurchaseHeader.ibusPlan.icdoPlan.benefit_provision_id,
             busConstant.ApplicationBenefitTypeRetirement, ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.Total_VSC, ibusServicePurchaseHeader.ibusPersonAccount, iobjPassInfo))
            {
                busConsolidatedPurchaseWeb lobjConsolidatePurchase1 = new busConsolidatedPurchaseWeb();
                lobjConsolidatePurchase1.istrConsolidatePurchaseTypeValue = busConstant.Service_Purchase_Type_Additional_Service_Credit;
                lobjConsolidatePurchase1.istrConsolidatePurchaseType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, lobjConsolidatePurchase1.istrConsolidatePurchaseTypeValue);
                iclbConsolidatedPurchase.Remove(lobjConsolidatePurchase1);
            }
            else
            {
                busConsolidatedPurchaseWeb lobjConsolidatePurchase1 = new busConsolidatedPurchaseWeb();
                lobjConsolidatePurchase1.istrConsolidatePurchaseTypeValue = busConstant.Service_Purchase_Type_Additional_Service_Credit;
                lobjConsolidatePurchase1.istrConsolidatePurchaseType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, lobjConsolidatePurchase1.istrConsolidatePurchaseTypeValue);
                iclbConsolidatedPurchase.Add(lobjConsolidatePurchase1);
            }

            busConsolidatedPurchaseWeb lobjConsolidatePurchase2 = new busConsolidatedPurchaseWeb();
            lobjConsolidatePurchase2.istrConsolidatePurchaseTypeValue = busConstant.Service_Purchase_Type_Leave_Of_Absence;
            lobjConsolidatePurchase2.istrConsolidatePurchaseType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, lobjConsolidatePurchase2.istrConsolidatePurchaseTypeValue);
            iclbConsolidatedPurchase.Add(lobjConsolidatePurchase2);

            busConsolidatedPurchaseWeb lobjConsolidatePurchase3 = new busConsolidatedPurchaseWeb();
            lobjConsolidatePurchase3.istrConsolidatePurchaseTypeValue = busConstant.Service_Purchase_Type_Military_Service;
            lobjConsolidatePurchase3.istrConsolidatePurchaseType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, lobjConsolidatePurchase3.istrConsolidatePurchaseTypeValue);
            iclbConsolidatedPurchase.Add(lobjConsolidatePurchase3);

            busConsolidatedPurchaseWeb lobjConsolidatePurchase4 = new busConsolidatedPurchaseWeb();
            lobjConsolidatePurchase4.istrConsolidatePurchaseTypeValue = busConstant.Service_Purchase_Type_Previous_Public_Employment;
            lobjConsolidatePurchase4.istrConsolidatePurchaseType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, lobjConsolidatePurchase4.istrConsolidatePurchaseTypeValue);
            iclbConsolidatedPurchase.Add(lobjConsolidatePurchase4);

            busConsolidatedPurchaseWeb lobjConsolidatePurchase5 = new busConsolidatedPurchaseWeb();
            lobjConsolidatePurchase5.istrConsolidatePurchaseTypeValue = busConstant.Service_Purchase_Type_Previous_Pers_Employment;
            lobjConsolidatePurchase5.istrConsolidatePurchaseType = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(326, lobjConsolidatePurchase5.istrConsolidatePurchaseTypeValue);
            iclbConsolidatedPurchase.Add(lobjConsolidatePurchase5);
        }

        public int iintAvailablePlanCount
        {
            get
            {
                return iclbEligiblePlan.Count;
            }
        }

        // load plans for which the member can estimate
        public Collection<cdoPlan> iclbEligiblePlan { get; set; }
        public Collection<cdoPlan> LoadPlansForServicePurchase()
        {
            LoadEligiblePlans();
            return iclbEligiblePlan;
        }

        public void LoadEligiblePlans()
        {
            iclbEligiblePlan = new Collection<cdoPlan>();

            if (ibusServicePurchaseHeader.ibusPerson.icolPersonAccountByBenefitType.IsNull())
                ibusServicePurchaseHeader.ibusPerson.LoadPersonAccountByBenefitType(busConstant.PlanBenefitTypeRetirement);

            var lenumPlanList = ibusServicePurchaseHeader.ibusPerson.icolPersonAccountByBenefitType.Where(lobjPA => lobjPA.icdoPersonAccount.person_account_id > 0
                && (lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled
                 || lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended));

            foreach (busPersonAccount lobjPlan in lenumPlanList)
            {
                lobjPlan.LoadPlan();
                iclbEligiblePlan.Add(lobjPlan.ibusPlan.icdoPlan);
            }
        }

        public bool IsDCMember()
        {
            if (ibusServicePurchaseHeader.IsNotNull() && ibusServicePurchaseHeader.ibusPerson.IsNotNull())
            {
                if (ibusServicePurchaseHeader.ibusPerson.IsDCPersonAccountExists(true)) //PIR 16439
                    return true;
            }
            return false;
        }

        public decimal idecCreditedServiceOnFile { get; set; }
        public decimal idecUnusedSickLeaveConversionCost { get; set; }
        public decimal idecUnusedSickLeaveMonths { get; set; }
        public decimal idecServiceMonthsPurchased { get; set; }
        public decimal idecServiceMonthsPurchaseCost { get; set; }
        public decimal idecEstimatedSCYearlyForVesting { get; set; }
        public decimal idecEstimatedSCForBenCal { get; set; }
        public decimal idecEstimatedSCForVesting { get; set; }

        public decimal idecEstimatedSCYearlyForBenCal { get; set; }
        public string idecEstimatedSCYearlyForBenCal_formatted
        {
            get
            {
                if (idecEstimatedSCYearlyForBenCal < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(idecEstimatedSCYearlyForBenCal / 12).ToString(),
                                     Math.Round((idecEstimatedSCYearlyForBenCal % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(idecEstimatedSCYearlyForBenCal / 12).ToString(),
                                     Math.Round((idecEstimatedSCYearlyForBenCal % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }

        public decimal idecServiceAge { get; set; }
        public string idecServiceAge_formatted
        {
            get
            {
                if (idecServiceAge < 0)
                    return String.Format("{0} Years {1} Months", Math.Ceiling(idecServiceAge / 12).ToString(), Math.Round((idecServiceAge % 12), 4, MidpointRounding.AwayFromZero).ToString());

                return String.Format("{0} Years {1} Months", Math.Floor(idecServiceAge / 12).ToString(), Math.Round((idecServiceAge % 12), 4, MidpointRounding.AwayFromZero).ToString());
            }
        }


        public ArrayList btnCalculatedEstimatedServicePurchase_Click()
        {
            ArrayList alReturn = new ArrayList();
            if (icdoServicePurchaseWeb.plan_id == 0)
            {
                utlError lobjError = new utlError();
                lobjError = AddError(0, "Please Select A Plan.");
                alReturn.Add(lobjError);
                return alReturn;
            }
            if (ibusServicePurchaseHeader.iintWhatIfNoOfPayments > 0M && ibusServicePurchaseHeader.idecWhatIfExpextedInstallmentAmount > 0M)
            {
                utlError lobjError = new utlError();
                lobjError = AddError(0, "Only Number of Payment or Payment Amount should be entered"); // PIR 9727
                alReturn.Add(lobjError);
                return alReturn;
            }
            // PIR 24243
            if (ibusServicePurchaseHeader.ibusPerson.IsNotNull() && ibusServicePurchaseHeader.ibusPerson.icdoPerson.person_id > 0 &&
               ibusServicePurchaseHeader.ibusPerson.icdoPerson.limit_401a == busConstant.Flag_Yes)
            {
                utlError lobjError = new utlError();
                lobjError = AddError(10425, busGlobalFunctions.GetMessageTextByMessageID(10425, iobjPassInfo)); 
                alReturn.Add(lobjError);
                return alReturn;
            }

            ibusServicePurchaseHeader.icdoServicePurchaseHeader = icdoServicePurchaseWeb;
            //PIR 18363 - Dual member unable to perform service purchase 
            if (ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_id == 0 ||
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id != ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_id)
                ibusServicePurchaseHeader.LoadPlan();
            if (ibusServicePurchaseHeader.ibusPersonAccount.IsNull() ||
                ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.plan_id != ibusServicePurchaseHeader.icdoServicePurchaseHeader.plan_id)
                ibusServicePurchaseHeader.LoadPersonAccount();
            if (ibusServicePurchaseHeader.ibusPersonAccount.ibusPersonAccountRetirement.IsNull())
                ibusServicePurchaseHeader.ibusPersonAccount.LoadPersonAccountRetirement();
            // PIR 26099
            //PROD PIR 26631 - MSS issue with Service Purchases -Due to Vesting
            //int lintMonths = 0;
            //decimal ldecMonthYear = 0.00M;
            //int lintMemberAgeYear = 0;
            //int lintMemberAgeMonth = 0;
            //busPersonBase.CalculateAge(ibusServicePurchaseHeader.ibusPerson.icdoPerson.date_of_birth, DateTime.Now, ref lintMonths, ref ldecMonthYear, 2, ref lintMemberAgeYear, ref lintMemberAgeMonth);

            //if (!busPersonBase.CheckIsPersonVestedForEstimateServicePurchase(ibusServicePurchaseHeader.ibusPlan.icdoPlan.plan_id, ibusServicePurchaseHeader.ibusPlan.icdoPlan.benefit_provision_id,
            //    busConstant.ApplicationBenefitTypeRetirement, ibusServicePurchaseHeader.ibusPersonAccount.icdoPersonAccount.Total_VSC, ibusServicePurchaseHeader.ibusPersonAccount, iobjPassInfo))
            //{
            //    utlError lobjError = new utlError();
            //    lobjError = AddError(10496, busGlobalFunctions.GetMessageTextByMessageID(10496, iobjPassInfo));
            //    alReturn.Add(lobjError);
            //    return alReturn;
            //}
            ibusServicePurchaseHeader.ibusPersonAccount.ibusPersonAccountRetirement.ibusPerson = ibusServicePurchaseHeader.ibusPerson;
            ibusServicePurchaseHeader.ibusPersonAccount.ibusPersonAccountRetirement.ibusPlan = ibusServicePurchaseHeader.ibusPlan;
            ibusServicePurchaseHeader.ibusPersonAccount.ibusPersonAccountRetirement.ibusPersonAccount = ibusServicePurchaseHeader.ibusPersonAccount;
            ibusServicePurchaseHeader.icdoServicePurchaseHeader.down_payment = ibusServicePurchaseHeader.idecWhatIfPaymentAmount;
            ibusServicePurchaseHeader.idecPayOffAmount = ibusServicePurchaseHeader.idecWhatIfPaymentAmount;
            iblnNumberAndAmountNotEntered = false;
            //F/W Upgrade PIRs 11238, 11157 - As Per Discussion with Maik, when both amount and number of payments not entered, default number of payments
            // to 180, both cannot be entered, when one of them entered calculate with that entered value.
            if (ibusServicePurchaseHeader.idecWhatIfExpextedInstallmentAmount == 0M && ibusServicePurchaseHeader.iintWhatIfNoOfPayments == 0)
            {
                iblnNumberAndAmountNotEntered = true;
                ibusServicePurchaseHeader.iintWhatIfNoOfPayments = 180;
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.number_of_payments = 180;
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.expected_installment_amount = 0M;
            }
            else if (ibusServicePurchaseHeader.iintWhatIfNoOfPayments > 0)
            {
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.number_of_payments = ibusServicePurchaseHeader.iintWhatIfNoOfPayments;
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.expected_installment_amount = 0M;
            }
            else if (ibusServicePurchaseHeader.idecWhatIfExpextedInstallmentAmount > 0M)
            {
                ibusServicePurchaseHeader.iintWhatIfNoOfPayments = 0;
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.number_of_payments = 0;
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.expected_installment_amount = ibusServicePurchaseHeader.idecWhatIfExpextedInstallmentAmount;
            }
            if(ibusServicePurchaseHeader.istrWhatIfPaymentFrequency.IsNullOrEmpty())
            {
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.payment_frequency_value = busConstant.ServicePurchasePaymentFrequencyValueMonthly;
                ibusServicePurchaseHeader.istrWhatIfPaymentFrequency = busConstant.ServicePurchasePaymentFrequencyValueMonthly;
            }
            else
            {
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.payment_frequency_value = ibusServicePurchaseHeader.istrWhatIfPaymentFrequency;
            }
            ibusServicePurchaseHeader.icdoServicePurchaseHeader.payroll_deduction = busConstant.Flag_No;
            ibusServicePurchaseHeader.icdoServicePurchaseHeader.pre_tax = busConstant.Flag_No;
            ibusServicePurchaseHeader.icdoServicePurchaseHeader.payor_value = busConstant.Service_Purchase_Payor_Employee;
            ibusServicePurchaseHeader.iblnLoadMemberTypeByPlan = true;
            cdoPersonEmploymentDetail lcdoPersonEmpDetail = ibusServicePurchaseHeader.LoadMemberTypeByContributingStatus().FirstOrDefault();
            if (lcdoPersonEmpDetail != null)
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.member_type_value = lcdoPersonEmpDetail.derived_member_type_value;
            ibusServicePurchaseHeader.CalculateCurrentAge();
            ibusServicePurchaseHeader.btnRefreshFAS_Click();
            ibusServicePurchaseHeader.CalculateDateOfExpiration();
            ibusServicePurchaseHeader.SetCurrentInterestRate();

            // Unused Sick Leave Purchase
            ibusServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value = busConstant.Service_Purchase_Type_Unused_Sick_Leave;
            ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail = ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail;
            ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.ibusServicePurchaseHeader = ibusServicePurchaseHeader;
            ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.RecomputeCalculatedFields();
            idecUnusedTotalTimeToPurchase = ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase;
            ibusRetirementBenefitCalculation = ibusServicePurchaseHeader.ibusPersonAccount.ibusPersonAccountRetirement.ibusRetirementBenefitCalculation;
            idecCreditedServiceOnFile = ibusRetirementBenefitCalculation.icdoBenefitCalculation.credited_psc;
            idecUnusedSickLeaveConversionCost = ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.SickLeaveTotalPurchaseCost;
            idecUnusedSickLeaveMonths = ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase;
            idecServiceMonthsPurchased = ibusServicePurchaseHeader.TotalTimeToPurchase;
            idecServiceMonthsPurchaseCost = ibusServicePurchaseHeader.TotalPurchaseCost;
            busBenefitCalculatorWeb lobjBenefitCalculator = new busBenefitCalculatorWeb { icdoWssBenefitcalculator = new cdoWssBenefitCalculator() };
            lobjBenefitCalculator.ibusMember = ibusServicePurchaseHeader.ibusPerson;
            lobjBenefitCalculator.GetTentativeOrApprovedTFFRTIAAAmount();
            idecEstimatedSCForVesting = ibusRetirementBenefitCalculation.icdoBenefitCalculation.credited_vsc
                            + lobjBenefitCalculator.icdoWssBenefitcalculator.tffr_tiaa_service_credit
                            + ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.RoundedTotalTimeOfPurchaseExcludeFreeService
                            + ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase;
            idecEstimatedSCYearlyForVesting = Math.Round((idecEstimatedSCForVesting / 12), 2, MidpointRounding.AwayFromZero);
            idecEstimatedSCForBenCal = ibusRetirementBenefitCalculation.icdoBenefitCalculation.credited_psc
                                         + ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.RoundedTotalTimeOfPurchaseExcludeFreeService
                                        + ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.icdoServicePurchaseDetail.total_time_to_purchase;
            idecEstimatedSCYearlyForBenCal = idecEstimatedSCForBenCal / 12;
            idecServiceAge = ibusRetirementBenefitCalculation.idecMemberAgeBasedOnRetirementDate + idecEstimatedSCYearlyForBenCal;

            iclbUnusedSickLeavePurchaseSchedule = new Collection<busServicePurchaseAmortizationSchedule>();
            bool iblnIsExpectedPaymentAmountTooSmall = false;
            ibusServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule =
                        busServicePurchaseAmortizationSchedule.CalculateAmortizationSchedule(ibusServicePurchaseHeader, iobjPassInfo,   ref iblnIsExpectedPaymentAmountTooSmall);
            iclbUnusedSickLeavePurchaseSchedule = busServicePurchaseAmortizationSchedule.CalculateWhatIfAmortizationSchedule(ibusServicePurchaseHeader, iobjPassInfo);

            // Consolidated Purchase calculation
            ibusServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value = busConstant.Service_Purchase_Type_Consolidated_Purchase;
            ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated = new Collection<busServicePurchaseDetailConsolidated>();
            foreach (busConsolidatedPurchaseWeb lbusConsolidatedPurchaseWeb in iclbConsolidatedPurchase)
            {
                if (lbusConsolidatedPurchaseWeb.IsValidEntry())
                {
                    busServicePurchaseDetailConsolidated lbusSPConsolidated = new busServicePurchaseDetailConsolidated
                    {
                        icdoServicePurchaseDetailConsolidated = new cdoServicePurchaseDetailConsolidated()
                    };
                    lbusSPConsolidated.icdoServicePurchaseDetailConsolidated.service_credit_type_value = lbusConsolidatedPurchaseWeb.istrConsolidatePurchaseTypeValue;
                    lbusSPConsolidated.icdoServicePurchaseDetailConsolidated.time_to_purchase = lbusConsolidatedPurchaseWeb.iintAdditionalServiceCredits;
                    lbusSPConsolidated.icdoServicePurchaseDetailConsolidated.service_purchase_start_date = lbusConsolidatedPurchaseWeb.idtFromDate;
                    lbusSPConsolidated.icdoServicePurchaseDetailConsolidated.service_purchase_end_date = lbusConsolidatedPurchaseWeb.idtToDate;
                    lbusSPConsolidated.ibusServicePurchaseDetail = ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail;
                    lbusSPConsolidated.ibusServicePurchaseDetail.ibusServicePurchaseHeader = ibusServicePurchaseHeader;
                    lbusSPConsolidated.CalculateTimeToPurchase();
                    ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.iclbServicePurchaseDetailConsolidated.Add(lbusSPConsolidated);
                }
            }
            ibusServicePurchaseHeader.ibusPrimaryServicePurchaseDetail.RecomputeCalculatedFields();

            iclbConsolidatedPurchaseSchedule = new Collection<busServicePurchaseAmortizationSchedule>();
            bool iblnIsExpectedPaymentAmountTooSmall1 = false;
            ibusServicePurchaseHeader.iblnFromMssCheck180 = true; 
            ibusServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule =
                        busServicePurchaseAmortizationSchedule.CalculateAmortizationSchedule(ibusServicePurchaseHeader, iobjPassInfo, ref iblnIsExpectedPaymentAmountTooSmall1);
            ibusServicePurchaseHeader.idecWhatIfPaymentAmount = 0M; // PIR 9766 Double deducting the down-payment amount issue resolved.
            iclbConsolidatedPurchaseSchedule = busServicePurchaseAmortizationSchedule.CalculateWhatIfAmortizationSchedule(ibusServicePurchaseHeader, iobjPassInfo);
            ibusServicePurchaseHeader.idecWhatIfPaymentAmount = ibusServicePurchaseHeader.icdoServicePurchaseHeader.down_payment; // PIR 21431 Down Payment Amount should be persist
            if(iblnNumberAndAmountNotEntered)
            {
                ibusServicePurchaseHeader.iintWhatIfNoOfPayments = 0;
                ibusServicePurchaseHeader.icdoServicePurchaseHeader.number_of_payments = 0;
            }
            alReturn.Add(this);
            return alReturn;
        }

        public string istrServicePurchaseType { get; set; }        

        public ArrayList btnGo_click()
        {
            ArrayList alReturn = new ArrayList();
            if(iarrErrors == null)  iarrErrors = new ArrayList();
            utlError lobjError = new utlError();
            if (ibusPerson == null)
                LoadPerson();
            if (istrServicePurchaseType.IsNotNullOrEmpty() && (istrServicePurchaseType == "CONT" || istrServicePurchaseType == "ESTI" || istrServicePurchaseType == "SREQ"))
            {
                if (IsMSSServicePurchaseTypeValid(ibusPerson.icdoPerson.person_id))
                {
                    if (!IsPermanentMember(ibusPerson.icdoPerson.person_id))                    
                    {
                        lobjError = AddError(0, "You are not eligible to Purchase Credit as a Temporary Employee.");
                        iarrErrors.Add(lobjError);
                        return iarrErrors;
                    }                }
                else
                {
                    lobjError = AddError(0, "You do not have valid NDPERS account to Perform an Estimate.");
                    iarrErrors.Add(lobjError);
                    return iarrErrors;
                }
            }
            
            alReturn.Add(this);
            return alReturn;
        }

        public bool IsMSSServicePurchaseTypeValid(int aintPersonID)
        {
            busPerson lobjPerson = new busPerson { icdoPerson = new cdoPerson { person_id = aintPersonID } };
            lobjPerson.LoadRetirementAccount();
            foreach (busPersonAccount lobjPA in lobjPerson.iclbRetirementAccount)
            {
                if (lobjPA.icdoPersonAccount.plan_id != busConstant.PlanIdDC &&
                    lobjPA.icdoPersonAccount.plan_id != busConstant.PlanIdJobService)
                {
                    if (lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                        lobjPA.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
                        return true;
                }
            }
            return false;
        }

        public bool IsPermanentMember(int aintPersonID)
        {

            DataTable ldtPersonEmployementDetail = busBase.Select("entPersonEmploymentDetail.LoadAllEmploymentDetailsForPerson", new object[1] { aintPersonID });
            Collection<busPersonEmploymentDetail> lclbPersonEmployementDetails = GetCollection<busPersonEmploymentDetail>(ldtPersonEmployementDetail, "icdoPersonEmploymentDetail");
            if (lclbPersonEmployementDetails.Count > 0)
            {
                if (lclbPersonEmployementDetails.All(emp => emp.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary))
                {
                    return false;
                }
                else
                {
                    System.Collections.Generic.IEnumerable<busPersonEmploymentDetail> lInuPersonEmployementDetails = lclbPersonEmployementDetails.Where(emp => emp.icdoPersonEmploymentDetail.end_date == DateTime.MinValue || emp.icdoPersonEmploymentDetail.end_date.Date >= DateTime.Today.Date);
                    if (lInuPersonEmployementDetails.Count() > 0 && lInuPersonEmployementDetails.All(emp => emp.icdoPersonEmploymentDetail.type_value == busConstant.PersonJobTypeTemporary))
                        return false;
                }
            }
            else if (lclbPersonEmployementDetails.Count == 0)
            {
                return false;
            }
            return true;
        }
        public void LoadPerson()
        {
            if (ibusPerson == null)
            {
                ibusPerson = new busPerson();
            }
            ibusPerson.FindPerson(icdoServicePurchaseWeb.person_id);
        }

        //wfmDefault.aspx file code conversion - btn_OpenPDF method 
        public string istrDownloadFileName
        {
            get
            {
                DataTable ldtbPathData = utlPassInfo.iobjPassInfo.isrvDBCache.GetCacheData("sgs_system_paths", "path_code = 'MSSHELP'");
                return ldtbPathData.Rows[0]["path_value"].ToString() + "Service_Purchase_Provisions.pdf";
            }
        }

        public bool iblnNumberAndAmountNotEntered { get; set; }
        //PIR 26099
        public bool iblnIsMemberHasLessThan3SalaryPosted
        {
            get
            {
                if (iintAvailablePlanCount == 1)
                {
                    ibusServicePurchaseHeader.ibusPerson.LoadPersonAccountByPlan(iclbEligiblePlan[0].plan_id);
                    busPersonAccount lobjPersonAccount = ibusServicePurchaseHeader.ibusPerson.icolPersonAccountByPlan.
                        // PIR 26486 Select Person Account with plan_participation_status_value ENRL1 or SUS2
                        Where(i => i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
                        i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended).FirstOrDefault();
                    DataTable ldtRetirementContributionAll = Select<cdoPersonAccountRetirementContribution>(new string[1] { "person_account_id" },
                                                                                    new object[1]
                                                                                        {
                                                                                             lobjPersonAccount.icdoPersonAccount.person_account_id
                                                                                        }, null, null);
                    return ldtRetirementContributionAll.AsEnumerable().Where(o => o.Field<decimal?>("SALARY_AMOUNT") > 0).AsDataTable().Rows.Count < 3;
                    
                }
                return false;
            }
        }
		//PIR 26099
        public Collection<cdoCodeValue> LoadParticipationStatusByBenefitType()
        {           
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(6006);
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);           
            Collection<cdoCodeValue> lclcServicePurchaseTypeCodeValue =  new Collection<cdoCodeValue>();
            foreach (cdoCodeValue lcdoCodeValue in lclcCodeValue)
            {
                if (lcdoCodeValue.code_value == busConstant.CalculateServicePurchaseCostEstimate)
                {
                    if(!((ibusServicePurchaseHeader.IsNotNull() && ibusServicePurchaseHeader.IsMemberIsTFFRdual())
                        || (iintAvailablePlanCount > 1) || (iintAvailablePlanCount == 1 && iblnIsMemberHasLessThan3SalaryPosted)))
                    {
                        lclcServicePurchaseTypeCodeValue.Add(lcdoCodeValue);
                    }
                }
                else
                {
                    lclcServicePurchaseTypeCodeValue.Add(lcdoCodeValue);
                }
            }
            return lclcServicePurchaseTypeCodeValue;
        }

    }
}
