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
    [Serializable]
    public class busPersonAccountInsuranceContribution : busPersonAccountInsuranceContributionGen
    {
        //Property to contain From date of Transaction Date Range
        private DateTime _idtTransactionDateFrom;
        public DateTime idtTransactionDateFrom
        {
            get { return _idtTransactionDateFrom; }
            set { _idtTransactionDateFrom = value; }
        }

        //Property to contain To date of Transaction Date Range
        private DateTime _idtTransactionDateTo;
        public DateTime idtTransactionDateTo
        {
            get { return _idtTransactionDateTo; }
            set { _idtTransactionDateTo = value; }
        }

        //Property to contain From date of Effective Date Range
        private DateTime _idtEffectiveDateFrom;
        public DateTime idtEffectiveDateFrom
        {
            get { return _idtEffectiveDateFrom; }
            set { _idtEffectiveDateFrom = value; }
        }

        //Property to contain To date of Effective Date Range
        private DateTime _idtEffectiveDateTo;
        public DateTime idtEffectiveDateTo
        {
            get { return _idtEffectiveDateTo; }
            set { _idtEffectiveDateTo = value; }
        }

        //Property to conatain transaction type of Person Account
        private string _istrTransactionType;
        public string istrTransactionType
        {
            get { return _istrTransactionType; }
            set { _istrTransactionType = value; }
        }

        //Property to contain Source
        private string _istrSource;
        public string istrSource
        {
            get { return _istrSource; }
            set { _istrSource = value; }
        }

        //Property to store group by conditon
        private string _istrGroupBy;
        public busPaymentElectionAdjustment ibusPaymentElectionAdjustment { get; set; }
    
        public string istrGroupBy
        {
            get { return _istrGroupBy; }
            set { _istrGroupBy = value; }
        }

        //FW Upgrade PIR 15477 primary key was not getting stored into session storing table and throwing exception while finding page object with unique key appended by primary key.
        public override long iintPrimaryKey
        {
            get
            {
                if (this.iobjPassInfo.istrFormName == "wfmPremiumDetailLTDMaintenance")
                {
                    return icdoPersonAccountInsuranceContribution.person_account_id;
                }
                else
                {
                    return base.iintPrimaryKey;
                }
            }
        }

        public void LoadPaymentElectionAdjustment()
        {
            if (ibusPaymentElectionAdjustment.IsNull())
                ibusPaymentElectionAdjustment = new busPaymentElectionAdjustment()
                {
                    icdoPaymentElectionAdjustment = new cdoPaymentElectionAdjustment(),
                    ibusPersonAccount = new busPersonAccount() { icdoPersonAccount = new cdoPersonAccount(), ibusPlan = new busPlan() { icdoPlan = new cdoPlan() } },
                };
            LoadPersonAccount();
            ibusPaymentElectionAdjustment.ibusPersonAccount = ibusPersonAccount;
        }

        public ArrayList ResetSearchFilter()
        {
            ArrayList larrList = new ArrayList();
            idtTransactionDateFrom = DateTime.MinValue;
            idtTransactionDateTo = DateTime.MinValue;
            idtEffectiveDateFrom = DateTime.MinValue;
            idtEffectiveDateTo = DateTime.MinValue;
            istrTransactionType = null;
            istrSource = null;
            istrGroupBy = "1";
            larrList.Add(this);
            return larrList;
        }

        //Method to filter Insurance Contribution and bind to data grid
        public ArrayList FilterInsuranceContribution()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            ibusPersonAccount.LoadInsuranceLTD();
            ArrayList larrList = new ArrayList();
            bool lblnTransactionDateResult, lblnEffectiveDateResult, lblnTransactionResult, lblnSourceResult;
            ibusPersonAccount.iclbInsuranceContribution = new Collection<busPersonAccountInsuranceContribution>();

            busPersonAccountInsuranceContribution lobjPreviousInsuranceContribution = new busPersonAccountInsuranceContribution();
            lobjPreviousInsuranceContribution.icdoPersonAccountInsuranceContribution = new cdoPersonAccountInsuranceContribution();

            //Sorting the original collection in desc
            ibusPersonAccount.iclbOriginalInsuranceContribution = busGlobalFunctions.Sort<busPersonAccountInsuranceContribution>(
                                                "icdoPersonAccountInsuranceContribution.PremiumMonthForOrderby desc",
                                                ibusPersonAccount.iclbOriginalInsuranceContribution);

            //Looping through the original collection for Insurance contribution
            foreach (busPersonAccountInsuranceContribution lobjInsuranceContribution in ibusPersonAccount.iclbOriginalInsuranceContribution)
            {
                lblnTransactionDateResult = true;
                lblnEffectiveDateResult = true;
                lblnTransactionResult = true;
                lblnSourceResult = true;
                //Checking whether transaction date comes in the given Transaction date range                           
                if (lobjInsuranceContribution.icdoPersonAccountInsuranceContribution.transaction_date >= idtTransactionDateFrom &&
                    lobjInsuranceContribution.icdoPersonAccountInsuranceContribution.transaction_date <= ((idtTransactionDateTo == DateTime.MinValue) ? DateTime.MaxValue : idtTransactionDateTo))
                    lblnTransactionDateResult = true;
                else
                    lblnTransactionDateResult = false;
                //Checking whether effective date comes in the given Effective date range
                if (lobjInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date >= idtEffectiveDateFrom &&
                    lobjInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date <= ((idtEffectiveDateTo == DateTime.MinValue) ? DateTime.MaxValue : idtEffectiveDateTo))
                    lblnEffectiveDateResult = true;
                else
                    lblnEffectiveDateResult = false;
                //Checking whether any transaction type is selected and if selected filtering based on that
                if (istrTransactionType != null)
                {
                    if (istrTransactionType == lobjInsuranceContribution.icdoPersonAccountInsuranceContribution.transaction_type_value)
                        lblnTransactionResult = true;
                    else
                        lblnTransactionResult = false;
                }
                //Checking whether any source is selected and if selected, filtering based on that
                if (istrSource != null)
                {
                    if (istrSource == lobjInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_value)
                        lblnSourceResult = true;
                    else
                        lblnSourceResult = false;
                }
                //If all filtering conditions are True, adding the row to filtered collection
                if (lblnTransactionDateResult && lblnEffectiveDateResult && lblnTransactionResult && lblnSourceResult)
                {
                    int lintPrevMonth, lintPrevYear, lintCurrMonth, lintCurrYear;

                    if (lobjPreviousInsuranceContribution.icdoPersonAccountInsuranceContribution.PremiumMonth != null)
                    {
                        string[] lstrPrevPayPeriod = lobjPreviousInsuranceContribution.icdoPersonAccountInsuranceContribution.PremiumMonth.Split('/');
                        lintPrevMonth = Convert.ToInt32(lstrPrevPayPeriod[0]);
                        lintPrevYear = Convert.ToInt32(lstrPrevPayPeriod[1]);
                    }
                    else
                    {
                        lintPrevMonth = 0;
                        lintPrevYear = 0;
                    }

                    if (lobjInsuranceContribution.icdoPersonAccountInsuranceContribution.PremiumMonth != null)
                    {
                        string[] lstrCurrPayPeriod = lobjInsuranceContribution.icdoPersonAccountInsuranceContribution.PremiumMonth.Split('/');
                        lintCurrMonth = Convert.ToInt32(lstrCurrPayPeriod[0]);
                        lintCurrYear = Convert.ToInt32(lstrCurrPayPeriod[1]);
                    }
                    else
                    {
                        lintCurrMonth = 0;
                        lintCurrYear = 0;
                    }
                    //No group by
                    if (string.IsNullOrEmpty(istrGroupBy) || istrGroupBy == "1")
                    {
                        ibusPersonAccount.iclbInsuranceContribution.Add(lobjInsuranceContribution);
                    }
                    //Group by Month / Year
                    else if (istrGroupBy == "2")
                    {
                        if (lintCurrMonth == lintPrevMonth && lintCurrYear == lintPrevYear)
                        {
                            AddAllAmounts(lobjPreviousInsuranceContribution, lobjInsuranceContribution);
                        }
                        else
                        {
                            if (lobjPreviousInsuranceContribution.icdoPersonAccountInsuranceContribution.person_account_id != 0)
                                ibusPersonAccount.iclbInsuranceContribution.Add(lobjPreviousInsuranceContribution);
                            lobjPreviousInsuranceContribution = new busPersonAccountInsuranceContribution();
                            lobjPreviousInsuranceContribution = AssignToPreviousContribution(lobjInsuranceContribution);
                        }
                    }
                    //Group by FY
                    else if (istrGroupBy == "3")
                    {
                        //if pay period year is  same, then sum of pay period months should be either less than or equal to 6 or greater than or equal to 8
                        // or if pay period year difference is 1, then current pay period month should be between 4 and 12 and previous pay period month
                        // should be less than or equal to 3 (sorted in descending order)
                        if (((lintPrevYear == lintCurrYear) && (((lintPrevMonth <= 6) && (lintCurrMonth <= 6)) ||
                            ((lintPrevMonth >= 7 && lintPrevMonth <= 12) && (lintCurrMonth >= 7 && lintCurrMonth <= 12)))) ||
                            ((Math.Abs(lintPrevYear - lintCurrYear) == 1) && ((lintCurrMonth >= 7 && lintCurrMonth <= 12) && (lintPrevMonth <= 6))))
                        {
                            AddAllAmounts(lobjPreviousInsuranceContribution, lobjInsuranceContribution);
                        }
                        else
                        {
                            if (lobjPreviousInsuranceContribution.icdoPersonAccountInsuranceContribution.person_account_id != 0)
                                ibusPersonAccount.iclbInsuranceContribution.Add(lobjPreviousInsuranceContribution);
                            lobjPreviousInsuranceContribution = new busPersonAccountInsuranceContribution();
                            lobjPreviousInsuranceContribution = AssignToPreviousContribution(lobjInsuranceContribution);
                        }
                    }
                    //Group by CY
                    else if (istrGroupBy == "4")
                    {
                        if (lintPrevYear == lintCurrYear)
                        {
                            AddAllAmounts(lobjPreviousInsuranceContribution, lobjInsuranceContribution);
                        }
                        else
                        {
                            if (lobjPreviousInsuranceContribution.icdoPersonAccountInsuranceContribution.person_account_id != 0)
                                ibusPersonAccount.iclbInsuranceContribution.Add(lobjPreviousInsuranceContribution);
                            lobjPreviousInsuranceContribution = new busPersonAccountInsuranceContribution();
                            lobjPreviousInsuranceContribution = AssignToPreviousContribution(lobjInsuranceContribution);
                        }
                    }
                }
            }

            if (istrGroupBy == "2" || istrGroupBy == "3" || istrGroupBy == "4")
                ibusPersonAccount.iclbInsuranceContribution.Add(lobjPreviousInsuranceContribution);

            //Adding the filterd collection to array list and returning so as to bind to datagrid
            larrList.Add(this);
            return larrList;
        }

        //Method to make all the fields other than group by columns to null
        private busPersonAccountInsuranceContribution AssignToPreviousContribution(busPersonAccountInsuranceContribution aobjInsuranceContribution)
        {
            aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_ref_id = 0;
            aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_description = string.Empty;
            aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.transaction_type_description = string.Empty;
            aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date = DateTime.MinValue;
            aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.transaction_date = DateTime.MinValue;
            aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.created_by = string.Empty;
            aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.modified_by = string.Empty;
            aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.health_insurance_contribution_id = 0;
            aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_value = string.Empty;
            return aobjInsuranceContribution;
        }

        private void AddAllAmounts(busPersonAccountInsuranceContribution aobjPreviousInsuranceContribution, busPersonAccountInsuranceContribution aobjInsuranceContribution)
        {
            aobjPreviousInsuranceContribution.icdoPersonAccountInsuranceContribution.due_premium_amount +=
                aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.due_premium_amount;
            aobjPreviousInsuranceContribution.icdoPersonAccountInsuranceContribution.paid_premium_amount +=
                aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.paid_premium_amount;
            aobjPreviousInsuranceContribution.icdoPersonAccountInsuranceContribution.rhic_benefit_amount +=
                aobjInsuranceContribution.icdoPersonAccountInsuranceContribution.rhic_benefit_amount;
        }

        /// <summary>
        /// PROD PIR 4399 : need to create ibs 
        /// </summary>
        /// <returns></returns>
        public ArrayList btnAllocateRemittanceClick()
        {
            ArrayList larrList = new ArrayList();
            AllocateIBSRemittance();
            larrList.Add(this);
            return larrList;
        }

        public void AllocateIBSRemittance()
        {
            if (ibusPersonAccount == null)
                LoadPersonAccount();
            busBase lobjBase = new busBase();
            DataTable ldtbRemittance = busNeoSpinBase.Select("cdoIbsRemittanceAllocation.GetIBSRemittanceAllocationForPerson",
                                                                new object[2] { DateTime.Today, ibusPersonAccount.icdoPersonAccount.person_id });
            DataTable ldtbPaymentElectionAdjustment = busNeoSpinBase.Select("cdoPaymentElectionAdjustment.LoadApprovedRequestsForPersonAccount",
                                                                new object[1] { ibusPersonAccount.icdoPersonAccount.person_account_id });
            bool lblnPaymentElectionUpdated = false;
            string lstrTransactionType = string.Empty;
            try
            {
                foreach (DataRow ldtrRemittance in ldtbRemittance.Rows)
                {
                    busRemittance lobjRemittance = new busRemittance { icdoRemittance = new cdoRemittance() };
                    lobjRemittance.icdoRemittance.LoadData(ldtrRemittance);
                    lobjRemittance.ibusDeposit = new busDeposit { icdoDeposit = new cdoDeposit() };
                    lobjRemittance.ibusDeposit.icdoDeposit.LoadData(ldtrRemittance);

                    decimal ldecAvailableAmount = busEmployerReportHelper.GetRemittanceAvailableAmount(lobjRemittance.icdoRemittance.remittance_id);
                    if (ldecAvailableAmount > 0.00M)
                    {
                        decimal ldecTotalAllocatedAmount = 0;

                        DataTable ldtbIBSDue = busNeoSpinBase.Select("cdoPersonAccountInsuranceContribution.LoadIBSDueContributionsWithoutStausCheck",
                                                                            new object[1] { ibusPersonAccount.icdoPersonAccount.person_account_id });
                        Collection<busPersonAccountInsuranceContribution> lclbIBSDue = new Collection<busPersonAccountInsuranceContribution>();
                        lclbIBSDue = lobjBase.GetCollection<busPersonAccountInsuranceContribution>(ldtbIBSDue, "icdoPersonAccountInsuranceContribution");
                        foreach (busPersonAccountInsuranceContribution lobjContribution in lclbIBSDue)
                        {
                            lstrTransactionType = string.Empty;
                            //Post Available Amount
                            decimal ldecAllocatedAmount = ldecAvailableAmount - ldecTotalAllocatedAmount;
                            if (ldecAllocatedAmount > lobjContribution.icdoPersonAccountInsuranceContribution.balance_amount)
                                ldecAllocatedAmount = lobjContribution.icdoPersonAccountInsuranceContribution.balance_amount;
                            if (ldecAllocatedAmount > 0.00M)
                            {
                                //ucs - 038  addendum
                                if (lobjContribution.icdoPersonAccountInsuranceContribution.rec_order == "1" ||
                                    lobjContribution.icdoPersonAccountInsuranceContribution.rec_order == "0")
                                {
                                    lstrTransactionType = busConstant.TransactionTypeIBSPayment;
                                }
                                else
                                {
                                    lstrTransactionType = busConstant.PersonAccountTransactionTypeIBSAdjPayment;
                                }
                                PostPaidAmount(lobjContribution.icdoPersonAccountInsuranceContribution.person_account_id, ldecAllocatedAmount,
                                            lobjRemittance.icdoRemittance.remittance_id, lobjContribution.icdoPersonAccountInsuranceContribution.effective_date,
                                            lobjRemittance.ibusDeposit.icdoDeposit.payment_history_header_id, lstrTransactionType,
                                            lobjContribution.icdoPersonAccountInsuranceContribution.provider_org_id);

                                AllocateRemittance(ldecAllocatedAmount, lobjRemittance.icdoRemittance.remittance_id,
                                    lobjContribution.icdoPersonAccountInsuranceContribution.person_account_id,
                                    lobjContribution.icdoPersonAccountInsuranceContribution.effective_date);

                                ldecTotalAllocatedAmount += ldecAllocatedAmount;

                                //ucs - 038  addendum
                                //Changing payment election adjustment status to In payment once first payment is made
                                if (lstrTransactionType == busConstant.PersonAccountTransactionTypeIBSAdjPayment && !lblnPaymentElectionUpdated)
                                {
                                    foreach (DataRow dr in ldtbPaymentElectionAdjustment.Rows)
                                    {
                                        busPaymentElectionAdjustment lobjAdjustment = new busPaymentElectionAdjustment { icdoPaymentElectionAdjustment = new cdoPaymentElectionAdjustment() };
                                        lobjAdjustment.icdoPaymentElectionAdjustment.LoadData(dr);
                                        lobjAdjustment.icdoPaymentElectionAdjustment.status_value = busConstant.IBSAdjustmentStatusInPayment;
                                        lobjAdjustment.icdoPaymentElectionAdjustment.Update();
                                        lobjAdjustment = null;
                                    }
                                    lblnPaymentElectionUpdated = true;
                                }
                            }
                            if (ldecTotalAllocatedAmount == ldecAvailableAmount) break;
                        }
                    }
                }
                //ucs - 038 addendum
                //updating papit entry if last month amount is less than monthly amount
                DataTable ldtbPAPIT = busNeoSpinBase.Select("cdoPaymentElectionAdjustment.LoadPAPIT",
                                                                new object[2] { DateTime.Today, ibusPersonAccount.icdoPersonAccount.person_account_id });
                foreach (DataRow dr in ldtbPAPIT.Rows)
                {
                    busPayeeAccount lobjPayeeAccount = new busPayeeAccount();
                    if (lobjPayeeAccount.FindPayeeAccount(Convert.ToInt32(dr["payee_account_id"])))
                    {
                        int lintPITId = Convert.ToInt32(dr["payment_item_type_id"]);
                        decimal ldecAmount = Convert.ToDecimal(dr["balance_amount"]);
                        string lstrAcctNo = dr["account_number"] == DBNull.Value ? string.Empty : dr["account_number"].ToString();
                        int lintVendorOrgId = dr["vendor_org_id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["vendor_org_id"]);
                        lobjPayeeAccount.LoadNexBenefitPaymentDate();
                        lobjPayeeAccount.iintBatchScheudleID = dr["batch_schedule_id"] == DBNull.Value ? 0 : Convert.ToInt32(dr["batch_schedule_id"]);
                        lobjPayeeAccount.CreatePayeeAccountPaymentItemType(lintPITId, ldecAmount, lstrAcctNo, lintVendorOrgId,
                            lobjPayeeAccount.idtNextBenefitPaymentDate, DateTime.MinValue, false);
                    }
                }
                //ucs - 038 addendum
                //updating payment election adjustment status to completed
                DBFunction.DBNonQuery("cdoPaymentElectionAdjustment.UpdateRequestToCompletedForPersonAccount", new object[1] { ibusPersonAccount.icdoPersonAccount.person_account_id },
                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private void PostPaidAmount(int aintPersonAccountID, decimal adecPaidAmount, int aintRemittanceID, DateTime adtEffectiveDate, int aintPaymentHistoryHeaderID,
            string astrTransactionType, int aintProviderOrgID)
        {
            cdoPersonAccountInsuranceContribution lobjInsrContribution = new cdoPersonAccountInsuranceContribution();
            lobjInsrContribution.person_account_id = aintPersonAccountID;
            lobjInsrContribution.paid_premium_amount = adecPaidAmount;
            lobjInsrContribution.subsystem_ref_id = aintRemittanceID;
            lobjInsrContribution.subsystem_value = busConstant.SubSystemValueIBSPayment;
            lobjInsrContribution.transaction_date = DateTime.Today;
            lobjInsrContribution.transaction_type_value = astrTransactionType;
            lobjInsrContribution.effective_date = adtEffectiveDate;
            lobjInsrContribution.payment_history_header_id = aintPaymentHistoryHeaderID;
            lobjInsrContribution.provider_org_id = aintProviderOrgID;
            lobjInsrContribution.Insert();
        }
        private void AllocateRemittance(decimal adecTotalAmountTobeAllocated, int lintRemittanceID, int aintPersonAccountID,
            DateTime adtEffectiveDate)
        {
            cdoIbsRemittanceAllocation lcdoIBSRemittanceAllocation = new cdoIbsRemittanceAllocation();
            lcdoIBSRemittanceAllocation.allocated_amount = adecTotalAmountTobeAllocated;
            lcdoIBSRemittanceAllocation.remittance_id = lintRemittanceID;
            lcdoIBSRemittanceAllocation.person_account_id = aintPersonAccountID;
            lcdoIBSRemittanceAllocation.effective_date = adtEffectiveDate;
            lcdoIBSRemittanceAllocation.ibs_allocation_status_value = busConstant.IBSAllocationStatusAllocated;
            lcdoIBSRemittanceAllocation.Insert();
        }

        # region 22 Correspondence

        public override busBase GetCorPerson()
        {
            if (ibusPersonAccount.IsNull())
                LoadPersonAccount();
            if (ibusPersonAccount.ibusPerson.IsNull())
                ibusPersonAccount.LoadPerson();

            return ibusPersonAccount.ibusPerson;
        }

        //get the earliest start date
        public string istrEarliestMonthYear
        {
            get
            {
                string lstrEarliestMonthYear = string.Empty;

                if (ibusPersonAccount.iclbOriginalInsuranceContribution.IsNull())
                    ibusPersonAccount.LoadInsuranceLTD();
                if (ibusPersonAccount.iclbOriginalInsuranceContribution.Count > 0)
                    lstrEarliestMonthYear = ibusPersonAccount.iclbOriginalInsuranceContribution.LastOrDefault().icdoPersonAccountInsuranceContribution.PremiumMonth;

                string[] lstrPremiumMonth = lstrEarliestMonthYear.Split('/');

                DateTime ldtTempDate = new DateTime(Convert.ToInt32(lstrPremiumMonth[1]), Convert.ToInt32(lstrPremiumMonth[0]), 1);
                return ldtTempDate.ToString(busConstant.DateFormatLongDate);
            }
        }

        //get the latest start date
        public string istrLatestMonthYear
        {
            get
            {
                string lstrLatestMonthYear = string.Empty;
                if (ibusPersonAccount.iclbOriginalInsuranceContribution.IsNull())
                    ibusPersonAccount.LoadInsuranceLTD();
                if (ibusPersonAccount.iclbOriginalInsuranceContribution.Count > 0)
                    lstrLatestMonthYear = ibusPersonAccount.iclbOriginalInsuranceContribution.FirstOrDefault().icdoPersonAccountInsuranceContribution.PremiumMonth;

                string[] lstrPremiumMonth = lstrLatestMonthYear.Split('/');

                DateTime ldtTempDate = new DateTime(Convert.ToInt32(lstrPremiumMonth[1]), Convert.ToInt32(lstrPremiumMonth[0]), 1);
                return ldtTempDate.ToString(busConstant.DateFormatLongDate);
            }
        }
        # endregion
    }
}
