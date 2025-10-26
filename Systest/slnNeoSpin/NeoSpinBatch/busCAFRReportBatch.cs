#region Using directives
using System;
using System.Data;
using System.Collections.Generic;
using System.Diagnostics;
using NeoSpin.CustomDataObjects;
using NeoSpin.BusinessObjects;
using System.Collections.ObjectModel;
using Sagitec.DataObjects;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using System.Collections;
using Sagitec.CorBuilder;
using System.IO;
using System.Text;
using Sagitec.ExceptionPub;
#endregion

namespace NeoSpinBatch
{
    public class busCAFRReportBatch : busNeoSpinBatch
    {
        //Property to contain Report batch request for current month
        public busCafrReportBatchRequest ibusReportRequest { get; set; }

        /// <summary>
        /// Main function to create reports
        /// </summary>
        public void CreateReports()
        {
            iobjPassInfo.BeginTransaction();
            istrProcessName = "Generate CAFR Report";
            //if (DateTime.Today.Day == DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month))
            //{
            DataTable ldtReportRequest = busBase.SelectWithOperator<cdoCafrReportBatchRequest>
                    (new string[2] { "status_value", "effective_date" },
                    new string[2] { "=", "<=" },
                    new object[2] { busConstant.CAFRReportStatusPending, DateTime.Today }, null);
            busBase lobjBase = new busBase();
            Collection<busCafrReportBatchRequest> lclbCAFRReportRequest =
                lobjBase.GetCollection<busCafrReportBatchRequest>(ldtReportRequest, "icdoCafrReportBatchRequest");
            foreach (busCafrReportBatchRequest lobjCAFRReportRequest in lclbCAFRReportRequest)
            {
                try
                {
                    ibusReportRequest = new busCafrReportBatchRequest();
                    ibusReportRequest = lobjCAFRReportRequest;
                    idlgUpdateProcessLog("Creating CAFR Report", "INFO", istrProcessName);
                    CreateCAFRReport();
                    ibusReportRequest.icdoCafrReportBatchRequest.status_value = busConstant.CAFRReportStatusProcessed;
                    ibusReportRequest.icdoCafrReportBatchRequest.Update();
                }
                catch (Exception ex)
                {
                    ExceptionManager.Publish(ex);
                    ibusReportRequest.icdoCafrReportBatchRequest.status_value = busConstant.CAFRReportStatusFailed;
                    ibusReportRequest.icdoCafrReportBatchRequest.Update();
                }
            }           
            //}
            ////else
            //    idlgUpdateProcessLog("CAFR/Contribution Master Reports not generated", "INFO", istrProcessName);
            iobjPassInfo.Commit();
        }
       
