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

#endregion

namespace NeoSpin.BusinessObjects
{
    public partial class busIbsDetail : busExtendBase
    {
		//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111 
        public int SrNo { get; set; }

        #region Business Properties
        private busIbsHeader _ibusIbsHeader;
        public busIbsHeader ibusIbsHeader
        {
            get { return _ibusIbsHeader; }
            set { _ibusIbsHeader = value; }
        }
        private busPerson _ibusPerson;
        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }
        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        private busPerson _ibusPersonPremiumFor;
        public busPerson ibusPersonPremiumFor
        {
            get { return _ibusPersonPremiumFor; }
            set { _ibusPersonPremiumFor = value; }
        }

        #endregion
        public void LoadIbsHeader()
        {
            if (_ibusIbsHeader == null)
            {
                _ibusIbsHeader = new busIbsHeader();
            }
            _ibusIbsHeader.FindIbsHeader(icdoIbsDetail.ibs_header_id);
        }
        public void LoadPerson()
        {
            if (_ibusPerson == null)
            {
                _ibusPerson = new busPerson();
            }
            _ibusPerson.FindPerson(_icdoIbsDetail.person_id);
        }
        public void LoadPremiumForMember(int AintPersonID)
        {
            if (_ibusPersonPremiumFor == null)
            {
                _ibusPersonPremiumFor = new busPerson();
            }
            _ibusPersonPremiumFor.FindPerson(AintPersonID);
        }

        public void LoadIbsPlanHistoryDetails()
        {
            if (icdoIbsDetail.plan_id == busConstant.PlanIdMedicarePartD)
            {
                busPersonAccountMedicarePartDHistory lobjMedicareHistory = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
				if (lobjMedicareHistory.iclbPersonAccountMedicarePartDHistory == null)
                    lobjMedicareHistory.LoadPersonAccountMedicarePartDHistory(icdoIbsDetail.person_id);
                this.iclbIBSLoadMedicarePartDHistory = lobjMedicareHistory.iclbPersonAccountMedicarePartDHistory;
            }

            LoadPersonAccount();

            if (icdoIbsDetail.plan_id == busConstant.PlanIdGroupLife)
            {
                busPersonAccountLife lobjLifeHistory = new busPersonAccountLife { icdoPersonAccountLife = new cdoPersonAccountLife() };
				lobjLifeHistory.icdoPersonAccount = ibusPersonAccount.icdoPersonAccount;
                if (lobjLifeHistory.iclbPersonAccountLifeHistory == null)
                    lobjLifeHistory.LoadHistory();
                this.iclbIBSLoadLifeHistory = lobjLifeHistory.iclbPersonAccountLifeHistory;
            }
            if(icdoIbsDetail.plan_id == busConstant.PlanIdGroupHealth || icdoIbsDetail.plan_id == busConstant.PlanIdVision || icdoIbsDetail.plan_id == busConstant.PlanIdDental)
            {
                busPersonAccountGhdv lobjGHDVHistory = new busPersonAccountGhdv { icdoPersonAccountGhdv = new cdoPersonAccountGhdv() };
				if(ibusPersonAccount.ibusPersonAccountGHDV == null)
                ibusPersonAccount.LoadPersonAccountGHDV();
				LoadPersonaccountGhdvHistory();
            }
        }

        public void LoadPersonaccountGhdvHistory()
        {
            DataTable ldtbHistory = Select("cdoPersonAccountGhdv.LoadPersonAccountGHDVHistory", new object[1] { ibusPersonAccount.ibusPersonAccountGHDV.icdoPersonAccountGhdv.person_account_ghdv_id });
            iclbIBSLoadGHDVHistory = GetCollection<busPersonAccountGhdvHistory>(ldtbHistory, "icdoPersonAccountGhdvHistory");
        }

