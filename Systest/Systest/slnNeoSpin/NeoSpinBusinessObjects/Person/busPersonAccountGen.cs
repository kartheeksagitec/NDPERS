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
using System.Globalization;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountGen : busPersonBase
    {
        public busPersonAccountGen()
        {

        }

        public Collection<busPersonAccount> icblPersonAccount { get; set; }

        public bool IsGhdvPlan
        {
            get
            {
                if ((icdoPersonAccount.plan_id == busConstant.PlanIdDental) ||
                    (icdoPersonAccount.plan_id == busConstant.PlanIdVision) ||
                    (icdoPersonAccount.plan_id == busConstant.PlanIdGroupHealth) ||
                    (icdoPersonAccount.plan_id == busConstant.PlanIdHMO) ||
                    (icdoPersonAccount.plan_id == busConstant.PlanIdMedicarePartD))
                {
                    return true;
                }
                return false;
            }
        }
        #region IBS Batch Letter Properties
        public DateTime last_day_of_current_month
        {
            get
            {
                DateTime ldate = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1);
                return ldate.AddDays(-1);
            }
        }
        public string istrLastDayofCurrentMonthFormatted
        {
            get
            {
                return last_day_of_current_month.ToString(busConstant.DateFormatLongDate);
            }
        }
        public DateTime idtFirstofCurrentMonth
        {
            get
            {
                DateTime ldate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                return ldate;
            }
        }
        public string istrFirstofCurrentMonthFormatted
        {
            get
            {
                return idtFirstofCurrentMonth.ToString(busConstant.DateFormatLongDate);
            }
        }
        public DateTime idtFirstofNextMonth
        {
            get
            {
                DateTime ldate = new DateTime(DateTime.Now.AddMonths(1).Year, DateTime.Now.AddMonths(1).Month, 1);
                return ldate;
            }
        }
        public string istrFirstofNextMonthFormatted
        {
            get
            {
                return idtFirstofNextMonth.ToString(busConstant.DateFormatLongDate);
            }
        }
        public DateTime idtFirstofPreviousMonth
        {
            get
            {
                DateTime ldate = new DateTime(DateTime.Now.AddMonths(-1).Year, DateTime.Now.AddMonths(-1).Month, 1);
                return ldate;
            }
        }
        public string istrFirstofPreviousMonthFormatted
        {
            get
            {
                return idtFirstofPreviousMonth.ToString(busConstant.DateFormatLongDate);
            }
        }
        public DateTime last_day_of_previous_month
        {
            get
            {
                DateTime ldate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                return ldate.AddDays(-1);
            }
        }
        public string istrLastDayofPreviousMonthFormatted
        {
            get
            {
                return last_day_of_previous_month.ToString(busConstant.DateFormatLongDate);
            }
        }
        public string premiummonth
        {
            get
            {
                return DateTime.Today.ToString(busConstant.DateFormatMonthYear, CultureInfo.CreateSpecificCulture("en-US"));
            }
        }
        public string previous_premium_month_in_mmyyyy
        {
            get
            {
                return last_day_of_previous_month.ToString(busConstant.DateFormatMonthYear, CultureInfo.CreateSpecificCulture("en-US"));
            }
        }
        private decimal _idecDueAmount;

        public decimal idecDueAmount
        {
            get { return _idecDueAmount; }
            set { _idecDueAmount = value; }
        }
        #endregion

        private busPersonAccountDeferredComp _ibusPersonDeferredComp;

        public busPersonAccountDeferredComp ibusPersonDeferredComp
        {
            get { return _ibusPersonDeferredComp; }
            set { _ibusPersonDeferredComp = value; }
        }
        private busPersonAccountGhdv _ibusPersonAccountGHDV;

        public busPersonAccountGhdv ibusPersonAccountGHDV
        {
            get { return _ibusPersonAccountGHDV; }
            set { _ibusPersonAccountGHDV = value; }
        }

        //Org to bill
        private busPersonAccountMedicarePartDHistory _ibusPersonAccountMedicarePartDHistory;

        public busPersonAccountMedicarePartDHistory ibusPersonAccountMedicarePartDHistory
        {
            get { return _ibusPersonAccountMedicarePartDHistory; }
            set { _ibusPersonAccountMedicarePartDHistory = value; }
        }

        private busOrganization _ibusProvider;

        public busOrganization ibusProvider
        {
            get { return _ibusProvider; }
            set { _ibusProvider = value; }
        }
        private busOrganization _ibusEPOProvider;

        public busOrganization ibusEPOProvider
        {
            get { return _ibusEPOProvider; }
            set { _ibusEPOProvider = value; }
        }

        private busPersonAccountRetirement _ibusPersonAccountRetirement;

        public busPersonAccountRetirement ibusPersonAccountRetirement
        {
            get { return _ibusPersonAccountRetirement; }
            set { _ibusPersonAccountRetirement = value; }
        }
        private cdoPersonAccount _icdoPersonAccount;
        public cdoPersonAccount icdoPersonAccount
        {
            get
            {
                return _icdoPersonAccount;
            }
            set
            {
                _icdoPersonAccount = value;
            }
        }
        private busPersonEmploymentDetail _ibusPersonEmploymentDetail;
        public busPersonEmploymentDetail ibusPersonEmploymentDetail
        {
            get { return _ibusPersonEmploymentDetail; }
            set { _ibusPersonEmploymentDetail = value; }
        }
        //PIR 25920 Get current Employment details
        private busPersonEmploymentDetail _ibusCurrentPersonEmploymentDetail;
        public busPersonEmploymentDetail ibusCurrentPersonEmploymentDetail
        {
            get { return _ibusCurrentPersonEmploymentDetail; }
            set { _ibusCurrentPersonEmploymentDetail = value; }
        }
        public bool FindPersonAccount(int Aintpersonaccountid)
        {
            bool lblnResult = false;
            if (_icdoPersonAccount == null)
            {
                _icdoPersonAccount = new cdoPersonAccount();
            }
            if (_icdoPersonAccount.SelectRow(new object[1] { Aintpersonaccountid }))
            {
                lblnResult = true;
            }
            return lblnResult;
        }
        private busOrgPlan _ibusOrgPlan;

        public busOrgPlan ibusOrgPlan
        {
            get { return _ibusOrgPlan; }
            set { _ibusOrgPlan = value; }
        }
        private Collection<busPersonAccountAchDetail> _iclbPersonAccountAchDetail;
        public Collection<busPersonAccountAchDetail> iclbPersonAccountAchDetail
        {
            get { return _iclbPersonAccountAchDetail; }
            set { _iclbPersonAccountAchDetail = value; }
        }

        private busOrgPlan _ibusProviderOrgPlan;

        public busOrgPlan ibusProviderOrgPlan
        {
            get { return _ibusProviderOrgPlan; }
            set { _ibusProviderOrgPlan = value; }
        }

        private busPerson _ibusPerson;

        public busPerson ibusPerson
        {
            get { return _ibusPerson; }
            set { _ibusPerson = value; }
        }

        private busBatchSchedule _ibusIBSBatchSchedule;

        public busBatchSchedule ibusIBSBatchSchedule
        {
            get { return _ibusIBSBatchSchedule; }
            set { _ibusIBSBatchSchedule = value; }
        }

        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        private Collection<busPersonAccountMissedDeposit> _iclbPersonAccountMissedDeposit;
        public Collection<busPersonAccountMissedDeposit> iclbPersonAccountMissedDeposit
        {
            get
            {
                return _iclbPersonAccountMissedDeposit;
            }
            set
            {
                _iclbPersonAccountMissedDeposit = value;
            }
        }

        public int iintMissedDepositCount
        {
            get
            {
                if (iclbPersonAccountMissedDeposit != null)
                    return iclbPersonAccountMissedDeposit.Count;
                return 0;
            }
        }

        public DateTime idteLastContributedDate { get; set; }

        public void LoadIBSBatchSchedule()
        {
            if (ibusIBSBatchSchedule == null)
            {
                ibusIBSBatchSchedule = new busBatchSchedule();
            }
            ibusIBSBatchSchedule.FindBatchSchedule(busConstant.IBSBillingBatchID);
        }

        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }
            if (idtbPlanCacheData != null)
                _ibusPlan.idtbPlanCacheData = idtbPlanCacheData;
            _ibusPlan.FindPlan(icdoPersonAccount.plan_id);
        }

        public void LoadOrgPlan()
        {
            LoadOrgPlan(icdoPersonAccount.current_plan_start_date_no_null);
        }
        public void LoadPersonAccountAchDetail()
        {
            DataTable ldtbList = Select<cdoPersonAccountAchDetail>(
                                      new string[1] { "person_account_id" },
                                      new object[1] { icdoPersonAccount.person_account_id }, null, null);
            iclbPersonAccountAchDetail = GetCollection<busPersonAccountAchDetail>(ldtbList, "icdoPersonAccountAchDetail");
            foreach (busPersonAccountAchDetail lobjAchDetail in iclbPersonAccountAchDetail)
            {
                if (lobjAchDetail.ibusBankOrg == null)
                    lobjAchDetail.LoadBankOrgByOrgID();
            }
        }
        public void LoadOrgPlan(DateTime adtEffectiveDate)
        {
            if (ibusPersonEmploymentDetail == null)
                LoadPersonEmploymentDetail();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                ibusPersonEmploymentDetail.LoadPersonEmployment();
            else if (ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment == null)
                ibusPersonEmploymentDetail.LoadPersonEmployment();

            if (ibusPersonEmploymentDetail.ibusPersonEmployment != null)
            {
                LoadOrgPlan(adtEffectiveDate, ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id);
            }
        }

        public void LoadOrgPlan(DateTime adtEffectiveDate, int aintOrgID)
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
                              new object[2] { aintOrgID, icdoPersonAccount.plan_id }, null, null);
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

        public void LoadOrgPlan(int aintOrgID)
        {
            if (_ibusOrgPlan == null)
                _ibusOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };

            if (aintOrgID != 0)
            {
                Collection<busOrgPlan> lclbOrgPlan = new Collection<busOrgPlan>();
                DataTable ldtbOrgPlan = Select<cdoOrgPlan>(
                                        new string[2] { "org_id", "plan_id" },
                                        new object[2] { aintOrgID, icdoPersonAccount.plan_id }, null, "PARTICIPATION_END_DATE desc");
                lclbOrgPlan = GetCollection<busOrgPlan>(ldtbOrgPlan, "icdoOrgPlan");

                foreach(busOrgPlan lobjOrgPlan in lclbOrgPlan)
                {
                    ibusOrgPlan = lobjOrgPlan;
                    break;
                }

            }
        }

        public void LoadProviderOrgPlan()
        {
            LoadProviderOrgPlan(icdoPersonAccount.current_plan_start_date_no_null);
        }

        public void LoadProviderOrgPlan(DateTime adtEffectiveDate)
        {
            if (_ibusProviderOrgPlan == null)
            {
                _ibusProviderOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            }
            Collection<busOrgPlan> lclbProviderOrgPlan = new Collection<busOrgPlan>();
            DataTable ldtbProviderOrgPlan = Select("cdoPersonAccount.GetProviderOrgPlan", new object[2] { icdoPersonAccount.plan_id, _ibusOrgPlan.icdoOrgPlan.org_plan_id });
            lclbProviderOrgPlan = GetCollection<busOrgPlan>(ldtbProviderOrgPlan, "icdoOrgPlan");
            foreach (busOrgPlan lobjOrgPlan in lclbProviderOrgPlan)
            {
                if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjOrgPlan.icdoOrgPlan.participation_start_date,
                                     lobjOrgPlan.icdoOrgPlan.participation_end_date))
                {
                    _ibusProviderOrgPlan = lobjOrgPlan;
                    break;
                }
            }

        }

        public void LoadProviderOrgPlanByProviderOrgID(int aintProviderOrgID, DateTime adtEffectiveDate)
        {
            if (_ibusProviderOrgPlan == null)
            {
                _ibusProviderOrgPlan = new busOrgPlan { icdoOrgPlan = new cdoOrgPlan() };
            }
            Collection<busOrgPlan> lclbProviderOrgPlan = new Collection<busOrgPlan>();
            DataTable ldtbList = Select<cdoOrgPlan>(
                              new string[2] { "org_id", "plan_id" },
                              new object[2] { aintProviderOrgID, icdoPersonAccount.plan_id }, null, null);
            lclbProviderOrgPlan = GetCollection<busOrgPlan>(ldtbList, "icdoOrgPlan");

            foreach (busOrgPlan lobjOrgPlan in lclbProviderOrgPlan)
            {
                if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjOrgPlan.icdoOrgPlan.participation_start_date,
                                     lobjOrgPlan.icdoOrgPlan.participation_end_date))
                {
                    _ibusProviderOrgPlan = lobjOrgPlan;
                    break;
                }
            }
           
        }

        //To store the Provider Org ID in the Person Account Table
        public void LoadActiveProviderOrgPlan(DateTime adtEffectiveDate)
        {
            if (_ibusProviderOrgPlan == null)
            {
                _ibusProviderOrgPlan = new busOrgPlan();
            }
            Collection<busOrgPlan> lclbProviderOrgPlanForCobaRetiree = new Collection<busOrgPlan>();
            DataTable ldtbProviderOrgPlan = Select("cdoPersonAccount.LoadProviderOrgPlanIfEmploymentNotExists", new object[1] { icdoPersonAccount.plan_id });
            lclbProviderOrgPlanForCobaRetiree = GetCollection<busOrgPlan>(ldtbProviderOrgPlan, "icdoOrgPlan");
            foreach (busOrgPlan lobjOrgPlan in lclbProviderOrgPlanForCobaRetiree)
            {
                if (busGlobalFunctions.CheckDateOverlapping(adtEffectiveDate, lobjOrgPlan.icdoOrgPlan.participation_start_date,
                    lobjOrgPlan.icdoOrgPlan.participation_end_date))
                {
                    _ibusProviderOrgPlan = lobjOrgPlan;
                    break;
                }
            }
            if (_ibusProviderOrgPlan.icdoOrgPlan.IsNull()) _ibusProviderOrgPlan.icdoOrgPlan = new cdoOrgPlan(); //PIR 19930       
        }

        public void LoadPerson()
        {
            if (_ibusPerson == null)
                _ibusPerson = new busPerson();
            _ibusPerson.FindPerson(icdoPersonAccount.person_id);
        }

        public DataTable idtbPlanCacheData { get; set; }

        public void LoadPersonEmploymentDetail()
        {
            if (_ibusPersonEmploymentDetail == null)
            {
                _ibusPersonEmploymentDetail = new busPersonEmploymentDetail();
            }
            _ibusPersonEmploymentDetail.FindPersonEmploymentDetail(icdoPersonAccount.person_employment_dtl_id);
            if (idtbPlanCacheData != null)
                _ibusPersonEmploymentDetail.idtbPlanCacheData = idtbPlanCacheData;
            _ibusPersonEmploymentDetail.LoadMemberType(icblPersonAccount);
        }

        public void LoadProvider()
        {
            if (ibusProvider == null)
            {
                ibusProvider = new busOrganization();
            }
            ibusProvider.FindOrganization(icdoPersonAccount.provider_org_id);
        }

        //This below two properties used in auto refund batch letter
        //busbenefitcalculation class can use this method - to be optimized -to inform karthi to change the code
        public DateTime GetLastcontibutionDate()
        {
            DataTable ldtbResult = busBase.Select("cdoBenefitCalculationFasMonths.LoadLastSalaryRecord",
                                                            new object[3] { icdoPersonAccount.person_id,
                                                                            icdoPersonAccount.plan_id,0 }); // For PIR ID 1920, to load RTW refunded FAS the
            // extra parameter is added to the query.
            if (ldtbResult.Rows.Count > 0)
            {
                busPersonAccountRetirementContribution lobjRetirementContribution = new busPersonAccountRetirementContribution();
                lobjRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();
                lobjRetirementContribution.icdoPersonAccountRetirementContribution.LoadData(ldtbResult.Rows[0]);
                idteLastContributedDate = new DateTime(lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                                                                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month, 01);
            }
            return idteLastContributedDate;
        }

        public string istrLastContributedDate
        {
            get
            {
                GetLastcontibutionDate();
                if (idteLastContributedDate == DateTime.MinValue)
                    return string.Empty;
                else
                    return idteLastContributedDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        public DateTime idtNextBenefiPaymentDate
        {
            get
            {
                return busPayeeAccountHelper.GetLastBenefitPaymentDate().AddMonths(1);
            }
        }

        public string istrNextBenefitPaymentDate
        {
            get
            {
                if (idtNextBenefiPaymentDate == DateTime.MinValue)
                    return string.Empty;
                else
                    return idtNextBenefiPaymentDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        //prod pir 6085 : need to get the payment date after 30 days from batch date
        public string istrNextBenefitPaymentDateAfter30Days
        {
            get
            {
                DateTime ldtBatchDate = busGlobalFunctions.GetSysManagementBatchDate();
                if (ldtBatchDate == DateTime.MinValue)
                    return string.Empty;
                else
                {
                    return ldtBatchDate.AddMonths(2).GetFirstDayofCurrentMonth().ToString(busConstant.DateFormatLongDate);
                }
            }
        }

        public Collection<busBenefitCalculationPersonAccount> iclbBenefitCalculationPersonAccount { get; set; }

        public void LoadBenefitCalculationPersonAccount()
        {
            DataTable ldtbResult = Select<cdoBenefitCalculationPersonAccount>(new string[1] { "person_account_id" },
                                        new object[1] { icdoPersonAccount.person_account_id }, null, null);
            iclbBenefitCalculationPersonAccount = GetCollection<busBenefitCalculationPersonAccount>(ldtbResult, "icdoBenefitCalculationPersonAccount");
        }
        public Collection<busBenefitApplicationPersonAccount> iclbBenefitAppicationPersonAccount { get; set; }

        public void LoadBenefitApplicationPersonAccount()
        {
            DataTable ldtbResult = Select<cdoBenefitApplicationPersonAccount>(new string[1] { "person_account_id" },
                                        new object[1] { icdoPersonAccount.person_account_id }, null, null);
            iclbBenefitAppicationPersonAccount = GetCollection<busBenefitApplicationPersonAccount>(ldtbResult, "icdoBenefitApplicationPersonAccount");
        }
        private Collection<busBenefitDroApplication> _iclbBenefitDROApplication;
        public Collection<busBenefitDroApplication> iclbBenefitDROApplication
        {
            get { return _iclbBenefitDROApplication; }
            set { _iclbBenefitDROApplication = value; }
        }

        public void LoadDROApplication()
        {
            DataTable ldtbList = Select<cdoBenefitDroApplication>(
                                    new string[2] { "member_perslink_id", "plan_id" },
                                    new object[2] { icdoPersonAccount.person_id, icdoPersonAccount.plan_id }, null, null);
            _iclbBenefitDROApplication = GetCollection<busBenefitDroApplication>(ldtbList, "icdoBenefitDroApplication");
        }
    }
}
