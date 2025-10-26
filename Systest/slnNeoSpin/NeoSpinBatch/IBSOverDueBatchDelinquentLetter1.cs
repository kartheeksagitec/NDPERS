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
    class IBSOverDueBatchDelinquentLetter1 : busNeoSpinBatch
    {
        public IBSOverDueBatchDelinquentLetter1()
        {
        }

        public busIbsHeader ibusLastPostedRegularIBSHeader { get; set; }
        public void LoadLastPostedRegularIBSHeader()
        {
            ibusLastPostedRegularIBSHeader = new busIbsHeader { icdoIbsHeader = new cdoIbsHeader() };
            DataTable ldtbList = busNeoSpinBase.SelectWithOperator<cdoIbsHeader>(
              new string[2] { "report_type_value", "report_status_value" },
               new string[2] { "=", "=" },
              new object[2] { busConstant.IBSHeaderReportTypeRegular, busConstant.IBSHeaderStatusPosted }, "billing_month_and_year desc");
            if (ldtbList.Rows.Count > 0)
            {
                ibusLastPostedRegularIBSHeader.icdoIbsHeader.LoadData(ldtbList.Rows[0]);
            }
        }

        public void CreateIBSDelinquentLetter()
        {
            istrProcessName = "1st Delinquent Letter";
            idlgUpdateProcessLog("IBS OverDue Batch - 1st Delinquent Letter", "INFO", istrProcessName);
            LoadLastPostedRegularIBSHeader();
            DataTable ldtbIBSMbrs = busNeoSpinBase.Select("cdoIbsHeader.LoadIBSMembers", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            //PROD PIR 5802 : due amount logic changes
            //--start--//
           
            DataTable ldtbRemittance = busNeoSpinBase.Select("cdoRemittance.GetAllRemittanceAfterLastIBS", new object[1] { iobjSystemManagement.icdoSystemManagement.batch_date });
            DataTable ldtIBSPersonSummary = busNeoSpinBase.Select<cdoIbsPersonSummary>(new string[1] { "ibs_header_id" },
                                                                                        new object[1] { ibusLastPostedRegularIBSHeader.icdoIbsHeader.ibs_header_id }, null, null);
            DataTable ldtbPAPIT = busNeoSpinBase.Select("cdoPayeeAccountPaymentItemType.LoadPAPITForInsrRecv", new object[0] { });
            int lintPreviousPersonID = 0;
            decimal ldecAmountDue = 0.00M;
            DateTime ldtPremiumDueDate = new DateTime(iobjSystemManagement.icdoSystemManagement.batch_date.Year, iobjSystemManagement.icdoSystemManagement.batch_date.Month, 1);
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
                    //uat pir 1422 
                    //to load medicare_in field
                    DataTable ldtFilteredMembers = ldtbIBSMbrs.AsEnumerable()
                                                    .Where(o => o.Field<int>("person_id") == lbusPersonAccount.icdoPersonAccount.person_id &&
                                                        (o.Field<int>("plan_id") == busConstant.PlanIdGroupHealth ||
                                                        o.Field<int>("plan_id") == busConstant.PlanIdMedicarePartD)).AsDataTable();
                    if (ldtFilteredMembers != null && ldtFilteredMembers.Rows.Count > 0)
                    {
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.LoadData(ldtFilteredMembers.Rows[0]);
                        lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccount.LoadData(ldtFilteredMembers.Rows[0]);

                        if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id > 0)
                        {
                            if (lbusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                            {
                                lbusPersonAccount.ibusPersonAccountGHDV.LoadRateStructureForUserStructureCode();
                            }
                            else
                            {
                                lbusPersonAccount.ibusPersonAccountGHDV.LoadHealthParticipationDate();
                                lbusPersonAccount.ibusPersonAccountGHDV.LoadRateStructure(ldtPremiumDueDate);
                            }
                            lbusPersonAccount.ibusPersonAccountGHDV.LoadCoverageRefID();
                        }
                    }

                    DataRow[] ldarrIBSSummary = ldtIBSPersonSummary.FilterTable(busConstant.DataType.Numeric, "person_id", lbusPersonAccount.icdoPersonAccount.person_id);
                    if (ldarrIBSSummary.Length > 0)
                    {
                        ldecAmountDue = ldarrIBSSummary[0]["balance_forward"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldarrIBSSummary[0]["balance_forward"]);
                        ldecAmountDue += ldarrIBSSummary[0]["adjustment_amount"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldarrIBSSummary[0]["adjustment_amount"]);
                        ldecAmountDue += ldarrIBSSummary[0]["member_premium_amount"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldarrIBSSummary[0]["member_premium_amount"]);
                    }
                    DataRow[] ldarrRemittance = ldtbRemittance.FilterTable(busConstant.DataType.Numeric, "person_id", lbusPersonAccount.icdoPersonAccount.person_id);
                    if (ldarrRemittance.Length > 0)
                    {
                        ldecAmountDue -= ldarrRemittance[0]["payments_made"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldarrRemittance[0]["payments_made"]);
                    }
                    DataRow[] ldarrPAPIT = ldtbPAPIT.FilterTable(busConstant.DataType.Numeric, "person_id", lbusPersonAccount.icdoPersonAccount.person_id);
                    if (ldarrPAPIT.Length > 0)
                    {
                        ldecAmountDue -= ldarrPAPIT[0]["amount"] == DBNull.Value ? 0.00M : Convert.ToDecimal(ldarrPAPIT[0]["amount"]);
                    }
                    lbusPersonAccount.idecDueAmount = ldecAmountDue;
                    if (lbusPersonAccount.idecDueAmount > 0.00M)
                    {
                        try
                        {
                            //ArrayList larrlist = new ArrayList();
                            //larrlist.Add(lbusPersonAccount);
                            idlgUpdateProcessLog("Creating 1st Delinquent Letter for PERSLinkID " + Convert.ToString(lbusPersonAccount.icdoPersonAccount.person_id)
                                                            , "INFO", istrProcessName);
                            Hashtable lhstDummyTable = new Hashtable();
                            lhstDummyTable.Add("sfwCallingForm", "Batch");
                            CreateCorrespondence("PAY-4301", lbusPersonAccount, lhstDummyTable);
                            idlgUpdateProcessLog("1st Delinquent Letter created", "INFO", istrProcessName);
                        }
                        catch (Exception _exc)
                        {
                            idlgUpdateProcessLog("ERROR:" + _exc.Message, "INFO", istrProcessName);
                        }
                    }
                    lintPreviousPersonID = lbusPersonAccount.icdoPersonAccount.person_id;
                    lbusPersonAccount = null;
                    //--end--//
                }
            }
        }
    }
}
