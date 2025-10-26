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
using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion

namespace NeoSpin.BusinessObjects
{
	/// <summary>
	/// Class NeoSpin.BusinessObjects.busPersonAccountMedicarePartDHistory:
	/// Inherited from busPersonAccountMedicarePartDHistoryGen, the class is used to customize the business object busPersonAccountMedicarePartDHistoryGen.
	/// </summary>
	[Serializable]
	public class busPersonAccountMedicarePartDHistory : busPersonAccountMedicarePartDHistoryGen
	{

        public Collection<busPersonAccountMedicarePartDHistory> iclbPersonAccountMedicarePartDMembers { get; set; }
        public Collection<busPersonAccountMedicarePartDHistory> iclbPersonAccountMedicarePartDMembersIBS { get; set; }

        public Collection<busPersonAccountMedicarePartDHistory> iclbMedicarePartDTemp { get; set; }
        public Collection<busPersonAccountMedicarePartDHistory> iclbMedicarePartDTemp1 { get; set; }

        private Collection<busPersonContact> _iclbPersonContact;
        public Collection<busPersonContact> iclbPersonContact
        {
            get { return _iclbPersonContact; }
            set { _iclbPersonContact = value; }
        }

        private busPersonAccountMedicarePartDHistory _ibusHistory;
        public busPersonAccountMedicarePartDHistory ibusHistory
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

        private decimal _TotalMonthlyPremiumAmount;
        public decimal TotalMonthlyPremiumAmount
        {
            get { return _TotalMonthlyPremiumAmount; }
            set { _TotalMonthlyPremiumAmount = value; }
        }

        private decimal _TotalMonthlyPremiumAmountPAPIT;
        public decimal TotalMonthlyPremiumAmountPAPIT
        {
            get { return _TotalMonthlyPremiumAmountPAPIT; }
            set { _TotalMonthlyPremiumAmountPAPIT = value; }
        }

        private ObjectState lobjCurrentObjectState;
        public string istrAllowOverlapHistory { get; set; }
        public Collection<busPersonAccountMedicarePartDHistory> iclbOverlappingHistory { get; set; }
        public DataTable idtbCachedLowIncomeCredit { get; set; }
        public DataTable idtbCachedHealthRate { get; set; }

        private int _iintMemberID;
        public int iintMemberID
        {
            get { return _iintMemberID; }
            set { _iintMemberID = value; }
        }

        public int iintPersonMemberID { get; set; }
        public bool iblnIsFromIBSBilling { get; set; }
        public bool iblnIsFromEmployerReportPosting { get; set; }
        public int iintOldPayeeAccountID { get; set; }

        public void LoadMedicarePartDMembers()
        {
            //int lintContactPersonID = 0;

            if (iclbPersonAccountMedicarePartDMembers == null)
                iclbPersonAccountMedicarePartDMembers = new Collection<busPersonAccountMedicarePartDHistory>();
            if (ibusPerson == null)
                LoadPerson();

            //lintContactPersonID = LoadPersonContact();

            //Change below code for neogrid, Neogrid requires MainCDO to be initialized(PIR 2369)
            DataTable ldtbResult = Select("cdoPersonAccountMedicarePartDHistory.LoadMedicarePartDMembers",
                new object[1] { icdoPersonAccountMedicarePartDHistory.member_person_id });
            iclbPersonAccountMedicarePartDMembers = new Collection<busPersonAccountMedicarePartDHistory>(); 

            foreach (DataRow aTotalsRow in ldtbResult.Rows)
            {
                busPersonAccountMedicarePartDHistory lbusPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
                lbusPersonAccountMedicarePartDHistory.icdoPersonAccount = new cdoPersonAccount();
                lbusPersonAccountMedicarePartDHistory.icdoPersonAccount.person_account_id = icdoPersonAccount.person_account_id;
                lbusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.LoadData(aTotalsRow);
                iclbPersonAccountMedicarePartDMembers.Add(lbusPersonAccountMedicarePartDHistory);
            }
        }

        public int LoadPersonContact()
        {
            int lintPersonContact = 0;
            DataTable ldtPersonContact = Select<cdoPersonContact>(new string[1] { "PERSON_ID" }, new object[1] { ibusPerson.icdoPerson.person_id }, null, null);
            if (ldtPersonContact.Rows.Count > 0)
            {
                if (ldtPersonContact.Rows[0]["relationship_value"].Equals(busConstant.PersonTypeSpouse))
                {
                    if (ldtPersonContact.Rows.Count > 0 && ldtPersonContact.Rows[0]["contact_person_id"] != DBNull.Value)
                        lintPersonContact = Convert.ToInt32(ldtPersonContact.Rows[0]["contact_person_id"]);
                }
            }
            return lintPersonContact;
        }

        public void LoadMedicareByPersonIDForAddress(int AintPersonMemberID)
        {
            if (iclbPersonAccountMedicarePartDMembers == null)
                iclbPersonAccountMedicarePartDMembers = new Collection<busPersonAccountMedicarePartDHistory>();

            DataTable ldtbList = Select("cdoPersonAccountMedicarePartDHistory.LoadMedicareForFlagUpdateAddress", new object[1] { AintPersonMemberID });
            iclbPersonAccountMedicarePartDMembers = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbList, "icdoPersonAccountMedicarePartDHistory");

            //if (icdoPersonAccountMedicarePartDHistory == null)
            //    icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();

            //foreach (DataRow dr in ldtbList.Rows)
            //{
            //    busPersonAccountMedicarePartDHistory lobjMedicare = new busPersonAccountMedicarePartDHistory { icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory() };
            //    lobjMedicare.icdoPersonAccountMedicarePartDHistory.LoadData(dr);
            //}
        }

