#region Using directives

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Text;
using System.Data;
using Sagitec.Common;
using Sagitec.DBUtility;
using Sagitec.BusinessObjects;
using NeoSpin.CustomDataObjects;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
#endregion


namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busEmployerPayrollMonthlyStatement : busEmployerPayrollMonthlyStatementGen
    {
        private Collection<busOrgPlan> _iclbActiveOrgPlan;

        public Collection<busOrgPlan> iclbActiveOrgPlan
        {
            get { return _iclbActiveOrgPlan; }
            set { _iclbActiveOrgPlan = value; }
        }

        private string _istrOrgCodeId;

        public string istrOrgCodeId
        {
            get { return _istrOrgCodeId; }
            set { _istrOrgCodeId = value; }
        }
        public int OrgID { get; set; }        

        public busOrganization ibusOrganization { get; set; }

        private busEmployerPayrollMonthlyStatement _ibusEmpPayrollMonthlyStatment;

        public busEmployerPayrollMonthlyStatement ibusEmpPayrollMonthlyStatment
        {
            get { return _ibusEmpPayrollMonthlyStatment; }
            set { _ibusEmpPayrollMonthlyStatment = value; }
        }
        public bool iblnIsFromESS { get; set; }
        public bool iblnIsFrombatch = false;

        //prod pir 6488
        public bool iblnServicePurchaseIncluded { get; set; }

        private void LoadOrganization()
        {
            if (ibusOrganization == null)
                ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };

            if (istrOrgCodeId.IsNotNullOrEmpty())
                ibusOrganization.FindOrganizationByOrgCode(istrOrgCodeId);
        }

        public ArrayList btnGenerateCorrespondence_Click()
        {
            ValidateHardErrors(utlPageMode.All);
            if (iarrErrors.Count > 0)
                return iarrErrors;

            ArrayList larrList = new ArrayList();
            LoadOrganization();

            //else run for org id specified               
            DataTable ldtbList = Select("cdoEmployerPayrollMonthlyStatement.GetActiveOrgPlanForGivenOrgId", new object[2] { ibusOrganization.icdoOrganization.org_id,
                                                                                                                            icdoEmployerPayrollMonthlyStatement.idtStartDate });
            LoadOrgPlan(ldtbList);

            DateTime ldtBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
            //prod pir 6488 : need to include service purchase posted header only once
            iblnServicePurchaseIncluded = false;
            //PIR 10707 - One Correspondence should be generated per plan.
            Collection<busOrgPlan> lclbPlanList = new Collection<busOrgPlan>();
            foreach (busOrgPlan lobjOrgPlan in iclbActiveOrgPlan)
            {

                if (!lclbPlanList.Any(lbus => lbus.icdoOrgPlan.plan_id == lobjOrgPlan.icdoOrgPlan.plan_id))
                {
                    GenerateMonthlyEmployerStatment(ldtBatchDate, lobjOrgPlan);
                    lclbPlanList.Add(lobjOrgPlan);
                }
            }

            larrList.Add(this);
            return larrList;
        }

        public void LoadOrgPlan(DataTable ldtTable)
        {
            iclbActiveOrgPlan = new Collection<busOrgPlan>();
            foreach (DataRow adrRow in ldtTable.Rows)
            {
                busOrgPlan lbusOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
                lbusOrgPlan.icdoOrgPlan.LoadData(adrRow);

                lbusOrgPlan.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                lbusOrgPlan.ibusOrganization.icdoOrganization.LoadData(adrRow);

                lbusOrgPlan.ibusPlan = new busPlan { icdoPlan = new cdoPlan() };
                lbusOrgPlan.ibusPlan.icdoPlan.LoadData(adrRow);

                iclbActiveOrgPlan.Add(lbusOrgPlan);
            }
        }
        //Load plan
        public busPlan ibusPlan { get; set; }
        public void LoadPlan()
        {
            if (ibusPlan == null)
                ibusPlan = new busPlan();
            ibusPlan.FindPlan(icdoEmployerPayrollMonthlyStatement.plan_id);
        }
        //get all active Employers
        //Get Org plan
        public void GenerateMonthlyEmployerStatment(DateTime adtBatchDate, busOrgPlan abusOrgPlan)
        {
            //foreach plan we need to load emp payroll monthly statement and we need to initialize everytime
            //Will contain the latest employer payroll monthly statement based on the start date.
            _ibusEmpPayrollMonthlyStatment = new busEmployerPayrollMonthlyStatement();
            _ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement = new cdoEmployerPayrollMonthlyStatement();


            //UAT PIR - 1962
            abusOrgPlan.iclbInvoiceCollection = new Collection<busOrgPlan>();
            Collection<busOrgPlan> iclbTempInvoiceCollection = new Collection<busOrgPlan>();
            abusOrgPlan.iclbRemittanceCollection = new Collection<busOrgPlan>();

            LoadLastRunMonthlyEmployerDetails(abusOrgPlan, adtBatchDate);

            abusOrgPlan.idtBatchRunDate = adtBatchDate;

            if (busGlobalFunctions.CheckDateOverlapping(abusOrgPlan.idtStartDate,
                abusOrgPlan.idtEndDate, abusOrgPlan.icdoOrgPlan.participation_start_date, abusOrgPlan.icdoOrgPlan.participation_end_date))
            {
                //Load Organization and Plan details
                if (abusOrgPlan.ibusOrganization == null)
                    abusOrgPlan.LoadOrganization();
                if (abusOrgPlan.ibusPlan == null)
                    abusOrgPlan.LoadPlanInfo();

                GetNewBeginingBalanceAmount(abusOrgPlan, ibusEmpPayrollMonthlyStatment);

                //SetBeginningBalance(abusOrgPlan);// For ESS Agency Statement//PIR 6543

                //Get the remittance amount for that time period.
                abusOrgPlan.idecNewRemittanceAmount = GetRemittanceAmountForGivenPeriod(abusOrgPlan.icdoOrgPlan.org_id,
                                                                          abusOrgPlan.icdoOrgPlan.plan_id,
                                                                          abusOrgPlan.idtStartDate,
                                                                          abusOrgPlan.idtEndDate, abusOrgPlan.iclbRemittanceCollection);

                //UAT PIR - 1962
                abusOrgPlan.idecTotalRemittanceAmount = abusOrgPlan.iclbRemittanceCollection.Sum(lobjRem => lobjRem.idecIndividualRemittanceAmount);

                //get detail with posted date between this start date and end date 
                abusOrgPlan.idecInvoiceAmount = GetInvoiceAmountForGivenPeriod(abusOrgPlan.icdoOrgPlan.org_id,
                                                                               abusOrgPlan.icdoOrgPlan.plan_id,
                                                                               abusOrgPlan.idtStartDate,
                                                                               abusOrgPlan.idtEndDate, iclbTempInvoiceCollection);

                //UAT PIR - 1962
                abusOrgPlan.idecTotalInvoiceAmount = iclbTempInvoiceCollection.Sum(lobjRem => lobjRem.idecIndividualInvoiceAmount);
                //Logically idecNewRemittanceAmount = idecTotalRemittanceAmount and idecInvoiceAmount = idecTotalInvoiceAmount
                abusOrgPlan.idecNewMonthDueAmount = abusOrgPlan.idecNewBeginingBalanceAmount +
                                                    abusOrgPlan.idecInvoiceAmount -
                                                    abusOrgPlan.idecNewRemittanceAmount;

                //group by employer header
                //IEnumerable<IGrouping<DateTime,busOrgPlan>> lclbTempCollection = iclbTempInvoiceCollection
                //                                                                        .OrderByDescending(lobj => lobj.idtBillingDate)
                //                                                                        .GroupBy(lobjInvoice => lobjInvoice.idtBillingDate);
                //Group by invoice number and Billing date
                var lclbTColl = from cm in iclbTempInvoiceCollection
                                group cm by new { cm.idtBillingDate, cm.iintInvoiceNumber } into cms
                                select
                                new { Key1 = cms.Key.idtBillingDate, Key2 = cms.Key.iintInvoiceNumber, coll = cms.Sum(CalcAmount => CalcAmount.idecIndividualInvoiceAmount), col = cms };

                foreach (var i in lclbTColl)
                {
                    busOrgPlan lobjNewInvoice = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
                    lobjNewInvoice = i.col.FirstOrDefault();
                    lobjNewInvoice.idecIndividualInvoiceAmount = i.coll;
                    abusOrgPlan.iclbInvoiceCollection.Add(lobjNewInvoice);
                }

                //foreach (IGrouping<DateTime, busOrgPlan> lclbOrgPlan in lclbTempCollection)
                //{
                //    busOrgPlan lobjNewInvoice = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
                //    lobjNewInvoice = lclbOrgPlan.FirstOrDefault();                 
                //    decimal ldecTemp = 0.00M;
                //    foreach (busOrgPlan lobjOrgPlan in lclbOrgPlan)
                //    {
                //        ldecTemp += lobjOrgPlan.idecIndividualInvoiceAmount;
                //    }
                //    lobjNewInvoice.idecIndividualInvoiceAmount = ldecTemp;
                //    abusOrgPlan.iclbInvoiceCollection.Add(lobjNewInvoice);
                //}

                if (!iblnIsFromESS)
                {
                    //PIR 949 - For Online Monthly Report Request, Generate Letter for Everyone
                    //For Batch Monthly Report Request, Generate Letter only for Org those Web Portal is not set.
                    //PIR 24618   FILLER: SFN 58807 and 58808 are discontinued.
                    //if ((iblnIsFrombatch && abusOrgPlan.ibusOrganization.icdoOrganization.emp_trans_report_flag != busConstant.Flag_Yes) || (!iblnIsFrombatch))
                    //{
                    //    if (abusOrgPlan.idecNewMonthDueAmount != 0) // PIR 8455 add if
                    //    {
                    //        GenerateCorrespondenceForEachEmployer(abusOrgPlan);
                    //    }
                    //}

                    if (iblnIsFrombatch)
                    {
                        InsertIntoMonthlyEmployerStatement(abusOrgPlan);
                    }
                }
            }
        }

        //public void GenerateCorrespondenceForEachEmployer(busOrgPlan aobjOrgPlan)
        //{
        //    //ArrayList larrlist = new ArrayList();
        //    //larrlist.Add(aobjOrgPlan);
        //    Hashtable lhstDummyTable = new Hashtable();
        //    lhstDummyTable.Add("sfwCallingForm", "Batch");
        //    //utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence("SFN-58807", larrlist, lhstDummyTable);
        //    utlCorresPondenceInfo lobjCorresPondenceInfo = busNeoSpinBase.SetCorrespondence("SFN-58807", aobjOrgPlan, lhstDummyTable);
        //    Sagitec.CorBuilder.CorBuilderXML lobjCorBuilder = new Sagitec.CorBuilder.CorBuilderXML(); //PIR 16947 - Change to CorBuilderXML class
        //    lobjCorBuilder.InstantiateWord();
        //    //PIR 24618   FILLER: SFN 58807 and 58808 are discontinued.
        //    //lobjCorBuilder.CreateCorrespondenceFromTemplate("SFN-58807", lobjCorresPondenceInfo, iobjPassInfo.istrUserID);
        //    lobjCorBuilder.CloseWord();

        //}

        #region 6543 - Commented Code

        // PIR 6543 - Commented Code 
        //public int iintMaxRetirementStatementID = 0;
        //public int iintMaxInsuranceStatementID = 0;
        //public int iintMaxDefCompStatementID = 0;
        //private void SetBeginningBalance(busOrgPlan lobjOrgPlan)
        //{
        //    if (lobjOrgPlan.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
        //    {
        //        if (iintMaxRetirementStatementID < ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.employer_payroll_monthly_statement_id)
        //        {
        //            iintMaxRetirementStatementID = ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.employer_payroll_monthly_statement_id;
        //            idecRetCompBeginnineBalanceAmount = ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.month_due_amt;
        //        }
        //    }
        //    else if (lobjOrgPlan.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeInsurance)
        //    {
        //        if (iintMaxInsuranceStatementID < ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.employer_payroll_monthly_statement_id)
        //        {
        //            iintMaxInsuranceStatementID = ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.employer_payroll_monthly_statement_id;
        //            idecInsCompBeginnineBalanceAmount = ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.month_due_amt;
        //        }
        //    }
        //    else if (lobjOrgPlan.ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeDeferredComp)
        //    {
        //        if (iintMaxDefCompStatementID < ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.employer_payroll_monthly_statement_id)
        //        {
        //            iintMaxDefCompStatementID = ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.employer_payroll_monthly_statement_id;
        //            idecDefCompBeginnineBalanceAmount = ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.month_due_amt;
        //        }
        //    }
        //}
        #endregion

        /// <summary>
        /// Load latest Employer Payroll Monthly Statement that was run before the start date
        /// </summary>
        /// <param name="lobjOrgPlan">Org Plan</param>
        /// <param name="adtBatchDate">Batch Date</param>
        public void LoadLastRunMonthlyEmployerDetails(busOrgPlan lobjOrgPlan, DateTime adtBatchDate)
        {
            //get last record this org plan combination - order by run date
            DataTable ldtbGetLastrecord = Select<cdoEmployerPayrollMonthlyStatement>(new string[2] { "org_id", "plan_id" },
                            new object[2] { lobjOrgPlan.icdoOrgPlan.org_id, lobjOrgPlan.icdoOrgPlan.plan_id }, null, "run_date desc");

            if (ldtbGetLastrecord.Rows.Count > 0)
            {
                if (iblnIsFrombatch)
                {
                    _ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.LoadData(ldtbGetLastrecord.Rows[0]);
                    lobjOrgPlan.idtStartDate = _ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.run_date.AddDays(1);
                    lobjOrgPlan.idtEndDate = adtBatchDate;
                }
                else
                {
                    // not  from batch - ESS Agency Statement or PERSlink Correspondence
                    lobjOrgPlan.idtStartDate = icdoEmployerPayrollMonthlyStatement.idtStartDate;
                    lobjOrgPlan.idtEndDate = icdoEmployerPayrollMonthlyStatement.idtEndDate;
                    //PIR 6543 - Load Latest Employer Payroll Monthly Statement Before the Org Plan start date
                    var lenumList = ldtbGetLastrecord.AsEnumerable().Where(i => i.Field<DateTime>("Run_Date").Date <= lobjOrgPlan.idtStartDate.Date).OrderByDescending(i => i.Field<DateTime>("run_date")).FirstOrDefault();
                    if (lenumList.IsNotNull())
                        _ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.LoadData(lenumList);
                    else
                        //Default Load First Run Employer Payroll Monthly Statement.
                        ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.LoadData(ldtbGetLastrecord.Rows[ldtbGetLastrecord.Rows.Count - 1]);
                    lobjOrgPlan.idtStartDate = ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.run_date.Date;

                    if (adtBatchDate.Date < lobjOrgPlan.idtEndDate.Date)
                    {
                        lobjOrgPlan.idtEndDate = adtBatchDate;
                    }
                    else if (lobjOrgPlan.idtEndDate == DateTime.MinValue) // UAT PIR fix - 1962 if the end date is not entered then take the current date else date entered
                    {
                        lobjOrgPlan.idtEndDate = DateTime.Now;
                    }
                }
            }
            else
            {
                if (iblnIsFrombatch)
                {
                    lobjOrgPlan.idtStartDate = lobjOrgPlan.icdoOrgPlan.participation_start_date;
                    lobjOrgPlan.idtEndDate = adtBatchDate;
                }
                else
                {
                    lobjOrgPlan.idtStartDate = icdoEmployerPayrollMonthlyStatement.idtStartDate;
                    lobjOrgPlan.idtEndDate = icdoEmployerPayrollMonthlyStatement.idtEndDate;
                }
                lobjOrgPlan.idecNewBeginingBalanceAmount = 0.00M;
            }
            //prod pir 1962
            DateTime ldtPersLinkGoLiveDate = busPayeeAccountHelper.GetPERSLinkGoLiveDate();
            if (ldtPersLinkGoLiveDate != DateTime.MinValue && lobjOrgPlan.idtStartDate < ldtPersLinkGoLiveDate)
            {
                lobjOrgPlan.idtStartDate = ldtPersLinkGoLiveDate;
                if (lobjOrgPlan.idtStartDate > lobjOrgPlan.idtEndDate)
                    lobjOrgPlan.idtEndDate = adtBatchDate;
            }
        }

        private static void GetNewBeginingBalanceAmount(busOrgPlan lobjOrgPlan, busEmployerPayrollMonthlyStatement aobjEmployerPayrollMonthlyStatement)
        {
            #region unused code - commented for pir 6543
            //decimal ldecBeginingBalanceAmount = 0.00M;
            //decimal ldecMonthAmountDue = 0.00M;
            //decimal ldecRemittanceAmount = 0.00M;

            //ldecBeginingBalanceAmount = aobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.beginning_balance_amt;
            //ldecMonthAmountDue = aobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.month_due_amt;
            //ldecRemittanceAmount = aobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.remittance_amount;
            //lobjOrgPlan.idecBeginningBalanceAsMonthlyDue = ldecMonthAmountDue;
            #endregion
            lobjOrgPlan.idecNewBeginingBalanceAmount = aobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.month_due_amt;// ldecMonthAmountDue;//ldecBeginingBalanceAmount + ldecMonthAmountDue;//PIR 6543        }
        }
        /// <summary>
        /// Gets the Total Remittance Amount for the org plan within the given start and End Date
        /// </summary>
        /// <param name="aintOrgId"></param>
        /// <param name="aintPlanId"></param>
        /// <param name="adtStartDate"></param>
        /// <param name="adtEndDate"></param>
        /// <param name="aclbRemittanceCollection"></param>
        /// <returns></returns>
        private decimal GetRemittanceAmountForGivenPeriod(int aintOrgId, int aintPlanId, DateTime adtStartDate, DateTime adtEndDate, Collection<busOrgPlan> aclbRemittanceCollection)
        {
            decimal ldecRemittanceAmount = 0.00M;
            DataTable ldtRemittanceForOrgPlan;

            // Gets all applied deposits and remittance type not in JS RHIC Deposit and Prior Judges Benefit for that org and plan
            ldtRemittanceForOrgPlan = Select("cdoEmployerPayrollMonthlyStatement.GetRemittanceAmount", new object[2] { aintOrgId, aintPlanId });

            foreach (DataRow dr in ldtRemittanceForOrgPlan.Rows)
            {
                if (!String.IsNullOrEmpty(dr["Applied_Date"].ToString()))
                {
                    DateTime ldtAppliedDate = Convert.ToDateTime(dr["Applied_Date"].ToString());
                    if (busGlobalFunctions.CheckDateOverlapping(ldtAppliedDate, adtStartDate, adtEndDate))
                    {
                        if (!String.IsNullOrEmpty(dr["available_amount"].ToString()))
                        {
                            //UAT PIR - 1962
                            busOrgPlan lobjRemittance = new busOrgPlan();
                            if (!String.IsNullOrEmpty(dr["REMITTANCE_ID"].ToString()))
                            {
                                lobjRemittance.iintRemittanceNumber = Convert.ToInt32(dr["REMITTANCE_ID"].ToString());
                            }
                            ldecRemittanceAmount += Convert.ToDecimal(dr["available_amount"]);
                            //Properties For Correspondence
                            lobjRemittance.idecIndividualRemittanceAmount = Convert.ToDecimal(dr["available_amount"]);
                            lobjRemittance.idtPaymentDate = ldtAppliedDate;
                            aclbRemittanceCollection.Add(lobjRemittance);
                        }
                    }
                }
            }
            return ldecRemittanceAmount;
        }


        /// <summary>
        /// Get the Invoice Amount for an org plan within the given start date and end date
        /// </summary>
        /// <param name="aintOrgId"></param>
        /// <param name="aintPlanId"></param>
        /// <param name="adtStartDate"></param>
        /// <param name="adtEndDate"></param>
        /// <param name="aclbInvoiceCollection"></param>
        /// <returns></returns>
        private decimal GetInvoiceAmountForGivenPeriod(int aintOrgId, int aintPlanId, DateTime adtStartDate, DateTime adtEndDate, Collection<busOrgPlan> aclbInvoiceCollection)
        {
            decimal ldecInvoiceAmount = 0.00M;
            DataTable ldtInvoiceForOrgPlan;
            //Get all posted employer payroll details for that org plan
            ldtInvoiceForOrgPlan = Select("cdoEmployerPayrollMonthlyStatement.GetInvoiceAmount", new object[2] { aintOrgId, aintPlanId });
            bool lblnRetirementHeader = false;
            if (ldtInvoiceForOrgPlan != null)
            {
                foreach (DataRow dr in ldtInvoiceForOrgPlan.Rows)
                {
                    decimal ldecIndividualInvoiceAmount = 0M;
                    if (!String.IsNullOrEmpty(dr["Posted_Date"].ToString()))
                    {
                        DateTime ldtAppliedDate = Convert.ToDateTime(dr["Posted_Date"].ToString());
                        if (busGlobalFunctions.CheckDateOverlapping(ldtAppliedDate, adtStartDate, adtEndDate))
                        {
                            busEmployerPayrollDetail lbusEmployerPayrollDetail = new busEmployerPayrollDetail
                            {
                                icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail()
                            };
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.LoadData(dr);

                            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader
                            {
                                icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader()
                            };

                            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(dr);

                            if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeRtmt)
                            {
                                //prod pir 6488
                                lblnRetirementHeader = true;
                                //Add the Contribution if detail entries are Regular / Bonus / +ADJ. Substract it for -ADJ
                                if ((lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular) ||
                                    (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ||
                                    (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypePositiveAdjustment))
                                {
                                    ldecInvoiceAmount +=
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.member_interest_calculated +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_interest_calculated +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported;
                                    //UAT PIR - 1962
                                    ldecIndividualInvoiceAmount = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.member_interest_calculated +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_interest_calculated +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_reported +
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported;
                                }
                                //PROD PIR 6291 : reverting chagne done for 5696
                                else if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment ||
                                        lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus)		// PIR 17777
                                {

                                    ldecInvoiceAmount -=
                                            (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.member_interest_calculated +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_interest_calculated +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported);
                                    //UAT PIR - 1962
                                    ldecIndividualInvoiceAmount = -(lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_contribution_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_contribution_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pre_tax_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_employer_pickup_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_ee_contribution_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.rhic_er_contribution_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.member_interest_calculated +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_interest_calculated +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.employer_rhic_interest_calculated +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.adec_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_pretax_addl_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.ee_post_tax_addl_reported +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.er_pretax_match_reported);
                                }
                            }
                            else if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeDefComp)
                            {
                                //prod pir 6488
                                lblnRetirementHeader = false;
                                //Exclude Other 457 Amount
                                if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.plan_id != busConstant.PlanIdOther457)
                                {
                                    //Add the Contribution if detail entries are Regular / Bonus / +ADJ. Substract it for -ADJ
                                    if ((lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeRegular) ||
                                        (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeBonus) ||
                                        (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypePositiveAdjustment))
                                    {
                                        ldecInvoiceAmount +=
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7;
                                        //UAT PIR - 1962
                                        ldecIndividualInvoiceAmount = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 +
                                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7;
                                    }
                                    //PROD PIR 6291 : reverting chagne done for 5696
                                    else if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
                                    {
                                        ldecInvoiceAmount -=
                                            (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7);
                                        //UAT PIR - 1962
                                        ldecIndividualInvoiceAmount = -(lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount1 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount2 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount3 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount4 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount5 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount6 +
                                             lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.contribution_amount7);
                                    }
                                }
                            }
                            else if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypeInsr)
                            {
                                //prod pir 6488
                                lblnRetirementHeader = false;
                                if (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.record_type_value == busConstant.PayrollDetailRecordTypeNegativeAdjustment)
                                {
                                    //PROD PIR 6291 : reverting chagne done for 5696
                                    ldecInvoiceAmount += (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount * -1M);
                                    //UAT PIR - 1962
                                    ldecIndividualInvoiceAmount = (lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount * -1M);
                                }
                                else
                                {
                                    ldecInvoiceAmount += lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount;
                                    //UAT PIR - 1962
                                    ldecIndividualInvoiceAmount = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.premium_amount;
                                }

                            }
                            else if (lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.header_type_value == busConstant.PayrollHeaderBenefitTypePurchases)
                            {
                                ldecInvoiceAmount += lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.purchase_amount_reported;
                                //UAT PIR - 1962
                                ldecIndividualInvoiceAmount = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.purchase_amount_reported;
                            }
                            //UAT PIR - 1962
                            //  if (ldecIndividualInvoiceAmount > 0.00M)
                            {
                                busOrgPlan lobjInvoice = new busOrgPlan();
                                lobjInvoice.idtBillingDate = ldtAppliedDate;
                                lobjInvoice.idecIndividualInvoiceAmount = ldecIndividualInvoiceAmount;
                                lobjInvoice.iintInvoiceNumber = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;
                                aclbInvoiceCollection.Add(lobjInvoice);
                            }
                        }
                    }
                }
            }
            if (lblnRetirementHeader && !iblnServicePurchaseIncluded)
            {
                iblnServicePurchaseIncluded = true;
                DataTable ldtServicePurchEmpHdr = Select("cdoEmployerPayrollMonthlyStatement.GetInvoiceAmountForPurchase", new object[1] { aintOrgId });
                foreach (DataRow dr in ldtServicePurchEmpHdr.Rows)
                {
                    decimal ldecIndividualInvoiceAmount = 0M;
                    if (!String.IsNullOrEmpty(dr["Posted_Date"].ToString()))
                    {
                        DateTime ldtAppliedDate = Convert.ToDateTime(dr["Posted_Date"].ToString());
                        if (busGlobalFunctions.CheckDateOverlapping(ldtAppliedDate, adtStartDate, adtEndDate))
                        {
                            busEmployerPayrollDetail lbusEmployerPayrollDetail = new busEmployerPayrollDetail
                            {
                                icdoEmployerPayrollDetail = new cdoEmployerPayrollDetail()
                            };
                            lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.LoadData(dr);

                            busEmployerPayrollHeader lbusEmployerPayrollHeader = new busEmployerPayrollHeader
                            {
                                icdoEmployerPayrollHeader = new cdoEmployerPayrollHeader()
                            };

                            lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.LoadData(dr);

                            ldecInvoiceAmount += lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.purchase_amount_reported;
                            //UAT PIR - 1962
                            ldecIndividualInvoiceAmount = lbusEmployerPayrollDetail.icdoEmployerPayrollDetail.purchase_amount_reported;
                            busOrgPlan lobjInvoice = new busOrgPlan();
                            lobjInvoice.idtBillingDate = ldtAppliedDate;
                            lobjInvoice.idecIndividualInvoiceAmount = ldecIndividualInvoiceAmount;
                            lobjInvoice.iintInvoiceNumber = lbusEmployerPayrollHeader.icdoEmployerPayrollHeader.employer_payroll_header_id;
                            aclbInvoiceCollection.Add(lobjInvoice);
                        }
                    }
                }
            }
            return ldecInvoiceAmount;
        }

        public void InsertIntoMonthlyEmployerStatement(busOrgPlan aobjOrgPlan)
        {
            busEmployerPayrollMonthlyStatement lobjEmployerPayrollMonthlyStatement = new busEmployerPayrollMonthlyStatement();
            lobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement = new cdoEmployerPayrollMonthlyStatement();
            lobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.org_id = aobjOrgPlan.icdoOrgPlan.org_id;
            lobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.plan_id = aobjOrgPlan.icdoOrgPlan.plan_id;
            lobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.remittance_amount = aobjOrgPlan.idecNewRemittanceAmount;
            lobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.beginning_balance_amt = aobjOrgPlan.idecNewBeginingBalanceAmount;
            lobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.month_due_amt = aobjOrgPlan.idecNewMonthDueAmount;
            lobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.invoice_amount = aobjOrgPlan.idecInvoiceAmount;
            lobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.run_date = busGlobalFunctions.GetSysManagementBatchDate().Date.AddDays(1).AddMinutes(-1);
            lobjEmployerPayrollMonthlyStatement.icdoEmployerPayrollMonthlyStatement.Insert();
        }

        //prop used in eSS
        public decimal idecDefCompInvoiceAmount { get; set; }
        public decimal idecDefPaymentsAmount { get; set; }
        public decimal idecDefCompCurrentDueAmount { get; set; }
        public decimal idecDefCompBeginnineBalanceAmount { get; set; }

        public decimal idecInsCompInvoiceAmount { get; set; }
        public decimal idecInsPaymentsAmount { get; set; }
        public decimal idecInsCompCurrentDueAmount { get; set; }
        public decimal idecInsCompBeginnineBalanceAmount { get; set; }

        public decimal idecRetCompInvoiceAmount { get; set; }
        public decimal idecRetPaymentsAmount { get; set; }
        public decimal idecRetCompCurrentDueAmount { get; set; }
        public decimal idecRetCompBeginnineBalanceAmount { get; set; }

        public DateTime idtFromDate { get; set; }
        public DateTime idtToDate { get; set; }

        public Collection<busEssMontlyAccountBalance> iclbusEssMontlyAccountBalance { get; set; }

        public void LoadLastRunMonthlyEmployerDetailsForSpecifiedDates(busOrgPlan lobjOrgPlan, DateTime adtBatchDate)
        {
            //get last record this org plan combination - order by run date
            DataTable ldtbGetLastrecord = Select<cdoEmployerPayrollMonthlyStatement>(new string[2] { "org_id", "plan_id" },
                            new object[2] { lobjOrgPlan.icdoOrgPlan.org_id, lobjOrgPlan.icdoOrgPlan.plan_id }, null, "run_date desc");

            if (ldtbGetLastrecord.Rows.Count > 0)
            {
                //PIR 6543 - Load Latest Employer Payroll Monthly Statement Before the Org Plan start date
                var lenumList = ldtbGetLastrecord.AsEnumerable().Where(i => i.Field<DateTime>("Run_Date").Date <= lobjOrgPlan.idtStartDate.Date).OrderByDescending(i => i.Field<DateTime>("run_date")).FirstOrDefault();
                if (lenumList.IsNotNull())
                    _ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.LoadData(lenumList);
                else
                    //Default Load First Run Employer Payroll Monthly Statement.
                    //ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement.LoadData(ldtbGetLastrecord.Rows[ldtbGetLastrecord.Rows.Count - 1]);
                    lobjOrgPlan.idecNewBeginingBalanceAmount = 0.00M;
            }
            else
            {
                //lobjOrgPlan.idtStartDate = icdoEmployerPayrollMonthlyStatement.idtStartDate;
                //lobjOrgPlan.idtEndDate = icdoEmployerPayrollMonthlyStatement.idtEndDate;
                lobjOrgPlan.idecNewBeginingBalanceAmount = 0.00M;
            }
        }

        public void GenerateMonthlyEmployerStatmentForSpecifiedDates(DateTime adtBatchDate, busOrgPlan abusOrgPlan)
        {
            //foreach plan we need to load emp payroll monthly statement and we need to initialize everytime
            //Will contain the latest employer payroll monthly statement based on the start date.
            _ibusEmpPayrollMonthlyStatment = new busEmployerPayrollMonthlyStatement();
            _ibusEmpPayrollMonthlyStatment.icdoEmployerPayrollMonthlyStatement = new cdoEmployerPayrollMonthlyStatement();


            //UAT PIR - 1962
            abusOrgPlan.iclbInvoiceCollection = new Collection<busOrgPlan>();
            Collection<busOrgPlan> iclbTempInvoiceCollection = new Collection<busOrgPlan>();
            abusOrgPlan.iclbRemittanceCollection = new Collection<busOrgPlan>();

            LoadLastRunMonthlyEmployerDetailsForSpecifiedDates(abusOrgPlan, adtBatchDate);

            abusOrgPlan.idtBatchRunDate = adtBatchDate;

            if (busGlobalFunctions.CheckDateOverlapping(abusOrgPlan.idtStartDate,
                abusOrgPlan.idtEndDate, abusOrgPlan.icdoOrgPlan.participation_start_date, abusOrgPlan.icdoOrgPlan.participation_end_date))
            {
                //Load Organization and Plan details
                if (abusOrgPlan.ibusOrganization == null)
                    abusOrgPlan.LoadOrganization();
                if (abusOrgPlan.ibusPlan == null)
                    abusOrgPlan.LoadPlanInfo();

                GetNewBeginingBalanceAmount(abusOrgPlan, ibusEmpPayrollMonthlyStatment);

                //SetBeginningBalance(abusOrgPlan);// For ESS Agency Statement//PIR 6543

                //Get the remittance amount for that time period.
                abusOrgPlan.idecNewRemittanceAmount = GetRemittanceAmountForGivenPeriod(abusOrgPlan.icdoOrgPlan.org_id,
                                                                          abusOrgPlan.icdoOrgPlan.plan_id,
                                                                          abusOrgPlan.idtStartDate,
                                                                          abusOrgPlan.idtEndDate, abusOrgPlan.iclbRemittanceCollection);

                //UAT PIR - 1962
                abusOrgPlan.idecTotalRemittanceAmount = abusOrgPlan.iclbRemittanceCollection.Sum(lobjRem => lobjRem.idecIndividualRemittanceAmount);

                //get detail with posted date between this start date and end date 
                abusOrgPlan.idecInvoiceAmount = GetInvoiceAmountForGivenPeriod(abusOrgPlan.icdoOrgPlan.org_id,
                                                                               abusOrgPlan.icdoOrgPlan.plan_id,
                                                                               abusOrgPlan.idtStartDate,
                                                                               abusOrgPlan.idtEndDate, iclbTempInvoiceCollection);

                //UAT PIR - 1962
                abusOrgPlan.idecTotalInvoiceAmount = iclbTempInvoiceCollection.Sum(lobjRem => lobjRem.idecIndividualInvoiceAmount);
                //Logically idecNewRemittanceAmount = idecTotalRemittanceAmount and idecInvoiceAmount = idecTotalInvoiceAmount
                abusOrgPlan.idecNewMonthDueAmount = abusOrgPlan.idecNewBeginingBalanceAmount +
                                                    abusOrgPlan.idecInvoiceAmount -
                                                    abusOrgPlan.idecNewRemittanceAmount;

                //group by employer header
                //IEnumerable<IGrouping<DateTime,busOrgPlan>> lclbTempCollection = iclbTempInvoiceCollection
                //                                                                        .OrderByDescending(lobj => lobj.idtBillingDate)
                //                                                                        .GroupBy(lobjInvoice => lobjInvoice.idtBillingDate);
                //Group by invoice number and Billing date
                var lclbTColl = from cm in iclbTempInvoiceCollection
                                group cm by new { cm.idtBillingDate, cm.iintInvoiceNumber } into cms
                                select
                                new { Key1 = cms.Key.idtBillingDate, Key2 = cms.Key.iintInvoiceNumber, coll = cms.Sum(CalcAmount => CalcAmount.idecIndividualInvoiceAmount), col = cms };

                foreach (var i in lclbTColl)
                {
                    busOrgPlan lobjNewInvoice = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
                    lobjNewInvoice = i.col.FirstOrDefault();
                    lobjNewInvoice.idecIndividualInvoiceAmount = i.coll;
                    abusOrgPlan.iclbInvoiceCollection.Add(lobjNewInvoice);
                }
            }
        }

        public void LoadAgencyStatementInfoWithinDates(int aintOrgId, DateTime adtEmpStartDate, DateTime adtEmpEndDate)
        {
            DataTable ldtbOrgPlan = Select("cdoEmployerPayrollMonthlyStatement.GetActiveOrgPlanForGivenOrgId", new object[2] { aintOrgId, adtEmpStartDate });
            LoadOrgPlan(ldtbOrgPlan);
            //icdoEmployerPayrollMonthlyStatement = new cdoEmployerPayrollMonthlyStatement();
            //prod pir 6488 : need to include service purchase posted header only once
            iblnServicePurchaseIncluded = false;

            Collection<busOrgPlan> lclbPlanList = new Collection<busOrgPlan>();
            foreach (busOrgPlan lobjOrgPlan in iclbActiveOrgPlan)
            {
                //DataTable ldtbStatement = Select<cdoEmployerPayrollMonthlyStatement>(new string[2] { "org_id", "plan_id" },
                //  new object[2] { lobjOrgPlan.icdoOrgPlan.org_id, lobjOrgPlan.icdoOrgPlan.plan_id }, null, null);
                //Collection<busEmployerPayrollMonthlyStatement> lclbStatement = GetCollection<busEmployerPayrollMonthlyStatement>(ldtbStatement, "icdoEmployerPayrollMonthlyStatement");
                //if (lclbStatement.Count > 0)
                //{
                //    icdoEmployerPayrollMonthlyStatement.idtStartDate = lclbStatement.Select(o => o.icdoEmployerPayrollMonthlyStatement.run_date).Max();
                //}
                //else
                //{
                //    icdoEmployerPayrollMonthlyStatement.idtStartDate = lobjOrgPlan.icdoOrgPlan.participation_start_date;
                //}
                //icdoEmployerPayrollMonthlyStatement.idtEndDate = DateTime.Today;

                //PIR 10707
                if (!lclbPlanList.Any(lbus => lbus.icdoOrgPlan.plan_id == lobjOrgPlan.icdoOrgPlan.plan_id))
                {
                    if (adtEmpStartDate < lobjOrgPlan.icdoOrgPlan.participation_start_date)
                        lobjOrgPlan.idtStartDate = lobjOrgPlan.icdoOrgPlan.participation_start_date;
                    else
                        lobjOrgPlan.idtStartDate = adtEmpStartDate;
                    if (adtEmpEndDate > DateTime.Today)
                        lobjOrgPlan.idtEndDate = DateTime.Today;
                    else
                        lobjOrgPlan.idtEndDate = adtEmpEndDate;

                    GenerateMonthlyEmployerStatmentForSpecifiedDates(DateTime.Today, lobjOrgPlan);
                    lclbPlanList.Add(lobjOrgPlan);
                }

            }


            var lvarMonthlyStatementByBenefitType = from lobjOrgPlan in iclbActiveOrgPlan
                                                    group lobjOrgPlan by new { lobjOrgPlan.ibusPlan.icdoPlan.benefit_type_value }
                                                        into MonthlyStatementByBenefitType
                                                    select new
                                                    {
                                                        lstrBenefitType = MonthlyStatementByBenefitType.Key.benefit_type_value,
                                                        ldecBeginnigBalance = MonthlyStatementByBenefitType.Sum(o => o.idecNewBeginingBalanceAmount), //PIR 6543
                                                        ldecInvoiceAmount = MonthlyStatementByBenefitType.Sum(o => o.idecInvoiceAmount),
                                                        ldecPaymentsAmount = MonthlyStatementByBenefitType.Sum(o => o.idecNewRemittanceAmount),
                                                        ldecCurrentAmountDue = MonthlyStatementByBenefitType.Sum(o => o.idecNewMonthDueAmount)
                                                    };
            iclbusEssMontlyAccountBalance = new Collection<busEssMontlyAccountBalance>();
            Array.ForEach(lvarMonthlyStatementByBenefitType.ToArray(), o =>
            {
                if (o.lstrBenefitType == busConstant.PlanBenefitTypeRetirement)
                {
                    idecRetCompBeginnineBalanceAmount = o.ldecBeginnigBalance; //PIR 6543
                    idecRetCompCurrentDueAmount = o.ldecCurrentAmountDue;
                    idecRetCompInvoiceAmount = o.ldecInvoiceAmount;
                    idecRetPaymentsAmount = o.ldecPaymentsAmount;
                    iclbusEssMontlyAccountBalance.Add(new busEssMontlyAccountBalance()
                    {
                        istrBenefitType = o.lstrBenefitType,
                        idecAccountBalance = o.ldecCurrentAmountDue,
                        istrBenefitDesc = busGlobalFunctions.GetDescriptionByCodeValue(1212, o.lstrBenefitType, this.iobjPassInfo)
                    });
                }
                else if (o.lstrBenefitType == busConstant.PlanBenefitTypeInsurance)
                {
                    idecInsCompBeginnineBalanceAmount = o.ldecBeginnigBalance; //PIR 6543
                    idecInsCompCurrentDueAmount = o.ldecCurrentAmountDue;
                    idecInsCompInvoiceAmount = o.ldecInvoiceAmount;
                    idecInsPaymentsAmount = o.ldecPaymentsAmount;
                    iclbusEssMontlyAccountBalance.Add(new busEssMontlyAccountBalance()
                    {
                        istrBenefitType = o.lstrBenefitType,
                        idecAccountBalance = o.ldecCurrentAmountDue,
                        istrBenefitDesc = busGlobalFunctions.GetDescriptionByCodeValue(1212, o.lstrBenefitType, this.iobjPassInfo)
                    });
                }
                else if (o.lstrBenefitType == busConstant.PlanBenefitTypeDeferredComp)
                {
                    idecDefCompBeginnineBalanceAmount = o.ldecBeginnigBalance; //PIR 6543
                    idecDefCompCurrentDueAmount = o.ldecCurrentAmountDue;
                    idecDefCompInvoiceAmount = o.ldecInvoiceAmount;
                    idecDefPaymentsAmount = o.ldecPaymentsAmount;
                    iclbusEssMontlyAccountBalance.Add(new busEssMontlyAccountBalance()
                    {
                        istrBenefitType = o.lstrBenefitType,
                        idecAccountBalance = o.ldecCurrentAmountDue,
                        istrBenefitDesc = busGlobalFunctions.GetDescriptionByCodeValue(1212, o.lstrBenefitType, this.iobjPassInfo)
                    });
                }
            });
        }

        public DataSet btnGenerateAgencyExcelReport_Click(int aintOrgId, string astrBenefitType, string astrFromDate, string astrToDate)
        {
            DataSet ldstReportDataSet = new DataSet();
            DataTable ldtBillingPaymentTable = new DataTable();
            DataTable ldtPaymentTable = new DataTable();
            DataTable ldtSummaryTable = new DataTable();
            ArrayList larrList = new ArrayList();
            utlError lobjError = new utlError();

            //Create Summary DataTable Column
            DataColumn ldc1 = new DataColumn("BALANCE_FORWARD", Type.GetType("System.Decimal"));
            DataColumn ldc2 = new DataColumn("BILLINGS_ADJUSTMENT", Type.GetType("System.Decimal"));
            DataColumn ldc3 = new DataColumn("PAYMENTS", Type.GetType("System.Decimal"));
            DataColumn ldc4 = new DataColumn("CURRENT_AMOUNT_DUE", Type.GetType("System.Decimal"));
            DataColumn ldc5 = new DataColumn("FROM_DATE", Type.GetType("System.DateTime"));
            DataColumn ldc6 = new DataColumn("TO_DATE", Type.GetType("System.DateTime"));
            ldtSummaryTable.Columns.Add(ldc1);
            ldtSummaryTable.Columns.Add(ldc2);
            ldtSummaryTable.Columns.Add(ldc3);
            ldtSummaryTable.Columns.Add(ldc4);
            ldtSummaryTable.Columns.Add(ldc5);
            ldtSummaryTable.Columns.Add(ldc6);

            //Fetch Data w.r.t From date & To date.
            LoadAgencyStatementInfoWithinDates(aintOrgId, Convert.ToDateTime(astrFromDate), Convert.ToDateTime(astrToDate));
            if (astrBenefitType.Equals("Retirement"))
            {
                DataRow drReport = ldtSummaryTable.NewRow();
                drReport["BALANCE_FORWARD"] = idecRetCompBeginnineBalanceAmount;
                drReport["BILLINGS_ADJUSTMENT"] = idecRetCompInvoiceAmount;
                drReport["PAYMENTS"] = idecRetPaymentsAmount;
                drReport["CURRENT_AMOUNT_DUE"] = idecRetCompCurrentDueAmount;
                drReport["FROM_DATE"] = Convert.ToDateTime(astrFromDate);
                drReport["TO_DATE"] = Convert.ToDateTime(astrToDate);
                ldtSummaryTable.Rows.Add(drReport);

                ldtBillingPaymentTable = Select("cdoWssPersonEmployment.LoadAgencyStmtRetirementBillingAndPayment", new object[3] { aintOrgId, astrFromDate, astrToDate });
            }
            else if (astrBenefitType.Equals("Insurance"))
            {
                DataRow drReport = ldtSummaryTable.NewRow();
                drReport["BALANCE_FORWARD"] = idecInsCompBeginnineBalanceAmount;
                drReport["BILLINGS_ADJUSTMENT"] = idecInsCompInvoiceAmount;
                drReport["PAYMENTS"] = idecInsPaymentsAmount;
                drReport["CURRENT_AMOUNT_DUE"] = idecInsCompCurrentDueAmount;
                drReport["FROM_DATE"] = Convert.ToDateTime(astrFromDate);
                drReport["TO_DATE"] = Convert.ToDateTime(astrToDate);
                ldtSummaryTable.Rows.Add(drReport);

                ldtBillingPaymentTable = Select("cdoWssPersonEmployment.LoadAgencyStmtInsuranceBillingAndPayment", new object[3] { aintOrgId, astrFromDate, astrToDate });
            }
            else if (astrBenefitType.Equals("DefComp"))
            {
                DataRow drReport = ldtSummaryTable.NewRow();
                drReport["BALANCE_FORWARD"] = idecDefCompBeginnineBalanceAmount;
                drReport["BILLINGS_ADJUSTMENT"] = idecDefCompInvoiceAmount;
                drReport["PAYMENTS"] = idecDefPaymentsAmount;
                drReport["CURRENT_AMOUNT_DUE"] = idecDefCompCurrentDueAmount;
                drReport["FROM_DATE"] = Convert.ToDateTime(astrFromDate);
                drReport["TO_DATE"] = Convert.ToDateTime(astrToDate);
                ldtSummaryTable.Rows.Add(drReport);

                ldtBillingPaymentTable = Select("cdoWssPersonEmployment.LoadAgencyStmtDefCompBillingAndPayment", new object[3] { aintOrgId, astrFromDate, astrToDate });
            }

            ldtBillingPaymentTable.TableName = busConstant.ReportTableName;
            ldstReportDataSet.Tables.Add(ldtBillingPaymentTable.Copy());
            ldtSummaryTable.TableName = busConstant.ReportTableName02;
            ldstReportDataSet.Tables.Add(ldtSummaryTable.Copy());

            return ldstReportDataSet;
        }

        public void LoadAgencyStatementInfo(int aintOrgId)
        {
            DataTable ldtbOrgPlan = Select("cdoEmployerPayrollMonthlyStatement.GetActiveOrgPlanForGivenOrgId", new object[2] { aintOrgId, DateTime.Now });
            LoadOrgPlan(ldtbOrgPlan);
            icdoEmployerPayrollMonthlyStatement = new cdoEmployerPayrollMonthlyStatement();
            //prod pir 6488 : need to include service purchase posted header only once
            iblnServicePurchaseIncluded = false;

            Collection<busOrgPlan> lclbPlanList = new Collection<busOrgPlan>();
            foreach (busOrgPlan lobjOrgPlan in iclbActiveOrgPlan)
            {
                //PIR 10707
                if (!lclbPlanList.Any(lbus => lbus.icdoOrgPlan.plan_id == lobjOrgPlan.icdoOrgPlan.plan_id))
                {
                    if (lobjOrgPlan.ibusPlan == null)
                        lobjOrgPlan.LoadPlanInfo();
                    DataTable ldtbStatement = Select<cdoEmployerPayrollMonthlyStatement>(new string[2] { "org_id", "plan_id" },
                      new object[2] { lobjOrgPlan.icdoOrgPlan.org_id, lobjOrgPlan.icdoOrgPlan.plan_id }, null, null);
                    Collection<busEmployerPayrollMonthlyStatement> lclbStatement = GetCollection<busEmployerPayrollMonthlyStatement>(ldtbStatement, "icdoEmployerPayrollMonthlyStatement");
                    if (lclbStatement.Count > 0)
                    {
                        icdoEmployerPayrollMonthlyStatement.idtStartDate = lclbStatement.Select(o => o.icdoEmployerPayrollMonthlyStatement.run_date).Max();
                    }
                    else
                    {
                        icdoEmployerPayrollMonthlyStatement.idtStartDate = lobjOrgPlan.icdoOrgPlan.participation_start_date;
                    }
                    icdoEmployerPayrollMonthlyStatement.idtEndDate = DateTime.Today;

                    GenerateMonthlyEmployerStatment(DateTime.Today, lobjOrgPlan);
                    lclbPlanList.Add(lobjOrgPlan);
                }

            }


            var lvarMonthlyStatementByBenefitType = from lobjOrgPlan in iclbActiveOrgPlan
                                                    group lobjOrgPlan by new { lobjOrgPlan.ibusPlan.icdoPlan.benefit_type_value }
                                                        into MonthlyStatementByBenefitType
                                                    select new
                                                    {
                                                        lstrBenefitType = MonthlyStatementByBenefitType.Key.benefit_type_value,
                                                        ldecBeginnigBalance = MonthlyStatementByBenefitType.Sum(o => o.idecNewBeginingBalanceAmount), //PIR 6543
                                                        ldecInvoiceAmount = MonthlyStatementByBenefitType.Sum(o => o.idecInvoiceAmount),
                                                        ldecPaymentsAmount = MonthlyStatementByBenefitType.Sum(o => o.idecNewRemittanceAmount),
                                                        ldecCurrentAmountDue = MonthlyStatementByBenefitType.Sum(o => o.idecNewMonthDueAmount)
                                                    };
            iclbusEssMontlyAccountBalance = new Collection<busEssMontlyAccountBalance>();
            Array.ForEach(lvarMonthlyStatementByBenefitType.ToArray(), o =>
                    {
                        if (o.lstrBenefitType == busConstant.PlanBenefitTypeRetirement)
                        {
                            idecRetCompBeginnineBalanceAmount = o.ldecBeginnigBalance; //PIR 6543
                            idecRetCompCurrentDueAmount = o.ldecCurrentAmountDue;
                            idecRetCompInvoiceAmount = o.ldecInvoiceAmount;
                            idecRetPaymentsAmount = o.ldecPaymentsAmount;
                            iclbusEssMontlyAccountBalance.Add(new busEssMontlyAccountBalance()
                            {
                                istrBenefitType = o.lstrBenefitType,
                                idecAccountBalance = o.ldecCurrentAmountDue,
                                istrBenefitDesc = busGlobalFunctions.GetDescriptionByCodeValue(1212, o.lstrBenefitType, this.iobjPassInfo)
                            });
                        }
                        else if (o.lstrBenefitType == busConstant.PlanBenefitTypeInsurance)
                        {
                            idecInsCompBeginnineBalanceAmount = o.ldecBeginnigBalance; //PIR 6543
                            idecInsCompCurrentDueAmount = o.ldecCurrentAmountDue;
                            idecInsCompInvoiceAmount = o.ldecInvoiceAmount;
                            idecInsPaymentsAmount = o.ldecPaymentsAmount;
                            iclbusEssMontlyAccountBalance.Add(new busEssMontlyAccountBalance()
                            {
                                istrBenefitType = o.lstrBenefitType,
                                idecAccountBalance = o.ldecCurrentAmountDue,
                                istrBenefitDesc = busGlobalFunctions.GetDescriptionByCodeValue(1212, o.lstrBenefitType, this.iobjPassInfo)
                            });
                        }
                        else if (o.lstrBenefitType == busConstant.PlanBenefitTypeDeferredComp)
                        {
                            idecDefCompBeginnineBalanceAmount = o.ldecBeginnigBalance; //PIR 6543
                            idecDefCompCurrentDueAmount = o.ldecCurrentAmountDue;
                            idecDefCompInvoiceAmount = o.ldecInvoiceAmount;
                            idecDefPaymentsAmount = o.ldecPaymentsAmount;
                            iclbusEssMontlyAccountBalance.Add(new busEssMontlyAccountBalance()
                            {
                                istrBenefitType = o.lstrBenefitType,
                                idecAccountBalance = o.ldecCurrentAmountDue,
                                istrBenefitDesc = busGlobalFunctions.GetDescriptionByCodeValue(1212, o.lstrBenefitType, this.iobjPassInfo)
                            });
                        }
                    });
        }
    }
    [Serializable]
    public class busEssMontlyAccountBalance
    {
        public string istrBenefitType { get; set; }
        public decimal idecAccountBalance { get; set; }
        public string istrBenefitDesc { get; set; }

    }
}