        private decimal _ldecTotalPremiumAmountForPAPIT;
        public decimal ldecTotalPremiumAmountForPAPIT
        {
            get { return _ldecTotalPremiumAmountForPAPIT; }
            set { _ldecTotalPremiumAmountForPAPIT = value; }
        }

        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            _ibusPlan.FindPlan(_icdoIbsDetail.plan_id);
        }

        public override busBase GetCorPerson()
        {
            if (_ibusPerson == null)
                LoadPerson();
            return _ibusPerson;
        }

        private Collection<busPersonAccountMedicarePartDHistory> _iclbIBSLoadMedicarePartDHistory;
        public Collection<busPersonAccountMedicarePartDHistory> iclbIBSLoadMedicarePartDHistory
        {
            get
            {
                return _iclbIBSLoadMedicarePartDHistory;
            }
            set
            {
                _iclbIBSLoadMedicarePartDHistory = value;
            }
        }


        private Collection<busIbsDetail> _iclbIBSPremiumFor;
        public Collection<busIbsDetail> iclbIBSPremiumFor
        {
            get
            {
                return _iclbIBSPremiumFor;
            }
            set
            {
                _iclbIBSPremiumFor = value;
            }
        }

        public DataTable idtbCachedLowIncomeCredit { get; set; }

        // PIR 26200 Replicate Button for IBS Detail
        public busIbsDetail btnReplicate_Clicked(int Aintibsdetailid)
        {
            busIbsDetail lobjIbsDetail = new busIbsDetail();
            if (lobjIbsDetail.FindIbsDetail(Aintibsdetailid))
            {
                lobjIbsDetail.LoadIbsHeader();
                lobjIbsDetail.LoadIbsPersonSummary();
                lobjIbsDetail.LoadPerson();
                lobjIbsDetail.LoadPersonAccountPremiumFor();
                lobjIbsDetail.LoadPlan();
                lobjIbsDetail.LoadIbsPlanHistoryDetails();
                // PROD PIR 933
                if (lobjIbsDetail.icdoIbsDetail.plan_id == busConstant.PlanIdGroupHealth &&
                    lobjIbsDetail.icdoIbsDetail.total_premium_amount < 0M)
                {
                    lobjIbsDetail.LoadPersonAccountDependentBillingLink();
                    lobjIbsDetail.LoadDependents();
                }
                if (lobjIbsDetail.icdoIbsDetail.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    //lobjIbsDetail.icdoIbsDetail.ldecLowIncomeCredit = lobjIbsDetail.icdoIbsDetail.lis_amount;
                    busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                    lobjMedicare.FindMedicareByPersonAccountID(lobjIbsDetail.icdoIbsDetail.person_account_id);
                    lobjMedicare.FindPersonAccount(lobjIbsDetail.icdoIbsDetail.person_account_id);

                    lobjMedicare.LoadPlanEffectiveDate();

                    //Low Income Credit Amount should be populated from Ref table. 
                    DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                    DateTime ldtEffectiveDate = new DateTime();
                    var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().OrderByDescending(i => i.Field<DateTime>("effective_date"));
                    foreach (DataRow dr in lenumList)
                    {
                        if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjIbsDetail.icdoIbsDetail.billing_month_and_year)
                        {
                            ldtEffectiveDate = Convert.ToDateTime(dr["effective_date"]).Date;
                            break;
                        }
                    }
                    DataTable ldtFilteredLowIncomeCredit = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<DateTime>("effective_date") == ldtEffectiveDate.Date).AsDataTable();

                    var lenumListFiltered = ldtFilteredLowIncomeCredit.AsEnumerable().Where(i => i.Field<Decimal>("amount") == Math.Abs(lobjIbsDetail.icdoIbsDetail.lis_amount)).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                    foreach (DataRow dr in lenumListFiltered)
                    {
                        if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjIbsDetail.icdoIbsDetail.billing_month_and_year)
                        {
                            lobjIbsDetail.icdoIbsDetail.idecLow_Income_Credit = Convert.ToDecimal(dr["low_income_credit"]);
                            break;
                        }
                    }

                    lobjIbsDetail.LoadLowIncomeCreditRef();

                    lobjIbsDetail.icdoIbsDetail.iintPremiumForPersonId = lobjMedicare.icdoPersonAccount.person_id;//Display Premium for on screen
                }

            }

            lobjIbsDetail.icdoIbsDetail.istrProviderOrgCode = busGlobalFunctions.GetOrgCodeFromOrgId(lobjIbsDetail.icdoIbsDetail.provider_org_id);

            lobjIbsDetail.ienmPageMode = Sagitec.Common.utlPageMode.New;
            this.icdoIbsDetail.ienuObjectState = Sagitec.Common.ObjectState.Insert;
            this.ibusSoftErrors = null;
            this.icdoIbsDetail.ibs_detail_id = 0;
            this.icdoIbsDetail.comment = string.Empty;
            this.icdoIbsDetail.created_date = DateTime.MinValue;
            this.icdoIbsDetail.created_by = string.Empty;
            this.icdoIbsDetail.modified_by = string.Empty;
            this.icdoIbsDetail.modified_date = DateTime.MinValue;
            this.icdoIbsDetail.detail_status_value = busConstant.StatusReview;
            this.icdoIbsDetail.detail_status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(1210, icdoIbsDetail.detail_status_value);

            return lobjIbsDetail;
        }

        public busPersonAccount ibusPersonAccount { get; set; }
        public void LoadPersonAccount()
        {
            if (ibusPersonAccount == null)
                ibusPersonAccount = new busPersonAccount();

            ibusPersonAccount.FindPersonAccount(icdoIbsDetail.person_account_id);
        }

        public busPersonAccount ibusPersonAccountPremiumFor { get; set; }
        public void LoadPersonAccountPremiumFor()
        {
            if (ibusPersonAccountPremiumFor == null)
                ibusPersonAccountPremiumFor = new busPersonAccount();

            ibusPersonAccountPremiumFor.FindPersonAccount(icdoIbsDetail.person_account_id);
            if (ibusPersonAccountPremiumFor.icdoPersonAccount.person_account_id > 0)
                LoadPremiumForMember(ibusPersonAccountPremiumFor.icdoPersonAccount.person_id);
        }

        public busPayeeAccount ibusPayeeAccount { get; set; }

        public void LoadPayeeAccount(int aintPayeeAccount)
        {
            ibusPayeeAccount = new busPayeeAccount { icdoPayeeAccount = new cdoPayeeAccount() };
            ibusPayeeAccount.FindPayeeAccount(aintPayeeAccount);
        }

        public busIbsPersonSummary ibusIbsPersonSummary { get; set; }
        public void LoadIbsPersonSummary()
        {
            if (ibusIbsPersonSummary == null)
                ibusIbsPersonSummary = new busIbsPersonSummary();
            ibusIbsPersonSummary.FindIbsPersonSummaryByIbsHeaderAndPerson(icdoIbsDetail.ibs_header_id, icdoIbsDetail.person_id);
        }

        /// <summary>
        /// Method to create or update papit items for insurance premium 
        /// </summary>
        public void CreateOrUpdatePAPITItems()
        {
            busPayeeAccountPaymentItemType lobjPAPIT;
            decimal ldecAmount = 0.0M;
            DataTable ldtIBSInsuranceItems;
            string lstrWhere = string.Empty;
            //Loading Person account
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            //loading person account payment election
            if (ibusPersonAccount.ibusPaymentElection == null)
                ibusPersonAccount.LoadPaymentElection();
            //checking whether payee account exists and payment method is NDPERS pension check
            if (ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id != 0 &&
                ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentPensionCheck)
            {
                //loading payee account
                LoadPayeeAccount(ibusPersonAccount.ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id);
                //loading next benefit payment date for payee account
                ibusPayeeAccount.LoadNexBenefitPaymentDate();
                //forming the where condition for code value table to get item code
                lstrWhere = " code_id = 1708 and data1 = '" + icdoIbsDetail.plan_id.ToString() + "'";
                ldtIBSInsuranceItems = iobjPassInfo.isrvDBCache.GetCacheData("sgs_code_value", lstrWhere);
                if ((ldtIBSInsuranceItems != null) && (ldtIBSInsuranceItems.Rows.Count > 0))
                {
                    if (ibusIbsHeader == null)
                        LoadIbsHeader();
                    //getting active PAPIT entry
                    //PROD ISSUE: when the enrollment suspended with future date, payment items gets ended with the future date. but, when the next IBS Billing batch run,
                    //it just check the open item only (end date = null) and creates the new item based on that.
                    //Now, we have added the logic to handle the future suspended items too.
                    if (icdoIbsDetail.plan_id == busConstant.PlanIdMedicarePartD)
                        lobjPAPIT = ibusPayeeAccount.GetLatestPayeeAccountPaymentItemTypeMedicare(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(), ibusIbsHeader.icdoIbsHeader.billing_month_and_year, ibusPersonAccount.icdoPersonAccount.person_account_id);
                    else
                        lobjPAPIT = ibusPayeeAccount.GetLatestPayeeAccountPaymentItemType(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(), ibusIbsHeader.icdoIbsHeader.billing_month_and_year);
                    if (lobjPAPIT != null)
                    {
                        //prod pir 6033
                        //if an item exists as of future date and if payment election changes as of next benefit payment, we need to delete future item
                        //and create regular item effective next benefit payment date
                        if (lobjPAPIT.icdoPayeeAccountPaymentItemType.start_date > ibusPayeeAccount.idtNextBenefitPaymentDate)
                        {
                            lobjPAPIT.icdoPayeeAccountPaymentItemType.Delete();
                            ibusPayeeAccount.LoadPayeeAccountPaymentItemType();
                        }
                        else
                            ldecAmount = lobjPAPIT.icdoPayeeAccountPaymentItemType.amount;
                    }
                    ibusPayeeAccount.iintBatchScheudleID = busConstant.IBSBillingBatchID;
                    //checking whether premium amount changed and then create or update
                    //premium amount should not include rhic amount
                    //For Medicare Part D total premium amount (member amount + spouse amount) should be inserted.

                    DateTime adteEndDate = DateTime.MinValue;

                    if (icdoIbsDetail.istrPersonAccountParticipationStatus == busConstant.PlanParticipationStatusInsuranceSuspended && ibusPersonAccount.icdoPersonAccount.end_date != DateTime.MinValue && ibusPersonAccount.icdoPersonAccount.end_date > DateTime.Today)
                    {
                        adteEndDate = ibusPersonAccount.icdoPersonAccount.end_date;
                    }

                    if (icdoIbsDetail.plan_id == busConstant.PlanIdMedicarePartD)
                    {
                        if (Math.Round(ldecAmount, 2, MidpointRounding.AwayFromZero) != Math.Round(icdoIbsDetail.member_premium_amount, 2, MidpointRounding.AwayFromZero))
                            ibusPayeeAccount.CreatePayeeAccountPaymentItemType(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(),
                                                                                icdoIbsDetail.member_premium_amount,
                                                                                string.Empty,
                                                                                icdoIbsDetail.provider_org_id,
                                                                                ibusPayeeAccount.idtNextBenefitPaymentDate,
                                                                                adteEndDate,
                                                                                false, false, ibusPersonAccount.icdoPersonAccount.person_account_id, true);
                    }
                    else
                    {
                        if (Math.Round(ldecAmount, 2, MidpointRounding.AwayFromZero) != Math.Round(icdoIbsDetail.member_premium_amount, 2, MidpointRounding.AwayFromZero))
                            ibusPayeeAccount.CreatePayeeAccountPaymentItemType(ldtIBSInsuranceItems.Rows[0]["data2"].ToString(),
                                                                                icdoIbsDetail.member_premium_amount,
                                                                                string.Empty,
                                                                                icdoIbsDetail.provider_org_id,
                                                                                ibusPayeeAccount.idtNextBenefitPaymentDate,
                                                                                adteEndDate,
                                                                                false, false, 0, ablnIsFromIBS : true);
                    }
                }
            }

            //Releaseing Objects for Performance
            ibusPayeeAccount = null;
            lobjPAPIT = null;
            ldtIBSInsuranceItems = null;
        }
        
        //property to identify whether ibs adj is created online
        public bool iblnOnlineCreation { get; set; }

        private decimal _ldecLowIncomeCredit;
        public decimal ldecLowIncomeCredit
        {
            get { return _ldecLowIncomeCredit; }
            set { _ldecLowIncomeCredit = value; }
        }

        public override void BeforePersistChanges()
        {
            base.BeforePersistChanges();
            if (iblnOnlineCreation)
            {
                int lintMultiplier = 1;
                if (icdoIbsDetail.istrReportType == busConstant.IBSReportTypeNegative)
                {
                    lintMultiplier = -1;
                }
                icdoIbsDetail.member_premium_amount = Math.Abs(icdoIbsDetail.member_premium_amount) * lintMultiplier;
                icdoIbsDetail.provider_premium_amount = Math.Abs(icdoIbsDetail.provider_premium_amount) * lintMultiplier;
                icdoIbsDetail.total_premium_amount = Math.Abs(icdoIbsDetail.total_premium_amount) * lintMultiplier;
                icdoIbsDetail.group_health_fee_amt = Math.Abs(icdoIbsDetail.group_health_fee_amt) * lintMultiplier;
                icdoIbsDetail.buydown_amount = Math.Abs(icdoIbsDetail.buydown_amount) * lintMultiplier;
                icdoIbsDetail.medicare_part_d_amt = Math.Abs(icdoIbsDetail.medicare_part_d_amt) * lintMultiplier; //PIR 14271
                icdoIbsDetail.rhic_amount = Math.Abs(icdoIbsDetail.rhic_amount) * lintMultiplier;             
                icdoIbsDetail.js_rhic_amount = Math.Abs(icdoIbsDetail.js_rhic_amount) * lintMultiplier;
                icdoIbsDetail.othr_rhic_amount = Math.Abs(icdoIbsDetail.othr_rhic_amount) * lintMultiplier;
                icdoIbsDetail.life_basic_premium_amount = Math.Abs(icdoIbsDetail.life_basic_premium_amount) * lintMultiplier;
                icdoIbsDetail.life_dep_supp_premium_amount = Math.Abs(icdoIbsDetail.life_dep_supp_premium_amount) * lintMultiplier;
                icdoIbsDetail.life_spouse_supp_premium_amount = Math.Abs(icdoIbsDetail.life_spouse_supp_premium_amount) * lintMultiplier;
                icdoIbsDetail.life_supp_premium_amount = Math.Abs(icdoIbsDetail.life_supp_premium_amount) * lintMultiplier;
                icdoIbsDetail.life_basic_coverage_amount = Math.Abs(icdoIbsDetail.life_basic_coverage_amount) * lintMultiplier;
                icdoIbsDetail.life_dep_supp_coverage_amount = Math.Abs(icdoIbsDetail.life_dep_supp_coverage_amount) * lintMultiplier;
                icdoIbsDetail.life_spouse_supp_coverage_amount = Math.Abs(icdoIbsDetail.life_spouse_supp_coverage_amount) * lintMultiplier;
                icdoIbsDetail.life_supp_coverage_amount = Math.Abs(icdoIbsDetail.life_supp_coverage_amount) * lintMultiplier;
                icdoIbsDetail.ad_and_d_basic_premium_rate = Math.Abs(icdoIbsDetail.ad_and_d_basic_premium_rate) * lintMultiplier;
                icdoIbsDetail.ad_and_d_supplemental_premium_rate = Math.Abs(icdoIbsDetail.ad_and_d_supplemental_premium_rate) * lintMultiplier;
                icdoIbsDetail.ltc_member_five_yrs_premium_amount = Math.Abs(icdoIbsDetail.ltc_member_five_yrs_premium_amount) * lintMultiplier;
                icdoIbsDetail.ltc_member_three_yrs_premium_amount = Math.Abs(icdoIbsDetail.ltc_member_three_yrs_premium_amount) * lintMultiplier;
                icdoIbsDetail.ltc_spouse_five_yrs_premium_amount = Math.Abs(icdoIbsDetail.ltc_spouse_five_yrs_premium_amount) * lintMultiplier;
                icdoIbsDetail.ltc_spouse_three_yrs_premium_amount = Math.Abs(icdoIbsDetail.ltc_spouse_three_yrs_premium_amount) * lintMultiplier;
                
                busOrganization lobjProvider = new busOrganization();
                lobjProvider.FindOrganizationByOrgCode(icdoIbsDetail.istrProviderOrgCode);
                icdoIbsDetail.provider_org_id = lobjProvider.icdoOrganization.org_id;
                busPerson lobjPerson = new busPerson();
                
                if (icdoIbsDetail.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    lobjPerson.FindPerson(icdoIbsDetail.iintPremiumForPersonId);
                }
                else
                {
                    lobjPerson.FindPerson(icdoIbsDetail.person_id);
                }
                lobjPerson.LoadPersonAccountByPlan(icdoIbsDetail.plan_id);
                if (lobjPerson.icolPersonAccountByPlan.Count > 0)
                    icdoIbsDetail.person_account_id = lobjPerson.icolPersonAccountByPlan[0].icdoPersonAccount.person_account_id;

                if (!string.IsNullOrEmpty(icdoIbsDetail.rate_structure_code))
                    icdoIbsDetail.rate_structure_code = icdoIbsDetail.rate_structure_code.PadLeft(4, '0');
                if (!string.IsNullOrEmpty(icdoIbsDetail.coverage_code_value))
                    icdoIbsDetail.coverage_code_value = icdoIbsDetail.coverage_code_value.PadLeft(4, '0');

                if (icdoIbsDetail.plan_id == busConstant.PlanIdMedicarePartD)
                {
                    ldecLowIncomeCredit = icdoIbsDetail.lis_amount;
                    busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                    lobjMedicare.FindMedicareByPersonAccountID(icdoIbsDetail.person_account_id);

                    lobjMedicare.FindPersonAccount(icdoIbsDetail.person_account_id);

                    lobjMedicare.LoadPlanEffectiveDate();

                    lobjMedicare.GetPremiumAmountFromRef(icdoIbsDetail.billing_month_and_year);
                    
                    //Low Income Credit Amount should be populated from Ref table. 
                    Decimal ldecLowIncomeCreditAmount = 0;
                    DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                    var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == icdoIbsDetail.idecLow_Income_Credit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                    foreach (DataRow dr in lenumList)
                    {
                        if (Convert.ToDateTime(dr["effective_date"]).Date <= icdoIbsDetail.billing_month_and_year)
                        {
                            ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                            break;
                        }
                    }

                    icdoIbsDetail.lis_amount = Math.Abs(ldecLowIncomeCreditAmount) * lintMultiplier;
                    icdoIbsDetail.lep_amount = Math.Abs(icdoIbsDetail.lep_amount) * lintMultiplier;

                    icdoIbsDetail.provider_premium_amount = Math.Abs(lobjMedicare.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef) * lintMultiplier;
                    icdoIbsDetail.member_premium_amount = Math.Abs(icdoIbsDetail.provider_premium_amount - icdoIbsDetail.lis_amount + icdoIbsDetail.lep_amount) * lintMultiplier;
                    icdoIbsDetail.total_premium_amount = Math.Abs(icdoIbsDetail.member_premium_amount) * lintMultiplier;
                }
            }

            if (icdoIbsDetail.plan_id == busConstant.PlanIdMedicarePartD)
            {
                ldecLowIncomeCredit = icdoIbsDetail.lis_amount;
                busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                //PIR 23922	When creating a manual adjustment on a header we are not able to edit this before it is posted.
                busPerson lobjPerson = new busPerson();
                lobjPerson.FindPerson(icdoIbsDetail.iintPremiumForPersonId);
                lobjPerson.LoadPersonAccountByPlan(icdoIbsDetail.plan_id);
                if (lobjPerson.icolPersonAccountByPlan.Count > 0)
                    icdoIbsDetail.person_account_id = lobjPerson.icolPersonAccountByPlan[0].icdoPersonAccount.person_account_id;

                lobjMedicare.FindMedicareByPersonAccountID(icdoIbsDetail.person_account_id);
                lobjMedicare.FindPersonAccount(icdoIbsDetail.person_account_id);

                lobjMedicare.LoadPlanEffectiveDate();

                lobjMedicare.GetPremiumAmountFromRef(icdoIbsDetail.billing_month_and_year);

                //Low Income Credit Amount should be populated from Ref table. 
                Decimal ldecLowIncomeCreditAmount = 0;
                DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == icdoIbsDetail.idecLow_Income_Credit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                foreach (DataRow dr in lenumList)
                {
                    if (Convert.ToDateTime(dr["effective_date"]).Date <= icdoIbsDetail.billing_month_and_year)
                    {
                        ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                        break;
                    }
                }

                if (icdoIbsDetail.member_premium_amount < 0 && icdoIbsDetail.provider_premium_amount < 0)
                {
                    icdoIbsDetail.lis_amount = Math.Abs(ldecLowIncomeCreditAmount) * -1;
                    lobjMedicare.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef = Math.Abs(lobjMedicare.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef) * -1;
                    icdoIbsDetail.lep_amount = Math.Abs(icdoIbsDetail.lep_amount) * -1;
                }
                else
                    icdoIbsDetail.lis_amount = ldecLowIncomeCreditAmount;


                icdoIbsDetail.member_premium_amount = lobjMedicare.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef - icdoIbsDetail.lis_amount + icdoIbsDetail.lep_amount;
            }

            //PROD PIR 933
            if (icdoIbsDetail.plan_id == busConstant.PlanIdGroupHealth && icdoIbsDetail.total_premium_amount < 0M && iclbPADependent != null)
                InsertPADependentLink();
        }

        public override void AfterPersistChanges()
        {
            //PROD PIR 933
            if (icdoIbsDetail.plan_id == busConstant.PlanIdGroupHealth &&
                icdoIbsDetail.total_premium_amount < 0M)
            {
                LoadPersonAccountDependentBillingLink();
                LoadDependents();
            }
            base.AfterPersistChanges();
        }

        /// <summary>
        /// prod pir 6077 : life premium validation
        /// </summary>
        /// <returns>boolean value</returns>
        public bool IsLifePremiumValid()
        {
            bool lblnResult = true;

            if (icdoIbsDetail.member_premium_amount != (icdoIbsDetail.life_basic_premium_amount + icdoIbsDetail.life_dep_supp_premium_amount +
                icdoIbsDetail.life_spouse_supp_premium_amount + icdoIbsDetail.life_supp_premium_amount))
            {
                lblnResult = false;
            }
            return lblnResult;
        }

        public bool IsADAndDRateEntered()
        {
            bool lblnResult = true;
            if ((icdoIbsDetail.life_basic_premium_amount > 0 && icdoIbsDetail.ad_and_d_basic_premium_rate == 0) ||
                (icdoIbsDetail.life_supp_premium_amount > 0 && icdoIbsDetail.ad_and_d_supplemental_premium_rate == 0))
            {
                lblnResult = false;
            }
            return lblnResult;
        }

        public bool IsBasicCoverageAmountValid()
        {
            bool lblnResult = true;
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonAccountLife == null)
                ibusPersonAccount.LoadPersonAccountLife();
            
            if (icdoIbsDetail.life_basic_coverage_amount > 0)
            {
                if (!ibusPersonAccount.ibusPersonAccountLife.IsValidCoverageAmount(busConstant.LevelofCoverage_Basic, busConstant.LifeInsuranceTypeRetireeMember, 
                    icdoIbsDetail.life_basic_coverage_amount, icdoIbsDetail.billing_month_and_year))
                    lblnResult = false;
            }

            return lblnResult;
        }

        public bool IsSuppCoverageAmountValid()
        {
            bool lblnResult = true;
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonAccountLife == null)
                ibusPersonAccount.LoadPersonAccountLife();

            if (icdoIbsDetail.life_supp_coverage_amount > 0)
            {
                if (!ibusPersonAccount.ibusPersonAccountLife.IsValidCoverageAmount(busConstant.LevelofCoverage_Supplemental, busConstant.LifeInsuranceTypeRetireeMember,
                    icdoIbsDetail.life_supp_coverage_amount, icdoIbsDetail.billing_month_and_year))
                    lblnResult = false;
            }

            return lblnResult;
        }

        public bool IsSpouseSuppCoverageAmountValid()
        {
            bool lblnResult = true;
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonAccountLife == null)
                ibusPersonAccount.LoadPersonAccountLife();

            if (icdoIbsDetail.life_spouse_supp_coverage_amount > 0)
            {
                if (!ibusPersonAccount.ibusPersonAccountLife.IsValidCoverageAmount(busConstant.LevelofCoverage_SpouseSupplemental, busConstant.LifeInsuranceTypeRetireeMember,
                    icdoIbsDetail.life_spouse_supp_coverage_amount, icdoIbsDetail.billing_month_and_year))
                    lblnResult = false;
            }

            return lblnResult;
        }

        public bool IsDepSuppCoverageAmountValid()
        {
            bool lblnResult = true;
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPersonAccountLife == null)
                ibusPersonAccount.LoadPersonAccountLife();

            if (icdoIbsDetail.life_dep_supp_coverage_amount > 0)
            {
                if (!ibusPersonAccount.ibusPersonAccountLife.IsValidCoverageAmount(busConstant.LevelofCoverage_DependentSupplemental, busConstant.LifeInsuranceTypeRetireeMember,
                    icdoIbsDetail.life_dep_supp_coverage_amount, icdoIbsDetail.billing_month_and_year))
                    lblnResult = false;
            }

            return lblnResult;
        }

        /// <summary>
        /// method to insert person account dependent billing link
        /// </summary>
        /// <param name="adarrPADep">array of person account dependent table</param>
        public void InsertPersonAccountDependentBillingLink(DataRow[] adarrPADep, int aintPersonAccountID = 0)
        {
            //need to do only for health plan
            if (icdoIbsDetail.plan_id == busConstant.PlanIdGroupHealth)
            {
                if (aintPersonAccountID > 0)
                {
                    DataTable ldtPADep = Select<cdoPersonAccountDependent>(new string[1] { "person_account_id" },
                                                                        new object[1] { aintPersonAccountID }, null, null);
                    foreach (DataRow ldrPADep in ldtPADep.Rows)
                    {
                        if (busGlobalFunctions.CheckDateOverlapping(icdoIbsDetail.billing_month_and_year, Convert.ToDateTime(ldrPADep["start_date"]),
                            Convert.ToDateTime(ldrPADep["end_date"] == DBNull.Value ? "9999/12/31" : ldrPADep["end_date"]).GetLastDayofMonth()))
                        {
                            cdoPersonAccountDependentBillingLink lcdoPADepBillingLink = new cdoPersonAccountDependentBillingLink();
                            lcdoPADepBillingLink.ibs_detail_id = icdoIbsDetail.ibs_detail_id;
                            lcdoPADepBillingLink.person_account_dependent_id = Convert.ToInt32(ldrPADep["person_account_dependent_id"]);
                            lcdoPADepBillingLink.Insert();
                        }
                    }
                }
                else if(adarrPADep != null)
                {
                    //inserting into link table
                    foreach (DataRow ldrPADep in adarrPADep)
                    {
                        cdoPersonAccountDependentBillingLink lcdoPADepBillingLink = new cdoPersonAccountDependentBillingLink();
                        lcdoPADepBillingLink.ibs_detail_id = icdoIbsDetail.ibs_detail_id;
                        lcdoPADepBillingLink.person_account_dependent_id = Convert.ToInt32(ldrPADep["person_account_dependent_id"]);
                        lcdoPADepBillingLink.Insert();
                    }
                }
            }
        }

        #region PROD PIR 933

        public Collection<busPersonAccountDependent> iclbPADependent { get; set; }

        public void LoadDependents()
        {
            iclbPADependent = new Collection<busPersonAccountDependent>();
            if (iclbPADependentBillingLink.IsNull()) LoadPersonAccountDependentBillingLink();
            DataTable ldtbResults = Select<cdoPersonAccountDependent>(new string[1] { "PERSON_ACCOUNT_ID" }, new object[1] { icdoIbsDetail.person_account_id }, null, null);
            foreach (DataRow ldtrRow in ldtbResults.Rows)
            {
                busPersonAccountDependent lobjPADependent = new busPersonAccountDependent
                {
                    icdoPersonAccountDependent = new cdoPersonAccountDependent(),
                    icdoPersonDependent = new cdoPersonDependent()
                };
                lobjPADependent.icdoPersonAccountDependent.LoadData(ldtrRow);
                lobjPADependent.FindPersonDependent(lobjPADependent.icdoPersonAccountDependent.person_dependent_id);
                lobjPADependent.LoadDependentInfo();
                if (!iclbPADependentBillingLink.Where(lobj => lobj.icdoPersonAccountDependentBillingLink.person_account_dependent_id ==
                    lobjPADependent.icdoPersonAccountDependent.person_account_dependent_id).Any())
                    iclbPADependent.Add(lobjPADependent);
            }
        }

        private void InsertPADependentLink()
        {
            foreach (busPersonAccountDependent lobjPADependent in iclbPADependent)
            {
                if (lobjPADependent.icdoPersonDependent.is_selected_flag == busConstant.Flag_Yes)
                {
                    cdoPersonAccountDependentBillingLink lcdoPADependentLink = new cdoPersonAccountDependentBillingLink
                    {
                        person_account_dependent_id = lobjPADependent.icdoPersonAccountDependent.person_account_dependent_id,
                        ibs_detail_id = icdoIbsDetail.ibs_detail_id
                    };
                    lcdoPADependentLink.Insert();
                }
            }
        }

        public Collection<busPersonAccountDependentBillingLink> iclbPADependentBillingLink { get; set; }

        public void LoadPersonAccountDependentBillingLink()
        {
            if (iclbPADependentBillingLink.IsNull()) iclbPADependentBillingLink = new Collection<busPersonAccountDependentBillingLink>();
            DataTable ldtbResult = Select<cdoPersonAccountDependentBillingLink>(new string[1] { "IBS_DETAIL_ID" }, new object[1] { icdoIbsDetail.ibs_detail_id }, null, null);
            iclbPADependentBillingLink = GetCollection<busPersonAccountDependentBillingLink>(ldtbResult, "icdoPersonAccountDependentBillingLink");
            foreach (busPersonAccountDependentBillingLink lobjPADependentBillLink in iclbPADependentBillingLink)
                lobjPADependentBillLink.LoadPADependent();
        }

        public bool iblnIsCoverageNeedsToSplit
        {
            get
            {
                if (Convert.ToString(busGlobalFunctions.GetData1ByCodeValue(52, busConstant.CoverageCodeSplitFlag, iobjPassInfo)) == busConstant.Flag_Yes)
                    return true;
                return false;
            }
        }

        #endregion

        public override int Delete()
        {
            DataTable ldtDependentLink = Select<cdoPersonAccountDependentBillingLink>(new string[1] { "ibs_detail_id" },
                new object[1] { icdoIbsDetail.ibs_detail_id }, null, null);
            foreach (DataRow ldr in ldtDependentLink.Rows)
            {
                busPersonAccountDependentBillingLink lobjDepLink = new busPersonAccountDependentBillingLink { icdoPersonAccountDependentBillingLink = new cdoPersonAccountDependentBillingLink() };
                lobjDepLink.icdoPersonAccountDependentBillingLink.LoadData(ldr);
                lobjDepLink.icdoPersonAccountDependentBillingLink.Delete();
            }
            return base.Delete();
        }
        //FW Upgrade PIR 17194 - Validation rule is checking 
        //the header's status, which is null, so loading here.
        public override bool ValidateDelete()
        {
            if (this.ibusIbsHeader.IsNull()) LoadIbsHeader();
            return base.ValidateDelete();
        }
        public string istrPersonName
        {
            get 
            {
                if (ibusPersonPremiumFor != null)
                {
                    const string lstrSeperator = ", ";
                    const string lstrSpaceSeperator = " ";
                    StringBuilder lsb = new StringBuilder();
                    if (!String.IsNullOrEmpty(ibusPersonPremiumFor.icdoPerson.person_id.ToString()))
                    {
                        lsb.Append(ibusPersonPremiumFor.icdoPerson.person_id);
                    }
                    if (!String.IsNullOrEmpty(ibusPersonPremiumFor.icdoPerson.last_name))
                    {
                        lsb.Append(lstrSpaceSeperator + ibusPersonPremiumFor.icdoPerson.last_name);
                    }
                    if (!String.IsNullOrEmpty(ibusPersonPremiumFor.icdoPerson.first_name))
                    {
                        lsb.Append(lstrSeperator + ibusPersonPremiumFor.icdoPerson.first_name);
                    }
                    if (!String.IsNullOrEmpty(ibusPersonPremiumFor.icdoPerson.middle_name))
                    {
                        lsb.Append(lstrSpaceSeperator + ibusPersonPremiumFor.icdoPerson.middle_name);
                    }

                    return lsb.ToString();
                }
                else
                    return string.Empty;
            }
        }

        //Backlog PIR 8164 & 8165 Ignore functionality on detail
        public void btnIgnored_Clicked()
        {
            _icdoIbsDetail.detail_status_value = busConstant.PayrollDetailStatusIgnored;
            _icdoIbsDetail.Update();
        }

        public bool IsProviderOrgIdValid()
        {
            DataTable ldtblist = new DataTable();
            ldtblist = Select("cdoIbsDetail.IsProviderOrgIdValid", new object[3] { icdoIbsDetail.plan_id, icdoIbsDetail.istrProviderOrgCode, icdoIbsDetail.billing_month_and_year });
            if(ldtblist.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }


        public Collection<cdoLowIncomeCreditRef> iclbLowIncomeCreditRef { get; set; }
        public Collection<cdoLowIncomeCreditRef> LoadLowIncomeCreditRef()
        {
            busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };

            lobjMedicare.FindMedicareByPersonAccountID(icdoIbsDetail.person_account_id);
            lobjMedicare.FindPersonAccount(icdoIbsDetail.person_account_id);
            

            if (idtbCachedLowIncomeCredit == null)
                idtbCachedLowIncomeCredit = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);

            if (lobjMedicare.IsNotNull())
            {
                if (lobjMedicare.idtPlanEffectiveDate == DateTime.MinValue)
                    lobjMedicare.LoadPlanEffectiveDate();
            }
            else
                lobjMedicare.idtPlanEffectiveDate = DateTime.Now;
            
            DateTime ldtEffectiveDate = new DateTime();
            var lenumList = idtbCachedLowIncomeCredit.AsEnumerable().OrderByDescending(i => i.Field<DateTime>("effective_date"));
            foreach (DataRow dr in lenumList)
            {
                if (Convert.ToDateTime(dr["effective_date"]).Date <= lobjMedicare.idtPlanEffectiveDate.Date)
                {
                    ldtEffectiveDate = Convert.ToDateTime(dr["effective_date"]).Date;
                    break;
                }
            }
            DataTable ldtFilteredLowIncomeCredit = idtbCachedLowIncomeCredit.AsEnumerable().Where(i => i.Field<DateTime>("effective_date") == ldtEffectiveDate.Date).AsDataTable();

            iclbLowIncomeCreditRef = Sagitec.DataObjects.doBase.GetCollection<cdoLowIncomeCreditRef>(ldtFilteredLowIncomeCredit);
            iclbLowIncomeCreditRef.ForEach(i => i.display_credit = i.low_income_credit.ToString());
            ////Adding Empty Item Here since Framework has bug if you select the Last Item. Temporary Workaround
            var lcdoTempRef = new cdoLowIncomeCreditRef();
            lcdoTempRef.amount = 0;
            lcdoTempRef.low_income_credit = 0;
            lcdoTempRef.display_credit = string.Empty;
            iclbLowIncomeCreditRef.Add(lcdoTempRef);
            iclbLowIncomeCreditRef.OrderBy(i => i.low_income_credit);
            return iclbLowIncomeCreditRef;
        }

        public Collection<busPersonAccountLifeHistory> iclbIBSLoadLifeHistory { get; set; }

        public Collection<busPersonAccountGhdvHistory> iclbIBSLoadGHDVHistory { get; set; }
    }
}
