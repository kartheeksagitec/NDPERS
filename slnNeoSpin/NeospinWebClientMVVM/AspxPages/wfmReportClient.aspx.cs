using CrystalDecisions.CrystalReports.Engine;
using NeoSpin.BusinessTier;
using Sagitec.Common;
using Sagitec.Interface;
using Sagitec.MVVMClient;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Services.Description;
using NeoSpin.BusinessObjects;

namespace Neo.AspxPages
{
    public partial class wfmReportClient : System.Web.UI.Page
    {
        private static string _istrShortDateString = "01/01/0001";
        private DataSet idstResult;
        protected MVVMSession iobjSessionData;
        Import lRprtObj;
        ReportDocument crystalReport;        
        protected override void OnInit(EventArgs aEventArgs)
        {

            // lRprtObj = ReportFactory.GetInstance();
            if (lRprtObj is srvReports)
            {
                iobjSessionData = new MVVMSession(Session.SessionID);
                //Pushawart: Restricted response header from passing unwanted information to browser.
                Response.Cache.SetCacheability(HttpCacheability.ServerAndNoCache);
                Response.Cache.SetNoStore();
                Response.AppendHeader("Cache-Control", "no-cache, no-store, private, must-revalidate"); // HTTP 1.1.
                Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
                Response.AppendHeader("Expires", "0"); // Proxies.

                base.OnInit(aEventArgs);
                if (iobjSessionData["MVVMReportDataSource"] != null)          // if the generate button is clicked, execute the report; otherwise not.
                {
                    DataSet ds = iobjSessionData["MVVMReportDataSource"] as DataSet;
                    iobjSessionData["ReportDataSource"] = ds;
                    ExecuteReport();
                }
            }
            else
                base.OnInit(aEventArgs);
             MVVMSession lobjSessionData = new MVVMSession(Session.SessionID);
            crystalReport = new ReportDocument();

            if (Request.QueryString["ddlReports"] != null)
            {
                string lstrReportFileName = null;
                if (Request.QueryString["ddlReports"] == "rptESSMemberContributionsAtAgencyLevelNew")
                {
                    lstrReportFileName = GetReportName(lobjSessionData["MVVMReportDataSource"] as DataSet);
                }
                else
                {
                    lstrReportFileName = Request.QueryString["ddlReports"] as string;

                }
                crystalReport.Load(Server.MapPath("~/Reports/" + lstrReportFileName + ".rpt"));
            }
            if (lobjSessionData["MVVMReportDataSource"] != null && lobjSessionData["MVVMReportDataSource"] is DataSet)
            {
                crystalReport.SetDataSource(lobjSessionData["MVVMReportDataSource"]);
                ExecuteReport();
                CRViewer.ToolPanelView = CrystalDecisions.Web.ToolPanelViewType.None;
                CRViewer.ReportSource = crystalReport;
            }
        }

