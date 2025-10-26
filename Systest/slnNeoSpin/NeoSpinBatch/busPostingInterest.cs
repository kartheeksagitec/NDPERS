using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using System.IO;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.ExceptionPub;
namespace NeoSpinBatch
{
    class busPostingInterest : busNeoSpinBatch
    {

        private DataTable idtbEmployerPayrollDetail;
        private DataTable idtbEmployerPayrollHeader;
        private List<int> ilstHeaderID;
        private DataTable idtbPendingApprovalCalculations; //holding all pending for approval calculations

        /// <summary>
        /// Get the Account Balance and Account ID from the database to calculate interest for that account id.
        /// </summary>
        public void CreatePostingInterest()
        {
            istrProcessName = "Interest Posting Batch";
            idlgUpdateProcessLog("Interest Posting Batch started", "INFO", istrProcessName);
            busPostingInterestBatch lobjbusPostingInterest = new busPostingInterestBatch();
            // busPayeeAccountHelper.LoadPaymentItemType();      
            if (lobjbusPostingInterest.ibusDBCacheData == null)
                lobjbusPostingInterest.ibusDBCacheData = new busDBCacheData();
            if (lobjbusPostingInterest.ibusDBCacheData.idtbCachedPaymentItemType == null)
                lobjbusPostingInterest.ibusDBCacheData.idtbCachedPaymentItemType = busGlobalFunctions.LoadPaymentItemTypeCacheData(iobjPassInfo);

            busEmployerPayrollHeader lobjEmployerPayrollHeader = new busEmployerPayrollHeader();
            lobjEmployerPayrollHeader.LoadLastInterestBatchDate();

            //Load all the Benefit Account Details at once here.. PERFORMANCE Fix
            lobjbusPostingInterest.idtbBenefitAccountPersonData =
                busNeoSpinBase.Select("cdoBenefitRefundCalculation.GetBenefitAccountInfo", new object[0] { });

            DateTime ldtCurrentDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); //Get First day of current month         
            DataTable ldtbActivePersons = busNeoSpinBase.Select("cdoPersonAccountRetirementContribution.GetPersonAccountForInterest", new object[1] { ldtCurrentDateTime });

