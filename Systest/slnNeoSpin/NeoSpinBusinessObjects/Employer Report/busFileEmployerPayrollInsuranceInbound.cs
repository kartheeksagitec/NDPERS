
#region Using directives
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using Sagitec.ExceptionPub;
using System.IO;
using System.Collections.Generic;
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busFileEmployerPayrollInsuranceInbound : busFileBase
    {
        public busFileEmployerPayrollInsuranceInbound()
        {

        }

        private static int lintHeaderGroupValue = 0;
        private string lstrHeaderGroupValue = "0";
        private static int lintCentralPayrollID = 0;
        private busPostingInsuranceBatch ibusPostingInsuranceBatch;

        public busPostingInsuranceBatch ibusPostingInsBatchForReloadInsuranceHeader { get; set; }
        //public DataTable idtbOrgPlan { get; set; }
        public DateTime idtBatchRunDate { get; set; }
        public DataTable idtMastarReportData { get; set; }
        public override void InitializeFile()
        {
            base.InitializeFile();

            ibusPostingInsBatchForReloadInsuranceHeader = new busPostingInsuranceBatch();

            idtBatchRunDate = busGlobalFunctions.GetSysManagementBatchDate();
            //Loading Complete Activte Provider Org Plan List (Optimization Purpose)
            ibusPostingInsBatchForReloadInsuranceHeader.LoadActiveProviders();

            //Loading Complete Activte Provider Org Plan List (Optimization Purpose)
            ibusPostingInsBatchForReloadInsuranceHeader.LoadAllOrgPlanProviders();

            //Loading the DB Cache Data
            ibusPostingInsBatchForReloadInsuranceHeader.LoadDBCacheData();

            /*  Moved the code to load for each header.
            //Loading the Life Option Data by Org (Optimization)
            ibusPostingInsBatchForReloadInsuranceHeader.LoadLifeOptionData();

            //Loading the EAP History (Optimization)
            ibusPostingInsBatchForReloadInsuranceHeader.LoadEAPHistory();

            //Loading the GHDV History (Optimization)
            ibusPostingInsBatchForReloadInsuranceHeader.LoadGHDVHistory();

            //Loading the Life History (Optimization)
            ibusPostingInsBatchForReloadInsuranceHeader.LoadLifeHistory();
            */
            ibusPostingInsBatchForReloadInsuranceHeader.LoadAllPlans();

            //Get the List of Employer Particpated in insurance org plans    
            //idtbOrgPlan = busNeoSpinBase.Select("cdoEmployerPayrollHeader.GetActiveInsuranceOrgPlans", new object[1] { idtBatchRunDate });
        }

        //Setting the Header Group Values if multiple ORG Code comes in the Same File (Central Payroll)
        public override void SetHeaderGroupValue()
        {
            if (icdoFileDtl != null)
            {
                if (icdoFileDtl.transaction_code_value == "1")
                {
                    lintHeaderGroupValue++;
                    if (lintHeaderGroupValue < 10)
                        lstrHeaderGroupValue = "0" + lintHeaderGroupValue.ToString();
                    else
                        lstrHeaderGroupValue = lintHeaderGroupValue.ToString();
                }
                icdoFileDtl.header_group_value = lstrHeaderGroupValue;
            }
        }

        private busEmployerPayrollHeader _ibusEmployerPayrollHeader;
        public busEmployerPayrollHeader ibusEmployerPayrollHeader
        {
            get { return _ibusEmployerPayrollHeader; }
            set { _ibusEmployerPayrollHeader = value; }
        }

        public override busBase NewHeader()
        {
            _ibusEmployerPayrollHeader = new busEmployerPayrollHeader();
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader();
            _ibusEmployerPayrollHeader.iblnValidateDetail = true;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.reporting_source_value = busConstant.PayrollHeaderReportingSourceWebRpt;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.balancing_status_value = busConstant.PayrollHeaderBalancingStatusNoRemittance;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.interest_waiver_flag = busConstant.Flag_No;
            return _ibusEmployerPayrollHeader;
        }

        public override busBase NewDetail()
        {
            if (icdoFileDtl.transaction_code_value == "1")
                return _ibusEmployerPayrollHeader;

            return _ibusEmployerPayrollHeader.CreateNewEmployerPayrollDetail();
        }

        //private void LoadDataForPayrollHeader()
        //{
        //    DateTime ldtPayrollPaidDate = ibusPostingInsBatchForReloadInsuranceHeader.PayrollPaidDate;
        //    int lintOrgId = ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id;

        //    ibusPostingInsBatchForReloadInsuranceHeader.idtbPALifeOptionHistory =
        //        busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadLifeOptionWithOrg",
        //            new object[2] { ldtPayrollPaidDate, lintOrgId  });

        //    ibusPostingInsBatchForReloadInsuranceHeader.idtbEAPHistory =
        //        busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadEAPHistoryWithOrg", new object[2] { ldtPayrollPaidDate, lintOrgId });

        //    //Loading the GHDV History (Optimization)
        //    ibusPostingInsBatchForReloadInsuranceHeader.idtbGHDVHistory =
        //        busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadGHDVHistoryWithOrg", new object[2] { ldtPayrollPaidDate, lintOrgId });

        //    //Loading the Life History (Optimization)
        //    ibusPostingInsBatchForReloadInsuranceHeader.idtbLifeHistory =
        //        busNeoSpinBase.Select("cdoEmployerPayrollHeader.LoadLifeHistoryWithOrg", new object[2] { ldtPayrollPaidDate, lintOrgId });
        //}

        public override void ProcessHeader()
        {

            long lngMemoryused = 0;

            string lstrLogMessage = "";
            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "LoadHeader               started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString() + " Org Code id " + ibusEmployerPayrollHeader.istrOrgCodeId;
            WriteInfLog(lstrLogMessage);
            LoadHeader();

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "LoadDataForPayrollHeader started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);
            string lstrMessage = ibusPostingInsBatchForReloadInsuranceHeader.LoadDataForOrg(ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id);


            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "LoadDetail               started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage); 
            LoadDetail();

            //LoadEnrollmentPayroll();
            //prod pir 4494 

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "ReloadEnrollmentPayroll  started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);
            
            ReloadEnrollmentPayroll();
            
            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "ReloadEnrollmentPayroll  ended " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "CreateInsMismatchTable   started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);
            DataTable ldtbReportTable = CreateInsuranceMismatchDataTable();
            //For First Organization init
            if (idtMastarReportData == null)
            {
                idtMastarReportData = CreateInsuranceMismatchDataTable();
            }
            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "Premium mismat Record in started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);
            //Premium Mismatch Records From File
            if (ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Count > 0)
            {
                var lenuMismatchRecords =
                    ibusEmployerPayrollHeader.iclbEmployerPayrollDetail.Where(
                        i =>
                        i.icdoEmployerPayrollDetail.premium_amount != i.icdoEmployerPayrollDetail.premium_amount_from_enrollment
                        && i.icdoEmployerPayrollDetail.premium_amount_from_enrollment != 0 && //Premium Amt 0 will come only on suspended member / IBS Active Member cases which will be covered next foreach block
                        i.icdoEmployerPayrollDetail.premium_amount > 0); //negative adjustments need not come in report

                if ((lenuMismatchRecords != null) && (lenuMismatchRecords.Count() > 0))
                {
                   
                    foreach (var lbusEmployerPayrollDetail in lenuMismatchRecords)
                    {
                        AddMismatchDataRow(ldtbReportTable, lbusEmployerPayrollDetail,
                                           lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount,
                                           lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount_from_enrollment);
                        AddMismatchDataRow(idtMastarReportData, lbusEmployerPayrollDetail,
                                           lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount,
                                           lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount_from_enrollment);
                    }
                }
            }

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "Premium not in enrollmnt started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);
            //Records which are exists in File and not in Enrollment
            foreach (var lbusEmployerPayrollDetail in ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
            {
                if (ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader.iclbEmployerPayrollDetail
                    .Where(i => i.icdoEmployerPayrollDetail.person_id == lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id
                        && i.icdoEmployerPayrollDetail.plan_id == lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id).Count() == 0)
                {
                    AddMismatchDataRow(ldtbReportTable, lbusEmployerPayrollDetail, adecPremiumAmt: lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount);
                    AddMismatchDataRow(idtMastarReportData, lbusEmployerPayrollDetail, adecPremiumAmt: lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount);
                }
            }

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "Premium not in file      started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            //Records which are exists in Enrollment and not in File
            foreach (var lbusEmployerPayrollDetail in ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
            {
                if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupHealth)
                {
                    busEmployerPayrollDetail lbusEmployerPayrollDetailMedicare = new busEmployerPayrollDetail();
                    if (lbusEmployerPayrollDetailMedicare.FindEmployerPayrollDetailMedicare(lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id,
                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date))
                    {
                        foreach (busEmployerPayrollDetail lbusEmpPayrollDetail in lbusEmployerPayrollDetailMedicare.iclbEmpPayrollDetailMedicare)
                        {
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount += lbusEmployerPayrollDetailMedicare.icdoEmployerPayrollDetail.premium_amount_from_enrollment;
                        }
                    }
                }

                if (ibusEmployerPayrollHeader.iclbEmployerPayrollDetail
                    .Where(i => i.icdoEmployerPayrollDetail.plan_id != busConstant.PlanIdMedicarePartD && 
                        i.icdoEmployerPayrollDetail.person_id == lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id
                        && i.icdoEmployerPayrollDetail.plan_id == lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id).Count() == 0
                    && lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id != busConstant.PlanIdMedicarePartD) //Medicare records should not be shown on the file. Medicare premium amount added in the Health premium amount
                {
                    AddMismatchDataRow(ldtbReportTable, lbusEmployerPayrollDetail, adecPremiumEnrollmentAmt: lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount);
                    AddMismatchDataRow(idtMastarReportData, lbusEmployerPayrollDetail, adecPremiumEnrollmentAmt: lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount);
                }
            }

            lngMemoryused = GC.GetTotalMemory(false) / 1000000;
            lstrLogMessage = "Creating Report          started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
            WriteInfLog(lstrLogMessage);

            // PROD PIR ID 4090
            if (ldtbReportTable.Rows.Count > 0)
            {
                // Create Insurance Mismatch report
                string lstrGenReportName = string.Empty;
                busNeoSpinBase lbusBase = new busNeoSpinBase();

                //ESS Redesign - Currently,the report is generated as PDF and Excel.The PDF is not being used, so it can be discontinued
                //lbusBase.CreateReport("rptInsuranceMismatchReport.rpt", ldtbReportTable,
                //ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_code.ToString() + "_"); // UAT Critical PIR 2421

                lngMemoryused = GC.GetTotalMemory(false) / 1000000;
                lstrLogMessage = "Creating excel report    started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
                WriteInfLog(lstrLogMessage);

                // Create Insurance Mismatch report as excel
                lstrGenReportName = lbusBase.CreateExcelReport("rptInsuranceMismatchReport.rpt", ldtbReportTable,
                            ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_code + "_",
                            busConstant.ReportESSPath);

                // Copy the report to ESS
                //string lstrSourceFolder = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptGN");
                string lstrDestinationFolder = utlPassInfo.iobjPassInfo.isrvDBCache.GetPathInfo("rptGenWSS");
                string lstrReportName = lstrGenReportName.Substring(lstrGenReportName.LastIndexOf("\\") + 1, lstrGenReportName.Length - lstrGenReportName.LastIndexOf("\\") - 1);
                if (lstrReportName.IndexOf(".xls") <= 0)
                    lstrDestinationFolder += lstrReportName + ".xls";
                if (lstrGenReportName.IndexOf(".xls") <= 0)
                    lstrGenReportName += ".xls";
                if (File.Exists(lstrGenReportName))
                    File.Copy(lstrGenReportName, lstrDestinationFolder);

                //PIR-20378 – When inbound insurance file is processed and Mismatch reports are created – Do not insert messages for Organizations where the PeopleSoft Org Group is STAT. (Still generate all the reports but do not ‘publish’).
                if (ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.peoplesoft_org_group_value != busConstant.PeopleSoftOrgGroupValueState)
                {
                    // Publish ESS Message to Org Contact
                    //PIR-10608 Start
                    if (ibusEmployerPayrollHeader.ibusOrganization.iclbOrgContact == null || ibusEmployerPayrollHeader.ibusOrganization.iclbOrgContact.Count() == 0)
                        ibusEmployerPayrollHeader.ibusOrganization.LoadOrgContact();

                    var lvarcontactrole = (from p in ibusEmployerPayrollHeader.ibusOrganization.iclbOrgContact
                                           where (p.icdoContactRole.contact_role_value == busConstant.OrgContactRolePrimaryAuthorizedAgent ||
                                                 p.icdoContactRole.contact_role_value == busConstant.OrgContactRoleAuthorizedAgent ||
                                                 p.icdoContactRole.contact_role_value == busConstant.OrgContactRoleFinance ||
                                                 p.icdoContactRole.contact_role_value == busConstant.OrgContactRoleOther) && (p.icdoOrgContact.status_value == busConstant.StatusActive)
                                           group p by p.icdoOrgContact.contact_id into a
                                           select a.First());

                    foreach (busOrgContact lbusOrgContact in lvarcontactrole)
                    {
                        string lstrPrioityValue = string.Empty;
                        busWSSHelper.PublishESSMessage(0, 0, string.Format(busGlobalFunctions.GetDBMessageTextByDBMessageID(1, iobjPassInfo, ref lstrPrioityValue), lstrReportName),
                            lstrPrioityValue, aintOrgID: ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_id, astrCorrespondenceLink: lstrDestinationFolder,
                            aintContactID: lbusOrgContact.icdoOrgContact.contact_id);
                    }
                    //PIR-10608 End
                }
                }
                lngMemoryused = GC.GetTotalMemory(false) / 1000000;
                lstrLogMessage = "Clearing memory          started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
                WriteInfLog(lstrLogMessage);

                string lstrOrgCode = ibusEmployerPayrollHeader.istrOrgCodeId;
                ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader.iclbEmployerPayrollDetail = null;
                ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader = null;
                if (ibusEmployerPayrollHeader != null && ibusEmployerPayrollHeader.iclbEmployerPayrollDetail != null)
                {
                    ibusEmployerPayrollHeader.iclbEmployerPayrollDetail = null;
                    ibusEmployerPayrollHeader = null;
                }
                ibusPostingInsBatchForReloadInsuranceHeader.iclbActiveInsuranceMembers = null;
                ibusPostingInsBatchForReloadInsuranceHeader.iclbPersonAccountGhdv = null;
                ibusPostingInsBatchForReloadInsuranceHeader.iclbPersonAccountLife = null;
                ibusPostingInsBatchForReloadInsuranceHeader.iclbRGroupPersonAccount = null;
                ibusPostingInsBatchForReloadInsuranceHeader.iclbRGroupPersonAccountGhdv = null;
                ibusPostingInsBatchForReloadInsuranceHeader.iclbRGroupPersonAccountLife = null;

                //dka
                //ibusPostingInsBatchForReloadInsuranceHeader.idtbPALifeOptionHistory = null;
                //ibusPostingInsBatchForReloadInsuranceHeader.idtbEAPHistory = null;
                //ibusPostingInsBatchForReloadInsuranceHeader.idtbGHDVHistory = null;
                //ibusPostingInsBatchForReloadInsuranceHeader.idtbLifeHistory = null;


                GC.Collect();
                lngMemoryused = GC.GetTotalMemory(false) / 1000000;
                lstrLogMessage = "process header           finished" + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString() + " Org Code id " + lstrOrgCode;
                WriteInfLog(lstrLogMessage);            

        }

        private void WriteInfLog(string lstrInf)
        {
            string lstrLogFile = AppDomain.CurrentDomain.BaseDirectory;

            lstrLogFile += "\\InsuranceInbound" + DateTime.Today.ToString("yyyy-MM-dd") + ".txt";

            using (FileStream fs = File.Open(lstrLogFile,
            FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.WriteLine(lstrInf);
                }
            }
        }
        /// <summary>
        /// method to reload insurance payroll header/details
        /// </summary>
        private void ReloadEnrollmentPayroll()
        {
            // PROD PIR 4494
            DataTable ldtPayrollHeader = busBase.Select("cdoEmployerPayrollHeader.LoadInsuranceHeaderToReloadForOrg",
                    new object[1] { ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id });
            if (ldtPayrollHeader.Rows.Count > 0)
            {
                busEmployerPayrollHeader lobjPayrollHeader = new busEmployerPayrollHeader { icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader() };
                lobjPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldtPayrollHeader.Rows[0]);
                lobjPayrollHeader.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lobjPayrollHeader.ibusOrganization.icdoOrganization.LoadData(ldtPayrollHeader.Rows[0]);

                busReloadInsuranceBatch lobjReloadInsurance = new busReloadInsuranceBatch();

                if (!Convert.IsDBNull(ldtPayrollHeader.Rows[0]["PAYROLL_STATUS_VALUE"]))
                {
                    lobjPayrollHeader.icdoEmployerPayrollHeader.status_value = ldtPayrollHeader.Rows[0]["PAYROLL_STATUS_VALUE"].ToString();
                }

                //Delete the Existing Detail Records
                lobjReloadInsurance.DeletePayrollDetails(lobjPayrollHeader);
                //Deleting the Allocated Remittance
                lobjReloadInsurance.DeleteRemittanceAllocation(lobjPayrollHeader);

                ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader = lobjPayrollHeader;

                //Setting Last Reload Date
                lobjPayrollHeader.icdoEmployerPayrollHeader.last_reload_run_date = idtBatchRunDate;

                LoadOrgPlan(lobjPayrollHeader);

                //prod pir 933
                ibusPostingInsBatchForReloadInsuranceHeader.LoadPersonAccountDepenedents();

                long lngMemoryused = GC.GetTotalMemory(false) / 1000000;
                string lstrLogMessage = "CreatePayrollDetailCollecton started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
                WriteInfLog(lstrLogMessage);

                //Reload the Detail and Process Validation
                ibusPostingInsBatchForReloadInsuranceHeader.CreatePayrollDetailCollecton(lobjPayrollHeader.ibusOrganization, false);

                lngMemoryused = GC.GetTotalMemory(false) / 1000000;
                lstrLogMessage = "CreatePayrollDetailCollecton ended " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
                WriteInfLog(lstrLogMessage);

                //need to reload only if file payroll paid date is same as reload payroll date
                if (ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date ==
                    ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.payroll_paid_date)
                {
                    lngMemoryused = GC.GetTotalMemory(false) / 1000000;
                    lstrLogMessage = "ValidatePayrollDetail started " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
                    WriteInfLog(lstrLogMessage);

                    ibusPostingInsBatchForReloadInsuranceHeader.ValidatePayrollDetail();

                    lngMemoryused = GC.GetTotalMemory(false) / 1000000;
                    lstrLogMessage = "ValidatePayrollDetail ended " + DateTime.Now.ToString() + " Memeory used : " + lngMemoryused.ToString();
                    WriteInfLog(lstrLogMessage);

                    //Override the Header Status (Reload Batch should update the Header Status to Ready to Post 
                    //if all the detail records are in VALID status
                    lobjReloadInsurance.OverrideHeaderStatus(lobjPayrollHeader, true);

                    //Update the Last Reload Run Date & Status (If Available)
                    lobjPayrollHeader.icdoEmployerPayrollHeader.Update();
                }
                lobjPayrollHeader = null;
                lobjReloadInsurance = null;
            }

            ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader = new busEmployerPayrollHeader
            {
                icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader(),
                iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>(),
                ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() }
            };
            // If the Header is already Posted, Load the posted header to avoid Mismatch report exceptions.
            DataTable ldtbResults = busBase.Select("cdoEmployerPayrollHeader.LoadCurrentMonthPostedInsuranceHeader",
                                        new object[2] { ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id, idtBatchRunDate });
            if (ldtbResults.Rows.Count > 0)
            {
                ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(ldtbResults.Rows[0]);
                ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.LoadData(ldtbResults.Rows[0]);
                //ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader.LoadEmployerPayrollDetailWithHeader();
                
                /*  Code Changes for performance- SH */ 
                ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader.LoadEmployerPayrollDetail();

                foreach (busEmployerPayrollDetail lbusEmployerPayrollDetail in ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
                {
                    lbusEmployerPayrollDetail.ibusEmployerPayrollHeader = ibusPostingInsBatchForReloadInsuranceHeader.ibusEmployerPayrollHeader;

                    foreach (busPlan lbusPlan in ibusPostingInsBatchForReloadInsuranceHeader.iclbAllPlans)
                    {
                        if (lbusPlan.icdoPlan.plan_id == lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id)
                        {
                            lbusEmployerPayrollDetail.ibusPlan = lbusPlan;
                            break;
                        }
                    }
                }
                /*  Code Changes for performance- SH */ 
            }
        }

        private void LoadOrgPlan(busEmployerPayrollHeader lobjPayrollHeader)
        {
            DataTable idtbOrgPlan = busNeoSpinBase.Select("cdoEmployerPayrollHeader.GetActiveInsuranceOrgPlans", new object[1] { idtBatchRunDate });
            if (idtbOrgPlan != null)
            {
                //Get the Filtered Org Plan List by Org
                DataRow[] larrRow = idtbOrgPlan.FilterTable(busConstant.DataType.Numeric, "org_id",
                                                            lobjPayrollHeader.icdoEmployerPayrollHeader.org_id);

                ibusPostingInsBatchForReloadInsuranceHeader.iclbOrgPlan = new List<busOrgPlan>();
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

                        ibusPostingInsBatchForReloadInsuranceHeader.iclbOrgPlan.Add(lbusOrgPlan);
                    }
                }
            }
        }

        private void AddMismatchDataRow(DataTable adtReportTable, busEmployerPayrollDetail abusDetail, decimal adecPremiumAmt = 0.00M, decimal adecPremiumEnrollmentAmt = 0.00M)
        {
            // if the difference between ‘Premium Paid’ and ‘Premium Billed’ is between -0.01 and 0.01, do not include on the report.
            decimal adecPremiumAmtDifference = adecPremiumAmt - adecPremiumEnrollmentAmt;
            if (Math.Abs(adecPremiumAmtDifference) > 0.01M)
            {
                DataRow drReport = adtReportTable.NewRow();
                drReport["OrgCode"] = abusDetail.ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_code;
                drReport["OrgName"] = abusDetail.ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_name;
                //if (abusDetail.ibusPerson == null)
                //{
                //    abusDetail.ibusPerson = new busPerson();
                //    abusDetail.ibusPerson.FindPerson(abusDetail.icdoEmployerPayrollDetail.person_id);
                //}
                drReport["SSN"] = abusDetail.icdoEmployerPayrollDetail.LastFourDigitsOfSSN.PadLeft(9, 'X');
                drReport["PeopleSoftID"] = abusDetail.icdoEmployerPayrollDetail.istrPeopleSoftID;
                drReport["PersonID"] = abusDetail.icdoEmployerPayrollDetail.person_id;
                drReport["FirstName"] = abusDetail.icdoEmployerPayrollDetail.first_name;
                drReport["LastName"] = abusDetail.icdoEmployerPayrollDetail.last_name;
                drReport["Plan"] = abusDetail.ibusPlan.icdoPlan.plan_name;
                drReport["PremimumReported"] = adecPremiumAmt;
                drReport["PremiumFromEnrollment"] = adecPremiumEnrollmentAmt;
                drReport["OrgType"] = abusDetail.ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.peoplesoft_org_group_value;
                adtReportTable.Rows.Add(drReport);
            }
        }

        private DataTable CreateInsuranceMismatchDataTable()
        {
           //Change name of column heading ‘Premium Reported’ to ‘Premium Paid’
           //Change name of column heading ‘Premium from Enrollment’ to ‘Premium Billed’
           //Add a column for PeopleSoft ID after the SSN column

            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("OrgCode", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("OrgName", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("SSN", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("PersonID", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("FirstName", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("LastName", Type.GetType("System.String"));
            DataColumn ldc7 = new DataColumn("Plan", Type.GetType("System.String"));
            DataColumn ldc8 = new DataColumn("PremimumReported", Type.GetType("System.Decimal"));
            DataColumn ldc9 = new DataColumn("PremiumFromEnrollment", Type.GetType("System.Decimal"));
            DataColumn ldc10 = new DataColumn("PeopleSoftID", Type.GetType("System.String"));
            DataColumn ldc11 = new DataColumn("OrgType", Type.GetType("System.String"));

            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);
            ldtbReportTable.Columns.Add(ldc7);
            ldtbReportTable.Columns.Add(ldc8);
            ldtbReportTable.Columns.Add(ldc9);
            ldtbReportTable.Columns.Add(ldc10);
            ldtbReportTable.Columns.Add(ldc11);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }
        
        private void LoadHeader()
        {
            //Assigning Prepopulated Values into Header
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value = busConstant.PayrollHeaderBenefitTypeInsr;

            if (!(String.IsNullOrEmpty(_ibusEmployerPayrollHeader.istrOrgCodeId)))
            {
                _ibusEmployerPayrollHeader.ibusOrganization = new busOrganization();
                _ibusEmployerPayrollHeader.ibusOrganization.FindOrganizationByOrgCode(_ibusEmployerPayrollHeader.istrOrgCodeId);
                _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id = _ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_id;
            }
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.received_date = DateTime.Now;
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.submitted_date = DateTime.Now;
            // If the File has multiple header, increment the existing max central payroll ID
            if ((lintHeaderGroupValue > 1) && (lintCentralPayrollID == 0))
            {
                lintCentralPayrollID = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoEmployerPayrollHeader.GetMaxCentralPayrollID", new object[] { },
                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                lintCentralPayrollID += 1;
            }
            _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.central_payroll_record_id = lintCentralPayrollID;
        }

        public void LoadDetail()
        {
            if (_ibusEmployerPayrollHeader.iclbEmployerPayrollDetail == null)
            {
                _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
            }
            foreach (busEmployerPayrollDetail lobjEmployerPayrollDetail in _ibusEmployerPayrollHeader.iclbEmployerPayrollDetail)
            {
                lobjEmployerPayrollDetail.ibusEmployerPayrollHeader = ibusEmployerPayrollHeader;
                //lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.ibusOrganization = ibusEmployerPayrollHeader.ibusOrganization;
                //lobjEmployerPayrollDetail.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader = ibusEmployerPayrollHeader.icdoEmployerPayrollHeader;
                //lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_payroll_header_id = _ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;

                //Plan ID
                if ((!(String.IsNullOrEmpty(lobjEmployerPayrollDetail.istrPlanValue))))
                {
                    lobjEmployerPayrollDetail.ibusPlan = new busPlan();
                    lobjEmployerPayrollDetail.ibusPlan.FindPlanByPlanCode(lobjEmployerPayrollDetail.istrPlanValue);

                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id = lobjEmployerPayrollDetail.ibusPlan.icdoPlan.plan_id;
                }
                //PERSON ID, FIRST NAME, LAST NAME
                if (!String.IsNullOrEmpty(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn))
                {
                    //DataTable ldtbSSN = busBase.Select<cdoPerson>(new string[1] { "ssn" }, new object[1] { lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn }, null, null);
                    //if (ldtbSSN.Rows.Count > 0)
                    //{
                    //    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id = Convert.ToInt32(ldtbSSN.Rows[0]["person_id"]);
                    //}
                    DataTable lobjResult = DBFunction.DBSelect("cdoPerson.GetPersonID", new object[1] { lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.ssn }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                    if (lobjResult != null && lobjResult.Rows.Count>0)
                    {
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id = Convert.ToInt32(lobjResult.Rows[0][enmPerson.person_id.ToString()]);
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.istrPeopleSoftID = Convert.ToString(lobjResult.Rows[0][enmPerson.peoplesoft_id.ToString()]);
                        lobjResult = null;
                    }
                }

                if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id != 0)
                {
                    if (_ibusEmployerPayrollHeader.ibusOrganization == null)
                        _ibusEmployerPayrollHeader.LoadOrganization();

                    if (lobjEmployerPayrollDetail.ibusPersonAccount == null)
                        lobjEmployerPayrollDetail.LoadPersonAccount();

                    decimal ldecPremiumAmount = 0.00M;
                    decimal ldecFeeAmt = 0.00M;
                    decimal ldecBuydownAmt = 0.00M;
                    decimal ldecMedicarePartD = 0.00M;
                    decimal ldecRHICAmt = 0.00M;
                    /* UAT PIR 476, Including other and JS RHIC Amount */
                    decimal ldecOthrRHICAmt = 0.00M;
                    decimal ldecJSRHICAmt = 0.00M;
                    //uat pir 1429 : post ghdv_history_id
                    int lintGHDVHistoryID = 0;
                    string lstrGroupNumber = string.Empty;
                    //prod pir 6076
                    string lstrCoverageCodeValue = string.Empty, lstrRateStructureCode = string.Empty;
                    //pir 7705
                    decimal ldecHSAAmt = 0.00M;
                    decimal ldecHSAVendorAmt = 0.0M;
                    ldecPremiumAmount =
                        busRateHelper.GetInsurancePremiumAmount(ibusEmployerPayrollHeader.ibusOrganization,
                            lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_last_date,
                            lobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.person_account_id,
                            lobjEmployerPayrollDetail.ibusPersonAccount.icdoPersonAccount.plan_id,
                            ref ldecFeeAmt, ref ldecBuydownAmt, ref ldecMedicarePartD,ref ldecRHICAmt, ref ldecOthrRHICAmt, ref ldecJSRHICAmt, ref ldecHSAAmt , ref ldecHSAVendorAmt,
                            lobjEmployerPayrollDetail.ibusPersonAccountLife,
                            lobjEmployerPayrollDetail.ibusPersonAccountGhdv,
                            lobjEmployerPayrollDetail.ibusPersonAccountLtc,
                            lobjEmployerPayrollDetail.ibusPersonAccountEAP,
                            lobjEmployerPayrollDetail.ibusPersonAccountMedicare,  //PIR 15434
                            iobjPassInfo, null, ref lintGHDVHistoryID, ref lstrGroupNumber,
                            ref lstrCoverageCodeValue, ref lstrRateStructureCode);

                    //Added null condition
                    if (lobjEmployerPayrollDetail.ibusPersonAccountGhdv.IsNotNull())
                        ldecMedicarePartD = lobjEmployerPayrollDetail.ibusPersonAccountGhdv.icdoPersonAccountGhdv.MedicarePartDAmount;//PIR 14271
                    /* UAT PIR 476 ends here */
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount_from_enrollment = busGlobalFunctions.RoundToPenny(ldecFeeAmt + ldecPremiumAmount + ldecHSAAmt - ldecRHICAmt - ldecBuydownAmt + ldecMedicarePartD); //PROD PIR 7705 //PIR 14271

                    //PIR 16601 - Adding premium_amount_from_enrollment from Medicare to Health.
                    if (lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id == busConstant.PlanIdGroupHealth)
                    {
                        busEmployerPayrollDetail lbusEmployerPayrollDetailMedicare = new busEmployerPayrollDetail();
                        if (lbusEmployerPayrollDetailMedicare.FindEmployerPayrollDetailMedicare(lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_id,
                            lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.pay_period_date))
                        {
                            foreach (busEmployerPayrollDetail lbusEmpPayrollDetail in lbusEmployerPayrollDetailMedicare.iclbEmpPayrollDetailMedicare)
                            {
                                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount_from_enrollment += lbusEmployerPayrollDetailMedicare.icdoEmployerPayrollDetail.premium_amount_from_enrollment;
                            }
                        }

                    }

                    //uat pir 1429
                    //prod pir 6076 & 6077 - Removal of person account ghdv history id
                    //lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.person_account_ghdv_history_id = lintGHDVHistoryID;
                    if (string.IsNullOrEmpty(lstrGroupNumber))
                    {
                        if (ibusEmployerPayrollHeader.ibusOrganization == null)
                            ibusEmployerPayrollHeader.LoadOrganization();
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.group_number = ibusEmployerPayrollHeader.ibusOrganization.icdoOrganization.org_code;
                    }
                    else
                    {
                        lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.group_number = lstrGroupNumber;
                    }
                    //prod pir 6076
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.coverage_code = lstrCoverageCodeValue;
                    lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.rate_structure_code = lstrRateStructureCode;
                }
                lobjEmployerPayrollDetail.icdoEmployerPayrollDetail.suppress_warnings_flag = "N";
            }
        }

        private void LoadEnrollmentPayroll()
        {
            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader
                                                                     {
                                                                         icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader()
                                                                     };

            lbusEmployerPayrollHeader.ibusOrganization = ibusEmployerPayrollHeader.ibusOrganization;

            ibusPostingInsuranceBatch = new busPostingInsuranceBatch();
            ibusPostingInsuranceBatch.ibusEmployerPayrollHeader = lbusEmployerPayrollHeader;

            //Reload the Detail and Process Validation
            ibusPostingInsuranceBatch.CreatePayrollDetailCollecton(lbusEmployerPayrollHeader.ibusOrganization, true);
        }

        public override string BeforeFieldAssigned(string astrFieldName, string astrFieldValue)
        {
            string lstrReturnValue = astrFieldValue;
            string lstrObjectField = astrFieldName.IndexOf(".") > -1 ? astrFieldName.Substring(astrFieldName.LastIndexOf(".") + 1) : astrFieldName;

            lstrObjectField = lstrObjectField.ToLower();

            if (((lstrObjectField == busConstant.EmployerReportPayrollPaidDate) || (lstrObjectField == busConstant.EmployerReportPayPeriod)
               || (lstrObjectField == busConstant.EmployerReportPayPeriodDate)) && (astrFieldValue.Length == 6))
            {
                lstrReturnValue = astrFieldValue.Substring(0, 2) + "/01/" + astrFieldValue.Substring(2, 4);
            }

            if (((lstrObjectField == busConstant.EmployerReportPayPeriodEndMonthForBonus) || (lstrObjectField == busConstant.EmployerReportMemberStatusEffectiveDate))
                                                                            && (astrFieldValue.Length == 6))
            {
                lstrReturnValue = astrFieldValue.Substring(0, 2) + "/01/" + astrFieldValue.Substring(2, 4);
            }

            //Report type for Header
            if (lstrObjectField == "report_type_value")
            {
                if ((!(String.IsNullOrEmpty(astrFieldValue))))
                {
                    if (astrFieldValue == "1")
                    {
                        lstrReturnValue = busConstant.PayrollHeaderReportTypeRegular;
                    }
                    else if (astrFieldValue == "2")
                    {
                        lstrReturnValue = busConstant.PayrollHeaderReportTypeAdjustment;
                    }
                }
            }

            return lstrReturnValue;
        }
        
        public override sfwOnFileError ContinueOnValueError(string astrObjectField, out string astrValue)
        {
            astrValue = String.Empty;
            string lstrObjectField = astrObjectField.IndexOf(".") > -1 ? astrObjectField.Substring(astrObjectField.LastIndexOf(".") + 1) : astrObjectField;
            //NOT YET IMPLEMENTED.. BASE IS DEFINED...  
            switch (lstrObjectField.ToLower())
            {
                case busConstant.EmployerReportPayrollPaidDate:
                case busConstant.EmployerReportPayPeriodDate:
                case busConstant.EmployerReportPayPeriod:
                case busConstant.EmployerReportPayPeriodEndMonthForBonus:
                    astrValue = String.Empty;
                    return sfwOnFileError.ContinueWithRecord;

                default: return base.ContinueOnValueError(astrObjectField, out astrValue);
            }
        }

        public override sfwOnFileError OnHeaderHardError()
        {
            return sfwOnFileError.ContinueWithRecord;
        }

        public override void FinalizeFile()
        {
            busNeoSpinBase lbusBase = new busNeoSpinBase();
            String lstrGenReportName = lbusBase.CreateExcelReport("rptPremiumDiscrepancyMaster.rpt", idtMastarReportData,
                            "",
                            busConstant.ReportESSPath);
            lintCentralPayrollID = 0;
            lintHeaderGroupValue = 0;
            base.FinalizeFile();
        }

        
    }
}
