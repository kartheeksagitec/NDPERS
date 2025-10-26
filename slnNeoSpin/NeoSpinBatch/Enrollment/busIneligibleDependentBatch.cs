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
using System.Linq;

#endregion

namespace NeoSpinBatch
{
    class busIneligibleDependentBatch : busNeoSpinBatch
    {
        DataTable idtResultTable = new DataTable();

        private Collection<busPersonDependent> _iclbDependents;
        public Collection<busPersonDependent> iclbDependents
        {
            get { return _iclbDependents; }
            set { _iclbDependents = value; }
        }

        public void CreateIneligibleDependentCorrespondence()
        {
            Collection<busPersonDependent> lclbGHDVDependents = new Collection<busPersonDependent>();
            _iclbDependents = new Collection<busPersonDependent>();
            /// Get All Child Dependents for Active Member Accounts
            DataTable ldtbDepenents = DBFunction.DBSelect("cdoPersonAccountGhdv.LoadGHDVActiveDependentChilds", new object[] { },
                                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            //PIR 20584 - Generate a Single Letter depend on PERSON_DEPENDENT_ID with plans
            IList<DataTable> ldtbGetAllDependentGroup = ldtbDepenents.AsEnumerable().GroupBy(row => new
            {
                PersonDependentId = row.Field<int>("PERSON_DEPENDENT_ID")
            }).Select(g => g.CopyToDataTable()).ToList();

            istrProcessName = "Ineligible Dependent Correspondence";
            idlgUpdateProcessLog("Creating Correspondence for Ineligible Dependent Persons", "INFO", istrProcessName);
            if (ldtbGetAllDependentGroup.Count > 0)
            {
                // This Loop for distinct PERSON_DEPENDENT_ID
                foreach (DataTable dt in ldtbGetAllDependentGroup)
                {
                    bool lblIsDateOverlap = false;
                    string lstrPlanNames = string.Empty;
                    busPersonDependent lobjDependent = new busPersonDependent();
                    // This Loop for PERSON_DEPENDENT - Plans and other details
                    foreach (DataRow dr in dt.Rows)
                    {
                        /// Load Dependent Person Details
                        lobjDependent = new busPersonDependent();
                        busPersonAccount lobjPersonAccount = new busPersonAccount();
                        lobjPersonAccount.icdoPersonAccount = new cdoPersonAccount();
                        lobjDependent.ibusPeronAccountDependent = new busPersonAccountDependent();
                        lobjDependent.ibusPeronAccountDependent.ibusPlan = new busPlan();
                        lobjDependent.icdoPersonDependent = new cdoPersonDependent();
                        lobjDependent.ibusPeronAccountDependent.icdoPersonAccountDependent = new cdoPersonAccountDependent();
                        lobjDependent.icdoPersonDependent.LoadData(dr);
                        lobjDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.LoadData(dr);
                        lobjPersonAccount.FindPersonAccount(lobjDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.person_account_id);

                        /// Load Plan
                        lobjDependent.ibusPeronAccountDependent.ibusPlan.FindPlan(lobjPersonAccount.icdoPersonAccount.plan_id);
                        lstrPlanNames += lobjDependent.ibusPeronAccountDependent.ibusPlan.icdoPlan.istr_PLAN_NAME + "/";
                        /// Calculate Dependent's Age
                        lobjDependent.LoadDependentInfo();
                        DateTime ldtDependentCurrentYearDOB = lobjDependent.icdoPersonDependent.dependent_DOB.AddYears(26);
                        DateTime ldtGuardianshipExpirationDate = lobjDependent.icdoPersonDependent.guardianship_expiration_date;
                        /// Last day of the month from (CurrentDate + 2 months)
                        DateTime ldtEndDate = DateTime.Now.AddMonths(2).GetLastDayofMonth();
                        lobjDependent.icdoPersonDependent.istr_Guardianship = busConstant.Flag_No;
                        if (ldtGuardianshipExpirationDate != DateTime.MinValue)
                            lobjDependent.icdoPersonDependent.istr_Guardianship = busConstant.Flag_Yes;

                        lobjDependent.icdoPersonDependent.istrLifePlan = busConstant.Flag_No;

                        if (Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfDependentEnrolledInLife", new object[1]
                            { lobjDependent.icdoPersonDependent.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework)) > 0)
                        {
                            lobjDependent.icdoPersonDependent.istrLifePlan = busConstant.Flag_Yes;
                        }
                        if (ldtGuardianshipExpirationDate == DateTime.MinValue && busGlobalFunctions.CheckDateOverlapping(ldtDependentCurrentYearDOB, DateTime.Now, ldtEndDate))
                        {
                            lobjDependent.icdoPersonDependent.dependent_age = busGlobalFunctions.CalulateAge(lobjDependent.icdoPersonDependent.dependent_DOB, ldtDependentCurrentYearDOB);
                            //PIR - 975 to show the plan effective date as month year
                            lobjDependent.icdoPersonDependent.istr_dependent_Ineligible_Date = ldtDependentCurrentYearDOB.ToString("y");
                            lobjDependent.icdoPersonDependent.dependent_coverage_enddate = ldtDependentCurrentYearDOB.GetLastDayofMonth();
                            lobjDependent.icdoPersonDependent.istr_dependent_coverage_enddate = lobjDependent.icdoPersonDependent.dependent_coverage_enddate.ToString(busConstant.DateFormatLongDate); // UAT PIR 975
                            lobjDependent.icdoPersonDependent.dependent_coverage_renew_date = new DateTime(ldtDependentCurrentYearDOB.Year, ldtDependentCurrentYearDOB.Month, 15).ToString(busConstant.DateFormatLongDate);// UAT PIR 975
                            _iclbDependents.Add(lobjDependent);
                            lblIsDateOverlap = true;
                        }
                        if (ldtGuardianshipExpirationDate != DateTime.MinValue && busGlobalFunctions.CheckDateOverlapping(ldtGuardianshipExpirationDate, DateTime.Now, ldtEndDate))
                        {
                            //lobjDependent.icdoPersonDependent.dependent_age = busGlobalFunctions.CalulateAge(lobjDependent.icdoPersonDependent.dependent_DOB, ldtDependentCurrentYearDOB);
                            //PIR - 975 to show the plan effective date as month year
                            //lobjDependent.icdoPersonDependent.istr_dependent_Ineligible_Date = ldtDependentCurrentYearDOB.ToString("y");
                            lobjDependent.icdoPersonDependent.istr_Actual_Expiration_Date = ldtGuardianshipExpirationDate.ToString(busConstant.DateFormatLongDate);
                            lobjDependent.icdoPersonDependent.dependent_coverage_enddate = ldtGuardianshipExpirationDate.GetLastDayofMonth();
                            lobjDependent.icdoPersonDependent.istr_dependent_coverage_enddate = lobjDependent.icdoPersonDependent.dependent_coverage_enddate.ToString(busConstant.DateFormatLongDate); // UAT PIR 975
                            lobjDependent.icdoPersonDependent.dependent_coverage_renew_date = new DateTime(ldtGuardianshipExpirationDate.Year, ldtGuardianshipExpirationDate.Month, 15).ToString(busConstant.DateFormatLongDate);// UAT PIR 975
                            _iclbDependents.Add(lobjDependent);
                            lblIsDateOverlap = true;
                        }
                    }
                    lobjDependent.ibusPeronAccountDependent.istrAllPlanNames = lstrPlanNames.TrimEnd('/');

                    Hashtable lshtTemp = new Hashtable();
                    lshtTemp.Add("FormTable", "Batch");

                    if (lblIsDateOverlap)
                    {
                        string lstrFileName = CreateCorrespondence("ENR-5100", lobjDependent, lshtTemp);
                    }
                }
            }
            /// Update the Dependent Coverage EndDate
            if (_iclbDependents.Count > 0)
            {
                idlgUpdateProcessLog("Updating the coverage end date for the Dependents", "INFO", istrProcessName);
                UpdateDependentCoverageEndDate();
                idlgUpdateProcessLog("Updated successfully", "INFO", istrProcessName);
                idlgUpdateProcessLog("Correspondence created successfully", "INFO", istrProcessName);
            }
            else
                idlgUpdateProcessLog("No Correspondence to generate", "INFO", istrProcessName);
            idlgUpdateProcessLog("Generating Ineligible Dependent Report", "INFO", istrProcessName);
            GenerateReport(_iclbDependents);
        }

