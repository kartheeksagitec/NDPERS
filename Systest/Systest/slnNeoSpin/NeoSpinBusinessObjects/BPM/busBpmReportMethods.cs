using NeoBase.Common;
using NeoSpin.BusinessObjects;
using Sagitec.BusinessObjects;
using Sagitec.DBUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoBase.BPM
{
    public class busBpmReportMethods : busBase
    {

        public DataSet GetBPMCompletedActivity(DateTime madtStartDate, DateTime madtEndDate, string astrProcessId = null, string astrRole = null)
        {
            int lintProcessId = 0;
            if (astrProcessId.IsNull())
                lintProcessId = 0;
            else if (Int32.TryParse(astrProcessId, out lintProcessId))
                lintProcessId = Convert.ToInt32(astrProcessId);

            int lintRoleId = 0;
            if (astrRole.IsNull())
                lintRoleId = 0;
            else if (Int32.TryParse(astrRole, out lintRoleId))
                lintRoleId = Convert.ToInt32(astrRole);

            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.START_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.END_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.PROCESS);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.ROLE);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            if (madtStartDate.Equals(DateTime.MinValue))
            {
                madtStartDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.START_DATE] = madtStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            if (madtEndDate.Equals(DateTime.MinValue))
            {
                madtEndDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.END_DATE] = madtEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            DataTable ldtbBPMCompletedActivity = DBFunction.DBSelect("entReport.rptBPMCompletedActivity",
               new object[4] { lintProcessId, madtStartDate, madtEndDate, busNeoBaseConstants.BPMCOMPLETEDACTIVITY }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            DataTable ldtbBPMCompletedActivityBarChart = DBFunction.DBSelect("entReport.rptBPMCompletedActivityBarChart",
                new object[4] { lintProcessId, madtStartDate, madtEndDate, busNeoBaseConstants.BPMCOMPLETEDACTIVITY }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            if (lintProcessId > 0)
            {
                DataTable ldtbProcessName = DBFunction.DBSelect("entReport.GetProcessNameByProcessID", new object[1] { lintProcessId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldtbProcessName.IsNotNull() && ldtbProcessName.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS] = ldtbProcessName.Rows[0][busNeoBaseConstants.NAME];
                }
            }

            if (lintRoleId > 0)
            {
                DataTable ldtbRoleName = DBFunction.DBSelect("entReport.GetRoleByRoleID", new object[1] { lintRoleId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldtbRoleName.IsNotNull() && ldtbRoleName.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.ROLE] = ldtbRoleName.Rows[0]["role_description"];
                }
            }
            ldtbReportParams.Rows.Add(ldrReportParam);
            DataSet ldstBPMCompletedActivityReport = new DataSet();

            ldtbBPMCompletedActivity.TableName = ReportConstants.REPORT_TABLE01;
            ldstBPMCompletedActivityReport.Tables.Add(ldtbBPMCompletedActivity.Copy());

            ldtbBPMCompletedActivityBarChart.TableName = ReportConstants.REPORT_TABLE02;
            ldstBPMCompletedActivityReport.Tables.Add(ldtbBPMCompletedActivityBarChart.Copy());
            ldstBPMCompletedActivityReport.Tables.Add(ldtbReportParams.Copy());

            return ldstBPMCompletedActivityReport;
        }

        /// <summary>
        /// Gets In Progress activities of BPM
        /// </summary>
        /// <param name="astrProcessId"></param>
        /// <param name="adtStartDate"></param>
        /// <param name="adtEndDate"></param>
        /// <param name="astrRole"></param>
        /// <returns></returns>
        public DataSet GetBPMInProgessActivity(DateTime madtStartDate, DateTime madtEndDate, string astrProcessId = null, string astrRole = null)
        {
            int lintProcessId = 0;

            if (astrProcessId.IsNullOrEmpty())
                lintProcessId = 0;
            else if (Int32.TryParse(astrProcessId, out lintProcessId))
                lintProcessId = Convert.ToInt32(astrProcessId);

            int lintRoleId = 0;
            if (astrRole.IsNullOrEmpty())
                lintRoleId = 0;
            else if (Int32.TryParse(astrRole, out lintRoleId))
                lintRoleId = Convert.ToInt32(astrRole);

            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.START_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.END_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.PROCESS);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.ROLE);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            if (madtStartDate.Equals(DateTime.MinValue))
            {
                madtStartDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.START_DATE] = madtStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            if (madtEndDate.Equals(DateTime.MinValue))
            {
                madtEndDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.END_DATE] = madtEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            DataTable ldtbBPMActivityInProgress = DBFunction.DBSelect("entReport.rptBPMActivityInProgress",
               new object[4] { lintProcessId, madtStartDate, madtEndDate, busNeoBaseConstants.IN_PROCCESS }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            DataTable ldtbBPMActivityInProgressBarChart = DBFunction.DBSelect("entReport.rptBPMActivityInProgressBarChart",
                new object[4] { lintProcessId, madtStartDate, madtEndDate, busNeoBaseConstants.IN_PROCCESS }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            if (lintProcessId > 0)
            {
                DataTable ldtbProcessName = DBFunction.DBSelect("entReport.GetProcessNameByProcessID", new object[1] { lintProcessId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldtbProcessName.IsNotNull() && ldtbProcessName.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS] = ldtbProcessName.Rows[0][busNeoBaseConstants.NAME];
                }
            }

            if (lintRoleId > 0)
            {
                DataTable ldtbRoleName = DBFunction.DBSelect("entReport.GetRoleByRoleID", new object[1] { lintRoleId }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                if (ldtbRoleName.IsNotNull() && ldtbRoleName.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.ROLE] = ldtbRoleName.Rows[0]["role_description"];
                }
            }

            ldtbReportParams.Rows.Add(ldrReportParam);

            ldtbBPMActivityInProgress.TableName = ReportConstants.REPORT_TABLE01;
            DataSet ldstOrgMainReport = new DataSet();
            ldstOrgMainReport.Tables.Add(ldtbBPMActivityInProgress.Copy());
            ldtbBPMActivityInProgressBarChart.TableName = ReportConstants.REPORT_TABLE02;
            ldstOrgMainReport.Tables.Add(ldtbBPMActivityInProgressBarChart.Copy());

            ldstOrgMainReport.Tables.Add(ldtbReportParams.Copy());
            return ldstOrgMainReport;
        }

        /// <summary>
        /// Gets Unclaimed activities of BPM
        /// </summary>
        /// <param name="adtStartDate"></param>
        /// <param name="adtEndDate"></param>
        /// <returns></returns>
        public DataSet GetBPMUnClaimedActivity(DateTime adtStartDate, DateTime adtEndDate)
        {
            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.START_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.END_DATE);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            if (adtStartDate.Equals(DateTime.MinValue))
            {
                adtStartDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.START_DATE] = adtStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            if (adtEndDate.Equals(DateTime.MinValue))
            {
                adtEndDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.END_DATE] = adtEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }
            ldtbReportParams.Rows.Add(ldrReportParam);
            DataTable ldtbBPMUnClaimedActivity = DBFunction.DBSelect("entReport.rptBPMUnclaimedActivities",
               new object[2] { adtStartDate, adtEndDate }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            DataTable ldtbBPMUnClaimedActivityPieChart = DBFunction.DBSelect("entReport.rptUnClaimedActivitiesPieChart",
                new object[2] { adtStartDate, adtEndDate }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);


            DataSet ldstOrgMainReport = new DataSet();
            ldtbBPMUnClaimedActivity.TableName = ReportConstants.REPORT_TABLE01;
            ldstOrgMainReport.Tables.Add(ldtbBPMUnClaimedActivity.Copy());

            ldtbBPMUnClaimedActivityPieChart.TableName = ReportConstants.REPORT_TABLE02;
            ldstOrgMainReport.Tables.Add(ldtbBPMUnClaimedActivityPieChart.Copy());
            ldtbReportParams.TableName = ReportConstants.REPORT_TABLE03;
            ldstOrgMainReport.Tables.Add(ldtbReportParams.Copy());
            ldtbReportParams.TableName = ReportConstants.REPORT_TABLE04;
            ldstOrgMainReport.Tables.Add(ldtbReportParams.Copy());
            return ldstOrgMainReport;
        }

        /// <summary>
        /// Work flow User Snapshot Report
        /// </summary>
        /// Developer:Ashish Saklani 08-06-2020 PIR -ID:1888
        /// Added the following code to handle the parameter scenario of the Report.
        /// <param name="Userid"></param>
        /// <returns></returns>
        public DataSet GetWorkflowUserSnapshot(string astrUserID = null, string astrProcessID = null)
        {
            DataSet ldsWorkFlowUserSnapshot = new DataSet();
            Collection<IDbDataParameter> lcolParams = new Collection<IDbDataParameter>();
            IDbDataParameter strUserID = DBFunction.GetDBParameter("@var_USER_ID", DbType.String, astrUserID);
            lcolParams.Add(strUserID);
            IDbDataParameter strProcessID = DBFunction.GetDBParameter("@var_PROCESS_ID", DbType.Int32, Convert.ToInt32(astrProcessID));
            lcolParams.Add(strProcessID);

            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.PROCESS_ID);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.USER_ID);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            int ID = 0;
            string U_name = String.Empty;
            // Developer- Rahul Mane
            // Iteration - Main-Iteration10
            // Date - 07-29-2021
            // PIR - 2305 - The system is not displaying the details under the generated reports related to the workflows.
            if (DBFunction.IsOracleConnection()|| DBFunction.IsPostgreSQLConnection())
            {
                DBFunction.DBExecuteProcedure("USP_REP_USER_SNAPSHOT", lcolParams, iobjPassInfo.iconFramework, utlPassInfo.iobjPassInfo.itrnFramework);
                DataTable ldtActivityStatistics = DBFunction.DBSelect("entReport.GetAcitvityStatisticsByWorkflowUserSnapshot", iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtActivityStatistics.TableName = ReportConstants.REPORT_TABLE01;
                ldtActivityStatistics.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                ldtActivityStatistics.Columns.Add(new DataColumn("User_ID", typeof(string)));

                foreach (DataRow rowitem in ldtActivityStatistics.Rows)
                {

                    rowitem[10] = astrProcessID != null ? astrProcessID : "ALL"; ;
                    rowitem[11] = astrUserID != null ? astrUserID : "ALL";
                }

                if (ldtActivityStatistics.IsNotNull() && ldtActivityStatistics.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtActivityStatistics.Rows[0][busNeoBaseConstants.PROCESS_ID];
                    ldrReportParam[busNeoBaseConstants.USER_ID] = ldtActivityStatistics.Rows[0][busNeoBaseConstants.USER_ID];
                }
                ldsWorkFlowUserSnapshot.Tables.Add(ldtActivityStatistics.Copy());
                ldsWorkFlowUserSnapshot.Tables.Add(ldtbReportParams.Copy());
            }
            else
            {
                DataTable ldtActivityStatistics = DBFunction.DBSelect("entReport.GetAcitvityStatisticsByWorkflowUserSnapshot", new object[4] { Convert.ToInt32(astrProcessID), astrUserID, ID, U_name }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtActivityStatistics.TableName = ReportConstants.REPORT_TABLE01;

                ldtActivityStatistics.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                ldtActivityStatistics.Columns.Add(new DataColumn("User_ID", typeof(string)));

                foreach (DataRow rowitem in ldtActivityStatistics.Rows)
                {

                    rowitem[10] = astrProcessID != null ? astrProcessID : "ALL"; ;
                    rowitem[11] = astrUserID != null ? astrUserID : "ALL";
                }
                if (ldtActivityStatistics.IsNotNull() && ldtActivityStatistics.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtActivityStatistics.Rows[0][busNeoBaseConstants.PROCESS_ID];
                    ldrReportParam[busNeoBaseConstants.USER_ID] = ldtActivityStatistics.Rows[0][busNeoBaseConstants.USER_ID];
                }
                ldsWorkFlowUserSnapshot.Tables.Add(ldtActivityStatistics.Copy());
                ldsWorkFlowUserSnapshot.Tables.Add(ldtbReportParams.Copy());
            }




            return ldsWorkFlowUserSnapshot;
        }

        /// <summary>
        /// WorkFlow User History Report Method.
        /// </summary>
        /// Developer:Ashish Saklani 08-06-2020 PIR -ID:1888
        /// Added the following code to handle the parameter scenario of the Report.
        /// <param name="astrUserID"></param>
        /// <param name="aintProcessID"></param>
        /// <returns></returns>
        public DataSet GetWorkFlowUserHistory(DateTime madtStartDate, DateTime madtEndDate, string astrUserID = null, string astrProcessId = null)
        {
            int Id = 0;
            string U_name = string.Empty;
            DataSet ldsWorkFlowUserHistory = new DataSet();
            Collection<IDbDataParameter> lcolParams = new Collection<IDbDataParameter>();
            IDbDataParameter strStart = DBFunction.GetDBParameter("var_START_DATE", DbType.DateTime, madtStartDate);
            lcolParams.Add(strStart);
            IDbDataParameter strEnd = DBFunction.GetDBParameter("var_END_DATE", DbType.DateTime, madtEndDate);
            lcolParams.Add(strEnd);
            IDbDataParameter strUserID = DBFunction.GetDBParameter("var_USER_ID", DbType.String, astrUserID);
            lcolParams.Add(strUserID);
            IDbDataParameter strProcessID = DBFunction.GetDBParameter("var_PROCESS_ID", DbType.Int32, Convert.ToInt32(astrProcessId));
            lcolParams.Add(strProcessID);

            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.START_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.END_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.PROCESS_ID);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.USER_ID);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            if (madtStartDate.Equals(DateTime.MinValue))
            {
                madtStartDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.START_DATE] = madtStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            if (madtEndDate.Equals(DateTime.MinValue))
            {
                madtEndDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.END_DATE] = madtEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }
            // Developer- Rahul Mane
            // Iteration - Main-Iteration10
            // Date - 07-29-2021
            // PIR - 2305 - The system is not displaying the details under the generated reports related to the workflows.
            if (DBFunction.IsOracleConnection() || DBFunction.IsPostgreSQLConnection())
            {
                DBFunction.DBExecuteProcedure("USP_REP_USER_HISTORY", lcolParams, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                DataTable ldtActivityDetails = DBFunction.DBSelect("entReport.RptWorkFlowUserHistoryFirst", iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtActivityDetails.TableName = ReportConstants.REPORT_TABLE01;
                ldtActivityDetails.Columns.Add(new DataColumn("Initiated_Date_from", typeof(DateTime)));
                ldtActivityDetails.Columns.Add(new DataColumn("Initiated_Date_to", typeof(DateTime)));
                ldtActivityDetails.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                ldtActivityDetails.Columns.Add(new DataColumn("User_ID", typeof(string)));

                foreach (DataRow rowitem in ldtActivityDetails.Rows)
                {
                    rowitem[16] = madtStartDate != null ? madtStartDate : DateTime.MinValue;
                    rowitem[17] = madtEndDate != null ? madtEndDate : DateTime.MinValue; ;
                    rowitem[18] = astrProcessId != null ? astrProcessId : "ALL";
                    rowitem[19] = astrUserID != null ? astrUserID : "ALL";

                }
                if (ldtActivityDetails.IsNotNull() && ldtActivityDetails.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtActivityDetails.Rows[0][busNeoBaseConstants.PROCESS_ID];
                    ldrReportParam[busNeoBaseConstants.USER_ID] = ldtActivityDetails.Rows[0][busNeoBaseConstants.USER_ID];
                }
                ldsWorkFlowUserHistory.Tables.Add(ldtActivityDetails.Copy());
                ldsWorkFlowUserHistory.Tables.Add(ldtbReportParams.Copy());

            }
            else
            {
                DataTable ldtActivityDetails = DBFunction.DBSelect("entReport.RptWorkFlowUserHistoryFirst", new object[6] { Convert.ToInt32(astrProcessId), madtStartDate, madtEndDate, astrUserID, Id, U_name }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtActivityDetails.TableName = ReportConstants.REPORT_TABLE01;
                ldtActivityDetails.Columns.Add(new DataColumn("Initiated_Date_from", typeof(DateTime)));
                ldtActivityDetails.Columns.Add(new DataColumn("Initiated_Date_to", typeof(DateTime)));
                ldtActivityDetails.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                ldtActivityDetails.Columns.Add(new DataColumn("User_ID", typeof(string)));
                foreach (DataRow rowitem in ldtActivityDetails.Rows)
                {
                    rowitem[16] = madtStartDate != null ? madtStartDate : DateTime.MinValue;
                    rowitem[17] = madtEndDate != null ? madtEndDate : DateTime.MinValue; ;
                    rowitem[18] = astrProcessId != null ? astrProcessId : "ALL";
                    rowitem[19] = astrUserID != null ? astrUserID : "ALL";

                }
                if (ldtActivityDetails.IsNotNull() && ldtActivityDetails.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtActivityDetails.Rows[0][busNeoBaseConstants.PROCESS_ID];
                    ldrReportParam[busNeoBaseConstants.USER_ID] = ldtActivityDetails.Rows[0][busNeoBaseConstants.USER_ID];
                }
                ldsWorkFlowUserHistory.Tables.Add(ldtActivityDetails.Copy());
                ldsWorkFlowUserHistory.Tables.Add(ldtbReportParams.Copy());
            }



            return ldsWorkFlowUserHistory;
        }

        /// <summary>
        /// WorkFlow Process History Report
        /// </summary>
        /// Developer:Ashish Saklani 08-06-2020 PIR -ID:1888
        /// Added the following code to handle the parameter scenario of the Report.
        /// <param name="aintProcessID"></param>
        /// <returns></returns>
        public DataSet LoadWorkFlowProcessHistoryReport(DateTime madtStartDate, DateTime madtEndDate, string astrProcessID = null)
        {
            int dayscount = 0;
            string astrStartDate = string.Empty;
            string astrEndDate = string.Empty;
            string afterReportdata = string.Empty;


            DataSet ldsProcessHistory = new DataSet();
            List<string> somevalueafterupdation = new List<string>();
            Collection<IDbDataParameter> lcolParams = new Collection<IDbDataParameter>();
            IDbDataParameter strStart = DBFunction.GetDBParameter("var_START_DATE", DbType.DateTime, madtStartDate);
            lcolParams.Add(strStart);
            IDbDataParameter strEnd = DBFunction.GetDBParameter("var_END_DATE", DbType.DateTime, madtEndDate);
            lcolParams.Add(strEnd);
            IDbDataParameter strProcessID = DBFunction.GetDBParameter("var_PROCESS_ID", DbType.Int32, Convert.ToInt32(astrProcessID));
            lcolParams.Add(strProcessID);

            DataTable ldtbReportParams = new DataTable
            {
                TableName = ReportConstants.REPORT_TABLE03
            };
            ldtbReportParams.Columns.Add(busNeoBaseConstants.START_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.END_DATE);
            ldtbReportParams.Columns.Add(busNeoBaseConstants.PROCESS_ID);
            DataRow ldrReportParam = ldtbReportParams.NewRow();

            if (madtStartDate.Equals(DateTime.MinValue))
            {
                madtStartDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.START_DATE] = madtStartDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            if (madtEndDate.Equals(DateTime.MinValue))
            {
                madtEndDate = new DateTime(1753, 1, 1);
            }
            else
            {
                ldrReportParam[busNeoBaseConstants.END_DATE] = madtEndDate.ToString(busNeoBaseConstants.REPORT_DATE_FORMAT);
            }

            // Developer- Rahul Mane
            // Iteration - Main-Iteration10
            // Date - 07-29-2021
            // PIR - 2305 - The system is not displaying the details under the generated reports related to the workflows.
            if (DBFunction.IsOracleConnection() || DBFunction.IsPostgreSQLConnection())
            {

                DBFunction.DBExecuteProcedure("USP_REP_PRCS_HIST", lcolParams, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                DataTable ldtPrcssHist = DBFunction.DBSelect("entReport.LoadProcessHistoryResult", iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtPrcssHist.TableName = ReportConstants.REPORT_TABLE01;
                ldtPrcssHist.Columns.Add(new DataColumn("Initiated_Date_from", typeof(DateTime)));
                ldtPrcssHist.Columns.Add(new DataColumn("Initiated_Date_to", typeof(DateTime)));
                ldtPrcssHist.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                foreach (DataRow rowitem in ldtPrcssHist.Rows)
                {
                    rowitem[11] = madtStartDate != null ? madtStartDate : DateTime.MinValue;
                    rowitem[12] = madtEndDate != null ? madtEndDate : DateTime.MinValue; ;
                    rowitem[13] = astrProcessID != null ? astrProcessID : "ALL";
                }
                if (ldtPrcssHist.IsNotNull() && ldtPrcssHist.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtPrcssHist.Rows[0][busNeoBaseConstants.PROCESS_ID];
                }
                ldsProcessHistory.Tables.Add(ldtPrcssHist.Copy());
                ldsProcessHistory.Tables.Add(ldtbReportParams.Copy());
            }
            else
            {
                DataTable ldtPrcssHist = DBFunction.DBSelect("entReport.LoadProcessHistoryResult", new object[3] { Convert.ToInt32(astrProcessID), madtStartDate, madtEndDate }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                ldtPrcssHist.TableName = ReportConstants.REPORT_TABLE01;

                ldtPrcssHist.Columns.Add(new DataColumn("Initiated_Date_from", typeof(DateTime)));
                ldtPrcssHist.Columns.Add(new DataColumn("Initiated_Date_to", typeof(DateTime)));
                ldtPrcssHist.Columns.Add(new DataColumn("Prcs_ID", typeof(string)));
                foreach (DataRow rowitem in ldtPrcssHist.Rows)
                {
                    rowitem[11] = madtStartDate != null ? madtStartDate : DateTime.MinValue;
                    rowitem[12] = madtEndDate != null ? madtEndDate : DateTime.MinValue; ;
                    rowitem[13] = astrProcessID != null ? astrProcessID : "ALL";
                }
                if (ldtPrcssHist.IsNotNull() && ldtPrcssHist.Rows.Count > 0)
                {
                    ldrReportParam[busNeoBaseConstants.PROCESS_ID] = ldtPrcssHist.Rows[0][busNeoBaseConstants.PROCESS_ID];
                }
                ldsProcessHistory.Tables.Add(ldtPrcssHist.Copy());
                ldsProcessHistory.Tables.Add(ldtbReportParams.Copy());
            }
            return ldsProcessHistory;
        }

    }
}
