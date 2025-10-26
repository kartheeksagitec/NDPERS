using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using NeoSpin.BusinessObjects;
using System.IO;
using Sagitec.CustomDataObjects;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;

namespace NeoSpinBatch
{
    class busReloadInsurance : busNeoSpinBatch
    {
        DataTable idtResultTable;

        /// <summary>
        /// Reloading the Payroll Detail
        /// </summary>
        public void ReloadInsurance()
        {
            istrProcessName = iobjBatchSchedule.step_name;
            bool lblnInTransaction = false;

            busPostingInsuranceBatch ibusPostInsuranceBatch = new busPostingInsuranceBatch();
            DataTable idtbPayrollHeader;
            DataTable idtbOrgPlan;

            //Reload the Current Month Regular Insurance File for all the Employers
            idtbPayrollHeader = busReloadInsuranceBatch.Select("cdoEmployerPayrollHeader.LoadInsuranceHeaderToReload", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });

            idlgUpdateProcessLog("Reload Insurance Batch Started", "INFO", iobjBatchSchedule.step_name);

            idlgUpdateProcessLog("Loading All Active Providers", "INFO", iobjBatchSchedule.step_name);
            //Loading Complete Active Provider Org Plan List (Optimization Purpose)
            ibusPostInsuranceBatch.LoadActiveProviders();

            idlgUpdateProcessLog("Loading All Org Plan Providers", "INFO", iobjBatchSchedule.step_name);
            //Loading Complete Active Provider Org Plan List (Optimization Purpose)
            ibusPostInsuranceBatch.LoadAllOrgPlanProviders();

            idlgUpdateProcessLog("Loading DB Cache Data", "INFO", iobjBatchSchedule.step_name);
            //Loading the DB Cache Data
            ibusPostInsuranceBatch.LoadDBCacheData();

            /* //RA
            idlgUpdateProcessLog("Loading Life Option Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
            //Loading the Life Option Data by Org (Optimization)
            ibusPostInsuranceBatch.LoadLifeOptionData();

            idlgUpdateProcessLog("Loading EAP History Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
            //Loading the EAP History (Optimization)
            ibusPostInsuranceBatch.LoadEAPHistory();

            idlgUpdateProcessLog("Loading GHDV History Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
            //Loading the GHDV History (Optimization)
            ibusPostInsuranceBatch.LoadGHDVHistory();

            idlgUpdateProcessLog("Loading LIFE History Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
            //Loading the Life History (Optimization)
            ibusPostInsuranceBatch.LoadLifeHistory();
            */

            idlgUpdateProcessLog("Loading All Plans", "INFO", iobjBatchSchedule.step_name);
            ibusPostInsuranceBatch.LoadAllPlans();

            //Get the List of Employer Participated in insurance org plans    
            idlgUpdateProcessLog("Loading All Org Plans", "INFO", iobjBatchSchedule.step_name);
            idtbOrgPlan = busNeoSpinBase.Select("cdoEmployerPayrollHeader.GetActiveInsuranceOrgPlans", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });

            //Initialize the Report Table
            idtResultTable = GetDataTable();