            ilstHeaderID = new List<int>();
            DateTime ldtePayPeriodDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1); // PROD PIR ID 5377
            idtbEmployerPayrollDetail = busNeoSpinBase.Select("cdoEmployerPayrollDetail.LoadRetirementDetail", new object[1] { ldtePayPeriodDate });
            idtbEmployerPayrollHeader = busNeoSpinBase.Select("cdoEmployerPayrollDetail.LoadRetirementEmployerPayrollHeader", new object[] { });
            lobjbusPostingInterest.idtbPendingApprovalCalculations = busNeoSpinBase.Select("cdoBenefitCalculation.LoadPendingApprovalCalculations", new object[1] { -999 }); //PIR 17140 - Loading all pending for approval calculations all at once for performance
			////PIR-17512 interest calculation
            DateTime ldteInterestEffectiveDate = (iobjSystemManagement.IsNotNull() && iobjSystemManagement.icdoSystemManagement.batch_date > DateTime.MinValue) ?
                                                    iobjSystemManagement.icdoSystemManagement.batch_date.AddMonths(-1) : DateTime.Today.AddMonths(-1);
            decimal ldecBalance = 0;
            decimal ldecERPreTaxAmount = 0;
            //decimal ldecERPreTaxAmountInterest = 0;
            decimal ldecInterestRate = 0;
            int lintPersonAcctId = 0;
            int lintTotalCount = 0;
            int lintProcessedCount = 0;
            bool lblnSuccess = false;
            while (true)
            {
                if (lintTotalCount == ldtbActivePersons.Rows.Count)
                    break;
                iobjPassInfo.BeginTransaction();
                try
                {
                    for (int i = 0; i < 100; i++)
                    {
                        if (lintTotalCount == ldtbActivePersons.Rows.Count)
                            break;
                        ldecBalance = Convert.ToDecimal(ldtbActivePersons.Rows[lintTotalCount]["ACCOUNT_BALANCE"].ToString());
                        ldecERPreTaxAmount = Convert.ToDecimal(ldtbActivePersons.Rows[lintTotalCount]["PRE_TAX_ER_AMOUNT"].ToString());
                        lintPersonAcctId = Convert.ToInt32(ldtbActivePersons.Rows[lintTotalCount]["PERSON_ACCOUNT_ID"].ToString());
                        int lintPersonID = Convert.ToInt32(ldtbActivePersons.Rows[lintTotalCount]["PERSON_ID"].ToString());
                        int lintPlanID = Convert.ToInt32(ldtbActivePersons.Rows[lintTotalCount]["PLAN_ID"].ToString());
                        ldecInterestRate = busInterestCalculationHelper.CalculateInterest(ldecBalance, lintPlanID, ldteInterestEffectiveDate);  //PIR-17512 interest calculation
                        //ldecERPreTaxAmountInterest = busEmployerReportHelper.CalculateEmployerInterest(lobjEmployerPayrollHeader.idtLastInterestPostingDate,
                        //    ldtCurrentDateTime.AddDays(-1), 0.0m, ldecERPreTaxAmount, iobjPassInfo, DateTime.MinValue, lintPlanID);
						int lintPostedIntRetirementContributionId = 0;
                        lintPostedIntRetirementContributionId = lobjbusPostingInterest.PostInterestRate(ldecInterestRate, 0, lintPersonAcctId,
                                                                new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).
                                                                    AddDays(-1), false, 0,
                                                                iobjSystemManagement.icdoSystemManagement.batch_date, string.Empty);
                        if (ldecInterestRate > 0.00m)
                        {
                            lobjbusPostingInterest.AdjustPayeeAccountForAdditionalContributions(lintPersonAcctId, ldecInterestRate, 0, lintPostedIntRetirementContributionId,iobjBatchSchedule.batch_schedule_id);
                        }
                        // PIR 26010  
                        //UpdateEmployerPayroll(lintPersonID, lintPlanID);
                        lintTotalCount++;
                        lintProcessedCount++;
                    }
                    idlgUpdateProcessLog(lintProcessedCount.ToString() + " Records Processed of " + ldtbActivePersons.Rows.Count.ToString(), "INFO", istrProcessName);
                    iobjPassInfo.Commit();
                }
                catch (Exception e)
                {
                    ExceptionManager.Publish(e);
                    idlgUpdateProcessLog("Interest Posting Batch Failed at Processing " + lintPersonAcctId.ToString(), "ERR", istrProcessName);
                    iobjPassInfo.Rollback();
                    break;
                }
                lblnSuccess = true;
            }
            DataTable ldtbInterestSummary = busBase.Select("cdoPersonAccountRetirementContribution.rptInterestUpdateSummary",
                new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            if (ldtbInterestSummary.Rows.Count > 0)
            {
                //create report for Insufficient report details
                CreateReport("rptInterestUpdateSummary.rpt", ldtbInterestSummary);
                idlgUpdateProcessLog("Interest Posting report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }
            if (lblnSuccess)
            {
                idlgUpdateProcessLog("Interest Posting Batch ended", "INFO", istrProcessName);
            }            
        }

        public void UpdateEmployerPayroll(int aintPersonID, int aintPlanID)
        {
            DataTable ldtbResults = idtbEmployerPayrollDetail.AsEnumerable().Where(o => o.Field<int>("PERSON_ID") == aintPersonID && o.Field<int>("PLAN_ID") == aintPlanID).AsDataTable();
            bool lblnSetInterestFlag = false;
            foreach (DataRow ldtrDetail in ldtbResults.Rows)
            {
                lblnSetInterestFlag = false;
                busEmployerPayrollDetail lobjDetail = new busEmployerPayrollDetail
                {
                    icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail(),
                    ibusEmployerPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() }
                };
                lobjDetail.icdoEmployerPayrollDetail.LoadData(ldtrDetail);
                DataTable ldtbHeader = idtbEmployerPayrollHeader.AsEnumerable().Where(o => 
                                    o.Field<int>("EMPLOYER_PAYROLL_HEADER_ID") == lobjDetail.icdoEmployerPayrollDetail.employer_payroll_header_id).AsDataTable();
                if (ldtbHeader.Rows.Count > 0)
                {
                    lobjDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldtbHeader.Rows[0]);
                    if (lobjDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.interest_waiver_flag != busConstant.Flag_Yes)
                    {
                        lblnSetInterestFlag = true;
                        if (!ilstHeaderID.Contains(lobjDetail.icdoEmployerPayrollDetail.employer_payroll_header_id))
                        {
                            // Update the Employer Payroll header to Review only once.
                            ilstHeaderID.Add(lobjDetail.icdoEmployerPayrollDetail.employer_payroll_header_id);
                            if (lobjDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value != busConstant.StatusReview)
                            {
                                lobjDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.status_value = busConstant.StatusReview;
                                lobjDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.Update();
                            }
                        }
                    }
                }

                // Updates the Employer Payroll detail to Review.
                if (lblnSetInterestFlag)
                {
                    lobjDetail.icdoEmployerPayrollDetail.status_value = busConstant.StatusReview;
                    lobjDetail.icdoEmployerPayrollDetail.recalculate_interest_flag = busConstant.Flag_Yes;
                    if (lobjDetail.icdoEmployerPayrollDetail.suppress_warnings_flag == busConstant.Flag_Yes)
                        lobjDetail.icdoEmployerPayrollDetail.suppress_warnings_flag = busConstant.Flag_No;
                    lobjDetail.icdoEmployerPayrollDetail.Update();

                    // Inserts Employer Payroll Detail Error
                    //busEmployerPayrollDetailError lobjError = new busEmployerPayrollDetailError
                    //{
                    //    icdoEmployerPayrollDetailError = new cdoEmployerPayrollDetailError
                    //    {
                    //        employer_payroll_detail_id = lobjDetail.icdoEmployerPayrollDetail.employer_payroll_detail_id,
                    //        message_id = 1175
                    //    }
                    //};
                    //lobjError.icdoEmployerPayrollDetailError.Insert();
                }
            }
        }
    }
}