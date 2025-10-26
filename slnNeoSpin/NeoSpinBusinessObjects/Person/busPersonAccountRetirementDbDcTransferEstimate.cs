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

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountRetirementDbDcTransferEstimate : busPersonAccountRetirementDbDcTransferEstimateGen
    {
        private busPersonAccount _ibusPersonAccount;
        public busPersonAccount ibusPersonAccount
        {
            get
            {
                return _ibusPersonAccount;
            }
            set
            {
                _ibusPersonAccount = value;
            }
        }

        private busPlan _ibusPlan;
        public busPlan ibusPlan
        {
            get { return _ibusPlan; }
            set { _ibusPlan = value; }
        }

        private decimal _idecProjectedAnnualSalary;
        public decimal idecProjectedAnnualSalary
        {
            get { return _idecProjectedAnnualSalary; }
            set { _idecProjectedAnnualSalary = value; }
        }

        private decimal _idecTotalEmployeeContribution;
        public decimal idecTotalEmployeeContribution
        {
            get { return _idecTotalEmployeeContribution; }
            set { _idecTotalEmployeeContribution = value; }
        }

        private decimal _idecEmployeeInterest;
        public decimal idecEmployeeInterest
        {
            get { return _idecEmployeeInterest; }
            set { _idecEmployeeInterest = value; }
        }

        private decimal _idecEmployerInterest;
        public decimal idecEmployerInterest
        {
            get { return _idecEmployerInterest; }
            set { _idecEmployerInterest = value; }
        }

        private decimal _idecTotalEmployerContribution;
        public decimal idecTotalEmployerContribution
        {
            get { return _idecTotalEmployerContribution; }
            set { _idecTotalEmployerContribution = value; }
        }


        private decimal _idecTransferAmt;
        public decimal idecTransferAmt
        {
            get { return _idecTransferAmt; }
            set { _idecTransferAmt = value; }
        }

        private decimal _idecProjectedContribution;
        public decimal idecProjectedContribution
        {
            get { return _idecProjectedContribution; }
            set { _idecProjectedContribution = value; }
        }

        private decimal _idecProjectedInterest;
        public decimal idecProjectedInterest
        {
            get { return _idecProjectedInterest; }
            set { _idecProjectedInterest = value; }
        }

        public bool _iblnIsDBEndGreaterThanDCEligibility;
        public bool iblnIsDBEndGreaterThanDCEligibility
        {
            get { return _iblnIsDBEndGreaterThanDCEligibility; }
            set { _iblnIsDBEndGreaterThanDCEligibility = value; }
        }

        public void LoadPersonAccount()
        {
            if (_ibusPersonAccount == null)
            {
                _ibusPersonAccount = new busPersonAccount();
            }
            _ibusPersonAccount.FindPersonAccount(icdoPersonAccountRetirementDbDcTransferEstimate.person_account_id);
        }

        public void LoadPlan()
        {
            if (_ibusPlan == null)
            {
                _ibusPlan = new busPlan();
            }

            _ibusPlan.FindPlan(ibusPersonAccount.icdoPersonAccount.plan_id);
        }

        //public void LoadProjectedAnnualSalary(int Aintdbdctransferestimateid)
        //{
        //    DataTable ldtbList = Select<cdoPersonAccountRetirementDbDcTransferEstimate>(
        //       new string[1] { "db_dc_transfer_estimate_id" },
        //       new object[1] { Aintdbdctransferestimateid }, null, null);

        //    if (ldtbList.Rows.Count > 0)
        //        idecProjectedAnnualSalary = Convert.ToDecimal(ldtbList.Rows[0]["PROJ_MONTHLY_SALARY_AMOUNT"].ToString()) * 12;
        //}

        public void CalculateContributionsAndInterest(decimal AdecRate)
        {
            decimal ldecProjMonthlySalary = 0;

            int lintMonths = 0;
            decimal ldecMonthlyInt = 0;
            decimal ldecMonthlyContri = 0;
            decimal ldecPrevMonthInt = 0;
            decimal ldecPrevMonthContri = 0;
            decimal ldecProjEmployeeContri = 0;
            decimal ldecTotalInt = 0;
            decimal ldecPrevMonthTotal = 0;

            if (ibusPersonAccount == null)
                LoadPersonAccount();
            busPostingInterestBatch lobjInt = new busPostingInterestBatch();

            lintMonths = icdoPersonAccountRetirementDbDcTransferEstimate.proj_month_no;
            ldecProjMonthlySalary = icdoPersonAccountRetirementDbDcTransferEstimate.proj_monthly_salary_amount;


            ldecMonthlyContri = ldecProjMonthlySalary * (AdecRate / 100);
            ldecProjEmployeeContri = ldecMonthlyContri * lintMonths;

            for (int i = 0; i < lintMonths; i++)
            {
                ldecMonthlyInt = busInterestCalculationHelper.CalculateInterest(ldecMonthlyContri + ldecPrevMonthTotal, ibusPersonAccount.icdoPersonAccount.plan_id);
                ldecPrevMonthTotal = ldecPrevMonthTotal + ldecMonthlyContri + ldecMonthlyInt;

                ldecTotalInt += ldecMonthlyInt;
            }

            idecProjectedInterest = ldecTotalInt;
            idecProjectedContribution = ldecPrevMonthTotal;

        }

        public void LoadTotalEmployeeContributions()
        {
            decimal ldecAcctBal = 0;
            decimal ldecContriRate = 0;

            DataTable ldtbAccountBalance = busNeoSpinBase.Select("cdoPersonAccountRetirementDbDcTransferEstimate.GetBalanceForPersonAccount", new object[1] { icdoPersonAccountRetirementDbDcTransferEstimate.person_account_id });
            if (ldtbAccountBalance.Rows.Count > 0)
                ldecAcctBal = Convert.ToDecimal(ldtbAccountBalance.Rows[0]["ACCOUNT_BALANCE"].ToString());

            //idecTotalEmployeeContribution = idecProjectedContribution + idecProjectedInterest + ldecAcctBal;
            idecTotalEmployeeContribution = idecProjectedContribution + ldecAcctBal;
            idecTotalEmployeeContribution = decimal.Parse(idecTotalEmployeeContribution.ToString("#0.00"));
        }

        public void LoadTotalEmployerContributions()
        {
            decimal ldecActualContri = 0;
            decimal ldecMonthlyInt = 0;
            decimal ldecPrevActualContri = 0;
            decimal ldecPrevMonthlyInt = 0;
            decimal ldecTotalInt = 0;
            decimal ldecTotalContriLTD = 0;
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            busPostingInterestBatch lobjInt = new busPostingInterestBatch();

            DataTable ldtbActualContributions = busNeoSpinBase.Select("cdoPersonAccountRetirementDbDcTransferEstimate.GetActualEmployerContribution", new object[1] { icdoPersonAccountRetirementDbDcTransferEstimate.person_account_id });
            if (ldtbActualContributions.Rows.Count > 0)
            {
                for (int i = 0; i < ldtbActualContributions.Rows.Count; i++)
                {
                    ldecActualContri = Convert.ToDecimal(ldtbActualContributions.Rows[i]["ActualContribution"].ToString());
                    ldecMonthlyInt = busInterestCalculationHelper.CalculateInterest(ldecActualContri + ldecPrevActualContri + ldecPrevMonthlyInt, ibusPersonAccount.icdoPersonAccount.plan_id);
                    ldecTotalInt += ldecMonthlyInt;
                    ldecTotalContriLTD += ldecActualContri;

                    ldecPrevActualContri = ldecActualContri;
                    ldecPrevMonthlyInt = ldecMonthlyInt;
                }
            }

            //idecTotalEmployerContribution = ldecTotalContriLTD + ldecTotalInt + idecProjectedInterest + idecProjectedContribution;
            idecTotalEmployerContribution = ldecTotalContriLTD + ldecTotalInt + idecProjectedContribution;
            idecTotalEmployerContribution = decimal.Parse(idecTotalEmployerContribution.ToString("#0.00"));
        }

        public void LoadTransferAmt()
        {
            idecTransferAmt = idecTotalEmployerContribution + idecTotalEmployeeContribution;

            icdoPersonAccountRetirementDbDcTransferEstimate.proj_total_ee_amount = idecTotalEmployeeContribution;
            icdoPersonAccountRetirementDbDcTransferEstimate.proj_total_er_amount = idecTotalEmployerContribution;
            icdoPersonAccountRetirementDbDcTransferEstimate.proj_ee_interest_amount = idecEmployeeInterest;
            icdoPersonAccountRetirementDbDcTransferEstimate.proj_er_interest_amount = idecEmployerInterest;
            icdoPersonAccountRetirementDbDcTransferEstimate.proj_total_transfer_amount = idecTransferAmt;
        }
        public override void BeforePersistChanges()
        {
            decimal ldecRate = 0;

            if (icdoPersonAccountRetirementDbDcTransferEstimate.suppress_warnings_flag == busConstant.Flag_Yes)
            {
                icdoPersonAccountRetirementDbDcTransferEstimate.suppress_warnings_by = iobjPassInfo.istrUserID;
                icdoPersonAccountRetirementDbDcTransferEstimate.suppress_warnings_date = DateTime.Now;
            }
            else
            {
                icdoPersonAccountRetirementDbDcTransferEstimate.suppress_warnings_by = string.Empty;
            }

            busPersonAccountRetirement lbusPersonAccountRetitrement = new busPersonAccountRetirement();
            lbusPersonAccountRetitrement.FindPersonAccountRetirement(_ibusPersonAccount.icdoPersonAccount.person_account_id);

            cdoPlanRetirementRate lcdoPlanRetirementRate =
                busGlobalFunctions.GetRetirementRateForPlanDateCombination(
                    _ibusPersonAccount.icdoPersonAccount.plan_id,
                    DateTime.Now, lbusPersonAccountRetitrement.GetMemberType(0));

            ldecRate = lcdoPlanRetirementRate.ee_pre_tax + lcdoPlanRetirementRate.ee_post_tax + lcdoPlanRetirementRate.ee_emp_pickup;
            CalculateContributionsAndInterest(ldecRate);
            idecEmployeeInterest = idecProjectedInterest;
            LoadTotalEmployeeContributions();



            ldecRate = lcdoPlanRetirementRate.er_post_tax;
            CalculateContributionsAndInterest(ldecRate);
            idecEmployerInterest = idecProjectedInterest;
            LoadTotalEmployerContributions();
            LoadTransferAmt();
            IsDBEndDateGreaterThanDCEligibility(lbusPersonAccountRetitrement);
            base.BeforePersistChanges();
        }

        public override void AfterPersistChanges()
        {
            idecProjectedAnnualSalary = icdoPersonAccountRetirementDbDcTransferEstimate.proj_monthly_salary_amount * 12;
            base.AfterPersistChanges();
        }

        private void IsDBEndDateGreaterThanDCEligibility(busPersonAccountRetirement AbusPersonAccountRetitrement)
        {
            DateTime ldtHistoryChangeDate;
            DateTime ldtDCEligibilityDate;

            if (AbusPersonAccountRetitrement.ibusHistory == null)
                AbusPersonAccountRetitrement.LoadPreviousHistory();

            DataTable ldtbHistory = Select("cdoPersonAccountRetirement.LatestEndDateForEnrolled", new object[1] { AbusPersonAccountRetitrement.icdoPersonAccount.person_account_id });
            if (ldtbHistory.Rows.Count > 0)
            {
                AbusPersonAccountRetitrement.ibusHistory.icdoPersonAccountRetirementHistory.LoadData(ldtbHistory.Rows[0]);
            }

            if ((AbusPersonAccountRetitrement.icdoPersonAccount.history_change_date != DateTime.MinValue) && (AbusPersonAccountRetitrement.ibusHistory.icdoPersonAccountRetirementHistory.dc_eligibility_date != DateTime.MinValue))
            {
                ldtDCEligibilityDate = AbusPersonAccountRetitrement.ibusHistory.icdoPersonAccountRetirementHistory.dc_eligibility_date;
                ldtHistoryChangeDate = AbusPersonAccountRetitrement.icdoPersonAccount.history_change_date;

                if (ldtHistoryChangeDate > ldtDCEligibilityDate)
                    iblnIsDBEndGreaterThanDCEligibility = true;
                else
                    iblnIsDBEndGreaterThanDCEligibility = false;
            }
        }
    }
}