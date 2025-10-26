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
using System.Collections.Generic;

#endregion

namespace NeoSpin.BusinessObjects
{
    [Serializable]
    public class busPersonAccountRetirementDbDbTransfer : busPersonAccountRetirementDbDbTransferGen
    {
        public bool iblnIsTFFR { get; set; }
        private bool iblnIsRecordTransferred = false;

        public busPersonAccountRetirement ibusFromPersonAccountRetirement { get; set; }
        public busPersonAccountRetirement ibusToPersonAccountRetirement { get; set; }
     
        // Retirement adjustment detail
        public Collection<busPersonAccountRetirementDbDbTransferContribution> iclbRetirementDbDbTransferContribution { get; set; }

        public void LoadFromPersonAccountRetirement()
        {
            if (ibusFromPersonAccountRetirement == null)
            {
                ibusFromPersonAccountRetirement = new busPersonAccountRetirement();
            }
            ibusFromPersonAccountRetirement.FindPersonAccountRetirement(icdoPersonAccountRetirementDbDbTransfer.from_person_account_id);
        }

        public void LoadToPersonAccountRetirement()
        {
            if (ibusToPersonAccountRetirement == null)
            {
                ibusToPersonAccountRetirement = new busPersonAccountRetirement();
            }
            ibusToPersonAccountRetirement.FindPersonAccountRetirement(icdoPersonAccountRetirementDbDbTransfer.to_person_account_id);
        }

        //load transferred records
        //Load destination grid
        public void LoadRetirementDbDbTransferContribution()
        {
            if (ibusFromPersonAccountRetirement == null)
                LoadFromPersonAccountRetirement();

            if (iclbRetirementDbDbTransferContribution == null)
                iclbRetirementDbDbTransferContribution = new Collection<busPersonAccountRetirementDbDbTransferContribution>();

            DataTable ldtbList = Select<cdoPersonAccountRetirementDbDbTransferContribution>(
                new string[1] { "db_db_transfer_id" }, new object[1] { icdoPersonAccountRetirementDbDbTransfer.db_db_transfer_id }, null, null);

            iclbRetirementDbDbTransferContribution = GetCollection<busPersonAccountRetirementDbDbTransferContribution>(ldtbList,
                "icdoRetirementDbDbTransferContribution");

            if (ibusFromPersonAccountRetirement.iclbRetirementContributionAll == null)
                ibusFromPersonAccountRetirement.LoadRetirementContributionAll();

            foreach (busPersonAccountRetirementDbDbTransferContribution lbusDbDbTransferContribution in iclbRetirementDbDbTransferContribution)
            {
                lbusDbDbTransferContribution.ibusRetirementContribution = ibusFromPersonAccountRetirement.iclbRetirementContributionAll.Where(lobjContribution => lobjContribution.icdoPersonAccountRetirementContribution.retirement_contribution_id
                    == lbusDbDbTransferContribution.icdoRetirementDbDbTransferContribution.retirement_contribution_id).FirstOrDefault();
                if (lbusDbDbTransferContribution.ibusRetirementContribution.IsNull()) //PIR 15545
                    lbusDbDbTransferContribution.ibusRetirementContribution = new busPersonAccountRetirementContribution() { icdoPersonAccountRetirementContribution = new cdoPersonAccountRetirementContribution() };
            }
        }

        //load only those contributions that are not of subsystem value Account transfer
        public void LoadPersonAccountContributionDetail()
        {
            if (ibusFromPersonAccountRetirement.iclbRetirementContribution == null)
                ibusFromPersonAccountRetirement.LoadRetirementContribution();
            //if (iclbRetirementSourceContribution.IsNull())
            iclbRetirementSourceContribution = new Collection<busPersonAccountRetirementContribution>();
            var lclbretSource = ibusFromPersonAccountRetirement.iclbRetirementContribution.Where(lobjRetCon => (!IsContributionExcluded(lobjRetCon)));

            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in lclbretSource)
            {
                iclbRetirementSourceContribution.Add(lobjRetirementContribution);
            }
        }

