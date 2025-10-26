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

#endregion

namespace NeoSpinBatch
{
    class busApproaching457ContributionLimitBatch : busNeoSpinBatch
    {
        private Collection<busPersonAccountDeferredComp> _iclbApproachingContributionLimit;
        public Collection<busPersonAccountDeferredComp> iclbApproachingContributionLimit
        {
            get { return _iclbApproachingContributionLimit; }
            set { _iclbApproachingContributionLimit = value; }
        }
        DataTable idtResultTable = new DataTable();
        public void CreateApproachingContributionLimitCorrespondence()
        {
            istrProcessName = "Approaching 457 Contribution Limit";
            idlgUpdateProcessLog("Creating Correspondence for Approaching 457 Contribution Limit Batch", "INFO", istrProcessName);
            DataTable ldtbLists = DBFunction.DBSelect("cdoPersonAccountDeferredComp.Approaching457ContributionLimit", new object[] { },
                                            iobjPassInfo.iconFramework,
                                            iobjPassInfo.itrnFramework);
            _iclbApproachingContributionLimit = new Collection<busPersonAccountDeferredComp>();
            bool lblnGenerateCorrespondence = false;
            foreach (DataRow dr in ldtbLists.Rows)
            {
                decimal ldecTotalamount = 0.00M;
                decimal ldecSumOfPerPayPeriodamount = 0.00M;
                decimal ldecPerPayPeriodamount = 0.00M;
                decimal ldecSumOfContributionAmount = 0.00M;
                string lstrReportFrequency = string.Empty;
                bool lblContributionForCurrentMonthIsPosted = false;
                /* UAT PIR: 995
                 The Monthly Contribution Amount Should  get only the First Month pay period contribution amount across deferred comp providers.                 
                 */
                int lintCnt = 0;
                decimal ldecMonthlyContributionAmount = 0.00M;

                DateTime ldtStartDate = iobjSystemManagement.icdoSystemManagement.batch_date;
                DateTime ldtContributionStartDate = new DateTime(ldtStartDate.Year, 1, 1);
                DateTime ldtContributionEndDate = new DateTime(ldtStartDate.Year, ldtStartDate.Month, 1);
                busPersonAccountDeferredComp lobjDeffComp = new busPersonAccountDeferredComp();
                lobjDeffComp.icdoPersonAccountDeferredComp = new cdoPersonAccountDeferredComp();
                lobjDeffComp.icdoPersonAccount = new cdoPersonAccount();
                lobjDeffComp.ibusPerson = new busPerson { icdoPerson = new cdoPerson() };
                lobjDeffComp.icdoPersonAccountDeferredComp.LoadData(dr);
                lobjDeffComp.icdoPersonAccount.LoadData(dr);
                lobjDeffComp.ibusPerson.icdoPerson.LoadData(dr);
                lobjDeffComp.Set457Limit();
                lobjDeffComp.LoadActivePersonAccountProviders();
                lobjDeffComp.LoadContributionsFor457BatchLetter(ldtContributionStartDate, ldtContributionEndDate);
                foreach (busPersonAccountDeferredCompContribution lobjContribution in lobjDeffComp.iclb457Contributions)
                {
                    if ((lobjContribution.icdoPersonAccountDeferredCompContribution.paid_date.Month == ldtStartDate.Month) &&
                        (lobjContribution.icdoPersonAccountDeferredCompContribution.paid_date.Year == ldtStartDate.Year) &&
                        (lobjContribution.icdoPersonAccountDeferredCompContribution.transaction_type_value == busConstant.TransactionTypeRegularPayroll))
                    {
                        lblContributionForCurrentMonthIsPosted = true;
                    }
                    ldecSumOfContributionAmount += lobjContribution.icdoPersonAccountDeferredCompContribution.pay_period_contribution_amount;
                }
                foreach (busPersonAccountDeferredCompProvider lobjDeffCompProvider in lobjDeffComp.icolPersonAccountDeferredCompProvider)
                {
                    lintCnt = 1;
                    if (!lblContributionForCurrentMonthIsPosted)
                    {
                        ldtStartDate = iobjSystemManagement.icdoSystemManagement.batch_date;                         
                    }
                    else
                    {
                        ldtStartDate = iobjSystemManagement.icdoSystemManagement.batch_date.AddMonths(1);                        
                    }

                    /************************************************************************************
                     * UAT PIR: 995. The Approaching Limit is checked for that Whole Month.
                    //Eg: if the batch runs at 02 Feb also, it would include those dates till the End of the Month.
                    //In Short, it would include the amount per pay period for the month till End of Month.
                     *****************************************************************************************/
                     ldtStartDate = busGlobalFunctions.GetLastDayOfMonth(ldtStartDate); 
                    /**********************UAT PIR 995 End ************************************************/

                    if (lobjDeffCompProvider.ibusPersonAccountDeferredComp == null)
                        lobjDeffCompProvider.LoadPersonAccountDeferredComp();
                    if (lobjDeffCompProvider.ibusPersonEmployment == null)
                        lobjDeffCompProvider.LoadPersonEmployment();
                    if (lobjDeffCompProvider.ibusPersonAccountDeferredComp.ibusOrgPlan == null)
                        lobjDeffCompProvider.ibusPersonAccountDeferredComp.LoadOrgPlan(
                            iobjSystemManagement.icdoSystemManagement.batch_date,
                            lobjDeffCompProvider.ibusPersonEmployment.icdoPersonEmployment.org_id);

                    lstrReportFrequency = lobjDeffCompProvider.ibusPersonAccountDeferredComp.ibusOrgPlan.icdoOrgPlan.report_frequency_value;

                    while ((ldtStartDate.Year == DateTime.Today.Year) && (ldtStartDate.Month <= 12))
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(ldtStartDate,
                                                                    lobjDeffCompProvider.icdoPersonAccountDeferredCompProvider.start_date,
                                                                    lobjDeffCompProvider.icdoPersonAccountDeferredCompProvider.end_date))
                        {
                            if ((lstrReportFrequency == busConstant.DeffCompFrequencyBiWeekly) ||
                                (lstrReportFrequency == busConstant.DeffCompFrequencySemiMonthly))
                            {
                                ldecPerPayPeriodamount =
                                    lobjDeffCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt * 2;
                            }
                            else if ((lstrReportFrequency == busConstant.DeffCompFrequencyWeekly))
                            {
                                ldecPerPayPeriodamount =
                                    lobjDeffCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt * 4;
                            }
                            else
                            {
                                ldecPerPayPeriodamount =
                                    lobjDeffCompProvider.icdoPersonAccountDeferredCompProvider.per_pay_period_contribution_amt;
                            }
                            ldecSumOfPerPayPeriodamount += ldecPerPayPeriodamount;
                            if(lintCnt ==1)
                            {
                                ldecMonthlyContributionAmount += ldecPerPayPeriodamount; 
                            }
                        }
                        ldtStartDate = ldtStartDate.AddMonths(1);
                        lintCnt =lintCnt + 1;
                    }
                }
                ldecTotalamount = ldecSumOfContributionAmount + ldecSumOfPerPayPeriodamount;
                lobjDeffComp.idecPayPeriodAmountCytd = ldecSumOfContributionAmount;
                lobjDeffComp.idecProjectedPayPeriodAmount = ldecSumOfPerPayPeriodamount;
                /* UAT PIR: 995
                The Monthly Contribution Amount Should  get only the First Month pay period contribution amount across deferred comp providers.                 
                */
                lobjDeffComp.idecMonthlyContributionAmount = ldecMonthlyContributionAmount;