        protected override object SaveViewState()
        {

            srvServers lsrvServers = new srvServers();
            try
            {
                lsrvServers.ConnectToBT("wfmPersonLookup");
                if (iobjSessionData != null)
                    iobjSessionData.Update(lsrvServers.isrvBusinessTier);
                return base.SaveViewState();
            }
            finally
            {
                lsrvServers.CloseChannels();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
        private void OnReportDocInit(object sender, System.EventArgs e)
        {
            ReportDocument irptDocument = new ReportDocument();
            irptDocument.SetDatabaseLogon("", "");
        }



        protected override void OnUnload(EventArgs e)
        {
            if (crystalReport != null)
            {
                crystalReport.Close();
                crystalReport.Dispose();
            }
            base.OnUnload(e);
        }
        private string GetReportName(DataSet adstReportData)
        {
            if (adstReportData != null && adstReportData.Tables.Contains("ReportName"))
            {
                return Convert.ToString(adstReportData.Tables["ReportName"].Rows[0]["ReportName"]);
            }
            return null;
        }
        /// <summary>
        /// Set All Report Parameters
        /// </summary>

        //FW Upgrade PIR ID :	15957
        public void ExecuteReport() //framework upgradation: Private to public
        {
            //Assigning the Parameter   
            string lstrReport = Request.QueryString["ddlReports"] as string;
            switch (lstrReport)
            {
                case busConstant.ReportNamePensionPaymentHistory:
                    string lstbeginDate = Request.QueryString["StartDate"] as string;
                    string lstendDate = Request.QueryString["EndDate"] as string;
                    crystalReport.SetParameterValue("txtStartDate", !string.IsNullOrEmpty(lstbeginDate) ? lstbeginDate : _istrShortDateString);
                    crystalReport.SetParameterValue("txtEndDate", !string.IsNullOrEmpty(lstendDate) ? lstendDate : _istrShortDateString);
                    break;
                case busConstant.ReportNamePaymentListingReport:
                    srvServers lsrvServers = new srvServers();
                    try
                    {
                        lsrvServers.ConnectToBT();
                        string lststatus = Request.QueryString["Status"] as string;
                        DataTable ldtbCodevalue = lsrvServers.isrvDbCache.GetCacheData("sgs_code_value", "code_id=2515 and code_value='" + lststatus + "'");
                        lststatus = ldtbCodevalue.Rows.Count > 0 ? ldtbCodevalue.Rows[0]["Description"].ToString() : string.Empty;
                        string lststartDate = Request.QueryString["STARTDATE"] as string;
                        string lstrendDate = Request.QueryString["ENDDATE"] as string;
                        crystalReport.SetParameterValue("txtPaymentStatus", !string.IsNullOrEmpty(lststatus) ? lststatus : string.Empty);
                        crystalReport.SetParameterValue("txtStartDate", !string.IsNullOrEmpty(lststartDate) ? lststartDate : _istrShortDateString);
                        crystalReport.SetParameterValue("txtEndDate", !string.IsNullOrEmpty(lstrendDate) ? lstrendDate : _istrShortDateString);
                    }
                    finally
                    {
                        lsrvServers.CloseChannels();
                    }
                    break;
                case busConstant.ReportNameOverPaymentReport:
                    string lstApprovalYear = Request.QueryString["APPROVAL_YEAR"] as string;
                    crystalReport.SetParameterValue("txtYear", !string.IsNullOrEmpty(lstApprovalYear) ? lstApprovalYear : string.Empty);
                    break;
                case busConstant.ReportNameSummaryreportofalladhocpaymentsforaMonth:
                    string lstrbeginDate = Request.QueryString["STARTDATE"] as string;
                    string lstrrendDate = Request.QueryString["ENDDATE"] as string;
                    string lstScheduletype = Request.QueryString["SCHEDULETYPE"] as string;
                    srvServers lsrvSchServers = new srvServers();
                    try
                    {
                        lsrvSchServers.ConnectToBT();
                        DataTable ldtbScheCodevalue = lsrvSchServers.isrvDbCache.GetCacheData("sgs_code_value", "code_id=2501 and code_value='" + lstScheduletype + "'");
                        string lstrSchDesc = ldtbScheCodevalue.Rows.Count > 0 ? Convert.ToString(ldtbScheCodevalue.Rows[0]["Description"]) : string.Empty;
                        crystalReport.SetParameterValue("txtStartDate", !string.IsNullOrEmpty(lstrbeginDate) ? lstrbeginDate : _istrShortDateString);
                        crystalReport.SetParameterValue("txtEndDate", !string.IsNullOrEmpty(lstrrendDate) ? lstrrendDate : _istrShortDateString);
                        crystalReport.SetParameterValue("txtScheduletype", !string.IsNullOrEmpty(lstScheduletype) && lstScheduletype == "VNPM" ? "Vendor" : lstrSchDesc);
                    }
                    finally
                    {
                        lsrvSchServers.CloseChannels();
                    }
                    break;
                case busConstant.ReportNameMaritalStatusChangedRecords:
                    string lstrWeekStartDate = Request.QueryString["Week_Start_Date"] as string;
                    string lstrWeekendDate = Request.QueryString["Week_End_Date"] as string;
                    crystalReport.SetParameterValue("txtWeekStartDate", !string.IsNullOrEmpty(lstrWeekStartDate) ? lstrWeekStartDate : _istrShortDateString);
                    crystalReport.SetParameterValue("txtWeekEndDate", !string.IsNullOrEmpty(lstrWeekendDate) ? lstrWeekendDate : _istrShortDateString);
                    break;
                case busConstant.ReportNameMedicareSplitError:
                    string lstcurrentdate = Request.QueryString["currentdate"] as string;
                    crystalReport.SetParameterValue("txtcurrentdate", !string.IsNullOrEmpty(lstcurrentdate) ? lstcurrentdate : _istrShortDateString);
                    break;
                case busConstant.ReportNameListOfAppointments:
                    string lststartdate = Request.QueryString["start_date"] as string;
                    string lstenddate = Request.QueryString["end_date"] as string;
                    crystalReport.SetParameterValue("txtStartDate", string.IsNullOrEmpty(lststartdate) ? _istrShortDateString : lststartdate);
                    crystalReport.SetParameterValue("txtEndDate", string.IsNullOrEmpty(lstenddate) ? _istrShortDateString : lstenddate);
                    break;
                case busConstant.ReportNameLeaveOfAbsence:
                case busConstant.ReportNameMissingRetirementEnrollment:
                    string lstsOrgID = Request.QueryString["ORG_ID"] as string;
                    int aintOrgID = 0;
                    if (!string.IsNullOrEmpty(lstsOrgID) && int.TryParse(lstsOrgID, out aintOrgID))
                    {
                        IBusinessTier lsrvBusinessTier = null;
                        Tuple<string, string> ltupOrgCodeOrgName = null;
                        try
                        {
                            string lstrUrl = string.Format(ConfigurationManager.AppSettings["BusinessTierUrl"], "srvOrganization");
                            lsrvBusinessTier = WCFClient<IBusinessTier>.CreateChannel(lstrUrl);
                            Hashtable lhstParameter = new Hashtable();
                            lhstParameter.Add("aintOrgID", aintOrgID);
                            ltupOrgCodeOrgName = (Tuple<string, string>)lsrvBusinessTier.ExecuteMethod("GetOrgCodeOrgNameById", lhstParameter, false, new System.Collections.Generic.Dictionary<string, object>());
                        }
                        finally
                        {
                            Sagitec.Common.HelperFunction.CloseChannel(lsrvBusinessTier);
                        }
                        crystalReport.SetParameterValue("OrgName", ltupOrgCodeOrgName.Item1);
                        crystalReport.SetParameterValue("OrgCode", ltupOrgCodeOrgName.Item2);
                    }
                    break;
            }
        }
    }
}