        //public 
        /// <summary>
        /// Sums the detail into summary fields for display
        /// </summary>
        public void SetSummaryDataForDisplay()
        {
            icdoPersonAccountRetirementDbDbTransfer.idecPostTaxERAmount = 0;
            icdoPersonAccountRetirementDbDbTransfer.idecPostTaxEEAmount = 0;
            icdoPersonAccountRetirementDbDbTransfer.idecPreTaxERAmount = 0;
            icdoPersonAccountRetirementDbDbTransfer.idecPreTaxEEAmount = 0;
            icdoPersonAccountRetirementDbDbTransfer.idecEERHICAmount = 0;
            icdoPersonAccountRetirementDbDbTransfer.idecERRHICAmount = 0;
            icdoPersonAccountRetirementDbDbTransfer.idecEEERPickupAmount = 0;
            icdoPersonAccountRetirementDbDbTransfer.idecERVestedAmount = 0;
            icdoPersonAccountRetirementDbDbTransfer.idecInterestAmount = 0;
            icdoPersonAccountRetirementDbDbTransfer.idecVestedServiceCredit = 0;
            icdoPersonAccountRetirementDbDbTransfer.idecPensionServiceCredit = 0;
            if (iclbRetirementDbDbTransferContribution == null)
                LoadRetirementDbDbTransferContribution();
            foreach (busPersonAccountRetirementDbDbTransferContribution lbusDbDbTransferContribution in iclbRetirementDbDbTransferContribution)
            {
                icdoPersonAccountRetirementDbDbTransfer.idecPostTaxERAmount += lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_er_amount;
                icdoPersonAccountRetirementDbDbTransfer.idecPostTaxEEAmount += lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_ee_amount;
                icdoPersonAccountRetirementDbDbTransfer.idecPreTaxERAmount += lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount;
                icdoPersonAccountRetirementDbDbTransfer.idecPreTaxEEAmount += lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_ee_amount;
                icdoPersonAccountRetirementDbDbTransfer.idecEERHICAmount += lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.ee_er_pickup_ser_pur_cont;
                icdoPersonAccountRetirementDbDbTransfer.idecERRHICAmount += lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.er_rhic_amount;
                icdoPersonAccountRetirementDbDbTransfer.idecEEERPickupAmount += lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.ee_er_pickup_amount;
                icdoPersonAccountRetirementDbDbTransfer.idecERVestedAmount += lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.er_vested_amount;
                icdoPersonAccountRetirementDbDbTransfer.idecInterestAmount += lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.interest_amount;
                icdoPersonAccountRetirementDbDbTransfer.idecVestedServiceCredit += lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit;
                icdoPersonAccountRetirementDbDbTransfer.idecPensionServiceCredit += lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit;
            }
        }

        # region Button Logic
        public ArrayList btnPost_Click()
        {
            ArrayList alReturn = new ArrayList();
            utlError lobjError = new utlError();
            LoadOtherDetails();
            // Update Status
            icdoPersonAccountRetirementDbDbTransfer.posted_by = iobjPassInfo.istrUserID;
            icdoPersonAccountRetirementDbDbTransfer.posted_date = DateTime.Now;
            icdoPersonAccountRetirementDbDbTransfer.status_value = busConstant.TransferStatusPosted;

            // Contributions
            if (iclbRetirementDbDbTransferContribution == null)
                LoadRetirementDbDbTransferContribution();
            foreach (busPersonAccountRetirementDbDbTransferContribution lbusDbDbTransferContribution in iclbRetirementDbDbTransferContribution)
            {
                //UCS-041 check if the effective date is overlapping with the person account retirement start date and end date
                //if not raise error and exit loop
                if (!(busGlobalFunctions.CheckDateOverlapping(lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.effective_date,
                    ibusToPersonAccountRetirement.icdoPersonAccount.start_date, ibusToPersonAccountRetirement.icdoPersonAccount.end_date)))
                {
                    lobjError = AddError(7654, "");
                    alReturn.Add(lobjError);
                    return alReturn;
                }
                else
                {
                    ibusFromPersonAccountRetirement.PostRetirementContribution(busConstant.SubSystemValueTransfer,
                        icdoPersonAccountRetirementDbDbTransfer.db_db_transfer_id,
                        icdoPersonAccountRetirementDbDbTransfer.posted_date,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.effective_date,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                        0, //?? Can we link it to old employment, if we create new employment record as part of this procedure then we need to link that here
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_er_amount,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_ee_amount,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_ee_amount,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.ee_rhic_amount,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.er_rhic_amount,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.ee_er_pickup_amount,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.er_vested_amount,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.interest_amount,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit,
                        -1M * lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit);
                    ibusToPersonAccountRetirement.PostRetirementContribution(busConstant.SubSystemValueTransfer,
                        icdoPersonAccountRetirementDbDbTransfer.db_db_transfer_id,
                        icdoPersonAccountRetirementDbDbTransfer.posted_date,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.effective_date,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_month,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pay_period_year,
                        0, //?? Can we link it to old employment, if we create new employment record as part of this procedure then we need to link that here
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.transaction_type_value,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.salary_amount,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_er_amount,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.post_tax_ee_amount,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_er_amount,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pre_tax_ee_amount,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.ee_rhic_amount,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.er_rhic_amount,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.ee_er_pickup_amount,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.er_vested_amount,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.interest_amount,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.vested_service_credit,
                        lbusDbDbTransferContribution.ibusRetirementContribution.icdoPersonAccountRetirementContribution.pension_service_credit);

                }
            }
            if (iclbRetirementDbDbTransferContribution.Count > 0)
            {
                // Accounts
                if (icdoPersonAccountRetirementDbDbTransfer.capital_gain > 0)
                {
                    ibusFromPersonAccountRetirement.icdoPersonAccountRetirement.capital_gain -= icdoPersonAccountRetirementDbDbTransfer.capital_gain;
                    ibusFromPersonAccountRetirement.icdoPersonAccountRetirement.Update();

                    ibusToPersonAccountRetirement.icdoPersonAccountRetirement.capital_gain += icdoPersonAccountRetirementDbDbTransfer.capital_gain;
                    ibusToPersonAccountRetirement.icdoPersonAccountRetirement.Update();
                }

                icdoPersonAccountRetirementDbDbTransfer.iblnUpdateModifiedBy = false;
                icdoPersonAccountRetirementDbDbTransfer.Update();
                icdoPersonAccountRetirementDbDbTransfer.iblnUpdateModifiedBy = true;

                icdoPersonAccountRetirementDbDbTransfer.status_description = iobjPassInfo.isrvDBCache.GetCodeDescriptionString(353, icdoPersonAccountRetirementDbDbTransfer.status_value);

                EvaluateInitialLoadRules();
            }
            alReturn.Add(this);
            return alReturn;
        }

