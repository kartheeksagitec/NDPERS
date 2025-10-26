using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Data;
using NeoSpin.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.BusinessObjects;
using Sagitec.ExceptionPub;
using System.Collections;

namespace NeoSpinBatch
{
    class busPostingInsurance : busNeoSpinBatch
    {
        public DateTime PayrollPaidDate
        {
            get { return new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.Year, iobjSystemManagement.icdoSystemManagement.batch_date.Month, 1); }
        }

        public busPostingInsuranceBatch ibusPostInsuranceBatch { get; set; }

        /// <summary>
        /// Reporting and Posting of Insurance Information into the Member Account.
        /// </summary>
        public void PostingInsurance()
        {
            ibusPostInsuranceBatch = new busPostingInsuranceBatch();
            istrProcessName = iobjBatchSchedule.step_name;
            idlgUpdateProcessLog("Posting Insurance Batch Started", "INFO", iobjBatchSchedule.step_name);

            idlgUpdateProcessLog("Loading All Active Providers", "INFO", iobjBatchSchedule.step_name);
            //Loading Complete Active Provider Org Plan List (Optimization Purpose)
            ibusPostInsuranceBatch.LoadActiveProviders();

            idlgUpdateProcessLog("Loading DB Cache Data", "INFO", iobjBatchSchedule.step_name);
            //Loading the DB Cache Data
            ibusPostInsuranceBatch.LoadDBCacheData();

            /*Change for Loading organization specific data
             * idlgUpdateProcessLog("Loading Life Option Records for All Insurance Members", "INFO", iobjBatchSchedule.step_name);
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

            //prod pir 933
            idlgUpdateProcessLog("Loading All Person Account Depenedents", "INFO", iobjBatchSchedule.step_name);
            ibusPostInsuranceBatch.LoadPersonAccountDepenedents();
            
            //Get the List of Employer Participated in insurance org plans    
            idlgUpdateProcessLog("Loading All Active Employers", "INFO", iobjBatchSchedule.step_name);
            bool lblnInTransaction = false;
            DataTable ldtbOrgPlan = busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadActiveInsuranceOrgPlanForPostingInsurance", new object[1] { PayrollPaidDate });

            var larrDistinctOrgList = ldtbOrgPlan.AsEnumerable().GroupBy(i => i["org_id"]).Select(i => i.First());

            foreach (DataRow adrRow in larrDistinctOrgList)
            {
                busOrganization lbusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lbusOrganization.icdoOrganization.LoadData(adrRow);

                try
                {
                    idlgUpdateProcessLog("Processing Org Code : " + lbusOrganization.icdoOrganization.org_code + ". ", "INFO", iobjBatchSchedule.step_name);
                    if (!lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.BeginTransaction();
                        lblnInTransaction = true;
                    }

                    var larrRow = ldtbOrgPlan.AsEnumerable().Where(i => Convert.ToInt32(i["org_id"]) == lbusOrganization.icdoOrganization.org_id);

                    ibusPostInsuranceBatch.iclbOrgPlan = new List<busOrgPlan>();
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

                    string lstrMessage = ibusPostInsuranceBatch.LoadDataForOrg(lbusOrganization.icdoOrganization.org_id);
                    idlgUpdateProcessLog(lstrMessage, "INFO", iobjBatchSchedule.step_name);
                    ibusPostInsuranceBatch.PostInsurancePayroll(lbusOrganization);
                    //PIR 24618   FILLER: SFN 58807 and 58808 are discontinued.
                    //if (ibusPostInsuranceBatch.ibusEmployerPayrollHeader != null &&
                    //    ibusPostInsuranceBatch.ibusEmployerPayrollHeader.iclbEmployerPayrollDetail != null &&
                    //    ibusPostInsuranceBatch.ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Count > 0)
                    //{
                    //    //Generate the Report (Only for Other than Web Enabled Employers)
                    //    if (ibusPostInsuranceBatch.ibusEmployerPayrollHeader.ibusOrganization != null)
                    //    {
                    //        if (ibusPostInsuranceBatch.ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.emp_trans_report_flag != busConstant.Flag_Yes)
                    //        {
                    //            //prod pir 6243
                    //            //ibusPostInsuranceBatch.ibusEmployerPayrollHeader.LoadOtherInsuranceObjectsForCorr();
                    //            GenerateSFNCorrespondence();
                    //        }
                    //    }
                    //}

                    if (lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Commit();
                        lblnInTransaction = false;
                    }

                    idlgUpdateProcessLog("Processed Org Code : " + lbusOrganization.icdoOrganization.org_code + ". ", "INFO", iobjBatchSchedule.step_name);

                }

                catch (Exception _exc)
                {
                    ExceptionManager.Publish(_exc);
                    if (lblnInTransaction)
                    {
                        utlPassInfo.iobjPassInfo.Rollback();
                        lblnInTransaction = false;
                    }
                    idlgUpdateProcessLog(" ORG Code : " + lbusOrganization.icdoOrganization.org_code + ". " +
                        " Message : " + _exc.Message, "ERR", iobjBatchSchedule.step_name);
                }
            }
            idlgUpdateProcessLog("Posting Insurance Batch Ended", "INFO", iobjBatchSchedule.step_name);
        }

        //private void GenerateSFNCorrespondence()
        //{
        //    //ArrayList larrlist = new ArrayList();
        //    //larrlist.Add(ibusPostInsuranceBatch.ibusEmployerPayrollHeader);
        //    Hashtable lhstDummyTable = new Hashtable();
        //    lhstDummyTable.Add("sfwCallingForm", "Batch");
        //    //PIR 24618   FILLER: SFN 58807 and 58808 are discontinued.
        //    //CreateCorrespondence("SFN-58808", ibusPostInsuranceBatch.ibusEmployerPayrollHeader, lhstDummyTable);            
        //}
    }
}
