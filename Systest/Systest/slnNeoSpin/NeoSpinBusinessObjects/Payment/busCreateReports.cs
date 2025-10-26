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

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busCreateReports : busExtendBase
    {
        public DataTable TrialMonthlyBenefitPaymentbyItemReport(DateTime adtPaymentDate, bool ablnMnthly)
        {
            if(ablnMnthly)
                return Select("cdoPayeeAccount.TrialMonthlyBenefitPaymentbyItemReport", new object[1] { adtPaymentDate });
            else
                return Select("cdoPayeeAccount.TrialMonthlyBenefitPaymentbyItemReportAdhoc", new object[1] { adtPaymentDate });
        }

        public DataTable TrialNewRetireeDetailReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialNewRetireeDetailReport", new object[1] { adtPaymentDate });
        }

        public DataTable TrialReinstatedRetireeDetailReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialReinstatedRetireeDetailReport", new object[1] { adtPaymentDate });
        }

        public DataTable TrialClosedorSuspendedPayeeAccountReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialClosedorSuspendedPayeeAccountReport", new object[1] { adtPaymentDate });
        }

        public DataTable TrialRetirementOptionSummaryReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialRetirementOptionSummaryReport", new object[1] { adtPaymentDate });
        }

        public DataSet TrialBenefitPaymentChangeReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            DataTable ldtBatchDetails = Select("cdoPayeeAccount.TrialBenefitPaymentChangeMainReport", new object[2] { adtPaymentDate, busConstant.Flag_Yes });
            DataTable ldtPersonDetails = Select("cdoPayeeAccount.TrialBenefitPaymentChangeMainReport", new object[2] { adtPaymentDate, busConstant.Flag_No });
            DataTable ldtSummaryPersonDetails = Select("cdoPayeeAccount.TrialBenefitPaymentChangeMainReport", new object[2] { adtPaymentDate, busConstant.BenefitPaymentChangeGroupby });
            DataSet ldsBenefitPaymentChangeReport = new DataSet();
            ldtBatchDetails.TableName = busConstant.ReportTableName;
            ldtPersonDetails.TableName = busConstant.ReportTableName02;
            ldtSummaryPersonDetails.TableName = busConstant.ReportTableName03;
            decimal ldecGrossBeforeAmount = 0.0M, ldecGrossAfterAmount = 0.0M, ldecGrossDiffAmount = 0.0M,
                ldecNetBeforeAmount = 0.0M, ldecNetAfterAmount = 0.0M, ldecNetDiffAmount = 0.0M;
            foreach (DataRow dr in ldtSummaryPersonDetails.Rows)
            {
                if (dr["ITEM_TYPE_DIRECTION"] != DBNull.Value && Convert.ToInt32(dr["ITEM_TYPE_DIRECTION"]) == 1)
                {
                    ldecGrossBeforeAmount += dr["BEFORE_AMOUNT"] == DBNull.Value ? 0.0M : Convert.ToDecimal(dr["BEFORE_AMOUNT"]);
                    ldecGrossAfterAmount += dr["AFTER_AMOUNT"] == DBNull.Value ? 0.0M : Convert.ToDecimal(dr["AFTER_AMOUNT"]);
                    ldecGrossDiffAmount += dr["DIFFERENCE_AMOUNT"] == DBNull.Value ? 0.0M : Convert.ToDecimal(dr["DIFFERENCE_AMOUNT"]);
                }
                ldecNetBeforeAmount += dr["BEFORE_AMOUNT"] == DBNull.Value ? 0.0M : Convert.ToDecimal(dr["BEFORE_AMOUNT"]);
                ldecNetAfterAmount += dr["AFTER_AMOUNT"] == DBNull.Value ? 0.0M : Convert.ToDecimal(dr["AFTER_AMOUNT"]);
                ldecNetDiffAmount += dr["DIFFERENCE_AMOUNT"] == DBNull.Value ? 0.0M : Convert.ToDecimal(dr["DIFFERENCE_AMOUNT"]);
            }
            DataTable ldtGrossNetDetails = ldtSummaryPersonDetails.Clone();
            ldtGrossNetDetails.TableName = busConstant.ReportTableName04;
            DataRow ldrGross = ldtGrossNetDetails.NewRow();
            ldrGross["ITEM_TYPE"] = "Gross Amount";
            ldrGross["BEFORE_AMOUNT"] = ldecGrossBeforeAmount;
            ldrGross["AFTER_AMOUNT"] = ldecGrossAfterAmount;
            ldrGross["DIFFERENCE_AMOUNT"] = ldecGrossDiffAmount;
            ldtGrossNetDetails.Rows.Add(ldrGross);
            DataRow ldrNet = ldtGrossNetDetails.NewRow();
            ldrNet["ITEM_TYPE"] = "Net Amount";
            ldrNet["BEFORE_AMOUNT"] = ldecNetBeforeAmount;
            ldrNet["AFTER_AMOUNT"] = ldecNetAfterAmount;
            ldrNet["DIFFERENCE_AMOUNT"] = ldecNetDiffAmount;
            ldtGrossNetDetails.Rows.Add(ldrNet);
            ldtGrossNetDetails.AcceptChanges();

            ldsBenefitPaymentChangeReport.Tables.Add(ldtBatchDetails.Copy());
            ldsBenefitPaymentChangeReport.Tables.Add(ldtPersonDetails.Copy());
            ldsBenefitPaymentChangeReport.Tables.Add(ldtSummaryPersonDetails.Copy());
            ldsBenefitPaymentChangeReport.Tables.Add(ldtGrossNetDetails.Copy());
            
            return ldsBenefitPaymentChangeReport;
        }

        public DataTable TrialNonMonthlyPaymentDetailReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialNonMonthlyPaymentDetailReport", new object[1] { adtPaymentDate });
        }

        public DataTable TrialNonMonthlyPaymentDetailReportAdHoc(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialNonMonthlyPaymentDetailReportAdhoc", new object[1] { adtPaymentDate });
        }
      
        public DataTable TrialMonthlyBenefitPaymentSummaryReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.TrialMonthlyBenefitPaymentSummaryReport", new object[2] { adtPaymentDate, aintPaymentScheduleID });
        }

        public DataTable FinalMonthlyBenefitPaymentbyItemReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalMonthlyBenefitPaymentbyItemReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable TrialVendorPaymentSummary(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialVendorPaymentSummary", new object[1] { adtPaymentDate });
        }


        public DataTable TrialVendorPaymentSummaryAdHoc(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.TrialVendorPaymentSummaryAdHoc", new object[1] { adtPaymentDate });
        }

        public DataTable FinalVendorPaymentSummary(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalVendorPaymentSummary", new object[1] { aintPaymentScheduleID });
        }

        public DataTable FinalDuesWithholdingReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            //Backlog PIR 13031 - We have used same query for reports (Payment process, online).
            return Select("cdoPayeeAccount.FinalDuesWithholdingReport", new object[1] { adtPaymentDate });
        }

        public DataTable FinalDuesWithholdingReportDisburse(int aintBatchRequestID)
        {
            //Backlog PIR 13031
            return Select("cdoPayeeAccount.FinalDuesWithholdingReportDisburse", new object[1] { aintBatchRequestID });
        }

        public DataTable TrialMinimumGuaranteeChangeSummaryReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.TrialMinimumGuaranteeChangeSummaryReport", new object[1] { adtPaymentDate });
        }

        public DataTable FinalChildSupportReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalChildSupportReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable FinalIRSLimitReport(DateTime adtPaymentDate)
        {
            return Select("cdoPayeeAccount.FinalIRSLimitReport", new object[1] { adtPaymentDate });
        }

        public DataTable FinalMasterPaymentReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalMasterPaymentReport", new object[1] {aintPaymentScheduleID });
        }

        public DataTable FinalACHRegisterReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalACHRegisterReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable FinalCheckRegisterReport(DateTime adtPaymentDate, int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.FinalCheckRegisterReport", new object[1] { aintPaymentScheduleID });
        }

        public DataTable CheckRegisterReportSummary(int aintPaymentScheduleID)
        {
            return Select("cdoPayeeAccount.CheckRegisterReportSummary", new object[1] { aintPaymentScheduleID });
        }
        public DataTable TFFRorTIAACREFReport(string astrChoice, int aintPaymentScheduleID)
        {
            return Select("cdoBenefitApplication.rptTFFRTransferReport", new object[2] { astrChoice, aintPaymentScheduleID });
        }
        
        public DataTable TFFRSalaryRecords(int aintPaymentScheduleID)
        {
            return Select("cdoBenefitApplication.rptTFFRSalaryRecords", new object[1] { aintPaymentScheduleID });
        }

        public DataTable MultipleACHOrCheckReport(int aintChoice, int aintPaymentScheduleID)
        {
            return Select("cdoPaymentHistoryDistribution.LoadPayeeWithMultipleACHorCheck", new object[2] { aintChoice, aintPaymentScheduleID });
        }

        public DataTable TrialRefundsWithPayrollAdjustments(int aintPaymentScheduleId, DateTime adtePaymentDate, bool ablnIsMonthly)
        {
            return ablnIsMonthly ? Select("cdoPaymentSchedule.LoadTrialMonthlyRefundsWithPayrollAdjustments", new object[1] { adtePaymentDate }) :
                                    Select("cdoPaymentSchedule.LoadTrialAdhocRefundsWithPayrollAdjustments", new object[1] { adtePaymentDate });
        }
        public DataTable LoadPayeeWithMultiplePaymentsReport(int aintPaymentScheduleID)
        {
            return Select("entPaymentHistoryDistribution.LoadPayeeWithMultiplePayments", new object[1] { aintPaymentScheduleID });
        }

        public DataTable LoadEmpowerSpecialElectionPaymentsReport(DateTime adtePaymentDate)
        {
            return Select("entPaymentSchedule.LoadTransferPaymentDetails", new object[1] { adtePaymentDate });
        }
    }
}