        public void LoadOtherDetails()
        {
            if (icdoPersonAccountRetirementDbDbTransfer.from_person_account_id > 0)
            {
                if (ibusFromPersonAccountRetirement.IsNull())
                    LoadFromPersonAccountRetirement();
                if (ibusFromPersonAccountRetirement.ibusPerson.IsNull())
                    ibusFromPersonAccountRetirement.LoadPerson();
                if (ibusFromPersonAccountRetirement.ibusPlan.IsNull())
                    ibusFromPersonAccountRetirement.LoadPlan();
            }
            if (ibusToPersonAccountRetirement.IsNull())
                LoadToPersonAccountRetirement();
            if (ibusToPersonAccountRetirement.ibusPlan.IsNull())
                ibusToPersonAccountRetirement.LoadPlan();
            if (ibusToPersonAccountRetirement.ibusPerson.IsNull())
                ibusToPersonAccountRetirement.LoadPerson();
        }

        # endregion

        //return collection of records that can be transferred
        public Collection<busPersonAccount> GetPersonAccountToTransfer()
        {
            if (ibusFromPersonAccountRetirement.IsNull())
                LoadFromPersonAccountRetirement();
            if (ibusFromPersonAccountRetirement.ibusPerson.IsNull())
                ibusFromPersonAccountRetirement.LoadPerson();
            Collection<busPersonAccount> lclbPersonAccount = new Collection<busPersonAccount>();
            if (ibusFromPersonAccountRetirement.ibusPerson.iclbRetirementAccount == null)
                ibusFromPersonAccountRetirement.ibusPerson.LoadRetirementAccount();

            foreach (busPersonAccount lbusPersonAccount in ibusFromPersonAccountRetirement.ibusPerson.iclbRetirementAccount)
            {
                if (lbusPersonAccount.icdoPersonAccount.person_account_id != ibusFromPersonAccountRetirement.icdoPersonAccount.person_account_id)
                    lclbPersonAccount.Add(lbusPersonAccount);
            }
            return lclbPersonAccount;
        }

        public void IsTFFRPlan()
        {
            if (ibusToPersonAccountRetirement.ibusPlan.icdoPlan.plan_id == busConstant.PlanIdTFFR)
            {
                iblnIsTFFR = CalculateStatusDateEndDateGreaterThanStartDate();
            }
            else
                iblnIsTFFR = CalculateStatusDateEndDateGreaterThanStartDate();
        }

