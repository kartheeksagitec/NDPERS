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
using NeoSpin.DataObjects;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccount : busPersonAccountGen
    {
		//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
        public Collection<busIbsDetail> icolPosNegIbsDetail { get; set; }
        public Collection<busEmployerPayrollDetail> icolPosNegEmployerPayrollDtl { get; set; }
        // UAT PIR ID 1045
        public bool iblnIsNewMode { get; set; }
        public bool iblnIsFromESS { get; set; }
        public bool iblnIsFromTerminationPost { get; set; }
        public bool iblnIsEnrollmentValidationApplicable { get; set; }
        private busPersonAccountPaymentElection _ibusPaymentElection;
        public busPersonAccountPaymentElection ibusPaymentElection
        {
            get { return _ibusPaymentElection; }
            set { _ibusPaymentElection = value; }
        }
        private Collection<busPersonEmploymentDetail> _iclbEmploymentDetail;
        public Collection<busPersonEmploymentDetail> iclbEmploymentDetail
        {
            get { return _iclbEmploymentDetail; }
            set { _iclbEmploymentDetail = value; }
        }
        public Collection<busPersonEmploymentDetail> iclbAllPersonEmploymentDetails { get; set; }
        public busWssMemberRecordRequest ibusMemberRecordRequest { get; set; }
        //PIR 25920 DC 2025 changes
        public busHb1040Communication ibusHb1040Communication { get; set; }
        public void LoadHb1040Communication()
        {
            ibusHb1040Communication = new busHb1040Communication();
            ibusHb1040Communication.FindHB1040Communication(icdoPersonAccount.person_account_id);
        }
        //PIR-15900 Added Method to get persons all Employment Details
        public void LoadAllPersonEmploymentDtls()
        {
            iclbAllPersonEmploymentDetails = new Collection<busPersonEmploymentDetail>();
            DataTable ldtbEmpDetail = Select("cdoPersonAccountRetirement.LoadEmploymentDetails",
                                                          new object[1] { icdoPersonAccount.person_id });
            foreach (DataRow ldtrempdetails in ldtbEmpDetail.Rows)
            {
                busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtrempdetails);
                iclbAllPersonEmploymentDetails.Add(lobjPersonEmploymentDetail);
            }
        }

        private DateTime _idtPlanEffectiveDate;
        public DateTime idtPlanEffectiveDate
        {
            get
            {
                return _idtPlanEffectiveDate;
            }
            set
            {
                _idtPlanEffectiveDate = value;
            }
        }
        //to load provider name in MSS Retiree Homepage
        public string istrProviderOrgName { get; set; }
        //To add bank details in benefit plan information grid
        public string istrBankName { get; set; }
        public DateTime idtACHStartDate { get; set; }
        public string istrAccountType { get; set; }
        public string istrAccountNumber { get; set; }
        //UAT PIR: 1014
        #region Member Plan Summary Properties
        private decimal _idecMemberPlanSummaryCurrentPremiumAmt;

        public decimal idecMemberPlanSummaryCurrentPremiumAmt
        {
            get { return _idecMemberPlanSummaryCurrentPremiumAmt; }
            set { _idecMemberPlanSummaryCurrentPremiumAmt = value; }
        }

        private decimal _idecMemberPlanSummaryNetPremiumAmt;

        public decimal idecMemberPlanSummaryNetPremiumAmt
        {
            get { return _idecMemberPlanSummaryNetPremiumAmt; }
            set { _idecMemberPlanSummaryNetPremiumAmt = value; }
        }
        private decimal _idecMemberPlanSummaryRhicAmt;

        public decimal idecMemberPlanSummaryRhicAmt
        {
            get { return _idecMemberPlanSummaryRhicAmt; }
            set { _idecMemberPlanSummaryRhicAmt = value; }
        }
        private decimal _idecMemberPlanSummaryJSRhicAmount;

        public decimal idecMemberPlanSummaryJSRhicAmount
        {
            get { return _idecMemberPlanSummaryJSRhicAmount; }
            set { _idecMemberPlanSummaryJSRhicAmount = value; }
        }
        private decimal _idecMemberPlanSummaryBalanceForward;

        public decimal idecMemberPlanSummaryBalanceForward
        {
            get { return _idecMemberPlanSummaryBalanceForward; }
            set { _idecMemberPlanSummaryBalanceForward = value; }
        }
        private decimal _idecMemberPlanSummaryTotalDueAmt;

        public decimal idecMemberPlanSummaryTotalDueAmt
        {
            get { return _idecMemberPlanSummaryTotalDueAmt; }
            set { _idecMemberPlanSummaryTotalDueAmt = value; }
        }
        private DateTime _idtMemberPlanSummaryEffectiveDate;

        public DateTime idtMemberPlanSummaryEffectiveDate
        {
            get { return _idtMemberPlanSummaryEffectiveDate; }
            set { _idtMemberPlanSummaryEffectiveDate = value; }
        }
        #endregion
        public DateTime idtHealthParticipationDate { get; set; }
        //This Property is used in NewAchDetail button visiblity under Payment Election election tab in All insurance plan screens.
        //UCS -071 .BR - 70,71
        public bool IsAchApplicable
        {
            get
            {
                int lintActiveAch = 0;
                if (ibusPaymentElection == null)
                {
                    LoadPaymentElection();
                }
                if (ibusIBSBatchSchedule == null)
                {
                    LoadIBSBatchSchedule();
                }
                if (iclbPersonAccountAchDetail == null)
                {
                    LoadPersonAccountAchDetail();
                }
                foreach (busPersonAccountAchDetail lobjAchDetail in iclbPersonAccountAchDetail)
                {
                    if ((lobjAchDetail.icdoPersonAccountAchDetail.ach_start_date <= ibusIBSBatchSchedule.icdoBatchSchedule.next_run_date)
                        && (lobjAchDetail.icdoPersonAccountAchDetail.ach_end_date == DateTime.MinValue))
                    {
                        lintActiveAch++;
                    }
                }
                if ((lintActiveAch == 0) && (ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentACH))
                {
                    return true;
                }
                return false;
            }
        }
        public int iPayeeAccountForPapit
        {
            get
            {
                int lintReturnValue = 0;
                if (ibusPaymentElection.IsNull()) LoadPaymentElection();
                if (ibusPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0 &&
                    !string.IsNullOrEmpty(ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value) &&
                    ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id > 0)
                {
                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck)
                        lintReturnValue = ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id;
                }
                return lintReturnValue;
            }

        }
        //Backlog PIR 8022 - Is Current PaymentMethod Pension check
        public bool IsCurrentPaymentMethodPensionCheck
        {
            get
            {
                bool lblnChanged = false;
                if (ibusPaymentElection.IsNull()) LoadPaymentElection();
                if (ibusPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0 &&
                    !string.IsNullOrEmpty(ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value) &&
                    ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id > 0)
                {
                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck)
                        lblnChanged = true;
                }
                return lblnChanged;
            }

        }



        // Retirement Contributions
        public Collection<busPersonAccountRetirementContribution> _iclbRetirementContributionAll;
        public Collection<busPersonAccountRetirementContribution> iclbRetirementContributionAll
        {
            get
            {
                return _iclbRetirementContributionAll;
            }
            set
            {
                _iclbRetirementContributionAll = value;
            }
        }

        // this is used in UCS - 51 and 53 to find only the contribution which are less than given date
        public Collection<busPersonAccountRetirementContribution> _iclbRetirementContributionAllAsOfDate;
        public Collection<busPersonAccountRetirementContribution> iclbRetirementContributionAllAsOfDate
        {
            get
            {
                return _iclbRetirementContributionAllAsOfDate;
            }
            set
            {
                _iclbRetirementContributionAllAsOfDate = value;
            }
        }

        private Collection<busPersonAccountBeneficiary> _iclbPersonAccountBeneficiary;
        public Collection<busPersonAccountBeneficiary> iclbPersonAccountBeneficiary
        {
            get { return _iclbPersonAccountBeneficiary; }
            set { _iclbPersonAccountBeneficiary = value; }
        }
        //For MSS Layout change
        public decimal idecMSSMonthlyPremiumAmount { get; set; }

        public void LoadPersonAccountBeneficiary()
        {
            DataTable ldtbList = Select<cdoPersonAccountBeneficiary>(
                                      new string[1] { "person_account_id" },
                                      new object[1] { icdoPersonAccount.person_account_id }, null, null);
            iclbPersonAccountBeneficiary = GetCollection<busPersonAccountBeneficiary>(ldtbList, "icdoPersonAccountBeneficiary");
        }

        public DataTable idtRetirementContributionAll { get; set; }
        private void LoadRetirementContributionAllDataTable()
        {
            if (iblnLoadOnlyRequiredFieldsForVSCOrPSC) //PIR 18567
                idtRetirementContributionAll = Select("cdoPersonAccountRetirementContribution.LoadRetirementContributionAllDataTable", new object[1]
                                                                                    {
                                                                                    icdoPersonAccount.person_account_id
                                                                                    });
            else
                idtRetirementContributionAll = Select<cdoPersonAccountRetirementContribution>(new string[1] { "person_account_id" },
                                                                                    new object[1]
                                                                                        {
                                                                                        icdoPersonAccount.person_account_id
                                                                                        }, null, null);

            if (idtMASStatementEffectiveDate != DateTime.MinValue)
                idtRetirementContributionAll = idtRetirementContributionAll.AsEnumerable().Where(o => o.Field<DateTime?>("effective_date") <= idtMASStatementEffectiveDate).AsDataTable();
        }

        public void LoadRetirementContributionAll()
        {
            if (idtRetirementContributionAll == null)
                LoadRetirementContributionAllDataTable();
            _iclbRetirementContributionAll = GetCollection<busPersonAccountRetirementContribution>(idtRetirementContributionAll, "icdoPersonAccountRetirementContribution");
        }

        public void LoadRetirementContributionByDate(string astrSortOrder, DateTime adtCompareDate, bool ablnIsDROEstimate = false)
        {
            if (idtRetirementContributionAll == null)
                LoadRetirementContributionAllDataTable();

            var lclbPARetContribution = GetCollection<busPersonAccountRetirementContribution>(idtRetirementContributionAll, "icdoPersonAccountRetirementContribution");
            if (astrSortOrder.IsNotNullOrEmpty())
            {
                lclbPARetContribution = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>(astrSortOrder, lclbPARetContribution);
            }

            iclbRetirementContributionAllAsOfDate = new Collection<busPersonAccountRetirementContribution>();
            if (adtCompareDate != DateTime.MinValue)
            {
                foreach (busPersonAccountRetirementContribution lobjRetContribution in lclbPARetContribution)
                {
                    if (ablnIsDROEstimate) // PROD PIR ID 1414
                    {
                        if (lobjRetContribution.icdoPersonAccountRetirementContribution.effective_date <= adtCompareDate)
                            _iclbRetirementContributionAllAsOfDate.Add(lobjRetContribution);
                    }
                    else
                    {
                        if (lobjRetContribution.icdoPersonAccountRetirementContribution.effective_date < adtCompareDate)
                            _iclbRetirementContributionAllAsOfDate.Add(lobjRetContribution);
                    }
                }
            }
            else
            {
                _iclbRetirementContributionAllAsOfDate = lclbPARetContribution;
            }
        }
        
        // Insurance Contributions
        public Collection<busPersonAccountInsuranceContribution> _iclbInsuranceContributionAll;
        public Collection<busPersonAccountInsuranceContribution> iclbInsuranceContributionAll
        {
            get
            {
                return _iclbInsuranceContributionAll;
            }
            set
            {
                _iclbInsuranceContributionAll = value;
            }
        }
        public void LoadInsuranceContributionAll()
        {
            //PROD PIR 5831 : need to sort the collection based on health insurance contribution id DESC
            DataTable ldtbList = Select<cdoPersonAccountInsuranceContribution>(
                new string[1] { "person_account_id" },
                new object[1] { icdoPersonAccount.person_account_id }, null, "HEALTH_INSURANCE_CONTRIBUTION_ID DESC");
            _iclbInsuranceContributionAll = GetCollection<busPersonAccountInsuranceContribution>(ldtbList, "icdoPersonAccountInsuranceContribution");
            foreach (busPersonAccountInsuranceContribution lbusInsuranceContribution in _iclbInsuranceContributionAll)
            {
                lbusInsuranceContribution.ibusPersonAccount = this;
            }
        }
        public Collection<busPersonAccountInsuranceContribution> _iclbInsuranceContribution;
        public Collection<busPersonAccountInsuranceContribution> iclbInsuranceContribution
        {
            get
            {
                return _iclbInsuranceContribution;
            }
            set
            {
                _iclbInsuranceContribution = value;
            }
        }

        private decimal _idecPaidAmount;
        public decimal idecPaidAmount
        {
            get { return _idecPaidAmount; }
            set { _idecPaidAmount = value; }
        }
        private decimal _idecPremiumDue;
        public decimal idecPremiumDue
        {
            get { return _idecPremiumDue; }
            set { _idecPremiumDue = value; }
        }
        //Collection which contain the original Retirement contribution for a given person account id
        private Collection<busPersonAccountInsuranceContribution> _iclbOriginalInsuranceContribution;
        public Collection<busPersonAccountInsuranceContribution> iclbOriginalInsuranceContribution
        {
            get { return _iclbOriginalInsuranceContribution; }
            set { _iclbOriginalInsuranceContribution = value; }
        }
        public void LoadInsuranceLTD()
        {
            DataTable ldtbList = Select("cdoPersonAccount.LoadInsuranceLTD", new object[1] { icdoPersonAccount.person_account_id });
            iclbInsuranceContribution = GetCollection<busPersonAccountInsuranceContribution>(ldtbList, "icdoPersonAccountInsuranceContribution");
            foreach (busPersonAccountInsuranceContribution lobjcontribution in iclbInsuranceContribution)
            {
                idecPaidAmount += lobjcontribution.icdoPersonAccountInsuranceContribution.paid_premium_amount;
                idecPremiumDue += lobjcontribution.icdoPersonAccountInsuranceContribution.due_premium_amount;
            }
            iclbOriginalInsuranceContribution = iclbInsuranceContribution;
        }
        public void LoadInsurancePremium(DateTime adtEffectivedate)
        {
            DataTable ldtbList = Select("cdoPersonAccountInsuranceContribution.CheckDueAmountForGivenEffectiveDate",
                                                                        new object[2] { icdoPersonAccount.person_account_id, adtEffectivedate });
            iclbInsuranceContribution = GetCollection<busPersonAccountInsuranceContribution>(ldtbList, "icdoPersonAccountInsuranceContribution");
            foreach (busPersonAccountInsuranceContribution lobjcontribution in iclbInsuranceContribution)
            {
                idecPaidAmount += lobjcontribution.icdoPersonAccountInsuranceContribution.paid_premium_amount;
                idecPremiumDue += lobjcontribution.icdoPersonAccountInsuranceContribution.due_premium_amount;
            }
        }

        public Collection<busPersonAccountDeferredCompContribution> _iclbDefCompContributionAll;
        public Collection<busPersonAccountDeferredCompContribution> iclbDefCompContributionAll
        {
            get
            {
                return _iclbDefCompContributionAll;
            }
            set
            {
                _iclbDefCompContributionAll = value;
            }
        }
        public void LoadDefCompContributionAll()
        {
            DataTable ldtbList = Select<cdoPersonAccountDeferredCompContribution>(
                new string[1] { "person_account_id" },
                new object[1] { icdoPersonAccount.person_account_id }, null, null);
            _iclbDefCompContributionAll = GetCollection<busPersonAccountDeferredCompContribution>(ldtbList, "icdoPersonAccountDeferredCompContribution");
            foreach (busPersonAccountDeferredCompContribution lbusPersonAccountDeferredCompContribution in _iclbDefCompContributionAll)
            {
                lbusPersonAccountDeferredCompContribution.ibusPADeferredComp = new busPersonAccountDeferredComp
                {
                    icdoPersonAccount = this.icdoPersonAccount,
                };
            }
        }
        // Filling the Payment Election Object.
        public void LoadPaymentElection()
        {
            if (_ibusPaymentElection == null)
            {
                _ibusPaymentElection = new busPersonAccountPaymentElection();
                _ibusPaymentElection.icdoPersonAccountPaymentElection = new cdoPersonAccountPaymentElection();
            }
            DataTable ldtbPaymentElection = Select<cdoPersonAccountPaymentElection>(new string[1] { "person_account_id" },
                                            new object[1] { icdoPersonAccount.person_account_id }, null, null);
            if (ldtbPaymentElection.Rows.Count > 0)
                _ibusPaymentElection.icdoPersonAccountPaymentElection.LoadData(ldtbPaymentElection.Rows[0]);
        }

		//PIR 12737 & 8565 -- added payment election history
        public Collection<busPersonAccountPaymentElectionHistory> iclbPersonAccountPaymentElectionHistory { get; set; }
        public void LoadPaymentElectionHistory()
        {
            DataTable ldtbPaymentElectionHistory = Select<cdoPersonAccountPaymentElectionHistory>(new string[1] { "person_account_id" },
                                            new object[1] { icdoPersonAccount.person_account_id }, null, "ACCOUNT_PAYMENT_ELECTION_HISTORY_ID DESC");
            iclbPersonAccountPaymentElectionHistory = GetCollection<busPersonAccountPaymentElectionHistory>(ldtbPaymentElectionHistory, "icdoPersonAccountPaymentElectionHistory");

            foreach (busPersonAccountPaymentElectionHistory lbusPersonAccountPaymentElectionHistory in iclbPersonAccountPaymentElectionHistory)
            {
                if (lbusPersonAccountPaymentElectionHistory.icdoPersonAccountPaymentElectionHistory.ibs_org_id > 0)
                {
                    lbusPersonAccountPaymentElectionHistory.icdoPersonAccountPaymentElectionHistory.Billing_Organization =
                        busGlobalFunctions.GetOrgCodeFromOrgId(lbusPersonAccountPaymentElectionHistory.icdoPersonAccountPaymentElectionHistory.ibs_org_id);
                }

                if (lbusPersonAccountPaymentElectionHistory.icdoPersonAccountPaymentElectionHistory.ibs_supplemental_org_id > 0)
                {
                    lbusPersonAccountPaymentElectionHistory.icdoPersonAccountPaymentElectionHistory.Supplemental_Billing_Organization =
                        busGlobalFunctions.GetOrgCodeFromOrgId(lbusPersonAccountPaymentElectionHistory.icdoPersonAccountPaymentElectionHistory.ibs_supplemental_org_id);
                }
            }
        }

        // Assign Billing Org Code ID in Payment Election.
        public void LoadBillingOrganization()
        {
            if (_ibusPaymentElection == null)
                LoadPaymentElection();

            if (_ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id > 0)
            {
                _ibusPaymentElection.icdoPersonAccountPaymentElection.Billing_Organization =
                    busGlobalFunctions.GetOrgCodeFromOrgId(_ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id);
            }

            if (_ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id > 0)
            {
                _ibusPaymentElection.icdoPersonAccountPaymentElection.Supplemental_Billing_Organization =
                    busGlobalFunctions.GetOrgCodeFromOrgId(_ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id);
            }
        }

        // Load Premium YTD
        public void LoadInsuranceYTD()
        {
            DateTime CYTDStartDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime CYTDEndDDate = new DateTime(DateTime.Now.Year, 12, 31);
            DataTable ldtbList = Select("cdoPersonAccount.LoadInsuranceYTD",
                    new object[3] { CYTDStartDate, CYTDEndDDate, icdoPersonAccount.person_account_id });
            iclbInsuranceContribution = GetCollection<busPersonAccountInsuranceContribution>(ldtbList, "icdoPersonAccountInsuranceContribution");
        }

        //Fill the Plan Participation Status DropDownList Based on the Benefit Type
        public Collection<cdoCodeValue> LoadParticipationStatusByBenefitType()
        {
            //UAT PIR 2042
            string lstrData3Filter = null;
            if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
                lstrData3Filter = busConstant.Flag_Yes;
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(337, ibusPlan.icdoPlan.benefit_type_value, null, lstrData3Filter);
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
			//PIR 14656 - start - Is there a way we can limit when this code value is available for a user to choose? 
            Collection<cdoCodeValue> lclcRetrCodeValue = null;
            if (ibusPlan.icdoPlan.benefit_type_value == busConstant.PlanBenefitTypeRetirement)
            {
                lclcRetrCodeValue = new Collection<cdoCodeValue>();
                foreach (cdoCodeValue lcdoCodeValue in lclcCodeValue)
                {
                    if (lcdoCodeValue.start_date != DateTime.MinValue && lcdoCodeValue.end_date != DateTime.MinValue)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(DateTime.Now,
                        lcdoCodeValue.start_date, lcdoCodeValue.end_date))
                        {
                            lclcRetrCodeValue.Add(lcdoCodeValue);
                        }

                    }
                    else
                    {
                        lclcRetrCodeValue.Add(lcdoCodeValue);
                    }
                }
                return lclcRetrCodeValue;
            }
			//PIR 14656 - end - Is there a way we can limit when this code value is available for a user to choose? 
            return lclcCodeValue;
        }

        public Collection<cdoCodeValue> LoadLevelOfCoverageByPlan()
        {
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(408, ibusPlan.icdoPlan.plan_id.ToString(), null, null);
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            return lclcCodeValue;
        }

        public Collection<cdoCodeValue> LoadHealthInsuranceTypeByPlan()
        {
            DataTable ldtbList = iobjPassInfo.isrvDBCache.GetCodeValues(345, ibusPlan.icdoPlan.plan_id.ToString(), null, null);
            Collection<cdoCodeValue> lclcCodeValue = Sagitec.DataObjects.doBase.GetCollection<cdoCodeValue>(ldtbList);
            return lclcCodeValue;
        }

        public void LoadAllPersonEmploymentDetails(bool ablnLoadOtherObjects)
        {
            iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>();
            DataTable ldtbEmpDetail = Select("cdoPersonAccountRetirement.LoadEmploymentDetail",
                                                          new object[1] { icdoPersonAccount.person_account_id });
            iclbEmploymentDetail = new Collection<busPersonEmploymentDetail>();
            foreach (DataRow ldtrempdetails in ldtbEmpDetail.Rows)
            {
                busPersonEmploymentDetail lobjPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lobjPersonEmploymentDetail.icdoPersonEmploymentDetail.LoadData(ldtrempdetails);
                if (ablnLoadOtherObjects)
                {
                    lobjPersonEmploymentDetail.ibusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                    lobjPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.LoadData(ldtrempdetails);

                    lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization = new busOrganization { icdoOrganization = new cdoOrganization() };
                    lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldtrempdetails);
                    if (!Convert.IsDBNull(ldtrempdetails["ORG_STATUS_VALUE"]))
                    {
                        lobjPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.status_value = ldtrempdetails["ORG_STATUS_VALUE"].ToString();
                    }
                }
                iclbEmploymentDetail.Add(lobjPersonEmploymentDetail);
            }
        }

        public void LoadAllPersonEmploymentDetails()
        {
            LoadAllPersonEmploymentDetails(true);
        }

        // Enrollment Date cannot be earlier than the First day of the following month of Employment Date 
        public bool IsPlanStartDateLessThanFirstDayofFollowingMonthEmpDate()
        {
            if (iclbAccountEmploymentDetail == null)
                LoadPersonAccountEmploymentDetails();

            if ((ibusPersonEmploymentDetail != null) && (ibusPersonEmploymentDetail.ibusPersonEmployment != null))
            {
                DateTime ldtEmploymentStartDate = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date;
                DateTime ldtPlanEnrollmentDate = new DateTime(ldtEmploymentStartDate.AddMonths(1).Year, ldtEmploymentStartDate.AddMonths(1).Month, 1);
                if (icdoPersonAccount.current_plan_start_date != DateTime.MinValue)
                {
                    //PIR 21153 a member to be able to enroll into deferred comp on date hire; hence remove both 475 plans from following condition
                    if ((ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeDental) ||
                       (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeEAP) || (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeGroupHealth) ||
                       (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeGroupLife) || (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeVision) ||
                       (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeFlex))
                    {
                        //Modified from Start Date to History Change Date - UAT PIR 1594
                        if (icdoPersonAccount.current_plan_start_date < ldtPlanEnrollmentDate)
                        {
                            return true;
                        }
                    }
                    //PIR 21153 a member to be able to enroll into deferred comp on date hire; hence remove both 475 plans from above condition and for validation check with emp start date
                    if ((ibusPlan.icdoPlan.plan_code == busConstant.PlanCode457) || (ibusPlan.icdoPlan.plan_code == busConstant.PlanCodeOther457))
                    {
                        //Modified from Start Date to History Change Date - UAT PIR 1594
                        if (icdoPersonAccount.current_plan_start_date != DateTime.MinValue && icdoPersonAccount.current_plan_start_date < ldtEmploymentStartDate)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// UAT PIR ID 1011 - Is Late Enrollment, throw a soft error only once in New mode if the Change reason is not Annual Enrollment
        /// Show Warning message if Plan Participation Start Date is greater than Employment start date + 31 days.
        public bool IsPostEnrollmentDate()
        {
            if ((iblnIsNewMode) &&
                (icdoPersonAccount.person_employment_dtl_id != 0))
            {
                if (ibusPersonEmploymentDetail.IsNull()) LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull()) ibusPersonEmploymentDetail.LoadPersonEmployment();
                DateTime ldtEmploymentStartDate = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date;
                DateTime ldtPlanEnrollmentDate = ldtEmploymentStartDate.AddDays(31);
                if ((icdoPersonAccount.start_date != DateTime.MinValue) && (icdoPersonAccount.start_date > ldtPlanEnrollmentDate))
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsEnrollmentDateFirstDay()
        {
            if (icdoPersonAccount.start_date != DateTime.MinValue)
                if (icdoPersonAccount.start_date.Day == 1)
                    return true;
            return false;
        }

        // Check Person account exists for the start date and end date
        //PIR : 1374 Exclude WithDrawn
        public bool CheckOverlapping()
        {
            if (icdoPersonAccount.start_date != DateTime.MinValue)
            {
                if (ibusPerson == null)
                    LoadPerson();

                if (ibusPerson.icolPersonAccount == null)
                    ibusPerson.LoadPersonAccount(true);

                foreach (var lbusPersonAccount in ibusPerson.icolPersonAccount)
                {
                    if (lbusPersonAccount.icdoPersonAccount.person_account_id != icdoPersonAccount.person_account_id)
                    {
                        bool IsOverrideCheckingForRTWMember = false;

                        IsOverrideCheckingForRTWMember = ibusPerson.IsRTWMember(lbusPersonAccount.icdoPersonAccount.plan_id);

                        if (!IsOverrideCheckingForRTWMember)
                        {
                            if ((lbusPersonAccount.icdoPersonAccount.plan_id == icdoPersonAccount.plan_id) &&
                                (!lbusPersonAccount.IsWithDrawn()))
                            {
                                bool lblnIsDCPlan = false;
                                bool lblnIsDCRetired = false;
                                //If DC Plan, exclude Retired Status too. Refer UCS 24 BR 20 for more details
                                if (lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC ||
                                    lbusPersonAccount.icdoPersonAccount.plan_id == busConstant.PlanIdDC2020) //PIR 20232
                                {
                                    lblnIsDCPlan = true;
                                    if (lbusPersonAccount.IsPlanParticipationStatusRetirementRetired())
                                    {
                                        lblnIsDCRetired = true;
                                    }
                                }

                                if (!lblnIsDCPlan || (lblnIsDCPlan && !lblnIsDCRetired))
                                {
                                    if (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.start_date, icdoPersonAccount.end_date,
                                        lbusPersonAccount.icdoPersonAccount.start_date, lbusPersonAccount.icdoPersonAccount.end_date))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        //  Validate whether Enrolling Person's Age is Greater than 18.
        public bool ValidatePersonEnrollmentAge()
        {
            bool iblnIsAgeLessThan18 = false;
            if (busGlobalFunctions.CalulateAge(ibusPerson.icdoPerson.date_of_birth, icdoPersonAccount.start_date) < 18)
            {
                iblnIsAgeLessThan18 = true;
            }
            return iblnIsAgeLessThan18;
        }

        public override int Delete()
        {
            if (icdoPersonAccount.person_account_id != 0)
            {
                DBFunction.DBNonQuery("cdoPersonAccount.Delete",
                               new object[1] { icdoPersonAccount.person_account_id },
                               iobjPassInfo.iconFramework,
                               iobjPassInfo.itrnFramework);
            }
            return 1;
        }

        // //Validate Plan particaipation date less than employment date
        public bool IsPlanStartDateOverlapWithEmployment()
        {
            bool lblnValid = false;
            if (icdoPersonAccount.person_account_id == 0)
            {
                if (icdoPersonAccount.start_date != DateTime.MinValue)
                {
                    if (icdoPersonAccount.start_date >= ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date)
                    {
                        lblnValid = true;
                    }
                }
            }
            else
            {
                if (iclbAccountEmploymentDetail == null)
                    LoadPersonAccountEmploymentDetails();
                foreach (busPersonAccountEmploymentDetail lobjAccountDetail in iclbAccountEmploymentDetail)
                {
                    if (lobjAccountDetail.ibusEmploymentDetail == null)
                        lobjAccountDetail.LoadPersonEmploymentDetail();
                    if (busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.start_date,
                           lobjAccountDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                           lobjAccountDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date))
                    {
                        lblnValid = true;
                        break;
                    }
                }
            }
            return lblnValid;
        }

        //Validate Plan participation date less than employment date
        public bool IsPlanStartDateOverlapWithEmployerOrgPlan()
        {
            //only when employment exists
            if (icdoPersonAccount.person_employment_dtl_id > 0)
            {
                if (ibusPersonEmploymentDetail == null)
                    LoadPersonEmploymentDetail();

                if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                    ibusPersonEmploymentDetail.LoadPersonEmployment();

                if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                    ibusPersonEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                if (ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan == null)
                    ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.LoadOrgPlan();

                // PROD PIR ID 6136 -- Enrollment start date cannot be earlier than plan participation start date
                // PROD PIR ID 5653 -- History change date should be used, Start date will be used only in new mode.
                bool lblnResult = ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.iclbOrgPlan.Any(i => i.icdoOrgPlan.plan_id == icdoPersonAccount.plan_id &&
                                        busGlobalFunctions.CheckDateOverlapping(
                                        (icdoPersonAccount.history_change_date == DateTime.MinValue) ? icdoPersonAccount.start_date : icdoPersonAccount.history_change_date,
                                        i.icdoOrgPlan.participation_start_date, i.icdoOrgPlan.participation_end_date));
                return lblnResult;
            }
            return true;
        }

        /// <summary>
        /// This Method is used to get the Employment Detail ID for the Given Plan.
        /// This will be useful when the user clicks the plan (person account) maintenance screen, and to determine the employment detail id
        /// </summary>        
        /// <returns></returns>
        public int GetEmploymentDetailID()
        {
            return GetEmploymentDetailID(icdoPersonAccount.plan_id, icdoPersonAccount.person_account_id);
        }

        public int GetEmploymentDetailID(int aintPlanID)
        {
            return GetEmploymentDetailID(aintPlanID, icdoPersonAccount.person_account_id);
        }

        public int GetEmploymentDetailID(DateTime adtEffectiveDate, bool ablnReloadPAEmpDetail = false)
        {
            return GetEmploymentDetailID(icdoPersonAccount.plan_id, icdoPersonAccount.person_account_id, adtEffectiveDate, ablnReloadPAEmpDetail);
        }

        public int GetEmploymentDetailID(int aintPlanID, int aintPersonAccountID)
        {
            //If No Effective Date Mentioned, Load the Plan Effective Date. If Plan Effective is not loaded, Load the Current Date
            DateTime ldtEffectiveDate = DateTime.Now;
            if (idtPlanEffectiveDate != DateTime.MinValue)
                ldtEffectiveDate = idtPlanEffectiveDate;
            return GetEmploymentDetailID(aintPlanID, aintPersonAccountID, ldtEffectiveDate);
        }

        public int GetEmploymentDetailID(int aintPlanID, int aintPersonAccountID, DateTime adtEffectiveDate, bool ablnReloadPAEmpDetail = false, bool ablnIncludeWaived = false)
        {
            int lintEmploymentDetailID = 0;

            if (iclbAccountEmploymentDetail == null || ablnReloadPAEmpDetail)
                LoadPersonAccountEmploymentDetails();

            foreach (busPersonAccountEmploymentDetail lobjPerAcctEmpDtl in iclbAccountEmploymentDetail)
            {
                if ((lobjPerAcctEmpDtl.icdoPersonAccountEmploymentDetail.plan_id == aintPlanID) &&
                    (lobjPerAcctEmpDtl.icdoPersonAccountEmploymentDetail.election_value == busConstant.PlanOptionStatusValueEnrolled || ablnIncludeWaived))
                {
                    if (lobjPerAcctEmpDtl.ibusEmploymentDetail == null)
                    {
                        if (idtbPlanCacheData != null)
                            lobjPerAcctEmpDtl.idtbPlanCacheData = idtbPlanCacheData;
                        lobjPerAcctEmpDtl.LoadPersonEmploymentDetail();
                    }
                    if ((lobjPerAcctEmpDtl.ibusEmploymentDetail != null) && (lobjPerAcctEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0))
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                            lobjPerAcctEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                            lobjPerAcctEmpDtl.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null))
                        {
                            lintEmploymentDetailID = lobjPerAcctEmpDtl.icdoPersonAccountEmploymentDetail.person_employment_dtl_id;
                            break;
                        }

                    }
                }
            }
            return lintEmploymentDetailID;
        }

        public int GetPreviousPersonAccountID(int AintPlanId, int AintPersonID)
        {
            int lintPrevPersonAccountID = 0;
            lintPrevPersonAccountID = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.GetPersonAccountIDWithEndDate", new object[2]{
                                        AintPersonID,AintPlanId}, iobjPassInfo.iconFramework,
                                        iobjPassInfo.itrnFramework));
            return lintPrevPersonAccountID;
        }

        public int GetPreviousEmploymentDetailID(int AintPersonAccountID)
        {
            int lintPreviousEmploymentDetailID = 0;
            lintPreviousEmploymentDetailID = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.GetLastEmploymentDetailID", new object[1]{
                                                    AintPersonAccountID}, iobjPassInfo.iconFramework,
                                                    iobjPassInfo.itrnFramework));
            return lintPreviousEmploymentDetailID;
        }

        public void PostRetirementContribution(string astrSubSystem, int aintRefID, DateTime adatTransaction, DateTime adatEffective, int aintMonth, int aintYear,
            int aintEmplDtlID, string astrTransactionType, decimal adecSalary, decimal adecPostTaxERAmt, decimal adecPostTaxEEAmt, decimal adecPreTaxERAmt,
            decimal adecPreTaxEEAmt, decimal adecEERHICAmt, decimal adecERRHICAmt, decimal adecPickupAmt, decimal adecERVastedAmt, decimal adecInterestAmt,
            decimal adecVSC, decimal adecPSC)
        {
            cdoPersonAccountRetirementContribution lcdoContribution = new cdoPersonAccountRetirementContribution();
            lcdoContribution.person_account_id = icdoPersonAccount.person_account_id;
            lcdoContribution.subsystem_value = astrSubSystem;
            lcdoContribution.subsystem_ref_id = aintRefID;
            lcdoContribution.transaction_date = adatTransaction;
            lcdoContribution.effective_date = adatEffective;
            lcdoContribution.pay_period_month = aintMonth;
            lcdoContribution.pay_period_year = aintYear;
            lcdoContribution.person_employment_dtl_id = aintEmplDtlID;
            lcdoContribution.transaction_type_value = astrTransactionType;
            lcdoContribution.salary_amount = adecSalary;
            lcdoContribution.post_tax_er_amount = adecPostTaxERAmt;
            lcdoContribution.post_tax_ee_amount = adecPostTaxEEAmt;
            lcdoContribution.pre_tax_er_amount = adecPreTaxERAmt;
            lcdoContribution.pre_tax_ee_amount = adecPreTaxEEAmt;
            lcdoContribution.ee_rhic_amount = adecEERHICAmt;
            lcdoContribution.er_rhic_amount = adecERRHICAmt;
            lcdoContribution.ee_er_pickup_amount = adecPickupAmt;
            lcdoContribution.er_vested_amount = adecERVastedAmt;
            lcdoContribution.interest_amount = adecInterestAmt;
            lcdoContribution.vested_service_credit = adecVSC;
            lcdoContribution.pension_service_credit = adecPSC;
            lcdoContribution.Insert();
        }

        //UCS-041 Psoting Def Comp Contribution
        /// <summary>
        /// Posting Def Comp Contribution 
        /// </summary>
        /// <param name="astrSubSystem"></param>
        /// <param name="aintRefID"></param>
        /// <param name="adatTransaction"></param>
        /// <param name="adatEffective"></param>
        /// <param name="adtPayPeriodStartDate"></param>
        /// <param name="adtPayPeriodEndDate"></param>
        /// <param name="aintEmplDtlID"></param>
        /// <param name="adtPaidDate"></param>
        /// <param name="astrTransactionType"></param>
        /// <param name="aintProviderOrgId"></param>
        /// <param name="adecPayPeriodContributionAmt"></param>
        public void PostDefCompContribution(string astrSubSystem, int aintRefID, DateTime adatTransaction, DateTime adatEffective, DateTime adtPayPeriodStartDate,
            DateTime adtPayPeriodEndDate, int aintEmplDtlID, DateTime adtPaidDate, string astrTransactionType, int aintProviderOrgId, decimal adecPayPeriodContributionAmt)
        {
            cdoPersonAccountDeferredCompContribution lcdoContribution = new cdoPersonAccountDeferredCompContribution();
            lcdoContribution.person_account_id = icdoPersonAccount.person_account_id;
            lcdoContribution.subsystem_value = astrSubSystem;
            lcdoContribution.subsystem_ref_id = aintRefID;
            lcdoContribution.transaction_date = adatTransaction;
            lcdoContribution.effective_date = adatEffective;
            lcdoContribution.pay_period_start_date = adtPayPeriodStartDate;
            lcdoContribution.pay_period_end_date = adtPayPeriodEndDate;
            lcdoContribution.person_employment_dtl_id = aintEmplDtlID;
            lcdoContribution.paid_date = adtPaidDate;
            lcdoContribution.transaction_type_value = astrTransactionType;
            lcdoContribution.provider_org_id = aintProviderOrgId;
            lcdoContribution.pay_period_contribution_amount = adecPayPeriodContributionAmt;
            lcdoContribution.Insert();
        }

        //Post Insurance Contribution
        public void PostInsuranceContribution(string astrSubSystem, int aintRefID, DateTime adatTransaction, DateTime adatEffective, int aintEmplDtlID,
            string astrTransactionType, decimal adecDuePremiumAmt, decimal adecPaidPremiumAmt, decimal adecRHICBenefitAmt, decimal adecOthrRHICAmt, decimal adecJSRHICAmt,
            decimal adecGroupHeatlthFeeAmt, decimal adecBuydownAmt, decimal adecMedicarePartDAmt,decimal adecLifeBasicPremiumAmount, decimal adecLifeSuppPremiumAmt, decimal adecLifeSpouseSuppPremiumAmt, decimal adecLifeDepSuppPremiumAmt,
            decimal adecLtcMemberThreeYrsPremium, decimal adecLtcMemberFiveYrsPremium, decimal adecLtcSpouseThreeYrsPremium, decimal adecLtcSpouseFiveYrsPremium,
            int aintProviderOrgID)
        {
            cdoPersonAccountInsuranceContribution lcdoContribution = new cdoPersonAccountInsuranceContribution();
            lcdoContribution.person_account_id = icdoPersonAccount.person_account_id;
            lcdoContribution.subsystem_value = astrSubSystem;
            lcdoContribution.subsystem_ref_id = aintRefID;
            lcdoContribution.transaction_date = adatTransaction;
            lcdoContribution.effective_date = adatEffective;
            lcdoContribution.due_premium_amount = adecDuePremiumAmt;
            lcdoContribution.paid_premium_amount = adecPaidPremiumAmt;
            lcdoContribution.person_employment_dtl_id = aintEmplDtlID;
            lcdoContribution.rhic_benefit_amount = adecRHICBenefitAmt;
            /* UAT PIR 476, Including other and JS RHIC Amount */
            lcdoContribution.othr_rhic_amount = adecOthrRHICAmt;
            lcdoContribution.js_rhic_amount = adecJSRHICAmt;
            /* UAT PIR 476 ends here */
            lcdoContribution.transaction_type_value = astrTransactionType;
            lcdoContribution.group_health_fee_amt = adecGroupHeatlthFeeAmt;
            lcdoContribution.buydown_amount = adecBuydownAmt;
            lcdoContribution.medicare_part_d_amt = adecMedicarePartDAmt;//PIR 14271
            lcdoContribution.life_basic_premium_amount = adecLifeBasicPremiumAmount;
            lcdoContribution.life_dep_supp_premium_amount = adecLifeDepSuppPremiumAmt;
            lcdoContribution.life_spouse_supp_premium_amount = adecLifeSpouseSuppPremiumAmt;
            lcdoContribution.life_dep_supp_premium_amount = adecLifeSuppPremiumAmt;
            lcdoContribution.ltc_member_three_yrs_premium_amount = adecLtcMemberThreeYrsPremium;
            lcdoContribution.ltc_member_five_yrs_premium_amount = adecLtcMemberFiveYrsPremium;
            lcdoContribution.ltc_spouse_three_yrs_premium_amount = adecLtcSpouseThreeYrsPremium;
            lcdoContribution.ltc_spouse_five_yrs_premium_amount = adecLtcSpouseFiveYrsPremium;
            lcdoContribution.provider_org_id = aintProviderOrgID;
            lcdoContribution.Insert();
        }
        /// <summary>
        /// PIR 18085, Need this method to load all contributing employments of a person account
        /// </summary>
        public void LoadAllPersonEmployments(DateTime adtEffectiveDate)
        {
            DataTable ldtbEmployments = Select("cdoPersonAccount.LoadAllPersonEmployments", new object[2] { icdoPersonAccount.person_account_id, adtEffectiveDate });
            iclcPersonEmployments = new Collection<busPersonEmployment>();
            if(ldtbEmployments.Rows.Count > 0)
            {
                foreach (DataRow ldatarow in ldtbEmployments.Rows)
                {
                    busPersonEmployment lbusPersonEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };
                    lbusPersonEmployment.ibusOrganization = new busOrganization  { icdoOrganization = new cdoOrganization() };
                    lbusPersonEmployment.icdoPersonEmployment.LoadData(ldatarow);
                    lbusPersonEmployment.ibusOrganization.icdoOrganization.LoadData(ldatarow);
                    iclcPersonEmployments.Add(lbusPersonEmployment);
                }
            }
        }
        public Collection<busPersonEmployment> iclcPersonEmployments { get; set; }
        public bool IsRetirementPlanOpen()
        {
            return (ibusPlan.IsRetirementPlan() &&
                (icdoPersonAccount.end_date_no_null == DateTime.MaxValue));
            //(icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled) ||
            //(icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended));
        }

        public bool IsPlanParticipationStatusRetirementRetired()
        {
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired)
            {
                return true;
            }
            return false;
        }

        public bool IsPlanParticipationStatusRetiredOrSuspended()
        {
            if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired)
                || (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended))
            {
                return true;
            }
            return false;
        }

        public bool IsPlanParticipationStatusRetiredOrSuspendedOrEnrolled()
        {
            if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                || IsPlanParticipationStatusRetiredOrSuspended())
            {
                return true;
            }
            return false;
        }
        //UAT PIR 1534
        public bool IsClosedAccountStatus()
        {
            if ((icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetirementEnrolled) &&
                (icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusRetimentSuspended))
            {
                return true;
            }
            return false;
        }

        public bool IsPlanParticipationStatusCancelled()
        {
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirmentCancelled)
            {
                return true;
            }
            return false;
        }

        public bool IsPlanParticipationStatusRetirementEnrolled()
        {
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
            {
                return true;
            }
            return false;
        }

        public bool IsPlanParticipationStatusRetirementSuspended()
        {
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended)
            {
                return true;
            }
            return false;
        }

        public void LoadTotalPSC()
        {
            LoadTotalPSC(DateTime.MinValue);
        }

        public void LoadTotalPSC(DateTime adteGivenDate, bool alblnNullCheckContribution = true, bool alblnIsDROEstimate = false, DateTime? adteEffectiveDate = null, bool ablnCopyFromPurchase = false) //Service Purchase PIR-11115 (PIR-12732)
        {
            //Check if any PSC, VSC already posted for this month
            DataTable ldtbList = new DataTable();
            if (idtRetirementContributionAll == null && alblnNullCheckContribution)
                LoadRetirementContributionAllDataTable();
            else
                LoadRetirementContributionAllDataTable();

            if (adteGivenDate == DateTime.MinValue)
            {
                ldtbList = idtRetirementContributionAll;
            }
            else
            {
                if (alblnIsDROEstimate)// PROD PIR ID 1414
                    ldtbList = idtRetirementContributionAll.AsEnumerable().Where(i => i.Field<DateTime>("effective_date") <= adteGivenDate).AsDataTable();
                else
                    ldtbList = idtRetirementContributionAll.AsEnumerable().Where(i => i.Field<DateTime>("effective_date") < adteGivenDate).AsDataTable();
            }
            //Service Purchase PIR-11115 (PIR-12732)
            if (adteEffectiveDate.IsNotNull() && adteEffectiveDate != DateTime.MinValue && ablnCopyFromPurchase)
            {
                ldtbList = idtRetirementContributionAll.AsEnumerable().Where(i => i.Field<DateTime>("created_date") < adteEffectiveDate).AsDataTable();
            }

            decimal ldecPSC = 0.00M;
            foreach (DataRow ldrRow in ldtbList.Rows)
            {
                // PIR ID 1886 - Exclude SubSystemValueBenefitPayment
                if (!Convert.IsDBNull(ldrRow["SUBSYSTEM_VALUE"]))
                {
                    if ((ldrRow["SUBSYSTEM_VALUE"]).ToString() != busConstant.SubSystemValueBenefitPayment)
                    {
                        if (!Convert.IsDBNull(ldrRow["PENSION_SERVICE_CREDIT"]))
                            ldecPSC += (decimal)ldrRow["PENSION_SERVICE_CREDIT"];
                    }
                }
            }
            icdoPersonAccount.Total_PSC = ldecPSC;
        }
        public void LoadTotalVSC(DateTime? adteEffectiveDate = null, bool ablnCopyFromPurchase = false, bool ablnExcludeTFFRTIAA = false) //Service Purchase PIR-11115 (PIR-12732)
        {
            decimal ldecVSC = 0.00M;
            decimal ldecTFFRService = 0.00M;
            decimal ldecTIAAService = 0.00M;
            //PIR 791
            //Check for TIAA , TFFR service amount ,if exists with approved status ,add it to TVSC
            LoadTFFRTIAAService(ref ldecTFFRService, ref ldecTIAAService);

            //Check if any PSC, VSC already posted for this month
            if (idtRetirementContributionAll == null)
            {
                LoadRetirementContributionAllDataTable();
            }
            //Service Purchase PIR-11115 (PIR-12732)
            if(adteEffectiveDate.IsNotNull() && adteEffectiveDate != DateTime.MinValue && ablnCopyFromPurchase)
            {
                idtRetirementContributionAll = idtRetirementContributionAll.AsEnumerable().Where(i => i.Field<DateTime>("created_date") <= adteEffectiveDate).AsDataTable();
            }
            foreach (DataRow ldrRow in idtRetirementContributionAll.Rows)
            {
                // PIR ID 1886 - Exclude SubSystemValueBenefitPayment
                if (!Convert.IsDBNull(ldrRow["SUBSYSTEM_VALUE"]))
                {
                    if ((ldrRow["SUBSYSTEM_VALUE"]).ToString() != busConstant.SubSystemValueBenefitPayment)
                    {
                        if (!Convert.IsDBNull(ldrRow["VESTED_SERVICE_CREDIT"]))
                            ldecVSC += (decimal)ldrRow["VESTED_SERVICE_CREDIT"];
                    }
                }
            }
            if (!ablnExcludeTFFRTIAA)
                icdoPersonAccount.Total_VSC = ldecVSC + ldecTFFRService + ldecTIAAService;
            else
                icdoPersonAccount.Total_VSC = ldecVSC;

        }

        public DataTable idtbListTFFRTIAA { get; set; }
        public void LoadTFFRTIAAService(ref decimal ldecTFFRService, ref decimal ldecTIAAService)
        {
            decimal ldecTFFRTentativeService = 0.0M;
            decimal ldecTIAATentativeService = 0.0M;
            LoadTFFRTIAAService(ref ldecTFFRService, ref  ldecTIAAService, ref  ldecTFFRTentativeService, ref  ldecTIAATentativeService);
        }

        public void LoadTFFRTIAAService(ref decimal ldecTFFRService, ref decimal ldecTIAAService, ref decimal ldecTFFRTentativeService, ref decimal ldecTIAATentativeService)
        {
            if (idtbListTFFRTIAA == null)
            {
                idtbListTFFRTIAA = Select<cdoPersonTffrTiaaService>(
                 new string[1] { "person_id" }, new object[1] { icdoPersonAccount.person_id }, null, null);
            }
            foreach (DataRow ldrRowService in idtbListTFFRTIAA.Rows)
            {
                if (ldrRowService["tffr_service_status_value"].ToString() == busConstant.PersonTFFRTIAAServiceStatusApproved)
                {
                    if (!Convert.IsDBNull(ldrRowService["tffr_service"]))
                    {
                        //Systest Critical PIR:2075 Casting Exception
                        ldecTFFRService += Convert.ToDecimal(ldrRowService["tffr_service"]);
                    }
                }
                if (ldrRowService["tffr_service_status_value"].ToString() == busConstant.PersonTFFRTIAAServiceStatusTentative)
                {
                    if (!Convert.IsDBNull(ldrRowService["tffr_service"]))
                    {
                        ldecTFFRTentativeService += Convert.ToDecimal(ldrRowService["tffr_service"]);
                    }
                }

                if (ldrRowService["tiaa_service_status_value"].ToString() == busConstant.PersonTFFRTIAAServiceStatusApproved)
                {
                    if (!Convert.IsDBNull(ldrRowService["tiaa_service"]))
                    {
                        //Systest Critical PIR:2075 Casting Exception
                        ldecTIAAService += Convert.ToDecimal(ldrRowService["tiaa_service"]);
                    }
                }

                if (ldrRowService["tiaa_service_status_value"].ToString() == busConstant.PersonTFFRTIAAServiceStatusTentative)
                {
                    if (!Convert.IsDBNull(ldrRowService["tiaa_service"]))
                    {
                        ldecTIAATentativeService += Convert.ToDecimal(ldrRowService["tiaa_service"]);
                    }
                }
            }
        }

        // Correspondence Batch Letters
        public override busBase GetCorPerson()
        {
            if (ibusPerson == null)
                LoadPerson();
            return ibusPerson;
        }

        /// Used in Correspondence and in HIPAA File
        private busPersonDependent _ibusDependentPerson;
        public busPersonDependent ibusDependentPerson
        {
            get { return _ibusDependentPerson; }
            set { _ibusDependentPerson = value; }
        }

        private Collection<busPersonAccountEmploymentDetail> _iclbAccountEmploymentDetail;
        public Collection<busPersonAccountEmploymentDetail> iclbAccountEmploymentDetail
        {
            get { return _iclbAccountEmploymentDetail; }
            set { _iclbAccountEmploymentDetail = value; }
        }

        //PIR 12547
        private Collection<busPersonAccountEmploymentDetail> _iclbPersonAccountEmpDetailByPlan;
        public Collection<busPersonAccountEmploymentDetail> iclbPersonAccountEmpDetailByPlan
        {
            get { return _iclbPersonAccountEmpDetailByPlan; }
            set { _iclbPersonAccountEmpDetailByPlan = value; }
        }

        public void LoadPersonAccountEmploymentDetails()
        {
            if (_iclbAccountEmploymentDetail == null)
                _iclbAccountEmploymentDetail = new Collection<busPersonAccountEmploymentDetail>();

            //Load the Person Account Employment Detail only if the person accout id not zero. 
            //otherwise it will load all the records which are having person account id is zero
            if (icdoPersonAccount.person_account_id > 0)
            {
                DataTable ldtbList = Select<cdoPersonAccountEmploymentDetail>(
                    new string[1] { "person_account_id" },
                    new object[1] { icdoPersonAccount.person_account_id }, null, null);
                _iclbAccountEmploymentDetail = GetCollection<busPersonAccountEmploymentDetail>(ldtbList, "icdoPersonAccountEmploymentDetail");
            }
        }
        //UAT PIR: 977 For Setting the DC Plan Eligibility Date for DB plans, we have to load the 
        //Employment details for that person and plan. This Method is used for this purpose.
        public void LoadPersonAccountEmploymentDetailsByPersonPlan()
        {
            if (_iclbAccountEmploymentDetail == null)
                _iclbAccountEmploymentDetail = new Collection<busPersonAccountEmploymentDetail>();

            DataTable ldtbList = Select("cdoPersonAccount.GetEmploymentDetailIDByPlan", new object[2] { icdoPersonAccount.plan_id, icdoPersonAccount.person_id });

            foreach (DataRow dr in ldtbList.Rows)
            {
                busPersonAccountEmploymentDetail lobjPersonAccountEmpDetail = new busPersonAccountEmploymentDetail
                {
                    icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail()
                };
                lobjPersonAccountEmpDetail.icdoPersonAccountEmploymentDetail.LoadData(dr);

                iclbAccountEmploymentDetail.Add(lobjPersonAccountEmpDetail);
            }
        }

        // Person account adjustment
        private Collection<busPersonAccountAdjustment> _iclbAdjustment;
        public Collection<busPersonAccountAdjustment> iclbAdjustment
        {
            get { return _iclbAdjustment; }
            set { _iclbAdjustment = value; }
        }
        public void LoadPersonAccountAdjustment()
        {
            DataTable ldtbList = Select<cdoPersonAccountAdjustment>(
                new string[1] { "person_account_id" },
                new object[1] { icdoPersonAccount.person_account_id }, null, null);
            _iclbAdjustment = GetCollection<busPersonAccountAdjustment>(ldtbList, "icdoPersonAccountAdjustment");
            foreach (busPersonAccountAdjustment lbusAdjustment in _iclbAdjustment)
            {
                lbusAdjustment.ibusPersonAccount = this;
            }
        }

        // Display for combo of Person Account List
        public string istrPlanName
        {
            get
            {
                return ibusPlan.icdoPlan.plan_name;
            }
        }
        public string istrCapitalPlanName
        {
            get
            {
                if (ibusPlan.IsNull())
                    LoadPlan();
                return ibusPlan.icdoPlan.plan_name.ToUpper();
            }
        }
		//PIR 14656 - Used on Contribution Transfer Maintenance to show the plan name along with plan participation status
        public string istrPlanNameWithParticipationStatusText
        {
            get
            {
                return ibusPlan.icdoPlan.plan_name + " - " + icdoPersonAccount.plan_participation_status_description;
            }
        }
        public int iiintPersonAccountID
        {
            get
            {
                return icdoPersonAccount.person_account_id;
            }
        }

        private bool _IsHistoryEntryRequired;
        public bool IsHistoryEntryRequired
        {
            get
            {
                return _IsHistoryEntryRequired;
            }
            set
            {
                _IsHistoryEntryRequired = value;
            }
        }

        public bool IsHistoryChangeDateStartsWithFirstDayOfMonth()
        {
            if ((IsHistoryEntryRequired)) //&& (icdoPersonAccount.end_date == DateTime.MinValue) // pir 7386
            {
                if (icdoPersonAccount.history_change_date != DateTime.MinValue)
                {
                    if (icdoPersonAccount.history_change_date.Day != 1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private busPersonEmploymentDetail _ibusPreviousEmploymentDetail;
        public busPersonEmploymentDetail ibusPreviousEmploymentDetail
        {
            get { return _ibusPreviousEmploymentDetail; }
            set { _ibusPreviousEmploymentDetail = value; }
        }

        public busPersonEmploymentDetail ibusPreviousEmploymentDetailForTransfer { get; set; }

        //Property to check whether Payment election is been changed or not
        public bool iblnIsPaymentElectionChanged { get; set; }

        // Used in GHDV and Life, to load previous employment detail.       
        public void LoadPreviousEmploymentDetail()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPlan == null)
                LoadPlan();
            if (_ibusPreviousEmploymentDetail == null)
            {
                _ibusPreviousEmploymentDetail = new busPersonEmploymentDetail();
                _ibusPreviousEmploymentDetail.icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();
            }
            _ibusPreviousEmploymentDetail = GetLatestClosedPersonEmploymentDetail(false);
        }

        //used to check for transfer employment
        //check for the employement header not employment detail
        public void LoadPreviousEmploymentDetailForTransfer()
        {
            if (ibusPerson == null)
                LoadPerson();
            if (ibusPlan == null)
                LoadPlan();
            if (_ibusPreviousEmploymentDetail == null)
            {
                ibusPreviousEmploymentDetailForTransfer = new busPersonEmploymentDetail();
                ibusPreviousEmploymentDetailForTransfer.icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail();
            }
            ibusPreviousEmploymentDetailForTransfer = GetLatestClosedPersonEmploymentDetail(true);
        }

        //get the latest  closed employment detail
        public busPersonEmploymentDetail GetLatestClosedPersonEmploymentDetail(bool ablnIsForTransfer)
        {
            busPersonEmploymentDetail lobjPersonEmploymentDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            if (iclbAccountEmploymentDetail.IsNull())
                LoadPersonAccountEmploymentDetails();

            foreach (busPersonAccountEmploymentDetail lobjPersonAccountEmploymentDetail in iclbAccountEmploymentDetail)
            {
                if (lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.IsNull())
                    lobjPersonAccountEmploymentDetail.LoadPersonEmploymentDetail();
                if (lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.ibusPersonEmployment.IsNull())
                    lobjPersonAccountEmploymentDetail.ibusEmploymentDetail.LoadPersonEmployment();
            }

            if (ibusPersonEmploymentDetail.IsNull())
                LoadPersonEmploymentDetail();
            if (ibusPersonEmploymentDetail.ibusPersonEmployment.IsNull())
                ibusPersonEmploymentDetail.LoadPersonEmployment();

            if (iclbAccountEmploymentDetail.Count > 0)
            {
                if (ablnIsForTransfer)
                {
                    //PIR - 589
                    //sort order by descending end date as the greater end will be closed to the current employment date
                    var lclbPAEmpDtlOrderByEndDate = iclbAccountEmploymentDetail.Where(lobjPersonAccEmpl => lobjPersonAccEmpl.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id
                                                                        != ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id
                                                                        && (lobjPersonAccEmpl.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id > 0))
                                                                        .OrderByDescending(lobjPerEMpl => lobjPerEMpl.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date);
                    if (lclbPAEmpDtlOrderByEndDate.Count() > 0)
                    {
                        lobjPersonEmploymentDtl = lclbPAEmpDtlOrderByEndDate.FirstOrDefault().ibusEmploymentDetail;
                    }
                }
                else
                {
                    var lclbPAEmpDtlOrderByEndDate = iclbAccountEmploymentDetail.OrderByDescending(lobjPerEMpl => lobjPerEMpl.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date);

                    if (lclbPAEmpDtlOrderByEndDate.Count() > 0)
                    {
                        lobjPersonEmploymentDtl = lclbPAEmpDtlOrderByEndDate.FirstOrDefault().ibusEmploymentDetail;
                    }
                }
            }
            return lobjPersonEmploymentDtl;
        }

        //get the latest  open employment detail
        public busPersonEmploymentDetail GetLatestOpenedPersonEmploymentDetail()
        {
            busPersonEmploymentDetail lobjPersonEmploymentDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            if (iclbAccountEmploymentDetail.IsNull())
                LoadPersonAccountEmploymentDetails();
            if (iclbAccountEmploymentDetail.Count > 0)
            {
                foreach(busPersonAccountEmploymentDetail lobjPAEmpDtl in iclbAccountEmploymentDetail)
                {
                    // Null check added to avoid exceptions
                    if(lobjPAEmpDtl.ibusEmploymentDetail.IsNull())
                        lobjPAEmpDtl.LoadPersonEmploymentDetail();
                }
                //PIR - 589
                //sort order by descending end date as the greater end will be open to the current employment date
                var lclbPAEmpDtlOrderByEndDate = iclbAccountEmploymentDetail.OrderByDescending(lobjPerEMpl => lobjPerEMpl.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null);

                if (lclbPAEmpDtlOrderByEndDate.Count() > 0)
                    lobjPersonEmploymentDtl = lclbPAEmpDtlOrderByEndDate.FirstOrDefault().ibusEmploymentDetail;
            }
            return lobjPersonEmploymentDtl;
        }

        // To Load by Person Account ID
        private Collection<busPersonAccountDependent> _iclbPersonAccountDependent;
        public Collection<busPersonAccountDependent> iclbPersonAccountDependent
        {
            get { return _iclbPersonAccountDependent; }
            set { _iclbPersonAccountDependent = value; }
        }

        public void LoadPersonAccountDependent()
        {
            if (_iclbPersonAccountDependent == null)
                _iclbPersonAccountDependent = new Collection<busPersonAccountDependent>();

            DataTable ldtbList = Select<cdoPersonAccountDependent>(
                                      new string[1] { "person_account_id" },
                                      new object[1] { icdoPersonAccount.person_account_id }, null, null);
            _iclbPersonAccountDependent = GetCollection<busPersonAccountDependent>(ldtbList, "icdoPersonAccountDependent");
        }

        //UCS -071 -PIR 992
        public bool IsACHDetailNotEnteredForACHPaymentMethod()
        {
            if (ibusPaymentElection == null)
                LoadPaymentElection();
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentACH)
            {
                if (iclbPersonAccountAchDetail == null)
                    LoadPersonAccountAchDetail();
                if (iclbPersonAccountAchDetail.Count == 0)
                {
                    return true;
                }
            }
            return false;
        }

        //Get Latest employment for the plan based on the latest start date.
        //Load Person Account Employment Details for the person account
        //loop thru the collection and load the employment detail 
        //return the busPersonEmploymentdetail which is having the latest employment start date
        //this function is used in Benefit Application.
        public busPersonEmploymentDetail GetLatestEmploymentDetail()
        {
            // UAT PIR ID 1351
            if (iclbAccountEmploymentDetail == null)
                LoadPersonAccountEmploymentDetails();
            busPersonEmploymentDetail lobjResultPersonEmploymentDetail = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            foreach (busPersonAccountEmploymentDetail lobjPersonAccEmploymentDetail in iclbAccountEmploymentDetail)
                lobjPersonAccEmploymentDetail.LoadPersonEmploymentDetail();
            var larrPAEmpDtl = iclbAccountEmploymentDetail.OrderByDescending(lobj => lobj.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date).FirstOrDefault();
            if (larrPAEmpDtl.IsNotNull())
                lobjResultPersonEmploymentDetail = larrPAEmpDtl.ibusEmploymentDetail;
            return lobjResultPersonEmploymentDetail;
        }

        public bool IsWithDrawn()
        {
            if ((icdoPersonAccount.plan_participation_status_value ==
                 busConstant.PlanParticipationStatusRetirementWithDrawn) ||
                (icdoPersonAccount.plan_participation_status_value ==
                 busConstant.PlanParticipationStatusDeffCompWithDrawn))
            {
                return true;
            }
            return false;
        }

        public bool IsTransferredDC()
        {
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferDC)
            {
                return true;
            }
            return false;
        }
        //PIR 16439
        public bool IsTransferredDB()
        {
            return (icdoPersonAccount.IsNotNull() && icdoPersonAccount.plan_participation_status_value == busConstant.RetirementPlanParticipationStatusTranToDb)
                ? true : false;
        }


        #region Correspondence

        //ERN- 5001 LTC related cor
        //TODO - need to assign
        public string istrLTCCarrierPlanName { get; set; }

        /// ENR-5950 Welcome Batch
        private bool _IsPermanentPreviousMember;
        public bool IsPermanentPreviousMember
        {
            get { return _IsPermanentPreviousMember; }
            set { _IsPermanentPreviousMember = value; }
        }

        // Non-Payee Employment Termination Batch, For Template PER-0152
        private int _Member_Terminated_Plan_ID;
        public int Member_Terminated_Plan_ID
        {
            get { return _Member_Terminated_Plan_ID; }
            set { _Member_Terminated_Plan_ID = value; }
        }

        private string _lstrPreviousEmploymentStartDate;
        public string lstrPreviousEmploymentStartDate
        {
            get { return _lstrPreviousEmploymentStartDate; }
            set { _lstrPreviousEmploymentStartDate = value; }
        }

        private string _lstrPreviousEmploymentEndDate;
        public string lstrPreviousEmploymentEndDate
        {
            get { return _lstrPreviousEmploymentEndDate; }
            set { _lstrPreviousEmploymentEndDate = value; }
        }

        private decimal _ldclTotalCOBRAPremiumAmount;
        public decimal ldclTotalCOBRAPremiumAmount
        {
            get { return _ldclTotalCOBRAPremiumAmount; }
            set { _ldclTotalCOBRAPremiumAmount = value; }
        }

        //Corrs. UCS-57 - TIAA-CREF TRANSFER
        public decimal idecTotalTIAA
        {
            get
            {
                decimal ldecTotalTIAA = 0.0M, ldecTotalTFFR = 0.0M;
                LoadTFFRTIAAService(ref ldecTotalTFFR, ref ldecTotalTIAA);
                return Math.Round(ldecTotalTIAA, MidpointRounding.AwayFromZero);
            }
        }

        //Property to return rounded PSC
        public decimal idecTotalPSC_Rounded
        {
            get
            {
                return Math.Round(icdoPersonAccount.Total_PSC, MidpointRounding.AwayFromZero);
            }
        }

        //Property to return rounded VSC
        public decimal idecTotalVSC_Rounded
        {
            get
            {
                return Math.Round(icdoPersonAccount.Total_VSC, MidpointRounding.AwayFromZero);
            }
        }

        #endregion

        #region Medicare Correspondence Properties
        private DateTime _PremiumDueDate;
        public DateTime PremiumDueDate
        {
            get { return _PremiumDueDate; }
            set { _PremiumDueDate = value; }
        }

        public string istrPremiumDueDate
        {
            get
            {
                if (PremiumDueDate != DateTime.MinValue)
                    return PremiumDueDate.ToString(busConstant.DateFormatLongDate);
                else
                    return string.Empty;
            }
        }

        #endregion

        //Function to check whether Payment Election is changed or not
        public void SetPaymentElectionChangedOrNot()
        {
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues.Count > 0)
            {
                if ((ibusPaymentElection.icdoPersonAccountPaymentElection.Billing_Organization !=
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["Billing_Organization"] == null ? null :
                    Convert.ToString(ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["Billing_Organization"]))) ||
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.Supplemental_Billing_Organization !=
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["Supplemental_Billing_Organization"] == null ? null :
                    Convert.ToString(ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["Supplemental_Billing_Organization"]))) ||
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date !=
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["ibs_effective_date"] == null ? DateTime.MinValue :
                    Convert.ToDateTime(ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["ibs_effective_date"]))) ||
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag !=
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["ibs_flag"] == null ? null :
                    Convert.ToString(ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["ibs_flag"]))) ||
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id !=
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payee_account_id"] == null ? 0 :
                    Convert.ToInt32(ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payee_account_id"]))) ||
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value !=
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payment_method_value"] == null ? null :
                    Convert.ToString(ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payment_method_value"]))))
                {
                    iblnIsPaymentElectionChanged = true;
                }
                else
                {
                    iblnIsPaymentElectionChanged = false;
                }
            }
            else
            {
                if (ibusPaymentElection.icdoPersonAccountPaymentElection.Billing_Organization != null ||
                   ibusPaymentElection.icdoPersonAccountPaymentElection.Supplemental_Billing_Organization != null ||
                   ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date != DateTime.MinValue ||
                   ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag != "N" ||
                   ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id != 0 ||
                   ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value != null)
                {
                    iblnIsPaymentElectionChanged = true;
                }
                else
                {
                    iblnIsPaymentElectionChanged = false;
                }
            }
        }

        //Function to check whether IBS date Starts With First Day Of Month
        public bool IsIBSEffectiveDateStartsWithFirstDayOfMonth()
        {
            if ((ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes) &&
                iblnIsPaymentElectionChanged && (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date != DateTime.MinValue))
            {
                if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date.Day != 1)
                    return false;
                else
                    return true;
            }
            else
                return true;
        }

        //Function to check whether ibs effective date is greater than equal to next ibs batch schedule date
        public bool IsIBSEffectiveDateValid()
        {
            if ((ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes) &&
                iblnIsPaymentElectionChanged && (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date != DateTime.MinValue))
            {
                //uat pir 1463
                if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date >=
                    busIBSHelper.GetLastPostedIBSBatchDate().AddMonths(1))
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        //Function to get latest date
        public DateTime GetLatestDate(DateTime ldtFirstDate, DateTime ldtSecondDate, DateTime ldtThirdDate)
        {
            return ldtFirstDate > ldtSecondDate ? (ldtFirstDate > ldtThirdDate ? ldtFirstDate : ldtThirdDate) :
                (ldtSecondDate > ldtThirdDate ? ldtSecondDate : ldtThirdDate);
        }

        /// <summary>
        /// Method to update Capital gain in Person Account Retirement
        /// </summary>
        /// <param name="adecCapitalGain">Capital Gain</param>
        /// <param name="ablnAdd">Bool variable to check whether to Add or Subtract</param>
        public void UpdateCapitalGain(decimal adecCapitalGain, bool ablnAdd)
        {
            if (ibusPersonAccountRetirement == null)
            {
                ibusPersonAccountRetirement = new busPersonAccountRetirement();
                ibusPersonAccountRetirement.FindPersonAccountRetirement(icdoPersonAccount.person_account_id);
            }
            if (ablnAdd)
                ibusPersonAccountRetirement.icdoPersonAccountRetirement.capital_gain += adecCapitalGain;
            else
                ibusPersonAccountRetirement.icdoPersonAccountRetirement.capital_gain -= adecCapitalGain;
            ibusPersonAccountRetirement.icdoPersonAccountRetirement.Update();
        }

        /// <summary>
        /// check if any Payee account exists with status as completed or cancelled
        /// this validation is used in PersonAccountAdjustment And PersonAccount DBDB transfer Screen
        /// </summary>
        /// <returns></returns>
        public bool IfAnyPayeeAccountWithCompletedOrCancelledStatus()
        {
            if (ibusPerson.iclbPayeeAccount.IsNull())
                ibusPerson.LoadPayeeAccount();
            foreach (busPayeeAccount lobjPayeeAccount in ibusPerson.iclbPayeeAccount)
            {
                if (lobjPayeeAccount.ibusApplication.IsNull())
                    lobjPayeeAccount.LoadApplication();
                if (lobjPayeeAccount.ibusApplication.icdoBenefitApplication.plan_id == icdoPersonAccount.plan_id)
                {
                    if (lobjPayeeAccount.iclbPayeeAccountStatus.IsNull())
                        lobjPayeeAccount.LoadActivePayeeStatus();
                    if ((!lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCancelled())
                        &&
                        (!lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusCompleted())
                        &&
                        lobjPayeeAccount.ibusPayeeAccountActiveStatus.IsStatusNotProcessed())
                        return false;
                }
            }
            return true;
        }

        //UCS-041 

        public Collection<busPersonAccountInsuranceTransfer> iclbPersonAccountInsuranceTransfer { get; set; }

        public Collection<busPersonAccountInsuranceTransfer> LoadPersonAccountInsuranceTransfer()
        {
            DataTable ldtbList = Select("cdoPersonAccount.LoadInsuranceTransferDetails", new object[1] { icdoPersonAccount.person_account_id });
            iclbPersonAccountInsuranceTransfer = new Collection<busPersonAccountInsuranceTransfer>();
            foreach (DataRow dr in ldtbList.Rows)
            {
                busPersonAccountInsuranceTransfer lobjInsuranceTransfer = new busPersonAccountInsuranceTransfer
                {
                    icdoPersonAccountInsuranceTransfer = new cdoPersonAccountInsuranceTransfer()
                };
                lobjInsuranceTransfer.icdoPersonAccountInsuranceTransfer.LoadData(dr);
                lobjInsuranceTransfer.LoadTransferedAmount();
                iclbPersonAccountInsuranceTransfer.Add(lobjInsuranceTransfer);
            }
            return iclbPersonAccountInsuranceTransfer;
        }

        # region UCS60

        public Collection<busBenefitApplicationPersonAccount> iclbBenefitApplicationPersonAccounts { get; set; }
        //UCS-060
        //Load All Benefit application based on the Person account
        //in table benefit application person account 
        //for all those person accounts load Payee account
        public void LoadBenefitApplicationPersonAccounts()
        {
            iclbBenefitApplicationPersonAccounts = new Collection<busBenefitApplicationPersonAccount>();
            DataTable ldtbList = Select<cdoBenefitApplicationPersonAccount>(new string[2] { "person_account_id", "is_application_person_account_flag" },
               new object[2] { icdoPersonAccount.person_account_id, busConstant.Flag_Yes }, null, null);

            iclbBenefitApplicationPersonAccounts = GetCollection<busBenefitApplicationPersonAccount>(ldtbList, "icdoBenefitApplicationPersonAccount");
        }

        // A Person account has a only one corresponding RTW Payee account.
        // We are having it as a collection for exceptional cases
        //check for person account not in withdrawan status
        public Collection<busPayeeAccount> iclbPayeeAccounts { get; set; }

        public void LoadPayeeAccounts()
        {
            if (iclbPayeeAccounts.IsNull())
                iclbPayeeAccounts = new Collection<busPayeeAccount>();
            if (iclbBenefitApplicationPersonAccounts.IsNull())
                LoadBenefitApplicationPersonAccounts();

            foreach (busBenefitApplicationPersonAccount lobjBAPersonAccount in iclbBenefitApplicationPersonAccounts)
            {
                if (lobjBAPersonAccount.ibusBenefitApplication.IsNull())
                    lobjBAPersonAccount.LoadBenefitApplication();

                if (lobjBAPersonAccount.ibusBenefitApplication.iclbPayeeAccount.IsNull())
                    lobjBAPersonAccount.ibusBenefitApplication.LoadPayeeAccount();

                if (lobjBAPersonAccount.ibusPersonAccount.IsNull())
                    lobjBAPersonAccount.LoadPersonAccount();

                foreach (busPayeeAccount lobjPayeeAccount in lobjBAPersonAccount.ibusBenefitApplication.iclbPayeeAccount)
                {
                    iclbPayeeAccounts.Add(lobjPayeeAccount);
                }
            }
        }

        /// <summary>
        /// Returns the Latest Retirment/Disability Payee Accounts
        /// </summary>
        /// <returns></returns>
        public busPayeeAccount LoadRetirementDisablityPayeeAccount()
        {
            var lbusPayeeAccount = new busPayeeAccount() { icdoPayeeAccount = new cdoPayeeAccount() };
            if (iclbPayeeAccounts.IsNull())
                LoadPayeeAccounts();
            //PIR 15595 - Canceled payee accounts should not be considered
            foreach (busPayeeAccount lbusstatusPayeeAccount in iclbPayeeAccounts)
            {
                lbusstatusPayeeAccount.LoadActivePayeeStatusAsofNextBenefitPaymentDate();                
            }

            var lenuRetDisPayeeAccount = iclbPayeeAccounts
                .Where(o => o.IsBenefitAccountTypeIsRetirmentOrDisability() && o.ibusPayeeAccountActiveStatus.istrPayeeAccountStatusData2 != busConstant.PayeeAccountStatusCancelled)
                .OrderByDescending(o => o.icdoPayeeAccount.benefit_begin_date);

            if ((lenuRetDisPayeeAccount != null) && (lenuRetDisPayeeAccount.Count() > 0))
            {
                lbusPayeeAccount = lenuRetDisPayeeAccount.First();
            }
            return lbusPayeeAccount;
        }

        /// <summary>
        /// Method to Determine any suspended payee account exists. It also returns the payee account id if exists as a ref parameter
        /// </summary>
        /// <param name="aintPayeeAccountID"></param>
        /// <returns></returns>
        public bool IsSuspendedPayeeAccountExists(ref int aintPayeeAccountID)
        {
            bool lblnResult = false;
            if (iclbPayeeAccounts.IsNull())
                LoadPayeeAccounts();

            foreach (busPayeeAccount lobjPayeeAccount in iclbPayeeAccounts)
            {
                lobjPayeeAccount.LoadActivePayeeStatus();
                if ((lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusDisabilitySuspended)
                    || (lobjPayeeAccount.ibusPayeeAccountActiveStatus.icdoPayeeAccountStatus.status_value == busConstant.PayeeAccountStatusSuspended))
                {
                    aintPayeeAccountID = lobjPayeeAccount.icdoPayeeAccount.payee_account_id;
                    lblnResult = true;
                    break;
                }
            }
            return lblnResult;
        }

        /// <summary>
        /// Function to check whether the Person is Active Dependent
        /// </summary>
        /// <returns>Returns true if the Dependent Person is Active</returns>
        public bool IsActiveDependentForPlan(DateTime adtEffectiveDate)
        {
            //PIR 10697
            if (icdoPersonAccount.is_from_mss == true)
            {
                Collection<busWssPersonDependent> lclbMSSPersonDependent  = new Collection<busWssPersonDependent> ();
                ibusPerson.ibusLatestPesonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();
                ibusPerson.ibusLatestPesonAccountEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(ibusPerson.iintEnrollmentRequest);
                DataTable ldtblist = Select<cdoWssPersonDependent>(new string[1] { "WSS_PERSON_ACCOUNT_ENROLLMENT_REQUEST_ID" },
                    new object[1] { ibusPerson.ibusLatestPesonAccountEnrollmentRequest.icdoWssPersonAccountEnrollmentRequest.wss_person_account_enrollment_request_id }, null, null);
                lclbMSSPersonDependent = GetCollection<busWssPersonDependent>(ldtblist, "icdoWssPersonDependent");
                foreach (busWssPersonDependent lobjPersonDependent in lclbMSSPersonDependent)
                {
                    if (!string.IsNullOrEmpty(lobjPersonDependent.icdoWssPersonDependent.ssn))
                    {
                        lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId = busGlobalFunctions.GetPersonIDBySSN(lobjPersonDependent.icdoWssPersonDependent.ssn);
                    }
                    if (lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId > 0)
                    {
                        busWssPersonDependent lbusWssPersonDependent = new busWssPersonDependent { icdoWssPersonDependent = new cdoWssPersonDependent() };
                        ibusPerson.LoadPersonDependentByDependentForMSS(lobjPersonDependent.icdoWssPersonDependent.iintDependentPersonId);
                        foreach (var lbusPersonDependent in ibusPerson.iclbPersonDependentByDependent)
                        {
                            if (lbusPersonDependent.iclbPersonAccountDependent == null)
                                lbusPersonDependent.LoadPersonAccountDependent();

                            foreach (var lbusPersonAccountDependent in lbusPersonDependent.iclbPersonAccountDependent)
                            {
                                if (lbusPersonAccountDependent.ibusPersonAccount == null)
                                    lbusPersonAccountDependent.LoadPersonAccount();
                                if ((lbusPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == icdoPersonAccount.plan_id) &&
                                   (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                                                                lbusPersonAccountDependent.icdoPersonAccountDependent.start_date,
                                                                lbusPersonAccountDependent.icdoPersonAccountDependent.end_date)))
                                {
                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                    else
                        return false;
                }
                
            }
            else
            {
                if (ibusPerson.iclbPersonDependentByDependent == null)
                    ibusPerson.LoadPersonDependentByDependent();
                foreach (var lbusPersonDependent in ibusPerson.iclbPersonDependentByDependent)
                {
                    if (lbusPersonDependent.iclbPersonAccountDependent == null)
                        lbusPersonDependent.LoadPersonAccountDependent();

                    foreach (var lbusPersonAccountDependent in lbusPersonDependent.iclbPersonAccountDependent)
                    {
                        if (lbusPersonAccountDependent.ibusPersonAccount == null)
                            lbusPersonAccountDependent.LoadPersonAccount();
                        if ((lbusPersonAccountDependent.ibusPersonAccount.icdoPersonAccount.plan_id == icdoPersonAccount.plan_id) &&
                           (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                                                        lbusPersonAccountDependent.icdoPersonAccountDependent.start_date,
                                                        lbusPersonAccountDependent.icdoPersonAccountDependent.end_date)))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            return false;
        }

        public void SetPersonAccountIDInPersonAccountEmploymentDetail()
        {
            if (ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();

            if (ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl == null)
                ibusPersonEmploymentDetail.LoadEnrolledPersonAccountEmploymentDetails();

            foreach (var lbusPAEmpDetail in ibusPersonEmploymentDetail.iclbPersonAccountEmpDtl)
            {
                if ((lbusPAEmpDetail.icdoPersonAccountEmploymentDetail.plan_id == icdoPersonAccount.plan_id) &&
                   (lbusPAEmpDetail.icdoPersonAccountEmploymentDetail.person_account_id == 0))
                {
                    lbusPAEmpDetail.icdoPersonAccountEmploymentDetail.person_account_id = icdoPersonAccount.person_account_id;
                    lbusPAEmpDetail.icdoPersonAccountEmploymentDetail.Update();
                }
            }
            if (iblnIsFromESS)
            {
                if (ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl == null)
                    ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();
                foreach (busPersonAccountEmploymentDetail lobjPAemp in ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl.Where(o =>
                    o.icdoPersonAccountEmploymentDetail.plan_id == busConstant.PlanIdOther457))
                {
                    if (lobjPAemp.icdoPersonAccountEmploymentDetail.election_value != busConstant.PersonAccountElectionValueEnrolled)
                        lobjPAemp.icdoPersonAccountEmploymentDetail.election_value = busConstant.PersonAccountElectionValueEnrolled;
                    lobjPAemp.icdoPersonAccountEmploymentDetail.person_account_id = icdoPersonAccount.person_account_id;
                    lobjPAemp.icdoPersonAccountEmploymentDetail.Update();
                }
            }
        }
        # endregion

        public string istrPreviousPlanParticipationStatus { get; set; }

        // UAT PIR ID 472
        // UCS-022-00b - When suspending/withdrawing a plan participation status from Enrolled, no active Flex Premium conversion should not exits.
        public bool IsActiveConversionExists()
        {
            if (ibusPerson.IsNull()) LoadPerson();

            if ((icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceEnrolled) &&
                  (istrPreviousPlanParticipationStatus == busConstant.PlanParticipationStatusInsuranceEnrolled))
            {
                if (ibusPerson.IsActiveFlexPremiumConversionExists(icdoPersonAccount.plan_id))
                {
                    return true;
                }
            }
            return false;
        }

        //UAT PIR: 968,970,971,982
        // Show Providers whose employment is without end date and show employment with future end date.
        public Collection<cdoOrganization> LoadActiveProviders(bool ablnIsUpdatemode = false)
        {
            Collection<busOrganization> lclbActiveEmployers = new Collection<busOrganization>();
            Collection<cdoOrganization> lclcActiveProviders = new Collection<cdoOrganization>();

            if (iclbEmploymentDetail == null)
                LoadAllPersonEmploymentDetails();
            foreach (busPersonEmploymentDetail lobjEmploymentDetail in iclbEmploymentDetail)
            {
                //PIR: 1904. Show Employers only in Contributing status where end date is null or future. 
                if ((busGlobalFunctions.CheckDateOverlapping(DateTime.Now, lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date,
                                                             lobjEmploymentDetail.icdoPersonEmploymentDetail.end_date_no_null) ||
                                                             lobjEmploymentDetail.icdoPersonEmploymentDetail.start_date > DateTime.Now)
                    || ablnIsUpdatemode)
                {
                    //PIR: 1904. Show Employers only in Contributing status where end date is null or future. 
                    //PROD PIR 4063 : Remove Contributing Condition here.
                    //if (icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation || icdoPersonAccount.plan_id == busConstant.PlanIdOther457)
                    //{
                    //    if (lobjEmploymentDetail.icdoPersonEmploymentDetail.status_value != busConstant.EmploymentStatusContributing)
                    //        continue;
                    //}

                    if (lobjEmploymentDetail.ibusPersonEmployment == null)
                        lobjEmploymentDetail.LoadPersonEmployment();
                    //if (lobjEmploymentDetail.ibusPersonEmployment.ibusOrganization == null)
                        lobjEmploymentDetail.ibusPersonEmployment.LoadOrganization();

                    if (lclbActiveEmployers.FirstOrDefault(i => i.icdoOrganization.org_id == lobjEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id) == null)
                    {
                        if (lobjEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.status_value == busConstant.OrganizationStatusActive)
                        {
                            lobjEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.istrProviderOrgName = lobjEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code;
                            lclbActiveEmployers.Add(lobjEmploymentDetail.ibusPersonEmployment.ibusOrganization);
                        }
                            
                    }
                }
            }

            foreach (var lbusOrganization in lclbActiveEmployers)
            {
                if (lbusOrganization.iclbOrgPlan == null)
                    lbusOrganization.LoadOrgPlan();

                var lenuList =
                    lbusOrganization.iclbOrgPlan.Where(
                        i => i.icdoOrgPlan.plan_id == icdoPersonAccount.plan_id &&
                             ((busGlobalFunctions.CheckDateOverlapping(DateTime.Now, i.icdoOrgPlan.participation_start_date, i.icdoOrgPlan.participation_end_date))
                             || i.icdoOrgPlan.participation_start_date > DateTime.Now || ablnIsUpdatemode));

                if (lenuList != null)
                {
                    foreach (var lbusOrgPlan in lenuList)
                    {
                        if (lbusOrgPlan.iclbOrgPlanProvider == null)
                            lbusOrgPlan.LoadActiveOrgPlanProvidersByEmployerOrgPlan();

                        foreach (var lbusProviderOrgPlanProvider in lbusOrgPlan.iclbOrgPlanProvider)
                        {
                            if (lbusProviderOrgPlanProvider.ibusProviderOrg == null)
                                lbusProviderOrgPlanProvider.LoadProviderOrg();
                            if (lbusProviderOrgPlanProvider.ibusProviderOrg.icdoOrganization.status_value == busConstant.OrganizationStatusActive)
                            {
                                if (lclcActiveProviders.Where(i => i.org_id == lbusProviderOrgPlanProvider.ibusProviderOrg.icdoOrganization.org_id).FirstOrDefault() == null)
                                {
                                    //***************UAT PIR 982 Check for Provider Org Plan Overlapping*************/
                                    if (lbusProviderOrgPlanProvider.ibusProviderOrg.iclbOrgPlan == null)
                                        lbusProviderOrgPlanProvider.ibusProviderOrg.LoadOrgPlan();

                                    var lenuProviderList = lbusProviderOrgPlanProvider.ibusProviderOrg.iclbOrgPlan.Where(
                                                            i => i.icdoOrgPlan.plan_id == icdoPersonAccount.plan_id &&
                                                                 ((busGlobalFunctions.CheckDateOverlapping(DateTime.Now, i.icdoOrgPlan.participation_start_date, i.icdoOrgPlan.participation_end_date))
                                                                 || i.icdoOrgPlan.participation_start_date > DateTime.Now || ablnIsUpdatemode));
                                    if ((lenuProviderList != null) && (lenuProviderList.Count() > 0))
                                    {
                                        lbusProviderOrgPlanProvider.ibusProviderOrg.icdoOrganization.istrProviderOrgName = lbusOrganization.icdoOrganization.istrProviderOrgName;
                                        lclcActiveProviders.Add(lbusProviderOrgPlanProvider.ibusProviderOrg.icdoOrganization);
                                    }
                                    //***************UAT PIR 982 Check for Provider Org Plan Overlapping*************/
                                }
                            }
                        }
                    }
                }
            }
            return lclcActiveProviders;
        }

        //UAT PIR - 589
        //if there is transfer in employment
        //person is not enrolled in the plan provided by the previous employer
        //and tries to enroll in the same plan provided by the new employer
        public bool IsPersonNotEligibleToEnrollInEmploymentTransfer()
        {
            bool lblnResult = false;
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                //UAT PIR 1592 : This validation should fire only at the time of Intial Transfer Employment.. after that it should not fire
                if (ibusPersonEmploymentDetail == null)
                    LoadPersonEmploymentDetail();

                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
                {
                    if (ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl == null)
                        ibusPersonEmploymentDetail.LoadAllPersonAccountEmploymentDetails();

                    if (ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl.Any(i => i.icdoPersonAccountEmploymentDetail.plan_id == icdoPersonAccount.plan_id
                                                                        && i.icdoPersonAccountEmploymentDetail.person_account_id == 0))
                    {
                        if (ibusPerson.ibusPreviousEmployment == null)
                            ibusPerson.LoadPreviousEmployment();

                        if (ibusPerson.ibusPreviousEmployment.icdoPersonEmployment.person_employment_id > 0)
                        {
                            if (ibusPersonEmploymentDetail.IsNull())
                                LoadPersonEmploymentDetail();
                            if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id > 0)
                            {
                                if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                                    ibusPersonEmploymentDetail.LoadPersonEmployment();

                                int lintTransferDays = busGlobalFunctions.DateDiffInDays(ibusPerson.ibusPreviousEmployment.icdoPersonEmployment.end_date_no_null,
                                    ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.start_date);

                                if (lintTransferDays >= 0 && lintTransferDays <= 31)
                                {
                                    //If the Previous Employer Offers the Plan and Member did not take it
                                    if (ibusPerson.ibusPreviousEmployment.ibusOrganization == null)
                                        ibusPerson.ibusPreviousEmployment.LoadOrganization();

                                    if (ibusPerson.ibusPreviousEmployment.ibusOrganization.iclbOrganizationOfferedPlans == null)
                                        ibusPerson.ibusPreviousEmployment.ibusOrganization.LoadOrganizationOfferedPlans();

                                    if (ibusPerson.ibusPreviousEmployment.ibusOrganization.iclbOrganizationOfferedPlans.Any(i => i.icdoOrgPlan.plan_id == icdoPersonAccount.plan_id))
                                    {
                                        //Member is not choosing
                                        bool lblnIsMemberTakenOfferedPlan = false;
                                        if (ibusPerson.ibusPreviousEmployment.icolPersonEmploymentDetail == null)
                                            ibusPerson.ibusPreviousEmployment.LoadPersonEmploymentDetail();

                                        foreach (busPersonEmploymentDetail lbusPAEmpDetail in ibusPerson.ibusPreviousEmployment.icolPersonEmploymentDetail)
                                        {
                                            if (lbusPAEmpDetail.iclbAllPersonAccountEmpDtl == null)
                                                lbusPAEmpDetail.LoadAllPersonAccountEmploymentDetails();

                                            if (lbusPAEmpDetail.iclbAllPersonAccountEmpDtl.Any(i => i.icdoPersonAccountEmploymentDetail.plan_id == icdoPersonAccount.plan_id
                                                                            && i.icdoPersonAccountEmploymentDetail.election_value == busConstant.PersonAccountElectionValueEnrolled))
                                            {
                                                lblnIsMemberTakenOfferedPlan = true;
                                                break;
                                            }
                                        }

                                        if (!lblnIsMemberTakenOfferedPlan)
                                        {
                                            lblnResult = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lblnResult;
        }


        # region 22 cor
        //declare all plans person account object to avoid repeated loading
        public busPersonAccountGhdv ibusPersonAccountGHDVForCor { get; set; }
        public busPersonAccountLife ibusPersonAccountLifeForCor { get; set; }
        public busPersonAccountLtc ibusPersonAccountLTCForCor { get; set; }
        public busPersonAccountLtc ibusPersonAccountEAPForCor { get; set; }
        public busPersonAccountFlexComp ibusPersonAccountFlexCompForCor { get; set; }

        public void LoadPersonAccountByPlan()
        {
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth
                || icdoPersonAccount.plan_id == busConstant.PlanIdDental
                || icdoPersonAccount.plan_id == busConstant.PlanIdVision
                || icdoPersonAccount.plan_id == busConstant.PlanIdHMO)
            {
                if (ibusPersonAccountGHDVForCor.IsNull())
                {
                    ibusPersonAccountGHDVForCor = new busPersonAccountGhdv
                    {
                        icdoPersonAccountGhdv = new cdoPersonAccountGhdv(),
                        icdoPersonAccount = new cdoPersonAccount()
                    };
                }
                ibusPersonAccountGHDVForCor.FindGHDVByPersonAccountID(icdoPersonAccount.person_account_id);
                ibusPersonAccountGHDVForCor.icdoPersonAccount = icdoPersonAccount;
                ibusPersonAccountGHDVForCor.ibusPlan = ibusPlan;
            }
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
            {
                if (ibusPersonAccountLifeForCor.IsNull())
                {
                    ibusPersonAccountLifeForCor = new busPersonAccountLife
                    {
                        icdoPersonAccountLife = new cdoPersonAccountLife(),
                        icdoPersonAccount = new cdoPersonAccount()
                    };
                }
                ibusPersonAccountLifeForCor.FindPersonAccountLife(icdoPersonAccount.person_account_id);
                ibusPersonAccountLifeForCor.icdoPersonAccount = icdoPersonAccount;
                ibusPersonAccountLifeForCor.ibusPlan = ibusPlan;
            }
            if (icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
            {
                if (ibusPersonAccountFlexCompForCor.IsNull())
                {
                    ibusPersonAccountFlexCompForCor = new busPersonAccountFlexComp
                    {
                        icdoPersonAccountFlexComp = new cdoPersonAccountFlexComp(),
                        icdoPersonAccount = new cdoPersonAccount()
                    };
                    ibusPersonAccountFlexCompForCor.FindPersonAccountFlexComp(icdoPersonAccount.person_account_id);
                    ibusPersonAccountFlexCompForCor.icdoPersonAccount = icdoPersonAccount;
                    ibusPersonAccountFlexCompForCor.ibusPlan = ibusPlan;
                }
            }
            if (icdoPersonAccount.plan_id == busConstant.PlanIdLTC)
            {
                if (ibusPersonAccountLTCForCor.IsNull())
                {
                    ibusPersonAccountLTCForCor = new busPersonAccountLtc { icdoPersonAccount = new cdoPersonAccount() };
                }
                ibusPersonAccountLTCForCor.FindPersonAccount(icdoPersonAccount.person_account_id);
                ibusPersonAccountLTCForCor.icdoPersonAccount = icdoPersonAccount;
                ibusPersonAccountLTCForCor.ibusPlan = ibusPlan;
            }
            if (icdoPersonAccount.plan_id == busConstant.PlanIdEAP)
            {
                if (ibusPersonAccountEAPForCor.IsNull())
                {
                    ibusPersonAccountEAPForCor = new busPersonAccountLtc { icdoPersonAccount = new cdoPersonAccount() };
                }
                ibusPersonAccountEAPForCor.FindPersonAccount(icdoPersonAccount.person_account_id);
                ibusPersonAccountEAPForCor.icdoPersonAccount = icdoPersonAccount;
                ibusPersonAccountEAPForCor.ibusPlan = ibusPlan;
            }
        }

        public DateTime idtCancelledEffectiveDate { get; set; }
        public void LoadLatestCancelledEffectiveDate()
        {
            DateTime ldtCancelledDate = DateTime.MinValue;
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth
                || icdoPersonAccount.plan_id == busConstant.PlanIdDental
                || icdoPersonAccount.plan_id == busConstant.PlanIdVision
                || icdoPersonAccount.plan_id == busConstant.PlanIdHMO)
            {
                if (ibusPersonAccountGHDVForCor.IsNull())
                    LoadPersonAccountByPlan();
                ibusPersonAccountGHDVForCor.LoadCancelledEndDate(ref ldtCancelledDate);
            }
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
            {
                if (ibusPersonAccountLifeForCor.IsNull())
                    LoadPersonAccountByPlan();
                ibusPersonAccountLifeForCor.LoadCancelledEndDate(ref ldtCancelledDate);
            }
            if (icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
            {
                if (ibusPersonAccountFlexCompForCor.IsNull())
                    LoadPersonAccountByPlan();
                ibusPersonAccountFlexCompForCor.LoadCancelledEndDate(ref ldtCancelledDate);
            }
            if (icdoPersonAccount.plan_id == busConstant.PlanIdLTC)
            {
                if (ibusPersonAccountLTCForCor.IsNull())
                    LoadPersonAccountByPlan();
                ibusPersonAccountLTCForCor.LoadCancelledEndDate(ref ldtCancelledDate);
            }
            idtCancelledEffectiveDate = ldtCancelledDate;
        }

        public DateTime idtFirstOfMonthSupendendEndDate { get; set; }
        public DateTime idtSupendendStartDate { get; set; }
        //UAT PIR - 2000
        public string istrSupendendStartDateLongDate
        {
            get
            {
                return idtSupendendStartDate.ToString(busConstant.DateFormatLongDate);
            }
        }
        private void LoadSuspendedEffectiveDate()
        {
            DateTime ldtSupendendStartDate = DateTime.MinValue;
            DateTime ldtFirstOfMonthSupendendEndDate = DateTime.MinValue;
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth
                              || icdoPersonAccount.plan_id == busConstant.PlanIdDental
                              || icdoPersonAccount.plan_id == busConstant.PlanIdVision
                              || icdoPersonAccount.plan_id == busConstant.PlanIdHMO)
            {
                if (ibusPersonAccountGHDVForCor.IsNull())
                    LoadPersonAccountByPlan();
                ibusPersonAccountGHDVForCor.LoadSuspendedEndDateForGHDV(ref ldtSupendendStartDate, ref ldtFirstOfMonthSupendendEndDate);
            }

            if (icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
            {
                if (ibusPersonAccountFlexCompForCor.IsNull())
                    LoadPersonAccountByPlan();
                ibusPersonAccountFlexCompForCor.LoadSuspendedEndDateForFlexComp(ref ldtSupendendStartDate, ref ldtFirstOfMonthSupendendEndDate);
            }
            idtFirstOfMonthSupendendEndDate = ldtFirstOfMonthSupendendEndDate.GetFirstDayofNextMonth();
            idtSupendendStartDate = ldtSupendendStartDate;
        }

        public decimal idecTotalTVSC { get; set; }
        public override void LoadCorresProperties(string astrTemplateName)
        {
            if (ibusPlan.IsNull())
                LoadPlan();
            if (ibusPerson.IsNull())
                LoadPerson();
            bool lblnIsPlanJobService = false;
            if (icdoPersonAccount.plan_id == busConstant.PlanIdJobService)
                lblnIsPlanJobService = true;
            idecTotalTVSC = ibusPerson.GetTotalVSCForPerson(lblnIsPlanJobService, DateTime.MinValue);

            LoadPersonAccountByPlan();
            //PIR 17436
            if (astrTemplateName == "PER-0157" && ibusPersonAccountGHDVForCor.IsNotNull())
            {
                LoadCoverageDate();
                if (idtCoverageBeginDate != DateTime.MinValue)
                {
                    ibusPersonAccountGHDVForCor.idtPlanEffectiveDate = idtCoverageBeginDate; //premium should be calculated as of cobra coverage start date.
                    ibusPersonAccountGHDVForCor.iblnPlaneffectiveDateLoaded = true;
                }
            }
            if (ibusPersonAccountGHDVForCor.IsNotNull() && ibusPersonAccountGHDVForCor.ibusOrgPlan.IsNull())
                ibusPersonAccountGHDVForCor.LoadOrgPlan();
            LoadLatestCancelledEffectiveDate();
            LoadLatestEmployment();
            LoadSuspendedEffectiveDate();
			//PIR 17938
            if (ibusPersonAccountGHDVForCor.IsNotNull() && ibusPersonAccountGHDVForCor.IsHealthOrMedicare) 
            {
                ibusPersonAccountGHDVForCor.DetermineEnrollmentAndLoadObjects(ibusPersonAccountGHDVForCor.icdoPersonAccount.current_plan_start_date_no_null, false);
                if (ibusPersonAccountGHDVForCor.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                {
                    ibusPersonAccountGHDVForCor.LoadRateStructureForUserStructureCode();
                }
                else
                {
                    //Load the Health Plan Participation Date (based on effective Date)
                    ibusPersonAccountGHDVForCor.LoadHealthParticipationDate();
                    //To Get the Rate Structure Code (Derived Field)
                    ibusPersonAccountGHDVForCor.LoadRateStructure();
                }
                //Get the Coverage Ref ID
                ibusPersonAccountGHDVForCor.LoadCoverageRefID();                
            }
            LoadPremiumMonthlyAmount();
            LoadCoverageDate();
            LoadLastContributionDate();
            //  GetNormalRetirementDate();
            GetCoverageCodeBasedOnPlans();
        }

        public override busBase GetCorOrganization()
        {
            if (ibusPersonLatestEmployment.IsNull())
                LoadLatestEmployment();

            return ibusPersonLatestEmployment.ibusOrganization;
        }

        //load the latest employment
        public busPersonEmployment ibusPersonLatestEmployment { get; set; }
        public void LoadLatestEmployment()
        {
            if (ibusPersonLatestEmployment.IsNull())
                ibusPersonLatestEmployment = new busPersonEmployment { icdoPersonEmployment = new cdoPersonEmployment() };

            busPersonEmploymentDetail lobjPersonLatestEmployment = GetLatestEmploymentDetail();

            if (lobjPersonLatestEmployment.ibusPersonEmployment.IsNull())
                lobjPersonLatestEmployment.LoadPersonEmployment();

            ibusPersonLatestEmployment = lobjPersonLatestEmployment.ibusPersonEmployment;

            if (ibusPersonLatestEmployment.ibusOrganization.IsNull())
                ibusPersonLatestEmployment.LoadOrganization();
        }

        public string istrCoverageCode { get; set; }
        public void GetCoverageCodeBasedOnPlans()
        {
            string lstrCoverageCode = string.Empty;
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth
                              || icdoPersonAccount.plan_id == busConstant.PlanIdDental
                              || icdoPersonAccount.plan_id == busConstant.PlanIdVision
                              || icdoPersonAccount.plan_id == busConstant.PlanIdHMO)
            {
                if (ibusPersonAccountGHDVForCor.IsNull())
                    LoadPersonAccountByPlan();
                ibusPersonAccountGHDVForCor.LoadCoverageCodeDescription();
                istrCoverageCode = ibusPersonAccountGHDVForCor.istrCoverageCode;
            }

            if (icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
            {
                if (ibusPersonAccountFlexCompForCor.IsNull())
                    LoadPersonAccountByPlan();
                ibusPersonAccountFlexCompForCor.LoadFlexCompOptionUpdate();
                var lFlexCompOtion = ibusPersonAccountFlexCompForCor.iclbFlexCompOption
                    .Where(lobjFlexComp => lobjFlexComp.icdoPersonAccountFlexCompOption.level_of_coverage_value != busConstant.FlexLevelOfCoverageDependentSpending);
                if (lFlexCompOtion.Count() > 0)
                    istrCoverageCode = lFlexCompOtion.First().icdoPersonAccountFlexCompOption.level_of_coverage_description;
            }
        }

        public DateTime idtEnrolmentEndPlusONeDay
        {
            get
            {
                DateTime ldtResultDate = DateTime.MinValue;
                // if (icdoPersonAccount.end_date != DateTime.MinValue)
                //ldtResultDate = icdoPersonAccount.end_date.AddDays(1);
                if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth
                    || icdoPersonAccount.plan_id == busConstant.PlanIdDental
                                       || icdoPersonAccount.plan_id == busConstant.PlanIdVision
                                       || icdoPersonAccount.plan_id == busConstant.PlanIdHMO)
                {
                    if (ibusPersonAccountGHDVForCor.IsNull())
                        LoadPersonAccountByPlan();

                    ibusPersonAccountGHDVForCor.LoadPreviousHistory();
                    if (ibusPersonAccountGHDVForCor.ibusHistory.icdoPersonAccountGhdvHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended)
                        ldtResultDate = ibusPersonAccountGHDVForCor.ibusHistory.icdoPersonAccountGhdvHistory.start_date;
                }
                else if (icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
                {
                    if (ibusPersonAccountFlexCompForCor.IsNull())
                        LoadPersonAccountByPlan();

                    ibusPersonAccountFlexCompForCor.LoadFlexCompHistory();
                    if (ibusPersonAccountFlexCompForCor.iclbFlexCompHistory.Count() > 0)
                    {
                        if (ibusPersonAccountFlexCompForCor.iclbFlexCompHistory[0].icdoPersonAccountFlexCompHistory.plan_participation_status_value == busConstant.PlanParticipationStatusFlexSuspended)
                            ldtResultDate = ibusPersonAccountFlexCompForCor.iclbFlexCompHistory[0].icdoPersonAccountFlexCompHistory.effective_start_date;
                    }
                }
                return ldtResultDate;
            }
        }

        //load the premium amount for FlexComp, GDV
        public decimal idecMonthlyPremiumAmountByPlan { get; set; }
        public void LoadPremiumMonthlyAmount()
        {
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
            {
                if (ibusPersonAccountGHDVForCor.IsNull())
                    LoadPersonAccountByPlan();

                ibusPersonAccountGHDVForCor.LoadActiveProviderOrgPlan(ibusPersonAccountGHDVForCor.icdoPersonAccount.current_plan_start_date_no_null);

                if (ibusPersonAccountGHDVForCor.icdoPersonAccountGhdv.overridden_structure_code.IsNotNullOrEmpty())
                {
                    ibusPersonAccountGHDVForCor.LoadRateStructureForUserStructureCode();
                }
                else
                {
                    ibusPersonAccountGHDVForCor.LoadHealthParticipationDate();
                    ibusPersonAccountGHDVForCor.LoadRateStructure();
                }
                ibusPersonAccountGHDVForCor.LoadCoverageRefID();
                ibusPersonAccountGHDVForCor.GetMonthlyPremiumAmountByRefID();
                idecMonthlyPremiumAmountByPlan = ibusPersonAccountGHDVForCor.icdoPersonAccountGhdv.MonthlyPremiumAmount;
            }
            if (icdoPersonAccount.plan_id == busConstant.PlanIdDental
                                   || icdoPersonAccount.plan_id == busConstant.PlanIdVision
                                   || icdoPersonAccount.plan_id == busConstant.PlanIdHMO)
            {
                if (ibusPersonAccountGHDVForCor.IsNull())
                    LoadPersonAccountByPlan();
                //PIR 18038 - loaded providerOrgPlan with plan effective date
                if ((ibusPersonAccountGHDVForCor.istrIsPlanDental == busConstant.Flag_Yes && ibusPersonAccountGHDVForCor.icdoPersonAccountGhdv.dental_insurance_type_value != busConstant.DentalInsuranceTypeRetiree)
                     || (ibusPersonAccountGHDVForCor.istrIsPlanVision == busConstant.Flag_Yes && ibusPersonAccountGHDVForCor.icdoPersonAccountGhdv.vision_insurance_type_value != busConstant.VisionInsuranceTypeRetiree))
                {
                    if (!ibusPersonAccountGHDVForCor.iblnPlaneffectiveDateLoaded)
                        ibusPersonAccountGHDVForCor.LoadPlanEffectiveDate();
                    ibusPersonAccountGHDVForCor.DetermineEnrollmentAndLoadObjects(ibusPersonAccountGHDVForCor.idtPlanEffectiveDate, false);
                }
                else
                {
                    //If person employment detail does not exist
                    ibusPersonAccountGHDVForCor.LoadActiveProviderOrgPlan(DateTime.Now);
                }                
                ibusPersonAccountGHDVForCor.GetMonthlyPremiumAmount();
                idecMonthlyPremiumAmountByPlan = ibusPersonAccountGHDVForCor.icdoPersonAccountGhdv.MonthlyPremiumAmount;
            }
            if (icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
            {
                if (ibusPersonAccountFlexCompForCor.IsNull())
                    LoadPersonAccountByPlan();
                ibusPersonAccountFlexCompForCor.LoadFlexCompOptionUpdate();
                var lFlexCompOtion = ibusPersonAccountFlexCompForCor.iclbFlexCompOption
                    .Where(lobjFlexComp => lobjFlexComp.icdoPersonAccountFlexCompOption.level_of_coverage_value == busConstant.FlexLevelOfCoverageMedicareSpending);
                if (lFlexCompOtion.Count() > 0)
                    idecMonthlyPremiumAmountByPlan = (lFlexCompOtion.First().icdoPersonAccountFlexCompOption.annual_pledge_amount / 12);
            }
        }
        //load coverage date
        public DateTime idtCoverageBeginDate { get; set; }
        public DateTime idtCoverageEndDate { get; set; }
        public DateTime idtEnrolledCoverageEndDate { get; set; }
        public string istrCoverageBeginDateLongFormat
        {
            get
            {
                return idtCoverageBeginDate == DateTime.MinValue ? string.Empty : idtCoverageBeginDate.ToString(busConstant.DateFormatLongDate);
            }
        }
        public string istrCoverageEndDateLongFormat
        {
            get
            {
                return idtCoverageEndDate == DateTime.MinValue ? string.Empty : idtCoverageEndDate.ToString(busConstant.DateFormatLongDate);
            }
        }
        public void LoadCoverageDate()
        {
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth
                                         || icdoPersonAccount.plan_id == busConstant.PlanIdDental
                                         || icdoPersonAccount.plan_id == busConstant.PlanIdVision
                                         || icdoPersonAccount.plan_id == busConstant.PlanIdHMO)
            {
                if (ibusPersonAccountGHDVForCor.IsNull())
                    LoadPersonAccountByPlan();
                ibusPersonAccountGHDVForCor.LoadCoverageDateForGHDV();
                idtCoverageBeginDate = ibusPersonAccountGHDVForCor.idtCoverageBeginDate;
                idtCoverageEndDate = ibusPersonAccountGHDVForCor.idtCoverageEndDate;
            }

            if (icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
            {
                if (ibusPersonAccountFlexCompForCor.IsNull())
                    LoadPersonAccountByPlan();
                ibusPersonAccountFlexCompForCor.LoadCoverageDateForFlex();
                idtCoverageBeginDate = ibusPersonAccountFlexCompForCor.idtCoverageBeginDate;
                idtCoverageEndDate = ibusPersonAccountFlexCompForCor.idtCoverageEndDate;
            }
        }
        //last contribution date for this person in retirement plan
        public DateTime idtLastContributionDate { get; set; }
        public decimal idecLastContributionWages = 0.00M; //PIR 25920

        public void LoadLastContributionDate()
        {
            if (iclbRetirementContributionAll.IsNull())
                LoadRetirementContributionAll();

            var lRetirementContribution = iclbRetirementContributionAll.OrderByDescending(lobjContribution => lobjContribution.icdoPersonAccountRetirementContribution.effective_date);

            if (lRetirementContribution.Count() > 0)
            {
                idtLastContributionDate = lRetirementContribution.FirstOrDefault().icdoPersonAccountRetirementContribution.effective_date;
                idecLastContributionWages = lRetirementContribution.FirstOrDefault().icdoPersonAccountRetirementContribution.salary_amount;
            }
        }
        /// <summary>
        /// PIR 25920 DC 2025 changes get the last contribution and wages for regular Payroll
        /// </summary>
        public void LoadLastContributionDateByRegularPayroll()
        {
            if (iclbRetirementContributionAll.IsNull())
                LoadRetirementContributionAll();

            var lRetirementContribution = iclbRetirementContributionAll.Where(lobjContribution => lobjContribution.icdoPersonAccountRetirementContribution.transaction_type_value == busConstant.TransactionTypeRegularPayroll)?.OrderByDescending(lobjContribution => lobjContribution.icdoPersonAccountRetirementContribution.effective_date);

            if (lRetirementContribution.Count() > 0)
            {
                idtLastContributionDate = lRetirementContribution.FirstOrDefault().icdoPersonAccountRetirementContribution.effective_date;
                idecLastContributionWages = lRetirementContribution.FirstOrDefault().icdoPersonAccountRetirementContribution.salary_amount;
            }
        }

        #endregion

        #region UCS 24
        public string istrMSSElectionStatus { get; set; }

        public busWssPersonAccountEnrollmentRequest ibusWSSPersonAccountEnrollmentRequest { get; set; }
        public void LoadWSSEnrollmentRequestUpdate(int aintRequestId)
        {
            if (ibusWSSPersonAccountEnrollmentRequest.IsNull())
                ibusWSSPersonAccountEnrollmentRequest = new busWssPersonAccountEnrollmentRequest();

            ibusWSSPersonAccountEnrollmentRequest.FindWssPersonAccountEnrollmentRequest(aintRequestId);
        }
        # endregion

        #region UCS 55
        public Collection<busBenefitRhicCombineHealthSplit> iclbBenefitRhicCombineHealthSplit { get; set; }
        public void LoadBenefitRhicCombineHealthSplit()
        {
            if (iclbBenefitRhicCombineHealthSplit == null)
                iclbBenefitRhicCombineHealthSplit = new Collection<busBenefitRhicCombineHealthSplit>();

            DataTable ldtbList = Select<cdoBenefitRhicCombineHealthSplit>(
                                      new string[1] { "person_account_id" },
                                      new object[1] { icdoPersonAccount.person_account_id }, null, null);
            iclbBenefitRhicCombineHealthSplit = GetCollection<busBenefitRhicCombineHealthSplit>(ldtbList, "icdoBenefitRhicCombineHealthSplit");
        }

        public decimal LoadTotalRhicAllocatedAmount(DateTime adtEffectiveDate)
        {
            decimal ldecJsRhicAmount = 0.00M;
            decimal ldecOtherRhicAmount = 0.00M;
            return LoadTotalRhicAllocatedAmount(adtEffectiveDate, ref ldecJsRhicAmount, ref ldecOtherRhicAmount);
        }

        public decimal LoadTotalRhicAllocatedAmount(DateTime adtEffectiveDate, ref decimal adecJsRhicAmount, ref decimal adecOtherRhicAmount)
        {
            //icdoPersonAccount.person_account_id = 186937;

            decimal ldecTotalRhicAmount = 0.00M;
            if (iclbBenefitRhicCombineHealthSplit == null)
                LoadBenefitRhicCombineHealthSplit();

            foreach (busBenefitRhicCombineHealthSplit lbusTempBenRhicCombineHealthSplit in iclbBenefitRhicCombineHealthSplit)
            {
                if (lbusTempBenRhicCombineHealthSplit.ibusBenefitRhicCombine == null)
                    lbusTempBenRhicCombineHealthSplit.LoadBenefitRhicCombine();
            }

            busBenefitRhicCombineHealthSplit lbusBenRhicCombineHealthSplit = iclbBenefitRhicCombineHealthSplit.Where(i => busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate,
                                                                                i.ibusBenefitRhicCombine.icdoBenefitRhicCombine.start_date,
                                                                                i.ibusBenefitRhicCombine.icdoBenefitRhicCombine.end_date)
                                                                                && i.ibusBenefitRhicCombine.icdoBenefitRhicCombine.status_value == busConstant.RHICStatusValid
                                                                                &&
                                                                                (i.ibusBenefitRhicCombine.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusApproved
                                                                                || i.ibusBenefitRhicCombine.icdoBenefitRhicCombine.action_status_value == busConstant.RHICActionStatusEnded)
                                                                                )
                                                                                .OrderByDescending(i => i.ibusBenefitRhicCombine.icdoBenefitRhicCombine.end_date_no_null)
                                                                                .FirstOrDefault();
            if (lbusBenRhicCombineHealthSplit != null)
            {
                adecJsRhicAmount = lbusBenRhicCombineHealthSplit.icdoBenefitRhicCombineHealthSplit.js_rhic_amount;
                adecOtherRhicAmount = lbusBenRhicCombineHealthSplit.icdoBenefitRhicCombineHealthSplit.other_rhic_amount;
                ldecTotalRhicAmount = adecJsRhicAmount + adecOtherRhicAmount;
            }

            return ldecTotalRhicAmount;
        }
        #endregion

        #region Missed Deposit
        public void LoadMissedDeposits()
        {
            iclbPersonAccountMissedDeposit = new Collection<busPersonAccountMissedDeposit>();
            Collection<busPersonAccountMissedDeposit> lclbPersonAccountMissedDepositTemp = new Collection<busPersonAccountMissedDeposit>();
            if (iclbAccountEmploymentDetail == null)
                LoadPersonAccountEmploymentDetails();

            if (iclbRetirementContributionAll == null)
                LoadRetirementContributionAll();

            foreach (busPersonAccountEmploymentDetail lbusPAEmpDetail in iclbAccountEmploymentDetail)
            {
                if (lbusPAEmpDetail.ibusEmploymentDetail == null)
                    lbusPAEmpDetail.LoadPersonEmploymentDetail();
                if (lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.status_value != busConstant.EmploymentStatusNonContributing)
                {
                    DateTime ldtStartDate = lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.start_date;
                    DateTime ldtEndDate = DateTime.Now.GetFirstDayofCurrentMonth();
                    if (lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date != DateTime.MinValue)
                        ldtEndDate = lbusPAEmpDetail.ibusEmploymentDetail.icdoPersonEmploymentDetail.end_date;
                    while (ldtStartDate <= ldtEndDate)
                    {
                        if (iclbRetirementContributionAll.Where(i => i.icdoPersonAccountRetirementContribution.pay_period_month == ldtStartDate.Month &&
                                                                    i.icdoPersonAccountRetirementContribution.pay_period_year == ldtStartDate.Year)
                                                        .Sum(i => i.icdoPersonAccountRetirementContribution.salary_amount) <= 0)
                        {
                            if (!lclbPersonAccountMissedDepositTemp.Any(i => i.icdoPersonAccountMissedDeposit.pay_period_month == ldtStartDate.Month &&
                                i.icdoPersonAccountMissedDeposit.pay_period_year == ldtStartDate.Year))
                            {
                                busPersonAccountMissedDeposit lbusPAMissedDeposit = new busPersonAccountMissedDeposit();
                                lbusPAMissedDeposit.icdoPersonAccountMissedDeposit = new cdoPersonAccountMissedDeposit();
                                lbusPAMissedDeposit.icdoPersonAccountMissedDeposit.person_account_id = icdoPersonAccount.person_account_id;
                                lbusPAMissedDeposit.icdoPersonAccountMissedDeposit.pay_period_month = ldtStartDate.Month;
                                lbusPAMissedDeposit.icdoPersonAccountMissedDeposit.pay_period_year = ldtStartDate.Year;
                                //ESS Backlog PIR 11786
                                lbusPAMissedDeposit.icdoPersonAccountMissedDeposit.pay_period_year_and_month = new DateTime(lbusPAMissedDeposit.icdoPersonAccountMissedDeposit.pay_period_year,
                                                                                                                              lbusPAMissedDeposit.icdoPersonAccountMissedDeposit.pay_period_month, 01);
                                lclbPersonAccountMissedDepositTemp.Add(lbusPAMissedDeposit);
                            }
                        }
                        ldtStartDate = ldtStartDate.AddMonths(1);
                    }
                }
            }
            //Oct 2010 forward only – do not consider anything prior to Oct 2010 as a Missed Deposit
            DateTime ldtNDPERSGOLiveDate = new DateTime(2010, 10, 01);
            lclbPersonAccountMissedDepositTemp = lclbPersonAccountMissedDepositTemp.Where(i => i.icdoPersonAccountMissedDeposit.pay_period_year_and_month.Date >= ldtNDPERSGOLiveDate.Date)
                                                                                    .ToList().ToCollection();
            //If contribution records exist of purchase subsystem value purchase(LOA, SEAS), do not show as Missed Deposit.
            var lenuPurchasedContributions = iclbRetirementContributionAll
                                                .Where(c => c.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueServicePurchase
                                                        && c.icdoPersonAccountRetirementContribution.subsystem_ref_id > 0);
            if (lenuPurchasedContributions.Count() > 0)
            {
                DataTable ldtbPurchasedLOASEASRetContributions = Select("cdoPersonAccount.LoadPurcLOASEASRetContriEffDates",
                                                            new object[1] { icdoPersonAccount.person_account_id });
                if (ldtbPurchasedLOASEASRetContributions.Rows.Count > 0)
                {
                    var lenmLOASEASEffDates = ldtbPurchasedLOASEASRetContributions
                                                .AsEnumerable()
                                                .Select(i => i.Field<DateTime>("EFFECTIVE_DATE").GetFirstDayofCurrentMonth());
                    if (lenmLOASEASEffDates.Count() > 0)
                    {
                        foreach (busPersonAccountMissedDeposit lbusPersonAccountMissedDeposit in lclbPersonAccountMissedDepositTemp)
                        {
                            if (!(lenmLOASEASEffDates.Any(i => i.Date ==
                                lbusPersonAccountMissedDeposit.icdoPersonAccountMissedDeposit.pay_period_year_and_month.Date)))
                                iclbPersonAccountMissedDeposit.Add(lbusPersonAccountMissedDeposit);
                        }
                    }
                    else
                    {
                        iclbPersonAccountMissedDeposit = lclbPersonAccountMissedDepositTemp;
                    }
                }
                else
                {
                    iclbPersonAccountMissedDeposit = lclbPersonAccountMissedDepositTemp;
                }
            }
            else
            {
                iclbPersonAccountMissedDeposit = lclbPersonAccountMissedDepositTemp;
            }
        }
        #endregion

        public void LoadPersonAccountRetirement()
        {
            if (ibusPersonAccountRetirement == null)
                ibusPersonAccountRetirement = new busPersonAccountRetirement();
            ibusPersonAccountRetirement.icdoPersonAccount = icdoPersonAccount;
            ibusPersonAccountRetirement.FindPersonAccountRetirement(icdoPersonAccount.person_account_id);
        }

        public void LoadPersonAccountGHDV()
        {
            if (ibusPersonAccountGHDV == null)
                ibusPersonAccountGHDV = new busPersonAccountGhdv();
            ibusPersonAccountGHDV.icdoPersonAccount = icdoPersonAccount;
            ibusPersonAccountGHDV.FindGHDVByPersonAccountID(icdoPersonAccount.person_account_id);
        }

        //PIR 18503 - Wizard changes
        public busPersonAccount ibusPersonAccount { get; set; }
        public void LoadPersonAccountMedicare(int iintPersonAccountID)
        {
            if (ibusPersonAccount.IsNull())
                ibusPersonAccount = new busPersonAccount { icdoPersonAccount = new cdoPersonAccount() };
            ibusPersonAccount.FindPersonAccount(iintPersonAccountID);
        }

        //Prod PIR: 4606.  Method to load FlexCompPersonAccount Added.
        public busPersonAccountFlexComp ibusPersonAccountFlex { get; set; }
        public void LoadPersonAccountFlex()
        {
            if (ibusPersonAccountFlex == null)
                ibusPersonAccountFlex = new busPersonAccountFlexComp();
            ibusPersonAccountFlex.icdoPersonAccount = icdoPersonAccount;
            ibusPersonAccountFlex.FindPersonAccountFlexComp(icdoPersonAccount.person_account_id);
        }

        public busPersonAccountLife ibusPersonAccountLife { get; set; }
        public void LoadPersonAccountLife()
        {
            if (ibusPersonAccountLife == null)
                ibusPersonAccountLife = new busPersonAccountLife();
            ibusPersonAccountLife.icdoPersonAccount = icdoPersonAccount;
            ibusPersonAccountLife.FindPersonAccountLife(icdoPersonAccount.person_account_id);
        }

        public busPersonAccountLtc ibusPersonAccountLtc { get; set; }
        public void LoadPersonAccountLtc()
        {
            if (ibusPersonAccountLtc == null)
                ibusPersonAccountLtc = new busPersonAccountLtc();
            ibusPersonAccountLtc.icdoPersonAccount = icdoPersonAccount;
        }

        public busPersonAccountEAP ibusPersonAccountEAP { get; set; }
        public void LoadPersonAccountEAP()
        {
            if (ibusPersonAccountEAP == null)
                ibusPersonAccountEAP = new busPersonAccountEAP();
            ibusPersonAccountEAP.icdoPersonAccount = icdoPersonAccount;
        }

        public DataTable LoadMemberWithWagesAndServiceCredit()=> Select("cdoPersonAccount.MembersWithServiceCreditandWages", new object[1] { icdoPersonAccount.person_account_id });
     
        //this is used in the correspondence SFN-51501
        public DateTime idtNormalRetirementDate { get; set; }
        public string istrNRDMonthYear { get; set; }
        //public void GetNormalRetirementDate()
        //{
        //  idtNormalRetirementDate =  GetNormalRetirementDateBasedOnNormalEligibility(icdoPersonAccount.plan_id, ibusPlan.icdoPlan.plan_code,
        //        ibusPlan.icdoPlan.benefit_provision_id, busConstant.ApplicationBenefitTypeRetirement, ibusPerson.icdoPerson.date_of_birth, idecTotalTVSC, 0, iobjPassInfo,
        //        ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date, icdoPersonAccount.person_account_id, false,0.00M);
        //  istrNRDMonthYear = idtNormalRetirementDate.Month.ToString() + "/" + idtNormalRetirementDate.Year;
        //}

        public string istrdtEnrollmentEndDate
        {
            get
            {
                return idtCancelledEffectiveDate.ToString(busConstant.DateFormatLongDate);
            }
        }
        public string istrdtEnrollmentStartDate
        {
            get
            {
                return icdoPersonAccount.start_date.ToString(busConstant.DateFormatLongDate);
            }
        }

        public string istrIsPlanDC
        {
            get
            {
                return (icdoPersonAccount.plan_id == busConstant.PlanIdDC || icdoPersonAccount.plan_id == busConstant.PlanIdDC2020) ? busConstant.Flag_Yes : busConstant.Flag_No; //PIR 20232
            }
        }
        public string istrIsPlan457
        {
            get
            {
                return icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation ? busConstant.Flag_Yes : busConstant.Flag_No;
            }
        }

        /// <summary>
        /// uat pir 2373 : method to set the PS event value based on employment hdr end date
        /// </summary>
        public void SetPersonAcccountForTeminationChange()
        {
            bool lblnOpenEmployment = false;
            if (iclbAccountEmploymentDetail == null)
                LoadPersonAccountEmploymentDetails();
            foreach (busPersonAccountEmploymentDetail lobjPAEmpDtl in iclbAccountEmploymentDetail)
            {
                if (lobjPAEmpDtl.ibusEmploymentDetail == null)
                    lobjPAEmpDtl.LoadPersonEmploymentDetail();
                if (lobjPAEmpDtl.ibusEmploymentDetail.ibusPersonEmployment == null)
                    lobjPAEmpDtl.ibusEmploymentDetail.LoadPersonEmployment();
                if (lobjPAEmpDtl.ibusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.end_date == DateTime.MinValue)
                {
                    lblnOpenEmployment = true;
                    break;
                }
            }
            if (lblnOpenEmployment)
            {
                icdoPersonAccount.ps_file_change_event_value = busConstant.PlanChangedToCancWithEmpHdrOpen;
                //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
            }
            else
            {
                icdoPersonAccount.ps_file_change_event_value = busConstant.EmploymentHdrEnddated;
                //icdoPersonAccount.people_soft_file_sent_flag = busConstant.Flag_No;//PIR 17081
            }
        }

        //Prod PIR: 4154
        //Method to Include the Service Credit Posted in Retirment Contribution Table Posted after Retirement date.
        public Decimal GetTotalServicePurchaseCreditPostedAfterRetirementDate(bool ablnFetchPSC, DateTime adtRetirementDate)
        {
            decimal idecSCCount = 0.0M;
            if (adtRetirementDate != DateTime.MinValue)
            {
                if (iclbRetirementContributionAll.IsNull())
                    LoadRetirementContributionAll();

                if (ablnFetchPSC)
                {
                    idecSCCount = iclbRetirementContributionAll.Where(o => (o.icdoPersonAccountRetirementContribution.effective_date >= adtRetirementDate)
                                && (o.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueServicePurchase)).Sum(o => o.icdoPersonAccountRetirementContribution.pension_service_credit);
                }
                else
                {
                    idecSCCount = iclbRetirementContributionAll.Where(o => (o.icdoPersonAccountRetirementContribution.effective_date >= adtRetirementDate)
                        && (o.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueServicePurchase)).Sum(o => o.icdoPersonAccountRetirementContribution.vested_service_credit);
                }
            }
            return idecSCCount;
        }

        //UAT PIR 2220
        public ObjectState istrObjectState { get; set; }
        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            istrObjectState = icdoPersonAccount.ienuObjectState;
            if (ibusPaymentElection != null && ibusPaymentElection.icdoPersonAccountPaymentElection != null)//added to fix pir 6287
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.person_account_id = icdoPersonAccount.person_account_id;//PIR 7671            
            }
           
            base.BeforeValidate(aenmPageMode);
        }

        /// <summary>
        /// Prod Pir - 4613
        /// Method to get the due premium amount for IBS Delinquent Letter
        /// </summary>
        /// <param name="adtEffectiveDate">Effective Date</param>
        public void LoadInsurancePremiumForIBSDelinquentLetter(DateTime adtEffectiveDate)
        {
            DataTable ldtInsuranceContribution = SelectWithOperator<cdoPersonAccountInsuranceContribution>(new string[4]{"person_account_id",
                "effective_date","effective_date","transaction_type_value"}, new string[4] { "=", ">=", "<=","<>" }, new object[4]{icdoPersonAccount.person_account_id,
                    adtEffectiveDate.GetFirstDayofCurrentMonth(),adtEffectiveDate.GetLastDayofMonth(),busConstant.TransactionTypeAdjustmentIBS}, null);
            DataTable ldtInsuranceContributionIBAP = SelectWithOperator<cdoPersonAccountInsuranceContribution>(new string[4]{"person_account_id",
                "transaction_date","transaction_date","transaction_type_value"}, new string[4] { "=", ">=", "<=", "=" }, new object[4]{icdoPersonAccount.person_account_id,
                    adtEffectiveDate.GetFirstDayofCurrentMonth(),adtEffectiveDate.GetLastDayofMonth(),busConstant.PersonAccountTransactionTypeIBSAdjPayment}, null);
            ldtInsuranceContribution.Merge(ldtInsuranceContributionIBAP);
            iclbInsuranceContribution = GetCollection<busPersonAccountInsuranceContribution>(ldtInsuranceContribution, "icdoPersonAccountInsuranceContribution");
            idecPremiumDue = iclbInsuranceContribution.Sum(o => o.icdoPersonAccountInsuranceContribution.due_premium_amount);
            idecPaidAmount = iclbInsuranceContribution.Sum(o => o.icdoPersonAccountInsuranceContribution.paid_premium_amount);
        }

        /// <summary>
        /// PROD PIR 7330 :method to get total vsc as of statement effective date in MAS 
        /// </summary>
        /// <param name="adtEffectiveDate">statement effective date</param>
        public void LoadTotalVSCForMAS(DateTime adtEffectiveDate)
        {
            decimal ldecVSC = 0.00M;
            decimal ldecTFFRService = 0.00M;
            decimal ldecTIAAService = 0.00M;
            //PIR 791
            //Check for TIAA , TFFR service amount ,if exists with approved status ,add it to TVSC
            LoadTFFRTIAAService(ref ldecTFFRService, ref ldecTIAAService);

            //Check if any PSC, VSC already posted for this month
            if (idtRetirementContributionAll == null)
            {
                LoadRetirementContributionAllDataTable();
            }
            foreach (DataRow ldrRow in idtRetirementContributionAll.Rows)
            {
                if (ldrRow["effective_date"] != DBNull.Value && Convert.ToDateTime(ldrRow["effective_date"]) <= adtEffectiveDate.GetLastDayofMonth())
                {
                    // PIR ID 1886 - Exclude SubSystemValueBenefitPayment
                    if (!Convert.IsDBNull(ldrRow["SUBSYSTEM_VALUE"]))
                    {
                        if ((ldrRow["SUBSYSTEM_VALUE"]).ToString() != busConstant.SubSystemValueBenefitPayment)
                        {
                            if (!Convert.IsDBNull(ldrRow["VESTED_SERVICE_CREDIT"]))
                                ldecVSC += (decimal)ldrRow["VESTED_SERVICE_CREDIT"];
                        }
                    }
                }
            }
            icdoPersonAccount.Total_VSC = ldecVSC + ldecTFFRService + ldecTIAAService;
        }

        //prod pir 7330
        public DateTime idtMASStatementEffectiveDate { get; set; }

        //Update Change History Date

        public int UH_Person_ID { get; set; }
        public int UH_Plan_ID { get; set; }
        public DateTime UH_Old_Change_Date { get; set; }
        public DateTime UH_New_Change_Date { get; set; }
        public string UH_User_ID { get; set; }
        public int UH_LVL_COVERAGE { get; set; }
        public ArrayList btn_Update_History()
        {
            ArrayList larrList = new ArrayList();
            utlError lobjError = null;
            
            DataTable ldtPersonAccount = Select<cdoPersonAccount>(new string[2] { "person_id", "plan_id" },
                                                                    new object[2] { UH_Person_ID, UH_Plan_ID }, null, null);
            if (ldtPersonAccount.Rows.Count == 0)
            {
                lobjError = AddError(1068, "");
                larrList.Add(lobjError);
                return larrList;
            }
           if(Convert.ToDateTime(ldtPersonAccount.Rows[0]["history_change_date"])!= DateTime.MinValue && 
               Convert.ToDateTime(ldtPersonAccount.Rows[0]["history_change_date"]) != UH_Old_Change_Date)
            {
                lobjError = AddError(1070, "");
                larrList.Add(lobjError);
                return larrList;
            }
            
            UH_User_ID = iobjPassInfo.istrUserID;

            int lintPersonAccountID = busPersonAccountHelper.GetPersonAccountID(UH_Plan_ID, UH_Person_ID);
            FindPersonAccount(lintPersonAccountID);
            icdoPersonAccount.is_From_Update_History_Form = true;
            icdoPersonAccount.history_change_date = UH_New_Change_Date;
            icdoPersonAccount.Update();
            DBFunction.DBNonQuery("cdoPersonAccount.UpdateHistory", new object[6] { UH_Plan_ID, UH_New_Change_Date, UH_User_ID, UH_Person_ID, UH_Old_Change_Date, UH_LVL_COVERAGE },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            larrList.Add(this);
            return larrList;
        }

        //PIR 12547
        public void LoadPersonAccountEmpDetailByPlan()
        {
            if (_iclbPersonAccountEmpDetailByPlan == null)
                _iclbPersonAccountEmpDetailByPlan = new Collection<busPersonAccountEmploymentDetail>();

            DataTable ldtbList = Select("cdoPersonAccount.GetEmploymentDetailIDByPlan", new object[2] { icdoPersonAccount.plan_id, icdoPersonAccount.person_id });

            foreach (DataRow dr in ldtbList.Rows)
            {
                busPersonAccountEmploymentDetail lobjPersonAccountEmpDetail = new busPersonAccountEmploymentDetail
                {
                    icdoPersonAccountEmploymentDetail = new cdoPersonAccountEmploymentDetail()
                };
                lobjPersonAccountEmpDetail.icdoPersonAccountEmploymentDetail.LoadData(dr);

                iclbPersonAccountEmpDetailByPlan.Add(lobjPersonAccountEmpDetail);
            }
        }
		
		//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
        public bool IsIbsCollectionSame(cdoIbsDetail lcdoPosNeg, cdoIbsDetail lcdoCompare, bool IsCompareAbsolute)
        {
            if (lcdoPosNeg.plan_id == busConstant.PlanIdGroupHealth)
            {
                if (lcdoPosNeg.mode_of_payment_value == lcdoCompare.mode_of_payment_value &&
                    lcdoPosNeg.person_account_id == lcdoCompare.person_account_id &&
                    lcdoPosNeg.group_number == lcdoCompare.group_number &&
                    lcdoPosNeg.provider_org_id == lcdoCompare.provider_org_id &&
                    lcdoPosNeg.rate_structure_code == lcdoCompare.rate_structure_code &&
                    lcdoPosNeg.coverage_code == lcdoCompare.coverage_code)
                {

                    if (IsCompareAbsolute == true &&
                        Math.Abs(lcdoPosNeg.member_premium_amount) == Math.Abs(lcdoCompare.member_premium_amount) &&
                        Math.Abs(lcdoPosNeg.total_premium_amount) == Math.Abs(lcdoCompare.total_premium_amount) &&
                        Math.Abs(lcdoPosNeg.group_health_fee_amt) == Math.Abs(lcdoCompare.group_health_fee_amt))
                        return true;
                    else if (IsCompareAbsolute == false &&
                    lcdoPosNeg.member_premium_amount == lcdoCompare.member_premium_amount &&
                    lcdoPosNeg.total_premium_amount == lcdoCompare.total_premium_amount &&
                    lcdoPosNeg.group_health_fee_amt == lcdoCompare.group_health_fee_amt)
                        return true;
                }
            }
            else if (lcdoPosNeg.plan_id == busConstant.PlanIdDental || lcdoPosNeg.plan_id == busConstant.PlanIdVision)
            {
                if (lcdoPosNeg.mode_of_payment_value == lcdoCompare.mode_of_payment_value &&
                    lcdoPosNeg.person_account_id == lcdoCompare.person_account_id &&
                    lcdoPosNeg.group_number == lcdoCompare.group_number &&
                    lcdoPosNeg.provider_org_id == lcdoCompare.provider_org_id &&
                    lcdoPosNeg.coverage_code == lcdoCompare.coverage_code)
                {
                    if (IsCompareAbsolute == true &&
                        Math.Abs(lcdoPosNeg.member_premium_amount) == Math.Abs(lcdoCompare.member_premium_amount) &&
                        Math.Abs(lcdoPosNeg.total_premium_amount) == Math.Abs(lcdoCompare.total_premium_amount))
                        return true;
                    else if (IsCompareAbsolute == false &&
                    lcdoPosNeg.member_premium_amount == lcdoCompare.member_premium_amount &&
                    lcdoPosNeg.total_premium_amount == lcdoCompare.total_premium_amount)
                        return true;
                }
            }
            else if (lcdoPosNeg.plan_id == busConstant.PlanIdMedicarePartD)
            {
                if (lcdoPosNeg.mode_of_payment_value == lcdoCompare.mode_of_payment_value &&
                    lcdoPosNeg.person_account_id == lcdoCompare.person_account_id &&
                    lcdoPosNeg.provider_org_id == lcdoCompare.provider_org_id)
                {
                    if (IsCompareAbsolute == true &&
                        Math.Abs(lcdoPosNeg.member_premium_amount) == Math.Abs(lcdoCompare.member_premium_amount) &&
                        Math.Abs(lcdoPosNeg.total_premium_amount) == Math.Abs(lcdoCompare.total_premium_amount) &&
                        Math.Abs(lcdoPosNeg.lis_amount) == Math.Abs(lcdoCompare.lis_amount) &&
                        Math.Abs(lcdoPosNeg.lep_amount) == Math.Abs(lcdoCompare.lep_amount))
                        return true;
                    else if (IsCompareAbsolute == false &&
                    lcdoPosNeg.member_premium_amount == lcdoCompare.member_premium_amount &&
                    lcdoPosNeg.total_premium_amount == lcdoCompare.total_premium_amount &&
                    lcdoPosNeg.lis_amount == lcdoCompare.lis_amount &&
                    lcdoPosNeg.lep_amount == lcdoCompare.lep_amount)
                        return true;
                }
            }
            else if (lcdoPosNeg.plan_id == busConstant.PlanIdGroupLife)
            {
                if (lcdoPosNeg.mode_of_payment_value == lcdoCompare.mode_of_payment_value &&
                    lcdoPosNeg.person_account_id == lcdoCompare.person_account_id &&
                    lcdoPosNeg.group_number == lcdoCompare.group_number &&
                    lcdoPosNeg.provider_org_id == lcdoCompare.provider_org_id)
                {
                    if (IsCompareAbsolute == true &&
                       Math.Abs(lcdoPosNeg.member_premium_amount) == Math.Abs(lcdoCompare.member_premium_amount) &&
                       Math.Abs(lcdoPosNeg.total_premium_amount) == Math.Abs(lcdoCompare.total_premium_amount) &&
                       Math.Abs(lcdoPosNeg.life_basic_premium_amount) == Math.Abs(lcdoCompare.life_basic_premium_amount) &&
                       Math.Abs(lcdoPosNeg.life_supp_premium_amount) == Math.Abs(lcdoCompare.life_supp_premium_amount) &&
                       Math.Abs(lcdoPosNeg.life_spouse_supp_premium_amount) == Math.Abs(lcdoCompare.life_spouse_supp_premium_amount) &&
                       Math.Abs(lcdoPosNeg.life_dep_supp_premium_amount) == Math.Abs(lcdoCompare.life_dep_supp_premium_amount) &&
                       Math.Abs(lcdoPosNeg.life_basic_coverage_amount) == Math.Abs(lcdoCompare.life_basic_coverage_amount) &&
                       Math.Abs(lcdoPosNeg.life_supp_coverage_amount) == Math.Abs(lcdoCompare.life_supp_coverage_amount) &&
                       Math.Abs(lcdoPosNeg.life_spouse_supp_coverage_amount) == Math.Abs(lcdoCompare.life_spouse_supp_coverage_amount) &&
                       Math.Abs(lcdoPosNeg.life_dep_supp_coverage_amount) == Math.Abs(lcdoCompare.life_dep_supp_coverage_amount) &&
                       Math.Abs(lcdoPosNeg.ad_and_d_basic_premium_rate) == Math.Abs(lcdoCompare.ad_and_d_basic_premium_rate) &&
                       Math.Abs(lcdoPosNeg.ad_and_d_supplemental_premium_rate) == Math.Abs(lcdoCompare.ad_and_d_supplemental_premium_rate))
                        return true;
                    else if (IsCompareAbsolute == false &&
                       lcdoPosNeg.member_premium_amount == lcdoCompare.member_premium_amount &&
                       lcdoPosNeg.total_premium_amount == lcdoCompare.total_premium_amount &&
                       lcdoPosNeg.life_basic_premium_amount == lcdoCompare.life_basic_premium_amount &&
                       lcdoPosNeg.life_supp_premium_amount == lcdoCompare.life_supp_premium_amount &&
                       lcdoPosNeg.life_spouse_supp_premium_amount == lcdoCompare.life_spouse_supp_premium_amount &&
                       lcdoPosNeg.life_dep_supp_premium_amount == lcdoCompare.life_dep_supp_premium_amount &&
                       lcdoPosNeg.life_basic_coverage_amount == lcdoCompare.life_basic_coverage_amount &&
                       lcdoPosNeg.life_supp_coverage_amount == lcdoCompare.life_supp_coverage_amount &&
                       lcdoPosNeg.life_spouse_supp_coverage_amount == lcdoCompare.life_spouse_supp_coverage_amount &&
                       lcdoPosNeg.life_dep_supp_coverage_amount == lcdoCompare.life_dep_supp_coverage_amount &&
                       lcdoPosNeg.ad_and_d_basic_premium_rate == lcdoCompare.ad_and_d_basic_premium_rate &&
                       lcdoPosNeg.ad_and_d_supplemental_premium_rate == lcdoCompare.ad_and_d_supplemental_premium_rate)
                        return true;
                }
            }
            return false;
        }

        //IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
        public Collection<busIbsDetail> GetIbsDetailsOfPerson(DateTime adtBillingMonth, int aintPersonAccountID)
        {
            Collection<busIbsDetail> lColIbsDtl = new Collection<busIbsDetail>();
            DataTable ldtbList = Select("cdoIbsHeader.LoadIBSDetailsForAdjustment", new object[2] { aintPersonAccountID, adtBillingMonth });

            lColIbsDtl = GetCollection<busIbsDetail>(ldtbList, "icdoIbsDetail");

            return lColIbsDtl.OrderBy(i => i.icdoIbsDetail.ibs_detail_id).ToList().ToCollection();
        }
        public Collection<busEmployerPayrollDetail> GetEmployerPayrollDetailsOfPerson(DateTime adtBillingMonth, int aintPersonID, int aintPlanID)
        {
            Collection<busEmployerPayrollDetail> lColEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();
            DataTable ldtbList = Select("entEmployerPayrollHeader.LoadReviewEmployerPayrollDtlForAdjustment", new object[3] { adtBillingMonth, aintPersonID, aintPlanID });

            lColEmployerPayrollDtl = GetCollection<busEmployerPayrollDetail>(ldtbList, "icdoEmployerPayrollDetail");

            return lColEmployerPayrollDtl;
        }
        //IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111

        //IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
        public Collection<busIbsDetail> ComparePositiveNegativeColIbsDetail()
        {
            Collection<busIbsDetail> lcolIbsDetailWithoutDuplicate = new Collection<busIbsDetail>();
            Collection<busIbsDetail> lcolFinal = new Collection<busIbsDetail>();

            if (icolPosNegIbsDetail.IsNotNull())
            {
                // icolPosNegIbsDetail contains positive as well as negative records, so if positive and negative records are same with absolute values then don't take those records in new collection 'lcolIbsDetailWithoutDuplicate'
                foreach (busIbsDetail lbusIbsDtlPosNeg in icolPosNegIbsDetail)
                {
                    busIbsDetail lbusIbsDetailCompare = icolPosNegIbsDetail.Where(i => i.SrNo != lbusIbsDtlPosNeg.SrNo
                                                        && i.icdoIbsDetail.billing_month_and_year == lbusIbsDtlPosNeg.icdoIbsDetail.billing_month_and_year
                                                        && i.icdoIbsDetail.person_account_id == lbusIbsDtlPosNeg.icdoIbsDetail.person_account_id
                                                        && i.icdoIbsDetail.person_id == lbusIbsDtlPosNeg.icdoIbsDetail.person_id
                                                        && i.icdoIbsDetail.plan_id == lbusIbsDtlPosNeg.icdoIbsDetail.plan_id
                                                        ).FirstOrDefault();

                    if (lbusIbsDetailCompare.IsNotNull())
                    {
                        LoadIBSCoverageCode(lbusIbsDetailCompare);
                        //if records are different then take that record in new collection
                        if (!(IsIbsCollectionSame(lbusIbsDtlPosNeg.icdoIbsDetail, lbusIbsDetailCompare.icdoIbsDetail, true)))
                        {
                            lcolIbsDetailWithoutDuplicate.Add(lbusIbsDtlPosNeg);
                        }
                    }
                    else
                        lcolIbsDetailWithoutDuplicate.Add(lbusIbsDtlPosNeg);
                }


                //let us check it in db Negative and Positive are the same - Should either update to Ignore or not insert at all.
                if (lcolIbsDetailWithoutDuplicate.Count > 0)
                {
                    DateTime adtBillingMonth = lcolIbsDetailWithoutDuplicate.Min(i => i.icdoIbsDetail.billing_month_and_year);
                    cdoIbsDetail lcdoIbsDetail = lcolIbsDetailWithoutDuplicate[0].icdoIbsDetail;
                    Collection<busIbsDetail> lcolIbsDtl = GetIbsDetailsOfPerson(adtBillingMonth, lcdoIbsDetail.person_account_id);

                    foreach (busIbsDetail lbusIbsDtl in lcolIbsDetailWithoutDuplicate)
                    {
                        Collection<busIbsDetail> lColbusIbsDetailCompare = lcolIbsDtl.Where(i => i.icdoIbsDetail.billing_month_and_year == lbusIbsDtl.icdoIbsDetail.billing_month_and_year
                                                            && i.icdoIbsDetail.person_account_id == lbusIbsDtl.icdoIbsDetail.person_account_id
                                                            && i.icdoIbsDetail.person_id == lbusIbsDtl.icdoIbsDetail.person_id
                                                            && i.icdoIbsDetail.plan_id == lbusIbsDtl.icdoIbsDetail.plan_id
                                                            ).ToList().ToCollection();

                        if (lColbusIbsDetailCompare.Count > 0)
                        {
                            bool isRecordFound = false;
                            foreach (busIbsDetail lbusIbsDetailCompare in lColbusIbsDetailCompare)
                            {
                                LoadIBSCoverageCode(lbusIbsDetailCompare);
                                if (IsIbsCollectionSame(lbusIbsDtl.icdoIbsDetail, lbusIbsDetailCompare.icdoIbsDetail, lbusIbsDetailCompare.icdoIbsDetail.detail_status_value == busConstant.IBSHeaderStatusPosted ? false : true))
                                {
                                    if (lbusIbsDetailCompare.icdoIbsDetail.detail_status_value == busConstant.IBSHeaderStatusPosted && lColbusIbsDetailCompare.Where(i => i.icdoIbsDetail.ibs_detail_id > lbusIbsDetailCompare.icdoIbsDetail.ibs_detail_id).Count() > 0)
                                        isRecordFound = true;
                                    else
                                    {
                                        if (lbusIbsDetailCompare.icdoIbsDetail.detail_status_value != busConstant.IBSHeaderStatusPosted)
                                        {
                                            DBFunction.DBNonQuery("cdoIbsDetail.UpdateIBSDtlStatusToIgnoreByDtlID", new object[1] { lbusIbsDetailCompare.icdoIbsDetail.ibs_detail_id },
                                               iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                        }
                                        isRecordFound = true;
                                    }
                                }
                            }
                            if (!isRecordFound)
                                lcolFinal.Add(lbusIbsDtl);
                        }
                        else
                            lcolFinal.Add(lbusIbsDtl);
                    }
                }
            }

            return lcolFinal;
        }

        private void LoadIBSCoverageCode(busIbsDetail lbusIbsDetailCompare)
        {
            if (lbusIbsDetailCompare.icdoIbsDetail.coverage_code.IsNullOrEmpty() && (icdoPersonAccount.plan_id == busConstant.PlanIdDental || icdoPersonAccount.plan_id == busConstant.PlanIdVision
                                                || icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth))
            {
                busPersonAccountGhdv lbusPersonAccountGhdv = new busPersonAccountGhdv() { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                lbusPersonAccountGhdv.FindGHDVByPersonAccountID(icdoPersonAccount.person_account_id);
                if (lbusPersonAccountGhdv.IsNotNull())
                {
                    busPersonAccountGhdvHistory lbusEndedHistory = lbusPersonAccountGhdv.LoadHistoryByDate(lbusIbsDetailCompare.icdoIbsDetail.billing_month_and_year);
                    if (lbusEndedHistory.IsNotNull())
                    {
                        if (icdoPersonAccount.plan_id == busConstant.PlanIdDental || icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                            lbusIbsDetailCompare.icdoIbsDetail.coverage_code = lbusEndedHistory.icdoPersonAccountGhdvHistory.level_of_coverage_description;
                        else if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                        {
                            DataTable ldtbCoverageCode = busNeoSpinBase.Select("entIbsHeader.GetCoverageCodeDescByCoverageCodeNRateStructureCode",
                                                                        new object[2] { lbusEndedHistory.icdoPersonAccountGhdvHistory.coverage_code, lbusEndedHistory.icdoPersonAccountGhdvHistory.rate_structure_code });
                            if (ldtbCoverageCode.Rows.Count > 0)
                            {
                                lbusIbsDetailCompare.icdoIbsDetail.coverage_code = ldtbCoverageCode.Rows[0]["CLIENT_DESCRIPTION"].ToString();
                            }
                        }
                    }
                }
            }
        }

        // PIR 15544

        public void UpdateIBSFlagForActive()
        {
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0)
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag = busConstant.Flag_No;
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id = 0;
                ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value = string.Empty;
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date = DateTime.MinValue;
                ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id = 0;
            }
        }
        //PIR 16282 - Update dependent flag so that Cobra letters are not Duplicated by Drop Dependent COBRA Notice Letter Batch.
        public void UpdatePersonAccountDependentList()
        {

            DBFunction.DBNonQuery("entPersonAccountDependent.UpdatePersonAccountDependentList", new object[1] { icdoPersonAccount.person_account_id }, 
                                                                                                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

            //DataTable ldtbDependentlist = busBase.Select<cdoPersonAccountDependent>(
            //      new string[1] { "person_account_id" },
            //      new object[1] { icdoPersonAccount.person_account_id }, null, null);
            //if (ldtbDependentlist.Rows.Count > 0)
            //{
            //    Collection<busPersonAccountDependent> lclbPersonAccountDependent = GetCollection<busPersonAccountDependent>(ldtbDependentlist, "icdoPersonAccountDependent");
            //    foreach (busPersonAccountDependent lbusDependent in lclbPersonAccountDependent)
            //    {
            //        lbusDependent.icdoPersonAccountDependent.is_drop_dependent_letter_sent_flag = busConstant.Flag_Yes;
            //        lbusDependent.icdoPersonAccountDependent.Update();
            //    }
            //}
        }
        //PIR 16439
        public bool IsCanceled()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(337, icdoPersonAccount.plan_participation_status_value, iobjPassInfo);
            return (lstrData2 == busConstant.PlanParticipationStatusData2Cancelled) ? true : false;
        }
        public bool IsTransferredTIAACREF()
        {
            return icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementTransferredTIAACREF ? true : false;
        }
        public bool IsTransferredToTFFR()
        {
            return icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusTransferToTFFR ? true : false;
        }
        //PIR 16647

        public bool IsEnrolledOrSuspended()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(337, icdoPersonAccount.plan_participation_status_value, iobjPassInfo);
            return (lstrData2 == busConstant.PlanParticipationStatusData2Enrolled || lstrData2 == busConstant.PlanParticipationStatusData2Suspended) ? true : false;
        }

        public bool IsRetiredOrWithdrwan()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(337, icdoPersonAccount.plan_participation_status_value, iobjPassInfo);
            return (lstrData2 == busConstant.PlanParticipationStatusRetirementRetired || lstrData2 == busConstant.PlanParticipationStatusData2WithDrawn) ? true : false;
        }
        public bool IsSuspended()
        {
            string lstrData2 = busGlobalFunctions.GetData2ByCodeValue(337, icdoPersonAccount.plan_participation_status_value, iobjPassInfo);
            return (lstrData2 == busConstant.PlanParticipationStatusData2Suspended);
        }

        //PIR-16812
        #region PIR-16812
        public Collection<busPersonAccountDeferredCompProvider> iclcDefCompActiveProviders { get; set; }
        public void LoadActiveDefCompProviders(DateTime adtEffectiveDate)
        {
            DataTable ldtbActiveProviders = Select("cdoPersonAccountDeferredCompProvider.LoadActiveProvidersByPayPeriodStartDate", new object[2] { icdoPersonAccount.person_account_id, adtEffectiveDate });
            iclcDefCompActiveProviders = GetCollection<busPersonAccountDeferredCompProvider>(ldtbActiveProviders, "icdoPersonAccountDeferredCompProvider");
        }
        #endregion PIR-16812
        public bool iblnLoadOnlyRequiredFieldsForVSCOrPSC { get; set; } //PIR 18567

        public bool iblnIsFromMSSForEnrollmentData { get; set; }//PIR 20017

        public bool iblnPlaneffectiveDateLoaded { get; set; }

        public ArrayList CreateOrUpdateAchDetail(int aintWssBenAppId)
        {
            ArrayList larrList = new ArrayList();
            DataTable ldtbWssWithdrawlAccount = Select<cdoWssBenAppAchDetail>(new string[2] { enmWssBenAppAchDetail.wss_ben_app_id.ToString(),
                                                                                                            enmWssBenAppAchDetail.ach_type.ToString() },
                                                                                            new object[2] { aintWssBenAppId,
                                                                                                             busConstant.WithdrawlAchType }, null
                                                                                                             , "wss_ben_app_ach_detail_id desc");
            if (icdoPersonAccount.person_account_id > 0 && icdoPersonAccount.history_change_date != DateTime.MinValue && ldtbWssWithdrawlAccount.Rows.Count > 0)
            {
                DataTable ldtbPersonAccountAchDetail = Select<cdoPersonAccountAchDetail>(new string[1] { "PERSON_ACCOUNT_ID" },
                                                                                            new object[1] { icdoPersonAccount.person_account_id }, null, null);
                if (ldtbPersonAccountAchDetail.Rows.Count > 0)
                {
                    Collection<busPersonAccountAchDetail> lclbPAAchDetail = GetCollection<busPersonAccountAchDetail>(ldtbPersonAccountAchDetail, "icdoPersonAccountAchDetail");
                    if (lclbPAAchDetail.Any(detail => busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                                                     detail.icdoPersonAccountAchDetail.ach_start_date, detail.icdoPersonAccountAchDetail.ach_end_date)))
                    {
                        busPersonAccountAchDetail busPersonAccountAchDetailUpdate = lclbPAAchDetail.FirstOrDefault(detail =>
                                                                              busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                                                                              detail.icdoPersonAccountAchDetail.ach_start_date, detail.icdoPersonAccountAchDetail.ach_end_date));
                        if (busPersonAccountAchDetailUpdate.IsNotNull())
                        {
                            busPersonAccountAchDetailUpdate.icdoPersonAccountAchDetail.ach_end_date =
                                (busPersonAccountAchDetailUpdate.icdoPersonAccountAchDetail.ach_start_date.Date == icdoPersonAccount.history_change_date.Date) ?
                                icdoPersonAccount.history_change_date.Date : icdoPersonAccount.history_change_date.Date.AddDays(-1);

                            busPersonAccountAchDetailUpdate.icdoPersonAccountAchDetail.Update();
                        }
                    }
                }
                cdoWssBenAppAchDetail lcdoWssBenAppAchDetail = new cdoWssBenAppAchDetail();
                lcdoWssBenAppAchDetail.LoadData(ldtbWssWithdrawlAccount.Rows[0]);
                busPersonAccountAchDetail lbusPersonAccountAchDetail = new busPersonAccountAchDetail { icdoPersonAccountAchDetail = new cdoPersonAccountAchDetail() };
                lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.pre_note_flag = busConstant.Flag_Yes;
                lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.ach_start_date = icdoPersonAccount.history_change_date;
                lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.person_account_id = icdoPersonAccount.person_account_id;
                lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.bank_account_number = lcdoWssBenAppAchDetail.account_number;
                lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.bank_account_type_value = (lcdoWssBenAppAchDetail.bank_account_type_value ==
                                                                                                busConstant.BankAccountSavings) ? busConstant.PersonAccountBankAccountSavings :
                                                                                                busConstant.PersonAccountBankAccountChecking;
                busOrganization lbusOrganization = new busOrganization();
                if (lbusOrganization.FindOrganizationByRoutingNumber(lcdoWssBenAppAchDetail.routing_no))
                {
                    lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.org_code = lbusOrganization.icdoOrganization.org_code;
                    lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.bank_org_id = lbusOrganization.icdoOrganization.org_id;
                }
                else
                {
                    busPayeeAccountAchDetail lbusPayeeAccountAchDetail = new busPayeeAccountAchDetail();
                    lbusPayeeAccountAchDetail.InsertOrgBankForRoutingNumber(lcdoWssBenAppAchDetail.bank_name, lcdoWssBenAppAchDetail.routing_no);
                    lbusPayeeAccountAchDetail.LoadBankOrgByRoutingNumber(lcdoWssBenAppAchDetail.routing_no);
                    if (lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_id > 0)
                    {
                        lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.org_code = lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_code;
                        lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.bank_org_id = lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_id;
                        lbusPayeeAccountAchDetail.InitializeWorkflow(lbusPayeeAccountAchDetail.ibusBankOrg.icdoOrganization.org_id, busConstant.Map_Process_Create_And_Maintain_Organization_Information);
                    }
                    else
                    {
                        utlError lerror = AddError(0, "Bank Org With the entered routing number cannot be created.");
                        larrList.Add(lerror);
                        return larrList;
                    }
                }
                lbusPersonAccountAchDetail.icdoPersonAccountAchDetail.ienuObjectState = ObjectState.Insert;
                lbusPersonAccountAchDetail.iarrChangeLog.Add(lbusPersonAccountAchDetail.icdoPersonAccountAchDetail);
                lbusPersonAccountAchDetail.BeforeValidate(utlPageMode.New);
                lbusPersonAccountAchDetail.ValidateHardErrors(utlPageMode.New);
                if (lbusPersonAccountAchDetail.iarrErrors.Count > 0)
                {
                    foreach (utlError lobjErr in lbusPersonAccountAchDetail.iarrErrors)
                    {
                        utlError lerror = new utlError
                        {
                            istrErrorID = lobjErr.istrErrorID,
                            istrErrorMessage = lobjErr.istrDisplayMessage
                        };
                        larrList.Add(lerror);
                        return larrList;
                    }
                }
                else
                {
                    lbusPersonAccountAchDetail.BeforePersistChanges();
                    lbusPersonAccountAchDetail.PersistChanges();
                    lbusPersonAccountAchDetail.ValidateSoftErrors();
                    lbusPersonAccountAchDetail.UpdateValidateStatus();
                    lbusPersonAccountAchDetail.AfterPersistChanges();
                }
            }
            return larrList;
        }

        public bool IsRetireePlanEnrolledOrIsCobra()
        {
            bool lblnResult = false;
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth || icdoPersonAccount.plan_id == busConstant.PlanIdDental 
                || icdoPersonAccount.plan_id == busConstant.PlanIdVision)
            {
                if (ibusPersonAccountGHDV.IsNull())
                    LoadPersonAccountGHDV();
                // Check for GHDV Plans
                //PIR 24439 - Cobra expiration date condition removed
                DateTime ldtSysDate = busGlobalFunctions.GetSysManagementBatchDate();

                if ((icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth && 
                   ibusPersonAccountGHDV?.icdoPersonAccountGhdv?.health_insurance_type_value == busConstant.HealthInsuranceTypeRetiree) 
                   //|| (!(string.IsNullOrEmpty(ibusPersonAccountGHDV?.icdoPersonAccountGhdv?.cobra_type_value)) && icdoPersonAccount.cobra_expiration_date > ldtSysDate
                   //     && icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceSuspended)))

                   || (icdoPersonAccount.plan_id == busConstant.PlanIdDental && 
                   ibusPersonAccountGHDV?.icdoPersonAccountGhdv?.dental_insurance_type_value == busConstant.DentalInsuranceTypeRetiree) 
                   //|| (!(string.IsNullOrEmpty(ibusPersonAccountGHDV?.icdoPersonAccountGhdv?.cobra_type_value)) && icdoPersonAccount.cobra_expiration_date > ldtSysDate
                   //     && icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceSuspended)))

                   || (icdoPersonAccount.plan_id == busConstant.PlanIdVision && 
                   ibusPersonAccountGHDV?.icdoPersonAccountGhdv?.vision_insurance_type_value == busConstant.VisionInsuranceTypeRetiree) )
                   //|| (!(string.IsNullOrEmpty(ibusPersonAccountGHDV?.icdoPersonAccountGhdv?.cobra_type_value)) && icdoPersonAccount.cobra_expiration_date > ldtSysDate
                   //    && icdoPersonAccount.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceSuspended)))
                {
                    lblnResult = true;
                }
            }
            // Check for Life Insurance Plan
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
            {
                if (ibusPersonAccountLife.IsNull())
                    LoadPersonAccountLife();
                if (ibusPersonAccountLife.icdoPersonAccountLife.life_insurance_type_value == busConstant.LifeInsuranceTypeRetireeMember)
                {
                    lblnResult = true;
                }
            }
            return lblnResult;

        }

        //PIR 23359
        public DateTime GetEarlyPlanParticiaptionStartDate()
        {
            Collection<busPersonAccount> lclbPersonAccounts = new Collection<busPersonAccount>();
            DataTable ldtbList = Select<cdoPersonAccount>(
                new string[1] { "person_id" },
                new object[1] { icdoPersonAccount.person_id }, null, null);
            if (ldtbList.Rows.Count > 0)
            {
                lclbPersonAccounts = GetCollection<busPersonAccount>(ldtbList, "icdoPersonAccount");

                var lclbRetirementPersonAccount = lclbPersonAccounts.Where(i => ((i.icdoPersonAccount.plan_id == busConstant.PlanIdMain ||
               i.icdoPersonAccount.plan_id == busConstant.PlanIdLE ||
               i.icdoPersonAccount.plan_id == busConstant.PlanIdNG ||
               i.icdoPersonAccount.plan_id == busConstant.PlanIdLEWithoutPS ||
               i.icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf ||
               i.icdoPersonAccount.plan_id == busConstant.PlanIdStatePublicSafety || //PIR 25729
               i.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020) &&
               (i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled ||
               i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetimentSuspended ||
               i.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementRetired)));

                return lclbRetirementPersonAccount.IsNotNull() && lclbRetirementPersonAccount.Count() > 0 ? lclbRetirementPersonAccount.Min(i => i.icdoPersonAccount.start_date) : DateTime.MinValue;
            }
            return DateTime.MinValue;
        }
		
		//PIR 23167, 23340, 23408
        public bool IsResourceForEnhancedOverlap(int aintEnhancedResourceID)
        {
            int lintCount = 0;
            lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckResourceForLoggedInUser", new object[2] { iobjPassInfo.iintUserSerialID, aintEnhancedResourceID },
                iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
            if (lintCount > 0)
                return true;
            else
                return false;
  
        }

        public bool IsResourceForOverlap
        {
            get
            {
                int lintCount = 0;
                if (ibusPlan.IsNull()) LoadPlan();
                if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth || icdoPersonAccount.plan_id == busConstant.PlanIdDental ||icdoPersonAccount.plan_id == busConstant.PlanIdVision
                    || icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfLoggedInUserHasOverlapResource", new object[3] { iobjPassInfo.iintUserSerialID, busConstant.OverlapResourceGHDV,
                                                                                                                                                    busConstant.OverlapResourceEnhancedGHDV},
                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                }
                else if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                {
                    lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfLoggedInUserHasOverlapResource", new object[3] { iobjPassInfo.iintUserSerialID, busConstant.OverlapResourceLife,
                                                                                                                                                       busConstant.OverlapResourceEnhancedLife},
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                }
                else if (icdoPersonAccount.plan_id == busConstant.PlanIdEAP)
                {
                    lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfLoggedInUserHasOverlapResource", new object[3] { iobjPassInfo.iintUserSerialID, busConstant.OverlapResourceEAP,
                                                                                                                                                        busConstant.OverlapResourceEnhancedEAP},
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                }
                else if (icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
                {
                    lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfLoggedInUserHasOverlapResource", new object[3] { iobjPassInfo.iintUserSerialID, busConstant.OverlapResourceFlex,
                                                                                                                                                      busConstant.OverlapResourceEnhancedFlex},
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                }
                else if (ibusPlan.IsRetirementPlan())
                {
                    lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfLoggedInUserHasOverlapResource", new object[3] { iobjPassInfo.iintUserSerialID, busConstant.OverlapResourceGHDV,
                                                                                                                                                    busConstant.OverlapResourceEnhancedRetirement},
                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                }
                else if (icdoPersonAccount.plan_id == busConstant.PlanIdDeferredCompensation || icdoPersonAccount.plan_id == busConstant.PlanIdOther457)
                {
                    lintCount = Convert.ToInt32(DBFunction.DBExecuteScalar("cdoPersonAccount.CheckIfLoggedInUserHasOverlapResource", new object[3] { iobjPassInfo.iintUserSerialID, busConstant.OverlapResourceGHDV,
                                                                                                                                                    busConstant.OverlapResourceEnhancedDepCompOtherPlan},
                     iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework));
                }
                if (lintCount > 0)
                    return true;
                else
                    return false;
            }
        }

        public bool IsResourceForEnhancedOverlapIBS()
        {
            if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth || icdoPersonAccount.plan_id == busConstant.PlanIdDental || icdoPersonAccount.plan_id == busConstant.PlanIdVision)
            {
                if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedGHDV))
                    return true;
            }
            else if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
            {
                if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedLife))
                    return true;
            }

            return false;
        }
        // PIR 23734 
        public Collection<cdoCodeValue> LoadChangeReason() => busGlobalFunctions.LoadCodeValuesDataByCodeId(332);

        /// <summary>
        /// aintPlanType = 1 - Retirement
        /// aintPlanType = 2 - EAP
        /// aintPlanType = 3 - Life
        /// </summary>
        public void UpdateNewHireMemberRecordReQuestDetailsManOrAutoPlanEnrollment(int aintPlanType)
        {
            string lstrPlanParticipationStatusValue = busGlobalFunctions.GetData2ByCodeValue(337, icdoPersonAccount.plan_participation_status_value, iobjPassInfo);
            if (icdoPersonAccount.person_employment_dtl_id > 0 && lstrPlanParticipationStatusValue == busConstant.PlanParticipationStatusData2Enrolled)
            {
                if (ibusMemberRecordRequest.IsNotNull() && ibusMemberRecordRequest?.icdoWssMemberRecordRequest?.status_value != busConstant.EmploymentChangeRequestStatusProcessed) //Enrollment Is From NewHireBatch or Post_Click from member Record Request
                {
                    UpdateMemberRecordRequestStatus(aintPlanType, ibusMemberRecordRequest);
                }
                else //Enrollment Is From screen
                {
                    DataTable ldtbWssPersonEmploymentDetails = busBase.Select("entPersonAccount.GetEmpDetailForMemberRecordRequestUpdate", new object[2] { icdoPersonAccount.person_account_id, icdoPersonAccount.history_change_date });
                    if (ldtbWssPersonEmploymentDetails.Rows.Count > 0)
                    {
                        Collection<busWssPersonEmploymentDetail> lcblbusWssPersonEmploymentDetail = GetCollection<busWssPersonEmploymentDetail>(ldtbWssPersonEmploymentDetails, "icdoWssPersonEmploymentDetail");
                        foreach (busWssPersonEmploymentDetail lbusWssPersonEmploymentDetail in lcblbusWssPersonEmploymentDetail)
                        {
                            busWssMemberRecordRequest lbusWssMemberRecordRequest = new busWssMemberRecordRequest() { icdoWssMemberRecordRequest = new cdoWssMemberRecordRequest() };
                            if (lbusWssMemberRecordRequest.FindWssMemberRecordRequest(lbusWssPersonEmploymentDetail.icdoWssPersonEmploymentDetail.member_record_request_id))
                            {
                                if (lbusWssMemberRecordRequest.icdoWssMemberRecordRequest.status_value != busConstant.EmploymentChangeRequestStatusProcessed)
                                {
                                    UpdateMemberRecordRequestStatus(aintPlanType, lbusWssMemberRecordRequest);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateMemberRecordRequestStatus(int aintPlanType, busWssMemberRecordRequest abusWssMemberRecordRequest)
        {
            switch (aintPlanType)
            {
                case busConstant.RetirementCategory:
                    abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.retr_status = busConstant.Flag_Yes;
                    break;
                case busConstant.EAPCategory:
                    abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.eap_status = busConstant.Flag_Yes;
                    break;
                case busConstant.LifeCategory:
                    abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.life_status = busConstant.Flag_Yes;
                    break;
                default:
                    break;
            }
            abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.Update();
            if (abusWssMemberRecordRequest?.icdoWssPersonEmploymentDetail?.person_employment_dtl_id > 0)
            {
                DataTable ldtbPersonActEmploymentDetails = busBase.Select<cdoPersonAccountEmploymentDetail>(new string[1] { enmPersonAccountEmploymentDetail.person_employment_dtl_id.ToString() }, new object[1] { abusWssMemberRecordRequest?.icdoWssPersonEmploymentDetail?.person_employment_dtl_id }, null, null);
                Collection<busPersonAccountEmploymentDetail> lclbbusPersonActEmploymentDetails = GetCollection<busPersonAccountEmploymentDetail>(ldtbPersonActEmploymentDetails, "icdoPersonAccountEmploymentDetail");
                lclbbusPersonActEmploymentDetails.ForEach(detail => detail.LoadPlan());
                bool lblnIsMemberEligibleForRetirementPlan = false;
                bool lblnIsMemberEligibleForEAPPlan = false;
                bool lblnIsMemberEligibleForLifePlan = false;
                lblnIsMemberEligibleForRetirementPlan = lclbbusPersonActEmploymentDetails.Any(detail => (detail.ibusPlan.IsDBRetirementPlan() || detail.ibusPlan.IsHBRetirementPlan()));
                lblnIsMemberEligibleForEAPPlan = lclbbusPersonActEmploymentDetails.Any(detail => detail.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdEAP);
                lblnIsMemberEligibleForLifePlan = lclbbusPersonActEmploymentDetails.Any(detail => detail.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdGroupLife);
                if (((lblnIsMemberEligibleForRetirementPlan && abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.retr_status == busConstant.Flag_Yes) || (!lblnIsMemberEligibleForRetirementPlan)) &&
                    ((lblnIsMemberEligibleForEAPPlan && abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.eap_status == busConstant.Flag_Yes) || (!lblnIsMemberEligibleForEAPPlan)) &&
                    ((lblnIsMemberEligibleForLifePlan && abusWssMemberRecordRequest.icdoWssPersonEmploymentDetail.life_status == busConstant.Flag_Yes) || (!lblnIsMemberEligibleForLifePlan)))
                {
                    abusWssMemberRecordRequest.icdoWssMemberRecordRequest.status_value = busConstant.EmploymentChangeRequestStatusProcessed;
                    abusWssMemberRecordRequest.icdoWssMemberRecordRequest.Update();
                }
            }
        }
        //PIR 25049
        public bool IsStartDatePriortoEnrollmentEffeDate()
        {
            if (iblnIsEnrollmentValidationApplicable)
            {
                if (icdoPersonAccount.person_id > 0 && icdoPersonAccount.history_change_date > DateTime.MinValue && icdoPersonAccount.person_account_id > 0)
                {
                    int lintLinkedDetailCount = Convert.ToInt32(DBFunction.DBExecuteScalar("entPersonAccount.GetEmpDetIdLinkedToPersonAccountId", new object[3] {
                                                                                                    icdoPersonAccount.person_id,
                                                                                                    icdoPersonAccount.person_account_id,
                                                                                                    icdoPersonAccount.history_change_date.Date },
                                                                                                        iobjPassInfo.iconFramework,
                                                                                                        iobjPassInfo.itrnFramework));
                    return lintLinkedDetailCount == 0;
                }
                if (icdoPersonAccount.person_account_id == 0)
                {
                    int lintLinkedDetailCount = Convert.ToInt32(DBFunction.DBExecuteScalar("entPersonAccount.CheckChangeIsWithinEmploymentDate", new object[3] {
                                                                                                    icdoPersonAccount.person_id,icdoPersonAccount.plan_id,
                                                                                                    icdoPersonAccount.history_change_date.Date },
                                                                                                         iobjPassInfo.iconFramework,
                                                                                                         iobjPassInfo.itrnFramework));
                    return lintLinkedDetailCount == 0;
                }
            }
            return false;
        }
        //PIR 24933
        public void UpdateActionStatusifDefeApplicationExists()
        {
            DataTable ldtbList = Select<cdoBenefitApplication>(
                                      new string[3] { "member_person_id", "plan_id", "action_status_value" },
                                      new object[3] { icdoPersonAccount.person_id, icdoPersonAccount.plan_id, busConstant.ApplicationActionStatusDeferred }, null, null);
            Collection<busBenefitApplication> lclbBenefitApplication = GetCollection<busBenefitApplication>(ldtbList, "icdoBenefitApplication");

            foreach (busBenefitApplication lbusBenefitApplication in lclbBenefitApplication)
            {
                lbusBenefitApplication.icdoBenefitApplication.action_status_value = busConstant.ApplicationActionStatusCancelled;
                lbusBenefitApplication.icdoBenefitApplication.Update();
            }
        }
        //PIR 24933
        public bool IsMemberEnrolledinPlanAsofBatchDate(int aintPersonId, int aintPlanID ,DateTime adtBatchRunDate )
        {
            bool lblnResult = false;
            if (aintPlanID == busConstant.PlanIdMain || aintPlanID == busConstant.PlanIdMain2020 || aintPlanID == busConstant.PlanIdJudges || aintPlanID == busConstant.PlanIdLEWithoutPS ||
                aintPlanID == busConstant.PlanIdLE || aintPlanID == busConstant.PlanIdNG || aintPlanID == busConstant.PlanIdBCILawEnf || aintPlanID == busConstant.PlanIdStatePublicSafety) //PIR 25729
            {
                DataTable ldtbResult = Select("entPersonAccountRetirement.IsPersonEnrolledAsofBatchDate", new object[2] { aintPersonId, adtBatchRunDate });
                if (ldtbResult.Rows.Count > 0)
                {
                    lblnResult = true;
                }                
            }
            return lblnResult;
        }

        public int GetAssociatedEmploymentDetailID() => Convert.ToInt32(DBFunction.DBExecuteScalar("entPersonAccount.GetAssociatedEmpDetId", new object[2] {
                                                                                                    icdoPersonAccount.person_id,
                                                                                                    icdoPersonAccount.person_account_id},
                                                                                                 iobjPassInfo.iconFramework,
                                                                                                 iobjPassInfo.itrnFramework));

        // PIR 26717 Update all qu Bookmarks to fieldbookamars for PER-0155
        public DateTime ldtmPremiumDueDate { get; set; }
        public override void LoadBookmarkValues(utlCorresPondenceInfo aobjCorrespondenceInfo, Hashtable ahstQueryBookmarks)
        {
            if (aobjCorrespondenceInfo.istrTemplateName == "PER-0155")
            {
                foreach (var item in aobjCorrespondenceInfo.icolBookmarkFieldInfo)
                {
                    if (item.istrName == "PLANNAME")
                    {                       
                        item.istrValue = istrCapitalPlanName.ToString();
                    }
                    if (item.istrName == "PremiumDueDate")
                    {
                        ldtmPremiumDueDate = DateTime.Now.AddDays(44);
                        item.istrValue = ldtmPremiumDueDate.ToString("MM/DD/YYYY");
                    }
                }
            }
            base.LoadBookmarkValues(aobjCorrespondenceInfo, ahstQueryBookmarks);
        }
        public bool IsMemberEligibleToEnrollInCurrentYear(DateTime adtmDateOfChange, string astrReasonValue, bool ablnIsCurrentPreTax = false, decimal adecFlexMSRAAmount = 0, decimal adecFlexDCRAAmount = 0)
        {
            if (adtmDateOfChange != DateTime.MinValue)
            {
                if (icdoPersonAccount.plan_id == busConstant.PlanIdDental || icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                {
                    busPersonAccountGhdvHistory lbusPAGHDVHistory = new busPersonAccountGhdvHistory { icdoPersonAccountGhdvHistory = new cdoPersonAccountGhdvHistory() };
                    if (ibusPersonAccountGHDV.IsNull())
                        LoadPersonAccountGHDV();
                    if (ibusPersonAccountGHDV.IsNotNull() && ibusPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id > 0 && ibusPersonAccountGHDV.ibusHistory.IsNull())
                        ibusPersonAccountGHDV.LoadPreviousHistory();
                    if(ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.IsNotNull() && ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.Count() > 0)
                    {
                        lbusPAGHDVHistory = ibusPersonAccountGHDV.iclbPersonAccountGHDVHistory.Where(i => i.icdoPersonAccountGhdvHistory.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceCancelled).FirstOrDefault();
                    }
                    ///PIR 26356 
                    ///If member is switching to org which doesnt provide Flex during same calender year, cannot Pre tax
                    if (!IsCurrentEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange) && ablnIsCurrentPreTax)
                        return true;
                    else if (!IsCurrentEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange) && !ablnIsCurrentPreTax)
                        return false;
                    ///regardless of gap – if member switches from employer that doesnt allow Flex to one that allows;
                    ///given member should have had N before; new enrollment can be Y or N
                    if (!IsPreviousEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange, ablnIsCurrentPreTax) &&
                         lbusPAGHDVHistory.icdoPersonAccountGhdvHistory.premium_conversion_indicator_flag == "N" ? true : false)
                        return false;

                    //if enrollment is effective from next year onwards or is employment is in the next year, then we dont need to throw any validation
                    LoadEmploymentStartDateLatestOrLinked();
                    if ((idtmLatestEndDatedEmplEndDate != DateTime.MinValue && idtmLatestEndDatedEmplEndDate.Year != adtmDateOfChange.Year) 
                        || (idtmLinkedEmplEndDate != DateTime.MinValue && idtmLinkedEmplEndDate.Year != adtmDateOfChange.Year))
                        return false;

                    DataTable ldtMemberWasEnrolledAsPreTax = Select("entPersonAccount.IsMemberWasEnrolledAsPreTax",
                                                new object[2] { adtmDateOfChange.Year, icdoPersonAccount.person_id });

                    //PIR 26356 Person Account does not currently exist – Dental/Vision/Flex – can enroll in any Pre or Post tax during the first 31 days 
                    if (IsCurrentDateAfter31DaysOfEmployment() && icdoPersonAccount.person_account_id == 0)
                        return true;
                    //PIR 26356 Existing Person Account and last enroll > 31 days – Can enroll in Dental/Vision but only Post tax – Validation in MSS/posting logic (New Hire change reason) 
                    else if (IsPreviousEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange, ablnIsCurrentPreTax)
                        && ablnIsCurrentPreTax && astrReasonValue == busConstant.ChangeReasonNewHire &&
                        IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days() && icdoPersonAccount.person_account_id > 0)
                        return true;
                    //PIR 26356 Already enrolled (Not NH or ANNE) – Dental/Vision – Pre Tax – can not change to Post tax 
                    //and cannot reduce Level of Coverage (Level of Coverage is already good with changes from PIR 26370) – Validation in MSS/posting logic 
                    else if (!ablnIsCurrentPreTax && (astrReasonValue != busConstant.ChangeReasonNewHire && astrReasonValue != busConstant.ChangeReasonAnnualEnrollment) &&
                            IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days())
                        return true;
                    //PIR 26356 Existing Person Account and last enroll < 31 days – Dental/Vision – Has to keep same as they had before transfer – Validation in MSS/posting logic  (New Hire change reason)
                    else if (IsPreviousEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange, ablnIsCurrentPreTax)
                            && !IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days() &&
                            icdoPersonAccount.person_account_id > 0 && astrReasonValue == busConstant.ChangeReasonNewHire && lbusPAGHDVHistory.IsNotNull() &&
                            ablnIsCurrentPreTax != (lbusPAGHDVHistory.icdoPersonAccountGhdvHistory.premium_conversion_indicator_flag == "Y" ? true : false))
                        return true;
                    //PIR 26428 / 26356 - > 31 days – if person account doesn’t exists then plan should not allow pre tax.
                    else if (IsPreviousEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange, ablnIsCurrentPreTax)
                        && ablnIsCurrentPreTax && IsEmploymentGapGreaterThan31DaysForNewPersonAccounts() && icdoPersonAccount.person_account_id == 0)
                        return true;
                }
                else if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupLife)
                {
                    busPersonAccountLifeHistory lbusPALifeHistory = new busPersonAccountLifeHistory { icdoPersonAccountLifeHistory = new cdoPersonAccountLifeHistory() };
                    if (ibusPersonAccountLife.IsNull())
                        LoadPersonAccountLife();
                    if (ibusPersonAccountLife.IsNotNull() && ibusPersonAccountLife.iclbPersonAccountLifeHistory.IsNull())
                        ibusPersonAccountLife.LoadHistory();
                    if(ibusPersonAccountLife.IsNotNull() && 
                        ibusPersonAccountLife.iclbPersonAccountLifeHistory.IsNotNull() && ibusPersonAccountLife.iclbPersonAccountLifeHistory.Count() > 0)
                    {
                        lbusPALifeHistory = ibusPersonAccountLife.iclbPersonAccountLifeHistory.Where(i => i.icdoPersonAccountLifeHistory.plan_participation_status_value != busConstant.PlanParticipationStatusInsuranceCancelled).FirstOrDefault();
                    }
                    //PIR 26356 If member is switching to org which doesnt provide Flex during same calender year, cannot Pre tax
                    if (!IsCurrentEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange) && ablnIsCurrentPreTax)
                        return true;
                    else if (!IsCurrentEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange) && !ablnIsCurrentPreTax)
                        return false;

                    if (!IsPreviousEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange, ablnIsCurrentPreTax))
                        return false;

                    //PIR 26356 if enrollment is effective from next year onwards, then we dont need to throw any validation
                    LoadEmploymentStartDateLatestOrLinked();
                    if ((idtmLatestEndDatedEmplEndDate != DateTime.MinValue && idtmLatestEndDatedEmplEndDate.Year != adtmDateOfChange.Year)
                        || (idtmLinkedEmplEndDate != DateTime.MinValue && idtmLinkedEmplEndDate.Year != adtmDateOfChange.Year))
                        return false;

                    DataTable ldtMemberWasEnrolledAsPreTax = Select("entPersonAccount.IsMemberWasEnrolledAsPreTax",
                                                                    new object[2] { adtmDateOfChange.Year, icdoPersonAccount.person_id });

                    //PIR 26356 Existing Person Account and last enroll > 31 days – Can enroll in Life Supplemental but only Post tax; – Validation in MSS/posting logic (New Hire change reason) 
                    if (IsPreviousEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange, ablnIsCurrentPreTax) &&
                            ablnIsCurrentPreTax && astrReasonValue == busConstant.ChangeReasonNewHire &&
                            IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days() 
                            //&& ldtMemberWasEnrolledAsPreTax.IsNotNull() && ldtMemberWasEnrolledAsPreTax.Rows.Count > 0 
                            && icdoPersonAccount.person_account_id > 0)
                        return true;
                    //PIR 26356 Existing Person Account and last enroll < 31 days – Life – Has to keep same as they had before transfer – Validation in MSS/posting logic  (New Hire change reason)
                    else if (IsPreviousEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange, ablnIsCurrentPreTax) &&
                            !IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days() 
                            //&& ldtMemberWasEnrolledAsPreTax.IsNotNull() && ldtMemberWasEnrolledAsPreTax.Rows.Count > 0 
                            && icdoPersonAccount.person_account_id > 0 && astrReasonValue == busConstant.ChangeReasonNewHire && lbusPALifeHistory.IsNotNull() &&
                            ablnIsCurrentPreTax != (lbusPALifeHistory.icdoPersonAccountLifeHistory.premium_conversion_indicator_flag == "Y" ? true : false))
                        return true;
                    else if (ablnIsCurrentPreTax && lbusPALifeHistory.IsNotNull() && lbusPALifeHistory.icdoPersonAccountLifeHistory.person_account_life_history_id > 0
                        && lbusPALifeHistory.icdoPersonAccountLifeHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended
                        && lbusPALifeHistory.icdoPersonAccountLifeHistory.effective_start_date.Date != lbusPALifeHistory.icdoPersonAccountLifeHistory.effective_end_date.Date
                        && adtmDateOfChange.Date.Year == lbusPALifeHistory.icdoPersonAccountLifeHistory.effective_start_date.Date.Year
                        && adtmDateOfChange.Date > lbusPALifeHistory.icdoPersonAccountLifeHistory.effective_start_date.Date)
                        return true;
                    //PIR 26428 / 26356 - > 31 days – if person account doesn’t exists then plan should not allow pre tax.
                    else if (IsPreviousEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange, ablnIsCurrentPreTax)
                        && ablnIsCurrentPreTax && IsEmploymentGapGreaterThan31DaysForNewPersonAccounts() && icdoPersonAccount.person_account_id == 0)
                        return true;
                }
                else if (icdoPersonAccount.plan_id == busConstant.PlanIdFlex)
                {
                    busPersonAccountFlexComp lbusPersonAccountFlexComp = new busPersonAccountFlexComp() { icdoPersonAccount = new cdoPersonAccount() };
                    lbusPersonAccountFlexComp.icdoPersonAccount = icdoPersonAccount;
                    if (lbusPersonAccountFlexComp.iclbModifiedHistory.IsNull())
                        lbusPersonAccountFlexComp.LoadModifiedFlexCompHistory();
                    busPersonAccountFlexCompHistory lbusPAFlexCompModifiedHistory = new busPersonAccountFlexCompHistory { icdoPersonAccountFlexCompHistory = new cdoPersonAccountFlexCompHistory() };

                    //PIR 26356 Already enrolled (Not NH or ANNE) – Flex – Can submit change but don’t autopost 
                    if (astrReasonValue != busConstant.ChangeReasonNewHire && astrReasonValue != busConstant.ChangeReasonAnnualEnrollment)
                        return false;

                    DataTable ldtMemberWasEnrolledAsPreTax = busBase.Select("entPersonAccount.IsMemberWasEnrolledAsPreTax",
                                                                    new object[2] { adtmDateOfChange.Year, icdoPersonAccount.person_id });
                    if (ldtMemberWasEnrolledAsPreTax.IsNull() || ldtMemberWasEnrolledAsPreTax.Rows.Count == 0)
                        return false;

                    if (lbusPersonAccountFlexComp.IsNotNull() &&
                        lbusPersonAccountFlexComp.iclbModifiedHistory.IsNotNull() && lbusPersonAccountFlexComp.iclbModifiedHistory.Count() > 0)
                    {
                        lbusPAFlexCompModifiedHistory = lbusPersonAccountFlexComp.iclbModifiedHistory.Where(i => i.icdoPersonAccountFlexCompHistory.plan_participation_status_value != busConstant.PlanParticipationStatusFlexCancelled).OrderByDescending(i => i.icdoPersonAccountFlexCompHistory.effective_start_date).FirstOrDefault();
                    }

                    //PIR 26356 if enrollment is effective from next year onwards, then we dont need to throw any validation
                    LoadEmploymentStartDateLatestOrLinked();
                    if ((idtmLatestEndDatedEmplEndDate != DateTime.MinValue && idtmLatestEndDatedEmplEndDate.Year != adtmDateOfChange.Year)
                        || (idtmLinkedEmplEndDate != DateTime.MinValue && idtmLinkedEmplEndDate.Year != adtmDateOfChange.Year))
                        return false;

                    //PIR 26356 Person Account does not currently exist – Dental/Vision/Flex – can enroll in any Pre or Post tax during the first 30 days 
                    if (IsCurrentDateAfter31DaysOfEmployment() && icdoPersonAccount.person_account_id == 0)
                        return true;
                    //PIR 26356 Existing Person Account and last enroll < 31 days – Flex – Has to keep same as they had before transfer – Validation in MSS/posting logic (New Hire change reason)
                    else if (!IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days() && ldtMemberWasEnrolledAsPreTax.IsNotNull() && ldtMemberWasEnrolledAsPreTax.Rows.Count > 0
                            && icdoPersonAccount.person_account_id > 0 && astrReasonValue == busConstant.ChangeReasonNewHire && lbusPAFlexCompModifiedHistory.IsNotNull() &&
                            (adecFlexMSRAAmount != lbusPAFlexCompModifiedHistory.idecMsraAnnualPledgeAmount || adecFlexDCRAAmount != lbusPAFlexCompModifiedHistory.idecDcraAnnualPledgeAmount))
                        return true;
                    //PIR 26356 Last enroll > 31 days - Not eligible for change (cannot change amount)
                    else if (IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days() && icdoPersonAccount.person_account_id > 0 && lbusPAFlexCompModifiedHistory.IsNotNull() &&
                            (adecFlexMSRAAmount != lbusPAFlexCompModifiedHistory.idecMsraAnnualPledgeAmount || adecFlexDCRAAmount != lbusPAFlexCompModifiedHistory.idecDcraAnnualPledgeAmount))
                        return true;
                    //PIR 26428 / 26356 - > 31 days – if person account doesn’t exists then plan should not allow pre tax.
                    else if (IsPreviousEmployerProvidesFlexDuringSameCalenderYear(adtmDateOfChange, ablnIsCurrentPreTax)
                        && ablnIsCurrentPreTax && IsEmploymentGapGreaterThan31DaysForNewPersonAccounts() && icdoPersonAccount.person_account_id == 0)
                        return true;

                }
            }
            return false;
        }
        public bool IsPersonEmpDtlPrevLinkedEndDateNCurrentStartDateDiff31Days(DateTime adtmDateOfChange)
        {            
            DataTable ldtMemberWasEnrolledAsPreTax = Select("entPersonAccount.IsMemberWasEnrolledAsPreTax",
                                                                                new object[2] { adtmDateOfChange.Year, icdoPersonAccount.person_id });

            if (IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days() && ldtMemberWasEnrolledAsPreTax.IsNotNull() && ldtMemberWasEnrolledAsPreTax.Rows.Count > 0)
                return true;

            return false;
        }
        //PIR 26356/26428
        public bool IsCurrentDateAfter31DaysOfEmployment()
        {
            DataTable ldtPersonLatestEmploymentDtl = busBase.Select("entPersonEmploymentDetail.LoadLatestEmploymentDtl", new object[1] { icdoPersonAccount.person_id });

            if (ldtPersonLatestEmploymentDtl.Rows.Count > 0)
            {
                busPersonEmploymentDetail lobjPersonLatestEmploymentDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lobjPersonLatestEmploymentDtl.icdoPersonEmploymentDetail.LoadData(ldtPersonLatestEmploymentDtl.Rows[0]);

                if (icdoPersonAccount.person_account_id == 0 && 
                    ((icdoPersonAccount.history_change_date == DateTime.MinValue ? DateTime.Now : icdoPersonAccount.history_change_date) <= lobjPersonLatestEmploymentDtl.icdoPersonEmploymentDetail.start_date.AddDays(31)))
                {
                    return false;
                }
            }
            return true;
        }
        /// PIR 26356/26428 If a member has a Dental, Vision, or Life plan Enrolled in the current calendar year and is now re-enrolling with a new employer, for <> 31 days; 
        /// the additional check is if the prior employer had an Org Plan for Plan ID 18 during the same calendar year.
        /// </summary>
        /// <returns></returns>
        public bool IsPreviousEmployerProvidesFlexDuringSameCalenderYear(DateTime adtmDateOfChange, bool ablnIsCurrentPreTax)
        {
            //DataTable ldtMemberWasEnrolledAsPreTax = busBase.Select("entPersonAccount.IsMemberWasEnrolledAsPreTax",
            //                                                        new object[2] { adtmDateOfChange.Year, icdoPersonAccount.person_id });
            DataTable ldtIsPreviousEmployerProvidesFlex = busBase.Select("entPersonAccount.IsPreviousEmployerProvidesFlexInCurrentYear",
                                                        new object[2] { icdoPersonAccount.person_id, adtmDateOfChange.Year });

            //If member switched to Org which doesnt provide Flex plan, only allowed to Post Tax.
            if (ldtIsPreviousEmployerProvidesFlex.IsNullOrEmpty() || ldtIsPreviousEmployerProvidesFlex.Rows.Count == 0)
                return false;
            return true;
        }
        public bool IsCurrentEmployerProvidesFlexDuringSameCalenderYear(DateTime adtmDateOfChange)
        {
            DataTable ldtIsCurrentEmployerProvidesFlex = busBase.Select("entPersonAccount.IsCurrentEmployerProvidesFlexInCurrentYear",
                                new object[2] { icdoPersonAccount.person_id, adtmDateOfChange.Year });

            //If member switched to Org which doesnt provide Flex plan, only allowed to Post Tax.
            if (ldtIsCurrentEmployerProvidesFlex.IsNullOrEmpty() || ldtIsCurrentEmployerProvidesFlex.Rows.Count == 0)
                return false;
            return true;
        }
        /// <summary>
        /// PIR 26356/26428 if person account doesnt exists then we are checking latest 2 employments to get the date difference.
        /// </summary>
        /// <returns></returns>
        public bool IsEmploymentGapGreaterThan31DaysForNewPersonAccounts()
        {
            return (Convert.ToInt32(DBFunction.DBExecuteScalar("entPersonEmploymentDetail.IsEmploymentGapGreaterThan31DaysForNewPersonAccounts",
                                   new object[1] { icdoPersonAccount.person_id }, iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework))) > 31;
        }
        /// <summary>
        /// PIR 25221 - Added method to determine Gap between linked emp dtl end date and latest emp det start date is Greater than 31 days
        /// PIR 26356/26428 from LOB, since we need to Enroll from Person Emp Detail screen the latest employment detail and the linked employment detail are both the same
        /// Updated query "GetEmpDetIdLinkedToPersonAccountId" to get the latest linked person emp detail excluding the non end dated ones.
        /// </summary>
        /// <returns></returns>
        public bool IsPersonLinkedNLatestEmpDtlGapGreaterThan31Days()
        {
            DataTable ldtEmpDetIdLinkedToPersonAccountId = busBase.Select("entPersonEmploymentDetail.GetEmpDetIdLinkedToPersonAccountId",
                                                                                            new object[2] { icdoPersonAccount.person_id, icdoPersonAccount.person_account_id });

            DataTable ldtPersonLatestEmploymentDtl = busBase.Select("entPersonEmploymentDetail.LoadLatestEmploymentDtl", new object[1] { icdoPersonAccount.person_id });

            if (ldtPersonLatestEmploymentDtl.Rows.Count > 0 && ldtEmpDetIdLinkedToPersonAccountId.Rows.Count > 0)
            {
                busPersonEmploymentDetail lobjPersonEmpDetLinked = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lobjPersonEmpDetLinked.icdoPersonEmploymentDetail.LoadData(ldtEmpDetIdLinkedToPersonAccountId.Rows[0]);

                busPersonEmploymentDetail lobjPersonLatestEmploymentDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
                lobjPersonLatestEmploymentDtl.icdoPersonEmploymentDetail.LoadData(ldtPersonLatestEmploymentDtl.Rows[0]);

                if (lobjPersonEmpDetLinked.icdoPersonEmploymentDetail.person_employment_dtl_id != lobjPersonLatestEmploymentDtl.icdoPersonEmploymentDetail.person_employment_dtl_id &&
                    (lobjPersonEmpDetLinked.icdoPersonEmploymentDetail.end_date.Date.AddDays(31) < lobjPersonLatestEmploymentDtl.icdoPersonEmploymentDetail.start_date.Date))
                {
                    return true;
                }
            }
            return false;
        }
        public void LoadEmploymentStartDateLatestOrLinked()
        {
            DataTable ldtEmpDetIdLinkedToPersonAccountId = busBase.Select("entPersonEmploymentDetail.GetEmpDetIdLinkedToPersonAccountId",
                                                                                new object[2] { icdoPersonAccount.person_id, icdoPersonAccount.person_account_id });

            DataTable ldtPersonLatestEmploymentDtl = busBase.Select("entPersonEmploymentDetail.LoadLatestEmploymentDtl", new object[1] { icdoPersonAccount.person_id });

            busPersonEmploymentDetail lobjPersonEmpDetLinked = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };
            busPersonEmploymentDetail lobjPersonLatestEmploymentDtl = new busPersonEmploymentDetail { icdoPersonEmploymentDetail = new cdoPersonEmploymentDetail() };

            if (ldtPersonLatestEmploymentDtl.Rows.Count > 0)
            {
                lobjPersonLatestEmploymentDtl.icdoPersonEmploymentDetail.LoadData(ldtPersonLatestEmploymentDtl.Rows[0]);
                    idtmLinkedEmplEndDate = lobjPersonLatestEmploymentDtl.icdoPersonEmploymentDetail.end_date;
            }
            if (ldtEmpDetIdLinkedToPersonAccountId.Rows.Count > 0)
            {
                lobjPersonEmpDetLinked.icdoPersonEmploymentDetail.LoadData(ldtEmpDetIdLinkedToPersonAccountId.Rows[0]);
                    idtmLatestEndDatedEmplEndDate = lobjPersonEmpDetLinked.icdoPersonEmploymentDetail.end_date;
            }
            idtmEmploymentStartDateOfLatestOrLinkedEmpl = (lobjPersonEmpDetLinked.icdoPersonEmploymentDetail.start_date > lobjPersonLatestEmploymentDtl.icdoPersonEmploymentDetail.start_date ? lobjPersonEmpDetLinked.icdoPersonEmploymentDetail.start_date : lobjPersonLatestEmploymentDtl.icdoPersonEmploymentDetail.start_date);
        }
        public DateTime idtmEmploymentStartDateOfLatestOrLinkedEmpl { get; set; }
        public DateTime idtmLinkedEmplEndDate { get; set; }
        public DateTime idtmLatestEndDatedEmplEndDate { get; set; }
        //PIR 24918
        public bool IsAdjustmentExistsPriorToEnteredEffectiveDate(int aintProviderOrgID, DateTime adtmBillingMonthAndYear, decimal adecMemberPremiumAmt, string astrPaymentMethod, string astrCoverageCode = null)
        {
            if (IsContributionEffectiveDatePriorToProviderOrgEffectiveDate(aintProviderOrgID, adtmBillingMonthAndYear))
            {
                if (ibusPaymentElection == null)
                    LoadPaymentElection();
                DataTable ldtbIBSDetailList = new DataTable();
                if ((icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth || icdoPersonAccount.plan_id == busConstant.PlanIdDental || icdoPersonAccount.plan_id == busConstant.PlanIdVision) && adecMemberPremiumAmt > 0)
                {
                    ldtbIBSDetailList = Select<cdoIbsDetail>(
                               new string[6] { "PERSON_ACCOUNT_ID", "BILLING_MONTH_AND_YEAR", "DETAIL_STATUS_VALUE", "MEMBER_PREMIUM_AMOUNT", "MODE_OF_PAYMENT_VALUE", "COVERAGE_CODE" },
                               new object[6] { icdoPersonAccount.person_account_id, adtmBillingMonthAndYear, busConstant.IBSHeaderStatusReview, Math.Round(adecMemberPremiumAmt, 2, MidpointRounding.AwayFromZero), astrPaymentMethod, astrCoverageCode }, null, null);
                }
                else if(adecMemberPremiumAmt < 0 && ibusPaymentElection.IsNotNull() && ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value.IsNotNullOrEmpty() && astrPaymentMethod != ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value)
                {
                    ldtbIBSDetailList = Select<cdoIbsDetail>(
                               new string[4] { "PERSON_ACCOUNT_ID", "BILLING_MONTH_AND_YEAR",  "MEMBER_PREMIUM_AMOUNT", "MODE_OF_PAYMENT_VALUE" },
                               new object[4] { icdoPersonAccount.person_account_id, adtmBillingMonthAndYear, Math.Round(adecMemberPremiumAmt, 2, MidpointRounding.AwayFromZero) * -1, ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value }, null, null);
                }
                else
                {
                    ldtbIBSDetailList = Select<cdoIbsDetail>(
                               new string[5] { "PERSON_ACCOUNT_ID", "BILLING_MONTH_AND_YEAR", "DETAIL_STATUS_VALUE", "MEMBER_PREMIUM_AMOUNT", "MODE_OF_PAYMENT_VALUE" },
                               new object[5] { icdoPersonAccount.person_account_id, adtmBillingMonthAndYear, busConstant.IBSHeaderStatusReview, Math.Round(adecMemberPremiumAmt, 2, MidpointRounding.AwayFromZero), astrPaymentMethod }, null, null);
                }

                Collection<busIbsDetail> lclbIBSDetail = GetCollection<busIbsDetail>(ldtbIBSDetailList, "icdoIbsDetail");

                if (ldtbIBSDetailList.Rows.Count > 0 && lclbIBSDetail.Any(i => i.icdoIbsDetail.detail_status_value == busConstant.IBSHeaderStatusPosted ||
                                                                                 i.icdoIbsDetail.detail_status_value == busConstant.IBSHeaderStatusReview))
                {
                    return false;
                }
            }
            else
                return false;

            return true;
        }
        /// <summary>
        /// PIR 24918 - Date limitation - when enrollment is saved, don’t create adjustments prior to effective date in new ref. 
        //(the date indicates a new contract period with a provider and adjustments prior to the date entered cannot be processed with the provider
        /// </summary>
        public bool IsContributionEffectiveDatePriorToProviderOrgEffectiveDate(int aintProviderOrgID, DateTime adtmBillingMonthAndYear)
        {
            DataTable ldtbList = null;
            if (aintProviderOrgID > 0)
            {
                ldtbList = SelectWithOperator<cdoCodeValue>(
                           new string[4] { "code_id", "Data1", "Data2", "Data3" },
                           new string[4] { "=", "=", "=", "<=" },
                           new object[4] { "7035", icdoPersonAccount.plan_id, aintProviderOrgID, adtmBillingMonthAndYear },
                           "code_serial_id");
            }
            return ldtbList.Rows.Count > 0 ? true : false;
        }
        //PIR 24918
        public bool IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(int aintProviderOrgID, DateTime adtmBillingMonthAndYear, decimal adecMemberPremiumAmt, string astrRecordType, string astrCoverageCode = null)
        {
            if (IsContributionEffectiveDatePriorToProviderOrgEffectiveDate(aintProviderOrgID, adtmBillingMonthAndYear))
            {
                DataTable ldtbEmployerPayrollDetailList = new DataTable();
                if (icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    ldtbEmployerPayrollDetailList = Select<cdoEmployerPayrollDetail>(
                               new string[4] { "PERSON_ACCOUNT_ID", "PAY_PERIOD_DATE", "PREMIUM_AMOUNT", "RECORD_TYPE_VALUE" },
                               new object[4] { icdoPersonAccount.person_account_id, adtmBillingMonthAndYear, Math.Round(adecMemberPremiumAmt, 2, MidpointRounding.AwayFromZero), astrRecordType }, null, null);
                }
                else if ((icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth || icdoPersonAccount.plan_id == busConstant.PlanIdDental || icdoPersonAccount.plan_id == busConstant.PlanIdVision) && astrRecordType != busConstant.PayrollDetailRecordTypeNegativeAdjustment)
                {
                    ldtbEmployerPayrollDetailList = Select<cdoEmployerPayrollDetail>(
                               new string[6] { "PERSON_ID", "PLAN_ID", "PAY_PERIOD_DATE", "PREMIUM_AMOUNT", "RECORD_TYPE_VALUE", "COVERAGE_CODE" },
                               new object[6] { icdoPersonAccount.person_id, icdoPersonAccount.plan_id, adtmBillingMonthAndYear, Math.Round(adecMemberPremiumAmt, 2, MidpointRounding.AwayFromZero), astrRecordType, astrCoverageCode }, null, null);
                }
                else
                {
                    ldtbEmployerPayrollDetailList = Select<cdoEmployerPayrollDetail>(
                               new string[5] { "PERSON_ID", "PLAN_ID", "PAY_PERIOD_DATE", "PREMIUM_AMOUNT", "RECORD_TYPE_VALUE" },
                               new object[5] { icdoPersonAccount.person_id, icdoPersonAccount.plan_id, adtmBillingMonthAndYear, Math.Round(adecMemberPremiumAmt, 2, MidpointRounding.AwayFromZero), astrRecordType }, null, null);
                }
                Collection<busEmployerPayrollDetail> lclbEmployerPayrollDetail = GetCollection<busEmployerPayrollDetail>(ldtbEmployerPayrollDetailList, "icdoEmployerPayrollDetail");

                if (ldtbEmployerPayrollDetailList.Rows.Count > 0 && lclbEmployerPayrollDetail.Any(i=> i.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusReview ||
                                                                                                      i.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusValid))
                {
                    return false;
                }
            }
            else
                return false;

            return true;
        }

		//PIR 26282
        public DateTime idtProjectedRetirementDateByService { get; set; }
        public void CalculateProjectedRetirementDateByService(int aintService, bool isCalTypeEstimate, decimal adecServiceEntered)
        {
            if (icdoPersonAccount.person_account_id > 0)
            {
                if (ibusPerson.IsNull())
                    LoadPerson();
                decimal ldecTotalVSC = Math.Round(ibusPerson.GetTotalVSCForPerson(icdoPersonAccount.plan_id == busConstant.PlanIdJobService, DateTime.MinValue,
                                                                                             isCalTypeEstimate, false, iintBenefitPlanId: icdoPersonAccount.plan_id), 4, MidpointRounding.AwayFromZero);              
                if (ldecTotalVSC <= aintService)
                {
                    DateTime ldtmLastContributionDate = LoadLastSalaryWithoutPersonAccount();
                    if (ldtmLastContributionDate != DateTime.MinValue)
                    {
                        int ldecRemainingService = aintService - Convert.ToInt32(ldecTotalVSC + adecServiceEntered);
                        idtProjectedRetirementDateByService = ldtmLastContributionDate.AddMonths(ldecRemainingService).AddMonths(1);
                        idtProjectedRetirementDateByService = new DateTime(idtProjectedRetirementDateByService.Year, idtProjectedRetirementDateByService.Month, 1);
                    }
                }
            }
        }
		//PIR 26282
        public DateTime LoadLastSalaryWithoutPersonAccount()
        {
            DataTable idtbLastSalaryWithoutPersonAccount = busBase.Select("entBenefitCalculationFasMonths.LoadLastSalaryRecord",
                                                            new object[3] { icdoPersonAccount.person_id, icdoPersonAccount.plan_id, 0 });
            
            return idtbLastSalaryWithoutPersonAccount.Rows.Count > 0 ? new DateTime(Convert.ToInt32(idtbLastSalaryWithoutPersonAccount.Rows[0]["pay_period_year"]),
                                        Convert.ToInt32(idtbLastSalaryWithoutPersonAccount.Rows[0]["pay_period_month"]), 01) 
                                        : DateTime.MinValue;
        }
        public Collection<busEmployerPayrollDetail> ComparePositiveNegativeColEmployerPayrollDetail()
        {
            // icolPosNegIbsDetail contains positive as well as negative records, so if positive and negative records are same with absolute values then don't take those records in new collection 'lcolIbsDetailWithoutDuplicate'
            Collection<busEmployerPayrollDetail> lcolEmployerPayrollDetailWithoutDuplicate = new Collection<busEmployerPayrollDetail>();
            Collection<busEmployerPayrollDetail> lcolFinal = new Collection<busEmployerPayrollDetail>();
            if (icolPosNegEmployerPayrollDtl.IsNotNull())
            {
                foreach (busEmployerPayrollDetail lbusEmployerPayrollPosNeg in icolPosNegEmployerPayrollDtl)
                {
                    busEmployerPayrollDetail lbusEmployerPayrollDetailCompare = icolPosNegEmployerPayrollDtl.Where(i => i.SrNo != lbusEmployerPayrollPosNeg.SrNo
                                                                                && i.icdoEmployerPayrollDetail.pay_period_date == lbusEmployerPayrollPosNeg.icdoEmployerPayrollDetail.pay_period_date
                                                                                //&& i.icdoEmployerPayrollDetail.record_type_value != lbusEmployerPayrollPosNeg.icdoEmployerPayrollDetail.record_type_value
                                                                                && i.icdoEmployerPayrollDetail.person_id == lbusEmployerPayrollPosNeg.icdoEmployerPayrollDetail.person_id
                                                                                && i.icdoEmployerPayrollDetail.plan_id == lbusEmployerPayrollPosNeg.icdoEmployerPayrollDetail.plan_id
                                                                                && i.icdoEmployerPayrollDetail.person_account_id == lbusEmployerPayrollPosNeg.icdoEmployerPayrollDetail.person_account_id
                                                                                ).FirstOrDefault();

                    if (lbusEmployerPayrollDetailCompare.IsNotNull())
                    {
                        LoadEmployerPayrollDeatilCoverageCode(lbusEmployerPayrollDetailCompare);
                        //if records are different then take that record in new collection
                        if (!(IsEmployerPayrollCollectionSame(lbusEmployerPayrollPosNeg.icdoEmployerPayrollDetail, lbusEmployerPayrollDetailCompare.icdoEmployerPayrollDetail, true)))
                        {
                            lcolEmployerPayrollDetailWithoutDuplicate.Add(lbusEmployerPayrollPosNeg);
                        }
                    }
                    else
                        lcolEmployerPayrollDetailWithoutDuplicate.Add(lbusEmployerPayrollPosNeg);
                }

                //let us check it in db Negative and Positive are the same - Should either update to Ignore or not insert at all.
                if (lcolEmployerPayrollDetailWithoutDuplicate.Count > 0)
                {
                    DateTime adtBillingMonth = lcolEmployerPayrollDetailWithoutDuplicate.Min(i => i.icdoEmployerPayrollDetail.pay_period_date);
                    cdoEmployerPayrollDetail lcdoEmployerPayrollDetail = lcolEmployerPayrollDetailWithoutDuplicate[0].icdoEmployerPayrollDetail;
                    //for medicare pass person_acccount_id
                    Collection<busEmployerPayrollDetail> lcolEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();

                    if (icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        DataTable ldtbEmployerPayrollDtl = busBase.Select<busEmployerPayrollDetail>(
                                                         new string[2] { "person_account_id", "PAY_PERIOD_DATE" },
                                                         new object[2] { icdoPersonAccount.person_account_id, adtBillingMonth }, null, null);
                        if (ldtbEmployerPayrollDtl.Rows.Count > 0)
                        {
                            lcolEmployerPayrollDtl = GetCollection<busEmployerPayrollDetail>(ldtbEmployerPayrollDtl, "icdoEmployerPayrollDetail");
                            lcolEmployerPayrollDtl = lcolEmployerPayrollDtl.Where(i => i.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusPosted ||
                                                                                        i.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusReview ||
                                                                                        i.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusValid).ToList().ToCollection();
                        }
                    }
                    else
                    {
                        lcolEmployerPayrollDtl = GetEmployerPayrollDetailsOfPerson(adtBillingMonth, icdoPersonAccount.person_id, icdoPersonAccount.plan_id);
                    }
                    lcolEmployerPayrollDtl = lcolEmployerPayrollDtl.OrderBy(i => i.icdoEmployerPayrollDetail.employer_payroll_detail_id).ToList().ToCollection();
                    foreach (busEmployerPayrollDetail lbusEmployerPayrollDtl in lcolEmployerPayrollDetailWithoutDuplicate)
                    {
                        Collection<busEmployerPayrollDetail> lColbusEmployerPayrollDetailCompare = lcolEmployerPayrollDtl.Where(i => i.icdoEmployerPayrollDetail.pay_period_date == lbusEmployerPayrollDtl.icdoEmployerPayrollDetail.pay_period_date
                                                            && i.icdoEmployerPayrollDetail.person_account_id == lbusEmployerPayrollDtl.icdoEmployerPayrollDetail.person_account_id
                                                            //&& i.icdoEmployerPayrollDetail.record_type_value != lbusEmployerPayrollDtl.icdoEmployerPayrollDetail.record_type_value
                                                            && i.icdoEmployerPayrollDetail.person_id == lbusEmployerPayrollDtl.icdoEmployerPayrollDetail.person_id
                                                            && i.icdoEmployerPayrollDetail.plan_id == lbusEmployerPayrollDtl.icdoEmployerPayrollDetail.plan_id
                                                            ).ToList().ToCollection();

                        if (lColbusEmployerPayrollDetailCompare.Count > 0)
                        {
                            bool isRecordFound = false;
                            foreach (busEmployerPayrollDetail lbusEmployerPayrollDetailCompare in lColbusEmployerPayrollDetailCompare)
                            {
                                LoadEmployerPayrollDeatilCoverageCode(lbusEmployerPayrollDetailCompare);

                                if (IsEmployerPayrollCollectionSame(lbusEmployerPayrollDtl.icdoEmployerPayrollDetail, lbusEmployerPayrollDetailCompare.icdoEmployerPayrollDetail, lbusEmployerPayrollDetailCompare.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusPosted ? false : true))
                                {
                                    if (lbusEmployerPayrollDetailCompare.icdoEmployerPayrollDetail.status_value == busConstant.PayrollDetailStatusPosted && lColbusEmployerPayrollDetailCompare.Where(i => i.icdoEmployerPayrollDetail.employer_payroll_detail_id > lbusEmployerPayrollDetailCompare.icdoEmployerPayrollDetail.employer_payroll_detail_id).Count() > 0)
                                        isRecordFound = true;
                                    else
                                    {
                                        if (lbusEmployerPayrollDetailCompare.icdoEmployerPayrollDetail.status_value != busConstant.PayrollDetailStatusPosted)
                                        {
                                            DBFunction.DBNonQuery("entEmployerPayrollHeader.UpdateEmplyerPayrollDtlStatusToIgnoreByDtlID", new object[1] { lbusEmployerPayrollDetailCompare.icdoEmployerPayrollDetail.employer_payroll_detail_id },
                                               iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                                        }
                                        isRecordFound = true;
                                    }
                                }
                            }
                            if (!isRecordFound)
                                lcolFinal.Add(lbusEmployerPayrollDtl);
                        }
                        else
                            lcolFinal.Add(lbusEmployerPayrollDtl);
                    }
                }
            }
            return lcolFinal;
        }

        private void LoadEmployerPayrollDeatilCoverageCode(busEmployerPayrollDetail lbusEmployerPayrollDetailCompare)
        {
            if (lbusEmployerPayrollDetailCompare.icdoEmployerPayrollDetail.coverage_code.IsNullOrEmpty() && (icdoPersonAccount.plan_id == busConstant.PlanIdDental || icdoPersonAccount.plan_id == busConstant.PlanIdVision
                                                || icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth))
            {
                busPersonAccountGhdv lbusPersonAccountGhdv = new busPersonAccountGhdv() { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
                lbusPersonAccountGhdv.FindGHDVByPersonAccountID(icdoPersonAccount.person_account_id);
                if (lbusPersonAccountGhdv.IsNotNull())
                {
                    busPersonAccountGhdvHistory lbusEndedHistory = lbusPersonAccountGhdv.LoadHistoryByDate(lbusEmployerPayrollDetailCompare.icdoEmployerPayrollDetail.pay_period_date);
                    if (lbusEndedHistory.IsNotNull())
                    {
                        if (icdoPersonAccount.plan_id == busConstant.PlanIdDental || icdoPersonAccount.plan_id == busConstant.PlanIdVision)
                            lbusEmployerPayrollDetailCompare.icdoEmployerPayrollDetail.coverage_code = lbusEndedHistory.icdoPersonAccountGhdvHistory.level_of_coverage_value;
                        else if (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth)
                            lbusEmployerPayrollDetailCompare.icdoEmployerPayrollDetail.coverage_code = lbusEndedHistory.icdoPersonAccountGhdvHistory.coverage_code;
                    }
                }
            }
        }

        //Employer Payroll Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
        public bool IsEmployerPayrollCollectionSame(cdoEmployerPayrollDetail lcdoPosNeg, cdoEmployerPayrollDetail lcdoCompare, bool IsCompareAbsolute)
        {
            bool lblnIsCurrentPosNegDtlIsNegAdjustment = false;
            bool lblnIsCurrentCompareDtlIsNegAdjustment = false;
            if (lcdoPosNeg.record_type_value == busConstant.RecordTypeNegativeAdjustment)
                lblnIsCurrentPosNegDtlIsNegAdjustment = true;
            if (lcdoCompare.record_type_value == busConstant.RecordTypeNegativeAdjustment)
                lblnIsCurrentCompareDtlIsNegAdjustment = true;
            if (lcdoPosNeg.plan_id == busConstant.PlanIdGroupHealth)
            {
                if (lcdoPosNeg.person_account_id == lcdoCompare.person_account_id &&
                    lcdoPosNeg.coverage_code == lcdoCompare.coverage_code &&
                    lcdoPosNeg.provider_org_id == lcdoCompare.provider_org_id)
                {
                    if (IsCompareAbsolute == true && lcdoPosNeg.premium_amount == lcdoCompare.premium_amount &&
                    lcdoPosNeg.premium_amount_from_enrollment == lcdoCompare.premium_amount_from_enrollment &&
                    lcdoPosNeg.group_health_fee_amount == lcdoCompare.group_health_fee_amount)
                        return true;
                    else if (IsCompareAbsolute == false &&
                    (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.premium_amount * -1 : lcdoPosNeg.premium_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.premium_amount * -1 : lcdoCompare.premium_amount) &&
                    (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.premium_amount_from_enrollment * -1 : lcdoPosNeg.premium_amount_from_enrollment) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.premium_amount_from_enrollment * -1 : lcdoCompare.premium_amount_from_enrollment) &&
                    (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.group_health_fee_amount * -1 : lcdoPosNeg.group_health_fee_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.group_health_fee_amount * -1 : lcdoCompare.group_health_fee_amount))
                        return true;
                }
            }
            else if (lcdoPosNeg.plan_id == busConstant.PlanIdDental || lcdoPosNeg.plan_id == busConstant.PlanIdVision)
            {
                if (lcdoPosNeg.person_account_id == lcdoCompare.person_account_id &&
                    lcdoPosNeg.coverage_code == lcdoCompare.coverage_code &&
                    lcdoPosNeg.provider_org_id == lcdoCompare.provider_org_id)
                {
                    if (IsCompareAbsolute == true && lcdoPosNeg.premium_amount == lcdoCompare.premium_amount &&
                        lcdoPosNeg.premium_amount_from_enrollment == lcdoCompare.premium_amount_from_enrollment)
                        return true;
                    else if (IsCompareAbsolute == false &&
                        (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.premium_amount * -1 : lcdoPosNeg.premium_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.premium_amount * -1 : lcdoCompare.premium_amount) &&
                        (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.premium_amount_from_enrollment * -1 : lcdoPosNeg.premium_amount_from_enrollment) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.premium_amount_from_enrollment * -1 : lcdoCompare.premium_amount_from_enrollment))
                        return true;
                }
            }
            else if (lcdoPosNeg.plan_id == busConstant.PlanIdMedicarePartD)
            {
                if (lcdoPosNeg.person_account_id == lcdoCompare.person_account_id &&
                    lcdoPosNeg.provider_org_id == lcdoCompare.provider_org_id)
                {
                    if (IsCompareAbsolute == true && lcdoPosNeg.premium_amount == lcdoCompare.premium_amount &&
                    lcdoPosNeg.premium_amount_from_enrollment == lcdoCompare.premium_amount_from_enrollment &&
                    lcdoPosNeg.lis_amount == lcdoCompare.lis_amount &&
                    lcdoPosNeg.lep_amount == lcdoCompare.lep_amount)
                        return true;
                    else if (IsCompareAbsolute == false &&
                    (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.premium_amount * -1 : lcdoPosNeg.premium_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.premium_amount * -1 : lcdoCompare.premium_amount) &&
                    (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.premium_amount_from_enrollment * -1 : lcdoPosNeg.premium_amount_from_enrollment) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.premium_amount_from_enrollment * -1 : lcdoCompare.premium_amount_from_enrollment) &&
                    (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.lis_amount * -1 : lcdoPosNeg.lis_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.lis_amount * -1 : lcdoCompare.lis_amount) &&
                    (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.lep_amount * -1 : lcdoPosNeg.lep_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.lep_amount * -1 : lcdoCompare.lep_amount))
                        return true;
                }
            }
            else if (lcdoPosNeg.plan_id == busConstant.PlanIdGroupLife)
            {
                if (lcdoPosNeg.person_account_id == lcdoCompare.person_account_id &&
                    lcdoPosNeg.provider_org_id == lcdoCompare.provider_org_id)
                {
                    if (IsCompareAbsolute == true &&
                       lcdoPosNeg.premium_amount == lcdoCompare.premium_amount &&
                       lcdoPosNeg.premium_amount_from_enrollment == lcdoCompare.premium_amount_from_enrollment &&
                       lcdoPosNeg.basic_premium == lcdoCompare.basic_premium &&
                       lcdoPosNeg.supplemental_premium == lcdoCompare.supplemental_premium &&
                       lcdoPosNeg.spouse_premium == lcdoCompare.spouse_premium &&
                       lcdoPosNeg.dependent_premium == lcdoCompare.dependent_premium &&
                       lcdoPosNeg.life_basic_coverage_amount == lcdoCompare.life_basic_coverage_amount &&
                       lcdoPosNeg.life_supp_coverage_amount == lcdoCompare.life_supp_coverage_amount &&
                       lcdoPosNeg.life_spouse_supp_coverage_amount == lcdoCompare.life_spouse_supp_coverage_amount &&
                       lcdoPosNeg.life_dep_supp_coverage_amount == lcdoCompare.life_dep_supp_coverage_amount &&
                       lcdoPosNeg.ad_and_d_basic_premium_rate == lcdoCompare.ad_and_d_basic_premium_rate &&
                       lcdoPosNeg.ad_and_d_supplemental_premium_rate == lcdoCompare.ad_and_d_supplemental_premium_rate)
                        return true;
                    else if (IsCompareAbsolute == false &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.premium_amount * -1 : lcdoPosNeg.premium_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.premium_amount * -1 : lcdoCompare.premium_amount) &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.premium_amount_from_enrollment * -1 : lcdoPosNeg.premium_amount_from_enrollment) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.premium_amount_from_enrollment * -1 : lcdoCompare.premium_amount_from_enrollment) &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.basic_premium * -1 : lcdoPosNeg.basic_premium) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.basic_premium * -1 : lcdoCompare.basic_premium) &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.supplemental_premium * -1 : lcdoPosNeg.supplemental_premium) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.supplemental_premium * -1 : lcdoCompare.supplemental_premium) &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.spouse_premium * -1 : lcdoPosNeg.spouse_premium) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.spouse_premium * -1 : lcdoCompare.spouse_premium) &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.dependent_premium * -1 : lcdoPosNeg.dependent_premium) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.dependent_premium * -1 : lcdoCompare.dependent_premium) &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.life_basic_coverage_amount * -1 : lcdoPosNeg.life_basic_coverage_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.life_basic_coverage_amount * -1 : lcdoCompare.life_basic_coverage_amount) &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.life_supp_coverage_amount * -1 : lcdoPosNeg.life_supp_coverage_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.life_supp_coverage_amount * -1 : lcdoCompare.life_supp_coverage_amount) &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.life_spouse_supp_coverage_amount * -1 : lcdoPosNeg.life_spouse_supp_coverage_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.life_spouse_supp_coverage_amount * -1 : lcdoCompare.life_spouse_supp_coverage_amount) &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.life_dep_supp_coverage_amount * -1 : lcdoPosNeg.life_dep_supp_coverage_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.life_dep_supp_coverage_amount * -1 : lcdoCompare.life_dep_supp_coverage_amount) &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.ad_and_d_basic_premium_rate * -1 : lcdoPosNeg.ad_and_d_basic_premium_rate) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.ad_and_d_basic_premium_rate * -1 : lcdoCompare.ad_and_d_basic_premium_rate) &&
                       (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.ad_and_d_supplemental_premium_rate * -1 : lcdoPosNeg.ad_and_d_supplemental_premium_rate) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.ad_and_d_supplemental_premium_rate * -1 : lcdoCompare.ad_and_d_supplemental_premium_rate))
                        return true;
                }
            }
            else if (lcdoPosNeg.plan_id == busConstant.PlanIdEAP)
            {
                if (lcdoPosNeg.person_account_id == lcdoCompare.person_account_id &&
                    lcdoPosNeg.provider_org_id == lcdoCompare.provider_org_id)

                    if (IsCompareAbsolute == true && lcdoPosNeg.premium_amount == lcdoCompare.premium_amount &&
                        lcdoPosNeg.premium_amount_from_enrollment == lcdoCompare.premium_amount_from_enrollment)
                        return true;
                    else if (IsCompareAbsolute == false &&
                        (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.premium_amount * -1 : lcdoPosNeg.premium_amount) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.premium_amount * -1 : lcdoCompare.premium_amount) &&
                        (lblnIsCurrentPosNegDtlIsNegAdjustment ? lcdoPosNeg.premium_amount_from_enrollment * -1 : lcdoPosNeg.premium_amount_from_enrollment) == (lblnIsCurrentCompareDtlIsNegAdjustment ? lcdoCompare.premium_amount_from_enrollment * -1 : lcdoCompare.premium_amount_from_enrollment))
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Check the Person account exist for retirement plan for that member has enrolled 
        /// if multiple account exists, take the first account
        /// get the optional election value, get the member type and max pretax match value
        /// return true if election value is less than pretax max 
        /// </summary>
        /// PIR 25920 New Plan DC 2025
        public bool IsEmployerMatchAvailableWithElection(DateTime adtEffectiveDate)
        {
            //return GetPersonRetirementOptionalValue() < GetMaxERPreTaxMatch();

            
            int lintaddl_ee_contribution_percent = 0;
            string lstMemberType = string.Empty;
            if (ibusPerson.IsNull()) LoadPerson();
            ibusPerson.LoadRetirementAccount();
            if (ibusPerson.iclbRetirementAccount.IsNotNull() && ibusPerson.iclbRetirementAccount.Count > 0 && ibusPerson.IsDBPersonAccountExists())
            {
                ibusPerson.ibusPersonAccountForOptionalElection = ibusPerson.iclbRetirementAccount.FirstOrDefault(objPersonAccount => objPersonAccount.icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled);
            }
            if (ibusPerson.ibusPersonAccountForOptionalElection.IsNotNull())
            {
                if (ibusPerson.ibusPersonAccountForOptionalElection.ibusPersonAccountRetirement == null)
                    ibusPerson.ibusPersonAccountForOptionalElection.LoadPersonAccountRetirement();
                //if (ibusPerson.ibusPersonAccountForOptionalElection.ibusPersonAccountRetirement.ibusHistoryAsofDate == null)
                ibusPerson.ibusPersonAccountForOptionalElection.ibusPersonAccountRetirement.LoadEnrolledHistoryByMonthYear(adtEffectiveDate);

                if (ibusPerson.ibusPersonAccountForOptionalElection.ibusPersonAccountRetirement.ibusHistoryAsofDate.icdoPersonAccountRetirementHistory.plan_participation_status_value == busConstant.PlanParticipationStatusRetirementEnrolled)
                {
                    lintaddl_ee_contribution_percent = ibusPerson.ibusPersonAccountForOptionalElection.ibusPersonAccountRetirement.ibusHistoryAsofDate.icdoPersonAccountRetirementHistory.addl_ee_contribution_percent;
                }

                
                if (ibusPersonEmploymentDetail.IsNull())
                    LoadPersonEmploymentDetail();
                if (ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id == 0)
                {
                    ibusPerson.LoadPersonEmployment();
                    if(ibusPerson.icolPersonEmployment.IsNotNull())
                    {
                        if (ibusPerson.icolPersonEmployment.Any(objPersonEmployment => busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, objPersonEmployment.icdoPersonEmployment.start_date, objPersonEmployment.icdoPersonEmployment.end_date)))
                            ibusPerson.ibusESSPersonEmployment =    ibusPerson.icolPersonEmployment.Where(objPersonEmployment => busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, objPersonEmployment.icdoPersonEmployment.start_date, objPersonEmployment.icdoPersonEmployment.end_date)).FirstOrDefault();
                        if (ibusPerson.ibusESSPersonEmployment.IsNotNull())
                        {
                            ibusPerson.ibusESSPersonEmployment.LoadPersonEmploymentDetail(true);
                            
                            if (ibusPerson.ibusESSPersonEmployment.icolPersonEmploymentDetail.Any(objPersonEmploymentDetail => busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, objPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, objPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date)))
                                ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail = ibusPerson.ibusESSPersonEmployment.icolPersonEmploymentDetail.Where(objPersonEmploymentDetail => busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, objPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date, objPersonEmploymentDetail.icdoPersonEmploymentDetail.end_date)).FirstOrDefault();

                            icdoPersonAccount.person_employment_dtl_id = ibusPerson.ibusESSPersonEmployment.ibusESSLatestPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;
                        }
                    }

                    LoadPersonEmploymentDetail();
                }
                ibusPerson.ibusPersonAccountForOptionalElection.icdoPersonAccount.person_employment_dtl_id = ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.person_employment_dtl_id;

                if (ibusPerson.ibusPersonAccountForOptionalElection.icdoPersonAccount.person_employment_dtl_id > 0)
                {
                    //ibusPerson.ibusPersonAccountForOptionalElection.icdoPersonAccount.person_employment_dtl_id = GetEmploymentDetailID();
                    if (ibusPerson.ibusPersonAccountForOptionalElection.ibusPersonEmploymentDetail.IsNull()) ibusPerson.ibusPersonAccountForOptionalElection.LoadPersonEmploymentDetail();
                    if (ibusPerson.ibusPersonAccountForOptionalElection.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value.IsNullOrEmpty())
                    {
                        ibusPerson.ibusPersonAccountForOptionalElection.ibusPersonEmploymentDetail.iclbAllPersonAccountEmpDtl.ForEach
                            (objPersonAccountEmpDtl => objPersonAccountEmpDtl.ibusPersonAccount?.ibusPersonAccountRetirement?.LoadEnrolledHistoryByMonthYear(adtEffectiveDate));
                        ibusPerson.ibusPersonAccountForOptionalElection.ibusPersonEmploymentDetail.LoadMemberType(adtEffectiveDate);
                    }
                    lstMemberType = ibusPerson.ibusPersonAccountForOptionalElection.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.derived_member_type_value;
                }

                busPlanRetirementRate lobjPlanRetirement = new busPlanRetirementRate();
                lobjPlanRetirement = busRateHelper.GetRatesForMemberTypeAndEffectiveDate(lstMemberType, adtEffectiveDate, ibusPerson.ibusPersonAccountForOptionalElection.icdoPersonAccount.plan_id);

                return lintaddl_ee_contribution_percent < lobjPlanRetirement.icdoPlanRetirementRate.er_pretax_match;

            }
            return false;
        }
    }
}