        public Collection<cdoLowIncomeCreditRef> iclbLowIncomeCreditRef { get; set; }
        public Collection<cdoLowIncomeCreditRef> LoadLowIncomeCreditRef()
        {
            if (idtbCachedLowIncomeCredit == null)
                idtbCachedLowIncomeCredit = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
            if (idtPlanEffectiveDate == DateTime.MinValue)
                LoadPlanEffectiveDate();
            DateTime ldtEffectiveDate = new DateTime();
            var lenumList = idtbCachedLowIncomeCredit.AsEnumerable().OrderByDescending(i => i.Field<DateTime>("effective_date"));
            foreach (DataRow dr in lenumList)
            {
                if (Convert.ToDateTime(dr["effective_date"]).Date <= idtPlanEffectiveDate.Date)
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

        public void LoadPlanEffectiveDate()
        {
            idtPlanEffectiveDate = DateTime.Now;

            //If the Current Participation status is enrolled, Set the Effective Date from History Change Date
            if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                if (icdoPersonAccount.current_plan_start_date_no_null > DateTime.Now)
                    idtPlanEffectiveDate = icdoPersonAccount.current_plan_start_date_no_null;
                else
                    idtPlanEffectiveDate = DateTime.Now;
            }
            else
            {
                if (iclbHistory.IsNull() || iclbHistory.Count > 0)
                    LoadHistory();
                
                foreach (busPersonAccountMedicarePartDHistory lbusMedicarePartDHistory in iclbHistory)
                {
                    if(lbusMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    {
                        if (lbusMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.end_date == DateTime.MinValue)
                        {
                            //If the Start Date is Future Date, Set it otherwise Current Date will be Start Date of Premium Calc
                            if (lbusMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.start_date > DateTime.Now)
                            {
                                idtPlanEffectiveDate = lbusMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.start_date;
                            }
                            else
                            {
                                idtPlanEffectiveDate = DateTime.Now;
                            }
                        }
                        else
                            idtPlanEffectiveDate = lbusMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.end_date;
                        break;
                    }
                }
            }
        }

        public override void BeforeValidate(utlPageMode aenmPageMode)
        {
            iclbOverlappingHistory = new Collection<busPersonAccountMedicarePartDHistory>();
            if ((istrAllowOverlapHistory == busConstant.Flag_Yes) && (icdoPersonAccount.history_change_date != DateTime.MinValue))
            {
                Collection<busPersonAccountMedicarePartDHistory> lclbOpenHistory = LoadOverlappingHistory();
                if (lclbOpenHistory.Count > 0)
                {
                    foreach (busPersonAccountMedicarePartDHistory lbusPAMedicarePartDHistory in lclbOpenHistory)
                    {
                        iclbPersonAccountMedicarePartDHistory.Remove(lbusPAMedicarePartDHistory);
                        iclbOverlappingHistory.Add(lbusPAMedicarePartDHistory);
                    }
                }
            }
            lobjCurrentObjectState = icdoPersonAccountMedicarePartDHistory.ienuObjectState;

            if (ibusPaymentElection == null)
                LoadPaymentElection();

            if (!String.IsNullOrEmpty(ibusPaymentElection.icdoPersonAccountPaymentElection.Billing_Organization))
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id =
                    busGlobalFunctions.GetOrgIdFromOrgCode(ibusPaymentElection.icdoPersonAccountPaymentElection.Billing_Organization);
            }
            else
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id = 0;
            }

            if (!String.IsNullOrEmpty(ibusPaymentElection.icdoPersonAccountPaymentElection.Supplemental_Billing_Organization))
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id =
                    busGlobalFunctions.GetOrgIdFromOrgCode(ibusPaymentElection.icdoPersonAccountPaymentElection.Supplemental_Billing_Organization);
            }
            else
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_supplemental_org_id = 0;
            }

            LoadPreviousHistory();

            if (icdoPersonAccountMedicarePartDHistory.suppress_warnings_flag == busConstant.Flag_Yes)
                icdoPersonAccountMedicarePartDHistory.suppress_warnings_by = iobjPassInfo.istrUserID;

            base.BeforeValidate(aenmPageMode);

        }
        string istrOldPaymentMethod = string.Empty;
        public override void BeforePersistChanges()
        {
            icolPosNegEmployerPayrollDtl = null;
            icolPosNegIbsDetail = null;
            //PIR 24309 - Medicare Provider change
            if (icdoPersonAccount.history_change_date != DateTime.MinValue)
                LoadActiveProviderOrgPlan(icdoPersonAccount.history_change_date);
            //Update the provider org id
            if (ibusProviderOrgPlan != null)
            {
                icdoPersonAccount.provider_org_id = ibusProviderOrgPlan.icdoOrgPlan.org_id;
            }

            if (icdoPersonAccountMedicarePartDHistory.ihstOldValues.Count > 0)
            {
                istrPreviousPlanParticipationStatus = Convert.ToString(icdoPersonAccount.ihstOldValues["plan_participation_status_value"]);
            }
            
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues.Count > 0)
            {
                iintOldPayeeAccountID = Convert.ToInt32(ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payee_account_id"]);
            }

            //TFFR Flag set to Y
            string lstrOldPaymentMethod = null;
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues.Count > 0)
            {
                iintOldPayeeAccountID = Convert.ToInt32(ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payee_account_id"]);

                lstrOldPaymentMethod = (string)ibusPaymentElection.icdoPersonAccountPaymentElection.ihstOldValues["payment_method_value"];
                if (icdoPersonAccount.status_value == busConstant.StatusReview)
                {
                    if (iclbPersonAccountPaymentElectionHistory.IsNull())
                        LoadPaymentElectionHistory();
                    if (iclbPersonAccountPaymentElectionHistory.Count > 1 && iclbPersonAccountPaymentElectionHistory.Any(i => i.icdoPersonAccountPaymentElectionHistory.history_change_date == icdoPersonAccount.history_change_date))
                        istrOldPaymentMethod = iclbPersonAccountPaymentElectionHistory.Skip(1).Take(1).FirstOrDefault().icdoPersonAccountPaymentElectionHistory.payment_method_value;
                    else
                        istrOldPaymentMethod = lstrOldPaymentMethod;
                }
                else
                    istrOldPaymentMethod = lstrOldPaymentMethod;
            }

            bool lblnResetAfterTffrFileSentFlag = false;
            if (((icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid)) ||
                (istrOldPaymentMethod != ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value))
            {
                lblnResetAfterTffrFileSentFlag = true;
            }

            if (lblnResetAfterTffrFileSentFlag)
            {
                // Update the TFFR Flag
                icdoPersonAccountMedicarePartDHistory.modified_after_tffr_file_sent_flag = busConstant.Flag_Yes;
            }
        }

        public override int PersistChanges()
        {
            LoadPersonAccount(icdoPersonAccount.person_id);

            //PIR 23944 - If initial_enroll_date is null then updating it to PA start date
            if (icdoPersonAccountMedicarePartDHistory.initial_enroll_date == DateTime.MinValue)
                icdoPersonAccountMedicarePartDHistory.initial_enroll_date = icdoPersonAccount.start_date;
            
            if (icdoPersonAccountMedicarePartDHistory.ienuObjectState == ObjectState.Insert )
            {
                icdoPersonAccount.person_id = ibusPerson.icdoPerson.person_id;
                icdoPersonAccount.plan_id = busConstant.PlanIdMedicarePartD;
                icdoPersonAccount.status_value = busConstant.StatusValid;
                icdoPersonAccount.history_change_date = icdoPersonAccount.start_date;

                LoadActiveProviderOrgPlan(icdoPersonAccount.history_change_date);
                icdoPersonAccount.provider_org_id = ibusProviderOrgPlan.icdoOrgPlan.org_id;


                icdoPersonAccount.Insert();
                icdoPersonAccountMedicarePartDHistory.person_account_id = icdoPersonAccount.person_account_id;
                icdoPersonAccountMedicarePartDHistory.start_date = icdoPersonAccount.start_date;
                icdoPersonAccountMedicarePartDHistory.person_id = icdoPersonAccount.person_id;
                icdoPersonAccountMedicarePartDHistory.plan_participation_status_value = icdoPersonAccount.plan_participation_status_value;
                icdoPersonAccountMedicarePartDHistory.status_value = busConstant.StatusValid;
                UpdateEnrollmentAndRecordTypeFlags();
                
                icdoPersonAccountMedicarePartDHistory.send_after = DateTime.Now.Date;
                //On initial save, initial_enroll_date should be Person Account start date.
                icdoPersonAccountMedicarePartDHistory.initial_enroll_date = icdoPersonAccount.start_date;
                icdoPersonAccountMedicarePartDHistory.provider_org_id = icdoPersonAccount.provider_org_id;
                icdoPersonAccountMedicarePartDHistory.Insert();
            }
            else
            {

                //End the previous history record
                if (_ibusHistory == null)
                    LoadPreviousHistory();

                if (ibusHistory.icdoPersonAccountMedicarePartDHistory.person_account_medicare_part_d_history_id > 0)
                {
                    if (ibusHistory.icdoPersonAccountMedicarePartDHistory.start_date == icdoPersonAccount.history_change_date)
                        ibusHistory.icdoPersonAccountMedicarePartDHistory.end_date = icdoPersonAccount.history_change_date;

                    else
                        ibusHistory.icdoPersonAccountMedicarePartDHistory.end_date = icdoPersonAccount.history_change_date.AddDays(-1);

                    ibusHistory.icdoPersonAccountMedicarePartDHistory.Update();
                }

                if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                    icdoPersonAccount.end_date = DateTime.MinValue;
                else
                    icdoPersonAccount.end_date = ibusHistory.icdoPersonAccountMedicarePartDHistory.end_date;

                icdoPersonAccount.Update();
                InsertHistory();
            }

            if (ibusPaymentElection.icdoPersonAccountPaymentElection.account_payment_election_id > 0)
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.Update();
            }
            else
            {
                ibusPaymentElection.icdoPersonAccountPaymentElection.ienuObjectState = ObjectState.Insert;
                ibusPaymentElection.icdoPersonAccountPaymentElection.person_account_id = icdoPersonAccount.person_account_id;
                ibusPaymentElection.icdoPersonAccountPaymentElection.Insert();

            }
			//PIR 12737 & 8565 -- added payment election history
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
            {
                busPersonAccountPaymentElectionHistory lbusPersonAccountPaymentElectionHistory = new busPersonAccountPaymentElectionHistory { icdoPersonAccountPaymentElectionHistory = new cdoPersonAccountPaymentElectionHistory() };
                lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection = ibusPaymentElection;
                lbusPersonAccountPaymentElectionHistory.ibusPersonAccountPaymentElection.icdoPersonAccount = icdoPersonAccount;
                lbusPersonAccountPaymentElectionHistory.InsertPaymentElectionHistory();
            }

            return 1;
        }

        public override void AfterPersistChanges()
        {
            //Remove the Overlapping History
            if (iclbOverlappingHistory != null && iclbOverlappingHistory.Count > 0)
            {
                foreach (busPersonAccountMedicarePartDHistory lbusPAMedicarePartDHistory in iclbOverlappingHistory)
                {
                    lbusPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.Delete();
                }
                //PIR 23340
                if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedGHDV))
                {
                    bool lblnIsPersonAccountModified = false;
                    if (icdoPersonAccount.start_date > icdoPersonAccount.history_change_date)
                    {
                        icdoPersonAccount.start_date = icdoPersonAccount.history_change_date;
                        lblnIsPersonAccountModified = true;

                    }
                    if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                        icdoPersonAccount.end_date != DateTime.MinValue)
                    {
                        icdoPersonAccount.end_date = DateTime.MinValue;
                        lblnIsPersonAccountModified = true;
                    }

                    if (lblnIsPersonAccountModified)
                        icdoPersonAccount.Update();
                }
            }
            LoadPersonAccountMedicarePartDHistory(icdoPersonAccountMedicarePartDHistory.person_id);
            base.AfterPersistChanges();
            GetMonthlyPremiumAmountForMedicarePartD();
            GetTotalPremiumAmountForMedicareForPapit();
            LoadMedicarePartDMembers();
            LoadPreviousHistory();
			//PIR 12737 & 8565 -- added payment election history
            LoadPaymentElectionHistory();

            //if ((lobjCurrentObjectState == ObjectState.Insert) && (icdoPersonAccount.person_employment_dtl_id > 0))
            //{
            //    SetPersonAccountIDInPersonAccountEmploymentDetail(); //Inserting enrty
            //}

            if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid)
            {
                if (ibusPaymentElection == null)
                    LoadPaymentElection();

                busPersonAccountMedicarePartDHistory lbusClosedMedicareHistory = null;
                busPersonAccountMedicarePartDHistory lbusAddedMedicareHistory = null;

                //Creating IBS Adjustment records.
                foreach (busPersonAccountMedicarePartDHistory lbusHistory in iclbPersonAccountMedicarePartDHistory)
                {
                    if (lbusHistory.icdoPersonAccountMedicarePartDHistory.end_date > DateTime.MinValue)
                    {
                        lbusClosedMedicareHistory = lbusHistory;
                        break;
                    }
                    else
                        lbusAddedMedicareHistory = lbusHistory;
                }

                if (lbusClosedMedicareHistory != null)
                {
                    if (lbusClosedMedicareHistory.icdoPersonAccountMedicarePartDHistory.start_date == lbusClosedMedicareHistory.icdoPersonAccountMedicarePartDHistory.end_date)
                    {
                        //Create adj payroll
						//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
                        CreateAdjustmentPayrollForEnrollmentHistoryCloseMedicare(lbusClosedMedicareHistory.icdoPersonAccountMedicarePartDHistory.end_date,
                               lbusClosedMedicareHistory.icdoPersonAccountMedicarePartDHistory.start_date, lbusClosedMedicareHistory.icdoPersonAccountMedicarePartDHistory.member_person_id,
                               lbusClosedMedicareHistory, lbusAddedMedicareHistory.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value); //PIR 15418 The record should insert to IBS Detail with the Person ID = Member Person ID from Medicare Part D History
                    }
                    else
                    {
                        //Create adj payroll
						//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
                        CreateAdjustmentPayrollForEnrollmentHistoryCloseMedicare(lbusClosedMedicareHistory.icdoPersonAccountMedicarePartDHistory.end_date.AddMonths(1),
                                lbusClosedMedicareHistory.icdoPersonAccountMedicarePartDHistory.start_date, lbusClosedMedicareHistory.icdoPersonAccountMedicarePartDHistory.member_person_id,
                                lbusClosedMedicareHistory, lbusAddedMedicareHistory.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value); //PIR 15418 The record should insert to IBS Detail with the Person ID = Member Person ID from Medicare Part D History
                    }
                }
                else if (lbusClosedMedicareHistory == null)
                {
                    CreateAdjustmentPayrollForEnrollmentHistoryCloseMedicare(lbusAddedMedicareHistory.icdoPersonAccountMedicarePartDHistory.start_date,
                            lbusAddedMedicareHistory.icdoPersonAccountMedicarePartDHistory.start_date,
                            lbusAddedMedicareHistory.icdoPersonAccountMedicarePartDHistory.member_person_id, lbusAddedMedicareHistory, lbusAddedMedicareHistory.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value);
                }
                if (lbusAddedMedicareHistory != null &&
                    lbusAddedMedicareHistory.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled)
                {
                    //Create adj payroll
                    CreateAdjustmentPayrollForEnrollmentHistoryAddMedicare(lbusAddedMedicareHistory.icdoPersonAccountMedicarePartDHistory.start_date, lbusAddedMedicareHistory.icdoPersonAccountMedicarePartDHistory.member_person_id); //PIR 15418 The record should insert to IBS Detail with the Person ID = Member Person ID from Medicare Part D History
                }
            }

            if ((icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceSuspended &&
                istrPreviousPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceSuspended) ||
                (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceCancelled &&
                istrPreviousPlanParticipationStatus != busConstant.PlanParticipationStatusInsuranceCancelled))
            {
                //End PAPIT Entries for suspended accounts
                ibusPaymentElection.EndPAPITEntriesForSuspendedAccount(icdoPersonAccount.plan_id, icdoPersonAccount.current_plan_start_date);
                // ignore if already in review IBS entries
                if (icdoPersonAccount.status_value == busConstant.PersonAccountStatusValid)
                {
                    DBFunction.DBNonQuery("cdoIbsDetail.UpdateAllIBSDetailStatusToIgnoreAfterPlanSusorCan", new object[2] { icdoPersonAccount.person_account_id, icdoPersonAccount.current_plan_start_date },
                                       iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);

                    DBFunction.DBNonQuery("entEmployerPayrollHeader.UpdateAllEPDetailStatusToIgnoreAfterPlanSusorCan", new object[3] {
                                    icdoPersonAccount.person_id, icdoPersonAccount.plan_id, icdoPersonAccount.current_plan_start_date },
                                    iobjPassInfo.iconFramework, iobjPassInfo.itrnFramework);
                }
            }
            else if (icdoPersonAccount.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled) 
            {
                if (iintOldPayeeAccountID != 0 && iintOldPayeeAccountID != ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id) //PIR 16393 - Wrong insurance deduction
                {
                    LoadActiveProviderOrgPlan(GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                            idtNextBenefiPaymentDate));

                    GetMonthlyPremiumAmountForMedicarePartD(GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                        idtNextBenefiPaymentDate));

                    ibusPaymentElection.ManagePayeeAccountPaymentItemType(ibusPaymentElection.icdoPersonAccountPaymentElection.payee_account_id,
                                        iintOldPayeeAccountID, icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount, icdoPersonAccount.plan_id,
                                        GetLatestDate(icdoPersonAccount.current_plan_start_date, ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date,
                                        idtNextBenefiPaymentDate), ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value, ibusProviderOrgPlan.icdoOrgPlan.org_id,icdoPersonAccount.person_account_id, true, true);
                }
            }
            //LoadPreviousHistory();
            GetMonthlyPremiumAmountForMedicarePartD();
        }

        public void GetMonthlyPremiumAmountForMedicarePartD()
        {
            if (iblnIsFromIBSBilling)
            {
                idtPlanEffectiveDate = busGlobalFunctions.GetSysManagementBatchDate().AddMonths(1);
                idtPlanEffectiveDate = new DateTime(idtPlanEffectiveDate.Year, idtPlanEffectiveDate.Month, 1);
            }
            else
            {
                if (idtPlanEffectiveDate == DateTime.MinValue)
                    LoadPlanEffectiveDate();
            }
            GetMonthlyPremiumAmountForMedicarePartD(idtPlanEffectiveDate);
        }

        public void GetMonthlyPremiumAmountForMedicarePartD(DateTime adtEffectiveDate)
        {
            //Always load Low_income_credit amount - because facing issues with reporting and posting insurance batch (low_income_credit column in both ghdv and medicare table - thats why need to load from db again).
            busPersonAccountMedicarePartDHistory lbusPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory();
            
            lbusPersonAccountMedicarePartDHistory.FindMedicareByPersonAccountIDAndEffectiveDate(icdoPersonAccountMedicarePartDHistory.person_account_id, adtEffectiveDate);
            decimal lLowIncomeCredit = lbusPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.low_income_credit;

            icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount =
                busRateHelper.GetMedicarePartDPremiumAmount(adtEffectiveDate, lLowIncomeCredit, idtbCachedHealthRate, iobjPassInfo);

            icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount = icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount + icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty;
        }

        public void GetPremiumAmountFromRef()
        {
            if (iblnIsFromIBSBilling || iblnIsFromEmployerReportPosting)
            {
                idtPlanEffectiveDate = busGlobalFunctions.GetSysManagementBatchDate().AddMonths(1);
                idtPlanEffectiveDate = new DateTime(idtPlanEffectiveDate.Year, idtPlanEffectiveDate.Month, 1);
            }
            else
            {
                if (idtPlanEffectiveDate == DateTime.MinValue)
                    LoadPlanEffectiveDate();
            }

            icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef =
                busRateHelper.GetMedicarePartDPremiumAmountFromRef(idtPlanEffectiveDate, idtbCachedHealthRate, iobjPassInfo);
        }

        public void GetPremiumAmountFromRef(DateTime adtEffectiveDate)
        {
            icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef =
                busRateHelper.GetMedicarePartDPremiumAmountFromRef(adtEffectiveDate, idtbCachedHealthRate, iobjPassInfo);
        }

        public void GetTotalPremiumAmountForMedicare()
        {
            int lintDependentID = 0;
            TotalMonthlyPremiumAmount = 0.00M;
            DataTable ldtbList = Select<cdoPersonAccountMedicarePartDHistory>(
                new string[1] { "person_id" },
                new object[1] { icdoPersonAccountMedicarePartDHistory.person_id }, null, null);

            iclbMedicarePartDTemp = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbList, "icdoPersonAccountMedicarePartDHistory");
            //lintDependentID = LoadPersonContact();

            //if (lintDependentID > 0)
            //{
            //    DataTable ldtbList1 = Select<cdoPersonAccountMedicarePartDHistory>(
            //    new string[1] { "person_id" },
            //    new object[1] { lintDependentID }, null, null);
            //    iclbMedicarePartDTemp1 = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbList1, "icdoPersonAccountMedicarePartDHistory");
            //    iclbMedicarePartDTemp = iclbMedicarePartDTemp.Union(iclbMedicarePartDTemp1).ToList().ToCollection();
            //}

            foreach (busPersonAccountMedicarePartDHistory lobjHistory in iclbMedicarePartDTemp)
            {
                lobjHistory.FindPersonAccount(lobjHistory.icdoPersonAccountMedicarePartDHistory.person_account_id);
                if (iblnIsFromIBSBilling)
                {
                    idtPlanEffectiveDate = busGlobalFunctions.GetSysManagementBatchDate().AddMonths(1);
                    idtPlanEffectiveDate = new DateTime(idtPlanEffectiveDate.Year, idtPlanEffectiveDate.Month, 1);
                }
                else
                {
                    if (idtPlanEffectiveDate == DateTime.MinValue)
                        LoadPlanEffectiveDate();
                }
                GetMonthlyPremiumAmountForMedicarePartD(); //PIR 16698
                
                if (lobjHistory.icdoPersonAccountMedicarePartDHistory.start_date != lobjHistory.icdoPersonAccountMedicarePartDHistory.end_date)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(idtPlanEffectiveDate, lobjHistory.icdoPersonAccountMedicarePartDHistory.start_date,
                            lobjHistory.icdoPersonAccountMedicarePartDHistory.end_date))
                        TotalMonthlyPremiumAmount += icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount;
                }
            }
        }

        public void GetTotalPremiumAmountForMedicareForPapit()
        {
            //int lintDependentID = 0;
            TotalMonthlyPremiumAmountPAPIT = 0.00M;
            DataTable ldtbList = Select<cdoPersonAccountMedicarePartDHistory>(
                new string[2] { "member_person_id" , "plan_participation_status_value"},
                new object[2] { icdoPersonAccountMedicarePartDHistory.member_person_id, busConstant.PlanParticipationStatusInsuranceEnrolled }, null, null);

            iclbMedicarePartDTemp = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbList, "icdoPersonAccountMedicarePartDHistory");
            //lintDependentID = LoadPersonContact();

            //if (lintDependentID > 0)
            //{
            //    DataTable ldtbList1 = Select<cdoPersonAccountMedicarePartDHistory>(
            //    new string[1] { "person_id" },
            //    new object[1] { lintDependentID }, null, null);
            //    iclbMedicarePartDTemp1 = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbList1, "icdoPersonAccountMedicarePartDHistory");
            //    iclbMedicarePartDTemp = iclbMedicarePartDTemp.Union(iclbMedicarePartDTemp1).ToList().ToCollection();
            //}

            foreach (busPersonAccountMedicarePartDHistory lobjHistory in iclbMedicarePartDTemp)
            {
                lobjHistory.FindPersonAccount(lobjHistory.icdoPersonAccountMedicarePartDHistory.person_account_id);
                if (iblnIsFromIBSBilling)
                {
                    idtPlanEffectiveDate = busGlobalFunctions.GetSysManagementBatchDate().AddMonths(1);
                    idtPlanEffectiveDate = new DateTime(idtPlanEffectiveDate.Year, idtPlanEffectiveDate.Month, 1);
                }
                else
                {
                    if (idtPlanEffectiveDate == DateTime.MinValue)
                        LoadPlanEffectiveDate();
                }
                lobjHistory.GetMonthlyPremiumAmountForMedicarePartD();

                if (lobjHistory.icdoPersonAccountMedicarePartDHistory.start_date != lobjHistory.icdoPersonAccountMedicarePartDHistory.end_date)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(idtPlanEffectiveDate, lobjHistory.icdoPersonAccountMedicarePartDHistory.start_date,
                            lobjHistory.icdoPersonAccountMedicarePartDHistory.end_date))
                        TotalMonthlyPremiumAmountPAPIT += lobjHistory.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount;
                }
            }
        }

        //PIR 6933 - Member and Spouse Premium To be shown under Member's Rate Change Letter.
        public void GetTotalPremiumAmountForMedicareInsuranceRateChanage(DateTime adtEffectiveDate)
        {
            TotalMonthlyPremiumAmount = 0.00M;
            DataTable ldtbList = Select("cdoPersonAccountMedicarePartDHistory.LoadPerAccMedHisByMemPersonId",
                new object[2] { icdoPersonAccountMedicarePartDHistory.member_person_id, adtEffectiveDate });

            iclbMedicarePartDTemp = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbList, "icdoPersonAccountMedicarePartDHistory");
            foreach (busPersonAccountMedicarePartDHistory lobjHistory in iclbMedicarePartDTemp)
            {
                lobjHistory.FindPersonAccount(lobjHistory.icdoPersonAccountMedicarePartDHistory.person_account_id);
                lobjHistory.ibusPaymentElection = new busPersonAccountPaymentElection();
                lobjHistory.ibusPaymentElection.FindPersonAccountPaymentElectionByPersonAccountID(lobjHistory.icdoPersonAccountMedicarePartDHistory.person_account_id);
                lobjHistory.idtPlanEffectiveDate = adtEffectiveDate;
                lobjHistory.GetMonthlyPremiumAmountForMedicarePartD();

                if (busGlobalFunctions.CheckDateOverlapping(lobjHistory.idtPlanEffectiveDate, lobjHistory.icdoPersonAccountMedicarePartDHistory.start_date,
                        lobjHistory.icdoPersonAccountMedicarePartDHistory.end_date) && lobjHistory.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id == 0
                     && lobjHistory.ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_effective_date <= adtEffectiveDate)
                    TotalMonthlyPremiumAmount += lobjHistory.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmount;
            }
        }

        public void InsertHistory()
        {
            cdoPersonAccountMedicarePartDHistory lobjCdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
            lobjCdoPersonAccountMedicarePartDHistory.person_account_id = icdoPersonAccount.person_account_id;
            lobjCdoPersonAccountMedicarePartDHistory.person_id = icdoPersonAccount.person_id;
            lobjCdoPersonAccountMedicarePartDHistory.start_date = icdoPersonAccount.history_change_date;
            lobjCdoPersonAccountMedicarePartDHistory.plan_participation_status_value = icdoPersonAccount.plan_participation_status_value;
            lobjCdoPersonAccountMedicarePartDHistory.status_value = icdoPersonAccount.status_value;

            lobjCdoPersonAccountMedicarePartDHistory.reason_value = icdoPersonAccountMedicarePartDHistory.reason_value;
            lobjCdoPersonAccountMedicarePartDHistory.suppress_warnings_flag = icdoPersonAccountMedicarePartDHistory.suppress_warnings_flag;
            lobjCdoPersonAccountMedicarePartDHistory.medicare_claim_no = icdoPersonAccountMedicarePartDHistory.medicare_claim_no;
            lobjCdoPersonAccountMedicarePartDHistory.medicare_part_a_effective_date = icdoPersonAccountMedicarePartDHistory.medicare_part_a_effective_date;
            lobjCdoPersonAccountMedicarePartDHistory.medicare_part_b_effective_date = icdoPersonAccountMedicarePartDHistory.medicare_part_b_effective_date;
            lobjCdoPersonAccountMedicarePartDHistory.low_income_credit = icdoPersonAccountMedicarePartDHistory.low_income_credit;
            lobjCdoPersonAccountMedicarePartDHistory.late_enrollment_penalty = icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty;
            lobjCdoPersonAccountMedicarePartDHistory.member_person_id = icdoPersonAccountMedicarePartDHistory.member_person_id;
            UpdateEnrollmentAndRecordTypeFlags();
            lobjCdoPersonAccountMedicarePartDHistory.record_type_flag = icdoPersonAccountMedicarePartDHistory.record_type_flag;
            lobjCdoPersonAccountMedicarePartDHistory.enrollment_file_sent_flag = icdoPersonAccountMedicarePartDHistory.enrollment_file_sent_flag;
            
            lobjCdoPersonAccountMedicarePartDHistory.send_after = DateTime.Now.Date;

            if (iclbPersonAccountMedicarePartDHistory == null)
                LoadPersonAccountMedicarePartDHistory(icdoPersonAccountMedicarePartDHistory.person_id);

            if (icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonInitialEnrollDateChange)
                lobjCdoPersonAccountMedicarePartDHistory.initial_enroll_date = icdoPersonAccountMedicarePartDHistory.initial_enroll_date;
            else if (iclbPersonAccountMedicarePartDHistory.Where(i => i.icdoPersonAccountMedicarePartDHistory.start_date != i.icdoPersonAccountMedicarePartDHistory.end_date &&
                    i.icdoPersonAccountMedicarePartDHistory.start_date > i.icdoPersonAccountMedicarePartDHistory.end_date &&
                    i.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled).Count() == 0)
                lobjCdoPersonAccountMedicarePartDHistory.initial_enroll_date = icdoPersonAccount.history_change_date;
            else
                lobjCdoPersonAccountMedicarePartDHistory.initial_enroll_date = ibusHistory.icdoPersonAccountMedicarePartDHistory.initial_enroll_date;

            //PIR 23944 - If initial_enroll_date is null then updating it to PA start date
            if (lobjCdoPersonAccountMedicarePartDHistory.initial_enroll_date == DateTime.MinValue)
                lobjCdoPersonAccountMedicarePartDHistory.initial_enroll_date = icdoPersonAccount.start_date;

            lobjCdoPersonAccountMedicarePartDHistory.provider_org_id = icdoPersonAccount.provider_org_id;

            lobjCdoPersonAccountMedicarePartDHistory.Insert();
           
        }

        public void UpdateEnrollmentAndRecordTypeFlags()
        {
            if (icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonMedicareEligibility ||
                icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonNewRetiree)
            {
                icdoPersonAccountMedicarePartDHistory.record_type_flag = "E";
                icdoPersonAccountMedicarePartDHistory.enrollment_file_sent_flag = busConstant.Flag_No;
            }

            if (icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonMemberCancellation ||
                icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonNonPayment ||
                icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonDeath ||
                icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonCMSLossOfEligibility)
            {
                icdoPersonAccountMedicarePartDHistory.record_type_flag = "D";
                icdoPersonAccountMedicarePartDHistory.enrollment_file_sent_flag = busConstant.Flag_No;
            }

            if (icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonIBSPaymentMethod ||
                icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonLowIncomeSubsidy ||
                icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonDateCorrection ||
            icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonLateEnrollmentPenalty)
            {
                icdoPersonAccountMedicarePartDHistory.record_type_flag = string.Empty;
                icdoPersonAccountMedicarePartDHistory.enrollment_file_sent_flag = busConstant.Flag_Yes;
            }

            if (icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonDataCorrection ||
                icdoPersonAccountMedicarePartDHistory.reason_value == busConstant.ChangeReasonInitialEnrollDateChange)
            {
                icdoPersonAccountMedicarePartDHistory.record_type_flag = "C";
                icdoPersonAccountMedicarePartDHistory.enrollment_file_sent_flag = busConstant.Flag_No;
            }

        }

        //Loading previous history record
        public void LoadPreviousHistory()
        {
            if (_ibusHistory == null)
            {
                _ibusHistory = new busPersonAccountMedicarePartDHistory();
                _ibusHistory.icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();
            }

            if (iclbPersonAccountMedicarePartDHistory == null)
                LoadPersonAccountMedicarePartDHistory(icdoPersonAccountMedicarePartDHistory.person_id);

            if (iclbPersonAccountMedicarePartDHistory.Count > 0)
                ibusHistory = iclbPersonAccountMedicarePartDHistory.First();
        }

        private Collection<busPersonAccountMedicarePartDHistory> LoadOverlappingHistory()
        {
            if (iclbPersonAccountMedicarePartDHistory == null)
                LoadPersonAccountMedicarePartDHistory(icdoPersonAccountMedicarePartDHistory.person_id);

            Collection<busPersonAccountMedicarePartDHistory> lclbPAMedicarePartDHistory = new Collection<busPersonAccountMedicarePartDHistory>();
            var lenuList = iclbPersonAccountMedicarePartDHistory.Where(i => busGlobalFunctions.CheckDateOverlapping(icdoPersonAccount.history_change_date,
                i.icdoPersonAccountMedicarePartDHistory.start_date, i.icdoPersonAccountMedicarePartDHistory.end_date)
                || i.icdoPersonAccountMedicarePartDHistory.start_date > icdoPersonAccount.history_change_date);
                
            foreach (busPersonAccountMedicarePartDHistory lobjHistory in lenuList)
            {
                if (lobjHistory.icdoPersonAccountMedicarePartDHistory.start_date >= icdoPersonAccount.history_change_date)
                {
                    lclbPAMedicarePartDHistory.Add(lobjHistory);
                }
                else if (lobjHistory.icdoPersonAccountMedicarePartDHistory.start_date == lobjHistory.icdoPersonAccountMedicarePartDHistory.end_date)
                {
                    lclbPAMedicarePartDHistory.Add(lobjHistory);
                }
                else if (lobjHistory.icdoPersonAccountMedicarePartDHistory.start_date != lobjHistory.icdoPersonAccountMedicarePartDHistory.end_date)
                {
                    break;
                }

            }
            return lclbPAMedicarePartDHistory;
        }

        public bool IsMoreThanOneEnrolledInOverlapHistory()
        {
            if (istrAllowOverlapHistory == busConstant.Flag_Yes)
            {
                if (iclbOverlappingHistory != null)
                {
                    var lenuList = iclbOverlappingHistory.Where(i => i.icdoPersonAccountMedicarePartDHistory.plan_participation_status_value == busConstant.PlanParticipationStatusInsuranceEnrolled &&
                        i.icdoPersonAccountMedicarePartDHistory.start_date != i.icdoPersonAccountMedicarePartDHistory.end_date);
                    //PIR Enhanced Overlap - 23167, 23340, 23408
                    if (IsResourceForEnhancedOverlap(busConstant.OverlapResourceEnhancedGHDV))
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

        public busPersonAccountMedicarePartDHistory LoadHistoryByDate(DateTime adtGivenDate)
        {
            busPersonAccountMedicarePartDHistory lobjPersonAccountMedicarePartDHistory = new busPersonAccountMedicarePartDHistory();
            lobjPersonAccountMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory = new cdoPersonAccountMedicarePartDHistory();

            if (iclbPersonAccountMedicarePartDHistory == null)
                LoadPersonAccountMedicarePartDHistory(icdoPersonAccountMedicarePartDHistory.person_id);

            foreach (busPersonAccountMedicarePartDHistory lobjPAMedicarePartDHistory in iclbPersonAccountMedicarePartDHistory)
            {
                //Ignore the Same Start Date and End Date Records
                if (lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.start_date != lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.end_date)
                {
                    if (busGlobalFunctions.CheckDateOverlapping(adtGivenDate, lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.start_date,
                        lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.end_date))
                    {
                        lobjPersonAccountMedicarePartDHistory = lobjPAMedicarePartDHistory;
                        break;
                    }
                }
            }
            return lobjPersonAccountMedicarePartDHistory;
        }

        public void LoadMedicarePartDMembersIBS(int AintPersonAccountID)
        {
            if (iclbPersonAccountMedicarePartDMembersIBS == null)
                iclbPersonAccountMedicarePartDMembersIBS = new Collection<busPersonAccountMedicarePartDHistory>();
            //if (ibusPerson == null)
            //    LoadPerson();

            DataTable ldtbResult = Select("cdoPersonAccountMedicarePartDHistory.LoadEnrolledMembersForIBS", new object[1] { AintPersonAccountID });
            iclbPersonAccountMedicarePartDMembersIBS = GetCollection<busPersonAccountMedicarePartDHistory>(ldtbResult, "icdoPersonAccountMedicarePartDHistory");
        }
        /// <summary>
        /// PIR_15418 added extra parameter, aintMemberPersonID, the record should insert to IBS Detail with the Person ID = Member Person ID from Medicare Part D History
        /// </summary>
        /// <param name="adtEffectiveDate"></param>
        /// <param name="aintMemberPersonID"></param>
        public void CreateAdjustmentPayrollForEnrollmentHistoryAddMedicare(DateTime adtEffectiveDate, int aintMemberPersonID)
        {
            int lintOrgID = 0;

            if (iclbInsuranceContributionAll == null)
                LoadInsuranceContributionAll();
            if (ibusPaymentElection == null)
                LoadPaymentElection();

            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_No)
            {
                if (ibusPersonEmploymentDetail == null)
                {
                    LoadPersonEmploymentDetail();
                }
                if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                {
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                }
                lintOrgID = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
            }
            else if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0)
            {
                lintOrgID = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id;
            }
            
            CreateIBSAdjustmentForEnrollmentHistoryAddMedicare(adtEffectiveDate, aintMemberPersonID); //PIR_15418
            CreateEmployerAdjustmentForEnrollmentHistoryAddMedicare(adtEffectiveDate, aintMemberPersonID); //PIR_15418

            GetMonthlyPremiumAmountForMedicarePartD(adtEffectiveDate);
        }

        /// <summary>
        /// PIR_15418 added extra parameter, aintMemberPersonID, the record should insert to IBS Detail with the Person ID = Member Person ID from Medicare Part D History
        /// </summary>
        /// <param name="adtEffectiveDate"></param>
        /// <param name="adtEndedHistoryStartDate"></param>
        /// <param name="aintMemberPersonID"></param>
        public void CreateAdjustmentPayrollForEnrollmentHistoryCloseMedicare(DateTime adtEffectiveDate, DateTime adtEndedHistoryStartDate, int aintMemberPersonID, busPersonAccountMedicarePartDHistory lbusClosedMedicareHistory, string astrHistoryAddedStatus = null)
        {
            busEmployerPayrollHeader lbusPayrollHdr = null;
            busIbsHeader lbusIBSHdr = null;
            int lintOrgID = 0;
            DateTime ldatEffectedDate = adtEffectiveDate;
            ldatEffectedDate = new DateTime(ldatEffectedDate.Year, ldatEffectedDate.Month, 1);
            if (iclbInsuranceContributionAll == null)
                LoadInsuranceContributionAll();
            if (ibusPaymentElection == null)
                LoadPaymentElection();
            if (ibusPerson == null)
                LoadPerson();

            //need to group IBS and employer reporting as separate
            var lenuContributionByMonthForIBS =
                iclbInsuranceContributionAll.Where(i => i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit)
                .GroupBy(i => i.icdoPersonAccountInsuranceContribution.effective_date).Select(o => o.First());

            if (lenuContributionByMonthForIBS != null)
            {
                foreach (busPersonAccountInsuranceContribution lbusInsuranceContribution in lenuContributionByMonthForIBS)
                {
                    if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date >= ldatEffectedDate)
                    { 
                        if ((lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit)
                            //&&
                            //((lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.due_premium_amount > 0))
                            )
                        {
                            if (lbusIBSHdr == null)
                            {
                                lbusIBSHdr = new busIbsHeader();
                                if (!lbusIBSHdr.LoadCurrentAdjustmentIBSHeader())
                                {
                                    lbusIBSHdr.CreateAdjustmentIBSHeader();
                                    lbusIBSHdr.icolIbsDetail = new Collection<busIbsDetail>();
                                }
                                else
                                {
                                    lbusIBSHdr.icdoIbsHeader.ienuObjectState = ObjectState.Update;
                                    lbusIBSHdr.LoadIbsDetails();
                                }
                            }

                            //PIR 15786
                            busPersonAccountMedicarePartDHistory lobjPAMedicarePartDHistory = lbusClosedMedicareHistory;//LoadHistoryByDate(adtEndedHistoryStartDate);
                            if (lobjPAMedicarePartDHistory != null)
                            {
                                lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef =
                                                                        busRateHelper.GetMedicarePartDPremiumAmountFromRef(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, 
                                                                        idtbCachedHealthRate, iobjPassInfo);
                            }

                            decimal ldecMemberPremium = 0.00M;
                            decimal ldecTotalPremiumAmount = 0.00M;
                            
                            var lclbFilterdContribution = _iclbInsuranceContributionAll.Where(
                                i =>
                                i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSDeposit &&
                                i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);
                            
                            var lclbPaidFilterdContribution = _iclbInsuranceContributionAll.Where(
                               i =>
                               i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueIBSPayment &&
                               i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);

                            if (lclbFilterdContribution != null)
                            {
                                ldecMemberPremium = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.due_premium_amount);
                                ldecTotalPremiumAmount = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.due_premium_amount);
                            }
                            string lstrPaymentMethod = string.Empty;
                            
                            if (ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentACH)
                            {
                                LoadPersonAccountAchDetail();
                                if (iclbPersonAccountAchDetail.Count == 0)
                                {
                                    lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                                }
                                busPersonAccountAchDetail lobjACHDetail = iclbPersonAccountAchDetail.Where(o => busGlobalFunctions.CheckDateOverlapping(lbusIBSHdr.icdoIbsHeader.billing_month_and_year,
                                    o.icdoPersonAccountAchDetail.ach_start_date, o.icdoPersonAccountAchDetail.ach_end_date == DateTime.MinValue ? DateTime.MaxValue : o.icdoPersonAccountAchDetail.ach_end_date))
                                    .FirstOrDefault();
                                if (lobjACHDetail != null && lobjACHDetail.icdoPersonAccountAchDetail.pre_note_flag == busConstant.Flag_No)
                                {
                                    lstrPaymentMethod = ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
                                }
                                else
                                    lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                            }
                            else
                            {
                                lstrPaymentMethod = ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
                            }

                            //Low Income Credit Amount should be populated from Ref table. 
                            DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                            Decimal ldecLowIncomeCreditAmount = 0;
                            var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.low_income_credit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                            foreach (DataRow dr in lenumList)
                            {
                                if (Convert.ToDateTime(dr["effective_date"]).Date <= lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date.Date)
                                {
                                    ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                                    break;
                                }
                            }
							
							//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
                            if (icolPosNegIbsDetail.IsNull())
                                icolPosNegIbsDetail = new Collection<busIbsDetail>();

                            if (ldecMemberPremium != 0 &&
                                (IsAdjustmentExistsPriorToEnteredEffectiveDate(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, -1M * ldecMemberPremium, istrOldPaymentMethod)))
                                lbusIBSHdr.AddIBSDetailForMedicare(icdoPersonAccount.person_account_id, aintMemberPersonID, icdoPersonAccount.plan_id,
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date,
                                    istrOldPaymentMethod, -1M * ldecMemberPremium,
                                    -1M * ldecTotalPremiumAmount,
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                    -1M * ldecLowIncomeCreditAmount, -1M * lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty,
                                    -1M * lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef, acolNegPosIBsDetail : icolPosNegIbsDetail);//PIR 15786
                        }
                    }
                }
            }

            //need to group IBS and employer reporting as separate
            var lenuContributionByMonthForPayroll =
                iclbInsuranceContributionAll.Where(i => i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting)
                .GroupBy(i => i.icdoPersonAccountInsuranceContribution.effective_date).Select(o => o.First());

            if (lenuContributionByMonthForPayroll != null)
            {
                foreach (busPersonAccountInsuranceContribution lbusInsuranceContribution in lenuContributionByMonthForPayroll)
                {
                    if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date >= ldatEffectedDate)
                    { //?? Need to address subsystem_value for converted data
                        if (lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting)
                        {
                            if (lbusPayrollHdr == null)
                            {
                                // As only most recent record can be ended then all these row should belong to same org id
                                busPersonEmploymentDetail lbusEmploymentDetail = new busPersonEmploymentDetail();
                                lbusEmploymentDetail.FindPersonEmploymentDetail(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.person_employment_dtl_id);
                                lbusEmploymentDetail.LoadPersonEmployment();
                                lintOrgID = lbusEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;

                                //There are scenario, where Regular Payroll may not come.. at that time, we need to load the employment in different wat
                                if (lintOrgID == 0)
                                {
                                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0)
                                        lintOrgID = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id;
                                    else
                                    {
                                        if (ibusPersonEmploymentDetail == null)
                                        {
                                            LoadPersonEmploymentDetail();
                                        }
                                        if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                                        {
                                            ibusPersonEmploymentDetail.LoadPersonEmployment();
                                        }
                                        lintOrgID = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
                                    }
                                }
                                //prod pir 5831
                                //For COBRA members, we dont have an option to find org id, so using substem ref id, load payroll detail and then load payroll header
                                //and get the org id
                                if (lintOrgID == 0)
                                {
                                    busEmployerPayrollDetail lobjPayrollDtl = new busEmployerPayrollDetail();
                                    if (lobjPayrollDtl.FindEmployerPayrollDetail(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.subsystem_ref_id))
                                    {
                                        lobjPayrollDtl.LoadPayrollHeader();
                                        lintOrgID = lobjPayrollDtl.ibusEmployerPayrollHeader.icdoEmployerPayrollHeader.org_id;
                                    }
                                }

                                lbusPayrollHdr = new busEmployerPayrollHeader();
                                if (!lbusPayrollHdr.LoadCurrentAdjustmentPayrollHeader(lintOrgID, busConstant.PayrollHeaderBenefitTypeInsr))
                                {
                                    lbusPayrollHdr.CreateInsuranceAdjustmentPayrollHeader(lintOrgID);
                                    lbusPayrollHdr.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
                                }
                                else
                                {
                                    lbusPayrollHdr.icdoEmployerPayrollHeader.ienuObjectState = ObjectState.Update;
                                    lbusPayrollHdr.LoadEmployerPayrollDetail();
                                }
                            }

                            busPersonAccountMedicarePartDHistory lobjPAMedicarePartDHistory = lbusClosedMedicareHistory;

                            if (lintOrgID > 0)
                            {
                                decimal ldecTotalPremium = 0.00M;
                                
                                string lstrGroupNumber = string.Empty;
                                
                                string lstrCoverageCodeValue = string.Empty, lstrRateStructureCode = string.Empty;
                                var lclbFilterdContribution = _iclbInsuranceContributionAll.Where(
                                    i =>
                                    i.icdoPersonAccountInsuranceContribution.subsystem_value == busConstant.SubSystemValueEmployerReporting &&
                                    i.icdoPersonAccountInsuranceContribution.effective_date == lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);

                                if (lclbFilterdContribution != null)
                                {
                                    ldecTotalPremium = lclbFilterdContribution.Sum(i => i.icdoPersonAccountInsuranceContribution.due_premium_amount);
                                }

                                //PIR 15434
                                string astrSSN = string.Empty;
                                if (aintMemberPersonID != ibusPerson.icdoPerson.person_id)
                                {
                                    busPerson abusPersonMember = new busPerson();
                                    abusPersonMember.FindPerson(aintMemberPersonID);
                                    astrSSN = abusPersonMember.icdoPerson.ssn;
                                }
                                else
                                    astrSSN = ibusPerson.icdoPerson.ssn;

                                //Low Income Credit Amount should be populated from Ref table. 
                                DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                                Decimal ldecLowIncomeCreditAmount = 0;
                                var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.low_income_credit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                                foreach (DataRow dr in lenumList)
                                {
                                    if (Convert.ToDateTime(dr["effective_date"]).Date <= lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date.Date)
                                    {
                                        ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                                        break;
                                    }
                                }

                                if (icolPosNegEmployerPayrollDtl.IsNull())
                                    icolPosNegEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();
                                if (ldecTotalPremium > 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, 
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecTotalPremium, busConstant.PayrollDetailRecordTypeNegativeAdjustment))
                                {
                                    lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(aintMemberPersonID, icdoPersonAccount.plan_id,
                                                                                       ibusPerson.icdoPerson.first_name, ibusPerson.icdoPerson.last_name,
                                                                                       astrSSN, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                                                                                       lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecTotalPremium,
                                                                                       lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                                                                       ldecLowIncomeCreditAmount, lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty,
                                                                                       aintPersonAccountID: icdoPersonAccount.person_account_id, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);//Org to bill
                                }
                                else if (ldecTotalPremium < 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id, 
                                    lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date, ldecTotalPremium * -1, busConstant.PayrollDetailRecordTypePositiveAdjustment))
                                {
                                    lbusPayrollHdr.UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date);
                                    lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(aintMemberPersonID, icdoPersonAccount.plan_id,
                                                                                       ibusPerson.icdoPerson.first_name, ibusPerson.icdoPerson.last_name,
                                                                                       astrSSN, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                                                                                       lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.effective_date,
                                                                                       ldecTotalPremium * -1, lbusInsuranceContribution.icdoPersonAccountInsuranceContribution.provider_org_id,
                                                                                       -1M * ldecLowIncomeCreditAmount, -1M * lobjPAMedicarePartDHistory.icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty,
                                                                                       aintPersonAccountID: icdoPersonAccount.person_account_id, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);//Org to bill
                                }
                            }
                        }
                    }
                }
            }
            // Update changes
			//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
            if ((lbusIBSHdr != null) && (lbusIBSHdr.icolIbsDetail != null) && (lbusIBSHdr.icolIbsDetail.Count > 0) && astrHistoryAddedStatus != busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                lbusIBSHdr.UpdateSummaryData(busConstant.IBSHeaderStatusReview); 
                foreach (busIbsDetail lbusIBSDtl in lbusIBSHdr.icolIbsDetail)
                {
                    lbusIBSDtl.icdoIbsDetail.ibs_header_id = lbusIBSHdr.icdoIbsHeader.ibs_header_id;
                    lbusIBSDtl.UpdateDataObject(lbusIBSDtl.icdoIbsDetail);
                }
            }
            if ((lbusPayrollHdr != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail != null) && (lbusPayrollHdr.iclbEmployerPayrollDetail.Count > 0) && astrHistoryAddedStatus != busConstant.PlanParticipationStatusInsuranceEnrolled)
            {
                lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                foreach (busEmployerPayrollDetail lbusPayrollDtl in lbusPayrollHdr.iclbEmployerPayrollDetail)
                {
                    lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                    lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                }
            }
        }
        /// <summary>
        /// PIR_15418, added extra parameter
        /// </summary>
        /// <param name="adtEffectiveDate"></param>
        /// <param name="aintMemberPersonID"></param>
        private void CreateIBSAdjustmentForEnrollmentHistoryAddMedicare(DateTime adtEffectiveDate, int aintMemberPersonID)
        {
            DateTime ldatCurrentPayPeriod = DateTime.MinValue;
            DateTime ldatEffectedDate = adtEffectiveDate;
            busIbsHeader lbusIBSHdr = null;
            
            //Pure IBS Member
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes &&
                ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id == 0)
            {
                DataTable ldtResult = Select<cdoIbsHeader>(
                                new string[2] { "report_type_value", "REPORT_STATUS_VALUE" },
                                new object[2] { busConstant.IBSHeaderReportTypeRegular, busConstant.IBSHeaderStatusPosted }, null, "BILLING_MONTH_AND_YEAR desc");
                if ((ldtResult != null) && (ldtResult.Rows.Count > 0))
                {
                    if (!Convert.IsDBNull(ldtResult.Rows[0]["BILLING_MONTH_AND_YEAR"]))
                    {
                        ldatCurrentPayPeriod = Convert.ToDateTime(ldtResult.Rows[0]["BILLING_MONTH_AND_YEAR"]);
                    }
                }

                if (ldatCurrentPayPeriod == DateTime.MinValue)
                {
                    cdoCodeValue lcdoCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantsAndVariablesCodeID, busConstant.SystemConstantsLastIBSPostingDate);
                    ldatCurrentPayPeriod = new DateTime(Convert.ToDateTime(lcdoCodeValue.data1).Year, Convert.ToDateTime(lcdoCodeValue.data1).Month, 1);
                }
            }

            while (ldatEffectedDate <= ldatCurrentPayPeriod)
            {
                LoadActiveProviderOrgPlan(ldatEffectedDate);

                decimal ldecCalculatedPremiumAmount = 0.00m;

                //GetMonthlyPremiumAmountForMedicarePartD(ldatEffectedDate);
                icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef =
                                                      busRateHelper.GetMedicarePartDPremiumAmountFromRef(ldatEffectedDate,
                                                      idtbCachedHealthRate, iobjPassInfo);

                if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes)
                {
                    string lstrCoverageCode = string.Empty;
                    if (lbusIBSHdr == null)
                    {
                        lbusIBSHdr = new busIbsHeader();
                        if (!lbusIBSHdr.LoadCurrentAdjustmentIBSHeader())
                        {
                            lbusIBSHdr.CreateAdjustmentIBSHeader();
                            lbusIBSHdr.icolIbsDetail = new Collection<busIbsDetail>();
                        }
                        else
                        {
                            lbusIBSHdr.icdoIbsHeader.ienuObjectState = ObjectState.Update;
                            //lbusIBSHdr.LoadIbsDetails();
                        }
                    }

                    string lstrPaymentMethod = string.Empty;
                    
                    if (ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value == busConstant.IBSModeOfPaymentACH)
                    {
                        LoadPersonAccountAchDetail();
                        if (iclbPersonAccountAchDetail.Count == 0)
                        {
                            lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                        }
                        busPersonAccountAchDetail lobjACHDetail = iclbPersonAccountAchDetail.Where(o => busGlobalFunctions.CheckDateOverlapping(lbusIBSHdr.icdoIbsHeader.billing_month_and_year,
                            o.icdoPersonAccountAchDetail.ach_start_date, o.icdoPersonAccountAchDetail.ach_end_date == DateTime.MinValue ? DateTime.MaxValue : o.icdoPersonAccountAchDetail.ach_end_date))
                            .FirstOrDefault();
                        if (lobjACHDetail != null && lobjACHDetail.icdoPersonAccountAchDetail.pre_note_flag == busConstant.Flag_No)
                        {
                            lstrPaymentMethod = ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
                        }
                        else
                            lstrPaymentMethod = busConstant.IBSModeOfPaymentPersonalCheck;
                    }
                    else
                    {
                        lstrPaymentMethod = ibusPaymentElection.icdoPersonAccountPaymentElection.payment_method_value;
                    }

                    LoadPlanEffectiveDate();

                    //Low Income Credit Amount should be populated from Ref table. 
                    DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                    Decimal ldecLowIncomeCreditAmount = 0;
                    var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == icdoPersonAccountMedicarePartDHistory.low_income_credit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                    foreach (DataRow dr in lenumList)
                    {
                        if (Convert.ToDateTime(dr["effective_date"]).Date <= ldatEffectedDate.Date)
                        {
                            ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                            break;
                        }
                    }
					
					ldecCalculatedPremiumAmount = icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef + icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty - ldecLowIncomeCreditAmount;
					
					//IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
                    if (icolPosNegIbsDetail.IsNull())
                        icolPosNegIbsDetail = new Collection<busIbsDetail>();

                    if (ldecCalculatedPremiumAmount >= 0)
                    {
                        //PIR 24918
                        if (IsAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, ldecCalculatedPremiumAmount, lstrPaymentMethod))
                        {
                            lbusIBSHdr.UpdateIBSDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_account_id, ldatEffectedDate);
                            lbusIBSHdr.AddIBSDetailForMedicare(icdoPersonAccount.person_account_id, aintMemberPersonID, icdoPersonAccount.plan_id,
                                ldatEffectedDate, lstrPaymentMethod,
                                ldecCalculatedPremiumAmount, ldecCalculatedPremiumAmount, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                                ldecLowIncomeCreditAmount, icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty,
                                icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef, acolNegPosIBsDetail: icolPosNegIbsDetail, aintPayeeAccountId: iPayeeAccountForPapit);//PIR 15786
                        }
                    }
                }

                ldatEffectedDate = ldatEffectedDate.AddMonths(1);
            }

            //IBS Adjustment creation logic changed (Affected area enroll to enroll scenario) - logged under Backlog PIR 8022 and 8111
            Collection<busIbsDetail> lcolFinal =  ComparePositiveNegativeColIbsDetail();
            // Update changes
            if (lbusIBSHdr != null && (lcolFinal != null) && (lcolFinal.Count > 0))
            {
                if (lbusIBSHdr != null)
                    lbusIBSHdr.UpdateSummaryData(busConstant.IBSHeaderStatusReview); //?? Save detail here
                foreach (busIbsDetail lbusIBSDtl in lcolFinal)
                {
                    if (lbusIBSHdr != null && lbusIBSDtl.icdoIbsDetail.ibs_header_id == 0)
                        lbusIBSDtl.icdoIbsDetail.ibs_header_id = lbusIBSHdr.icdoIbsHeader.ibs_header_id;
                    lbusIBSDtl.UpdateDataObject(lbusIBSDtl.icdoIbsDetail);
                }
            }
        }

        private void CreateEmployerAdjustmentForEnrollmentHistoryAddMedicare(DateTime adtEffectiveDate, int aintMemberPersonID)
        {
            busEmployerPayrollHeader lbusPayrollHdr = null;
            if (ibusPerson == null)
                LoadPerson();
            DateTime ldatEffectedDate = adtEffectiveDate;
            DateTime ldatCurrentPayPeriod = DateTime.MinValue;
            int lintOrgID = 0;
            //PIR 22767
            if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_No || ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.IsNullOrEmpty()
                || (!ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.IsNullOrEmpty()
                && ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.Trim() == string.Empty))
            {
                if (ibusPersonEmploymentDetail == null)
                {
                    LoadPersonEmploymentDetail();
                }
                if (ibusPersonEmploymentDetail.ibusPersonEmployment == null)
                {
                    ibusPersonEmploymentDetail.LoadPersonEmployment();
                }
                lintOrgID = ibusPersonEmploymentDetail.ibusPersonEmployment.icdoPersonEmployment.org_id;
            }
            else if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0)
            {
                lintOrgID = ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id;
            }
            if (lintOrgID > 0)
            {
                //PIR 22767
                if (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_No || ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.IsNullOrEmpty() || 
                    (!ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.IsNullOrEmpty() && ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag.Trim() == string.Empty) ||
                    (ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_flag == busConstant.Flag_Yes && ibusPaymentElection.icdoPersonAccountPaymentElection.ibs_org_id != 0)) 
                {
                    DataTable ldtResult = Select<cdoEmployerPayrollHeader>(
                           new string[4] { "org_id", "HEADER_TYPE_VALUE", "REPORT_TYPE_VALUE", "STATUS_VALUE" },
                           new object[4]
                            {
                                lintOrgID,busConstant.PayrollHeaderBenefitTypeInsr, 
                                busConstant.PayrollHeaderReportTypeRegular,busConstant.PayrollHeaderStatusPosted
                            }, null, "EMPLOYER_PAYROLL_HEADER_ID desc");
                    if ((ldtResult != null) && (ldtResult.Rows.Count > 0))
                    {
                        if (!Convert.IsDBNull(ldtResult.Rows[0]["PAYROLL_PAID_DATE"]))
                        {
                            ldatCurrentPayPeriod = Convert.ToDateTime(ldtResult.Rows[0]["PAYROLL_PAID_DATE"]);
                        }
                    }
                }

                if (ldatCurrentPayPeriod == DateTime.MinValue)
                {
                    cdoCodeValue lcdoCodeValue = busGlobalFunctions.GetCodeValueDetails(busConstant.SystemConstantsAndVariablesCodeID, busConstant.SystemConstantsLastEmployerPostingDate);
                    ldatCurrentPayPeriod = new DateTime(Convert.ToDateTime(lcdoCodeValue.data1).Year, Convert.ToDateTime(lcdoCodeValue.data1).Month, 1);
                }

                while (ldatEffectedDate <= ldatCurrentPayPeriod)
                {
                    decimal idecCalculatedPremiumAmount = 0.00m;
                    if (lbusPayrollHdr == null)
                    {
                        lbusPayrollHdr = new busEmployerPayrollHeader();
                        if (!lbusPayrollHdr.LoadCurrentAdjustmentPayrollHeader(lintOrgID, busConstant.PayrollHeaderBenefitTypeInsr))
                        {
                            lbusPayrollHdr.CreateInsuranceAdjustmentPayrollHeader(lintOrgID);
                            lbusPayrollHdr.iclbEmployerPayrollDetail = new Collection<busEmployerPayrollDetail>();
                        }
                        else
                        {
                            lbusPayrollHdr.icdoEmployerPayrollHeader.ienuObjectState = ObjectState.Update;
                            lbusPayrollHdr.LoadEmployerPayrollDetail();
                        }
                    }
                    
                    if (icdoPersonAccount.person_employment_dtl_id > 0)
                    {
                        LoadOrgPlan(ldatEffectedDate);
                        LoadProviderOrgPlan(ldatEffectedDate);
                    }
                    else
                    {
                        LoadActiveProviderOrgPlan(ldatEffectedDate);
                    }
                    //GetMonthlyPremiumAmountForMedicarePartD(ldatEffectedDate);
                    //GetPremiumAmountFromRef();
                    icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef = busRateHelper.GetMedicarePartDPremiumAmountFromRef
                        (ldatEffectedDate, idtbCachedHealthRate, iobjPassInfo);

					//PIR 15434
                    string astrSSN = string.Empty;
                    if (aintMemberPersonID != ibusPerson.icdoPerson.person_id)
                    {
                        busPerson abusPersonMember = new busPerson();
                        abusPersonMember.FindPerson(aintMemberPersonID);
                        astrSSN = abusPersonMember.icdoPerson.ssn;
                    }
                    else
                        astrSSN = ibusPerson.icdoPerson.ssn;

                    LoadPlanEffectiveDate();
                    //Low Income Credit Amount should be populated from Ref table. 
                    DataTable adtbCachedLowIncomeCreditRef = busGlobalFunctions.LoadLowIncomeCreditRefCacheData(iobjPassInfo);
                    Decimal ldecLowIncomeCreditAmount = 0;
                    var lenumList = adtbCachedLowIncomeCreditRef.AsEnumerable().Where(i => i.Field<Decimal>("low_income_credit") == icdoPersonAccountMedicarePartDHistory.low_income_credit).OrderByDescending(i => i.Field<DateTime>("effective_date"));
                    foreach (DataRow dr in lenumList)
                    {
                        if (Convert.ToDateTime(dr["effective_date"]).Date <= ldatEffectedDate.Date)
                        {
                            ldecLowIncomeCreditAmount = Convert.ToDecimal(dr["amount"]);
                            break;
                        }
                    }
                    if (icolPosNegEmployerPayrollDtl.IsNull())
                        icolPosNegEmployerPayrollDtl = new Collection<busEmployerPayrollDetail>();
                    idecCalculatedPremiumAmount = icdoPersonAccountMedicarePartDHistory.MonthlyPremiumAmountFromRef + icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty - ldecLowIncomeCreditAmount;

                    if (idecCalculatedPremiumAmount >= 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, idecCalculatedPremiumAmount, busConstant.PayrollDetailRecordTypePositiveAdjustment))
                    {
                        lbusPayrollHdr.UpdateEmployerPayrollDetailStatusReviewAndValidToIgnore(icdoPersonAccount.person_id, icdoPersonAccount.plan_id, ldatEffectedDate);
                        lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(aintMemberPersonID, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                            ibusPerson.icdoPerson.last_name, astrSSN, busConstant.PayrollDetailRecordTypePositiveAdjustment,
                            ldatEffectedDate, idecCalculatedPremiumAmount, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                            ldecLowIncomeCreditAmount, icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty,
                            aintPersonAccountID: icdoPersonAccount.person_account_id, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);// Org to bill
                    }
                    else if (idecCalculatedPremiumAmount < 0 && IsPayrollDtlAdjustmentExistsPriorToEnteredEffectiveDate(ibusProviderOrgPlan.icdoOrgPlan.org_id, ldatEffectedDate, idecCalculatedPremiumAmount * -1, busConstant.PayrollDetailRecordTypeNegativeAdjustment))
                    {
                        lbusPayrollHdr.AddInsuranceAdjustmentPayrollDetail(aintMemberPersonID, icdoPersonAccount.plan_id, ibusPerson.icdoPerson.first_name,
                            ibusPerson.icdoPerson.last_name, astrSSN, busConstant.PayrollDetailRecordTypeNegativeAdjustment,
                            ldatEffectedDate, idecCalculatedPremiumAmount * -1, ibusProviderOrgPlan.icdoOrgPlan.org_id,
                            ldecLowIncomeCreditAmount * -1, icdoPersonAccountMedicarePartDHistory.late_enrollment_penalty * -1,
                            aintPersonAccountID: icdoPersonAccount.person_account_id, acolPosNegEmployerPayrollDtl: icolPosNegEmployerPayrollDtl);// Org to bill
                    }                
                    ldatEffectedDate = ldatEffectedDate.AddMonths(1);
                }
            }
            Collection<busEmployerPayrollDetail> lcolFinal = ComparePositiveNegativeColEmployerPayrollDetail();
            if (lbusPayrollHdr != null && (lcolFinal != null) && (lcolFinal.Count > 0))
            {
                if(lbusPayrollHdr != null)
                    lbusPayrollHdr.UpdateDataObject(lbusPayrollHdr.icdoEmployerPayrollHeader);
                foreach (busEmployerPayrollDetail lbusPayrollDtl in lcolFinal)
                {
                    if (lbusPayrollHdr != null && lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id == 0)
                        lbusPayrollDtl.icdoEmployerPayrollDetail.employer_payroll_header_id = lbusPayrollHdr.icdoEmployerPayrollHeader.employer_payroll_header_id;
                    lbusPayrollDtl.UpdateDataObject(lbusPayrollDtl.icdoEmployerPayrollDetail);
                }
            }
        }

        public bool DoesTheMemberHavePermAddress()
        {
            if(icdoPersonAccountMedicarePartDHistory.member_person_id > 0)
            {
                busPerson lbusperson = new busPerson();
                if(lbusperson.FindPerson(icdoPersonAccountMedicarePartDHistory.member_person_id))
                {
                    lbusperson.LoadPersonAddress();
                    if (!lbusperson.iclbPersonAddress.Any(address => address.icdoPersonAddress.address_type_value == busConstant.AddressTypePermanent && 
                                                                     address.icdoPersonAddress.end_date == DateTime.MinValue))
                        return false;
                }
            }
            return true;
        }
    }
}