        public bool CalculateStatusDateEndDateGreaterThanStartDate()
        {
            // if the Transferred TFFR participation status date and DB plan End Date are greater than 90 days from the Employment Start Date
            DateTime ldtHistoryChangeDate;
            DateTime ldtEndDate;
            DateTime ldtStartDate;

            if (ibusFromPersonAccountRetirement.ibusHistory == null)
                ibusFromPersonAccountRetirement.LoadPreviousHistory();

            if (ibusFromPersonAccountRetirement.ibusPersonEmploymentDetail == null)
            {
                ibusFromPersonAccountRetirement.icdoPersonAccount.person_employment_dtl_id = ibusFromPersonAccountRetirement.GetEmploymentDetailID();
                ibusFromPersonAccountRetirement.LoadPersonEmploymentDetail();
            }

            DataTable ldtbHistory = Select("cdoPersonAccountRetirement.LatestEndDateForEnrolled", new object[1] { ibusFromPersonAccountRetirement.icdoPersonAccount.person_account_id });
            if (ldtbHistory.Rows.Count > 0)
            {
                ibusFromPersonAccountRetirement.ibusHistory.icdoPersonAccountRetirementHistory.LoadData(ldtbHistory.Rows[0]);
            }

            if ((ibusFromPersonAccountRetirement.icdoPersonAccount.history_change_date != DateTime.MinValue) && (ibusFromPersonAccountRetirement.ibusHistory.icdoPersonAccountRetirementHistory.end_date != DateTime.MinValue)
                && (ibusFromPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date != DateTime.MinValue))
            {
                ldtHistoryChangeDate = ibusFromPersonAccountRetirement.icdoPersonAccount.history_change_date;
                ldtEndDate = ibusFromPersonAccountRetirement.ibusHistory.icdoPersonAccountRetirementHistory.end_date;
                ldtStartDate = ibusFromPersonAccountRetirement.ibusPersonEmploymentDetail.icdoPersonEmploymentDetail.start_date;

                ldtStartDate = ldtStartDate.AddDays(90);

                if ((ldtHistoryChangeDate > ldtStartDate) && (ldtEndDate > ldtStartDate))
                    return true;
                else
                    return false;
            }

            return false;
        }

        # region UCS 041

        //this is to check if the To Person ID entered is also enrolled in the Same Plan As from Person Account
        public bool IsTransferMemberEnrolledInFromMemberPlan()
        {
            if (icdoPersonAccountRetirementDbDbTransfer.iintTransferToMemberPersonID > 0)
            {
                if (icdoPersonAccountRetirementDbDbTransfer.to_person_account_id == 0)
                    return false;
            }
            return true;
        }

        //check if the contribution records already transferred 
        private bool IsRecordAlreadyTransferred(int aintContributionID)
        {
            DataTable ldtbList = Select("cdoPersonAccountRetirementDbDbTransfer.GetDBDBTransferredRecords",
                new object[2] { aintContributionID, icdoPersonAccountRetirementDbDbTransfer.from_person_account_id });
            if (ldtbList.Rows.Count > 0)
            {
                return true;
            }           
            return false;
        }

        public busPerson ibusTransferToPerson { get; set; }
        public void LoadTransferToPerson()
        {
            if (ibusTransferToPerson == null)
                ibusTransferToPerson = new busPerson();

            ibusTransferToPerson.FindPerson(icdoPersonAccountRetirementDbDbTransfer.iintTransferToMemberPersonID);
        }

        //if the To Transfer Person is entered then get the person account for the plan of from Person account id.
        public void GetToPersonAccountID()
        {
            if (icdoPersonAccountRetirementDbDbTransfer.transfer_type_value == busConstant.TransferTypeMember)
            {
                if (icdoPersonAccountRetirementDbDbTransfer.iintTransferToMemberPersonID != 0)
                {
                    if (ibusTransferToPerson == null)
                        LoadTransferToPerson();
                    busPersonAccount lobjPersonAccount = ibusTransferToPerson.LoadActivePersonAccountByPlan(ibusFromPersonAccountRetirement.icdoPersonAccount.plan_id);
                    icdoPersonAccountRetirementDbDbTransfer.to_person_account_id = lobjPersonAccount.icdoPersonAccount.person_account_id;
                }
            }
            else if (icdoPersonAccountRetirementDbDbTransfer.iintTransferToPlanPersonAccountID > 0)
            {
                icdoPersonAccountRetirementDbDbTransfer.to_person_account_id = icdoPersonAccountRetirementDbDbTransfer.iintTransferToPlanPersonAccountID;
            }
        }

        private bool IsContributionExcluded(busPersonAccountRetirementContribution aobjRetirementContribution)
        {
            // 1. Check if the Contribution is already transferred
            // 2. Check if the Contribution is a Contra entry of the contribution
            if (IsRecordAlreadyTransferred(aobjRetirementContribution.icdoPersonAccountRetirementContribution.retirement_contribution_id))
            {
                return true;
            }
            else if ((aobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_value == busConstant.SubSystemValueTransfer) &&
                     (aobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id > 0))
            {
                if (IsContraEntry(aobjRetirementContribution))
                    return true;
            }
            return false;
        }


