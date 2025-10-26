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
using System.Globalization;
using System.Collections;
using Sagitec.CorBuilder;
#endregion
namespace NeoSpinBatch
{
    class IBSOverDueBatchDelinquentLetter2 : busNeoSpinBatch
    {
        public IBSOverDueBatchDelinquentLetter2()
        {
        }

        public busIbsHeader ibusLastPostedRegularIBSHeader { get; set; }
        public void LoadLastPostedRegularIBSHeader()
        {
            ibusLastPostedRegularIBSHeader = new busIbsHeader { icdoIbsHeader = new cdoIbsHeader() };
            DataTable ldtbList = busNeoSpinBase.SelectWithOperator<cdoIbsHeader>(
              new string[2] { "report_type_value", "report_status_value" },
               new string[2] { "=", "=" },
              new object[2] { busConstant.IBSHeaderReportTypeRegular, busConstant.IBSHeaderStatusPosted}, "billing_month_and_year desc");
            if (ldtbList.Rows.Count > 0)
            {
                ibusLastPostedRegularIBSHeader.icdoIbsHeader.LoadData(ldtbList.Rows[0]);
            }
        }

        public void CreateIBSDelinquentLetter()
        {
            istrProcessName = "2nd Delinquent Letter";
            idlgUpdateProcessLog("IBS OverDue Batch - 2nd Delinquent Letter", "INFO", istrProcessName);
            LoadLastPostedRegularIBSHeader();
            DataTable ldtbIBSMbrs = busNeoSpinBase.Select("cdoIbsHeader.LoadIBSMembers", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtbRemittance = busNeoSpinBase.Select("cdoRemittance.GetAllRemittanceAfterLastIBS", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtIBSPersonSummary = busNeoSpinBase.Select<cdoIbsPersonSummary>(new string[1] { "ibs_header_id" },
                                                                                        new object[1] { ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id }, null, null);
            DataTable ldtReportData = new DataTable();
            ldtReportData = ldtbIBSMbrs.Clone();
            int lintPreviousPersonID = 0;
            decimal ldecAmountDue = 0.00M;
            foreach (DataRow ldrRow in ldtbIBSMbrs.Rows)
            {                
                var lbusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
                lbusPersonAccount.icdoPersonAccount.LoadData(ldrRow);

                lbusPersonAccount.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lbusPersonAccount.ibusPerson.icdoPerson.LoadData(ldrRow);

                lbusPersonAccount.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusPersonAccount.ibusPlan.icdoPlan.LoadData(ldrRow);

                lbusPersonAccount.ibusPersonAccountGHDV = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv(), icdoPersonAccount = new cdoPersonAccount() };

                if (lintPreviousPersonID != lbusPersonAccount.icdoPersonAccount.person_id)
                {
                    ldecAmountDue = 0.00M;
                    idlgUpdateProcessLog("Processing PERSLinkID " + Convert.ToString(lbusPersonAccount.icdoPersonAccount.person_id)
                                , "INFO", istrProcessName);
                                        
                    DataRow[] ldarrIBSSummary = ldtIBSPersonSummary.FilterTable(busConstant.DataType.Numeric, "person_id", lbusPersonAccount.icdoPersonAccount.person_id);
                    if (ldarrIBSSummary.Length > 0)
                    {
                        ldecAmountDue = ldarrIBSSummary[0]["balance_forward"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldarrIBSSummary[0]["balance_forward"]);
                    }
                    DataRow[] ldarrRemittance = ldtbRemittance.FilterTable(busConstant.DataType.Numeric, "person_id", lbusPersonAccount.icdoPersonAccount.person_id);
                    if (ldarrRemittance.Length > 0)
                    {
                        ldecAmountDue -= ldarrRemittance[0]["payments_made"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldarrRemittance[0]["payments_made"]);
                    }

                    if (ldecAmountDue > 0.00M)
                    {
                        try
                        {
                            /*ArrayList larrlist = new ArrayList();
                            larrlist.Add(lbusPersonAccount);
                            idlgUpdateProcessLog("Creating 2nd Delinquent Letter for PERSLinkID " + Convert.ToString(lbusPersonAccount.icdoPersonAccount.person_id)
                                , "INFO", istrProcessName);
                            Hashtable lhstDummyTable = new Hashtable();
                            lhstDummyTable.Add("sfwCallingForm", "Batch");
                            CreateCorrespondence("PAY-4302", iobjPassInfo.istrUserID, iobjPassInfo.iintUserSerialID, larrlist, lhstDummyTable);
                            idlgUpdateProcessLog("2nd Delinquent Letter created", "INFO", istrProcessName);*/

                            DataRow ldrReportData = ldtReportData.NewRow();
                            ldrReportData["person_id"] = lbusPersonAccount.ibusPerson.icdoPerson.person_id;
                            ldrReportData["last_name"] = lbusPersonAccount.ibusPerson.icdoPerson.last_name;
                            ldrReportData["first_name"] = lbusPersonAccount.ibusPerson.icdoPerson.first_name;
                            ldtReportData.Rows.Add(ldrReportData);
                            ldtReportData.AcceptChanges();
                        }
                        catch (Exception _exc)
                        {
                            idlgUpdateProcessLog("ERROR:" + _exc.Message, "INFO", istrProcessName);
                        }
                    }
                }
                lintPreviousPersonID = lbusPersonAccount.icdoPersonAccount.person_id;

                lbusPersonAccount = null;
            }
            if (ldtReportData.Rows.Count > 0)
            {
                idlgUpdateProcessLog("Creating Delinquent Cancellation Report", "INFO", istrProcessName);
                CreateReport("rptDelinquentCancellation.rpt", ldtReportData);
                idlgUpdateProcessLog("Successfully created Delinquent Cancellation Report", "INFO", istrProcessName);
            }
        }
    }
}