        /// <summary>
        /// Method to create CAFR report
        /// </summary>
        private void CreateCAFRReport()
        {
            try
            {
                //First Part of CAFR
                DateTime ldtReportDate = ibusReportRequest.icdoCafrReportBatchRequest.effective_date.GetFirstDayofNextMonth().AddDays(-1);
                DataTable ldtDBPensionPlan = busBase.Select("cdoPersonAccount.rptDBPensionPlans",
                    new object[1] { ldtReportDate });
                ldtDBPensionPlan.TableName = busConstant.ReportTableName;
                DataTable ldtDCPensionPlan = busBase.Select("cdoPersonAccount.rptDCPensionPlans",
                    new object[1] { ldtReportDate });
                ldtDCPensionPlan.TableName = busConstant.ReportTableName03;
                DataTable ldtSummary = busBase.Select("cdoPersonAccount.rptDBLifeGHDVSummary",
                    new object[1] { ldtReportDate });
                DataTable ldtDBSummary = new DataTable();
                ldtDBSummary = CreateReportResult(ldtSummary, busConstant.PlanRetirementTypeValueDB);
                ldtDBSummary.TableName = busConstant.ReportTableName02;
                DataTable ldtHISummary = new DataTable();
                ldtHISummary = CreateReportResult(ldtSummary, busConstant.CAFRReportHealthInsurance);
                ldtHISummary.TableName = busConstant.ReportTableName06;
                DataTable ldtLISummary = new DataTable();
                ldtLISummary = CreateReportResult(ldtSummary, busConstant.CAFRReportLifeInsurance);
                ldtLISummary.TableName = busConstant.ReportTableName13;
                DataTable ldtDentalSummary = new DataTable();
                ldtDentalSummary = CreateReportResult(ldtSummary, busConstant.CAFRReportDentalInsurance);
                ldtDentalSummary.TableName = busConstant.ReportTableName08;
                DataTable ldtVisionSummary = new DataTable();
                ldtVisionSummary = CreateReportResult(ldtSummary, busConstant.CAFRReportVisionInsurance);
                ldtVisionSummary.TableName = busConstant.ReportTableName10;
                DataTable ldtRHIC = busBase.Select("cdoPersonAccount.rptRHICProgram",
                    new object[1] { ldtReportDate });
                ldtRHIC.TableName = busConstant.ReportTableName04;
                DataTable ldtAllPlanDetails = busBase.Select("cdoPersonAccount.rptGHDV_LTC_EAP_DCDetails",
                    new object[1] { ldtReportDate });
                DataTable ldtHealth = new DataTable();
                ldtHealth = CreateReportResult(ldtAllPlanDetails, busConstant.CAFRReportHealthInsurance);
                ldtHealth.TableName = busConstant.ReportTableName05;
                DataTable ldtDental = new DataTable();
                ldtDental = CreateReportResult(ldtAllPlanDetails, busConstant.CAFRReportDentalInsurance);
                ldtDental.TableName = busConstant.ReportTableName07;
                DataTable ldtVision = new DataTable();
                ldtVision = CreateReportResult(ldtAllPlanDetails, busConstant.CAFRReportVisionInsurance);
                ldtVision.TableName = busConstant.ReportTableName09;
                DataTable ldtLTC = new DataTable();
                ldtLTC = CreateReportResult(ldtAllPlanDetails, busConstant.CAFRReportLTCEAPDCInsurance);
                ldtLTC.TableName = busConstant.ReportTableName14;
                DataTable ldtGroupLife = busBase.Select("cdoPersonAccount.rptGroupLife",
                    new object[1] { ldtReportDate });
                ldtGroupLife.TableName = busConstant.ReportTableName11;
                DataTable ldtGroupLifeTotalAmount = busBase.Select("cdoPersonAccount.rptTotalDollarsLI",
                    new object[1] { ldtReportDate });
                ldtGroupLifeTotalAmount.TableName = busConstant.ReportTableName12;
                DataTable ldtFlexComp = busBase.Select("cdoPersonAccount.rptFlexComp",
                    new object[1] { ldtReportDate });
                ldtFlexComp.TableName = busConstant.ReportTableName15;

                //Second Part of CAFR
                DataTable ldtDualPlanandEmp = busBase.Select("cdoPersonAccount.rptDualPlanAndEmp",
                    new object[1] { ldtReportDate });
                ldtDualPlanandEmp.TableName = busConstant.ReportTableName16;
                DataTable ldtDBPlanBrokenByEmp = busBase.Select("cdoPersonAccount.rptDBPlansBrokenbyEmp",
                    new object[1] { ldtReportDate });
                ldtDBPlanBrokenByEmp.TableName = busConstant.ReportTableName17;

                //Third Part of CAFR
                DataTable ldtBenefitExpStatistics = busBase.Select("cdoPaymentHistoryHeader.rptBenefitExpStatistics",
                    new object[1] { ldtReportDate });
                ldtBenefitExpStatistics.TableName = busConstant.ReportTableName18;

                //Fourth Part of CAFR
                DataTable ldtEmployerEnrolledPlans = busBase.Select("cdoOrganization.rptEmployersEnrldPlans",
                    new object[1] { ldtReportDate });
                ldtEmployerEnrolledPlans.TableName = busConstant.ReportTableName19;

                //Fifth Part of CAFR
                DataTable ldtParticipatingEmployers = busBase.Select("cdoOrganization.rptPrincipalEmployerDetails",
                    new object[1] { ldtReportDate });
                DataTable ldtTop10Employers = CreateReportResult(ldtParticipatingEmployers, busConstant.Flag_Yes);
                ldtTop10Employers.TableName = busConstant.ReportTableName20;
                DataTable ldtRemainingEmps = CreateReportResult(ldtParticipatingEmployers, busConstant.Flag_No);
                ldtRemainingEmps.TableName = busConstant.ReportTableName21;

                //Report Heading
                DataTable ldtReportTitle = new DataTable();
                ldtReportTitle.Columns.Add("SNO", Type.GetType("System.Int32"));
                ldtReportTitle.Columns.Add("REPORT_NAME", Type.GetType("System.String"));
                DataRow ldrReportTitle = ldtReportTitle.NewRow();
                ldrReportTitle["SNO"] = 1;
                ldrReportTitle["REPORT_NAME"] = busConstant.CAFRReportTitle + ibusReportRequest.icdoCafrReportBatchRequest.effective_date.ToString("MM/dd/yyyy");
                ldtReportTitle.Rows.Add(ldrReportTitle);
                ldtReportTitle.AcceptChanges();
                ldtReportTitle.TableName = busConstant.ReportTableName22;

                //Plan Employer Summary
                DataTable ldtPlanEmployerSummary = busBase.Select("cdoPersonAccount.rptPlanEmpSummary",
                    new object[1] { ldtReportDate });
                ldtPlanEmployerSummary.TableName = busConstant.ReportTableName23;

                DataSet ldsFinalCAFR = new DataSet();
                ldsFinalCAFR.Tables.Add(ldtDBPensionPlan.Copy());
                ldsFinalCAFR.Tables.Add(ldtDBSummary.Copy());
                ldsFinalCAFR.Tables.Add(ldtDCPensionPlan.Copy());
                ldsFinalCAFR.Tables.Add(ldtRHIC.Copy());
                ldsFinalCAFR.Tables.Add(ldtHealth.Copy());
                ldsFinalCAFR.Tables.Add(ldtHISummary.Copy());
                ldsFinalCAFR.Tables.Add(ldtDental.Copy());
                ldsFinalCAFR.Tables.Add(ldtDentalSummary.Copy());
                ldsFinalCAFR.Tables.Add(ldtVision.Copy());
                ldsFinalCAFR.Tables.Add(ldtVisionSummary.Copy());
                ldsFinalCAFR.Tables.Add(ldtGroupLife.Copy());
                ldsFinalCAFR.Tables.Add(ldtGroupLifeTotalAmount.Copy());
                ldsFinalCAFR.Tables.Add(ldtLISummary.Copy());
                ldsFinalCAFR.Tables.Add(ldtLTC.Copy());
                ldsFinalCAFR.Tables.Add(ldtFlexComp.Copy());
                ldsFinalCAFR.Tables.Add(ldtDualPlanandEmp.Copy());
                ldsFinalCAFR.Tables.Add(ldtDBPlanBrokenByEmp.Copy());
                ldsFinalCAFR.Tables.Add(ldtBenefitExpStatistics.Copy());
                ldsFinalCAFR.Tables.Add(ldtEmployerEnrolledPlans.Copy());
                ldsFinalCAFR.Tables.Add(ldtTop10Employers.Copy());
                ldsFinalCAFR.Tables.Add(ldtRemainingEmps.Copy());
                ldsFinalCAFR.Tables.Add(ldtReportTitle.Copy());
                ldsFinalCAFR.Tables.Add(ldtPlanEmployerSummary.Copy());
                CreateReport("rptCAFR.rpt", ldsFinalCAFR);
                idlgUpdateProcessLog("CAFR Report generated successfully", "INFO", istrProcessName);
            }
            catch (Exception ex)
            {
                idlgUpdateProcessLog("CAFR Report generation failed", "INFO", istrProcessName);
                throw ex;
            }



        }

        /// <summary>
        /// method to create child table from master table based on Indicator
        /// </summary>
        /// <param name="adtMasterTable">Master table</param>
        /// <param name="astrIndicator">Indicator to split</param>
        /// <returns>Child table</returns>
        public DataTable CreateReportResult(DataTable adtMasterTable, string astrIndicator)
        {
            IEnumerable<DataRow> lenuReportResult = from ldrRow in adtMasterTable.AsEnumerable()
                                                    where ldrRow.Field<string>("indicator") == astrIndicator
                                                    select ldrRow;
            return lenuReportResult.CopyToDataTable();
        }
    }
}
