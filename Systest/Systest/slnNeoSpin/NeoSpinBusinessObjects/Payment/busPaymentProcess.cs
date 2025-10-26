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
    public class busPaymentProcess : busExtendBase
    { //Load Batch steps to be called from the payment batch
        public Collection<busPaymentScheduleStep> iclbBatchScheduleSteps { get; set; }
        public void LoadBatchScheduleSteps(int aintPaymentScheduleId)
        {
            DataTable ldtbScheduleSteps = Select("cdoPaymentSchedule.LoadMonthlyScheduleSteps", new object[1] { aintPaymentScheduleId });
            iclbBatchScheduleSteps = GetCollection<busPaymentScheduleStep>(ldtbScheduleSteps, "icdoPaymentScheduleStep");
        }
        //Load payment Step details
        public void LoadBatchScheduleStepRef(int aintPaymentScheduleId)
        {
            if (iclbBatchScheduleSteps == null)
                LoadBatchScheduleSteps(aintPaymentScheduleId);
            foreach (busPaymentScheduleStep lobjPaymentScheduleStep in iclbBatchScheduleSteps)
            {
                if (lobjPaymentScheduleStep.ibusPaymentStep == null)
                    lobjPaymentScheduleStep.LoadPaymentStep();
            }
        }        
        //Create Payment History Header for all the payee accounts considered for this payment process
        public int CreatePaymentHistoryHeader(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryHeader.createPaymentHistory", new object[3] { adtPaymentScheduleDate, 
                                        aintPaymentScheduleId,aintBatchScheduleId}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Create Payment History details for all the payee accounts considered for this payment process
        public int CreatePaymentHistoryDetail(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDetail.CreatePaymentHistoryDetail",
                new object[3] { aintPaymentScheduleId, adtPaymentScheduleDate, aintBatchScheduleId},
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Create Check History details for all the payee accounts considered for this payment process
        public int CreateCheckHistoryforPayees(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, DateTime adtNextBenefitPaymentDate, int aintBatchScheduleId)
        {
            //Backlog PIR-9804 removed NextBenefitPaymentDate from query and used PaymentDate to check ACH start date
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateCheckHistoryforPayees",
                new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Create Check History details for all the payee accounts considered for this payment process
        public int CreateACHHistoryforPayees(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, DateTime adtNextBenefitPaymentDate, int aintBatchScheduleId)
        {
            //Backlog PIR-9804 removed NextBenefitPaymentDate from query and used PaymentDate to check ACH start date
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateACHHistoryForPayees",
                new object[3] { aintPaymentScheduleId, adtPaymentScheduleDate, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Create Check History details for all the payee accounts considered for this payment process
        public int CreateRollOverCheckACHHistoryforPayees(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateRolloverCheckACHHistoryForPayees",
                new object[3] { aintPaymentScheduleId, adtPaymentScheduleDate, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //update payee account status to processed for all the refund payee accounts considered for this payment process
        public int UpdatePayeeAccountStatus(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {

            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountStatus.UpdatePayeeAccountStatus",
                new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Create Check History details for all the payee accounts considered for this payment process
        public int CreateDeposit(int aintDepositTapeId, int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {

            int lintrtn = DBFunction.DBNonQuery("cdoDeposit.CreateDeposit",
                new object[4] { aintDepositTapeId, aintPaymentScheduleId , adtPaymentScheduleDate, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Create Check History details for all the payee accounts considered for this payment process
        public int CreateRemittance(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoRemittance.CreateRemittance",
                        new object[2] { aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Create Check History details for all the payee accounts considered for this payment process
        public int GenerateGL(DateTime adtPaymentScheduleDate)
        {
            int lintrtn = 0;

            return lintrtn;
        }
        //monthly batch - update the plan participation status to withdrawn if the benfit type is 'REFUND' and or Retired if the benefit type is 'Retirement' or
        //Pre -retirement death
        public int UpdatePersonAccountStatus(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPersonAccount.UpdatePersonAccountPlanParticipationStatus",
                new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;            
        }
        //Adhoc batch  update the plan participation status to withdrawn if the benfit type is 'REFUND' 
        public int UpdatePersonAccountStatusAdhoc(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPersonAccount.UpdatePersonAccountPlanParticipationStatusAdhoc",
                new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Reduce the transferred amount from the member contributions for the member payee account   for all transfers
        //For DB to DC transfer ,transfer the whole amount to DC plan account
        public int UpdateRetirementContributionForAdhoc(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPersonAccountRetirementContribution.UpdatePersonAccountRetirementContributionForAdhoc",
                    new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Reduce the paid amount from the member contributions for the member payee account
        //not for benefit type disability
        public int UpdateRetirementContributionForMember(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPersonAccountRetirementContribution.UpdatePersonAccountRetirementContributionForMember",
                    new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Reduce the paid amount from the member contributions for the alternate payee's payee account
        //not for benefit type disability
        public int UpdateRetirementContributionForAltPayee(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPersonAccountRetirementContribution.UpdatePersonAccountRetirementContributionForAltPayee" ,
                new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        // Create Vendor Payment Summary for monthly batch
        public int CreateVendorPayments(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoProviderReportPayment.CreateVendarPaymentFileRecords",
                new object[2] { aintPaymentScheduleId , aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        // Create Vendor Payment Summary for adhoc batch
        public int CreateVendorPaymentsForAdhoc(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoProviderReportPayment.CreateVendarPaymentFileRecordsForAdhoc",
                new object[2] { aintPaymentScheduleId , aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Updating payee status to Receiving / Processed for all the retirement payee account considered for monthly paymen process
        public int UpdatePayeeAccountStatustoPaymentComplete(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountStatus.UpdatePayeeAccountStatusComplete",
                new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId , aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Updating Non-Taxable Amount, 
        public int UpdateNonTaxableAmount(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountPaymentItemType.UpdateNonTaxableAmount",
                new object[2] {adtPaymentScheduleDate, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }       
        //Updating FBO_CO in payment distribution detail table, 
        public int UpdateFBOCO(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.UpdateFBO",
                                     new object[2] { aintPaymentScheduleId, adtPaymentScheduleDate },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Updating Benefit Applications to Processed
        public int UpdateBenefitApplication(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoBenefitApplication.UpdateBenefitApplication",
                                       new object[2] { aintPaymentScheduleId , aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }

        //Updating Benefit DRO Applications to Processed
        public int UpdateBenefitDROApplication(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoBenefitDroApplication.UpdateDROApplication",
                                     new object[2] { aintPaymentScheduleId , aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Updating Benefit Calculations to Processed
        public int UpdateBenefitCalculations(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoBenefitCalculation.UpdateBenefitCalculation",
                                       new object[2] { aintPaymentScheduleId , aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }

        //Updating Benefit DRO Calculations to Processed
        public int UpdateBenefitDROCalcualtions(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoBenefitDroCalculation.UpdateDROCalculation",
                                   new object[2] { aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }

        //Create Payment History Header for all the payee accounts considered for this Adhoc payment process(Payee Accounts with benefit option as DB to DC transfer,DB to TFFR,DB to TIAA )
        public int CreateAdhocPaymentHistoryHeader(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryHeader.CreatePaymentHistoryAdhoc", new object[3] { adtPaymentScheduleDate, 
                                        aintPaymentScheduleId,aintBatchScheduleId}, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }

        //Create Payment History details for all the payee accounts considered for this payment process(Payee Accounts with benefit option as DB to DC transfer,DB to TFFR,DB to TIAA )
        public int CreateAdhocPaymentHistoryDetail(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDetail.CreatePaymentHistoryDetailAdhoc",
                new object[3] { aintPaymentScheduleId, adtPaymentScheduleDate,aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }


        //Create Payment check History details for all the members whos had made excess contributions(eg: Un allocated remittance amount cum deposit refund)
        public int CreateChkACHHistoryforExcessContrReturnAdhoc(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate ,int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateChkACHHistoryforExcessContrReturnAdhoc",
                 new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }

        //Update Adjustments with Payment history information FOR MONTHLY BATCH
        public int UpdateAdjustmentsWithPaymentHistoryRegular(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountRetroPayment.UpdateAdjustmentPaymentHistoryID",
                 new object[4] { adtPaymentScheduleDate, aintPaymentScheduleId, busConstant.PayeeAccountRetroPaymentOptionRegular, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }

        //Update Adjustments with Payment history information FOR ADHOC BATCH
        public int UpdateAdjustmentsWithPaymentHistorySpecial(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountRetroPayment.UpdateAdjustmentPaymentHistoryID",
                 new object[4] { adtPaymentScheduleDate, aintPaymentScheduleId, busConstant.PayeeAccountRetroPaymentOptionSpecial, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }

        //Update Deductions with Payment history information FOR ADHOC BATCH
        public int UpdateDeductionsWithPaymentHistorySpecial(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountDeductionRefund.UpdateDeductionPaymentHistoryDetails",
                 new object[3] { busConstant.PayeeAccountDeductionPaymentOptionSpecial, aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }

        //update payee account status  to processed for all the payee accounts considered for  adhoc payment process
        public int UpdatePayeeAccountStatusAdhoc(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {

            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountStatus.UpdatePayeeAccountStatusAdhoc",
                                  new object[3] { aintPaymentScheduleId, adtPaymentScheduleDate, aintBatchScheduleId },
                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }

        //Calculate interest fot DB to Dc transfer options and create payment items into payee account payment item table
        public int CalculateInterestAdhoc(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {

            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryHeader.CalculateInterest",
                                  new object[2] { adtPaymentScheduleDate, aintBatchScheduleId },
                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }     

        //Get available number checks
		//PIR 938
        public bool GetAvailableChecks(int aintPaymentScheduleID)
        {
            bool lblnChecksAvailable = true;
            
            DataTable ldtChecks = Select("cdoPaymentCheckBook.GetAvailableChecksByBenefitType", new object[1] { aintPaymentScheduleID });

            foreach (DataRow dr in ldtChecks.Rows)
            {
                int lintAvailableChecks = Convert.ToInt32(dr["NO_OF_CHECKS_AVAILABLE_AFTER"]);
                if(lintAvailableChecks < 0)
                {
                    lblnChecksAvailable = false;
                    break;
                }
            }
            return lblnChecksAvailable;
        }

        //Create GL entries for the premium payment for IBS
        public int CreateGLForPremiumPayment(int aintPaymentScheduleId,int aintBatchScheduleId)
        {

            int lintrtn = Convert.ToInt32(DBFunction.DBNonQuery("cdoGLTransaction.CreateGLFromPayrollForPremiumPayment", new object[2] { aintPaymentScheduleId, aintBatchScheduleId },
                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            return lintrtn;
        }
        //Update Rollover Details status to processed
        public int UpdateRolloverDetailStatusToProcessed(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountRolloverDetail.UpdateRolloverDetailStatusToProcessed",
                 new object[2] { aintPaymentScheduleId, aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Update Deduction Refund EndDate For Payment option - Regular
        public int UpdateDeductionRefundEndDateForRegularPayment(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountDeductionRefund.UpdateDeductionRefundEndDateForRegularPayment",
                 new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId , aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Update Deduction Refund EndDate For Payment option - Special
        public int UpdateDeductionRefundEndDateForSpecialPayment(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate,int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountDeductionRefund.UpdateDeductionRefundEndDateForSpecialPayment",
                 new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId , aintBatchScheduleId },
                 iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Update beneficiary flag when payee's minimum guarantee amount goes below zero
        public int UpdateBeneficiaryFlag(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPerson.UpdateBeneficiaryRequiredFlag",
                                     new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Update Deposit refund status to processed from the adhoc batch
        public int UpdateDepositRefundStatus(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoRemittance.UpdateDepositRefundStatusToProcessed",
                                     new object[2] { aintPaymentScheduleId, aintBatchScheduleId},
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }

        //Update end date for refund items from the  monthly or adhoc batch - As per Satya discussion
        public int UpdateRefundItems(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate,int aintBatchScheduleID)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountPaymentItemType.UpdateRefundItems",
                                     new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleID },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Update end date for refund items from the  monthly or adhoc batch - As per Satya discussion
        public int UpdateRefundItemsAdhoc(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleID)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountPaymentItemType.UpdateRefundItemAdhoc",
                                     new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId , aintBatchScheduleID },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        
         //Update Capital gain for refund items from the  monthly 
        public int UpdateCapitalGain(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleID)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPersonAccountRetirement.UpdateCapitalGain",
                                     new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId , aintBatchScheduleID },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }

        //Move All beneficiaries from DB Plan account to DC Plan Account for DB to DC transfer refund items from the  adhoc 
        public int UpdatePersonAccountBeneficiaryAdhoc(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleID)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPersonAccount.UpdatePersonAccountBeneficiaryAdhoc",
                                     new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleID },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Create Reissue Rollover Payments history header
        public int CreateReissueRolloverPaymentsHeader(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleID)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryHeader.CreateReissuePaymentsHeader",
                                     new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleID },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Create Reissue Rollover Payments history dettails
        public int CreateReissueRolloverPaymentDetails(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleID)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDetail.CreateReissuePaymentDetails",
                                     new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId , aintBatchScheduleID },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }       
     
        //Create Outstanding Distribution History Records
        public int CreateOutstandingHistoryRecords(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleID)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateOutstandingDistributionHistoryRecords",
                                     new object[2] { aintPaymentScheduleId, aintBatchScheduleID },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Update Old Distribution Id 
        public int UpdateOldDistributionId(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleID)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.UpdateOldDistribtuionID",
                                     new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId , aintBatchScheduleID },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Update Distribution From Reissue Approve To Reissued 
        public int UpdateDistributionFromReissueApproveToReissued(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleID)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.UpdateDistributionFromReissueApproveToReissued",
                                     new object[2] { adtPaymentScheduleDate, aintBatchScheduleID },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Update Recipient Name For Death Issue
        public int UpdateRecipientNameForDeathIssue(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.UpdateRecipientNameForDeathIssue",
                                     new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Calculate tax for the check which is reissued to member
        //public void CalculateTaxForRolloverAmountToPayee(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        //{
        //    DataTable ldtPaymentHistory = Select("cdoPaymentHistoryHeader.LoadProcessedPaymentHeaders", new object[2] { aintPaymentScheduleId, adtPaymentScheduleDate });
        //    Collection<busPaymentHistoryHeader> lclbPaymentDetails = GetCollection<busPaymentHistoryHeader>(ldtPaymentHistory, "icdoPaymentHistoryHeader");
        //    foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in lclbPaymentDetails)
        //    {
        //        lobjPaymentHistoryHeader.CalculateTaxForRolloverAmountToPayee();
        //    }
        //}
        //Correspondence
        public Collection<busPaymentHistoryHeader> iclbPaymentHistoryHeader { get; set; }
      
        //Create Vendor Receivables - provider report payment - negative entries
        public int CreateVendorReceivables(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate,int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateVendorReceivables",
                                     new object[3] { aintPaymentScheduleId, adtPaymentScheduleDate, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }   
        /// <summary>
        /// Method to cretate Premium Payments for IBS
        /// </summary>
        public int CreatePremiumPaymentsForIBS(int aintPaymentScheduleID, DateTime adtPaymentDate, int aintBatchScheduleId)
        {
            int lintrtn = 0;
            //Create Deposit Tape
            busDepositTape lobjDepositTape = new busDepositTape();
            lobjDepositTape.icdoDepositTape = new cdoDepositTape();
            lobjDepositTape.icdoDepositTape.status_value = busConstant.DepositTapeStatusValid;
            lobjDepositTape.icdoDepositTape.deposit_method_value = busConstant.DepositTapeMethodWireTransfer;
            lobjDepositTape.icdoDepositTape.deposit_date = adtPaymentDate;
            lobjDepositTape.icdoDepositTape.bank_account_value = busConstant.DepositTapeAccountInsurance;
            lobjDepositTape.icdoDepositTape.Insert();

            //Create Deposits
            lintrtn = CreateDeposit(lobjDepositTape.icdoDepositTape.deposit_tape_id, aintPaymentScheduleID,adtPaymentDate, aintBatchScheduleId);

            if (lobjDepositTape.iclbDeposit == null)
                lobjDepositTape.LoadDeposits();
            lobjDepositTape.icdoDepositTape.total_amount = lobjDepositTape.iclbDeposit.Select(o => o.icdoDeposit.deposit_amount).Sum();
            if (lobjDepositTape.icdoDepositTape.total_amount > 0)
                lobjDepositTape.icdoDepositTape.Update();

            //Create Remittances
            lintrtn = CreateRemittance(aintPaymentScheduleID, adtPaymentDate, aintBatchScheduleId);

            //Create GL
            lintrtn = CreateGLForPremiumPayment(aintPaymentScheduleID, aintBatchScheduleId);
            return lintrtn;
        }

        //Update sTatus to Satisfied
        public int UpdateRecoveryToSatified(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentRecovery.UpdateRecoveryToSatisfied",
                                     new object[2] { aintPaymentScheduleId, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }

        //Update Pension recievables
        public int UpdatePensionRecievables(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate,int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountPaymentItemType.UpdatePensionReceivables",
                                     new object[3] {adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        // Update Pension recievables final Payment
        public int UpdatePensionRecievablesfinalPayment(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountPaymentItemType.CreatePensionReceivableFinalPayment",
                                     new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Update Minimum Guarantee History
        public int UpdateMinimumGuaranteeHistory(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountMinimumGuaranteeHistory.UpdateMinimumGuaranteeHistory",
                                     new object[2] { aintPaymentScheduleId , aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        
        //Create GL
        public int CreateGL(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate,string astrSourceType, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoGLTransaction.CreateGLFromPayroll",
                                     new object[3] { astrSourceType, aintPaymentScheduleId, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        public int CreateGLAdHoc(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, string astrSourceType, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoGLTransaction.CreateGLFromPayrollAdhoc",
                                     new object[3] { astrSourceType, aintPaymentScheduleId, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Method to update benefit end date for refund payee account
        public int UpdateBenefitEndDateFromMonthly(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccount.UpdateBenefitEndDateFromMonthly",
                                             new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }
        //Method to update benefit end for payee accounts having transfers 
        public int UpdateBenefitEndDateFromAdhoc(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccount.UpdateBenefitEndDateFromAdhoc",
                                             new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }

        //Create Escheat Reissue  Payments history header
        public int CreateEscheatReissuepaymentsHeader(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryHeader.CreateEscheatReissuePaymentsHeader",
                                     new object[3] { aintPaymentScheduleId, adtPaymentScheduleDate, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //PIR 16219 - Create Escheat To State Payment history headers
        public int CreateEscheatToStatepaymentsHeader(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryHeader.CreateEscheatToStatePaymentsHeaders",
                                     new object[3] { aintPaymentScheduleId, adtPaymentScheduleDate, aintBatchScheduleId},
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Create Escheat Reissue Payments history dettails
        public int CreateEscheatReissueRolloverPaymentDetails(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDetail.CreateEscheatReissuePaymentDetails",
                                     new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        ////PIR 16219 - Create Escheat to state payment details
        public int CreateEscheatToStatePaymentDetails(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDetail.CreateEscheatToStatePaymentDetails",
                                     new object[3] { adtPaymentScheduleDate, aintPaymentScheduleId, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Update Distribution From Escheat Reissue Approve To Escheat Reissued 
        public int UpdateDistributionFromEscheatApproveToEscheatReissue(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.UpdateDistributionFromEscheatApproveToEscheatReissue",
                                     new object[2] { adtPaymentScheduleDate, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //method to update taxable amount with SSLI reduction amount
        public int UpdateTaxableAmountWithSSLReduction(int aintPaymentScheduleId, DateTime adtPaymentDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccountPaymentItemType.UpdateTaxableAmountWithSSLIReduction",
                                                new object[2] { adtPaymentDate.AddMonths(1), aintBatchScheduleId },
                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //Method to update benefit end date for term certain payee account
        public int UpdateBenefitEndDateFromMonthlyForTermCertain(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPayeeAccount.UpdateBenefitEndDateWithTermCertainEndDate",
                                             new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId },
                                                           iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            return lintrtn;
        }

        public DataTable LoadPayeeForMaintainRHICWorkflow(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate)
        {
            return Select("cdoPayeeAccount.LoadTermCertainPayees", new object[2] { adtPaymentScheduleDate, aintPaymentScheduleId });             
        }

        //method to delete back up data for current schedule id
        public int DeleteBackUpDataForCurrentScheduleId(int aintPaymentScheduleId)
        {
            int lintRtn = DBFunction.DBNonQuery("cdoPayeeAccount.DeletePrevBackUpForSamePaymentSchedule",
                                      new object[1] { aintPaymentScheduleId },
                                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintRtn;
        }

        //method to delete back up data for current schedule id
        public int AddGraduatedBenefit(int aintPaymentScheduleId, int aintBatchScheduleId)
        {
            int lintRtn = DBFunction.DBNonQuery("cdoPayeeAccountPaymentItemType.AddGraduatedBenefit",
                                      new object[2] { aintPaymentScheduleId, aintBatchScheduleId },
                                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintRtn;
        }
        //PIR 16219 - Payment history distribution creation for escheat to state
        public int CreateCheckHistoryforEscheatToState(int aintPaymentScheduleId, DateTime adtPaymentDate,int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.CreateCheckHistoryforEscheatToState",
                new object[3] { aintPaymentScheduleId, adtPaymentDate , aintBatchScheduleId },
                                      iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;            
        }
        //PIR 16219 - Escheated state payments distribution history creation and updating the current distribution status
        public int UpdateDistributionFromEscheatToStateToPaymentIssued(int aintPaymentScheduleId, DateTime adtPaymentDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.UpdateDistributionFromEscheatToStateToPaymentIssued",
                                     new object[2] { adtPaymentDate, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        //PIR 16219 - GL for escheat to state, debit using the escheated to state header's personid or orgid and credit using current org_id(code_id 52, code_value ETSO)
        public int CreateGLEscheatToState(int aintPaymentScheduleId, string astrSourceType, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoGLTransaction.CreateGLEscheatToState",
                                     new object[3] { astrSourceType, aintPaymentScheduleId, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        public int InsertIntoReportDataDCInDBDCSPEL(int aintPaymentScheduleId, DateTime adtPaymentDate, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("cdoPaymentHistoryDistribution.InsertIntoReportDataDCInDBDCSPEL",
                                     new object[3] { aintPaymentScheduleId,adtPaymentDate, aintBatchScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
        public void CalculateTaxForExcessTaxableContributionAmountToMember(int aintPaymentScheduleId, DateTime adtPaymentScheduleDate, int aintBatchScheduleId)
        {
            DataTable ldtPaymentHistory = Select("entPaymentHistoryHeader.LoadAllExcessContributionPaymentHistoryHeader", new object[1] { aintPaymentScheduleId });
            Collection<busPaymentHistoryHeader> lclbPaymentDetails = GetCollection<busPaymentHistoryHeader>(ldtPaymentHistory, "icdoPaymentHistoryHeader");
            foreach (busPaymentHistoryHeader lobjPaymentHistoryHeader in lclbPaymentDetails)
            {
                lobjPaymentHistoryHeader.LoadPaymentHistoryDetails();
                if(lobjPaymentHistoryHeader.iclbPaymentHistoryDetail.Count > 0)
                {
                    decimal ldecExcessTaxableAmount = lobjPaymentHistoryHeader.iclbPaymentHistoryDetail.Where(i => i.ibusPaymentItemType.icdoPaymentItemType.item_type_direction == 1 &&
                    i.ibusPaymentItemType.icdoPaymentItemType.taxable_item_flag == busConstant.Flag_Yes &&
                    i.ibusPaymentItemType.icdoPaymentItemType.item_type_code == busConstant.PaymentItemTypeCodeITEM501).Sum(o => o.icdoPaymentHistoryDetail.amount);

                    decimal ldecEEInterestForExcessContribution = 0;
                    if (lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.employer_payroll_header_id > 0)
                    {
                        busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader() { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                        lbusEmployerPayrollHeader.FindEmployerPayrollHeader(lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.employer_payroll_header_id);
                        DateTime ldtmPayPeriodDateForInterestCalculation = adtPaymentScheduleDate;
                        ldecEEInterestForExcessContribution = Convert.ToDecimal(DBFunction.DBExecuteScalar("entEmployerPayrollDetail.CalculateInterestForDetail", new object[6] { adtPaymentScheduleDate, ldtmPayPeriodDateForInterestCalculation, lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date, ldecExcessTaxableAmount, lobjPaymentHistoryHeader.icdoPaymentHistoryHeader.plan_id, "FALSE" }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                        int lintEEInterestPaymentItemTypeID = lobjPaymentHistoryHeader.GetPaymentItemTypeIDByItemCode(busConstant.PaymentItemTypeCodeITEM505);
                        lobjPaymentHistoryHeader.CreatePaymentHistoryDetail(lintEEInterestPaymentItemTypeID, Math.Round(ldecEEInterestForExcessContribution, 2, MidpointRounding.AwayFromZero), aintBatchScheduleId);
                    }
                    lobjPaymentHistoryHeader.CalculateTaxForExcessTaxableContrbutionAmountToMember(ldecExcessTaxableAmount + ldecEEInterestForExcessContribution, aintBatchScheduleId);
                }

            }
        }
        //Update Deposit refund status to processed from the Monthly batch
        public int UpdateDepositRefundStatusToProcessed(int aintPaymentScheduleId, int aintBatchScheduleId)
        {
            int lintrtn = DBFunction.DBNonQuery("entRemittance.UpdateDepositRefundStatusToProcessedFromMonthly",
                                     new object[2] { aintBatchScheduleId, aintPaymentScheduleId },
                                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            return lintrtn;
        }
    }
}
