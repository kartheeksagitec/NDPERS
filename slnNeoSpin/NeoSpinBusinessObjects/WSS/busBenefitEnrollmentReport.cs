using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sagitec.BusinessObjects;
using System.Data;
using System.Collections.ObjectModel;
using NeoSpin.CustomDataObjects;
using NeoSpin.DataObjects;
using Sagitec.CustomDataObjects;
using System.Collections;
using Sagitec.Common;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using Sagitec.DBUtility;

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busBenefitEnrollmentReport : busExtendBase
    {
        public Collection<busWssMessageDetail> iclbWSSMessageDetails { get; set; }

        public busOrganization ibusOrganization { get; set; }

        public busContact ibusContact { get; set; }

        public string istrMessageText { get; set; }

        public ReportDocument irptDocument;

        public DateTime ldtFromDate, ldtToDate;

        public string istrOrgCodeAndName { get; set; } // PIR 10053

        public DataTable idtbCachedLifeRate { get; set; }

        public DataTable idtbCachedDentalRate { get; set; }

        public DataTable idtbCachedVisionRate { get; set; }

        public DataTable idtbCachedHealthRate { get; set; }

        public DataTable idtbCachedEAPRate { get; set; }

        public DataTable idtbCachedLtcRate { get; set; }

        public string istrIsPIR9115Enabled { get; set; }

        private busOrgPlan _ibusOrgPlan;
        public busOrgPlan ibusOrgPlan
        {
            get { return _ibusOrgPlan; }
            set { _ibusOrgPlan = value; }
        }

        private busOrgPlan _ibusProviderOrgPlan;
        public busOrgPlan ibusProviderOrgPlan
        {
            get { return _ibusProviderOrgPlan; }
            set { _ibusProviderOrgPlan = value; }
        }

        private Collection<busWssPersonAccountEnrollmentRequest> _iclbWssPersonAccountEnrollmentRequest;
        public Collection<busWssPersonAccountEnrollmentRequest> iclbWssPersonAccountEnrollmentRequest
        {
            get { return _iclbWssPersonAccountEnrollmentRequest; }
            set { _iclbWssPersonAccountEnrollmentRequest = value; }
        }

        public void LoadGeneratedReportsFromInbox(int aintOrgID, int aintContactID)
        {
            iclbWSSMessageDetails = new Collection<busWssMessageDetail>();
            if (string.IsNullOrEmpty(istrMessageText))
                istrMessageText = string.Empty;
            DataTable ldtMessageDetail = Select("cdoWssMessageHeader.LoadGeneratedReports",
                new object[2] { ibusOrganization.icdoOrganization.org_id, istrMessageText });

            // PIR 10027 start - remove duplicates
            List<String> listCorLink = new List<String>();
            DataTable ldtReports = new DataTable();
            ldtReports = ldtMessageDetail.Clone();

            foreach (DataRow dr in ldtMessageDetail.Rows)
            {
                if (!listCorLink.Contains(dr["CORRESPONDENCE_LINK"].ToString()))
                {
                    //DBFunction.DBNonQuery("cdoWssMessageHeader.DeleteWssMessage", new object[1] { Convert.ToInt32(dr["WSS_MESSAGE_ID"]) },
                    //                        iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework, iobjPassInfo.isrvMetaDataCache);
                    //continue;
                    listCorLink.Add(dr["CORRESPONDENCE_LINK"].ToString());
                    ldtReports.ImportRow(dr); 
                }
            }
            // PIR 10027 end

            foreach (DataRow dr in ldtReports.Rows)
            {
                busWssMessageDetail lobDetail = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
                lobDetail.ibusWSSMessageHeader = new busWssMessageHeader { icdoWssMessageHeader = new cdoWssMessageHeader() };

                lobDetail.icdoWssMessageDetail.LoadData(dr);
                lobDetail.ibusWSSMessageHeader.icdoWssMessageHeader.LoadData(dr);
                if (dr["REPORT_CREATED_DATE"] != DBNull.Value && (Convert.ToDateTime(dr["REPORT_CREATED_DATE"]) > DateTime.Now.AddMonths(-6))) //PIR 16754
                {
                    iclbWSSMessageDetails.Add(lobDetail);
                }
            }
        }

        public void LoadOrganization(int aintOrgID)
        {
            ibusOrganization = new busOrganization();
            ibusOrganization.FindOrganization(aintOrgID);
        }

        public bool LoadContact(int aintContactID)
        {
            ibusContact = new busContact();
            return ibusContact.FindContact(aintContactID);
        }

        // PIR 9979
        public void LoadOrgPlan(DateTime adtEffectiveDate, int aintOrgID, int aintPlanId)
        {
            if (_ibusOrgPlan == null)
            {
                _ibusOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            }
            if (aintOrgID != 0)
            {
                Collection<busOrgPlan> lclbOrgPlan = new Collection<busOrgPlan>();
                DataTable ldtbList = Select<cdoOrgPlan>(
                              new string[2] { "org_id", "plan_id" },
                              new object[2] { aintOrgID, aintPlanId }, null, null);
                lclbOrgPlan = GetCollection<busOrgPlan>(ldtbList, "icdoOrgPlan");
                LoadOrgPlan(adtEffectiveDate, lclbOrgPlan);
            }
        }

        public void LoadOrgPlan(DateTime adtEffectiveDate, Collection<busOrgPlan> aclbOrgPlan)
        {
            if (aclbOrgPlan != null)
            {
                foreach (busOrgPlan lobjOrgPlan in aclbOrgPlan)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjOrgPlan.icdoOrgPlan.participation_start_date,
                                          lobjOrgPlan.icdoOrgPlan.participation_end_date))
                    {
                        ibusOrgPlan = lobjOrgPlan;
                        break;
                    }
                }
            }
        }
        // PIR 9979 end

        public string IsPIR9115Enabled()
        {
            istrIsPIR9115Enabled = busGlobalFunctions.GetData1ByCodeValue(52, "9115", iobjPassInfo);
            return istrIsPIR9115Enabled.IsNotNullOrEmpty() ? istrIsPIR9115Enabled : string.Empty;
        }


        public ArrayList btnGenerateReport_Click()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();
          
            DataTable ldtbBenefitEnrollment = busBase.Select("cdoWssPersonAccountEnrollmentRequest.rptBenefitEnrollment", new object[1] { ibusOrganization.icdoOrganization.org_id });

            DateTime ldtPrevYearLastDay = new DateTime(DateTime.Now.Year - 1, 12, 31);
            if (ldtbBenefitEnrollment.Rows.Count > 0)
            {
                //To display Org name on report heading
                istrOrgCodeAndName = ibusOrganization.icdoOrganization.org_code + " " + ibusOrganization.icdoOrganization.org_name; // PIR 10053

                DateTime createdDate = DateTime.Now;

                // PIR 10172 
                string istrMessageText = busGlobalFunctions.GetMessageTextByMessageID(10082, iobjPassInfo);
                DataTable ldtLastCreatedDate = busBase.Select("cdoWssMessageHeader.GetLastReportCreatedDate",
                    new object[2] { ibusOrganization.icdoOrganization.org_id, istrMessageText });

                if (ldtLastCreatedDate.Rows.Count > 0)
                {
                    ldtFromDate = Convert.ToDateTime(ldtLastCreatedDate.Rows[0]["REPORT_CREATED_DATE"]);
                }
                else
                {
                    ldtFromDate = Convert.ToDateTime(ldtbBenefitEnrollment.Compute("min(POSTED_DATE)", string.Empty));
                }

                ldtToDate = createdDate;

                // PIR 10172 end

                string lstrPath = CreateReport("rptBenefitEnrollment.rpt", ldtbBenefitEnrollment, createdDate.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "_" + ibusOrganization.icdoOrganization.org_code + "_");

                UpdateReportGeneratedFlags(ibusOrganization.icdoOrganization.org_id);

                int lintUpdatedRowCount = 0;
                lintUpdatedRowCount= DBFunction.DBNonQuery("cdoWssMessageHeader.UpdateReportLink", new object[4] { ibusOrganization.icdoOrganization.org_id, 
                                    lstrPath + ".pdf", createdDate, busGlobalFunctions.GetMessageTextByMessageID(10082, iobjPassInfo)},
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                //PIR 14573-On Generate Report button click if no message(Message Id-10082) exists we need to insert a message header and details.
                if (lintUpdatedRowCount < 1 && !String.IsNullOrEmpty(lstrPath))
                {
                    string lstrPrioityValue = string.Empty;
                    PublishESSMessage(0, 0, busGlobalFunctions.GetDBMessageTextByDBMessageID(10, iobjPassInfo, ref lstrPrioityValue), lstrPrioityValue,
                                      aintOrgID: ibusOrganization.icdoOrganization.org_id, astrCorrespondenceLink: lstrPath + ".pdf");
                }
                // Create Benefit Enrollment report as excel
                busNeoSpinBase lbusBase = new busNeoSpinBase();
                string lstrGenExcelReportName = lbusBase.CreateExcelReport("rptBenefitEnrollment_Excel.rpt", ldtbBenefitEnrollment,
                            string.Empty, busConstant.ReportESSPath);
            }
            else
            {
                DBFunction.DBNonQuery("cdoWssMessageHeader.UpdateClearMessageFlag", new object[2] { ibusContact.icdoContact.contact_id, ibusOrganization.icdoOrganization.org_id },
                                         iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                iobjPassInfo.Commit();
                iobjPassInfo.BeginTransaction();
                lobjError.istrErrorMessage = "No benefit enrollment changes have been made since you last ran the report.";
                larrList.Add(lobjError);
            }
            return larrList;
        }

        private void UpdateReportGeneratedFlags(int aintOrgId)
        {
            DBFunction.DBNonQuery("cdoWssPersonAccountEnrollmentRequest.UpdateBenefitEnrollmentFlag", new object[1] { aintOrgId },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }


        public static void PublishESSMessage(int aintWssMessageId, int aintMessageID, string astrMessageText, string astrPriorityValue,
                                            int aintPlanID = 0, int aintOrgID = 0, string astrContactRoleValue = null, int aintContactID = 0, string astrEmpCategoryValue = null,
                                            string astrWebLink = null, string astrCorrespondenceLink = null, int aintTrackingID = 0, string astrTemplateName = null)
        {
            DataTable ldtOrgContact = busBase.Select("cdoOrgContact.GetOrgContactIds", new object[1] { aintOrgID});

            if (ldtOrgContact.Rows.Count > 0)
            {
                busWssMessageHeader lobjMessageHeader = new busWssMessageHeader { icdoWssMessageHeader = new cdoWssMessageHeader() };
                lobjMessageHeader.icdoWssMessageHeader.message_id = aintMessageID;
                lobjMessageHeader.icdoWssMessageHeader.message_text = astrMessageText;
                lobjMessageHeader.icdoWssMessageHeader.priority_value = astrPriorityValue;
                lobjMessageHeader.icdoWssMessageHeader.audience_value = busConstant.WSS_MessageBoard_Audience_Employer;
                lobjMessageHeader.icdoWssMessageHeader.plan_id = aintPlanID;
                lobjMessageHeader.icdoWssMessageHeader.org_id = aintOrgID;
                lobjMessageHeader.icdoWssMessageHeader.contact_id = aintContactID;
                lobjMessageHeader.icdoWssMessageHeader.contact_role_value = astrContactRoleValue;
                lobjMessageHeader.icdoWssMessageHeader.emp_category_value = astrEmpCategoryValue;
                if (aintWssMessageId > 0)
                    lobjMessageHeader.icdoWssMessageHeader.wss_message_id = aintWssMessageId;
                else
                    lobjMessageHeader.icdoWssMessageHeader.Insert();

                DateTime adtReportCreatedDate = DateTime.Now;
                foreach (DataRow dr in ldtOrgContact.Rows)
                {
                    PublishESSMessageDetail(Convert.ToInt32(dr["CONTACT_ID"]), lobjMessageHeader.icdoWssMessageHeader.wss_message_id, aintOrgID, adtReportCreatedDate, astrCorrespondenceLink);
                }
            }
        }

        private static void PublishESSMessageDetail(int aintContactId, int aintWSSMessageID, int aintOrgId, DateTime adtReportCreatedDate, string astrCorrespondenceLink = null)
        {
            busWssMessageDetail lobjMessageDetail = new busWssMessageDetail { icdoWssMessageDetail = new cdoWssMessageDetail() };
            lobjMessageDetail.icdoWssMessageDetail.wss_message_id = aintWSSMessageID;
            lobjMessageDetail.icdoWssMessageDetail.org_id = aintOrgId;
            lobjMessageDetail.icdoWssMessageDetail.contact_id = aintContactId;
            lobjMessageDetail.icdoWssMessageDetail.clear_message_flag = busConstant.Flag_No;
            if (!String.IsNullOrEmpty(astrCorrespondenceLink))//PIR 14573
            {
                lobjMessageDetail.icdoWssMessageDetail.correspondence_link = astrCorrespondenceLink;
                lobjMessageDetail.icdoWssMessageDetail.report_created_date = adtReportCreatedDate;
            }
            lobjMessageDetail.icdoWssMessageDetail.Insert();        
        }

        private string iabsRptDefPath;
        private string iabsRptGenPath;

        //Call this Method outside the Loop (From Caller) for Optimization
        public void InitializeReportBuilder(string astrReportGNPath)
        {
            iabsRptDefPath = iobjPassInfo.isrvDBCache.GetPathInfo("BatchRptDF");
            iabsRptGenPath = iobjPassInfo.isrvDBCache.GetPathInfo(astrReportGNPath);
        }

        public string CreateReport(string astrReportName, System.Data.DataTable adstResult, string astrPrefix, string astrReportGNPath = busConstant.ReportESSPath)
        {
            InitializeReportBuilder(astrReportGNPath);
            string lstrReportFullName = string.Empty;
            irptDocument = new ReportDocument();
            irptDocument.InitReport += new EventHandler(this.OnReportDocInit); // Add event handler for report document init            
            irptDocument.Load(iabsRptDefPath + astrReportName);
            // gets the data and bind to the report doc control
            irptDocument.SetDataSource(adstResult);

            
            irptDocument.SetParameterValue("FromDate", ldtFromDate);
            irptDocument.SetParameterValue("ToDate", ldtToDate);
            irptDocument.SetParameterValue("OrgCodeAndName", istrOrgCodeAndName); // PIR 10053
            irptDocument.SetParameterValue("ReportName", busConstant.BenefitEnrollmentReportTitle); // PIR 23729

            lstrReportFullName = iabsRptGenPath + astrPrefix + astrReportName;
            irptDocument.ExportToDisk(ExportFormatType.PortableDocFormat, lstrReportFullName + ".pdf");
            irptDocument.Close();
            irptDocument.Dispose();
            return lstrReportFullName;
        }

        // Initialize the report documnet. This event removes any databse logon information 
        // saved in the report. The call to Load the report in the above function fires this event.
        private void OnReportDocInit(object sender, System.EventArgs e)
        {
            irptDocument.SetDatabaseLogon("", "");
        }

    }
}
