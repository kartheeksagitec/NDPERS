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

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busServicePurchaseAmortizationSchedule : busServicePurchaseAmortizationScheduleGen
    {

        private static Collection<busServicePurchaseAmortizationSchedule> ProcessAmortizationSchedule(
            busServicePurchaseHeader aobjServicePurchaseHeader,
            decimal adecPrincipleBalance,
            string astrPaymentFrequency,
            int aintNoOfPayments,
            decimal adecExpectedInstallmentAmount,
            DateTime adtExpectedPaymentStartDate,
            bool ablnIsWhatIfSchedule,
            decimal adecWhatIfPaymentAmount,
            DateTime adtWhatIfPaymentDate,
            utlPassInfo aobjPassInfo,
            ref bool ablnIsExpectedPaymetAmountTooSmall,
            ref int aintTotalNoOfFuturePayments,
            ref decimal adecCalculatedExpectedInstallmentAmount,
            ref decimal adecPayOffAmount,
            ref int aintPaymentNo
            )
        {
            aintTotalNoOfFuturePayments = 0;
            Collection<busServicePurchaseAmortizationSchedule> lclbAmortizationSchedule = new Collection<busServicePurchaseAmortizationSchedule>();

          //  decimal ldecInterestRate = aobjServicePurchaseHeader.icdoServicePurchaseHeader.contract_interest; ////PIR-17512 

            //Load the Detail Collection if null
            if (aobjServicePurchaseHeader.ibusPrimaryServicePurchaseDetail == null)
                aobjServicePurchaseHeader.LoadServicePurchaseDetail();
            DateTime ldtPreviousPaymentDate = DateTime.MinValue;
            ldtPreviousPaymentDate = aobjServicePurchaseHeader.first_payment_date;
            // we have to do the amortization schedule only if the principle amount is greater than Zero 
            if (adecPrincipleBalance > 0)
            {

                // Go ahead and get all the code values tied to the payment frequency selected by the user
                if (astrPaymentFrequency.IsNullOrEmpty()) //PROD PIR 4793 - we have added the validation.. for existing data, in order to avoid exception, we setting the default to monthly.
                    astrPaymentFrequency = busConstant.ServicePurchasePaymentFrequencyValueMonthly;
                cdoCodeValue lobjCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.Service_Purchase_Payment_Frequency_Code_Id, astrPaymentFrequency);

                // Change the ldecInterestRate based on the Payment Frequency
                //Interest should not be calculated for OneTime Lumbsum. To Avoid, division by zero exception, we check data3 value.
              /*  // PIR-17521
			  if ((Convert.ToDecimal(lobjCodeValue.data3) != 0) && (aintNoOfPayments != 1))
                {
                    ldecInterestRate = ldecInterestRate / Convert.ToDecimal(lobjCodeValue.data3);
                }
                else
                {
                    ldecInterestRate = 0M;
                }*/
                int lintTotalNoofPayments = CountofServicePurchasePaymentAllocation(aobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id);
                int lintMonthsToBeAddedToDeriveNextDueDate = Convert.ToInt32(lobjCodeValue.data2);

                aintPaymentNo = 0;
                int lintRemittanceId = 0;
                string lstrReferenceFormConst = String.Empty;
                int lintReferenceID = 0;
                int lintEmployerPayrollDetailID = 0;
                string lstrPaymentClass = string.Empty;

                // Get the payment start date and reduce it by number of months, so we get to use the datetime variable once we get into the loop.
                DateTime ldtPaymentDueDate = adtExpectedPaymentStartDate.AddMonths(-1 * lintMonthsToBeAddedToDeriveNextDueDate);
                decimal ldecInterestCarryOver = 0;
                DateTime ldtCurrentDate = Convert.ToDateTime(System.DateTime.Now.ToShortDateString());
                // Changes for PIR reported by Jeeva, Divide by Zero exception thrown in scenarios
                // where the number of payments goes below Zero
                bool lblnContinueAmortizationSchedule = true;
                // PIR 23827
                decimal ldecData3Value = astrPaymentFrequency == busConstant.ServicePurchasePaymentFrequencyValueAnnual ? 1 :
                                         astrPaymentFrequency == busConstant.ServicePurchasePaymentFrequencyValueQuarterly ? 4 :
                                         astrPaymentFrequency == busConstant.ServicePurchasePaymentFrequencyValueSemiAnnual ? 2 :
                                         Convert.ToDecimal(lobjCodeValue.data3);
                if (aintNoOfPayments > 0 || adecExpectedInstallmentAmount > 0)
                {
                    while (Math.Round(adecPrincipleBalance, 2, MidpointRounding.AwayFromZero) > 0 && lblnContinueAmortizationSchedule)
                    {
                        #region PIR-17512
                        decimal ldecInterestRate = busGlobalFunctions.GetCodeValueDetailsfromData2(busConstant.ServicePurchaseContractInterestCodeId, ldtPaymentDueDate.AddMonths(lintMonthsToBeAddedToDeriveNextDueDate), aobjPassInfo);
                        if (aobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value != busConstant.Service_Purchase_Type_USERRA_Military_Service && (Convert.ToDecimal(lobjCodeValue.data3) != 0) && (aintNoOfPayments != 1))
                        {
                            ldecInterestRate = ldecInterestRate / ldecData3Value;
                        }
                        else
                        {
                            ldecInterestRate = 0M;
                        }

                        #endregion PIR-17512
                        decimal ldecExpectedTotalAmount = 0;
                        decimal ldecExpectedPrincipalAmount = 0;
                        decimal ldecPrincipleBalance = adecPrincipleBalance;
                        // Add the new interest to be calculated with InterestCarryOver from Last schedule.

                        //PIR : 919, Do Not Set the Interest Amount for the First Payment Schedule Entry
                        //PIR 759 : This condition would not be valid for What If Schdules
                        decimal ldecExpectedInterestAmount = 0;
                        if ((aintPaymentNo >= 0) || (ablnIsWhatIfSchedule))
                            ldecExpectedInterestAmount = Math.Round(adecPrincipleBalance * ldecInterestRate + ldecInterestCarryOver, 2, MidpointRounding.AwayFromZero);

                        // Incase the user is specifying the TotalPaymentAmount he is ready to pay, we need to use that
                        // value for deriving other values, or else we need to calculate the TotalPaymentAmount based on the number of payments he is 
                        // willing to pay.
                        if (adecExpectedInstallmentAmount > 0)
                        {
                            ldecExpectedTotalAmount = adecExpectedInstallmentAmount;
                            // Towards the end of our amortization schedule calculation, we might face a scenario where
                            // we would have the total payment amount greater than the Principle balance + Interest Amount, in which case we need
                            // to go ahead and set the minimum of the amounts

                            ldecExpectedTotalAmount = Math.Min(ldecExpectedTotalAmount, (adecPrincipleBalance + ldecExpectedInterestAmount));
                        }
                        else
                        {
                            //if the interst amount is zero (for onetime lumpsum), assign the pricipal balance as expected total amount
                            if (ldecInterestRate == 0M && (aobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value != busConstant.Service_Purchase_Type_USERRA_Military_Service))
                            {
                                ldecExpectedTotalAmount = adecPrincipleBalance;
                            }
                            else
                            {
                                try
                                {   //PIR 17946 - Interest should not be charged on USERRA purchases
                                    if (aobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_type_value == busConstant.Service_Purchase_Type_USERRA_Military_Service && ldecInterestRate == 0M)
                                    {			//PIR-17512 interest calculation 
                                        if (aintNoOfPayments > 0)
                                            ldecExpectedTotalAmount = adecPrincipleBalance / aintNoOfPayments;
                                        else
                                            ldecExpectedTotalAmount = adecPrincipleBalance;
                                    }
                                    else
                                        ldecExpectedTotalAmount = (ldecInterestRate / (1 - (busGlobalFunctions.Power((1 + ldecInterestRate), -(aintNoOfPayments))))) * adecPrincipleBalance;
                                    adecExpectedInstallmentAmount = ldecExpectedTotalAmount;
                                }
                                catch (DivideByZeroException _exc)
                                {
                                    //Do Nothing
                                }
                            }
                        }

                        ldecExpectedPrincipalAmount = ldecExpectedTotalAmount - ldecExpectedInterestAmount;
                        // Arrive at the principle balance by subtracting the principle payment amount
                        adecPrincipleBalance = adecPrincipleBalance - ldecExpectedPrincipalAmount;
                        // Add months to arrive at the next payment due date.
                        ldtPaymentDueDate = ldtPaymentDueDate.AddMonths(lintMonthsToBeAddedToDeriveNextDueDate);

                        // The following set of variables have been declared to keep the carryover amount to the next iteration. 
                        // we initialize them to the payment amount expected 
                        decimal ldecPrincipalCarryover = ldecExpectedPrincipalAmount;
                        if(astrPaymentFrequency == busConstant.ServicePurchasePaymentFrequencyValueMonthly)
                            ldecInterestCarryOver = ldecExpectedInterestAmount;


                        // We have to see if the payment has not been received for a member for a particular month and based on that we have to recalculate
                        // the interest amount and payment amount. (this has to be done only if the payment due date is lesser than the current date.
                        // Go and get all the recorded Remittance Allocation for this service purchase from SGT_Service_Purchase_Payment_Allocation table between this date
                        // and the next payment start date.
                        // We have to get all the payments we have received in the duration of this month/quarter.
                        DateTime ldtPaymentStartDate =
                            ldtPaymentDueDate.AddMonths(-1 * lintMonthsToBeAddedToDeriveNextDueDate).AddDays(1);
                        if (ldtPaymentDueDate < ldtCurrentDate || ldtPaymentStartDate <= ldtCurrentDate)
                        {
                            decimal ldecPaymentAmountForPayOff = 0;
                            bool lblnLastPaymentMade = false;
                            Collection<busServicePurchasePaymentAllocation> lclbPaymentAllocation = new Collection<busServicePurchasePaymentAllocation>();

                            //For What IF Schedule, if the payment amount is not zero, it will add the collection with the one allocation entry only if the 
                            //payment start date and due date condtion satifies.
                            if (ablnIsWhatIfSchedule)
                            {
                                if (adecWhatIfPaymentAmount != 0)
                                {
                                    if ((adtWhatIfPaymentDate >= ldtPaymentStartDate) &&
                                        (adtWhatIfPaymentDate <= ldtPaymentDueDate))
                                    {
                                        busServicePurchasePaymentAllocation lbusAllocation = new busServicePurchasePaymentAllocation();
                                        lbusAllocation.icdoServicePurchasePaymentAllocation = new cdoServicePurchasePaymentAllocation();
                                        lbusAllocation.icdoServicePurchasePaymentAllocation.service_purchase_header_id =
                                            aobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id;

                                        lbusAllocation.icdoServicePurchasePaymentAllocation.
                                            service_purchase_payment_class_value =
                                            busConstant.Service_Purchase_Class_Employer_Installment_PreTax;

                                        lbusAllocation.icdoServicePurchasePaymentAllocation.payment_date = adtWhatIfPaymentDate;
                                        lbusAllocation.icdoServicePurchasePaymentAllocation.applied_amount = adecWhatIfPaymentAmount;
                                        lclbPaymentAllocation.Add(lbusAllocation);
                                    }
                                }
                            }
                            else
                            {
                                DataTable ldtbList = busBase.SelectWithOperator<cdoServicePurchasePaymentAllocation>
                                     (
                                        new string[3] { "service_purchase_header_id", "payment_date", "payment_date" },
                                        new string[3] { "=", ">=", "<=" },
                                        new object[3] { aobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id
                                        , ldtPaymentStartDate, ldtPaymentDueDate}, null
                                     );
                                lclbPaymentAllocation =
                                    aobjServicePurchaseHeader.GetCollection<busServicePurchasePaymentAllocation>(ldtbList,
                                                                                                                 "icdoServicePurchasePaymentAllocation");
                            }

                            // If there are some applied amounts, then we need to go ahead and 
                            // loop through them and create the payment entries
                            if (lclbPaymentAllocation.Count > 0)
                            {
                                DateTime ldtOldPaymentDate = DateTime.MinValue;
                                int lintPaymentAllocIndex = 0;
                                bool lblnPrincipalBalanceAlreadyReAssigned = false;//PIR 8297
                                foreach (busServicePurchasePaymentAllocation lbusPaymentAllocation in lclbPaymentAllocation)
                                {
                                    lintPaymentAllocIndex++;
                                    int lintSerPurPaymentAllocationID = lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_allocation_id;
                                    decimal ldecPaymentAmount = lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.applied_amount;
                                    decimal ldecPaymentCarryoverAmount = ldecPaymentAmount;
                                    DateTime ldtPaymentDate = lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.payment_date;
                                    if (lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.remittance_id > 0)
                                    {
                                        lintReferenceID = lintRemittanceId = lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.remittance_id;
                                        lstrReferenceFormConst = busConstant.ReferenceFormConstRemittance;
                                    }
                                    if (lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.employer_payroll_detail_id > 0)
                                    {
                                        lintReferenceID =
                                            lintEmployerPayrollDetailID =
                                            lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.employer_payroll_detail_id;

                                        lstrReferenceFormConst = busConstant.ReferenceFormConstEmployerReporting;
                                    }

                                    lstrPaymentClass = aobjPassInfo.isrvDBCache.
                                        GetCodeDescriptionString(331, lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value);

                                    //Jeeva : Get the Prorated PSC , VSC for each payment and display in grid
                                    decimal ldecProratedPSC = 0;
                                    if (lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.prorated_psc > 0)
                                        ldecProratedPSC = lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.prorated_psc;

                                    decimal ldecProratedVSC = 0;
                                    if (lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.prorated_vsc > 0)
                                        ldecProratedVSC = lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.prorated_vsc;

                                    // Prem : Start changes made for Payoff value Calculation
                                    // Store the payment amount in a private variable so we can use for payoff amount calculation.
                                    ldecPaymentAmountForPayOff = ldecPaymentAmount;
                                    // End Changes 

                                    //Calculate Interest only for Payment Class PreTax, PostTax Installment
                                    // First we have to see if the amount paid by the user meets the interest amount,
                                    // we have deduct that amount from the carryover amount.
                                    decimal ldecInterstAmountForPayClass = 0;
                                    //if ((lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value == busConstant.Service_Purchase_Class_Employer_Installment_PostTax) ||
                                    //    (lbusPaymentAllocation.icdoServicePurchasePaymentAllocation.service_purchase_payment_class_value == busConstant.Service_Purchase_Class_Employer_Installment_PreTax))
                                    //{
                                    //    if (ldecInterestCarryOver > 0)
                                    //    {
                                    //        if (ldecInterestCarryOver <= ldecPaymentCarryoverAmount)
                                    //        {
                                    if (astrPaymentFrequency == busConstant.ServicePurchasePaymentFrequencyValueMonthly)
                                    {
                                        if (aintPaymentNo > 0)
                                        {
                                             //PIR 23513 First we have to see if the amount paid by the user meets the interest amount,
                                             // we have deduct that amount from the carryover amount.
                                            if (ldecInterestCarryOver > ldecPaymentCarryoverAmount)
                                            {
                                                ldecInterestCarryOver = ldecInterestCarryOver - ldecPaymentCarryoverAmount;
                                                ldecExpectedInterestAmount = ldecPaymentCarryoverAmount;
                                                ldecPaymentCarryoverAmount = 0;
                                            }
                                            else
                                            {
                                                ldecPaymentCarryoverAmount = ldecPaymentCarryoverAmount - ldecInterestCarryOver;
                                                ldecExpectedInterestAmount = ldecInterestCarryOver;//PIR 8297 
                                                ldecInterestCarryOver = 0;
                                            }
                                            ldecPrincipalCarryover = 0;
                                            adecPrincipleBalance = ldecPrincipleBalance;
                                        }
                                        else
                                        {
                                            ldecInterestCarryOver = 0;
                                            ldecExpectedInterestAmount = 0;
                                        }
                                    }
                                    else
                                    {
                                        //if (lintMonthsToBeAddedToDeriveNextDueDate > 0 && ldtPreviousPaymentDate != DateTime.MinValue && ldtPaymentDate != DateTime.MinValue)
                                        //    ldecExpectedInterestAmount = (ldecPrincipleBalance * ldecInterestRate / lintMonthsToBeAddedToDeriveNextDueDate) * DateDiffByMonthForServicePurchase(ldtPreviousPaymentDate, ldtPaymentDate);//PIR 20185
                                        //else
                                        //    ldecExpectedInterestAmount = 0;
                                        //ldecPaymentCarryoverAmount = ldecPaymentCarryoverAmount - ldecExpectedInterestAmount;
                                        //ldecInterestCarryOver = ldecExpectedInterestAmount;
                                        decimal ldecTotalInterestApplied;
                                        Collection<busServicePurchaseAmortizationSchedule> lSPASchedule = CalculateInterestAccrualAmount(ldtPreviousPaymentDate, ldtPaymentDate,
                                                                                                       ldecPrincipleBalance, aintPaymentNo, ldecInterstAmountForPayClass, ldtPaymentDueDate,
                                                                                                       aobjPassInfo, out ldecTotalInterestApplied);

                                        foreach (busServicePurchaseAmortizationSchedule lobjSPASchedule in lSPASchedule)
                                        {
                                            lclbAmortizationSchedule.Add(lobjSPASchedule);
                                        }
                                        //PIR 23513 First we have to see if the amount paid by the user meets the interest amount,
                                        // we have deduct that amount from the carryover amount.
                                        ldecInterestCarryOver += Math.Round(ldecTotalInterestApplied, 2, MidpointRounding.AwayFromZero);

                                        if (ldecInterestCarryOver > ldecPaymentCarryoverAmount)
                                        {
                                            ldecInterestCarryOver = ldecInterestCarryOver - ldecPaymentCarryoverAmount;
                                            ldecExpectedInterestAmount = ldecPaymentCarryoverAmount;
                                            ldecPaymentCarryoverAmount = 0;
                                        }
                                        else
                                        {
                                            ldecPaymentCarryoverAmount = ldecPaymentCarryoverAmount - ldecInterestCarryOver;
                                            ldecExpectedInterestAmount = ldecInterestCarryOver;//PIR 8297 
                                            ldecInterestCarryOver = 0;
                                        }
                                        //ldecPaymentCarryoverAmount = ldecPaymentCarryoverAmount - ldecExpectedInterestAmount;
                                        //ldecInterestCarryOver = ldecPaymentCarryoverAmount;
                                        ldecPrincipalCarryover = 0;
                                        adecPrincipleBalance = ldecPrincipleBalance;
                                    }
                                    //        }
                                    //        else
                                    //        {
                                    //            ldecInterestCarryOver = ldecInterestCarryOver - ldecPaymentCarryoverAmount;
                                    //            ldecExpectedInterestAmount = ldecPaymentCarryoverAmount;
                                    //            ldecPaymentCarryoverAmount = 0;
                                    //        }
                                    //    }
                                    //    else
                                    //    {
                                    //        ldecExpectedInterestAmount = 0;
                                    //    }

                                    ldecInterstAmountForPayClass = ldecExpectedInterestAmount;
                                    //}
                                    //else
                                    //{
                                    //    ldecInterestCarryOver = 0;
                                    //    //PROD ISSUE FIX : When down payment comes as second entry, payoff amount comes with interest amt even if they paid everything.
                                    //    ldecExpectedInterestAmount = 0;
                                    //}

                                    if (ldecPaymentCarryoverAmount > 0)
                                    {
                                        if (ldecPrincipalCarryover <= ldecPaymentCarryoverAmount)
                                        {
                                            ldecExpectedPrincipalAmount = ldecPaymentCarryoverAmount;
                                            //PIR 8297
                                            if(lblnPrincipalBalanceAlreadyReAssigned == false)
                                                adecPrincipleBalance = adecPrincipleBalance + ldecPrincipalCarryover - ldecPaymentCarryoverAmount;
                                            else
                                                adecPrincipleBalance = adecPrincipleBalance - ldecPaymentCarryoverAmount;
                                            ldecPrincipalCarryover = 0;
                                        }
                                        else
                                        {
                                            ldecPrincipalCarryover = ldecPrincipalCarryover - ldecPaymentCarryoverAmount;
                                            ldecExpectedPrincipalAmount = ldecPaymentCarryoverAmount;
                                            adecPrincipleBalance = adecPrincipleBalance + ldecPrincipalCarryover;
                                            //UAT PIR 2099 : Multiple payments came in the same month and first payment is less than expected principal
                                            //principal carryover got added multiple time which caues the issue.
                                            ldecPrincipalCarryover = 0;
                                        }
                                    }
                                    else
                                    {
                                        ldecExpectedPrincipalAmount = 0;
                                        if (IsPaymentMadeOnSamePeriod(ldtOldPaymentDate, ldtPaymentDate)) ldecPrincipalCarryover = 0;                                        
                                        adecPrincipleBalance = adecPrincipleBalance + ldecPrincipalCarryover;
                                        lblnPrincipalBalanceAlreadyReAssigned = true;//PIR 8297

                                    }

                                    //PIR 1848 - No. Of Payments keeps updating with different value rather then what we entered
                                    if (!(IsPaymentMadeOnSamePeriod(ldtOldPaymentDate, ldtPaymentDate)))
                                    {
                                        aintTotalNoOfFuturePayments++;
                                    }
                                    ldtOldPaymentDate = ldtPaymentDate;
                                    ldtPreviousPaymentDate = ldtPaymentDate;
									ldecPrincipleBalance = adecPrincipleBalance;
                                    aintPaymentNo++;
                                    if (astrPaymentFrequency != busConstant.ServicePurchasePaymentFrequencyValueMonthly && lintTotalNoofPayments == aintPaymentNo)
                                    {
                                        if (ldtPaymentDate.Day <= 15)
                                        {
                                            ldtPaymentDueDate = new DateTime(ldtPaymentDate.Year, ldtPaymentDate.Month, 15);
                                        }
                                        else
                                        {
                                            ldtPaymentDueDate = new DateTime(ldtPaymentDate.Year, ldtPaymentDate.Month, 15).AddMonths(1);
                                        }
                                    }
                                    lblnLastPaymentMade = true;
                                    // Create the schedule transaction for Remittance amount received.
                                    busServicePurchaseAmortizationSchedule lobjServicePurchaseAmortizationSchedule =
                                            CreateServicePurchaseAmortizationSchedule(System.DateTime.MinValue,
                                                                          ldtPaymentDate,
                                                                          aintPaymentNo, ldecPaymentAmount, 0,
                                                                          ldecExpectedPrincipalAmount,
                                                                          ldecInterstAmountForPayClass,
                                                                          adecPrincipleBalance, lintRemittanceId, lstrPaymentClass,
                                                                          ldecProratedPSC, ldecProratedVSC, lintEmployerPayrollDetailID,
                                                                          lstrReferenceFormConst, lintReferenceID, lintSerPurPaymentAllocationID, ldtPaymentDueDate, ldtPaymentDate, 0, aintPaymentNo.ToString());
                                    lclbAmortizationSchedule.Add(lobjServicePurchaseAmortizationSchedule);

                                    if ((aintNoOfPayments > 0) && (lintPaymentAllocIndex == lclbPaymentAllocation.Count))
                                    {
                                        //UAT PIR 762: If the Payment Already Made, for the Future payments, we need to generate Future payment schedule with the remaining balance.
                                        try
                                        {
                                            decimal ldecInterestForBalance = Math.Round(adecPrincipleBalance * ldecInterestRate + ldecInterestCarryOver, 2, MidpointRounding.AwayFromZero);
                                            ldecExpectedTotalAmount = (ldecInterestRate / (1 - (busGlobalFunctions.Power((1 + ldecInterestRate), -(aintNoOfPayments - aintTotalNoOfFuturePayments))))) * (adecPrincipleBalance + ldecInterestForBalance);
                                            adecExpectedInstallmentAmount = ldecExpectedTotalAmount;
                                        }
                                        catch (DivideByZeroException _exc)
                                        {
                                            //Do Nothing
                                        }
                                    }
                                } // End of For each for every payment allocation.
                            }
                            else
                            {
                                // Now recalculate the amount the member has paid based on the deductions in inteerest and principle

                                // only for the past installment payment we have to rollover those amount to be carried over
                                // to next month, in case of current installment payment we could allocate all the amounts
                                // to the current year bucket. (no rollover is needed till the payment due date is reached)
                                if (ldtPaymentDueDate < ldtCurrentDate)
                                {
                                    ldecExpectedPrincipalAmount = ldecExpectedPrincipalAmount - ldecPrincipalCarryover;
                                    ldecExpectedInterestAmount = ldecExpectedInterestAmount - ldecInterestCarryOver;

                                    ldecExpectedTotalAmount = ldecExpectedPrincipalAmount + ldecExpectedInterestAmount;
                                    adecPrincipleBalance = adecPrincipleBalance + ldecPrincipalCarryover;
                                }

                                bool lblnIsFuturePayment = true;

                                if (astrPaymentFrequency != busConstant.ServicePurchasePaymentFrequencyValueMonthly)
                                {
                                    decimal ldecTotalInterestApplied;
                                    Collection<busServicePurchaseAmortizationSchedule> lSPASchedule = CalculateInterestAccrualAmount(ldtPreviousPaymentDate, ldtPaymentDueDate,
                                                                                                      ldecPrincipleBalance, aintPaymentNo, 0, ldtPaymentDueDate,
                                                                                                      aobjPassInfo, out ldecTotalInterestApplied);

                                    foreach (busServicePurchaseAmortizationSchedule lobjSPASchedule in lSPASchedule)
                                    {
                                        lclbAmortizationSchedule.Add(lobjSPASchedule);
                                    }

                                    if (lintTotalNoofPayments > aintPaymentNo)
                                    {
                                        lblnIsFuturePayment = false;
                                        ldecInterestCarryOver += ldecTotalInterestApplied;
                                        ldtPreviousPaymentDate = ldtPaymentDueDate.AddMonths(-1);
                                    }
                                    else
                                    {
                                        ldecExpectedInterestAmount = Math.Round(ldecTotalInterestApplied, 2, MidpointRounding.AwayFromZero);
                                        if (adecExpectedInstallmentAmount > 0)
                                        {
                                            ldecExpectedTotalAmount = adecExpectedInstallmentAmount;
                                            ldecExpectedTotalAmount = Math.Min(ldecExpectedTotalAmount, (ldecPrincipleBalance + ldecExpectedInterestAmount));
                                        }
                                        ldecExpectedPrincipalAmount = ldecExpectedTotalAmount - ldecExpectedInterestAmount;
                                        adecPrincipleBalance = ldecPrincipleBalance - ldecExpectedPrincipalAmount;
                                        ldecPrincipalCarryover = 0;
                                    }
                                }
                                if (lblnIsFuturePayment)
                                {

                                    aintPaymentNo++;
                                    lblnLastPaymentMade = false;
                                    aintTotalNoOfFuturePayments++;
                                    ldtPreviousPaymentDate = ldtPaymentDueDate;
                                    if (aobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value != busConstant.Service_Purchase_Action_Status_Closed)
                                    {
                                        // Create the schedule transaction for Remittance amount received.
                                        busServicePurchaseAmortizationSchedule lobjServicePurchaseAmortizationSchedule =
                                                CreateServicePurchaseAmortizationSchedule(ldtPaymentDueDate,
                                                                              System.DateTime.MinValue,
                                                                              aintPaymentNo, 0, ldecExpectedTotalAmount,
                                                                              ldecExpectedPrincipalAmount,
                                                                              ldecExpectedInterestAmount,
                                                                              adecPrincipleBalance, lintRemittanceId, String.Empty, 0, 0, lintEmployerPayrollDetailID,
                                                                              lstrReferenceFormConst, lintReferenceID, 0, ldtPaymentDueDate, ldtPaymentDueDate, 0, aintPaymentNo.ToString());
                                        lclbAmortizationSchedule.Add(lobjServicePurchaseAmortizationSchedule);
                                    }
                                }
                            }
                            // Start Change
                            // Made by : Premkumar
                            // Made on : October 14th 2008
                            // Calculate PayOff Amount : PayOff amount should be calculated only when the current date 
                            // falls between the payment dates
                            adecPayOffAmount = 0.00M;

                            if (ldtPaymentStartDate <= ldtCurrentDate && ldtPaymentDueDate >= ldtCurrentDate)
                            {
                                // From current month onwards, interest should not be carried forward to next month schedule, so we set the 
                                // interest carryover amount to be equal to zero.
                                if (!lblnLastPaymentMade)
                                    ldecInterestCarryOver = 0;
                                if (astrPaymentFrequency == busConstant.ServicePurchasePaymentFrequencyValueOneTimeLumpSumAmt)
                                    ldecInterestCarryOver = 0;
                                adecPayOffAmount = (adecPrincipleBalance + ldecExpectedPrincipalAmount +
                                                   ldecExpectedInterestAmount + ldecInterestCarryOver) - ldecPaymentAmountForPayOff;
                            }
                            // End Change
                        }
                        else
                        {
                            // There is no interest carry over for the future payments that are yet to be made.
                            bool lblnIsFuturePayment = true;
                            if (astrPaymentFrequency != busConstant.ServicePurchasePaymentFrequencyValueMonthly)
                            {
                                decimal ldecTotalInterestApplied;
                                Collection<busServicePurchaseAmortizationSchedule> lSPASchedule = CalculateInterestAccrualAmount(ldtPreviousPaymentDate, ldtPaymentDueDate,
                                                                                                  ldecPrincipleBalance, aintPaymentNo, 0, ldtPaymentDueDate,
                                                                                                  aobjPassInfo, out ldecTotalInterestApplied);

                                foreach (busServicePurchaseAmortizationSchedule lobjSPASchedule in lSPASchedule)
                                {
                                    lclbAmortizationSchedule.Add(lobjSPASchedule);
                                }

                                if (lintTotalNoofPayments > aintPaymentNo)
                                {
                                    lblnIsFuturePayment = false;
                                    ldecInterestCarryOver += ldecTotalInterestApplied;
                                    ldtPreviousPaymentDate = ldtPaymentDueDate.AddMonths(-1);
                                }
                                else
                                {
                                    ldecExpectedInterestAmount = Math.Round(ldecTotalInterestApplied, 2, MidpointRounding.AwayFromZero);
                                    if (adecExpectedInstallmentAmount > 0)
                                    {
                                        ldecExpectedTotalAmount = adecExpectedInstallmentAmount;
                                        ldecExpectedTotalAmount = Math.Min(ldecExpectedTotalAmount, (ldecPrincipleBalance + ldecExpectedInterestAmount));
                                    }
                                    ldecExpectedPrincipalAmount = ldecExpectedTotalAmount - ldecExpectedInterestAmount;
                                    adecPrincipleBalance = ldecPrincipleBalance - ldecExpectedPrincipalAmount;
                                    ldecPrincipalCarryover = 0;
                                }
                            }
                            if (lblnIsFuturePayment)
                            {
                                ldecInterestCarryOver = 0;
                                aintPaymentNo++;
                                aintTotalNoOfFuturePayments++;
                                ldtPreviousPaymentDate = ldtPaymentDueDate;
                                if (aobjServicePurchaseHeader.icdoServicePurchaseHeader.action_status_value != busConstant.Service_Purchase_Action_Status_Closed)
                                {
                                    busServicePurchaseAmortizationSchedule lobjServicePurchaseAmortizationSchedule =
                                        CreateServicePurchaseAmortizationSchedule(ldtPaymentDueDate,
                                                                                  System.DateTime.MinValue,
                                                                                  aintPaymentNo, 0, ldecExpectedTotalAmount,
                                                                                  ldecExpectedPrincipalAmount,
                                                                                  ldecExpectedInterestAmount,
                                                                                  adecPrincipleBalance, lintRemittanceId, String.Empty, 0, 0, lintEmployerPayrollDetailID,
                                                                                  String.Empty, 0, 0, ldtPaymentDueDate, ldtPaymentDueDate, 0, aintPaymentNo.ToString());
                                    lclbAmortizationSchedule.Add(lobjServicePurchaseAmortizationSchedule);
                                }
                            }
                        }

                        if (aintNoOfPayments > 0)
                        {
                            if (aintTotalNoOfFuturePayments == aintNoOfPayments)
                            {
                                lblnContinueAmortizationSchedule = false;
                            }
                        }

                        //PIR 763 Value is Too Large or Small when Expected Payment Amount is less than Interest Amount
                        //For more than 10 schedule, if the expected installment amount is less than expected interest amount, we are not going generate the schedule 
                        //since its going to go in endless loop
                        if (aintTotalNoOfFuturePayments > 100)
                        {
                            ablnIsExpectedPaymetAmountTooSmall = IsExpectedPaymentAmountLessThanInterest(adecExpectedInstallmentAmount, ldecExpectedInterestAmount);
                            if ((ablnIsExpectedPaymetAmountTooSmall) || (aobjServicePurchaseHeader.iblnFromMssCheck180 && (aintTotalNoOfFuturePayments > 180 || aobjServicePurchaseHeader?.ibusPrimaryServicePurchaseDetail?.icdoServicePurchaseDetail?.total_time_to_purchase > 60)))
                            {
                                return new Collection<busServicePurchaseAmortizationSchedule>();
                            }
                        }
                    }
                }
            }

            bool lblnIsUpdated = false;
            //Assign the Header Object in each entry for getting the pay off amount
            //PIR 489 : Changes made by Jeeva S
            // In Each entry in the Schedule, the pay off amount should follow the following rules.
            // Principle Balance of Prior Month + Interest of Current Month).
            //For the Paid Entry, Payoff Amount should be Empty

            int lintCurrIndex = 0;
            foreach (busServicePurchaseAmortizationSchedule lobjServicePurchaseAmortizationSchedule in lclbAmortizationSchedule)
            {
                //Update the Expected Total Payment Amount with First Future Payment Entry.
                if ((!lblnIsUpdated)
                    && (lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.payment_amount == 0)
                    && (lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.expected_payment_amount != 0))
                {
                    adecCalculatedExpectedInstallmentAmount = lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.expected_payment_amount;
                    lblnIsUpdated = true;
                }

                lobjServicePurchaseAmortizationSchedule.ibusServicePurchaseHeader = aobjServicePurchaseHeader;

                busServicePurchaseAmortizationSchedule lobjPreviousSchedule = lclbAmortizationSchedule[0];
                if (lintCurrIndex > 0)
                    lobjPreviousSchedule = lclbAmortizationSchedule[lintCurrIndex - 1];


                if (lintCurrIndex == 0)
                {
                    lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.idecPayOffAmount =
                        lobjPreviousSchedule.icdoServicePurchaseAmortizationSchedule.principle_balance +
                        lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.principle_in_payment_amount;

                    lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.idecBeginningPrincipalBalance =
                        lobjPreviousSchedule.icdoServicePurchaseAmortizationSchedule.principle_balance +
                        lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.principle_in_payment_amount;

                }
                else
                {
                    lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.idecPayOffAmount =
                   lobjPreviousSchedule.icdoServicePurchaseAmortizationSchedule.principle_balance +
                   lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.interest_in_payment_amount;

                    lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.idecBeginningPrincipalBalance =
                        lobjPreviousSchedule.icdoServicePurchaseAmortizationSchedule.principle_balance;
                }

                lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.idecPayOffAmountActualValue =
                        lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.idecPayOffAmount;

                //PIR 919: If the Payment made or for Missed Payments, PayOff Amount is Zero
                if ((lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.payment_date != DateTime.MinValue)
                    || (lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.expected_payment_amount == 0))
                {
                    lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.idecPayOffAmount = 0;
                }

                lintCurrIndex++;
            }
            return lclbAmortizationSchedule;
        }
        
        // Mail dated Mon 8/22/2011 from Maik : 
        // If the multiple payment came on the same month and payment amounts are less then interest amount, principal carryover keep getting added.
        // If payment is made in the same period the carryover amount needs to include once.
        private static bool IsPaymentMadeOnSamePeriod(DateTime adtOldPaymentDate, DateTime adtCurrentPaymentDate)
        {
            if (adtOldPaymentDate == DateTime.MinValue) return false;
            DateTime ldtStartDate = DateTime.MinValue;
            DateTime ldtEndDate = DateTime.MinValue;
            if (adtOldPaymentDate.Day >= 15)
            {
                ldtStartDate = new DateTime(adtOldPaymentDate.Year, adtOldPaymentDate.Month, 15);
                DateTime ldtTempDate = adtOldPaymentDate.AddMonths(1);
                ldtEndDate = new DateTime(ldtTempDate.Year, ldtTempDate.Month, 14);
            }
            else
            {
                DateTime ldtTempDate = adtOldPaymentDate.AddMonths(-1);
                ldtStartDate = new DateTime(ldtTempDate.Year, ldtTempDate.Month, 15);
                ldtEndDate = new DateTime(adtOldPaymentDate.Year, adtOldPaymentDate.Month, 14);
            }
            return busGlobalFunctions.CheckDateOverlapping(adtCurrentPaymentDate, ldtStartDate, ldtEndDate);
        }
        //PIR 20185 - Calculate Month difference between payment dates
        public static int DateDiffByMonthForServicePurchase(DateTime adtStartDate, DateTime adtEndDate)
        {
            if (adtStartDate != DateTime.MinValue && adtStartDate.Day > 15)
                adtStartDate = adtStartDate.AddMonths(1);
            if (adtEndDate != DateTime.MinValue && adtEndDate.Day <= 15)
                adtEndDate = adtEndDate.AddMonths(-1);
            //Calculate Total Months Difference
            int lintTotalDueMonths = busGlobalFunctions.DateDiffByMonth(adtStartDate, adtEndDate);
            return lintTotalDueMonths;
        }

        public static Collection<busServicePurchaseAmortizationSchedule> CalculateInterestAccrualAmount(DateTime adtPreviousPaymentDate, DateTime adtPaymentDate, decimal adecPrincipleBalance, int aintPaymentNo,
            decimal adecInterstAmountForPayClass, DateTime adtPaymentDueDate, utlPassInfo aobjPassInfo, out decimal adecTotalInterestApplied)
        {
            Collection<busServicePurchaseAmortizationSchedule> lclbAmortizationSchedule = new Collection<busServicePurchaseAmortizationSchedule>();
            int lintMonthsDiff = DateDiffByMonthForServicePurchase(adtPreviousPaymentDate, adtPaymentDate);
            adecTotalInterestApplied = 0;

            DateTime ldtInterestAccrualDate = DateTime.MinValue;
            for (int i = 0; i < lintMonthsDiff; i++)
            {
                if (adtPreviousPaymentDate.Day < 15)
                    ldtInterestAccrualDate = new DateTime(adtPreviousPaymentDate.Year, adtPreviousPaymentDate.Month, 15);
                else
                {
                    ldtInterestAccrualDate = adtPreviousPaymentDate.AddMonths(1);
                    ldtInterestAccrualDate = new DateTime(ldtInterestAccrualDate.Year, ldtInterestAccrualDate.Month, 15);
                }
                if (adtPaymentDate != ldtInterestAccrualDate && adtPaymentDate > ldtInterestAccrualDate)
                {
                    decimal ldecInterestRate = busGlobalFunctions.GetCodeValueDetailsfromData2(busConstant.ServicePurchaseContractInterestCodeId, ldtInterestAccrualDate, aobjPassInfo);
                    // (Principle Balance * Interest Rate) / 12
                    decimal ldecInterestAccrualAmount = (adecPrincipleBalance * ldecInterestRate) / 12;

                    adecTotalInterestApplied += Math.Round(ldecInterestAccrualAmount, 2, MidpointRounding.AwayFromZero); 
                    busServicePurchaseAmortizationSchedule lobjSPAmortizationSchedule =
                      CreateServicePurchaseAmortizationSchedule(System.DateTime.MinValue,
                           System.DateTime.MinValue,
                           aintPaymentNo, 0, 0,
                           0,
                           adecInterstAmountForPayClass,
                           adecPrincipleBalance, 0, string.Empty,
                           0, 0, 0,
                           string.Empty, 0, 0, adtPaymentDueDate, ldtInterestAccrualDate, ldecInterestAccrualAmount);
                    lclbAmortizationSchedule.Add(lobjSPAmortizationSchedule);
                }
                adtPreviousPaymentDate = ldtInterestAccrualDate;
            }
            return lclbAmortizationSchedule;
        }
        /// <summary>
        /// Function to Calculate Amortization Schdule
        /// </summary>
        /// <param name="aobjServicePurchaseHeader"></param>
        /// <param name="aobjPassInfo"></param>
        /// <param name="ablnIsExpectedPaymetAmountTooSmall"></param>
        /// <returns></returns>
        public static Collection<busServicePurchaseAmortizationSchedule> CalculateAmortizationSchedule(busServicePurchaseHeader aobjServicePurchaseHeader,
                                                                                                        utlPassInfo aobjPassInfo,
                                                                                                        ref bool ablnIsExpectedPaymetAmountTooSmall)
        {
            // Get the principle amount for the amortizationschedule
            //if no payment made, minus the down payment that defined at header level
            //else go with the Higher Purchase Cost only
            decimal ldecPrincipleBalance = aobjServicePurchaseHeader.HigherPurchaseCost - aobjServicePurchaseHeader.icdoServicePurchaseHeader.down_payment;
            if (IsPaymentMade(aobjServicePurchaseHeader.icdoServicePurchaseHeader.service_purchase_header_id))
            {
                ldecPrincipleBalance = aobjServicePurchaseHeader.HigherPurchaseCost;
            }

            string lstrPaymentFrequency = aobjServicePurchaseHeader.icdoServicePurchaseHeader.payment_frequency_value;
            int lintNumberofPayments = aobjServicePurchaseHeader.icdoServicePurchaseHeader.number_of_payments;
            decimal ldecExpectedInstallmentAmount = aobjServicePurchaseHeader.icdoServicePurchaseHeader.expected_installment_amount;
            DateTime ldtExpectedPaymentStartDate = aobjServicePurchaseHeader.expected_payment_start_date;

            int lintTotalNoOfPayments = 0;
            decimal ldecCalculatedExpectedInstallmentAmount = 0;
            decimal ldecPayOffAmount = 0;
            int lintPaymentNo = 0;

            //Some case, we may not change any payment election. But, paymeny allocation might get changed. such time, we should always recalcuate based on installment amount
            //(Both amount and no of payment would have the value at that point of time.. so, resetting to zero to number of payments.
            if (ldecExpectedInstallmentAmount > 0)
                lintNumberofPayments = 0;

            Collection<busServicePurchaseAmortizationSchedule> lclbSchedule =
                ProcessAmortizationSchedule(aobjServicePurchaseHeader, ldecPrincipleBalance, lstrPaymentFrequency,
                                            lintNumberofPayments, ldecExpectedInstallmentAmount,
                                            ldtExpectedPaymentStartDate,
                                            false, 0, DateTime.MinValue,
                                            aobjPassInfo, ref ablnIsExpectedPaymetAmountTooSmall,
                                            ref lintTotalNoOfPayments,
                                            ref ldecCalculatedExpectedInstallmentAmount,
                                            ref ldecPayOffAmount, ref lintPaymentNo);

            aobjServicePurchaseHeader.iintTotalNoOfFuturePayments = lintTotalNoOfPayments;
            if (ldecCalculatedExpectedInstallmentAmount != 0)
                aobjServicePurchaseHeader.idecExpectedInstallmentAmount = ldecCalculatedExpectedInstallmentAmount;
            aobjServicePurchaseHeader.idecPayOffAmount = ldecPayOffAmount;
            return lclbSchedule;
        }

        /// <summary>
        /// Function to Calculate the What If Amortization Schdule. 
        /// Principle balance would be Current Payoff Amount
        /// </summary>
        /// <param name="aobjServicePurchaseHeader"></param>
        /// <param name="aobjPassInfo"></param>        
        /// <returns></returns>
        public static Collection<busServicePurchaseAmortizationSchedule> CalculateWhatIfAmortizationSchedule(busServicePurchaseHeader aobjServicePurchaseHeader,
                                                                                                        utlPassInfo aobjPassInfo)
        {
            //Load the Current Amortization Schedule if not ran before (We need to generate the Pa
            if (aobjServicePurchaseHeader.iclbServicePurchaseAmortizationSchedule == null)
                aobjServicePurchaseHeader.LoadAmortizationSchedule();

            decimal ldecPrincipleBalance = aobjServicePurchaseHeader.idecPayOffAmount;
            string lstrPaymentFrequency = aobjServicePurchaseHeader.istrWhatIfPaymentFrequency;
            int lintNumberofPayments = aobjServicePurchaseHeader.iintWhatIfNoOfPayments;
            decimal ldecExpectedInstallmentAmount = aobjServicePurchaseHeader.idecWhatIfExpextedInstallmentAmount;

            DateTime ldtExpectedPaymentStartDate = DateTime.Now;
            if (ldtExpectedPaymentStartDate.Day <= 15)
            {
                ldtExpectedPaymentStartDate = new DateTime(ldtExpectedPaymentStartDate.Year, ldtExpectedPaymentStartDate.Month, 15);
            }
            else
            {
                ldtExpectedPaymentStartDate = ldtExpectedPaymentStartDate.AddMonths(1);
                ldtExpectedPaymentStartDate = new DateTime(ldtExpectedPaymentStartDate.Year, ldtExpectedPaymentStartDate.Month, 15);
            }

            decimal ldecWhatIfPaymentAmount = aobjServicePurchaseHeader.idecWhatIfPaymentAmount;
            DateTime ldtWhatIfPaymentDate = DateTime.Now;

            bool lblnIsExpectedPaymetAmountTooSmall = false;
            int lintTotalNoOfPayments = 0;
            decimal ldecCalculatedExpectedInstallmentAmount = 0;
            decimal ldecPayOffAmount = 0;
            int lintPaymentNo = 0;

            Collection<busServicePurchaseAmortizationSchedule> lclbSchedule =
                ProcessAmortizationSchedule(aobjServicePurchaseHeader, ldecPrincipleBalance, lstrPaymentFrequency,
                                            lintNumberofPayments, ldecExpectedInstallmentAmount,
                                            ldtExpectedPaymentStartDate,
                                            true,
                                            ldecWhatIfPaymentAmount,
                                            ldtWhatIfPaymentDate,
                                            aobjPassInfo, ref lblnIsExpectedPaymetAmountTooSmall,
                                            ref lintTotalNoOfPayments,
                                            ref ldecCalculatedExpectedInstallmentAmount,
                                            ref ldecPayOffAmount, ref lintPaymentNo);

            return lclbSchedule;
        }

        private static bool IsExpectedPaymentAmountLessThanInterest(decimal adecExpectedInstallmentAmount, decimal adecInterestAmount)
        {
            //PIR 763 Value is Too Large or Small when Expected Payment Amount is less than Interest Amount
            bool lblnResult = false;
            if ((adecExpectedInstallmentAmount < adecInterestAmount) && (adecExpectedInstallmentAmount != 0))
            {
                lblnResult = true;
            }
            return lblnResult;
        }

        private static busServicePurchaseAmortizationSchedule CreateServicePurchaseAmortizationSchedule
            (
                DateTime adtDueDate, DateTime adtPaymentDate,
                int aintPaymentNumber, decimal adecPaymentAmount, decimal adecExpectedPaymentAmount,
                decimal adecPrincipalPaymentAmount, decimal adecInterestPaymentAmount, decimal adecPrincipleBalance, int aintRemittanceID,
                string astrPaymentClass, decimal adecProratedPSC, decimal adecProratedVSC, int aintEmployerPayrollDetailID,
                string astrReferenceFormConst, int aintReferenceID, int aintSerPurPaymentAllocationID, DateTime adtActualDueDate,
                DateTime adtInterestAccrualDate, decimal adecInterestAccrualAmount, string astrPaymentNumber = null
            )
        {
            busServicePurchaseAmortizationSchedule lobjServicePurchaseAmortizationSchedule = new busServicePurchaseAmortizationSchedule();
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule = new cdoServicePurchaseAmortizationSchedule();
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.payment_due_date = adtDueDate;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.payment_date = adtPaymentDate;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.payment_number = aintPaymentNumber;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.payment_amount = adecPaymentAmount;

            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.expected_payment_amount =
                Math.Round(adecExpectedPaymentAmount, 2, MidpointRounding.AwayFromZero);
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.principle_in_payment_amount
                = Math.Round(adecPrincipalPaymentAmount, 2, MidpointRounding.AwayFromZero);
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.interest_in_payment_amount =
                Math.Round(adecInterestPaymentAmount, 2, MidpointRounding.AwayFromZero);
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.principle_balance =
                Math.Round(adecPrincipleBalance, 2, MidpointRounding.AwayFromZero);

            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.iintRemittanceID = aintRemittanceID;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.iintEmployerPayrollDetailID = aintEmployerPayrollDetailID;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.istrPaymentClass = astrPaymentClass;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.prorated_psc = adecProratedPSC;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.prorated_vsc = adecProratedVSC;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.istrReferenceConst = astrReferenceFormConst;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.iintReferenceID = aintReferenceID;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.iintSerPurPaymentAllocationID = aintSerPurPaymentAllocationID;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.idtActualDueDate = adtActualDueDate;

            // PIR 22807
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.interest_accrual = Math.Round(adecInterestAccrualAmount, 2, MidpointRounding.AwayFromZero);
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.interest_accrual_date = adtInterestAccrualDate;
            lobjServicePurchaseAmortizationSchedule.icdoServicePurchaseAmortizationSchedule.istrPaymentNumber = astrPaymentNumber;
            return lobjServicePurchaseAmortizationSchedule;
        }

        /// <summary>
        /// Method to determine whethere the Down payment is paid or not
        /// This will be useful when we generate the amortization schedule
        /// </summary>
        /// <param name="aintServicePurchaseHeaderID"></param>
        /// <returns></returns>
        public static bool IsPaymentMade(int aintServicePurchaseHeaderID)
        {
            bool lblnResult = false;
            DataTable ldtbList = Select<cdoServicePurchasePaymentAllocation>(
             new string[1] { "service_purchase_header_id" },
             new object[1] { aintServicePurchaseHeaderID }, null, null);

            if (ldtbList.Rows.Count > 0)
                lblnResult = true;

            return lblnResult;
        }
        public static int CountofServicePurchasePaymentAllocation(int aintServicePurchaseHeaderID)
        {
            DataTable ldtbList = Select<cdoServicePurchasePaymentAllocation>(
             new string[1] { "service_purchase_header_id" },
             new object[1] { aintServicePurchaseHeaderID }, null, null);

            return ldtbList.Rows.Count;
        }
        public decimal idecOriginalPayOffAmount
        {
            get
            {
                // UAT PIR ID 2352
                return icdoServicePurchaseAmortizationSchedule.idecPayOffAmountActualValue - icdoServicePurchaseAmortizationSchedule.payment_amount;
            }
        }
    }
}