        public void UpdateDependentCoverageEndDate()
        {
            foreach (busPersonDependent lobjDependent in _iclbDependents)
            {
                busPersonAccountDependent lobjPersonAccountDependent = new busPersonAccountDependent();
                lobjPersonAccountDependent.icdoPersonAccountDependent = new cdoPersonAccountDependent();
                lobjPersonAccountDependent.FindPersonAccountDependent(lobjDependent.icdoPersonDependent.person_dependent_id,
                                                lobjDependent.ibusPeronAccountDependent.icdoPersonAccountDependent.person_account_id);
                lobjPersonAccountDependent.icdoPersonAccountDependent.end_date = lobjDependent.icdoPersonDependent.dependent_coverage_enddate;
                if (lobjPersonAccountDependent.icdoPersonAccountDependent.end_date == DateTime.MinValue)
                    lobjPersonAccountDependent.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag = null;
                else
                    lobjPersonAccountDependent.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag = busConstant.Flag_No; 
                lobjPersonAccountDependent.icdoPersonAccountDependent.Update();
            }
        }
        
        public void GenerateReport(Collection<busPersonDependent> aclbPersonDependent)
        {
            idtResultTable = CreateNewDataTable();

            foreach (busPersonDependent lobjPersonDependent in aclbPersonDependent)
            {
                AddToNewDataRow(lobjPersonDependent);
            }

            if (idtResultTable.Rows.Count > 0)
            {
                idtResultTable = idtResultTable.AsEnumerable().OrderBy(o => o.Field<string>("MemberLastName")).AsDataTable();
                CreateReport("rptIneligibleDependentReport.rpt", idtResultTable);
                idlgUpdateProcessLog("Ineligible Dependent Report generated successfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }

        }

        public DataTable CreateNewDataTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("MemberFullName", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("MemberLastName", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("DependentName", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("EndDate", Type.GetType("System.DateTime"));
            DataColumn ldc5 = new DataColumn("Plan", Type.GetType("System.String"));
            DataColumn ldc6 = new DataColumn("LevelOfcoverage", Type.GetType("System.String"));
            DataColumn ldc7 = new DataColumn("PERSLinkID", Type.GetType("System.Int32"));
            ldtbReportTable.Columns.Add(ldc1);
            ldtbReportTable.Columns.Add(ldc2);
            ldtbReportTable.Columns.Add(ldc3);
            ldtbReportTable.Columns.Add(ldc4);
            ldtbReportTable.Columns.Add(ldc5);
            ldtbReportTable.Columns.Add(ldc6);
            ldtbReportTable.Columns.Add(ldc7);
            ldtbReportTable.TableName = busConstant.ReportTableName;
            return ldtbReportTable;
        }

        public void AddToNewDataRow(busPersonDependent aobjPersonDependent)
        {
            if (aobjPersonDependent.ibusPeronAccountDependent == null)
                aobjPersonDependent.LoadPersonAccountDependent();
            if (aobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccountGhdv == null)
            {
                aobjPersonDependent.ibusPeronAccountDependent.LoadPersonAccountGhdv();
                aobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccountGhdv
                    .FindPersonAccount(aobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.person_account_id);
            }
            if (aobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccount == null)
                aobjPersonDependent.ibusPeronAccountDependent.LoadPersonAccount();
            if (aobjPersonDependent.ibusPerson == null)
                aobjPersonDependent.LoadPerson();
            if (aobjPersonDependent.ibusDependentPerson == null)
                aobjPersonDependent.LoadDependentInfo();
            DataRow dr = idtResultTable.NewRow();
            dr["MemberFullName"] = aobjPersonDependent.ibusPerson.icdoPerson.PersonName;
            dr["MemberLastName"] = aobjPersonDependent.ibusPerson.icdoPerson.last_name;
            dr["DependentName"] = aobjPersonDependent.icdoPersonDependent.dependent_name;
            if (aobjPersonDependent.icdoPersonDependent.dependent_coverage_enddate != DateTime.MinValue)
                dr["EndDate"] = aobjPersonDependent.icdoPersonDependent.dependent_coverage_enddate;
            else
                dr["EndDate"] = DBNull.Value;
            dr["Plan"] = aobjPersonDependent.ibusPeronAccountDependent.ibusPlan.icdoPlan.plan_name;
            if (aobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth ||
                aobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
            {
                aobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccountGhdv.LoadCoverageCodeDescription();
                if (!string.IsNullOrEmpty(aobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccountGhdv.istrCoverageCode))
                {
                    dr["LevelOfcoverage"] = aobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.coverage_code + "-" +
                        aobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccountGhdv.istrCoverageCode;
                }
            }
            else
            {
                dr["LevelOfcoverage"] = aobjPersonDependent.ibusPeronAccountDependent.ibusPersonAccountGhdv.icdoPersonAccountGhdv.level_of_coverage_description;
            }
            dr["PERSLinkID"] = aobjPersonDependent.icdoPersonDependent.person_id;
            idtResultTable.Rows.Add(dr);
        }
    }
}