                //PIR-11730 Now We have to get the Limit Value from 457 Limit Ref Table 21 October 2013 
                lobjDeffComp.ldcl457ContributionLimit = lobjDeffComp.GetIRSLimitAmount(lobjDeffComp.icdoPersonAccountDeferredComp.limit_457_value,DateTime.Now);
                
                if (ldecTotalamount > lobjDeffComp.ldcl457ContributionLimit)
                {
                    lblnGenerateCorrespondence = true;
                    //ArrayList larrList = new ArrayList();
                    //larrList.Add(lobjDeffComp);
                    _iclbApproachingContributionLimit.Add(lobjDeffComp);
                    Hashtable lshtTemp = new Hashtable();
                    lshtTemp.Add("sfwCallingForm", "Batch");
                    string lstrFileName = CreateCorrespondence("ENR-5400", lobjDeffComp, lshtTemp);
                    CreateContactTicket(lobjDeffComp.icdoPersonAccount.person_id);                    
                }
            }
            if (lblnGenerateCorrespondence)
                idlgUpdateProcessLog("Correspondence Created successfully", "INFO", istrProcessName);
            idlgUpdateProcessLog("Generating Approaching 457 Contribution Limit Report ", "INFO", istrProcessName);
            GenerateReport(_iclbApproachingContributionLimit);
        }
        // Create Contact Ticket
        private void CreateContactTicket(int aintPersonID)
        {
            cdoContactTicket lobjContactTicket = new cdoContactTicket();
            CreateContactTicket(aintPersonID, busConstant.ContactTicketTypeInsuranceRetiree, lobjContactTicket);
        }

        public void GenerateReport(Collection<busPersonAccountDeferredComp> aclbPersonAccounDeferredComp)
        {
            idtResultTable = CreateNewDataTable();

            foreach (busPersonAccountDeferredComp lobjPersonAccountDeferredComp in aclbPersonAccounDeferredComp)
            {
                AddToNewDataRow(lobjPersonAccountDeferredComp);
            }

            if (idtResultTable.Rows.Count > 0)
            {
                CreateReport("rptApproaching457ContributionLimit.rpt", idtResultTable);

                idlgUpdateProcessLog("Approaching 457 Contribution Limit Report generated succesfully", "INFO", istrProcessName);
            }
            else
            {
                idlgUpdateProcessLog("No Report Generated", "INFO", istrProcessName);
            }

        }

        public DataTable CreateNewDataTable()
        {
            DataTable ldtbReportTable = new DataTable();
            DataColumn ldc1 = new DataColumn("FullName", Type.GetType("System.String"));
            DataColumn ldc2 = new DataColumn("LastName", Type.GetType("System.String"));
            DataColumn ldc3 = new DataColumn("Employer", Type.GetType("System.String"));
            DataColumn ldc4 = new DataColumn("457Limit", Type.GetType("System.String"));
            DataColumn ldc5 = new DataColumn("MonthlyContributionAmount", Type.GetType("System.Decimal"));
            DataColumn ldc6 = new DataColumn("YTDContributions", Type.GetType("System.Decimal"));
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
        public void AddToNewDataRow(busPersonAccountDeferredComp aobjPersonAccountDeferredComp)
        {
            if (aobjPersonAccountDeferredComp.ibusPerson == null)
                aobjPersonAccountDeferredComp.LoadPerson();
            DataRow dr = idtResultTable.NewRow();
            dr["FullName"] = aobjPersonAccountDeferredComp.ibusPerson.icdoPerson.FullName;
            dr["LastName"] = aobjPersonAccountDeferredComp.ibusPerson.icdoPerson.last_name;
            dr["Employer"] = "";
            dr["457Limit"] = aobjPersonAccountDeferredComp.icdoPersonAccountDeferredComp.limit_457_description;
            /* UAT PIR: 995
            The Monthly Contribution Amount Should  get only the First Month pay period contribution amount across deferred comp providers.                 
            */
            dr["MonthlyContributionAmount"] = aobjPersonAccountDeferredComp.idecMonthlyContributionAmount;

            dr["YTDContributions"] = aobjPersonAccountDeferredComp.idecPayPeriodAmountCytd; ;
            dr["PERSLinkID"] = aobjPersonAccountDeferredComp.icdoPersonAccount.person_id;
            idtResultTable.Rows.Add(dr);
        }
    }
}