            foreach (DataRow adrRow in idtbPayrollHeader.Rows)
            {
                busReloadInsuranceBatch lobjReloadInsurance = new busReloadInsuranceBatch();

                var lobjPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                lobjPayrollHeader.icdoEmployerPayrollHeader.LoadData(adrRow);
                lobjPayrollHeader.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lobjPayrollHeader.ibusOrganization.icdoOrganization.LoadData(adrRow);

                if (!Convert.IsDBNull(adrRow["PAYROLL_STATUS_VALUE"]))
                {
                    lobjPayrollHeader.icdoEmployerPayrollHeader.status_value = adrRow["PAYROLL_STATUS_VALUE"].ToString();
                }

                idlgUpdateProcessLog(" Payroll Header ID : " + lobjPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id + ". ",
                       "INFO", iobjBatchSchedule.step_name);
                try
                {
                    if (!lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.BeginTransaction();
                        lblnInTransaction = true;
                    }

                    //Delete the Existing Detail Records
                    lobjReloadInsurance.DeletePayrollDetails(lobjPayrollHeader);

                    //Deleting the Allocated Remittance
                    lobjReloadInsurance.DeleteRemittanceAllocation(lobjPayrollHeader);

                    ibusPostInsuranceBatch.ibusEmployerPayrollHeader = lobjPayrollHeader;

                    string lstrMessage = ibusPostInsuranceBatch.LoadDataForOrg(lobjPayrollHeader.icdoEmployerPayrollHeader.org_id);
                    idlgUpdateProcessLog(lstrMessage, "INFO", iobjBatchSchedule.step_name);

                    //Setting Last Reload Date
                    lobjPayrollHeader.icdoEmployerPayrollHeader.last_reload_run_date = iobjSystemManagement.icdoSystemManagement.batch_date;

                    if (idtbOrgPlan != null)
                    {
                        //Get the Filtered Org Plan List by Org
                        DataRow[] larrRow = idtbOrgPlan.FilterTable(busConstant.DataType.Numeric, "org_id",
                                                                    lobjPayrollHeader.icdoEmployerPayrollHeader.org_id);

                        ibusPostInsuranceBatch.iclbOrgPlan = new List<busOrgPlan>();
                        if (larrRow != null)
                        {
                            foreach (DataRow ldrRow in larrRow)
                            {
                                var lbusOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
                                lbusOrgPlan.icdoOrgPlan.LoadData(ldrRow);

                                lbusOrgPlan.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                                lbusOrgPlan.ibusOrganization.icdoOrganization.LoadData(ldrRow);

                                lbusOrgPlan.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                                lbusOrgPlan.ibusPlan.icdoPlan.LoadData(ldrRow);

                                ibusPostInsuranceBatch.iclbOrgPlan.Add(lbusOrgPlan);
                            }
                        }
                    }

                    //prod pir 933
                    ibusPostInsuranceBatch.LoadPersonAccountDepenedents();
            
                    //Reload the Detail and Process Validation
                    ibusPostInsuranceBatch.CreatePayrollDetailCollecton(lobjPayrollHeader.ibusOrganization, false);
                    ibusPostInsuranceBatch.ValidatePayrollDetail();

                    //Override the Header Status (Reload Batch should update the Header Status to Ready to Post 
                    //if all the detail records are in VALID status
                    lobjReloadInsurance.OverrideHeaderStatus(lobjPayrollHeader, true);

                    //Update the Last Reload Run Date & Status (If Available)
                    lobjPayrollHeader.icdoEmployerPayrollHeader.Update();

                    if (lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Commit();
                        lblnInTransaction = false;
                    }

                    if (lobjPayrollHeader.icdoEmployerPayrollHeader.status_value == busConstant.PayrollHeaderStatusReadyToPost)
                    {
                        FillDataTableForReloadInsuranceReport(lobjPayrollHeader.ibusOrganization, lobjPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date);
                    }
                }
                catch (Exception _exc)
                {
                    if (lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Rollback();
                        lblnInTransaction = false;
                    }
                    idlgUpdateProcessLog(" Payroll Header ID : " + lobjPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id + ". " +
                        " Message : " + _exc.Message, "ERR", iobjBatchSchedule.step_name);
                }
            }

            if (idtResultTable.Rows.Count > 0)
            {
                GenerateReport();
            }
        }

        private void FillDataTableForReloadInsuranceReport(busOrganization abusOrganization, DateTime adtPayrollPaidDate)
        {
            DataRow dr = idtResultTable.NewRow();
            dr["OrgCodeID"] = abusOrganization.icdoOrganization.org_code;
            dr["OrgName"] = abusOrganization.icdoOrganization.org_name;
            dr["PayrollPaidDate"] = adtPayrollPaidDate;
            idtResultTable.Rows.Add(dr);
        }

        private void GenerateReport()
        {
            idlgUpdateProcessLog("Reload Insurance Batch Reports", "INFO", istrProcessName);
            CreateReport("rptInsuranceReloadBatchReport.rpt", idtResultTable);
        }

        private DataTable GetDataTable()
        {
            DataTable ldtResultTable = new DataTable();
            DataColumn ldc1 = new DataColumn("OrgCodeID", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("OrgName", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("PayrollPaidDate", Type.GetType("System.DateTime"));
            ldtResultTable.Columns.Add(ldc1);
            ldtResultTable.Columns.Add(ldc2);
            ldtResultTable.Columns.Add(ldc3);
            ldtResultTable.TableName = busConstant.ReportTableName;
            return ldtResultTable;
        }
    }
}
