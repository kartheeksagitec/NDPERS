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
using Sagitec.CustomDataObjects;
using System.Linq;
using System.Globalization;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountDeferredComp : busPersonAccountDeferredCompGen
    {
        public int countPos { get; set; }
        // Used in Correspondence to differentiate
        public string istrIsMonthlyBatch { get; set; }
        public bool iblnIsFromMSS { get; set; }// PIR 6961
        //PIR 13815
        public string istrAllowOverlapHistory { get; set; }

        public int CYTDID { get { return 1; } }
		
        public int LTDID { get { return 2; } }

        public Collection<busPersonAccountDeferredCompHistory> iclbOverlappingHistory { get; set; }

        public DateTime idteRMDEligibleDate { get; set; }

        public string istrRMDEligibleMonth
        {
            get
            {
                return idteRMDEligibleDate.ToString("MMM");
            }
        }

        public int iintRMDEligibleYear
        {
            get
            {
                return idteRMDEligibleDate.Year;
            }
        }

        public int idteNextYearToRMD
        {
            get { return idteRMDEligibleDate.Year + 1; }
        }

        private Collection<busPersonAccountDeferredCompContribution> _iclb457Contributions;
        public Collection<busPersonAccountDeferredCompContribution> iclb457Contributions
        {
            get { return _iclb457Contributions; }
            set { _iclb457Contributions = value; }
        }

        private decimal _idecProjectedPayPeriodAmount;
        public decimal idecProjectedPayPeriodAmount
        {
            get { return _idecProjectedPayPeriodAmount; }
            set { _idecProjectedPayPeriodAmount = value; }
        }
        //UAT PIR-995 Used for Batch Report Approaching 457 Contribution Limit Batch.
        private decimal _idecMonthlyContributionAmount;
        public decimal idecMonthlyContributionAmount
        {
            get { return _idecMonthlyContributionAmount; }
            set { _idecMonthlyContributionAmount = value; }
        }
        private decimal _idecCurrentPayPeriodAmount;
        public decimal idecCurrentPayPeriodAmount
        {
            get { return _idecCurrentPayPeriodAmount; }
            set { _idecCurrentPayPeriodAmount = value; }
        }

        public string IsCYTD
        {
            get { return busConstant.Flag_Yes; }
        }
        private string _Limit_457_Contribution_Cytd;
        public string Limit_457_Contribution_Cytd
        {
            get
            {
                _Limit_457_Contribution_Cytd = icdoPersonAccountDeferredComp.limit_457_description + ' ' + '$'
                                                 + Convert.ToString(_idecPayPeriodAmountCytd);
                return _Limit_457_Contribution_Cytd;
            }
            set
            {
                _Limit_457_Contribution_Cytd = value;
            }
        }
        private string _Limit_457_Contribution_Ltd;
        public string Limit_457_Contribution_Ltd
        {
            get
            {

                _Limit_457_Contribution_Ltd = icdoPersonAccountDeferredComp.limit_457_description + ' ' + '$'
                                               + Convert.ToString(_idecPayPeriodAmountLtd);
                return _Limit_457_Contribution_Ltd;
            }
            set
            {
                _Limit_457_Contribution_Ltd = value;
            }
        }
        private decimal _idecPayPeriodAmountCytd;

        public decimal idecPayPeriodAmountCytd
        {
            get { return _idecPayPeriodAmountCytd; }
            set { _idecPayPeriodAmountCytd = value; }
        }
        private decimal _idecPayPeriodAmountLtd;

        public decimal idecPayPeriodAmountLtd
        {
            get { return _idecPayPeriodAmountLtd; }
            set { _idecPayPeriodAmountLtd = value; }
        }

        private busPersonAccountDeferredCompHistory _ibusHistory;
        public busPersonAccountDeferredCompHistory ibusHistory
        {
            get
            {
                return _ibusHistory;
            }
            set
            {
                _ibusHistory = value;
            }
        }

        private Collection<busPersonAccountDeferredCompContribution> _iclbDeferredCompContribution;

        public Collection<busPersonAccountDeferredCompContribution> iclbDeferredCompContribution
        {
            get { return _iclbDeferredCompContribution; }
            set { _iclbDeferredCompContribution = value; }
        }

        //Collection which contain the original DeferredComp contribution for a given person account id
        private Collection<busPersonAccountDeferredCompContribution> _iclbOriginalDeferredCompContribution;
        public Collection<busPersonAccountDeferredCompContribution> iclbOriginalDeferredCompContribution
        {
            get { return _iclbOriginalDeferredCompContribution; }
            set { _iclbOriginalDeferredCompContribution = value; }
        }
        public String istrIsMutualFundFlagRequired
        {
            get
            {
                return busGlobalFunctions.GetData1ByCodeValue(52, busConstant.MutualFundWindowFlag, utlPassInfo.iobjPassInfo);
            }
        }
        #region DC/457 TIAACREF Enrollment file
        public string filler0
        {
            get
            {
                return "0000000000000000000000000000000000000000";
            }
        }

        public string istrNewLine
        {
            get
            {
                //PIR 9122
                countPos = countPos + 1;
                if (this.ibusPerson.ibusPersonCurrentAddress.icdoPersonCurrentAddress.addr_country_value == "0001" && (this.countPos == 3))
                {
                    return string.Empty;
                }
                else
                return Environment.NewLine;
            }
        }
        #endregion
        public void LoadCYTDDetail()
        {
            _iclbDeferredCompContribution = new Collection<busPersonAccountDeferredCompContribution>();
            DateTime CYTDStartDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime CYTDEndDDate = new DateTime(DateTime.Now.Year, 12, 31);
            DataTable ldtbList = Select("cdoPersonAccountDeferredCompContribution.LoadCYTDDetail",
                    new object[3] { CYTDStartDate, CYTDEndDDate, icdoPersonAccount.person_account_id });
            foreach (DataRow dr in ldtbList.Rows)
            {
                busPersonAccountDeferredCompContribution lbusPADCCont = new busPersonAccountDeferredCompContribution { icdoPersonAccountDeferredCompContribution = new cdoPersonAccountDeferredCompContribution() };
                lbusPADCCont.icdoPersonAccountDeferredCompContribution.LoadData(dr);
                lbusPADCCont.ibusPADeferredComp = this;
                lbusPADCCont.ibusPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lbusPADCCont.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                lbusPADCCont.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                if (!Convert.IsDBNull(dr["org_code"]))
                    lbusPADCCont.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code = dr["org_code"].ToString();
                lbusPADCCont.ibusProvider = new busOrganization { icdoOrganization = new cdoOrganization() };
                lbusPADCCont.ibusProvider.icdoOrganization.org_id = lbusPADCCont.icdoPersonAccountDeferredCompContribution.provider_org_id;

                if (!Convert.IsDBNull(dr["PROVIDER_ORG_NAME"]))
                    lbusPADCCont.ibusProvider.icdoOrganization.org_name = dr["PROVIDER_ORG_NAME"].ToString();

                if (!Convert.IsDBNull(dr["PROVIDER_ORG_CODE"]))
                    lbusPADCCont.ibusProvider.icdoOrganization.org_code = dr["PROVIDER_ORG_CODE"].ToString();

                lbusPADCCont.icdoPersonAccountDeferredCompContribution.istrPayPeriodStartDateFormatted =
                    lbusPADCCont.icdoPersonAccountDeferredCompContribution.pay_period_start_date == DateTime.MinValue ? string.Empty :
                    lbusPADCCont.icdoPersonAccountDeferredCompContribution.pay_period_start_date.ToString("MM/dd/yyyy");
                lbusPADCCont.icdoPersonAccountDeferredCompContribution.istrPayPeriodEndDateFormatted =
                    lbusPADCCont.icdoPersonAccountDeferredCompContribution.pay_period_end_date == DateTime.MaxValue ? string.Empty :
                    lbusPADCCont.icdoPersonAccountDeferredCompContribution.pay_period_end_date.ToString("MM/dd/yyyy");
                _iclbDeferredCompContribution.Add(lbusPADCCont);
            }
            _iclbOriginalDeferredCompContribution = _iclbDeferredCompContribution;
        }
        public void LoadLTDDetail()
        {
            _iclbDeferredCompContribution = new Collection<busPersonAccountDeferredCompContribution>();
            DataTable ldtbList = Select("cdoPersonAccountDeferredCompContribution.LoadLTDDetail",
                    new object[1] { icdoPersonAccount.person_account_id });
            foreach (DataRow dr in ldtbList.Rows)
            {
                busPersonAccountDeferredCompContribution lbusPADCCont = new busPersonAccountDeferredCompContribution { icdoPersonAccountDeferredCompContribution = new cdoPersonAccountDeferredCompContribution() };
                lbusPADCCont.icdoPersonAccountDeferredCompContribution.LoadData(dr);
                lbusPADCCont.ibusPADeferredComp = this;
                lbusPADCCont.ibusPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lbusPADCCont.ibusPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                lbusPADCCont.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                if (!Convert.IsDBNull(dr["org_code"]))
                    lbusPADCCont.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code = dr["org_code"].ToString();
                lbusPADCCont.ibusProvider = new busOrganization { icdoOrganization = new cdoOrganization() };
                lbusPADCCont.ibusProvider.icdoOrganization.org_id = lbusPADCCont.icdoPersonAccountDeferredCompContribution.provider_org_id;

                if (!Convert.IsDBNull(dr["PROVIDER_ORG_NAME"]))
                    lbusPADCCont.ibusProvider.icdoOrganization.org_name = dr["PROVIDER_ORG_NAME"].ToString();

                if (!Convert.IsDBNull(dr["PROVIDER_ORG_CODE"]))
                    lbusPADCCont.ibusProvider.icdoOrganization.org_code = dr["PROVIDER_ORG_CODE"].ToString();

                lbusPADCCont.icdoPersonAccountDeferredCompContribution.istrPayPeriodStartDateFormatted =
                    lbusPADCCont.icdoPersonAccountDeferredCompContribution.pay_period_start_date == DateTime.MinValue ? string.Empty :
                    lbusPADCCont.icdoPersonAccountDeferredCompContribution.pay_period_start_date.ToString("MM/dd/yyyy");
                lbusPADCCont.icdoPersonAccountDeferredCompContribution.istrPayPeriodEndDateFormatted =
                    lbusPADCCont.icdoPersonAccountDeferredCompContribution.pay_period_end_date == DateTime.MaxValue ? string.Empty :
                    lbusPADCCont.icdoPersonAccountDeferredCompContribution.pay_period_end_date.ToString("MM/dd/yyyy");
                _iclbDeferredCompContribution.Add(lbusPADCCont);
            }
            _iclbOriginalDeferredCompContribution = _iclbDeferredCompContribution;
        }

        public void LoadContributionSummary()
        {
            DateTime CYTDStartDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime CYTDEndDDate = new DateTime(DateTime.Now.Year, 12, 31);
            _idecPayPeriodAmountCytd = (decimal)DBFunction.DBExecuteScalar("cdoPersonAccountDeferredComp.ContrbutionSummaryCYTD",
                  new object[3] { CYTDStartDate, CYTDEndDDate, icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            _idecPayPeriodAmountLtd = (decimal)DBFunction.DBExecuteScalar("cdoPersonAccountDeferredComp.ContrbutionSummaryLTD",
                  new object[1] { icdoPersonAccount.person_account_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
        }
        public void LoadContributionsFor457BatchLetter(DateTime adtStartDate, DateTime adtEndDate)
        {
            DataTable ldtbContribution = Select("cdoPersonAccountDeferredComp.ContrbutionSummaryByPayCheckDate",
                                               new object[3] { icdoPersonAccount.person_account_id, adtEndDate, adtStartDate });
            iclb457Contributions = GetCollection<busPersonAccountDeferredCompContribution>(ldtbContribution, "icdoPersonAccountDeferredCompContribution");
        }
        public bool IsSumOfDeductionsZeroForPast24Months()
        {
            decimal ldecTotalDeductionAmount =
                     Convert.ToDecimal(DBFunction.DBExecuteScalar("cdoPersonAccountDeferredComp.SumOfDeductionsForPast24Months",
                                                                  new object[3]
                                                                 {                                                                     
                                                                     DateTime.Now.AddMonths(-24),
                                                                     DateTime.Now,
                                                                     icdoPersonAccountDeferredComp.person_account_id
                                                                 },
                                                                  iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            if (ldecTotalDeductionAmount > 0.00M)
            {
                return true;
            }
            return false;
        }
        public bool CheckDeMinimusOptionUsedBefore()
        {
            if (ibusHistory == null)
                LoadPreviousHistory();

            if (ibusHistory.icdoPersonAccountDeferredCompHistory.person_account_deferred_comp_history_id > 0)
            {
                if (icdoPersonAccountDeferredComp.de_minimus_distribution_flag == busConstant.Flag_Yes)
                {
                    if (icdoPersonAccountDeferredComp.de_minimus_distribution_flag != ibusHistory.icdoPersonAccountDeferredCompHistory.de_minimus_distribution_flag)
                    {
                        foreach (busPersonAccountDeferredCompHistory lobjPADeffCompHistory in icolPADeferredCompHistory)
                        {
                            if (lobjPADeffCompHistory.icdoPersonAccountDeferredCompHistory.de_minimus_distribution_flag == busConstant.Flag_Yes)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public bool IsStartDateNotFirstOfMonth()
        {
            if (icdoPersonAccount.start_date != DateTime.MinValue)
            {
                if (icdoPersonAccount.start_date.Day != 1)
                {
                    return true;
                }
            }
            return false;
        }
        public int CalculateDeffCompEnrollingPersonAge()
        {
            // get the difference in years
            if (ibusPerson != null)
            {
                DateTime ldtPersonDateOfBirth = ibusPerson.icdoPerson.date_of_birth;
                DateTime ldtCalculationDate = new DateTime(DateTime.Now.Year, 12, 31);
                return busGlobalFunctions.CalulateAge(ldtPersonDateOfBirth, ldtCalculationDate);
            }
            return 0;
        }

        //property to check whether enrollment change is happended in def. comp provider maintenance screen
        public bool iblnCheckForEnrollmentChange { get; set; }

        public override void BeforePersistChanges()
        {
            if (icdoPersonAccount.ienuObjectState == ObjectState.Insert)
            {
                //UAT PIR 2373 people soft file changes
                //--Start--//
                //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                //--End--//
                icdoPersonAccount.history_change_date = icdoPersonAccount.start_date;
                icdoPersonAccount.Insert();
                icdoPersonAccountDeferredComp.person_account_id = icdoPersonAccount.person_account_id;
            }
            if (icdoPersonAccount.suppress_warnings_flag == "Y")
            {
                icdoPersonAccount.suppress_warnings_by = iobjPassInfo.istrUserID;
            }
            if (icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation)
            {
                icdoPersonAccountDeferredComp.file_457_sent_flag = busConstant.Flag_No;
            }
            Set457Limit();
            if (icdoPersonAccount.ienuObjectState == ObjectState.Update)
            {
                if (icdoPersonAccountDeferredComp.hardship_withdrawal_flag == busConstant.Flag_Yes)
                {
                    Collection<busPersonAccountDeferredCompProvider> lclcDCPro = new Collection<busPersonAccountDeferredCompProvider>();
                    DataTable ldtbList = Select<cdoPersonAccountDeferredCompProvider>(new string[1] { "person_account_id" },
                                                      new object[1] { icdoPersonAccountDeferredComp.person_account_id }, null, "person_account_provider_id desc");
                    lclcDCPro = GetCollection<busPersonAccountDeferredCompProvider>(ldtbList, "icdoPersonAccountDeferredCompProvider");
                    if (lclcDCPro.Count > 0)
                    {
                        lclcDCPro[0].icdoPersonAccountDeferredCompProvider.end_date = icdoPersonAccountDeferredComp.hardship_withdrawal_effective_date;
                        lclcDCPro[0].icdoPersonAccountDeferredCompProvider.Update();
                    }
                }
            }

            //UAT PIR 2373 people soft file changes
            //--Start--//
            if (IsHistoryEntryRequired && (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompSuspended ||
                icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompCancelled))
            {
                SetPersonAcccountForTeminationChange();
                iblnCheckForEnrollmentChange = false;
            }
            else
            {
                iblnCheckForEnrollmentChange = true;
                SetPersonAccountForEnrollmentChange();
            }
            //--End--//
        }

        /// <summary>
        /// uat pir 2373 : method to set the PS event value based on enrollment change
        /// </summary>
        private void SetPersonAccountForEnrollmentChange()
        {
            if (ibusHistory == null)
                LoadPreviousHistory();
            if (IsHistoryEntryRequired)
            {
                if (ibusHistory.icdoPersonAccountDeferredCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                    icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
                    icdoPersonAccount.ps_file_change_event_value = busConstant.NewEnrollment;
                    iblnCheckForEnrollmentChange = false;
                }
            }
        }

        public void Set457Limit()
        {
            /**********************************/
            //UAT PIR: 988
            //Logic: The Default is Regular. 
            //When the person Age is >= 50 then 50 +
            //When the current date between the Catchup Start Date and End date then Catch up.
            /**********************************/
            /* PROD PIR 4014 and as per Maik's mail 
             * Regular – Current date is outside Catch-Up date range and member age is below 50
                Catch-Up – Current date is within Catch-Up date range and member age is below 50
                50+ – Member age on current date is above 50 regardless of catch-up date range
             * Changed on 20 Oct 2010
             */
            //Prod PIR 4366
            //457 limit should not be set for Plan Other 457
            if (icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation)
            {
                //new logic as per Sharmain & Maik 
                //Prod PIR 4457
                if ((icdoPersonAccountDeferredComp.catch_up_start_date != DateTime.MinValue) && (icdoPersonAccountDeferredComp.catch_up_end_date != DateTime.MinValue) &&
                    busGlobalFunctions.CheckDateOverlapping(DateTime.Now, new DateTime(icdoPersonAccountDeferredComp.catch_up_start_date.Year, 1, 1),
                    new DateTime(icdoPersonAccountDeferredComp.catch_up_end_date.Year, 12, 31)))
                {
                    icdoPersonAccountDeferredComp.limit_457_value = busConstant.PersonAccount457LimitCatchUp;
                }
                else if (CalculateDeffCompEnrollingPersonAge() >= 50)
                {
                    icdoPersonAccountDeferredComp.limit_457_value = busConstant.PersonAccount457Limit50;
                }
                else
                {
                    icdoPersonAccountDeferredComp.limit_457_value = busConstant.PersonAccount457LimitRegular;
                }

                /*else
                {
                    if ((icdoPersonAccountDeferredComp.catch_up_start_date == DateTime.MinValue) && (icdoPersonAccountDeferredComp.catch_up_end_date == DateTime.MinValue))
                    {
                        icdoPersonAccountDeferredComp.limit_457_value = busConstant.PersonAccount457LimitRegular;
                    }
                    else if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now, icdoPersonAccountDeferredComp.catch_up_start_date, icdoPersonAccountDeferredComp.catch_up_end_date))
                    {
                        icdoPersonAccountDeferredComp.limit_457_value = busConstant.PersonAccount457LimitCatchUp;
                    }
                    else
                    {
                        icdoPersonAccountDeferredComp.limit_457_value = busConstant.PersonAccount457LimitRegular;
                    }
                }*/
                /**********************************/
                //UAT PIR: 988 Ends
                /**********************************/

                icdoPersonAccountDeferredComp.limit_457_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(334,
                                                                                     icdoPersonAccountDeferredComp.limit_457_value);
            }
            else
            {
                icdoPersonAccountDeferredComp.limit_457_value = null;
                icdoPersonAccountDeferredComp.limit_457_description = null;
            }
        }
        public int iintRequestId;//PIR 6961
        public override void AfterPersistChanges()
        {
            base.AfterPersistChanges();
            ProcessHistory();
            LoadPADeffCompHistory();
            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
                SetPersonAccountIDInPersonAccountEmploymentDetail();
            }
            LoadPreviousHistory();
            // UAT PIR ID 1077 - To refresh the screen values only if the Suppress flag is On.
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes)
                RefreshValues();
            //uat pir 2118 : new workflow added
            if (ibusBaseActivityInstance != null)
                // SetProcessInstanceParameters();
                SetCaseInstanceParameters();
            if (iblnIsFromMSS)
            {
                //InsertWSSEnrollmentRequest();

                cdoWssPersonAccountDeferredComp lcdoWssPersonAccountDeferredComp = new cdoWssPersonAccountDeferredComp();
                lcdoWssPersonAccountDeferredComp.wss_person_account_enrollment_request_id = iintRequestId;
                lcdoWssPersonAccountDeferredComp.target_person_account_id = icdoPersonAccountDeferredComp.person_account_id;
                lcdoWssPersonAccountDeferredComp.catch_up_start_date = icdoPersonAccountDeferredComp.catch_up_start_date;
                lcdoWssPersonAccountDeferredComp.catch_up_end_date = icdoPersonAccountDeferredComp.catch_up_end_date;
                lcdoWssPersonAccountDeferredComp.limit_457_id = icdoPersonAccountDeferredComp.limit_457_id;
                lcdoWssPersonAccountDeferredComp.limit_457_value = icdoPersonAccountDeferredComp.limit_457_value;
                lcdoWssPersonAccountDeferredComp.hardship_withdrawal_flag = icdoPersonAccountDeferredComp.hardship_withdrawal_flag;
                lcdoWssPersonAccountDeferredComp.hardship_withdrawal_effective_date = icdoPersonAccountDeferredComp.hardship_withdrawal_effective_date;
                lcdoWssPersonAccountDeferredComp.de_minimus_distribution_flag = icdoPersonAccountDeferredComp.de_minimus_distribution_flag;
                lcdoWssPersonAccountDeferredComp.file_457_sent_flag = icdoPersonAccountDeferredComp.file_457_sent_flag;
                lcdoWssPersonAccountDeferredComp.Insert();
                iintRequestId = 0;
            }
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompSuspended)
                UpdateEndDateForSuspendStatus();
        }

        public void UpdateEndDateForSuspendStatus()
        {
            if (icolPersonAccountDeferredCompProvider == null || icolPersonAccountDeferredCompProvider.Count() == 0)
                LoadPersonAccountProviders();
            if (icolPersonAccountDeferredCompProvider.Count > 0)
            {
                busPersonAccountDeferredCompHistory lbusPersonAccountDeferredCompHistory = icolPADeferredCompHistory
                                                                                                   .FirstOrDefault(history =>
                                                                                                    history.icdoPersonAccountDeferredCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled &&
                                                                                                    history.icdoPersonAccountDeferredCompHistory.start_date.Date != history.icdoPersonAccountDeferredCompHistory.end_date.Date);
                DateTime ldteProviderEndDate = lbusPersonAccountDeferredCompHistory.IsNotNull() &&
                                                                                      lbusPersonAccountDeferredCompHistory.icdoPersonAccountDeferredCompHistory.end_date != DateTime.MinValue
                                                                                    ? lbusPersonAccountDeferredCompHistory.icdoPersonAccountDeferredCompHistory.end_date : icdoPersonAccount.history_change_date_no_null.AddDays(-1);
                foreach (busPersonAccountDeferredCompProvider lobjProvider in icolPersonAccountDeferredCompProvider)
                {
                    if (lobjProvider.icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue)
                    {
                        if (!iblnIsFromTerminationPost)
                            lobjProvider.icdoPersonAccountDeferredCompProvider.end_date = lobjProvider.icdoPersonAccountDeferredCompProvider.start_date > icdoPersonAccount.history_change_date_no_null.AddDays(-1) ? lobjProvider.icdoPersonAccountDeferredCompProvider.start_date : ldteProviderEndDate;
                        else
                            lobjProvider.icdoPersonAccountDeferredCompProvider.end_date = idtmProviderEndDateWhenTEEM;
                        lobjProvider.icdoPersonAccountDeferredCompProvider.Update();
                    }
                    else if (istrAllowOverlapHistory == "Y" && lobjProvider.icdoPersonAccountDeferredCompProvider.end_date > icdoPersonAccount.history_change_date_no_null.AddDays(-1))
                    {
                        lobjProvider.icdoPersonAccountDeferredCompProvider.end_date = lobjProvider.icdoPersonAccountDeferredCompProvider.start_date > icdoPersonAccount.history_change_date_no_null.AddDays(-1) ? lobjProvider.icdoPersonAccountDeferredCompProvider.start_date : icdoPersonAccount.history_change_date_no_null.AddDays(-1);
                        lobjProvider.icdoPersonAccountDeferredCompProvider.Update();
                    }
                }
            }
        }
        public DateTime idtmProviderEndDateWhenTEEM { get; set; }
        //PIR 6961
        public void InsertWSSEnrollmentWaiveRequest()
        {
            iintRequestId = 0;
            cdoWssPersonAccountEnrollmentRequest lcdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest();
            LoadPersonAccount();
            LoadPersonAccountEmploymentDetails();
            lcdoWssPersonAccountEnrollmentRequest.person_id = ibusPersonAccount.icdoPersonAccount.person_id;
            lcdoWssPersonAccountEnrollmentRequest.plan_id = ibusPersonAccount.icdoPersonAccount.plan_id;
            lcdoWssPersonAccountEnrollmentRequest.target_person_account_id = ibusPersonAccount.icdoPersonAccount.person_account_id;
            lcdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = ibusPersonAccount.icdoPersonAccount.person_employment_dtl_id;
            lcdoWssPersonAccountEnrollmentRequest.reason_value = ibusPersonAccount.icdoPersonAccount.reason_value;
            lcdoWssPersonAccountEnrollmentRequest.status_id = busConstant.MemberPortalEnrollmentRequestStatus;
            lcdoWssPersonAccountEnrollmentRequest.status_value = busConstant.StatusProcessed;
            lcdoWssPersonAccountEnrollmentRequest.enrollment_type_id = busConstant.MemberPortalEnrollmentType;
            lcdoWssPersonAccountEnrollmentRequest.enrollment_type_value = busConstant.DeferredCompensation;
            lcdoWssPersonAccountEnrollmentRequest.plan_enrollment_option_value = busConstant.PlanEnrollmentOptionValueWaive;
            lcdoWssPersonAccountEnrollmentRequest.reason_value = busConstant.ReasonValueWaivePlan; // PIR 10054
            lcdoWssPersonAccountEnrollmentRequest.Insert();
            iintRequestId = lcdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id;        
        }

        public void RefreshValues()
        {
            icdoPersonAccount.suppress_warnings_flag = string.Empty; // UAT PIR ID 1015
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            iclbOverlappingHistory = new Collection<busPersonAccountDeferredCompHistory>();
            if ((istrAllowOverlapHistory == busConstant.Flag_Yes) && (icdoPersonAccount.history_change_date != DateTime.MinValue))
            {
                Collection<busPersonAccountDeferredCompHistory> lclbOpenHistory = LoadOverlappingHistory();
                if (lclbOpenHistory.Count > 0)
                {
                    foreach (busPersonAccountDeferredCompHistory lbusPADefComp in lclbOpenHistory)
                    {
                        icolPADeferredCompHistory.Remove(lbusPADefComp);
                        iclbOverlappingHistory.Add(lbusPADefComp);
                    }
                }
            }

            if (ibusHistory == null)
                LoadPreviousHistory();

            // PIR 2165
            LoadPlanEffectiveDate();
            if (icdoPersonAccount.person_account_id > 0)
            {
                icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                LoadPersonEmploymentDetail();
                ibusPersonEmploymentDetail.LoadPersonEmployment();
                ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            }
            LoadOrgPlan(idtPlanEffectiveDate);
            SetHistoryEntryRequiredOrNot();
            base.BeforeValidate(aenmPageMode);
        }

        public void InsertHistory()
        {
            cdoPersonAccountDeferredCompHistory lobjCdoDeffCompHistory = new cdoPersonAccountDeferredCompHistory();
            lobjCdoDeffCompHistory.person_account_id = icdoPersonAccount.person_account_id;
            lobjCdoDeffCompHistory.plan_participation_status_value = icdoPersonAccount.plan_participation_status_value;
            lobjCdoDeffCompHistory.start_date = icdoPersonAccount.history_change_date;
            lobjCdoDeffCompHistory.status_value = icdoPersonAccount.status_value;
            if (icdoPersonAccount.suppress_warnings_flag == busConstant.Flag_Yes) // PROD PIR 7079
                lobjCdoDeffCompHistory.suppress_warnings_by = icdoPersonAccount.suppress_warnings_by;
            lobjCdoDeffCompHistory.suppress_warnings_date = icdoPersonAccount.suppress_warnings_date;
            lobjCdoDeffCompHistory.suppress_warnings_flag = icdoPersonAccount.suppress_warnings_flag;
            lobjCdoDeffCompHistory.from_person_account_id = icdoPersonAccount.from_person_account_id;
            lobjCdoDeffCompHistory.to_person_account_id = icdoPersonAccount.to_person_account_id;
            lobjCdoDeffCompHistory.de_minimus_distribution_flag = icdoPersonAccountDeferredComp.de_minimus_distribution_flag;
            lobjCdoDeffCompHistory.hardship_withdrawal_flag = icdoPersonAccountDeferredComp.hardship_withdrawal_flag;
            lobjCdoDeffCompHistory.hardship_withdrawal_effective_date = icdoPersonAccountDeferredComp.hardship_withdrawal_effective_date;
            lobjCdoDeffCompHistory.limit_457_value = icdoPersonAccountDeferredComp.limit_457_value;
            lobjCdoDeffCompHistory.catch_up_start_date = icdoPersonAccountDeferredComp.catch_up_start_date;
            lobjCdoDeffCompHistory.catch_up_end_date = icdoPersonAccountDeferredComp.catch_up_end_date;
            lobjCdoDeffCompHistory.Insert();
        }

        public void ProcessHistory()
        {
            Collection<busPersonAccountDeferredCompHistory> lclbDeletedDeferredCompHistory = new Collection<busPersonAccountDeferredCompHistory>();
            if ((icdoPersonAccount.status_value == "VALD") && (IsHistoryEntryRequired))
            {
                //Removing Overlapping history
                if (iclbOverlappingHistory != null && iclbOverlappingHistory.Count > 0)
                {
                    foreach (busPersonAccountDeferredCompHistory lobjHistory in iclbOverlappingHistory)
                    {
                        lclbDeletedDeferredCompHistory.Add(lobjHistory);
                        lobjHistory.icdoPersonAccountDeferredCompHistory.Delete();
                    }
                    //PIR 23340
                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedDepCompOtherPlan))
                    {
                        bool lblnIsPersonAccountModified = false;
                        if (icdoPersonAccount.start_date > icdoPersonAccount.history_change_date)
                        {
                            icdoPersonAccount.start_date = icdoPersonAccount.history_change_date;
                            lblnIsPersonAccountModified = true;

                        }
                        if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled &&
                            icdoPersonAccount.end_date != DateTime.MinValue)
                        {
                            icdoPersonAccount.end_date = DateTime.MinValue;
                            lblnIsPersonAccountModified = true;
                        }

                        if (lblnIsPersonAccountModified)
                            icdoPersonAccount.Update();
                    }
                }
                
                //if (_ibusHistory == null)
                    LoadPreviousHistory();

                //If the Current Record is Getting End Dated, We should not create New History Entry. 
                //We Just need to Update the Previous History Entry

                //If the History is already End Dated and the New Record is now removing End Date, Then 
                //We should not update the Previous History End Date. We Just need to Create the New History Record Only.
                if (//(ibusHistory.icdoPersonAccountDeferredCompHistory.end_date == DateTime.MinValue) &&
                    (ibusHistory.icdoPersonAccountDeferredCompHistory.person_account_deferred_comp_history_id > 0))
                {
                    if (!lclbDeletedDeferredCompHistory.Any(i => i.icdoPersonAccountDeferredCompHistory.person_account_deferred_comp_history_id == ibusHistory.icdoPersonAccountDeferredCompHistory.person_account_deferred_comp_history_id))
                    {

                        if (ibusHistory.icdoPersonAccountDeferredCompHistory.start_date == icdoPersonAccount.history_change_date)
                        {
                            ibusHistory.icdoPersonAccountDeferredCompHistory.end_date = icdoPersonAccount.history_change_date;
                        }
                        else
                        {
                            ibusHistory.icdoPersonAccountDeferredCompHistory.end_date = icdoPersonAccount.history_change_date.AddDays(-1);
                        }
                        ibusHistory.icdoPersonAccountDeferredCompHistory.Update();

                        //PIR 1729 : Update the End Date with Previous History End Date if the Plan Participation Status Retired or WithDrawn
                        //Otherwise Reset the End Date with Blank Value
                        if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDeffCompWithDrawn)
                        {
                            icdoPersonAccount.end_date = ibusHistory.icdoPersonAccountDeferredCompHistory.end_date;
                        }
                        else
                        {
                            icdoPersonAccount.end_date = DateTime.MinValue;
                        }
                        icdoPersonAccount.Update();
                    }
                }

                //Always Insert New History Whenever HistoryEntry Required Flag is Set
                InsertHistory();
            }
        }

        public void LoadPreviousHistory()
        {
            if (_ibusHistory == null)
            {
                _ibusHistory = new busPersonAccountDeferredCompHistory();
                _ibusHistory.icdoPersonAccountDeferredCompHistory = new cdoPersonAccountDeferredCompHistory();
            }

            DataTable ldtbHistory = Select("cdoPersonAccountDeferredComp.LatestHistoryRecord", new object[1] { icdoPersonAccount.person_account_id });
            if (ldtbHistory.Rows.Count > 0)
            {
                _ibusHistory.icdoPersonAccountDeferredCompHistory.LoadData(ldtbHistory.Rows[0]);
            }
        }

        private void SetHistoryEntryRequiredOrNot()
        {
            if (_ibusHistory == null)
                LoadPreviousHistory();

            if ((icdoPersonAccount.plan_participation_status_value != _ibusHistory.icdoPersonAccountDeferredCompHistory.plan_participation_status_value) ||
             (icdoPersonAccount.current_plan_start_date != _ibusHistory.icdoPersonAccountDeferredCompHistory.start_date) ||
             (icdoPersonAccountDeferredComp.catch_up_start_date != _ibusHistory.icdoPersonAccountDeferredCompHistory.catch_up_start_date) ||
             (icdoPersonAccountDeferredComp.catch_up_end_date != _ibusHistory.icdoPersonAccountDeferredCompHistory.catch_up_end_date) ||
             (icdoPersonAccountDeferredComp.de_minimus_distribution_flag != _ibusHistory.icdoPersonAccountDeferredCompHistory.de_minimus_distribution_flag) ||
             (icdoPersonAccountDeferredComp.hardship_withdrawal_effective_date != _ibusHistory.icdoPersonAccountDeferredCompHistory.hardship_withdrawal_effective_date) ||
             (icdoPersonAccountDeferredComp.limit_457_value != _ibusHistory.icdoPersonAccountDeferredCompHistory.limit_457_value) ||
             (icdoPersonAccountDeferredComp.hardship_withdrawal_flag != _ibusHistory.icdoPersonAccountDeferredCompHistory.hardship_withdrawal_flag))
            {
                IsHistoryEntryRequired = true;
            }
            else
            {
                IsHistoryEntryRequired = false;
            }
        }

        #region Used in Approaching 457 Contribution Limit Correspondence

        private decimal _ldcl457ContributionLimit;

        public decimal ldcl457ContributionLimit { get; set; }

        private decimal _ldclTotalContributionAmount;
        public decimal ldclTotalContributionAmount
        {
            get { return _ldclTotalContributionAmount; }
            set { _ldclTotalContributionAmount = value; }
        }

        public DateTime ldtTodaysDate
        {
            get { return DateTime.Now; }
        }

        public string lstrCurrentYear
        {
            get { return DateTime.Now.ToString("yyyy"); }
        }

        #endregion

        //Logic of Date to Get the Employment and Calculating Premium Amount has been changed and the details are mailed on 3/18/2009 after the discussion with RAJ.
        /*************************
         * 1) Member A started the Health Plan on Jan 1981 and the plan is still open.
         *      In this case, System will display the rates as of Today.
         * 2) Member A started the Health Plan on Jan 2000 and Suspended the Plan on May 2009. 
         *      In this case, system will display the rate as of End date of Latest Enrolled Status History Record. (i.e) Apr 2009. 
         * 3) Third Scenario (Future Date Scenario) might be little bit complicated. Let me know your feedback too. 
         *    If the Member starts the plan on Jan 2000 with the Single Coverage and May 2009 he wants to change to Family.
         *      Current Date is Mar 18 2009. But the latest enrolled history record is future date. 
         *      So System will display the rate as of Start Date of Latest Enrolled History Date. (i.e) of May 2009
         * *************************/

        public void LoadPlanEffectiveDate()
        {
            idtPlanEffectiveDate = DateTime.Now;

            //If the Current Participation status is enrolled, Set the Effective Date from History Change Date
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled)
            {
                if (icdoPersonAccount.current_plan_start_date_no_null > DateTime.Now)
                    idtPlanEffectiveDate = icdoPersonAccount.current_plan_start_date_no_null;
                else
                    idtPlanEffectiveDate = DateTime.Now;
            }
            else
            {
                if (icolPADeferredCompHistory == null)
                    LoadPADeffCompHistory();

                //By Default the Collection sorted by latest date
                foreach (busPersonAccountDeferredCompHistory lbusPersonAccountDeferredCompHistory in icolPADeferredCompHistory)
                {
                    if (lbusPersonAccountDeferredCompHistory.icdoPersonAccountDeferredCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled)
                    {
                        if (lbusPersonAccountDeferredCompHistory.icdoPersonAccountDeferredCompHistory.end_date == DateTime.MinValue)
                        {
                            //If the Start Date is Future Date, Set it otherwise Current Date will be Start Date of Premium Calc
                            if (lbusPersonAccountDeferredCompHistory.icdoPersonAccountDeferredCompHistory.start_date > DateTime.Now)
                            {
                                idtPlanEffectiveDate = lbusPersonAccountDeferredCompHistory.icdoPersonAccountDeferredCompHistory.start_date;
                            }
                            else
                            {
                                idtPlanEffectiveDate = DateTime.Now;
                            }
                        }
                        else
                        {
                            idtPlanEffectiveDate = lbusPersonAccountDeferredCompHistory.icdoPersonAccountDeferredCompHistory.end_date;
                        }
                        break;
                    }
                }
            }
        }

        # region UCS-041

        //UCS-041 Load def comp transfer details        
        public void LoadDefCompTransferDetails()
        {
            DataTable ldtbList = Select("cdoPersonAccountDeferredComp.LoadDefCompTransferDetails", new object[1] { icdoPersonAccount.person_account_id });
            iclbPersonAccountDeferredCompTransfer = new Collection<busPersonAccountDeferredCompTransfer>();
            foreach (DataRow dr in ldtbList.Rows)
            {
                busPersonAccountDeferredCompTransfer lobjDefCompTransfer = new busPersonAccountDeferredCompTransfer
                {
                    icdoPersonAccountDeferredCompTransfer = new cdoPersonAccountDeferredCompTransfer()
                };
                lobjDefCompTransfer.icdoPersonAccountDeferredCompTransfer.LoadData(dr);
                lobjDefCompTransfer.LoadTransferedAmount();
                iclbPersonAccountDeferredCompTransfer.Add(lobjDefCompTransfer);
            }
        }


        # endregion

        public override void LoadCorresProperties(string astrTemplateName)
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();

            SetIsCatchUpDateOverlapsCurrentDateFlag();
        }
        //load person account
        public busPersonAccount ibusPersonAccount { get; set; }
        public void LoadPersonAccount()
        {
            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };

            ibusPersonAccount.icdoPersonAccount = this.icdoPersonAccount;
        }
        public cdoWssPersonAccountEnrollmentRequest icdoWssPersonAccountEnrollmentRequest { get; set; }
        
        public override busBase GetCorOrganization()
        {
            if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusPersonEmploymentDetail.LoadPersonEmployment();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.IsNull()) ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();
            return ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization;
        }

        // UAT PIR ID 1201
        public decimal idecAnnualLimitAmount
        {
            get
            {
                //return Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(334, busConstant.PersonAccount457LimitRegular, iobjPassInfo));
                //pir 8915
                return GetIRSLimitAmount(busConstant.PersonAccount457LimitRegular, DateTime.Now);
            }
        }

        // UAT PIR ID 1201
        public decimal idec50PlusLimitAmount
        {
            get
            {
                //return Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(334, busConstant.PersonAccount457Limit50, iobjPassInfo));
                //pir 8915
                return GetIRSLimitAmount(busConstant.PersonAccount457Limit50, DateTime.Now);
            }
        }

        //PIR 8915
        public decimal GetIRSLimitAmount(string aLimit457Value, DateTime adtEffectiveDate)
        {
            decimal lLimitAmount = 0;
            DataTable ldtIRSLimitAmount = Select("cdoPersonAccount457LimitRef.GetIRSLimit", new object[2] { adtEffectiveDate, aLimit457Value });
            if (ldtIRSLimitAmount.Rows.Count > 0)
            {
                cdoPersonAccount457LimitRef lcdoPersonAccount457LimitRef = new cdoPersonAccount457LimitRef();
                lcdoPersonAccount457LimitRef.LoadData(ldtIRSLimitAmount.Rows[0]);
                lLimitAmount = lcdoPersonAccount457LimitRef.amount;
            }
            return lLimitAmount;
        }

        /// UAT PIR ID 1285
        public bool IsRTWPayeeStatusReceiving()
        {
            if (ibusPerson.IsNull()) LoadPerson();
            if ((ibusPerson.IsRTWMember()) &&
                (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled))
            {
                if (icolPersonAccountDeferredCompProvider.IsNotNull())
                {
                    if (icolPersonAccountDeferredCompProvider.Where(lobj =>
                        lobj.icdoPersonAccountDeferredCompProvider.payment_status_value == busConstant.DeferredPaymentStatusReceiving).Any())
                        return true;
                }
            }
            return false;
        }

        # region UCS 24

        public decimal idecLimit457
        {
            get
            {
                if (icdoPersonAccountDeferredComp.limit_457_value == busConstant.PersonAccount457Limit50)
                    //return Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(334, busConstant.PersonAccount457Limit50, iobjPassInfo));
                    //PIR 8915
                    return GetIRSLimitAmount(busConstant.PersonAccount457Limit50, DateTime.Now);
                else if (icdoPersonAccountDeferredComp.limit_457_value == busConstant.PersonAccount457LimitCatchUp)
                    //return Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(334, busConstant.PersonAccount457LimitCatchUp, iobjPassInfo));
                    //PIR 8915
                    return GetIRSLimitAmount(busConstant.PersonAccount457LimitCatchUp, DateTime.Now);
                else if (icdoPersonAccountDeferredComp.limit_457_value == busConstant.PersonAccount457LimitRegular)
                    //return Convert.ToDecimal(busGlobalFunctions.GetData1ByCodeValue(334, busConstant.PersonAccount457LimitRegular, iobjPassInfo));
                    //PIR 8915
                    return GetIRSLimitAmount(busConstant.PersonAccount457LimitRegular, DateTime.Now);
                else
                    return 0.00M;
            }
        }
        public bool iblnIsFromPortal { get; set; }
        #endregion


        //UAT PIR 1626
        //for cor ENR-5404
        public string istrIsCatchUpDateOverlapsCurrentDate { get; set; }

        private void SetIsCatchUpDateOverlapsCurrentDateFlag()
        {
            if (icdoPersonAccountDeferredComp.catch_up_end_date != DateTime.MinValue)
            {
                istrIsCatchUpDateOverlapsCurrentDate = busConstant.Flag_No;
                if (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccountDeferredComp.catch_up_end_date, DateTime.Now, DateTime.Now.AddDays(60)))
                    istrIsCatchUpDateOverlapsCurrentDate = busConstant.Flag_Yes;
            }
        }

        public int iintCurrentYearPlus1Yr
        {
            get
            {
                return DateTime.Now.AddYears(1).Year;
            }
        }


        public DateTime idtePayrollDate { get; set; }

        private Collection<busPersonAccountDeferredCompHistory> LoadOverlappingHistory()
        {
            if (icolPADeferredCompHistory == null)
                LoadPADeffCompHistory();
            Collection<busPersonAccountDeferredCompHistory> lclbPADeffCompHistory = new Collection<busPersonAccountDeferredCompHistory>();
            var lenuList = icolPADeferredCompHistory.Where(i => busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                i.icdoPersonAccountDeferredCompHistory.start_date, i.icdoPersonAccountDeferredCompHistory.end_date)
                || i.icdoPersonAccountDeferredCompHistory.start_date > icdoPersonAccount.history_change_date);
            foreach (busPersonAccountDeferredCompHistory lobjHistory in lenuList)
            {
                if (lobjHistory.icdoPersonAccountDeferredCompHistory.start_date >= icdoPersonAccount.history_change_date)
                {
                    lclbPADeffCompHistory.Add(lobjHistory);
                }
                else if (lobjHistory.icdoPersonAccountDeferredCompHistory.start_date == lobjHistory.icdoPersonAccountDeferredCompHistory.end_date)
                {
                    lclbPADeffCompHistory.Add(lobjHistory);
                }
                else if (lobjHistory.icdoPersonAccountDeferredCompHistory.start_date != lobjHistory.icdoPersonAccountDeferredCompHistory.end_date)
                {
                    break;
                }

            }
            return lclbPADeffCompHistory;
        }

        public bool IsMoreThanOneEnrolledInOverlapHistory()
        {
            if (istrAllowOverlapHistory == busConstant.Flag_Yes)
            {
                if (iclbOverlappingHistory != null)
                {
                    var lenuList = iclbOverlappingHistory.Where(i => i.icdoPersonAccountDeferredCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompEnrolled &&
                        i.icdoPersonAccountDeferredCompHistory.start_date != i.icdoPersonAccountDeferredCompHistory.end_date);
                    //PIR Enhanced Overlap - 23167, 23340, 23408
                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedDepCompOtherPlan))
                    {
                        if ((lenuList != null) && (lenuList.Count() > 2))
                            return true;
                    }
                    else
                    {
                        if ((lenuList != null) && (lenuList.Count() > 1))
                            return true;
                    }
                }
            }
            return false;
        }

        public void LoadActivePersonAccountProvidersByPayPeriodStartDate(DateTime adtPayPeriodStartDate)
        {
            iclbActiveProvidersByPayPeriodStartDate = new Collection<busPersonAccountDeferredCompProvider>();

            DataTable ldtbList = Select("cdoPersonAccountDeferredCompProvider.LoadActiveProvidersByPayPeriodStartDate",
                    new object[2] { icdoPersonAccountDeferredComp.person_account_id, adtPayPeriodStartDate });

            iclbActiveProvidersByPayPeriodStartDate = GetCollection<busPersonAccountDeferredCompProvider>(ldtbList, "icdoPersonAccountDeferredCompProvider");

        }

        public Collection<busPersonAccountDeferredCompProvider> iclbActiveProvidersByPayPeriodStartDate { get; set; }

        //PIR 17081
        //Check if any Def Comp provider is not end dated before suspending the plan.
        public bool IsDeferredCompProviderEnded()
        {
            if (icolPersonAccountDeferredCompProvider.IsNull())
                LoadPersonAccountProviders();

            return (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusDefCompSuspended &&
                icolPersonAccountDeferredCompProvider.Any(i => i.icdoPersonAccountDeferredCompProvider.end_date == DateTime.MinValue));
        }
        public string istrEligibilityCodeForFile
        {
            get
            {
                return " ";
            }
        }
        public DateTime idtmReHireDateForFile
        {
            get
            {
                return DateTime.MinValue;
            }
        }
        public DateTime idtmCheckDateForFile
        {
            get
            {
                return DateTime.Today;
            }
        }
        /// <summary>
        /// return if DC25 plan is Employment Detail in election 
        /// </summary>
        /// <returns></returns>
        public bool IsEmploymentLinkedWithDC25Plan
        {
            get
            {
                busWssPersonAccountEnrollmentRequest lbusWssPersonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest { icdoWssPersonAccountEnrollmentRequest = new cdoWssPersonAccountEnrollmentRequest() };
                //lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_employment_dtl_id = icdoPersonAccount.person_employment_dtl_id;
                lbusWssPersonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.person_id = icdoPersonAccount.person_id;
                return lbusWssPersonAccountEnrollmentRequest.IsEmploymentLinkedWithDC25Plan;
            }
        }
        public bool IsEmployerMatchAvailable
        {
            get
            {
                if (ibusPersonAccount.IsNull())
                    LoadPersonAccount();
                return ibusPersonAccount.IsEmployerMatchAvailableWithElection(icdoPersonAccount.history_change_date);
                //return IsEmploymentLinkedWithDC25Plan && GetPersonRetirementOptionalValue() < GetMaxERPreTaxMatch();
            }
        }
        
    }
}
