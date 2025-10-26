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
    public class busPersonAccountDeferredCompContribution : busPersonAccountDeferredCompContributionGen
	{
        //Property to contain Employer Organization Code
        private string _istrEmpOrgCodeID;
        public string istrEmpOrgCodeID
        {
            get { return _istrEmpOrgCodeID; }
            set { _istrEmpOrgCodeID = value; }
        }

        //Property to contain Provider Organization Code
        private string _istrPrvOrgCodeID;
        public string istrPrvOrgCodeID
        {
            get { return _istrPrvOrgCodeID; }
            set { _istrPrvOrgCodeID = value; }
        }

        //Property to contain From date of Pay Period Date Range
        private DateTime _idtPayPeriodDateFrom;
        public DateTime idtPayPeriodDateFrom
        {
            get { return _idtPayPeriodDateFrom; }
            set { _idtPayPeriodDateFrom = value; }
        }

        //Property to contain To date of Pay Period Date Range
        private DateTime _idtPayPeriodDateTo;
        public DateTime idtPayPeriodDateTo
        {
            get { return _idtPayPeriodDateTo; }
            set { _idtPayPeriodDateTo = value; }
        }

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

        //Property to contain From date of PayCheck Date Range
        private DateTime _idtPayCheckDateFrom;
        public DateTime idtPayCheckDateFrom
        {
            get { return _idtPayCheckDateFrom; }
            set { _idtPayCheckDateFrom = value; }
        }

        //Property to contain To date of PayCheck Date Range
        private DateTime _idtPayCheckDateTo;
        public DateTime idtPayCheckDateTo
        {
            get { return _idtPayCheckDateTo; }
            set { _idtPayCheckDateTo = value; }
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
        public string istrGroupBy
        {
            get { return _istrGroupBy; }
            set { _istrGroupBy = value; }
        }

		//used in 41 for display purpose.
        public string istrEmployerName { get; set; }
        public string istrProviderName { get; set; }

        public ArrayList ResetSearchFilter()
        {
            ArrayList larrList = new ArrayList();
            idtPayPeriodDateFrom = DateTime.MinValue;
            idtPayPeriodDateTo = DateTime.MinValue;
            idtPayCheckDateFrom = DateTime.MinValue;
            idtPayCheckDateTo = DateTime.MinValue;
            istrEmpOrgCodeID = null;
            istrPrvOrgCodeID = null;
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

        public bool iblnIsCYTDFlag { get; set; }

        //Method to filter DeferredComp Contribution and bind to data grid
        public ArrayList FilterDeferredCompContribution()
        {
            if(ibusPADeferredComp == null)
                LoadPersonAccountDeferredComp();
            if (iblnIsCYTDFlag)
                ibusPADeferredComp.LoadCYTDDetail();
            else
                ibusPADeferredComp.LoadLTDDetail();
            ArrayList larrList = new ArrayList();
            bool lblnEmpOrgResult, lblnPrvOrgResult, lblnPayPeriodResult, lblnPayCheckDateResult, lblnTransactionDateResult, lblnEffectiveDateResult, lblnTransactionResult, lblnSourceResult;
            ibusPADeferredComp.iclbDeferredCompContribution = new Collection<busPersonAccountDeferredCompContribution>();
            busPersonAccountDeferredCompContribution lobjPreviousDeferredCompContribution = new busPersonAccountDeferredCompContribution();
            lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution = new cdoPersonAccountDeferredCompContribution();

            ibusPADeferredComp.iclbOriginalDeferredCompContribution = busGlobalFunctions.Sort<busPersonAccountDeferredCompContribution>(
                "icdoPersonAccountDeferredCompContribution.PayPeriodYearMonth desc", ibusPADeferredComp.iclbOriginalDeferredCompContribution);

            //Looping through the original collection for retirement contribution
            foreach (busPersonAccountDeferredCompContribution lobjDeferredCompContribution in ibusPADeferredComp.iclbOriginalDeferredCompContribution)
            {
                lblnEmpOrgResult = true;
                lblnPrvOrgResult = true;
                lblnPayPeriodResult = true;
                lblnPayCheckDateResult = true;
                lblnTransactionDateResult = true;
                lblnEffectiveDateResult = true;
                lblnTransactionResult = true;
                lblnSourceResult = true;
                //Checking whether Employer org code id is entered and if entered filtering based on that
                if (istrEmpOrgCodeID != null)
                {
                    if (istrEmpOrgCodeID == lobjDeferredCompContribution.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code)
                        lblnEmpOrgResult = true;
                    else
                        lblnEmpOrgResult = false;
                }
                //Checking whether Provider org code id is entered and if entered filtering based on that
                if (istrPrvOrgCodeID != null)
                {
                    if (istrPrvOrgCodeID == lobjDeferredCompContribution.ibusProvider.icdoOrganization.org_code)
                        lblnPrvOrgResult = true;
                    else
                        lblnPrvOrgResult = false;
                }                
                //Checking whether Pay period comes in between the given Pay Period Range
                if (busGlobalFunctions.CheckDateOverlapping(lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_start_date,
                    lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_end_date,
                    idtPayPeriodDateFrom, ((idtPayPeriodDateTo == DateTime.MinValue) ? DateTime.MaxValue : idtPayPeriodDateTo)))
                    lblnPayPeriodResult = true;
                else
                    lblnPayPeriodResult = false;
                //Checking whether Pay Check date comes in the given Pay Check date range                           
                if (lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.paid_date >= idtPayCheckDateFrom &&
                    lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.paid_date <= ((idtPayCheckDateTo == DateTime.MinValue) ? DateTime.MaxValue : idtPayCheckDateTo))
                    lblnPayCheckDateResult = true;
                else
                    lblnPayCheckDateResult = false;
                //Checking whether transaction date comes in the given Transaction date range                           
                if (lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.transaction_date >= idtTransactionDateFrom &&
                    lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.transaction_date <= ((idtTransactionDateTo == DateTime.MinValue) ? DateTime.MaxValue : idtTransactionDateTo))
                    lblnTransactionDateResult = true;
                else
                    lblnTransactionDateResult = false;
                //Checking whether effective date comes in the given Effective date range
                if (lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.effective_date >= idtEffectiveDateFrom &&
                    lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.effective_date <= ((idtEffectiveDateTo == DateTime.MinValue) ? DateTime.MaxValue : idtEffectiveDateTo))
                    lblnEffectiveDateResult = true;
                else
                    lblnEffectiveDateResult = false;
                //Checking whether any transaction type is selected and if selected filtering based on that
                if (istrTransactionType != null)
                {
                    if (istrTransactionType == lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.transaction_type_value)
                        lblnTransactionResult = true;
                    else
                        lblnTransactionResult = false;
                }
                //Checking whether any source is selected and if selected, filtering based on that
                if (istrSource != null)
                {
                    if (istrSource == lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.subsystem_value)
                        lblnSourceResult = true;
                    else
                        lblnSourceResult = false;
                }
                //If all filtering conditions are True, adding the row to filtered collection
                if (lblnEmpOrgResult && lblnPrvOrgResult && lblnPayPeriodResult && lblnPayCheckDateResult && lblnTransactionDateResult && 
                    lblnEffectiveDateResult && lblnTransactionResult && lblnSourceResult)
                {                    
                    //No group by
                    if (string.IsNullOrEmpty(istrGroupBy) || istrGroupBy == "1")
                    {
                        ibusPADeferredComp.iclbDeferredCompContribution.Add(lobjDeferredCompContribution);
                    }
                    //Group by Month / Year
                    else if (istrGroupBy == "2")
                    {
                        if (lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodMonth ==
                            lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodMonth &&
                            lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodYear ==
                            lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodYear)
                        {
                            AddAllAmounts(lobjPreviousDeferredCompContribution, lobjDeferredCompContribution);
                        }
                        else
                        {
                            if (lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.person_account_id != 0)
                                ibusPADeferredComp.iclbDeferredCompContribution.Add(lobjPreviousDeferredCompContribution);
                            lobjPreviousDeferredCompContribution = new busPersonAccountDeferredCompContribution();
                            lobjPreviousDeferredCompContribution = AssignToPreviousContribution(lobjDeferredCompContribution);
                        }
                    }
                    //Group by FY
                    else if (istrGroupBy == "3")
                    {
                        //if pay period year is  same, then pay period months should be either less than or equal to 3 or greater than or equal to 4 and less than or equal to 12
                        // or if pay period year difference is 1, then current pay period month should be between 4 and 12 and previous pay period month
                        // should be less than or equal to 3 (sorted in descending order)
                        if (((lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodYear ==
                            lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodYear) &&
                            (((lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodMonth <= 6)
                            && (lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodMonth <= 6)) ||
                            ((lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodMonth >= 7
                            && lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodMonth <= 12)
                            && (lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodMonth >= 7
                            && lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodMonth <= 12)))) ||
                            ((Math.Abs(lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodYear -
                            lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodYear) == 1) &&
                            ((lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodMonth >= 7 &&
                            lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodMonth <= 12) &&
                            (lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodMonth <= 6))))
                        {
                            AddAllAmounts(lobjPreviousDeferredCompContribution, lobjDeferredCompContribution);
                        }
                        else
                        {
                            if (lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.person_account_id != 0)
                                ibusPADeferredComp.iclbDeferredCompContribution.Add(lobjPreviousDeferredCompContribution);
                            lobjPreviousDeferredCompContribution = new busPersonAccountDeferredCompContribution();
                            lobjPreviousDeferredCompContribution = AssignToPreviousContribution(lobjDeferredCompContribution);
                        }
                    }
                    //Group by CY
                    else if (istrGroupBy == "4")
                    {
                        if (lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodYear ==
                            lobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.PayPeriodYear)
                        {
                            AddAllAmounts(lobjPreviousDeferredCompContribution, lobjDeferredCompContribution);
                        }
                        else
                        {
                            if (lobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.person_account_id != 0)
                                ibusPADeferredComp.iclbDeferredCompContribution.Add(lobjPreviousDeferredCompContribution);
                            lobjPreviousDeferredCompContribution = new busPersonAccountDeferredCompContribution();
                            lobjPreviousDeferredCompContribution = AssignToPreviousContribution(lobjDeferredCompContribution);
                        }
                    }
                }
            }

            if (istrGroupBy == "2" || istrGroupBy == "3" || istrGroupBy == "4")
                ibusPADeferredComp.iclbDeferredCompContribution.Add(lobjPreviousDeferredCompContribution);
            //Adding the filterd collection to array list and returning so as to bind to datagrid
            larrList.Add(this);
            return larrList;
        }

        //Method to make all the fields other than group by columns to null
        private busPersonAccountDeferredCompContribution AssignToPreviousContribution(busPersonAccountDeferredCompContribution aobjDeferredCompContribution)
        {
            aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.istrPayPeriodStartDateFormatted =
                aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_start_date.Month.ToString() + "/" +
                aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_start_date.Year.ToString();
            aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.istrPayPeriodEndDateFormatted = string.Empty;
            aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.paid_date = DateTime.MinValue;
            aobjDeferredCompContribution.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code = string.Empty;
            aobjDeferredCompContribution.ibusProvider.icdoOrganization.org_name = string.Empty;
            aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.effective_date = DateTime.MinValue;
            aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.transaction_date = DateTime.MinValue;
            aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.subsystem_description = string.Empty;
            aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.Payroll_Detail_ID = string.Empty;
            aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.created_by = string.Empty;
            aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.modified_by = string.Empty;
            aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.deferred_comp_contribution_id = 0;
            aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.subsystem_ref_id = 0;
            aobjDeferredCompContribution.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_id = 0;
            return aobjDeferredCompContribution;
        }

        private void AddAllAmounts(busPersonAccountDeferredCompContribution aobjPreviousDeferredCompContribution, busPersonAccountDeferredCompContribution aobjDeferredCompContribution)
        {
            aobjPreviousDeferredCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_contribution_amount +=
                aobjDeferredCompContribution.icdoPersonAccountDeferredCompContribution.pay_period_contribution_amount;
        }        
	}
}
