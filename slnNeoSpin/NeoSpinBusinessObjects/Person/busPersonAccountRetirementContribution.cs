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
    public class busPersonAccountRetirementContribution : busPersonAccountRetirementContributionGen
    {
        // this property used id manual adjustmens report
        private busPersonAccountRetirementDbDbTransfer _ibusPersonAccountDBDBTransfer;
        public busPersonAccountRetirementDbDbTransfer ibusPersonAccountDBDBTransfer
        {
            get { return _ibusPersonAccountDBDBTransfer; }
            set { _ibusPersonAccountDBDBTransfer = value; }
        }
        //Property to contain Organization Code
        private string _istrOrgCodeID;
        public string istrOrgCodeID
        {
            get { return _istrOrgCodeID; }
            set { _istrOrgCodeID = value; }
        }

        //Property to contain To Year part of Pay Period Range
        private int _iintPayPeriodFromYear;
        public int iintPayPeriodFromYear
        {
            get { return _iintPayPeriodFromYear; }
            set { _iintPayPeriodFromYear = value; }
        }

        //Property to contain From Year part of Pay Period Range
        private int _iintPayPeriodToYear;
        public int iintPayPeriodToYear
        {
            get { return _iintPayPeriodToYear; }
            set { _iintPayPeriodToYear = value; }
        }

        //Property to contain From Month part or Pay Period Range
        private int _iintPayPeriodFromMonth;
        public int iintPayPeriodFromMonth
        {
            get { return _iintPayPeriodFromMonth; }
            set { _iintPayPeriodFromMonth = value; }
        }

        //Property to contain To Month part or Pay Period Range
        private int _iintPayPeriodToMonth;
        public int iintPayPeriodToMonth
        {
            get { return _iintPayPeriodToMonth; }
            set { _iintPayPeriodToMonth = value; }
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
		//PIR 8912
        public bool iblnIsDBPlan
        {
            get
            {
                if (ibusPARetirement.icdoPersonAccount == null)
                    ibusPARetirement.LoadPersonAccount();
                if (ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdMain
                      || ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdMain2020 //PIR 20232
                     || ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdLE
                     || ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdNG
                     || ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdHP
                     || ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdJudges
                     || ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdJobService
                     || ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdLEWithoutPS
                     || ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdBCILawEnf
                     || ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdStatePublicSafety) //PIR 25729
                    return true;
                else return false;
            }
        }
        public bool iblnIsDCPlan
        {
            get
            {
                if (ibusPARetirement.icdoPersonAccount == null)
                    ibusPARetirement.LoadPersonAccount();
                if (ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdDC ||
                    ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdDC2020 || //PIR 20232
                    ibusPARetirement.icdoPersonAccount.plan_id == busConstant.PlanIdDC2025) //PIR 25920
                    return true;
                else return false;                
            }
        }
        public string istrGender
        {
            get
            {
                if(ibusPARetirement.ibusPerson.IsNull())
                    ibusPARetirement.LoadPerson();
                return ibusPARetirement.ibusPerson.icdoPerson.gender_description;                 
            }
        }
        public string istrMaritalStatus
        {
            get
            {
                if (ibusPARetirement.ibusPerson.IsNull())
                    ibusPARetirement.LoadPerson();
                return ibusPARetirement.ibusPerson.icdoPerson.marital_status_description;
            }
        }
        public string istrSpouseName
        {
            get
            {
                if (ibusPARetirement.ibusPerson.IsNull())
                    ibusPARetirement.LoadPerson();
                if (ibusPARetirement.ibusPerson.ibusSpouse.IsNull())
                    ibusPARetirement.ibusPerson.LoadSpouse();
                return ibusPARetirement.ibusPerson.ibusSpouse.icdoPerson.FullName;
            }
        }
        public DateTime idtSpouseDOB
        {
            get
            {
                if (ibusPARetirement.ibusPerson.IsNull())
                    ibusPARetirement.LoadPerson();
                if (ibusPARetirement.ibusPerson.ibusSpouse.IsNull())
                    ibusPARetirement.ibusPerson.LoadSpouse();
                return ibusPARetirement.ibusPerson.ibusSpouse.icdoPerson.date_of_birth;
            }
        }
        public string istrSpouseSSN
        {
            get
            {
                if (ibusPARetirement.ibusPerson.IsNull())
                    ibusPARetirement.LoadPerson();
                if (ibusPARetirement.ibusPerson.ibusSpouse.IsNull())
                    ibusPARetirement.ibusPerson.LoadSpouse();
                return ibusPARetirement.ibusPerson.ibusSpouse.icdoPerson.ssn;
            }
        }
        public void GetMonthName()
        {
            if (icdoPersonAccountRetirementContribution.pay_period_month != 0)
            {
                DateTime ldtPayPeriod = new DateTime(1, Convert.ToInt32(icdoPersonAccountRetirementContribution.pay_period_month), 1);
                istrMonthName = ldtPayPeriod.ToString("MMMM");
            }
        }
        public DataSet rptVestedEmployerContributionErrorReport(DateTime adtPayPeriodDate)
        {
            DataTable ldtbVestedEmployer = busBase.Select("cdoPersonAccountRetirementContribution.rptVestedEmployerContributionErrorReport", new object[1] { adtPayPeriodDate });
            ldtbVestedEmployer.TableName = busConstant.ReportTableName;
            DataSet ldsReportResult = new DataSet();
            ldsReportResult.Tables.Add(ldtbVestedEmployer.Copy());
            return ldsReportResult;
        }

        public ArrayList ResetSearchFilter()
        {
            ArrayList larrList = new ArrayList();
            istrOrgCodeID = null;
            iintPayPeriodFromMonth = 0;
            iintPayPeriodFromYear = 0;
            iintPayPeriodToMonth = 0;
            iintPayPeriodToYear = 0;
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
        public bool iblnIsFYTDFlag { get; set; }
        //Method to filter Retirement Contribution and bind to data grid
        public ArrayList FilterRetirementContribution()
        {
            if (ibusPARetirement == null)
                LoadPersonAccountRetirement();
            if (iblnIsFYTDFlag)
                ibusPARetirement.LoadFYTDDetail();
            else
                ibusPARetirement.LoadLTDDetail();
            ArrayList larrList = new ArrayList();
            bool lblnOrgResult, lblnPayPeriodResult, lblnTransactionDateResult, lblnEffectiveDateResult, lblnTransactionResult, lblnSourceResult;
            DateTime ldtPayPeriodDate, ldtPayPeriodFromDate, ldtPayPeriodToDate;
            ibusPARetirement.iclbRetirementContribution = new Collection<busPersonAccountRetirementContribution>();

            busPersonAccountRetirementContribution lobjPreviousRetirementContribution = new busPersonAccountRetirementContribution();
            lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution();

            //Sorting the original collection in desc
            ibusPARetirement.iclbOriginalRetirementContribution = busGlobalFunctions.Sort<busPersonAccountRetirementContribution>(
                                                "icdoPersonAccountRetirementContribution.PayPeriodYearMonth desc",
                                                ibusPARetirement.iclbOriginalRetirementContribution);
            //Looping through the original collection for retirement contribution
            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in ibusPARetirement.iclbOriginalRetirementContribution)
            {
                lblnOrgResult = true;
                lblnPayPeriodResult = true;
                lblnTransactionDateResult = true;
                lblnEffectiveDateResult = true;
                lblnTransactionResult = true;
                lblnSourceResult = true;
                //Checking whether org code id is entered and if entered filtering based on that
                if (istrOrgCodeID != null)
                {
                    if (istrOrgCodeID == lobjRetirementContribution.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code)
                        lblnOrgResult = true;
                    else
                        lblnOrgResult = false;
                }

                if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year == 0 ||
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month == 0)
                    ldtPayPeriodDate = DateTime.MinValue;
                else
                    ldtPayPeriodDate = new DateTime(lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                        lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month, 1);

                // UAT PIR ID 495
                utlError lobjError = null;
                if ((iintPayPeriodFromMonth != 0) && (iintPayPeriodFromYear != 0))
                {
                    string lstrFromDate = Convert.ToString(iintPayPeriodFromMonth) + "-01-" + Convert.ToString(iintPayPeriodFromYear);
                    if (!(DateTime.TryParse(lstrFromDate, out ldtPayPeriodFromDate)))
                    {
                        lobjError = AddError(1167, string.Empty);
                        larrList.Add(lobjError);
                        return larrList;
                    }
                    ldtPayPeriodFromDate = new DateTime(iintPayPeriodFromYear, iintPayPeriodFromMonth, 1);
                }
                else
                    ldtPayPeriodFromDate = DateTime.MinValue;
                
                if ((iintPayPeriodToMonth != 0) && (iintPayPeriodToYear != 0))
                {
                    string lstrToDate = Convert.ToString(iintPayPeriodToMonth) + "-01-" + Convert.ToString(iintPayPeriodToYear);
                    if (!(DateTime.TryParse(lstrToDate, out ldtPayPeriodToDate)))
                    {
                        lobjError = AddError(1168, string.Empty);
                        larrList.Add(lobjError);
                        return larrList;
                    }
                    ldtPayPeriodToDate = new DateTime(iintPayPeriodToYear, iintPayPeriodToMonth, 1);
                }
                else
                    ldtPayPeriodToDate = DateTime.MaxValue;

                if (ldtPayPeriodFromDate > ldtPayPeriodToDate)
                {
                    lobjError = AddError(1169, string.Empty);
                    larrList.Add(lobjError);
                    return larrList;
                }

                //Checking whether Pay period comes in between the given Pay Period Range
                if (ldtPayPeriodDate >= ldtPayPeriodFromDate && ldtPayPeriodDate <= ldtPayPeriodToDate)
                    lblnPayPeriodResult = true;
                else
                    lblnPayPeriodResult = false;
                //Checking whether transaction date comes in the given Transaction date range                           
                if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date >= idtTransactionDateFrom &&
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date <= ((idtTransactionDateTo == DateTime.MinValue) ? DateTime.MaxValue : idtTransactionDateTo))
                    lblnTransactionDateResult = true;
                else
                    lblnTransactionDateResult = false;
                //Checking whether effective date comes in the given Effective date range
                if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date >= idtEffectiveDateFrom &&
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date <= ((idtEffectiveDateTo == DateTime.MinValue) ? DateTime.MaxValue : idtEffectiveDateTo))
                    lblnEffectiveDateResult = true;
                else
                    lblnEffectiveDateResult = false;
                //Checking whether any transaction type is selected and if selected filtering based on that
                if (istrTransactionType != null)
                {
                    if (istrTransactionType == lobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value)
                        lblnTransactionResult = true;
                    else
                        lblnTransactionResult = false;
                }
                //Checking whether any source is selected and if selected, filtering based on that
                if (istrSource != null)
                {
                    if (istrSource == lobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value)
                        lblnSourceResult = true;
                    else
                        lblnSourceResult = false;
                }
                //If all filtering conditions are True, adding the row to filtered collection after applying group by condition
                if (lblnOrgResult && lblnPayPeriodResult && lblnTransactionDateResult && lblnEffectiveDateResult && lblnTransactionResult && lblnSourceResult)
                {
                    //No group by
                    if (string.IsNullOrEmpty(istrGroupBy) || istrGroupBy == "1")
                    {
                        ibusPARetirement.iclbRetirementContribution.Add(lobjRetirementContribution);
                    }
                    //Group by Month / Year
                    else if (istrGroupBy == "2")
                    {
                        if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month ==
                            lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month &&
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year ==
                            lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year)
                        {
                            AddAllAmounts(lobjPreviousRetirementContribution, lobjRetirementContribution);
                        }
                        else
                        {
                            if (lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id != 0)
                                ibusPARetirement.iclbRetirementContribution.Add(lobjPreviousRetirementContribution);
                            lobjPreviousRetirementContribution = new busPersonAccountRetirementContribution();
                            lobjPreviousRetirementContribution = AssignToPreviousContribution(lobjRetirementContribution);
                        }
                    }
                    //Group by FY
                    else if (istrGroupBy == "3")
                    {
                        //if pay period year is  same, then pay period months should be either less than or equal to 3 or greater than or equal to 4 and less than or equal to 12
                        // or if pay period year difference is 1, then current pay period month should be between 4 and 12 and previous pay period month
                        // should be less than or equal to 3 (sorted in descending order)
                        if (((lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year ==
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year) &&
                            (((lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month <= 6)
                            && (lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month <= 6)) ||
                            ((lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month >= 7
                            && lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month <= 12)
                            && (lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month >= 7
                            && lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month <= 12)))) ||
                            ((Math.Abs(lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year -
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year) == 1) &&
                            ((lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month >= 7 &&
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month <= 12) &&
                            (lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month <= 6))))
                        {
                            AddAllAmounts(lobjPreviousRetirementContribution, lobjRetirementContribution);
                        }
                        else
                        {
                            if (lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id != 0)
                                ibusPARetirement.iclbRetirementContribution.Add(lobjPreviousRetirementContribution);
                            lobjPreviousRetirementContribution = new busPersonAccountRetirementContribution();
                            lobjPreviousRetirementContribution = AssignToPreviousContribution(lobjRetirementContribution);
                        }
                    }
                    //Group by CY
                    else if (istrGroupBy == "4")
                    {
                        if (lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year ==
                            lobjRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year)
                        {
                            AddAllAmounts(lobjPreviousRetirementContribution, lobjRetirementContribution);
                        }
                        else
                        {
                            if (lobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id != 0)
                                ibusPARetirement.iclbRetirementContribution.Add(lobjPreviousRetirementContribution);
                            lobjPreviousRetirementContribution = new busPersonAccountRetirementContribution();
                            lobjPreviousRetirementContribution = AssignToPreviousContribution(lobjRetirementContribution);
                        }
                    }
                }
            }

            if (istrGroupBy == "2" || istrGroupBy == "3" || istrGroupBy == "4")
                ibusPARetirement.iclbRetirementContribution.Add(lobjPreviousRetirementContribution);

            //Adding the filterd collection to array list and returning so as to bind to datagrid
            larrList.Add(this);
            return larrList;
        }

        //Method to make all the fields other than group by columns to null
        private busPersonAccountRetirementContribution AssignToPreviousContribution(busPersonAccountRetirementContribution aobjRetirementContribution)
        {
            aobjRetirementContribution.ibusPersonEmploymentDetail.ibusPersonEmployment.ibusOrganization.icdoOrganization.org_code = string.Empty;
            aobjRetirementContribution.icdoPersonAccountRetirementContribution.effective_date = DateTime.MinValue;
            aobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_date = DateTime.MinValue;
            aobjRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_description = string.Empty;
            aobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_description = string.Empty;
            aobjRetirementContribution.icdoPersonAccountRetirementContribution.created_by = string.Empty;
            aobjRetirementContribution.icdoPersonAccountRetirementContribution.modified_by = string.Empty;
            aobjRetirementContribution.icdoPersonAccountRetirementContribution.retirement_contribution_id = 0;
            aobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id = 0;
            aobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value = string.Empty;
            return aobjRetirementContribution;
        }

        public void AddAllAmounts(busPersonAccountRetirementContribution aobjPreviousRetirementContribution, busPersonAccountRetirementContribution aobjRetirementContribution)
        {
            aobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount +=
                                aobjRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount;
            aobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_ee_amount +=
                aobjRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_ee_amount;
            aobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_ee_amount +=
                aobjRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_ee_amount;
            aobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.er_vested_amount +=
                aobjRetirementContribution.icdoPersonAccountRetirementContribution.er_vested_amount;
            aobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.ee_er_pickup_amount +=
                aobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_er_pickup_amount;
            aobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.interest_amount +=
                aobjRetirementContribution.icdoPersonAccountRetirementContribution.interest_amount;
            aobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.ee_rhic_amount +=
                aobjRetirementContribution.icdoPersonAccountRetirementContribution.ee_rhic_amount;
            aobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.er_rhic_amount +=
                aobjRetirementContribution.icdoPersonAccountRetirementContribution.er_rhic_amount;
            aobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_er_amount +=
                aobjRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_er_amount;
            aobjPreviousRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount +=
                aobjRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount;

        }

        //correspondence - ENR-5250
        public override busBase GetCorPerson()
        {
            if (ibusPARetirement.IsNull())
                LoadPersonAccountRetirement();
            if (ibusPARetirement.ibusPerson.IsNull())
                ibusPARetirement.LoadPerson();
            return ibusPARetirement.ibusPerson;
        }


        public override void LoadCorresProperties(string astrTemplateName)
        {
		     //PIR 16890
            if (astrTemplateName == "PER-0500")
            {
                if (ibusPARetirement.ibusPerson.IsNull()) ibusPARetirement.LoadPerson();
                if (ibusPARetirement.ibusPerson.icolPersonEmployment.IsNull()) ibusPARetirement.ibusPerson.LoadPersonEmployment(true);                
                if (iclbMonthlySalaryDetails.IsNull()) LoadMonthlySalaryDetails();
            }
            else
            {
                ibusPARetirement.GetLastContributionDate();
                ibusPARetirement.SetVestingProperty();
                ibusPARetirement.LoadLTDSummary();
                ibusPARetirement.LoadTotalVSC();
            }
        }

        //PIR PIR 16890 Fixed for Correspondence btn issue
        public override long iintPrimaryKey
        {
            get
            {
                if (this.iobjPassInfo.istrFormName == "wfmPensionContributionDetailMaintenance")
                {
                    return icdoPersonAccountRetirementContribution.iintPrimaryId;
                }
                else
                {
                    return base.iintPrimaryKey;
                }
            }
        }
        public Collection<busPersonAccountRetirementContribution> iclbMonthlySalaryDetails { get; set; }
        public void LoadMonthlySalaryDetails()
        {
            iclbMonthlySalaryDetails = new Collection<busPersonAccountRetirementContribution>();
            DataTable ldtMonthlySalaryDetails = Select("entPersonAccountRetirementContribution.LoadMonthlySalaryDetails", new object[1] { icdoPersonAccountRetirementContribution.person_account_id });
            ibusPARetirement.LoadTotalPSC();
            if (ldtMonthlySalaryDetails?.Rows?.Count > 0)
            {
                foreach (DataRow dr in ldtMonthlySalaryDetails.Rows)
                {
                    busPersonAccountRetirementContribution lbusPersonAccountRetirementContribution = new busPersonAccountRetirementContribution { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
                    lbusPersonAccountRetirementContribution.icdoPersonAccountRetirementContribution.LoadData(dr);
                    lbusPersonAccountRetirementContribution.GetMonthName();
                    iclbMonthlySalaryDetails.Add(lbusPersonAccountRetirementContribution);
                }
            }
        }
    }
}