        private bool IsContraEntry(busPersonAccountRetirementContribution aobjRetirementContribution)
        {
            // Check if the contribution has a subsystem Ref Id and subsystem type as Transfer
            // Load the DBDBTransfer record and check if the FromPersonAccountId and the Contribution PersonAccount Id is the same,           
            // if so then this Record is a contra entry record.
            DataTable ldtblist = Select<cdoPersonAccountRetirementDbDbTransfer>(
                                                new string[2] { "db_db_transfer_id", "from_person_account_id" },
                                                new object[2] { aobjRetirementContribution.icdoPersonAccountRetirementContribution.subsystem_ref_id,
                                                    aobjRetirementContribution.icdoPersonAccountRetirementContribution.person_account_id }, null, null);

            if (ldtblist.Rows.Count > 0)

                return true;
            return false;
        }

        # endregion

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            GetToPersonAccountID();
            LoadOtherDetails();
        }
        public override int PersistChanges()
        {
            if (icdoPersonAccountRetirementDbDbTransfer.ienuObjectState.Equals(ObjectState.Insert))
                icdoPersonAccountRetirementDbDbTransfer.Insert();
            else if (icdoPersonAccountRetirementDbDbTransfer.ienuObjectState.Equals(ObjectState.Update))
                icdoPersonAccountRetirementDbDbTransfer.Update();

            foreach (busPersonAccountRetirementContribution lobjRetirementContribution in iclbRetirementSourceContribution)
            {
                if (lobjRetirementContribution.icdoPersonAccountRetirementContribution.isChecked == busConstant.Flag_Yes)
                {
                    busPersonAccountRetirementDbDbTransferContribution lobjTransferDestination = new busPersonAccountRetirementDbDbTransferContribution { icdoRetirementDbDbTransferContribution = new cdoPersonAccountRetirementDbDbTransferContribution() };
                    lobjTransferDestination.icdoRetirementDbDbTransferContribution.retirement_contribution_id = lobjRetirementContribution.icdoPersonAccountRetirementContribution.retirement_contribution_id;
                    lobjTransferDestination.icdoRetirementDbDbTransferContribution.db_db_transfer_id = icdoPersonAccountRetirementDbDbTransfer.db_db_transfer_id;
                    lobjTransferDestination.icdoRetirementDbDbTransferContribution.Insert();
                    lobjTransferDestination.ibusRetirementContribution = lobjRetirementContribution;
                    iclbRetirementDbDbTransferContribution.Add(lobjTransferDestination);
                }
            }
            return 1;
        }
        public override void AfterPersistChanges()
        {
            LoadRetirementDbDbTransferContribution();
            LoadPersonAccountContributionDetail();
        }

        public override int Delete()
        {
            if (ibusSoftErrors.IsNull())
                LoadErrors();         
            ibusSoftErrors.DeleteErrors();

            foreach (busPersonAccountRetirementDbDbTransferContribution  lobjContribution in iclbRetirementDbDbTransferContribution)
            {
                lobjContribution.Delete();
            }
            return base.Delete();
        }

        public bool IsLoggedinUserNotSameasCreatedUser()
        {
            // UAT PIR ID 1410
            // Framework Upgrade : Button "btnSelectDeselect" is not working in New Mode on wfmDBDBTransferMaintenance
            if (icdoPersonAccountRetirementDbDbTransfer.modified_by != null)
            {
                if (icdoPersonAccountRetirementDbDbTransfer.modified_by.ToLower() != iobjPassInfo.istrUserID.ToLower())
                    return true;
            }
                return false;      
        }

        //pir 8702
        public bool IsAllContributionsSelected { get; set; }
        public ArrayList btnSelectDeselect_Click()
        {
            ArrayList larrList = new ArrayList();
            if (!IsAllContributionsSelected)
            {
                foreach (busPersonAccountRetirementContribution lobjRetirementContribution in iclbRetirementSourceContribution)
                {
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.isChecked = busConstant.Flag_Yes;
                }
                IsAllContributionsSelected = true;
            }
            else
            {
                foreach (busPersonAccountRetirementContribution lobjRetirementContribution in iclbRetirementSourceContribution)
                {
                    lobjRetirementContribution.icdoPersonAccountRetirementContribution.isChecked = busConstant.Flag_No;
                }
                IsAllContributionsSelected = false;
            }
            larrList.Add(this);
            return larrList;
        }
        
    }
}
